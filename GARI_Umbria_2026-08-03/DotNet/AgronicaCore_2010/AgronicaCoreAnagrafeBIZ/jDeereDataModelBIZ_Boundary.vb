

Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreAnagrafeDAL
Imports Newtonsoft.Json.Linq

Public Class jDeereDataModelBIZ_Boundary_W
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


    Public Function Aggiorna_jDeereDataModel_Boundary(
            ByVal righeInserite As String,
            ByVal righeModificate As String,
            ByVal righeCancellate As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As String


        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty

        Dim NomeRoutine As String = "jDeereDataModel_Boundary_W.Aggiorna_jDeereDataModel_Boundary_W()"

        Try
            Dim campConf_R As New jDeereDataModelDAL_Boundary_R

            Dim curjDeereDataModel_Boundary As New jDeereDataModel_Boundary

            Dim righeInseriteArray As JArray = JArray.Parse(righeInserite)
            Dim righeModificateArray As JArray = JArray.Parse(righeModificate)
            Dim righeCancellateArray As JArray = JArray.Parse(righeCancellate)
            Dim EFArrayToInsert As New ArrayList
            Dim EFArrayToUpdate As New ArrayList
            Dim EFArrayToDelete As New ArrayList
            Dim IdCurr As Integer = 0
            Dim idCurrViaGUID As Integer = -1

            For Each obj As JObject In righeInseriteArray
                curjDeereDataModel_Boundary = New jDeereDataModel_Boundary
                curjDeereDataModel_Boundary.ID = 0
                curjDeereDataModel_Boundary.guid = obj("ID")
                curjDeereDataModel_Boundary.Name = obj("Name")
                curjDeereDataModel_Boundary.sourceType = obj("sourceType")
                curjDeereDataModel_Boundary.area = obj("area")("valueAsDouble")
                If obj("workableArea") IsNot Nothing Then
                    curjDeereDataModel_Boundary.workableArea = obj("workableArea")("valueAsDouble")
                    curjDeereDataModel_Boundary.workableArea_unit = obj("workableArea")("unit")
                Else
                    curjDeereDataModel_Boundary.workableArea = 0
                    curjDeereDataModel_Boundary.workableArea_unit = ""
                End If
                curjDeereDataModel_Boundary.active = obj("active")
                curjDeereDataModel_Boundary.irrigated = obj("irrigated")
                curjDeereDataModel_Boundary.type = ""
                curjDeereDataModel_Boundary.irrigated = obj("irrigated")
                curjDeereDataModel_Boundary.area_unit = obj("area")("unit")

                curjDeereDataModel_Boundary.boundaryData = obj.ToString()

                If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
                    curjDeereDataModel_Boundary.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio"), Format, Provider)
                Else
                    curjDeereDataModel_Boundary.Validita_Inizio = ValiditaInizio
                End If
                If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
                    curjDeereDataModel_Boundary.Validita_Fine = Date.ParseExact(obj("Validita_Fine"), Format, Provider)
                Else
                    curjDeereDataModel_Boundary.Validita_Fine = ValiditaFine
                End If
                curjDeereDataModel_Boundary.Data_Creazione = Date.Now
                curjDeereDataModel_Boundary.Username_Creazione = objParametri.UsernameOperazione
                curjDeereDataModel_Boundary.Data_Modifica = Date.Now
                curjDeereDataModel_Boundary.Username_Modifica = objParametri.UsernameOperazione
                curjDeereDataModel_Boundary.inviato = 0

                EFArrayToInsert.Add(curjDeereDataModel_Boundary)
            Next

            For Each obj As JObject In righeModificateArray
                idCurrViaGUID = campConf_R.LeggiIDViaGUID(obj("ID"), objParametri)
                IdCurr = If(idCurrViaGUID <> -1, idCurrViaGUID, obj("ID"))
                curjDeereDataModel_Boundary = campConf_R.Leggi(IdCurr, 0, objParametri)

                If curjDeereDataModel_Boundary Is Nothing Then
                    MessaggioErrore &= "Riga da aggiornare " & obj("Validita_Inizio").ToString & " - " & obj("Validita_Fine").ToString & " non trovata"
                Else

                    curjDeereDataModel_Boundary.guid = If(obj("guid") Is Nothing, obj("ID"), obj("guid"))
                    curjDeereDataModel_Boundary.Name = obj("Name")
                    curjDeereDataModel_Boundary.sourceType = obj("sourceType")
                    curjDeereDataModel_Boundary.area = obj("area")("valueAsDouble")
                    curjDeereDataModel_Boundary.active = obj("active")
                    curjDeereDataModel_Boundary.irrigated = obj("irrigated")
                    curjDeereDataModel_Boundary.type = ""
                    curjDeereDataModel_Boundary.irrigated = obj("irrigated")
                    curjDeereDataModel_Boundary.area_unit = obj("area")("unit")
                    If obj("workableArea") IsNot Nothing Then
                        curjDeereDataModel_Boundary.workableArea = obj("workableArea")("valueAsDouble")
                        curjDeereDataModel_Boundary.workableArea_unit = obj("workableArea")("unit")
                    End If

                    curjDeereDataModel_Boundary.boundaryData = obj.ToString()


                    If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
                        curjDeereDataModel_Boundary.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio"), Format, Provider)
                    Else
                        curjDeereDataModel_Boundary.Validita_Fine = ValiditaFine
                    End If
                    If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
                        curjDeereDataModel_Boundary.Validita_Fine = Date.ParseExact(obj("Validita_Fine"), Format, Provider)
                    Else
                        curjDeereDataModel_Boundary.Validita_Fine = ValiditaFine
                    End If
                    curjDeereDataModel_Boundary.Data_Modifica = Date.Now
                    curjDeereDataModel_Boundary.Username_Modifica = objParametri.UsernameOperazione
                    EFArrayToUpdate.Add(curjDeereDataModel_Boundary)

                End If
            Next
            For Each obj As JObject In righeCancellateArray
                curjDeereDataModel_Boundary = New jDeereDataModel_Boundary
                IdCurr = campConf_R.LeggiIDViaGUID(obj("ID"), objParametri)
                curjDeereDataModel_Boundary.ID = IdCurr
                EFArrayToDelete.Add(curjDeereDataModel_Boundary)
            Next


            If String.IsNullOrEmpty(MessaggioErrore) Then
                Dim campConf_W As New jDeereDataModelDAL_Boundary_W

                MessaggioErrore = campConf_W.Aggiorna_jDeereDataModel_Boundary(
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