Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class OModuli_Referenze_Config_Dettagli_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal piva As String,
                          ByVal id_Testata As Integer,
                          ByVal SoloCampiAbilitatiEtichetta As Boolean,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        '----- Descrizione
        Const nomeRoutine As String = "AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli.Leggi()"

        '----- Variabili
        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        'Per non cambiare tutti i chiamanti
        If id_Testata = 0 AndAlso xFiltroAggiuntivo.Contains("Tipo") AndAlso Not xFiltroAggiuntivo.Contains("OModuli_Referenze_Config_Dettagli.Tipo") Then
            xFiltroAggiuntivo = xFiltroAggiuntivo.Replace("Tipo", "OModuli_Referenze_Config_Dettagli.Tipo")
        End If

        Try

            stb.Length = 0
            stb.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
            If id_Testata <> 0 Then
                stb.AppendLine(" Select OModuli_Referenze_Config_Dettagli.* ")
            Else
                stb.AppendLine(" Select DISTINCT ")
                stb.AppendLine("   OModuli_Referenze_Config_Dettagli.Tabella_ID ")
                stb.AppendLine(" , OModuli_Referenze_Config_Dettagli.Tabella_Key ")
                stb.AppendLine(" , ISNULL(OTabelle.Tipo, 1) As Tipo ")
                stb.AppendLine(" , ISNULL(OTabelle.Tabella_Des, '') As Tabella_Des ")
                stb.AppendLine(" , ISNULL(OTabelle.NumDecimali_Maximo, 0) As NumDecimali_Maximo ")
            End If

            stb.AppendLine(" FROM OModuli_Referenze_Config_Dettagli  WITH(NOLOCK)")

            If id_Testata = 0 Then
                stb.AppendLine(" Left Join OTabelle On Convert(nvarchar, OTabelle.Tabella_Cod) = OModuli_Referenze_Config_Dettagli.Tabella_ID ")
            End If

            stb.AppendLine(" WHERE 1 = 1 ")
            stb.AppendLine(" AND OModuli_Referenze_Config_Dettagli.Piva = '" & Agro_SQL_SaveText(piva) & "' ")

            If id_Testata <> 0 Then
                stb.AppendLine(" And id_testata = " & id_Testata)
            End If

            If SoloCampiAbilitatiEtichetta Then
                stb.AppendLine(" And Tabella_Key <> 0 ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" And " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    stb.AppendLine(" And   OModuli_Referenze_Config_Dettagli.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    stb.AppendLine(" And   OModuli_Referenze_Config_Dettagli.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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


    '##############################################################################################
    Public Function ElencoOTabelleConfigurateSulVegCod(ByVal Piva As String,
                                                       ByVal Veg_Cod As Integer,
                                                       ByVal xFiltroAggiuntivo As String,
                                                       ByRef objParametri As AgronicaCoreParametri
                                                       ) As DataTable

        '----- Descrizione
        Const nomeRoutine As String = "AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli.ElencoOTabelleConfigurateSulVegCod()"

        '----- Variabili
        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT OTabelle.tabella_cod, OTabelle.Tipo, OTabelle.Tabella_Des, OTabelle.Tabella_Cod_Des ")
            stb.AppendLine(" FROM OModuli_Referenze_Config_Testata  " & vbCrLf)
            stb.AppendLine(" INNER JOIN OModuli_Referenze_Config_Dettagli ON  OModuli_Referenze_Config_Dettagli.piva = OModuli_Referenze_Config_Testata.piva ")
            stb.AppendLine(" AND  OModuli_Referenze_Config_Dettagli.id_testata = OModuli_Referenze_Config_Testata.id_testata ")
            stb.AppendLine(" INNER JOIN   OTabelle ON CONVERT(varchar(50),OTabelle.Tabella_cod) = OModuli_Referenze_Config_Dettagli.Tabella_id ")
            stb.AppendLine(" AND OModuli_Referenze_Config_Testata.modulo_generazione = OTabelle.modulo_generazione ")
            stb.AppendLine(" WHERE  OModuli_Referenze_Config_Testata.piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            stb.AppendLine(" AND (OModuli_Referenze_Config_Testata.OFiltro_Veg_Cod LIKE '%|" & Agro_SQL_SaveNum(Veg_Cod) & "|%' OR OModuli_Referenze_Config_Testata.OFiltro_Veg_Cod='') ")

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            stb.AppendLine(" ORDER BY ordine ")

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


    '##############################################################################################
    Public Function Recupera_DettagliEConfezionamento_Prodotto(ByVal Piva As String,
                                                               ByVal Veg_Cod As Integer,
                                                               ByVal Cal_Cod As Integer,
                                                               ByVal xFiltroAggiuntivo As String,
                                                               ByRef Imballaggio As String,
                                                               ByRef Contenitore As String,
                                                               ByRef Confezione As String,
                                                               ByRef objParametri As AgronicaCoreParametri
                                                               ) As String

        Const nomeRoutine As String = "AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli.Recupera_DettagliEConfezionamento_Prodotto()"
        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim strDettagli As String = ""

        Confezione = ""
        Contenitore = ""
        Imballaggio = ""

        Try

            dt = ElencoOTabelleConfigurateSulVegCod_JoinMPCampionature(Piva,
                                                                       Veg_Cod,
                                                                       Cal_Cod,
                                                                       "",
                                                                       objParametri)

            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                For i = 0 To dt.Rows.Count - 1
                    Select Case dt.Rows(i).Item("Tabella_Cod")
                        Case enum_OTabelle.Confezione
                            Confezione = dt.Rows(i).Item("Descrizione")
                        Case enum_OTabelle.Contenitore
                            Contenitore = dt.Rows(i).Item("Descrizione")
                        Case enum_OTabelle.Imballaggio
                            Imballaggio = dt.Rows(i).Item("Descrizione")
                        Case Else
                            strDettagli &= " - " & dt.Rows(i).Item("Descrizione")
                    End Select
                Next
                'strDettagli.Trim()
                'If strDettagli.Length > 2 Then
                '    strDettagli = Left(strDettagli, strDettagli.Length - 2)
                'End If
            End If

            dt = Nothing

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return strDettagli

    End Function

    '##############################################################################################
    'usa sigla al posto di descrizione
    Public Function Recupera_Dettagli_Prodotto_XStampa(ByVal Piva As String,
                                                       ByVal Veg_Cod As Integer,
                                                       ByVal Cal_Cod As Integer,
                                                       ByVal xFiltroAggiuntivo As String,
                                                       ByRef objParametri As AgronicaCoreParametri
                                                       ) As String

        Const nomeRoutine As String = "AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli.Recupera_Dettagli_Prodotto_XStampa()"
        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim strDettagli As String = ""

        Try

            dt = ElencoOTabelleConfigurateSulVegCod_JoinMPCampionature(Piva,
                                                                       Veg_Cod,
                                                                       Cal_Cod,
                                                                       xFiltroAggiuntivo,
                                                                       objParametri)

            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                For i = 0 To dt.Rows.Count - 1
                    Select Case dt.Rows(i).Item("Tabella_Cod")
                        Case enum_OTabelle.Confezione, enum_OTabelle.Contenitore, enum_OTabelle.Imballaggio
                        Case Else
                            strDettagli &= " - " & dt.Rows(i).Item("Sigla")
                    End Select
                Next
            End If

            dt = Nothing

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return strDettagli

    End Function

    '##############################################################################################
    'nel caso di stampa doc di vendita in xFiltroAggiuntivo passa il filtro su ChkEtichetta
    Public Function ElencoOTabelleConfigurateSulVegCod_JoinMPCampionature(
                                                    ByVal Piva As String,
                                                    ByVal Veg_Cod As Integer,
                                                    ByVal Cal_Cod As Integer,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByRef objParametri As AgronicaCoreParametri
                                                    ) As DataTable

        '----- Descrizione
        Const nomeRoutine As String = "AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli.ElencoOTabelleConfigurateSulVegCod_JoinMPCampionature()"

        '----- Variabili
        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT OModuli_Referenze_Config_Dettagli.ChkEtichetta, OTabelle_Parametri.* ")
            stb.AppendLine(" FROM  OModuli_Referenze_Config_Testata  ")
            stb.AppendLine(" INNER JOIN OModuli_Referenze_Config_Dettagli ON  OModuli_Referenze_Config_Dettagli.piva = OModuli_Referenze_Config_Testata.piva ")
            stb.AppendLine(" AND  OModuli_Referenze_Config_Dettagli.id_testata = OModuli_Referenze_Config_Testata.id_testata  ")
            stb.AppendLine(" INNER JOIN   OTabelle ON CONVERT(varchar(50),OTabelle.Tabella_cod) = OModuli_Referenze_Config_Dettagli.Tabella_id  ")
            stb.AppendLine(" AND OModuli_Referenze_Config_Testata.modulo_generazione = OTabelle.modulo_generazione ")
            stb.AppendLine(" INNER JOIN Materie_Prime_Campionature ON LOWER(Materie_Prime_Campionature.tipo) = LOWER('o'+ OTabelle.tabella_cod_des) ")
            stb.AppendLine(" INNER JOIN OTabelle_Parametri ON  Materie_Prime_Campionature.tipo_cod = OTabelle_Parametri.Tabella_Par_Cod ")
            stb.AppendLine(" AND  OTabelle_Parametri.Tabella_Par_Cod = Materie_Prime_Campionature.tipo_cod ")
            stb.AppendLine(" AND  OTabelle_Parametri.Tabella_Cod = OTabelle.Tabella_Cod ")
            stb.AppendLine(" --AND  OTabelle_Parametri.piva = OTabelle.piva --non va bene il join ")
            stb.AppendLine(" AND  OTabelle_Parametri.modulo_generazione = OTabelle.modulo_generazione  ")
            stb.AppendLine("  " & vbCrLf)
            stb.AppendLine(" WHERE  OModuli_Referenze_Config_Testata.piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            stb.AppendLine(" AND (OModuli_Referenze_Config_Testata.OFiltro_Veg_Cod LIKE '%|" & Agro_SQL_SaveNum(Veg_Cod) & "|%' OR OModuli_Referenze_Config_Testata.OFiltro_Veg_Cod='') ")
            stb.AppendLine(" AND Materie_Prime_Campionature.tipo_cod <> 0  ")
            stb.AppendLine(" AND progressivo = " & Agro_SQL_SaveNum(Cal_Cod) & " ")
            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            stb.AppendLine(" ORDER BY ordine ")

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
