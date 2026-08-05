Imports System.Data.Entity
Imports System.Linq
Imports System.Transactions
Imports System.Xml
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json.Linq
Imports System.Web.UI.WebControls
Imports AgronicaCoreGisBIZ
Imports Newtonsoft.Json
Imports AgronicaCoreModelsSTD.metaschema.utilizzi
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreModelsSTD.costanti
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreModelsSTD.anagrafiche

Public Class UnitaMisura_Alternativa_R
    Inherits AgronicaCoreDataProvider.LogProvider

    ''' <summary>
    ''' Legge la lista di possibili conversioni per l'unità di misura passata
    ''' </summary>
    ''' <param name="Udm_From"></param>
    ''' <param name="objParametri"></param>
    ''' <param name="xSelezioneVariabile">Di base impostato con join comprese per descrizioni udm</param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <returns>DT con possibili conversioni</returns>
    Public Function Leggi_Conversioni(ByVal Udm_From As Integer,
                                      ByRef objParametri As AgronicaCoreParametri,
                                      Optional ByVal xSelezioneVariabile As enumSelezioneVariabile = enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                      Optional ByVal xFiltroAggiuntivo As String = "",
                                      Optional ByVal xOrderBy As String = "") As DataTable
        Dim NomeRoutine As String = "AnagrafeDAL.UnitaMisura_Alternativa_Read.Leggi_Conversioni()"

        Dim dtConversioni As DataTable

        Dim messaggioErrore As String = ""
        Dim FlagConnessioneLocale As Boolean = False

        Try
            If IsNothing(objParametri.objConnessione) Then
                FlagConnessioneLocale = True
                objParametri.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
            End If

            Dim objUdmAlt_R As New AgronicaCoreAnagrafeDAL.UnitaMisura_Alternativa_Read
            dtConversioni = objUdmAlt_R.Leggi(Udm_From, 0,
                                              xSelezioneVariabile, xFiltroAggiuntivo, xOrderBy,
                                              objParametri)

        Catch ex As Exception
            dtConversioni = Nothing
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, messaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & messaggioErrore)
        Finally
            If FlagConnessioneLocale = True Then
                objParametri.objConnessione.Close()
                objParametri.objConnessione.Dispose()
            End If
        End Try

        Return dtConversioni

    End Function

    ''' <summary>
    ''' Legge il tasso di conversione per passare dall'unità di misura originale a quella alternativa
    ''' </summary>
    ''' <param name="Udm_From"></param>
    ''' <param name="Udm_Alt"></param>
    ''' <param name="objParametri"></param>
    ''' <returns>Tasso di conversione come Double</returns>
    Public Function Leggi_TassoConversione(ByVal Udm_From As Integer,
                                           ByVal Udm_Alt As Integer,
                                           ByRef objParametri As AgronicaCoreParametri) As Double
        Dim NomeRoutine As String = "AnagrafeDAL.UnitaMisura_Alternativa_Read.Leggi_TassoConversione()"

        Dim tasso As Double = 1.0

        Dim messaggioErrore As String = ""
        Dim FlagConnessioneLocale As Boolean = False

        Try
            If IsNothing(objParametri.objConnessione) Then
                FlagConnessioneLocale = True
                objParametri.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
            End If

            Dim objUdmAlt_R As New AgronicaCoreAnagrafeDAL.UnitaMisura_Alternativa_Read
            Dim dt As DataTable = objUdmAlt_R.Leggi(Udm_From, Udm_Alt,
                                                    enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                    "", "", objParametri)

            If Not IsNothing(dt) AndAlso dt.Rows.Count = 1 Then
                tasso = dt(0)("Tasso_Conv")
            Else
                Throw New Exception("Esiste più di una configurazione per la stessa unità di misura")
            End If

        Catch ex As Exception
            tasso = 1.0
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, messaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & messaggioErrore)

        Finally
            If FlagConnessioneLocale = True Then
                objParametri.objConnessione.Close()
                objParametri.objConnessione.Dispose()
            End If
        End Try

        Return tasso

    End Function

End Class


Public Class UnitaMisura_Alternativa_W

End Class

