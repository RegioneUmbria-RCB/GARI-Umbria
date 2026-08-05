Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class BIO_Notifica_SezA_Informazioni_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Enum Enum_IndirizzoProduttivo_Valore
        Nessuno = 0
        Convenzionale = 1
        Conversione = 2
        Biologico = 3
        Convenzionale_Biologico = 4
    End Enum

    '##############################################################################################
    Public Enum Enum_StrutturaStoccaggio_Valore
        Nessuno = 0
        Convenzionale = 1
        Conversione = 2
        Biologico = 3
        Convenzionale_Biologico = 4
    End Enum

    '##############################################################################################
    Public Function IndirizzoProduttivoValore_from_BioConv(ByVal bio As String,
                                                           ByVal conv As String
                                                           ) As Enum_IndirizzoProduttivo_Valore

        If bio.ToUpper = "S" AndAlso conv.ToUpper = "S" Then
            Return Enum_IndirizzoProduttivo_Valore.Convenzionale_Biologico
        Else
            If bio.ToUpper = "S" Then
                Return Enum_IndirizzoProduttivo_Valore.Biologico
            Else
                If conv.ToUpper = "S" Then
                    Return Enum_IndirizzoProduttivo_Valore.Convenzionale
                Else
                    Return Enum_IndirizzoProduttivo_Valore.Nessuno
                End If
            End If
        End If

    End Function

    '##############################################################################################
    Public Function StrutturaStoccaggioValore_from_BioConv(ByVal bio As String,
                                                           ByVal conv As String
                                                           ) As Enum_StrutturaStoccaggio_Valore

        If bio.ToUpper = "S" AndAlso conv.ToUpper = "S" Then
            Return Enum_StrutturaStoccaggio_Valore.Convenzionale_Biologico
        Else
            If bio.ToUpper = "S" Then
                Return Enum_StrutturaStoccaggio_Valore.Biologico
            Else
                If conv.ToUpper = "S" Then
                    Return Enum_StrutturaStoccaggio_Valore.Convenzionale
                Else
                    Return Enum_StrutturaStoccaggio_Valore.Nessuno
                End If
            End If
        End If

    End Function

    '##############################################################################################
    Public Function Leggi(ByVal Notifica_ID As Integer, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreBiologicoDAL.BIO_Notifica_SezA_Informazioni_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT BIO_Notifica_SezA_Informazioni.* ")
            StrSQL.Append(" FROM  BIO_Notifica_SezA_Informazioni ")
            StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Notifica_ID <> 0 Then
                StrSQL.Append(" AND BIO_Notifica_SezA_Informazioni.Notifica_ID = " & Agro_SQL_SaveNum(Notifica_ID) & " ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   BIO_Notifica_SezA_Informazioni.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   BIO_Notifica_SezA_Informazioni.Inviato =-1 ")
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

Public Class BIO_Notifica_SezA_Informazioni_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    'Data_Creazione e username_creazione sono passati come parametri per tenere traccia 
    'dei dati originali di creazione
    '(la modifica avviene con cancella e scrivi)
    Public Function Scrivi(ByVal Notifica_ID As Integer,
                            ByVal UnitaProduttiva_Piva As String,
                            ByVal UnitaProduttiva_RagSoc As String,
                            ByVal UnitaProduttiva_CodiceOperatore As String,
                            ByVal UnitaProduttiva_Via As String,
                            ByVal UnitaProduttiva_Numero As String,
                            ByVal UnitaProduttiva_CAP As String,
                            ByVal UnitaProduttiva_CodIstat_Provincia As String,
                            ByVal UnitaProduttiva_CodIstat_Comune As String,
                            ByVal UnitaProduttiva_Telefono As String,
                            ByVal UnitaProduttiva_Fax As String,
                            ByVal UnitaProduttiva_Email As String,
                            ByVal Sup_Catastale As Decimal,
                            ByVal Sup_Bosco As Decimal,
                            ByVal Sup_Tare As Decimal,
                            ByVal Sup_PratiPascoli As Decimal,
                            ByVal Sup_SAU_Totale As Decimal,
                            ByVal Sup_SAU_Convenzionale As Decimal,
                            ByVal Sup_SAU_Conversione As Decimal,
                            ByVal Sup_SAU_Biologico As Decimal,
                            ByVal IP_Cerealicolo As Integer,
                            ByVal IP_Cerealicolo_Riso As Integer,
                            ByVal IP_Cerealicolo_GranoDuro As Integer,
                            ByVal IP_Cerealicolo_GranoTenero As Integer,
                            ByVal IP_Cerealicolo_Mais As Integer,
                            ByVal IP_Cerealicolo_AltriCereali As Integer,
                            ByVal IP_Orticolo As Integer,
                            ByVal IP_Orticolo_PienoCampo As Integer,
                            ByVal IP_Orticolo_ColturaProtetta As Integer,
                            ByVal IP_ColtureIndustriali As Integer,
                            ByVal IP_Frutticolo As Integer,
                            ByVal IP_Frutticolo_Pomacee As Integer,
                            ByVal IP_Frutticolo_Drupacee As Integer,
                            ByVal IP_Frutticolo_Agrumi As Integer,
                            ByVal IP_Frutticolo_FruttaSecca As Integer,
                            ByVal IP_Frutticolo_UvaTavola As Integer,
                            ByVal IP_Frutticolo_Altro As Integer,
                            ByVal IP_Frutticolo_Altro_Des As String,
                            ByVal IP_Vitivinicolo As Integer,
                            ByVal IP_Vitivinicolo_DaTavola As Integer,
                            ByVal IP_Vitivinicolo_Denominazione As Integer,
                            ByVal IP_Olivicolo As Integer,
                            ByVal IP_Olivicolo_DaMensa As Integer,
                            ByVal IP_Olivicolo_DaOlio As Integer,
                            ByVal IP_Foraggero As Integer,
                            ByVal IP_VivaisticoSementiero As Integer,
                            ByVal IP_Altro As Integer,
                            ByVal IP_Altro_Des As String,
                            ByVal S_Magazzini_Aziendali As Integer,
                            ByVal S_Magazzini_Esterni As Integer,
                            ByVal S_Magazzini_xMezziTecnici As Integer,
                            ByVal S_Magazzini_xAttrezzature As Integer,
                            ByVal S_Magazzini_xVenditaProdotti As Integer,
                            ByVal S_Magazzini_Altro As Integer,
                            ByVal S_Magazzini_Altro_Des As String,
                            ByVal S_Sili_Aziendali As Integer,
                            ByVal S_Sili_Esterni As Integer,
                            ByVal S_Sili_StockGranaglie As Integer,
                            ByVal S_Sili_StockColtureIndustriali As Integer,
                            ByVal S_Sili_StockMangimi As Integer,
                            ByVal S_Sili_PreparazioneInsilati As Integer,
                            ByVal S_Sili_Altro As Integer,
                            ByVal S_Sili_Altro_Des As String,
                            ByVal S_Celle_Aziendali As Integer,
                            ByVal S_Celle_Esterne As Integer,
                            ByVal S_Celle_ProdVegetali As Integer,
                            ByVal S_Celle_ProdZootecniche As Integer,
                            ByVal S_Celle_Altro As Integer,
                            ByVal S_Celle_Altro_Des As String,
                            ByVal S_Impianti_Aziendali As Integer,
                            ByVal S_Impianti_Esterni As Integer,
                            ByVal S_Impianti_Altro As Integer,
                            ByVal S_Impianti_Altro_Des As String,
                            ByVal IR_CEREALI As Integer,
                            ByVal IR_Cereali_Granella As Integer,
                            ByVal IR_Cereali_Conservazione As Integer,
                            ByVal IR_Cereali_Sfarinati As Integer,
                            ByVal IR_Cereali_Pastificazione As Integer,
                            ByVal IR_Cereali_Panificazione As Integer,
                            ByVal IR_Cereali_ProdottiDaForno As Integer,
                            ByVal IR_Cereali_AltriProdotti As Integer,
                            ByVal IR_COLTUREIND As Integer,
                            ByVal IR_ColtureInd_Granella As Integer,
                            ByVal IR_ColtureInd_Sfarinati As Integer,
                            ByVal IR_ColtureIndi_EstrazioneOlio As Integer,
                            ByVal IR_ColtureInd_Conservazione As Integer,
                            ByVal IR_ColtureInd_Confezionamento As Integer,
                            ByVal IR_ColtureInd_AltriProdotti As Integer,
                            ByVal IR_ORTOFRUTTICOLI As Integer,
                            ByVal IR_Ortofrutticoli_Freschi As Integer,
                            ByVal IR_Ortofrutticoli_ConserveVegetali As Integer,
                            ByVal IR_Ortofrutticoli_Conservazione As Integer,
                            ByVal IR_Ortofrutticoli_Confezionamento As Integer,
                            ByVal IR_VITIVINICOLO As Integer,
                            ByVal IR_Vitivinicolo_Vinificazione As Integer,
                            ByVal IR_Vitivinicolo_Mostificazione As Integer,
                            ByVal IR_Vitivinicolo_Imbottigliamento As Integer,
                            ByVal IR_OLEICOLO As Integer,
                            ByVal IR_Oleicolo_ConserveVegetali As Integer,
                            ByVal IR_Oleicolo_EstrazioneOlio As Integer,
                            ByVal IR_Oleicolo_Imbottigliamento As Integer,
                            ByVal IR_VIVSEMENTIERO As Integer,
                            ByVal IR_VivSementiero_Semi As Integer,
                            ByVal IR_VivSementiero_OrticoleTrapianto As Integer,
                            ByVal IR_VivSementiero_Astoni As Integer,
                            ByVal IR_VivSementiero_Barbatelle As Integer,
                            ByVal IR_VivSementiero_Altro As Integer,
                            ByVal IR_PIANTE_OFFICINALI As Integer,
                            ByVal IR_PRODOTTI_SPONTANEI As Integer,
                            ByVal Username_Creazione As String,
                               ByVal Data_Creazione As Date,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                , Optional ByVal Data_modifica As Date = #2/1/1900# _
                , Optional ByVal username_modifica As String = ""
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreBiologicoDAL.BIO_Notifica_SezA_Informazioni_W.Scrivi()"

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
            StrSQL.Append(" INSERT INTO BIO_Notifica_SezA_Informazioni " & vbCrLf)

            StrSQL.Append("             (Notifica_SuperUser, " & vbCrLf)
            StrSQL.Append("             Notifica_ID, " & vbCrLf)
            StrSQL.Append("              UnitaProduttiva_Piva, UnitaProduttiva_RagSoc, UnitaProduttiva_CodiceOperatore, UnitaProduttiva_Via, " & vbCrLf)
            StrSQL.Append("              UnitaProduttiva_Numero, UnitaProduttiva_CAP, UnitaProduttiva_CodIstat_Provincia, UnitaProduttiva_CodIstat_Comune, UnitaProduttiva_Telefono, " & vbCrLf)
            StrSQL.Append("              UnitaProduttiva_Fax, UnitaProduttiva_Email, Sup_Catastale, Sup_Bosco, Sup_Tare, Sup_PratiPascoli, Sup_SAU_Totale, Sup_SAU_Convenzionale, " & vbCrLf)
            StrSQL.Append("              Sup_SAU_Conversione, Sup_SAU_Biologico, IP_Cerealicolo, IP_Cerealicolo_Riso, IP_Cerealicolo_GranoDuro, IP_Cerealicolo_GranoTenero, " & vbCrLf)
            StrSQL.Append("              IP_Cerealicolo_Mais, IP_Cerealicolo_AltriCereali, IP_Orticolo, IP_Orticolo_PienoCampo, IP_Orticolo_ColturaProtetta, IP_ColtureIndustriali, IP_Frutticolo, " & vbCrLf)
            StrSQL.Append("              IP_Frutticolo_Pomacee, IP_Frutticolo_Drupacee, IP_Frutticolo_Agrumi, IP_Frutticolo_FruttaSecca, IP_Frutticolo_UvaTavola, IP_Frutticolo_Altro, " & vbCrLf)
            StrSQL.Append("              IP_Frutticolo_Altro_Des, IP_Vitivinicolo, IP_Vitivinicolo_DaTavola, IP_Vitivinicolo_Denominazione, IP_Olivicolo, IP_Olivicolo_DaMensa, " & vbCrLf)
            StrSQL.Append("              IP_Olivicolo_DaOlio, IP_Foraggero, IP_VivaisticoSementiero, IP_Altro, IP_Altro_Des, S_Magazzini_Aziendali, S_Magazzini_Esterni, " & vbCrLf)
            StrSQL.Append("              S_Magazzini_xMezziTecnici, S_Magazzini_xAttrezzature, S_Magazzini_xVenditaProdotti, S_Magazzini_Altro, S_Magazzini_Altro_Des, " & vbCrLf)
            StrSQL.Append("              S_Sili_Aziendali, S_Sili_Esterni, S_Sili_StockGranaglie, S_Sili_StockColtureIndustriali, S_Sili_StockMangimi, S_Sili_PreparazioneInsilati, S_Sili_Altro, " & vbCrLf)
            StrSQL.Append("              S_Sili_Altro_Des, S_Celle_Aziendali, S_Celle_Esterne, S_Celle_ProdVegetali, S_Celle_ProdZootecniche, S_Celle_Altro, S_Celle_Altro_Des, " & vbCrLf)
            StrSQL.Append("              S_Impianti_Aziendali, S_Impianti_Esterni, S_Impianti_Altro, S_Impianti_Altro_Des, IR_CEREALI, IR_Cereali_Granella, IR_Cereali_Conservazione, " & vbCrLf)
            StrSQL.Append("              IR_Cereali_Sfarinati, IR_Cereali_Pastificazione, IR_Cereali_Panificazione, IR_Cereali_ProdottiDaForno, IR_Cereali_AltriProdotti, IR_COLTUREIND, " & vbCrLf)
            StrSQL.Append("              IR_ColtureInd_Granella, IR_ColtureInd_Sfarinati, IR_ColtureIndi_EstrazioneOlio, IR_ColtureInd_Conservazione, IR_ColtureInd_Confezionamento, " & vbCrLf)
            StrSQL.Append("              IR_ColtureInd_AltriProdotti, IR_ORTOFRUTTICOLI, IR_Ortofrutticoli_Freschi, IR_Ortofrutticoli_ConserveVegetali, IR_Ortofrutticoli_Conservazione, " & vbCrLf)
            StrSQL.Append("              IR_Ortofrutticoli_Confezionamento, IR_VITIVINICOLO, IR_Vitivinicolo_Vinificazione, IR_Vitivinicolo_Mostificazione, IR_Vitivinicolo_Imbottigliamento, " & vbCrLf)
            StrSQL.Append("              IR_OLEICOLO, IR_Oleicolo_ConserveVegetali, IR_Oleicolo_EstrazioneOlio, IR_Oleicolo_Imbottigliamento, IR_VIVSEMENTIERO, " & vbCrLf)
            StrSQL.Append("              IR_VivSementiero_Semi, IR_VivSementiero_OrticoleTrapianto, IR_VivSementiero_Astoni, IR_VivSementiero_Barbatelle, IR_VivSementiero_Altro, " & vbCrLf)
            StrSQL.Append("              IR_PIANTE_OFFICINALI, IR_PRODOTTI_SPONTANEI, " & vbCrLf)

            StrSQL.Append("              Inviato,            datainvio, ")
            StrSQL.Append("              Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("              UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("              Validita_Inizio,    Validita_Fine  , DataLock ")
            StrSQL.Append("              ) ")

            StrSQL.Append(" VALUES (")
            StrSQL.Append("             '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            StrSQL.Append("             ," & Agro_SQL_SaveNum(Trim(Notifica_ID)) & ", " & vbCrLf)

            StrSQL.Append("             '" & Agro_SQL_SaveText(Trim(UnitaProduttiva_Piva)) & "', " & vbCrLf)
            StrSQL.Append("             '" & Agro_SQL_SaveText(Trim(UnitaProduttiva_RagSoc)) & "', " & vbCrLf)
            StrSQL.Append("             '" & Agro_SQL_SaveText(Trim(UnitaProduttiva_CodiceOperatore)) & "', " & vbCrLf)
            StrSQL.Append("             '" & Agro_SQL_SaveText(Trim(UnitaProduttiva_Via)) & "', " & vbCrLf)
            StrSQL.Append("             '" & Agro_SQL_SaveText(Trim(UnitaProduttiva_Numero)) & "', " & vbCrLf)
            StrSQL.Append("             '" & Agro_SQL_SaveText(Trim(UnitaProduttiva_CAP)) & "', " & vbCrLf)
            StrSQL.Append("             '" & Agro_SQL_SaveText(Trim(UnitaProduttiva_CodIstat_Provincia)) & "', " & vbCrLf)
            StrSQL.Append("             '" & Agro_SQL_SaveText(Trim(UnitaProduttiva_CodIstat_Comune)) & "', " & vbCrLf)
            StrSQL.Append("             '" & Agro_SQL_SaveText(Trim(UnitaProduttiva_Telefono)) & "', " & vbCrLf)
            StrSQL.Append("             '" & Agro_SQL_SaveText(Trim(UnitaProduttiva_Fax)) & "', " & vbCrLf)
            StrSQL.Append("             '" & Agro_SQL_SaveText(Trim(UnitaProduttiva_Email)) & "', " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(Sup_Catastale)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(Sup_Bosco)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(Sup_Tare)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(Sup_PratiPascoli)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(Sup_SAU_Totale)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(Sup_SAU_Convenzionale)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(Sup_SAU_Conversione)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(Sup_SAU_Biologico)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IP_Cerealicolo)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IP_Cerealicolo_Riso)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IP_Cerealicolo_GranoDuro)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IP_Cerealicolo_GranoTenero)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IP_Cerealicolo_Mais)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IP_Cerealicolo_AltriCereali)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IP_Orticolo)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IP_Orticolo_PienoCampo)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IP_Orticolo_ColturaProtetta)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IP_ColtureIndustriali)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IP_Frutticolo)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IP_Frutticolo_Pomacee)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IP_Frutticolo_Drupacee)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IP_Frutticolo_Agrumi)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IP_Frutticolo_FruttaSecca)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IP_Frutticolo_UvaTavola)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IP_Frutticolo_Altro)) & ", " & vbCrLf)
            StrSQL.Append("             '" & Agro_SQL_SaveText(Trim(IP_Frutticolo_Altro_Des)) & "', " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IP_Vitivinicolo)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IP_Vitivinicolo_DaTavola)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IP_Vitivinicolo_Denominazione)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IP_Olivicolo)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IP_Olivicolo_DaMensa)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IP_Olivicolo_DaOlio)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IP_Foraggero)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IP_VivaisticoSementiero)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IP_Altro)) & ", " & vbCrLf)
            StrSQL.Append("             '" & Agro_SQL_SaveText(Trim(IP_Altro_Des)) & "', " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(S_Magazzini_Aziendali)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(S_Magazzini_Esterni)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(S_Magazzini_xMezziTecnici)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(S_Magazzini_xAttrezzature)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(S_Magazzini_xVenditaProdotti)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(S_Magazzini_Altro)) & ", " & vbCrLf)
            StrSQL.Append("             '" & Agro_SQL_SaveText(Trim(S_Magazzini_Altro_Des)) & "', " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(S_Sili_Aziendali)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(S_Sili_Esterni)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(S_Sili_StockGranaglie)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(S_Sili_StockColtureIndustriali)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(S_Sili_StockMangimi)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(S_Sili_PreparazioneInsilati)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(S_Sili_Altro)) & ", " & vbCrLf)
            StrSQL.Append("             '" & Agro_SQL_SaveText(Trim(S_Sili_Altro_Des)) & "', " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(S_Celle_Aziendali)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(S_Celle_Esterne)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(S_Celle_ProdVegetali)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(S_Celle_ProdZootecniche)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(S_Celle_Altro)) & ", " & vbCrLf)
            StrSQL.Append("             '" & Agro_SQL_SaveText(Trim(S_Celle_Altro_Des)) & "', " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(S_Impianti_Aziendali)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(S_Impianti_Esterni)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(S_Impianti_Altro)) & ", " & vbCrLf)
            StrSQL.Append("             '" & Agro_SQL_SaveText(Trim(S_Impianti_Altro_Des)) & "', " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IR_CEREALI)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IR_Cereali_Granella)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IR_Cereali_Conservazione)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IR_Cereali_Sfarinati)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IR_Cereali_Pastificazione)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IR_Cereali_Panificazione)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IR_Cereali_ProdottiDaForno)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IR_Cereali_AltriProdotti)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IR_COLTUREIND)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IR_ColtureInd_Granella)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IR_ColtureInd_Sfarinati)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IR_ColtureIndi_EstrazioneOlio)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IR_ColtureInd_Conservazione)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IR_ColtureInd_Confezionamento)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IR_ColtureInd_AltriProdotti)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IR_ORTOFRUTTICOLI)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IR_Ortofrutticoli_Freschi)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IR_Ortofrutticoli_ConserveVegetali)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IR_Ortofrutticoli_Conservazione)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IR_Ortofrutticoli_Confezionamento)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IR_VITIVINICOLO)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IR_Vitivinicolo_Vinificazione)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IR_Vitivinicolo_Mostificazione)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IR_Vitivinicolo_Imbottigliamento)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IR_OLEICOLO)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IR_Oleicolo_ConserveVegetali)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IR_Oleicolo_EstrazioneOlio)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IR_Oleicolo_Imbottigliamento)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IR_VIVSEMENTIERO)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IR_VivSementiero_Semi)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IR_VivSementiero_OrticoleTrapianto)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IR_VivSementiero_Astoni)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IR_VivSementiero_Barbatelle)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IR_VivSementiero_Altro)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IR_PIANTE_OFFICINALI)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(IR_PRODOTTI_SPONTANEI)) & " " & vbCrLf)

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


    '################################################################
    Public Function Cancella(ByVal Notifica_ID As Int32,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreBiologicoDAL.BIO_Notifica_SezA_Informazioni_W.Cancella()"

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
                StrSQL.Append(" UPDATE BIO_Notifica_SezA_Informazioni ")
                StrSQL.Append(" SET ")
                StrSQL.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE   Notifica_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND     Notifica_ID = " & Agro_SQL_SaveNum(Notifica_ID) & " ")
                StrSQL.Append(" AND     Inviato >= 0 ")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM    BIO_Notifica_SezA_Informazioni ")
                StrSQL.Append(" WHERE   Notifica_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND     Notifica_ID = " & Agro_SQL_SaveNum(Notifica_ID) & " ")

            End If
            '---------------------------------------------

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
