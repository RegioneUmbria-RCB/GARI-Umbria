Imports System.Xml
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO



Public Class Ricette_R
    Inherits AgronicaCoreDataProvider.LogProvider

    Public Function Ricetta_Leggi_APP(ByVal piva As String, objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As List(Of Ricette)


        Dim xLetturaRicetteApp As New AgronicaCoreContabDAL.Ricette_R
        Dim rval1 As List(Of Ricette) =
            xLetturaRicetteApp.Ricetta_Leggi_APP(piva, objParametri_Server)



        Return rval1

    End Function

    Public Function Ricetta_Leggi_dtAPP(ByVal piva As String, objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable


        Dim xLetturaRicetteApp As New AgronicaCoreContabDAL.Ricette_R
        Dim rval1 As DataTable =
            xLetturaRicetteApp.Ricetta_Leggi_dtAPP(piva, "", objParametri_Server)



        Return rval1

    End Function

    ''============================================================================
    'Public Function Ricetta_Leggi(ByRef ErrMSG As String, _
    '                                ByVal Ricetta_Cod As Integer, _
    '                                ByVal Piva As String, _
    '                                ByVal Sa_Cod As Integer, _
    '                                ByVal Tipo_Ricetta As Integer, _
    '                                ByVal Veg_Cod As Integer, _
    '                                    ByVal Validita_Inizio As Date, _
    '                                    ByVal Validita_Fine As Date, _
    '                                    ByRef Dt_Ricetta As DataTable, _
    '                                    ByRef Dt_RicettaxCultivar As DataTable, _
    '                                    ByRef Dt_RicettaxAgenda As DataTable, _
    '                                    ByRef Dt_Operazioni As DataTable, _
    '                                    ByRef Dt_Dettagli As DataTable, _
    '                                    ByRef Dt_Dettagli_Tecnici As DataTable, _
    '                                    ByRef Dt_Destinazioni As DataTable, _
    '                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                                        )


    '    Dim NomeRoutine As String = "ContabBIZ.Ricette_R.Ricette_Leggi()"

    '    Dim MessaggioErrore As String = ""
    '    Dim FlagConnessioneLocale As Boolean = False

    '    Dim i As Integer

    '    Dim ObjRicetta As AgronicaCoreContabDAL.Ricette_R
    '    Dim ObjRicettaxAgenda As AgronicaCoreContabDAL.RicettexAgenda_R
    '    Dim ObjRicettaxCultivar As AgronicaCoreContabDAL.RicettexCultivar_R
    '    Dim ObjRicettaOperazioni As AgronicaCoreContabDAL.Ricette_Operazioni_R
    '    Dim ObjRicettaDettagli As AgronicaCoreContabDAL.Ricette_Dettagli_R
    '    Dim ObjRicettaDettagliTecnici As AgronicaCoreContabDAL.Ricette_Dett_Tecnico_R
    '    Dim ObjRicettaDestinazioni As AgronicaCoreContabDAL.Ricette_Destinazioni_R

    '    Try

    '        '------------------------------
    '        'Verifico se e' stata impostata una connessione
    '        If IsNothing(objParametri.objConnessione) Then
    '            'Flag
    '            FlagConnessioneLocale = True
    '            'Creo la connessione localmente
    '            objParametri.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
    '            objParametri.objConnessione.Open()
    '        End If
    '        If objParametri.objConnessione.State = ConnectionState.Closed Then
    '            'Flag
    '            FlagConnessioneLocale = True
    '            'Creo la connessione localmente
    '            objParametri.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
    '            objParametri.objConnessione.Open()
    '        End If
    '        '------------------------------

    '        ObjRicetta = New AgronicaCoreContabDAL.Ricette_R
    '        ObjRicettaxCultivar = New AgronicaCoreContabDAL.RicettexCultivar_R

    '        Dt_Ricetta = ObjRicetta.Leggi( _
    '                                Ricetta_Cod, _
    '                                Piva, _
    '                                Sa_Cod, _
    '                                Tipo_Ricetta, _
    '                                Veg_Cod, _
    '                                Validita_Inizio, _
    '                                Validita_Fine, _
    '                                AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
    '                                "", _
    '                                "", _
    '                                objParametri)

    '        If Not Dt_Ricetta Is Nothing AndAlso Dt_Ricetta.Rows.Count > 0 Then

    '            Ricetta_Cod = Dt_Ricetta.Rows(0).Item("Ricetta_Cod")

    '            Dt_RicettaxCultivar = ObjRicettaxCultivar.Leggi(Ricetta_Cod, _
    '                                                            Veg_Cod, _
    '                                                            0, _
    '                                                            AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO, _
    '                                                            AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAFINE, _
    '                                                            AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
    '                                                            "", _
    '                                                            "", _
    '                                                            objParametri)

    '            Dt_RicettaxAgenda = ObjRicettaxAgenda.Leggi(Ricetta_Cod, _
    '                                                        0, _
    '                                                        0, _
    '                                                        AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO, _
    '                                                        AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAFINE, _
    '                                                        AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
    '                                                        "", _
    '                                                        "", _
    '                                                        objParametri)


    '            Dt_Operazioni = ObjRicettaOperazioni.Leggi(Ricetta_Cod, _
    '                                                    0, _
    '                                                    0, _
    '                                                    AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO, _
    '                                                    AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAFINE, _
    '                                                        AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
    '                                                        "", "", _
    '                                                        objParametri)


    '            Dt_Dettagli = ObjRicettaDettagli.Leggi(Ricetta_Cod, _
    '                                                    0, _
    '                                                    0, _
    '                                                    "", _
    '                                                    0, _
    '                                                    0, _
    '                                                    0, _
    '                                                    0, _
    '                                                    Validita_Inizio, _
    '                                                    Validita_Fine, _
    '                                                    AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, _
    '                                                    "", "", _
    '                                                    objParametri)


    '            Dt_Destinazioni = ObjRicettaDestinazioni.Leggi(Ricetta_Cod, _
    '                                                            0, _
    '                                                            0, _
    '                                                            0, _
    '                                                            0, _
    '                                                            "", _
    '                                                            0, _
    '                                                            0, _
    '                                                            0, _
    '                                                            Validita_Inizio, _
    '                                                            Validita_Fine, _
    '                                                            AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
    '                                                            "", "", _
    '                                                            objParametri)

    '        End If


    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    Finally

    '        If FlagConnessioneLocale = True Then
    '            If Not IsNothing(objParametri.objConnessione) Then
    '                objParametri.objConnessione.Close()
    '                objParametri.objConnessione.Dispose()
    '            End If
    '        End If

    '    End Try


    'End Function




    '<<<<<  WORK IN PROGRESS  >>>>>





    '============================================================================
    Public Function Ricetta_Leggi( _
                                    ByVal Ricetta_Cod As Int32, _
                                    ByVal Piva As String, _
                                    ByVal Sa_Cod As Int32, _
                                    ByVal Tipo_Ricetta As Int32, _
                                    ByVal Veg_Cod As Int32, _
                                    ByVal ForDelete As Boolean, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                    Optional ByVal FiltriXlettura_vuoti As Boolean = False, _
                                    Optional ByVal RicettaDettagliOrderBy As String = "" _
                    ) _
                                        As String

        Dim NomeRoutine As String = "ContabBIZ.Ricette_R.Ricetta_Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim FlagConnessioneLocale As Boolean = False

        Dim i As Int32
        Dim j As Int32
        Dim RisultatoFunzione As String = String.Empty

        '-----

        Dim XmlDoc As XmlDocument                           'MSXML2.DOMDocument40
        Dim XmlDatiRicetta As XmlElement                    'IXMLDOMElement
        Dim XmlRicetta As XmlElement                        'IXMLDOMElement
        Dim XmlDatiRicettaxCultivar As XmlElement           'IXMLDOMElement
        Dim XmlRicettaxCultivar As XmlElement               'IXMLDOMElement


        Dim XmlDatiRicettaxNote As XmlElement           'IXMLDOMElement
        Dim XmlRicettaxNote As XmlElement               'IXMLDOMElement

        Dim XmlDatiRicettaxAgenda As XmlElement           'IXMLDOMElement
        Dim XmlRicettaxAgenda As XmlElement               'IXMLDOMElement

        Dim ObjRicetta As AgronicaCoreContabDAL.Ricette_R
        Dim ObjRicettaxCultivar As AgronicaCoreContabDAL.RicettexCultivar_R
        Dim ObjRicettaOperazioni As AgronicaCoreContabBIZ.Ricette_Operazioni_R

        Dim DtRicetta As DataTable
        Dim DtRicettaxCultivar As DataTable

        Dim DatiOperazioni As String



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

            '------------------------------
            '------------------------------
            '------------------------------


            ObjRicetta = New AgronicaCoreContabDAL.Ricette_R

            DtRicetta = ObjRicetta.Leggi( _
                                        Ricetta_Cod, _
                                        Piva, _
                                        Sa_Cod, _
                                        Tipo_Ricetta, _
                                        Veg_Cod, _
                                        AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO, _
                                        AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAFINE, _
                                        AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                        "", _
                                        "", _
                                        objParametri, _
                                        FiltriXlettura_vuoti)

            'Se ottengo almeno un risultato, creo la struttura XML
            If DtRicetta.Rows.Count > 0 Then

                '----- < Documento XML > -----
                XmlDoc = New XmlDocument

                XmlDatiRicetta = XmlDoc.CreateElement("DatiRicetta")

                For i = 0 To DtRicetta.Rows.Count - 1

                    '----- < RICETTA > -----
                    XmlRicetta = XmlDoc.CreateElement("Ricetta")

                    With XmlRicetta
                        .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                        .SetAttribute("ricetta_superuser", Agro_SQL_Load(DtRicetta.Rows(i).Item("Ricetta_SuperUser")))
                        .SetAttribute("ricetta_cod", Agro_SQL_Load(DtRicetta.Rows(i).Item("Ricetta_Cod")))
                        .SetAttribute("ricetta_numero", Agro_SQL_Load(DtRicetta.Rows(i).Item("ricetta_numero")))
                        .SetAttribute("ricetta_des", Agro_SQL_Load(DtRicetta.Rows(i).Item("Ricetta_Des")))
                        .SetAttribute("ricetta_des_long", Agro_SQL_Load(DtRicetta.Rows(i).Item("Ricetta_Des_Long")))
                        .SetAttribute("veg_cod", Agro_SQL_Load(DtRicetta.Rows(i).Item("Veg_Cod")))
                        .SetAttribute("note", Agro_SQL_Load(DtRicetta.Rows(i).Item("Note")))
                        .SetAttribute("inviato", Agro_SQL_Load(DtRicetta.Rows(i).Item("Inviato")))
                        .SetAttribute("username_creazione", Agro_SQL_Load(DtRicetta.Rows(i).Item("Username_Creazione")))
                        .SetAttribute("username_modifica", Agro_SQL_Load(DtRicetta.Rows(i).Item("Username_Modifica")))
                        .SetAttribute("data_creazione", Agro_SQL_Load(DtRicetta.Rows(i).Item("Data_Creazione")))
                        .SetAttribute("data_modifica", Agro_SQL_Load(DtRicetta.Rows(i).Item("Data_Modifica")))
                        .SetAttribute("validita_inizio", Agro_SQL_Load(DtRicetta.Rows(i).Item("Validita_Inizio")))
                        .SetAttribute("validita_fine", Agro_SQL_Load(DtRicetta.Rows(i).Item("Validita_Fine")))
                        .SetAttribute("piva", Agro_SQL_Load(DtRicetta.Rows(i).Item("Piva")))
                        .SetAttribute("sa_cod", Agro_SQL_Load(DtRicetta.Rows(i).Item("sa_cod")))
                        .SetAttribute("tipo_ricetta", Agro_SQL_Load(DtRicetta.Rows(i).Item("tipo_ricetta")))
                        .SetAttribute("programmazione_cod", Agro_SQL_Load(DtRicetta.Rows(i).Item("programmazione_cod")))
                        .SetAttribute("imputazione_cod", Agro_SQL_Load(DtRicetta.Rows(i).Item("imputazione_cod")))
                        .SetAttribute("imputazione_fase_cod", Agro_SQL_Load(DtRicetta.Rows(i).Item("imputazione_fase_cod")))
                        .SetAttribute("origine", Agro_SQL_Load(DtRicetta.Rows(i).Item("origine")))
                    End With


                    '#####################################
                    '##########  CULTIVAR  ###############
                    '#####################################

                    ObjRicettaxCultivar = New AgronicaCoreContabDAL.RicettexCultivar_R

                    DtRicettaxCultivar = ObjRicettaxCultivar.Leggi(CInt(Agro_SQL_Load(DtRicetta.Rows(i).Item("Ricetta_Cod"))), _
                                                                   CInt(Agro_SQL_Load(DtRicetta.Rows(i).Item("Veg_Cod"))), _
                                                                   0, _
                                                                    AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO, _
                                                                    AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAFINE, _
                                                                    AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                                                    "", _
                                                                    "", _
                                                                    objParametri)

                    'Se ottengo almeno un risultato, creo la struttura XML
                    If DtRicettaxCultivar.Rows.Count > 0 Then

                        XmlDatiRicettaxCultivar = XmlDoc.CreateElement("DatiRicettaxCultivar")

                        'Effettuo un ciclo sui Dettagli Tecnici della ricetta

                        'Do While Not RsRicettaxCultivar.EOF
                        For j = 0 To DtRicettaxCultivar.Rows.Count - 1

                            '----- < RICETTAXCULTIVAR > -----
                            XmlRicettaxCultivar = XmlDoc.CreateElement("RicettaxCultivar")

                            With XmlRicettaxCultivar
                                .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                .SetAttribute("ricetta_cod", Agro_SQL_Load(DtRicettaxCultivar.Rows(j).Item("Ricetta_Cod")))
                                .SetAttribute("veg_cod", Agro_SQL_Load(DtRicettaxCultivar.Rows(j).Item("Veg_Cod")))
                                .SetAttribute("cul_cod", Agro_SQL_Load(DtRicettaxCultivar.Rows(j).Item("Cul_Cod")))
                                .SetAttribute("inviato", Agro_SQL_Load(DtRicettaxCultivar.Rows(j).Item("Inviato")))
                                .SetAttribute("data_creazione", Agro_SQL_Load(DtRicettaxCultivar.Rows(j).Item("Data_Creazione")))
                                .SetAttribute("data_modifica", Agro_SQL_Load(DtRicettaxCultivar.Rows(j).Item("Data_Modifica")))
                                .SetAttribute("username_creazione", Agro_SQL_Load(DtRicettaxCultivar.Rows(j).Item("Username_Creazione")))
                                .SetAttribute("username_modifica", Agro_SQL_Load(DtRicettaxCultivar.Rows(j).Item("Username_Modifica")))
                                .SetAttribute("validita_inizio", Agro_SQL_Load(DtRicettaxCultivar.Rows(j).Item("validita_inizio")))
                                .SetAttribute("validita_fine", Agro_SQL_Load(DtRicettaxCultivar.Rows(j).Item("validita_fine")))
                            End With

                            XmlDatiRicettaxCultivar.AppendChild(XmlRicettaxCultivar)
                            '----- < / RICETTAXCULTIVAR > -----

                        Next
                        'Loop

                        XmlRicetta.AppendChild(XmlDatiRicettaxCultivar)

                    End If

                    XmlDatiRicettaxCultivar = Nothing
                    XmlRicettaxCultivar = Nothing
                    DtRicettaxCultivar.Dispose()
                    DtRicettaxCultivar = Nothing
                    ObjRicettaxCultivar = Nothing




                    '#####################################
                    '##########  RicettexNote  ###########
                    '#####################################

                    Dim objRicettexNote As New AgronicaCoreContabDAL.RicettexNote_R
                    Dim DtRicettexNote As DataTable

                    'prendo solo le note con ricetta_operazione_cod = 0 
                    DtRicettexNote = objRicettexNote.Leggi(CInt(Agro_SQL_Load(DtRicetta.Rows(i).Item("Ricetta_Cod"))), _
                                                            0, _
                                                            0, _
                                                            AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO, _
                                                            AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAFINE, _
                                                            AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                                            " RicettexNote.Ricetta_Operazione_Cod =0 ", _
                                                            "", _
                                                            objParametri)


                    'Se ottengo almeno un risultato, creo la struttura XML
                    If DtRicettexNote.Rows.Count > 0 Then

                        XmlDatiRicettaxNote = XmlDoc.CreateElement("DatiRicettaxNote")

                        'Effettuo un ciclo sui Dettagli Tecnici della ricetta

                        'Do While Not RsRicettaxCultivar.EOF
                        For j = 0 To DtRicettexNote.Rows.Count - 1

                            '----- < RICETTAXCULTIVAR > -----
                            XmlRicettaxNote = XmlDoc.CreateElement("RicettaxNote")

                            With XmlRicettaxNote
                                .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                .SetAttribute("ricetta_superuser", Agro_SQL_Load(DtRicettexNote.Rows(j).Item("Ricetta_Superuser")))
                                .SetAttribute("ricetta_cod", Agro_SQL_Load(DtRicettexNote.Rows(j).Item("Ricetta_Cod")))
                                .SetAttribute("ricetta_operazione_cod", Agro_SQL_Load(DtRicettexNote.Rows(j).Item("Ricetta_Operazione_Cod")))
                                .SetAttribute("nota_cod", Agro_SQL_Load(DtRicettexNote.Rows(j).Item("Nota_Cod")))
                                .SetAttribute("inviato", Agro_SQL_Load(DtRicettexNote.Rows(j).Item("Inviato")))

                                .SetAttribute("data_creazione", Agro_SQL_Load(DtRicettexNote.Rows(j).Item("Data_Creazione")))
                                .SetAttribute("data_modifica", Agro_SQL_Load(DtRicettexNote.Rows(j).Item("Data_Modifica")))
                                .SetAttribute("username_creazione", Agro_SQL_Load(DtRicettexNote.Rows(j).Item("Username_Creazione")))
                                .SetAttribute("username_modifica", Agro_SQL_Load(DtRicettexNote.Rows(j).Item("Username_Modifica")))
                                .SetAttribute("validita_inizio", Agro_SQL_Load(DtRicettexNote.Rows(j).Item("validita_inizio")))
                                .SetAttribute("validita_fine", Agro_SQL_Load(DtRicettexNote.Rows(j).Item("validita_fine")))
                            End With

                            XmlDatiRicettaxNote.AppendChild(XmlRicettaxNote)
                            '----- < / RICETTAXCULTIVAR > -----

                        Next
                        'Loop

                        XmlRicetta.AppendChild(XmlDatiRicettaxNote)

                    End If



                    '###########################################
                    '##########  RicettexAgenda  ###############
                    '###########################################

                    Dim objRicettexAgenda As New AgronicaCoreContabDAL.RicettexAgenda_R
                    Dim DtRicettexAgenda As DataTable

                    'prendo solo le note con ricetta_operazione_cod = 0 
                    DtRicettexAgenda = objRicettexAgenda.Leggi(CInt(Agro_SQL_Load(DtRicetta.Rows(i).Item("Ricetta_Cod"))), _
                                                                   0, _
                                                                   0, _
                                                                    AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO, _
                                                                    AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAFINE, _
                                                                    AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                                                    "RicettexAgenda.Ricetta_Operazione_Cod =0", _
                                                                    "", _
                                                                    objParametri)



                    'Se ottengo almeno un risultato, creo la struttura XML
                    If DtRicettexAgenda.Rows.Count > 0 Then

                        XmlDatiRicettaxAgenda = XmlDoc.CreateElement("DatiRicettaxAgenda")

                        'Effettuo un ciclo sui Dettagli Tecnici della ricetta

                        'Do While Not RsRicettaxCultivar.EOF
                        For j = 0 To DtRicettexAgenda.Rows.Count - 1

                            '----- < RICETTAXCULTIVAR > -----
                            XmlRicettaxAgenda = XmlDoc.CreateElement("RicettaxAgenda")

                            With XmlRicettaxAgenda
                                .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                .SetAttribute("ricetta_superuser", Agro_SQL_Load(DtRicettexAgenda.Rows(j).Item("Ricetta_Superuser")))
                                .SetAttribute("ricetta_cod", Agro_SQL_Load(DtRicettexAgenda.Rows(j).Item("Ricetta_Cod")))
                                .SetAttribute("ricetta_operazione_cod", Agro_SQL_Load(DtRicettexAgenda.Rows(j).Item("Ricetta_Operazione_Cod")))
                                .SetAttribute("id_agenda", Agro_SQL_Load(DtRicettexAgenda.Rows(j).Item("Id_Agenda")))
                                .SetAttribute("inviato", Agro_SQL_Load(DtRicettexAgenda.Rows(j).Item("Inviato")))

                                .SetAttribute("data_creazione", Agro_SQL_Load(DtRicettexAgenda.Rows(j).Item("Data_Creazione")))
                                .SetAttribute("data_modifica", Agro_SQL_Load(DtRicettexAgenda.Rows(j).Item("Data_Modifica")))
                                .SetAttribute("username_creazione", Agro_SQL_Load(DtRicettexAgenda.Rows(j).Item("Username_Creazione")))
                                .SetAttribute("username_modifica", Agro_SQL_Load(DtRicettexAgenda.Rows(j).Item("Username_Modifica")))
                                .SetAttribute("validita_inizio", Agro_SQL_Load(DtRicettexAgenda.Rows(j).Item("validita_inizio")))
                                .SetAttribute("validita_fine", Agro_SQL_Load(DtRicettexAgenda.Rows(j).Item("validita_fine")))
                            End With

                            XmlDatiRicettaxAgenda.AppendChild(XmlRicettaxAgenda)
                            '----- < / RICETTAXCULTIVAR > -----

                        Next
                        'Loop

                        XmlRicetta.AppendChild(XmlDatiRicettaxAgenda)

                    End If




                    '#####################################
                    '##########  OPERAZIONI  #############
                    '#####################################

                    ObjRicettaOperazioni = New AgronicaCoreContabBIZ.Ricette_Operazioni_R

                    DatiOperazioni = ObjRicettaOperazioni.Ricetta_Operazioni_Leggi(CInt(Agro_SQL_Load(DtRicetta.Rows(i).Item("Ricetta_Cod"))), _
                                                                                    0, _
                                                                                    0, _
                                                                                    0, _
                                                                                    AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO, _
                                                                                    AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAFINE, _
                                                                                    ForDelete, _
                                                                                    objParametri, _
                                                                                    RicettaDettagliOrderBy)

                    XmlRicetta.InnerXml = XmlRicetta.InnerXml & DatiOperazioni

                    DatiOperazioni = Nothing
                    ObjRicettaOperazioni = Nothing


                    '#################################
                    '#################################
                    '#################################

                    XmlDatiRicetta.AppendChild(XmlRicetta)
                    '----- < / RICETTA > -----


                Next

                XmlDoc.AppendChild(XmlDatiRicetta)

                RisultatoFunzione = XmlDoc.OuterXml
                '----- < / Documento XML > -----


                XmlRicetta = Nothing
                XmlDatiRicetta = Nothing
                XmlDoc = Nothing

            Else

                RisultatoFunzione = ""

            End If


            '------------------------------
            '------------------------------
            '------------------------------

            'Elimino gli oggetti che ho creato

            ObjRicetta = Nothing



            '-------------------------------------------------------------------------------


        Catch ex As Exception

            RisultatoFunzione = ""
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)

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

    Public Function Genera_Nuovo_Nome_Ricetta(piva As String, data_operazione As DateTime, objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        'Grilli 10/05/2018 Su indicazione di Fabrizio propongo di default l'anno + un progressivo
        Dim dt_elenco As DataTable = New AgronicaCoreContabDAL.Ricette_R().Leggi(0, piva, 0, enum_TipoRicetta.Standard_Destinazioni, 0, AGRODATAINIZIO, AGRODATAFINE, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "Validita_Inizio >= " & UtilityProvider.Agro_SQL_SaveDate(New Date(Date.Now.Year, 1, 1)) & " AND Validita_Inizio <= " & UtilityProvider.Agro_SQL_SaveDate(New Date(Date.Now.Year, 12, 31)), "ricetta_cod DESC", objParametri_Server)

        Dim max As Integer = 0
        For Each dr As DataRow In dt_elenco.Rows
            Dim tmp As String = dr.Item("ricetta_numero").ToString
            If tmp.StartsWith(data_operazione.Year & "_") AndAlso IsNumeric(tmp.Substring(5)) AndAlso CInt(tmp.Substring(5)) > max Then
                max = CInt(tmp.Substring(5))
            End If
        Next

        Dim ricetta_numero As String = data_operazione.Year & "_" & (max + 1)

        Return ricetta_numero

    End Function


    Public Function XML_GeneraStringa_Ricetta(piva As String, sa_cod As Integer, veg_cod As Integer,
                                              ricetta_cod As Integer, ricetta_des As String,
                                              data_inizio As DateTime, data_fine As DateTime,
                                              ricetta_numero As String, nota As String,
                                              tipo_operazione As enum_TipoOperazioneDB, tipo_ricetta As enum_TipoRicetta,
                                              Programmazione_Cod As Integer,
                                              listaXMLRicettaOperazione As List(Of String),
                                              progressivo_gias As Integer,
                                              objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                              Optional Origine As String = ""
                                              ) As String

        Dim Cul_Cod As Integer = -1 '---> TUTTE LE VARIETA'

        'Dim Validita_Inizio As Date = data_operazione
        'Dim Validita_Fine As Date = data_operazione

        Dim frm_BaseCode, frm_TopCode As Integer
        Call Calcola_BaseCode_TopCode(frm_BaseCode, frm_TopCode, progressivo_gias)

        ' --------------- RICETTA NUMERO ---------------
        If ricetta_numero = "" Then

            Dim r_R As New AgronicaCoreContabBIZ.Ricette_R
            ricetta_numero = r_R.Genera_Nuovo_Nome_Ricetta(piva, data_inizio, objParametri_Server)

        End If
        ' ---------------------------------------------

        Dim tipoOperazioneTestata As enum_TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura

        If ricetta_cod <> 0 Then
            tipoOperazioneTestata = enum_TipoOperazioneDB.Modifica
        End If

        'If tipo_operazione = enum_TipoOperazioneDB.Modifica Or
        '    (tipo_operazione = enum_TipoOperazioneDB.Scrittura And tipo_ricetta = enum_TipoRicetta.PianoDistribuzionePua) Or
        '    (tipo_operazione = enum_TipoOperazioneDB.Scrittura And ricetta_cod <> 0) Then
        '    tipoOperazioneTestata = enum_TipoOperazioneDB.Lettura
        'Else
        '    'veg_cod = -1 '---> TUTTE LE SPECIE
        '    ricetta_cod = 0
        'End If

        Dim objXml As New AgronicaCoreXML.XML_Contab

        '------------------------------------------------
        '----- Genero la struttura XML
        '------------------------------------------------

        '----- DatiRicetta
        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XMLDatiRicetta As System.Xml.XmlElement = XmlDoc.CreateElement("DatiRicetta")

        Select Case tipo_operazione
            Case enum_TipoOperazioneDB.Scrittura

                XMLDatiRicetta.InnerXml = objXml.XML_Ricetta(
                                                    tipoOperazioneTestata,
                                                    objParametri_Server.PivaSuperUser,
                                                    frm_BaseCode,
                                                    frm_TopCode,
                                                    ricetta_cod,
                                                    ricetta_des,
                                                    ricetta_des,
                                                    veg_cod,
                                                    nota,
                                                    data_inizio,
                                                    data_fine,
                                                    piva,
                                                    sa_cod,
                                                    tipo_ricetta,
                                                    ricetta_numero,
                                                    Programmazione_Cod,
                                                    Origine)

            Case enum_TipoOperazioneDB.Modifica

                'Imposto lettura così non viene salvato ma non va in errore il salvataggio
                XMLDatiRicetta.InnerXml = objXml.XML_Ricetta(
                                                    tipoOperazioneTestata,
                                                    objParametri_Server.PivaSuperUser,
                                                    frm_BaseCode,
                                                    frm_TopCode,
                                                    ricetta_cod,
                                                    ricetta_des,
                                                    ricetta_des,
                                                    veg_cod,
                                                    nota,
                                                    data_inizio,
                                                    data_fine,
                                                    piva,
                                                    sa_cod,
                                                    tipo_ricetta,
                                                    ricetta_numero,
                                                    Programmazione_Cod,
                                                    Origine)

        End Select



        '----- Ricetta
        Dim XMLRicetta As System.Xml.XmlElement = XMLDatiRicetta.SelectSingleNode("Ricetta")

        '----- DatiRicettaxCultivar
        Dim XMLDatiRicettaxCultivar As System.Xml.XmlElement = XmlDoc.CreateElement("DatiRicettaxCultivar")

        XMLRicetta.AppendChild(XMLDatiRicettaxCultivar)

        Select Case tipo_operazione
            Case enum_TipoOperazioneDB.Scrittura
                XMLDatiRicettaxCultivar.InnerXml &= objXml.XML_RicettaxCultivar(tipoOperazioneTestata,
                                                                    objParametri_Server.PivaSuperUser,
                                                                    ricetta_cod,
                                                                    veg_cod,
                                                                    Cul_Cod,
                                                                    data_inizio,
                                                                    data_fine)
            Case enum_TipoOperazioneDB.Modifica
                'Imposto lettura così non viene salvato ma non va in errore il salvataggio
                XMLDatiRicettaxCultivar.InnerXml &= objXml.XML_RicettaxCultivar(enum_TipoOperazioneDB.Lettura,
                                                                    objParametri_Server.PivaSuperUser,
                                                                    ricetta_cod,
                                                                    veg_cod,
                                                                    Cul_Cod,
                                                                    data_inizio,
                                                                    data_fine)
        End Select



        '----- DatiRicetta_Operazioni
        Dim XMLDatiRicetta_Operazioni As System.Xml.XmlElement = XmlDoc.CreateElement("DatiRicetta_Operazioni")
        XMLRicetta.AppendChild(XMLDatiRicetta_Operazioni)

        XMLDatiRicetta_Operazioni.InnerXml = String.Join("", listaXMLRicettaOperazione)

        '----- Assemblo la struttura
        XmlDoc.AppendChild(XMLDatiRicetta)

        '----- Restituisco il risultato
        Return XmlDoc.OuterXml

    End Function


    Public Function Genera_Nuovo_Numero_Ricetta(ByVal Piva As String,
                                                ByVal data As DateTime,
                                                objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As String

        Dim ricetta_numero As String = ""

        Dim dt_elenco As DataTable = New AgronicaCoreContabDAL.Ricette_R().Leggi(0, Piva, 0,
                                                                                 0, 0,
                                                                                 CostantiPersonalizzate.AGRODATAINIZIO, CostantiPersonalizzate.AGRODATAFINE,
                                                                                 enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                                 "Validita_Inizio >= " & UtilityProvider.Agro_SQL_SaveDate(New Date(data.Year, 1, 1)) & " AND Validita_Inizio <= " & UtilityProvider.Agro_SQL_SaveDate(New Date(data.Year, 12, 31)),
                                                                                 "", objParametri_Server)
        If Not IsNothing(dt_elenco) AndAlso dt_elenco.Rows.Count > 0 Then
            ricetta_numero = data.Year & "_" & CStr(dt_elenco.Rows.Count + 1)
        Else
            ricetta_numero = data.Year & "_1"
        End If

        Return ricetta_numero

    End Function

End Class


'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§


Public Class Ricette_W
    Inherits AgronicaCoreDataProvider.DataProvider


    '============================================================================
    Public Function Ricetta_Scrivi(ByVal DatiRicetta As String,
                                   ByRef OUTPUT_Ricetta_Cod As Int32,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                   Optional ByVal idServizio As enum_Id_Servizio = enum_Id_Servizio.GiasOnline
                                   ) As Boolean


        '----------------------------------------------------------------------

        Dim NomeRoutine As String = "AgronicaCoreContabBIZ.Ricette_W.Ricetta_Scrivi()"

        '----------------------------------------------------------------------

        '----------------------------------------------------------------------

        Dim ObjSequenze As AgronicaCoreDataProvider.Agro_Sequenze
        Dim ObjRicetta As AgronicaCoreContabDAL.Ricette_W
        Dim objRicettaxCultivar As AgronicaCoreContabDAL.RicettexCultivar_W
        Dim objRicetta_Operazioni As AgronicaCoreContabBIZ.Ricette_Operazioni_W
        Dim ObjRicetteLog As AgronicaCoreContabDAL.AgronicaLogRicette_W


        Dim Ricetta_Cod As Integer
        Dim Tipo_Ricetta As Integer
        Dim Ricetta_Operazione_Cod As Integer
        Dim Veg_Cod As Integer
        Dim Cul_Cod As Integer

        Dim Dummy As Boolean
        Dim XmlDoc As XmlDocument

        Dim XmlDatiRicetta_Operazioni As XmlNodeList
        Dim XmlDatiRicetta_Operazione As XmlElement


        Dim xDatiRicette As XmlNodeList
        Dim xDatiRicetta As XmlElement
        Dim xRicette As XmlNodeList
        Dim xRicetta As XmlElement

        Dim xListaDatiRicettaxCultivar As XmlNodeList
        Dim xDatiRicettaxCultivar As XmlElement
        Dim xListaRicettaxCultivar As XmlNodeList
        Dim xRicettaxCultivar As XmlElement

        Dim i_DatiRicettaxCultivar As Integer
        Dim i_RicettaxCultivar As Integer

        Dim i_DatiRicetta As Integer
        Dim i_Ricetta As Integer
        Dim intDummy As Long
        Dim Id_Ricetta As Long

        Dim OpeDB_Ricetta As String
        Dim OpeDB_RicettaxCultivar As String
        Dim OpeDB_RicettaxNote As String
        Dim OpeDB_RicettaxAgenda As String

        Dim DatiRicetta_Operazioni As String

        Dim Ricetta_SuperUser As String

        '------------------------------
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim MessaggioErrore As String = ""
        Dim xRisp As Boolean = True
        '------------------------------


        Try

            '------------------------------

            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, objParametri)

            '------------------------------

            XmlDoc = New Xml.XmlDocument
            'XmlDoc.async = False
            XmlDoc.LoadXml(DatiRicetta)

            '------------------------------

            xDatiRicette = XmlDoc.GetElementsByTagName("DatiRicetta")

            i_DatiRicetta = 0

            Do While i_DatiRicetta < xDatiRicette.Count

                'Prelevo l'i-esimo blocco
                xDatiRicetta = xDatiRicette.Item(i_DatiRicetta)

                '------------------------------
                xRicette = xDatiRicetta.GetElementsByTagName("Ricetta")

                i_Ricetta = 0

                Do While i_Ricetta < xRicette.Count

                    'Prelevo l' i-esima Codifica Agenda
                    xRicetta = xRicette.Item(i_Ricetta)

                    'Prelevo gli attributi dell'agenda selezionata
                    OpeDB_Ricetta = xRicetta.GetAttribute("TipoOperazioneDB")

                    ObjRicetta = New AgronicaCoreContabDAL.Ricette_W
                    ObjRicetteLog = New AgronicaCoreContabDAL.AgronicaLogRicette_W


                    'Inizializzo Preventivamente il Ricetta_Cod
                    Ricetta_Cod = CInt(xRicetta.GetAttribute("ricetta_cod"))

                    Tipo_Ricetta = IIf(xRicetta.GetAttribute("tipo_ricetta") = String.Empty, "0", xRicetta.GetAttribute("tipo_ricetta"))

                    Ricetta_SuperUser = CStr(xRicetta.GetAttribute("ricetta_superuser"))

                    'Verifico l'operazione richiesta
                    Select Case OpeDB_Ricetta
                        '
                        Case "0"      'LEGGI -------------------------------------------------------
                            '
                        Case "1"  'SALVA  -------------------------------------------------------

                            If Ricetta_Cod <= 0 Then

                                ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

                                'Richiedo un nuovo codice movimento
                                Ricetta_Cod = ObjSequenze.NuovoId_Tabella(
                                                                "Ricette",
                                                                CInt(xRicetta.GetAttribute("basecode")),
                                                                CInt(xRicetta.GetAttribute("topcode")),
                                                                objParametri)

                                ObjSequenze = Nothing

                                OUTPUT_Ricetta_Cod = Ricetta_Cod

                            Else

                                'Esportazione in Locale

                            End If



                            Dim xRicetta_data_creazione As Date = #2/1/1900#
                            Dim xRicetta_data_Modifica As Date = #2/1/1900#
                            Dim xRicetta_username_creazione As String = ""
                            Dim xRicetta_username_modifica As String = ""

                            '  Marco Grilli, 07/04/2016 14:43:18: Parametri gestiti dal LAN e quindi valorizzati a 0 per l'online
                            Dim xRicetta_Imputazione_Cod As Integer = 0
                            Dim xRicetta_Imputazione_Fase_Cod As Integer = 0

                            If Not IsNothing(xRicetta.GetAttribute("data_creazione")) AndAlso
                             xRicetta.GetAttribute("data_creazione") <> "" Then
                                xRicetta_data_creazione = CDate(xRicetta.GetAttribute("data_creazione"))
                            End If

                            If Not IsNothing(xRicetta.GetAttribute("data_modifica")) AndAlso
                             xRicetta.GetAttribute("data_modifica") <> "" Then
                                xRicetta_data_Modifica = CDate(xRicetta.GetAttribute("data_modifica"))
                            End If

                            If Not IsNothing(xRicetta.GetAttribute("username_creazione")) Then
                                xRicetta_username_creazione = CStr(xRicetta.GetAttribute("username_creazione"))
                            End If

                            If Not IsNothing(xRicetta.GetAttribute("username_modifica")) Then
                                xRicetta_username_modifica = CStr(xRicetta.GetAttribute("username_modifica"))
                            End If


                            If Not IsNothing(xRicetta.GetAttribute("imputazione_cod")) AndAlso xRicetta.GetAttribute("imputazione_cod") <> "" Then
                                xRicetta_Imputazione_Cod = CInt(xRicetta.GetAttribute("imputazione_cod"))
                            End If

                            If Not IsNothing(xRicetta.GetAttribute("imputazione_fase_cod")) AndAlso xRicetta.GetAttribute("imputazione_fase_cod") <> "" Then
                                xRicetta_Imputazione_Fase_Cod = CInt(xRicetta.GetAttribute("imputazione_fase_cod"))
                            End If

                            'Se passo nell'XML il programmazione_cod allora lo metto, se no metto il default
                            Dim xRicetta_Programmazione_cod As Integer
                            If Not IsNothing(xRicetta.GetAttribute("programmazione_cod")) AndAlso xRicetta.GetAttribute("programmazione_cod") <> "0" Then
                                xRicetta_Programmazione_cod = CInt(xRicetta.GetAttribute("programmazione_cod"))
                            Else
                                xRicetta_Programmazione_cod = Agro_XML_GetInteger(xRicetta, "programmazione_cod", 0)
                            End If

                            Dim xRicetta_Origine As String = ""
                            If Not IsNothing(xRicetta.GetAttribute("origine")) AndAlso xRicetta.GetAttribute("origine") <> "" Then
                                xRicetta_Origine = CStr(xRicetta.GetAttribute("origine"))
                            End If

                            Dummy = ObjRicetta.Scrivi(
                                     CInt(Ricetta_Cod),
                                     CStr(xRicetta.GetAttribute("ricetta_numero")),
                                     CStr(xRicetta.GetAttribute("piva")),
                                     IIf(xRicetta.GetAttribute("sa_cod") = String.Empty, "0", xRicetta.GetAttribute("sa_cod")),
                                     Tipo_Ricetta,
                                     CStr(xRicetta.GetAttribute("ricetta_des")),
                                     CStr(xRicetta.GetAttribute("ricetta_des_long")),
                                     CInt(xRicetta.GetAttribute("veg_cod")),
                                     CStr(xRicetta.GetAttribute("note")),
                                     xRicetta_Programmazione_cod,
                                     CDate(xRicetta.GetAttribute("validita_inizio")),
                                     CDate(xRicetta.GetAttribute("validita_fine")),
                                     xRicetta_Imputazione_Cod,
                                     xRicetta_Imputazione_Fase_Cod,
                                     objParametri,
                                     xRicetta_data_creazione,
                                     xRicetta_data_Modifica,
                                     xRicetta_username_creazione,
                                     xRicetta_username_modifica,
                                     xRicetta_Origine)


                            Dummy = ObjRicetteLog.Scrivi(OpeDB_Ricetta,
                                                         "Ricette",
                                                         CInt(Ricetta_Cod),
                                                         CStr(xRicetta.GetAttribute("piva")),
                                                         IIf(xRicetta.GetAttribute("sa_cod") = String.Empty, "0", xRicetta.GetAttribute("sa_cod")),
                                                         Tipo_Ricetta,
                                                         xRicetta_Programmazione_cod,
                                                         Nothing,
                                                         CStr(xRicetta.GetAttribute("ricetta_des")),
                                                         CInt(idServizio),
                                                         objParametri,
                                                         DatiRicetta)




                            '
                        Case "2"    'MODIFICA

                            Dim xRicetta_data_creazione As Date = #2/1/1900#
                            Dim xRicetta_data_Modifica As Date = #2/1/1900#
                            Dim xRicetta_username_creazione As String = ""
                            Dim xRicetta_username_modifica As String = ""

                            '  Marco Grilli, 07/04/2016 14:43:18: Parametri gestiti dal LAN e quindi valorizzati a 0 per l'online
                            Dim xRicetta_Imputazione_Cod As Integer = 0
                            Dim xRicetta_Imputazione_Fase_Cod As Integer = 0

                            If Not IsNothing(xRicetta.GetAttribute("data_creazione")) AndAlso
                             xRicetta.GetAttribute("data_creazione") <> "" Then
                                xRicetta_data_creazione = CDate(xRicetta.GetAttribute("data_creazione"))
                            End If

                            If Not IsNothing(xRicetta.GetAttribute("data_modifica")) AndAlso
                             xRicetta.GetAttribute("data_modifica") <> "" Then
                                xRicetta_data_Modifica = CDate(xRicetta.GetAttribute("data_modifica"))
                            End If

                            If Not IsNothing(xRicetta.GetAttribute("username_creazione")) Then
                                xRicetta_username_creazione = CStr(xRicetta.GetAttribute("username_creazione"))
                            End If

                            If Not IsNothing(xRicetta.GetAttribute("username_modifica")) Then
                                xRicetta_username_modifica = CStr(xRicetta.GetAttribute("username_modifica"))
                            End If


                            If Not IsNothing(xRicetta.GetAttribute("imputazione_cod")) AndAlso xRicetta.GetAttribute("imputazione_cod") <> "" Then
                                xRicetta_Imputazione_Cod = CInt(xRicetta.GetAttribute("imputazione_cod"))
                            End If

                            If Not IsNothing(xRicetta.GetAttribute("imputazione_fase_cod")) AndAlso xRicetta.GetAttribute("imputazione_fase_cod") <> "" Then
                                xRicetta_Imputazione_Fase_Cod = CInt(xRicetta.GetAttribute("imputazione_fase_cod"))
                            End If

                            'Se passo nell'XML il programmazione_cod allora lo metto, se no metto il default
                            Dim xRicetta_Programmazione_cod As Integer
                            If Not IsNothing(xRicetta.GetAttribute("programmazione_cod")) AndAlso xRicetta.GetAttribute("programmazione_cod") <> "0" Then
                                xRicetta_Programmazione_cod = CInt(xRicetta.GetAttribute("programmazione_cod"))
                            Else
                                xRicetta_Programmazione_cod = Agro_XML_GetInteger(xRicetta, "programmazione_cod", 0)
                            End If

                            Dim xRicetta_Origine As String = ""
                            If Not IsNothing(xRicetta.GetAttribute("origine")) AndAlso xRicetta.GetAttribute("origine") <> "" Then
                                xRicetta_Origine = CStr(xRicetta.GetAttribute("origine"))
                            End If

                            Dummy = ObjRicetta.Modifica(
                                     CInt(Ricetta_Cod),
                                     CStr(xRicetta.GetAttribute("ricetta_numero")),
                                     CStr(xRicetta.GetAttribute("piva")),
                                     IIf(xRicetta.GetAttribute("sa_cod") = String.Empty, "0", xRicetta.GetAttribute("sa_cod")),
                                     Tipo_Ricetta,
                                     CStr(xRicetta.GetAttribute("ricetta_des")),
                                     CStr(xRicetta.GetAttribute("ricetta_des_long")),
                                     xRicetta_Programmazione_cod,
                                     CInt(xRicetta.GetAttribute("veg_cod")),
                                     CStr(xRicetta.GetAttribute("note")),
                                     CDate(xRicetta.GetAttribute("validita_inizio")),
                                     CDate(xRicetta.GetAttribute("validita_fine")),
                                     xRicetta_Imputazione_Cod,
                                     xRicetta_Imputazione_Fase_Cod,
                                     "",
                                     objParametri,
                                     xRicetta_Origine)

                            Dummy = ObjRicetteLog.Scrivi(OpeDB_Ricetta,
                                                         "Ricette",
                                                         CInt(Ricetta_Cod),
                                                         CStr(xRicetta.GetAttribute("piva")),
                                                         IIf(xRicetta.GetAttribute("sa_cod") = String.Empty, "0", xRicetta.GetAttribute("sa_cod")),
                                                         Tipo_Ricetta,
                                                         xRicetta_Programmazione_cod,
                                                         Nothing,
                                                         CStr(xRicetta.GetAttribute("ricetta_des")),
                                                         CInt(idServizio),
                                                         objParametri,
                                                         DatiRicetta)

                        Case "3"   'CANCELLAZIONE

                    End Select

                    '##################################################
                    '##########  RICETTAXCULTIVAR  ####################
                    '##################################################



                    xListaDatiRicettaxCultivar = xRicetta.GetElementsByTagName("DatiRicettaxCultivar")

                    i_DatiRicettaxCultivar = 0

                    Do While i_DatiRicettaxCultivar < xListaDatiRicettaxCultivar.Count

                        'Prelevo l'i-esimo blocco di DatiRicettaxCultivar (in realta' ne esiste uno solo)
                        xDatiRicettaxCultivar = xListaDatiRicettaxCultivar.Item(i_DatiRicettaxCultivar)

                        '------------------------------

                        xListaRicettaxCultivar = xDatiRicettaxCultivar.GetElementsByTagName("RicettaxCultivar")

                        i_RicettaxCultivar = 0

                        Do While i_RicettaxCultivar < xListaRicettaxCultivar.Count

                            'Prelevo l' i-esimo RicettaxCultivar
                            xRicettaxCultivar = xListaRicettaxCultivar.Item(i_RicettaxCultivar)

                            'Creo l'oggetto COM
                            objRicettaxCultivar = New AgronicaCoreContabDAL.RicettexCultivar_W

                            'Prelevo gli attributi dell'Operazione selezionata
                            OpeDB_RicettaxCultivar = xRicettaxCultivar.GetAttribute("TipoOperazioneDB")

                            'Prelevo gli attributi veg_cod e cul_cod
                            Veg_Cod = xRicettaxCultivar.GetAttribute("veg_cod")
                            Cul_Cod = xRicettaxCultivar.GetAttribute("cul_cod")


                            'Verifico l'operazione richiesta
                            Select Case OpeDB_RicettaxCultivar
                                '
                                Case "0"    'LEGGI -------------------------------------------------------
                                    '
                                Case "1"    'SALVA -------------------------------------------------------


                                    Dim xRicettaxCultivar_data_creazione As Date = #2/1/1900#
                                    Dim xRicettaxCultivar_data_Modifica As Date = #2/1/1900#
                                    Dim xRicettaxCultivar_username_creazione As String = ""
                                    Dim xRicettaxCultivar_username_modifica As String = ""

                                    If Not IsNothing(xRicettaxCultivar.GetAttribute("data_creazione")) AndAlso
                                     xRicettaxCultivar.GetAttribute("data_creazione") <> "" Then
                                        xRicettaxCultivar_data_creazione = CDate(xRicettaxCultivar.GetAttribute("data_creazione"))
                                    End If

                                    If Not IsNothing(xRicettaxCultivar.GetAttribute("data_modifica")) AndAlso
                                     xRicettaxCultivar.GetAttribute("data_modifica") <> "" Then
                                        xRicettaxCultivar_data_Modifica = CDate(xRicettaxCultivar.GetAttribute("data_modifica"))
                                    End If

                                    If Not IsNothing(xRicettaxCultivar.GetAttribute("username_creazione")) Then
                                        xRicettaxCultivar_username_creazione = CStr(xRicettaxCultivar.GetAttribute("username_creazione"))
                                    End If

                                    If Not IsNothing(xRicettaxCultivar.GetAttribute("username_modifica")) Then
                                        xRicettaxCultivar_username_modifica = CStr(xRicettaxCultivar.GetAttribute("username_modifica"))
                                    End If



                                    Dummy = objRicettaxCultivar.Scrivi(
                                                      CInt(Ricetta_Cod),
                                                      CInt(Veg_Cod),
                                                      CInt(Cul_Cod),
                                                      CDate(xRicettaxCultivar.GetAttribute("validita_inizio")),
                                                      CDate(xRicettaxCultivar.GetAttribute("validita_fine")),
                                                      objParametri,
                                                      xRicettaxCultivar_data_creazione,
                                                      xRicettaxCultivar_data_Modifica,
                                                      xRicettaxCultivar_username_creazione,
                                                      xRicettaxCultivar_username_modifica)


                                    '
                                Case "2"    'MODIFICA -------------------------------------------------------
                                    '
                                    objRicettaxCultivar.Modifica(
                                                      CInt(Ricetta_Cod),
                                                      CInt(Veg_Cod),
                                                      CInt(Cul_Cod),
                                                      CDate(xRicettaxCultivar.GetAttribute("validita_inizio")),
                                                      CDate(xRicettaxCultivar.GetAttribute("validita_fine")),
                                                      String.Empty,
                                                      objParametri)



                                Case "3"    'ELIMINA -------------------------------------------------------
                                    '

                                    objRicettaxCultivar.Cancella(
                                                CInt(Ricetta_Cod),
                                                CInt(Veg_Cod),
                                                CInt(Cul_Cod),
                                                String.Empty,
                                                objParametri)

                            End Select

                            i_RicettaxCultivar = i_RicettaxCultivar + 1

                        Loop

                        i_DatiRicettaxCultivar = i_DatiRicettaxCultivar + 1


                    Loop

                    xListaDatiRicettaxCultivar = Nothing
                    xDatiRicettaxCultivar = Nothing
                    xListaRicettaxCultivar = Nothing
                    xRicettaxCultivar = Nothing

                    objRicettaxCultivar = Nothing




                    '#######################################################################
                    '#######################################################################
                    '#######################################################################
                    '#######################################################################


                    '##################################################
                    '##########  NOTE              ####################
                    '##################################################

                    Dim xListaDatiRicettaxNote As XmlNodeList
                    Dim xDatiRicettaxNote As XmlElement
                    Dim xListaRicettaxNote As XmlNodeList
                    Dim xRicettaxNote As XmlElement

                    Dim i_DatiRicettaxNote As Integer = 0

                    xListaDatiRicettaxNote = xRicetta.GetElementsByTagName("DatiRicettaxNote")

                    Do While i_DatiRicettaxNote < xListaDatiRicettaxNote.Count

                        'Prelevo l'i-esimo blocco di DatiRicettaxCultivar (in realta' ne esiste uno solo)
                        xDatiRicettaxNote = xListaDatiRicettaxNote.Item(i_DatiRicettaxNote)

                        '------------------------------

                        xListaRicettaxNote = xDatiRicettaxNote.GetElementsByTagName("RicettaxNote")

                        Dim i_RicettaxNote As Integer = 0
                        i_RicettaxNote = 0

                        Do While i_RicettaxNote < xListaRicettaxNote.Count

                            'Prelevo l' i-esimo RicettaxCultivar
                            xRicettaxNote = xListaRicettaxNote.Item(i_RicettaxNote)

                            'Creo l'oggetto COM
                            Dim objRicettaxNote As New AgronicaCoreContabDAL.RicettexNote_W

                            'Prelevo gli attributi dell'Operazione selezionata
                            OpeDB_RicettaxNote = xRicettaxNote.GetAttribute("TipoOperazioneDB")


                            'Verifico l'operazione richiesta
                            Select Case OpeDB_RicettaxNote
                                '
                                Case "0"    'LEGGI -------------------------------------------------------
                                    '
                                Case "1"    'SALVA -------------------------------------------------------


                                    Dim xricettaxnote_data_creazione As Date = #2/1/1900#
                                    Dim xricettaxnote_data_Modifica As Date = #2/1/1900#
                                    Dim xricettaxnote_username_creazione As String = ""
                                    Dim xricettaxnote_username_modifica As String = ""

                                    If Not IsNothing(xRicettaxNote.GetAttribute("data_creazione")) AndAlso
                                     xRicettaxNote.GetAttribute("data_creazione") <> "" Then
                                        xricettaxnote_data_creazione = CDate(xRicettaxNote.GetAttribute("data_creazione"))
                                    End If

                                    If Not IsNothing(xRicettaxNote.GetAttribute("data_modifica")) AndAlso
                                     xRicettaxNote.GetAttribute("data_modifica") <> "" Then
                                        xricettaxnote_data_Modifica = CDate(xRicettaxNote.GetAttribute("data_modifica"))
                                    End If

                                    If Not IsNothing(xRicettaxNote.GetAttribute("username_creazione")) Then
                                        xricettaxnote_username_creazione = CStr(xRicettaxNote.GetAttribute("username_creazione"))
                                    End If

                                    If Not IsNothing(xRicettaxNote.GetAttribute("username_modifica")) Then
                                        xricettaxnote_username_modifica = CStr(xRicettaxNote.GetAttribute("username_modifica"))
                                    End If



                                    Dummy = objRicettaxNote.Scrivi(
                                                      CInt(Ricetta_Cod),
                                                      0,
                                                      CInt(xRicettaxNote.GetAttribute("nota_cod")),
                                                      CDate(xRicettaxNote.GetAttribute("validita_inizio")),
                                                      CDate(xRicettaxNote.GetAttribute("validita_fine")),
                                                      objParametri,
                                                      xricettaxnote_data_creazione,
                                                      xricettaxnote_data_Modifica,
                                                      xricettaxnote_username_creazione,
                                                      xricettaxnote_username_modifica)


                                    '
                                Case "2"    'MODIFICA -------------------------------------------------------
                                    '
                                    objRicettaxNote.Modifica(
                                                      CInt(Ricetta_Cod),
                                                      0,
                                                      CInt(xRicettaxNote.GetAttribute("nota_cod")),
                                                      CDate(xRicettaxNote.GetAttribute("validita_inizio")),
                                                      CDate(xRicettaxNote.GetAttribute("validita_fine")),
                                                      String.Empty,
                                                      objParametri)



                                Case "3"    'ELIMINA -------------------------------------------------------
                                    '

                                    objRicettaxNote.Cancella(
                                                CInt(Ricetta_Cod),
                                                0,
                                                CInt(xRicettaxNote.GetAttribute("nota_cod")),
                                                String.Empty,
                                                objParametri)

                            End Select

                            i_RicettaxNote = i_RicettaxNote + 1
                            objRicettaxNote = Nothing

                        Loop

                        i_DatiRicettaxNote = i_DatiRicettaxNote + 1


                    Loop

                    xListaDatiRicettaxNote = Nothing
                    xDatiRicettaxNote = Nothing
                    xListaRicettaxNote = Nothing
                    xRicettaxNote = Nothing



                    '##################################################
                    '##########  AGENDA            ####################
                    '##################################################

                    Dim xListaDatiRicettaxAgenda As XmlNodeList
                    Dim xDatiRicettaxAgenda As XmlElement
                    Dim xListaRicettaxAgenda As XmlNodeList
                    Dim xRicettaxAgenda As XmlElement

                    Dim i_DatiRicettaxAgenda As Integer = 0

                    xListaDatiRicettaxAgenda = xRicetta.GetElementsByTagName("DatiRicettaxAgenda")

                    Do While i_DatiRicettaxAgenda < xListaDatiRicettaxAgenda.Count

                        'Prelevo l'i-esimo blocco di DatiRicettaxCultivar (in realta' ne esiste uno solo)
                        xDatiRicettaxAgenda = xListaDatiRicettaxAgenda.Item(i_DatiRicettaxAgenda)

                        '------------------------------

                        xListaRicettaxAgenda = xDatiRicettaxAgenda.GetElementsByTagName("RicettaxAgenda")

                        Dim i_RicettaxAgenda As Integer = 0
                        i_RicettaxAgenda = 0

                        Do While i_RicettaxAgenda < xListaRicettaxAgenda.Count

                            'Prelevo l' i-esimo RicettaxCultivar
                            xRicettaxAgenda = xListaRicettaxAgenda.Item(i_RicettaxAgenda)

                            'Creo l'oggetto COM
                            Dim objRicettaxAgenda As New AgronicaCoreContabDAL.RicettexAgenda_W

                            'Prelevo gli attributi dell'Operazione selezionata
                            OpeDB_RicettaxAgenda = xRicettaxAgenda.GetAttribute("TipoOperazioneDB")


                            'Verifico l'operazione richiesta
                            Select Case OpeDB_RicettaxAgenda
                                '
                                Case "0"    'LEGGI -------------------------------------------------------
                                    '
                                Case "1"    'SALVA -------------------------------------------------------


                                    Dim xRicettaXAgenda_data_creazione As Date = #2/1/1900#
                                    Dim xRicettaXAgenda_data_Modifica As Date = #2/1/1900#
                                    Dim xRicettaXAgenda_username_creazione As String = ""
                                    Dim xRicettaXAgenda_username_modifica As String = ""

                                    If Not IsNothing(xRicettaxAgenda.GetAttribute("data_creazione")) AndAlso
                                     xRicettaxAgenda.GetAttribute("data_creazione") <> "" Then
                                        xRicettaXAgenda_data_creazione = CDate(xRicettaxAgenda.GetAttribute("data_creazione"))
                                    End If

                                    If Not IsNothing(xRicettaxAgenda.GetAttribute("data_modifica")) AndAlso
                                     xRicettaxAgenda.GetAttribute("data_modifica") <> "" Then
                                        xRicettaXAgenda_data_Modifica = CDate(xRicettaxAgenda.GetAttribute("data_modifica"))
                                    End If

                                    If Not IsNothing(xRicettaxAgenda.GetAttribute("username_creazione")) Then
                                        xRicettaXAgenda_username_creazione = CStr(xRicettaxAgenda.GetAttribute("username_creazione"))
                                    End If

                                    If Not IsNothing(xRicettaxAgenda.GetAttribute("username_modifica")) Then
                                        xRicettaXAgenda_username_modifica = CStr(xRicettaxAgenda.GetAttribute("username_modifica"))
                                    End If



                                    Dummy = objRicettaxAgenda.Scrivi(
                                                      CInt(Ricetta_Cod),
                                                      0,
                                                      CInt(xRicettaxAgenda.GetAttribute("id_agenda")),
                                                      CDate(xRicettaxAgenda.GetAttribute("validita_inizio")),
                                                      CDate(xRicettaxAgenda.GetAttribute("validita_fine")),
                                                      objParametri,
                                                      xRicettaXAgenda_data_creazione,
                                                      xRicettaXAgenda_data_Modifica,
                                                      xRicettaXAgenda_username_creazione,
                                                      xRicettaXAgenda_username_modifica)


                                    '
                                Case "2"    'MODIFICA -------------------------------------------------------
                                    '
                                    objRicettaxAgenda.Modifica(
                                                      CInt(Ricetta_Cod),
                                                      0,
                                                      CInt(xRicettaxAgenda.GetAttribute("id_agenda")),
                                                      CDate(xRicettaxAgenda.GetAttribute("validita_inizio")),
                                                      CDate(xRicettaxAgenda.GetAttribute("validita_fine")),
                                                      String.Empty,
                                                      objParametri)



                                Case "3"    'ELIMINA -------------------------------------------------------
                                    '

                                    objRicettaxAgenda.Cancella(
                                                CInt(Ricetta_Cod),
                                                0,
                                                CInt(xRicettaxAgenda.GetAttribute("id_agenda")),
                                                String.Empty,
                                                objParametri)

                            End Select

                            i_RicettaxAgenda = i_RicettaxAgenda + 1
                            objRicettaxAgenda = Nothing

                        Loop

                        i_DatiRicettaxAgenda = i_DatiRicettaxAgenda + 1


                    Loop

                    xListaDatiRicettaxAgenda = Nothing
                    xDatiRicettaxAgenda = Nothing
                    xListaRicettaxAgenda = Nothing
                    xRicettaxAgenda = Nothing



                    '#######################################################################
                    '#######################################################################
                    '#######################################################################
                    '#######################################################################





                    '#####################################
                    '##########  OPERAZIONI   ############
                    '#####################################


                    'Prelevo l'elenco delle operazioni
                    XmlDatiRicetta_Operazioni = xRicetta.GetElementsByTagName("DatiRicetta_Operazioni")

                    If XmlDatiRicetta_Operazioni.Count > 0 Then

                        XmlDatiRicetta_Operazione = XmlDatiRicetta_Operazioni.Item(0)

                        DatiRicetta_Operazioni = XmlDatiRicetta_Operazione.OuterXml

                        'Creo l'oggetto COM
                        objRicetta_Operazioni = New AgronicaCoreContabBIZ.Ricette_Operazioni_W

                        Dummy = objRicetta_Operazioni.Ricetta_Operazione_Scrivi(
                                                          CStr(xRicetta.GetAttribute("piva")),
                                                             IIf(xRicetta.GetAttribute("sa_cod") = String.Empty, "0", xRicetta.GetAttribute("sa_cod")),
                                                           CStr(DatiRicetta_Operazioni),
                                                            CInt(Ricetta_Cod),
                                                            Tipo_Ricetta,
                                                            Ricetta_Operazione_Cod,
                                                            objParametri)

                        objRicetta_Operazioni = Nothing

                    End If


                    Select Case OpeDB_Ricetta


                        Case 3 'CANCELLAZIONE

                            ObjRicetta.Cancella(
                                   CInt(Ricetta_Cod),
                                   String.Empty,
                                   objParametri)

                            Dummy = ObjRicetteLog.Scrivi(OpeDB_Ricetta,
                                                         "Ricette",
                                                         CInt(Ricetta_Cod),
                                                         CStr(xRicetta.GetAttribute("piva")),
                                                         IIf(xRicetta.GetAttribute("sa_cod") = String.Empty, "0", xRicetta.GetAttribute("sa_cod")),
                                                         Tipo_Ricetta,
                                                         0,
                                                         Nothing,
                                                         CStr(xRicetta.GetAttribute("ricetta_des")),
                                                         CInt(idServizio),
                                                         objParametri)

                    End Select


                    'Elimino l'oggetto
                    ObjRicetta = Nothing

                    '-------------------------------------------------------------


                    '-------------------------------------------------------------

                    'Incremento l'indice
                    i_Ricetta = i_Ricetta + 1

                Loop

                '------------------------------

                'Incremento l'indice
                i_DatiRicetta = i_DatiRicetta + 1

            Loop

            '------------------------------
            '------------------------------
            '------------------------------

            'Elimino tutti gli oggetti utilizzati

            xDatiRicette = Nothing
            xDatiRicetta = Nothing
            xRicette = Nothing
            xRicetta = Nothing
            XmlDoc = Nothing

            '------------------------------

            'Restituisco un valore Dummy
            xRisp = True

            'Se ho la transazione è stata avviata in questa routine faccio il commit
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

            '----------------------------------------------------------------------------

        Catch ex As Exception

            'Restituisco un valore Dummy
            xRisp = False

            'Faccio il rollback della transazione
            If Not objParametri.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)

        End Try

        Return xRisp


    End Function

    Public Function Ricetta_Copia(ByVal Ricetta_Cod_DaCopiare As Integer,
                                  ByRef OUTPUT_Ricetta_Cod As Integer,
                                  ByVal IndiceProgressivoGIAS As Integer,
                                  ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim ric_R As New AgronicaCoreContabBIZ.Ricette_R
        Dim StringaXmlRicetta As String = ric_R.Ricetta_Leggi(Ricetta_Cod_DaCopiare, "", 0, 0, 0, False, objParametri_Server)

        'se sto copiando una ricetta devo eliminare i riferimenti a ricettexagenda, che si riferiscono all'operazione copiata
        Dim doc As New XmlDocument()
        doc.LoadXml(StringaXmlRicetta)

        'Elimino i riferimenti a ricettexagenda
        Dim nodi As XmlNodeList = doc.SelectNodes("//DatiRicettaxAgenda_2")
        For Each n As XmlNode In nodi
            n.ParentNode.RemoveChild(n)
        Next

        'Elimino i riferimenti ad altre ricette, tabelle di interscambio e dati su creazione/modifica
        nodi = doc.SelectNodes("//*[@username_creazione] | //*[@username_modifica] | //*[@data_creazione] | //*[@data_modifica] | //*[@ricetta_operazione_cod_rif] | //*[@app_ricetta_operazione_id]")
        For Each n As XmlNode In nodi
            n.Attributes.RemoveNamedItem("username_creazione")
            n.Attributes.RemoveNamedItem("username_modifica")
            n.Attributes.RemoveNamedItem("data_creazione")
            n.Attributes.RemoveNamedItem("data_modifica")
            n.Attributes.RemoveNamedItem("ricetta_operazione_cod_rif")
            n.Attributes.RemoveNamedItem("app_ricetta_operazione_id")
            n.Attributes.RemoveNamedItem("invia_app")
            n.Attributes.RemoveNamedItem("invia_hubiot")
        Next

        'Assegno un nuovo nome
        Dim piva As String = doc.SelectSingleNode("//Ricetta/@piva").Value
        Dim data_operazione As DateTime = doc.SelectSingleNode("//Ricetta_Operazione/@validita_inizio").Value
        Dim r_R As New AgronicaCoreContabBIZ.Ricette_R
        Dim nome_ricetta As String = r_R.Genera_Nuovo_Nome_Ricetta(piva, data_operazione, objParametri_Server)
        doc.SelectSingleNode("//Ricetta/@ricetta_numero").Value = nome_ricetta
        doc.SelectSingleNode("//Ricetta/@ricetta_des").Value = nome_ricetta
        doc.SelectSingleNode("//Ricetta/@ricetta_des_long").Value = nome_ricetta

        'aggiungo baseCode e topCode
        Dim baseCod As Integer = 0
        Dim topCod As Integer = 0
        Call Calcola_BaseCode_TopCode(baseCod, topCod, IndiceProgressivoGIAS)
        nodi = doc.SelectNodes("//*")
        For Each n As XmlNode In nodi
            Dim bc As XmlAttribute = doc.CreateAttribute("basecode")
            bc.Value = baseCod
            n.Attributes.Append(bc)
            Dim tc As XmlAttribute = doc.CreateAttribute("topcode")
            tc.Value = topCod
            n.Attributes.Append(tc)
        Next

        StringaXmlRicetta = doc.OuterXml
        StringaXmlRicetta = StringaXmlRicetta.Replace("TipoOperazioneDB=""0""", "TipoOperazioneDB =""1""")

        'Azzero eventuali chiavi. Il trucco è di rendere negativi i codici in modo tale che il componente ne crei dei nuovi
        StringaXmlRicetta = StringaXmlRicetta.Replace("ricetta_cod=""", "ricetta_cod =""-")
        StringaXmlRicetta = StringaXmlRicetta.Replace("ricetta_operazione_cod=""", "ricetta_operazione_cod =""-")
        StringaXmlRicetta = StringaXmlRicetta.Replace("ricetta_dettaglio_cod=""", "ricetta_dettaglio_cod =""-")
        StringaXmlRicetta = StringaXmlRicetta.Replace("ricetta_tecnico_cod=""", "ricetta_tecnico_cod =""-")
        StringaXmlRicetta = StringaXmlRicetta.Replace("ricetta_destinazione_cod=""", "ricetta_destinazione_cod =""-")


        'genero i raccoglitore_cod corretti
        Dim raccoglitoreHashSet As New HashSet(Of Integer)
        Dim raccoglitoreDict As New Dictionary(Of Integer, Integer)
        Dim ricettaOperazioneNodes = doc.SelectNodes("//Ricetta_Operazione")
        For Each ricettaOperazioneNode As XmlNode In ricettaOperazioneNodes
            Dim raccoglitoreCodOld = 0
            If ricettaOperazioneNode.Attributes.GetNamedItem("raccoglitore_cod") IsNot Nothing Then
                If ricettaOperazioneNode.Attributes("raccoglitore_cod").Value <> "" Then
                    raccoglitoreCodOld = ricettaOperazioneNode.Attributes("raccoglitore_cod").Value
                End If
            End If
            If raccoglitoreCodOld <> 0 Then
                raccoglitoreHashSet.Add(raccoglitoreCodOld)
            End If
        Next

        Dim objSequenze As New Agro_Sequenze
        For Each raccoglitoreKey In raccoglitoreHashSet
            'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
            'Raccoglitore_Cod = seq.NuovoId_Tabella("raccoglitore", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri_Server)
            raccoglitoreDict.Add(raccoglitoreKey, objSequenze.NuovoId_Tabella("raccoglitore", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri_Server))
        Next

        For Each raccoglitoreItem In raccoglitoreDict
            Dim strReplaceFrom As String = "raccoglitore_cod=""" & raccoglitoreItem.Key & """"
            Dim strReplaceTo As String = "raccoglitore_cod=""" & raccoglitoreItem.Value & """"
            StringaXmlRicetta = StringaXmlRicetta.Replace(strReplaceFrom, strReplaceTo)
        Next

        'Inserisco la NUOVA RICETTA
        Dim esito As Boolean = Ricetta_Scrivi(StringaXmlRicetta, OUTPUT_Ricetta_Cod, objParametri_Server)

        Return esito

    End Function

    Public Function ImportaAgendDaTabelleAPP(
            ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
            ByRef msgFinale As String,
            Optional ByVal piva As String = "",
            Optional ByVal guid As String = "",
            Optional ByRef riferimento As String = "",
            Optional ByRef destinazioni As List(Of String) = Nothing
        ) As Boolean


        Dim agnd_W As New AgronicaCoreContabDAL.Agenda_W
        Dim agndMov_W As New AgronicaCoreContabDAL.Movimenti_W
        Dim agndrDet_W As New AgronicaCoreContabDAL.Movimenti_Dettagli_W
        Dim agndrtec_W As New AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_W
        Dim agndrdest_W As New AgronicaCoreContabDAL.Mov_Destinazioni_W

        Dim listaRicetteApp As List(Of APP_Ricette)
        Dim listaOperazioniApp As List(Of APP_Ricette_Operazioni)
        Dim listaDettaglioApp As List(Of APP_Ricette_Dettagli)
        Dim listaTecnicoApp As List(Of APP_Ricette_Dettaglio_Tecnico)
        Dim listaDestinazioneApp As List(Of APP_Ricette_Destinazioni)

        Dim seq As New Agro_Sequenze()
        Dim dicIDAgenda As New Dictionary(Of String, Integer)
        Dim dicMovimento As New Dictionary(Of String, Integer)
        Dim dic_Ricetta_Operazione_Cod As New Dictionary(Of String, Integer)
        Dim dicMovimentoDettaglio As New Dictionary(Of String, Integer)
        Dim dic_Ricetta_Tecnico_Cod As New Dictionary(Of String, Integer)
        Dim dicMovDestinazioni As New Dictionary(Of String, Integer)

        Dim ScriviRicetteAPP As New AgronicaCoreContabDAL.APP_Ricette_Operazioni_W

        Dim ricetteOperazioniImportate As Integer = 0
        Dim ricetteOperazioniDaImportare As Integer = 0
        Dim ricetteImportate As Integer = 0

        Dim msgErr As String = ""
        Dim esitoFinale As Boolean = False

        Try


            Dim gefutils As New Gias_EF_Utility
            Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
            Dim dal As Gias_DeveloperServer_Entities = Nothing

            'Leggo solo le verificate -> TODO
            ImportaRicetteDaTabelleAPP_LetturaOggetti(piva, listaRicetteApp, listaOperazioniApp, listaDettaglioApp, listaTecnicoApp, listaDestinazioneApp, efConnString, True, guid)

            ricetteOperazioniDaImportare = listaRicetteApp.Count()

            For Each dr As APP_Ricette In listaRicetteApp


                Dim FlagTransazioneLocale As Boolean = False
                Dim FlagConnessioneLocale As Boolean = False

                'Filtro le liste per la ricetta in oggetto
                Dim listaOperazioniApp_filtrata As List(Of APP_Ricette_Operazioni) = (From r In listaOperazioniApp Where r.Ricetta_Cod = dr.Ricetta_Cod AndAlso r.ID.Split("|")(0) = dr.ID.Split("|")(0) Select r).ToList()
                Dim listaDettaglioApp_filtrata As List(Of APP_Ricette_Dettagli) = (From r In listaDettaglioApp Where r.Ricetta_Cod = dr.Ricetta_Cod AndAlso r.ID.Split("|")(0) = dr.ID.Split("|")(0) Select r).ToList()
                Dim listaTecnicoApp_filtrata As List(Of APP_Ricette_Dettaglio_Tecnico) = (From r In listaTecnicoApp Where r.Ricetta_Cod = dr.Ricetta_Cod AndAlso r.ID.Split("|")(0) = dr.ID.Split("|")(0) Select r).ToList()
                Dim listaDestinazioneApp_filtrata As List(Of APP_Ricette_Destinazioni) = (From r In listaDestinazioneApp Where r.Ricetta_Cod = dr.Ricetta_Cod AndAlso r.ID.Split("|")(0) = dr.ID.Split("|")(0) Select r).ToList()

                ' VAnni: 10/5/2021: bug fix: importazione Agenda da Tabelle frontiera APP: se ci sono più operazioni per ciascun upload non vegono creati i record nella tabella movimenti
                ' ... il dictionary dei movimenti va ripulito per ciascuna ricetta, poichè non è ricercabile per chiave
                dicMovimento.Clear()

                'TODO: Gestire la tranzazione
                Try

                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    'Apro la connessione al DB
                    AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                            FlagTransazioneLocale,
                                                                            objParametri_Server)

                    verificaCorrettezzaFormale_PerImportazione(dr, listaOperazioniApp_filtrata, listaDettaglioApp_filtrata, listaTecnicoApp_filtrata, listaDestinazioneApp_filtrata, objParametri_Server)

                    Dim id_Agenda As Integer
                    Dim esito As Boolean
                    Dim DataOperazione As Date
                    Dim NoteOperazione As String = ""

                    '########### Ricette Operazioni APP --> Agenda  ########### 
                    For Each elem As APP_Ricette_Operazioni In listaOperazioniApp_filtrata

                        If dr.Ricetta_Cod > 0 Then
                            id_Agenda = dr.Ricetta_Cod
                        Else
                            Dim chiave_APP As String = dr.ID.Split("|")(0) & "|" & dr.Ricetta_Cod
                            If dicIDAgenda.ContainsKey(chiave_APP) Then
                                id_Agenda = dicIDAgenda(chiave_APP)
                            Else
                                id_Agenda = seq.NuovoId_Tabella("agenda", 0, 2000000000, objParametri_Server)
                                dicIDAgenda.Add(chiave_APP, id_Agenda)
                            End If
                        End If

                        DataOperazione = elem.Validita_Inizio
                        NoteOperazione = elem.Note

                        Dim origine = If(Not String.IsNullOrEmpty(dr.Origine), dr.Origine, enum_OrigineApp.GiasApp)

                        esito = agnd_W.Scrivi(
                            Piva:=dr.Piva,
                            Sa_Cod:=dr.Sa_Cod,
                            Id_Agenda:=id_Agenda,
                            Lav_Cod:=elem.Lav_Cod,
                            Linea_Cod:=0,
                            Preparazione_Cod:=0,
                            Id_Trasformazione:=0,
                            Des_Lib:=elem.Ricetta_Operazione_Des,
                            Tipo_Accettazione:=0,
                            Blocco_Flag:=0,
                            Blocco_Username:="",
                            Blocco_Data:=AGRODATAINIZIO,
                            Validita_Inizio:=DataOperazione,
                            Validita_Fine:=AGRODATAFINE,
                            objParametri:=objParametri_Server,
                            Origine:=origine
                            )

                        Dim ObjAgendaLog As New AgronicaCoreContabDAL.AgronicaLogAgenda_W
                        Dim Dummy As Boolean = ObjAgendaLog.Scrivi(
                            elem.Validita_Inizio,
                            enum_TipoOperazioneDB.Scrittura,
                            elem.Ricetta_Operazione_Des,
                            id_Agenda,
                            dr.Piva,
                            dr.Sa_Cod,
                            elem.Lav_Cod,
                            enum_Id_Servizio.GiasOnline,
                            objParametri_Server
                         )


                        If esito = False Then
                            Throw New Exception("Errore durante il salvataggio dell' operazione in tab Agenda (OP: " & elem.ID & ")")
                        End If

                    Next
                    'Operazione

                    Dim id_mov As Integer

                    '########### Ricette Dettagli --> Movimenti e mov_Dettagli ########### 
                    For Each elem As APP_Ricette_Dettagli In listaDettaglioApp_filtrata


                        'se non è stato creato il movimento
                        If dicMovimento.Count = 0 Then
                            id_mov = seq.NuovoId_Tabella("MOVIMENTI", 0, 2000000000, objParametri_Server)
                            Dim chiave_APP As String = elem.ID.Split("|")(0) & "|" & elem.Ricetta_Dettaglio_Cod
                            dicMovimento.Add(chiave_APP, id_mov)

                            agndMov_W.Scrivi(
                                Piva:=dr.Piva,
                                Sa_Cod:=dr.Sa_Cod,
                                Id_Agenda:=id_Agenda,
                                Id_Mov:=id_mov,
                                Cod_RisUm:=0,
                                Cau_Mov:=elem.Cau_Mov,
                                Mov_Desc:=If(String.IsNullOrEmpty(elem.Descrizione), NoteOperazione, elem.Descrizione),
                                Data_Movimento:=DataOperazione,
                                Scadenza:=AGRODATAFINE,
                                Scadenza_Extra:=AGRODATAFINE,
                                Doc_Numero:=0,
                                Num_Protocollo:=-1,
                                Cod_IndirizzoRisUm:=0,
                                Cod_Destinazione:=0,
                                Cod_IndirizzoDestinazione:=0,
                                Mezzo:=0,
                                Cod_Vettore:=0,
                                Cod_IndirizzoVettore:=0,
                                Causale_Trasporto:="",
                                Aspetto:="",
                                Peso:=0,
                                Ora:=DataOperazione,
                                Colli:=0,
                                Tipo_Sconto:=0,
                                Extra_Str:="",
                                Extra_Int:=0,
                                Extra_Date:=AGRODATAINIZIO,
                                Doc_Numero_Sin:="",
                                Doc_Numero_Des:="",
                                Natura_Beni:="",
                                Tara_Veicolo:=0,
                                Tara_Imballi:=0,
                                Tipo_Peso:=0,
                                Modalita:=0,
                                Username_Note:="",
                                Progr_Protocollo:=0,
                                Progr_Registrazione:=0,
                                Data_Registrazione:=AGRODATAINIZIO,
                                ChkLayOut_Bypass_Fatturato:=0,
                                ChkLayOut_Join_Prodotti:=0,
                                Validita_Inizio:=AGRODATAINIZIO,
                                Validita_Fine:=AGRODATAFINE,
                                Disciplinare_PubblicoPrivato:=0,
                                objParametri:=objParametri_Server
                            )



                        End If

                        Dim id_mov_det As Integer
                        If elem.Ricetta_Dettaglio_Cod > 0 Then
                            id_mov_det = elem.Ricetta_Dettaglio_Cod
                        Else
                            Dim chiave_APP As String = elem.ID.Split("|")(0) & "|" & elem.Ricetta_Dettaglio_Cod
                            If dicMovimentoDettaglio.ContainsKey(chiave_APP) Then
                                id_mov_det = dicMovimentoDettaglio(chiave_APP)
                            Else
                                id_mov_det = seq.NuovoId_Tabella("MOVIMENTI_DETTAGLI", 0, 2000000000, objParametri_Server)
                                dicMovimentoDettaglio.Add(chiave_APP, id_mov_det)
                            End If
                        End If


                        esito = agndrDet_W.Scrivi(
                                Piva:=dr.Piva,
                                Sa_Cod:=dr.Sa_Cod,
                                Id_Agenda:=id_Agenda,
                                Id_Mov:=id_mov,
                                Id_Mov_Det:=id_mov_det,
                                Elem_Cod:=If(IsNothing(elem.Elem_Cod), 0, elem.Elem_Cod),
                                Pro_Cod:=If(IsNothing(elem.Pro_Cod), 0, elem.Pro_Cod),
                                Mat_Cod:=If(IsNothing(elem.Mat_Cod), 0, elem.Mat_Cod),
                                Mov_Det_Des:=elem.Descrizione,
                                Qta:=If(IsNothing(elem.Qta), 0, elem.Qta),
                                Udm_Cod:=If(IsNothing(elem.Udm_Cod), 0, elem.Udm_Cod),
                                Cod_Iva:=0,
                                Jolly_Int:=0,
                                Sconto:=0,
                                Prezzo_Unitario:=0,
                                Prezzo_Unitario_Netto:=0,
                                Cod_Conto:=0,
                                Cal_Cod:=0,
                                Cod_Progetto:=0,
                                Fase_Cod:=0,
                                Extra_Str:="",
                                Extra_Int:=0,
                                Extra_Date:=AGRODATAINIZIO,
                                Ric_Cod:=0,
                                Anno:=AGRODATAINIZIO.Year(),
                                Imponibile:=0,
                                Imponibile_Netto:=0,
                                Iva:=0,
                                Listino_Cod:=0,
                                Contabilizzato:=1,
                                Pendente:=3,
                                Lotto:=elem.Lotto,
                                Udm_Cod_Extra:=If(IsNothing(elem.Udm_Cod_Extra), 0, elem.Udm_Cod_Extra),
                                Qta_Extra:=If(IsNothing(elem.Qta_Extra), 0, elem.Qta_Extra),
                                Qta_Extra_Totale:=If(IsNothing(elem.Qta_Extra_Totale), 0, elem.Qta_Extra_Totale),
                                Prezzo_Effettivo:=0,
                                Variazione:=0,
                                Tara:=0,
                                ChkLayOut_Hide:=0,
                                ChkIva_Manuale:=0,
                                Cod_IvaIndetraibile:=0,
                                TempoCarenza:=0,
                                DoseEtichetta:="",
                                Turno_Cod:=0,
                                ID_Attivita:=0,
                                Dettaglio_VegCod:=0,
                                PrincipiAttivi:="",
                                ClassiTossicologiche:="",
                                DoseEtichetta_Value:="",
                                Validita_Inizio:=elem.Validita_Inizio,
                                Validita_Fine:=elem.Validita_Fine,
                                objParametri:=objParametri_Server
                            )



                        If esito = False Then
                            Throw New Exception("Errore durante il salvataggio dell' operazione della Ricetta (DET: " & elem.ID & ")")
                        End If

                    Next

                    '########### Ricette Dettaglio Tecnico ########### 
                    For Each elem As APP_Ricette_Dettaglio_Tecnico In listaTecnicoApp_filtrata

                        Dim id_reg_dettaglio As Integer
                        If elem.Ricetta_Tecnico_Cod > 0 Then
                            id_reg_dettaglio = elem.Ricetta_Tecnico_Cod
                        Else
                            Dim chiave_APP As String = elem.ID.Split("|")(0) & "|" & elem.Ricetta_Tecnico_Cod
                            If dic_Ricetta_Tecnico_Cod.ContainsKey(chiave_APP) Then
                                id_reg_dettaglio = dic_Ricetta_Tecnico_Cod(chiave_APP)
                            Else
                                id_reg_dettaglio = seq.NuovoId_Tabella("MOVIMENTI_DETTAGLI_TECNICI", 0, 2000000000, objParametri_Server)
                                dic_Ricetta_Tecnico_Cod.Add(chiave_APP, id_reg_dettaglio)
                            End If
                        End If

                        Dim id_mov_det As Integer = dicMovimentoDettaglio(elem.ID.Split("|")(0) & "|" & elem.Ricetta_Dettaglio_Cod)

                        agndrtec_W.Scrivi(
                                Piva:=dr.Piva,
                                Sa_Cod:=dr.Sa_Cod,
                                Id_Agenda:=id_Agenda,
                                Id_Mov:=id_mov,
                                Id_Mov_Det:=id_mov_det,
                                Id_Reg_Dettaglio:=id_reg_dettaglio,
                                Av_Cod:=If(IsNothing(elem.Av_Cod), 0, elem.Av_Cod),
                                Av_Gru:=If(IsNothing(elem.Av_Gru), 0, elem.Av_Gru),
                                Sigla_AV:="",
                                Data_Ril:=AGRODATAINIZIO,
                                Qta_Ril:=If(IsNothing(elem.Qta_Ril), 0, elem.Qta_Ril),
                                Dose:=If(IsNothing(elem.Dose), 0, elem.Dose),
                                Ditta_Cod:=0,
                                Dett_Cod:=If(IsNothing(elem.Dett_Cod), 0, elem.Dett_Cod),
                                Id_Insetto:=0,
                                FF_Classe:=If(IsNothing(elem.FF_Classe), 0, elem.FF_Classe),
                                Mg:=0,
                                N:=If(IsNothing(elem.N), 0, elem.N),
                                P:=If(IsNothing(elem.P), 0, elem.P),
                                K:=If(IsNothing(elem.K), 0, elem.K),
                                Parziale:=If(IsNothing(elem.Parziale), 0, elem.Parziale),
                                Nitrati:=If(IsNothing(elem.Nitrati), 0, elem.Nitrati),
                                Freatimetro:=If(IsNothing(elem.Freatimetro), 0, elem.Freatimetro),
                                Piezo1:=If(IsNothing(elem.Piezo1), 0, elem.Piezo1),
                                Piezo2:=If(IsNothing(elem.piezo2), 0, elem.piezo2),
                                Piezo3:=If(IsNothing(elem.piezo3), 0, elem.piezo3),
                                Piezo4:=If(IsNothing(elem.piezo4), 0, elem.piezo4),
                                Trap_Num:=0,
                                Inn1_Data:=If(IsNothing(elem.Inn1_Data), 0, elem.Inn1_Data),
                                Inn2_Data:=If(IsNothing(elem.Inn2_Data), 0, elem.Inn2_Data),
                                Inn3_Data:=AGRODATAINIZIO,
                                Inn4_Data:=AGRODATAINIZIO,
                                Lotto:="",
                                Extra_Int:=0,
                                Extra_Str:="",
                                Extra_Date:=AGRODATAINIZIO,
                                Soglia_Cod:=0,
                                Soglia_Quantita:=0,
                                Soglia_Des:="",
                                Efficienza:=0,
                                Cu:=If(IsNothing(elem.CU), 0, elem.CU),
                                Validita_Inizio:=elem.Validita_Inizio,
                                Validita_Fine:=elem.Validita_Fine,
                                objParametri:=objParametri_Server
                            )


                        If esito = False Then
                            Throw New Exception("Errore durante il salvataggio dell' operazione della Ricetta (DETTEC: " & elem.ID & ")")
                        End If

                    Next

                    '########### Ricette Destinazioni ########### 
                    For Each elem As APP_Ricette_Destinazioni In listaDestinazioneApp_filtrata

                        Dim id_mov_det As Integer = dicMovimentoDettaglio(elem.ID.Split("|")(0) & "|" & elem.Ricetta_Dettaglio_Cod)

                        agndrdest_W.Scrivi(
                            Piva:=dr.Piva,
                            Sa_Cod:=dr.Sa_Cod,
                            Id_Agenda:=id_Agenda,
                            Id_Mov:=id_mov,
                            Id_Mov_Det:=id_mov_det,
                            Appezza:=elem.Appezza,
                            Id_Destinazione:=elem.Id_Reg,
                            Tipo_Destinazione:=elem.Tipo_Destinazione,
                            Qta:=If(IsNothing(elem.Qta), 0, elem.Qta),
                            Qta2:=If(IsNothing(elem.Qta2), 0, elem.Qta2),
                            Tipo_Scorta:=0,
                            Scorta_Min:=0,
                            mov_destinazioni_graphickey:="",
                            QuotaDistribuzione:=If(IsNothing(elem.QuotaDistribuzione), 0, elem.QuotaDistribuzione),
                            Validita_Inizio:=elem.Validita_Inizio,
                            Validita_Fine:=elem.Validita_Fine,
                            objParametri:=objParametri_Server
                        )

                        ' destinazioni x posizione rilievo
                        If destinazioni IsNot Nothing Then
                            destinazioni.Add(dr.Piva & "|" & dr.Sa_Cod & "|" & elem.Appezza & "|" & elem.Id_Reg & "|" & id_Agenda & "|" & id_mov_det)
                        End If

                        If esito = False Then
                            Throw New Exception("Errore durante il salvataggio dell' operazione della Ricetta (DEST: " & elem.ID & ")")
                        End If

                    Next

                    '########### Ricette Note ########### 
                    'TO DO


                    ''Segno come processati i record o la ricetta_operazione
                    'For Each elem As APP_Ricette_Operazioni In listaOperazioniApp_filtrata
                    '    dal.APP_Ricette_Operazioni.Attach(elem)
                    '    elem.Importato_Data = DateTime.Now
                    '    elem.MarkAsModified()
                    'Next

                    For Each elem As APP_Ricette_Operazioni In listaOperazioniApp_filtrata
                        ScriviRicetteAPP.APP_MarcaOperazioneImportata(elem.ID, "", objParametri_Server)
                    Next

                    If Not String.IsNullOrEmpty(guid) Then
                        riferimento = id_Agenda
                    End If

                    'Se è andato tutto bene                    
                    ricetteOperazioniImportate += listaOperazioniApp_filtrata.Count
                    ricetteImportate += 1

                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    'Chiudo la connessione al DB
                    AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)
                    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

                Catch ex As Exception

                    'Scrivo che non sono state importate
                    For Each elem As APP_Ricette_Operazioni In listaOperazioniApp_filtrata
                        ScriviRicetteAPP.APP_MarcaOperazioneImportata(elem.ID, ex.Message, objParametri_Server)
                    Next

                    'msgErr &= AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)
                    msgErr &= ex.Message & "<br>"

                    'Faccio il rollback della transazione
                    If Not objParametri_Server.objTransazione Is Nothing Then
                        'objParametri.objTransazione.Rollback()
                        'objParametri.objTransazione = Nothing
                        AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)

                    End If

                Finally

                    AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

                End Try

            Next

            If String.IsNullOrEmpty(guid) Then
                esitoFinale = True
                msgFinale = "Sono state importate correttamente " & ricetteOperazioniImportate & " operazioni su " & ricetteOperazioniDaImportare
                If Not String.IsNullOrEmpty(msgErr) Then
                    msgFinale &= "<br><br>ERRORI:<br>" & msgErr
                End If
            Else
                esitoFinale = ricetteOperazioniImportate = ricetteOperazioniDaImportare
                If Not String.IsNullOrEmpty(msgErr) Then
                    msgFinale = msgErr
                End If
            End If

        Catch ex As Exception

            esitoFinale = False
            msgFinale = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return esitoFinale
    End Function

    Public Function ImportaRicetteDaTabelleAPP(
            ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
            ByRef msgFinale As String,
            Optional ByVal piva As String = "",
            Optional ByVal guid As String = "",
            Optional ByRef riferimento As String = ""
        ) As Boolean

        Dim r_R As New AgronicaCoreContabDAL.Ricette_R
        Dim ro_R As New AgronicaCoreContabDAL.Ricette_Operazioni_R
        Dim rdet_R As New AgronicaCoreContabDAL.Ricette_Dettagli_R
        Dim rtec_R As New AgronicaCoreContabDAL.Ricette_Dett_Tecnico_R
        Dim rdest_R As New AgronicaCoreContabDAL.Ricette_Destinazioni_R

        Dim r_W As New AgronicaCoreContabDAL.Ricette_W
        Dim ro_W As New AgronicaCoreContabDAL.Ricette_Operazioni_W
        Dim rdet_W As New AgronicaCoreContabDAL.Ricette_Dettagli_W
        Dim rtec_W As New AgronicaCoreContabDAL.Ricette_Dett_Tecnico_W
        Dim rdest_W As New AgronicaCoreContabDAL.Ricette_Destinazioni_W

        Dim listaRicetteApp As List(Of APP_Ricette)
        Dim listaOperazioniApp As List(Of APP_Ricette_Operazioni)
        Dim listaDettaglioApp As List(Of APP_Ricette_Dettagli)
        Dim listaTecnicoApp As List(Of APP_Ricette_Dettaglio_Tecnico)
        Dim listaDestinazioneApp As List(Of APP_Ricette_Destinazioni)

        Dim seq As New Agro_Sequenze()
        Dim dic_Ricetta_Cod As New Dictionary(Of String, Integer)
        Dim dic_Ricetta_Operazione_Cod As New Dictionary(Of String, Integer)
        Dim dic_Ricetta_Dettaglio_Cod As New Dictionary(Of String, Integer)
        Dim dic_Ricetta_Tecnico_Cod As New Dictionary(Of String, Integer)
        Dim dic_Ricetta_Destinazione_Cod As New Dictionary(Of String, Integer)

        Dim ScriviRicetteAPP As New AgronicaCoreContabDAL.APP_Ricette_Operazioni_W

        Dim ricetteOperazioniImportate As Integer = 0
        Dim ricetteOperazioniDaImportare As Integer = 0
        Dim ricetteImportate As Integer = 0

        Dim msgErr As String = ""
        Dim esitoFinale As Boolean = False

        Try


            Dim gefutils As New Gias_EF_Utility
            Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
            Dim dal As Gias_DeveloperServer_Entities = Nothing

            'Leggo solo le verificate -> TODO
            ImportaRicetteDaTabelleAPP_LetturaOggetti(piva, listaRicetteApp, listaOperazioniApp, listaDettaglioApp, listaTecnicoApp, listaDestinazioneApp, efConnString, False, guid)

            ricetteOperazioniDaImportare = listaRicetteApp.Count()

            ' Aggiunta gestione raccoglitore per attivita miste app
            Dim Raccoglitore_Cod As Integer = 0
            If Not String.IsNullOrEmpty(guid) AndAlso ricetteOperazioniDaImportare > 1 Then
                'Raccoglitore_Cod = seq.Agronica_SequenzaTabelle_NuovoID("raccoglitore", objParametri_Server)
                'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
                Raccoglitore_Cod = seq.NuovoId_Tabella("raccoglitore", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri_Server)
            End If

            Dim ric_R As New AgronicaCoreContabBIZ.Ricette_R
            Dim ricOP_R As New AgronicaCoreContabBIZ.Ricette_Operazioni_R
            For Each dr As APP_Ricette In listaRicetteApp

                Dim FlagTransazioneLocale As Boolean = False
                Dim FlagConnessioneLocale As Boolean = False

                'Filtro le liste per la ricetta in oggetto
                Dim listaOperazioniApp_filtrata As List(Of APP_Ricette_Operazioni) = (From r In listaOperazioniApp Where r.Ricetta_Cod = dr.Ricetta_Cod AndAlso r.ID.Split("|")(0) = dr.ID.Split("|")(0) Select r).ToList()
                Dim listaDettaglioApp_filtrata As List(Of APP_Ricette_Dettagli) = (From r In listaDettaglioApp Where r.Ricetta_Cod = dr.Ricetta_Cod AndAlso r.ID.Split("|")(0) = dr.ID.Split("|")(0) Select r).ToList()
                Dim listaTecnicoApp_filtrata As List(Of APP_Ricette_Dettaglio_Tecnico) = (From r In listaTecnicoApp Where r.Ricetta_Cod = dr.Ricetta_Cod AndAlso r.ID.Split("|")(0) = dr.ID.Split("|")(0) Select r).ToList()
                Dim listaDestinazioneApp_filtrata As List(Of APP_Ricette_Destinazioni) = (From r In listaDestinazioneApp Where r.Ricetta_Cod = dr.Ricetta_Cod AndAlso r.ID.Split("|")(0) = dr.ID.Split("|")(0) Select r).ToList()

                Dim Ricetta_Operazione_Lav_Cod As Integer = 0

                'TODO: Gestire la tranzazione
                Try

                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    'Apro la connessione al DB
                    AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                            FlagTransazioneLocale,
                                                                            objParametri_Server)

                    verificaCorrettezzaFormale_PerImportazione(dr, listaOperazioniApp_filtrata, listaDettaglioApp_filtrata, listaTecnicoApp_filtrata, listaDestinazioneApp_filtrata, objParametri_Server)

                    '########### Ricette ########### 
                    Dim Ricetta_Cod As Integer
                    If dr.Ricetta_Cod > 0 Then
                        Ricetta_Cod = dr.Ricetta_Cod
                    Else
                        Dim chiave_APP As String = dr.ID.Split("|")(0) & "|" & dr.Ricetta_Cod
                        If dic_Ricetta_Cod.ContainsKey(chiave_APP) Then
                            Ricetta_Cod = dic_Ricetta_Cod(chiave_APP)
                        Else
                            Ricetta_Cod = seq.NuovoId_Tabella("ricette", 0, 2000000000, objParametri_Server)
                            dic_Ricetta_Cod.Add(chiave_APP, Ricetta_Cod)
                        End If
                    End If

                    'Programmazione_Cod -> 0
                    'Imputazione_Cod -> 0
                    'Imputazione_Fase_Cod -> 0

                    Dim origine = If(Not String.IsNullOrEmpty(dr.Origine), dr.Origine, enum_OrigineApp.GiasApp)

                    Dim esito As Boolean = r_W.Scrivi(Ricetta_Cod, dr.Ricetta_Numero, dr.Piva, dr.Sa_Cod,
                                                           dr.Tipo_Ricetta, dr.Ricetta_Des, dr.Ricetta_Des, dr.Veg_Cod, dr.Note,
                                                           0, dr.Validita_Inizio, dr.Validita_Fine, 0, 0,
                                                           objParametri_Server, dr.Data_Creazione, dr.Data_Modifica, dr.Username_Creazione, dr.Username_Modifica, Origine:=origine)
                    If esito = False Then
                        Throw New Exception("Errore durante il salvataggio della Ricetta (RIC: " & dr.ID & ")")
                    End If

                    Dim listaDatiOperazioni As New List(Of IDictionary(Of String, Object))
                    '########### Ricette Operazioni ########### 
                    For Each elem As APP_Ricette_Operazioni In listaOperazioniApp_filtrata

                        Dim Ricetta_Operazione_Cod As Integer
                        Ricetta_Operazione_Lav_Cod = elem.Lav_Cod
                        If elem.Ricetta_Operazione_Cod > 0 Then
                            Ricetta_Operazione_Cod = elem.Ricetta_Operazione_Cod
                        Else
                            Dim chiave_APP As String = elem.ID.Split("|")(0) & "|" & elem.Ricetta_Operazione_Cod
                            If dic_Ricetta_Operazione_Cod.ContainsKey(chiave_APP) Then
                                Ricetta_Operazione_Cod = dic_Ricetta_Operazione_Cod(chiave_APP)
                            Else
                                Ricetta_Operazione_Cod = seq.NuovoId_Tabella("ricette_operazioni", 0, 2000000000, objParametri_Server)
                                dic_Ricetta_Operazione_Cod.Add(chiave_APP, Ricetta_Operazione_Cod)
                            End If
                        End If

                        'Num_Protocollo -> -1
                        'Id_Rcdpi -> 0
                        'Gru_Op -> 0
                        'Costo -> 0
                        'Noleggio_Passivo -> 0
                        'Id_Tp_Fer -> 0
                        'EM_Cod -> 0
                        'Eff_Perc -> 0
                        'Disciplinare_PubblicoPrivato -> 0

                        esito = ro_W.Scrivi(Ricetta_Cod, Ricetta_Operazione_Cod, elem.Lav_Cod, elem.Ricetta_Operazione_Des, elem.Note,
                                                -1, 0, elem.Extra_Int, elem.Mezzo, 0,
                                                0, 0, 0, 0, 0,
                                                0, elem.Validita_Inizio, elem.Validita_Fine,
                                                objParametri_Server, elem.Data_Creazione, elem.Data_Modifica, elem.Username_Creazione, elem.Username_Modifica,
                                                elem.W_Anagrafica_Stati_Cod, elem.Ricetta_Operazione_Cod_RIF, elem.ID, elem.Invia_App, Raccoglitore_Cod:=Raccoglitore_Cod, Ora:=elem.Validita_Inizio)

                        'Dim xmlRicettaOp = ricOP_R.Ricetta_Operazioni_Leggi(Ricetta_Cod, Ricetta_Operazione_Cod, elem.Lav_Cod, 0, elem.Validita_Inizio, elem.Validita_Fine, False, objParametri_Server)

                        listaDatiOperazioni.Add(New Dictionary(Of String, Object) From {{"Ricetta_Operazione_Cod", CInt(Ricetta_Operazione_Cod)}, {"Validita_Inizio", elem.Validita_Inizio}, {"Validita_Fine", elem.Validita_Fine},
                                                 {"Ricetta_Operazione_Des", elem.Ricetta_Operazione_Des}, {"Lav_Cod", elem.Lav_Cod}})


                        If esito = False Then
                            Throw New Exception("Errore durante il salvataggio dell' operazione della Ricetta (OP: " & elem.ID & ")")
                        End If

                        If Not String.IsNullOrEmpty(guid) Then
                            riferimento = Ricetta_Operazione_Cod
                        End If

                    Next

                    '########### Ricette Dettagli ########### 
                    For Each elem As APP_Ricette_Dettagli In listaDettaglioApp_filtrata

                        Dim Ricetta_Dettaglio_Cod As Integer
                        If elem.Ricetta_Dettaglio_Cod > 0 Then
                            Ricetta_Dettaglio_Cod = elem.Ricetta_Dettaglio_Cod
                        Else
                            Dim chiave_APP As String = elem.ID.Split("|")(0) & "|" & elem.Ricetta_Dettaglio_Cod
                            If dic_Ricetta_Dettaglio_Cod.ContainsKey(chiave_APP) Then
                                Ricetta_Dettaglio_Cod = dic_Ricetta_Dettaglio_Cod(chiave_APP)
                            Else
                                Ricetta_Dettaglio_Cod = seq.NuovoId_Tabella("ricette_dettagli", 0, 2000000000, objParametri_Server)
                                dic_Ricetta_Dettaglio_Cod.Add(chiave_APP, Ricetta_Dettaglio_Cod)
                            End If
                        End If

                        Dim Ricetta_Operazione_Cod As Integer = If(elem.Ricetta_Operazione_Cod > 0, elem.Ricetta_Operazione_Cod, dic_Ricetta_Operazione_Cod(elem.ID.Split("|")(0) & "|" & elem.Ricetta_Operazione_Cod))

                        'Miscela_Cod -> 1
                        'Prezzo_Unitario -> 0
                        'Qualifica_Cod -> 0
                        'Tariffa_Cod -> 0
                        'TempoCarenza -> 0
                        'DoseEtichetta -> “”
                        'PrincipiAttivi -> “”
                        'ClassiTossicologiche -> “”
                        'DoseEtichetta_Value -> “”
                        'Turno_Cod -> 0
                        'ID_Attivita -> 0
                        'PrincipiAttiviPesi -> “”
                        esito = rdet_W.Scrivi(Ricetta_Cod, Ricetta_Operazione_Cod, Ricetta_Dettaglio_Cod, 1, elem.Elem_Cod, elem.Pro_Cod,
                                                   elem.Mat_Cod, elem.Udm_Cod, elem.Qta, elem.Extra_Int, 0,
                                                   elem.Cau_Mov, 0, 0, elem.Validita_Inizio, elem.Validita_Fine,
                                                   objParametri_Server, elem.Data_Creazione, elem.Data_Modifica, elem.Username_Creazione, elem.Username_Modifica,
                                                   0, "", "", "", "",
                                                   0, 0, If(IsNothing(elem.Lotto), 0, elem.Lotto), elem.Qta_Extra, elem.Qta_Extra_Totale,
                                                   elem.Udm_Cod_Extra, elem.Mezzo_Det, "")

                        If esito = False Then
                            Throw New Exception("Errore durante il salvataggio dell' operazione della Ricetta (DET: " & elem.ID & ")")
                        End If

                    Next

                    '########### Ricette Dettaglio Tecnico ########### 
                    For Each elem As APP_Ricette_Dettaglio_Tecnico In listaTecnicoApp_filtrata

                        Dim Ricetta_Tecnico_Cod As Integer
                        If elem.Ricetta_Tecnico_Cod > 0 Then
                            Ricetta_Tecnico_Cod = elem.Ricetta_Tecnico_Cod
                        Else
                            Dim chiave_APP As String = elem.ID.Split("|")(0) & "|" & elem.Ricetta_Tecnico_Cod
                            If dic_Ricetta_Tecnico_Cod.ContainsKey(chiave_APP) Then
                                Ricetta_Tecnico_Cod = dic_Ricetta_Tecnico_Cod(chiave_APP)
                            Else
                                Ricetta_Tecnico_Cod = seq.NuovoId_Tabella("ricette_dettaglio_tecnico", 0, 2000000000, objParametri_Server)
                                dic_Ricetta_Tecnico_Cod.Add(chiave_APP, Ricetta_Tecnico_Cod)
                            End If
                        End If

                        Dim Ricetta_Operazione_Cod As Integer = If(elem.Ricetta_Operazione_Cod > 0, elem.Ricetta_Operazione_Cod, dic_Ricetta_Operazione_Cod(elem.ID.Split("|")(0) & "|" & elem.Ricetta_Operazione_Cod))
                        Dim Ricetta_Dettaglio_Cod As Integer = If(elem.Ricetta_Dettaglio_Cod >= 0, elem.Ricetta_Dettaglio_Cod, dic_Ricetta_Dettaglio_Cod(elem.ID.Split("|")(0) & "|" & elem.Ricetta_Dettaglio_Cod))

                        'Miscela_Cod -> 1 se figlio di ricetta_dettaglio; 0 altrimenti -> 0
                        'Inn3_Data -> ‘01/01/1900’
                        'Inn4_Data -> ‘01/01/1900’
                        'Ditta_Cod -> 0
                        'Sigla_AV -> 0
                        'Trap_Num -> 0
                        'Id_Insetto -> 0                        
                        'Mg -> 0
                        'ApportoxHa -> 0
                        'NnettoxHa -> 0
                        'NutilexHa -> 0
                        'Soglia_Cod -> 0
                        'Soglia_Des -> “”
                        'Soglia_Quantita -> 0
                        'Efficienza -> 1
                        'Ricette_Dettaglio_Tecnico_graphickey -> “”

                        ' fix per irrigazione: efficienza 100% (da aggiungere in APP_Ricette_Dettaglio_Tecnico)
                        Dim Efficienza As Integer = If(Ricetta_Operazione_Lav_Cod = LAVCOD_IRRIGAZIONE, 100, 1)
                        Dim Miscela_Cod As Integer = If(Ricetta_Operazione_Lav_Cod = LAVCOD_IRRIGAZIONE, 1, 0)

                        esito = rtec_W.Scrivi(Ricetta_Cod, Ricetta_Operazione_Cod, Ricetta_Dettaglio_Cod, Miscela_Cod, Ricetta_Tecnico_Cod, elem.Qta_Ril, elem.Av_Cod,
                                                   elem.Av_Gru, elem.Dett_Cod, elem.Dose, elem.Parziale, elem.Nitrati, elem.Freatimetro,
                                                   elem.Inn1_Data, elem.Inn2_Data, CostantiPersonalizzate.AGRODATAINIZIO, CostantiPersonalizzate.AGRODATAINIZIO, 0, 0, 0,
                                                   0, If(IsNothing(elem.FF_Classe), 0, elem.FF_Classe), 0, elem.N, If(IsNothing(elem.P), 0, elem.P), If(IsNothing(elem.K), 0, elem.K),
                                                   0, 0, 0, 0, "", 0,
                                                   Efficienza, "", elem.Piezo1, elem.piezo2, elem.piezo3, elem.piezo4,
                                                   elem.CU, elem.Validita_Inizio, elem.Validita_Fine,
                                                   objParametri_Server, elem.Data_Creazione, elem.Data_Modifica, elem.Username_Creazione, elem.Username_Modifica)

                        If esito = False Then
                            Throw New Exception("Errore durante il salvataggio dell' operazione della Ricetta (DETTEC: " & elem.ID & ")")
                        End If

                    Next

                    '########### Ricette Destinazioni ########### 
                    For Each elem As APP_Ricette_Destinazioni In listaDestinazioneApp_filtrata

                        Dim Ricetta_Destinazione_Cod As Integer
                        If elem.Ricetta_Destinazione_Cod > 0 Then
                            Ricetta_Destinazione_Cod = elem.Ricetta_Destinazione_Cod
                        Else
                            Dim chiave_APP As String = elem.ID.Split("|")(0) & "|" & elem.Ricetta_Destinazione_Cod
                            If dic_Ricetta_Destinazione_Cod.ContainsKey(chiave_APP) Then
                                Ricetta_Destinazione_Cod = dic_Ricetta_Cod(chiave_APP)
                            Else
                                Ricetta_Destinazione_Cod = seq.NuovoId_Tabella("ricette_destinazioni", 0, 2000000000, objParametri_Server)
                                dic_Ricetta_Destinazione_Cod.Add(chiave_APP, Ricetta_Destinazione_Cod)
                            End If
                        End If

                        Dim Ricetta_Operazione_Cod As Integer = If(elem.Ricetta_Operazione_Cod > 0, elem.Ricetta_Operazione_Cod, dic_Ricetta_Operazione_Cod(elem.ID.Split("|")(0) & "|" & elem.Ricetta_Operazione_Cod))
                        Dim Ricetta_Dettaglio_Cod As Integer = If(elem.Ricetta_Dettaglio_Cod > 0, elem.Ricetta_Dettaglio_Cod, dic_Ricetta_Dettaglio_Cod(elem.ID.Split("|")(0) & "|" & elem.Ricetta_Dettaglio_Cod))

                        'Programmazione_Entita_Cod -> 0
                        esito = rdest_W.Scrivi(Ricetta_Cod, Ricetta_Operazione_Cod, Ricetta_Dettaglio_Cod, Ricetta_Destinazione_Cod, 0,
                                                   elem.Piva, elem.Sa_Cod, elem.Appezza, elem.Id_Reg, elem.Qta, elem.Qta2,
                                                   elem.QuotaDistribuzione, elem.Tipo_Destinazione, elem.Validita_Inizio, elem.Validita_Fine,
                                                   objParametri_Server, elem.Data_Creazione, elem.Data_Modifica, elem.Username_Creazione, elem.Username_Modifica,
                                                   elem.MagazzinoEsterno_Cod, elem.MagazzinoEsterno_Des, elem.MagazzinoEsterno_Dettagli)

                        If esito = False Then
                            Throw New Exception("Errore durante il salvataggio dell' operazione della Ricetta (DEST: " & elem.ID & ")")
                        End If

                    Next

                    '########### Ricette Note ########### 
                    'TO DO


                    ''Segno come processati i record o la ricetta_operazione
                    'For Each elem As APP_Ricette_Operazioni In listaOperazioniApp_filtrata
                    '    dal.APP_Ricette_Operazioni.Attach(elem)
                    '    elem.Importato_Data = DateTime.Now
                    '    elem.MarkAsModified()
                    'Next

                    For Each elem As APP_Ricette_Operazioni In listaOperazioniApp_filtrata
                        ScriviRicetteAPP.APP_MarcaOperazioneImportata(elem.ID, "", objParametri_Server)
                    Next


                    Dim StringaXmlRicetta As String = ric_R.Ricetta_Leggi(Ricetta_Cod, dr.Piva, dr.Sa_Cod, dr.Tipo_Ricetta, dr.Veg_Cod, False, objParametri_Server)

                    Dim ObjRicetteLog As New AgronicaCoreContabDAL.AgronicaLogRicette_W
                    Dim Dummy As Boolean = ObjRicetteLog.Scrivi(enum_TipoOperazioneDB.Scrittura,
                                                         "Ricette",
                                                         CInt(Ricetta_Cod),
                                                         dr.Piva, dr.Sa_Cod,
                                                         dr.Tipo_Ricetta,
                                                         0,
                                                         Nothing,
                                                         dr.Ricetta_Des,
                                                         enum_Id_Servizio.GiasOnline,
                                                         objParametri_Server,
                                                         StringaXmlRicetta)

                    listaDatiOperazioni.ForEach(Sub(dati)
                                                    Dim Ricetta_Operazione_Cod = CInt(dati("Ricetta_Operazione_Cod"))
                                                    Dim elemLavCod = dati("Lav_Cod")
                                                    Dim validitaInizio = dati("Validita_Inizio")
                                                    Dim Validita_Fine = dati("Validita_Fine")
                                                    Dim Ricetta_Operazione_Des = dati("Ricetta_Operazione_Des")

                                                    Dim xmlRicettaOp = ricOP_R.Ricetta_Operazioni_Leggi(Ricetta_Cod, Ricetta_Operazione_Cod, elemLavCod, 0, validitaInizio, Validita_Fine, False, objParametri_Server)

                                                    Dim ricettaRead As New AgronicaCoreContabDAL.Ricette_Operazioni_R
                                                    Dim guidRicetta = ricettaRead.LeggiGuidRicetta(Ricetta_Cod, Ricetta_Operazione_Cod, objParametri_Server)

                                                    Dummy = ObjRicetteLog.Scrivi(enum_TipoOperazioneDB.Scrittura,
                                                         "Ricette_Operazioni",
                                                         CInt(Ricetta_Cod),
                                                         Ricetta_Operazione_Cod,
                                                         dr.Piva, dr.Sa_Cod,
                                                        elemLavCod,
                                                        validitaInizio,
                                                        Ricetta_Operazione_Des,
                                                         enum_Id_Servizio.GiasOnline,
                                                         objParametri_Server,
                                                        xmlRicettaOp,
                                                        guidRicetta:=guidRicetta)
                                                End Sub)
                    'Se è andato tutto bene                    
                    ricetteOperazioniImportate += listaOperazioniApp_filtrata.Count
                    ricetteImportate += 1

                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    'Chiudo la connessione al DB
                    AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)
                    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

                Catch ex As Exception

                    'Scrivo che non sono state importate
                    For Each elem As APP_Ricette_Operazioni In listaOperazioniApp_filtrata
                        ScriviRicetteAPP.APP_MarcaOperazioneImportata(elem.ID, ex.Message, objParametri_Server)
                    Next

                    'msgErr &= AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)
                    msgErr &= ex.Message & "<br>"

                    'Faccio il rollback della transazione
                    If Not objParametri_Server.objTransazione Is Nothing Then
                        'objParametri.objTransazione.Rollback()
                        'objParametri.objTransazione = Nothing
                        AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)

                    End If

                Finally

                    AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

                End Try

            Next

            If String.IsNullOrEmpty(guid) Then
                esitoFinale = True
                msgFinale = "Sono state importate correttamente " & ricetteOperazioniImportate & " operazioni su " & ricetteOperazioniDaImportare
                If Not String.IsNullOrEmpty(msgErr) Then
                    msgFinale &= "<br><br>ERRORI:<br>" & msgErr
                End If
            Else
                esitoFinale = ricetteOperazioniImportate = ricetteOperazioniDaImportare
                If Not String.IsNullOrEmpty(msgErr) Then
                    msgFinale = msgErr
                End If
            End If

        Catch ex As Exception

            esitoFinale = False
            msgFinale = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return esitoFinale
    End Function

    Private Shared Sub ImportaRicetteDaTabelleAPP_LetturaOggetti(piva As String,
                                                                 ByRef listaRicetteApp As List(Of APP_Ricette),
                                                                 ByRef listaOperazioniApp As List(Of APP_Ricette_Operazioni),
                                                                 ByRef listaDettaglioApp As List(Of APP_Ricette_Dettagli),
                                                                 ByRef listaTecnicoApp As List(Of APP_Ricette_Dettaglio_Tecnico),
                                                                 ByRef listaDestinazioneApp As List(Of APP_Ricette_Destinazioni),
                                                                 efConnString As String,
                                                                 LeggiPerImportazioneSuAgenda As Boolean,
                                                                 Optional guid As String = "")


        ' VAnni: 20/4/2021: i rilievi vengono importati direttamente sulle tabelle di agenda
        Dim listaOperazioniDaImportareSuAgenda As New List(Of Integer)

        listaOperazioniDaImportareSuAgenda.Add(LAVCOD_RILIEVO_AVVERSITA_CAMPO)
        listaOperazioniDaImportareSuAgenda.Add(LAVCOD_RILIEVO_INDICI_MATURITA)
        listaOperazioniDaImportareSuAgenda.Add(LAVCOD_FASI_FENOLOGICHE)
        'aggiungere altre quando rilasciate su APP

        Using giasContext As New Gias_DeveloperServer_Entities(efConnString)

            'Ricette ...
            Dim qRicetteAPP = (From r In giasContext.APP_Ricette
                               Join ro In giasContext.APP_Ricette_Operazioni
                                   On r.Ricetta_Cod Equals ro.Ricetta_Cod _
                                   And r.ID.Substring(0, r.ID.IndexOf("|")) Equals ro.ID.Substring(0, ro.ID.IndexOf("|"))
                               Where ro.Importato_Data Is Nothing AndAlso (piva = "" OrElse r.Piva = piva) AndAlso (guid = "" OrElse r.ID.Contains(guid))
                               Select r, ro)

            If LeggiPerImportazioneSuAgenda Then
                qRicetteAPP = qRicetteAPP.Where(Function(Q) listaOperazioniDaImportareSuAgenda.Contains(Q.ro.Lav_Cod))
            Else
                qRicetteAPP = qRicetteAPP.Where(Function(Q) Not listaOperazioniDaImportareSuAgenda.Contains(Q.ro.Lav_Cod))
            End If

            listaRicetteApp = qRicetteAPP.Select(Function(Q) Q.r).Distinct().ToList()


            'Ricette Operazioni ...
            Dim qOperazioniApp = (From r In giasContext.APP_Ricette_Operazioni
                                  Where r.Importato_Data Is Nothing
                                  Select r)

            If LeggiPerImportazioneSuAgenda Then
                qOperazioniApp = qOperazioniApp.Where(Function(Q) listaOperazioniDaImportareSuAgenda.Contains(Q.Lav_Cod))
            Else
                qOperazioniApp = qOperazioniApp.Where(Function(Q) Not listaOperazioniDaImportareSuAgenda.Contains(Q.Lav_Cod))
            End If

            listaOperazioniApp = qOperazioniApp.ToList()


            'Ricette dettagli ...
            Dim qDettaglioApp = (From r In giasContext.APP_Ricette_Dettagli
                                 Join ro In giasContext.APP_Ricette_Operazioni
                                     On r.Ricetta_Cod Equals ro.Ricetta_Cod _
                                     And r.Ricetta_Operazione_Cod Equals ro.Ricetta_Operazione_Cod _
                                     And r.ID.Substring(0, r.ID.IndexOf("|")) Equals ro.ID.Substring(0, ro.ID.IndexOf("|"))
                                 Where ro.Importato_Data Is Nothing
                                 Select r, ro)

            If LeggiPerImportazioneSuAgenda Then
                qDettaglioApp = qDettaglioApp.Where(Function(Q) listaOperazioniDaImportareSuAgenda.Contains(Q.ro.Lav_Cod))
            Else
                qDettaglioApp = qDettaglioApp.Where(Function(Q) Not listaOperazioniDaImportareSuAgenda.Contains(Q.ro.Lav_Cod))
            End If

            listaDettaglioApp = qDettaglioApp.Select(Function(Q) Q.r).Distinct().ToList()


            'Ricetta dettaglio tecnico ...
            Dim qTecnicoApp = (From r In giasContext.APP_Ricette_Dettaglio_Tecnico
                               Join ro In giasContext.APP_Ricette_Operazioni
                               On r.Ricetta_Cod Equals ro.Ricetta_Cod _
                                   And r.Ricetta_Operazione_Cod Equals ro.Ricetta_Operazione_Cod _
                                   And r.ID.Substring(0, r.ID.IndexOf("|")) Equals ro.ID.Substring(0, ro.ID.IndexOf("|"))
                               Where ro.Importato_Data Is Nothing
                               Select r, ro)

            If LeggiPerImportazioneSuAgenda Then
                qTecnicoApp = qTecnicoApp.Where(Function(Q) listaOperazioniDaImportareSuAgenda.Contains(Q.ro.Lav_Cod))
            Else
                qTecnicoApp = qTecnicoApp.Where(Function(Q) Not listaOperazioniDaImportareSuAgenda.Contains(Q.ro.Lav_Cod))
            End If


            listaTecnicoApp = qTecnicoApp.Select(Function(Q) Q.r).Distinct().ToList()

            'Ricetta Destinazioni ... 
            Dim qDestinazioneApp = (From r In giasContext.APP_Ricette_Destinazioni
                                    Join ro In giasContext.APP_Ricette_Operazioni
                                        On r.Ricetta_Cod Equals ro.Ricetta_Cod _
                                        And r.Ricetta_Operazione_Cod Equals ro.Ricetta_Operazione_Cod _
                                        And r.ID.Substring(0, r.ID.IndexOf("|")) Equals ro.ID.Substring(0, ro.ID.IndexOf("|"))
                                    Where ro.Importato_Data Is Nothing
                                    Select r, ro)

            If LeggiPerImportazioneSuAgenda Then
                qDestinazioneApp = qDestinazioneApp.Where(Function(Q) listaOperazioniDaImportareSuAgenda.Contains(Q.ro.Lav_Cod))
            Else
                qDestinazioneApp = qDestinazioneApp.Where(Function(Q) Not listaOperazioniDaImportareSuAgenda.Contains(Q.ro.Lav_Cod))
            End If

            listaDestinazioneApp = qDestinazioneApp.Select(Function(Q) Q.r).Distinct().ToList()


        End Using


    End Sub

    Private Sub verificaCorrettezzaFormale_PerImportazione(ByRef ricettaApp As APP_Ricette,
                                                           ByRef listaOperazioniApp As List(Of APP_Ricette_Operazioni),
                                                           ByRef listaDettaglioApp As List(Of APP_Ricette_Dettagli),
                                                           ByRef listaTecnicoApp As List(Of APP_Ricette_Dettaglio_Tecnico),
                                                           ByRef listaDestinazioneApp As List(Of APP_Ricette_Destinazioni),
                                                           ByRef objParametri_Server As AgronicaCoreParametri
                                                           )

        'TODO: VERIFICO SE LA RICETTA PADRE ESISTE ANCORA E NON E' STATA MODIFICATA? Per ora no (Valerio 11/10/2018)

        Dim az_r As New AgronicaCoreAnagrafeDAL.Imprese_Read
        Dim ca_r As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
        Dim fabb_r As New AgronicaCoreAnagrafeDAL.Fabbricati_R
        Dim app_r As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
        Dim imp_r As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim dist_r As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R

        Dim dt_Imprese As DataTable = az_r.EsisteRecordInTabellaImprese(ricettaApp.Piva, objParametri_Server)

        'verifico se l'impresa esiste
        If IsNothing(dt_Imprese) OrElse dt_Imprese.Rows.Count = 0 Then
            Throw New Exception("[RIC: " & ricettaApp.ID & "] Non è stata trovata nessuna Impresa con P.Iva '" & ricettaApp.Piva & "'")
        End If

        Dim impresa_validita_inizio As DateTime = dt_Imprese.Rows(0).Item("validita_inizio")
        Dim impresa_validita_fine As DateTime = dt_Imprese.Rows(0).Item("validita_fine")
        Dim rag_soc As String = dt_Imprese.Rows(0).Item("rag_soc")

        For Each elem As APP_Ricette_Operazioni In listaOperazioniApp

            'Filtro le liste per l'operazione in oggetto
            Dim listaDettaglioApp_filtrata As List(Of APP_Ricette_Dettagli) = (From r In listaDettaglioApp Where r.Ricetta_Operazione_Cod = elem.Ricetta_Operazione_Cod Select r).ToList()
            Dim listaTecnicoApp_filtrata As List(Of APP_Ricette_Dettaglio_Tecnico) = (From r In listaTecnicoApp Where r.Ricetta_Operazione_Cod = elem.Ricetta_Operazione_Cod Select r).ToList()
            Dim listaDestinazioneApp_filtrata As List(Of APP_Ricette_Destinazioni) = (From r In listaDestinazioneApp Where r.Ricetta_Operazione_Cod = elem.Ricetta_Operazione_Cod Select r).ToList()

            Dim dataOperazione As Date = elem.Validita_Inizio
            dataOperazione = dataOperazione.Date

            'verifico se l'impresa è attiva nella data dell'operazione
            If dataOperazione < impresa_validita_inizio OrElse dataOperazione > impresa_validita_fine Then
                Throw New Exception("[OP: " & elem.ID & "] L'Impresa '" & rag_soc & "' non risulta attiva alla data dell'operazione (data operazione = " & dataOperazione & " validità azienda = " & impresa_validita_inizio & " - " & impresa_validita_fine & ")")
            End If

            '######### CENTRO AZIENDALE #########
            Dim caGiaVerificati As New List(Of String)

            For Each ca As APP_Ricette_Destinazioni In listaDestinazioneApp_filtrata

                'Se ho già controllato, salto al successivo
                If caGiaVerificati.Contains(String.Join("_", ca.Piva, ca.Sa_Cod)) Then
                    Continue For
                End If

                Dim dt_centro As DataTable = ca_r.Leggi(ca.Piva, ca.Sa_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)

                'verifico se il centro esiste
                If IsNothing(dt_centro) OrElse dt_centro.Rows.Count = 0 Then
                    Throw New Exception("[DEST: " & ca.ID & "] Non è stato trovato nessun centro '" & String.Join("_", ca.Piva, ca.Sa_Cod) & "' per l'Impresa '" & rag_soc & "'")
                End If

                'verifico se il centro è attivo nella data dell'operazione
                Dim centro_validita_inizio As DateTime = dt_centro.Rows(0).Item("validita_inizio")
                Dim centro_validita_fine As DateTime = dt_centro.Rows(0).Item("validita_fine")
                If dataOperazione < centro_validita_inizio OrElse dataOperazione > centro_validita_fine Then
                    Dim rag_soc_2 As String = az_r.RagSoc_from_Piva(ca.Piva, objParametri_Server)
                    Throw New Exception("[DEST: " & ca.ID & "] Il centro '" & dt_centro.Rows(0).Item("sa_nome") & "' dell'impresa '" & rag_soc_2 & "' non risulta attivo alla data dell'operazione (data operazione = " & dataOperazione & " validità centro = " & centro_validita_inizio & " - " & centro_validita_fine & ")")
                End If

                caGiaVerificati.Add(String.Join("_", ca.Piva, ca.Sa_Cod))

            Next

            '######### DISCIPLINARE/REGOLAMENTO OPERAZIONE #########
            'verifico se il disciplinare esiste
            'verifico se il disciplinare è attivo nella data dell'operazione

            '######### MAGAZZINO #########
            Dim listaDestinazioneApp_filtrataXMagazzini As List(Of APP_Ricette_Destinazioni) = (From r In listaDestinazioneApp Where r.Tipo_Destinazione = 20 Select r).ToList()
            Dim magGiaVerificati As New List(Of String)

            For Each mag As APP_Ricette_Destinazioni In listaDestinazioneApp_filtrataXMagazzini

                'Se ho già controllato, salto al successivo
                If magGiaVerificati.Contains(String.Join("_", mag.Piva, mag.Sa_Cod, mag.Id_Reg)) Then
                    Continue For
                End If

                Dim dt_fabbricati As DataTable = fabb_r.Leggi_3(mag.Piva, mag.Sa_Cod, mag.Id_Reg, "", "", objParametri_Server)

                'verifico se il magazzino esiste 
                If IsNothing(dt_fabbricati) OrElse dt_fabbricati.Rows.Count = 0 Then
                    Dim sa_nome As String = ca_r.SaNome_from_SaCod(mag.Piva, mag.Sa_Cod, objParametri_Server)
                    Throw New Exception("[DEST: " & mag.ID & "] Non è stato trovato nessun magazzino '" & String.Join("_", mag.Piva, mag.Sa_Cod, mag.Id_Reg) & "' nel centro '" & sa_nome & "' per l'impresa '" & rag_soc)
                End If

                'verifico se il magazzino è attivo nella data dell'operazione
                Dim fabbricato_validita_inizio As DateTime = dt_fabbricati.Rows(0).Item("validita_inizio")
                Dim fabbricato_validita_fine As DateTime = dt_fabbricati.Rows(0).Item("validita_fine")
                If dataOperazione < fabbricato_validita_inizio OrElse dataOperazione > fabbricato_validita_fine Then
                    Dim sa_nome As String = ca_r.SaNome_from_SaCod(mag.Piva, mag.Sa_Cod, objParametri_Server)
                    Dim fabbricato_des As String = dt_fabbricati.Rows(0).Item("fabbricato_des")
                    Throw New Exception("[DEST: " & mag.ID & "] Il magazzino '" & fabbricato_des & "' nel centro '" & sa_nome & "' dell'impresa '" & rag_soc & "' non risulta attivo alla data dell'operazione (data operazione = " & dataOperazione & " validità magazzino = " & fabbricato_validita_inizio & " - " & fabbricato_validita_fine & ")")
                End If

                magGiaVerificati.Add(String.Join("_", mag.Piva, mag.Sa_Cod, mag.Id_Reg))

            Next

            '######### IMPIANTI #########
            Dim listaDestinazioneApp_filtrataXImpianti As List(Of APP_Ricette_Destinazioni) = (From r In listaDestinazioneApp Where r.Tipo_Destinazione = 0 Select r).ToList()
            Dim impGiaVerificati As New List(Of String)

            For Each imp As APP_Ricette_Destinazioni In listaDestinazioneApp_filtrataXImpianti

                'Se ho già controllato, salto al successivo
                If impGiaVerificati.Contains(String.Join("_", imp.Piva, imp.Sa_Cod, imp.Appezza, imp.Id_Reg)) Then
                    Continue For
                End If

                Dim dt_imp As DataTable = imp_r.Leggi(imp.Piva, imp.Sa_Cod, imp.Appezza, imp.Id_Reg, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)

                'verifico se gli impianti esistono
                If IsNothing(dt_imp) OrElse dt_imp.Rows.Count = 0 Then
                    Dim sa_nome As String = ca_r.SaNome_from_SaCod(imp.Piva, imp.Sa_Cod, objParametri_Server)
                    Throw New Exception("[DEST: " & imp.ID & "] Non è stato trovato nessun impianto '" & String.Join("_", imp.Piva, imp.Sa_Cod, imp.Appezza, imp.Id_Reg) & "' nel centro '" & sa_nome & "' per l'impresa '" & rag_soc)
                End If

                'verifico se gli impianti sono attivi nella data dell'operazione
                Dim impianto_validita_inizio As DateTime = dt_imp.Rows(0).Item("validita_inizio")
                Dim impianto_validita_fine As DateTime = dt_imp.Rows(0).Item("validita_fine")
                If dataOperazione < impianto_validita_inizio OrElse dataOperazione > impianto_validita_fine Then
                    Dim sa_nome As String = ca_r.SaNome_from_SaCod(imp.Piva, imp.Sa_Cod, objParametri_Server)
                    Dim app_nome As String = app_r.AppezzamentoNome_from_Appezza(imp.Piva, imp.Sa_Cod, imp.Appezza, objParametri_Server)
                    Throw New Exception("[DEST: " & imp.ID & "] L'impianto '" & String.Join("_", imp.Piva, imp.Sa_Cod, imp.Appezza, imp.Id_Reg) & "' dell'appezzamento '" & app_nome & "' nel centro '" & sa_nome & "' dell'impresa '" & rag_soc & "' non risulta attivo alla data dell'operazione (data operazione = " & dataOperazione & " validità impianto = " & impianto_validita_inizio & " - " & impianto_validita_fine & ")")
                End If

                'controllo irrigazioni su terreni nudi
                If elem.Lav_Cod = LAVCOD_IRRIGAZIONE AndAlso dt_imp.Rows(0).Item("Cul_Cod") = 0 Then
                    Dim sa_nome As String = ca_r.SaNome_from_SaCod(imp.Piva, imp.Sa_Cod, objParametri_Server)
                    Dim app_nome As String = app_r.AppezzamentoNome_from_Appezza(imp.Piva, imp.Sa_Cod, imp.Appezza, objParametri_Server)
                    Throw New Exception("[DEST: " & imp.ID & "] L'operazione IRRIGAZIONE non è consentita sull'impianto di terreno nudo '" & String.Join("_", imp.Piva, imp.Sa_Cod, imp.Appezza, imp.Id_Reg) & "' dell'appezzamento '" & app_nome & "' nel centro '" & sa_nome & "' dell'impresa '" & rag_soc & "'")
                End If

                impGiaVerificati.Add(String.Join("_", imp.Piva, imp.Sa_Cod, imp.Appezza, imp.Id_Reg))

            Next

            '######### DISTINTE #########
            Dim progGiaVerificati As New List(Of String)

            'verifico se per ogni impianto esiste distinta attiva nella data dell'operazione
            For Each prog As APP_Ricette_Destinazioni In listaDestinazioneApp_filtrataXImpianti

                'Se ho già controllato, salto al successivo
                If progGiaVerificati.Contains(String.Join("_", prog.Piva, prog.Sa_Cod, prog.Appezza, prog.Id_Reg)) Then
                    Continue For
                End If

                Dim cod_prog As Integer = dist_r.CodProgetto_from_DataValidita(prog.Piva, prog.Sa_Cod, prog.Appezza, prog.Id_Reg, dataOperazione, objParametri_Server)

                If cod_prog = 0 Then
                    Dim sa_nome As String = ca_r.SaNome_from_SaCod(prog.Piva, prog.Sa_Cod, objParametri_Server)
                    Dim app_nome As String = app_r.AppezzamentoNome_from_Appezza(prog.Piva, prog.Sa_Cod, prog.Appezza, objParametri_Server)
                    Throw New Exception("[DEST: " & prog.ID & "] L'impianto '" & String.Join("_", prog.Piva, prog.Sa_Cod, prog.Appezza, prog.Id_Reg) & "' dell'appezzamento '" & app_nome & "' nel centro '" & sa_nome & "' dell'impresa '" & rag_soc & "' non ha distinta attiva alla data dell'operazione (data operazione = " & dataOperazione & ")")
                End If

                progGiaVerificati.Add(String.Join("_", prog.Piva, prog.Sa_Cod, prog.Appezza, prog.Id_Reg))

            Next

            '######### SPECIE - FINALITA' - COPERTURA #########
            'NO -> verifico se specie, varietà, finalità, copertura e superficie sono coerenti con l'attuale situazione
            'SI -> verifico se specie, finalità, copertura sono omogenei tra di loro (Valerio) -> Per ora solo Specie

            Dim vegGiaVerificati As New List(Of String)
            Dim listaVeg_Cod As New List(Of Integer)

            For Each veg As APP_Ricette_Destinazioni In listaDestinazioneApp_filtrataXImpianti

                'Se ho già controllato, salto al successivo
                If vegGiaVerificati.Contains(String.Join("_", veg.Piva, veg.Sa_Cod, veg.Appezza, veg.Id_Reg)) Then
                    Continue For
                End If

                Dim veg_cod As Integer = imp_r.VegCod_from_PivaSaCodAppezzaIdimp(veg.Piva, veg.Sa_Cod, veg.Appezza, veg.Id_Reg, "", "", objParametri_Server)

                If listaVeg_Cod.Count > 0 AndAlso Not listaVeg_Cod.Contains(veg_cod) Then
                    Dim sa_nome As String = ca_r.SaNome_from_SaCod(veg.Piva, veg.Sa_Cod, objParametri_Server)
                    Dim app_nome As String = app_r.AppezzamentoNome_from_Appezza(veg.Piva, veg.Sa_Cod, veg.Appezza, objParametri_Server)
                    Throw New Exception("[DEST: " & veg.ID & "] L'operazione contiene impianti con specie diverse tra di loro")
                End If

                listaVeg_Cod.Add(veg_cod)
                vegGiaVerificati.Add(String.Join("_", veg.Piva, veg.Sa_Cod, veg.Appezza, veg.Id_Reg))

            Next

            '######### PRODOTTI #########
            For Each det As APP_Ricette_Dettagli In listaDettaglioApp_filtrata
                'verifico se il prodotto esiste
                If Not String.IsNullOrEmpty(det.Descrizione) AndAlso det.Pro_Cod = 0 Then
                    Throw New Exception("Non è presente il prodotto con descrizione '" & det.Descrizione & "'")
                End If
            Next

            '######### DISCIPLINARE IMPIANTO #########
            'NO -> verifico se disciplinare dell'esercizio è coerente con l'attuale situazione -> No al più diventa arancio ma non è bloccante

            '######### MACCHINE #########
            'verifico se le macchine esistono 
            'verifico se le macchine sono attive nella data dell'operazione

            '######### OPERATORI #########
            'verifico se gli operatori esistono
            'verifico se gli operatori sono attivi nella data dell'operazione

            '######### NOTE #########
            'verifico se le note esistono
            'verifico se le note sono attive nella data dell'operazione

        Next

    End Sub

    Public Function InviaRicettaApp(ByVal listaRicette As List(Of AgronicaCoreDTOStd.InData.Agenda.APP_Ricette_Operazioni),
                                    ByVal value As Integer,
                                    ByVal objParametri_Server As AgronicaCoreParametri) As Boolean

        Dim objRicette As New AgronicaCoreContabDAL.Ricette_W
        Dim Piva = objParametri_Server.PivaSuperUser
        Dim success As Boolean = True

        Dim msgIncompatibiliList As New List(Of String)

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

        For Each ricetta In listaRicette
            success = objRicette.AggiornaRicetteInviataAPP(
            Piva, ricetta.Ricetta_Cod, value,
            "", objParametri_Server)

            If Not success Then
                ' Handle exception? Throw exception? One transaction for all the recipes?
            End If
        Next

        Return success
    End Function

    Public Function Update_Origine(Ricetta_Cod As Integer,
                                    Origine As String,
                                    objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabBIZ.Ricette_W.Update_Origine()"

        Dim xRisp As Boolean = False

        Dim objW As New AgronicaCoreContabDAL.Ricette_W
        xRisp = objW.Update_Origine(Ricetta_Cod, Origine, objParametri_Server)

        Return xRisp

    End Function

End Class