Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreEntityFramework_POCO
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class jDeereDataModelBIZ_FieldOperationMeasurement_W
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

    Public Function Aggiorna_jDeereDataModelBIZ_FieldOperationMeasurement(
            ByVal righeInserite As String,
            ByVal righeModificate As String,
            ByVal righeCancellate As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As String


        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty

        Dim NomeRoutine As String = "Aggiorna_jDeereDataModelBIZ_FieldOperationMeasurement_W.Aggiorna_jDeereDataModelBIZ_FieldOperationMeasurement()"

        Try
            Dim jDRead As New jDeereDataModelDAL_FieldOperationMeasurement_R

            Dim curjDeereDataModelBIZ_FieldOperationMeasurement As New jDeereDataModel_FieldOperationMeasurement

            Dim righeInseriteArray As JArray = JArray.Parse(righeInserite)
            Dim righeModificateArray As JArray = JArray.Parse(righeModificate)
            Dim righeCancellateArray As JArray = JArray.Parse(righeCancellate)
            Dim EFArrayToInsert As New ArrayList
            Dim EFArrayToUpdate As New ArrayList
            Dim EFArrayToDelete As New ArrayList

            For Each obj As JObject In righeInseriteArray
                curjDeereDataModelBIZ_FieldOperationMeasurement = JsonConvert.DeserializeObject(Of jDeereDataModel_FieldOperationMeasurement)(obj.ToString())

                If (obj("measurementName") Is Nothing) Then
                    curjDeereDataModelBIZ_FieldOperationMeasurement.measurementName = ""
                Else
                    curjDeereDataModelBIZ_FieldOperationMeasurement.measurementName = obj("measurementName").ToString()
                End If
                If (obj("measurementCategory") Is Nothing) Then
                    curjDeereDataModelBIZ_FieldOperationMeasurement.measurementCategory = ""
                Else
                    curjDeereDataModelBIZ_FieldOperationMeasurement.measurementCategory = obj("measurementCategory").ToString()
                End If
                If (obj("area") Is Nothing) Then
                    curjDeereDataModelBIZ_FieldOperationMeasurement.area = 0
                Else
                    curjDeereDataModelBIZ_FieldOperationMeasurement.area = Decimal.Parse(obj("area").ToString())
                End If
                If (obj("area_unit") Is Nothing) Then
                    curjDeereDataModelBIZ_FieldOperationMeasurement.area_unit = ""
                Else
                    curjDeereDataModelBIZ_FieldOperationMeasurement.area_unit = obj("area_unit").ToString()
                End If

                If (obj("totalMaterial") Is Nothing) Then
                    curjDeereDataModelBIZ_FieldOperationMeasurement.totalMaterial = 0
                Else
                    curjDeereDataModelBIZ_FieldOperationMeasurement.totalMaterial = Decimal.Parse(obj("totalMaterial").ToString())
                End If
                If (obj("totalMaterial_unit") Is Nothing) Then
                    curjDeereDataModelBIZ_FieldOperationMeasurement.totalMaterial_unit = ""
                Else
                    curjDeereDataModelBIZ_FieldOperationMeasurement.totalMaterial_unit = obj("totalMaterial_unit").ToString()
                End If

                If (obj("averageMaterial") Is Nothing) Then
                    curjDeereDataModelBIZ_FieldOperationMeasurement.averageMaterial = 0
                Else
                    curjDeereDataModelBIZ_FieldOperationMeasurement.averageMaterial = Decimal.Parse(obj("averageMaterial").ToString())
                End If
                If (obj("averageMaterial_unit") Is Nothing) Then
                    curjDeereDataModelBIZ_FieldOperationMeasurement.averageMaterial_unit = ""
                Else
                    curjDeereDataModelBIZ_FieldOperationMeasurement.averageMaterial_unit = obj("averageMaterial_unit").ToString()
                End If

                If (obj("yield") Is Nothing) Then
                    curjDeereDataModelBIZ_FieldOperationMeasurement.yield = 0
                Else
                    curjDeereDataModelBIZ_FieldOperationMeasurement.yield = Decimal.Parse(obj("yield").ToString())
                End If
                If (obj("yield_unit") Is Nothing) Then
                    curjDeereDataModelBIZ_FieldOperationMeasurement.yield_unit = ""
                Else
                    curjDeereDataModelBIZ_FieldOperationMeasurement.yield_unit = obj("yield_unit").ToString()
                End If

                If (obj("averageYield") Is Nothing) Then
                    curjDeereDataModelBIZ_FieldOperationMeasurement.averageYield = 0
                Else
                    curjDeereDataModelBIZ_FieldOperationMeasurement.averageYield = Decimal.Parse(obj("averageYield").ToString())
                End If
                If (obj("averageYield_unit") Is Nothing) Then
                    curjDeereDataModelBIZ_FieldOperationMeasurement.averageYield_unit = ""
                Else
                    curjDeereDataModelBIZ_FieldOperationMeasurement.averageYield_unit = obj("averageYield_unit").ToString()
                End If

                If (obj("averageMistoure") Is Nothing) Then
                    curjDeereDataModelBIZ_FieldOperationMeasurement.averageMistoure = 0
                Else
                    curjDeereDataModelBIZ_FieldOperationMeasurement.averageMistoure = Decimal.Parse(obj("averageMistoure").ToString())
                End If
                If (obj("averageMistoure_unit") Is Nothing) Then
                    curjDeereDataModelBIZ_FieldOperationMeasurement.averageMistoure_unit = ""
                Else
                    curjDeereDataModelBIZ_FieldOperationMeasurement.averageMistoure_unit = obj("averageMistoure_unit").ToString()
                End If

                If (obj("wetMass") Is Nothing) Then
                    curjDeereDataModelBIZ_FieldOperationMeasurement.wetMass = 0
                Else
                    curjDeereDataModelBIZ_FieldOperationMeasurement.wetMass = Decimal.Parse(obj("wetMass").ToString())
                End If
                If (obj("wetMass_unit") Is Nothing) Then
                    curjDeereDataModelBIZ_FieldOperationMeasurement.wetMass_unit = ""
                Else
                    curjDeereDataModelBIZ_FieldOperationMeasurement.wetMass_unit = obj("wetMass_unit").ToString()
                End If

                If (obj("averageWetMass") Is Nothing) Then
                    curjDeereDataModelBIZ_FieldOperationMeasurement.averageWetMass = 0
                Else
                    curjDeereDataModelBIZ_FieldOperationMeasurement.averageWetMass = Decimal.Parse(obj("averageWetMass").ToString())
                End If
                If (obj("averageWetMass_unit") Is Nothing) Then
                    curjDeereDataModelBIZ_FieldOperationMeasurement.averageWetMass_unit = ""
                Else
                    curjDeereDataModelBIZ_FieldOperationMeasurement.averageWetMass_unit = obj("averageWetMass_unit").ToString()
                End If

                If (obj("averageSpeed") Is Nothing) Then
                    curjDeereDataModelBIZ_FieldOperationMeasurement.averageSpeed = 0
                Else
                    curjDeereDataModelBIZ_FieldOperationMeasurement.averageSpeed = Decimal.Parse(obj("averageSpeed").ToString())
                End If
                If (obj("averageSpeed_unit") Is Nothing) Then
                    curjDeereDataModelBIZ_FieldOperationMeasurement.averageSpeed_unit = ""
                Else
                    curjDeereDataModelBIZ_FieldOperationMeasurement.averageSpeed_unit = obj("averageSpeed_unit").ToString()
                End If

                curjDeereDataModelBIZ_FieldOperationMeasurement.measurementData = obj.ToString()

                curjDeereDataModelBIZ_FieldOperationMeasurement.FieldOperationID = obj("FieldOperationID")

                If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
                    curjDeereDataModelBIZ_FieldOperationMeasurement.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio"), Format, Provider)
                Else
                    curjDeereDataModelBIZ_FieldOperationMeasurement.Validita_Inizio = ValiditaInizio
                End If
                If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
                    curjDeereDataModelBIZ_FieldOperationMeasurement.Validita_Fine = Date.ParseExact(obj("Validita_Fine"), Format, Provider)
                Else
                    curjDeereDataModelBIZ_FieldOperationMeasurement.Validita_Fine = ValiditaFine
                End If
                curjDeereDataModelBIZ_FieldOperationMeasurement.Data_Creazione = Date.Now
                curjDeereDataModelBIZ_FieldOperationMeasurement.Username_Creazione = objParametri.UsernameOperazione
                curjDeereDataModelBIZ_FieldOperationMeasurement.Data_Modifica = Date.Now
                curjDeereDataModelBIZ_FieldOperationMeasurement.Username_Modifica = objParametri.UsernameOperazione
                curjDeereDataModelBIZ_FieldOperationMeasurement.inviato = 0

                EFArrayToInsert.Add(curjDeereDataModelBIZ_FieldOperationMeasurement)
            Next

            For Each obj As JObject In righeModificateArray
                curjDeereDataModelBIZ_FieldOperationMeasurement = jDRead.Leggi(Integer.Parse(obj("ID")), objParametri)

                If curjDeereDataModelBIZ_FieldOperationMeasurement Is Nothing Then
                    MessaggioErrore &= "Riga da aggiornare " & obj("Validita_Inizio").ToString & " - " & obj("Validita_Fine").ToString & " non trovata"
                Else

                    If (obj("measurementName") IsNot Nothing) Then
                        curjDeereDataModelBIZ_FieldOperationMeasurement.measurementName = obj("measurementName").ToString()
                    End If
                    If (obj("measurementCategory") IsNot Nothing) Then
                        curjDeereDataModelBIZ_FieldOperationMeasurement.measurementCategory = obj("measurementCategory").ToString()
                    End If
                    If (obj("area") IsNot Nothing) Then
                        curjDeereDataModelBIZ_FieldOperationMeasurement.area = Decimal.Parse(obj("area").ToString())
                    End If
                    If (obj("area_unit") IsNot Nothing) Then
                        curjDeereDataModelBIZ_FieldOperationMeasurement.area_unit = obj("area_unit").ToString()
                    End If

                    If (obj("totalMaterial") IsNot Nothing) Then
                        curjDeereDataModelBIZ_FieldOperationMeasurement.totalMaterial = Decimal.Parse(obj("totalMaterial").ToString())
                    End If
                    If (obj("totalMaterial_unit") IsNot Nothing) Then
                        curjDeereDataModelBIZ_FieldOperationMeasurement.totalMaterial_unit = obj("totalMaterial_unit").ToString()
                    End If
                    If (obj("averageMaterial") IsNot Nothing) Then
                        curjDeereDataModelBIZ_FieldOperationMeasurement.averageMaterial = Decimal.Parse(obj("averageMaterial").ToString())
                    End If
                    If (obj("averageMaterial_unit") IsNot Nothing) Then
                        curjDeereDataModelBIZ_FieldOperationMeasurement.averageMaterial_unit = obj("averageMaterial_unit").ToString()
                    End If
                    If (obj("yield") IsNot Nothing) Then
                        curjDeereDataModelBIZ_FieldOperationMeasurement.yield = Decimal.Parse(obj("yield").ToString())
                    End If
                    If (obj("yield_unit") IsNot Nothing) Then
                        curjDeereDataModelBIZ_FieldOperationMeasurement.yield_unit = obj("yield_unit").ToString()
                    End If
                    If (obj("averageYield") IsNot Nothing) Then
                        curjDeereDataModelBIZ_FieldOperationMeasurement.averageYield = Decimal.Parse(obj("averageYield").ToString())
                    End If
                    If (obj("averageYield_unit") IsNot Nothing) Then
                        curjDeereDataModelBIZ_FieldOperationMeasurement.averageYield_unit = obj("averageYield_unit").ToString()
                    End If
                    If (obj("averageMistoure") IsNot Nothing) Then
                        curjDeereDataModelBIZ_FieldOperationMeasurement.averageMistoure = Decimal.Parse(obj("averageMistoure").ToString())
                    End If
                    If (obj("averageMistoure_unit") IsNot Nothing) Then
                        curjDeereDataModelBIZ_FieldOperationMeasurement.averageMistoure_unit = obj("averageMistoure_unit").ToString()
                    End If
                    If (obj("wetMass") IsNot Nothing) Then
                        curjDeereDataModelBIZ_FieldOperationMeasurement.wetMass = Decimal.Parse(obj("wetMass").ToString())
                    End If
                    If (obj("wetMass_unit") IsNot Nothing) Then
                        curjDeereDataModelBIZ_FieldOperationMeasurement.wetMass_unit = obj("wetMass_unit").ToString()
                    End If
                    If (obj("averageWetMass") IsNot Nothing) Then
                        curjDeereDataModelBIZ_FieldOperationMeasurement.averageWetMass = Decimal.Parse(obj("averageWetMass").ToString())
                    End If
                    If (obj("averageWetMass_unit") IsNot Nothing) Then
                        curjDeereDataModelBIZ_FieldOperationMeasurement.averageWetMass_unit = obj("averageWetMass_unit").ToString()
                    End If
                    If (obj("averageSpeed") IsNot Nothing) Then
                        curjDeereDataModelBIZ_FieldOperationMeasurement.averageSpeed = Decimal.Parse(obj("averageSpeed").ToString())
                    End If
                    If (obj("averageSpeed_unit") IsNot Nothing) Then
                        curjDeereDataModelBIZ_FieldOperationMeasurement.averageSpeed_unit = obj("averageSpeed_unit").ToString()
                    End If

                    curjDeereDataModelBIZ_FieldOperationMeasurement.measurementData = obj.ToString()


                    If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
                        curjDeereDataModelBIZ_FieldOperationMeasurement.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio"), Format, Provider)
                    Else
                        curjDeereDataModelBIZ_FieldOperationMeasurement.Validita_Fine = ValiditaFine
                    End If
                    If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
                        curjDeereDataModelBIZ_FieldOperationMeasurement.Validita_Fine = Date.ParseExact(obj("Validita_Fine"), Format, Provider)
                    Else
                        curjDeereDataModelBIZ_FieldOperationMeasurement.Validita_Fine = ValiditaFine
                    End If
                    curjDeereDataModelBIZ_FieldOperationMeasurement.Data_Modifica = Date.Now
                    curjDeereDataModelBIZ_FieldOperationMeasurement.Username_Modifica = objParametri.UsernameOperazione
                    EFArrayToUpdate.Add(curjDeereDataModelBIZ_FieldOperationMeasurement)

                End If
            Next
            For Each obj As JObject In righeCancellateArray
                curjDeereDataModelBIZ_FieldOperationMeasurement = New jDeereDataModel_FieldOperationMeasurement
                curjDeereDataModelBIZ_FieldOperationMeasurement.ID = obj("ID")
                EFArrayToDelete.Add(curjDeereDataModelBIZ_FieldOperationMeasurement)
            Next


            If String.IsNullOrEmpty(MessaggioErrore) Then
                Dim campConf_W As New jDeereDataModelDAL_FieldOperationMeasurement_W

                MessaggioErrore = campConf_W.Aggiorna_jDeereDataModel_FieldOperationMeasurement(
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
