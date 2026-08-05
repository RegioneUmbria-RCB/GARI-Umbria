Imports System.Xml
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class Pagamento_R
    Inherits AgronicaCoreDataProvider.LogProvider

    '============================================================================
    Public Function Pagamento_Leggi(ByVal Piva As String,
                                    ByVal Sa_Cod As Integer,
                                    ByVal Id_Agenda As Integer,
                                    ByVal Id_Mov As Integer,
                                    ByVal Cod_Pagamento As Integer,
                                    ByVal ForDelete As Boolean,
                                    ByRef objParametri As AgronicaCoreParametri
                                    ) As String

        Const nomeRoutine = "ContabBIZ.Pagamento_R.Pagamento_Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Sa_Cod = 0
        '   Id_Agenda = 0
        '   Id_Mov=0
        '   Cod_Pagamento = 0
        '====================================================================================

        Dim messaggioErrore As String = ""

        Dim flagConnessioneLocale As Boolean = False


        Dim i As Integer

        Dim RisultatoFunzione As String = String.Empty

        Dim XmlDoc As XmlDocument
        Dim XmlDatiPagamento As XmlElement
        Dim XmlPagamento As XmlElement

        Dim ObjPagamento As AgronicaCoreContabDAL.Pagamenti_R

        Dim DtPagamento As DataTable

        Try
            '------------------------------
            'Verifico se e' stata impostata una connessione
            If IsNothing(objParametri.objConnessione) Then
                'Flag
                flagConnessioneLocale = True
                'Creo la connessione localmente
                objParametri.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
            End If

            '------------------------------

            'Mi procuro un elenco degli istituti di credito associati all'Impresa
            'all'interno della finestra temporale selezionata

            'Creo l'oggetto COM+
            ObjPagamento = New AgronicaCoreContabDAL.Pagamenti_R

            'Mi procuro il datatable richiesto
            DtPagamento = ObjPagamento.Leggi(CStr(Piva),
                                             CInt(Sa_Cod),
                                             CInt(Id_Agenda),
                                             CInt(Id_Mov),
                                             CInt(Cod_Pagamento),
                                             enumSelezioneVariabile.Selezione_TabellaCompleta,
                                             "",
                                             "",
                                             objParametri)

            'Se ottengo almeno un risultato, creo la struttura XML
            If DtPagamento.Rows.Count > 0 Then

                '----- < Documento XML > -----
                XmlDoc = New XmlDocument

                XmlDatiPagamento = XmlDoc.CreateElement("DatiPagamenti")


                'Effettuo un ciclo sugli Istituti di Credito
                For i = 0 To DtPagamento.Rows.Count - 1


                    '----- < PAGAMENTO > -----
                    XmlPagamento = XmlDoc.CreateElement("Pagamento")

                    With XmlPagamento
                        .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                        .SetAttribute("piva", Agro_SQL_Load(DtPagamento.Rows(i).Item("PIVA")))
                        .SetAttribute("sa_cod", Agro_SQL_Load(DtPagamento.Rows(i).Item("Sa_cod")))
                        .SetAttribute("id_agenda", Agro_SQL_Load(DtPagamento.Rows(i).Item("Id_Agenda")))
                        .SetAttribute("id_mov", Agro_SQL_Load(DtPagamento.Rows(i).Item("Id_Mov")))
                        .SetAttribute("cod_pagamento", Agro_SQL_Load(DtPagamento.Rows(i).Item("Cod_Pagamento")))
                        .SetAttribute("importo", Agro_SQL_Load(DtPagamento.Rows(i).Item("Importo")))
                        .SetAttribute("percentuale", Agro_SQL_Load(DtPagamento.Rows(i).Item("Percentuale")))
                        .SetAttribute("data_pagamento", Agro_SQL_Load(DtPagamento.Rows(i).Item("Data_Pagamento")))
                        .SetAttribute("note", Agro_SQL_Load(DtPagamento.Rows(i).Item("Note")))
                        .SetAttribute("cod_liquidita_dare", Agro_SQL_Load(DtPagamento.Rows(i).Item("Cod_Liquidita_Dare")))
                        .SetAttribute("cod_liquidita_avere", Agro_SQL_Load(DtPagamento.Rows(i).Item("Cod_Liquidita_Avere")))
                        .SetAttribute("cau_risorsa", Agro_SQL_Load(DtPagamento.Rows(i).Item("Cau_Risorsa")))
                        .SetAttribute("cau_pagamento", Agro_SQL_Load(DtPagamento.Rows(i).Item("Cau_Pagamento")))
                        .SetAttribute("lav_cod", Agro_SQL_Load(DtPagamento.Rows(i).Item("Lav_Cod")))
                        .SetAttribute("des_lib", Agro_SQL_Load(DtPagamento.Rows(i).Item("Des_Lib")))
                        .SetAttribute("cod_risum", Agro_SQL_Load(DtPagamento.Rows(i).Item("Cod_RisUm")))
                        .SetAttribute("cau_mov", Agro_SQL_Load(DtPagamento.Rows(i).Item("Cau_Mov")))
                        .SetAttribute("scadenza", Agro_SQL_Load(DtPagamento.Rows(i).Item("Scadenza")))
                        .SetAttribute("data_movimento", Agro_SQL_Load(DtPagamento.Rows(i).Item("Data_Movimento")))
                        .SetAttribute("num_protocollo", Agro_SQL_Load(DtPagamento.Rows(i).Item("Num_Protocollo")))
                        .SetAttribute("extra_str", Agro_SQL_Load(DtPagamento.Rows(i).Item("Extra_Str")))
                        .SetAttribute("extra_int", Agro_SQL_Load(DtPagamento.Rows(i).Item("Extra_Int")))
                        .SetAttribute("extra_date", Agro_SQL_Load(DtPagamento.Rows(i).Item("Extra_Date")))
                        .SetAttribute("validita_inizio", Agro_SQL_Load(DtPagamento.Rows(i).Item("Validita_Inizio")))
                        .SetAttribute("validita_fine", Agro_SQL_Load(DtPagamento.Rows(i).Item("Validita_Fine")))
                    End With

                    XmlDatiPagamento.AppendChild(XmlPagamento)
                    '----- < / PAGAMENTO > -----

                Next

                XmlDoc.AppendChild(XmlDatiPagamento)

                RisultatoFunzione = XmlDoc.OuterXml
                '----- < / Documento XML > -----


                XmlPagamento = Nothing
                XmlDatiPagamento = Nothing
                XmlDoc = Nothing

            Else

                'Altrimenti, se non risulta selezionato nessun pagamento ...
                RisultatoFunzione = ""

            End If

            'Elimino gli oggetti che ho creato
            DtPagamento.Dispose()
            DtPagamento = Nothing
            ObjPagamento = Nothing

            '------------------------------


        Catch ex As Exception
            RisultatoFunzione = ""
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        Finally

            'Pulizia
            'objUtentixImprese = Nothing
            'objImpresexCodici = Nothing
            'objImpresexIndirizzi = Nothing

            If flagConnessioneLocale = True Then
                objParametri.objConnessione.Close()
                objParametri.objConnessione.Dispose()
            End If

        End Try

        Return RisultatoFunzione

    End Function

    '####################################################
    Public Sub Verifica_CASSA(ByRef objParametri As AgronicaCoreParametri)

        Const nomeRoutine = "AgronicaCoreContabBIZ.Pagamento_R.Verifica_CASSA()"

        Dim flag_esiste As Boolean = False
        Dim insert As Boolean = False
        Dim messaggioErrore As String = ""

        Dim objIst_R As New AgronicaCoreContabDAL.Ist_Credito_R
        Dim objIst_W As New AgronicaCoreContabDAL.Ist_Credito_W

        Try

            flag_esiste = objIst_R.Esiste_CASSA(objParametri)

            If flag_esiste = False Then
                insert = objIst_W.Inserisci_CASSA(objParametri)

                Scrivi_LOG(objParametri, nomeRoutine, "Inserito record CASSA")
            Else
                insert = objIst_W.Modifica_CASSA(objParametri)
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub


