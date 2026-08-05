Imports System.Data.OleDb
Imports System.Xml
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreEntityFramework
Imports System.Transactions
Imports AgronicaCoreDTOStd.InData.Agenda

Public Class Agenda_R
    Inherits AgronicaCoreDataProvider.LogProvider


    '============================================================================
    Public Function Agenda_Leggi(ByVal Piva As String,
                                 ByVal Sa_Cod As Integer,
                                 ByVal Id_Agenda As Integer,
                                 ByVal Lav_Cod As Integer,
                                 ByVal ForDelete As Boolean,
                                 ByRef objParametri As AgronicaCoreParametri,
                                 Optional ByVal LeggiRiferimentiInversi As Boolean = True,
                                 Optional ByVal TipoG2G As Integer = 0,
                                 Optional ByVal xFiltroAggiuntivo As String = ""
                                 ) As String

        Const nomeRoutine = "ContabBIZ.Agenda_R.Agenda_Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese dell'utente
        '====================================================================================

        Dim messaggioErrore As String = ""

        Dim FlagConnessioneLocale As Boolean = False

        Dim i, j As Int32

        Dim RisultatoFunzione As String = String.Empty

        Dim XmlDoc As XmlDocument
        Dim XmlDatiMovimenti As XmlDocument

        Dim XmlDatiAgenda As XmlElement
        Dim XmlAgenda As XmlElement
        Dim XmlDatiNote As XmlElement
        Dim XmlNota As XmlElement
        Dim xDatiMovimenti As XmlNodeList
        Dim xDatiMovimento As XmlElement

        Dim ObjAgenda As AgronicaCoreContabDAL.Agenda_R
        Dim ObjNote As AgronicaCoreContabDAL.AgendaxNote_R
        Dim objMovimenti As AgronicaCoreContabBIZ.Movimenti_R

        Dim DtAgenda As DataTable

        Dim DatiMovimenti As String

        '------------------------------

        Try

            '------------------------------
            'Verifico se e' stata impostata una connessione
            If IsNothing(objParametri.objConnessione) Then
                'Flag
                FlagConnessioneLocale = True
                'Creo la connessione localmente
                objParametri.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
                objParametri.objConnessione.Open()
            End If
            If objParametri.objConnessione.State = ConnectionState.Closed Then
                'Flag
                FlagConnessioneLocale = True
                'Creo la connessione localmente
                objParametri.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
                objParametri.objConnessione.Open()
            End If
            '------------------------------

            'Mi procuro un elenco dei Movimenti di Agenda associati all'Impresa
            'all'interno della finestra temporale selezionata

            ObjAgenda = New AgronicaCoreContabDAL.Agenda_R

            'Mi procuro il RecordSet richiesto
            DtAgenda = ObjAgenda.Leggi(CStr(Piva),
                                       CInt(Sa_Cod),
                                       CInt(Id_Agenda),
                                       CInt(Lav_Cod),
                                       enumSelezioneVariabile.Selezione_TabellaCompleta,
                                       xFiltroAggiuntivo,
                                       "",
                                       objParametri,
                                       TipoG2G)

            'Se ottengo almeno un risultato, creo la struttura XML
            If DtAgenda.Rows.Count > 0 Then

                '----- < Documento XML > -----
                XmlDoc = New XmlDocument

                XmlDatiAgenda = XmlDoc.CreateElement("DatiAgenda")

                'Effettuo un ciclo 
                For i = 0 To DtAgenda.Rows.Count - 1

                    '----- < AGENDA > -----
                    XmlAgenda = XmlDoc.CreateElement("Agenda")

                    If TipoG2G = 3 Then

                        With XmlAgenda
                            .SetAttribute("TipoOperazioneDB", "3")
                            .SetAttribute("piva", Agro_SQL_Load(DtAgenda.Rows(i).Item("PIVA")))
                            .SetAttribute("sa_cod", Agro_SQL_Load(DtAgenda.Rows(i).Item("Sa_Cod")))
                            .SetAttribute("id_agenda", Agro_SQL_Load(DtAgenda.Rows(i).Item("Id_Agenda")))
                            .SetAttribute("lav_cod", "-1000")
                            .SetAttribute("validita_inizio", "01/01/1900")
                            .SetAttribute("des_lib", "G2G Delete")
                            .SetAttribute("raccoglitore_cod", "0")
                        End With

                        XmlDatiAgenda.AppendChild(XmlAgenda)
                        '----- < / AGENDA > -----

                    Else

                        With XmlAgenda
                            .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                            .SetAttribute("piva", Agro_SQL_Load(DtAgenda.Rows(i).Item("PIVA")))
                            .SetAttribute("sa_cod", Agro_SQL_Load(DtAgenda.Rows(i).Item("Sa_Cod")))
                            .SetAttribute("id_agenda", Agro_SQL_Load(DtAgenda.Rows(i).Item("Id_Agenda")))
                            .SetAttribute("lav_cod", Agro_SQL_Load(DtAgenda.Rows(i).Item("Lav_Cod")))
                            .SetAttribute("linea_cod", Agro_SQL_Load(DtAgenda.Rows(i).Item("Linea_Cod")))
                            .SetAttribute("preparazione_cod", Agro_SQL_Load(DtAgenda.Rows(i).Item("Preparazione_Cod")))
                            .SetAttribute("id_trasformazione", Agro_SQL_Load(DtAgenda.Rows(i).Item("Id_Trasformazione")))
                            .SetAttribute("des_lib", Agro_SQL_Load(DtAgenda.Rows(i).Item("Des_Lib")))
                            .SetAttribute("tipo_accettazione", Agro_SQL_Load(DtAgenda.Rows(i).Item("Tipo_Accettazione")))
                            .SetAttribute("validita_inizio", Agro_SQL_Load(DtAgenda.Rows(i).Item("Validita_Inizio")))
                            .SetAttribute("validita_fine", Agro_SQL_Load(DtAgenda.Rows(i).Item("Validita_Fine")))
                            .SetAttribute("blocco_flag", Agro_SQL_Load(DtAgenda.Rows(i).Item("Blocco_Flag")))
                            .SetAttribute("blocco_data", Agro_SQL_Load(DtAgenda.Rows(i).Item("Blocco_Data")))
                            .SetAttribute("blocco_username", Agro_SQL_Load(DtAgenda.Rows(i).Item("Blocco_Username")))

                            .SetAttribute("data_creazione", Agro_SQL_Load(DtAgenda.Rows(i).Item("data_creazione")))
                            .SetAttribute("data_modifica", Agro_SQL_Load(DtAgenda.Rows(i).Item("data_modifica")))
                            .SetAttribute("username_creazione", Agro_SQL_Load(DtAgenda.Rows(i).Item("username_creazione")))
                            .SetAttribute("username_modifica", Agro_SQL_Load(DtAgenda.Rows(i).Item("username_modifica")))

                            .SetAttribute("Audit_Cod", Agro_SQL_Load(DtAgenda.Rows(i).Item("Audit_Cod")))
                            .SetAttribute("stato_export", Agro_SQL_Load(DtAgenda.Rows(i).Item("Stato_Export")))
                            .SetAttribute("stato_export_2", Agro_SQL_Load(DtAgenda.Rows(i).Item("Stato_Export_2")))
                            .SetAttribute("tipo_visibilita", Agro_SQL_Load(DtAgenda.Rows(i).Item("Tipo_Visibilita")))
                            .SetAttribute("chkcoge_manuale", Agro_SQL_Load(DtAgenda.Rows(i).Item("ChkCoge_Manuale")))
                            .SetAttribute("id_attivita", Agro_SQL_Load(DtAgenda.Rows(i).Item("Id_Attivita")))
                            .SetAttribute("modulo", Agro_SQL_Load(DtAgenda.Rows(i).Item("Modulo")))
                            .SetAttribute("raccoglitore_cod", Agro_SQL_Load(DtAgenda.Rows(i).Item("Raccoglitore_Cod")))
                            .SetAttribute("split", Agro_SQL_Load(DtAgenda.Rows(i).Item("Split")))
                            .SetAttribute("stato_cod", Agro_SQL_Load(DtAgenda.Rows(i).Item("Stato_Cod")))
                            .SetAttribute("daremoto", Agro_SQL_Load(DtAgenda.Rows(i).Item("DaRemoto")))

                        End With


                        '#########################################################
                        '##########  NOTE  #######################################
                        '#########################################################

                        ObjNote = New AgronicaCoreContabDAL.AgendaxNote_R
                        Dim dtNote As DataTable

                        dtNote = ObjNote.Leggi(CInt(Agro_SQL_Load(DtAgenda.Rows(i).Item("Id_Agenda"))),
                                          0,
                                          AGRODATAINIZIO,
                                          AGRODATAFINE,
                                          enumSelezioneVariabile.Selezione_TabellaCompleta,
                                          "",
                                          "",
                                          objParametri)

                        'Se ottengo almeno un risultato, creo la struttura XML
                        If dtNote.Rows.Count > 0 Then

                            XmlDatiNote = XmlDoc.CreateElement("DatiNote")

                            'Effettuo un ciclo sui Dettagli Tecnici del Movimento
                            For j = 0 To dtNote.Rows.Count - 1

                                '----- < NOTA > -----
                                XmlNota = XmlDoc.CreateElement("Nota")

                                With XmlNota
                                    .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                    .SetAttribute("id_agenda", Agro_SQL_Load(dtNote.Rows(j).Item("Id_Agenda")))
                                    .SetAttribute("nota_cod", Agro_SQL_Load(dtNote.Rows(j).Item("nota_cod")))
                                    .SetAttribute("validita_inizio", Agro_SQL_Load(dtNote.Rows(j).Item("validita_inizio")))
                                    .SetAttribute("validita_fine", Agro_SQL_Load(dtNote.Rows(j).Item("validita_fine")))

                                    .SetAttribute("data_creazione", Agro_SQL_Load(dtNote.Rows(j).Item("data_creazione")))
                                    .SetAttribute("data_modifica", Agro_SQL_Load(dtNote.Rows(j).Item("data_modifica")))
                                    .SetAttribute("username_creazione", Agro_SQL_Load(dtNote.Rows(j).Item("username_creazione")))
                                    .SetAttribute("username_modifica", Agro_SQL_Load(dtNote.Rows(j).Item("username_modifica")))


                                End With

                                XmlDatiNote.AppendChild(XmlNota)
                                '----- < / NOTA > -----

                            Next

                            XmlAgenda.AppendChild(XmlDatiNote)

                        End If

                        'XmlDatiNote = Nothing
                        XmlNota = Nothing
                        dtNote.Dispose()
                        ObjNote = Nothing



                        ''            If Agro_SQL_Load(RsAgenda("Id_Trasformazione")) <> 0 Then
                        ''
                        ''
                        ''               '########################################
                        ''               '##########  TRASFORMAZIONE  ############
                        ''               '########################################
                        ''
                        ''               'Mi procuro i dettagli della trasformazione associata all'operazione di agenda
                        ''
                        ''               'Creo l'oggetto COM+
                        ''               Set objTrasformazioni = CreateObject("Agro_Contab.Trasformazioni_R")
                        ''
                        ''               DatiTrasformazioni = objTrasformazioni.Trasformazioni_Leggi( _
                        ''                           CStr(RsAgenda("PIVA")), _
                        ''                           CInt(RsAgenda("Id_Trasformazione")), _
                        ''                           , , _
                        ''                           objCnManager, _
                        ''                           ForDelete, _
                        ''                           ConnessioneAlternativa)
                        ''
                        ''
                        ''              Set XmlDatiTrasformazioni = CreateObject("MSXML2.DOMDocument.4.0")
                        ''              XmlDatiTrasformazioni.async = False
                        ''              XmlDatiTrasformazioni.loadXML DatiTrasformazioni
                        ''
                        ''              Set xDatiTrasformazioni = XmlDatiTrasformazioni.getElementsByTagName("DatiTrasformazioni")
                        ''
                        ''              i_DatiTrasformazioni = 0
                        ''
                        ''              Do While i_DatiTrasformazioni < xDatiTrasformazioni.length
                        ''                 Set xDatiTrasformazione = xDatiTrasformazioni.Item(i_DatiTrasformazioni)
                        ''                 XmlAgenda.appendChild xDatiTrasformazione
                        ''                 i_DatiTrasformazioni = i_DatiTrasformazioni + 1
                        ''              Loop
                        ''
                        ''              '----- < / TRASFORMAZIONE  > -----
                        ''
                        ''              Set xDatiTrasformazioni = Nothing
                        ''              Set xDatiTrasformazioni = Nothing
                        ''              Set xDatiTrasformazione = Nothing
                        ''              Set objTrasformazioni = Nothing
                        ''
                        ''
                        ''              '#################################
                        ''              '#################################
                        ''              '#################################
                        ''
                        ''
                        ''         End If




                        '#####################################
                        '##########  MOVIMENTI   #############
                        '#####################################

                        'Mi procuro un elenco dei movimenti
                        objMovimenti = New AgronicaCoreContabBIZ.Movimenti_R


                        DatiMovimenti = objMovimenti.Movimento_Leggi(
                                    CStr(DtAgenda.Rows(i).Item("PIVA")),
                                    0,
                                    CInt(DtAgenda.Rows(i).Item("Id_Agenda")),
                                    0,
                                    0,
                                    "",
                                    ForDelete,
                                    CDate(DtAgenda.Rows(i).Item("validita_inizio")),
                                    CDate(DtAgenda.Rows(i).Item("validita_fine")),
                                    objParametri,
                                    leggiRiferimentiInversi:=LeggiRiferimentiInversi)

                        XmlAgenda.InnerXml = XmlAgenda.InnerXml & DatiMovimenti

                        'XmlDatiMovimenti = New XmlDocument
                        ''XmlDatiMovimenti.async = False
                        'XmlDatiMovimenti.LoadXml(DatiMovimenti)

                        'xDatiMovimenti = XmlDatiMovimenti.GetElementsByTagName("DatiMovimenti")

                        'i_DatiMovimenti = 0

                        'Do While i_DatiMovimenti < xDatiMovimenti.Count
                        '    xDatiMovimento = xDatiMovimenti.Item(i_DatiMovimenti)
                        '    XmlAgenda.AppendChild(xDatiMovimento)
                        '    i_DatiMovimenti = i_DatiMovimenti + 1
                        'Loop

                        '----- < / MOVIMENTI  > -----

                        XmlDatiNote = Nothing
                        XmlDatiMovimenti = Nothing
                        xDatiMovimenti = Nothing
                        xDatiMovimento = Nothing
                        objMovimenti = Nothing


                        '#################################
                        '#################################
                        '#################################

                        XmlDatiAgenda.AppendChild(XmlAgenda)
                        '----- < / AGENDA > -----


                    End If

                Next

                XmlDoc.AppendChild(XmlDatiAgenda)

                RisultatoFunzione = XmlDoc.OuterXml
                '----- < / Documento XML > -----

                XmlAgenda = Nothing
                XmlDatiAgenda = Nothing
                XmlDoc = Nothing

            Else

                'Altrimenti, se non risulta selezionato nessun contatto ...
                RisultatoFunzione = ""

            End If


            'Elimino gli oggetti che ho creato
            DtAgenda.Dispose()
            DtAgenda = Nothing
            ObjAgenda = Nothing

            '------------------------------

        Catch ex As Exception

            RisultatoFunzione = ""
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        Finally

            If FlagConnessioneLocale = True Then
                If Not IsNothing(objParametri.objConnessione) Then
                    objParametri.objConnessione.Close()
                    objParametri.objConnessione.Dispose()
                End If
            End If

        End Try

        'Restituisco il risultato
        Return RisultatoFunzione

    End Function

    Public Function Agenda_LeggiReverse(ByVal Piva As String,
                                 ByVal Sa_Cod As Integer,
                                 ByVal Id_Agenda As Integer,
                                 ByVal Lav_Cod As Integer,
                                 ByVal ForDelete As Boolean,
                                 ByRef objParametri As AgronicaCoreParametri,
                                 Optional ByVal LeggiRiferimentiInversi As Boolean = True,
                                 Optional ByVal TipoG2G As Integer = 0,
                                 Optional ByVal xFiltroAggiuntivo As String = "",
                                 Optional ByVal From_PivaSuperUser As String = ""
                                 ) As String

        Const nomeRoutine = "ContabBIZ.Agenda_R.Agenda_LeggiReverse()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese dell'utente
        '====================================================================================

        Dim messaggioErrore As String = ""

        Dim FlagConnessioneLocale As Boolean = False

        Dim i, j As Int32

        Dim RisultatoFunzione As String = String.Empty

        Dim XmlDoc As XmlDocument
        Dim XmlDatiMovimenti As XmlDocument

        Dim XmlDatiAgenda As XmlElement
        Dim XmlAgenda As XmlElement
        Dim XmlDatiNote As XmlElement
        Dim XmlNota As XmlElement
        Dim xDatiMovimenti As XmlNodeList
        Dim xDatiMovimento As XmlElement

        Dim ObjAgenda As AgronicaCoreContabDAL.Agenda_R
        Dim ObjNote As AgronicaCoreContabDAL.AgendaxNote_R
        Dim objMovimenti As AgronicaCoreContabBIZ.Movimenti_R

        Dim DtAgenda As DataTable

        Dim DatiMovimenti As String

        '------------------------------

        Try

            '------------------------------
            'Verifico se e' stata impostata una connessione
            If IsNothing(objParametri.objConnessione) Then
                'Flag
                FlagConnessioneLocale = True
                'Creo la connessione localmente
                objParametri.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
                objParametri.objConnessione.Open()
            End If
            If objParametri.objConnessione.State = ConnectionState.Closed Then
                'Flag
                FlagConnessioneLocale = True
                'Creo la connessione localmente
                objParametri.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
                objParametri.objConnessione.Open()
            End If
            '------------------------------

            'Mi procuro un elenco dei Movimenti di Agenda associati all'Impresa
            'all'interno della finestra temporale selezionata

            ObjAgenda = New AgronicaCoreContabDAL.Agenda_R

            'Mi procuro il RecordSet richiesto
            DtAgenda = ObjAgenda.LeggiReverse(CStr(Piva),
                                       CInt(Sa_Cod),
                                       CInt(Id_Agenda),
                                       CInt(Lav_Cod),
                                       enumSelezioneVariabile.Selezione_TabellaCompleta,
                                       xFiltroAggiuntivo,
                                       "",
                                       objParametri,
                                       TipoG2G, From_PivaSuperUser:=From_PivaSuperUser)

            'Se ottengo almeno un risultato, creo la struttura XML
            If DtAgenda.Rows.Count > 0 Then

                '----- < Documento XML > -----
                XmlDoc = New XmlDocument

                XmlDatiAgenda = XmlDoc.CreateElement("DatiAgenda")

                'Effettuo un ciclo 
                For i = 0 To DtAgenda.Rows.Count - 1

                    '----- < AGENDA > -----
                    XmlAgenda = XmlDoc.CreateElement("Agenda")

                    If TipoG2G = 3 Then

                        With XmlAgenda
                            .SetAttribute("TipoOperazioneDB", "3")
                            .SetAttribute("piva", Agro_SQL_Load(DtAgenda.Rows(i).Item("PIVA")))
                            .SetAttribute("sa_cod", Agro_SQL_Load(DtAgenda.Rows(i).Item("Sa_Cod")))
                            .SetAttribute("id_agenda", Agro_SQL_Load(DtAgenda.Rows(i).Item("Id_Agenda")))
                            .SetAttribute("lav_cod", "-1000")
                            .SetAttribute("validita_inizio", "01/01/1900")
                            .SetAttribute("des_lib", "G2G Delete")
                            .SetAttribute("raccoglitore_cod", "0")
                        End With

                        XmlDatiAgenda.AppendChild(XmlAgenda)
                        '----- < / AGENDA > -----

                    Else

                        With XmlAgenda
                            .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                            .SetAttribute("piva", Agro_SQL_Load(DtAgenda.Rows(i).Item("PIVA")))
                            .SetAttribute("sa_cod", Agro_SQL_Load(DtAgenda.Rows(i).Item("Sa_Cod")))
                            .SetAttribute("id_agenda", Agro_SQL_Load(DtAgenda.Rows(i).Item("Id_Agenda")))
                            .SetAttribute("lav_cod", Agro_SQL_Load(DtAgenda.Rows(i).Item("Lav_Cod")))
                            .SetAttribute("linea_cod", Agro_SQL_Load(DtAgenda.Rows(i).Item("Linea_Cod")))
                            .SetAttribute("preparazione_cod", Agro_SQL_Load(DtAgenda.Rows(i).Item("Preparazione_Cod")))
                            .SetAttribute("id_trasformazione", Agro_SQL_Load(DtAgenda.Rows(i).Item("Id_Trasformazione")))
                            .SetAttribute("des_lib", Agro_SQL_Load(DtAgenda.Rows(i).Item("Des_Lib")))
                            .SetAttribute("tipo_accettazione", Agro_SQL_Load(DtAgenda.Rows(i).Item("Tipo_Accettazione")))
                            .SetAttribute("validita_inizio", Agro_SQL_Load(DtAgenda.Rows(i).Item("Validita_Inizio")))
                            .SetAttribute("validita_fine", Agro_SQL_Load(DtAgenda.Rows(i).Item("Validita_Fine")))
                            .SetAttribute("blocco_flag", Agro_SQL_Load(DtAgenda.Rows(i).Item("Blocco_Flag")))
                            .SetAttribute("blocco_data", Agro_SQL_Load(DtAgenda.Rows(i).Item("Blocco_Data")))
                            .SetAttribute("blocco_username", Agro_SQL_Load(DtAgenda.Rows(i).Item("Blocco_Username")))

                            .SetAttribute("data_creazione", Agro_SQL_Load(DtAgenda.Rows(i).Item("data_creazione")))
                            .SetAttribute("data_modifica", Agro_SQL_Load(DtAgenda.Rows(i).Item("data_modifica")))
                            .SetAttribute("username_creazione", Agro_SQL_Load(DtAgenda.Rows(i).Item("username_creazione")))
                            .SetAttribute("username_modifica", Agro_SQL_Load(DtAgenda.Rows(i).Item("username_modifica")))

                            .SetAttribute("Audit_Cod", Agro_SQL_Load(DtAgenda.Rows(i).Item("Audit_Cod")))
                            .SetAttribute("stato_export", Agro_SQL_Load(DtAgenda.Rows(i).Item("Stato_Export")))
                            .SetAttribute("stato_export_2", Agro_SQL_Load(DtAgenda.Rows(i).Item("Stato_Export_2")))
                            .SetAttribute("tipo_visibilita", Agro_SQL_Load(DtAgenda.Rows(i).Item("Tipo_Visibilita")))
                            .SetAttribute("chkcoge_manuale", Agro_SQL_Load(DtAgenda.Rows(i).Item("ChkCoge_Manuale")))
                            .SetAttribute("id_attivita", Agro_SQL_Load(DtAgenda.Rows(i).Item("Id_Attivita")))
                            .SetAttribute("modulo", Agro_SQL_Load(DtAgenda.Rows(i).Item("Modulo")))
                            .SetAttribute("raccoglitore_cod", Agro_SQL_Load(DtAgenda.Rows(i).Item("Raccoglitore_Cod")))
                            .SetAttribute("split", Agro_SQL_Load(DtAgenda.Rows(i).Item("Split")))
                            .SetAttribute("stato_cod", Agro_SQL_Load(DtAgenda.Rows(i).Item("Stato_Cod")))
                            .SetAttribute("daremoto", Agro_SQL_Load(DtAgenda.Rows(i).Item("DaRemoto")))

                        End With


                        '#########################################################
                        '##########  NOTE  #######################################
                        '#########################################################

                        ObjNote = New AgronicaCoreContabDAL.AgendaxNote_R
                        Dim dtNote As DataTable

                        dtNote = ObjNote.Leggi(CInt(Agro_SQL_Load(DtAgenda.Rows(i).Item("Id_Agenda"))),
                                          0,
                                          AGRODATAINIZIO,
                                          AGRODATAFINE,
                                          enumSelezioneVariabile.Selezione_TabellaCompleta,
                                          "",
                                          "",
                                          objParametri)

                        'Se ottengo almeno un risultato, creo la struttura XML
                        If dtNote.Rows.Count > 0 Then

                            XmlDatiNote = XmlDoc.CreateElement("DatiNote")

                            'Effettuo un ciclo sui Dettagli Tecnici del Movimento
                            For j = 0 To dtNote.Rows.Count - 1

                                '----- < NOTA > -----
                                XmlNota = XmlDoc.CreateElement("Nota")

                                With XmlNota
                                    .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                    .SetAttribute("id_agenda", Agro_SQL_Load(dtNote.Rows(j).Item("Id_Agenda")))
                                    .SetAttribute("nota_cod", Agro_SQL_Load(dtNote.Rows(j).Item("nota_cod")))
                                    .SetAttribute("validita_inizio", Agro_SQL_Load(dtNote.Rows(j).Item("validita_inizio")))
                                    .SetAttribute("validita_fine", Agro_SQL_Load(dtNote.Rows(j).Item("validita_fine")))

                                    .SetAttribute("data_creazione", Agro_SQL_Load(dtNote.Rows(j).Item("data_creazione")))
                                    .SetAttribute("data_modifica", Agro_SQL_Load(dtNote.Rows(j).Item("data_modifica")))
                                    .SetAttribute("username_creazione", Agro_SQL_Load(dtNote.Rows(j).Item("username_creazione")))
                                    .SetAttribute("username_modifica", Agro_SQL_Load(dtNote.Rows(j).Item("username_modifica")))


                                End With

                                XmlDatiNote.AppendChild(XmlNota)
                                '----- < / NOTA > -----

                            Next

                            XmlAgenda.AppendChild(XmlDatiNote)

                        End If

                        'XmlDatiNote = Nothing
                        XmlNota = Nothing
                        dtNote.Dispose()
                        ObjNote = Nothing



                        ''            If Agro_SQL_Load(RsAgenda("Id_Trasformazione")) <> 0 Then
                        ''
                        ''
                        ''               '########################################
                        ''               '##########  TRASFORMAZIONE  ############
                        ''               '########################################
                        ''
                        ''               'Mi procuro i dettagli della trasformazione associata all'operazione di agenda
                        ''
                        ''               'Creo l'oggetto COM+
                        ''               Set objTrasformazioni = CreateObject("Agro_Contab.Trasformazioni_R")
                        ''
                        ''               DatiTrasformazioni = objTrasformazioni.Trasformazioni_Leggi( _
                        ''                           CStr(RsAgenda("PIVA")), _
                        ''                           CInt(RsAgenda("Id_Trasformazione")), _
                        ''                           , , _
                        ''                           objCnManager, _
                        ''                           ForDelete, _
                        ''                           ConnessioneAlternativa)
                        ''
                        ''
                        ''              Set XmlDatiTrasformazioni = CreateObject("MSXML2.DOMDocument.4.0")
                        ''              XmlDatiTrasformazioni.async = False
                        ''              XmlDatiTrasformazioni.loadXML DatiTrasformazioni
                        ''
                        ''              Set xDatiTrasformazioni = XmlDatiTrasformazioni.getElementsByTagName("DatiTrasformazioni")
                        ''
                        ''              i_DatiTrasformazioni = 0
                        ''
                        ''              Do While i_DatiTrasformazioni < xDatiTrasformazioni.length
                        ''                 Set xDatiTrasformazione = xDatiTrasformazioni.Item(i_DatiTrasformazioni)
                        ''                 XmlAgenda.appendChild xDatiTrasformazione
                        ''                 i_DatiTrasformazioni = i_DatiTrasformazioni + 1
                        ''              Loop
                        ''
                        ''              '----- < / TRASFORMAZIONE  > -----
                        ''
                        ''              Set xDatiTrasformazioni = Nothing
                        ''              Set xDatiTrasformazioni = Nothing
                        ''              Set xDatiTrasformazione = Nothing
                        ''              Set objTrasformazioni = Nothing
                        ''
                        ''
                        ''              '#################################
                        ''              '#################################
                        ''              '#################################
                        ''
                        ''
                        ''         End If




                        '#####################################
                        '##########  MOVIMENTI   #############
                        '#####################################

                        'Mi procuro un elenco dei movimenti
                        objMovimenti = New AgronicaCoreContabBIZ.Movimenti_R


                        DatiMovimenti = objMovimenti.Movimento_Leggi(
                                    CStr(DtAgenda.Rows(i).Item("PIVA")),
                                    0,
                                    CInt(DtAgenda.Rows(i).Item("Id_Agenda")),
                                    0,
                                    0,
                                    "",
                                    ForDelete,
                                    CDate(DtAgenda.Rows(i).Item("validita_inizio")),
                                    CDate(DtAgenda.Rows(i).Item("validita_fine")),
                                    objParametri,
                                    leggiRiferimentiInversi:=LeggiRiferimentiInversi)

                        XmlAgenda.InnerXml = XmlAgenda.InnerXml & DatiMovimenti

                        'XmlDatiMovimenti = New XmlDocument
                        ''XmlDatiMovimenti.async = False
                        'XmlDatiMovimenti.LoadXml(DatiMovimenti)

                        'xDatiMovimenti = XmlDatiMovimenti.GetElementsByTagName("DatiMovimenti")

                        'i_DatiMovimenti = 0

                        'Do While i_DatiMovimenti < xDatiMovimenti.Count
                        '    xDatiMovimento = xDatiMovimenti.Item(i_DatiMovimenti)
                        '    XmlAgenda.AppendChild(xDatiMovimento)
                        '    i_DatiMovimenti = i_DatiMovimenti + 1
                        'Loop

                        '----- < / MOVIMENTI  > -----

                        XmlDatiNote = Nothing
                        XmlDatiMovimenti = Nothing
                        xDatiMovimenti = Nothing
                        xDatiMovimento = Nothing
                        objMovimenti = Nothing


                        '#################################
                        '#################################
                        '#################################

                        XmlDatiAgenda.AppendChild(XmlAgenda)
                        '----- < / AGENDA > -----


                    End If

                Next

                XmlDoc.AppendChild(XmlDatiAgenda)

                RisultatoFunzione = XmlDoc.OuterXml
                '----- < / Documento XML > -----

                XmlAgenda = Nothing
                XmlDatiAgenda = Nothing
                XmlDoc = Nothing

            Else

                'Altrimenti, se non risulta selezionato nessun contatto ...
                RisultatoFunzione = ""

            End If


            'Elimino gli oggetti che ho creato
            DtAgenda.Dispose()
            DtAgenda = Nothing
            ObjAgenda = Nothing

            '------------------------------

        Catch ex As Exception

            RisultatoFunzione = ""
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        Finally

            If FlagConnessioneLocale = True Then
                If Not IsNothing(objParametri.objConnessione) Then
                    objParametri.objConnessione.Close()
                    objParametri.objConnessione.Dispose()
                End If
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


