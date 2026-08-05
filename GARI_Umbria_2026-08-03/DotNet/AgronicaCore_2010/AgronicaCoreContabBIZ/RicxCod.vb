Imports System.Xml
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate


Public Class RicxCod_W

    Public Function RicxCod_Scrivi(ByVal RicxCod As String,
                                   ByRef objParametri As AgronicaCoreParametri
                                   ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabBIZ.RicxCod_W.RicxCod_Scrivi()"

        Dim dummy As Boolean
        Dim xmlDoc As XmlDocument

        '------------------------------
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim messaggioErrore As String = ""
        Dim xRisp As Boolean = True
        '------------------------------

        Try

            ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, objParametri)

            xmlDoc = New XmlDocument
            xmlDoc.LoadXml(RicxCod)

            Dim objSequenze As New Agro_Sequenze
            Dim objRicxCod As New AgronicaCoreContabDAL.RicxConti_W
            Dim objConti As New AgronicaCoreContabDAL.Conti_W

            'objConti = CreateObject("agro_contab_ad.conti_w")

            Dim xDatiConti As XmlNodeList
            xDatiConti = xmlDoc.GetElementsByTagName("DatiConti")


            Dim i_DatiConti As Integer
            For i_DatiConti = 0 To xDatiConti.Count - 1
                Dim xDatiConto As XmlElement
                xDatiConto = xDatiConti.Item(i_DatiConti)

                Dim xmlConti As XmlNodeList
                xmlConti = xDatiConto.GetElementsByTagName("Conto")

                Dim xmlConto As XmlElement
                Dim I_Conti As Integer
                xmlConto = xmlConti.Item(I_Conti)

                Dim codConto As Integer
                codConto = CLng(xmlConto.GetAttribute("cod_conto"))

                Dim TipoOperazioneDB As Integer
                TipoOperazioneDB = CInt(xmlConto.GetAttribute("TipoOperazioneDB"))

                Select Case TipoOperazioneDB

                    Case 1 'Salva

                        'Inserimento Nuovo Nodo

                        'Mi ricavo il codice per il nuovo conto
                        codConto = objSequenze.NuovoId_Tabella("Conti",
                                                               CLng(xmlConto.GetAttribute("basecode")),
                                                               CLng(xmlConto.GetAttribute("topcode")),
                                                               objParametri)


                        'Creo il Nuovo Conto nella Tabella Conti
                        dummy = objConti.Scrivi(CStr(xmlConto.GetAttribute("piva")),
                                                CLng(codConto),
                                                CStr(xmlConto.GetAttribute("conto_descr")),
                                                CInt(xmlConto.GetAttribute("flag_ue")),
                                                CStr(xmlConto.GetAttribute("cod_contatto")),
                                                CStr(xmlConto.GetAttribute("extra_str")),
                                                CLng(xmlConto.GetAttribute("extra_int")),
                                                CDate(xmlConto.GetAttribute("extra_date")),
                                                AGRODATAINIZIO,
                                                AGRODATAFINE,
                                                "", objParametri)

                    Case 2 'Modifica

                        'Modifico Nodo nella Tabella Conti
                        objConti.Modifica(CLng(xmlConto.GetAttribute("cod_conto")),
                                          CStr(xmlConto.GetAttribute("conto_descr")),
                                          CInt(xmlConto.GetAttribute("flag_ue")),
                                          CStr(xmlConto.GetAttribute("cod_contatto")),
                                          CStr(xmlConto.GetAttribute("extra_str")),
                                          CLng(xmlConto.GetAttribute("extra_int")),
                                          CDate(xmlConto.GetAttribute("extra_date")),
                                          AGRODATAINIZIO,
                                          AGRODATAFINE,
                                          "",
                                          objParametri)

                    Case 3
                        'Eliminazione Tutti Le Relazioni del Conto nella Tabella RicxCod
                        objRicxCod.Cancella(CStr(xmlConto.GetAttribute("piva")),
                                            0,
                                            CLng(xmlConto.GetAttribute("cod_conto")),
                                            0,
                                            "",
                                            objParametri)

                        'Eliminazione Nodo Tabella Conti
                        objConti.Cancella(CStr(xmlConto.GetAttribute("piva")),
                                          CLng(xmlConto.GetAttribute("cod_conto")),
                                          "",
                                          objParametri)

                End Select

                '-------------------------------------------------------------
                ' RICLASSIFICAZIONI X CONTO
                '-------------------------------------------------------------

                Dim xmlRicxCods As XmlNodeList
                xmlRicxCods = xmlConto.GetElementsByTagName("RicXCod")

                Dim i_RicxCod As Integer = 0

                Dim xmlRicxCod As XmlElement
                'Prelevo l'i-esima rubrica
                xmlRicxCod = xmlRicxCods.Item(i_RicxCod)

                'Prelevo gli attributi del codice selezionato
                TipoOperazioneDB = xmlRicxCod.GetAttribute("TipoOperazioneDB")

                'Verifico l'operazione richiesta
                Select Case TipoOperazioneDB

                    Case "0"    'LEGGI -------------------------------------------------------

                    Case "1"    'SALVA -------------------------------------------------------


                        'Creo il Nuovo Conto nella Tabella RicxCod
                        dummy = objRicxCod.Scrivi(CStr(xmlRicxCod.GetAttribute("piva")),
                                                  CLng(xmlRicxCod.GetAttribute("ric_cod")),
                                                  CLng(codConto),
                                                  CLng(xmlRicxCod.GetAttribute("anno")),
                                                  CStr(xmlRicxCod.GetAttribute("id_riclassificazione")),
                                                  CStr(xmlRicxCod.GetAttribute("dare_avere")),
                                                  CDbl(xmlRicxCod.GetAttribute("saldo")),
                                                  CInt(xmlRicxCod.GetAttribute("imputabile")),
                                                  0, 0, 0,
                                                  CDate(xmlRicxCod.GetAttribute("validita_inizio")),
                                                  CDate(xmlRicxCod.GetAttribute("validita_fine")),
                                                  objParametri)


                    Case "2"    'MODIFICA -------------------------------------------------------

                        'Modifica Nodo nella Tabella RicxCod
                        objRicxCod.Modifica_NoSaldo(CStr(xmlRicxCod.GetAttribute("piva")),
                                                    CLng(xmlRicxCod.GetAttribute("ric_cod")),
                                                    CLng(codConto),
                                                    CLng(xmlRicxCod.GetAttribute("anno")),
                                                    CStr(xmlRicxCod.GetAttribute("id_riclassificazione")),
                                                    CStr(xmlRicxCod.GetAttribute("dare_avere")),
                                                    CInt(xmlRicxCod.GetAttribute("imputabile")),
                                                    CDate(xmlRicxCod.GetAttribute("validita_inizio")),
                                                    CDate(xmlRicxCod.GetAttribute("validita_fine")),
                                                    "",
                                                    objParametri)

                        'Controllo se occorre aggiornare le riclassificazioni dei figli
                        If (xmlRicxCod.GetAttribute("old_id_riclassificazione") > 0) Then

                            'objRicxCod.Modifica_Solo_Riclassificazioni(CStr(XmlRicxCod.GetAttribute("piva")),
                            '                                           CLng(XmlRicxCod.GetAttribute("ric_cod")),
                            '                                           CLng(XmlRicxCod.GetAttribute("anno")),
                            '                                           CStr(XmlRicxCod.GetAttribute("old_id_riclassificazione")),
                            '                                           CStr(XmlRicxCod.GetAttribute("id_riclassificazione")),
                            '                                           CStr(objParametri.UtenteUsername),
                            '                                           CDate(XmlRicxCod.GetAttribute("validita_inizio")),
                            '                                           CDate(XmlRicxCod.GetAttribute("validita_fine")),
                            '                                           objParametri)

                        End If


                    Case "3"    'ELIMINA -------------------------------------------------------

                        'Eliminazione Nodo Tabella RicxCod
                        objRicxCod.Cancella(CStr(xmlRicxCod.GetAttribute("piva")),
                                            CLng(xmlRicxCod.GetAttribute("ric_cod")),
                                            CLng(xmlRicxCod.GetAttribute("cod_conto")),
                                            CLng(xmlRicxCod.GetAttribute("anno")),
                                            CStr(objParametri.UtenteUsername),
                                            objParametri)

                End Select

            Next

            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)
            xRisp = True

        Catch ex As Exception

            xRisp = False

            If objParametri.objTransazione IsNot Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            messaggioErrore = ex.Message

            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        Finally
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)
        End Try

        'Restituisco il risultato
        Return xRisp

    End Function

End Class
