'ANCORA NON UTILIZZATO


'Imports System.Runtime.Serialization
'Imports AgronicaCoreDataProvider
'Imports AgronicaCoreDataProvider.UtilityProvider
'Imports AgronicaCoreDataProvider.TipiEnumerativi
'Imports AgronicaCoreDataProvider.CostantiPersonalizzate

'<DataContract()>
'Public Class Materie_Prime_Alias

'    <DataMember()> Public Property Piva_SuperUser() As String
'    <DataMember()> Public Property Piva() As String
'    <DataMember()> Public Property Sa_Cod() As Integer
'    <DataMember()> Public Property Mat_Cod() As Integer
'    <DataMember()> Public Property Mat_Cod_Alias() As Integer

'    <DataMember()> Public Property inviato() As Int16
'    <DataMember()> Public Property datainvio() As DateTime
'    <DataMember()> Public Property data_creazione() As DateTime
'    <DataMember()> Public Property data_modifica() As DateTime
'    <DataMember()> Public Property username_creazione() As String
'    <DataMember()> Public Property username_modifica() As String
'    <DataMember()> Public Property validita_inizio() As DateTime
'    <DataMember()> Public Property validita_fine() As DateTime

'    <DataMember()> Public Property Codice_Lingua() As String

'    Public Sub New()
'        Piva_SuperUser = ""
'        Piva = ""
'        Sa_Cod = PRIVATO
'        Mat_Cod = 0
'        Mat_Cod_Alias = 0

'        inviato = 0
'        datainvio = AGRODATAINIZIO
'        data_creazione = DateTime.Now
'        data_modifica = DateTime.Now
'        username_creazione = ""
'        username_modifica = ""
'        validita_inizio = AGRODATAINIZIO
'        validita_fine = AGRODATAFINE

'        Codice_Lingua = ""
'    End Sub

'End Class

'Public Class Materie_Prime_Alias_R

'    Public Function Leggi( _
'                            ByVal PIVA As String, _
'                            ByVal Mat_Cod As Integer, _
'                            ByVal Mat_Cod_Alias As Integer, _
'                            ByVal xFiltroAggiuntivo As String, _
'                            ByVal xOrderBy As String, _
'                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
'                            ) As List(Of Materie_Prime_Alias)

'        Dim NomeRoutine As String = "AgronicaCoreAnagrafeObject.Materie_Prime_Alias_R.LeggiAlias()"
'        Dim mp_R As New AgronicaCoreAnagrafeDAL.Materie_Prime_Alias_R()
'        Dim listaObjMP As New List(Of Materie_Prime_Alias)

'        'leggo da DB l'oggetto sulla base dei parametri passati
'        Dim dt As DataTable = mp_R.Leggi(PIVA, Mat_Cod, Mat_Cod_Alias, _
'                                         xFiltroAggiuntivo, xOrderBy, objParametri)

'        If Not IsNothing(dt) Then
'            'creo il singolo oggetto e lo aggiungo alla lista
'            For Each dRow As DataRow In dt.Rows

'                Dim mp As New Materie_Prime_Alias()

'                'SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
'                mp.Piva_SuperUser = dRow("PivaSuperUser")
'                mp.Piva = dRow("Piva")
'                mp.Sa_Cod = dRow("Sa_Cod")
'                mp.Mat_Cod = dRow("Mat_Cod")
'                mp.Mat_Cod_Alias = dRow("Mat_Cod_Alias")
'                mp.inviato = dRow("inviato")
'                mp.datainvio = dRow("datainvio")
'                mp.data_creazione = dRow("data_creazione")
'                mp.data_modifica = dRow("data_modifica")
'                mp.username_creazione = dRow("username_creazione")
'                mp.username_modifica = dRow("username_modifica")
'                mp.validita_inizio = dRow("validita_inizio")
'                mp.validita_fine = dRow("validita_fine")
'                mp.Codice_Lingua = dRow("Codice_Lingua")

'                listaObjMP.Add(mp)
'            Next
'        Else
'            listaObjMP = Nothing
'        End If


'        Return listaObjMP
'    End Function


'End Class


'Public Class Materie_Prime_Alias_W





'End Class