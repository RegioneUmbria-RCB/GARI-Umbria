Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDTOStd.InData.Utility
Imports Newtonsoft.Json

Public Class SottoscrizioneServizioQDC
    Public Function VerificaSottoscrizioneServizioQDC(piva As String, data As DateTime, obj_Server As AgronicaCoreParametri) As Boolean
        Dim check As Boolean = False
        Dim objCfgSiti = New AgronicaCoreVarieBIZ.Configurazione_Siti_BIZ_R

        If objCfgSiti.CheckSottoscrizioneServizioQDC(obj_Server) And (data > CostantiPersonalizzate.AGRODATAINIZIO AndAlso data < CostantiPersonalizzate.AGRODATAFINE) Then
            Dim cfgCheck = objCfgSiti.GetConfigControlloServizioQDC(obj_Server)
            Select Case cfgCheck.tipo
                Case TipiEnumerativi.enum_TipoControlloSottoscrizioneServizioQDC.ControlloStatoPratica

                    Dim cfgTipoControllo = JsonConvert.DeserializeObject(Of ConfigurazioneControlloServizioQDC(Of ParametriControlloServizioQDCColdiretti))(JsonConvert.SerializeObject(cfgCheck))

                    check = CheckStatoPratica(piva,
                                              data,
                                              cfgTipoControllo.pars.codiceServizio,
                                              cfgTipoControllo.pars.statiAmmessi,
                                              obj_Server)
                Case Else
                    check = False
            End Select
        Else
            'nel caso in cui non sia attivo il controllo, non devo bloccare l'inserimento di operazioni qdc
            check = True
        End If
        Return check
    End Function

    Private Function CheckStatoPratica(piva As String,
                                       data As DateTime,
                                       CodiceServizio As Integer,
                                       StatiAmmessi As List(Of Integer),
                                       objServer As AgronicaCoreParametri) As Boolean

        Dim ret As Boolean = False

        If CodiceServizio = 0 Then
            Throw New Exception("Specificare il codice del servizio\pratica da verificare")
        End If

        If StatiAmmessi Is Nothing OrElse StatiAmmessi.Count <= 0 Then
            Throw New Exception("Specificare gli stati della pratica da verificare")
        End If

        Dim xPraticheRead As New AgronicaCoreProfilazioneDAL.Pratiche_R
        Dim xPraticheStatiRead As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_Attuali_R
        Dim xImpCod_R As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        Dim CUAA As String = ""
        Try
            Dim dt = xImpCod_R.Leggi(piva, TipiEnumerativi.enum_CodiciAnagrafe.CodiceCUAA, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objServer)
            If dt.Rows.Count > 0 Then
                CUAA = dt.Rows(0)("val_cod")
            Else
                'Throw New Exception("Azienda non trovata in gias")
            End If

            Dim dtp = xPraticheRead.Leggi(0, "", piva, CUAA, 0, 0, 0, 1014, data, data, "", "", objServer, 0, 0)
            If dtp.Rows.Count > 0 Then
                Dim dts = xPraticheStatiRead.Leggi(dtp.Rows(0)("Pratica_Cod"), "", "", objServer)
                If dts.Rows.Count > 0 Then
                    If StatiAmmessi.Contains(CInt(dts.Rows(0)("Stato_Cod"))) Then
                        ret = True
                    End If
                Else
                    'Throw New Exception(String.Format("Nessuna pratica per il Servizio QDC Bluarancio trovata sul CUAA {0} nell'anno corrente. Impossibile procedere", CUAA))
                End If
            Else
                'Throw New Exception(String.Format("Nessuna pratica per il Servizio QDC Bluarancio trovata sul CUAA {0} nell'anno corrente. Impossibile procedere", CUAA))
            End If

        Catch ex As Exception
            ret = False
            Throw ex
        End Try

        Return ret
    End Function
End Class
