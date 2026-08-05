Imports System.Data.Entity
Imports System.Text
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModelsSTD.metaschema
Imports InData
Imports Newtonsoft.Json

Public Class OGenerazioni_Anagrafe_Log_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function OMNI_Esiste(ByVal Modulo_Generazione As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByRef objParametri As AgronicaCoreParametri
                                ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Log_R.OMNI_Esiste()"

        Dim messaggioErrore As String = ""
        Dim Flag_Esiste As Boolean = False
        Dim dt As DataTable

        Try

            dt = Leggi("", 0, 0,
                       Modulo_Generazione,
                       0, 0,
                       AGRODATAINIZIO,
                       AGRODATAFINE,
                       xFiltroAggiuntivo,
                       "",
                       objParametri)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                Flag_Esiste = True
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return Flag_Esiste

    End Function


    '##############################################################################################
    Public Function Leggi(ByVal Piva As String,
                          ByVal Sa_Cod As Integer,
                          ByVal Linea_Cod As Integer,
                          ByVal Modulo_Generazione As Integer,
                          ByVal Tipo_Generazione As Integer,
                          ByVal Codice_Generazione As Integer,
                          ByVal Validita_Inizio As Date,
                          ByVal Validita_Fine As Date,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Log_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT * ")
            strSql.AppendLine(" FROM  OGenerazioni_Anagrafe_Log ")

            strSql.AppendLine(" WHERE   OGenerazioni_Anagrafe_Log.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            strSql.AppendLine(" AND     OGenerazioni_Anagrafe_Log.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            strSql.AppendLine(" AND     OGenerazioni_Anagrafe_Log.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            If Piva <> "" Then
                strSql.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Linea_Cod <> 0 Then
                strSql.AppendLine(" AND Linea_Cod = " & Agro_SQL_SaveNum(Linea_Cod) & "   ")
            End If

            If Modulo_Generazione <> 0 Then
                strSql.AppendLine(" AND Modulo_Generazione = " & Agro_SQL_SaveNum(Modulo_Generazione) & "   ")
            End If

            If Tipo_Generazione <> 0 Then
                strSql.AppendLine(" AND Tipo_Generazione = " & Agro_SQL_SaveNum(Tipo_Generazione) & " ")
            End If

            If Codice_Generazione <> 0 Then
                strSql.AppendLine(" AND Codice_Generazione = " & Agro_SQL_SaveNum(Codice_Generazione) & " ")
            End If


            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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


    '##############################################################################################
    Public Function MateriePrime_JoinReport_byLinee(ByVal Piva As String,
                                                    ByVal Sa_Cod As Integer,
                                                    ByVal Linea_Cod As Integer,
                                                    ByVal Modulo_Generazione As Integer,
                                                    ByVal Tipo_Generazione As Integer,
                                                    ByVal Codice_Generazione As Integer,
                                                    ByVal Elem_cod As Integer,
                                                    ByVal Mat_cod As Integer,
                                                    ByVal Id_Report As Integer,
                                                    ByVal Validita_Inizio As Date,
                                                    ByVal Validita_Fine As Date,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreParametri
                                                    ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Log_R.MateriePrime_JoinReport_byLinee()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT DISTINCT Materie_Prime.mat_cod, Mat_des, cod_articolo,ChkScollegamento ")

            strSql.AppendLine(" FROM  OGenerazioni_Anagrafe_Log ")

            strSql.AppendLine(" INNER JOIN Materie_Prime ")
            strSql.AppendLine("     ON Materie_Prime.mat_Cod =  OGenerazioni_Anagrafe_Log.mat_Cod and  Materie_Prime.ELEM_Cod =  OGenerazioni_Anagrafe_Log.ELEM_Cod ")

            strSql.AppendLine(" INNER JOIN Materie_PrimexReport ON materie_PrimexReport.Piva = Materie_Prime.Piva AND materie_PrimexReport.mat_cod = Materie_Prime.mat_cod ")

            strSql.AppendLine(" WHERE   OGenerazioni_Anagrafe_Log.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            strSql.AppendLine(" AND     OGenerazioni_Anagrafe_Log.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            strSql.AppendLine(" AND     OGenerazioni_Anagrafe_Log.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            If Piva <> "" Then
                strSql.AppendLine(" AND OGenerazioni_Anagrafe_Log.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND OGenerazioni_Anagrafe_Log.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Linea_Cod <> 0 Then
                strSql.AppendLine(" AND OGenerazioni_Anagrafe_Log.Linea_Cod = " & Agro_SQL_SaveNum(Linea_Cod) & "   ")
            End If

            If Modulo_Generazione <> 0 Then
                strSql.AppendLine(" AND OGenerazioni_Anagrafe_Log.Modulo_Generazione = " & Agro_SQL_SaveNum(Modulo_Generazione) & "   ")
            End If

            If Tipo_Generazione <> 0 Then
                strSql.AppendLine(" AND OGenerazioni_Anagrafe_Log.Tipo_Generazione = " & Agro_SQL_SaveNum(Tipo_Generazione) & " ")
            End If

            If Codice_Generazione <> 0 Then
                strSql.AppendLine(" AND OGenerazioni_Anagrafe_Log.Codice_Generazione = " & Agro_SQL_SaveNum(Codice_Generazione) & " ")
            End If

            If Elem_cod <> 0 Then
                strSql.AppendLine(" AND OGenerazioni_Anagrafe_Log.elem_cod = " & Agro_SQL_SaveNum(Elem_cod) & " ")
            End If

            If Mat_cod <> 0 Then
                strSql.AppendLine(" AND OGenerazioni_Anagrafe_Log.mat_cod = " & Agro_SQL_SaveNum(Mat_cod) & " ")
            End If

            If Id_Report <> 0 Then
                strSql.AppendLine(" AND Materie_PrimexReport.Id_Report = " & Agro_SQL_SaveNum(Id_Report) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   OGenerazioni_Anagrafe_Log.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   OGenerazioni_Anagrafe_Log.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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


    '##############################################################################################
    Public Function Linee_JoinReport_byMateriePrime(ByVal Piva As String,
                                                    ByVal Sa_Cod As Integer,
                                                    ByVal Linea_Cod As Integer,
                                                    ByVal Modulo_Generazione As Integer,
                                                    ByVal Tipo_Generazione As Integer,
                                                    ByVal Codice_Generazione As Integer,
                                                    ByVal Elem_cod As Integer,
                                                    ByVal Mat_cod As Integer,
                                                    ByVal Id_Report As Integer,
                                                    ByVal Validita_Inizio As Date,
                                                    ByVal Validita_Fine As Date,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreParametri
                                                    ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Log_R.Linee_JoinReport_byMateriePrime()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT DISTINCT Linee_Produzioni.*, Linea_Classe_Des ")

            strSql.AppendLine(" FROM  OGenerazioni_Anagrafe_Log ")

            strSql.AppendLine(" INNER JOIN Linee_Produzioni ")
            strSql.AppendLine("     ON Linee_Produzioni.linea_Cod =  OGenerazioni_Anagrafe_Log.linea_Cod AND  Linee_Produzioni.PIVA =  OGenerazioni_Anagrafe_Log.piva ")

            strSql.AppendLine(" INNER JOIN Linee_Classi_Produzioni ")
            strSql.AppendLine("             ON Linee_Produzioni.Piva =  Linee_Classi_Produzioni.Piva ")
            strSql.AppendLine("             AND Linee_Produzioni.Linea_Classe_Cod =  Linee_Classi_Produzioni.Linea_Classe_Cod ")

            strSql.AppendLine(" INNER JOIN Linee_ProduzionixPreparazioni ")
            strSql.AppendLine("             ON Linee_Produzioni.Piva =  Linee_ProduzionixPreparazioni.Piva ")
            strSql.AppendLine("             AND Linee_Produzioni.Linea_Cod =  Linee_ProduzionixPreparazioni.Linea_Cod ")

            strSql.AppendLine(" INNER JOIN  Linee_Preparazioni ")
            strSql.AppendLine("              ON Linee_Preparazioni.Piva =  Linee_ProduzionixPreparazioni.Piva ")
            strSql.AppendLine("             AND Linee_Preparazioni.Preparazione_Cod =  Linee_ProduzionixPreparazioni.Preparazione_Cod ")

            strSql.AppendLine(" INNER JOIN Linee_PreparazionixReport ")
            strSql.AppendLine("             ON Linee_Preparazioni.Piva =  Linee_PreparazionixReport.Piva  ")
            strSql.AppendLine("             AND Linee_Preparazioni.Preparazione_Cod =  Linee_PreparazionixReport.Preparazione_Cod ")

            strSql.AppendLine(" WHERE   OGenerazioni_Anagrafe_Log.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            strSql.AppendLine(" AND     OGenerazioni_Anagrafe_Log.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            strSql.AppendLine(" AND     OGenerazioni_Anagrafe_Log.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            If Piva <> "" Then
                strSql.AppendLine(" AND OGenerazioni_Anagrafe_Log.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND OGenerazioni_Anagrafe_Log.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Linea_Cod <> 0 Then
                strSql.AppendLine(" AND OGenerazioni_Anagrafe_Log.Linea_Cod = " & Agro_SQL_SaveNum(Linea_Cod) & "   ")
            End If

            If Modulo_Generazione <> 0 Then
                strSql.AppendLine(" AND OGenerazioni_Anagrafe_Log.Modulo_Generazione = " & Agro_SQL_SaveNum(Modulo_Generazione) & "   ")
            End If

            If Tipo_Generazione <> 0 Then
                strSql.AppendLine(" AND OGenerazioni_Anagrafe_Log.Tipo_Generazione = " & Agro_SQL_SaveNum(Tipo_Generazione) & " ")
            End If

            If Codice_Generazione <> 0 Then
                strSql.AppendLine(" AND OGenerazioni_Anagrafe_Log.Codice_Generazione = " & Agro_SQL_SaveNum(Codice_Generazione) & " ")
            End If

            If Elem_cod <> 0 Then
                strSql.AppendLine(" AND OGenerazioni_Anagrafe_Log.elem_cod = " & Agro_SQL_SaveNum(Elem_cod) & " ")
            End If

            If Mat_cod <> 0 Then
                strSql.AppendLine(" AND OGenerazioni_Anagrafe_Log.mat_cod = " & Agro_SQL_SaveNum(Mat_cod) & " ")
            End If

            If Id_Report <> 0 Then
                strSql.AppendLine(" AND Linee_PreparazionixReport.Id_Report = " & Agro_SQL_SaveNum(Id_Report) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   OGenerazioni_Anagrafe_Log.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   OGenerazioni_Anagrafe_Log.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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


    '##############################################################################################
    Public Function Leggi_MateriaPrima(ByVal Piva As String,
                                       ByVal Sa_Cod As Integer,
                                       ByVal Linea_Cod As Integer,
                                       ByVal Modulo_Generazione As Integer,
                                       ByVal Tipo_Generazione As Integer,
                                       ByVal Codice_Generazione As Integer,
                                       ByVal Validita_Inizio As Date,
                                       ByVal Validita_Fine As Date,
                                       ByVal xFiltroAggiuntivo As String,
                                       ByVal xOrderBy As String,
                                       ByRef objParametri As AgronicaCoreParametri
                                       ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Log_R.Leggi_MateriaPrima()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT DISTINCT Elem_Cod, Mat_Cod ")
            strSql.AppendLine(" FROM  OGenerazioni_Anagrafe_Log ")
            strSql.AppendLine(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            strSql.AppendLine(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            If objParametri.PivaSuperUser <> "" Then
                strSql.AppendLine(" AND Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            End If

            If Piva <> "" Then
                strSql.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Linea_Cod <> 0 Then
                strSql.AppendLine(" AND Linea_Cod = " & Agro_SQL_SaveNum(Linea_Cod) & "   ")
            End If

            If Modulo_Generazione <> 0 Then
                strSql.AppendLine(" AND Modulo_Generazione = " & Agro_SQL_SaveNum(Modulo_Generazione) & "   ")
            End If

            If Tipo_Generazione <> 0 Then
                strSql.AppendLine(" AND Tipo_Generazione = " & Agro_SQL_SaveNum(Tipo_Generazione) & " ")
            End If

            If Codice_Generazione <> 0 Then
                strSql.AppendLine(" AND Codice_Generazione = " & Agro_SQL_SaveNum(Codice_Generazione) & " ")
            End If


            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    Public Function LeggiOmniLog_Modulo(ByVal piva As String,
                                        ByVal Modulo_Generazione As Integer,
                                        ByVal filtro_aggiuntivo As String,
                                        ByRef objParametri As AgronicaCoreParametri
                                        ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Log_R.LeggiOmniLog_Modulo()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT OGenerazioni_Anagrafe_Moduli_Log.* ")
            strSql.AppendLine(" From OGenerazioni_Anagrafe_Moduli_Log ")
            strSql.AppendLine(" Where OGenerazioni_Anagrafe_Moduli_Log.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            strSql.AppendLine(" And OGenerazioni_Anagrafe_Moduli_Log.Piva = '" & Agro_SQL_SaveText(piva) & "'")

            If Modulo_Generazione <> 0 Then
                strSql.AppendLine(" And OGenerazioni_Anagrafe_Moduli_Log.Modulo_Generazione = " & Agro_SQL_SaveNum(Modulo_Generazione) & " ")
            End If

            If filtro_aggiuntivo <> "" Then
                strSql.AppendLine(" And " & filtro_aggiuntivo & " ")
            End If


            strSql.AppendLine(" Order by Modulo_Generazione Asc ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("" & nomeRoutine & " :  " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function LeggiOmniLog_Prodotto(ByVal piva As String,
                                          ByVal Modulo_Generazione As Integer,
                                          ByVal Tipo_Generazione As Integer,
                                          ByVal Codice_Generazione As Integer,
                                          ByVal Linea_Cod As Integer,
                                          ByVal Elem_Cod As Integer,
                                          ByVal Mat_Cod As Integer,
                                          ByVal filtro_aggiuntivo As String,
                                          ByRef objParametri As AgronicaCoreParametri
                                          ) As DataTable

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Const nomeRoutine = "AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Log_R.LeggiOmniLog_Prodotto()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT OGenerazioni_Anagrafe_Log.*, Materie_Prime.Cod_Articolo, Materie_Prime.Mat_Des, Materie_Prime.Veg_Cod, Materie_Prime.Cul_Cod , Materie_Prime.Regolamento")
            strSql.AppendLine(" From OGenerazioni_Anagrafe_Log, Materie_Prime ")
            strSql.AppendLine(" Where OGenerazioni_Anagrafe_Log.Piva = '" & Agro_SQL_SaveText(piva) & "'")
            strSql.AppendLine(" And  OGenerazioni_Anagrafe_Log.Mat_Cod = Materie_Prime.Mat_Cod ")

            If Modulo_Generazione <> 0 Then
                strSql.AppendLine(" And OGenerazioni_Anagrafe_Log.Modulo_Generazione = " & Agro_SQL_SaveNum(Modulo_Generazione) & " ")
            End If

            If Tipo_Generazione <> 0 Then
                strSql.AppendLine(" And OGenerazioni_Anagrafe_Log.Tipo_Generazione = " & Agro_SQL_SaveNum(Tipo_Generazione) & " ")
            End If

            If Codice_Generazione <> 0 Then
                strSql.AppendLine(" And OGenerazioni_Anagrafe_Log.Codice_Generazione = " & Agro_SQL_SaveNum(Codice_Generazione) & " ")
            End If

            If Linea_Cod <> 0 Then
                strSql.AppendLine(" And OGenerazioni_Anagrafe_Log.Linea_Cod = " & Agro_SQL_SaveNum(Linea_Cod) & " ")
            End If

            If Elem_Cod <> 0 Then
                strSql.AppendLine(" And OGenerazioni_Anagrafe_Log.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & " ")
            End If

            If Mat_Cod <> 0 Then
                strSql.AppendLine(" And OGenerazioni_Anagrafe_Log.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & " ")
            End If

            If filtro_aggiuntivo <> "" Then
                strSql.AppendLine(" And " & filtro_aggiuntivo & " ")
            End If


            strSql.AppendLine(" Order by Id_Generazione Desc ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("" & nomeRoutine & " :  " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Leggi_Linee_Produzione_Log(ByVal piva As String,
                                               ByVal Modulo_Generazione As Integer,
                                               ByVal Tipo_Generazione As Integer,
                                               ByVal Codice_Generazione As Integer,
                                               ByVal Linea_Cod As Integer,
                                               ByVal Elem_Cod As Integer,
                                               ByVal Mat_Cod As Integer,
                                               ByVal filtro_aggiuntivo As String,
                                               ByRef objParametri As AgronicaCoreParametri
                                               ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Log_R.Leggi_Linee_Produzione_Log()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT Linee_Produzioni.*  ")
            strSql.AppendLine(" From OGenerazioni_Anagrafe_Log, Linee_Produzioni ")
            strSql.AppendLine(" Where OGenerazioni_Anagrafe_Log.Piva = '" & Agro_SQL_SaveText(piva) & "'")
            strSql.AppendLine(" And  OGenerazioni_Anagrafe_Log.Linea_Cod = Linee_Produzioni.Linea_Cod ")

            If Modulo_Generazione <> 0 Then
                strSql.AppendLine(" And OGenerazioni_Anagrafe_Log.Modulo_Generazione = " & Agro_SQL_SaveNum(Modulo_Generazione) & " ")
            End If

            If Tipo_Generazione <> 0 Then
                strSql.AppendLine(" And OGenerazioni_Anagrafe_Log.Tipo_Generazione = " & Agro_SQL_SaveNum(Tipo_Generazione) & " ")
            End If

            If Codice_Generazione <> 0 Then
                strSql.AppendLine(" And OGenerazioni_Anagrafe_Log.Codice_Generazione = " & Agro_SQL_SaveNum(Codice_Generazione) & " ")
            End If

            If Linea_Cod <> 0 Then
                strSql.AppendLine(" And OGenerazioni_Anagrafe_Log.Linea_Cod = " & Agro_SQL_SaveNum(Linea_Cod) & " ")
            End If

            If Elem_Cod <> 0 Then
                strSql.AppendLine(" And OGenerazioni_Anagrafe_Log.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & " ")
            End If

            If Mat_Cod <> 0 Then
                strSql.AppendLine(" And OGenerazioni_Anagrafe_Log.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & " ")
            End If

            If filtro_aggiuntivo <> "" Then
                strSql.AppendLine(" And " & filtro_aggiuntivo & " ")
            End If

            strSql.AppendLine(" Order by Linea_Des Desc ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("" & nomeRoutine & " :  " & messaggioErrore)
        End Try

        Return dt

    End Function

    '###################################################################################
    Public Sub Recupera_Magazzino_Confezionato(ByVal Piva As String,
                                               ByVal xFiltroAggiuntivo As String,
                                               ByRef objParametri As AgronicaCoreParametri,
                                               ByRef Sa_Cod As Integer,
                                               ByRef Fabbricato_Cod As Integer)

        Const nomeRoutine = "AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Log_R.Recupera_Magazzino_Confezionato()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable

        Fabbricato_Cod = 0
        If IsNothing(Sa_Cod) Then
            Sa_Cod = 0
        End If
        Try

            dt = Leggi(Piva,
                       Sa_Cod, 0,
                       enum_Omni_Modulo_Generazione.Cantine,
                       enum_Omni_Tipo_Generazione.MagazziniRecipienti,
                       enum_Omni_Codice_Generazione.Tipo15_RepartoConfezionato,
                       AGRODATAINIZIO,
                       AGRODATAFINE,
                       xFiltroAggiuntivo,
                       "",
                       objParametri)

            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                Sa_Cod = dt.Rows(0).Item("Sa_cod")
                Fabbricato_Cod = dt.Rows(0).Item("Key1")
            End If

            dt = Nothing

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

    Public Sub Recupera_Magazzino_Imballaggi(ByVal piva As String,
                                             ByVal moduloGias As enum_Omni_Modulo_Generazione,
                                             ByVal xFiltroAggiuntivo As String,
                                             ByRef objParametri As AgronicaCoreParametri,
                                             ByRef Sa_Cod As Integer,
                                             ByRef Fabbricato_Cod As Integer)

        Const nomeRoutine = "AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Log_R.Recupera_Magazzino_Imballaggi()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable

        Try

            Dim codiceGenerazioneMagazzino As Integer = 0
            Select Case moduloGias
                Case enum_Omni_Modulo_Generazione.FreshFood
                    codiceGenerazioneMagazzino = enum_Omni_Codice_Generazione.Tipo15_MagazzinoOrtofrutta
                Case enum_Omni_Modulo_Generazione.Tabacco
                    codiceGenerazioneMagazzino = enum_Omni_Codice_Generazione.Tipo15_MagazzinoTabacco
            End Select


            Fabbricato_Cod = 0
            If IsNothing(Sa_Cod) Then
                Sa_Cod = 0
            End If


            dt = Leggi(piva,
                       Sa_Cod, 0,
                       moduloGias,
                       enum_Omni_Tipo_Generazione.MagazziniRecipienti,
                       codiceGenerazioneMagazzino,
                       AGRODATAINIZIO,
                       AGRODATAFINE,
                       xFiltroAggiuntivo,
                       "Piva, Sa_Cod, ChkScollegamento, Tipo_Generazione, Codice_Generazione, Linea_Cod, Key1, Mat_Cod, Elem_Cod ASC",
                       objParametri)

            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                Sa_Cod = dt.Rows(0).Item("Sa_cod")
                Fabbricato_Cod = dt.Rows(0).Item("Key1")
            End If

            dt = Nothing

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

    Public Function LeggiProdottiPerSpecificaLavorazione(ByVal piva As String,
                                                         ByVal linee_Preparazioni_CodiceGenerazione As Integer,
                                                         ByVal oGenerazioni_Anagrafe_Log_CodiceGenerazione As Integer,
                                                         ByRef objParametri As AgronicaCoreParametri
                                                         ) As String

        Dim risposta As String = ""

        Const nomeRoutine = "OGenerazioni_Anagrafe_Log_R.ProdottiPerLineaCalibratura()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim mat = From mp In GiasContext.Materie_Prime
                      Join ogal In GiasContext.OGenerazioni_Anagrafe_Log.Where(Function(x) x.ChkScollegamento = 0)
                          On ogal.Mat_Cod Equals mp.Mat_Cod
                      Join l_pxp In GiasContext.Linee_ProduzionixPreparazioni
                          On l_pxp.Linea_Cod Equals mp.Linea_Cod
                      Join l_p In GiasContext.Linee_Preparazioni
                          On l_p.Preparazione_Cod Equals l_pxp.Preparazione_Cod
                      Group Join m_p_referenza In GiasContext.OGenerazioni_Anagrafe_Log.Where(Function(x) x.ChkScollegamento = 0)
                          On mp.Elem_Cod Equals m_p_referenza.Elem_Cod And
                          mp.Mat_Cod_Referenza Equals m_p_referenza.Mat_Cod Into Materie_Prime_Group = Group
                      From _Materie_Prime_Group In Materie_Prime_Group.DefaultIfEmpty()
                      Where l_p.Codice_Generazione = linee_Preparazioni_CodiceGenerazione And
                          ogal.Codice_Generazione = oGenerazioni_Anagrafe_Log_CodiceGenerazione And
                          l_pxp.Piva = piva
                      Order By mp.Mat_Des
                      Select New With {
                          .Mat_Cod = mp.Mat_Cod,
                         .Mat_Des = mp.Mat_Des,
                          .Elem_Cod = mp.Elem_Cod,
                          .Linea_Cod = If(_Materie_Prime_Group Is Nothing, mp.Linea_Cod, _Materie_Prime_Group.Linea_Cod),
                          .Preparazione_Cod = l_p.Preparazione_Cod
                      }
            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(mat.Distinct.ToList(), Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function

    Public Function ImpostaRelazioneMateriePrimeLineaProduzione(lineaProduzione As Linee_Produzioni, statiProdotto As String(), objParametri As AgronicaCoreParametri) As List(Of RelazioneMateriePrime)

        Dim risposta As List(Of RelazioneMateriePrime)

        Const nomeRoutine = "OGenerazioni_Anagrafe_Log_R.ImpostaRelazioneMateriePrimeLineaProduzione()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim statiEsistenti = GiasContext.OGenerazioni_Anagrafe_Log _
                .Where(Function(x) x.Linea_Cod = lineaProduzione.Linea_Cod) _
                .Select(Function(x) x.Codice_Generazione) _
                .Distinct()

            Dim stati = GiasContext.OGenerazioni_Anagrafe _
                .Where(Function(x) x.Tipo_Generazione = 4 AndAlso
                    x.Modulo_Generazione = lineaProduzione.Modulo_Generazione AndAlso
                    x.Elem_Cod = 210 AndAlso
                    (Not statiProdotto.Any() OrElse statiProdotto.Select(Function(s) s.ToLower).Contains(x.Descrizione.ToLower))) _
                .Where(Function(s) Not statiEsistenti.Contains(s.Codice_Generazione)) _
                .ToList()

            risposta = stati _
                .Select(Function(gal) New RelazioneMateriePrime With
                    {
                        .OGenerazioni_Anagrafe_Log = New OGenerazioni_Anagrafe_Log With
                            {
                                .ID = 0,
                                .Piva_SuperUser = objParametri.PivaSuperUser,
                                .Piva = lineaProduzione.Piva,
                                .Sa_Cod = -1,
                                .Linea_Cod = lineaProduzione.Linea_Cod,
                                .Modulo_Generazione = lineaProduzione.Modulo_Generazione,
                                .Tipo_Generazione = gal.Tipo_Generazione,
                                .Codice_Generazione = gal.Codice_Generazione,
                                .Elem_Cod = gal.Elem_Cod,
                                .Mat_Cod = 0,
                                .Key1 = 0,
                                .ID_Generazione = 0,
                                .Colore_Log = lineaProduzione.Colore,
                                .Categoria_Gias_Cod_Log = lineaProduzione.Categoria_Gias_Cod,
                                .Classificazione_Gias_Cod_Log = lineaProduzione.Classificazione_Gias_Cod,
                                .Preparazione_Cod_Rif_Log = 0,
                                .Livello_Log = 0,
                                .Veg_Cod_Log = lineaProduzione.Veg_Cod,
                                .Cul_Cod_Log = lineaProduzione.Cul_Cod,
                                .Gen_Cod_Log = 0,
                                .Raz_Cod_Log = 0,
                                .Spe_Cod_Log = 0,
                                .Reg_Cod_Log = lineaProduzione.Reg_Cod,
                                .Grfi_Cod_Log = lineaProduzione.Grfi_Cod,
                                .inviato = lineaProduzione.inviato,
                                .datainvio = lineaProduzione.datainvio,
                                .Data_Creazione = lineaProduzione.Data_Creazione,
                                .Data_Modifica = lineaProduzione.Data_Modifica,
                                .Username_Creazione = lineaProduzione.Username_Creazione,
                                .Username_Modifica = lineaProduzione.Username_Modifica,
                                .Validita_Inizio = lineaProduzione.Validita_Inizio,
                                .Validita_Fine = lineaProduzione.Validita_Fine,
                                .Dicitura_Gias_Cod_Log = lineaProduzione.Dicitura_Gias_Cod,
                                .Deno_Gias_Cod_Log = lineaProduzione.Deno_Gias_Cod,
                                .ChkScollegamento = 0,
                                .Caratteristica_Gias_Cod_Log = lineaProduzione.Caratteristica_Gias_Cod
                            },
                        .Materie_Prime = New Materie_Prime With
                            {
                                .Piva = lineaProduzione.Piva,
                                .Sa_Cod = -1,
                                .Elem_Cod = gal.Elem_Cod,
                                .Mat_Cod = 0,
                                .Cod_Articolo = $"{gal.Codice_Generazione}_{lineaProduzione.Linea_Cod_Des}",
                                .Mat_Des = $"{gal.Descrizione} {lineaProduzione.Linea_Des}",
                                .Sem_Cod = 0,
                                .Veg_Cod = lineaProduzione.Veg_Cod,
                                .Cul_Cod = lineaProduzione.Cul_Cod,
                                .Cal_Cod = 0,
                                .GRVA_COD_VEG = 0,
                                .Grfi_Cod = lineaProduzione.Grfi_Cod,
                                .Trap_Dur = 0,
                                .Uso = 0,
                                .ClToss_Cod = 0,
                                .NewClToss_Cod = 0,
                                .Ditta_Cod = 0,
                                .N = 0,
                                .P2O5 = 0,
                                .K2O = 0,
                                .MgO = 0,
                                .Note = String.Empty,
                                .inviato = lineaProduzione.inviato,
                                .datainvio = lineaProduzione.datainvio,
                                .data_creazione = lineaProduzione.Data_Creazione,
                                .data_modifica = lineaProduzione.Data_Modifica,
                                .username_creazione = lineaProduzione.Username_Creazione,
                                .username_modifica = lineaProduzione.Username_Modifica,
                                .validita_inizio = lineaProduzione.Validita_Inizio,
                                .validita_fine = lineaProduzione.Validita_Fine,
                                .Regolamento = lineaProduzione.Reg_Cod,
                                .Prezzo_Unitario = 0,
                                .Extra_Str = String.Empty,
                                .Extra_Int = 0,
                                .Extra_Date = AGRODATAINIZIO,
                                .GEN_COD = 0,
                                .SPE_COD = 0,
                                .RAZ_COD = 0,
                                .IPRO_COD = 0,
                                .CAT_COD = lineaProduzione.Linea_Classe_Cod,
                                .Flag_Biologico = 0,
                                .Flag_Convenzionale = 0,
                                .Flag_NonAgricolo = 0,
                                .Flag_AusiliareFabbricazione = 0,
                                .Udm_Cod_Extra = 0,
                                .Flag_Extra = 0,
                                .ChkImballaggio = 0,
                                .Qta_Extra = 0,
                                .Taglio = 0,
                                .Mat_Cod_Origine = 0,
                                .Piva_SuperUser_Origine = String.Empty,
                                .ChkListino = 0,
                                .Codice_Prodotto = 0,
                                .Colore = 0,
                                .Codice_NC = String.Empty,
                                .Manipolazioni = 0,
                                .Titolo_Alcol = 0,
                                .ChkContenitore = 0,
                                .Qta_Contenitore = 0,
                                .Tipo_Peso = 0,
                                .Tara = 0,
                                .Udm_Cod = 0,
                                .Peso_Set = 0,
                                .ChkEscludi_Magazzino = 0,
                                .ChkEscludi_Preparazione = 0,
                                .Id_Accisa_Cod = 0,
                                .Confezione_Cod = 0,
                                .Categoria_Vino_Cod = 0,
                                .Tipo_Reg_Alcoli = 0,
                                .ChkAlias = 0,
                                .Linea_Cod = 0,
                                .Tipo_Default = 0,
                                .ChkStampa_Dettagli = 0,
                                .ID_DisciplinareAcquisti = -1,
                                .ChkReferenza = 1,
                                .Mat_Cod_Referenza = 0,
                                .Lotto_Default = String.Empty,
                                .OTabella_Cod_Base = 0,
                                .Provenienza_TR = String.Empty,
                                .eBacchus = String.Empty,
                                .Flag_Variazione = 0,
                                .Codice_Esterno = String.Empty,
                                .Flag_Importato = 0,
                                .Priorita = 0
                            }
                    }
                ) _
                .ToList()

        End Using

        Return risposta

    End Function

    Public Function ImpostaRelazioneMateriePrimeLineaProduzione(lineaProduzione As Linee_Produzioni, objParametri As AgronicaCoreParametri) As List(Of RelazioneMateriePrime)

        Dim risposta As List(Of RelazioneMateriePrime)

        Const nomeRoutine = "OGenerazioni_Anagrafe_Log_R.ImpostaRelazioneMateriePrimeLineaProduzione()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            risposta = GiasContext.Materie_Prime _
                .Where(Function(x) x.Linea_Cod = lineaProduzione.Linea_Cod) _
                .Select(Function(mp) New RelazioneMateriePrime With
                    {
                        .OGenerazioni_Anagrafe_Log = Nothing,
                        .Materie_Prime = mp
                    }
                ) _
                .ToList()

            For Each item In risposta
                item.Materie_Prime.CAT_COD = lineaProduzione.Linea_Classe_Cod
            Next

        End Using

        Return risposta

    End Function


    Public Function LeggiRelazioneLineeProduzioneMateriePrime(piva As String, lineaCod As Integer, moduloGenerazione As Integer,
                                                              statiProdotto As String(), objParametri As AgronicaCoreParametri) As List(Of RelazioneMateriePrime)
        Dim risposta As List(Of RelazioneMateriePrime)

        Const nomeRoutine = "OGenerazioni_Anagrafe_Log_R.LeggiRelazioneLineeProduzioneMateriePrime()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            risposta = (
                From ga In GiasContext.OGenerazioni_Anagrafe _
                    .Where(Function(x) x.Modulo_Generazione = moduloGenerazione AndAlso
                        x.Tipo_Generazione = 4 AndAlso
                        statiProdotto.Contains(x.Descrizione))
                Join gal In GiasContext.OGenerazioni_Anagrafe_Log _
                    .Where(Function(x) x.Piva = piva AndAlso
                    x.Linea_Cod = lineaCod AndAlso
                    x.Modulo_Generazione = moduloGenerazione AndAlso
                    x.Tipo_Generazione = 4 AndAlso
                    x.Elem_Cod = 210)
                On ga.Modulo_Generazione Equals gal.Modulo_Generazione _
                And ga.Tipo_Generazione Equals gal.Tipo_Generazione _
                And ga.Codice_Generazione Equals gal.Codice_Generazione
                Join mp In GiasContext.Materie_Prime _
                    .Where(Function(x) x.Piva = piva AndAlso
                    x.Elem_Cod = 210)
                On gal.Piva Equals mp.Piva _
                And gal.Elem_Cod Equals mp.Elem_Cod _
                And gal.Mat_Cod Equals mp.Mat_Cod
                Select New RelazioneMateriePrime With
                {
                    .OGenerazioni_Anagrafe_Log = gal,
                    .Materie_Prime = mp
                }
                ) _
                .ToList()
        End Using

        Return risposta
    End Function
End Class


Public Class OGenerazioni_Anagrafe_Log_W
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Scrivi(ByVal Piva As String,
                           ByVal Sa_Cod As Long,
                           ByVal Linea_Cod As Integer,
                           ByVal Modulo_Generazione As Integer,
                           ByVal Tipo_Generazione As Integer,
                           ByVal Codice_Generazione As Integer,
                           ByVal Elem_Cod As Integer,
                           ByVal Mat_Cod As Integer,
                           ByVal Key1 As Integer,
                           ByVal ID_Generazione As Integer,
                           ByVal Colore_Log As Integer,
                           ByVal Categoria_Gias_Cod_Log As Integer,
                           ByVal Classificazione_Gias_Cod_Log As Integer,
                           ByVal Preparazione_Cod_Rif_Log As Integer,
                           ByVal Livello_Log As Integer,
                           ByVal Veg_Cod_Log As Integer,
                           ByVal Cul_Cod_Log As Integer,
                           ByVal Gen_Cod_Log As Integer,
                           ByVal Raz_Cod_Log As Integer,
                           ByVal Spe_Cod_Log As Integer,
                           ByVal Reg_Cod_Log As String,
                           ByVal Grfi_Cod_Log As Integer,
                           ByVal inviato As Integer,
                           ByVal datainvio As Date,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByVal Dicitura_Gias_Cod_Log As Integer,
                           ByVal Deno_Gias_Cod_Log As Integer,
                           ByVal ChkScollegamento As Integer,
                           ByVal Caratteristica_Gias_Cod_Log As Integer,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Log_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try


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



            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" INSERT INTO [dbo].[OGenerazioni_Anagrafe_Log] ")
            strSql.AppendLine("            ([Piva_SuperUser] ")
            strSql.AppendLine("            ,[Piva] ")
            strSql.AppendLine("            ,[Sa_Cod] ")
            strSql.AppendLine("            ,[Linea_Cod] ")
            strSql.AppendLine("            ,[Modulo_Generazione] ")
            strSql.AppendLine("            ,[Tipo_Generazione] ")
            strSql.AppendLine("            ,[Codice_Generazione] ")
            strSql.AppendLine("            ,[Elem_Cod] ")
            strSql.AppendLine("            ,[Mat_Cod] ")
            strSql.AppendLine("            ,[Key1] ")
            strSql.AppendLine("            ,[ID_Generazione] ")
            strSql.AppendLine("            ,[Colore_Log] ")
            strSql.AppendLine("            ,[Categoria_Gias_Cod_Log] ")
            strSql.AppendLine("            ,[Classificazione_Gias_Cod_Log] ")
            strSql.AppendLine("            ,[Preparazione_Cod_Rif_Log] ")
            strSql.AppendLine("            ,[Livello_Log] ")
            strSql.AppendLine("            ,[Veg_Cod_Log] ")
            strSql.AppendLine("            ,[Cul_Cod_Log] ")
            strSql.AppendLine("            ,[Gen_Cod_Log] ")
            strSql.AppendLine("            ,[Raz_Cod_Log] ")
            strSql.AppendLine("            ,[Spe_Cod_Log] ")
            strSql.AppendLine("            ,[Reg_Cod_Log] ")
            strSql.AppendLine("            ,[Grfi_Cod_Log] ")
            strSql.AppendLine("            ,[inviato] ")
            strSql.AppendLine("            ,[datainvio] ")
            strSql.AppendLine("            ,[Data_Creazione] ")
            strSql.AppendLine("            ,[Data_Modifica] ")
            strSql.AppendLine("            ,[Username_Creazione] ")
            strSql.AppendLine("            ,[Username_Modifica] ")
            strSql.AppendLine("            ,[Validita_Inizio] ")
            strSql.AppendLine("            ,[Validita_Fine] ")
            strSql.AppendLine("            ,[Dicitura_Gias_Cod_Log] ")
            strSql.AppendLine("            ,[Deno_Gias_Cod_Log] ")
            strSql.AppendLine("            ,[ChkScollegamento] ")
            strSql.AppendLine("            ,[Caratteristica_Gias_Cod_Log]) ")
            strSql.AppendLine("      VALUES ")
            strSql.AppendLine("            (" & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveText_NULL(Piva) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveNum(Linea_Cod) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveNum(Modulo_Generazione) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveNum(Tipo_Generazione) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveNum(Codice_Generazione) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveNum(Elem_Cod) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveNum(Mat_Cod) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveNum(Key1) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveNum(ID_Generazione) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveNum(Colore_Log) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveNum(Categoria_Gias_Cod_Log) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveNum(Classificazione_Gias_Cod_Log) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveNum(Preparazione_Cod_Rif_Log) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveNum(Livello_Log) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveNum(Veg_Cod_Log) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveNum(Cul_Cod_Log) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveNum(Gen_Cod_Log) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveNum(Raz_Cod_Log) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveNum(Spe_Cod_Log) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveNum(Reg_Cod_Log) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveNum(Grfi_Cod_Log) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveNum(inviato) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveDate(datainvio) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveDateTime(DateTime.Now) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveDateTime(DateTime.Now) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveText_NULL(username_creazione) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveText_NULL(username_modifica) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveNum(Dicitura_Gias_Cod_Log) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveNum(Deno_Gias_Cod_Log) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveNum(ChkScollegamento) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveNum(Caratteristica_Gias_Cod_Log) & " ) ")


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


    Public Function Cancella(ByVal Piva As String,
                           ByVal Sa_Cod As Long,
                           ByVal Linea_Cod As Integer,
                           ByVal Modulo_Generazione As Integer,
                           ByVal Tipo_Generazione As Integer,
                           ByVal Codice_Generazione As Integer,
                           ByVal Elem_Cod As Integer,
                           ByVal Mat_Cod As Integer,
                           ByVal Key1 As Integer,
                           ByVal ID_Generazione As Integer,
                           ByVal Colore_Log As Integer,
                           ByVal Categoria_Gias_Cod_Log As Integer,
                           ByVal Classificazione_Gias_Cod_Log As Integer,
                           ByVal Preparazione_Cod_Rif_Log As Integer,
                           ByVal Livello_Log As Integer,
                           ByVal Veg_Cod_Log As Integer,
                           ByVal Cul_Cod_Log As Integer,
                           ByVal Gen_Cod_Log As Integer,
                           ByVal Raz_Cod_Log As Integer,
                           ByVal Spe_Cod_Log As Integer,
                           ByVal Reg_Cod_Log As String,
                           ByVal Grfi_Cod_Log As Integer,
                           ByVal Dicitura_Gias_Cod_Log As Integer,
                           ByVal Deno_Gias_Cod_Log As Integer,
                           ByVal ChkScollegamento As Integer,
                           ByVal Caratteristica_Gias_Cod_Log As Integer,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Log_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" DELETE FROM [dbo].[OGenerazioni_Anagrafe_Log] ")
            strSql.AppendLine("      WHERE 1=1 ")

            If Piva <> "" Then
                strSql.AppendLine(" AND Piva = " & Agro_SQL_SaveText(Piva) & " ")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Linea_Cod <> 0 Then
                strSql.AppendLine(" AND Linea_Cod = " & Agro_SQL_SaveNum(Linea_Cod) & " ")
            End If

            If Modulo_Generazione <> 0 Then
                strSql.AppendLine(" AND Modulo_Generazione = " & Agro_SQL_SaveNum(Modulo_Generazione) & " ")
            End If

            If Tipo_Generazione <> 0 Then
                strSql.AppendLine(" AND Tipo_Generazione = " & Agro_SQL_SaveNum(Tipo_Generazione) & " ")
            End If

            If Codice_Generazione <> 0 Then
                strSql.AppendLine(" AND Codice_Generazione = " & Agro_SQL_SaveNum(Codice_Generazione) & " ")
            End If

            If Elem_Cod <> 0 Then
                strSql.AppendLine(" AND Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & " ")
            End If

            If Mat_Cod <> 0 Then
                strSql.AppendLine(" AND Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & " ")
            End If

            If Key1 <> 0 Then
                strSql.AppendLine(" AND Key1 = " & Agro_SQL_SaveNum(Key1) & " ")
            End If

            If ID_Generazione <> 0 Then
                strSql.AppendLine(" AND ID_Generazione = " & Agro_SQL_SaveNum(ID_Generazione) & " ")
            End If

            If Colore_Log <> 0 Then
                strSql.AppendLine(" AND Colore_Log = " & Agro_SQL_SaveNum(Colore_Log) & " ")
            End If

            If Categoria_Gias_Cod_Log <> 0 Then
                strSql.AppendLine(" AND Categoria_Gias_Cod_Log = " & Agro_SQL_SaveNum(Categoria_Gias_Cod_Log) & " ")
            End If

            If Classificazione_Gias_Cod_Log <> 0 Then
                strSql.AppendLine(" AND Classificazione_Gias_Cod_Log = " & Agro_SQL_SaveNum(Classificazione_Gias_Cod_Log) & " ")
            End If

            If Preparazione_Cod_Rif_Log <> 0 Then
                strSql.AppendLine(" AND Preparazione_Cod_Rif_Log = " & Agro_SQL_SaveNum(Preparazione_Cod_Rif_Log) & " ")
            End If

            If Livello_Log <> 0 Then
                strSql.AppendLine(" AND Livello_Log = " & Agro_SQL_SaveNum(Livello_Log) & " ")
            End If

            If Veg_Cod_Log <> 0 Then
                strSql.AppendLine(" AND Veg_Cod_Log = " & Agro_SQL_SaveNum(Veg_Cod_Log) & " ")
            End If

            If Cul_Cod_Log <> 0 Then
                strSql.AppendLine(" AND Cul_Cod_Log = " & Agro_SQL_SaveNum(Cul_Cod_Log) & " ")
            End If

            If Gen_Cod_Log <> 0 Then
                strSql.AppendLine(" AND Gen_Cod_Log = " & Agro_SQL_SaveNum(Gen_Cod_Log) & " ")
            End If

            If Raz_Cod_Log <> 0 Then
                strSql.AppendLine(" AND Raz_Cod_Log = " & Agro_SQL_SaveNum(Raz_Cod_Log) & " ")
            End If

            If Spe_Cod_Log <> 0 Then
                strSql.AppendLine(" AND Spe_Cod_Log = " & Agro_SQL_SaveNum(Spe_Cod_Log) & " ")
            End If

            If Reg_Cod_Log <> 0 Then
                strSql.AppendLine(" AND Reg_Cod_Log = " & Agro_SQL_SaveNum(Reg_Cod_Log) & " ")
            End If

            If Grfi_Cod_Log <> 0 Then
                strSql.AppendLine(" AND Grfi_Cod_Log = " & Agro_SQL_SaveNum(Grfi_Cod_Log) & " ")
            End If

            If Dicitura_Gias_Cod_Log <> 0 Then
                strSql.AppendLine(" AND Dicitura_Gias_Cod_Log = " & Agro_SQL_SaveNum(Dicitura_Gias_Cod_Log) & " ")
            End If

            If Deno_Gias_Cod_Log <> 0 Then
                strSql.AppendLine(" AND Deno_Gias_Cod_Log = " & Agro_SQL_SaveNum(Deno_Gias_Cod_Log) & " ")
            End If

            If ChkScollegamento <> 0 Then
                strSql.AppendLine(" AND ChkScollegamento = " & Agro_SQL_SaveNum(ChkScollegamento) & " ")
            End If

            If Caratteristica_Gias_Cod_Log <> 0 Then
                strSql.AppendLine(" AND Caratteristica_Gias_Cod_Log = " & Agro_SQL_SaveNum(Caratteristica_Gias_Cod_Log) & " ")
            End If

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