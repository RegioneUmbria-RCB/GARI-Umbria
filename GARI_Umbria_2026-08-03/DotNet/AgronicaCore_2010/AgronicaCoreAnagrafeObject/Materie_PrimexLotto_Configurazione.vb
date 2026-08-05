'ANCORA NON UTILIZZATO

'Imports AgronicaCoreDataProvider
'Imports AgronicaCoreDataProvider.UtilityProvider
'Imports AgronicaCoreDataProvider.TipiEnumerativi
'Imports AgronicaCoreDataProvider.CostantiPersonalizzate
'Imports System.Runtime.Serialization

'<DataContract()>
'Public Class Materie_PrimexLotto_Configurazione

'    <DataMember()> Public Property Piva_SuperUser() As String
'    <DataMember()> Public Property Piva() As String
'    <DataMember()> Public Property Sa_Cod() As Integer
'    <DataMember()> Public Property Elem_Cod() As Integer
'    <DataMember()> Public Property Pro_Cod() As Integer
'    <DataMember()> Public Property Mat_Cod() As Integer
'    <DataMember()> Public Property Lotto_Cod() As Integer
'    <DataMember()> Public Property ChkListini() As Int16
'    <DataMember()> Public Property ChkReport() As Int16

'    <DataMember()> Public Property inviato() As Int16
'    <DataMember()> Public Property datainvio() As DateTime
'    <DataMember()> Public Property data_creazione() As DateTime
'    <DataMember()> Public Property data_modifica() As DateTime
'    <DataMember()> Public Property username_creazione() As String
'    <DataMember()> Public Property username_modifica() As String
'    <DataMember()> Public Property validita_inizio() As DateTime
'    <DataMember()> Public Property validita_fine() As DateTime

'    <DataMember()> Public Property ChkProprieta() As Int16
'    <DataMember()> Public Property Cifra_Start() As Int16
'    <DataMember()> Public Property Cifra_End() As Int16


'    Public Sub New()
'        Piva_SuperUser = ""
'        Piva = ""
'        Sa_Cod = PRIVATO
'        Elem_Cod = 0
'        Pro_Cod = 0
'        Mat_Cod = 0
'        Lotto_Cod = 0
'        ChkListini = 0
'        ChkReport = 0

'        inviato = 0
'        datainvio = AGRODATAINIZIO
'        data_creazione = DateTime.Now
'        data_modifica = DateTime.Now
'        username_creazione = ""
'        username_modifica = ""
'        validita_inizio = AGRODATAINIZIO
'        validita_fine = AGRODATAFINE

'        ChkProprieta = 0
'        Cifra_Start = 0
'        Cifra_End = 0
'    End Sub

'End Class


'Public Class Materie_PrimexLotto_Configurazione_R

'    Public Function Leggi_xAltoLivello( _
'                        ByVal Piva As String, _
'                        ByVal Sa_Cod As Integer, _
'                        ByVal Elem_Cod As Integer, _
'                        ByVal Pro_Cod As Integer, _
'                        ByVal Mat_Cod As Integer, _
'                        ByVal Lotto_Cod As Integer, _
'                        ByVal ChkListini As Integer, _
'                        ByVal ChkReport As Integer, _
'                        ByVal ChkProprieta As Integer, _
'                        ByVal xFiltroAggiuntivo As String, _
'                        ByVal xOrderBy As String, _
'                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
'                        ) As List(Of Materie_PrimexLotto_Configurazione)


'        Dim NomeRoutine As String = "AgronicaCoreAnagrafeObject.Materie_PrimexLotto_Configurazione_R.Leggi_xAltoLivello()"

'        Dim mp_R As New AgronicaCoreAnagrafeDAL.Materie_PrimexLC_R()
'        Dim listaObjMP As New List(Of Materie_PrimexLotto_Configurazione)

'        'leggo da DB l'oggetto sulla base dei parametri passati
'        Dim dt As DataTable = mp_R.Leggi_xAltoLivello(Piva, Sa_Cod, Elem_Cod, Pro_Cod, Mat_Cod, _
'                                                      Lotto_Cod, ChkListini, ChkReport, ChkProprieta, _
'                                                      xFiltroAggiuntivo, xOrderBy, objParametri)

'        If Not IsNothing(dt) Then
'            'creo il singolo oggetto e lo aggiungo alla lista
'            For Each dRow As DataRow In dt.Rows

'                Dim mp As New Materie_PrimexLotto_Configurazione()

