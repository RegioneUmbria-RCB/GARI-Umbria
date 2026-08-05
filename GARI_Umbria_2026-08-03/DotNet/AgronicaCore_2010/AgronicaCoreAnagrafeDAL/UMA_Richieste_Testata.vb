Imports System.Data.Entity
Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO

Public Class UMA_Richieste_Testata_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal piva As String,
                          ByVal Richiesta_Cod As Integer,
                          ByVal Pratica_cod As Integer,
                          ByVal Validita_Inizio As Date,
                          ByVal Validita_Fine As Date,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          Optional isTerzista As Integer = 1,
                          Optional anno As Integer = 0,
                          Optional ByVal avanzamento As Integer = -1,
                          Optional ByVal xOrderBy As String = "") As DataTable


        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_Richieste_Testata_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM UMA_Richieste_Testata t ")
            If anno <> 0 Then
                stb.AppendLine("JOIN Pratiche p on p.pratica_cod = t.pratica_cod")
            End If
            stb.AppendLine(" WHERE t.Piva_SuperUser = '" + Agro_SQL_SaveText(objParametri.PivaSuperUser) + "' ")
            If piva <> "" Then
                stb.AppendLine(" AND t.piva = '" + Agro_SQL_SaveText(piva) + "' ")
            End If

            If Richiesta_Cod <> 0 Then
                stb.AppendLine(" AND t.Richiesta_Cod = " + Agro_SQL_SaveNum(Richiesta_Cod) + " ")
            End If

            If Pratica_cod <> 0 Then
                stb.AppendLine(" AND t.Pratica_cod = " + Agro_SQL_SaveNum(Pratica_cod) + " ")
            End If

            If Validita_Inizio <> AGRODATAINIZIO Then
                stb.AppendLine(" AND t.Validita_Inizio >= " + Agro_SQL_SaveDateTime(Validita_Inizio) + " ")
            End If

            If Validita_Fine <> AGRODATAFINE Then
                stb.AppendLine(" AND t.Validita_Fine <= " + Agro_SQL_SaveDateTime(Validita_Fine) + " ")
            End If

            If anno <> 0 Then
                stb.AppendLine(" AND p.anno = " + anno.ToString + " ")
            End If

            If avanzamento <> -1 Then
                stb.AppendLine(" AND t.avanzamento_richiesta = " + avanzamento.ToString + " ")
            End If

            If isTerzista <> 1 Then
                stb.AppendLine(" AND t.Tipo_Richiesta = " + isTerzista.ToString + " ")
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " + xOrderBy + " ")
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

    Public Function Leggi(ByVal piva As String,
                          ByVal Richiesta_Cod As Integer,
                          ByVal Pratica_cod As Integer,
                          ByVal Validita_Inizio As Date,
                          ByVal Validita_Fine As Date,
                          ByVal Anno As Integer,
                          ByVal Numero As String,
                          ByVal Avanzamento_Richiesta As Integer,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          Optional isTerzista As Boolean = False,
                          Optional xFiltroAggiuntivo As String = "") As DataTable


        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_Richieste_Testata_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM UMA_Richieste_Testata ")
            stb.AppendLine(" JOIN Pratiche ON UMA_Richieste_Testata.Pratica_Cod = Pratiche.Pratica_Cod ")
            stb.AppendLine(" JOIN Pratiche_Stati_Attuali ON Pratiche_Stati_Attuali.Pratica_Cod = Pratiche.Pratica_Cod ")
            stb.AppendLine(" WHERE UMA_Richieste_Testata.Piva_SuperUser = '" + Agro_SQL_SaveText(objParametri.PivaSuperUser) + "' ")
            If piva <> "" Then
                stb.AppendLine(" AND UMA_Richieste_Testata.piva = '" + Agro_SQL_SaveText(piva) + "' ")
            End If

            If Richiesta_Cod <> 0 Then
                stb.AppendLine(" AND UMA_Richieste_Testata.Richiesta_Cod = " + Agro_SQL_SaveNum(Richiesta_Cod) + " ")
            End If

            If Pratica_cod <> 0 Then
                stb.AppendLine(" AND UMA_Richieste_Testata.Pratica_cod = " + Agro_SQL_SaveNum(Pratica_cod) + " ")
            End If

            If Anno <> 0 Then
                stb.AppendLine(" AND Pratiche.Anno = " + Agro_SQL_SaveNum(Anno) + " ")
            End If

            If Numero <> "" Then
                stb.AppendLine(" AND Pratiche.Numero =  '" + Agro_SQL_SaveText(Numero) + "' ")
            End If

            If Avanzamento_Richiesta <> -1 Then
                stb.AppendLine(" AND UMA_Richieste_Testata.Avanzamento_Richiesta = " + Agro_SQL_SaveNum(Avanzamento_Richiesta) + " ")
            End If

            If Validita_Inizio <> AGRODATAINIZIO Then
                stb.AppendLine(" AND UMA_Richieste_Testata.Validita_Inizio >= " + Agro_SQL_SaveDateTime(Validita_Inizio) + " ")
            End If

            If Validita_Fine <> AGRODATAFINE Then
                stb.AppendLine(" AND UMA_Richieste_Testata.Validita_Fine <= " + Agro_SQL_SaveDateTime(Validita_Fine) + " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " + Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo) + " ")
            End If

            stb.AppendLine(" AND Tipo_Richiesta = " + IIf(isTerzista, "-1", "0") + " ")

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

    Public Function Leggi_rimanenza_iniziale(ByVal piva As String,
                                             ByVal Avanzamento_Richiesta As Integer,
                                             ByVal Tipo_Richiesta As Integer,
                                             ByVal anno As Integer,
                                             ByRef objParametri_Server As AgronicaCoreParametri)

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_Richieste_Testata_R.Leggi_rimanenza_iniziale()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT TOP (1) t.Rimanenza_Gasolio, t.Rimanenza_Benzina, t.Rimanenza_Gasolio_Serra ")
            stb.AppendLine(" FROM UMA_Richieste_Testata t")
            stb.AppendLine(" JOIN Pratiche p ON p.pratica_cod = t.pratica_cod ")
            stb.AppendLine(" WHERE t.Avanzamento_Richiesta = " & Avanzamento_Richiesta & " and t.Richiesta_Integrativa = 0 and t.Piva = '" & piva & "' ")
            stb.AppendLine(" And t.tipo_richiesta = " & Tipo_Richiesta & " And p.Anno = " & anno & " ")
            stb.AppendLine(" ORDER BY p.Numero ASC, t.Data_Creazione ASC")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Server, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function LeggiDaRichiestaCod(ByVal richiestaCod As Integer,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable


        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_Richieste_Testata_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM UMA_Richieste_Testata ")
            stb.AppendLine(" WHERE richiesta_cod = '" + Agro_SQL_SaveNum(richiestaCod) + "' ")
            stb.AppendLine(" AND Piva_SuperUser = '" + Agro_SQL_SaveText(objParametri.PivaSuperUser) + "' ")

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


    Public Function Leggi_Elenco(ByVal piva As String,
                                 ByVal xFiltroAggiuntivo As String,
                                 ByVal xOrderBy As String,
                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                 Optional ByVal rendicontazioni As Boolean = False,
                                 Optional ByVal statoCod As Integer = -1,
                                 Optional ByVal citta As String = "-1") As DataTable


        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_Richieste_Testata_R.Leggi_Elenco()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT t.Piva, t.Pratica_Cod, p.Anno, p.Numero, psa.stato_cod, psaw.WAnagraficaStati_Des ")
            stb.AppendLine(" ,psa.Validita_Inizio As Data_Ultimo_Passaggio_Stato, t.Validita_Inizio, t.Validita_Fine, t.richiesta_Cod ")
            stb.AppendLine(" ,t.Carburante_Calcolato, t.Carburante_Richiesto, t.Carburante_Approvato, t.Tipo_Richiesta  ")
            stb.AppendLine(" FROM UMA_Richieste_Testata t ")
            stb.AppendLine(" join Pratiche p on t.Piva = p.Piva and t.Pratica_Cod = p.Pratica_Cod ")
            stb.AppendLine(" join Pratiche_Stati_Attuali psa on p.Piva_SuperUser = psa.Piva_SuperUser and p.Pratica_Cod = psa.Pratica_Cod ")
            stb.AppendLine(" join WAnagraficaStati psaw on psa.Stato_Cod = psaw.WAnagraficaStati_Cod ")
            If (citta <> "-1") Then
                stb.AppendLine(" INNER JOIN ImpresexIndirizzi ON t.Piva = ImpresexIndirizzi.Piva ")
                stb.AppendLine(" INNER JOIN Indirizzi ON ImpresexIndirizzi.Cod_Indirizzo = Indirizzi.Cod_Indirizzo  ")
                stb.AppendLine(" JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM AND ISTAT.LOCALITA = '" + citta + "' ")
            End If
            stb.AppendLine(" WHERE 1 = 1")
            If (piva <> "") Then
                stb.AppendLine(" AND t.piva = '" + Agro_SQL_SaveText(piva) + "' ")
            End If

            stb.AppendLine(" AND t.Piva_SuperUser = '" + Agro_SQL_SaveText(objParametri.PivaSuperUser) + "' ")
            stb.AppendLine(" AND t.Avanzamento_Richiesta " + IIf(rendicontazioni, " = 1 ", " = 0 "))

            If (statoCod > 0) Then
                stb.AppendLine(" AND psaw.WAnagraficaStati_Cod = " + statoCod.ToString + " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND  " + Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo) + " ")
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " + Agro_SQL_Save_xOrderBy(xOrderBy) + " ")
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


    Public Function Leggi_Elenco2(ByVal piva As String,
                                 ByVal xFiltroAggiuntivo As String,
                                 ByVal xOrderBy As String,
                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                 Optional ByVal rendicontazioni As Boolean = False,
                                 Optional ByVal statoCod As Integer = -1,
                                 Optional ByVal citta As String = "-1") As DataTable


        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_Richieste_Testata_R.Leggi_Elenco2()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT t.Piva, t.Pratica_Cod, p.Anno, p.Numero, ")
            stb.AppendLine(" t.Validita_Inizio, t.Validita_Fine, t.richiesta_Cod ")
            stb.AppendLine(" ,t.Carburante_Calcolato, t.Carburante_Richiesto, t.Carburante_Approvato, t.Tipo_Richiesta  ")
            stb.AppendLine(" FROM UMA_Richieste_Testata t ")
            stb.AppendLine(" join Pratiche p on t.Piva = p.Piva and t.Pratica_Cod = p.Pratica_Cod ")

            stb.AppendLine(" WHERE 1 = 1")
            If (piva <> "") Then
                stb.AppendLine(" AND t.piva = '" + Agro_SQL_SaveText(piva) + "' ")
            End If

            stb.AppendLine(" AND t.Piva_SuperUser = '" + Agro_SQL_SaveText(objParametri.PivaSuperUser) + "' ")

            If (statoCod > 0) Then
                stb.AppendLine(" AND psaw.WAnagraficaStati_Cod = " + statoCod.ToString + " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND  " + Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo) + " ")
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " + Agro_SQL_Save_xOrderBy(xOrderBy) + " ")
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

    Public Function Leggi_Stato_Modificabile(ByVal piva As String, ByVal richiestea_Cod As Integer, ByVal isApprovazione As Boolean,
                                ByVal xFiltroAggiuntivo As String, ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_Richieste_Testata_R.Leggi_Stato_Modificabile()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable
        Dim res As Boolean = False

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM UMA_Richieste_Testata t")
            stb.AppendLine(" JOIN Pratiche_Stati_Attuali p on p.Pratica_Cod = t.Pratica_Cod ")
            stb.AppendLine(" WHERE richiesta_cod = " + Agro_SQL_SaveNum(richiestea_Cod) + " ")
            stb.AppendLine(" AND t.Piva_SuperUser = '" + Agro_SQL_SaveText(objParametri.PivaSuperUser) + "' ")
            stb.AppendLine(" AND p.stato_cod = " + IIf(isApprovazione, "2002", "2001") + " ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If (dt.Rows.Count > 0) Then
            res = True
        End If

        Return res

    End Function

    Public Function LeggiRiepilogo(ByVal piva As String,
                                   ByVal anno As Integer,
                                   ByVal filtroVisibilita As Boolean,
                                   ByVal piveVisibili As String,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByVal xOrderBy As String,
                                   ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                   ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                   ByVal rendicontazioni As Boolean,
                                   ByVal statoCod As Integer,
                                   ByVal citta As String,
                                   ByVal prov As String,
                                   ByVal conto As Integer,
                                   ByVal FiltroNuovaVisibilita As Boolean,
                                   ByVal FiltroUtente As Boolean,
                                   ByVal FiltroGruppoUtente As Boolean,
                                   ByVal GruppoUtente As Integer,
                                   ByVal VisibilitaTotale As Boolean) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_Richieste_Testata_R.LeggiRiepilogo()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            'Creazione delle CTE
            stb.AppendLine(" WITH  ")
            stb.AppendLine("  PraticheXAnno_CTE as ")
            stb.AppendLine("  ( ")
            stb.AppendLine("         Select  p.anno, p.Piva_SuperUser, p.piva, p.Pratica_Cod from pratiche p (nolock) ")
            stb.AppendLine("         JOIN Pratiche_Stati_Attuali psa (nolock) On p.Piva_SuperUser = psa.Piva_SuperUser And p.Pratica_Cod = psa.Pratica_Cod  ")
            stb.AppendLine("         JOIN WAnagraficaStati psaw (nolock) On psa.Stato_Cod = psaw.WAnagraficaStati_Cod  ")
            stb.AppendLine("         where p.anno = " + anno.ToString + " And p.Servizio_Cod = 2007 And psa.Stato_Cod IN (2004, 2005) ")
            If (statoCod > 0) Then
                stb.AppendLine(" AND psaw.WAnagraficaStati_Cod = " + statoCod.ToString + " ")
            End If
            stb.AppendLine("   ), ")

            stb.AppendLine(" Testa_CTE AS  ")
            stb.AppendLine("( ")
            stb.AppendLine(" Select t.piva, p.anno, t.tipo_richiesta, ")
            stb.AppendLine(" MAX(t.Rimanenza_Gasolio) As Rimanenza_Gasolio, MAX(t.Rimanenza_Benzina) As Rimanenza_Benzina, MAX(t.Rimanenza_Gasolio_Serra) As Rimanenza_Gasolio_Serra ")
            stb.AppendLine(" from UMA_Richieste_Testata t (nolock) ")
            stb.AppendLine(" Join PraticheXAnno_CTE p on p.Pratica_Cod = t.Pratica_Cod ")
            stb.AppendLine(" Where Avanzamento_Richiesta = 0 ")
            stb.AppendLine(" GROUP BY p.Anno, t.piva, t.tipo_richiesta  ")
            stb.AppendLine(" ) , ")

            'Una CTE per ogni tipo di carburante sia per richieste che per rendicontazioni
            stb.AppendLine(" Lav_CTE(Piva, Avanzamento_Richiesta, Tipo_Richiesta, Tipo_Carburante, Fabbisogno_Richiesto, Fabbisogno_Assegnato, Pratica_Cod, Rimanenza_Gasolio, Rimanenza_Benzina, Rimanenza_Gasolio_Serra) AS  ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" SELECT t.piva, t.Avanzamento_Richiesta, t.Tipo_Richiesta, l.Tipo_Carburante, l.Fabbisogno_Richiesto, l.Fabbisogno_Assegnato, t.pratica_cod, t.Rimanenza_Gasolio, t.rimanenza_benzina, t.rimanenza_gasolio_serra ")
            stb.AppendLine(" FROM UMA_Richieste_Lavorazioni l (nolock) ")
            stb.AppendLine(" JOIN UMA_Richieste_Testata t (nolock) on t.Richiesta_Cod = l.Richiesta_Cod ")
            stb.AppendLine(" JOIN PraticheXAnno_CTE p on p.Pratica_Cod = t.pratica_cod ")
            stb.AppendLine(" ), ")
            stb.AppendLine(" Lav_Avanzamento_0_CTE ")
            stb.AppendLine(" As ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" Select * from Lav_CTE where Avanzamento_Richiesta = 0 ")
            stb.AppendLine(" ), ")
            stb.AppendLine(" Lav_Avanzamento_1_CTE ")
            stb.AppendLine(" As ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" Select * from Lav_CTE where Avanzamento_Richiesta = 1 ")
            stb.AppendLine(" ), ")
            stb.AppendLine(" Lav_Avanzamento_0_Gasolio_CTE ")
            stb.AppendLine(" As ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" Select Piva, Tipo_Richiesta, SUM(Fabbisogno_Richiesto) As Fabbisogno_Richiesto, SUM(Fabbisogno_Assegnato) As Fabbisogno_Assegnato ")
            stb.AppendLine(" from Lav_Avanzamento_0_CTE where Tipo_Carburante = 2   ")
            stb.AppendLine(" Group By Piva, Tipo_Richiesta  ")
            stb.AppendLine(" ), ")
            stb.AppendLine(" Lav_Avanzamento_0_Benzina_CTE ")
            stb.AppendLine(" As ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" Select Piva, Tipo_Richiesta, SUM(Fabbisogno_Richiesto) As Fabbisogno_Richiesto, SUM(Fabbisogno_Assegnato) As Fabbisogno_Assegnato ")
            stb.AppendLine(" from Lav_Avanzamento_0_CTE where Tipo_Carburante = 3  ")
            stb.AppendLine(" Group By Piva, Tipo_Richiesta ")
            stb.AppendLine(" ), ")
            stb.AppendLine(" Lav_Avanzamento_0_GasolioSerra_CTE ")
            stb.AppendLine(" As ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" Select Piva, Tipo_Richiesta, SUM(Fabbisogno_Richiesto) As Fabbisogno_Richiesto, SUM(Fabbisogno_Assegnato) As Fabbisogno_Assegnato ")
            stb.AppendLine(" from Lav_Avanzamento_0_CTE where Tipo_Carburante = 8 ")
            stb.AppendLine("  Group By Piva, Tipo_Richiesta ")
            stb.AppendLine(" ), ")
            stb.AppendLine(" Lav_Avanzamento_1_Gasolio_CTE ")
            stb.AppendLine(" As ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" Select Piva, Tipo_Richiesta, SUM(Fabbisogno_Richiesto) As Fabbisogno_Richiesto, SUM(Fabbisogno_Assegnato) As Fabbisogno_Assegnato, ")
            stb.AppendLine(" AVG(Rimanenza_Gasolio) As Rimanenza_Gasolio")
            stb.AppendLine(" from Lav_Avanzamento_1_CTE where Tipo_Carburante = 2   ")
            stb.AppendLine(" Group By Piva, Tipo_Richiesta  ")
            stb.AppendLine(" ), ")
            stb.AppendLine(" Lav_Avanzamento_1_Benzina_CTE ")
            stb.AppendLine(" As ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" Select Piva, Tipo_Richiesta, SUM(Fabbisogno_Richiesto) As Fabbisogno_Richiesto, SUM(Fabbisogno_Assegnato) As Fabbisogno_Assegnato, ")
            stb.AppendLine(" AVG(Rimanenza_Benzina) As Rimanenza_Benzina ")
            stb.AppendLine(" from Lav_Avanzamento_1_CTE where Tipo_Carburante = 3  ")
            stb.AppendLine(" Group By Piva, Tipo_Richiesta ")
            stb.AppendLine(" ), ")
            stb.AppendLine(" Lav_Avanzamento_1_GasolioSerra_CTE ")
            stb.AppendLine(" As ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" Select Piva, Tipo_Richiesta, SUM(Fabbisogno_Richiesto) As Fabbisogno_Richiesto, SUM(Fabbisogno_Assegnato) As Fabbisogno_Assegnato, ")
            stb.AppendLine(" AVG(Rimanenza_Gasolio_Serra) As Rimanenza_Gasolio_Serra ")
            stb.AppendLine(" from Lav_Avanzamento_1_CTE where Tipo_Carburante = 8 ")
            stb.AppendLine("  Group By Piva, Tipo_Richiesta ")
            stb.AppendLine(" ), ")
            stb.AppendLine(" Terzisti_Senza_Lav AS ")
            stb.AppendLine(" (")
            stb.AppendLine("  Select t.piva, t.tipo_richiesta, SUM(t.Richiesta_Iniziale_Gasolio) As Richiesta_Iniziale_Gasolio, SUM(t.Richiesta_Iniziale_Benzina) As Richiesta_Iniziale_Benzina, ")
            stb.AppendLine("  SUM(t.Richiesta_Iniziale_Gasolio_Serra) As Richiesta_Iniziale_Gasolio_Serra, SUM(t.Approvazione_Iniziale_Gasolio) As Approvazione_Iniziale_Gasolio, ")
            stb.AppendLine("  SUM(t.Approvazione_Iniziale_Benzina) As Approvazione_Iniziale_Benzina, SUM(t.Approvazione_Iniziale_Gasolio_Serra) As Approvazione_Iniziale_Gasolio_Serra")
            stb.AppendLine("  from UMA_Richieste_Testata t")
            stb.AppendLine("  JOIN PraticheXAnno_CTE p on p.Pratica_Cod = t.pratica_cod")
            stb.AppendLine("  WHERE t.Carburante_Calcolato = 0 AND (t.Richiesta_Iniziale_Gasolio > 0 OR t.Richiesta_Iniziale_Benzina > 0 OR t.Richiesta_Iniziale_Gasolio_Serra > 0)")
            stb.AppendLine("  Group By t.Piva, Tipo_Richiesta ")
            stb.AppendLine("  ), ")
            stb.AppendLine(" Allevamenti_CTE AS ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" Select al.piva, t.tipo_Richiesta, al.Carburante_Richiesto, al.Carburante_Approvato As Carburante_Approvato , al.Tipo_Carburante ")
            stb.AppendLine(" From UMA_Richieste_Allevamenti al (nolock) ")
            stb.AppendLine(" Join UMA_Richieste_Testata t (nolock) on t.Richiesta_Cod = al.Richiesta_Cod ")
            stb.AppendLine(" Join PraticheXAnno_CTE p on p.Pratica_Cod = t.Pratica_Cod ")
            stb.AppendLine(" ), ")
            stb.AppendLine(" Carburante_Gasolio_Allevamenti_CTE As ")
            stb.AppendLine(" ( ")
            stb.AppendLine("      Select piva, tipo_richiesta, SUM(Carburante_Richiesto) As Carburante_Richiesto, SUM(Carburante_Approvato) As Carburante_Approvato ")
            stb.AppendLine("      from Allevamenti_CTE ")
            stb.AppendLine("      where Tipo_Carburante = 2 ")
            stb.AppendLine("      group by piva, tipo_richiesta ")
            stb.AppendLine(" ), ")
            stb.AppendLine(" Carburante_Benzina_Allevamenti_CTE As ")
            stb.AppendLine(" ( ")
            stb.AppendLine("      Select piva, tipo_richiesta, SUM(Carburante_Richiesto) As Carburante_Richiesto, SUM(Carburante_Approvato) As Carburante_Approvato ")
            stb.AppendLine("      from Allevamenti_CTE ")
            stb.AppendLine("      where Tipo_Carburante = 3 ")
            stb.AppendLine("      group by piva, tipo_richiesta ")
            stb.AppendLine(" ), ")

            stb.AppendLine(" Pratiche_Stati_Attuali_CTE As ")
            stb.AppendLine(" ( ")
            stb.AppendLine("       Select * From Pratiche_Stati_Attuali (nolock) Where Stato_Cod In (2003, 2005, 2008) ")
            stb.AppendLine("       And Piva_SuperUser = '" + Agro_SQL_SaveText(objParametri_Server.PivaSuperUser) + "' ")
            stb.AppendLine(" ), ")

            stb.AppendLine(" Indirizzi_CTE as ")
            stb.AppendLine("   ( ")
            stb.AppendLine("          Select  ImpresexIndirizzi.PIVA ")
            stb.AppendLine("          From ImpresexIndirizzi (nolock)  ")
            stb.AppendLine("          INNER Join Indirizzi ind (nolock) On ImpresexIndirizzi.Cod_Indirizzo = ind.Cod_Indirizzo   ")
            stb.AppendLine("          Join ISTAT (nolock) ON ind.pro_cod_istat = ISTAT.PROV And ind.com_cod_istat = ISTAT.COM  ")
            stb.AppendLine("          Where 1=1 ")
            If (citta <> "-1" AndAlso citta <> "") Then
                stb.AppendLine(" AND ISTAT.COM = '" + Agro_SQL_SaveText(citta) + "' ")
            End If
            If (prov <> "-1") Then
                stb.AppendLine(" AND ISTAT.PROV = '" + Agro_SQL_SaveText(prov) + "' ")
            End If
            stb.AppendLine("    ), ")

            stb.AppendLine("  Imprese_CTE as ")
            stb.AppendLine("       ( ")
            stb.AppendLine("        Select  i.piva, i.Rag_Soc, ic.val_cod as CUAA from Imprese i (nolock) ")
            stb.AppendLine("        join Imprese_Codici ic on i.PIVA = ic.Piva ")
            stb.AppendLine("        WHERE ic.id_cod = 1010 ")
            stb.AppendLine("       ) ")

            'Query più esterna per il calcolo dell'acquistabile
            stb.AppendLine(" select *, ")
            stb.AppendLine("   qf.approvato_Gasolio_Totale - qf.Acquistato_Gasolio - qf.Rimanenza_Iniziale_Gasolio As Acquistabile_Gasolio, ")
            stb.AppendLine("   qf.approvato_Benzina_Totale - qf.Acquistato_Benzina - qf.Rimanenza_Iniziale_Benzina As Acquistabile_Benzina, ")
            stb.AppendLine("   qf.Richiesto_Approvato_Gasolio_Serra - qf.Acquistato_Gasolio_Serra - qf.Rimanenza_Iniziale_Gasolio_Serra As Acquistabile_Gasolio_Serra ")
            stb.AppendLine("   from ")

            'Query per calcolo dei totali (Colture + Allevamenti)
            stb.AppendLine(" ( ")
            stb.AppendLine(" select  ")
            stb.AppendLine("    Rag_Soc, CUAA, ")
            stb.AppendLine("    Richiesta_Gasolio_Coltura + Gasolio_Richiesto_Allevamenti as richiesto_Gasolio_Totale , ")
            stb.AppendLine("    Richiesta_Benzina_Coltura + Benzina_Richiesto_Allevamenti as richiesto_Benzina_Totale , ")
            stb.AppendLine("    Richiesto_Gasolio_Serra, ")
            stb.AppendLine("    Rendicontato_Gasolio,  ")
            stb.AppendLine("    Rendicontato_Benzina,  ")
            stb.AppendLine("    Rendicontato_Gasolio_Serra, ")
            stb.AppendLine("    Gasolio_Approvato_Allevamenti + Approvato_Gasolio_Coltura as approvato_Gasolio_Totale , ")
            stb.AppendLine("    Benzina_Approvato_Allevamenti + Approvato_Benzina_Coltura as approvato_Benzina_Totale , ")
            stb.AppendLine("    Richiesto_Approvato_Gasolio_Serra, ")
            stb.AppendLine("    Rendicontato_Approvato_Gasolio, ")
            stb.AppendLine("    Rendicontato_Approvato_Benzina,  ")
            stb.AppendLine("    Rendicontato_Approvato_Gasolio_Serra, ")
            stb.AppendLine("    Rimanenza_Iniziale_Gasolio,  ")
            stb.AppendLine("    Rimanenza_Iniziale_Benzina, ")
            stb.AppendLine("    Rimanenza_Iniziale_Gasolio_Serra, ")
            stb.AppendLine("    Rimanenza_Finale_Gasolio, ")
            stb.AppendLine("    Rimanenza_Finale_Benzina, ")
            stb.AppendLine("    Rimanenza_Finale_Gasolio_Serra, ")
            stb.AppendLine("    Conto, ")
            stb.AppendLine("    Acquistato_Gasolio, ")
            stb.AppendLine("    Acquistato_Benzina,   ")
            stb.AppendLine("    Acquistato_Gasolio_Serra, ")
            stb.AppendLine("    Anno ")
            stb.AppendLine(" from ")

            'Query per distinguere tra carburante conto terzi o conto proprio
            stb.AppendLine(" ( ")
            stb.AppendLine("    select  ")
            stb.AppendLine("           qp.Rag_Soc, qp.CUAA, ")
            stb.AppendLine("           isnull(cgr.Carburante_Richiesto,0) as Gasolio_Richiesto_Allevamenti, ")
            stb.AppendLine("           ISNULL(cbr.Carburante_Richiesto, 0) as Benzina_Richiesto_Allevamenti, ")
            stb.AppendLine("           case when qp.Tipo_Richiesta = -1 THEN isnull(qp.Richiesta_Iniziale_Gasolio_Terzisti,0) + isnull(qp.Richiesta_Iniziale_Gasolio,0) ELSE isnull(qp.Richiesta_Iniziale_Gasolio,0) END As Richiesta_Gasolio_Coltura, ")
            stb.AppendLine("           Case When qp.Tipo_Richiesta = -1 THEN isnull(qp.Richiesta_Iniziale_Benzina_Terzisti,0) + isnull(qp.Richiesta_Iniziale_Benzina,0) ELSE isnull(qp.Richiesta_Iniziale_Benzina,0) END As Richiesta_Benzina_Coltura, ")
            stb.AppendLine("           Case When qp.Tipo_Richiesta = -1 THEN isnull(qp.Richiesta_Iniziale_Gasolio_Serra_Terzisti,0) + isnull(qp.Richiesta_Iniziale_Gasolio_Serra,0) ELSE isnull(qp.Richiesta_Iniziale_Gasolio_Serra,0) END As Richiesto_Gasolio_Serra, ")
            stb.AppendLine("           Rendicontato_Gasolio,  ")
            stb.AppendLine("           Rendicontato_Benzina,  ")
            stb.AppendLine("           Rendicontato_Gasolio_Serra, ")
            stb.AppendLine("           isnull(cgr.Carburante_Approvato,0) As Gasolio_Approvato_Allevamenti, ")
            stb.AppendLine("           ISNULL(cbr.Carburante_Approvato, 0) As Benzina_Approvato_Allevamenti, ")
            stb.AppendLine("           Case When qp.Tipo_Richiesta = -1 THEN isnull(qp.Approvazione_Iniziale_Gasolio_Terzisti,0) + isnull(qp.Approvazione_Iniziale_Gasolio,0) ELSE isnull(qp.Approvazione_Iniziale_Gasolio,0) END As Approvato_Gasolio_Coltura, ")
            stb.AppendLine("           Case When qp.Tipo_Richiesta = -1 THEN isnull(qp.Approvazione_Iniziale_Benzina_Terzisti,0) + isnull(qp.Approvazione_Iniziale_Benzina,0) ELSE isnull(qp.Approvazione_Iniziale_Benzina,0) END As Approvato_Benzina_Coltura, ")
            stb.AppendLine("           Case When qp.Tipo_Richiesta = -1 THEN isnull(qp.Approvazione_Iniziale_Gasolio_Serra_Terzisti,0) + isnull(qp.Approvazione_Iniziale_Gasolio_Serra,0) ELSE isnull(qp.Approvazione_Iniziale_Gasolio_Serra,0) END As Richiesto_Approvato_Gasolio_Serra, ")
            stb.AppendLine("           Rendicontato_Approvato_Gasolio,  ")
            stb.AppendLine("           Rendicontato_Approvato_Benzina,  ")
            stb.AppendLine("           Rendicontato_Approvato_Gasolio_Serra, ")
            stb.AppendLine("           Rimanenza_Iniziale_Gasolio,  ")
            stb.AppendLine("           Rimanenza_Iniziale_Benzina, ")
            stb.AppendLine("           Rimanenza_Iniziale_Gasolio_Serra, ")
            stb.AppendLine("           Rimanenza_Finale_Gasolio, ")
            stb.AppendLine("           Rimanenza_Finale_Benzina, ")
            stb.AppendLine("           Rimanenza_Finale_Gasolio_Serra, ")
            stb.AppendLine("           Conto, ")
            stb.AppendLine("           Acquistato_Gasolio, ")
            stb.AppendLine("           Acquistato_Benzina,   ")
            stb.AppendLine("           Acquistato_Gasolio_Serra, ")
            stb.AppendLine("           Anno ")
            stb.AppendLine("          FROM(    ")

            'Query più interna 
            stb.AppendLine("  Select t.Anno, i.Rag_Soc, i.CUAA, t.piva, t.Tipo_Richiesta, ")

            'Richiesta iniziale Gasolio
            stb.AppendLine("  SUM(tsl.Richiesta_Iniziale_Gasolio) as Richiesta_Iniziale_Gasolio_Terzisti, ")
            stb.AppendLine("  ( ")
            stb.AppendLine("    SELECT f.fabbisogno_Richiesto from Lav_Avanzamento_0_Gasolio_CTE f where f.piva = t.Piva AND f.tipo_Richiesta = t.tipo_Richiesta ")
            stb.AppendLine("  ) as Richiesta_Iniziale_Gasolio, ")

            'Richiesta iniziale Benzina
            stb.AppendLine("  SUM(tsl.Richiesta_Iniziale_Benzina) as Richiesta_Iniziale_Benzina_Terzisti, ")
            stb.AppendLine("  ( ")
            stb.AppendLine("    SELECT f.fabbisogno_Richiesto from Lav_Avanzamento_0_Benzina_CTE f where f.piva = t.Piva AND f.tipo_Richiesta = t.tipo_Richiesta ")
            stb.AppendLine("  ) as Richiesta_Iniziale_Benzina, ")

            'Richiesta iniziale Gasolio Serra
            stb.AppendLine("  SUM(tsl.Richiesta_Iniziale_Gasolio_Serra ) as Richiesta_Iniziale_Gasolio_Serra_Terzisti, ")
            stb.AppendLine("  ( ")
            stb.AppendLine("    SELECT f.fabbisogno_Richiesto from Lav_Avanzamento_0_GasolioSerra_CTE f where f.piva = t.Piva AND f.tipo_Richiesta = t.tipo_Richiesta ")
            stb.AppendLine("  ) as Richiesta_Iniziale_Gasolio_Serra, ")

            'Carburanti rendicontati
            stb.AppendLine("  ISNULL( (SELECT f.fabbisogno_Richiesto from Lav_Avanzamento_1_Gasolio_CTE f where f.piva = t.Piva AND f.tipo_Richiesta = t.tipo_Richiesta), 0 ) AS Rendicontato_Gasolio,  ")
            stb.AppendLine("  ISNULL( (SELECT f.fabbisogno_Richiesto from Lav_Avanzamento_1_Benzina_CTE f where f.piva = t.Piva AND f.tipo_Richiesta = t.tipo_Richiesta), 0 ) AS Rendicontato_Benzina,  ")
            stb.AppendLine("  ISNULL( (SELECT f.fabbisogno_Richiesto from Lav_Avanzamento_1_GasolioSerra_CTE f where f.piva = t.Piva AND f.tipo_Richiesta = t.tipo_Richiesta), 0 ) AS Rendicontato_Gasolio_Serra,  ")

            'Gasolio Approvato in Richiesta
            stb.AppendLine(" SUM(tsl.Approvazione_Iniziale_Gasolio) as Approvazione_Iniziale_Gasolio_Terzisti, ")
            stb.AppendLine(" ( ")
            stb.AppendLine("   SELECT f.Fabbisogno_Assegnato from Lav_Avanzamento_0_Gasolio_CTE f where f.piva = t.Piva AND f.tipo_Richiesta = t.tipo_Richiesta ")
            stb.AppendLine(" ) as Approvazione_Iniziale_Gasolio, ")

            'Benzina Approvata in Richiesta
            stb.AppendLine(" SUM(tsl.Approvazione_Iniziale_Benzina) as Approvazione_Iniziale_Benzina_Terzisti, ")
            stb.AppendLine(" ( ")
            stb.AppendLine("   SELECT f.Fabbisogno_Assegnato from Lav_Avanzamento_0_Benzina_CTE f where f.piva = t.Piva AND f.tipo_Richiesta = t.tipo_Richiesta ")
            stb.AppendLine(" ) as Approvazione_Iniziale_Benzina, ")

            'Gasolio Serra Approvato in Richiesta
            stb.AppendLine(" SUM(tsl.Approvazione_Iniziale_Gasolio_Serra) as Approvazione_Iniziale_Gasolio_Serra_Terzisti, ")
            stb.AppendLine(" ( ")
            stb.AppendLine("   SELECT f.Fabbisogno_Assegnato from Lav_Avanzamento_0_GasolioSerra_CTE f where f.piva = t.Piva AND f.tipo_Richiesta = t.tipo_Richiesta ")
            stb.AppendLine(" ) as Approvazione_Iniziale_Gasolio_Serra, ")

            'Carburanti rendicontati approvati
            stb.AppendLine("   isnull( (SELECT f.Fabbisogno_Assegnato from Lav_Avanzamento_1_Gasolio_CTE f where f.piva = t.Piva AND f.tipo_Richiesta = t.tipo_Richiesta), 0) as Rendicontato_Approvato_Gasolio,  ")
            stb.AppendLine("   isnull( (SELECT f.Fabbisogno_Assegnato from Lav_Avanzamento_1_Benzina_CTE f where f.piva = t.Piva AND f.tipo_Richiesta = t.tipo_Richiesta), 0) as Rendicontato_Approvato_Benzina,  ")
            stb.AppendLine("   isnull( (SELECT f.Fabbisogno_Assegnato from Lav_Avanzamento_1_GasolioSerra_CTE f where f.piva = t.Piva AND f.tipo_Richiesta = t.tipo_Richiesta), 0) as Rendicontato_Approvato_Gasolio_Serra,  ")

            'Varburanti acquistati
            stb.AppendLine("   isnull((SELECT SUM(v.Lt) from UMA_Vendite v (nolock)   ")
            stb.AppendLine("        WHERE v.PIVA_Cliente = t.Piva AND v.Anno = t.anno AND v.Conto_Proprio_Terzi = t.tipo_richiesta AND v.tipo_Carburante = 2), 0) As Acquistato_Gasolio,   ")

            stb.AppendLine("   isnull((SELECT SUM(v.Lt) from UMA_Vendite v (nolock)   ")
            stb.AppendLine("        WHERE v.PIVA_Cliente = t.Piva AND v.Anno = t.anno AND v.Conto_Proprio_Terzi = t.tipo_richiesta AND v.tipo_Carburante = 3), 0) As Acquistato_Benzina,   ")

            stb.AppendLine("   isnull((SELECT SUM(v.Lt) from UMA_Vendite v (nolock)   ")
            stb.AppendLine("        WHERE v.PIVA_Cliente = t.Piva AND v.Anno = t.anno AND v.Conto_Proprio_Terzi = t.tipo_richiesta AND v.tipo_Carburante = 8), 0) As Acquistato_Gasolio_Serra,  ")

            'Rimanenze Iniziali
            stb.AppendLine(" ( ")
            stb.AppendLine("   MAX(t.Rimanenza_Gasolio) ")
            stb.AppendLine(" ) As Rimanenza_Iniziale_Gasolio,  ")
            stb.AppendLine(" ( ")
            stb.AppendLine("   MAX(t.Rimanenza_Benzina) ")
            stb.AppendLine(" ) As Rimanenza_Iniziale_Benzina,  ")
            stb.AppendLine(" ( ")
            stb.AppendLine("   MAX(t.Rimanenza_Gasolio_Serra) ")
            stb.AppendLine(" ) As Rimanenza_Iniziale_Gasolio_Serra,  ")

            'Rimanenze Finali
            stb.AppendLine(" ( ")
            stb.AppendLine("   SELECT isnull(MAX(f.Rimanenza_Gasolio),0) from Lav_Avanzamento_1_Gasolio_CTE f where f.piva = t.Piva AND f.tipo_Richiesta = t.tipo_Richiesta ")
            stb.AppendLine(" ) As Rimanenza_Finale_Gasolio,  ")
            stb.AppendLine(" ( ")
            stb.AppendLine("   SELECT isnull(MAX(f.Rimanenza_Benzina),0) from Lav_Avanzamento_1_Benzina_CTE f where f.piva = t.Piva AND f.tipo_Richiesta = t.tipo_Richiesta ")
            stb.AppendLine(" ) As Rimanenza_Finale_Benzina,  ")
            stb.AppendLine(" ( ")
            stb.AppendLine("   SELECT isnull(MAX(f.Rimanenza_Gasolio_Serra),0) from Lav_Avanzamento_1_GasolioSerra_CTE f where f.piva = t.Piva AND f.tipo_Richiesta = t.tipo_Richiesta ")
            stb.AppendLine(" ) As Rimanenza_Finale_Gasolio_Serra,  ")

            'Conto Proprio/Terzi
            stb.AppendLine(" Case When t.Tipo_Richiesta = 0 Then 'Conto Proprio' else 'Conto Terzi' END As Conto ")

            stb.AppendLine(" FROM Testa_CTE t ")
            'stb.AppendLine(" LEFT JOIN Lav_Avanzamento_0_Gasolio_CTE ul_gas ON t.piva = ul_gas.piva AND t.Tipo_Richiesta = ul_gas.Tipo_Richiesta  ")
            'stb.AppendLine(" LEFT JOIN Lav_Avanzamento_0_Benzina_CTE ul_benz ON t.piva = ul_benz.piva AND t.Tipo_Richiesta = ul_benz.Tipo_Richiesta  ")
            'stb.AppendLine(" LEFT JOIN Lav_Avanzamento_0_GasolioSerra_CTE ul_serra ON t.piva = ul_serra.piva AND t.Tipo_Richiesta = ul_serra.Tipo_Richiesta ")
            'stb.AppendLine(" LEFT JOIN Lav_Avanzamento_1_Gasolio_CTE ula_gas on T.Piva = ula_gas.Piva and t.Tipo_Richiesta = ula_gas.Tipo_Richiesta ")
            'stb.AppendLine(" LEFT JOIN Lav_Avanzamento_1_Benzina_CTE ula_benz on T.Piva = ula_benz.Piva and t.Tipo_Richiesta = ula_benz.Tipo_Richiesta ")
            'stb.AppendLine(" LEFT JOIN Lav_Avanzamento_1_GasolioSerra_CTE ula_serra on T.Piva = ula_serra.Piva and t.Tipo_Richiesta = ula_serra.Tipo_Richiesta ")
            stb.AppendLine(" JOIN Imprese_CTE i ON i.PIVA = t.Piva  ")
            stb.AppendLine(" LEFT JOIN Terzisti_Senza_Lav tsl on tsl.Piva = t.Piva ")
            stb.AppendLine(" join Indirizzi_CTE ind on ind.PIVA = t.Piva ")

            If filtroVisibilita Then
                stb.AppendLine("     Left Join Utenti_Visibilita_Appoggio (nolock) On t.Piva = Utenti_Visibilita_Appoggio.Piva  ")
                stb.AppendLine("     And Utenti_Visibilita_Appoggio.Entita_Cod=1  ")
            End If

            If FiltroNuovaVisibilita Then
                If Not VisibilitaTotale Then
                    If FiltroUtente And Not FiltroGruppoUtente Then
                        stb.AppendLine(" LEFT JOIN " & objParametri_Utenti.Recupera_NomeDB & ".dbo.Utenti_Visibilita visibilita ON t.Piva = visibilita.Piva_Azienda AND visibilita.Username = '" & objParametri_Utenti.UtenteUsername & "' ")
                    End If
                    If Not FiltroUtente And FiltroGruppoUtente Then
                        stb.AppendLine(" LEFT JOIN " & objParametri_Utenti.Recupera_NomeDB & ".dbo.Utenti_Visibilita visibilita ON t.Piva = visibilita.Piva_Azienda AND visibilita.Gruppo = " & GruppoUtente & " ")
                    End If
                End If
            End If

            stb.AppendLine("    where  1=1 ")
            If filtroVisibilita Then
                stb.AppendLine("     And Utenti_Visibilita_Appoggio.Username = " & Agro_SQL_SaveText_NULL(objParametri_Server.UtenteUsername) & "  ")
            End If

            If FiltroNuovaVisibilita And Not VisibilitaTotale Then
                stb.Append(" AND (NOT visibilita.Piva_Azienda IS NULL) ")
            End If

            If piveVisibili.Length > 0 Then
                stb.AppendLine(" AND t.Piva IN (" & Agro_SQL_Save_Clausola_IN("'" & piveVisibili & "'", True) & ")  ")
            End If

            If (piva <> "") Then
                stb.AppendLine(" AND t.piva = '" + Agro_SQL_SaveText(piva) + "' ")
            End If

            If (conto < 1) Then
                stb.AppendLine(" AND t.tipo_richiesta = " + conto.ToString + " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND  " + Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo) + " ")
            End If

            stb.AppendLine(" GROUP BY Anno, i.Rag_Soc, i.CUAA, t.piva, t.tipo_richiesta ) As qp ")
            stb.AppendLine(" LEFT JOIN Carburante_Gasolio_Allevamenti_CTE cgr (nolock) ON qp.piva = cgr.piva AND qp.tipo_richiesta = cgr.Tipo_Richiesta ")
            stb.AppendLine(" LEFT JOIN Carburante_Benzina_Allevamenti_CTE cbr (nolock) ON qp.piva = cbr.piva AND qp.tipo_richiesta = cbr.Tipo_Richiesta ) as qs ")
            stb.AppendLine(" ) as qf ")

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " + Agro_SQL_Save_xOrderBy(xOrderBy) + " ")
            Else
                stb.AppendLine(" order by qf.Rag_Soc ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Server, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    Public Function Leggi_Schema_Template(ByVal Id_Schema_Template As Integer,
                                          ByVal xFiltroAggiuntivo As String,
                                          ByVal xOrderBy As String,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable


        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_Richieste_Testata_R.Leggi_Schema_Template()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT * From Schema_Documenti_Template ")
            stb.AppendLine(" WHERE 1 = 1 ")

            If (Id_Schema_Template <> 0) Then
                stb.AppendLine(" AND Id_Schema_Template = " & Id_Schema_Template & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND  " + Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo) + " ")
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " + Agro_SQL_Save_xOrderBy(xOrderBy) + " ")
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

    Public Function CheckValiditaLavorazioniDaPratica(ByVal pratica_cod As Integer,
                                                      ByVal xFiltroAggiuntivo As String,
                                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_Richieste_Testata_R.CheckValiditaLavorazioniDaPratica()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT i.rag_soc, um.Macrouso_UMA_Des, ul.Lav_UMA_Des, l.* ")
            stb.AppendLine(" FROM Pratiche p ")
            stb.AppendLine(" join UMA_Richieste_Testata t on t.Pratica_Cod = p.Pratica_Cod")
            stb.AppendLine(" join UMA_Richieste_Lavorazioni l on l.Richiesta_Cod = t.Richiesta_Cod")
            stb.AppendLine(" join Imprese i on i.PIVA = l.Piva ")
            stb.AppendLine(" join UMA_Macrousi um on um.Macrouso_UMA_Cod = l.Gruppo_Colturale_UMA ")
            stb.AppendLine(" join UMA_Lavorazioni ul on ul.Lav_UMA_Cod = l.Lavorazione_UMA ")
            stb.AppendLine(" join UMA_Configurazione_MacrousixLavorazioni ml on l.Lavorazione_UMA = ml.Lav_UMA_Cod AND l.Gruppo_Colturale_UMA = ml.Macrouso_UMA_Cod")
            stb.AppendLine(" Where p.pratica_cod = " + pratica_cod.ToString + " AND t.Avanzamento_Richiesta = 0 AND NOT (GETDATE() BETWEEN ml.Validita_Inizio AND ml.Validita_Fine)")
            stb.AppendLine(" UNION")
            stb.AppendLine(" SELECT i.rag_soc, um.Macrouso_UMA_Des, ul.Lav_UMA_Des, l.* ")
            stb.AppendLine(" FROM Pratiche p ")
            stb.AppendLine(" join UMA_Richieste_Testata t on t.Pratica_Cod = p.Pratica_Cod")
            stb.AppendLine(" join UMA_Richieste_Lavorazioni l on l.Richiesta_Cod = t.Richiesta_Cod")
            stb.AppendLine(" join Imprese i on i.PIVA = l.Piva ")
            stb.AppendLine(" join UMA_Macrousi um on um.Macrouso_UMA_Cod = l.Gruppo_Colturale_UMA ")
            stb.AppendLine(" join UMA_Lavorazioni ul on ul.Lav_UMA_Cod = l.Lavorazione_UMA ")
            stb.AppendLine(" join UMA_Configurazione_MacrousixLavorazioni ml on l.Lavorazione_UMA = ml.Lav_UMA_Cod AND l.Gruppo_Colturale_UMA = ml.Macrouso_UMA_Cod")
            stb.AppendLine(" Where p.pratica_cod = " + pratica_cod.ToString + " AND t.Avanzamento_Richiesta = 1 AND NOT (l.Validita_Inizio BETWEEN ml.Validita_Inizio AND ml.Validita_Fine)")

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND  " + Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo) + " ")
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

    Public Function Leggi_Elenco_Con_Indirizzi(ByVal piva As String,
                                               ByVal anno As Integer,
                                               ByVal xFiltroAggiuntivo As String,
                                               ByVal xOrderBy As String,
                                               ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                               ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                               ByVal Filtro_Visibilita_Utente As Boolean,
                                               ByVal piveVisibili As String,
                                               ByVal rendicontazioni As Boolean,
                                               ByVal statoCod As Integer,
                                               ByVal citta As String,
                                               ByVal prov As String,
                                               ByVal conto As Integer,
                                               ByVal nuovaVisibilita As Boolean,
                                               ByVal FiltroUtente As Boolean,
                                               ByVal FiltroGruppoUtente As Boolean,
                                               ByVal GruppoUtente As Integer,
                                               ByVal VisibilitaTotale As Boolean
                                               ) As DataTable


        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_Richieste_Testata_R.Leggi_Elenco()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        'Anna 12/08/21: Aggiunte due nuove colonne per UMA che necessitano JOIN con DB Utenti
        Dim NomeDB_Utenti As String = objParametri_Utenti.StringaConnessione.Split(";")(2)
        NomeDB_Utenti = NomeDB_Utenti.Split("=")(1)

        Try

            stb.Length = 0
            'Creazione tabella temporanea
            'stb.AppendLine(" If(OBJECT_ID('tempdb..#TempLav') Is Not Null) ")
            'stb.AppendLine(" Begin ")
            'stb.AppendLine(" Drop Table #TempLav ")
            'stb.AppendLine(" End ")

            'stb.AppendLine(" create table #TempLav  ")
            'stb.AppendLine("( ")
            'stb.AppendLine("        Richiesta_Cod int, ")
            'stb.AppendLine("        Tipo_Carburante int, ")
            'stb.AppendLine("        Fabbisogno_Calcolato float, ")
            'stb.AppendLine("        Fabbisogno_Richiesto float, ")
            'stb.AppendLine("       Fabbisogno_Assegnato float ")
            'stb.AppendLine(" ) ")

            stb.AppendLine(" with #TempLav as ( ")
            stb.AppendLine(" SELECT Richiesta_Cod, Tipo_Carburante, SUM(Fabbisogno_Calcolato) as Fabbisogno_Calcolato, SUM(Fabbisogno_Richiesto) as Fabbisogno_Richiesto, SUM(Fabbisogno_Assegnato) as Fabbisogno_Assegnato ")
            stb.AppendLine(" From UMA_Richieste_Lavorazioni ")
            stb.AppendLine(" GROUP BY Richiesta_Cod, Tipo_Carburante ), ")

            'stb.AppendLine(" If(OBJECT_ID('tempdb..#TempAllevamenti') Is Not Null)  ")
            'stb.AppendLine("  Begin  ")
            'stb.AppendLine("  Drop Table #TempAllevamenti  ")
            'stb.AppendLine("  End  ")
            'stb.AppendLine("  create table #TempAllevamenti   ")
            'stb.AppendLine("(  ")
            'stb.AppendLine("         Richiesta_Cod int,  ")
            'stb.AppendLine("         Tipo_Carburante int,  ")
            'stb.AppendLine("         Carburante_Calcolato float,  ")
            'stb.AppendLine("         Carburante_Richiesto float,  ")
            'stb.AppendLine("         Carburante_Approvato float ")
            'stb.AppendLine("  )  ")
            stb.AppendLine("  #TempAllevamenti  as( ")
            stb.AppendLine("  SELECT Richiesta_Cod, Tipo_Carburante, SUM(Carburante_Richiesto) as Carburante_Richiesto, SUM(Carburante_Calcolato) as Carburante_Calcolato, SUM(Carburante_Approvato) as Carburante_Approvato ")
            stb.AppendLine("  From UMA_Richieste_Allevamenti ")
            stb.AppendLine("  GROUP BY Richiesta_Cod, Tipo_Carburante ), ")

            stb.AppendLine("  #TempVerificaInCorso  as( ")
            stb.AppendLine("  SELECT Pratiche.Pratica_Cod,  ")
            stb.AppendLine("  (SELECT TOP 1 Username_Creazione FROM Pratiche_Stati psa WHERE psa.Pratica_Cod = Pratiche.Pratica_Cod AND psa.Stato_Cod = 2002 ORDER BY Data_Creazione DESC) as Username_Creazione ")
            stb.AppendLine("  FROM Pratiche  ")
            stb.AppendLine("  WHERE pratiche.Servizio_Cod = 2007 AND Pratiche.Anno = '" & Agro_SQL_SaveText(anno) & "' ) ")

            If (rendicontazioni) Then
                stb.AppendLine(", #TempRimanenze as( ")
                stb.AppendLine(" Select SUM(Rimanenza_Gasolio) as Rimanenza_Gasolio, SUM(Rimanenza_Benzina) as Rimanenza_Benzina, SUM(Rimanenza_Gasolio_Serra) as Rimanenza_Gasolio_Serra, UMA_Richieste_Testata.Piva, Tipo_Richiesta  ")
                stb.AppendLine(" From UMA_Richieste_Testata ")
                stb.AppendLine(" JOIN Pratiche ON UMA_Richieste_Testata.Pratica_Cod = Pratiche.Pratica_Cod ")
                stb.AppendLine(" where Avanzamento_Richiesta = 0 AND Pratiche.Anno = '" & Agro_SQL_SaveText(anno) & "'  ")
                stb.AppendLine(" GROUP BY UMA_Richieste_Testata.Piva, Tipo_Richiesta) ")
            End If

            stb.AppendLine(" SELECT DISTINCT x.* FROM ( ")
            stb.AppendLine(" SELECT t.Piva AS ID, t.Piva as piva, imp.rag_soc As azienda, ic.val_cod AS CUAA, p.Numero AS nRichiesta, p.Pratica_Cod, p.Anno AS annoRichiesta, psa.stato_cod AS CodstatoAv, psaw.WAnagraficaStati_Des AS statoAv, ")
            'stb.AppendLine(" ISNULL(u.[USER], '') As Ispettore, ")
            stb.AppendLine(" ISTAT.Localita AS citta, i.ind_des AS via, i.CAP, i.pro_cod AS Prov, t.Data_Creazione  ")
            stb.AppendLine(" ,psa.Validita_Inizio As ultimo_Avanzamento, t.Validita_Inizio AS val_Inizio, t.Validita_Fine AS val_Fine, t.richiesta_Cod AS richiestaCod, ")
            'stb.AppendLine(" ,t.Carburante_Calcolato AS calcolato, t.Carburante_Richiesto AS richiesto, t.Carburante_Approvato AS assegnato, IIf(t.Tipo_Richiesta = 0, 'Conto Proprio', 'Conto Terzi') AS tipo_richiesta ")

            stb.AppendLine(" ISNULL(ul_gas.Fabbisogno_Calcolato, 0) + ISNULL(uag.Carburante_Calcolato, 0) as calcolato_Gasolio, ")
            stb.AppendLine(" ISNULL(ul_benz.Fabbisogno_Calcolato, 0) + ISNULL(uab.Carburante_Calcolato, 0) as calcolato_Benzina, ")
            stb.AppendLine(" ISNULL(ul_serra.Fabbisogno_Calcolato, 0) as calcolato_Gasolio_Serra, ")
            stb.AppendLine(" ISNULL(CASE ")
            stb.AppendLine("       WHEN t.Tipo_Richiesta = -1 AND t.Avanzamento_Richiesta = 0 AND t.Richiesta_Iniziale_Gasolio > 0 THEN t.Richiesta_Iniziale_Gasolio ")
            stb.AppendLine("       ELSE ul_gas.Fabbisogno_Richiesto ")
            stb.AppendLine(" END, 0) + ISNULL(uag.Carburante_Richiesto, 0) as richiesto_Gasolio, ")
            stb.AppendLine(" ISNULL(CASE ")
            stb.AppendLine("        WHEN t.Tipo_Richiesta = -1 AND t.Avanzamento_Richiesta = 0 AND t.Richiesta_Iniziale_Benzina > 0 THEN t.Richiesta_Iniziale_Benzina ")
            stb.AppendLine("        ELSE ul_benz.Fabbisogno_Richiesto ")
            stb.AppendLine(" END, 0) + ISNULL(uab.Carburante_Richiesto, 0) as richiesto_Benzina, ")
            stb.AppendLine(" ISNULL(CASE ")
            stb.AppendLine("        WHEN t.Tipo_Richiesta = -1 AND t.Avanzamento_Richiesta = 0 AND t.Richiesta_Iniziale_Gasolio_Serra > 0 THEN t.Richiesta_Iniziale_Gasolio_Serra ")
            stb.AppendLine(" ELSE ul_serra.Fabbisogno_Richiesto ")
            stb.AppendLine(" END, 0) as richiesto_Gasolio_Serra, ")

            stb.AppendLine(" ISNULL(CASE ")
            stb.AppendLine("        WHEN t.Tipo_Richiesta = -1 AND t.Avanzamento_Richiesta = 0 AND t.Approvazione_Iniziale_Gasolio > 0 THEN t.Approvazione_Iniziale_Gasolio ")
            stb.AppendLine("        ELSE ul_gas.Fabbisogno_Assegnato ")
            stb.AppendLine(" END, 0) + ISNULL(uag.Carburante_Approvato, 0) as assegnato_gasolio, ")
            stb.AppendLine(" ISNULL(CASE ")
            stb.AppendLine("        WHEN t.Tipo_Richiesta = -1 And t.Avanzamento_Richiesta = 0 AND t.Approvazione_Iniziale_Benzina > 0 Then t.Approvazione_Iniziale_Benzina ")
            stb.AppendLine("        Else ul_benz.Fabbisogno_Assegnato ")
            stb.AppendLine(" End, 0) + ISNULL(uab.Carburante_Approvato, 0) As assegnato_benzina, ")
            stb.AppendLine(" ISNULL(Case ")
            stb.AppendLine("       When t.Tipo_Richiesta = -1 And t.Avanzamento_Richiesta = 0 AND t.Approvazione_Iniziale_Gasolio_Serra > 0 Then t.Approvazione_Iniziale_Gasolio_Serra ")
            stb.AppendLine("        Else ul_serra.Fabbisogno_Assegnato  ")
            stb.AppendLine(" End, 0) As assegnato_gasolio_serra, ")
            stb.AppendLine(" ISNULL(uag.Carburante_Calcolato, 0) as Carburante_Calcolato_Gasolio_Allevamenti, ")
            stb.AppendLine(" ISNULL(uag.Carburante_Richiesto, 0) as Carburante_Richiesto_Gasolio_Allevamenti, ")
            stb.AppendLine(" ISNULL(uag.Carburante_Approvato, 0) as Carburante_Approvato_Gasolio_Allevamenti, ")
            stb.AppendLine(" ISNULL(uab.Carburante_Calcolato, 0) as Carburante_Calcolato_Benzina_Allevamenti, ")
            stb.AppendLine(" ISNULL(uab.Carburante_Richiesto, 0) as Carburante_Richiesto_Benzina_Allevamenti, ")
            stb.AppendLine(" ISNULL(uab.Carburante_Approvato, 0) as Carburante_Approvato_Benzina_Allevamenti, ")
            stb.AppendLine("                            IIf(t.Tipo_Richiesta = 0, 'Conto Proprio', 'Conto Terzi') AS tipo_richiesta,  ")
            stb.AppendLine("                            IIf(t.Richiesta_Integrativa = 1, 'Richiesta Integrativa', 'Prima Richiesta' ) AS Prima_Richiesta,  ")
            'Anna 12/08/21: Aggiunte due nuove colonne per UMA
            stb.AppendLine(" CASE WHEN richiedente.Nome = '' AND Richiedente.cognome = ''  ")
            stb.AppendLine("    THEN (t.username_creazione +' [' + richiedente.Rag_Soc + ']') ")
            stb.AppendLine(" ELSE (t.username_creazione +' [' + richiedente.Nome + ' ' + Richiedente.cognome +']') ")
            stb.AppendLine(" END AS Richiedente,  ")

            stb.AppendLine(" CASE WHEN psa.Stato_Cod BETWEEN 2003 AND 2006 THEN ")
            stb.AppendLine(" CASE WHEN Approvatoret.UserName IS NOT NULL THEN (Approvatoret.UserName +' [' + Approvatoret.Nome + ' ' + Approvatoret.cognome +']')  ")
            stb.AppendLine(" ELSE (Approvatorep.UserName +' [' + Approvatorep.Nome + ' ' + Approvatorep.cognome +']') END  ")
            stb.AppendLine(" END AS Approvatore ")

            If (rendicontazioni) Then
                'stb.AppendLine(", Case WHEN ROUND((((ISNULL(ul_gas.Fabbisogno_Richiesto, 0) + ISNULL(ul_benz.Fabbisogno_Richiesto, 0) + ")
                'stb.AppendLine(" ISNULL(ul_serra.Fabbisogno_Richiesto, 0) + ISNULL(uag.Carburante_Richiesto, 0) + ISNULL(uab.Carburante_Richiesto, 0)) * ((100 - uset.Per_Riduzione) / 100))), 0) >  ")
                'stb.AppendLine(" ISNULL(ISNULL((Select SUM(uv.lt) From UMA_Vendite uv where uv.PIVA_Cliente = t.Piva And uv.Conto_Proprio_Terzi = t.Tipo_Richiesta AND uv.Anno = " + Agro_SQL_SaveNum(anno) + "), 0) + (Select tr.Rimanenza_Gasolio + tr.Rimanenza_Benzina + tr.Rimanenza_Gasolio_Serra From #TempRimanenze tr where tr.Piva = t.Piva And tr.Tipo_Richiesta = t.Tipo_Richiesta) - ( t.Rimanenza_Gasolio + t.Rimanenza_Benzina + t.Rimanenza_Gasolio_Serra ), 0) Then  ")
                'stb.AppendLine(" 'SI' ")
                'stb.AppendLine(" Else 'NO' ")
                'stb.AppendLine(" End As 'Litri_In_Esubero', ")

                stb.AppendLine(" , '' As 'Litri_In_Esubero', ")

                stb.AppendLine(" ROUND(ISNULL((Select SUM(uv.lt) From UMA_Vendite uv where uv.PIVA_Cliente = t.Piva AND uv.Tipo_Carburante = 2 AND uv.Conto_Proprio_Terzi = t.Tipo_Richiesta AND uv.Anno = " + Agro_SQL_SaveNum(anno) + "), 0) + ")
                stb.AppendLine(" (Select tr.Rimanenza_Gasolio From #TempRimanenze tr where tr.Piva = t.Piva And tr.Tipo_Richiesta = t.Tipo_Richiesta) - ")
                stb.AppendLine(" (t.Rimanenza_Gasolio), 0) as Acquistato_e_Rimanenza_Gasolio, ")
                stb.AppendLine(" ROUND(ISNULL((Select SUM(uv.lt) From UMA_Vendite uv where uv.PIVA_Cliente = t.Piva AND uv.Tipo_Carburante = 3 AND uv.Conto_Proprio_Terzi = t.Tipo_Richiesta AND uv.Anno = " + Agro_SQL_SaveNum(anno) + "), 0) + ")
                stb.AppendLine(" (Select tr.Rimanenza_Benzina From #TempRimanenze tr where tr.Piva = t.Piva And tr.Tipo_Richiesta = t.Tipo_Richiesta) - ")
                stb.AppendLine(" (t.Rimanenza_Benzina), 0) as Acquistato_e_Rimanenza_Benzina, ")
                stb.AppendLine(" ROUND(ISNULL((Select SUM(uv.lt) From UMA_Vendite uv where uv.PIVA_Cliente = t.Piva AND uv.Tipo_Carburante = 8 AND uv.Conto_Proprio_Terzi = t.Tipo_Richiesta AND uv.Anno = " + Agro_SQL_SaveNum(anno) + "), 0) + ")
                stb.AppendLine(" (Select tr.Rimanenza_Gasolio_Serra From #TempRimanenze tr where tr.Piva = t.Piva And tr.Tipo_Richiesta = t.Tipo_Richiesta) - ")
                stb.AppendLine(" (t.Rimanenza_Gasolio_Serra), 0) as Acquistato_e_Rimanenza_Gasolio_Serra, ")
                stb.AppendLine(" ROUND(((ISNULL((ISNULL(ul_gas.Fabbisogno_Richiesto, 0) + ISNULL(uag.Carburante_Richiesto, 0)) * ((100 - uset.Per_Riduzione) / 100), 0))), 0) AS Richiesto_Netto_Gasolio,  ")
                stb.AppendLine(" ROUND(((ISNULL((ISNULL(ul_benz.Fabbisogno_Richiesto, 0) + ISNULL(uab.Carburante_Richiesto, 0)) * ((100 - uset.Per_Riduzione) / 100), 0))), 0) AS Richiesto_Netto_Benzina,  ")
                stb.AppendLine(" ROUND(((ISNULL((ISNULL(ul_serra.Fabbisogno_Richiesto, 0)) * ((100 - uset.Per_Riduzione) / 100), 0))), 0) AS Richiesto_Netto_Gasolio_Serra  ")
            Else
                stb.AppendLine(", ISNULL(pOrigin.Numero, '') as Rendicontazine_Originale  ")
            End If

            'stb.AppendLine(" case when t.richiesta_cod IN ( ")
            'stb.AppendLine("    SELECT top(1) tt.Richiesta_Cod ")
            'stb.AppendLine("    FROM UMA_Richieste_Testata tt ")
            'stb.AppendLine("    JOIN Pratiche ON tt.Pratica_Cod = Pratiche.Pratica_Cod ")
            'stb.AppendLine("    JOIN Pratiche_Stati_Attuali On Pratiche_Stati_Attuali.Pratica_Cod = Pratiche.Pratica_Cod ")
            'stb.AppendLine("    WHERE tt.Piva_SuperUser = t.Piva_SuperUser And tt.piva = t.piva And Pratiche.Anno = p.Anno ")
            'stb.AppendLine("    And tt.Tipo_Richiesta = t.Tipo_Richiesta And tt.Avanzamento_Richiesta = t.Avanzamento_Richiesta ")
            'stb.AppendLine("    order by numero ASC, tt.data_Creazione ASC ) ")
            'stb.AppendLine(" Then 'Prima Richiesta' else 'Richiesta Integrativa' END As Prima_Richiesta ")
            stb.AppendLine(" FROM UMA_Richieste_Testata t ")
            stb.AppendLine(" Left Join #TempLav ul_gas ON t.Richiesta_Cod = ul_gas.Richiesta_Cod And ul_gas.Tipo_Carburante = 2 ")
            stb.AppendLine(" Left Join #TempLav ul_benz ON t.Richiesta_Cod = ul_benz.Richiesta_Cod And ul_benz.Tipo_Carburante = 3 ")
            stb.AppendLine(" Left Join #TempLav ul_serra ON t.Richiesta_Cod = ul_serra.Richiesta_Cod And ul_serra.Tipo_Carburante = 8 ")
            stb.AppendLine(" Left Join #TempAllevamenti uag ON t.Richiesta_Cod = uag.Richiesta_Cod AND uag.Tipo_Carburante = 2 ")
            stb.AppendLine(" Left Join #TempAllevamenti uab ON t.Richiesta_Cod = uab.Richiesta_Cod And uab.Tipo_Carburante = 3 ")
            stb.AppendLine(" join Imprese imp on imp.Piva = t.Piva ")
            stb.AppendLine(" join Pratiche p on t.Piva = p.Piva And t.Pratica_Cod = p.Pratica_Cod ")
            stb.AppendLine(" left join Pratiche_Stati ps on t.Pratica_Cod = ps.Pratica_Cod AND ps.stato_Cod BETWEEN 2003 AND 2006 ")
            'stb.AppendLine(" left join Utenti u on u.CODICE_FISCALE = ps.Username_Modifica ")
            stb.AppendLine(" join Pratiche_Stati_Attuali psa on p.Piva_SuperUser = psa.Piva_SuperUser And p.Pratica_Cod = psa.Pratica_Cod ")
            stb.AppendLine(" join WAnagraficaStati psaw on psa.Stato_Cod = psaw.WAnagraficaStati_Cod ")
            stb.AppendLine(" join Imprese_Codici ic on t.Piva = ic.PIVA And ic.id_cod = 1010 ")
            stb.AppendLine(" INNER JOIN ImpresexIndirizzi ixi ON t.Piva = ixi.Piva ")
            stb.AppendLine(" INNER JOIN Indirizzi i ON ixi.Cod_Indirizzo = i.Cod_Indirizzo  ")
            If (rendicontazioni) Then
                stb.AppendLine(" Join UMA_Setup uset On uset.Anno = '" + Agro_SQL_SaveText(anno) + "' ")
            End If
            stb.AppendLine(" LEFT JOIN ISTAT ON i.pro_cod_istat = ISTAT.PROV And i.com_cod_istat = ISTAT.COM ")

            stb.AppendLine(" LEFT JOIN #TempVerificaInCorso appr ON t.Pratica_Cod = appr.Pratica_Cod ")

            If Filtro_Visibilita_Utente Then
                stb.AppendLine(" LEFT JOIN Utenti_Visibilita_Appoggio On imp.Piva = Utenti_Visibilita_Appoggio.Piva AND Utenti_Visibilita_Appoggio.Entita_Cod=1 AND Utenti_Visibilita_Appoggio.Username = " & Agro_SQL_SaveText_NULL(objParametri_Server.UtenteUsername) & " ")
            End If

            If nuovaVisibilita Then
                If Not VisibilitaTotale Then
                    If FiltroUtente And Not FiltroGruppoUtente Then
                        stb.AppendLine(" LEFT JOIN " & objParametri_Utenti.Recupera_NomeDB & ".dbo.Utenti_Visibilita visibilita ON t.Piva = visibilita.Piva_Azienda AND visibilita.Username = '" & objParametri_Utenti.UtenteUsername & "' ")
                    End If
                    If Not FiltroUtente And FiltroGruppoUtente Then
                        stb.AppendLine(" LEFT JOIN " & objParametri_Utenti.Recupera_NomeDB & ".dbo.Utenti_Visibilita visibilita ON t.Piva = visibilita.Piva_Azienda AND visibilita.Gruppo = " & GruppoUtente & " ")
                    End If
                End If
            End If

            'Anna 12/08/21: Aggiunte due nuove colonne per UMA
            stb.AppendLine("  LEFT JOIN " & NomeDB_Utenti & ".dbo.utenti_dettagli Richiedente ")
            stb.Append("  ON Richiedente.UserName = t.Username_Creazione")

            stb.AppendLine("  LEFT JOIN " & NomeDB_Utenti & ".dbo.utenti_dettagli Approvatoret ")
            stb.Append(" ON appr.UserName_Creazione = Approvatoret.Username ")
            stb.AppendLine(" LEFT JOIN " & NomeDB_Utenti & ".dbo.utenti_dettagli Approvatorep ")
            stb.Append(" ON appr.UserName_Creazione = Approvatorep.CodFisc ")

            If (Not rendicontazioni) Then
                stb.AppendLine(" LEFT JOIN UMA_Richieste_Testata origin ON t.Richiesta_Origine_Cod = origin.Richiesta_Cod ")
                stb.AppendLine(" LEFT JOIN Pratiche pOrigin ON origin.Pratica_Cod = pOrigin.Pratica_Cod ")
            End If

            stb.AppendLine(" WHERE ")
            If piva <> "" Then
                stb.AppendLine(" t.piva = '" + Agro_SQL_SaveText(piva) + "' ")
            stb.AppendLine(" AND t.Piva_SuperUser = '" + Agro_SQL_SaveText(objParametri_Server.PivaSuperUser) + "' AND ")
            End If
            stb.AppendLine(" t.Avanzamento_Richiesta " + IIf(rendicontazioni, " = 1 ", " = 0 "))
            If (anno > 0) Then
                stb.AppendLine(" AND p.Anno = '" + Agro_SQL_SaveText(anno) + "' ")
            End If
            If (statoCod > 0) Then
                stb.AppendLine(" AND psaw.WAnagraficaStati_Cod = " + statoCod.ToString + " ")
            End If
            If (conto < 1) Then
                stb.AppendLine(" AND t.tipo_richiesta = " + conto.ToString + " ")
            End If
            If (citta <> "-1" AndAlso citta <> "") Then
                stb.AppendLine(" And ISTAT.COM = '" + Agro_SQL_SaveText(citta) + "' ")
            End If
            If (prov <> "-1") Then
                stb.AppendLine(" AND ISTAT.PROV = '" + Agro_SQL_SaveText(prov) + "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND  " + Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo) + " ")
            End If

            If Filtro_Visibilita_Utente Then
                stb.AppendLine(" AND Utenti_Visibilita_Appoggio.Username = " & Agro_SQL_SaveText_NULL(objParametri_Server.UtenteUsername) & "  ")
            End If

            If nuovaVisibilita And Not VisibilitaTotale Then
                stb.Append(" AND (NOT visibilita.Piva_Azienda IS NULL) ")
            End If

            If piveVisibili.Length > 0 Then
                stb.AppendLine(" AND t.Piva IN (" & Agro_SQL_Save_Clausola_IN("'" & piveVisibili & "'", True) & ")  ")
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " + Agro_SQL_Save_xOrderBy(xOrderBy) + " ")
            End If

            stb.AppendLine(" ) As x ")
            stb.AppendLine(" where (x.CodstatoAv between 2003 and 2006) OR x.CodstatoAv IN (2001, 2002, 2007, 2008, 2009) ")

            'Eliminazione tabella temporanea
            'stb.AppendLine(" If (OBJECT_ID('tempdb..#TempLav') Is Not Null) ")
            'stb.AppendLine(" Begin ")
            'stb.AppendLine(" Drop Table #TempLav ")
            'stb.AppendLine(" End ")

            'stb.AppendLine(" If (OBJECT_ID('tempdb..#TempAllevamenti') Is Not Null) ")
            'stb.AppendLine(" Begin ")
            'stb.AppendLine(" Drop Table #TempAllevamenti ")
            'stb.AppendLine(" End ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Server, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt


    End Function

    Public Function CheckTestate(ByVal piva As String,
                                 ByVal Richiesta_Cod As Integer,
                                 ByVal Tipo_Richiesta As Integer,
                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable


        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_Richieste_Testata_R.CheckTestate()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable
        Dim cod As Integer = -1

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT UMA_Richieste_Testata.*, Pratiche.Anno, Pratiche.Numero ")
            stb.AppendLine(" FROM UMA_Richieste_Testata ")
            stb.AppendLine(" LEFT JOIN Pratiche ON UMA_Richieste_Testata.Pratica_Cod = Pratiche.Pratica_Cod ")
            stb.AppendLine(" WHERE UMA_Richieste_Testata.piva = '" + Agro_SQL_SaveText(piva) + "' ")
            stb.AppendLine(" AND UMA_Richieste_Testata.Piva_SuperUser = '" + Agro_SQL_SaveText(objParametri.PivaSuperUser) + "' ")
            If Tipo_Richiesta <> -2 Then
                stb.AppendLine(" AND UMA_Richieste_Testata.Tipo_Richiesta = " + Agro_SQL_SaveNum(Tipo_Richiesta) + " ")
            End If

            If Richiesta_Cod <> 0 AndAlso Richiesta_Cod <> -1 Then
                stb.AppendLine(" AND UMA_Richieste_Testata.Richiesta_Cod = " + Agro_SQL_SaveNum(Richiesta_Cod) + " ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            'If (dt.Rows.Count > 0) Then
            '    cod = dt.Rows.Item(0).Item("Richiesta_Cod")
            'End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt


    End Function

    Public Function GetProvince(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_Richieste_Testata_R.GetProvince()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT Distinct Lista_Province.PROV ,Lista_Province.PROVINCIA ")
            stb.AppendLine(" FROM  Lista_Province ")
            stb.AppendLine(" WHERE SIGLA <> '00' AND PROV > '0' ")
            stb.AppendLine(" ORDER BY PROVINCIA ")

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

    Public Function Macrousi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable


        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_Richieste_Testata_R.Macrousi()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT distinct Macrouso_UMA_Cod, Macrouso_UMA_Des ")
            stb.AppendLine(" FROM Codifica_SpecieVegetali_Agea_2015_2020 ")
            stb.AppendLine(" where Macrouso_UMA_Cod > 0 ")

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

End Class


Public Class UMA_Richieste_Testata_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Nuova_Richiesta(ByVal Richiesta As UMA_Richieste_Testata,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.UMA_Richieste_Testata_W.Nuova_Richiesta()"
        Dim messaggioErrore As String = ""
        Dim risultato As Integer = 0

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try


            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)


                GiasContext.UMA_Richieste_Testata.Add(Richiesta)

                ' COMMIT Effettivo
                GiasContext.SaveChanges()


            End Using


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
            risultato = -1
        End Try

        Return risultato


    End Function

    Public Function Elimina_Richiesta(ByRef GiasContext As Gias_DeveloperServer_Entities,
                                      ByVal testataToDelete As ArrayList,
                                      ByVal richiesteToDelete As ArrayList,
                                      ByVal lavorazioniToDelete As ArrayList,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.UMA_Richieste_Testata.Elimina_Richiesta()"
        Dim messaggioErrore As String = ""
        Dim risultato As Integer = 0

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try
            Dim gsWasNothing As Boolean = False
            If GiasContext Is Nothing Then
                GiasContext = New Gias_DeveloperServer_Entities(EFConnString)
                gsWasNothing = True
            End If

            For Each delL As UMA_Richieste_Lavorazioni In lavorazioniToDelete.ToArray
                GiasContext.UMA_Richieste_Lavorazioni.Attach(delL)
                GiasContext.UMA_Richieste_Lavorazioni.Remove(delL)
            Next

            For Each delR As UMA_Richieste In richiesteToDelete
                GiasContext.UMA_Richieste.Attach(delR)
                GiasContext.UMA_Richieste.Remove(delR)
            Next

            For Each delT As UMA_Richieste_Testata In testataToDelete
                GiasContext.UMA_Richieste_Testata.Attach(delT)
                GiasContext.UMA_Richieste_Testata.Remove(delT)
            Next

            ' COMMIT Effettivo
            GiasContext.SaveChanges()

            If gsWasNothing Then
                GiasContext.Dispose()
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
            risultato = -1
        End Try

        Return risultato

    End Function

    Public Function Modifica_Campo_Richiesta(Campo As String,
                                             Valore As Object,
                                             Piva As String,
                                             Richiesta_Cod As Integer,
                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.UMA_Richieste_Testata_W.Modifica_Campo_Richiesta()"
        Dim messaggioErrore As String = ""
        Dim risultato As Integer = 0

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try


            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
                Dim PivaSuperUser = objParametri.PivaSuperUser
                Dim testata = (From t In GiasContext.UMA_Richieste_Testata Where t.Piva_SuperUser = PivaSuperUser And
                                                                               t.Piva = Piva And
                                                                               t.Richiesta_Cod = Richiesta_Cod).FirstOrDefault

                If testata IsNot Nothing Then

                    testata.GetType.GetProperty(Campo).SetValue(testata, Valore)
                    testata.Data_Modifica = DateTime.Now
                    testata.Username_Modifica = objParametri.UtenteUsername

                    ' COMMIT Effettivo
                    GiasContext.SaveChanges()

                End If


            End Using


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
            risultato = -1
        End Try

        Return risultato


    End Function

    Public Function Aggiorna_Richiesta_Carburanti(richiestaTesta As UMA_Richieste_Testata, objParametri As AgronicaCoreParametri) As Integer

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.UMA_Richieste_Testata.Aggiorna_Richiesta_Carburanti()"
        Dim messaggioErrore As String = ""
        Dim risultato As Integer = 0

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                GiasContext.UMA_Richieste_Testata.Attach(richiestaTesta)
                GiasContext.Entry(richiestaTesta).State = EntityState.Modified

                ' COMMIT Effettivo
                GiasContext.SaveChanges()


            End Using


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
            risultato = -1
        End Try

        Return risultato

    End Function


End Class