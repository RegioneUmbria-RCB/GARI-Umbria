Imports System.Runtime.Serialization
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

<DataContract()>
Public Class Materie_Prime_Dettagli

    <DataMember()> Public Property Piva_SuperUser() As String
    <DataMember()> Public Property Piva() As String
    <DataMember()> Public Property Mat_Cod() As Integer

    <DataMember()> Public Property Extra_Smallint1() As Int16
    <DataMember()> Public Property Extra_Smallint2() As Int16
    <DataMember()> Public Property Extra_Smallint3() As Int16
    <DataMember()> Public Property Extra_Smallint4() As Int16
    <DataMember()> Public Property Extra_Smallint5() As Int16
    <DataMember()> Public Property Extra_Smallint6() As Int16
    <DataMember()> Public Property Extra_Int1() As Integer
    <DataMember()> Public Property Extra_Int2() As Integer
    <DataMember()> Public Property Extra_Int3() As Integer
    <DataMember()> Public Property Extra_Int4() As Integer
    <DataMember()> Public Property Extra_Int5() As Integer
    <DataMember()> Public Property Extra_Int6() As Integer
    <DataMember()> Public Property Extra_Dbl1() As Decimal
    <DataMember()> Public Property Extra_Dbl2() As Decimal
    <DataMember()> Public Property Extra_Dbl3() As Decimal
    <DataMember()> Public Property Extra_Dbl4() As Decimal
    <DataMember()> Public Property Extra_Dbl5() As Decimal
    <DataMember()> Public Property Extra_Dbl6() As Decimal
    <DataMember()> Public Property Extra_Str1() As String
    <DataMember()> Public Property Extra_Str2() As String
    <DataMember()> Public Property Extra_Str3() As String
    <DataMember()> Public Property Extra_Str4() As String
    <DataMember()> Public Property Extra_Str5() As String
    <DataMember()> Public Property Extra_Str6() As String
    <DataMember()> Public Property Extra_Str7() As String
    <DataMember()> Public Property Extra_Str8() As String
    <DataMember()> Public Property Extra_Str9() As String
    <DataMember()> Public Property Extra_Date1() As DateTime
    <DataMember()> Public Property Extra_Date2() As DateTime
    <DataMember()> Public Property Extra_Date3() As DateTime
    <DataMember()> Public Property Extra_Date4() As DateTime
    <DataMember()> Public Property Extra_Date5() As DateTime
    <DataMember()> Public Property Extra_Date6() As DateTime

    <DataMember()> Public Property inviato() As Int16
    <DataMember()> Public Property datainvio() As Date?
    <DataMember()> Public Property data_creazione() As DateTime
    <DataMember()> Public Property data_modifica() As DateTime
    <DataMember()> Public Property username_creazione() As String
    <DataMember()> Public Property username_modifica() As String
    <DataMember()> Public Property validita_inizio() As DateTime
    <DataMember()> Public Property validita_fine() As DateTime

    <DataMember()> Public Property tipo_operazione() As Integer

    Public Sub New()
        Piva_SuperUser = ""
        Piva = ""
        Mat_Cod = 0

        Extra_Smallint1 = 0
        Extra_Smallint2 = 0
        Extra_Smallint3 = 0
        Extra_Smallint4 = 0
        Extra_Smallint5 = 0
        Extra_Smallint6 = 0
        Extra_Int1 = 0
        Extra_Int2 = 0
        Extra_Int3 = 0
        Extra_Int4 = 0
        Extra_Int5 = 0
        Extra_Int6 = 0
        Extra_Dbl1 = 0
        Extra_Dbl2 = 0
        Extra_Dbl3 = 0
        Extra_Dbl4 = 0
        Extra_Dbl5 = 0
        Extra_Dbl6 = 0
        Extra_Str1 = ""
        Extra_Str2 = ""
        Extra_Str3 = ""
        Extra_Str4 = ""
        Extra_Str5 = ""
        Extra_Str6 = ""
        Extra_Str7 = ""
        Extra_Str8 = ""
        Extra_Str9 = ""
        Extra_Date1 = AGRODATAINIZIO
        Extra_Date2 = AGRODATAINIZIO
        Extra_Date3 = AGRODATAINIZIO
        Extra_Date4 = AGRODATAINIZIO
        Extra_Date5 = AGRODATAINIZIO
        Extra_Date6 = AGRODATAINIZIO

        inviato = 0
        datainvio = Nothing
        data_creazione = DateTime.Now
        data_modifica = DateTime.Now
        username_creazione = ""
        username_modifica = ""
        validita_inizio = AGRODATAINIZIO
        validita_fine = AGRODATAFINE

        tipo_operazione = 0
    End Sub

