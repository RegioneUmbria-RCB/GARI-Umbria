Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.My.Resources
Imports System.Data.Entity.Infrastructure

Public Class Indirizzi_Read
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Cod_Indirizzo As Integer,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Indirizzi_Read.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            If Cod_Indirizzo = 0 Then
                Throw New Exception("Parametro non corretto nella query (Cod_Indirizzo obbligatorio)")
            End If

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    strSql.Length = 0

                    strSql.AppendLine(" SELECT  Indirizzi.cod_indirizzo, Indirizzi.ind_des, Indirizzi.frz_des, Indirizzi.CAP, Indirizzi.stato, Indirizzi.note, Indirizzi.pro_cod_istat, Indirizzi.com_cod_istat   ")
                    strSql.AppendLine("         , Indirizzi.inviato, Indirizzi.datainvio, Indirizzi.Data_Creazione, Indirizzi.Data_Modifica, Indirizzi.Username_Creazione, Indirizzi.Username_Modifica, Indirizzi.Validita_Inizio, Indirizzi.Validita_Fine ")
                    strSql.AppendLine("         , Indirizzi.Validazione, Indirizzi.Data_Validazione, Indirizzi.UserName_Validazione, Indirizzi.Codice_Lingua, Indirizzi.Codice_Alternativo, ")
                    strSql.AppendLine("         ISNULL(ISTAT.LOCALITA, '') AS com_des,  ")
                    strSql.AppendLine("         ISNULL(ISTAT.COMUNI_PROV, '') AS pro_cod,  ")
                    strSql.AppendLine("         ISNULL(Lista_Province.PROVINCIA, '') AS pro_des  ")
                    strSql.AppendLine("         ,ISNULL(Lista_Province.REG, '') AS reg")
                    strSql.AppendLine(" FROM    Indirizzi ")
                    strSql.AppendLine("         LEFT OUTER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM ")
                    strSql.AppendLine("         LEFT OUTER JOIN  Lista_Province ON Indirizzi.pro_cod_istat = Lista_Province.PROV ")
                    strSql.AppendLine(" WHERE   Indirizzi.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND     Indirizzi.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    strSql.AppendLine(" AND     Indirizzi.Cod_Indirizzo = " & Cod_Indirizzo & " ")
                    strSql.AppendLine(" AND     Indirizzi.Inviato >= 0 ")

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Indirizzi.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Indirizzi.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Indirizzi.Validita_inizio ASC")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case enumSelezioneVariabile.Selezione_JoinCompleta


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

    Public Function LeggixGisCentro(ByVal Piva As String,
                                    ByVal Sa_Cod As String,
                                    ByRef objParametri As AgronicaCoreParametri
                                    ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Indirizzi_Read.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" select * from ( ")

            strSql.AppendLine(" SELECT  Indirizzi.stato , Indirizzi.ind_des, Indirizzi.frz_des, Indirizzi.CAP, Indirizzi.com_des, ISTAT.LOCALITA,  Lista_Province.PROVINCIA , 0 as  ordine  ")
            strSql.AppendLine(" FROM         CentrixIndirizzi  ")
            strSql.AppendLine("     INNER JOIN Indirizzi ON CentrixIndirizzi.cod_indirizzo = Indirizzi.cod_indirizzo  ")
            strSql.AppendLine("     INNER JOIN ISTAT ON   Indirizzi.com_cod_istat = ISTAT.COM  ")
            strSql.AppendLine("and Indirizzi.pro_cod_istat = ISTAT.PROV  ")

            strSql.AppendLine("     INNER JOIN Lista_Province ON ISTAT.PROV = Lista_Province.PROV ")
            strSql.AppendLine(" WHERE   (stato <>'' or ind_des<>'' or frz_des<>'' or Indirizzi.CAp <>'' or com_des <>'' ) ")

            If Piva <> "" Then
                strSql.AppendLine(" AND     CentrixIndirizzi.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If
            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND     CentrixIndirizzi.sa_cod = " & Agro_vb_SaveNum(Sa_Cod) & " ")
            End If


            strSql.AppendLine(" UNION ")

            strSql.AppendLine(" SELECT  Indirizzi.stato , Indirizzi.ind_des, Indirizzi.frz_des, Indirizzi.CAP, Indirizzi.com_des, ISTAT.LOCALITA,  Lista_Province.PROVINCIA , 1 as  ordine ")
            strSql.AppendLine(" FROM         impresexIndirizzi  ")
            strSql.AppendLine("     INNER JOIN Indirizzi ON impresexIndirizzi.cod_indirizzo = Indirizzi.cod_indirizzo  ")
            strSql.AppendLine("     INNER JOIN ISTAT ON   Indirizzi.com_cod_istat = ISTAT.COM  ")
            strSql.AppendLine("and Indirizzi.pro_cod_istat = ISTAT.PROV  ")

            strSql.AppendLine("     INNER JOIN Lista_Province ON ISTAT.PROV = Lista_Province.PROV ")
            strSql.AppendLine(" WHERE   (stato <>'' or ind_des<>'' or frz_des<>'' or Indirizzi.CAp <>'' or com_des <>'' ) ")

            If Piva <> "" Then
                strSql.AppendLine(" AND     impresexIndirizzi.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If
            'If Sa_Cod <> 0 Then
            '    strSql.AppendLine(" AND     impresexIndirizzi.sa_cod = " & Agro_vb_SaveNum(Sa_Cod) & " ")
            'End If

            strSql.AppendLine(" ) a order by ordine ")

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

    '###############################################################
    Public Function Indirizzo_from_CodIndirizzo(ByVal Cod_Indirizzo As Integer,
                                                ByRef objParametri As AgronicaCoreParametri
                                                ) As String

        Dim dt As DataTable
        Dim indirizzo As String = ""

        dt = Leggi(Cod_Indirizzo,
                   enumSelezioneVariabile.Selezione_TabellaCompleta,
                   "", "", objParametri)


        If Not IsNothing(dt) AndAlso dt.Rows.Count <> 0 Then
            indirizzo = CStr(dt.Rows(0).Item("ind_des")) & " " &
                        CStr(dt.Rows(0).Item("frz_des")) & " " &
                        CStr(dt.Rows(0).Item("com_des")) & " " &
                        CStr(dt.Rows(0).Item("pro_cod"))
        End If

        Return indirizzo

    End Function

    Public Function LeggiCodiceAslDatoCodIndirizzo(ByVal codice As Integer,
                                                   ByRef objParametri As AgronicaCoreParametri
                                                   ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Indirizzi_Read.LeggiCodiceAslDatoCodIndirizzo()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try
            strSql.Length = 0

            strSql.AppendLine("Select Lista_AUSL.codice, * From indirizzi")
            strSql.AppendLine("INNER Join IstatxDistretti")
            strSql.AppendLine("On ( (IstatxDistretti.pro_cod = indirizzi.pro_cod_istat) And (IstatxDistretti.com_cod = indirizzi.com_cod_istat) )")
            strSql.AppendLine("INNER Join Lista_Distretti")
            strSql.AppendLine("On (Lista_Distretti.distretto_id = IstatxDistretti.distretto_id)")
            strSql.AppendLine("INNER Join Lista_AUSL")
            strSql.AppendLine("On (Lista_AUSL.asl_id = Lista_distretti.asl_id)")
            strSql.AppendLine(String.Format("WHERE Indirizzi.cod_indirizzo = {0}", codice))
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Indirizzi.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Indirizzi.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

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

    '#############################################################################
    Public Function IndirizziImpresa(ByVal Piva As String,
                                           ByVal Tipo_Indirizzo As Integer,
                                           ByVal Cod_Indirizzo As Integer,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByVal xOrderBy As String,
                                           ByRef objParametri As AgronicaCoreParametri
                                           ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Indirizzi_Read.IndirizziImpresa()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try
            strSql.Length = 0

            strSql.AppendLine(" SELECT Indirizzi.*, ")
            strSql.AppendLine(" ISNULL(ISTAT.LOCALITA, '') AS Com_Des_Istat, ISNULL(ISTAT.COMUNI_PROV, '') AS Pro_Sigla_Istat, ISNULL(Lista_Province.PROVINCIA, '') AS Pro_Des_Istat ")
            strSql.AppendLine(" FROM    ImpresexIndirizzi  ")
            strSql.AppendLine(" INNER JOIN Indirizzi ON ImpresexIndirizzi.Cod_Indirizzo = Indirizzi.cod_indirizzo ")
            strSql.AppendLine(" INNER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM ")
            strSql.AppendLine(" LEFT OUTER JOIN Lista_Province ON Indirizzi.pro_cod_istat = Lista_Province.PROV ")

            strSql.AppendLine(" WHERE ImpresexIndirizzi.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")

            If Tipo_Indirizzo <> 0 Then
                strSql.AppendLine(" AND     ImpresexIndirizzi.Tipo_Indirizzo = " & Agro_SQL_SaveNum(Tipo_Indirizzo) & "   ")
            End If
            If Cod_Indirizzo <> 0 Then
                strSql.AppendLine(" AND     ImpresexIndirizzi.Cod_Indirizzo = " & Agro_SQL_SaveNum(Cod_Indirizzo) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Indirizzi.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Indirizzi.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY cod_indirizzo ")
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
    '#############################################################################
    Public Function IndirizziImpresaCentri(ByVal Piva As String,
                                           ByVal Sa_Cod As Integer,
                                           ByVal Tipo_Indirizzo As Integer,
                                           ByVal Cod_Indirizzo As Integer,
                                           ByVal xFiltroAggiuntivo1 As String,
                                           ByVal xFiltroAggiuntivo2 As String,
                                           ByVal xOrderBy As String,
                                           ByRef objParametri As AgronicaCoreParametri
                                           ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Indirizzi_Read.IndirizziImpresaCentri()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try
            strSql.Length = 0

            strSql.AppendLine(" SELECT * ")
            strSql.AppendLine(" FROM ")

            strSql.AppendLine(" ( ")

            '/*************************************************************************************
            '/**************** IMPRESA        *************************
            '/*************************************************************************************

            strSql.AppendLine(" ( ")

            strSql.AppendLine(" SELECT ImpresexIndirizzi.Tipo_Indirizzo, Indirizzi.Cod_Indirizzo, Indirizzi.ind_des, Indirizzi.frz_des, Indirizzi.CAP, Indirizzi.com_cod_istat, Indirizzi.pro_cod_istat, Indirizzi.stato, Indirizzi.note, ")
            strSql.AppendLine(" ISNULL(ISTAT.LOCALITA, '') AS com_des, ISNULL(ISTAT.COMUNI_PROV, '') AS pro_cod  ")
            strSql.AppendLine(" FROM    ImpresexIndirizzi  ")
            strSql.AppendLine(" INNER JOIN Indirizzi ON ImpresexIndirizzi.Cod_Indirizzo = Indirizzi.cod_indirizzo ")
            strSql.AppendLine(" INNER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM ")

            strSql.AppendLine(" WHERE ImpresexIndirizzi.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")

            If Tipo_Indirizzo <> 0 Then
                strSql.AppendLine(" AND     ImpresexIndirizzi.Tipo_Indirizzo = " & Agro_SQL_SaveNum(Tipo_Indirizzo) & "   ")
            End If
            If Cod_Indirizzo <> 0 Then
                strSql.AppendLine(" AND     ImpresexIndirizzi.Cod_Indirizzo = " & Agro_SQL_SaveNum(Cod_Indirizzo) & "   ")
            End If

            If xFiltroAggiuntivo1 <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo1, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Indirizzi.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Indirizzi.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            strSql.AppendLine(" ) ")


            strSql.AppendLine(" UNION ALL ")

            '/*************************************************************************************
            '/******************    CENTRI        *************************
            '/*************************************************************************************

            strSql.AppendLine(" ( ")


            strSql.AppendLine(" SELECT CentrixIndirizzi.Tipo_Indirizzo, Indirizzi.Cod_Indirizzo, Indirizzi.ind_des, Indirizzi.frz_des, Indirizzi.CAP, Indirizzi.com_cod_istat, Indirizzi.pro_cod_istat, Indirizzi.stato, Indirizzi.note, ")
            strSql.AppendLine(" ISNULL(ISTAT.LOCALITA, '') AS com_des, ISNULL(ISTAT.COMUNI_PROV, '') AS pro_cod  ")
            strSql.AppendLine(" FROM    CentrixIndirizzi  ")
            strSql.AppendLine(" INNER JOIN Indirizzi ON CentrixIndirizzi.Cod_Indirizzo = Indirizzi.cod_indirizzo ")
            strSql.AppendLine(" INNER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM ")

            strSql.AppendLine(" WHERE CentrixIndirizzi.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND     CentrixIndirizzi.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If
            If Tipo_Indirizzo <> 0 Then
                strSql.AppendLine(" AND     CentrixIndirizzi.Tipo_Indirizzo = " & Agro_SQL_SaveNum(Tipo_Indirizzo) & "   ")
            End If
            If Cod_Indirizzo <> 0 Then
                strSql.AppendLine(" AND     CentrixIndirizzi.Cod_Indirizzo = " & Agro_SQL_SaveNum(Cod_Indirizzo) & "   ")
            End If

            If xFiltroAggiuntivo2 <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo2, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Indirizzi.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Indirizzi.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------


            strSql.AppendLine(" )")

            '/*************************************************************************************

            strSql.AppendLine(" ) indirizzi ")


            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY cod_indirizzo ")
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

    Public Function LeggiIndContattiImpreseCentri(ByVal piva As String,
                                                  ByVal codContatto As String,
                                                  ByVal codIndirizzo As Integer,
                                                  ByVal tipoIndirizzo As Integer,
                                                  ByVal xFiltroAggiuntivo As String,
                                                  ByVal xOrderBy As String,
                                                  ByRef objParametri As AgronicaCoreParametri,
                                                  Optional ByVal flagLeggiContatti As Boolean = True,
                                                  Optional ByVal flagLeggiImprese As Boolean = True,
                                                  Optional ByVal flagLeggiCentri As Boolean = True,
                                                  Optional ByVal finestraTempInizio As Date = AGRODATAINIZIO,
                                                  Optional ByVal finestraTempFine As Date = AGRODATAFINE
                                                  ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Indirizzi_Read.LeggiIndContattiImpreseCentri()"

        '====================================================================================
        'Parametri opzionali :
        '   Cod_Contatto = ""           =>  si leggono tutti i contatti
        '   Cod_Indirizzo = 0           =>  si leggono tutti gli indirizzi
        '   Tipo_Indirizzo = 0          =>  si leggono tutti i tipi di indirizzo
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable
        Dim flagNeedUnion As Boolean = False

        Try

            If piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatoria)")
            End If

            If Not flagLeggiContatti AndAlso Not flagLeggiImprese AndAlso Not flagLeggiCentri Then
                Throw New Exception("Deve essere True almeno uno tra: flagLeggiContatti, flagLeggiImprese, flagLeggiCentri")
            End If

            strSql.Length = 0
            strSql.AppendLine(" SELECT IND.*, ")
            strSql.AppendLine(" ISNULL(ISTAT.LOCALITA, '') AS Com_Des_Istat, ISNULL(ISTAT.COMUNI_PROV, '') AS Pro_Sigla_Istat, ISNULL(Lista_Province.PROVINCIA, '') AS Pro_Des_Istat ")
            strSql.AppendLine(" FROM ")
            strSql.AppendLine(" ( ")

            If flagLeggiContatti Then
                strSql.AppendLine("     ( ")
                strSql.AppendLine("     SELECT c.Piva, c.Sa_Cod, c.Cod_Contatto, c.Tipo_Indirizzo, Indirizzi.*, ")
                strSql.AppendLine("     IndirizzoTipo.Descrizione AS IndirizzoTipo_Des ")
                strSql.AppendLine("     FROM ContattixIndirizzi c ")
                strSql.AppendLine("     INNER JOIN Indirizzi ON c.Cod_Indirizzo = Indirizzi.Cod_Indirizzo ")
                strSql.AppendLine("     INNER JOIN (SELECT IndirizzoTipo_Cod, Descrizione FROM IndirizzoTipo ")
                strSql.AppendLine("           UNION SELECT 2 AS IndirizzoTipo_Cod, '" & Agro_SQL_SaveText(Gias.Domicilio) & "' AS Descrizione ")
                strSql.AppendLine("           UNION SELECT 3 AS IndirizzoTipo_Cod, '" & Agro_SQL_SaveText(Gias.Residenza) & "' AS Descrizione ")
                strSql.AppendLine("           UNION SELECT 4 AS IndirizzoTipo_Cod, '" & Agro_SQL_SaveText(Gias.ResidenzaEstiva) & "' AS Descrizione ")
                strSql.AppendLine("           UNION SELECT 5 AS IndirizzoTipo_Cod, '" & Agro_SQL_SaveText(Gias.LuogoDiNascita) & "' AS Descrizione ")
                strSql.AppendLine("           UNION SELECT 1 AS IndirizzoTipo_Cod, '" & Agro_SQL_SaveText(Gias.SedeOperativa) & "' AS Descrizione ")
                strSql.AppendLine("           UNION SELECT 101 AS IndirizzoTipo_Cod, '" & Agro_SQL_SaveText(Gias.SedeLegale) & "' AS Descrizione ")
                strSql.AppendLine("           UNION SELECT 102 AS IndirizzoTipo_Cod, '" & Agro_SQL_SaveText(Gias.SedeAziendale) & "' AS Descrizione ")
                strSql.AppendLine("           UNION SELECT 103 AS IndirizzoTipo_Cod, '" & Agro_SQL_SaveText(Gias.Stabilimento) & "' AS Descrizione) ")
                strSql.AppendLine("       AS IndirizzoTipo ")
                strSql.AppendLine("            ON c.Tipo_Indirizzo = IndirizzoTipo.IndirizzoTipo_Cod ")
                strSql.AppendLine("     WHERE c.Validita_inizio < " & Agro_SQL_SaveDate(finestraTempFine) & " ")
                strSql.AppendLine("     AND   c.Validita_Fine > " & Agro_SQL_SaveDate(finestraTempInizio) & " ")
                strSql.AppendLine("     AND   Indirizzi.Validita_inizio < " & Agro_SQL_SaveDate(finestraTempFine) & " ")
                strSql.AppendLine("     AND   Indirizzi.Validita_Fine > " & Agro_SQL_SaveDate(finestraTempInizio) & " ")
                strSql.AppendLine("     AND   ( c.Piva = '" & Trim(Agro_SQL_SaveText(piva)) & "'  OR  c.Sa_Cod = -1 )  ")
                strSql.AppendLine("     ) ")
                strSql.AppendLine(" ")
                flagNeedUnion = True
            End If


            If flagLeggiImprese Then
                If flagNeedUnion Then
                    strSql.AppendLine(" UNION ALL ")
                End If

                strSql.AppendLine(" ")
                strSql.AppendLine("     ( ")
                strSql.AppendLine("     SELECT c.Piva, 0 AS Sa_Cod, c.Piva AS Cod_Contatto, c.Tipo_Indirizzo, Indirizzi.*, ")
                strSql.AppendLine("     IndirizzoTipo.Descrizione AS IndirizzoTipo_Des ")
                strSql.AppendLine("     FROM ImpresexIndirizzi c ")
                strSql.AppendLine("     INNER JOIN Indirizzi ON c.Cod_Indirizzo = Indirizzi.Cod_Indirizzo ")
                strSql.AppendLine("     INNER JOIN (SELECT IndirizzoTipo_Cod, Descrizione FROM IndirizzoTipo ")
                strSql.AppendLine("           UNION SELECT 1 AS IndirizzoTipo_Cod, '" & Agro_SQL_SaveText(Gias.SedeLegaleImpresa) & "' AS Descrizione ")
                strSql.AppendLine("           UNION SELECT 101 AS IndirizzoTipo_Cod, '" & Agro_SQL_SaveText(Gias.SedeLegale) & "' AS Descrizione ")
                strSql.AppendLine("           UNION SELECT 102 AS IndirizzoTipo_Cod, '" & Agro_SQL_SaveText(Gias.SedeAziendale) & "' AS Descrizione ")
                strSql.AppendLine("           UNION SELECT 103 AS IndirizzoTipo_Cod, '" & Agro_SQL_SaveText(Gias.Stabilimento) & "' AS Descrizione) ")
                strSql.AppendLine("       AS IndirizzoTipo ")
                strSql.AppendLine("            ON c.Tipo_Indirizzo = IndirizzoTipo.IndirizzoTipo_Cod ")
                strSql.AppendLine("     WHERE c.Validita_inizio < " & Agro_SQL_SaveDate(finestraTempFine) & " ")
                strSql.AppendLine("     AND   c.Validita_Fine > " & Agro_SQL_SaveDate(finestraTempInizio) & " ")
                strSql.AppendLine("     AND   Indirizzi.Validita_inizio < " & Agro_SQL_SaveDate(finestraTempFine) & " ")
                strSql.AppendLine("     AND   Indirizzi.Validita_Fine > " & Agro_SQL_SaveDate(finestraTempInizio) & " ")
                strSql.AppendLine("     ) ")
                strSql.AppendLine(" ")
                flagNeedUnion = True
            End If

            If flagLeggiCentri Then
                If flagNeedUnion Then
                    strSql.AppendLine(" UNION ALL ")
                End If

                strSql.AppendLine(" ")
                strSql.AppendLine("     ( ")
                strSql.AppendLine("     SELECT c.Piva, c.Sa_Cod, c.Piva AS Cod_Contatto, c.Tipo_Indirizzo, Indirizzi.*, ")
                strSql.AppendLine("     IndirizzoTipo.Descrizione + ' (' + ca.Sa_Nome + ')' AS IndirizzoTipo_Des ")
                strSql.AppendLine("     FROM CentrixIndirizzi c ")
                strSql.AppendLine("     INNER JOIN Centri_Aziendali ca ON c.Piva = ca.Piva AND ca.Sa_Cod = c.Sa_Cod ")
                strSql.AppendLine("     INNER JOIN Indirizzi ON c.Cod_Indirizzo = Indirizzi.Cod_Indirizzo ")
                strSql.AppendLine("     INNER JOIN (SELECT IndirizzoTipo_Cod, Descrizione FROM IndirizzoTipo ")
                strSql.AppendLine("           UNION SELECT 1 AS IndirizzoTipo_Cod, '" & Agro_SQL_SaveText(Gias.SedeOperativa) & "' AS Descrizione ")
                strSql.AppendLine("           UNION SELECT 101 AS IndirizzoTipo_Cod, '" & Agro_SQL_SaveText(Gias.SedeLegale) & "' AS Descrizione ")
                strSql.AppendLine("           UNION SELECT 102 AS IndirizzoTipo_Cod, '" & Agro_SQL_SaveText(Gias.SedeAziendale) & "' AS Descrizione ")
                strSql.AppendLine("           UNION SELECT 103 AS IndirizzoTipo_Cod, '" & Agro_SQL_SaveText(Gias.Stabilimento) & "' AS Descrizione) ")
                strSql.AppendLine("       AS IndirizzoTipo ")
                strSql.AppendLine("            ON c.Tipo_Indirizzo = IndirizzoTipo.IndirizzoTipo_Cod ")
                strSql.AppendLine("     WHERE c.Validita_inizio < " & Agro_SQL_SaveDate(finestraTempFine) & " ")
                strSql.AppendLine("     AND   c.Validita_Fine > " & Agro_SQL_SaveDate(finestraTempInizio) & " ")
                strSql.AppendLine("     AND   Indirizzi.Validita_inizio < " & Agro_SQL_SaveDate(finestraTempFine) & " ")
                strSql.AppendLine("     AND   Indirizzi.Validita_Fine > " & Agro_SQL_SaveDate(finestraTempInizio) & " ")
                strSql.AppendLine("     ) ")
            End If

            strSql.AppendLine(" ")
            strSql.AppendLine(" ) AS IND ")
            strSql.AppendLine(" ")

            strSql.AppendLine(" LEFT OUTER JOIN ISTAT ON IND.pro_cod_istat = ISTAT.PROV AND IND.com_cod_istat = ISTAT.COM ")
            strSql.AppendLine(" LEFT OUTER JOIN Lista_Province ON IND.pro_cod_istat = Lista_Province.PROV ")

            strSql.AppendLine(" WHERE 1 = 1 ")

            If codContatto <> "" Then
                strSql.AppendLine(" AND IND.Cod_Contatto = '" & Agro_SQL_SaveText(codContatto) & "' ")
            End If

            If codIndirizzo <> 0 Then
                strSql.AppendLine(" AND IND.Cod_Indirizzo = " & Agro_SQL_SaveNum(codIndirizzo) & " ")
            End If

            If tipoIndirizzo <> 0 Then
                strSql.AppendLine(" AND IND.Tipo_Indirizzo = " & Agro_SQL_SaveNum(tipoIndirizzo) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   IND.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   IND.Inviato =-1 ")
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

    Public Function LeggiContatti_X_RapportiContab_X_TipiInd(
        ByRef objParametri As AgronicaCoreParametri,
        ByVal piva As String,
        Optional ByVal codContatto As String = "",
        Optional ByVal cliente As Boolean = False,
        Optional ByVal fornitore As Boolean = False,
        Optional ByVal dipendente As Boolean = False,
        Optional ByVal terzista As Boolean = False,
        Optional ByVal legale As Boolean = False,
        Optional ByVal agente As Boolean = False,
        Optional ByVal consulente As Boolean = False,
        Optional ByVal conferente As Boolean = False,
        Optional ByVal tipiIndirizzi As Integer() = Nothing
        ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Indirizzi_Read.LeggiContatti_X_RapportiContab_X_TipiInd()"

        Dim dtRis As DataTable
        Dim strSql As New StringBuilder

        Try
            Dim filtroContattiXIndirizzi = ""
            If tipiIndirizzi IsNot Nothing AndAlso tipiIndirizzi.Length > 0 Then
                filtroContattiXIndirizzi = "WHERE ContattiXIndirizzi.Tipo_Indirizzo IN (" & Agro_SQL_Save_Clausola_IN(String.Join(", ", tipiIndirizzi)) & ")"
            End If

            Dim filtroContatto = ""
            If Not String.IsNullOrEmpty(codContatto) Then
                filtroContatto = " AND Risorse_Umane.Cod_Contatto = '" & Agro_SQL_SaveText(codContatto) & "'"
            End If

            Dim listFiltriRappContab As New List(Of String)
            If cliente Then
                listFiltriRappContab.Add("Rapporti_Contabili.Cliente = 1")
            End If
            If fornitore Then
                listFiltriRappContab.Add("Rapporti_Contabili.Fornitore = 1")
            End If
            If dipendente Then
                listFiltriRappContab.Add("Rapporti_Contabili.Dipendente = 1")
            End If
            If terzista Then
                listFiltriRappContab.Add("Rapporti_Contabili.Terzista = 1")
            End If
            If legale Then
                listFiltriRappContab.Add("Rapporti_Contabili.Legale = 1")
            End If
            If agente Then
                listFiltriRappContab.Add("Rapporti_Contabili.Agente = 1")
            End If
            If consulente Then
                listFiltriRappContab.Add("Rapporti_Contabili.Consulente = 1")
            End If
            If conferente Then
                listFiltriRappContab.Add("Rapporti_Contabili.Cliente = 1 AND Rapporti_Contabili.Fornitore = 1")
            End If

            Dim filtroRappContab = ""
            If listFiltriRappContab.Count > 0 Then
                filtroRappContab = "WHERE (" & String.Join(") OR (", listFiltriRappContab) & ")"
            End If

            strSql.AppendLine("SELECT DISTINCT Risorse_Umane.Cod_Contatto, ")
            strSql.AppendLine("Risorse_Umane.Cod_Contatto AS CF_Piva,")
            strSql.AppendLine("Contatti.Id_CF, Contatti.Rag_Soc, Contatti.Cognome, Contatti.Nome,")
            strSql.AppendLine("Contatti.Rag_Soc + Contatti.Cognome + ' ' + Contatti.Nome AS Rag_Soc_Completa,")
            strSql.AppendLine("COALESCE(ContattiXIndirizzi_Tipo.Tipo_Indirizzo, 0) AS Tipo_Indirizzo,")
            strSql.AppendLine("COALESCE(Indirizzi.ind_des, '') AS ind_des, COALESCE(Indirizzi.com_des, '') AS com_des, COALESCE(Indirizzi.CAP, '') AS CAP, COALESCE(Indirizzi.frz_des, '') AS frz_des, COALESCE(Indirizzi.stato, '') AS stato")
            strSql.AppendLine("FROM Risorse_Umane")
            strSql.AppendLine("INNER JOIN (SELECT * FROM Rapporti_Contabili " & filtroRappContab & ") Rapporti_Contabili_Specifici")
            strSql.AppendLine("ON Risorse_Umane.Cod_Rapporto = Rapporti_Contabili_Specifici.Cod_Rapporto AND Rapporti_Contabili_Specifici.Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            strSql.AppendLine("INNER JOIN Contatti")
            strSql.AppendLine("ON Risorse_Umane.Piva = Contatti.Piva AND Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto")
            strSql.AppendLine("LEFT JOIN (SELECT * FROM ContattiXIndirizzi " & filtroContattiXIndirizzi & ") As ContattiXIndirizzi_Tipo")
            strSql.AppendLine("ON Risorse_Umane.Piva = ContattiXIndirizzi_Tipo.Piva AND Risorse_Umane.Cod_Contatto = ContattiXIndirizzi_Tipo.Cod_Contatto")
            strSql.AppendLine("LEFT JOIN Indirizzi")
            strSql.AppendLine("ON ContattiXIndirizzi_Tipo.Cod_Indirizzo = Indirizzi.cod_indirizzo")
            strSql.AppendLine("WHERE (Contatti.Sa_Cod = -1 OR Contatti.Piva = '" & Agro_SQL_SaveText(piva) & "') " & filtroContatto)
            strSql.AppendLine("AND GETDATE() BETWEEN Risorse_Umane.Validita_Inizio AND Risorse_Umane.Validita_Fine")
            strSql.Append("ORDER BY Risorse_Umane.Cod_Contatto ASC")

            '--------------------------------------------------------------------------
            dtRis = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Dim messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dtRis

    End Function

    Public Function Leggi_Indirizzo_Completo(Cod_Indirizzo As Integer,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Indirizzi.Leggi_Indirizzo_Completo()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT  ")
            StrSQL.AppendLine("      Indirizzi.Ind_Des AS Indirizzo ")
            StrSQL.AppendLine("    , Indirizzi.Frz_Des AS Frazione ")
            StrSQL.AppendLine("    , Indirizzi.CAP AS CAP ")
            StrSQL.AppendLine("    , Lista_Stati.Descrizione AS Stato ")
            StrSQL.AppendLine("    , Lista_Regioni.Regione_Des AS Regione ")
            StrSQL.AppendLine("    , Lista_Province.Provincia AS Provincia ")
            StrSQL.AppendLine("    , ISTAT.Localita AS Comune ")
            StrSQL.AppendLine(" FROM  Indirizzi ")
            StrSQL.AppendLine(" INNER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM ")
            StrSQL.AppendLine(" INNER JOIN Lista_Province ON Lista_Province.Sigla = ISTAT.COMUNI_PROV ")
            StrSQL.AppendLine(" INNER JOIN Lista_Regioni ON Lista_Regioni.REG = Lista_Province.REG ")
            StrSQL.AppendLine(" INNER JOIN ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166 Lista_Stati ON Lista_Stati.Codice = Lista_Regioni.Stato_Country ")

            If Cod_Indirizzo <> 0 Then
                StrSQL.Append(" AND Indirizzi.Cod_Indirizzo = " & Agro_SQL_SaveNum(Cod_Indirizzo) & " ")
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
End Class


'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§



Public Class Indirizzi_Write
    Inherits AgronicaCoreDataProvider.DataProvider

    'cod indirizzo calcolato
    Public Function ScriviNuovo(ByVal Ind_Des As String,
                                ByVal Frz_Des As String,
                                ByVal CAP As String,
                                ByVal Com_Des As String,
                                ByVal Pro_Cod As String,
                                ByVal Stato As String,
                                ByVal Note As String,
                                ByVal Pro_Cod_Istat As String,
                                ByVal Com_Cod_Istat As String,
                                ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                ByRef objParametri As AgronicaCoreParametri,
                                Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                                Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                                Optional ByVal username_creazione As String = "",
                                Optional ByVal username_modifica As String = "",
                                Optional ByVal Validazione As Integer = 0,
                                Optional ByVal Data_Validazione As DateTime = #2/1/1900#,
                                Optional ByVal UserName_Validazione As String = "",
                                Optional ByVal Codice_Lingua As String = "",
                                Optional ByVal Codice_Alternativo As String = ""
                                ) As Long

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Indirizzo_Write.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        If Data_creazione = #2/1/1900# Then
            Data_creazione = Now
        End If

        If Data_modifica = #2/1/1900# Then
            Data_modifica = Now
        End If

        If username_creazione = "" Then
            username_creazione = objParametri.UsernameOperazione
        End If

        If username_modifica = "" Then
            username_modifica = objParametri.UsernameOperazione
        End If

        If Data_Validazione = #2/1/1900# Then
            Data_Validazione = Now
        End If

        Dim codIndirizzo As Long = 0

        Try
            Dim objSequenzaTabelle As New Agro_Sequenze()
            Dim basecode = 0
            Dim topcode = UpperBoundTabelle_Per_SequenzaTabelle_Topcode
            codIndirizzo = objSequenzaTabelle.NuovoId_Tabella("indirizzi", basecode, topcode, objParametri)

            '---------------------------------------------
            strSql.Length = 0
            strSql.AppendLine("INSERT INTO Indirizzi( ")
            strSql.AppendLine("                     Cod_Indirizzo,   Ind_Des,       Frz_Des,    CAP,  ")
            strSql.AppendLine("                     Com_Des,         Pro_Cod,       Stato,      Note, ")
            strSql.AppendLine("                     Pro_Cod_Istat,   Com_Cod_Istat, ")

            strSql.AppendLine("                     Validazione,     Data_Validazione,    UserName_Validazione, ")
            strSql.AppendLine("                     Codice_Lingua,   Codice_Alternativo, ")

            strSql.AppendLine("                     Inviato,            DataInvio, ")
            strSql.AppendLine("                     Data_Creazione,     Data_Modifica, ")
            strSql.AppendLine("                     UserName_Creazione, UserName_Modifica, ")
            strSql.AppendLine("                     Validita_Inizio,    Validita_Fine ")
            strSql.AppendLine("                     ) ")


            strSql.AppendLine("VALUES ( ")
            strSql.AppendLine("           " & Agro_SQL_SaveNum(codIndirizzo) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Ind_Des) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Frz_Des) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(CAP) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Com_Des) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Pro_Cod) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Stato) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Note) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Pro_Cod_Istat) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Com_Cod_Istat) & "' ")

            strSql.AppendLine("         ," & Agro_SQL_SaveNum(Validazione) & " ")
            strSql.AppendLine("         ," & Agro_SQL_SaveDateTime(Data_Validazione) & " ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(UserName_Validazione) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Codice_Lingua) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Codice_Alternativo) & "' ")

            strSql.AppendLine("         , 0  ")
            strSql.AppendLine("         , Null  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            strSql.AppendLine(" )")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return codIndirizzo

    End Function

    Public Function Scrivi(ByVal cod_indirizzo As Long,
                           ByVal Ind_Des As String,
                           ByVal Frz_Des As String,
                           ByVal CAP As String,
                           ByVal Com_Des As String,
                           ByVal Pro_Cod As String,
                           ByVal Stato As String,
                           ByVal Note As String,
                           ByVal Pro_Cod_Istat As String,
                           ByVal Com_Cod_Istat As String,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = "",
                           Optional ByVal Validazione As Integer = 0,
                           Optional ByVal Data_Validazione As DateTime = #2/1/1900#,
                           Optional ByVal UserName_Validazione As String = "",
                           Optional ByVal Codice_Lingua As String = "",
                           Optional ByVal Codice_Alternativo As String = ""
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Indirizzo_Write.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        If Data_creazione = #2/1/1900# Then
            Data_creazione = Now
        End If

        If Data_modifica = #2/1/1900# Then
            Data_modifica = Now
        End If

        If username_creazione = "" Then
            username_creazione = objParametri.UsernameOperazione
        End If

        If username_modifica = "" Then
            username_modifica = objParametri.UsernameOperazione
        End If

        If Data_Validazione = #2/1/1900# Then
            Data_Validazione = Now
        End If

        Try

            strSql.Length = 0
            strSql.AppendLine("INSERT INTO Indirizzi( ")
            strSql.AppendLine("                     Cod_Indirizzo,   Ind_Des,       Frz_Des,    CAP,  ")
            strSql.AppendLine("                     Com_Des,         Pro_Cod,       Stato,      Note, ")
            strSql.AppendLine("                     Pro_Cod_Istat,   Com_Cod_Istat, ")

            strSql.AppendLine("                     Validazione,     Data_Validazione,    UserName_Validazione, ")
            strSql.AppendLine("                     Codice_Lingua,   Codice_Alternativo, ")

            strSql.AppendLine("                     Inviato,            DataInvio, ")
            strSql.AppendLine("                     Data_Creazione,     Data_Modifica, ")
            strSql.AppendLine("                     UserName_Creazione, UserName_Modifica, ")
            strSql.AppendLine("                     Validita_Inizio,    Validita_Fine ")
            strSql.AppendLine("                     ) ")


            strSql.AppendLine("VALUES ( ")
            strSql.AppendLine("           " & Agro_SQL_SaveNum(cod_indirizzo) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Ind_Des) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Frz_Des) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(CAP) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Com_Des) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Pro_Cod) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Stato) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Note) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Pro_Cod_Istat) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Com_Cod_Istat) & "' ")

            strSql.AppendLine("         ," & Agro_SQL_SaveNum(Validazione) & " ")
            strSql.AppendLine("         ," & Agro_SQL_SaveDateTime(Data_Validazione) & " ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(UserName_Validazione) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Codice_Lingua) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Codice_Alternativo) & "' ")

            strSql.AppendLine("         , 0  ")
            strSql.AppendLine("         , Null  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            strSql.AppendLine(" )")

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

    Public Function Modifica(ByVal cod_indirizzo As Long,
                             ByVal Ind_Des As String,
                             ByVal Frz_Des As String,
                             ByVal CAP As String,
                             ByVal Com_Des As String,
                             ByVal Pro_Cod As String,
                             ByVal Stato As String,
                             ByVal Note As String,
                             ByVal Pro_Cod_Istat As String,
                             ByVal Com_Cod_Istat As String,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri,
                             Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                             Optional ByVal username_modifica As String = "",
                             Optional ByVal Validazione As Integer? = Nothing,
                             Optional ByVal Data_Validazione As DateTime? = Nothing,
                             Optional ByVal UserName_Validazione As String = Nothing,
                             Optional ByVal Codice_Lingua As String = Nothing,
                             Optional ByVal Codice_Alternativo As String = Nothing
                             ) As Boolean
        'Optional ByVal tipo_indirizzo As Long


        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Indirizzo_Write.Modifica()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        If Data_modifica = #2/1/1900# Then
            Data_modifica = Now
        End If

        If username_modifica = "" Then
            username_modifica = objParametri.UsernameOperazione
        End If

        Try

            If cod_indirizzo = 0 Then
                Throw New Exception("Parametro non corretto nella query (Cod_indirizzo obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine("UPDATE Indirizzi SET ")
            strSql.AppendLine("    Ind_Des           = '" & Agro_SQL_SaveText(Ind_Des) & "'")
            strSql.AppendLine("   ,Frz_Des           = '" & Agro_SQL_SaveText(Frz_Des) & "'")
            strSql.AppendLine("   ,CAP               = '" & Agro_SQL_SaveText(CAP) & "'")
            strSql.AppendLine("   ,Com_Des           = '" & Agro_SQL_SaveText(Com_Des) & "'")
            strSql.AppendLine("   ,Pro_Cod           = '" & Agro_SQL_SaveText(Pro_Cod) & "'")
            strSql.AppendLine("   ,Stato             = '" & Agro_SQL_SaveText(Stato) & "'")
            strSql.AppendLine("   ,Note              = '" & Agro_SQL_SaveText(Note) & "'")
            strSql.AppendLine("   ,Pro_Cod_Istat     = '" & Agro_SQL_SaveText(Pro_Cod_Istat) & "'")
            strSql.AppendLine("   ,Com_Cod_Istat     = '" & Agro_SQL_SaveText(Com_Cod_Istat) & "'")

            strSql.AppendLine("   ,Inviato           =  0 ")
            strSql.AppendLine("   ,DataInvio         =  Null ")
            strSql.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Data_modifica))
            strSql.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "'")
            strSql.AppendLine("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.AppendLine("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine) & " ")

            If Not IsNothing(Validazione) Then
                strSql.AppendLine("   ,Validazione =  " & Agro_SQL_SaveNum(Validazione) & " ")
            End If

            If Not IsNothing(Data_Validazione) Then
                strSql.AppendLine("   ,Data_Validazione =  " & Agro_SQL_SaveDateTime(Data_Validazione) & " ")
            End If

            If Not IsNothing(UserName_Validazione) Then
                strSql.AppendLine("   ,UserName_Validazione =  '" & Agro_SQL_SaveText(UserName_Validazione) & "' ")
            End If

            If Not IsNothing(Codice_Lingua) Then
                strSql.AppendLine("   ,Codice_Lingua =  '" & Agro_SQL_SaveText(Codice_Lingua) & "' ")
            End If

            If Not IsNothing(Codice_Alternativo) Then
                strSql.AppendLine("   ,Codice_Alternativo =  '" & Agro_SQL_SaveText(Codice_Alternativo) & "' ")
            End If


            strSql.AppendLine(" WHERE Cod_Indirizzo = " & cod_indirizzo & " ")

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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

    Public Function Cancella(ByVal cod_indirizzo As Long,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Indirizzo_Write.Cancella()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If cod_indirizzo = 0 Then
                Throw New Exception("Parametro non corretto nella query (Cod_indirizzo obbligatorio)")
            End If

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.Length = 0
                strSql.AppendLine(" UPDATE Indirizzi ")
                strSql.AppendLine(" SET ")
                strSql.AppendLine("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.AppendLine("      ,Inviato = -1 ")
                strSql.AppendLine(" WHERE  Cod_Indirizzo = " & Agro_SQL_SaveNum(cod_indirizzo) & " ")
                strSql.AppendLine(" AND Inviato >= 0")

            Else

                strSql.Length = 0
                strSql.AppendLine(" DELETE ")
                strSql.AppendLine(" FROM     Indirizzi ")
                strSql.AppendLine(" WHERE  Cod_Indirizzo = " & Agro_SQL_SaveNum(cod_indirizzo) & " ")
                strSql.AppendLine(" AND Inviato = 0")

            End If


            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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
