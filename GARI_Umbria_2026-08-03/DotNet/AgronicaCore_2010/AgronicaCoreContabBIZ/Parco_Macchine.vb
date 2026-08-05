
Imports System.Data
Imports System.Data.OleDb
Imports System.Xml
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate


Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreMetaSchemaDAL
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreContabBIZ.AnagrafeNG
Imports System.Transactions
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreContabDAL
Imports Newtonsoft.Json
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.metaschema
Imports AgronicaCoreModelsSTD.baseClass
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDTOStd.InData.Demetra
Imports AgronicaCoreAnagrafeDAL
Imports DocumentFormat.OpenXml.Office.CustomXsn
Imports AgronicaCoreModelsSTD.exceptions

Public Class Parco_Macchine_R
    Inherits AgronicaCoreDataProvider.LogProvider

    Public Sub Leggi_DettaglioMacchina(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            ByVal Piva As String,
                                            ByVal Mac_Cod As Integer,
                                            ByRef Targa As String,
                                            ByRef Mac_Des As String,
                                            ByRef N_Immatricolazione As String,
                                            ByRef N_Immatricolazione_Rimorchio As String,
                                            ByRef N_Autorizzazione_Trasporto As String,
                                            ByRef Data_Rilascio_Autorizzazione As String,
                                            ByRef PesoTara As Double)

        Dim DT As DataTable
        Dim objContabDAL As New AgronicaCoreContabDAL.Parco_Macchine_R



        DT = objContabDAL.NewCom_ParcoMacchine_Leggi("", "", objParametri,
                                            Piva, Mac_Cod,
                                            False, , , , , , , , )


        If Not IsNothing(DT) AndAlso DT.Rows.Count <> 0 Then

            Targa = DT.Rows(0).Item("Targa")
            Mac_Des = DT.Rows(0).Item("Mac_Des")

            N_Immatricolazione = DT.Rows(0).Item("N_Immatricolazione")
            N_Immatricolazione_Rimorchio = DT.Rows(0).Item("N_Immatricolazione_Rimorchio")
            N_Autorizzazione_Trasporto = DT.Rows(0).Item("N_Autorizzazione_Trasporto")

            If DT.Rows(0).Item("Data_Rilascio_Autorizzazione") = "01/01/1900" Then
                Data_Rilascio_Autorizzazione = ""
            Else
                Data_Rilascio_Autorizzazione = DT.Rows(0).Item("Data_Rilascio_Autorizzazione")
            End If

            PesoTara = DT.Rows(0).Item("Peso")

        End If

    End Sub

    Public Function Leggi_Caratteristiche_Macchina(Class_Code As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        If Class_Code Is Nothing Or Class_Code = "" Then
            Throw New Exception("Class_Code non valorizzato")
        End If

        Dim objMacchine_Dal As New AgronicaCoreContabDAL.Parco_Macchine_R
        Dim DT = objMacchine_Dal.Leggi_Caratteristiche(Class_Code, objParametri)

        Return DT

    End Function

    Public Function Exists_Macchina(Piva As String,
                                   Mac_Cod As Integer,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim ret As Boolean = False
        Try
            Dim objMacchine_Dal As New AgronicaCoreContabDAL.Parco_Macchine_R
            Dim DT = objMacchine_Dal.Leggi(Piva, Mac_Cod, True, "", "", "", "", "", 0, "", True, 0, "", True, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri)
            If DT.Rows.Count = 0 Then
                ret = False
            Else
                ret = True
            End If
        Catch ex As Exception
            ret = False
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Public Function Leggi_Macchina(Piva As String,
                                   Mac_Cod As Integer,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As ParcoMacchine

        Dim m As New ParcoMacchine

        If Mac_Cod = 0 Then
            Throw New Exception("Mac_Cod non valorizzato")
        End If

        Dim objMacchine_Dal As New AgronicaCoreContabDAL.Parco_Macchine_R
        Dim objCostoUnitario_Dal As New Prodotti_Costi_R
        Dim objCaratteristiche_Dal As New Caratteristiche_R
        Dim objGerarchiaMacchine_Dal As New GerarchiaMacchine_R
        Dim DT = objMacchine_Dal.Leggi(Piva, Mac_Cod, True, "", "", "", "", "", 0, "", True, 0, "", True, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri)

        If DT.Rows.Count = 0 Then
            Throw New Exception("Macchina non trovata")
        End If

        m.codice = DT.Rows(0)("Mac_Cod")
        m.partitaIva = DT.Rows(0)("Piva")
        m.centroPK = New CentroAziendale.PK(DT.Rows(0)("Sa_Cod"), DT.Rows(0)("Piva"))

        m.visibilitaPubblica = If(DT.Rows(0)("Sa_Cod") = -1, True, False)
        m.descrizione = DT.Rows(0)("Mac_Des")
        m.marca = New DittaMacchina(DT.Rows(0)("Ditta_Cod")) With {.descrizione = DT.Rows(0)("Ditta_Des")}
        m.modello = DT.Rows(0)("Modello")
        m.finalita = New FinalitaMacchina(DT.Rows(0)("Tipo")) With {.descrizione = ""}
        If DT.Rows(0)("Class_Code").ToString.Split(".").Length = 1 Then
            m.tipo = New AgronicaCoreModelsSTD.metaschema.Macchine(DT.Rows(0)("Class_Code")) With {.descrizione = If(IsDBNull(DT.Rows(0)("tipo_desc")), "", DT.Rows(0)("tipo_desc"))}
            m.dettaglio_1 = New MacchineDettaglio1("") With {.descrizione = ""}
            m.dettaglio_2 = New MacchineDettaglio2("") With {.descrizione = ""}
        ElseIf DT.Rows(0)("Class_Code").ToString.Split(".").Length = 2 Then
            m.tipo = New AgronicaCoreModelsSTD.metaschema.Macchine(DT.Rows(0)("Class_Code").ToString.Split(".")(0)) With {.descrizione = If(IsDBNull(DT.Rows(0)("tipo_desc")), "", DT.Rows(0)("tipo_desc"))}
            m.dettaglio_1 = New MacchineDettaglio1(DT.Rows(0)("Class_Code").ToString.Split(".")(1)) With {.descrizione = If(IsDBNull(DT.Rows(0)("dettaglio_1_desc")), "", DT.Rows(0)("dettaglio_1_desc"))}
            m.dettaglio_2 = New MacchineDettaglio2("") With {.descrizione = ""}
        ElseIf DT.Rows(0)("Class_Code").ToString.Split(".").Length = 3 Then
            m.tipo = New AgronicaCoreModelsSTD.metaschema.Macchine(DT.Rows(0)("Class_Code").ToString.Split(".")(0)) With {.descrizione = If(IsDBNull(DT.Rows(0)("tipo_desc")), "", DT.Rows(0)("tipo_desc"))}
            m.dettaglio_1 = New MacchineDettaglio1(DT.Rows(0)("Class_Code").ToString.Split(".")(1)) With {.descrizione = If(IsDBNull(DT.Rows(0)("dettaglio_1_desc")), "", DT.Rows(0)("dettaglio_1_desc"))}
            m.dettaglio_2 = New MacchineDettaglio2(DT.Rows(0)("Class_Code").ToString.Split(".")(2)) With {.descrizione = If(IsDBNull(DT.Rows(0)("Class_Desc")), "", DT.Rows(0)("Class_Desc"))}
        End If
        m.codice_stringa = If(DT.Rows(0)("Codice") = "", If(DT.Rows(0)("Mac_Cod_Origine") = 0, "", DT.Rows(0)("Mac_Cod_Origine").ToString()), DT.Rows(0)("Codice"))
        m.codiceOrigine = DT.Rows(0)("Mac_Cod_Origine")
        m.validita = New IntervalloTemporale(
            If(IsDBNull(DT.Rows(0)("Validita_Inizio")), AGRODATAINIZIO, DT.Rows(0)("Validita_Inizio")),
            If(IsDBNull(DT.Rows(0)("Validita_Fine")), AGRODATAFINE, DT.Rows(0)("Validita_Fine"))
        )

        m.Data_Carico = DT.Rows(0)("Data_Carico")
        m.Data_Scarico = DT.Rows(0)("Data_Scarico")

        m.Username_Creazione = DT.Rows(0)("username_creazione")
        m.Username_Modifica = DT.Rows(0)("username_modifica")

        m.titolo_Possesso = New TitoloDiPossesso(DT.Rows(0)("TitoloPossesso")) With {.descrizione = ""}
        m.proprietario = DT.Rows(0)("Denominazione_Proprietario")
        m.CUAA_Proprietario = DT.Rows(0)("CUAA_Proprietario")
        m.targa = DT.Rows(0)("Targa")
        m.tipo_Targa = New TipoTarga(DT.Rows(0)("Tipo_Targa_Cod")) With {.descrizione = ""}
        m.telaio = DT.Rows(0)("Telaio")
        m.n_Immatricolazione = DT.Rows(0)("N_Immatricolazione")
        m.data_Immatricolazione = DT.Rows(0)("Data_Immatricolazione")
        m.n_Immatricolazione_Rimorchio = DT.Rows(0)("N_Immatricolazione_Rimorchio")
        m.N_Autorizzazione_Trasporto = DT.Rows(0)("N_Autorizzazione_Trasporto")
        m.data_Rilascio_Autorizzazione = DT.Rows(0)("Data_Rilascio_Autorizzazione")
        m.data_Ultima_Revisione = DT.Rows(0)("Ultima_Revisione")
        m.data_Ultima_Manutenzione = DT.Rows(0)("Ultima_Manutenzione")
        m.alimentazione = New Carburante(DT.Rows(0)("Alimentazione_Cod")) With {.descrizione = ""}
        m.numero_certificato = DT.Rows(0)("numero_certificato")
        m.taratura_Ugello = 0
        If Not IsDBNull(DT.Rows(0)("Taratura_Ugello")) Then
            m.taratura_Ugello = DT.Rows(0)("Taratura_Ugello")
        End If
        m.visibileControlloGestione = False
        If Not IsDBNull(DT.Rows(0)("Visibile_ctrl_gestione")) Then
            m.visibileControlloGestione = If(DT.Rows(0)("Visibile_ctrl_gestione") = 0, False, True)
        End If

        If Not IsDBNull(DT.Rows(0)("VIN")) Then
            m.VIN = DT.Rows(0)("VIN")
        End If
        If Not IsDBNull(DT.Rows(0)("BTM_Serial")) Then
            m.BTM_Serial = DT.Rows(0)("BTM_Serial")
        End If

        If Not IsDBNull(DT.Rows(0)("ExternalAPIKey")) Then
            m.ExternalAPIKey = DT.Rows(0)("ExternalAPIKey")
        End If

        If Not IsDBNull(DT.Rows(0)("HubIoT_PlatformDestination")) Then
            m.HubIoT_PlatformDestination = New BaseCodeDescr(DT.Rows(0)("HubIoT_PlatformDestination"), DT.Rows(0)("HubIoT_PlatformDestinationDes"))
        End If

        If CStr(DT.Rows(0)("Cod_Contatto")) <> "" Then
            Dim objContattiR As New AgronicaCoreAnagrafeDAL.Contatti_R
            Dim dtContatto As DataTable = objContattiR.Leggi("", CStr(DT.Rows(0)("Cod_Contatto")), 0, 0, False, False, 0, 0, False, 0, -99, 0, "", True, 0, 0, 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, False, "", "", objParametri)
            If dtContatto.Rows.Count = 0 Then
                dtContatto = objContattiR.Leggi("", CStr(DT.Rows(0)("Cod_Contatto")), 0, 0, True, False, 0, 0, False, 0, -99, 0, "", True, 0, 0, 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, False, "", "", objParametri)
            End If

            If dtContatto.Rows.Count > 0 Then
                Dim piva_contatto = dtContatto.Rows(0)("Piva")
                m.contatto = New AgronicaCoreModelsSTD.anagrafiche.Contatto() With {
                    .primaryKey = New AgronicaCoreModelsSTD.anagrafiche.Contatto.PK(piva_contatto, CStr(DT.Rows(0)("Cod_Contatto"))),
                    .cognome = dtContatto.Rows(0)("Cognome"),
                    .nome = dtContatto.Rows(0)("Nome"),
                    .ragione_Sociale = dtContatto.Rows(0)("Rag_Soc")
                }
            Else
                m.contatto = New AgronicaCoreModelsSTD.anagrafiche.Contatto() With {
                    .primaryKey = New AgronicaCoreModelsSTD.anagrafiche.Contatto.PK("", ""),
                    .cognome = "",
                    .nome = "",
                    .ragione_Sociale = ""
                }
            End If
        Else
            m.contatto = New AgronicaCoreModelsSTD.anagrafiche.Contatto() With {
                .primaryKey = New AgronicaCoreModelsSTD.anagrafiche.Contatto.PK("", ""),
                .cognome = "",
                .nome = "",
                .ragione_Sociale = ""
            }
        End If

        m.data_Ultima_Taratura = If(IsDBNull(DT.Rows(0)("Validita_Taratura_Inizio")), AGRODATAINIZIO, DT.Rows(0)("Validita_Taratura_Inizio"))
        m.scadenza_Taratura = If(IsDBNull(DT.Rows(0)("Validita_Taratura_Fine")), AGRODATAFINE, DT.Rows(0)("Validita_Taratura_Fine"))
        'If Not IsDBNull(DT.Rows(0)("Validita_Taratura_Inizio")) Then
        '    m.data_Ultima_Taratura = DT.Rows(0)("Validita_Taratura_Inizio")
        'End If
        'If Not IsDBNull(DT.Rows(0)("Validita_Taratura_Fine")) Then
        '    m.scadenza_Taratura = DT.Rows(0)("Validita_Taratura_Fine")
        'End If
        'm.data_Ultima_Taratura = AGRODATAINIZIO
        'm.scadenza_Taratura = AGRODATAFINE

        m.stato_Utilizzo = DT.Rows(0)("Stato_Utilizzo")
        m.potenza = DT.Rows(0)("Potenza")
        m.unita_Misura = New UnitaDiMisura(DT.Rows(0)("Potenza_Udm_Cod")) With {.descrizione = ""}
        m.note = DT.Rows(0)("Note")
        If Not IsDBNull(DT.Rows(0)("Img_Thumbnail_FileName")) And Not IsDBNull(DT.Rows(0)("Img_Thumbnail_Extension")) And Not IsDBNull(DT.Rows(0)("Img_Thumbnail")) Then
            m.immaginePiccola = New AgronicaCoreModelsSTD.Utility.Immagine() With {.nome = DT.Rows(0)("Img_Thumbnail_FileName"), .estensione = DT.Rows(0)("Img_Thumbnail_Extension"), .immagine = Convert.ToBase64String(DT.Rows(0)("Img_Thumbnail"))}
        End If
        If Not IsDBNull(DT.Rows(0)("Img_Large_FileName")) And Not IsDBNull(DT.Rows(0)("Img_Large_Extension")) And Not IsDBNull(DT.Rows(0)("Img_Large")) Then
            m.immagineGrande = New AgronicaCoreModelsSTD.Utility.Immagine() With {.nome = DT.Rows(0)("Img_Large_FileName"), .estensione = DT.Rows(0)("Img_Large_Extension"), .immagine = Convert.ToBase64String(DT.Rows(0)("Img_Large"))}
        End If

        If Not IsDBNull(DT.Rows(0)("Agea_Cod")) Then
            m.ageaCod = New AgronicaCoreModelsSTD.metaschema.MacchineCodificaAgea(DT.Rows(0)("Agea_Cod"))
            m.ageaCod.descrizione = If(IsDBNull(DT.Rows(0)("AGEA_Des")), "", DT.Rows(0)("AGEA_Des"))
        End If

        If Not IsDBNull(DT.Rows(0)("Portata")) Then
            m.portata = DT.Rows(0)("Portata")
        End If

        If Not IsDBNull(DT.Rows(0)("Efficienza")) Then
            m.efficienza = DT.Rows(0)("Efficienza")
        End If

        If Not IsDBNull(DT.Rows(0)("IMP_COD")) Then
            m.codice_impianto = DT.Rows(0)("IMP_COD")
        End If

        'stazione Meteo
        If Not IsDBNull(DT.Rows(0)("Distinta_Installazione")) Then
            m.Distinta_Installazione = DT.Rows(0)("Distinta_Installazione")
        End If

        If Not IsDBNull(DT.Rows(0)("Contratto_Installazione")) Then
            m.Contratto_Installazione = DT.Rows(0)("Contratto_Installazione")
        End If

        If Not IsDBNull(DT.Rows(0)("Tipologia_Installazione")) Then
            m.Tipologia_Installazione = DT.Rows(0)("Tipologia_Installazione")
        End If

        If Not IsDBNull(DT.Rows(0)("Data_Inizio_Installazione")) Then
            m.Data_Inizio_Installazione = DT.Rows(0)("Data_Inizio_Installazione")
        End If

        If Not IsDBNull(DT.Rows(0)("Data_Fine_Installazione")) Then
            m.Data_Fine_Installazione = DT.Rows(0)("Data_Fine_Installazione")
        End If

        If Not IsDBNull(DT.Rows(0)("Stato_Installazione")) Then
            m.Stato_Installazione = DT.Rows(0)("Stato_Installazione")
        End If

        If Not IsDBNull(DT.Rows(0)("Provincia_Istat_Installazione")) Then
            m.Provincia_Istat_Installazione = DT.Rows(0)("Provincia_Istat_Installazione")
        End If

        If Not IsDBNull(DT.Rows(0)("Comune_Istat_Installazione")) Then
            m.Comune_Istat_Installazione = DT.Rows(0)("Comune_Istat_Installazione")
        End If

        If Not IsDBNull(DT.Rows(0)("Indirizzo_Installazione")) Then
            m.Indirizzo_Installazione = DT.Rows(0)("Indirizzo_Installazione")
        End If

        If Not IsDBNull(DT.Rows(0)("Latitudine_Installazione")) Then
            m.Latitudine_Installazione = DT.Rows(0)("Latitudine_Installazione")
        End If

        If Not IsDBNull(DT.Rows(0)("Longitudine_Installazione")) Then
            m.Longitudine_Installazione = DT.Rows(0)("Longitudine_Installazione")
        End If

        Dim DT_CostoUitario = objCostoUnitario_Dal.Leggi_Macchine(Piva, m.codice,
                                                enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                "", "", objParametri)
        m.costi = New List(Of CostoUnitario)()

        Dim index = 0
        For Each row As DataRow In DT_CostoUitario.Rows
            Dim costo As New CostoUnitario(row("ID"))
            costo.unitaDiMisura = New UnitaDiMisura()
            costo.unitaDiMisura.descrizione = row("Udm_Des")
            costo.unitaDiMisura.codice = row("Mezzo")
            costo.prezzo = row("Prezzo_Unitario")
            costo.validita = New IntervalloTemporale(
                If(IsDBNull(DT_CostoUitario.Rows(index)("Validita_Inizio")), AGRODATAINIZIO, DT_CostoUitario.Rows(index)("Validita_Inizio")),
                If(IsDBNull(DT_CostoUitario.Rows(index)("Validita_Fine")), AGRODATAFINE, DT_CostoUitario.Rows(index)("Validita_Fine")))
            m.costi.Add(costo)
            index += 1
        Next

        Dim DT_Caratteristiche As New DataTable

        'If (m.codice <> 0 And m.codice <> Nothing) Then

        DT_Caratteristiche = objCaratteristiche_Dal.Leggi(m.codice, "",
                                                "", "", objParametri)
        m.caratteristiche = New List(Of ParcoMacchineCaratteristiche)()

        index = 0
        For Each row As DataRow In DT_Caratteristiche.Rows
            Dim caratteristica As New ParcoMacchineCaratteristiche()
            caratteristica.codice = row("ID")
            caratteristica.caratteristica = New CaratteristicaMacchina(row("Mac_Car_Cod"))
            caratteristica.caratteristica.descrizione = objMacchine_Dal.Leggi_Caratteristica(row("Mac_Car_Cod"), objParametri)
            caratteristica.valore = row("Valore")
            caratteristica.validita_inizio = If(IsDBNull(DT_Caratteristiche.Rows(index)("Validita_Inizio")), AGRODATAINIZIO, DT_Caratteristiche.Rows(index)("Validita_Inizio"))
            caratteristica.validita_fine = If(IsDBNull(DT_Caratteristiche.Rows(index)("Validita_Fine")), AGRODATAFINE, DT_Caratteristiche.Rows(index)("Validita_Fine"))
            m.caratteristiche.Add(caratteristica)
            index += 1
        Next

        Dim DT_GerarchiaMacchine As New DataTable

        'If (m.codice <> 0 And m.codice <> Nothing) Then

        DT_GerarchiaMacchine = objGerarchiaMacchine_Dal.Leggi(m.codice, 0,
                                                "", "", objParametri)
        m.gerarchiaPadre = New MacchinaGerarchia()
        m.gerarchiaFigli = New List(Of MacchinaGerarchia)()

        index = 0
        For Each row As DataRow In DT_GerarchiaMacchine.Rows
            Dim gerarchia As New MacchinaGerarchia()
            gerarchia.ID = row("ID")
            gerarchia.macchina = Leggi_Macchina(Piva, CInt(row("Mac_Cod_Figlio")), objParametri)
            gerarchia.legame = New BaseCodeDescr(row("Tipo_Legame"), "")
            gerarchia.desclegame = row("Descr_Legame")
            gerarchia.udm = New UnitaDiMisura(row("Udm_Cod"))
            gerarchia.qta = row("Qta")
            gerarchia.validita_inizio = If(IsDBNull(DT_GerarchiaMacchine.Rows(index)("Validita_Inizio")), AGRODATAINIZIO, DT_GerarchiaMacchine.Rows(index)("Validita_Inizio"))
            gerarchia.validita_fine = If(IsDBNull(DT_GerarchiaMacchine.Rows(index)("Validita_Fine")), AGRODATAFINE, DT_GerarchiaMacchine.Rows(index)("Validita_Fine"))
            m.gerarchiaFigli.Add(gerarchia)
            index += 1
        Next

        'leggiamo i ratei tempo associati alla macchina, se presenti
        Dim DT_RateiTempoMacchina As New DataTable

        Dim r_RateiTempoDAL = New AgronicaCoreContabDAL.Parco_Macchine_R

        DT_RateiTempoMacchina = r_RateiTempoDAL.Leggi_RateiTempo_Macchina(m.codice, objParametri)

        m.rateiTempo = New List(Of RateoTempo)()

        For Each row As DataRow In DT_RateiTempoMacchina.Rows
            Dim rateo As New RateoTempo
            rateo.Rateo_Cod = row("Rateo_Cod")
            rateo.DataInizio = row("DataInizio")
            rateo.DataFine = row("DataFine")
            rateo.OraInizio = row("OraInizio")
            rateo.OraFine = row("OraFine")
            rateo.Rotazione = row("Rotazione")
            rateo.ValiditaInizio = row("Validita_Inizio")
            rateo.ValiditaFine = row("Validita_Fine")

            m.rateiTempo.Add(rateo)
        Next

        Return m
    End Function

    Public Function Leggi_MacchinaCaratteristiche_Da_ChiaveAPI(Codice As String,
                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Dictionary(Of String, String)
        Dim r As New Dictionary(Of String, String)
        Dim MessaggioErrore As String = ""
        Dim NomeRoutine As String = "Parco_Macchine_R.Leggi_Macchina_Da_Codice()"

        If Codice = "" Then
            Throw New Exception("Codice non valorizzato")
        End If

        Dim objMacchine_Dal As New AgronicaCoreContabDAL.Parco_Macchine_R
        Dim objMaccXCarat_Dal As New AgronicaCoreContabDAL.Parco_MacchinexCaratteristiche_R
        Dim objGerarchiaMacchine_Dal As New AgronicaCoreContabDAL.GerarchiaMacchine_R
        Try
            Dim parco_macchine = objMacchine_Dal.Leggi_MacchinaXChiaveAPI(Codice, "", "", objParametri)
            Dim caratteristiche = objMaccXCarat_Dal.Leggi_MacchineCaratteristicheXChiaveAPI(Codice, "", "", objParametri)
            Dim dtImg = objMacchine_Dal.Leggi_Macchina_Image_da_ChiaveAPI(Codice, "", "", objParametri)
            Dim dtGerarchia = objGerarchiaMacchine_Dal.LeggiConDettagliPadreFiglio(0, parco_macchine.Rows(0)("Mac_Cod"), "f.Class_Code='05.07'", "", objParametri)

            r.Add("Fornitore", IIf(parco_macchine.Rows(0)("Fornitore") Is DBNull.Value, "", parco_macchine.Rows(0)("Fornitore")))
            r.Add("Modello", IIf(parco_macchine.Rows(0)("Modello") Is DBNull.Value, "", parco_macchine.Rows(0)("Modello")))
            r.Add("Matricola", IIf(parco_macchine.Rows(0)("Matricola") Is DBNull.Value, "", parco_macchine.Rows(0)("Matricola")))
            r.Add("Alimentazione", IIf(parco_macchine.Rows(0)("Alimentazione") Is DBNull.Value, "", parco_macchine.Rows(0)("Alimentazione")))
            For Each car In caratteristiche.Rows
                r.Add(car("Mac_Car_Des"), car("Valore"))
            Next

            If Not r.ContainsKey("Comizio Irriguo") Then
                If dtGerarchia.Rows.Count > 0 Then
                    r.Add("Comizio Irriguo", IIf(dtGerarchia.Rows(0)("Codice_Padre") Is DBNull.Value, "", dtGerarchia.Rows(0)("Codice_Padre")))
                Else
                    r.Add("Comizio Irriguo", "not found")
                End If
            End If

            If dtImg.Rows(0)("Img_Thumbnail") IsNot DBNull.Value Then
                r.Add("Image", Convert.ToBase64String(dtImg.Rows(0)("Img_Thumbnail")))
                Return r
            End If

            If dtImg.Rows(0)("Img_Thumbnail_Filename") IsNot DBNull.Value Then
                Dim p = dtImg.Rows(0)("Img_Thumbnail_Filename") + "." + dtImg.Rows(0)("Img_Thumbnail_Extension")
                If System.IO.File.Exists(p) Then
                    r.Add("Image", Convert.ToBase64String(IO.File.ReadAllBytes(p)))
                End If
                Return r
            End If

            If dtImg.Rows(0)("Img_Large") IsNot DBNull.Value Then
                r.Add("Image", Convert.ToBase64String(dtImg.Rows(0)("Img_Large")))
                Return r
            End If

            If dtImg.Rows(0)("Img_Large_Filename") IsNot DBNull.Value Then
                Dim p = dtImg.Rows(0)("Img_Large_Filename") + "." + dtImg.Rows(0)("Img_Large_Extension")
                If System.IO.File.Exists(p) Then
                    r.Add("Image", Convert.ToBase64String(IO.File.ReadAllBytes(p)))
                End If
                Return r
            End If

        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            r = Nothing
        End Try

        Return r
    End Function

    Public Function ReadMachinesRegistryNg(parametriAgenda As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG,
                                           objParamteriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                           Optional orderBy As String = "") As DataTable

        Dim objMacchineDal As New AgronicaCoreContabDAL.Parco_Macchine_R
        Return objMacchineDal.leggi_x_anagrafica(parametriAgenda.Piva, String.Empty, orderBy, objParamteriServer)
    End Function

    Public Function ReadMachinesByClassCode(params As InData.Anagrafica.MachinesXTypeReadParams,
                                            objParamteriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            Optional orderBy As String = "") As DataTable

        Dim objMacchineDal As New AgronicaCoreContabDAL.Parco_Macchine_R
        Dim classCode As String = String.Empty
        Dim filtroAggiuntivo As String = String.Empty

        ' concatenate parameters to build the classCode
        If Not String.IsNullOrEmpty(params.type.Trim) Then
            classCode = $"{params.type.Trim}"
            If Not (String.IsNullOrEmpty(params.det1.Trim)) Then
                classCode = $"{classCode}.{params.det1.Trim}"
                If Not (String.IsNullOrEmpty(params.det2.Trim)) Then
                    classCode = $"{classCode}.{params.det2.Trim}"
                End If
            End If
        Else
            Throw New Exception("ClassCode non valido")
        End If

        If Not String.IsNullOrEmpty(classCode) Then
            filtroAggiuntivo = ($"Parco_Macchine.Class_Code LIKE '{classCode.Trim}%'")
        End If

        Return objMacchineDal.leggi_x_anagrafica(params.parametriAgenda.Piva, filtroAggiuntivo, orderBy, objParamteriServer)
    End Function

    Public Function ReadMachineLight(Mac_Cod As Integer,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As ParcoMacchine

        If Mac_Cod = 0 Then
            Throw New Exception("Mac_Cod non valorizzato")
        End If

        Dim m As New ParcoMacchine
        Dim dalR As New AgronicaCoreContabDAL.Parco_Macchine_R

        Dim dt = dalR.ParcoMacchine_Leggi(
            "",
            Mac_Cod,
            False, "", "", "", "", "", 0, "", False, 0, "", False, AGRODATAINIZIO, AGRODATAFINE,
            enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
            "", "",
            objParametri
            )

        If dt.Rows.Count = 1 Then
            Dim row = dt.Rows(0)
            m.codice = row("Mac_Cod")
            m.partitaIva = row("Piva")
            m.centroPK.codice = row("Sa_Cod")
            m.centroPK.partitaIva = row("Piva")

            If dt.Rows(0)("Class_Code").ToString.Split(".").Length = 1 Then
                m.tipo = New AgronicaCoreModelsSTD.metaschema.Macchine(dt.Rows(0)("Class_Code")) With {.descrizione = If(IsDBNull(dt.Rows(0)("tipo_desc")), "", dt.Rows(0)("tipo_desc"))}
                m.dettaglio_1 = New MacchineDettaglio1("") With {.descrizione = ""}
                m.dettaglio_2 = New MacchineDettaglio2("") With {.descrizione = ""}
            ElseIf dt.Rows(0)("Class_Code").ToString.Split(".").Length = 2 Then
                m.tipo = New AgronicaCoreModelsSTD.metaschema.Macchine(dt.Rows(0)("Class_Code").ToString.Split(".")(0)) With {.descrizione = If(IsDBNull(dt.Rows(0)("tipo_desc")), "", dt.Rows(0)("tipo_desc"))}
                m.dettaglio_1 = New MacchineDettaglio1(dt.Rows(0)("Class_Code").ToString.Split(".")(1)) With {.descrizione = If(IsDBNull(dt.Rows(0)("dettaglio_1_desc")), "", dt.Rows(0)("dettaglio_1_desc"))}
                m.dettaglio_2 = New MacchineDettaglio2("") With {.descrizione = ""}
            ElseIf dt.Rows(0)("Class_Code").ToString.Split(".").Length = 3 Then
                m.tipo = New AgronicaCoreModelsSTD.metaschema.Macchine(dt.Rows(0)("Class_Code").ToString.Split(".")(0)) With {.descrizione = If(IsDBNull(dt.Rows(0)("tipo_desc")), "", dt.Rows(0)("tipo_desc"))}
                m.dettaglio_1 = New MacchineDettaglio1(dt.Rows(0)("Class_Code").ToString.Split(".")(1)) With {.descrizione = If(IsDBNull(dt.Rows(0)("dettaglio_1_desc")), "", dt.Rows(0)("dettaglio_1_desc"))}
                m.dettaglio_2 = New MacchineDettaglio2(dt.Rows(0)("Class_Code").ToString.Split(".")(2)) With {.descrizione = If(IsDBNull(dt.Rows(0)("Class_Desc")), "", dt.Rows(0)("Class_Desc"))}
            End If

            m.descrizione = row("Mac_Des")

            m.validita = New IntervalloTemporale(
                If(IsDBNull(dt.Rows(0)("Validita_Inizio")), AGRODATAINIZIO, dt.Rows(0)("Validita_Inizio")),
                If(IsDBNull(dt.Rows(0)("Validita_Fine")), AGRODATAFINE, dt.Rows(0)("Validita_Fine"))
                )
        Else
            Throw New GiasException(AgronicaCoreDataProvider.My.Resources.Gias.MachineNotFound)
        End If

        Return m
    End Function
End Class

Public Class Parco_Macchine_W
    Inherits AgronicaCoreDataProvider.LogProvider

#Region "Costruttori"

    Public Sub New()
        Provider = Globalization.CultureInfo.InvariantCulture
        Format = "yyyyMMdd"
        ValiditaInizio = Date.ParseExact("19000101", Format, Provider)
        ValiditaFine = Date.ParseExact("21001231", Format, Provider)
    End Sub

#End Region

#Region "Properties"
    Private _format As String
    Public Shadows Property Format() As String
        Get
            Return _format
        End Get
        Set
            _format = Value
        End Set
    End Property

    Private _provider As Globalization.CultureInfo
    Public Shadows Property Provider() As Globalization.CultureInfo
        Get
            Return _provider
        End Get
        Set
            _provider = Value
        End Set
    End Property

    Private _validitaInizio As Date
    Public Shadows Property ValiditaInizio() As Date
        Get
            Return _validitaInizio
        End Get
        Set
            _validitaInizio = Value
        End Set
    End Property

    Private _validitaFine As Date
    Public Shadows Property ValiditaFine() As Date
        Get
            Return _validitaFine
        End Get
        Set
            _validitaFine = Value
        End Set
    End Property
#End Region

    Public Function Aggiorna_Parco_Macchine(
            ByVal piva As String,
            ByRef righeInseriteArray As JArray,
            ByRef righeModificateArray As JArray,
            ByRef righeCancellateArray As JArray,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As String


        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty

        Dim NomeRoutine As String = "Parco_Macchine_W.Aggiorna_Parco_Macchine_W()"

        Try
            Dim campConf_R As New AgronicaCoreContabDAL.Parco_Macchine_R

            Dim curParco_Macchine As New Parco_Macchine

            AgronicaCoreUtility.DataOra.JarrayAggiustaDate(righeInseriteArray)
            AgronicaCoreUtility.DataOra.JarrayAggiustaDate(righeModificateArray)

            Dim EFArrayToInsert As New ArrayList
            Dim EFArrayToUpdate As New ArrayList
            Dim EFArrayToDelete As New ArrayList
            For Each obj As JObject In righeInseriteArray

                curParco_Macchine = New Parco_Macchine

                curParco_Macchine.Piva = piva
                curParco_Macchine.Mac_Cod = 0

                If Not String.IsNullOrEmpty(obj("Sa_Cod")) Then
                    curParco_Macchine.Sa_Cod = obj("Sa_Cod")
                Else
                    curParco_Macchine.Sa_Cod = 0
                End If


                If Not String.IsNullOrEmpty(obj("class_code")) Then
                    curParco_Macchine.Class_Code = obj("class_code")
                Else
                    curParco_Macchine.Class_Code = ""
                End If

                If Not String.IsNullOrEmpty(obj("mac_des")) Then
                    curParco_Macchine.Mac_Des = obj("mac_des")
                Else
                    curParco_Macchine.Mac_Des = obj("Modello")
                End If

                If Not String.IsNullOrEmpty(obj("Costo_Acquisto")) Then
                    curParco_Macchine.Costo_Acquisto = CDbl(obj("Costo_Acquisto"))
                Else
                    curParco_Macchine.Costo_Acquisto = 0
                End If

                If Not String.IsNullOrEmpty(obj("Targa")) Then
                    curParco_Macchine.Targa = obj("Targa")
                Else
                    curParco_Macchine.Targa = ""
                End If

                If Not String.IsNullOrEmpty(obj("Telaio")) Then
                    curParco_Macchine.Telaio = obj("Telaio")
                Else
                    curParco_Macchine.Telaio = ""
                End If

                If Not String.IsNullOrEmpty(obj("Ditta_Cod")) Then
                    curParco_Macchine.Ditta_Cod = obj("Ditta_Cod")
                Else
                    curParco_Macchine.Ditta_Cod = 0
                End If

                If Not String.IsNullOrEmpty(obj("Modello")) Then
                    curParco_Macchine.Modello = obj("Modello")
                Else
                    curParco_Macchine.Modello = ""
                End If

                If Not String.IsNullOrEmpty(obj("Potenza")) Then
                    curParco_Macchine.Potenza = obj("Potenza")
                Else
                    curParco_Macchine.Potenza = ""
                End If

                If Not String.IsNullOrEmpty(obj("Ammortamento")) Then
                    curParco_Macchine.Ammortamento = obj("Ammortamento")
                Else
                    curParco_Macchine.Ammortamento = 0
                End If

                If Not String.IsNullOrEmpty(obj("Ammortizzato")) Then
                    curParco_Macchine.Ammortizzato = obj("Ammortizzato")
                Else
                    curParco_Macchine.Ammortizzato = 0
                End If

                If Not String.IsNullOrEmpty(obj("Data_Immatricolazione")) Then
                    curParco_Macchine.Data_Immatricolazione = obj("Data_Immatricolazione")
                Else
                    curParco_Macchine.Data_Immatricolazione = AGRODATAINIZIO
                End If

                If Not String.IsNullOrEmpty(obj("Ultima_Manutenzione")) Then
                    curParco_Macchine.Ultima_Manutenzione = AgronicaCoreUtility.DataOra.DataStringaJSON_to_ISO8601(obj("Ultima_Manutenzione"))
                Else
                    curParco_Macchine.Ultima_Manutenzione = AGRODATAINIZIO
                End If

                If Not String.IsNullOrEmpty(obj("Ultima_Revisione")) Then
                    curParco_Macchine.Ultima_Revisione = AgronicaCoreUtility.DataOra.DataStringaJSON_to_ISO8601(obj("Ultima_Revisione"))
                Else
                    curParco_Macchine.Ultima_Revisione = AGRODATAINIZIO
                End If

                If Not String.IsNullOrEmpty(obj("Stato_Utilizzo")) Then
                    curParco_Macchine.Stato_Utilizzo = obj("Stato_Utilizzo")
                Else
                    curParco_Macchine.Stato_Utilizzo = ""
                End If

                If Not String.IsNullOrEmpty(obj("Note")) Then
                    curParco_Macchine.Note = obj("Note")
                Else
                    curParco_Macchine.Note = ""
                End If

                If Not String.IsNullOrEmpty(obj("Tipo")) Then
                    curParco_Macchine.Tipo = obj("Tipo")
                Else
                    curParco_Macchine.Tipo = 0
                End If

                If Not String.IsNullOrEmpty(obj("N_Immatricolazione")) Then
                    curParco_Macchine.N_Immatricolazione = obj("N_Immatricolazione")
                Else
                    curParco_Macchine.N_Immatricolazione = ""
                End If

                If Not String.IsNullOrEmpty(obj("N_Immatricolazione_Rimorchio")) Then
                    curParco_Macchine.N_Immatricolazione_Rimorchio = obj("N_Immatricolazione_Rimorchio")
                Else
                    curParco_Macchine.N_Immatricolazione_Rimorchio = ""
                End If

                If Not String.IsNullOrEmpty(obj("N_Autorizzazione_Trasporto")) Then
                    curParco_Macchine.N_Autorizzazione_Trasporto = obj("N_Autorizzazione_Trasporto")
                Else
                    curParco_Macchine.N_Autorizzazione_Trasporto = ""
                End If

                If Not String.IsNullOrEmpty(obj("Data_Rilascio_Autorizzazione")) Then
                    curParco_Macchine.Data_Rilascio_Autorizzazione = AgronicaCoreUtility.DataOra.DataStringaJSON_to_ISO8601(obj("Data_Rilascio_Autorizzazione"))
                Else
                    curParco_Macchine.Data_Rilascio_Autorizzazione = AGRODATAINIZIO
                End If

                If Not String.IsNullOrEmpty(obj("Peso")) Then
                    curParco_Macchine.Peso = obj("Peso")
                Else
                    curParco_Macchine.Peso = 0
                End If


                If Not String.IsNullOrEmpty(obj("Mac_Cod_Origine")) Then
                    curParco_Macchine.Mac_Cod_Origine = obj("Mac_Cod_Origine")
                Else
                    curParco_Macchine.Mac_Cod_Origine = 0
                End If


                If Not String.IsNullOrEmpty(obj("Piva_SuperUser_Origine")) Then
                    curParco_Macchine.Piva_SuperUser_Origine = obj("Piva_SuperUser_Origine")
                Else
                    curParco_Macchine.Piva_SuperUser_Origine = ""
                End If


                If Not String.IsNullOrEmpty(obj("ChkDefault")) Then
                    curParco_Macchine.ChkDefault = obj("ChkDefault")
                Else
                    curParco_Macchine.ChkDefault = 0
                End If


                If Not String.IsNullOrEmpty(obj("Portata_Max")) Then
                    curParco_Macchine.Portata_Max = obj("Portata_Max")
                Else
                    curParco_Macchine.Portata_Max = 0
                End If


                If Not String.IsNullOrEmpty(obj("Cod_Contatto")) Then
                    curParco_Macchine.Cod_Contatto = obj("Cod_Contatto")
                Else
                    curParco_Macchine.Cod_Contatto = ""
                End If


                If Not String.IsNullOrEmpty(obj("CUAA_Proprietario")) Then
                    curParco_Macchine.CUAA_Proprietario = obj("CUAA_Proprietario")
                Else
                    curParco_Macchine.CUAA_Proprietario = ""
                End If


                If Not String.IsNullOrEmpty(obj("Denominazione_Proprietario")) Then
                    curParco_Macchine.Denominazione_Proprietario = obj("Denominazione_Proprietario")
                Else
                    curParco_Macchine.Denominazione_Proprietario = ""
                End If


                If Not String.IsNullOrEmpty(obj("Alimentazione_Cod")) Then
                    curParco_Macchine.Alimentazione_Cod = obj("Alimentazione_Cod")
                Else
                    curParco_Macchine.Alimentazione_Cod = 0
                End If


                If Not String.IsNullOrEmpty(obj("Potenza_Udm_Cod")) Then
                    curParco_Macchine.Potenza_Udm_Cod = obj("Potenza_Udm_Cod")
                Else
                    curParco_Macchine.Potenza_Udm_Cod = 0
                End If


                If Not String.IsNullOrEmpty(obj("Tipo_Targa_Cod")) Then
                    curParco_Macchine.Tipo_Targa_Cod = obj("Tipo_Targa_Cod")
                Else
                    curParco_Macchine.Tipo_Targa_Cod = 0
                End If


                If Not String.IsNullOrEmpty(obj("Tipo_Trazione_Cod")) Then
                    curParco_Macchine.Tipo_Trazione_Cod = obj("Tipo_Trazione_Cod")
                Else
                    curParco_Macchine.Tipo_Trazione_Cod = 0
                End If


                If Not String.IsNullOrEmpty(obj("N_Omologazione")) Then
                    curParco_Macchine.N_Omologazione = obj("N_Omologazione")
                Else
                    curParco_Macchine.N_Omologazione = ""
                End If


                If Not String.IsNullOrEmpty(obj("Ditta_Cod_Motore")) Then
                    curParco_Macchine.Ditta_Cod_Motore = obj("Ditta_Cod_Motore")
                Else
                    curParco_Macchine.Ditta_Cod_Motore = 0
                End If


                If Not String.IsNullOrEmpty(obj("Tipo_Motore")) Then
                    curParco_Macchine.Tipo_Motore = obj("Tipo_Motore")
                Else
                    curParco_Macchine.Tipo_Motore = ""
                End If


                If Not String.IsNullOrEmpty(obj("Matricola_Motore")) Then
                    curParco_Macchine.Matricola_Motore = obj("Matricola_Motore")
                Else
                    curParco_Macchine.Matricola_Motore = ""
                End If


                If Not String.IsNullOrEmpty(obj("Data_Reimmatricolazione")) Then
                    curParco_Macchine.Data_Reimmatricolazione = AgronicaCoreUtility.DataOra.DataStringaJSON_to_ISO8601(obj("Data_Reimmatricolazione"))
                Else
                    curParco_Macchine.Data_Reimmatricolazione = AGRODATAINIZIO
                End If


                If Not String.IsNullOrEmpty(obj("Data_Carico")) Then
                    curParco_Macchine.Data_Carico = obj("Data_Carico")
                Else
                    curParco_Macchine.Data_Carico = AGRODATAINIZIO
                End If


                If Not String.IsNullOrEmpty(obj("Data_Scarico")) Then
                    curParco_Macchine.Data_Scarico = obj("Data_Scarico")
                Else
                    curParco_Macchine.Data_Scarico = AGRODATAINIZIO
                End If


                If Not String.IsNullOrEmpty(obj("TitoloPossesso")) Then
                    curParco_Macchine.TitoloPossesso = obj("TitoloPossesso")
                Else
                    curParco_Macchine.TitoloPossesso = 0
                End If


                If Not String.IsNullOrEmpty(obj("Flag_Attrezzatura_Macchina")) Then
                    curParco_Macchine.Flag_Attrezzatura_Macchina = obj("Flag_Attrezzatura_Macchina")
                Else
                    curParco_Macchine.Flag_Attrezzatura_Macchina = "M"
                End If


                If Not String.IsNullOrEmpty(obj("Taratura_Ugello")) Then
                    curParco_Macchine.Taratura_Ugello = obj("Taratura_Ugello")
                Else
                    curParco_Macchine.Taratura_Ugello = 0
                End If


                If Not String.IsNullOrEmpty(obj("Codice")) Then
                    curParco_Macchine.Codice = obj("Codice")
                Else
                    curParco_Macchine.Codice = ""
                End If


                If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
                    curParco_Macchine.validita_inizio = AgronicaCoreUtility.DataOra.DataStringaJSON_to_ISO8601(obj("Validita_Inizio"))
                Else
                    curParco_Macchine.validita_inizio = ValiditaInizio
                End If


                If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
                    curParco_Macchine.validita_fine = AgronicaCoreUtility.DataOra.DataStringaJSON_to_ISO8601(obj("Validita_Fine"))
                Else
                    curParco_Macchine.validita_fine = ValiditaFine
                End If


                curParco_Macchine.data_creazione = Date.Now
                curParco_Macchine.username_creazione = objParametri.UsernameOperazione
                curParco_Macchine.data_modifica = Date.Now
                curParco_Macchine.username_modifica = objParametri.UsernameOperazione
                curParco_Macchine.inviato = 0

                EFArrayToInsert.Add(curParco_Macchine)
            Next


            For Each obj As JObject In righeModificateArray
                curParco_Macchine = campConf_R.Leggi_Parco_Macchine(obj("mac_cod"), objParametri)
                If curParco_Macchine Is Nothing Then
                    MessaggioErrore += "Riga da aggiornare " & obj("Validita_Inizio").ToString & " - " & obj("Validita_Fine").ToString & " non trovata"
                Else

                    curParco_Macchine.Mac_Cod = obj("mac_cod")
                    curParco_Macchine.Mac_Des = obj("mac_des")
                    curParco_Macchine.Modello = obj("Modello")
                    curParco_Macchine.Class_Code = obj("class_code")
                    curParco_Macchine.Ditta_Cod = obj("Ditta_Cod")
                    curParco_Macchine.Taratura_Ugello = obj("Taratura_Ugello")

                    curParco_Macchine.Ultima_Manutenzione = AgronicaCoreUtility.DataOra.DataStringaJSON_to_ISO8601(obj("Ultima_Manutenzione"))
                    curParco_Macchine.Ultima_Revisione = AgronicaCoreUtility.DataOra.DataStringaJSON_to_ISO8601(obj("Ultima_Revisione"))


                    If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
                        curParco_Macchine.validita_inizio = Date.ParseExact(obj("Validita_Inizio"), Format, Provider)
                    End If
                    If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
                        curParco_Macchine.validita_fine = Date.ParseExact(obj("Validita_Fine"), Format, Provider)
                    End If
                    curParco_Macchine.data_modifica = Date.Now
                    curParco_Macchine.username_modifica = objParametri.UsernameOperazione
                    EFArrayToUpdate.Add(curParco_Macchine)

                End If
            Next
            For Each obj As JObject In righeCancellateArray
                curParco_Macchine = New Parco_Macchine
                curParco_Macchine.Mac_Cod = obj("mac_cod")
                EFArrayToDelete.Add(curParco_Macchine)
            Next


            If String.IsNullOrEmpty(MessaggioErrore) Then
                Dim campConf_W As New AgronicaCoreContabDAL.Parco_Macchine_W

                MessaggioErrore = campConf_W.Aggiorna_Parco_Macchine(
                      EFArrayToInsert,
                      EFArrayToUpdate,
                      EFArrayToDelete,
                      objParametri
                 )

                For i As Integer = 0 To righeInseriteArray.Count - 1
                    righeInseriteArray(i).Item("mac_cod") = CType(EFArrayToInsert(i), Parco_Macchine).Mac_Cod
                Next

            End If



        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
        Finally

        End Try


        If Not String.IsNullOrEmpty(MessaggioErrore) Then
            Throw New Exception(MessaggioErrore)
        End If

        Return MessaggioErrore
    End Function

    Public Function Macchina_Scrivi(
                                    ByVal DatiMacchina As String,
                                    ByRef OUTPUT_Piva As String,
                                    ByRef OUTPUT_Mac_Cod As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabBIZ.Parco_Macchine.Macchina_Scrivi()"

        Dim XmlDoc As XmlDocument

        Dim FlagTransazioneLocale As Boolean = False 'True
        Dim FlagConnessioneLocale As Boolean = False

        Dim objMacchina As AgronicaCoreContabDAL.Parco_Macchine_W
        Dim ObjSequenze As AgronicaCoreDataProvider.Agro_Sequenze

        Dim xDatiMacchine As Xml.XmlNodeList
        Dim xDatiMacchina As Xml.XmlElement
        Dim xMacchine As Xml.XmlNodeList
        Dim xMacchina As Xml.XmlElement

        Dim i_DatiMacchina As Integer
        Dim i_Macchina As Integer
        Dim Dummy As Long

        Dim Mac_Cod As Integer

        Dim OpeDB_Macchina As String

        Try

            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri)

            '------------------------------

            XmlDoc = New Xml.XmlDocument
            XmlDoc.LoadXml(DatiMacchina)

            '------------------------------

            xDatiMacchine = XmlDoc.GetElementsByTagName("DatiMacchine")

            i_DatiMacchina = 0

            Do While i_DatiMacchina < xDatiMacchine.Count
                xDatiMacchina = xDatiMacchine.Item(i_DatiMacchina)

                '------------------------------

                xMacchine = xDatiMacchina.GetElementsByTagName("Macchina")

                i_Macchina = 0

                Do While i_Macchina < xMacchine.Count

                    xMacchina = xMacchine.Item(i_Macchina)

                    'Prelevo gli attributi della macchina selezionata
                    OpeDB_Macchina = xMacchina.GetAttribute("TipoOperazioneDB")

                    'Istanzio l'oggetto
                    objMacchina = New AgronicaCoreContabDAL.Parco_Macchine_W

                    'Inizializzo Preventivamente il Mac_Cod
                    Mac_Cod = CStr(xMacchina.GetAttribute("mac_cod"))


                    Select Case OpeDB_Macchina
                        Case "0" 'LEGGI

                            OUTPUT_Piva = CStr(xMacchina.GetAttribute("piva"))
                            OUTPUT_Mac_Cod = CStr(xMacchina.GetAttribute("mac_cod"))

                        Case "1" 'SALVA

                            Select Case Mac_Cod
                                Case 0
                                    ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze
                                    'CreateObject("Agro_Contab_AD.Agro_Sequenze")

                                    Mac_Cod = CInt(ObjSequenze.NuovoId_Tabella(
                                                                        "Parco_Macchine",
                                                                        -2000000000,
                                                                        0,
                                                                            objParametri))
                                Case Else

                            End Select

                            Dummy = objMacchina.Scrivi(
                                    CStr(xMacchina.GetAttribute("piva")),
                                                CInt(xMacchina.GetAttribute("sa_cod")),
                                                CInt(Mac_Cod),
                                                IIf(xMacchina.HasAttribute("cod_contatto") = False, "", xMacchina.GetAttribute("cod_contatto")),
                                                CStr(xMacchina.GetAttribute("class_code")),
                                                CInt(xMacchina.GetAttribute("tipo")),
                                                CStr(xMacchina.GetAttribute("mac_des")),
                                                CDbl(xMacchina.GetAttribute("costo_acquisto")),
                                                CStr(xMacchina.GetAttribute("targa")),
                                                CStr(xMacchina.GetAttribute("telaio")),
                                                CInt(xMacchina.GetAttribute("ditta_cod")),
                                                CStr(xMacchina.GetAttribute("modello")),
                                                CStr(xMacchina.GetAttribute("potenza")),
                                                CDbl(xMacchina.GetAttribute("ammortamento")),
                                                CDate(xMacchina.GetAttribute("data_immatricolazione")),
                                                CDate(xMacchina.GetAttribute("ultima_manutenzione")),
                                                CDate(xMacchina.GetAttribute("ultima_revisione")),
                                                CStr(xMacchina.GetAttribute("stato_utilizzo")),
                                                IIf(xMacchina.HasAttribute("note") = False, "", xMacchina.GetAttribute("note")),
                                                CStr(xMacchina.GetAttribute("n_immatricolazione")),
                                                CStr(xMacchina.GetAttribute("n_immatricolazione_rimorchio")),
                                                CStr(xMacchina.GetAttribute("n_autorizzazione_trasporto")),
                                                CDate(xMacchina.GetAttribute("data_rilascio_autorizzazione")),
                                                CDbl(xMacchina.GetAttribute("peso")),
                                                IIf(IsNumeric(xMacchina.GetAttribute("portata_max")), xMacchina.GetAttribute("portata_max"), 0),
                                                IIf(IsNumeric(xMacchina.GetAttribute("chkdefault")), xMacchina.GetAttribute("chkdefault"), 0),
                                                CInt(xMacchina.GetAttribute("alimentazione_cod")),
                                                CInt(xMacchina.GetAttribute("potenza_udm_cod")),
                                                CInt(xMacchina.GetAttribute("mac_cod_origine")),
                                                CStr(xMacchina.GetAttribute("piva_superuser_origine")),
                                                Agro_XML_GetString(xMacchina, "cuaa_proprietario", ""),
                                                Agro_XML_GetString(xMacchina, "denominazione_proprietario", ""),
                                                Agro_XML_GetInteger(xMacchina, "tipo_targa_cod", 0),
                                                Agro_XML_GetInteger(xMacchina, "tipo_trazione", 0),
                                                Agro_XML_GetString(xMacchina, "n_omologazione", ""),
                                                Agro_XML_GetInteger(xMacchina, "ditta_cod_motore", 0),
                                                Agro_XML_GetString(xMacchina, "tipo_motore", ""),
                                                Agro_XML_GetString(xMacchina, "matricola_motore", ""),
                                                Agro_XML_GetDate(xMacchina, "data_reimmatricolazione", AGRODATAINIZIO),
                                                Agro_XML_GetDate(xMacchina, "data_carico", AGRODATAINIZIO),
                                                Agro_XML_GetDate(xMacchina, "data_scarico", AGRODATAFINE),
                                                Agro_XML_GetInteger(xMacchina, "titolopossesso", 0),
                                                Agro_XML_GetString(xMacchina, "flag_attrezzatura_macchina", ""),
                                                CDate(xMacchina.GetAttribute("validita_inizio")),
                                                CDate(xMacchina.GetAttribute("validita_fine")),
                                                objParametri, Agro_XML_GetDecimal(xMacchina, "taratura_ugello", 0))

                        Case "2" 'MODIFICA

                            objMacchina.Modifica(
                                    CStr(xMacchina.GetAttribute("piva")),
                                                CInt(xMacchina.GetAttribute("sa_cod")),
                                                CInt(Mac_Cod),
                                                IIf(xMacchina.HasAttribute("cod_contatto") = False, "", xMacchina.GetAttribute("cod_contatto")),
                                                CStr(xMacchina.GetAttribute("class_code")),
                                                CInt(xMacchina.GetAttribute("tipo")),
                                                CStr(xMacchina.GetAttribute("mac_des")),
                                                CDbl(xMacchina.GetAttribute("costo_acquisto")),
                                                CStr(xMacchina.GetAttribute("targa")),
                                                CStr(xMacchina.GetAttribute("telaio")),
                                                CInt(xMacchina.GetAttribute("ditta_cod")),
                                                CStr(xMacchina.GetAttribute("modello")),
                                                CStr(xMacchina.GetAttribute("potenza")),
                                                CDbl(xMacchina.GetAttribute("ammortamento")),
                                                CDate(xMacchina.GetAttribute("data_immatricolazione")),
                                                CDate(xMacchina.GetAttribute("ultima_manutenzione")),
                                                CDate(xMacchina.GetAttribute("ultima_revisione")),
                                                CStr(xMacchina.GetAttribute("stato_utilizzo")),
                                                IIf(xMacchina.HasAttribute("note") = False, "", xMacchina.GetAttribute("note")),
                                                CStr(xMacchina.GetAttribute("n_immatricolazione")),
                                                CStr(xMacchina.GetAttribute("n_immatricolazione_rimorchio")),
                                                CStr(xMacchina.GetAttribute("n_autorizzazione_trasporto")),
                                                CDate(xMacchina.GetAttribute("data_rilascio_autorizzazione")),
                                                CDbl(xMacchina.GetAttribute("peso")),
                                                IIf(IsNumeric(xMacchina.GetAttribute("portata_max")), xMacchina.GetAttribute("portata_max"), 0),
                                                IIf(IsNumeric(xMacchina.GetAttribute("chkdefault")), xMacchina.GetAttribute("chkdefault"), 0),
                                                CInt(xMacchina.GetAttribute("alimentazione_cod")),
                                                CInt(xMacchina.GetAttribute("potenza_udm_cod")),
                                                Agro_XML_GetString(xMacchina, "cuaa_proprietario", ""),
                                                Agro_XML_GetString(xMacchina, "denominazione_proprietario", ""),
                                                Agro_XML_GetInteger(xMacchina, "tipo_targa_cod", 0),
                                                Agro_XML_GetInteger(xMacchina, "tipo_trazione", 0),
                                                Agro_XML_GetString(xMacchina, "n_omologazione", ""),
                                                Agro_XML_GetInteger(xMacchina, "ditta_cod_motore", 0),
                                                Agro_XML_GetString(xMacchina, "tipo_motore", ""),
                                                Agro_XML_GetString(xMacchina, "matricola_motore", ""),
                                                Agro_XML_GetDate(xMacchina, "data_reimmatricolazione", AGRODATAINIZIO),
                                                Agro_XML_GetDate(xMacchina, "data_carico", AGRODATAINIZIO),
                                                Agro_XML_GetDate(xMacchina, "data_scarico", AGRODATAFINE),
                                                Agro_XML_GetInteger(xMacchina, "titolopossesso", 0),
                                                Agro_XML_GetString(xMacchina, "flag_attrezzatura_macchina", ""),
                                                CDate(xMacchina.GetAttribute("validita_inizio")),
                                                CDate(xMacchina.GetAttribute("validita_fine")),
                                                "", objParametri, Agro_XML_GetDecimal(xMacchina, "taratura_ugello", 0))

                        Case "3" 'ELIMINA

                            objMacchina.Cancella(CStr(xMacchina.GetAttribute("piva")),
                                                            Mac_Cod,
                                                            "",
                                                            "", objParametri)

                    End Select

                    i_Macchina += i_Macchina + 1

                Loop

                'Incremento l'indice
                i_DatiMacchina = i_DatiMacchina + 1

            Loop

        Catch ex As Exception

        End Try
    End Function

    Public Function Scrivi_Macchina_Anagrafica(ByRef objMacchina As AgronicaCoreModelsSTD.anagrafiche.ParcoMacchine,
                                               ByVal tipoOperazione As AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB,
                                               ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                               ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim Piva_SuperUser = objParametri_Server.PivaSuperUser
        Dim dal As New AgronicaCoreContabDAL.EFMacchine()

        Dim ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

        Dim NomeRoutine As String = "AgronicaCoreContabBIZ.Parco_Macchine_W.Scrivi_Macchina_Anagrafica()"
        Dim MessaggioErrore As String = String.Empty

        ''poiché i rateiTempo non vengono gestiti in EF, devo aprire la transazione
        'Dim GiasContext As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities = Nothing

        'Dim FlagConnessioneLocale, FlagTransazioneLocale As Boolean

        'Dim trOpt As New TransactionOptions With {
        '    .Timeout = New TimeSpan(0, 10, 0),
        '    .IsolationLevel = Transactions.IsolationLevel.ReadUncommitted
        '}
        ''Istanzio la transazione forzando l'uso di una nuova transazione
        'Using ts As New TransactionScope(TransactionScopeOption.RequiresNew, trOpt)
        '    Dim OpenNewTransaction As Boolean = False

        Dim scope As TransactionScope = Nothing

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)

        Dim GiasContext As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities = Nothing
        Dim bCloseContext As Boolean = False
        Dim OpenNewTransaction As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri_Server.StringaConnessione)
            bCloseContext = True
        End If

        If objParametri_Server.objTransazione Is Nothing Then
            Dim scopeOption As New TransactionScopeOption
            Dim transactionOptions As New TransactionOptions
            transactionOptions.IsolationLevel = Transactions.IsolationLevel.ReadUncommitted
            scope = New TransactionScope(scopeOption, transactionOptions)
            OpenNewTransaction = True
        End If

        Try
            'GiasContext = New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(EFConnString)
            ''Open the contextObject connection state explicitly
            'GiasContext.Database.Connection.Open()

            'ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, objParametri_Server, System.Data.IsolationLevel.ReadUncommitted)
            dal.impostaDefaultMacchina(objMacchina)
            If tipoOperazione <> enum_TipoOperazioneDB.Cancellazione AndAlso Not ParcoMacchineUtility.CheckValidity(objMacchina) Then
                Throw New Exception(AgronicaCoreDataProvider.My.Resources.Gias.MachineDateValidityError)
            End If

            If (tipoOperazione = enum_TipoOperazioneDB.Scrittura) Then
                Dim macchina = dal.Macchina_Scrivi_EF(objMacchina, objParametri_Server, objParametri_Server.UsernameOperazione, GiasContext, False)
                ' restituisce il codice della macchina creata nell'oggetto passato
                If macchina IsNot Nothing Then
                    If objMacchina.rateiTempo IsNot Nothing Then
                        dal.gestisciRateiTempo(objMacchina, objParametri_Server)
                    End If
                    objMacchina.codice = macchina.Mac_Cod
                End If
            Else

                If (tipoOperazione = enum_TipoOperazioneDB.Modifica) AndAlso ParcoMacchineUtility.IsEditAllowed(objMacchina, objParametri_Server) Then
                    dal.Macchina_Modifica_EF(objMacchina, objParametri_Server, objParametri_Server.UsernameOperazione, objParametri_Utenti, GiasContext, False)
                    If objMacchina.rateiTempo IsNot Nothing Then
                        dal.gestisciRateiTempo(objMacchina, objParametri_Server)
                    End If
                ElseIf (tipoOperazione = enum_TipoOperazioneDB.Cancellazione) AndAlso ParcoMacchineUtility.IsRemoveAllowed(objMacchina, objParametri_Server) Then
                    Dim objAxP As New AppezzamentiXParcoMacchine_R
                    If objAxP.Read(objParametri_Server, macCod:=objMacchina.codice).Rows.Count > 0 Then
                        Throw New GiasException(My.Resources.AgronicaCoreContabBIZ.ErroreCancellazioneMacchinaAppezzamentiAssociati)
                    End If

                    dal.Macchina_Cancella_EF(objMacchina, objParametri_Server, GiasContext, False)

                    If objMacchina.rateiTempo IsNot Nothing Then
                        dal.gestisciRateiTempo(objMacchina, objParametri_Server)
                    End If
                End If

            End If

            If OpenNewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If

        Catch ex As GiasException
            'If objParametri_Server.objTransazione IsNot Nothing Then
            '    ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            'End If
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            Throw ex
        Catch ex As Exception
            'If objParametri_Server.objTransazione IsNot Nothing Then
            '    ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            'End If
            If scope IsNot Nothing Then
                scope.Dispose()
            End If

            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)

            'Finally

            '    ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

            '    ts.Dispose()

            '    If GiasContext IsNot Nothing AndAlso GiasContext.Database.Connection.State = ConnectionState.Open Then
            '        GiasContext.Database.Connection.Close()
            '        GiasContext.Dispose()
            '    End If

        End Try
        'End Using

        If bCloseContext Then
            GiasContext.Dispose()
        End If

        Return MessaggioErrore

    End Function

    ' verifica associazione macchina con BTM e nel caso associa VIN e BTM alla macchina
    ' "0" = Macchina non associata a BTM e BTM non associato ad altre macchine
    ' "1" = Macchina già associata a BTM (codice, VIN e BTM coincidono)
    ' "2" = Macchina associata ad altro BTM (codice e VIN coincidono ma BTM diverso)
    ' "3" = BTM associato ad altra macchina (BTM presente su macchine con codice e VIN diverso)
    ' "9" = Macchina non trovata o VIN non corrispondentente
    Public Function Verifica_Associazione_BTM(ByVal codice As Integer, ByVal VIN As String, ByVal BTM_Serial As String,
                                              ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim objMacchinaR As New AgronicaCoreContabDAL.Parco_Macchine_R
        Dim objMacchinaW As New AgronicaCoreContabDAL.Parco_Macchine_W

        Dim dt = objMacchinaR.LeggiAssociazioneBTM(codice, "", "", objParametri_Server)
        Dim dtBTM = objMacchinaR.LeggiAssociazioneBTM(0, "", BTM_Serial, objParametri_Server)

        If dt.Rows.Count > 0 Then

            Dim VIN_Macchina As String = If(IsDBNull(dt.Rows(0).Item("VIN")), "", dt.Rows(0).Item("VIN"))
            Dim BTM_Macchina As String = If(IsDBNull(dt.Rows(0).Item("BTM_Serial")), "", dt.Rows(0).Item("BTM_Serial"))

            ' VIN non corrisponde a quello della macchina
            If Not String.IsNullOrEmpty(VIN_Macchina) AndAlso VIN_Macchina <> VIN Then
                Return "9"
            End If

            ' macchina non associata a BTM e BTM non associato ad altre macchine
            If String.IsNullOrEmpty(BTM_Macchina) AndAlso dtBTM.Rows.Count = 0 Then
                objMacchinaW.ScriviAssociazioneBTM(codice, VIN, BTM_Serial, objParametri_Server)
                Return "0"
            End If

            ' macchina già associata a BTM
            If VIN_Macchina = VIN AndAlso BTM_Macchina = BTM_Serial Then
                Return "1"
            End If

            ' macchina associata ad altro BTM
            If Not String.IsNullOrEmpty(BTM_Macchina) AndAlso BTM_Macchina <> BTM_Serial Then
                Return "2"
            End If

            ' BTM associato ad altra macchina 
            If dtBTM.Rows.Count > 0 Then
                Return "3"
            End If

        End If

        Return "9"

    End Function

End Class

Public Class ParcoMacchineUtility
    Public Shared Function CheckValidity(macchina As ParcoMacchine) As Boolean
        Return macchina.validita.fine > macchina.validita.inizio
    End Function

    Public Shared Function VerificaMovimentiMacchina(objMacchina As ParcoMacchine,
                                                     objParametri_Server As AgronicaCoreParametri,
                                                     Optional BypassControlloDifferenze As Boolean = False) As Boolean

        'TODO Salvo: da aggiustare la funzione di controllo per verificare che non vi siano movimenti dopo la data fine e prima della data inizio
        Try
            If BypassControlloDifferenze Then
                Return (CheckMovimentazioneMacchina(objMacchina, objParametri_Server) OrElse CheckCostiMacchina(objMacchina, objParametri_Server))
            Else
                Dim objMacchineBIZ As New AgronicaCoreContabBIZ.Parco_Macchine_R
                Dim oldM As ParcoMacchine
                oldM = objMacchineBIZ.Leggi_Macchina(objMacchina.partitaIva, objMacchina.codice, objParametri_Server)

                If IsVisibilityChanged(objMacchina, oldM) OrElse IsTypeChanged(objMacchina, oldM) OrElse IsContactChanged(objMacchina, oldM) Then
                    Return CheckMovimentazioneMacchina(objMacchina, objParametri_Server)
                ElseIf IsValidityChanged(objMacchina, oldM) Then
                    Return CheckMovimentazioneMacchina(objMacchina, objParametri_Server, True)
                End If
            End If
        Catch ex As Exception
            Throw ex
        End Try

        Return False
    End Function

    Public Shared Function IsEditAllowed(newM As ParcoMacchine, objParametri As AgronicaCoreParametri) As Boolean
        Dim objMacchineBIZ As New AgronicaCoreContabBIZ.Parco_Macchine_R
        Dim oldM As ParcoMacchine = objMacchineBIZ.Leggi_Macchina(newM.partitaIva, newM.codice, objParametri)

        If Not CheckValidity(newM) Then
            Throw New GiasException(AgronicaCoreDataProvider.My.Resources.Gias.MachineDateValidityError)
        ElseIf IsTypeChanged(newM, oldM) AndAlso (CheckMovimentazioneMacchina(newM, objParametri) OrElse CheckCostiMacchina(newM, objParametri)) Then
            Throw New GiasException(AgronicaCoreDataProvider.My.Resources.Gias.ImpossibileModificareTipoMacchinaMovimentiAssociati)
        ElseIf IsContactChanged(newM, oldM) AndAlso (CheckMovimentazioneMacchina(newM, objParametri) OrElse CheckCostiMacchina(newM, objParametri)) Then
            Throw New GiasException(AgronicaCoreDataProvider.My.Resources.Gias.ImpossibileModificareContattoMacchinaMovimentiAssociati)
        ElseIf IsValidityChanged(newM, oldM) Then
            If CheckMovimentazioneMacchina(newM, objParametri, True) Then
                Throw New GiasException(AgronicaCoreDataProvider.My.Resources.Gias.ImpossibileModificareDataMacchinaMovimentiAssociati)
            ElseIf CheckCostiMacchina(newM, objParametri, True) Then
                Throw New GiasException(AgronicaCoreDataProvider.My.Resources.Gias.ImpossibileModificareDataMacchinaCostiAssociati)
            End If
        ElseIf IsTypeChanged(newM, oldM) AndAlso IsLinkedToAgriculturalPlot(newM, objParametri) Then
            Throw New GiasException(My.Resources.AgronicaCoreContabBIZ.MachineLinkedToAgriculturalPlotTypeEditError)
        ElseIf IsVisibilityChanged(newM, oldM) Then
            Select Case newM.centroPK.codice
                Case EFMacchine.VisibilitaPubblica 'aumento la visibilità: sempre OK
                    Return True
                Case EFMacchine.VisibilitaPrivata
                    If oldM.centroPK.codice <> EFMacchine.VisibilitaPubblica Then ' aumento la visibilità (da centro a privata): sempre OK
                        Return True
                    Else ' diminuisco la visbilità (da pubblica a privata): solo se la macchina è stata usata solo dall'azienda proprietaria
                        Dim pivas = CompaniesByWhichIsUsed(newM, objParametri)
                        If Not (pivas.Count = 0 OrElse (pivas.Count = 1 AndAlso pivas.First = newM.partitaIva)) Then
                            Throw New GiasException(AgronicaCoreDataProvider.My.Resources.Gias.MacchinaMovimentataDaAltreImprese)
                        End If
                    End If
                Case Else ' imposto la visbilità su un centro
                    ' OK: se è stata usata solo sullo stesso centro su cui si vuole stringere la visibilità
                    Dim centres = CentresOnWichIsUsed(newM, objParametri)
                    If Not (centres.Count = 0 OrElse (centres.Count = 1 AndAlso centres.First = newM.centroPK.codice)) Then
                        Throw New GiasException(AgronicaCoreDataProvider.My.Resources.Gias.MacchinaMovimentataSuAltriCentri)
                    End If
            End Select
        End If

        ' non è cambiato ne il tipo, ne visibilità, ne il contatto: sempre OK
        Return True
    End Function

    Public Shared Function IsRemoveAllowed(mac As ParcoMacchine, objParametri As AgronicaCoreParametri) As Boolean
        If CheckMovimentazioneMacchina(mac, objParametri) Then
            Throw New GiasException(AgronicaCoreDataProvider.My.Resources.Gias.ImpossibileCancellareMacchinaMovimentiAssociati)
        ElseIf CheckCostiMacchina(mac, objParametri) Then
            Throw New GiasException(AgronicaCoreDataProvider.My.Resources.Gias.ImpossibileCancellareMacchinaCostiAssociati)
        Else
            Return True
        End If
    End Function

    Public Shared Function IsTypeChanged(ByVal objMacchina As AgronicaCoreModelsSTD.anagrafiche.ParcoMacchine,
                                         ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim objMacchineBIZ As New AgronicaCoreContabBIZ.Parco_Macchine_R
        Dim m_old As ParcoMacchine
        m_old = objMacchineBIZ.Leggi_Macchina(objMacchina.partitaIva, objMacchina.codice, objParametri_Server)

        Return IsTypeChanged(objMacchina, m_old)
    End Function

    Public Shared Function IsTypeChanged(ByVal newM As ParcoMacchine,
                                         ByVal oldM As ParcoMacchine) As Boolean
        Return (
            oldM.tipo.codice <> newM.tipo.codice OrElse
            oldM.dettaglio_1.codice <> newM.dettaglio_1.codice OrElse
            oldM.dettaglio_2.codice <> newM.dettaglio_2.codice
            )
    End Function

    Public Shared Function IsContactChanged(ByVal objMacchina As AgronicaCoreModelsSTD.anagrafiche.ParcoMacchine,
                                     ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim objMacchineBIZ As New AgronicaCoreContabBIZ.Parco_Macchine_R
        Dim m_old As ParcoMacchine
        m_old = objMacchineBIZ.Leggi_Macchina(objMacchina.partitaIva, objMacchina.codice, objParametri_Server)

        Return IsContactChanged(objMacchina, m_old)
    End Function

    Public Shared Function IsContactChanged(ByVal newM As ParcoMacchine,
                                            ByVal oldM As ParcoMacchine) As Boolean
        Return (
            oldM.contatto.primaryKey.codice <> newM.contatto.primaryKey.codice AndAlso
            oldM.contatto.primaryKey.partitaIva <> newM.contatto.primaryKey.partitaIva
            )
    End Function

    Public Shared Function IsVisibilityChanged(ByVal objMacchina As ParcoMacchine,
                                        ByVal objParametri_Server As AgronicaCoreParametri) As Boolean

        Dim objMacchineBIZ As New AgronicaCoreContabBIZ.Parco_Macchine_R
        Dim m_old As ParcoMacchine
        m_old = objMacchineBIZ.Leggi_Macchina(objMacchina.partitaIva, objMacchina.codice, objParametri_Server)

        Return IsVisibilityChanged(objMacchina, m_old)
    End Function

    Public Shared Function IsVisibilityChanged(ByVal newM As ParcoMacchine,
                                               ByVal oldM As ParcoMacchine) As Boolean

        Return (
            (Not IsNothing(newM.centroPK) AndAlso newM.centroPK.codice <> oldM.centroPK.codice) OrElse
            (CInt(newM.visibilitaPubblica) <> oldM.centroPK.codice AndAlso (oldM.centroPK.codice = 0 OrElse oldM.centroPK.codice = -1))
            )
    End Function

    Public Shared Function CentresOnWichIsUsed(objMacchina As ParcoMacchine,
                                               objParametri_Server As AgronicaCoreParametri) As List(Of Integer)

        Dim objMacchineDAL As New AgronicaCoreContabDAL.Parco_Macchine_R
        Dim movs = objMacchineDAL.LeggiMovimentiMacchina(objMacchina.partitaIva, objMacchina.codice, objParametri_Server, True)
        Dim recipes = objMacchineDAL.RecipesCentres(objMacchina.codice, objParametri_Server)

        recipes.Merge(movs, True, MissingSchemaAction.Ignore)
        Dim saCods = DatatableUtility.SelectDistinct_To_DT(recipes, True, {"Sa_Cod"})

        'Dim costs = objMacchineDAL.CheckMacchinaCosti(objMacchina.partitaIva, objMacchina.codice, objParametri_Server)

        Return saCods.AsEnumerable().Select(Of Integer)(Function(r) r.Field(Of Integer)("Sa_Cod")).ToList
    End Function

    Public Shared Function CompaniesByWhichIsUsed(objMacchina As ParcoMacchine,
                                                  objParametri_Server As AgronicaCoreParametri) As List(Of String)

        Dim objMacchineDAL As New AgronicaCoreContabDAL.Parco_Macchine_R
        Dim movs = objMacchineDAL.LeggiMovimentiMacchina(objMacchina.partitaIva, objMacchina.codice, objParametri_Server, True)
        Dim recipes = objMacchineDAL.CheckMacchinaRicette(objMacchina.partitaIva, objMacchina.codice, objParametri_Server, True)
        Dim costs = objMacchineDAL.CheckMacchinaCosti(objMacchina.partitaIva, objMacchina.codice, objParametri_Server, True)

        movs.Merge(recipes, True, MissingSchemaAction.Ignore)
        movs.Merge(costs, True, MissingSchemaAction.Ignore)

        Dim pivas = DatatableUtility.SelectDistinct_To_DT(movs, True, {"PIVA"})
        Return pivas.AsEnumerable().Select(Of String)(Function(r) r.Field(Of String)("PIVA")).ToList
    End Function

    Private Shared Function CheckMovimentazioneMacchina(ByVal objMacchina As AgronicaCoreModelsSTD.anagrafiche.ParcoMacchine,
                                                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                        Optional ByVal CheckData As Boolean = False) As Boolean

        If CheckData AndAlso
            objMacchina.validita.inizio = AGRODATAINIZIO AndAlso
            objMacchina.validita.fine = AGRODATAFINE Then
            'Non ho bisogno di controllare i movimenti perchè le validità della macchina sono AGRODATAINIZIO e AGRODATAFINE
            Return False
        End If

        Dim objMacchineDAL As New AgronicaCoreContabDAL.Parco_Macchine_R
        Dim validita_inizio As Date = AGRODATAINIZIO
        Dim validita_fine As Date = AGRODATAFINE

        'Se le date della Macchina sono state modificate
        If CheckData Then
            validita_inizio = objMacchina.validita.inizio
            validita_fine = objMacchina.validita.fine
        End If

        ' operazioni agenda
        Dim dt As DataTable = objMacchineDAL.LeggiMovimentiMacchina(objMacchina.partitaIva,
                                                                    objMacchina.codice,
                                                                    objParametri_Server,
                                                                    False,
                                                                    validita_inizio,
                                                                    validita_fine)
        If (dt.Rows.Count > 0) Then
            Return True
        End If

        'ricette
        dt = objMacchineDAL.CheckMacchinaRicette(objMacchina.partitaIva,
                                                 objMacchina.codice,
                                                 objParametri_Server,
                                                 False,
                                                 validita_inizio,
                                                 validita_fine)
        If (dt.Rows.Count > 0) Then
            Return True
        End If

        Return False
    End Function

    Private Shared Function IsLinkedToAgriculturalPlot(machine As ParcoMacchine,
                                                       objServer As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim objAxP As New AppezzamentiXParcoMacchine_R
        Return objAxP.Read(objServer, macCod:=machine.codice).Rows.Count > 0
    End Function

    Private Shared Function IsValidityChanged(newM As ParcoMacchine,
                                              oldM As ParcoMacchine) As Boolean
        Return (
            newM.validita.inizio <> oldM.validita.inizio OrElse
            newM.validita.fine <> oldM.validita.fine
            )
    End Function

    Private Shared Function CheckCostiMacchina(ByVal objMacchina As AgronicaCoreModelsSTD.anagrafiche.ParcoMacchine,
                                                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                        Optional ByVal CheckData As Boolean = False) As Boolean


        If CheckData AndAlso
            objMacchina.validita.inizio = AGRODATAINIZIO AndAlso
            objMacchina.validita.fine = AGRODATAFINE Then
            'Non ho bisogno di controllare i movimenti perchè le validità della macchina sono AGRODATAINIZIO e AGRODATAFINE
            Return False
        End If

        Dim objMacchineDAL As New AgronicaCoreContabDAL.Parco_Macchine_R
        Dim validita_inizio As Date = AGRODATAINIZIO
        Dim validita_fine As Date = AGRODATAFINE

        'Se le date della Macchina sono state modificate
        If CheckData Then
            validita_inizio = objMacchina.validita.inizio
            validita_fine = objMacchina.validita.fine
        End If

        'costi
        Dim dt As DataTable = objMacchineDAL.CheckMacchinaCosti(objMacchina.partitaIva,
                                               objMacchina.codice,
                                               objParametri_Server,
                                               False,
                                               validita_inizio,
                                               validita_fine)
        If (dt.Rows.Count > 0) Then
            Return True
        End If

        Return False
    End Function
End Class