End Class

'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§


Public Class Pagamento_W
    Inherits AgronicaCoreDataProvider.DataProvider


    '============================================================================
    Public Function Pagamento_Scrivi(ByVal DatiPagamento As String,
                                     ByVal Id_Agenda As Integer,
                                     ByVal Id_Mov As Integer,
                                     ByRef objParametri As AgronicaCoreParametri,
                                     Optional ByVal Data_creazione As Date = #2/1/1900#,
                                     Optional ByVal Data_modifica As Date = #2/1/1900#,
                                     Optional ByVal username_creazione As String = "",
                                     Optional ByVal username_modifica As String = ""
                                     ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabBIZ.Pagamento_W.Pagamento_Scrivi()"

        Dim ObjSequenze As Agro_Sequenze
        Dim ObjPagamento As AgronicaCoreContabDAL.Pagamenti_W
        Dim ObjLiquidita As AgronicaCoreContabDAL.Liquidita_W

        Dim Dummy As Boolean
        Dim XmlDoc As XmlDocument

        Dim xDatiPagamenti As XmlNodeList
        Dim xDatiPagamento As XmlElement
        Dim xPagamenti As XmlNodeList
        Dim xPagamento As XmlElement

        Dim i_DatiPagamento As Integer
        Dim i_Pagamento As Integer

        Dim OpeDB_Pagamento As String

        Dim Cod_Pagamento As Integer

        '------------------------------
        Dim flagTransazioneLocale As Boolean = False
        Dim flagConnessioneLocale As Boolean = False

        Dim messaggioErrore As String = ""
        Dim xRisp As Boolean = True
        '------------------------------


        If Data_creazione = #2/1/1900# Then
            Data_creazione = Now
        End If

        If Data_modifica = #2/1/1900# Then
            Data_modifica = Now
        End If

        If username_creazione = "" Then
            username_creazione = objParametri.UsernameOperazione
        End If

        If username_modifica = "" Then
            username_modifica = objParametri.UsernameOperazione
        End If



        Try

            '------------------------------

            'Se la connessione è chiusa la apro
            If objParametri.objConnessione Is Nothing Then
                objParametri.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
                objParametri.objConnessione.Open()
                flagConnessioneLocale = True
            End If

            If objParametri.objTransazione Is Nothing Then
                objParametri.objTransazione = objParametri.objConnessione.BeginTransaction
                flagTransazioneLocale = True
            End If

            '------------------------------

            XmlDoc = New XmlDocument
            'XmlDoc.async = False
            XmlDoc.LoadXml(DatiPagamento)

            '------------------------------

            xDatiPagamenti = XmlDoc.GetElementsByTagName("DatiPagamenti")

            i_DatiPagamento = 0

            Do While i_DatiPagamento < xDatiPagamenti.Count

                'Prelevo l'i-esimo blocco di DatiPagamenti (in realta' ne esiste uno solo)
                xDatiPagamento = xDatiPagamenti.Item(i_DatiPagamento)

                '------------------------------

                xPagamenti = xDatiPagamento.GetElementsByTagName("Pagamento")

                i_Pagamento = 0

                Do While i_Pagamento < xPagamenti.Count
                    'Prelevo l' i-esimo Pagamento di credito
                    xPagamento = xPagamenti.Item(i_Pagamento)

                    'Prelevo gli attributi del rapporto selezionato
                    OpeDB_Pagamento = xPagamento.GetAttribute("TipoOperazioneDB")

                    'Creo l'oggetto COM
                    ObjPagamento = New AgronicaCoreContabDAL.Pagamenti_W
                    ObjLiquidita = New AgronicaCoreContabDAL.Liquidita_W

                    'Inizializzo Preventivamente il Cod_Pagamento
                    Cod_Pagamento = CInt(xPagamento.GetAttribute("cod_pagamento"))

                    'Verifico l'operazione richiesta
                    Select Case OpeDB_Pagamento
                        '
                        Case "0"    'LEGGI -------------------------------------------------------
                            '
                        Case "1"    'SALVA -------------------------------------------------------

                            ObjSequenze = New Agro_Sequenze

                            'Richiedo un nuovo codice rapporto
                            Cod_Pagamento = ObjSequenze.NuovoId_Tabella("Pagamento",
                                                                        CInt(xPagamento.GetAttribute("basecode")),
                                                                        CInt(xPagamento.GetAttribute("topcode")),
                                                                        objParametri)

                            ObjSequenze = Nothing


                            Dummy = ObjPagamento.Scrivi(CStr(xPagamento.GetAttribute("piva")),
                                                        CInt(xPagamento.GetAttribute("sa_cod")),
                                                        Id_Agenda,
                                                        Id_Mov,
                                                        Cod_Pagamento,
                                                        CDbl(xPagamento.GetAttribute("importo")),
                                                        CDbl(xPagamento.GetAttribute("percentuale")),
                                                        CDate(xPagamento.GetAttribute("data_pagamento")),
                                                        CStr(xPagamento.GetAttribute("note")),
                                                        CStr(xPagamento.GetAttribute("cau_risorsa")),
                                                        CInt(xPagamento.GetAttribute("cod_liquidita_dare")),
                                                        CInt(xPagamento.GetAttribute("cod_liquidita_avere")),
                                                        CInt(xPagamento.GetAttribute("cau_pagamento")),
                                                        IIf(xPagamento.HasAttribute("extra_str") = False, "", xPagamento.GetAttribute("extra_str")),
                                                        IIf(xPagamento.HasAttribute("extra_int") = False, 0, xPagamento.GetAttribute("extra_int")),
                                                        IIf(xPagamento.HasAttribute("extra_date") = False, CDate(Now), xPagamento.GetAttribute("extra_date")),
                                                        CDate(xPagamento.GetAttribute("validita_inizio")),
                                                        CDate(xPagamento.GetAttribute("validita_fine")),
                                                        objParametri,
                                                        Data_creazione,
                                                        Data_modifica,
                                                        username_creazione,
                                                        username_modifica)

                            'Aggiornamento Saldo della Risorsa Finanziaria
                            If CDate(xPagamento.GetAttribute("data_pagamento")) < AGRODATAFINE Then

                                'Pagamento Effettuato

                                If CInt(xPagamento.GetAttribute("cod_liquidita_dare")) <> -1 Then
                                    'Risorsa Non Fittizia

                                    Dummy = ObjLiquidita.ModificaSaldo(CInt(xPagamento.GetAttribute("cod_liquidita_dare")),
                                                                       CDbl(xPagamento.GetAttribute("importo")),
                                                                       "",
                                                                       objParametri)

                                End If

                                If CInt(xPagamento.GetAttribute("cod_liquidita_avere")) <> -1 Then
                                    'Risorsa Non Fittizia

                                    Dummy = ObjLiquidita.ModificaSaldo(CInt(xPagamento.GetAttribute("cod_liquidita_avere")),
                                                                       -CDbl(xPagamento.GetAttribute("importo")),
                                                                       "",
                                                                       objParametri)

                                End If

                            End If

                            '
                            '
                        Case "2"    'MODIFICA -------------------------------------------------------
                            '
                            'Non Gestita


                            '                  ObjPagamento.Modifica _
                            '                           CStr(xPagamento.getAttribute("piva")), _
                            '                           cint(xPagamento.getAttribute("sa_cod")), _
                            '                           Id_Agenda, _
                            '                           Id_Mov, _
                            '                           Cod_Pagamento, _
                            '                           CDbl(xPagamento.getAttribute("importo")), _
                            '                           CDbl(xPagamento.getAttribute("percentuale")), _
                            '                           CDate(xPagamento.getAttribute("data_pagamento")), _
                            '                           CStr(xPagamento.getAttribute("note")), _
                            '                           CStr(xPagamento.getAttribute("cau_risorsa")), _
                            '                           cint(xPagamento.getAttribute("cod_liquidita_dare")), _
                            '                           cint(xPagamento.getAttribute("cod_liquidita_avere")), _
                            '                           cint(xPagamento.getAttribute("cau_pagamento")), _
                            '                           IIf(IsNull(xPagamento.getAttribute("extra_str")), "", xPagamento.getAttribute("extra_str")), _
                            '                           IIf(IsNull(xPagamento.getAttribute("extra_int")), 0, xPagamento.getAttribute("extra_int")), _
                            '                           IIf(IsNull(xPagamento.getAttribute("extra_date")), CDate(Now), xPagamento.getAttribute("extra_date")), _
                            '                           CStr(Utente), _
                            '                           CDate(xPagamento.getAttribute("validita_inizio")), _
                            '                           CDate(xPagamento.getAttribute("validita_fine")), _
                            '                           objCnManager, ConnessioneAlternativa



                        Case "3" 'CANCELLAZIONE PAGAMENTO

                            ObjPagamento.Cancella(CStr(xPagamento.GetAttribute("piva")),
                                                  CInt(xPagamento.GetAttribute("sa_cod")),
                                                  Id_Agenda,
                                                  Id_Mov,
                                                  CInt(xPagamento.GetAttribute("cod_pagamento")),
                                                  "",
                                                  objParametri)

                            If CDate(xPagamento.GetAttribute("data_pagamento")) < AGRODATAFINE Then

                                'Pagamento Effettuato

                                If CInt(xPagamento.GetAttribute("cod_liquidita_dare")) <> -1 Then
                                    'Risorsa Non Fittizia
                                    Dummy = ObjLiquidita.ModificaSaldo( _
                                                CInt(xPagamento.GetAttribute("cod_liquidita_dare")), _
                                                -CDbl(xPagamento.GetAttribute("importo")), _
                                                "", _
                                                objParametri)

                                End If

                                If CInt(xPagamento.GetAttribute("cod_liquidita_avere")) <> -1 Then
                                    'Risorsa Non Fittizia

                                    Dummy = ObjLiquidita.ModificaSaldo( _
                                                CInt(xPagamento.GetAttribute("cod_liquidita_avere")), _
                                                CDbl(xPagamento.GetAttribute("importo")), _
                                                "", _
                                                objParametri)
                                End If

                            End If
                    End Select

                    'Elimino l'oggetto
                    ObjPagamento = Nothing
                    ObjLiquidita = Nothing

                    '-------------------------------------------------------------

                    'Incremento l'indice
                    i_Pagamento = i_Pagamento + 1

                Loop

                '------------------------------

                'Incremento l'indice
                i_DatiPagamento = i_DatiPagamento + 1

            Loop


            'Elimino tutti gli oggetti utilizzati

            xPagamento = Nothing
            xPagamenti = Nothing
            xDatiPagamento = Nothing
            xDatiPagamenti = Nothing
            XmlDoc = Nothing


            'Restituisco un valore Dummy
            xRisp = True

            'Se ho la transazione è stata avviata in questa routine faccio il commit
            If flagTransazioneLocale = True Then
                objParametri.objTransazione.Commit()
            End If

            '----------------------------------------------------------------------------

        Catch ex As Exception

            'Restituisco un valore Dummy
            xRisp = False

            'Faccio il rollback della transazione
            If Not objParametri.objTransazione Is Nothing Then
                objParametri.objTransazione.Rollback()
                objParametri.objTransazione = Nothing
            End If

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        Finally

            'Chiudo la connessione se è stata aperta in questa routine
            If (flagConnessioneLocale = True) AndAlso (Not objParametri.objConnessione Is Nothing) Then
                objParametri.objConnessione.Close()
                objParametri.objConnessione.Dispose()
            End If

        End Try

        Return xRisp

    End Function

End Class
