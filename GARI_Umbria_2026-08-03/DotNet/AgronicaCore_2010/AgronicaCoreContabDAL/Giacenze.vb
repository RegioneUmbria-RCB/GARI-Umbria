Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreModelsSTD.metaschema
Imports AgronicaCoreDataProvider
Imports System.Web.UI

Public Class Giacenze_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '#############################################################################################
    'legge le tabelle dell'agenda per le giacenze
    'con gli inner join sulle tabelle fabbricati, unità di misura

    ' LEGGE RECORD STATICO NON USARE
    'Public Function Giacenze_Leggi( _
    '                                ByVal Piva As String, _
    '                                ByVal Sa_Cod As Integer, _
    '                                ByVal Id_Destinazione As Integer, _
    '                                ByVal Tipo_Scorta As Integer, _
    '                                ByVal Scorta_Min As Decimal, _
    '                                ByVal Elem_Cod As Integer, _
    '                                ByVal Pro_Cod As Integer, _
    '                                ByVal Mat_Cod As Integer, _
    '                                ByVal Cal_Cod As Integer, _
    '                                ByVal Cod_Progetto As Integer, _
    '                                ByVal Fase_Cod As Integer, _
    '                                ByVal Udm_Cod As Integer, _
    '                                ByVal Lotto As String, _
    '                                ByVal Id_Agenda As Integer, _
    '                                ByVal Id_Mov As Integer, _
    '                                ByVal Id_Mov_Det As Integer, _
    '                                ByVal Data_Giacenza As Date, _
    '                                ByVal xFiltroAggiuntivo As String, _
    '                                ByVal xOrderBy As String, _
    '                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                                ) As DataTable

    '    Dim NomeRoutine As String = "AgronicaCoreContabDAL.Giacenze_R.Giacenze_Leggi()"

    '    Dim MessaggioErrore As String = ""
    '    Dim SQL_Generale As New System.Text.StringBuilder
    '    Dim DT As DataTable

    '    Try

    '        '------------------------------------------------------------------
    '        SQL_Generale.Length = 0

    '        '------------------------------------------------------ 
    '        '------------------- SELECT ---------------------------
    '        '------------------------------------------------------
    '        SQL_Generale.Append(" SELECT Imprese.Rag_Soc AS Impresa, CategorieMagazzino.*, " + vbCrLf)
    '        SQL_Generale.Append(" Movimenti_dettagli.*, Mov_Destinazioni.Id_Destinazione, Mov_Destinazioni.Tipo_Destinazione, Mov_Destinazioni.Qta AS Qta_Dest, Mov_Destinazioni.Qta2, " + vbCrLf)
    '        SQL_Generale.Append(" Mov_Destinazioni.Tipo_Scorta, Mov_Destinazioni.Scorta_Min, Fabbricati.Fabbricato_Des, Fabbricati_Tipi.Tipo_Fabbricato_Cod,  " + vbCrLf)
    '        SQL_Generale.Append(" Fabbricati_Tipi.Tipo_Fabbricato_Des, UnitaMisura.Udm_Des, UnitaMisura.Udm_Sim " + vbCrLf)

    '        '------------------------------------------------------ 
    '        '-------------------- FROM ----------------------------
    '        '------------------------------------------------------
    '        'JOIN AGENDA - MOVIMENTI
    '        SQL_Generale.Append(" FROM    Agenda " + vbCrLf)


    '        '------------------------------------------------------ 
    '        '-------------------- JOIN ----------------------------
    '        '------------------------------------------------------
    '        'JOIN AGENDA - MOVIMENTI
    '        SQL_Generale.Append(" INNER JOIN Movimenti " + vbCrLf)
    '        SQL_Generale.Append(" ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda" + vbCrLf)

    '        'JOIN IMPRESE - AGENDA
    '        SQL_Generale.Append(" INNER JOIN Imprese ON Imprese.Piva = Agenda.Piva " + vbCrLf)

    '        'JOIN MOVIMENTI - MOVIMENTI DETTAGLI
    '        SQL_Generale.Append(" INNER JOIN Movimenti_dettagli " + vbCrLf)
    '        SQL_Generale.Append(" ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov " + vbCrLf)

    '        'JOIN MOVIMENTI DETTAGLI - MOVIMENTI DESTINAZIONI
    '        SQL_Generale.Append(" INNER JOIN Mov_Destinazioni " + vbCrLf)
    '        SQL_Generale.Append(" ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod " + vbCrLf)
    '        SQL_Generale.Append(" AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov " + vbCrLf)
    '        SQL_Generale.Append(" AND Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det " + vbCrLf)

    '        'JOIN FABBRICATO
    '        SQL_Generale.Append(" INNER JOIN Fabbricati ON Mov_Destinazioni.Piva = Fabbricati.PIVA AND Mov_Destinazioni.Sa_Cod = Fabbricati.SA_COD AND " + vbCrLf)
    '        SQL_Generale.Append(" Mov_Destinazioni.Id_Destinazione = Fabbricati.Fabbricato_Cod " + vbCrLf)

    '        'JOIN TIPO FABBRICATO
    '        SQL_Generale.Append(" INNER JOIN Fabbricati_Tipi ON Fabbricati.Tipo_Fabbricato_Cod = Fabbricati_Tipi.Tipo_Fabbricato_Cod " + vbCrLf)

    '        'JOIN CATEGORIE MAGAZZINO
    '        SQL_Generale.Append(" INNER JOIN CategorieMagazzino ON Movimenti_dettagli.Elem_Cod = CategorieMagazzino.Elem_Cod " + vbCrLf)

    '        'JOIN UNITA DI MISURA
    '        SQL_Generale.Append(" INNER JOIN UnitaMisura ON Movimenti_dettagli.Udm_Cod = UnitaMisura.Udm_Cod " + vbCrLf)

    '        '------------------------------------------------------ 
    '        '-------------------- WHERE ---------------------------
    '        '------------------------------------------------------

    '        SQL_Generale.Append(" WHERE     Agenda.Lav_Cod = -1 " + vbCrLf)
    '        SQL_Generale.Append(" AND       Movimenti.Cau_Mov = 'GIACENZE' " + vbCrLf)
    '        SQL_Generale.Append(" AND       Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Giacenza) & " " + vbCrLf)
    '        SQL_Generale.Append(" AND       Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(AGRODATAINIZIO) & " " + vbCrLf)
    '        SQL_Generale.Append(" AND       Movimenti_Dettagli.Validita_inizio <= " & Agro_SQL_SaveDate(Data_Giacenza))
    '        SQL_Generale.Append(" AND       Movimenti_Dettagli.Validita_Fine >= " & Agro_SQL_SaveDate(AGRODATAINIZIO))
    '        SQL_Generale.Append(" AND       Movimenti_Dettagli.Elem_Cod <> " + CStr(CALI_LAVORAZIONE) + " " + vbCrLf) 'escludere i cali di lavorazione
    '        SQL_Generale.Append(" AND       Movimenti_Dettagli.Elem_Cod <> " + CStr(CORPI_ESTRANEI) + " " + vbCrLf) 'escludere i corpiestranei
    '        SQL_Generale.Append(" AND       Movimenti_Dettagli.Qta <> 0 " + vbCrLf) 'solo giacenze significative
    '        SQL_Generale.Append(" AND       Mov_Destinazioni.Tipo_Destinazione = " + CStr(TIPO_DESTINAZIONE_MAGAZZINO) + "" + vbCrLf)

    '        If Piva <> "" Then
    '            SQL_Generale.Append(" AND Movimenti_Dettagli.Piva = '" & Agro_SQL_SaveText(Piva) & "'   " + vbCrLf)
    '        End If

    '        If Sa_Cod <> 0 Then
    '            SQL_Generale.Append(" AND Movimenti_Dettagli.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   " + vbCrLf)
    '        End If

    '        If Elem_Cod <> 0 Then
    '            SQL_Generale.Append(" AND Movimenti_Dettagli.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   " + vbCrLf)
    '        End If

    '        If Pro_Cod <> 0 Then
    '            SQL_Generale.Append(" AND Movimenti_Dettagli.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   " + vbCrLf)
    '        End If

    '        If Mat_Cod <> 0 Then
    '            SQL_Generale.Append(" AND Movimenti_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   " + vbCrLf)
    '        End If

    '        If Cal_Cod <> 0 Then
    '            SQL_Generale.Append(" AND Movimenti_Dettagli.Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "   " + vbCrLf)
    '        End If

    '        If Cod_Progetto <> 0 Then
    '            SQL_Generale.Append(" AND Movimenti_Dettagli.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & "   " + vbCrLf)
    '        End If

    '        If Fase_Cod <> 0 Then
    '            SQL_Generale.Append(" AND Movimenti_Dettagli.Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   " + vbCrLf)
    '        End If

    '        If Udm_Cod <> 0 Then
    '            SQL_Generale.Append(" AND Movimenti_Dettagli.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   " + vbCrLf)
    '        End If

    '        If Lotto <> LOTTO_NONDEFINITO Then
    '            SQL_Generale.Append(" AND Movimenti_Dettagli.Lotto = '" & Agro_SQL_SaveText(Lotto) & "'   " + vbCrLf)
    '        End If


    '        If Id_Destinazione <> 0 Then
    '            SQL_Generale.Append(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "   " + vbCrLf)
    '        End If

    '        If Tipo_Scorta <> 0 Then
    '            SQL_Generale.Append(" AND Mov_Destinazioni.Tipo_Scorta = " & Agro_SQL_SaveNum(Tipo_Scorta) & "   " + vbCrLf)
    '        End If

    '        If Scorta_Min <> 0 Then
    '            SQL_Generale.Append(" AND Mov_Destinazioni.Scorta_Min = " & Agro_SQL_SaveNum(Scorta_Min) & "   " + vbCrLf)
    '        End If

    '        If Id_Agenda <> 0 Then
    '            SQL_Generale.Append(" AND Movimenti_Dettagli.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   " + vbCrLf)
    '        End If

    '        If Id_Mov <> 0 Then
    '            SQL_Generale.Append(" AND Movimenti_Dettagli.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   " + vbCrLf)
    '        End If

    '        If Id_Mov_Det <> 0 Then
    '            SQL_Generale.Append(" AND Movimenti_Dettagli.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   " + vbCrLf)
    '        End If

    '        If xFiltroAggiuntivo <> "" Then
    '            SQL_Generale.Append("AND " + xFiltroAggiuntivo)
    '        End If
    '        '--------------------------------------------------------------------------
    '        Select Case objParametri.FlagVisibilita
    '            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
    '                SQL_Generale.Append(" AND   Movimenti_Dettagli.Inviato >=0 ")
    '            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
    '                SQL_Generale.Append(" AND   Movimenti_Dettagli.Inviato =-1 ")
    '            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
    '                '...................................
    '            Case Else
    '                Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
    '        End Select
    '        '--------------------------------------------------------------------------
    '        If xOrderBy <> "" Then
    '            SQL_Generale.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
    '        Else
    '            SQL_Generale.Append(" ORDER BY Agenda.PIVA, Movimenti.Data_Movimento DESC ")
    '        End If


    '        '--------------------------------------------------------------------------
    '        DT = EseguiQuery_Lettura(objParametri, SQL_Generale.ToString, NomeRoutine)
    '        '--------------------------------------------------------------------------

    '    Catch ex As Exception
    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        DT = Nothing
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
    '    End Try

    '    Return DT

    'End Function


    '#############################################################################################
    'legge le tabelle dell'agenda per le giacenze
    'con gli inner join sulle tabelle fabbricati, unità di misura
    Public Function Giacenze_Leggi_MateriePrime(
                                                        ByVal Piva As String,
                                                        ByVal Sa_Cod As Integer,
                                                        ByVal Id_Destinazione As Integer,
                                                        ByVal Elem_Cod As Integer,
                                                        ByVal Pro_Cod As Integer,
                                                        ByVal Mat_Cod As Integer,
                                                        ByVal Udm_Cod As Integer,
                                                        ByVal Cal_Cod As Integer,
                                                        ByVal Cod_Progetto As Integer,
                                                        ByVal Fase_Cod As Integer,
                                                        ByVal Lotto As String,
                                                        ByVal Cod_Articolo As String,
                                                        ByVal Veg_Cod As Integer,
                                                        ByVal Cul_Cod As Integer,
                                                        ByVal Gen_Cod As Integer,
                                                        ByVal Spe_Cod As Integer,
                                                        ByVal Raz_Cod As Integer,
                                                        ByVal Ipro_Cod As Integer,
                                                        ByVal Cat_Cod As Integer,
                                                        ByVal FinestraTemp_Inizio As String,
                                                        ByVal FinestraTemp_Fine As String,
                                                        ByVal RicercaNomeProdotto As String,
                                                        ByVal RicercaLottoAccettazione As String,
                                                        ByVal RicercaCodArticolo As String,
                                                        ByVal Mat_Cod_Origine As Integer,
                                                        ByVal Piva_SuperUser_Origine As String,
                                                        ByVal Flag_AncheImportatati As Boolean,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Giacenze_R.Giacenze_Leggi_MateriePrime()"

        Dim MessaggioErrore As String = ""
        Dim SQL_Generale As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '------------------------------------------------------------------
            SQL_Generale.Length = 0

            '------------------------------------------------------ 
            '------------------- SELECT ---------------------------
            '------------------------------------------------------
            SQL_Generale.Append(" ")
            SQL_Generale.Append("SELECT Movimenti_dettagli.Pro_Cod, Movimenti_dettagli.Udm_Cod, Movimenti_dettagli.Prezzo_Unitario, Movimenti_dettagli.Cod_Progetto, Movimenti_dettagli.Cal_Cod,  " &
            " Movimenti_dettagli.Fase_Cod, Movimenti_dettagli.QTA_EXTRA, Movimenti_dettagli.UDM_COD_EXTRA, UnitaMisuraExtra.UDM_DES AS Udm_Des_Extra, UnitaMisuraExtra.UDM_SIM AS Udm_Sim_Extra, Movimenti_dettagli.Prezzo_Unitario_Netto, " &
            " Movimenti_dettagli.Imponibile_Netto, Movimenti_dettagli.Jolly_Int, Movimenti_dettagli.Lotto, Mov_Destinazioni.Qta AS Dest_Qta, " &
            "  Movimenti_dettagli.Qta AS Dett_Qta, Materie_Prime.*, CategorieMagazzino.NomeComune AS CategoriaMagazzino, UnitaMisuraStandard.UDM_DES AS Udm_Des_Standard, UnitaMisuraStandard.UDM_SIM AS Udm_Sim_Standard, Fabbricati.Fabbricato_Des, Fabbricati.Tipo_Fabbricato_Cod, Appezzamento.APP_NOME,")

            SQL_Generale.Append(
            "   Imprese_Progetti.Progetto_Nome, Imprese_Progetti.Progetto_Des, Materie_Prime_Campionature.Tipo, Materie_Prime_Campionature.Tipo_Cod,  " &
            " Materie_Prime_Campionature.Udm_Cod AS Udm_Cod_Camp, Materie_Prime_Campionature.Val_Cod, Materie_Prime_Campionature.Descrizione, SpecieVegetali.Veg_Des, Cultivar.Cul_Des, CalibriFrutti.CAL_DES, " &
            "  Lista_Generi_Animali.GEN_DES, Lista_Specie_Animali.SPE_DES, Lista_Razze_Animali.RAZ_DES, Lista_IndirizziProd_Animali.IPRO_DES,  " &
            " Lista_Categorie_Animali.CAT_DES, Lista_Categorie_Animali.SESSO, Lista_Categorie_Animali.ETA_GG_DA, Lista_Categorie_Animali.ETA_GG_A, " &
             "  Lista_Categorie_Animali.PESO_KG_DA, Lista_Categorie_Animali.PESO_KG_A, Lista_Categorie_Animali.COEFF_UBA, " &
             "  ISNULL(ACC_VetrinaProdotti.Id_Prodotto, 0) AS Id_Prodotto ")

            'Join con Movimenti
            SQL_Generale.Append(
                   " FROM Agenda INNER JOIN " &
                   "  Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda ")

            'Join con Movimenti_dettagli
            SQL_Generale.Append(
                   " INNER JOIN   Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND  " &
                   "  Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov ")

            'Join con Mov_Destinazioni
            SQL_Generale.Append(
                   "  INNER JOIN   Mov_Destinazioni ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod AND  " &
                   " Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND  " &
                   "  Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")

            'Join con Materie_Prime 'Movimenti_dettagli.PIVA = Materie_Prime.Piva AND
            SQL_Generale.Append(
                   " INNER JOIN  Materie_Prime ON   " &
                   "      Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod AND Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod  ")

            'Join con Materie_Prime_Campionature 
            SQL_Generale.Append(
                   "LEFT OUTER JOIN  Materie_Prime_Campionature ON Movimenti_dettagli.Cal_Cod = Materie_Prime_Campionature.Progressivo ")

            'Join con Imprese_Progetti 
            SQL_Generale.Append(
                   "  LEFT OUTER JOIN Imprese_Progetti ON Movimenti_dettagli.Cod_Progetto = Imprese_Progetti.Progetto_Cod  ")

            'Join con Lista_IndirizziProd_Animali 
            SQL_Generale.Append(
                   " LEFT OUTER JOIN  Lista_IndirizziProd_Animali ON Materie_Prime.GEN_COD = Lista_IndirizziProd_Animali.GEN_COD AND  " &
                   "  Materie_Prime.SPE_COD = Lista_IndirizziProd_Animali.SPE_COD AND  " &
                   " Materie_Prime.IPRO_COD = Lista_IndirizziProd_Animali.IPRO_COD  ")

            'Join con Lista_Categorie_Animali
            SQL_Generale.Append(
                   " LEFT OUTER JOIN Lista_Categorie_Animali ON Materie_Prime.GEN_COD = Lista_Categorie_Animali.GEN_COD AND  " &
                   "   Materie_Prime.SPE_COD = Lista_Categorie_Animali.SPE_COD AND Materie_Prime.IPRO_COD = Lista_Categorie_Animali.IPRO_COD AND " &
                   "  Materie_Prime.CAT_COD = Lista_Categorie_Animali.CAT_COD ")

            'Join con Lista_Generi_Animali 
            SQL_Generale.Append(
                   "  LEFT OUTER JOIN  Lista_Generi_Animali ON Materie_Prime.GEN_COD = Lista_Generi_Animali.GEN_COD ")

            'Join con Lista_Razze_Animali
            SQL_Generale.Append(
                   "  LEFT OUTER JOIN  Lista_Razze_Animali ON Materie_Prime.GEN_COD = Lista_Razze_Animali.GEN_COD AND  " &
                   "  Materie_Prime.SPE_COD = Lista_Razze_Animali.SPE_COD AND Materie_Prime.RAZ_COD = Lista_Razze_Animali.RAZ_COD ")

            'Join con Lista_Specie_Animali 
            SQL_Generale.Append(
                   "  LEFT OUTER JOIN Lista_Specie_Animali ON Materie_Prime.GEN_COD = Lista_Specie_Animali.GEN_COD AND  " &
                   "  Materie_Prime.SPE_COD = Lista_Specie_Animali.SPE_COD ")

            'Join con CalibriFrutti
            SQL_Generale.Append(
                   "  LEFT OUTER JOIN CalibriFrutti ON Movimenti_dettagli.Cal_Cod = CalibriFrutti.CAL_COD ")

            'Join con SpecieVegetali
            SQL_Generale.Append(
                   "  LEFT OUTER JOIN SpecieVegetali ON Materie_Prime.Veg_Cod = SpecieVegetali.Veg_Cod ")

            'Join con Cultivar
            SQL_Generale.Append(
                   "  LEFT OUTER JOIN Cultivar ON Materie_Prime.Cul_Cod = Cultivar.Cul_Cod ")

            'Join con UnitaMisura
            SQL_Generale.Append(
                   "  INNER JOIN   UnitaMisura UnitaMisuraStandard ON Movimenti_dettagli.Udm_Cod =  UnitaMisuraStandard.UDM_COD ")

            'Join con UnitaMisuraExtra
            SQL_Generale.Append(
                   "   LEFT OUTER JOIN  UnitaMisura UnitaMisuraExtra ON Movimenti_dettagli.UDM_COD_EXTRA = UnitaMisuraExtra.UDM_COD ")

            'Join con CategorieMagazzino
            SQL_Generale.Append(
                   " INNER JOIN  CategorieMagazzino ON Materie_Prime.Elem_Cod = CategorieMagazzino.Elem_Cod  ")

            'Join con Fabbricati
            SQL_Generale.Append(
                   " INNER JOIN   Fabbricati ON Mov_Destinazioni.Piva = Fabbricati.PIVA AND Mov_Destinazioni.Sa_Cod = Fabbricati.SA_COD AND  " &
                   " Mov_Destinazioni.Id_Destinazione = Fabbricati.Fabbricato_Cod ")

            'Join con Fabbricati
            SQL_Generale.Append(
                   "  LEFT OUTER JOIN   Appezzamento ON Imprese_Progetti.Piva = Appezzamento.PIVA AND Imprese_Progetti.Sa_Cod = Appezzamento.SA_COD AND   " &
                   "  Imprese_Progetti.Appezza = Appezzamento.APPEZZA ")

            'Join con ACC_VetrinaProdotti
            SQL_Generale.Append(
                   "   LEFT OUTER JOIN   ACC_VetrinaProdotti ON Movimenti_dettagli.PIVA = ACC_VetrinaProdotti.Piva AND Movimenti_dettagli.Elem_Cod = ACC_VetrinaProdotti.Elem_Cod AND    " &
                   "    Movimenti_dettagli.Mat_Cod = ACC_VetrinaProdotti.Mat_Cod AND Movimenti_dettagli.Cod_Progetto = ACC_VetrinaProdotti.Cod_Progetto AND    " &
                   "     Movimenti_dettagli.Lotto = ACC_VetrinaProdotti.Lotto AND Movimenti_dettagli.Cal_Cod = ACC_VetrinaProdotti.Cal_Cod AND    " &
                   "   Movimenti_dettagli.Udm_Cod = ACC_VetrinaProdotti.Udm_Cod AND    " &
                   " Movimenti_dettagli.UDM_COD_EXTRA = ACC_VetrinaProdotti.Udm_Cod_Extra  ")

            'WHERE
            SQL_Generale.Append(
                    " WHERE Movimenti_Dettagli.Validita_inizio <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " " &
                    " AND   Movimenti_Dettagli.Validita_Fine >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " " &
                    " AND   Agenda.Lav_Cod = -1    " &
                    " AND   Movimenti.Cau_Mov = 'GIACENZE'    " &
                    " AND   Mov_Destinazioni.Tipo_Destinazione = 20   " &
                    " AND   Materie_Prime.Mat_Cod_Origine = 0   ")

            If RicercaNomeProdotto <> "" Then
                SQL_Generale.Append(" AND Materie_Prime.Mat_Des like '%" & Agro_SQL_SaveText(RicercaNomeProdotto) & "%'   ")
            End If

            If RicercaLottoAccettazione <> "" Then
                SQL_Generale.Append(" AND Movimenti_Dettagli.Lotto like '%" & Agro_SQL_SaveText(RicercaLottoAccettazione) & "%'   ")
            End If

            If RicercaCodArticolo <> "" Then
                SQL_Generale.Append(" AND Materie_Prime.Cod_Articolo like '%" & Agro_SQL_SaveText(Cod_Articolo) & "%'   ")
            End If

            If Lotto <> LOTTO_NONDEFINITO Then
                SQL_Generale.Append(" AND Movimenti_Dettagli.Lotto = '" & Agro_SQL_SaveText(Lotto) & "'   ")
            End If

            If Cod_Articolo <> "" Then
                SQL_Generale.Append(" AND Materie_Prime.Cod_Articolo = '" & Agro_SQL_SaveText(Cod_Articolo) & "'   ")
            End If

            If Piva <> "" Then
                SQL_Generale.Append(" AND Mov_Destinazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                SQL_Generale.Append(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Elem_Cod <> 0 Then
                SQL_Generale.Append(" AND Movimenti_Dettagli.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
            End If

            If Pro_Cod <> 0 Then
                SQL_Generale.Append(" AND Movimenti_Dettagli.Pro_Cod = " & Agro_SQL_SaveNum(0) & "   ")
            End If

            If Mat_Cod <> 0 Then
                SQL_Generale.Append(" AND Movimenti_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
            End If

            If Udm_Cod <> 0 Then
                SQL_Generale.Append(" AND Movimenti_Dettagli.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   ")
            End If

            If Id_Destinazione <> 0 Then
                SQL_Generale.Append(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "   ")
            End If

            If Cal_Cod <> 0 Then
                SQL_Generale.Append(" AND Movimenti_Dettagli.Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "   ")
            End If


            If Cod_Progetto <> CODPROGETTO_NONDEFINITO Then
                SQL_Generale.Append(" AND Movimenti_Dettagli.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & "   ")
            End If

            If Fase_Cod <> 0 Then
                SQL_Generale.Append(" AND Movimenti_Dettagli.Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   ")
            End If

            If Veg_Cod <> 0 Then
                SQL_Generale.Append(" AND Materie_Prime.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "   ")
            End If

            If Cul_Cod <> 0 Then
                SQL_Generale.Append(" AND Materie_Prime.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & "   ")
            End If

            If Gen_Cod <> 0 Then
                SQL_Generale.Append(" AND Materie_Prime.Gen_Cod = " & Agro_SQL_SaveNum(Gen_Cod) & "   ")
            End If

            If Spe_Cod <> 0 Then
                SQL_Generale.Append(" AND Materie_Prime.Spe_Cod = " & Agro_SQL_SaveNum(Spe_Cod) & "   ")
            End If

            If Raz_Cod <> 0 Then
                SQL_Generale.Append(" AND Materie_Prime.Raz_Cod = " & Agro_SQL_SaveNum(Raz_Cod) & "   ")
            End If

            If Ipro_Cod <> 0 Then
                SQL_Generale.Append(" AND Materie_Prime.Ipro_Cod = " & Agro_SQL_SaveNum(Ipro_Cod) & "   ")
            End If

            If Cat_Cod <> 0 Then
                SQL_Generale.Append(" AND Materie_Prime.Cat_Cod = " & Agro_SQL_SaveNum(Cat_Cod) & "   ")
            End If

            If Mat_Cod_Origine <> 0 Then
                SQL_Generale.Append(" AND  Materie_Prime.Mat_Cod_Origine = " & Agro_SQL_SaveNum(Mat_Cod_Origine) & "   ")
            End If

            If Piva_SuperUser_Origine <> "" Then
                SQL_Generale.Append(" AND  Materie_Prime.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'")
            End If

            If Flag_AncheImportatati = False Then
                SQL_Generale.Append(" AND  Materie_Prime.Mat_Cod_Origine = " & Agro_SQL_SaveNum(0) & " ")
            End If


            If xFiltroAggiuntivo <> "" Then
                SQL_Generale.Append("AND " + Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    SQL_Generale.Append(" AND   Movimenti_Dettagli.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    SQL_Generale.Append(" AND   Movimenti_Dettagli.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                SQL_Generale.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                SQL_Generale.Append(" ORDER BY Agenda.PIVA, Movimenti.Data_Movimento DESC ")
            End If


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, SQL_Generale.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function



    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' chiama la funzione del core AgronicaStampeDAL che legge le giacenze di 
    ''' magazzino alla data passata fa un sum della qta dei movimenti fino alla data 
    ''' specificata
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    'default lotto=LOTTO_NONDEFINITO
    'se Flag_QtaNoZero = True  si filtrano le qta =0
    Public Function SchedaGiacenzeMagazzino(ByVal Data_Giacenza As Date,
                                            ByVal Piva As String,
                                            ByVal Sa_Cod As Integer,
                                            ByVal Id_Destinazione As Integer,
                                            ByVal Elem_Cod As Integer,
                                            ByVal Pro_Cod As Integer,
                                            ByVal Mat_Cod As Integer,
                                            ByVal Cal_Cod As Integer,
                                            ByVal Cod_Progetto As Integer,
                                            ByVal Fase_Cod As Integer,
                                            ByVal Udm_Cod As Integer,
                                            ByVal Lotto As String,
                                            ByVal Flag_QtaNoZero As Boolean,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xFiltroAggiuntivo_Coadiuvanti As String,
                                            ByVal xFiltroAggiuntivo_Carburanti As String,
                                            ByVal xFiltroAggiuntivo_Fertilizzanti As String,
                                            ByVal xFiltroAggiuntivo_Formulati As String,
                                            ByVal xFiltroAggiuntivo_InneschiTrappole As String,
                                            ByVal xFiltroAggiuntivo_InsettiUtili As String,
                                            ByVal xFiltroAggiuntivo_MateriePrime As String,
                                            ByVal xFiltroAggiuntivo_Trappole As String,
                                            ByVal xFiltroAggiuntivo_Semilavorati As String,
                                            ByVal xFiltroAggiuntivo_TrasformatiVeg As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            ByRef objParametriUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            Optional ByVal xFiltroAggiuntivo_13 As String = "",
                                            Optional ByRef StrQuery_Output As String = "",
                                            Optional ByVal isFreshAndFood As Boolean = False,
                                            Optional ByVal cercaLottoPerLike As Boolean = False,
                                            Optional ByVal xFiltroAggiuntivo_14 As String = "",
                                            Optional ByVal flagRecuperaCodArticolo As Boolean = False,
                                            Optional ByVal codArticolo As String = "",
                                            Optional ByVal cercaCodArticoloPerLike As Boolean = False,
                                            Optional ByVal Flag_QtaMaggioreZero As Boolean = False,
                                            Optional ByVal xFiltroAggiuntivo_15 As String = "",
                                            Optional ByVal gruppiMerceDefaultPerCategoria As List(Of ImpostazioneDefault_GruppiMerce) = Nothing,
                                            Optional ByVal inibisciVisibilitaGruppiMerce As Boolean = False,
                                            Optional ByVal creaParametriSql As Boolean = True,
                                            Optional ByVal xFiltroAggiuntivo_16 As String = "",
                                            Optional ByVal leggiLinea As Boolean = False,
                                            Optional ByVal filtroMagazziniEsterni As List(Of String) = Nothing,
                                            Optional ByVal calCodEsclusi As String = Nothing
                                            ) As DataTable

        Dim objStampe As New AgronicaCoreStampeDAL.Magazzino

        'aggiungere variabile xFiltroAggiuntivo_12 se occorre mandare anche il filtro su confezioni prodotti
        'aggiungere variabile xFiltroAggiuntivo_13 se occorre mandare anche il filtro su ore conto terzi

        Return objStampe.SchedaGiacenzeMagazzino(Data_Giacenza,
                                                 Piva,
                                                 Sa_Cod,
                                                 Id_Destinazione,
                                                 Elem_Cod,
                                                 Pro_Cod,
                                                 Mat_Cod,
                                                 Cal_Cod,
                                                 Cod_Progetto,
                                                 Fase_Cod,
                                                 Udm_Cod,
                                                 Lotto,
                                                 Flag_QtaNoZero,
                                                 xFiltroAggiuntivo,
                                                 xFiltroAggiuntivo_Coadiuvanti,
                                                 xFiltroAggiuntivo_Carburanti,
                                                 xFiltroAggiuntivo_Fertilizzanti,
                                                 xFiltroAggiuntivo_Formulati,
                                                 xFiltroAggiuntivo_InneschiTrappole,
                                                 xFiltroAggiuntivo_InsettiUtili,
                                                 xFiltroAggiuntivo_MateriePrime,
                                                 xFiltroAggiuntivo_Trappole,
                                                 xFiltroAggiuntivo_Semilavorati,
                                                 xFiltroAggiuntivo_TrasformatiVeg,
                                                 "",
                                                 xOrderBy,
                                                 objParametriServer,
                                                 objParametriUtenti,
                                                 xFiltroAggiuntivo_13,
                                                 StrQuery_Output,
                                                 isFreshAndFood,
                                                 cercaLottoPerLike,
                                                 xFiltroAggiuntivo_14,
                                                 flagRecuperaCodArticolo:=flagRecuperaCodArticolo,
                                                 codArticolo:=codArticolo,
                                                 cercaCodArticoloPerLike:=cercaCodArticoloPerLike,
                                                 Flag_QtaMaggioreZero:=Flag_QtaMaggioreZero,
                                                 xFiltroAggiuntivo_15:=xFiltroAggiuntivo_15,
                                                 gruppiMerceDefaultPerCategoria:=gruppiMerceDefaultPerCategoria,
                                                 inibisciVisibilitaGruppiMerce:=inibisciVisibilitaGruppiMerce,
                                                 creaParametriSql:=creaParametriSql,
                                                 xFiltroAggiuntivo_16:=xFiltroAggiuntivo_16,
                                                 leggiLinea:=leggiLinea,
                                                 filtroMagazziniEsterni:=filtroMagazziniEsterni,
                                                 calCodEsclusi:=calCodEsclusi)

    End Function

    '##############################################
    Public Function Verifica_Giacenza(ByVal Data_Giacenza As Date,
                                        ByVal Piva As String,
                                        ByVal Sa_Cod As Integer,
                                        ByVal Id_Destinazione As Integer,
                                        ByVal Elem_Cod As Integer,
                                        ByVal Pro_Cod As Integer,
                                        ByVal Mat_Cod As Integer,
                                        ByVal Cal_Cod As Integer,
                                        ByVal Cod_Progetto As Integer,
                                        ByVal Fase_Cod As Integer,
                                        ByVal Udm_Cod As Integer,
                                        ByVal Lotto As String,
                                        ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametriUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As Decimal

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Giacenze.Verifica_Giacenza()"
        Dim MessaggioErrore As String = ""

        Dim objStampe As New AgronicaCoreStampeDAL.Magazzino
        Dim DT As DataTable
        Dim Giacenza As Decimal = 0

        Try

            'Imposto questo filtro perché la funzione SchedaGiacenzeMagazzino in caso i parametri Cal_Cod e Cod_Progetto siano a zero non li filtra nella query
            'mentre in questo contesto sono valori significativi
            Dim xFiltroAggCalCodCodProgetto As String = "AND Movimenti_Dettagli.Cal_Cod = " & Cal_Cod & " AND Movimenti_Dettagli.Cod_Progetto = " & Cod_Progetto

            DT = objStampe.SchedaGiacenzeMagazzino(Data_Giacenza,
                                              Piva, Sa_Cod, Id_Destinazione,
                                              Elem_Cod, Pro_Cod, Mat_Cod,
                                              Cal_Cod, Cod_Progetto, Fase_Cod,
                                              Udm_Cod, Lotto,
                                              False,
                                              xFiltroAggCalCodCodProgetto, "",
                                                "", "", "",
                                                 "", "", "",
                                                "", "", "",
                                                "",
                                                "",
                                                   objParametriServer, objParametriUtenti)

            If Not IsNothing(DT) Then
                Select Case DT.Rows.Count
                    Case Is < 0
                        Throw New Exception("eccezione")
                    Case 0
                        'giacenza non trovata
                        Giacenza = 0
                    Case 1
                        Giacenza = CDec(DT.Rows(0).Item("Giacenza"))
                    Case Is > 1
                        Throw New Exception("Sono stati trovati più record, la chiave del prodotto inviata non è corretta.")
                End Select

            End If

        Catch ex As Exception
            Giacenza = 0
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Giacenza

    End Function


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' chiama la funzione del core AgronicaStampeDAL che legge le giacenze di 
    ''' magazzino alla data passata fa un sum della qta dei movimenti fino alla data 
    ''' specificata
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    'default lotto=LOTTO_NONDEFINITO
    'se Flag_QtaNoZero = True  si filtrano le qta =0
    Public Function GiacenzeMagazzino_SIGPA_Blocco(
                            ByVal SIGPA_ComandiEsportazione_cod As Integer,
                            ByVal PIVA_PadreInGerarchia As String,
                            ByVal Data_Giacenza As Date,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Integer,
                            ByVal Id_Destinazione As Integer,
                            ByVal Elem_Cod As Integer,
                            ByVal Pro_Cod As Integer,
                            ByVal Mat_Cod As Integer,
                            ByVal Cal_Cod As Integer,
                            ByVal Cod_Progetto As Integer,
                            ByVal Fase_Cod As Integer,
                            ByVal Udm_Cod As Integer,
                            ByVal Lotto As String,
                            ByVal Flag_QtaNoZero As Boolean,
                            ByVal Esporta_Elem_cod As Integer,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xFiltroAggiuntivo_1 As String,
                            ByVal xFiltroAggiuntivo_2 As String,
                            ByVal xFiltroAggiuntivo_3 As String,
                            ByVal xFiltroAggiuntivo_4 As String,
                            ByVal xFiltroAggiuntivo_5 As String,
                            ByVal xFiltroAggiuntivo_6 As String,
                            ByVal xFiltroAggiuntivo_7 As String,
                            ByVal xFiltroAggiuntivo_8 As String,
                            ByVal xFiltroAggiuntivo_9 As String,
                            ByVal xFiltroAggiuntivo_10 As String,
                            ByVal xFiltroAggiuntivo_12 As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As DataTable

        Dim objStampe As New AgronicaCoreStampeDAL.Magazzino

        Return objStampe.SchedaGiacenzeMagazzino_SIGPA_Blocco(SIGPA_ComandiEsportazione_cod, PIVA_PadreInGerarchia, Data_Giacenza, Piva, Sa_Cod, Id_Destinazione, Elem_Cod,
                                                 Pro_Cod, Mat_Cod, Cal_Cod, Cod_Progetto, Fase_Cod, Udm_Cod,
                                                 Lotto, Flag_QtaNoZero, Esporta_Elem_cod, xFiltroAggiuntivo, xFiltroAggiuntivo_1,
                                                 xFiltroAggiuntivo_2, xFiltroAggiuntivo_3, xFiltroAggiuntivo_4,
                                                 xFiltroAggiuntivo_5, xFiltroAggiuntivo_6, xFiltroAggiuntivo_7,
                                                 xFiltroAggiuntivo_8, xFiltroAggiuntivo_9, xFiltroAggiuntivo_10,
                                                 xFiltroAggiuntivo_12,
                                                 xOrderBy, objParametri)

    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' chiama la funzione del core AgronicaStampeDAL che legge le giacenze di 
    ''' magazzino alla data passata fa un sum della qta dei movimenti fino alla data 
    ''' specificata
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    'default lotto=LOTTO_NONDEFINITO
    'se Flag_QtaNoZero = True  si filtrano le qta =0
    Public Function GiacenzeMagazzino_SIGPA(
                    ByVal SIGPA_ComandiEsportazione_cod As Integer,
                    ByVal PIVA_PadreInGerarchia As String,
                    ByVal Data_Giacenza As Date,
                    ByVal Piva As String,
                    ByVal Sa_Cod As Integer,
                    ByVal Id_Destinazione As Integer,
                    ByVal Elem_Cod As Integer,
                    ByVal Pro_Cod As Integer,
                    ByVal Mat_Cod As Integer,
                    ByVal Cal_Cod As Integer,
                    ByVal Cod_Progetto As Integer,
                    ByVal Fase_Cod As Integer,
                    ByVal Udm_Cod As Integer,
                    ByVal Lotto As String,
                    ByVal Flag_QtaNoZero As Boolean,
                    ByVal Esportazione_elem_cod As Integer,
                    ByVal xFiltroAggiuntivo As String,
                    ByVal xFiltroAggiuntivo_1 As String,
                    ByVal xFiltroAggiuntivo_2 As String,
                    ByVal xFiltroAggiuntivo_3 As String,
                    ByVal xFiltroAggiuntivo_4 As String,
                    ByVal xFiltroAggiuntivo_5 As String,
                    ByVal xFiltroAggiuntivo_6 As String,
                    ByVal xFiltroAggiuntivo_7 As String,
                    ByVal xFiltroAggiuntivo_8 As String,
                    ByVal xFiltroAggiuntivo_9 As String,
                    ByVal xFiltroAggiuntivo_10 As String,
                    ByVal xFiltroAggiuntivo_12 As String,
                    ByVal xOrderBy As String,
                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As DataTable

        Dim objStampe As New AgronicaCoreStampeDAL.Magazzino

        Return objStampe.SchedaGiacenzeMagazzino_SIGPA(SIGPA_ComandiEsportazione_cod, PIVA_PadreInGerarchia, Data_Giacenza, Piva, Sa_Cod, Id_Destinazione, Elem_Cod,
                                                 Pro_Cod, Mat_Cod, Cal_Cod, Cod_Progetto, Fase_Cod, Udm_Cod,
                                                 Lotto, Flag_QtaNoZero, Esportazione_elem_cod, xFiltroAggiuntivo, xFiltroAggiuntivo_1,
                                                 xFiltroAggiuntivo_2, xFiltroAggiuntivo_3, xFiltroAggiuntivo_4,
                                                 xFiltroAggiuntivo_5, xFiltroAggiuntivo_6, xFiltroAggiuntivo_7,
                                                 xFiltroAggiuntivo_8, xFiltroAggiuntivo_9, xFiltroAggiuntivo_10,
                                                 xFiltroAggiuntivo_12,
                                                 xOrderBy, objParametri)

    End Function



    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' legge le giacenze di magazzino alla data odierna 
    ''' legge il record statico della giacenza (sperando che la qta sia salvata bene, se non lo è bisogna lanciare il verificatore)
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    'default lotto=LOTTO_NONDEFINITO
    'se Flag_QtaNoZero = True  si filtrano le qta =0
    Public Function SchedaGiacenzeMagazzinoRecordStatico(ByVal Piva As String,
                                                        ByVal Sa_Cod As Integer,
                                                        ByVal Id_Destinazione As Integer,
                                                        ByVal Tipo_Scorta As Integer,
                                                        ByVal Scorta_Min As Decimal,
                                                        ByVal Elem_Cod As Integer,
                                                        ByVal Pro_Cod As Integer,
                                                        ByVal Mat_Cod As Integer,
                                                        ByVal Cal_Cod As Integer,
                                                        ByVal Cod_Progetto As Integer,
                                                        ByVal Fase_Cod As Integer,
                                                        ByVal Udm_Cod As Integer,
                                                        ByVal Lotto As String,
                                                        ByVal Flag_QtaNoZero As Boolean,
                                                        ByVal xFiltroAggiuntivo As String,
                                                        ByVal xFiltroAggiuntivo_1 As String,
                                                        ByVal xFiltroAggiuntivo_2 As String,
                                                        ByVal xFiltroAggiuntivo_3 As String,
                                                        ByVal xFiltroAggiuntivo_4 As String,
                                                        ByVal xFiltroAggiuntivo_5 As String,
                                                        ByVal xFiltroAggiuntivo_6 As String,
                                                        ByVal xFiltroAggiuntivo_7 As String,
                                                        ByVal xFiltroAggiuntivo_8 As String,
                                                        ByVal xFiltroAggiuntivo_9 As String,
                                                        ByVal xFiltroAggiuntivo_10 As String,
                                                       ByVal xOrderBy As String,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                        ) As DataTable


        Dim objStampe As New AgronicaCoreStampeDAL.Magazzino

        'aggiungere variabile xFiltroAggiuntivo_12 se occorre mandare anche il filtro su confezioni prodotti
        Return objStampe.SchedaGiacenzeMagazzinoRecordStatico(Piva, Sa_Cod, Id_Destinazione,
                                                                Tipo_Scorta, Scorta_Min,
                                                                Elem_Cod,
                                                                Pro_Cod, Mat_Cod, Cal_Cod, Cod_Progetto, Fase_Cod, Udm_Cod,
                                                                Lotto, Flag_QtaNoZero, xFiltroAggiuntivo, xFiltroAggiuntivo_1,
                                                                xFiltroAggiuntivo_2, xFiltroAggiuntivo_3, xFiltroAggiuntivo_4,
                                                                xFiltroAggiuntivo_5, xFiltroAggiuntivo_6, xFiltroAggiuntivo_7,
                                                                xFiltroAggiuntivo_8, xFiltroAggiuntivo_9, xFiltroAggiuntivo_10,
                                                                "", xOrderBy, objParametri)


    End Function


    '###############################################################################
    Public Function Verifica_ScarichiProdotto(ByRef Des_Lib As String,
                                                ByRef Data_Movimento As String,
                                                ByVal strFiltro_Semilavorati As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim Dt_Mov As DataTable

        strFiltro_Semilavorati += "AND (Cau_Mov IN ('" + CAU_SCARICO + "', '" + CAU_CONFERIMENTO_DIVERSI + "'))"


        Dt_Mov = New AgronicaCoreContabDAL.Movimenti_Dettagli_R().MovimentiDettagli_Leggi("", 0,
                                                            0, 0, 0, 0, 0, 0, 0, 0, LOTTO_NONDEFINITO, 0, 0, 0, "", False, strFiltro_Semilavorati, "", objParametri)



        If Not IsNothing(Dt_Mov) AndAlso Dt_Mov.Rows.Count > 0 Then
            Des_Lib = Dt_Mov.Rows(0).Item("Des_Lib")
            Data_Movimento = Dt_Mov.Rows(0).Item("Data_Movimento")
            Return True
        Else
            Return False
        End If


    End Function

    '###############################################################################
    Public Function Calcola_ValorizzazioneProdotti(ByVal Impostazione As Integer,
                                                   ByVal Piva As String, ByVal Data As Date,
                                                   ByVal Elem_Cod As Integer, ByVal Pro_Cod As Integer,
                                                   ByVal Mat_Cod As Integer, ByVal Udm_Cod As Integer,
                                                   ByVal Lotto As String,
                                                   ByRef Valore_Medio_Ponderato As Dictionary(Of String, Decimal),
                                                   ByRef Valore_Anagrafica_Prodotti As Dictionary(Of String, Decimal),
                                                   ByRef Valore_Ultimo_Prodotto As Dictionary(Of String, Decimal),
                                                   ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                   ByRef objParametriUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As String()

        'TIPO_VALORIZZAZIONE (Impostazione_Cod = 846)
        '1 = COSTO MEDIA PONDERATA
        '2 = RICAVO MEDIO PONDERATO
        '3 = Costo / Ricavo da tabella Prodotti_Costi (non gestita qui)
        '4 = ULTIMO COSTO
        '5 = ULTIMO RICAVO

        Dim Lav_Cod_Costi = "1000, 1002, 1021, 1022, 1025, 1054, 1056, 1057, 1075, 1076, 1077, 1078, 3001"
        Dim Lav_Cod_Ricavi = "1001, 1003, 1020, 1023, 1031, 1053, 1052, 1055, 1058, 1028, 1069"
        Dim filtroProdotti = "Elem_Cod <> 0 And Elem_Cod <> 1"
        Dim filtroProdotti_Costi = "Prodotti_Costi.Elem_Cod <> 0 And Prodotti_Costi.Elem_Cod <> 1"
        Dim bGestioneLotto As Boolean = False

        Dim Tipo_Valorizzazione As String() = {}
        Dim objImpostazioni As New Utenti_Impostazioni_Read
        Dim dtImpostazioni = objImpostazioni.Leggi2(2, objParametriServer.SuperUserUsername, Impostazione, "", "", objParametriUtenti)

        If Not IsNothing(dtImpostazioni) AndAlso dtImpostazioni.Rows.Count > 0 Then
            Dim Impostazione_Valore = dtImpostazioni.Rows(0).Item("Impostazione_Valore_1")
            If Not String.IsNullOrEmpty(Impostazione_Valore) Then
                Tipo_Valorizzazione = Impostazione_Valore.Split(",")
            End If
        End If

        'Definizione Lotto (nota: nella gestione lotto viene inserito lo 0 in coda)
        If Lotto = "***Bypass***" And
            (Tipo_Valorizzazione.Contains("10") OrElse Tipo_Valorizzazione.Contains("20") Or
           Tipo_Valorizzazione.Contains("40") OrElse Tipo_Valorizzazione.Contains("50")) Then
            Lotto = "***CDG***"
        End If


        ' valore medio ponderato
        If Tipo_Valorizzazione.Contains("1") OrElse Tipo_Valorizzazione.Contains("2") Or
           Tipo_Valorizzazione.Contains("10") OrElse Tipo_Valorizzazione.Contains("20") Then
            'Dim DataInizio = If(Data = AGRODATAFINE, AGRODATAINIZIO, New Date(Data.Year, 1, 1))
            Dim filtro = filtroProdotti & " AND Agenda.Lav_Cod IN (" & If(Tipo_Valorizzazione.Contains("1") Or Tipo_Valorizzazione.Contains("10"), Lav_Cod_Costi, Lav_Cod_Ricavi) & ") "
            Dim dt_Valore_Medio_Ponderato = Leggi_ValoreMedioPonderato(Piva, Elem_Cod, Pro_Cod, Mat_Cod, Udm_Cod, AGRODATAINIZIO, Data, filtro, objParametriServer, Lotto)
            For Each row In dt_Valore_Medio_Ponderato.Rows
                Dim chiave = row.Item("Piva") & "_" & row.Item("Elem_Cod") & "_" & row.Item("Pro_Cod") & "_" & row.Item("Mat_Cod") & "_" & row.Item("Udm_Cod")

                If Tipo_Valorizzazione.Contains("10") OrElse Tipo_Valorizzazione.Contains("20") Then
                    chiave = chiave & "_" & row.Item("Lotto")
                End If

                If Not Valore_Medio_Ponderato.ContainsKey(chiave) Then
                    Valore_Medio_Ponderato(chiave) = Format(row.Item("Costo_Medio_Ponderato"), "###,###,##0.0#####")
                End If
            Next
        End If

        ' anagrafica prodotti valore
        If Tipo_Valorizzazione.Contains("3") Then
            Dim filtro = filtroProdotti_Costi
            Dim dt_Valore_Anagrafica_Prodotti = Leggi_ValoreAnagraficaProdotti(Piva, Elem_Cod, Pro_Cod, Mat_Cod, Udm_Cod, AGRODATAINIZIO, Data, filtro, 0, objParametriServer)
            ' Rimuovo gli elementi con validità che non comprende data giacenza
            For index As Integer = 0 To dt_Valore_Anagrafica_Prodotti.Rows.Count - 1
                Dim row = dt_Valore_Anagrafica_Prodotti.Rows(index)
                If row.Item("Validita_Inizio") > Data OrElse
                   row.Item("Validita_Fine") < Data Then
                    row.Delete()
                End If
            Next
            dt_Valore_Anagrafica_Prodotti.AcceptChanges()
            'Eseguo un primo ciclo per inserire gli elementi che hanno la stessa PIVA
            For Each row In dt_Valore_Anagrafica_Prodotti.Rows
                If row.Item("Piva") = Piva Then
                    Dim chiave = row.Item("Piva") & "_" & row.Item("Elem_Cod") & "_" & row.Item("Pro_Cod") & "_" & row.Item("Mat_Cod") & "_" & row.Item("Udm_Cod")
                    If Not Valore_Anagrafica_Prodotti.ContainsKey(chiave) Then
                        Valore_Anagrafica_Prodotti(chiave) = Format(row.Item("Prezzo_Unitario"), "###,###,##0.0#####")
                    End If
                End If
            Next
            'Eseguo un secondo ciclo per inserire gli elementi che hanno PIVA diversa (prodotti pubblici)
            'Forzo la piva corrente al posto della PIVA dell'azienda che possiede il prodotto: se esisteva già un record specifico per quella PIVA l'ho inserita al passo sopra
            'Diversamente viene inserito in questa fase
            For Each row In dt_Valore_Anagrafica_Prodotti.Rows
                If row.Item("Piva") <> Piva Then
                    Dim chiave = Piva & "_" & row.Item("Elem_Cod") & "_" & row.Item("Pro_Cod") & "_" & row.Item("Mat_Cod") & "_" & row.Item("Udm_Cod")
                    If Not Valore_Anagrafica_Prodotti.ContainsKey(chiave) Then
                        Valore_Anagrafica_Prodotti(chiave) = Format(row.Item("Prezzo_Unitario"), "###,###,##0.0#####")
                    End If
                End If
            Next
        End If

        ' valore ultimo prodotto
        If Tipo_Valorizzazione.Contains("4") OrElse Tipo_Valorizzazione.Contains("5") Or
           Tipo_Valorizzazione.Contains("40") OrElse Tipo_Valorizzazione.Contains("50") Then
            Dim filtro = filtroProdotti & " AND Agenda.Lav_Cod IN (" & If(Tipo_Valorizzazione.Contains("4") Or Tipo_Valorizzazione.Contains("40"), Lav_Cod_Costi, Lav_Cod_Ricavi) & ") "
            Dim dt_Valore_Ultimo_Prodotto = Leggi_ValoreUltimoProdotto(Piva, Elem_Cod, Pro_Cod, Mat_Cod, Udm_Cod, AGRODATAINIZIO, Data, filtro, objParametriServer, Lotto)
            For Each row In dt_Valore_Ultimo_Prodotto.Rows
                Dim chiave = row.Item("Piva") & "_" & row.Item("Elem_Cod") & "_" & row.Item("Pro_Cod") & "_" & row.Item("Mat_Cod") & "_" & row.Item("Udm_Cod")

                If Tipo_Valorizzazione.Contains("40") OrElse Tipo_Valorizzazione.Contains("50") Then
                    chiave = chiave & "_" & row.Item("Lotto")
                End If

                If Not Valore_Ultimo_Prodotto.ContainsKey(chiave) Then
                    Valore_Ultimo_Prodotto(chiave) = Format(row.Item("Costo_Ultimo_Prodotto"), "###,###,##0.0#####")
                End If
            Next
        End If

        Return Tipo_Valorizzazione

    End Function

    '###############################################################################
    Public Function Leggi_ValoreMedioPonderato(ByVal PIVA As String,
                                                ByVal Elem_Cod As Integer,
                                                ByVal Pro_Cod As Integer,
                                                ByVal Mat_Cod As Integer,
                                                ByVal Udm_Cod As Integer,
                                                ByVal Dal_Data_Verifica As Date,
                                                ByVal Al_Data_Verifica As Date,
                                                ByVal strFiltro As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                Optional ByVal Lotto As String = "***Bypass***"
                                                ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Giacenze.Leggi_ValoreMedioPonderato()"

        Dim messaggioErrore As String = ""
        Dim listaCampi As String = ""
        Dim filtroProdotto As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            listaCampi = "Mov_Destinazioni.Piva,Movimenti_Dettagli.Elem_Cod,Movimenti_Dettagli.Pro_Cod,Movimenti_Dettagli.Mat_Cod,Movimenti_Dettagli.Udm_Cod"
            listaCampi &= If(Trim(Lotto) <> "***Bypass***", ",Movimenti_Dettagli.Lotto", "")

            strSql.AppendLine(" SELECT " & listaCampi)
            strSql.AppendLine(" ,MAX(Movimenti.Data_Movimento) AS Data_Ultimo_Movimento ")
            strSql.AppendLine(" ,SUM(Abs(IsNull(Movimenti_Dettagli.Imponibile_Netto,0))) as Delta, SUM(IsNull(Movimenti_Dettagli.Qta ,0)) as Qta_Complessiva ")
            strSql.AppendLine(" ,SUM(Abs(IsNull(Movimenti_Dettagli.Imponibile_Netto,0))) / SUM(IsNull(Movimenti_Dettagli.Qta ,0)) as Costo_Medio_Ponderato ")
            strSql.AppendLine(" FROM Agenda, Movimenti, Movimenti_Dettagli, Mov_Destinazioni ")

            ' esclude movimenti collegati con lav_cod 1000 e 1001 (fatture)
            strSql.AppendLine(" WHERE  Movimenti_Dettagli.Id_Mov_Det NOT IN ")
            strSql.AppendLine("        (Select Distinct Id_Mov_Det_Rif From Mov_Dettagli_Riferimenti ")
            strSql.AppendLine("         Where Piva = '" & Agro_SQL_SaveText(PIVA) & "' And Lav_Cod In (1000, 1001) )")

            'Condizioni Di Join
            strSql.AppendLine(" AND Agenda.Piva = Movimenti.Piva ")
            strSql.AppendLine(" AND Movimenti.Piva = Movimenti_Dettagli.Piva ")
            strSql.AppendLine(" AND Movimenti_Dettagli.Piva = Mov_Destinazioni.Piva ")
            strSql.AppendLine(" AND Agenda.Id_Agenda = Movimenti.Id_Agenda ")
            strSql.AppendLine(" AND Movimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda ")
            strSql.AppendLine(" AND Movimenti_Dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda ")
            strSql.AppendLine(" AND Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov ")
            strSql.AppendLine(" AND Movimenti_Dettagli.Id_Mov = Mov_Destinazioni.Id_Mov ")
            strSql.AppendLine(" AND Movimenti_Dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")

            'filtro sul prodotto
            filtroProdotto = " AND Mov_Destinazioni.Piva = '" & Agro_SQL_SaveText(PIVA) & "' "
            filtroProdotto &= If(Elem_Cod <> 0, " AND Movimenti_Dettagli.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & " ", "")
            filtroProdotto &= If(Pro_Cod <> 0, " AND Movimenti_Dettagli.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & " ", "")
            filtroProdotto &= If(Mat_Cod <> 0, " AND Movimenti_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & " ", "")
            filtroProdotto &= If(Udm_Cod <> 0, " AND Movimenti_Dettagli.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & " ", "")
            strSql.AppendLine(filtroProdotto)

            'Nota Marco: per preservare il pregresso viene aggiunta la chiave "***CDG***" per bypassare il filtro ma far tornare il valore in select list
            If Trim(Lotto) <> "***Bypass***" And Trim(Lotto) <> "***CDG***" Then
                If Trim(Lotto) = "" Then
                    strSql.AppendLine(" AND Upper(Movimenti_Dettagli.Lotto) In ('INDEFINITO', '" & UCase(Agro_SQL_SaveText(Lotto)) & "')   ")
                Else
                    strSql.AppendLine(" AND Upper(Movimenti_Dettagli.Lotto) = '" & UCase(Agro_SQL_SaveText(Lotto)) & "' ")
                End If
            End If

            If Trim(strFiltro) <> "" Then
                strSql.AppendLine(" AND (" & Agro_SQL_Save_xFiltroAggiuntivo(strFiltro,, objParametri) & ") ")
            End If

            strSql.AppendLine(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Dal_Data_Verifica) & " ")
            strSql.AppendLine(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Al_Data_Verifica) & " ")

            'La condizione necessaria è che il dettaglio abbia una imputazione economica e una quantità valida
            strSql.AppendLine(" AND Abs(Movimenti_Dettagli.Imponibile_Netto) > 0 AND Movimenti_Dettagli.Qta > 0 ")

            strSql.AppendLine(" GROUP BY " & listaCampi)

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("" & nomeRoutine & " : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '###############################################################################
    Public Function Leggi_ValoreUltimoProdotto(ByVal PIVA As String,
                                                ByVal Elem_Cod As Integer,
                                                ByVal Pro_Cod As Integer,
                                                ByVal Mat_Cod As Integer,
                                                ByVal Udm_Cod As Integer,
                                                ByVal Dal_Data_Verifica As Date,
                                                ByVal Al_Data_Verifica As Date,
                                                ByVal strFiltro As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                Optional ByVal Lotto As String = "***Bypass***"
                                                ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Giacenze.Leggi_ValoreUltimoProdotto()"

        Dim messaggioErrore As String = ""
        Dim filtroProdotto As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.AppendLine(" SELECT Mov_Destinazioni.Piva,Movimenti_Dettagli.Elem_Cod,Movimenti_Dettagli.Pro_Cod,Movimenti_Dettagli.Mat_Cod,Movimenti_Dettagli.Udm_Cod, Movimenti.Data_Movimento ")
            'strSql.AppendLine(" ,Abs(IsNull(Movimenti_Dettagli.Imponibile_Netto,0)) as Delta, IsNull(Movimenti_Dettagli.Qta ,0) as Qta_Complessiva ")
            strSql.AppendLine(" ,Abs(IsNull(Movimenti_Dettagli.Imponibile_Netto,0)) / IsNull(Movimenti_Dettagli.Qta ,0) AS Costo_Ultimo_Prodotto ")

            If Trim(Lotto) <> "***Bypass***" Then
                strSql.AppendLine(" , Movimenti_Dettagli.Lotto ")
            End If

            strSql.AppendLine(" FROM Agenda, Movimenti, Movimenti_Dettagli, Mov_Destinazioni ")

            'Condizioni Di Join
            strSql.AppendLine(" WHERE Agenda.Piva = Movimenti.Piva ")
            strSql.AppendLine(" AND Movimenti.Piva = Movimenti_Dettagli.Piva ")
            strSql.AppendLine(" AND Movimenti_Dettagli.Piva = Mov_Destinazioni.Piva ")
            strSql.AppendLine(" AND Agenda.Id_Agenda = Movimenti.Id_Agenda ")
            strSql.AppendLine(" AND Movimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda ")
            strSql.AppendLine(" AND Movimenti_Dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda ")
            strSql.AppendLine(" AND Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov ")
            strSql.AppendLine(" AND Movimenti_Dettagli.Id_Mov = Mov_Destinazioni.Id_Mov ")
            strSql.AppendLine(" AND Movimenti_Dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")

            'filtro sul prodotto
            filtroProdotto = " AND Mov_Destinazioni.Piva = '" & Agro_SQL_SaveText(PIVA) & "' "
            filtroProdotto &= If(Elem_Cod <> 0, " AND Movimenti_Dettagli.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & " ", "")
            filtroProdotto &= If(Pro_Cod <> 0, " AND Movimenti_Dettagli.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & " ", "")
            filtroProdotto &= If(Mat_Cod <> 0, " AND Movimenti_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & " ", "")
            filtroProdotto &= If(Udm_Cod <> 0, " AND Movimenti_Dettagli.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & " ", "")
            strSql.AppendLine(filtroProdotto)

            'Nota Marco: per preservare il pregresso viene aggiunta la chiave "***CDG***" per bypassare il filtro ma far tornare il valore in select list
            If Trim(Lotto) <> "***Bypass***" And Trim(Lotto) <> "***CDG***" Then
                If Trim(Lotto) = "" Then
                    strSql.AppendLine(" AND Upper(Movimenti_Dettagli.Lotto) In ('INDEFINITO', '" & UCase(Agro_SQL_SaveText(Lotto)) & "')   ")
                Else
                    strSql.AppendLine(" AND Upper(Movimenti_Dettagli.Lotto) = '" & UCase(Agro_SQL_SaveText(Lotto)) & "' ")
                End If
            End If

            If Trim(strFiltro) <> "" Then
                strSql.AppendLine(" AND (" & Agro_SQL_Save_xFiltroAggiuntivo(strFiltro,, objParametri) & ") ")
            End If

            strSql.AppendLine(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Dal_Data_Verifica) & " ")
            strSql.AppendLine(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Al_Data_Verifica) & " ")

            'La condizione necessaria è che il dettaglio abbia una imputazione economica e una quantità valida
            strSql.AppendLine(" AND Abs(Movimenti_Dettagli.Imponibile_Netto) > 0 AND Movimenti_Dettagli.Qta > 0 ")

            strSql.AppendLine(" ORDER BY Mov_Destinazioni.Piva,Movimenti_Dettagli.Elem_Cod,Movimenti_Dettagli.Pro_Cod,Movimenti_Dettagli.Mat_Cod,Movimenti_Dettagli.Udm_Cod, Movimenti.Data_Movimento DESC ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("" & nomeRoutine & " : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '###############################################################################
    Public Function Leggi_ValoreAnagraficaProdotti(ByVal PIVA As String,
                                                    ByVal Elem_Cod As Long,
                                                    ByVal Pro_Cod As Long,
                                                    ByVal Mat_Cod As Long,
                                                    ByVal Udm_Cod As Long,
                                                    ByVal FinestraTemp_Inizio As Date,
                                                    ByVal FinestraTemp_Fine As Date,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal budget As Integer,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                    ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Giacenze.Calcola_ValoreAnagraficaProdotti()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim strJoin As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.AppendLine(" SELECT Prodotti_Costi.* ")
            strSql.AppendLine(" FROM   Prodotti_Costi ")
            strSql.AppendLine(" Left join materie_prime on Prodotti_Costi.elem_cod = materie_prime.elem_cod And Prodotti_Costi.mat_cod = materie_prime.mat_cod ")
            strSql.AppendLine(" WHERE Prodotti_Costi.Validita_inizio <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " ")
            strSql.AppendLine(" AND Prodotti_Costi.Validita_Fine >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")
            strSql.AppendLine(" AND  ")
            strSql.AppendLine(" (")
            strSql.AppendLine(" Prodotti_Costi.Piva = '" & Agro_SQL_SaveText(PIVA) & "'  ")
            strSql.AppendLine(" or ISNULL(materie_prime.sa_cod, 0) = -1) ")
            strSql.AppendLine(" AND Abs(Prodotti_Costi.Prezzo_Unitario) > 0 And Prodotti_Costi.Id_Budget = " & budget & " ")

            If Elem_Cod <> 0 Then
                strSql.AppendLine(" AND Prodotti_Costi.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & " ")
            End If

            If Pro_Cod <> 0 Then
                strSql.AppendLine(" AND Prodotti_Costi.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
            End If

            If Mat_Cod <> 0 Then
                strSql.AppendLine("  AND Prodotti_Costi.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
            End If

            If Udm_Cod <> 0 Then
                strSql.AppendLine(" AND Prodotti_Costi.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND  " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            strSql.AppendLine(" ORDER BY Prodotti_Costi.Elem_Cod, Prodotti_Costi.Pro_Cod, Prodotti_Costi.Mat_Cod, Prodotti_Costi.Udm_Cod ASC ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("" & nomeRoutine & " : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function LeggiSpecieMagazzino_QdC(piva As String,
                                             sa_cod As Integer,
                                             validitaFine As DateTime,
                                             elem_cod_list As List(Of Integer),
                                             objParametri_Server As AgronicaCoreParametri,
                                             objParametri_Utenti As AgronicaCoreParametri,
                                             Optional Flag_QtaNoZero As Boolean = True
                                             ) As List(Of utilizzi.UtilizzoTerreno)

        Dim UtilizzoTerrenoList As New List(Of utilizzi.UtilizzoTerreno)

        Dim Filtro As String = ""

        If validitaFine <> AGRODATAINIZIO Then
            Filtro = " AND (Fabbricati.Validita_Inizio <= " & Agro_SQL_SaveDate(validitaFine) & " AND Fabbricati.Validita_Fine >= " & Agro_SQL_SaveDate(validitaFine) & ")"
        End If

        If sa_cod <> 0 Then
            Filtro &= " AND Fabbricati.Sa_Cod = " & Agro_SQL_SaveNum(sa_cod) & " "
        End If

        Dim objG As New AgronicaCoreStampeDAL.Magazzino
        Dim listaSpecie As New List(Of Integer)

        For Each elem_cod In elem_cod_list
            Dim DtRisultati As DataTable = objG.SchedaGiacenzeMagazzino(validitaFine,
                                                                        piva, 0, 0,
                                                                        elem_cod,
                                                                        0,
                                                                        0,
                                                                        0, 0, 0, 0,
                                                                        LOTTO_NONDEFINITO,
                                                                        Flag_QtaNoZero, 'DT: ex escludiGiacenzeZero
                                                                        Filtro,
                                                                        "",
                                                                        "", "",
                                                                        "", "",
                                                                        "", "",
                                                                        "", "",
                                                                        "", "",
                                                                        "",
                                                                        objParametri_Server, objParametri_Utenti, "",
                                                                        Flag_QtaMaggioreZero:=True)

            If DtRisultati IsNot Nothing Then
                listaSpecie.AddRange((From row In DtRisultati.AsEnumerable()
                                      Select row.Field(Of Integer)("Veg_cod")).ToList())
            End If
        Next

        Dim xFiltroAggiuntivo As String = ""
        If listaSpecie IsNot Nothing AndAlso listaSpecie.Count > 0 Then
            xFiltroAggiuntivo &= " SpecieVegetali.Veg_Cod in (-99999, " & String.Join(", ", listaSpecie) & ")"

            Dim objSpecVeg As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R()
            Dim dtSpecieConVisibilita = objSpecVeg.SpecieVegetali_GestioneFiltroUtente_Leggi(0, 0,
                                                                          "",
                                                                          "",
                                                                          xFiltroAggiuntivo,
                                                                          "",
                                                                          objParametri_Utenti)

            If dtSpecieConVisibilita IsNot Nothing AndAlso dtSpecieConVisibilita.Rows.Count > 0 Then
                For Each row In dtSpecieConVisibilita.Rows
                    Dim varieta As New utilizzi.Varieta(0) With {
                        .specie = New utilizzi.Specie(row.Item("Veg_Cod")) With {
                            .descrizione = row.Item("Veg_Des")
                        }
                     }
                    UtilizzoTerrenoList.Add(varieta)
                Next
            End If
        End If

        Return UtilizzoTerrenoList

    End Function

End Class


'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
