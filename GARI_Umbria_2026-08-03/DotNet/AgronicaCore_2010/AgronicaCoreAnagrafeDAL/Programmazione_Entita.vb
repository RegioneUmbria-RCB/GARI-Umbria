Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports System.Text
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider

Public Class Programmazione_Entita_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function EF_Leggi(ByVal Programmazione_Entita_Cod As Integer,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Programmazione_Entita

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

        Dim xREt As Programmazione_Entita = (
            From ee In GiasContext.Programmazione_Entita
            Where ee.Programmazione_Entita_Cod = Programmazione_Entita_Cod
            Select ee).First()

        Return xREt

    End Function

    '################################################################################
    Public Function Anagrafica_ParticellexProgrammazioneEntita_Leggi(ByVal Piva As String,
                                                                     ByVal Programmazione_Cod As Integer,
                                                                     ByRef MessaggioErrore As String,
                                                                     ByVal xFiltroAggiuntivo As String,
                                                                     ByVal xOrderBy As String,
                                                                     ByRef objParametri As AgronicaCoreParametri
                                                                     ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_R.Anagrafica_ParticellexProgrammazioneEntita_Leggi()"

        Dim DT As New DataTable
        Dim StrSQL As New StringBuilder

        Try

            StrSQL.Length = 0


            StrSQL.Append(" SELECT DISTINCT IP.PIVA, IP.sa_cod, '' AS Sa_Nome, PC.PROV, PC.COM, PC.SEZIONE, PC.FOGLIO, PC.NUMERO, PC.SUBALTERNO, ")

            StrSQL.Append(" PE.Superficie, PE.Entita_Des, ")        'IP.TitoloPossesso, IP.Sup_Condotta,

            StrSQL.Append(" PE.Campo_Cod, PE.Appezza, PE.Id_Reg, ")

            'StrSQL.Append(" PE.Veg_Cod, ISNULL(SpecieVegetali.Veg_Des,'') AS Veg_Des, ")
            StrSQL.Append(" PE.Veg_Cod_Cliente AS Veg_Cod, ISNULL(CAC_Codifica_Veg_Cod.Descrizione,'') AS Veg_Des, ")

            'StrSQL.Append(" PE.Cul_Cod, ISNULL(Cultivar.Cul_Des, '') AS Cul_Des, ")
            StrSQL.Append(" PE.Cul_Cod_Cliente AS Cul_Cod, ISNULL(CAC_Codifica_Cultivar.Descrizione, '') AS Cul_Des, ")

            'StrSQL.Append(" PE.Grfi_Cod, ISNULL(GruppoFinalita.Grfi_Des,'') AS Grfi_Des, ")
            StrSQL.Append(" PE.Grfi_Cod, '' AS Grfi_Des, ")

            'StrSQL.Append(" PE.Id_Cod AS DestinazioneUso_Cod, ISNULL(Codici_Anagrafe.descrizione, '') AS DestinazioneUso_Des, ")
            StrSQL.Append(" PE.Id_Cod AS DestinazioneUso_Cod, '' AS DestinazioneUso_Des, ")

            StrSQL.Append(" PE.Validita_Inizio, PE.Validita_Fine, ")

            StrSQL.Append(" I.COMUNI_PROV AS Prov_Des, I.Localita AS Com_Des, PP.Superficie AS Sup_Coltura, PE.Programmazione_Entita_Cod, PE.Note, ")

            StrSQL.Append(" PC.Ettari, PC.Are, PC.Centiare,  PC.Validita_Inizio AS Particella_Inizio, PC.Validita_Fine AS Particella_Fine,")
            StrSQL.Append(" PE.Veg_Cod_Agea, PE.Cul_Cod_Agea, PE.Uso_Cod_Agea, PE.Occupazione_Cod_Agea, PE.Destinazione_Cod_Agea, PE.Qualita_Cod_Agea")

            StrSQL.Append("ISNULL( (SELECT TOP 1 Superficie ")
            StrSQL.Append("          FROM ParticelleCatastalixEleggibilitaParticelle EP ")
            StrSQL.Append("          WHERE EP.PROV = PC.PROV AND EP.COM = PC.COM AND ")
            StrSQL.Append("          EP.SEZIONE = PC.SEZIONE AND EP.FOGLIO = PC.FOGLIO AND ")
            StrSQL.Append("         EP.NUMERO = PC.NUMERO And EP.SUBALTERNO = PC.SUBALTERNO And EP.Eleggibilita_Cod = 1) ")
            StrSQL.Append("    , 0) AS Sup_Seminabile,  ")

            StrSQL.Append("ISNULL( (SELECT TOP 1 Superficie ")
            StrSQL.Append("          FROM ParticelleCatastalixEleggibilitaParticelle EP ")
            StrSQL.Append("          WHERE EP.PROV = PC.PROV AND EP.COM = PC.COM AND ")
            StrSQL.Append("          EP.SEZIONE = PC.SEZIONE AND EP.FOGLIO = PC.FOGLIO AND ")
            StrSQL.Append("         EP.NUMERO = PC.NUMERO And EP.SUBALTERNO = PC.SUBALTERNO And EP.Eleggibilita_Cod = 3) ")
            StrSQL.Append("    , 0) AS Sup_Unar,  ")

            StrSQL.Append("ISNULL( (SELECT TOP 1  PEC.Val_Cod ")
            StrSQL.Append("          FROM Programmazione_Entita_Codici PEC ")
            StrSQL.Append("          WHERE PE.Piva_SuperUser = PEC.Piva_SuperUser ")
            StrSQL.Append("          AND PE.Programmazione_Entita_Cod = PEC.Programmazione_Entita_Cod ")
            StrSQL.Append("	         AND PEC.Id_Cod = " & Agro_SQL_SaveNum(enum_DatiAnagrafici_CodiciAnagrafe.PianoColturale) & " ), '0') AS Modifica, ")

            StrSQL.Append("ISNULL( (SELECT TOP 1 IP_Sup.Sup_Condotta ")
            StrSQL.Append("          FROM ImpreseXParticelle IP_Sup ")
            StrSQL.Append("          WHERE IP.Piva = IP_Sup.Piva AND IP.Sa_Cod = IP_Sup.Sa_Cod ")
            StrSQL.Append("          AND IP.PROV = IP_Sup.PROV And IP.COM = IP_Sup.COM And ")
            StrSQL.Append("         IP.SEZIONE = IP_Sup.SEZIONE AND IP.FOGLIO = IP_Sup.FOGLIO AND ")
            StrSQL.Append("         IP.NUMERO = IP_Sup.NUMERO AND IP.SUBALTERNO = IP_Sup.SUBALTERNO ")
            StrSQL.Append("         ORDER BY Validita_Fine DESC), '0') AS Sup_Condotta, ")

            StrSQL.Append("ISNULL( (SELECT TOP 1 IP_T.TitoloPossesso ")
            StrSQL.Append("          FROM ImpreseXParticelle IP_T ")
            StrSQL.Append("          WHERE IP.Piva = IP_T.Piva AND IP.Sa_Cod = IP_T.Sa_Cod ")
            StrSQL.Append("          AND IP.PROV = IP_T.PROV And IP.COM = IP_T.COM And ")
            StrSQL.Append("         IP.SEZIONE = IP_T.SEZIONE AND IP.FOGLIO = IP_T.FOGLIO AND ")
            StrSQL.Append("         IP.NUMERO = IP_T.NUMERO AND IP.SUBALTERNO = IP_T.SUBALTERNO ")
            StrSQL.Append("         ORDER BY Validita_Fine DESC), '0') AS TitoloPossesso ")

            StrSQL.Append(" FROM ImpreseXParticelle AS IP ")
            StrSQL.Append(" INNER JOIN ParticelleCatastali PC ON IP.PROV = PC.PROV AND IP.COM = PC.COM AND ")
            StrSQL.Append(" IP.SEZIONE = PC.SEZIONE AND IP.FOGLIO = PC.FOGLIO AND ")
            StrSQL.Append(" IP.NUMERO = PC.NUMERO AND IP.SUBALTERNO = PC.SUBALTERNO ")
            StrSQL.Append(" INNER JOIN ISTAT I ON PC.Prov = I.PROV AND PC.COM = I.COM ")
            StrSQL.Append(" LEFT OUTER JOIN Programmazione_Particelle PP ")
            StrSQL.Append(" ON PP.Prov = PC.PROV AND PP.Com = PC.COM AND PP.Sezione = PC.SEZIONE AND ")
            StrSQL.Append(" PP.Foglio = PC.FOGLIO And PP.Numero = PC.NUMERO And PP.Subalterno = PC.SUBALTERNO ")
            StrSQL.Append(" LEFT OUTER JOIN Programmazione_Entita AS PE ON PE.Programmazione_Entita_Cod = PP.Programmazione_Entita_Cod ")

            'StrSQL.Append(" LEFT OUTER JOIN Copertura ON PE.Cop_Cod = Copertura.Cop_Cod ")
            'StrSQL.Append(" LEFT OUTER JOIN GruppoVarietale ON PE.Grva_Cod = GruppoVarietale.Grva_Cod ")
            'StrSQL.Append(" LEFT OUTER JOIN GruppoFinalita ON PE.Grfi_Cod = GruppoFinalita.Grfi_Cod ")
            'StrSQL.Append(" LEFT OUTER JOIN Cultivar ON PE.Cul_Cod = Cultivar.Cul_Cod ")
            'StrSQL.Append(" LEFT OUTER JOIN SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod ")

            StrSQL.Append(" LEFT OUTER JOIN CAC_Codifica_Veg_Cod ON PE.Veg_Cod_Cliente = CAC_Codifica_Veg_Cod.Veg_Cod_Coltiva_2 ")
            StrSQL.Append(" LEFT OUTER JOIN CAC_Codifica_Cultivar ON PE.Cul_Cod_Cliente = CAC_Codifica_Cultivar.Cultivar_Coltiva_2 ")

            StrSQL.Append(" WHERE   (PE.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "')   ")
            StrSQL.Append(" AND     (IP.Piva = '" & Agro_SQL_SaveText(Piva) & "')   ")

            If Programmazione_Cod <> 0 Then
                StrSQL.Append(" AND     (PE.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod.ToString) & ")  ")
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   IP.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   IP.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else

                StrSQL.Append(" ORDER BY IP.PIVA, IP.sa_cod, Sa_Nome, PC.PROV, PC.COM, PC.SEZIONE, PC.FOGLIO, PC.NUMERO, PC.SUBALTERNO,  ")
                StrSQL.Append(" PE.Superficie, PE.Entita_Des,  PE.Campo_Cod, PE.Appezza, PE.Id_Reg,  Veg_Cod,  ")
                StrSQL.Append(" Veg_Des,  Cul_Cod, Cul_Des,  PE.Grfi_Cod, Grfi_Des, DestinazioneUso_Cod, DestinazioneUso_Des,  PE.Validita_Inizio, PE.Validita_Fine,  ")
                StrSQL.Append(" Prov_Des, Com_Des, Sup_Coltura, PE.Programmazione_Entita_Cod, PE.Note,   ")
                StrSQL.Append(" PC.Ettari, PC.Are, PC.Centiare ")

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

    Public Function PianteLeggiCodiceOrdineCollegatoARichiesta(
        ByVal Programmazione_Entita_Cod As Integer,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreParametri
        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_R.PianteLeggiCodiceOrdineCollegatoARichiesta()"

        Dim DT As New DataTable
        Dim stb As New StringBuilder
        Dim MessaggioErrore As String

        Try


            stb.Length = 0
            stb.AppendLine(" select top 1  ")
            stb.AppendLine("     eOrdine.Riferimento_Alfanumerico_Appezzamento as CodiceOrdine ")
            stb.AppendLine(" from ( ")
            stb.AppendLine("  select riferimento_Alfanumerico_Appezzamento ")
            stb.AppendLine("  from programmazione_entita ")
            stb.AppendLine("  where programmazione_Entita_Cod = " & Programmazione_Entita_Cod)
            stb.AppendLine(" ) daRif ")
            stb.AppendLine("  inner join programmazione_entita eRif ")
            stb.AppendLine("      on daRif.Riferimento_Alfanumerico_Appezzamento = eRif.Riferimento_Alfanumerico_Appezzamento ")
            stb.AppendLine("      and eRif.Programmazione_Entita_Cod <>  " & Programmazione_Entita_Cod)
            stb.AppendLine("  inner join programmazione_testata tRichiesta ")
            stb.AppendLine("      on eRif.Programmazione_Cod = tRichiesta.Programmazione_Cod ")
            stb.AppendLine("  inner join programmazione_testata tOrdine ")
            stb.AppendLine("      on  tRichiesta.Programmazione_Cod_Padre = tOrdine.Programmazione_Cod ")
            stb.AppendLine("  inner join Programmazione_Entita eOrdine ")
            stb.AppendLine("      on eOrdine.Programmazione_Cod = tOrdine.Programmazione_Cod")

            If xFiltroAggiuntivo <> "" Then
                stb.Append(" WHERE " & xFiltroAggiuntivo)
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function


    '################################################################################
    Public Function Anagrafica_AppezzaCampo_Leggi(ByVal Programmazione_Cod As Integer,
                                                        ByVal Campo_Cod As Integer,
                                                        ByRef MessaggioErrore As String,
                                                            ByVal xFiltroAggiuntivo As String,
                                                            ByVal xOrderBy As String,
                                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                            ) As DataTable
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_R.Anagrafica_AppezzaCampo_Leggi()"

        Dim DT As New DataTable

        Dim StrSQL As New System.Text.StringBuilder

        Try

            'modificare la query di select
            StrSQL.Length = 0

            StrSQL.Append(" SELECT PE.Superficie, PE.Entita_Des, PE.Programmazione_Entita_Cod, ")
            StrSQL.Append(" PE.Campo_Cod, PE.Appezza, PE.Id_Reg, ")
            StrSQL.Append(" PE.Veg_Cod_Cliente AS Veg_Cod, PE.Cul_Cod_Cliente AS Cul_Cod, ")

            'StrSQL.Append(" ISNULL( (SELECT TOP 1 CAC_Codifica_Veg_Cod.Descrizione ")
            'StrSQL.Append("          FROM CAC_Codifica_Veg_Cod ")
            'StrSQL.Append("          WHERE CAC_Codifica_Veg_Cod.Veg_Cod_Coltiva_2 = PE.Veg_Cod_Cliente ), ''  ) AS Veg_Des, ")

            StrSQL.Append(" ISNULL( (SELECT TOP 1 CAC_Codifica_Colture.Descrizione_Coltura ")
            StrSQL.Append("          FROM CAC_Codifica_Colture ")
            StrSQL.Append("          WHERE (CAC_Codifica_Colture.COD_UTILIZZO + CAC_Codifica_Colture.COD_COLTURA) = PE.Veg_Cod_Cliente ), ''  ) AS Veg_Des, ")

            'StrSQL.Append(" ISNULL( (SELECT TOP 1 CAC_Codifica_Cultivar.Descrizione ")
            'StrSQL.Append("          FROM CAC_Codifica_Cultivar ")
            'StrSQL.Append("          WHERE CAC_Codifica_Cultivar.Cultivar_Coltiva_2 = PE.Cul_Cod_Cliente ), ''  ) AS Cul_Des, ")

            StrSQL.Append(" ISNULL( (SELECT TOP 1 CAC_Codifica_Colture.Descrizione_Varieta ")
            StrSQL.Append("          FROM CAC_Codifica_Colture ")
            StrSQL.Append("          WHERE (CAC_Codifica_Colture.COD_UTILIZZO + CAC_Codifica_Colture.COD_COLTURA + CAC_Codifica_Colture.COD_VARIETA) = PE.Cul_Cod_Cliente ), ''  ) AS Cul_Des, ")


            StrSQL.Append(" PE.Grfi_Cod, '' AS Grfi_Des, ")
            StrSQL.Append(" PE.Id_Cod AS DestinazioneUso_Cod, '' AS DestinazioneUso_Des, ")
            StrSQL.Append(" PE.Validita_Inizio, PE.Validita_Fine ")

            StrSQL.Append(" PE.Veg_Cod_Agea, PE.Cul_Cod_Agea, PE.Uso_Cod_Agea, PE.Occupazione_Cod_Agea, PE.Destinazione_Cod_Agea, PE.Qualita_Cod_Agea")

            StrSQL.Append(" FROM Programmazione_Entita AS PE ")


            StrSQL.Append(" WHERE   (PE.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "')   ")
            StrSQL.Append(" AND     (PE.Programmazione_Cod = " & Agro_SQL_SaveText(Programmazione_Cod.ToString) & ")  ")
            StrSQL.Append(" AND     (PE.Campo_Cod = " & Agro_SQL_SaveText(Campo_Cod.ToString) & ")  ")
            StrSQL.Append(" AND     (PE.Appezza <> 0 )")


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   PE.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   PE.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY PE.Validita_Inizio ")
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


    '################################################################################
    Public Function Programmazione_Specie_Leggi(ByVal Piva_SuperUser As String,
                                                ByVal Programmazione_Cod As Integer,
                                                ByRef MessaggioErrore As String,
                                                ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreParametri
                                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_R.Programmazione_Specie_Leggi()"


        Dim DT As DataTable
        Dim StrSQL As New StringBuilder

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT  DISTINCT CAC_Codifica_Veg_Cod.Veg_Cod_Coltiva_2 AS Veg_Cod, CAC_Codifica_Veg_Cod.Descrizione AS Veg_Des ")
                    StrSQL.Append(" FROM    Programmazione_Entita INNER JOIN ")
                    StrSQL.Append(" CAC_Codifica_Veg_Cod ON Programmazione_Entita.Veg_Cod_Cliente = CAC_Codifica_Veg_Cod.Veg_Cod_Coltiva_2 ")

                    StrSQL.Append(" WHERE   Programmazione_Entita.Piva_SuperUser = '" & Agro_SQL_SaveText(Piva_SuperUser) & "' ")

                    If Programmazione_Cod <> 0 Then
                        StrSQL.Append(" AND Programmazione_Entita.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod.ToString))
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Programmazione_Entita.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Programmazione_Entita.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Veg_Cod ")
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT  DISTINCT CAC_Codifica_Veg_Cod.Veg_Cod_Coltiva_2 AS Veg_Cod, CAC_Codifica_Veg_Cod.Descrizione AS Veg_Des ")
                    StrSQL.Append(" FROM    Programmazione_Entita INNER JOIN ")
                    StrSQL.Append(" CAC_Codifica_Veg_Cod ON Programmazione_Entita.Veg_Cod_Cliente = CAC_Codifica_Veg_Cod.Veg_Cod_Coltiva_2 ")

                    StrSQL.Append(" WHERE   Programmazione_Entita.Piva_SuperUser = '" & Agro_SQL_SaveText(Piva_SuperUser) & "' ")

                    If Programmazione_Cod <> 0 Then
                        StrSQL.Append(" AND Programmazione_Entita.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod.ToString))
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Programmazione_Entita.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Programmazione_Entita.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Veg_Cod ")
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni



                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta


            End Select

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


    '################################################################################
    Public Function Programmazione_Entita_Leggi(
                                            ByVal Programmazione_Cod As Integer,
                                            ByRef MessaggioErrore As String,
                                            ByVal Programmazione_Entita_Cod As Integer,
                                            ByVal Entita_Des As String,
                                            ByVal Piva As String,
                                            ByVal Sa_Cod As Integer,
                                            ByVal Campo_Cod As Integer,
                                            ByVal Appezza As Integer,
                                            ByVal Id_Reg As Integer,
                                            ByVal Progetto_Cod As Integer,
                                            ByVal Validita_Inizio As Date,
                                            ByVal Validita_Fine As Date,
                                            ByVal SORT_Des1_Inizio2_Fine2 As Integer,
                                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                    Optional ByVal Fonte_Cod As Integer = 0,
                                            Optional ByVal RicavaAppRibaltati As Boolean = False,
                                            Optional ByVal Lettura_Per_UMA As Boolean = False
                                                ) As DataTable
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_R.Programmazione_Entita_Leggi()"


        Dim DT As DataTable

        Dim StrSQL As New System.Text.StringBuilder

        Try

            StrSQL.Length = 0

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT  Programmazione_Entita.*, ")
            StrSQL.AppendLine("         ISNULL(Centri_Aziendali.sa_nome,'') AS sa_nome, ")
            StrSQL.AppendLine("         ISNULL(Campi.Campo_Des,'') AS Campo_Des, ")
            StrSQL.AppendLine("         ISNULL(Programmazione_Entita.Grva_Cod,0) AS Grva_Cod, ")
            StrSQL.AppendLine("         ISNULL(SpecieVegetali.Veg_Des,'') AS Veg_Des, ")
            StrSQL.AppendLine("         ISNULL(Cultivar.Cul_Des,'') AS Cul_Des, ")
            StrSQL.AppendLine("         ISNULL(GruppoVarietale.Grva_Des,'') AS Grva_Des, ")
            StrSQL.AppendLine("         ISNULL(GruppoFinalita.Grfi_Des,'') AS Grfi_Des, ")
            StrSQL.AppendLine("         ISNULL(Copertura.Cop_Des,'') AS Cop_Des, ")
            StrSQL.AppendLine("         ISNULL(Codici_Anagrafe.descrizione,'') AS DestinazioneUso_Des, ")
            StrSQL.AppendLine("         Programmazione_Testata.Programmazione_Des, ")
            StrSQL.AppendLine("         Programmazione_Testata.Validita_Inizio AS Programmazione_Validita_Inizio, ")
            StrSQL.AppendLine("         Programmazione_Testata.Validita_Fine AS Programmazione_Validita_Fine, ")
            StrSQL.AppendLine("         Programmazione_Testata.Tipo_Pianificazione, ")

            StrSQL.AppendLine("         CASE Programmazione_Entita.MetodoProduzione_Cod WHEN 1 THEN 'Integrato' WHEN 2 THEN 'In Conversione' ")
            StrSQL.Append("                                                WHEN 3 THEN 'Biologico' ELSE 'Altro' END AS MetodoProduzione_Des, ")

            StrSQL.AppendLine("         ISNULL(Macrousi.Macrouso_Des,'') AS Macrouso_Des, " & vbCrLf)

            StrSQL.Append("         ISNULL(Programmazione_Entita.Unita_Vitata,0) AS Unita_Vitata " & vbCrLf)

            'aggiunta in data 4/7/13 da maga

            StrSQL.AppendLine("  , Case WHEN ISNULL(I.partitaIvaReale, '') = '' THEN I.PIVA ELSE I.partitaIvaReale END AS partitaIvaReale ")
            'aggiunta 27/05/2026 Casadei

            Select Case Fonte_Cod

                Case enum_Planning_Fonte.Agrea
                    StrSQL.Append("         , ISNULL( (SELECT TOP 1 Veg_Des_Ente + '|' + isnull(Cul_Des_Ente,'') as Cul_Des_Agrea  " & vbCrLf)
                    StrSQL.Append("                     FROM Codifica_SpecieVegetali_Enti_2015_2020 " & vbCrLf)
                    StrSQL.Append("                     WHERE Codifica_SpecieVegetali_Enti_2015_2020.Veg_Cod_Ente = Programmazione_Entita.veg_cod_cliente " & vbCrLf)
                    StrSQL.Append("                     AND Codifica_SpecieVegetali_Enti_2015_2020.Cul_Cod_Ente = Programmazione_Entita.cul_cod_cliente " & vbCrLf)
                    StrSQL.Append("                     AND Ente_Cod = 2 " & vbCrLf)
                    StrSQL.Append("                     ), '|') AS VegCul_Des_Cliente " & vbCrLf)

                Case enum_Planning_Fonte.Avepa
                    'StrSQL.Append("         , ISNULL( (SELECT Veg_Des_Avepa + '|' + isnull(Cul_Des_Avepa,'') as Cul_Des_Avepa " & vbCrLf)
                    'StrSQL.Append("                     FROM Codifica_SpecieVegetali_Avepa " & vbCrLf)
                    'StrSQL.Append("                     WHERE Codifica_SpecieVegetali_Avepa.Veg_Cod_Avepa = Programmazione_Entita.veg_cod_cliente " & vbCrLf)
                    'StrSQL.Append("                     AND Codifica_SpecieVegetali_Avepa.Cul_Cod_Avepa = Programmazione_Entita.cul_cod_cliente " & vbCrLf)
                    'StrSQL.Append("                     ), '|') AS VegCul_Des_Cliente " & vbCrLf)
                    StrSQL.Append("         , ISNULL( (SELECT TOP 1 Veg_Des_Ente + '|' + isnull(Cul_Des_Ente,'') as Cul_Des_Agrea  " & vbCrLf)
                    StrSQL.Append("                     FROM Codifica_SpecieVegetali_Enti_2015_2020 " & vbCrLf)
                    StrSQL.Append("                     WHERE Codifica_SpecieVegetali_Enti_2015_2020.Veg_Cod_Ente = Programmazione_Entita.veg_cod_cliente " & vbCrLf)
                    StrSQL.Append("                     AND Codifica_SpecieVegetali_Enti_2015_2020.Cul_Cod_Ente = Programmazione_Entita.cul_cod_cliente " & vbCrLf)
                    StrSQL.Append("                     AND Ente_Cod = 9 " & vbCrLf)
                    StrSQL.Append("                     ), '|') AS VegCul_Des_Cliente " & vbCrLf)

                Case enum_Planning_Fonte.Artea
                    'StrSQL.Append("         , ISNULL( (SELECT Veg_Des_Artea + '|' + isnull(Cul_Des_Artea,'') as Cul_Des_Artea " & vbCrLf)
                    'StrSQL.Append("                     FROM Codifica_SpecieVegetali_Artea " & vbCrLf)
                    'StrSQL.Append("                     WHERE Codifica_SpecieVegetali_Artea.Veg_Cod_Artea = Programmazione_Entita.veg_cod_cliente " & vbCrLf)
                    'StrSQL.Append("                     AND Codifica_SpecieVegetali_Artea.Cul_Cod_Artea = Programmazione_Entita.cul_cod_cliente " & vbCrLf)
                    'StrSQL.Append("                     ), '|') AS VegCul_Des_Cliente " & vbCrLf)
                    StrSQL.Append("         , ISNULL( (SELECT TOP 1 Veg_Des_Ente + '|' + isnull(Cul_Des_Ente,'') as Cul_Des_Agrea  " & vbCrLf)
                    StrSQL.Append("                     FROM Codifica_SpecieVegetali_Enti_2015_2020 " & vbCrLf)
                    StrSQL.Append("                     WHERE Codifica_SpecieVegetali_Enti_2015_2020.Veg_Cod_Ente = Programmazione_Entita.veg_cod_cliente " & vbCrLf)
                    StrSQL.Append("                     AND Codifica_SpecieVegetali_Enti_2015_2020.Cul_Cod_Ente = Programmazione_Entita.cul_cod_cliente " & vbCrLf)
                    StrSQL.Append("                     AND Ente_Cod = 11 " & vbCrLf)
                    StrSQL.Append("                     ), '|') AS VegCul_Des_Cliente " & vbCrLf)

                Case Else
                    StrSQL.Append("         , ISNULL( (SELECT Veg_Des_Agea + '|' + isnull(Cul_Des_Agea,'') as Cul_Des_Agea " & vbCrLf)
                    StrSQL.Append("                     FROM Codifica_SpecieVegetali_Agea " & vbCrLf)
                    StrSQL.Append("                     WHERE Codifica_SpecieVegetali_Agea.Veg_Cod_Agea = Programmazione_Entita.veg_cod_cliente " & vbCrLf)
                    StrSQL.Append("                     AND Codifica_SpecieVegetali_Agea.Cul_Cod_Agea = Programmazione_Entita.cul_cod_cliente " & vbCrLf)
                    StrSQL.Append("                     ), '|') AS VegCul_Des_Cliente " & vbCrLf)
            End Select

            StrSQL.AppendLine("                     , Codice_Fiscale_Tecnico " & vbCrLf)
            StrSQL.AppendLine("                     ,ISNULL(GIS_ElementiGrafici.ElementoGrafico_Cod,0) as ElementoGrafico_Cod")
            StrSQL.AppendLine("                     ,ISNULL(GIS_ElementiGrafici.ElementoGrafico_des, '') as ElementoGrafico_des")
            StrSQL.Append("         " & vbCrLf)
            StrSQL.Append("         " & vbCrLf)

            If RicavaAppRibaltati Then
                StrSQL.Append("                     ,ISNULL(Reg_Impianti_Programmazioni.piva, '') as piva_ribaltato" & vbCrLf)
                StrSQL.Append("                     ,ISNULL(Reg_Impianti_Programmazioni.sa_cod, 0) as sa_cod_ribaltato" & vbCrLf)
                StrSQL.Append("                     ,ISNULL(Reg_Impianti_Programmazioni.appezza, 0) as appezza_ribaltato" & vbCrLf)
                StrSQL.Append("                     ,ISNULL(Reg_Impianti_Programmazioni.id_reg, 0) as id_reg_ribaltato" & vbCrLf)
            End If

            If Not Lettura_Per_UMA Then
                StrSQL.AppendLine(" , case when gEnt.entita_cod is null then '0' else '1' end as datoGis  ")
            End If

            StrSQL.AppendLine(" , cp.InfoAgg_Des as CapitolatoPrivato_Des ")

            'INDIRIZZO
            StrSQL.AppendLine(" , ISNULL(Indirizzi.ind_des, '') as ind_des, ISNULL(Indirizzi.frz_des, '') as frz_des, ISNULL(Indirizzi.CAP, '') as CAP, ISNULL(indirizzi.stato, '') as stato_indirizzo ")
            StrSQL.AppendLine(" , ISNULL(indirizzi.note, '') as note_indirizzo, ISNULL(indirizzi.pro_cod_istat, '') as pro_cod_istat_indirizzo, ISNULL(indirizzi.com_cod_istat, '') as com_cod_istat_indirizzo  ")
            StrSQL.AppendLine(" , ISNULL(ISTAT.COMUNI_PROV, '') as pro_cod_indirizzo, ISNULL(ISTAT.LOCALITA, '') as com_des_indirizzo, ISNULL(ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166.Descrizione, '') as stato_indirizzo_des ")

            StrSQL.AppendLine(" , KPIN.Val_Cod as KPIN, Block_Name.Val_Cod as Block_Name ")

            StrSQL.AppendLine(" , ISNULL(FormeAllevamento.Foral_Des, '') as Foral_Des ")
            StrSQL.AppendLine(" , ISNULL(Data_Inizio_Portinnesto.val_cod, '') as Data_Inizio_Portinnesto ")

            'FASI ZESPRI
            StrSQL.AppendLine(" , ISNULL(ZespriFase1.val_cod, 0) as ZespriFase_Cod ")
            StrSQL.AppendLine(" , ISNULL(ZespriFase1T.Descrizione, '') as ZespriFase_Des ")

            StrSQL.AppendLine(" , ISNULL(ZespriFase2.val_cod, 0) as ZespriGrower_Cod ")
            StrSQL.AppendLine(" , ISNULL(ZespriFase2T.Descrizione, '') as ZespriGrower_Des ")

            StrSQL.AppendLine(" , ISNULL(ZespriFase3.val_cod, 0) as ZespriTipo_Cod ")
            StrSQL.AppendLine(" , ISNULL(ZespriFase3T.Descrizione, '') as ZespriTipo_Des ")

            'NUM PIANTE MF
            StrSQL.AppendLine(" , ISNULL(NFemmine.val_cod, 0) as Num_Piante_Femmine ")
            StrSQL.AppendLine(" , ISNULL(NMaschi.val_cod, 0) as Num_Piante_Maschi ")

            'PORTINNESTO
            StrSQL.AppendLine(" , ISNULL(Programmazione_Entita.Port_Cod, 0) as Port_Cod ")
            StrSQL.AppendLine(" , ISNULL(Portinnesti.Port_Des, '') as Port_Des ")

            'Tipologia di innesto o trapianto
            StrSQL.AppendLine(", ISNULL(TIT.Val_Cod, 0 ) as TipologiaInnestoTrapianto_cod ")
            StrSQL.AppendLine(", ISNULL(TITAnag.TipologiaInnestoTrapianto_des, '' ) as TipologiaInnestoTrapianto_des ")

            'Tipologia di innesto o trapianto
            StrSQL.AppendLine(", COALESCE(Materie_Prime.Mat_Cod, 0 ) as Mat_Cod")
            StrSQL.AppendLine(", COALESCE(Materie_Prime.Cod_Articolo + ' - ' + Materie_Prime.Mat_Des, '') as Mat_Des ")


            'INIZIO JOIN
            StrSQL.AppendLine(" FROM    Programmazione_Entita INNER JOIN ")
            StrSQL.AppendLine(" Programmazione_Testata ON Programmazione_Entita.Programmazione_Cod = Programmazione_Testata.Programmazione_Cod ")
            StrSQL.AppendLine(" AND Programmazione_Entita.Piva_SuperUser = Programmazione_Testata.Piva_SuperUser LEFT OUTER JOIN ")
            StrSQL.AppendLine(" Codici_Anagrafe ON Programmazione_Entita.Id_Cod = Codici_Anagrafe.codice LEFT OUTER JOIN ")
            StrSQL.AppendLine(" Copertura ON Programmazione_Entita.Cop_Cod = Copertura.Cop_Cod LEFT OUTER JOIN ")
            StrSQL.AppendLine(" GruppoVarietale ON Programmazione_Entita.Grva_Cod = GruppoVarietale.Grva_Cod LEFT OUTER JOIN ")
            StrSQL.AppendLine(" GruppoFinalita ON Programmazione_Entita.Grfi_Cod = GruppoFinalita.Grfi_Cod LEFT OUTER JOIN ")
            StrSQL.AppendLine(" SpecieVegetali ON Programmazione_Entita.Veg_Cod = SpecieVegetali.Veg_Cod LEFT OUTER JOIN ")
            StrSQL.AppendLine(" Cultivar ON Programmazione_Entita.Cul_Cod = Cultivar.Cul_Cod LEFT OUTER JOIN ")
            StrSQL.AppendLine(" Macrousi ON Programmazione_Entita.Macrouso_Cod = Macrousi.Macrouso_Cod LEFT OUTER JOIN")
            StrSQL.AppendLine(" portinnesti ON Programmazione_Entita.Port_Cod = Portinnesti.Port_Cod ")

            StrSQL.AppendLine(" LEFT JOIN Materie_Prime ON Programmazione_Entita.Mat_Cod = Materie_Prime.Mat_Cod ")

            StrSQL.AppendLine(" LEFT JOIN Imprese I on Programmazione_Entita.Piva = I.PIVA ")

            StrSQL.AppendLine(" LEFT OUTER JOIN ")
            StrSQL.AppendLine("           Campi ON Programmazione_Entita.Campo_Cod = Campi.Campo_Cod AND Programmazione_Entita.Piva = Campi.Piva AND  ")
            StrSQL.AppendLine("           Programmazione_Entita.Sa_Cod = Campi.Sa_Cod LEFT OUTER JOIN ")
            StrSQL.AppendLine("           Centri_Aziendali ON Programmazione_Entita.Piva = Centri_Aziendali.PIVA AND Programmazione_Entita.Sa_Cod = Centri_Aziendali.sa_cod " & vbCrLf)
            StrSQL.AppendLine("           LEFT OUTER JOIN GIS_ElementiGrafici ON Programmazione_Entita.Programmazione_Entita_Cod = GIS_ElementiGrafici.Entita_Cod " & vbCrLf)

            If RicavaAppRibaltati Then
                StrSQL.Append(" LEFT OUTER JOIN ")
                StrSQL.Append("         Reg_Impianti_Programmazioni ON Programmazione_Entita.Piva_SuperUser = Reg_Impianti_Programmazioni.Piva_SuperUser  " & vbCrLf)
                StrSQL.Append("          AND Programmazione_Entita.Programmazione_Cod = Reg_Impianti_Programmazioni.Programmazione_Cod  " & vbCrLf)
                StrSQL.Append("          AND Programmazione_Entita.Programmazione_Entita_Cod = Reg_Impianti_Programmazioni.Programmazione_Entita_Cod " & vbCrLf)
            End If

            If Not Lettura_Per_UMA Then
                StrSQL.AppendLine(" LEFT JOIN GIS_Entita gEnt on gEnt.Programmazione_entita_Cod = Programmazione_Entita.Programmazione_entita_Cod ")
            End If

            StrSQL.AppendLine(" LEFT JOIN CAC_Codifica_InfoAggiuntive cp on cp.InfoAgg_Cod = Programmazione_Entita.CapitolatoPrivato AND cp.Argomento_Cod = 1 ")

            'INDIRIZZO
            StrSQL.AppendLine(" LEFT JOIN Indirizzi on INDIRIZZI.Cod_Indirizzo = Programmazione_Entita.Cod_Indirizzo ")
            StrSQL.AppendLine(" LEFT JOIN ISTAT on INDIRIZZI.pro_cod_istat = ISTAT.PROV AND INDIRIZZI.com_cod_istat = ISTAT.COM ")
            StrSQL.AppendLine(" LEFT JOIN ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166 on INDIRIZZI.stato = ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166.Codice ")

            StrSQL.AppendLine(" LEFT JOIN Programmazione_Entita_Codici KPIN ON Programmazione_Entita.Programmazione_Entita_Cod = KPIN.Programmazione_Entita_Cod AND KPIN.id_cod = " & enum_CodiciAnagrafe.Zespri_Codice_kPIN & " ")
            StrSQL.AppendLine(" LEFT JOIN Programmazione_Entita_Codici Block_Name ON Programmazione_Entita.Programmazione_Entita_Cod = Block_Name.Programmazione_Entita_Cod AND Block_Name.id_cod = " & enum_CodiciAnagrafe.Zespri_Block_Name & " ")

            StrSQL.AppendLine(" LEFT JOIN FormeAllevamento ON FormeAllevamento.Foral_Cod = Programmazione_Entita.Foral_Cod ")

            StrSQL.AppendLine(" LEFT JOIN Programmazione_Entita_Codici Data_Inizio_Portinnesto ON Programmazione_Entita.Programmazione_Entita_Cod = Data_Inizio_Portinnesto.Programmazione_Entita_Cod AND Data_Inizio_Portinnesto.ID_Cod = " & enum_CodiciAnagrafe.Data_Inizio_Portinnesto & " ")

            'FASI ZESPRI
            StrSQL.AppendLine(" LEFT JOIN Programmazione_Entita_Codici ZespriFase1 ON Programmazione_Entita.Programmazione_Entita_Cod = ZespriFase1.Programmazione_Entita_Cod AND ZespriFase1.id_cod = " & enum_CodiciAnagrafe.Zespri_Fasi_Fase & " ")
            StrSQL.AppendLine(" LEFT JOIN OTabelle_Parametri ZespriFase1T ON ZespriFase1.val_cod = ZespriFase1T.Tabella_Par_Cod AND ZespriFase1T.Tabella_Cod = " & enum_CodiciAnagrafe.Zespri_Fasi_Fase & " ")

            StrSQL.AppendLine(" LEFT JOIN Programmazione_Entita_Codici ZespriFase2 ON Programmazione_Entita.Programmazione_Entita_Cod = ZespriFase2.Programmazione_Entita_Cod AND ZespriFase2.id_cod = " & enum_CodiciAnagrafe.Zespri_Fasi_Grower & " ")
            StrSQL.AppendLine(" LEFT JOIN OTabelle_Parametri ZespriFase2T ON ZespriFase2.val_cod = ZespriFase2T.Tabella_Par_Cod AND ZespriFase2T.Tabella_Cod = " & enum_CodiciAnagrafe.Zespri_Fasi_Grower & " ")

            StrSQL.AppendLine(" LEFT JOIN Programmazione_Entita_Codici ZespriFase3 ON Programmazione_Entita.Programmazione_Entita_Cod = ZespriFase3.Programmazione_Entita_Cod AND ZespriFase3.id_cod = " & enum_CodiciAnagrafe.Zespri_Fasi_Tipo & " ")
            StrSQL.AppendLine(" LEFT JOIN OTabelle_Parametri ZespriFase3T ON ZespriFase3.val_cod = ZespriFase3T.Tabella_Par_Cod AND ZespriFase3T.Tabella_Cod = " & enum_CodiciAnagrafe.Zespri_Fasi_Tipo & " ")

            'NUM PIANTE MF
            StrSQL.AppendLine(" LEFT JOIN Programmazione_Entita_Codici NFemmine ON Programmazione_Entita.Programmazione_Entita_Cod = NFemmine.Programmazione_Entita_Cod AND NFemmine.id_cod = " & enum_CodiciAnagrafe.Num_Piante_Femmine & " ")
            StrSQL.AppendLine(" LEFT JOIN Programmazione_Entita_Codici NMaschi ON Programmazione_Entita.Programmazione_Entita_Cod = NMaschi.Programmazione_Entita_Cod AND NMaschi.id_cod = " & enum_CodiciAnagrafe.Num_Piante_Maschi & " ")

            'Tipologia di innesto o trapianto
            StrSQL.AppendLine(" LEFT JOIN Programmazione_Entita_Codici TIT ON Programmazione_Entita.Programmazione_Entita_Cod = TIT.Programmazione_Entita_Cod AND TIT.id_cod = " & enum_CodiciAnagrafe.TipologiaDIInnestoTrapianto & " ")
            StrSQL.AppendLine(" LEFT JOIN ( ")

            StrSQL.AppendLine(" Select 1 As TipologiaInnestoTrapianto_cod,'innesto su portinnesto dell''anno' as TipologiaInnestoTrapianto_des ")
            StrSQL.AppendLine("     union ")
            StrSQL.AppendLine(" Select 2,'innesto su portinnesto > 3 anni' ")
            StrSQL.AppendLine("     union ")
            StrSQL.AppendLine(" Select 3,'trapianto' ")
            StrSQL.AppendLine("     union ")
            StrSQL.AppendLine(" Select 4,'innesto su portinnesto da 1 a 3 anni' ")
            StrSQL.AppendLine("     union ")
            StrSQL.AppendLine(" Select 5,'INNESTO'")

            StrSQL.AppendLine(" ) TITAnag ON  TIT.Val_cod = TITAnag.TipologiaInnestoTrapianto_cod ")

            StrSQL.AppendLine(" WHERE   Programmazione_Entita.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine(" AND     Programmazione_Entita.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.AppendLine(" AND     Programmazione_Entita.Validita_fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")


            If Programmazione_Cod <> 0 Then
                StrSQL.AppendLine(" AND Programmazione_Entita.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod.ToString))
            End If

            If Programmazione_Entita_Cod <> 0 Then
                StrSQL.AppendLine(" AND Programmazione_Entita.Programmazione_Entita_Cod = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod.ToString))
            End If

            If Piva <> "" Then
                StrSQL.AppendLine(" AND Programmazione_Entita.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Entita_Des <> "" Then
                StrSQL.AppendLine(" AND Programmazione_Entita.Entita_Des like '%" & Agro_SQL_SaveText(Entita_Des) & "%' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Programmazione_Entita.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod.ToString))
            End If

            If Campo_Cod <> 0 Then
                StrSQL.AppendLine(" AND Programmazione_Entita.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod.ToString))
            End If

            If Appezza <> 0 Then
                StrSQL.AppendLine(" AND Programmazione_Entita.Appezza = " & Agro_SQL_SaveNum(Appezza.ToString))
            End If

            If Id_Reg <> 0 Then
                StrSQL.AppendLine(" AND Programmazione_Entita.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg.ToString))
            End If

            If Progetto_Cod <> 0 Then
                StrSQL.AppendLine(" AND Programmazione_Entita.Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod.ToString))
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Programmazione_Entita.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Programmazione_Entita.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")

            End Select

            Select Case SORT_Des1_Inizio2_Fine2
                Case 1
                    StrSQL.AppendLine(" ORDER BY Programmazione_Entita.Entita_Des ASC ")
                Case 2
                    StrSQL.AppendLine(" ORDER BY Programmazione_Entita.Validita_Inizio ASC ")
                Case 3
                    StrSQL.AppendLine(" ORDER BY Programmazione_Entita.Validita_fine ASC ")
                Case Else

                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY Programmazione_Entita.Entita_Des ASC ")
                    End If
            End Select


            '----------------------------------------------------
            '--- Recupero il datatable --------------------------
            '----------------------------------------------------

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

    Public Function Programmazione_Entita_Leggi_per_UMA(
                                            ByVal Programmazione_Cod As Integer,
                                            ByRef MessaggioErrore As String,
                                            ByVal Occupazione_Cod_Agea As String,
                                            ByVal Uso_Cod As String,
                                            ByVal Macrouso_Cod As String,
                                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                    Optional ByVal Fonte_Cod As Integer = 0,
                                            Optional ByVal RicavaAppRibaltati As Boolean = False
                                                ) As DataRow

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_R.Programmazione_Entita_Leggi()"

        Dim DT As DataTable
        Dim dtresult As New DataTable
        dtresult.Columns.Add(New DataColumn("macrouso_UMA_Cod", GetType(String)))
        dtresult.Columns.Add(New DataColumn("macrouso_UMA_Des", GetType(String)))
        dtresult.Columns.Add(New DataColumn("sup_A", GetType(Decimal)))
        dtresult.Columns.Add(New DataColumn("sup_B", GetType(Decimal)))

        Dim StrSQL As New System.Text.StringBuilder
        Dim resultrow As DataRow
        resultrow = dtresult.NewRow()

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine("SELECT DISTINCT COALESCE(a.macrouso_UMA_Cod, '') as 'macrouso_UMA_Cod', COALESCE(a.macrouso_UMA_Des, '') as 'macrouso_UMA_Des', COALESCE(CASE zz.Zona_Cod WHEN -46 THEN e.Superficie END, 0) as sup_A , ")
            StrSQL.AppendLine("COALESCE(CASE zz.Zona_Cod WHEN -47 THEN e.Superficie END, 0) as sup_B ")
            StrSQL.AppendLine("FROM Programmazione_Entita e ")
            StrSQL.AppendLine("JOIN Codifica_SpecieVegetali_Agea_2015_2020 a on e.Occupazione_Cod_Agea = a.Occupazione_Cod and ")
            StrSQL.AppendLine("e.Uso_Cod_Agea = a.Uso_Cod and e.Macrouso_Cod = a.Macrouso_Cod ")
            StrSQL.AppendLine("LEFT OUTER JOIN Programmazione_Particelle p on e.Programmazione_Entita_Cod = p.Programmazione_Entita_Cod ")
            StrSQL.AppendLine("LEFT OUTER JOIN ZonexParticelle z on p.prov = z.PROV AND p.Com = z.COM AND p.Sezione = z.SEZIONE AND ")
            StrSQL.AppendLine("p.Foglio = z.FOGLIO AND p.Numero = z.NUMERO AND p.Subalterno = z.SUBALTERNO ")
            StrSQL.AppendLine("LEFT OUTER JOIN Zone zz on zz.zona_cod = z.Zona_Cod ")
            StrSQL.AppendLine("where programmazione_cod = " & Agro_SQL_SaveText(Programmazione_Cod) & " and z.prov in ('054', '055') ")
            StrSQL.AppendLine("and e.Occupazione_Cod_Agea = " & Agro_SQL_SaveText(Occupazione_Cod_Agea) & " ")
            StrSQL.AppendLine("and e.Uso_Cod_Agea = " & Agro_SQL_SaveText(Uso_Cod) & " and e.macrouso_cod = " & Agro_SQL_SaveText(Macrouso_Cod) & " ")
            'StrSQL.Append("group by a.Macrouso_UMA_Cod, a.macrouso_UMA_Des ")

            '----------------------------------------------------
            '--- Recupero il datatable --------------------------
            '----------------------------------------------------

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If (DT.Rows.Count > 0) Then

                resultrow.Item("macrouso_UMA_Cod") = DT.Rows.Item(0).Item("macrouso_UMA_Cod")
                resultrow.Item("macrouso_UMA_Des") = DT.Rows.Item(0).Item("macrouso_UMA_Des").ToString().TrimEnd("  ")
                resultrow.Item("sup_A") = 0
                resultrow.Item("sup_B") = 0

                For Each row As DataRow In DT.Rows

                    If (row.Item("macrouso_UMA_Cod") = resultrow.Item("macrouso_UMA_Cod")) Then

                        resultrow.Item("sup_A") += Math.Round(Decimal.Parse(row.Item("sup_A")), 4)
                        resultrow.Item("sup_B") += Math.Round(Decimal.Parse(row.Item("sup_B")), 4)

                    End If

                Next
            Else

                resultrow.Item("macrouso_UMA_Cod") = ""
                resultrow.Item("macrouso_UMA_Des") = ""
                resultrow.Item("sup_A") = 0
                resultrow.Item("sup_B") = 0

            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return resultrow

    End Function

    Public Function Programmazione_Entita_Leggi_per_UMA_DT_Pendenza(
                                            ByVal Programmazione_Cod As Integer,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            Optional ByVal xFiltroAggiuntivo As String = "",
                                            Optional ByVal xOrderBy As String = "",
                                            Optional ByVal macro_cod As Integer = -1,
                                            Optional ByVal particelle As Boolean = False,
                                            Optional ByVal richiesta_Cod As Integer = -1,
                                            Optional ByVal anno As Integer = 0
                                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_R.Programmazione_Entita_Leggi_per_UMA_DT_Pendenza()"

        Dim DT As DataTable
        'Dim dtresult As New DataTable
        'dtresult.Columns.Add(New DataColumn("macrouso_UMA_Cod", GetType(String)))
        'dtresult.Columns.Add(New DataColumn("macrouso_UMA_Des", GetType(String)))
        'dtresult.Columns.Add(New DataColumn("macrouso_Cod", GetType(String)))
        'dtresult.Columns.Add(New DataColumn("macrouso_Des", GetType(String)))
        'dtresult.Columns.Add(New DataColumn("sup_A", GetType(Decimal)))
        'dtresult.Columns.Add(New DataColumn("sup_B", GetType(Decimal)))

        Dim StrSQL As New System.Text.StringBuilder

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT ")
            If particelle Then
                StrSQL.AppendLine(" ROW_NUMBER() OVER ( ")
                StrSQL.AppendLine("     ORDER BY Programmazione_Cod DESC ")
                StrSQL.AppendLine(" ) AS ID, ")
                StrSQL.AppendLine(" PROV, PROVINCIA, COM, COMUNE, SEZIONE, FOGLIO , NUMERO , SUBALTERNO, ")
            End If
            StrSQL.AppendLine("macrouso_UMA_Cod, macrouso_UMA_Des, ")
            If particelle Then
                StrSQL.AppendLine("ROUND(SUM(sup_A) + SUM(sup_NULL), 3) As Sup_A, ROUND(SUM(sup_B), 3) As Sup_B, ")
            Else
                StrSQL.AppendLine("SUM(sup_A) + SUM(sup_NULL) As sup_A, SUM(sup_B) As sup_B, ")
                StrSQL.AppendLine("Cul_Cod_Agea, macrouso_cod, Macrouso_Des, Occupazione_Cod_Agea, Destinazione_Cod_Agea, veg_cod, Id_Cod, ")
            End If
            StrSQL.AppendLine(" Programmazione_Cod, Programmazione_Des FROM ")
            StrSQL.AppendLine("(Select ")
            If particelle Then
                StrSQL.AppendLine(" z.PROV, pro.PROVINCIA, z.COM, comu.Descrizione as COMUNE, z.SEZIONE, z.FOGLIO , z.NUMERO , z.SUBALTERNO, ")
            End If
            StrSQL.AppendLine("e.Cul_Cod_Agea, e.macrouso_cod, m.Macrouso_Des, e.Occupazione_Cod_Agea, e.Destinazione_Cod_Agea, e.veg_cod, e.Id_Cod, ")
            StrSQL.AppendLine("COALESCE(a.macrouso_UMA_Cod, x.macrouso_UMA_Cod, b.macrouso_UMA_Cod, c.Macrouso_UMA_Cod, d.Macrouso_UMA_Cod, a_veg.macrouso_UMA_Cod, '') as 'macrouso_UMA_Cod',  ")
            StrSQL.AppendLine("COALESCE(a.macrouso_UMA_Des, x.macrouso_UMA_Des, b.macrouso_UMA_Des, c.macrouso_UMA_Des, d.Macrouso_UMA_Des, a_veg.macrouso_UMA_Des, '') as 'macrouso_UMA_Des',  ")
            StrSQL.AppendLine("COALESCE(CASE zz.Zona_Cod WHEN -46 THEN p.Superficie END, 0) as sup_A , ")
            StrSQL.AppendLine("COALESCE(CASE zz.Zona_Cod WHEN -47 THEN p.Superficie END, 0) as sup_B, ")
            StrSQL.AppendLine("COALESCE(CASE WHEN zz.Zona_Cod IS NULL THEN p.Superficie END, 0) as sup_NULL, ")
            StrSQL.AppendLine(" t.Programmazione_Cod, t.Programmazione_Des  ")
            StrSQL.AppendLine("FROM Programmazione_Entita e ")
            StrSQL.AppendLine("JOIN Programmazione_Testata t ON e.Programmazione_Cod = t.Programmazione_Cod ")
            StrSQL.AppendLine("OUTER APPLY (SELECT TOP 1 * FROM Codifica_SpecieVegetali_Agea_2015_2020 ci WHERE ci.Occupazione_Cod = e.Occupazione_Cod_Agea AND ci.Macrouso_Cod = e.Macrouso_Cod AND ci.Destinazione_Cod = e.Destinazione_Cod_Agea AND ci.Qualita_Cod = e.Qualita_Cod_Agea AND ci.Uso_Cod = e.Uso_Cod_Agea AND e.Veg_Cod = ci.Veg_Cod AND ci.Macrouso_UMA_Cod IS NOT NULL AND ci.Macrouso_UMA_Des IS NOT NULL) a  ")
            StrSQL.AppendLine("OUTER APPLY (SELECT TOP 1 * FROM Codifica_SpecieVegetali_Agea_2015_2020 ci WHERE ci.Occupazione_Cod = e.Occupazione_Cod_Agea AND ci.Macrouso_Cod = e.Macrouso_Cod AND ci.Destinazione_Cod = e.Destinazione_Cod_Agea AND ci.Qualita_Cod = e.Qualita_Cod_Agea AND ci.Uso_Cod = e.Uso_Cod_Agea AND ci.Macrouso_UMA_Cod IS NOT NULL AND ci.Macrouso_UMA_Des IS NOT NULL) x  ")
            StrSQL.AppendLine("OUTER APPLY (SELECT TOP 1 * FROM Codifica_SpecieVegetali_Agea_2015_2020 ci WHERE ci.Occupazione_Cod = e.Occupazione_Cod_Agea AND ci.Macrouso_Cod = e.Macrouso_Cod AND ci.Destinazione_Cod = e.Destinazione_Cod_Agea AND e.Veg_Cod = ci.Veg_Cod AND ci.Macrouso_UMA_Cod IS NOT NULL AND ci.Macrouso_UMA_Des IS NOT NULL) b  ")
            StrSQL.AppendLine("OUTER APPLY (SELECT TOP 1 * FROM Codifica_SpecieVegetali_Agea_2015_2020 ci WHERE ci.Occupazione_Cod = e.Occupazione_Cod_Agea AND ci.Macrouso_Cod = e.Macrouso_Cod AND e.Veg_Cod = ci.Veg_Cod AND ci.Macrouso_UMA_Cod IS NOT NULL AND ci.Macrouso_UMA_Des IS NOT NULL) c ")
            StrSQL.AppendLine("OUTER APPLY (SELECT TOP 1 * FROM Codifica_SpecieVegetali_Agea_2015_2020 ci WHERE ci.Occupazione_Cod = e.Occupazione_Cod_Agea AND e.Veg_Cod = ci.Veg_Cod AND ci.Macrouso_UMA_Cod IS NOT NULL AND ci.Macrouso_UMA_Des IS NOT NULL) d ")
            StrSQL.AppendLine("OUTER APPLY (SELECT TOP 1 * FROM Codifica_SpecieVegetali_Agea_2015_2020 cveg WHERE cveg.Veg_Cod = e.Veg_Cod AND cveg.Macrouso_UMA_Cod IS NOT NULL AND cveg.Macrouso_UMA_Des IS NOT NULL) a_veg ")
            StrSQL.AppendLine("LEFT JOIN Macrousi m on m.Macrouso_Cod = e.Macrouso_Cod")
            StrSQL.AppendLine("LEFT OUTER JOIN Programmazione_Particelle p on e.Programmazione_Entita_Cod = p.Programmazione_Entita_Cod ")
            StrSQL.AppendLine("LEFT OUTER JOIN ZonexParticelle z on p.prov = z.PROV AND p.Com = z.COM AND p.Sezione = z.SEZIONE AND ")
            StrSQL.AppendLine("                p.Foglio = z.FOGLIO AND p.Numero = z.NUMERO AND p.Subalterno = z.SUBALTERNO AND z.Zona_Cod IN (-46, -47) ")
            StrSQL.AppendLine("LEFT OUTER JOIN Zone zz on zz.zona_cod = z.Zona_Cod ")
            If particelle Then
                StrSQL.AppendLine(" JOIN Lista_Province pro on pro.PROV = p.prov ")
                StrSQL.AppendLine(" JOIN ISTAT_Comuni comu on comu.pro_cod_Istat = p.prov and comu.com_cod_Istat = p.Com ")
            ElseIf richiesta_Cod >= 0 Then
                StrSQL.AppendLine(" JOIN UMA_Richieste_Testata testa on testa.richiesta_Cod = " & richiesta_Cod.ToString & " ")
                StrSQL.AppendLine(" JOIN Pratiche pra on testa.pratica_cod = pra.pratica_cod ")
            End If
            StrSQL.AppendLine("where p.prov in ('054', '055') ")
            If Not particelle AndAlso (richiesta_Cod >= 0 OrElse anno > 0) Then
                StrSQL.AppendLine(" and (SELECT COUNT(*) From UMA_Blocco_Particelle ubp where ubp.prov = z.PROV AND ubp.Com = z.COM AND ubp.Sezione = z.SEZIONE AND ")
                StrSQL.AppendLine(" ubp.Foglio = z.FOGLIO AND ubp.Numero = z.NUMERO AND ubp.Subalterno = z.SUBALTERNO AND ubp.Gruppo_Colturale_UMA = COALESCE(a.macrouso_UMA_Cod, x.macrouso_UMA_Cod, b.macrouso_UMA_Cod, c.Macrouso_UMA_Cod, d.Macrouso_UMA_Cod, a_veg.macrouso_UMA_Cod, '')")
                StrSQL.AppendLine(" and ubp.Anno = " & If(anno > 0, anno.ToString, "pra.Anno") & " AND ubp.Programmazione_Cod = e.Programmazione_Cod AND Bloccato = 1) < 1 ")
            End If
            If Programmazione_Cod > 0 Then
                StrSQL.AppendLine(" and e.programmazione_cod = " & Agro_SQL_SaveNum(Programmazione_Cod) & " ")
            End If
            If (macro_cod > 0) Then
                StrSQL.AppendLine(" and COALESCE(a.macrouso_UMA_Cod, x.macrouso_UMA_Cod, b.macrouso_UMA_Cod, c.Macrouso_UMA_Cod, d.Macrouso_UMA_Cod, a_veg.macrouso_UMA_Cod, '') = '" & Agro_SQL_SaveText(macro_cod) & "' ")
            End If
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo))
            End If

            StrSQL.AppendLine(" ) as B ")

            'StrSQL.AppendLine("and e.Cul_Cod_Agea = " + Agro_SQL_SaveText(Cul_Cod_Agea) + " and e.macrouso_cod = " + Agro_SQL_SaveText(Macrouso_Cod) + " ")

            If particelle Then
                StrSQL.AppendLine(" WHERE PROV IS NOT NULL ")
                StrSQL.AppendLine(" GROUP BY PROV, PROVINCIA, COM, COMUNE, SEZIONE, FOGLIO , NUMERO , SUBALTERNO, macrouso_UMA_Cod, macrouso_UMA_Des, Programmazione_Cod, Programmazione_Des ")
            Else
                StrSQL.AppendLine(" GROUP BY Occupazione_Cod_Agea, Cul_Cod_Agea, Macrouso_Cod, Veg_Cod, Id_Cod, Destinazione_Cod_Agea, Macrouso_Des, macrouso_UMA_Cod, macrouso_UMA_Des, Programmazione_Cod, Programmazione_Des ")
            End If
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If
            'StrSQL.Append("group by a.Macrouso_UMA_Cod, a.macrouso_UMA_Des ")

            '----------------------------------------------------
            '--- Recupero il datatable --------------------------
            '----------------------------------------------------

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            'If (DT.Rows.Count > 0) Then

            '    resultrow.Item("macrouso_UMA_Cod") = DT.Rows.Item(0).Item("macrouso_UMA_Cod")
            '    resultrow.Item("macrouso_UMA_Des") = DT.Rows.Item(0).Item("macrouso_UMA_Des").ToString().TrimEnd("  ")
            '    resultrow.Item("sup_A") = 0
            '    resultrow.Item("sup_B") = 0

            '    For Each row As DataRow In DT.Rows

            '        If (row.Item("macrouso_UMA_Cod") = resultrow.Item("macrouso_UMA_Cod")) Then

            '            resultrow.Item("sup_A") += Math.Round(Decimal.Parse(row.Item("sup_A")), 4)
            '            resultrow.Item("sup_B") += Math.Round(Decimal.Parse(row.Item("sup_B")), 4)

            '        End If

            '    Next
            'Else

            '    resultrow.Item("macrouso_UMA_Cod") = ""
            '    resultrow.Item("macrouso_UMA_Des") = ""
            '    resultrow.Item("sup_A") = 0
            '    resultrow.Item("sup_B") = 0

            'End If

        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try

        Return DT

    End Function

    Public Function Programmazione_Entita_Leggi_per_UMA_DT_Pendenza_Dettaglio(ByVal Programmazione_Cod As Integer,
                                                                               ByVal Programmazione_Entita_Cod As Integer,
                                                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                                              Optional ByVal richiesta_Cod As Integer = -1) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_R.Programmazione_Entita_Leggi_per_UMA_DT_Pendenza_Dettaglio()"

        Dim DT As DataTable

        Dim StrSQL As New System.Text.StringBuilder

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine("SELECT ")
            StrSQL.AppendLine("SUM(sup_A) + SUM(sup_NULL) as sup_A, SUM(sup_B) as sup_B FROM ")
            StrSQL.AppendLine("(SELECT ")
            StrSQL.AppendLine("COALESCE(CASE zz.Zona_Cod WHEN -46 THEN p.Superficie END, 0) as sup_A , ")
            StrSQL.AppendLine("COALESCE(CASE zz.Zona_Cod WHEN -47 THEN p.Superficie END, 0) as sup_B, ")
            StrSQL.AppendLine("COALESCE(CASE WHEN zz.Zona_Cod IS NULL THEN p.Superficie END, 0) as sup_NULL  ")
            StrSQL.AppendLine("FROM Programmazione_Entita e ")
            StrSQL.AppendLine("OUTER APPLY (SELECT TOP 1 * FROM Codifica_SpecieVegetali_Agea_2015_2020 ci WHERE ci.Occupazione_Cod = e.Occupazione_Cod_Agea AND ci.Macrouso_Cod = e.Macrouso_Cod) a ")
            StrSQL.AppendLine("OUTER APPLY (SELECT TOP 1 * FROM Codifica_SpecieVegetali_Agea_2015_2020 cveg WHERE cveg.Veg_Cod = e.Veg_Cod) a_veg ")
            StrSQL.AppendLine("LEFT OUTER JOIN Programmazione_Particelle p on e.Programmazione_Entita_Cod = p.Programmazione_Entita_Cod ")
            StrSQL.AppendLine("LEFT OUTER JOIN ZonexParticelle z on p.prov = z.PROV AND p.Com = z.COM AND p.Sezione = z.SEZIONE AND p.Foglio = z.FOGLIO AND p.Numero = z.NUMERO AND p.Subalterno = z.SUBALTERNO ")
            StrSQL.AppendLine("LEFT OUTER JOIN Zone zz on zz.zona_cod = z.Zona_Cod ")
            If richiesta_Cod >= 0 Then
                StrSQL.AppendLine(" JOIN UMA_Richieste_Testata testa on testa.richiesta_Cod = " & richiesta_Cod.ToString & " ")
                StrSQL.AppendLine(" JOIN Pratiche pra on testa.pratica_cod = pra.pratica_cod ")
            End If
            StrSQL.AppendLine("where e.programmazione_cod = " & Agro_SQL_SaveNum(Programmazione_Cod) & " and p.prov in ('054', '055') ")
            If richiesta_Cod >= 0 Then
                StrSQL.AppendLine(" and (SELECT COUNT(*) From UMA_Blocco_Particelle ubp where ubp.prov = z.PROV AND ubp.Com = z.COM AND ubp.Sezione = z.SEZIONE AND ")
                StrSQL.AppendLine(" ubp.Foglio = z.FOGLIO AND ubp.Numero = z.NUMERO AND ubp.Subalterno = z.SUBALTERNO AND ubp.Gruppo_Colturale_UMA = COALESCE(a.macrouso_UMA_Cod, a_veg.macrouso_UMA_Cod, '')")
                StrSQL.AppendLine(" and ubp.Anno = pra.Anno AND ubp.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod) & " AND Bloccato = 1) < 1 ")
            End If
            StrSQL.AppendLine("and e.Programmazione_Entita_Cod = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & " ")
            StrSQL.AppendLine(" ) as B ")


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------



        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try

        Return DT

    End Function

    Public Function Programmazione_Entita_Leggi_per_UMA_DT_Tessitura(
                                            ByVal Programmazione_Cod As Integer,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            Optional ByVal macro_cod As Integer = -1,
                                            Optional ByVal richiesta_Cod As Integer = -1,
                                            Optional ByVal anno As Integer = 0,
                                            Optional ByVal avanzamento As Integer = -2,
                                            Optional ByVal terzista As Integer = 1
                                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_R.Programmazione_Entita_Leggi()"

        Dim DT As DataTable
        'Dim dtresult As New DataTable
        'dtresult.Columns.Add(New DataColumn("macrouso_UMA_Cod", GetType(String)))
        'dtresult.Columns.Add(New DataColumn("macrouso_UMA_Des", GetType(String)))
        'dtresult.Columns.Add(New DataColumn("sup_A", GetType(Decimal)))
        'dtresult.Columns.Add(New DataColumn("sup_B", GetType(Decimal)))

        Dim StrSQL As New System.Text.StringBuilder

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT Cul_Cod_Agea, Macrouso_Cod, Macrouso_Des, Veg_Cod, Id_Cod, Occupazione_Cod_Agea, Destinazione_Cod_Agea, macrouso_UMA_Cod, macrouso_UMA_Des, ")
            StrSQL.AppendLine(" SUM(sup_Normale) + SUM(Sup_Null) as sup_Normale,  ")
            StrSQL.AppendLine(" SUM(sup_Media) as sup_Media,  ")
            StrSQL.AppendLine(" SUM (sup_Tenace) as sup_Tenace, Programmazione_Cod, Programmazione_Des FROM  ")
            StrSQL.AppendLine("(SELECT e.Cul_Cod_Agea, e.macrouso_cod, m.Macrouso_Des, e.Occupazione_Cod_Agea, e.Destinazione_Cod_Agea, e.veg_cod, e.Id_Cod, ")
            StrSQL.AppendLine("COALESCE(a.macrouso_UMA_Cod, x.macrouso_UMA_Cod, b.macrouso_UMA_Cod, c.Macrouso_UMA_Cod, d.Macrouso_UMA_Cod, a_veg.macrouso_UMA_Cod, '') as 'macrouso_UMA_Cod', ")
            StrSQL.AppendLine("COALESCE(a.macrouso_UMA_Des, x.macrouso_UMA_Des, b.macrouso_UMA_Des, c.macrouso_UMA_Des, d.Macrouso_UMA_Des, a_veg.macrouso_UMA_Des, '') as 'macrouso_UMA_Des', ")
            StrSQL.AppendLine("COALESCE(CASE WHEN (att.Id_ClasseTessitura = 1 Or att.Id_ClasseTessitura = 0) THEN p.Superficie END, 0) as sup_Normale, ")
            StrSQL.AppendLine("COALESCE(CASE att.Id_ClasseTessitura WHEN 2 THEN p.Superficie END, 0) as sup_Media, ")
            StrSQL.AppendLine("COALESCE(CASE att.Id_ClasseTessitura WHEN 3 THEN p.Superficie END, 0) as sup_Tenace, ")
            StrSQL.AppendLine("COALESCE(CASE WHEN att.Id_ClasseTessitura IS NULL THEN p.Superficie END, 0) as sup_NULL, ")
            StrSQL.AppendLine("t.Programmazione_Des, t.Programmazione_Cod ")
            StrSQL.AppendLine("FROM Programmazione_Entita e ")
            StrSQL.AppendLine("JOIN Programmazione_Testata t ON e.Programmazione_Cod = t.Programmazione_Cod ")
            StrSQL.AppendLine("OUTER APPLY (SELECT TOP 1 * FROM Codifica_SpecieVegetali_Agea_2015_2020 ci WHERE ci.Occupazione_Cod = e.Occupazione_Cod_Agea AND ci.Macrouso_Cod = e.Macrouso_Cod AND ci.Destinazione_Cod = e.Destinazione_Cod_Agea AND ci.Qualita_Cod = e.Qualita_Cod_Agea AND ci.Uso_Cod = e.Uso_Cod_Agea AND e.Veg_Cod = ci.Veg_Cod AND ci.Macrouso_UMA_Cod IS NOT NULL AND ci.Macrouso_UMA_Des IS NOT NULL) a ")
            StrSQL.AppendLine("OUTER APPLY (SELECT TOP 1 * FROM Codifica_SpecieVegetali_Agea_2015_2020 ci WHERE ci.Occupazione_Cod = e.Occupazione_Cod_Agea AND ci.Macrouso_Cod = e.Macrouso_Cod AND ci.Destinazione_Cod = e.Destinazione_Cod_Agea AND ci.Qualita_Cod = e.Qualita_Cod_Agea AND ci.Uso_Cod = e.Uso_Cod_Agea AND ci.Macrouso_UMA_Cod IS NOT NULL AND ci.Macrouso_UMA_Des IS NOT NULL) x ")
            StrSQL.AppendLine("OUTER APPLY (SELECT TOP 1 * FROM Codifica_SpecieVegetali_Agea_2015_2020 ci WHERE ci.Occupazione_Cod = e.Occupazione_Cod_Agea AND ci.Macrouso_Cod = e.Macrouso_Cod AND ci.Destinazione_Cod = e.Destinazione_Cod_Agea AND e.Veg_Cod = ci.Veg_Cod AND ci.Macrouso_UMA_Cod IS NOT NULL AND ci.Macrouso_UMA_Des IS NOT NULL) b ")
            StrSQL.AppendLine("OUTER APPLY (SELECT TOP 1 * FROM Codifica_SpecieVegetali_Agea_2015_2020 ci WHERE ci.Occupazione_Cod = e.Occupazione_Cod_Agea AND ci.Macrouso_Cod = e.Macrouso_Cod AND e.Veg_Cod = ci.Veg_Cod AND ci.Macrouso_UMA_Cod IS NOT NULL AND ci.Macrouso_UMA_Des IS NOT NULL) c ")
            StrSQL.AppendLine("OUTER APPLY (SELECT TOP 1 * FROM Codifica_SpecieVegetali_Agea_2015_2020 ci WHERE ci.Occupazione_Cod = e.Occupazione_Cod_Agea  AND e.Veg_Cod = ci.Veg_Cod AND ci.Macrouso_UMA_Cod IS NOT NULL AND ci.Macrouso_UMA_Des IS NOT NULL) d ")
            StrSQL.AppendLine("OUTER APPLY (SELECT TOP 1 * FROM Codifica_SpecieVegetali_Agea_2015_2020 cveg WHERE cveg.Veg_Cod = e.Veg_Cod AND cveg.Macrouso_UMA_Cod IS NOT NULL AND cveg.Macrouso_UMA_Des IS NOT NULL) a_veg ")
            StrSQL.AppendLine("LEFT JOIN Macrousi m on m.Macrouso_Cod = e.Macrouso_Cod")
            StrSQL.AppendLine("LEFT OUTER JOIN Programmazione_Particelle p on e.Programmazione_Entita_Cod = p.Programmazione_Entita_Cod ")
            If richiesta_Cod >= 0 Then
                StrSQL.AppendLine(" JOIN UMA_Richieste_Testata testa on testa.richiesta_Cod = " & richiesta_Cod.ToString & " ")
                StrSQL.AppendLine(" JOIN Pratiche pra on testa.pratica_cod = pra.pratica_cod ")
            End If
            StrSQL.AppendLine("OUTER APPLY (SELECT TOP 1 aet.* FROM Analisi_EntitaxTestata aet inner join Analisi_Testata att ON att.Analisi_Testata_Cod = aet.Analisi_Testata_Cod ")
            StrSQL.AppendLine(" WHERE ((p.Prov = aet.Prov AND p.Com = aet.Com AND p.sezione = IIF(aet.sezione = '', '0',  aet.sezione) AND p.foglio = aet.foglio AND p.numero = aet.numero AND ")
            StrSQL.AppendLine(" p.subalterno = iif(aet.subalterno = '', '0',  aet.subalterno) and aet.Analisi_Entita_Cod = " & enum_Entita_Analisi.Particella & ") OR (e.Piva = aet.Piva and e.Sa_Cod = aet.Sa_Cod and aet.Analisi_Entita_Cod = " & enum_Entita_Analisi.Centro & ") ")
            StrSQL.AppendLine(" OR (e.Piva = aet.Piva and aet.Analisi_Entita_Cod = " & enum_Entita_Analisi.Impresa & ") ) And ")
            If avanzamento = 0 AndAlso richiesta_Cod > 0 Then
                StrSQL.AppendLine(" testa.Data_Creazione >= att.Analisi_Testata_Data_Inizio ")
            ElseIf avanzamento = 1 AndAlso richiesta_Cod > 0 Then
                StrSQL.AppendLine(" CONCAT('01/07/', pra.anno) >= att.Analisi_Testata_Data_Inizio ")
            End If
            StrSQL.AppendLine(" order by att.Analisi_Testata_Data_Inizio desc) aaeett ")
            StrSQL.AppendLine("LEFT JOIN Analisi_Testata att On aaeett.Analisi_Testata_Cod = att.Analisi_Testata_Cod ")
            StrSQL.AppendLine("where e.programmazione_cod = " & Agro_SQL_SaveNum(Programmazione_Cod) & " And p.prov In ('054', '055') ")
            If (richiesta_Cod >= 0 OrElse anno > 0) AndAlso terzista > -1 Then
                StrSQL.AppendLine(" and (SELECT COUNT(*) From UMA_Blocco_Particelle ubp where ubp.prov = p.PROV AND ubp.Com = p.COM AND ubp.Sezione = p.SEZIONE AND ")
                StrSQL.AppendLine(" ubp.Foglio = p.FOGLIO AND ubp.Numero = p.NUMERO AND ubp.Subalterno = p.SUBALTERNO AND ubp.Gruppo_Colturale_UMA = COALESCE(a.macrouso_UMA_Cod, x.macrouso_UMA_Cod, b.macrouso_UMA_Cod, c.Macrouso_UMA_Cod, d.Macrouso_UMA_Cod, a_veg.macrouso_UMA_Cod, '')")
                StrSQL.AppendLine(" and ubp.Anno = " & If(anno > 0, anno.ToString, "pra.Anno") & " AND ubp.Programmazione_Cod = e.Programmazione_Cod AND Bloccato = 1) < 1 ")
            End If
            If (macro_cod > 0) Then
                StrSQL.AppendLine(" and COALESCE(a.macrouso_UMA_Cod, x.macrouso_UMA_Cod, b.macrouso_UMA_Cod, c.Macrouso_UMA_Cod, d.Macrouso_UMA_Cod, a_veg.macrouso_UMA_Cod, '') = '" & Agro_SQL_SaveText(macro_cod) & "' ")
            End If
            StrSQL.AppendLine(" ) as B ")

            StrSQL.AppendLine(" GROUP BY Occupazione_Cod_Agea, Cul_Cod_Agea, Macrouso_Cod, Veg_Cod, Id_Cod, Destinazione_Cod_Agea, Macrouso_Des, macrouso_UMA_Cod, macrouso_UMA_Des, Programmazione_Cod, Programmazione_Des ")

            '----------------------------------------------------
            '--- Recupero il datatable --------------------------
            '----------------------------------------------------

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            'If (DT.Rows.Count > 0) Then

            '    resultrow.Item("macrouso_UMA_Cod") = DT.Rows.Item(0).Item("macrouso_UMA_Cod")
            '    resultrow.Item("macrouso_UMA_Des") = DT.Rows.Item(0).Item("macrouso_UMA_Des").ToString().TrimEnd("  ")
            '    resultrow.Item("sup_A") = 0
            '    resultrow.Item("sup_B") = 0

            '    For Each row As DataRow In DT.Rows

            '        If (row.Item("macrouso_UMA_Cod") = resultrow.Item("macrouso_UMA_Cod")) Then

            '            resultrow.Item("sup_A") += Math.Round(Decimal.Parse(row.Item("sup_A")), 4)
            '            resultrow.Item("sup_B") += Math.Round(Decimal.Parse(row.Item("sup_B")), 4)

            '        End If

            '    Next
            'Else

            '    resultrow.Item("macrouso_UMA_Cod") = ""
            '    resultrow.Item("macrouso_UMA_Des") = ""
            '    resultrow.Item("sup_A") = 0
            '    resultrow.Item("sup_B") = 0

            'End If

        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try

        Return DT

    End Function

    Public Function Programmazione_Entita_Leggi_per_UMA_DT_Tessitura_Dettaglio(ByVal Programmazione_Cod As Integer,
                                                                               ByVal Programmazione_Entita_Cod As Integer,
                                                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                                               Optional ByVal richiesta_cod As Integer = -1,
                                                                               Optional ByVal avanzamento As Integer = -2) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_R.Programmazione_Entita_Leggi_per_UMA_DT_Tessitura_Dettaglio()"

        Dim DT As DataTable

        Dim StrSQL As New System.Text.StringBuilder

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine("SELECT ")
            StrSQL.AppendLine("SUM(sup_Normale) + SUM(Sup_Null) as sup_Normale, ")
            StrSQL.AppendLine("SUM(sup_Media) as sup_Media,  ")
            StrSQL.AppendLine("SUM (sup_Tenace) as sup_Tenace FROM ")
            StrSQL.AppendLine("(SELECT ")
            StrSQL.AppendLine("COALESCE(CASE att.Id_ClasseTessitura WHEN 1 THEN p.Superficie END, 0) as sup_Normale, ")
            StrSQL.AppendLine("COALESCE(CASE att.Id_ClasseTessitura WHEN 2 THEN p.Superficie END, 0) as sup_Media, ")
            StrSQL.AppendLine("COALESCE(CASE att.Id_ClasseTessitura WHEN 3 THEN p.Superficie END, 0) as sup_Tenace, ")
            StrSQL.AppendLine("COALESCE(CASE WHEN att.Id_ClasseTessitura IS NULL THEN p.Superficie END, 0) as sup_NULL ")
            StrSQL.AppendLine("FROM Programmazione_Entita e ")
            StrSQL.AppendLine("OUTER APPLY (SELECT TOP 1 * FROM Codifica_SpecieVegetali_Agea_2015_2020 ci WHERE ci.Occupazione_Cod = e.Occupazione_Cod_Agea AND ci.Macrouso_Cod = e.Macrouso_Cod) a ")
            StrSQL.AppendLine("OUTER APPLY (SELECT TOP 1 * FROM Codifica_SpecieVegetali_Agea_2015_2020 cveg WHERE cveg.Veg_Cod = e.Veg_Cod) a_veg ")
            StrSQL.AppendLine("LEFT OUTER JOIN Programmazione_Particelle p on e.Programmazione_Entita_Cod = p.Programmazione_Entita_Cod ")
            If richiesta_cod >= 0 Then
                StrSQL.AppendLine(" JOIN UMA_Richieste_Testata testa on testa.richiesta_Cod = " & richiesta_cod.ToString & " ")
                StrSQL.AppendLine(" JOIN Pratiche pra on testa.pratica_cod = pra.pratica_cod ")
            End If
            StrSQL.AppendLine("OUTER APPLY (SELECT TOP 1 aet.* FROM Analisi_EntitaxTestata aet inner join Analisi_Testata att ON att.Analisi_Testata_Cod = aet.Analisi_Testata_Cod ")
            StrSQL.AppendLine(" WHERE ((p.Prov = aet.Prov AND p.Com = aet.Com AND p.sezione = IIF(aet.sezione = '', '0',  aet.sezione) AND p.foglio = aet.foglio AND p.numero = aet.numero AND ")
            StrSQL.AppendLine(" p.subalterno = iif(aet.subalterno = '', '0',  aet.subalterno) and aet.Analisi_Entita_Cod = " & enum_Entita_Analisi.Particella & ") OR (e.Piva = aet.Piva and e.Sa_Cod = aet.Sa_Cod and aet.Analisi_Entita_Cod = " & enum_Entita_Analisi.Centro & ") ")
            StrSQL.AppendLine(" OR (e.Piva = aet.Piva and aet.Analisi_Entita_Cod = " & enum_Entita_Analisi.Impresa & ") ) And ")
            If avanzamento = 0 AndAlso richiesta_cod > 0 Then
                StrSQL.AppendLine(" testa.Data_Creazione >= att.Analisi_Testata_Data_Inizio ")
            ElseIf avanzamento = 1 AndAlso richiesta_cod > 0 Then
                StrSQL.AppendLine(" CONCAT('01/07/', pra.anno) >= att.Analisi_Testata_Data_Inizio ")
            End If
            StrSQL.AppendLine(" order by att.Analisi_Testata_Data_Inizio desc) aaeett ")
            StrSQL.AppendLine("LEFT JOIN Analisi_Testata att ON aaeett.Analisi_Testata_Cod = att.Analisi_Testata_Cod ")
            StrSQL.AppendLine("where e.programmazione_cod = " & Agro_SQL_SaveNum(Programmazione_Cod) & " and p.prov in ('054', '055') ")
            If richiesta_cod >= 0 Then
                StrSQL.AppendLine(" and (SELECT COUNT(*) From UMA_Blocco_Particelle ubp where ubp.prov = p.PROV AND ubp.Com = p.COM AND ubp.Sezione = p.SEZIONE AND ")
                StrSQL.AppendLine(" ubp.Foglio = p.FOGLIO AND ubp.Numero = p.NUMERO AND ubp.Subalterno = p.SUBALTERNO AND ubp.Gruppo_Colturale_UMA = COALESCE(a.macrouso_UMA_Cod, a_veg.macrouso_UMA_Cod, '')")
                StrSQL.AppendLine(" and ubp.Anno = pra.Anno AND ubp.Programmazione_Cod = e.Programmazione_Cod AND Bloccato = 1) < 1 ")
            End If
            StrSQL.AppendLine("and e.Programmazione_Entita_Cod = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & " ")
            StrSQL.AppendLine(" ) as B ")


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------



        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try

        Return DT

    End Function

    '################################################################################
    'LEGGE ENTITA IN JOIN CON TESTATA
    'union di due parti:
    'la prima legge con inner join su specie e varietà
    'la seconda legge con inner join su codice anagrafe destinazione d'uso
    Public Function Leggi(ByVal Programmazione_Cod As Integer,
                            ByVal Programmazione_Entita_Cod As Integer,
                            ByVal Entita_Des As String,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Integer,
                            ByVal Campo_Cod As Integer,
                            ByVal Appezza As Integer,
                            ByVal Id_Reg As Integer,
                            ByVal Progetto_Cod As Integer,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                            ByVal xFiltroAggiuntivo1 As String,
                            ByVal xFiltroAggiuntivo2 As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_R.Leggi()"

        Dim MessaggioErrore As String
        Dim DT As DataTable
        Dim StrSQL As New System.Text.StringBuilder

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT * FROM ")
            StrSQL.AppendLine(" ( ")


            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine(" SELECT  Programmazione_Entita.*, ")
            StrSQL.AppendLine("         ISNULL(SpecieVegetali.Veg_Des,'') AS Veg_Des, ")
            StrSQL.AppendLine("         ISNULL(Cultivar.Cul_Des,'') AS Cul_Des, '' AS DestinazioneUso_Des, ")
            StrSQL.AppendLine("         Programmazione_Testata.Programmazione_Des, ")
            StrSQL.AppendLine("         Programmazione_Testata.Validita_Inizio AS Programmazione_Validita_Inizio, ")
            StrSQL.AppendLine("         Programmazione_Testata.Validita_Fine AS Programmazione_Validita_Fine, ")
            StrSQL.AppendLine("         Programmazione_Testata.Tipo_Pianificazione ")

            StrSQL.AppendLine(" FROM    Programmazione_Entita ")
            StrSQL.AppendLine(" INNER JOIN Programmazione_Testata ON Programmazione_Entita.Programmazione_Cod = Programmazione_Testata.Programmazione_Cod ")
            StrSQL.AppendLine("             AND Programmazione_Entita.Piva_SuperUser = Programmazione_Testata.Piva_SuperUser ")

            StrSQL.AppendLine(" INNER JOIN SpecieVegetali ON Programmazione_Entita.Veg_Cod = SpecieVegetali.Veg_Cod ")
            StrSQL.AppendLine(" INNER JOIN Cultivar ON Programmazione_Entita.Cul_Cod = Cultivar.Cul_Cod ")

            StrSQL.AppendLine(" WHERE   Programmazione_Entita.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            StrSQL.AppendLine(" AND     Programmazione_Entita.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.AppendLine(" AND     Programmazione_Entita.Validita_fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            If Programmazione_Cod <> 0 Then
                StrSQL.AppendLine(" AND Programmazione_Entita.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod.ToString))
            End If

            If Programmazione_Entita_Cod <> 0 Then
                StrSQL.AppendLine(" AND Programmazione_Entita.Programmazione_Entita_Cod = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod.ToString))
            End If

            If Piva <> "" Then
                StrSQL.AppendLine(" AND Programmazione_Entita.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Entita_Des <> "" Then
                StrSQL.AppendLine(" AND Programmazione_Entita.Entita_Des like '%" & Agro_SQL_SaveText(Entita_Des) & "%' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Programmazione_Entita.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod.ToString))
            End If

            If Campo_Cod <> 0 Then
                StrSQL.AppendLine(" AND Programmazione_Entita.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod.ToString))
            End If

            If Appezza <> 0 Then
                StrSQL.AppendLine(" AND Programmazione_Entita.Appezza = " & Agro_SQL_SaveNum(Appezza.ToString))
            End If

            If Id_Reg <> 0 Then
                StrSQL.AppendLine(" AND Programmazione_Entita.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg.ToString))
            End If

            If Progetto_Cod <> 0 Then
                StrSQL.AppendLine(" AND Programmazione_Entita.Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod.ToString))
            End If

            If xFiltroAggiuntivo1 <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo1,, objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   Programmazione_Entita.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   Programmazione_Entita.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")

            End Select
            StrSQL.AppendLine(" ) ")

            StrSQL.AppendLine(" UNION ALL ")

            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine(" SELECT  Programmazione_Entita.*, ")
            StrSQL.AppendLine("         '' AS Veg_Des, ")
            StrSQL.AppendLine("         '' AS Cul_Des, ")
            StrSQL.AppendLine("         ISNULL(Codici_Anagrafe.descrizione,'') AS DestinazioneUso_Des, ")
            StrSQL.AppendLine("         Programmazione_Testata.Programmazione_Des, ")
            StrSQL.AppendLine("         Programmazione_Testata.Validita_Inizio AS Programmazione_Validita_Inizio, ")
            StrSQL.AppendLine("         Programmazione_Testata.Validita_Fine AS Programmazione_Validita_Fine, ")
            StrSQL.AppendLine("         Programmazione_Testata.Tipo_Pianificazione ")

            StrSQL.AppendLine(" FROM    Programmazione_Entita ")
            StrSQL.AppendLine(" INNER JOIN Programmazione_Testata ON Programmazione_Entita.Programmazione_Cod = Programmazione_Testata.Programmazione_Cod ")
            StrSQL.AppendLine("         AND Programmazione_Entita.Piva_SuperUser = Programmazione_Testata.Piva_SuperUser ")

            StrSQL.AppendLine(" INNER JOIN Codici_Anagrafe ON Programmazione_Entita.Id_Cod = Codici_Anagrafe.codice  ")

            StrSQL.AppendLine(" WHERE   Programmazione_Entita.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            StrSQL.AppendLine(" AND     Programmazione_Entita.Id_Cod >= 3000 AND Programmazione_Entita.Id_Cod < 4000")

            StrSQL.AppendLine(" AND     Programmazione_Entita.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.AppendLine(" AND     Programmazione_Entita.Validita_fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")


            If Programmazione_Cod <> 0 Then
                StrSQL.AppendLine(" AND Programmazione_Entita.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod.ToString))
            End If

            If Programmazione_Entita_Cod <> 0 Then
                StrSQL.AppendLine(" AND Programmazione_Entita.Programmazione_Entita_Cod = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod.ToString))
            End If

            If Piva <> "" Then
                StrSQL.AppendLine(" AND Programmazione_Entita.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Entita_Des <> "" Then
                StrSQL.AppendLine(" AND Programmazione_Entita.Entita_Des like '%" & Agro_SQL_SaveText(Entita_Des) & "%' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Programmazione_Entita.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod.ToString))
            End If

            If Campo_Cod <> 0 Then
                StrSQL.AppendLine(" AND Programmazione_Entita.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod.ToString))
            End If

            If Appezza <> 0 Then
                StrSQL.AppendLine(" AND Programmazione_Entita.Appezza = " & Agro_SQL_SaveNum(Appezza.ToString))
            End If

            If Id_Reg <> 0 Then
                StrSQL.AppendLine(" AND Programmazione_Entita.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg.ToString))
            End If

            If Progetto_Cod <> 0 Then
                StrSQL.AppendLine(" AND Programmazione_Entita.Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod.ToString))
            End If


            If xFiltroAggiuntivo2 <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo2,, objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   Programmazione_Entita.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   Programmazione_Entita.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")

            End Select
            StrSQL.AppendLine(" ) ")



            StrSQL.AppendLine(" ) AS Entita ")

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Entita_Des ASC ")
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


    Public Function Leggi_solo_Programmazione_entita(ByVal Programmazione_Cod As Integer,
                                                     ByVal Programmazione_Entita_Cod As Integer,
                                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                     Optional Campo_Cod As Integer = 0,
                                                     Optional xFiltroAggiuntivo As String = ""
                                                     ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_R.Leggi()"

        Dim MessaggioErrore As String
        Dim DT As DataTable
        Dim StrSQL As New System.Text.StringBuilder

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT *  ")

            StrSQL.AppendLine(" FROM    Programmazione_Entita ")
            StrSQL.AppendLine(" Where  1=1 ")

            If Programmazione_Cod <> 0 Then
                StrSQL.AppendLine(" and   Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod))
            End If

            If Programmazione_Entita_Cod <> 0 Then
                StrSQL.AppendLine(" and   Programmazione_Entita_Cod = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod))
            End If

            If Campo_Cod <> 0 Then
                StrSQL.AppendLine(" and   Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod))
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append("AND  " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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

    ''' <summary>
    ''' Lettura dei planning di tipo richiesta Piante per Zespri et al.
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="ValiditaInizio"></param>
    ''' <param name="ValiditaFine"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    Public Function LeggiProgettoPianificazioneRichiestaPiante(
        ByVal Piva As String,
        ByVal ValiditaInizio As Date,
        ByVal ValiditaFine As Date,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_R.Leggi()"

        Dim MessaggioErrore As String
        Dim DT As DataTable
        Dim stb As New System.Text.StringBuilder

        Try

            stb.Length = 0
            stb.AppendLine("  select  ")
            stb.AppendLine("    t.Programmazione_Des ")
            stb.AppendLine("  , e.Entita_Des ")
            stb.AppendLine("  , e.Riferimento_Alfanumerico_Appezzamento ")
            stb.AppendLine("  , isnull(veg.veg_des, '') as veg_des ")
            stb.AppendLine("  , isnull(uso.descrizione, '') as DestinazioneUso_Des ")
            stb.AppendLine("  , isnull(kpin.val_cod, '') as kpin ")
            stb.AppendLine("  , isnull(blkname.val_cod, '') as blockName ")
            stb.AppendLine("  , t.Validita_Inizio as Programmazione_Validita_Inizio ")
            stb.AppendLine("  , t.Validita_Fine as Programmazione_Validita_Fine ")
            stb.AppendLine("  , t.programmazione_cod  ")
            stb.AppendLine("  , e.programmazione_entita_cod ")
            stb.AppendLine("  , e.Data_Semina as DataPrevistaInnestoTrapianto ")
            stb.AppendLine(" from programmazione_testata t ")
            stb.AppendLine("  inner join Programmazione_Entita e ")
            stb.AppendLine("      on t.Programmazione_Cod = e.Programmazione_Cod ")
            stb.AppendLine("  left join cultivar c ")
            stb.AppendLine("      on c.Cul_Cod = e.Cul_Cod ")
            stb.AppendLine("  left join specieVegetali veg ")
            stb.AppendLine("      on veg.veg_cod = c.Veg_Cod ")
            stb.AppendLine("  left join Codici_Anagrafe uso ")
            stb.AppendLine("      on uso.codice = e.Id_Cod ")
            stb.AppendLine("  left join Programmazione_Entita_Codici kpin ")
            stb.AppendLine("      on kpin.Programmazione_Entita_Cod = e.Programmazione_Entita_Cod ")
            stb.AppendLine("      and kpin.id_cod = " & enum_CodiciAnagrafe.Zespri_Codice_kPIN)
            stb.AppendLine("  left join Programmazione_Entita_Codici blkName ")
            stb.AppendLine("      on blkName.Programmazione_Entita_Cod = e.Programmazione_Entita_Cod ")
            stb.AppendLine("      and blkName.id_cod =  " & enum_CodiciAnagrafe.Zespri_Block_Name)
            stb.AppendLine("  ")

            stb.AppendLine(" where t.Tipo_Pianificazione = " & enum_TipoPianificazione.Pianificazione_RichiestaPiante)

            If Piva <> "" Then
                stb.AppendLine(" and e.piva = '" & Agro_SQL_SaveText(Piva) & "'  ")
            End If


            stb.AppendLine(" AND     e.Validita_Inizio <= " & Agro_SQL_SaveDate(ValiditaFine) & " ")
            stb.AppendLine(" AND     e.Validita_fine >= " & Agro_SQL_SaveDate(ValiditaInizio) & " ")

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If
            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function



    '################################################################################
    Public Function Leggi_x_Budget_Totali(ByVal _1_XVisualizzazioni_2X_modifiche As Integer, ByVal Programmazione_Cod As Integer,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_R.Leggi()"

        Dim MessaggioErrore As String
        Dim DT As DataTable
        Dim StrSQL As New System.Text.StringBuilder

        Try

            StrSQL.Length = 0


            If _1_XVisualizzazioni_2X_modifiche = 2 Then
                StrSQL.Append(" SELECT Programmazione_Entita.programmazione_entita_cod,  Programmazione_Entita.Appezza, Programmazione_Entita.Veg_Cod, Programmazione_Entita.Superficie, SpecieVegetali.Veg_Des, Programmazione_Entita.Piva,  ")
                StrSQL.Append("         Programmazione_Entita.Superficie_Futura +'' as Superficie_Futura")

                StrSQL.Append(" FROM    Programmazione_Entita INNER JOIN ")
                StrSQL.Append("       SpecieVegetali ON Programmazione_Entita.Veg_Cod = SpecieVegetali.Veg_Cod ")

                StrSQL.Append(" where 1=1  ")

                If Programmazione_Cod <> 0 Then
                    StrSQL.Append(" AND Programmazione_Entita.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod.ToString))
                End If

                StrSQL.Append(" order by veg_des  ")
            Else
                StrSQL.Append(" select * from( ")
                StrSQL.Append(" SELECT Programmazione_Entita.programmazione_entita_cod,  Programmazione_Entita.Appezza, Programmazione_Entita.Veg_Cod, Programmazione_Entita.Superficie, SpecieVegetali.Veg_Des, Programmazione_Entita.Piva,  ")
                StrSQL.Append("         Programmazione_Entita.Superficie_Futura +'' as Superficie_Futura,  SpecieVegetali.Veg_Des as Ordinamento")
                StrSQL.Append(" FROM    Programmazione_Entita INNER JOIN ")
                StrSQL.Append("       SpecieVegetali ON Programmazione_Entita.Veg_Cod = SpecieVegetali.Veg_Cod ")

                StrSQL.Append(" where 1=1  ")

                If Programmazione_Cod <> 0 Then
                    StrSQL.Append(" AND Programmazione_Entita.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod.ToString))
                End If


                StrSQL.Append(" union ")
                StrSQL.Append(" SELECT 0 as Programmazione_Entita_Cod, 0 as Appezza, 0 as Veg_Cod,sum(Programmazione_Entita.Superficie) as Superficie , 'TOTALE' as Veg_Des, Programmazione_Entita.Piva, ")
                StrSQL.Append("    sum (Programmazione_Entita.Superficie_Futura ) AS Superficie_Futura,  'zzzzzzzzzzzzzzzzzzzzzzzzzzz' as Ordinamento ")

                StrSQL.Append(" FROM    Programmazione_Entita INNER JOIN ")
                StrSQL.Append("       SpecieVegetali ON Programmazione_Entita.Veg_Cod = SpecieVegetali.Veg_Cod ")

                StrSQL.Append(" where 1=1  ")

                If Programmazione_Cod <> 0 Then
                    StrSQL.Append(" AND Programmazione_Entita.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod.ToString))
                End If


                StrSQL.Append("  group by Programmazione_Entita.Piva  ")

                StrSQL.Append("  ) a order by  a.Ordinamento  ")


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

    '################################################################################
    Public Function Leggi_x_Budget_Totalix_Export(ByVal Anno_corrente As String, ByVal Anno_futuro As String, ByVal Programmazione_Cod As Integer,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_R.Leggi()"

        Dim MessaggioErrore As String
        Dim DT As DataTable
        Dim StrSQL As New System.Text.StringBuilder

        Try

            StrSQL.Length = 0



            StrSQL.Append(" SELECT SpecieVegetali.Veg_Des as [Specie Vegetale],  ")
            StrSQL.Append("     Programmazione_Entita.Superficie as [Superficie " & Anno_corrente & "] , ")
            StrSQL.Append("     Programmazione_Entita.Superficie_Futura +'' as [Superficie " & Anno_futuro & "] ,")
            StrSQL.Append("     ( Programmazione_Entita.Superficie_Futura -Programmazione_Entita.Superficie ) as variazione , ")
            StrSQL.Append("     CASE Programmazione_Entita.Superficie WHEN 0 THEN '0' ELSE ROUND((Programmazione_Entita.Superficie_Futura - Programmazione_Entita.Superficie) * 100 / Programmazione_Entita.Superficie, 3) END AS percentuale ")

            StrSQL.Append(" FROM    Programmazione_Entita INNER JOIN ")
            StrSQL.Append("       SpecieVegetali ON Programmazione_Entita.Veg_Cod = SpecieVegetali.Veg_Cod ")

            StrSQL.Append(" where 1=1  ")

            If Programmazione_Cod <> 0 Then
                StrSQL.Append(" AND Programmazione_Entita.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod.ToString))
            End If

            StrSQL.Append(" order by veg_des  ")


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


    '################################################################################
    Public Function Leggi_x_Budget_Aziendali(ByVal _1_XVisualizzazioni_2X_modifiche As Integer, ByVal Programmazione_Cod As Integer,
                                             ByVal FiltroAggiuntivo As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_R.Leggi()"

        Dim MessaggioErrore As String
        Dim DT As DataTable
        Dim StrSQL As New System.Text.StringBuilder

        Try

            StrSQL.Length = 0

            If _1_XVisualizzazioni_2X_modifiche = 2 Then
                StrSQL.Append(" SELECT  Programmazione_Entita.Programmazione_Entita_Cod, Programmazione_Entita.Appezza, Programmazione_Entita.Veg_Cod, Programmazione_Entita.Superficie, SpecieVegetali.Veg_Des, Programmazione_Entita.Piva,  ")
                StrSQL.Append("       Programmazione_Entita.Superficie_Futura + '' AS Superficie_Futura, Imprese.rag_soc ")

                StrSQL.Append(" FROM         Programmazione_Entita INNER JOIN ")
                StrSQL.Append("       SpecieVegetali ON Programmazione_Entita.Veg_Cod = SpecieVegetali.Veg_Cod INNER JOIN ")
                StrSQL.Append("       Imprese ON Programmazione_Entita.Piva = Imprese.PIVA ")

                StrSQL.Append(" where 1=1  ")

                If Programmazione_Cod <> 0 Then
                    StrSQL.Append(" AND Programmazione_Entita.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod.ToString))
                End If
                If FiltroAggiuntivo <> "" Then
                    StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(FiltroAggiuntivo,, objParametri))
                End If

                StrSQL.Append(" order by rag_soc , veg_des  ")

            Else

                StrSQL.Append(" select * from( ")
                StrSQL.Append("     SELECT     Programmazione_Entita.Programmazione_Entita_Cod, Programmazione_Entita.Appezza, Programmazione_Entita.Veg_Cod, Programmazione_Entita.Superficie, SpecieVegetali.Veg_Des, Programmazione_Entita.Piva, Programmazione_Entita.Superficie_Futura + '' AS Superficie_Futura, Imprese.rag_soc, SpecieVegetali.Veg_Des as Ordinamento ")
                StrSQL.Append("     FROM         Programmazione_Entita INNER JOIN SpecieVegetali ON Programmazione_Entita.Veg_Cod = SpecieVegetali.Veg_Cod INNER JOIN Imprese ON Programmazione_Entita.Piva = Imprese.PIVA ")

                StrSQL.Append(" where 1=1  ")
                If Programmazione_Cod <> 0 Then
                    StrSQL.Append(" AND Programmazione_Entita.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod.ToString))
                End If
                If FiltroAggiuntivo <> "" Then
                    StrSQL.Append(" AND " & FiltroAggiuntivo)
                End If



                StrSQL.Append(" union ")
                StrSQL.Append(" SELECT 0 as Programmazione_Entita_Cod, 0 as Appezza, 0 as Veg_Cod,sum(Programmazione_Entita.Superficie) as Superficie , 'TOTALE' as Veg_Des, Programmazione_Entita.Piva,  sum (Programmazione_Entita.Superficie_Futura ) AS Superficie_Futura, Imprese.rag_soc  , 'zzzzzzzzzzzzzzzzzzzzzzzzzzz' as Ordinamento ")
                StrSQL.Append("     FROM         Programmazione_Entita INNER JOIN        SpecieVegetali ON Programmazione_Entita.Veg_Cod = SpecieVegetali.Veg_Cod INNER JOIN        Imprese ON Programmazione_Entita.Piva = Imprese.PIVA  ")

                StrSQL.Append(" where 1=1  ")
                If Programmazione_Cod <> 0 Then
                    StrSQL.Append(" AND Programmazione_Entita.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod.ToString))
                End If
                If FiltroAggiuntivo <> "" Then
                    StrSQL.Append(" AND " & FiltroAggiuntivo)
                End If
                StrSQL.Append("  group by Programmazione_Entita.Piva, Imprese.rag_soc ")

                StrSQL.Append("  ) a order by a.rag_soc, a.Ordinamento  ")

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



    '################################################################################
    Public Function Leggi_SpecieVegetali_gia_utilizzate(ByVal Programmazione_Cod As Integer,
                                           ByVal Piva As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_R.Leggi()"

        Dim MessaggioErrore As String
        Dim DT As DataTable
        Dim StrSQL As New System.Text.StringBuilder

        Try
            StrSQL.Length = 0

            StrSQL.Append(" select distinct veg_cod from programmazione_entita  ")
            StrSQL.Append(" where 1=1  ")

            If Piva <> "" Then
                StrSQL.Append(" AND Programmazione_Entita.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod.ToString))
            End If
            If Piva <> "" Then
                StrSQL.Append(" AND Programmazione_Entita.piva = '" & Agro_SQL_SaveText(Piva) & "'")
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

    Public Function Leggi_Piva_Sa_Cod_Distinct(
            ByVal Programmazione_Entita_cod_lista As String,
            ByVal FiltroAggiuntivo As String,
            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_R.Leggi_PlanningAppartenenza_Distinct()"

        Dim MessaggioErrore As String
        Dim DT As DataTable
        Dim StrSQL As New System.Text.StringBuilder

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT distinct piva, sa_cod ")

            StrSQL.Append(" FROM   Programmazione_Entita  ")

            StrSQL.Append(" where Programmazione_Entita_cod in (" & Agro_SQL_Save_Clausola_IN(Programmazione_Entita_cod_lista) & ")  ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function


    Public Function Leggi_PlanningAppartenenza_Distinct(
            ByVal Programmazione_Entita_cod_lista As String,
            ByVal JoinCentroAziendale As Boolean,
            ByVal Leggi_1_Programmazione_Testata_2_Entita As Integer,
            ByVal xFiltroAggiuntivo As String,
            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_R.Leggi_PlanningAppartenenza_Distinct()"

        Dim MessaggioErrore As String
        Dim DT As DataTable
        Dim stb As New System.Text.StringBuilder

        Try

            stb.Length = 0

            If Leggi_1_Programmazione_Testata_2_Entita = 1 Then
                stb.Append(" SELECT distinct t.programmazione_cod, t.programmazione_des ")
            Else
                stb.Append(" SELECT distinct programmazione_entita_cod ")
            End If


            stb.Append(" FROM   Programmazione_Entita ee " & vbCrLf)
            stb.Append(" INNER JOIN   Programmazione_Testata t " & vbCrLf)
            stb.Append(" On ee.Programmazione_Cod = t.Programmazione_cod " & vbCrLf)

            If JoinCentroAziendale Then
                stb.Append(" inner join centri_aziendali " & vbCrLf)
                stb.Append("        on centri_aziendali.piva = ee.piva " & vbCrLf)
                stb.Append("        and centri_aziendali.sa_cod = ee.sa_cod " & vbCrLf)
                stb.Append("    inner join imprese  " & vbCrLf)
                stb.Append("        on imprese.piva = ee.piva  " & vbCrLf)
                stb.Append(" ")

            End If

            stb.Append(" where 1=1 " & vbCrLf)

            If Programmazione_Entita_cod_lista <> "" Then
                stb.Append(" and Programmazione_Entita_cod in (" & Agro_SQL_Save_Clausola_IN(Programmazione_Entita_cod_lista) & ")  " & vbCrLf)
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.Append(xFiltroAggiuntivo & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    '################################################################################


    Public Function Leggi_x_Budget_Aziendalix_Export(ByVal Anno_corrente As String, ByVal Anno_futuro As String, ByVal Programmazione_Cod As Integer,
                                                     ByVal FiltroAggiuntivo As String,
                                             ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_R.Leggi_x_Budget_Aziendalix_Export()"

        Dim MessaggioErrore As String
        Dim DT As DataTable
        Dim StrSQL As New System.Text.StringBuilder

        Try

            StrSQL.Length = 0

            'Dim i As Integer
            'Dim appoggio As String = objParametri_Utenti.StringaConnessione
            ''Dim dbUtente As String

            'For i = 0 To appoggio.Split(";").Length - 1
            '    If appoggio.Split(";")(i).StartsWith("Initial Catalog=") Then
            '        dbUtente = appoggio.Split(";")(i).Split("=")(1)
            '    End If
            'Next


            'Dim dbUtente As String = objParametri_Utenti.StringaConnessione.Split("Initial Catalog=")(1).Split(";")(0)



            StrSQL.Append(" SELECT '" & objParametri_Server.UtenteUsername & "' as Tecnico, Lista_Province.PROVINCIA, Imprese.rag_soc as [Ragione Sociale],Imprese.Piva ,SpecieVegetali.Veg_Des as [Specie Vegetale],  ")
            StrSQL.Append("     Programmazione_Entita.Superficie as [Superficie " & Anno_corrente & "] , ")
            StrSQL.Append("     Programmazione_Entita.Superficie_Futura +'' as [Superficie " & Anno_futuro & "] ,")
            StrSQL.Append("     ( Programmazione_Entita.Superficie_Futura -Programmazione_Entita.Superficie ) as variazione , ")
            StrSQL.Append("     CASE Programmazione_Entita.Superficie WHEN 0 THEN '0' ELSE ROUND((Programmazione_Entita.Superficie_Futura - Programmazione_Entita.Superficie) * 100 / Programmazione_Entita.Superficie, 3) END AS percentuale ")
            'StrSQL.Append("     , ISNULL( (SELECT    top 1 " & dbUtente & ".dbo.Utenti_Dettagli.[USERname] FROM         Imprese_Codici INNER JOIN " & dbUtente & ".dbo.Utenti_Dettagli ON Imprese_Codici.val_cod =  " & dbUtente & ".dbo.Utenti_Dettagli.codfisc  WHERE (Imprese_Codici.id_cod = 1088) and Imprese_Codici.PIVA = Imprese.Piva), '') as [Tecnico Riferimento] ")

            StrSQL.Append("     , ISNULL( (SELECT TOP 1 Contatti.Rag_Soc + Contatti.Nome + ' ' + Contatti.Cognome AS Rag_Soc  FROM         Imprese_Codici INNER JOIN contatti ON Imprese_Codici.val_cod =  contatti.cod_contatto  WHERE (Imprese_Codici.id_cod = 1088) and Imprese_Codici.PIVA = Imprese.Piva), '') as [Tecnico Riferimento]  ")


            StrSQL.Append(" FROM   Programmazione_Entita INNER JOIN ")
            StrSQL.Append("       SpecieVegetali ON Programmazione_Entita.Veg_Cod = SpecieVegetali.Veg_Cod INNER JOIN ")
            StrSQL.Append("       Imprese ON Programmazione_Entita.Piva = Imprese.PIVA INNER JOIN ")
            StrSQL.Append("       ImpresexIndirizzi ON Imprese.PIVA = ImpresexIndirizzi.PIVA INNER JOIN ")
            StrSQL.Append("       Indirizzi ON ImpresexIndirizzi.cod_indirizzo = Indirizzi.cod_indirizzo INNER JOIN ")
            StrSQL.Append("       Lista_Province ON Indirizzi.pro_cod_istat = Lista_Province.PROV ")

            StrSQL.Append(" where 1=1  ")

            If Programmazione_Cod <> 0 Then
                StrSQL.Append(" AND Programmazione_Entita.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod.ToString))
            End If
            If FiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & FiltroAggiuntivo)
            End If
            StrSQL.Append(" order by Imprese.rag_soc, SpecieVegetali.Veg_Des  ")


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function




    '################################################################################
    'LEGGE ENTITA 
    'IN JOIN CON TESTATA 
    'IN JOIN CON Programmazione_particelle 
    'IN JOIN CON IMPRESE E INDIRIZZI
    Public Function LeggixEsportazione(ByVal Programmazione_Cod As Integer,
                            ByVal Programmazione_Entita_Cod As Integer,
                            ByVal Entita_Des As String,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Integer,
                            ByVal Campo_Cod As Integer,
                            ByVal Appezza As Integer,
                            ByVal Id_Reg As Integer,
                            ByVal Progetto_Cod As Integer,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                            ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_R.Leggi()"

        Dim MessaggioErrore As String
        Dim DT As DataTable
        Dim StrSQL As New System.Text.StringBuilder

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT  Programmazione_Entita.*, ")
            StrSQL.Append("         ISNULL(SpecieVegetali.Veg_Des,'') AS Veg_Des, ")
            StrSQL.Append("         ISNULL(Cultivar.Cul_Des,'') AS Cul_Des, ")
            StrSQL.Append("         ISNULL(Codici_Anagrafe.descrizione, '') AS DestinazioneUso_Des, ")
            StrSQL.Append("         Programmazione_Testata.Programmazione_Des, ")
            StrSQL.Append("         Programmazione_Testata.Validita_Inizio AS Programmazione_Validita_Inizio, ")
            StrSQL.Append("         Programmazione_Testata.Validita_Fine AS Programmazione_Validita_Fine, ")
            StrSQL.Append("         Programmazione_Testata.Tipo_Pianificazione, ")

            StrSQL.Append("         Imprese.rag_soc, Imprese_Codici.val_cod AS cuaa, ")
            StrSQL.Append("         Case WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN Imprese.PIVA ELSE Imprese.partitaIvaReale END AS partitaIvaReale, ")
            StrSQL.Append("         Indirizzi.ind_des, Indirizzi.CAP, ")
            StrSQL.Append("         Indirizzi.com_des as comune_impresa, Indirizzi.pro_cod as provincia_impresa, ")
            StrSQL.Append("         Indirizzi.pro_cod_istat as pro_cod_istat_impresa, Indirizzi.com_cod_istat as com_cod_istat_impresa, ")

            StrSQL.Append("         ISNULL(Programmazione_Particelle.Prov,'') as pro_cod_istat_particella, ISNULL(Programmazione_Particelle.Com,'') as com_cod_istat_particella,  ")
            StrSQL.Append("         ISNULL(ISTAT.LOCALITA,'') as comune_particella, ISNULL(ISTAT.COMUNI_PROV,'') as provincia_particella,  ")
            StrSQL.Append("         ISNULL(Programmazione_Particelle.Sezione, '0') AS sezione, ISNULL(Programmazione_Particelle.Foglio,0) as foglio,  ")
            StrSQL.Append("         ISNULL(Programmazione_Particelle.Numero,0) as numero, ISNULL(Programmazione_Particelle.Subalterno,'0') as subalterno, ISNULL(Programmazione_Particelle.Superficie,0) AS Superficie_intersezione ")


            StrSQL.Append(" FROM    Codici_Anagrafe RIGHT OUTER JOIN ")
            StrSQL.Append("         Programmazione_Entita INNER JOIN ")
            StrSQL.Append("         Programmazione_Testata ON Programmazione_Entita.Programmazione_Cod = Programmazione_Testata.Programmazione_Cod AND  ")
            StrSQL.Append("         Programmazione_Entita.Piva_SuperUser = Programmazione_Testata.Piva_SuperUser INNER JOIN ")
            StrSQL.Append("         ImpresexIndirizzi INNER JOIN ")
            StrSQL.Append("         Indirizzi ON ImpresexIndirizzi.cod_indirizzo = Indirizzi.cod_indirizzo INNER JOIN ")
            StrSQL.Append("         Imprese ON ImpresexIndirizzi.PIVA = Imprese.PIVA ON Programmazione_Testata.Piva = Imprese.PIVA INNER JOIN ")
            StrSQL.Append("         Imprese_Codici ON ImpresexIndirizzi.PIVA = Imprese_Codici.PIVA ON Codici_Anagrafe.codice = Programmazione_Entita.Id_Cod LEFT OUTER JOIN ")
            StrSQL.Append("         SpecieVegetali INNER JOIN ")
            StrSQL.Append("         Cultivar ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ON Programmazione_Entita.Cul_Cod = Cultivar.Cul_Cod LEFT OUTER JOIN ")
            StrSQL.Append("         ISTAT INNER JOIN ")
            StrSQL.Append("         Programmazione_Particelle ON ISTAT.PROV = Programmazione_Particelle.Prov AND ISTAT.COM = Programmazione_Particelle.Com ON  ")
            StrSQL.Append("         Programmazione_Entita.Programmazione_Entita_Cod = Programmazione_Particelle.Programmazione_Entita_Cod And ")
            StrSQL.Append("         Programmazione_Entita.Piva_SuperUser = Programmazione_Particelle.Piva_SuperUser ")


            StrSQL.Append(" WHERE   Programmazione_Entita.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            StrSQL.Append(" AND     Programmazione_Entita.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.Append(" AND     Programmazione_Entita.Validita_fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            StrSQL.Append(" AND     Imprese_Codici.id_cod = 1010 ")

            If Programmazione_Cod <> 0 Then
                StrSQL.Append(" AND Programmazione_Entita.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod.ToString))
            End If

            If Programmazione_Entita_Cod <> 0 Then
                StrSQL.Append(" AND Programmazione_Entita.Programmazione_Entita_Cod = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod.ToString))
            End If

            If Piva <> "" Then
                StrSQL.Append(" AND Programmazione_Entita.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Entita_Des <> "" Then
                StrSQL.Append(" AND Programmazione_Entita.Entita_Des like '%" & Agro_SQL_SaveText(Entita_Des) & "%' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Programmazione_Entita.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod.ToString))
            End If

            If Campo_Cod <> 0 Then
                StrSQL.Append(" AND Programmazione_Entita.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod.ToString))
            End If

            If Appezza <> 0 Then
                StrSQL.Append(" AND Programmazione_Entita.Appezza = " & Agro_SQL_SaveNum(Appezza.ToString))
            End If

            If Id_Reg <> 0 Then
                StrSQL.Append(" AND Programmazione_Entita.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg.ToString))
            End If

            If Progetto_Cod <> 0 Then
                StrSQL.Append(" AND Programmazione_Entita.Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod.ToString))
            End If



            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Programmazione_Entita.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Programmazione_Entita.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")

            End Select

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Entita_Des ASC ")
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

    '################################################################################
    'LEGGE SOLO ENTITA SENZA JOIN 
    Public Function LeggiTabellaCompleta(ByVal Programmazione_Cod As Integer,
                                        ByVal Programmazione_Entita_Cod As Integer,
                                        ByVal Entita_Des As String,
                                        ByVal Piva As String,
                                        ByVal Sa_Cod As Integer,
                                        ByVal Campo_Cod As Integer,
                                        ByVal Appezza As Integer,
                                        ByVal Id_Reg As Integer,
                                        ByVal Progetto_Cod As Integer,
                                        ByVal Validita_Inizio As Date,
                                        ByVal Validita_Fine As Date,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_R.LeggiTabellaCompleta()"

        Dim MessaggioErrore As String
        Dim DT As DataTable
        Dim StrSQL As New System.Text.StringBuilder

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT  Programmazione_Entita.* ")

            StrSQL.Append(" FROM    Programmazione_Entita ")

            StrSQL.Append(" WHERE   Programmazione_Entita.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            StrSQL.Append(" AND     Programmazione_Entita.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.Append(" AND     Programmazione_Entita.Validita_fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            If Programmazione_Cod <> 0 Then
                StrSQL.Append(" AND Programmazione_Entita.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod.ToString))
            End If

            If Programmazione_Entita_Cod <> 0 Then
                StrSQL.Append(" AND Programmazione_Entita.Programmazione_Entita_Cod = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod.ToString))
            End If

            If Piva <> "" Then
                StrSQL.Append(" AND Programmazione_Entita.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Entita_Des <> "" Then
                StrSQL.Append(" AND Programmazione_Entita.Entita_Des like '%" & Agro_SQL_SaveText(Entita_Des) & "%' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Programmazione_Entita.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod.ToString))
            End If

            If Campo_Cod <> 0 Then
                StrSQL.Append(" AND Programmazione_Entita.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod.ToString))
            End If

            If Appezza <> 0 Then
                StrSQL.Append(" AND Programmazione_Entita.Appezza = " & Agro_SQL_SaveNum(Appezza.ToString))
            End If

            If Id_Reg <> 0 Then
                StrSQL.Append(" AND Programmazione_Entita.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg.ToString))
            End If

            If Progetto_Cod <> 0 Then
                StrSQL.Append(" AND Programmazione_Entita.Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod.ToString))
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Programmazione_Entita.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Programmazione_Entita.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")

            End Select

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Entita_Des ASC ")
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

    '################################################################################
    Public Function Programmazione_Entita_Leggi_conAreeOmogenee(
                                            ByVal Programmazione_Cod As Integer,
                                            ByRef MessaggioErrore As String,
                                            ByVal Programmazione_Entita_Cod As Integer,
                                            ByVal Entita_Des As String,
                                            ByVal Piva As String,
                                            ByVal Sa_Cod As Integer,
                                            ByVal Campo_Cod As Integer,
                                            ByVal Appezza As Integer,
                                            ByVal Id_Reg As Integer,
                                            ByVal Progetto_Cod As Integer,
                                            ByVal Validita_Inizio As Date,
                                            ByVal Validita_Fine As Date,
                                            ByVal SORT_Des1_Inizio2_Fine2 As Integer,
                                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        Optional ByVal flagLeggiDatiZero As Boolean = False
                                                ) As DataTable
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_R.Programmazione_Entita_Leggi_conAreeOmogenee()"


        Dim DT As DataTable

        Dim StrSQL As New System.Text.StringBuilder

        Try

            StrSQL.Length = 0

            StrSQL.Length = 0

            StrSQL.Append(" SELECT  Programmazione_Entita.*, ")
            StrSQL.Append("         ISNULL(Programmazione_Entita.Grva_Cod,0) AS Grva_Cod, ")
            StrSQL.Append("         ISNULL(SpecieVegetali.Veg_Des,'') AS Veg_Des, ")
            StrSQL.Append("         ISNULL(Cultivar.Cul_Des,'') AS Cul_Des, ")
            StrSQL.Append("         ISNULL(GruppoVarietale.Grva_Des,'') AS Grva_Des, ")
            StrSQL.Append("         ISNULL(GruppoFinalita.Grfi_Des,'') AS Grfi_Des, ")
            StrSQL.Append("         ISNULL(Copertura.Cop_Des,'') AS Cop_Des, ")
            StrSQL.Append("         ISNULL(Codici_Anagrafe.descrizione,'') AS DestinazioneUso_Des, ")
            StrSQL.Append("         Programmazione_Testata.Programmazione_Des, ")
            StrSQL.Append("         Programmazione_Testata.Validita_Inizio AS Programmazione_Validita_Inizio, ")
            StrSQL.Append("         Programmazione_Testata.Validita_Fine AS Programmazione_Validita_Fine, ")
            StrSQL.Append("         Programmazione_Testata.Tipo_Pianificazione, ")
            StrSQL.Append("         ISNULL(Area_OmogeneaxEntita.Area_Cod, 0) AS Area_Cod, ")
            StrSQL.Append("         ISNULL(Area_OmogeneaxEntita.Entita_Cod, 0) AS Entita_Cod, ")
            StrSQL.Append("         ISNULL(Programmazione_Entita.Veg_Cod_Agea, '') AS Veg_Cod_Agea, ")
            StrSQL.Append("         ISNULL(Programmazione_Entita.Cul_Cod_Agea, '') AS Cul_Cod_Agea, ")
            StrSQL.Append("         ISNULL(Programmazione_Entita.Uso_Cod_Agea, '') AS Uso_Cod_Agea, ")
            StrSQL.Append("         ISNULL(Programmazione_Entita.Occupazione_Cod_Agea, '') AS Occupazione_Cod_Agea, ")
            StrSQL.Append("         ISNULL(Programmazione_Entita.Destinazione_Cod_Agea, '') AS Destinazione_Cod_Agea, ")
            StrSQL.Append("         ISNULL(Programmazione_Entita.Qualita_Cod_Agea, '') AS Qualita_Cod_Agea ")

            StrSQL.Append(" FROM    Programmazione_Entita INNER JOIN ")
            StrSQL.Append(" Programmazione_Testata ON Programmazione_Entita.Programmazione_Cod = Programmazione_Testata.Programmazione_Cod ")
            StrSQL.Append(" AND Programmazione_Entita.Piva_SuperUser = Programmazione_Testata.Piva_SuperUser LEFT OUTER JOIN ")
            StrSQL.Append(" Area_OmogeneaxEntita ON Programmazione_Entita.Piva_SuperUser = Area_OmogeneaxEntita.Piva_SuperUser AND  ")
            StrSQL.Append(" Programmazione_Entita.Programmazione_Entita_Cod = Area_OmogeneaxEntita.Entita_Cod LEFT OUTER JOIN ")
            StrSQL.Append(" Codici_Anagrafe ON Programmazione_Entita.Id_Cod = Codici_Anagrafe.codice LEFT OUTER JOIN ")
            StrSQL.Append(" Copertura ON Programmazione_Entita.Cop_Cod = Copertura.Cop_Cod LEFT OUTER JOIN ")
            StrSQL.Append(" GruppoVarietale ON Programmazione_Entita.Grva_Cod = GruppoVarietale.Grva_Cod LEFT OUTER JOIN ")
            StrSQL.Append(" GruppoFinalita ON Programmazione_Entita.Grfi_Cod = GruppoFinalita.Grfi_Cod LEFT OUTER JOIN ")
            StrSQL.Append(" SpecieVegetali ON Programmazione_Entita.Veg_Cod = SpecieVegetali.Veg_Cod LEFT OUTER JOIN ")
            StrSQL.Append(" Cultivar ON Programmazione_Entita.Cul_Cod = Cultivar.Cul_Cod ")

            StrSQL.Append(" WHERE   Programmazione_Entita.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND     Programmazione_Entita.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.Append(" AND     Programmazione_Entita.Validita_fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")


            If Programmazione_Cod <> 0 Then
                StrSQL.Append(" AND Programmazione_Entita.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod.ToString))
            End If

            If Programmazione_Entita_Cod <> 0 Then
                StrSQL.Append(" AND Programmazione_Entita.Programmazione_Entita_Cod = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod.ToString))
            End If

            If Piva <> "" Then
                StrSQL.Append(" AND Programmazione_Entita.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Entita_Des <> "" Then
                StrSQL.Append(" AND Programmazione_Entita.Entita_Des like '%" & Agro_SQL_SaveText(Entita_Des) & "%' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Programmazione_Entita.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod.ToString))
            Else
                If flagLeggiDatiZero Then
                    StrSQL.Append(" AND Programmazione_Entita.Sa_Cod = 0 ")
                End If
            End If

            If Campo_Cod <> 0 Then
                StrSQL.Append(" AND Programmazione_Entita.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod.ToString))
            End If

            If Appezza <> 0 Then
                StrSQL.Append(" AND Programmazione_Entita.Appezza = " & Agro_SQL_SaveNum(Appezza.ToString))
            End If

            If Id_Reg <> 0 Then
                StrSQL.Append(" AND Programmazione_Entita.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg.ToString))
            End If

            If Progetto_Cod <> 0 Then
                StrSQL.Append(" AND Programmazione_Entita.Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod.ToString))
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Programmazione_Entita.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Programmazione_Entita.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")

            End Select

            Select Case SORT_Des1_Inizio2_Fine2
                Case 1
                    StrSQL.Append(" ORDER BY Programmazione_Entita.Entita_Des ASC ")
                Case 2
                    StrSQL.Append(" ORDER BY Programmazione_Entita.Validita_Inizio ASC ")
                Case 3
                    StrSQL.Append(" ORDER BY Programmazione_Entita.Validita_fine ASC ")
                Case Else

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Programmazione_Entita.Entita_Des ASC ")
                    End If
            End Select


            '----------------------------------------------------
            '--- Recupero il datatable --------------------------
            '----------------------------------------------------

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


    '################################################################################
    Public Function Programmazione_Specie_Leggi_2(ByVal Programmazione_Cod As Integer,
                                                  ByRef MessaggioErrore As String,
                                                  ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                                  ByVal xFiltroAggiuntivo As String,
                                                  ByVal xOrderBy As String,
                                                  ByRef objParametri As AgronicaCoreParametri
                                                  ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_R.Programmazione_Specie_Leggi_2()"

        Dim DT As DataTable
        Dim StrSQL As New StringBuilder

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT  DISTINCT PE.Veg_Cod_Cliente AS Veg_Cod, PE.Cul_Cod_Cliente AS Cul_Cod, ")

                    StrSQL.Append(" ISNULL( (SELECT TOP 1 CAC_Codifica_Colture.DESCRIZIONE_COLTURA ")
                    StrSQL.Append("          FROM CAC_Codifica_Colture ")
                    StrSQL.Append("          WHERE CAC_Codifica_Colture.COD_UTILIZZO = substring(PE.Veg_Cod_Cliente, 1, 2)  ")
                    StrSQL.Append("          AND CAC_Codifica_Colture.COD_COLTURA = substring(PE.Veg_Cod_Cliente, 3, 3)  ")
                    StrSQL.Append("          ), ''  ) AS Veg_Des, ")

                    StrSQL.Append(" ISNULL( (SELECT TOP 1 CAC_Codifica_Colture.DESCRIZIONE_VARIETA ")
                    StrSQL.Append("          FROM CAC_Codifica_Colture ")
                    StrSQL.Append("          WHERE CAC_Codifica_Colture.COD_UTILIZZO = substring(PE.Cul_Cod_Cliente, 1, 2)  ")
                    StrSQL.Append("          AND CAC_Codifica_Colture.COD_COLTURA = substring(PE.Cul_Cod_Cliente, 3, 3)  ")
                    StrSQL.Append("          AND CAC_Codifica_Colture.COD_VARIETA = substring(PE.Cul_Cod_Cliente,6,3)  ")
                    StrSQL.Append("          ), ''  ) AS Cul_Des ")

                    StrSQL.Append(" FROM    Programmazione_Entita PE ")

                    StrSQL.Append(" WHERE   PE.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

                    StrSQL.Append(" AND PE.Appezza <> 0 ")

                    If Programmazione_Cod <> 0 Then
                        StrSQL.Append(" AND PE.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod.ToString))
                    End If



                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Programmazione_Entita.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Programmazione_Entita.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Veg_Cod ")
                    End If




                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT  DISTINCT PE.Veg_Cod_Cliente AS Veg_Cod, PE.Cul_Cod_Cliente AS Cul_Cod, ")

                    StrSQL.Append(" ISNULL( (SELECT TOP 1 CAC_Codifica_Colture.DESCRIZIONE_COLTURA ")
                    StrSQL.Append("          FROM CAC_Codifica_Colture ")
                    StrSQL.Append("          WHERE CAC_Codifica_Colture.COD_UTILIZZO = substring(PE.Veg_Cod_Cliente, 1, 2)  ")
                    StrSQL.Append("          AND CAC_Codifica_Colture.COD_COLTURA = substring(PE.Veg_Cod_Cliente, 3, 3)  ")
                    StrSQL.Append("          ), ''  ) AS Veg_Des, ")

                    StrSQL.Append(" ISNULL( (SELECT TOP 1 CAC_Codifica_Colture.DESCRIZIONE_VARIETA ")
                    StrSQL.Append("          FROM CAC_Codifica_Colture ")
                    StrSQL.Append("          WHERE CAC_Codifica_Colture.COD_UTILIZZO = substring(PE.Cul_Cod_Cliente, 1, 2)  ")
                    StrSQL.Append("          AND CAC_Codifica_Colture.COD_COLTURA = substring(PE.Cul_Cod_Cliente, 3, 3)  ")
                    StrSQL.Append("          AND CAC_Codifica_Colture.COD_VARIETA = substring(PE.Cul_Cod_Cliente,6,3)  ")
                    StrSQL.Append("          ), ''  ) AS Cul_Des ")

                    StrSQL.Append(" FROM    Programmazione_Entita PE ")

                    StrSQL.Append(" WHERE   PE.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

                    StrSQL.Append(" AND PE.Appezza <> 0 ")

                    If Programmazione_Cod <> 0 Then
                        StrSQL.Append(" AND PE.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod.ToString))
                    End If



                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Programmazione_Entita.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Programmazione_Entita.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Veg_Cod ")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case enumSelezioneVariabile.Selezione_JoinCompleta


            End Select

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


    '#########################################################################
    Public Function DestinazioniUso(ByVal Programmazione_Cod As Integer,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_R.DestinazioniUso()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT DISTINCT Codici_Anagrafe.codice, Codici_Anagrafe.descrizione ")
            StrSQL.Append(" FROM   Codici_Anagrafe ")
            StrSQL.Append(" INNER JOIN Programmazione_Entita ON Programmazione_Entita.Id_Cod = Codici_Anagrafe.Codice ")
            StrSQL.Append(" WHERE   Codici_Anagrafe.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
            StrSQL.Append(" AND     Codici_Anagrafe.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            StrSQL.Append(" AND (Codice >= 3000) AND (gruppo = 'TERRENO') ")

            If Programmazione_Cod <> 0 Then
                StrSQL.Append(" AND Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND    " & xFiltroAggiuntivo & "   ")
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   (Codici_Anagrafe.inviato >= 0) ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   (Codici_Anagrafe.inviato =-1) ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select


            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Codici_Anagrafe.descrizione  ")
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


    '#########################################################################
    Public Function SpecieVegetali(ByVal Programmazione_Cod As Integer,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_R.SpecieVegetali()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT DISTINCT SpecieVegetali.Veg_Cod, SpecieVegetali.Veg_Des ")
            StrSQL.Append(" FROM   SpecieVegetali ")
            StrSQL.Append(" INNER JOIN Programmazione_Entita ON Programmazione_Entita.Veg_Cod = SpecieVegetali.Veg_Cod ")
            StrSQL.Append(" WHERE   SpecieVegetali.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
            StrSQL.Append(" AND     SpecieVegetali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")


            If Programmazione_Cod <> 0 Then
                StrSQL.Append(" AND Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND    " & xFiltroAggiuntivo & "   ")
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   (SpecieVegetali.inviato >= 0) ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   (SpecieVegetali.inviato =-1) ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select


            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY SpecieVegetali.Veg_Des  ")
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

    '#########################################################################
    Public Function Cultivar(ByVal Programmazione_Cod As Integer,
                             ByVal Veg_Cod As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByVal xOrderBy As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_R.Cultivar()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT DISTINCT Cultivar.Cul_Cod, Cultivar.Cul_Des ")
            StrSQL.Append(" FROM   Cultivar ")
            StrSQL.Append(" INNER JOIN Programmazione_Entita ON Programmazione_Entita.Cul_Cod = Cultivar.Cul_Cod ")
            StrSQL.Append(" WHERE   Cultivar.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
            StrSQL.Append(" AND     Cultivar.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Veg_Cod <> 0 Then
                StrSQL.Append(" AND     Cultivar.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
            End If

            If Programmazione_Cod <> 0 Then
                StrSQL.Append(" AND Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod) & " ")
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND    " & xFiltroAggiuntivo & "   ")
            End If


            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   (Cultivar.inviato >= 0) ")

                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   (Cultivar.inviato =-1) ")

                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select


            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Cultivar.Cul_Des  ")
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

    Public Function Centri(ByVal Programmazione_Cod As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_R.Centri()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT DISTINCT Centri_Aziendali.Sa_Cod, Centri_Aziendali.Sa_Nome ")
            StrSQL.Append(" FROM   Centri_Aziendali ")
            StrSQL.Append(" INNER JOIN Programmazione_Entita ON Programmazione_Entita.Sa_Cod = Centri_Aziendali.Sa_Cod ")
            StrSQL.Append(" AND Programmazione_Entita.Piva = Centri_Aziendali.Piva ")
            StrSQL.Append(" WHERE   1 = 1 ")


            If Programmazione_Cod <> 0 Then
                StrSQL.Append(" AND Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND    " & xFiltroAggiuntivo & "   ")
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   (Centri_Aziendali.inviato >= 0) ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   (Centri_Aziendali.inviato =-1) ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select


            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Centri_Aziendali.Sa_Nome ")
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

    Public Function Campi(ByVal Programmazione_Cod As Integer,
                            ByVal Sa_Cod As Integer,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_R.Campi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT DISTINCT Campi.Campo_Cod, Campi.Campo_Des, Centri_Aziendali.Sa_Cod, Centri_Aziendali.Sa_Nome ")
            StrSQL.Append(" FROM   Centri_Aziendali  ")
            StrSQL.Append(" INNER JOIN Programmazione_Entita ON Programmazione_Entita.Sa_Cod = Centri_Aziendali.Sa_Cod ")
            StrSQL.Append(" AND Programmazione_Entita.Piva = Centri_Aziendali.Piva ")
            StrSQL.Append(" INNER JOIN Campi ON Programmazione_Entita.Piva = Campi.Piva ")
            StrSQL.Append(" AND Programmazione_Entita.Sa_Cod = Campi.Sa_Cod ")
            StrSQL.Append(" AND Programmazione_Entita.Campo_Cod = Campi.Campo_Cod ")
            StrSQL.Append(" WHERE  Programmazione_Entita.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")


            If Programmazione_Cod <> 0 Then
                StrSQL.Append(" AND Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND    " & xFiltroAggiuntivo & "   ")
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   (Centri_Aziendali.inviato >= 0) ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   (Centri_Aziendali.inviato =-1) ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select


            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Campi.Campo_Des ")
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

    '#########################################################################
    Public Function Macrousi(ByVal Programmazione_Cod As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByVal xOrderBy As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_R.Macrousi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT DISTINCT Macrousi.Macrouso_Cod, Macrousi.Macrouso_Des ")
            StrSQL.Append(" FROM   Macrousi ")
            StrSQL.Append(" INNER JOIN Programmazione_Entita ON Programmazione_Entita.Macrouso_Cod = Macrousi.Macrouso_Cod ")
            StrSQL.Append(" WHERE   Macrousi.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
            StrSQL.Append(" AND     Macrousi.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")


            If Programmazione_Cod <> 0 Then
                StrSQL.Append(" AND Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND    " & xFiltroAggiuntivo & "   ")
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   (Macrousi.inviato >= 0) ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   (Macrousi.inviato =-1) ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select


            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Macrousi.Macrouso_Des  ")
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

    '#########################################################################
    Public Function Utilizzi(ByVal Programmazione_Cod As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByVal xOrderBy As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_R.Utilizzi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT DISTINCT Codifica_SpecieVegetali_Agea.Veg_Cod_Agea, Codifica_SpecieVegetali_Agea.Veg_Des_Agea ")
            StrSQL.Append(" FROM   Codifica_SpecieVegetali_Agea ")
            StrSQL.Append(" INNER JOIN Programmazione_Entita ON Programmazione_Entita.Veg_Cod_Cliente = Codifica_SpecieVegetali_Agea.Veg_Cod_Agea ")
            StrSQL.Append(" WHERE   Codifica_SpecieVegetali_Agea.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
            StrSQL.Append(" AND     Codifica_SpecieVegetali_Agea.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")


            If Programmazione_Cod <> 0 Then
                StrSQL.Append(" AND Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND    " & xFiltroAggiuntivo & "   ")
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   (Codifica_SpecieVegetali_Agea.inviato >= 0) ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   (Codifica_SpecieVegetali_Agea.inviato =-1) ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select


            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Codifica_SpecieVegetali_Agea.Veg_Des_Agea  ")
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


    Public Function Leggi_Entita_Data_Particella(ByVal Programmazione_Cod As Integer?,
                                                 ByVal piva As String,
                                                 ByVal Prov As String,
                                                 ByVal Com As String,
                                                 ByVal Sezione As String,
                                                 ByVal Foglio As Integer?,
                                                 ByVal Numero As Integer?,
                                                 ByVal subalterno As String,
                                                 ByVal macrouso_cod As String,
                                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_R.Utilizzi()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Try

            Stb.AppendLine(" Select * From Programmazione_Entita pe ")
            Stb.AppendLine(" Left Join Programmazione_Particelle pp ON pe.Programmazione_Entita_Cod = pp.Programmazione_Entita_Cod ")
            Stb.AppendLine(" Left Join Programmazione_Testata pt ON pe.Programmazione_Cod=pt.Programmazione_Cod ")
            Stb.AppendLine(" WHERE 1=1  ")
            If Prov IsNot Nothing Then
                Stb.AppendLine(" And pp.Prov =" & Agro_SQL_SaveText_NULL(Prov) & "  ")
            End If
            If Com IsNot Nothing Then
                Stb.AppendLine(" And pp.Com =" & Agro_SQL_SaveText_NULL(Com) & " ")
            End If
            If Sezione IsNot Nothing Then
                Stb.AppendLine(" And pp.Sezione =" & Agro_SQL_SaveText_NULL(Sezione) & " ")
            End If
            If Foglio IsNot Nothing Then
                Stb.AppendLine(" And pp.Foglio = " & Agro_vb_SaveNum(Foglio) & " ")
            End If
            If Numero IsNot Nothing Then
                Stb.AppendLine(" And pp.Numero = " & Agro_vb_SaveNum(Numero) & " ")
            End If
            If subalterno IsNot Nothing Then
                Stb.AppendLine(" And pp.Subalterno = " & Agro_SQL_SaveText_NULL(subalterno) & " ")
            End If
            If macrouso_cod <> "" Then
                Stb.AppendLine(" And pe.Macrouso_Cod= " & Agro_SQL_SaveText_NULL(macrouso_cod) & " ")
            End If
            If Programmazione_Cod IsNot Nothing Then
                Stb.AppendLine(" And pt.Programmazione_Cod = " & Agro_vb_SaveNum(Programmazione_Cod) & " ")
            End If
            If piva <> "" Then
                Stb.AppendLine(" And pt.piva = " & Agro_SQL_SaveText_NULL(piva) & " ")
            End If


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function

    Public Function Leggi_Macrousi_Entita_Data_Particella(ByVal Programmazione_Cod As Integer,
                                                 ByVal piva As String,
                                                 ByVal Prov As String,
                                                 ByVal Com As String,
                                                 ByVal Sezione As String,
                                                 ByVal Foglio As Integer,
                                                 ByVal Numero As Integer,
                                                 ByVal subalterno As String,
                                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_R.Utilizzi()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Try

            Stb.AppendLine(" Select Distinct(Macrouso_Cod) From Programmazione_Entita pe ")
            Stb.AppendLine(" Left Join Programmazione_Particelle pp ON pe.Programmazione_Entita_Cod = pp.Programmazione_Entita_Cod ")
            Stb.AppendLine(" Left Join Programmazione_Testata pt ON pe.Programmazione_Cod=pt.Programmazione_Cod ")
            Stb.AppendLine(" WHERE pp.Prov =" & Agro_SQL_SaveText_NULL(Prov) & "  ")
            Stb.AppendLine(" And pp.Com =" & Agro_SQL_SaveText_NULL(Com) & " ")
            Stb.AppendLine(" And pp.Sezione =" & Agro_SQL_SaveText_NULL(Sezione) & " ")
            Stb.AppendLine(" And pp.Foglio = " & Agro_vb_SaveNum(Foglio) & " ")
            Stb.AppendLine(" And pp.Numero = " & Agro_vb_SaveNum(Numero) & " ")
            Stb.AppendLine(" And pp.Subalterno = " & Agro_SQL_SaveText_NULL(subalterno) & " ")
            Stb.AppendLine(" And pt.Programmazione_Cod = " & Agro_vb_SaveNum(Programmazione_Cod) & " ")
            Stb.AppendLine(" And pt.piva = " & Agro_SQL_SaveText_NULL(piva) & " ")


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function

    Public Function PrenotazionePiante_Riepilogo(
                                             ByVal Programmazione_Cod As Integer,
                                             ByVal Programmazione_Entita_Cod As Integer,
                                             ByVal Piva As String,
                                             ByVal Data As Date,
                                             ByVal Filtro_Visibilita_Utente As Boolean,
                                             ByVal xFiltroAggiuntivo As String,
                                             ByVal xOrderBy As String,
                                             ByVal Sintetico As Boolean,
                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_R.Prenotazione_Piante_Riepilogo()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Try


            Stb.AppendLine(" SELECT  ")
            Stb.AppendLine(" Padre.Piva as Piva_Padre, ")
            Stb.AppendLine(" Padre.rag_soc as Rag_soc_Padre, ")
            Stb.AppendLine(" Imprese.Piva, ")
            Stb.AppendLine(" ISNULL(Imprese.partitaIvaReale, Imprese.PIVA) as partitaIvaReale,  ")
            Stb.AppendLine(" Cuaa.val_cod as CUAA, ")
            Stb.AppendLine(" codice_socio.val_cod AS Codice_Socio, ")
            Stb.AppendLine(" Imprese.rag_soc, ")

            Stb.AppendLine(" Indirizzo_Impresa.Ind_des as Impresa_ind_des, ")
            Stb.AppendLine(" Indirizzo_Impresa.frz_des as Impresa_frz_des, ")
            Stb.AppendLine(" Indirizzo_Impresa.CAP as Impresa_CAP, ")
            Stb.AppendLine(" Indirizzo_Impresa.pro_cod_istat as Impresa_pro_cod_istat, ")
            Stb.AppendLine(" ISTAT_Impresa.COMUNI_PROV as Impresa_COMUNI_PROV, ")
            Stb.AppendLine(" Indirizzo_Impresa.com_cod_istat as Impresa_com_cod_istat, ")
            Stb.AppendLine(" ISTAT_Impresa.LOCALITA as Impresa_LOCALITA, ")
            Stb.AppendLine(" Indirizzo_Impresa.stato as Impresa_Stato, ")
            Stb.AppendLine(" LR_Impresa.Regione_Des as Impresa_Regione, ")

            Stb.AppendLine(" Programmazione_Entita.Programmazione_Cod, ")
            Stb.AppendLine(" Programmazione_Entita.Programmazione_Entita_Cod, ")
            Stb.AppendLine(" Programmazione_Entita.Entita_Des, ")
            Stb.AppendLine(" Programmazione_Entita.Veg_Cod, ")
            Stb.AppendLine(" SpecieVegetali.Veg_Des, ")
            Stb.AppendLine(" Programmazione_Entita.Cul_Cod, ")
            Stb.AppendLine(" Cultivar.Cul_Des, ")
            Stb.AppendLine(" Programmazione_Entita.Superficie, ")
            Stb.AppendLine(" Programmazione_Entita.Num_Piante, ")
            Stb.AppendLine(" Programmazione_Entita.TRA_Fila, ")
            Stb.AppendLine(" Programmazione_Entita.SU_Fila, ")
            Stb.AppendLine(" Programmazione_Entita.Cop_Cod, ")
            Stb.AppendLine(" Copertura.Cop_Des, ")
            Stb.AppendLine(" Programmazione_Entita.Validita_Inizio_Impianto, ")
            Stb.AppendLine(" Programmazione_Entita.Data_Semina, ")
            Stb.AppendLine(" Programmazione_Entita.Data_Creazione, ")

            Stb.AppendLine(" Indirizzo_Entita.Ind_des as Entita_ind_des, ")
            Stb.AppendLine(" Indirizzo_Entita.frz_des as Entita_frz_des, ")
            Stb.AppendLine(" Indirizzo_Entita.CAP as Entita_CAP, ")
            Stb.AppendLine(" Indirizzo_Entita.pro_cod_istat as Entita_pro_cod_istat, ")
            Stb.AppendLine(" ISTAT_Entita.COMUNI_PROV as Entita_COMUNI_PROV, ")
            Stb.AppendLine(" Indirizzo_Entita.com_cod_istat as Entita_com_cod_istat, ")
            Stb.AppendLine(" ISTAT_Entita.LOCALITA as Entita_LOCALITA, ")
            Stb.AppendLine(" Indirizzo_Entita.stato as Entita_Stato, ")
            Stb.AppendLine(" LR_Entita.Regione_Des as Entita_Regione, ")

            Stb.AppendLine(" Pratiche.Pratica_Cod, ")
            Stb.AppendLine(" Pratiche.Pratica_Des, ")
            Stb.AppendLine(" Pratiche.Servizio_Cod, ")
            Stb.AppendLine(" Servizi.Servizio_Des, ")
            Stb.AppendLine(" Pratiche_Stati.Stato_Cod, ")
            Stb.AppendLine(" Pratiche_Stati.Note, ")
            Stb.AppendLine(" Pratiche_Stati.Data_Modifica, ")
            Stb.AppendLine(" WAnagraficaStati.WAnagraficaStati_Des, ")

            Stb.AppendLine(" Contatti.Nome, ")
            Stb.AppendLine(" Contatti.Cognome, ")
            Stb.AppendLine(" ISNULL(Contatti.Data_Nascita, CONVERT(datetime, '1900-01-01 00:00:00.000', 120)) as Data_Nascita, ")
            Stb.AppendLine(" Indirizzo_Legale.ind_des As Indirizzo_Legale, ")
            Stb.AppendLine(" Indirizzo_Legale.frz_des as Frazione_Legale, ")
            Stb.AppendLine(" Indirizzo_Legale.stato As Stato_Legale, ")
            Stb.AppendLine(" ISTAT_Legale.COMUNI_PROV as Prov_Legale, ")
            Stb.AppendLine(" ISTAT_Legale.Localita As Com_Legale, ")
            Stb.AppendLine(" Indirizzo_Legale.CAP as CAP_Legale, ")

            If Not Sintetico Then

                Stb.AppendLine("  Prenotazione.Num_Piante as [Num_Piante_Richiesta], ")
                Stb.AppendLine("  Prenotazione.Data_Semina As Data_Prenotazione, ")
                Stb.AppendLine("  Prenotazione.Stato_Ribaltamento as [Num_Piante_Maschi], ")
                Stb.AppendLine("  Prenotazione.Regolamento_Concimazione_Cod As [Num_Piante_Femmine], ")
                Stb.AppendLine("  Prenotazione.Riferimento_Alfanumerico_Appezzamento as [Codice_Prenotazione], ")
                Stb.AppendLine("  PrenotazioneT.NumColture as [Num_Prenotazione], ")
                Stb.AppendLine("  Vivaio.PIVA As [Piva_Vivaio], ")
                Stb.AppendLine("  Vivaio.rag_soc as [Rag_Soc_Vivaio],")

            End If

            Stb.AppendLine("  COALESCE(KPIN.Val_Cod, '') As KPIN,")
            Stb.AppendLine("  COALESCE(BlockName.Val_Cod, '') As Block_Name,")

            If Not Sintetico Then

                Stb.AppendLine("  SpecieVegetaliP.Veg_Des as Veg_Des_Ric, ")
                Stb.AppendLine("  CultivarP.Cul_Des as Cul_Des_Ric, ")
                Stb.AppendLine("  Prenotazione.Grva_Cod as N_Marze, ")

            End If


            Stb.AppendLine("    CASE WHEN (Programmazione_Entita.Programmazione_Entita_Cod IS NULL) THEN '' ELSE ISNULL(Programmazione_Entita.Entita_Des, '') + ' - ' + + ISNULL(SpecieVegetali.Veg_Des, '') + ' ' + ISNULL(cultivar.cul_des, '') + ' ' + 'KPIN:' + ISNULL(KPIN.val_cod, '') + ' - '  + 'Block Name:' + ISNULL(BlockName.val_cod, '') END as Descrizione_Progetto ")


            Stb.AppendLine(" , ISNULL(Zespri_phase.val_cod, 0) as ZespriFase_Cod  ")
            Stb.AppendLine(" , ISNULL(Zespri_phaseT.Descrizione, '') as ZespriFase_Des ")
            Stb.AppendLine(" , ISNULL(Zespri_type.val_cod, 0) as ZespriGrower_Cod ")
            Stb.AppendLine(" , ISNULL(Zespri_typeT.Descrizione, '') as ZespriGrower_Des ")
            Stb.AppendLine(" , ISNULL(Zespri_Grower.val_cod, 0) as ZespriTipo_Cod ")
            Stb.AppendLine(" , ISNULL(Zespri_GrowerT.Descrizione, '') as ZespriTipo_Des ")


            Stb.AppendLine(" , ISNULL(TipologiaDiInnesto.Val_Cod, 0) as TipologiaDiInnesto_Cod ")
            Stb.AppendLine(" , CASE TipologiaDiInnesto.Val_Cod
                                    WHEN NULL THEN '' 
                                    WHEN 0 THEN '' 
                                    WHEN 1 THEN 'innesto su portinnesto dell''anno' 
                                    WHEN 2 THEN 'innesto su portinnesto > 3 anni' 
                                    WHEN 3 THEN 'trapianto' 
                                    WHEN 4 THEN 'innesto su portinnesto 1 a 3 anni' 
                                    ELSE ''
                                END as TipologiaDiInnesto_Des")
            Stb.AppendLine(" , Programmazione_Entita.Port_Cod ")
            Stb.AppendLine(" , ISNULL(Portinnesti.Port_Des, '') as Port_Des ")

            Stb.AppendLine(" FROM Programmazione_Entita ")
            Stb.AppendLine(" JOIN Programmazione_Testata ON Programmazione_Entita.Programmazione_Cod = Programmazione_Testata.Programmazione_Cod ")
            Stb.AppendLine(" JOIN Imprese ON Programmazione_Entita.piva = Imprese.Piva ")
            Stb.AppendLine(" LEFT JOIN Imprese_Codici CUAA ON Imprese.Piva = CUAA.piva AND CUAA.id_Cod = 1010 ")
            Stb.AppendLine(" LEFT JOIN Imprese_Codici codice_socio ON Imprese.Piva = codice_socio.piva AND codice_socio.id_Cod = 1033 ")
            Stb.AppendLine(" LEFT JOIN GerarchiaImprese ON Imprese.Piva = GerarchiaImprese.Figlio ")
            Stb.AppendLine(" LEFT JOIN Imprese Padre ON GerarchiaImprese.Padre = Padre.PIVA ")
            Stb.AppendLine(" JOIN SpecieVegetali ON Programmazione_Entita.veg_Cod = SpecieVegetali.Veg_Cod ")
            Stb.AppendLine(" JOIN Cultivar ON Programmazione_Entita.Cul_Cod = Cultivar.cul_cod ")
            Stb.AppendLine(" JOIN Copertura ON Programmazione_Entita.Cop_Cod = Copertura.Cop_Cod ")
            Stb.AppendLine(" LEFT JOIN Pratiche ON Programmazione_Entita.programmazione_Entita_Cod = Pratiche.Programmazione_Entita_Cod ")
            Stb.AppendLine(" LEFT JOIN Pratiche_Stati_Attuali ON Pratiche.Pratica_Cod = Pratiche_Stati_Attuali.Pratica_Cod ")
            Stb.AppendLine(" LEFT JOIN Pratiche_Stati ON Pratiche_Stati.PassaggioDiStato_cod = (SELECT MAX(PassaggioDiStato_cod) FROM Pratiche_Stati WHERE Pratiche.Pratica_Cod = Pratiche_Stati.Pratica_Cod AND Pratiche_Stati.Stato_Cod = Pratiche_Stati_Attuali.Stato_Cod GROUP BY Pratica_Cod) ")
            Stb.AppendLine(" LEFT JOIN Servizi ON Pratiche.Servizio_Cod = Servizi.Servizio_Cod ")
            Stb.AppendLine(" LEFT JOIN WAnagraficaStati ON Pratiche_Stati_Attuali.Stato_Cod = WAnagraficaStati.WAnagraficaStati_Cod ")
            Stb.AppendLine(" LEFT JOIN ImpresexIndirizzi ON Imprese.Piva = ImpresexIndirizzi.Piva ")
            Stb.AppendLine(" LEFT JOIN Indirizzi Indirizzo_Impresa ON ImpresexIndirizzi.cod_indirizzo = Indirizzo_Impresa.Cod_Indirizzo ")
            Stb.AppendLine(" LEFT JOIN ISTAT ISTAT_Impresa ON Indirizzo_Impresa.pro_cod_istat = ISTAT_Impresa.PROV AND Indirizzo_Impresa.com_cod_istat = ISTAT_Impresa.COM ")
            Stb.AppendLine(" LEFT JOIN Lista_Province LP_Impresa ON ISTAT_Impresa.PROV = LP_Impresa.PROV ")
            Stb.AppendLine(" LEFT JOIN Lista_Regioni LR_Impresa ON LP_Impresa.REG = LR_Impresa.REG ")

            Stb.AppendLine(" LEFT JOIN Indirizzi Indirizzo_Entita ON Programmazione_Entita.Cod_Indirizzo = Indirizzo_Entita.cod_indirizzo ")
            Stb.AppendLine(" LEFT JOIN ISTAT ISTAT_Entita ON Indirizzo_Entita.pro_cod_istat = ISTAT_Entita.PROV AND Indirizzo_Entita.com_cod_istat = ISTAT_Entita.COM ")
            Stb.AppendLine(" LEFT JOIN Lista_Province LP_Entita ON ISTAT_Entita.PROV = LP_Entita.PROV ")
            Stb.AppendLine(" LEFT JOIN Lista_Regioni LR_Entita ON LR_Entita.REG = LP_Entita.REG ")

            Stb.AppendLine("  LEFT JOIN Risorse_Umane ON Risorse_Umane.Cod_RisUm = (SELECT MIN(Cod_RisUm) FROM Risorse_Umane WHERE Imprese.Piva = Risorse_Umane.Piva AND Risorse_Umane.Cod_Rapporto = -1 GROUP BY Risorse_Umane.Piva, Risorse_Umane.Cod_Rapporto) ")
            Stb.AppendLine("  LEFT JOIN Contatti ON Imprese.Piva = Contatti.Piva AND Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto ")
            Stb.AppendLine("  LEFT JOIN ContattixIndirizzi ON Contatti.Piva = ContattixIndirizzi.Piva AND Contatti.Cod_Contatto = ContattixIndirizzi.Cod_Contatto And ContattixIndirizzi.Tipo_Indirizzo = 3 ")
            Stb.AppendLine("  LEFT JOIN Indirizzi Indirizzo_Legale ON ContattixIndirizzi.Cod_Indirizzo = Indirizzo_Legale.cod_indirizzo ")
            Stb.AppendLine("  LEFT JOIN ISTAT ISTAT_Legale ON Indirizzo_Legale.pro_cod_istat = ISTAT_Legale.PROV AND Indirizzo_Legale.com_cod_istat = ISTAT_Legale.COM ")

            If Not Sintetico Then

                Stb.AppendLine("  LEFT JOIN Programmazione_EntitaXProgrammazione_Entita ON Programmazione_Entita.Programmazione_Entita_Cod = Programmazione_EntitaXProgrammazione_Entita.Programmazione_Entita_Cod_To AND Programmazione_EntitaXProgrammazione_Entita.TipoRelazione = 1 ")
                Stb.AppendLine("  LEFT JOIN Programmazione_Entita Prenotazione ON Prenotazione.Programmazione_Entita_Cod = Programmazione_EntitaXProgrammazione_Entita.Programmazione_Entita_Cod_From ")
                Stb.AppendLine("  LEFT JOIN Programmazione_Testata PrenotazioneT ON Prenotazione.Programmazione_Cod = PrenotazioneT.Programmazione_Cod ")
                Stb.AppendLine("  LEFT JOIN Imprese Vivaio ON Prenotazione.Veg_Cod_Cliente = vivaio.PIVA ")

            End If

            Stb.AppendLine("  LEFT JOIN Programmazione_Entita_Codici KPIN ON Programmazione_Entita.Programmazione_Entita_Cod = KPIN.Programmazione_Entita_Cod AND KPIN.ID_Cod = " & enum_CodiciAnagrafe.Zespri_Codice_kPIN & " ")
            Stb.AppendLine("  LEFT JOIN Programmazione_Entita_Codici BlockName ON Programmazione_Entita.Programmazione_Entita_Cod = BlockName.Programmazione_Entita_Cod AND BlockName.ID_Cod = " & enum_CodiciAnagrafe.Zespri_Block_Name & " ")

            If Not Sintetico Then

                Stb.AppendLine("  LEFT JOIN SpecieVegetali SpecieVegetaliP ON Prenotazione.veg_Cod = SpecieVegetaliP.Veg_Cod  ")
                Stb.AppendLine("  LEFT JOIN Cultivar CultivarP ON Prenotazione.Cul_Cod = CultivarP.cul_cod ")

            End If
            Stb.AppendLine("  LEFT JOIN Programmazione_Entita_Codici Zespri_phase ON Programmazione_Entita.Programmazione_Entita_Cod = Zespri_phase.Programmazione_Entita_Cod AND Zespri_phase.ID_Cod = 1330 ")
            Stb.AppendLine("  LEFT JOIN OTabelle_Parametri Zespri_phaseT ON Zespri_phaseT.Tabella_Par_Cod = Zespri_phase.Val_Cod AND Zespri_phaseT.Tabella_Cod = 1330 ")

            Stb.AppendLine("  LEFT JOIN Programmazione_Entita_Codici Zespri_type ON Programmazione_Entita.Programmazione_Entita_Cod = Zespri_type.Programmazione_Entita_Cod AND Zespri_type.ID_Cod = 1331 ")
            Stb.AppendLine("  LEFT JOIN OTabelle_Parametri Zespri_typeT ON Zespri_typeT.Tabella_Par_Cod = Zespri_type.Val_Cod AND Zespri_typeT.Tabella_Cod = 1331 ")

            Stb.AppendLine("  LEFT JOIN Programmazione_Entita_Codici Zespri_Grower ON Programmazione_Entita.Programmazione_Entita_Cod = Zespri_Grower.Programmazione_Entita_Cod AND Zespri_Grower.ID_Cod = 1332 ")
            Stb.AppendLine("  LEFT JOIN OTabelle_Parametri Zespri_GrowerT ON Zespri_GrowerT.Tabella_Par_Cod = Zespri_Grower.Val_Cod AND Zespri_GrowerT.Tabella_Cod = 1332 ")

            Stb.AppendLine("  LEFT JOIN Programmazione_Entita_Codici TipologiaDiInnesto ON Programmazione_Entita.Programmazione_Entita_Cod = TipologiaDiInnesto.Programmazione_Entita_Cod AND TipologiaDiInnesto.ID_Cod = 1336 ")
            Stb.AppendLine("  LEFT JOIN Portinnesti ON Programmazione_Entita.Port_Cod = Portinnesti.Port_Cod ")

            If Filtro_Visibilita_Utente Then
                Stb.AppendLine("   LEFT JOIN Utenti_Visibilita_Appoggio (NOLOCK) On Imprese.Piva = Utenti_Visibilita_Appoggio.Piva AND Utenti_Visibilita_Appoggio.Entita_Cod=1  ")
            End If


            Stb.AppendLine(" WHERE Programmazione_Testata.Tipo_Pianificazione = 13 ")

            If Filtro_Visibilita_Utente Then
                Stb.AppendLine(" AND Utenti_Visibilita_Appoggio.Username = " & Agro_SQL_SaveText_NULL(objParametri.UtenteUsername) & "  ")
            End If

            If Piva <> "" Then
                Stb.AppendLine(" AND Programmazione_Entita.Piva = " & Agro_SQL_SaveText_NULL(Piva) & " ")
            End If

            If Data <> AGRODATAINIZIO Then
                Stb.AppendLine(" AND Programmazione_Entita.Validita_Inizio < " & Agro_SQL_SaveDate(Data) & " ")
                Stb.AppendLine(" AND Programmazione_Entita.Validita_Fine > " & Agro_SQL_SaveDate(Data) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else

            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function

    Public Function LeggiPrenotazionePianteBudget(Id_Budget As Integer,
                                                  Piva As String,
                                                  Sa_Cod As Integer,
                                                  Appezza As Integer,
                                                  Id_Reg As Integer,
                                                  xFiltroAggiuntivo As String,
                                                  xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                  ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_R.LeggiPrenotazionePianteBudget()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            '------------------------------------------------------------------
            strSql.Length = 0
            strSql.AppendLine(" SELECT * ")
            strSql.AppendLine(" FROM  Programmazione_Entita")

            strSql.AppendLine(" WHERE 1 = 1 ")

            If Id_Budget <> 0 Then
                strSql.AppendLine(" AND Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & "   ")
            End If

            If Piva <> "" Then
                strSql.AppendLine(" AND Budget_Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND Budget_Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Appezza <> 0 Then
                strSql.AppendLine(" AND Budget_Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
            End If

            If Id_Reg <> 0 Then
                strSql.AppendLine(" AND Budget_Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & "   ")
            End If


            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Inviato >= 0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Inviato = -1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Id_Budget ASC ")
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
End Class



'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################

Public Class Programmazione_Entita_W
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Scrivi(ByVal Programmazione_Cod As Integer,
                           ByRef Programmazione_Entita_Cod As Integer,
                           ByVal Entita_Des As String,
                           ByVal Piva As String,
                           ByVal Sa_Cod As Integer,
                           ByVal Campo_Cod As Integer,
                           ByVal Appezza As Integer,
                           ByVal Id_Reg As Integer,
                           ByVal Progetto_Cod As Integer,
                           ByVal Progetto_Des As String,
                           ByVal Id_Cod As Integer,
                           ByVal Veg_Cod As Integer,
                           ByVal Cul_Cod As Integer,
                           ByVal Grva_Cod As Integer,
                           ByVal Grfi_Cod As Integer,
                           ByVal Cop_Cod As Integer,
                           ByVal Superficie As Decimal,
                           ByVal Resa As Decimal,
                           ByVal TipoZona As String,
                           ByVal Veg_Cod_Prec As Integer,
                           ByVal Id_Mat_O As Integer,
                           ByVal Id_Fre As Integer,
                           ByVal N_distribuito As Integer,
                           ByVal Num_Piante As Integer,
                           ByVal Tra_Fila As Decimal,
                           ByVal Su_Fila As Decimal,
                           ByVal Foral_Cod As Integer,
                           ByVal Port_Cod As Integer,
                           ByVal Imp_Cod As Integer,
                           ByVal Regolamento_Cod As Integer,
                           ByVal Disciplinare_Cod As Integer,
                           ByVal Stato_Cod As Integer,
                           ByVal Ciclo As Integer,
                           ByVal Data_Semina As Date,
                           ByVal Data_Raccolta As Date,
                           ByVal Note As String,
                           ByVal Veg_Cod_Cliente As String,
                           ByVal Cul_Cod_Cliente As String,
                           ByVal MetodoProduzione_Cod As Integer,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                               Optional ByVal Data_creazione As Date = #2/1/1900#,
                               Optional ByVal Data_modifica As Date = #2/1/1900#,
                               Optional ByVal username_creazione As String = "",
                               Optional ByVal username_modifica As String = "",
                               Optional ByVal flagIrrigabilita As Integer = 0,
                               Optional ByVal flagSecondoRaccolto As Integer = 0,
                               Optional ByVal Codice_Fiscale_Tecnico As String = "",
                               Optional ByVal Superficie_Futura As String = "",
                               Optional ByVal Operazione_Cod As Integer = 0,
                               Optional ByVal Cod_Macrouso As String = "",
                               Optional ByVal via_stringa As String = "",
                               Optional ByVal Unita_Vitata As Integer = 0,
                               Optional ByVal Validita_Inizio_Impianto As Date = #1/1/1900#,
                               Optional ByVal Conversione_Data_Inizio As Date = #1/1/1900#,
                               Optional ByVal Conversione_Data_Fine As Date = #12/31/2100#,
                               Optional ByVal Veg_Cod_Agea As String = "",
                               Optional ByVal Cul_Cod_Agea As String = "",
                               Optional ByVal Uso_Cod_Agea As String = "",
                               Optional ByVal Occupazione_Cod_Agea As String = "",
                               Optional ByVal Destinazione_Cod_Agea As String = "",
                               Optional ByVal Qualita_Cod_Agea As String = "",
                               Optional ByVal Limite_N As String = "",
                               Optional ByVal Limite_P As String = "",
                               Optional ByVal Limite_K As String = "",
                               Optional ByVal Data_Fioritura_Prevista As Date = AGRODATAINIZIO,
                               Optional ByVal Veg_Cod_Prec2 As Integer = 0,
                               Optional ByVal Veg_Cod_Prec3 As Integer = 0,
                               Optional ByVal Veg_Cod_Prec4 As Integer = 0,
                               Optional ByVal Piano_Semina As String = "",
                               Optional ByVal Codice_Contratto As String = "",
                               Optional ByVal Stato_Ribaltamento As Integer = 0,
                               Optional ByVal IAF As String = "",
                               Optional ByVal Regolamento_Concimazione_Cod As Integer = 0,
                               Optional ByVal Flag_PubblicoPrivato As Integer = 0,
                               Optional ByVal id_tr As Integer = 0,
                               Optional ByVal DistBZ_CorpiIdrici As Double = 0,
                               Optional ByVal DistBZ_AreeResPub As Double = 0,
                               Optional ByVal DistBZ_Allevamenti As Double = 0,
                               Optional ByVal DistBZ_VegNatNonColt As Double = 0,
                               Optional ByVal SupBZ_Riduzione As Double = 0,
                               Optional ByVal riferimento_alfanumerico_appezzamento As String = "",
                               Optional ByVal isola As String = "",
                               Optional ByVal CapitolatoPrivato As String = "",
                               Optional ByVal Finalita_Concimazione_Impianto As Integer = 0,
                               Optional ByVal cod_indirizzo As Integer = 0,
                               Optional ByVal cod_rubrica_email_richiedente As Integer = 0,
                               Optional ByVal id_budget As Integer = 0,
                               Optional ByVal germinabilita As Double = 0,
                               Optional ByVal descimpiantobudget As String = "",
                               Optional ByVal dataconsegna As Date = AGRODATAINIZIO,
                               Optional ByVal budget_piva As String = "",
                               Optional ByVal budget_sa_cod As Double = 0,
                               Optional ByVal budget_appezza As Integer = 0,
                               Optional ByVal budget_id_reg As Integer = 0,
                               Optional ByVal Mat_Cod As Integer = 0,
                               Optional ByVal qta_seme_omaggio As Integer = 0,
                               Optional ByVal id_plateau As Integer = 0
                           ) As Boolean



        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_W.Scrivi()"

        Dim ObjSequenze As AgronicaCoreDataProvider.Agro_Sequenze

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '----- Ricavo il codice in Sequenza_Tabelle
            If Programmazione_Entita_Cod = 0 Then

                ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

                Programmazione_Entita_Cod = ObjSequenze.NuovoId_Tabella(
                                       "Programmazione_Entita", 0, 2000000000, objParametri)

                ObjSequenze = Nothing

            End If


            If Data_creazione = #2/1/1900# Then
                Data_creazione = Date.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If

            '----- Genero la query SQL 
            StrSQL.Append(" INSERT INTO Programmazione_Entita ( ")
            StrSQL.Append("             Piva_SuperUser, Programmazione_Entita_Cod,  Programmazione_Cod, ")
            StrSQL.Append("             Entita_Des,     Piva,                       Sa_Cod, ")
            StrSQL.Append("             Campo_Cod,      Appezza,                    Id_Reg,  ")
            StrSQL.Append("             Progetto_Cod,   Progetto_Des,               Id_Cod, ")
            StrSQL.Append("             Veg_Cod,        Cul_Cod,                    Grfi_Cod, ")
            StrSQL.Append("             Cop_Cod,        Superficie,                 Resa, ")
            StrSQL.Append("             TipoZona,       Veg_Cod_Prec,               Id_Mat_O, ")
            StrSQL.Append("             Id_Fre,         N_distribuito,                       ")
            StrSQL.Append("             Num_Piante,     Tra_Fila,                   Su_Fila, ")
            StrSQL.Append("             Foral_Cod,      Port_Cod,                   Imp_Cod, ")
            StrSQL.Append("             Regolamento_Cod,Disciplinare_Cod,                       ")
            StrSQL.Append("             Grva_Cod,       Stato_Cod,                  Ciclo, ")
            StrSQL.Append("             Data_Semina,    Data_Raccolta,              Note, ")
            StrSQL.Append("             Veg_Cod_Cliente, Cul_Cod_Cliente,           MetodoProduzione_Cod,  ")
            StrSQL.Append("             flagIrrigabilita, flagSecondoRaccolto, Codice_Fiscale_Tecnico, ")
            StrSQL.Append("             Macrouso_Cod, ")
            StrSQL.Append("             via_stringa, unita_vitata, Validita_Inizio_Impianto, ")


            StrSQL.Append("             Inviato,                ")
            StrSQL.Append("             Data_Creazione,         Data_Modifica, ")
            StrSQL.Append("             UserName_Creazione,     UserName_Modifica, ")
            StrSQL.Append("             Validita_Inizio,        Validita_Fine ")

            If Superficie_Futura <> "" Then
                StrSQL.Append("         , Superficie_Futura ")
            End If

            StrSQL.Append("         , Operazione_Cod, Conversione_Data_Inizio, Conversione_Data_Fine  ")
            StrSQL.Append("         , Veg_Cod_Agea, Cul_Cod_Agea, Uso_Cod_Agea, Occupazione_Cod_Agea, Destinazione_Cod_Agea, Qualita_cod_Agea  ")

            StrSQL.Append("         , Limite_N, Limite_P, Limite_K")
            StrSQL.Append("         , Data_Fioritura_Prevista, Veg_Cod_Prec2, Veg_Cod_Prec3, Veg_Cod_Prec4")
            StrSQL.Append("         , Piano_Semina, Codice_Contratto, Stato_Ribaltamento, IAF ")
            StrSQL.Append("         , Regolamento_Concimazione_Cod, Flag_PubblicoPrivato, Id_tr ")
            StrSQL.Append("         , DistBZ_CorpiIdrici, DistBZ_AreeResPub, DistBZ_Allevamenti, DistBZ_VegNatNonColt ")
            StrSQL.Append("         , SupBZ_Riduzione ")
            StrSQL.Append("         , riferimento_alfanumerico_appezzamento, isola ")
            StrSQL.Append("         , CapitolatoPrivato ") 'Finalita_Concimazione_Impianto
            StrSQL.Append("         , Finalita_Concimazione_Impianto ")
            StrSQL.Append("         , Cod_Indirizzo ")
            StrSQL.Append("         , unar, id_budget, germinabilita, desc_impianto_budget, Data_Consegna")
            StrSQL.Append("         , budget_piva, budget_sa_cod, budget_appezza, budget_id_reg, Mat_Cod, qta_seme_omaggio, id_plateau")
            StrSQL.Append(" ) ")

            StrSQL.Append(" VALUES ( ")
            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser.ToString) & "' ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Programmazione_Entita_Cod.ToString) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Programmazione_Cod.ToString) & " ")
            StrSQL.Append("        ,'" & Agro_SQL_SaveText(Entita_Des.ToString) & "' ")
            StrSQL.Append("        ,'" & Agro_SQL_SaveText(Piva.ToString) & "' ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Sa_Cod.ToString) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Campo_Cod.ToString) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Appezza.ToString) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Id_Reg.ToString) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Progetto_Cod.ToString) & " ")
            StrSQL.Append("        ,'" & Agro_SQL_SaveText(Progetto_Des.ToString) & "' ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Id_Cod.ToString) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Veg_Cod.ToString) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Cul_Cod.ToString) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Grfi_Cod.ToString) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Cop_Cod.ToString) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Superficie.ToString) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Resa.ToString) & " ")
            StrSQL.Append("        ,'" & Agro_SQL_SaveText(TipoZona.ToString) & "' ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Veg_Cod_Prec.ToString) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Id_Mat_O.ToString) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Id_Fre.ToString) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(N_distribuito.ToString) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Num_Piante.ToString) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Tra_Fila.ToString) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Su_Fila.ToString) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Foral_Cod.ToString) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Port_Cod.ToString) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Imp_Cod.ToString) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Regolamento_Cod.ToString) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Disciplinare_Cod.ToString) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Grva_Cod.ToString) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Stato_Cod.ToString) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Ciclo.ToString) & " ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_Semina) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_Raccolta) & "  ")
            StrSQL.Append("        ,'" & Agro_SQL_SaveText(Note.ToString) & "' ")
            StrSQL.Append("        ,'" & Agro_SQL_SaveText(Veg_Cod_Cliente.ToString) & "' ")
            StrSQL.Append("        ,'" & Agro_SQL_SaveText(Cul_Cod_Cliente.ToString) & "' ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(MetodoProduzione_Cod) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(flagIrrigabilita) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(flagSecondoRaccolto) & " ")
            If Codice_Fiscale_Tecnico <> "" Then
                StrSQL.Append("         ,'" & Agro_SQL_SaveText(Codice_Fiscale_Tecnico) & "' ")
            Else
                StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            End If

            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Cod_Macrouso) & "' ")

            StrSQL.Append("         ,'" & Agro_SQL_SaveText(via_stringa) & "' ")

            StrSQL.Append("         ," & Agro_SQL_SaveNum(Unita_Vitata) & " ")

            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio_Impianto) & "  ")

            StrSQL.Append("         , 0  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")



            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            If Superficie_Futura <> "" Then
                StrSQL.Append("         , " & Agro_SQL_SaveNum(Superficie_Futura) & "  ")
            End If

            StrSQL.Append("         ," & Agro_SQL_SaveNum(Operazione_Cod) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveDate(Conversione_Data_Inizio) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveDate(Conversione_Data_Fine) & " ")

            StrSQL.Append("			,'" & Agro_SQL_SaveText(Veg_Cod_Agea) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(Cul_Cod_Agea) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(Uso_Cod_Agea) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(Occupazione_Cod_Agea) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(Destinazione_Cod_Agea) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(Qualita_Cod_Agea) & "' ")

            StrSQL.Append("			,'" & Agro_SQL_SaveText(Limite_N) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(Limite_P) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(Limite_K) & "' ")
            StrSQL.Append("			," & Agro_SQL_SaveDate(Data_Fioritura_Prevista) & " ")
            StrSQL.Append("			," & Agro_SQL_SaveNum(Veg_Cod_Prec2) & " ")
            StrSQL.Append("			," & Agro_SQL_SaveNum(Veg_Cod_Prec3) & " ")
            StrSQL.Append("			," & Agro_SQL_SaveNum(Veg_Cod_Prec4) & " ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(Piano_Semina) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(Codice_Contratto) & "' ")
            StrSQL.Append("			," & Agro_SQL_SaveNum(Stato_Ribaltamento) & " ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(IAF) & "' ")
            StrSQL.Append("			," & Agro_SQL_SaveNum(Regolamento_Concimazione_Cod) & " ")
            StrSQL.Append("			," & Agro_SQL_SaveNum(Flag_PubblicoPrivato) & " ")
            StrSQL.Append("			," & Agro_SQL_SaveNum(id_tr) & " ")

            StrSQL.Append("			," & Agro_SQL_SaveNum(DistBZ_CorpiIdrici) & " ")
            StrSQL.Append("			," & Agro_SQL_SaveNum(DistBZ_AreeResPub) & " ")
            StrSQL.Append("			," & Agro_SQL_SaveNum(DistBZ_Allevamenti) & " ")
            StrSQL.Append("			," & Agro_SQL_SaveNum(DistBZ_VegNatNonColt) & " ")
            StrSQL.Append("			," & Agro_SQL_SaveNum(SupBZ_Riduzione) & " ")

            StrSQL.Append("			,'" & Agro_SQL_SaveText(riferimento_alfanumerico_appezzamento) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(isola) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(CapitolatoPrivato) & "' ") 'Finalita_Concimazione_Impianto
            StrSQL.Append("			," & Agro_SQL_SaveNum(Finalita_Concimazione_Impianto) & " ")
            StrSQL.Append("			," & Agro_SQL_SaveNum(cod_indirizzo) & " ")
            StrSQL.Append("			," & Agro_SQL_SaveNum(cod_rubrica_email_richiedente) & " ")
            StrSQL.Append("			," & Agro_SQL_SaveNum(id_budget) & " ")
            StrSQL.Append("			," & Agro_SQL_SaveNum(germinabilita) & " ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(descimpiantobudget) & "' ")
            StrSQL.Append("           ," & Agro_SQL_SaveDate(dataconsegna) & " ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(budget_piva) & "' ")
            StrSQL.Append("			," & Agro_SQL_SaveNum(budget_sa_cod) & " ")
            StrSQL.Append("			," & Agro_SQL_SaveNum(budget_appezza) & " ")
            StrSQL.Append("			," & Agro_SQL_SaveNum(budget_id_reg) & " ")
            StrSQL.Append("			," & Agro_SQL_SaveNum(Mat_Cod) & " ")
            StrSQL.Append("			," & Agro_SQL_SaveNum(qta_seme_omaggio) & " ")
            StrSQL.Append("			," & Agro_SQL_SaveNum(id_plateau) & " ")
            StrSQL.Append(" ) ")

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

    Public Function reset_x_prenotazione_piante(
                            ByVal Programmazione_Cod As Integer,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Programmazione_Cod=0        => Vengono cancellate tutte le entità del Piva_SuperUser
        '   Programmazione_Entita_Cod=0 => Vengono cancellate tutte le entità di una programmazione
        '
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" UPDATE Programmazione_Entita ")
            StrSQL.Append(" SET ")
            StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("      ,cop_cod = 0  ")

            StrSQL.Append(" WHERE  Inviato >= 0")

            If objParametri.PivaSuperUser <> "" Then
                StrSQL.Append("  AND   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            End If

            StrSQL.Append("  AND   Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod) & " ")

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

    Public Function Aggiorna_Stato_Ribaltamento(ByVal Programmazione_Cod As Integer,
                                                ByRef Programmazione_Entita_Cod As Integer,
                                                ByVal Piva As String,
                                                ByVal Sa_Cod As Integer,
                                                ByVal Campo_Cod As Integer,
                                                ByVal Appezza As Integer,
                                                ByVal Id_Reg As Integer,
                                                ByVal stato_ribaltamento As enum_Programmazione_Entita_Stato_Ribaltamento,
                                                ByRef objParametri As AgronicaCoreParametri)

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_W.Aggiorna_Stato_Ribaltamento()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New StringBuilder
        Dim xRisp As Boolean = False

        Try


            Stb.AppendLine(" UPDATE Programmazione_Entita ")
            Stb.AppendLine(" SET Stato_Ribaltamento = " & Agro_SQL_SaveNum(stato_ribaltamento) & " ")
            Stb.AppendLine(" WHERE 1 = 1 ")
            If Programmazione_Cod <> 0 Then
                Stb.AppendLine(" And Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod) & "  ")
            End If
            If Programmazione_Entita_Cod <> 0 Then
                Stb.AppendLine(" And Programmazione_Entita_Cod = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & " ")
            End If
            If Piva <> "" Then
                Stb.AppendLine(" And Piva = " & Agro_SQL_SaveText_NULL(Piva) & " ")
            End If
            If Sa_Cod <> 0 Then
                Stb.AppendLine(" And Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If
            If Campo_Cod <> 0 Then
                Stb.AppendLine(" And Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & " ")
            End If
            If Appezza <> 0 Then
                Stb.AppendLine(" And Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If
            If Id_Reg <> 0 Then
                Stb.AppendLine(" And Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & "")
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function Cancella(
                            ByVal Programmazione_Cod As Integer,
                            ByVal Programmazione_Entita_Cod As Integer,
                                ByVal Cancella_Singola_Entita As Boolean,
                                ByVal Cancella_Campo As Boolean,
                                ByVal Campo_Cod As Integer,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Programmazione_Cod=0        => Vengono cancellate tutte le entità del Piva_SuperUser
        '   Programmazione_Entita_Cod=0 => Vengono cancellate tutte le entità di una programmazione
        '
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Programmazione_Entita ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     Programmazione_Entita ")
                StrSQL.Append(" WHERE   1=1 ")

            End If

            If objParametri.PivaSuperUser <> "" Then
                StrSQL.Append("  AND   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            End If

            If Cancella_Singola_Entita Then

                If Programmazione_Entita_Cod <> 0 Then
                    StrSQL.Append("  AND   Programmazione_Entita_Cod = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & " ")
                End If

                If Programmazione_Cod <> 0 Then
                    StrSQL.Append("  AND   Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod) & " ")
                End If

            Else

                StrSQL.Append("  AND   Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod) & " ")

                If Cancella_Campo Then

                    StrSQL.Append(" AND Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod.ToString) & " ")

                End If

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


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' AgronicaCoreAnagrafeDAL.Programmazione_Entita_W.Modifica
    ''' </summary>
    ''' <param name="Programmazione_Cod">OBBLIGATORIO</param>
    ''' <param name="Programmazione_Entita_Cod">OBBLIGATORIO</param>
    ''' <param name="Entita_Des">StrDefault_per_MODIFICA</param>
    ''' <param name="Piva">StrDefault_per_MODIFICA</param>
    ''' <param name="Sa_Cod">IntDefault_per_MODIFICA</param>
    ''' <param name="Campo_Cod">IntDefault_per_MODIFICA</param>
    ''' <param name="Appezza">IntDefault_per_MODIFICA</param>
    ''' <param name="Id_Reg">IntDefault_per_MODIFICA</param>
    ''' <param name="Progetto_Cod">IntDefault_per_MODIFICA</param>
    ''' <param name="Progetto_Des">StrDefault_per_MODIFICA</param>
    ''' <param name="Id_Cod">IntDefault_per_MODIFICA</param>
    ''' <param name="Veg_Cod">IntDefault_per_MODIFICA</param>
    ''' <param name="Cul_Cod">IntDefault_per_MODIFICA</param>
    ''' <param name="Grva_Cod">IntDefault_per_MODIFICA</param>
    ''' <param name="Grfi_Cod">IntDefault_per_MODIFICA</param>
    ''' <param name="Cop_Cod">IntDefault_per_MODIFICA</param>
    ''' <param name="Superficie">DoubleDefault_per_MODIFICA</param>
    ''' <param name="Resa">DoubleDefault_per_MODIFICA</param>
    ''' <param name="TipoZona">StrDefault_per_MODIFICA</param>
    ''' <param name="Veg_Cod_Prec">IntDefault_per_MODIFICA</param>
    ''' <param name="Id_Mat_O">IntDefault_per_MODIFICA</param>
    ''' <param name="Id_Fre">IntDefault_per_MODIFICA</param>
    ''' <param name="N_distribuito">IntDefault_per_MODIFICA</param>
    ''' <param name="Num_Piante">IntDefault_per_MODIFICA</param>
    ''' <param name="Tra_Fila">DoubleDefault_per_MODIFICA</param>
    ''' <param name="Su_Fila">DoubleDefault_per_MODIFICA</param>
    ''' <param name="Foral_Cod">IntDefault_per_MODIFICA</param>
    ''' <param name="Port_Cod">IntDefault_per_MODIFICA</param>
    ''' <param name="Imp_Cod">IntDefault_per_MODIFICA</param>
    ''' <param name="Regolamento_Cod">IntDefault_per_MODIFICA</param>
    ''' <param name="Disciplinare_Cod">IntDefault_per_MODIFICA</param>
    ''' <param name="Stato_Cod">IntDefault_per_MODIFICA</param>
    ''' <param name="Ciclo">IntDefault_per_MODIFICA</param>
    ''' <param name="Data_Semina">DataDefault_per_MODIFICA</param>
    ''' <param name="Data_Raccolta">DataDefault_per_MODIFICA</param>
    ''' <param name="Note">StrDefault_per_MODIFICA</param>
    ''' <param name="Veg_Cod_Cliente">StrDefault_per_MODIFICA</param>
    ''' <param name="Cul_Cod_Cliente">StrDefault_per_MODIFICA</param>
    ''' <param name="via_stringa">StrDefault_per_MODIFICA</param>
    ''' <param name="Unita_Vitata">IntDefault_per_MODIFICA</param>
    ''' <param name="Validita_Inizio">DataDefault_per_MODIFICA</param>
    ''' <param name="Validita_Fine">DataDefault_per_MODIFICA</param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="objParametri"></param>
    ''' -----------------------------------------------------------------------------
    Public Function Modifica(ByVal Programmazione_Cod As Integer,
                             ByRef Programmazione_Entita_Cod As Integer,
                             ByVal Entita_Des As String,
                             ByVal Piva As String,
                             ByVal Sa_Cod As Integer,
                             ByVal Campo_Cod As Integer,
                             ByVal Appezza As Integer,
                             ByVal Id_Reg As Integer,
                             ByVal Progetto_Cod As Integer,
                             ByVal Progetto_Des As String,
                             ByVal Id_Cod As Integer,
                             ByVal Veg_Cod As Integer,
                             ByVal Cul_Cod As Integer,
                             ByVal Grva_Cod As Integer,
                             ByVal Grfi_Cod As Integer,
                             ByVal Cop_Cod As Integer,
                             ByVal Superficie As Decimal,
                             ByVal Resa As Decimal,
                             ByVal TipoZona As String,
                             ByVal Veg_Cod_Prec As Integer,
                             ByVal Id_Mat_O As Integer,
                             ByVal Id_Fre As Integer,
                             ByVal N_distribuito As Integer,
                             ByVal Num_Piante As Integer,
                             ByVal Tra_Fila As Decimal,
                             ByVal Su_Fila As Decimal,
                             ByVal Foral_Cod As Integer,
                             ByVal Port_Cod As Integer,
                             ByVal Imp_Cod As Integer,
                             ByVal Regolamento_Cod As Integer,
                             ByVal Disciplinare_Cod As Integer,
                             ByVal Stato_Cod As Integer,
                             ByVal Ciclo As Integer,
                             ByVal Data_Semina As Date,
                             ByVal Data_Raccolta As Date,
                             ByVal Note As String,
                             ByVal Veg_Cod_Cliente As String,
                             ByVal Cul_Cod_Cliente As String,
                             ByVal via_stringa As String,
                             ByVal Unita_Vitata As Integer,
                             ByVal Validita_Inizio_Impianto As Date,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                 Optional ByVal Data_Fioritura_Prevista As Date = AGRODATAINIZIO,
                                 Optional ByVal Conversione_Data_Inizio As Date = AGRODATAINIZIO,
                                 Optional ByVal Veg_Cod_Prec3 As String = Nothing,
                                 Optional ByVal Veg_Cod_Prec4 As String = Nothing,
                                 Optional ByVal Stato_Ribaltamento As Integer? = Nothing,
                                 Optional ByVal Regolamento_Concimazione_Cod As Integer? = Nothing,
                                 Optional ByVal isola As String = Nothing,
                                 Optional ByVal CapitolatoPrivato As String = Nothing,
                                 Optional ByVal Codice_Contratto As String = Nothing,
                                 Optional ByVal cod_rubrica_email_richiedente As String = Nothing,
                               Optional ByVal Data_creazione As Date = #2/1/1900#,
                               Optional ByVal Data_modifica As Date = #2/1/1900#,
                               Optional ByVal username_creazione As String = "",
                               Optional ByVal username_modifica As String = "",
                               Optional ByVal flagIrrigabilita As Integer = 0,
                               Optional ByVal flagSecondoRaccolto As Integer = 0,
                               Optional ByVal Codice_Fiscale_Tecnico As String = "",
                               Optional ByVal Superficie_Futura As Integer = 0,
                               Optional ByVal Operazione_Cod As Integer = 0,
                               Optional ByVal Cod_Macrouso As String = "",
                               Optional ByVal Conversione_Data_Fine As Date = #12/31/2100#,
                               Optional ByVal Veg_Cod_Agea As String = "",
                               Optional ByVal Cul_Cod_Agea As String = "",
                               Optional ByVal Uso_Cod_Agea As String = "",
                               Optional ByVal Occupazione_Cod_Agea As String = "",
                               Optional ByVal Destinazione_Cod_Agea As String = "",
                               Optional ByVal Qualita_Cod_Agea As String = "",
                               Optional ByVal Limite_N As String = "",
                               Optional ByVal Limite_P As String = "",
                               Optional ByVal Limite_K As String = "",
                               Optional ByVal Piano_Semina As String = "",
                               Optional ByVal IAF As String = "",
                               Optional ByVal Flag_PubblicoPrivato As Integer = 0,
                               Optional ByVal id_tr As Integer = 0,
                               Optional ByVal DistBZ_CorpiIdrici As Double = 0,
                               Optional ByVal DistBZ_AreeResPub As Double = 0,
                               Optional ByVal DistBZ_Allevamenti As Double = 0,
                               Optional ByVal DistBZ_VegNatNonColt As Double = 0,
                               Optional ByVal SupBZ_Riduzione As Double = 0,
                               Optional ByVal riferimento_alfanumerico_appezzamento As String = "",
                               Optional ByVal Finalita_Concimazione_Impianto As Integer = 0,
                               Optional ByVal cod_indirizzo As Integer = 0,
                               Optional ByVal id_budget As Integer = 0,
                               Optional ByVal germinabilita As Double = 0,
                               Optional MetodoProduzione_Cod As Integer = 0,
                               Optional descImpiantoBudget As String = "",
                               Optional ByVal dataConsegna As Date = AGRODATAINIZIO,
                               Optional ByVal budget_piva As String = "",
                               Optional ByVal budget_sa_cod As Double = 0,
                               Optional ByVal budget_appezza As Integer = 0,
                               Optional ByVal budget_id_reg As Integer = 0,
                               Optional ByVal qta_seme_evaso As Integer = -999,
                               Optional ByVal qta_seme_omaggio As Integer = -999,
                               Optional ByVal note_integrative As String = "",
                               Optional ByVal id_plateau As Integer = -999
                             ) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_W.Modifica()"

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

            If Programmazione_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Programmazione_Cod obbligatorio)")
            End If

            If Programmazione_Entita_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Programmazione_Des obbligatorio)")
            End If


            '---------------------------------------------
            StrSQL.Append(" UPDATE Programmazione_Entita SET  ")
            If Entita_Des <> StrDefault_per_MODIFICA Then
                StrSQL.AppendLine("    Entita_Des     = '" & Agro_SQL_SaveText(Entita_Des) & "'  ,")
            End If
            If Piva <> StrDefault_per_MODIFICA Then
                StrSQL.AppendLine("    Piva     = '" & Agro_SQL_SaveText(Piva) & "'  ,")
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
                StrSQL.AppendLine("Data_Modifica = " & Agro_SQL_SaveDateTime(Data_modifica) & "  ,")
            End If


            If Sa_Cod <> IntDefault_per_MODIFICA Then
                StrSQL.AppendLine("    Sa_Cod     = " & Agro_SQL_SaveNum(Sa_Cod) & "  ,")
            End If
            If Campo_Cod <> IntDefault_per_MODIFICA Then
                StrSQL.AppendLine("    Campo_Cod     = " & Agro_SQL_SaveNum(Campo_Cod) & "  ,")
            End If
            If Appezza <> IntDefault_per_MODIFICA Then
                StrSQL.AppendLine("    Appezza     = " & Agro_SQL_SaveNum(Appezza) & "  ,")
            End If
            If Id_Reg <> IntDefault_per_MODIFICA Then
                StrSQL.AppendLine("    Id_Reg     = " & Agro_SQL_SaveNum(Id_Reg) & "  ,")
            End If
            If Progetto_Cod <> IntDefault_per_MODIFICA Then
                StrSQL.AppendLine("    Progetto_Cod     = " & Agro_SQL_SaveNum(Progetto_Cod) & "  ,")
            End If

            If Progetto_Des <> StrDefault_per_MODIFICA Then
                StrSQL.AppendLine("    Progetto_Des     = '" & Agro_SQL_SaveText(Progetto_Des) & "'  ,")
            End If

            If Id_Cod <> IntDefault_per_MODIFICA Then
                StrSQL.AppendLine("    Id_Cod     = " & Agro_SQL_SaveNum(Id_Cod) & "  ,")
            End If
            If Veg_Cod <> IntDefault_per_MODIFICA Then
                StrSQL.AppendLine("    Veg_Cod     = " & Agro_SQL_SaveNum(Veg_Cod) & "  ,")
            End If
            If Cul_Cod <> IntDefault_per_MODIFICA Then
                StrSQL.AppendLine("    Cul_Cod     = " & Agro_SQL_SaveNum(Cul_Cod) & "  ,")
            End If
            If Grva_Cod <> IntDefault_per_MODIFICA Then
                StrSQL.AppendLine("    Grva_Cod     = " & Agro_SQL_SaveNum(Grva_Cod) & "  ,")
            End If
            If Grfi_Cod <> IntDefault_per_MODIFICA Then
                StrSQL.AppendLine("    Grfi_Cod     = " & Agro_SQL_SaveNum(Grfi_Cod) & "  ,")
            End If
            If Cop_Cod <> IntDefault_per_MODIFICA Then
                StrSQL.AppendLine("    Cop_Cod     = " & Agro_SQL_SaveNum(Cop_Cod) & "  ,")
            End If

            If Superficie <> DoubleDefault_per_MODIFICA Then
                StrSQL.AppendLine("    Superficie     = " & Agro_SQL_SaveNum(Superficie) & "  ,")
            End If
            If Resa <> DoubleDefault_per_MODIFICA Then
                StrSQL.AppendLine("    Resa     = " & Agro_SQL_SaveNum(Resa) & "  ,")
            End If

            If TipoZona <> StrDefault_per_MODIFICA Then
                StrSQL.AppendLine("    TipoZona     = '" & Agro_SQL_SaveText(TipoZona) & "'  ,")
            End If

            If Veg_Cod_Prec <> IntDefault_per_MODIFICA Then
                StrSQL.AppendLine("    Veg_Cod_Prec     = " & Agro_SQL_SaveNum(Veg_Cod_Prec) & "  ,")
            End If
            If Id_Mat_O <> IntDefault_per_MODIFICA Then
                StrSQL.AppendLine("    Id_Mat_O     = " & Agro_SQL_SaveNum(Id_Mat_O) & "  ,")
            End If
            If Id_Fre <> IntDefault_per_MODIFICA Then
                StrSQL.AppendLine("    Id_Fre     = " & Agro_SQL_SaveNum(Id_Fre) & "  ,")
            End If
            If N_distribuito <> IntDefault_per_MODIFICA Then
                StrSQL.AppendLine("    N_distribuito     = " & Agro_SQL_SaveNum(N_distribuito) & "  ,")
            End If
            If Num_Piante <> IntDefault_per_MODIFICA Then
                StrSQL.AppendLine("    Num_Piante     = " & Agro_SQL_SaveNum(Num_Piante) & "  ,")
            End If
            If Tra_Fila <> DoubleDefault_per_MODIFICA Then
                StrSQL.AppendLine("    Tra_Fila     = " & Agro_SQL_SaveNum(Tra_Fila) & "  ,")
            End If
            If Su_Fila <> DoubleDefault_per_MODIFICA Then
                StrSQL.AppendLine("    Su_Fila     = " & Agro_SQL_SaveNum(Su_Fila) & "  ,")
            End If

            If Foral_Cod <> IntDefault_per_MODIFICA Then
                StrSQL.AppendLine("    Foral_Cod     = " & Agro_SQL_SaveNum(Foral_Cod) & "  ,")
            End If
            If Port_Cod <> IntDefault_per_MODIFICA Then
                StrSQL.AppendLine("    Port_Cod     = " & Agro_SQL_SaveNum(Port_Cod) & "  ,")
            End If
            If Imp_Cod <> IntDefault_per_MODIFICA Then
                StrSQL.AppendLine("    Imp_Cod     = " & Agro_SQL_SaveNum(Imp_Cod) & "  ,")
            End If
            If Regolamento_Cod <> IntDefault_per_MODIFICA Then
                StrSQL.AppendLine("    Regolamento_Cod     = " & Agro_SQL_SaveNum(Regolamento_Cod) & "  ,")
            End If
            If Disciplinare_Cod <> IntDefault_per_MODIFICA Then
                StrSQL.AppendLine("    Disciplinare_Cod     = " & Agro_SQL_SaveNum(Disciplinare_Cod) & "  ,")
            End If
            If Stato_Cod <> IntDefault_per_MODIFICA Then
                StrSQL.AppendLine("    Stato_Cod     = " & Agro_SQL_SaveNum(Stato_Cod) & "  ,")
            End If
            If Ciclo <> IntDefault_per_MODIFICA Then
                StrSQL.AppendLine("    Ciclo     = " & Agro_SQL_SaveNum(Ciclo) & "  ,")
            End If

            If Data_Semina <> DataDefault_per_MODIFICA Then
                StrSQL.AppendLine("    Data_Semina     = " & Agro_SQL_SaveDate(Data_Semina) & "  ,")
            End If
            If Data_Raccolta <> DataDefault_per_MODIFICA Then
                StrSQL.AppendLine("    Data_Raccolta     = " & Agro_SQL_SaveDate(Data_Raccolta) & "  ,")
            End If

            If Note <> StrDefault_per_MODIFICA Then
                StrSQL.AppendLine("    Note     = '" & Agro_SQL_SaveText(Note) & "'  ,")
            End If
            If Veg_Cod_Cliente <> StrDefault_per_MODIFICA Then
                StrSQL.AppendLine("    Veg_Cod_Cliente     = '" & Agro_SQL_SaveText(Veg_Cod_Cliente) & "'  ,")
            End If
            If Cul_Cod_Cliente <> StrDefault_per_MODIFICA Then
                StrSQL.AppendLine("    Cul_Cod_Cliente     = '" & Agro_SQL_SaveText(Cul_Cod_Cliente) & "'  ,")
            End If

            If via_stringa <> StrDefault_per_MODIFICA Then
                StrSQL.AppendLine("    via_stringa     = '" & Agro_SQL_SaveText(via_stringa) & "'  ,")
            End If

            If Unita_Vitata <> IntDefault_per_MODIFICA Then
                StrSQL.AppendLine("    Unita_Vitata     = " & Agro_SQL_SaveNum(Unita_Vitata) & "  ,")
            End If

            If Validita_Inizio <> DataDefault_per_MODIFICA Then
                StrSQL.AppendLine("    Validita_Inizio     = " & Agro_SQL_SaveDate(Validita_Inizio) & "  ,")
            End If
            If Validita_Fine <> DataDefault_per_MODIFICA Then
                StrSQL.AppendLine("    Validita_Fine     = " & Agro_SQL_SaveDate(Validita_Fine) & "  ,")
            End If

            If Validita_Inizio_Impianto <> DataDefault_per_MODIFICA Then
                StrSQL.AppendLine("    Validita_Inizio_Impianto     = " & Agro_SQL_SaveDate(Validita_Inizio_Impianto) & "  ,")
            End If

            If Data_Fioritura_Prevista <> AGRODATAINIZIO Then
                StrSQL.AppendLine(" data_fioritura_prevista =  " & Agro_SQL_SaveDate(Data_Fioritura_Prevista) & "  ,")
            End If

            If Conversione_Data_Inizio <> AGRODATAINIZIO Then
                StrSQL.AppendLine(" Conversione_Data_Inizio =  " & Agro_SQL_SaveDate(Conversione_Data_Inizio) & "  ,")
            End If

            If Veg_Cod_Prec3 IsNot Nothing Then
                StrSQL.AppendLine(" Veg_Cod_Prec3 = '" & Agro_SQL_SaveText(Veg_Cod_Prec3) & "'  ,")
            End If

            If Veg_Cod_Prec4 IsNot Nothing Then
                StrSQL.AppendLine(" Veg_Cod_Prec4 = '" & Agro_SQL_SaveText(Veg_Cod_Prec4) & "'  ,")
            End If

            If Stato_Ribaltamento IsNot Nothing Then
                StrSQL.AppendLine(" Stato_Ribaltamento = " & Agro_SQL_SaveNum(Stato_Ribaltamento) & "  ,")
            End If

            If Regolamento_Concimazione_Cod IsNot Nothing Then
                StrSQL.AppendLine(" Regolamento_Concimazione_Cod = " & Agro_SQL_SaveNum(Regolamento_Concimazione_Cod) & "  ,")
            End If

            If isola IsNot Nothing Then
                StrSQL.AppendLine(" isola = '" & Agro_SQL_SaveText(isola) & "'  ,")
            End If

            If CapitolatoPrivato IsNot Nothing Then
                StrSQL.AppendLine(" CapitolatoPrivato = '" & Agro_SQL_SaveText(CapitolatoPrivato) & "'  ,")
            End If

            If Codice_Contratto IsNot Nothing Then
                StrSQL.AppendLine(" Codice_Contratto = '" & Agro_SQL_SaveText(Codice_Contratto) & "'  ,")
            End If

            If cod_rubrica_email_richiedente <> "" Then
                StrSQL.AppendLine(" unar = '" & Agro_SQL_SaveText(cod_rubrica_email_richiedente) & "'  ,")
            End If



            If id_budget <> 0 Then
                StrSQL.AppendLine(" id_budget = " & Agro_SQL_SaveNum(id_budget) & "  ,")
            End If

            If Codice_Fiscale_Tecnico <> "" Then
                StrSQL.AppendLine(" Codice_Fiscale_Tecnico = '" & Agro_SQL_SaveText(Codice_Fiscale_Tecnico) & "'  ,")
            End If

            If descImpiantoBudget <> "" Then
                StrSQL.AppendLine(" Desc_impianto_budget = '" & Agro_SQL_SaveText(descImpiantoBudget) & "'  ,")
            End If

            If IsNumeric(Superficie_Futura) AndAlso Superficie_Futura <> 0 Then
                StrSQL.AppendLine(" Superficie_Futura = " & Agro_SQL_SaveNum(Superficie_Futura) & "  ,")
            End If
            If IsNumeric(Cod_Macrouso) AndAlso Cod_Macrouso <> 0 Then
                StrSQL.AppendLine(" Macrouso_Cod = " & Agro_SQL_SaveNum(Cod_Macrouso) & "  ,")
            End If

            If IsNumeric(germinabilita) AndAlso germinabilita <> 0 Then
                StrSQL.AppendLine(" germinabilita = " & Agro_SQL_SaveNum(germinabilita) & "  ,")
            End If

            If IsNumeric(MetodoProduzione_Cod) AndAlso MetodoProduzione_Cod <> 0 Then
                StrSQL.AppendLine(" MetodoProduzione_Cod = " & Agro_SQL_SaveNum(MetodoProduzione_Cod) & "  ,")
            End If

            If IsNumeric(DistBZ_VegNatNonColt) AndAlso DistBZ_VegNatNonColt <> 0 Then
                StrSQL.AppendLine(" DistBZ_VegNatNonColt = " & Agro_SQL_SaveNum(DistBZ_VegNatNonColt) & "  ,")
            End If

            If IsNumeric(SupBZ_Riduzione) AndAlso SupBZ_Riduzione <> 0 Then
                StrSQL.AppendLine(" SupBZ_Riduzione = " & Agro_SQL_SaveNum(SupBZ_Riduzione) & "  ,")
            End If

            If dataConsegna <> AGRODATAINIZIO Then
                StrSQL.AppendLine(" Data_Consegna =  " & Agro_SQL_SaveDate(dataConsegna) & "  ,")
            End If

            If Not String.IsNullOrEmpty(budget_piva) Then
                StrSQL.AppendLine(" Budget_piva = '" & Agro_SQL_SaveText(budget_piva) & "'  ,")
            End If

            If IsNumeric(budget_sa_cod) AndAlso budget_sa_cod <> 0 Then
                StrSQL.AppendLine(" budget_sa_cod = " & Agro_SQL_SaveNum(budget_sa_cod) & "  ,")
            End If

            If IsNumeric(budget_appezza) AndAlso budget_appezza <> 0 Then
                StrSQL.AppendLine(" budget_appezza = " & Agro_SQL_SaveNum(budget_appezza) & "  ,")
            End If

            If IsNumeric(budget_id_reg) AndAlso budget_id_reg <> 0 Then
                StrSQL.AppendLine(" budget_id_reg = " & Agro_SQL_SaveNum(budget_id_reg) & "  ,")
            End If

            If IsNumeric(qta_seme_evaso) AndAlso qta_seme_evaso <> IntDefault_per_MODIFICA Then
                StrSQL.AppendLine(" Qta_Seme_Evaso = " & Agro_SQL_SaveNum(qta_seme_evaso) & "  ,")
            End If

            If IsNumeric(qta_seme_omaggio) AndAlso qta_seme_omaggio <> IntDefault_per_MODIFICA Then
                StrSQL.AppendLine(" Qta_Seme_Omaggio = " & Agro_SQL_SaveNum(qta_seme_omaggio) & "  ,")
            End If

            If Not String.IsNullOrEmpty(note_integrative) Then
                StrSQL.AppendLine(" Note_Integrative = " & Agro_SQL_SaveNum(note_integrative) & "  ,")
            End If

            If IsNumeric(id_plateau) AndAlso id_plateau <> IntDefault_per_MODIFICA Then
                StrSQL.AppendLine(" id_plateau = " & Agro_SQL_SaveNum(id_plateau) & "  ,")
            End If


            'rimuovo l ultima virgola
            'StrSQL.Remove(StrSQL.Length - 3, 3)
            StrSQL.Replace(",", "", StrSQL.Length - 3, 3)

            StrSQL.AppendLine(" WHERE Programmazione_Cod         = " & Agro_SQL_SaveNum(Programmazione_Cod) & "  ")
            StrSQL.AppendLine(" AND Programmazione_Entita_Cod         = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & "  ")
            StrSQL.AppendLine(" AND   Piva_SuperUser =  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            '---------------------------------------------

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

    Public Function Superficie_ReImpostaDaRipartoCatasto(
        ByVal Programmazione_Entita_cod As Integer,
        ByVal xFiltroAggiuntivo As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Appezzamento_Write.ModificaSingolo_CampoNumerico()"

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Programmazione_Entita_cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Programmazione_Entita_cod obbligatorio)")
            End If


            '---------------------------------------------

            stb.Length = 0
            stb.AppendLine("  update app ")
            stb.AppendLine("  set superficie = catasto.area ")
            stb.Append("         ,Data_Modifica        =  " & Agro_SQL_SaveDateTime(Now))
            stb.Append("         ,UserName_Modifica    = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            stb.AppendLine(" from( ")
            stb.AppendLine("  select Programmazione_entita_cod, sum(Superficie) as area ")
            stb.AppendLine("     From Programmazione_Particelle ")
            stb.AppendLine("     Where Programmazione_Entita_cod =  " & Programmazione_Entita_cod)

            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            stb.AppendLine("     Group By Programmazione_Entita_cod ")
            stb.AppendLine(" ) catasto ")
            stb.AppendLine("  inner Join Programmazione_entita app  ")
            stb.AppendLine("         On app.Programmazione_entita_cod = catasto.Programmazione_entita_cod ")






            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp


    End Function



    Public Function ModificaSingolo_CampoNumericoDec(ByVal Programmazione_Entita_cod As Integer,
                                                  ByVal NomeCampo As String,
                                                  ByVal CampoValoreNumerico As Decimal,
                                                  ByVal xFiltroAggiuntivo As String,
                                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                  ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Appezzamento_Write.ModificaSingolo_CampoNumerico()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            If Programmazione_Entita_cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Programmazione_Entita_cod obbligatorio)")
            End If


            '---------------------------------------------      

            StrSQL.Length = 0

            StrSQL.Append(" UPDATE   Programmazione_Entita SET ")
            StrSQL.Append("         " & NomeCampo & "     =  " & Agro_SQL_SaveNum(CampoValoreNumerico) & " ")
            StrSQL.Append("         ,Data_Modifica        =  " & Agro_SQL_SaveDateTime(Now))
            StrSQL.Append("         ,UserName_Modifica    = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            StrSQL.Append(" WHERE    Programmazione_Entita_cod = " & Agro_SQL_SaveNum(Programmazione_Entita_cod) & " ")



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

    Public Function Modifica_x_Budget(
                             ByRef Programmazione_Entita_Cod As Integer,
                             ByVal Superficie As Decimal,
                             ByVal Superficie_Futura As Decimal,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                          ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_W.Modifica()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try



            If Programmazione_Entita_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Programmazione_Des obbligatorio)")
            End If


            '---------------------------------------------
            StrSQL.Append(" UPDATE Programmazione_Entita SET  ")
            If Superficie <> -1 Then
                StrSQL.Append("    Superficie     = " & Agro_SQL_SaveNum(Superficie) & "  ,")
            End If
            If Superficie_Futura <> -1 Then
                StrSQL.Append("    Superficie_Futura     = " & Agro_SQL_SaveNum(Superficie_Futura) & "  ")
            End If

            StrSQL.Append(" WHERE  Programmazione_Entita_Cod         = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & "  ")
            StrSQL.Append(" AND   Piva_SuperUser =  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            '---------------------------------------------


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

    Public Function Modifica_PrenotazionePiante_Ordine_Small(
        ByRef Programmazione_Entita_Cod As Integer,
        ByVal piva As String,
        ByVal port_cod As Integer,
        ByVal Tra_Fila As Integer,
        ByVal Su_Fila As Integer,
        ByVal imp_cod As Integer,
        ByVal riferimento_alfanumerico_appezzamento As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_W.Modifica()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try



            If Programmazione_Entita_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Programmazione_Des obbligatorio)")
            End If


            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" UPDATE Programmazione_Entita SET  ")

            StrSQL.AppendLine("           piva                = '" & Agro_SQL_SaveText(piva) & "'")
            StrSQL.AppendLine("         , port_cod            =  " & Agro_SQL_SaveNum(port_cod) & " ")
            StrSQL.AppendLine("         , tra_fila            =  " & Agro_SQL_SaveNum(StringaNumero:=Tra_Fila) & " ")
            StrSQL.AppendLine("         , su_fila            =  " & Agro_SQL_SaveNum(Su_Fila) & " ")
            StrSQL.AppendLine("         , imp_cod            =  " & Agro_SQL_SaveNum(imp_cod) & " ")
            StrSQL.AppendLine("         , riferimento_alfanumerico_appezzamento            =  '" & Agro_SQL_SaveText(riferimento_alfanumerico_appezzamento) & "' ")

            StrSQL.AppendLine("         ,Data_Modifica        =  " & Agro_SQL_SaveDateTime(Now))
            StrSQL.AppendLine("         ,UserName_Modifica    = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            StrSQL.AppendLine(" WHERE  Programmazione_Entita_Cod         = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & "  ")
            StrSQL.AppendLine(" AND   Piva_SuperUser =  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            '---------------------------------------------


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
    Public Function Azzera_x_Budget(
                                ByRef Programmazione_Cod As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                          ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_W.Modifica()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try



            If Programmazione_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Programmazione_Des obbligatorio)")
            End If


            '---------------------------------------------
            StrSQL.Append(" UPDATE Programmazione_Entita SET  ")
            StrSQL.Append("    Superficie_Futura     = Superficie ")

            StrSQL.Append(" WHERE  Programmazione_Cod         = " & Agro_SQL_SaveNum(Programmazione_Cod) & "  ")
            StrSQL.Append(" AND   Piva_SuperUser =  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")

            '---------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND   " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
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

    Public Function ModificaFabbisogno(
                               ByVal Programmazione_Cod As Integer,
                               ByRef Programmazione_Entita_Cod As Integer,
                               ByVal N_distribuito As Decimal,
                               ByVal N_fabbisogno As Decimal,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_W.ModificaFabbisogno()"

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

            If Programmazione_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Programmazione_Cod obbligatorio)")
            End If

            If Programmazione_Entita_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Programmazione_Des obbligatorio)")
            End If


            '---------------------------------------------
            StrSQL.Append(" UPDATE Programmazione_Entita SET  ")

            If N_distribuito <> DoubleDefault_per_MODIFICA Then
                StrSQL.Append("    N_distribuito     = " & Agro_SQL_SaveNum(N_distribuito) & "  ,")
            End If
            If N_fabbisogno <> DoubleDefault_per_MODIFICA Then
                StrSQL.Append("    N_fabbisogno     = " & Agro_SQL_SaveNum(N_fabbisogno) & "  ,")
            End If


            'rimuovo l ultima virgola
            StrSQL.Remove(StrSQL.Length - 1, 1)

            StrSQL.Append(" WHERE Programmazione_Cod         = " & Agro_SQL_SaveNum(Programmazione_Cod) & "  ")
            StrSQL.Append(" AND Programmazione_Entita_Cod         = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & "  ")
            StrSQL.Append(" AND   Piva_SuperUser =  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            '---------------------------------------------

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

    Public Function Crea_Tabella_Temporanea_superfici(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_W.Scrivi()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Append("    CREATE TABLE [dbo].[appoggio]([piva_superuser] [nvarchar](25) NOT NULL,[Programmazione_Entita_Cod] [int] NOT NULL,[Superficie] [float] NULL,[Superficie_Futura] [float] NULL, ")
            StrSQL.Append("    CONSTRAINT [PK_appoggio] PRIMARY KEY CLUSTERED  ")
            StrSQL.Append("    ([Programmazione_Entita_Cod] ASC,[piva_superuser] ASC ")
            StrSQL.Append("    ) ON [PRIMARY]) ON [PRIMARY] ")

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

    Public Function CANCELLA_Tabella_Temporanea_superfici(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_W.Scrivi()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Append("   drop TABLE Appoggio  ")

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

    Public Function Scrivi_In_Tabella_Appoggio(ByRef Programmazione_Entita_Cod As Integer,
                             ByVal Superficie As Decimal,
                            ByVal Superficie_Futura As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                  ) As Boolean



        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try


            '----- Genero la query SQL 
            StrSQL.Append(" INSERT INTO Appoggio ( ")
            StrSQL.Append("             Piva_SuperUser, Programmazione_Entita_Cod,  Superficie ,Superficie_Futura ")
            StrSQL.Append(" ) ")

            StrSQL.Append(" VALUES ( ")
            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser.ToString) & "' ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Programmazione_Entita_Cod.ToString) & " ")

            StrSQL.Append("         ," & Agro_SQL_SaveNum(Superficie.ToString) & " ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Superficie_Futura) & "  ")

            StrSQL.Append(" ) ")


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

    Public Function Update_from_Tabella_Appoggio(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try


            '----- Genero la query SQL 
            StrSQL.Append(" update ent ")
            StrSQL.Append("     set  superficie = app.superficie , superficie_futura = app.superficie_futura ")
            StrSQL.Append("     from programmazione_entita ent ")
            StrSQL.Append("     inner join appoggio app on app.piva_superuser = ent.piva_superuser and app.programmazione_entita_cod =  ent.programmazione_entita_cod ")


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

    Public Function Modifica_Validita_Fine(
                                ByVal Programmazione_Cod As Integer,
                                ByRef Programmazione_Entita_Cod As Integer,
                                ByVal Validita_Fine As Date,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                             ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_W.Modifica_Validita_Fine()"

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

            If Programmazione_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Programmazione_Cod obbligatorio)")
            End If

            'If Programmazione_Entita_Cod = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Programmazione_Des obbligatorio)")
            'End If


            '---------------------------------------------
            StrSQL.Append(" UPDATE Programmazione_Entita SET  ")

            StrSQL.Append("    Validita_Fine     = " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.Append("    , UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("    , Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))

            StrSQL.Append(" WHERE Programmazione_Cod         = " & Agro_SQL_SaveNum(Programmazione_Cod) & "  ")
            StrSQL.Append(" AND   Piva_SuperUser =  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")

            If Programmazione_Entita_Cod <> 0 Then
                StrSQL.Append(" AND Programmazione_Entita_Cod         = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & "  ")
            End If

            '---------------------------------------------

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

    Public Function Modifica_Validita(
                            ByVal Programmazione_Cod As Integer,
                            ByRef Programmazione_Entita_Cod As Integer,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                                     ByVal xFiltroAggiuntivo As String,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                         ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_W.Modifica_Validita()"

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

            If Programmazione_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Programmazione_Cod obbligatorio)")
            End If

            'If Programmazione_Entita_Cod = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Programmazione_Des obbligatorio)")
            'End If


            '---------------------------------------------
            StrSQL.Append(" UPDATE Programmazione_Entita SET  ")

            StrSQL.Append("    Validita_Inizio     = " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            StrSQL.Append("    , Validita_Fine     = " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append("    , UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("    , Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))

            StrSQL.Append(" WHERE Programmazione_Cod         = " & Agro_SQL_SaveNum(Programmazione_Cod) & "  ")
            StrSQL.Append(" AND   Piva_SuperUser =  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")

            If Programmazione_Entita_Cod <> 0 Then
                StrSQL.Append(" AND Programmazione_Entita_Cod         = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & "  ")
            End If

            '---------------------------------------------

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

    Public Function Modifica_Campo_Cod(
                            ByVal Programmazione_Cod As Integer,
                            ByRef Programmazione_Entita_Cod As Integer,
                            ByVal Campo_cod As Integer,
                                     ByVal xFiltroAggiuntivo As String,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                         ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_W.Modifica_Campo_Cod()"

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

            If Programmazione_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Programmazione_Cod obbligatorio)")
            End If

            'If Programmazione_Entita_Cod = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Programmazione_Des obbligatorio)")
            'End If


            '---------------------------------------------
            StrSQL.Append(" UPDATE Programmazione_Entita SET  ")

            StrSQL.Append("    Campo_cod     = " & Agro_SQL_SaveNum(Campo_cod) & " ")
            StrSQL.Append("    , UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("    , Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))

            StrSQL.Append(" WHERE Programmazione_Cod         = " & Agro_SQL_SaveNum(Programmazione_Cod) & "  ")
            StrSQL.Append(" AND   Piva_SuperUser =  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")

            If Programmazione_Entita_Cod <> 0 Then
                StrSQL.Append(" AND Programmazione_Entita_Cod         = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & "  ")
            End If

            '---------------------------------------------

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

    Public Function Azzera_Campo_Cod(
                            ByVal Piva As String,
                            ByRef sa_cod As Integer,
                            ByVal Campo_cod As Integer,
                            ByVal xFiltroAggiuntivo As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_W.Azzera_Campo_Cod()"

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

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (piva obbligatorio)")
            End If

            If sa_cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (sa_cod obbligatorio)")
            End If

            If Campo_cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Campo_cod obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Append(" UPDATE Programmazione_Entita SET  ")

            StrSQL.Append("    Campo_cod     = " & Agro_SQL_SaveNum(0) & " ")
            StrSQL.Append("    , UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("    , Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))

            StrSQL.Append(" WHERE Piva_SuperUser =  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            StrSQL.Append(" AND Piva =  '" & Agro_SQL_SaveText(Piva) & "'   ")
            StrSQL.Append(" AND sa_cod = " & Agro_SQL_SaveNum(sa_cod) & "  ")
            StrSQL.Append(" AND Campo_cod = " & Agro_SQL_SaveNum(Campo_cod) & "  ")

            '---------------------------------------------

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

    Public Function Modifica_Residuo_Ordine(
                            ByVal Programmazione_Cod As Integer,
                            ByRef Residuo As Integer,
                            ByRef Qta_Associata As Integer,
                                     ByVal xFiltroAggiuntivo As String,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                         ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_W.Modifica_Validita()"

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

            If Programmazione_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Programmazione_Cod obbligatorio)")
            End If

            'If Programmazione_Entita_Cod = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Programmazione_Des obbligatorio)")
            'End If


            '---------------------------------------------
            StrSQL.Append(" UPDATE Programmazione_Entita SET  ")

            StrSQL.Append("    Sa_cod     = " & Agro_SQL_SaveNum(Residuo) & " ")
            StrSQL.Append("    ,Campo_Cod     = " & Agro_SQL_SaveNum(Qta_Associata) & " ")

            StrSQL.Append("    , UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("    , Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))

            StrSQL.Append(" WHERE Programmazione_Cod         = " & Agro_SQL_SaveNum(Programmazione_Cod) & "  ")
            StrSQL.Append(" AND   Piva_SuperUser =  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")


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

    '################################################################################
    Public Function Pianificazione_Modifica_GIS(
            ByVal Programamzione_entita_cod As Integer,
            ByVal sup_imp As Double,
            ByVal via_stringa As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As String

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_W.Aggiorna_TestataGriglia_Calibri()"
        Dim MessaggioErrore As String = ""

        Try

            Dim gefutils As New Gias_EF_Utility

            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
            Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim entitaDaModificare As New Programmazione_Entita

            entitaDaModificare.Programmazione_Entita_Cod = Programamzione_entita_cod
            entitaDaModificare.Piva_SuperUser = objParametri.PivaSuperUser

            GiasContext.Programmazione_Entita.Attach(entitaDaModificare)


            If sup_imp <> 0 Then
                entitaDaModificare.Superficie = sup_imp
            End If

            entitaDaModificare.Via_Stringa = via_stringa

            GiasContext.SaveChanges()

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return MessaggioErrore

    End Function

    Public Function Modifica_Centro(ByVal Programmazione_Entita_Cod As Integer,
                        ByVal Sa_Cod_OLD As Int32,
                        ByVal Sa_Cod_NEW As Int32,
                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_W.Modifica_Centro()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.Append(" UPDATE Programmazione_Entita SET  ")

            StrSQL.Append("    sa_cod     = " & Agro_SQL_SaveNum(Sa_Cod_NEW) & " ")
            StrSQL.Append("    , UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("    , Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))

            StrSQL.Append(" WHERE Piva_SuperUser =  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")

            If Programmazione_Entita_Cod <> 0 Then
                StrSQL.Append(" AND Programmazione_Entita_Cod         = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & "  ")
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

    'Query usata per la modifica vivaio
    Public Function ModificaPiva(Programmazione_Entita_Cod As Integer,
                                 Programmazione_Cod As Integer,
                                 piva As String,
                                 veg_cod_cliente As String,
                                 xFiltroAggiuntivo As String,
                                 objParametri As AgronicaCoreParametri)
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_W.ModificaPiva()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" UPDATE Programmazione_Entita SET  ")

            If Not String.IsNullOrEmpty(piva) Then
                StrSQL.AppendLine("    Piva     = '" & Agro_SQL_SaveText(piva) & "' , ")
            End If

            If Not String.IsNullOrEmpty(veg_cod_cliente) Then
                StrSQL.AppendLine("    Veg_Cod_Cliente     = '" & Agro_SQL_SaveText(veg_cod_cliente) & "' ,")
            End If

            StrSQL.AppendLine("     UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "', ")
            StrSQL.AppendLine("     Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))

            StrSQL.AppendLine(" WHERE Piva_SuperUser =  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")

            If Programmazione_Entita_Cod <> 0 Then
                StrSQL.AppendLine(" AND Programmazione_Entita_Cod = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & "  ")
            End If

            If Programmazione_Cod <> 0 Then
                StrSQL.AppendLine(" AND Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod) & "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendFormat(" AND {0}", xFiltroAggiuntivo)
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
