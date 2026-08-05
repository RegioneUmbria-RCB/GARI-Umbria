
Imports <xmlns="http://www.agronica.it/grafica/">

Imports Gias2Gias_LIB
Imports AgronicaCoreDataProvider
Imports System.Text
Imports GIAS2GIAS_LOCALE
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaConversioneCartografiaGIAS.Agronica
Imports AgronicaCoreModello

Public Class ConversioneDaListaImprese
    Public Const AGRODATAINIZIO As Date = #1/1/1900#
    Public Const AGRODATAFINE As Date = #12/31/2100#

    Private _LayersImprese As New List(Of String)

    Private _g2g_set As clsOpzioni

    Public Sub VecchiDatiGIAS(
        ByVal GEORiferimento_COD As Integer,
        ByVal TipoOperazione_DB As Integer,
        ByVal DatiLayers As String,
        ByVal ASG_objParametri_Server As AgronicaCoreParametri,
        ByVal ProgressivoGIAS_ORIGINE As Integer,
        Optional ByVal DatiPassaggioSportelloSementi As String = "",
        Optional ByVal PivaReferenteImpresaSementiera As String = ""
    )


        Dim convertHelper As New ConvertiVecchioNuovo
        Dim InterferenzeHelper As New AgronicaCoreGisBIZ.Interferenze

        Dim localDatiPassaggioSportelloSementi As String = ""
        Dim localPivaReferenteImpresaSementiera As String = ""

        _g2g_set = New clsOpzioni()
        _g2g_set.objParametri_Server_GIAS_ORIGINE = ASG_objParametri_Server
        _g2g_set.objParametri_Server_GIAS_DESTINAZIONE = ASG_objParametri_Server

        '_g2g_set.objParametri_Utenti_GIAS_ORIGINE = Session("ASG_objParametri_Utenti")
        '_g2g_set.objParametri_Utenti_GIAS_DESTINAZIONE = Session("ASG_objParametri_Utenti")

        _g2g_set.ProgressivoGIAS_ORIGINE = ProgressivoGIAS_ORIGINE
        _g2g_set.ProgressivoGIAS_DESTINAZIONE = ProgressivoGIAS_ORIGINE


        ''super user
        '_g2g_set.SuperUser_CodFiscale_ORIGINE = AppSettings("PivaSuperUser_ORIGINE")
        '_g2g_set.SuperUser_CodFiscale_DESTINAZIONE = AppSettings("PivaSuperUser_DESTINAZIONE")
        '_g2g_set.SuperUser_Username_ORIGINE = AppSettings("UsernameSuperUser_Origine")
        '_g2g_set.SuperUser_Username_DESTINAZIONE = AppSettings("UsernameSuperUser_Destinazione")

        'importatore
        _g2g_set.Import_CodFiscale_ORIGINE = _g2g_set.objParametri_Server_GIAS_ORIGINE.PivaSuperUser
        _g2g_set.Import_CodFiscale_DESTINAZIONE = _g2g_set.objParametri_Server_GIAS_ORIGINE.PivaSuperUser
        _g2g_set.Import_Username_ORIGINE = _g2g_set.objParametri_Server_GIAS_ORIGINE.SuperUserUsername
        _g2g_set.Import_Username_DESTINAZIONE = _g2g_set.objParametri_Server_GIAS_ORIGINE.SuperUserUsername


        Dim log_G2G As New StringBuilder
        Dim log_Errori As New StringBuilder
        Dim log_Riepilogo As New StringBuilder

        Dim trans As New GIAS2GIAS_LOCALE.GIAS_2_GIAS

        Dim letturaDatiImprese As New GIAS2GIAS_LOCALE.LetturaDatiImpreseXML
        Dim sXmlImprese As String = letturaDatiImprese.GeneraXmlDaImportare(True, _g2g_set.objParametri_Server_GIAS_ORIGINE, xFiltroAggiuntivo:=" and aa.flagimporta_gis='' ", SoloDatiNonImportatiGrafica:=True, includiRicette:=True)

        Dim imprese = XDocument.Parse(sXmlImprese)

        Dim listOfDataImport As List(Of clsImpresa) = LetturaDatiImpreseXML.getImprese(imprese)

        Dim letturaoldGraficaHelper As New Gias2Gias_LIB.Funzioni(_g2g_set.objParametri_Server_GIAS_ORIGINE, _g2g_set.TimeOut_Chiamata_WS)

        'Dim NonImportatiSQL As String = AppSettings("DumpNonImportati")
        'My.Computer.FileSystem.WriteAllText(NonImportatiSQL, "", False)

        Dim ScriviElementiGrafici As New AgronicaCoreGisBIZ.GIS_Entita_W

        Dim i As Integer = 1
        Dim percent As Single = 0
        Dim totale As Single = listOfDataImport.Count



        Dim leggiTrasformazione As New AgronicaCoreGisDAL.GIS_SistemiRiferimentoCartografia_R
        Dim dtLeggiTrasformazione As DataTable = leggiTrasformazione.Leggi(GEORiferimento_COD, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", _g2g_set.objParametri_Server_GIAS_DESTINAZIONE)


        Dim ParametriCartografici As New ParametriCoordinateConverter With {
            .CSFromText = dtLeggiTrasformazione(0)("CSFrom"),
            .CStoText = dtLeggiTrasformazione(0)("CSTo"),
            .CStoGeoText = dtLeggiTrasformazione(0)("CStoGeo"),
            .AgronicaLatOffset = dtLeggiTrasformazione(0)("AgronicaLatOffset"),
            .AgronicaLonOffset = dtLeggiTrasformazione(0)("AgronicaLonOffset"),
            .LibreriaDaUsare = dtLeggiTrasformazione(0)("LibreriaDaUsare")
        }

        For Each impresa In listOfDataImport

            'trans.GIAS_2_GIAS_Completaopzioni(_g2g_set)
            Dim fileContents As String =
                letturaoldGraficaHelper.GetDatiXML_Grafica(
                    TipoOperazione_DB,
                    _g2g_set,
                    impresa.Piva_ORIGINE,
                    impresa.Opzionale_Sa_Cod_Origine,
                    DatiLayers
                )

            Dim output As String

            Dim pivaimpresa As String = ""
            If Not impresa.Piva_ORIGINE.Contains("-") Then
                pivaimpresa = impresa.Piva_ORIGINE
            Else
                If impresa.Piva_ORIGINE.Split("-")(0) = impresa.Piva_ORIGINE.Split("-")(1) Then
                    pivaimpresa = impresa.Piva_ORIGINE.Split("-")(0)
                End If
            End If

            If pivaimpresa <> "" And Not _LayersImprese.Contains(pivaimpresa) Then
                _LayersImprese.Add(pivaimpresa & "|" & impresa.RagioneSociale)
            End If


            If fileContents <> "" Then

                Dim layersDaImportare As String() = DatiLayers.Split(",")



                For Each layerTipoEntita As String In layersDaImportare

                    output = convertHelper.ConvertiXml(fileContents, _g2g_set.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser, impresa.PivaPadre_DESTINAZIONE, impresa.Piva_ORIGINE, impresa.Opzionale_Sa_Cod_Origine, _g2g_set, layerTipoEntita, True, True, 1, ParametriCartografici, _g2g_set.objParametri_Server_GIAS_DESTINAZIONE)


                    Dim gml As XNamespace = "http://www.opengis.net/gml"



                    Dim tmpDoc As XDocument = XDocument.Parse(output)
                    For Each elemento In (
                        From a In tmpDoc.<DatiEntita>.<Entita>
                        Select a).ToList()



                        'per test formato database ...
                        Try

                            'transizione sulla singola chiamata di un entità grafica/GIAS

                            Dim OutputEntitaCod As Integer
                            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, _g2g_set.objParametri_Server_GIAS_DESTINAZIONE)
                            ScriviElementiGrafici.scrivi(elemento.ToString, OutputEntitaCod, _g2g_set.objParametri_Server_GIAS_DESTINAZIONE)


                            Dim xDAtiGias = (From el In elemento.<EntitaGIAS>.<DatoGias> Select el).FirstOrDefault

                            Dim TipoOperazioneDbElementoCorrente As String = elemento.Attribute("TipoOperazioneDB").Value

                            If TipoOperazioneDbElementoCorrente = 2 And OutputEntitaCod = 0 Then
                                OutputEntitaCod = xDAtiGias.<Entita_Cod>.Value
                            End If

                            Dim Data_Riferimento As DateTime = xDAtiGias.<Data_Modifica>.Value

                            Dim Appezza As Integer = xDAtiGias.<Appezza>.Value
                            Dim idReg As Integer = xDAtiGias.<Id_Imp>.Value

                            Dim RecuperaDataInizio As Date = _g2g_set.objParametri_Server_GIAS_DESTINAZIONE.FinestraTemporaleInizio
                            Dim RecuperaDataFine As Date = _g2g_set.objParametri_Server_GIAS_DESTINAZIONE.FinestraTemporaleFine

                            _g2g_set.objParametri_Server_GIAS_DESTINAZIONE.FinestraTemporaleInizio = AGRODATAINIZIO
                            _g2g_set.objParametri_Server_GIAS_DESTINAZIONE.FinestraTemporaleFine = AGRODATAFINE

                            Dim LetturaDatiImpianto As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                            Dim dtLetturaDatiImpianto As DataTable =
                                LetturaDatiImpianto.Leggi(
                                    pivaimpresa, impresa.Opzionale_Sa_Cod_Origine, Appezza, idReg,
                                    enumSelezioneVariabile.Selezione_JoinDescrizioni, "", "",
                                    _g2g_set.objParametri_Server_GIAS_DESTINAZIONE
                                )



                            _g2g_set.objParametri_Server_GIAS_DESTINAZIONE.FinestraTemporaleInizio = RecuperaDataInizio
                            _g2g_set.objParametri_Server_GIAS_DESTINAZIONE.FinestraTemporaleFine = RecuperaDataFine


                            Dim specie As String = 0
                            Dim Tipologia As Integer = 0
                            Dim Data_Inizio As DateTime
                            Dim Data_Fine As DateTime
                            Dim Data_Inizio_Sportello As DateTime
                            Dim Data_Fine_Sportello As DateTime


                            If dtLetturaDatiImpianto.Rows.Count > 0 Then

                                'reset .. 
                                localDatiPassaggioSportelloSementi = ""
                                localPivaReferenteImpresaSementiera = ""

                                'imposto i dati
                                specie = dtLetturaDatiImpianto(0)("veg_cod")
                                Tipologia = dtLetturaDatiImpianto(0)("grva_Cod_veg")
                                Data_Inizio = dtLetturaDatiImpianto(0)("validita_inizio")
                                Data_Fine = dtLetturaDatiImpianto(0)("validita_fine")

                                ' se sto eseguendo l'importazione come super-user allora imposto il dato per il referente sementiero
                                If PivaReferenteImpresaSementiera = "" OrElse PivaReferenteImpresaSementiera = _g2g_set.Import_CodFiscale_DESTINAZIONE Then
                                    localPivaReferenteImpresaSementiera = dtLetturaDatiImpianto(0)("Codice_Fiscale_Tecnico")
                                Else
                                    localPivaReferenteImpresaSementiera = PivaReferenteImpresaSementiera
                                End If

                                'se non viene passato uno sportello leggo i dati da databse

                                'Ricerco con i parametri indicati sull'impianto (specie, date) uno sportello
                                Dim lSportelli As List(Of String) =
                                    InterferenzeHelper.LeggiSportelloAttivoDataSpecieIntervalloData(specie, Data_Riferimento, Data_Riferimento, _g2g_set.objParametri_Server_GIAS_DESTINAZIONE)

                                If lSportelli.Count > 0 Then
                                    localDatiPassaggioSportelloSementi = lSportelli.First
                                End If

                                'se alla fine ho uno sportello è necessario verificare e gestire le interferenze .. 
                                If Not String.IsNullOrEmpty(localDatiPassaggioSportelloSementi) Then

                                    Dim vDataSportello As String() = localDatiPassaggioSportelloSementi.Split("|")

                                    Data_Inizio_Sportello = vDataSportello(1)
                                    Data_Fine_Sportello = vDataSportello(2)


                                    'anche se in teoria i periodi devono coincidere, 
                                    'considero i periodi di validità: se il periodo di validità dell'impianto interseca il periodo di validità dello sportello allora devo considerare in poligono come interferenza
                                    If Data_Inizio < Data_Fine_Sportello And Data_Inizio_Sportello < Data_Fine Then

                                        Dim HiddenPunti As String = (From gg In elemento.<geodata> Select gg).Elements(gml + "Polygon").FirstOrDefault.ToString()
                                        InterferenzeHelper.InterferenzeGestione(TipoOperazioneDbElementoCorrente, OutputEntitaCod, HiddenPunti, localDatiPassaggioSportelloSementi, localPivaReferenteImpresaSementiera, specie, Tipologia, Data_Inizio_Sportello, Data_Fine_Sportello, _g2g_set.objParametri_Server_GIAS_DESTINAZIONE, _g2g_set.objParametri_Utenti_GIAS_DESTINAZIONE)

                                    End If
                                    'fine --se necessario verifico e gestisco le interferenze .. 

                                End If 'se esise uno sportello per l'impianto

                            End If
                            'fine se esiste impianto

                            G2G_Chiusura_Transazione(1)

                        Catch ex As Exception

                            G2G_Chiusura_Transazione(2)

                            'My.Computer.FileSystem.WriteAllText(NonImportatiSQL, head & "<Entita>" & elemento.Elements.FirstOrDefault.ToString & "</Entita>" & tail, True)

                        End Try


                    Next

                Next

            End If



            i += 1
        Next

        'FinalizzaRisultato(NonImportatiSQL)
        'System.Web.HttpContext.Current.Session("conteggio") = "stop"

    End Sub

    Private Sub G2G_Chiusura_Transazione(
                                ByVal Flag_Commit1_Rollback2 As Integer
                                )

        Dim NomeRoutine As String = "G2G_Chiusura_Transazione"

        Try
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(Flag_Commit1_Rollback2, _g2g_set.objParametri_Server_GIAS_DESTINAZIONE)

        Catch ex As Exception
        End Try

    End Sub



End Class
