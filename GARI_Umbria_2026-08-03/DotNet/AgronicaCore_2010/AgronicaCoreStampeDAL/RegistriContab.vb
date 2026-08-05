Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports System.Text

Public Class RegistriContab
    Inherits AgronicaCoreDataProvider.DataProvider


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' query per lo spesometro
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function Spesometro(ByVal Piva As String,
                               ByVal Sezionale_Cod As Integer,
                               ByVal Flag_Vendita As Boolean,
                               ByVal Validita_Inizio As String,
                               ByVal Validita_Fine As String,
                               ByVal xFiltroAggiuntivo1 As String,
                               ByVal xFiltroAggiuntivo2 As String,
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                               ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreStampeDAL.RegistriContab.Spesometro"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            Try
                'per evitare di non conteggiare dei dati,
                'faccio una query di update per impostare di default la data di registrazione = alla data del movimento
                'quando questa è <= agrodatainizio
                Imposta_DataRegistrazione_Default(Piva, objParametri)

            Catch ex As Exception

            End Try

            stb.Length = 0

            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM ")

            stb.AppendLine(" ( ")

            '/*************************************************************************************
            '/**************** FATTURE, RICEVUTE, NOTE DI ACCREDITO       *************************
            '/*************************************************************************************

            stb.AppendLine(" ( ")
            stb.AppendLine(" SELECT Imprese.Rag_Soc AS Impresa, ")
            stb.AppendLine("")
            'agenda
            stb.AppendLine("        Agenda.PIVA, Agenda.Sa_Cod,  ")
            stb.AppendLine("        Agenda.Id_Agenda, Agenda.Lav_Cod, Operazioni.LAV_DES, Agenda.des_lib, ")
            stb.AppendLine("")
            'movimenti
            stb.AppendLine("        Movimenti_Contab.Cod_RisUm,  Movimenti_Contab.Mov_Desc, Movimenti_Contab.Modalita, ")
            stb.AppendLine("        ISNULL(Movimenti_Contab.Num_Protocollo,0) AS Num_Protocollo,  ")
            stb.AppendLine("        Movimenti_Contab.Data_Movimento, Movimenti_Contab.Doc_Numero_Sin, Movimenti_Contab.Doc_Numero, Movimenti_Contab.Doc_Numero_Des, ")
            stb.AppendLine("        Movimenti_Contab.Progr_Protocollo, Movimenti_Contab.Progr_Registrazione, Movimenti_Contab.Data_Registrazione, ")
            stb.AppendLine("")
            'contatti
            stb.AppendLine("        Contatti_Contab.Cod_Contatto, Contatti_Contab.Codice_Fiscale, Contatti_Contab.Rag_Soc, ")
            stb.AppendLine("")
            'iva_aliquote
            stb.AppendLine("        IVA_Aliquote.aliquota, IVA_Aliquote.Sigla AS Sigla_IVA, IVA_AliquoteInd.Sigla AS Sigla_IVAIndetraibile, ")
            stb.AppendLine("")
            'movimenti_dettagli
            stb.AppendLine("        Movimenti_dettagli.Id_Mov_Det, Movimenti_dettagli.ChkLayOut_Hide, ")
            stb.AppendLine("        Movimenti_dettagli.sconto , Movimenti_dettagli.sconto_listino , Movimenti_dettagli.sconto_modalita , ")
            stb.AppendLine("        Movimenti_dettagli.Cod_Iva, Movimenti_dettagli.Cod_IvaIndetraibile, ")
            'Stb.AppendLine("        SUM(Movimenti_dettagli.Qta) AS Qta_Totale, ")
            'Stb.AppendLine("         SUM(ROUND(Movimenti_dettagli.Imponibile,2)) AS Imponibile_Totale, ")
            'Stb.AppendLine("        SUM(ROUND(Movimenti_dettagli.Imponibile_Netto,2)) AS Imponibile_Netto_Totale, ")
            'Stb.AppendLine("        SUM(ROUND(Movimenti_dettagli.Iva,2)) AS Iva_Totale ")
            stb.AppendLine("        Movimenti_dettagli.Qta, ")
            stb.AppendLine("         Movimenti_dettagli.Prezzo_Unitario, ")
            stb.AppendLine("        Movimenti_dettagli.Prezzo_Unitario_Netto, ")
            stb.AppendLine("         Movimenti_dettagli.Imponibile, ")
            stb.AppendLine("        Movimenti_dettagli.Imponibile_Netto, ")
            stb.AppendLine("        Movimenti_dettagli.Iva ")
            stb.AppendLine("")

            stb.AppendLine(" FROM    Agenda ")

            'JOIN IMPRESE - AGENDA
            stb.AppendLine(" INNER JOIN Imprese ON Imprese.Piva = Agenda.Piva ")

            'JOIN AGENDA - OPERAZIONI
            stb.AppendLine(" INNER JOIN Operazioni ON Agenda.Lav_Cod = Operazioni.LAV_COD ")

            'JOIN AGENDA - MOVIMENTI CONTAB
            stb.AppendLine(" INNER JOIN Movimenti Movimenti_Contab ")
            stb.AppendLine(" ON Agenda.PIVA = Movimenti_Contab.PIVA AND Agenda.Sa_Cod = Movimenti_Contab.Sa_Cod AND Agenda.Id_Agenda = Movimenti_Contab.Id_Agenda")

            'JOIN MOVIMENTI DETTAGLI - RISORSE UMANE CONTAB
            'non mettere in join la piva, mi raccomando!!!!!!
            stb.AppendLine(" INNER JOIN Risorse_Umane RisUm_Contab ON RisUm_Contab.Cod_RisUm = Movimenti_Contab.Cod_RisUm ")
            'JOIN RISORSE UMANE - CONTATTI CONTAB
            stb.AppendLine(" INNER JOIN Contatti Contatti_Contab ON RisUm_Contab.Piva = Contatti_Contab.Piva AND RisUm_Contab.Cod_Contatto = Contatti_Contab.Cod_Contatto")

            'JOIN AGENDA - MOVIMENTI
            stb.AppendLine("INNER JOIN Movimenti Movimenti_Mag ")
            stb.AppendLine(" ON Agenda.PIVA = Movimenti_Mag.PIVA AND Agenda.Sa_Cod = Movimenti_Mag.Sa_Cod AND Agenda.Id_Agenda = Movimenti_Mag.Id_Agenda")

            'JOIN MOVIMENTI - MOVIMENTI DETTAGLI
            stb.AppendLine(" INNER JOIN Movimenti_dettagli ")
            stb.AppendLine(" ON Movimenti_dettagli.PIVA = Movimenti_Mag.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti_Mag.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti_Mag.Id_Mov ")

            'JOIN MOVIMENTI DETTAGLI - IVA ALIQUOTE
            stb.AppendLine(" INNER JOIN IVA_Aliquote ON IVA_Aliquote.Codice = Movimenti_dettagli.Cod_Iva   ")
            stb.AppendLine(" INNER JOIN IVA_Aliquote IVA_AliquoteInd ON IVA_AliquoteInd.Codice = Movimenti_dettagli.Cod_IvaIndetraibile   ")

            'CONDIZIONI

            ''MODIFICA DEL 24/11/2011: x rensi, sostituito filtro su da_movimento con Data_Registrazione
            ''Stb.AppendLine(" WHERE  Movimenti_Contab.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            ''Stb.AppendLine(" AND    Movimenti_Contab.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            ''Stb.AppendLine(" WHERE  Movimenti_Contab.Data_Registrazione <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            ''Stb.AppendLine(" AND    Movimenti_Contab.Data_Registrazione >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            'Stb.AppendLine(" WHERE   Movimenti_Contab.Data_Registrazione < " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            ''Stb.AppendLine(" AND     Movimenti_Contab.Data_Registrazione > " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            'Stb.AppendLine(" AND     Movimenti_Contab.Data_Registrazione >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")


            'poichè il giaslan salva nella data di registrazione anche l'ora (che in realtà non serve)
            'le date degli estremi rischiano di non essere conteggiate
            Validita_Fine = DateAdd(DateInterval.Day, 1, CDate(Validita_Fine))

            'modifica del 10/05/2012: nel registro vendite, filtrare data_movimento
            'mentre nel registro acquisti filtrare data_registrazione
            If Flag_Vendita = True Then
                stb.AppendLine(" WHERE   Movimenti_Contab.Data_Movimento < " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                stb.AppendLine(" AND     Movimenti_Contab.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            Else
                stb.AppendLine(" WHERE   Movimenti_Contab.Data_Registrazione < " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                stb.AppendLine(" AND     Movimenti_Contab.Data_Registrazione >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            End If

            stb.AppendLine(" AND    Movimenti_Contab.Cau_Mov = '" & CAU_REGISTRAZIONI & "'   ")

            If Sezionale_Cod <> -1 Then
                stb.AppendLine(" AND Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
            End If

            'MODIFICATO IF IN DATA 27/07/2011:
            'venivano visualizzate le note di accredito solo se erano degli abbuoni (che hanno causale dedicata)
            'questo perchè la causale di magazzino dei resi è inversa 
            If Flag_Vendita = True Then

                stb.AppendLine("AND ( ")

                'blocco fatture, ricevute e abbuoni
                stb.AppendLine(" ( ")
                stb.AppendLine("    ( ")
                stb.AppendLine("        (Agenda.Lav_Cod = " & CStr(LAVCOD_FATTURA_EMESSA) & ")")
                'modifica del 28/11/2011: le ricevute fiscali vanno nel registro dei corrispettivi
                'Stb.AppendLine("        OR (Agenda.Lav_Cod = " & CStr(LAVCOD_RICEVUTA_EMESSA) & ")")
                stb.AppendLine("        OR (Agenda.Lav_Cod = " & CStr(LAVCOD_NOTA_ACCREDITO_EMESSA) & " ) ")
                stb.AppendLine("        OR (Agenda.Lav_Cod = " & CStr(LAVCOD_FATTURA_LIQ_CONF_EMESSA) & " ) ")
                stb.AppendLine("        OR (Agenda.Lav_Cod = " & CStr(LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA) & " ) ")
                stb.AppendLine("    ) ")
                stb.AppendLine(" AND    Movimenti_Mag.Cau_Mov IN ( '" & CAU_SCARICO & "', '" & CAU_ABBUONI & "'  ) ")
                stb.AppendLine(" )")

                stb.AppendLine(" OR ")

                'blocco resi
                stb.AppendLine(" ( ")
                stb.AppendLine("    Agenda.Lav_Cod = " & CStr(LAVCOD_NOTA_ACCREDITO_EMESSA) & " ")
                stb.AppendLine("    AND ")
                stb.AppendLine("    Movimenti_Mag.Cau_Mov ='" & CAU_CARICO & "' ")
                stb.AppendLine(" ) ")

                stb.AppendLine(" ) ")

            Else

                stb.AppendLine("AND ( ")

                'blocco fatture, ricevute e abbuoni
                stb.AppendLine(" ( ")
                stb.AppendLine("    ( ")
                stb.AppendLine("        Agenda.Lav_Cod IN ( " & CStr(LAVCOD_FATTURA_RICEVUTA) & ", ")
                stb.AppendLine("                            " & CStr(LAVCOD_NOTA_ACCREDITO_RICEVUTA) & ", ")
                stb.AppendLine("                            " & CStr(LAVCOD_FATTURA_PROFESSIONISTI) & "")
                stb.AppendLine("                            )")
                'Stb.AppendLine("        (Agenda.Lav_Cod = " & CStr(LAVCOD_FATTURA_RICEVUTA) & ")")
                'Stb.AppendLine("        OR (Agenda.Lav_Cod = " & CStr(LAVCOD_NOTA_ACCREDITO_RICEVUTA) & ")")
                stb.AppendLine("    ) ")
                stb.AppendLine(" AND    Movimenti_Mag.Cau_Mov IN ( '" & CAU_CARICO & "', '" & CAU_ABBUONI & "'  ) ")
                stb.AppendLine(" )")

                stb.AppendLine(" OR ")

                'blocco resi
                stb.AppendLine(" ( ")
                stb.AppendLine("    Agenda.Lav_Cod = " & CStr(LAVCOD_NOTA_ACCREDITO_RICEVUTA) & " ")
                stb.AppendLine("    AND ")
                stb.AppendLine("    Movimenti_Mag.Cau_Mov ='" & CAU_SCARICO & "' ")
                stb.AppendLine(" ) ")

                stb.AppendLine(" ) ")

            End If


            stb.AppendLine(" AND    Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            stb.AppendLine(" AND    Agenda.Sa_Cod = 0 ")

            If xFiltroAggiuntivo1 <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo1, , objParametri))
            End If

            'Stb.AppendLine(" GROUP BY Agenda.PIVA, Imprese.Rag_Soc, Agenda.Sa_Cod,  ")
            'Stb.AppendLine("        Agenda.Id_Agenda, Agenda.Lav_Cod, Operazioni.LAV_DES, Agenda.des_lib, ")
            'Stb.AppendLine("        Movimenti_Contab.Cod_RisUm,  Movimenti_Contab.Mov_Desc, Movimenti_Contab.Modalita,  ")
            'Stb.AppendLine("        Movimenti_Contab.Num_Protocollo,  ")
            'Stb.AppendLine("        Movimenti_Contab.Data_Movimento, Movimenti_Contab.Doc_Numero_Sin, Movimenti_Contab.Doc_Numero, Movimenti_Contab.Doc_Numero_Des, ")
            'Stb.AppendLine("        Movimenti_Contab.Progr_Protocollo, Movimenti_Contab.Progr_Registrazione, Movimenti_Contab.Data_Registrazione, ")
            'Stb.AppendLine("        Contatti_Contab.Cod_Contatto, Contatti_Contab.Codice_Fiscale, Contatti_Contab.Rag_Soc, ")
            'Stb.AppendLine("        Movimenti_dettagli.Cod_Iva, Movimenti_dettagli.Cod_IvaIndetraibile, ")
            'Stb.AppendLine("        IVA_Aliquote.Sigla , IVA_AliquoteInd.Sigla ")

            stb.AppendLine(" ) ")



            stb.AppendLine(" UNION ALL ")

            '/*************************************************************************************
            '/******************            AUTOCONSUMO                   *************************
            '/*************************************************************************************

            stb.AppendLine(" (")

            stb.AppendLine(" SELECT Imprese.Rag_Soc AS Impresa, ")
            stb.AppendLine("")
            'agenda
            stb.AppendLine("        Agenda.PIVA, Agenda.Sa_Cod,  ")
            stb.AppendLine("        Agenda.Id_Agenda, Agenda.Lav_Cod, Operazioni.LAV_DES, Agenda.des_lib, ")
            stb.AppendLine("")
            'movimenti
            stb.AppendLine("        0 AS Cod_RisUm,  '' AS Mov_Desc, 0 AS Modalita, ")
            stb.AppendLine("        Movimenti_Mag.Num_Protocollo,  ")
            stb.AppendLine("        Movimenti_Mag.Data_Movimento, '' AS Doc_Numero_Sin, 0 AS Doc_Numero, '' AS Doc_Numero_Des, ")
            stb.AppendLine("        Movimenti_Mag.Progr_Protocollo, Movimenti_Mag.Progr_Registrazione, Movimenti_Mag.Data_Registrazione, ")
            stb.AppendLine("")
            'contatti
            stb.AppendLine("        '---' AS Cod_Contatto, '---' AS Codice_Fiscale, '---' AS Rag_Soc, ")
            stb.AppendLine("")
            'iva_aliquote
            stb.AppendLine("        IVA_Aliquote.aliquota, IVA_Aliquote.Sigla AS Sigla_IVA, IVA_AliquoteInd.Sigla AS Sigla_IVAIndetraibile, ")
            stb.AppendLine("")
            'movimenti_dettagli
            stb.AppendLine("        Movimenti_dettagli.Id_Mov_Det, Movimenti_dettagli.ChkLayOut_Hide, ")
            stb.AppendLine("        Movimenti_dettagli.sconto , Movimenti_dettagli.sconto_listino , Movimenti_dettagli.sconto_modalita , ")
            stb.AppendLine("        Movimenti_dettagli.Cod_Iva, Movimenti_dettagli.Cod_IvaIndetraibile, ")
            stb.AppendLine("        Movimenti_dettagli.Qta, ")
            stb.AppendLine("         Movimenti_dettagli.Prezzo_Unitario, ")
            stb.AppendLine("        Movimenti_dettagli.Prezzo_Unitario_Netto, ")
            stb.AppendLine("         Movimenti_dettagli.Imponibile, ")
            stb.AppendLine("        Movimenti_dettagli.Imponibile_Netto, ")
            stb.AppendLine("        Movimenti_dettagli.Iva ")
            stb.AppendLine("")

            stb.AppendLine(" FROM    Agenda ")

            'JOIN IMPRESE - AGENDA
            stb.AppendLine(" INNER JOIN Imprese ON Imprese.Piva = Agenda.Piva ")

            'JOIN AGENDA - OPERAZIONI
            stb.AppendLine(" INNER JOIN Operazioni ON Agenda.Lav_Cod = Operazioni.LAV_COD ")

            'JOIN AGENDA - MOVIMENTI
            stb.AppendLine("INNER JOIN Movimenti Movimenti_Mag ")
            stb.AppendLine(" ON Agenda.PIVA = Movimenti_Mag.PIVA AND Agenda.Id_Agenda = Movimenti_Mag.Id_Agenda")

            'JOIN MOVIMENTI - MOVIMENTI DETTAGLI
            stb.AppendLine(" INNER JOIN Movimenti_dettagli ")
            stb.AppendLine(" ON Movimenti_dettagli.PIVA = Movimenti_Mag.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti_Mag.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti_Mag.Id_Mov ")

            'JOIN MOVIMENTI DETTAGLI - IVA ALIQUOTE
            stb.AppendLine(" INNER JOIN IVA_Aliquote ON IVA_Aliquote.Codice = Movimenti_dettagli.Cod_Iva   ")
            stb.AppendLine(" INNER JOIN IVA_Aliquote IVA_AliquoteInd ON IVA_AliquoteInd.Codice = Movimenti_dettagli.Cod_IvaIndetraibile   ")

            'CONDIZIONI
            stb.AppendLine(" WHERE  Movimenti_Mag.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            stb.AppendLine(" AND    Movimenti_Mag.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            If Flag_Vendita = True Then
                stb.AppendLine(" AND    Movimenti_Mag.Cau_Mov = '" & CAU_SCARICO & "'   ")
            Else
                stb.AppendLine(" AND    Movimenti_Mag.Cau_Mov = '" & CAU_CARICO & "'   ")
            End If

            stb.AppendLine(" AND    Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")

            stb.AppendLine(" AND    Agenda.Lav_Cod IN (" & CStr(LAVCOD_AUTOCONSUMO) & ", " & CStr(LAVCOD_AUTOCONSUMO_VINO_SFUSO) & ") ")

            If Sezionale_Cod <> -1 Then
                stb.AppendLine(" AND Movimenti_Mag.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
            End If

            If xFiltroAggiuntivo2 <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo2, , objParametri))
            End If

            stb.AppendLine(" )")

            '/*************************************************************************************

            stb.AppendLine(" ) REGISTRO_IVA ")

            If Flag_Vendita = True Then
                stb.AppendLine(" ORDER BY Doc_Numero, Data_Movimento, Doc_Numero_Sin, Doc_Numero_Des, Lav_Cod, Id_Agenda, Cod_IVA ")
            Else
                stb.AppendLine(" ORDER BY Progr_Protocollo, Data_Movimento, Doc_Numero, Doc_Numero_Sin, Doc_Numero_Des, Lav_Cod, Id_Agenda, Cod_IVA ")
            End If

            stb.AppendLine(" ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function


    'vecchia query, quella attuale è la RegistriIVA_3
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' a differenza dell'altra, non fa sum e group by, legge tutti i dettagli, che per ogni fattura vanno dati in pasto alla funzione di Marco
    ''' Registro IVA Vendite
    ''' Registro IVA Acquisti
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function RegistriIVA_2(ByVal Piva As String,
                                  ByVal Sezionale_Cod As Integer,
                                  ByVal Flag_Vendita As Boolean,
                                  ByVal Validita_Inizio As String,
                                  ByVal Validita_Fine As String,
                                  ByVal xFiltroAggiuntivo1 As String,
                                  ByVal xFiltroAggiuntivo2 As String,
                                  ByVal Ordinamento As enum_RegistriIva_Ordinamento,
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                  ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreStampeDAL.RegistriContab.RegistriIVA_2"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            Try
                'per evitare di non conteggiare dei dati,
                'faccio una query di update per impostare di default la data di registrazione = alla data del movimento
                'quando questa è <= agrodatainizio
                Imposta_DataRegistrazione_Default(Piva, objParametri)

            Catch ex As Exception

            End Try

            stb.Length = 0

            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM ")

            stb.AppendLine(" ( ")

            '/*************************************************************************************
            '/**************** FATTURE, RICEVUTE, NOTE DI ACCREDITO       *************************
            '/*************************************************************************************

            stb.AppendLine(" ( ")
            stb.AppendLine(" SELECT Imprese.Rag_Soc AS Impresa, ")
            stb.AppendLine("")
            'agenda
            stb.AppendLine("        Agenda.PIVA, Agenda.Sa_Cod,  ")
            stb.AppendLine("        Agenda.Id_Agenda, Agenda.Lav_Cod, Operazioni.LAV_DES, Agenda.des_lib, ")
            stb.AppendLine("")
            'movimenti
            stb.AppendLine("        Movimenti_Contab.Cod_RisUm,  Movimenti_Contab.Mov_Desc, Movimenti_Contab.Modalita, ")
            stb.AppendLine("        ISNULL(Movimenti_Contab.Num_Protocollo,0) AS Num_Protocollo, ISNULL(Movimenti_Contab.Tipo_Sconto ,0) AS Tipo_Sconto, ")
            stb.AppendLine("        Movimenti_Contab.Data_Movimento, Movimenti_Contab.Doc_Numero_Sin, Movimenti_Contab.Doc_Numero, Movimenti_Contab.Doc_Numero_Des, ")
            stb.AppendLine("        Movimenti_Contab.Progr_Protocollo, Movimenti_Contab.Progr_Registrazione, Movimenti_Contab.Data_Registrazione, ")
            stb.AppendLine("")
            'contatti
            stb.AppendLine("        Contatti_Contab.Cod_Contatto, Contatti_Contab.Codice_Fiscale, (Contatti_Contab.Rag_Soc + Contatti_Contab.Nome + ' ' + Contatti_Contab.cognome) AS Rag_Soc, ")
            stb.AppendLine("")
            'iva_aliquote
            'IVA_AliquoteInd.Sigla AS Sigla_IVAIndetraibile,
            stb.AppendLine("        IVA_Aliquote.aliquota, IVA_Aliquote.Sigla AS Sigla_IVA,  ")
            stb.AppendLine("")
            'movimenti_dettagli
            stb.AppendLine("        Movimenti_dettagli.Id_Mov_Det, Movimenti_dettagli.ChkLayOut_Hide, ")
            stb.AppendLine("        Movimenti_dettagli.sconto , Movimenti_dettagli.sconto_listino , Movimenti_dettagli.sconto_modalita , ")
            stb.AppendLine("        Movimenti_dettagli.Cod_Iva, Movimenti_dettagli.Cod_IvaIndetraibile, ")
            stb.AppendLine("        Movimenti_dettagli.Iva_Indetraibile, Movimenti_dettagli.Iva_Indetraibile_Perc, ")
            'Stb.AppendLine("        SUM(Movimenti_dettagli.Qta) AS Qta_Totale, ")
            'Stb.AppendLine("         SUM(ROUND(Movimenti_dettagli.Imponibile,2)) AS Imponibile_Totale, ")
            'Stb.AppendLine("        SUM(ROUND(Movimenti_dettagli.Imponibile_Netto,2)) AS Imponibile_Netto_Totale, ")
            'Stb.AppendLine("        SUM(ROUND(Movimenti_dettagli.Iva,2)) AS Iva_Totale ")
            stb.AppendLine("        Movimenti_dettagli.Qta, ")
            stb.AppendLine("         Movimenti_dettagli.Prezzo_Unitario, ")
            stb.AppendLine("        Movimenti_dettagli.Prezzo_Unitario_Netto, ")
            stb.AppendLine("         Movimenti_dettagli.Imponibile, ")
            stb.AppendLine("        Movimenti_dettagli.Imponibile_Netto, ")
            stb.AppendLine("        Movimenti_dettagli.Iva ")
            stb.AppendLine("")

            stb.AppendLine(" FROM    Agenda ")

            'JOIN IMPRESE - AGENDA
            stb.AppendLine(" INNER JOIN Imprese ON Imprese.Piva = Agenda.Piva ")

            'JOIN AGENDA - OPERAZIONI
            stb.AppendLine(" INNER JOIN Operazioni ON Agenda.Lav_Cod = Operazioni.LAV_COD ")

            'JOIN AGENDA - MOVIMENTI CONTAB
            stb.AppendLine(" INNER JOIN Movimenti Movimenti_Contab ")
            stb.AppendLine(" ON Agenda.PIVA = Movimenti_Contab.PIVA AND Agenda.Sa_Cod = Movimenti_Contab.Sa_Cod AND Agenda.Id_Agenda = Movimenti_Contab.Id_Agenda")

            'JOIN MOVIMENTI DETTAGLI - RISORSE UMANE CONTAB
            'non mettere in join la piva, mi raccomando!!!!!!
            stb.AppendLine(" INNER JOIN Risorse_Umane RisUm_Contab ON RisUm_Contab.Cod_RisUm = Movimenti_Contab.Cod_RisUm ")
            'JOIN RISORSE UMANE - CONTATTI CONTAB
            stb.AppendLine(" INNER JOIN Contatti Contatti_Contab ON RisUm_Contab.Piva = Contatti_Contab.Piva AND RisUm_Contab.Cod_Contatto = Contatti_Contab.Cod_Contatto")

            'JOIN AGENDA - MOVIMENTI
            stb.AppendLine("INNER JOIN Movimenti Movimenti_Mag ")
            stb.AppendLine(" ON Agenda.PIVA = Movimenti_Mag.PIVA AND Agenda.Sa_Cod = Movimenti_Mag.Sa_Cod AND Agenda.Id_Agenda = Movimenti_Mag.Id_Agenda")

            'JOIN MOVIMENTI - MOVIMENTI DETTAGLI
            stb.AppendLine(" INNER JOIN Movimenti_dettagli ")
            stb.AppendLine(" ON Movimenti_dettagli.PIVA = Movimenti_Mag.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti_Mag.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti_Mag.Id_Mov ")

            'JOIN MOVIMENTI DETTAGLI - IVA ALIQUOTE
            stb.AppendLine(" INNER JOIN IVA_Aliquote ON IVA_Aliquote.Codice = Movimenti_dettagli.Cod_Iva   ")

            'Stb.AppendLine(" INNER JOIN IVA_Aliquote IVA_AliquoteInd ON IVA_AliquoteInd.Codice = Movimenti_dettagli.Cod_IvaIndetraibile   ")

            'CONDIZIONI

            ''MODIFICA DEL 24/11/2011: x rensi, sostituito filtro su da_movimento con Data_Registrazione
            ''Stb.AppendLine(" WHERE  Movimenti_Contab.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            ''Stb.AppendLine(" AND    Movimenti_Contab.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            ''Stb.AppendLine(" WHERE  Movimenti_Contab.Data_Registrazione <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            ''Stb.AppendLine(" AND    Movimenti_Contab.Data_Registrazione >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            'Stb.AppendLine(" WHERE   Movimenti_Contab.Data_Registrazione < " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            ''Stb.AppendLine(" AND     Movimenti_Contab.Data_Registrazione > " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            'Stb.AppendLine(" AND     Movimenti_Contab.Data_Registrazione >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")


            'poichè il giaslan salva nella data di registrazione anche l'ora (che in realtà non serve)
            'le date degli estremi rischiano di non essere conteggiate
            Validita_Fine = DateAdd(DateInterval.Day, 1, CDate(Validita_Fine))

            'modifica del 10/05/2012: nel registro vendite, filtrare data_movimento
            'mentre nel registro acquisti filtrare data_registrazione
            If Flag_Vendita = True Then
                stb.AppendLine(" WHERE   Movimenti_Contab.Data_Movimento < " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                stb.AppendLine(" AND     Movimenti_Contab.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            Else
                stb.AppendLine(" WHERE   Movimenti_Contab.Data_Registrazione < " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                stb.AppendLine(" AND     Movimenti_Contab.Data_Registrazione >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            End If

            stb.AppendLine(" AND Movimenti_dettagli.Cod_Iva >= 0 ")

            stb.AppendLine(" AND    Movimenti_Contab.Cau_Mov = '" & CAU_REGISTRAZIONI & "'   ")

            If Sezionale_Cod <> -1 Then
                stb.AppendLine(" AND Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
            End If

            'MODIFICATO IF IN DATA 27/07/2011:
            'venivano visualizzate le note di accredito solo se erano degli abbuoni (che hanno causale dedicata)
            'questo perchè la causale di magazzino dei resi è inversa 
            If Flag_Vendita = True Then

                stb.AppendLine("AND ( ")

                'blocco fatture, ricevute e abbuoni
                stb.AppendLine(" ( ")
                stb.AppendLine("    ( ")
                stb.AppendLine("        (Agenda.Lav_Cod = " & CStr(LAVCOD_FATTURA_EMESSA) & ")")
                'modifica del 28/11/2011: le ricevute fiscali vanno nel registro dei corrispettivi
                'Stb.AppendLine("        OR (Agenda.Lav_Cod = " & CStr(LAVCOD_RICEVUTA_EMESSA) & ")")
                stb.AppendLine("        OR (Agenda.Lav_Cod = " & CStr(LAVCOD_NOTA_ACCREDITO_EMESSA) & " ) ")
                stb.AppendLine("        OR (Agenda.Lav_Cod = " & CStr(LAVCOD_FATTURA_LIQ_CONF_EMESSA) & " ) ")
                stb.AppendLine("        OR (Agenda.Lav_Cod = " & CStr(LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA) & " ) ")
                stb.AppendLine("    ) ")
                stb.AppendLine(" AND    Movimenti_Mag.Cau_Mov IN ( '" & CAU_SCARICO & "', '" & CAU_ABBUONI & "'  ) ")
                stb.AppendLine(" )")

                stb.AppendLine(" OR ")

                'blocco resi
                stb.AppendLine(" ( ")
                stb.AppendLine("    Agenda.Lav_Cod = " & CStr(LAVCOD_NOTA_ACCREDITO_EMESSA) & " ")
                stb.AppendLine("    AND ")
                stb.AppendLine("    Movimenti_Mag.Cau_Mov ='" & CAU_CARICO & "' ")
                stb.AppendLine(" ) ")

                stb.AppendLine(" ) ")

            Else

                stb.AppendLine("AND ( ")

                'blocco fatture, ricevute e abbuoni
                stb.AppendLine(" ( ")
                stb.AppendLine("    ( ")
                stb.AppendLine("        Agenda.Lav_Cod IN ( " & CStr(LAVCOD_FATTURA_RICEVUTA) & ", ")
                stb.AppendLine("                            " & CStr(LAVCOD_NOTA_ACCREDITO_RICEVUTA) & ", ")
                stb.AppendLine("                            " & CStr(LAVCOD_FATTURA_PROFESSIONISTI) & "")
                stb.AppendLine("                            )")
                'Stb.AppendLine("        (Agenda.Lav_Cod = " & CStr(LAVCOD_FATTURA_RICEVUTA) & ")")
                'Stb.AppendLine("        OR (Agenda.Lav_Cod = " & CStr(LAVCOD_NOTA_ACCREDITO_RICEVUTA) & ")")
                stb.AppendLine("    ) ")
                stb.AppendLine(" AND    Movimenti_Mag.Cau_Mov IN ( '" & CAU_CARICO & "', '" & CAU_ABBUONI & "'  ) ")
                stb.AppendLine(" )")

                stb.AppendLine(" OR ")

                'blocco resi
                stb.AppendLine(" ( ")
                stb.AppendLine("    Agenda.Lav_Cod = " & CStr(LAVCOD_NOTA_ACCREDITO_RICEVUTA) & " ")
                stb.AppendLine("    AND ")
                stb.AppendLine("    Movimenti_Mag.Cau_Mov ='" & CAU_SCARICO & "' ")
                stb.AppendLine(" ) ")

                stb.AppendLine(" ) ")

            End If

            stb.AppendLine(" AND    Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            stb.AppendLine(" AND    Agenda.Sa_Cod = 0 ")

            'MODIFICA DEL 13/08/2014: gestione ChkCoge_Manuale
            stb.AppendLine(" AND (  ")
            stb.AppendLine("    ( Agenda.ChkCoge_Manuale = " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & " ) ")
            stb.AppendLine("    OR ")
            stb.AppendLine("    ( ")
            stb.AppendLine("    Agenda.ChkCoge_Manuale = " & CStr(enum_ChkCoGe.CoGe_CONTABILIZZAZIONE_MANUALE) & " ")
            stb.AppendLine("    AND EXISTS ( ")
            stb.AppendLine("    SELECT 1 ")
            stb.AppendLine("    FROM  Mov_Dettagli_Riferimenti ")
            stb.AppendLine("    INNER JOIN Agenda AgPD on AgPD.PIVA =Mov_Dettagli_Riferimenti.piva ")
            stb.AppendLine("    AND AgPD.Id_Agenda= Mov_Dettagli_Riferimenti.id_agenda ")
            stb.AppendLine("    WHERE Mov_Dettagli_Riferimenti.Piva_Rif=agenda.piva   ")
            stb.AppendLine("    AND Mov_Dettagli_Riferimenti.Id_Agenda_Rif=agenda.id_agenda ")
            stb.AppendLine("    AND Mov_Dettagli_Riferimenti.Id_mov_Rif=-1 ")
            stb.AppendLine("    AND Mov_Dettagli_Riferimenti.Id_mov_det_Rif=-1 ")
            stb.AppendLine("    AND Mov_Dettagli_Riferimenti.lav_cod = " & CStr(LAVCOD_MOV_FINANZIARIO) & " ")
            stb.AppendLine("    AND AgPD.ChkCoge_Manuale = " & CStr(enum_ChkCoGe.CoGe_PD_COLLEGATAaDOCUMENTO) & " ")
            stb.AppendLine("            ) -- exists ")
            stb.AppendLine("        ) -- or ")
            stb.AppendLine("    ) -- and ")
            stb.AppendLine("  ")

            If xFiltroAggiuntivo1 <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo1, , objParametri))
            End If

            ''DEBUG!!!!!!!!!
            'Stb.AppendLine(" AND Movimenti_dettagli.Cod_Iva = 22 ")

            'Stb.AppendLine(" GROUP BY Agenda.PIVA, Imprese.Rag_Soc, Agenda.Sa_Cod,  ")
            'Stb.AppendLine("        Agenda.Id_Agenda, Agenda.Lav_Cod, Operazioni.LAV_DES, Agenda.des_lib, ")
            'Stb.AppendLine("        Movimenti_Contab.Cod_RisUm,  Movimenti_Contab.Mov_Desc, Movimenti_Contab.Modalita,  ")
            'Stb.AppendLine("        Movimenti_Contab.Num_Protocollo,  ")
            'Stb.AppendLine("        Movimenti_Contab.Data_Movimento, Movimenti_Contab.Doc_Numero_Sin, Movimenti_Contab.Doc_Numero, Movimenti_Contab.Doc_Numero_Des, ")
            'Stb.AppendLine("        Movimenti_Contab.Progr_Protocollo, Movimenti_Contab.Progr_Registrazione, Movimenti_Contab.Data_Registrazione, ")
            'Stb.AppendLine("        Contatti_Contab.Cod_Contatto, Contatti_Contab.Codice_Fiscale, Contatti_Contab.Rag_Soc, ")
            'Stb.AppendLine("        Movimenti_dettagli.Cod_Iva, Movimenti_dettagli.Cod_IvaIndetraibile, ")
            'Stb.AppendLine("        IVA_Aliquote.Sigla , IVA_AliquoteInd.Sigla ")

            stb.AppendLine(" ) ")



            stb.AppendLine(" UNION ALL ")

            '/*************************************************************************************
            '/******************            AUTOCONSUMO                   *************************
            '/*************************************************************************************

            stb.AppendLine(" (")

            stb.AppendLine(" SELECT Imprese.Rag_Soc AS Impresa, ")
            stb.AppendLine("")
            'agenda
            stb.AppendLine("        Agenda.PIVA, Agenda.Sa_Cod,  ")
            stb.AppendLine("        Agenda.Id_Agenda, Agenda.Lav_Cod, Operazioni.LAV_DES, Agenda.des_lib, ")
            stb.AppendLine("")
            'movimenti
            stb.AppendLine("        0 AS Cod_RisUm,  '' AS Mov_Desc, 0 AS Modalita, ")
            stb.AppendLine("        Movimenti_Mag.Num_Protocollo, Movimenti_Mag.Tipo_Sconto,  ")
            stb.AppendLine("        Movimenti_Mag.Data_Movimento, '' AS Doc_Numero_Sin, 0 AS Doc_Numero, '' AS Doc_Numero_Des, ")
            stb.AppendLine("        Movimenti_Mag.Progr_Protocollo, Movimenti_Mag.Progr_Registrazione, Movimenti_Mag.Data_Registrazione, ")
            stb.AppendLine("")
            'contatti
            stb.AppendLine("        '---' AS Cod_Contatto, '---' AS Codice_Fiscale, '---' AS Rag_Soc, ")
            stb.AppendLine("")
            'iva_aliquote
            'IVA_AliquoteInd.Sigla AS Sigla_IVAIndetraibile,
            stb.AppendLine("        IVA_Aliquote.aliquota, IVA_Aliquote.Sigla AS Sigla_IVA,  ")
            stb.AppendLine("")
            'movimenti_dettagli
            stb.AppendLine("        Movimenti_dettagli.Id_Mov_Det, Movimenti_dettagli.ChkLayOut_Hide, ")
            stb.AppendLine("        Movimenti_dettagli.sconto , Movimenti_dettagli.sconto_listino , Movimenti_dettagli.sconto_modalita , ")
            stb.AppendLine("        Movimenti_dettagli.Cod_Iva, Movimenti_dettagli.Cod_IvaIndetraibile, ")
            stb.AppendLine("        Movimenti_dettagli.Iva_Indetraibile, Movimenti_dettagli.Iva_Indetraibile_Perc, ")
            stb.AppendLine("        Movimenti_dettagli.Qta, ")
            stb.AppendLine("         Movimenti_dettagli.Prezzo_Unitario, ")
            stb.AppendLine("        Movimenti_dettagli.Prezzo_Unitario_Netto, ")
            stb.AppendLine("         Movimenti_dettagli.Imponibile, ")
            stb.AppendLine("        Movimenti_dettagli.Imponibile_Netto, ")
            stb.AppendLine("        Movimenti_dettagli.Iva ")
            stb.AppendLine("")

            stb.AppendLine(" FROM    Agenda ")

            'JOIN IMPRESE - AGENDA
            stb.AppendLine(" INNER JOIN Imprese ON Imprese.Piva = Agenda.Piva ")

            'JOIN AGENDA - OPERAZIONI
            stb.AppendLine(" INNER JOIN Operazioni ON Agenda.Lav_Cod = Operazioni.LAV_COD ")

            'JOIN AGENDA - MOVIMENTI
            stb.AppendLine("INNER JOIN Movimenti Movimenti_Mag ")
            stb.AppendLine(" ON Agenda.PIVA = Movimenti_Mag.PIVA AND Agenda.Id_Agenda = Movimenti_Mag.Id_Agenda")

            'JOIN MOVIMENTI - MOVIMENTI DETTAGLI
            stb.AppendLine(" INNER JOIN Movimenti_dettagli ")
            stb.AppendLine(" ON Movimenti_dettagli.PIVA = Movimenti_Mag.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti_Mag.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti_Mag.Id_Mov ")

            'JOIN MOVIMENTI DETTAGLI - IVA ALIQUOTE
            stb.AppendLine(" INNER JOIN IVA_Aliquote ON IVA_Aliquote.Codice = Movimenti_dettagli.Cod_Iva   ")
            'Stb.AppendLine(" INNER JOIN IVA_Aliquote IVA_AliquoteInd ON IVA_AliquoteInd.Codice = Movimenti_dettagli.Cod_IvaIndetraibile   ")

            'CONDIZIONI
            stb.AppendLine(" WHERE  Movimenti_Mag.Data_Movimento < " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            stb.AppendLine(" AND    Movimenti_Mag.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            stb.AppendLine(" AND Movimenti_dettagli.Cod_Iva >= 0 ")

            If Flag_Vendita = True Then
                stb.AppendLine(" AND    Movimenti_Mag.Cau_Mov = '" & CAU_SCARICO & "'   ")
            Else
                stb.AppendLine(" AND    Movimenti_Mag.Cau_Mov = '" & CAU_CARICO & "'   ")
            End If

            stb.AppendLine(" AND    Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")

            stb.AppendLine(" AND    Agenda.Lav_Cod IN (" & CStr(LAVCOD_AUTOCONSUMO) & ", " & CStr(LAVCOD_AUTOCONSUMO_VINO_SFUSO) & ") ")

            If Sezionale_Cod <> -1 Then
                stb.AppendLine(" AND Movimenti_Mag.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
            End If

            If xFiltroAggiuntivo2 <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo2, , objParametri))
            End If

            stb.AppendLine(" )")

            '/*************************************************************************************

            stb.AppendLine(" ) REGISTRO_IVA ")

            'modifica del 21/05/14
            Select Case Ordinamento
                Case enum_RegistriIva_Ordinamento.NumeroDoc
                    stb.AppendLine(" ORDER BY Doc_Numero, Data_Movimento, Doc_Numero_Sin, Doc_Numero_Des, Lav_Cod, Id_Agenda, Cod_IVA ")
                Case enum_RegistriIva_Ordinamento.NumeroProtocollo
                    stb.AppendLine(" ORDER BY Progr_Protocollo, Data_Registrazione, Doc_Numero, Doc_Numero_Sin, Doc_Numero_Des, Lav_Cod, Id_Agenda, Cod_IVA ")
            End Select
            'If Flag_Vendita = True Then
            '    Stb.AppendLine(" ORDER BY Doc_Numero, Data_Movimento, Doc_Numero_Sin, Doc_Numero_Des, Lav_Cod, Id_Agenda, Cod_IVA ")
            'Else
            '    Stb.AppendLine(" ORDER BY Progr_Protocollo, Data_Movimento, Doc_Numero, Doc_Numero_Sin, Doc_Numero_Des, Lav_Cod, Id_Agenda, Cod_IVA ")
            'End If



            stb.AppendLine(" ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' stessa query di RegistriIVA_2 ma SENZA AUTOCONSUMI (perchè quelli sono confluiti nel registro corrispettivi)
    ''' Registro IVA Vendite
    ''' Registro IVA Acquisti
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function RegistriIVA_3(ByVal Piva As String,
                                  ByVal Sezionale_Cod As Integer,
                                  ByVal Flag_Vendita As Boolean,
                                  ByVal Validita_Inizio As String,
                                  ByVal Validita_Fine As String,
                                  ByVal xFiltroAggiuntivo1 As String,
                                  ByVal xFiltroAggiuntivo2 As String,
                                  ByVal Ordinamento As enum_RegistriIva_Ordinamento,
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                  ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreStampeDAL.RegistriContab.RegistriIVA_3"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            Try
                'per evitare di non conteggiare dei dati,
                'faccio una query di update per impostare di default la data di registrazione = alla data del movimento
                'quando questa è <= agrodatainizio
                Imposta_DataRegistrazione_Default(Piva, objParametri)

            Catch ex As Exception

            End Try

            stb.Length = 0

            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM ")

            stb.AppendLine(" ( ")

            '/*************************************************************************************
            '/**************** FATTURE, RICEVUTE, NOTE DI ACCREDITO       *************************
            '/*************************************************************************************

            stb.AppendLine(" ( ")
            stb.AppendLine(" SELECT Imprese.Rag_Soc AS Impresa, ")
            stb.AppendLine("")
            'agenda
            stb.AppendLine("        Agenda.PIVA, Agenda.Sa_Cod,  ")
            stb.AppendLine("        Agenda.Id_Agenda, Agenda.Lav_Cod, Operazioni.LAV_DES, Agenda.des_lib, ")
            stb.AppendLine("")
            'movimenti
            stb.AppendLine("        Movimenti_Contab.Cod_RisUm,  Movimenti_Contab.Mov_Desc, Movimenti_Contab.Modalita, ")
            stb.AppendLine("        ISNULL(Movimenti_Contab.Num_Protocollo,0) AS Num_Protocollo, ISNULL(Movimenti_Contab.Tipo_Sconto ,0) AS Tipo_Sconto, ")
            stb.AppendLine("        Movimenti_Contab.Data_Movimento, Movimenti_Contab.Doc_Numero_Sin, Movimenti_Contab.Doc_Numero, Movimenti_Contab.Doc_Numero_Des, ")
            stb.AppendLine("        Movimenti_Contab.Progr_Protocollo, Movimenti_Contab.Progr_Registrazione, Movimenti_Contab.Data_Registrazione, ")
            stb.AppendLine("")
            'contatti
            stb.AppendLine("        Contatti_Contab.Cod_Contatto, Contatti_Contab.Codice_Fiscale, (Contatti_Contab.Rag_Soc + Contatti_Contab.Nome + ' ' + Contatti_Contab.cognome) AS Rag_Soc, ")
            stb.AppendLine("")
            'iva_aliquote
            stb.AppendLine("        IVA_Aliquote.aliquota, IVA_Aliquote.Sigla AS Sigla_IVA,  ")
            stb.AppendLine("")
            'movimenti_dettagli
            stb.AppendLine("        Movimenti_dettagli.Id_Mov_Det, Movimenti_dettagli.ChkLayOut_Hide, ")
            stb.AppendLine("        Movimenti_dettagli.sconto , Movimenti_dettagli.sconto_listino , Movimenti_dettagli.sconto_modalita , ")
            stb.AppendLine("        Movimenti_dettagli.Cod_Iva, Movimenti_dettagli.Cod_IvaIndetraibile, ")
            stb.AppendLine("        Movimenti_dettagli.Iva_Indetraibile, Movimenti_dettagli.Iva_Indetraibile_Perc, ")
            stb.AppendLine("        Movimenti_dettagli.Qta, ")
            stb.AppendLine("         Movimenti_dettagli.Prezzo_Unitario, ")
            stb.AppendLine("        Movimenti_dettagli.Prezzo_Unitario_Netto, ")
            stb.AppendLine("         Movimenti_dettagli.Imponibile, ")
            stb.AppendLine("        Movimenti_dettagli.Imponibile_Netto, ")
            stb.AppendLine("        Movimenti_dettagli.Iva ")
            stb.AppendLine("")

            stb.AppendLine(" FROM    Agenda ")

            'JOIN IMPRESE - AGENDA
            stb.AppendLine(" INNER JOIN Imprese ON Imprese.Piva = Agenda.Piva ")

            'JOIN AGENDA - OPERAZIONI
            stb.AppendLine(" INNER JOIN Operazioni ON Agenda.Lav_Cod = Operazioni.LAV_COD ")

            'JOIN AGENDA - MOVIMENTI CONTAB
            stb.AppendLine(" INNER JOIN Movimenti Movimenti_Contab ")
            stb.AppendLine(" ON Agenda.PIVA = Movimenti_Contab.PIVA AND Agenda.Sa_Cod = Movimenti_Contab.Sa_Cod AND Agenda.Id_Agenda = Movimenti_Contab.Id_Agenda")

            'JOIN MOVIMENTI DETTAGLI - RISORSE UMANE CONTAB
            'non mettere in join la piva, mi raccomando!!!!!!
            stb.AppendLine(" INNER JOIN Risorse_Umane RisUm_Contab ON RisUm_Contab.Cod_RisUm = Movimenti_Contab.Cod_RisUm ")
            'JOIN RISORSE UMANE - CONTATTI CONTAB
            stb.AppendLine(" INNER JOIN Contatti Contatti_Contab ON RisUm_Contab.Piva = Contatti_Contab.Piva AND RisUm_Contab.Cod_Contatto = Contatti_Contab.Cod_Contatto")

            'JOIN AGENDA - MOVIMENTI
            stb.AppendLine("INNER JOIN Movimenti Movimenti_Mag ")
            stb.AppendLine(" ON Agenda.PIVA = Movimenti_Mag.PIVA AND Agenda.Sa_Cod = Movimenti_Mag.Sa_Cod AND Agenda.Id_Agenda = Movimenti_Mag.Id_Agenda")

            'JOIN MOVIMENTI - MOVIMENTI DETTAGLI
            stb.AppendLine(" INNER JOIN Movimenti_dettagli ")
            stb.AppendLine(" ON Movimenti_dettagli.PIVA = Movimenti_Mag.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti_Mag.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti_Mag.Id_Mov ")

            'JOIN MOVIMENTI DETTAGLI - IVA ALIQUOTE
            stb.AppendLine(" INNER JOIN IVA_Aliquote ON IVA_Aliquote.Codice = Movimenti_dettagli.Cod_Iva   ")

            'poichè il giaslan salva nella data di registrazione anche l'ora (che in realtà non serve)
            'le date degli estremi rischiano di non essere conteggiate
            Validita_Fine = DateAdd(DateInterval.Day, 1, CDate(Validita_Fine))

            'modifica del 10/05/2012: nel registro vendite, filtrare data_movimento
            'mentre nel registro acquisti filtrare data_registrazione
            If Flag_Vendita = True Then
                stb.AppendLine(" WHERE   Movimenti_Contab.Data_Movimento < " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                stb.AppendLine(" AND     Movimenti_Contab.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            Else
                stb.AppendLine(" WHERE   Movimenti_Contab.Data_Registrazione < " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                stb.AppendLine(" AND     Movimenti_Contab.Data_Registrazione >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            End If

            stb.AppendLine(" AND Movimenti_dettagli.Cod_Iva >= 0 ")

            stb.AppendLine(" AND    Movimenti_Contab.Cau_Mov = '" & CAU_REGISTRAZIONI & "'   ")

            If Sezionale_Cod <> -1 Then
                stb.AppendLine(" AND Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
            End If

            'MODIFICATO IF IN DATA 27/07/2011:
            'venivano visualizzate le note di accredito solo se erano degli abbuoni (che hanno causale dedicata)
            'questo perchè la causale di magazzino dei resi è inversa 
            If Flag_Vendita = True Then

                stb.AppendLine("AND ( ")

                'blocco fatture, ricevute e abbuoni
                stb.AppendLine(" ( ")
                stb.AppendLine("    ( ")
                stb.AppendLine("        (Agenda.Lav_Cod = " & CStr(LAVCOD_FATTURA_EMESSA) & ")")
                'modifica del 28/11/2011: le ricevute fiscali vanno nel registro dei corrispettivi
                'Stb.AppendLine("        OR (Agenda.Lav_Cod = " & CStr(LAVCOD_RICEVUTA_EMESSA) & ")")
                stb.AppendLine("        OR (Agenda.Lav_Cod = " & CStr(LAVCOD_NOTA_ACCREDITO_EMESSA) & " ) ")
                stb.AppendLine("        OR (Agenda.Lav_Cod = " & CStr(LAVCOD_FATTURA_LIQ_CONF_EMESSA) & " ) ")
                stb.AppendLine("        OR (Agenda.Lav_Cod = " & CStr(LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA) & " ) ")
                stb.AppendLine("    ) ")
                stb.AppendLine(" AND    Movimenti_Mag.Cau_Mov IN ( '" & CAU_SCARICO & "', '" & CAU_ABBUONI & "'  ) ")
                stb.AppendLine(" )")

                stb.AppendLine(" OR ")

                'blocco resi
                stb.AppendLine(" ( ")
                stb.AppendLine("    Agenda.Lav_Cod = " & CStr(LAVCOD_NOTA_ACCREDITO_EMESSA) & " ")
                stb.AppendLine("    AND ")
                stb.AppendLine("    Movimenti_Mag.Cau_Mov ='" & CAU_CARICO & "' ")
                stb.AppendLine(" ) ")

                stb.AppendLine(" ) ")

            Else

                stb.AppendLine("AND ( ")

                'blocco fatture, ricevute e abbuoni
                stb.AppendLine(" ( ")
                stb.AppendLine("    ( ")
                stb.AppendLine("        Agenda.Lav_Cod IN ( " & CStr(LAVCOD_FATTURA_RICEVUTA) & ", ")
                stb.AppendLine("                            " & CStr(LAVCOD_NOTA_ACCREDITO_RICEVUTA) & ", ")
                stb.AppendLine("                            " & CStr(LAVCOD_FATTURA_PROFESSIONISTI) & "")
                stb.AppendLine("                            )")
                'Stb.AppendLine("        (Agenda.Lav_Cod = " & CStr(LAVCOD_FATTURA_RICEVUTA) & ")")
                'Stb.AppendLine("        OR (Agenda.Lav_Cod = " & CStr(LAVCOD_NOTA_ACCREDITO_RICEVUTA) & ")")
                stb.AppendLine("    ) ")
                stb.AppendLine(" AND    Movimenti_Mag.Cau_Mov IN ( '" & CAU_CARICO & "', '" & CAU_ABBUONI & "'  ) ")
                stb.AppendLine(" )")

                stb.AppendLine(" OR ")

                'blocco resi
                stb.AppendLine(" ( ")
                stb.AppendLine("    Agenda.Lav_Cod = " & CStr(LAVCOD_NOTA_ACCREDITO_RICEVUTA) & " ")
                stb.AppendLine("    AND ")
                stb.AppendLine("    Movimenti_Mag.Cau_Mov ='" & CAU_SCARICO & "' ")
                stb.AppendLine(" ) ")

                stb.AppendLine(" ) ")

            End If

            stb.AppendLine(" AND    Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            stb.AppendLine(" AND    Agenda.Sa_Cod = 0 ")


            ''12/12/2018: modifiche a seguito dei cambiamenti sulla contabilizzazione
            '
            'MODIFICA DEL 13/08/2014: gestione ChkCoge_Manuale
            'stb.AppendLine(" AND (  ")
            'stb.AppendLine("    ( Agenda.ChkCoge_Manuale = " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & " ) ")
            'stb.AppendLine("    OR ")
            'stb.AppendLine("    ( ")
            'stb.AppendLine("    Agenda.ChkCoge_Manuale = " & CStr(enum_ChkCoGe.CoGe_CONTABILIZZAZIONE_MANUALE) & " ")
            'stb.AppendLine("    AND EXISTS ( ")
            'stb.AppendLine("    SELECT 1 ")
            'stb.AppendLine("    FROM  Mov_Dettagli_Riferimenti ")
            'stb.AppendLine("    INNER JOIN Agenda AgPD on AgPD.PIVA =Mov_Dettagli_Riferimenti.piva ")
            'stb.AppendLine("    AND AgPD.Id_Agenda= Mov_Dettagli_Riferimenti.id_agenda ")
            'stb.AppendLine("    WHERE Mov_Dettagli_Riferimenti.Piva_Rif=agenda.piva   ")
            'stb.AppendLine("    AND Mov_Dettagli_Riferimenti.Id_Agenda_Rif=agenda.id_agenda ")
            'stb.AppendLine("    AND Mov_Dettagli_Riferimenti.Id_mov_Rif=-1 ")
            'stb.AppendLine("    AND Mov_Dettagli_Riferimenti.Id_mov_det_Rif=-1 ")
            'stb.AppendLine("    AND Mov_Dettagli_Riferimenti.lav_cod = " & CStr(LAVCOD_MOV_FINANZIARIO) & " ")
            'stb.AppendLine("    AND AgPD.ChkCoge_Manuale = " & CStr(enum_ChkCoGe.CoGe_PD_COLLEGATAaDOCUMENTO) & " ")
            'stb.AppendLine("            ) -- exists ")
            'stb.AppendLine("        ) -- or ")
            'stb.AppendLine("    ) -- and ")
            'stb.AppendLine("  ")
            stb.AppendLine(" AND Agenda.ChkCoge_Manuale IN ( " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & "  ")
            stb.AppendLine("                                , " & CStr(enum_ChkCoGe.CoGe_CONTABILIZZAZIONE_MANUALE) & " ")
            stb.AppendLine("                                , " & CStr(enum_ChkCoGe.CoGe_CONTABILIZZAZIONE_AUTOMATICA) & " ")
            stb.AppendLine("                                ) ")


            If xFiltroAggiuntivo1 <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo1, , objParametri))
            End If

            stb.AppendLine(" ) ")


            '/*************************************************************************************

            stb.AppendLine(" ) REGISTRO_IVA ")

            'modifica del 21/05/14
            Select Case Ordinamento
                Case enum_RegistriIva_Ordinamento.NumeroDoc
                    stb.AppendLine(" ORDER BY Doc_Numero, Data_Movimento, Doc_Numero_Sin, Doc_Numero_Des, Lav_Cod, Id_Agenda, Cod_IVA ")
                Case enum_RegistriIva_Ordinamento.NumeroProtocollo
                    stb.AppendLine(" ORDER BY Progr_Protocollo, Data_Registrazione, Doc_Numero, Doc_Numero_Sin, Doc_Numero_Des, Lav_Cod, Id_Agenda, Cod_IVA ")
            End Select

            stb.AppendLine(" ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    'vecchia query, quella attuale è la RegistriIVA_3
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Registro IVA Vendite
    ''' Registro IVA Acquisti
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function RegistriIVA(ByVal Piva As String,
                                ByVal Sezionale_Cod As Integer,
                                ByVal Flag_Vendita As Boolean,
                                ByVal Validita_Inizio As String,
                                ByVal Validita_Fine As String,
                                ByVal xFiltroAggiuntivo1 As String,
                                ByVal xFiltroAggiuntivo2 As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreStampeDAL.RegistriContab.RegistriIVA"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            Try
                'per evitare di non conteggiare dei dati,
                'faccio una query di update per impostare di default la data di registrazione = alla data del movimento
                Imposta_DataRegistrazione_Default(Piva, objParametri)

            Catch ex As Exception

            End Try

            stb.Length = 0

            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM ")

            stb.AppendLine(" ( ")

            '/*************************************************************************************
            '/**************** FATTURE, RICEVUTE, NOTE DI ACCREDITO       *************************
            '/*************************************************************************************

            stb.AppendLine(" ( ")
            stb.AppendLine(" SELECT Agenda.PIVA, Imprese.Rag_Soc AS Impresa, Agenda.Sa_Cod,  ")
            stb.AppendLine("        Agenda.Id_Agenda, Agenda.Lav_Cod, Operazioni.LAV_DES, Agenda.des_lib, ")
            stb.AppendLine("        Movimenti_Contab.Cod_RisUm,  Movimenti_Contab.Mov_Desc, Movimenti_Contab.Modalita, ")
            stb.AppendLine("        ISNULL(Movimenti_Contab.Num_Protocollo,0) AS Num_Protocollo,  ")
            stb.AppendLine("        Movimenti_Contab.Data_Movimento, Movimenti_Contab.Doc_Numero_Sin, Movimenti_Contab.Doc_Numero, Movimenti_Contab.Doc_Numero_Des, ")
            stb.AppendLine("        Movimenti_Contab.Progr_Protocollo, Movimenti_Contab.Progr_Registrazione, Movimenti_Contab.Data_Registrazione, ")
            stb.AppendLine("        Contatti_Contab.Cod_Contatto, Contatti_Contab.Codice_Fiscale, Contatti_Contab.Rag_Soc, ")
            stb.AppendLine("        Movimenti_dettagli.Cod_Iva, Movimenti_dettagli.Cod_IvaIndetraibile, ")
            stb.AppendLine("        IVA_Aliquote.Sigla AS Sigla_IVA, IVA_AliquoteInd.Sigla AS Sigla_IVAIndetraibile, ")
            stb.AppendLine("        SUM(Movimenti_dettagli.Qta) AS Qta_Totale, ")
            'Stb.AppendLine("        SUM(Movimenti_dettagli.Imponibile) AS Imponibile_Totale, ")
            'Stb.AppendLine("        SUM(Movimenti_dettagli.Imponibile_Netto) AS Imponibile_Netto_Totale, ")
            'Stb.AppendLine("        SUM(Movimenti_dettagli.Iva) AS Iva_Totale ")
            stb.AppendLine("         SUM(ROUND(Movimenti_dettagli.Imponibile,2)) AS Imponibile_Totale, ")
            stb.AppendLine("        SUM(ROUND(Movimenti_dettagli.Imponibile_Netto,2)) AS Imponibile_Netto_Totale, ")
            stb.AppendLine("        SUM(ROUND(Movimenti_dettagli.Iva,2)) AS Iva_Totale ")
            stb.AppendLine("")

            stb.AppendLine(" FROM    Agenda ")

            'JOIN IMPRESE - AGENDA
            stb.AppendLine(" INNER JOIN Imprese ON Imprese.Piva = Agenda.Piva ")

            'JOIN AGENDA - OPERAZIONI
            stb.AppendLine(" INNER JOIN Operazioni ON Agenda.Lav_Cod = Operazioni.LAV_COD ")

            'JOIN AGENDA - MOVIMENTI CONTAB
            stb.AppendLine(" INNER JOIN Movimenti Movimenti_Contab ")
            stb.AppendLine(" ON Agenda.PIVA = Movimenti_Contab.PIVA AND Agenda.Sa_Cod = Movimenti_Contab.Sa_Cod AND Agenda.Id_Agenda = Movimenti_Contab.Id_Agenda")

            'JOIN MOVIMENTI DETTAGLI - RISORSE UMANE CONTAB
            'non mettere in join la piva, mi raccomando!!!!!!
            stb.AppendLine(" INNER JOIN Risorse_Umane RisUm_Contab ON RisUm_Contab.Cod_RisUm = Movimenti_Contab.Cod_RisUm ")
            'JOIN RISORSE UMANE - CONTATTI CONTAB
            stb.AppendLine(" INNER JOIN Contatti Contatti_Contab ON RisUm_Contab.Piva = Contatti_Contab.Piva AND RisUm_Contab.Cod_Contatto = Contatti_Contab.Cod_Contatto")

            'JOIN AGENDA - MOVIMENTI
            stb.AppendLine("INNER JOIN Movimenti Movimenti_Mag ")
            stb.AppendLine(" ON Agenda.PIVA = Movimenti_Mag.PIVA AND Agenda.Sa_Cod = Movimenti_Mag.Sa_Cod AND Agenda.Id_Agenda = Movimenti_Mag.Id_Agenda")

            'JOIN MOVIMENTI - MOVIMENTI DETTAGLI
            stb.AppendLine(" INNER JOIN Movimenti_dettagli ")
            stb.AppendLine(" ON Movimenti_dettagli.PIVA = Movimenti_Mag.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti_Mag.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti_Mag.Id_Mov ")

            'JOIN MOVIMENTI DETTAGLI - IVA ALIQUOTE
            stb.AppendLine(" INNER JOIN IVA_Aliquote ON IVA_Aliquote.Codice = Movimenti_dettagli.Cod_Iva   ")
            stb.AppendLine(" INNER JOIN IVA_Aliquote IVA_AliquoteInd ON IVA_AliquoteInd.Codice = Movimenti_dettagli.Cod_IvaIndetraibile   ")

            'CONDIZIONI

            ''MODIFICA DEL 24/11/2011: x rensi, sostituito filtro su da_movimento con Data_Registrazione
            ''Stb.AppendLine(" WHERE  Movimenti_Contab.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            ''Stb.AppendLine(" AND    Movimenti_Contab.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            ''Stb.AppendLine(" WHERE  Movimenti_Contab.Data_Registrazione <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            ''Stb.AppendLine(" AND    Movimenti_Contab.Data_Registrazione >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            'Stb.AppendLine(" WHERE   Movimenti_Contab.Data_Registrazione < " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            ''Stb.AppendLine(" AND     Movimenti_Contab.Data_Registrazione > " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            'Stb.AppendLine(" AND     Movimenti_Contab.Data_Registrazione >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")


            'poichè il giaslan salva nella data di registrazione anche l'ora (che in realtà non serve)
            'le date degli estremi rischiano di non essere conteggiate
            Validita_Fine = DateAdd(DateInterval.Day, 1, CDate(Validita_Fine))

            'modifica del 10/05/2012: nel registro vendite, filtrare data_movimento
            'mentre nel registro acquisti filtrare data_registrazione
            If Flag_Vendita = True Then
                stb.AppendLine(" WHERE   Movimenti_Contab.Data_Movimento < " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                stb.AppendLine(" AND     Movimenti_Contab.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            Else
                stb.AppendLine(" WHERE   Movimenti_Contab.Data_Registrazione < " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                stb.AppendLine(" AND     Movimenti_Contab.Data_Registrazione >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            End If

            stb.AppendLine(" AND    Movimenti_Contab.Cau_Mov = '" & CAU_REGISTRAZIONI & "'   ")

            If Sezionale_Cod <> -1 Then
                stb.AppendLine(" AND Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
            End If

            'MODIFICATO IF IN DATA 27/07/2011:
            'venivano visualizzate le note di accredito solo se erano degli abbuoni (che hanno causale dedicata)
            'questo perchè la causale di magazzino dei resi è inversa 
            If Flag_Vendita = True Then

                stb.AppendLine("AND ( ")

                'blocco fatture, ricevute e abbuoni
                stb.AppendLine(" ( ")
                stb.AppendLine("    ( ")
                stb.AppendLine("        (Agenda.Lav_Cod = " & CStr(LAVCOD_FATTURA_EMESSA) & ")")
                'modifica del 28/11/2011: le ricevute fiscali vanno nel registro dei corrispettivi
                'Stb.AppendLine("        OR (Agenda.Lav_Cod = " & CStr(LAVCOD_RICEVUTA_EMESSA) & ")")
                stb.AppendLine("        OR (Agenda.Lav_Cod = " & CStr(LAVCOD_NOTA_ACCREDITO_EMESSA) & " ) ")
                stb.AppendLine("        OR (Agenda.Lav_Cod = " & CStr(LAVCOD_FATTURA_LIQ_CONF_EMESSA) & " ) ")
                stb.AppendLine("        OR (Agenda.Lav_Cod = " & CStr(LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA) & " ) ")
                stb.AppendLine("    ) ")
                stb.AppendLine(" AND    Movimenti_Mag.Cau_Mov IN ( '" & CAU_SCARICO & "', '" & CAU_ABBUONI & "'  ) ")
                stb.AppendLine(" )")

                stb.AppendLine(" OR ")

                'blocco resi
                stb.AppendLine(" ( ")
                stb.AppendLine("    Agenda.Lav_Cod = " & CStr(LAVCOD_NOTA_ACCREDITO_EMESSA) & " ")
                stb.AppendLine("    AND ")
                stb.AppendLine("    Movimenti_Mag.Cau_Mov ='" & CAU_CARICO & "' ")
                stb.AppendLine(" ) ")

                stb.AppendLine(" ) ")

            Else

                stb.AppendLine("AND ( ")

                'blocco fatture, ricevute e abbuoni
                stb.AppendLine(" ( ")
                stb.AppendLine("    ( ")
                stb.AppendLine("        (Agenda.Lav_Cod = " & CStr(LAVCOD_FATTURA_RICEVUTA) & ")")
                stb.AppendLine("        OR (Agenda.Lav_Cod = " & CStr(LAVCOD_NOTA_ACCREDITO_RICEVUTA) & ")")
                stb.AppendLine("    ) ")
                stb.AppendLine(" AND    Movimenti_Mag.Cau_Mov IN ( '" & CAU_CARICO & "', '" & CAU_ABBUONI & "'  ) ")
                stb.AppendLine(" )")

                stb.AppendLine(" OR ")

                'blocco resi
                stb.AppendLine(" ( ")
                stb.AppendLine("    Agenda.Lav_Cod = " & CStr(LAVCOD_NOTA_ACCREDITO_RICEVUTA) & " ")
                stb.AppendLine("    AND ")
                stb.AppendLine("    Movimenti_Mag.Cau_Mov ='" & CAU_SCARICO & "' ")
                stb.AppendLine(" ) ")

                stb.AppendLine(" ) ")

            End If


            stb.AppendLine(" AND    Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            stb.AppendLine(" AND    Agenda.Sa_Cod = 0 ")

            If xFiltroAggiuntivo1 <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo1, , objParametri))
            End If

            stb.AppendLine(" GROUP BY Agenda.PIVA, Imprese.Rag_Soc, Agenda.Sa_Cod,  ")
            stb.AppendLine("        Agenda.Id_Agenda, Agenda.Lav_Cod, Operazioni.LAV_DES, Agenda.des_lib, ")
            stb.AppendLine("        Movimenti_Contab.Cod_RisUm,  Movimenti_Contab.Mov_Desc, Movimenti_Contab.Modalita,  ")
            stb.AppendLine("        Movimenti_Contab.Num_Protocollo,  ")
            stb.AppendLine("        Movimenti_Contab.Data_Movimento, Movimenti_Contab.Doc_Numero_Sin, Movimenti_Contab.Doc_Numero, Movimenti_Contab.Doc_Numero_Des, ")
            stb.AppendLine("        Movimenti_Contab.Progr_Protocollo, Movimenti_Contab.Progr_Registrazione, Movimenti_Contab.Data_Registrazione, ")
            stb.AppendLine("        Contatti_Contab.Cod_Contatto, Contatti_Contab.Codice_Fiscale, Contatti_Contab.Rag_Soc, ")
            stb.AppendLine("        Movimenti_dettagli.Cod_Iva, Movimenti_dettagli.Cod_IvaIndetraibile, ")
            stb.AppendLine("        IVA_Aliquote.Sigla , IVA_AliquoteInd.Sigla ")

            'If Ordinamento <> "" Then
            '    Stb.AppendLine(Ordinamento)
            'Else
            '    Stb.AppendLine(" ORDER BY Agenda.PIVA, Movimenti.Data_Movimento DESC ")
            'End If

            'ORDER BY Movimenti_Contab.Data_Movimento, Movimenti_Contab.Progr_Protocollo, Movimenti_Contab.Doc_Numero, Movimenti_Contab.Doc_Numero_Sin, Movimenti_Contab.Doc_Numero_Des, Movimenti_dettagli.Cod_IVA 

            stb.AppendLine(" ) ")



            stb.AppendLine(" UNION ALL ")

            '/*************************************************************************************
            '/******************            AUTOCONSUMO                   *************************
            '/*************************************************************************************

            stb.AppendLine(" (")
            stb.AppendLine(" SELECT Agenda.PIVA, Imprese.Rag_Soc AS Impresa, Agenda.Sa_Cod,  ")
            stb.AppendLine("        Agenda.Id_Agenda, Agenda.Lav_Cod, Operazioni.LAV_DES, Agenda.des_lib, ")
            stb.AppendLine("        0 AS Cod_RisUm,  '' AS Mov_Desc, 0 AS Modalita, ")
            stb.AppendLine("        0 AS Num_Protocollo,  ")
            stb.AppendLine("        Movimenti_Mag.Data_Movimento, '' AS Doc_Numero_Sin, 0 AS Doc_Numero, '' AS Doc_Numero_Des, ")
            stb.AppendLine("        0 AS Progr_Protocollo, 0 AS Progr_Registrazione, '01/01/1900' AS Data_Registrazione, ")
            stb.AppendLine("        '---' AS Cod_Contatto, '---' AS Codice_Fiscale, '---' AS Rag_Soc, ")
            stb.AppendLine("        Movimenti_dettagli.Cod_Iva, Movimenti_dettagli.Cod_IvaIndetraibile, ")
            stb.AppendLine("        IVA_Aliquote.Sigla AS Sigla_IVA, IVA_AliquoteInd.Sigla AS Sigla_IVAIndetraibile, ")
            'Stb.AppendLine("        SUM(Movimenti_dettagli.Qta) AS Qta_Totale, ")
            'Stb.AppendLine("        SUM(Movimenti_dettagli.Imponibile) AS Imponibile_Totale, ")
            'Stb.AppendLine("        SUM(Movimenti_dettagli.Imponibile_Netto) AS Imponibile_Netto_Totale, ")
            'Stb.AppendLine("        SUM(Movimenti_dettagli.Iva) AS Iva_Totale ")
            stb.AppendLine("        Movimenti_dettagli.Qta AS Qta_Totale, ")
            stb.AppendLine("        Movimenti_dettagli.Imponibile AS Imponibile_Totale, ")
            stb.AppendLine("        Movimenti_dettagli.Imponibile_Netto AS Imponibile_Netto_Totale, ")
            stb.AppendLine("        Movimenti_dettagli.Iva AS Iva_Totale ")
            stb.AppendLine("")

            stb.AppendLine(" FROM    Agenda ")

            'JOIN IMPRESE - AGENDA
            stb.AppendLine(" INNER JOIN Imprese ON Imprese.Piva = Agenda.Piva ")

            'JOIN AGENDA - OPERAZIONI
            stb.AppendLine(" INNER JOIN Operazioni ON Agenda.Lav_Cod = Operazioni.LAV_COD ")

            'JOIN AGENDA - MOVIMENTI
            stb.AppendLine("INNER JOIN Movimenti Movimenti_Mag ")
            stb.AppendLine(" ON Agenda.PIVA = Movimenti_Mag.PIVA AND Agenda.Id_Agenda = Movimenti_Mag.Id_Agenda")

            'JOIN MOVIMENTI - MOVIMENTI DETTAGLI
            stb.AppendLine(" INNER JOIN Movimenti_dettagli ")
            stb.AppendLine(" ON Movimenti_dettagli.PIVA = Movimenti_Mag.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti_Mag.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti_Mag.Id_Mov ")

            'JOIN MOVIMENTI DETTAGLI - IVA ALIQUOTE
            stb.AppendLine(" INNER JOIN IVA_Aliquote ON IVA_Aliquote.Codice = Movimenti_dettagli.Cod_Iva   ")
            stb.AppendLine(" INNER JOIN IVA_Aliquote IVA_AliquoteInd ON IVA_AliquoteInd.Codice = Movimenti_dettagli.Cod_IvaIndetraibile   ")

            'CONDIZIONI
            stb.AppendLine(" WHERE  Movimenti_Mag.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            stb.AppendLine(" AND    Movimenti_Mag.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            If Flag_Vendita = True Then
                stb.AppendLine(" AND    Movimenti_Mag.Cau_Mov = '" & CAU_SCARICO & "'   ")
            Else
                stb.AppendLine(" AND    Movimenti_Mag.Cau_Mov = '" & CAU_CARICO & "'   ")
            End If

            stb.AppendLine(" AND    Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")

            stb.AppendLine(" AND    Agenda.Lav_Cod IN (" & CStr(LAVCOD_AUTOCONSUMO) & ", " & CStr(LAVCOD_AUTOCONSUMO_VINO_SFUSO) & ") ")

            If Sezionale_Cod <> -1 Then
                stb.AppendLine(" AND Movimenti_Mag.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
            End If

            If xFiltroAggiuntivo2 <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo2, , objParametri))
            End If

            'Stb.AppendLine(" GROUP BY Agenda.PIVA, Imprese.Rag_Soc, Agenda.Sa_Cod,  ")
            'Stb.AppendLine("        Agenda.Id_Agenda, Agenda.Lav_Cod, Operazioni.LAV_DES, Agenda.des_lib, ")
            'Stb.AppendLine("        Cod_RisUm,  Mov_Desc, ")
            'Stb.AppendLine("        Num_Protocollo,  ")
            'Stb.AppendLine("        Data_Movimento, Doc_Numero_Sin, Doc_Numero, Doc_Numero_Des, ")
            'Stb.AppendLine("        Progr_Protocollo, Progr_Registrazione, Data_Registrazione, ")
            ''Stb.AppendLine("        Cod_Contatto, Codice_Fiscale, Rag_Soc, ")
            'Stb.AppendLine("        Rag_Soc, ")
            'Stb.AppendLine("        Movimenti_dettagli.Cod_Iva, Movimenti_dettagli.Cod_IvaIndetraibile ")
            stb.AppendLine(" )")

            '/*************************************************************************************

            stb.AppendLine(" ) REGISTRO_IVA ")

            If Flag_Vendita = True Then
                stb.AppendLine(" ORDER BY Doc_Numero, Data_Movimento, Doc_Numero_Sin, Doc_Numero_Des, Lav_Cod, Id_Agenda, Cod_IVA ")
            Else
                'Stb.AppendLine(" ORDER BY Progr_Protocollo, Data_Movimento, Doc_Numero, Doc_Numero_Sin, Doc_Numero_Des, Lav_Cod, Id_Agenda, Cod_IVA ")
                stb.AppendLine(" ORDER BY Progr_Protocollo, Data_Movimento, Doc_Numero, Doc_Numero_Sin, Doc_Numero_Des, Lav_Cod, Id_Agenda, Cod_IVA ")
            End If

            stb.AppendLine(" ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' legge, per ogni giorno dell'intervallo, gli importi totali
    ''' dei corrispettivi di vendita, lav_cod = 1020
    ''' delle ricevute fiscali, lav_cod = 1053
    ''' e dei ddt contabilizzati, lav_cod = 1069
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function Z_OLD_RegistroCorrispettivi_Importi_RoundSql(ByVal Piva As String,
                                                           ByVal Sezionale_Cod As Integer,
                                                           ByVal Validita_Inizio As String,
                                                           ByVal Validita_Fine As String,
                                                           ByVal xFiltroAggiuntivo As String,
                                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                           ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreStampeDAL.RegistriContab.RegistroCorrispettivi_Importi"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            '/*************************************************************************************
            '/************************* CORRISPETTIVI VENDITA *************************************
            '/*************************************************************************************

            stb.AppendLine(" SELECT PIVA,  Impresa,  Data_Movimento , ")
            stb.AppendLine("        SUM(ROUND(Importo_4, 2))  AS Importo_4,  ")
            stb.AppendLine("        SUM(ROUND(Iva_4, 2))  AS Iva_4,  ")

            stb.AppendLine("        SUM(ROUND(Importo_10, 2))  AS Importo_10,  ")
            stb.AppendLine("        SUM(ROUND(Iva_10, 2))  AS Iva_10,  ")

            stb.AppendLine("        SUM(ROUND(Importo_12, 2))  AS Importo_12,  ")
            stb.AppendLine("        SUM(ROUND(Iva_12, 2))  AS Iva_12,  ")

            stb.AppendLine("        SUM(ROUND(Importo_20, 2))  AS Importo_20,  ")
            stb.AppendLine("        SUM(ROUND(Iva_20, 2))  AS Iva_20,  ")

            stb.AppendLine("        SUM(ROUND(Importo_21, 2))  AS Importo_21,  ")
            stb.AppendLine("        SUM(ROUND(Iva_21, 2))  AS Iva_21, ")

            stb.AppendLine("        SUM(ROUND(Esente, 2)) AS Esente ")

            stb.AppendLine(" FROM ")

            '/*************************************************************************************
            '/************************* QUERY INTERNA *************************************
            stb.AppendLine(" ( ")
            stb.AppendLine("    SELECT  Agenda.PIVA, Imprese.Rag_Soc AS Impresa, Lav_Cod, Movimenti_Contab.Data_Movimento , ")

            stb.AppendLine("    CASE WHEN Movimenti_dettagli.Cod_Iva =4  THEN  Movimenti_Contab.Num_Protocollo ELSE 0 END AS Importo_4,  ")
            stb.AppendLine("    CASE WHEN Movimenti_dettagli.Cod_Iva =4  THEN  Movimenti_dettagli.Iva ELSE 0 END AS Iva_4 , ")

            stb.AppendLine("    CASE WHEN Movimenti_dettagli.Cod_Iva =10  THEN  Movimenti_Contab.Num_Protocollo ELSE 0 END AS Importo_10,  ")
            stb.AppendLine("    CASE WHEN Movimenti_dettagli.Cod_Iva =10  THEN  Movimenti_dettagli.Iva ELSE 0 END AS Iva_10, ")

            stb.AppendLine("    CASE WHEN Movimenti_dettagli.Cod_Iva =12  THEN  Movimenti_Contab.Num_Protocollo ELSE 0 END AS Importo_12,  ")
            stb.AppendLine("    CASE WHEN Movimenti_dettagli.Cod_Iva =12  THEN  Movimenti_dettagli.Iva ELSE 0 END AS Iva_12, ")

            stb.AppendLine("    CASE WHEN Movimenti_dettagli.Cod_Iva =20 THEN  Movimenti_Contab.Num_Protocollo ELSE 0 END AS Importo_20, ")
            stb.AppendLine("    CASE WHEN Movimenti_dettagli.Cod_Iva =20  THEN  Movimenti_dettagli.Iva ELSE 0 END AS Iva_20, ")

            stb.AppendLine("    CASE WHEN Movimenti_dettagli.Cod_Iva =21 THEN  Movimenti_Contab.Num_Protocollo ELSE 0 END AS Importo_21, ")
            stb.AppendLine("    CASE WHEN Movimenti_dettagli.Cod_Iva =21  THEN  Movimenti_dettagli.Iva ELSE 0 END AS Iva_21, ")

            stb.AppendLine("    CASE WHEN Movimenti_dettagli.Cod_Iva NOT IN (4,10,12,20,21)  THEN  Movimenti_dettagli.Imponibile_Netto ELSE 0 END AS Esente ")

            stb.AppendLine("    FROM Agenda ")

            stb.AppendLine("    INNER JOIN Imprese ON Imprese.Piva = Agenda.Piva  ")

            stb.AppendLine("    INNER JOIN Movimenti Movimenti_Mag  ")
            stb.AppendLine("    ON Agenda.PIVA = Movimenti_Mag.PIVA AND Agenda.Id_Agenda = Movimenti_Mag.Id_Agenda ")

            stb.AppendLine("    INNER JOIN Movimenti Movimenti_Contab  ")
            stb.AppendLine("    ON Agenda.PIVA = Movimenti_Contab.PIVA AND Agenda.Id_Agenda = Movimenti_Contab.Id_Agenda ")

            stb.AppendLine("    INNER JOIN Movimenti_dettagli  ")
            stb.AppendLine("    ON Movimenti_dettagli.PIVA = Movimenti_Mag.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti_Mag.Id_Agenda  ")
            stb.AppendLine("    AND Movimenti_dettagli.Id_Mov = Movimenti_Mag.Id_Mov  ")

            stb.AppendLine("    INNER JOIN IVA_Aliquote ON IVA_Aliquote.Codice = Movimenti_dettagli.Cod_Iva    ")
            stb.AppendLine("    INNER JOIN IVA_Aliquote IVA_AliquoteInd ON IVA_AliquoteInd.Codice = Movimenti_dettagli.Cod_IvaIndetraibile    ")

            'CONDIZIONI
            stb.AppendLine("    WHERE  Movimenti_Contab.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            stb.AppendLine("    AND    Movimenti_Contab.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            'Stb.AppendLine("    AND    Agenda.Lav_Cod = " & Agro_SQL_SaveNum(LAVCOD_VENDITA) & "  ")
            stb.AppendLine(" AND    Agenda.Lav_Cod IN ( " & LAVCOD_VENDITA & ", " &
                                                        LAVCOD_CORRISPETTIVO_VENDITA_SFUSO & ", " &
                                                        LAVCOD_RICEVUTA_EMESSA & ", " &
                                                        LAVCOD_DDT_CONTABILIZZATO_EMESSO &
                                                        "  ) ")

            stb.AppendLine("    AND    Movimenti_Mag.Cau_Mov = '" & CAU_SCARICO & "'  ")
            stb.AppendLine("    AND    Movimenti_Contab.Cau_Mov = '" & CAU_REGISTRAZIONI & "'  ")

            stb.AppendLine("    AND    Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            stb.AppendLine("    AND    Agenda.Sa_Cod = 0 ")

            If Sezionale_Cod <> -1 Then
                stb.AppendLine(" AND    Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            stb.AppendLine(" ) CORRISPETTIVI ")
            '/************************* QUERY INTERNA *************************************
            '/*************************************************************************************

            stb.AppendLine(" GROUP BY PIVA,  Impresa,  Data_Movimento  ")
            stb.AppendLine(" ORDER BY Data_Movimento ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' legge, per ogni giorno dell'intervallo, gli importi totali
    ''' dei corrispettivi di vendita, lav_cod = 1020
    ''' delle ricevute fiscali, lav_cod = 1053
    ''' e dei ddt contabilizzati, lav_cod = 1069
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function Z_OLD_RegistroCorrispettivi_Importi(ByVal Piva As String,
                                                  ByVal Sezionale_Cod As Integer,
                                                  ByVal Validita_Inizio As String,
                                                  ByVal Validita_Fine As String,
                                                  ByVal xFiltroAggiuntivo As String,
                                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                  ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreStampeDAL.RegistriContab.RegistroCorrispettivi_Importi"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            '/*************************************************************************************
            '/************************* CORRISPETTIVI VENDITA *************************************
            '/*************************************************************************************

            stb.AppendLine(" SELECT PIVA,  Impresa,  Data_Movimento , ")
            'Stb.AppendLine("        SUM(Imponibile_4)  AS Imponibile_4,  ")
            'Stb.AppendLine("        SUM(Iva_4)  AS Iva_4,  ")
            'Stb.AppendLine("        SUM(Imponibile_10)  AS Imponibile_10,  ")
            'Stb.AppendLine("        SUM(Iva_10)  AS Iva_10,  ")
            'Stb.AppendLine("        SUM(Imponibile_12)  AS Imponibile_12,  ")
            'Stb.AppendLine("        SUM(Iva_12)  AS Iva_12,  ")
            'Stb.AppendLine("        SUM(Imponibile_20)  AS Imponibile_20,  ")
            'Stb.AppendLine("        SUM(Iva_20)  AS Iva_20,  ")
            'Stb.AppendLine("        SUM(Imponibile_21)  AS Imponibile_21,  ")
            'Stb.AppendLine("        SUM(Iva_21)  AS Iva_21, ")
            'Stb.AppendLine("        SUM(Esente) AS Esente ")
            stb.AppendLine("        SUM(ROUND(Imponibile_4, 2))  AS Imponibile_4,  ")
            stb.AppendLine("        SUM(ROUND(Iva_4, 2))  AS Iva_4,  ")
            stb.AppendLine("        SUM(ROUND(Imponibile_10, 2))  AS Imponibile_10,  ")
            stb.AppendLine("        SUM(ROUND(Iva_10, 2))  AS Iva_10,  ")
            stb.AppendLine("        SUM(ROUND(Imponibile_12, 2))  AS Imponibile_12,  ")
            stb.AppendLine("        SUM(ROUND(Iva_12, 2))  AS Iva_12,  ")
            stb.AppendLine("        SUM(ROUND(Imponibile_20, 2))  AS Imponibile_20,  ")
            stb.AppendLine("        SUM(ROUND(Iva_20, 2))  AS Iva_20,  ")
            stb.AppendLine("        SUM(ROUND(Imponibile_21, 2))  AS Imponibile_21,  ")
            stb.AppendLine("        SUM(ROUND(Iva_21, 2))  AS Iva_21, ")
            stb.AppendLine("        SUM(ROUND(Esente, 2)) AS Esente ")

            stb.AppendLine(" FROM ")

            '/*************************************************************************************
            '/************************* QUERY INTERNA *************************************
            stb.AppendLine(" ( ")
            stb.AppendLine("    SELECT  Agenda.PIVA, Imprese.Rag_Soc AS Impresa, Lav_Cod, Movimenti_Contab.Data_Movimento , ")

            stb.AppendLine("    CASE WHEN Movimenti_dettagli.Cod_Iva =4  THEN  Movimenti_dettagli.Imponibile_Netto ELSE 0 END AS Imponibile_4,  ")
            stb.AppendLine("    CASE WHEN Movimenti_dettagli.Cod_Iva =4  THEN  Movimenti_dettagli.Iva ELSE 0 END AS Iva_4 , ")
            stb.AppendLine("    CASE WHEN Movimenti_dettagli.Cod_Iva =10  THEN  Movimenti_dettagli.Imponibile_Netto ELSE 0 END AS Imponibile_10,  ")
            stb.AppendLine("    CASE WHEN Movimenti_dettagli.Cod_Iva =10  THEN  Movimenti_dettagli.Iva ELSE 0 END AS Iva_10, ")
            stb.AppendLine("    CASE WHEN Movimenti_dettagli.Cod_Iva =12  THEN  Movimenti_dettagli.Imponibile_Netto ELSE 0 END AS Imponibile_12,  ")
            stb.AppendLine("    CASE WHEN Movimenti_dettagli.Cod_Iva =12  THEN  Movimenti_dettagli.Iva ELSE 0 END AS Iva_12, ")
            stb.AppendLine("    CASE WHEN Movimenti_dettagli.Cod_Iva =20 THEN  Movimenti_dettagli.Imponibile_Netto ELSE 0 END AS Imponibile_20, ")
            stb.AppendLine("    CASE WHEN Movimenti_dettagli.Cod_Iva =20  THEN  Movimenti_dettagli.Iva ELSE 0 END AS Iva_20, ")
            stb.AppendLine("    CASE WHEN Movimenti_dettagli.Cod_Iva =21 THEN  Movimenti_dettagli.Imponibile_Netto ELSE 0 END AS Imponibile_21, ")
            stb.AppendLine("    CASE WHEN Movimenti_dettagli.Cod_Iva =21  THEN  Movimenti_dettagli.Iva ELSE 0 END AS Iva_21, ")
            stb.AppendLine("    CASE WHEN Movimenti_dettagli.Cod_Iva NOT IN (4,10,12,20,21)  THEN  Movimenti_dettagli.Imponibile_Netto ELSE 0 END AS Esente ")

            stb.AppendLine("    FROM Agenda ")

            stb.AppendLine("    INNER JOIN Imprese ON Imprese.Piva = Agenda.Piva  ")

            stb.AppendLine("    INNER JOIN Movimenti Movimenti_Mag  ")
            stb.AppendLine("    ON Agenda.PIVA = Movimenti_Mag.PIVA AND Agenda.Id_Agenda = Movimenti_Mag.Id_Agenda ")

            stb.AppendLine("    INNER JOIN Movimenti Movimenti_Contab  ")
            stb.AppendLine("    ON Agenda.PIVA = Movimenti_Contab.PIVA AND Agenda.Id_Agenda = Movimenti_Contab.Id_Agenda ")

            stb.AppendLine("    INNER JOIN Movimenti_dettagli  ")
            stb.AppendLine("    ON Movimenti_dettagli.PIVA = Movimenti_Mag.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti_Mag.Id_Agenda  ")
            stb.AppendLine("    AND Movimenti_dettagli.Id_Mov = Movimenti_Mag.Id_Mov  ")

            stb.AppendLine("    INNER JOIN IVA_Aliquote ON IVA_Aliquote.Codice = Movimenti_dettagli.Cod_Iva    ")
            stb.AppendLine("    INNER JOIN IVA_Aliquote IVA_AliquoteInd ON IVA_AliquoteInd.Codice = Movimenti_dettagli.Cod_IvaIndetraibile    ")

            'CONDIZIONI
            stb.AppendLine("    WHERE  Movimenti_Contab.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            stb.AppendLine("    AND    Movimenti_Contab.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            stb.AppendLine(" AND    Agenda.Lav_Cod IN ( " & LAVCOD_VENDITA & ", " &
                                                        LAVCOD_RICEVUTA_EMESSA & ", " &
                                                        LAVCOD_CORRISPETTIVO_VENDITA_SFUSO & ", " &
                                                        LAVCOD_DDT_CONTABILIZZATO_EMESSO &
                                                        "  ) ")

            stb.AppendLine("    AND    Movimenti_Mag.Cau_Mov = '" & CAU_SCARICO & "'  ")
            stb.AppendLine("    AND    Movimenti_Contab.Cau_Mov = '" & CAU_REGISTRAZIONI & "'  ")

            stb.AppendLine("    AND    Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            stb.AppendLine("    AND    Agenda.Sa_Cod = 0 ")

            If Sezionale_Cod <> -1 Then
                stb.AppendLine(" AND    Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            stb.AppendLine(" ) CORRISPETTIVI ")
            '/************************* QUERY INTERNA *************************************
            '/*************************************************************************************

            stb.AppendLine(" GROUP BY PIVA,  Impresa,  Data_Movimento  ")
            stb.AppendLine(" ORDER BY Data_Movimento ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' legge 
    '''  corrispettivi di vendita, lav_cod = 1020
    '''  ricevute fiscali, lav_cod = 1053
    '''  ddt contabilizzati, lav_cod = 1069
    '''  autoconsumi, lav_cod = 1028 e 1066
    ''' 
    ''' Flag_TipoQuery: 0 -> query per popolare il gruppo (ogni aliquota)
    '''                 1 -> query per popolare il dt_iva_round (c'è sia aliquota che % compensazione)
    '''                 2 -> query per totale giornata
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function RegistroCorrispettivi_ReleaseArrotondamenti2019(ByVal Piva As String,
                                                                    ByVal Sezionale_Cod As Integer,
                                                                    ByVal Validita_Inizio As String,
                                                                    ByVal Validita_Fine As String,
                                                                    ByVal xFiltroAggiuntivo As String,
                                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                    ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreStampeDAL.RegistriContab.RegistroCorrispettivi_ReleaseArrotondamenti2019"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            'Select Case Flag_TipoQuery
            '    Case 0, 1, 2
            '    Case Else
            '        Throw New Exception("Flag_TipoQuery non corretto")
            'End Select

            stb.Length = 0

            '/*************************************************************************************
            '/************** CORRISPETTIVI VENDITA  e RICEVUTE FISCALI *****************************
            '/*************************************************************************************

            'Select Case Flag_TipoQuery

            '    Case 0 'query per popolare il gruppo (ogni aliquota)
            '        stb.AppendLine(" -- query per popolare il gruppo (ogni aliquota)  ")
            '        stb.AppendLine(" SELECT Data_Movimento, aliquota, Sigla_IVA, Cod_Iva,  ")
            '        stb.AppendLine(" -- SUM(Imponibile_Netto_V) AS Imponibile_Netto_V,")
            '        stb.AppendLine(" SUM(Imponibile_Netto_AUTOCONSUMO) AS Imponibile_Netto_AUTOCONSUMO,")
            '        stb.AppendLine(" SUM(Imponibile_Netto_OMAGGI) AS Imponibile_Netto_OMAGGI,")
            '        stb.AppendLine(" SUM(Imponibile_Netto) AS Imponibile_Netto,")
            '        stb.AppendLine(" ")
            '        stb.AppendLine(" -- SUM(iva_V ) AS iva_V,")
            '        stb.AppendLine(" SUM(Iva_AUTOCONSUMO ) AS Iva_AUTOCONSUMO,")
            '        stb.AppendLine(" SUM(Iva_OMAGGI ) AS Iva_OMAGGI,")
            '        stb.AppendLine(" SUM(iva ) AS iva,")
            '        stb.AppendLine(" ")
            '        stb.AppendLine(" --SUM(Imponibile_Netto_V) + SUM(Iva_V ) AS Importo_V,")
            '        stb.AppendLine(" SUM(Imponibile_Netto_AUTOCONSUMO) + SUM(Iva_AUTOCONSUMO ) AS Importo_AUTOCONSUMO,")
            '        stb.AppendLine(" SUM(Imponibile_Netto_OMAGGI) + SUM(Iva_OMAGGI) AS Importo_OMAGGI,")
            '        stb.AppendLine(" SUM(Imponibile_Netto) + SUM(Iva ) AS Importo")
            '        stb.AppendLine(" ")
            '    Case 1 'query per popolare il sottoreport iva (c'è sia aliquota che % compensazione)
            '        stb.AppendLine(" -- query per popolare il sottoreport iva (c'è sia aliquota che % compensazione)  ")
            '        stb.AppendLine(" SELECT aliquota, Sigla_IVA, Cod_Iva, Iva_Indetraibile_Perc,  ")
            '        stb.AppendLine("        SUM(Iva_Indetraibile) AS Iva_Indetraibile, ")
            '        stb.AppendLine(" ")
            '        stb.AppendLine(" -- SUM(Imponibile_Netto_V) AS Imponibile_Netto_V,")
            '        stb.AppendLine(" SUM(Imponibile_Netto_AUTOCONSUMO) AS Imponibile_Netto_AUTOCONSUMO,")
            '        stb.AppendLine(" SUM(Imponibile_Netto_OMAGGI) AS Imponibile_Netto_OMAGGI,")
            '        stb.AppendLine(" SUM(Imponibile_Netto) AS Imponibile_Netto,")
            '        stb.AppendLine(" ")
            '        stb.AppendLine(" -- SUM(iva_V ) AS iva_V,")
            '        stb.AppendLine(" SUM(Iva_AUTOCONSUMO ) AS Iva_AUTOCONSUMO,")
            '        stb.AppendLine(" SUM(Iva_OMAGGI ) AS Iva_OMAGGI,")
            '        stb.AppendLine(" SUM(iva ) AS iva,")
            '        stb.AppendLine(" ")
            '        stb.AppendLine(" --SUM(Imponibile_Netto_V) + SUM(Iva_V ) AS Importo_V,")
            '        stb.AppendLine(" SUM(Imponibile_Netto_AUTOCONSUMO) + SUM(Iva_AUTOCONSUMO ) AS Importo_AUTOCONSUMO,")
            '        stb.AppendLine(" SUM(Imponibile_Netto_OMAGGI) + SUM(Iva_OMAGGI) AS Importo_OMAGGI,")
            '        stb.AppendLine(" SUM(Imponibile_Netto) + SUM(Iva ) AS Importo")
            '        stb.AppendLine(" ")
            '    Case 2 'query per totale giornata
            '        stb.AppendLine(" -- query per totale giornata ")
            '        stb.AppendLine(" SELECT Data_Movimento, ")
            '        stb.AppendLine(" -- SUM(Imponibile_Netto_V) + SUM(Iva_V ) AS Importo_V,")
            '        stb.AppendLine(" SUM(Imponibile_Netto_AUTOCONSUMO) + SUM(Iva_AUTOCONSUMO ) AS Importo_AUTOCONSUMO, ")
            '        stb.AppendLine(" SUM(Imponibile_Netto_OMAGGI) + SUM(Iva_OMAGGI) AS Importo_OMAGGI, ")
            '        stb.AppendLine(" SUM(Imponibile_Netto) + SUM(Iva ) AS Importo")
            '        stb.AppendLine(" ")
            'End Select

            stb.AppendLine(" ")
            'stb.AppendLine(" FROM ")
            'stb.AppendLine(" ( ")
            stb.AppendLine(" ")
            stb.AppendLine(" -- CORRISPETTIVI VENDITA, RICEVUTE FISCALI, DDT CONTABILIZZATI ")
            stb.AppendLine(" SELECT Agenda.Id_Agenda, Agenda.lav_cod, id_mov_det, Movimenti_Contab.Num_Protocollo, Movimenti_Contab.Tipo_sconto, Movimenti_Contab.Data_Movimento AS Data_Movimento, Movimenti_dettagli.Sconto_Modalita,  ")
            stb.AppendLine(" case when Sconto_Modalita = 0 THEN 'Sconto %' ")
            stb.AppendLine(" when Sconto_Modalita = 1 THEN 'Sconto Merce' ")
            stb.AppendLine("  when Sconto_Modalita = 2 THEN 'Omaggio senza rivalsa'  ")
            stb.AppendLine(" when Sconto_Modalita = 3 THEN 'Campione Gratuito'  ")
            stb.AppendLine(" when Sconto_Modalita = 4 THEN 'Omaggio con rivalsa' ")
            stb.AppendLine(" end  AS Sconto_Modalita_Desc , ")
            stb.AppendLine(" IVA_Aliquote.aliquota, IVA_Aliquote.Sigla AS Sigla_IVA, ")
            stb.AppendLine(" Movimenti_dettagli.Cod_Iva, Movimenti_dettagli.Iva_Indetraibile_Perc, ")
            stb.AppendLine(" -- IVA IN COMPENSAZIONE")
            stb.AppendLine(" Movimenti_dettagli.Iva_Indetraibile, ")
            stb.AppendLine(" -- IMPONIBILE")
            stb.AppendLine(" Movimenti_dettagli.Imponibile,  Movimenti_dettagli.Imponibile_Netto,")
            stb.AppendLine(" -- IVA")
            stb.AppendLine("  Movimenti_dettagli.Iva")
            stb.AppendLine("  , ChkLayOut_Hide, Sconto, Sconto_listino,qta,Prezzo_Unitario,Prezzo_Unitario_Netto ")
            'FROM
            stb.AppendLine("    FROM Agenda ")
            stb.AppendLine("    INNER JOIN Movimenti Movimenti_Mag  ")
            stb.AppendLine("    ON Agenda.PIVA = Movimenti_Mag.PIVA AND Agenda.Id_Agenda = Movimenti_Mag.Id_Agenda ")
            stb.AppendLine("    INNER JOIN Movimenti Movimenti_Contab  ")
            stb.AppendLine("    ON Agenda.PIVA = Movimenti_Contab.PIVA AND Agenda.Id_Agenda = Movimenti_Contab.Id_Agenda ")
            stb.AppendLine("    INNER JOIN Movimenti_dettagli  ")
            stb.AppendLine("    ON Movimenti_dettagli.PIVA = Movimenti_Mag.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti_Mag.Id_Agenda  ")
            stb.AppendLine("    AND Movimenti_dettagli.Id_Mov = Movimenti_Mag.Id_Mov  ")
            stb.AppendLine("    INNER JOIN IVA_Aliquote ON IVA_Aliquote.Codice = Movimenti_dettagli.Cod_Iva    ")
            'CONDIZIONI
            stb.AppendLine("    WHERE  Movimenti_Contab.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            stb.AppendLine("    AND    Movimenti_Contab.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            stb.AppendLine(" AND    Agenda.Lav_Cod IN ( " & LAVCOD_VENDITA & ", " &
                                                        LAVCOD_CORRISPETTIVO_VENDITA_SFUSO & ", " &
                                                        LAVCOD_RICEVUTA_EMESSA & ", " &
                                                        LAVCOD_DDT_CONTABILIZZATO_EMESSO &
                                                        " ) ")

            stb.AppendLine("    AND    Movimenti_Mag.Cau_Mov = '" & CAU_SCARICO & "'  ")
            stb.AppendLine("    AND    Movimenti_Contab.Cau_Mov = '" & CAU_REGISTRAZIONI & "'  ")

            stb.AppendLine("    AND    Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            stb.AppendLine("    AND    Agenda.Sa_Cod = 0 ")

            If Sezionale_Cod <> -1 Then
                stb.AppendLine(" AND    Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
            End If

            'If xFiltroAggiuntivo <> "" Then
            '    stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            'End If

            'stb.AppendLine(" GROUP BY  Movimenti_dettagli.Cod_Iva , Movimenti_Contab.data_movimento,Movimenti_dettagli.Sconto_Modalita,LAV_COD, ")
            'stb.AppendLine(" IVA_Aliquote.aliquota, IVA_Aliquote.Sigla ,  Movimenti_dettagli.Cod_Iva, Movimenti_dettagli.Iva_Indetraibile_Perc ")
            stb.AppendLine(" ")
            stb.AppendLine(" -----------------------------------------------------")
            stb.AppendLine(" UNION ALL ")
            stb.AppendLine(" -----------------------------------------------------")
            stb.AppendLine(" ")

            stb.AppendLine(" -- AUTOCONSUMO ")
            stb.AppendLine(" SELECT Agenda.Id_Agenda, Agenda.lav_cod, id_mov_det, Movimenti_Mag.Num_Protocollo, Movimenti_Mag.Tipo_sconto, Movimenti_Mag.Data_Movimento AS Data_Movimento,  ")

            '01/02/2019: non si può usare questo trucco perché sconto_modalita viene utilizzata dalla FormAggiornaImportoNEW!!!!
            ''visto che nell'autoconsumo non si mettono gli omaggi, 
            ''nel campo sconto_modalita è stato messo il lav_cod per identificare gli autoconsumi
            'stb.AppendLine("  1028 AS Sconto_Modalita, 'Autoconsumo' AS Sconto_Modalita_Desc,  " & vbCrLf)

            stb.AppendLine("  Movimenti_dettagli.Sconto_Modalita, ")
            stb.AppendLine(" case when Sconto_Modalita = 0 THEN 'Sconto %' ")
            stb.AppendLine(" when Sconto_Modalita = 1 THEN 'Sconto Merce' ")
            stb.AppendLine("  when Sconto_Modalita = 2 THEN 'Omaggio senza rivalsa'  ")
            stb.AppendLine(" when Sconto_Modalita = 3 THEN 'Campione Gratuito'  ")
            stb.AppendLine(" when Sconto_Modalita = 4 THEN 'Omaggio con rivalsa' ")
            stb.AppendLine(" end  AS Sconto_Modalita_Desc , ")

            stb.AppendLine(" IVA_Aliquote.aliquota, IVA_Aliquote.Sigla AS Sigla_IVA, ")
            stb.AppendLine(" Movimenti_dettagli.Cod_Iva, Movimenti_dettagli.Iva_Indetraibile_Perc, ")
            stb.AppendLine(" -- IVA IN COMPENSAZIONE")
            stb.AppendLine(" Iva_Indetraibile, ")
            stb.AppendLine(" -- IMPONIBILE NETTO")
            stb.AppendLine("   Movimenti_dettagli.Imponibile,  Movimenti_dettagli.Imponibile_Netto, ")
            stb.AppendLine(" -- IVA")
            stb.AppendLine("  Movimenti_dettagli.Iva ")
            stb.AppendLine("  ,ChkLayOut_Hide, Sconto, Sconto_listino,qta,Prezzo_Unitario,Prezzo_Unitario_Netto ")
            'FROM
            stb.AppendLine("    FROM Agenda ")
            stb.AppendLine("    INNER JOIN Movimenti Movimenti_Mag  ")
            stb.AppendLine("    ON Agenda.PIVA = Movimenti_Mag.PIVA AND Agenda.Id_Agenda = Movimenti_Mag.Id_Agenda ")
            stb.AppendLine("    INNER JOIN Movimenti_dettagli  ")
            stb.AppendLine("    ON Movimenti_dettagli.PIVA = Movimenti_Mag.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti_Mag.Id_Agenda  ")
            stb.AppendLine("    AND Movimenti_dettagli.Id_Mov = Movimenti_Mag.Id_Mov  ")
            stb.AppendLine("    INNER JOIN IVA_Aliquote ON IVA_Aliquote.Codice = Movimenti_dettagli.Cod_Iva    ")
            'CONDIZIONI
            stb.AppendLine("    WHERE  Movimenti_Mag.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            stb.AppendLine("    AND    Movimenti_Mag.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            stb.AppendLine(" AND    Agenda.Lav_Cod IN ( " & LAVCOD_AUTOCONSUMO & ", " & LAVCOD_AUTOCONSUMO_VINO_SFUSO & "  ) ")

            stb.AppendLine("    AND    Movimenti_Mag.Cau_Mov = '" & CAU_SCARICO & "'  ")
            stb.AppendLine("    AND    Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            'stb.AppendLine("    AND    Agenda.Sa_Cod = 0 ")

            If Sezionale_Cod <> -1 Then
                stb.AppendLine(" AND    Movimenti_Mag.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
            End If

            'stb.AppendLine(" GROUP BY  Movimenti_dettagli.Cod_Iva, Movimenti_Mag.data_movimento, Movimenti_dettagli.Sconto_Modalita,  ")
            'stb.AppendLine(" IVA_Aliquote.aliquota, IVA_Aliquote.Sigla ,Movimenti_dettagli.Cod_Iva,  Movimenti_dettagli.Iva_Indetraibile_Perc ")
            ' stb.AppendLine(" --ORDER BY Movimenti_dettagli.Cod_Iva , data_movimento, Movimenti_dettagli.Sconto_Modalita")
            stb.AppendLine(" ")
            'stb.AppendLine(" ) AS REG_CORRISPETTIVI ")

            'Select Case Flag_TipoQuery

            '    Case 0
            '        stb.AppendLine(" ")
            '        'stb.AppendLine(" GROUP BY  Cod_Iva, data_movimento,aliquota, Sigla_IVA  ")
            ' stb.AppendLine(" ORDER BY Cod_Iva, data_movimento ")
            '        stb.AppendLine(" ")
            '    Case 1
            '        stb.AppendLine(" ")
            '        'stb.AppendLine(" GROUP BY  Cod_Iva, aliquota, Sigla_IVA, Iva_Indetraibile_Perc  ")
            '        stb.AppendLine(" ORDER BY Cod_Iva ")
            '        stb.AppendLine(" ")
            '    Case 2
            '        stb.AppendLine(" ")
            '        'stb.AppendLine(" GROUP BY data_movimento")
            '        stb.AppendLine(" ORDER BY data_movimento ")
            '        stb.AppendLine(" ")
            'End Select

            'l'ordinamento serve per id_agenda per elaborare tutti i dettagli della stessa operazione (con la classe round)
            'poi il cod_iva perché serve per il gruppo del report
            'data e sconto per calcolare gli importi degli omaggi
            stb.AppendLine(" ORDER BY Agenda.Id_Agenda, Movimenti_dettagli.Cod_Iva, data_movimento, sconto_modalita ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' legge 
    '''  corrispettivi di vendita, lav_cod = 1020
    '''  ricevute fiscali, lav_cod = 1053
    '''  ddt contabilizzati, lav_cod = 1069
    '''  autoconsumi, lav_cod = 1028 e 1066
    ''' 
    ''' Flag_TipoQuery: 0 -> query per popolare il gruppo (ogni aliquota)
    '''                 1 -> query per popolare il dt_iva_round (c'è sia aliquota che % compensazione)
    '''                 2 -> query per totale giornata
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function RegistroCorrispettivi_ReleaseArrotondamenti2018(ByVal Flag_TipoQuery As Integer,
                                                                ByVal Piva As String,
                                                                ByVal Sezionale_Cod As Integer,
                                                                ByVal Validita_Inizio As String,
                                                                ByVal Validita_Fine As String,
                                                                ByVal xFiltroAggiuntivo As String,
                                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreStampeDAL.RegistriContab.RegistroCorrispettivi_ReleaseArrotondamenti2018"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            Select Case Flag_TipoQuery
                Case 0, 1, 2
                Case Else
                    Throw New Exception("Flag_TipoQuery non corretto")
            End Select

            stb.Length = 0

            '/*************************************************************************************
            '/************************* CORRISPETTIVI VENDITA *************************************
            '/*************************************************************************************

            Select Case Flag_TipoQuery

                Case 0 'query per popolare il gruppo (ogni aliquota)
                    stb.AppendLine(" -- query per popolare il gruppo (ogni aliquota)  " & vbCrLf)
                    stb.AppendLine(" SELECT Data_Movimento, aliquota, Sigla_IVA, Cod_Iva,  " & vbCrLf)
                    stb.AppendLine(" -- SUM(Imponibile_Netto_V) AS Imponibile_Netto_V," & vbCrLf)
                    stb.AppendLine(" SUM(Imponibile_Netto_AUTOCONSUMO) AS Imponibile_Netto_AUTOCONSUMO," & vbCrLf)
                    stb.AppendLine(" SUM(Imponibile_Netto_OMAGGI) AS Imponibile_Netto_OMAGGI," & vbCrLf)
                    stb.AppendLine(" SUM(Imponibile_Netto) AS Imponibile_Netto," & vbCrLf)
                    stb.AppendLine(" " & vbCrLf)
                    stb.AppendLine(" -- SUM(iva_V ) AS iva_V," & vbCrLf)
                    stb.AppendLine(" SUM(Iva_AUTOCONSUMO ) AS Iva_AUTOCONSUMO," & vbCrLf)
                    stb.AppendLine(" SUM(Iva_OMAGGI ) AS Iva_OMAGGI," & vbCrLf)
                    stb.AppendLine(" SUM(iva ) AS iva," & vbCrLf)
                    stb.AppendLine(" " & vbCrLf)
                    stb.AppendLine(" --SUM(Imponibile_Netto_V) + SUM(Iva_V ) AS Importo_V," & vbCrLf)
                    stb.AppendLine(" SUM(Imponibile_Netto_AUTOCONSUMO) + SUM(Iva_AUTOCONSUMO ) AS Importo_AUTOCONSUMO," & vbCrLf)
                    stb.AppendLine(" SUM(Imponibile_Netto_OMAGGI) + SUM(Iva_OMAGGI) AS Importo_OMAGGI," & vbCrLf)
                    stb.AppendLine(" SUM(Imponibile_Netto) + SUM(Iva ) AS Importo" & vbCrLf)
                    stb.AppendLine(" " & vbCrLf)
                Case 1 'query per popolare il sottoreport iva (c'è sia aliquota che % compensazione)
                    stb.AppendLine(" -- query per popolare il sottoreport iva (c'è sia aliquota che % compensazione)  " & vbCrLf)
                    stb.AppendLine(" SELECT aliquota, Sigla_IVA, Cod_Iva, Iva_Indetraibile_Perc,  " & vbCrLf)
                    stb.AppendLine("        SUM(Iva_Indetraibile) AS Iva_Indetraibile, " & vbCrLf)
                    stb.AppendLine(" " & vbCrLf)
                    stb.AppendLine(" -- SUM(Imponibile_Netto_V) AS Imponibile_Netto_V," & vbCrLf)
                    stb.AppendLine(" SUM(Imponibile_Netto_AUTOCONSUMO) AS Imponibile_Netto_AUTOCONSUMO," & vbCrLf)
                    stb.AppendLine(" SUM(Imponibile_Netto_OMAGGI) AS Imponibile_Netto_OMAGGI," & vbCrLf)
                    stb.AppendLine(" SUM(Imponibile_Netto) AS Imponibile_Netto," & vbCrLf)
                    stb.AppendLine(" " & vbCrLf)
                    stb.AppendLine(" -- SUM(iva_V ) AS iva_V," & vbCrLf)
                    stb.AppendLine(" SUM(Iva_AUTOCONSUMO ) AS Iva_AUTOCONSUMO," & vbCrLf)
                    stb.AppendLine(" SUM(Iva_OMAGGI ) AS Iva_OMAGGI," & vbCrLf)
                    stb.AppendLine(" SUM(iva ) AS iva," & vbCrLf)
                    stb.AppendLine(" " & vbCrLf)
                    stb.AppendLine(" --SUM(Imponibile_Netto_V) + SUM(Iva_V ) AS Importo_V," & vbCrLf)
                    stb.AppendLine(" SUM(Imponibile_Netto_AUTOCONSUMO) + SUM(Iva_AUTOCONSUMO ) AS Importo_AUTOCONSUMO," & vbCrLf)
                    stb.AppendLine(" SUM(Imponibile_Netto_OMAGGI) + SUM(Iva_OMAGGI) AS Importo_OMAGGI," & vbCrLf)
                    stb.AppendLine(" SUM(Imponibile_Netto) + SUM(Iva ) AS Importo" & vbCrLf)
                    stb.AppendLine(" " & vbCrLf)
                Case 2 'query per totale giornata
                    stb.AppendLine(" -- query per totale giornata " & vbCrLf)
                    stb.AppendLine(" SELECT Data_Movimento, " & vbCrLf)
                    stb.AppendLine(" -- SUM(Imponibile_Netto_V) + SUM(Iva_V ) AS Importo_V," & vbCrLf)
                    stb.AppendLine(" SUM(Imponibile_Netto_AUTOCONSUMO) + SUM(Iva_AUTOCONSUMO ) AS Importo_AUTOCONSUMO, " & vbCrLf)
                    stb.AppendLine(" SUM(Imponibile_Netto_OMAGGI) + SUM(Iva_OMAGGI) AS Importo_OMAGGI, " & vbCrLf)
                    stb.AppendLine(" SUM(Imponibile_Netto) + SUM(Iva ) AS Importo" & vbCrLf)
                    stb.AppendLine(" " & vbCrLf)
            End Select

            stb.AppendLine(" " & vbCrLf)
            stb.AppendLine(" FROM " & vbCrLf)
            stb.AppendLine(" ( " & vbCrLf)
            stb.AppendLine(" " & vbCrLf)
            stb.AppendLine(" SELECT Movimenti_Contab.Data_Movimento, Movimenti_dettagli.Sconto_Modalita,   " & vbCrLf)
            stb.AppendLine(" case when [Sconto_Modalita] = 0 THEN 'Sconto %' " & vbCrLf)
            stb.AppendLine(" when [Sconto_Modalita] = 1 THEN 'Sconto Merce' " & vbCrLf)
            stb.AppendLine("  when [Sconto_Modalita] = 2 THEN 'Omaggio senza rivalsa'  " & vbCrLf)
            stb.AppendLine(" when [Sconto_Modalita] = 3 THEN 'Campione Gratuito'  " & vbCrLf)
            stb.AppendLine(" when [Sconto_Modalita] = 4 THEN 'Omaggio con rivalsa' " & vbCrLf)
            stb.AppendLine(" end  AS Tipo_Sconto , " & vbCrLf)
            stb.AppendLine(" IVA_Aliquote.aliquota, IVA_Aliquote.Sigla AS Sigla_IVA, " & vbCrLf)
            stb.AppendLine(" Movimenti_dettagli.Cod_Iva, Movimenti_dettagli.Iva_Indetraibile_Perc, " & vbCrLf)
            'stb.AppendLine(" SUM(Movimenti_dettagli.Imponibile_Netto ) AS Imponibile_Netto_V, " & vbCrLf)
            stb.AppendLine(" -- IMPONIBILE" & vbCrLf)
            stb.AppendLine(" 0   AS Imponibile_Netto_AUTOCONSUMO," & vbCrLf)
            stb.AppendLine(" CASE   WHEN (Movimenti_dettagli.Sconto_Modalita = 4 OR Movimenti_dettagli.Sconto_Modalita = 2) " & vbCrLf)
            stb.AppendLine("        THEN SUM(Movimenti_dettagli.Imponibile_Netto) " & vbCrLf)
            stb.AppendLine("        ELSE 0 END  AS Imponibile_Netto_OMAGGI, " & vbCrLf)
            stb.AppendLine(" CASE   WHEN (Movimenti_dettagli.Sconto_Modalita = 4 OR Movimenti_dettagli.Sconto_Modalita = 2)  " & vbCrLf)
            stb.AppendLine("        THEN 0 " & vbCrLf)
            stb.AppendLine("        ELSE SUM(Movimenti_dettagli.Imponibile_Netto)  END  AS Imponibile_Netto, " & vbCrLf)
            stb.AppendLine(" -- IVA" & vbCrLf)
            'stb.AppendLine(" SUM(Movimenti_dettagli.Iva * -1) AS iva_V," & vbCrLf)
            stb.AppendLine(" 0   AS Iva_AUTOCONSUMO," & vbCrLf)
            stb.AppendLine(" CASE   WHEN (Movimenti_dettagli.Sconto_Modalita = 4 OR Movimenti_dettagli.Sconto_Modalita = 2)  " & vbCrLf)
            stb.AppendLine("        THEN SUM(Movimenti_dettagli.Iva * -1) " & vbCrLf)
            stb.AppendLine("        ELSE 0 END  AS Iva_OMAGGI, " & vbCrLf)
            stb.AppendLine(" CASE   WHEN (Movimenti_dettagli.Sconto_Modalita = 4 OR Movimenti_dettagli.Sconto_Modalita = 2) " & vbCrLf)
            stb.AppendLine("        THEN 0 " & vbCrLf)
            stb.AppendLine("        ELSE SUM(Movimenti_dettagli.Iva * -1)  END  AS Iva, " & vbCrLf)
            stb.AppendLine(" -- IVA IN COMPENSAZIONE" & vbCrLf)
            stb.AppendLine(" SUM(Movimenti_dettagli.Iva_Indetraibile) AS Iva_Indetraibile " & vbCrLf)
            stb.AppendLine(" " & vbCrLf)
            'FROM
            stb.AppendLine("    FROM Agenda ")
            stb.AppendLine("    INNER JOIN Movimenti Movimenti_Mag  ")
            stb.AppendLine("    ON Agenda.PIVA = Movimenti_Mag.PIVA AND Agenda.Id_Agenda = Movimenti_Mag.Id_Agenda ")
            stb.AppendLine("    INNER JOIN Movimenti Movimenti_Contab  ")
            stb.AppendLine("    ON Agenda.PIVA = Movimenti_Contab.PIVA AND Agenda.Id_Agenda = Movimenti_Contab.Id_Agenda ")
            stb.AppendLine("    INNER JOIN Movimenti_dettagli  ")
            stb.AppendLine("    ON Movimenti_dettagli.PIVA = Movimenti_Mag.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti_Mag.Id_Agenda  ")
            stb.AppendLine("    AND Movimenti_dettagli.Id_Mov = Movimenti_Mag.Id_Mov  ")
            stb.AppendLine("    INNER JOIN IVA_Aliquote ON IVA_Aliquote.Codice = Movimenti_dettagli.Cod_Iva    ")
            'CONDIZIONI
            stb.AppendLine("    WHERE  Movimenti_Contab.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            stb.AppendLine("    AND    Movimenti_Contab.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            stb.AppendLine(" AND    Agenda.Lav_Cod IN ( " & LAVCOD_VENDITA & ", " &
                                                        LAVCOD_CORRISPETTIVO_VENDITA_SFUSO & ", " &
                                                        LAVCOD_RICEVUTA_EMESSA & ", " &
                                                        LAVCOD_DDT_CONTABILIZZATO_EMESSO &
                                                        " ) ")

            stb.AppendLine("    AND    Movimenti_Mag.Cau_Mov = '" & CAU_SCARICO & "'  ")
            stb.AppendLine("    AND    Movimenti_Contab.Cau_Mov = '" & CAU_REGISTRAZIONI & "'  ")

            stb.AppendLine("    AND    Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            stb.AppendLine("    AND    Agenda.Sa_Cod = 0 ")

            If Sezionale_Cod <> -1 Then
                stb.AppendLine(" AND    Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
            End If

            'If xFiltroAggiuntivo <> "" Then
            '    stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            'End If

            stb.AppendLine(" " & vbCrLf)

            stb.AppendLine(" GROUP BY  Movimenti_dettagli.Cod_Iva , Movimenti_Contab.data_movimento,Movimenti_dettagli.Sconto_Modalita,LAV_COD, " & vbCrLf)
            stb.AppendLine(" IVA_Aliquote.aliquota, IVA_Aliquote.Sigla ,  Movimenti_dettagli.Cod_Iva, Movimenti_dettagli.Iva_Indetraibile_Perc " & vbCrLf)
            stb.AppendLine(" " & vbCrLf)
            stb.AppendLine(" -----------------------------------------------------" & vbCrLf)
            stb.AppendLine(" UNION ALL " & vbCrLf)
            stb.AppendLine(" -----------------------------------------------------" & vbCrLf)

            stb.AppendLine(" SELECT Movimenti_Mag.Data_Movimento, Movimenti_dettagli.Sconto_Modalita,   " & vbCrLf)
            stb.AppendLine(" case when [Sconto_Modalita] = 0 THEN 'Sconto %' " & vbCrLf)
            stb.AppendLine(" when [Sconto_Modalita] = 1 THEN 'Sconto Merce' " & vbCrLf)
            stb.AppendLine("  when [Sconto_Modalita] = 2 THEN 'Omaggio senza rivalsa'  " & vbCrLf)
            stb.AppendLine(" when [Sconto_Modalita] = 3 THEN 'Campione Gratuito'  " & vbCrLf)
            stb.AppendLine(" when [Sconto_Modalita] = 4 THEN 'Omaggio con rivalsa' " & vbCrLf)
            stb.AppendLine(" end  AS Tipo_Sconto , " & vbCrLf)
            stb.AppendLine(" IVA_Aliquote.aliquota, IVA_Aliquote.Sigla AS Sigla_IVA, " & vbCrLf)
            stb.AppendLine(" Movimenti_dettagli.Cod_Iva, Movimenti_dettagli.Iva_Indetraibile_Perc, " & vbCrLf)
            'stb.AppendLine(" SUM(Movimenti_dettagli.Imponibile_Netto ) AS Imponibile_Netto_V, " & vbCrLf)
            stb.AppendLine(" -- IMPONIBILE NETTO" & vbCrLf)
            stb.AppendLine(" SUM(Movimenti_dettagli.Imponibile_Netto) AS Imponibile_Netto_AUTOCONSUMO, " & vbCrLf)
            stb.AppendLine(" 0   AS Imponibile_Netto_OMAGGI, " & vbCrLf)
            stb.AppendLine(" 0  AS Imponibile_Netto," & vbCrLf)
            stb.AppendLine(" -- IVA" & vbCrLf)
            'stb.AppendLine(" SUM(Movimenti_dettagli.Iva * -1) AS iva_V," & vbCrLf)
            stb.AppendLine(" SUM(Movimenti_dettagli.Iva * -1) AS Iva_AUTOCONSUMO, " & vbCrLf)
            stb.AppendLine(" 0   AS Iva_OMAGGI, " & vbCrLf)
            stb.AppendLine(" 0 AS Iva, " & vbCrLf)
            stb.AppendLine(" -- IVA IN COMPENSAZIONE" & vbCrLf)
            stb.AppendLine(" SUM(Movimenti_dettagli.Iva_Indetraibile) AS Iva_Indetraibile " & vbCrLf)
            stb.AppendLine(" " & vbCrLf)
            'FROM
            stb.AppendLine("    FROM Agenda ")
            stb.AppendLine("    INNER JOIN Movimenti Movimenti_Mag  ")
            stb.AppendLine("    ON Agenda.PIVA = Movimenti_Mag.PIVA AND Agenda.Id_Agenda = Movimenti_Mag.Id_Agenda ")
            stb.AppendLine("    INNER JOIN Movimenti_dettagli  ")
            stb.AppendLine("    ON Movimenti_dettagli.PIVA = Movimenti_Mag.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti_Mag.Id_Agenda  ")
            stb.AppendLine("    AND Movimenti_dettagli.Id_Mov = Movimenti_Mag.Id_Mov  ")
            stb.AppendLine("    INNER JOIN IVA_Aliquote ON IVA_Aliquote.Codice = Movimenti_dettagli.Cod_Iva    ")
            'CONDIZIONI
            stb.AppendLine("    WHERE  Movimenti_Mag.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            stb.AppendLine("    AND    Movimenti_Mag.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            stb.AppendLine(" AND    Agenda.Lav_Cod IN ( " & LAVCOD_AUTOCONSUMO & ", " & LAVCOD_AUTOCONSUMO_VINO_SFUSO & "  ) ")

            stb.AppendLine("    AND    Movimenti_Mag.Cau_Mov = '" & CAU_SCARICO & "'  ")
            stb.AppendLine("    AND    Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            'stb.AppendLine("    AND    Agenda.Sa_Cod = 0 ")

            If Sezionale_Cod <> -1 Then
                stb.AppendLine(" AND    Movimenti_Mag.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
            End If

            stb.AppendLine(" GROUP BY  Movimenti_dettagli.Cod_Iva, Movimenti_Mag.data_movimento, Movimenti_dettagli.Sconto_Modalita,  ")
            stb.AppendLine(" IVA_Aliquote.aliquota, IVA_Aliquote.Sigla ,Movimenti_dettagli.Cod_Iva,  Movimenti_dettagli.Iva_Indetraibile_Perc ")
            stb.AppendLine(" --ORDER BY Movimenti_dettagli.Cod_Iva , data_movimento, Movimenti_dettagli.Sconto_Modalita")
            stb.AppendLine(" ")
            stb.AppendLine(" ) AS REG_CORRISPETTIVI ")

            Select Case Flag_TipoQuery

                Case 0
                    stb.AppendLine(" " & vbCrLf)
                    stb.AppendLine(" GROUP BY  Cod_Iva, data_movimento,aliquota, Sigla_IVA  " & vbCrLf)
                    stb.AppendLine(" ORDER BY Cod_Iva, data_movimento " & vbCrLf)
                    stb.AppendLine(" " & vbCrLf)
                Case 1
                    stb.AppendLine(" " & vbCrLf)
                    stb.AppendLine(" GROUP BY  Cod_Iva, aliquota, Sigla_IVA, Iva_Indetraibile_Perc  " & vbCrLf)
                    stb.AppendLine(" ORDER BY Cod_Iva " & vbCrLf)
                    stb.AppendLine(" " & vbCrLf)
                Case 2
                    stb.AppendLine(" " & vbCrLf)
                    stb.AppendLine(" GROUP BY data_movimento" & vbCrLf)
                    stb.AppendLine(" ORDER BY data_movimento " & vbCrLf)
                    stb.AppendLine(" " & vbCrLf)
            End Select


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' legge 
    '''  corrispettivi di vendita, lav_cod = 1020
    '''  ricevute fiscali, lav_cod = 1053
    '''  ddt contabilizzati, lav_cod = 1069
    ''' 
    ''' Flag_TipoQuery: 0 -> select normale, query ufficiale
    '''                 1 -> select distinct cod_iva delle aliquote 
    '''                 2 -> select distinct cod_iva dei non imp e escl iva 
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function RegistroCorrispettivi_Importi_2(ByVal Flag_TipoQuery As Integer,
                                                    ByVal Piva As String,
                                                    ByVal Sezionale_Cod As Integer,
                                                    ByVal Validita_Inizio As String,
                                                    ByVal Validita_Fine As String,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                    ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreStampeDAL.RegistriContab.RegistroCorrispettivi_Importi_2"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            '/*************************************************************************************
            '/************************* CORRISPETTIVI VENDITA *************************************
            '/*************************************************************************************

            If Flag_TipoQuery = 0 Then

                stb.AppendLine("    SELECT  Agenda.PIVA, Agenda.id_agenda, Imprese.Rag_Soc AS Impresa, Lav_Cod, ")

                'movimenti
                stb.AppendLine("        Movimenti_Contab.Cod_RisUm,  Movimenti_Contab.Mov_Desc, Movimenti_Contab.Modalita, ")
                stb.AppendLine("        ISNULL(Movimenti_Contab.Num_Protocollo,0) AS Num_Protocollo, Movimenti_Contab.Tipo_Sconto,  ")
                stb.AppendLine("        Movimenti_Contab.Data_Movimento, Movimenti_Contab.Doc_Numero_Sin, Movimenti_Contab.Doc_Numero, Movimenti_Contab.Doc_Numero_Des, ")
                stb.AppendLine("        Movimenti_Contab.Progr_Protocollo, Movimenti_Contab.Progr_Registrazione, Movimenti_Contab.Data_Registrazione, ")
                stb.AppendLine("")
                'iva_aliquote
                'IVA_AliquoteInd.Sigla AS Sigla_IVAIndetraibile, 
                stb.AppendLine("        IVA_Aliquote.aliquota, IVA_Aliquote.Sigla AS Sigla_IVA, ")
                stb.AppendLine("")
                'movimenti_dettagli
                stb.AppendLine("        Movimenti_dettagli.Id_Mov_Det, Movimenti_dettagli.ChkLayOut_Hide, ")
                stb.AppendLine("        Movimenti_dettagli.sconto , Movimenti_dettagli.sconto_listino , Movimenti_dettagli.sconto_modalita , ")
                stb.AppendLine("        Movimenti_dettagli.Cod_Iva, --Movimenti_dettagli.Cod_IvaIndetraibile, ")
                stb.AppendLine("        Movimenti_dettagli.Iva_Indetraibile, Movimenti_dettagli.Iva_Indetraibile_Perc, ")
                stb.AppendLine("        Movimenti_dettagli.Qta, ")
                stb.AppendLine("         Movimenti_dettagli.Prezzo_Unitario, ")
                stb.AppendLine("        Movimenti_dettagli.Prezzo_Unitario_Netto, ")
                stb.AppendLine("         Movimenti_dettagli.Imponibile, ")
                stb.AppendLine("        Movimenti_dettagli.Imponibile_Netto, ")
                stb.AppendLine("        Movimenti_dettagli.Iva ")
                stb.AppendLine("")

            Else
                stb.AppendLine("   SELECT DISTINCT  Movimenti_dettagli.Cod_Iva, ")
                stb.AppendLine("                    IVA_Aliquote.aliquota, IVA_Aliquote.Sigla AS Sigla_IVA, IVA_Aliquote.Tipologia ")
            End If

            stb.AppendLine("    FROM Agenda ")

            stb.AppendLine("    INNER JOIN Imprese ON Imprese.Piva = Agenda.Piva  ")

            stb.AppendLine("    INNER JOIN Movimenti Movimenti_Mag  ")
            stb.AppendLine("    ON Agenda.PIVA = Movimenti_Mag.PIVA AND Agenda.Id_Agenda = Movimenti_Mag.Id_Agenda ")

            stb.AppendLine("    INNER JOIN Movimenti Movimenti_Contab  ")
            stb.AppendLine("    ON Agenda.PIVA = Movimenti_Contab.PIVA AND Agenda.Id_Agenda = Movimenti_Contab.Id_Agenda ")

            stb.AppendLine("    INNER JOIN Movimenti_dettagli  ")
            stb.AppendLine("    ON Movimenti_dettagli.PIVA = Movimenti_Mag.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti_Mag.Id_Agenda  ")
            stb.AppendLine("    AND Movimenti_dettagli.Id_Mov = Movimenti_Mag.Id_Mov  ")

            stb.AppendLine("    INNER JOIN IVA_Aliquote ON IVA_Aliquote.Codice = Movimenti_dettagli.Cod_Iva    ")

            '03/08/2015: Movimenti_dettagli.Cod_IvaIndetraibile va in join con IVA_CodiciIvaProdottiAgricoli
            'Stb.AppendLine("    INNER JOIN IVA_Aliquote IVA_AliquoteInd ON IVA_AliquoteInd.Codice = Movimenti_dettagli.Cod_IvaIndetraibile    ")

            'CONDIZIONI
            stb.AppendLine("    WHERE  Movimenti_Contab.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            stb.AppendLine("    AND    Movimenti_Contab.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            stb.AppendLine(" AND    Agenda.Lav_Cod IN ( " & LAVCOD_VENDITA & ", " &
                                                        LAVCOD_CORRISPETTIVO_VENDITA_SFUSO & ", " &
                                                        LAVCOD_RICEVUTA_EMESSA & ", " &
                                                        LAVCOD_DDT_CONTABILIZZATO_EMESSO &
                                                        "  ) ")

            stb.AppendLine("    AND    Movimenti_Mag.Cau_Mov = '" & CAU_SCARICO & "'  ")
            stb.AppendLine("    AND    Movimenti_Contab.Cau_Mov = '" & CAU_REGISTRAZIONI & "'  ")

            stb.AppendLine("    AND    Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            stb.AppendLine("    AND    Agenda.Sa_Cod = 0 ")

            If Sezionale_Cod <> -1 Then
                stb.AppendLine(" AND    Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If Flag_TipoQuery = enum_TipologiaIva.Aliquota Then
                'cerco solo le aliquote, non le esclusioni iva
                stb.AppendLine(" AND IVA_Aliquote.aliquota <> 0 ")
            End If
            If Flag_TipoQuery = enum_TipologiaIva.NonImponibile OrElse Flag_TipoQuery = enum_TipologiaIva.EsclusioneIva Then
                'cerco solo i non imp e le esclusioni iva 
                stb.AppendLine(" AND IVA_Aliquote.aliquota = 0 ")
            End If
            If Flag_TipoQuery = enum_TipologiaIva.NonImponibile Then
                stb.AppendLine(" AND IVA_Aliquote.Tipologia =  " & CStr(enum_TipologiaIva.NonImponibile))
            End If
            If Flag_TipoQuery = enum_TipologiaIva.EsclusioneIva Then
                stb.AppendLine(" AND IVA_Aliquote.Tipologia =  " & CStr(enum_TipologiaIva.EsclusioneIva))
            End If

            If Flag_TipoQuery = 0 Then
                'l'ordinamento è importante!!!!!!!
                stb.AppendLine(" ORDER BY Agenda.Id_Agenda, Movimenti_dettagli.Cod_Iva ")
            Else
                stb.AppendLine(" ORDER BY Movimenti_dettagli.Cod_Iva ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' legge, per ogni giorno dell'intervallo, il primo e l'ultimo numero (per ogni prefisso e suffisso)
    ''' dei corrispettivi di vendita, lav_cod = 1020
    ''' e delle ricevute fiscali, lav_cod = 1053
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function RegistroCorrispettivi_NumeriDocumento(ByVal Piva As String,
                                                          ByVal Sezionale_Cod As Integer,
                                                          ByVal Validita_Inizio As String,
                                                          ByVal Validita_Fine As String,
                                                          ByVal xFiltroAggiuntivo As String,
                                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                          ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreStampeDAL.RegistriContab.RegistroCorrispettivi_NumeriDocumento"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            '/*************************************************************************************
            '/************************* CORRISPETTIVI VENDITA *************************************
            '/*************************************************************************************

            stb.AppendLine("    SELECT  Agenda.PIVA, Imprese.Rag_Soc AS Impresa, Lav_Cod, Movimenti_Contab.Data_Movimento, ")
            stb.AppendLine("            Movimenti_Contab.doc_numero_sin, Movimenti_Contab.doc_numero_des,  ")
            stb.AppendLine("            MIN(Movimenti_Contab.doc_numero) as Numero_Min, MAX(Movimenti_Contab.doc_numero) aS Numero_Max ")
            stb.AppendLine("     ")

            stb.AppendLine("    FROM Agenda ")

            stb.AppendLine("    INNER JOIN Imprese ON Imprese.Piva = Agenda.Piva  ")

            stb.AppendLine("    INNER JOIN Movimenti Movimenti_Contab  ")
            stb.AppendLine("    ON Agenda.PIVA = Movimenti_Contab.PIVA AND Agenda.Id_Agenda = Movimenti_Contab.Id_Agenda ")

            'CONDIZIONI
            stb.AppendLine("    WHERE  Movimenti_Contab.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            stb.AppendLine("    AND    Movimenti_Contab.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            stb.AppendLine(" AND    Agenda.Lav_Cod IN ( " & LAVCOD_VENDITA & ", " &
                                                       LAVCOD_CORRISPETTIVO_VENDITA_SFUSO & ", " &
                                                       LAVCOD_RICEVUTA_EMESSA & ", " &
                                                       LAVCOD_DDT_CONTABILIZZATO_EMESSO &
                                                       "  ) ")

            stb.AppendLine("    AND    Movimenti_Contab.Cau_Mov = '" & CAU_REGISTRAZIONI & "'  ")

            stb.AppendLine("    AND    Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            stb.AppendLine("    AND    Agenda.Sa_Cod = 0 ")

            If Sezionale_Cod <> -1 Then
                stb.AppendLine(" AND Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            stb.AppendLine(" GROUP BY Agenda.PIVA, Imprese.Rag_Soc , Lav_Cod, Movimenti_Contab.Data_Movimento,  ")
            stb.AppendLine(" Movimenti_Contab.doc_numero_sin ,Movimenti_Contab.doc_numero_des ")

            stb.AppendLine(" ORDER BY Agenda.PIVA, Imprese.Rag_Soc, Movimenti_Contab.Data_Movimento,  ")
            stb.AppendLine(" Movimenti_Contab.doc_numero_sin ,Movimenti_Contab.doc_numero_des  ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' ritorna un dt con le note per ogni giorno dell'intervallo
    ''' (nelle note sono riepilogati ricevute fiscali dal... al... - scontrini dal... al... - ddt contabilizzati dal... al...)
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function RegistroCorrispettivi_RicavaNotaGiorno(ByVal Piva As String,
                                                           ByVal Sezionale_Cod As Integer,
                                                           ByVal Validita_Inizio As String,
                                                           ByVal Validita_Fine As String,
                                                           ByVal xFiltroAggiuntivo As String,
                                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                           ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreStampeDAL.RegistriContab.RegistroCorrispettivi_RicavaNotaGiorno"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable = Nothing
        Dim dtNote As DataTable = Nothing
        Dim i As Integer

        Try

            dt = RegistroCorrispettivi_NumeriDocumento(Piva, Sezionale_Cod, Validita_Inizio, Validita_Fine, xFiltroAggiuntivo, objParametri)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then

                Dim dr As DataRow
                Dim Temp_Data, Data As Date
                Temp_Data = AGRODATAINIZIO
                Dim Note As String = ""
                Dim Lav_Cod As Integer
                Dim numeroDal, numeroAl As String

                dtNote = New DataTable

                dtNote.Columns.Add(New DataColumn("Data", GetType(Date)))
                dtNote.Columns.Add(New DataColumn("Note", GetType(String)))

                For i = 0 To dt.Rows.Count - 1

                    With dt.Rows(i)

                        Data = .Item("Data_Movimento")
                        Lav_Cod = .Item("Lav_Cod")

                        If Temp_Data <> Data Then
                            If i <> 0 Then
                                dr = dtNote.NewRow
                                dr.Item("Data") = Temp_Data
                                dr.Item("Note") = Note
                                dtNote.Rows.Add(dr)
                            End If
                            'azzero le note
                            Note = ""
                            Temp_Data = Data
                        Else
                            'stessa data
                            'ci sono più numerazioni all'interno
                        End If

                        Select Case Lav_Cod
                            Case LAVCOD_VENDITA, LAVCOD_CORRISPETTIVO_VENDITA_SFUSO
                                Note &= "Scontrini "
                            Case LAVCOD_RICEVUTA_EMESSA
                                Note &= "Ric. Fiscali "
                            Case LAVCOD_DDT_CONTABILIZZATO_EMESSO
                                Note &= "DDT "
                        End Select

                        numeroDal = .Item("Doc_numero_Sin") & CStr(.Item("Numero_Min")) & .Item("Doc_numero_Des")
                        numeroAl = .Item("Doc_numero_Sin") & CStr(.Item("Numero_Max")) & .Item("Doc_numero_Des")

                        If numeroDal <> numeroAl Then
                            Note &= "dal " & numeroDal & " al " & numeroAl & " "
                        Else
                            Note &= "n. " & numeroDal & " "
                        End If

                    End With

                Next

                'inserisco l'ultima riga
                dr = dtNote.NewRow
                dr.Item("Data") = Data
                dr.Item("Note") = Note
                dtNote.Rows.Add(dr)

            End If


        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dtNote

    End Function


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     legge i pagamenti con riba e i relativi documenti con contatti
    ''' Tipo_Report = 0 RIBA Tipo_Report = 1 ANTICPO FATTURE
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function RegistroRIBA(ByVal Piva As String,
                                 ByVal Cod_RisUm As Integer,
                                 ByVal Cod_Rapporto As Integer,
                                 ByVal Lav_Cod As Integer,
                                 ByVal Cod_Istituto_DARE As Integer,
                                 ByVal Cod_Istituto_AVERE As Integer,
                                 ByVal Cod_Liquidita_DARE As Integer,
                                 ByVal Cod_Liquidita_AVERE As Integer,
                                 ByVal Data_Pagamento As Date,
                                 ByVal Tipo_Scadenza As Integer,
                                 ByVal Scadenza_inizio As Date,
                                 ByVal Scadenza_fine As Date,
                                 ByVal Tipo_NumeroDoc As Integer,
                                 ByVal NumeroDoc As Integer,
                                 ByVal AnnoDoc As Integer,
                                 ByVal Tipo_Report As Integer,
                                 ByVal CBI_Causale As CBI_Riba_Causali,
                                 ByVal xFiltroAggiuntivo As String,
                                 ByVal xOrderBy As String,
                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                 ) As DataTable


        'ByVal Previsto_Avvenuto As enum_Pagamento,
        ' ByVal Flag_PagamentoNonAvvenuto As Boolean,

        Dim nomeRoutine As String = "AgronicaCoreStampeDAL.RegistriContab.RegistroRIBA"

        Dim messaggioErrore As String = ""
        Dim stbQuery As New StringBuilder
        Dim dt As DataTable

        Try

            '") OR (Agenda.Lav_Cod = " & CStr(LAVCOD_RICEVUTA_EMESSA) &

            If xFiltroAggiuntivo <> "" Then
                xFiltroAggiuntivo &= " AND "
            End If

            xFiltroAggiuntivo &= " (Agenda.Lav_Cod IN ( " &
                                    CStr(LAVCOD_FATTURA_EMESSA) & ", " &
                                    CStr(LAVCOD_FATTURA_LIQ_CONF_EMESSA) & ", " &
                                    CStr(LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA) & ", " &
                                    CStr(LAVCOD_AUTOFATTURA_LIQ_CONF_EMESSA) &
                                    " ) ) "


            '/*************************************************************************************
            '/************ PAGAMENTI CON RIBA E RELATIVI DOCUMENTI *******************************
            '/*************************************************************************************

            stbQuery.Length = 0

            stbQuery.AppendLine(" SELECT coalesce(cc.val_cod, '') as MittenteSIA, 0 as MarketPlace, Imprese.Rag_Soc AS Impresa, ic.codice_fiscale as Codice_Fiscale_Creditore,  Agenda.PIVA, Agenda.Id_Agenda, Agenda.Lav_Cod, Agenda.des_lib,  ")

            'Movimenti
            stbQuery.AppendLine(" Movimenti.Id_Mov, Movimenti.Cod_RisUm, Movimenti.Cau_Mov, Movimenti.Mov_Desc,  ")
            stbQuery.AppendLine(" Movimenti.Data_Movimento ")
            '  Vanni, 24/02/2016 18:15:52: TFS1049 Sul file di flusso la data di scadenza viene calcolata male, ad esempio sul tipo 30 giorni da fine mese viene impostato il 30 giorni da data fattura. Leggere il dato dalla tabella Pagamenti (colonna DataScadenza_Manuale), allora leggere dalla tabella movimenti (colonna scadenza)
            stbQuery.AppendLine(" , DataScadenza_Manuale as Scadenza ")
            stbQuery.AppendLine(", Movimenti.Cod_IndirizzoRisUm  ")
            stbQuery.AppendLine(",Movimenti.Doc_Numero_Des, Movimenti.Doc_Numero, Movimenti.Doc_Numero_Sin,  ")
            stbQuery.AppendLine(" Movimenti.Num_Protocollo, ")


            'Pagamenti
            stbQuery.AppendLine("  Pagamenti.Cod_Liquidita_Dare, Pagamenti.Cod_Liquidita_Avere, Pagamenti.Cod_Pagamento, Pagamenti.Importo, Pagamenti.Percentuale, Pagamenti.Data_Pagamento, Pagamenti.Note, Pagamenti.Cau_Risorsa, Pagamenti.Cau_Pagamento, ")

            'Risorse_Umane
            stbQuery.AppendLine(" Risorse_Umane.Cod_RisUm, Risorse_Umane.Cod_Rapporto, ")
            'Contatti
            stbQuery.AppendLine(" cont.Piva AS Piva_Contatto, cont.Cod_Contatto, cont.Id_CF, case when substring(cont.cod_contatto, 1,1)  = '9' and cont.Id_CF = 0 and cont.rag_soc = '' then cont.Cognome + ' ' + cont.Nome  else   cont.Rag_Soc end AS Rag_Soc_Contatto, cont.Nome, cont.Cognome,  case when substring(cont.cod_contatto, 1,1)  = '9' and cont.Id_CF = 0  then cont.Cod_Contatto else  cont.Codice_Fiscale end as Codice_fiscale, ")
            stbQuery.AppendLine(" Indirizzi.ind_des, Indirizzi.frz_des, Indirizzi.CAP, ISNULL(ISTAT.LOCALITA, '') AS com_des, ISNULL(ISTAT.COMUNI_PROV, '') AS comuni_prov, Indirizzi.stato, --Indirizzi.note, Indirizzi.pro_cod_istat, Indirizzi.com_cod_istat ")

            'Liquidita_DARE 
            stbQuery.AppendLine("  ISNULL(Liquidita_DARE.Cod_Contatto, '') AS Cod_Contatto_DARE, ISNULL(Liquidita_DARE.Numero, '') AS Numero_DARE, ISNULL(Liquidita_DARE.Abi , '') AS Abi_DARE, ISNULL(Liquidita_DARE.Cab, '') AS Cab_DARE, ISNULL(Liquidita_DARE.Cin, '') AS Cin_DARE,   ")
            stbQuery.AppendLine("  ISNULL(Liquidita_DARE.Cifre_Controllo, '') AS Cifre_Controllo_DARE, ISNULL(Liquidita_DARE.Nazione, '') AS Nazione_DARE, ISNULL(Liquidita_DARE.Bic, '') AS Bic_DARE, ")
            stbQuery.AppendLine("  ISNULL(Liquidita_DARE.Interbancario, '') AS Interbancario_DARE, ISNULL(Liquidita_DARE.Saldo_Attuale, 0) AS Saldo_Attuale_DARE,  ")
            stbQuery.AppendLine("  ISNULL(Liquidita_DARE.Saldo_Iniziale, 0) AS Saldo_Iniziale_DARE, ISNULL(Liquidita_DARE.Cau_Risorsa, '') AS Cau_Risorsa_DARE, ")
            stbQuery.AppendLine("  ISNULL(Liquidita_DARE.Avviso, 0) AS Avviso_DARE, ISNULL(Liquidita_DARE.Importo_Avviso, 0) AS Importo_Avviso_DARE, ISNULL(Liquidita_DARE.Note, '') AS Note_DARE, ")

            'Ist_Credito_DARE
            stbQuery.AppendLine("  ISNULL(Liquidita_DARE.Cod_Istituto, 0) AS Cod_Istituto_DARE, ISNULL(Ist_Credito_DARE.Istituto_Des, '') AS Istituto_Des_DARE, ISNULL(Ist_Credito_DARE.Filiale, 0) AS Filiale_DARE,  ")

            'Liquidita_AVERE 
            stbQuery.AppendLine("  ISNULL(Liquidita_AVERE.Cod_Contatto, '') AS Cod_Contatto_AVERE, ISNULL(Liquidita_AVERE.Numero, '') AS Numero_AVERE, ISNULL(Liquidita_AVERE.Abi, '') AS Abi_AVERE, ISNULL(Liquidita_AVERE.Cab, '') AS Cab_AVERE, ISNULL(Liquidita_AVERE.Cin, '') AS Cin_AVERE, ")
            stbQuery.AppendLine("  ISNULL(Liquidita_AVERE.Cifre_Controllo, '') AS Cifre_Controllo_AVERE, ISNULL(Liquidita_AVERE.Nazione, '') AS Nazione_AVERE, ISNULL(Liquidita_AVERE.Bic, '') AS Bic_AVERE,   ")
            stbQuery.AppendLine("  ISNULL(Liquidita_AVERE.Interbancario, '') AS Interbancario_AVERE,  ISNULL(Liquidita_AVERE.Saldo_Attuale, 0) AS Saldo_Attuale_AVERE,  ")
            stbQuery.AppendLine("  ISNULL(Liquidita_AVERE.Saldo_Iniziale, 0) AS Saldo_Iniziale_AVERE,  ISNULL(Liquidita_AVERE.Cau_Risorsa, '') AS Cau_Risorsa_AVERE,   ")
            stbQuery.AppendLine("  ISNULL(Liquidita_AVERE.Avviso, 0) AS Avviso_AVERE, ISNULL(Liquidita_AVERE.Importo_Avviso, 0) AS Importo_Avviso_AVERE, ISNULL(Liquidita_AVERE.Note, '') AS Note_AVERE,  ")

            'Ist_Credito_AVERE
            stbQuery.AppendLine("  ISNULL(Liquidita_AVERE.Cod_Istituto, 0) AS Cod_Istituto_AVERE, ISNULL(Ist_Credito_AVERE.Istituto_Des, '') AS Istituto_Des_AVERE, ISNULL(Ist_Credito_AVERE.Filiale, 0) AS Filiale_AVERE, ")

            'PAGAMENTI CAUSALI
            stbQuery.AppendLine(" ISNULL(Pagamenti_Causali.Cau_Pagamento_Sigla, '') AS Cau_Pagamento_Sigla, ISNULL(Pagamenti_Causali.Cau_Pagamento_Des, '') AS Cau_Pagamento_Des,  ")
            stbQuery.AppendLine(" ISNULL(Pagamenti_Causali.Giorni_Scadenza, 0) AS Giorni_Scadenza, ISNULL(Pagamenti_Causali.Opzione, 0) AS Opzione,  ISNULL(Pagamenti_Causali.Cau_Risorsa, 0) AS Cau_Risorsa,   ")

            'CBI
            stbQuery.AppendLine("  cbic.CBI_Causali_DesBreve, ")
            stbQuery.AppendLine("  cbic.Colore, ")

            'CBI, nuovo stato
            stbQuery.AppendLine("  -1 as CBI_Causali_NuovoStato_Cod, ")
            stbQuery.AppendLine("  '---' as CBI_Causali_NuovoStato_DESBreve, ")
            stbQuery.AppendLine("  'black' as CBI_Causali_NuovoStato_Colore, ")
            stbQuery.AppendLine("   " & Agro_SQL_SaveDate(AGRODATAINIZIO) & " as CBI_Causali_NuovoStato_DataScadenza ")


            '**********************
            '****     FROM ********
            '**********************

            'PAGAMENTI
            stbQuery.AppendLine(" FROM Pagamenti ")

            'CBI
            stbQuery.AppendLine(" INNER JOIN CBI_Causali cbic on cbic.CBI_Causali_cod = Pagamenti.CBI_Causale ")

            'PAGAMENTI - PAGAMENTI CAUSALI
            stbQuery.AppendLine(" INNER JOIN  Pagamenti_Causali ON Pagamenti_Causali.Cau_Pagamento = Pagamenti.Cau_Pagamento ")

            'PAGAMENTI - LIQUIDITA DARE
            stbQuery.AppendLine(" LEFT OUTER JOIN  Liquidita Liquidita_DARE ON Liquidita_DARE.Piva = Pagamenti.Piva AND Liquidita_DARE.Sa_Cod = Pagamenti.Sa_Cod AND Liquidita_DARE.Cod_Liquidita = Pagamenti.Cod_Liquidita_Dare ")

            'LIQUIDITA DARE - IST CREDITO DARE
            'StbQuery.AppendLine(" LEFT OUTER JOIN Ist_Credito Ist_Credito_DARE ON Ist_Credito_DARE.Piva = Liquidita_DARE.Piva AND Ist_Credito_DARE.Sa_Cod = Liquidita_DARE.Sa_Cod AND Ist_Credito_DARE.Cod_Istituto = Liquidita_DARE.Cod_Istituto ")
            'CORREZIONE BUG DEL 07/01/2013: la piva di ist_credito è quella del superuser!
            stbQuery.AppendLine(" LEFT OUTER JOIN Ist_Credito Ist_Credito_DARE ON Ist_Credito_DARE.Cod_Istituto = Liquidita_DARE.Cod_Istituto ")
            stbQuery.AppendLine("                 AND Ist_Credito_DARE.Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            'PAGAMENTI - LIQUIDITA AVERE
            'correzione del 25/11/2015: messo left join altrimenti gli anticipi fatture non vengono letti
            'StbQuery.AppendLine(" INNER JOIN  Liquidita Liquidita_AVERE ON Liquidita_AVERE.Piva = Pagamenti.Piva AND Liquidita_AVERE.Sa_Cod = Pagamenti.Sa_Cod AND Liquidita_AVERE.Cod_Liquidita = Pagamenti.Cod_Liquidita_Avere ")
            stbQuery.AppendLine(" LEFT OUTER JOIN  Liquidita Liquidita_AVERE ON Liquidita_AVERE.Piva = Pagamenti.Piva AND Liquidita_AVERE.Sa_Cod = Pagamenti.Sa_Cod AND Liquidita_AVERE.Cod_Liquidita = Pagamenti.Cod_Liquidita_Avere ")

            'LIQUIDITA AVERE - IST CREDITO AVERE
            'StbQuery.AppendLine(" INNER JOIN Ist_Credito Ist_Credito_AVERE ON Ist_Credito_AVERE.Piva = Liquidita_AVERE.Piva AND Ist_Credito_AVERE.Sa_Cod = Liquidita_AVERE.Sa_Cod AND Ist_Credito_AVERE.Cod_Istituto = Liquidita_AVERE.Cod_Istituto ")
            'CORREZIONE BUG DEL 07/01/2013: la piva di ist_credito è quella del superuser!
            stbQuery.AppendLine(" LEFT OUTER JOIN Ist_Credito Ist_Credito_AVERE ON Ist_Credito_AVERE.Cod_Istituto = Liquidita_AVERE.Cod_Istituto ")
            stbQuery.AppendLine("                 AND Ist_Credito_AVERE.Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")


            ' PAGAMENTI - MOVIMENTI
            stbQuery.AppendLine(" INNER JOIN Movimenti ")
            stbQuery.AppendLine(" ON Pagamenti.PIVA = Movimenti.PIVA AND Pagamenti.Id_Agenda = Movimenti.Id_Agenda AND Pagamenti.Id_Mov = Movimenti.Id_Mov ")

            'MOVIMENTI - AGENDA
            stbQuery.AppendLine(" INNER JOIN  Agenda ")
            stbQuery.AppendLine(" ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda ")

            'CONTATTI
            'no join Risorse_Umane.Piva = Movimenti.PIVA !!!!!!!!!
            'altrimenti i contatti pubblici creati da un'altra azienda non vengono letti
            stbQuery.AppendLine(" INNER JOIN Risorse_Umane ON Risorse_Umane.Cod_RisUm = Movimenti.Cod_RisUm ")
            stbQuery.AppendLine(" INNER JOIN Contatti cont ON Risorse_Umane.Piva = cont.Piva AND Risorse_Umane.Cod_Contatto = cont.Cod_Contatto ")

            'CONTATTI - INDIRIZZI
            'StbQuery.AppendLine(" INNER JOIN ContattiXIndirizzi ON Cont.Cod_Contatto = ContattiXIndirizzi.Cod_Contatto AND ContattiXIndirizzi.Cod_Indirizzo = Movimenti.Cod_IndirizzoRisUm  ")
            stbQuery.AppendLine(" INNER JOIN (  ")
            stbQuery.AppendLine("               select Cod_Contatto, Cod_Indirizzo  ")
            stbQuery.AppendLine("               from ContattiXIndirizzi ")
            stbQuery.AppendLine("               union  ")
            stbQuery.AppendLine("               select PIVA as cod_contatto, cod_indirizzo  ")
            stbQuery.AppendLine("               from ImpresexIndirizzi  ")
            stbQuery.AppendLine("               ) ContattiXIndirizzi ON Cont.Cod_Contatto = ContattiXIndirizzi.Cod_Contatto AND ContattiXIndirizzi.Cod_Indirizzo = Movimenti.Cod_IndirizzoRisUm  ")

            stbQuery.AppendLine(" INNER JOIN Indirizzi ON ContattiXIndirizzi.Cod_Indirizzo = Indirizzi.cod_indirizzo ")
            stbQuery.AppendLine(" INNER JOIN ISTAT ON Indirizzi.pro_cod_istat = Istat.PROV AND Indirizzi.com_cod_istat = Istat.COM ")

            'JOIN IMPRESE - AGENDA
            stbQuery.AppendLine(" INNER JOIN Imprese ON Imprese.Piva = Agenda.Piva ")

            'JOIN CODICI Anagrafe x codice Fiscale            
            stbQuery.AppendLine(" INNER JOIN Contatti ic on ")
            stbQuery.AppendLine("   ic.PIVA = Imprese.PIVA ")
            stbQuery.AppendLine("   and ic.Cod_Contatto = imprese.piva ")

            stbQuery.AppendLine("left join Imprese_codici cc on ")
            stbQuery.AppendLine("       cc.PIVA = imprese.PIVA ")
            stbQuery.AppendLine("   and cc.id_cod = 1264 ")



            '**********************
            '**** WHERE ********
            '**********************
            'StbQuery.AppendLine(" WHERE   Movimenti.Validita_Inizio <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " ")
            'StbQuery.AppendLine(" AND     Movimenti.Validita_Fine >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")

            'StbQuery.AppendLine(" AND     Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " ")
            'StbQuery.AppendLine(" AND     Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")

            stbQuery.AppendLine(" WHERE ( Pagamenti_Causali.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' OR Pagamenti_Causali.Piva_SuperUser = 'AAAAAAAAAAA' ) ")

            'dopo l'introduzione del campo Tipo (migra 250)
            'modificata la condizione
            'StbQuery.AppendLine(" AND ( Pagamenti_Causali.Cau_Pagamento_Des LIKE '%Ricevuta%Bancaria%' ")
            'StbQuery.AppendLine("       OR Pagamenti_Causali.Cau_Pagamento_Des LIKE '%RI.BA%' ")
            'StbQuery.AppendLine("       OR Pagamenti_Causali.Cau_Pagamento_Des LIKE '%RI BA%' ")
            'StbQuery.AppendLine("       OR Pagamenti_Causali.Cau_Pagamento_Des LIKE '%RIBA%' ")
            'StbQuery.AppendLine("       ) ")
            'Public Enum enum_PagamentiCausali
            'NonImpostato = 0
            'RIBA = 1
            'BONIFICO = 2
            'CONTANTI = 3
            'RimessaDiretta = 4
            'End Enum
            Select Case Tipo_Report
                Case 0 'RIBA
                    stbQuery.AppendLine(" AND Pagamenti_Causali.Tipo = " & Agro_SQL_SaveNum(CStr(enum_PagamentiCausali.RiBa)) & "   ")
                Case 1 'ANTICIPO FATTURA
                    stbQuery.AppendLine(" AND Pagamenti_Causali.Tipo = " & Agro_SQL_SaveNum(CStr(enum_PagamentiCausali.Bonifico)) & "   ")
            End Select

            stbQuery.AppendLine(" AND Movimenti.Cau_Mov = '" & CAU_REGISTRAZIONI & "'   ")

            stbQuery.AppendLine(" AND Movimenti.Sa_Cod = 0 ")

            'verifico l'impostazione del pagamento previsto
            stbQuery.AppendLine(" AND Pagamenti.Previsto_Avvenuto = " & Agro_SQL_SaveNum(enum_Pagamento.Previsto) & "   ")

            If CBI_Causale <> CBI_Riba_Causali.Nessuno Then
                stbQuery.AppendLine(" AND Pagamenti.CBI_Causale = " & CBI_Causale)
            End If


            ''inoltre il movimento non deve avere il pagamento eseguito, con la stessa modalità (riba o bonifico a seconda dei casi)
            '' e per l'intero importo della fattura (num protocollo)
            'StbQuery.AppendLine("  AND NOT EXISTS ( ")
            'StbQuery.AppendLine("                    SELECT 1 ")
            'StbQuery.AppendLine("                    FROM Pagamenti Pag2 ")
            'StbQuery.AppendLine("                    INNER JOIN  Pagamenti_Causali  PC2 ON PC2.Cau_Pagamento = Pag2.Cau_Pagamento  ")
            'StbQuery.AppendLine("                     WHERE ( PC2.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' OR PC2.Piva_SuperUser = 'AAAAAAAAAAA' )")
            'StbQuery.AppendLine("                   AND Pagamenti.piva =Pag2.piva")
            'StbQuery.AppendLine("                    AND Pagamenti.Id_Agenda= Pag2.id_agenda ")
            'StbQuery.AppendLine("                    AND Pagamenti.id_mov= Pag2.id_mov ")
            'StbQuery.AppendLine("                    AND Pag2.previsto_avvenuto = 1 ")


            'Select Case Tipo_Report
            '    Case 0 'RIBA
            '        StbQuery.AppendLine("           AND PC2.Tipo = " & Agro_SQL_SaveNum(CStr(enum_PagamentiCausali.RiBa)) & "   ")
            '    Case 1 'ANTICIPO FATTURA
            '        StbQuery.AppendLine("           AND PC2.Tipo = " & Agro_SQL_SaveNum(CStr(enum_PagamentiCausali.Bonifico)) & "   ")
            'End Select
            'StbQuery.AppendLine("                    AND movimenti.num_protocollo = Pag2.importo ")
            'StbQuery.AppendLine("                   ) ")

            ''vecchia modalità
            'If Flag_PagamentoNonAvvenuto = True Then
            '    StbQuery.AppendLine(" AND    Pagamenti.Data_Pagamento = " & Agro_SQL_SaveDate(AGRODATAFINE) & " ")
            'Else
            '    StbQuery.AppendLine(" AND    Pagamenti.Data_Pagamento <> " & Agro_SQL_SaveDate(AGRODATAFINE) & " ")
            'End If

            If Data_Pagamento <> AGRODATAFINE Then
                stbQuery.AppendLine(" AND    Pagamenti.Data_Pagamento = " & Agro_SQL_SaveDate(Data_Pagamento) & " ")
            End If

            'Giulia - 08/02/2018: visto che quella che viene mostrata è Pagamenti.DataScadenza_Manuale, il filtro lo devo fare su quello, non su Movimenti.Scadenza
            '           (anche perchè su un documento con + tranches quella sarebbe la scadenza del documento, non della singola tranches di pagamento, quindi della singola riba)
            Select Case Tipo_Scadenza
                Case 0
                    'nessun filtro

                Case 1 'scadenza = certa data
                    stbQuery.AppendLine(" AND     Pagamenti.DataScadenza_Manuale = " & Agro_SQL_SaveDate(Scadenza_inizio) & " ")
                Case 2 'scadenza <= certa data
                    stbQuery.AppendLine(" AND     Pagamenti.DataScadenza_Manuale <= " & Agro_SQL_SaveDate(Scadenza_inizio) & " ")
                Case 3 'scadenza < certa data
                    stbQuery.AppendLine(" AND     Pagamenti.DataScadenza_Manuale < " & Agro_SQL_SaveDate(Scadenza_inizio) & " ")
                Case 4 'scadenza >= certa data
                    stbQuery.AppendLine(" AND     Pagamenti.DataScadenza_Manuale >= " & Agro_SQL_SaveDate(Scadenza_inizio) & " ")
                Case 5 'scadenza > certa data
                    stbQuery.AppendLine(" AND     Pagamenti.DataScadenza_Manuale > " & Agro_SQL_SaveDate(Scadenza_inizio) & " ")
                Case 6 'intervallo di date (solo per stampe 2010)

                    If Scadenza_inizio <> AGRODATAINIZIO Then
                        stbQuery.AppendLine(" AND     Pagamenti.DataScadenza_Manuale >= " & Agro_SQL_SaveDate(Scadenza_inizio) & " ")
                    End If
                    If Scadenza_fine <> AGRODATAFINE Then
                        stbQuery.AppendLine(" AND     Pagamenti.DataScadenza_Manuale <= " & Agro_SQL_SaveDate(Scadenza_fine) & " ")
                    End If

            End Select

            Select Case Tipo_NumeroDoc
                Case 0
                    'nessun filtro
                Case 1 'doc_numero = 
                    stbQuery.AppendLine(" AND     Movimenti.doc_numero = " & Agro_SQL_SaveNum(NumeroDoc) & " ")
                Case 2 'doc_numero <= 
                    stbQuery.AppendLine(" AND     Movimenti.doc_numero <= " & Agro_SQL_SaveNum(NumeroDoc) & " ")
                Case 3 'doc_numero < 
                    stbQuery.AppendLine(" AND     Movimenti.doc_numero < " & Agro_SQL_SaveNum(NumeroDoc) & " ")
                Case 4 'doc_numero >= 
                    stbQuery.AppendLine(" AND     Movimenti.doc_numero >= " & Agro_SQL_SaveNum(NumeroDoc) & " ")
                Case 5 'doc_numero > 
                    stbQuery.AppendLine(" AND     Movimenti.doc_numero > " & Agro_SQL_SaveNum(NumeroDoc) & " ")
            End Select

            If AnnoDoc <> 0 Then
                stbQuery.AppendLine(" AND   YEAR(Movimenti.data_movimento) = " & Agro_SQL_SaveNum(AnnoDoc) & " ")
            End If

            If Piva <> "" Then
                stbQuery.AppendLine(" AND Movimenti.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Cod_RisUm <> 0 Then
                stbQuery.AppendLine(" AND Movimenti.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
            End If

            If Cod_Rapporto <> 0 Then
                stbQuery.AppendLine(" AND Risorse_Umane.Cod_Rapporto = " & Agro_SQL_SaveNum(Cod_Rapporto) & "   ")
            End If

            If Lav_Cod <> 0 Then
                stbQuery.AppendLine(" AND Agenda.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & "   ")
            End If

            If Cod_Istituto_DARE <> 0 Then
                stbQuery.AppendLine(" AND Liquidita_DARE.Cod_Istituto = " & Agro_SQL_SaveNum(Cod_Istituto_DARE) & "   ")
            End If

            If Cod_Istituto_AVERE <> 0 Then
                stbQuery.AppendLine(" AND Liquidita_AVERE.Cod_Istituto = " & Agro_SQL_SaveNum(Cod_Istituto_AVERE) & "   ")
            End If

            If Cod_Liquidita_DARE <> 0 Then
                stbQuery.AppendLine(" AND Pagamenti.Cod_Liquidita_Dare = " & Agro_SQL_SaveNum(Cod_Liquidita_DARE) & "   ")
            End If

            If Cod_Liquidita_AVERE <> 0 Then
                stbQuery.AppendLine(" AND Pagamenti.Cod_Liquidita_Avere = " & Agro_SQL_SaveNum(Cod_Liquidita_AVERE) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stbQuery.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                stbQuery.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                stbQuery.AppendLine(" ORDER BY Movimenti.Scadenza, Istituto_Des_AVERE, Rag_Soc_Contatto, Data_Movimento ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbQuery.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function

    '########################################################################
    Public Function Lista_Insoluti_Movimenti(ByVal Flag_Debiti As Boolean,
                                             ByVal Piva As String,
                                             ByVal Cod_RisUm As Integer,
                                             ByVal Cod_Rapporto As Integer,
                                             ByVal Piva_Contatto As String,
                                             ByVal Cod_Contatto As String,
                                             ByVal cod_risum_agente As Integer,
                                             ByVal Sezionale_Cod As Integer,
                                             ByVal Validita_Inizio As String,
                                             ByVal Validita_Fine As String,
                                             ByVal Tipo_Scadenza As Integer,
                                             ByVal Scadenza As String,
                                             ByVal Flag_SoloScadute As Boolean,
                                             ByVal Lista_tipiPag As String,
                                             ByVal FiltroAggiuntivo1 As String,
                                             ByVal FiltroAggiuntivo2 As String,
                                             ByVal FiltroAggiuntivo3 As String,
                                             ByVal Ordinamento As enum_ReportInsoluti_Ordinamento,
                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                             Optional ByVal flagNuoviArrotondamenti As Boolean = False
                                             ) As DataTable

        Const nomeRoutine = "AgronicaCoreStampeDAL.RegistriContab.Lista_Insoluti_Movimenti"
        Dim messaggioErrore As String = ""

        Dim Stb_Globale As New StringBuilder
        Dim StbSelect As New StringBuilder
        Dim StbSelect12 As New StringBuilder
        Dim StbSelect3 As New StringBuilder
        Dim StbSelectSIRisum As New StringBuilder
        Dim StbSelectNORisum As New StringBuilder
        Dim StbSelectEnasarco As New StringBuilder
        Dim StbSelectNoEnasarco As New StringBuilder
        Dim StbWhere As New StringBuilder
        Dim StbWhere12 As New StringBuilder
        Dim StbWhere3 As New StringBuilder
        Dim StbWhereAgenti As New StringBuilder
        Dim StbWhereSez As New StringBuilder
        Dim StbJoin As New StringBuilder
        Dim StbJoinSIRisum As New StringBuilder
        Dim StbJoin3 As New StringBuilder
        Dim StbJoinAgenti As New StringBuilder

        Dim dt As DataTable
        Dim FiltroLavCod_Debiti_1 As String = ""
        Dim FiltroLavCod_Debiti_2 As String = ""
        Dim FiltroLavCod_Crediti_1 As String = ""
        Dim FiltroLavCod_Crediti_2 As String = ""

        Try

            '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            'ATTENZIONE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            'se si cambiano/aggiungono lav_cod
            'gestirli anche nella pagina Insoluti.aspx

            If Flag_Debiti = False Then
                'la fattura proforma non è valida ai fini fiscali
                '") OR (Agenda.Lav_Cod = " & CStr(LAVCOD_FATTURA_PROFORMA) & 
                FiltroLavCod_Crediti_1 = " AND (Agenda.Lav_Cod IN ( " &
                                       CStr(LAVCOD_FATTURA_EMESSA) & ", " &
                                       CStr(LAVCOD_FATTURA_LIQ_CONF_EMESSA) & ", " &
                                       CStr(LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA) & ", " &
                                       CStr(LAVCOD_NOTA_ACCREDITO_EMESSA) & ", " &
                                       CStr(LAVCOD_RICEVUTA_EMESSA) & ", " &
                                       CStr(LAVCOD_DDT_CONTABILIZZATO_EMESSO) & " " &
                                       " ) ) "

                FiltroLavCod_Crediti_2 = " AND (Agenda.Lav_Cod IN ( " &
                                     CStr(LAVCOD_VENDITA) & ", " &
                                     CStr(LAVCOD_CORRISPETTIVO_VENDITA_SFUSO) & ", " &
                                     CStr(LAVCOD_ALTRI_RICAVI) & " " &
                                     " ) ) "

            Else
                FiltroLavCod_Debiti_1 = " AND (Agenda.Lav_Cod IN ( " &
                                                 CStr(LAVCOD_FATTURA_RICEVUTA) & ", " &
                                                 CStr(LAVCOD_FATTURA_PROFESSIONISTI) & ", " &
                                                 CStr(LAVCOD_NOTA_ACCREDITO_RICEVUTA) & " " &
                                                 " ) ) "

                FiltroLavCod_Debiti_2 = " AND (Agenda.Lav_Cod IN ( " &
                                                CStr(LAVCOD_ACQUISTO) & ", " &
                                                CStr(LAVCOD_ALTRI_COSTI) &
                                                " ) ) "
            End If

            '------------------------------------------------------------------
            '----------- SELECT ---------------------------
            '------------------------------------------------------------------

            StbSelect.AppendLine(" SELECT Imprese.Rag_Soc AS Impresa,  Agenda.PIVA, Agenda.Sa_Cod, Agenda.Id_Agenda, Agenda.Lav_Cod, Agenda.des_lib,  ")
            StbSelect.AppendLine(" Movimenti.Id_Mov, Movimenti.Cod_RisUm, Movimenti.Cau_Mov, Movimenti.Mov_Desc, Movimenti.Data_Movimento, Movimenti.Scadenza,  ")
            StbSelect.AppendLine("  Movimenti.Doc_Numero, Movimenti.Cod_IndirizzoRisUm, Movimenti.Cod_Destinazione, Movimenti.Cod_IndirizzoDestinazione, Movimenti.Mezzo,  ")
            StbSelect.AppendLine("  Movimenti.Cod_Vettore, Movimenti.Cod_IndirizzoVettore, Movimenti.Causale_Trasporto, Movimenti.Aspetto, Movimenti.Peso, Movimenti.Tipo_Sconto,  ")
            StbSelect.AppendLine("  Movimenti.Doc_Numero_Des, Movimenti.Natura_Beni, Movimenti.Doc_Numero_Sin, Movimenti.Scadenza_Extra, Movimenti.Extra_Str, Movimenti.Extra_Date, ")
            StbSelect.AppendLine("  ")

            'Risorse_Umane
            StbSelectSIRisum.AppendLine(" ISNULL(Risorse_Umane.Cod_RisUm, 0) AS Cod_RisUm, ISNULL(Risorse_Umane.Cod_Rapporto, 0) AS Cod_Rapporto, ISNULL(Risorse_Umane.Validita_Inizio, '01/01/1900') AS Validita_Inizio, ISNULL(Risorse_Umane.Validita_Fine, '31/12/2100') AS Validita_Fine,  ")
            StbSelectSIRisum.AppendLine(" ISNULL(Risorse_Umane.Settore_Des,'') AS Settore_Des, ISNULL(Risorse_Umane.Attivita_Des,'') AS Attivita_Des,  ")
            'Contatti
            StbSelectSIRisum.AppendLine(" ISNULL(Contatti.Piva, '') AS Piva_Contatto, ISNULL(Contatti.Sa_Cod, 0) AS Sa_Cod_Contatto, ISNULL(Contatti.Cod_Contatto,'') AS Cod_Contatto,  ")
            StbSelectSIRisum.AppendLine(" ISNULL(Contatti.Id_CF, 0) AS Id_CF, ISNULL(Contatti.Rag_Soc + Contatti.nOME + ' ' + Contatti.Cognome,'') AS Rag_Soc_Contatto,  ISNULL(Contatti.Codice_Fiscale,'') AS Codice_Fiscale, ")

            'Risorse_Umane
            StbSelectNORisum.AppendLine(" 0 AS Cod_RisUm, 0 AS Cod_Rapporto, '01/01/1900' AS Validita_Inizio, '31/12/2100' AS Validita_Fine,  ")
            StbSelectNORisum.AppendLine(" '' AS Settore_Des, '' AS Attivita_Des,  ")
            'Contatti
            StbSelectNORisum.AppendLine(" '' AS Piva_Contatto, 0 AS Sa_Cod_Contatto, '' AS Cod_Contatto,  ")
            StbSelectNORisum.AppendLine(" 0 AS Id_CF, '' AS Rag_Soc_Contatto, '' AS Codice_Fiscale, ")


            StbSelect12.AppendLine(" Movimenti.Num_Protocollo, ")
            StbSelect12.AppendLine(" ISNULL ((SELECT     SUM(  ROUND(imponibile_Netto,2)   ) ")
            StbSelect12.AppendLine("             FROM         movimenti_dettagli ")
            StbSelect12.AppendLine("             WHERE     movimenti_dettagli.piva = agenda.piva  ")
            StbSelect12.AppendLine("             AND movimenti_dettagli.id_agenda = agenda.id_agenda), 0) AS Imponibile_Netto_Doc, ")
            StbSelect12.AppendLine(" ISNULL ((SELECT     SUM(  ROUND(imponibile,2)   ) ")
            StbSelect12.AppendLine("             FROM         movimenti_dettagli ")
            StbSelect12.AppendLine("             WHERE     movimenti_dettagli.piva = agenda.piva  ")
            StbSelect12.AppendLine("             AND movimenti_dettagli.id_agenda = agenda.id_agenda), 0) AS Imponibile_Doc, ")
            StbSelect12.AppendLine(" ISNULL ((SELECT     SUM(  ROUND(iva,2)   ) ")
            StbSelect12.AppendLine("             FROM         movimenti_dettagli ")
            StbSelect12.AppendLine("             WHERE     movimenti_dettagli.piva = agenda.piva  ")
            StbSelect12.AppendLine("             AND movimenti_dettagli.id_agenda = agenda.id_agenda), 0) AS Iva_Doc ")
            StbSelect12.AppendLine("  ")

            'TODO: ARROTONDAMENTO???
            StbSelect3.AppendLine(" ISNULL (Movimenti_Dettagli.Prezzo_Unitario_Netto, 0) AS Num_Protocollo, ")
            StbSelect3.AppendLine(" 0 AS Imponibile_Netto_Doc, ")
            StbSelect3.AppendLine(" ISNULL ((SELECT     ABS(SUM(imponibile)) ")
            StbSelect3.AppendLine("             FROM         movimenti_dettagli ")
            StbSelect3.AppendLine("             WHERE     movimenti_dettagli.piva = agenda.piva  ")
            StbSelect3.AppendLine("             AND movimenti_dettagli.id_agenda = agenda.id_agenda), 0) AS Imponibile_Doc, ")
            StbSelect3.AppendLine(" ISNULL (Movimenti_Dettagli.Iva, 0) AS Iva_Doc ")
            StbSelect3.AppendLine("  ")

            'TODO: ARROTONDAMENTO???
            StbSelectEnasarco.AppendLine(" , ISNULL ( (SELECT SUM(enasarco + ritenuta_acconto) ")
            StbSelectEnasarco.AppendLine("              FROM Mov_Dettaglio_Tecnico_Extra  ")
            StbSelectEnasarco.AppendLine("              WHERE Mov_Dettaglio_Tecnico_Extra.piva = Movimenti.piva ")
            StbSelectEnasarco.AppendLine("              AND Mov_Dettaglio_Tecnico_Extra.id_agenda = Movimenti.id_agenda ")
            StbSelectEnasarco.AppendLine("              AND Mov_Dettaglio_Tecnico_Extra.id_mov = Movimenti.id_mov ")
            StbSelectEnasarco.AppendLine("              ) , 0) AS Tot_Ritenute_Enasarco  ")
            StbSelectEnasarco.AppendLine("  ")

            StbSelectNoEnasarco.AppendLine(" , 0 AS Tot_Ritenute_Enasarco ")

            '------------------------------------------------------------------
            '----------- JOIN ---------------------------
            '------------------------------------------------------------------

            'AGENDA - MOVIMENTI
            StbJoin.AppendLine(" FROM    Agenda  ")
            StbJoin.AppendLine(" INNER JOIN Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda ")

            'JOIN IMPRESE - AGENDA
            StbJoin.AppendLine(" INNER JOIN Imprese ON Imprese.Piva = Agenda.Piva ")

            'CONTATTI
            'no join Risorse_Umane.Piva = Movimenti.PIVA !!!!!!!!!
            'altrimenti i contatti pubblici creati da un'altra azienda non vengono letti
            StbJoinSIRisum.AppendLine(" INNER JOIN Risorse_Umane ON Risorse_Umane.Cod_RisUm = Movimenti.Cod_RisUm ")
            StbJoinSIRisum.AppendLine(" INNER JOIN Contatti ON Risorse_Umane.Piva = Contatti.Piva AND Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto ")

            'JOIN AGENDA - MOVIMENTI COMPENSI
            StbJoin3.AppendLine(" INNER JOIN Movimenti Movimenti_Comp ")
            StbJoin3.AppendLine(" ON Agenda.PIVA = Movimenti_Comp.PIVA AND Agenda.Sa_Cod = Movimenti_Comp.Sa_Cod AND Agenda.Id_Agenda = Movimenti_Comp.Id_Agenda")

            ''JOIN MOVIMENTI COMPENSI - PAGAMENTI
            'StbJoin3.AppendLine(" INNER JOIN Pagamenti  ")
            'StbJoin3.AppendLine(" ON Pagamenti.PIVA = Movimenti_Comp.PIVA AND Pagamenti.Sa_Cod = Movimenti_Comp.Sa_Cod AND Pagamenti.Id_Agenda = Movimenti_Comp.Id_Agenda AND AND Pagamenti.Id_Mov = Movimenti_Comp.Id_Mov")

            'JOIN MOVIMENTI COMPENSI - MOVIMENTI DETTAGLI
            StbJoin3.AppendLine(" INNER JOIN Movimenti_dettagli ")
            StbJoin3.AppendLine(" ON Movimenti_dettagli.PIVA = Movimenti_Comp.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti_Comp.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti_Comp.Id_Mov ")

            'JOIN MOVIMENTI DETTAGLI - RISORSE UMANE CORRISPETTIVI
            'no join Risorse_Umane.Piva = Movimenti_dettagli.PIVA !!!!!!!!!
            'altrimenti i contatti pubblici creati da un'altra azienda non vengono letti
            StbJoin3.AppendLine(" INNER JOIN Risorse_Umane  ON Risorse_Umane.Cod_RisUm = Movimenti_dettagli.Mat_Cod ")

            'JOIN RISORSE UMANE - CONTATTI 
            StbJoin3.AppendLine(" INNER JOIN Contatti  ON Risorse_Umane.Piva = Contatti.Piva AND Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto")

            StbJoinAgenti.AppendLine(" INNER JOIN Mov_Dettaglio_Tecnico_Extra Movimento_Extra ON Movimento_Extra.PIVA = Movimenti.PIVA AND Movimento_Extra.Sa_Cod = Movimenti.Sa_Cod AND Movimento_Extra.Id_Agenda = Movimenti.Id_Agenda AND Movimento_Extra.Id_Mov = Movimenti.Id_Mov ")


            '------------------------------------------------------------------
            '----------- WHERE ---------------------------
            '------------------------------------------------------------------

            StbWhere.AppendLine(" WHERE   Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StbWhere.AppendLine(" AND     Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            StbWhere.AppendLine(" AND Movimenti.Cau_Mov = '" & CAU_REGISTRAZIONI & "' ")

            StbWhereAgenti.AppendLine(" AND Movimento_Extra.Agente_Cod = " & Agro_SQL_SaveNum(cod_risum_agente) & " ")

            'la verifica di sezionale_cod <> -1 viene fatta dopo
            StbWhereSez.AppendLine(" AND Movimenti.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")


            'MODIFICA IN DATA 05/06/2012: RICHIESTA DELLA MOLINELLI
            'filtrati i documenti con importo 0 (contengono omaggi) e non sono da visualizzare
            StbWhere12.AppendLine(" AND Movimenti.Num_Protocollo <> 0 ")
            StbWhere3.AppendLine(" AND Movimenti_Dettagli.Prezzo_Unitario_Netto <> 0 -- num_protocollo")

            If Flag_SoloScadute = True Then
                StbWhere.AppendLine(" AND Movimenti.Scadenza < " & Agro_SQL_SaveDate(CDate(Now)) & " ")
            End If

            Select Case Tipo_Scadenza
                Case 0
                    'nessun filtro
                Case 1 'scadenza = certa data
                    StbWhere.AppendLine(" AND     Movimenti.Scadenza = " & Agro_SQL_SaveDate(Scadenza) & " ")
                Case 2 'scadenza <= certa data
                    StbWhere.AppendLine(" AND     Movimenti.Scadenza <= " & Agro_SQL_SaveDate(Scadenza) & " ")
                Case 3 'scadenza < certa data
                    StbWhere.AppendLine(" AND     Movimenti.Scadenza < " & Agro_SQL_SaveDate(Scadenza) & " ")
                Case 4 'scadenza >= certa data
                    StbWhere.AppendLine(" AND     Movimenti.Scadenza >= " & Agro_SQL_SaveDate(Scadenza) & " ")
                Case 5 'scadenza > certa data
                    StbWhere.AppendLine(" AND     Movimenti.Scadenza > " & Agro_SQL_SaveDate(Scadenza) & " ")
            End Select

            If Lista_tipiPag <> "" Then
                StbWhere.AppendLine(" AND EXISTS ( ")
                StbWhere.AppendLine("           SELECT 1 ")
                StbWhere.AppendLine("           FROM Pagamenti ")
                StbWhere.AppendLine("           INNER JOIN pagamenti_causali on Pagamenti.Cau_Pagamento=pagamenti_causali.Cau_Pagamento ")
                StbWhere.AppendLine("           WHERE Agenda.PIVA = Pagamenti.piva ")
                StbWhere.AppendLine("           AND Agenda.id_agenda = Pagamenti.id_agenda ")
                StbWhere.AppendLine("           AND pagamenti_causali.tipo IN " & Agro_SQL_Save_Clausola_IN(Lista_tipiPag, False))
                StbWhere.AppendLine("           ) ")
            End If

            If Piva <> "" Then
                StbWhere.AppendLine(" AND Movimenti.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            StbWhere.AppendLine(" AND Agenda.Sa_Cod = 0 ")

            If Cod_RisUm <> 0 Then
                StbWhere.AppendLine(" AND Movimenti.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
            End If

            If Cod_Rapporto <> 0 Then
                StbWhere.AppendLine(" AND Risorse_Umane.Cod_Rapporto = " & Agro_SQL_SaveNum(Cod_Rapporto) & "   ")
            End If

            If Piva_Contatto <> "" Then
                StbWhere.AppendLine(" AND Contatti.Piva = '" & Agro_SQL_SaveText(Piva_Contatto) & "'   ")
            End If

            If Cod_Contatto <> "" Then
                StbWhere.AppendLine(" AND Contatti.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "'   ")
            End If

            If FiltroAggiuntivo1 <> "" Then
                StbWhere.AppendLine(Agro_SQL_Save_xFiltroAggiuntivo(FiltroAggiuntivo1, , objParametri))
            End If


            '/************************************************************************************
            '/***** 1° PARTE: DOC CONTABILI  ***************
            '/************************************************************************************

            Stb_Globale.AppendLine(" ( ")

            Stb_Globale.AppendLine(StbSelect.ToString)
            Stb_Globale.AppendLine(StbSelectSIRisum.ToString)
            Stb_Globale.AppendLine(StbSelect12.ToString)
            Stb_Globale.AppendLine(StbSelectEnasarco.ToString)
            Stb_Globale.AppendLine(StbJoin.ToString)
            Stb_Globale.AppendLine(StbJoinSIRisum.ToString)
            If cod_risum_agente <> 0 Then
                Stb_Globale.AppendLine(StbJoinAgenti.ToString)
            End If
            Stb_Globale.AppendLine(StbWhere.ToString)
            Stb_Globale.AppendLine(StbWhere12.ToString)
            If cod_risum_agente <> 0 Then
                Stb_Globale.AppendLine(StbWhereAgenti.ToString)
            End If
            If Sezionale_Cod <> -1 Then
                Stb_Globale.AppendLine(StbWhereSez.ToString)
            End If
            Stb_Globale.AppendLine(FiltroLavCod_Crediti_1)
            Stb_Globale.AppendLine(FiltroLavCod_Debiti_1)
            If FiltroAggiuntivo1 <> "" Then
                Stb_Globale.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(FiltroAggiuntivo1, , objParametri))
            End If

            Stb_Globale.AppendLine(" ) ")
            Stb_Globale.AppendLine(" ")
            Stb_Globale.AppendLine(" ")

            'se è impostato il filtro agente, inutile leggere movimenti sui quali non è previsto l'agente
            If cod_risum_agente = 0 Then


                '/////////////////
                Stb_Globale.AppendLine(" UNION ALL ")
                '/////////////////

                '/************************************************************************************
                '/***** 2° PARTE: CORRISPETTIVI ACQU/VENDITA + ALTRI COSTI/RICAVI  ***************
                'i corrispettivi di acquisto non hanno la gestione pagamenti
                '/************************************************************************************

                Stb_Globale.AppendLine(" ( ")

                Stb_Globale.AppendLine(StbSelect.ToString)
                Stb_Globale.AppendLine(StbSelectNORisum.ToString)
                Stb_Globale.AppendLine(StbSelect12.ToString)
                Stb_Globale.AppendLine(StbSelectNoEnasarco.ToString)
                Stb_Globale.AppendLine(StbJoin.ToString)
                If Cod_Rapporto <> 0 OrElse Piva_Contatto <> "" OrElse Cod_Contatto <> "" Then
                    Stb_Globale.AppendLine(StbJoinSIRisum.ToString)
                End If
                Stb_Globale.AppendLine(StbWhere.ToString)
                Stb_Globale.AppendLine(StbWhere12.ToString)
                If Sezionale_Cod <> -1 Then
                    Stb_Globale.AppendLine(StbWhereSez.ToString)
                End If
                Stb_Globale.AppendLine(FiltroLavCod_Crediti_2)
                Stb_Globale.AppendLine(FiltroLavCod_Debiti_2)
                If FiltroAggiuntivo2 <> "" Then
                    Stb_Globale.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(FiltroAggiuntivo2, , objParametri))
                End If

                Stb_Globale.AppendLine(" ) ")
                Stb_Globale.AppendLine(" ")
                Stb_Globale.AppendLine(" ")
                Stb_Globale.AppendLine(" ")

                If Flag_Debiti = True Then

                    '/////////////////
                    Stb_Globale.AppendLine(" UNION ALL ")
                    '/////////////////

                    '/************************************************************************************
                    '/***** 3° PARTE: COMPENSI  ***************
                    '/************************************************************************************

                    Stb_Globale.AppendLine(" ( ")

                    Stb_Globale.AppendLine(StbSelect.ToString)
                    Stb_Globale.AppendLine(StbSelectSIRisum.ToString)
                    Stb_Globale.AppendLine(StbSelect3.ToString)
                    Stb_Globale.AppendLine(StbSelectNoEnasarco.ToString)
                    Stb_Globale.AppendLine(StbJoin.ToString)
                    Stb_Globale.AppendLine(StbJoin3.ToString)
                    Stb_Globale.AppendLine(StbWhere.ToString)
                    If Sezionale_Cod <> -1 Then
                        Stb_Globale.AppendLine(StbWhereSez.ToString)
                    End If
                    Stb_Globale.AppendLine(" AND Agenda.lav_cod = " & LAVCOD_REG_COMPENSI & "  ")
                    Stb_Globale.AppendLine(" AND Movimenti_Comp.Cau_Mov = '" & CAU_COMPENSI & "'   ")
                    Stb_Globale.AppendLine(StbWhere3.ToString)

                    If FiltroAggiuntivo3 <> "" Then
                        Stb_Globale.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(FiltroAggiuntivo3, , objParametri))
                    End If

                    Stb_Globale.AppendLine(" ) ")
                    Stb_Globale.AppendLine(" ")
                    Stb_Globale.AppendLine(" ")
                    Stb_Globale.AppendLine(" ")

                End If

            End If 'filtro x agente

            '  Stb_Globale.AppendLine(" ORDER BY Movimenti.Scadenza, Rag_Soc_Contatto, Num_Protocollo ")
            Select Case Ordinamento
                Case enum_ReportInsoluti_Ordinamento.Scadenza
                    Stb_Globale.AppendLine(" ORDER BY Movimenti.Scadenza, Rag_Soc_Contatto, Data_Movimento, Agenda.Lav_Cod, Doc_Numero ")

                Case enum_ReportInsoluti_Ordinamento.ClienteData
                    Stb_Globale.AppendLine(" ORDER BY Rag_Soc_Contatto, Data_Movimento, Agenda.Lav_Cod, Doc_Numero  ")

                Case enum_ReportInsoluti_Ordinamento.Data
                    Stb_Globale.AppendLine(" ORDER BY Data_Movimento, Agenda.Lav_Cod, Doc_Numero  ")

                Case enum_ReportInsoluti_Ordinamento.Numero
                    Stb_Globale.AppendLine(" ORDER BY Agenda.Lav_Cod, Doc_Numero  ")

                Case enum_ReportInsoluti_Ordinamento.ClienteScadenza
                    Stb_Globale.AppendLine(" ORDER BY Rag_Soc_Contatto, scadenza, Data_Movimento, Agenda.Lav_Cod, Doc_Numero  ")

                Case Else
                    Stb_Globale.AppendLine(" ORDER BY Movimenti.Scadenza, Rag_Soc_Contatto, Num_Protocollo ")
            End Select

            '#############################################################

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, Stb_Globale.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '####################################################################
    Public Function Lista_Insoluti_Pagamenti(ByVal Piva As String,
                                             ByVal Sa_Cod As Integer,
                                             ByVal Id_Agenda As Integer,
                                             ByVal Id_Mov As Integer,
                                             ByVal Cod_Pagamento As Integer,
                                             ByVal Cod_Liquidita As Integer,
                                             ByVal Cau_Pagamento As Integer,
                                             ByVal Flag_Insolvenze As Boolean,
                                             ByVal Flag_Riscossioni As Boolean,
                                             ByVal Previsto_Avvenuto As enum_Pagamento,
                                             ByVal FiltroAggiuntivo As String,
                                             ByVal DT_Codifiche_Pat As DataTable,
                                             ByVal xOrderBy As String,
                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                             ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreStampeDAL.RegistriContab.Lista_Insoluti_Pagamenti"

        Dim messaggioErrore As String = ""
        Dim stbQuery As New StringBuilder
        Dim dt As DataTable

        Try

            '-------------------------------------------------------
            'pezzo che serve per scremare dai pagamenti i record dell'enasarco e ritenuta d'acconto
            'che servono solo nella partita doppia
            '-------------------------------------------------------
            Dim objContabHlp As New AgronicaCoreContabHLP.Contabilita
            Dim CodContoPat_DebitiVsEnasarco As Integer
            Dim CodContoPat_ErarioRitenuteLavoroAutonomo As Integer

            objContabHlp.Recupera_CodContoPat_EnasarcoRitAcconto(DT_Codifiche_Pat,
                                                                 CodContoPat_DebitiVsEnasarco,
                                                                 CodContoPat_ErarioRitenuteLavoroAutonomo)

            If CodContoPat_DebitiVsEnasarco <> 0 AndAlso CodContoPat_ErarioRitenuteLavoroAutonomo <> 0 Then
                If FiltroAggiuntivo <> "" Then
                    FiltroAggiuntivo &= " AND Pagamenti.Cod_Conto_Pat_Avere NOT IN (" & Agro_SQL_SaveNum(CodContoPat_DebitiVsEnasarco) & ", " & Agro_SQL_SaveNum(CodContoPat_ErarioRitenuteLavoroAutonomo) & " )"
                End If
            End If
            '-------------------------------------------------------


            stbQuery.Length = 0

            stbQuery.AppendLine(" ( ")

            'Pagamenti
            stbQuery.AppendLine(" SELECT Pagamenti.id_agenda, Pagamenti.id_mov, pagamenti.Previsto_Avvenuto, Pagamenti.Cod_Liquidita_Dare, Pagamenti.Cod_Liquidita_Avere, Pagamenti.Cod_Pagamento, Pagamenti.Importo, Pagamenti.Percentuale, Pagamenti.Data_Pagamento, Pagamenti.Note, Pagamenti.Cau_Risorsa, Pagamenti.Cau_Pagamento, ")

            'Liquidita_DARE + Ist_Credito_DARE
            stbQuery.AppendLine("  ISNULL(Liquidita_DARE.Cod_Contatto, '') AS Cod_Contatto_DARE, ISNULL(Liquidita_DARE.Numero, '') AS Numero_DARE, ISNULL(Liquidita_DARE.Abi , '') AS Abi_DARE, ISNULL(Liquidita_DARE.Cab, '') AS Cab_DARE, ISNULL(Liquidita_DARE.Cin, '') AS Cin_DARE,   ")
            stbQuery.AppendLine("  ISNULL(Liquidita_DARE.Cifre_Controllo, '') AS Cifre_Controllo_DARE, ISNULL(Liquidita_DARE.Nazione, '') AS Nazione_DARE, ISNULL(Liquidita_DARE.Bic, '') AS Bic_DARE, ")
            stbQuery.AppendLine("  ISNULL(Liquidita_DARE.Interbancario, '') AS Interbancario_DARE, ISNULL(Liquidita_DARE.Saldo_Attuale, 0) AS Saldo_Attuale_DARE,  ")
            stbQuery.AppendLine("  ISNULL(Liquidita_DARE.Saldo_Iniziale, 0) AS Saldo_Iniziale_DARE, ISNULL(Liquidita_DARE.Cau_Risorsa, '') AS Cau_Risorsa_DARE, ")
            stbQuery.AppendLine("  ISNULL(Liquidita_DARE.Avviso, 0) AS Avviso_DARE, ISNULL(Liquidita_DARE.Importo_Avviso, 0) AS Importo_Avviso_DARE, ISNULL(Liquidita_DARE.Note, '') AS Note_DARE, ")
            stbQuery.AppendLine("  ISNULL(Liquidita_DARE.Cod_Istituto, 0) AS Cod_Istituto_DARE, ISNULL(Ist_Credito_DARE.Istituto_Des, '') AS Istituto_Des_DARE, ISNULL(Ist_Credito_DARE.Filiale, 0) AS Filiale_DARE,  ")
            'Liquidita_AVERE + Ist_Credito_AVERE
            stbQuery.AppendLine("  ISNULL(Liquidita_AVERE.Cod_Contatto, '') AS Cod_Contatto_AVERE, ISNULL(Liquidita_AVERE.Numero, '') AS Numero_AVERE, ISNULL(Liquidita_AVERE.Abi, '') AS Abi_AVERE, ISNULL(Liquidita_AVERE.Cab, '') AS Cab_AVERE, ISNULL(Liquidita_AVERE.Cin, '') AS Cin_AVERE, ")
            stbQuery.AppendLine("  ISNULL(Liquidita_AVERE.Cifre_Controllo, '') AS Cifre_Controllo_AVERE, ISNULL(Liquidita_AVERE.Nazione, '') AS Nazione_AVERE, ISNULL(Liquidita_AVERE.Bic, '') AS Bic_AVERE,   ")
            stbQuery.AppendLine("  ISNULL(Liquidita_AVERE.Interbancario, '') AS Interbancario_AVERE,  ISNULL(Liquidita_AVERE.Saldo_Attuale, 0) AS Saldo_Attuale_AVERE,  ")
            stbQuery.AppendLine("  ISNULL(Liquidita_AVERE.Saldo_Iniziale, 0) AS Saldo_Iniziale_AVERE,  ISNULL(Liquidita_AVERE.Cau_Risorsa, '') AS Cau_Risorsa_AVERE,   ")
            stbQuery.AppendLine("  ISNULL(Liquidita_AVERE.Avviso, 0) AS Avviso_AVERE, ISNULL(Liquidita_AVERE.Importo_Avviso, 0) AS Importo_Avviso_AVERE, ISNULL(Liquidita_AVERE.Note, '') AS Note_AVERE,  ")
            stbQuery.AppendLine("  ISNULL(Liquidita_AVERE.Cod_Istituto, 0) AS Cod_Istituto_AVERE, ISNULL(Ist_Credito_AVERE.Istituto_Des, '') AS Istituto_Des_AVERE, ISNULL(Ist_Credito_AVERE.Filiale, 0) AS Filiale_AVERE ")
            'PAGAMENTI CAUSALI
            stbQuery.AppendLine(" , ISNULL(Pagamenti_Causali.Cau_Pagamento_Sigla, '') AS Cau_Pagamento_Sigla, ISNULL(Pagamenti_Causali.Cau_Pagamento_Des, '') AS Cau_Pagamento_Des, Pagamenti_Causali.Tipo AS Tipo_Pagamento ")

            'AGENDA - MOVIMENTI
            stbQuery.AppendLine(" FROM    Pagamenti ")

            'PAGAMENTI - LIQUIDITA DARE
            stbQuery.AppendLine(" LEFT OUTER JOIN  Liquidita Liquidita_DARE ON Liquidita_DARE.Piva = Pagamenti.Piva AND Liquidita_DARE.Sa_Cod = Pagamenti.Sa_Cod AND Liquidita_DARE.Cod_Liquidita = Pagamenti.Cod_Liquidita_Dare ")

            'LIQUIDITA DARE - IST CREDITO DARE
            'StbQuery.AppendLine(" LEFT OUTER JOIN Ist_Credito Ist_Credito_DARE ON Ist_Credito_DARE.Piva = Liquidita_DARE.Piva AND Ist_Credito_DARE.Sa_Cod = Liquidita_DARE.Sa_Cod AND Ist_Credito_DARE.Cod_Istituto = Liquidita_DARE.Cod_Istituto ")
            'CORREZIONE BUG DEL 07/01/2013: la piva di ist_credito è quella del superuser!
            stbQuery.AppendLine(" LEFT OUTER JOIN Ist_Credito Ist_Credito_DARE ON Ist_Credito_DARE.Cod_Istituto = Liquidita_DARE.Cod_Istituto ")
            stbQuery.AppendLine("                 AND Ist_Credito_DARE.Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            'PAGAMENTI - LIQUIDITA AVERE
            stbQuery.AppendLine(" LEFT OUTER JOIN  Liquidita Liquidita_AVERE ON Liquidita_AVERE.Piva = Pagamenti.Piva AND Liquidita_AVERE.Sa_Cod = Pagamenti.Sa_Cod AND Liquidita_AVERE.Cod_Liquidita = Pagamenti.Cod_Liquidita_Avere ")

            'LIQUIDITA AVERE - IST CREDITO AVERE
            'StbQuery.AppendLine(" LEFT OUTER JOIN Ist_Credito Ist_Credito_AVERE ON Ist_Credito_AVERE.Piva = Liquidita_AVERE.Piva AND Ist_Credito_AVERE.Sa_Cod = Liquidita_AVERE.Sa_Cod AND Ist_Credito_AVERE.Cod_Istituto = Liquidita_AVERE.Cod_Istituto ")
            'CORREZIONE BUG DEL 07/01/2013: la piva di ist_credito è quella del superuser!
            stbQuery.AppendLine(" LEFT OUTER JOIN Ist_Credito Ist_Credito_AVERE ON Ist_Credito_AVERE.Cod_Istituto = Liquidita_AVERE.Cod_Istituto ")
            stbQuery.AppendLine("                 AND Ist_Credito_AVERE.Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            'PAGAMENTI - PAGAMENTI CAUSALI
            stbQuery.AppendLine(" INNER JOIN  Pagamenti_Causali ON Pagamenti_Causali.Cau_Pagamento = Pagamenti.Cau_Pagamento ")

            stbQuery.AppendLine(" WHERE  ( Pagamenti_Causali.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' OR Pagamenti_Causali.Piva_SuperUser = 'AAAAAAAAAAA' ) ")

            If Previsto_Avvenuto <> enum_Pagamento.NonDefinito Then
                stbQuery.AppendLine(" AND Pagamenti.Previsto_Avvenuto = " & Agro_SQL_SaveNum(Previsto_Avvenuto) & "   ")
            End If

            If Flag_Insolvenze = True Then
                stbQuery.AppendLine(" AND Pagamenti.Data_Pagamento = " & Agro_SQL_SaveDate(AGRODATAFINE) & " ")
            ElseIf Flag_Riscossioni = True Then
                stbQuery.AppendLine(" AND    Pagamenti.Data_Pagamento < " & Agro_SQL_SaveDate(AGRODATAFINE) & " ")
            End If

            If Piva <> "" Then
                stbQuery.AppendLine(" AND Pagamenti.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            'If Sa_Cod <> 0 Then
            stbQuery.AppendLine(" AND Pagamenti.Sa_Cod = " & Agro_SQL_SaveNum(0) & "   ")
            'End If

            If Id_Agenda <> 0 Then
                stbQuery.AppendLine(" AND Pagamenti.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
            End If

            If Id_Mov <> 0 Then
                stbQuery.AppendLine(" AND Pagamenti.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
            End If

            If FiltroAggiuntivo <> "" Then
                stbQuery.AppendLine(Agro_SQL_Save_xFiltroAggiuntivo(FiltroAggiuntivo, , objParametri))
            End If

            stbQuery.AppendLine(" ) ")

            '°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°

            If xOrderBy <> "" Then
                stbQuery.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                stbQuery.AppendLine(" ORDER BY Pagamenti.Id_agenda, Pagamenti.Id_mov, previsto_avvenuto, Percentuale, data_pagamento ")
            End If

            '#############################################################

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbQuery.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function LiquidazioneIVA(ByVal Piva As String,
                                    ByVal Sezionale_Cod As Integer,
                                    ByVal Cod_Conto As Integer,
                                    ByVal Ric_Cod As Integer,
                                    ByVal Anno As Integer,
                                    ByVal Data_Inizio As String,
                                    ByVal Data_Fine As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreStampeDAL.RegistriContab.LiquidazioneIVA"

        'ByVal xFiltroAggiuntivo As String, _
        'ByVal xOrderBy As String, _

        Dim messaggioErrore As String = ""
        'select e join
        Dim Stb_SelectJoin_Contab As New StringBuilder
        Dim Stb_SelectJoin_NoContab As New StringBuilder
        'where
        Dim Stb_Where_Generale As New StringBuilder
        Dim Stb_Where_Contab As New StringBuilder
        Dim Stb_Where_NoContab As New StringBuilder
        'query generale
        Dim Stb_Globale As New StringBuilder
        Dim dt As DataTable

        Dim xFiltroAggiuntivo_xDataRegistrazione, xFiltroAggiuntivo_xDataMovimento As String

        Try

            Try
                'per evitare di non conteggiare dei dati,
                'faccio una query di update per impostare di default la data di registrazione = alla data di movimento
                Imposta_DataRegistrazione_Default(Piva, objParametri)

            Catch ex As Exception

            End Try

            'LAVCOD_ALTRI_COSTI e LAVCOD_ACQUISTO non vengono usati nel registro iva,
            'attenzione a questi se non tornano i conti

            'acquisto è gestito in una parte dedicata della union
            'CStr(LAVCOD_ACQUISTO) & ", " &
            xFiltroAggiuntivo_xDataRegistrazione = " (Agenda.Lav_Cod IN ( " &
                                                    CStr(LAVCOD_FATTURA_RICEVUTA) & ", " &
                                                    CStr(LAVCOD_NOTA_ACCREDITO_RICEVUTA) & ", " &
                                                    CStr(LAVCOD_ALTRI_COSTI) &
                                                    " ) ) "
            '--------------------

            'LAVCOD_ALTRI_RICAVI non viene usato nel registro iva,
            'attenzione a questo se non tornano i conti

            'LAVCOD_AUTOCONSUMO e LAVCOD_AUTOCONSUMO_VINO_SFUSO
            'sono gestiti in una parte dedicata della union
            xFiltroAggiuntivo_xDataMovimento = " (Agenda.Lav_Cod IN ( " &
                                                CStr(LAVCOD_FATTURA_EMESSA) & ", " &
                                                CStr(LAVCOD_FATTURA_LIQ_CONF_EMESSA) & ", " &
                                                CStr(LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA) & ", " &
                                                CStr(LAVCOD_NOTA_ACCREDITO_EMESSA) & ", " &
                                                CStr(LAVCOD_RICEVUTA_EMESSA) & ", " &
                                                CStr(LAVCOD_VENDITA) & ", " &
                                                CStr(LAVCOD_CORRISPETTIVO_VENDITA_SFUSO) & ", " &
                                                CStr(LAVCOD_DDT_CONTABILIZZATO_EMESSO) & ", " &
                                                CStr(LAVCOD_ALTRI_RICAVI) & " " &
                                                " ) ) "

            Stb_SelectJoin_Contab.Length = 0
            Stb_SelectJoin_NoContab.Length = 0
            Stb_Where_Generale.Length = 0
            Stb_Where_Contab.Length = 0
            Stb_Where_NoContab.Length = 0
            Stb_Globale.Length = 0

            '/////////////////////////////////////////////////////////////////////////////
            '            SELECT E JOIN

            '------------------------------------------------------------------------------
            'Select e join dei documenti contabili

            Stb_SelectJoin_Contab.AppendLine(" SELECT Imprese.Rag_Soc AS Impresa, Operazioni.LAV_DES, ")
            Stb_SelectJoin_Contab.AppendLine("   Agenda.PIVA, Agenda.Sa_Cod, Agenda.Id_Agenda, Agenda.Lav_Cod, Agenda.des_lib,  ")
            'movimento contabile
            Stb_SelectJoin_Contab.AppendLine("   Movimenti_Contab.Mov_Desc, ISNULL(Movimenti_Contab.Num_Protocollo,0) AS Num_Protocollo, ")
            Stb_SelectJoin_Contab.AppendLine("   Movimenti_Contab.Data_Movimento, Movimenti_Contab.Scadenza, Movimenti_Contab.Doc_Numero_Sin, Movimenti_Contab.Doc_Numero, Movimenti_Contab.Doc_Numero_Des, ")
            Stb_SelectJoin_Contab.AppendLine("    Movimenti_Contab.Progr_Protocollo, Movimenti_Contab.Progr_Registrazione, Movimenti_Contab.Data_Registrazione, ")
            'dett
            Stb_SelectJoin_Contab.AppendLine("   Movimenti_dettagli.Cod_Iva, Movimenti_dettagli.Cod_IvaIndetraibile, Movimenti_dettagli.Sconto, ")
            Stb_SelectJoin_Contab.AppendLine("     Movimenti_dettagli.Prezzo_Unitario, Movimenti_dettagli.Cod_Conto,  Movimenti_dettagli.Anno, Movimenti_dettagli.Ric_Cod,   ")
            Stb_SelectJoin_Contab.AppendLine("  ROUND(Movimenti_dettagli.Imponibile,2) AS Imponibile, ROUND(Movimenti_dettagli.Iva,2) AS Iva, ROUND(Movimenti_dettagli.Imponibile_Netto,2) AS Imponibile_Netto, ")
            Stb_SelectJoin_Contab.AppendLine("   Movimenti_dettagli.Prezzo_Unitario_Netto, Movimenti_dettagli.Sconto_Modalita,  ")
            Stb_SelectJoin_Contab.AppendLine(" Movimenti_dettagli.Mov_Det_Des, Movimenti_dettagli.Qta, IVA_Aliquote.Sigla AS Sigla_IVA ")

            'JOIN AGENDA - MOVIMENTI
            Stb_SelectJoin_Contab.AppendLine(" FROM    Agenda  ")
            Stb_SelectJoin_Contab.AppendLine(" INNER JOIN Movimenti ")
            Stb_SelectJoin_Contab.AppendLine(" ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda")
            'JOIN AGENDA - OPERAZIONI
            Stb_SelectJoin_Contab.AppendLine(" INNER JOIN Operazioni ON Agenda.Lav_Cod = Operazioni.LAV_COD ")
            'JOIN AGENDA - MOVIMENTI CONTAB
            Stb_SelectJoin_Contab.AppendLine(" INNER JOIN Movimenti Movimenti_Contab ")
            Stb_SelectJoin_Contab.AppendLine(" ON Agenda.PIVA = Movimenti_Contab.PIVA AND Agenda.Sa_Cod = Movimenti_Contab.Sa_Cod AND Agenda.Id_Agenda = Movimenti_Contab.Id_Agenda")
            'JOIN MOVIMENTI - MOVIMENTI DETTAGLI
            Stb_SelectJoin_Contab.AppendLine(" INNER JOIN Movimenti_dettagli ")
            Stb_SelectJoin_Contab.AppendLine(" ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov ")
            'iva
            Stb_SelectJoin_Contab.AppendLine(" INNER JOIN IVA_Aliquote ON IVA_Aliquote.Codice = Movimenti_dettagli.Cod_Iva   ")
            'JOIN IMPRESE - AGENDA
            Stb_SelectJoin_Contab.AppendLine(" INNER JOIN Imprese ON Imprese.Piva = Agenda.Piva ")

            'Select e join dei docuemnti contabili
            '------------------------------------------------------------------------------


            '------------------------------------------------------------------------------
            'Select e join di acquisto e autoconsumo

            Stb_SelectJoin_NoContab.AppendLine(" SELECT Imprese.Rag_Soc AS Impresa, Operazioni.LAV_DES, ")
            Stb_SelectJoin_NoContab.AppendLine("   Agenda.PIVA, Agenda.Sa_Cod, Agenda.Id_Agenda, Agenda.Lav_Cod, Agenda.des_lib,  ")
            'movimento magazzino
            Stb_SelectJoin_NoContab.AppendLine("   Movimenti.Mov_Desc, ISNULL(Movimenti.Num_Protocollo,0) AS Num_Protocollo, ")
            Stb_SelectJoin_NoContab.AppendLine("   Movimenti.Data_Movimento, Movimenti.Scadenza, Movimenti.Doc_Numero_Sin, Movimenti.Doc_Numero, Movimenti.Doc_Numero_Des, ")
            Stb_SelectJoin_NoContab.AppendLine("    Movimenti.Progr_Protocollo, Movimenti.Progr_Registrazione, Movimenti.Data_Registrazione, ")
            'dett
            Stb_SelectJoin_NoContab.AppendLine("   Movimenti_dettagli.Cod_Iva, Movimenti_dettagli.Cod_IvaIndetraibile, Movimenti_dettagli.Sconto, ")
            Stb_SelectJoin_NoContab.AppendLine("     Movimenti_dettagli.Prezzo_Unitario, Movimenti_dettagli.Cod_Conto,  Movimenti_dettagli.Anno, Movimenti_dettagli.Ric_Cod,   ")
            Stb_SelectJoin_NoContab.AppendLine("  ROUND(Movimenti_dettagli.Imponibile,2) AS Imponibile, ROUND(Movimenti_dettagli.Iva,2) AS Iva, ROUND(Movimenti_dettagli.Imponibile_Netto,2) AS Imponibile_Netto, ")
            Stb_SelectJoin_NoContab.AppendLine("   Movimenti_dettagli.Prezzo_Unitario_Netto, Movimenti_dettagli.Sconto_Modalita,  ")
            Stb_SelectJoin_NoContab.AppendLine(" Movimenti_dettagli.Mov_Det_Des, Movimenti_dettagli.Qta, IVA_Aliquote.Sigla AS Sigla_IVA ")

            'JOIN AGENDA - MOVIMENTI
            Stb_SelectJoin_NoContab.AppendLine(" FROM    Agenda  ")
            Stb_SelectJoin_NoContab.AppendLine(" INNER JOIN Movimenti ")
            Stb_SelectJoin_NoContab.AppendLine(" ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda")
            'JOIN AGENDA - OPERAZIONI
            Stb_SelectJoin_NoContab.AppendLine(" INNER JOIN Operazioni ON Agenda.Lav_Cod = Operazioni.LAV_COD ")
            'JOIN MOVIMENTI - MOVIMENTI DETTAGLI
            Stb_SelectJoin_NoContab.AppendLine(" INNER JOIN Movimenti_dettagli ")
            Stb_SelectJoin_NoContab.AppendLine(" ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov ")
            'iva
            Stb_SelectJoin_NoContab.AppendLine(" INNER JOIN IVA_Aliquote ON IVA_Aliquote.Codice = Movimenti_dettagli.Cod_Iva   ")
            'JOIN IMPRESE - AGENDA
            Stb_SelectJoin_NoContab.AppendLine(" INNER JOIN Imprese ON Imprese.Piva = Agenda.Piva ")

            'Select e join acquisto e autoconsumo
            '------------------------------------------------------------------------------


            '/////////////////////////////////////////////////////////////////////////////
            '               WHERE

            '------------------------------------------------------------------------------
            'where dei documenti contabili
            Stb_Where_Contab.AppendLine(" AND     Movimenti_Contab.Cau_Mov = '" & CAU_REGISTRAZIONI & "'   ")
            'aggiunta del 17/04/2013: devo escludere gli acquisti intracom
            Stb_Where_Contab.AppendLine(" AND     Movimenti_Contab.Modalita <> 4  ")

            If Sezionale_Cod <> -1 Then
                Stb_Where_Contab.AppendLine(" AND Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
            End If
            'where dei documenti contabili
            '------------------------------------------------------------------------------

            '------------------------------------------------------------------------------
            'where di acquisto e autoconsumo

            If Sezionale_Cod <> -1 Then
                Stb_Where_NoContab.AppendLine(" AND Movimenti.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
            End If
            'where di acquisto e autoconsumo
            '------------------------------------------------------------------------------


            '------------------------------------------------------------------------------
            'where generale
            If Piva <> "" Then
                Stb_Where_Generale.AppendLine(" AND Movimenti_Dettagli.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Ric_Cod <> 0 Then
                Stb_Where_Generale.AppendLine(" AND Movimenti_Dettagli.Ric_Cod = " & Agro_SQL_SaveNum(Ric_Cod) & "   ")
            End If

            If Anno <> 0 Then
                Stb_Where_Generale.AppendLine(" AND Movimenti_Dettagli.Anno = " & Agro_SQL_SaveNum(Anno) & "   ")
            End If

            If Cod_Conto <> 0 Then
                Stb_Where_Generale.AppendLine(" AND Movimenti_Dettagli.Cod_Conto = " & Agro_SQL_SaveNum(Cod_Conto) & "   ")
            End If

            'If xFiltroAggiuntivo <> "" Then
            '    Stb_Where_Generale.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            'End If
            'where generale
            '------------------------------------------------------------------------------




            '/////////////////////////////////////////////////////////////////////////////

            'MODIFICA DEL 09/08/2012:
            'poichè il giaslan salva nella data di registrazione anche l'ora (che in realtà non serve)
            'le date degli estremi rischiano di non essere conteggiate
            Data_Fine = DateAdd(DateInterval.Day, 1, CDate(Data_Fine))
            'Data_Inizio = DateAdd(DateInterval.Day, -1, CDate(Data_Inizio))

            'nel registro iva era stata fatta una modifica il 10/05/2012: nel registro vendite, filtrare data_movimento
            'mentre nel registro acquisti filtrare data_registrazione
            'la liquidazione iva era rimasta disallineata,
            'in data 09/08/2012 è stato allineato facendo la union

            'MODIFICA DEL 22/08/2012:
            'gestiti nella query gli autoconsumo e l'acquisto


            '///////////////////////////////////////////////
            'UNION DI QUATTRO PARTI:
            Stb_Globale.AppendLine(" SELECT * ")
            Stb_Globale.AppendLine(" FROM ")

            Stb_Globale.AppendLine(" ( ")

            '/************************************************************************************
            '/***** 1° PARTE: DOC CONTABILI FILTRATI X DATA REGISTRAZIONE  ***************
            '/************************************************************************************

            Stb_Globale.AppendLine(" ( ")

            Stb_Globale.AppendLine(Stb_SelectJoin_Contab.ToString)

            'CONDIZIONI
            'leggi nota sopra, sulla data fine
            Stb_Globale.AppendLine(" WHERE   Movimenti_Contab.Data_Registrazione < " & Agro_SQL_SaveDate(Data_Fine) & " ")
            'Stb.AppendLine(" AND     Movimenti_Contab.Data_Registrazione > " & Agro_SQL_SaveDate(Data_Inizio) & " ")
            Stb_Globale.AppendLine(" AND     Movimenti_Contab.Data_Registrazione >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")

            Stb_Globale.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_xDataRegistrazione, , objParametri))

            Stb_Globale.AppendLine(Stb_Where_Contab.ToString)
            Stb_Globale.AppendLine(Stb_Where_Generale.ToString)

            Stb_Globale.AppendLine(" ) ")



            Stb_Globale.AppendLine(" UNION ALL ")
            '/************************************************************************************
            '/***** 2° PARTE: DOC CONTABILI FILTRATI X DATA MOVIMENTO  ***************
            '/************************************************************************************
            Stb_Globale.AppendLine(" ( ")

            Stb_Globale.AppendLine(Stb_SelectJoin_Contab.ToString)

            'CONDIZIONI
            'leggi nota sopra, sulla data fine
            Stb_Globale.AppendLine(" WHERE   Movimenti_Contab.Data_Movimento < " & Agro_SQL_SaveDate(Data_Fine) & " ")
            Stb_Globale.AppendLine(" AND     Movimenti_Contab.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")

            Stb_Globale.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_xDataMovimento, , objParametri))

            Stb_Globale.AppendLine(Stb_Where_Contab.ToString)
            Stb_Globale.AppendLine(Stb_Where_Generale.ToString)

            Stb_Globale.AppendLine(" ) ")


            Stb_Globale.AppendLine(" UNION ALL ")
            '/************************************************************************************
            '/***** 3° PARTE: ACQUISTO FILTRATO X DATA REGISTRAZIONE  ***************
            '/************************************************************************************
            Stb_Globale.AppendLine(" ( ")

            Stb_Globale.AppendLine(Stb_SelectJoin_NoContab.ToString)

            'CONDIZIONI
            'leggi nota sopra, sulla data fine
            Stb_Globale.AppendLine(" WHERE   Movimenti.Data_Registrazione < " & Agro_SQL_SaveDate(Data_Fine) & " ")
            Stb_Globale.AppendLine(" AND     Movimenti.Data_Registrazione >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")

            Stb_Globale.AppendLine(" AND Agenda.lav_cod = " & CStr(LAVCOD_ACQUISTO))

            Stb_Globale.AppendLine(Stb_Where_NoContab.ToString)
            Stb_Globale.AppendLine(Stb_Where_Generale.ToString)

            Stb_Globale.AppendLine(" ) ")



            Stb_Globale.AppendLine(" UNION ALL ")
            '/************************************************************************************
            '/***** 4° PARTE: AUTOCONSUMO FILTRATO X DATA MOVIMENTO  ***************
            '/************************************************************************************
            Stb_Globale.AppendLine(" ( ")

            Stb_Globale.AppendLine(Stb_SelectJoin_NoContab.ToString)

            'CONDIZIONI
            'leggi nota sopra, sulla data fine
            Stb_Globale.AppendLine(" WHERE   Movimenti.Data_Movimento < " & Agro_SQL_SaveDate(Data_Fine) & " ")
            Stb_Globale.AppendLine(" AND     Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")

            Stb_Globale.AppendLine(" AND Agenda.lav_cod IN ( " & CStr(LAVCOD_AUTOCONSUMO) & "," & CStr(LAVCOD_AUTOCONSUMO_VINO_SFUSO) & ")")

            Stb_Globale.AppendLine(Stb_Where_NoContab.ToString)
            Stb_Globale.AppendLine(Stb_Where_Generale.ToString)

            Stb_Globale.AppendLine(" ) ")



            Stb_Globale.AppendLine(" ) LIQ_IVA ")
            'If xOrderBy <> "" Then
            'Stb_Globale.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            'Else
            Stb_Globale.AppendLine(" ORDER BY lav_cod, Id_Agenda ")
            'End If
            Stb_Globale.AppendLine(" ")


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, Stb_Globale.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function


    '#####################################################################
    'attuale funzione della liquidazione iva
    Public Function LiquidazioneIVA_2(ByVal Piva As String,
                                      ByVal Sezionale_Cod As Integer,
                                      ByVal Cod_Conto As Integer,
                                      ByVal Ric_Cod As Integer,
                                      ByVal Anno As Integer,
                                      ByVal Data_Inizio As String,
                                      ByVal Data_Fine As String,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                      ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreStampeDAL.RegistriContab.LiquidazioneIVA_2"

        'ByVal xFiltroAggiuntivo As String, _
        'ByVal xOrderBy As String, _

        Dim messaggioErrore As String = ""
        'select e join
        Dim Stb_SelectJoin_Contab As New StringBuilder
        Dim Stb_SelectJoin_NoContab As New StringBuilder
        'where
        Dim Stb_Where_Generale As New StringBuilder
        Dim Stb_Where_Contab As New StringBuilder
        Dim Stb_Where_NoContab As New StringBuilder
        'query generale
        Dim Stb_Globale As New StringBuilder
        Dim Stb_ChkCoGE As New StringBuilder
        Dim dt As DataTable

        Dim xFiltroAggiuntivo_xDataRegistrazione, xFiltroAggiuntivo_xDataMovimento As String

        Try

            Try
                'per evitare di non conteggiare dei dati,
                'faccio una query di update per impostare di default la data di registrazione = alla data di movimento
                Imposta_DataRegistrazione_Default(Piva, objParametri)

            Catch ex As Exception

            End Try

            ''12/12/2018: modifiche a seguito dei cambiamenti sulla contabilizzazione
            '
            ''MODIFICA DEL 13/08/2014: gestione ChkCoge_Manuale
            'Stb_ChkCoGE.AppendLine(" AND (  ")
            'Stb_ChkCoGE.AppendLine("    ( Agenda.ChkCoge_Manuale = " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & " ) ")
            'Stb_ChkCoGE.AppendLine("    OR ")
            'Stb_ChkCoGE.AppendLine("    ( ")
            'Stb_ChkCoGE.AppendLine("    Agenda.ChkCoge_Manuale = " & CStr(enum_ChkCoGe.CoGe_CONTABILIZZAZIONE_MANUALE) & " ")
            'Stb_ChkCoGE.AppendLine("    AND EXISTS ( ")
            'Stb_ChkCoGE.AppendLine("    SELECT 1 ")
            'Stb_ChkCoGE.AppendLine("    FROM  Mov_Dettagli_Riferimenti ")
            'Stb_ChkCoGE.AppendLine("    INNER JOIN Agenda AgPD on AgPD.PIVA =Mov_Dettagli_Riferimenti.piva ")
            'Stb_ChkCoGE.AppendLine("    AND AgPD.Id_Agenda= Mov_Dettagli_Riferimenti.id_agenda ")
            'Stb_ChkCoGE.AppendLine("    WHERE Mov_Dettagli_Riferimenti.Piva_Rif=agenda.piva   ")
            'Stb_ChkCoGE.AppendLine("    AND Mov_Dettagli_Riferimenti.Id_Agenda_Rif=agenda.id_agenda ")
            'Stb_ChkCoGE.AppendLine("    AND Mov_Dettagli_Riferimenti.Id_mov_Rif=-1 ")
            'Stb_ChkCoGE.AppendLine("    AND Mov_Dettagli_Riferimenti.Id_mov_det_Rif=-1 ")
            'Stb_ChkCoGE.AppendLine("    AND Mov_Dettagli_Riferimenti.lav_cod = " & CStr(LAVCOD_MOV_FINANZIARIO) & " ")
            'Stb_ChkCoGE.AppendLine("    AND AgPD.ChkCoge_Manuale = " & CStr(enum_ChkCoGe.CoGe_PD_COLLEGATAaDOCUMENTO) & " ")
            'Stb_ChkCoGE.AppendLine("            ) -- exists ")
            'Stb_ChkCoGE.AppendLine("        ) -- or ")
            'Stb_ChkCoGE.AppendLine("    ) -- and ")
            'Stb_ChkCoGE.AppendLine("  ")
            Stb_ChkCoGE.AppendLine(" AND Agenda.ChkCoge_Manuale IN ( " & CStr(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA) & "  ")
            Stb_ChkCoGE.AppendLine("                                , " & CStr(enum_ChkCoGe.CoGe_CONTABILIZZAZIONE_MANUALE) & " ")
            Stb_ChkCoGE.AppendLine("                                , " & CStr(enum_ChkCoGe.CoGe_CONTABILIZZAZIONE_AUTOMATICA) & " ")
            Stb_ChkCoGE.AppendLine("                                ) ")




            'LAVCOD_ALTRI_COSTI e LAVCOD_ACQUISTO non vengono usati nel registro iva,
            'attenzione a questi se non tornano i conti

            'acquisto è gestito in una parte dedicata della union
            'CStr(LAVCOD_ACQUISTO) & ", " &
            xFiltroAggiuntivo_xDataRegistrazione = " (Agenda.Lav_Cod IN ( " &
                                                    CStr(LAVCOD_FATTURA_RICEVUTA) & ", " &
                                                    CStr(LAVCOD_FATTURA_PROFESSIONISTI) & ", " &
                                                    CStr(LAVCOD_NOTA_ACCREDITO_RICEVUTA) & ", " &
                                                    CStr(LAVCOD_ALTRI_COSTI) &
                                                    " ) ) "
            '--------------------

            'LAVCOD_ALTRI_RICAVI non viene usato nel registro iva,
            'attenzione a questo se non tornano i conti

            'LAVCOD_AUTOCONSUMO e LAVCOD_AUTOCONSUMO_VINO_SFUSO
            'sono gestiti in una parte dedicata della union
            xFiltroAggiuntivo_xDataMovimento = " (Agenda.Lav_Cod IN ( " &
                                                CStr(LAVCOD_FATTURA_EMESSA) & ", " &
                                                CStr(LAVCOD_FATTURA_LIQ_CONF_EMESSA) & ", " &
                                                CStr(LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA) & ", " &
                                                CStr(LAVCOD_NOTA_ACCREDITO_EMESSA) & ", " &
                                                CStr(LAVCOD_RICEVUTA_EMESSA) & ", " &
                                                CStr(LAVCOD_VENDITA) & ", " &
                                                CStr(LAVCOD_CORRISPETTIVO_VENDITA_SFUSO) & ", " &
                                                CStr(LAVCOD_DDT_CONTABILIZZATO_EMESSO) & ", " &
                                                CStr(LAVCOD_ALTRI_RICAVI) & " " &
                                                " ) ) "

            Stb_SelectJoin_Contab.Length = 0
            Stb_SelectJoin_NoContab.Length = 0
            Stb_Where_Generale.Length = 0
            Stb_Where_Contab.Length = 0
            Stb_Where_NoContab.Length = 0
            Stb_Globale.Length = 0

            '/////////////////////////////////////////////////////////////////////////////
            '            SELECT E JOIN

            '------------------------------------------------------------------------------
            'Select e join dei documenti contabili

            Stb_SelectJoin_Contab.AppendLine(" SELECT Imprese.Rag_Soc AS Impresa, Operazioni.LAV_DES, ")
            Stb_SelectJoin_Contab.AppendLine("   Agenda.PIVA, Agenda.Sa_Cod, Agenda.Id_Agenda, Agenda.Lav_Cod, Agenda.des_lib,  ")
            'movimento contabile
            Stb_SelectJoin_Contab.AppendLine("   Movimenti_Contab.Mov_Desc, ISNULL(Movimenti_Contab.Num_Protocollo,0) AS Num_Protocollo, ISNULL(Movimenti_Contab.Tipo_Sconto,0) AS Tipo_Sconto, ")
            Stb_SelectJoin_Contab.AppendLine("   Movimenti_Contab.Data_Movimento, Movimenti_Contab.Scadenza, Movimenti_Contab.Doc_Numero_Sin, Movimenti_Contab.Doc_Numero, Movimenti_Contab.Doc_Numero_Des, ")
            Stb_SelectJoin_Contab.AppendLine("    Movimenti_Contab.Progr_Protocollo, Movimenti_Contab.Progr_Registrazione, Movimenti_Contab.Data_Registrazione, Movimenti_Contab.Modalita, Movimenti_Contab.Sezionale_Cod,")
            'iva aliquote
            Stb_SelectJoin_Contab.AppendLine("  IVA_Aliquote.aliquota, IVA_Aliquote.Sigla AS Sigla_IVA, ")
            'movimenti_dettagli
            Stb_SelectJoin_Contab.AppendLine("      ")
            Stb_SelectJoin_Contab.AppendLine("  Movimenti_dettagli.Id_Mov_Det, Movimenti_dettagli.ChkLayOut_Hide, Movimenti_dettagli.Mov_Det_Des, ")
            Stb_SelectJoin_Contab.AppendLine("  Movimenti_dettagli.sconto , Movimenti_dettagli.sconto_listino , Movimenti_dettagli.sconto_modalita , ")
            Stb_SelectJoin_Contab.AppendLine("  Movimenti_dettagli.Cod_Iva, --Movimenti_dettagli.Cod_IvaIndetraibile, ")
            Stb_SelectJoin_Contab.AppendLine("  Movimenti_dettagli.Iva_Indetraibile, Movimenti_dettagli.Iva_Indetraibile_Perc, ")
            Stb_SelectJoin_Contab.AppendLine("  Movimenti_dettagli.Qta, ")
            Stb_SelectJoin_Contab.AppendLine("  Movimenti_dettagli.Prezzo_Unitario, ")
            Stb_SelectJoin_Contab.AppendLine("  Movimenti_dettagli.Prezzo_Unitario_Netto, ")
            Stb_SelectJoin_Contab.AppendLine("  Movimenti_dettagli.Imponibile, ")
            Stb_SelectJoin_Contab.AppendLine("  Movimenti_dettagli.Imponibile_Netto, ")
            Stb_SelectJoin_Contab.AppendLine("  Movimenti_dettagli.Iva, ")
            Stb_SelectJoin_Contab.AppendLine("  Movimenti_dettagli.Cod_Conto,  Movimenti_dettagli.Anno, Movimenti_dettagli.Ric_Cod   ")

            'JOIN AGENDA - MOVIMENTI
            Stb_SelectJoin_Contab.AppendLine(" FROM    Agenda  ")
            Stb_SelectJoin_Contab.AppendLine(" INNER JOIN Movimenti ")
            Stb_SelectJoin_Contab.AppendLine(" ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda")
            'JOIN AGENDA - OPERAZIONI
            Stb_SelectJoin_Contab.AppendLine(" INNER JOIN Operazioni ON Agenda.Lav_Cod = Operazioni.LAV_COD ")
            'JOIN AGENDA - MOVIMENTI CONTAB
            Stb_SelectJoin_Contab.AppendLine(" INNER JOIN Movimenti Movimenti_Contab ")
            Stb_SelectJoin_Contab.AppendLine(" ON Agenda.PIVA = Movimenti_Contab.PIVA AND Agenda.Sa_Cod = Movimenti_Contab.Sa_Cod AND Agenda.Id_Agenda = Movimenti_Contab.Id_Agenda")
            'JOIN MOVIMENTI - MOVIMENTI DETTAGLI
            Stb_SelectJoin_Contab.AppendLine(" INNER JOIN Movimenti_dettagli ")
            Stb_SelectJoin_Contab.AppendLine(" ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov ")
            'iva
            Stb_SelectJoin_Contab.AppendLine(" INNER JOIN IVA_Aliquote ON IVA_Aliquote.Codice = Movimenti_dettagli.Cod_Iva   ")
            'JOIN IMPRESE - AGENDA
            Stb_SelectJoin_Contab.AppendLine(" INNER JOIN Imprese ON Imprese.Piva = Agenda.Piva ")

            'Select e join dei docuemnti contabili
            '------------------------------------------------------------------------------


            '------------------------------------------------------------------------------
            'Select e join di acquisto e autoconsumo

            Stb_SelectJoin_NoContab.AppendLine(" SELECT Imprese.Rag_Soc AS Impresa, Operazioni.LAV_DES, ")
            Stb_SelectJoin_NoContab.AppendLine("   Agenda.PIVA, Agenda.Sa_Cod, Agenda.Id_Agenda, Agenda.Lav_Cod, Agenda.des_lib,  ")
            'movimento magazzino
            Stb_SelectJoin_NoContab.AppendLine("   Movimenti.Mov_Desc, ISNULL(Movimenti.Num_Protocollo,0) AS Num_Protocollo, ISNULL(Movimenti.Tipo_Sconto,0) AS Tipo_Sconto, ")
            Stb_SelectJoin_NoContab.AppendLine("   Movimenti.Data_Movimento, Movimenti.Scadenza, Movimenti.Doc_Numero_Sin, Movimenti.Doc_Numero, Movimenti.Doc_Numero_Des, ")
            Stb_SelectJoin_NoContab.AppendLine("    Movimenti.Progr_Protocollo, Movimenti.Progr_Registrazione, Movimenti.Data_Registrazione, Movimenti.Modalita, Movimenti.Sezionale_Cod, ")
            'iva aliquote
            Stb_SelectJoin_NoContab.AppendLine("  IVA_Aliquote.aliquota, IVA_Aliquote.Sigla AS Sigla_IVA, ")
            'movimenti_dettagli
            Stb_SelectJoin_NoContab.AppendLine("      ")
            Stb_SelectJoin_NoContab.AppendLine("  Movimenti_dettagli.Id_Mov_Det, Movimenti_dettagli.ChkLayOut_Hide, Movimenti_dettagli.Mov_Det_Des, ")
            Stb_SelectJoin_NoContab.AppendLine("  Movimenti_dettagli.sconto , Movimenti_dettagli.sconto_listino , Movimenti_dettagli.sconto_modalita , ")
            Stb_SelectJoin_NoContab.AppendLine("  Movimenti_dettagli.Cod_Iva, --Movimenti_dettagli.Cod_IvaIndetraibile, ")
            Stb_SelectJoin_NoContab.AppendLine("  Movimenti_dettagli.Iva_Indetraibile, Movimenti_dettagli.Iva_Indetraibile_Perc, ")
            Stb_SelectJoin_NoContab.AppendLine("  Movimenti_dettagli.Qta, ")
            Stb_SelectJoin_NoContab.AppendLine("  Movimenti_dettagli.Prezzo_Unitario, ")
            Stb_SelectJoin_NoContab.AppendLine("  Movimenti_dettagli.Prezzo_Unitario_Netto, ")
            Stb_SelectJoin_NoContab.AppendLine("  Movimenti_dettagli.Imponibile, ")
            Stb_SelectJoin_NoContab.AppendLine("  Movimenti_dettagli.Imponibile_Netto, ")
            Stb_SelectJoin_NoContab.AppendLine("  Movimenti_dettagli.Iva, ")
            Stb_SelectJoin_NoContab.AppendLine("  Movimenti_dettagli.Cod_Conto,  Movimenti_dettagli.Anno, Movimenti_dettagli.Ric_Cod   ")

            ''dett
            'Stb_SelectJoin_NoContab.AppendLine("   Movimenti_dettagli.Cod_Iva, Movimenti_dettagli.Cod_IvaIndetraibile, Movimenti_dettagli.Sconto, ")
            'Stb_SelectJoin_NoContab.AppendLine("     Movimenti_dettagli.Prezzo_Unitario, Movimenti_dettagli.Cod_Conto,  Movimenti_dettagli.Anno, Movimenti_dettagli.Ric_Cod,   ")
            'Stb_SelectJoin_NoContab.AppendLine("  ROUND(Movimenti_dettagli.Imponibile,2) AS Imponibile, ROUND(Movimenti_dettagli.Iva,2) AS Iva, ROUND(Movimenti_dettagli.Imponibile_Netto,2) AS Imponibile_Netto, ")
            'Stb_SelectJoin_NoContab.AppendLine("   Movimenti_dettagli.Prezzo_Unitario_Netto, Movimenti_dettagli.Sconto_Modalita,  ")
            'Stb_SelectJoin_NoContab.AppendLine(" Movimenti_dettagli.Mov_Det_Des, Movimenti_dettagli.Qta, IVA_Aliquote.Sigla AS Sigla_IVA ")

            'JOIN AGENDA - MOVIMENTI
            Stb_SelectJoin_NoContab.AppendLine(" FROM    Agenda  ")
            Stb_SelectJoin_NoContab.AppendLine(" INNER JOIN Movimenti ")
            Stb_SelectJoin_NoContab.AppendLine(" ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda")
            'JOIN AGENDA - OPERAZIONI
            Stb_SelectJoin_NoContab.AppendLine(" INNER JOIN Operazioni ON Agenda.Lav_Cod = Operazioni.LAV_COD ")
            'JOIN MOVIMENTI - MOVIMENTI DETTAGLI
            Stb_SelectJoin_NoContab.AppendLine(" INNER JOIN Movimenti_dettagli ")
            Stb_SelectJoin_NoContab.AppendLine(" ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov ")
            'iva
            Stb_SelectJoin_NoContab.AppendLine(" INNER JOIN IVA_Aliquote ON IVA_Aliquote.Codice = Movimenti_dettagli.Cod_Iva   ")
            'JOIN IMPRESE - AGENDA
            Stb_SelectJoin_NoContab.AppendLine(" INNER JOIN Imprese ON Imprese.Piva = Agenda.Piva ")

            'Select e join acquisto e autoconsumo
            '------------------------------------------------------------------------------


            '/////////////////////////////////////////////////////////////////////////////
            '               WHERE

            '------------------------------------------------------------------------------
            'where dei documenti contabili
            Stb_Where_Contab.AppendLine(" AND     Movimenti_Contab.Cau_Mov = '" & CAU_REGISTRAZIONI & "'   ")

            'modifica del 02/08/2013: nuova gestione reg iva e liq iva, queste fatture non vanno escluse
            ''aggiunta del 17/04/2013: devo escludere gli acquisti intracom
            'Stb_Where_Contab.AppendLine(" AND     Movimenti_Contab.Modalita <> 4  ")

            If Sezionale_Cod <> -1 Then
                Stb_Where_Contab.AppendLine(" AND Movimenti_Contab.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
            End If
            'where dei documenti contabili
            '------------------------------------------------------------------------------

            '------------------------------------------------------------------------------
            'where di acquisto e autoconsumo

            If Sezionale_Cod <> -1 Then
                Stb_Where_NoContab.AppendLine(" AND Movimenti.Sezionale_Cod = " & Agro_SQL_SaveNum(Sezionale_Cod) & "   ")
            End If
            'where di acquisto e autoconsumo
            '------------------------------------------------------------------------------


            '------------------------------------------------------------------------------
            'where generale

            'Giulia - 27/06/2017 - bisogna escludere sempre la riga descrizione libera, perchè non contiene alcun dato contabile e "sporcherebbe" i risultati
            Stb_Where_Generale.AppendLine(" AND Movimenti_Dettagli.Elem_Cod <> " & Agro_SQL_SaveNum(RIGA_DESCRIZIONE_LIBERA) & "   ")

            If Piva <> "" Then
                Stb_Where_Generale.AppendLine(" AND Movimenti_Dettagli.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Ric_Cod <> 0 Then
                Stb_Where_Generale.AppendLine(" AND Movimenti_Dettagli.Ric_Cod = " & Agro_SQL_SaveNum(Ric_Cod) & "   ")
            End If

            If Anno <> 0 Then
                Stb_Where_Generale.AppendLine(" AND Movimenti_Dettagli.Anno = " & Agro_SQL_SaveNum(Anno) & "   ")
            End If

            If Cod_Conto <> 0 Then
                Stb_Where_Generale.AppendLine(" AND Movimenti_Dettagli.Cod_Conto = " & Agro_SQL_SaveNum(Cod_Conto) & "   ")
            End If

            'If xFiltroAggiuntivo <> "" Then
            '    Stb_Where_Generale.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            'End If
            'where generale
            '------------------------------------------------------------------------------




            '/////////////////////////////////////////////////////////////////////////////

            'MODIFICA DEL 09/08/2012:
            'poichè il giaslan salva nella data di registrazione anche l'ora (che in realtà non serve)
            'le date degli estremi rischiano di non essere conteggiate
            Data_Fine = DateAdd(DateInterval.Day, 1, CDate(Data_Fine))
            'Data_Inizio = DateAdd(DateInterval.Day, -1, CDate(Data_Inizio))

            'nel registro iva era stata fatta una modifica il 10/05/2012: nel registro vendite, filtrare data_movimento
            'mentre nel registro acquisti filtrare data_registrazione
            'la liquidazione iva era rimasta disallineata,
            'in data 09/08/2012 è stato allineato facendo la union

            'MODIFICA DEL 22/08/2012:
            'gestiti nella query gli autoconsumo e l'acquisto


            '///////////////////////////////////////////////
            'UNION DI QUATTRO PARTI:
            Stb_Globale.AppendLine(" SELECT * ")
            Stb_Globale.AppendLine(" FROM ")

            Stb_Globale.AppendLine(" ( ")

            '/************************************************************************************
            '/***** 1° PARTE: DOC CONTABILI FILTRATI X DATA REGISTRAZIONE  ***************
            '/************************************************************************************

            Stb_Globale.AppendLine(" ( ")

            Stb_Globale.AppendLine(Stb_SelectJoin_Contab.ToString)

            'CONDIZIONI
            'leggi nota sopra, sulla data fine
            Stb_Globale.AppendLine(" WHERE   Movimenti_Contab.Data_Registrazione < " & Agro_SQL_SaveDate(Data_Fine) & " ")
            'Stb.AppendLine(" AND     Movimenti_Contab.Data_Registrazione > " & Agro_SQL_SaveDate(Data_Inizio) & " ")
            Stb_Globale.AppendLine(" AND     Movimenti_Contab.Data_Registrazione >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")

            Stb_Globale.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_xDataRegistrazione, , objParametri))

            Stb_Globale.AppendLine(Stb_Where_Contab.ToString)
            Stb_Globale.AppendLine(Stb_Where_Generale.ToString)

            Stb_Globale.AppendLine(Stb_ChkCoGE.ToString)

            Stb_Globale.AppendLine(" ) ")



            Stb_Globale.AppendLine(" UNION ALL ")
            '/************************************************************************************
            '/***** 2° PARTE: DOC CONTABILI FILTRATI X DATA MOVIMENTO  ***************
            '/************************************************************************************
            Stb_Globale.AppendLine(" ( ")

            Stb_Globale.AppendLine(Stb_SelectJoin_Contab.ToString)

            'CONDIZIONI
            'leggi nota sopra, sulla data fine
            Stb_Globale.AppendLine(" WHERE   Movimenti_Contab.Data_Movimento < " & Agro_SQL_SaveDate(Data_Fine) & " ")
            Stb_Globale.AppendLine(" AND     Movimenti_Contab.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")

            Stb_Globale.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_xDataMovimento, , objParametri))

            Stb_Globale.AppendLine(Stb_Where_Contab.ToString)
            Stb_Globale.AppendLine(Stb_Where_Generale.ToString)

            Stb_Globale.AppendLine(Stb_ChkCoGE.ToString)

            Stb_Globale.AppendLine(" ) ")


            Stb_Globale.AppendLine(" UNION ALL ")
            '/************************************************************************************
            '/***** 3° PARTE: ACQUISTO FILTRATO X DATA REGISTRAZIONE  ***************
            '/************************************************************************************
            Stb_Globale.AppendLine(" ( ")

            Stb_Globale.AppendLine(Stb_SelectJoin_NoContab.ToString)

            'CONDIZIONI
            'leggi nota sopra, sulla data fine
            Stb_Globale.AppendLine(" WHERE   Movimenti.Data_Registrazione < " & Agro_SQL_SaveDate(Data_Fine) & " ")
            Stb_Globale.AppendLine(" AND     Movimenti.Data_Registrazione >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")

            Stb_Globale.AppendLine(" AND Agenda.lav_cod = " & CStr(LAVCOD_ACQUISTO))

            Stb_Globale.AppendLine(Stb_Where_NoContab.ToString)
            Stb_Globale.AppendLine(Stb_Where_Generale.ToString)

            Stb_Globale.AppendLine(" ) ")



            Stb_Globale.AppendLine(" UNION ALL ")
            '/************************************************************************************
            '/***** 4° PARTE: AUTOCONSUMO FILTRATO X DATA MOVIMENTO  ***************
            '/************************************************************************************
            Stb_Globale.AppendLine(" ( ")

            Stb_Globale.AppendLine(Stb_SelectJoin_NoContab.ToString)

            'CONDIZIONI
            'leggi nota sopra, sulla data fine
            Stb_Globale.AppendLine(" WHERE   Movimenti.Data_Movimento < " & Agro_SQL_SaveDate(Data_Fine) & " ")
            Stb_Globale.AppendLine(" AND     Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")

            Stb_Globale.AppendLine(" AND Agenda.lav_cod IN ( " & CStr(LAVCOD_AUTOCONSUMO) & "," & CStr(LAVCOD_AUTOCONSUMO_VINO_SFUSO) & ")")

            Stb_Globale.AppendLine(Stb_Where_NoContab.ToString)
            Stb_Globale.AppendLine(Stb_Where_Generale.ToString)

            Stb_Globale.AppendLine(" ) ")



            Stb_Globale.AppendLine(" ) LIQ_IVA ")
            'Stb_Globale.AppendLine(" ORDER BY lav_cod, Id_Agenda ")
            Stb_Globale.AppendLine(" ORDER BY Id_Agenda, cod_iva ")

            Stb_Globale.AppendLine(" ")


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, Stb_Globale.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function


    '##################################################################################
    Private Function Imposta_DataRegistrazione_Default(ByVal Piva As String,
                                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                       ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_W.DataRegistrazione_Default()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            strSql.Length = 0
            strSql.AppendLine(" UPDATE Movimenti  ")
            strSql.AppendLine(" SET  Data_Registrazione  =  Data_Movimento  ")

            strSql.AppendLine(" WHERE Piva      = '" & Agro_SQL_SaveText(Piva) & "'  ")
            strSql.AppendLine(" AND   Data_Registrazione <=  " & Agro_SQL_SaveDate(AGRODATAINIZIO) & "   ")
            strSql.AppendLine(" AND   Cau_Mov =  '" & CAU_REGISTRAZIONI & "'   ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

End Class
