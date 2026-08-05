'ANCORA NON UTILIZZATO

'Imports AgronicaCoreDataProvider
'Imports AgronicaCoreDataProvider.UtilityProvider
'Imports AgronicaCoreDataProvider.TipiEnumerativi
'Imports AgronicaCoreDataProvider.CostantiPersonalizzate
'Imports System.Runtime.Serialization

'<DataContract()>
'Public Class Materie_PrimexReport

'    <DataMember()> Public Property Piva() As String
'    <DataMember()> Public Property Pro_Cod() As Integer
'    <DataMember()> Public Property Mat_Cod() As Integer
'    <DataMember()> Public Property Id_Report() As Integer
'    <DataMember()> Public Property inviato() As Int16
'    <DataMember()> Public Property datainvio() As DateTime
'    <DataMember()> Public Property data_creazione() As DateTime
'    <DataMember()> Public Property data_modifica() As DateTime
'    <DataMember()> Public Property username_creazione() As String
'    <DataMember()> Public Property username_modifica() As String
'    <DataMember()> Public Property validita_inizio() As DateTime
'    <DataMember()> Public Property validita_fine() As DateTime

'    Public Sub New()
'        Piva = ""
'        Pro_Cod = 0
'        Mat_Cod = 0
'        Id_Report = 0
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

'Public Class Materie_PrimexReport_R
'    Public Function Leggi( _
'                        ByVal Piva As String, _
'                        ByVal Pro_Cod As Integer, _
'                        ByVal Mat_Cod As Integer, _
'                        ByVal Id_Report As Integer, _
'                        ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
'                        ByVal xFiltroAggiuntivo As String, _
'                        ByVal xOrderBy As String, _
'                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
'                        ) As List(Of Materie_PrimexReport)


'        Dim NomeRoutine As String = "AgronicaCoreAnagrafeObject.Materie_PrimexReport.Leggi()"
'        Dim mp_R As New AgronicaCoreAnagrafeDAL.Materie_PrimexReport_R()
'        Dim listaObjMP As New List(Of Materie_PrimexReport)

'        'leggo da DB l'oggetto sulla base dei parametri passati
'        Dim dt As DataTable = mp_R.Leggi(Piva, Pro_Cod, Mat_Cod, Id_Report, xSelezioneVariabile, _
'                                       xFiltroAggiuntivo, xOrderBy, objParametri)

'        If Not IsNothing(dt) Then
'            'creo il singolo oggetto e lo aggiungo alla lista
'            For Each dRow As DataRow In dt.Rows

'                Dim mp As New Materie_PrimexReport()

'                'SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
'                'SetAttribute("piva_superuser", objParametri.PivaSuperUser)
'                mp.Piva = dRow("Piva")
'                mp.Pro_Cod = dRow("Pro_Cod")
'                mp.Mat_Cod = dRow("Mat_Cod")
'                mp.Id_Report = dRow("Id_Report")
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


'Public Class Materie_PrimexReport_W

'    Public Function Scrivi( _
'                            ByVal Piva As String, _
'                            ByVal Pro_Cod As Int32, _
'                            ByVal Mat_Cod As Int32, _
'                            ByVal Id_Report As Int32, _
'                            ByVal Validita_Inizio As Date, _
'                            ByVal Validita_Fine As Date, _
'                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
'                            ) As Boolean

'        Dim NomeRoutine As String = "AgronicaCoreAnagrafeObject.Materie_PrimexReport_W.Scrivi()"
'        Dim mp_W As New AgronicaCoreAnagrafeDAL.Materie_PrimexReport_W

'        Dim res As Boolean = mp_W.Scrivi(Piva, Pro_Cod, Mat_Cod, Id_Report, _
'                                         Validita_Inizio, Validita_Fine, objParametri)

'        Return res

'    End Function

'    Public Function Scrivi( _
'                            ByVal mp As Materie_PrimexReport, _
'                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
'                            ) As Boolean

'        Dim res As Boolean = Scrivi(mp.Piva, mp.Pro_Cod, mp.Mat_Cod, mp.Id_Report, _
'                                    mp.validita_inizio, mp.validita_fine, objParametri)

'        Return res

'    End Function

'    Public Function Modifica( _
'                                ByVal Old_Piva As String, _
'                                ByVal Old_Pro_Cod As Int32, _
'                                ByVal Old_Mat_Cod As Int32, _
'                                ByVal New_Id_Report As Int32, _
'                                ByVal New_FinestraTemp_Inizio As Date, _
'                                ByVal New_FinestraTemp_Fine As Date, _
'                                ByVal xFiltroAggiuntivo As String, _
'                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
'                                ) As Boolean

'        Dim NomeRoutine As String = "AgronicaCoreAnagrafeObject.Materie_PrimexReport_W.Modifica()"
'        Dim mp_W As New AgronicaCoreAnagrafeDAL.Materie_PrimexReport_W

'        Dim res As Boolean = mp_W.Modifica(Old_Piva, Old_Pro_Cod, Old_Mat_Cod, New_Id_Report, _
'                                    New_FinestraTemp_Inizio, New_FinestraTemp_Fine, _
'                                    xFiltroAggiuntivo, objParametri)

'        Return res

'    End Function

'    Public Function Cancella( _
'                                ByVal Piva As String, _
'                                ByVal Pro_Cod As Int32, _
'                                ByVal Mat_Cod As Int32, _
'                                ByVal Id_Report As Int32, _
'                                ByVal xFiltroAggiuntivo As String, _
'                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
'                                ) As Boolean

'        Dim NomeRoutine As String = "AgronicaCoreAnagrafeObject.Materie_PrimexReport_W.Cancella()"
'        Dim mp_W As New AgronicaCoreAnagrafeDAL.Materie_PrimexReport_W

'        Dim res As Boolean = mp_W.Cancella(Piva, Pro_Cod, Mat_Cod, Id_Report, _
'                                            xFiltroAggiuntivo, objParametri)

'        Return res

'    End Function

'End Class