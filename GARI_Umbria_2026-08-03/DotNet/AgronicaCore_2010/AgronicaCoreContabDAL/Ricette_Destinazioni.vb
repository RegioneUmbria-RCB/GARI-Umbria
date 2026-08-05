Imports System.Data.OleDb
Imports System.Text
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.ListExtensions
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreDataProvider

Public Class Ricette_Destinazioni_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal Ricetta_Cod As Int32,
                          ByVal Ricetta_Operazione_Cod As Int32,
                          ByVal Ricetta_Dettaglio_Cod As Int32,
                          ByVal Ricetta_Destinazione_Cod As Int32,
                          ByVal Programmazione_Entita_Cod As Int32,
                          ByVal Piva As String,
                          ByVal Sa_Cod As Int32,
                          ByVal Appezza As Int32,
                          ByVal Id_Reg As Int32,
                          ByVal Validita_Inizio As Date,
                          ByVal Validita_Fine As Date,
                          ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            Optional ByVal joinRicette As Boolean = False,
                            Optional joinDescrizioneImpianto As Boolean = False
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Destinazioni_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Ricetta_Cod = 0
        '   Ricetta_Operazione_Cod = 0
        '   Ricetta_Dettaglio_Cod = 0
        '   Ricetta_Destinazione_Cod = 0
        '   Programmazione_Entita_Cod = 0
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    '------------------------------------------------------------------
                    StrSQL.Length = 0

                    StrSQL.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")

                    StrSQL.AppendLine(" SELECT Ricette_Destinazioni.Ricetta_Cod, Ricette_Destinazioni.Ricetta_Operazione_Cod, Ricetta_Dettaglio_Cod, Ricetta_Destinazione_Cod, ")
                    StrSQL.AppendLine(" Programmazione_Entita_Cod, Ricette_Destinazioni.Piva, Ricette_Destinazioni.Sa_Cod, Ricette_Destinazioni.Appezza, Ricette_Destinazioni.Id_Reg, Qta, Ricette_Destinazioni.Validita_Inizio, Ricette_Destinazioni.Validita_Fine, Qta2, Tipo_Destinazione ")


                    StrSQL.AppendLine(" FROM  Ricette_Destinazioni ")
                    If joinRicette Then
                        StrSQL.AppendLine(" INNER JOIN  Ricette on Ricette.Ricetta_Cod = Ricette_Destinazioni.Ricetta_COD ")
                        StrSQL.AppendLine(" INNER JOIN  Ricette_Operazioni on Ricette_Operazioni.Ricetta_Operazione_Cod = Ricette_Destinazioni.Ricetta_Operazione_COD ")
                    End If

                    StrSQL.AppendLine(" WHERE Ricette_Destinazioni.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    StrSQL.AppendLine(" AND   Ricette_Destinazioni.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

                    If objParametri.PivaSuperUser <> "" Then
                        StrSQL.AppendLine(" AND Ricette_Destinazioni.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
                    End If

                    If Ricetta_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Destinazioni.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
                    End If

                    If Ricetta_Operazione_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Destinazioni.Ricetta_Operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & "   ")
                    End If

                    If Ricetta_Dettaglio_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Destinazioni.Ricetta_Dettaglio_Cod = " & Agro_SQL_SaveNum(Ricetta_Dettaglio_Cod) & "   ")
                    End If

                    If Ricetta_Destinazione_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Destinazioni.Ricetta_Destinazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Destinazione_Cod) & "   ")
                    End If

                    If Programmazione_Entita_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Destinazioni.Programmazione_Entita_Cod = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & " ")
                    End If

                    If Piva <> "" Then
                        StrSQL.AppendLine(" AND Ricette_Destinazioni.Piva = " & Agro_SQL_SaveText(Piva) & " ")
                    End If
                    If Sa_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If
                    If Appezza <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Destinazioni.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
                    End If
                    If Id_Reg <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Destinazioni.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & " ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   Ricette_Destinazioni.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   Ricette_Destinazioni.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY Ricette_Destinazioni.Ricetta_SuperUser, Ricette_Destinazioni.Ricetta_Cod, Ricette_Destinazioni.Ricetta_Dettaglio_Cod Asc ")
                    End If



                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
                    '------------------------------------------------------------------
                    StrSQL.Length = 0

                    StrSQL.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")

                    StrSQL.AppendLine(" SELECT Ricette_Destinazioni.* ")
                    If joinRicette Then
                        StrSQL.AppendLine("  , Ricette_Operazioni.Validita_Inizio AS Data_Ricetta_Operazione ")
                    End If

                    If joinDescrizioneImpianto Then
                        StrSQL.AppendLine(" , Reg_Impianti.Validita_Inizio AS Validita_Inizio_Impianto ")
                        StrSQL.AppendLine(" , Reg_Impianti.Validita_Fine AS Validita_Fine_Impianto ")
                        StrSQL.AppendLine(" , Reg_Impianti.Cul_Cod ")
                        StrSQL.AppendLine(" , ISNULL(SpecieVegetali.Veg_Des,'') AS Veg_Des ")
                        StrSQL.AppendLine(" , ISNULL(Cultivar.Cul_Des,'') AS Cul_Des ")
                        StrSQL.AppendLine(" , Appezzamento.APP_NOME ")
                        StrSQL.AppendLine(" , ISNULL(DestinazioneUsoDes.descrizione, '') AS destinazioneUso ")
                    End If

                    StrSQL.AppendLine(" FROM  Ricette_Destinazioni ")

                    If joinRicette Then
                        StrSQL.AppendLine(" INNER JOIN  Ricette on Ricette.Ricetta_Cod = Ricette_Destinazioni.Ricetta_COD ")
                        StrSQL.AppendLine(" INNER JOIN  Ricette_Operazioni on Ricette_Operazioni.Ricetta_Operazione_Cod = Ricette_Destinazioni.Ricetta_Operazione_COD ")
                    End If

                    If joinDescrizioneImpianto Then
                        StrSQL.AppendLine(" INNER JOIN Reg_Impianti ON ")
                        StrSQL.AppendLine("     Reg_Impianti.PIVA = Ricette_Destinazioni.PIVA ")
                        StrSQL.AppendLine(" AND Reg_Impianti.SA_COD = Ricette_Destinazioni.SA_COD ")
                        StrSQL.AppendLine(" AND Reg_Impianti.APPEZZA = Ricette_Destinazioni.APPEZZA ")
                        StrSQL.AppendLine(" AND Reg_Impianti.Id_Reg = Ricette_Destinazioni.Id_Reg ")
                        StrSQL.AppendLine(" INNER JOIN Appezzamento ON ")
                        StrSQL.AppendLine("     Appezzamento.PIVA = Reg_Impianti.PIVA ")
                        StrSQL.AppendLine(" AND Appezzamento.SA_COD = Reg_Impianti.SA_COD ")
                        StrSQL.AppendLine(" AND Appezzamento.APPEZZA = Reg_Impianti.APPEZZA ")
                        StrSQL.AppendLine(" LEFT JOIN Cultivar ON ")
                        StrSQL.AppendLine("     Reg_Impianti.CUL_COD = Cultivar.Cul_Cod ")
                        StrSQL.AppendLine(" LEFT JOIN SpecieVegetali ON ")
                        StrSQL.AppendLine("     SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ")

                        StrSQL.AppendLine(" LEFT JOIN Reg_Impianti_Codici DestinazioneUsoCod ON ")
                        StrSQL.AppendLine("     DestinazioneUsoCod.PIVA = Reg_Impianti.PIVA ")
                        StrSQL.AppendLine(" AND DestinazioneUsoCod.sa_cod = Reg_Impianti.sa_cod ")
                        StrSQL.AppendLine(" AND DestinazioneUsoCod.appezza = Reg_Impianti.appezza ")
                        StrSQL.AppendLine(" AND DestinazioneUsoCod.ID_REG = Reg_Impianti.Id_Reg ")
                        StrSQL.AppendLine(" AND DestinazioneUsoCod.id_cod >=3000 and DestinazioneUsoCod.id_cod < 4000 ")
                        StrSQL.AppendLine(" LEFT JOIN Codici_Anagrafe DestinazioneUsoDes ON DestinazioneUsoDes.codice = DestinazioneUsoCod.id_cod ")
                    End If

                    StrSQL.AppendLine(" WHERE Ricette_Destinazioni.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    StrSQL.AppendLine(" AND   Ricette_Destinazioni.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

                    If objParametri.PivaSuperUser <> "" Then
                        StrSQL.AppendLine(" AND Ricette_Destinazioni.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
                    End If

                    If Ricetta_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Destinazioni.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
                    End If

                    If Ricetta_Operazione_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Destinazioni.Ricetta_Operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & "   ")
                    End If

                    If Ricetta_Dettaglio_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Destinazioni.Ricetta_Dettaglio_Cod = " & Agro_SQL_SaveNum(Ricetta_Dettaglio_Cod) & "   ")
                    End If

                    If Ricetta_Destinazione_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Destinazioni.Ricetta_Destinazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Destinazione_Cod) & "   ")
                    End If

                    If Programmazione_Entita_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Destinazioni.Programmazione_Entita_Cod = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & " ")
                    End If

                    If Piva <> "" Then
                        StrSQL.AppendLine(" AND Ricette_Destinazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If
                    If Sa_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If
                    If Appezza <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Destinazioni.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
                    End If
                    If Id_Reg <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Destinazioni.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & " ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   Ricette_Destinazioni.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   Ricette_Destinazioni.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY Ricette_Destinazioni.Ricetta_SuperUser, Ricette_Destinazioni.Ricetta_Cod, Ricette_Destinazioni.Ricetta_Dettaglio_Cod Asc ")
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta


                    '
                    '
                    '
                    '


            End Select


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)
        End Try

        Return DT

    End Function

    '##############################################################################################
    'usata nella stampa 3 della ricetta
    Public Function LeggiConDettagliImpiantoReale(ByVal Ricetta_Cod As Int32,
                                        ByVal Ricetta_Operazione_Cod As Int32,
                                        ByVal Ricetta_Dettaglio_Cod As Int32,
                                        ByVal Ricetta_Destinazione_Cod As Int32,
                                        ByVal Piva As String,
                                        ByVal Sa_Cod As Int32,
                                        ByVal Appezza As Int32,
                                        ByVal Id_Reg As Int32,
                                        ByVal Flag_LeggiIndirizziRubricaAppezza As Boolean,
                                            ByVal Flag_LeggiRegolamentoDPI As Boolean,
                                            ByVal Data_inizio_FiltroDistinta As Date,
                                            ByVal Data_fine_FiltroDistinta As Date,
                                             ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Destinazioni_R.LeggiConDettagliImpiantoReale()"

        '====================================================================================
        'Parametri opzionali :
        '   Ricetta_Cod = 0
        '   Ricetta_Operazione_Cod = 0
        '   Ricetta_Dettaglio_Cod = 0
        '   Ricetta_Destinazione_Cod = 0
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '------------------------------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT RD.Ricetta_Cod, RD.Ricetta_Operazione_Cod, Ricetta_Dettaglio_Cod, Ricetta_Destinazione_Cod, RO.lav_cod, ")
            StrSQL.AppendLine(" Programmazione_Entita_Cod, RD.Piva, RD.Sa_Cod, RD.Appezza, RD.Id_Reg, RD.Qta, RD.Qta2 ")
            StrSQL.AppendLine(" , Reg_Impianti.Sup_Imp, ")
            StrSQL.AppendLine(" ISNULL(SpecieVegetali.Veg_Des,'') AS Veg_Des, ISNULL(SpecieVegetali.Veg_Cod,0) AS Veg_Cod, ")
            StrSQL.AppendLine(" ISNULL(Cultivar.Cul_Des,'') AS Cul_Des, ISNULL(Cultivar.Cul_Cod,'') AS Cul_Cod, ")
            StrSQL.AppendLine(" Appezzamento.APP_NOME, ")
            StrSQL.AppendLine(" ISNULL ((SELECT     TOP 1 Appezzamento_Codici.val_cod ")
            StrSQL.AppendLine("           FROM Appezzamento_Codici ")
            StrSQL.AppendLine("           WHERE     (Appezzamento_Codici.PIVA = Appezzamento.PIVA) ")
            StrSQL.AppendLine("           AND (Appezzamento_Codici.sa_cod = Appezzamento.sa_cod)  ")
            StrSQL.AppendLine("           AND (Appezzamento_Codici.appezza = Appezzamento.appezza) ")
            StrSQL.AppendLine("           AND (Appezzamento_Codici.id_cod = 1104)), '') AS App_Nome_Breve, ")
            StrSQL.AppendLine(" ISNULL(Centri_Aziendali.sa_nome,'') AS sa_nome, ")
            StrSQL.AppendLine(" ISNULL(Campi.Campo_Des,'') AS Campo_Des ")

            If Flag_LeggiRegolamentoDPI = True Then
                StrSQL.AppendLine(", ISNULL(( SELECT " & vbCrLf)
                StrSQL.AppendLine("         CASE WHEN Regolamento_Cod= 4 THEN 'Bio' " & vbCrLf)
                StrSQL.AppendLine("         ELSE " & vbCrLf)
                StrSQL.AppendLine("                 CASE WHEN Disciplinare_Cod= 0 THEN'Convezionale' " & vbCrLf)
                StrSQL.AppendLine("                 ELSE 'Integrato' " & vbCrLf)
                StrSQL.AppendLine("                 END " & vbCrLf)
                StrSQL.AppendLine("         END" & vbCrLf)
                StrSQL.AppendLine(" FROM    Imprese_Progetti " & vbCrLf)
                StrSQL.AppendLine(" WHERE  (Imprese_Progetti.Piva = Reg_Impianti.Piva) " & vbCrLf)
                StrSQL.AppendLine(" AND     (Imprese_Progetti.Sa_Cod = Reg_Impianti.sa_cod) " & vbCrLf)
                StrSQL.AppendLine(" AND     (Imprese_Progetti.Appezza  = Reg_Impianti.Appezza) " & vbCrLf)
                StrSQL.AppendLine(" AND     (Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg) " & vbCrLf)
                StrSQL.AppendLine(" AND     (Imprese_Progetti.Validita_Inizio <= RO.Validita_Inizio ) " & vbCrLf)
                StrSQL.AppendLine(" AND     (Imprese_Progetti.Validita_Fine >= RO.Validita_Inizio ) " & vbCrLf)
                StrSQL.AppendLine(" ) , '') AS reg_dpi " & vbCrLf)
            End If

            If Flag_LeggiIndirizziRubricaAppezza = True Then
                StrSQL.AppendLine(" , ISNULL( (")
                StrSQL.AppendLine("         SELECT   Indirizzi.ind_des + ' '  + Indirizzi.CAP + ' - ' + Indirizzi.frz_des + ' '  + ISNULL(ISTAT.LOCALITA, '')  + ' (' + ISNULL(ISTAT.COMUNI_PROV, '') + ')'    " & vbCrLf)
                StrSQL.AppendLine("         FROM [AppezzamentixIndirizzi] AI  " & vbCrLf)
                StrSQL.AppendLine("         Inner join indirizzi on AI.cod_indirizzo = Indirizzi.cod_indirizzo  " & vbCrLf)
                StrSQL.AppendLine("         LEFT OUTER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM " & vbCrLf)
                StrSQL.AppendLine("          where AI.tipo_indirizzo=1 " & vbCrLf)
                StrSQL.AppendLine("         AND AI.piva = Appezzamento.piva " & vbCrLf)
                StrSQL.AppendLine("         and AI.sa_cod = Appezzamento.sa_cod " & vbCrLf)
                StrSQL.AppendLine("         and AI.appezza = Appezzamento.appezza " & vbCrLf)
                StrSQL.AppendLine("      ),  '') AS Appezza_Indirizzo    " & vbCrLf)

                StrSQL.AppendLine(" , ISNULL( (" & vbCrLf)
                StrSQL.AppendLine("         SELECT  TOP 1 numero " & vbCrLf)
                StrSQL.AppendLine("         FROM [Appezzamentixrubrica] AR  " & vbCrLf)
                StrSQL.AppendLine("         Inner join rubrica on AR.cod_rubrica = rubrica.cod_rubrica  " & vbCrLf)
                StrSQL.AppendLine("         where numero <>'' " & vbCrLf)
                StrSQL.AppendLine("         AND AR.piva = Appezzamento.piva " & vbCrLf)
                StrSQL.AppendLine("         and AR.sa_cod = Appezzamento.sa_cod " & vbCrLf)
                StrSQL.AppendLine("         and AR.appezza = Appezzamento.appezza " & vbCrLf)
                StrSQL.AppendLine("      ),  '') AS Appezza_Rubrica   " & vbCrLf)
            End If


            StrSQL.AppendLine(" FROM  Ricette_Destinazioni RD " & vbCrLf)
            StrSQL.AppendLine(" inner join Ricette_Operazioni RO on RD.Ricetta_SuperUser = RO.Ricetta_SuperUser and RD.Ricetta_Cod= RO.Ricetta_Cod and RD.Ricetta_Operazione_Cod= RO.Ricetta_Operazione_Cod " & vbCrLf)

            StrSQL.AppendLine(" INNER JOIN Reg_Impianti ON RD.PIVA = Reg_Impianti.PIVA AND RD.SA_COD = Reg_Impianti.SA_COD AND RD.APPEZZA = Reg_Impianti.APPEZZA AND RD.Id_Reg = Reg_Impianti.Id_Reg " & vbCrLf)
            StrSQL.AppendLine(" INNER JOIN Appezzamento ON Appezzamento.PIVA = Reg_Impianti.PIVA AND Appezzamento.SA_COD = Reg_Impianti.SA_COD AND Appezzamento.APPEZZA = Reg_Impianti.APPEZZA " & vbCrLf)
            StrSQL.AppendLine(" INNER JOIN Centri_Aziendali ON Appezzamento.PIVA = Centri_Aziendali.PIVA AND Appezzamento.SA_COD = Centri_Aziendali.sa_cod " & vbCrLf)
            StrSQL.AppendLine(" LEFT OUTER JOIN Campi ON Appezzamento.PIVA = Campi.Piva AND Appezzamento.SA_COD = Campi.Sa_Cod AND Appezzamento.Campo_Cod = Campi.Campo_Cod  " & vbCrLf)
            StrSQL.AppendLine(" INNER JOIN Cultivar ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod " & vbCrLf)
            StrSQL.AppendLine(" INNER JOIN SpecieVegetali ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod " & vbCrLf)

            StrSQL.AppendLine(" WHERE 1 = 1 " & vbCrLf)

            If objParametri.PivaSuperUser <> "" Then
                StrSQL.AppendLine(" AND RD.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   " & vbCrLf)
            End If

            If Ricetta_Cod <> 0 Then
                StrSQL.AppendLine(" AND RD.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   " & vbCrLf)
            End If

            If Ricetta_Operazione_Cod <> 0 Then
                StrSQL.AppendLine(" AND RD.Ricetta_Operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & "   " & vbCrLf)
            End If

            If Ricetta_Dettaglio_Cod <> 0 Then
                StrSQL.AppendLine(" AND RD.Ricetta_Dettaglio_Cod = " & Agro_SQL_SaveNum(Ricetta_Dettaglio_Cod) & "   " & vbCrLf)
            End If

            If Ricetta_Destinazione_Cod <> 0 Then
                StrSQL.AppendLine(" AND RD.Ricetta_Destinazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Destinazione_Cod) & "   " & vbCrLf)
            End If

            If Piva <> "" Then
                StrSQL.AppendLine(" AND RD.Piva = " & Agro_SQL_SaveText(Piva) & " " & vbCrLf)
            End If
            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND RD.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " " & vbCrLf)
            End If
            If Appezza <> 0 Then
                StrSQL.AppendLine(" AND RD.Appezza = " & Agro_SQL_SaveNum(Appezza) & " " & vbCrLf)
            End If
            If Id_Reg <> 0 Then
                StrSQL.AppendLine(" AND RD.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & " " & vbCrLf)
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Ricetta_Cod, Ricetta_Operazione_Cod, Sa_nome, Veg_Des, Cul_Des, App_Nome " & vbCrLf)
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




    '##############################################################################################
    Public Function Leggi_Operazioni_Da_Programmazione_Entita_Cod(
                            ByVal Ricetta_Cod As Int32,
                            ByVal Ricetta_Operazione_Cod As Int32,
                            ByVal Ricetta_Dettaglio_Cod As Int32,
                            ByVal Ricetta_Destinazione_Cod As Int32,
                            ByVal Programmazione_Entita_Cod As Int32,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Destinazioni_R.Leggi_Operazioni_Da_Programmazione_Entita_Cod()"

        '====================================================================================
        'Parametri opzionali :
        '   Ricetta_Cod = 0
        '   Ricetta_Operazione_Cod = 0
        '   Ricetta_Dettaglio_Cod = 0
        '   Ricetta_Destinazione_Cod = 0
        '   Programmazione_Entita_Cod = 0
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            '------------------------------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" SELECT     Ricette_Destinazioni.Ricetta_Cod, Ricette_Destinazioni.Ricetta_Operazione_Cod, Ricette_Destinazioni.Ricetta_Dettaglio_Cod,  ")
            StrSQL.Append("                       Ricette_Destinazioni.Ricetta_Destinazione_Cod, Ricette_Destinazioni.Programmazione_Entita_Cod, Ricette_Destinazioni.Qta,  ")
            StrSQL.Append("                       Ricette_Destinazioni.Validita_Inizio, Ricette_Destinazioni.Validita_Fine, Ricette_Dettagli.Miscela_Cod, Ricette_Dettagli.Elem_Cod,  ")
            StrSQL.Append("                       Ricette_Dettagli.Pro_Cod, Ricette_Dettagli.Mat_Cod, Ricette_Dettagli.Udm_Cod, Ricette_Dettagli.Qta AS Expr1, Ricette_Dettagli.Extra_Int,  ")
            StrSQL.Append("                       Ricette_Dettagli.Prezzo_Unitario, Ricette_Operazioni.Ricetta_Operazione_Des, Ricette_Operazioni.Lav_Cod, Ricette_Operazioni.Note,  ")
            StrSQL.Append("                       Ricette_Operazioni.Mezzo, Ricette_Operazioni.Validita_Inizio AS Expr2, Ricette_Operazioni.Gru_Op, Ricette_Operazioni.Costo,  ")
            StrSQL.Append("                       Ricette_Operazioni.Noleggio_Passivo ")
            StrSQL.Append(" FROM         Ricette_Destinazioni INNER JOIN ")
            StrSQL.Append("                       Ricette_Dettagli ON Ricette_Destinazioni.Ricetta_SuperUser = Ricette_Dettagli.Ricetta_SuperUser AND  ")
            StrSQL.Append("                       Ricette_Destinazioni.Ricetta_Cod = Ricette_Dettagli.Ricetta_Cod AND  ")
            StrSQL.Append("                       Ricette_Destinazioni.Ricetta_Operazione_Cod = Ricette_Dettagli.Ricetta_Operazione_Cod AND  ")
            StrSQL.Append("                       Ricette_Destinazioni.Ricetta_Dettaglio_Cod = Ricette_Dettagli.Ricetta_Dettaglio_Cod INNER JOIN ")
            StrSQL.Append("                       Ricette_Operazioni ON Ricette_Dettagli.Ricetta_SuperUser = Ricette_Operazioni.Ricetta_SuperUser AND  ")
            StrSQL.Append("                       Ricette_Dettagli.Ricetta_Cod = Ricette_Operazioni.Ricetta_Cod AND  ")
            StrSQL.Append("                       Ricette_Dettagli.Ricetta_Operazione_Cod = Ricette_Operazioni.Ricetta_Operazione_Cod ")

            StrSQL.Append(" WHERE Ricette_Destinazioni.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.Append(" AND   Ricette_Destinazioni.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            If objParametri.PivaSuperUser <> "" Then
                StrSQL.Append(" AND Ricette_Destinazioni.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            End If

            If Ricetta_Cod <> 0 Then
                StrSQL.Append(" AND Ricette_Destinazioni.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
            End If

            If Ricetta_Operazione_Cod <> 0 Then
                StrSQL.Append(" AND Ricette_Destinazioni.Ricetta_Operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & "   ")
            End If

            If Ricetta_Dettaglio_Cod <> 0 Then
                StrSQL.Append(" AND Ricette_Destinazioni.Ricetta_Dettaglio_Cod = " & Agro_SQL_SaveNum(Ricetta_Dettaglio_Cod) & "   ")
            End If

            If Ricetta_Destinazione_Cod <> 0 Then
                StrSQL.Append(" AND Ricette_Destinazioni.Ricetta_Destinazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Destinazione_Cod) & "   ")
            End If

            If Programmazione_Entita_Cod <> 0 Then
                StrSQL.Append(" AND Ricette_Destinazioni.Programmazione_Entita_Cod = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Ricette_Destinazioni.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Ricette_Destinazioni.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Ricette_Destinazioni.Ricetta_SuperUser, Ricette_Destinazioni.Ricetta_Cod, Ricette_Destinazioni.Ricetta_Dettaglio_Cod Asc ")
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

    '##############################################################################################
    Public Function Leggi_Operazioni_Da_Programmazione_Cod(
                            ByVal Piva As String,
                            ByVal Programmazione_Cod As Int32,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Destinazioni_R.Leggi_Operazioni_Da_Programmazione_Cod()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            '------------------------------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" SELECT     Ricette_Destinazioni.Ricetta_Cod, Ricette_Destinazioni.Ricetta_Operazione_Cod, Ricette_Destinazioni.Ricetta_Dettaglio_Cod,  ")
            StrSQL.Append("                       Ricette_Destinazioni.Ricetta_Destinazione_Cod, Ricette_Destinazioni.Programmazione_Entita_Cod, Ricette_Destinazioni.Qta,  ")
            StrSQL.Append("                       Ricette_Destinazioni.Validita_Inizio, Ricette_Destinazioni.Validita_Fine, Ricette_Dettagli.Miscela_Cod, Ricette_Dettagli.Elem_Cod,  ")
            StrSQL.Append("                       Ricette_Dettagli.Pro_Cod, Ricette_Dettagli.Mat_Cod, Ricette_Dettagli.Udm_Cod, Ricette_Dettagli.Qta AS Expr1, Ricette_Dettagli.Extra_Int,  ")
            StrSQL.Append("                       Ricette_Dettagli.Prezzo_Unitario, Ricette_Operazioni.Ricetta_Operazione_Des, Ricette_Operazioni.Lav_Cod, Ricette_Operazioni.Note,  ")
            StrSQL.Append("                       Ricette_Operazioni.Mezzo, Ricette_Operazioni.Validita_Inizio AS Expr2, Ricette_Operazioni.Gru_Op, Ricette_Operazioni.Costo,  ")
            StrSQL.Append("                       Ricette_Operazioni.Noleggio_Passivo ")
            StrSQL.Append(" FROM         Ricette_Destinazioni INNER JOIN ")
            StrSQL.Append("                       Ricette_Dettagli ON Ricette_Destinazioni.Ricetta_SuperUser = Ricette_Dettagli.Ricetta_SuperUser AND  ")
            StrSQL.Append("                       Ricette_Destinazioni.Ricetta_Cod = Ricette_Dettagli.Ricetta_Cod AND  ")
            StrSQL.Append("                       Ricette_Destinazioni.Ricetta_Operazione_Cod = Ricette_Dettagli.Ricetta_Operazione_Cod AND  ")
            StrSQL.Append("                       Ricette_Destinazioni.Ricetta_Dettaglio_Cod = Ricette_Dettagli.Ricetta_Dettaglio_Cod INNER JOIN ")
            StrSQL.Append("                       Ricette_Operazioni ON Ricette_Dettagli.Ricetta_SuperUser = Ricette_Operazioni.Ricetta_SuperUser AND  ")
            StrSQL.Append("                       Ricette_Dettagli.Ricetta_Cod = Ricette_Operazioni.Ricetta_Cod AND  ")
            StrSQL.Append("                       Ricette_Dettagli.Ricetta_Operazione_Cod = Ricette_Operazioni.Ricetta_Operazione_Cod INNER JOIN ")
            StrSQL.Append("       Programmazione_Entita ON Ricette_Destinazioni.Programmazione_Entita_Cod = Programmazione_Entita.Programmazione_Entita_Cod AND  ")
            StrSQL.Append("       Ricette_Destinazioni.Ricetta_SuperUser = Programmazione_Entita.Piva_SuperUser  ")

            StrSQL.Append(" WHERE Ricette_Destinazioni.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.Append(" AND   Ricette_Destinazioni.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            If objParametri.PivaSuperUser <> "" Then
                StrSQL.Append(" AND Ricette_Destinazioni.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            End If

            If Piva <> "" Then
                StrSQL.Append(" AND Programmazione_Entita.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Programmazione_Cod <> 0 Then
                StrSQL.Append(" AND Programmazione_Entita.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Ricette_Destinazioni.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Ricette_Destinazioni.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Ricette_Destinazioni.Ricetta_SuperUser, Ricette_Destinazioni.Ricetta_Cod, Ricette_Destinazioni.Ricetta_Dettaglio_Cod Asc ")
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

    Public Function EsistonoOperazioni_suProgrammazioneEntita(ByVal Piva As String,
                                                              ByVal Programmazione_Entita_Cod As Int32,
                                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                 ) As Boolean


        Dim bRet As Boolean = False

        Try

            Dim dt As DataTable
            dt = Leggi(0, 0, 0, 0, Programmazione_Entita_Cod, Piva, 0, 0, 0,
                                         AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO,
                                         AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAFINE,
                                         AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                         "", "", objParametri)

            If dt.Rows.Count > 0 Then
                bRet = True
            End If

            dt.Dispose()
            dt = Nothing

        Catch ex As Exception

            bRet = False

        Finally


        End Try


        Return bRet


    End Function


    Public Function EsistonoOperazioni_suProgrammazioneEntita_daProgrammazioneCod(ByVal Piva As String,
                                                          ByVal Programmazione_Cod As Int32,
                                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                             ) As Boolean


        Dim bRet As Boolean = False

        Try

            Dim dt As DataTable
            dt = Leggi_Operazioni_Da_Programmazione_Cod(Piva, Programmazione_Cod,
                                         AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO,
                                         AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAFINE,
                                         "", "", objParametri)

            If dt.Rows.Count > 0 Then
                bRet = True
            End If

            dt.Dispose()
            dt = Nothing

        Catch ex As Exception

            bRet = False

        Finally


        End Try


        Return bRet


    End Function

    Public Function SupTotTrattata_from_RicettaOperazioneCod(ByVal Ricetta_Operazione_Cod As Integer,
                                              ByVal xFiltroAggiuntivo As String,
                                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                              ) As Decimal

        Dim dt As DataTable
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Destinazioni_R.SupTotTrattata_from_RicettaOperazioneCod()"
        Dim messaggioErrore As String

        Dim supTot As Decimal = 0

        Try

            dt = SupTotTrattata_from_RicettaOperazioneCod(Ricetta_Operazione_Cod, xFiltroAggiuntivo, "", objParametri)

            Dim i As Integer

            For i = 0 To dt.Rows.Count - 1
                supTot += dt.Rows(i).Item("Sup_Operazione")
            Next

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try


        dt.Dispose()
        dt = Nothing
        Return supTot

    End Function

    Public Function SupTotTrattata_from_RicettaOperazioneCod(ByVal Ricetta_Operazione_Cod As Integer,
                                             ByVal xFiltroAggiuntivo As String,
                                             ByVal xOrderBy As String,
                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                             ) As DataTable


        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Destinazioni_R.SupTotTrattata_from_IdAgenda()"

        Dim messaggioErrore As String
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.Append(" SELECT Distinct Ricetta_Operazione_Cod, SUM(Ricette_Destinazioni.Qta2*10000)/10000 AS Sup_Operazione  ")
            strSql.Append(" FROM  Ricette_Destinazioni ")
            strSql.Append(" WHERE tipo_destinazione=0 ")
            strSql.Append(" AND Ricetta_Operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & " ")
            strSql.Append(" group by Ricetta_Operazione_Cod, ricetta_dettaglio_cod ")

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.Append(" ORDER BY Ricetta_Operazione_Cod ASC ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    '#############################################################################################
    Public Function Leggi_DistinctRicetta_Operazione_Cod_Impianti(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                           ByVal Piva As String,
                                                           ByVal Sa_Cod As Integer,
                                                           ByVal Appezza As Integer,
                                                           ByVal Id_Reg As Integer,
                                                           ByVal FinestraTemp_Inizio As Date,
                                                           ByVal FinestraTemp_Fine As Date,
                                                           ByVal FiltroAggiuntivo As String,
                                                           ByVal Ordinamento As String) As DataTable

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Destinazioni_R.Leggi_DistinctRicetta_Cod_Impianti()"
        Dim MessaggioErrore As String = ""
        Dim DT As New DataTable

        Try


            Dim stbQuery As New System.Text.StringBuilder


            Dim i As Integer = 0

            '----------------------------------------------------
            '--- Preparo la Query SQL ---------------------------
            '----------------------------------------------------

            stbQuery.Append(" SELECT DISTINCT Ricette_Destinazioni.Ricetta_Operazione_Cod ")

            stbQuery.Append(" FROM Ricette_Destinazioni ")
            stbQuery.Append(" JOIN Ricette_Operazioni ON Ricette_Destinazioni.Ricetta_Cod = Ricette_Operazioni.Ricetta_Cod  ")
            stbQuery.Append("       AND Ricette_Destinazioni.Ricetta_Operazione_Cod = Ricette_Operazioni.Ricetta_Operazione_Cod ")

            'CONDIZIONI
            stbQuery.Append(" WHERE Ricette_Operazioni.Validita_Inizio <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " ")
            stbQuery.Append(" AND   Ricette_Operazioni.Validita_Inizio >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")
            stbQuery.Append(" AND   Ricette_Operazioni.Validita_Inizio >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")
            stbQuery.Append(" AND   Ricette_Destinazioni.Tipo_Destinazione >= " & Agro_SQL_SaveNum(TIPO_DESTINAZIONE_IMPIANTO) & " ")

            If Piva <> "" Then
                stbQuery.Append(" AND Ricette_Destinazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                stbQuery.Append(" AND Ricette_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Appezza <> 0 Then
                stbQuery.Append(" AND Ricette_Destinazioni.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
            End If

            If Id_Reg <> 0 Then
                stbQuery.Append(" AND Ricette_Destinazioni.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & "   ")
            End If

            If FiltroAggiuntivo <> "" Then
                stbQuery.Append(FiltroAggiuntivo + " ")
            End If

            If Ordinamento <> "" Then
                stbQuery.Append(Ordinamento + " ")
            End If


            '----------------------------------------------------
            '--- Recupero il datatable --------------------------
            '----------------------------------------------------

            'Recupero il datatable
            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stbQuery.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function


    '#############################################################################################
    Public Function Leggi_Distinct_Impianti(ByVal Ricetta_Cod As Integer,
                                            ByVal Validita_Inizio As Date,
                                            ByVal Validita_Fine As Date,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Destinazioni_R.Leggi_Distinct_Impianti()"
        Dim MessaggioErrore As String = ""
        Dim DT As New DataTable

        Try


            Dim stbQuery As New System.Text.StringBuilder


            Dim i As Integer = 0

            '----------------------------------------------------
            '--- Preparo la Query SQL ---------------------------
            '----------------------------------------------------

            stbQuery.AppendLine(" SELECT DISTINCT Ricette_Destinazioni.Ricetta_Operazione_Cod, Ricette_Operazioni.Validita_Inizio,")
            stbQuery.AppendLine(" Ricette_Operazioni.Lav_Cod, Operazioni.Lav_Des,Ricette_Operazioni.Raccoglitore_Cod,Ricette_Operazioni.Invia_App,")
            stbQuery.AppendLine(" Ricette_Destinazioni.Piva, Ricette_Destinazioni.Sa_Cod")

            stbQuery.AppendLine(" FROM Ricette_Destinazioni ")
            stbQuery.AppendLine(" JOIN Ricette_Operazioni ON Ricette_Destinazioni.Ricetta_Cod = Ricette_Operazioni.Ricetta_Cod  ")
            stbQuery.AppendLine("       AND Ricette_Destinazioni.Ricetta_Operazione_Cod = Ricette_Operazioni.Ricetta_Operazione_Cod ")

            stbQuery.AppendLine(" JOIN Operazioni ON Operazioni.Lav_Cod = Ricette_Operazioni.Lav_Cod  ")

            'CONDIZIONI
            stbQuery.AppendLine(" WHERE Ricette_Operazioni.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            stbQuery.AppendLine(" AND   Ricette_Operazioni.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            stbQuery.AppendLine(" AND   Ricette_Destinazioni.Tipo_Destinazione = " & Agro_SQL_SaveNum(TIPO_DESTINAZIONE_IMPIANTO) & " ")

            If Ricetta_Cod <> 0 Then
                stbQuery.AppendLine(" AND Ricette_Operazioni.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stbQuery.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                stbQuery.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If


            '----------------------------------------------------
            '--- Recupero il datatable --------------------------
            '----------------------------------------------------

            'Recupero il datatable
            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stbQuery.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function



    Public Function Lettura_Ricette(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, dataUpperBound As DateTime, dataLower As DateTime?) As DataTable
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Operazioni_R.Lettura_Ricette()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim dt As DataTable
        Dim res As Integer = -1

        Try

            strSql.Length = 0
            strSql.AppendLine("With #UltimeRicetteInviate AS (")
            strSql.AppendLine("SELECT *")
            strSql.AppendLine("FROM (")
            strSql.AppendLine("SELECT  ID_Ricetta,")
            strSql.AppendLine("ID_Ricetta_Operazione,")
            strSql.AppendLine("Data_Modifica,")
            strSql.AppendLine("ROW_NUMBER() OVER(PARTITION BY ID_Ricetta_Operazione ORDER BY Data_Modifica DESC) rn")
            strSql.AppendLine("FROM Agronica_Log_Invio_Ricette")
            strSql.AppendLine("WHERE INVIATO = 0")
            strSql.Append("AND Tipo_Esportazione = ").AppendLine(enum_Esportazioni_Sistema_Cod.Recap_Ricette_CAI)
            strSql.AppendLine(") a")
            strSql.AppendLine("WHERE rn = 1")
            strSql.AppendLine(")")
            strSql.AppendLine("SELECT rDest.Piva,rDest.Sa_Cod, rDest.Id_Reg , rdet.Lotto, rDest.Ricetta_Cod, rDest.Ricetta_Operazione_Cod,  rDest.Ricetta_Destinazione_Cod, rDest.Piva, imp.rag_soc, rDest.MagazzinoEsterno_Cod, rDest.MagazzinoEsterno_Des, rDest.magazzinoEsterno_Dettagli, rDest.Qta, uM.UDM_DES, rdet.Pro_Cod,")
            strSql.AppendLine("rDet.Elem_Cod, rdet.Mat_Cod, rOp.Data_Modifica, rOp.Data_Creazione,")
            strSql.AppendLine("CAST(CASE")
            strSql.AppendLine("WHEN agLog.data_modifica Is Not Null")
            strSql.AppendLine("THEN 1")
            strSql.AppendLine("ELSE 0")
            strSql.AppendLine("END AS Bit) as Inviata,")
            strSql.AppendLine("CAST(CASE")
            strSql.AppendLine("WHEN rOp.Data_Modifica > agLog.data_modifica")
            strSql.AppendLine("THEN 1")
            strSql.AppendLine("ELSE 0")
            strSql.AppendLine("END AS Bit) as Modificata")
            strSql.AppendLine("FROM Ricette_Destinazioni rDest")
            strSql.AppendLine("INNER JOIN Ricette_Dettagli rDet ON (rDest.Ricetta_Cod = rDet.Ricetta_Cod AND rDest.Ricetta_Dettaglio_Cod = rDet.Ricetta_Dettaglio_Cod)")
            strSql.AppendLine("INNER JOIN Ricette_Operazioni rOP on (rDest.Ricetta_Cod = rOP.Ricetta_Cod AND rDest.Ricetta_Operazione_Cod = rOP.Ricetta_Operazione_Cod)")
            strSql.AppendLine("INNER JOIN UnitaMisura uM on (uM.UDM_COD = rDet.Udm_Cod)")
            strSql.AppendLine("INNER JOIN Imprese imp on (imp.PIVA = rDest.Piva)")
            strSql.AppendLine("LEFT JOIN #UltimeRicetteInviate agLog ON (rDest.Ricetta_Cod = agLog.ID_Ricetta AND rDest.Ricetta_Operazione_Cod = agLog.ID_Ricetta_Operazione)")
            strSql.AppendLine("WHERE")
            strSql.AppendLine("rDest.Tipo_Destinazione = 20 AND")
            strSql.AppendLine("rDest.MagazzinoEsterno_Cod <> '' AND")
            strSql.Append("rOp.Data_Modifica <= ").AppendLine(Agro_SQL_SaveDateTime(dataUpperBound))

            If dataLower.HasValue Then
                strSql.Append("AND rOp.Data_Modifica >= ").AppendLine(Agro_SQL_SaveDateTime(dataLower.Value))
            End If



            '--------------------------------------------------------------------------
            'If xFiltroAggiuntivo <> "" Then
            '    strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            'End If

            ''--------------------------------------------------------------------------
            'If xOrderBy <> "" Then
            '    strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            'End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt
    End Function

    Public Function Ricette_Eliminate(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Operazioni_R.Ricette_Eliminate()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim dt As DataTable
        Dim res As Integer = -1

        Try

            strSql.Length = 0
            strSql.AppendLine("With #UltimeRicetteInviate AS (")
            strSql.AppendLine("SELECT *")
            strSql.AppendLine("FROM (")
            strSql.AppendLine("SELECT  ID_Ricetta,")
            strSql.AppendLine("ID_Ricetta_Operazione,")
            strSql.AppendLine("Agronica_Log_Invio_Ricette.Data_Modifica,")
            strSql.AppendLine("Agronica_Log_Invio_Chiamate.Data_Invio,")
            strSql.AppendLine("ROW_NUMBER() OVER(PARTITION BY ID_Ricetta_Operazione ORDER BY Agronica_Log_Invio_Ricette.Data_Modifica DESC) rn")
            strSql.AppendLine("FROM Agronica_Log_Invio_Ricette")
            strSql.AppendLine("INNER JOIN Agronica_Log_Invio_Chiamate on (Agronica_Log_Invio_Chiamate.ID = Agronica_Log_Invio_Ricette.ID_Log_Invio)")
            strSql.AppendLine("WHERE Agronica_Log_Invio_Ricette.INVIATO = 0")
            strSql.Append("AND Agronica_Log_Invio_Ricette.Tipo_Esportazione = ").AppendLine(enum_Esportazioni_Sistema_Cod.Recap_Ricette_CAI)
            strSql.AppendLine(") a")
            strSql.AppendLine("WHERE rn = 1")
            strSql.AppendLine("), #UltimeRicetteEliminateInviate AS (")
            strSql.AppendLine("SELECT *")
            strSql.AppendLine("FROM (")
            strSql.AppendLine("SELECT  Tipo_Operazione,")
            strSql.AppendLine("Chiave as Id_Ricetta_Operazione,")
            strSql.AppendLine("data_ora_registrazionelog as dataCancellazione,")
            strSql.AppendLine("#UltimeRicetteInviate.Data_Invio as Data_Invio,")
            strSql.AppendLine("object_data,")
            strSql.AppendLine("ROW_NUMBER() OVER(PARTITION BY Chiave ORDER BY data_ora_registrazionelog DESC) rn")
            strSql.AppendLine("FROM Agronica_Log_Ricette")
            strSql.AppendLine("INNER JOIN #UltimeRicetteInviate ON (#UltimeRicetteInviate.ID_Ricetta_Operazione = Chiave)")
            strSql.AppendLine("WHERE Tipo = 'Ricette_Operazioni'")
            strSql.AppendLine(") a")
            strSql.AppendLine("WHERE rn = 1")
            strSql.AppendLine("AND Tipo_Operazione = 3")
            strSql.AppendLine("AND Data_Invio < dataCancellazione")
            strSql.AppendLine("), #ultimiDati AS (")
            strSql.AppendLine("SELECT *")
            strSql.AppendLine("FROM (")
            strSql.AppendLine("SELECT  Agronica_Log_Ricette.*,")
            strSql.AppendLine("ROW_NUMBER() OVER(PARTITION BY Chiave ORDER BY data_ora_registrazionelog DESC) rn")
            strSql.AppendLine("FROM Agronica_Log_Ricette")
            strSql.AppendLine("INNER JOIN #UltimeRicetteEliminateInviate ON (#UltimeRicetteEliminateInviate.Id_Ricetta_Operazione = Agronica_Log_Ricette.Chiave)")
            strSql.AppendLine("WHERE Tipo = 'Ricette_Operazioni'")
            strSql.AppendLine("AND Agronica_Log_Ricette.Chiave IN (SELECT ID_Ricetta_Operazione from #UltimeRicetteEliminateInviate)")
            strSql.AppendLine("AND Agronica_Log_Ricette.Data_Ora_RegistrazioneLog <= #UltimeRicetteEliminateInviate.Data_Invio")
            strSql.AppendLine(") a")
            strSql.AppendLine("WHERE  Tipo_Operazione = 1")
            strSql.AppendLine("AND rn = 1")
            strSql.AppendLine(")")
            strSql.AppendLine("select * from #ultimiDati")





            ''--------------------------------------------------------------------------
            'If xOrderBy <> "" Then
            '    strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            'End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt
    End Function

    Public Function Ricette_Inviate_Passato(ID_Ricetta_Operazione As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Operazioni_R.Ricette_Inviate_Passato()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim dt As DataTable
        Dim res As Integer = -1

        Try

            strSql.Length = 0

            strSql.AppendLine("SELECT * FROM Agronica_Log_Invio_Ricette")
            strSql.AppendLine("INNER JOIN Agronica_Log_Invio_Chiamate on (Agronica_Log_Invio_Chiamate.ID = Agronica_Log_Invio_Ricette.ID_Log_Invio)")
            strSql.AppendLine("where ID_Ricetta_Operazione = ").Append(ID_Ricetta_Operazione)
            strSql.AppendLine("ORDER BY Agronica_Log_Invio_Chiamate.Data_Modifica DESC")



            ''--------------------------------------------------------------------------
            'If xOrderBy <> "" Then
            '    strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            'End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt
    End Function

    Public Function Leggi_PresenzaImpianti_Da_RicettaOperazioneCod(ricetta_operazione_cod As Integer,
                                                                   ByRef objParametri As AgronicaCoreParametri
                                                                   ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Destinazioni_R.Leggi_PresenzaImpianti_Da_RicettaOperazioneCod()"

        Dim messaggioErrore As String
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Dim PresenzaImpianti As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT ISNULL(COUNT(0), 0) AS CountImpianti ")
            StrSQL.AppendLine(" FROM Ricette_Destinazioni ")
            StrSQL.AppendLine(" WHERE Ricetta_Operazione_Cod = " & Agro_SQL_SaveNum(ricetta_operazione_cod) & " ")
            StrSQL.AppendLine(" AND Tipo_Destinazione = " & TIPO_DESTINAZIONE_IMPIANTO & " ")

            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                PresenzaImpianti = dt.Rows(0)("CountImpianti") > 0
            End If

        Catch ex As Exception
            Return True
        End Try

        Return PresenzaImpianti
    End Function

    ''' <summary>
    ''' Legge la cronologia delle ricette da un lista di chiavi (app/impianti)
    ''' </summary>
    ''' <param name="listChiavi">Piva, Sa_Cod, Appezza, Id_Reg, Progetto_Cod</param>
    ''' <param name="profonditaJoin"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    Public Function LeggiCronologiaRicette_Massivo(listChiavi As List(Of (String, Integer, Integer, Integer)),
                                                   profonditaJoin As Enum_EntitaModificaMultiplaPianoColturale,
                                                   ByVal xFiltroAggiuntivo As String,
                                                   ByRef objParametri As AgronicaCoreParametri
                                                   ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Destinazioni_R.LeggiCronologiaRicette_Massivo()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim flagConnessione, flagTransazione As Boolean

        Try

            Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

            TempChiaviMassivo.CreaTabellaTemp_FiltroImpianti(listChiavi, NomeRoutine, objParametri)

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT DISTINCT ")
            StrSQL.AppendLine("     Ricette_Destinazioni.Piva, Ricette_Destinazioni.Sa_Cod, Ricette_Destinazioni.Appezza, Ricette_Destinazioni.Id_Reg ")
            StrSQL.AppendLine(" FROM  Ricette_Destinazioni ")

            If listChiavi IsNot Nothing AndAlso listChiavi.Count > 0 Then
                StrSQL.AppendLine("	JOIN #TempImpianto temp (NOLOCK) ON Ricette_Destinazioni.Piva COLLATE SQL_Latin1_General_CP850_CI_AS = temp.Piva ")
                StrSQL.AppendLine("	AND Ricette_Destinazioni.Sa_Cod = temp.Sa_Cod ")
                If profonditaJoin >= Enum_EntitaModificaMultiplaPianoColturale.Appezzamenti Then
                    StrSQL.AppendLine("	AND Ricette_Destinazioni.Appezza = temp.Appezza ")
                End If
                If profonditaJoin >= Enum_EntitaModificaMultiplaPianoColturale.Impianti Then
                    StrSQL.AppendLine("	AND Ricette_Destinazioni.Id_Reg = temp.Id_Reg ")
                End If
            End If

            StrSQL.AppendLine(" WHERE 1 = 1 ")

            If objParametri.PivaSuperUser <> "" Then
                StrSQL.AppendLine(" AND Ricette_Destinazioni.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            ' Eliminazione tabella temporanea
            TempChiaviMassivo.EliminaTabellaTemp_FiltroImpianti(NomeRoutine, objParametri)

            'commit transazione
            Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing

            'rollback transazione
            Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        Finally
            Utility.VerificaChiudiConnessione(objParametri, flagConnessione)
        End Try

        Return DT

    End Function

    Private Function CreaTabellaTemp_FiltroChiavi() As String
        Dim stb As New StringBuilder

        stb.AppendLine(" IF OBJECT_ID('tempdb.dbo.#TempChiavi') IS NULL BEGIN ")
        stb.AppendLine("    CREATE TABLE #TempChiavi ( ")
        stb.AppendLine("        Piva VARCHAR(50) NULL")
        stb.AppendLine("      , Sa_Cod INT NULL")
        stb.AppendLine("      , Appezza INT NULL")
        stb.AppendLine("      , Id_Reg INT NULL")
        stb.AppendLine("    )")
        stb.AppendLine(" END ")

        Return stb.ToString()
    End Function

    Private Function EliminaTabellaTemp_FiltroChiavi() As String
        Dim stb As New StringBuilder

        stb.AppendLine(" IF NOT OBJECT_ID('tempdb.dbo.#TempChiavi') IS NULL BEGIN ")
        stb.AppendLine("    DROP TABLE #TempChiavi ")
        stb.AppendLine(" END ")

        Return stb.ToString()
    End Function

End Class

'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################

Public Class Ricette_Destinazioni_W
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Scrivi(
                                ByVal Ricetta_Cod As Int32,
                                ByVal Ricetta_Operazione_Cod As Int32,
                                ByVal Ricetta_Dettaglio_Cod As Int32,
                                ByVal Ricetta_Destinazione_Cod As Int32,
                                ByVal Programmazione_Entita_Cod As Int32,
                                ByVal Piva As String,
                                ByVal Sa_Cod As Int32,
                                ByVal Appezza As Int32,
                                ByVal Id_Reg As Int32,
                                ByVal Qta As Decimal,
                                ByVal Qta2 As Decimal,
                                ByVal QuotaDistribuzione As Decimal,
                                ByVal Tipo_Destinazione As Integer,
                                ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                , Optional ByVal Data_creazione As Date = #2/1/1900# _
                                , Optional ByVal Data_modifica As Date = #2/1/1900# _
                                , Optional ByVal username_creazione As String = "" _
                                , Optional ByVal username_modifica As String = "" _
                                , Optional ByVal magazzinoEsterno_Cod As String = "" _
                                , Optional ByVal magazzinoEsterno_Des As String = "" _
                                , Optional ByVal magazzinoEsterno_Dettagli As String = "" _
                                , Optional ByVal Sup_Riduzione_BufferZone As Decimal = 0 _
                                , Optional ByVal Perc_Riduzione_Deriva As Decimal = 0
                                        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Destinazioni_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try



            If Data_creazione = #2/1/1900# Then
                Data_creazione = DateTime.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = DateTime.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If





            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" INSERT INTO Ricette_Destinazioni ")
            StrSQL.Append("         ( ")
            StrSQL.Append("          Ricetta_SuperUser,      Ricetta_Cod,      Ricetta_Operazione_Cod, ")
            StrSQL.Append("          Ricetta_Dettaglio_Cod,                    Ricetta_Destinazione_Cod,       ")
            StrSQL.Append("          Programmazione_Entita_Cod,                Piva, Sa_Cod, Appezza, id_Reg, Qta, Qta2, QuotaDistribuzione, Tipo_Destinazione, MagazzinoEsterno_Cod, MagazzinoEsterno_Des, MagazzinoEsterno_Dettagli, ")

            StrSQL.Append("          Sup_Riduzione_BufferZone,            Perc_Riduzione_Deriva, ")

            StrSQL.Append("          Inviato,            DataInvio, ")
            StrSQL.Append("          Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("          UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("          Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("         ) ")

            StrSQL.Append(" VALUES ( ")
            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Ricetta_Cod))
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & " ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Ricetta_Dettaglio_Cod) & " ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Ricetta_Destinazione_Cod) & " ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & "  ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(Piva) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Appezza) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Reg) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Qta) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Qta2) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(QuotaDistribuzione) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Tipo_Destinazione) & "  ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(magazzinoEsterno_Cod) & "'  ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(magazzinoEsterno_Des) & "'  ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(magazzinoEsterno_Dettagli) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sup_Riduzione_BufferZone) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Perc_Riduzione_Deriva) & "  ")

            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")

            StrSQL.Append("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")



            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

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




    '##############################################################################################
    Public Function Modifica(
                            ByVal Ricetta_Cod As Int32,
                            ByVal Ricetta_Operazione_Cod As Int32,
                            ByVal Ricetta_Dettaglio_Cod As Int32,
                            ByVal Ricetta_Destinazione_Cod As Int32,
                            ByVal Programmazione_Entita_Cod As Int32,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Int32,
                            ByVal Appezza As Int32,
                            ByVal Id_Reg As Int32,
                            ByVal Qta As Decimal,
                            ByVal Qta2 As Decimal,
                            ByVal QuotaDistribuzione As Decimal,
                            ByVal Tipo_Destinazione As Integer,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                            ByVal xFiltroAggiuntivo As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            Optional ByVal magazzinoEsterno_Cod As String = "",
                            Optional ByVal magazzinoEsterno_Des As String = "",
                             Optional ByVal magazzinoEsterno_Dettagli As String = "",
                           Optional ByVal Sup_Riduzione_BufferZone As Decimal = 0,
                           Optional ByVal Perc_Riduzione_Deriva As Decimal = 0
                           ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Destinazioni_W.Modifica()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try


            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (Ricetta_SuperUser obbligatorio)")
            End If

            '---------------------------------------------

            StrSQL.Length = 0

            StrSQL.Append(" UPDATE Ricette_Destinazioni SET ")
            StrSQL.Append("    Programmazione_Entita_Cod    =  " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & "  ")
            StrSQL.Append("   ,Piva                         =  '" & Agro_SQL_SaveText(Piva) & "'  ")
            StrSQL.Append("   ,Sa_Cod                       =  " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append("   ,Appezza                      =  " & Agro_SQL_SaveNum(Appezza) & "  ")
            StrSQL.Append("   ,Id_Reg                       =  " & Agro_SQL_SaveNum(Id_Reg) & "  ")
            StrSQL.Append("   ,Qta                          =  " & Agro_SQL_SaveNum(Qta) & "   ")
            StrSQL.Append("   ,Qta2                          =  " & Agro_SQL_SaveNum(Qta2) & "   ")
            StrSQL.Append("   ,QuotaDistribuzione           =  " & Agro_SQL_SaveNum(QuotaDistribuzione) & "   ")
            StrSQL.Append("   ,tipo_destinazione           =  " & Agro_SQL_SaveNum(Tipo_Destinazione) & "   ")
            StrSQL.Append("   ,Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(DateTime.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append("   ,MagazzinoEsterno_Cod  =  '" & Agro_SQL_SaveText(magazzinoEsterno_Cod) & "'  ")
            StrSQL.Append("   ,MagazzinoEsterno_Des  =  '" & Agro_SQL_SaveText(magazzinoEsterno_Des) & "'  ")
            StrSQL.Append("   ,MagazzinoEsterno_Dettagli  =  '" & Agro_SQL_SaveText(magazzinoEsterno_Dettagli) & "'  ")
            StrSQL.Append("   ,Sup_Riduzione_BufferZone  =  " & Agro_SQL_SaveNum(Sup_Riduzione_BufferZone) & "  ")
            StrSQL.Append("   ,Perc_Riduzione_Deriva  =  " & Agro_SQL_SaveNum(Perc_Riduzione_Deriva) & "  ")

            StrSQL.Append(" WHERE  Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Ricetta_Cod <> 0 Then
                StrSQL.Append(" AND Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
            End If

            If Ricetta_Operazione_Cod <> 0 Then
                StrSQL.Append(" AND Ricetta_Operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & "   ")
            End If

            If Ricetta_Dettaglio_Cod <> 0 Then
                StrSQL.Append(" AND Ricetta_Dettaglio_Cod = " & Agro_SQL_SaveNum(Ricetta_Dettaglio_Cod) & "   ")
            End If

            If Ricetta_Destinazione_Cod <> 0 Then
                StrSQL.Append(" AND Ricetta_Destinazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Destinazione_Cod) & "   ")
            End If



            '----------------------------------------------------------------------
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



    '##############################################################################################
    Public Function Cancella(
                            ByVal Ricetta_Cod As Int32,
                            ByVal Ricetta_Operazione_Cod As Int32,
                            ByVal Ricetta_Dettaglio_Cod As Int32,
                            ByVal Ricetta_Destinazione_Cod As Int32,
                                ByVal xFiltroAggiuntivo As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Destinazioni_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Ricetta_SuperUser = ""
        '   Ricetta_Cod = 0
        '   Ricetta_Operazione_Cod = 0
        '   Ricetta_Dettaglio_Cod = 0
        '   Miscela_Cod = 0
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            'If Piva = "" Then
            '    Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            'End If

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Ricette_Destinazioni ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")

            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM Ricette_Destinazioni ")
                StrSQL.Append(" WHERE  1=1 ")

            End If

            If objParametri.PivaSuperUser <> "" Then
                StrSQL.Append(" AND Ricette_Destinazioni.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            End If

            If Ricetta_Cod <> 0 Then
                StrSQL.Append(" AND Ricette_Destinazioni.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
            End If

            If Ricetta_Operazione_Cod <> 0 Then
                StrSQL.Append(" AND Ricette_Destinazioni.Ricetta_Operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & "   ")
            End If

            If Ricetta_Dettaglio_Cod <> 0 Then
                StrSQL.Append(" AND Ricette_Destinazioni.Ricetta_Dettaglio_Cod = " & Agro_SQL_SaveNum(Ricetta_Dettaglio_Cod) & "   ")
            End If

            If Ricetta_Destinazione_Cod <> 0 Then
                StrSQL.Append(" AND Ricette_Destinazioni.Ricetta_Destinazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Destinazione_Cod) & "   ")
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
