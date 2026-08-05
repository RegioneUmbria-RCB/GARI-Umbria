Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreAnagrafeDAL
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Public Class jDeereDataModelBIZ_FieldOperation_W
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

    Public Function Aggiorna_jDeereDataModelBIZ_FieldOperation(
            ByVal righeInserite As String,
            ByVal righeModificate As String,
            ByVal righeCancellate As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As String


        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty

        Dim NomeRoutine As String = "jDeereDataModelBIZ_FieldOperation_W.Aggiorna_jDeereDataModelBIZ_FieldOperation()"

        Try
            Dim jDRead As New jDeereDataModelDAL_FieldOperation_R

            Dim curjDeereDataModelBIZ_FieldOperation As New jDeereDataModel_FieldOperation

            Dim righeInseriteArray As JArray = JArray.Parse(righeInserite)
            Dim righeModificateArray As JArray = JArray.Parse(righeModificate)
            Dim righeCancellateArray As JArray = JArray.Parse(righeCancellate)
            Dim EFArrayToInsert As New ArrayList
            Dim EFArrayToUpdate As New ArrayList
            Dim EFArrayToDelete As New ArrayList
            Dim IdCurr As Integer = 0
            Dim idCurrViaGUID As Integer = -1

            For Each obj As JObject In righeInseriteArray
                curjDeereDataModelBIZ_FieldOperation = JsonConvert.DeserializeObject(Of jDeereDataModel_FieldOperation)(obj.ToString())
                'curjDeereDataModelBIZ_FieldOperation.guid = curjDeereDataModelBIZ_FieldOperation.ID.ToString()
                If (obj("adaptMachineType") Is Nothing) Then
                    curjDeereDataModelBIZ_FieldOperation.adaptMachineType = ""
                Else
                    curjDeereDataModelBIZ_FieldOperation.adaptMachineType = obj("adaptMachineType").ToString()
                End If
                If (obj("cropSeason") Is Nothing) Then
                    curjDeereDataModelBIZ_FieldOperation.cropSeason = ""
                Else
                    curjDeereDataModelBIZ_FieldOperation.cropSeason = obj("cropSeason").ToString()
                End If
                If Not String.IsNullOrEmpty(obj("startDate")) Then
                    Dim dat As DateTime

                    If Date.TryParseExact(obj("startDate"), Format, Provider, Globalization.DateTimeStyles.AdjustToUniversal, dat) = True Then
                        curjDeereDataModelBIZ_FieldOperation.startDate = dat
                    Else
                        curjDeereDataModelBIZ_FieldOperation.startDate = obj("startDate")
                    End If
                    'curjDeereDataModelBIZ_FieldOperation.startDate = Date.TryParseExact(obj("startDate"), Format, Provider)
                End If
                If Not String.IsNullOrEmpty(obj("endDate")) Then
                    Dim dat As DateTime

                    If Date.TryParseExact(obj("endDate"), Format, Provider, Globalization.DateTimeStyles.AdjustToUniversal, dat) = True Then
                        curjDeereDataModelBIZ_FieldOperation.endDate = dat
                    Else
                        curjDeereDataModelBIZ_FieldOperation.endDate = obj("endDate")
                    End If
                    'curjDeereDataModelBIZ_FieldOperation.endDate = Date.ParseExact(obj("endDate"), Format, Provider)
                End If

                curjDeereDataModelBIZ_FieldOperation.operationData = obj.ToString()
                If obj("OrganizationID") IsNot Nothing Then
                    curjDeereDataModelBIZ_FieldOperation.OrganizationID = obj("OrganizationID")
                End If
                If obj("FieldID") IsNot Nothing Then
                    curjDeereDataModelBIZ_FieldOperation.FieldID = obj("FieldID")
                End If


                If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
                    curjDeereDataModelBIZ_FieldOperation.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio"), Format, Provider)
                Else
                    curjDeereDataModelBIZ_FieldOperation.Validita_Inizio = ValiditaInizio
                End If
                If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
                    curjDeereDataModelBIZ_FieldOperation.Validita_Fine = Date.ParseExact(obj("Validita_Fine"), Format, Provider)
                Else
                    curjDeereDataModelBIZ_FieldOperation.Validita_Fine = ValiditaFine
                End If

                curjDeereDataModelBIZ_FieldOperation.Allegati_Documenti_Cod = -1  'per evitare che faccia join con indicixentità che hanno rif a 0

                curjDeereDataModelBIZ_FieldOperation.Data_Creazione = Date.Now
                curjDeereDataModelBIZ_FieldOperation.Username_Creazione = objParametri.UsernameOperazione
                curjDeereDataModelBIZ_FieldOperation.Data_Modifica = Date.Now
                curjDeereDataModelBIZ_FieldOperation.Username_Modifica = objParametri.UsernameOperazione
                curjDeereDataModelBIZ_FieldOperation.inviato = 0

                EFArrayToInsert.Add(curjDeereDataModelBIZ_FieldOperation)
            Next
            For Each obj As JObject In righeModificateArray
                idCurrViaGUID = jDRead.LeggiIDViaGUID(obj("guid"), objParametri)
                IdCurr = IIf(idCurrViaGUID <> -1, idCurrViaGUID, obj("ID"))
                curjDeereDataModelBIZ_FieldOperation = jDRead.Leggi(IdCurr, objParametri)

                If (obj("adaptMachineType") IsNot Nothing) Then
                    curjDeereDataModelBIZ_FieldOperation.adaptMachineType = obj("adaptMachineType").ToString()
                End If
                If (obj("cropSeason") IsNot Nothing) Then
                    curjDeereDataModelBIZ_FieldOperation.cropSeason = obj("cropSeason").ToString()
                End If
                If Not String.IsNullOrEmpty(obj("startDate")) Then
                    Dim dat As DateTime

                    If Date.TryParseExact(obj("startDate"), Format, Provider, Globalization.DateTimeStyles.AdjustToUniversal, dat) = True Then
                        curjDeereDataModelBIZ_FieldOperation.startDate = dat
                    Else
                        curjDeereDataModelBIZ_FieldOperation.startDate = obj("startDate")
                    End If
                    'curjDeereDataModelBIZ_FieldOperation.startDate = Date.TryParseExact(obj("startDate"), Format, Provider)
                End If
                If Not String.IsNullOrEmpty(obj("endDate")) Then
                    Dim dat As DateTime

                    If Date.TryParseExact(obj("endDate"), Format, Provider, Globalization.DateTimeStyles.AdjustToUniversal, dat) = True Then
                        curjDeereDataModelBIZ_FieldOperation.endDate = dat
                    Else
                        curjDeereDataModelBIZ_FieldOperation.endDate = obj("endDate")
                    End If
                    'curjDeereDataModelBIZ_FieldOperation.endDate = Date.ParseExact(obj("endDate"), Format, Provider)
                End If

                curjDeereDataModelBIZ_FieldOperation.operationData = obj.ToString()
                If obj("OrganizationID") IsNot Nothing Then
                    curjDeereDataModelBIZ_FieldOperation.OrganizationID = obj("OrganizationID")
                End If
                If obj("FieldID") IsNot Nothing Then
                    curjDeereDataModelBIZ_FieldOperation.FieldID = obj("FieldID")
                End If

                If obj("Allegati_Documenti_Cod") IsNot Nothing And Integer.Parse(obj("Allegati_Documenti_Cod")) > 0 Then
                    curjDeereDataModelBIZ_FieldOperation.Allegati_Documenti_Cod = Integer.Parse(obj("Allegati_Documenti_Cod"))
                End If

                If curjDeereDataModelBIZ_FieldOperation Is Nothing Then
                    MessaggioErrore += "Riga da aggiornare " & obj("Validita_Inizio").ToString & " - " & obj("Validita_Fine").ToString & " non trovata"
                Else

                    If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
                        curjDeereDataModelBIZ_FieldOperation.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio"), Format, Provider)
                    Else
                        curjDeereDataModelBIZ_FieldOperation.Validita_Fine = ValiditaFine
                    End If
                    If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
                        curjDeereDataModelBIZ_FieldOperation.Validita_Fine = Date.ParseExact(obj("Validita_Fine"), Format, Provider)
                    Else
                        curjDeereDataModelBIZ_FieldOperation.Validita_Fine = ValiditaFine
                    End If
                    curjDeereDataModelBIZ_FieldOperation.Data_Modifica = Date.Now
                    curjDeereDataModelBIZ_FieldOperation.Username_Modifica = objParametri.UsernameOperazione
                    EFArrayToUpdate.Add(curjDeereDataModelBIZ_FieldOperation)

                End If
            Next
            For Each obj As JObject In righeCancellateArray
                curjDeereDataModelBIZ_FieldOperation = New jDeereDataModel_FieldOperation
                curjDeereDataModelBIZ_FieldOperation.ID = obj("ID")
                EFArrayToDelete.Add(curjDeereDataModelBIZ_FieldOperation)
            Next


            If String.IsNullOrEmpty(MessaggioErrore) Then
                Dim campConf_W As New jDeereDataModelDAL_FieldOperation_W

                MessaggioErrore = campConf_W.Aggiorna_jDeereDataModel_FieldOperation(
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
