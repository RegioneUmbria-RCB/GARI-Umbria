Imports System.Data
Imports System.Data.OleDb
Imports System.Xml
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class BIO_Notifica_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '============================================================================
    Public Function BIO_Notifica_Cancella(ByVal Notifica_ID As Integer, _
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                            ) As Boolean

        '============================================================================

        Dim objFronte_W As AgronicaCoreBiologicoDAL.BIO_Notifica_Frontespizio_W
        Dim objSezA_W As AgronicaCoreBiologicoDAL.BIO_Notifica_SezA_Informazioni_W
        Dim objSezB_W As AgronicaCoreBiologicoDAL.BIO_Notifica_SezB_Zootecnico_W
        Dim objSezC_W As AgronicaCoreBiologicoDAL.BIO_Notifica_SezC_PreparazioniAlimentari_W
        Dim objSezD_W As AgronicaCoreBiologicoDAL.BIO_Notifica_SezD_Importazione_W
        Dim objSezE_W As AgronicaCoreBiologicoDAL.BIO_Notifica_SezE_Particelle_W
        Dim objSezF_W As AgronicaCoreBiologicoDAL.BIO_Notifica_SezF_Appezzamenti_W
        Dim objSezG_W As AgronicaCoreBiologicoDAL.BIO_Notifica_SezG_Fabbricati_W
   
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False
        Dim MessaggioErrore As String = ""
        Dim xRisp As Boolean = True

        '------------------------------

        Dim NomeRoutine As String = "AgronicaCoreBiologicoBIZ.BIO_Notifica_W.BIO_Notifica_Cancella()"

        '------------------------------

        Try

            '------------------------------
            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, _
                                                                                    FlagTransazioneLocale, _
                                                                                    objParametri)
            '------------------------------

            objFronte_W = New AgronicaCoreBiologicoDAL.BIO_Notifica_Frontespizio_W

            xRisp = objFronte_W.Cancella(Notifica_ID, _
                                        "", _
                                        objParametri)

            If xRisp = True Then

                xRisp = False

                objSezA_W = New AgronicaCoreBiologicoDAL.BIO_Notifica_SezA_Informazioni_W

                xRisp = objSezA_W.Cancella(Notifica_ID, _
                                            "", _
                                            objParametri)

                If xRisp = True Then

                    xRisp = False

                    objSezB_W = New AgronicaCoreBiologicoDAL.BIO_Notifica_SezB_Zootecnico_W

                    xRisp = objSezB_W.Cancella(Notifica_ID, _
                                                "", 0, _
                                                "", _
                                                objParametri)

                    If xRisp = True Then

                        xRisp = False

                        objSezC_W = New AgronicaCoreBiologicoDAL.BIO_Notifica_SezC_PreparazioniAlimentari_W

                        xRisp = objSezC_W.Cancella(Notifica_ID, _
                                                    0, "", 0, _
                                                    "", _
                                                    objParametri)

                        If xRisp = True Then

                            xRisp = False

                            objSezD_W = New AgronicaCoreBiologicoDAL.BIO_Notifica_SezD_Importazione_W

                            xRisp = objSezD_W.Cancella(Notifica_ID, _
                                                        "", 0, _
                                                        "", _
                                                        objParametri)

                            If xRisp = True Then

                                xRisp = False

                                objSezE_W = New AgronicaCoreBiologicoDAL.BIO_Notifica_SezE_Particelle_W

                                xRisp = objSezE_W.Cancella(Notifica_ID, _
                                                            0, _
                                                            "", _
                                                            objParametri)

                                If xRisp = True Then

                                    xRisp = False

                                    objSezF_W = New AgronicaCoreBiologicoDAL.BIO_Notifica_SezF_Appezzamenti_W

                                    xRisp = objSezF_W.Cancella(Notifica_ID, _
                                                                0, 0, _
                                                                "", _
                                                                objParametri)

                                    If xRisp = True Then

                                        xRisp = False

                                        objSezG_W = New AgronicaCoreBiologicoDAL.BIO_Notifica_SezG_Fabbricati_W

                                        xRisp = objSezG_W.Cancella(Notifica_ID, _
                                                                    0, _
                                                                    "", _
                                                                    objParametri)

                                        If xRisp = True Then

                                            'TUTTO OK

                                        Else
                                            Throw New Exception("Errore durante la cancellazione della sezione G.")
                                        End If


                                    Else
                                        Throw New Exception("Errore durante la cancellazione della sezione F.")
                                    End If


                                Else
                                    Throw New Exception("Errore durante la cancellazione della sezione E.")
                                End If


                            Else
                                Throw New Exception("Errore durante la cancellazione della sezione D.")
                            End If


                        Else
                            Throw New Exception("Errore durante la cancellazione della sezione C.")
                        End If


                    Else
                        Throw New Exception("Errore durante la cancellazione della sezione B.")
                    End If


                Else
                    Throw New Exception("Errore durante la cancellazione della sezione A.")
                End If


            Else
                Throw New Exception("Errore durante la cancellazione del Frontespizio.")
            End If

            '------------------------------
            '------------------------------

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)


        Catch ex As Exception

            xRisp = False

            'Faccio il rollback della transazione
            If Not objParametri.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            '//////////////////////////////////////////////////////////////////////
            MessaggioErrore = "(Notifica_ID=" + CStr(Notifica_ID) + ")" + _
                              " : " + ex.Message
            '//////////////////////////////////////////////////////////////////////

            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)
        End Try

        Return xRisp


    End Function


End Class

'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
