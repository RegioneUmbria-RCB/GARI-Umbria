Imports System.Runtime.Serialization
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

<DataContract()>
Public Class Prodotti_Costi
    <DataMember()> Public Property ID() As Integer
    <DataMember()> Public Property Piva() As String
    <DataMember()> Public Property Riferimento() As String
    <DataMember()> Public Property Elem_Cod() As Integer
    <DataMember()> Public Property Pro_Cod() As Integer
    <DataMember()> Public Property Mat_Cod() As Integer
    <DataMember()> Public Property Udm_Cod() As Integer
    <DataMember()> Public Property Mezzo() As Int16
    <DataMember()> Public Property Prezzo_Unitario() As Decimal
    <DataMember()> Public Property Veg_Cod() As Integer
    <DataMember()> Public Property Cul_Cod() As Integer
    <DataMember()> Public Property Id_Budget() As Integer

    <DataMember()> Public Property inviato() As Int16
    <DataMember()> Public Property datainvio() As DateTime
    <DataMember()> Public Property data_creazione() As DateTime
    <DataMember()> Public Property data_modifica() As DateTime
    <DataMember()> Public Property username_creazione() As String
    <DataMember()> Public Property username_modifica() As String
    <DataMember()> Public Property validita_inizio() As DateTime
    <DataMember()> Public Property validita_fine() As DateTime

    <DataMember()> Public Property tipo_operazione() As Integer

    Public Sub New()
        ID = 0
        Piva = ""
        Riferimento = ""
        Elem_Cod = 0
        Pro_Cod = 0
        Mat_Cod = 0
        Udm_Cod = 0
        Mezzo = 0
        Prezzo_Unitario = 0
        Veg_Cod = 0
        Cul_Cod = 0
        Id_Budget = 0

        inviato = 0
        datainvio = AGRODATAINIZIO
        data_creazione = AGRODATAINIZIO
        data_modifica = AGRODATAINIZIO
        username_creazione = ""
        username_modifica = ""
        validita_inizio = AGRODATAINIZIO
        validita_fine = AGRODATAINIZIO

        tipo_operazione = 0
    End Sub

End Class