End Class

Public Class Materie_Prime_Dettagli_R

    Public Function Leggi( _
                        ByVal PIVA As String, _
                        ByVal Mat_Cod As Integer, _
                        ByVal ForDelete As Boolean, _
                        ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                        ByVal xFiltroAggiuntivo As String, _
                        ByVal xOrderBy As String, _
                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                        ) As List(Of Materie_Prime_Dettagli)


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeObject.Materie_Prime_Dettagli_R.Leggi()"
        Dim mp_R As New AgronicaCoreAnagrafeDAL.Materie_Prime_Dettagli_R()
        Dim listaObjMP As New List(Of Materie_Prime_Dettagli)

        'leggo da DB l'oggetto sulla base dei parametri passati
        Dim dt As DataTable = mp_R.Leggi(PIVA, Mat_Cod, xSelezioneVariabile, _
                                                xFiltroAggiuntivo, xOrderBy, objParametri)

        If Not IsNothing(dt) Then
            'creo il singolo oggetto e lo aggiungo alla lista
            For Each dRow As DataRow In dt.Rows

                Dim mp As New Materie_Prime_Dettagli()

                mp.tipo_operazione = IIf(ForDelete, 3, 0)
                mp.Piva_SuperUser = dRow("PivaSuperUser")
                mp.Piva = dRow("Piva")
                mp.Mat_Cod = dRow("Mat_Cod")
                mp.Extra_Smallint1 = dRow("Extra_Smallint1")
                mp.Extra_Smallint2 = dRow("Extra_Smallint2")
                mp.Extra_Smallint3 = dRow("Extra_Smallint3")
                mp.Extra_Smallint4 = dRow("Extra_Smallint4")
                mp.Extra_Smallint5 = dRow("Extra_Smallint5")
                mp.Extra_Smallint6 = dRow("Extra_Smallint6")
                mp.Extra_Int1 = dRow("Extra_Int1")
                mp.Extra_Int2 = dRow("Extra_Int2")
                mp.Extra_Int3 = dRow("Extra_Int3")
                mp.Extra_Int4 = dRow("Extra_Int4")
                mp.Extra_Int5 = dRow("Extra_Int5")
                mp.Extra_Int6 = dRow("Extra_Int6")
                mp.Extra_Dbl1 = dRow("Extra_Dbl1")
                mp.Extra_Dbl2 = dRow("Extra_Dbl2")
                mp.Extra_Dbl3 = dRow("Extra_Dbl3")
                mp.Extra_Dbl4 = dRow("Extra_Dbl4")
                mp.Extra_Dbl5 = dRow("Extra_Dbl5")
                mp.Extra_Dbl6 = dRow("Extra_Dbl6")
                mp.Extra_Str1 = dRow("Extra_Str1")
                mp.Extra_Str2 = dRow("Extra_Str2")
                mp.Extra_Str3 = dRow("Extra_Str3")
                mp.Extra_Str4 = dRow("Extra_Str4")
                mp.Extra_Str5 = dRow("Extra_Str5")
                mp.Extra_Str6 = dRow("Extra_Str6")
                mp.Extra_Str7 = dRow("Extra_Str7")
                mp.Extra_Str8 = dRow("Extra_Str8")
                mp.Extra_Str9 = dRow("Extra_Str9")
                mp.Extra_Date1 = dRow("Extra_Date1")
                mp.Extra_Date2 = dRow("Extra_Date2")
                mp.Extra_Date3 = dRow("Extra_Date3")
                mp.Extra_Date4 = dRow("Extra_Date4")
                mp.Extra_Date5 = dRow("Extra_Date5")
                mp.Extra_Date6 = dRow("Extra_Date6")

                mp.inviato = Agro_SQL_Load_ConDefault(dRow("inviato"), mp.inviato.GetType)
                mp.datainvio = DBNullToNothing(dRow("datainvio"))
                mp.data_creazione = Agro_SQL_Load_ConDefault(dRow("data_creazione"), mp.data_creazione.GetType)
                mp.data_modifica = Agro_SQL_Load_ConDefault(dRow("data_modifica"), mp.data_modifica.GetType)
                mp.username_creazione = Agro_SQL_Load_ConDefault(dRow("username_creazione"), mp.username_creazione.GetType)
                mp.username_modifica = Agro_SQL_Load_ConDefault(dRow("username_modifica"), mp.username_modifica.GetType)
                mp.validita_inizio = Agro_SQL_Load_ConDefault(dRow("validita_inizio"), mp.validita_inizio.GetType)
                mp.validita_fine = Agro_SQL_Load_ConDefault(dRow("validita_fine"), mp.validita_fine.GetType)

                listaObjMP.Add(mp)
            Next
        Else
            listaObjMP = Nothing
        End If

        Return listaObjMP
    End Function

