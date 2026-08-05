Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreDataProvider
Imports System.Transactions
Imports Newtonsoft.Json



Public Class Linee_Preparazioni_R
    Inherits AgronicaCoreDataProvider.DataProvider


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' WORK IN PROGRESS!!!!!!!!!!!!!!!!!!!!!!!!!
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function Leggi_MateriePrime_Preparazione(ByVal Piva As String,
                                                    ByVal Preparazione_Cod As Integer,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreParametri
                                                    ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Linee_Preparazioni_R.Leggi_MateriePrime_Preparazione"

        Dim messaggioErrore As String = ""
        Dim stbSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            stbSql.Length = 0


            ''------------------------------------------------------ 
            ''------------------- SELECT ---------------------------
            ''------------------------------------------------------
            'stbSql.Append(" SELECT Linee_Preparazioni.Piva, 0 AS Linea_Cod, Linee_Preparazioni.preparazione_cod, Linee_Preparazioni.Preparazione_Des AS Descrizione, Linee_Preparazioni.note ")
            'stbSql.Append(" Linee_Preparazioni_Dettagli.*, Materie_Prime.mat_des, Materie_Prime.cod_articolo ")

            ''------------------------------------------------------ 
            ''-------------------- FROM ----------------------------
            ''------------------------------------------------------
            'stbSql.Append(" FROM Linee_Preparazioni ")

            'stbSql.Append(" INNER JOIN Linee_Preparazioni_Dettagli ")
            'stbSql.Append("             ON Linee_Preparazioni.Piva =  Linee_Preparazioni_Dettagli.Piva ")
            'stbSql.Append("             AND Linee_Preparazioni.Preparazione_Cod =  Linee_Preparazioni_Dettagli.Preparazione_Cod ")

            'stbSql.Append(" INNER JOIN Materie_Prime ")
            'stbSql.Append("             ON Materie_Prime.Elem_Cod =  Linee_Preparazioni_Dettagli.Elem_Cod ")
            'stbSql.Append("             AND Materie_Prime.Mat_Cod =  Linee_Preparazioni_Dettagli.Mat_Cod ")

            'stbSql.Append(" INNER JOIN Linee_PreparazionixReport ")
            'stbSql.Append("             ON Linee_Preparazioni.Piva =  Linee_PreparazionixReport.Piva  ")
            'stbSql.Append("             AND Linee_Preparazioni.Preparazione_Cod =  Linee_PreparazionixReport.Preparazione_Cod ")

            ''------------------------------------------------------ 
            ''-------------------- WHERE ---------------------------
            ''------------------------------------------------------
            'stbSql.Append(" WHERE Linee_Preparazioni.Piva = '" + Agro_SQL_SaveText(Piva) + "' " + vbCrLf)

            ''la materia prima deve essere scaricata, quindi un ingrediente, non un prodotto risultante 
            'stbSql.Append(" AND Linee_Preparazioni_Dettagli.Cau_Mov = '" + Agro_SQL_SaveText(CAU_SCARICO) + "' " + vbCrLf)
            ''la materia prima deve essere il risultato della preparazione
            'stbSql.Append(" AND Linee_Preparazioni_Dettagli.ChkFinale =1 " + vbCrLf)

            '' LA PREPARAZIONE DEVE ESSERE SINGOLA, NON DEVE ESSERE CONTENUTA IN UNA LINEA
            'stbSql.Append(" AND Linee_Preparazioni.Preparazione_Cod not in (select Preparazione_Cod from [Linee_ProduzionixPreparazioni]) " + vbCrLf)

            ''IL DETTAGLIO DELLA PREPARAZIONE NON DEVE COMPARIRE
            'stbSql.Append(" AND NOT EXISTS (SELECT * FROM  Linee_Preparazioni_Report_Esclusi LPRE " + vbCrLf)
            'stbSql.Append("                 WHERE LPRE.Id_Report = Linee_PreparazionixReport.Id_Report  " + vbCrLf)
            'stbSql.Append("                 AND LPRE.Piva = Linee_Preparazioni_Dettagli.Piva AND LPRE.Preparazione_Cod = Linee_Preparazioni_Dettagli.Preparazione_Cod AND LPRE.Dettaglio_Cod = Linee_Preparazioni_Dettagli.Dettaglio_Cod ) " + vbCrLf)

            'If Preparazione_Cod <> 0 Then
            '    stbSql.Append(" AND Linee_Preparazioni.Preparazione_Cod = " + Agro_SQL_SaveNum(Preparazione_Cod) + " " + vbCrLf)
            'End If

            'If Mat_Cod <> 0 Then
            '    stbSql.Append(" AND Linee_Preparazioni_Dettagli.Mat_Cod = " + Agro_SQL_SaveNum(Mat_Cod) + " " + vbCrLf)
            'End If

            ''--------------------------------------------------------------------------
            'If xFiltroAggiuntivo <> "" Then
            '    stbSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            'End If
            ''--------------------------------------------------------------------------

            ''################################
            'If xOrderBy <> "" Then
            '    stbSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            'Else
            '    stbSql.Append("  ")
            'End If


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    Public Function Leggi(ByVal Piva As String,
                          ByVal Preparazione_Cod As Integer,
                          ByVal Modulo_Generazione As Integer,
                          ByVal Codice_Generazione As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Linee_Preparazioni_R.Leggi"

        Dim messaggioErrore As String = ""
        Dim stbSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            stbSql.Length = 0


            '------------------------------------------------------ 
            '------------------- SELECT ---------------------------
            '------------------------------------------------------
            stbSql.AppendLine(" SELECT * ")

            '------------------------------------------------------ 
            '-------------------- FROM ----------------------------
            '------------------------------------------------------
            stbSql.AppendLine(" FROM Linee_Preparazioni ")

            '------------------------------------------------------ 
            '-------------------- WHERE ---------------------------
            '------------------------------------------------------
            stbSql.AppendLine(" WHERE Linee_Preparazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            If Preparazione_Cod <> 0 Then
                stbSql.Append(" AND Preparazione_Cod = " & Agro_SQL_SaveNum(Preparazione_Cod))
            End If
            If Modulo_Generazione <> 0 Then
                stbSql.Append(" AND Modulo_Generazione = " & Agro_SQL_SaveNum(Modulo_Generazione))
            End If
            If Codice_Generazione <> 0 Then
                stbSql.Append(" AND Codice_Generazione = " & Agro_SQL_SaveNum(Codice_Generazione))
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                stbSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            '################################
            If xOrderBy <> "" Then
                stbSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                stbSql.Append("  ")
            End If


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    Public Function LeggiProdottiPresenti(ByVal Piva As String,
                                          ByVal Preparazione_Cod As Integer,
                                          ByVal Modulo_Generazione As Integer,
                                          ByVal Codice_Generazione As Integer,
                                          ByVal xFiltroAggiuntivo As String,
                                          ByVal xOrderBy As String,
                                          ByRef objParametri As AgronicaCoreParametri
                                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Linee_Preparazioni_R.Leggi"

        Dim messaggioErrore As String = ""
        Dim stbSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            stbSql.Length = 0

            stbSql.AppendLine(" SELECT * ")
            stbSql.AppendLine(" FROM Linee_Preparazioni ")
            stbSql.AppendLine(" WHERE Linee_Preparazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            If Preparazione_Cod <> 0 Then
                stbSql.AppendLine(" AND Preparazione_Cod = " & Agro_SQL_SaveNum(Preparazione_Cod))
            End If

            If Modulo_Generazione <> 0 Then
                stbSql.AppendLine(" AND Modulo_Generazione = " & Agro_SQL_SaveNum(Modulo_Generazione))
            End If

            If Codice_Generazione <> 0 Then
                stbSql.AppendLine(" AND Codice_Generazione = " & Agro_SQL_SaveNum(Codice_Generazione))
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                stbSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            stbSql.AppendLine(" And   preparazione_cod in ")

            stbSql.AppendLine(" (SELECT Codice_Generazione FROM Linee_Preparazioni  where piva != '" & Agro_SQL_SaveText(Piva) & "' ")
            'stbSql.AppendLine(" And [Preparazione_Cod] in ")
            'stbSql.AppendLine(" ( SELECT distinct [Preparazione_Cod] ")
            'stbSql.AppendLine("From  Linee_Preparazioni_Dettagli   ) )")
            stbSql.AppendLine(")")


            '################################
            If xOrderBy <> "" Then
                stbSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                stbSql.AppendLine("  ")
            End If


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '###################################################################################
    Public Function PreparazioneDes_From_PreparazioneCod(ByVal piva As String,
                                                         ByVal preparazioneCod As Integer,
                                                         ByRef objParametri As AgronicaCoreParametri
                                                         ) As String

        Dim dt As DataTable = Leggi(piva, preparazioneCod, 0, 0, "", "", objParametri)

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            Return dt.Rows(0).Item("Preparazione_Des")
        Else
            Return ""
        End If

    End Function


    '###################################################################################
    Public Function Lista_PreparazioneCod_xQuery(ByVal Piva As String,
                                                 ByVal Modulo_Generazione As Integer,
                                                 ByVal Codice_Generazione As Integer,
                                                 ByVal xFiltroAggiuntivo As String,
                                                 ByVal xOrderBy As String,
                                                 ByRef objParametri As AgronicaCoreParametri
                                                 ) As String

        Const nomeRoutine = "AgronicaCoreContabDAL.Linee_Preparazioni_R.Lista_PreparazioneCod_xQuery"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim listaPrepCod As String = ""

        Try

            dt = Leggi(Piva,
                        0,
                        Modulo_Generazione,
                        Codice_Generazione,
                        xFiltroAggiuntivo, xOrderBy,
                        objParametri)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                For i As Integer = 0 To dt.Rows.Count - 1
                    listaPrepCod &= CStr(dt.Rows(i).Item("Preparazione_Cod")) & ","
                Next
                listaPrepCod = Left(listaPrepCod, listaPrepCod.Length - 1)
                listaPrepCod = "(" & listaPrepCod & ")"
            End If

            dt = Nothing

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return listaPrepCod

    End Function

    Public Function LeggiPreparazioneDaLinea(ByVal piva As String,
                                             ByVal codice_generazione As Integer,
                                             ByVal preparazione_cod As Integer,
                                             ByVal linea_cod As Integer,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As String

        Dim risposta As String

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Linee_Ppreparazioni_R.LeggiPreparazioneDaLinea()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim Elem As Linee_Preparazioni

            Elem = (From lp In GiasContext.Linee_Preparazioni
                    Join lpxp In GiasContext.Linee_ProduzionixPreparazioni
                        On lp.Preparazione_Cod Equals lpxp.Preparazione_Cod
                    Where lp.Piva.Equals(piva) AndAlso
                          lpxp.Linea_Cod = linea_cod AndAlso
                          (codice_generazione = 0 OrElse lp.Codice_Generazione = codice_generazione) AndAlso
                          (preparazione_cod = 0 OrElse lp.Preparazione_Cod = preparazione_cod)
                    Select lp).FirstOrDefault()

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(Elem, Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function
    
    Public Function Leggi_Codice_Generazione(ByVal piva As String,
                                             ByVal id_agenda As Integer,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Linee_Preparazioni_R.Leggi"

        Dim messaggioErrore As String = ""
        Dim stbSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            stbSql.Length = 0

            stbSql.AppendLine(" Select Linee_Preparazioni.Codice_Generazione From Linee_Preparazioni, Agenda ")
            stbSql.AppendLine(" WHERE Linee_Preparazioni.Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            stbSql.AppendLine(" And Linee_Preparazioni.Piva = Agenda.Piva ")
            stbSql.AppendLine(" And Linee_Preparazioni.Preparazione_Cod = Agenda.Preparazione_Cod ")
            stbSql.AppendLine(" And Agenda.Id_Agenda = " & Agro_SQL_SaveNum(id_agenda))

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbSql.ToString, nomeRoutine)
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

'#################################################################
'#################################################################
'#################################################################

Public Class Linee_Preparazioni_W
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Scrivi(ByVal Piva As String,
                           ByVal Preparazione_Cod As Integer,
                           ByVal Preparazione_Des As String,
                           ByVal Tipo_Produzione As String,
                           ByVal Lav_Cod As Integer,
                           ByVal Preparazione_Sigla As String,
                           ByVal Tipo_Preparazione As String,
                           ByVal Tipo_Durata As String,
                           ByVal Durata As String,
                           ByVal Udm_Cod As Integer,
                           ByVal Qta As String,
                           ByVal ChkMonitor As String,
                           ByVal ChkAnalisi As String,
                           ByVal Tipo_Preselezione As String,
                           ByVal Tipo_Scarico As String,
                           ByVal Tipo_Ripartizione As String,
                           ByVal ChkData_Default As String,
                           ByVal ChkCampo_Note As String,
                           ByVal ChkOrdini As String,
                           ByVal ChkBilancia As String,
                           ByVal ChkScarto As String,
                           ByVal ChkBilancia_LottoEdit As String,
                           ByVal ChkBilancia_Abilitazione As String,
                           ByVal Tipo_Insert As String,
                           ByVal ChkTopDown As String,
                           ByVal ChkDataScadenza As String,
                           ByVal Giorni_Scadenza As Integer,
                           ByVal Tipo_Save As String,
                           ByVal Step_Save As Integer,
                           ByVal ChkSalva_Continua As String,
                           ByVal Udm_Cod_Extra As Integer,
                           ByVal Cod_RisUm_Conferimento As Integer,
                           ByVal ChkLotto_Unico As String,
                           ByVal Lotto_Unico As String,
                           ByVal ChkLotto_Componente As String,
                           ByVal ChkLotto_Progetto As String,
                           ByVal ChkLotto_Linea As String,
                           ByVal ChkLotto_Ordine As String,
                           ByVal Tipo_Lotto_Identificativo As String,
                           ByVal Note As String,
                           ByVal ChkDiscriminante_Lotto As String,
                           ByVal ChkDes_Lib_Ora As String,
                           ByVal ChkDes_Lib_Lotto As String,
                           ByVal ChkDes_Lib_Contatto As String,
                           ByVal Linea_Classe_Cod As Integer,
                           ByVal ChkConfezionamento As String,
                           ByVal ChkDes_Lib_Provenienza As String,
                           ByVal ChkDes_Lib_Destinazione As String,
                           ByVal Colore As Integer,
                           ByVal ChkDocumenti As String,
                           ByVal Tipo_Integrazione As String,
                           ByVal Tipo_Default As Integer,
                           ByVal ChkUtility As String,
                           ByVal Tipo_Filtro_Preparati As String,
                           ByVal Modulo_Generazione As Integer,
                           ByVal Codice_Generazione As Integer,
                           ByVal Tipo_Alterazione As Integer,
                           ByVal ChkLink_Parametri As String,
                           ByVal ChkLink_Utility As String,
                           ByVal Domanda As String,
                           ByVal ChkMix As String,
                           ByVal ChkRisposta_Indefinita As String,
                           ByVal validita_inizio As Date,
                           ByVal validita_fine As Date,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#, 
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean


        Const nomeRoutine = "CoreContabDAL.Linee_Preparazioni_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
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
            strSql.AppendLine(" INSERT Linee_Preparazioni_W ")

            strSql.AppendLine("              (")

            strSql.AppendLine("   [Piva] ")
            strSql.AppendLine("  ,[Preparazione_Cod] ")
            strSql.AppendLine("  ,[Preparazione_Des] ")
            strSql.AppendLine("  ,[Tipo_Produzione] ")
            strSql.AppendLine("  ,[Lav_Cod] ")
            strSql.AppendLine("  ,[Preparazione_Sigla] ")
            strSql.AppendLine("  ,[Tipo_Preparazione] ")
            strSql.AppendLine("  ,[Tipo_Durata] ")
            strSql.AppendLine("  ,[Durata] ")
            strSql.AppendLine("  ,[Udm_Cod] ")
            strSql.AppendLine("  ,[Qta] ")
            strSql.AppendLine("  ,[ChkMonitor] ")
            strSql.AppendLine("  ,[ChkAnalisi] ")
            strSql.AppendLine("  ,[Tipo_Preselezione] ")
            strSql.AppendLine("  ,[Tipo_Scarico] ")
            strSql.AppendLine("  ,[Tipo_Ripartizione] ")
            strSql.AppendLine("  ,[ChkData_Default] ")
            strSql.AppendLine("  ,[ChkCampo_Note] ")
            strSql.AppendLine("  ,[ChkOrdini] ")
            strSql.AppendLine("  ,[ChkBilancia] ")
            strSql.AppendLine("  ,[ChkScarto] ")
            strSql.AppendLine("  ,[ChkBilancia_LottoEdit] ")
            strSql.AppendLine("  ,[ChkBilancia_Abilitazione] ")
            strSql.AppendLine("  ,[Tipo_Insert] ")
            strSql.AppendLine("  ,[ChkTopDown] ")
            strSql.AppendLine("  ,[ChkDataScadenza] ")
            strSql.AppendLine("  ,[Giorni_Scadenza] ")
            strSql.AppendLine("  ,[Tipo_Save] ")
            strSql.AppendLine("  ,[Step_Save] ")
            strSql.AppendLine("  ,[ChkSalva_Continua] ")
            strSql.AppendLine("  ,[Udm_Cod_Extra] ")
            strSql.AppendLine("  ,[Cod_RisUm_Conferimento] ")
            strSql.AppendLine("  ,[ChkLotto_Unico] ")
            strSql.AppendLine("  ,[Lotto_Unico] ")
            strSql.AppendLine("  ,[ChkLotto_Componente] ")
            strSql.AppendLine("  ,[ChkLotto_Progetto] ")
            strSql.AppendLine("  ,[ChkLotto_Linea] ")
            strSql.AppendLine("  ,[ChkLotto_Ordine] ")
            strSql.AppendLine("  ,[Tipo_Lotto_Identificativo] ")
            strSql.AppendLine("  ,[Note] ")
            strSql.AppendLine("  ,[ChkDiscriminante_Lotto] ")
            strSql.AppendLine("  ,[ChkDes_Lib_Ora] ")
            strSql.AppendLine("  ,[ChkDes_Lib_Lotto] ")
            strSql.AppendLine("  ,[ChkDes_Lib_Contatto] ")
            strSql.AppendLine("  ,[Linea_Classe_Cod] ")
            strSql.AppendLine("  ,[ChkConfezionamento] ")
            strSql.AppendLine("  ,[ChkDes_Lib_Provenienza] ")
            strSql.AppendLine("  ,[ChkDes_Lib_Destinazione] ")
            strSql.AppendLine("  ,[Colore] ")
            strSql.AppendLine("  ,[ChkDocumenti] ")
            strSql.AppendLine("  ,[Tipo_Integrazione] ")
            strSql.AppendLine("  ,[Tipo_Default] ")
            strSql.AppendLine("  ,[ChkUtility] ")
            strSql.AppendLine("  ,[Tipo_Filtro_Preparati] ")
            strSql.AppendLine("  ,[Modulo_Generazione] ")
            strSql.AppendLine("  ,[Codice_Generazione] ")
            strSql.AppendLine("  ,[Tipo_Alterazione] ")
            strSql.AppendLine("  ,[ChkLink_Parametri] ")
            strSql.AppendLine("  ,[ChkLink_Utility] ")
            strSql.AppendLine("  ,[Domanda] ")
            strSql.AppendLine("  ,[ChkMix] ")
            strSql.AppendLine("  ,[ChkRisposta_Indefinita], ")


            strSql.Append("              Inviato,            datainvio, ")
            strSql.Append("              Data_Creazione,     Data_Modifica, ")
            strSql.Append("              UserName_Creazione, UserName_Modifica, ")
            strSql.Append("              Validita_Inizio,    Validita_Fine ")
            strSql.Append("              ) ")

            strSql.Append(" VALUES ( ")

            strSql.Append(",'" & Agro_SQL_SaveText(Piva) & "'" & vbCrLf)
            strSql.Append(", " & Agro_SQL_SaveNum(Preparazione_Cod) & " " & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(Preparazione_Des) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(Tipo_Produzione) & "'" & vbCrLf)
            strSql.Append(", " & Agro_SQL_SaveNum(Lav_Cod) & " " & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(Preparazione_Sigla) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(Tipo_Preparazione) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(Tipo_Durata) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(Durata) & "'" & vbCrLf)
            strSql.Append(", " & Agro_SQL_SaveNum(Udm_Cod) & " " & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(Qta) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(ChkMonitor) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(ChkAnalisi) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(Tipo_Preselezione) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(Tipo_Scarico) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(Tipo_Ripartizione) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(ChkData_Default) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(ChkCampo_Note) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(ChkOrdini) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(ChkBilancia) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(ChkScarto) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(ChkBilancia_LottoEdit) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(ChkBilancia_Abilitazione) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(Tipo_Insert) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(ChkTopDown) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(ChkDataScadenza) & "'" & vbCrLf)
            strSql.Append(", " & Agro_SQL_SaveNum(Giorni_Scadenza) & " " & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(Tipo_Save) & "'" & vbCrLf)
            strSql.Append(", " & Agro_SQL_SaveNum(Step_Save) & " " & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(ChkSalva_Continua) & "'" & vbCrLf)
            strSql.Append(", " & Agro_SQL_SaveNum(Udm_Cod_Extra) & " " & vbCrLf)
            strSql.Append(", " & Agro_SQL_SaveNum(Cod_RisUm_Conferimento) & " " & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(ChkLotto_Unico) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(Lotto_Unico) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(ChkLotto_Componente) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(ChkLotto_Progetto) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(ChkLotto_Linea) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(ChkLotto_Ordine) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(Tipo_Lotto_Identificativo) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(Note) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(ChkDiscriminante_Lotto) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(ChkDes_Lib_Ora) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(ChkDes_Lib_Lotto) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(ChkDes_Lib_Contatto) & "'" & vbCrLf)
            strSql.Append(", " & Agro_SQL_SaveNum(Linea_Classe_Cod) & " " & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(ChkConfezionamento) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(ChkDes_Lib_Provenienza) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(ChkDes_Lib_Destinazione) & "'" & vbCrLf)
            strSql.Append(", " & Agro_SQL_SaveNum(Colore) & " " & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(ChkDocumenti) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(Tipo_Integrazione) & "'" & vbCrLf)
            strSql.Append(", " & Agro_SQL_SaveNum(Tipo_Default) & " " & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(ChkUtility) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(Tipo_Filtro_Preparati) & "'" & vbCrLf)
            strSql.Append(", " & Agro_SQL_SaveNum(Modulo_Generazione) & " " & vbCrLf)
            strSql.Append(", " & Agro_SQL_SaveNum(Codice_Generazione) & " " & vbCrLf)
            strSql.Append(", " & Agro_SQL_SaveNum(Tipo_Alterazione) & " " & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(ChkLink_Parametri) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(ChkLink_Utility) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(Domanda) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(ChkMix) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(ChkRisposta_Indefinita) & "'" & vbCrLf)
            
            strSql.AppendLine("         , 0  ")
            strSql.AppendLine("         , Null  ")

            strSql.Append("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            strSql.Append("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            strSql.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            strSql.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            strSql.Append("			, " & Agro_SQL_SaveDate(validita_inizio) & "  ")
            strSql.Append("			, " & Agro_SQL_SaveDate(validita_fine) & "  ")
            
            strSql.Append(") ")

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


    '#################################################################
    Public Function Cancella(ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        '----- Descrizione
        Dim nomeRoutine As String = "Cancella()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            strSql.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                strSql.Append(" UPDATE ... ")
                strSql.Append(" SET ")
                strSql.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                strSql.Append("         ,Inviato = -1 ")
                strSql.Append(" WHERE   1=1 ")
                strSql.Append(" AND     Inviato >= 0 ")
            Else
                strSql.Append(" DELETE FROM ... ")
                strSql.Append(" WHERE 1=1 ")
            End If
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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
