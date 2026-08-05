Imports System.Data.Entity.Migrations
Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Imprese_Read
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function EsisteRecordInTabellaImprese(ByVal piva As String,
                                                 ByRef objParametri As AgronicaCoreParametri
                                                 ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Read.EsisteRecordInTabellaImprese()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.AppendLine(" SELECT *, ")
            Stb.AppendLine("  CASE WHEN ISNULL(partitaIvaReale, '') = '' THEN PIVA ELSE partitaIvaReale END PivaReale")
            Stb.AppendLine(" FROM imprese  ")
            Stb.AppendLine(" WHERE PIVA = '" & Agro_SQL_SaveText(piva) & "' ")

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

    ''' <summary>
    ''' Verifica che sia raggiungibile e attivo
    ''' </summary>
    ''' <param name="objP_Server"></param>
    ''' <returns></returns>
    Public Function IsAlive(ByRef objP_Server As AgronicaCoreParametri) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Imprese_Read.IsAlive()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim dt As DataTable
        Dim r As Boolean = False

        Try
            strSql.Length = 0
            strSql.AppendLine(" SELECT TOP(1) * FROM Imprese ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objP_Server, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                r = True
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objP_Server, nomeRoutine, messaggioErrore)
            r = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return r

    End Function



    '##############################################################################################
    Public Function DatiIntestazione_Impresa_Legale(ByVal Piva As String,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreParametri
                                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Read.DatiIntestazione_Impresa_Legale()"

        Dim MessaggioErrore As String = ""
        Dim StrSql As String = ""
        Dim DT As DataTable

        Try

            StrSql = ""
            StrSql = "  SELECT      Impresa.Rag_Soc, Contatti.Cod_Contatto, Contatti.Rag_Soc + Contatti.Nome + ' ' + Contatti.Cognome AS Rappr_Legale, Contatti.Codice_Fiscale, Risorse_Umane.Cod_RisUm, Risorse_Umane.Cod_Rapporto,  "
            StrSql &= "             ContattiXIndirizzo_Residenza.Cod_Indirizzo AS Cod_Indirizzo_Residenza, ContattiXIndirizzo_Residenza.Tipo_Indirizzo AS Tipo_Indirizzo_Residenza, "
            StrSql &= "             Indirizzo_Residenza.ind_des AS ind_residenza, Indirizzo_Residenza.frz_des AS fraz_residenza, ISNULL(Indirizzo_Residenza.CAP, '') AS cap_residenza,  "
            StrSql &= "             ISNULL(ISTAT_Residenza.LOCALITA, '') AS com_residenza, ISNULL(ISTAT_Residenza.COMUNI_PROV, '') AS prov_residenza,  "
            StrSql &= "             ContattiXIndirizzo_Nascita.Tipo_Indirizzo AS Tipo_Indirizzo_Nascita, ISNULL(ISTAT_Nascita.LOCALITA, '') AS com_nascita, "
            StrSql &= "             ISNULL(ISTAT_Nascita.COMUNI_PROV, '') AS prov_nascita, ISNULL(ISTAT_Nascita.CAP, '') AS cap_nascita, Contatti.Piva, Impresa.rag_soc, "
            StrSql &= "             ImpresexIndirizzo_Impresa.Tipo_Indirizzo AS tipo_indirizzo_impresa, Indirizzo_Impresa.ind_des AS ind_impresa, Indirizzo_Impresa.frz_des, "
            StrSql &= "             ISNULL(ISTAT_Impresa.LOCALITA, '') AS com_impresa, ISNULL(ISTAT_Impresa.COMUNI_PROV, '') AS prov_impresa, ISNULL(Indirizzo_Impresa.CAP, '') AS cap_impresa, "
            StrSql &= "             ISNULL  ((  SELECT    TOP 1 val_cod  "
            StrSql &= "                         FROM    Imprese_Codici AS Imprese_Codici_3  "
            StrSql &= "                         WHERE   Imprese_Codici_3.piva = Impresa.piva AND Imprese_Codici_3.id_cod = 1010), '') AS CUAA, "
            StrSql &= "             ISNULL  ((SELECT    val_cod  "
            StrSql &= "                         FROM    Imprese_Codici  "
            StrSql &= "                         WHERE   Impresa.PIVA = Imprese_Codici.PIVA AND id_cod = '1109'), ' ') AS Codice_ICQ, "
            StrSql &= "             ISNULL  ((SELECT TOP 1   Codice_Fiscale "
            StrSql &= "                         FROM    Contatti "
            StrSql &= "                         WHERE   Impresa.PIVA = Contatti.Cod_Contatto), ' ') AS Codice_Fiscale_Impresa "

            StrSql &= " FROM    Imprese Impresa "
            StrSql &= "         INNER JOIN ImpresexIndirizzi ImpresexIndirizzo_Impresa ON  Impresa.PIVA = ImpresexIndirizzo_Impresa.PIVA "
            StrSql &= "         INNER JOIN Indirizzi Indirizzo_Impresa ON Indirizzo_Impresa.cod_indirizzo = ImpresexIndirizzo_Impresa.cod_indirizzo "
            StrSql &= "         INNER JOIN ISTAT ISTAT_Impresa ON Indirizzo_Impresa.pro_cod_istat = ISTAT_Impresa.PROV AND Indirizzo_Impresa.com_cod_istat = ISTAT_Impresa.COM "

            StrSql &= "         INNER JOIN Contatti ON Impresa.PIVA = Contatti.Piva "
            StrSql &= "         INNER JOIN Risorse_Umane ON Contatti.Piva = Risorse_Umane.Piva AND Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto "

            StrSql &= "         INNER JOIN ContattiXIndirizzi ContattiXIndirizzo_Residenza ON Contatti.Piva = ContattiXIndirizzo_Residenza.Piva AND Contatti.Cod_Contatto = ContattiXIndirizzo_Residenza.Cod_Contatto "
            StrSql &= "         INNER JOIN Indirizzi Indirizzo_Residenza ON Indirizzo_Residenza.cod_indirizzo = ContattiXIndirizzo_Residenza.Cod_Indirizzo "
            StrSql &= "         INNER JOIN ISTAT ISTAT_Residenza ON Indirizzo_Residenza.pro_cod_istat = ISTAT_Residenza.PROV AND Indirizzo_Residenza.com_cod_istat = ISTAT_Residenza.COM "

            StrSql &= "         INNER JOIN ContattiXIndirizzi ContattiXIndirizzo_Nascita ON Contatti.Piva = ContattiXIndirizzo_Nascita.Piva AND Contatti.Cod_Contatto = ContattiXIndirizzo_Nascita.Cod_Contatto"
            StrSql &= "         INNER JOIN Indirizzi Indirizzo_Nascita ON ContattiXIndirizzo_Nascita.Cod_Indirizzo = Indirizzo_Nascita.cod_indirizzo "
            StrSql &= "         INNER JOIN ISTAT ISTAT_Nascita ON Indirizzo_Nascita.pro_cod_istat = ISTAT_Nascita.PROV AND Indirizzo_Nascita.com_cod_istat = ISTAT_Nascita.COM "

            StrSql &= " WHERE       (Risorse_Umane.Cod_Rapporto = - 1)  "
            StrSql &= " AND         (ContattiXIndirizzo_Residenza.Tipo_Indirizzo = 3)  "
            StrSql &= " AND         (Impresa.Piva = '" & Agro_SQL_SaveText(Piva) & "')  "
            StrSql &= " AND         (ContattiXIndirizzo_Nascita.Tipo_Indirizzo = 5) "




            If xFiltroAggiuntivo <> "" Then
                StrSql &= " AND " & xFiltroAggiuntivo
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSql &= " AND   Impresa.Inviato >=0 "
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSql &= " AND   Impresa.Inviato =-1 "
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSql &= " ORDER BY " & xOrderBy
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSql, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function


    Public Function DatiIntestazioneImpresa(ByVal Piva As String,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreParametri
                                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Read.DatiIntestazioneImpresa()"

        Dim MessaggioErrore As String = ""
        Dim StrSql As String = ""
        Dim DT As DataTable

        Try


            StrSql = ""
            StrSql = "  SELECT       Impresa.PIVA, ISNULL(Impresa.partitaIvaReale, '') AS partitaIvaReale, Impresa.rag_soc, ISNULL(Impresa.Forma_Giuridica, 0) Forma_Giuridica_Cod, ISNULL(FormeGiuridiche.FG_Des, '') AS Forma_Giuridica_Des, Impresa.TipoImpresaGerarchia, Impresa.Validita_Inizio, Impresa.Validita_Fine,        "
            StrSql &= "               ImpresexIndirizzo_Impresa.Tipo_Indirizzo AS tipo_indirizzo_impresa, Indirizzo_Impresa.ind_des AS ind_impresa, Indirizzo_Impresa.frz_des, ISTAT_Impresa.LOCALITA, ISTAT_Impresa.COMUNI_PROV, ISTAT_Impresa.CAP, "

            StrSql &= "             ISNULL  ((  SELECT    TOP 1 val_cod  "
            StrSql &= "                         FROM    Imprese_Codici AS Imprese_Codici_1  "
            StrSql &= "                         WHERE   Imprese_Codici_1.piva = Impresa.piva AND Imprese_Codici_1.id_cod = 1086), ' ') AS codice_libro_soci, "
            StrSql &= "             ISNULL  ((  SELECT    TOP 1 val_cod  "
            StrSql &= "                         FROM    Imprese_Codici AS Imprese_Codici_2  "
            StrSql &= "                         WHERE   Imprese_Codici_2.piva = Impresa.piva AND Imprese_Codici_2.id_cod = 1087), '01/01/1900') AS data_libro_soci, "
            StrSql &= "             ISNULL  ((  SELECT    TOP 1 val_cod  "
            StrSql &= "                         FROM    Imprese_Codici AS Imprese_Codici_3  "
            StrSql &= "                         WHERE   Imprese_Codici_3.piva = Impresa.piva AND Imprese_Codici_3.id_cod = 1033), ' ') AS codice_socio, "
            StrSql &= "             ISNULL  ((  SELECT    TOP 1 val_cod  "
            StrSql &= "                         FROM    Imprese_Codici AS Imprese_Codici_4  "
            StrSql &= "                         WHERE   Imprese_Codici_4.piva = Impresa.piva AND Imprese_Codici_4.id_cod = 1010), ' ') AS codice_cuaa, "
            StrSql &= "             ISNULL  ((  SELECT    TOP 1 val_cod  "
            StrSql &= "                         FROM    Imprese_Codici AS Imprese_Codici_5  "
            StrSql &= "                         WHERE   Imprese_Codici_5.piva = Impresa.piva AND Imprese_Codici_5.id_cod = 4), ' ') AS codice_ausl, "

            StrSql &= "             ISNULL(Contatti.Codice_Fiscale, '') AS Codice_Fiscale, ISNULL(CooperativaPadre.PIVA, '') AS piva_padre,  ISNULL(CooperativaPadre.rag_soc, '') AS Cooperativa, ISNULL(ImpresexIndirizzo_Coop.Tipo_Indirizzo, 0) AS tipo_indirizzo_coop, "
            StrSql &= "              ISNULL(Indirizzo_Coop.ind_des, '') AS ind_coop, ISNULL(ISTAT_Coop.LOCALITA, '') AS com_coop, ISNULL(ISTAT_Coop.COMUNI_PROV, '')  AS prov_coop, ISNULL(ISTAT_Coop.CAP, '') AS cap_coop   "

            StrSql &= " FROM    Imprese Impresa "
            StrSql &= "         LEFT OUTER JOIN FormeGiuridiche ON Impresa.Forma_Giuridica = FormeGiuridiche.FG_Cod "
            StrSql &= "         LEFT OUTER JOIN ImpresexIndirizzi ImpresexIndirizzo_Impresa ON  Impresa.PIVA = ImpresexIndirizzo_Impresa.PIVA "
            StrSql &= "         LEFT OUTER JOIN Indirizzi Indirizzo_Impresa ON Indirizzo_Impresa.cod_indirizzo = ImpresexIndirizzo_Impresa.cod_indirizzo "
            StrSql &= "         LEFT OUTER JOIN ISTAT ISTAT_Impresa ON Indirizzo_Impresa.pro_cod_istat = ISTAT_Impresa.PROV AND Indirizzo_Impresa.com_cod_istat = ISTAT_Impresa.COM "
            StrSql &= "         LEFT OUTER JOIN Contatti ON Impresa.PIVA = Contatti.Cod_Contatto "

            StrSql &= "         LEFT OUTER JOIN GerarchiaImprese ON Impresa.PIVA = GerarchiaImprese.Figlio "
            StrSql &= "         LEFT OUTER JOIN Imprese CooperativaPadre ON GerarchiaImprese.Padre = CooperativaPadre.PIVA "
            StrSql &= "         LEFT OUTER JOIN ImpresexIndirizzi ImpresexIndirizzo_Coop ON  ImpresexIndirizzo_Coop.PIVA = CooperativaPadre.PIVA "
            StrSql &= "         LEFT OUTER JOIN Indirizzi Indirizzo_Coop ON ImpresexIndirizzo_Coop.cod_indirizzo = Indirizzo_Coop.cod_indirizzo "
            StrSql &= "         LEFT OUTER JOIN ISTAT ISTAT_Coop ON ISTAT_Coop.PROV = Indirizzo_Coop.pro_cod_istat AND ISTAT_Coop.COM = Indirizzo_Coop.com_cod_istat "
            StrSql &= " "

            StrSql &= " WHERE       (Impresa.Piva = '" & Agro_SQL_SaveText(Piva) & "')  "




            If xFiltroAggiuntivo <> "" Then
                StrSql &= " AND " & xFiltroAggiuntivo
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSql &= " AND   Impresa.Inviato >=0 "
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSql &= " AND   Impresa.Inviato =-1 "
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSql &= " ORDER BY " & xOrderBy
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSql, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function


    Public Function DatiIntestazioneSocio(ByVal Piva As String,
                                          ByVal xFiltroAggiuntivo As String,
                                          ByVal xOrderBy As String,
                                          ByRef objParametri As AgronicaCoreParametri
                                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese.DatiIntestazioneSocio()"

        Dim MessaggioErrore As String = ""
        Dim StrSql As String = ""
        Dim DT As DataTable

        Try

            StrSql = ""
            StrSql = "  SELECT ISNULL(Contatti.Cod_Contatto, '') AS Cod_Contatto, ISNULL(Contatti.Rag_Soc + Contatti.Cognome + ' ' + Contatti.Nome, '') AS Rappr_Legale, ISNULL(Contatti.Codice_Fiscale, '') AS Codice_Fiscale, ISNULL(Risorse_Umane.Cod_RisUm, 0) AS Cod_RisUm, ISNULL(Risorse_Umane.Cod_Rapporto, 0) AS Cod_Rapporto, " & vbCrLf
            '28/01/2019: aggiunte le date di validità del legale rappresentante per gestire il cambio del legale sull'azienda Terre da Frutta di Terremerse
            StrSql &= "        isnull(Risorse_Umane.Validita_Inizio, '01/01/1900') AS DataInizio_Legale, isnull(Risorse_Umane.Validita_Fine,'31/12/2100') AS DataFine_Legale,    " & vbCrLf

            StrSql &= "        ISNULL( ContattiXIndirizzo_Residenza.Cod_Indirizzo,0) AS Cod_Indirizzo_Residenza, ISNULL(ContattiXIndirizzo_Residenza.Tipo_Indirizzo, 0) AS Tipo_Indirizzo_Residenza,    " & vbCrLf
            StrSql &= "        ISNULL(Indirizzo_Residenza.ind_des, '') AS ind_residenza, ISNULL(Indirizzo_Residenza.frz_des,'') AS fraz_residenza, ISNULL(ISTAT_Residenza.CAP, '') AS cap_residenza,  " & vbCrLf
            StrSql &= "        ISNULL(ISTAT_Residenza.LOCALITA, '') AS com_residenza, ISNULL(ISTAT_Residenza.COMUNI_PROV, '') AS prov_residenza,  " & vbCrLf
            '28/01/2019: aggiunta data_nascita
            StrSql &= "        ISNULL(data_nascita, '01/01/1900') AS data_nascita, ISNULL(ContattiXIndirizzo_Nascita.Tipo_Indirizzo, 0) AS Tipo_Indirizzo_Nascita, ISNULL(ISTAT_Nascita.LOCALITA, '') AS com_nascita,  ISNULL(ISTAT_Nascita.COMUNI_PROV, '') AS prov_nascita," & vbCrLf
            StrSql &= "        ISNULL(ISTAT_Nascita.CAP, '') AS cap_nascita, Impresa.Piva, Impresa.rag_soc, " & vbCrLf
            StrSql &= "        ImpresexIndirizzo_Impresa.Tipo_Indirizzo AS tipo_indirizzo_impresa, Indirizzo_Impresa.ind_des AS ind_impresa, Indirizzo_Impresa.frz_des, ISNULL(ISTAT_Impresa.LOCALITA, '') AS com_impresa, ISNULL(ISTAT_Impresa.COMUNI_PROV, '') AS prov_impresa, ISNULL(ISTAT_Impresa.CAP, '') AS cap_impresa, " & vbCrLf
            StrSql &= "             ISNULL  ((  SELECT    TOP 1 val_cod  " & vbCrLf
            StrSql &= "                         FROM    Imprese_Codici AS Imprese_Codici_1  " & vbCrLf
            StrSql &= "                         WHERE   Imprese_Codici_1.piva = Impresa.piva AND Imprese_Codici_1.id_cod = 1086), ' ') AS codice_libro_soci, " & vbCrLf
            StrSql &= "             ISNULL  ((  SELECT    TOP 1 val_cod  " & vbCrLf
            StrSql &= "                         FROM    Imprese_Codici AS Imprese_Codici_2  " & vbCrLf
            StrSql &= "                         WHERE   Imprese_Codici_2.piva = Impresa.piva AND Imprese_Codici_2.id_cod = 1087), '01/01/1900') AS data_libro_soci, " & vbCrLf
            StrSql &= "             ISNULL  ((  SELECT    TOP 1 val_cod  " & vbCrLf
            StrSql &= "                         FROM    Imprese_Codici AS Imprese_Codici_3  " & vbCrLf
            StrSql &= "                         WHERE   Imprese_Codici_3.piva = Impresa.piva AND Imprese_Codici_3.id_cod = 1010), '') AS CUAA, " & vbCrLf

            StrSql &= "             ISNULL(CooperativaPadre.PIVA, '') AS piva_padre,  ISNULL(CooperativaPadre.rag_soc, '') AS Cooperativa, ISNULL(ImpresexIndirizzo_Coop.Tipo_Indirizzo, 0) AS tipo_indirizzo_coop, " & vbCrLf
            StrSql &= "             ISNULL(Indirizzo_Coop.ind_des, '') AS ind_coop, ISNULL(ISTAT_Coop.LOCALITA, '') AS com_coop, ISNULL(ISTAT_Coop.COMUNI_PROV, '')  AS prov_coop, ISNULL(ISTAT_Coop.CAP, '') AS cap_coop   " & vbCrLf

            StrSql &= " FROM    Imprese Impresa " & vbCrLf
            StrSql &= "         INNER JOIN ImpresexIndirizzi ImpresexIndirizzo_Impresa ON  Impresa.PIVA = ImpresexIndirizzo_Impresa.PIVA " & vbCrLf
            StrSql &= "         INNER JOIN Indirizzi Indirizzo_Impresa ON Indirizzo_Impresa.cod_indirizzo = ImpresexIndirizzo_Impresa.cod_indirizzo " & vbCrLf
            StrSql &= "         LEFT OUTER JOIN ISTAT ISTAT_Impresa ON Indirizzo_Impresa.pro_cod_istat = ISTAT_Impresa.PROV AND Indirizzo_Impresa.com_cod_istat = ISTAT_Impresa.COM " & vbCrLf
            StrSql &= " " & vbCrLf
            StrSql &= "         LEFT OUTER JOIN Risorse_Umane ON Impresa.PIVA = Risorse_Umane.Piva    AND   (Risorse_Umane.Cod_Rapporto = - 1) " & vbCrLf
            StrSql &= "         LEFT OUTER JOIN Contatti ON Contatti.Piva = Risorse_Umane.Piva AND Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto  " & vbCrLf
            StrSql &= " " & vbCrLf
            StrSql &= "         LEFT OUTER JOIN ContattiXIndirizzi ContattiXIndirizzo_Residenza ON Contatti.Piva = ContattiXIndirizzo_Residenza.Piva AND Contatti.Cod_Contatto = ContattiXIndirizzo_Residenza.Cod_Contatto AND         (ContattiXIndirizzo_Residenza.Tipo_Indirizzo = 3)" & vbCrLf
            StrSql &= "         LEFT OUTER JOIN Indirizzi Indirizzo_Residenza ON Indirizzo_Residenza.cod_indirizzo = ContattiXIndirizzo_Residenza.Cod_Indirizzo " & vbCrLf
            StrSql &= "         LEFT OUTER JOIN ISTAT ISTAT_Residenza ON Indirizzo_Residenza.pro_cod_istat = ISTAT_Residenza.PROV AND Indirizzo_Residenza.com_cod_istat = ISTAT_Residenza.COM " & vbCrLf
            StrSql &= " " & vbCrLf
            StrSql &= "         LEFT OUTER JOIN ContattiXIndirizzi ContattiXIndirizzo_Nascita ON Contatti.Piva = ContattiXIndirizzo_Nascita.Piva AND Contatti.Cod_Contatto = ContattiXIndirizzo_Nascita.Cod_Contatto AND         (ContattiXIndirizzo_Nascita.Tipo_Indirizzo = 5) " & vbCrLf
            StrSql &= "         LEFT OUTER JOIN Indirizzi Indirizzo_Nascita ON ContattiXIndirizzo_Nascita.Cod_Indirizzo = Indirizzo_Nascita.cod_indirizzo " & vbCrLf
            StrSql &= "         LEFT OUTER JOIN ISTAT ISTAT_Nascita ON Indirizzo_Nascita.pro_cod_istat = ISTAT_Nascita.PROV AND Indirizzo_Nascita.com_cod_istat = ISTAT_Nascita.COM " & vbCrLf
            StrSql &= " " & vbCrLf
            StrSql &= "         LEFT OUTER JOIN GerarchiaImprese ON Impresa.PIVA = GerarchiaImprese.Figlio " & vbCrLf
            StrSql &= "         LEFT OUTER JOIN Imprese CooperativaPadre ON GerarchiaImprese.Padre = CooperativaPadre.PIVA " & vbCrLf
            StrSql &= "         LEFT OUTER JOIN ImpresexIndirizzi ImpresexIndirizzo_Coop ON  ImpresexIndirizzo_Coop.PIVA = CooperativaPadre.PIVA " & vbCrLf
            StrSql &= "         LEFT OUTER JOIN Indirizzi Indirizzo_Coop ON ImpresexIndirizzo_Coop.cod_indirizzo = Indirizzo_Coop.cod_indirizzo " & vbCrLf
            StrSql &= "         LEFT OUTER JOIN ISTAT ISTAT_Coop ON ISTAT_Coop.PROV = Indirizzo_Coop.pro_cod_istat AND ISTAT_Coop.COM = Indirizzo_Coop.com_cod_istat " & vbCrLf
            StrSql &= " "

            StrSql &= " WHERE   Impresa.Piva = '" & Agro_SQL_SaveText(Piva) & "'  "

            If xFiltroAggiuntivo <> "" Then
                StrSql &= " AND " & xFiltroAggiuntivo
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSql &= " AND   Impresa.Inviato >=0 "
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSql &= " AND   Impresa.Inviato =-1 "
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSql &= " ORDER BY " & xOrderBy
            Else
                '2019/01/28: aggiunto ordinamento per visualizzare l'ultimo legale rappresentante attivo
                StrSql &= " ORDER BY DataInizio_Legale DESC "
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSql, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function DatiIntestazioneSocio_New(ByVal Piva As String,
                                              ByVal xFiltroAggiuntivo As String,
                                              ByVal xOrderBy As String,
                                              ByRef objParametri As AgronicaCoreParametri
                                              ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese.DatiIntestazioneSocio()"

        Dim MessaggioErrore As String = ""
        Dim StrSql As String = ""
        Dim DT As DataTable

        Try

            StrSql = ""
            StrSql = "  SELECT ISNULL(Contatti.Cod_Contatto, '') AS Cod_Contatto, ISNULL(Contatti.Rag_Soc + Contatti.Cognome + ' ' + Contatti.Nome, '') AS Rappr_Legale, ISNULL(ISNULL(NULLIF(Contatti.Codice_Fiscale, ''), Contatti.Cod_Contatto), '') AS Codice_Fiscale, ISNULL(Risorse_Umane.Cod_RisUm, 0) AS Cod_RisUm, ISNULL(Risorse_Umane.Cod_Rapporto, 0) AS Cod_Rapporto, " & vbCrLf
            '28/01/2019: aggiunte le date di validità del legale rappresentante per gestire il cambio del legale sull'azienda Terre da Frutta di Terremerse
            StrSql &= "        isnull(Risorse_Umane.Validita_Inizio, '01/01/1900') AS DataInizio_Legale, isnull(Risorse_Umane.Validita_Fine,'31/12/2100') AS DataFine_Legale,    " & vbCrLf

            StrSql &= "        ISNULL( ContattiXIndirizzo_Residenza.Cod_Indirizzo,0) AS Cod_Indirizzo_Residenza, ISNULL(ContattiXIndirizzo_Residenza.Tipo_Indirizzo, 0) AS Tipo_Indirizzo_Residenza,    " & vbCrLf
            StrSql &= "        ISNULL(Indirizzo_Residenza.ind_des, '') AS ind_residenza, ISNULL(Indirizzo_Residenza.frz_des,'') AS fraz_residenza, ISNULL(ISTAT_Residenza.CAP, '') AS cap_residenza,  " & vbCrLf
            StrSql &= "        ISNULL(ISTAT_Residenza.LOCALITA, '') AS com_residenza, ISNULL(ISTAT_Residenza.COMUNI_PROV, '') AS prov_residenza,  " & vbCrLf
            '28/01/2019: aggiunta data_nascita
            StrSql &= "        ISNULL(Contatti.data_nascita, '01/01/1900') AS data_nascita, ISNULL(ContattiXIndirizzo_Nascita.Tipo_Indirizzo, 0) AS Tipo_Indirizzo_Nascita, ISNULL(ISTAT_Nascita.LOCALITA, '') AS com_nascita,  ISNULL(ISTAT_Nascita.COMUNI_PROV, '') AS prov_nascita," & vbCrLf
            StrSql &= "        ISNULL(ISTAT_Nascita.CAP, '') AS cap_nascita, Impresa.Piva, Impresa.rag_soc, " & vbCrLf
            StrSql &= "        ImpresexIndirizzo_Impresa.Tipo_Indirizzo AS tipo_indirizzo_impresa, Indirizzo_Impresa.ind_des AS ind_impresa, Indirizzo_Impresa.frz_des, ISNULL(ISTAT_Impresa.LOCALITA, '') AS com_impresa, ISNULL(ISTAT_Impresa.COMUNI_PROV, '') AS prov_impresa, ISNULL(ISTAT_Impresa.CAP, '') AS cap_impresa, " & vbCrLf
            StrSql &= "             ISNULL  ((  SELECT    TOP 1 LibroSoci_Codice  " & vbCrLf
            StrSql &= "                         FROM    GerarchiaImprese AS GerarchiaImprese_1  " & vbCrLf
            StrSql &= "                         WHERE   GerarchiaImprese_1.Figlio = Impresa.piva  " & vbCrLf
            StrSql &= "                         AND GerarchiaImprese_1.Padre = CooperativaPadre.PIVA), ' ') AS codice_libro_soci, " & vbCrLf
            StrSql &= "             ISNULL  ((  SELECT    TOP 1 LibroSoci_DataIscrizione  " & vbCrLf
            StrSql &= "                         FROM    GerarchiaImprese AS GerarchiaImprese_2  " & vbCrLf
            StrSql &= "                         WHERE   GerarchiaImprese_2.Figlio = Impresa.piva " & vbCrLf
            StrSql &= "                         AND GerarchiaImprese_2.Padre = CooperativaPadre.PIVA), '01/01/1900') AS data_libro_soci, " & vbCrLf
            StrSql &= "             ISNULL  ((  SELECT    TOP 1 val_cod  " & vbCrLf
            StrSql &= "                         FROM    Imprese_Codici  " & vbCrLf
            StrSql &= "                         WHERE   Imprese_Codici.piva = Impresa.piva AND Imprese_Codici.id_cod = " & enum_CodiciAnagrafe.CodiceCUAA & "), '') AS CUAA, " & vbCrLf

            StrSql &= "         (SELECT COUNT(Padri.Padre) as num_padri  " & vbCrLf
            StrSql &= "             From Imprese INNER Join GerarchiaImprese As Padri On Padri.Figlio = '" & Agro_SQL_SaveText(Piva) & "'  " & vbCrLf
            StrSql &= "             Where Piva Like '" & Agro_SQL_SaveText(Piva) & "') As num_padri,  " & vbCrLf

            StrSql &= "             ISNULL(CooperativaPadre.PIVA, '') AS piva_padre,  ISNULL(CooperativaPadre.rag_soc, '') AS Cooperativa, ISNULL(ImpresexIndirizzo_Coop.Tipo_Indirizzo, 0) AS tipo_indirizzo_coop, " & vbCrLf
            StrSql &= "             ISNULL(Indirizzo_Coop.ind_des, '') AS ind_coop, ISNULL(ISTAT_Coop.LOCALITA, '') AS com_coop, ISNULL(ISTAT_Coop.COMUNI_PROV, '')  AS prov_coop, ISNULL(ISTAT_Coop.CAP, '') AS cap_coop,   " & vbCrLf

            StrSql &= "             ISNULL(Contatti_Coop.Cognome + ' ' + Contatti_Coop.Nome, '') AS nome_ref_coop,  " & vbCrLf
            StrSql &= "             ISNULL(Imprese_CodiciBP.val_cod, '') AS codice_bp  " & vbCrLf

            StrSql &= " FROM    Imprese Impresa " & vbCrLf
            StrSql &= "         INNER JOIN ImpresexIndirizzi ImpresexIndirizzo_Impresa ON  Impresa.PIVA = ImpresexIndirizzo_Impresa.PIVA " & vbCrLf
            StrSql &= "         INNER JOIN Indirizzi Indirizzo_Impresa ON Indirizzo_Impresa.cod_indirizzo = ImpresexIndirizzo_Impresa.cod_indirizzo " & vbCrLf
            StrSql &= "         LEFT OUTER JOIN ISTAT ISTAT_Impresa ON Indirizzo_Impresa.pro_cod_istat = ISTAT_Impresa.PROV AND Indirizzo_Impresa.com_cod_istat = ISTAT_Impresa.COM " & vbCrLf
            StrSql &= " " & vbCrLf
            StrSql &= "         LEFT OUTER JOIN Risorse_Umane ON Impresa.PIVA = Risorse_Umane.Piva    AND   (Risorse_Umane.Cod_Rapporto = - 1) " & vbCrLf
            StrSql &= "         LEFT OUTER JOIN Contatti ON Contatti.Piva = Risorse_Umane.Piva AND Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto  " & vbCrLf
            StrSql &= " " & vbCrLf
            StrSql &= "         LEFT OUTER JOIN ContattiXIndirizzi ContattiXIndirizzo_Residenza ON Contatti.Piva = ContattiXIndirizzo_Residenza.Piva AND Contatti.Cod_Contatto = ContattiXIndirizzo_Residenza.Cod_Contatto AND         (ContattiXIndirizzo_Residenza.Tipo_Indirizzo = " & enum_IndirizzoTipo.Residenza & ")" & vbCrLf
            StrSql &= "         LEFT OUTER JOIN Indirizzi Indirizzo_Residenza ON Indirizzo_Residenza.cod_indirizzo = ContattiXIndirizzo_Residenza.Cod_Indirizzo " & vbCrLf
            StrSql &= "         LEFT OUTER JOIN ISTAT ISTAT_Residenza ON Indirizzo_Residenza.pro_cod_istat = ISTAT_Residenza.PROV AND Indirizzo_Residenza.com_cod_istat = ISTAT_Residenza.COM " & vbCrLf
            StrSql &= " " & vbCrLf
            StrSql &= "         LEFT OUTER JOIN ContattiXIndirizzi ContattiXIndirizzo_Nascita ON Contatti.Piva = ContattiXIndirizzo_Nascita.Piva AND Contatti.Cod_Contatto = ContattiXIndirizzo_Nascita.Cod_Contatto AND         (ContattiXIndirizzo_Nascita.Tipo_Indirizzo = " & enum_IndirizzoTipo.LuogoNascita & ") " & vbCrLf
            StrSql &= "         LEFT OUTER JOIN Indirizzi Indirizzo_Nascita ON ContattiXIndirizzo_Nascita.Cod_Indirizzo = Indirizzo_Nascita.cod_indirizzo " & vbCrLf
            StrSql &= "         LEFT OUTER JOIN ISTAT ISTAT_Nascita ON Indirizzo_Nascita.pro_cod_istat = ISTAT_Nascita.PROV AND Indirizzo_Nascita.com_cod_istat = ISTAT_Nascita.COM " & vbCrLf
            StrSql &= " " & vbCrLf
            StrSql &= "         LEFT OUTER JOIN GerarchiaImprese ON Impresa.PIVA = GerarchiaImprese.Figlio " & vbCrLf
            StrSql &= "         LEFT OUTER JOIN Imprese CooperativaPadre ON GerarchiaImprese.Padre = CooperativaPadre.PIVA " & vbCrLf
            StrSql &= "         LEFT OUTER JOIN ImpresexIndirizzi ImpresexIndirizzo_Coop ON  ImpresexIndirizzo_Coop.PIVA = CooperativaPadre.PIVA " & vbCrLf
            StrSql &= "         LEFT OUTER JOIN Indirizzi Indirizzo_Coop ON ImpresexIndirizzo_Coop.cod_indirizzo = Indirizzo_Coop.cod_indirizzo " & vbCrLf
            StrSql &= "         LEFT OUTER JOIN ISTAT ISTAT_Coop ON ISTAT_Coop.PROV = Indirizzo_Coop.pro_cod_istat AND ISTAT_Coop.COM = Indirizzo_Coop.com_cod_istat " & vbCrLf
            StrSql &= " " & vbCrLf
            StrSql &= "         LEFT OUTER JOIN Imprese_Codici AS Imprese_CodiciCoop ON CooperativaPadre.PIVA = Imprese_CodiciCoop.PIVA AND id_cod = " & enum_CodiciAnagrafe.Tecnico & " " & vbCrLf
            StrSql &= "         LEFT OUTER JOIN Contatti AS Contatti_Coop ON Contatti_Coop.Cod_Contatto = Imprese_CodiciCoop.val_cod " & vbCrLf
            StrSql &= "         LEFT OUTER JOIN Imprese_Codici AS Imprese_CodiciBP ON Imprese_CodiciBP.PIVA = '" & Agro_SQL_SaveText(Piva) & "' AND Imprese_CodiciBP.id_cod = " & enum_CodiciAnagrafe.Codice_Socio & " " & vbCrLf
            StrSql &= " "

            StrSql &= " WHERE   Impresa.Piva = '" & Agro_SQL_SaveText(Piva) & "'  "

            If xFiltroAggiuntivo <> "" Then
                StrSql &= " AND " & xFiltroAggiuntivo
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSql &= " AND   Impresa.Inviato >=0 "
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSql &= " AND   Impresa.Inviato =-1 "
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSql &= " ORDER BY " & xOrderBy
            Else
                '2019/01/28: aggiunto ordinamento per visualizzare l'ultimo legale rappresentante attivo
                StrSql &= " ORDER BY DataInizio_Legale DESC "
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSql, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    '##################################################################################
    Public Function DatiIntestazioneSoci_New(ByVal Pive As List(Of String),
                                              ByVal xFiltroAggiuntivo As String,
                                              ByVal xOrderBy As String,
                                              ByRef objParametri As AgronicaCoreParametri
                                              ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese.DatiIntestazioneSocio()"

        Dim MessaggioErrore As String = ""
        Dim StrSql As String = ""
        Dim DT As DataTable

        Try

            If Pive Is Nothing OrElse Pive.Count = 0 Then
                Throw New Exception("La lista di PIVA è vuota.")
            End If

            ConnessioniTransazioni.ApriConnessione(True, objParametri)

            TempChiaviMassivo.CreaTabellaTemp_FiltroPiva(Pive, NomeRoutine, objParametri)

            StrSql = ""
            StrSql = "  SELECT ISNULL(Contatti.Cod_Contatto, '') AS Cod_Contatto, ISNULL(Contatti.Rag_Soc + Contatti.Cognome + ' ' + Contatti.Nome, '') AS Rappr_Legale, ISNULL(ISNULL(NULLIF(Contatti.Codice_Fiscale, ''), Contatti.Cod_Contatto), '') AS Codice_Fiscale, ISNULL(Risorse_Umane.Cod_RisUm, 0) AS Cod_RisUm, ISNULL(Risorse_Umane.Cod_Rapporto, 0) AS Cod_Rapporto, " & vbCrLf
            '28/01/2019: aggiunte le date di validità del legale rappresentante per gestire il cambio del legale sull'azienda Terre da Frutta di Terremerse
            StrSql &= "        isnull(Risorse_Umane.Validita_Inizio, '01/01/1900') AS DataInizio_Legale, isnull(Risorse_Umane.Validita_Fine,'31/12/2100') AS DataFine_Legale,    " & vbCrLf

            StrSql &= "        ISNULL( ContattiXIndirizzo_Residenza.Cod_Indirizzo,0) AS Cod_Indirizzo_Residenza, ISNULL(ContattiXIndirizzo_Residenza.Tipo_Indirizzo, 0) AS Tipo_Indirizzo_Residenza,    " & vbCrLf
            StrSql &= "        ISNULL(Indirizzo_Residenza.ind_des, '') AS ind_residenza, ISNULL(Indirizzo_Residenza.frz_des,'') AS fraz_residenza, ISNULL(ISTAT_Residenza.CAP, '') AS cap_residenza,  " & vbCrLf
            StrSql &= "        ISNULL(ISTAT_Residenza.LOCALITA, '') AS com_residenza, ISNULL(ISTAT_Residenza.COMUNI_PROV, '') AS prov_residenza,  " & vbCrLf
            '28/01/2019: aggiunta data_nascita
            StrSql &= "        ISNULL(Contatti.data_nascita, '01/01/1900') AS data_nascita, ISNULL(ContattiXIndirizzo_Nascita.Tipo_Indirizzo, 0) AS Tipo_Indirizzo_Nascita, ISNULL(ISTAT_Nascita.LOCALITA, '') AS com_nascita,  ISNULL(ISTAT_Nascita.COMUNI_PROV, '') AS prov_nascita," & vbCrLf
            StrSql &= "        ISNULL(ISTAT_Nascita.CAP, '') AS cap_nascita, Impresa.PIVA, CASE WHEN ISNULL(Impresa.partitaIvaReale, '') = '' THEN Impresa.PIVA ELSE Impresa.partitaIvaReale END AS PivaReale, Impresa.rag_soc, " & vbCrLf
            StrSql &= "        ImpresexIndirizzo_Impresa.Tipo_Indirizzo AS tipo_indirizzo_impresa, Indirizzo_Impresa.ind_des AS ind_impresa, Indirizzo_Impresa.frz_des, ISNULL(ISTAT_Impresa.LOCALITA, '') AS com_impresa, ISNULL(ISTAT_Impresa.COMUNI_PROV, '') AS prov_impresa, ISNULL(ISTAT_Impresa.CAP, '') AS cap_impresa, " & vbCrLf
            StrSql &= "             ISNULL  ((  SELECT    TOP 1 LibroSoci_Codice  " & vbCrLf
            StrSql &= "                         FROM    GerarchiaImprese AS GerarchiaImprese_1  " & vbCrLf
            StrSql &= "                         WHERE   GerarchiaImprese_1.Figlio = Impresa.piva  " & vbCrLf
            StrSql &= "                         AND GerarchiaImprese_1.Padre = CooperativaPadre.PIVA), ' ') AS codice_libro_soci, " & vbCrLf
            StrSql &= "             ISNULL  ((  SELECT    TOP 1 LibroSoci_DataIscrizione  " & vbCrLf
            StrSql &= "                         FROM    GerarchiaImprese AS GerarchiaImprese_2  " & vbCrLf
            StrSql &= "                         WHERE   GerarchiaImprese_2.Figlio = Impresa.piva " & vbCrLf
            StrSql &= "                         AND GerarchiaImprese_2.Padre = CooperativaPadre.PIVA), '01/01/1900') AS data_libro_soci, " & vbCrLf
            StrSql &= "             ISNULL  ((  SELECT    TOP 1 val_cod  " & vbCrLf
            StrSql &= "                         FROM    Imprese_Codici  " & vbCrLf
            StrSql &= "                         WHERE   Imprese_Codici.piva = Impresa.piva AND Imprese_Codici.id_cod = " & enum_CodiciAnagrafe.CodiceCUAA & "), '') AS CUAA, " & vbCrLf

            StrSql &= "         (SELECT COUNT(Padri.Padre) as num_padri  " & vbCrLf
            StrSql &= "             From Imprese INNER Join GerarchiaImprese As Padri On Padri.Figlio = Impresa.Piva  " & vbCrLf
            StrSql &= "             Where Piva Like Impresa.Piva) As num_padri,  " & vbCrLf

            StrSql &= "             ISNULL(CooperativaPadre.PIVA, '') AS piva_padre,  ISNULL(CooperativaPadre.rag_soc, '') AS Cooperativa, ISNULL(ImpresexIndirizzo_Coop.Tipo_Indirizzo, 0) AS tipo_indirizzo_coop, " & vbCrLf
            StrSql &= "             ISNULL(Indirizzo_Coop.ind_des, '') AS ind_coop, ISNULL(ISTAT_Coop.LOCALITA, '') AS com_coop, ISNULL(ISTAT_Coop.COMUNI_PROV, '')  AS prov_coop, ISNULL(ISTAT_Coop.CAP, '') AS cap_coop,   " & vbCrLf

            StrSql &= "             ISNULL(Contatti_Coop.Cognome + ' ' + Contatti_Coop.Nome, '') AS nome_ref_coop,  " & vbCrLf
            StrSql &= "             ISNULL(Imprese_CodiciBP.val_cod, '') AS codice_bp  " & vbCrLf

            StrSql &= " FROM    Imprese Impresa " & vbCrLf
            StrSql &= "         INNER JOIN #TempPiva tmp ON tmp.Piva = Impresa.Piva COLLATE DATABASE_DEFAULT " & vbCrLf
            StrSql &= "         INNER JOIN ImpresexIndirizzi ImpresexIndirizzo_Impresa ON  Impresa.PIVA = ImpresexIndirizzo_Impresa.PIVA " & vbCrLf
            StrSql &= "         INNER JOIN Indirizzi Indirizzo_Impresa ON Indirizzo_Impresa.cod_indirizzo = ImpresexIndirizzo_Impresa.cod_indirizzo " & vbCrLf
            StrSql &= "         LEFT OUTER JOIN ISTAT ISTAT_Impresa ON Indirizzo_Impresa.pro_cod_istat = ISTAT_Impresa.PROV AND Indirizzo_Impresa.com_cod_istat = ISTAT_Impresa.COM " & vbCrLf
            StrSql &= " " & vbCrLf
            StrSql &= "         LEFT OUTER JOIN Risorse_Umane ON Impresa.PIVA = Risorse_Umane.Piva    AND   (Risorse_Umane.Cod_Rapporto = - 1) " & vbCrLf
            StrSql &= "         LEFT OUTER JOIN Contatti ON Contatti.Piva = Risorse_Umane.Piva AND Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto  " & vbCrLf
            StrSql &= " " & vbCrLf
            StrSql &= "         LEFT OUTER JOIN ContattiXIndirizzi ContattiXIndirizzo_Residenza ON Contatti.Piva = ContattiXIndirizzo_Residenza.Piva AND Contatti.Cod_Contatto = ContattiXIndirizzo_Residenza.Cod_Contatto AND         (ContattiXIndirizzo_Residenza.Tipo_Indirizzo = " & enum_IndirizzoTipo.Residenza & ")" & vbCrLf
            StrSql &= "         LEFT OUTER JOIN Indirizzi Indirizzo_Residenza ON Indirizzo_Residenza.cod_indirizzo = ContattiXIndirizzo_Residenza.Cod_Indirizzo " & vbCrLf
            StrSql &= "         LEFT OUTER JOIN ISTAT ISTAT_Residenza ON Indirizzo_Residenza.pro_cod_istat = ISTAT_Residenza.PROV AND Indirizzo_Residenza.com_cod_istat = ISTAT_Residenza.COM " & vbCrLf
            StrSql &= " " & vbCrLf
            StrSql &= "         LEFT OUTER JOIN ContattiXIndirizzi ContattiXIndirizzo_Nascita ON Contatti.Piva = ContattiXIndirizzo_Nascita.Piva AND Contatti.Cod_Contatto = ContattiXIndirizzo_Nascita.Cod_Contatto AND         (ContattiXIndirizzo_Nascita.Tipo_Indirizzo = " & enum_IndirizzoTipo.LuogoNascita & ") " & vbCrLf
            StrSql &= "         LEFT OUTER JOIN Indirizzi Indirizzo_Nascita ON ContattiXIndirizzo_Nascita.Cod_Indirizzo = Indirizzo_Nascita.cod_indirizzo " & vbCrLf
            StrSql &= "         LEFT OUTER JOIN ISTAT ISTAT_Nascita ON Indirizzo_Nascita.pro_cod_istat = ISTAT_Nascita.PROV AND Indirizzo_Nascita.com_cod_istat = ISTAT_Nascita.COM " & vbCrLf
            StrSql &= " " & vbCrLf
            StrSql &= "         LEFT OUTER JOIN GerarchiaImprese ON Impresa.PIVA = GerarchiaImprese.Figlio " & vbCrLf
            StrSql &= "         LEFT OUTER JOIN Imprese CooperativaPadre ON GerarchiaImprese.Padre = CooperativaPadre.PIVA " & vbCrLf
            StrSql &= "         LEFT OUTER JOIN ImpresexIndirizzi ImpresexIndirizzo_Coop ON  ImpresexIndirizzo_Coop.PIVA = CooperativaPadre.PIVA " & vbCrLf
            StrSql &= "         LEFT OUTER JOIN Indirizzi Indirizzo_Coop ON ImpresexIndirizzo_Coop.cod_indirizzo = Indirizzo_Coop.cod_indirizzo " & vbCrLf
            StrSql &= "         LEFT OUTER JOIN ISTAT ISTAT_Coop ON ISTAT_Coop.PROV = Indirizzo_Coop.pro_cod_istat AND ISTAT_Coop.COM = Indirizzo_Coop.com_cod_istat " & vbCrLf
            StrSql &= " " & vbCrLf
            StrSql &= "         LEFT OUTER JOIN Imprese_Codici AS Imprese_CodiciCoop ON CooperativaPadre.PIVA = Imprese_CodiciCoop.PIVA AND id_cod = " & enum_CodiciAnagrafe.Tecnico & " " & vbCrLf
            StrSql &= "         LEFT OUTER JOIN Contatti AS Contatti_Coop ON Contatti_Coop.Cod_Contatto = Imprese_CodiciCoop.val_cod " & vbCrLf
            StrSql &= "         LEFT OUTER JOIN Imprese_Codici AS Imprese_CodiciBP ON Imprese_CodiciBP.PIVA = Impresa.PIVA AND Imprese_CodiciBP.id_cod = " & enum_CodiciAnagrafe.Codice_Socio & " " & vbCrLf
            StrSql &= " "

            StrSql &= " WHERE   1 = 1  "

            If xFiltroAggiuntivo <> "" Then
                StrSql &= " AND " & xFiltroAggiuntivo
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSql &= " AND   Impresa.Inviato >=0 "
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSql &= " AND   Impresa.Inviato =-1 "
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSql &= " ORDER BY " & xOrderBy
            Else
                '2019/01/28: aggiunto ordinamento per visualizzare l'ultimo legale rappresentante attivo
                StrSql &= " ORDER BY DataInizio_Legale DESC "
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSql, NomeRoutine)
            '--------------------------------------------------------------------------

            ' Eliminazione tabella temporanea
            TempChiaviMassivo.EliminaTabellaTemp_FiltroPiva(NomeRoutine, objParametri)

            'commit transazione
            ConnessioniTransazioni.ChiudiTransazione(1, objParametri)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            'rollback transazione
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        Finally
            ConnessioniTransazioni.ChiudiConnessione(objParametri)
        End Try

        Return DT

    End Function

    '##################################################################################
    Public Function VerificaEsistenza_PivaGIAS(ByVal Piva As String,
                                               ByRef objParametri As AgronicaCoreParametri
                                               ) As Boolean

        Dim DT As DataTable

        DT = Leggi_2(Piva, "",
                     enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                     "", "",
                     objParametri)

        If Not IsNothing(DT) AndAlso DT.Rows.Count <> 0 Then
            Return True
        Else
            Return False
        End If

    End Function

    '##################################################################################
    Public Sub VerificaEsistenza_PivaGIAS(ByVal Piva As String,
                                            ByVal Chiave_Cliente As Integer,
                                            ByRef Impresa_Presente As Boolean,
                                            ByRef Data_Modifica As Date,
                                            ByRef Socio As String,
                                            ByRef objparametriserver As AgronicaCoreParametri)


        Dim nomeroutine As String = "VerificaEsistenza_PivaGIAS"

        Dim DT As DataTable

        Try


            Dim StrSQL As String
            StrSQL = ""
            StrSQL = StrSQL & " SELECT  Imprese.*, Imprese_Codici.id_cod, Imprese_Codici.val_cod "
            StrSQL = StrSQL & " FROM    Imprese_Codici RIGHT OUTER JOIN  Imprese ON Imprese_Codici.PIVA = Imprese.PIVA "
            StrSQL = StrSQL & " WHERE   Imprese.Piva = '" & Agro_SQL_SaveText(Piva) & "' "
            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objparametriserver, StrSQL.ToString, nomeroutine)
            '--------------------------------------------------------------------------

            If Not IsNothing(DT) Then
                If DT.Rows.Count <> 0 Then

                    Impresa_Presente = True
                    Data_Modifica = CDate(DT.Rows(0).Item("data_modifica"))

                    Dim drs As DataRow() = DT.Select(" id_cod = " & Chiave_Cliente)

                    If drs.Length > 0 Then
                        Socio = drs(0).Item("val_cod")
                    End If
                    Exit Sub
                Else


                    Impresa_Presente = False
                    Data_Modifica = Nothing
                    Socio = ""
                    Exit Sub


                End If

            Else

                Impresa_Presente = False
                Data_Modifica = Nothing
                Socio = ""

            End If

        Catch ex As Exception
            Dim MessaggioErrore = ex.Message
            Scrivi_LOG(objparametriserver, nomeroutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & nomeroutine & "] : " & MessaggioErrore)
        End Try


    End Sub

    Public Function Leggi_x_anagrafica(ByVal Piva As String,
                                       ByVal xFiltroAggiuntivo As String,
                                       ByVal xOrderBy As String,
                                       ByRef objParametri As AgronicaCoreParametri
                                       ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Read.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT distinct i.piva as chiave, i.piva, ISNULL(i.partitaIvaReale, i.piva) AS partitaIvaReale, i.rag_soc, i.Data_Creazione, i.Data_Modifica, i.Validita_Inizio, i.Validita_Fine, ")
            StrSQL.AppendLine(" ind_des, com_des, pro_cod, Cap, Stato, ind_des + ' ' + Cap + ' ' + com_des + ' ' + pro_cod + ' ' + Stato AS Indirizzo, ii.Tipo_Indirizzo, ")
            StrSQL.AppendLine(" ISNULL ((SELECT TOP 1 val_cod FROM Imprese_Codici WHERE Imprese_Codici.id_cod = " & enum_CodiciAnagrafe.CodiceCUAA & " AND Imprese_Codici.piva=i.piva), ' ') AS Codice_Cuaa, ")
            StrSQL.AppendLine(" ISNULL ((SELECT TOP 1 val_cod FROM Imprese_Codici WHERE Imprese_Codici.id_cod = " & enum_CodiciAnagrafe.Codice_Socio & " AND Imprese_Codici.piva=i.piva), ' ') AS Codice_Socio, ")
            StrSQL.AppendLine(" ISNULL ((SELECT TOP 1 val_cod FROM Imprese_Codici WHERE Imprese_Codici.id_cod = " & enum_CodiciAnagrafe.Contratto_Produzione & " AND Imprese_Codici.piva=i.piva), ' ') AS Contratto_Produzione, ")
            StrSQL.AppendLine(" ISNULL ((SELECT TOP 1 Contatti.Cognome + ' ' + Contatti.Nome FROM Imprese_Codici INNER JOIN Contatti ON Imprese_Codici.val_cod = Contatti.Cod_Contatto WHERE Imprese_Codici.id_cod = " & enum_CodiciAnagrafe.Tecnico & " AND Imprese_Codici.piva=i.piva), ' ') AS Tecnico_Referente, ")
            StrSQL.AppendLine(" ISNULL ((SELECT TOP 1 Codice_Fiscale FROM Contatti WHERE i.PIVA = Contatti.Cod_Contatto), ' ') AS Codice_Fiscale, ")
            StrSQL.AppendLine(" ISNULL ((SELECT TOP 1 [User] FROM Utenti WHERE CODICE_FISCALE = i.Username_Creazione), ' ') AS Utente_Creazione, ")
            StrSQL.AppendLine(" ISNULL ((SELECT TOP 1 [User] FROM Utenti WHERE CODICE_FISCALE = i.Username_Modifica), ' ') AS Utente_Modifica ")

            StrSQL.AppendLine(" FROM Imprese  i ")

            StrSQL.AppendLine(" LEFT JOIN ImpresexIndirizzi ii ON ii.piva = i.piva ")
            StrSQL.AppendLine(" LEFT JOIN Indirizzi ind ON ind.cod_indirizzo = ii.cod_indirizzo  ")

            StrSQL.AppendLine(" WHERE (i.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
            StrSQL.AppendLine(" AND   (i.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND (i.Piva = '" & Agro_SQL_SaveText(Piva).Trim & "') ")
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   i.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   i.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY i.Rag_Soc ASC ")
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

    Public Function Leggi_x_anagraficaVisibilita_Utente(
                             ByVal Piva As String,
                                 ByVal xFiltroAggiuntivo As String,
                                 ByVal xOrderBy As String,
                                 ByRef objParametri_Server As AgronicaCoreParametri,
                                 ByRef objParametri_Utenti As AgronicaCoreParametri,
                                 ByVal Filtro_Visibilita_Utente As Boolean
                                 ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Read.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT distinct i.piva as chiave, i.piva, ISNULL(i.partitaIvaReale, '') AS partitaIvaReale, i.rag_soc, i.Data_Creazione, i.Data_Modifica, i.Validita_Inizio, i.Validita_Fine, ")
            StrSQL.AppendLine(" ind_des, frz_des, com_des, pro_cod, ind.Cap,  ")
            StrSQL.AppendLine(" CASE Stato WHEN '' THEN 'IT' WHEN 'ITALIA' THEN 'IT' ELSE stato END as Stato_Cod,  ")
            StrSQL.AppendLine(" ISNULL(ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166.Descrizione, 'Italia') as Stato, ")
            StrSQL.AppendLine(" ind_des + ' ' + ind.Cap + ' ' + com_des + ' ' + pro_cod + ' ' + Stato AS Indirizzo, ii.Tipo_Indirizzo,    ")
            StrSQL.AppendLine(" ISNULL ((SELECT TOP 1 val_cod FROM Imprese_Codici WHERE Imprese_Codici.id_cod = " & enum_CodiciAnagrafe.CodiceCUAA & " AND Imprese_Codici.piva=i.piva), ' ') AS Codice_Cuaa, ")
            StrSQL.AppendLine(" ISNULL ((SELECT TOP 1 val_cod FROM Imprese_Codici WHERE Imprese_Codici.id_cod = " & enum_CodiciAnagrafe.Codice_Socio & " AND Imprese_Codici.piva=i.piva), ' ') AS Codice_Socio, ")
            StrSQL.AppendLine(" ISNULL ((SELECT TOP 1 val_cod FROM Imprese_Codici WHERE Imprese_Codici.id_cod = " & enum_CodiciAnagrafe.Contratto_Produzione & " AND Imprese_Codici.piva=i.piva), ' ') AS Contratto_Produzione, ")
            StrSQL.AppendLine(" ISNULL ((SELECT TOP 1 Contatti.Cognome + ' ' + Contatti.Nome FROM Imprese_Codici INNER JOIN Contatti ON Imprese_Codici.val_cod = Contatti.Cod_Contatto WHERE Imprese_Codici.id_cod = " & enum_CodiciAnagrafe.Tecnico & " AND Imprese_Codici.piva=i.piva), ' ') AS Tecnico_Referente, ")
            StrSQL.AppendLine(" ISNULL ((SELECT TOP 1 Codice_Fiscale FROM Contatti WHERE i.PIVA = Contatti.Cod_Contatto), ' ') AS Codice_Fiscale, ")
            StrSQL.AppendLine(" ISNULL ((SELECT TOP 1 [User] FROM Utenti WHERE CODICE_FISCALE = i.Username_Creazione), ' ') AS Utente_Creazione, ")
            StrSQL.AppendLine(" ISNULL ((SELECT TOP 1 [User] FROM Utenti WHERE CODICE_FISCALE = i.Username_Modifica), ' ') AS Utente_Modifica, ")

            StrSQL.AppendLine(" ISTAT.PROV as Pro_Cod_Istat, ")
            StrSQL.AppendLine(" Istat.COMUNI_PROV as Prov, ")
            StrSQL.AppendLine(" ISTAT.COM as Com_Cod_Istat, ")
            StrSQL.AppendLine(" ISTAT.Localita as Com, ")
            StrSQL.AppendLine(" i.Validita_Inizio, i.Validita_Fine ")

            StrSQL.AppendLine(" FROM Imprese  i ")

            StrSQL.AppendLine(" JOIN ImpresexIndirizzi ii ON ii.piva = i.piva ")
            StrSQL.AppendLine(" JOIN Indirizzi ind ON ind.cod_indirizzo = ii.cod_indirizzo  ")
            StrSQL.AppendLine(" JOIN ISTAT ON ISTAT.PROV = ind.pro_cod_istat AND ISTAT.COM = ind.com_cod_istat  ")
            StrSQL.AppendLine(" LEFT JOIN ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166 ON ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166.Codice = ind.Stato ")

            If Filtro_Visibilita_Utente Then
                StrSQL.AppendLine(" LEFT JOIN Utenti_Visibilita_Appoggio (NOLOCK) On i.Piva = Utenti_Visibilita_Appoggio.Piva AND Utenti_Visibilita_Appoggio.Entita_Cod=1 AND Utenti_Visibilita_Appoggio.Username = " & Agro_SQL_SaveText_NULL(objParametri_Server.UtenteUsername) & " ")
            End If

            StrSQL.AppendLine(" WHERE (i.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine) & ") ")
            StrSQL.AppendLine(" AND   (i.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleInizio) & ") ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND (i.Piva = '" & Agro_SQL_SaveText(Piva).Trim & "') ")
            End If

            If Filtro_Visibilita_Utente Then
                StrSQL.AppendLine(" AND Utenti_Visibilita_Appoggio.Username = " & Agro_SQL_SaveText_NULL(objParametri_Server.UtenteUsername) & "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Server))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri_Server.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   i.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   i.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Server))
            Else
                StrSQL.AppendLine(" ORDER BY i.Rag_Soc ASC ")
            End If



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

    Public Function Leggi_x_anagraficaVisibilita_Utente_NG(
    ByVal Piva As String,
    ByVal xFiltroAggiuntivo As String,
    ByVal xOrderBy As String,
    ByRef objParametri_Server As AgronicaCoreParametri,
    ByRef objParametri_Utenti As AgronicaCoreParametri,
    ByVal Filtro_Visibilita_Utente As Boolean,
    Optional ByVal leggiSuperfici As Boolean = False
) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Read.Leggi()"
        Dim StrSQL As New System.Text.StringBuilder()
        Dim DT As DataTable

        Try
            ' 1) Isolation level
            StrSQL.AppendLine("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")

            ' 2) CTEs
            StrSQL.AppendLine("WITH sup_catastali AS (")
            StrSQL.AppendLine("    SELECT IX.Piva, SUM(IX.Sup_Condotta) AS Sup_Catastale")
            StrSQL.AppendLine("    FROM ImpreseXParticelle AS IX WITH (NOLOCK)")
            StrSQL.AppendLine("    WHERE IX.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine))
            StrSQL.AppendLine("        AND IX.Validita_Fine   >= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine))
            StrSQL.AppendLine("        AND IX.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            StrSQL.AppendLine("    GROUP BY IX.Piva")
            StrSQL.AppendLine("),")
            StrSQL.AppendLine("contattiAziende AS (")
            StrSQL.AppendLine("    SELECT")
            StrSQL.AppendLine("        IP.Piva AS Piva_Proprietario")
            StrSQL.AppendLine("        , IP.Rag_Soc AS Rag_Soc_Proprietario")
            StrSQL.AppendLine("        , RU.Cod_Contatto")
            StrSQL.AppendLine("        , C.sa_cod AS Tipo")
            StrSQL.AppendLine($"       , CASE WHEN C.Sa_Cod = 0 THEN '{Gias.Privato}' ELSE '{Gias.Pubblico}' END AS TipoDes")
            StrSQL.AppendLine("        , STRING_AGG(RC.Rapporto_Des, ', ') AS Rapporti_Des")
            StrSQL.AppendLine("        , STRING_AGG(RC.Cod_Rapporto, ', ') AS Rapporti_Codice")
            StrSQL.AppendLine("    FROM Risorse_Umane RU WITH (NOLOCK)")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("    JOIN Contatti C WITH (NOLOCK) ON RU.Piva = C.Piva")
            StrSQL.AppendLine("        AND RU.Cod_Contatto = C.Cod_Contatto")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("    JOIN Rapporti_Contabili AS RC WITH (NOLOCK) ON RU.Cod_Rapporto = RC.Cod_Rapporto")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("    JOIN Imprese IP WITH (NOLOCK) ON C.Piva = IP.Piva")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("    GROUP BY RU.Cod_Contatto, C.sa_cod, IP.Piva, IP.Rag_Soc")
            StrSQL.AppendLine("),")
            StrSQL.AppendLine("sup_appezzamenti AS (")
            StrSQL.AppendLine("    SELECT")
            StrSQL.AppendLine("        AE.PIVA")
            StrSQL.AppendLine("        , SUM(CASE WHEN AC.val_cod = '1' OR AC.val_cod IS NULL THEN AE.sup_app ELSE 0 END) AS SAU_Convenzionale")
            StrSQL.AppendLine("        , SUM(CASE WHEN AC.val_cod = '2' THEN AE.sup_app ELSE 0 END) AS SAU_Conversione")
            StrSQL.AppendLine("        , SUM(CASE WHEN AC.val_cod = '3' THEN AE.sup_app ELSE 0 END) AS SAU_Biologico")
            StrSQL.AppendLine("        , SUM(AE.sup_app) AS SAU_Totale")
            StrSQL.AppendLine("    FROM Appezzamento             AS AE WITH (NOLOCK)")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("        LEFT JOIN Appezzamento_Codici AS AC WITH (NOLOCK) ON AE.PIVA = AC.PIVA")
            StrSQL.AppendLine("            AND AE.SA_COD = AC.sa_cod")
            StrSQL.AppendLine("            AND AE.APPEZZA = AC.appezza")
            StrSQL.AppendLine("            AND AC.id_cod = 1018")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("     WHERE AE.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine))
            StrSQL.AppendLine("         AND AE.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine))
            StrSQL.AppendLine("         AND AE.PIVA = '" & Agro_SQL_SaveText(Piva) & "'")
            StrSQL.AppendLine("    GROUP BY AE.PIVA")
            StrSQL.AppendLine(")")

            ' 3) Main SELECT
            StrSQL.AppendLine("SELECT")
            StrSQL.AppendLine("    COALESCE(CA.Cod_Contatto, '') as Cod_Contatto")
            StrSQL.AppendLine("    , COALESCE(CA.Rapporti_Des, '') as Rapporti_Des")
            StrSQL.AppendLine("    , COALESCE(CA.Rapporti_Codice, '') as Rapporti_Codice")
            StrSQL.AppendLine("    , COALESCE(CA.Tipo, 0) as Tipo")
            StrSQL.AppendLine("    , COALESCE(CA.TipoDes, '') as TipoDes")
            StrSQL.AppendLine("    , COALESCE(CA.Piva_Proprietario, '') as Piva_Proprietario")
            StrSQL.AppendLine("    , COALESCE(CA.Rag_Soc_Proprietario, '') as Rag_Soc_Proprietario")
            StrSQL.AppendLine("    , ISNULL(Padre.Piva,'') + '_' + I.Piva AS chiave")
            StrSQL.AppendLine("    , I.Piva")
            StrSQL.AppendLine("    , I.Rag_Soc")
            StrSQL.AppendLine("    , I.Data_Creazione")
            StrSQL.AppendLine("    , I.Data_Modifica")
            StrSQL.AppendLine("    , I.Validita_Inizio")
            StrSQL.AppendLine("    , I.Validita_Fine")
            StrSQL.AppendLine("    , ind.ind_des")
            StrSQL.AppendLine("    , ind.frz_des")
            StrSQL.AppendLine("    , ind.com_des")
            StrSQL.AppendLine("    , ind.pro_cod")
            StrSQL.AppendLine("    , ind.Cap")
            StrSQL.AppendLine("    , CASE ind.Stato WHEN '' THEN 'IT' WHEN 'ITALIA' THEN 'IT' ELSE ind.Stato END AS Stato_Cod")
            StrSQL.AppendLine("    , ISNULL(t3.Descrizione,'Italia') AS Stato")
            StrSQL.AppendLine("    , ind.ind_des + ' ' + ind.Cap + ' ' + ind.com_des + ' ' + ind.pro_cod + ' ' + ISNULL(t3.Descrizione,'Italia') AS Indirizzo")
            StrSQL.AppendLine("    , ii.Tipo_Indirizzo")
            StrSQL.AppendLine("    , ISNULL(cuaa.val_cod,'') AS Codice_Cuaa")
            StrSQL.AppendLine("    , ISNULL(socio.val_cod,'') AS Codice_Socio")
            StrSQL.AppendLine("    , ISNULL(contrattoProduzione.val_cod,'') AS Contratto_Produzione")
            StrSQL.AppendLine("    , ISNULL(creazione.[USER],'') AS Utente_Creazione")
            StrSQL.AppendLine("    , ISNULL(modifica.[USER],'')  AS Utente_Modifica")
            StrSQL.AppendLine("    , lp.Provincia")
            StrSQL.AppendLine("    , ist.PROV AS Pro_Cod_Istat")
            StrSQL.AppendLine("    , ist.COMUNI_PROV AS Prov")
            StrSQL.AppendLine("    , ist.COM AS Com_Cod_Istat")
            StrSQL.AppendLine("    , ist.Localita AS Com")
            StrSQL.AppendLine("    , Padre.Piva AS Piva_Padre")
            StrSQL.AppendLine("    , IIF(ISNULL(cuaaPadre.val_cod,'') = '', Padre.Rag_Soc, Padre.Rag_Soc + ' (' + ISNULL(cuaaPadre.val_cod,'') + ')') AS Rag_Soc_Padre")
            StrSQL.AppendLine("    , gi.LibroSoci_Codice AS codice_iscrizione_libro_soci")
            StrSQL.AppendLine("    , gi.LibroSoci_DataIscrizione AS data_iscrizione_libro_soci")
            StrSQL.AppendLine("    , COUNT(*) OVER (PARTITION BY I.Rag_Soc) AS Num_Padri")
            StrSQL.AppendLine("    , COALESCE(gr.GruppoRaccolta_Cod,0) AS GruppoRaccolta_Cod")
            StrSQL.AppendLine("    , COALESCE(gr.GruppoRaccolta_Des,'') AS GruppoRaccolta_Des")
            StrSQL.AppendLine("    , CASE WHEN I.Validita_Inizio < GETDATE() AND I.Validita_Fine > GETDATE() THEN 1 ELSE 0 END AS Attivo")
            StrSQL.AppendLine("    , CAST(ISNULL(sup_catastali.Sup_Catastale,0) AS float) AS Superficie_Catastale")
            StrSQL.AppendLine("    , CAST(ISNULL(sup_appezzamenti.SAU_Convenzionale,0) AS float) AS Superficie_Convenzionale")
            StrSQL.AppendLine("    , CAST(ISNULL(sup_appezzamenti.SAU_Conversione,0) AS float) AS Superficie_Conversione")
            StrSQL.AppendLine("    , CAST(ISNULL(sup_appezzamenti.SAU_Biologico,0) AS float) AS Superficie_Biologico")
            StrSQL.AppendLine("    , CAST(ISNULL(sup_appezzamenti.SAU_Totale,0) AS float) AS Superficie_Total")
            StrSQL.AppendLine("    , CASE WHEN ISNULL(I.partitaIvaReale, '') = '' THEN I.Piva ELSE I.partitaIvaReale END AS partitaIvaReale")
            StrSQL.AppendLine("FROM Imprese I WITH (NOLOCK)")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("    LEFT JOIN sup_catastali ON I.Piva = sup_catastali.Piva")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("    LEFT JOIN contattiAziende CA ON CA.Cod_Contatto = I.Piva")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("    LEFT JOIN sup_appezzamenti ON I.Piva = sup_appezzamenti.PIVA")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("    LEFT JOIN GerarchiaImprese gi WITH (NOLOCK) ON I.Piva = gi.Figlio")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("    LEFT JOIN Imprese Padre WITH (NOLOCK) ON gi.Padre = Padre.Piva")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("    LEFT JOIN Imprese_Codici cuaa WITH (NOLOCK) ON I.Piva = cuaa.PIVA AND cuaa.id_cod = 1010")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("    LEFT JOIN Imprese_Codici socio WITH (NOLOCK) ON I.Piva = socio.PIVA AND socio.id_cod = 1033")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("    LEFT JOIN Imprese_Codici contrattoProduzione WITH (NOLOCK) ON I.Piva = contrattoProduzione.PIVA AND contrattoProduzione.id_cod = 1324")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("    LEFT JOIN Imprese_Codici cuaaPadre WITH (NOLOCK) ON Padre.Piva = cuaaPadre.PIVA AND cuaaPadre.id_cod = 1010")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("    LEFT JOIN Utenti creazione WITH (NOLOCK) ON I.Username_Creazione = creazione.CODICE_FISCALE")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("    LEFT JOIN Utenti modifica WITH (NOLOCK) ON I.Username_Modifica = modifica.CODICE_FISCALE")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("    JOIN ImpresexIndirizzi ii WITH (NOLOCK) ON ii.Piva = I.Piva")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("    JOIN Indirizzi ind WITH (NOLOCK) ON ind.Cod_Indirizzo = ii.Cod_Indirizzo")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("    JOIN ISTAT ist WITH (NOLOCK) ON ist.PROV = ind.pro_cod_istat AND ist.COM = ind.com_cod_istat")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("    LEFT JOIN Lista_Province lp WITH (NOLOCK) ON ind.pro_cod_istat = lp.PROV")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("    LEFT JOIN ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166 AS t3 WITH (NOLOCK) ON t3.Codice = ind.Stato")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("    LEFT JOIN Gruppi_Raccolta gr WITH (NOLOCK) ON I.GruppoRaccolta_Cod = gr.GruppoRaccolta_Cod")
            StrSQL.AppendLine("")

            If Filtro_Visibilita_Utente Then
                StrSQL.AppendLine("    JOIN Utenti_Visibilita_Appoggio AS UVA WITH (NOLOCK) ON I.Piva = UVA.Piva")
                StrSQL.AppendLine("        AND UVA.Entita_Cod = 1")
                StrSQL.AppendLine("        AND UVA.Username = " & Agro_SQL_SaveText_NULL(objParametri_Server.UtenteUsername))
            End If

            StrSQL.AppendLine("")
            StrSQL.AppendLine("WHERE I.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine))
            StrSQL.AppendLine("    AND I.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine))
            If Piva <> ""Then
                StrSQL.AppendLine("    AND I.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If
            StrSQL.AppendLine("    AND I.Inviato >= 0")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine("    AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Server))
            End If

            Select Case objParametri_Server.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine("    AND I.Inviato >= 0")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine("    AND I.Inviato = -1")
                Case enumVisibilita.Visibilita_Tutti
                    ' no extra filter
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            ' 4) ORDER BY
            If xOrderBy <> "" Then
                StrSQL.AppendLine("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Server))
            Else
                StrSQL.AppendLine("ORDER BY I.Rag_Soc ASC")
            End If

            ' 5) Execute
            DT = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString(), NomeRoutine)

            'For Each col As DataColumn In DT.Columns
            '    col.ColumnName = col.ColumnName.ToLowerInvariant()
            'Next

        Catch ex As Exception
            Scrivi_LOG(objParametri_Server, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try

        Return DT
    End Function


    Public Function Leggi_Max_DataModifica(
                             ByVal Piva As String,
                             ByRef objParametri_Server As AgronicaCoreParametri,
                             ByRef objParametri_Utenti As AgronicaCoreParametri,
                             ByVal Filtro_Visibilita_Utente As Boolean) As DateTime

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Read.Leggi_Max_DataModifica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Data_Modifica = AGRODATAINIZIO
        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT MAX(i.Data_Modifica) ")
            StrSQL.AppendLine(" FROM Imprese  i ")

            If Filtro_Visibilita_Utente Then
                StrSQL.AppendLine(" LEFT JOIN Utenti_Visibilita_Appoggio (NOLOCK) On i.Piva = Utenti_Visibilita_Appoggio.Piva AND Utenti_Visibilita_Appoggio.Entita_Cod=1 AND Utenti_Visibilita_Appoggio.Username = " & Agro_SQL_SaveText_NULL(objParametri_Server.UtenteUsername) & " ")
            End If

            StrSQL.AppendLine(" WHERE (i.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine) & ") ")
            StrSQL.AppendLine(" AND   (i.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleInizio) & ") ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND (i.Piva = '" & Agro_SQL_SaveText(Piva).Trim & "') ")
            End If

            If Filtro_Visibilita_Utente Then
                StrSQL.AppendLine(" AND Utenti_Visibilita_Appoggio.Username = " & Agro_SQL_SaveText_NULL(objParametri_Server.UtenteUsername) & "  ")
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri_Server.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   i.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   i.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select



            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT IsNot Nothing AndAlso DT.Rows.Count > 0 Then
                Data_Modifica = DT.Rows(0)(0)
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            Data_Modifica = AGRODATAINIZIO
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Data_Modifica

    End Function

    Public Function Leggi_Certificato(ByVal Piva As String,
                                      ByRef objParametri_Server As AgronicaCoreParametri
                                      ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Read.Leggi_Certificato()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT val_cod ")
            StrSQL.AppendLine(" FROM Imprese_Codici ")
            StrSQL.AppendLine(" WHERE (Piva = '" & Agro_SQL_SaveText(Piva).Trim & "') ")
            StrSQL.AppendLine(" AND id_cod = '" & enum_CodiciAnagrafe.Codice_Certificazione & "' ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function Leggi_Certificazioni_Totali(ByRef objParametri_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Read.Leggi_Certificazioni_Totali()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT InfoAgg_Cod ")
            StrSQL.AppendLine(" FROM CAC_Codifica_InfoAggiuntive ")
            StrSQL.AppendLine(" WHERE Argomento_Des = 'Residuo' ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function Leggi(ByVal Piva As String,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Read.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  PIVA, rag_soc, Validita_Inizio, Validita_Fine, TipoImpresaGerarchia ")
                    StrSQL.Append(" FROM    Imprese ")
                    StrSQL.Append(" WHERE   (Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ")  ")
                    StrSQL.Append(" AND     (Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")

                    If Piva <> "" Then
                        StrSQL.Append(" AND     (Piva = '" & Agro_SQL_SaveText(Piva).Trim & "') ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Rag_Soc ASC ")
                    End If



                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  * ")
                    StrSQL.Append(" FROM    Imprese ")
                    StrSQL.Append(" WHERE   (Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ")  ")
                    StrSQL.Append(" AND     (Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")

                    If Piva <> "" Then
                        StrSQL.Append(" AND     (Piva = '" & Agro_SQL_SaveText(Piva).Trim & "') ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Rag_Soc ASC ")
                    End If


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  * ,(select [User] from utenti where CODICE_FISCALE = imprese.Username_Modifica ) as utente_modifica")
                    StrSQL.Append(" FROM    Imprese ")
                    StrSQL.Append(" WHERE   (Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ")  ")
                    StrSQL.Append(" AND     (Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")

                    If Piva <> "" Then
                        StrSQL.Append(" AND     (Piva = '" & Agro_SQL_SaveText(Piva).Trim & "') ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Rag_Soc ASC ")
                    End If


                Case enumSelezioneVariabile.Selezione_JoinCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  * ")
                    StrSQL.Append(" , isnull( (select top 1 val_cod  ")
                    StrSQL.Append("         from imprese_codici ii ")
                    StrSQL.Append("         where Imprese.piva= ii.piva ")
                    StrSQL.Append("         and id_cod= " & CStr(CA_COD_SOCIO) & ") ,'') as cod_socio ")
                    StrSQL.Append(" FROM    Imprese ")

                    StrSQL.Append(" WHERE   (Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ")  ")
                    StrSQL.Append(" AND     (Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")

                    If Piva <> "" Then
                        StrSQL.Append(" AND     (Piva = '" & Agro_SQL_SaveText(Piva).Trim & "') ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Rag_Soc ASC ")
                    End If

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

    Public Function Leggi_PivaReale(ByVal Piva As String,
                                      ByRef objParametri_Server As AgronicaCoreParametri
                                      ) As String

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Read.Leggi_PivaReale()"

        Dim result As String = Piva
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Try

            StrSQL.Length = 0
            StrSQL.AppendLine("SELECT CASE ")
            StrSQL.AppendLine("  WHEN ISNULL(partitaIvaReale, '') = '' THEN PIVA ")
            StrSQL.AppendLine("  ELSE partitaIvaReale ")
            StrSQL.AppendLine("END PivaReale ")
            StrSQL.AppendLine("FROM Imprese ")
            StrSQL.AppendLine("WHERE PIVA = '" & Agro_SQL_SaveText(Piva).Trim & "' ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                result = DT.Rows(0).Field(Of String)("PivaReale")
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return result

    End Function

    Public Function Leggi_PivaPadreChiave(ByVal Piva As String,
                                      ByRef objParametri_Server As AgronicaCoreParametri
                                      ) As String

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Read.Leggi_PivaPadreReale()"

        Dim result As String = Piva
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Try

            StrSQL.Length = 0
            StrSQL.AppendLine("SELECT PIVA ")
            StrSQL.AppendLine("FROM Imprese ")
            StrSQL.AppendLine("WHERE PIVA = '" & Agro_SQL_SaveText(Piva).Trim & "' ")
            StrSQL.AppendLine("OR ISNULL(partitaIvaReale, PIVA) = '" & Agro_SQL_SaveText(Piva).Trim & "' ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                result = DT.Rows(0).Field(Of String)("PIVA")
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return result

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Codice_Fiscale_Tecnico"></param>
    ''' <param name="Veg_Cod">impostare -1 per non cagare</param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function distinct_Piva_From_Codice_Fiscale_Tecnico(
                            ByVal Codice_Fiscale_Tecnico As String,
                            ByVal Veg_Cod As Integer,
                                ByRef objParametri As AgronicaCoreParametri
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Read.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------

            StrSQL.Length = 0
            StrSQL.Append(" select distinct piva  ")
            StrSQL.Append(" FROM    ( ")
            StrSQL.Append(" select distinct piva  from reg_impianti  ")
            StrSQL.Append(" where CODICE_FISCALE_TECNICO ='" & Agro_SQL_SaveText(Codice_Fiscale_Tecnico) & "' ")
            StrSQL.Append(" AND (Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ")  ")
            StrSQL.Append(" AND  (Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")

            If Veg_Cod <> -1 Then
                If Veg_Cod = 0 Then
                    StrSQL.Append(" AND  reg_impianti.cul_cod = 0 ")
                Else
                    StrSQL.Append(" AND reg_impianti.cul_cod in (select cul_cod from cultivar where veg_cod = " & Veg_Cod & " ) ")
                End If
            End If


            StrSQL.Append(" union ")

            StrSQL.Append(" select distinct piva from Programmazione_Entita ")
            StrSQL.Append(" where CODICE_FISCALE_TECNICO = '" & Agro_SQL_SaveText(Codice_Fiscale_Tecnico) & "' ")
            StrSQL.Append(" AND (Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ")  ")
            StrSQL.Append(" AND  (Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")

            If Veg_Cod <> -1 Then
                If Veg_Cod = 0 Then
                    StrSQL.Append(" AND  Programmazione_Entita.cul_cod = 0 ")
                Else
                    StrSQL.Append(" AND Programmazione_Entita.cul_cod in (select cul_cod from cultivar where veg_cod = " & Veg_Cod & " ) ")
                End If
            End If

            StrSQL.Append(" ) a")
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





    Public Function Leggi_by_Cod_Socio_Rag_soc_Stabilimento(
                            ByVal Piva As String,
                            ByVal Rag_soc As String,
                            ByVal Codice As String,
                            ByVal Stabilimento As String,
                            ByVal Emun_Codice As Integer,
                            ByVal join_UtentiVisibilitaAppoggio As Boolean,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreParametri
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Read.Leggi_by_Cod_Socio_Rag_soc_Stabilimento()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" select val_cod , imprese.piva, rag_soc  ")


            StrSQL.AppendLine(" FROM    Imprese ")

            If join_UtentiVisibilitaAppoggio Then
                StrSQL.AppendLine(" INNER JOIN Utenti_Visibilita_Appoggio app (NOLOCK) ")
                StrSQL.AppendLine(" on app.piva = imprese.piva ")
                StrSQL.AppendLine(" and app.entita_cod = 1 ")
                StrSQL.AppendLine(" and app.username = '" & objParametri.UtenteUsername & "' ")
            End If


            StrSQL.AppendLine(" left join imprese_codici ")
            StrSQL.AppendLine(" ON imprese_codici.piva = imprese.piva ")
            StrSQL.AppendLine(" AND     id_cod = " & Emun_Codice & " ")

            StrSQL.AppendLine(" WHERE   (imprese.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ")  ")
            StrSQL.AppendLine(" AND     (imprese.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND     (Imprese.Piva = '" & Agro_SQL_SaveText(Piva).Trim & "') ")
            End If
            If Rag_soc <> "" Then
                StrSQL.AppendLine(" AND     (Rag_soc like '%" & Agro_SQL_SaveText(Rag_soc).Trim & "%') ")
            End If
            If Codice <> "" Then
                StrSQL.AppendLine(" AND     val_cod like '%" & Agro_SQL_SaveText(Codice).Trim & "%' ")
            End If

            If Stabilimento <> "" Then
                StrSQL.AppendLine(" AND (select val_cod from Imprese_Codici i where i.piva = imprese.piva and id_cod = 1163) = '" & Agro_SQL_SaveText(Stabilimento) & "' ")
            End If


            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   imprese.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   imprese.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Rag_Soc ASC ")
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
    Public Function Recupera_Dati_CentroAziendale(ByVal Piva As String,
                                                  ByVal Sa_Cod As Integer,
                                                  ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                                  ByVal xFiltroAggiuntivo As String,
                                                  ByVal xOrderBy As String,
                                                  ByRef objParametri As AgronicaCoreParametri
                                                  ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Read.Recupera_Dati_CentroAziendale()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '----- Genero la query SQL
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT Imprese.rag_soc, Centri_Aziendali.sa_nome,  ")
                    StrSQL.Append(" Centri_Aziendali.Validita_Inizio, Centri_Aziendali.Validita_Fine ")

                    StrSQL.Append(" FROM  Imprese INNER JOIN ")
                    StrSQL.Append(" Centri_Aziendali ON Imprese.PIVA = Centri_Aziendali.PIVA  ")

                    StrSQL.Append(" WHERE Centri_Aziendali.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    StrSQL.Append(" AND Centri_Aziendali.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Imprese.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Imprese.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

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


    Public Function Leggi_2(ByVal Piva As String,
                            ByVal strFiltro As String,
                            ByVal xSelezioneVariabile As enumSelezioneVariabile,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreParametri
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Read.Leggi_2()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    '---------------------------------------------
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  * ")
                    StrSQL.Append(" FROM    Imprese ")
                    StrSQL.Append(" WHERE   (Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ")  ")
                    StrSQL.Append(" AND     (Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")

                    If Piva <> "" Then
                        StrSQL.Append(" AND     (Piva = '" & Agro_SQL_SaveText(Piva).Trim & "') ")
                    End If

                    If strFiltro <> "" Then
                        StrSQL.Append(strFiltro)
                    End If



                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Rag_Soc ASC ")
                    End If



                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    '---------------------------------------------
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  * ")
                    StrSQL.Append(" FROM    Imprese ")
                    StrSQL.Append(" WHERE   (Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ")  ")
                    StrSQL.Append(" AND     (Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")

                    If Piva <> "" Then
                        StrSQL.Append(" AND     (Piva = '" & Agro_SQL_SaveText(Piva).Trim & "') ")
                    End If

                    If strFiltro <> "" Then
                        StrSQL.Append(strFiltro)
                    End If



                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Rag_Soc ASC ")
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



    '##############################################################################################
    Public Function Leggi_3(ByVal Piva As String,
                            ByVal Flag_Indirizzi As Boolean,
                            ByVal TipoIndirizzo As Integer,
                            ByVal CodIndirizzo As Integer,
                            ByVal Flag_CUAA As Boolean,
                            ByVal Flag_COD_SOCIO As Boolean,
                            ByVal Flag_TITOLO_POSSESSO As Boolean,
                            ByVal Flag_TECNICO_RIF As Boolean,
                            ByVal Flag_COD_CLIENTE As Boolean,
                            ByVal Flag_COD_FORNITORE As Boolean,
                            ByVal Flag_COD_FORNITORE_2 As Boolean,
                            ByVal Flag_COD_LIBRO_SOCI As Boolean,
                            ByVal Flag_DATA_ISCRIZIONE_LIBRO_SOCI As Boolean,
                            ByVal Flag_PIVA_SUPERUSER_ORIGINE As Boolean,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreParametri
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Read.Leggi_3()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT  Imprese.*   ")

            If Flag_Indirizzi Then
                StrSQL.Append(" , ImpresexIndirizzi.Tipo_Indirizzo, Indirizzi.Cod_Indirizzo, Indirizzi.ind_des, Indirizzi.frz_des, Indirizzi.CAP, Indirizzi.com_cod_istat, Indirizzi.pro_cod_istat, Indirizzi.stato, Indirizzi.note, ")
                StrSQL.Append(" ISNULL(ISTAT.LOCALITA, '') AS com_des, ISNULL(ISTAT.COMUNI_PROV, '') AS pro_cod  ")
            End If

            If Flag_CUAA Then
                StrSQL.Append("             , ISNULL  ((  SELECT    TOP 1 val_cod  ")
                StrSQL.Append("                         FROM    Imprese_Codici AS Imprese_Codici_1  ")
                StrSQL.Append("                         WHERE   Imprese_Codici_1.piva = Imprese.piva AND Imprese_Codici_1.id_cod = " & CStr(CA_CUAA) & "), '') AS CUAA ")
            End If
            If Flag_COD_SOCIO Then
                StrSQL.Append("             , ISNULL  ((  SELECT    TOP 1 val_cod  ")
                StrSQL.Append("                         FROM    Imprese_Codici AS Imprese_Codici_2  ")
                StrSQL.Append("                         WHERE   Imprese_Codici_2.piva = Imprese.piva AND Imprese_Codici_2.id_cod = " & CStr(CA_COD_SOCIO) & "), '') AS Codice_Socio ")
            End If
            If Flag_TITOLO_POSSESSO Then
                StrSQL.Append("             , ISNULL  ((  SELECT    TOP 1 val_cod  ")
                StrSQL.Append("                         FROM    Imprese_Codici AS Imprese_Codici_3  ")
                StrSQL.Append("                         WHERE   Imprese_Codici_3.piva = Imprese.piva AND Imprese_Codici_3.id_cod = " & CStr(CA_TITOLO_POSSESSO) & "), '0') AS Titolo_Possesso ")
            End If
            If Flag_TECNICO_RIF Then
                StrSQL.Append("             , ISNULL  ((  SELECT    TOP 1 val_cod  ")
                StrSQL.Append("                         FROM    Imprese_Codici AS Imprese_Codici_4  ")
                StrSQL.Append("                         WHERE   Imprese_Codici_4.piva = Imprese.piva AND Imprese_Codici_4.id_cod = " & CStr(CA_TECNICO_RIF) & "), '') AS Tecnico_Riferimento ")
            End If
            If Flag_COD_CLIENTE Then
                StrSQL.Append("             , ISNULL  ((  SELECT    TOP 1 val_cod  ")
                StrSQL.Append("                         FROM    Imprese_Codici AS Imprese_Codici_5  ")
                StrSQL.Append("                         WHERE   Imprese_Codici_5.piva = Imprese.piva AND Imprese_Codici_5.id_cod = " & CStr(CA_COD_CLIENTE) & "), '') AS Codice_Cliente ")
            End If
            If Flag_COD_FORNITORE Then
                StrSQL.Append("             , ISNULL  ((  SELECT    TOP 1 val_cod  ")
                StrSQL.Append("                         FROM    Imprese_Codici AS Imprese_Codici_6  ")
                StrSQL.Append("                         WHERE   Imprese_Codici_6.piva = Imprese.piva AND Imprese_Codici_6.id_cod = " & CStr(CA_COD_FORNITORE) & "), '') AS Codice_Fornitore ")
            End If
            If Flag_COD_FORNITORE_2 Then
                StrSQL.Append("             , ISNULL  ((  SELECT    TOP 1 val_cod  ")
                StrSQL.Append("                         FROM    Imprese_Codici AS Imprese_Codici_7  ")
                StrSQL.Append("                         WHERE   Imprese_Codici_7.piva = Imprese.piva AND Imprese_Codici_7.id_cod = " & CStr(CA_COD_FORNITORE_2) & "), '') AS Codice_Fornitore_2 ")
            End If
            If Flag_COD_LIBRO_SOCI Then
                StrSQL.Append("             , ISNULL  ((  SELECT    TOP 1 val_cod  ")
                StrSQL.Append("                         FROM    Imprese_Codici AS Imprese_Codici_8  ")
                StrSQL.Append("                         WHERE   Imprese_Codici_8.piva = Imprese.piva AND Imprese_Codici_8.id_cod = " & CStr(CA_COD_LIBRO_SOCI) & "), '') AS codice_libro_soci ")
            End If
            If Flag_DATA_ISCRIZIONE_LIBRO_SOCI Then
                StrSQL.Append("             , ISNULL  ((  SELECT    TOP 1 val_cod  ")
                StrSQL.Append("                         FROM    Imprese_Codici AS Imprese_Codici_9  ")
                StrSQL.Append("                         WHERE   Imprese_Codici_9.piva = Imprese.piva AND Imprese_Codici_9.id_cod = " & CStr(CA_DATA_ISCRIZIONE_LIBRO_SOCI) & "), '01/01/1900') AS data_libro_soci ")
            End If
            If Flag_PIVA_SUPERUSER_ORIGINE Then
                StrSQL.Append("             , ISNULL  ((  SELECT    TOP 1 val_cod  ")
                StrSQL.Append("                         FROM    Imprese_Codici AS Imprese_Codici_10  ")
                StrSQL.Append("                         WHERE   Imprese_Codici_10.piva = Imprese.piva AND Imprese_Codici_10.id_cod = " & CStr(CA_PIVA_SUPERUSER_ORIGINE) & "), '') AS Piva_SuperUser_Origine ")
            End If

            StrSQL.Append(" FROM    Imprese ")
            StrSQL.Append(" INNER JOIN UtentiXImprese ON Imprese.PIVA = UtentiXImprese.PIVA ")

            If Flag_Indirizzi Then
                StrSQL.Append("  INNER JOIN  ImpresexIndirizzi ON Imprese.Piva = ImpresexIndirizzi.Piva ")
                StrSQL.Append("  INNER JOIN Indirizzi ON ImpresexIndirizzi.Cod_Indirizzo = Indirizzi.cod_indirizzo ")
                StrSQL.Append("  LEFT OUTER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM ")
            End If

            StrSQL.Append(" WHERE UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Piva <> "" Then
                StrSQL.Append(" AND     (Imprese.Piva = '" & Agro_SQL_SaveText(Piva) & "')   ")
            End If

            If Flag_Indirizzi Then
                If TipoIndirizzo <> 0 Then
                    StrSQL.Append(" AND     (ImpresexIndirizzi.Tipo_Indirizzo = " & Agro_SQL_SaveNum(TipoIndirizzo) & ")   ")
                End If
                If CodIndirizzo <> 0 Then
                    StrSQL.Append(" AND     (ImpresexIndirizzi.Cod_Indirizzo = " & Agro_SQL_SaveNum(CodIndirizzo) & ")   ")
                End If
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Imprese.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Imprese.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Rag_Soc ASC  ")
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



    Public Function Leggi_4(ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.Imprese_Read.Leggi_4()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Dim WHERE_Utenti_VA As String = "PivaSuperUser='" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'"
            WHERE_Utenti_VA &= " AND Username = '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "'"
            WHERE_Utenti_VA &= " AND Entita_Cod = 1"

            Dim WHERE_Validita As String = "Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine)
            WHERE_Validita &= " AND Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio)
            WHERE_Validita &= " AND Inviato "

            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    WHERE_Validita &= ">= 0"
                Case enumVisibilita.Visibilita_SoloCancellati
                    WHERE_Validita &= "= -1"
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            StrSQL.Length = 0

            StrSQL.AppendLine("SELECT COUNT(*) AS FLAG_JOIN")
            StrSQL.AppendLine("FROM Utenti_Visibilita_Appoggio (NOLOCK) ")
            StrSQL.AppendLine("WHERE " & WHERE_Utenti_VA.ToOrigin(objParametri))

            Dim parametriCollezionati As Dictionary(Of Int32, AgroDBParametro) = DammiParametriCollezionati()
            Dim dt_flag As DataTable = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)

            Dim need_join As Boolean = (dt_flag.Rows.Count > 0 AndAlso CInt(dt_flag.Rows(0)("FLAG_JOIN")) > 0)

            StrSQL.Length = 0

            StrSQL.AppendLine("SELECT *")
            StrSQL.AppendLine("FROM Imprese")
            StrSQL.AppendLine("INNER JOIN (")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("SELECT DISTINCT p_iva ")
            StrSQL.AppendLine("FROM (")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("SELECT DISTINCT Padre AS p_iva")
            StrSQL.AppendLine("FROM GerarchiaImprese")
            StrSQL.AppendLine("WHERE Foglia = 1 AND " & WHERE_Validita.ToOrigin(objParametri))
            StrSQL.AppendLine("")
            StrSQL.AppendLine("UNION ALL")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("SELECT DISTINCT Figlio AS p_iva")
            StrSQL.AppendLine("FROM GerarchiaImprese")
            StrSQL.AppendLine("WHERE Foglia = 1 AND " & WHERE_Validita.ToOrigin(objParametri))
            StrSQL.AppendLine(") A")
            If need_join Then
                StrSQL.AppendLine("")
                StrSQL.AppendLine("INNER JOIN Utenti_Visibilita_Appoggio u_v_a (NOLOCK) ")
                StrSQL.AppendLine("ON u_v_a.Piva = A.p_iva AND " & WHERE_Utenti_VA.ToOrigin(objParametri))
                StrSQL.AppendLine("")
            End If
            StrSQL.AppendLine(") B")
            StrSQL.AppendLine("ON B.p_iva = Imprese.PIVA")
            StrSQL.AppendLine("WHERE " & WHERE_Validita.ToOrigin(objParametri))
            StrSQL.AppendLine("")
            StrSQL.AppendLine("ORDER BY Rag_Soc ASC")

            SettaParametriPrecedenti(parametriCollezionati)
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
    ''' Restituisce un DataTable contenente solo una colonna con le top <paramref name="N"/> P.IVA (i dati restituiti non sono ordinati)
    ''' </summary>
    ''' <param name="N"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    Public Function LeggiPivaTopN(ByVal N As Integer, ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Read.LeggiPivaTopN()"
        Dim messaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            strSQL.Length = 0
            strSQL.Append($" SELECT TOP {N} Piva")
            strSQL.Append(" FROM Imprese")
            strSQL.Append(" WHERE   (Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ")  ")
            strSQL.Append(" AND     (Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")

            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSQL.Append(" AND   Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSQL.Append(" AND   Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            DT = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return DT

    End Function


    Public Function Leggi_Gerarchia_Imprese(pivaFiglio As String,
                                            pivaPadre As String,
                                            livelli As List(Of Integer),
                                            foglia As Integer,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreParametri,
                                            Optional ByVal TipoImpresa As List(Of Integer) = Nothing)

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Read.Leggi_Gerarchia_Imprese()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            StrSQL.Append("SELECT *  " & vbCrLf)
            StrSQL.Append(" FROM GerarchiaImprese gi " & vbCrLf)
            StrSQL.Append(" LEFT JOIN Imprese i  " & vbCrLf)
            StrSQL.Append(" ON gi.Figlio = i.PIVA " & vbCrLf)
            StrSQL.Append(" WHERE 1=1 " & vbCrLf)
            If pivaFiglio <> "" Then
                StrSQL.Append(" AND gi.Figlio = " & Agro_SQL_SaveText_NULL(pivaFiglio) & " " & vbCrLf)
            End If
            If pivaPadre <> "" Then
                StrSQL.Append(" AND gi.Padre = " & Agro_SQL_SaveText_NULL(pivaPadre) & " " & vbCrLf)
            End If
            If foglia <> -1 Then
                StrSQL.Append(" AND gi.Foglia = " & Agro_SQL_SaveNum(foglia) & " " & vbCrLf)
            End If
            If livelli.Count <> 0 Then
                StrSQL.Append(" AND gi.livello IN ( ")
                Dim first = True
                For Each liv In livelli
                    If Not first Then
                        StrSQL.Append(", ")
                    Else
                        first = False
                    End If
                    StrSQL.Append(" " & Agro_SQL_SaveNum(liv) & " ")
                Next
                StrSQL.Append(" ) ")
            End If
            If TipoImpresa IsNot Nothing AndAlso TipoImpresa.Count <> 0 Then
                StrSQL.Append(" AND i.TipoImpresaGerarchia IN ( ")
                Dim first = True
                For Each tipi In TipoImpresa
                    If Not first Then
                        StrSQL.Append(", ")
                    Else
                        first = False
                    End If
                    StrSQL.Append(" " & Agro_SQL_SaveNum(tipi) & " ")
                Next
                StrSQL.Append(" ) ")
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
    Public Sub Recupera_DateValidita_ElementiGerarchia(
                                           ByVal ElementoGerarchia As enum_GerarchiaImpresa_Elementi,
                                           ByRef Validita_Inizio As String,
                                           ByRef Validita_Fine As String,
                                           ByRef Errore As String,
                                           ByVal Piva As String,
                                           ByVal Sa_Cod As Integer,
                                           ByVal Campo_Cod As Integer,
                                           ByVal Appezza As Integer,
                                           ByVal Id_Imp As Integer,
                                                    ByRef objParametri As AgronicaCoreParametri
                                                    )

        Dim DT As DataTable

        '---------------

        Select Case ElementoGerarchia

            Case enum_GerarchiaImpresa_Elementi.Gerarchia_Impresa

                '----- Tabella IMPRESE
                Dim DT_Impresa As DataTable

                DT_Impresa = Leggi_3(Piva, False, 0, 0, False, False, False, False, False, False, False, False, False, False,
                                     "", "", objParametri)

                If DT_Impresa.Rows.Count <> 0 Then

                    Errore = ""
                    Validita_Inizio = DT_Impresa.Rows(0).Item("Validita_Inizio")
                    Validita_Fine = DT_Impresa.Rows(0).Item("Validita_Fine")

                Else

                    Errore = "Elemento non trovato"
                    Validita_Inizio = #1/1/1900#
                    Validita_Fine = #12/31/2100#

                End If


                '=================================================================

            Case enum_GerarchiaImpresa_Elementi.Gerarchia_Centro

                Dim objCOM As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read

                'Leggo le informazioni sull'impresa selezionata
                DT = objCOM.Leggi(CStr(Piva), CInt(Sa_Cod),
                                  enumSelezioneVariabile.Selezione_TabellaCompleta,
                                  "", "",
                                  objParametri)

                objCOM = Nothing

                'Se il recordset non è nullo
                If DT.Rows.Count > 0 Then

                    Errore = ""
                    Validita_Inizio = DT.Rows(0).Item("Validita_Inizio")
                    Validita_Fine = DT.Rows(0).Item("Validita_Fine")

                Else

                    Errore = "Elemento non trovato"
                    Validita_Inizio = #1/1/1900#
                    Validita_Fine = #12/31/2100#

                End If


            Case enum_GerarchiaImpresa_Elementi.Gerarchia_Campo

                Dim objCOM As New AgronicaCoreAnagrafeDAL.Campi_R

                'Leggo le informazioni sul campo selezionata
                DT = objCOM.Leggi(CStr(Piva), CInt(Sa_Cod), CInt(Campo_Cod),
                                  enumSelezioneVariabile.Selezione_TabellaCompleta,
                                  "", "",
                                  objParametri)

                objCOM = Nothing

                'Se il recordset non e' nullo
                If DT.Rows.Count > 0 Then

                    Errore = ""
                    Validita_Inizio = DT.Rows(0).Item("Validita_Inizio")
                    Validita_Fine = DT.Rows(0).Item("Validita_Fine")

                Else

                    Errore = "Elemento non trovato"
                    Validita_Inizio = #1/1/1900#
                    Validita_Fine = #12/31/2100#

                End If



            Case enum_GerarchiaImpresa_Elementi.Gerarchia_Appezzamento

                Dim objCOM As New AgronicaCoreAnagrafeDAL.Appezzamento_Read

                'Leggo le informazioni sull'impresa selezionata
                DT = objCOM.Leggi(CStr(Piva), CInt(Sa_Cod), CInt(Appezza),
                                  enumSelezioneVariabile.Selezione_TabellaCompleta,
                                  "", "",
                                  objParametri)

                objCOM = Nothing

                'Se il recordset non è nullo
                If DT.Rows.Count > 0 Then

                    Errore = ""
                    Validita_Inizio = DT.Rows(0).Item("Validita_Inizio")
                    Validita_Fine = DT.Rows(0).Item("Validita_Fine")

                Else

                    Errore = "Elemento non trovato"
                    Validita_Inizio = #1/1/1900#
                    Validita_Fine = #12/31/2100#

                End If


            Case enum_GerarchiaImpresa_Elementi.Gerarchia_Impianto

                Dim objCOM As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read

                'Leggo le informazioni sull'impresa selezionata
                DT = objCOM.Leggi(CStr(Piva), CInt(Sa_Cod), CInt(Appezza), CInt(Id_Imp),
                                  enumSelezioneVariabile.Selezione_TabellaCompleta,
                                  "", "",
                                  objParametri)

                objCOM = Nothing

                'Se il recordset non è nullo
                If DT.Rows.Count > 0 Then

                    Errore = ""
                    Validita_Inizio = DT.Rows(0).Item("Validita_Inizio")
                    Validita_Fine = DT.Rows(0).Item("Validita_Fine")

                Else

                    Errore = "Elemento non trovato"
                    Validita_Inizio = #1/1/1900#
                    Validita_Fine = #12/31/2100#

                End If

        End Select

    End Sub


    Public Function RecuperaDatiImpresa_From_Piva(ByVal Piva As String,
                                                  ByVal Piva_SuperUser As String,
                                                  ByRef ErrMSG As String,
                                                  ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                                  ByVal xFiltroAggiuntivo As String,
                                                  ByVal xOrderBy As String,
                                                  ByRef objParametri As AgronicaCoreParametri
                                                  ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Read.RecuperaDatiImpresa_From_Piva()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT Imprese.PIVA, ")
            StrSQL.AppendLine(" CASE WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN Imprese.PIVA ELSE Imprese.partitaIvaReale END PivaReale, ")
            StrSQL.AppendLine(" Imprese.rag_soc, Imprese.TipoImpresaGerarchia,  ")
            StrSQL.AppendLine(" Imprese.Validita_Inizio, Imprese.Validita_Fine, ")
            StrSQL.AppendLine(" Indirizzi.Cod_Indirizzo, Indirizzi.ind_des, Indirizzi.frz_des,  Indirizzi.CAP,  ")
            StrSQL.AppendLine(" ISTAT.LOCALITA, ISTAT.COMUNI_PROV , Indirizzi.pro_cod_istat, Indirizzi.com_cod_istat, ISNULL(Blk_Inizio_Note,'') AS Note, ")
            StrSQL.AppendLine(" ISNULL ((SELECT TOP 1  Rubrica.numero   ")
            StrSQL.AppendLine("          FROM       Risorse_Umane ")
            StrSQL.AppendLine("          JOIN Contatti ON Risorse_Umane.Piva = Contatti.Piva AND Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto ")
            StrSQL.AppendLine("          JOIN ContattiXRubrica ON ContattiXRubrica.Cod_Contatto = Contatti.Cod_Contatto ")
            StrSQL.AppendLine("          JOIN Rubrica ON Rubrica.cod_rubrica = ContattiXRubrica.Cod_Rubrica ")
            StrSQL.AppendLine("          WHERE       Imprese.PIVA = Risorse_Umane.Piva   " & vbCrLf)
            StrSQL.AppendLine("          AND         Cod_Rapporto = -1   " & vbCrLf)
            StrSQL.AppendLine("          AND         descr LIKE '%tel%'   " & vbCrLf)
            StrSQL.AppendLine("         ), '') AS Telefono, ")
            StrSQL.AppendLine(" ISNULL ((SELECT TOP 1  Rubrica.numero   ")
            StrSQL.AppendLine("          FROM       Risorse_Umane ")
            StrSQL.AppendLine("          JOIN Contatti ON Risorse_Umane.Piva = Contatti.Piva AND Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto ")
            StrSQL.AppendLine("          JOIN ContattiXRubrica ON ContattiXRubrica.Cod_Contatto = Contatti.Cod_Contatto ")
            StrSQL.AppendLine("          JOIN Rubrica ON Rubrica.cod_rubrica = ContattiXRubrica.Cod_Rubrica ")
            StrSQL.AppendLine("          WHERE       Imprese.PIVA = Risorse_Umane.Piva   " & vbCrLf)
            StrSQL.AppendLine("          AND         Cod_Rapporto = -1   " & vbCrLf)
            StrSQL.AppendLine("          AND         descr LIKE '%mail%'   " & vbCrLf)
            StrSQL.AppendLine("         ), '') AS EMail, ")
            StrSQL.AppendLine(" ISNULL ((SELECT TOP 1   val_cod  ")
            StrSQL.AppendLine("          FROM        Imprese_Codici ")
            StrSQL.AppendLine("          WHERE       Imprese.PIVA = Imprese_Codici.PIVA AND id_cod = 1010), '') AS CUAA, ")
            StrSQL.AppendLine("  ISNULL  ((SELECT TOP 1   val_cod  ")
            StrSQL.AppendLine("          FROM        Imprese_Codici  ")
            StrSQL.AppendLine("          WHERE       Imprese.PIVA = Imprese_Codici.PIVA AND id_cod = 1033), '') AS Codice_Socio, ")
            StrSQL.AppendLine("  ISNULL  ((SELECT TOP 1   Codice_Fiscale ")
            StrSQL.AppendLine("          FROM        Contatti ")
            StrSQL.AppendLine("          WHERE       Imprese.PIVA = Contatti.Cod_Contatto), '') AS Codice_Fiscale, ")
            StrSQL.AppendLine("  ISNULL  ((SELECT TOP 1   Piva ")
            StrSQL.AppendLine("          FROM        Contatti ")
            StrSQL.AppendLine("          WHERE       Imprese.PIVA = Contatti.Cod_Contatto), '') AS Piva_Contatto, ")
            StrSQL.AppendLine("  ISNULL  ((SELECT TOP 1   Cod_RisUm ")
            StrSQL.AppendLine("          FROM        Risorse_Umane ")
            StrSQL.AppendLine("          WHERE       Imprese.PIVA = Risorse_Umane.Cod_Contatto ")
            StrSQL.AppendLine("          AND         Risorse_Umane.Piva = '" & Agro_SQL_SaveText(Piva_SuperUser) & "' ")
            StrSQL.AppendLine("          AND         Risorse_Umane.Cod_Rapporto = -3), '') AS Cod_RisUm, ")
            StrSQL.AppendLine("  ISNULL  ((SELECT TOP 1   Padre  ")
            StrSQL.AppendLine("          FROM        GerarchiaImprese  ")
            StrSQL.AppendLine("          WHERE       Imprese.PIVA = GerarchiaImprese.Figlio), '') AS Padre, ")

            StrSQL.AppendLine(" ISNULL ((SELECT TOP 1   val_cod  ")
            StrSQL.AppendLine("          FROM        Imprese_Codici ")
            StrSQL.AppendLine("          WHERE       Imprese.PIVA = Imprese_Codici.PIVA AND id_cod = 1118), '') AS Cod_Zona, ")

            StrSQL.AppendLine("  ISNULL ((SELECT TOP 1   val_cod  ")
            StrSQL.AppendLine("          FROM        Imprese_Codici ")
            StrSQL.AppendLine("          WHERE       Imprese.PIVA = Imprese_Codici.PIVA AND id_cod = " & Agro_SQL_SaveNum(CStr(enum_DatiAnagrafici_CodiciAnagrafe.Azienda)) & "), '0') AS Modifica ")

            StrSQL.AppendLine(" FROM Imprese INNER JOIN ")
            StrSQL.AppendLine(" ImpresexIndirizzi ON Imprese.PIVA = ImpresexIndirizzi.PIVA INNER JOIN ")
            StrSQL.AppendLine(" Indirizzi ON ImpresexIndirizzi.cod_indirizzo = Indirizzi.cod_indirizzo INNER JOIN ")
            StrSQL.AppendLine(" ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM ")

            StrSQL.AppendLine(" WHERE   (Imprese.PIVA = '" & Agro_SQL_SaveText(Piva) & "')  ")


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        Finally
            ErrMSG = MessaggioErrore
        End Try

        Return DT

    End Function

    Public Function RecuperaDatiImpresa_From_Pive(ByVal Pive As List(Of String),
                                                  ByVal Piva_SuperUser As String,
                                                  ByRef ErrMSG As String,
                                                  ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                                  ByVal xFiltroAggiuntivo As String,
                                                  ByVal xOrderBy As String,
                                                  ByRef objParametri As AgronicaCoreParametri
                                                  ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Read.RecuperaDatiImpresa_From_Piva()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            If Pive Is Nothing OrElse Pive.Count = 0 Then
                Throw New Exception("La lista di PIVA è vuota.")
            End If

            ConnessioniTransazioni.ApriConnessione(True, objParametri)

            TempChiaviMassivo.CreaTabellaTemp_FiltroPiva(Pive, NomeRoutine, objParametri)

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT CASE WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN Imprese.PIVA ELSE Imprese.partitaIvaReale END AS Piva,  ")
            StrSQL.AppendLine(" Imprese.rag_soc, Imprese.TipoImpresaGerarchia, Imprese.Validita_Inizio, Imprese.Validita_Fine, ")
            StrSQL.AppendLine(" Indirizzi.Cod_Indirizzo, Indirizzi.ind_des, Indirizzi.frz_des,  Indirizzi.CAP,  ")
            StrSQL.AppendLine(" ISTAT.LOCALITA, ISTAT.COMUNI_PROV , Indirizzi.pro_cod_istat, Indirizzi.com_cod_istat, ISNULL(Blk_Inizio_Note,'') AS Note, ")
            StrSQL.AppendLine(" ISNULL ((SELECT TOP 1  Rubrica.numero   ")
            StrSQL.AppendLine("          FROM       Risorse_Umane ")
            StrSQL.AppendLine("          JOIN Contatti ON Risorse_Umane.Piva = Contatti.Piva AND Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto ")
            StrSQL.AppendLine("          JOIN ContattiXRubrica ON ContattiXRubrica.Cod_Contatto = Contatti.Cod_Contatto ")
            StrSQL.AppendLine("          JOIN Rubrica ON Rubrica.cod_rubrica = ContattiXRubrica.Cod_Rubrica ")
            StrSQL.AppendLine("          WHERE       Imprese.PIVA = Risorse_Umane.Piva   " & vbCrLf)
            StrSQL.AppendLine("          AND         Cod_Rapporto = -1   " & vbCrLf)
            StrSQL.AppendLine("          AND         descr LIKE '%tel%'   " & vbCrLf)
            StrSQL.AppendLine("         ), '') AS Telefono, ")
            StrSQL.AppendLine(" ISNULL ((SELECT TOP 1   val_cod  ")
            StrSQL.AppendLine("          FROM        Imprese_Codici ")
            StrSQL.AppendLine("          WHERE       Imprese.PIVA = Imprese_Codici.PIVA AND id_cod = 1010), '') AS CUAA, ")
            StrSQL.AppendLine("  ISNULL  ((SELECT TOP 1   val_cod  ")
            StrSQL.AppendLine("          FROM        Imprese_Codici  ")
            StrSQL.AppendLine("          WHERE       Imprese.PIVA = Imprese_Codici.PIVA AND id_cod = 1033), '') AS Codice_Socio, ")
            StrSQL.AppendLine("  ISNULL  ((SELECT TOP 1   Codice_Fiscale ")
            StrSQL.AppendLine("          FROM        Contatti ")
            StrSQL.AppendLine("          WHERE       Imprese.PIVA = Contatti.Cod_Contatto), '') AS Codice_Fiscale, ")
            StrSQL.AppendLine("  ISNULL  ((SELECT TOP 1   Piva ")
            StrSQL.AppendLine("          FROM        Contatti ")
            StrSQL.AppendLine("          WHERE       Imprese.PIVA = Contatti.Cod_Contatto), '') AS Piva_Contatto, ")
            StrSQL.AppendLine("  ISNULL  ((SELECT TOP 1   Cod_RisUm ")
            StrSQL.AppendLine("          FROM        Risorse_Umane ")
            StrSQL.AppendLine("          WHERE       Imprese.PIVA = Risorse_Umane.Cod_Contatto ")
            StrSQL.AppendLine("          AND         Risorse_Umane.Piva = '" & Agro_SQL_SaveText(Piva_SuperUser) & "' ")
            StrSQL.AppendLine("          AND         Risorse_Umane.Cod_Rapporto = -3), '') AS Cod_RisUm, ")
            StrSQL.AppendLine("  ISNULL  ((SELECT TOP 1   Padre  ")
            StrSQL.AppendLine("          FROM        GerarchiaImprese  ")
            StrSQL.AppendLine("          WHERE       Imprese.PIVA = GerarchiaImprese.Figlio), '') AS Padre, ")

            StrSQL.AppendLine(" ISNULL ((SELECT TOP 1   val_cod  ")
            StrSQL.AppendLine("          FROM        Imprese_Codici ")
            StrSQL.AppendLine("          WHERE       Imprese.PIVA = Imprese_Codici.PIVA AND id_cod = 1118), '') AS Cod_Zona, ")

            StrSQL.AppendLine("  ISNULL ((SELECT TOP 1   val_cod  ")
            StrSQL.AppendLine("          FROM        Imprese_Codici ")
            StrSQL.AppendLine("          WHERE       Imprese.PIVA = Imprese_Codici.PIVA AND id_cod = " & Agro_SQL_SaveNum(CStr(enum_DatiAnagrafici_CodiciAnagrafe.Azienda)) & "), '0') AS Modifica ")

            StrSQL.AppendLine(" FROM Imprese INNER JOIN")
            StrSQL.AppendLine(" #TempPiva tmp ON tmp.Piva = Imprese.Piva COLLATE DATABASE_DEFAULT INNER JOIN")
            StrSQL.AppendLine(" ImpresexIndirizzi ON Imprese.PIVA = ImpresexIndirizzi.PIVA INNER JOIN ")
            StrSQL.AppendLine(" Indirizzi ON ImpresexIndirizzi.cod_indirizzo = Indirizzi.cod_indirizzo INNER JOIN ")
            StrSQL.AppendLine(" ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            ' Eliminazione tabella temporanea
            TempChiaviMassivo.EliminaTabellaTemp_FiltroPiva(NomeRoutine, objParametri)

            'commit transazione
            ConnessioniTransazioni.ChiudiTransazione(1, objParametri)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            'rollback transazione
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        Finally
            ErrMSG = MessaggioErrore
            ConnessioniTransazioni.ChiudiConnessione(objParametri)
        End Try

        Return DT

    End Function

    Public Function RecuperaDatiImpresa_From_Cuaa(ByVal Cuaa As String,
                                                  ByRef ErrMSG As String,
                                                  ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                                  ByVal xFiltroAggiuntivo As String,
                                                  ByVal xOrderBy As String,
                                                  ByRef objParametri As AgronicaCoreParametri
                                                  ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Read.RecuperaDatiImpresa_From_Cuaa()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT Imprese.PIVA, Imprese.rag_soc, Imprese_Codici.val_cod AS Cuaa, ")
            StrSQL.Append(" Indirizzi.ind_des, Indirizzi.frz_des, Indirizzi.CAP,  ")
            StrSQL.Append(" Indirizzi.com_des, Indirizzi.pro_cod, Indirizzi.pro_cod_istat, Indirizzi.com_cod_istat, ")
            StrSQL.Append(" ISNULL  ((SELECT TOP 1   Padre  ")
            StrSQL.Append("          FROM        GerarchiaImprese  ")
            StrSQL.Append("          WHERE       Imprese.PIVA = GerarchiaImprese.Figlio), '') AS Padre ")

            StrSQL.Append(" FROM Imprese INNER JOIN ")
            StrSQL.Append(" Imprese_Codici ON Imprese.PIVA = Imprese_Codici.PIVA INNER JOIN ")
            StrSQL.Append(" ImpresexIndirizzi ON Imprese.PIVA = ImpresexIndirizzi.PIVA INNER JOIN ")
            StrSQL.Append(" Indirizzi ON ImpresexIndirizzi.cod_indirizzo = Indirizzi.cod_indirizzo ")

            StrSQL.Append(" WHERE   (Imprese_Codici.id_cod = 1010)   ")
            If Cuaa <> "" Then
                StrSQL.Append(" AND     (Imprese_Codici.val_cod = '" & Agro_SQL_SaveText(Cuaa) & "')  ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            ErrMSG = MessaggioErrore
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        Finally
            ErrMSG = MessaggioErrore
        End Try

        Return DT

    End Function

    '  Galassi, 16/08/2016 17:16:44: Inserita nei core per leggere direttamente i dati dell'impresa che ha quel cod_socio senza fare 2 chiamate
    Public Function RecuperaDatiImpresa_From_Cod_Socio(ByVal Cod_Socio As String,
                                                       ByRef ErrMSG As String,
                                                       ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                                       ByVal xFiltroAggiuntivo As String,
                                                       ByVal xOrderBy As String,
                                                       ByRef objParametri As AgronicaCoreParametri
                                                       ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Read.RecuperaDatiImpresa_From_Cod_Socio()"

        '====================================================================================
        'Parametri opzionali :
        '   Cod_Socio = ""                   =>  si leggono tutte le imprese aventi codice socio
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT Imprese.PIVA, Imprese.rag_soc, Imprese_Codici.val_cod AS Cod_Socio, ")
            StrSQL.Append(" Indirizzi.ind_des, Indirizzi.frz_des, Indirizzi.CAP,  ")
            StrSQL.Append(" Indirizzi.com_des, Indirizzi.pro_cod, Indirizzi.pro_cod_istat, Indirizzi.com_cod_istat, ")
            StrSQL.Append(" ISNULL  ((SELECT TOP 1   Padre  ")
            StrSQL.Append("          FROM        GerarchiaImprese  ")
            StrSQL.Append("          WHERE       Imprese.PIVA = GerarchiaImprese.Figlio), '') AS Padre ")

            StrSQL.Append(" FROM Imprese INNER JOIN ")
            StrSQL.Append(" Imprese_Codici ON Imprese.PIVA = Imprese_Codici.PIVA INNER JOIN ")
            StrSQL.Append(" ImpresexIndirizzi ON Imprese.PIVA = ImpresexIndirizzi.PIVA INNER JOIN ")
            StrSQL.Append(" Indirizzi ON ImpresexIndirizzi.cod_indirizzo = Indirizzi.cod_indirizzo ")

            StrSQL.Append(" WHERE   (Imprese_Codici.id_cod = 1033)   ")
            If Cod_Socio <> "" Then
                StrSQL.Append(" AND     (Imprese_Codici.val_cod = '" & Agro_SQL_SaveText(Cod_Socio) & "')  ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            ErrMSG = MessaggioErrore
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        Finally
            ErrMSG = MessaggioErrore
        End Try

        Return DT

    End Function

    Public Function NumeroImprese_Leggi(ByVal Piva_SuperUser As String,
                                        ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByRef objParametri As AgronicaCoreParametri
                                        ) As Integer

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Read.NumeroImprese_Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT  COUNT(*) AS Num_Imprese  ")
            StrSQL.Append("  FROM    Imprese ")
            StrSQL.Append("  INNER JOIN UtentiXImprese ON Imprese.PIVA = UtentiXImprese.PIVA ")
            StrSQL.Append("  WHERE UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(Piva_SuperUser) & "' ")


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Imprese.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Imprese.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
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

        If Not IsNothing(DT) Then
            Return DT.Rows(0).Item("Num_Imprese")
        Else
            Return 0
        End If

    End Function


    Public Function Numero_Imprese_from_SuperUser(ByVal Piva_SuperUser As String,
                                                  ByRef Piva As String,
                                                  ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                                  ByVal xFiltroAggiuntivo As String,
                                                  ByRef objParametri As AgronicaCoreParametri
                                                  ) As Integer

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Read.Numero_Imprese_from_SuperUser()"

        Dim MessaggioErrore As String = ""

        Try

            Dim Num_Imprese As Integer = 0

            Num_Imprese = NumeroImprese_Leggi(Piva_SuperUser,
                                            xSelezioneVariabile,
                                            xFiltroAggiuntivo,
                                            objParametri)

            Select Case Num_Imprese

                Case 0

                    Piva = ""
                    Return 0

                Case 1

                    Dim DT As DataTable

                    Dim xOrderBy As String = ""

                    DT = Leggi("",
                        xSelezioneVariabile,
                        xFiltroAggiuntivo,
                        xOrderBy,
                        objParametri)

                    If Not IsNothing(DT) AndAlso DT.Rows.Count <> 0 Then
                        Piva = DT.Rows(0).Item("Piva")
                        Return DT.Rows.Count
                    End If

                Case Is > 1
                    Piva = ""
                    Return Num_Imprese

                Case Else
                    Piva = ""
                    Return 0

            End Select

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Piva = ""
            Return 0
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

    End Function


    '###########################################################################################
    Public Function Recupera_DatiImpresa(ByVal Piva As String,
                                         ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByVal xOrderBy As String,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Read.Recupera_Dati_Impresa()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0
                    StrSQL.Append("SELECT Imprese.*, ")
                    StrSQL.Append(" ISNULL ")
                    StrSQL.Append("         ((SELECT TOP 1   val_cod  ")
                    StrSQL.Append("         FROM        Imprese_Codici ")
                    StrSQL.Append("         WHERE       Imprese.PIVA = Imprese_Codici.PIVA AND id_cod = '1010'), ' ') AS CUAA, ")
                    StrSQL.Append(" ISNULL  ")
                    StrSQL.Append("         ((SELECT TOP 1   val_cod  ")
                    StrSQL.Append("         FROM        Imprese_Codici  ")
                    StrSQL.Append("         WHERE       Imprese.PIVA = Imprese_Codici.PIVA AND id_cod = '1033'), ' ') AS Codice_Socio, ")
                    StrSQL.Append(" ISNULL  ((SELECT TOP 1   Codice_Fiscale ")
                    StrSQL.Append("         FROM        Contatti ")
                    StrSQL.Append("         WHERE       Imprese.PIVA = Contatti.Cod_Contatto), ' ') AS Codice_Fiscale ")
                    StrSQL.Append(" FROM    Imprese ")
                    StrSQL.Append(" WHERE   Piva = '" & Agro_SQL_SaveText(Piva).Trim & "' ")
                    StrSQL.Append(" AND     Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND     Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If


                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append("SELECT Imprese.*, ")
                    StrSQL.Append(" ISNULL ")
                    StrSQL.Append("         ((SELECT TOP 1   val_cod  ")
                    StrSQL.Append("         FROM        Imprese_Codici ")
                    StrSQL.Append("         WHERE       Imprese.PIVA = Imprese_Codici.PIVA AND id_cod = '1010'), ' ') AS CUAA, ")
                    StrSQL.Append(" ISNULL  ")
                    StrSQL.Append("         ((SELECT TOP 1   val_cod  ")
                    StrSQL.Append("         FROM        Imprese_Codici  ")
                    StrSQL.Append("         WHERE       Imprese.PIVA = Imprese_Codici.PIVA AND id_cod = '1033'), ' ') AS Codice_Socio, ")
                    StrSQL.Append(" ISNULL  ((SELECT TOP 1   Codice_Fiscale ")
                    StrSQL.Append("         FROM        Contatti ")
                    StrSQL.Append("         WHERE       Imprese.PIVA = Contatti.Cod_Contatto), ' ') AS Codice_Fiscale ")
                    StrSQL.Append(" FROM    Imprese ")
                    StrSQL.Append(" WHERE   Piva = '" & Agro_SQL_SaveText(Piva).Trim & "' ")
                    StrSQL.Append(" AND     Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND     Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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


    '###############################################################
    Public Sub Recupera_Superfici_Impresa(
                                        ByVal Piva As String,
                                        ByRef Sup_Totale As Decimal,
                                        ByRef Sup_Bosco As Decimal,
                                        ByRef Sup_Prati As Decimal,
                                        ByRef Sup_Tare As Decimal,
                                        ByRef SAU_Totale As Decimal,
                                        ByRef SAU_Biologico As Decimal,
                                        ByRef SAU_Conversione As Decimal,
                                        ByRef SAU_Convenzionale As Decimal,
                                        ByVal DataRecupero As Date,
                                            ByRef objParametri As AgronicaCoreParametri
                                            )

        Dim objCentriAz As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
        Dim DT As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Read.Recupera_Superfici_Impresa()"

        DT = objCentriAz.Leggi(CStr(Piva), 0,
                               enumSelezioneVariabile.Selezione_TabellaCompleta,
                               "", "",
                               objParametri)

        'Inizializzo
        Sup_Totale = 0
        Sup_Bosco = 0
        Sup_Prati = 0
        Sup_Tare = 0
        SAU_Totale = 0
        SAU_Biologico = 0
        SAU_Conversione = 0
        SAU_Convenzionale = 0

        Dim xSup_Totale As Decimal = 0
        Dim xSup_Bosco As Decimal = 0
        Dim xSup_Prati As Decimal = 0
        Dim xSup_Tare As Decimal = 0
        Dim xSAU_Totale As Decimal = 0
        Dim xSAU_Biologico As Decimal = 0
        Dim xSAU_Conversione As Decimal = 0
        Dim xSAU_Convenzionale As Decimal = 0


        Dim i As Integer
        For i = 0 To DT.Rows.Count - 1
            objCentriAz.Recupera_Superfici_CentroAziendale(
                                    DT.Rows(i).Item("Piva"),
                                    DT.Rows(i).Item("Sa_Cod"),
                                    xSup_Totale,
                                    xSup_Bosco,
                                    xSup_Prati,
                                    xSup_Tare,
                                    xSAU_Totale,
                                    xSAU_Biologico,
                                    xSAU_Conversione,
                                    xSAU_Convenzionale,
                                    DataRecupero,
                                    objParametri)

            'Aggiorno la somma
            Sup_Totale += xSup_Totale
            Sup_Bosco += xSup_Bosco
            Sup_Prati += xSup_Prati
            Sup_Tare += xSup_Tare
            SAU_Totale += xSAU_Totale
            SAU_Biologico += xSAU_Biologico
            SAU_Conversione += xSAU_Conversione
            SAU_Convenzionale += xSAU_Convenzionale
        Next


    End Sub


    '###############################################################
    Public Function RagSoc_from_Piva(ByVal Piva As String,
                                     ByRef objParametri As AgronicaCoreParametri
                                     ) As String

        Dim DT As DataTable

        DT = Leggi(Piva, enumSelezioneVariabile.Selezione_TabellaCompleta,
                   "", "", objParametri)


        If Not IsNothing(DT) Then
            If DT.Rows.Count <> 0 Then
                Return DT.Rows(0).Item("Rag_Soc")
            Else
                Return ""
            End If
        Else
            Return ""
        End If

    End Function

    Public Function RagSoc_from_Piva_Massivo(ByVal PivaList As List(Of String),
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Read.RagSoc_from_Piva_Massivo()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, objParametri)

            EseguiQuery_Scrittura(objParametri, CreaTabellaTemp_FiltroPiva(), NomeRoutine)

            If Not IsNothing(PivaList) AndAlso PivaList.Any Then
                Dim chunks = ChunkBy(Of String)(PivaList, 1000)
                For Each chunk In chunks
                    StrSQL.AppendLine("INSERT INTO #TempPiva (Piva) VALUES ")
                    For Each p As String In chunk
                        StrSQL.AppendLine(String.Format("('{0}'),", p))
                    Next
                    Dim strSqlInsert As String = StrSQL.ToString
                    strSqlInsert = strSqlInsert.Remove(strSqlInsert.LastIndexOf(","))
                    StrSQL.Clear()
                    EseguiQuery_Scrittura(objParametri, strSqlInsert, NomeRoutine)
                Next
            End If

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT Imprese.PIVA, Rag_Soc")
            StrSQL.Append(" FROM Imprese ")
            If PivaList IsNot Nothing AndAlso PivaList.Count > 0 Then
                StrSQL.AppendLine("	INNER JOIN #TempPiva temp (NOLOCK) ON Imprese.Piva COLLATE SQL_Latin1_General_CP850_CI_AS = temp.Piva ")
            End If
            StrSQL.Append(" WHERE 1 = 1 ")

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            ' Eliminazione tabella temporanea
            EseguiQuery_Scrittura(objParametri, EliminaTabellaTemp_FiltroPiva, NomeRoutine)

            'commit transazione
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, objParametri)


        Catch ex As Exception
            ' Rollback
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri)
        End Try


        Return DT

    End Function
    Private Function CreaTabellaTemp_FiltroPiva() As String
        Dim stb As New StringBuilder

        stb.AppendLine(" IF OBJECT_ID('tempdb.dbo.#TempPiva') IS NULL BEGIN ")
        stb.AppendLine("    CREATE TABLE #TempPiva ( ")
        stb.AppendLine("        Piva varchar(50) NULL")
        stb.AppendLine("    )")
        stb.AppendLine(" END ")

        Return stb.ToString()
    End Function
    Private Function EliminaTabellaTemp_FiltroPiva() As String
        Dim stb As New StringBuilder

        stb.AppendLine(" IF NOT OBJECT_ID('tempdb.dbo.#TempPiva') IS NULL BEGIN ")
        stb.AppendLine("    DROP TABLE #TempPiva ")
        stb.AppendLine(" END ")

        Return stb.ToString()
    End Function

    Public Function FormaGiuridica_from_Piva(ByVal Piva As String,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As String

        Dim DT As DataTable

        DT = Leggi(Piva, enumSelezioneVariabile.Selezione_TabellaCompleta,
                   "", "", objParametri)

        If Not IsNothing(DT) Then
            If DT.Rows.Count <> 0 Then
                Return DT.Rows(0).Item("Forma_Giuridica")
            Else
                Return ""
            End If
        Else
            Return ""
        End If

    End Function

    '################################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[magnani]	28/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function Recupera_OP_from_Piva(ByVal Piva As String, ByRef objParametri As AgronicaCoreParametri) As DataTable


        Dim objCOMGerarchia As New GerarchiaImprese_R
        Dim RsGerarchia As DataTable
        Dim RsImprese As DataTable

        'leggo i padri..
        'per ogni padre leggo il tipo gerarchia impresa..
        'se è una op restituisco il record..
        'altrimenti leggo il padre del padre..
        'se è una op restituisco il record..

        RsGerarchia = objCOMGerarchia.LeggixFiglio(CStr(Piva), enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)

        If Not IsNothing(RsGerarchia) AndAlso RsGerarchia.Rows.Count <> 0 Then

            Dim i As Integer
            For i = 0 To RsGerarchia.Rows.Count - 1

                RsImprese = Leggi(CStr(RsGerarchia.Rows(i).Item("padre")), enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)

                If Not IsNothing(RsImprese) AndAlso RsImprese.Rows.Count <> 0 Then

                    Select Case CInt(RsImprese.Rows(0).Item("TipoImpresaGerarchia"))

                        Case 4 'op

                            Return RsImprese

                        Case 2 'cooperativa

                            Dim objCOMGerarchia1 As New GerarchiaImprese_R
                            Dim RsGerarchia1 As DataTable

                            RsGerarchia1 = objCOMGerarchia1.LeggixFiglio(CStr(RsGerarchia.Rows(i).Item("padre")), enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)
                            If Not IsNothing(RsGerarchia1) AndAlso RsGerarchia1.Rows.Count <> 0 Then

                                Dim j As Integer
                                For j = 0 To RsGerarchia1.Rows.Count - 1

                                    Dim RsImprese1 As DataTable

                                    RsImprese1 = Leggi(CStr(RsGerarchia1.Rows(j).Item("padre")), enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)

                                    If Not IsNothing(RsImprese1) AndAlso RsImprese1.Rows.Count <> 0 Then

                                        Select Case CInt(RsImprese1.Rows(0).Item("TipoImpresaGerarchia"))

                                            Case 4 'op

                                                Return RsImprese1

                                        End Select

                                    End If

                                Next

                            End If

                    End Select

                End If

            Next

        End If

    End Function


    '###############################################################################
    Public Function EsistePiva_RitornaDati(ByVal Piva As String,
                                                ByRef Rag_Soc As String,
                                                ByRef Pro_Cod_Istat As String,
                                                ByRef Com_Cod_Istat As String,
                                                ByRef Ind_Des As String,
                                                ByRef Frz_Des As String,
                                                ByRef CAP As String,
                                                ByRef Titolo_Possesso As Integer,
                                                ByRef objParametri As AgronicaCoreParametri
                                           ) As Boolean

        Dim DT As DataTable
        Dim Esiste As Boolean = False

        DT = Leggi_3(Piva,
                        True,
                        0,
                        0,
                        False,
                        False,
                        True,
                        False,
                        False,
                        False,
                        False,
                        False,
                        False,
                        False,
                        "", "",
                        objParametri)

        If Not IsNothing(DT) AndAlso DT.Rows.Count <> 0 Then
            Rag_Soc = DT.Rows(0).Item("Rag_Soc")
            Pro_Cod_Istat = DT.Rows(0).Item("Pro_Cod_Istat")
            Com_Cod_Istat = DT.Rows(0).Item("Com_Cod_Istat")
            Ind_Des = DT.Rows(0).Item("Ind_Des")
            Frz_Des = DT.Rows(0).Item("Frz_Des")
            CAP = DT.Rows(0).Item("CAP")
            Titolo_Possesso = DT.Rows(0).Item("Titolo_Possesso")
            Esiste = True
        End If

        Return Esiste

    End Function


    '###############################################################################
    Public Function Esiste_CUAA(ByVal CUAA As String,
                                ByRef objParametri As AgronicaCoreParametri
                                ) As Boolean

        'verifico se l'impresa è presente su Gias
        Dim Dt_Impresa As New DataTable
        Dim strErr As String = ""
        Dim Flag_Esiste As Boolean = False

        Dt_Impresa = RecuperaDatiImpresa_From_Cuaa(CUAA, strErr,
                                                   enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                   "", "",
                                                   objParametri)

        If Dt_Impresa IsNot Nothing AndAlso Dt_Impresa.Rows.Count > 0 AndAlso strErr = "" Then
            Flag_Esiste = True
        End If

        Return Flag_Esiste


    End Function

    '###############################################################################
    Public Function Piva_From_CUAA(ByVal CUAA As String,
                                   ByRef objParametri As AgronicaCoreParametri
                                   ) As String

        'verifico se l'impresa è presente su Gias
        Dim Dt_Impresa As New DataTable
        Dim strErr As String = ""

        Dt_Impresa = RecuperaDatiImpresa_From_Cuaa(CUAA, strErr,
                                                   enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                   "", "",
                                                   objParametri)

        If Dt_Impresa IsNot Nothing AndAlso Dt_Impresa.Rows.Count > 0 AndAlso strErr = "" Then
            If Dt_Impresa.Rows.Count <> 1 Then
                Throw New Exception("Ci sono più imprese con lo stesso cuaa")
            End If
            Return Dt_Impresa.Rows(0).Item("Piva")
        End If

        Return ""

    End Function

    '##############################################################################################
    Public Function ImpreseSenzaCentriAziendali(ByVal Piva As String,
                                                ByVal Flag_Indirizzi As Boolean,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreParametri
                                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Read.ImpreseSenzaCentriAziendali()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT  Imprese.*   ")

            If Flag_Indirizzi Then
                StrSQL.Append(" , ImpresexIndirizzi.Tipo_Indirizzo, Indirizzi.Cod_Indirizzo, Indirizzi.ind_des, Indirizzi.frz_des, Indirizzi.CAP, Indirizzi.com_cod_istat, Indirizzi.pro_cod_istat, Indirizzi.stato, Indirizzi.note, ")
                StrSQL.Append(" ISNULL(ISTAT.LOCALITA, '') AS com_des, ISNULL(ISTAT.COMUNI_PROV, '') AS pro_cod  ")
            End If

            StrSQL.Append(" FROM    Imprese ")
            StrSQL.Append(" INNER JOIN UtentiXImprese ON Imprese.PIVA = UtentiXImprese.PIVA ")

            If Flag_Indirizzi Then
                StrSQL.Append("  INNER JOIN  ImpresexIndirizzi ON Imprese.Piva = ImpresexIndirizzi.Piva ")
                StrSQL.Append("  INNER JOIN Indirizzi ON ImpresexIndirizzi.Cod_Indirizzo = Indirizzi.cod_indirizzo ")
                StrSQL.Append("  LEFT OUTER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM ")
            End If

            StrSQL.Append(" WHERE UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Piva <> "" Then
                StrSQL.Append(" AND     Imprese.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            StrSQL.Append(" AND not exists ( " & vbCrLf)
            StrSQL.Append("                 select 1  " & vbCrLf & vbCrLf)
            StrSQL.Append("                 from centri_aziendali " & vbCrLf)
            StrSQL.Append("                 where centri_aziendali.piva=imprese.piva " & vbCrLf)
            StrSQL.Append("                 ) ")

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Imprese.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Imprese.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Rag_Soc ASC  ")
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


    Public Function Esiste_Impresa(ByVal Piva As String,
                        ByVal ID_Cod As Integer,
                             ByRef Data_Modifica As Date,
                             ByRef Piva_Padre As String,
                             ByRef Socio As String,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Codici_Read.Esiste_Impresa()"

        '====================================================================================

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim Impresa_Presente As Boolean = False

        Try
            '---------------------------------------------


            StrSQL.Length = 0
            StrSQL.Append(" SELECT  Imprese.*, Imprese_Codici.id_cod, Imprese_Codici.val_cod, ISNULL(GerarchiaImprese.Padre,'') AS Padre ")
            StrSQL.Append(" FROM    Imprese INNER JOIN GerarchiaImprese ON Imprese.PIVA = GerarchiaImprese.Figlio LEFT OUTER JOIN Imprese_Codici ON Imprese.PIVA = Imprese_Codici.PIVA ")
            StrSQL.Append(" WHERE 1=1 ")

            If Piva <> "" Then
                StrSQL.Append(" AND     (Imprese_Codici.PIVA = '" & Agro_SQL_SaveText(Piva).Trim & "')  ")
            End If

            If ID_Cod <> 0 Then
                StrSQL.Append(" AND     (Imprese_Codici.id_cod = " & Agro_SQL_SaveNum(ID_Cod) & ") ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   (Imprese.inviato >= 0) ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   (Imprese.inviato = -1) ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT.Rows.Count > 0 Then

                Impresa_Presente = True
                Data_Modifica = CDate(DT.Rows(0).Item("data_modifica"))
                Piva_Padre = DT.Rows(0).Item("padre")


                Dim drs As DataRow()
                drs = DT.Select("ID_Cod = " & Agro_SQL_SaveNum(ID_Cod, False))
                If drs.Length > 0 Then
                    Socio = drs(0).Item("val_cod")
                Else
                    Socio = ""
                End If

            ElseIf DT.Rows.Count = 0 Then

                Impresa_Presente = False
                Data_Modifica = Nothing
                Socio = ""
                Piva_Padre = ""

            Else
                Throw New Exception("Sono presenti più imprese con quel codice, verificare sul db i record su Imprese_Codici ")
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Impresa_Presente

    End Function


    Public Function Azienda_G2G(ByVal Piva As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Read.Azienda_G2G()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim isG2G As Boolean = True

        If Piva = "" Then
            Throw New Exception("Parametro non corretto nella query (Piva obbligatoria)")
        End If

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * " & vbCrLf)
            StrSQL.Append(" FROM    G2G_Recode_Imprese " & vbCrLf)
            StrSQL.Append(" WHERE   from_piva = '" & Agro_SQL_SaveText(Piva) & "' OR  to_piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & xFiltroAggiuntivo & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                isG2G = True
            Else
                isG2G = False
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return isG2G

    End Function

    Public Function Leggi_ImpreseCUAA_Visibilita_Utente(ByRef objParametri_Server As AgronicaCoreParametri,
                                                        ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                        ByVal Filtro_Visibilita_Utente As Boolean,
                                                        Optional ByVal moduli_Generazione As String = "") As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Read.Leggi_ImpreseCUAA_Visibilita_Utente()"

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Try

            stb.Length = 0


            stb.AppendLine(" SELECT DISTINCT dbo.Imprese.PIVA as piva, dbo.Imprese.rag_soc + ' (' + COALESCE(IC_Cuaa.val_cod, '') + ')'  as rag_soc ")
            'stb.AppendLine("                 COALESCE(FG.flagPubblica, 0)  as flagPubblica, forma_giuridica")

            stb.AppendLine(" FROM Imprese ")
            stb.AppendLine(" JOIN Imprese_Codici IC_cuaa on IC_cuaa.Piva = Imprese.Piva AND IC_cuaa.id_cod = 1010  ")

            If Filtro_Visibilita_Utente Then
                stb.AppendLine("   LEFT JOIN Utenti_Visibilita_Appoggio (NOLOCK) On Imprese.Piva = Utenti_Visibilita_Appoggio.Piva AND Utenti_Visibilita_Appoggio.Entita_Cod=1  ")
            End If

            If Not IsNothing(moduli_Generazione) AndAlso moduli_Generazione <> "" Then
                stb.AppendLine(" JOIN OGenerazioni_Anagrafe_Moduli_Log on OGenerazioni_Anagrafe_Moduli_Log.Piva = Imprese.Piva ")
            End If

            stb.AppendLine(" WHERE 1=1  ")

            If Filtro_Visibilita_Utente Then
                stb.AppendLine(" AND Utenti_Visibilita_Appoggio.Username = " & Agro_SQL_SaveText_NULL(objParametri_Server.UtenteUsername) & "  ")
            End If

            If Not IsNothing(moduli_Generazione) AndAlso moduli_Generazione <> "" Then
                stb.AppendLine(" AND OGenerazioni_Anagrafe_Moduli_Log.Modulo_Generazione " & moduli_Generazione & " ")
            End If

            stb.AppendLine(" AND   Imprese.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine) & " ")
            stb.AppendLine(" AND   Imprese.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleInizio) & " ")

            stb.AppendLine(" ORDER BY dbo.Imprese.rag_soc + ' (' + COALESCE(IC_Cuaa.val_cod, '') + ')' ")

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

    Public Function Leggi_ImpreseCUAA_Visibilita_Area(ByRef objParametri_Server As AgronicaCoreParametri,
                                                      ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                      ByVal area As Integer,
                                                      ByVal userName As String,
                                                      ByVal gruppo As Integer,
                                                      ByVal visibilitaCompleta As Boolean,
                                                      Optional Tipo_Azienda_UMA As Integer = 0
                                                      ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Read.Leggi_ImpreseCUAA_Visibilita_Utente()"

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Try

            stb.Length = 0


            stb.AppendLine(" SELECT DISTINCT dbo.Imprese.PIVA as piva, dbo.Imprese.rag_soc + ' (' + COALESCE(IC_Cuaa.val_cod, '') + ')'  as rag_soc,")
            stb.AppendLine("                 COALESCE(FG.flagPubblica, 0)  as flagPubblica, forma_giuridica, dbo.Imprese.PartitaIvaReale")

            stb.AppendLine(" FROM Imprese ")
            stb.AppendLine(" JOIN Imprese_Codici IC_cuaa on IC_cuaa.Piva = Imprese.Piva AND IC_cuaa.id_cod = 1010  ")

            'Vado in left perché non è un campo sempre riempito per le imprese
            stb.AppendLine(" LEFT JOIN FormeGiuridiche FG on FG.FG_cod = Imprese.Forma_Giuridica")

            '12/03/2025 Vado in left perchè gli utenti con visibilità globale devono poter vedere tutte le imprese
            stb.AppendLine(" LEFT JOIN " & objParametri_Utenti.Recupera_NomeDB & ".dbo.Utenti_Visibilita uv On Imprese.Piva = uv.Piva_Azienda ")

            stb.AppendLine(" WHERE Imprese.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine) & " ")
            stb.AppendLine(" AND   Imprese.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleInizio) & " ")

            stb.AppendLine(" AND (uv.Area = " & area.ToString & If(Not visibilitaCompleta, " AND uv.userName = '" & userName & "' AND uv.gruppo = " & gruppo.ToString, " OR uv.Area IS NULL ") & " ) ")

            If Tipo_Azienda_UMA <> 0 Then
                Select Case Tipo_Azienda_UMA
                    Case enum_TipoAzienda_UMA.Cooperativa_Agricola
                        'Mostro solo le cooperative
                        stb.AppendLine(" AND FG.FG_cod in (" & enum_FormeGiuridiche.Societa_Cooperativa_a_Mutualita_Prevalente & ", " & enum_FormeGiuridiche.Societa_Cooperativa_Diversa & " ," & enum_FormeGiuridiche.Societa_Cooperativa_Sociale & ") ")
                    Case enum_TipoAzienda_UMA.Azienda_Terzista
                        'Escludo le aziende pubbliche
                        stb.AppendLine(" AND COALESCE(FG.flagPubblica, 0) <> '1' ")
                        'Escludo le cooperative
                        stb.AppendLine(" AND COALESCE(FG.FG_cod, 0) NOT IN (" & enum_FormeGiuridiche.Societa_Cooperativa_a_Mutualita_Prevalente & ", " & enum_FormeGiuridiche.Societa_Cooperativa_Diversa & " ," & enum_FormeGiuridiche.Societa_Cooperativa_Sociale & ") ")
                End Select
            End If

            stb.AppendLine(" ORDER BY dbo.Imprese.rag_soc + ' (' + COALESCE(IC_Cuaa.val_cod, '') + ')' ")

            'stb.AppendLine(" ORDER BY CASE WHEN COALESCE(IC_Cuaa.val_cod, '') = dbo.Imprese.PIVA THEN ")
            'stb.AppendLine(" dbo.Imprese.rag_soc + ' (' + dbo.Imprese.PartitaIvaReale + ')' ELSE dbo.Imprese.rag_soc + ' (' + COALESCE(IC_Cuaa.val_cod, '') + ')' END ")

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

    Public Function Leggi_CUAA_ImpreseFoglia(ByVal pivaFiglio As String,
                                             ByVal pivaPadre As String,
                                             ByVal livelli As List(Of Integer),
                                             ByVal foglia As Integer,
                                             ByVal TipoImpresa As List(Of Integer),
                                             ByVal xFiltroAggiuntivo As String,
                                             ByVal xOrderBy As String,
                                             ByRef objParametri As AgronicaCoreParametri)

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Read.Leggi_CUAA_ImpreseFoglia()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT DISTINCT i.*, Imprese_Codici.val_Cod as CUAA ")
            StrSQL.AppendLine(" FROM GerarchiaImprese gi ")
            StrSQL.AppendLine(" JOIN Imprese i ON gi.Figlio = i.PIVA ")
            StrSQL.AppendLine(" JOIN Imprese_Codici ON i.Piva = Imprese_Codici.Piva AND  Imprese_Codici.id_Cod = 1010 ")
            StrSQL.AppendLine(" WHERE 1=1 ")
            If pivaFiglio <> "" Then
                StrSQL.AppendLine(" AND gi.Figlio = " & Agro_SQL_SaveText_NULL(pivaFiglio) & " " & vbCrLf)
            End If
            If pivaPadre <> "" Then
                StrSQL.AppendLine(" AND gi.Padre = " & Agro_SQL_SaveText_NULL(pivaPadre) & " " & vbCrLf)
            End If
            If foglia <> -1 Then
                StrSQL.AppendLine(" AND gi.Foglia = " & Agro_SQL_SaveNum(foglia) & " " & vbCrLf)
            End If
            If livelli IsNot Nothing AndAlso livelli.Count <> 0 Then
                StrSQL.AppendLine(" AND gi.livello IN ( ")
                Dim first = True
                For Each liv In livelli
                    If Not first Then
                        StrSQL.AppendLine(", ")
                    Else
                        first = False
                    End If
                    StrSQL.AppendLine(" " & Agro_SQL_SaveNum(liv) & " ")
                Next
                StrSQL.AppendLine(" ) ")
            End If
            If TipoImpresa IsNot Nothing AndAlso TipoImpresa.Count <> 0 Then
                StrSQL.Append(" AND i.TipoImpresaGerarchia IN ( ")
                Dim first = True
                For Each tipi In TipoImpresa
                    If Not first Then
                        StrSQL.Append(", ")
                    Else
                        first = False
                    End If
                    StrSQL.Append(" " & Agro_SQL_SaveNum(tipi) & " ")
                Next
                StrSQL.Append(" ) ")
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy))
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

    Public Function Leggi_CUAA_ImpreseFoglia_TOPN(ByVal pivaFiglio As String,
                                             ByVal pivaPadre As String,
                                             ByVal livelli As List(Of Integer),
                                             ByVal foglia As Integer,
                                             ByVal TipoImpresa As List(Of Integer),
                                             ByVal TopN As Integer,
                                             ByVal xFiltroAggiuntivo As String,
                                             ByVal xOrderBy As String,
                                             ByRef objParametri As AgronicaCoreParametri)

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Read.Leggi_CUAA_ImpreseFoglia()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT DISTINCT TOP " & TopN & " i.*, Imprese_Codici.val_Cod as CUAA ")
            StrSQL.AppendLine(" FROM GerarchiaImprese gi ")
            StrSQL.AppendLine(" JOIN Imprese i ON gi.Figlio = i.PIVA ")
            StrSQL.AppendLine(" JOIN Imprese_Codici ON i.Piva = Imprese_Codici.Piva AND  Imprese_Codici.id_Cod = 1010 ")
            StrSQL.AppendLine(" WHERE 1=1 ")
            If pivaFiglio <> "" Then
                StrSQL.AppendLine(" AND gi.Figlio = " & Agro_SQL_SaveText_NULL(pivaFiglio) & " " & vbCrLf)
            End If
            If pivaPadre <> "" Then
                StrSQL.AppendLine(" AND gi.Padre = " & Agro_SQL_SaveText_NULL(pivaPadre) & " " & vbCrLf)
            End If
            If foglia <> -1 Then
                StrSQL.AppendLine(" AND gi.Foglia = " & Agro_SQL_SaveNum(foglia) & " " & vbCrLf)
            End If
            If livelli IsNot Nothing AndAlso livelli.Count <> 0 Then
                StrSQL.AppendLine(" AND gi.livello IN ( ")
                Dim first = True
                For Each liv In livelli
                    If Not first Then
                        StrSQL.AppendLine(", ")
                    Else
                        first = False
                    End If
                    StrSQL.AppendLine(" " & Agro_SQL_SaveNum(liv) & " ")
                Next
                StrSQL.AppendLine(" ) ")
            End If
            If TipoImpresa IsNot Nothing AndAlso TipoImpresa.Count <> 0 Then
                StrSQL.Append(" AND i.TipoImpresaGerarchia IN ( ")
                Dim first = True
                For Each tipi In TipoImpresa
                    If Not first Then
                        StrSQL.Append(", ")
                    Else
                        first = False
                    End If
                    StrSQL.Append(" " & Agro_SQL_SaveNum(tipi) & " ")
                Next
                StrSQL.Append(" ) ")
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy))
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

    Public Function Leggi_Agenzie_Visibilita_Utente(ByRef objParametri_Server As AgronicaCoreParametri,
                                                    ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                    ByVal Filtro_Visibilita_Utente As Boolean) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Read.Leggi_AgenzieCAI_Visibilita_Utente()"

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Try

            stb.Length = 0
            stb.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")
            stb.AppendLine(" SELECT DISTINCT dbo.Imprese.PIVA as piva, dbo.Imprese.rag_soc ")
            stb.AppendLine(" FROM Imprese ")
            stb.AppendLine(" Join Imprese_Codici IC on IC.Piva = Imprese.Piva And IC.id_cod = " & enum_CodiciAnagrafe.CodiceAgenzia & " ")

            If Filtro_Visibilita_Utente Then
                stb.AppendLine("   LEFT JOIN Utenti_Visibilita_Appoggio (NOLOCK) On Imprese.Piva = Utenti_Visibilita_Appoggio.Piva AND Utenti_Visibilita_Appoggio.Entita_Cod=1  ")
            End If

            stb.AppendLine(" WHERE 1=1  ")

            If Filtro_Visibilita_Utente Then
                stb.AppendLine(" AND Utenti_Visibilita_Appoggio.Username = " & Agro_SQL_SaveText_NULL(objParametri_Server.UtenteUsername) & "  ")
            End If

            stb.AppendLine(" AND   Imprese.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine) & " ")
            stb.AppendLine(" AND   Imprese.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleInizio) & " ")

            stb.AppendLine(" ORDER BY dbo.Imprese.rag_soc ")

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

    Public Function Imprese_Leggi_VisibilitaUtente_Agenzie_PIVA(objParametri_Server As AgronicaCoreParametri,
                                                                objParametri_Utenti As AgronicaCoreParametri
                                                                ) As List(Of String)
        Dim DT As DataTable
        Dim objProfilo As New AgronicaCoreUtentiDAL.Utenti_Profili_Read
        Dim Filtro_Visibilita_Utente = Not objProfilo.HasFullVisibility(objParametri_Utenti.UtenteUsername, objParametri_Utenti)

        DT = Leggi_Agenzie_Visibilita_Utente(objParametri_Server, objParametri_Utenti, Filtro_Visibilita_Utente)

        Dim listaPivaAgenzie As List(Of String) = Nothing
        If DT IsNot Nothing AndAlso DT.Rows.Count > 0 Then
            listaPivaAgenzie = New List(Of String)
            For Each row In DT.Rows
                listaPivaAgenzie.Add(row.Item("piva"))
            Next
        End If

        Return listaPivaAgenzie
    End Function

    ''' <summary>
    ''' Query usata da AgronicaSincronizzatore_2010\Interscambio_Anagrafica\Export_CAI_Aziende.vb
    ''' </summary>
    ''' <param name="fineValidita"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    Public Function Imprese_QdcAttivo_CodiceSocio(
            ByVal fineValidita As Date,
            ByVal xFiltroAggiuntivo As String,
            ByVal xOrderBy As String,
            ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Read.Imprese_QdcAttivo_CodiceSocio()"

        Dim dt As DataTable
        Dim strSql As New StringBuilder()

        Try

            strSql.AppendLine("select Imprese.PIVA")
            strSql.AppendLine(",Imprese.rag_soc")
            strSql.AppendLine(",Imprese_Codici_Cuaa.val_cod As Cuaa")
            strSql.AppendLine(",Imprese_Codici_Socio.val_cod AS Codice_Socio")
            strSql.AppendLine(",Pratiche.Validita_Fine AS Validita_Fine_Stato")
            strSql.AppendLine("from Imprese")
            strSql.AppendLine("inner join Imprese_Codici as Imprese_Codici_Cuaa")
            strSql.AppendLine("on Imprese_Codici_Cuaa.PIVA = Imprese.PIVA")
            strSql.AppendLine("and Imprese_Codici_Cuaa.Id_Cod = " & enum_CodiciAnagrafe.CodiceCUAA)
            strSql.AppendLine("inner join Imprese_Codici as Imprese_Codici_Socio")
            strSql.AppendLine("on Imprese_Codici_Socio.PIVA = Imprese.PIVA")
            strSql.AppendLine("and Imprese_Codici_Socio.Id_Cod = " & enum_CodiciAnagrafe.Codice_Socio)
            strSql.AppendLine("inner join Pratiche")
            strSql.AppendLine("on Pratiche.Piva = Imprese.PIVA")
            strSql.AppendLine("and Pratiche.Servizio_Cod = " & enum_Servizi.CAIImpresaQdCAttivo)
            strSql.AppendLine("inner join Pratiche_Stati_Attuali")
            strSql.AppendLine("on Pratiche_Stati_Attuali.Piva_SuperUser = Pratiche.Piva_SuperUser")
            strSql.AppendLine("and Pratiche_Stati_Attuali.Pratica_Cod = Pratiche.Pratica_Cod")
            strSql.AppendLine("and Pratiche_Stati_Attuali.Stato_Cod = " & enum_WWorflow_WAnagraficaStati.QdCAttivo)
            strSql.AppendLine("where 1=1")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.Append("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.Append("AND   Imprese.Inviato >= 0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.Append("AND   Imprese.Inviato = -1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
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




Public Class Imprese_Write
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal Piva As String,
                           ByVal Rag_Soc As String,
                           ByVal Delega As String,
                           ByVal AT_Prevalente As String,
                           ByVal Forma_Giuridica As String,
                           ByVal Forma_Conduzione As String,
                           ByVal Sup_Totale As Decimal,
                           ByVal TipoImpresaGerarchia As Integer,
                           ByVal Note As String,
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
                           Optional ByVal Blk_Flag As Integer = 0,
                           Optional ByVal Blk_Inizio_Data As DateTime = #2/1/1900#,
                           Optional ByVal Blk_Inizio_Username As String = "",
                           Optional ByVal Blk_Fine_Data As DateTime = #2/1/1900#,
                           Optional ByVal Blk_Fine_Username As String = "",
                           Optional ByVal Blk_Fine_Note As String = ""
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Imprese_Write.Scrivi()"

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

        If Blk_Inizio_Data = #2/1/1900# Then
            Blk_Inizio_Data = Now
        End If

        If Blk_Fine_Data = #2/1/1900# Then
            Blk_Fine_Data = Now
        End If

        If Not PivaValida(Piva) Then
            Throw New Exception("Rilevato carattere non valido nella PIVA:" & Piva)
        End If

        Try
            '---------------------------------------------
            strSql.Length = 0
            strSql.AppendLine("INSERT INTO Imprese(Piva,   Rag_Soc,    Delega,  AT_Prevalente, ")
            strSql.AppendLine("                    Forma_Giuridica,    Forma_Conduzione,  ")
            strSql.AppendLine("                    Sup_Totale, TipoImpresaGerarchia, Blk_Inizio_Note, ")

            strSql.AppendLine("                    Validazione,   Data_Validazione,  UserName_Validazione, ")
            strSql.AppendLine("                    Blk_Flag,      Blk_Inizio_Data,   Blk_Inizio_Username, ")
            strSql.AppendLine("                    Blk_Fine_Data, Blk_Fine_Username, Blk_Fine_Note, ")

            strSql.AppendLine("                    Inviato, DataInvio, ")
            strSql.AppendLine("                    Data_Creazione,     Data_Modifica, ")
            strSql.AppendLine("                    UserName_Creazione, UserName_Modifica, ")
            strSql.AppendLine("                    Validita_Inizio,    Validita_Fine, PartitaIvaReale ")
            strSql.AppendLine("                    ) ")

            strSql.AppendLine("VALUES (")
            strSql.AppendLine("          '" & Agro_SQL_SaveText(Piva) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Rag_Soc) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Delega) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(AT_Prevalente) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Forma_Giuridica) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Forma_Conduzione) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Sup_Totale) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(TipoImpresaGerarchia) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Note) & "' ")

            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Validazione) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_Validazione) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(UserName_Validazione) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Blk_Flag) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Blk_Inizio_Data) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Blk_Inizio_Username) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Blk_Fine_Data) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Blk_Fine_Username) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Blk_Fine_Note) & "' ")

            strSql.AppendLine("         , 0  ")
            strSql.AppendLine("         , NULL  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(Piva) & "' ")
            strSql.AppendLine(")")
            '---------------------------------------------


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            'inserisco il record in visibilita_appoggio per vedere l'azienda
            '(20/07/2015) aggiunto controllo che l'utente non abbia visibilita totale (tabella Utenti_Visibilita_Appoggio vuota)
            If xRisp Then
                Try
                    Dim objUtentiVisibilitaR As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                    Dim DtImpreseVisibili As DataTable
                    DtImpreseVisibili = objUtentiVisibilitaR.Leggi(enum_TipoEntita.Impresa, "", "", objParametri)
                    If DtImpreseVisibili IsNot Nothing AndAlso DtImpreseVisibili.Rows.Count > 0 Then
                        Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_W
                        objUtentiVisibilita.Scrivi(enum_TipoEntita.Impresa, Piva, 0, 0, 0, objParametri)
                    End If
                Catch ex As Exception

                End Try
            End If


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function Modifica(ByVal Piva As String,
                             ByVal Rag_Soc As String,
                             ByVal Delega As String,
                             ByVal AT_Prevalente As String,
                             ByVal Forma_Giuridica As String,
                             ByVal Forma_Conduzione As String,
                             ByVal Sup_Totale As Decimal,
                             ByVal TipoImpresaGerarchia As Integer,
                             ByVal Note As String,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri,
                             Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                             Optional ByVal username_modifica As String = "",
                             Optional ByVal Validazione As Integer? = Nothing,
                             Optional ByVal Data_Validazione As DateTime? = Nothing,
                             Optional ByVal UserName_Validazione As String = Nothing,
                             Optional ByVal Blk_Flag As Integer? = Nothing,
                             Optional ByVal Blk_Inizio_Data As DateTime? = Nothing,
                             Optional ByVal Blk_Inizio_Username As String = Nothing,
                             Optional ByVal Blk_Fine_Data As DateTime? = Nothing,
                             Optional ByVal Blk_Fine_Username As String = Nothing,
                             Optional ByVal Blk_Fine_Note As String = Nothing
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Imprese_Write.Modifica()"

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
            '---------------------------------------------
            strSql.Length = 0
            strSql.AppendLine("UPDATE Imprese SET ")
            strSql.AppendLine("   Inviato              =  0 ")
            strSql.AppendLine("   ,PartitaIvaReale = ISNULL(PartitaIvaReale, piva)")

            If Rag_Soc <> "" Then
                strSql.AppendLine("   ,Rag_Soc              = '" & Agro_SQL_SaveText(Rag_Soc) & "'")
            End If

            If Delega <> "" Then
                strSql.AppendLine("   ,Delega               = '" & Agro_SQL_SaveText(Delega) & "'")
            End If

            If AT_Prevalente <> "" Then
                strSql.AppendLine("   ,AT_Prevalente        = '" & Agro_SQL_SaveText(AT_Prevalente) & "'")
            End If

            If Forma_Giuridica <> "" Then
                strSql.AppendLine("   ,Forma_Giuridica      = '" & Agro_SQL_SaveText(Forma_Giuridica) & "'")
            End If

            If Forma_Conduzione <> "" Then
                strSql.AppendLine("   ,Forma_Conduzione     = '" & Agro_SQL_SaveText(Forma_Conduzione) & "'")
            End If

            If Sup_Totale <> 0 Then
                strSql.AppendLine("   ,Sup_Totale           =  " & Agro_SQL_SaveNum(Sup_Totale) & " ")
            End If

            If TipoImpresaGerarchia <> 0 Then
                strSql.AppendLine("   ,TipoImpresaGerarchia =  " & Agro_SQL_SaveNum(TipoImpresaGerarchia) & " ")
            End If


            strSql.AppendLine("   ,Blk_Inizio_Note     = '" & Agro_SQL_SaveText(Note) & "'")
            strSql.AppendLine("   ,DataInvio            =  Null ")
            strSql.AppendLine("   ,Data_Modifica        =  " & Agro_SQL_SaveDateTime(Data_modifica))
            strSql.AppendLine("   ,UserName_Modifica    = '" & Agro_SQL_SaveText(username_modifica) & "'")
            strSql.AppendLine("   ,Validita_Inizio      =  " & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.AppendLine("   ,Validita_Fine        =  " & Agro_SQL_SaveDate(Validita_Fine))

            If Not IsNothing(Validazione) Then
                strSql.AppendLine("   ,Validazione =  " & Agro_SQL_SaveNum(Validazione) & " ")
            End If

            If Not IsNothing(Data_Validazione) Then
                strSql.AppendLine("   ,Data_Validazione =  " & Agro_SQL_SaveDateTime(Data_Validazione) & " ")
            End If

            If Not IsNothing(UserName_Validazione) Then
                strSql.AppendLine("   ,UserName_Validazione =  '" & Agro_SQL_SaveText(UserName_Validazione) & "' ")
            End If

            If Not IsNothing(Blk_Flag) Then
                strSql.AppendLine("   ,Blk_Flag =  " & Agro_SQL_SaveNum(Blk_Flag) & " ")
            End If

            If Blk_Inizio_Data <> "#01/01/0001#" Then
                strSql.AppendLine("   ,Blk_Inizio_Data =  " & Agro_SQL_SaveDateTime(Blk_Inizio_Data) & " ")
            End If

            If Not IsNothing(Blk_Inizio_Username) Then
                strSql.AppendLine("   ,Blk_Inizio_Username =  '" & Agro_SQL_SaveText(Blk_Inizio_Username) & "' ")
            End If

            If Not IsNothing(Blk_Fine_Data) Then
                strSql.AppendLine("   ,Blk_Fine_Data =  " & Agro_SQL_SaveDateTime(Blk_Fine_Data) & " ")
            End If

            If Not IsNothing(Blk_Fine_Username) Then
                strSql.AppendLine("   ,Blk_Fine_Username =  '" & Agro_SQL_SaveText(Blk_Fine_Username) & "' ")
            End If

            If Not IsNothing(Blk_Fine_Note) Then
                strSql.AppendLine("   ,Blk_Fine_Note =  '" & Agro_SQL_SaveText(Blk_Fine_Note) & "' ")
            End If


            strSql.AppendLine(" WHERE Piva='" & Agro_SQL_SaveText(Piva) & "'")
            '---------------------------------------------

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

    Public Function Cancella(ByVal Piva As String,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Imprese_Write.Cancella()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.Length = 0
                strSql.AppendLine(" UPDATE Imprese ")
                strSql.AppendLine(" SET ")
                strSql.AppendLine("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.AppendLine("      ,Inviato = -1 ")
                strSql.AppendLine(" WHERE  Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                strSql.AppendLine(" AND Inviato >= 0")

            Else

                strSql.Length = 0
                strSql.AppendLine(" DELETE ")
                strSql.AppendLine(" FROM     Imprese ")
                strSql.AppendLine(" WHERE    Piva= '" & Agro_SQL_SaveText(Piva) & "' ")

            End If
            '---------------------------------------------

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

    Public Function Aggiorna_Validazione(ByVal Validazione As Integer,
                                         ByVal Piva As String,
                                         ByVal Data_Validazione As Date,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Imprese_Write.Aggiorna_Validazione()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            strSql.Length = 0
            strSql.AppendLine("UPDATE Imprese SET ")
            strSql.AppendLine("    Validazione          = " & Agro_SQL_SaveNum(Validazione))
            strSql.AppendLine("   ,Username_Validazione = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            strSql.AppendLine("   ,Data_Validazione        = " & Agro_SQL_SaveDate(Data_Validazione))
            strSql.AppendLine(" WHERE Piva='" & Agro_SQL_SaveText(Piva) & "'")

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

    Public Function Modifica_dataVariazioneAzienda(ByVal Piva As String,
                                                   ByVal Data_Variazione As Date,
                                                   ByRef objParametri As AgronicaCoreParametri
                                                   ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Imprese_Write.Modifica_dataVariazioneAzienda()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            strSql.Length = 0
            strSql.AppendLine(" UPDATE Imprese SET ")
            strSql.AppendLine("     dataVariazioneAzienda = " & Agro_SQL_SaveDateTime(Data_Variazione))
            strSql.AppendLine(" WHERE Piva='" & Agro_SQL_SaveText(Piva) & "'")

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
