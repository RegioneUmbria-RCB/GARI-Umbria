'ANCORA NON UTILIZZATO

'Imports AgronicaCoreDataProvider
'Imports AgronicaCoreDataProvider.UtilityProvider
'Imports AgronicaCoreDataProvider.TipiEnumerativi
'Imports AgronicaCoreDataProvider.CostantiPersonalizzate
'Imports System.Runtime.Serialization

'<DataContract()>
'Public Class Materie_PrimexLotto_Proprieta

'    <DataMember()> Public Property Piva_SuperUser() As String
'    <DataMember()> Public Property Piva() As String
'    <DataMember()> Public Property ID() As Integer
'    <DataMember()> Public Property Sa_Cod() As Integer
'    <DataMember()> Public Property Elem_Cod() As Integer
'    <DataMember()> Public Property Pro_Cod() As Integer
'    <DataMember()> Public Property Mat_Cod() As Integer
'    <DataMember()> Public Property Lotto_Cod1() As Integer
'    <DataMember()> Public Property Lotto_Val1() As String
'    <DataMember()> Public Property Lotto_Cod2() As Integer
'    <DataMember()> Public Property Lotto_Val2() As String
'    <DataMember()> Public Property Lotto_Cod3() As Integer
'    <DataMember()> Public Property Lotto_Val3() As String
'    <DataMember()> Public Property Id_Proprieta() As Integer
'    <DataMember()> Public Property Proprieta_Val() As String

'    <DataMember()> Public Property inviato() As Int16
'    <DataMember()> Public Property datainvio() As DateTime
'    <DataMember()> Public Property data_creazione() As DateTime
'    <DataMember()> Public Property data_modifica() As DateTime
'    <DataMember()> Public Property username_creazione() As String
'    <DataMember()> Public Property username_modifica() As String
'    <DataMember()> Public Property validita_inizio() As DateTime
'    <DataMember()> Public Property validita_fine() As DateTime

'    Public Sub New()
'        Piva_SuperUser = ""
'        Piva = ""
'        ID = 0
'        Sa_Cod = PRIVATO
'        Elem_Cod = 0
'        Pro_Cod = 0
'        Mat_Cod = 0
'        Lotto_Cod1 = 0
'        Lotto_Val1 = ""
'        Lotto_Cod2 = 0
'        Lotto_Val2 = ""
'        Lotto_Cod3 = 0
'        Lotto_Val3 = ""
'        Id_Proprieta = 0
'        Proprieta_Val = ""

'        inviato = 0
'        datainvio = AGRODATAINIZIO
'        data_creazione = Date.Now
'        data_modifica = Date.Now
'        username_creazione = ""
'        username_modifica = ""
'        validita_inizio = AGRODATAINIZIO
'        validita_fine = AGRODATAFINE
'    End Sub

'End Class

'Public Class Materie_PrimexLotto_Proprieta_R

'    Public Function Leggi( _
'                        ByVal PIVA As String, _
'                        ByVal ID As Integer, _
'                        ByVal Sa_Cod As Integer, _
'                        ByVal Elem_Cod As Integer, _
'                        ByVal Pro_Cod As Integer, _
'                        ByVal Mat_Cod As Integer, _
'                        ByVal Lotto_Cod1 As Integer, _
'                        ByVal Lotto_Val1 As String, _
'                        ByVal Lotto_Cod2 As Integer, _
'                        ByVal Lotto_Val2 As String, _
'                        ByVal Lotto_Cod3 As Integer, _
'                        ByVal Lotto_Val3 As String, _
'                        ByVal ID_Proprieta As Integer, _
'                        ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
'                        ByVal xFiltroAggiuntivo As String, _
'                        ByVal xOrderBy As String, _
'                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
'                        ) As List(Of Materie_PrimexLotto_Proprieta)


'        Dim NomeRoutine As String = "AgronicaCoreAnagrafeObject.Materie_PrimexLotto_Proprieta_R.Leggi()"
'        Dim mp_R As New AgronicaCoreAnagrafeDAL.Materie_PrimexLP_R()
'        Dim listaObjMP As New List(Of Materie_PrimexLotto_Proprieta)

'        'leggo da DB l'oggetto sulla base dei parametri passati
'        Dim dt As DataTable = mp_R.Leggi(PIVA, ID, Sa_Cod, Elem_Cod, Pro_Cod, Mat_Cod, _
'                                        Lotto_Cod1, Lotto_Val1, Lotto_Cod2, Lotto_Val2, _
'                                        Lotto_Cod3, Lotto_Val3, ID_Proprieta, xSelezioneVariabile, _
'                                        xFiltroAggiuntivo, xOrderBy, objParametri)

'        If Not IsNothing(dt) Then
'            'creo il singolo oggetto e lo aggiungo alla lista
'            For Each dRow As DataRow In dt.Rows

'                Dim mp As New Materie_PrimexLotto_Proprieta()

'                'SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
'                mp.Piva_SuperUser = dRow("PivaSuperUser")
'                mp.Piva = dRow("Piva")
'                mp.ID = dRow("ID")
'                mp.Sa_Cod = dRow("Sa_Cod")
'                mp.Elem_Cod = dRow("Elem_Cod")
'                mp.Pro_Cod = dRow("Pro_Cod")
'                mp.Mat_Cod = dRow("Mat_Cod")
'                mp.Lotto_Cod1 = dRow("Lotto_Cod1")
'                mp.Lotto_Val1 = dRow("Lotto_Val1")
'                mp.Lotto_Cod2 = dRow("Lotto_Cod2")
'                mp.Lotto_Val2 = dRow("Lotto_Val2")
'                mp.Lotto_Cod3 = dRow("Lotto_Cod3")
'                mp.Lotto_Val3 = dRow("Lotto_Val3")
'                mp.Id_Proprieta = dRow("Id_Proprieta")
'                mp.Proprieta_Val = dRow("Proprieta_Val")
'                mp.inviato = dRow("inviato")
'                mp.datainvio = dRow("datainvio")
'                mp.data_creazione = dRow("data_creazione")
'                mp.data_modifica = dRow("data_modifica")
'                mp.username_creazione = dRow("username_creazione")
'                mp.username_modifica = dRow("username_modifica")
'                mp.validita_inizio = dRow("validita_inizio")
'                mp.validita_fine = dRow("validita_fine")

'                listaObjMP.Add(mp)
'            Next
'        Else
'            listaObjMP = Nothing
'        End If

'        Return listaObjMP
'    End Function

'End Class

'Public Class Materie_PrimexLotto_Proprieta_W

'End Class