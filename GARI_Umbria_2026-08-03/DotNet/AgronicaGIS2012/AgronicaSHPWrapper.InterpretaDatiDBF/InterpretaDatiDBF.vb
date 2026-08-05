Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreModello.ParametriAgenda_Temp

Imports AgronicaCoreDataProvider.UtilityProvider

Imports System.Configuration.ConfigurationManager
Imports System.Xml
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports AgronicaCoreVarieDAL

Public Class InterpretaDatiDBF

    Public Function ElaboraRaccolte(ByVal ProgressivoGias As Integer, ByVal objParametri_Server As AgronicaCoreParametri) As RispostaStandard

        Dim EsitoFinale As New RispostaStandard
        EsitoFinale.RispostaOK = True
        EsitoFinale.RispostaStringa = "Assegnazione raccolte completata. " & vbCrLf & vbCrLf
        EsitoFinale.Errore = ""

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        '---- Seconda parte: Elaborazione automatica delle raccolte

        FlagTransazioneLocale = False
        FlagConnessioneLocale = False


        Try


            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Apro la connessione al DB e la transazione
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                FlagTransazioneLocale,
                                                                objParametri_Server)


            Dim DistanzaAttiva As Double
            Dim EliminaRaccolteNonBloccate As Boolean = False


            Dim LeggiCFGDatiIniziali As New Configurazione_Siti_R
            Dim dtLeggiCfgDatiIniziali As DataTable = _
                LeggiCFGDatiIniziali.Leggi( _
                    6, _
                    "iMotionCFG", _
                    "", _
                    "", _
                    objParametri_Server _
                    )


            If dtLeggiCfgDatiIniziali.Rows.Count > 0 Then

                Dim jSonCfg As String = dtLeggiCfgDatiIniziali.Rows(0)("Valore")

                Dim obV As JObject = JsonConvert.DeserializeObject(jSonCfg)

                DistanzaAttiva = obV("DistanzaAttiva")
                EliminaRaccolteNonBloccate = obV("EliminaRaccolteNonBloccate")
            Else
                DistanzaAttiva = 150
            End If


            Dim xPF As New AgronicaCoreGisDAL.PrecisionFarming
            If EliminaRaccolteNonBloccate Then
                xPF.EliminaRaccolteNonBloccate(objParametri_Server)
            End If



            Dim dtRaccolteProposte As DataTable
            dtRaccolteProposte = xPF.LeggiImpiantiPerVicinanzaPuntiStop( _
                objParametri_Server.PivaSuperUser, _
                66, _
                "19", _
                "from_time", _
                "to_time", _
                DistanzaAttiva, _
                0, _
                True, _
                "and charindex('isStop§ 1', ElementoGrafico_DES , 0)>1", _
                "", _
                objParametri_Server _
            )




            'scorro tutte le raccolte proposte per creare le op. di agenda e per assegnare i dati ai punti, così da considerarli raccolti.

            Dim ConteggioRaccolteAssegnate As Integer = 0

            For Each iRaccolto As DataRow In dtRaccolteProposte.Rows

                ConteggioRaccolteAssegnate += 1

                '-----
                '1. Assegno al punto GiS i dati dell'impianto.
                '-----
                xPF.AssegnaImpiantoSuEntitaEsistente( _
                    iRaccolto("Punto_Stop_Entita_Cod"), _
                    iRaccolto("Piva"), _
                    iRaccolto("sa_cod"), _
                    iRaccolto("appezza"), _
                    iRaccolto("id_imp"), _
                    "", _
                    "", _
                    objParametri_Server _
                )




                '-----
                '2. Genero l'operazione di raccolta
                '-----

                Dim ListaOpAgendaCollegate As List(Of Operazione_Agenda)
                Dim ListaImpianti As New List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)

                ListaImpianti.Add( _
                    New AgronicaCoreModello.ParametriAgenda_Temp.Impianto With { _
                        .Piva = iRaccolto("Piva"), _
                        .Sa_Cod = iRaccolto("sa_cod"), _
                        .Appezza = iRaccolto("appezza"), _
                        .ID_Reg = iRaccolto("id_imp"), _
                        .Cul_Cod = iRaccolto("cul_cod") _
                    }
                )


                Dim veg_cod As Integer = iRaccolto("veg_cod")



                'TODO: popolare oggetto parametri
                'Dim objParametriAgenda As New ParametriAgenda(True)

                'objParametriAgenda.Veg_Cod = veg_cod
                'objParametriAgenda.Cul_Cod = iRaccolto("cul_cod")
                'objParametriAgenda.Tipo_Operazione = 1
                'objParametriAgenda.Lav_Cod = 125                


                Dim messaggio_errore As String
                Dim Agenda As Operazione_Agenda = CreaOggettoAgenda(ProgressivoGias, iRaccolto("piva"), veg_cod, iRaccolto("DataORA_Inizio_Raccolta"), ListaImpianti, iRaccolto("Sa_Cod"), messaggio_errore, ListaOpAgendaCollegate)
                If IsNothing(Agenda) Then
                    Throw New Exception("Non È StatoPossibile Creare L'Operazione")
                End If


                Dim objAgendaScrivi As New Agenda_Operazione_Helper

                Dim Id_Agenda As Integer
                Id_Agenda = objAgendaScrivi.Scrivi(Agenda, objParametri_Server)





                '-----
                '3. Genero un elemento grafico legato all'operazione di raccolta (Partendo dall'elemento grafico dell'impianto).
                '-----
                xPF.CopiaIncollaOggettoGrafico( _
                    iRaccolto("Entita_cod"), _
                    Id_Agenda, _
                    "", _
                    "", _
                    objParametri_Server _
                )



            Next

            EsitoFinale.RispostaStringa &= vbCrLf & " Sono state elaborate " & ConteggioRaccolteAssegnate & " Raccolte. Distanza attiva: " & DistanzaAttiva.ToString & " Metri." & vbCrLf

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Chiudo la connessione al DB
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)


        Catch ex As Exception


            'Faccio il rollback della transazione
            If Not objParametri_Server.objTransazione Is Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)

            End If


            EsitoFinale.RispostaOK = False
            EsitoFinale.RispostaStringa &= "Importazione Elaborazione Raccolte Fallita."
            EsitoFinale.Errore &= AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True, source:=True)

        Finally

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

        End Try

        Return EsitoFinale

    End Function



    Private Function CreaOggettoAgenda(ByVal ProgressivoGias As Integer, ByVal Piva As String, ByVal Veg_cod As Integer, ByVal DataOraRaccolta As String, ByVal ListaImp As List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto), ByVal Sa_Cod As Integer, ByRef messaggio_errore As String, ByRef ListaOpAgendaCollegate As List(Of Operazione_Agenda)) As Operazione_Agenda

        Dim Agenda As Operazione_Agenda

        Dim Lotto As String = ""



        Dim Elem_Cod As Integer = TRASFORMATI_VEGETALI

        Dim Mat_Cod_Generato As Integer = 0

        '--------------AGENDA------------------
        If Not Crea_Agenda(ProgressivoGias, Piva, Veg_cod, DataOraRaccolta, ListaImp, Sa_Cod, Agenda, messaggio_errore, Lotto, Elem_Cod, Mat_Cod_Generato) Then
            Return Nothing
        End If

        '------------- MOVIMENTO RACCOLTA---------        
        If Not Crea_Agenda_Movimento_Raccolta(Agenda, ListaImp, messaggio_errore, Lotto, Elem_Cod, Mat_Cod_Generato) Then
            Return Nothing
        End If



        Return Agenda
    End Function



    Private Function Crea_Agenda(ByVal ProgressivoGias As Integer, ByVal piva As String, ByVal veg_cod As Integer, ByVal DataOraRaccolta As String, ByRef ListaImp As List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto), ByRef Sa_Cod As Integer, ByRef Agenda As Operazione_Agenda, ByRef messaggio_errore As String, ByRef Lotto As String, ByVal Elem_Cod As Integer, ByRef Mat_Cod_Ritorno As Integer) As Boolean

        '---------------------------------------
        ' recupero la NOTE
        'Dim strNota As String = CType(Master, Operazione).GetNota()




        '---------------------------------------
        ' recupero la DATA
        Dim Data As Date = DataOraRaccolta


        '---------------------------------------
        ' recupero la SPECIE
        Dim Veg_Des As String = ""



        '---------------------------------------
        ' recupero la OPERAZIONE
        'Dim Lav_Cod As String = ""
        Dim Lav_Des As String = ""
        Lav_Des = "Raccolta da importazione iMotion"



        Dim BaseCode As Integer
        Dim TopCode As Integer

        '------------------------------------------------
        '----- Calcolo i valori di BaseCode e TopCode
        '------------------------------------------------
        Calcola_BaseCode_TopCode(BaseCode, _
                              TopCode, _
                              ProgressivoGIAS _
        )



        '------------------------------------------------
        '----- AGENDA
        '------------------------------------------------


        Dim trovato As Boolean
        Dim ListaVarieta As New List(Of String)
        Dim ListaCul_Cod As New List(Of Integer)
        Dim StrVarieta As String = ""
        For i = 0 To ListaImp.Count - 1
            trovato = False
            For j = 0 To ListaVarieta.Count - 1
                If ListaVarieta(j) = ListaImp(i).Cul_Des Then
                    trovato = True
                    Exit For
                End If
            Next
            If trovato = False Then
                ListaVarieta.Add(ListaImp(i).Cul_Des)
                ListaCul_Cod.Add(ListaImp(i).Cul_Cod)
                StrVarieta += ", " + ListaImp(i).Cul_Des
            End If
        Next

        StrVarieta = StrVarieta.Substring(2, (StrVarieta.Length - 2))




        Agenda = New Operazione_Agenda

        Agenda.Tipo_Operazione = 1
        Agenda.Id_Agenda = 0
        Agenda.Data = Data
        Agenda.Piva = piva
        Agenda.Sa_Cod = Sa_Cod
        Agenda.Lav_Cod = 125

        Lotto = DataOraRaccolta



        Agenda.Des_Lib = Lav_Des & " (" & Veg_Des & "  [" + StrVarieta + "])"


        Agenda.BaseCode = BaseCode
        Agenda.TopCode = TopCode

        Return True
    End Function

    Private Function Crea_Agenda_Movimento_Raccolta(ByRef Agenda As Operazione_Agenda, ByRef ListaImp As List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto), ByRef messaggio_errore As String, ByVal Lotto As String, ByVal Elem_Cod As Integer, ByVal Mat_Cod As Integer) As Boolean

        'KG
        Dim Udm As Integer = CInt(2)

        Dim Movimento_OperazioneColturale As New Movimento

        Movimento_OperazioneColturale.Id_Agenda = Agenda.Id_Agenda
        Movimento_OperazioneColturale.Piva = Agenda.Piva
        Movimento_OperazioneColturale.Sa_Cod = Agenda.Sa_Cod
        Movimento_OperazioneColturale.Lav_Cod = 125
        Movimento_OperazioneColturale.Cau_Mov = "2200"
        Movimento_OperazioneColturale.Mov_Desc = ""
        Movimento_OperazioneColturale.Data = Agenda.Data
        Movimento_OperazioneColturale.BaseCode = Agenda.BaseCode
        Movimento_OperazioneColturale.TopCode = Agenda.TopCode

        Movimento_OperazioneColturale.Modalita = 1

        Movimento_OperazioneColturale.Extra_Int = 1 'salvo il tipo raccolta, puo essere utile

        '----------------------------------------------------------
        '----- MOVIMENTI DETTAGLI , DET.TECNICI, DESTINAZIONI -----
        '----------------------------------------------------------
        Dim Movimento_Dettaglio As New Movimento_Dettaglio
        Movimento_Dettaglio.Id_Agenda = Agenda.Id_Agenda
        Movimento_Dettaglio.Piva = Agenda.Piva
        Movimento_Dettaglio.Sa_Cod = Agenda.Sa_Cod

        Movimento_Dettaglio.Mov_det_des = "Dettagli Prodotti Aziendali Ottenuti da Raccolta"

        Movimento_Dettaglio.Elem_Cod = Elem_Cod
        Movimento_Dettaglio.Pro_Cod = 0
        Movimento_Dettaglio.Mat_Cod = Mat_Cod
        Movimento_Dettaglio.Udm_Cod = Udm
        'dopo  Movimento_Dettaglio.Qta = QtaTot
        Movimento_Dettaglio.Contabilizzato = NONCONTABILE
        Movimento_Dettaglio.Pendente = enum_Pendenza.MovESENTE
        Movimento_Dettaglio.Cal_Cod = 12 'Materie_Prime_Calibri cal_cod=12 indefinito
        Movimento_Dettaglio.Lotto = Lotto
        Movimento_Dettaglio.Data = Agenda.Data
        Movimento_Dettaglio.Anno = 1900
        Movimento_Dettaglio.BaseCode = Agenda.BaseCode
        Movimento_Dettaglio.TopCode = Agenda.TopCode

        Dim tot As Decimal = 0
        For i = 0 To ListaImp.Count - 1

            Dim Movimento_Destinazione As New Movimento_Destinazione

            Movimento_Destinazione.Id_Agenda = Agenda.Id_Agenda
            Movimento_Destinazione.Piva = ListaImp(i).Piva
            Movimento_Destinazione.Sa_Cod = ListaImp(i).Sa_Cod
            Movimento_Destinazione.Appezza = ListaImp(i).Appezza
            Movimento_Destinazione.Id_Destinazione = ListaImp(i).ID_Reg
            Movimento_Destinazione.Qta = ListaImp(i).Qta
            Movimento_Destinazione.Qta2 = ListaImp(i).Qta2
            tot = tot + ListaImp(i).Qta
            Movimento_Destinazione.BaseCode = Agenda.BaseCode
            Movimento_Destinazione.TopCode = Agenda.TopCode


            'salvo qui progetto cod per metytere poi nel dettaglio della destinazione nel caso di semilavorati
            Movimento_Destinazione.parametroGenerico = ListaImp(i).Progetto_Cod

            Movimento_Dettaglio.Movimenti_Destinazioni.Add(Movimento_Destinazione)
        Next

        Movimento_Dettaglio.Qta = tot

        Movimento_OperazioneColturale.Movimenti_Dettagli.Add(Movimento_Dettaglio)
        'se ho almeno un movimento dettaglio nell'operazione la aggiungo all'agenda
        If Movimento_OperazioneColturale.Movimenti_Dettagli.Count > 0 Then
            'aggiungo il movimento rilievo in campo-installazione trappola all'operazione agenda
            Agenda.Movimenti.Add(Movimento_OperazioneColturale)
        Else
            messaggio_errore = messaggio_errore & "Nessun Dato Salvato"
            Return False
        End If



        Return True
    End Function

    

    Public Function ImportaDatiDBF(ByRef righe As String(), ByVal Separatore As Char(), ByVal tipo_Importazione As Tipo_Importazione.Tipo_Importazione_ShapeFile, ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim rval As Boolean = False

        Select Case tipo_Importazione
            Case AgronicaSHPWrapper.InterpretaDatiDBF.Tipo_Importazione.Tipo_Importazione_ShapeFile.Importazione_Agrea_Crea_Planning

                rval = ImportaRigaDBF_ToPlanning(righe, Separatore, objParametri)

            Case AgronicaSHPWrapper.InterpretaDatiDBF.Tipo_Importazione.Tipo_Importazione_ShapeFile.Importazione_Trimble
                rval = ImportaPrimaRigaDBF_ToRicetta(righe, Separatore, objParametri)


        End Select


        Return rval
    End Function

    Private Function ImportaPrimaRigaDBF_ToRicetta(ByRef riga As String(), ByVal Separatore As Char(), ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim Ricetta_Operazione_Cod As Integer
        Dim lNomedir As String = GetValue(riga(0).Split({"|"c, "§"c}), "NomeDir")


        Dim lCodice As String() = lNomedir.Split("_")

        Dim lcodiceRicetta As String = lCodice(lCodice.Length - 1)
        If Not lcodiceRicetta.StartsWith("AG") Then
            Dim Agenda As Operazione_Agenda
            Agenda = CreaOggettoAgenda(riga, Separatore)

            Dim objAgendaScrivi As New Agenda_Operazione_Helper


            'Ricetta_Operazione_Cod = objAgendaScrivi.Scrivi(Agenda, objParametri)
            Ricetta_Operazione_Cod = CreaRicetta(riga, Separatore, 0, objParametri)

            'Dim agenda_cod As Integer
            'agenda_cod = objAgendaScrivi.Scrivi(Agenda, objParametri)
            'Dim ricetta As Ricetta = CreaRicetta(agenda_cod)
            'Dim objscriviRicetta As New Agenda_Ricette_Helper
            'Ricetta_Operazione_Cod = objscriviRicetta.Scrivi(ricetta, objParametri)

        Else
            Ricetta_Operazione_Cod = lcodiceRicetta.Replace("AG", "")

        End If


        riga(0) = riga(0) & "Ricetta_Operazione_Cod" & Separatore(1) & " " & Ricetta_Operazione_Cod.ToString

        Return True

    End Function

    Private Function CreaRicetta(ByVal riga As String(), ByVal Separatore As Char(), ByVal id_agenda As Integer, ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer

        Dim id_ricetta As Integer


        'RIGA

        'data_operazione§ 01/01/2012|progressivoGias§ 1|lav_cod§ 123|lav_des§ testme|cau_mov§ 1200|pivasuperuser§ 01234567890|piva§ 01234567890|sa_cod§ 123§appezza§ 123§reg_impianto§ 123anno§ 2011Version§ 5.10.038|GPS_Status§ 2|Status_Txt§ DGPS|Swath§ 8|Height§ -6,136|DateClosed§ 16/03/2012 00:00:00|TimeClosed§ 01:16:40pm|AppldRate§ 96,111|Moisture§ |Material§ Default_AppMaterial|MaterialID§ -1|Speed§ 3,144|

        Dim DatiPrimaRiga As String() = _
            riga(0).Split(Separatore)

        Dim XmlDoc As New System.Xml.XmlDocument

        Dim XMLDatiRicetta As System.Xml.XmlElement
        Dim XMLRicetta As System.Xml.XmlElement
        Dim XMLRicetta_XCultivar As System.Xml.XmlElement
        Dim XMLRicetta_Cultivar As System.Xml.XmlElement
        Dim XMLDatiRicetta_Operazioni As System.Xml.XmlElement
        Dim XMLDatiRicetta_Operazione As System.Xml.XmlElement
        Dim XMLDatiRicetta_Dettagli As System.Xml.XmlElement
        Dim XMLDatiRicetta_Dettaglio As System.Xml.XmlElement
        Dim XMLDatiRicetta_DettaglioTecnico As System.Xml.XmlElement
        Dim XMLDatiRicetta_Destinazioni As System.Xml.XmlElement


        Dim str_Ricetta_Operazione As String
        Dim str_Ricetta_Destinazione As String

        Dim Validita_Inizio As Date
        Dim Validita_Fine As Date

        Validita_Inizio = #1/1/1900#
        Validita_Fine = #12/31/2100#

        '------------------------------------------------
        '----- Genero la struttura XML
        '------------------------------------------------

        Dim objXml As New AgronicaCoreXML.XML_Contab

        Dim PivaSuperUser As String
        Dim Des As String
        Dim data_Operazione As Date
        Dim MaterialID As Integer
        Dim cod_risum As Integer
        Dim cau_mov As Integer
        Dim cod_mac As Integer
        Dim elem_cod As Integer
        Dim sup_imp As Double

        Dim programmazione_cod As Integer = GetValue(DatiPrimaRiga, "programmazione_cod")

        Dim objDestinazioni As New List(Of AgronicaCoreAnagrafeBIZ.Appezzamento_Piccolo)


        PivaSuperUser = GetValue(DatiPrimaRiga, "PivaSuperUser")



        data_Operazione = GetValue(DatiPrimaRiga, "DateClosed")

        Dim veg_cod As Integer
        Dim cul_Cod As Integer
        Dim leggiVegCod As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read



        If programmazione_cod = 0 Then

            objDestinazioni.Add(New AgronicaCoreAnagrafeBIZ.Appezzamento_Piccolo With { _
            .Piva = GetValue(DatiPrimaRiga, "Piva"), _
            .Sa_Cod = GetValue(DatiPrimaRiga, "sa_cod"), _
            .Appezza = GetValue(DatiPrimaRiga, "appezza"), _
            .Id_reg = GetValue(DatiPrimaRiga, "reg_impianto"), _
            .Superficie_IMPIANTO = sup_imp _
        })

        Else

            'leggo le destinazioni dal planning
            Dim LeggiprogDest As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_R
            Dim dtLeggiprogDest As DataTable = _
            LeggiprogDest.Leggi( _
                programmazione_cod, _
                0, _
                "", _
                "", _
                0, _
                0, _
                0, _
                0, _
                0, _
                AGRODATAINIZIO, _
                AGRODATAFINE, _
                "", _
                "", _
                "", _
                objParametri _
            )

            For Each drLeggiprogDest In dtLeggiprogDest.Rows
                objDestinazioni.Add( _
                    New AgronicaCoreAnagrafeBIZ.Appezzamento_Piccolo With { _
                        .Piva = drLeggiprogDest("Piva"), _
                        .Sa_Cod = drLeggiprogDest("sa_cod"), _
                        .Appezza = drLeggiprogDest("appezza"), _
                        .Id_reg = drLeggiprogDest("id_reg"), _
                        .Superficie_IMPIANTO = drLeggiprogDest("Superficie")
                })
            Next


        End If

        Dim DestinazioniRif As AgronicaCoreAnagrafeBIZ.Appezzamento_Piccolo = objDestinazioni.FirstOrDefault
        Dim dtleggiVegCod As DataTable = _
        leggiVegCod.Leggi_SpecieVarieta( _
            DestinazioniRif.Piva, _
            DestinazioniRif.Sa_Cod, _
            DestinazioniRif.Appezza, _
            DestinazioniRif.Id_reg, _
            veg_cod, _
            cul_Cod, _
            AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
            "", _
            "", _
            objParametri _
        )

        If dtleggiVegCod.Rows.Count > 0 Then
            veg_cod = dtleggiVegCod.Rows(0)("veg_cod")
            cul_Cod = dtleggiVegCod.Rows(0)("cul_cod")
            sup_imp = dtleggiVegCod.Rows(0)("Sup_imp")
        End If

        If programmazione_cod = 0 Then
            DestinazioniRif.Superficie_IMPIANTO = sup_imp
        End If


        Des = GetValue(DatiPrimaRiga, "lav_des")
        Des &= " (" & data_Operazione & ")"

        MaterialID = GetValue(DatiPrimaRiga, "MaterialID")
        cod_risum = GetValue(DatiPrimaRiga, "cod_risum")
        cod_mac = GetValue(DatiPrimaRiga, "cod_mac")

        cau_mov = GetValue(DatiPrimaRiga, "cau_mov")

        elem_cod = 191


        Dim BaseCode As Integer
        Dim TopCode As Integer
        ProgressivoBaseTop(DatiPrimaRiga, BaseCode, TopCode)

        '----- DatiRicetta
        XMLDatiRicetta = XmlDoc.CreateElement("DatiRicetta")


        '----- Ricetta
        XMLDatiRicetta.InnerXml = objXml.XML_Ricetta( _
                                        CInt(enum_TipoOperazioneDB.Scrittura), _
                                        CStr(PivaSuperUser), _
                                        BaseCode, _
                                        TopCode, _
                                        0, _
                                        Des, _
                                        Des, _
                                        veg_cod, _
                                        "", _
                                        data_Operazione, _
                                        data_Operazione, _
                                        DestinazioniRif.Piva, _
                                        DestinazioniRif.Sa_Cod)

        XMLRicetta = XMLDatiRicetta.SelectSingleNode("Ricetta")

        '----- DatiRicettaxCultivar
        XMLRicetta_XCultivar = XmlDoc.CreateElement("DatiRicettaxCultivar")

        XMLRicetta_Cultivar = XmlDoc.CreateElement("RicettaxCultivar")

        Dim str_Ricetta_cultivar As String = objXml.XML_RicettaxCultivar( _
            AgronicaCoreXML.XML_Contab.enum_TipoOperazioneDB.Scrittura, _
            PivaSuperUser, _
            Veg_Cod:=veg_cod, _
            Cul_Cod:=cul_Cod, _
            Validita_Inizio:=data_Operazione, _
            Validita_Fine:=data_Operazione _
        )

        XMLRicetta_Cultivar.InnerXml = str_Ricetta_cultivar
        XMLRicetta_Cultivar = XMLRicetta_Cultivar.SelectSingleNode("RicettaxCultivar")

        XMLRicetta_XCultivar.AppendChild(XMLRicetta_Cultivar)
        XMLRicetta.AppendChild(XMLRicetta_XCultivar)

        '----- DatiRicetta_Operazioni

        XMLDatiRicetta_Operazioni = XmlDoc.CreateElement("DatiRicetta_Operazioni")

        ' recupero la OPERAZIONE
        Dim Lav_Cod As String = 0
        Dim Lav_Des As String = ""


        Lav_Cod = GetValue(DatiPrimaRiga, "lav_cod")
        Lav_Des = GetValue(DatiPrimaRiga, "lav_des")

        XMLDatiRicetta_Dettagli = XmlDoc.CreateElement("DatiRicetta_Dettagli")

        XMLDatiRicetta_Dettaglio = XmlDoc.CreateElement("Ricetta_Dettaglio")
        Dim str_Ricetta_Dettaglio As String = objXml.XML_Ricetta_Dettaglio(AgronicaCoreXML.XML_Contab.enum_TipoOperazioneDB.Scrittura, _
                                            PivaSuperUser, _
                                            Cau_Mov:=cau_mov _
                                        )


        XMLDatiRicetta_DettaglioTecnico = XmlDoc.CreateElement("Ricetta_Dettaglio_Tecnico_2")


        Dim lAvCod As Integer = 0
        If Lav_Cod = CostantiPersonalizzate.LAVCOD_TRATTAMENTO_ANTIPARASSITARIO Then
            lAvCod = 1
        End If
        If Lav_Cod = CostantiPersonalizzate.LAVCOD_DISERBO Then
            lAvCod = 1
        End If

        Dim str_Ricetta_DettaglioTecnico As String = objXml.XML_Ricetta_DettaglioTecnico_2( _
                    AgronicaCoreXML.XML_Contab.enum_TipoOperazioneDB.Scrittura, _
                    PivaSuperUser, _
                    Av_Cod:=lAvCod _
                )



        XMLDatiRicetta_DettaglioTecnico.InnerXml = str_Ricetta_DettaglioTecnico
        XMLDatiRicetta_DettaglioTecnico = XMLDatiRicetta_DettaglioTecnico.SelectSingleNode("Ricetta_Dettaglio_Tecnico_2")

        XMLDatiRicetta_Operazione = XmlDoc.CreateElement("Ricetta_Operazione")

        Dim mezzo As Integer = 0
        If Lav_Cod = CostantiPersonalizzate.LAVCOD_DISTRIBUZIONE_CONCIME Then
            mezzo = 1
        End If
        str_Ricetta_Operazione = objXml.XML_Ricetta_Operazione(
                                         AgronicaCoreXML.XML_Contab.enum_TipoOperazioneDB.Scrittura,
                                         PivaSuperUser,
                                         BaseCode,
                                         TopCode,
                                         Lav_Cod:=Lav_Cod,
                                         Num_Protocollo:=-1,
                                         Ricetta_Operazione_Des:=Des,
                                         Mezzo:=mezzo,
                                         W_Anagrafica_Stati_Cod:=enum_WWorflow_WAnagraficaStati.Esecuzione_ed_avanzamento_delle_ricette_Eseguita
                                    )


        XMLDatiRicetta_Operazione.InnerXml = str_Ricetta_Operazione
        XMLDatiRicetta_Operazione = XMLDatiRicetta_Operazione.SelectSingleNode("Ricetta_Operazione")

        'parco macchine ed operatori


        XMLDatiRicetta_Dettaglio.InnerXml = str_Ricetta_Dettaglio
        XMLDatiRicetta_Dettaglio = XMLDatiRicetta_Dettaglio.SelectSingleNode("Ricetta_Dettaglio")

        '----- DatiRicetta_Destinazioni
        For Each singleDest In objDestinazioni
            XMLDatiRicetta_Destinazioni = XmlDoc.CreateElement("Ricetta_Destinazione")

            str_Ricetta_Destinazione = objXml.XML_Ricetta_Destinazione( _
                                AgronicaCoreXML.XML_Contab.enum_TipoOperazioneDB.Scrittura, _
                                PivaSuperUser, _
                                Piva:=singleDest.Piva, _
                                Sa_Cod:=singleDest.Sa_Cod, _
                                Appezza:=singleDest.Appezza, _
                                Id_Reg:=singleDest.Id_reg, _
                                Qta:=singleDest.Superficie_IMPIANTO _
                                )

            'operazione vera e propria
            XMLDatiRicetta_Destinazioni.InnerXml = str_Ricetta_Destinazione
            XMLDatiRicetta_Destinazioni = XMLDatiRicetta_Destinazioni.SelectSingleNode("Ricetta_Destinazione")
            XMLDatiRicetta_Dettaglio.AppendChild(XMLDatiRicetta_Destinazioni)

        Next

        XMLDatiRicetta_Dettaglio.AppendChild(XMLDatiRicetta_DettaglioTecnico)

        XMLDatiRicetta_Dettagli.AppendChild(XMLDatiRicetta_Dettaglio)

        Dim str_ricetta_dettagli_manodopera As String
        Const oreUomo As Integer = 8
        str_ricetta_dettagli_manodopera = objXml.XML_Ricetta_Dettaglio(AgronicaCoreXML.XML_Contab.enum_TipoOperazioneDB.Scrittura, _
                                                        PivaSuperUser, _
                                                        Cau_Mov:=CostantiPersonalizzate.CAU_IMPUTAZIONE_MANODOPERA, _
                                                        Elem_Cod:=0, _
                                                        Mat_Cod:=cod_risum, _
                                                        Udm_Cod:=0, _
                                                        Qta:=1 _
                                                        )

        Dim str_ricetta_dettagli_parcoMacchine As String
        Const oreMacchina As Integer = 9
        str_ricetta_dettagli_parcoMacchine = objXml.XML_Ricetta_Dettaglio(AgronicaCoreXML.XML_Contab.enum_TipoOperazioneDB.Scrittura, _
                                                        PivaSuperUser, _
                                                        Cau_Mov:=CostantiPersonalizzate.CAU_IMPUTAZIONE_PARCOMACCHINE, _
                                                        Elem_Cod:=1, _
                                                        Mat_Cod:=cod_mac, _
                                                        Udm_Cod:=0, _
                                                        Qta:=1 _
                                                        )

        'manodopera
        If cod_risum <> 0 Then
            XMLDatiRicetta_Dettaglio = XmlDoc.CreateElement("Ricetta_Dettaglio")
            XMLDatiRicetta_Dettaglio.InnerXml = str_ricetta_dettagli_manodopera
            XMLDatiRicetta_Dettaglio = XMLDatiRicetta_Dettaglio.SelectSingleNode("Ricetta_Dettaglio")
            XMLDatiRicetta_Dettagli.AppendChild(XMLDatiRicetta_Dettaglio)
        End If

        'macchine
        If cod_mac <> 0 Then
            XMLDatiRicetta_Dettaglio = XmlDoc.CreateElement("Ricetta_Dettaglio")
            XMLDatiRicetta_Dettaglio.InnerXml = str_ricetta_dettagli_parcoMacchine
            XMLDatiRicetta_Dettaglio = XMLDatiRicetta_Dettaglio.SelectSingleNode("Ricetta_Dettaglio")
            XMLDatiRicetta_Dettagli.AppendChild(XMLDatiRicetta_Dettaglio)
        End If

        'fine parco macchine ed operatori


        XMLDatiRicetta_Operazione.AppendChild(XMLDatiRicetta_Dettagli)
        XMLDatiRicetta_Operazioni.AppendChild(XMLDatiRicetta_Operazione)

        XMLRicetta.AppendChild(XMLDatiRicetta_Operazioni)





        '----- Assemblo la struttura

        XmlDoc.AppendChild(XMLDatiRicetta)

        objXml = Nothing

        '----- Restituisco il risultato

        Dim sXmlRicetta As String = XmlDoc.OuterXml
        Dim objRicetta_Write As New AgronicaCoreContabBIZ.Ricette_W 'Object

        objRicetta_Write.Ricetta_Scrivi(sXmlRicetta, id_ricetta, objParametri)

        Dim ricettaoperazioneRead As New AgronicaCoreContabDAL.Ricette_Operazioni_R
        Dim dt As DataTable
        dt = ricettaoperazioneRead.Leggi(id_ricetta, 0, 0, 0, CostantiPersonalizzate.AGRODATAINIZIO, CostantiPersonalizzate.AGRODATAFINE, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)

        Dim Ricetta_Operazione_cod As Integer
        If dt.Rows.Count > 0 Then
            Ricetta_Operazione_cod = dt(0)("Ricetta_Operazione_cod")
        End If


        Return Ricetta_Operazione_cod
    End Function

    Private Shared Sub ProgressivoBaseTop(ByVal DatiPrimaRiga As String(), ByRef BaseCode As Integer, ByRef TopCode As Integer)
        Dim progressivoGias As Integer

        progressivoGias = GetValue(DatiPrimaRiga, "progressivoGias")

        '------------------------------------------------
        '----- Calcolo i valori di BaseCode e TopCode
        '------------------------------------------------
        Call Calcola_BaseCode_TopCode(BaseCode, _
                              TopCode, _
                              progressivoGias)
    End Sub
    '#############################################################################################################
    Private Function CreaOggettoAgenda(ByVal riga As String(), ByVal Separatore As Char()) As Operazione_Agenda

        'RIGA

        'data_operazione§ 01/01/2012|progressivoGias§ 1|lav_cod§ 123|lav_des§ testme|cau_mov§ 1200|pivasuperuser§ 01234567890|piva§ 01234567890|sa_cod§ 123§appezza§ 123§reg_impianto§ 123anno§ 2011Version§ 5.10.038|GPS_Status§ 2|Status_Txt§ DGPS|Swath§ 8|Height§ -6,136|DateClosed§ 16/03/2012 00:00:00|TimeClosed§ 01:16:40pm|AppldRate§ 96,111|Moisture§ |Material§ Default_AppMaterial|MaterialID§ -1|Speed§ 3,144|

        Dim DatiPrimaRiga As String() = _
            riga(0).Split(Separatore)


        Dim PivaSuperUser As String
        Dim Piva As String
        Dim sa_Cod As Integer
        Dim Appezza As Integer
        Dim reg_impianto As Integer

        Dim data_Operazione As Date

        PivaSuperUser = GetValue(DatiPrimaRiga, "PivaSuperUser")
        Piva = GetValue(DatiPrimaRiga, "Piva")
        sa_Cod = GetValue(DatiPrimaRiga, "sa_cod")
        Appezza = GetValue(DatiPrimaRiga, "appezza")
        reg_impianto = GetValue(DatiPrimaRiga, "reg_impianto")
        Try
            data_Operazione = GetValue(DatiPrimaRiga, "DateClosed")
        Catch ex As Exception
            data_Operazione = Now
        End Try


        Dim id_Agenda As Integer

        'ByVal Qta_da_scaricare As List(Of Double),
        ''---------------------------------------
        ' recupero le NOTE
        'Dim strNota As String = txt_note.Text

        '---------------------------------------
        ' recupero la DATA



        ' recupero la OPERAZIONE
        Dim Lav_Cod As String = 0
        Dim Lav_Des As String = ""


        Lav_Cod = GetValue(DatiPrimaRiga, "lav_cod")
        Lav_Des = GetValue(DatiPrimaRiga, "lav_des")
        Dim BaseCode As Integer
        Dim TopCode As Integer
        ProgressivoBaseTop(DatiPrimaRiga, BaseCode, TopCode)
        '------------------------------------------------
        '----- AGENDA
        '------------------------------------------------
        Dim Agenda As New Operazione_Agenda

        Agenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
        Agenda.Id_Agenda = 0
        Agenda.Data = data_Operazione
        Agenda.Piva = Piva
        Agenda.Sa_Cod = sa_Cod
        Agenda.Lav_Cod = Lav_Cod
        Agenda.Des_Lib = Lav_Des

        Agenda.BaseCode = BaseCode
        Agenda.TopCode = TopCode

        Dim mov As New AgronicaCoreModello.OperazioneAgenda_Temp.Movimento
        Dim cau_mov As Integer = GetValue(DatiPrimaRiga, "cau_mov")
        mov.Cau_Mov = cau_mov
        mov.Mov_Desc = ""

        mov.Piva = Agenda.Piva
        mov.Sa_Cod = Agenda.Sa_Cod
        mov.Data = data_Operazione
        ' il codice della risorsa umana va nei movimenti dettagli
        ' mov.Cod_Risum = 0   'ddl_Contatti.SelectedItem.Value    

        Dim movdet As New AgronicaCoreModello.OperazioneAgenda_Temp.Movimento_Dettaglio
        movdet.Piva = Agenda.Piva
        movdet.Sa_Cod = Agenda.Sa_Cod

        movdet.Elem_Cod = 0
        movdet.Pro_Cod = 0
        Dim MaterialID As Integer = GetValue(DatiPrimaRiga, "MaterialID")
        movdet.Mat_Cod = MaterialID
        movdet.Qta = 0
        movdet.Turno_Cod = 0
        movdet.ID_Attivita = 0
        movdet.Veg_Cod = 0

        movdet.Data = Agenda.Data

        Dim movDest As New AgronicaCoreModello.OperazioneAgenda_Temp.Movimento_Destinazione
        movDest.Piva = Piva
        movDest.Sa_Cod = sa_Cod
        movDest.Tipo = 20
        movDest.Id_Destinazione = reg_impianto
        movDest.Appezza = Appezza

        movdet.Movimenti_Destinazioni.Add(movDest)

        mov.Movimenti_Dettagli.Add(movdet)


        Agenda.Movimenti.Add(mov)

        Return Agenda


    End Function




    Private Function ImportaRigaDBF_ToPlanning(ByRef righe As String(), ByVal Separatore As Char(), ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        'pivasuperuser: 01234567890|piva: 01234567890|anno: 2011|ISTATP: 039|SIGLA_PROV: RA|ISTATC: 014|NOME: RAVENNA-SANT`ALBERTO|ID_SEZC: B|FOGLIO: 137|PARTICELLA: 00127|SUB: |PROG_POLIG: 9|CUL_COD: 1|COD_VARIET: 1|DESCRIZION: GRANTURCO (MAIS)|AREA_COLT: 78646,81|DATA_SEMINA: 11/11/2011|DATA_RACCOLTA: 30/06/2012

        Dim PivaSuperUser As String
        Dim Piva As String
        Dim sa_Cod As Integer
        Dim rval As Boolean = True

        Dim ScriviProgrammazione As New AgronicaCoreAnagrafeBIZ.Programmazione_W

        Dim DTProgrammazione_Particelle As New DataTable
        dammiDTProgrammazioneParticelle(DTProgrammazione_Particelle)

        Dim DTProgrammazione_Entita As New DataTable
        dammiDTProgrammazione_Entita(DTProgrammazione_Entita)

        Dim DTProgrammazione_Entita_Eliminate As New DataTable
        dammiDTProgrammazione_Entita_Eliminate(DTProgrammazione_Entita_Eliminate)

        Dim TipoOperazione_Particella As enum_TipoOperazioneDB


        Dim Prov As String = String.Empty
        Dim Com As String = String.Empty
        Dim strSezione As String = String.Empty
        Dim Foglio As String = String.Empty
        Dim Numero As String = String.Empty
        Dim strSubalterno As String = String.Empty


        Dim DatiPrimaRiga As String() = _
            righe(0).Split(Separatore)

        Dim anno As Integer = -1
        If DatiPrimaRiga.Contains("ANNO") Then
            anno = GetValue(DatiPrimaRiga, "anno")
        End If

        Dim ValiditaInizio As DateTime
        Dim ValiditaFine As DateTime


        If anno <> -1 Then
            ValiditaInizio = CType((anno - 1).ToString & "-11-01", DateTime)
            ValiditaFine = CType(anno.ToString & "-10-31", DateTime)
        Else
            If DatiPrimaRiga.Contains("DATA_INIZIO") AndAlso GetValue(DatiPrimaRiga, "DATA_INIZIO") <> "" Then
                ValiditaInizio = CType(GetValue(DatiPrimaRiga, "DATA_INIZIO"), DateTime)
                ValiditaFine = CType(GetValue(DatiPrimaRiga, "DATA_FINE"), DateTime)
            Else
                ValiditaInizio = AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO
                ValiditaFine = AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAFINE
            End If
        End If


        PivaSuperUser = GetValue(DatiPrimaRiga, "PivaSuperUser")
        Piva = GetValue(DatiPrimaRiga, "Piva")


        Dim Tipo_Operazione_Db As Integer = 1
        If DatiPrimaRiga.Contains("TipoOperazione_DB") Then
            Tipo_Operazione_Db = GetValue(DatiPrimaRiga, "TipoOperazione_DB")
        End If

        Dim Programmazione_Des As String = ""
        If DatiPrimaRiga.Contains("Programmazione_Des") Then
            Programmazione_Des = GetValue(DatiPrimaRiga, "Programmazione_Des")
        End If

        Dim Programmazione_cod As Integer = 0
        Dim Programmazione_Entita_cod As Integer = 0

        Programmazione_cod = GetValue(DatiPrimaRiga, "Programmazione_Cod")

        For Each Riga As String In righe

            Dim dati As String() = Riga.Split(Separatore)

            Dim objImpresexParticelle As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R

            Dim cacCodificaSpecie As New AgronicaCoreAnagrafeDAL.CAC_Codifica_Veg_Cod_R

            Dim veg_cod As Integer


            If Tipo_Operazione_Db <> 1 Then
                Programmazione_Entita_cod = GetValue(dati, "Programmazione_Entita_Cod")
            End If

            If dati.Contains("VEG_COD") Then
                veg_cod = GetValue(dati, "VEG_COD")
            Else
                veg_cod = cacCodificaSpecie.VegCodGias_from_VegCodCliente( _
                            GetValue(dati, "COD_VARIET"), _
                            objParametri)
            End If

            Dim Cul_cod As Integer
            If dati.Contains("CUL_COD") Then
                Cul_cod = GetValue(dati, "CUL_COD")
            Else
                Cul_cod = 0
            End If

            Dim grva_cod As Integer
            If dati.Contains("tipologia") Then
                grva_cod = GetValue(dati, "tipologia")
            Else
                grva_cod = 0
            End If


            Dim id_cod As Integer = 0

            Dim lMquadrati As String = GetValue(dati, "AREA_COLT")
            Dim dblSupEttari As Double

            If Not dati.Contains("udm_sup") Then
                dblSupEttari = AgronicaGIS2012.Commons.xyz.myCDBL(lMquadrati) / 10000
            Else
                dblSupEttari = AgronicaGIS2012.Commons.xyz.myCDBL(lMquadrati)
            End If

            Dim data_semina As DateTime?
            Dim data_raccolta As DateTime?
            Dim codice_fiscale_Tecnico As String = ""

            If dati.Contains("DATA_SEMINA") AndAlso GetValue(dati, "DATA_SEMINA") <> "" Then
                data_semina = GetValue(dati, "DATA_SEMINA")
            End If

            If dati.Contains("DATA_RACCOLTA") AndAlso GetValue(dati, "DATA_RACCOLTA") <> "" Then
                data_raccolta = GetValue(dati, "DATA_RACCOLTA")
            End If

            If data_semina Is Nothing Then
                data_semina = AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO
            End If

            If data_raccolta Is Nothing Then
                data_raccolta = AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAFINE
            End If

            If dati.Contains("CODICE_FISCALE_TECNICO") AndAlso GetValue(dati, "CODICE_FISCALE_TECNICO") <> "" Then
                codice_fiscale_Tecnico = GetValue(dati, "CODICE_FISCALE_TECNICO")
            End If

            If dati.Contains("sa_cod") AndAlso GetValue(dati, "sa_cod") <> "" Then
                sa_Cod = GetValue(dati, "sa_cod")
            End If

            Dim Finalita As String = "0"
            If dati.Contains("finalita") AndAlso GetValue(dati, "finalita") <> "" Then
                Finalita = GetValue(dati, "finalita")
            End If

            Dim Appezza As String = "0"
            If dati.Contains("appezza") AndAlso GetValue(dati, "appezza") <> "" Then
                Appezza = GetValue(dati, "appezza")
            End If

            Dim id_reg As String = "0"
            If dati.Contains("id_reg") AndAlso GetValue(dati, "id_reg") <> "" Then
                id_reg = GetValue(dati, "id_reg")
            End If

            Dim via_stringa As String = ""
            If dati.Contains("via_stringa") AndAlso GetValue(dati, "via_stringa") <> "" Then
                via_stringa = GetValue(dati, "via_stringa")
            End If

            Dim dRow_DTProgrammazione_Entita = DTProgrammazione_Entita.NewRow

            dRow_DTProgrammazione_Entita("Piva") = Piva
            dRow_DTProgrammazione_Entita("Piva_SuperUSer") = PivaSuperUser
            dRow_DTProgrammazione_Entita("Programmazione_Entita_Cod") = Programmazione_Entita_cod
            dRow_DTProgrammazione_Entita("Programmazione_Cod") = Programmazione_cod
            dRow_DTProgrammazione_Entita("Sa_Cod") = sa_Cod
            dRow_DTProgrammazione_Entita("Campo_Cod") = 0
            dRow_DTProgrammazione_Entita("Appezza") = Appezza
            dRow_DTProgrammazione_Entita("id_reg") = id_reg
            dRow_DTProgrammazione_Entita("App_Nome") = ""
            dRow_DTProgrammazione_Entita("imp_cod") = 0
            dRow_DTProgrammazione_Entita("MetodoProduzione_Cod") = 0
            dRow_DTProgrammazione_Entita("FlagIrrigabilita") = 0
            dRow_DTProgrammazione_Entita("FlagSecondoRaccolto") = 0
            dRow_DTProgrammazione_Entita("Grva_Cod") = grva_cod
            dRow_DTProgrammazione_Entita("Foral_Cod") = 0
            dRow_DTProgrammazione_Entita("Port_Cod") = 0
            dRow_DTProgrammazione_Entita("Imp_Cod") = 0
            dRow_DTProgrammazione_Entita("Regolamento_Cod") = 0
            dRow_DTProgrammazione_Entita("Disciplinare_Cod") = 0
            dRow_DTProgrammazione_Entita("Veg_Cod_Prec") = 0
            dRow_DTProgrammazione_Entita("Id_Mat_O") = 0
            dRow_DTProgrammazione_Entita("Id_Fre") = 0
            dRow_DTProgrammazione_Entita("Num_Piante") = 0
            dRow_DTProgrammazione_Entita("Stato_Cod") = 0
            dRow_DTProgrammazione_Entita("Ciclo") = 0
            dRow_DTProgrammazione_Entita("Progetto_Cod") = 0
            dRow_DTProgrammazione_Entita("Id_Cod") = 0
            dRow_DTProgrammazione_Entita("Veg_Cod") = veg_cod
            dRow_DTProgrammazione_Entita("Cul_Cod") = Cul_cod
            dRow_DTProgrammazione_Entita("Grfi_Cod") = Finalita
            dRow_DTProgrammazione_Entita("Cop_Cod") = 0
            dRow_DTProgrammazione_Entita("Data_Semina") = data_semina
            dRow_DTProgrammazione_Entita("Data_Raccolta") = data_raccolta
            dRow_DTProgrammazione_Entita("Superficie") = dblSupEttari
            dRow_DTProgrammazione_Entita("Sup_App") = dblSupEttari
            dRow_DTProgrammazione_Entita("Resa") = 0
            dRow_DTProgrammazione_Entita("N_distribuito") = 0
            dRow_DTProgrammazione_Entita("N_fabbisogno") = 0
            dRow_DTProgrammazione_Entita("TRA_Fila") = 0
            dRow_DTProgrammazione_Entita("Su_Fila") = 0
            dRow_DTProgrammazione_Entita("Progetto_Cod") = 0
            dRow_DTProgrammazione_Entita("Progetto_Nome") = 0
            dRow_DTProgrammazione_Entita("DestinazioneUso") = 0
            dRow_DTProgrammazione_Entita("TipoZona") = 0
            dRow_DTProgrammazione_Entita("Validita_Inizio") = ValiditaInizio
            dRow_DTProgrammazione_Entita("Validita_Fine") = ValiditaFine
            dRow_DTProgrammazione_Entita("UNID_APP") = 0
            dRow_DTProgrammazione_Entita("codice_fiscale_Tecnico") = codice_fiscale_Tecnico
            dRow_DTProgrammazione_Entita("Veg_Cod_Cliente") = 0
            dRow_DTProgrammazione_Entita("Cul_Cod_Cliente") = 0
            dRow_DTProgrammazione_Entita("macrouso_cod") = 0
            dRow_DTProgrammazione_Entita("via_stringa") = via_stringa.Replace("'", "`")
            dRow_DTProgrammazione_Entita("unita_vitata") = 0
            dRow_DTProgrammazione_Entita("Validita_Inizio_Impianto") = ValiditaInizio


            DTProgrammazione_Entita.Rows.Add(dRow_DTProgrammazione_Entita)




            If dati.Contains("ISTATP") Then
                Prov = GetValue(dati, "ISTATP")
                Com = GetValue(dati, "ISTATC")
                strSezione = GetValue(dati, "ID_SEZC")
                Foglio = GetValue(dati, "FOGLIO")
                Numero = GetValue(dati, "PARTICELLA")
                strSubalterno = GetValue(dati, "SUB")
            End If

            If strSubalterno = "" Then
                strSubalterno = "0"
            End If

            'valore fisso a zero perchè gias non ha mai considerato la sezione nelle particelle
            strSezione = "0"

            Dim dtLetturaSaCod As DataTable = _
            objImpresexParticelle.LeggixChiave( _
                0, _
                Piva, _
                0, _
                Prov, _
                Com, _
                strSezione, _
                CInt(Foglio), _
                CInt(Numero), _
                strSubalterno, _
                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                " ImpresexParticelle.Validita_fine >= cast('" & ValiditaFine & "' as date )", _
                "", _
                objParametri _
            )



            If dtLetturaSaCod.Rows.Count > 0 Then
                sa_Cod = dtLetturaSaCod.Rows(0)("sa_Cod")
                TipoOperazione_Particella = enum_TipoOperazioneDB.Modifica
            Else
                TipoOperazione_Particella = enum_TipoOperazioneDB.Scrittura
            End If



            Dim Ettari As Integer = 0
            Dim Are As Integer = 0
            Dim Centiare As Integer = 0

            AgronicaCoreDataProvider.UtilityProvider.EttariAreCentiare_from_Ettari(dblSupEttari, Ettari, Are, Centiare)

            If Prov <> -1 Then
                Dim dRow_DTProgrammazione_Particelle = DTProgrammazione_Particelle.NewRow
                dRow_DTProgrammazione_Particelle("Programmazione_Entita_Cod") = 0
                dRow_DTProgrammazione_Particelle("Foglio") = Foglio
                dRow_DTProgrammazione_Particelle("Numero") = Numero
                dRow_DTProgrammazione_Particelle("Superficie") = dblSupEttari
                dRow_DTProgrammazione_Particelle("Piva_SuperUser") = PivaSuperUser
                dRow_DTProgrammazione_Particelle("sa_cod") = sa_Cod
                dRow_DTProgrammazione_Particelle("Appezza") = 0
                dRow_DTProgrammazione_Particelle("Campo_cod") = 0
                dRow_DTProgrammazione_Particelle("Prov") = Prov
                dRow_DTProgrammazione_Particelle("Com") = Com
                dRow_DTProgrammazione_Particelle("Sezione") = strSezione
                dRow_DTProgrammazione_Particelle("Subalterno") = strSubalterno
            End If


        Next



        Try

            Dim lTipoPianificazione As Integer = 0
            If DatiPrimaRiga.Contains("TipoPianificazione") AndAlso GetValue(DatiPrimaRiga, "TipoPianificazione") <> "" Then
                lTipoPianificazione = GetValue(DatiPrimaRiga, "TipoPianificazione")
            End If

            Programmazione_cod = _
            ScriviProgrammazione.Pianificazione_Scrivi( _
                Tipo_Operazione_Db, _
                Programmazione_cod, _
                Programmazione_Des, _
                "", _
                Piva, _
                "", _
                objParametri.UsernameOperazione, _
                ValiditaInizio, _
                ValiditaFine, _
                lTipoPianificazione, _
                0, _
                DTProgrammazione_Entita, _
                DTProgrammazione_Particelle, _
                DTProgrammazione_Entita_Eliminate, _
                "", _
                AGRODATAINIZIO, _
                objParametri _
            )

            Dim idx As Integer = 0
            For Each rrow In DTProgrammazione_Entita.Rows

                righe(idx) = righe(idx) & "Programmazione_Cod§ " & Programmazione_cod & "|Programmazione_Entita_Cod§ " & rrow("Programmazione_Entita_Cod") & "|"
                idx += 1
            Next

        Catch ex As Exception
            rval = False
        End Try

        Return rval


    End Function



    Private Shared Sub dammiDTProgrammazione_Entita(ByVal DTProgrammazione_Entita As DataTable)


        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "Programmazione_Entita_Cod", .DataType = System.Type.GetType("System.Int32")})
        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "Programmazione_Cod", .DataType = System.Type.GetType("System.Int32")})
        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "Contratto_Cod", .DataType = System.Type.GetType("System.Int32")})
        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "Id_Programmazione_Entita", .DataType = System.Type.GetType("System.Int32")})
        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "Sa_Cod", .DataType = System.Type.GetType("System.Int32")})
        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "Campo_Cod", .DataType = System.Type.GetType("System.Int32")})
        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "Appezza", .DataType = System.Type.GetType("System.Int32")})
        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "Veg_Cod", .DataType = System.Type.GetType("System.Int32")})
        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "MetodoProduzione_Cod", .DataType = System.Type.GetType("System.Int32")})
        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "Ciclo", .DataType = System.Type.GetType("System.Int32")})
        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "Foral_Cod", .DataType = System.Type.GetType("System.Int32")})
        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "Port_Cod", .DataType = System.Type.GetType("System.Int32")})
        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "Imp_Cod", .DataType = System.Type.GetType("System.Int32")})
        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "Regolamento_Cod", .DataType = System.Type.GetType("System.Int32")})
        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "Disciplinare_Cod", .DataType = System.Type.GetType("System.Int32")})
        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "Cul_Cod", .DataType = System.Type.GetType("System.Int32")})
        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "Grfi_Cod", .DataType = System.Type.GetType("System.Int32")})
        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "Cop_Cod", .DataType = System.Type.GetType("System.Int32")})
        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "Num_Piante", .DataType = System.Type.GetType("System.Int32")})
        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "Grva_Cod", .DataType = System.Type.GetType("System.Int32")})
        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "Stato_Cod", .DataType = System.Type.GetType("System.Int32")})
        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "FlagIrrigabilita", .DataType = System.Type.GetType("System.Int32")})
        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "FlagSecondoRaccolto", .DataType = System.Type.GetType("System.Int32")})
        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "Data_Semina", .DataType = System.Type.GetType("System.DateTime")})
        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "Data_Raccolta", .DataType = System.Type.GetType("System.DateTime")})
        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "Superficie", .DataType = System.Type.GetType("System.String")})
        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "Resa", .DataType = System.Type.GetType("System.String")})
        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "N_fabbisogno", .DataType = System.Type.GetType("System.String")})
        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "TRA_Fila", .DataType = System.Type.GetType("System.String")})
        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "SU_Fila", .DataType = System.Type.GetType("System.String")})
        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "Piva_SuperUser", .DataType = System.Type.GetType("System.String")})
        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "Piva", .DataType = System.Type.GetType("System.String")})
        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "Note", .DataType = System.Type.GetType("System.String")})
        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "Veg_Cod_Cliente", .DataType = System.Type.GetType("System.String")})
        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "Cul_Cod_Cliente", .DataType = System.Type.GetType("System.String")})

        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "Veg_Cod_Prec", .DataType = System.Type.GetType("System.Int32")})
        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "Id_Mat_O", .DataType = System.Type.GetType("System.Int32")})
        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "Id_Fre", .DataType = System.Type.GetType("System.Int32")})
        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "Progetto_Cod", .DataType = System.Type.GetType("System.Int32")})

        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "id_reg", .DataType = System.Type.GetType("System.Int32")})
        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "id_cod", .DataType = System.Type.GetType("System.Int32")})
        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "n_distribuito", .DataType = System.Type.GetType("System.Int32")})

        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "App_Nome", .DataType = System.Type.GetType("System.String")})
        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "Progetto_Nome", .DataType = System.Type.GetType("System.String")})

        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "DestinazioneUso", .DataType = System.Type.GetType("System.Int32")})
        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "TipoZona", .DataType = System.Type.GetType("System.Int32")})
        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "SUP_APP", .DataType = System.Type.GetType("System.Double")})

        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "Validita_Inizio", .DataType = System.Type.GetType("System.DateTime")})
        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "Validita_Fine", .DataType = System.Type.GetType("System.DateTime")})

        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "UNID_APP", .DataType = System.Type.GetType("System.Int32")})

        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "Codice_Fiscale_Tecnico", .DataType = System.Type.GetType("System.String")})
        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "macrouso_cod", .DataType = System.Type.GetType("System.Int32")})

        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "via_stringa", .DataType = System.Type.GetType("System.String")})

        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "unita_vitata", .DataType = System.Type.GetType("System.Int32")})

        DTProgrammazione_Entita.Columns.Add(New DataColumn With {.ColumnName = "Validita_Inizio_Impianto", .DataType = System.Type.GetType("System.DateTime")})

    End Sub

    Private Shared Sub dammiDTProgrammazione_Entita_Eliminate(ByRef DTProgrammazione_Entita_Eliminate As DataTable)
        DTProgrammazione_Entita_Eliminate.Columns.Add(New DataColumn With {.ColumnName = "UNID_APP_NEW", .DataType = System.Type.GetType("System.Int32")})
    End Sub

    Private Shared Sub dammiDTProgrammazioneParticelle(ByRef DTProgrammazione_Particelle As DataTable)
        DTProgrammazione_Particelle.Columns.Add(New DataColumn With {.ColumnName = "Programmazione_Entita_Cod", .DataType = System.Type.GetType("System.Int32")})
        DTProgrammazione_Particelle.Columns.Add(New DataColumn With {.ColumnName = "Foglio", .DataType = System.Type.GetType("System.Int32")})
        DTProgrammazione_Particelle.Columns.Add(New DataColumn With {.ColumnName = "Numero", .DataType = System.Type.GetType("System.Int32")})
        DTProgrammazione_Particelle.Columns.Add(New DataColumn With {.ColumnName = "Superficie", .DataType = System.Type.GetType("System.String")})
        DTProgrammazione_Particelle.Columns.Add(New DataColumn With {.ColumnName = "Piva_SuperUser", .DataType = System.Type.GetType("System.String")})
        DTProgrammazione_Particelle.Columns.Add(New DataColumn With {.ColumnName = "piva", .DataType = System.Type.GetType("System.String")})
        DTProgrammazione_Particelle.Columns.Add(New DataColumn With {.ColumnName = "sa_cod", .DataType = System.Type.GetType("System.Int32")})
        DTProgrammazione_Particelle.Columns.Add(New DataColumn With {.ColumnName = "Appezza", .DataType = System.Type.GetType("System.Int32")})
        DTProgrammazione_Particelle.Columns.Add(New DataColumn With {.ColumnName = "Campo_cod", .DataType = System.Type.GetType("System.Int32")})
        DTProgrammazione_Particelle.Columns.Add(New DataColumn With {.ColumnName = "Prov", .DataType = System.Type.GetType("System.String")})
        DTProgrammazione_Particelle.Columns.Add(New DataColumn With {.ColumnName = "Com", .DataType = System.Type.GetType("System.String")})
        DTProgrammazione_Particelle.Columns.Add(New DataColumn With {.ColumnName = "Sezione", .DataType = System.Type.GetType("System.String")})
        DTProgrammazione_Particelle.Columns.Add(New DataColumn With {.ColumnName = "Subalterno", .DataType = System.Type.GetType("System.String")})

    End Sub
    Public Shared Function GetValue(ByVal vettore As String(), ByVal CodiceDaRestituire As String) As String

        Dim pos As Integer = 0
        For Each s In vettore
            If s.TrimEnd(" ").ToLower = CodiceDaRestituire.ToLower Then
                Return vettore(pos + 1).TrimStart(" ").TrimEnd(" ")
            End If
            pos += 1
        Next

        Return Nothing

    End Function


End Class
