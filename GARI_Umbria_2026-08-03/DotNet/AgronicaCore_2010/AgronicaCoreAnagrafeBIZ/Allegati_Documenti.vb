Imports System.IO
Imports System.Linq
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreModelsSTD.Gis

Public Class Allegati_Documenti

    Public Shared Function LeggiListaAllegatiDocumentiCodDaListaRicettaOperazioni(ByRef objParametri_Server As AgronicaCoreParametri, sFiltro As String) As String


        Dim leggiAllegati As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_R

        Dim dtAllegati As DataTable =
            leggiAllegati.LeggiListaCodDaListaRicettaOperazioni(enumSelezioneVariabile.Selezione_TabellaCompleta, sFiltro, "", objParametri_Server)

        If dtAllegati.Rows.Count = 0 Then
            Return "-1"
        End If

        Dim sFiltroRicettaOperazioni As String = String.Join(",",
            (From dt In dtAllegati.AsEnumerable Select dt("Allegati_Documenti_Cod")).ToList()
        )

        Return sFiltroRicettaOperazioni
    End Function
    Public Shared Function LeggiAllegatiDaRicettaDestinazione(ByVal PivaSuperUser As String,
                                                              ByVal Piva As String,
                                                              ByVal Sa_Cod As Int32,
                                                              ByVal Appezza As Int32,
                                                              ByVal Id_Imp As Int32,
                                                              ByVal RicettaOperazione_cod As Int32,
                                                              ByVal Allegati_Documenti_CatCod As Int32,
                                                              ByRef objParametri_Server As AgronicaCoreParametri) As LeggiAllegatiDaRicettaDestinazione

        Dim ret As New LeggiAllegatiDaRicettaDestinazione
        ret.ListaAllegati = New List(Of AllegatiDaRicettaDestinazione)

        Dim leggiAllegati As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_R

        Dim dtAllegati As DataTable =
            leggiAllegati.LeggiAllegatiDaRicettaDestinazione(PivaSuperUser,
                                                             Piva,
                                                             Sa_Cod,
                                                             Appezza,
                                                             Id_Imp,
                                                             RicettaOperazione_cod,
                                                             Allegati_Documenti_CatCod,
                                                             enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                             "",
                                                             objParametri_Server)

        For Each row In dtAllegati.Rows
            Dim allegato As New AllegatiDaRicettaDestinazione

            allegato.Allegati_Documenti_Cod = Int32.Parse(row("Allegati_Documenti_Cod"))
            allegato.Allegati_Documenti_Des = row("Allegati_Documenti_Des").ToString

            ret.ListaAllegati.Add(allegato)
        Next

        Return ret
    End Function


End Class


