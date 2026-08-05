Imports System.Data
Imports System.Xml
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.UtilityProvider



Public Class Trasformazioni_R
    Inherits AgronicaCoreDataProvider.LogProvider


    '============================================================================
    Public Function Trasformazioni_Leggi(
                                ByVal Piva As String,
                                ByVal Id_Trasformazione As Int32,
                                    ByVal ForDelete As Boolean,
                                    ByRef objParametri As AgronicaCoreParametri) _
                                        As String

        Const nomeRoutine = "ContabBIZ.Trasformazioni_R.Trasformazioni_Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Id_Trasformazione = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim FlagConnessioneLocale As Boolean = False

        Dim i As Int32
        Dim j As Int32
        Dim RisultatoFunzione As String = String.Empty

        '-----

        Dim XmlDoc As XmlDocument                           'MSXML2.DOMDocument40

        Dim XmlDatiTrasformazioni As XmlElement             'IXMLDOMElement
        Dim XmlTrasformazione As XmlElement                 'IXMLDOMElement
        Dim XmlDatiContrattoxTrasformazioni As XmlElement   'IXMLDOMElement
        Dim XmlContrattoxTrasformazione As XmlElement       'IXMLDOMElement

        'Dim objTrasformazioni                  As Agro_Contab_AD.Trasformazioni_R
        Dim objTrasformazioni As AgronicaCoreContabDAL.Trasformazioni_R

        'Dim ObjContrattixTrasformazioni        As Agro_Contab_AD.Imprese_ContrattixTras_R
        Dim ObjContrattixTrasformazioni As AgronicaCoreContabDAL.Imprese_ContrattixTras_R


        Dim DtTrasformazioni As DataTable                   'ADODB.Recordset
        Dim DtContrattixTrasformazioni As DataTable         'ADODB.Recordset



        Try

            '------------------------------
            'Verifico se e' stata impostata una connessione
            If IsNothing(objParametri.objConnessione) Then
                FlagConnessioneLocale = True
                objParametri.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
            End If
            '------------------------------



            'Mi procuro un elenco delle trasformazioni associate all'Impresa
            'all'interno della finestra temporale selezionata

            'Creo l'oggetto COM+
            'objTrasformazioni = CreateObject("Agro_Contab_AD.Trasformazioni_R")
            objTrasformazioni = New AgronicaCoreContabDAL.Trasformazioni_R

            'Mi procuro il recordset richiesto

            DtTrasformazioni = objTrasformazioni.Leggi(
                                                CStr(Piva),
                                                0,
                                                Id_Trasformazione,
                                                0,
                                                0,
                                                0,
                                                0,
                                                "",
                                                enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                "",
                                                "",
                                                objParametri)

            'Se ottengo almeno un risultato, creo la struttura XML
            If DtTrasformazioni.Rows.Count > 0 Then

                '----- < Documento XML > -----
                'XmlDoc = CreateObject("Msxml2.DOMDocument.4.0")
                XmlDoc = New XmlDocument

                XmlDatiTrasformazioni = XmlDoc.CreateElement("DatiTrasformazioni")


                'Effettuo un ciclo sulle Trasformazioni
                'Do While Not RsTrasformazioni.EOF
                For i = 0 To DtTrasformazioni.Rows.Count - 1

                    '----- < Trasformazione > -----
                    XmlTrasformazione = XmlDoc.CreateElement("Trasformazione")

                    With XmlTrasformazione
                        .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                        .SetAttribute("piva", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("piva")))
                        .SetAttribute("sa_cod", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("sa_cod")))
                        .SetAttribute("preparazione_cod", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("preparazione_cod")))
                        .SetAttribute("linea_cod", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("linea_cod")))
                        .SetAttribute("id_trasformazione", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("id_trasformazione")))
                        .SetAttribute("trasformazione_des", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("trasformazione_des")))
                        .SetAttribute("stato", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("stato")))
                        .SetAttribute("id_agenda", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("id_agenda")))
                        .SetAttribute("cau_mov", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("cau_mov")))
                        .SetAttribute("step_giorni", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("step_giorni")))
                        .SetAttribute("ora1", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("ora1")))
                        .SetAttribute("ora2", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("ora2")))
                        .SetAttribute("ora3", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("ora3")))
                        .SetAttribute("ora4", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("ora4")))
                        .SetAttribute("tipo_stima", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("tipo_stima")))
                        .SetAttribute("elem_cod", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("elem_cod")))
                        .SetAttribute("mat_cod", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("mat_cod")))
                        .SetAttribute("udm_cod", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("udm_cod")))
                        .SetAttribute("stima", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("stima")))
                        .SetAttribute("colore", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("colore")))
                        .SetAttribute("monitor", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("monitor")))
                        .SetAttribute("note", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("note")))
                        .SetAttribute("validita_inizio", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("validita_inizio")))
                        .SetAttribute("validita_fine", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("validita_fine")))
                        .SetAttribute("data_creazione", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("Data_Creazione")))
                        .SetAttribute("data_modifica", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("Data_Modifica")))
                        .SetAttribute("username_creazione", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("Username_Creazione")))
                        .SetAttribute("username_modifica", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("Username_Modifica")))
                    End With




                    '#############################################
                    '###########  CONTRATTIxTRASFORMAZIONI  ######
                    '#############################################

                    'Mi procuro un elenco dei contratti associati alla trasformazione

                    'Creo l'oggetto COM+
                    'ObjContrattixTrasformazioni = CreateObject("Agro_Contab_AD.Imprese_ContrattixTras_R")
                    ObjContrattixTrasformazioni = New AgronicaCoreContabDAL.Imprese_ContrattixTras_R

                    'Mi procuro il recordset richiesto
                    DtContrattixTrasformazioni = ObjContrattixTrasformazioni.Leggi(
                                                Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("PIVA")),
                                                0,
                                                0,
                                                0,
                                                Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("Id_Trasformazione")),
                                                enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                "",
                                                "",
                                                objParametri)

                    'Se ottengo almeno un risultato, creo la struttura XML
                    If DtContrattixTrasformazioni.Rows.Count > 0 Then

                        XmlDatiContrattoxTrasformazioni = XmlDoc.CreateElement("DatiContrattixTrasformazioni")

                        'Effettuo un ciclo sui contrattixtrasformazioni
                        'Do While Not RsContrattixTrasformazioni.EOF
                        For j = 0 To DtContrattixTrasformazioni.Rows.Count - 1

                            '----- < ContrattoxTrasformazioni > -----
                            XmlContrattoxTrasformazione = XmlDoc.CreateElement("ContrattoxTrasformazione")

                            With XmlContrattoxTrasformazione
                                .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                .SetAttribute("piva", Agro_SQL_Load(DtContrattixTrasformazioni.Rows(j).Item("piva")))
                                .SetAttribute("contratto_cod", Agro_SQL_Load(DtContrattixTrasformazioni.Rows(j).Item("contratto_cod")))
                                .SetAttribute("progetto_cod", Agro_SQL_Load(DtContrattixTrasformazioni.Rows(j).Item("progetto_cod")))
                                .SetAttribute("fase_cod", Agro_SQL_Load(DtContrattixTrasformazioni.Rows(j).Item("fase_cod")))
                                .SetAttribute("id_trasformazione", Agro_SQL_Load(DtContrattixTrasformazioni.Rows(j).Item("id_trasformazione")))
                                .SetAttribute("udm_cod", Agro_SQL_Load(DtContrattixTrasformazioni.Rows(j).Item("udm_cod")))
                                .SetAttribute("qta", Agro_SQL_Load(DtContrattixTrasformazioni.Rows(j).Item("qta")))
                                .SetAttribute("contratto_nome", Agro_SQL_Load(DtContrattixTrasformazioni.Rows(j).Item("contratto_nome")))
                                .SetAttribute("contratto_des", Agro_SQL_Load(DtContrattixTrasformazioni.Rows(j).Item("contratto_des")))
                                .SetAttribute("contratto_numero", Agro_SQL_Load(DtContrattixTrasformazioni.Rows(j).Item("contratto_numero")))
                                .SetAttribute("cau_contratto", Agro_SQL_Load(DtContrattixTrasformazioni.Rows(j).Item("cau_contratto")))
                                .SetAttribute("cod_risum", Agro_SQL_Load(DtContrattixTrasformazioni.Rows(j).Item("cod_risum")))
                                .SetAttribute("elem_cod", Agro_SQL_Load(DtContrattixTrasformazioni.Rows(j).Item("elem_cod")))
                                .SetAttribute("pro_cod", Agro_SQL_Load(DtContrattixTrasformazioni.Rows(j).Item("pro_cod")))
                                .SetAttribute("mat_cod", Agro_SQL_Load(DtContrattixTrasformazioni.Rows(j).Item("mat_cod")))
                                .SetAttribute("cal_cod", Agro_SQL_Load(DtContrattixTrasformazioni.Rows(j).Item("cal_cod")))
                                .SetAttribute("progetto_cod", Agro_SQL_Load(DtContrattixTrasformazioni.Rows(j).Item("progetto_cod")))
                                .SetAttribute("lotto", Agro_SQL_Load(DtContrattixTrasformazioni.Rows(j).Item("lotto")))
                                .SetAttribute("udm_cod_fase", Agro_SQL_Load(DtContrattixTrasformazioni.Rows(j).Item("udm_cod_fase")))
                                .SetAttribute("qta_fase", Agro_SQL_Load(DtContrattixTrasformazioni.Rows(j).Item("qta_fase")))
                                .SetAttribute("validita_inizio", Agro_SQL_Load(DtContrattixTrasformazioni.Rows(j).Item("validita_inizio")))
                                .SetAttribute("validita_fine", Agro_SQL_Load(DtContrattixTrasformazioni.Rows(j).Item("validita_fine")))
                            End With

                            XmlDatiContrattoxTrasformazioni.AppendChild(XmlContrattoxTrasformazione)

                            '----- < / CONTRATTOXTRASFORMAZIONE > -----


                        Next
                        'Loop

                        XmlTrasformazione.AppendChild(XmlDatiContrattoxTrasformazioni)


                    End If

                    XmlContrattoxTrasformazione = Nothing
                    DtContrattixTrasformazioni.Dispose()
                    DtContrattixTrasformazioni = Nothing
                    ObjContrattixTrasformazioni = Nothing


                    '#################################
                    '#################################
                    '#################################


                    XmlDatiTrasformazioni.AppendChild(XmlTrasformazione)

                    '----- < / Trasformazione > -----

                Next
                'Loop


                XmlDoc.AppendChild(XmlDatiTrasformazioni)

                RisultatoFunzione = XmlDoc.OuterXml
                '----- < / Documento XML > -----


                XmlDatiTrasformazioni = Nothing
                XmlTrasformazione = Nothing
                XmlDoc = Nothing

            Else

                'Altrimenti, se non risulta selezionato nessun contatto ...
                RisultatoFunzione = ""

            End If


            '------------------------------
            '------------------------------
            '------------------------------

            'Elimino gli oggetti che ho creato

            DtTrasformazioni.Dispose()
            DtTrasformazioni = Nothing
            objTrasformazioni = Nothing


            '-------------------------------------------------------------------------------


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
            '
            '
            '
            '

            '------------------------------
            If FlagConnessioneLocale Then
                objParametri.objConnessione.Close()
                objParametri.objConnessione.Dispose()
            End If

        End Try

        'Restituisco il risultato
        Return RisultatoFunzione

    End Function


    ''============================================================================
    'Public Function Trasformazioni_Leggi( _
    '                            ByVal Piva As String, _
    '                            ByVal Id_Trasformazione As Int32, _
    '                                ByVal ForDelete As Boolean, _
    '                                ByVal FinestraTemp_Inizio As Date, _
    '                                ByVal FinestraTemp_Fine As Date, _
    '                                ByRef objConnessione As DbConnection, _
    '                                ByVal StringaConnessione As String, _
    '                                ByVal FlagVisibilita As Int32, _
    '                                ByVal DirectoryLOG As String, _
    '                                ByVal FileLOG As String, _
    '                                ByVal IdentificatoreUtente As String) _
    '                                    As String

    '    Dim NomeRoutine As String = "ContabBIZ.Trasformazioni_R.Trasformazioni_Leggi()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   Id_Trasformazione = 0
    '    '
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim FlagConnessioneLocale As Boolean = False
    '    Dim xConnessione As DbConnection
    '     
    '    Dim i As Int32
    '    Dim j As Int32
    '    Dim RisultatoFunzione As String = String.Empty

    '    '-----

    '    Dim XmlDoc As XmlDocument                           'MSXML2.DOMDocument40

    '    Dim XmlDatiTrasformazioni As XmlElement             'IXMLDOMElement
    '    Dim XmlTrasformazione As XmlElement                 'IXMLDOMElement
    '    Dim XmlDatiContrattoxTrasformazioni As XmlElement   'IXMLDOMElement
    '    Dim XmlContrattoxTrasformazione As XmlElement       'IXMLDOMElement

    '    'Dim objTrasformazioni                  As Agro_Contab_AD.Trasformazioni_R
    '    Dim objTrasformazioni As AgronicaCoreContabDAL.Trasformazioni_R

    '    'Dim ObjContrattixTrasformazioni        As Agro_Contab_AD.Imprese_ContrattixTras_R
    '    Dim ObjContrattixTrasformazioni As AgronicaCoreContabDAL.Imprese_ContrattixTras_R


    '    Dim DtTrasformazioni As DataTable                   'ADODB.Recordset
    '    Dim DtContrattixTrasformazioni As DataTable         'ADODB.Recordset



    '    Try

    '        '------------------------------
    '        'Verifico se e' stata impostata una connessione
    '        If IsNothing(objConnessione) Then
    '            'Flag
    '            FlagConnessioneLocale = True
    '            'Creo la connessione localmente
    '            xConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(StringaConnessione)
    '        Else
    '            'Utilizzo quella passata come parametro
    '            xConnessione = objConnessione
    '            xConnectionState = objConnessione.State
    '        End If

    '        '------------------------------



    '        'Mi procuro un elenco delle trasformazioni associate all'Impresa
    '        'all'interno della finestra temporale selezionata

    '        'Creo l'oggetto COM+
    '        'objTrasformazioni = CreateObject("Agro_Contab_AD.Trasformazioni_R")
    '        objTrasformazioni = New AgronicaCoreContabDAL.Trasformazioni_R

    '        'Mi procuro il recordset richiesto
    '        DtTrasformazioni = objTrasformazioni.Leggi( _
    '                                            CStr(Piva), _
    '                                            0, _
    '                                            Id_Trasformazione, _
    '                                            0, _
    '                                            0, _
    '                                            0, _
    '                                            0, _
    '                                            "", _
    '                                            FinestraTemp_Inizio, _
    '                                            FinestraTemp_Fine, _
    '                                            objConnessione, _
    '                                            StringaConnessione, _
    '                                            FlagVisibilita, _
    '                                            DirectoryLOG, _
    '                                            FileLOG, _
    '                                            IdentificatoreUtente)

    '        'Se ottengo almeno un risultato, creo la struttura XML
    '        If DtTrasformazioni.Rows.Count > 0 Then

    '            '----- < Documento XML > -----
    '            'XmlDoc = CreateObject("Msxml2.DOMDocument.4.0")
    '            XmlDoc = New XmlDocument

    '            XmlDatiTrasformazioni = XmlDoc.CreateElement("DatiTrasformazioni")


    '            'Effettuo un ciclo sulle Trasformazioni
    '            'Do While Not RsTrasformazioni.EOF
    '            For i = 0 To DtTrasformazioni.Rows.Count - 1

    '                '----- < Trasformazione > -----
    '                XmlTrasformazione = XmlDoc.CreateElement("Trasformazione")

    '                With XmlTrasformazione
    '                    .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
    '                    .SetAttribute("piva", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("piva")))
    '                    .SetAttribute("sa_cod", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("sa_cod")))
    '                    .SetAttribute("preparazione_cod", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("preparazione_cod")))
    '                    .SetAttribute("linea_cod", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("linea_cod")))
    '                    .SetAttribute("id_trasformazione", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("id_trasformazione")))
    '                    .SetAttribute("trasformazione_des", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("trasformazione_des")))
    '                    .SetAttribute("stato", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("stato")))
    '                    .SetAttribute("id_agenda", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("id_agenda")))
    '                    .SetAttribute("cau_mov", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("cau_mov")))
    '                    .SetAttribute("step_giorni", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("step_giorni")))
    '                    .SetAttribute("ora1", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("ora1")))
    '                    .SetAttribute("ora2", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("ora2")))
    '                    .SetAttribute("ora3", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("ora3")))
    '                    .SetAttribute("ora4", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("ora4")))
    '                    .SetAttribute("tipo_stima", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("tipo_stima")))
    '                    .SetAttribute("elem_cod", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("elem_cod")))
    '                    .SetAttribute("mat_cod", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("mat_cod")))
    '                    .SetAttribute("udm_cod", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("udm_cod")))
    '                    .SetAttribute("stima", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("stima")))
    '                    .SetAttribute("colore", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("colore")))
    '                    .SetAttribute("monitor", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("monitor")))
    '                    .SetAttribute("note", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("note")))
    '                    .SetAttribute("validita_inizio", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("validita_inizio")))
    '                    .SetAttribute("validita_fine", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("validita_fine")))
    '                    .SetAttribute("data_creazione", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("Data_Creazione")))
    '                    .SetAttribute("data_modifica", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("Data_Modifica")))
    '                    .SetAttribute("username_creazione", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("Username_Creazione")))
    '                    .SetAttribute("username_modifica", Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("Username_Modifica")))
    '                End With




    '                '#############################################
    '                '###########  CONTRATTIxTRASFORMAZIONI  ######
    '                '#############################################

    '                'Mi procuro un elenco dei contratti associati alla trasformazione

    '                'Creo l'oggetto COM+
    '                'ObjContrattixTrasformazioni = CreateObject("Agro_Contab_AD.Imprese_ContrattixTras_R")
    '                ObjContrattixTrasformazioni = New AgronicaCoreContabDAL.Imprese_ContrattixTras_R

    '                'Mi procuro il recordset richiesto
    '                DtContrattixTrasformazioni = ObjContrattixTrasformazioni.Leggi( _
    '                                            Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("PIVA")), _
    '                                            0, _
    '                                            0, _
    '                                            0, _
    '                                            Agro_SQL_Load(DtTrasformazioni.Rows(i).Item("Id_Trasformazione")), _
    '                                            FinestraTemp_Inizio, _
    '                                            FinestraTemp_Fine, _
    '                                            objConnessione, _
    '                                            StringaConnessione, _
    '                                            FlagVisibilita, _
    '                                            DirectoryLOG, _
    '                                            FileLOG, _
    '                                            IdentificatoreUtente)

    '                'Se ottengo almeno un risultato, creo la struttura XML
    '                If DtContrattixTrasformazioni.Rows.Count > 0 Then

    '                    XmlDatiContrattoxTrasformazioni = XmlDoc.CreateElement("DatiContrattixTrasformazioni")

    '                    'Effettuo un ciclo sui contrattixtrasformazioni
    '                    'Do While Not RsContrattixTrasformazioni.EOF
    '                    For j = 0 To DtContrattixTrasformazioni.Rows.Count - 1

    '                        '----- < ContrattoxTrasformazioni > -----
    '                        XmlContrattoxTrasformazione = XmlDoc.CreateElement("ContrattoxTrasformazione")

    '                        With XmlContrattoxTrasformazione
    '                            .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
    '                            .SetAttribute("piva", Agro_SQL_Load(DtContrattixTrasformazioni.Rows(j).Item("piva")))
    '                            .SetAttribute("contratto_cod", Agro_SQL_Load(DtContrattixTrasformazioni.Rows(j).Item("contratto_cod")))
    '                            .SetAttribute("progetto_cod", Agro_SQL_Load(DtContrattixTrasformazioni.Rows(j).Item("progetto_cod")))
    '                            .SetAttribute("fase_cod", Agro_SQL_Load(DtContrattixTrasformazioni.Rows(j).Item("fase_cod")))
    '                            .SetAttribute("id_trasformazione", Agro_SQL_Load(DtContrattixTrasformazioni.Rows(j).Item("id_trasformazione")))
    '                            .SetAttribute("udm_cod", Agro_SQL_Load(DtContrattixTrasformazioni.Rows(j).Item("udm_cod")))
    '                            .SetAttribute("qta", Agro_SQL_Load(DtContrattixTrasformazioni.Rows(j).Item("qta")))
    '                            .SetAttribute("contratto_nome", Agro_SQL_Load(DtContrattixTrasformazioni.Rows(j).Item("contratto_nome")))
    '                            .SetAttribute("contratto_des", Agro_SQL_Load(DtContrattixTrasformazioni.Rows(j).Item("contratto_des")))
    '                            .SetAttribute("contratto_numero", Agro_SQL_Load(DtContrattixTrasformazioni.Rows(j).Item("contratto_numero")))
    '                            .SetAttribute("cau_contratto", Agro_SQL_Load(DtContrattixTrasformazioni.Rows(j).Item("cau_contratto")))
    '                            .SetAttribute("cod_risum", Agro_SQL_Load(DtContrattixTrasformazioni.Rows(j).Item("cod_risum")))
    '                            .SetAttribute("elem_cod", Agro_SQL_Load(DtContrattixTrasformazioni.Rows(j).Item("elem_cod")))
    '                            .SetAttribute("pro_cod", Agro_SQL_Load(DtContrattixTrasformazioni.Rows(j).Item("pro_cod")))
    '                            .SetAttribute("mat_cod", Agro_SQL_Load(DtContrattixTrasformazioni.Rows(j).Item("mat_cod")))
    '                            .SetAttribute("cal_cod", Agro_SQL_Load(DtContrattixTrasformazioni.Rows(j).Item("cal_cod")))
    '                            .SetAttribute("progetto_cod", Agro_SQL_Load(DtContrattixTrasformazioni.Rows(j).Item("progetto_cod")))
    '                            .SetAttribute("lotto", Agro_SQL_Load(DtContrattixTrasformazioni.Rows(j).Item("lotto")))
    '                            .SetAttribute("udm_cod_fase", Agro_SQL_Load(DtContrattixTrasformazioni.Rows(j).Item("udm_cod_fase")))
    '                            .SetAttribute("qta_fase", Agro_SQL_Load(DtContrattixTrasformazioni.Rows(j).Item("qta_fase")))
    '                            .SetAttribute("validita_inizio", Agro_SQL_Load(DtContrattixTrasformazioni.Rows(j).Item("validita_inizio")))
    '                            .SetAttribute("validita_fine", Agro_SQL_Load(DtContrattixTrasformazioni.Rows(j).Item("validita_fine")))
    '                        End With

    '                        XmlDatiContrattoxTrasformazioni.AppendChild(XmlContrattoxTrasformazione)

    '                        '----- < / CONTRATTOXTRASFORMAZIONE > -----


    '                    Next
    '                    'Loop

    '                    XmlTrasformazione.AppendChild(XmlDatiContrattoxTrasformazioni)


    '                End If

    '                XmlContrattoxTrasformazione = Nothing
    '                DtContrattixTrasformazioni.Dispose()
    '                DtContrattixTrasformazioni = Nothing
    '                ObjContrattixTrasformazioni = Nothing


    '                '#################################
    '                '#################################
    '                '#################################


    '                XmlDatiTrasformazioni.AppendChild(XmlTrasformazione)

    '                '----- < / Trasformazione > -----

    '            Next
    '            'Loop


    '            XmlDoc.AppendChild(XmlDatiTrasformazioni)

    '            RisultatoFunzione = XmlDoc.OuterXml
    '            '----- < / Documento XML > -----


    '            XmlDatiTrasformazioni = Nothing
    '            XmlTrasformazione = Nothing
    '            XmlDoc = Nothing

    '        Else

    '            'Altrimenti, se non risulta selezionato nessun contatto ...
    '            RisultatoFunzione = ""

    '        End If


    '        '------------------------------
    '        '------------------------------
    '        '------------------------------

    '        'Elimino gli oggetti che ho creato

    '        DtTrasformazioni.Dispose()
    '        DtTrasformazioni = Nothing
    '        objTrasformazioni = Nothing


    '        '-------------------------------------------------------------------------------


    '    Catch ex As Exception

    '        RisultatoFunzione = ""
    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)


    '    Finally

    '        'Pulizia
    '        'objUtentixImprese = Nothing
    '        'objImpresexCodici = Nothing
    '        'objImpresexIndirizzi = Nothing
    '        '
    '        '
    '        '
    '        '

    '        If FlagConnessioneLocale = True Then
    '            If Not IsNothing(xConnessione) Then
    '                xConnessione.Close()
    '                xConnessione.Dispose()
    '            End If
    '        Else
    '            If xConnectionState = ConnectionState.Closed Then
    '                xConnessione.Close()
    '            End If
    '        End If

    '    End Try

    '    'Restituisco il risultato
    '    Return RisultatoFunzione

    'End Function


