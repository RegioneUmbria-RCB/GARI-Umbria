Imports System.Data.Entity
Imports System.Text
Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports Newtonsoft.Json.Linq

Public Class Risorse_Umane_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal Piva As String,
                          ByVal Cod_Contatto As String,
                          ByVal Cod_RisUm As Long,
                          ByVal Cod_Rapporto As Long,
                          ByVal Cod_RisUm_Origine As Long,
                          ByVal Piva_SuperUser_Origine As String,
                          ByVal Flag_AncheImportatati As Boolean,
                          ByVal Flag_AnchePubblici As Boolean,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Risorse_Umane_R.Leggi()"
        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.Append(" SELECT     Risorse_Umane.*, Rapporti_Contabili.* ")
            strSql.Append(" FROM        Risorse_Umane ")
            strSql.Append(" INNER JOIN  UtentiXImprese ON Risorse_Umane.Piva = UtentiXImprese.PIVA ")
            strSql.Append(" LEFT JOIN Rapporti_Contabili ON Risorse_Umane.Cod_Rapporto = Rapporti_Contabili.Cod_Rapporto ")
            If Flag_AnchePubblici = True Then
                strSql.Append(" INNER JOIN  Contatti ")
                strSql.Append("             ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto AND Contatti.Piva = Risorse_Umane.Piva ")
            End If
            strSql.Append(" WHERE       UtentiXImprese.[USER] = '" & Trim(objParametri.PivaSuperUser) & "' ")
            strSql.Append(" AND         Risorse_Umane.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            strSql.Append(" AND         Risorse_Umane.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Flag_AnchePubblici = False Then
                If Piva <> "" Then
                    strSql.Append(" AND Risorse_Umane.Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "'   ")
                End If
            Else
                strSql.Append(" AND ( Contatti.Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "'  OR  Contatti.Sa_Cod = -1 )  ")
            End If

            If Cod_Contatto <> "" Then
                strSql.Append(" AND Risorse_Umane.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "' ")
            End If

            If Cod_RisUm <> 0 Then
                strSql.Append(" AND Risorse_Umane.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
            End If

            If Cod_Rapporto <> 0 Then
                strSql.Append(" AND Risorse_Umane.Cod_Rapporto = " & Agro_SQL_SaveNum(Cod_Rapporto) & " ")
            End If

            If Cod_RisUm_Origine <> 0 Then
                strSql.Append(" AND  Risorse_Umane.Cod_RisUm_Origine = " & Agro_SQL_SaveNum(Cod_RisUm_Origine) & "   ")
            End If

            If Piva_SuperUser_Origine <> "" Then
                strSql.Append(" AND  Risorse_Umane.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'")
            End If

            If Flag_AncheImportatati = False Then
                strSql.Append(" AND  Risorse_Umane.Cod_RisUm_Origine = " & Agro_SQL_SaveNum(0) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.Append(" AND   Risorse_Umane.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.Append(" AND   Risorse_Umane.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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
    'legge risum e contatti con filtri semplici
    Public Function Leggi3(ByVal Piva As String,
                           ByVal Cod_Contatto As String,
                           ByVal Cod_RisUm As Long,
                           ByVal Cod_Rapporto As Long,
                           ByVal xFiltroAggiuntivo As String,
                           ByVal xOrderBy As String,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Risorse_Umane_R.Leggi3()"
        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.Append(" SELECT     * ")
            strSql.Append(" FROM        Risorse_Umane ")
            strSql.Append(" INNER JOIN  UtentiXImprese ")
            strSql.Append("             ON Risorse_Umane.Piva = UtentiXImprese.PIVA ")
            strSql.Append(" INNER JOIN  Contatti ")
            strSql.Append("             ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto AND Contatti.Piva = Risorse_Umane.Piva ")
            strSql.Append(" WHERE       UtentiXImprese.[USER] = '" & Trim(objParametri.PivaSuperUser) & "' ")

            If Piva <> "" Then
                strSql.Append(" AND Risorse_Umane.Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "'   ")
            End If

            If Cod_Contatto <> "" Then
                strSql.Append(" AND Risorse_Umane.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "' ")
            End If

            If Cod_RisUm <> 0 Then
                strSql.Append(" AND Risorse_Umane.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
            End If

            If Cod_Rapporto <> 0 Then
                strSql.Append(" AND Risorse_Umane.Cod_Rapporto = " & Agro_SQL_SaveNum(Cod_Rapporto) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.Append(" AND   Risorse_Umane.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.Append(" AND   Risorse_Umane.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    Public Function Leggi_RisorseUmane(Cod_RisUm As Integer, objParametri_Server As AgronicaCoreParametri) As Risorse_Umane

        Dim elem As Risorse_Umane = Nothing

        Dim Piva_SuperUser = objParametri_Server.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "Risorse_Umane_R.Leggi_RisorseUmane()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            elem = (From ru In GiasContext.Risorse_Umane
                    Where ru.Cod_RisUm = Cod_RisUm
                    Select ru).FirstOrDefault()

        End Using

        Return elem

    End Function

    '##############################################################################################
    'legge risum join rapporti_contabili 
    Public Function Leggi4(ByVal Piva As String,
                           ByVal Cod_Contatto As String,
                           ByVal Cod_RisUm As Long,
                           ByVal Cod_Rapporto As Long,
                           ByVal Progressivo As String,
                           ByVal Cliente As Boolean,
                           ByVal Fornitore As Boolean,
                           ByVal Dipendente As Boolean,
                           ByVal Terzista As Boolean,
                           ByVal Legale As Boolean,
                           ByVal Agente As Boolean,
                           ByVal Consulente As Boolean,
                           ByVal xFiltroAggiuntivo As String,
                           ByVal xOrderBy As String,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Attivita_Des As String = ""
                           ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Risorse_Umane_R.Leggi4()"
        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.Append(" SELECT     * ")
            strSql.Append(" FROM        Risorse_Umane WITH(NOLOCK)")
            strSql.Append(" INNER JOIN  UtentiXImprese WITH(NOLOCK)")
            strSql.Append("             ON Risorse_Umane.Piva = UtentiXImprese.PIVA ")
            strSql.Append(" INNER JOIN  Rapporti_Contabili WITH(NOLOCK)")
            strSql.Append("             ON Rapporti_Contabili.Cod_Rapporto = Risorse_Umane.Cod_Rapporto AND Rapporti_Contabili.Piva = UtentiXImprese.[USER] ")
            strSql.Append(" WHERE       UtentiXImprese.[USER] = '" & Trim(objParametri.PivaSuperUser) & "' ")

            If Piva <> "" Then
                strSql.Append(" AND Risorse_Umane.Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "'   ")
            End If

            If Cod_Contatto <> "" Then
                strSql.Append(" AND Risorse_Umane.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "' ")
            End If

            If Cod_RisUm <> 0 Then
                strSql.Append(" AND Risorse_Umane.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
            End If

            If Cod_Rapporto <> 0 Then
                strSql.Append(" AND Risorse_Umane.Cod_Rapporto = " & Agro_SQL_SaveNum(Cod_Rapporto) & " ")
            End If

            If Progressivo <> "" Then
                strSql.Append(" AND Risorse_Umane.Settore_Des = '" & Agro_SQL_SaveText(Progressivo) & "' ")
            End If

            If Attivita_Des <> "" Then
                strSql.Append(" AND Risorse_Umane.Attivita_Des = '" & Agro_SQL_SaveText(Attivita_Des) & "' ")
            End If

            If Cliente = True Then
                strSql.Append(" AND     Rapporti_Contabili.Cliente = " & Agro_SQL_SaveNum(1) & "   ")
            End If

            If Fornitore = True Then
                strSql.Append(" AND     Rapporti_Contabili.Fornitore = " & Agro_SQL_SaveNum(1) & "   ")
            End If

            If Dipendente = True Then
                strSql.Append(" AND     Rapporti_Contabili.Dipendente = " & Agro_SQL_SaveNum(1) & "   ")
            End If

            If Terzista = True Then
                strSql.Append(" AND     Rapporti_Contabili.Terzista = " & Agro_SQL_SaveNum(1) & "   ")
            End If

            If Legale = True Then
                strSql.Append(" AND     Rapporti_Contabili.Legale = " & Agro_SQL_SaveNum(1) & "   ")
            End If

            If Agente = True Then
                strSql.Append(" AND     Rapporti_Contabili.Agente = " & Agro_SQL_SaveNum(1) & "   ")
            End If

            If Consulente = True Then
                strSql.Append(" AND     Rapporti_Contabili.Consulente = " & Agro_SQL_SaveNum(1) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.Append(" AND   Risorse_Umane.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.Append(" AND   Risorse_Umane.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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
    'utilizzata da scheda di campagna
    Public Function LeggiLegaleEReferente_xTestataQDC(ByVal Piva As String,
                                                        ByVal Sa_Cod_CentroAziendale As Integer,
                                                        ByVal data_inizio_risum As Date,
                                                        ByVal data_fine_risum As Date,
                                                            ByVal xFiltroAggiuntivo As String,
                                                            ByVal xOrderBy As String,
                                                            ByRef objParametri As AgronicaCoreParametri
                                                            ) As String
        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Risorse_Umane_R.LeggiLegaleEReferente_xTestataQDC()"
        Dim messaggioErrore As String = ""
        Dim Str_LegaleReferente As String = ""
        Dim DT As DataTable
        Dim Flag_CercaReferente As Boolean = False
        Dim str_dal As String
        Dim str_al As String

        DT = LeggiLegaleRappresentanteReferenteAziendale(Piva,
                                                        Sa_Cod_CentroAziendale,
                                                        data_inizio_risum,
                                                        data_fine_risum,
                                                        xFiltroAggiuntivo,
                                                        xOrderBy,
                                                        objParametri)

        If Not IsNothing(DT) AndAlso DT.Rows.Count Then

            Dim Dr_legale() As DataRow
            Dr_legale = DT.Select("cod_rapporto = " & CStr(COD_LEGALE))
            If Not IsNothing(Dr_legale) Then
                Select Case Dr_legale.Count
                    Case 0
                        'non c'è alcun legale -> cerca referente
                        Flag_CercaReferente = True
                    Case 1
                        Str_LegaleReferente = Trim(Dr_legale(0).Item("Nome"))
                    Case Is > 1
                        For i = 0 To Dr_legale.Count - 1
                            str_dal = ""
                            str_al = ""
                            If CDate(Dr_legale(i).Item("validita_inizio")) <> AGRODATAINIZIO Then
                                str_dal = " dal " & CDate(Dr_legale(i).Item("validita_inizio")).ToShortDateString
                            End If
                            If CDate(Dr_legale(i).Item("validita_fine")) <> AGRODATAFINE Then
                                str_al = " al " & CDate(Dr_legale(i).Item("validita_fine")).ToShortDateString
                            End If
                            Str_LegaleReferente &= " " & Trim(Dr_legale(i).Item("Nome")) & str_dal & str_al
                        Next
                End Select
            End If

            If Flag_CercaReferente = True Then
                Dim Dr_ref() As DataRow
                Dr_ref = DT.Select("cod_rapporto = " & CStr(COD_REFERENTEAZIENDALE))
                If Not IsNothing(Dr_ref) Then
                    Select Case Dr_ref.Count
                        Case 0
                            'cerca referente
                            Flag_CercaReferente = True
                        Case 1
                            Str_LegaleReferente = Trim(Dr_ref(0).Item("Nome"))
                        Case Is > 1
                            For i = 0 To Dr_ref.Count - 1
                                str_dal = ""
                                str_al = ""
                                If CDate(Dr_ref(i).Item("validita_inizio")) <> AGRODATAINIZIO Then
                                    str_dal = " dal " & CDate(Dr_ref(i).Item("validita_inizio")).ToShortDateString
                                End If
                                If CDate(Dr_ref(i).Item("validita_fine")) <> AGRODATAFINE Then
                                    str_al = " al " & CDate(Dr_ref(i).Item("validita_fine")).ToShortDateString
                                End If
                                Str_LegaleReferente &= " " & Trim(Dr_ref(i).Item("Nome")) & str_dal & str_al
                            Next
                    End Select
                End If

            End If

            Str_LegaleReferente = Trim(Str_LegaleReferente)

        End If

        Return Str_LegaleReferente

    End Function

    '##############################################################################################
    Public Function LeggiLegaleRappresentanteReferenteAziendale(ByVal Piva As String,
                                                                ByVal Sa_Cod_CentroAziendale As Integer,
                                                                ByVal data_inizio_risum As Date,
                                                                ByVal data_fine_risum As Date,
                                                                  ByVal xFiltroAggiuntivo As String,
                                                                  ByVal xOrderBy As String,
                                                                  ByRef objParametri As AgronicaCoreParametri
                                                                  ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Risorse_Umane_R.LeggiLegaleRappresentanteReferenteAziendale()"
        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.Append(" SELECT Contatti.Cognome + ' ' + Contatti.nome + ' ' + Contatti.Rag_Soc AS Nome, Risorse_Umane.Validita_Inizio, Risorse_Umane.Validita_fine, Risorse_Umane.COD_RAPPORTO  " & vbCrLf)
            strSql.Append(" FROM Contatti " & vbCrLf)
            strSql.Append(" INNER JOIN Risorse_Umane ON Contatti.Piva = Risorse_Umane.Piva  AND Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto " & vbCrLf)
            strSql.Append(" INNER JOIN  UtentiXImprese ON Risorse_Umane.Piva = UtentiXImprese.PIVA ")
            strSql.Append(" WHERE Risorse_Umane.Cod_Rapporto IN (" & CStr(COD_LEGALE) & "," & CStr(COD_REFERENTEAZIENDALE) & ")  " & vbCrLf)
            strSql.Append(" AND    (      (Contatti.Sa_Cod = -1 AND Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "')   " & vbCrLf)
            strSql.Append("         OR    (Contatti.Sa_Cod = 0 AND Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "')   " & vbCrLf)
            strSql.Append("         OR    (Contatti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod_CentroAziendale) & " AND Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "')   " & vbCrLf)
            strSql.Append("         )   " & vbCrLf)
            strSql.Append(" AND       UtentiXImprese.[USER] = '" & Trim(objParametri.PivaSuperUser) & "' ")
            strSql.Append(" AND         Risorse_Umane.Validita_inizio <= " & Agro_SQL_SaveDate(data_fine_risum) & " ")
            strSql.Append(" AND         Risorse_Umane.Validita_Fine >= " & Agro_SQL_SaveDate(data_inizio_risum) & " ")

            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.Append(" AND   Risorse_Umane.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.Append(" AND   Risorse_Umane.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.Append(" ORDER BY Risorse_Umane.COD_RAPPORTO DESC, Risorse_Umane.Validita_Inizio ASC, Risorse_Umane.Validita_fine ASC ")
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
    Public Function LeggiRisorseCosti(ByVal Cod_RisUm As Long,
                                      ByRef objParametri As AgronicaCoreParametri
                                      ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Risorse_Umane_R.LeggiRisorseCosti()"
        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT Risorse_Umane.Cod_RisUm,  Risorse_Umane.Cod_Rapporto, Risorse_Umane.Corrispettivo_Mensile, Risorse_Umane.Corrispettivo_Orario, ")
            strSql.AppendLine("        Risorse_Umane.Occasionale, Risorse_Umane.Ore_Settimanali, Risorse_Umane.Giorni_Ferie, Risorse_Umane.Ferie_Godute,Risorse_Umane.Giorni_Malattia, Risorse_Umane.Inviato, Risorse_Umane.DataInvio, Risorse_Umane.Data_Creazione, Risorse_Umane.Data_Modifica,Risorse_Umane.Username_Creazione, Risorse_Umane.Username_Modifica, Risorse_Umane.Patentino, Risorse_Umane.Data_Rilascio_Patentino,Risorse_Umane.Data_Scadenza_Patentino, Risorse_Umane.Cod_RisUm_Origine, Risorse_Umane.Piva_SuperUser_Origine, Prodotti_Costi.Mezzo,Prodotti_Costi.Prezzo_Unitario ")
            strSql.AppendLine(" FROM Risorse_Umane ")
            strSql.AppendLine(" INNER JOIN Prodotti_Costi ON Risorse_Umane.Cod_RisUm = Prodotti_Costi.Mat_Cod And Prodotti_Costi.Id_Budget = 0 ")

            strSql.AppendLine(" WHERE cod_risum = " & Cod_RisUm)

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
    Public Function Lista_CodRisUm_ByChiaveContatto(ByVal Piva As String,
                                                    ByVal Cod_Contatto As String,
                                                    ByVal Piva_SuperUser_Origine As String,
                                                    ByVal Flag_AncheImportatati As Boolean,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByRef objParametri As AgronicaCoreParametri
                                                    ) As String

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Risorse_Umane_R.Lista_CodRisUm_ByChiaveContatto()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim filtro As String = ""
        Dim i As Integer

        Try

            dt = Leggi(Piva,
                       Cod_Contatto,
                       0,
                       0, 0,
                       Piva_SuperUser_Origine,
                       Flag_AncheImportatati,
                       True,
                       xFiltroAggiuntivo, "",
                       objParametri)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                For i = 0 To dt.Rows.Count - 1
                    filtro &= " " & CStr(dt.Rows(i).Item("Cod_RisUm")) & ","
                Next
                'tolgo l'ultima la virgola
                filtro = Left(filtro, filtro.Length - 1)
            End If

            dt = Nothing

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return filtro

    End Function

    Public Function LeggiSoloContatto(ByVal Piva As String,
                                      ByVal Cod_RisUm As Integer,
                                      ByVal Cod_Contatto As String,
                                      ByVal Cod_Rapporto As Integer,
                                      ByVal Cod_RisUm_Origine As Integer,
                                      ByVal Piva_SuperUser_Origine As String,
                                      ByVal Flag_AncheImportatati As Boolean,
                                      ByVal Validita_Inizio As Date,
                                      ByVal Validita_Fine As Date,
                                      ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                      ByVal xFiltroAggiuntivo As String,
                                      ByVal xOrderBy As String,
                                      ByRef objParametri As AgronicaCoreParametri
                                      ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Risorse_Umane_R.LeggiSoloContatto()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""
        '   Cod_RisUm = 0            
        '   Cod_Contatto = ""           
        '   Cod_Rapporto = 0
        '   Cod_RisUm_Origine = 0  
        '   Piva_SuperUser_Origine = ""
        '   Flag_AncheImportati = True
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            '------------------------------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    strSql.Length = 0

                    '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                    '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                    '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

                    strSql.Append(" SELECT  Risorse_Umane.*")
                    strSql.Append(" FROM        Contatti ")
                    strSql.Append(" INNER JOIN  Risorse_Umane ")
                    strSql.Append("             ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto AND Contatti.Piva = Risorse_Umane.Piva ")
                    strSql.Append(" INNER JOIN  Rapporti_Contabili ")
                    strSql.Append("             ON Risorse_Umane.Cod_Rapporto = Rapporti_Contabili.Cod_Rapporto ")
                    strSql.Append(" INNER JOIN  UtentiXImprese ")
                    strSql.Append("             ON Contatti.Piva = UtentiXImprese.PIVA AND Rapporti_Contabili.Piva = UtentiXImprese.[USER] ")
                    strSql.Append(" WHERE       (Rapporti_Contabili.Piva = '" & Trim(objParametri.PivaSuperUser) & "') ")
                    strSql.Append(" AND         Risorse_Umane.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.Append(" AND         Risorse_Umane.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")


                    If Piva <> "" Then
                        strSql.Append(" AND  Risorse_Umane.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
                    End If

                    If Cod_RisUm <> 0 Then
                        strSql.Append(" AND Risorse_Umane.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
                    End If

                    If Cod_Contatto <> "" Then
                        strSql.Append(" AND Risorse_Umane.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "' ")
                    End If

                    If Cod_Rapporto <> 0 Then
                        strSql.Append(" AND Risorse_Umane.Cod_Rapporto = " & Agro_SQL_SaveNum(Cod_Rapporto) & " ")
                    End If

                    If Cod_RisUm_Origine <> 0 Then
                        strSql.Append(" AND  Risorse_Umane.Cod_RisUm_Origine = " & Agro_SQL_SaveNum(Cod_RisUm_Origine) & "   ")
                    End If

                    If Piva_SuperUser_Origine <> "" Then
                        strSql.Append(" AND  Risorse_Umane.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'")
                    End If

                    If Flag_AncheImportatati <> True Then
                        strSql.Append(" AND  Risorse_Umane.Cod_RisUm_Origine = " & Agro_SQL_SaveNum(0) & " ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.Append(" AND   Risorse_Umane.Inviato >=0 ")
                            strSql.Append(" AND   Contatti.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.Append(" AND   Risorse_Umane.Inviato =-1 ")
                            strSql.Append(" AND   Contatti.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.Append(" ORDER BY Risorse_Umane.Cod_Rapporto Asc , Risorse_Umane.Validita_Fine Desc ")
                    End If


                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    strSql.Length = 0

                    '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                    '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                    '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

                    strSql.Append(" SELECT  Risorse_Umane.* , Contatti.* ")
                    strSql.Append(" FROM    Risorse_Umane , Contatti ")
                    strSql.Append(" WHERE   Contatti.Piva = Risorse_Umane.Piva ")
                    strSql.Append(" AND     Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto ")
                    strSql.Append(" AND     Risorse_Umane.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    strSql.Append(" AND     Risorse_Umane.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")


                    If Piva <> "" Then
                        strSql.Append(" AND  Risorse_Umane.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
                    End If

                    If Cod_RisUm <> 0 Then
                        strSql.Append(" AND Risorse_Umane.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
                    End If

                    If Cod_Contatto <> "" Then
                        strSql.Append(" AND Risorse_Umane.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "' ")
                    End If

                    If Cod_Rapporto <> 0 Then
                        strSql.Append(" AND Risorse_Umane.Cod_Rapporto = " & Agro_SQL_SaveNum(Cod_Rapporto) & " ")
                    End If

                    If Cod_RisUm_Origine <> 0 Then
                        strSql.Append(" AND  Risorse_Umane.Cod_RisUm_Origine = " & Agro_SQL_SaveNum(Cod_RisUm_Origine) & "   ")
                    End If

                    If Piva_SuperUser_Origine <> "" Then
                        strSql.Append(" AND  Risorse_Umane.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'")
                    End If

                    If Flag_AncheImportatati <> True Then
                        strSql.Append(" AND  Risorse_Umane.Cod_RisUm_Origine = " & Agro_SQL_SaveNum(0) & " ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.Append(" AND   Risorse_Umane.Inviato >=0 ")
                            strSql.Append(" AND   Contatti.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.Append(" AND   Risorse_Umane.Inviato =-1 ")
                            strSql.Append(" AND   Contatti.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.Append(" ORDER BY Risorse_Umane.Cod_Rapporto Asc , Risorse_Umane.Validita_Fine Desc ")
                    End If


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni
                    '
                    '
                    '
                    '


                Case enumSelezioneVariabile.Selezione_JoinCompleta
                    '
                    '
                    '
                    '


            End Select

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
    'uguale alla leggi, senza il join con contatti
    Public Function Leggi2(ByVal Piva As String,
                           ByVal Cod_Contatto As String,
                           ByVal Cod_RisUm As Long,
                           ByVal Cod_Rapporto As Long,
                           ByVal Cod_RisUm_Origine As Long,
                           ByVal Piva_SuperUser_Origine As String,
                           ByVal Flag_AncheImportatati As Boolean,
                           ByVal xFiltroAggiuntivo As String,
                           ByVal xOrderBy As String,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Risorse_Umane_R.Leggi2()"
        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.Append(" SELECT     Risorse_Umane.* ")
            strSql.Append(" FROM        Risorse_Umane ")
            strSql.Append(" INNER JOIN  UtentiXImprese ")
            strSql.Append("             ON Risorse_Umane.Piva = UtentiXImprese.PIVA ")
            strSql.Append(" WHERE       UtentiXImprese.[USER] = '" & Trim(objParametri.PivaSuperUser) & "' ")
            strSql.Append(" AND         Risorse_Umane.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            strSql.Append(" AND         Risorse_Umane.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Piva <> "" Then
                strSql.Append(" AND Risorse_Umane.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Cod_Contatto <> "" Then
                strSql.Append(" AND Risorse_Umane.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "' ")
            End If

            If Cod_RisUm <> 0 Then
                strSql.Append(" AND Risorse_Umane.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
            End If

            If Cod_Rapporto <> 0 Then
                strSql.Append(" AND Risorse_Umane.Cod_Rapporto = " & Agro_SQL_SaveNum(Cod_Rapporto) & " ")
            End If

            If Cod_RisUm_Origine <> 0 Then
                strSql.Append(" AND  Risorse_Umane.Cod_RisUm_Origine = " & Agro_SQL_SaveNum(Cod_RisUm_Origine) & "   ")
            End If

            If Piva_SuperUser_Origine <> "" Then
                strSql.Append(" AND  Risorse_Umane.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'")
            End If

            If Flag_AncheImportatati = False Then
                strSql.Append(" AND  Risorse_Umane.Cod_RisUm_Origine = " & Agro_SQL_SaveNum(0) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.Append(" AND   Risorse_Umane.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.Append(" AND   Risorse_Umane.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    '###################################################################################
    Public Function CodRisum_from_SettoreDes(ByVal Progressivo As String,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As Integer

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Risorse_Umane_R.CodRisum_from_SettoreDes()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim codRisum As Integer = 0

        Try

            Dim filtro As String = " UPPER(Risorse_Umane.Settore_Des) = '" & Agro_SQL_SaveText(Trim(Progressivo).ToUpper) & "' "

            dt = Leggi2("",
                        "",
                        0,
                        0, 0, "",
                        True,
                        filtro, "",
                        objParametri)

            If dt IsNot Nothing Then

                Select Case dt.Rows.Count
                    Case 0
                        'non trovato
                    Case 1
                        codRisum = dt.Rows(0).Item("Cod_Risum")
                    Case Is > 1
                        Throw New Exception("Sono stati trovate più risorse umane per il codice: " & Progressivo)
                End Select

            End If

            dt = Nothing

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return codRisum

    End Function

    '###################################################################################
    'a differenza della 1 chiama la leggi4
    'e ha il filtro cod_rapporto e un filtro aggiuntivo
    Public Function CodRisum_from_SettoreDes2(ByVal Progressivo As String,
                                              ByVal Cliente As Boolean,
                                              ByVal Fornitore As Boolean,
                                              ByVal Dipendente As Boolean,
                                              ByVal Terzista As Boolean,
                                              ByVal Legale As Boolean,
                                              ByVal Agente As Boolean,
                                              ByVal Consulente As Boolean,
                                              ByVal xFiltroAggiuntivo As String,
                                              ByRef objParametri As AgronicaCoreParametri
                                              ) As Integer

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Risorse_Umane_R.CodRisum_from_SettoreDes2()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim codRisum As Integer = 0

        Try

            dt = Leggi4("",
                        "",
                        0,
                        0,
                        Progressivo,
                        Cliente, Fornitore, Dipendente,
                        Terzista, Legale, Agente, Consulente,
                        xFiltroAggiuntivo, "",
                        objParametri)

            If dt IsNot Nothing Then

                Select Case dt.Rows.Count
                    Case 0
                        'non trovato
                    Case 1
                        codRisum = dt.Rows(0).Item("Cod_Risum")
                    Case Is > 1
                        Throw New Exception("Sono stati trovate più risorse umane per il codice: " & Progressivo)
                End Select

            End If

            dt = Nothing

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return codRisum

    End Function


    '###################################################################################
    Public Function Esiste_SettoreDes(ByVal Progressivo As String,
                                      ByRef objParametri As AgronicaCoreParametri
                                      ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Risorse_Umane_R.Esiste_SettoreDes()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim flagEsiste As Boolean = False

        Try

            Dim filtro As String = " Risorse_Umane.Settore_Des = '" & Agro_SQL_SaveText(Trim(Progressivo)) & "' "

            dt = Leggi2("",
                        "",
                        0,
                        0, 0, "",
                        True,
                        filtro, "",
                        objParametri)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                flagEsiste = True
            End If

            dt = Nothing

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return flagEsiste

    End Function

    '###################################################################################
    Public Function Esiste_SettoreDes_RitornaDati(ByVal Progressivo As String,
                                                  ByVal xFiltroAggiuntivo As String,
                                                  ByRef Piva As String,
                                                  ByRef cod_contatto As String,
                                                  ByRef rag_soc As String,
                                                  ByRef objParametri As AgronicaCoreParametri
                                                  ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Risorse_Umane_R.Esiste_SettoreDes_RitornaDati()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim flagEsiste As Boolean = False

        Try

            If xFiltroAggiuntivo <> "" Then
                xFiltroAggiuntivo &= " AND "
            End If
            xFiltroAggiuntivo &= " Risorse_Umane.Settore_Des = '" & Agro_SQL_SaveText(Trim(Progressivo)) & "' "

            dt = Leggi3("", "", 0, 0,
                        xFiltroAggiuntivo, "",
                        objParametri)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                flagEsiste = True
                Piva = dt.Rows(0).Item("Piva")
                cod_contatto = dt.Rows(0).Item("cod_contatto")
                rag_soc = dt.Rows(0).Item("rag_soc") & dt.Rows(0).Item("nome") & " " & dt.Rows(0).Item("cognome")
            End If

            dt = Nothing

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return flagEsiste

    End Function

    '###################################################################################
    Public Function Esiste_CodRisUm(ByVal Cod_RisUm As Integer,
                                    ByRef objParametri As AgronicaCoreParametri
                                    ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Risorse_Umane_R.Esiste_CodRisUm()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim flagEsiste As Boolean = False

        Try

            dt = Leggi2("",
                        "",
                        Cod_RisUm,
                        0, 0, "",
                        True,
                        "", "",
                        objParametri)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                flagEsiste = True
            End If

            dt = Nothing

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return flagEsiste

    End Function

    '###################################################################################
    Public Function CodRisUm_by_PivaCodContattoCodRapporto(ByVal Piva As String,
                                                           ByVal Cod_Contatto As String,
                                                           ByVal Cod_Rapporto As Integer,
                                                           ByRef objParametri As AgronicaCoreParametri
                                                           ) As Integer

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Risorse_Umane_R.CodRisUm_by_PivaCodContattoCodRapporto()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim codRisUm As Integer = 0

        Try

            If Cod_Rapporto = 0 Then
                Throw New Exception("Parametro non corretto nella query (Cod_Rapporto obbligatorio)")
            End If

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Cod_Contatto = "" Then
                Throw New Exception("Parametro non corretto nella query (Cod_Contatto obbligatorio)")
            End If

            '05/05/2015: corretto bug, non filtrava il cod_rapporto che veniva passato
            dt = Leggi2(Piva,
                        Cod_Contatto,
                        0,
                        Cod_Rapporto,
                        0, "",
                         True,
                        "", "",
                        objParametri)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                codRisUm = dt.Rows(0).Item("cod_Risum")
            End If

            If codRisUm = 0 Then
                Throw New Exception("Cod_RisUm = 0")
            End If

            dt = Nothing

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            codRisUm = 0
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return codRisUm

    End Function

    '###################################################################################
    'rispetto alla CodRisUm_by_PivaCodContattoCodRapporto, chiama la funzione Leggi
    'da usare quando non si sa esattamente la piva di creazione del contatto (per cui fa piva =... or sa_cod = -1)
    Public Function CodRisUm_by_PivaCodContattoCodRapporto2(ByVal Piva As String,
                                                           ByVal Cod_Contatto As String,
                                                           ByVal Cod_Rapporto As Integer,
                                                           ByRef objParametri As AgronicaCoreParametri
                                                           ) As Integer

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Risorse_Umane_R.CodRisUm_by_PivaCodContattoCodRapporto2()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim codRisUm As Integer = 0

        Try

            If Cod_Rapporto = 0 Then
                Throw New Exception("Parametro non corretto nella query (Cod_Rapporto obbligatorio)")
            End If

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Cod_Contatto = "" Then
                Throw New Exception("Parametro non corretto nella query (Cod_Contatto obbligatorio)")
            End If

            dt = Leggi(Piva,
                        Cod_Contatto,
                        0,
                        Cod_Rapporto,
                        0,
                        "",
                        True,
                        True,
                        "",
                        "",
                        objParametri)


            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                codRisUm = dt.Rows(0).Item("cod_Risum")
            End If

            If codRisUm = 0 Then
                Throw New Exception("Cod_RisUm = 0")
            End If

            dt = Nothing

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            codRisUm = 0
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return codRisUm

    End Function


    '###################################################################################
    Public Function CodRapporto_from_CodRisUm(ByVal Cod_RisUm As Integer,
                                              ByRef objParametri As AgronicaCoreParametri
                                              ) As Integer

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Risorse_Umane_R.CodRapporto_from_CodRisUm()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim codRapporto As Integer = 0

        Try

            dt = Leggi2("",
                        "",
                        Cod_RisUm,
                        0, 0, "",
                        True,
                        "", "",
                        objParametri)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                codRapporto = dt.Rows(0).Item("cod_rapporto")
            End If

            dt = Nothing

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return codRapporto

    End Function

    '###################################################################################
    Public Function ProvvigioneAgente_from_CodRisUm(ByVal Cod_RisUm As Integer,
                                              ByRef objParametri As AgronicaCoreParametri
                                              ) As Integer

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Risorse_Umane_R.ProvvigioneAgente_from_CodRisUm()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim Provvigione As Double = 0

        Try

            dt = Leggi3("",
                         "",
                            Cod_RisUm,
                            0,
                           "",
                           "",
                            objParametri)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                Provvigione = dt.Rows(0).Item("Provvigione")
            End If

            dt = Nothing

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return Provvigione

    End Function

    Public Function Esiste_RisorsaUmana(ByVal Piva As String,
                                        ByVal Cod_RisUm As Integer,
                                        ByVal Cod_Contatto As String,
                                        ByVal Cod_Rapporto As Integer,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByRef objParametri As AgronicaCoreParametri
                                        ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Risorse_Umane_R.Esiste_RisorsaUmana()"

        '====================================================================================
        'Parametri opzionali :
        '
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Dim bRet As Boolean = False

        Try
            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatoria)")
            End If

            If Cod_Contatto = "" Then
                Throw New Exception("Parametro non corretto nella query (Cod_Contatto obbligatorio)")
            End If

            If Cod_Rapporto = 0 Then
                Throw New Exception("Parametro non corretto nella query (Cod_Rapporto obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0
            strSql.Append(" SELECT * ")
            strSql.Append(" FROM  Risorse_Umane ")
            strSql.Append(" WHERE Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            strSql.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            strSql.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "'  ")
            strSql.Append(" AND Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "' ")
            strSql.Append(" AND Cod_Rapporto = " & Agro_SQL_SaveNum(Cod_Rapporto) & " ")

            If Cod_RisUm <> 0 Then
                strSql.Append(" AND Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & " ")
            End If


            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.Append(" AND   Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.Append(" AND   Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If dt IsNot Nothing Then
                If dt.Rows.Count > 0 Then
                    bRet = True
                Else
                    bRet = False
                End If
            End If

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            bRet = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        dt = Nothing
        Return bRet

    End Function

    '#########################################
    'cerca per piva, cod_contatto, cod_rapporto e ritorna il cod_risum
    Public Function Esiste_RisorsaUmana2(ByVal Piva As String,
                                         ByVal Cod_Contatto As String,
                                         ByVal Cod_Rapporto As Integer,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByRef Cod_RisUm As Integer,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Risorse_Umane_R.Esiste_RisorsaUmana2()"
        Dim messaggioErrore As String = ""
        Dim bRet As Boolean = False
        Dim dt As DataTable
        Cod_RisUm = 0

        Try
            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatoria)")
            End If

            If Cod_Contatto = "" Then
                Throw New Exception("Parametro non corretto nella query (Cod_Contatto obbligatorio)")
            End If

            If Cod_Rapporto = 0 Then
                Throw New Exception("Parametro non corretto nella query (Cod_Rapporto obbligatorio)")
            End If

            dt = Leggi2(Piva,
                        Cod_Contatto,
                        0,
                        Cod_Rapporto,
                        0, "", True,
                        "", "",
                        objParametri)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                bRet = True
                Cod_RisUm = dt.Rows(0).Item("Cod_RisUm")
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            bRet = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return bRet

    End Function



    '##############################################################################################
    Public Function LeggiContattixSuperUser(ByVal PIVA As String,
                                            ByVal Cod_RisUm As Long,
                                            ByVal Cod_Contatto As String,
                                            ByVal Cod_Rapporto As Long,
                                            ByVal Cod_RisUm_Origine As Long,
                                            ByVal Piva_SuperUser_Origine As String,
                                            ByVal Flag_AncheImportatati As Boolean,
                                            ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreParametri
                                            ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Risorse_Umane_R.LeggiContattixSuperUser()"

        '====================================================================================
        'Parametri opzionali :
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                Case enumSelezioneVariabile.Selezione_JoinCompleta
                    strSql.Length = 0
                    strSql.Append(" SELECT      Risorse_Umane.*, Rapporti_Contabili.*, Contatti.*,  ")
                    strSql.Append("             Risorse_Umane.Validita_Inizio AS Validita_Inizio, Risorse_Umane.Validita_Fine AS Validita_Fine ")
                    strSql.Append("  ,  ISNULL((SELECT  TOP 1 Prodotti_Costi.prezzo_unitario ")
                    strSql.Append("             FROM    Prodotti_Costi ")
                    strSql.Append("             WHERE   elem_cod = 0 AND Risorse_Umane.cod_risum = mat_cod And Id_Budget = 0 ), 0) AS prezzo_unitario ")

                    strSql.Append(" FROM        Contatti ")
                    strSql.Append(" INNER JOIN  Risorse_Umane ")
                    strSql.Append("             ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto AND Contatti.Piva = Risorse_Umane.Piva ")
                    strSql.Append(" INNER JOIN  Rapporti_Contabili ")
                    strSql.Append("             ON Risorse_Umane.Cod_Rapporto = Rapporti_Contabili.Cod_Rapporto ")
                    strSql.Append(" INNER JOIN  UtentiXImprese ")
                    strSql.Append("             ON Contatti.Piva = UtentiXImprese.PIVA AND Rapporti_Contabili.Piva = UtentiXImprese.[USER] ")
                    strSql.Append(" WHERE       (Rapporti_Contabili.Piva = '" & Trim(objParametri.PivaSuperUser) & "') ")
                    strSql.Append(" AND         Risorse_Umane.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.Append(" AND         Risorse_Umane.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If PIVA <> "" Then
                        strSql.Append(" AND ( Contatti.Piva = '" & Agro_SQL_SaveText(Trim(PIVA)) & "'  OR  Contatti.Sa_Cod = -1 )  ")
                    End If

                    If Cod_Contatto <> "" Then
                        strSql.Append(" AND Risorse_Umane.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "' ")
                    End If

                    If Cod_RisUm <> 0 Then
                        strSql.Append(" AND Risorse_Umane.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
                    End If

                    If Cod_Rapporto <> 0 Then
                        strSql.Append(" AND Risorse_Umane.Cod_Rapporto = " & Agro_SQL_SaveNum(Cod_Rapporto) & " ")
                    End If

                    If Cod_RisUm_Origine <> 0 Then
                        strSql.Append(" AND  Risorse_Umane.Cod_RisUm_Origine = " & Agro_SQL_SaveNum(Cod_RisUm_Origine) & "   ")
                    End If

                    If Piva_SuperUser_Origine <> "" Then
                        strSql.Append(" AND  Risorse_Umane.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'")
                    End If

                    If Flag_AncheImportatati <> True Then
                        strSql.Append(" AND  Risorse_Umane.Cod_RisUm_Origine = " & Agro_SQL_SaveNum(0) & " ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.Append(" AND   Risorse_Umane.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.Append(" AND   Risorse_Umane.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.Append(" ORDER BY Risorse_Umane.Cod_Rapporto Asc , Risorse_Umane.Validita_Fine Desc ")
                    End If

            End Select

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
    Public Function LeggiJoinRapportiContabili(ByVal Piva As String,
                                               ByVal Cod_Contatto As String,
                                               ByVal Cod_RisUm As Long,
                                               ByVal Cod_Rapporto As Long,
                                               ByVal Chk_Cliente As Integer,
                                               ByVal Chk_Fornitore As Integer,
                                               ByVal Chk_Dipendente As Integer,
                                               ByVal Chk_Terzista As Integer,
                                               ByVal Chk_Legale As Integer,
                                               ByVal Chk_Agente As Integer,
                                               ByVal Cod_RisUm_Origine As Long,
                                               ByVal Piva_SuperUser_Origine As String,
                                               ByVal Flag_AncheImportatati As Boolean,
                                               ByVal xFiltroAggiuntivo As String,
                                               ByVal xOrderBy As String,
                                               ByRef objParametri As AgronicaCoreParametri
                                               ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Risorse_Umane_R.LeggiJoinRapportiContabili()"
        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.Append(" SELECT     Contatti.ID_CF, Rag_Soc, Nome, Cognome,Data_Nascita, Sesso,  ")
            strSql.Append("             Risorse_Umane.*, ")
            strSql.Append("             Rapporto_Des, Cliente, Fornitore, Dipendente, Terzista, Legale, Agente ")

            strSql.Append(" FROM        Contatti ")
            strSql.Append(" INNER JOIN  Risorse_Umane ")
            strSql.Append("             ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto AND Contatti.Piva = Risorse_Umane.Piva ")
            strSql.Append(" INNER JOIN  Rapporti_Contabili ")
            strSql.Append("             ON Risorse_Umane.Cod_Rapporto = Rapporti_Contabili.Cod_Rapporto ")
            strSql.Append(" INNER JOIN  UtentiXImprese ")
            strSql.Append("             ON Contatti.Piva = UtentiXImprese.PIVA AND Rapporti_Contabili.Piva = UtentiXImprese.[USER] ")
            strSql.Append(" WHERE       Rapporti_Contabili.Piva = '" & Trim(objParametri.PivaSuperUser) & "' ")
            strSql.Append(" AND         Risorse_Umane.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            strSql.Append(" AND         Risorse_Umane.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Piva <> "" Then
                strSql.Append(" AND Risorse_Umane.Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "'   ")
            End If

            If Cod_Contatto <> "" Then
                strSql.Append(" AND Risorse_Umane.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "' ")
            End If

            If Cod_RisUm <> 0 Then
                strSql.Append(" AND Risorse_Umane.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
            End If

            If Cod_Rapporto <> 0 Then
                strSql.Append(" AND Risorse_Umane.Cod_Rapporto = " & Agro_SQL_SaveNum(Cod_Rapporto) & " ")
            End If

            If Cod_RisUm_Origine <> 0 Then
                strSql.Append(" AND  Risorse_Umane.Cod_RisUm_Origine = " & Agro_SQL_SaveNum(Cod_RisUm_Origine) & "   ")
            End If

            If Piva_SuperUser_Origine <> "" Then
                strSql.Append(" AND  Risorse_Umane.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'")
            End If

            If Flag_AncheImportatati = False Then
                strSql.Append(" AND  Risorse_Umane.Cod_RisUm_Origine = " & Agro_SQL_SaveNum(0) & " ")
            End If

            If Not (Chk_Agente = 0 AndAlso Chk_Fornitore = 0 AndAlso Chk_Cliente = 0 AndAlso Chk_Dipendente = 0 AndAlso Chk_Terzista = 0 AndAlso Chk_Legale = 0) Then
                strSql.Append(" AND Rapporti_Contabili.Cliente = " & Agro_SQL_SaveNum(Chk_Cliente) & " ")
                strSql.Append(" AND Rapporti_Contabili.Fornitore = " & Agro_SQL_SaveNum(Chk_Fornitore) & " ")
                strSql.Append(" AND Rapporti_Contabili.Dipendente = " & Agro_SQL_SaveNum(Chk_Dipendente) & " ")
                strSql.Append(" AND Rapporti_Contabili.Terzista = " & Agro_SQL_SaveNum(Chk_Terzista) & " ")
                strSql.Append(" AND Rapporti_Contabili.Legale = " & Agro_SQL_SaveNum(Chk_Legale) & " ")
                strSql.Append(" AND Rapporti_Contabili.Agente = " & Agro_SQL_SaveNum(Chk_Agente) & " ")
            End If


            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.Append(" AND   Risorse_Umane.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.Append(" AND   Risorse_Umane.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    Public Function LeggiRapportoSpecifico(ByVal piva As String,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByVal xOrderBy As String,
                                           ByRef objParametri As AgronicaCoreParametri,
                                           Optional ByVal Rapporto_Attivo As Boolean = False,
                                           Optional ByVal Cliente As Boolean = False,
                                           Optional ByVal Fornitore As Boolean = False,
                                           Optional ByVal Dipendente As Boolean = False,
                                           Optional ByVal Terzista As Boolean = False,
                                           Optional ByVal Legale As Boolean = False,
                                           Optional ByVal Agente As Boolean = False,
                                           Optional ByVal Consulente As Boolean = False,
                                           Optional ByVal FinestraTemp_Inizio As Date = AGRODATAINIZIO,
                                           Optional ByVal FinestraTemp_Fine As Date = AGRODATAFINE,
                                           Optional ByVal Cod_RisUm_Origine As Long = 0,
                                           Optional ByVal Piva_SuperUser_Origine As String = "",
                                           Optional ByVal Flag_AncheImportatati As Boolean = True,
                                           Optional ByVal Classificazione_Cod As Long = 0
                                           ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Risorse_Umane_R.LeggiRapportoSpecifico()"
        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT Distinct Risorse_Umane.* , Rapporti_Contabili.* , Contatti.*, ")
            strSql.AppendLine(" Risorse_Umane.Validita_Inizio as XValidita_Inizio, Risorse_Umane.Validita_Fine as XValidita_Fine, ")
            strSql.AppendLine(" Contatti.Rag_Soc + Contatti.Cognome + ' ' + Contatti.Nome AS Rag_Soc_Contatto, ")
            strSql.AppendLine(" Contatti.Piva As Piva_Contatto, Contatti.Sa_Cod AS Sa_Cod_Contatto, ")
            strSql.AppendLine(" CASE WHEN Imprese.PIVA IS NULL THEN 0 ELSE 1 END AS IsImpresaGias ")

            strSql.AppendLine(" FROM  Risorse_Umane ")
            strSql.AppendLine(" INNER JOIN Rapporti_Contabili ON Risorse_Umane.Cod_Rapporto = Rapporti_Contabili.Cod_Rapporto ")
            strSql.AppendLine(" INNER JOIN Contatti ON Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto AND Risorse_Umane.Piva = Contatti.Piva ")
            strSql.AppendLine(" LEFT JOIN Imprese ON Contatti.Cod_Contatto = Imprese.PIVA ")

            strSql.AppendLine(" WHERE 1 = 1 ")

            'Visibilità
            strSql.AppendLine(QueryVisibilitaCentriPerContatti(piva, objParametri))

            'strSql.AppendLine(" AND ( Contatti.Piva = '" & Trim(Agro_SQL_SaveText(piva)) & "'  OR  Contatti.Sa_Cod = -1 )  ")

            strSql.AppendLine(" AND   Risorse_Umane.Validita_inizio <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " ")
            strSql.AppendLine(" AND   Risorse_Umane.Validita_Fine >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")

            If Cliente Then
                strSql.AppendLine(" AND Rapporti_Contabili.Cliente = 1 ")
            End If

            If Fornitore Then
                strSql.AppendLine(" AND Rapporti_Contabili.Fornitore = 1 ")
            End If

            If Dipendente Then
                strSql.AppendLine(" AND Rapporti_Contabili.Dipendente = 1 ")
            End If

            If Terzista Then
                strSql.AppendLine(" AND Rapporti_Contabili.Terzista = 1 ")
            End If

            If Legale Then
                strSql.AppendLine(" AND Rapporti_Contabili.Legale = 1 ")
            End If

            If Agente Then
                strSql.AppendLine(" AND Rapporti_Contabili.Agente = 1 ")
            End If

            If Consulente Then
                strSql.AppendLine(" AND Rapporti_Contabili.Consulente = 1 ")
            End If

            If Cod_RisUm_Origine <> 0 Then
                strSql.AppendLine(" AND  Risorse_Umane.Cod_RisUm_Origine = " & Agro_SQL_SaveNum(Cod_RisUm_Origine) & "   ")
            End If

            If Piva_SuperUser_Origine <> "" Then
                strSql.AppendLine(" AND  Risorse_Umane.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'")
            End If

            If Flag_AncheImportatati <> True Then
                strSql.AppendLine(" AND  Risorse_Umane.Cod_RisUm_Origine = 0 ")
            End If

            If Classificazione_Cod <> 0 Then
                strSql.AppendLine(" AND  Risorse_Umane.Classificazione_Cod = " & Agro_SQL_SaveNum(Classificazione_Cod) & "   ")
            End If

            '==================================================================================
            'Filtro sulle Risorse Umane ancora Attive
            '----------------------------------------------------------------------------------
            If Rapporto_Attivo Then
                strSql.AppendLine(" AND Risorse_Umane.Validita_Fine >= " & Agro_SQL_SaveDate(Now) & " ")
            End If
            '==================================================================================

            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.Append(" AND   Risorse_Umane.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.Append(" AND   Risorse_Umane.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Risorse_Umane.Validita_Fine Desc ")
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

    Private Function QueryVisibilitaCentriPerContatti(ByVal piva As String,
                                                      ByRef objParametri As AgronicaCoreParametri
                                                      ) As String

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.QryVisibilitaCentriPerContatti()"
        Dim strSql As New StringBuilder

        Try

            'PUBBLICHE
            strSql.AppendLine(" AND    ( (Contatti.Sa_Cod = -1) ")

            'VISIBILITA CENTRI
            'leggo se ci sono filtri sui centri
            Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
            Dim filtroCentri As String = ""
            Dim dtCentriVisibili As DataTable
            dtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro,
                                                         " Piva='" & Agro_SQL_SaveText(piva) & "'",
                                                         "", objParametri)
            If dtCentriVisibili IsNot Nothing AndAlso dtCentriVisibili.Rows.Count > 0 Then
                For i = 0 To dtCentriVisibili.Rows.Count - 1
                    filtroCentri &= " (Contatti.Piva = '" & dtCentriVisibili.Rows(i).Item("piva") & "' AND Contatti.Sa_Cod = " & dtCentriVisibili.Rows(i).Item("sa_cod") & ") OR "
                Next
                If filtroCentri <> "" Then
                    strSql.AppendLine(" OR (" & Left(filtroCentri, filtroCentri.Length - 3) & " OR (Contatti.Piva = '" & Agro_SQL_SaveText(piva) & "' AND Contatti.Sa_Cod = 0) ) ")
                End If
            End If

            'AZIENDALI
            If filtroCentri = "" Then
                strSql.AppendLine("          OR  (Contatti.Piva = '" & Agro_SQL_SaveText(piva) & "')  ")
            End If

            strSql.AppendLine("        ) ")

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return strSql.ToString

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="piva"></param>
    ''' <param name="Rapporto_Attivo"></param>
    ''' <param name="Tipo_Rapporto">0 = Clienti, 1 = Fornitori, 2 = Professionisti, 3 = Dipendenti+Terzisti, 4 = Agenti, 5 = Capo Area, 6 = Conferenti x Accettazione, 7 = Vettori, 8 = Clienti+Fornitori, 9 = Terzisti(no filtro), 10 = Dipendenti</param>
    ''' <param name="cercaSoloValidi"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <param name="FinestraTemp_Inizio"></param>
    ''' <param name="FinestraTemp_Fine"></param>
    ''' <returns></returns>
    Public Function LeggiRapportoSpecificoxDocumenti(ByVal piva As String,
                                                     ByVal Rapporto_Attivo As Boolean,
                                                     ByVal Tipo_Rapporto As Integer,
                                                     ByVal cercaSoloValidi As Boolean,
                                                     ByVal rapportiSpecificiStrIN As String,
                                                     ByVal xFiltroAggiuntivo As String,
                                                     ByVal xOrderBy As String,
                                                     ByRef objParametri As AgronicaCoreParametri,
                                                     Optional ByVal accettazioneConGerarchia As Integer = 0,
                                                     Optional ByVal pivaPadreGerarchia As String = "",
                                                     Optional ByVal dataValidita As Date = #2/1/1900#,
                                                     Optional ByVal FinestraTemp_Inizio As Date = AGRODATAINIZIO,
                                                     Optional ByVal FinestraTemp_Fine As Date = AGRODATAFINE,
                                                     Optional ByVal TestoRicerca As String = Nothing,
                                                     Optional ByVal includiIndirizzo As Boolean = True,
                                                     Optional ByVal checkRaccolteNonCollegateConferimento As Boolean = False,
                                                     Optional ByVal dataFineRaccolte As Date = #2/1/1900#
                                                     ) As DataTable

        '==================================================================================
        '   Tipo_Rapporto
        '----------------------------------------------------------------------------------
        '   0 = Clienti
        '   1 = Fornitori
        '   2 = Professionisti
        '   3 = Dipendenti+Terzisti
        '   4 = Agenti
        '   5 = Capo Area
        '   6 = Conferenti x Accettazione
        '   7 = Vettori
        '   8 = Clienti+Fornitori
        '   9 = Terzisti (senza ulteriore filtro rapporti contabili)
        '   10 = Dipendenti
        '   11 = Referente_Conferimento
        '==================================================================================
        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Risorse_Umane_R.LeggiRapportoSpecificoxDocumenti()"
        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable
        Dim filtroRapporto As String = ""

        If dataValidita = #2/1/1900# Then
            dataValidita = Now
        End If

        Try

            strSql.Length = 0

            ' Scatto 13/9/2024 per velocizzare lettura
            ' strSql.AppendLine(" SELECT Distinct Risorse_Umane.* , Rapporti_Contabili.* , Contatti.* ")
            strSql.AppendLine(" SELECT Distinct Contatti.Cod_Contatto, Contatti.Rag_Soc, Contatti.Cognome, Contatti.Nome, Rapporto_Des, ")
            strSql.AppendLine("   Cod_RisUm, Id_CF, Codice_Fiscale, Tipo_Indirizzo_Default, Cod_Risum_Destinazione_Diversa, ")
            strSql.AppendLine("   Tipo_Indirizzo_Default_Destinazione_Diversa, Vettore_Cod, Agente_Cod, Provvigione, CapoArea_Cod, ")
            strSql.AppendLine("   Provvigione_CapoArea, Attivita_Des, Settore_Des, Contatti.Cod_Iva_Contatto ")

            If accettazioneConGerarchia <> 0 Then
                strSql.AppendLine(" , GerarchiaImprese.* ")
            End If

            strSql.AppendLine(" , Risorse_Umane.Validita_Inizio as XValidita_Inizio, Risorse_Umane.Validita_Fine as XValidita_Fine ")
            strSql.AppendLine(" , Contatti.Rag_Soc + Contatti.Cognome + ' ' + Contatti.Nome AS Rag_Soc_Contatto ")
            strSql.AppendLine(" , Contatti.Piva As Piva_Contatto, Contatti.Sa_Cod AS Sa_Cod_Contatto ")
            strSql.AppendLine(" , CASE WHEN Imprese.PIVA IS NULL THEN 0 ELSE 1 END AS IsImpresaGias ")
            strSql.AppendLine(" , CASE WHEN Imprese.PIVA IS NULL THEN 1 ELSE Imprese.Compliance_ISCC END AS Compliance_ISCC ")

            If includiIndirizzo Then
                strSql.AppendLine(" , ISNULL(Indirizzi.Stato, '') AS Stato, ISNULL(Paesi.Descrizione, '') AS Stato_Des ")
                strSql.AppendLine(" , ISNULL(Indirizzi.Pro_Cod_Istat, '') AS Pro_Cod_Istat, ISNULL(Lista_Province.Provincia, '') AS Provincia_Des ")
            End If

            Select Case Tipo_Rapporto

                Case 0 'Clienti

                    filtroRapporto = " (Rapporti_Contabili.Cliente = 1 OR Rapporti_Contabili.Terzista = 1) "

                Case 1 'Fornitori

                    filtroRapporto = " (Rapporti_Contabili.Fornitore = 1 OR Rapporti_Contabili.Terzista = 1) "

                Case 2 'Professionisti

                    filtroRapporto = " (Rapporti_Contabili.Agente = 1 OR Rapporti_Contabili.Consulente = 1) "

                Case 3 'Dipendenti+Terzisti

                    filtroRapporto = " (Rapporti_Contabili.Dipendente = 1 OR Rapporti_Contabili.Terzista = 1) "

                Case 4 'Agenti

                    filtroRapporto = " (Rapporti_Contabili.Agente = 1 AND Rapporti_Contabili.Cod_Rapporto <> " & enum_Rapporti_Contabili_Standard.Capo_Area & ") "

                Case 5 'Capo Area

                    filtroRapporto = " (Rapporti_Contabili.Agente = 1 AND Rapporti_Contabili.Cod_Rapporto = " & enum_Rapporti_Contabili_Standard.Capo_Area & ") "

                Case 6 'Conferenti x Accettazione

                    filtroRapporto = " (Rapporti_Contabili.Fornitore = 1 AND Rapporti_Contabili.Cliente = 1) "

                Case 7 'Vettori
                    filtroRapporto = " (Rapporti_Contabili.Terzista = 1 AND Rapporti_Contabili.Cod_Rapporto IN (" &
                                     enum_Rapporti_Contabili_Standard.Terzista & "," &
                                     enum_Rapporti_Contabili_Standard.Trasportatore & "," &
                                     enum_Rapporti_Contabili_Standard.Spedizioniere & ")) "

                Case 8 'Clienti OR Fornitori
                    filtroRapporto = " (Rapporti_Contabili.Cliente = 1 OR Rapporti_Contabili.Fornitore = 1) "

                Case 9 'Terzisti (senza ulteriore filtro rapporti contabili)
                    filtroRapporto = " (Rapporti_Contabili.Terzista = 1) "

                Case 10 'Dipendenti
                    filtroRapporto = " (Rapporti_Contabili.Dipendente = 1) "

                Case 11 'Referente_Conferimento
                    filtroRapporto = "(Rapporti_Contabili.Cod_Rapporto = " & enum_Rapporti_Contabili_Standard.Referente_Conferimento & ") "
            End Select

            If filtroRapporto <> "" Then
                If rapportiSpecificiStrIN <> "" Then
                    filtroRapporto &= " AND Risorse_Umane.Cod_Rapporto IN (" & Agro_SQL_Save_Clausola_IN(rapportiSpecificiStrIN, False) & ") "
                End If

                strSql.AppendLine(" , (SELECT CASE WHEN (" & filtroRapporto & ") THEN 1 ELSE 0 END) AS Valido")
            End If

            If checkRaccolteNonCollegateConferimento = True Then
                strSql.AppendLine(" , ISNULL(Agenda_Raccolte.Lav_Cod, 0) AS LavCod_Raccolta")
            Else
                strSql.AppendLine(" , 0 AS LavCod_Raccolta")
            End If

            strSql.AppendLine(" , ISNULL(Contatti_Codici.Val_Cod, 0) AS Modalita_Pagamento")

            strSql.AppendLine(" FROM  Risorse_Umane ")
            strSql.AppendLine(" INNER JOIN Rapporti_Contabili ON Risorse_Umane.Cod_Rapporto = Rapporti_Contabili.Cod_Rapporto ")
            strSql.AppendLine(" INNER JOIN Contatti ON Risorse_Umane.Piva = Contatti.Piva AND Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto ")
            strSql.AppendLine(" LEFT JOIN Contatti_Codici ON Contatti.Piva = Contatti_Codici.Piva AND Contatti.Cod_Contatto = Contatti_Codici.Cod_Contatto AND Contatti_Codici.Id_Cod = " & enum_CodiciAnagrafe.ModalitaPagamentoDefault)

            If accettazioneConGerarchia <> 0 Then
                strSql.AppendLine(" INNER JOIN Imprese ON Contatti.Cod_Contatto = Imprese.PIVA ")
                strSql.AppendLine(" INNER JOIN GerarchiaImprese ON Imprese.Piva = GerarchiaImprese.Figlio ")
            Else
                strSql.AppendLine(" LEFT JOIN Imprese ON Contatti.Cod_Contatto = Imprese.PIVA ")
            End If

            If includiIndirizzo Then
                strSql.AppendLine(" LEFT OUTER JOIN ContattixIndirizzi ")
                strSql.AppendLine("    ON Contatti.Piva = ContattixIndirizzi.Piva ")
                strSql.AppendLine("   AND Contatti.Cod_Contatto = ContattixIndirizzi.Cod_Contatto ")
                strSql.AppendLine("   AND Contatti.Tipo_Indirizzo_Default = ContattixIndirizzi.Tipo_Indirizzo ")
                strSql.AppendLine(" LEFT OUTER JOIN Indirizzi ON ContattixIndirizzi.Cod_Indirizzo = Indirizzi.Cod_Indirizzo ")
                strSql.AppendLine(" LEFT OUTER JOIN ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166 Paesi ")
                strSql.AppendLine("    ON (Indirizzi.Stato = Paesi.Codice OR Indirizzi.Stato = Paesi.Descrizione) ")
                strSql.AppendLine(" LEFT OUTER JOIN Lista_Province ON Indirizzi.pro_cod_istat = Lista_Province.PROV ")
            End If

            If checkRaccolteNonCollegateConferimento Then
                Dim arrLavCodAccettazioniConf As Integer() = New Integer() {LAVCOD_ACCETTAZIONE_DIVERSI, LAVCOD_DISTINTA_CARICO_ACCETTAZIONE, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE}

                If dataFineRaccolte = #2/1/1900# OrElse dataFineRaccolte = Nothing Then
                    dataFineRaccolte = Now
                End If

                'TODO Sostituire il calcolo fisso leggendo la relativa impostazione
                Dim dataInizioRaccolte = dataFineRaccolte.AddDays(-31)

                'Individuo per ogni impresa se c'è almeno una raccolta collegabile
                strSql.AppendLine(" LEFT JOIN (")
                strSql.AppendLine("     SELECT DISTINCT Agenda.Piva, Agenda.Lav_Cod")
                strSql.AppendLine("     FROM Agenda")

                strSql.AppendLine("     INNER JOIN Movimenti")
                strSql.AppendLine("     ON Agenda.Piva = Movimenti.Piva")
                strSql.AppendLine("     AND Agenda.Id_Agenda = Movimenti.Id_Agenda")

                strSql.AppendLine("     LEFT JOIN Movimenti Movimenti_Carico")
                strSql.AppendLine("     ON Agenda.Piva = Movimenti_Carico.Piva")
                strSql.AppendLine("     AND Agenda.Id_Agenda = Movimenti_Carico.Id_Agenda")

                strSql.AppendLine("     LEFT JOIN Movimenti_dettagli")
                strSql.AppendLine("     ON Movimenti_dettagli.Piva = Movimenti_Carico.Piva")
                strSql.AppendLine("     AND Movimenti_dettagli.Id_Agenda = Movimenti_Carico.Id_Agenda")
                strSql.AppendLine("     AND Movimenti_dettagli.Id_Mov = Movimenti_Carico.Id_Mov")

                strSql.AppendLine("     LEFT JOIN Mov_Dettagli_Riferimenti")
                strSql.AppendLine("     ON Mov_Dettagli_Riferimenti.Piva_Rif = Agenda.Piva")
                strSql.AppendLine("     AND Mov_Dettagli_Riferimenti.Id_Agenda_Rif = Agenda.Id_Agenda")
                strSql.AppendLine("     AND (Mov_Dettagli_Riferimenti.Id_Mov_Rif = Movimenti_Carico.Id_Mov OR Mov_Dettagli_Riferimenti.Id_Mov_Rif = -1)")
                strSql.AppendLine("     AND (Mov_Dettagli_Riferimenti.Id_Mov_Det_Rif = Movimenti_dettagli.Id_Mov_Det OR Mov_Dettagli_Riferimenti.Id_Mov_Det_Rif = -1)")
                strSql.AppendLine("     AND Mov_Dettagli_Riferimenti.Lav_Cod IN (" & Agro_SQL_Save_Clausola_IN(String.Join(", ", arrLavCodAccettazioniConf)) & ")")

                strSql.AppendLine("     WHERE agenda.Lav_Cod = " & LAVCOD_RACCOLTA)
                strSql.AppendLine("     AND Movimenti.Cau_Mov = '" & CAU_RILIEVO_RACCOLTA & "'")
                strSql.AppendLine("     AND Movimenti_Carico.Cau_Mov = '" & CAU_CARICO & "'")
                strSql.AppendLine("     AND Mov_Dettagli_Riferimenti.Lav_Cod IS NULL")
                strSql.AppendLine("     AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(dataFineRaccolte))
                strSql.AppendLine("     AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(dataInizioRaccolte))
                strSql.AppendLine(" ) AS Agenda_Raccolte")
                strSql.AppendLine(" ON Contatti.Cod_Contatto = Agenda_Raccolte.PIVA")
            End If

            strSql.AppendLine(" WHERE 1 = 1 ")

            'Visibilità
            strSql.AppendLine(QueryVisibilitaCentriPerContatti(piva, objParametri))

            strSql.AppendLine(" AND   Risorse_Umane.Validita_inizio <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " ")
            strSql.AppendLine(" AND   Risorse_Umane.Validita_Fine >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")

            If cercaSoloValidi AndAlso filtroRapporto <> "" Then
                strSql.AppendLine(" AND (" & filtroRapporto & ") ")
            End If

            Select Case accettazioneConGerarchia

                Case 1  'Conferente
                    If pivaPadreGerarchia <> "" Then
                        strSql.AppendLine(" AND GerarchiaImprese.Padre = '" & Agro_SQL_SaveText(pivaPadreGerarchia) & "' ")
                    End If

                Case 2, 3  'Cooperativa, 2° Cooperativa
                    strSql.AppendLine(" AND GerarchiaImprese.Foglia =  0")
                    If pivaPadreGerarchia <> "" Then
                        strSql.AppendLine(" AND GerarchiaImprese.Padre = '" & Agro_SQL_SaveText(pivaPadreGerarchia) & "' ")
                    End If

                Case 4  'Produttore
                    If pivaPadreGerarchia <> "" Then
                        strSql.AppendLine(" AND GerarchiaImprese.Padre = '" & Agro_SQL_SaveText(pivaPadreGerarchia) & "' ")
                    End If

            End Select

            '==================================================================================
            'Filtro sulle Risorse Umane Attive alla data
            '----------------------------------------------------------------------------------
            If Rapporto_Attivo Then
                strSql.AppendLine(" AND Risorse_Umane.Validita_Inizio <= " & Agro_SQL_SaveDate(dataValidita) & " ")
                strSql.AppendLine(" AND Risorse_Umane.Validita_Fine >= " & Agro_SQL_SaveDate(dataValidita) & " ")

                If accettazioneConGerarchia <> 0 Then
                    strSql.AppendLine(" AND GerarchiaImprese.Validita_Inizio <= " & Agro_SQL_SaveDate(dataValidita) & " ")
                    strSql.AppendLine(" AND GerarchiaImprese.Validita_Fine >= " & Agro_SQL_SaveDate(dataValidita) & " ")
                End If
            End If
            '==================================================================================

            Dim filtroCliente As String = ""
            If TestoRicerca IsNot Nothing Then
                Dim objFilters As JArray = Nothing
                If Not String.IsNullOrEmpty(TestoRicerca) Then
                    objFilters = JArray.Parse(TestoRicerca)
                    If objFilters IsNot Nothing Then
                        For Each obj As JObject In objFilters
                            filtroCliente = "%" & obj("value").ToString() & "%"
                        Next
                    End If
                End If
            End If

            If filtroCliente <> "" Then
                strSql.AppendLine(" AND ( Contatti.Rag_Soc LIKE '" & Agro_SQL_SaveText(filtroCliente) & "' ")
                strSql.AppendLine(" OR Contatti.Cod_Contatto LIKE '" & Agro_SQL_SaveText(filtroCliente) & "' ")
                strSql.AppendLine(" OR Contatti.Nome LIKE '" & Agro_SQL_SaveText(filtroCliente) & "' ")
                strSql.AppendLine(" OR Contatti.Cognome LIKE '" & Agro_SQL_SaveText(filtroCliente) & "' )")
            End If


            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Risorse_Umane.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Risorse_Umane.Inviato =-1 ")
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

    '###################################################################################
    Public Function LeggiContattiSenzaCodice(ByVal Piva As String,
                                             ByVal filtroConAND As String,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Risorse_Umane_R.LeggiContattiSenzaCodice()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable

        Try

            Dim filtro As String = " UPPER(Risorse_Umane.Settore_Des) = '' " &
                                    "  AND    (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' OR Contatti.Sa_Cod = -1 ) " &
                                    Agro_SQL_Save_xFiltroAggiuntivo(filtroConAND,, objParametri)

            dt = Leggi3("",
                        "",
                         0,
                        0,
                        filtro, "",
                        objParametri)


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    'Query utilizzata in AgronicaCoreDemetraBIZ/ExportContatti
    Public Function VerificaUtilizzoContattoQdca(ByRef objParametriServer As AgronicaCoreParametri, ByVal lstCodRisum As String) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Risorse_Umane_R.VerificaUtilizzoContattoQdc()"

        Dim dt As DataTable
        Dim strSql As New StringBuilder

        Try
            strSql.Length = 0

            If String.IsNullOrWhiteSpace(lstCodRisum) Then
                Throw New Exception("Lista Codici Risorse Umane vuota o nulla.")
            End If

            strSql.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")
            strSql.AppendLine(" SELECT 1 As Utilizzato ")
            strSql.AppendLine(" WHERE EXISTS ( ")
            strSql.AppendLine("   SELECT Mat_Cod ")
            strSql.AppendLine("   FROM Movimenti m ")
            strSql.AppendLine("   INNER JOIN Movimenti_dettagli md ")
            strSql.AppendLine("   on md.PIVA = m.PIVA and md.Id_Agenda = m.Id_Agenda AND md.Id_Mov = m.Id_Mov ")
            strSql.AppendLine("   WHERE m.Cau_Mov = '" & CostantiPersonalizzate.CAU_IMPUTAZIONE_MANODOPERA & "' ")
            strSql.AppendLine("   AND md.Elem_Cod = 0 ")
            strSql.AppendLine("   AND md.Mat_Cod IN (" & Agro_SQL_Save_Clausola_IN(lstCodRisum, False) & ") ")
            strSql.AppendLine("   UNION ALL ")
            strSql.AppendLine("   SELECT Mat_Cod ")
            strSql.AppendLine("   FROM Ricette_Dettagli rd ")
            strSql.AppendLine("   WHERE rd.Cau_Mov = '" & CostantiPersonalizzate.CAU_IMPUTAZIONE_MANODOPERA & "' ")
            strSql.AppendLine("   AND rd.Elem_Cod = 0 ")
            strSql.AppendLine("   AND rd.Mat_Cod IN (" & Agro_SQL_Save_Clausola_IN(lstCodRisum, False) & ") ")
            strSql.AppendLine(" ) ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametriServer, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Scrivi_LOG(objParametriServer, nomeRoutine, ex.ToString)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return dt

    End Function

End Class



'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################



Public Class Risorse_Umane_W
    Inherits AgronicaCoreDataProvider.DataProvider


    '############################################################################ 
    Public Function Scrivi(ByVal Piva As String,
                           ByVal Sa_Cod As Integer,
                           ByVal Cod_RisUm As Integer,
                           ByVal Cod_Contatto As String,
                           ByVal Cod_Rapporto As Integer,
                           ByVal Settore_Des As String,
                           ByVal Attivita_Des As String,
                           ByVal Corrispettivo_Mensile As Decimal,
                           ByVal Corrispettivo_Orario As Decimal,
                           ByVal Ore_Settimanali As Decimal,
                           ByVal Giorni_Ferie As Integer,
                           ByVal Ferie_Godute As Integer,
                           ByVal Giorni_Malattia As Integer,
                           ByVal Occasionale As Integer,
                           ByVal Patentino As String,
                           ByVal Data_Rilascio_Patentino As Date,
                           ByVal Data_Scadenza_Patentino As Date,
                           ByVal Ente_di_rilascio As String,
                           ByVal Cod_RisUm_Origine As Integer,
                           ByVal Piva_SuperUser_Origine As String,
                           ByVal Saldo_Iniziale_Crediti As Decimal,
                           ByVal Saldo_Iniziale_Debiti As Decimal,
                           ByVal ChkSpesometro As Integer,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_creazione As Date = #2/1/1900#,
                           Optional ByVal Data_modifica As Date = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = "",
                           Optional ByVal Qualifica_Cod As Integer = 0,
                           Optional ByVal Mansione_Cod As Integer = 0,
                           Optional ByVal Classificazione_Cod As Integer = 0,
                           Optional ByVal Info_Famiglia As String = "",
                           Optional ByVal Cod_Iva_Contatto As Integer = -1,
                           Optional ByVal Cod_Conto_Econ As Integer = 0,
                           Optional ByVal Cod_Conto_Pat As Integer = 0
                           ) As Boolean

        Dim nomeRoutine As String = "AnagrafeCoreAnagrafeDAL.Risorse_Umane_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

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

        If Not AgronicaCoreDataProvider.UtilityProvider.PivaValida(Piva) Then
            Throw New Exception("Rilevato carattere non valido nella PIVA:" & Piva)
        End If

        If Not AgronicaCoreDataProvider.UtilityProvider.PivaValida(Cod_Contatto) Then
            Throw New Exception("Rilevato carattere non valido nel codice contatto:" & Piva)
        End If

        Try

            '---------------------------------------------
            strSql.Length = 0
            strSql.Append("INSERT INTO Risorse_Umane( ")
            strSql.Append("                    Piva, ")
            strSql.Append("                    Sa_Cod, ")
            strSql.Append("                    Cod_RisUm, ")
            strSql.Append("                    Cod_Contatto, ")
            strSql.Append("                    Cod_Rapporto, ")
            strSql.Append("                    Settore_Des, ")
            strSql.Append("                    Attivita_Des, ")
            strSql.Append("                    Corrispettivo_Mensile, ")
            strSql.Append("                    Corrispettivo_Orario, ")
            strSql.Append("                    Giorni_Ferie, ")
            strSql.Append("                    Ferie_Godute, ")
            strSql.Append("                    Giorni_Malattia, ")
            strSql.Append("                    Occasionale, ")
            strSql.Append("                    Ore_Settimanali, ")
            strSql.Append("                    Patentino, ")
            strSql.Append("                    Data_Rilascio_Patentino, ")
            strSql.Append("                    Data_Scadenza_Patentino, ")
            strSql.Append("                    Ente_di_rilascio, ")

            strSql.Append("                    Cod_RisUm_Origine,  Piva_SuperUser_Origine,    ")
            strSql.Append("                    ChkSpesometro, ")
            strSql.Append("                    Saldo_Iniziale_Crediti, ")
            strSql.Append("                    Saldo_Iniziale_Debiti, ")

            strSql.Append("                    Inviato, DataInvio, ")
            strSql.Append("                    Data_Creazione,     Data_Modifica, ")
            strSql.Append("                    UserName_Creazione, UserName_Modifica, ")
            strSql.Append("                    Validita_Inizio,    Validita_Fine, ")
            strSql.Append("                    Qualifica_Cod,    Mansione_Cod,  ")
            strSql.Append("                    Classificazione_Cod,    Info_Famiglia,  ")
            strSql.Append("                    Cod_Iva_Contatto, Cod_Conto_Econ, Cod_Conto_Pat  ")
            strSql.Append("                    ) ")

            strSql.Append("VALUES (")
            strSql.Append("          '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Cod_RisUm) & "  ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(Trim(Cod_Contatto)) & "' ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Cod_Rapporto) & "  ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(Trim(Settore_Des)) & "' ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(Trim(Attivita_Des)) & "' ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Corrispettivo_Mensile) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Corrispettivo_Orario) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Giorni_Ferie) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Ferie_Godute) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Giorni_Malattia) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Occasionale) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Ore_Settimanali) & "  ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(Trim(Patentino)) & "' ")
            strSql.Append("         , " & Agro_SQL_SaveDate(Data_Rilascio_Patentino) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveDate(Data_Scadenza_Patentino) & "  ")
            strSql.Append("         , '" & Agro_SQL_SaveText(Ente_di_rilascio) & "'  ")

            strSql.Append("         , " & Agro_SQL_SaveNum(Cod_RisUm_Origine) & "  ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "' ")

            strSql.Append("         , " & Agro_SQL_SaveNum(ChkSpesometro) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Saldo_Iniziale_Crediti) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Saldo_Iniziale_Debiti) & "  ")

            strSql.Append("         , 0  ")
            strSql.Append("         , Null  ")
            strSql.Append("         , " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            strSql.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            strSql.Append("         , " & Agro_SQL_SaveNum(Qualifica_Cod) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Mansione_Cod) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Classificazione_Cod) & "  ")
            strSql.Append("         , '" & Agro_SQL_SaveText(Info_Famiglia) & "'  ")
            strSql.Append("         , '" & Agro_SQL_SaveText(Cod_Iva_Contatto) & "'  ")
            strSql.Append("         , '" & Agro_SQL_SaveText(Cod_Conto_Econ) & "'  ")
            strSql.Append("         , '" & Agro_SQL_SaveText(Cod_Conto_Pat) & "'  ")

            strSql.Append(")")

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


    '############################################################################
    'nuova funzione, fatta il 03/04/2013
    'piva, cod_contatto e cod_rapporto non vanno modificati!
    'in realtà se si cambia rapporto contabile, si cancella il vecchio e si inserisce il nuovo
    'previo controllo da codice che non ci siano operazioni (contabili e non) su quel cod_risum
    '---
    'Corrispettivo_Mensile e Corrispettivo_Orario sono vecchi campi, ora il prezzo è in prodotti_costi
    Public Function Modifica(ByVal Cod_RisUm As Integer,
                             ByVal Settore_Des As String,
                             ByVal Attivita_Des As String,
                             ByVal Ore_Settimanali As Decimal,
                             ByVal Giorni_Ferie As Integer,
                             ByVal Ferie_Godute As Integer,
                             ByVal Giorni_Malattia As Integer,
                             ByVal Occasionale As Integer,
                             ByVal Patentino As String,
                             ByVal Data_Rilascio_Patentino As Date,
                             ByVal Data_Scadenza_Patentino As Date,
                             ByVal Ente_di_rilascio As String,
                             ByVal Saldo_Iniziale_Crediti As Decimal,
                             ByVal Saldo_Iniziale_Debiti As Decimal,
                             ByVal ChkSpesometro As Integer,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri,
                             Optional ByVal Qualifica_Cod As Integer = 0,
                             Optional ByVal Mansione_Cod As Integer = 0,
                             Optional ByVal Classificazione_Cod As Integer = 0,
                             Optional ByVal Info_Famiglia As String = "",
                             Optional ByVal Cod_Rapporto As Integer = 0,
                             Optional ByVal Cod_Iva_Contatto As Integer = -1,
                             Optional ByVal Cod_Conto_Econ As Integer = 0,
                             Optional ByVal Cod_Conto_Pat As Integer = 0,
                             Optional ByVal Sa_Cod As Integer = -99
                             ) As Boolean

        Dim nomeRoutine As String = "AnagrafeCoreAnagrafeDAL.Risorse_Umane_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Cod_RisUm = 0 Then
                Throw New Exception("Parametro non corretto nella query (Cod_RisUm obbligatorio)")
            End If


            '---------------------------------------------
            strSql.Length = 0
            strSql.Append("UPDATE Risorse_Umane SET ")
            strSql.Append("   Settore_Des           = '" & Agro_SQL_SaveText(Settore_Des) & "'")
            strSql.Append("   ,Attivita_Des          = '" & Agro_SQL_SaveText(Attivita_Des) & "'")
            strSql.Append("   ,Ore_Settimanali       = " & Agro_SQL_SaveNum(Ore_Settimanali) & "  ")
            strSql.Append("   ,Giorni_Ferie          = " & Agro_SQL_SaveNum(Giorni_Ferie) & "  ")
            strSql.Append("   ,Ferie_Godute          = " & Agro_SQL_SaveNum(Ferie_Godute) & "  ")
            strSql.Append("   ,Giorni_Malattia       = " & Agro_SQL_SaveNum(Giorni_Malattia) & "  ")
            strSql.Append("   ,Occasionale           = " & Agro_SQL_SaveNum(Occasionale) & "  ")
            strSql.Append("   ,Patentino             = '" & Agro_SQL_SaveText(Patentino) & "'")
            strSql.Append("   ,Data_Rilascio_Patentino =  " & Agro_SQL_SaveDate(Data_Rilascio_Patentino))
            strSql.Append("   ,Data_Scadenza_Patentino =  " & Agro_SQL_SaveDate(Data_Scadenza_Patentino))
            strSql.Append("   ,Ente_di_rilascio =  '" & Agro_SQL_SaveText(Ente_di_rilascio) & "'")

            strSql.Append("   ,Saldo_Iniziale_Crediti   = " & Agro_SQL_SaveNum(Saldo_Iniziale_Crediti) & "  ")
            strSql.Append("   ,Saldo_Iniziale_Debiti    = " & Agro_SQL_SaveNum(Saldo_Iniziale_Debiti) & "  ")
            strSql.Append("   ,ChkSpesometro            = " & Agro_SQL_SaveNum(ChkSpesometro) & "  ")

            strSql.Append("   ,Inviato           =  0 ")
            strSql.Append("   ,DataInvio         =  Null ")
            strSql.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))
            strSql.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            strSql.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            strSql.Append("   ,Qualifica_Cod     =  " & Agro_SQL_SaveNum(Qualifica_Cod))
            strSql.Append("   ,Mansione_Cod     =  " & Agro_SQL_SaveNum(Mansione_Cod))
            strSql.Append("   ,Classificazione_Cod     =  " & Agro_SQL_SaveNum(Classificazione_Cod))
            strSql.Append("   ,Info_Famiglia     =  '" & Agro_SQL_SaveText(Info_Famiglia) & "'")

            If Sa_Cod <> -99 Then
                strSql.Append("   , Sa_Cod  =  " & Agro_SQL_SaveNum(Sa_Cod))
            End If

            If Cod_Rapporto <> 0 Then
                strSql.Append("   ,Cod_Rapporto  =  " & Agro_SQL_SaveNum(Cod_Rapporto))
            End If

            If Cod_Iva_Contatto <> -1 Then
                strSql.Append("   ,Cod_Iva_Contatto  =  " & Agro_SQL_SaveNum(Cod_Iva_Contatto))
            End If

            If Cod_Conto_Econ <> 0 Then
                strSql.Append("   ,Cod_Conto_Econ  =  " & Agro_SQL_SaveNum(Cod_Conto_Econ))
            End If

            If Cod_Conto_Pat <> 0 Then
                strSql.Append("   ,Cod_Conto_Pat  =  " & Agro_SQL_SaveNum(Cod_Conto_Pat))
            End If

            strSql.Append(" WHERE   Cod_RisUm = " & Cod_RisUm & "  ")

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


    '############################################################################
    'Attenzione!!!! questa è la vecchia funzione, modifica il cod_rapporto (non proprio corretto)
    'in realtà se si cambia rapporto contabile, si cancella il vecchio e si inserisce il nuovo
    'previo controllo da codice che non ci siano operazioni (contabili e non) su quel cod_risum
    Public Function Modifica2(ByVal Cod_RisUm As Integer,
                              ByVal Cod_Contatto As String,
                              ByVal Cod_Rapporto As Integer,
                              ByVal Settore_Des As String,
                              ByVal Attivita_Des As String,
                              ByVal Corrispettivo_Mensile As Decimal,
                              ByVal Corrispettivo_Orario As Decimal,
                              ByVal Ore_Settimanali As Decimal,
                              ByVal Giorni_Ferie As Integer,
                              ByVal Ferie_Godute As Integer,
                              ByVal Giorni_Malattia As Integer,
                              ByVal Occasionale As Integer,
                              ByVal Patentino As String,
                              ByVal Data_Rilascio_Patentino As Date,
                              ByVal Data_Scadenza_Patentino As Date,
                              ByVal Ente_di_rilascio As String,
                              ByVal Validita_Inizio As Date,
                              ByVal Validita_Fine As Date,
                              ByVal xFiltroAggiuntivo As String,
                              ByRef objParametri As AgronicaCoreParametri
                              ) As Boolean

        Dim nomeRoutine As String = "AnagrafeCoreAnagrafeDAL.Risorse_Umane_W.Modifica2()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            'MODIFICATA IN DATA 03/04/2013: questa modifica veniva fatta per cod_risum ->
            'sistemati i controlli

            'If Cod_Contatto = "" Then
            '    Throw New Exception("Parametro non corretto nella query (Cod_Contatto obbligatorio)")
            'End If

            If Cod_RisUm = 0 Then
                Throw New Exception("Parametro non corretto nella query (Cod_RisUm obbligatorio)")
            End If


            '---------------------------------------------
            strSql.Length = 0
            strSql.Append("UPDATE Risorse_Umane SET ")
            strSql.Append("    Cod_Rapporto          = " & Agro_SQL_SaveNum(Cod_Rapporto) & "  ")
            strSql.Append("   ,Settore_Des           = '" & Agro_SQL_SaveText(Settore_Des) & "'")
            strSql.Append("   ,Attivita_Des          = '" & Agro_SQL_SaveText(Attivita_Des) & "'")
            strSql.Append("   ,Corrispettivo_Mensile = " & Agro_SQL_SaveNum(Corrispettivo_Mensile) & "  ")
            strSql.Append("   ,Corrispettivo_Orario  = " & Agro_SQL_SaveNum(Corrispettivo_Orario) & "  ")
            strSql.Append("   ,Ore_Settimanali       = " & Agro_SQL_SaveNum(Ore_Settimanali) & "  ")
            strSql.Append("   ,Giorni_Ferie          = " & Agro_SQL_SaveNum(Giorni_Ferie) & "  ")
            strSql.Append("   ,Ferie_Godute          = " & Agro_SQL_SaveNum(Ferie_Godute) & "  ")
            strSql.Append("   ,Giorni_Malattia       = " & Agro_SQL_SaveNum(Giorni_Malattia) & "  ")
            strSql.Append("   ,Occasionale           = " & Agro_SQL_SaveNum(Occasionale) & "  ")
            strSql.Append("   ,Patentino             = '" & Agro_SQL_SaveText(Patentino) & "'")
            strSql.Append("   ,Data_Rilascio_Patentino =  " & Agro_SQL_SaveDate(Data_Rilascio_Patentino))
            strSql.Append("   ,Data_Scadenza_Patentino =  " & Agro_SQL_SaveDate(Data_Scadenza_Patentino))
            strSql.Append("   ,Ente_di_rilascio             = '" & Agro_SQL_SaveText(Ente_di_rilascio) & "'")

            strSql.Append("   ,Inviato           =  0 ")
            strSql.Append("   ,DataInvio         =  Null ")
            strSql.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))
            strSql.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            strSql.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            strSql.Append(" WHERE   Cod_RisUm = " & Cod_RisUm & "  ")

            'If Cod_Contatto <> "" Then
            '    StrSQL.Append(" AND Risorse_Umane.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "' ")
            'End If

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

    Public Function UpdateValiditaFine_byCodRisUm(ByVal Cod_Risum As Integer,
                                                 ByVal Validita_Fine As Date,
                                                 ByRef objParametri As AgronicaCoreParametri
                                                 ) As Boolean

        Dim nomeRoutine As String = "AnagrafeCoreAnagrafeDAL.Risorse_Umane_W.UpdateValiditaFine()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Cod_Risum = 0 Then
                Throw New Exception("Parametro non corretto nella query (Cod_RisUm obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0
            strSql.Append(" UPDATE   Risorse_Umane  ")
            strSql.Append(" SET     Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            strSql.Append("         , Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            strSql.Append("         , UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            strSql.Append(" WHERE   Cod_RisUm = " & Cod_Risum & "  ")

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


    '############################################################################
    Public Function Modifica_Parametrizzata(ByVal Piva As String,
                                            ByVal Cod_Contatto As String,
                                            ByVal Campo As String,
                                            ByVal Valore As Object,
                                            ByVal Validita_Inizio As Date,
                                            ByVal Validita_Fine As Date,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByRef objParametri As AgronicaCoreParametri
                                            ) As Boolean

        Dim nomeRoutine As String = "AnagrafeCoreAnagrafeDAL.Risorse_Umane_W.Modifica_Parametrizzata()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False
        Dim strAssegnamento As String = String.Empty

        ' a seconda del tipo del valore che devo aggiornare, formatto la query
        Dim Stringa As Type = GetType(System.String)
        Dim Data As Type = GetType(System.DateTime)
        Dim Intero32 As Type = GetType(System.Int32)

        Try
            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatoria)")
            End If

            If Cod_Contatto = "" Then
                Throw New Exception("Parametro non corretto nella query (Cod_Contatto obbligatorio)")
            End If

            '---------------------------------------------

            Dim TypeVal As Type = Valore.GetType()

            If TypeVal.Equals(Stringa) Then
                strAssegnamento = Campo & "= '" & Agro_SQL_SaveText(Valore.ToString) & "' "
            ElseIf TypeVal.Equals(Data) Then
                strAssegnamento = Campo & "= " & Agro_SQL_SaveDate(Valore.ToString) & " "
            Else
                strAssegnamento = Campo & "= " & Agro_SQL_SaveNum(Valore.ToString) & " "
            End If


            '---------------------------------------------
            strSql.Length = 0
            strSql.Append(" UPDATE Risorse_Umane SET ")
            strSql.Append(strAssegnamento)
            strSql.Append("   ,Inviato           =  0 ")
            strSql.Append("   ,DataInvio         =  Null ")
            strSql.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))
            strSql.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            strSql.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            strSql.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            strSql.Append(" AND   Cod_contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "' ")

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

    '############################################################################
    Public Function Cancella(ByVal Cod_RisUm As Integer,
                             ByVal Cod_Contatto As String,
                             ByVal Cod_Rapporto As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Indirizzo_Write.Cancella()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.Length = 0
                strSql.Append(" UPDATE Risorse_Umane ")
                strSql.Append(" SET ")
                strSql.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.Append("      ,Inviato = -1 ")
                strSql.Append(" WHERE Inviato >= 0")

            Else

                strSql.Length = 0
                strSql.Append(" DELETE ")
                strSql.Append(" FROM     Risorse_Umane ")
                strSql.Append(" WHERE Inviato = 0")

            End If

            If Cod_RisUm <> 0 Then
                strSql.Append(" AND   Cod_RisUm = " & Cod_RisUm & " ")
            End If

            If Cod_Contatto <> "" Then
                strSql.Append(" AND   Cod_Contatto = '" & Agro_SQL_SaveText(Trim(Cod_Contatto)) & "' ")
            End If

            If Cod_Rapporto <> 0 Then
                strSql.Append(" AND   Cod_Rapporto = " & Cod_Rapporto & " ")
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

    Public Function Aggiorna_RisorseUmane(eFArrayToInsert As ArrayList, eFArrayToUpdate As ArrayList, eFArrayToDelete As ArrayList, objParametri_Server As AgronicaCoreParametri) As String

        Dim Piva_SuperUser = objParametri_Server.PivaSuperUser

        Dim objSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

        Dim retries As Integer = 3
        Dim success As Boolean = True

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Risorse_Umane_W.Aggiorna_RisorseUmane()"
        Dim messaggioErrore As String = ""

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)

        Try

            Using scope As New TransactionScope()

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    Dim idSeq As Integer = 0

                    For Each curRisUm As Risorse_Umane In eFArrayToInsert

                        success = False
                        For i As Integer = 0 To retries - 1

                            Try

                                'Richiedo un nuovo id sequenza
                                idSeq = objSequenze.NuovoId_Tabella_EF(GiasContext, "Risorse_Umane", 0, 2000000000, objParametri_Server)
                                curRisUm.Cod_RisUm = idSeq

                                GiasContext.Risorse_Umane.Add(curRisUm)
                                GiasContext.SaveChanges()
                                success = True

                                Exit For
                            Catch ex As Exception
                                Threading.Thread.Sleep(500) ' 500 milliseconds = 0.5 seconds
                            End Try
                        Next
                        ' Al primo errore evito di continuare le modifiche
                        If Not success Then
                            messaggioErrore = "Non sono riuscito ad aggiornare i dati dopo " & retries & " tentativi."
                            Exit For
                        End If
                    Next

                    If success Then
                        For Each listRisorse_Umane As Risorse_Umane In eFArrayToUpdate
                            GiasContext.Risorse_Umane.Attach(listRisorse_Umane)
                            GiasContext.Entry(listRisorse_Umane).State = EntityState.Modified
                            GiasContext.SaveChanges()
                        Next

                        For Each listRisorse_Umane As Risorse_Umane In eFArrayToDelete
                            GiasContext.Risorse_Umane.Attach(listRisorse_Umane)
                            GiasContext.Risorse_Umane.Remove(listRisorse_Umane)
                            GiasContext.SaveChanges()
                        Next

                        ' COMMIT Effettivo
                        scope.Complete()
                    End If

                End Using
            End Using

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore, ex)

        End Try

        Return messaggioErrore

    End Function

End Class
