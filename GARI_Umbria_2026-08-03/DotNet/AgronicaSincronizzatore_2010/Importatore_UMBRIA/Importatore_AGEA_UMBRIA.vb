Imports System.IO
Imports System.Xml.Serialization
Imports AGEA_Coordinamento
Imports AgroFascicoloBA_BIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports Newtonsoft.Json

Public Class Importatore_AGEA_UMBRIA

    Private ObjParametri_SuperServer As AgronicaCoreParametri
    Private ObjParametri_Server As AgronicaCoreParametri
    Private ObjParametri_Utenti As AgronicaCoreParametri


    Private objLog As AgronicaCoreDataProvider.LogProvider
    Private Configurazione_Servizi_R As AgronicaCoreVarieDAL.Configurazione_Servizi_R

    Private LogFileName As String
    Private LogDirectory As String
    Private LogDescrizioneUtente As String
    Private DirectoryFileImportazioni As String
    Private DirectoryFileEsportazioni As String
    Private ParametriExtra As String

    Private username As String
    Private password As String

    Private linkWS_AGEAFS5 As String
    Private linkWS_AGEAFS6 As String
    Private username_AGEA As String
    Private password_AGEA As String

    Sub New(ByVal _Configurazione_Servizio As AgronicaCoreVarieDAL.Configurazione_Servizio, ByVal _ObjParametri_SuperServer As AgronicaCoreParametri, ByVal _ObjParametri_Server As AgronicaCoreParametri, ByVal _ObjParametri_Utenti As AgronicaCoreParametri)
        Me.ObjParametri_SuperServer = _ObjParametri_SuperServer
        Me.ObjParametri_Server = _ObjParametri_Server
        Me.ObjParametri_Utenti = _ObjParametri_Utenti

        InizializzoOggettiCore()

        ImpostoGliAltriParametri(_Configurazione_Servizio)

        impostaParametriExtra()

    End Sub

    Public Function avviaImportazione() As Boolean
        logga("START Servizio")
        Dim returnBool = True
        'If False Then
        If linkWS_AGEAFS5 <> "" AndAlso username_AGEA <> "" AndAlso password_AGEA <> "" Then
            Try
                logga("Inizio allineamentoDatiAGEAFS5")
                allineamentoDatiAGEAFS5()
            Catch ex As Exception
                logga("errore avviaImportazione -> allineamentoDatiAGEAFS5 " & ex.Message)
            End Try
            Try
                logga("Inizio importazioneAGEAFS5")
                importazioneAGEAFS5()
            Catch ex As Exception
                logga("errore avviaImportazione -> importazioneAGEAFS5 " & ex.Message)
            End Try

        End If


        'If linkWS_AGEAFS6 <> "" And username_AGEA <> "" And password_AGEA <> "" Then
        '    importazioneAGEAFS6()
        'End If

        logga("END Servizio")
        Return returnBool
    End Function

    Public Sub importazioneAGEAFS5()

        Dim import_Agea As New Import_AGEAFS5(linkWS_AGEAFS5, username_AGEA, password_AGEA)


        Dim returnBool = True
        Dim reader As New Fascicolo_R
        Dim writer As New Fascicolo_W
        Dim dt1 = reader.leggiImportaFascicoli(Now.Date, 4, ObjParametri_Server)
        Dim fascicoliGiaInseriti As Integer = 0
        If dt1.Rows.Count > 0 Then
            If Not (IsDBNull(dt1.Rows(0).Item("numeroFascicoliCaricati"))) Then
                fascicoliGiaInseriti = CInt(dt1.Rows(0).Item("numeroFascicoliCaricati"))
            End If
        Else
            writer.resettaCuaaDaImportare(ObjParametri_Server, 0, 4)
        End If

        writer.resettaCuaaDaImportare(ObjParametri_Server, -1, 4)

        Dim i = 0
        Dim listaCuaa As New List(Of String)
        For Each r As DataRow In reader.leggiCuaaImportazioneMassiva(ObjParametri_Server, 0, 0, 4).Rows
            listaCuaa.Add(r.Item("cuaa"))
        Next
        If listaCuaa.Count = 0 Then
            'For Each r As DataRow In reader.leggiCuaaImportazioneMassiva(ObjParametri_Server, 0, -1, 4).Rows
            '    listaCuaa.Add(r.Item("cuaa"))
            'Next
            If listaCuaa.Count = 0 Then
                writer.resettaCuaaDaImportare(ObjParametri_Server, 1, 4)
                For Each r As DataRow In reader.leggiCuaaImportazioneMassiva(ObjParametri_Server, 0, 0, 4).Rows
                    listaCuaa.Add(r.Item("cuaa"))
                Next
            End If
        End If

        Dim xml = ""

        Dim CUAA_Modificati As AGEA_Coordinamento.ISWSToOprResponse = Nothing
        import_Agea.LeggiCUAA_Modificati(2, CUAA_Modificati)

        logga("CUAA Modificati: " & CUAA_Modificati.Items.Count)
        logga("Elenco CUAA Modificati:" & JsonConvert.SerializeObject(CUAA_Modificati))
        logga("lista CUAA: " & listaCuaa.Count)

        Try
            Dim listaCuaaModificati = (From el In CUAA_Modificati.Items Select CStr(el)).ToList
            For Each cuaa In listaCuaaModificati
                listaCuaa.Insert(0, cuaa)
            Next
        Catch ex As Exception
            logga("Errore listaCuaaModificati: " & ex.Message)
        End Try

        logga("lista CUAA Totali: " & listaCuaa.Count)
        Dim index As Integer = 0
        For Each Cuaa As String In listaCuaa
            index += 1
            Dim ErrMsg As String = ""
            Dim FascicoliInseriti As Integer = 0
            Dim errori As Integer = 0
            Try
                Dim fascicolo As New AGEA_Coordinamento.ISWSToOprResponse
                import_Agea.TrovaFascicoloFS50(Cuaa, fascicolo)

                'Dim vincoli As AGEA_Coordinamento.ISWSToOprResponse
                'import_Agea.LeggiVincoliAgronomici(Cuaa, vincoli)

                If fascicolo IsNot Nothing Then

                    If (fascicolo.Items(0).ToString <> "") Then
                        If fascicolo.Items(0).schedaValidazione IsNot Nothing Then

                            'If fascicolo.Items(0).Cuaa <> Cuaa Then
                            '    Dim aaaa = 0
                            'End If

                            If fascicolo.Items(0).Cuaa = Cuaa Then
                                'If fascicolo.Items(0).Cuaa = Cuaa AndAlso Not esisteFascicolo(Cuaa, fascicolo.Items(0).schedaValidazione) Then
                                Dim schedaValidazione = fascicolo.Items(0).schedaValidazione
                                Dim dataValidazione As Date = AGRODATAINIZIO

                                'INIZIO aggiunto x inserimento di provincia, comune e stato
                                Dim Istat_provincia As String
                                Dim Istat_comune As String
                                Dim Stato As String
                                'FINE 

                                If fascicolo IsNot Nothing AndAlso
                                   fascicolo.Items(0).schedaValidazione IsNot Nothing Then

                                    dataValidazione = CDate(Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(fascicolo.Items(0).dataValidazFascicolo))

                                End If

                                'INIZIO aggiunto valorizzo le variabili provincia, comune e stato leggendo dell xml del fascicolo
                                Istat_provincia = fascicolo.Items(0).sedeResidenza.provincia
                                Istat_comune = fascicolo.Items(0).sedeResidenza.comune
                                If fascicolo.Items(0).sedeResidenza.codiceStatoEstero = Nothing Then
                                    Stato = "IT"
                                End If
                                'FINE

                                xml = ""
                                xml = serializza(fascicolo)
                                writer.AggiornaInCacheFascicolo(4, Cuaa, schedaValidazione, dataValidazione, xml, Now.Date, New Date(2100, 12, 31), ObjParametri_Server, "TrovaFascicoloFS5.0", Istat_provincia, Istat_comune, Stato)



                                Dim consistenze As AGEA_Coordinamento.ISWSToOprResponse = Nothing
                                import_Agea.LeggiConsistenzaFS50(Cuaa, consistenze)
                                If consistenze.ISWSResponse.codRet <> "016" Then
                                    xml = ""
                                    xml = serializza(consistenze)
                                    writer.AggiornaInCacheFascicolo(41, Cuaa, schedaValidazione, dataValidazione, xml, Now.Date, New Date(2100, 12, 31), ObjParametri_Server, "LeggiConsistenzaFS5.0")
                                End If



                                Dim macchine As AGEA_Coordinamento.ISWSToOprResponse = Nothing
                                import_Agea.LeggiMacchine(Cuaa, macchine)
                                If macchine.ISWSResponse.codRet <> "016" Then
                                    xml = ""
                                    xml = serializza(macchine)
                                    writer.AggiornaInCacheFascicolo(42, Cuaa, schedaValidazione, dataValidazione, xml, Now.Date, New Date(2100, 12, 31), ObjParametri_Server, "LeggiMacchine")
                                End If


                                Dim soggetti As AGEA_Coordinamento.ISWSToOprResponse = Nothing
                                import_Agea.LeggiSoggetti(Cuaa, dataValidazione, soggetti)
                                If soggetti.ISWSResponse.codRet <> "016" Then
                                    xml = ""
                                    xml = serializza(soggetti)
                                    writer.AggiornaInCacheFascicolo(43, Cuaa, schedaValidazione, dataValidazione, xml, Now.Date, New Date(2100, 12, 31), ObjParametri_Server, "DettaglioSoggettoFS1.0")
                                End If

                                Dim fascicolo20 As AGEA_Coordinamento.ISWSToOprResponse = Nothing
                                import_Agea.TrovaFascicoloFS20(Cuaa, fascicolo20)
                                If fascicolo20.ISWSResponse.codRet <> "016" Then
                                    xml = ""
                                    xml = serializza(fascicolo20)
                                    writer.AggiornaInCacheFascicolo(44, Cuaa, schedaValidazione, dataValidazione, xml, Now.Date, New Date(2100, 12, 31), ObjParametri_Server, "TrovaFascicoloFS2.0")
                                End If


                                Dim consistenzeFS3 As AGEA_Coordinamento.ISWSToOprResponse = Nothing
                                import_Agea.LeggiConsistenzaFS30(Cuaa, consistenzeFS3)
                                If consistenzeFS3.ISWSResponse.codRet <> "016" Then
                                    xml = ""
                                    xml = serializza(consistenzeFS3)
                                    writer.AggiornaInCacheFascicolo(45, Cuaa, schedaValidazione, dataValidazione, xml, Now.Date, New Date(2100, 12, 31), ObjParametri_Server, "LeggiConsistenzaFS3")
                                End If

                                Dim allevamenti As AGEA_Coordinamento.ISWSToOprResponse = Nothing
                                import_Agea.LeggiAllevamenti(Cuaa, allevamenti)
                                If allevamenti.ISWSResponse.codRet <> "016" Then
                                    xml = ""
                                    xml = serializza(allevamenti)
                                    writer.AggiornaInCacheFascicolo(46, Cuaa, schedaValidazione, dataValidazione, xml, Now.Date, New Date(2100, 12, 31), ObjParametri_Server, "LeggiAllevamentiFS5.0")
                                End If


                                'Dim piano As AGEA_Coordinamento.ISWSToOprResponse
                                'import_Agea.LeggiPianoColturale(Cuaa, dataValidazione.Year, piano)

                                'xml = ""
                                'xml = serializza(piano)
                                'writer.AggiornaInCacheFascicolo(47, Cuaa, schedaValidazione, dataValidazione, xml, Now.Date, New Date(2100, 12, 31), ObjParametri_Server, "LeggiPianoColturale")


                                'Dim territorio As AGEA_Coordinamento.ISWSToOprResponse
                                'import_Agea.LeggiTerritorio(Cuaa, territorio)

                                'xml = ""
                                'xml = serializza(territorio)
                                'writer.AggiornaInCacheFascicolo(48, Cuaa, schedaValidazione, dataValidazione, xml, Now.Date, New Date(2100, 12, 31), ObjParametri_Server, "LeggiTerritorio")

                                Try
                                    Dim import_Agea6 As New Import_AGEAFS6(linkWS_AGEAFS6, username_AGEA, password_AGEA)
                                    Dim anagraficaAziendaT As New AGEA_UMBRIA_FS6.ISWSToOprResponse
                                    import_Agea6.AnagraficaAziendaFS6(Cuaa, anagraficaAziendaT)
                                    If anagraficaAziendaT.ISWSResponse.codRet <> "016" AndAlso anagraficaAziendaT.Items.Length > 0 AndAlso anagraficaAziendaT.Items(0).ToString <> "" Then
                                        xml = ""
                                        xml = serializza(anagraficaAziendaT)
                                        writer.AggiornaInCacheFascicolo(47, Cuaa, schedaValidazione, dataValidazione, xml, Now.Date, New Date(2100, 12, 31), ObjParametri_Server, "AnagraficaAziendaFS6")
                                    End If
                                Catch ex As Exception

                                End Try
                                'Dim fascicolo6 As New AGEA_UMBRIA_FS6.ISWSToOprResponse
                                'import_Agea6.TrovaFascicoloFS60(Cuaa, fascicolo6)
                                'If fascicolo6.ISWSResponse.codRet <> "016" AndAlso fascicolo6.Items.Length > 0 AndAlso fascicolo6.Items(0).ToString <> "" Then
                                '    xml = ""
                                '    xml = serializza(fascicolo6)
                                '    writer.AggiornaInCacheFascicolo(48, Cuaa, schedaValidazione, dataValidazione, xml, Now.Date, New Date(2100, 12, 31), ObjParametri_Server, "TrovaFascicoloFS6.0")
                                'End If

                                'Dim consistenzeFS6 As New AGEA_UMBRIA_FS6.ISWSToOprResponse
                                'import_Agea6.LeggiConsistenzaFS6(Cuaa, consistenzeFS6)
                                'If consistenzeFS6.ISWSResponse.codRet <> "016" AndAlso consistenzeFS6.Items.Length > 0 AndAlso consistenzeFS6.Items(0).ToString <> "" Then
                                '    xml = ""
                                '    xml = serializza(consistenzeFS6)
                                '    writer.AggiornaInCacheFascicolo(49, Cuaa, schedaValidazione, dataValidazione, xml, Now.Date, New Date(2100, 12, 31), ObjParametri_Server, "LeggiConsistenzaFS6.0")
                                'End If


                                writer.FascicoloImportato(Cuaa, 1, ObjParametri_Server)
                                FascicoliInseriti += 1
                                logga("Fascicolo Importato: " & Cuaa & " (" & schedaValidazione & " - " & dataValidazione.ToShortDateString & ") [" & index & "]")
                            Else
                                writer.FascicoloImportato(Cuaa, 1, ObjParametri_Server)
                                FascicoliInseriti += 1
                            End If

                        Else

                            writer.FascicoloImportato(Cuaa, 1, ObjParametri_Server)
                            writer.EliminaFascicoloDaCaricare(ObjParametri_Server, Cuaa, 4)
                            FascicoliInseriti += 1

                        End If
                    Else
                        logga("ImportAgrea1 Errore nel Cuaa:" & Cuaa & " - " & ErrMsg & " [" & CStr(i) & "]")
                        writer.FascicoloImportato(Cuaa, -1, ObjParametri_Server)
                        If reader.leggiFascicolo(4, Cuaa, ObjParametri_Server) = "" Then
                            writer.EliminaFascicoloDaCaricare(ObjParametri_Server, Cuaa, 4)
                        End If
                    End If



                Else
                    errori += 1
                    logga("ImportAgrea1 Errore nel Cuaa:" & Cuaa & " - " & ErrMsg & " [" & CStr(i) & "]")
                    writer.FascicoloImportato(Cuaa, -1, ObjParametri_Server)
                    If reader.leggiFascicolo(4, Cuaa, ObjParametri_Server) = "" Then
                        writer.EliminaFascicoloDaCaricare(ObjParametri_Server, Cuaa, 4)
                    End If

                    returnBool = False

                End If

            Catch ex As Exception
                errori += 1
                logga("Import_AGEA_UMBRIA Errore nel Cuaa:" & CStr(Cuaa) & " - " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, "True") & " [" & CStr(i) & "]")
                returnBool = False
                writer.FascicoloImportato(Cuaa, -1, ObjParametri_Server)
                If reader.leggiFascicolo(4, Cuaa, ObjParametri_Server) = "" Then
                    writer.EliminaFascicoloDaCaricare(ObjParametri_Server, Cuaa, 4)
                End If
            End Try
            Dim dt = reader.leggiImportaFascicoli(Now.Date, 4, ObjParametri_Server)
            If dt.Rows.Count > 0 Then
                If Not (IsDBNull(dt.Rows(0).Item("numeroFascicoliCaricati"))) Then
                    FascicoliInseriti = FascicoliInseriti + CInt(dt.Rows(0).Item("numeroFascicoliCaricati"))
                End If
                If Not (IsDBNull(dt.Rows(0).Item("numeroErrori"))) Then
                    errori = errori + CInt(dt.Rows(0).Item("numeroErrori"))
                End If
            End If
            writer.ScriviImportaFascicoli(Now.Date, 4, FascicoliInseriti, errori, ObjParametri_Server)
            i += 1
        Next

    End Sub

    Public Function importazioneAGEAFS6() As Boolean

        Dim import_Agea As New Import_AGEAFS6(linkWS_AGEAFS6, username_AGEA, password_AGEA)

        Dim returnBool = True
        Dim reader As New Fascicolo_R
        Dim writer As New Fascicolo_W
        Dim dt1 = reader.leggiImportaFascicoli(Now.Date, 4, ObjParametri_Server)
        Dim fascicoliGiaInseriti As Integer = 0
        If dt1.Rows.Count > 0 Then
            If Not (IsDBNull(dt1.Rows(0).Item("numeroFascicoliCaricati"))) Then
                fascicoliGiaInseriti = CInt(dt1.Rows(0).Item("numeroFascicoliCaricati"))
            End If
        Else
            writer.resettaCuaaDaImportare(ObjParametri_Server, 0, 4)
        End If

        writer.resettaCuaaDaImportare(ObjParametri_Server, -1, 4)

        Dim i = 0
        Dim listaCuaa As New List(Of String)
        For Each r As DataRow In reader.leggiCuaaImportazioneMassiva(ObjParametri_Server, 0, 0, 4).Rows
            listaCuaa.Add(r.Item("cuaa"))
        Next
        If listaCuaa.Count = 0 Then
            'For Each r As DataRow In reader.leggiCuaaImportazioneMassiva(ObjParametri_Server, 0, -1, 4).Rows
            '    listaCuaa.Add(r.Item("cuaa"))
            'Next
            If listaCuaa.Count = 0 Then
                writer.resettaCuaaDaImportare(ObjParametri_Server, 1, 4)
                For Each r As DataRow In reader.leggiCuaaImportazioneMassiva(ObjParametri_Server, 0, 0, 4).Rows
                    listaCuaa.Add(r.Item("cuaa"))
                Next
            End If
        End If

        Dim CUAA_Modificati As AGEA_UMBRIA_FS6.ISWSToOprResponse = Nothing
        import_Agea.LeggiCUAA_Modificati(20, CUAA_Modificati)

        listaCuaa = (From el In CUAA_Modificati.Items Select CStr(el)).ToList

        Dim listaAnni As New List(Of Integer)
        listaAnni.Add(Date.Now.Year)
        listaAnni.Add(Date.Now.Year - 1)
        'listaAnni.Add(Date.Now.Year - 2)

        For Each Cuaa As String In listaCuaa
            Dim FascicoliInseriti As Integer = 0
            Dim errori As Integer = 0
            Dim anagraficaAziendaT As New AGEA_UMBRIA_FS6.ISWSToOprResponse
            Dim anagraficaAzienda As AGEA_UMBRIA_FS6.ISWSRespAnagFascicolo15
            import_Agea.AnagraficaAziendaFS6(Cuaa, anagraficaAziendaT)
            If anagraficaAziendaT.Items.Length > 0 AndAlso anagraficaAziendaT.Items(0).ToString <> "" Then
                anagraficaAzienda = CType(anagraficaAziendaT.Items(0), AGEA_UMBRIA_FS6.ISWSRespAnagFascicolo15)
            End If

            Try
                For Each anno In listaAnni
                    Dim schede As New AGEA_UMBRIA_FS6.ISWSToOprResponse
                    import_Agea.LeggiSchedeCuaa(Cuaa, anno, schede)
                    If schede.Items.Length = 1 AndAlso schede.Items(0).ToString = "" Then
                        Continue For
                    End If
                    Dim respSchede As New List(Of AGEA_UMBRIA_FS6.ISWSRespSchede)
                    For Each el In schede.Items
                        respSchede.Add(el)
                    Next

                    For Each scheda In respSchede
                        Dim schedaCompletaT As AGEA_UMBRIA_FS6.ISWSToOprResponse = Nothing
                        import_Agea.LeggiSchedaValidazione(Cuaa, scheda.IDScheda, schedaCompletaT)
                        If schedaCompletaT.Items.Length = 1 AndAlso schedaCompletaT.Items(0).ToString <> "" Then
                            Dim ISWSScheda = CType(schedaCompletaT.Items(0), AGEA_UMBRIA_FS6.ISWSScheda)
                            If ISWSScheda.Riepilogo IsNot Nothing OrElse ISWSScheda.PianoColtivazione IsNot Nothing OrElse ISWSScheda.ComposizioneTerritorio IsNot Nothing Then
                                Dim gino = 0
                            End If
                        End If


                    Next

                Next

            Catch ex As Exception
                errori += 1
                logga("Import_AGEA_UMBRIA Errore nel Cuaa:" & CStr(Cuaa) & " - " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, "True") & " [" & CStr(i) & "]")
                returnBool = False
                writer.FascicoloImportato(Cuaa, -1, ObjParametri_Server)
                If reader.leggiFascicolo(4, Cuaa, ObjParametri_Server) = "" Then
                    writer.EliminaFascicoloDaCaricare(ObjParametri_Server, Cuaa, 4)
                End If
            End Try
            Dim dt = reader.leggiImportaFascicoli(Now.Date, 4, ObjParametri_Server)
            If dt.Rows.Count > 0 Then
                If Not (IsDBNull(dt.Rows(0).Item("numeroFascicoliCaricati"))) Then
                    FascicoliInseriti = FascicoliInseriti + CInt(dt.Rows(0).Item("numeroFascicoliCaricati"))
                End If
                If Not (IsDBNull(dt.Rows(0).Item("numeroErrori"))) Then
                    errori = errori + CInt(dt.Rows(0).Item("numeroErrori"))
                End If
            End If
            writer.ScriviImportaFascicoli(Now.Date, 4, FascicoliInseriti, errori, ObjParametri_Server)
            i += 1
        Next
        Return returnBool

    End Function

    Private Function getDataModifica(ByRef enteValidatore_cod As Integer) As DateTime?
        Dim reader As New Fascicolo_R
        Dim dataModifica As Date
        Dim dt As DataTable = reader.LeggiAggiornaFascicoli(ObjParametri_Server, enteValidatore_cod)
        If dt.Rows.Count > 0 Then
            Dim dt1 As DataTable = reader.LeggiAggiornaFascicoli(ObjParametri_Server, enteValidatore_cod, , , , 0)
            If dt1.Rows.Count = 0 Then
                dataModifica = CDate(reader.maxDataAggiornaFascicoli(ObjParametri_Server, enteValidatore_cod).Rows(0)(0))
                If dataModifica = Now.Date Then
                    Return Nothing
                End If
            Else
                Return Nothing
            End If
        Else
            dataModifica = New Date(2016, 12, 1)
        End If
        Return dataModifica
    End Function

    Private Function esisteFascicolo(cuaa As String, numero_validazione As String) As Boolean

        Dim reader As New Fascicolo_R

        If reader.legggiAziendaFascicolo(4, cuaa, numero_validazione, ObjParametri_Server) IsNot Nothing Then
            Return True
        Else
            Return False
        End If

    End Function

    Private Function serializza(oggetto As Object) As String

        Try
            Dim x As New XmlSerializer(oggetto.GetType)

            Dim return_str As String = ""

            Dim sw As New IO.StringWriter()
            x.Serialize(sw, oggetto)
            return_str = sw.ToString()

            Return return_str
        Catch ex As Exception

        End Try

        Return ""

    End Function

    'INIZIO
    'Funzione per inserire la provincia, il comune e lo stato  nelle importazioni dei fascicoli  GLORIA
    Public Sub allineamentoDatiAGEAFS5()

        Dim import_Agea As New Import_AGEAFS5(linkWS_AGEAFS5, username_AGEA, password_AGEA)

        Dim reader As New Fascicolo_R
        Dim writer As New Fascicolo_W
        'Dim dt1 = reader.leggiImportaFascicoli(Now.Date, 4, ObjParametri_Server)
        Dim dt1 = reader.leggiFascicoliDaAggiornareCampiProvicia(4, ObjParametri_Server)

        Dim provincia As String = ""
        Dim comune As String = ""
        Dim stato As String = ""
        Dim dataValidazione As Date = AGRODATAINIZIO
        Dim fascicolo As New AGEA_Coordinamento.ISWSToOprResponse

        If dt1.Rows.Count > 0 Then
            For Each r As DataRow In dt1.Rows
                Try
                    Dim textReader As TextReader = New StringReader(r.Item("Fascicolo").ToString())
                    Dim serializer As New XmlSerializer(GetType(AGEA_Coordinamento.ISWSToOprResponse))
                    fascicolo = serializer.Deserialize(textReader)
                    provincia = fascicolo.Items(0).sedeResidenza.provincia
                    comune = fascicolo.Items(0).sedeResidenza.comune
                    If fascicolo.Items(0).sedeResidenza.codiceStatoEstero = Nothing Then
                        stato = "IT"
                    End If

                    dataValidazione = CDate(r.Item("Validazione_Data").ToString())
                    writer.ModificaInCacheFascicoloAllineamento(4, r.Item("CUAA").ToString(), r.Item("Validazione_Numero").ToString(), dataValidazione, provincia, comune, stato, ObjParametri_Server)
                Catch ex As Exception
                    logga("Errore allineamentoDatiAGEAFS5 " & r.Item("CUAA") & ": " & ex.Message)
                End Try
            Next
        End If

    End Sub
    'Fine