End Class

'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§


Public Class Trasformazioni_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '============================================================================
    Public Function Trasformazioni_Scrivi(ByVal DatiTrasformazioni As String,
                                          ByRef objParametri As AgronicaCoreParametri
                                          ) As Boolean

        '----------------------------------------------------------------------

        Const nomeRoutine = "AgronicaCoreContabBIZ.Trasformazioni_W.Trasformazioni_Scrivi()"

        '----------------------------------------------------------------------

        Dim Dummy As Boolean

        '   Dim ObjSequenze                  As Agro_Contab_AD.Agro_Sequenze
        '   Dim ObjTrasformazione            As Agro_Contab_AD.Trasformazioni_W
        '   Dim ObjContrattoxTrasformazioni  As Agro_Contab_AD.Imprese_ContrattixTras_W

        Dim ObjSequenze As Agro_Sequenze
        Dim ObjTrasformazione As AgronicaCoreContabDAL.Trasformazioni_W
        Dim ObjContrattoxTrasformazioni As AgronicaCoreContabDAL.Imprese_ContrattixTras_W

        Dim XmlDoc As XmlDocument

        Dim XmlDatiTrasformazioni As XmlNodeList                'IXMLDOMNodeList
        Dim XmlDatiTrasformazione As XmlElement                 'IXMLDOMElement
        Dim XmlTrasformazioni As XmlNodeList                    'IXMLDOMNodeList
        Dim XmlTrasformazione As XmlElement                     'IXMLDOMElement
        Dim XmlDatiContrattoxTrasformazioni As XmlNodeList      'IXMLDOMNodeList
        Dim xDatiContrattixTrasformazioni As XmlNodeList        'IXMLDOMNodeList
        Dim xDatiContrattixTrasformazione As XmlElement         'IXMLDOMElement
        Dim xContrattoxTrasformazione As XmlElement             'IXMLDOMElement

        Dim i_DatiContrattixTrasformazioni As Integer
        Dim i_ContrattixTrasformazioni As Integer

        Dim OpeDB_Trasformazione As String
        Dim OpeDB_ContrattoxTrasformazione As String

        Dim Id_Trasformazione As Integer


        '------------------------------
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim messaggioErrore As String = ""
        Dim xRisp As Boolean = True
        '------------------------------



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

            '------------------------------

            XmlDoc = New XmlDocument
            'XmlDoc.async = False
            XmlDoc.LoadXml(DatiTrasformazioni)

            '------------------------------
            '------------------------------
            '------------------------------


            '#####################################################################################
            '#####################  Gestione Parametri Produzione/Trasformazione  ################
            '#####################################################################################

            'Prelevo l'elenco dei parametri
            XmlDatiTrasformazioni = XmlDoc.GetElementsByTagName("DatiTrasformazioni")

            If XmlDatiTrasformazioni.Count > 0 Then

                'Creo gli Oggetti COM+

                'ObjTrasformazione = CreateObject("Agro_Contab_AD.Trasformazioni_W")
                'ObjSequenze = CreateObject("Agro_Contab_AD.Agro_Sequenze")

                ObjTrasformazione = New AgronicaCoreContabDAL.Trasformazioni_W
                ObjSequenze = New Agro_Sequenze

                XmlDatiTrasformazione = XmlDatiTrasformazioni.Item(0)



                '#####################################################################################
                '################################  Trasformazione  ###################################
                '#####################################################################################

                'Prelevo l'eventuale elenco dei dettagli della Trasformazione
                XmlTrasformazioni = XmlDatiTrasformazione.GetElementsByTagName("Trasformazione")

                If XmlTrasformazioni.Count > 0 Then

                    XmlTrasformazione = XmlTrasformazioni.Item(0)

                    'Prelevo gli attributi della trasformazione selezionata
                    OpeDB_Trasformazione = XmlTrasformazione.GetAttribute("TipoOperazioneDB")

                    'Creo l'oggetto COM
                    'ObjTrasformazione = CreateObject("Agro_Contab_AD.Trasformazioni_W")
                    ObjTrasformazione = New AgronicaCoreContabDAL.Trasformazioni_W

                    Id_Trasformazione = UtilityProvider.Agro_SQL_SaveNum(XmlTrasformazione.GetAttribute("id_trasformazione"))

                    'Verifico l'operazione richiesta
                    Select Case OpeDB_Trasformazione

                        Case "0"    'LEGGI -------------------------------------------------------

                        Case "1"    'SALVA -------------------------------------------------------


                            If Id_Trasformazione <= 0 Then

                                'ObjSequenze = CreateObject("Agro_Contab_AD.Agro_Sequenze")
                                ObjSequenze = New Agro_Sequenze

                                'Richiedo un nuovo codice trasformazione

                                Id_Trasformazione = ObjSequenze.NuovoId_Tabella( _
                                                "Trasformazioni", _
                                                CInt(XmlTrasformazione.GetAttribute("basecode")), _
                                                CInt(XmlTrasformazione.GetAttribute("topcode")), _
                                                objParametri)


                                ObjSequenze = Nothing

                            Else

                                'Si è verificata 1 delle seguenti ipotesi:
                                '1.Fase Linea Produttiva (Collegamento tra + operazioni di agenda aventi il medesimo id_trasformazione e linea_cod)
                                '2.Esportazione x Modulo StandAlone

                            End If

                            Dummy = ObjTrasformazione.Scrivi( _
                                                CStr(XmlTrasformazione.GetAttribute("piva")), _
                                                CInt(XmlTrasformazione.GetAttribute("sa_cod")), _
                                                CInt(Id_Trasformazione), _
                                                CStr(XmlTrasformazione.GetAttribute("trasformazione_des")), _
                                                CInt(XmlTrasformazione.GetAttribute("stato")), _
                                                CInt(XmlTrasformazione.GetAttribute("linea_cod")), _
                                                CInt(XmlTrasformazione.GetAttribute("preparazione_cod")), _
                                                CInt(XmlTrasformazione.GetAttribute("id_agenda")), _
                                                CStr(XmlTrasformazione.GetAttribute("cau_mov")), _
                                                CInt(XmlTrasformazione.GetAttribute("step_giorni")), _
                                                CDate(XmlTrasformazione.GetAttribute("ora1")), _
                                                CDate(XmlTrasformazione.GetAttribute("ora2")), _
                                                CDate(XmlTrasformazione.GetAttribute("ora3")), _
                                                CDate(XmlTrasformazione.GetAttribute("ora4")), _
                                                CInt(XmlTrasformazione.GetAttribute("colore")), _
                                                CInt(XmlTrasformazione.GetAttribute("monitor")), _
                                                CStr(XmlTrasformazione.GetAttribute("note")), _
                                                CInt(XmlTrasformazione.GetAttribute("tipo_stima")), _
                                                CInt(XmlTrasformazione.GetAttribute("elem_cod")), _
                                                CInt(XmlTrasformazione.GetAttribute("mat_cod")), _
                                                CInt(XmlTrasformazione.GetAttribute("udm_cod")), _
                                                CDbl(XmlTrasformazione.GetAttribute("stima")), _
                                                CDate(XmlTrasformazione.GetAttribute("validita_inizio")), _
                                                CDate(XmlTrasformazione.GetAttribute("validita_fine")), _
                                                objParametri)



                        Case "2"    'MODIFICA -------------------------------------------------------

                            ObjTrasformazione.Modifica( _
                                                CStr(XmlTrasformazione.GetAttribute("piva")), _
                                                CInt(XmlTrasformazione.GetAttribute("sa_cod")), _
                                                CInt(Id_Trasformazione), _
                                                CStr(XmlTrasformazione.GetAttribute("trasformazione_des")), _
                                                CInt(XmlTrasformazione.GetAttribute("stato")), _
                                                CInt(XmlTrasformazione.GetAttribute("linea_cod")), _
                                                CInt(XmlTrasformazione.GetAttribute("preparazione_cod")), _
                                                CInt(XmlTrasformazione.GetAttribute("id_agenda")), _
                                                CStr(XmlTrasformazione.GetAttribute("cau_mov")), _
                                                CInt(XmlTrasformazione.GetAttribute("step_giorni")), _
                                                CDate(XmlTrasformazione.GetAttribute("ora1")), _
                                                CDate(XmlTrasformazione.GetAttribute("ora2")), _
                                                CDate(XmlTrasformazione.GetAttribute("ora3")), _
                                                CDate(XmlTrasformazione.GetAttribute("ora4")), _
                                                CInt(XmlTrasformazione.GetAttribute("colore")), _
                                                CInt(XmlTrasformazione.GetAttribute("monitor")), _
                                                CStr(XmlTrasformazione.GetAttribute("note")), _
                                                CInt(XmlTrasformazione.GetAttribute("tipo_stima")), _
                                                CInt(XmlTrasformazione.GetAttribute("elem_cod")), _
                                                CInt(XmlTrasformazione.GetAttribute("mat_cod")), _
                                                CInt(XmlTrasformazione.GetAttribute("udm_cod")), _
                                                CDbl(XmlTrasformazione.GetAttribute("stima")), _
                                                CDate(XmlTrasformazione.GetAttribute("validita_inizio")), _
                                                CDate(XmlTrasformazione.GetAttribute("validita_fine")), _
                                                "", _
                                                objParametri)

                        Case "3"    'CANCELLAZIONE -------------------------------------------------------

                            ObjTrasformazione.Cancella( _
                                                CStr(XmlTrasformazione.GetAttribute("piva")), _
                                                0, _
                                                Id_Trasformazione, _
                                                "", _
                                                objParametri)

                            'ObjContrattoxTrasformazioni = CreateObject("Agro_Contab_AD.Imprese_ContrattixTras_W")
                            ObjContrattoxTrasformazioni = New AgronicaCoreContabDAL.Imprese_ContrattixTras_W

                            ObjContrattoxTrasformazioni.Cancella( _
                                                CStr(XmlTrasformazione.GetAttribute("piva")), _
                                                0, _
                                                0, _
                                                0, _
                                                Id_Trasformazione, _
                                                "", _
                                                objParametri)

                            ObjContrattoxTrasformazioni = Nothing

                    End Select

                    ObjTrasformazione = Nothing


                    '==================================================================================






                    '#########################################################
                    '############   CONTRATTI X TRASFORMAZIONI  ##############
                    '#########################################################

                    'Prelevo l'elenco dei dati trasformazioni associate al contratto
                    XmlDatiContrattoxTrasformazioni = XmlTrasformazione.GetElementsByTagName("DatiContrattixTrasformazioni")


                    If XmlDatiContrattoxTrasformazioni.Count > 0 Then


                        If OpeDB_Trasformazione = "2" Then

                            'Cancellazione Preventiva delle relazioni contrattixtrasformazioni

                            'Creo l'oggetto COM
                            'ObjContrattoxTrasformazioni = CreateObject("Agro_Contab_AD.Imprese_ContrattixTras_W")
                            ObjContrattoxTrasformazioni = New AgronicaCoreContabDAL.Imprese_ContrattixTras_W

                            ObjContrattoxTrasformazioni.Cancella( _
                                                CStr(XmlTrasformazione.GetAttribute("piva")), _
                                                0, _
                                                0, _
                                                0, _
                                                Id_Trasformazione, _
                                                "", _
                                                objParametri)


                            ObjContrattoxTrasformazioni = Nothing

                        End If


                        i_DatiContrattixTrasformazioni = 0

                        Do While i_DatiContrattixTrasformazioni < XmlDatiContrattoxTrasformazioni.Count

                            'Prelevo l'elenco delle trasformazioni associate al contratto
                            xDatiContrattixTrasformazione = XmlDatiContrattoxTrasformazioni.Item(i_DatiContrattixTrasformazioni)

                            xDatiContrattixTrasformazioni = xDatiContrattixTrasformazione.GetElementsByTagName("ContrattoxTrasformazione")

                            i_ContrattixTrasformazioni = 0

                            Do While i_ContrattixTrasformazioni < xDatiContrattixTrasformazioni.Count

                                'Prelevo l'i-esimo riferimento
                                xContrattoxTrasformazione = xDatiContrattixTrasformazioni.Item(i_ContrattixTrasformazioni)

                                'Prelevo gli attributi del riferimento selezionato
                                OpeDB_ContrattoxTrasformazione = xContrattoxTrasformazione.GetAttribute("TipoOperazioneDB")

                                'Creo l'oggetto COM
                                'ObjContrattoxTrasformazioni = CreateObject("Agro_Contab_AD.Imprese_ContrattixTras_W")
                                ObjContrattoxTrasformazioni = New AgronicaCoreContabDAL.Imprese_ContrattixTras_W


                                'Verifico l'operazione richiesta
                                Select Case OpeDB_ContrattoxTrasformazione

                                    Case "0"    'LEGGI -------------------------------------------------------

                                    Case "1"    'SALVA -------------------------------------------------------

                                        'Salvo il riferimento
                                        Dummy = ObjContrattoxTrasformazioni.Scrivi( _
                                                CStr(xContrattoxTrasformazione.GetAttribute("piva")), _
                                                CInt(xContrattoxTrasformazione.GetAttribute("contratto_cod")), _
                                                CInt(xContrattoxTrasformazione.GetAttribute("progetto_cod")), _
                                                CInt(xContrattoxTrasformazione.GetAttribute("fase_cod")), _
                                                Id_Trasformazione, _
                                                CInt(xContrattoxTrasformazione.GetAttribute("udm_cod")), _
                                                CDbl(xContrattoxTrasformazione.GetAttribute("qta")), _
                                                CDate(xContrattoxTrasformazione.GetAttribute("validita_inizio")), _
                                                CDate(xContrattoxTrasformazione.GetAttribute("validita_fine")), _
                                                objParametri)

                                    Case "2"    'MODIFICA -------------------------------------------------------

                                        ObjContrattoxTrasformazioni.Modifica( _
                                                CStr(xContrattoxTrasformazione.GetAttribute("piva")), _
                                                CInt(xContrattoxTrasformazione.GetAttribute("contratto_cod")), _
                                                CInt(xContrattoxTrasformazione.GetAttribute("progetto_cod")), _
                                                CInt(xContrattoxTrasformazione.GetAttribute("fase_cod")), _
                                                Id_Trasformazione, _
                                                CInt(xContrattoxTrasformazione.GetAttribute("udm_cod")), _
                                                CDbl(xContrattoxTrasformazione.GetAttribute("qta")), _
                                                CDate(xContrattoxTrasformazione.GetAttribute("validita_inizio")), _
                                                CDate(xContrattoxTrasformazione.GetAttribute("validita_fine")), _
                                                "", _
                                                objParametri)



                                    Case "3"    'ELIMINA -------------------------------------------------------

                                        ObjContrattoxTrasformazioni.Cancella( _
                                                CStr(xContrattoxTrasformazione.GetAttribute("piva")), _
                                                CInt(xContrattoxTrasformazione.GetAttribute("contratto_cod")), _
                                                CInt(xContrattoxTrasformazione.GetAttribute("progetto_cod")), _
                                                CInt(xContrattoxTrasformazione.GetAttribute("fase_cod")), _
                                                Id_Trasformazione, _
                                                "", _
                                                objParametri)

                                End Select


                                'Elimino l'oggetto
                                ObjContrattoxTrasformazioni = Nothing

                                'Incremento l'indice
                                i_ContrattixTrasformazioni += 1

                            Loop

                            i_DatiContrattixTrasformazioni += 1

                        Loop

                    End If

                Else

                    'Nessuna Gestione Parametri Processi Produttivi/Trasformazione
                    Id_Trasformazione = 0

                End If

            Else

                'Nessuna Gestione Parametri Processi Produttivi/Trasformazione
                Id_Trasformazione = 0

            End If



            '------------------------------
            '------------------------------
            '------------------------------

            'Elimino tutti gli oggetti utilizzati

            ObjSequenze = Nothing

            XmlDoc = Nothing

            '------------------------------

            'Restituisco un valore Dummy
            xRisp = True

            'Se ho la transazione è stata avviata in questa routine faccio il commit
            If FlagTransazioneLocale Then
                objParametri.objTransazione.Commit()
            End If

            '----------------------------------------------------------------------------

        Catch ex As Exception

            'Restituisco un valore Dummy
            xRisp = False

            'Faccio il rollback della transazione
            If objParametri.objTransazione IsNot Nothing Then
                objParametri.objTransazione.Rollback()
                objParametri.objTransazione = Nothing
            End If

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        Finally

            'Chiudo la connessione se è stata aperta in questa routine
            If FlagConnessioneLocale AndAlso objParametri.objConnessione IsNot Nothing Then
                objParametri.objConnessione.Close()
                objParametri.objConnessione.Dispose()
            End If

        End Try

        Return xRisp

    End Function

    ''============================================================================
    'Public Function Trasformazioni_Scrivi( _
    '                                ByVal DatiTrasformazioni As String, _
    '                                    ByVal PivaSuperUser As String, _
    '                                    ByVal Username_Operazione As String, _
    '                                    ByVal FinestraTemp_Inizio As Date, _
    '                                    ByVal FinestraTemp_Fine As Date, _
    '                                    ByVal FlagVisibilita As Int32, _
    '                                    ByVal FlagCancellazioneLogica As Int32, _
    '                                    ByRef objConnessione As DbConnection, _
    '                                    ByRef objTransazione As DbTransaction, _
    '                                    ByVal StringaConnessione As String, _
    '                                    ByVal DirectoryLOG As String, _
    '                                    ByVal FileLOG As String, _
    '                                    ByVal IdentificatoreUtente As String) _
    '                                        As Boolean

    '    '----------------------------------------------------------------------

    '    Dim NomeRoutine As String = "AgronicaCoreContabBIZ.Trasformazioni_W.Trasformazioni_Scrivi()"

    '    '----------------------------------------------------------------------

    '    Dim Dummy As Boolean

    '    '   Dim ObjSequenze                  As Agro_Contab_AD.Agro_Sequenze
    '    '   Dim ObjTrasformazione            As Agro_Contab_AD.Trasformazioni_W
    '    '   Dim ObjContrattoxTrasformazioni  As Agro_Contab_AD.Imprese_ContrattixTras_W

    '    Dim ObjSequenze As AgronicaCoreDataProvider.Agro_Sequenze
    '    Dim ObjTrasformazione As AgronicaCoreContabDAL.Trasformazioni_W
    '    Dim ObjContrattoxTrasformazioni As AgronicaCoreContabDAL.Imprese_ContrattixTras_W

    '    Dim XmlDoc As XmlDocument

    '    Dim XmlDatiTrasformazioni As XmlNodeList                'IXMLDOMNodeList
    '    Dim XmlDatiTrasformazione As XmlElement                 'IXMLDOMElement
    '    Dim XmlTrasformazioni As XmlNodeList                    'IXMLDOMNodeList
    '    Dim XmlTrasformazione As XmlElement                     'IXMLDOMElement
    '    Dim XmlDatiContrattoxTrasformazioni As XmlNodeList      'IXMLDOMNodeList
    '    Dim xDatiContrattixTrasformazioni As XmlNodeList        'IXMLDOMNodeList
    '    Dim xDatiContrattixTrasformazione As XmlElement         'IXMLDOMElement
    '    Dim xContrattoxTrasformazione As XmlElement             'IXMLDOMElement

    '    Dim i_DatiTrasformazioni As Integer
    '    Dim i_Trasformazione As Integer
    '    Dim i_DatiContrattixTrasformazioni As Integer
    '    Dim i_ContrattixTrasformazioni As Integer
    '    Dim i_ContrattoxTrasformazione As Integer

    '    Dim OpeDB_Trasformazione As String
    '    Dim OpeDB_ContrattoxTrasformazione As String

    '    Dim Id_Trasformazione As Integer


    '    '------------------------------
    '    Dim FlagTransazioneLocale As Boolean = False
    '    Dim FlagConnessioneLocale As Boolean = False

    '    Dim MessaggioErrore As String = ""
    '    Dim xRisp As Boolean = True
    '    '------------------------------



    '    Try

    '        '------------------------------

    '        'Se la connessione è chiusa la apro
    '        If objConnessione Is Nothing Then
    '            'Richiedo una connessione
    '            objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(StringaConnessione)
    '            objConnessione.Open()
    '            FlagConnessioneLocale = True
    '        End If

    '        If objTransazione Is Nothing Then
    '            'Inizializzo la transazione
    '            objTransazione = objConnessione.BeginTransaction
    '            FlagTransazioneLocale = True
    '        End If

    '        '------------------------------

    '        XmlDoc = New Xml.XmlDocument
    '        'XmlDoc.async = False
    '        XmlDoc.LoadXml(DatiTrasformazioni)

    '        '------------------------------
    '        '------------------------------
    '        '------------------------------


    '        '#####################################################################################
    '        '#####################  Gestione Parametri Produzione/Trasformazione  ################
    '        '#####################################################################################

    '        'Prelevo l'elenco dei parametri
    '        XmlDatiTrasformazioni = XmlDoc.GetElementsByTagName("DatiTrasformazioni")

    '        If XmlDatiTrasformazioni.Count > 0 Then

    '            'Creo gli Oggetti COM+

    '            'ObjTrasformazione = CreateObject("Agro_Contab_AD.Trasformazioni_W")
    '            'ObjSequenze = CreateObject("Agro_Contab_AD.Agro_Sequenze")

    '            ObjTrasformazione = New AgronicaCoreContabDAL.Trasformazioni_W
    '            ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

    '            XmlDatiTrasformazione = XmlDatiTrasformazioni.Item(0)



    '            '#####################################################################################
    '            '################################  Trasformazione  ###################################
    '            '#####################################################################################

    '            'Prelevo l'eventuale elenco dei dettagli della Trasformazione
    '            XmlTrasformazioni = XmlDatiTrasformazione.GetElementsByTagName("Trasformazione")

    '            If XmlTrasformazioni.Count > 0 Then

    '                XmlTrasformazione = XmlTrasformazioni.Item(0)

    '                'Prelevo gli attributi della trasformazione selezionata
    '                OpeDB_Trasformazione = XmlTrasformazione.GetAttribute("TipoOperazioneDB")

    '                'Creo l'oggetto COM
    '                'ObjTrasformazione = CreateObject("Agro_Contab_AD.Trasformazioni_W")
    '                ObjTrasformazione = New AgronicaCoreContabDAL.Trasformazioni_W

    '                Id_Trasformazione = Agro_SQL_SaveNum(XmlTrasformazione.GetAttribute("id_trasformazione"))

    '                'Verifico l'operazione richiesta
    '                Select Case OpeDB_Trasformazione
    '                    '
    '                Case "0"    'LEGGI -------------------------------------------------------
    '                        '
    '                    Case "1"    'SALVA -------------------------------------------------------


    '                        If Id_Trasformazione <= 0 Then

    '                            'ObjSequenze = CreateObject("Agro_Contab_AD.Agro_Sequenze")
    '                            ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

    '                            'Richiedo un nuovo codice trasformazione
    '                            Id_Trasformazione = ObjSequenze.NuovoId_Tabella( _
    '                                            "Trasformazioni", _
    '                                            CStr(Username_Operazione), _
    '                                            CInt(XmlTrasformazione.GetAttribute("basecode")), _
    '                                            CInt(XmlTrasformazione.GetAttribute("topcode")), _
    '                                            objConnessione, _
    '                                            objTransazione, _
    '                                            StringaConnessione, _
    '                                            FlagVisibilita, _
    '                                            DirectoryLOG, _
    '                                            FileLOG, _
    '                                            IdentificatoreUtente)

    '                            ObjSequenze = Nothing

    '                        Else

    '                            'Si è verificata 1 delle seguenti ipotesi:
    '                            '1.Fase Linea Produttiva (Collegamento tra + operazioni di agenda aventi il medesimo id_trasformazione e linea_cod)
    '                            '2.Esportazione x Modulo StandAlone

    '                        End If


    '                        Dummy = ObjTrasformazione.Scrivi( _
    '                                            CStr(XmlTrasformazione.GetAttribute("piva")), _
    '                                            CInt(XmlTrasformazione.GetAttribute("sa_cod")), _
    '                                            CInt(Id_Trasformazione), _
    '                                            CStr(XmlTrasformazione.GetAttribute("trasformazione_des")), _
    '                                            CInt(XmlTrasformazione.GetAttribute("stato")), _
    '                                            CInt(XmlTrasformazione.GetAttribute("linea_cod")), _
    '                                            CInt(XmlTrasformazione.GetAttribute("preparazione_cod")), _
    '                                            CInt(XmlTrasformazione.GetAttribute("id_agenda")), _
    '                                            CStr(XmlTrasformazione.GetAttribute("cau_mov")), _
    '                                            CInt(XmlTrasformazione.GetAttribute("step_giorni")), _
    '                                            CDate(XmlTrasformazione.GetAttribute("ora1")), CDate(XmlTrasformazione.GetAttribute("ora2")), _
    '                                            CDate(XmlTrasformazione.GetAttribute("ora3")), CDate(XmlTrasformazione.GetAttribute("ora4")), _
    '                                            CInt(XmlTrasformazione.GetAttribute("colore")), _
    '                                            CInt(XmlTrasformazione.GetAttribute("monitor")), _
    '                                            CStr(XmlTrasformazione.GetAttribute("note")), _
    '                                            CInt(XmlTrasformazione.GetAttribute("tipo_stima")), _
    '                                            CInt(XmlTrasformazione.GetAttribute("elem_cod")), _
    '                                            CInt(XmlTrasformazione.GetAttribute("mat_cod")), _
    '                                            CInt(XmlTrasformazione.GetAttribute("udm_cod")), _
    '                                            CDbl(XmlTrasformazione.GetAttribute("stima")), _
    '                                            CStr(Username_Operazione), _
    '                                            CDate(XmlTrasformazione.GetAttribute("validita_inizio")), _
    '                                            CDate(XmlTrasformazione.GetAttribute("validita_fine")), _
    '                                            objConnessione, _
    '                                            objTransazione, _
    '                                            StringaConnessione, _
    '                                            DirectoryLOG, _
    '                                            FileLOG, _
    '                                            IdentificatoreUtente)


    '                    Case "2"    'MODIFICA -------------------------------------------------------



    '                        ObjTrasformazione.Modifica( _
    '                                            CStr(XmlTrasformazione.GetAttribute("piva")), _
    '                                            CInt(XmlTrasformazione.GetAttribute("sa_cod")), _
    '                                            CInt(Id_Trasformazione), _
    '                                            CStr(XmlTrasformazione.GetAttribute("trasformazione_des")), _
    '                                            CInt(XmlTrasformazione.GetAttribute("stato")), _
    '                                            CInt(XmlTrasformazione.GetAttribute("linea_cod")), _
    '                                            CInt(XmlTrasformazione.GetAttribute("preparazione_cod")), _
    '                                            CInt(XmlTrasformazione.GetAttribute("id_agenda")), _
    '                                            CStr(XmlTrasformazione.GetAttribute("cau_mov")), _
    '                                            CInt(XmlTrasformazione.GetAttribute("step_giorni")), _
    '                                            CDate(XmlTrasformazione.GetAttribute("ora1")), CDate(XmlTrasformazione.GetAttribute("ora2")), _
    '                                            CDate(XmlTrasformazione.GetAttribute("ora3")), CDate(XmlTrasformazione.GetAttribute("ora4")), _
    '                                            CInt(XmlTrasformazione.GetAttribute("colore")), _
    '                                            CInt(XmlTrasformazione.GetAttribute("monitor")), _
    '                                            CStr(XmlTrasformazione.GetAttribute("note")), _
    '                                            CInt(XmlTrasformazione.GetAttribute("tipo_stima")), _
    '                                            CInt(XmlTrasformazione.GetAttribute("elem_cod")), _
    '                                            CInt(XmlTrasformazione.GetAttribute("mat_cod")), _
    '                                            CInt(XmlTrasformazione.GetAttribute("udm_cod")), _
    '                                            CDbl(XmlTrasformazione.GetAttribute("stima")), _
    '                                            CStr(Username_Operazione), _
    '                                            CDate(XmlTrasformazione.GetAttribute("validita_inizio")), _
    '                                            CDate(XmlTrasformazione.GetAttribute("validita_fine")), _
    '                                            objConnessione, _
    '                                            objTransazione, _
    '                                            StringaConnessione, _
    '                                            DirectoryLOG, _
    '                                            FileLOG, _
    '                                            IdentificatoreUtente)



    '                    Case "3"    'CANCELLAZIONE -------------------------------------------------------


    '                        ObjTrasformazione.Cancella( _
    '                                            CStr(XmlTrasformazione.GetAttribute("piva")), _
    '                                            0, _
    '                                            Id_Trasformazione, _
    '                                            CStr(Username_Operazione), _
    '                                            FlagCancellazioneLogica, _
    '                                            objConnessione, _
    '                                            objTransazione, _
    '                                            StringaConnessione, _
    '                                            DirectoryLOG, _
    '                                            FileLOG, _
    '                                            IdentificatoreUtente)


    '                        'ObjContrattoxTrasformazioni = CreateObject("Agro_Contab_AD.Imprese_ContrattixTras_W")
    '                        ObjContrattoxTrasformazioni = New AgronicaCoreContabDAL.Imprese_ContrattixTras_W

    '                        ObjContrattoxTrasformazioni.Cancella( _
    '                                            CStr(XmlTrasformazione.GetAttribute("piva")), _
    '                                            0, _
    '                                            0, _
    '                                            0, _
    '                                            Id_Trasformazione, _
    '                                            CStr(Username_Operazione), _
    '                                            FlagCancellazioneLogica, _
    '                                            objConnessione, _
    '                                            objTransazione, _
    '                                            StringaConnessione, _
    '                                            DirectoryLOG, _
    '                                            FileLOG, _
    '                                            IdentificatoreUtente)

    '                        ObjContrattoxTrasformazioni = Nothing


    '                End Select

    '                ObjTrasformazione = Nothing


    '                '==================================================================================






    '                '#########################################################
    '                '############   CONTRATTI X TRASFORMAZIONI  ##############
    '                '#########################################################

    '                'Prelevo l'elenco dei dati trasformazioni associate al contratto
    '                XmlDatiContrattoxTrasformazioni = XmlTrasformazione.GetElementsByTagName("DatiContrattixTrasformazioni")


    '                If XmlDatiContrattoxTrasformazioni.Count > 0 Then


    '                    If OpeDB_Trasformazione = "2" Then

    '                        'Cancellazione Preventiva delle relazioni contrattixtrasformazioni

    '                        'Creo l'oggetto COM
    '                        'ObjContrattoxTrasformazioni = CreateObject("Agro_Contab_AD.Imprese_ContrattixTras_W")
    '                        ObjContrattoxTrasformazioni = New AgronicaCoreContabDAL.Imprese_ContrattixTras_W

    '                        ObjContrattoxTrasformazioni.Cancella( _
    '                                            CStr(XmlTrasformazione.GetAttribute("piva")), _
    '                                            0, _
    '                                            0, _
    '                                            0, _
    '                                            Id_Trasformazione, _
    '                                            CStr(Username_Operazione), _
    '                                            FlagCancellazioneLogica, _
    '                                            objConnessione, _
    '                                            objTransazione, _
    '                                            StringaConnessione, _
    '                                            DirectoryLOG, _
    '                                            FileLOG, _
    '                                            IdentificatoreUtente)


    '                        ObjContrattoxTrasformazioni = Nothing

    '                    End If


    '                    i_DatiContrattixTrasformazioni = 0

    '                    Do While i_DatiContrattixTrasformazioni < XmlDatiContrattoxTrasformazioni.Count

    '                        'Prelevo l'elenco delle trasformazioni associate al contratto
    '                        xDatiContrattixTrasformazione = XmlDatiContrattoxTrasformazioni.Item(i_DatiContrattixTrasformazioni)

    '                        xDatiContrattixTrasformazioni = xDatiContrattixTrasformazione.GetElementsByTagName("ContrattoxTrasformazione")

    '                        i_ContrattixTrasformazioni = 0

    '                        Do While i_ContrattixTrasformazioni < xDatiContrattixTrasformazioni.Count

    '                            'Prelevo l'i-esimo riferimento
    '                            xContrattoxTrasformazione = xDatiContrattixTrasformazioni.Item(i_ContrattixTrasformazioni)

    '                            'Prelevo gli attributi del riferimento selezionato
    '                            OpeDB_ContrattoxTrasformazione = xContrattoxTrasformazione.GetAttribute("TipoOperazioneDB")

    '                            'Creo l'oggetto COM
    '                            'ObjContrattoxTrasformazioni = CreateObject("Agro_Contab_AD.Imprese_ContrattixTras_W")
    '                            ObjContrattoxTrasformazioni = New AgronicaCoreContabDAL.Imprese_ContrattixTras_W


    '                            'Verifico l'operazione richiesta
    '                            Select Case OpeDB_ContrattoxTrasformazione
    '                                '
    '                            Case "0"    'LEGGI -------------------------------------------------------
    '                                    '
    '                                Case "1"    'SALVA -------------------------------------------------------
    '                                    '


    '                                    'Salvo il riferimento
    '                                    Dummy = ObjContrattoxTrasformazioni.Scrivi( _
    '                                            CStr(xContrattoxTrasformazione.GetAttribute("piva")), _
    '                                            CInt(xContrattoxTrasformazione.GetAttribute("contratto_cod")), _
    '                                            CInt(xContrattoxTrasformazione.GetAttribute("progetto_cod")), _
    '                                            CInt(xContrattoxTrasformazione.GetAttribute("fase_cod")), _
    '                                            Id_Trasformazione, _
    '                                            CInt(xContrattoxTrasformazione.GetAttribute("udm_cod")), _
    '                                            CDbl(xContrattoxTrasformazione.GetAttribute("qta")), _
    '                                            CStr(Username_Operazione), _
    '                                            CDate(xContrattoxTrasformazione.GetAttribute("validita_inizio")), _
    '                                            CDate(xContrattoxTrasformazione.GetAttribute("validita_fine")), _
    '                                            objConnessione, _
    '                                            objTransazione, _
    '                                            StringaConnessione, _
    '                                            DirectoryLOG, _
    '                                            FileLOG, _
    '                                            IdentificatoreUtente)
    '                                    '
    '                                Case "2"    'MODIFICA -------------------------------------------------------
    '                                    '
    '                                    ObjContrattoxTrasformazioni.Modifica( _
    '                                            CStr(xContrattoxTrasformazione.GetAttribute("piva")), _
    '                                            CInt(xContrattoxTrasformazione.GetAttribute("contratto_cod")), _
    '                                            CInt(xContrattoxTrasformazione.GetAttribute("progetto_cod")), _
    '                                            CInt(xContrattoxTrasformazione.GetAttribute("fase_cod")), _
    '                                            Id_Trasformazione, _
    '                                            CInt(xContrattoxTrasformazione.GetAttribute("udm_cod")), _
    '                                            CDbl(xContrattoxTrasformazione.GetAttribute("qta")), _
    '                                            CStr(Username_Operazione), _
    '                                            CDate(xContrattoxTrasformazione.GetAttribute("validita_inizio")), _
    '                                            CDate(xContrattoxTrasformazione.GetAttribute("validita_fine")), _
    '                                            objConnessione, _
    '                                            objTransazione, _
    '                                            StringaConnessione, _
    '                                            DirectoryLOG, _
    '                                            FileLOG, _
    '                                            IdentificatoreUtente)


    '                                Case "3"    'ELIMINA -------------------------------------------------------
    '                                    '

    '                                    ObjContrattoxTrasformazioni.Cancella( _
    '                                            CStr(xContrattoxTrasformazione.GetAttribute("piva")), _
    '                                            CInt(xContrattoxTrasformazione.GetAttribute("contratto_cod")), _
    '                                            CInt(xContrattoxTrasformazione.GetAttribute("progetto_cod")), _
    '                                            CInt(xContrattoxTrasformazione.GetAttribute("fase_cod")), _
    '                                            Id_Trasformazione, _
    '                                            CStr(Username_Operazione), _
    '                                            FlagCancellazioneLogica, _
    '                                            objConnessione, _
    '                                            objTransazione, _
    '                                            StringaConnessione, _
    '                                            DirectoryLOG, _
    '                                            FileLOG, _
    '                                            IdentificatoreUtente)

    '                            End Select


    '                            'Elimino l'oggetto
    '                            ObjContrattoxTrasformazioni = Nothing

    '                            'Incremento l'indice
    '                            i_ContrattixTrasformazioni = i_ContrattixTrasformazioni + 1

    '                        Loop

    '                        i_DatiContrattixTrasformazioni = i_DatiContrattixTrasformazioni + 1

    '                    Loop

    '                End If

    '            Else

    '                'Nessuna Gestione Parametri Processi Produttivi/Trasformazione
    '                Id_Trasformazione = 0

    '            End If

    '        Else

    '            'Nessuna Gestione Parametri Processi Produttivi/Trasformazione
    '            Id_Trasformazione = 0

    '        End If



    '        '------------------------------
    '        '------------------------------
    '        '------------------------------

    '        'Elimino tutti gli oggetti utilizzati

    '        ObjSequenze = Nothing

    '        XmlDoc = Nothing

    '        '------------------------------

    '        'Restituisco un valore Dummy
    '        xRisp = True

    '        'Se ho la transazione è stata avviata in questa routine faccio il commit
    '        If FlagTransazioneLocale = True Then
    '            objTransazione.Commit()
    '        End If

    '        '----------------------------------------------------------------------------

    '    Catch ex As Exception

    '        'Restituisco un valore Dummy
    '        xRisp = False

    '        'Faccio il rollback della transazione
    '        If Not objTransazione Is Nothing Then
    '            objTransazione.Rollback()
    '            objTransazione = Nothing
    '        End If

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    Finally

    '        'Chiudo la connessione se è stata aperta in questa routine
    '        If (FlagConnessioneLocale = True) AndAlso (Not objConnessione Is Nothing) Then
    '            objConnessione.Close()
    '        End If

    '    End Try

    '    Return xRisp

    'End Function


End Class