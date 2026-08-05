
Imports System.IO
Imports Newtonsoft.Json

Namespace Irriframe


    Public Class ImpiantiIrrigabili
        Public CUAA As String
        Public Id_Centro As Integer
        Public Id_Campo As Integer
        Public Veg_Cod As Integer
        Public Cul_Cod As Integer
        Public Centro As String
        Public Campo As String
        Public Impianto As String
        Public Coltura As String
        Public Varieta As String
        Public GruppoVegetale As String
        Public TraFila As Decimal?
        Public SuFila As Decimal?
        Public CondTraFila As String
        Public DataRilievo As DateTime?
        Public DataSemina As DateTime?
        Public DataSeminaPrevista As DateTime?
        Public DataInizioImpianto As DateTime
        Public Sup_Imp As Decimal
        Public Lat As Decimal?
        Public Lng As Decimal?
        <JsonConverter(GetType(StringConverter(Of ObjIndirizzo)))>
        Public Indirizzo As ObjIndirizzo
        Public Irri_Veg_Cod As Integer?
        Public Irri_Veg_Des As String
        Public Irri_Imp_Cod As Integer?
        Public Irri_Imp_Des As String
        <JsonConverter(GetType(StringConverter(Of ObjStazioneMeteo)))>
        Public StazioneMeteo As ObjStazioneMeteo
        <JsonConverter(GetType(StringConverter(Of List(Of ObjIrrigazione))))>
        Public Irrigazioni As List(Of ObjIrrigazione)
        <JsonConverter(GetType(StringConverter(Of List(Of ObjFertilizzazioni))))>
        Public Fertilizzazioni As List(Of ObjFertilizzazioni)
        <JsonConverter(GetType(StringConverter(Of ObjFertiRecipe)))>
        Public FertiRecipe As ObjFertiRecipe
        Public Pendenza As Decimal?
        Public Vigoria As Integer?
        Public Sabbia As Decimal?
        Public Limo As Decimal?
        Public Argilla As Decimal?
        Public GIAS_PIva As String
        Public GIAS_SaCod As Integer
        Public GIAS_Appezza As Integer
        Public GIAS_IdReg As Integer
        Public GIAS_ProgettoCod As Integer
        Public Irri_Plot_Id As String
        Public Irri_Crop_Id As String
        Public Cop_Cod As Integer?
        Public Cop_Cod_Irriframe As Integer?
        Public Mac_Cod As Integer?
        Public Portata As Decimal?


        Public Class ObjIndirizzo
            Public Indirizzo As String
            Public Fraz As String
            Public CAP As String
            Public Comune As String
            Public Prov As String
        End Class

        Public Class ObjStazioneMeteo
            Public Sorgente As Integer
            Public Stazione As Integer
        End Class

        Public Class ObjIrrigazione
            Public DataIrri As DateTime
            Public VolumeMM As Decimal
        End Class

        Public Class ObjFertilizzazioni
            Public TitoloN As Decimal
            Public TitoloP As Decimal
            Public TitoloK As Decimal
            Public DoseKg As Decimal
            Public NomeCommercialeDescri As String
            Public Data As DateTime
        End Class

        Public Class ObjFertiRecipe
            Public N_Ammesso As Decimal
            Public P_Ammesso As Decimal
            Public K_Ammesso As Decimal
        End Class
    End Class


    Public Class ImpiantiIrrigabiliComparer
        Implements IComparer(Of ImpiantiIrrigabili)

        Public Function Compare(x As ImpiantiIrrigabili, y As ImpiantiIrrigabili) As Integer Implements IComparer(Of ImpiantiIrrigabili).Compare

            Dim res = x.Campo.ToUpper().CompareTo(y.Campo.ToUpper())

            If res = 0 Then

                res = x.Impianto.ToUpper().CompareTo(y.Impianto.ToUpper())
            End If

            Return res
        End Function
    End Class


    Public Class ImpiantiXCampiIrrigabiliComparer
        Implements IComparer(Of ImpiantiIrrigabili)

        Public Function Compare(x As ImpiantiIrrigabili, y As ImpiantiIrrigabili) As Integer Implements IComparer(Of ImpiantiIrrigabili).Compare
            'devo gestire un ordine che mi permetta di dare la precedenza agli impianti più ricchi di informazioni

            Dim res = CompareLatLng(x, y)

            If res = 0 Then

                res = CompareDataStart(x, y)

                If res = 0 Then

                    res = CompareTerreno(x, y)

                    If res = 0 Then

                        res = CompareIrriguo(x, y)

                        If res = 0 Then

                            res = CompareSesto(x, y)

                            If res = 0 Then

                                If x.GIAS_Appezza < y.GIAS_Appezza Then

                                    res = -1
                                Else

                                    If x.GIAS_Appezza > y.GIAS_Appezza Then

                                        res = 1
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            End If

            Return res
        End Function

        Private Function CompareLatLng(x As ImpiantiIrrigabili, y As ImpiantiIrrigabili) As Integer

            Dim xValid As Boolean = x.Lat.HasValue AndAlso x.Lng.HasValue
            Dim yValid As Boolean = y.Lat.HasValue AndAlso y.Lng.HasValue

            If xValid Then

                If yValid Then

                    Return 0
                End If

                Return -1
            End If

            If yValid Then
                Return 1
            End If

            Return 0
        End Function

        Private Function CompareDataStart(x As ImpiantiIrrigabili, y As ImpiantiIrrigabili) As Integer

            Dim xDataStart As Date? = If(x.DataRilievo, If(x.DataSemina, If(x.DataSeminaPrevista, Nothing)))
            Dim yDataStart As Date? = If(y.DataRilievo, If(y.DataSemina, If(y.DataSeminaPrevista, Nothing)))

            Dim xValid As Boolean = xDataStart.HasValue
            Dim yValid As Boolean = yDataStart.HasValue

            If xValid Then

                If yValid Then

                    Return 0
                End If

                Return -1
            End If

            If yValid Then

                Return 1
            End If

            Return 0
        End Function

        Private Function CompareTerreno(x As ImpiantiIrrigabili, y As ImpiantiIrrigabili) As Integer

            Dim xValid As Integer = If(x.Argilla.HasValue, 1, 0) + If(x.Sabbia.HasValue, 1, 0) + If(x.Pendenza.HasValue, 1, 0)
            Dim yValid As Integer = If(y.Argilla.HasValue, 1, 0) + If(y.Sabbia.HasValue, 1, 0) + If(y.Pendenza.HasValue, 1, 0)

            If xValid <> yValid Then

                If xValid > yValid Then

                    Return -1
                Else

                    Return 1
                End If
            End If

            Return 0
        End Function

        Private Function CompareIrriguo(x As ImpiantiIrrigabili, y As ImpiantiIrrigabili) As Integer

            Dim xValid As Boolean = x.Irri_Imp_Cod.HasValue
            Dim yValid As Boolean = y.Irri_Imp_Cod.HasValue

            If xValid Then

                If yValid Then

                    Return 0
                End If

                Return -1
            End If

            If yValid Then

                Return 1
            End If

            Return 0
        End Function

        Private Function CompareSesto(x As ImpiantiIrrigabili, y As ImpiantiIrrigabili) As Integer

            If x.GruppoVegetale.ToLower().CompareTo(y.GruppoVegetale.ToLower()) <> 0 Then

                Return 0
            End If

            If x.GruppoVegetale.ToLower().CompareTo("arboree") <> 0 Then

                Return 0
            End If

            Dim xValid As Integer = If(x.TraFila.HasValue, 1, 0) + If(x.SuFila.HasValue, 1, 0) + If(Not String.IsNullOrEmpty(x.CondTraFila), 1, 0)
            Dim yValid As Integer = If(y.TraFila.HasValue, 1, 0) + If(y.SuFila.HasValue, 1, 0) + If(Not String.IsNullOrEmpty(y.CondTraFila), 1, 0)

            If xValid <> yValid Then

                If xValid > yValid Then

                    Return -1
                Else

                    Return 1
                End If
            End If

            Return 0
        End Function
    End Class



    Public Class StringConverter(Of T)
        Inherits JsonConverter(Of T)

        Public Overrides Sub WriteJson(writer As JsonWriter, value As T, serializer As JsonSerializer)
            serializer.Serialize(writer, value)
        End Sub

        Public Overrides Function ReadJson(reader As JsonReader, objectType As Type, existingValue As T, hasExistingValue As Boolean, serializer As JsonSerializer) As T

            Dim instr As String = reader.Value

            If String.IsNullOrEmpty(instr) Then
                Return Nothing
            End If

            Dim obj = JsonConvert.DeserializeObject(Of T)(instr)

            Return obj
        End Function
    End Class





    Public Class WBInput
        Public CUAA As String
        Public Description As String
        Public Superficie_ha As Decimal
        Public Pendenza As Decimal?
        Public Sabbia As Decimal?
        Public Argilla As Decimal?
        Public GruppoVegetale As String
        Public TraFila As Decimal?
        Public SuFila As Decimal?
        Public CondTraFila As String
        Public Vigoria As Integer?
        Public DataInizioImpianto As DateTime
        Public Irri_Veg_Cod As Integer?
        Public Irri_Imp_Cod As Integer?
        Public Irri_Plot_Id As String
        Public Irri_Crop_Id As String
        Public DataFaseStart As DateTime
        Public RunData As DateTime?
        Public Coord As LatLng
        Public UsaMeteo As Boolean
        Public StazioneMeteo As Irriframe.ImpiantiIrrigabili.ObjStazioneMeteo
        Public IrriChoice As Integer
        Public Irrigazioni As List(Of Irriframe.ImpiantiIrrigabili.ObjIrrigazione)
        Public Fertilizzazioni As List(Of Irriframe.ImpiantiIrrigabili.ObjFertilizzazioni)
        Public FertiRecipe As Irriframe.ImpiantiIrrigabili.ObjFertiRecipe
        Public Cop_Cod_Irriframe As Integer?
        Public Mac_Cod As Integer?
        Public Portata As Decimal?

        Public Class LatLng
            Public Lat As Decimal
            Public Lng As Decimal
        End Class
    End Class





    Public Class AdviceBody
        Public IdPlot As Integer?
        Public Plot As Plot
        Public Crop As Crop
        Public IsCsv As Integer
        Public IsIrriUser As Integer
        Public RunData As Date?
        Public MeteoGList As List(Of MeteoG)
        Public Irrigations As List(Of Irrigation)
        Public SoilMoistures As List(Of SoilMoisture)
        Public FertiRecipe As FertiRecipe
        Public FertIrriGList As List(Of FertiIrriG)

        Public Sub New(input As WBInput, persistent As Boolean)

            IsCsv = 1
            RunData = Date.Now().Date
            If input.RunData.HasValue Then
                RunData = input.RunData.Value
            End If

            If persistent Then

                IdPlot = input.Irri_Plot_Id
            End If

            Plot = New Plot With {
                .CUAA = input.CUAA,
                .Description = input.Description,
                .Latitude = 0,
                .Longitude = 0,
                .Superficie = input.Superficie_ha * 10000, ' Ha -> Metri quadrati
                .Horizon1Clay = input.Argilla,
                .Horizon1Sand = input.Sabbia,
                .IrriSysCode = input.Irri_Imp_Cod
            }

            If input.Coord IsNot Nothing Then
                Plot.Latitude = input.Coord.Lat
                Plot.Longitude = input.Coord.Lng
            End If

            Dim pendenza As Decimal = If(input.Pendenza, 0)

            Select Case pendenza
                Case < 5
                    Plot.Slope = 2
                Case <= 10
                    Plot.Slope = 7
                Case <= 20
                    Plot.Slope = 15
                Case Else
                    Plot.Slope = 22
            End Select

            Crop = New Crop With {
                .Id_CropType = input.Irri_Veg_Cod,
                .DataFaseStart = input.DataFaseStart,
                .AnnoImpianto = input.DataInizioImpianto.Year,
                .ColturaProtetta = input.Cop_Cod_Irriframe.HasValue,
                .Id_GreenHouseType = input.Cop_Cod_Irriframe
            }

            If input.GruppoVegetale.ToLower().Trim() = "arboree" Then

                Crop.DistaTraFilaPiante = input.TraFila
                Crop.DistaSuFilaPiante = input.SuFila

                If Not String.IsNullOrEmpty(input.CondTraFila) Then

                    If input.CondTraFila.ToLower().Trim() = "inerbito" Then

                        Crop.ConduInterfilare = "I"
                    Else

                        Crop.ConduInterfilare = "L"
                    End If
                End If

                If input.Vigoria.HasValue AndAlso input.Vigoria.Value > 0 Then

                    Crop.ClasseVigore = input.Vigoria.Value 'Debole / Medio / Vigoroso / Molto vigoroso
                End If

            End If

            IsIrriUser = 1

            Select Case input.IrriChoice

                Case 1 'Irrigazioni_GIAS

                    Irrigations = New List(Of Irrigation)

                    If input.Irrigazioni IsNot Nothing Then

                        For Each irri In input.Irrigazioni

                            Irrigations.Add(New Irrigation With {.DataIrri = irri.DataIrri, .VolumeMM = irri.VolumeMM})
                        Next
                    End If

                Case 2 'Irrigazioni_IF

                    Irrigations = Nothing

                Case 3 'Bilancio_IF

                    IsIrriUser = 0

            End Select

            FertiRecipe = Nothing

            If input.FertiRecipe IsNot Nothing Then

                FertiRecipe = New FertiRecipe With {
                    .N_Ammesso = input.FertiRecipe.N_Ammesso,
                    .P_Ammesso = input.FertiRecipe.P_Ammesso,
                    .K_Ammesso = input.FertiRecipe.K_Ammesso
                }
            End If

            FertIrriGList = Nothing

            If input.Fertilizzazioni IsNot Nothing Then

                FertIrriGList = New List(Of FertiIrriG)

                For Each ferti In input.Fertilizzazioni
                    FertIrriGList.Add(New FertiIrriG With {
                                      .Data = ferti.Data,
                                      .DoseKg = ferti.DoseKg,
                                      .TitoloN = ferti.TitoloN,
                                      .TitoloP = ferti.TitoloP,
                                      .TitoloK = ferti.TitoloK,
                                      .NomeCommercialeDescri = ferti.NomeCommercialeDescri
                                      })
                Next
            End If
        End Sub
    End Class


    Public Class Plot
        Public CUAA As String
        Public Description As String
        Public Latitude As Decimal
        Public Longitude As Decimal
        Public Slope As Integer
        Public Horizon1Clay As Decimal?
        Public Horizon1Sand As Decimal?
        Public IrriSysCode As Integer?
        Public Superficie As Integer?
        'Horizon1Stone
        'IrriFlowRate Pluviometria impianto mm/h (Values: between 0.1 and 1000)
    End Class

    Public Class Crop
        Public Id_CropType As Integer
        Public DataFaseStart As Date?
        Public DistaTraFilaPiante As Decimal?
        Public DistaSuFilaPiante As Decimal?
        Public ConduInterfilare As String
        Public ClasseVigore As Integer? 'Debole / Medio / Vigoroso / Molto vigoroso
        Public AnnoImpianto As Integer?
        Public ColturaProtetta As Boolean?
        Public Id_GreenHouseType As Integer?
    End Class

    Public Class MeteoG
        <JsonProperty("Date")>
        Public Data As DateTime
        Public Tmed As Decimal?
        Public Tmin As Decimal?
        Public Tmax As Decimal?
        Public RainMM As Decimal?
        Public RainH As Decimal?
    End Class

    Public Class Irrigation
        Public DataIrri As Date
        Public VolumeMM As Decimal
    End Class

    Public Class SoilMoisture
        <JsonProperty("Date")>
        Public Data As Date
        Public SoilMoisturePercVol As Decimal
    End Class

    Public Structure FertiRecipe
        Public N_Ammesso As Decimal
        Public P_Ammesso As Decimal
        Public K_Ammesso As Decimal
    End Structure

    Public Structure FertiIrriG
        Public Data As DateTime
        Public TitoloN As Decimal
        Public TitoloP As Decimal
        Public TitoloK As Decimal
        Public DoseKg As Decimal
        Public NomeCommercialeDescri As String
    End Structure





    Public Class AdviceResponse
        Public DataEsecuzione As DateTime
        Public IrrigazioniPreviste As List(Of IrrigazionePrevista)
        Public VolumeIrriMm As Decimal?
        Public DataPrevistaIrriDt As DateTime?
        Public Csv As String
        Public IdPlot As Integer?
        Public IdCHU As Integer?
        Public ConsumoColturaMM As Decimal?
        Public InfoProvider As String
        Public FertiResidualN As Decimal?
        Public FertiResidualP As Decimal?
        Public FertiResidualK As Decimal?
        Public FertiRecipeN As Decimal?
        Public FertiRecipeP As Decimal?
        Public FertiRecipeK As Decimal?
        Public FertiGivenN As Decimal?
        Public FertiGivenP As Decimal?
        Public FertiGivenK As Decimal?
        Public NextFertiDate As DateTime?
    End Class


    Public Class IrrigazionePrevista
        Public DataPrevistaIrri As DateTime
        Public VolumeIrriMM As Decimal
    End Class


    Public Class IrrigazionePrevistaOut
        Public Data As DateTime
        Public VolumeMM As Decimal
    End Class

    Public Class TurnoIrriguoDisponibile
        Public Data As DateTime
        Public DurataM As Integer
    End Class

    Public Class TurnoIrriguoPrevisto
        Public Data As DateTime
        Public DurataM As Integer
        Public VolumeMM As Decimal
    End Class


    Public Class CSVParser

        Public Class _WBElem
            Public Data As Date
            Public Prec As Decimal
            Public Irri As Decimal
            Public Umid As Decimal
            Public SoInf As Decimal
            Public SoSup As Decimal
            Public SoInfPerc As Decimal
            Public SoSupPerc As Decimal

            Public Sub New(arrStr As String())

                Date.TryParse(arrStr(1).Split(" ")(0), Data)
                Decimal.TryParse(arrStr(17), Prec)
                Decimal.TryParse(arrStr(20), Irri)

                Dim cc As Decimal
                Dim pa As Decimal
                Dim _umid As Decimal
                Dim _soInf As Decimal
                Dim _soSup As Decimal
                Dim DzSup As Decimal

                Decimal.TryParse(arrStr(11), cc)
                Decimal.TryParse(arrStr(12), pa)
                Decimal.TryParse(arrStr(8), _umid)
                Decimal.TryParse(arrStr(16), _soInf)
                Decimal.TryParse(arrStr(15), _soSup)
                Decimal.TryParse(arrStr(9), DzSup)

                Dim cc_pa As Decimal = cc - pa

                Umid = (_umid - pa) / cc_pa * 100D
                SoInf = (_soInf - pa) / cc_pa * 100D
                SoSup = (_soSup - pa) / cc_pa * 100D
                SoInfPerc = _soInf / DzSup * 100D
                SoSupPerc = _soSup / DzSup * 100D
            End Sub
        End Class

        Public Shared Function GetList(csv As String) As List(Of _WBElem)

            Dim retList As New List(Of _WBElem)

            Dim _reader As New StringReader(csv)

            'Scarto le prime 2 righe, la funzione ReadLine fa avanzare il contatore interno dello StringReader,
            'di modo che ad ogni chiamata venga letta la riga successiva. Viene restituito nothing quando si raggiunge la fine della stringa
            Dim lineIgnore = _reader.ReadLine()
            If lineIgnore Is Nothing Then
                Return retList
            End If

            lineIgnore = _reader.ReadLine()

            If lineIgnore Is Nothing Then
                Return retList
            End If

            Dim _line As String
            Dim _split As String()
            Dim _endWhile As Boolean = False
            Dim _fase As Integer

            While Not _endWhile

                _line = _reader.ReadLine()

                If _line IsNot Nothing Then

                    _split = _line.Split(";"c)

                    If _split.Count >= 72 Then

                        If Not String.IsNullOrEmpty(_split(11)) Then

                            If _fase < 2 Then
                                Integer.TryParse(_split(2), _fase)
                            End If
                            If _fase > 1 Then

                                retList.Add(New _WBElem(_split))
                            End If
                        Else

                            _endWhile = True
                        End If
                    End If
                Else

                    _endWhile = True
                End If
            End While

            Return retList
        End Function
    End Class



    Public Class WBOutput
        Public DataEsecuzione As DateTime
        Public InfoProvider As String
        Public IrrigazioniPreviste As List(Of IrrigazionePrevistaOut)
        Public TurniIrriguiPrevisti As List(Of TurnoIrriguoPrevisto)
        Public ConsumoMM As Decimal?
        Public FertiResidualN As Decimal?
        Public FertiResidualP As Decimal?
        Public FertiResidualK As Decimal?
        Public FertiRecipeN As Decimal?
        Public FertiRecipeP As Decimal?
        Public FertiRecipeK As Decimal?
        Public FertiGivenN As Decimal?
        Public FertiGivenP As Decimal?
        Public FertiGivenK As Decimal?
        Public NextFertiDate As DateTime?
        Public IrriframeData As IFData
        Public Table As String
        Public TableTurni As String

        Public Class IFData
            Public PlotId As Integer
            Public CropId As Integer
        End Class
    End Class


End Namespace
