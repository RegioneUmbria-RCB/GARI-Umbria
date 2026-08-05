Imports System.Data
Imports System.Data.Common
Imports System.Xml
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class Grafica_Write
    'Inherits AgronicaCoreDataProvider.LogProvider
    Inherits AgronicaCoreDataProvider.DataProvider






    Public Function Grafica_Scrivi2005( _
                     ByVal Dati As String, _
                     ByVal Piva As String, _
                     ByVal Sa_cod As Long, _
                     ByVal Codice As Long, _
                      ByVal ConvertiInEsadecimale As Boolean, _
                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                        ) As Long
        '============================================================================


        Dim InMts As Boolean
        Dim XmlDoc As XmlDocument

        'Dim objGrafica                As Agro_Grafica_AD.Grafica_Write
        Dim objGrafica As New AgronicaCoreGraficaDAL.Grafica_Write

        Dim Dummy As Long

        Dim xDati As Xml.XmlNodeList
        Dim xDato As Xml.XmlElement

        Dim OpeDB_Dato As Long

        Dim i_Dati As Integer

        Dim GraphicKey As String

        Dim Fuso As Integer

        Dim Proiezione As String

        Dim Delta_Nord As Decimal

        Dim Delta_Est As Decimal

        Dim Quota As Decimal

        Dim Pdop As Decimal

        Dim Lat As Decimal

        Dim Lon As Decimal


        '------------------------------

        Dim NomeRoutine As String = "AgronicaCoreGraficaBIZ.Grafica_W.Grafica_Scrivi2005()"

        '------------------------------

        Dim MessaggioErrore As String = ""


        Dim FlagConnessioneLocale As Boolean = False
        Dim FlagTransazioneLocale As Boolean = False


        Try

            '------------------------------

            'Se la connessione è chiusa la apro
            If objParametri.objConnessione Is Nothing Then
                objParametri.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
                objParametri.objConnessione.Open()
                FlagConnessioneLocale = True
            End If

            If objParametri.objTransazione Is Nothing Then
                objParametri.objTransazione = objParametri.objConnessione.BeginTransaction
                FlagTransazioneLocale = True
            End If




            '------------------------------
            XmlDoc = New Xml.XmlDocument
            XmlDoc.LoadXml(Dati)

            '------------------------------



            xDati = XmlDoc.GetElementsByTagName("Entita")


            i_Dati = 0

            'objGrafica = CreateObject("Agro_Grafica_AD.Grafica_Write")

            Do While i_Dati < xDati.Count

                'Prelevo l'i-esimo blocco di DatiAppezzamenti (in realta' ne esiste uno solo)
                xDato = xDati.Item(i_Dati)


                'Prelevo gli attributi dell'appezzamento selezionato
                OpeDB_Dato = xDato.GetAttribute("TipoOperazioneDB")

                If Not IsNothing(xDato.GetAttribute("pdop")) Then
                    Pdop = Val(UtilityProvider.Agro_SQL_SaveNum(xDato.GetAttribute("pdop")))
                Else
                    Pdop = 0
                End If

                If Not IsNothing(xDato.GetAttribute("delta_nord")) Then
                    Delta_Nord = Val(UtilityProvider.Agro_SQL_SaveNum(xDato.GetAttribute("delta_nord")))
                Else
                    Delta_Nord = 0
                End If

                If Not IsNothing(xDato.GetAttribute("delta_est")) Then
                    Delta_Est = Val(UtilityProvider.Agro_SQL_SaveNum(xDato.GetAttribute("delta_est")))
                Else
                    Delta_Est = 0
                End If

                If Not IsNothing(xDato.GetAttribute("quota")) Then
                    Quota = Val(UtilityProvider.Agro_SQL_SaveNum(xDato.GetAttribute("quota")))
                Else
                    Quota = 0
                End If

                If Not IsNothing(xDato.GetAttribute("fuso")) Then
                    Fuso = CInt(UtilityProvider.Agro_SQL_SaveNum(xDato.GetAttribute("fuso")))
                Else
                    Fuso = 32
                End If

                If Not IsNothing(xDato.GetAttribute("proiezione")) Then
                    Proiezione = CStr(xDato.GetAttribute("proiezione"))
                Else
                    Proiezione = "E"
                End If

                If Not IsNothing(xDato.GetAttribute("lat")) Then
                    Lat = Val(UtilityProvider.Agro_SQL_SaveNum(xDato.GetAttribute("lat")))
                Else
                    Lat = 0
                End If

                If Not IsNothing(xDato.GetAttribute("lon")) Then
                    Lon = Val(UtilityProvider.Agro_SQL_SaveNum(xDato.GetAttribute("lon")))
                Else
                    Lon = 0
                End If

                'Verifico l'operazione richiesta
                Select Case OpeDB_Dato
                    '
                    Case "0"    'LEGGI -------------------------------------------------------
                        '
                    Case "1"    'SALVA -------------------------------------------------------
                        '
                        If Codice <= 0 Then

                            Dim objAgroSeq As New AgronicaCoreDataProvider.Agro_Sequenze
                            Codice = objAgroSeq.NuovoId_Grafica(Piva, _
                                                    Sa_cod, _
                                                    CStr(xDato.GetAttribute("layer")), _
                                                    CLng(xDato.GetAttribute("basecode")), _
                                                    CLng(xDato.GetAttribute("topcode")), _
                                                    objParametri)

                        End If

                        GraphicKey = Left(CStr(xDato.GetAttribute("id")), 1) & Right(New String("0", 8) & Hex(Codice), 8)




                        Dummy = objGrafica.Scrivi2005(
                                                   CStr(Piva), CLng(Sa_cod), CStr(xDato.GetAttribute("section")), CStr(GraphicKey),
                                                   CStr(xDato.GetAttribute("descr")), Val(UtilityProvider.Agro_SQL_SaveNum(xDato.GetAttribute("ecolor"))), CStr(xDato.GetAttribute("layer")),
                                                   Val(UtilityProvider.Agro_SQL_SaveNum(xDato.GetAttribute("eline"))), Val(UtilityProvider.Agro_SQL_SaveNum(xDato.GetAttribute("vx1"))), Val(UtilityProvider.Agro_SQL_SaveNum(xDato.GetAttribute("vy1"))),
                                                   Val(UtilityProvider.Agro_SQL_SaveNum(xDato.GetAttribute("vx2"))), Val(UtilityProvider.Agro_SQL_SaveNum(xDato.GetAttribute("vy2"))), Val(UtilityProvider.Agro_SQL_SaveNum(xDato.GetAttribute("rad"))),
                                                   CStr(xDato.GetAttribute("text")), Val(UtilityProvider.Agro_SQL_SaveNum(xDato.GetAttribute("gps"))), CDbl(Lat), CDbl(Lon),
                                                   CDbl(Pdop),
                                                   CInt(Fuso),
                                                   CStr(Proiezione),
                                                   CDbl(Delta_Nord),
                                                   CDbl(Delta_Est),
                                                   CDbl(Quota),
                                                   ConvertiInEsadecimale,
                                                    objParametri)



                        '
                    Case "2"    'MODIFICA -------------------------------------------------------
                        '
                        '                     objGrafica.Modifica _
                        '                                                Piva, Sa_Cod, CStr(xDato.getAttribute("section")), CStr(xDato.getAttribute("id")), _
                        '                                                CStr(xDato.getAttribute("descr")), Val(xDato.getAttribute("ecolor")), CStr(xDato.getAttribute("layer")), _
                        '                                                Val(CStr(xDato.getAttribute("eline"))), Val(CStr(xDato.getAttribute("vx1"))), Val(CStr(xDato.getAttribute("vy1"))), _
                        '                                                Val(CStr(xDato.getAttribute("vx2"))), Val(CStr(xDato.getAttribute("vy2"))), Val(CStr(xDato.getAttribute("rad"))), _
                        '                                                CStr(xDato.getAttribute("text")), Val(Agro_SQL_SaveNum(xDato.getAttribute("gps"))), Val(Agro_SQL_SaveNum(xDato.getAttribute("lat"))), Val(Agro_SQL_SaveNum(xDato.getAttribute("lon"))), Val(Agro_SQL_SaveNum(xDato.getAttribute("pdop"))), Utente, CDate(xDato.getAttribute("validita_inizio")), CDate(xDato.getAttribute("validita_fine")), objCnManager, ConnessioneAlternativa

                        objGrafica.Modifica2005(
                                                  Piva, Sa_cod,
                                                  CStr(xDato.GetAttribute("section")),
                                                  CStr(xDato.GetAttribute("id")),
                                                  CStr(xDato.GetAttribute("descr")),
                                                  Val(xDato.GetAttribute("ecolor")),
                                                  CStr(xDato.GetAttribute("layer")),
                                                  Val(CStr(xDato.GetAttribute("eline"))),
                                                  CDbl(Agro_XML_GetDecimal(xDato, "vx1", -1)),
                                                  CDbl(Agro_XML_GetDecimal(xDato, "vy1", -1)),
                                                  CDbl(Agro_XML_GetDecimal(xDato, "vx2", -1)),
                                                  CDbl(Agro_XML_GetDecimal(xDato, "vy2", -1)),
                                                  Val(CStr(xDato.GetAttribute("rad"))),
                                                  CStr(xDato.GetAttribute("text")),
                                                  Val(UtilityProvider.Agro_SQL_SaveNum(xDato.GetAttribute("gps"))),
                                                  CDbl(Lat), CDbl(Lon),
                                                  CDbl(Pdop),
                                                  CInt(Fuso),
                                                  CStr(Proiezione),
                                                  CDbl(Delta_Nord),
                                                  CDbl(Delta_Est),
                                                  CDbl(Quota),
                                                  "",
                                                  objParametri)


                    Case "3"    'CANCELLA -------------------------------------------------------
                        '
                        objGrafica.Cancella(Piva, _
                                            Sa_cod, _
                                            CStr(xDato.GetAttribute("section")), _
                                            CStr(xDato.GetAttribute("id")), _
                                            CStr(xDato.GetAttribute("descr")), _
                                                "", _
                                                objParametri)


                End Select

                'Incremento l'indice
                i_Dati = i_Dati + 1

            Loop

            '------------------------------

            'Elimino tutti gli oggetti Xml utilizzati

            xDati = Nothing
            objGrafica = Nothing
            XmlDoc = Nothing

            '------------------------------



            If FlagConnessioneLocale = True Then
                objParametri.objConnessione.Close()
                objParametri.objConnessione.Dispose()
            End If


            If FlagTransazioneLocale = True Then
                objParametri.objTransazione.Commit()
            End If


        Catch ex As Exception

            'Restituisco un valore Dummy
            Codice = -1

            'Faccio il rollback della transazione
            If Not objParametri.objTransazione Is Nothing Then
                objParametri.objTransazione.Rollback()
                objParametri.objTransazione = Nothing
            End If

            '//////////////////////////////////////////////////////////////////////
            MessaggioErrore = ex.Message
            '//////////////////////////////////////////////////////////////////////


            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Codice = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally

            'Chiudo la connessione se è stata aperta in questa routine
            If (FlagConnessioneLocale = True) AndAlso (Not objParametri.objConnessione Is Nothing) Then
                objParametri.objConnessione.Close()
            End If

        End Try

        Return Codice

    End Function

    Public Function Grafica_Scrivi( _
                     ByVal Dati As String, _
                     ByVal Piva As String, _
                     ByVal Sa_cod As Int32, _
                     ByVal Codice As Int32, _
                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                     Optional ByVal usaCodice As Boolean = True) As Int32

        Dim NomeRoutine As String = "GraficaBIZ.Grafica_write.Grafica_Scrivi()"
        Dim MessaggioErrore As String = ""
        Dim FlagConnessioneLocale As Boolean = False
        Dim xConnessione As DbConnection
        Dim xConnectionState As ConnectionState = ConnectionState.Closed

        Dim xTransazione As DbTransaction
        Dim RisultatoFunzione As String = String.Empty
        'Const MetodoNome = "Grafica_Scrivi:"

        Dim OpeDB_Dato As Int32
        Dim i_Dati As Int32
        Dim GraphicKey As String

        Dim Dummy As Int32

        Try

            '------------------------------
            'Verifico se e' stata impostata una connessione
            If IsNothing(objParametri.objConnessione) Then
                'Flag
                FlagConnessioneLocale = True
                'Creo la connessione localmente
                xConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
                xTransazione = Nothing
            Else
                'Utilizzo quella passata come parametro
                xConnessione = objParametri.objConnessione
                xConnectionState = objParametri.objConnessione.State
                xTransazione = objParametri.objTransazione
            End If

            '------------------------------


            Dim XmlDoc As New XmlDocument
            Dim xDato As XmlElement
            Dim xDati As XmlNodeList

            XmlDoc.LoadXml(Dati)
            '------------------------------

            xDati = XmlDoc.GetElementsByTagName("Entita")

            i_Dati = 0

            'objGrafica = CreateObject("Agro_Grafica_AD.Grafica_Write")
            Dim objGrafica As New AgronicaCoreGraficaDAL.Grafica_Write

            Do While i_Dati < xDati.Count

                'Prelevo l'i-esimo blocco di DatiAppezzamenti (in realta' ne esiste uno solo)
                xDato = xDati.Item(i_Dati)

                'Prelevo gli attributi dell'appezzamento selezionato
                OpeDB_Dato = xDato.GetAttribute("TipoOperazioneDB")

                'Verifico l'operazione richiesta
                Select Case OpeDB_Dato
                    '
                    Case "0"    'LEGGI -------------------------------------------------------
                        '
                    Case "1"    'SALVA -------------------------------------------------------
                        '
                        If Codice <= 0 Then

                            Dim objSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
                            Codice = objSequenze.NuovoId_Grafica(Piva, _
                                                                 Sa_cod, _
                                                                CStr(xDato.GetAttribute("layer")), _
                                                                CLng(xDato.GetAttribute("basecode")), _
                                                                CLng(xDato.GetAttribute("topcode")), _
                                                                 objParametri)

                        End If

                        If usaCodice Then
                            GraphicKey = Left(CStr(xDato.GetAttribute("id")), 1) & Right("00000000" & Hex(Codice), 8)
                        Else
                            GraphicKey = xDato.GetAttribute("id")

                        End If

                        Dim Dato_data_creazione As Date = #2/1/1900#
                        Dim Dato_data_Modifica As Date = #2/1/1900#
                        Dim Dato_username_creazione As String = ""
                        Dim Dato_username_modifica As String = ""

                        If Not IsNothing(xDato.GetAttribute("data_creazione")) AndAlso _
                         xDato.GetAttribute("data_creazione") <> "" Then
                            Dato_data_creazione = CDate(xDato.GetAttribute("data_creazione"))
                        End If

                        If Not IsNothing(xDato.GetAttribute("data_modifica")) AndAlso _
                         xDato.GetAttribute("data_modifica") <> "" Then
                            Dato_data_Modifica = CDate(xDato.GetAttribute("data_modifica"))
                        End If

                        If Not IsNothing(xDato.GetAttribute("username_creazione")) Then
                            Dato_username_creazione = CStr(xDato.GetAttribute("username_creazione"))
                        End If

                        If Not IsNothing(xDato.GetAttribute("username_modifica")) Then
                            Dato_username_modifica = CStr(xDato.GetAttribute("username_modifica"))
                        End If

                        Dummy = objGrafica.Scrivi(
                                                   CStr(Piva), CLng(Sa_cod), CStr(xDato.GetAttribute("section")), CStr(GraphicKey),
                                                   CStr(xDato.GetAttribute("descr")), Val(UtilityProvider.Agro_SQL_SaveNum(xDato.GetAttribute("ecolor"))), CStr(xDato.GetAttribute("layer")),
                                                   Val(UtilityProvider.Agro_SQL_SaveNum(xDato.GetAttribute("eline"))), Val(UtilityProvider.Agro_SQL_SaveNum(xDato.GetAttribute("vx1"))), Val(UtilityProvider.Agro_SQL_SaveNum(xDato.GetAttribute("vy1"))),
                                                   Val(UtilityProvider.Agro_SQL_SaveNum(xDato.GetAttribute("vx2"))), Val(UtilityProvider.Agro_SQL_SaveNum(xDato.GetAttribute("vy2"))), Val(UtilityProvider.Agro_SQL_SaveNum(xDato.GetAttribute("rad"))),
                                                   CStr(xDato.GetAttribute("text")), Val(UtilityProvider.Agro_SQL_SaveNum(xDato.GetAttribute("gps"))), Val(UtilityProvider.Agro_SQL_SaveNum(xDato.GetAttribute("lat"))),
                                                   Val(UtilityProvider.Agro_SQL_SaveNum(xDato.GetAttribute("lon"))), Val(UtilityProvider.Agro_SQL_SaveNum(xDato.GetAttribute("pdop"))),
                                                   CDate(xDato.GetAttribute("validita_inizio")),
                                                   CDate(xDato.GetAttribute("validita_fine")),
                                                   objParametri,
                                                    Dato_data_creazione,
                                                    Dato_data_Modifica,
                                                    Dato_username_creazione,
                                                    Dato_username_modifica
                                                   )

                    Case "2"    'MODIFICA -------------------------------------------------------
                        '
                        objGrafica.Modifica(
                                                   Piva, Sa_cod, CStr(xDato.GetAttribute("section")), CStr(xDato.GetAttribute("id")),
                                                   CStr(xDato.GetAttribute("descr")), Val(xDato.GetAttribute("ecolor")), CStr(xDato.GetAttribute("layer")),
                                                   Val(CStr(xDato.GetAttribute("eline"))), Val(CStr(xDato.GetAttribute("vx1"))), Val(CStr(xDato.GetAttribute("vy1"))),
                                                   Val(CStr(xDato.GetAttribute("vx2"))), Val(CStr(xDato.GetAttribute("vy2"))), Val(CStr(xDato.GetAttribute("rad"))),
                                                   CStr(xDato.GetAttribute("text")), Val(UtilityProvider.Agro_SQL_SaveNum(xDato.GetAttribute("gps"))), Val(UtilityProvider.Agro_SQL_SaveNum(xDato.GetAttribute("lat"))),
                                                   Val(UtilityProvider.Agro_SQL_SaveNum(xDato.GetAttribute("lon"))), Val(UtilityProvider.Agro_SQL_SaveNum(xDato.GetAttribute("pdop"))),
                                                   CDate(xDato.GetAttribute("validita_inizio")),
                                                   CDate(xDato.GetAttribute("validita_fine")),
                                                   "", objParametri)


                    Case "3"    'CANCELLA -------------------------------------------------------
                        '
                        objGrafica.Cancella( _
                                                   Piva, Sa_cod, CStr(xDato.GetAttribute("section")), CStr(xDato.GetAttribute("id")), CStr(xDato.GetAttribute("descr")), _
                                                   "", objParametri)



                End Select

                'Incremento l'indice
                i_Dati = i_Dati + 1

            Loop

            '------------------------------

            'Elimino tutti gli oggetti Xml utilizzati

            xDati = Nothing
            objGrafica = Nothing
            XmlDoc = Nothing


        Catch ex As Exception

            RisultatoFunzione = ""
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)


        Finally

            If FlagConnessioneLocale = True Then
                If Not IsNothing(xConnessione) Then
                    xConnessione.Close()
                    xConnessione.Dispose()
                End If
            Else
                If xConnectionState = ConnectionState.Closed Then
                    xConnessione.Close()
                End If
            End If

        End Try


    End Function




End Class
