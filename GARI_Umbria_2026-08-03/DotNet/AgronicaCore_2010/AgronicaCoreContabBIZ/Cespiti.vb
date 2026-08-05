Imports System.Data
Imports System.Data.OleDb
Imports System.Xml
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider

Public Class Cespiti_R
    Inherits AgronicaCoreDataProvider.LogProvider


    '============================================================================
    Public Function Leggi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                ByVal ForDelete As Boolean, _
                                ByVal Piva_SuperUser As String, _
                                ByVal Piva As String, _
                                Optional ByVal Sa_Cod As Long = 0, _
                                Optional ByVal IdCodCespite As Long = 0, _
                                Optional ByVal TipoCod As Integer = 0, _
                                Optional ByVal xFiltroAggiuntivo As String = "", _
                                Optional ByVal xOrderBy As String = "", _
                                Optional ByVal ChiamataDaGiasLan As Boolean = False, _
                                Optional ByRef strSQLOutput As String = "") As String



        Dim NomeRoutine As String = "ContabBIZ.Cespiti_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Sa_Cod = 0
        '   Id_Agenda = 0
        '   Id_Mov=0
        '   Cod_Pagamento = 0
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================


        Dim MessaggioErrore As String = ""

        Dim FlagConnessioneLocale As Boolean = False


        Dim i As Int32

        Dim RisultatoFunzione As String = String.Empty

        Dim XmlDoc As XmlDocument             'MSXML2.DOMDocument40
        Dim XmlDatiCespite As XmlElement      'IXMLDOMElement
        Dim XmlCespite As XmlElement          'IXMLDOMElement

        Dim ObjCespite As AgronicaCoreContabDAL.Cespiti_R

        Dim DtCespite As DataTable

        Try
            '------------------------------
            'Verifico se e' stata impostata una connessione
            If IsNothing(objParametri.objConnessione) Then
                'Flag
                FlagConnessioneLocale = True
                'Creo la connessione localmente
                objParametri.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
            End If

            '------------------------------


            ObjCespite = New AgronicaCoreContabDAL.Cespiti_R

            'Mi procuro il datatable richiesto
            DtCespite = ObjCespite.Leggi(objParametri, _
                                            CStr(Piva_SuperUser), _
                                            CStr(Piva), _
                                            CInt(Sa_Cod), _
                                            CLng(IdCodCespite), _
                                            CInt(TipoCod), _
                                            "", "")



            'Se ottengo almeno un risultato, creo la struttura XML
            If DtCespite.Rows.Count > 0 Then


                '----- < Documento XML > -----
                XmlDoc = New XmlDocument

                XmlDatiCespite = XmlDoc.CreateElement("DatiCespiti")


                Dim name(DtCespite.Columns.Count) As String
                Dim j As Integer = 0

                For i = 0 To DtCespite.Rows.Count - 1


                    '----- < Cespite > -----
                    XmlCespite = XmlDoc.CreateElement("Cespite")

                    With XmlCespite
                        .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))

                        For Each column As DataColumn In DtCespite.Columns
                            name(j) = column.ColumnName
                            .SetAttribute(name(j).ToLower, Agro_SQL_Load(DtCespite.Rows(i).Item(name(j))))
                            j += 1
                        Next


                        '.SetAttribute("piva_superuser", Agro_SQL_Load(DtCespite.Rows(i).Item("Piva_SuperUser")))
                        '.SetAttribute("piva", Agro_SQL_Load(DtCespite.Rows(i).Item("Piva")))
                        '.SetAttribute("sa_cod", Agro_SQL_Load(DtCespite.Rows(i).Item("Sa_Cod")))
                        '.SetAttribute("id_cod_cespite", Agro_SQL_Load(DtCespite.Rows(i).Item("id_cod_cespite")))
                        '.SetAttribute("des_cespite", Agro_SQL_Load(DtCespite.Rows(i).Item("des_cespite")))
                        '.SetAttribute("des_cespite_agg", Agro_SQL_Load(DtCespite.Rows(i).Item("des_cespite_agg")))
                        '.SetAttribute("elem_cod", Agro_SQL_Load(DtCespite.Rows(i).Item("Elem_Cod")))
                        '.SetAttribute("tipo_cod", Agro_SQL_Load(DtCespite.Rows(i).Item("tipo_cod")))
                        '.SetAttribute("cesp_cod", Agro_SQL_Load(DtCespite.Rows(i).Item("cesp_cod")))
                        '.SetAttribute("id_cod_categoria", Agro_SQL_Load(DtCespite.Rows(i).Item("id_cod_categoria")))
                        '.SetAttribute("prc_ammor", Agro_SQL_Load(DtCespite.Rows(i).Item("prc_ammor")))
                        '.SetAttribute("ind_tipo_bene", Agro_SQL_Load(DtCespite.Rows(i).Item("ind_tipo_bene")))
                        '.SetAttribute("dat_ini_utilizzo", Agro_SQL_Load(DtCespite.Rows(i).Item("dat_ini_utilizzo")))
                        '.SetAttribute("dat_chiusura", Agro_SQL_Load(DtCespite.Rows(i).Item("dat_chiusura")))
                        '.SetAttribute("flg_import", Agro_SQL_Load(DtCespite.Rows(i).Item("flg_import")))
                    End With

                    XmlDatiCespite.AppendChild(XmlCespite)
                    '----- < / Cespite > -----

                Next

                XmlDoc.AppendChild(XmlDatiCespite)

                RisultatoFunzione = XmlDoc.OuterXml
                '----- < / Documento XML > -----


                XmlCespite = Nothing
                XmlDatiCespite = Nothing
                XmlDoc = Nothing

            Else

                'Altrimenti, se non risulta selezionato nessun pagamento ...
                RisultatoFunzione = ""

            End If

            'Elimino gli oggetti che ho creato
            DtCespite.Dispose()
            DtCespite = Nothing
            ObjCespite = Nothing

            '------------------------------


        Catch ex As Exception

            RisultatoFunzione = ""
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)


        Finally

            'Pulizia
            'objUtentixImprese = Nothing
            'objImpresexCodici = Nothing
            'objImpresexIndirizzi = Nothing
            '
            '
            '
            '

            If FlagConnessioneLocale = True Then
                objParametri.objConnessione.Close()
                objParametri.objConnessione.Dispose()
            End If

        End Try

        'Restituisco il risultato
        Return RisultatoFunzione

    End Function




