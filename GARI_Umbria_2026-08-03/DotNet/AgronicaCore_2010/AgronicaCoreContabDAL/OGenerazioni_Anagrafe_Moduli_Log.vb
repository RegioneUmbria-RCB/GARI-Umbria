Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework_POCO

Public Class OGenerazioni_Anagrafe_Moduli_Log_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '###################################################################################
    Public Sub Recupera_Modulo_Cliente(ByVal Piva As String,
                                       ByRef Modulo_Cantine As Boolean,
                                       ByRef Modulo_FreshFood As Boolean,
                                       ByRef Modulo_Tabacco As Boolean,
                                       ByRef Modulo_Zoo As Boolean,
                                       ByRef objParametri As AgronicaCoreParametri)

        Const nomeRoutine = "AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R.Recupera_Modulo_Cliente()"
        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim i As Integer

        Modulo_Cantine = False
        Modulo_FreshFood = False
        Modulo_Tabacco = False
        Modulo_Zoo = False

        Try

            dt = Leggi(Piva, 0, 0, 0, 0,
                       -1, -1, -1,
                       AGRODATAINIZIO, AGRODATAFINE,
                       "", "", objParametri)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                For i = 0 To dt.Rows.Count - 1

                    Select Case dt.Rows(i).Item("Modulo_Generazione")
                        Case enum_Omni_Modulo_Generazione.Cantine
                            Modulo_Cantine = True
                        Case enum_Omni_Modulo_Generazione.FreshFood
                            Modulo_FreshFood = True
                        Case enum_Omni_Modulo_Generazione.Tabacco
                            Modulo_Tabacco = True
                        Case enum_Omni_Modulo_Generazione.Zoo
                            Modulo_Zoo = True
                    End Select

                Next

            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

    '###################################################################################
    Public Function Recupera_Moduli_Cliente(ByVal Piva As String,
                                            ByRef objParametri As AgronicaCoreParametri
                                            ) As List(Of Integer)

        Const nomeRoutine = "CoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R.Recupera_Moduli_Cliente()"
        Dim messaggioErrore As String = ""
        Dim elencoModuli As New List(Of Integer)

        Try

            Dim dt As DataTable = Leggi(Piva, 0, 0, 0, 0,
                                        -1, -1, -1,
                                        AGRODATAINIZIO, AGRODATAFINE,
                                        "", "", objParametri)
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                For Each rAnagrafeLog In dt.Rows
                    elencoModuli.Add(CInt(rAnagrafeLog.Item("Modulo_Generazione")))
                Next
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return elencoModuli

    End Function

    '###################################################################################
    Public Function IsFreshAndFood(ByVal Piva As String,
                                   ByVal Elem_Cod As Integer,
                                   ByRef objParametri As AgronicaCoreParametri
                                   ) As Boolean

        Const nomeRoutine = "CoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R.IsFreshAndFood()"
        Dim messaggioErrore As String = ""
        Dim isFreshAndFoodBool As Boolean = False

        Try

            Dim listaElemCodFF As New List(Of Integer) From {SEMILAVORATI_VEGETALI, TRASFORMATI_VEGETALI, TRASFORMATI_ANIMALI}
            Dim listaModFF As New List(Of Integer) From {
                    enum_Omni_Modulo_Generazione.FreshFood,
                    enum_Omni_Modulo_Generazione.Tabacco,
                    enum_Omni_Modulo_Generazione.Zoo
            }

            If Elem_Cod = 0 OrElse listaElemCodFF.Contains(Elem_Cod) Then

                'Dim Modulo_Cantine = False
                'Dim Modulo_FreshFood = False
                'Dim Modulo_Tabacco = False
                'Dim Modulo_Zoo = False

                'Recupera_Modulo_Cliente(Piva,
                '                        Modulo_Cantine, Modulo_FreshFood, Modulo_Tabacco, Modulo_Zoo,
                '                        objParametri)

                'If Modulo_FreshFood OrElse Modulo_Tabacco OrElse Modulo_Zoo Then
                '    isFreshAndFoodBool = True
                'End If

                Dim elencoModuli As List(Of Integer) = Recupera_Moduli_Cliente(Piva, objParametri)

                If elencoModuli.Exists(Function(modulo) listaModFF.Contains(modulo)) Then
                    isFreshAndFoodBool = True
                End If

            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return isFreshAndFoodBool

    End Function

    '###################################################################################
    Public Function FF_GestioneCodiceEsterno_0No_1Si(ByVal Piva As String,
                                                     ByRef objParametri As AgronicaCoreParametri
                                                     ) As Integer

        Const nomeRoutine = "AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R.FF_GestioneCodiceEsterno_0No_1Si()"
        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim Flag01 As Integer = 0

        Try

            dt = Leggi(Piva, 0, enum_Omni_Modulo_Generazione.FreshFood,
                       0, 0,
                       -1, -1, -1,
                       AGRODATAINIZIO, AGRODATAFINE,
                       " ChkCodiceEsterno = 1 ",
                       "", objParametri)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                Flag01 = 1
            Else
                Flag01 = 0
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return Flag01

    End Function

    '##############################################################################################
    ' default dei check: -1
    Public Function Leggi(ByVal Piva As String,
                          ByVal Sa_Cod As Integer,
                          ByVal Modulo_Generazione As Integer,
                          ByVal Tipo_Denominazione As Integer,
                          ByVal Tipo_Raggruppamento As Integer,
                          ByVal ChkData_Default As Integer,
                          ByVal ChkDes_Lib_Destinazione As Integer,
                          ByVal ChkTerzi As Integer,
                          ByVal Validita_Inizio As Date,
                          ByVal Validita_Fine As Date,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R.Leggi()"
        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
            strSql.AppendLine(" SELECT * ")
            strSql.AppendLine(" FROM  OGenerazioni_Anagrafe_Moduli_Log ")
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

            If Modulo_Generazione <> 0 Then
                strSql.AppendLine(" AND Modulo_Generazione = " & Agro_SQL_SaveNum(Modulo_Generazione) & "   ")
            End If

            If Tipo_Denominazione <> 0 Then
                strSql.AppendLine(" AND Tipo_Denominazione = " & Agro_SQL_SaveNum(Tipo_Denominazione) & "   ")
            End If

            If Tipo_Raggruppamento <> 0 Then
                strSql.AppendLine(" AND Tipo_Raggruppamento = " & Agro_SQL_SaveNum(Tipo_Raggruppamento) & " ")
            End If

            If ChkData_Default <> -1 Then
                strSql.AppendLine(" AND ChkData_Default = " & Agro_SQL_SaveNum(ChkData_Default) & " ")
            End If

            If ChkDes_Lib_Destinazione <> -1 Then
                strSql.AppendLine(" AND ChkDes_Lib_Destinazione = " & Agro_SQL_SaveNum(ChkDes_Lib_Destinazione) & " ")
            End If

            If ChkTerzi <> -1 Then
                strSql.AppendLine(" AND ChkTerzi = " & Agro_SQL_SaveNum(ChkTerzi) & " ")
            End If


            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   OGenerazioni_Anagrafe_Moduli_Log.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   OGenerazioni_Anagrafe_Moduli_Log.Inviato =-1 ")
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
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '################################################################################
    Public Function LeggiModuliParametriQualitativi(ByVal Piva As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R.LeggiModuliParametriQualitativi()"
        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As New DataTable

        Try
            Dim filtro As String = $"(Modulo_Generazione = {CInt(enum_Omni_Modulo_Generazione.FreshFood)} "
            filtro += $"Or Modulo_Generazione = {CInt(enum_Omni_Modulo_Generazione.Zoo)})"

            '--------------------------------------------------------------------------
            Dim dtAnagrafeLog As DataTable = Leggi(Piva, 0, 0, 0, 0, -1, -1, -1, AGRODATAINIZIO, AGRODATAFINE, filtro, "", objParametri)
            '--------------------------------------------------------------------------

            dt.Columns.Add("Modulo_Cod", GetType(Integer))
            dt.Columns.Add("Modulo_Des", GetType(String))

            If Not IsNothing(dtAnagrafeLog) Then
                ModuloAttivo(dtAnagrafeLog, enum_Omni_Modulo_Generazione.FreshFood, "FreshFood", dt)
                ModuloAttivo(dtAnagrafeLog, enum_Omni_Modulo_Generazione.Zoo, "Zootecnia", dt)
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "]  " & messaggioErrore)
        End Try

        Return dt

    End Function

    '################################################################################
    Public Function LeggiModuliConferimento(ByVal Piva As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R.LeggiModuliConferimento()"
        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As New DataTable

        Try
            Dim filtro As String = $"(Modulo_Generazione = {CInt(enum_Omni_Modulo_Generazione.FreshFood)} "
            filtro += $"Or Modulo_Generazione = {CInt(enum_Omni_Modulo_Generazione.Zoo)})"

            '--------------------------------------------------------------------------
            Dim dtAnagrafeLog As DataTable = Leggi(Piva, 0, 0, 0, 0, -1, -1, -1, AGRODATAINIZIO, AGRODATAFINE, filtro, "", objParametri)
            '--------------------------------------------------------------------------

            dt.Columns.Add("Modulo_Cod", GetType(Integer))
            dt.Columns.Add("Modulo_Des", GetType(String))

            If Not IsNothing(dtAnagrafeLog) Then
                ModuloAttivo(dtAnagrafeLog, enum_Omni_Modulo_Generazione.FreshFood, "Trasformati Vegetali", dt)
                ModuloAttivo(dtAnagrafeLog, enum_Omni_Modulo_Generazione.Zoo, "Trasformati Animali", dt)
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "]  " & messaggioErrore)
        End Try

        Return dt

    End Function

    '################################################################################
    Public Function Esiste_GestioneContoTerzi_RegCantina(ByVal piva As String,
                                                         ByVal saCod As Integer,
                                                         ByRef objParametri As AgronicaCoreParametri
                                                         ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Log_R.Esiste_GestioneContoTerzi_RegCantina()"
        Dim messaggioErrore As String
        Dim flagContoTerzi As Boolean = False
        Dim dt As DataTable

        Try

            'cerco se nel modulo cantine il check conto terzi è a 1
            dt = Leggi(piva, saCod,
                       enum_Omni_Modulo_Generazione.Cantine,
                       0, 0,
                       -1, -1, 1,
                       AGRODATAINIZIO,
                       AGRODATAFINE,
                       "", "", objParametri)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                flagContoTerzi = True
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "]  " & messaggioErrore)
        End Try

        Return flagContoTerzi

    End Function

    '################################################################################
    Public Function TipoArrotondamentoFF(ByVal piva As String,
                                         ByVal saCod As Integer,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As enum_TipoArrotondamentoFF

        Const nomeRoutine = "AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Log_R.TipoArrotondamentoFF()"
        Dim messaggioErrore As String
        Dim tipoArrotondamento As enum_TipoArrotondamentoFF = enum_TipoArrotondamentoFF.Nessuno
        Dim dt As DataTable

        Try

            'cerco qual è nel modulo FF il tipo di arrotondamento
            dt = Leggi(piva, saCod,
                       enum_Omni_Modulo_Generazione.FreshFood,
                       0, 0,
                       -1, -1, -1,
                       AGRODATAINIZIO,
                       AGRODATAFINE,
                       "", "", objParametri)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                tipoArrotondamento = dt.Rows(0).Item("Tipo_Arrotondamento")
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "]  " & messaggioErrore)
        End Try

        Return tipoArrotondamento

    End Function

    Public Function Leggi_Join_Con_OGenerazioni_Anagrafe_Moduli(ByVal Piva_SuperUser As String,
                                                                ByVal Piva As String,
                                                                ByVal Sa_Cod As Integer,
                                                                ByVal Modulo_Generazione As String,
                                                                ByVal xFiltroAggiuntivo As String,
                                                                ByVal xOrderBy As String,
                                                                ByRef objParametri As AgronicaCoreParametri
                                                                ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R.Leggi_Join_Con_OGenerazioni_Anagrafe_Moduli()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT * ")
            strSql.AppendLine(" FROM  OGenerazioni_Anagrafe_Moduli, OGenerazioni_Anagrafe_Moduli_Log ")
            strSql.AppendLine(" WHERE OGenerazioni_Anagrafe_Moduli_Log.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            strSql.AppendLine(" And   OGenerazioni_Anagrafe_Moduli_Log.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            strSql.AppendLine(" And   OGenerazioni_Anagrafe_Moduli_Log.Modulo_Generazione = OGenerazioni_Anagrafe_Moduli.Modulo_Generazione ")

            'Il superuser è opzionale per impostare la picture di accesso
            If Piva_SuperUser <> "" Then
                strSql.AppendLine("And OGenerazioni_Anagrafe_Moduli_Log.Piva_SuperUser = '" & Agro_SQL_SaveText(Piva_SuperUser) & "'   ")
            End If

            If Piva <> "" Then
                strSql.AppendLine("AND OGenerazioni_Anagrafe_Moduli_Log.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND OGenerazioni_Anagrafe_Moduli_Log.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Not String.IsNullOrEmpty(Modulo_Generazione) Then
                strSql.AppendLine(" And OGenerazioni_Anagrafe_Moduli_Log.Modulo_Generazione IN (" & Agro_SQL_Save_Clausola_IN(Modulo_Generazione) & ") ")
            End If


            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Piva_SuperUser, Piva, OGenerazioni_Anagrafe_Moduli.Modulo_Descrizione ASC")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Private Sub ModuloAttivo(dtAnagrafeLog As DataTable, codiceModuloGenerazione As enum_Omni_Modulo_Generazione,
                            descrizione As String, DT As DataTable)
        Dim result As Object() = Nothing

        Dim attivo As Boolean = dtAnagrafeLog.AsEnumerable() _
            .Any(Function(x) IsNumeric(x.Item("Modulo_Generazione")) AndAlso
                x.Item("Modulo_Generazione") = CInt(codiceModuloGenerazione))

        If attivo Then
            DT.Rows.Add(CInt(codiceModuloGenerazione), descrizione)
        End If

    End Sub

End Class

Public Class OGenerazioni_Anagrafe_Moduli_Log_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal Piva As String,
                           ByVal Sa_Cod As Long,
                           ByVal Modulo_Generazione As Integer,
                           ByVal Tipo_Denominazione As Integer,
                           ByVal Tipo_Raggruppamento As Integer,
                           ByVal ChkData_Default As Integer,
                           ByVal ChkDes_Lib_Destinazione As Integer,
                           ByVal inviato As Integer,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByVal ChkTerzi As Integer,
                           ByVal ChkAllerta_MP As Integer,
                           ByVal ChkConsistenza_Statica As Integer,
                           ByVal Tipo_Lotto_Scarti As Integer,
                           ByVal ChkAbilitazione_Validita As Integer,
                           ByVal Lotto_Configurazione As String,
                           ByVal Zona_Viticola As String,
                           ByVal Tabella_Cod_Metaschema As Integer,
                           ByVal Separatore_Lotto As String,
                           ByVal Tipo_Arrotondamento As Integer,
                           ByVal Udm_Cod_Prezzo As Integer,
                           ByVal ChkCodiceEsterno As Integer,
                           ByVal ChkGenerazione_Privata As Integer,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal datainvio As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Log_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            Dim leggiAnagrafeLog As New OGenerazioni_Anagrafe_Moduli_Log_R
            Dim dtAnagrafeLog = leggiAnagrafeLog.Leggi(Piva, Sa_Cod, Modulo_Generazione, Tipo_Denominazione,
                                                       Tipo_Raggruppamento, ChkData_Default, ChkDes_Lib_Destinazione,
                                                       ChkTerzi, AGRODATAINIZIO, AGRODATAFINE, "", "",
                                                       objParametri)

            If dtAnagrafeLog IsNot Nothing AndAlso dtAnagrafeLog.AsEnumerable().Any() Then
                xRisp = Modifica(Piva, Sa_Cod, Modulo_Generazione, Tipo_Denominazione, Tipo_Raggruppamento, ChkData_Default,
                         ChkDes_Lib_Destinazione, ChkTerzi, ChkAllerta_MP, ChkConsistenza_Statica, Tipo_Lotto_Scarti,
                         ChkAbilitazione_Validita, Lotto_Configurazione, Zona_Viticola, Tabella_Cod_Metaschema, Separatore_Lotto,
                         Tipo_Arrotondamento, Udm_Cod_Prezzo, ChkCodiceEsterno, ChkGenerazione_Privata, objParametri,
                         Data_modifica, username_modifica)
            Else
                xRisp = Aggiungi(Piva, Sa_Cod, Modulo_Generazione, Tipo_Denominazione, Tipo_Raggruppamento, ChkData_Default,
                         ChkDes_Lib_Destinazione, inviato, ChkTerzi, ChkAllerta_MP, ChkConsistenza_Statica, Tipo_Lotto_Scarti,
                         ChkAbilitazione_Validita, Lotto_Configurazione, Zona_Viticola, Tabella_Cod_Metaschema, Separatore_Lotto,
                         Tipo_Arrotondamento, Udm_Cod_Prezzo, ChkCodiceEsterno, ChkGenerazione_Privata, objParametri,
                         Data_creazione, Data_modifica, datainvio, username_creazione, username_modifica)
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function Scrivi_AttivazioneModuloFreshAndFood(ByVal Piva As String,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal lottoConfigurazione As String = "",
                           Optional ByVal separatoreLotto As String = ""
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Log_W.Scrivi_AttivazioneModuloFreshAndFood()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            xRisp = Scrivi(Piva, 0, enum_Omni_Modulo_Generazione.FreshFood, 1, 1, 0, 0, 0,
                           AGRODATAINIZIO, AGRODATAFINE, 0, 0, 0, 0, 0, lottoConfigurazione,
                           "", 0, separatoreLotto, 0, 5, 0, 0, objParametri)

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function Scrivi_AttivazioneModuloZoo(ByVal Piva As String,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal lottoConfigurazione As String = "",
                           Optional ByVal separatoreLotto As String = ""
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Log_W.Scrivi_AttivazioneModuloZoo()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            xRisp = Scrivi(Piva, 0, enum_Omni_Modulo_Generazione.Zoo, 1, 1, 0, 0, 0,
                           AGRODATAINIZIO, AGRODATAFINE, 0, 0, 0, 0, 0, lottoConfigurazione,
                           "", 0, separatoreLotto, 0, 5, 0, 0, objParametri)

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function Cancella_AttivazioneModuloFreshAndFood(ByVal Piva As String,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Log_W.Scrivi_AttivazioneModuloFreshAndFood()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            xRisp = Cancella(Piva, 0, enum_Omni_Modulo_Generazione.FreshFood, objParametri)

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function Cancella_AttivazioneModuloZoo(ByVal Piva As String,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Log_W.Scrivi_AttivazioneModuloZoo()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            xRisp = Cancella(Piva, 0, enum_Omni_Modulo_Generazione.Zoo, objParametri)

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Private Function Cancella(ByVal Piva As String,
                           ByVal Sa_Cod As Long,
                           ByVal Modulo_Generazione As Integer,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Tipo_Denominazione As Integer = -1,
                           Optional ByVal Tipo_Raggruppamento As Integer = -1,
                           Optional ByVal ChkData_Default As Integer = -1,
                           Optional ByVal ChkDes_Lib_Destinazione As Integer = -1,
                           Optional ByVal ChkTerzi As Integer = -1,
                           Optional ByVal ChkAllerta_MP As Integer = -1,
                           Optional ByVal ChkConsistenza_Statica As Integer = -1,
                           Optional ByVal Tipo_Lotto_Scarti As Integer = -1,
                           Optional ByVal ChkAbilitazione_Validita As Integer = -1,
                           Optional ByVal Lotto_Configurazione As String = "",
                           Optional ByVal Zona_Viticola As String = "",
                           Optional ByVal Tabella_Cod_Metaschema As Integer = -1,
                           Optional ByVal Separatore_Lotto As String = "",
                           Optional ByVal Tipo_Arrotondamento As Integer = -1,
                           Optional ByVal Udm_Cod_Prezzo As Integer = -1,
                           Optional ByVal ChkCodiceEsterno As Integer = -1,
                           Optional ByVal ChkGenerazione_Privata As Integer = -1
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Log_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" DELETE FROM [dbo].[OGenerazioni_Anagrafe_Moduli_Log] ")
            strSql.AppendLine("      WHERE 1=1 ")

            If Piva <> "" Then
                strSql.AppendLine(" AND Piva = " & Agro_SQL_SaveText_NULL(Piva) & " ")
            End If

            If Sa_Cod > -1 Then
                strSql.AppendLine(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Modulo_Generazione > -1 Then
                strSql.AppendLine(" AND Modulo_Generazione = " & Agro_SQL_SaveNum(Modulo_Generazione) & " ")
            End If

            If Tipo_Denominazione > -1 Then
                strSql.AppendLine(" AND Tipo_Denominazione = " & Agro_SQL_SaveNum(Tipo_Denominazione) & " ")
            End If

            If Tipo_Raggruppamento > -1 Then
                strSql.AppendLine(" AND Tipo_Raggruppamento = " & Agro_SQL_SaveNum(Tipo_Raggruppamento) & " ")
            End If

            If ChkData_Default > -1 Then
                strSql.AppendLine(" AND ChkData_Default = " & Agro_SQL_SaveNum(ChkData_Default) & " ")
            End If

            If ChkDes_Lib_Destinazione > -1 Then
                strSql.AppendLine(" AND ChkDes_Lib_Destinazione = " & Agro_SQL_SaveNum(ChkDes_Lib_Destinazione) & " ")
            End If

            If ChkTerzi > -1 Then
                strSql.AppendLine(" AND ChkTerzi = " & Agro_SQL_SaveNum(ChkTerzi) & " ")
            End If

            If ChkAllerta_MP > -1 Then
                strSql.AppendLine(" AND ChkAllerta_MP = " & Agro_SQL_SaveNum(ChkAllerta_MP) & " ")
            End If

            If ChkConsistenza_Statica > -1 Then
                strSql.AppendLine(" AND ChkConsistenza_Statica = " & Agro_SQL_SaveNum(ChkConsistenza_Statica) & " ")
            End If

            If Tipo_Lotto_Scarti > -1 Then
                strSql.AppendLine(" AND Tipo_Lotto_Scarti = " & Agro_SQL_SaveNum(Tipo_Lotto_Scarti) & " ")
            End If

            If ChkAbilitazione_Validita > -1 Then
                strSql.AppendLine(" AND ChkAbilitazione_Validita = " & Agro_SQL_SaveNum(ChkAbilitazione_Validita) & " ")
            End If

            If Lotto_Configurazione <> "" Then
                strSql.AppendLine(" AND Lotto_Configurazione = " & Agro_SQL_SaveText(Lotto_Configurazione) & " ")
            End If

            If Zona_Viticola <> "" Then
                strSql.AppendLine(" AND Zona_Viticola = " & Agro_SQL_SaveText(Zona_Viticola) & " ")
            End If

            If Tabella_Cod_Metaschema > -1 Then
                strSql.AppendLine(" AND Tabella_Cod_Metaschema = " & Agro_SQL_SaveNum(Tabella_Cod_Metaschema) & " ")
            End If

            If Separatore_Lotto <> "" Then
                strSql.AppendLine(" AND Separatore_Lotto = " & Agro_SQL_SaveText(Separatore_Lotto) & " ")
            End If

            If Tipo_Arrotondamento > -1 Then
                strSql.AppendLine(" AND Tipo_Arrotondamento = " & Agro_SQL_SaveNum(Tipo_Arrotondamento) & " ")
            End If

            If Udm_Cod_Prezzo > -1 Then
                strSql.AppendLine(" AND Udm_Cod_Prezzo = " & Agro_SQL_SaveNum(Udm_Cod_Prezzo) & " ")
            End If

            If ChkCodiceEsterno > -1 Then
                strSql.AppendLine(" AND ChkCodiceEsterno = " & Agro_SQL_SaveNum(ChkCodiceEsterno) & " ")
            End If

            If ChkGenerazione_Privata > -1 Then
                strSql.AppendLine(" AND ChkGenerazione_Privata = " & Agro_SQL_SaveNum(ChkGenerazione_Privata) & " ")
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

    Private Function Aggiungi(ByVal Piva As String,
                           ByVal Sa_Cod As Long,
                           ByVal Modulo_Generazione As Integer,
                           ByVal Tipo_Denominazione As Integer,
                           ByVal Tipo_Raggruppamento As Integer,
                           ByVal ChkData_Default As Integer,
                           ByVal ChkDes_Lib_Destinazione As Integer,
                           ByVal inviato As Integer,
                           ByVal ChkTerzi As Integer,
                           ByVal ChkAllerta_MP As Integer,
                           ByVal ChkConsistenza_Statica As Integer,
                           ByVal Tipo_Lotto_Scarti As Integer,
                           ByVal ChkAbilitazione_Validita As Integer,
                           ByVal Lotto_Configurazione As String,
                           ByVal Zona_Viticola As String,
                           ByVal Tabella_Cod_Metaschema As Integer,
                           ByVal Separatore_Lotto As String,
                           ByVal Tipo_Arrotondamento As Integer,
                           ByVal Udm_Cod_Prezzo As Integer,
                           ByVal ChkCodiceEsterno As Integer,
                           ByVal ChkGenerazione_Privata As Integer,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal datainvio As DateTime = #2/1/1900#,
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

            strSql.AppendLine(" INSERT INTO [dbo].[OGenerazioni_Anagrafe_Moduli_Log] ")
            strSql.AppendLine("            ([Piva_SuperUser] ")
            strSql.AppendLine("            ,[Piva] ")
            strSql.AppendLine("            ,[Sa_Cod] ")
            strSql.AppendLine("            ,[Modulo_Generazione] ")
            strSql.AppendLine("            ,[Tipo_Denominazione] ")
            strSql.AppendLine("            ,[Tipo_Raggruppamento] ")
            strSql.AppendLine("            ,[ChkData_Default] ")
            strSql.AppendLine("            ,[ChkDes_Lib_Destinazione] ")
            strSql.AppendLine("            ,[inviato] ")

            If datainvio <> #2/1/1900# Then
                strSql.AppendLine("            ,[datainvio] ")
            End If

            strSql.AppendLine("            ,[Data_Creazione] ")
            strSql.AppendLine("            ,[Data_Modifica] ")
            strSql.AppendLine("            ,[Username_Creazione] ")
            strSql.AppendLine("            ,[Username_Modifica] ")
            strSql.AppendLine("            ,[Validita_Inizio] ")
            strSql.AppendLine("            ,[Validita_Fine] ")
            strSql.AppendLine("            ,[ChkTerzi] ")
            strSql.AppendLine("            ,[ChkAllerta_MP] ")
            strSql.AppendLine("            ,[ChkConsistenza_Statica] ")
            strSql.AppendLine("            ,[Tipo_Lotto_Scarti] ")
            strSql.AppendLine("            ,[ChkAbilitazione_Validita] ")
            strSql.AppendLine("            ,[Lotto_Configurazione] ")
            strSql.AppendLine("            ,[Zona_Viticola] ")
            strSql.AppendLine("            ,[Tabella_Cod_Metaschema] ")
            strSql.AppendLine("            ,[Separatore_Lotto] ")
            strSql.AppendLine("            ,[Tipo_Arrotondamento] ")
            strSql.AppendLine("            ,[Udm_Cod_Prezzo] ")
            strSql.AppendLine("            ,[Validita_Inizio_Teleregistri] ")
            strSql.AppendLine("            ,[Validita_Fine_Teleregistri] ")
            strSql.AppendLine("            ,[ChkCodiceEsterno] ")
            strSql.AppendLine("            ,[ChkGenerazione_Privata]) ")
            strSql.AppendLine("      VALUES ")
            strSql.AppendLine("            (" & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveText_NULL(Piva) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveNum(Modulo_Generazione) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveNum(Tipo_Denominazione) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveNum(Tipo_Raggruppamento) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveNum(ChkData_Default) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveNum(ChkDes_Lib_Destinazione) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveNum(inviato) & " ")

            If datainvio <> #2/1/1900# Then
                strSql.AppendLine("            , " & Agro_SQL_SaveDate(datainvio) & " ")
            End If

            strSql.AppendLine("            , " & Agro_SQL_SaveDateTime(Data_creazione) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveDateTime(Data_modifica) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveText_NULL(username_creazione) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveText_NULL(username_modifica) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveDate(AGRODATAINIZIO) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveDate(AGRODATAFINE) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveNum(ChkTerzi) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveNum(ChkAllerta_MP) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveNum(ChkConsistenza_Statica) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveNum(Tipo_Lotto_Scarti) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveNum(ChkAbilitazione_Validita) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveText_NULL(Lotto_Configurazione) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveText_NULL(Zona_Viticola) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveNum(Tabella_Cod_Metaschema) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveText_NULL(Separatore_Lotto) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveNum(Tipo_Arrotondamento) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveNum(Udm_Cod_Prezzo) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveDateTime(AGRODATAINIZIO) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveDateTime(AGRODATAFINE) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveNum(ChkCodiceEsterno) & " ")
            strSql.AppendLine("            , " & Agro_SQL_SaveNum(ChkGenerazione_Privata) & " ) ")

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

    Private Function Modifica(ByVal Piva As String,
                           ByVal Sa_Cod As Long,
                           ByVal Modulo_Generazione As Integer,
                           ByVal Tipo_Denominazione As Integer,
                           ByVal Tipo_Raggruppamento As Integer,
                           ByVal ChkData_Default As Integer,
                           ByVal ChkDes_Lib_Destinazione As Integer,
                           ByVal ChkTerzi As Integer,
                           ByVal ChkAllerta_MP As Integer,
                           ByVal ChkConsistenza_Statica As Integer,
                           ByVal Tipo_Lotto_Scarti As Integer,
                           ByVal ChkAbilitazione_Validita As Integer,
                           ByVal Lotto_Configurazione As String,
                           ByVal Zona_Viticola As String,
                           ByVal Tabella_Cod_Metaschema As Integer,
                           ByVal Separatore_Lotto As String,
                           ByVal Tipo_Arrotondamento As Integer,
                           ByVal Udm_Cod_Prezzo As Integer,
                           ByVal ChkCodiceEsterno As Integer,
                           ByVal ChkGenerazione_Privata As Integer,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Log_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE [dbo].[OGenerazioni_Anagrafe_Moduli_Log] ")
            strSql.AppendLine(" SET [Data_Modifica] = " & Agro_SQL_SaveDateTime(Data_modifica) & " ")
            strSql.AppendLine("     ,[Username_Modifica] = " & Agro_SQL_SaveText_NULL(username_modifica) & " ")

            If Tipo_Denominazione > 0 Then
                strSql.AppendLine("     ,[Tipo_Denominazione] = " & Agro_SQL_SaveNum(Tipo_Denominazione) & " ")
            End If

            If Tipo_Raggruppamento > 0 Then
                strSql.AppendLine("     ,[Tipo_Raggruppamento] = " & Agro_SQL_SaveNum(Tipo_Raggruppamento) & " ")
            End If

            If ChkData_Default > -1 Then
                strSql.AppendLine("     ,[ChkData_Default] = " & Agro_SQL_SaveNum(ChkData_Default) & " ")
            End If

            If ChkDes_Lib_Destinazione > -1 Then
                strSql.AppendLine("     ,[ChkDes_Lib_Destinazione] = " & Agro_SQL_SaveNum(ChkDes_Lib_Destinazione) & " ")
            End If

            If ChkTerzi > -1 Then
                strSql.AppendLine("     ,[ChkTerzi] = " & Agro_SQL_SaveNum(ChkTerzi) & " ")
            End If

            If ChkAllerta_MP > -1 Then
                strSql.AppendLine("     ,[ChkAllerta_MP] = " & Agro_SQL_SaveNum(ChkAllerta_MP) & " ")
            End If

            If ChkConsistenza_Statica > -1 Then
                strSql.AppendLine("     ,[ChkConsistenza_Statica] = " & Agro_SQL_SaveNum(ChkConsistenza_Statica) & " ")
            End If

            If Tipo_Lotto_Scarti > -1 Then
                strSql.AppendLine("     ,[Tipo_Lotto_Scarti] = " & Agro_SQL_SaveNum(Tipo_Lotto_Scarti) & " ")
            End If

            If ChkAbilitazione_Validita > -1 Then
                strSql.AppendLine("     ,[ChkAbilitazione_Validita] = " & Agro_SQL_SaveNum(ChkAbilitazione_Validita) & " ")
            End If

            If Not String.IsNullOrEmpty(Lotto_Configurazione) Then
                strSql.AppendLine("     ,[Lotto_Configurazione] = " & Agro_SQL_SaveText_NULL(Lotto_Configurazione) & " ")
            End If

            If Not String.IsNullOrEmpty(Zona_Viticola) Then
                strSql.AppendLine("     ,[Zona_Viticola] = " & Agro_SQL_SaveText_NULL(Zona_Viticola) & " ")
            End If

            If Tabella_Cod_Metaschema > -1 Then
                strSql.AppendLine("     ,[Tabella_Cod_Metaschema] = " & Agro_SQL_SaveNum(Tabella_Cod_Metaschema) & " ")
            End If

            If Not IsNothing(Separatore_Lotto) Then
                strSql.AppendLine("     ,[Separatore_Lotto] = " & Agro_SQL_SaveText_NULL(Separatore_Lotto) & " ")
            End If

            If Tipo_Arrotondamento > -1 Then
                strSql.AppendLine("     ,[Tipo_Arrotondamento] = " & Agro_SQL_SaveNum(Tipo_Arrotondamento) & " ")
            End If

            If Udm_Cod_Prezzo > -1 Then
                strSql.AppendLine("     ,[Udm_Cod_Prezzo] = " & Agro_SQL_SaveNum(Udm_Cod_Prezzo) & " ")
            End If

            If ChkCodiceEsterno > -1 Then
                strSql.AppendLine("     ,[ChkCodiceEsterno] = " & Agro_SQL_SaveNum(ChkCodiceEsterno) & " ")
            End If

            If ChkGenerazione_Privata > -1 Then
                strSql.AppendLine("     ,[ChkGenerazione_Privata] = " & Agro_SQL_SaveNum(ChkGenerazione_Privata) & " ")
            End If

            strSql.AppendLine(" WHERE [Piva_SuperUser] = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser) & " ")
            strSql.AppendLine(" AND [Piva] = " & Agro_SQL_SaveText_NULL(Piva) & " ")
            strSql.AppendLine(" AND [Sa_Cod] = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            strSql.AppendLine(" AND [Modulo_Generazione] = " & Agro_SQL_SaveNum(Modulo_Generazione) & " ")

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