Imports System.Xml
Imports AgronicaCoreDataProvider


Public Class Rapporti_Contabili_W
    Inherits AgronicaCoreDataProvider.LogProvider

    '============================================================================
    Public Function RapCon_Scrivi(ByVal DatiRapCon As String,
                                  ByRef Cod_Rapporto As Integer,
                                  ByRef objParametri As AgronicaCoreParametri
                                  ) As Boolean
        '============================================================================

        Dim XmlDoc As XmlDocument

        Dim ObjSequenze As New Agro_Sequenze
        Dim objRapCon As New AgronicaCoreAnagrafeDAL.Rapporti_Contabili_W
        Dim objRisorse_Umane As New AgronicaCoreAnagrafeDAL.Risorse_Umane_W

        Dim Dummy As Boolean
        'Dim Cod_Rapporto As Long

        Dim xDatiRapCons As XmlNodeList
        Dim xDatiRapCon As XmlElement
        Dim xRapCons As XmlNodeList
        Dim xRapCon As XmlElement

        Dim i_DatiRapCon As Integer
        Dim i_RapCon As Integer

        Dim OpeDB_RapCon As String

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim MessaggioErrore As String

        Dim Agente, Consulente As Integer

        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.Rapporti_Contabili_W.RapCon_Scrivi()"

        Try

            '------------------------------
            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri)
            '------------------------------
            XmlDoc = New XmlDocument
            XmlDoc.LoadXml(DatiRapCon)
            '------------------------------

            xDatiRapCons = XmlDoc.GetElementsByTagName("DatiRapCon")

            i_DatiRapCon = 0

            Do While i_DatiRapCon < xDatiRapCons.Count

                'Prelevo l'i-esimo blocco di DatiRapCons (in realtà ne esiste uno solo)
                xDatiRapCon = xDatiRapCons.Item(i_DatiRapCon)

                '------------------------------

                xRapCons = xDatiRapCon.GetElementsByTagName("RapCon")

                i_RapCon = 0

                Do While i_RapCon < xRapCons.Count

                    'Prelevo l' i-esimo Rapporto Contabile
                    xRapCon = xRapCons.Item(i_RapCon)

                    'Prelevo gli attributi del rapporto selezionato
                    OpeDB_RapCon = xRapCon.GetAttribute("TipoOperazioneDB")

                    'Inizializzo Preventivamente il Cod_Rapporto
                    Cod_Rapporto = CLng(xRapCon.GetAttribute("cod_rapporto"))

                    If Not IsNothing(xRapCon.GetAttribute("agente")) AndAlso
                       xRapCon.GetAttribute("agente") <> "" Then
                        agente = CInt(xRapCon.GetAttribute("agente"))
                    Else
                        agente = 0
                    End If
                    If Not IsNothing(xRapCon.GetAttribute("consulente")) AndAlso
                       xRapCon.GetAttribute("consulente") <> "" Then
                        Consulente = CInt(xRapCon.GetAttribute("consulente"))
                    Else
                        Consulente = 0
                    End If

                    'Verifico l'operazione richiesta
                    Select Case OpeDB_RapCon
                        '
                    Case "0"    'LEGGI -------------------------------------------------------
                            '
                        Case "1"    'SALVA -------------------------------------------------------

                            'Marco Nota: Attenzione. Il nuovo codice rapporto viene creato solo in caso di cod_rapporto = 0.
                            'Infatti potrebbe essere possibile inserire in locale per la versione standalone
                            'anche i codici base (negativi).

                            If Cod_Rapporto = 0 Then

                                'Richiedo un nuovo codice rapporto
                                Cod_Rapporto = ObjSequenze.NuovoId_Tabella("Rapporti_Contabili",
                                                                           CInt(xRapCon.GetAttribute("basecode")),
                                                                           CInt(xRapCon.GetAttribute("topcode")),
                                                                           objParametri)

                                ObjSequenze = Nothing

                            End If

                            Dummy = objRapCon.Scrivi(CInt(xRapCon.GetAttribute("sa_cod")),
                                                     CInt(Cod_Rapporto),
                                                     CStr(xRapCon.GetAttribute("rapporto_des")),
                                                     CInt(xRapCon.GetAttribute("cliente")),
                                                     CInt(xRapCon.GetAttribute("fornitore")),
                                                     CInt(xRapCon.GetAttribute("dipendente")),
                                                     CInt(xRapCon.GetAttribute("terzista")),
                                                     CInt(xRapCon.GetAttribute("legale")),
                                                     Agente,
                                                     Consulente,
                                                     CDate(xRapCon.GetAttribute("validita_inizio")),
                                                     CDate(xRapCon.GetAttribute("validita_fine")),
                                                     objParametri)


                        Case "2"    'MODIFICA -------------------------------------------------------

                            objRapCon.Modifica(CInt(xRapCon.GetAttribute("sa_cod")),
                                               CInt(Cod_Rapporto),
                                               CStr(xRapCon.GetAttribute("rapporto_des")),
                                               CInt(xRapCon.GetAttribute("cliente")),
                                               CInt(xRapCon.GetAttribute("fornitore")),
                                               CInt(xRapCon.GetAttribute("dipendente")),
                                               CInt(xRapCon.GetAttribute("terzista")),
                                               CInt(xRapCon.GetAttribute("legale")),
                                               Agente,
                                               Consulente,
                                               CDate(xRapCon.GetAttribute("validita_inizio")),
                                               CDate(xRapCon.GetAttribute("validita_fine")),
                                               objParametri)


                        Case "3" 'CANCELLAZIONE RAPPORTO CONTABILE

                            'Distruggo le relazioni ContattiXRapporti_Contabili

                            objRisorse_Umane.Cancella(0, "",
                                                      CInt(xRapCon.GetAttribute("cod_rapporto")),
                                                      "", objParametri)

                            objRisorse_Umane = Nothing

                            'Cancellazione del tipo di rapporto contabile
                            objRapCon.Cancella(CInt(xRapCon.GetAttribute("cod_rapporto")),
                                               "", objParametri)

                    End Select

                    'Elimino l'oggetto
                    objRapCon = Nothing


                    '-------------------------------------------------------------

                    'Incremento l'indice
                    i_RapCon += 1

                Loop

                '------------------------------

                'Incremento l'indice
                i_DatiRapCon += 1

            Loop

            '------------------------------

            'Elimino tutti gli oggetti utilizzati

            xRapCon = Nothing
            xRapCons = Nothing
            xDatiRapCon = Nothing
            xDatiRapCons = Nothing
            XmlDoc = Nothing

            '------------------------------

            'Commit della transazione
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)


        Catch ex As Exception

            Dummy = False

            'Faccio il rollback della transazione
            If objParametri.objTransazione IsNot Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            '//////////////////////////////////////////////////////////////////////
            MessaggioErrore = "(Cod_Rapporto=" & Cod_Rapporto & ")" &
                              " : " & ex.Message
            '//////////////////////////////////////////////////////////////////////

            Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)

            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)


        Finally

            ''Chiudo la connessione se è stata aperta in questa routine
            'If (FlagConnessioneLocale) AndAlso (objParametri.objConnessione IsNot Nothing) Then
            '    objParametri.objConnessione.Close()
            'End If
            'Chiudo la connessione se è stata aperta in questa routine
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)

        End Try


        Return Dummy

    End Function

End Class