#Region "Util"

    Private Sub logga(ByVal msg As String)
        Dim customLOGParams As New CustomLOGParams With {
            .LogDescrizioneUtente = LogDescrizioneUtente,
            .LogDirectory = LogDirectory,
            .LogFileName = LogFileName
        }

        objLog.Scrivi_LOG(ObjParametri_Server, "", msg, CustomLOGParams:=customLOGParams)
    End Sub

    Private Sub InizializzoOggettiCore()

        objLog = New AgronicaCoreDataProvider.LogProvider

    End Sub

    Private Sub ImpostoGliAltriParametri(ByVal _Configurazione_Servizio As AgronicaCoreVarieDAL.Configurazione_Servizio)

        LogFileName = _Configurazione_Servizio.Tipo_Sincro.ToString & "_log.txt"
        LogDirectory = _Configurazione_Servizio.DirectoryLOG
        LogDescrizioneUtente = _Configurazione_Servizio.Tipo_Sincro.ToString
        DirectoryFileImportazioni = _Configurazione_Servizio.DirectoryFileImportazioni
        DirectoryFileEsportazioni = _Configurazione_Servizio.DirectoryFileEsportazioni
        ParametriExtra = _Configurazione_Servizio.Parametri_Extra

        username = _Configurazione_Servizio.Username
        password = _Configurazione_Servizio.Password

    End Sub

    Private Sub impostaParametriExtra()
        Dim listaPar As List(Of String) = ParametriExtra.Split("|").ToList
        Dim ht As New Hashtable
        For Each par In listaPar
            Dim key = par.Split("=")(0)
            Dim value = par.Split("=")(1)
            ht.Add(key, value)
        Next
        'WS
        linkWS_AGEAFS5 = ht.Item("linkWS_AGEAFS5")

        If ht.ContainsKey("linkWS_AGEAFS6") Then
            linkWS_AGEAFS6 = ht.Item("linkWS_AGEAFS6")
        End If

        username_AGEA = ht.Item("username_AGEA")
        password_AGEA = ht.Item("password_AGEA")

    End Sub

#End Region

End Class
