Imports AgronicaCoreDataProvider
Imports AgronicaCoreModelsSTD.exceptions

Public Class SincroBDNAzienda
    Dim objParametriServer As AgronicaCoreParametri
    Dim objParametriUtenti As AgronicaCoreParametri

    Dim wsRegistroStalla As ChiamawsRegistroStallaQry
    Dim wsAnagraficaCapo As ChiamawsAnagraficaCapoQry
    Dim wsAziende As ChiamawsAziendeQry
    Dim wsCodici As ChiamawsCodiciQry
    Dim wsIdentificativi As ChiamawsIdentificativiGet
    Dim wsStrutture As ChiamawsStruttureQry
    Dim wsTerritorio As ChiamawsTerritorioQry
    Dim wsGestioneAssConsorzi As ChiamawsGestioneAssConsorzi
    Dim wsInterrogazioniModello4 As wsInterrogazioneModello4
    Dim sincronizzatoreAllevamento As SincroBDNAllevamento
    Public Sub New(objParametriServer As AgronicaCoreParametri, objParametriUtenti As AgronicaCoreParametri, link As String, username As String, password As String, token As String, ruolo As String, valore_ruolo_codice As String)
        Me.objParametriServer = objParametriServer
        Me.objParametriUtenti = objParametriUtenti

        Me.wsRegistroStalla = New ChiamawsRegistroStallaQry(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Me.wsAnagraficaCapo = New ChiamawsAnagraficaCapoQry(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Me.wsAziende = New ChiamawsAziendeQry(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Me.wsCodici = New ChiamawsCodiciQry(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Me.wsIdentificativi = New ChiamawsIdentificativiGet(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Me.wsStrutture = New ChiamawsStruttureQry(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Me.wsTerritorio = New ChiamawsTerritorioQry(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Me.wsGestioneAssConsorzi = New ChiamawsGestioneAssConsorzi(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Me.wsInterrogazioniModello4 = New wsInterrogazioneModello4(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Me.sincronizzatoreAllevamento = New SincroBDNAllevamento(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
    End Sub

    Public Sub New(objParametriServer As AgronicaCoreParametri, objParametriUtenti As AgronicaCoreParametri, token As String, ruolo As String, valore_ruolo_codice As String)
        Me.objParametriServer = objParametriServer
        Me.objParametriUtenti = objParametriUtenti

        Me.wsRegistroStalla = New ChiamawsRegistroStallaQry(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Me.wsAnagraficaCapo = New ChiamawsAnagraficaCapoQry(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Me.wsAziende = New ChiamawsAziendeQry(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Me.wsCodici = New ChiamawsCodiciQry(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Me.wsIdentificativi = New ChiamawsIdentificativiGet(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Me.wsStrutture = New ChiamawsStruttureQry(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Me.wsTerritorio = New ChiamawsTerritorioQry(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Me.wsGestioneAssConsorzi = New ChiamawsGestioneAssConsorzi(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Me.wsInterrogazioniModello4 = New wsInterrogazioneModello4(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Me.sincronizzatoreAllevamento = New SincroBDNAllevamento(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
    End Sub

    Public Sub New(objParametriServer As AgronicaCoreParametri, objParametriUtenti As AgronicaCoreParametri)
        Me.objParametriServer = objParametriServer
        Me.objParametriUtenti = objParametriUtenti

        Me.wsRegistroStalla = New ChiamawsRegistroStallaQry(objParametriServer, objParametriUtenti, "", "", "")
        Me.wsAnagraficaCapo = New ChiamawsAnagraficaCapoQry(objParametriServer, objParametriUtenti, "", "", "")
        Me.wsAziende = New ChiamawsAziendeQry(objParametriServer, objParametriUtenti, "", "", "")
        Me.wsCodici = New ChiamawsCodiciQry(objParametriServer, objParametriUtenti, "", "", "")
        Me.wsIdentificativi = New ChiamawsIdentificativiGet(objParametriServer, objParametriUtenti, "", "", "")
        Me.wsStrutture = New ChiamawsStruttureQry(objParametriServer, objParametriUtenti, "", "", "")
        Me.wsTerritorio = New ChiamawsTerritorioQry(objParametriServer, objParametriUtenti, "", "", "")
        Me.wsGestioneAssConsorzi = New ChiamawsGestioneAssConsorzi(objParametriServer, objParametriUtenti, "", "", "")
        Me.wsInterrogazioniModello4 = New wsInterrogazioneModello4(objParametriServer, objParametriUtenti, "", "", "")
        Me.sincronizzatoreAllevamento = New SincroBDNAllevamento(objParametriServer, objParametriUtenti, "", "", "")
    End Sub

    Public Function SincronizzaAzienda(codice_azienda_BDN As String,
                                       Piva As String,
                                       ByVal sincronizzaAllevamenti As Boolean,
                                       ByVal sincronizzaSuDetentore As Boolean) As List(Of SincroBDN_Allevamento_Response)

        Dim resp As New List(Of SincroBDN_Allevamento_Response)
        Dim getAziendaResponse = wsAziende.getAzienda(codice_azienda_BDN)
        Dim infoAziendaResponse As DataTable

        Dim customLOGParams As New CustomLOGParams With {
            .LogDescrizioneUtente = objParametriServer.LogDescrizioneUtente,
            .LogDirectory = objParametriServer.LogDirectory & "\BDN\",
            .LogFileName = Date.Now.Year & Date.Now.Month & Date.Now.Day & " " & codice_azienda_BDN & ".txt"
        }

        Dim objLog As New AgronicaCoreDataProvider.LogProvider
        objLog.Scrivi_LOG(objParametriServer,
                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Inizio Sincronizzazione Azienda " & codice_azienda_BDN,
                          CustomLOGParams:=customLOGParams)

        If getAziendaResponse IsNot Nothing AndAlso getAziendaResponse.Rows.Count > 0 Then
            infoAziendaResponse = wsAziende.getInfoAzienda(getAziendaResponse.Rows(0)("AZIENDA_ID"))

            Dim cuaa = ""
            If sincronizzaSuDetentore Then
                Dim objImprese_Codici As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
                cuaa = objImprese_Codici.Leggi_CUAA(Piva, objParametriServer)

                If cuaa = "" Then
                    Throw New GiasException("CUAA non impostato per l'azienda")
                End If

            End If

            Dim allevamenti = wsAziende.FindAllevamento(codice_azienda_BDN, "", "")

            If sincronizzaAllevamenti Then
                For Each allevamento In allevamenti.Rows

                    'controlla se l'allevamento è attivo
                    If allevamenti.Columns.Contains("DT_FINE_ATTIVITA") Then
                        Dim DataFine As Date = AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAFINE

                        If Not IsDBNull(allevamento("DT_FINE_ATTIVITA")) _
                           AndAlso allevamento("DT_FINE_ATTIVITA") <> "" _
                           AndAlso IsDate(allevamento("DT_FINE_ATTIVITA")) Then
                            DataFine = CDate(allevamento("DT_FINE_ATTIVITA"))

                        End If

                        If DataFine < Date.Now Then
                            Continue For
                        End If

                    End If

                    Dim ID_Fiscale As String = allevamento("ID_FISCALE")
                    Dim allev_id As String = allevamento("ALLEV_ID")
                    Dim spe_codice As String = allevamento("SPE_CODICE")
                    Dim id_fiscale_detentore As String = allevamento("ID_FISCALE_DETEN")
                    'sincronizzazione allevamento
                    'sincronizzazione allevamento
                    'sincronizzatoreAllevamento.SincronizzaAllevamento(codice_azienda_BDN, ID_Fiscale, spe_codice, allev_id)

                    objLog.Scrivi_LOG(objParametriServer,
                                      System.Reflection.MethodBase.GetCurrentMethod().Name,
                                      "Inizio Sincronizzazione Allevamento " & codice_azienda_BDN & " " & ID_Fiscale & " " & spe_codice,
                                      CustomLOGParams:=customLOGParams)

                    Try
                        Dim totCapiCaricati = 0
                        Dim totCapiScaricati = 0
                        'Dim objSincro_Allevamento As SincroBDN_Allevamento_Response = sincronizzatoreAllevamento.SincronizzaAllevamento(Piva,
                        '                                                                                                                codice_azienda_BDN,
                        'leggiModelli4Uscita(Asl_Codice, codice_azienda_BDN,
                        '          azienda_ID, ID_Fiscale, allev_id)

                        'Dim totCapiCaricati = 0
                        'Dim totCapiScaricati = 0
                        Dim objSincro_Allevamento As SincroBDN_Allevamento_Response
                        If sincronizzaSuDetentore AndAlso id_fiscale_detentore <> cuaa Then
                            Continue For
                        End If
                        'objSincro_Allevamento = sincronizzatoreAllevamento.SincronizzaAllevamento(Piva, codice_azienda_BDN, ID_Fiscale, spe_codice)

                        'totCapiCaricati = objSincro_Allevamento.listaCapi_Ingresso.Count
                        'totCapiScaricati = objSincro_Allevamento.listaCapi_Uscita.Count

                        objLog.Scrivi_LOG(objParametriServer,
                                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                                          "Fine Sincronizzazione Allevamento " & codice_azienda_BDN & " " & ID_Fiscale & " " & spe_codice & vbCrLf &
                                          " Caricati " & totCapiCaricati & " capi" & vbCrLf &
                                          " Scaricati " & totCapiScaricati & " capi",
                                          CustomLOGParams:=customLOGParams)
                        'resp.Add(objSincro_Allevamento)
                    Catch ex As BDNException
                        objLog.Scrivi_LOG(objParametriServer,
                                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                                          "Errore Sincronizzazione Allevamento " & codice_azienda_BDN & " " & ID_Fiscale & " " & spe_codice & vbCrLf &
                                          " Errore: " & ex.Message,
                                          CustomLOGParams:=customLOGParams)
                        Throw ex
                    Catch ex As GiasException
                        objLog.Scrivi_LOG(objParametriServer,
                                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                                          "Errore Sincronizzazione Allevamento " & codice_azienda_BDN & " " & ID_Fiscale & " " & spe_codice & vbCrLf &
                                          " Errore: " & ex.Message,
                                          CustomLOGParams:=customLOGParams)
                        Throw ex
                    Catch ex As Exception
                        objLog.Scrivi_LOG(objParametriServer,
                                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                                          "Errore Sincronizzazione Allevamento " & codice_azienda_BDN & " " & ID_Fiscale & " " & spe_codice & vbCrLf &
                                          " Errore: " & ex.Message,
                                          CustomLOGParams:=customLOGParams)
                    End Try
                Next

                objLog.Scrivi_LOG(objParametriServer,
                                  System.Reflection.MethodBase.GetCurrentMethod().Name,
                                  "Fine Sincronizzazione Azienda " & codice_azienda_BDN,
                                  CustomLOGParams:=customLOGParams)

            End If

        End If

        objLog.Scrivi_LOG(objParametriServer,
                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Fine Sincronizzazione Azienda " & codice_azienda_BDN,
                          CustomLOGParams:=customLOGParams)

        Return resp

    End Function


End Class
