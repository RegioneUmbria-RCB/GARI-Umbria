Imports System.Web.Services
Imports AgronicaCoreContabBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreVarieBIZ
Imports AgroAgenda_2010.Resources

Public Class Giacenze_MagazzinoUC
    Inherits System.Web.UI.UserControl

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri

    Public Shared Function CaricaGrigliaGiacenze(ByVal piva As String,
                                                 ByVal _prodotto As Integer,
                                                 ByVal _specie As Integer,
                                                 ByVal _varieta As String,
                                                 ByVal _sa_cod As Integer,
                                                 ByVal _dataRif As String,
                                                 ByVal _lotto As String,
                                                 ByVal _calibro As Integer,
                                                 ByVal _qualita As Integer,
                                                 ByVal _certificazione As Integer,
                                                 ByVal _rugginosita As Integer,
                                                 ByVal _imballaggio As Integer,
                                                 ByVal _contenitore As Integer,
                                                 ByVal _confezione As Integer,
                                                 ByVal _chkGiacenzePositive As Boolean,
                                                 ByVal _mostraCampiInput As Boolean,
                                                 Optional ByVal calCodEsclusi As String = Nothing
                                                 ) As RispostaStandard

        Dim dtDati As DataTable = Nothing

        Dim separators() As String = {"|"}
        Dim varietaStringArray As String()
        Dim varietaDaFiltrareArrayInt As Integer() = {}

        If Not _varieta Is Nothing And Not _varieta = "null" And _varieta.Length > 0 Then
            varietaStringArray = _varieta.Split(separators, StringSplitOptions.RemoveEmptyEntries)
            varietaDaFiltrareArrayInt = Array.ConvertAll(varietaStringArray, New Converter(Of String, Integer)(AddressOf StringToInteger))
        End If

        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriUtenti) Then
            r.Sessione = False
            Return r
        End If

        Try

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim leggi As New FF_MagazzinoBIZ
            r.RispostaStringa = leggi.Leggi_Giacenza(piva,
                                                     _prodotto,
                                                     _specie,
                                                     varietaDaFiltrareArrayInt,
                                                     _sa_cod,
                                                     0,
                                                     "210",
                                                     "",
                                                     "0",
                                                     _lotto,
                                                     _calibro,
                                                     _qualita,
                                                     _certificazione,
                                                     _rugginosita,
                                                     _imballaggio,
                                                     _contenitore,
                                                     _confezione,
                                                     0,
                                                     _dataRif,
                                                     "0",
                                                     "5",
                                                     _mostraCampiInput,
                                                     True,
                                                     _chkGiacenzePositive,
                                                     True,
                                                     "",
                                                     "",
                                                     dtDati,
                                                     objParametriServer,
                                                     objParametriUtenti,
                                                     calCodEsclusi:=calCodEsclusi)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ &
                vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    Public Shared Function Controlla_Giacenza(ByVal piva As String,
                                              ByVal Sa_Cod As String,
                                              ByVal strFabbricato_Cod As String,
                                              ByVal elemCod As Integer,
                                              ByVal NomeProdotto As String,
                                              ByVal CodiceProdotto As String,
                                              ByVal Lotto As String,
                                              ByVal Cal_Cod As Integer,
                                              ByVal Data As String,
                                              ByVal cifreArrotondamentoPesi As Integer,
                                              ByVal messaggioDettagliato As Boolean,
                                              ByVal checkPeso As Boolean,
                                              ByVal imballiDaScaricare As Integer,
                                              ByVal contenitoriDaScaricare As Integer,
                                              ByVal confezioniDaScaricare As Integer,
                                              ByVal kgLordiDaScaricare As Decimal,
                                              ByVal kgNettiDaScaricare As Decimal,
                                              ByRef objParametriServer As AgronicaCoreParametri,
                                              ByRef objParametriUtenti As AgronicaCoreParametri
                                              ) As RispostaStandard


        Dim r As New RispostaStandard
        Dim messaggioErrore As String = ""

        Try
            Dim dataRicerca As Date

            If Data = "" Then
                dataRicerca = AGRODATAFINE
            Else
                dataRicerca = CDate(Data)
            End If
            
            Dim leggi As New FF_MagazzinoBIZ
            messaggioErrore =
                leggi.Controllo_Giacenza(piva,
                                         Sa_Cod,
                                         strFabbricato_Cod,
                                         elemCod,
                                         NomeProdotto,
                                         CodiceProdotto,
                                         Lotto,
                                         Cal_Cod,
                                         dataRicerca,
                                         CifreArrotondamentoPesi,
                                         messaggioDettagliato,
                                         checkPeso,
                                         imballiDaScaricare,
                                         contenitoriDaScaricare,
                                         confezioniDaScaricare,
                                         kgLordiDaScaricare,
                                         kgNettiDaScaricare,
                                         objParametriServer,
                                         objParametriUtenti)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    Public Shared Function StringToInteger(st As String) As Integer
        Return CInt(st)
    End Function

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

End Class