Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports System.Runtime.Serialization

<DataContract()>
Public Class Materie_Prime_ParametriQualitativi

    <DataMember()> Public Property Piva As String
    <DataMember()> Public Property Sa_Cod As Integer
    <DataMember()> Public Property Mat_Cod As Integer
    <DataMember()> Public Property Tipo As String
    <DataMember()> Public Property Tipo_Cod As Integer
    <DataMember()> Public Property Udm_Cod As Integer

    <DataMember()> Public Property inviato() As Int16
    <DataMember()> Public Property datainvio() As Date?
    <DataMember()> Public Property data_creazione() As DateTime
    <DataMember()> Public Property data_modifica() As DateTime
    <DataMember()> Public Property username_creazione() As String
    <DataMember()> Public Property username_modifica() As String
    <DataMember()> Public Property validita_inizio() As DateTime
    <DataMember()> Public Property validita_fine() As DateTime

    <DataMember()> Public Property Valore_Des As String
    <DataMember()> Public Property Valore_Min As Decimal
    <DataMember()> Public Property Valore_Max As Decimal
    <DataMember()> Public Property ChkRegistri As Int16
    <DataMember()> Public Property ChkCalibri As Int16
    '<DataMember()> Public Property ChkRegistri_Vinificazione As Int16 --> SERVE SOLO AL LAN

    <DataMember()> Public Property tipo_operazione As Integer

    Public Sub New()
        Piva = ""
        Sa_Cod = PRIVATO
        Mat_Cod = 0
        Tipo = ""
        Tipo_Cod = 0
        Udm_Cod = 0

        inviato = 0
        datainvio = Nothing
        data_creazione = AGRODATAINIZIO
        data_modifica = AGRODATAINIZIO
        username_creazione = ""
        username_modifica = ""
        validita_inizio = AGRODATAINIZIO
        validita_fine = AGRODATAINIZIO

        Valore_Des = ""
        Valore_Min = 0
        Valore_Max = 0
        ChkRegistri = 0
        ChkCalibri = 0
        'ChkRegistri_Vinificazione = 0 --> SERVE SOLO AL LAN

        tipo_operazione = 0
    End Sub

End Class

