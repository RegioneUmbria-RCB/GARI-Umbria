Imports System.Xml
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports Newtonsoft.Json

Public Class Prodotti_Costi_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '============================================================================
    Public Function Prodotti_Costi_Scrivi(ByVal DatiProdotti_Costi As String,
                                          ByVal Mat_Cod As Integer,
                                          ByRef objParametri As AgronicaCoreParametri
                                          ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabBIZ.Prodotti_Costi_W.Prodotti_Costi_Scrivi()"

        Dim ObjProdotti_Costi As AgronicaCoreContabDAL.Prodotti_Costi_W

        Dim Dummy As Boolean
        Dim XmlDoc As XmlDocument

        Dim xDatiProdotti_Costis As XmlNodeList
        Dim xDatiProdotti_Costi As XmlElement
        Dim xProdotti_Costis As XmlNodeList
        Dim xProdotti_Costi As XmlElement

        Dim i_DatiProdotti_Costi As Integer
        Dim i_Prodotti_Costi As Integer

        Dim OpeDB_Prodotti_Costi As String

        '------------------------------
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim messaggioErrore As String = ""
        Dim xRisp As Boolean = True
        '------------------------------

        Try

            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri)

            ''Se la connessione è chiusa la apro
            'If objParametri.objConnessione Is Nothing Then
            '    'Richiedo una connessione
            '    objParametri.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
            '    objParametri.objConnessione.Open()
            '    FlagConnessioneLocale = True
            'End If

            'If objParametri.objTransazione Is Nothing Then
            '    'Inizializzo la transazione
            '    objParametri.objTransazione = objParametri.objConnessione.BeginTransaction
            '    FlagTransazioneLocale = True
            'End If

            '------------------------------

            XmlDoc = New Xml.XmlDocument
            'XmlDoc.async = False
            XmlDoc.LoadXml(DatiProdotti_Costi)

            '------------------------------

            xDatiProdotti_Costis = XmlDoc.GetElementsByTagName("DatiProdotti_Costi")

            i_DatiProdotti_Costi = 0

            Do While i_DatiProdotti_Costi < xDatiProdotti_Costis.Count

                'Prelevo l'i-esimo blocco di Dati Liquidità
                xDatiProdotti_Costi = xDatiProdotti_Costis.Item(i_DatiProdotti_Costi)

                '------------------------------

                xProdotti_Costis = xDatiProdotti_Costi.GetElementsByTagName("Prodotto_Costo")

                i_Prodotti_Costi = 0

                Do While i_Prodotti_Costi < xProdotti_Costis.Count

                    'Prelevo l' i-esima Liquidità/Risorsa Finanziaria
                    xProdotti_Costi = xProdotti_Costis.Item(i_Prodotti_Costi)

                    'Prelevo gli attributi del rapporto selezionato
                    OpeDB_Prodotti_Costi = xProdotti_Costi.GetAttribute("TipoOperazioneDB")

                    'Creo l'oggetto COM
                    ObjProdotti_Costi = New AgronicaCoreContabDAL.Prodotti_Costi_W

                    'Verifico l'operazione richiesta
                    Select Case OpeDB_Prodotti_Costi
                        '
                        Case "0"      'LEGGI -------------------------------------------------------
                            '
                        Case "1", "2" 'SALVA MODIFICA -------------------------------------------------------

                            'La prima volta cancello il vecchio storico prezzi

                            If i_Prodotti_Costi = 0 And (CInt(xProdotti_Costi.GetAttribute("pro_cod")) <> 0 Or CInt(xProdotti_Costi.GetAttribute("mat_cod")) <> 0) Then

                                'Cancellazione Vecchi Valori
                                ObjProdotti_Costi.Cancella(
                                            CStr(xProdotti_Costi.GetAttribute("piva")),
                                            CStr(xProdotti_Costi.GetAttribute("riferimento")),
                                            CInt(xProdotti_Costi.GetAttribute("elem_cod")),
                                            CInt(xProdotti_Costi.GetAttribute("pro_cod")),
                                            CInt(xProdotti_Costi.GetAttribute("mat_cod")),
                                            0,
                                            0,
                                            "",
                                            objParametri)

                            End If

                            Dummy = ObjProdotti_Costi.Scrivi(
                                            CStr(xProdotti_Costi.GetAttribute("piva")),
                                            CStr(xProdotti_Costi.GetAttribute("riferimento")),
                                            CInt(xProdotti_Costi.GetAttribute("elem_cod")),
                                            CInt(xProdotti_Costi.GetAttribute("pro_cod")),
                                            Mat_Cod,
                                            CInt(xProdotti_Costi.GetAttribute("udm_cod")),
                                            CInt(xProdotti_Costi.GetAttribute("mezzo")),
                                            CDbl(xProdotti_Costi.GetAttribute("prezzo_unitario")),
                                            CInt(xProdotti_Costi.GetAttribute("veg_cod")),
                                            CInt(xProdotti_Costi.GetAttribute("cul_cod")),
                                                CDate(xProdotti_Costi.GetAttribute("validita_inizio")),
                                                CDate(xProdotti_Costi.GetAttribute("validita_fine")),
                                                objParametri)

                        Case "3"   'CANCELLAZIONE

                            Dummy = ObjProdotti_Costi.Cancella("", "",
                                                      CInt(xProdotti_Costi.GetAttribute("elem_cod")),
                                                      CInt(xProdotti_Costi.GetAttribute("pro_cod")),
                                                      CInt(xProdotti_Costi.GetAttribute("mat_cod")),
                                                      0,
                                                      0,
                                                      "",
                                                      objParametri)

                    End Select

                    'Elimino l'oggetto
                    ObjProdotti_Costi = Nothing

                    '-------------------------------------------------------------

                    'Incremento l'indice
                    i_Prodotti_Costi += 1

                Loop

                '------------------------------

                'Incremento l'indice
                i_DatiProdotti_Costi += 1

            Loop

            '------------------------------
            '------------------------------
            '------------------------------

            'Elimino tutti gli oggetti utilizzati

            xProdotti_Costi = Nothing
            xProdotti_Costis = Nothing
            xDatiProdotti_Costi = Nothing
            xDatiProdotti_Costis = Nothing
            XmlDoc = Nothing

            '------------------------------

            'Restituisco un valore Dummy
            xRisp = True

            'Se ho la transazione è stata avviata in questa routine faccio il commit
            If FlagTransazioneLocale = True Then
                objParametri.objTransazione.Commit()
            End If

            '----------------------------------------------------------------------------

        Catch ex As Exception

            'Restituisco un valore Dummy
            xRisp = False

            'Faccio il rollback della transazione
            If objParametri.objTransazione IsNot Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            messaggioErrore = ex.Message
            '//////////////////////////////////////////////////////////////////////


            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        Finally
            'Chiudo la connessione se è stata aperta in questa routine
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)
        End Try

        Return xRisp

    End Function

    Public Function Prodotti_Costi_Completa_Scrivi(ByVal DatiProdotti_Costi As String,
                                                   ByVal Mat_Cod As Integer,
                                                   ByRef objParametri As AgronicaCoreParametri
                                                   ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabBIZ.Prodotti_Costi_W.Prodotti_Costi_Scrivi()"

        Dim ObjProdotti_Costi As AgronicaCoreContabDAL.Prodotti_Costi_W

        Dim Dummy As Boolean
        Dim XmlDoc As XmlDocument

        Dim xDatiProdotti_Costis As XmlNodeList
        Dim xDatiProdotti_Costi As XmlElement
        Dim xProdotti_Costis As XmlNodeList
        Dim xProdotti_Costi As XmlElement

        Dim i_DatiProdotti_Costi As Integer
        Dim i_Prodotti_Costi As Integer

        Dim OpeDB_Prodotti_Costi As String

        '------------------------------
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim messaggioErrore As String = ""
        Dim xRisp As Boolean = True
        '------------------------------

        Try

            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri)


            XmlDoc = New Xml.XmlDocument
            'XmlDoc.async = False
            XmlDoc.LoadXml(DatiProdotti_Costi)

            '------------------------------

            xDatiProdotti_Costis = XmlDoc.GetElementsByTagName("DatiProdotti_Costi")

            i_DatiProdotti_Costi = 0

            Do While i_DatiProdotti_Costi < xDatiProdotti_Costis.Count

                'Prelevo l'i-esimo blocco di Dati Liquidità
                xDatiProdotti_Costi = xDatiProdotti_Costis.Item(i_DatiProdotti_Costi)

                '------------------------------

                xProdotti_Costis = xDatiProdotti_Costi.GetElementsByTagName("Prodotto_Costo")

                i_Prodotti_Costi = 0

                Do While i_Prodotti_Costi < xProdotti_Costis.Count

                    'Prelevo l' i-esima Liquidità/Risorsa Finanziaria
                    xProdotti_Costi = xProdotti_Costis.Item(i_Prodotti_Costi)

                    'Prelevo gli attributi del rapporto selezionato
                    OpeDB_Prodotti_Costi = xProdotti_Costi.GetAttribute("TipoOperazioneDB")

                    'Creo l'oggetto COM
                    ObjProdotti_Costi = New AgronicaCoreContabDAL.Prodotti_Costi_W

                    'Verifico l'operazione richiesta
                    Select Case OpeDB_Prodotti_Costi


                        Case "1", "2" ' MODIFICA SALVA ----------------------------------------------------------
                            Dummy = ObjProdotti_Costi.Scrivi_Completa(
                                CInt(xProdotti_Costi.GetAttribute("id")),
                                CStr(xProdotti_Costi.GetAttribute("piva")),
                                CStr(xProdotti_Costi.GetAttribute("riferimento")),
                                CInt(xProdotti_Costi.GetAttribute("elem_cod")),
                                CInt(xProdotti_Costi.GetAttribute("pro_cod")),
                                Mat_Cod,
                                CInt(xProdotti_Costi.GetAttribute("udm_cod")),
                                CInt(xProdotti_Costi.GetAttribute("mezzo")),
                                CDbl(xProdotti_Costi.GetAttribute("prezzo_unitario")),
                                CInt(xProdotti_Costi.GetAttribute("veg_cod")),
                                CInt(xProdotti_Costi.GetAttribute("cul_cod")),
                                CDate(xProdotti_Costi.GetAttribute("validita_inizio")),
                                CDate(xProdotti_Costi.GetAttribute("validita_fine")),
                                CInt(OpeDB_Prodotti_Costi),
                                objParametri)

                        Case "3"   'CANCELLAZIONE
                            Dummy = ObjProdotti_Costi.Cancella_da_ID(CInt(xProdotti_Costi.GetAttribute("id")),
                                                                     "", objParametri)
                    End Select

                    'Elimino l'oggetto
                    ObjProdotti_Costi = Nothing

                    '-------------------------------------------------------------

                    'Incremento l'indice
                    i_Prodotti_Costi += 1

                Loop

                '------------------------------

                'Incremento l'indice
                i_DatiProdotti_Costi += 1

            Loop

            '------------------------------
            '------------------------------
            '------------------------------

            'Elimino tutti gli oggetti utilizzati

            xProdotti_Costi = Nothing
            xProdotti_Costis = Nothing
            xDatiProdotti_Costi = Nothing
            xDatiProdotti_Costis = Nothing
            XmlDoc = Nothing

            '------------------------------

            'Restituisco un valore Dummy
            xRisp = True

            'Se ho la transazione è stata avviata in questa routine faccio il commit
            If FlagTransazioneLocale = True Then
                objParametri.objTransazione.Commit()
            End If

            '----------------------------------------------------------------------------

        Catch ex As Exception

            'Restituisco un valore Dummy
            xRisp = False

            'Faccio il rollback della transazione
            If objParametri.objTransazione IsNot Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            messaggioErrore = ex.Message
            '//////////////////////////////////////////////////////////////////////


            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        Finally
            'Chiudo la connessione se è stata aperta in questa routine
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)
        End Try

        Return xRisp

    End Function

    '============================================================================

    Public Shared Function Controlli_SalvaProdottiCosti(ByVal righeInseriteStr As String,
                                                        ByVal righeModificateStr As String,
                                                        ByVal righeEliminateStr As String,
                                                        ByVal tutteLeRigheStr As String,
                                                        ByRef errori As String
                                                        ) As Boolean

        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim righeInserite As List(Of ProdottiCostiModel) = JsonConvert.DeserializeObject(Of List(Of ProdottiCostiModel))(righeInseriteStr, settingLoc)
        Dim righeModificate As List(Of ProdottiCostiModel) = JsonConvert.DeserializeObject(Of List(Of ProdottiCostiModel))(righeModificateStr, settingLoc)
        Dim righeEliminate As List(Of ProdottiCostiModel) = JsonConvert.DeserializeObject(Of List(Of ProdottiCostiModel))(righeEliminateStr, settingLoc)
        Dim tutteLeRighe As List(Of ProdottiCostiModel) = JsonConvert.DeserializeObject(Of List(Of ProdottiCostiModel))(tutteLeRigheStr, settingLoc)
        Dim count As Integer = 0

        If righeInserite IsNot Nothing Then
            'Controllo che non ci siano righe inserite con validita sovrapposte che hanno diversi prezzi ma stesse unita di misura
            For Each ins As ProdottiCostiModel In righeInserite
                Dim unitadimisura_ins = ins.Udm_Cod
                Dim unitadimisurades_ins = ins.Udm_Des
                Dim validitaInizio = ins.Validita_Inizio
                Dim validitaFine = ins.Validita_Fine
                Dim prezzo = ins.Prezzo_Unitario

                If unitadimisura_ins = -1 OrElse unitadimisura_ins = 0 Then
                    'i18N Inserire una Unità di Misura corretta.
                    errori += "Inserire una Unità di Misura corretta."
                    Return False
                End If

                If CDec(prezzo) <= 0 Then
                    'i18N Inserire un prezzo corretto.
                    errori += "Inserire un prezzo corretto."
                    Return False
                End If
                
                If validitaInizio < validitaFine Then
                    Dim esistenti = tutteLeRighe.Where(Function(f) f.Udm_Cod = unitadimisura_ins AndAlso
                                   ((f.Validita_Fine >= validitaInizio) AndAlso
                                    (f.Validita_Inizio <= validitaFine)))

                    If esistenti.Count > 1 Then
                        'i18N Ci sono delle righe inserite con  Date sovrapposte da tradurre
                        errori += "Ci sono delle righe inserite con Date sovrapposte " + validitaInizio + " - " + validitaFine + " che hanno un diverso Prezzo ma la stessa Unità di Misura: " & unitadimisurades_ins
                        Return False
                    End If
                Else
                    'i18N Ci sono delle righe inserite con  validità di inizio
                    errori += "Ci sono delle righe inserite con validità di inizio (" + validitaInizio + ") maggiore di quella finale (" + validitaFine + ")"
                    Return False
                End If

            Next
        End If


        If righeModificate IsNot Nothing Then
            'Controllo che non ci siano righe modificate con validita sovrapposte che hanno diversi prezzi ma stesse unita di misura
            For Each modi As ProdottiCostiModel In righeModificate
                Dim unitadimisura_modi = modi.Udm_Cod
                Dim unitadimisurades_modi = modi.Udm_Des
                Dim validitaInizio = modi.Validita_Inizio
                Dim validitaFine = modi.Validita_Fine
                Dim prezzo = modi.Prezzo_Unitario

                If unitadimisura_modi = -1 OrElse unitadimisura_modi = 0 Then
                    'i18N Inserire una Unità di Misura corretta.
                    errori += "Inserire una Unità di Misura corretta."
                    Return False
                End If

                If CDec(prezzo) <= 0 Then
                    'i18N Inserire un prezzo corretto.
                    errori += "Inserire un prezzo corretto."
                    Return False
                End If
                
                If validitaInizio < validitaFine Then
                    Dim esistenti = tutteLeRighe.Where(Function(f) f.Udm_Cod = unitadimisura_modi AndAlso
                                                       ((f.Validita_Fine >= validitaInizio) AndAlso
                                                        (f.Validita_Inizio <= validitaFine)))

                    If esistenti.Count > 1 Then
                        'i18N Ci sono delle righe modificate con  Date sovrapposte da tradurre
                        errori += "Ci sono delle righe modificate con Date sovrapposte " + validitaInizio + " - " + validitaFine + " che hanno un diverso Prezzo ma la stessa Unità di Misura: " & unitadimisura_modi
                        Return False
                    End If
                Else
                    'i18N Ci sono delle righe modificate con  validità di inizio
                    errori += "Ci sono delle righe modificate con validità di inizio (" + validitaInizio + ") maggiore di quella finale (" + validitaFine + ")"
                    Return False
                End If

            Next
        End If

        Return True

    End Function

    ''' <summary>
    ''' Il chiamante si deve occupare di gestire il tutto in una transazione
    ''' </summary>
    Public Shared Function SalvaStoricoPrezzi(ByVal piva As String,
                                              ByVal Elem_Cod As Integer,
                                              ByVal Mat_Cod As Integer,
                                              ByVal Pro_Cod As Integer,
                                              ByVal righeInseriteStr As String,
                                              ByVal righeModificateStr As String,
                                              ByVal righeEliminateStr As String,
                                              ByVal tutteLeRigheStr As String,
                                              ByRef errori As String,
                                              ByRef objParametriServer As AgronicaCoreParametri,
                                              Optional ByVal Id_Budget As Integer = 0
                                              ) As Boolean

        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim obj As New AgronicaCoreContabDAL.Prodotti_Costi_W

        Try

            Dim righeInserite As List(Of ProdottiCostiModel) = JsonConvert.DeserializeObject(Of List(Of ProdottiCostiModel))(righeInseriteStr, settingLoc)

            If righeInserite IsNot Nothing AndAlso righeInserite.Any Then
                Dim Scrittura_OK As Boolean = False

                For Each p As ProdottiCostiModel In righeInserite

                    SistemaCodiciByCategory(p, Elem_Cod, Pro_Cod, Mat_Cod, Id_Budget)

                    Scrittura_OK = obj.Scrivi(piva,
                                              "",
                                              p.Elem_Cod,
                                              p.Pro_Cod,
                                              p.Mat_Cod,
                                              p.Udm_Cod,
                                              p.Mezzo,
                                              If(p.Prezzo_Unitario, 0D),
                                              0,
                                              0,
                                              p.Validita_Inizio,
                                              p.Validita_Fine,
                                              objParametriServer,
                                              Id_Budget:=If(p.Id_Budget, 0))

                    If (Scrittura_OK = False) Then
                        Return False
                    End If

                Next
            End If

            Dim righeModificate As List(Of ProdottiCostiModel) = JsonConvert.DeserializeObject(Of List(Of ProdottiCostiModel))(righeModificateStr, settingLoc)
            
            If righeModificate IsNot Nothing AndAlso righeModificate.Any Then
                Dim Modifica_OK As Boolean = False

                For Each p As ProdottiCostiModel In righeModificate

                    SistemaCodiciByCategory(p, Elem_Cod, Pro_Cod, Mat_Cod, Id_Budget)

                    Modifica_OK = obj.Scrivi_Completa(p.ID,
                                                      piva,
                                                      p.Riferimento,
                                                      p.Elem_Cod,
                                                      p.Pro_Cod,
                                                      p.Mat_Cod,
                                                      p.Udm_Cod,
                                                      p.Mezzo,
                                                      If(p.Prezzo_Unitario, 0D),
                                                      0,
                                                      0,
                                                      p.Validita_Inizio,
                                                      p.Validita_Fine,
                                                      enum_TipoOperazioneDB.Modifica,
                                                      objParametriServer,
                                                      Id_Budget:=If(p.Id_Budget, 0),
                                                      Data_creazione:=p.Data_Creazione,
                                                      username_creazione:=p.Username_Creazione)

                    If (Modifica_OK = False) Then
                        Return False
                    End If
                Next
            End If

            Dim righeEliminate As List(Of ProdottiCostiModel) = JsonConvert.DeserializeObject(Of List(Of ProdottiCostiModel))(righeEliminateStr, settingLoc)
            
            If righeEliminate IsNot Nothing AndAlso righeEliminate.Any Then
                Dim Cancella_OK As Boolean = False
                
                For Each p As ProdottiCostiModel In righeEliminate
                    Cancella_OK = obj.Cancella_da_ID(p.ID, "", objParametriServer)
                    If (Cancella_OK = False) Then
                        Return False
                    End If
                Next
            End If

        Catch ex As Exception
            'i18N Errore durante l'operazione da tradurre
            errori = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return False
        End Try

        Return True

    End Function

    Private Shared Sub SistemaCodiciByCategory(ByRef p As ProdottiCostiModel,
                                               ByVal Elem_Cod As Integer,
                                               ByVal Pro_Cod As Integer,
                                               ByVal Mat_Cod As Integer,
                                               ByVal Id_Budget As Integer)

        'Se l'id_budget mi è arrivato come campo separato, devo valorizzare la proprietà
        If p.Id_Budget Is Nothing Then
            p.Id_Budget = Id_Budget
        End If

        If p.Elem_Cod Is Nothing Then
            'Arrivo dall'anagrafica dove non ho i dati in griglia ma fuori
            p.Elem_Cod = Elem_Cod
            p.Pro_Cod = Pro_Cod
            p.Mat_Cod = Mat_Cod
            p.Mezzo = -1

        ElseIf p.Elem_Cod = MACCHINE Then
            'Se l'udm mi è arrivato sul parametro udm, lo sposto su Mezzo
            If (p.Mezzo Is Nothing OrElse p.Mezzo = -1) AndAlso p.Udm_Cod > 0 Then
                p.Mezzo = p.Udm_Cod
                p.Udm_Cod = 0
            End If

            'Se l'identificativo della macchina mi è arrivato su Mac_Cod, lo metto su Mat_cod (dove deve essere scritto)
            If (p.Mat_Cod Is Nothing OrElse p.Mat_Cod = 0) AndAlso p.Mac_Cod > 0 Then
                p.Mat_Cod = p.Mac_Cod
            End If

            p.Pro_Cod = 0

        ElseIf p.Elem_Cod = 0 Then
            'Persona
            'udm in mezzo?!?
        End If
    End Sub

End Class

Public Class ProdottiCostiModel
    Public ID As Integer? = Nothing
    Public Piva As String = ""
    Public Riferimento As String = ""
    Public Elem_Cod As Integer? = Nothing
    Public Pro_Cod As Integer? = Nothing
    Public Mat_Cod As Integer? = Nothing
    Public Udm_Cod As Integer? = Nothing
    Public Udm_Des As String = ""
    Public Mezzo As Integer? = -1
    Public Prezzo_Unitario As Decimal? = Nothing
    'Veg_Cod
    'Cul_Cod
    Public Validita_Inizio As Date = AGRODATAINIZIO
    Public Validita_Fine As Date = AGRODATAFINE
    Public Id_Budget As Integer? = 0
    Public Username_Creazione As String = ""
    Public Data_Creazione As DateTime = Date.Now

    Public Mac_Cod As Integer? = Nothing
End Class


'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
