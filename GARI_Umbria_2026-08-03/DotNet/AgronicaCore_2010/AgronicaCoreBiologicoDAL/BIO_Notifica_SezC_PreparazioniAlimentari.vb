Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class BIO_Notifica_SezC_PreparazioniAlimentari_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal Notifica_ID As Integer, _
                            ByVal CaratteristichePA_Numero As Int32, _
                            ByVal CentroPreparazione_Piva As String, _
                            ByVal CentroPreparazione_SaCod As Int32, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreBiologicoDAL.BIO_Notifica_SezC_PreparazioniAlimentari_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT BIO_Notifica_SezC_PreparazioniAlimentari.* ")
            StrSQL.Append(" FROM  BIO_Notifica_SezC_PreparazioniAlimentari ")
            StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Notifica_ID <> 0 Then
                StrSQL.Append(" AND BIO_Notifica_SezC_PreparazioniAlimentari.Notifica_ID = " & Agro_SQL_SaveNum(Notifica_ID) & " ")
            End If

            If CaratteristichePA_Numero <> 0 Then
                StrSQL.Append(" AND CaratteristichePA_Numero = " & Agro_SQL_SaveNum(CaratteristichePA_Numero) & "  ")
            End If

            If CentroPreparazione_Piva <> "" Then
                StrSQL.Append(" AND CentroPreparazione_Piva = '" & Agro_SQL_SaveText(CentroPreparazione_Piva) & "'  ")
            End If

            If CentroPreparazione_SaCod <> 0 Then
                StrSQL.Append(" AND CentroPreparazione_SaCod = " & Agro_SQL_SaveNum(CentroPreparazione_SaCod) & "  ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   BIO_Notifica_SezC_PreparazioniAlimentari.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   BIO_Notifica_SezC_PreparazioniAlimentari.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                'StrSQL.Append(" ORDER BY Desc_CorpoEstraneo")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

End Class

'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################

Public Class BIO_Notifica_SezC_PreparazioniAlimentari_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    'Data_Creazione e username_creazione sono passati come parametri per tenere traccia 
    'dei dati originali di creazione
    '(la modifica avviene con cancella e scrivi)
    Public Function Scrivi(ByVal Notifica_ID As Integer,
                            ByVal CentroPreparazione_Piva As String,
                            ByVal CentroPreparazione_SaCod As Integer,
                            ByVal CentroPreparazionePiva_Flag_Nostampa_Stampa As Integer,
                            ByVal CentroPreparazione_RagSoc As String,
                            ByVal CentroPreparazione_Via As String,
                            ByVal CentroPreparazione_Numero As String,
                            ByVal CentroPreparazione_CAP As String,
                            ByVal CentroPreparazione_CodIstat_Provincia As String,
                            ByVal CentroPreparazione_CodIstat_Comune As String,
                            ByVal CentroPreparazione_Telefono As String,
                            ByVal CentroPreparazione_Fax As String,
                            ByVal CentroPreparazione_Email As String,
                            ByVal CentroPreparazione_CodAUSL As String,
                            ByVal CentroPreparazione_Comune As String,
                            ByVal CentroPreparazione_Sigla_Provincia As String,
                            ByVal CentroPreparazione_Provincia As String,
                            ByVal PA_VEGETALE_Cp As Integer,
                            ByVal PA_VEGETALE_Ct As Integer,
                            ByVal PA_VEGETALE_Cm As Integer,
                            ByVal PA_Vegetale_Ortofrutticoli_Cp As Integer,
                            ByVal PA_Vegetale_Ortofrutticoli_Ct As Integer,
                            ByVal PA_Vegetale_Ortofrutticoli_Cm As Integer,
                            ByVal PA_Vegetale_Molinatura_Cp As Integer,
                            ByVal PA_Vegetale_Molinatura_Ct As Integer,
                            ByVal PA_Vegetale_Molinatura_Cm As Integer,
                            ByVal PA_Vegetale_Fioccatura_Cp As Integer,
                            ByVal PA_Vegetale_Fioccatura_Ct As Integer,
                            ByVal PA_Vegetale_Fioccatura_Cm As Integer,
                            ByVal PA_Vegetale_Pastificazione_Cp As Integer,
                            ByVal PA_Vegetale_Pastificazione_Ct As Integer,
                            ByVal PA_Vegetale_Pastificazione_Cm As Integer,
                            ByVal PA_Vegetale_Surgelati_Cp As Integer,
                            ByVal PA_Vegetale_Surgelati_Ct As Integer,
                            ByVal PA_Vegetale_Surgelati_Cm As Integer,
                            ByVal PA_Vegetale_Conserve_Cp As Integer,
                            ByVal PA_Vegetale_Conserve_Ct As Integer,
                            ByVal PA_Vegetale_Conserve_Cm As Integer,
                            ByVal PA_Vegetale_IntegratoriAlimentari_Cp As Integer,
                            ByVal PA_Vegetale_IntegratoriAlimentari_Ct As Integer,
                            ByVal PA_Vegetale_IntegratoriAlimentari_Cm As Integer,
                            ByVal PA_Vegetale_EstrazioneOlio_Cp As Integer,
                            ByVal PA_Vegetale_EstrazioneOlio_Ct As Integer,
                            ByVal PA_Vegetale_EstrazioneOlio_Cm As Integer,
                            ByVal PA_Vegetale_Vinificazione_Cp As Integer,
                            ByVal PA_Vegetale_Vinificazione_Ct As Integer,
                            ByVal PA_Vegetale_Vinificazione_Cm As Integer,
                            ByVal PA_Vegetale_Liquori_Cp As Integer,
                            ByVal PA_Vegetale_Liquori_Ct As Integer,
                            ByVal PA_Vegetale_Liquori_Cm As Integer,
                            ByVal PA_Vegetale_Imbottigliamento_Cp As Integer,
                            ByVal PA_Vegetale_Imbottigliamento_Ct As Integer,
                            ByVal PA_Vegetale_Imbottigliamento_Cm As Integer,
                            ByVal PA_Vegetale_ProdottiErboristici_Cp As Integer,
                            ByVal PA_Vegetale_ProdottiErboristici_Ct As Integer,
                            ByVal PA_Vegetale_ProdottiErboristici_Cm As Integer,
                            ByVal PA_Vegetale_AltroDes_Cp As Integer,
                            ByVal PA_Vegetale_AltroDes_Ct As Integer,
                            ByVal PA_Vegetale_AltroDes_Cm As Integer,
                            ByVal PA_Vegetale_AltroDes As String,
                            ByVal PA_ANIMALE_Cp As Integer,
                            ByVal PA_ANIMALE_Ct As Integer,
                            ByVal PA_ANIMALE_Cm As Integer,
                            ByVal PA_Animale_Porzionatura_Cp As Integer,
                            ByVal PA_Animale_Porzionatura_Ct As Integer,
                            ByVal PA_Animale_Porzionatura_Cm As Integer,
                            ByVal PA_Animale_Macellazione_Cp As Integer,
                            ByVal PA_Animale_Macellazione_Ct As Integer,
                            ByVal PA_Animale_Macellazione_Cm As Integer,
                            ByVal PA_Animale_Sezionamento_Cp As Integer,
                            ByVal PA_Animale_Sezionamento_Ct As Integer,
                            ByVal PA_Animale_Sezionamento_Cm As Integer,
                            ByVal PA_Animale_DerivatiCarne_Cp As Integer,
                            ByVal PA_Animale_DerivatiCarne_Ct As Integer,
                            ByVal PA_Animale_DerivatiCarne_Cm As Integer,
                            ByVal PA_Animale_ConserveAnimali_Cp As Integer,
                            ByVal PA_Animale_ConserveAnimali_Ct As Integer,
                            ByVal PA_Animale_ConserveAnimali_Cm As Integer,
                            ByVal PA_Animale_ProdottiSalumeria_Cp As Integer,
                            ByVal PA_Animale_ProdottiSalumeria_Ct As Integer,
                            ByVal PA_Animale_ProdottiSalumeria_Cm As Integer,
                            ByVal PA_Animale_LatteAlimentare_Cp As Integer,
                            ByVal PA_Animale_LatteAlimentare_Ct As Integer,
                            ByVal PA_Animale_LatteAlimentare_Cm As Integer,
                            ByVal PA_Animale_Caseificazione_Cp As Integer,
                            ByVal PA_Animale_Caseificazione_Ct As Integer,
                            ByVal PA_Animale_Caseificazione_Cm As Integer,
                            ByVal PA_Animale_Burro_Cp As Integer,
                            ByVal PA_Animale_Burro_Ct As Integer,
                            ByVal PA_Animale_Burro_Cm As Integer,
                            ByVal PA_Animale_Yogurt_Cm As Integer,
                            ByVal PA_Animale_Yogurt_Cp As Integer,
                            ByVal PA_Animale_Yogurt_Ct As Integer,
                            ByVal PA_Animale_Uova_Cp As Integer,
                            ByVal PA_Animale_Uova_Ct As Integer,
                            ByVal PA_Animale_Uova_Cm As Integer,
                            ByVal PA_Animale_Altro_Cp As Integer,
                            ByVal PA_Animale_Altro_Ct As Integer,
                            ByVal PA_Animale_Altro_Cm As Integer,
                            ByVal PA_Animale_AltroDes As String,
                            ByVal PA_INDUSTRIADOLCIARIA_Cp As Integer,
                            ByVal PA_INDUSTRIADOLCIARIA_Ct As Integer,
                            ByVal PA_IndustriaDolciaria_DaForno_Cp As Integer,
                            ByVal PA_IndustriaDolciaria_DaForno_Ct As Integer,
                            ByVal PA_IndustriaDolciaria_DaForno_Cm As Integer,
                            ByVal PA_IndustriaDolciaria_ProdottiDolciari_Cp As Integer,
                            ByVal PA_IndustriaDolciaria_ProdottiDolciari_Ct As Integer,
                            ByVal PA_IndustriaDolciaria_AltriProdotti_Cp As Integer,
                            ByVal PA_IndustriaDolciaria_AltriProdotti_Ct As Integer,
                            ByVal PA_IndustriaDolciaria_AltriProdotti_Cm As Integer,
                            ByVal PA_MANGIMI_Cp As Integer,
                            ByVal PA_MANGIMI_Ct As Integer,
                            ByVal PA_MANGIMI_Cm As Integer,
                            ByVal PA_Mangimi_AltroDes_Cp As Integer,
                            ByVal PA_Mangimi_AltroDes_Ct As Integer,
                            ByVal PA_Mangimi_AltroDes_Cm As Integer,
                            ByVal PA_Mangimi_AltroDes As String,
                            ByVal PA_IMMAGAZZINAMENTO_Cp As Integer,
                            ByVal PA_IMMAGAZZINAMENTO_Ct As Integer,
                            ByVal PA_CONSERVAZIONE_Cp As Integer,
                            ByVal PA_CONSERVAZIONE_Ct As Integer,
                            ByVal PA_CONDIZIONAMENTO_Cp As Integer,
                            ByVal PA_CONDIZIONAMENTO_Ct As Integer,
                            ByVal PA_CONFEZIONAMENTO_Cp As Integer,
                            ByVal PA_CONFEZIONAMENTO_Ct As Integer,
                            ByVal PA_ETICHETTATURA_Cp As Integer,
                            ByVal PA_ETICHETTATURA_Ct As Integer,
                            ByVal PA_ALTRO_Cp As Integer,
                            ByVal PA_ALTRO_Ct As Integer,
                            ByVal PA_ALTRODes As String,
                            ByVal CaratteristichePA_Numero As Integer,
                            ByVal CaratteristichePA_TipoDes As String,
                            ByVal CaratteristichePA_Flag_Periodica_Continuativa As Integer,
                            ByVal CaratteristichePA_CapacitaLavoro As Decimal,
                            ByVal CaratteristichePA_CapacitaStoccaggio As Decimal,
                            ByVal CaratteristichePA_CapacitaLavoro_UdmCod As Integer,
                            ByVal CaratteristichePA_CapacitaLavoroTempo_UdmCod As Integer,
                            ByVal CaratteristichePA_CapacitaStoccaggio_UdmCod As Integer,
                            ByVal CaratteristichePA_Periodica_Bio As Integer,
                            ByVal CaratteristichePA_Continuativa_Bio As Integer,
                            ByVal CaratteristichePA_Periodica_Conv As Integer,
                            ByVal CaratteristichePA_Continuativa_Conv As Integer,
                            ByVal STRUTTURE_SILI_mc As Decimal,
                            ByVal STRUTTURE_SILI_StoccaggioCereali As Integer,
                            ByVal STRUTTURE_SILI_StoccaggioProteoleaginose As Integer,
                            ByVal STRUTTURE_SILI_Altro_Des As String,
                            ByVal STRUTTURE_SILI_Altro As Integer,
                            ByVal STRUTTURE_CELLE_mc As Decimal,
                            ByVal STRUTTURE_CELLE_Vegetali As Integer,
                            ByVal STRUTTURE_CELLE_Zootecnici As Integer,
                            ByVal STRUTTURE_CELLE_Altro_Des As String,
                            ByVal STRUTTURE_CELLE_Altro As Integer,
                            ByVal STRUTTURE_UTILIZZO_Dedicato As Integer,
                            ByVal STRUTTURE_UTILIZZO_Misto As Integer,
                            ByVal COMMERCIO_Ingrosso As Integer,
                            ByVal COMMERCIO_Dettagliante As Integer,
                            ByVal COMMERCIO_GDO As Integer,
                            ByVal COMMERCIO_Amarchio As Integer,
                            ByVal COMMERCIO_Altro As Integer,
                            ByVal COMMERCIO_PERIODICA_Bio As Integer,
                            ByVal COMMERCIO_PERIODICA_Conv As Integer,
                            ByVal COMMERCIO_CONTINUATIVA_Bio As Integer,
                            ByVal COMMERCIO_CONTINUATIVA_Conv As Integer,
                            ByVal COMMERCIO_AMARCHIO_Des As String,
                            ByVal Username_Creazione As String,
                               ByVal Data_Creazione As Date,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                , Optional ByVal Data_modifica As Date = #2/1/1900# _
                , Optional ByVal username_modifica As String = ""
                                ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreBiologicoDAL.BIO_Notifica_SezC_PreparazioniAlimentari_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try



            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If


            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" INSERT INTO BIO_Notifica_SezC_PreparazioniAlimentari " & vbCrLf)

            StrSQL.Append("             (Notifica_SuperUser, " & vbCrLf)
            StrSQL.Append("             Notifica_ID, " & vbCrLf)
            StrSQL.Append("             CaratteristichePA_Numero, CentroPreparazione_Piva, CentroPreparazione_SaCod, " & vbCrLf)
            StrSQL.Append("             CentroPreparazionePiva_Flag_Nostampa_Stampa, CentroPreparazione_RagSoc, CentroPreparazione_Via, " & vbCrLf)
            StrSQL.Append("             CentroPreparazione_Numero, " & vbCrLf)
            StrSQL.Append("             CentroPreparazione_CAP, CentroPreparazione_CodIstat_Provincia, CentroPreparazione_CodIstat_Comune, " & vbCrLf)
            StrSQL.Append("             CentroPreparazione_Telefono, CentroPreparazione_Fax, CentroPreparazione_Email, CentroPreparazione_CodAUSL, CentroPreparazione_Comune, CentroPreparazione_Sigla_Provincia, CentroPreparazione_Provincia, PA_VEGETALE_Cp, PA_VEGETALE_Ct, " & vbCrLf)
            StrSQL.Append("             PA_Vegetale_Molinatura_Cp, PA_Vegetale_Molinatura_Ct, PA_Vegetale_Fioccatura_Cp, PA_Vegetale_Fioccatura_Ct, PA_Vegetale_Pastificazione_Cp, " & vbCrLf)
            StrSQL.Append("             PA_Vegetale_Pastificazione_Ct, PA_Vegetale_Conserve_Cp, PA_Vegetale_Conserve_Ct, PA_Vegetale_IntegratoriAlimentari_Cp, " & vbCrLf)
            StrSQL.Append("             PA_Vegetale_IntegratoriiAlimentari_Ct, PA_Vegetale_EstrazioneOlio_Cp, PA_Vegetale_EstrazioneOlio_Ct, PA_Vegetale_Vinificazione_Cp, " & vbCrLf)
            StrSQL.Append("             PA_Vegetale_Vinificazione_Ct, PA_Vegetale_Liquori_Cp, PA_Vegetale_Liquori_Ct, PA_Vegetale_Imbottigliamento_Cp, " & vbCrLf)
            StrSQL.Append("             PA_Vegetale_Imbottigliamento_Ct, PA_Vegetale_ProdottiErboristici_Cp, PA_Vegetale_ProdottiErboristici_Ct, PA_ANIMALE_Cp, PA_ANIMALE_Ct, " & vbCrLf)
            StrSQL.Append("             PA_Animale_Macellazione_Cp, PA_Animale_Macellazione_Ct, PA_Animale_Sezionamento_Cp, PA_Animale_Sezionamento_Ct, " & vbCrLf)
            StrSQL.Append("             PA_Animale_DerivatiCarne_Cp, PA_Animale_DerivatiCarne_Ct, PA_Animale_ConserveAnimali_Cp, PA_Animale_ConserveAnimali_Ct, " & vbCrLf)
            StrSQL.Append("             PA_Animale_ProdottiSalumeria_Cp, PA_Animale_ProdottiSalumeria_Ct, PA_Animale_LatteAlimentare_Cp, PA_Animale_LatteAlimentare_Ct, " & vbCrLf)
            StrSQL.Append("             PA_Animale_Caseificazione_Cp, PA_Animale_Caseificazione_Ct, PA_Animale_Burro_Cp, PA_Animale_Burro_Ct, PA_Animale_Yogurt_Cp, " & vbCrLf)
            StrSQL.Append("             PA_Animale_Yogurt_Ct, PA_Animale_Altro_Cp, PA_Animale_Altro_Ct, PA_Animale_AltroDes, PA_INDUSTRIADOLCIARIA_Cp, " & vbCrLf)
            StrSQL.Append("             PA_INDUSTRIADOLCIARIA_Ct, PA_IndustriaDolciaria_ProdottiDolciari_Cp, PA_IndustriaDolciaria_ProdottiDolciari_Ct, " & vbCrLf)
            StrSQL.Append("             PA_IndustriaDolciaria_AltriProdotti_Cp, PA_IndustriaDolciaria_AltriProdotti_Ct, PA_MANGIMI_Cp, PA_MANGIMI_Ct, PA_ETICHETTATURA_Cp, " & vbCrLf)
            StrSQL.Append("             PA_ETICHETTATURA_Ct, PA_ALTRO_Cp, PA_ALTRO_Ct, PA_ALTRODes, CaratteristichePA_TipoDes, CaratteristichePA_Flag_Periodica_Continuativa, " & vbCrLf)
            StrSQL.Append("             CaratteristichePA_CapacitaLavoro, CaratteristichePA_CapacitaLavoro_UdmCod, CaratteristichePA_CapacitaLavoroTempo_UdmCod, " & vbCrLf)
            StrSQL.Append("             CaratteristichePA_CapacitaStoccaggio, CaratteristichePA_CapacitaStoccaggio_UdmCod,  " & vbCrLf)

            'Aggiuntive nuova notifica
            StrSQL.Append("             PA_VEGETALE_Cm, PA_Vegetale_Ortofrutticoli_Cp, PA_Vegetale_Ortofrutticoli_Ct, PA_Vegetale_Ortofrutticoli_Cm, PA_Vegetale_Molinatura_Cm, PA_Vegetale_Fioccatura_Cm, " & vbCrLf)
            StrSQL.Append("             PA_Vegetale_Pastificazione_Cm, PA_Vegetale_Surgelati_Cp, PA_Vegetale_Surgelati_Ct, PA_Vegetale_Surgelati_Cm, PA_Vegetale_Conserve_Cm, PA_Vegetale_IntegratoriAlimentari_Cm, " & vbCrLf)
            StrSQL.Append("             PA_Vegetale_EstrazioneOlio_Cm, PA_Vegetale_Vinificazione_Cm, PA_Vegetale_Liquori_Cm, PA_Vegetale_Imbottigliamento_Cm, PA_Vegetale_ProdottiErboristici_Cm, PA_Vegetale_AltroDes, " & vbCrLf)
            StrSQL.Append("             PA_Vegetale_AltroDes_Cp, PA_Vegetale_AltroDes_Ct, PA_Vegetale_AltroDes_Cm, PA_ANIMALE_Cm, PA_Animale_Porzionatura_Cp, PA_Animale_Porzionatura_Ct, PA_Animale_Porzionatura_Cm, " & vbCrLf)
            StrSQL.Append("             PA_Animale_Macellazione_Cm, PA_Animale_Sezionamento_Cm, PA_Animale_DerivatiCarne_Cm, PA_Animale_ConserveAnimali_Cm, PA_Animale_ProdottiSalumeria_Cm, PA_Animale_LatteAlimentare_Cm, " & vbCrLf)
            StrSQL.Append("             PA_Animale_Caseificazione_Cm, PA_Animale_Burro_Cm, PA_Animale_Yogurt_Cm, PA_Animale_Uova_Cp, PA_Animale_Uova_Ct, PA_Animale_Uova_Cm, PA_Animale_Altro_Cm, PA_IndustriaDolciaria_DaForno_Cp, " & vbCrLf)
            StrSQL.Append("             PA_IndustriaDolciaria_DaForno_Ct, PA_IndustriaDolciaria_DaForno_Cm, PA_IndustriaDolciaria_AltriProdotti_Cm, PA_MANGIMI_Cm, PA_Mangimi_AltroDes, PA_Mangimi_AltroDes_Cp, PA_Mangimi_AltroDes_Ct, " & vbCrLf)
            StrSQL.Append("             PA_Mangimi_AltroDes_Cm, PA_IMMAGAZZINAMENTO_Cp, PA_IMMAGAZZINAMENTO_Ct, PA_CONSERVAZIONE_Cp, PA_CONSERVAZIONE_Ct, PA_CONDIZIONAMENTO_Cp, PA_CONDIZIONAMENTO_Ct, PA_CONFEZIONAMENTO_Cp, " & vbCrLf)
            StrSQL.Append("             PA_CONFEZIONAMENTO_Ct, CaratteristichePA_Periodica_Bio, CaratteristichePA_Continuativa_Bio, CaratteristichePA_Periodica_Conv, CaratteristichePA_Continuativa_Conv, STRUTTURE_SILI_mc, " & vbCrLf)
            StrSQL.Append("             STRUTTURE_SILI_StoccaggioCereali, STRUTTURE_SILI_StoccaggioProteoleaginose, STRUTTURE_SILI_Altro_Des, STRUTTURE_SILI_Altro, STRUTTURE_CELLE_mc, STRUTTURE_CELLE_Vegetali, STRUTTURE_CELLE_Zootecnici, " & vbCrLf)
            StrSQL.Append("             STRUTTURE_CELLE_Altro_Des, STRUTTURE_CELLE_Altro, STRUTTURE_UTILIZZO_Dedicato, STRUTTURE_UTILIZZO_Misto, COMMERCIO_Ingrosso, COMMERCIO_Dettagliante, COMMERCIO_GDO, COMMERCIO_Amarchio, COMMERCIO_Altro, " & vbCrLf)
            StrSQL.Append("             COMMERCIO_PERIODICA_Bio, COMMERCIO_PERIODICA_Conv, COMMERCIO_CONTINUATIVA_Bio, COMMERCIO_CONTINUATIVA_Conv, COMMERCIO_AMARCHIO_Des, " & vbCrLf)



            StrSQL.Append("              Inviato,            datainvio, " & vbCrLf)
            StrSQL.Append("              Data_Creazione,     Data_Modifica, " & vbCrLf)
            StrSQL.Append("              UserName_Creazione, UserName_Modifica, " & vbCrLf)
            StrSQL.Append("              Validita_Inizio,    Validita_Fine  , DataLock " & vbCrLf)
            StrSQL.Append("              ) ")

            StrSQL.Append(" VALUES (")
            StrSQL.Append(" '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(Notifica_ID)) & "" & vbCrLf)

            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(CaratteristichePA_Numero)) & " " & vbCrLf)
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(CentroPreparazione_Piva)) & "' " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(CentroPreparazione_SaCod)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(CentroPreparazionePiva_Flag_Nostampa_Stampa)) & " " & vbCrLf)
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(CentroPreparazione_RagSoc)) & "' " & vbCrLf)
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(CentroPreparazione_Via)) & "' " & vbCrLf)
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(CentroPreparazione_Numero)) & "' " & vbCrLf)
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(CentroPreparazione_CAP)) & "' " & vbCrLf)
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(CentroPreparazione_CodIstat_Provincia)) & "' " & vbCrLf)
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(CentroPreparazione_CodIstat_Comune)) & "' " & vbCrLf)
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(CentroPreparazione_Telefono)) & "' " & vbCrLf)
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(CentroPreparazione_Fax)) & "' " & vbCrLf)
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(CentroPreparazione_Email)) & "' " & vbCrLf)
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(CentroPreparazione_CodAUSL)) & "' " & vbCrLf)
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(CentroPreparazione_Comune)) & "' " & vbCrLf)
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(CentroPreparazione_Sigla_Provincia)) & "' " & vbCrLf)
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(CentroPreparazione_Provincia)) & "' " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_VEGETALE_Cp)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_VEGETALE_Ct)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Vegetale_Molinatura_Cp)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Vegetale_Molinatura_Ct)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Vegetale_Fioccatura_Cp)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Vegetale_Fioccatura_Ct)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Vegetale_Pastificazione_Cp)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Vegetale_Pastificazione_Ct)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Vegetale_Conserve_Cp)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Vegetale_Conserve_Ct)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Vegetale_IntegratoriAlimentari_Cp)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Vegetale_IntegratoriAlimentari_Ct)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Vegetale_EstrazioneOlio_Cp)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Vegetale_EstrazioneOlio_Ct)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Vegetale_Vinificazione_Cp)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Vegetale_Vinificazione_Ct)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Vegetale_Liquori_Cp)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Vegetale_Liquori_Ct)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Vegetale_Imbottigliamento_Ct)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Vegetale_Imbottigliamento_Cp)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Vegetale_ProdottiErboristici_Ct)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Vegetale_ProdottiErboristici_Cp)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_ANIMALE_Cp)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_ANIMALE_Ct)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Animale_Macellazione_Cp)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Animale_Macellazione_Ct)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Animale_Sezionamento_Cp)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Animale_Sezionamento_Ct)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Animale_DerivatiCarne_Cp)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Animale_DerivatiCarne_Ct)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Animale_ConserveAnimali_Cp)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Animale_ConserveAnimali_Ct)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Animale_ProdottiSalumeria_Cp)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Animale_ProdottiSalumeria_Ct)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Animale_LatteAlimentare_Cp)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Animale_LatteAlimentare_Ct)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Animale_Caseificazione_Cp)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Animale_Caseificazione_Ct)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Animale_Burro_Cp)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Animale_Burro_Ct)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Animale_Yogurt_Cp)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Animale_Yogurt_Ct)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Animale_Altro_Cp)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Animale_Altro_Ct)) & " " & vbCrLf)
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(PA_Animale_AltroDes)) & "' " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_INDUSTRIADOLCIARIA_Cp)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_INDUSTRIADOLCIARIA_Ct)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_IndustriaDolciaria_ProdottiDolciari_Cp)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_IndustriaDolciaria_ProdottiDolciari_Ct)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_IndustriaDolciaria_AltriProdotti_Cp)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_IndustriaDolciaria_AltriProdotti_Ct)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_MANGIMI_Cp)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_MANGIMI_Ct)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_ETICHETTATURA_Cp)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_ETICHETTATURA_Ct)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_ALTRO_Cp)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_ALTRO_Ct)) & " " & vbCrLf)
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(PA_ALTRODes)) & "' " & vbCrLf)
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(CaratteristichePA_TipoDes)) & "' " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(CaratteristichePA_Flag_Periodica_Continuativa)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(CaratteristichePA_CapacitaLavoro)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(CaratteristichePA_CapacitaLavoro_UdmCod)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(CaratteristichePA_CapacitaLavoroTempo_UdmCod)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(CaratteristichePA_CapacitaStoccaggio)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(CaratteristichePA_CapacitaStoccaggio_UdmCod)) & " " & vbCrLf)

            'Aggiuntive nuova notifica
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_VEGETALE_Cm)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Vegetale_Ortofrutticoli_Cp)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Vegetale_Ortofrutticoli_Ct)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Vegetale_Ortofrutticoli_Cm)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Vegetale_Molinatura_Cm)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Vegetale_Fioccatura_Cm)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Vegetale_Pastificazione_Cm)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Vegetale_Surgelati_Cp)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Vegetale_Surgelati_Ct)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Vegetale_Surgelati_Cm)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Vegetale_Conserve_Cm)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Vegetale_IntegratoriAlimentari_Cm)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Vegetale_EstrazioneOlio_Cm)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Vegetale_Vinificazione_Cm)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Vegetale_Liquori_Cm)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Vegetale_Imbottigliamento_Cm)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Vegetale_ProdottiErboristici_Cm)) & " " & vbCrLf)
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(PA_Vegetale_AltroDes)) & "' " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Vegetale_AltroDes_Cp)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Vegetale_AltroDes_Ct)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Vegetale_AltroDes_Cm)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_ANIMALE_Cm)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Animale_Porzionatura_Cp)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Animale_Porzionatura_Ct)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Animale_Porzionatura_Cm)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Animale_Macellazione_Cm)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Animale_Sezionamento_Cm)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Animale_DerivatiCarne_Cm)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Animale_ConserveAnimali_Cm)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Animale_ProdottiSalumeria_Cm)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Animale_LatteAlimentare_Cm)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Animale_Caseificazione_Cm)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Animale_Burro_Cm)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Animale_Yogurt_Cm)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Animale_Uova_Cp)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Animale_Uova_Ct)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Animale_Uova_Cm)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Animale_Altro_Cm)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_IndustriaDolciaria_DaForno_Cp)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_IndustriaDolciaria_DaForno_Ct)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_IndustriaDolciaria_DaForno_Cm)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_IndustriaDolciaria_AltriProdotti_Cm)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_MANGIMI_Cm)) & " " & vbCrLf)
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(PA_Mangimi_AltroDes)) & "' " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Mangimi_AltroDes_Cp)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Mangimi_AltroDes_Ct)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_Mangimi_AltroDes_Cm)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_IMMAGAZZINAMENTO_Cp)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_IMMAGAZZINAMENTO_Ct)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_CONSERVAZIONE_Cp)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_CONSERVAZIONE_Ct)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_CONDIZIONAMENTO_Cp)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_CONDIZIONAMENTO_Ct)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_CONFEZIONAMENTO_Cp)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PA_CONFEZIONAMENTO_Ct)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(CaratteristichePA_Periodica_Bio)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(CaratteristichePA_Continuativa_Bio)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(CaratteristichePA_Periodica_Conv)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(CaratteristichePA_Continuativa_Conv)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(STRUTTURE_SILI_mc)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(STRUTTURE_SILI_StoccaggioCereali)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(STRUTTURE_SILI_StoccaggioProteoleaginose)) & " " & vbCrLf)
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(STRUTTURE_SILI_Altro_Des)) & "' " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(STRUTTURE_SILI_Altro)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(STRUTTURE_CELLE_mc)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(STRUTTURE_CELLE_Vegetali)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(STRUTTURE_CELLE_Zootecnici)) & " " & vbCrLf)
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(STRUTTURE_CELLE_Altro_Des)) & "' " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(STRUTTURE_CELLE_Altro)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(STRUTTURE_UTILIZZO_Dedicato)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(STRUTTURE_UTILIZZO_Misto)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(COMMERCIO_Ingrosso)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(COMMERCIO_Dettagliante)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(COMMERCIO_GDO)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(COMMERCIO_Amarchio)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(COMMERCIO_Altro)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(COMMERCIO_PERIODICA_Bio)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(COMMERCIO_PERIODICA_Conv)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(COMMERCIO_CONTINUATIVA_Bio)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(COMMERCIO_CONTINUATIVA_Conv)) & " " & vbCrLf)
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(COMMERCIO_AMARCHIO_Des)) & "' " & vbCrLf)

            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")

            StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_Creazione) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(Username_Creazione) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")



            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append("         , 0  ")
            StrSQL.Append(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

    ''##############################################################################################
    'Public Function Scrivi_2(ByVal Notifica_ID As Integer, _
    '                        ByVal CaratteristichePA_Numero As Integer, _
    '                        ByVal CentroPreparazione_Piva As String, _
    '                        ByVal CentroPreparazione_SaCod As Integer, _
    '                        ByVal CentroPreparazionePiva_Flag_Nostampa_Stampa As Integer, _
    '                        ByVal CentroPreparazione_RagSoc As String, _
    '                        ByVal CentroPreparazione_Via As String, _
    '                        ByVal CentroPreparazione_Numero As String, _
    '                        ByVal CentroPreparazione_CAP As String, _
    '                        ByVal CentroPreparazione_CodIstat_Provincia As String, _
    '                        ByVal CentroPreparazione_CodIstat_Comune As String, _
    '                        ByVal CentroPreparazione_Telefono As String, _
    '                        ByVal CentroPreparazione_Fax As String, _
    '                        ByVal CentroPreparazione_Email As String, _
    '                        ByVal CentroPreparazione_CodAUSL As String, _
    '                        ByVal CentroPreparazione_Comune As String, _
    '                        ByVal CentroPreparazione_Sigla_Provincia As String, _
    '                        ByVal CentroPreparazione_Provincia As String, _
    '                        ByVal PA_VEGETALE_Cp As Integer, _
    '                        ByVal PA_VEGETALE_Ct As Integer, _
    '                        ByVal PA_Vegetale_Molinatura_Cp As Integer, _
    '                        ByVal PA_Vegetale_Molinatura_Ct As Integer, _
    '                        ByVal PA_Vegetale_Fioccatura_Cp As Integer, _
    '                        ByVal PA_Vegetale_Fioccatura_Ct As Integer, _
    '                        ByVal PA_Vegetale_Pastificazione_Cp As Integer, _
    '                        ByVal PA_Vegetale_Pastificazione_Ct As Integer, _
    '                        ByVal PA_Vegetale_Conserve_Cp As Integer, _
    '                        ByVal PA_Vegetale_Conserve_Ct As Integer, _
    '                        ByVal PA_Vegetale_IntegratoriAlimentari_Cp As Integer, _
    '                        ByVal PA_Vegetale_IntegratoriiAlimentari_Ct As Integer, _
    '                        ByVal PA_Vegetale_EstrazioneOlio_Cp As Integer, _
    '                        ByVal PA_Vegetale_EstrazioneOlio_Ct As Integer, _
    '                        ByVal PA_Vegetale_Vinificazione_Cp As Integer, _
    '                        ByVal PA_Vegetale_Vinificazione_Ct As Integer, _
    '                        ByVal PA_Vegetale_Liquori_Cp As Integer, _
    '                        ByVal PA_Vegetale_Liquori_Ct As Integer, _
    '                        ByVal PA_Vegetale_Imbottigliamento_Ct As Integer, _
    '                        ByVal PA_Vegetale_Imbottigliamento_Cp As Integer, _
    '                        ByVal PA_Vegetale_ProdottiErboristici_Ct As Integer, _
    '                        ByVal PA_Vegetale_ProdottiErboristici_Cp As Integer, _
    '                        ByVal PA_ANIMALE_Cp As Integer, _
    '                        ByVal PA_ANIMALE_Ct As Integer, _
    '                        ByVal PA_Animale_Macellazione_Cp As Integer, _
    '                        ByVal PA_Animale_Macellazione_Ct As Integer, _
    '                        ByVal PA_Animale_Sezionamento_Cp As Integer, _
    '                        ByVal PA_Animale_Sezionamento_Ct As Integer, _
    '                        ByVal PA_Animale_DerivatiCarne_Cp As Integer, _
    '                        ByVal PA_Animale_DerivatiCarne_Ct As Integer, _
    '                        ByVal PA_Animale_ConserveAnimali_Cp As Integer, _
    '                        ByVal PA_Animale_ConserveAnimali_Ct As Integer, _
    '                        ByVal PA_Animale_ProdottiSalumeria_Cp As Integer, _
    '                        ByVal PA_Animale_ProdottiSalumeria_Ct As Integer, _
    '                        ByVal PA_Animale_LatteAlimentare_Cp As Integer, _
    '                        ByVal PA_Animale_LatteAlimentare_Ct As Integer, _
    '                        ByVal PA_Animale_Caseificazione_Cp As Integer, _
    '                        ByVal PA_Animale_Caseificazione_Ct As Integer, _
    '                        ByVal PA_Animale_Burro_Cp As Integer, _
    '                        ByVal PA_Animale_Burro_Ct As Integer, _
    '                        ByVal PA_Animale_Yogurt_Cp As Integer, _
    '                        ByVal PA_Animale_Yogurt_Ct As Integer, _
    '                        ByVal PA_Animale_Altro_Cp As Integer, _
    '                        ByVal PA_Animale_Altro_Ct As Integer, _
    '                        ByVal PA_Animale_AltroDes As String, _
    '                        ByVal PA_INDUSTRIADOLCIARIA_Cp As Integer, _
    '                        ByVal PA_INDUSTRIADOLCIARIA_Ct As Integer, _
    '                        ByVal PA_IndustriaDolciaria_ProdottiDolciari_Cp As Integer, _
    '                        ByVal PA_IndustriaDolciaria_ProdottiDolciari_Ct As Integer, _
    '                        ByVal PA_IndustriaDolciaria_AltriProdotti_Cp As Integer, _
    '                        ByVal PA_IndustriaDolciaria_AltriProdotti_Ct As Integer, _
    '                        ByVal PA_MANGIMI_Cp As Integer, _
    '                        ByVal PA_MANGIMI_Ct As Integer, _
    '                        ByVal PA_ETICHETTATURA_Cp As Integer, _
    '                        ByVal PA_ETICHETTATURA_Ct As Integer, _
    '                        ByVal PA_ALTRO_Cp As Integer, _
    '                        ByVal PA_ALTRO_Ct As Integer, _
    '                        ByVal PA_ALTRODes As String, _
    '                        ByVal CaratteristichePA_TipoDes As String, _
    '                        ByVal CaratteristichePA_Flag_Periodica_Continuativa As Integer, _
    '                        ByVal CaratteristichePA_CapacitaLavoro As Decimal, _
    '                        ByVal CaratteristichePA_CapacitaStoccaggio As Decimal, _
    '                        ByVal CaratteristichePA_CapacitaLavoro_UdmCod As Integer, _
    '                        ByVal CaratteristichePA_CapacitaLavoroTempo_UdmCod As Integer, _
    '                        ByVal CaratteristichePA_CapacitaStoccaggio_UdmCod As Integer, _
    '                        ByVal PA_VEGETALE_Cm As Integer, _
    '                        ByVal PA_Vegetale_Ortofrutticoli_Cp As Integer, _
    '                        ByVal PA_Vegetale_Ortofrutticoli_Ct As Integer, _
    '                        ByVal PA_Vegetale_Ortofrutticoli_Cm As Integer, _
    '                        ByVal PA_Vegetale_Molinatura_Cm As Integer, _
    '                        ByVal PA_Vegetale_Fioccatura_Cm As Integer, _
    '                        ByVal PA_Vegetale_Pastificazione_Cm As Integer, _
    '                        ByVal PA_Vegetale_Surgelati_Cp As Integer, _
    '                        ByVal PA_Vegetale_Surgelati_Ct As Integer, _
    '                        ByVal PA_Vegetale_Surgelati_Cm As Integer, _
    '                        ByVal PA_Vegetale_Conserve_Cm As Integer, _
    '                        ByVal PA_Vegetale_IntegratoriAlimentari_Cm As Integer, _
    '                        ByVal PA_Vegetale_EstrazioneOlio_Cm As Integer, _
    '                        ByVal PA_Vegetale_Vinificazione_Cm As Integer, _
    '                        ByVal PA_Vegetale_Liquori_Cm As Integer, _
    '                        ByVal PA_Vegetale_Imbottigliamento_Cm As Integer, _
    '                        ByVal PA_Vegetale_ProdottiErboristici_Cm As Integer, _
    '                        ByVal PA_Vegetale_AltroDes As String, _
    '                        ByVal PA_Vegetale_AltroDes_Cp As Integer, _
    '                        ByVal PA_Vegetale_AltroDes_Ct As Integer, _
    '                        ByVal PA_Vegetale_AltroDes_Cm As Integer, _
    '                        ByVal PA_ANIMALE_Cm As Integer, _
    '                        ByVal PA_Animale_Porzionatura_Cp As Integer, _
    '                        ByVal PA_Animale_Porzionatura_Ct As Integer, _
    '                        ByVal PA_Animale_Porzionatura_Cm As Integer, _
    '                        ByVal PA_Animale_Macellazione_Cm As Integer, _
    '                        ByVal PA_Animale_Sezionamento_Cm As Integer, _
    '                        ByVal PA_Animale_DerivatiCarne_Cm As Integer, _
    '                        ByVal PA_Animale_ConserveAnimali_Cm As Integer, _
    '                        ByVal PA_Animale_ProdottiSalumeria_Cm As Integer, _
    '                        ByVal PA_Animale_LatteAlimentare_Cm As Integer, _
    '                        ByVal PA_Animale_Caseificazione_Cm As Integer, _
    '                        ByVal PA_Animale_Burro_Cm As Integer, _
    '                        ByVal PA_Animale_Yogurt_Cm As Integer, _
    '                        ByVal PA_Animale_Uova_Cp As Integer, _
    '                        ByVal PA_Animale_Uova_Ct As Integer, _
    '                        ByVal PA_Animale_Uova_Cm As Integer, _
    '                        ByVal PA_Animale_Altro_Cm As Integer, _
    '                        ByVal PA_IndustriaDolciaria_DaForno_Cp As Integer, _
    '                        ByVal PA_IndustriaDolciaria_DaForno_Ct As Integer, _
    '                        ByVal PA_IndustriaDolciaria_DaForno_Cm As Integer, _
    '                        ByVal PA_IndustriaDolciaria_AltriProdotti_Cm As Integer, _
    '                        ByVal PA_MANGIMI_Cm As Integer, _
    '                        ByVal PA_Mangimi_AltroDes As String, _
    '                        ByVal PA_Mangimi_AltroDes_Cp As Integer, _
    '                        ByVal PA_Mangimi_AltroDes_Ct As Integer, _
    '                        ByVal PA_Mangimi_AltroDes_Cm As Integer, _
    '                        ByVal PA_IMMAGAZZINAMENTO_Cp As Integer, _
    '                        ByVal PA_IMMAGAZZINAMENTO_Ct As Integer, _
    '                        ByVal PA_CONSERVAZIONE_Cp As Integer, _
    '                        ByVal PA_CONSERVAZIONE_Ct As Integer, _
    '                        ByVal PA_CONDIZIONAMENTO_Cp As Integer, _
    '                        ByVal PA_CONDIZIONAMENTO_Ct As Integer, _
    '                        ByVal PA_CONFEZIONAMENTO_Cp As Integer, _
    '                        ByVal PA_CONFEZIONAMENTO_Ct As Integer, _
    '                        ByVal CaratteristichePA_Periodica_Bio As Integer, _
    '                        ByVal CaratteristichePA_Continuativa_Bio As Integer, _
    '                        ByVal CaratteristichePA_Periodica_Conv As Integer, _
    '                        ByVal CaratteristichePA_Continuativa_Conv As Integer, _
    '                        ByVal STRUTTURE_SILI_mc As Decimal, _
    '                        ByVal STRUTTURE_SILI_StoccaggioCereali As Integer, _
    '                        ByVal STRUTTURE_SILI_StoccaggioProteoleaginose As Integer, _
    '                        ByVal STRUTTURE_SILI_Altro_Des As String, _
    '                        ByVal STRUTTURE_SILI_Altro As Integer, _
    '                        ByVal STRUTTURE_CELLE_mc As Decimal, _
    '                        ByVal STRUTTURE_CELLE_Vegetali As Integer, _
    '                        ByVal STRUTTURE_CELLE_Zootecnici As Integer, _
    '                        ByVal STRUTTURE_CELLE_Altro_Des As String, _
    '                        ByVal STRUTTURE_CELLE_Altro As Integer, _
    '                        ByVal STRUTTURE_UTILIZZO_Dedicato As Integer, _
    '                        ByVal STRUTTURE_UTILIZZO_Misto As Integer, _
    '                        ByVal COMMERCIO_Ingrosso As Integer, _
    '                        ByVal COMMERCIO_Dettagliante As Integer, _
    '                        ByVal COMMERCIO_GDO As Integer, _
    '                        ByVal COMMERCIO_Amarchio As Integer, _
    '                        ByVal COMMERCIO_Altro As Integer, _
    '                        ByVal COMMERCIO_PERIODICA_Bio As Integer, _
    '                        ByVal COMMERCIO_PERIODICA_Conv As Integer, _
    '                        ByVal COMMERCIO_CONTINUATIVA_Bio As Integer, _
    '                        ByVal COMMERCIO_CONTINUATIVA_Conv As Integer, _
    '                        ByVal COMMERCIO_AMARCHIO_Des As String, _
    '                        ByVal Validita_Inizio As Date, _
    '                        ByVal Validita_Fine As Date, _
    '                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                            ) As Boolean


    '    Dim NomeRoutine As String = "AgronicaCoreBiologicoDAL.BIO_Notifica_SezC_PreparazioniAlimentari_W.Scrivi()"

    '    '====================================================================================
    '    'Parametri opzionali :

    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Try

    '        '---------------------------------------------
    '        StrSQL.Length = 0
    '        StrSQL.Append(" INSERT INTO BIO_Notifica_SezC_PreparazioniAlimentari " & vbCrLf)

    '        StrSQL.Append("             (Notifica_SuperUser, " & vbCrLf)
    '        StrSQL.Append("             Notifica_ID, " & vbCrLf)
    '        StrSQL.Append("             CaratteristichePA_Numero, CentroPreparazione_Piva, CentroPreparazione_SaCod, " & vbCrLf)
    '        StrSQL.Append("             CentroPreparazionePiva_Flag_Nostampa_Stampa, CentroPreparazione_RagSoc, CentroPreparazione_Via, " & vbCrLf)
    '        StrSQL.Append("             CentroPreparazione_Numero, " & vbCrLf)
    '        StrSQL.Append("             CentroPreparazione_CAP, CentroPreparazione_CodIstat_Provincia, CentroPreparazione_CodIstat_Comune, " & vbCrLf)
    '        StrSQL.Append("             CentroPreparazione_Telefono, CentroPreparazione_Fax, CentroPreparazione_Email, CentroPreparazione_CodAUSL, CentroPreparazione_Comune, CentroPreparazione_Sigla_Provincia, CentroPreparazione_Provincia, PA_VEGETALE_Cp, PA_VEGETALE_Ct, " & vbCrLf)
    '        StrSQL.Append("             PA_Vegetale_Molinatura_Cp, PA_Vegetale_Molinatura_Ct, PA_Vegetale_Fioccatura_Cp, PA_Vegetale_Fioccatura_Ct, PA_Vegetale_Pastificazione_Cp, " & vbCrLf)
    '        StrSQL.Append("             PA_Vegetale_Pastificazione_Ct, PA_Vegetale_Conserve_Cp, PA_Vegetale_Conserve_Ct, PA_Vegetale_IntegratoriAlimentari_Cp, " & vbCrLf)
    '        StrSQL.Append("             PA_Vegetale_IntegratoriiAlimentari_Ct, PA_Vegetale_EstrazioneOlio_Cp, PA_Vegetale_EstrazioneOlio_Ct, PA_Vegetale_Vinificazione_Cp, " & vbCrLf)
    '        StrSQL.Append("             PA_Vegetale_Vinificazione_Ct, PA_Vegetale_Liquori_Cp, PA_Vegetale_Liquori_Ct, PA_Vegetale_Imbottigliamento_Cp, " & vbCrLf)
    '        StrSQL.Append("             PA_Vegetale_Imbottigliamento_Ct, PA_Vegetale_ProdottiErboristici_Cp, PA_Vegetale_ProdottiErboristici_Ct, PA_ANIMALE_Cp, PA_ANIMALE_Ct, " & vbCrLf)
    '        StrSQL.Append("             PA_Animale_Macellazione_Cp, PA_Animale_Macellazione_Ct, PA_Animale_Sezionamento_Cp, PA_Animale_Sezionamento_Ct, " & vbCrLf)
    '        StrSQL.Append("             PA_Animale_DerivatiCarne_Cp, PA_Animale_DerivatiCarne_Ct, PA_Animale_ConserveAnimali_Cp, PA_Animale_ConserveAnimali_Ct, " & vbCrLf)
    '        StrSQL.Append("             PA_Animale_ProdottiSalumeria_Cp, PA_Animale_ProdottiSalumeria_Ct, PA_Animale_LatteAlimentare_Cp, PA_Animale_LatteAlimentare_Ct, " & vbCrLf)
    '        StrSQL.Append("             PA_Animale_Caseificazione_Cp, PA_Animale_Caseificazione_Ct, PA_Animale_Burro_Cp, PA_Animale_Burro_Ct, PA_Animale_Yogurt_Cp, " & vbCrLf)
    '        StrSQL.Append("             PA_Animale_Yogurt_Ct, PA_Animale_Altro_Cp, PA_Animale_Altro_Ct, PA_Animale_AltroDes, PA_INDUSTRIADOLCIARIA_Cp, " & vbCrLf)
    '        StrSQL.Append("             PA_INDUSTRIADOLCIARIA_Ct, PA_IndustriaDolciaria_ProdottiDolciari_Cp, PA_IndustriaDolciaria_ProdottiDolciari_Ct, " & vbCrLf)
    '        StrSQL.Append("             PA_IndustriaDolciaria_AltriProdotti_Cp, PA_IndustriaDolciaria_AltriProdotti_Ct, PA_MANGIMI_Cp, PA_MANGIMI_Ct, PA_ETICHETTATURA_Cp, " & vbCrLf)
    '        StrSQL.Append("             PA_ETICHETTATURA_Ct, PA_ALTRO_Cp, PA_ALTRO_Ct, PA_ALTRODes, CaratteristichePA_TipoDes, CaratteristichePA_Flag_Periodica_Continuativa, " & vbCrLf)
    '        StrSQL.Append("             CaratteristichePA_CapacitaLavoro, CaratteristichePA_CapacitaLavoro_UdmCod, CaratteristichePA_CapacitaLavoroTempo_UdmCod, " & vbCrLf)
    '        StrSQL.Append("             CaratteristichePA_CapacitaStoccaggio, CaratteristichePA_CapacitaStoccaggio_UdmCod,  " & vbCrLf)

    '        'Aggiuntive nuova notifica
    '        StrSQL.Append("             PA_VEGETALE_Cm, PA_Vegetale_Ortofrutticoli_Cp, PA_Vegetale_Ortofrutticoli_Ct, PA_Vegetale_Ortofrutticoli_Cm, PA_Vegetale_Molinatura_Cm, PA_Vegetale_Fioccatura_Cm, " & vbCrLf)
    '        StrSQL.Append("             PA_Vegetale_Pastificazione_Cm, PA_Vegetale_Surgelati_Cp, PA_Vegetale_Surgelati_Ct, PA_Vegetale_Surgelati_Cm, PA_Vegetale_Conserve_Cm, PA_Vegetale_IntegratoriAlimentari_Cm, " & vbCrLf)
    '        StrSQL.Append("             PA_Vegetale_EstrazioneOlio_Cm, PA_Vegetale_Vinificazione_Cm, PA_Vegetale_Liquori_Cm, PA_Vegetale_Imbottigliamento_Cm, PA_Vegetale_ProdottiErboristici_Cm, PA_Vegetale_AltroDes, " & vbCrLf)
    '        StrSQL.Append("             PA_Vegetale_AltroDes_Cp, PA_Vegetale_AltroDes_Ct, PA_Vegetale_AltroDes_Cm, PA_ANIMALE_Cm, PA_Animale_Porzionatura_Cp, PA_Animale_Porzionatura_Ct, PA_Animale_Porzionatura_Cm, " & vbCrLf)
    '        StrSQL.Append("             PA_Animale_Macellazione_Cm, PA_Animale_Sezionamento_Cm, PA_Animale_DerivatiCarne_Cm, PA_Animale_ConserveAnimali_Cm, PA_Animale_ProdottiSalumeria_Cm, PA_Animale_LatteAlimentare_Cm, " & vbCrLf)
    '        StrSQL.Append("             PA_Animale_Caseificazione_Cm, PA_Animale_Burro_Cm, PA_Animale_Yogurt_Cm, PA_Animale_Uova_Cp, PA_Animale_Uova_Ct, PA_Animale_Uova_Cm, PA_Animale_Altro_Cm, PA_IndustriaDolciaria_DaForno_Cp, " & vbCrLf)
    '        StrSQL.Append("             PA_IndustriaDolciaria_DaForno_Ct, PA_IndustriaDolciaria_DaForno_Cm, PA_IndustriaDolciaria_AltriProdotti_Cm, PA_MANGIMI_Cm, PA_Mangimi_AltroDes, PA_Mangimi_AltroDes_Cp, PA_Mangimi_AltroDes_Ct, " & vbCrLf)
    '        StrSQL.Append("             PA_Mangimi_AltroDes_Cm, PA_IMMAGAZZINAMENTO_Cp, PA_IMMAGAZZINAMENTO_Ct, PA_CONSERVAZIONE_Cp, PA_CONSERVAZIONE_Ct, PA_CONDIZIONAMENTO_Cp, PA_CONDIZIONAMENTO_Ct, PA_CONFEZIONAMENTO_Cp, " & vbCrLf)
    '        StrSQL.Append("             PA_CONFEZIONAMENTO_Ct, CaratteristichePA_Periodica_Bio, CaratteristichePA_Continuativa_Bio, CaratteristichePA_Periodica_Conv, CaratteristichePA_Continuativa_Conv, STRUTTURE_SILI_mc, " & vbCrLf)
    '        StrSQL.Append("             STRUTTURE_SILI_StoccaggioCereali, STRUTTURE_SILI_StoccaggioProteoleaginose, STRUTTURE_SILI_Altro_Des, STRUTTURE_SILI_Altro, STRUTTURE_CELLE_mc, STRUTTURE_CELLE_Vegetali, STRUTTURE_CELLE_Zootecnici, " & vbCrLf)
    '        StrSQL.Append("             STRUTTURE_CELLE_Altro_Des, STRUTTURE_CELLE_Altro, STRUTTURE_UTILIZZO_Dedicato, STRUTTURE_UTILIZZO_Misto, COMMERCIO_Ingrosso, COMMERCIO_Dettagliante, COMMERCIO_GDO, COMMERCIO_Amarchio, COMMERCIO_Altro, " & vbCrLf)
    '        StrSQL.Append("             COMMERCIO_PERIODICA_Bio, COMMERCIO_PERIODICA_Conv, COMMERCIO_CONTINUATIVA_Bio, COMMERCIO_CONTINUATIVA_Conv, COMMERCIO_AMARCHIO_Des, " & vbCrLf)



    '        StrSQL.Append("              Inviato,            datainvio, " & vbCrLf)
    '        StrSQL.Append("              Data_Creazione,     Data_Modifica, " & vbCrLf)
    '        StrSQL.Append("              UserName_Creazione, UserName_Modifica, " & vbCrLf)
    '        StrSQL.Append("              Validita_Inizio,    Validita_Fine  , DataLock " & vbCrLf)
    '        StrSQL.Append("              ) ")

    '        StrSQL.Append(" VALUES (")
    '        StrSQL.Append(" '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(Notifica_ID)) + "" & vbCrLf)

    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(CaratteristichePA_Numero)) + " " & vbCrLf)
    '        StrSQL.Append(" '" & Agro_SQL_SaveText(Trim(CentroPreparazione_Piva)) + "' " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(CentroPreparazione_SaCod)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(CentroPreparazionePiva_Flag_Nostampa_Stampa)) + " " & vbCrLf)
    '        StrSQL.Append(" '" & Agro_SQL_SaveText(Trim(CentroPreparazione_RagSoc)) + "' " & vbCrLf)
    '        StrSQL.Append(" '" & Agro_SQL_SaveText(Trim(CentroPreparazione_Via)) + "' " & vbCrLf)
    '        StrSQL.Append(" '" & Agro_SQL_SaveText(Trim(CentroPreparazione_Numero)) + "' " & vbCrLf)
    '        StrSQL.Append(" '" & Agro_SQL_SaveText(Trim(CentroPreparazione_CAP)) + "' " & vbCrLf)
    '        StrSQL.Append(" '" & Agro_SQL_SaveText(Trim(CentroPreparazione_CodIstat_Provincia)) + "' " & vbCrLf)
    '        StrSQL.Append(" '" & Agro_SQL_SaveText(Trim(CentroPreparazione_CodIstat_Comune)) + "' " & vbCrLf)
    '        StrSQL.Append(" '" & Agro_SQL_SaveText(Trim(CentroPreparazione_Telefono)) + "' " & vbCrLf)
    '        StrSQL.Append(" '" & Agro_SQL_SaveText(Trim(CentroPreparazione_Fax)) + "' " & vbCrLf)
    '        StrSQL.Append(" '" & Agro_SQL_SaveText(Trim(CentroPreparazione_Email)) + "' " & vbCrLf)
    '        StrSQL.Append(" '" & Agro_SQL_SaveText(Trim(CentroPreparazione_CodAUSL)) + "' " & vbCrLf)
    '        StrSQL.Append(" '" & Agro_SQL_SaveText(Trim(CentroPreparazione_Comune)) + "' " & vbCrLf)
    '        StrSQL.Append(" '" & Agro_SQL_SaveText(Trim(CentroPreparazione_Sigla_Provincia)) + "' " & vbCrLf)
    '        StrSQL.Append(" '" & Agro_SQL_SaveText(Trim(CentroPreparazione_Provincia)) + "' " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_VEGETALE_Cp)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_VEGETALE_Ct)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Vegetale_Molinatura_Cp)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Vegetale_Molinatura_Ct)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Vegetale_Fioccatura_Cp)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Vegetale_Fioccatura_Ct)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Vegetale_Pastificazione_Cp)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Vegetale_Pastificazione_Ct)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Vegetale_Conserve_Cp)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Vegetale_Conserve_Ct)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Vegetale_IntegratoriAlimentari_Cp)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Vegetale_IntegratoriiAlimentari_Ct)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Vegetale_EstrazioneOlio_Cp)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Vegetale_EstrazioneOlio_Ct)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Vegetale_Vinificazione_Cp)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Vegetale_Vinificazione_Ct)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Vegetale_Liquori_Cp)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Vegetale_Liquori_Ct)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Vegetale_Imbottigliamento_Ct)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Vegetale_Imbottigliamento_Cp)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Vegetale_ProdottiErboristici_Ct)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Vegetale_ProdottiErboristici_Cp)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_ANIMALE_Cp)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_ANIMALE_Ct)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Animale_Macellazione_Cp)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Animale_Macellazione_Ct)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Animale_Sezionamento_Cp)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Animale_Sezionamento_Ct)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Animale_DerivatiCarne_Cp)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Animale_DerivatiCarne_Ct)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Animale_ConserveAnimali_Cp)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Animale_ConserveAnimali_Ct)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Animale_ProdottiSalumeria_Cp)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Animale_ProdottiSalumeria_Ct)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Animale_LatteAlimentare_Cp)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Animale_LatteAlimentare_Ct)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Animale_Caseificazione_Cp)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Animale_Caseificazione_Ct)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Animale_Burro_Cp)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Animale_Burro_Ct)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Animale_Yogurt_Cp)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Animale_Yogurt_Ct)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Animale_Altro_Cp)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Animale_Altro_Ct)) + " " & vbCrLf)
    '        StrSQL.Append(" '" & Agro_SQL_SaveText(Trim(PA_Animale_AltroDes)) + "' " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_INDUSTRIADOLCIARIA_Cp)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_INDUSTRIADOLCIARIA_Ct)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_IndustriaDolciaria_ProdottiDolciari_Cp)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_IndustriaDolciaria_ProdottiDolciari_Ct)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_IndustriaDolciaria_AltriProdotti_Cp)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_IndustriaDolciaria_AltriProdotti_Ct)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_MANGIMI_Cp)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_MANGIMI_Ct)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_ETICHETTATURA_Cp)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_ETICHETTATURA_Ct)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_ALTRO_Cp)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_ALTRO_Ct)) + " " & vbCrLf)
    '        StrSQL.Append(" '" & Agro_SQL_SaveText(Trim(PA_ALTRODes)) + "' " & vbCrLf)
    '        StrSQL.Append(" '" & Agro_SQL_SaveText(Trim(CaratteristichePA_TipoDes)) + "' " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(CaratteristichePA_Flag_Periodica_Continuativa)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(CaratteristichePA_CapacitaLavoro)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(CaratteristichePA_CapacitaLavoro_UdmCod)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(CaratteristichePA_CapacitaLavoroTempo_UdmCod)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(CaratteristichePA_CapacitaStoccaggio)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(CaratteristichePA_CapacitaStoccaggio_UdmCod)) + " " & vbCrLf)

    '        'Aggiuntive nuova notifica
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_VEGETALE_Cm)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Vegetale_Ortofrutticoli_Cp)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Vegetale_Ortofrutticoli_Ct)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Vegetale_Ortofrutticoli_Cm)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Vegetale_Molinatura_Cm)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Vegetale_Fioccatura_Cm)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Vegetale_Pastificazione_Cm)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Vegetale_Surgelati_Cp)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Vegetale_Surgelati_Ct)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Vegetale_Surgelati_Cm)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Vegetale_Conserve_Cm)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Vegetale_IntegratoriAlimentari_Cm)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Vegetale_EstrazioneOlio_Cm)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Vegetale_Vinificazione_Cm)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Vegetale_Liquori_Cm)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Vegetale_Imbottigliamento_Cm)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Vegetale_ProdottiErboristici_Cm)) + " " & vbCrLf)
    '        StrSQL.Append(" '" & Agro_SQL_SaveText(Trim(PA_Vegetale_AltroDes)) + "' " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Vegetale_AltroDes_Cp)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Vegetale_AltroDes_Ct)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Vegetale_AltroDes_Cm)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_ANIMALE_Cm)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Animale_Porzionatura_Cp)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Animale_Porzionatura_Ct)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Animale_Porzionatura_Cm)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Animale_Macellazione_Cm)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Animale_Sezionamento_Cm)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Animale_DerivatiCarne_Cm)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Animale_ConserveAnimali_Cm)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Animale_ProdottiSalumeria_Cm)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Animale_LatteAlimentare_Cm)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Animale_Caseificazione_Cm)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Animale_Burro_Cm)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Animale_Yogurt_Cm)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Animale_Uova_Cp)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Animale_Uova_Ct)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Animale_Uova_Cm)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Animale_Altro_Cm)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_IndustriaDolciaria_DaForno_Cp)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_IndustriaDolciaria_DaForno_Ct)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_IndustriaDolciaria_DaForno_Cm)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_IndustriaDolciaria_AltriProdotti_Cm)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_MANGIMI_Cm)) + " " & vbCrLf)
    '        StrSQL.Append(" '" & Agro_SQL_SaveText(Trim(PA_Mangimi_AltroDes)) + "' " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Mangimi_AltroDes_Cp)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Mangimi_AltroDes_Ct)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_Mangimi_AltroDes_Cm)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_IMMAGAZZINAMENTO_Cp)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_IMMAGAZZINAMENTO_Ct)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_CONSERVAZIONE_Cp)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_CONSERVAZIONE_Ct)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_CONDIZIONAMENTO_Cp)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_CONDIZIONAMENTO_Ct)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_CONFEZIONAMENTO_Cp)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(PA_CONFEZIONAMENTO_Ct)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(CaratteristichePA_Periodica_Bio)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(CaratteristichePA_Continuativa_Bio)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(CaratteristichePA_Periodica_Conv)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(CaratteristichePA_Continuativa_Conv)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(STRUTTURE_SILI_mc)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(STRUTTURE_SILI_StoccaggioCereali)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(STRUTTURE_SILI_StoccaggioProteoleaginose)) + " " & vbCrLf)
    '        StrSQL.Append(" '" & Agro_SQL_SaveText(Trim(STRUTTURE_SILI_Altro_Des)) + "' " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(STRUTTURE_SILI_Altro)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(STRUTTURE_CELLE_mc)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(STRUTTURE_CELLE_Vegetali)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(STRUTTURE_CELLE_Zootecnici)) + " " & vbCrLf)
    '        StrSQL.Append(" '" & Agro_SQL_SaveText(Trim(STRUTTURE_CELLE_Altro_Des)) + "' " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(STRUTTURE_CELLE_Altro)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(STRUTTURE_UTILIZZO_Dedicato)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(STRUTTURE_UTILIZZO_Misto)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(COMMERCIO_Ingrosso)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(COMMERCIO_Dettagliante)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(COMMERCIO_GDO)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(COMMERCIO_Amarchio)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(COMMERCIO_Altro)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(COMMERCIO_PERIODICA_Bio)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(COMMERCIO_PERIODICA_Conv)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(COMMERCIO_CONTINUATIVA_Bio)) + " " & vbCrLf)
    '        StrSQL.Append(" ," + Agro_SQL_SaveNum(Trim(COMMERCIO_CONTINUATIVA_Conv)) + " " & vbCrLf)
    '        StrSQL.Append(" '" & Agro_SQL_SaveText(Trim(COMMERCIO_AMARCHIO_Des)) + "' " & vbCrLf)

    '        StrSQL.Append("         , 0  ")
    '        StrSQL.Append("         , Null  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
    '        StrSQL.Append("         , 0  ")
    '        StrSQL.Append(") ")

    '        '--------------------------------------------------------------------------
    '        xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
    '        '--------------------------------------------------------------------------

    '    Catch ex As Exception
    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
    '    End Try

    '    Return xRisp

    'End Function

    '#########################################################
    Public Function Cancella(ByVal Notifica_ID As Int32,
                             ByVal CaratteristichePA_Numero As Int32,
                             ByVal CentroPreparazione_Piva As String,
                             ByVal CentroPreparazione_SaCod As Int32,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreBiologicoDAL.BIO_Notifica_SezC_PreparazioniAlimentari_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            If Notifica_ID = 0 Then
                Throw New Exception("Parametro non corretto nella query (Notifica_ID obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                StrSQL.Length = 0
                StrSQL.Append(" UPDATE BIO_Notifica_SezC_PreparazioniAlimentari ")
                StrSQL.Append(" SET ")
                StrSQL.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE   Notifica_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND     Notifica_ID = " & Agro_SQL_SaveNum(Notifica_ID) & " ")
                StrSQL.Append(" AND     Inviato >= 0 ")
            Else
                StrSQL.Append(" DELETE FROM BIO_Notifica_SezC_PreparazioniAlimentari ")
                StrSQL.Append(" WHERE Notifica_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND Notifica_ID = " & Agro_SQL_SaveNum(Notifica_ID) & "  ")
            End If
            '---------------------------------------------

            If CaratteristichePA_Numero <> 0 Then
                StrSQL.Append(" AND CaratteristichePA_Numero = " & Agro_SQL_SaveNum(CaratteristichePA_Numero) & "  ")
            End If

            If CentroPreparazione_Piva <> "" Then
                StrSQL.Append(" AND CentroPreparazione_Piva = '" & Agro_SQL_SaveText(CentroPreparazione_Piva) & "'  ")
            End If

            If CentroPreparazione_SaCod <> 0 Then
                StrSQL.Append(" AND CentroPreparazione_SaCod = " & Agro_SQL_SaveNum(CentroPreparazione_SaCod) & "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

End Class