Public Class Materie_Prime_ParametriQualitativi_R

    Public Function Leggi( _
                            ByVal PIVA As String, _
                            ByVal Sa_Cod As Integer, _
                            ByVal Mat_Cod As Integer, _
                            ByVal Tipo As String, _
                            ByVal Tipo_Cod As Integer, _
                            ByVal Udm_Cod As Integer, _
                            ByVal ForDelete As Boolean, _
                            ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As List(Of Materie_Prime_ParametriQualitativi)

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeObject.Materie_Prime_ParametriQualitativi_R.Leggi()"
        Dim mp_R As New AgronicaCoreAnagrafeDAL.Materie_Prime_PQ_R
        Dim listaObjMP As New List(Of Materie_Prime_ParametriQualitativi)

        'leggo da DB l'oggetto sulla base dei parametri passati
        Dim dt As DataTable = mp_R.Leggi(PIVA, Sa_Cod, Mat_Cod, Tipo, Tipo_Cod, _
                                         Udm_Cod, xSelezioneVariabile, _
                                         xFiltroAggiuntivo, xOrderBy, objParametri)

        If Not IsNothing(dt) Then
            'creo il singolo oggetto e lo aggiungo alla lista
            For Each dRow As DataRow In dt.Rows

                Dim mp As New Materie_Prime_ParametriQualitativi()

                mp.tipo_operazione = IIf(ForDelete, 3, 0)
                'mp.Piva_SuperUser = dRow("PivaSuperUser")
                mp.Piva = dRow("Piva")
                mp.Sa_Cod = dRow("Sa_Cod")
                mp.Mat_Cod = dRow("Mat_Cod")
                mp.Tipo = dRow("Tipo")
                mp.Tipo_Cod = dRow("Tipo_Cod")
                mp.Udm_Cod = dRow("Udm_Cod")

                mp.inviato = Agro_SQL_Load_ConDefault(dRow("inviato"), mp.inviato.GetType)
                mp.datainvio = DBNullToNothing(dRow("datainvio"))
                mp.data_creazione = Agro_SQL_Load_ConDefault(dRow("data_creazione"), mp.data_creazione.GetType)
                mp.data_modifica = Agro_SQL_Load_ConDefault(dRow("data_modifica"), mp.data_modifica.GetType)
                mp.username_creazione = Agro_SQL_Load_ConDefault(dRow("username_creazione"), mp.username_creazione.GetType)
                mp.username_modifica = Agro_SQL_Load_ConDefault(dRow("username_modifica"), mp.username_modifica.GetType)
                mp.validita_inizio = Agro_SQL_Load_ConDefault(dRow("validita_inizio"), mp.validita_inizio.GetType)
                mp.validita_fine = Agro_SQL_Load_ConDefault(dRow("validita_fine"), mp.validita_fine.GetType)

                mp.Valore_Des = dRow("Valore_Des")
                mp.Valore_Min = dRow("Valore_Min")
                mp.Valore_Max = dRow("Valore_Max")
                mp.ChkRegistri = dRow("ChkRegistri")
                mp.ChkCalibri = dRow("ChkCalibri")
                'mp.ChkRegistri_Vinificazione = dRow("ChkRegistri_Vinificazione")  --> SERVE SOLO AL LAN

                listaObjMP.Add(mp)
            Next
        Else
            listaObjMP = Nothing
        End If

        Return listaObjMP
    End Function

End Class

Public Class Materie_Prime_ParametriQualitativi_W

    Public Function Scrivi( _
                            ByVal PIVA As String, _
                            ByVal Sa_Cod As Int32, _
                            ByVal Mat_Cod As Int32, _
                            ByVal Tipo As String, _
                            ByVal Tipo_Cod As Int32, _
                            ByVal Udm_Cod As Int32, _
                            ByVal Valore_Des As String, _
                            ByVal Valore_Min As Decimal, _
                            ByVal Valore_Max As Decimal, _
                            ByVal ChkRegistri As Int16, _
                            ByVal ChkCalibri As Int16, _
                            ByVal Validita_Inizio As Date, _
                            ByVal Validita_Fine As Date, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                            Optional ByVal Data_creazione As DateTime = #2/1/1900#, _
                            Optional ByVal Data_modifica As DateTime = #2/1/1900#, _
                            Optional ByVal username_creazione As String = "", _
                            Optional ByVal username_modifica As String = "" _
                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeObject.Materie_Prime_PQ_W.Scrivi()"
        Dim mp_W As New AgronicaCoreAnagrafeDAL.Materie_Prime_PQ_W

        Dim res As Boolean = mp_W.Scrivi(PIVA, Sa_Cod, Mat_Cod, Tipo, Tipo_Cod, Udm_Cod, _
                                         Agro_SQL_SaveText(Valore_Des), _
                                         Valore_Min, Valore_Max, ChkRegistri, ChkCalibri, _
                                         Validita_Inizio, Validita_Fine, objParametri, _
                                         Data_creazione, Data_modifica, username_creazione, username_modifica)

        Return res

    End Function

    Public Function Modifica( _
                            ByVal Old_Piva As String, _
                            ByVal Old_Sa_Cod As Int32, _
                            ByVal Old_Mat_Cod As Int32, _
                            ByVal Old_Tipo As String, _
                            ByVal Old_Tipo_Cod As Int32, _
                            ByVal New_Udm_Cod As Int32, _
                            ByVal New_Valore_Des As String, _
                            ByVal New_Valore_Min As Decimal, _
                            ByVal New_Valore_Max As Decimal, _
                            ByVal New_ChkRegistri As Int16, _
                            ByVal New_ChkCalibri As Int16, _
                            ByVal New_Validita_Inizio As Date, _
                            ByVal New_Validita_Fine As Date, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                            Optional ByVal Data_modifica As DateTime = #2/1/1900#, _
                            Optional ByVal username_modifica As String = "" _
                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeObject.Materie_Prime_PQ_W.Modifica()"
        Dim mp_W As New AgronicaCoreAnagrafeDAL.Materie_Prime_PQ_W

        Dim res As Boolean = mp_W.Modifica(Old_Piva, Old_Sa_Cod, Old_Mat_Cod, Old_Tipo, Old_Tipo_Cod, _
                                           New_Udm_Cod, Agro_SQL_SaveText(New_Valore_Des), New_Valore_Min, New_Valore_Max, _
                                           New_ChkRegistri, New_ChkCalibri, _
                                           New_Validita_Inizio, New_Validita_Fine, _
                                           xFiltroAggiuntivo, objParametri, _
                                           Data_modifica, username_modifica)

        Return res

    End Function


    Public Function Cancella( _
                                ByVal Piva As String, _
                                ByVal Sa_Cod As Long, _
                                ByVal Mat_Cod As Long, _
                                ByVal Tipo As String, _
                                ByVal Tipo_Cod As Long, _
                                ByVal Udm_Cod As Long, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeObject.Materie_Prime_PQ_W.Cancella()"
        Dim mp_W As New AgronicaCoreAnagrafeDAL.Materie_Prime_PQ_W

        Dim res As Boolean = mp_W.Cancella(Piva, Sa_Cod, Mat_Cod, Tipo, Tipo_Cod, Udm_Cod, _
                                         xFiltroAggiuntivo, objParametri)

        Return res

    End Function

End Class