'                'SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
'                mp.Piva_SuperUser = dRow("PivaSuperUser")
'                mp.Piva = dRow("Piva")
'                mp.Sa_Cod = dRow("Sa_Cod")
'                mp.Elem_Cod = dRow("Elem_Cod")
'                mp.Pro_Cod = dRow("Pro_Cod")
'                mp.Mat_Cod = dRow("Mat_Cod")
'                mp.Lotto_Cod = dRow("Lotto_Cod")
'                mp.ChkListini = dRow("ChkListini")
'                mp.ChkReport = dRow("ChkReport")
'                mp.ChkProprieta = dRow("ChkProprieta")
'                mp.inviato = dRow("inviato")
'                mp.datainvio = dRow("datainvio")
'                mp.data_creazione = dRow("data_creazione")
'                mp.data_modifica = dRow("data_modifica")
'                mp.username_creazione = dRow("username_creazione")
'                mp.username_modifica = dRow("username_modifica")
'                mp.validita_inizio = dRow("validita_inizio")
'                mp.validita_fine = dRow("validita_fine")
'                mp.Cifra_Start = dRow("Cifra_Start")
'                mp.Cifra_End = dRow("Cifra_End")

'                listaObjMP.Add(mp)
'            Next
'        Else
'            listaObjMP = Nothing
'        End If

'        Return listaObjMP
'    End Function

'End Class

'Public Class Materie_PrimexLotto_Configurazione_W

'    Public Function Scrivi( _
'                           ByVal PIVA As String, _
'                           ByVal Sa_Cod As Int32, _
'                           ByVal Elem_Cod As Int32, _
'                           ByVal Pro_Cod As Int32, _
'                           ByVal Mat_Cod As Int32, _
'                           ByVal Lotto_Cod As Int32, _
'                           ByVal ChkListini As Integer, _
'                           ByVal ChkReport As Integer, _
'                           ByVal Validita_Inizio As Date, _
'                           ByVal Validita_Fine As Date, _
'                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
'                           ) As Boolean

'        Dim NomeRoutine As String = "AgronicaCoreAnagrafeObject.Materie_PrimexLC_W.Scrivi()"
'        Dim mp_W As New AgronicaCoreAnagrafeDAL.Materie_PrimexReport_W

'        'Dim res As Boolean = mp_W.Scrivi(PIVA, Pro_Cod, Mat_Cod, Id_Report, _
'        '                                 Validita_Inizio, Validita_Fine, objParametri)

'        Return res




'    End Function

'    Public Function Scrivi( _
'                           ByVal mp As Materie_PrimexLotto_Configurazione, _
'                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
'                           ) As Boolean

'        Dim NomeRoutine As String = "AgronicaCoreAnagrafeObject.Materie_PrimexLC_W.Scrivi()"


'        '''''''''''''''''''''''''''''''''''''''''


'    End Function

'    Public Function Modifica( _
'                                ByVal Piva As String, _
'                                ByVal Sa_Cod As Int32, _
'                                ByVal Elem_Cod As Int32, _
'                                ByVal Pro_Cod As Int32, _
'                                ByVal Mat_Cod As Int32, _
'                                ByVal Lotto_Cod As Int32, _
'                                ByVal ChkListini As Int16, _
'                                ByVal ChkReport As Int16, _
'                                ByVal Validita_Inizio As Date, _
'                                ByVal Validita_Fine As Date, _
'                                ByVal xFiltroAggiuntivo As String, _
'                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
'                                ) As Boolean

'        Dim NomeRoutine As String = "AgronicaCoreAnagrafeObject.Materie_PrimexLC_W.Modifica()"
'        Dim mp_W As New AgronicaCoreAnagrafeDAL.Materie_PrimexReport_W

'        'Dim res As Boolean = mp_W.Modifica(Piva, Pro_Cod, Mat_Cod, Id_Report, _
'        '                                 Validita_Inizio, Validita_Fine, objParametri)

'        Return res

'    End Function


'    Public Function Cancella( _
'                                ByVal Piva As String, _
'                                ByVal Sa_Cod As Int32, _
'                                ByVal Elem_Cod As Int32, _
'                                ByVal Pro_Cod As Int32, _
'                                ByVal Mat_Cod As Int32, _
'                                ByVal Lotto_Cod As Int32, _
'                                ByVal xFiltroAggiuntivo As String, _
'                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
'                                ) As Boolean

'        Dim NomeRoutine As String = "AgronicaCoreAnagrafeObject.Materie_PrimexLC_W.Cancella()"
'        Dim mp_W As New AgronicaCoreAnagrafeDAL.Materie_PrimexReport_W

'        'Dim res As Boolean = mp_W.Cancella(Piva, Pro_Cod, Mat_Cod, Id_Report, _
'        '                                 Validita_Inizio, Validita_Fine, objParametri)

'        Return res

'    End Function

'End Class