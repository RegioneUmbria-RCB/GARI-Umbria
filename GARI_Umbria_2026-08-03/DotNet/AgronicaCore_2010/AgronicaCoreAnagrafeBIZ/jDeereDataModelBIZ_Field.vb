Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreEntityFramework_POCO
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class jDeereDataModelBIZ_Field_W
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


    Public Function Aggiorna_jDeereDataModel_Field(
            ByVal righeInserite As String,
            ByVal righeModificate As String,
            ByVal righeCancellate As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As String


        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty

        Dim NomeRoutine As String = "jDeereDataModel_Field_W.Aggiorna_jDeereDataModel_Field_W()"

        Try
            Dim campConf_R As New jDeereDataModelDAL_Field_R
            Dim linkClient_R As New jDeereDataModelDAL_Client_R
            Dim linkFarm_R As New jDeereDataModelDAL_Farm_R

            Dim curjDeereDataModel_Field As New jDeereDataModel_Field

            Dim righeInseriteArray As JArray = JArray.Parse(righeInserite)
            Dim righeModificateArray As JArray = JArray.Parse(righeModificate)
            Dim righeCancellateArray As JArray = JArray.Parse(righeCancellate)
            Dim EFArrayToInsert As New ArrayList
            Dim EFArrayToUpdate As New ArrayList
            Dim EFArrayToDelete As New ArrayList
            Dim IdCurr As Integer = 0
            Dim idCurrViaGUID As Integer = -1
            Dim refClient As String
            Dim refFarm As String

            Dim refClientAsInt As Integer = -1
            Dim refFarmAsInt As Integer = -1

            For Each obj As JObject In righeInseriteArray
                curjDeereDataModel_Field = New jDeereDataModel_Field
                curjDeereDataModel_Field.ID = 0
                curjDeereDataModel_Field.archived = obj("archived")
                curjDeereDataModel_Field.Name = obj("Name")
                curjDeereDataModel_Field.guid = obj("ID")
                curjDeereDataModel_Field.OrganizationID = obj("OrganizationID")

                refClient = obj("clients").Item("values").Item(0).Item("ID") '  JsonConvert.DeserializeObject(Of List(Of jDeereDataModel_Client))(obj("clients")("values"))
                refFarm = obj("farms").Item("values").Item(0).Item("ID") 'JsonConvert.DeserializeObject(Of List(Of jDeereDataModel_Farm))(obj("farms"))
                curjDeereDataModel_Field.ClientID = linkClient_R.LeggiIDViaGUID(refClient, objParametri)
                curjDeereDataModel_Field.FarmID = linkFarm_R.LeggiIDViaGUID(refFarm, objParametri)

                If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
                    curjDeereDataModel_Field.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio"), Format, Provider)
                Else
                    curjDeereDataModel_Field.Validita_Inizio = ValiditaInizio
                End If
                If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
                    curjDeereDataModel_Field.Validita_Fine = Date.ParseExact(obj("Validita_Fine"), Format, Provider)
                Else
                    curjDeereDataModel_Field.Validita_Fine = ValiditaFine
                End If
                curjDeereDataModel_Field.Data_Creazione = Date.Now
                curjDeereDataModel_Field.Username_Creazione = objParametri.UsernameOperazione
                curjDeereDataModel_Field.Data_Modifica = Date.Now
                curjDeereDataModel_Field.Username_Modifica = objParametri.UsernameOperazione
                curjDeereDataModel_Field.inviato = 0

                EFArrayToInsert.Add(curjDeereDataModel_Field)
            Next

            For Each obj As JObject In righeModificateArray
                idCurrViaGUID = campConf_R.LeggiIDViaGUID(obj("ID"), objParametri)
                IdCurr = If(idCurrViaGUID <> -1, idCurrViaGUID, obj("ID"))
                curjDeereDataModel_Field = campConf_R.Leggi(IdCurr, objParametri)

                If curjDeereDataModel_Field Is Nothing Then
                    MessaggioErrore &= "Riga da aggiornare " & obj("Validita_Inizio").ToString & " - " & obj("Validita_Fine").ToString & " non trovata"
                Else

                
                    curjDeereDataModel_Field.archived = obj("archived")
                    curjDeereDataModel_Field.Name = obj("Name")
                    curjDeereDataModel_Field.OrganizationID = obj("OrganizationID")
                    If obj("guid") Is Nothing Then
                        curjDeereDataModel_Field.guid = obj("ID")
                    Else
                        curjDeereDataModel_Field.guid = obj("guid")
                    End If


                    'in caso di aggiornamento devo verificare che nel campo ID dell'oggetto di riferimento sia castabile in intero

                    refClient = obj("clients").Item("values").Item(0).Item("ID")
                    refFarm = obj("farms").Item("values").Item(0).Item("ID")
                    If Integer.TryParse(refClient, refClientAsInt) = True Then
                        curjDeereDataModel_Field.ClientID = refClientAsInt
                    Else
                        curjDeereDataModel_Field.ClientID = linkClient_R.LeggiIDViaGUID(refClient, objParametri)
                    End If
                    If Integer.TryParse(refFarm, refFarmAsInt) = True Then
                        curjDeereDataModel_Field.FarmID = refFarmAsInt
                    Else
                        curjDeereDataModel_Field.FarmID = linkFarm_R.LeggiIDViaGUID(refFarm, objParametri)
                    End If


                    If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
                        curjDeereDataModel_Field.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio"), Format, Provider)
                    Else
                        curjDeereDataModel_Field.Validita_Fine = ValiditaFine
                    End If
                    If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
                        curjDeereDataModel_Field.Validita_Fine = Date.ParseExact(obj("Validita_Fine"), Format, Provider)
                    Else
                        curjDeereDataModel_Field.Validita_Fine = ValiditaFine
                    End If
                    curjDeereDataModel_Field.Data_Modifica = Date.Now
                    curjDeereDataModel_Field.Username_Modifica = objParametri.UsernameOperazione
                    EFArrayToUpdate.Add(curjDeereDataModel_Field)

                End If
            Next
            For Each obj As JObject In righeCancellateArray
                IdCurr = campConf_R.LeggiIDViaGUID(obj("ID"), objParametri)
                curjDeereDataModel_Field = New jDeereDataModel_Field
                curjDeereDataModel_Field.ID = IdCurr
                EFArrayToDelete.Add(curjDeereDataModel_Field)
            Next


            If String.IsNullOrEmpty(MessaggioErrore) Then
                Dim campConf_W As New jDeereDataModelDAL_Field_W

                MessaggioErrore = campConf_W.Aggiorna_jDeereDataModel_Field(
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