Public Class Prodotti_Costi_W
    Inherits AgronicaCoreDataProvider.DataProvider

    'Public Function Scrivi( _
    '                        ByVal Piva As String, _
    '                        ByVal Riferimento As String, _
    '                        ByVal Elem_Cod As Int32, _
    '                        ByVal Pro_Cod As Int32, _
    '                        ByVal Mat_Cod As Int32, _
    '                        ByVal Udm_Cod As Int32, _
    '                        ByVal Mezzo As Integer, _
    '                        ByVal Prezzo_Unitario As Decimal, _
    '                        ByVal Veg_Cod As Int32, _
    '                        ByVal Cul_Cod As Int32, _
    '                        ByVal Validita_Inizio As Date, _
    '                        ByVal Validita_Fine As Date, _
    '                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
    '                        Optional ByVal Data_creazione As DateTime = #2/1/1900#, _
    '                        Optional ByVal Data_modifica As DateTime = #2/1/1900#, _
    '                        Optional ByVal username_creazione As String = "", _
    '                        Optional ByVal username_modifica As String = "" _
    '                        ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreContabObject.Prodotti_Costi_W.Scrivi()"
    '    Dim pc As New AgronicaCoreContabDAL.Prodotti_Costi_W

    '    Dim res As Boolean = pc.Scrivi(Piva, Riferimento, Elem_Cod, Pro_Cod, Mat_Cod, Udm_Cod, _
    '                                      Mezzo, Prezzo_Unitario, Veg_Cod, Cul_Cod, _
    '                                      Validita_Inizio, Validita_Fine, objParametri, _
    '                                      Data_creazione, Data_modifica, username_creazione, username_modifica)
    '    Return res

    'End Function

    'Public Function Cancella( _
    '                        ByVal Piva As String, _
    '                        ByVal Riferimento As String, _
    '                        ByVal Elem_Cod As Int32, _
    '                        ByVal Pro_Cod As Int32, _
    '                        ByVal Mat_Cod As Int32, _
    '                        ByVal Veg_Cod As Int32, _
    '                        ByVal Cul_Cod As Int32, _
    '                        ByVal xFiltroAggiuntivo As String, _
    '                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                        ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreContabObject.Prodotti_Costi_W.Cancella()"
    '    Dim pc As New AgronicaCoreContabDAL.Prodotti_Costi_W

    '    Dim res As Boolean = pc.Cancella(Piva, Riferimento, Elem_Cod, Pro_Cod, Mat_Cod, _
    '                                     Veg_Cod, Cul_Cod, xFiltroAggiuntivo, objParametri)
    '    Return res

    'End Function

    Public Function Prodotti_Costi_Scrivi( _
                                           ByVal ListaProdotti_Costi As List(Of Prodotti_Costi), _
                                           ByVal Mat_Cod As Int32, _
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                           ) As Boolean

        '----------------------------------------------------------------------
        Dim NomeRoutine As String = "AgronicaCoreContabObject.Prodotti_Costi_W.Prodotti_Costi_Scrivi()"

        Dim ObjProdotti_Costi_W As New AgronicaCoreContabDAL.Prodotti_Costi_W
        Dim objProdotti_Costi As Prodotti_Costi

        Dim Dummy As Boolean

        '------------------------------
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim MessaggioErrore As String = ""
        Dim xRisp As Boolean = True
        '------------------------------


        Try
            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, _
                                                                                    FlagTransazioneLocale, _
                                                                                    objParametri)

            'For Each objProdotti_Costi As Prodotti_Costi In ListaProdotti_Costi
            For i As Integer = 0 To ListaProdotti_Costi.Count - 1
                objProdotti_Costi = ListaProdotti_Costi(i)

                'Verifico l'operazione richiesta
                Select Case objProdotti_Costi.tipo_operazione

                    Case "0"      'LEGGI -------------------------------------------------------

                    Case "1", "2" 'SALVA MODIFICA -------------------------------------------------------

                        'La prima volta cancello il vecchio storico prezzi
                        If i = 0 And (objProdotti_Costi.Pro_Cod <> 0 Or objProdotti_Costi.Mat_Cod <> 0) Then

                            'Cancellazione Vecchi Valori
                            ObjProdotti_Costi_W.Cancella( _
                                        objProdotti_Costi.Piva, _
                                        objProdotti_Costi.Riferimento, _
                                        objProdotti_Costi.Elem_Cod, _
                                        objProdotti_Costi.Pro_Cod, _
                                        objProdotti_Costi.Mat_Cod, _
                                        0, _
                                        0, _
                                        "", _
                                        objParametri)

                        End If

                        Dummy = ObjProdotti_Costi_W.Scrivi( _
                                        objProdotti_Costi.Piva, _
                                        objProdotti_Costi.Riferimento, _
                                        objProdotti_Costi.Elem_Cod, _
                                        objProdotti_Costi.Pro_Cod, _
                                        Mat_Cod, _
                                        objProdotti_Costi.Udm_Cod, _
                                        objProdotti_Costi.Mezzo, _
                                        objProdotti_Costi.Prezzo_Unitario, _
                                        objProdotti_Costi.Veg_Cod, _
                                        objProdotti_Costi.Cul_Cod, _
                                        objProdotti_Costi.validita_inizio, _
                                        objProdotti_Costi.validita_fine, _
                                        objParametri)

                    Case "3"   'CANCELLAZIONE

                        Dummy = ObjProdotti_Costi_W.Cancella("", "", _
                                                    objProdotti_Costi.Elem_Cod, _
                                                    objProdotti_Costi.Pro_Cod, _
                                                    objProdotti_Costi.Mat_Cod, _
                                                    0, _
                                                    0, _
                                                    "", _
                                                    objParametri)
                End Select

            Next

            xRisp = True 'Se è andato tutto bene...

            'Se ho la transazione è stata avviata in questa routine faccio il commit
            If FlagTransazioneLocale = True Then
                objParametri.objTransazione.Commit()
            End If

            '----------------------------------------------------------------------------

        Catch ex As Exception
            'Restituisco un valore Dummy
            xRisp = False

            'Faccio il rollback della transazione
            If Not IsNothing(objParametri.objTransazione) Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally
            'Chiudo la connessione se è stata aperta in questa routine
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)
        End Try

        Return xRisp

    End Function

End Class