Public Class Allegati_Documenti_W

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Allegati_Documenti_CatCod"></param>
    ''' <param name="DescrizioneDelPiano"></param>
    ''' <param name="objParametri_Server"></param>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="Appezza"></param>
    ''' <param name="Ricetta_Operazione_Cod"></param>
    ''' <param name="regImpianto"></param>
    ''' <param name="xmlAgronica2012">Stringa Vuota se non richiesto</param>
    ''' <param name="fileAllegatoDB">nothing se non richiesto</param>
    ''' <param name="scritturaDaEngineMappePrescrizione">indica se la scrittura arriva da Engine MappePrescrizione</param>
    ''' <param name="objParametri_Super_Server">necessario solo se scrittura arriva da Engine MappePrescrizione</param>
    ''' <returns></returns>
    Public Function PrecisionFarmingScriviSuAllegati(
            Allegati_Documenti_CatCod As enum_CategorieDocumenti,
            DescrizioneDelPiano As String,
            Allegati_Documenti_NomeFile As String,
            objParametri_Server As AgronicaCoreParametri,
            Piva As String,
            Sa_Cod As Integer,
            Appezza As Integer,
            Ricetta_Operazione_Cod As Integer,
            regImpianto As Integer,
            xmlAgronica2012 As String,
            fileAllegatoDB As Byte(),
            jsonAgronica2012 As String,
            ByRef OUTPUT_Allegati_Documenti_Cod As Integer,
            ByVal Optional usaTransazioneEsterna As Boolean = False,
            ByVal Optional scritturaDaEngineMappePrescrizione As Boolean = False,
            ByRef Optional objParametri_Super_Server As AgronicaCoreParametri = Nothing
    ) As RispostaStandard

        Dim rval As New RispostaStandard
        rval.RispostaOK = True

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False


        'TODO: Gestire la tranzazione
        Try

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Apro la connessione al DB
            If Not usaTransazioneEsterna Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                        FlagTransazioneLocale,
                                                                                        objParametri_Server)
            End If

            '' VAnni: 19/2/2021: scrivo in allegati_Documenti e nelle entità il dato. 
            Dim allegatiLeggiPerEntita As New AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_R
            Dim allegatiScrivi As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_W
            Dim alertEntitaScrivi As New AgronicaCoreScadenziario.Alert_Entita_W
            Dim alertElencoScrivi As New AgronicaCoreScadenziario.Alert_Elenco_W

            If DescrizioneDelPiano = "" Then
                DescrizioneDelPiano = "Piano a Rateo Variabile per Ricetta_Operazione_Cod = " & Ricetta_Operazione_Cod
            End If

            Dim scriviAllegato As Integer = 0
            Dim sottoCartella As String = ""
            Dim Estensione As String = ""
            If Not IsNothing(fileAllegatoDB) Then
                scriviAllegato = 1
                Dim vFile1 As String() = Allegati_Documenti_NomeFile.Split(".")
                Estensione = vFile1(vFile1.Length - 1)
                Select Case Allegati_Documenti_CatCod
                    Case enum_CategorieDocumenti.PrecisionFarming_MappaPrescrizione
                        sottoCartella = "PrecisionFarming_MappaPrescrizione"
                    Case enum_CategorieDocumenti.PrecisionFarming_MappaProduzione
                        sottoCartella = "PrecisionFarming_MappaProduzione"
                    Case enum_CategorieDocumenti.PrecisionFarming_FileBordoMacchina
                        sottoCartella = "PrecisionFarming_FileBordoMacchina"
                End Select
            End If
            
            If scritturaDaEngineMappePrescrizione Then
                ' Salviamo l'allegato su File
                scriviAllegato = 0
                
                Dim agroWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
                agroWebConfig.impostaDaDB(objParametri_Super_Server, objParametri_Server)
                Dim pathFile As String = agroWebConfig.GestioneAllegati_Repository.TrimEnd("\") & "\" & sottoCartella
                
                If Not Directory.Exists(pathFile) Then
                    Directory.CreateDirectory(pathFile)
                End If

                Dim percorsoCompleto = Path.Combine(pathFile, Allegati_Documenti_NomeFile)
                File.WriteAllBytes(percorsoCompleto, fileAllegatoDB)
            End If

            allegatiScrivi.Scrivi(
                Allegati_Documenti_Piva:=Piva,
                Allegati_Documenti_Des:=DescrizioneDelPiano,
                Allegati_Documenti_CatCod:=Allegati_Documenti_CatCod,
                Allegati_Documenti_NomeFile:=Allegati_Documenti_NomeFile,
                Allegati_Documenti_Numero:="",
                Allegati_Documenti_Ente_Cod:=0,
                Sottocartella:=sottoCartella,
                Validita_Inizio:=AGRODATAINIZIO,
                Validita_Fine:=AGRODATAFINE,
                OUTPUT_Allegati_Documenti_Cod:=OUTPUT_Allegati_Documenti_Cod,
                objParametri:=objParametri_Server,
                strXml:=xmlAgronica2012,
                strJSON:=jsonAgronica2012,
                SalvaAllegato:=scriviAllegato,
                File_Allegato_DB:=fileAllegatoDB
            )


            Dim Base As Integer = 0
            Dim Top As Integer = 2000000000

            Dim objSeq As New Agro_Sequenze
            Dim id_Entita As Integer = objSeq.NuovoId_Tabella("Alert_Entita", Base, Top, objParametri_Server)

            alertEntitaScrivi.Scrivi(New AgronicaCoreScadenziario.Alert_Entita With {
                    .ID_Alert_Entita = id_Entita,
                    .TipoEntita_Cod = enum_TipoEntita.Ricetta_Destinazione,
                    .PivaSuperUser = objParametri_Server.PivaSuperUser,
                    .Allegati_Documenti_Cod = OUTPUT_Allegati_Documenti_Cod,
                    .Piva = Piva,
                    .Sa_Cod = Sa_Cod,
                    .Appezza = Appezza,
                    .Id_Imp = regImpianto,
                    .Ricetta_Operazione_cod = Ricetta_Operazione_Cod,
                    .ChkDocumento = 1
                    },
                AGRODATAINIZIO,
                AGRODATAFINE,
                objParametri_Server
            )

            Dim id_Elenco As Integer = objSeq.NuovoId_Tabella("Alert_Elenco", Base, Top, objParametri_Server)

            Dim idAreaTipologia As enum_ID_Area_Tipologia
            Select Case Allegati_Documenti_CatCod
                Case enum_CategorieDocumenti.PrecisionFarming_MappaPrescrizione
                    idAreaTipologia = enum_ID_Area_Tipologia.AgricolturaDiPrecisione_MappaPrescrizione
                Case enum_CategorieDocumenti.PrecisionFarming_MappaProduzione
                    idAreaTipologia = enum_ID_Area_Tipologia.AgricolturaDiPrecisione_MappaProduzione
                Case enum_CategorieDocumenti.PrecisionFarming_FileBordoMacchina
                    idAreaTipologia = enum_ID_Area_Tipologia.AgricolturaDiPrecisione_FileBordoMacchina
            End Select

            alertElencoScrivi.Scrivi(
                id_Elenco, idAreaTipologia,
                id_Entita, AGRODATAFINE, "", False,
                AGRODATAINIZIO, AGRODATAFINE, objParametri_Server, "")

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Chiudo la transazione e la connessione al DB
            If Not usaTransazioneEsterna Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)
            End If
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        Catch ex As Exception

            'Faccio il rollback della transazione
            If Not objParametri_Server.objTransazione Is Nothing And Not usaTransazioneEsterna Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)

            End If

            rval.Errore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)
            rval.RispostaOK = False



        Finally
            If Not usaTransazioneEsterna Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)
            End If
        End Try

        Return rval
    End Function

    Public Function PrecisionFarmingAggiornaAllegati(
            allegati_Documenti_CatCod As enum_CategorieDocumenti,
            allegati_Documenti_Cod As Integer,
            descrizioneDelPiano As String,
            allegati_Documenti_NomeFile As String,
            objParametri_Server As AgronicaCoreParametri,
            Piva As String,
            Sa_Cod As Integer,
            Appezza As Integer,
            Ricetta_Operazione_Cod As Integer,
            regImpianto As Integer,
            xmlAgronica2012 As String,
            fileAllegatoDB As Byte(),
            jsonAgronica2012 As String,
            ByVal Optional scritturaDaEngineMappePrescrizione As Boolean = False,
            ByRef Optional objParametri_Super_Server As AgronicaCoreParametri = Nothing
    ) As RispostaStandard

        Dim rval As New RispostaStandard
        rval.RispostaOK = True

        Dim flagTransazioneLocale As Boolean = False
        Dim flagConnessioneLocale As Boolean = False

        Try

            ConnessioniTransazioni.ApriConnessioneXCoreBiz(flagConnessioneLocale,
                                                                            flagTransazioneLocale,
                                                                            objParametri_Server)


            Dim allegatiScrivi As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_W
            Dim allegatiLeggi As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_R
            Dim agroWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
            Dim pathOriginale As String
            
            If scritturaDaEngineMappePrescrizione Then
                agroWebConfig.impostaDaDB(objParametri_Super_Server, objParametri_Server)
            End If

            If descrizioneDelPiano = "" Then
                descrizioneDelPiano = "Piano a Rateo Variabile per Ricetta_Operazione_Cod = " & Ricetta_Operazione_Cod

                Dim dtori = allegatiLeggi.Leggi(allegati_Documenti_Cod, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                If dtori.Rows.Count > 0 Then
                    pathOriginale = Path.Combine(agroWebConfig.GestioneAllegati_Repository, dtori.Rows(0)("sottoCartella"), dtori.Rows(0)("Allegati_documenti_NomeFile"))
                    If dtori.Rows(0)("Allegati_documenti_des").ToString() <> "" Then
                        descrizioneDelPiano = dtori.Rows(0)("Allegati_documenti_des").ToString()
                    End If
                End If
            End If

            Dim scriviAllegato = 0
            Dim sottoCartella = ""
            If Not IsNothing(fileAllegatoDB) Then
                scriviAllegato = 1
                Select Case allegati_Documenti_CatCod
                    Case enum_CategorieDocumenti.PrecisionFarming_MappaPrescrizione
                        sottoCartella = "PrecisionFarming_MappaPrescrizione"
                    Case enum_CategorieDocumenti.PrecisionFarming_MappaProduzione
                        sottoCartella = "PrecisionFarming_MappaProduzione"
                    Case enum_CategorieDocumenti.PrecisionFarming_FileBordoMacchina
                        sottoCartella = "PrecisionFarming_FileBordoMacchina"
                End Select
            End If
            
            
            If scritturaDaEngineMappePrescrizione Then
                ' Salviamo l'allegato su File
                scriviAllegato = 0
                
                If Not String.IsNullOrEmpty(pathOriginale) And File.Exists(pathOriginale) Then
                    File.Delete(pathOriginale)
                End If
                
                Dim pathFile As String = agroWebConfig.GestioneAllegati_Repository.TrimEnd("\") & "\" & sottoCartella
                If Not Directory.Exists(pathFile) Then
                    Directory.CreateDirectory(pathFile)
                End If

                Dim percorsoCompleto = Path.Combine(pathFile, allegati_Documenti_NomeFile)
                File.WriteAllBytes(percorsoCompleto, fileAllegatoDB)
            End If

            allegatiScrivi.Modifica(
                Allegati_Documenti_Piva:=Piva,
                Allegati_Documenti_Des:=descrizioneDelPiano,
                Allegati_Documenti_NomeFile:=allegati_Documenti_NomeFile,
                Allegati_Documenti_Numero:="",
                Sottocartella:=sottoCartella,
                Validita_Inizio:=AGRODATAINIZIO,
                Validita_Fine:=AGRODATAFINE,
                Allegati_Documenti_Cod:=allegati_Documenti_Cod,
                objParametri:=objParametri_Server,
                strXml:=xmlAgronica2012,
                strJSON:=jsonAgronica2012,
                SalvaAllegato:=scriviAllegato,
                File_Allegato_DB:=fileAllegatoDB,
                bAllegato_Modificato:=Not fileAllegatoDB Is Nothing
            )

            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(flagTransazioneLocale, objParametri_Server)
        Catch ex As Exception
            If Not objParametri_Server.objTransazione Is Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

            rval.Errore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)
            rval.RispostaOK = False
        Finally
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(flagConnessioneLocale, objParametri_Server)
        End Try

        Return rval
    End Function
End Class
