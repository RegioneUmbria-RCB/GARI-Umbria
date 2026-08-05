Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreEntityFramework
Imports System.Xml

Public Class ContixContatti_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function ContixContatti_Scrivi(ByVal DatiConto As String,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean


        '----------------------------------------------------------------------

        Dim NomeRoutine As String = "AgronicaCoreContabBIZ.ContixContatti_W.Conto_Scrivi()"

        '----------------------------------------------------------------------

        Dim XmlDoc As New XmlDocument

        Dim ObjSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
        Dim ObjConto As New AgronicaCoreContabDAL.ContixContatti_W

        Dim xRisp As Boolean
        Dim xDatiContixContatti As XmlNodeList
        Dim xDatiContoxContatto As XmlElement
        Dim xContixContatti As XmlNodeList
        Dim xContoxContatto As XmlElement

        Dim i_DatiContixContatti As Integer
        Dim i_ContixContatti As Integer

        Dim OpeDB_Liquidita As String

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim MessaggioErrore As String


        Try

            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri)
            '------------------------------

            XmlDoc.LoadXml(DatiConto)

            '------------------------------

            xDatiContixContatti = XmlDoc.GetElementsByTagName("DatiConti")

            i_DatiContixContatti = 0

            Do While i_DatiContixContatti < xDatiContixContatti.Count

                'Prelevo l'i-esimo blocco di Dati Liquidità
                xDatiContoxContatto = xDatiContixContatti.Item(i_DatiContixContatti)
                '------------------------------

                xContixContatti = xDatiContoxContatto.GetElementsByTagName("Conto")

                i_ContixContatti = 0

                Do While i_ContixContatti < xContixContatti.Count

                    'Prelevo l' i-esima Liquidità/Risorsa Finanziaria
                    xContoxContatto = xContixContatti.Item(i_ContixContatti)

                    'Prelevo gli attributi del rapporto selezionato
                    OpeDB_Liquidita = xContoxContatto.GetAttribute("TipoOperazioneDB")

                    'Verifico l'operazione richiesta
                    Select Case OpeDB_Liquidita
                        '
                        Case "0"    'LEGGI -------------------------------------------------------
                            '
                        Case "1"    'SALVA -------------------------------------------------------

                            xRisp = ObjConto.Scrivi(
                                CStr(xContoxContatto.GetAttribute(LCase("piva"))),
                                CInt(xContoxContatto.GetAttribute(LCase("cod_conto"))),
                                CStr(xContoxContatto.GetAttribute(LCase("cod_contatto"))),
                                CDate(xContoxContatto.GetAttribute("validita_inizio")),
                                CDate(xContoxContatto.GetAttribute("validita_fine")),
                                objParametri)

                        Case "2"    'MODIFICA -------------------------------------------------------

                        Case "3" 'CANCELLAZIONE

                            xRisp = ObjConto.Cancella(
                                CStr(xContoxContatto.GetAttribute(LCase("piva"))),
                                CInt(xContoxContatto.GetAttribute(LCase("cod_conto"))),
                                CStr(xContoxContatto.GetAttribute(LCase("cod_contatto"))),
                                "", objParametri)

                    End Select

                    '-------------------------------------------------------------

                    'Incremento l'indice
                    i_ContixContatti = i_ContixContatti + 1

                Loop

                '------------------------------

                'Incremento l'indice
                i_DatiContixContatti = i_DatiContixContatti + 1

            Loop

            'Elimino l'oggetto
            ObjConto = Nothing
            '------------------------------

            'Elimino tutti gli oggetti utilizzati

            xContoxContatto = Nothing
            xContixContatti = Nothing
            xDatiContoxContatto = Nothing
            xDatiContixContatti = Nothing
            XmlDoc = Nothing

            xRisp = True

            '------------------------------

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

            '------------------------------

            'Restituisco un valore Dummy
            Return xRisp

        Catch ex As Exception

            'Restituisco un valore Dummy
            xRisp = False

            'Faccio il rollback della transazione
            If Not objParametri.objTransazione Is Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            '//////////////////////////////////////////////////////////////////////
            MessaggioErrore = ex.Message
            '//////////////////////////////////////////////////////////////////////

            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally

            'Chiudo la connessione se è stata aperta in questa routine
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)

        End Try

    End Function

End Class