End Class

Public Class Materie_Prime_Dettagli_W

    Public Function Scrivi( _
                            ByVal Piva As String, _
                            ByVal Mat_Cod As Long, _
                            ByVal Extra_Smallint1 As Int16, _
                            ByVal Extra_Smallint2 As Int16, _
                            ByVal Extra_Smallint3 As Int16, _
                            ByVal Extra_Smallint4 As Int16, _
                            ByVal Extra_Smallint5 As Int16, _
                            ByVal Extra_Smallint6 As Int16, _
                            ByVal Extra_Int1 As Int32, _
                            ByVal Extra_Int2 As Int32, _
                            ByVal Extra_Int3 As Int32, _
                            ByVal Extra_Int4 As Int32, _
                            ByVal Extra_Int5 As Int32, _
                            ByVal Extra_Int6 As Int32, _
                            ByVal Extra_Dbl1 As Decimal, _
                            ByVal Extra_Dbl2 As Decimal, _
                            ByVal Extra_Dbl3 As Decimal, _
                            ByVal Extra_Dbl4 As Decimal, _
                            ByVal Extra_Dbl5 As Decimal, _
                            ByVal Extra_Dbl6 As Decimal, _
                            ByVal Extra_Str1 As String, _
                            ByVal Extra_Str2 As String, _
                            ByVal Extra_Str3 As String, _
                            ByVal Extra_Str4 As String, _
                            ByVal Extra_Str5 As String, _
                            ByVal Extra_Str6 As String, _
                            ByVal Extra_Str7 As String, _
                            ByVal Extra_Str8 As String, _
                            ByVal Extra_Str9 As String, _
                            ByVal Extra_Date1 As Date, _
                            ByVal Extra_Date2 As Date, _
                            ByVal Extra_Date3 As Date, _
                            ByVal Extra_Date4 As Date, _
                            ByVal Extra_Date5 As Date, _
                            ByVal Extra_Date6 As Date, _
                            ByVal Validita_Inizio As Date, _
                            ByVal Validita_Fine As Date, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                            Optional ByVal Data_creazione As DateTime = #2/1/1900#, _
                            Optional ByVal Data_modifica As DateTime = #2/1/1900#, _
                            Optional ByVal username_creazione As String = "", _
                            Optional ByVal username_modifica As String = "" _
                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeObject.Materie_Prime_Dettagli_W.Scrivi()"
        Dim mp_W As New AgronicaCoreAnagrafeDAL.Materie_Prime_Dettagli_W

        Dim res As Boolean = mp_W.Scrivi(Piva, Mat_Cod, _
                                        Extra_Smallint1, Extra_Smallint2, Extra_Smallint3, Extra_Smallint4, Extra_Smallint5, Extra_Smallint6, _
                                        Extra_Int1, Extra_Int2, Extra_Int3, Extra_Int4, Extra_Int5, Extra_Int6, _
                                        Extra_Dbl1, Extra_Dbl2, Extra_Dbl3, Extra_Dbl4, Extra_Dbl5, Extra_Dbl6, _
                                        Extra_Str1, Extra_Str2, Extra_Str3, Extra_Str4, Extra_Str5, Extra_Str6, Extra_Str7, Extra_Str8, Extra_Str9, _
                                        Extra_Date1, Extra_Date2, Extra_Date3, Extra_Date4, Extra_Date5, Extra_Date6, _
                                        Validita_Inizio, Validita_Fine, objParametri, _
                                        Data_creazione, Data_modifica, username_creazione, username_modifica)

        Return res

    End Function

    Public Function Modifica( _
                            ByVal Old_Piva As String, _
                            ByVal Old_Mat_Cod As Int32, _
                            ByVal New_Extra_Smallint1 As Int16, _
                            ByVal New_Extra_Smallint2 As Int16, _
                            ByVal New_Extra_Smallint3 As Int16, _
                            ByVal New_Extra_Smallint4 As Int16, _
                            ByVal New_Extra_Smallint5 As Int16, _
                            ByVal New_Extra_Smallint6 As Int16, _
                            ByVal New_Extra_Int1 As Int32, _
                            ByVal New_Extra_Int2 As Int32, _
                            ByVal New_Extra_Int3 As Int32, _
                            ByVal New_Extra_Int4 As Int32, _
                            ByVal New_Extra_Int5 As Int32, _
                            ByVal New_Extra_Int6 As Int32, _
                            ByVal New_Extra_Dbl1 As Decimal, _
                            ByVal New_Extra_Dbl2 As Decimal, _
                            ByVal New_Extra_Dbl3 As Decimal, _
                            ByVal New_Extra_Dbl4 As Decimal, _
                            ByVal New_Extra_Dbl5 As Decimal, _
                            ByVal New_Extra_Dbl6 As Decimal, _
                            ByVal New_Extra_Str1 As String, _
                            ByVal New_Extra_Str2 As String, _
                            ByVal New_Extra_Str3 As String, _
                            ByVal New_Extra_Str4 As String, _
                            ByVal New_Extra_Str5 As String, _
                            ByVal New_Extra_Str6 As String, _
                            ByVal New_Extra_Str7 As String, _
                            ByVal New_Extra_Str8 As String, _
                            ByVal New_Extra_Str9 As String, _
                            ByVal New_Extra_Date1 As Date, _
                            ByVal New_Extra_Date2 As Date, _
                            ByVal New_Extra_Date3 As Date, _
                            ByVal New_Extra_Date4 As Date, _
                            ByVal New_Extra_Date5 As Date, _
                            ByVal New_Extra_Date6 As Date, _
                            ByVal New_Validita_Inizio As Date, _
                            ByVal New_Validita_Fine As Date, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                            Optional ByVal Data_modifica As DateTime = #2/1/1900#, _
                            Optional ByVal username_modifica As String = "" _
                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeObject.Materie_Prime_Dettagli_W.Modifica()"
        Dim mp_W As New AgronicaCoreAnagrafeDAL.Materie_Prime_Dettagli_W

        Dim res As Boolean = mp_W.Modifica(Old_Piva, Old_Mat_Cod, _
                                        New_Extra_Smallint1, New_Extra_Smallint2, New_Extra_Smallint3, New_Extra_Smallint4, New_Extra_Smallint5, New_Extra_Smallint6, _
                                        New_Extra_Int1, New_Extra_Int2, New_Extra_Int3, New_Extra_Int4, New_Extra_Int5, New_Extra_Int6, _
                                        New_Extra_Dbl1, New_Extra_Dbl2, New_Extra_Dbl3, New_Extra_Dbl4, New_Extra_Dbl5, New_Extra_Dbl6, _
                                        New_Extra_Str1, New_Extra_Str2, New_Extra_Str3, New_Extra_Str4, New_Extra_Str5, New_Extra_Str6, New_Extra_Str7, New_Extra_Str8, New_Extra_Str9, _
                                        New_Extra_Date1, New_Extra_Date2, New_Extra_Date3, New_Extra_Date4, New_Extra_Date5, New_Extra_Date6, _
                                        New_Validita_Inizio, New_Validita_Fine, xFiltroAggiuntivo, objParametri, _
                                        Data_modifica, username_modifica)


        Return res

    End Function


    Public Function Cancella( _
                            ByVal Piva As String, _
                            ByVal Mat_Cod As Int32, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeObject.Materie_Prime_Dettagli_W.Cancella()"
        Dim mp_W As New AgronicaCoreAnagrafeDAL.Materie_Prime_Dettagli_W

        Dim res As Boolean = mp_W.Cancella(Piva, Mat_Cod, xFiltroAggiuntivo, objParametri)

        Return res

    End Function


End Class