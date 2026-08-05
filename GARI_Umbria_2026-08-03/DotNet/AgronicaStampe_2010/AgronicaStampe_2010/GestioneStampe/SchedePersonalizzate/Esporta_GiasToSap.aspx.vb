Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreAnagrafeDAL.Imprese_Read
Imports System.Web.Services
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports Newtonsoft.Json.Linq
Public Class Esporta_GiasToSap
    Inherits System.Web.UI.Page

    Dim BaseCode As Integer

    'oggetto objparametri x server e utenti
    Dim objparametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objparametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri

#Region "Codice generato da Progettazione Web Form"

    'Chiamata richiesta da Progettazione Web Form
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub


    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

    '###########################################################################
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'inizializzazione oggetti objParametri_Utenti e objParametri_Server
        objparametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri
        objparametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objparametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri
        objparametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

    End Sub

    '###########################################################################

    Public Function stampa(ByVal Cooperativa As Object,
                      ByVal PianoSemina As Object,
                      ByVal SpecieVegetali As Object,
                      ByVal Varieta As Object,
                      ByVal DataInizio As Object,
                      ByVal DataFine As Object,
                      ByVal Intervallo As Object) As String

        Dim objparametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri
        objparametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        Dim Dt As DataTable
        Dim TargetURL As String
        Dim strPianoSemina As String
        Dim strCooperativa As String
        Dim strSpecieVeg As String
        Dim strVarieta As String
        Dim strDataInizio As String
        Dim strdataFine As String
        Dim Filtro As String
        Dim i As String
        Dim impr As New AgronicaCoreAnagrafeDAL.Imprese_Read
        Dim Cultivar As New AgronicaCoreMetaSchemaDAL.Cultivar_R

        strSpecieVeg = SpecieVegetali("Testo").ToString

        If DataFine <> Nothing Then
            strdataFine = DataFine.ToString
        Else
            If DataFine Is Nothing Then

                'Se la data fine è nulla
                strdataFine = DateTime.Now.Date
            Else
                'Se è stato richiesto l'anno e non l'intervallo
                strdataFine = ""
            End If
        End If

        If DataInizio <> Nothing Then
            strDataInizio = DataInizio.ToString
        Else

            'Se la data inizio è nulla
            strDataInizio = DateTime.Now.Date
            If strdataFine = "" Then

                'Se il valore Anno è nullo
                strDataInizio = DateTime.Now.Year
            End If
        End If

        Dim objStamp As New AgronicaCoreStampeDAL.SchedePersonalizzate

        Dt = objStamp.Esporta_GiasToSap(Cooperativa, PianoSemina, SpecieVegetali, Varieta, strDataInizio, strdataFine, objparametri_Server)
        Session("Dt_DatiGias") = Dt

        Filtro = "<b>FILTRO SELEZIONATO : </b>"

        For Each i In Cooperativa
            strCooperativa &= impr.RagSoc_from_Piva(i.ToString, objparametri_Server) & ","
        Next

        If strCooperativa <> "" Then
            strCooperativa = Left(strCooperativa, strCooperativa.Length - 1)
            Filtro += "   <b>Cooperativa= </b>" & strCooperativa
        End If

        If PianoSemina(0) <> "" Then
            For Each i In PianoSemina
                strPianoSemina &= i.ToString & ","
            Next
        Else
            strPianoSemina = ""
        End If

        If strPianoSemina <> "" Then
            strPianoSemina = Left(strPianoSemina, strPianoSemina.Length - 1)
            Filtro += "   <b>Piano Semina= </b>" & strPianoSemina
        End If

        If Varieta(0) <> "" Then
            For Each i In Varieta
                strVarieta &= Cultivar.CulDes_from_CulCod(i.ToString, objparametri_Server) & ","
            Next
        Else
            strVarieta = ""
        End If

        If strSpecieVeg <> "" Then
            Filtro += "   <b>Specie Vegetale= </b>" & strSpecieVeg
        End If

        If strVarieta <> "" Then
            strVarieta = Left(strVarieta, strVarieta.Length - 1)
            Filtro += "   <b>Varieta'= </b>" & strVarieta
        End If

        If strDataInizio <> strdataFine Then
            If strdataFine <> "" Then
                Filtro += "   <b>Da= </b>" & strDataInizio
                Filtro += "   <b>A= </b>" & strdataFine
            Else
                Filtro += "   <b>Anno= </b>" & strDataInizio
            End If
        Else
            Filtro += "   <b>Giorno= </b>" & strDataInizio
        End If

        TargetURL = "Esporta_GiasToSap_XLS.aspx" &
                "?f=" &
                Stringa_Codifica(Filtro, AgroKey_EncoderDecoder, Server) &
                "&t=" &
                Stringa_Codifica(Intervallo, AgroKey_EncoderDecoder, Server)

        Return TargetURL

    End Function

End Class