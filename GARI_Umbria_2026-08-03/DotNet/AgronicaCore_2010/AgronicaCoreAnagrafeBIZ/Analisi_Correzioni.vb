Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports System.IO
Imports AgronicaCoreScadenziario
Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports AgronicaCoreUtility
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Analisi_Correzioni_W
    Inherits AgronicaCoreDataProvider.DataProvider




    Public Function AggiornaCorrezione(ByVal TipoOperazione As enum_TipoOperazioneDB,
                                       ByVal piva As String,
                                       ByVal sa_cod As Integer,
                                       ByVal id_correzione_testata As Integer,
                                       ByVal nome As String,
                                       ByVal analisi_parametro_cod As Integer,
                                       ByVal scala_definizione As Integer,
                                       ByVal scala_inizio As Decimal,
                                       ByVal scala_fine As Decimal,
                                       ByVal note As String,
                                       ByVal validita_inizio As DateTime,
                                       ByVal validita_fine As DateTime,
                                       ByVal righeGrid_Dettagli As String,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                       ) As Integer


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Analisi_Correzione_W.AggiornaCorrezione()"
        Dim strErr As String = ""
        Dim FlagConnessioneLocale, FlagTransazioneLocale As Boolean
        Try

            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, objParametri)

            Dim objSeq As New AgronicaCoreDataProvider.Agro_Sequenze
            Dim objCorrezione As New AgronicaCoreAnagrafeDAL.Analisi_Correzioni_Testate_W
            Dim objCorrezioneDettagli As New AgronicaCoreAnagrafeDAL.Analisi_Correzioni_Dettagli_W
            Dim PivaSuperUser = objParametri.PivaSuperUser
            Dim Validita_Inizio_Dettaglio As Date = AGRODATAINIZIO
            Dim Validita_Fine_Dettaglio As Date = AGRODATAFINE


            ' forzati
            Dim Base, Top As Integer
            Base = 0
            Top = 2000000000

            Select Case TipoOperazione

                Case enum_TipoOperazioneDB.Scrittura

                    'Nuova Correzione
                    id_correzione_testata = objSeq.NuovoId_Tabella("Analisi_Correzioni_Testata", Base, Top, objParametri)

                    'Salvataggio Indice
                    objCorrezione.Scrivi(piva, sa_cod, id_correzione_testata, nome, analisi_parametro_cod, scala_definizione, scala_inizio, scala_fine, note, validita_inizio, validita_fine, objParametri)


                Case enum_TipoOperazioneDB.Modifica


                    'Modifica Correzione
                    objCorrezione.Modifica(piva, sa_cod, id_correzione_testata, nome, analisi_parametro_cod, scala_definizione, scala_inizio, scala_fine, note, validita_inizio, validita_fine, objParametri)


                Case enum_TipoOperazioneDB.Cancellazione

                    'Cancellazione Correzione
                    objCorrezione.Cancella(piva, id_correzione_testata, "", objParametri)
                    objCorrezioneDettagli.Cancella(piva, id_correzione_testata, "", objParametri)



            End Select



            If righeGrid_Dettagli <> "" And righeGrid_Dettagli <> "[]" Then

                'Cancellazione Preventiva
                objCorrezioneDettagli.Cancella(piva, id_correzione_testata, "", objParametri)

                For Each obj As JObject In JArray.Parse(righeGrid_Dettagli)

                    objCorrezioneDettagli.Scrivi(piva, id_correzione_testata, CDec(obj("Valore_Sangue")), CDec(obj("Valore_Correzione")), objParametri)

                Next

            End If



            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

        Catch ex As Exception
            If Not objParametri.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If



            strErr = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, strErr)

            Throw ex

        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)

        End Try

        Return id_correzione_testata

    End Function

End Class

