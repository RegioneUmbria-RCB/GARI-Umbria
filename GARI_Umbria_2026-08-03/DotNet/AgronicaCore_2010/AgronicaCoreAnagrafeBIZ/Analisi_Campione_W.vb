Imports System.Data
Imports System.Data.OleDb
Imports System.Xml
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider



'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

Public Class Analisi_Campione_W
    Inherits AgronicaCoreDataProvider.LogProvider


    Public Function Campione_Scrivi(
                                ByVal DatiCampione As String,
                                ByVal Testata_Cod As Long,
                                ByVal Dettaglio_Cod As Long,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AnagrafeBIZ.Analisi_Campione_W.Campione_Scrivi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese dell'utente
        '
        '====================================================================================

        Dim MessaggioErrore As String = ""

        Dim XmlDoc As XmlDocument

        Dim OpeDB_Campione As String

        Dim GraficaPresente As Boolean
        Dim GraphicKey As String
        Dim Campione_Cod As Long

        Dim Dummy As Integer
        Dim objSequenze As New AgronicaCoreDataProvider.Agro_Sequenze

        Dim objCampionixDettagli As New AgronicaCoreAnagrafeDAL.Analisi_CampionexDet_W

        Dim objCampioni As New AgronicaCoreAnagrafeDAL.Analisi_Campione_W

        Dim objGrafica As New AgronicaCoreGraficaBIZ.Grafica_Write
        Dim objDett As New AgronicaCoreAnagrafeDAL.Analisi_CampionexDet_R

        Dim XmlDatiCampioni As XmlNodeList
        Dim XmlDatiCampione As XmlElement
        Dim xCampioni As XmlNodeList
        Dim xCampione As XmlElement

        Dim xEntitaGrafiche As XmlNodeList
        Dim i_Campione As Integer

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim xRisp As Boolean

        Try

            '------------------------------

            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri)

            ''Se la connessione è chiusa la apro
            'If objParametri.objConnessione Is Nothing Then
            '    objParametri.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
            '    objParametri.objConnessione.Open()
            '    FlagConnessioneLocale = True
            'End If

            'If objParametri.objTransazione Is Nothing Then
            '    objParametri.objTransazione = objParametri.objConnessione.BeginTransaction
            '    FlagTransazioneLocale = True
            'End If

            '------------------------------

            XmlDoc = New Xml.XmlDocument
            XmlDoc.LoadXml(DatiCampione)

            '------------------------------




            '-------------------------------------------------------------
            ' CAMPIONI
            '-------------------------------------------------------------

            XmlDatiCampioni = XmlDoc.GetElementsByTagName("DatiCampioni")

            If Not XmlDatiCampioni Is Nothing Then

                If XmlDatiCampioni.Count > 0 Then

                    XmlDatiCampione = XmlDatiCampioni.Item(0)

                    xCampioni = XmlDatiCampione.GetElementsByTagName("Campione")


                    If Not xCampioni Is Nothing Then

                        If xCampioni.Count > 0 Then


                            i_Campione = 0

                            Do While i_Campione < xCampioni.Count

                                'Prelevo l'i-esimo campione
                                xCampione = xCampioni.Item(i_Campione)

                                'Prelevo gli attributi del codice selezionato
                                OpeDB_Campione = xCampione.GetAttribute("TipoOperazioneDB")



                                'Set objCampioni = New Agro_Anagrafe_AD.Analisi_Campioni_Write


                                'Verifico se è presente un tag per la grafica, se c'è mi calcolo già la chiave GraphicKey per la tabella grafica
                                xEntitaGrafiche = Nothing
                                GraficaPresente = False
                                GraphicKey = ""




                                'Verifico l'operazione richiesta
                                Select Case OpeDB_Campione
                                    '
                                    Case "0"    'LEGGI -------------------------------------------------------
                                        '
                                    Case "1"    'SALVA -------------------------------------------------------
                                        '
                                        If CLng(xCampione.GetAttribute("analisi_campione_cod")) <= 0 Then


                                            Campione_Cod = objSequenze.NuovoId_Tabella(CStr("ANALISI_CAMPIONI"), _
                                                                                        CLng(xCampione.GetAttribute("basecode")), _
                                                                                        CLng(xCampione.GetAttribute("topcode")), _
                                                                                        objParametri)

                                        Else
                                            Campione_Cod = CLng(xCampione.GetAttribute("analisi_campione_cod"))
                                        End If

                                        xEntitaGrafiche = xCampione.GetElementsByTagName("DatiEntita")

                                        If Not xEntitaGrafiche Is Nothing Then

                                            If xEntitaGrafiche.Count = 1 Then
                                                GraficaPresente = True
                                                GraphicKey = "X" & Right("0" & Hex(Campione_Cod), 8)
                                            Else
                                                GraficaPresente = False
                                                GraphicKey = ""
                                            End If

                                        Else
                                            GraficaPresente = False
                                            GraphicKey = ""
                                        End If

                                        'CAMPIONIXDETTAGLI
                                        Dummy = objCampionixDettagli.Scrivi(Testata_Cod, _
                                                                         Dettaglio_Cod, _
                                                                         Campione_Cod, _
                                                                         0, _
                                                                         CDate(xCampione.GetAttribute("validita_inizio")), _
                                                                         CDate(xCampione.GetAttribute("validita_fine")), _
                                                                         objParametri)


                                        'CAMPIONI

                                        Dummy = objCampioni.Scrivi(Campione_Cod, _
                                                                CStr(xCampione.GetAttribute("analisi_campione_des")), _
                                                                CDbl(xCampione.GetAttribute("analisi_campione_coord_x")), _
                                                                CDbl(xCampione.GetAttribute("analisi_campione_coord_y")), _
                                                                CDbl(xCampione.GetAttribute("analisi_campione_quantita")), _
                                                                CLng(xCampione.GetAttribute("analisi_campione_udm")), _
                                                                CDbl(xCampione.GetAttribute("analisi_campione_profondita")), _
                                                                CDbl(xCampione.GetAttribute("analisi_campione_profondita_min")), _
                                                                CDbl(xCampione.GetAttribute("analisi_campione_profondita_max")), _
                                                                CStr(xCampione.GetAttribute("analisi_campione_riferimento_1")), CStr(xCampione.GetAttribute("analisi_campione_riferimento_2")), CStr(xCampione.GetAttribute("analisi_campione_riferimento_3")), CStr(xCampione.GetAttribute("analisi_campione_riferimento_4")), CStr(xCampione.GetAttribute("analisi_campione_riferimento_5")), _
                                                                CStr(xCampione.GetAttribute("analisi_campione_note")), _
                                                                CStr(xCampione.GetAttribute("analisi_campione_key_piva")), CLng(xCampione.GetAttribute("analisi_campione_key_sacod")), CStr(GraphicKey), _
                                                                0, _
                                                                CStr(xCampione.GetAttribute("analisi_campione_prov")), CStr(xCampione.GetAttribute("analisi_campione_com")), CStr(xCampione.GetAttribute("analisi_campione_sezione")), _
                                                                CLng(xCampione.GetAttribute("analisi_campione_foglio")), CLng(xCampione.GetAttribute("analisi_campione_numero")), CStr(xCampione.GetAttribute("analisi_campione_subalterno")), _
                                                                CDate(xCampione.GetAttribute("validita_inizio")), _
                                                                CDate(xCampione.GetAttribute("validita_fine")), _
                                                                objParametri)







                                    Case "2"    'MODIFICA -------------------------------------------------------

                                        xEntitaGrafiche = xCampione.GetElementsByTagName("DatiEntita")

                                        If Not xEntitaGrafiche Is Nothing Then

                                            If xEntitaGrafiche.Count = 1 Then
                                                GraficaPresente = True
                                                GraphicKey = "X" & Right("0" & Hex(CLng(xCampione.GetAttribute("analisi_campione_cod"))), 8)
                                            Else
                                                GraficaPresente = False
                                                GraphicKey = ""
                                            End If

                                        Else
                                            GraficaPresente = False
                                            GraphicKey = ""
                                        End If

                                        Campione_Cod = CLng(xCampione.GetAttribute("analisi_campione_cod"))



                                        'CAMPIONIXDETTAGLI
                                        objCampionixDettagli.Modifica2(Testata_Cod, _
                                                                        Dettaglio_Cod, _
                                                                        CLng(xCampione.GetAttribute("analisi_campione_cod")), _
                                                                        0, _
                                                                        CDate(xCampione.GetAttribute("validita_inizio")), _
                                                                        CDate(xCampione.GetAttribute("validita_fine")), _
                                                                        objParametri)


                                        'CAMPIONI
                                        objCampioni.Modifica(CLng(xCampione.GetAttribute("analisi_campione_cod")), _
                                                          CStr(xCampione.GetAttribute("analisi_campione_des")), _
                                                          CDbl(xCampione.GetAttribute("analisi_campione_coord_x")), _
                                                          CDbl(xCampione.GetAttribute("analisi_campione_coord_y")), _
                                                          CDbl(xCampione.GetAttribute("analisi_campione_quantita")), _
                                                          CLng(xCampione.GetAttribute("analisi_campione_udm")), _
                                                          CDbl(xCampione.GetAttribute("analisi_campione_profondita")), _
                                                          CDbl(xCampione.GetAttribute("analisi_campione_profondita_min")), _
                                                          CDbl(xCampione.GetAttribute("analisi_campione_profondita_max")), _
                                                          CStr(xCampione.GetAttribute("analisi_campione_riferimento_1")), CStr(xCampione.GetAttribute("analisi_campione_riferimento_2")), CStr(xCampione.GetAttribute("analisi_campione_riferimento_3")), CStr(xCampione.GetAttribute("analisi_campione_riferimento_4")), CStr(xCampione.GetAttribute("analisi_campione_riferimento_5")), _
                                                          CStr(xCampione.GetAttribute("analisi_campione_note")), _
                                                          CStr(xCampione.GetAttribute("analisi_campione_key_piva")), CLng(xCampione.GetAttribute("analisi_campione_key_sacod")), CStr(GraphicKey), _
                                                          0, _
                                                          CStr(xCampione.GetAttribute("analisi_campione_prov")), CStr(xCampione.GetAttribute("analisi_campione_com")), CStr(xCampione.GetAttribute("analisi_campione_sezione")), _
                                                          CLng(xCampione.GetAttribute("analisi_campione_foglio")), CLng(xCampione.GetAttribute("analisi_campione_numero")), CStr(xCampione.GetAttribute("analisi_campione_subalterno")), _
                                                          CDate(xCampione.GetAttribute("validita_inizio")), CDate(xCampione.GetAttribute("validita_fine")), _
                                                          objParametri)


                                        '
                                    Case "3"    'ELIMINA -------------------------------------------------------
                                        '
                                        objCampionixDettagli.Cancella( _
                                                    CLng(xCampione.GetAttribute("analisi_campione_cod")), _
                                                    "", _
                                                    objParametri)
                                        '

                                        objCampioni.Cancella( _
                                                    CStr(xCampione.GetAttribute("analisi_campione_cod")), _
                                                    "", _
                                                    objParametri)
                                        '
                                End Select

                                '-------------------------------------------------------------
                                ' ENTITA GRAFICHE
                                '-------------------------------------------------------------


                                If GraficaPresente Then
                                    Dummy = objGrafica.Grafica_Scrivi2005(xEntitaGrafiche.Item(0).OuterXml, _
                                                            CStr(xCampione.GetAttribute("analisi_campione_key_piva")), _
                                                            CLng(xCampione.GetAttribute("analisi_campione_key_sacod")), _
                                                            Campione_Cod, _
                                                             False, objParametri)
                                End If

                                i_Campione = i_Campione + 1

                            Loop

                        End If

                    End If

                End If

            End If
            '------------------------------

            xRisp = True

            'If FlagTransazioneLocale = True Then
            '    objParametri.objTransazione.Commit()
            'End If
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)


        Catch ex As Exception

            xRisp = False

            'Faccio il rollback della transazione
            If Not objParametri.objTransazione Is Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)


        Finally
            ''Chiudo la connessione se è stata aperta in questa routine
            'If (FlagConnessioneLocale = True) AndAlso (Not objParametri.objConnessione Is Nothing) Then
            '    objParametri.objConnessione.Close()
            'End If
            'Chiudo la connessione se è stata aperta in questa routine
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)

        End Try

        'Restituisco il risultato
        Return xRisp

    End Function



End Class