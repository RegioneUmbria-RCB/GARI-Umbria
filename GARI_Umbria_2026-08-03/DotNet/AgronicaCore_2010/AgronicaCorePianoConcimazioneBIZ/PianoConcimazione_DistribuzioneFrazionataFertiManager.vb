
Imports System.Runtime.Serialization

Public Class PianoConcimazione_DistribuzioneFrazionataFertiManager
    Public Function DistribFrazFertiManager(ByVal input As Input_FertiManager) As Output_FertiAdvice

        'Devo gestire classi diverse per l'input e l'output solo per problemi di serializzazione delle date

        Dim Calcolo As New DistribuzioneFrazionata_FertiManager
        Dim fa As FertiAdvice = Calcolo.DistribFrazFertiManager(input.realInput())
        Return New Output_FertiAdvice(fa)

    End Function

End Class
Public Class Input_FertiSommiG
    <IgnoreDataMember>
    Public Data_ As DateTime 'Data somministrazione (Opzionale ?)
    Public Data As String
    Public TitoloN As Decimal? 'Titolo N 
    Public TitoloP As Decimal? 'Titolo P₂O₅ 
    Public TitoloK As Decimal? 'Titolo K₂O 
    Public DoseKg As Decimal? 'Dose distribuita Kg 
    <OnDeserialized>
    Private Sub OnDeserialized(context As StreamingContext)
        Me.Data_ = AgronicaCoreUtility.DataOra.DataStringaJSON_to_ISO8601(Me.Data)
    End Sub
End Class

Public Class Input_FertiManager
    <IgnoreDataMember>
    Public data_ As DateTime
    Public data As String
    Public Tsum As Decimal()
    Public fertiTotN As Decimal
    Public fertiTotP As Decimal
    Public fertiTotK As Decimal
    <IgnoreDataMember>
    Public dataFaseStart_ As DateTime
    Public dataFaseStart As String
    Public fenofasi As List(Of FertiParsPhenophase)
    Public fertiSommiList As List(Of Input_FertiSommiG)
    Public frazResiduaN As Decimal 'Opzionale ?
    Public frazResiduaP As Decimal 'Opzionale ?
    Public frazResiduaK As Decimal 'Opzionale ?
    <IgnoreDataMember>
    Public dataIrriPrevista1_ As DateTime
    Public dataIrriPrevista1 As String
    <IgnoreDataMember>
    Public dataIrriPrevista2_ As DateTime
    Public dataIrriPrevista2 As String

    <OnDeserialized>
    Private Sub OnDeserialized(context As StreamingContext)
        Me.data_ = AgronicaCoreUtility.DataOra.DataStringaJSON_to_ISO8601(Me.data)
        Me.dataFaseStart_ = AgronicaCoreUtility.DataOra.DataStringaJSON_to_ISO8601(Me.dataFaseStart)
        Me.dataIrriPrevista1_ = AgronicaCoreUtility.DataOra.DataStringaJSON_to_ISO8601(Me.dataIrriPrevista1)
        Me.dataIrriPrevista2_ = AgronicaCoreUtility.DataOra.DataStringaJSON_to_ISO8601(Me.dataIrriPrevista2)
    End Sub

    Public Function realInput() As FertiManager_input
        Dim fmi As New FertiManager_input
        fmi.data = data_
        fmi.Tsum = Tsum
        fmi.fertiTotN = fertiTotN
        fmi.fertiTotP = fertiTotP
        fmi.fertiTotK = fertiTotK
        fmi.dataFaseStart = dataFaseStart_
        fmi.fenofasi = fenofasi
        fmi.fertiSommiList = New List(Of FertiSommiG)
        For Each ifs In fertiSommiList
            Dim fs As New FertiSommiG
            fs.Data = ifs.Data_
            fs.TitoloN = ifs.TitoloN
            fs.TitoloP = ifs.TitoloP
            fs.TitoloK = ifs.TitoloK
            fs.DoseKg = ifs.DoseKg
            fmi.fertiSommiList.Add(fs)
        Next
        fmi.frazResiduaN = frazResiduaN
        fmi.frazResiduaP = frazResiduaP
        fmi.frazResiduaK = frazResiduaK
        fmi.dataIrriPrevista1 = dataIrriPrevista1_
        fmi.dataIrriPrevista2 = dataIrriPrevista2_
        Return fmi
    End Function

End Class

Public Class Output_FertiAdvice
    <IgnoreDataMember>
    Public Data_ As DateTime
    Public Data As String
    Public DayDoseN As Decimal 'Apporti nutritivi da distribuire N (kg/superficie) (Opzionale ?)
    Public DayDoseP As Decimal 'Apporti nutritivi da distribuire P (kg/superficie) (Opzionale ?)
    Public DayDoseK As Decimal 'Apporti nutritivi da distribuire K (kg/superficie) (Opzionale ?)
    Public DayconsumptionN As Decimal 'Consumo giornaliero N (kg/ha) (Opzionale ?)
    Public DayconsumptionP As Decimal 'Consumo giornaliero P (kg/ha) (Opzionale ?)
    Public DayconsumptionK As Decimal 'Consumo giornaliero K (kg/ha) (Opzionale ?)
    <IgnoreDataMember>
    Public NextDate_ As DateTime
    Public NextDate As String
    Public Forzatura As Boolean
    Public Result As Integer 'Enumerativo che restituisce il risultato (es: 1=Ok, 2=StopConsiglio,...)   
    Public Errore As String 'Eventuale messaggio di errore
    'Array giornalieri soglie per grafico
    Public NSogliaInf As Decimal()
    Public NSogliaSup As Decimal()
    Public Nsd As Decimal()
    Public PSogliaInf As Decimal()
    Public PSogliaSup As Decimal()
    Public Psd As Decimal()
    Public KSogliaInf As Decimal()
    Public KSogliaSup As Decimal()
    Public Ksd As Decimal()
    Public Sub New()

    End Sub


    Public Sub New(ByRef fa As FertiAdvice)
        Data_ = fa.Data
        DayDoseN = fa.DayDoseN
        DayDoseP = fa.DayDoseP
        DayDoseK = fa.DayDoseK
        DayconsumptionN = fa.DayconsumptionN
        DayconsumptionP = fa.DayconsumptionP
        DayconsumptionK = fa.DayconsumptionK
        NextDate_ = fa.NextDate
        Forzatura = fa.Forzatura
        Result = fa.Result
        Errore = fa.Errore
        NSogliaInf = fa.NSogliaInf
        NSogliaSup = fa.NSogliaSup
        Nsd = fa.Nsd
        PSogliaInf = fa.PSogliaInf
        PSogliaSup = fa.PSogliaSup
        Psd = fa.Psd
        KSogliaInf = fa.KSogliaInf
        KSogliaSup = fa.KSogliaSup
        Ksd = fa.Ksd
    End Sub
    <OnSerializing>
    Private Sub OnSerializing(context As StreamingContext)
        Me.Data = AgronicaCoreUtility.DataOra.DataOraToDate_JSON_ISO8601(Me.Data_)
        Me.NextDate = AgronicaCoreUtility.DataOra.DataOraToDate_JSON_ISO8601(Me.NextDate_)
    End Sub
End Class

