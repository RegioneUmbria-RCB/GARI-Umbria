
Imports System.Data.Common
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class AnalizzaModifiche
    Inherits AgronicaCoreDataProvider.LogProvider


    Public Function Leggi_DT_Modifiche(ByVal kTabella As ChiaveTabella) As DS_AnalizzaModifiche.DT_AnalizzaModificheDataTable

        Dim dt As DS_AnalizzaModifiche.DT_AnalizzaModificheDataTable = Nothing
        Dim idCod As Int32

        Try

            Select Case kTabella.Tabella_Nome

                Case DatiAnagrafici_Azienda

                    idCod = enum_DatiAnagrafici_CodiciAnagrafe.Azienda

            End Select

        Catch ex As Exception
            dt = Nothing
        End Try

        Return dt

    End Function


    'Leggo la maschera di bit associata ad una chiave
    Public Function LeggiStato(ByVal key As ChiaveTabella,
                               ByVal FinestraTemp_Inizio As Date,
                               ByVal FinestraTemp_Fine As Date,
                               ByRef objConnessione As DbConnection,
                               ByVal StringaConnessione As String,
                               ByRef objTransazione As DbTransaction,
                               ByVal FlagVisibilita As Int32,
                               ByVal DirectoryLOG As String,
                               ByVal FileLOG As String,
                               ByVal IdentificatoreUtente As String,
                               ByRef MessaggioErrore As String,
                               ByRef objParametri As AgronicaCoreParametri
                               ) As Boolean()

        Const nomeRoutine = "GestoreModificheBIZ.AnalizzaModifiche.LeggiStato()"

        Dim vRisp(31) As Boolean
        Dim objAnagrafeDAL As AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        Dim objAnagrafeMacDAL As AgronicaCoreContabDAL.Parco_Macchine_Codici_R
        Dim objAnagrafeFabDAL As AgronicaCoreAnagrafeDAL.Fabbricati_Codici_R
        Dim objAnagrafeConDAL As AgronicaCoreAnagrafeDAL.Contatti_Codici_R
        Dim objAnagrafeParCodDAL As AgronicaCoreAnagrafeDAL.ImpresexParticelle_Codici_R
        Dim objAnagrafePCDAL As AgronicaCoreAnagrafeDAL.Programmazione_Entita_Codici_R
        Dim xConnectionState As ConnectionState = ConnectionState.Closed
        Dim dt As DataTable

        Dim xConnessione As DbConnection

        Dim FlagConnessioneLocale As Boolean = False

        Dim sValore As String
        Dim vValore(0) As Int32
        Dim b As BitArray

        Try

            If key IsNot Nothing Then

                '------------------------------
                'Verifico se è stata impostata una connessione
                If IsNothing(objConnessione) Then
                    'Flag
                    FlagConnessioneLocale = True
                    'Creo la connessione localmente
                    xConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(StringaConnessione)
                Else
                    'Utilizzo quella passata come parametro
                    xConnessione = objConnessione
                End If
                '------------------------------

                Select Case key.Tabella_Nome

                    Case DatiAnagrafici_Azienda

                        '====================================================================
                        '-------------------------- AZIENDA ---------------------------------
                        '====================================================================

                        objAnagrafeDAL = New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read

                        dt = objAnagrafeDAL.LeggixCodice(key.Piva, enum_DatiAnagrafici_CodiciAnagrafe.Azienda,
                                                         enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                         "", "", objParametri)

                    Case DatiAnagrafici_Catasto

                        '====================================================================
                        '-------------------------- CATASTO ---------------------------------
                        '====================================================================

                        objAnagrafeParCodDAL = New AgronicaCoreAnagrafeDAL.ImpresexParticelle_Codici_R

                        dt = objAnagrafeParCodDAL.Leggi(key.ID,
                                                        "", 0, "", "", "", 0, 0, "",
                                                        enum_DatiAnagrafici_CodiciAnagrafe.Catasto,
                                                        "",
                                                        enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                        "", "", objParametri)

                    Case DatiAnagrafici_Contatti

                        '====================================================================
                        '-------------------------- RAPPR. LEGALE ---------------------------
                        '====================================================================

                        objAnagrafeConDAL = New AgronicaCoreAnagrafeDAL.Contatti_Codici_R

                        dt = objAnagrafeConDAL.Leggi(key.Piva, key.Cod_Contatto,
                                                     enum_DatiAnagrafici_CodiciAnagrafe.Rappr_Legale, "",
                                                     enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                     "", "", objParametri)

                    Case DatiAnagrafici_Macchine

                        '====================================================================
                        '-------------------------- MACCHINE --------------------------------
                        '====================================================================

                        objAnagrafeMacDAL = New AgronicaCoreContabDAL.Parco_Macchine_Codici_R

                        dt = objAnagrafeMacDAL.Leggi(key.Piva,
                                                     key.Sa_Cod,
                                                     key.Mac_Cod,
                                                     enum_DatiAnagrafici_CodiciAnagrafe.Macchine,
                                                     "",
                                                     enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                     "",
                                                     "",
                                                     objParametri)

                    Case DatiAnagrafici_Fabbricati

                        '====================================================================
                        '-------------------------- FABBRICATI ------------------------------
                        '====================================================================

                        objAnagrafeFabDAL = New AgronicaCoreAnagrafeDAL.Fabbricati_Codici_R

                        dt = objAnagrafeFabDAL.Leggi(key.Piva, key.Sa_Cod, key.Fabbricato_Cod,
                                                     enum_DatiAnagrafici_CodiciAnagrafe.Fabbricati, "",
                                                     enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                     "", "", objParametri)

                    Case DatiAnagrafici_Allevamenti

                        '====================================================================
                        '-------------------------- ALLEVAMENTI -----------------------------
                        '====================================================================

                        objAnagrafeFabDAL = New AgronicaCoreAnagrafeDAL.Fabbricati_Codici_R

                        dt = objAnagrafeFabDAL.Leggi(key.Piva, key.Sa_Cod, key.Fabbricato_Cod,
                                                     enum_DatiAnagrafici_CodiciAnagrafe.Allevamenti, "",
                                                     enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                     "", "", objParametri)

                    Case DatiAnagrafici_PianoColturale

                        '====================================================================
                        '-------------------------- PIANOCOLTURALE --------------------------
                        '====================================================================

                        objAnagrafePCDAL = New AgronicaCoreAnagrafeDAL.Programmazione_Entita_Codici_R

                        dt = objAnagrafePCDAL.Leggi(key.Programmazione_Entita_Cod,
                                                    enum_DatiAnagrafici_CodiciAnagrafe.PianoColturale,
                                                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                    "", "", objParametri)

                End Select

                If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then

                    sValore = CStr(dt.Rows(0).Item("Val_Cod"))
                    vValore(0) = CInt(sValore)

                    b = New BitArray(31)

                    b = New BitArray(vValore)

                    b.CopyTo(vRisp, 0)

                Else
                    vRisp = Nothing
                End If

            Else
                vRisp = Nothing
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            'Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, nomeRoutine, MessaggioErrore)

            Dim customLOGParams As New CustomLOGParams With {
                .LogDescrizioneUtente = IdentificatoreUtente,
                .LogDirectory = DirectoryLOG,
                .LogFileName = FileLOG
            }
            Scrivi_LOG(objParametri,
                       nomeRoutine,
                       MessaggioErrore,
                       CustomLOGParams:=customLOGParams)

            vRisp = Nothing
        Finally

            If FlagConnessioneLocale Then
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

        Return vRisp

    End Function

    'Modifico la maschera di bit associata ad una chiave
    Public Function AggiornaStato(ByVal keyDT As ChiaveDT,
                                  ByVal FinestraTemp_Inizio As Date,
                                  ByVal FinestraTemp_Fine As Date,
                                  ByRef objConnessione As DbConnection,
                                  ByRef objTransazione As DbTransaction,
                                  ByVal StringaConnessione As String,
                                  ByVal FlagVisibilita As Int32,
                                  ByVal DirectoryLOG As String,
                                  ByVal FileLOG As String,
                                  ByVal IdentificatoreUtente As String,
                                  ByRef MessaggioErrore As String,
                                  ByRef objParametri As AgronicaCoreParametri
                                  ) As Boolean

        Const nomeRoutine = "AgronicaCoreGestioneModificheBIZ.AnalizzaModifiche.AggiornaStato()"

        Dim i As Int32
        Dim vBit As BitArray
        Dim dr As DS_AnalizzaModifiche.DT_AnalizzaModificheRow
        Dim vStato(0) As Int32
        Dim sStato As String

        Dim objImpresa_Codici As AgronicaCoreAnagrafeDAL.Imprese_Codici_Write
        Dim objMacchine_Codici As AgronicaCoreContabDAL.Parco_Macchine_Codici_W
        Dim objFabb_Codici As AgronicaCoreAnagrafeDAL.Fabbricati_Codici_W
        Dim objContatti_Codici As AgronicaCoreAnagrafeDAL.Contatti_Codici_W
        Dim objCatasto_Codici As AgronicaCoreAnagrafeDAL.ImpresexParticelle_Codici_W
        Dim objPC As AgronicaCoreAnagrafeDAL.Programmazione_Entita_Codici_W

        '------------------------------
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim xRisp As Boolean = False
        '------------------------------

        Try

            If keyDT IsNot Nothing Then

                If keyDT.Chiave IsNot Nothing AndAlso keyDT.DT_Modifiche IsNot Nothing Then

                    If keyDT.DT_Modifiche.Rows.Count > 0 Then

                        vBit = New BitArray(32)

                        For i = 0 To keyDT.DT_Modifiche.Rows.Count - 1

                            dr = keyDT.DT_Modifiche.Rows(i)

                            If dr.stato = False Then
                                If dr.old_value <> dr.new_value Then
                                    dr.stato = True
                                End If
                            End If

                            vBit(i) = dr.stato

                        Next

                        'Trasformo la maschera di bit in un intero
                        vBit.CopyTo(vStato, 0)

                        'Converto in stringa il numero intero ottenuto convertendo la maschera di bit
                        sStato = CStr(vStato(0))

                        '------------------------------

                        'Se la connessione è chiusa la apro
                        If objConnessione Is Nothing Then

                            'Richiedo una connessione
                            objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(StringaConnessione)
                            objConnessione.Open()

                            FlagConnessioneLocale = True

                        End If

                        If objTransazione Is Nothing Then

                            'Inizializzo la transazione
                            objTransazione = objConnessione.BeginTransaction

                            FlagTransazioneLocale = True

                        End If

                        Select Case keyDT.Chiave.Tabella_Nome

                            Case DatiAnagrafici_Azienda

                                '====================================================================
                                '-------------------------- AZIENDA ---------------------------------
                                '====================================================================

                                objImpresa_Codici = New AgronicaCoreAnagrafeDAL.Imprese_Codici_Write

                                objImpresa_Codici.Cancella(keyDT.Chiave.Piva,
                                                           enum_DatiAnagrafici_CodiciAnagrafe.Azienda,
                                                           "", objParametri)

                                objImpresa_Codici.Scrivi(keyDT.Chiave.Piva,
                                                         enum_DatiAnagrafici_CodiciAnagrafe.Azienda,
                                                         sStato,
                                                         FinestraTemp_Inizio,
                                                         FinestraTemp_Fine,
                                                         objParametri)

                            Case DatiAnagrafici_Macchine

                                '====================================================================
                                '-------------------------- MACCHINE --------------------------------
                                '====================================================================

                                objMacchine_Codici = New AgronicaCoreContabDAL.Parco_Macchine_Codici_W

                                objMacchine_Codici.Cancella(keyDT.Chiave.Piva,
                                                            keyDT.Chiave.Sa_Cod,
                                                            keyDT.Chiave.Mac_Cod,
                                                            enum_DatiAnagrafici_CodiciAnagrafe.Macchine,
                                                            "", objParametri)

                                objMacchine_Codici.Scrivi(keyDT.Chiave.Piva,
                                                          keyDT.Chiave.Sa_Cod,
                                                          keyDT.Chiave.Mac_Cod,
                                                          enum_DatiAnagrafici_CodiciAnagrafe.Macchine,
                                                          sStato,
                                                          FinestraTemp_Inizio,
                                                          FinestraTemp_Fine,
                                                          objParametri)

                            Case DatiAnagrafici_Fabbricati

                                '====================================================================
                                '-------------------------- FABBRICATI ------------------------------
                                '====================================================================

                                objFabb_Codici = New AgronicaCoreAnagrafeDAL.Fabbricati_Codici_W

                                objFabb_Codici.Cancella(keyDT.Chiave.Piva,
                                                        keyDT.Chiave.Sa_Cod,
                                                        keyDT.Chiave.Fabbricato_Cod,
                                                        enum_DatiAnagrafici_CodiciAnagrafe.Fabbricati,
                                                        "", objParametri)

                                objFabb_Codici.Scrivi(keyDT.Chiave.Piva,
                                                      keyDT.Chiave.Sa_Cod,
                                                      keyDT.Chiave.Fabbricato_Cod,
                                                      enum_DatiAnagrafici_CodiciAnagrafe.Fabbricati,
                                                      FinestraTemp_Inizio,
                                                      FinestraTemp_Fine,
                                                      sStato,
                                                      objParametri)

                            Case DatiAnagrafici_Allevamenti

                                '====================================================================
                                '-------------------------- ALLEVAMENTI ------------------------------
                                '====================================================================

                                objFabb_Codici = New AgronicaCoreAnagrafeDAL.Fabbricati_Codici_W

                                objFabb_Codici.Cancella(keyDT.Chiave.Piva,
                                                        keyDT.Chiave.Sa_Cod,
                                                        keyDT.Chiave.Fabbricato_Cod,
                                                        enum_DatiAnagrafici_CodiciAnagrafe.Allevamenti,
                                                        "", objParametri)

                                objFabb_Codici.Scrivi(keyDT.Chiave.Piva,
                                                      keyDT.Chiave.Sa_Cod,
                                                      keyDT.Chiave.Fabbricato_Cod,
                                                      enum_DatiAnagrafici_CodiciAnagrafe.Allevamenti,
                                                      FinestraTemp_Inizio,
                                                      FinestraTemp_Fine,
                                                      sStato,
                                                      objParametri)

                            Case DatiAnagrafici_Contatti

                                '====================================================================
                                '-------------------------- RAPPR. LEGALE ---------------------------
                                '====================================================================

                                objContatti_Codici = New AgronicaCoreAnagrafeDAL.Contatti_Codici_W

                                objContatti_Codici.Cancella(keyDT.Chiave.Piva,
                                                            keyDT.Chiave.Cod_Contatto,
                                                            enum_DatiAnagrafici_CodiciAnagrafe.Rappr_Legale,
                                                            "", objParametri)

                                objContatti_Codici.Scrivi(keyDT.Chiave.Piva,
                                                          keyDT.Chiave.Sa_Cod,
                                                          keyDT.Chiave.Cod_Contatto,
                                                          enum_DatiAnagrafici_CodiciAnagrafe.Rappr_Legale,
                                                          sStato,
                                                          FinestraTemp_Inizio,
                                                          FinestraTemp_Fine,
                                                          objParametri)

                            Case DatiAnagrafici_Catasto

                                '====================================================================
                                '----------------------------- CATASTO ------------------------------
                                '====================================================================

                                objCatasto_Codici = New AgronicaCoreAnagrafeDAL.ImpresexParticelle_Codici_W

                                objCatasto_Codici.Cancella(keyDT.Chiave.ID,
                                                           enum_DatiAnagrafici_CodiciAnagrafe.Catasto,
                                                           "", objParametri)

                                objCatasto_Codici.Scrivi(keyDT.Chiave.ID,
                                                         enum_DatiAnagrafici_CodiciAnagrafe.Catasto,
                                                         sStato,
                                                         FinestraTemp_Inizio,
                                                         FinestraTemp_Fine,
                                                         objParametri)

                            Case DatiAnagrafici_PianoColturale

                                '====================================================================
                                '-------------------------- PIANO COLTURALE -------------------------
                                '====================================================================

                                objPC = New AgronicaCoreAnagrafeDAL.Programmazione_Entita_Codici_W

                                objPC.Cancella(keyDT.Chiave.Programmazione_Entita_Cod,
                                               enum_DatiAnagrafici_CodiciAnagrafe.PianoColturale,
                                               "", objParametri)

                                objPC.Scrivi(keyDT.Chiave.Programmazione_Entita_Cod,
                                             enum_DatiAnagrafici_CodiciAnagrafe.PianoColturale,
                                             sStato,
                                             FinestraTemp_Inizio,
                                             FinestraTemp_Fine,
                                             objParametri)

                        End Select

                        'Restituisco un valore Dummy
                        xRisp = True

                        'Se ho la transazione è stata avviata in questa routine faccio il commit
                        If FlagTransazioneLocale Then
                            objTransazione.Commit()
                        End If

                    End If 'FINE: If keyDT.DT_Modifiche.Rows.Count > 0 Then


                End If 'FINE: If (keyDT.Chiave IsNot Nothing) AndAlso (keyDT.DT_Modifiche IsNot Nothing) Then

            End If 'FINE: If (Not keyDT Is Nothing) Then


        Catch ex As Exception

            MessaggioErrore = ex.Message

            'Restituisco un valore Dummy
            xRisp = False

            'Faccio il rollback della transazione
            If objTransazione IsNot Nothing Then
                objTransazione.Rollback()
            End If

            'Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, nomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)

        Finally

            'Chiudo la connessione se è stata aperta in questa routine
            If FlagConnessioneLocale AndAlso objConnessione IsNot Nothing Then
                objConnessione.Close()
            End If

        End Try

    End Function


    'Dim nb As BitConverter
    'Dim b As BitArray

    'b = New BitArray(3)

    'b(0) = True
    'b(1) = True
    'b(2) = False

    'Dim iB(0) As Integer

    'b.CopyTo(iB, 0)

    'Dim k As Integer

    'k = iB(0)

    'iB(0) = 2

    'b = New BitArray(iB)

End Class
