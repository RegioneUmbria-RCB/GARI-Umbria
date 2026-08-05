Imports System.Data.OleDb
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class PUA_R

    Public Function LeggiG2G() As String

    End Function

End Class

Public Class PUA_W
    Inherits AgronicaCoreDataProvider.DataProvider
    Public Function Pua_Scrivi(ByVal Tipo_Operazione As Integer,
                               ByVal Piva As String,
                               ByRef PUA_Cod As Integer,
                               ByVal Id_Servizio As Integer,
                               ByRef objParametri As AgronicaCoreParametri
                                  ) As Boolean

        '----------------------------------------------------------------------
        Dim nomeRoutine As String = "AgronicaCorePUA_BIZ.PUA_W.Pua_Scrivi()"
        '----------------------------------------------------------------------
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim xRisp As Boolean = True
        Dim MessaggioErrore As String = ""

        Dim BoolDummy As Boolean

        Try

            '------------------------------
            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri)

            Select Case Tipo_Operazione

                Case enum_TipoOperazioneDB.Scrittura




                Case enum_TipoOperazioneDB.Cancellazione

                    'elimino PUA_TrattamentiEffluenti
                    Dim objPua_TrattamentoEffluenti As New AgronicaCorePUA_DAL.Pua_TrattamentoEffluenti_W
                    BoolDummy = objPua_TrattamentoEffluenti.Cancella(PUA_Cod, 0, 0, 0, "", objParametri)

                    If BoolDummy = False Then
                        Throw New Exception("Impossibile eliminare TrattamentoEffluenti")
                    End If

                    'elimino PUA_ConsistenzeAnimali
                    Dim objPUA_Consistenze_Animali As New AgronicaCorePUA_DAL.PUA_Consistenze_Animali_W
                    BoolDummy = objPUA_Consistenze_Animali.Cancella(PUA_Cod, 0, 0, "", objParametri)

                    If BoolDummy = False Then
                        Throw New Exception("Impossibile eliminare Consistenze_Animali")
                    End If


                    'elimino PUA_Effluente
                    Dim objPua_Effluente As New AgronicaCorePUA_DAL.Pua_Effluente_W
                    BoolDummy = objPua_Effluente.Cancella(PUA_Cod, 0, 0, "", objParametri)

                    If BoolDummy = False Then
                        Throw New Exception("Impossibile eliminare PuaEffluente")
                    End If

                    'elimino PUA_Programmazione
                    Dim objPua_Programmazione As New AgronicaCorePUA_DAL.Pua_Programmazione_W
                    BoolDummy = objPua_Programmazione.Cancella(PUA_Cod, 0, 0, "", objParametri)

                    If BoolDummy = False Then
                        Throw New Exception("Impossibile eliminare PuaProgrammazione")
                    End If

                    'elimino PUA_Testata
                    Dim objPUA_Testata As New AgronicaCorePUA_DAL.PUA_Testata_W
                    BoolDummy = objPUA_Testata.Cancella(Piva, PUA_Cod, 0, "", objParametri)

                    If BoolDummy = False Then
                        Throw New Exception("Impossibile eliminare PUATestata")
                    End If

            End Select

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

        Catch ex As Exception

            AgronicaCoreDataProvider.ConnessioniTransazioni.RollBackTransazione(objParametri)

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)

        Finally

            'Chiudo la connessione se è stata aperta in questa routine
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)

        End Try

        Return xRisp

    End Function

End Class