Public Class Agenda_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Agenda_Scrivi(ByVal DatiAgenda As String,
                                  ByRef OUTPUT_ID_Agenda As Integer,
                                  ByVal Flag_Mirror As Integer,
                                  ByVal Id_Servizio As Integer,
                                  ByVal Rimappa_Codici As Integer,
                                  ByVal Piva_SuperUser_Origine As String,
                                  ByRef objParametri As AgronicaCoreParametri,
                                  Optional ByRef CodiciRimappati As String = "",
                                  Optional ByVal G2G As Boolean = False
                                  ) As Boolean

        '============================================================================
        'Restituisce in uscita il codice del nuovo "ID_Agenda" appena creato : OUTPUT_ID_Agenda
        '============================================================================

        '----------------------------------------------------------------------
        Dim nomeRoutine As String = "AgronicaCoreContabBIZ.Agenda_W.Agenda_Scrivi()"
        '----------------------------------------------------------------------

        Dim dummy As Boolean

        Dim objSequenze As New Agro_Sequenze
        Dim objAgenda As AgronicaCoreContabDAL.Agenda_W
        Dim objNote As AgronicaCoreContabDAL.AgendaxNote_W
        Dim objRicette As AgronicaCoreContabDAL.RicettexAgenda_W
        Dim objMovimenti As AgronicaCoreContabBIZ.Movimenti_W
        Dim objTrasformazioneR As AgronicaCoreContabBIZ.Trasformazioni_R
        Dim objTrasformazioneW As AgronicaCoreContabBIZ.Trasformazioni_W
        Dim objAgendaTrasformazione As AgronicaCoreContabDAL.Trasformazioni_R

        Dim objAgendaMirrorR As AgronicaCoreContabDAL.Agenda_Mirror_R
        Dim objAgendaMirrorW As AgronicaCoreContabDAL.Agenda_Mirror_W
        Dim objAgronicaLogAgendaW As New AgronicaCoreContabDAL.AgronicaLogAgenda_W


        Dim xmlDoc As XmlDocument

        Dim xmlNote As XmlNodeList
        Dim xmlNota As XmlElement
        Dim xmlRicette As XmlNodeList
        Dim xmlRicetta As XmlElement
        Dim xmlDatiMovimenti As XmlNodeList
        Dim xmlDatiMovimento As XmlElement
        Dim xmlDatiTrasformazioni As XmlNodeList
        Dim xmlDatiTrasformazione As XmlElement
        Dim xmlTrasformazioni As XmlNodeList
        Dim xmlTrasformazione As XmlElement


        Dim dtAgenda As DataTable
        Dim Cod_Agenda As Integer

        Dim Linea_Cod As Integer
        Dim Preparazione_Cod As Integer
        Dim Id_Trasformazione As Integer

        Dim xDatiAgende As XmlNodeList
        Dim xDatiAgenda As XmlElement
        Dim xAgende As XmlNodeList
        Dim xAgenda As XmlElement

        Dim i_DatiAgenda As Integer
        Dim i_Agenda As Integer
        Dim Progressivo_Mirror As Integer
        Dim intDummy As Integer


        Dim OpeDB_Agenda As String
        Dim OpeDB_Nota As String
        Dim OpeDB_Ricetta As String

        Dim datiMovimenti As String
        Dim datiTrasformazioni As String

        Dim DtTrasformazioni As DataTable
        Dim Lav_Cod As Integer


        Dim vCodiciRimappati As String() = CodiciRimappati.Split("|")


        '------------------------------
        Dim flagTransazioneLocale As Boolean = False
        Dim flagConnessioneLocale As Boolean = False

        Dim messaggioErrore As String = ""
        Dim xRisp As Boolean = True
        '------------------------------


        Try

            '------------------------------

            'Se la connessione è chiusa la apro
            If objParametri.objConnessione Is Nothing Then
                'Richiedo una connessione
                objParametri.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
                objParametri.objConnessione.Open()
                flagConnessioneLocale = True
            ElseIf objParametri.objConnessione.State = ConnectionState.Closed Then
                objParametri.objConnessione.ConnectionString = objParametri.StringaConnessione
                objParametri.objConnessione.Open()
                flagConnessioneLocale = True
            End If

            If objParametri.objTransazione Is Nothing Then
                'Inizializzo la transazione
                objParametri.objTransazione = objParametri.objConnessione.BeginTransaction
                flagTransazioneLocale = True
            End If

            '------------------------------

            xmlDoc = New XmlDocument
            'xmlDoc.async = False
            xmlDoc.LoadXml(DatiAgenda)

            '------------------------------
            '------------------------------
            '------------------------------


            xDatiAgende = xmlDoc.GetElementsByTagName("DatiAgenda")

            i_DatiAgenda = 0

            Do While i_DatiAgenda < xDatiAgende.Count

                'Prelevo l'i-esimo blocco di DatiAgenda (in realtà ne esiste uno solo)
                xDatiAgenda = xDatiAgende.Item(i_DatiAgenda)


                '------------------------------


                xAgende = xDatiAgenda.GetElementsByTagName("Agenda")

                i_Agenda = 0

                Do While i_Agenda < xAgende.Count

                    'Prelevo l' i-esima Codifica Agenda
                    xAgenda = xAgende.Item(i_Agenda)

                    Dim xAgendaDataCreazione As DateTime = #2/1/1900#
                    Dim xAgendaDataModifica As DateTime = #2/1/1900#
                    Dim xAgendaUsernameCreazione As String = ""
                    Dim xAgendaUsernameModifica As String = ""

                    If Not IsNothing(xAgenda.GetAttribute("data_creazione")) AndAlso
                     xAgenda.GetAttribute("data_creazione") <> "" Then
                        xAgendaDataCreazione = CDate(xAgenda.GetAttribute("data_creazione"))
                    End If

                    If Not IsNothing(xAgenda.GetAttribute("data_modifica")) AndAlso
                     xAgenda.GetAttribute("data_modifica") <> "" Then
                        xAgendaDataModifica = CDate(xAgenda.GetAttribute("data_modifica"))
                    End If

                    If Not IsNothing(xAgenda.GetAttribute("username_creazione")) Then
                        xAgendaUsernameCreazione = CStr(xAgenda.GetAttribute("username_creazione"))
                    End If

                    If Not IsNothing(xAgenda.GetAttribute("username_modifica")) Then
                        xAgendaUsernameModifica = CStr(xAgenda.GetAttribute("username_modifica"))
                    End If



                    'Prelevo gli attributi dell'agenda selezionata
                    OpeDB_Agenda = xAgenda.GetAttribute("TipoOperazioneDB")

                    'Creo l'oggetto COM
                    objAgenda = New AgronicaCoreContabDAL.Agenda_W

                    'Inizializzo Preventivamente il Cod_Agenda
                    Cod_Agenda = CInt(xAgenda.GetAttribute("id_agenda"))

                    Lav_Cod = CInt(xAgenda.GetAttribute("lav_cod"))


                    '===========================================================================================
                    'Linea_Cod e Preparazione_Cod sono all'interno dell'Xml dell'Agenda

                    Linea_Cod = UtilityProvider.Agro_SQL_SaveNum(xAgenda.GetAttribute("linea_cod"))
                    Preparazione_Cod = UtilityProvider.Agro_SQL_SaveNum(xAgenda.GetAttribute("preparazione_cod"))

                    'Inizializzazione Id_Trasformazione
                    Id_Trasformazione = UtilityProvider.Agro_SQL_SaveNum(xAgenda.GetAttribute("id_trasformazione"))

                    'Vengono inizializzati in questo modo per gestire il valore 'null'
                    '===========================================================================================

                    'Verifico l'operazione richiesta
                    Select Case OpeDB_Agenda

                        Case "0"    'LEGGI -------------------------------------------------------
                            '
                        Case "1"    'SALVA -------------------------------------------------------
                            '
                            'IN CASO DI INSERIMENTO, NELLE TABELLE MIRROR NON VA FATTO NIENTE

                            If Cod_Agenda <= 0 Then

                                'Richiedo un nuovo codice movimento
                                Cod_Agenda = objSequenze.NuovoId_Tabella("Agenda",
                                                                         CInt(xAgenda.GetAttribute("basecode")),
                                                                         CInt(xAgenda.GetAttribute("topcode")),
                                                                         objParametri)

                                'OUTPUT_ID_Agenda = Cod_Agenda

                            Else

                                'Esportazione in Locale

                            End If

                            OUTPUT_ID_Agenda = Cod_Agenda
                            ImpostaRimappaturaCodici(vCodiciRimappati, "Agenda",
                                                     objParametri.PivaSuperUser & "," &
                                                     CStr(xAgenda.GetAttribute("piva")) & "," &
                                                     CStr(xAgenda.GetAttribute("sa_cod")) & "," &
                                                     Cod_Agenda)

                            '#####################################################################################
                            '#####################  Gestione Parametri Produzione/Trasformazione  ################
                            '#####################################################################################

                            'Prelevo l'elenco dei parametri
                            xmlDatiTrasformazioni = xAgenda.GetElementsByTagName("DatiTrasformazioni")

                            If xmlDatiTrasformazioni.Count > 0 Then

                                xmlDatiTrasformazione = xmlDatiTrasformazioni.Item(0)

                                'Prelevo l'eventuale elenco dei dettagli della Trasformazione
                                xmlTrasformazioni = xmlDatiTrasformazione.GetElementsByTagName("Trasformazione")

                                If xmlTrasformazioni.Count > 0 Then

                                    objTrasformazioneW = New AgronicaCoreContabBIZ.Trasformazioni_W

                                    'Impostazione Id_Trasformazione
                                    Id_Trasformazione = objTrasformazioneW.Trasformazioni_Scrivi(
                                                CStr(xAgenda.OuterXml),
                                                objParametri)

                                    objTrasformazioneW = Nothing

                                End If


                            End If
                            '==========================================================================================================

                            'Ricodifico il raccoglitore_cod
                            Dim inputRaccoglitore_Cod As Integer = Agro_XML_GetInteger(xAgenda, "raccoglitore_cod", 0)
                            Dim outpuRaccoglitore_Cod As Integer = 0

                            If inputRaccoglitore_Cod > 0 Then
                                outpuRaccoglitore_Cod = inputRaccoglitore_Cod

                                ImpostaRimappaturaCodici(vCodiciRimappati, "Raccoglitore",
                                                 objParametri.PivaSuperUser & "," &
                                                 CStr(xAgenda.GetAttribute("piva")) & "," &
                                                 outpuRaccoglitore_Cod)

                            ElseIf inputRaccoglitore_Cod = -1 Then

                                'Creo un nuovo raccoglitore
                                outpuRaccoglitore_Cod = objSequenze.NuovoId_Tabella("raccoglitore", CInt(xAgenda.GetAttribute("basecode")), CInt(xAgenda.GetAttribute("topcode")), objParametri)

                                ImpostaRimappaturaCodici(vCodiciRimappati, "Raccoglitore",
                                                 objParametri.PivaSuperUser & "," &
                                                 CStr(xAgenda.GetAttribute("piva")) & "," &
                                                 outpuRaccoglitore_Cod)

                            End If

                            '==========================================================================================================

                            dummy = objAgenda.Scrivi(CStr(xAgenda.GetAttribute("piva")),
                                                     CInt(xAgenda.GetAttribute("sa_cod")),
                                                     CInt(Cod_Agenda),
                                                     CInt(xAgenda.GetAttribute("lav_cod")),
                                                     Linea_Cod,
                                                     Preparazione_Cod,
                                                     Id_Trasformazione,
                                                     IIf(CDate(xAgenda.GetAttribute("validita_inizio")) > CDate(Now), "Pianificazione ", "") & CStr(xAgenda.GetAttribute("des_lib")),
                                                     Agro_SQL_SaveNum(xAgenda.GetAttribute("tipo_accettazione"), False),
                                                     Agro_XML_GetInteger(xAgenda, "blocco_flag", 0),
                                                     Agro_XML_GetString(xAgenda, "blocco_username", ""),
                                                     Agro_XML_GetDate(xAgenda, "blocco_data", AGRODATAINIZIO),
                                                     CDate(xAgenda.GetAttribute("validita_inizio")),
                                                     CDate(xAgenda.GetAttribute("validita_fine")),
                                                     objParametri,
                                                     xAgendaDataCreazione,
                                                     xAgendaDataModifica,
                                                     xAgendaUsernameCreazione,
                                                     xAgendaUsernameModifica,
                                                     Audit_Cod:=Agro_XML_GetInteger(xAgenda, "Audit_Cod", 0),
                                                     Stato_Export:=Agro_XML_GetInteger(xAgenda, "stato_export", 0),
                                                     Stato_Export_2:=Agro_XML_GetInteger(xAgenda, "stato_export_2", 0),
                                                     Tipo_Visibilita:=Agro_XML_GetInteger(xAgenda, "tipo_visibilita", 0),
                                                     ChkCoge_Manuale:=Agro_XML_GetInteger(xAgenda, "chkcoge_manuale", 0),
                                                     Id_Attivita:=Agro_XML_GetInteger(xAgenda, "id_attivita", 0),
                                                     Modulo:=Agro_XML_GetInteger(xAgenda, "modulo", 0),
                                                     Raccoglitore_Cod:=outpuRaccoglitore_Cod,
                                                     Split:=Agro_XML_GetInteger(xAgenda, "split", 0),
                                                     Origine:=Agro_XML_GetString(xAgenda, "origine", ""),
                                                     Stato_Cod:=Agro_XML_GetInteger(xAgenda, "stato_cod", 0),
                                                     DaRemoto:=Agro_XML_GetInteger(xAgenda, "daremoto", 0)
)


                            '************************************************
                            '************************************************
                            '*********** INIZIO LOGGING *******************
                            '************************************************


                            'MEMORIZZO L'INSERIMENTO DELL'OPERAZIONE NELLA TABELLA DI LOG
                            'è necessario lanciare lo script Inserimento tabella Agronica_Log_Agenda.sql (che è su Palladio)


                            'AGRONICA LOG AGENDA

                            'scrivo nella tabella mirror
                            intDummy = objAgronicaLogAgendaW.Scrivi(CDate(xAgenda.GetAttribute("validita_inizio")),
                                                                    CInt(OpeDB_Agenda),
                                                                    CStr(xAgenda.GetAttribute("des_lib")),
                                                                    CInt(Cod_Agenda),
                                                                    CStr(xAgenda.GetAttribute("piva")),
                                                                    CInt(xAgenda.GetAttribute("sa_cod")),
                                                                    CInt(xAgenda.GetAttribute("lav_cod")),
                                                                    CInt(Id_Servizio),
                                                                    objParametri)

                            '************************************************
                            '*********** FINE LOGGING *********************
                            '************************************************
                            '************************************************


                        Case "2"    'MODIFICA -------------------------------------------------------
                            '

                            ' QUESTO CASO NON E' MAI CONTEMPLATO, OCCORRE SOLO RIMAPPARE I CODICI X G2G                            
                            ImpostaRimappaturaCodici(vCodiciRimappati, "Agenda",
                                                     objParametri.PivaSuperUser & "," &
                                                     CStr(xAgenda.GetAttribute("piva")) & "," &
                                                     CStr(xAgenda.GetAttribute("sa_cod")) & "," &
                                                     Cod_Agenda)


                        Case "3"    'CANCELLA -------------------------------------------------------

                            '@MIRROR@

                            If Flag_Mirror = 1 Then

                                'LA CANCELLAZIONE E' GESTITA DOPO
                                'PER IL MOMENTO DEVO RICAVARE IL PROGRESSIVO_MIRROR
                                'DA PASSARE AL COMPONENTE MOVIMENTO_W


                                'AGENDA_MIRROR_R


                                objAgendaMirrorR = New AgronicaCoreContabDAL.Agenda_Mirror_R


                                'Mi procuro l'ultimo progressivo_mirror
                                dtAgenda = objAgendaMirrorR.MaxProgressivo_from_IdAgenda(
                                                CStr(xAgenda.GetAttribute("piva")),
                                                CStr(xAgenda.GetAttribute("sa_cod")),
                                                CInt(Cod_Agenda),
                                                "",
                                                objParametri)

                                'Se ottengo almeno un risultato
                                If dtAgenda.Rows.Count > 0 Then

                                    Progressivo_Mirror = CInt(dtAgenda.Rows(0).Item("Progressivo"))
                                Else
                                    Progressivo_Mirror = 0
                                End If

                                objAgendaMirrorR = Nothing

                                Progressivo_Mirror = Progressivo_Mirror + 1

                            End If 'Flag_Mirror

                            ImpostaRimappaturaCodici(vCodiciRimappati, "Agenda",
                                                     objParametri.PivaSuperUser & "," &
                                                     CStr(xAgenda.GetAttribute("piva")) & "," &
                                                     CStr(xAgenda.GetAttribute("sa_cod")) & "," &
                                                     Cod_Agenda)

                    End Select




                    '#####################################
                    '##########  NOTE   ##################
                    '#####################################

                    Dim Nota_Cod As Integer
                    Dim i_Note As Integer

                    xmlNote = xAgenda.GetElementsByTagName("Nota")

                    If Not xmlNote Is Nothing AndAlso xmlNote.Count > 0 Then

                        Do While i_Note < xmlNote.Count

                            xmlNota = xmlNote.Item(i_Note)

                            OpeDB_Nota = xmlNota.GetAttribute("TipoOperazioneDB")

                            Nota_Cod = CInt(xmlNota.GetAttribute("nota_cod"))

                            objNote = New AgronicaCoreContabDAL.AgendaxNote_W

                            'Verifico l'operazione richiesta
                            Select Case OpeDB_Nota

                                Case "0"    'LEGGI -------------------------------------------------------
                                    '
                                Case "1"    'SALVA -------------------------------------------------------

                                    dummy = objNote.Scrivi(Cod_Agenda,
                                                           Nota_Cod,
                                                           CDate(xmlNota.GetAttribute("validita_inizio")),
                                                           CDate(xmlNota.GetAttribute("validita_fine")),
                                                           objParametri,
                                                           xAgendaDataCreazione,
                                                           xAgendaDataModifica,
                                                           xAgendaUsernameCreazione,
                                                           xAgendaUsernameModifica)

                                Case "2"    'MODIFICA -------------------------------------------------------

                                    objNote.Modifica(Cod_Agenda,
                                                     Nota_Cod,
                                                     CDate(xmlNota.GetAttribute("validita_inizio")),
                                                     CDate(xmlNota.GetAttribute("validita_fine")),
                                                     "",
                                                     objParametri)

                                Case "3"    'ELIMINA -------------------------------------------------------

                                    objNote.Cancella(Cod_Agenda,
                                                     Nota_Cod,
                                                     "",
                                                     objParametri)

                            End Select

                            objNote = Nothing

                            i_Note = i_Note + 1

                        Loop

                    End If

                    '#####################################
                    '##########  RICETTA   ###############
                    '#####################################

                    Dim i_Ricette As Integer
                    Dim Ricetta_Cod As Integer
                    Dim Ricetta_Operazione_Cod As Integer

                    'Prelevo l'elenco dei movimenti
                    xmlRicette = xAgenda.GetElementsByTagName("Ricetta")

                    If Not xmlRicette Is Nothing AndAlso xmlRicette.Count > 0 Then

                        Do While i_Ricette < xmlRicette.Count

                            xmlRicetta = xmlRicette.Item(i_Ricette)

                            OpeDB_Ricetta = xmlRicetta.GetAttribute("TipoOperazioneDB")

                            Ricetta_Cod = CInt(xmlRicetta.GetAttribute("ricetta_cod"))
                            Ricetta_Operazione_Cod = CInt(xmlRicetta.GetAttribute("ricetta_operazione_cod"))

                            objRicette = New AgronicaCoreContabDAL.RicettexAgenda_W

                            'Verifico l'operazione richiesta
                            Select Case OpeDB_Ricetta

                                Case "0"    'LEGGI -------------------------------------------------------
                                    '
                                Case "1"    'SALVA -------------------------------------------------------

                                    dummy = objRicette.Scrivi(Ricetta_Cod,
                                                              Ricetta_Operazione_Cod,
                                                              Cod_Agenda,
                                                              CDate(xmlRicetta.GetAttribute("validita_inizio")),
                                                              CDate(xmlRicetta.GetAttribute("validita_fine")),
                                                              objParametri,
                                                              xAgendaDataCreazione,
                                                              xAgendaDataModifica,
                                                              xAgendaUsernameCreazione,
                                                              xAgendaUsernameModifica)

                                Case "2"    'MODIFICA -------------------------------------------------------

                                    objRicette.Modifica(Ricetta_Cod,
                                                        Ricetta_Operazione_Cod,
                                                        Cod_Agenda,
                                                        CDate(xmlRicetta.GetAttribute("validita_inizio")),
                                                        CDate(xmlRicetta.GetAttribute("validita_fine")),
                                                        "",
                                                        objParametri)

                                Case "3"    'ELIMINA -------------------------------------------------------

                                    objRicette.Cancella(Ricetta_Cod,
                                                        Ricetta_Operazione_Cod,
                                                        Cod_Agenda,
                                                        "",
                                                        objParametri)

                            End Select

                            objRicette = Nothing

                            i_Ricette = i_Ricette + 1

                        Loop

                    End If

                    '#####################################
                    '##########  MOVIMENTI   #############
                    '#####################################


                    'Prelevo l'elenco dei movimenti
                    xmlDatiMovimenti = xAgenda.GetElementsByTagName("DatiMovimenti")

                    If xmlDatiMovimenti.Count > 0 Then

                        xmlDatiMovimento = xmlDatiMovimenti.Item(0)

                        datiMovimenti = xmlDatiMovimento.OuterXml

                        'Creo l'oggetto COM

                        objMovimenti = New AgronicaCoreContabBIZ.Movimenti_W

                        Dim mCodiciRimappati As String = ""
                        If CodiciRimappati <> "" Then
                            mCodiciRimappati = String.Join("|", vCodiciRimappati)
                        End If

                        '@MIRROR@
                        dummy = objMovimenti.Movimento_Scrivi(CStr(datiMovimenti),
                                                                  CInt(Cod_Agenda),
                                                                  CInt(Lav_Cod),
                                                                  Progressivo_Mirror,
                                                                  Flag_Mirror,
                                                                  Rimappa_Codici,
                                                                  Piva_SuperUser_Origine,
                                                                  objParametri,
                                                                  xAgendaDataCreazione,
                                                                  xAgendaDataModifica,
                                                                  xAgendaUsernameCreazione,
                                                                  xAgendaUsernameModifica,
                                                                  CodiciRimappati:=mCodiciRimappati,
                                                                  G2G:=G2G)

                        If CodiciRimappati <> "" Then
                            vCodiciRimappati = mCodiciRimappati.Split("|")
                        End If

                        objMovimenti = Nothing

                    End If


                    Select Case OpeDB_Agenda


                        Case 3 'CANCELLAZIONE

                            '@MIRROR@

                            If Flag_Mirror = 1 Then

                                '************************************************
                                '************************************************
                                '*********** INIZIO MIRRORING *******************
                                '************************************************

                                'IN CASO DI CANCELLAZIONE, VA INSERITA L'OPERAZIONE NELLA TABELLA MIRROR
                                'E POI CANCELLATA NELLA TABELLA BUONA


                                'AGENDA_MIRROR_W

                                'Creo l'oggetto COM+
                                objAgendaMirrorW = New AgronicaCoreContabDAL.Agenda_Mirror_W

                                'scrivo nella tabella mirror
                                intDummy = objAgendaMirrorW.Scrivi(
                                                CStr(xAgenda.GetAttribute("piva")),
                                                CInt(xAgenda.GetAttribute("sa_cod")),
                                                CInt(Cod_Agenda),
                                                Progressivo_Mirror,
                                                CInt(xAgenda.GetAttribute("lav_cod")),
                                                Agro_SQL_SaveNum(xAgenda.GetAttribute("linea_cod"), False),
                                                Agro_SQL_SaveNum(xAgenda.GetAttribute("preparazione_cod"), False),
                                                Agro_SQL_SaveNum(xAgenda.GetAttribute("id_trasformazione"), False),
                                                CStr(xAgenda.GetAttribute("des_lib")),
                                                objParametri)

                                objAgendaMirrorW = Nothing


                                '************************************************
                                '*********** FINE MIRRORING *********************
                                '************************************************
                                '************************************************

                            End If 'Flag_Mirror


                            objAgenda.Cancella(CStr(xAgenda.GetAttribute("piva")),
                                               CInt(xAgenda.GetAttribute("sa_cod")),
                                               CInt(Cod_Agenda),
                                               "",
                                               objParametri)

                            objAgenda.CancellaRiferimentiAgenda(
                                                CStr(xAgenda.GetAttribute("piva")),
                                                CInt(xAgenda.GetAttribute("sa_cod")),
                                                CInt(Cod_Agenda),
                                                "",
                                                objParametri)


                            '############################################################################
                            '##########  Gestione Cancellazione Automatica Trasformazioni   #############
                            '############################################################################


                            If Id_Trasformazione <> 0 Then

                                'Controllo che non esistano più operazioni di agenda associate alla trasformazione
                                objAgendaTrasformazione = New AgronicaCoreContabDAL.Trasformazioni_R

                                DtTrasformazioni =
                                    objAgendaTrasformazione.LeggiTrasformazioni_Agenda(
                                                CStr(xAgenda.GetAttribute("piva")),
                                                0,
                                                Id_Trasformazione,
                                                0,
                                                0,
                                                0,
                                                enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                "",
                                                "",
                                                objParametri)

                                objAgendaTrasformazione = Nothing

                                If DtTrasformazioni.Rows.Count = 0 Then

                                    'Cancello la Trasformazione + le sue relazioni con gli Ordini poiché dati 'Ghost'

                                    objTrasformazioneR = New AgronicaCoreContabBIZ.Trasformazioni_R

                                    'Lettura Dati Trasformazioni in Cancellazione
                                    datiTrasformazioni =
                                         objTrasformazioneR.Trasformazioni_Leggi(
                                                CStr(xAgenda.GetAttribute("piva")),
                                                Id_Trasformazione,
                                                True,
                                                objParametri)


                                    'Creo l'oggetto COM
                                    objTrasformazioneW = New AgronicaCoreContabBIZ.Trasformazioni_W

                                    'Cancellazione Trasformazione + Dati Collegati
                                    dummy = objTrasformazioneW.Trasformazioni_Scrivi(
                                                datiTrasformazioni,
                                                objParametri)


                                    'Distruzione Oggetti
                                    objTrasformazioneR = Nothing
                                    objTrasformazioneW = Nothing


                                End If


                            End If


                            '############################################################################
                            '############################################################################
                            '############################################################################


                            '************************************************
                            '************************************************
                            '*********** INIZIO LOGGING *******************
                            '************************************************


                            'MEMORIZZO L'INSERIMENTO DELL'OPERAZIONE NELLA TABELLA DI LOG
                            'è necessario lanciare lo script Inserimento tabella Agronica_Log_Agenda.sql (che è su Palladio)


                            'AGRONICA LOG AGENDA

                            'scrivo nella tabella mirror
                            intDummy = objAgronicaLogAgendaW.Scrivi(CDate(xAgenda.GetAttribute("validita_inizio")),
                                                                    CInt(OpeDB_Agenda),
                                                                    CStr(xAgenda.GetAttribute("des_lib")),
                                                                    CInt(Cod_Agenda),
                                                                    CStr(xAgenda.GetAttribute("piva")),
                                                                    CInt(xAgenda.GetAttribute("sa_cod")),
                                                                    CInt(xAgenda.GetAttribute("lav_cod")),
                                                                    CInt(Id_Servizio),
                                                                    objParametri)

                            '************************************************
                            '*********** FINE LOGGING *********************
                            '************************************************
                            '************************************************

                    End Select


                    'Elimino l'oggetto
                    objAgenda = Nothing

                    '-------------------------------------------------------------

                    'Incremento l'indice
                    i_Agenda = i_Agenda + 1

                Loop

                '------------------------------

                'Incremento l'indice
                i_DatiAgenda = i_DatiAgenda + 1

            Loop

            '------------------------------
            '------------------------------
            '------------------------------

            'Elimino tutti gli oggetti utilizzati

            xDatiAgende = Nothing
            xDatiAgenda = Nothing
            xAgende = Nothing
            xAgenda = Nothing
            xmlDatiMovimenti = Nothing
            xmlDatiMovimento = Nothing
            xmlDatiTrasformazioni = Nothing
            xmlDatiTrasformazione = Nothing
            xmlTrasformazioni = Nothing
            xmlTrasformazione = Nothing
            xmlDoc = Nothing

            '------------------------------

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
            End If

        End Try

        CodiciRimappati = String.Join("|", vCodiciRimappati)

        Return xRisp

    End Function


    Public Shared Sub ImpostaRimappaturaCodici(ByRef vCodici As String(), ByVal tipoCodice As String, ByVal NuovoCodice As String)

        Dim fatto As Boolean = False
        Dim i As Integer = 0

        For Each c As String In vCodici

            Dim vRimappaAgenda As String() = vCodici(i).Split(":")
            If vRimappaAgenda.Length > 1 AndAlso
                vRimappaAgenda(0).ToLower = tipoCodice.ToLower AndAlso
                Not c.IndexOf("*") > 1 _
                And Not fatto Then

                vCodici(i) &= "*" & NuovoCodice
                fatto = True
            End If


            i += 1
        Next

    End Sub

    Public Function Scrivi_Modifica(ByRef Agenda As AgronicaCoreEntityFramework_POCO.Agenda,
                                    ByRef GiasContext As Gias_DeveloperServer_Entities,
                                    ByRef objParametriServer As AgronicaCoreParametri
                                    ) As Integer

        Dim agroDP As New Agro_Sequenze
        Dim idAgenda As Integer = 0
        Dim esiste As Boolean = False

        Try
            If Agenda.Id_Agenda = 0 Then

                idAgenda = agroDP.NuovoId_Tabella("Agenda", 0, 200000000, objParametriServer)
                Agenda.Id_Agenda = idAgenda

            Else

                idAgenda = Agenda.Id_Agenda

                Dim agendaCount = From a In GiasContext.Agenda Where a.Id_Agenda = idAgenda Select a
                If agendaCount.Count > 0 Then
                    esiste = True
                End If

            End If

            Dim agendaW As New AgronicaCoreContabDAL.Agenda_W

            If esiste Then
                agendaW.Modifica(Agenda, GiasContext, objParametriServer)
            Else
                agendaW.Scrivi(Agenda, GiasContext, objParametriServer)
            End If

        Catch ex As Exception
            Dim messaggioErrore = "Errore nella funzione Agenda_W.Scrivi_Modifica: " & ex.Message
            If ex.InnerException IsNot Nothing Then
                messaggioErrore &= " Inner exception:" & ex.InnerException.Message
            End If
            Throw New Exception(messaggioErrore)
        End Try

        Return idAgenda

    End Function

    Public Function Elimina_InteraOperazione(ByVal Piva As String,
                                             ByVal ID_Agenda As Integer,
                                             ByRef objParametriServer As AgronicaCoreParametri,
                                             Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing
                                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabBIZ.Agenda_W.Elimina_InteraOperazione()"
        Dim messaggioErrore As String = ""

        Try

            If GiasContext Is Nothing Then
                Dim gefutils As New Gias_EF_Utility
                Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametriServer.StringaConnessione)
                GiasContext = New Gias_DeveloperServer_Entities(EFConnString)
            End If

            Dim obj_Mov_Destinazioni_W As New AgronicaCoreContabBIZ.Mov_Destinazioni_W
            Dim obj_Movimenti_Dettagli_W As New AgronicaCoreContabBIZ.Movimenti_Dettagli_W
            Dim obj_Movimenti_W As New AgronicaCoreContabBIZ.Movimenti_W
            Dim obj_Agenda_W As New AgronicaCoreContabDAL.Agenda_W

            Dim Mov_Destinazioni = (From a In GiasContext.Mov_Destinazioni Where a.Piva = Piva And a.Id_Agenda = ID_Agenda).ToArray
            obj_Mov_Destinazioni_W.Elimina(Mov_Destinazioni, GiasContext, objParametriServer)
            GiasContext.SaveChanges()

            Dim Movimenti_Dettagli = (From a In GiasContext.Movimenti_dettagli Where a.PIVA = Piva And a.Id_Agenda = ID_Agenda).ToArray
            obj_Movimenti_Dettagli_W.Elimina(Movimenti_Dettagli, GiasContext, objParametriServer)
            GiasContext.SaveChanges()

            Dim Movimenti = (From a In GiasContext.Movimenti Where a.PIVA = Piva And a.Id_Agenda = ID_Agenda).ToArray
            obj_Movimenti_W.Elimina(Movimenti, GiasContext, objParametriServer)
            GiasContext.SaveChanges()

            Dim Agenda = (From a In GiasContext.Agenda Where a.PIVA = Piva And a.Id_Agenda = ID_Agenda).FirstOrDefault
            obj_Agenda_W.Elimina_Testata(Agenda, GiasContext, objParametriServer)

            GiasContext.SaveChanges()

            Return True

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Function

    ''' <param name="operazioni"><seealso cref="BloccaAttivitaAgenda"/></param>
    ''' <param name="objParametri">objParametri_Server</param>
    Public sub Blocca_AttivitaAgenda(operazioni As List(Of Object), objParametri As AgronicaCoreParametri)
        Dim objAgenda As New AgronicaCoreContabDAL.Agenda_W
        dim isZooOperation = Function(cod as integer)  cod >= 3000 AndAlso cod < 4000
        AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, objParametri)
        Try
            operazioni.Select(Function(x) New BloccaAttivitaAgenda With {
                .id_agenda = x.item("id_agenda"),
                .piva = x.item("piva"),
                .sa_cod = if(isZooOperation(x.item("lav_cod")), 0, x.item("sa_cod")),
                .lav_cod = x.item("lav_cod")
            }).toList.foreach(sub(x as BloccaAttivitaAgenda)
                                  If Not objAgenda.Agenda_Blocca(x.piva, 0, x.id_agenda, String.Empty, objParametri) Then
                                      Throw New Exception("Blocco non riuscito")
                                  End If
                              End Sub)
            
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, objParametri)
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri)
        Catch ex As Exception
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri)

            Throw New Exception("Blocco Attività" & ": " & ex.Message, ex)
        End Try
    End sub

    Public sub Sblocca_AttivitaAgenda(operazioni As List(Of Object), objParametri As AgronicaCoreParametri)
        Dim objAgenda As New AgronicaCoreContabDAL.Agenda_W
        dim isZooOperation = Function(cod as integer)  cod >= 3000 AndAlso cod < 4000
        AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, objParametri)
        Try
            operazioni.Select(Function(x) New BloccaAttivitaAgenda With {
                .id_agenda = x.item("id_agenda"),
                .piva = x.item("piva"),
                .sa_cod = if(isZooOperation(x.item("lav_cod")), 0, x.item("sa_cod")),
                .lav_cod = x.item("lav_cod")
            }).toList.foreach(sub(x as BloccaAttivitaAgenda)
                                  If Not objAgenda.Agenda_Sblocca(
                                                x.piva,
                                                0,
                                                x.id_agenda,
                                                objParametri.UtenteUsername,
                                                AGRODATAINIZIO,
                                                String.Empty,
                                                objParametri
                ) Then
                                      Throw New Exception("Blocco non riuscito")
                                  End If
                              End Sub)
            
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, objParametri)
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri)
        Catch ex As Exception
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri)

            Throw New Exception("Blocco Attività" & ": " & ex.Message, ex)
        End Try
    End sub

End Class


Public Class obj_ID_Agenda_ID_Mov_ID_Mov_Det
    Public ID_Agenda As Integer
    Public ID_Mov As Integer
    Public ID_Mov_Det As Integer

    Public Function toString_ID_Agenda_ID_Mov() As String
        Return "(id_agenda=" & ID_Agenda & " AND ID_Mov =" & ID_Mov & ")"
    End Function
    Public Function toString_ID_Agenda() As String
        Return "(id_agenda=" & ID_Agenda & ")"
    End Function

    Public Function toString_ID_Agenda_ID_Mov_ID_Mov_Det() As String
        Return "(id_agenda=" & ID_Agenda & " AND ID_Mov =" & ID_Mov & " AND ID_Mov_Det=" & ID_Mov_Det & ")"
    End Function
End Class