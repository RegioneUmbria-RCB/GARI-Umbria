Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class Sportello_R
    Inherits AgronicaCoreDataProvider.DataProvider

    ''' <summary>
    ''' Legge gli sportelli che intersecano l'intervallo di date indicato
    ''' </summary>
    ''' <param name="ApplicaFiltroUtente"></param>
    ''' <param name="VegCod"></param>
    ''' <param name="Sementieri_Sportello_Configurazione_Cod"></param>
    ''' <param name="Sementieri_Sportello_Passaggi_cod"></param>
    ''' <param name="Data_Inizio"></param>
    ''' <param name="Data_Fine"></param>
    ''' <param name="xSelezioneVariabile"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri_server"></param>
    ''' <param name="objParametri_utenti"></param>
    ''' <param name="ApplicaFiltroUtente"></param>
    ''' <returns></returns>
    Public Function Leggi(
            ByVal UtentePerFiltroSpecie As String,
            ByVal VegCod As Integer,
            ByVal Sementieri_Sportello_Configurazione_Cod As Integer,
            ByVal Sementieri_Sportello_Passaggi_cod As Integer,
            ByVal Data_Inizio As DateTime,
            ByVal Data_Fine As DateTime,
            ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
            ByVal xOrderBy As String,
            ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri,
            Optional ByRef objParametri_utenti As AgronicaCoreDataProvider.AgronicaCoreParametri = Nothing,
            Optional ByVal ApplicaFiltroUtente As Boolean = True
        ) As DataTable

        Dim NomeRoutine As String = "Sportello_R.Sportello_LeggiXData()"
        Dim MessaggioErrore As String = ""

        Dim dt As DataTable
        Dim stb As New StringBuilder


        Try

            LeggiCommon(UtentePerFiltroSpecie, VegCod, Sementieri_Sportello_Configurazione_Cod, Sementieri_Sportello_Passaggi_cod, objParametri_server, objParametri_utenti, ApplicaFiltroUtente, stb)

            If Data_Fine <> AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAFINE Then

                If Data_Inizio = Data_Fine Then
                    stb.Append(" and " & Agro_SQL_SaveDate(Data_Inizio) & " <= ssp.Data_Fine " & vbCrLf)
                    stb.Append(" and " & Agro_SQL_SaveDate(Data_Inizio) & " >= ssp.Data_Inizio " & vbCrLf)
                    stb.Append(" and CONVERT(DateTime,'2100/12/31',120) <> ssp.Data_Fine " & vbCrLf)
                Else
                    stb.Append(" and " & Agro_SQL_SaveDate(Data_Inizio) & " <= ssp.Data_Fine " & vbCrLf)
                    stb.Append(" and  ssp.Data_Inizio <=" & Agro_SQL_SaveDate(Data_Fine) & " " & vbCrLf)
                    stb.Append(" and CONVERT(DateTime,'2100/12/31',120) <> ssp.Data_Fine " & vbCrLf)
                End If

            Else
                stb.Append(" and " & Agro_SQL_SaveDate(Data_Fine) & " = ssp.Data_Fine " & vbCrLf)
            End If


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_server, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return dt
    End Function

    Private Sub LeggiCommon(UtentePerFiltroSpecie As String, veg_Cod As Integer, Sementieri_Sportello_Configurazione_Cod As Integer, Sementieri_Sportello_Passaggi_cod As Integer, objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri, objParametri_utenti As AgronicaCoreDataProvider.AgronicaCoreParametri, ApplicaFiltroUtente As Boolean, stb As StringBuilder)

        stb.Append(" Select distinct " & vbCrLf)
        stb.Append("     c.sementieri_Sportello_configurazione_cod " & vbCrLf)
        stb.Append("    ,c.sementieri_Sportello_configurazione_des " & vbCrLf)
        stb.Append("    ,c.validita_inizio " & vbCrLf)
        stb.Append("    ,c.validita_fine " & vbCrLf)

        stb.Append("    , mp.id_specie " & vbCrLf)
        stb.Append("    , ss.Sementieri_ClassiDiSpecieVegetali_des " & vbCrLf)

        stb.Append("    , ssp.Data_Inizio " & vbCrLf)
        stb.Append("    , ssp.Data_Fine " & vbCrLf)

        stb.Append("    , p.* " & vbCrLf)
        stb.Append("    , c.Validita_Fine " & vbCrLf)

        If UtentePerFiltroSpecie <> "" Then
            stb.Append("    , Mappatura_Specie_x_Utente.UserName " & vbCrLf)
        Else
            stb.Append("    , '' as UserName " & vbCrLf)
        End If


        If objParametri_utenti IsNot Nothing Then
            stb.Append("    , gu.Gruppi_Utente_Identificativo as CodiceFiscaleTecnico " & vbCrLf)
            stb.Append("    , d.email  " & vbCrLf)
            stb.Append("    , d.codFisc as utenteCodFiscale  " & vbCrLf)
        End If


        stb.Append(" from dbo.Sementieri_Sportello_Configurazione c " & vbCrLf)
        stb.Append("    inner join dbo.Sementieri_Sportello_ConfigurazioneXmappatura_specie mp " & vbCrLf)
        stb.Append("        on c.Sementieri_Sportello_Configurazione_cod = mp.Sementieri_Sportello_Configurazione_cod  " & vbCrLf)

        stb.Append("    inner join Sementieri_Sportello_ConfigurazioneXPassaggi ssp " & vbCrLf)
        stb.Append("        on ssp.Sementieri_Sportello_Configurazione_cod = c.Sementieri_Sportello_Configurazione_cod  " & vbCrLf)

        stb.Append("    inner join Sementieri_sportello_passaggi p " & vbCrLf)
        stb.Append("        on ssp.Sementieri_Sportello_Passaggi_cod = p.Sementieri_Sportello_Passaggi_cod  " & vbCrLf)

        stb.Append("    inner join Sementieri_ClassiDiSpecieVegetali ss " & vbCrLf)
        stb.Append("        on  mp.ID_Specie = ss.id_specie " & vbCrLf)
        stb.Append("        and mp.ID_Sottospecie = ss.ID_Sottospecie " & vbCrLf)
        stb.Append("        and mp.ID_Gruppo = ss.ID_Gruppo  " & vbCrLf)
        stb.Append("        and mp.ID_Genotipo = ss.ID_Genotipo  " & vbCrLf)

        If veg_Cod <> -1 Then

            stb.Append("    inner join Mappatura_Specie ms " & vbCrLf)
            stb.Append("        on  ms.ID_Specie = ss.id_specie " & vbCrLf)
            stb.Append("        and ms.ID_Sottospecie = ss.ID_Sottospecie " & vbCrLf)
            stb.Append("        and ms.ID_Gruppo = ss.ID_Gruppo  " & vbCrLf)
            stb.Append("        and ms.ID_Genotipo = ss.ID_Genotipo  " & vbCrLf)
        End If

        If UtentePerFiltroSpecie <> "" Then
            stb.Append("    inner Join " & vbCrLf)
            stb.Append("        Mappatura_Specie_x_Utente ON mp.ID_Specie = Mappatura_Specie_x_Utente.ID_Specie " & vbCrLf)
        End If


        If objParametri_utenti IsNot Nothing AndAlso UtentePerFiltroSpecie <> "" Then

            Dim NomeDB_Utenti As String = objParametri_utenti.StringaConnessione.Split(";")(2).Split("=")(1)

            stb.Append("    inner Join  " & NomeDB_Utenti & ".dbo.Utenti_xGruppi_Utente uxgu " & vbCrLf)
            stb.Append("    on uxgu.Username = Mappatura_Specie_x_Utente.UserName " & vbCrLf)
            stb.Append("                inner Join " & NomeDB_Utenti & ".dbo.Gruppi_Utente gu " & vbCrLf)
            stb.Append("     On gu.Gruppi_Utente_cod = uxgu.Gruppi_Utente_cod" & vbCrLf)

            stb.Append("                inner Join " & NomeDB_Utenti & ".dbo.Utenti_Dettagli d " & vbCrLf)
            stb.Append("     On d.UserName = uxgu.UserName " & vbCrLf)

        End If

        stb.Append(" where 1=1 " & vbCrLf)

        If Sementieri_Sportello_Configurazione_Cod <> 0 Then
            stb.Append(" and c.Sementieri_Sportello_Configurazione_Cod = " & Agro_SQL_SaveNum(Sementieri_Sportello_Configurazione_Cod) & vbCrLf)
        End If

        If Sementieri_Sportello_Passaggi_cod <> 0 Then
            stb.Append(" and p.Sementieri_Sportello_Passaggi_cod = " & Agro_SQL_SaveNum(Sementieri_Sportello_Passaggi_cod) & vbCrLf)
        End If

        If ApplicaFiltroUtente AndAlso UtentePerFiltroSpecie <> "" Then
            stb.Append(" and Mappatura_Specie_x_Utente.UserName = '" & Agro_SQL_SaveText(UtentePerFiltroSpecie) & "'")
        End If

        If veg_Cod <> -1 Then
            stb.Append(" and ms.veg_cod = " & veg_Cod & " ")
        End If

    End Sub

    Public Function Leggi(
            ByVal Sementieri_Sportello_Configurazione_Cod As Integer,
            ByVal Sementieri_Sportello_Passaggi_cod As Integer,
            ByVal DataCorrente As DateTime,
            ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
            ByVal xOrderBy As String,
            ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri,
            Optional ByRef objParametri_utenti As AgronicaCoreDataProvider.AgronicaCoreParametri = Nothing,
            Optional ByVal ApplicaFiltroUtente As Boolean = True
        ) As DataTable

        Dim NomeRoutine As String = "Sportello_R.Sportello_LeggiXData()"
        Dim MessaggioErrore As String = ""

        Dim dt As DataTable
        Dim stb As New StringBuilder


        Try

            LeggiCommon(objParametri_server.UtenteUsername, -1, Sementieri_Sportello_Configurazione_Cod, Sementieri_Sportello_Passaggi_cod, objParametri_server, objParametri_utenti, ApplicaFiltroUtente, stb)


            If DataCorrente <> AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAFINE Then
                stb.Append(" and " & Agro_SQL_SaveDate(DataCorrente) & " <= ssp.Data_Fine " & vbCrLf)
                stb.Append(" and " & Agro_SQL_SaveDate(DataCorrente) & " >= ssp.Data_Inizio " & vbCrLf)
                stb.Append(" and CONVERT(DateTime,'2100/12/31',120) <> ssp.Data_Fine " & vbCrLf)
            Else
                stb.Append(" and " & Agro_SQL_SaveDate(DataCorrente) & " = ssp.Data_Fine " & vbCrLf)
            End If


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_server, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return dt
    End Function

    Public Function Leggi_Sementieri_Sportello_Configurazione(
           ByVal Sementieri_Sportello_Configurazione_Cod As Integer,
           ByVal DataCorrente As DateTime,
           ByVal xOrderBy As String,
           ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri
       ) As DataTable

        Dim NomeRoutine As String = "Sportello_R.Sportello_LeggiXData()"
        Dim MessaggioErrore As String = ""

        Dim dt As DataTable
        Dim stb As New StringBuilder


        Try

            stb.Append(" Select * " & vbCrLf)

            stb.Append(" from dbo.Sementieri_Sportello_Configurazione c " & vbCrLf)

            stb.Append(" where 1=1 " & vbCrLf)

            If Sementieri_Sportello_Configurazione_Cod <> 0 Then
                stb.Append(" and Sementieri_Sportello_Configurazione_Cod = " & Agro_SQL_SaveNum(Sementieri_Sportello_Configurazione_Cod) & vbCrLf)
            End If

            If DataCorrente <> AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO Then
                stb.Append(" and " & Agro_SQL_SaveDate(DataCorrente) & " <= p.Data_Fine " & vbCrLf)
                stb.Append(" and " & Agro_SQL_SaveDate(DataCorrente) & " >= p.Data_Inizio " & vbCrLf)
            End If


            If Not String.IsNullOrEmpty(xOrderBy) Then

                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_server))

            End If
            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_server, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return dt
    End Function

    Public Function Leggi_SportelloxSpecie(
        ByVal Sementieri_Sportello_Configurazione_Cod As Integer,
        ByVal ID_Specie As Integer,
        ByVal ID_SottoSpecie As Integer,
        ByVal ID_Gruppo As Integer,
        ByVal ID_Genotipo As Integer,
        ByVal Veg_Cod As Integer,
        ByVal xOrderBy As String,
        ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        Dim NomeRoutine As String = "Sportello_R.Sportello_LeggiXData()"
        Dim MessaggioErrore As String = ""

        Dim dt As DataTable
        Dim stb As New StringBuilder


        Try

            stb.Append(" Select distinct " & vbCrLf)
            stb.Append("     Sementieri_Sportello_ConfigurazioneXmappatura_specie.Sementieri_Sportello_Configurazione_cod,  " & vbCrLf)
            stb.Append("     Sementieri_Sportello_ConfigurazioneXmappatura_specie.ID_Specie, Sementieri_Sportello_ConfigurazioneXmappatura_specie.ID_SottoSpecie, " & vbCrLf)
            stb.Append("     Sementieri_Sportello_ConfigurazioneXmappatura_specie.ID_Gruppo, Sementieri_Sportello_ConfigurazioneXmappatura_specie.ID_Genotipo, " & vbCrLf)
            stb.Append("     Mappatura_Specie.Grva_Cod, Mappatura_Specie.Raggruppamento, Mappatura_Specie.Hybrid, SpecieVegetali.Veg_Cod_AUX, SpecieVegetali.Veg_Des, " & vbCrLf)
            stb.Append("     SpecieVegetali.Veg_Des_Lat, SpecieVegetali.Grsp_Cod, SpecieVegetali.Gru_Cod, SpecieVegetali.Bayer_SpecieVegetale, GruppoVarietale.Grva_Des, " & vbCrLf)
            stb.Append("     SpecieVegetali.Veg_Cod" & vbCrLf)

            stb.Append(" from dbo.Sementieri_Sportello_ConfigurazioneXmappatura_specie  " & vbCrLf)
            stb.Append("    inner join mappatura_specie " & vbCrLf)
            stb.Append("        on Sementieri_Sportello_ConfigurazioneXmappatura_specie.ID_Specie  = mappatura_specie.ID_Specie  " & vbCrLf)
            stb.Append("             and Sementieri_Sportello_ConfigurazioneXmappatura_specie.ID_SottoSpecie = mappatura_specie.ID_SottoSpecie   " & vbCrLf)
            stb.Append("             and   Sementieri_Sportello_ConfigurazioneXmappatura_specie.ID_Gruppo = mappatura_specie.ID_Gruppo   " & vbCrLf)
            stb.Append("             and Sementieri_Sportello_ConfigurazioneXmappatura_specie.ID_Genotipo  = mappatura_specie.ID_Genotipo   " & vbCrLf)

            stb.Append("    inner join SpecieVegetali  " & vbCrLf)
            stb.Append("        on mappatura_specie.veg_cod = SpecieVegetali.veg_cod   " & vbCrLf)

            stb.Append("    left join GruppoVarietale  " & vbCrLf)
            stb.Append("        on mappatura_specie.Grva_cod = GruppoVarietale.Grva_cod   " & vbCrLf)


            stb.Append(" where 1=1 " & vbCrLf)

            If Sementieri_Sportello_Configurazione_Cod <> 0 Then
                stb.Append(" and Sementieri_Sportello_ConfigurazioneXmappatura_specie.Sementieri_Sportello_Configurazione_Cod = " & Agro_SQL_SaveNum(Sementieri_Sportello_Configurazione_Cod) & vbCrLf)
            End If






            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_server, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return dt
    End Function

    Public Function Leggi_Gru_Cod(
        ByVal Sementieri_Sportello_Configurazione_Cod As Integer,
        ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        Dim NomeRoutine = "Sportello_R.Leggi_Gru_Cod()"
        Dim MessaggioErrore = ""

        Dim dt As DataTable
        Dim stb As New StringBuilder

        Try
            stb.Append(" SELECT TOP 1 SV.Gru_Cod " & vbCrLf)
            stb.Append(" FROM Sementieri_Sportello_ConfigurazioneXmappatura_specie SP " & vbCrLf)
            stb.Append(" INNER JOIN Mappatura_Specie MS " & vbCrLf)
            stb.Append("     ON SP.ID_Specie = MS.ID_Specie AND " & vbCrLf)
            stb.Append("     SP.ID_SottoSpecie = MS.ID_SottoSpecie AND " & vbCrLf)
            stb.Append("     SP.ID_Gruppo = MS.ID_Gruppo AND " & vbCrLf)
            stb.Append("     SP.ID_SottoSpecie = MS.ID_SottoSpecie AND " & vbCrLf)
            stb.Append("     SP.ID_Genotipo = MS.ID_Genotipo " & vbCrLf)
            stb.Append(" INNER JOIN SpecieVegetali SV " & vbCrLf)
            stb.Append("     ON SV.Veg_Cod = MS.Veg_Cod " & vbCrLf)
            stb.Append(" WHERE SP.Sementieri_Sportello_Configurazione_cod = " & Agro_SQL_SaveNum(Sementieri_Sportello_Configurazione_Cod) & vbCrLf)

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_server, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return dt
    End Function


    Public Function LeggiFaseSportelloXSpecie(ByVal VegCod As Integer, ByVal DataOra As DateTime, ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "Sportello_R.LeggiFaseSportelloXSpecie()"
        Dim MessaggioErrore As String = ""

        Dim DT As DataTable
        Dim stb As New StringBuilder


        Try

            stb.AppendLine("SELECT DISTINCT ")
            stb.AppendLine("	cxp.Data_Inizio ")
            stb.AppendLine("	, cxp.Data_Fine ")
            stb.AppendLine("	, p.Sementieri_Sportello_Passaggi_des ")
            stb.AppendLine("	, p.DestinazioneSalvataggio ")
            stb.AppendLine("	, p.Visibilita_Impianti ")
            stb.AppendLine("	, p.LoggaOperazioni ")
            stb.AppendLine("	, p.RichiediConfermaSuInterferenze ")
            stb.AppendLine("FROM Sementieri_Sportello_Configurazione c ")
            stb.AppendLine("INNER JOIN Sementieri_Sportello_ConfigurazioneXPassaggi cxp ON cxp.Sementieri_Sportello_Configurazione_cod = c.Sementieri_Sportello_Configurazione_cod ")
            stb.AppendLine("INNER JOIN Sementieri_Sportello_Passaggi p ON p.Sementieri_Sportello_Passaggi_cod = cxp.Sementieri_Sportello_Passaggi_cod ")
            stb.AppendLine("INNER JOIN Sementieri_Sportello_ConfigurazioneXmappatura_specie cxms ON cxms.Sementieri_Sportello_Configurazione_cod = c.Sementieri_Sportello_Configurazione_cod ")
            stb.AppendLine("INNER JOIN Mappatura_Specie ms ON ms.ID_Specie = cxms.ID_Specie ")
            stb.AppendLine("WHERE cxp.Data_Inizio <= " & Agro_SQL_SaveDate(DataOra))
            stb.AppendLine("AND " & Agro_SQL_SaveDate(DataOra) & " <= cxp.Data_Fine ")
            stb.AppendLine("AND Veg_Cod = " & VegCod.ToString)


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_server, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT
    End Function

    Public Function Leggi_MappaturaSpecie(
     ByVal ID_Specie As Integer,
     ByVal ID_SottoSpecie As Integer,
     ByVal ID_Gruppo As Integer,
     ByVal ID_Genotipo As Integer,
     ByVal Veg_Cod As Integer,
     ByVal xOrderBy As String,
     ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri
 ) As DataTable

        Dim NomeRoutine As String = "Sportello_R.Sportello_LeggiXData()"
        Dim MessaggioErrore As String = ""

        Dim dt As DataTable
        Dim stb As New StringBuilder


        Try

            stb.Append(" Select distinct " & vbCrLf)
            stb.Append("    mappatura_specie.*  " & vbCrLf)
            stb.Append("    , SpecieVegetali.* " & vbCrLf)
            stb.Append("    , GruppoVarietale.Grva_Des  " & vbCrLf)

            stb.Append(" from   " & vbCrLf)
            stb.Append("    mappatura_specie " & vbCrLf)
            stb.Append("    inner join SpecieVegetali  " & vbCrLf)
            stb.Append("        on mappatura_specie.veg_cod = SpecieVegetali.veg_cod   " & vbCrLf)

            stb.Append("    left join GruppoVarietale  " & vbCrLf)
            stb.Append("        on mappatura_specie.Grva_cod = GruppoVarietale.Grva_cod   " & vbCrLf)

            stb.Append(" where 1=1 " & vbCrLf)








            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_server, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return dt
    End Function

    Public Function LeggiClassiDiSpecieXSportello(ByVal Sementieri_Sportello_Configurazione_Cod As Integer, ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "Sportello_R.LeggiClassiDiSpecieXSportello()"
        Dim MessaggioErrore As String = ""

        Dim dt As DataTable
        Dim stb As New StringBuilder


        Try

            stb.AppendLine("SELECT DISTINCT ")
            stb.AppendLine("	css.ID_specie AS IdSpecie")
            stb.AppendLine("	, css.Sementieri_ClassiDiSpecieVegetali_des AS DesSpecie ")
            stb.AppendLine("FROM Mappatura_Specie ms ")
            stb.AppendLine("INNER JOIN Sementieri_ClassiDiSpecieVegetali css ON css.ID_Specie = ms.ID_Specie ")
            If Sementieri_Sportello_Configurazione_Cod > 0 Then
                stb.AppendLine("INNER JOIN Sementieri_Sportello_ConfigurazioneXmappatura_specie cms ON cms.ID_Specie = ms.ID_Specie ")
                stb.AppendLine("WHERE Sementieri_Sportello_Configurazione_cod = " & Sementieri_Sportello_Configurazione_Cod.ToString)
            End If
            stb.AppendLine("ORDER BY DesSpecie ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_server, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return dt
    End Function

    Function Leggi_SportelloxPassaggi(
        ByVal Sementieri_Sportello_Configurazione_Cod As Integer,
        ByVal Sementieri_Sportello_Passaggi_cod As Integer,
        ByVal Ordine As String,
        ByVal xOrderBy As String,
        ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        Dim NomeRoutine As String = "Sportello_R.Sportello_LeggiXData()"
        Dim MessaggioErrore As String = ""

        Dim dt As DataTable
        Dim stb As New StringBuilder


        Try

            stb.Append(" Select distinct " & vbCrLf)
            stb.Append("    Sementieri_Sportello_ConfigurazioneXPassaggi.Sementieri_Sportello_Configurazione_cod, " & vbCrLf)
            stb.Append("    Sementieri_Sportello_ConfigurazioneXPassaggi.Sementieri_Sportello_Passaggi_cod, Sementieri_Sportello_ConfigurazioneXPassaggi.Ordine, " & vbCrLf)
            stb.Append("    Sementieri_Sportello_Passaggi.Sementieri_Sportello_Passaggi_des, Sementieri_Sportello_ConfigurazioneXPassaggi.Data_Fine, " & vbCrLf)
            stb.Append("    Sementieri_Sportello_Passaggi.DestinazioneSalvataggio, Sementieri_Sportello_Passaggi.RichiediConfermaSuInterferenze, " & vbCrLf)
            stb.Append("    Sementieri_Sportello_Passaggi.LoggaOperazioni, Sementieri_Sportello_Passaggi.VisualizzaOperazioniLoggate, Sementieri_Sportello_ConfigurazioneXPassaggi.Data_Inizio, " & vbCrLf)
            stb.Append("    Sementieri_Sportello_Passaggi.Visibilita_Impianti" & vbCrLf)

            stb.Append(" from dbo.Sementieri_Sportello_ConfigurazioneXpassaggi  " & vbCrLf)
            stb.Append("    inner join Sementieri_Sportello_passaggi " & vbCrLf)
            stb.Append("        on Sementieri_Sportello_ConfigurazioneXpassaggi.Sementieri_Sportello_Passaggi_cod  = Sementieri_Sportello_passaggi.Sementieri_Sportello_Passaggi_cod  " & vbCrLf)


            stb.Append(" where 1=1 " & vbCrLf)

            If Sementieri_Sportello_Configurazione_Cod <> 0 Then
                stb.Append(" and Sementieri_Sportello_ConfigurazioneXpassaggi.Sementieri_Sportello_Configurazione_Cod = " & Agro_SQL_SaveNum(Sementieri_Sportello_Configurazione_Cod) & vbCrLf)
            End If


            If xOrderBy <> "" Then
                stb.Append(xOrderBy)
            End If


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_server, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return dt
    End Function


    Function Leggi_Passaggi(
     ByVal Sementieri_Sportello_Passaggi_cod As Integer,
    ByVal Ordine As String,
    ByVal xOrderBy As String,
    ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri
) As DataTable

        Dim NomeRoutine As String = "Sportello_R.Sportello_LeggiXData()"
        Dim MessaggioErrore As String = ""

        Dim dt As DataTable
        Dim stb As New StringBuilder


        Try

            stb.Append(" Select distinct " & vbCrLf)
            stb.Append("     Sementieri_Sportello_passaggi.*  " & vbCrLf)

            stb.Append(" from Sementieri_Sportello_passaggi " & vbCrLf)

            stb.Append(" where 1=1 " & vbCrLf)

            If Sementieri_Sportello_Passaggi_cod <> 0 Then
                stb.Append(" and Sementieri_Sportello_passaggi.Sementieri_Sportello_Passaggi_cod = " & Agro_SQL_SaveNum(Sementieri_Sportello_Passaggi_cod) & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_server, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return dt
    End Function

    Public Function PossoCancellare(ByVal id_sportello As Integer, ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "Sportello_R.PossoCancellare()"
        Dim MessaggioErrore As String = ""

        Dim result As Boolean = False

        Try

            Dim stb As New StringBuilder

            stb.AppendLine("SELECT COUNT(*) AS cnt ")
            stb.AppendLine("FROM Sementieri_Sportello_LogOperazioni ")
            stb.AppendLine("WHERE Sementieri_Sportello_Configurazione_cod = " & id_sportello.ToString)

            '--------------------------------------------------------------------------
            Dim dt = EseguiQuery_Lettura(objParametri_server, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
            If dt.Rows.Count > 0 Then
                result = CInt(dt.Rows(0)("cnt")) = 0
            End If

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            result = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return result
    End Function

End Class


Public Class Sportello_W
    Inherits AgronicaCoreDataProvider.DataProvider


    '//////////////////////////////////////////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////
    Public Function Scrivi_Sementieri_Sportello_Passaggi(
                            ByVal Sementieri_Sportello_Passaggi_cod As Integer,
                            ByVal Sementieri_Sportello_Passaggi_des As String,
                                ByVal Visibilita_Impianti As Integer,
                                ByVal DestinazioneSalvataggio As Integer,
                                ByVal RichiediConfermaSuInterferenze As Integer,
                                ByVal LoggaOperazioni As Integer,
                                ByVal VisualizzaOperazioniLoggate As Integer,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Const NomeRoutine = "AgronicaCoreSementieriDAL.Sportello_W.Scrivi_Sementieri_Sportello_Passaggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False


        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO Sementieri_Sportello_Passaggi ")
            StrSQL.Append("                   ( ")
            StrSQL.Append("                    Sementieri_Sportello_Passaggi_cod,           ")
            StrSQL.Append("                    Sementieri_Sportello_Passaggi_des,           ")

            StrSQL.Append("                    Visibilita_Impianti,            ")
            StrSQL.Append("                    DestinazioneSalvataggio, ")
            StrSQL.Append("                    RichiediConfermaSuInterferenze,      ")
            StrSQL.Append("                    LoggaOperazioni, VisualizzaOperazioniLoggate ")

            StrSQL.Append("                   ) ")

            StrSQL.Append("VALUES (")

            StrSQL.Append("         " & Agro_SQL_SaveNum(Sementieri_Sportello_Passaggi_cod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Sementieri_Sportello_Passaggi_des) & "'  ")

            StrSQL.Append("         ," & Agro_SQL_SaveNum(Visibilita_Impianti) & "  ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(DestinazioneSalvataggio) & "  ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(RichiediConfermaSuInterferenze) & "  ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(LoggaOperazioni) & "  ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(VisualizzaOperazioniLoggate) & "  ")


            StrSQL.Append(" )")
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





    '//////////////////////////////////////////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////
    Public Function Scrivi_Sementieri_Sportello_Configurazione(
                            ByVal Sementieri_Sportello_Configurazione_cod As Int32,
                            ByVal Sementieri_Sportello_Configurazione_des As String,
                                ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Const NomeRoutine = "AgronicaCoreSementieriDAL.Sportello_W.Scrivi_Sementieri_Sportello_Configurazione()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False


        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO Sementieri_Sportello_Configurazione ")
            StrSQL.Append("                   ( ")
            StrSQL.Append("                    Sementieri_Sportello_Configurazione_cod,           ")
            StrSQL.Append("                    Sementieri_Sportello_Configurazione_des,           ")

            StrSQL.Append("                    Inviato,            ")
            StrSQL.Append("                    DataInvio, ")
            StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                    UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("                    Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("                   ) ")

            StrSQL.Append("VALUES (")

            StrSQL.Append("         " & Agro_SQL_SaveNum(Sementieri_Sportello_Configurazione_cod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Sementieri_Sportello_Configurazione_des) & "'  ")

            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append(" )")
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



    '//////////////////////////////////////////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////
    Public Function Scrivi_Sementieri_Sportello_ConfigurazioneXmappatura_specie(
                            ByVal Sementieri_Sportello_Configurazione_cod As Int32,
                            ByVal ID_Specie As Int32,
                            ByVal ID_SottoSpecie As Int32,
                            ByVal ID_Gruppo As Int32,
                            ByVal ID_Genotipo As Int32,
                                ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Const NomeRoutine = "AgronicaCoreSementieriDAL.Sportello_W.Scrivi_Sementieri_Sportello_ConfigurazioneXmappatura_specie()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False


        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO Sementieri_Sportello_ConfigurazioneXmappatura_specie ")
            StrSQL.Append("                   ( ")
            StrSQL.Append("                    Sementieri_Sportello_Configurazione_cod,           ")
            StrSQL.Append("                    ID_Specie,      ID_SottoSpecie,    ")
            StrSQL.Append("                    ID_Gruppo,              ID_Genotipo,    ")

            StrSQL.Append("                    Inviato,            DataInvio, ")
            StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                    UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("                    Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("                   ) ")

            StrSQL.Append("VALUES (")

            StrSQL.Append("         " & Agro_SQL_SaveNum(Sementieri_Sportello_Configurazione_cod) & "  ")

            StrSQL.Append("         , " & Agro_SQL_SaveNum(ID_Specie) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(ID_SottoSpecie) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(ID_Gruppo) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(ID_Genotipo) & "  ")

            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append(" )")
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

    Public Function Scrivi_Sementieri_Sportello_ConfigurazioneXMappatura_Specie_(
                                                                                ByVal Sementieri_Sportello_Configurazione_cod As Int32,
                                                                                ByVal ID_Specie As Int32,
                                                                                ByVal Validita_Inizio As Date,
                                                                                ByVal Validita_Fine As Date,
                                                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                                ) As Boolean

        Dim NomeRoutine As String = "Sportello_W.GIS_Entita_W.Scrivi_Sementieri_Sportello_ConfigurazioneXMappatura_Specie_()"

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
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine("INSERT INTO Sementieri_Sportello_ConfigurazioneXmappatura_specie ")
            StrSQL.AppendLine("SELECT ")
            StrSQL.AppendLine(Sementieri_Sportello_Configurazione_cod.ToString & " AS Sementieri_Sportello_Configurazione_cod ")
            StrSQL.AppendLine(", ID_Specie ")
            StrSQL.AppendLine(", ID_SottoSpecie ")
            StrSQL.AppendLine(", ID_Gruppo ")
            StrSQL.AppendLine(", ID_Genotipo ")
            StrSQL.AppendLine(", 0 AS inviato")
            StrSQL.AppendLine(", NULL AS datainvio")
            StrSQL.AppendLine(", " & Agro_SQL_SaveDate(Date.Now) & " AS Data_Creazione ")
            StrSQL.AppendLine(", " & Agro_SQL_SaveDate(Date.Now) & " AS Data_Modifica ")
            StrSQL.AppendLine(",'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' AS Username_Creazione ")
            StrSQL.AppendLine(",'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' AS Username_Modifica ")
            StrSQL.AppendLine(", " & Agro_SQL_SaveDate(Validita_Inizio) & " AS Validita_Inizio ")
            StrSQL.AppendLine(", " & Agro_SQL_SaveDate(Validita_Fine) & " AS Validita_Fine ")
            StrSQL.AppendLine("FROM Mappatura_Specie ")
            StrSQL.AppendLine("WHERE ID_Specie = " & ID_Specie.ToString)

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

    '//////////////////////////////////////////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////
    Public Function Scrivi_Sementieri_Sportello_ConfigurazioneXpassaggi(
                            ByVal Sementieri_Sportello_Configurazione_cod As Int32,
                            ByVal Sementieri_Sportello_Passaggi_cod As Int32,
                            ByVal Ordine As Int32,
                            ByVal Data_Inizio As String,
                            ByVal Data_Fine As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Const NomeRoutine = "AgronicaCoreSementieriDAL.Sportello_W.Scrivi_Sementieri_Sportello_ConfigurazioneXpassaggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False


        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO Sementieri_Sportello_ConfigurazioneXpassaggi ")
            StrSQL.Append("                   ( ")
            StrSQL.Append("                    Sementieri_Sportello_Configurazione_cod,           ")
            StrSQL.Append("                    Sementieri_Sportello_Passaggi_cod,          ")
            StrSQL.Append("                    Ordine,     ")
            If IsDate(Data_Inizio) Then
                StrSQL.Append("                    Data_Inizio,     ")
            End If
            If IsDate(Data_Fine) Then
                StrSQL.Append("                    Data_Fine     ")
            End If
            StrSQL.Append("                   ) ")

            StrSQL.Append("VALUES (")

            StrSQL.Append("          " & Agro_SQL_SaveNum(Sementieri_Sportello_Configurazione_cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sementieri_Sportello_Passaggi_cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Ordine) & "  ")
            If IsDate(Data_Inizio) Then
                StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_Inizio) & "  ")
            End If
            If IsDate(Data_Fine) Then
                StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_Fine) & "  ")
            End If

            StrSQL.Append(" )")
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





    '//////////////////////////////////////////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////
    'cancella Sementieri_Sportello_Configurazione
    Public Function MODIFICA_Sementieri_Sportello_Configurazione(
                            ByVal Sementieri_Sportello_Configurazione_cod As Int32,
                            ByVal Sementieri_Sportello_Configurazione_des As String,
                                ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreSementieriDAL.Sportello_W.Cancella_Sementieri_Sportello_Configurazione()"

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
            '---------------------------------------------

            StrSQL.Length = 0
            StrSQL.Append("UPDATE Sementieri_Sportello_Configurazione SET ")
            StrSQL.Append("    Sementieri_Sportello_Configurazione_des           =  '" & Agro_SQL_SaveText(Sementieri_Sportello_Configurazione_des) & "' ")


            StrSQL.Append("   ,Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append(" WHERE Sementieri_Sportello_Configurazione_cod = " & Agro_SQL_SaveNum(Sementieri_Sportello_Configurazione_cod))

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



    '//////////////////////////////////////////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////
    'cancella Sementieri_Sportello_Configurazione
    Public Function MODIFICA_Sementieri_Sportello_Passaggi(
                             ByVal Sementieri_Sportello_Passaggi_cod As Integer,
                            ByVal Sementieri_Sportello_Passaggi_des As String,
                                ByVal Visibilita_Impianti As Integer,
                                ByVal DestinazioneSalvataggio As Integer,
                                ByVal RichiediConfermaSuInterferenze As Integer,
                                ByVal LoggaOperazioni As Integer,
                                ByVal VisualizzaOperazioniLoggate As Integer,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreSementieriDAL.Sportello_W.MODIFICA_Sementieri_Sportello_Passaggi()"

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
            '---------------------------------------------

            StrSQL.Length = 0
            StrSQL.Append("UPDATE Sementieri_Sportello_Passaggi SET ")
            StrSQL.Append("    Sementieri_Sportello_Passaggi_des           =  '" & Agro_SQL_SaveText(Sementieri_Sportello_Passaggi_des) & "' ")

            StrSQL.Append("   ,Visibilita_Impianti     =  " & Agro_SQL_SaveNum(Visibilita_Impianti))
            StrSQL.Append("   ,DestinazioneSalvataggio     =  " & Agro_SQL_SaveNum(DestinazioneSalvataggio))
            StrSQL.Append("   ,RichiediConfermaSuInterferenze     =  " & Agro_SQL_SaveNum(RichiediConfermaSuInterferenze))
            StrSQL.Append("   ,LoggaOperazioni     =  " & Agro_SQL_SaveNum(LoggaOperazioni))
            StrSQL.Append("   ,VisualizzaOperazioniLoggate     =  " & Agro_SQL_SaveNum(VisualizzaOperazioniLoggate))


            StrSQL.Append(" WHERE Sementieri_Sportello_Passaggi_cod = " & Agro_SQL_SaveNum(Sementieri_Sportello_Passaggi_cod))

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





    '//////////////////////////////////////////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////
    'cancella Sementieri_Sportello_Configurazione
    Public Function Cancella_Sementieri_Sportello_Passaggi(
                            ByVal Sementieri_Sportello_Passaggi_cod As Int32,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreSementieriDAL.Sportello_W.Cancella_Sementieri_Sportello_Configurazione()"

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

            If Sementieri_Sportello_Passaggi_cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Sementieri_Sportello_Passaggi_cod obbligatorio)")
            End If

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Sementieri_Sportello_Passaggi ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE Sementieri_Sportello_Passaggi_cod = " & Agro_SQL_SaveNum(Sementieri_Sportello_Passaggi_cod))
                StrSQL.Append(" AND Inviato >= 0")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     Sementieri_Sportello_Passaggi ")
                StrSQL.Append(" WHERE Sementieri_Sportello_Passaggi_cod = " & Agro_SQL_SaveNum(Sementieri_Sportello_Passaggi_cod))

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





    '//////////////////////////////////////////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////
    'cancella Sementieri_Sportello_Configurazione
    Public Function Cancella_Sementieri_Sportello_Configurazione(
                            ByVal Sementieri_Sportello_Configurazione_Cod As Int32,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreSementieriDAL.Sportello_W.Cancella_Sementieri_Sportello_Configurazione()"

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

            If Sementieri_Sportello_Configurazione_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Sementieri_Sportello_Configurazione_Cod obbligatorio)")
            End If

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Sementieri_Sportello_Configurazione ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Sementieri_Sportello_Configurazione_Cod      =   " & Agro_SQL_SaveNum(Sementieri_Sportello_Configurazione_Cod) & "  ")
                StrSQL.Append(" AND Inviato >= 0")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     Sementieri_Sportello_Configurazione ")
                StrSQL.Append(" WHERE  Sementieri_Sportello_Configurazione_Cod      =   " & Agro_SQL_SaveNum(Sementieri_Sportello_Configurazione_Cod) & "  ")

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

    '//////////////////////////////////////////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////
    'cancella Sementieri_Sportello_Configurazione
    Public Function Cancella_Sementieri_Sportello_ConfigurazioneXmappatura_specie(
                            ByVal Sementieri_Sportello_Configurazione_Cod As Int32,
                            ByVal ID_Specie As Int32,
                            ByVal ID_SottoSpecie As Int32,
                            ByVal ID_Gruppo As Int32,
                            ByVal ID_Genotipo As Int32,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreSementieriDAL.Sportello_W.Cancella_Sementieri_Sportello_ConfigurazioneXmappatura_specie()"

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

            If Sementieri_Sportello_Configurazione_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Sportello_ConfigurazioneXmappatura_specie obbligatorio)")
            End If

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Sementieri_Sportello_ConfigurazioneXmappatura_specie ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Sementieri_Sportello_Configurazione_Cod      =   " & Agro_SQL_SaveNum(Sementieri_Sportello_Configurazione_Cod) & "  ")
                If ID_Specie <> 0 Then
                    StrSQL.Append(" AND  ID_Specie      =   " & Agro_SQL_SaveNum(ID_Specie) & "  ")
                End If
                If ID_SottoSpecie <> 0 Then
                    StrSQL.Append(" AND  ID_SottoSpecie      =   " & Agro_SQL_SaveNum(ID_SottoSpecie) & "  ")
                End If
                If ID_Gruppo <> 0 Then
                    StrSQL.Append(" AND  ID_Gruppo      =   " & Agro_SQL_SaveNum(ID_Gruppo) & "  ")
                End If
                If ID_Genotipo <> 0 Then
                    StrSQL.Append(" AND  ID_Genotipo      =   " & Agro_SQL_SaveNum(ID_Genotipo) & "  ")
                End If
                StrSQL.Append(" AND Inviato >= 0")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     Sementieri_Sportello_ConfigurazioneXmappatura_specie ")
                StrSQL.Append(" WHERE  Sementieri_Sportello_Configurazione_Cod      =   " & Agro_SQL_SaveNum(Sementieri_Sportello_Configurazione_Cod) & "  ")
                If ID_Specie <> 0 Then
                    StrSQL.Append(" AND  ID_Specie      =   " & Agro_SQL_SaveNum(ID_Specie) & "  ")
                End If
                If ID_SottoSpecie <> 0 Then
                    StrSQL.Append(" AND  ID_SottoSpecie      =   " & Agro_SQL_SaveNum(ID_SottoSpecie) & "  ")
                End If
                If ID_Gruppo <> 0 Then
                    StrSQL.Append(" AND  ID_Gruppo      =   " & Agro_SQL_SaveNum(ID_Gruppo) & "  ")
                End If
                If ID_Genotipo <> 0 Then
                    StrSQL.Append(" AND  ID_Genotipo      =   " & Agro_SQL_SaveNum(ID_Genotipo) & "  ")
                End If

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

    '//////////////////////////////////////////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////
    'cancella Sementieri_Sportello_Configurazione
    Public Function Cancella_Sementieri_Sportello_ConfigurazioneXpassaggi(
                            ByVal Sementieri_Sportello_Configurazione_cod As Int32,
                            ByVal Sementieri_Sportello_Passaggi_cod As Int32,
                            ByVal Ordine As Int32,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreSementieriDAL.Sportello_W.Cancella_Sementieri_Sportello_ConfigurazioneXpassaggi()"

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

            If Sementieri_Sportello_Configurazione_cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Sementieri_Sportello_Configurazione_Cod obbligatorio)")
            End If

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Sementieri_Sportello_ConfigurazioneXpassaggi ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Sementieri_Sportello_Configurazione_Cod      =   " & Agro_SQL_SaveNum(Sementieri_Sportello_Configurazione_cod) & "  ")
                If Sementieri_Sportello_Passaggi_cod <> 0 Then
                    StrSQL.Append(" AND  Sementieri_Sportello_Passaggi_cod      =   " & Agro_SQL_SaveNum(Sementieri_Sportello_Passaggi_cod) & "  ")
                End If
                If Sementieri_Sportello_Passaggi_cod <> 0 Then
                    StrSQL.Append(" AND  Sementieri_Sportello_Passaggi_cod      =   " & Agro_SQL_SaveNum(Sementieri_Sportello_Passaggi_cod) & "  ")
                End If
                StrSQL.Append(" AND Inviato >= 0")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     Sementieri_Sportello_ConfigurazioneXpassaggi ")
                StrSQL.Append(" WHERE  Sementieri_Sportello_Configurazione_Cod      =   " & Agro_SQL_SaveNum(Sementieri_Sportello_Configurazione_cod) & "  ")
                If Sementieri_Sportello_Passaggi_cod <> 0 Then
                    StrSQL.Append(" AND  Sementieri_Sportello_Passaggi_cod      =   " & Agro_SQL_SaveNum(Sementieri_Sportello_Passaggi_cod) & "  ")
                End If
                If Sementieri_Sportello_Passaggi_cod <> 0 Then
                    StrSQL.Append(" AND  Sementieri_Sportello_Passaggi_cod      =   " & Agro_SQL_SaveNum(Sementieri_Sportello_Passaggi_cod) & "  ")
                End If

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