End Class

'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§


Public Class Cespiti_W
    Inherits AgronicaCoreDataProvider.DataProvider



    ''============================================================================
    '============================================================================
    'Public Function Scrivi( _
    '                                ByVal DatiCespite As String, _
    '                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                    , Optional ByVal Data_creazione As Date = #2/1/1900# _
    '                    , Optional ByVal Data_modifica As Date = #2/1/1900# _
    '                    , Optional ByVal username_creazione As String = "" _
    '                    , Optional ByVal username_modifica As String = "" _
    '                        ) _
    '                                        As Boolean

    '    '----------------------------------------------------------------------

    '    Dim NomeRoutine As String = "AgronicaCoreContabBIZ.Cespiti_W.Scrivi()"

    '    '----------------------------------------------------------------------

    '    Dim ObjSequenze As AgronicaCoreDataProvider.Agro_Sequenze
    '    Dim ObjCespiteW As AgronicaCoreContabDAL.Cespiti_W
    '    'Dim ObjLiquidita As AgronicaCoreContabDAL.Liquidita_W

    '    Dim Dummy As Boolean
    '    Dim XmlDoc As XmlDocument

    '    Dim xDatiCespiti As XmlNodeList       'IXMLDOMNodeList
    '    Dim xDatiCespite As XmlElement        'IXMLDOMElement
    '    Dim xCespiti As XmlNodeList           'IXMLDOMNodeList
    '    Dim xCespite As XmlElement            'IXMLDOMElement

    '    Dim i_DatiCespite As Integer
    '    Dim i_Cespite As Integer

    '    Dim OpeDB_Cespite As String

    '    Dim Cod_Cespite As Int32

    '    '------------------------------
    '    Dim FlagTransazioneLocale As Boolean = False
    '    Dim FlagConnessioneLocale As Boolean = False

    '    Dim MessaggioErrore As String = ""
    '    Dim xRisp As Boolean = True
    '    '------------------------------


    '    If Data_creazione = #2/1/1900# Then
    '        Data_creazione = Date.Now
    '    End If

    '    If Data_modifica = #2/1/1900# Then
    '        Data_modifica = Date.Now
    '    End If

    '    If username_creazione = "" Then
    '        username_creazione = objParametri.UsernameOperazione
    '    End If

    '    If username_modifica = "" Then
    '        username_modifica = objParametri.UsernameOperazione
    '    End If



    '    Try

    '        '------------------------------

    '        'Se la connessione è chiusa la apro
    '        If objParametri.objConnessione Is Nothing Then
    '            objParametri.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
    '            objParametri.objConnessione.Open()
    '            FlagConnessioneLocale = True
    '        End If

    '        If objParametri.objTransazione Is Nothing Then
    '            objParametri.objTransazione = objParametri.objConnessione.BeginTransaction
    '            FlagTransazioneLocale = True
    '        End If

    '        '------------------------------

    '        XmlDoc = New Xml.XmlDocument
    '        'XmlDoc.async = False
    '        XmlDoc.LoadXml(DatiCespite)

    '        '------------------------------

    '        xDatiCespiti = XmlDoc.GetElementsByTagName("DatiCespiti")

    '        i_DatiCespite = 0

    '        Do While i_DatiCespite < xDatiCespiti.Count

    '            'Prelevo l'i-esimo blocco di DatiPagamenti (in realta' ne esiste uno solo)
    '            xDatiCespite = xDatiCespiti.Item(i_DatiCespite)

    '            '------------------------------

    '            xCespiti = xDatiCespite.GetElementsByTagName("Cespite")

    '            i_Cespite = 0

    '            Do While i_Cespite < xCespiti.Count
    '                'Prelevo l' i-esimo Pagamento di credito
    '                xCespite = xCespiti.Item(i_Cespite)

    '                'Prelevo gli attributi del rapporto selezionato
    '                OpeDB_Cespite = xCespite.GetAttribute("TipoOperazioneDB")

    '                'Creo l'oggetto COM


    '                ObjCespiteW = New AgronicaCoreContabDAL.Cespiti_W

    '                'ObjLiquidita = New AgronicaCoreContabDAL.Liquidita_W

    '                'Inizializzo Preventivamente il Cod_Pagamento
    '                Cod_Cespite = CInt(xCespite.GetAttribute("id_cod_cespite"))

    '                'Verifico l'operazione richiesta
    '                Select Case OpeDB_Cespite
    '                    '
    '                    Case "0"    'LEGGI -------------------------------------------------------
    '                        '
    '                    Case "1"    'SALVA -------------------------------------------------------
    '                        '

    '                        'ObjSequenze = CreateObject("Agro_Contab_AD.Agro_Sequenze")
    '                        ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

    '                        'Richiedo un nuovo codice rapporto
    '                        Cod_Cespite = ObjSequenze.NuovoId_Tabella( _
    '                                            "Cespiti", _
    '                                            CInt(xCespite.GetAttribute("basecode")), _
    '                                            CInt(xCespite.GetAttribute("topcode")), _
    '                                            objParametri)

    '                        ObjSequenze = Nothing


    '                        Dummy = ObjCespiteW.Scrivi(objParametri, _
    '                                               CStr(xCespite.GetAttribute("piva_superuser")), _
    '                                               CStr(xCespite.GetAttribute("piva")), _
    '                                               CInt(xCespite.GetAttribute("sa_cod")), _
    '                                               CLng(xCespite.GetAttribute("id_cod_cespite")), _
    '                                               CStr(xCespite.GetAttribute("des_cespite")), _
    '                                               CInt(xCespite.GetAttribute("elem_cod")), _
    '                                               CInt(xCespite.GetAttribute("tipo_cod")), _
    '                                               CLng(xCespite.GetAttribute("cesp_cod")),
    '                                               CLng(xCespite.GetAttribute("cod_categoria")), _
    '                                               CInt(xCespite.GetAttribute("prc_ammor")), _
    '                                               CStr(xCespite.GetAttribute("ind_tipo_bene")), _
    '                                               CDate(xCespite.GetAttribute("dat_ini_utilizzo")), _
    '                                               CDate(xCespite.GetAttribute("dat_chiusura")), _
    '                                               CDate(xCespite.GetAttribute("validita_inizio")), _
    '                                               CDate(xCespite.GetAttribute("validita_fine")), _
    '                                              Data_creazione, _
    '                                               Data_modifica, _
    '                                               username_creazione, _
    '                                               username_modifica)




    '                        ''Aggiornamento Saldo della Risorsa Finanziaria
    '                        'If CDate(xCespite.GetAttribute("data_pagamento")) < CDate("31/12/2100") Then

    '                        '    'Pagamento Effettuato

    '                        '    If CInt(xCespite.GetAttribute("cod_liquidita_dare")) <> -1 Then
    '                        '        'Risorsa Non Fittizia

    '                        '        Dummy = ObjLiquidita.ModificaSaldo( _
    '                        '                    CInt(xCespite.GetAttribute("cod_liquidita_dare")), _
    '                        '                    CDbl(xCespite.GetAttribute("importo")), _
    '                        '                    "", _
    '                        '                    objParametri)

    '                        '    End If

    '                        '    If CInt(xCespite.GetAttribute("cod_liquidita_avere")) <> -1 Then
    '                        '        'Risorsa Non Fittizia

    '                        '        Dummy = ObjLiquidita.ModificaSaldo( _
    '                        '                    CInt(xCespite.GetAttribute("cod_liquidita_avere")), _
    '                        '                    -CDbl(xCespite.GetAttribute("importo")), _
    '                        '                    "", _
    '                        '                    objParametri)


    '                        '    End If

    '                        'End If

    '                        '
    '                        '
    '                    Case "2"    'MODIFICA -------------------------------------------------------
    '                        '
    '                        'Non Gestita


    '                        '                  ObjCespite.Modifica _
    '                        '                           CStr(xCespite.getAttribute("piva")), _
    '                        '                           cint(xCespite.getAttribute("sa_cod")), _
    '                        '                           Id_Agenda, _
    '                        '                           Id_Mov, _
    '                        '                           Cod_Pagamento, _
    '                        '                           CDbl(xCespite.getAttribute("importo")), _
    '                        '                           CDbl(xCespite.getAttribute("percentuale")), _
    '                        '                           CDate(xCespite.getAttribute("data_pagamento")), _
    '                        '                           CStr(xCespite.getAttribute("note")), _
    '                        '                           CStr(xCespite.getAttribute("cau_risorsa")), _
    '                        '                           cint(xCespite.getAttribute("cod_liquidita_dare")), _
    '                        '                           cint(xCespite.getAttribute("cod_liquidita_avere")), _
    '                        '                           cint(xCespite.getAttribute("cau_pagamento")), _
    '                        '                           IIf(IsNull(xCespite.getAttribute("extra_str")), "", xCespite.getAttribute("extra_str")), _
    '                        '                           IIf(IsNull(xCespite.getAttribute("extra_int")), 0, xCespite.getAttribute("extra_int")), _
    '                        '                           IIf(IsNull(xCespite.getAttribute("extra_date")), CDate(Now), xCespite.getAttribute("extra_date")), _
    '                        '                           CStr(Utente), _
    '                        '                           CDate(xCespite.getAttribute("validita_inizio")), _
    '                        '                           CDate(xCespite.getAttribute("validita_fine")), _
    '                        '                           objCnManager, ConnessioneAlternativa



    '                    Case "3" 'CANCELLAZIONE PAGAMENTO

    '                        'ObjCespiteW.Cancella( _
    '                        '                    CStr(xCespite.GetAttribute("piva")), _
    '                        '                    CInt(xCespite.GetAttribute("sa_cod")), _
    '                        '                    Id_Agenda, _
    '                        '                    Id_Mov, _
    '                        '                    CInt(xCespite.GetAttribute("cod_pagamento")), _
    '                        '                    "", _
    '                        '                    objParametri)

    '                        'If CDate(xCespite.GetAttribute("data_pagamento")) < CDate("31/12/2100") Then

    '                        '    'Pagamento Effettuato

    '                        '    If CInt(xCespite.GetAttribute("cod_liquidita_dare")) <> -1 Then
    '                        '        'Risorsa Non Fittizia
    '                        '        Dummy = ObjLiquidita.ModificaSaldo( _
    '                        '                    CInt(xCespite.GetAttribute("cod_liquidita_dare")), _
    '                        '                    -CDbl(xCespite.GetAttribute("importo")), _
    '                        '                    "", _
    '                        '                    objParametri)

    '                        '    End If

    '                        '    If CInt(xCespite.GetAttribute("cod_liquidita_avere")) <> -1 Then
    '                        '        'Risorsa Non Fittizia

    '                        '        Dummy = ObjLiquidita.ModificaSaldo( _
    '                        '                    CInt(xCespite.GetAttribute("cod_liquidita_avere")), _
    '                        '                    CDbl(xCespite.GetAttribute("importo")), _
    '                        '                    "", _
    '                        '                    objParametri)
    '                        '    End If

    '                        'End If
    '                End Select

    '                'Elimino l'oggetto
    '                ObjCespiteW = Nothing
    '                'ObjLiquidita = Nothing

    '                '-------------------------------------------------------------

    '                'Incremento l'indice
    '                i_Cespite = i_Cespite + 1

    '            Loop

    '            '------------------------------

    '            'Incremento l'indice
    '            i_DatiCespite = i_DatiCespite + 1

    '        Loop

    '        '------------------------------
    '        '------------------------------
    '        '------------------------------

    '        'Elimino tutti gli oggetti utilizzati

    '        xCespite = Nothing
    '        xCespiti = Nothing
    '        xDatiCespite = Nothing
    '        xDatiCespiti = Nothing
    '        XmlDoc = Nothing

    '        '------------------------------

    '        'Restituisco un valore Dummy
    '        xRisp = True

    '        'Se ho la transazione è stata avviata in questa routine faccio il commit
    '        If FlagTransazioneLocale = True Then
    '            objParametri.objTransazione.Commit()
    '        End If

    '        '----------------------------------------------------------------------------

    '    Catch ex As Exception

    '        'Restituisco un valore Dummy
    '        xRisp = False

    '        'Faccio il rollback della transazione
    '        If Not objParametri.objTransazione Is Nothing Then
    '            objParametri.objTransazione.Rollback()
    '            objParametri.objTransazione = Nothing
    '        End If

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    Finally

    '        'Chiudo la connessione se è stata aperta in questa routine
    '        If (FlagConnessioneLocale = True) AndAlso (Not objParametri.objConnessione Is Nothing) Then
    '            objParametri.objConnessione.Close()
    '            objParametri.objConnessione.Dispose()
    '        End If

    '    End Try

    '    Return xRisp

    'End Function



End Class

