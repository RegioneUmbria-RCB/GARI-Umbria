Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreUtentiDAL

Public Class ElencoSA
    Public Tipo As String
    Public Principio_Attivo As String
    Public Principio_Attivo_Descrizione As String
    Public udm_cod As String
    Public udm_sim As String
    Public Qta As String
End Class

Public Class ImportaRisultatiAnalisi

    Public Function ScriviRisultatoAnalisi(ByVal Analisi_testata_cod As String,
                                           ByVal codiceAnalisi As String,
                                           ByVal dataInizioAnalisi As DateTime,
                                           ByVal dataFineAnalisi As DateTime,
                                           ByVal listaSostanzeRilevate As List(Of ElencoSA),
                                           ByRef codRisUm As Integer,
                                           ByRef objParametriServer As AgronicaCoreParametri,
                                           ByRef objParametriUtenti As AgronicaCoreParametri
                                           ) As String

        Dim xRisp As String = ""
        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False

        Try
            'apro la transazione
            Utility.VerificaApriTransazione(objParametriServer, flagConnessione, flagTransazione)

            Dim objTestataR As New AgronicaCoreAnagrafeDAL.Analisi_Testata_R
            Dim dtTestata As DataTable = objTestataR.Leggi(Analisi_testata_cod, 0,
                                                           AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                           "", "", objParametriServer)

            If IsNothing(dtTestata) OrElse dtTestata.Rows.Count = 0 Then
                Throw New Exception(String.Format("Impossibile trovare l'analisi con Analisi_Testata_Cod={0}", Analisi_testata_cod))
            End If

            Dim Analisi_Tipo As String = dtTestata.Rows(0).Item("Analisi_Testata_Tipo")
            Dim altre As String = dtTestata.Rows(0).Item("Analisi_Testata_Note1")
            
            Dim objTestataW As New AgronicaCoreAnagrafeDAL.Analisi_Testata_W
            objTestataW.Modifica(Analisi_testata_cod, 0,
                                 codiceAnalisi,
                                 dataInizioAnalisi,
                                 dataFineAnalisi,
                                 0, 0, "", "", "", "", "", altre, "", "", "",
                                 Analisi_Tipo,
                                 0, 0, AGRODATAINIZIO, AGRODATAFINE, objParametriServer)

            'identifico il codice 
            Dim objPdc_Ananalisi_R As New AgronicaCorePianidiCampionamentoDAL.PDC_Analisi_R
            Dim dtPDC As DataTable = objPdc_Ananalisi_R.Leggi(0, 0, 0, Analisi_testata_cod,
                                                              "", "", objParametriServer)

            codRisUm = dtPDC.Rows(0).Item("Cod_Risum")

            Dim objPDCAnalisiW As New AgronicaCorePianidiCampionamentoDAL.PDC_Analisi_W
            objPDCAnalisiW.ModificaPuntuale(dtPDC.Rows(0).Item("Id_PDC_Testata"),
                                            dtPDC.Rows(0).Item("ID_PDC_Dettagli"),
                                            dtPDC.Rows(0).Item("ID_PDC_Campione"),
                                            Analisi_testata_cod,
                                            objParametriServer,
                                            PDC_Stato_Analisi:=enum_PDC_Stato_Analisi.Analizzata)

            'elimino tutti i dettagli precedenti
            Dim objDettagli As New AgronicaCoreAnagrafeDAL.Analisi_Dettagli_W
            objDettagli.Cancella(Analisi_testata_cod, 0, 0, "", objParametriServer)

            Dim progressivo As Integer = 0
            Dim topCode As Integer = 0
            Dim baseCode As Integer = 0
            CodiciProgressivi.Trova_Base_e_Top(progressivo, baseCode, topCode, objParametriUtenti, objParametriServer)

            For Each obj As ElencoSA In listaSostanzeRilevate

                'scrivo i singoli dettagli
                Dim objAgroSeq As New AgronicaCoreDataProvider.Agro_Sequenze
                Dim indice As Integer = objAgroSeq.NuovoId_Tabella("Analisi_Dettagli", baseCode, topCode, objParametriServer)

                Dim PaCod_FamCod As Integer = 0
                Select Case obj.Tipo
                    Case enum_PDC_Tipo_Risultato.Singolo_PA
                        PaCod_FamCod = CInt(obj.Principio_Attivo)

                    Case enum_PDC_Tipo_Risultato.Famiglia_PA
                        PaCod_FamCod = 0 - CInt(obj.Principio_Attivo)

                    Case enum_PDC_Tipo_Risultato.Merceologica
                        PaCod_FamCod = CInt(obj.Principio_Attivo)

                    Case Else
                        Throw New Exception(String.Format("Risultato analisi di tipo [{0}] non riconosciuto", obj.Tipo))
                End Select

                objDettagli.Scrivi(Analisi_testata_cod, indice,
                                   PaCod_FamCod, CDbl(obj.Qta), 0,
                                   obj.udm_cod, 0,
                                   0, AGRODATAINIZIO, AGRODATAFINE, objParametriServer)

            Next

            Utility.VerificaChiudiTransazione(objParametriServer, flagTransazione)

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(objParametriServer, flagTransazione)
            xRisp = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True, source:=True)
        Finally
            Utility.VerificaChiudiConnessione(objParametriServer, flagConnessione)
        End Try

        Return xRisp

    End Function

    Public Sub SalvaAllegatoSuTabella(ByVal Analisi_testata_cod As Integer,
                                      ByVal Numero_file As String,
                                      ByVal soloNome As String,
                                      ByRef PathCompleto As String,
                                      ByRef objParametriServer As AgronicaCoreParametri,
                                      ByRef objParametriUtenti As AgronicaCoreParametri)

        Const nomeRoutine = "SalvaAllegatoSuTabella()"

        Try

            Dim sottoDirectory As String = "Analisi"
            Dim objConf As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim dtConf As DataTable = objConf.Leggi(0, "GestioneAllegati_Repository", "", "", objParametriServer)
            PathCompleto = dtConf.Rows(0).Item("Valore") & sottoDirectory & "\"

            'controllo se esiste la cartella 
            If Not IO.Directory.Exists(PathCompleto) Then
                IO.Directory.CreateDirectory(PathCompleto)
            End If

            'controllo se esiste già un Allegati_Documenti con quel numero
            Dim objAllegatoDoc_R As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_R
            Dim dtAllegato As DataTable = objAllegatoDoc_R.LeggixSincroAnalisi(Analisi_testata_cod, Numero_file, objParametriServer)

            If Not dtAllegato Is Nothing AndAlso dtAllegato.Rows.Count > 0 Then
                'ho già un allegato con quel numero ==> lo sostituisco senza modificare nulla

                If IO.File.Exists(PathCompleto & soloNome) = True Then
                    IO.File.Delete(PathCompleto & soloNome)
                End If

            Else

                Dim progressivo As Integer = 0
                Dim topCode As Integer = 0
                Dim baseCode As Integer = 0
                CodiciProgressivi.Trova_Base_e_Top(progressivo, baseCode, topCode, objParametriUtenti, objParametriServer)

                Dim allegatiDocumentiCod As Integer
                Dim objAllegati As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_W
                objAllegati.Scrivi(objParametriServer.PivaSuperUser, Numero_file,
                                   enum_CategorieDocumenti.AnalisiFitofarmaci, soloNome,
                                   "", 0, sottoDirectory, 
                                   AGRODATAINIZIO, AGRODATAFINE,
                                   allegatiDocumentiCod, objParametriServer)

                Dim objAgroSeq As New AgronicaCoreDataProvider.Agro_Sequenze
                Dim Allegati_EntitaxDocumenti As Integer = objAgroSeq.NuovoId_Tabella("Allegati_EntitaxDocumenti", baseCode, topCode, objParametriServer)

                Dim objAllegati_Entita As New AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_W
                objAllegati_Entita.Scrivi_Analisi(allegatiDocumentiCod, Allegati_EntitaxDocumenti, Analisi_testata_cod,
                                                  AGRODATAINIZIO, AGRODATAFINE, objParametriServer)

            End If

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

    End Sub



End Class
