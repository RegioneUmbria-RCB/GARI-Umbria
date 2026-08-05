
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports System.Text

Public Class Meteo_R
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Agronica_Stazioni_Appoggio_Leggi_Con_Distanza(
        ByVal piva_SuperUser As String,
        ByVal Piva As String,
        ByVal Utente_Username_client_GIAS As String,
        ByVal nome As String,
        ByVal Long_Centro As Decimal,
        ByVal Lat_Centro As Decimal,
        ByVal TipoSorgente As enum_Meteo_Tiposorgente,
        ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        Dim NomeRoutine As String = "Agronica_Stazioni_Appoggio_Leggi_Con_Distanza"
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable



        Try

            Stb.Length = 0

            'valido per tutte le origini dati
            Stb.AppendLine(" select Metos_Stazioni_Appoggio.* , a.distanza, a.[f_longitude],a.[f_latitude]  ")
            Stb.AppendLine("  , CASE WHEN a.distanza IS NULL THEN 1 ELSE 0 END AS ordine")

            Stb.AppendLine(" from ")
            Stb.AppendLine(" ( ")
            Stb.AppendLine(" select *,")
            If Lat_Centro <> 0 AndAlso Long_Centro <> 0 Then
                Stb.AppendLine("     geography::STGeomFromText( ")
                Stb.AppendLine("     'POINT(' + ")
                Stb.AppendLine("         replace(replace([f_longitude], 'E', ''), ',', '.')      + ' ' +  ")
                Stb.AppendLine("         replace(replace([f_latitude], 'N', ''), ',', '.')       ")
                Stb.AppendLine("     + ')' , 4326 ).STDistance(  geography::STGeomFromText('POINT(" & Agro_SQL_SaveNum(Long_Centro) & " " & Agro_SQL_SaveNum(Lat_Centro) & ")', 4326 ) ) as distanza ")
            Else
                Stb.AppendLine("   NULL as distanza ")
            End If


            Select Case TipoSorgente

                Case enum_Meteo_Tiposorgente.RetiPartner
                    Stb.AppendLine("  FROM ( ")
                    StazioniAppoggioQry(piva_SuperUser, False, True, False, Stb)
                    Stb.AppendLine("  ) Metos_Sazioni ")

                Case enum_Meteo_Tiposorgente.Gias_RER
                    Stb.AppendLine("  FROM ( ")
                    Stb.AppendLine("   select S.ID_Stazione as nome, Q.X_LON_GC as f_latitude, Q.Y_LAT_GC as f_longitude, '' as Stazione_Cod_Fornitore ")
                    Stb.AppendLine("   from TB_Stazioni S ")
                    Stb.AppendLine("      inner join TB_QuadrantixStazioni QS ")
                    Stb.AppendLine("          on S.ID_Stazione = QS.ID_Stazione ")
                    Stb.AppendLine("      inner join TB_Quadranti Q ")
                    Stb.AppendLine("          on Q.ID_Quadrante = QS.ID_Quadrante ")
                    Stb.AppendLine("  ) Metos_Stazioni")

                Case enum_Meteo_Tiposorgente.Gias_RER_Quadranti
                    Stb.AppendLine("  FROM ( ")
                    Stb.AppendLine("   select Q.ID_Quadrante as nome, Q.X_LON_GC as f_latitude, Q.Y_LAT_GC as f_longitude, '' as Stazione_Cod_Fornitore ")
                    Stb.AppendLine("   from TB_Quadranti Q ")
                    Stb.AppendLine("  ) Metos_Stazioni")


                Case enum_Meteo_Tiposorgente.Aziendali
                    Stb.AppendLine("  FROM ( ")
                    StazioniAppoggioQry(piva_SuperUser, Piva, Utente_Username_client_GIAS, False, True, False, Stb)
                    Stb.AppendLine("  ) Metos_Sazioni ")

                Case Else
                    Stb.AppendLine("  FROM ( ")
                    StazioniAppoggioQry(piva_SuperUser, False, True, False, Stb)
                    Stb.AppendLine("  ) Metos_Sazioni ")

            End Select



            Stb.AppendLine("  where f_latitude <> ''    ")
            Stb.AppendLine("  and f_longitude <> ''   ")
            Stb.AppendLine("   ) a   ")

            Stb.AppendLine(" right join (  ")


            Select Case TipoSorgente
                Case enum_Meteo_Tiposorgente.RetiPartner

                    StazioniAppoggioQry(piva_SuperUser, True, True, True, Stb)

                Case enum_Meteo_Tiposorgente.Gias_RER

                    Stb.AppendLine("  select id_stazione as nome, Stazione_Des as descrizione, '01/01/1900' as data_ultimo_agg, '' as Stazione_Cod_Fornitore ")
                    Stb.AppendLine("  from TB_Stazioni ")

                Case enum_Meteo_Tiposorgente.Gias_RER_Quadranti

                    Stb.AppendLine("  select id_quadrante as nome, Quadrante_Des as descrizione, '01/01/1900' as data_ultimo_agg, '' as Stazione_Cod_Fornitore ")
                    Stb.AppendLine("  from TB_Quadranti ")

                Case enum_Meteo_Tiposorgente.Aziendali
                    StazioniAppoggioQry(piva_SuperUser, Piva, Utente_Username_client_GIAS, True, True, True, Stb)

                Case Else
                    StazioniAppoggioQry(piva_SuperUser, True, True, True, Stb)


            End Select

            Stb.AppendLine("  )  Metos_Stazioni_Appoggio    ")
            Stb.AppendLine(" on a.nome=Metos_Stazioni_Appoggio.nome     ")
            Stb.AppendLine(" WHERE   1=1  ")
            If nome <> "" Then
                Stb.AppendLine(" and nome  = " & Agro_SQL_SaveText_NULL(nome) & "  ")
            End If
            Stb.AppendLine(" order by ordine, distanza, Metos_Stazioni_Appoggio.descrizione     ")
            Stb.AppendLine("      ")




            '-----------------------------------------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '-----------------------------------------------------------------------------------------------------------

            Return DT

        Catch ex As Exception

            DT = Nothing
            MessaggioErrore = "[" & NomeRoutine & "]:" & ex.Message & " "
            Throw New Exception(MessaggioErrore)

        End Try

        Return Nothing

    End Function

    Public Function Agronica_Stazioni_Appoggio_Leggi(
            ByVal piva_SuperUser As String,
            ByVal nome As String,
            ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        Dim NomeRoutine As String = "Metos_Stazioni_Appoggio_Leggi"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM ( ")
            StazioniAppoggioQry(piva_SuperUser, True, False, False, StrSQL)
            StrSQL.Append(" ) a ")


            StrSQL.Append(" WHERE   1=1  ")
            If nome <> "" Then
                StrSQL.Append(" and nome  = " & Agro_SQL_SaveText_NULL(nome) & "  ")
            End If


            StrSQL.Append(" ORDER BY nome")

            '-----------------------------------------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '-----------------------------------------------------------------------------------------------------------

            Return DT

        Catch ex As Exception

            DT = Nothing
            MessaggioErrore = "[" & NomeRoutine & "]:" & ex.Message & " "
            Throw New Exception(MessaggioErrore)

        End Try

        Return Nothing
    End Function

    ''' <summary>
    ''' Versione per stazioni aziendali
    ''' </summary>
    ''' <param name="pivaSuperUser"></param>
    ''' <param name="piva"></param>
    ''' <param name="Username"></param>
    ''' <param name="LeggiDataUltimoAgg"></param>
    ''' <param name="LeggiPosizione"></param>
    ''' <param name="RicavaUltimaDataAggiornamento"></param>
    ''' <param name="stb"></param>    
    Private Shared Sub StazioniAppoggioQry(ByVal pivaSuperUser As String, ByVal piva As String, ByVal Username As String, ByVal LeggiDataUltimoAgg As Boolean, ByVal LeggiPosizione As Boolean, ByVal RicavaUltimaDataAggiornamento As Boolean, ByRef stb As StringBuilder)

        stb.AppendLine("  Select  ")
        stb.AppendLine("    z.Stazione_Cod as nome ")
        stb.AppendLine("  , '[' + FiltroForn.Fornitore_Des + '] ' + Stazione_Des as Descrizione  ")
        If LeggiPosizione Then
            stb.AppendLine("  , Posizione ")
            stb.AppendLine("  , PosizioneLatitudine as f_latitude ")
            stb.AppendLine("  , PosizioneLongitudine as f_longitude ")
        End If

        stb.AppendLine("  , Stazione_Cod_Fornitore  ")

        If LeggiDataUltimoAgg Then
            stb.AppendLine("  , isNull (  ")
            stb.AppendLine("              ( ")
            stb.AppendLine("                 Select top 1 cast(DataOraRilievo As Date) As UltimaDataRilievo  ")
            stb.AppendLine("                 From Agronica_Dati_Rilevati RR1  ")
            stb.AppendLine("                 Where RR1.Stazione_Cod = Z.Stazione_Cod ")
            stb.AppendLine("                 Order By DataOraRilievo desc ")
            stb.AppendLine("          ) ")
            stb.AppendLine("    , cast('01/01/1900' as date) ")
            stb.AppendLine("  ) as  data_ultimo_agg")
        End If


        stb.AppendLine(" ")

        stb.AppendLine(" From agronica_Stazioni z ")
        stb.AppendLine(" inner Join  ")
        stb.AppendLine(" ( ")
        stb.AppendLine("  Select ")
        stb.AppendLine("         f.Fornitore_Cod ")
        stb.AppendLine("      , z1.Stazione_Cod ")
        stb.AppendLine("      , isnull(opAutor.DescrizioneAggiuntiva, f.Fornitore_Des) as   Fornitore_Des  ")
        stb.AppendLine("      , isNull(opAutor.Tipo_Visibilità, f.Tipo_Visibilità) as Tipo_Visibilità ")
        stb.AppendLine("   ")
        stb.AppendLine("  From agronica_Stazioni z1 ")
        stb.AppendLine("  inner join  agronica_fornitori f ")
        stb.AppendLine("  on f.Fornitore_Cod = z1.Fornitore_Cod ")
        stb.AppendLine("      inner Join Agronica_StazioniXpiva_OperazioniAutorizzate opAutor ")
        stb.AppendLine("             On z1.Stazione_Cod = opAutor.Stazione_Cod ")

        If Not String.IsNullOrEmpty(piva) Then

            If piva.Contains(",") Then
                If Not piva.Contains("'") Then

                    Dim vPiva As String() = piva.Split(",")
                    For i = 0 To vPiva.Length
                        vPiva(i) = "'" & vPiva(i) & "'"
                    Next

                    piva = String.Join(",", vPiva)
                End If

                stb.AppendLine("          And opAutor.piva in (" & piva & ")")

            Else
                stb.AppendLine("          And opAutor.piva = '" & piva & "' ")
            End If

        Else

            ' VAnni: 6/2/2019: se non viene inviata una p.iva allora il risultato deve essere vuoto!
            stb.AppendLine("          And opAutor.piva = '' ")

        End If

        'If Not String.IsNullOrEmpty(Username) Then
        '    stb.AppendLine("          And opAutor.Utente_Username = '" & Username & "' ")
        'End If

        stb.AppendLine("          And opAutor.piva_SuperUser = '" & pivaSuperUser & "' ")
        stb.AppendLine(" ) FiltroForn ")
        stb.AppendLine("  ")
        stb.AppendLine(" On FiltroForn.Stazione_cod = z.Stazione_cod ")
        stb.AppendLine("  ")
        stb.AppendLine("  ")
        stb.AppendLine(" where Tipo_Visibilità = 0 ")

    End Sub


    ''' <summary>
    ''' Versione per Super-User
    ''' </summary>
    ''' <param name="pivaSuperUser"></param>
    ''' <param name="LeggiDataUltimoAgg"></param>
    ''' <param name="LeggiPosizione"></param>
    ''' <param name="RicavaUltimaDataAggiornamento"></param>
    ''' <param name="stb"></param>
    ''' <remarks>Tipo_Visibilità, viene letto dalla singola autorizzazione di super user oppure da quella del fornitore</remarks>
    Private Shared Sub StazioniAppoggioQry(ByVal pivaSuperUser As String, ByVal LeggiDataUltimoAgg As Boolean, ByVal LeggiPosizione As Boolean, ByVal RicavaUltimaDataAggiornamento As Boolean, ByRef stb As StringBuilder)


        stb.AppendLine("  Select  ")
        stb.AppendLine("    Stazione_Cod as nome ")
        stb.AppendLine("  , '[' + FiltroForn.Fornitore_Des + '] ' + Stazione_Des as Descrizione  ")
        If LeggiPosizione Then
            stb.AppendLine("  , Posizione ")
            stb.AppendLine("  , PosizioneLatitudine as f_latitude ")
            stb.AppendLine("  , PosizioneLongitudine as f_longitude ")
        End If

        stb.AppendLine("  , Stazione_Cod_Fornitore  ")

        If LeggiDataUltimoAgg Then
            stb.AppendLine("  , isNull (  ")
            stb.AppendLine("              ( ")
            stb.AppendLine("                 Select top 1 cast(DataOraRilievo As Date) As UltimaDataRilievo  ")
            stb.AppendLine("                 From Agronica_Dati_Rilevati RR1  ")
            stb.AppendLine("                 Where RR1.Stazione_Cod = Z.Stazione_Cod ")
            stb.AppendLine("                 Order By DataOraRilievo desc ")
            stb.AppendLine("          ) ")
            stb.AppendLine("    , cast('01/01/1900' as date) ")
            stb.AppendLine("  ) as  data_ultimo_agg")
        End If


        stb.AppendLine(" ")

        stb.AppendLine(" From agronica_Stazioni z ")
        stb.AppendLine(" inner Join  ")
        stb.AppendLine(" ( ")
        stb.AppendLine("  Select ")
        stb.AppendLine("         f.Fornitore_Cod ")
        stb.AppendLine("      , isnull(opAutor.DescrizioneAggiuntiva, f.Fornitore_Des) as   Fornitore_Des  ")
        stb.AppendLine("      , isNull(opAutor.Tipo_Visibilità, f.Tipo_Visibilità) as Tipo_Visibilità ")
        stb.AppendLine("   ")
        stb.AppendLine("  From agronica_fornitori f ")
        stb.AppendLine("      Left Join Agronica_FornitoriXpiva_SuperUser_OperazioniAutorizzate opAutor ")
        stb.AppendLine("             On f.fornitore_Cod = opAutor.Fornitore_Cod ")
        stb.AppendLine("          And opAutor.piva_SuperUser = '" & pivaSuperUser & "' ")
        stb.AppendLine(" ) FiltroForn ")
        stb.AppendLine("  ")
        stb.AppendLine(" On FiltroForn.Fornitore_Cod = z.Fornitore_Cod ")
        stb.AppendLine("  ")
        stb.AppendLine("  ")
        stb.AppendLine(" where Tipo_Visibilità = 0 ")



    End Sub


    Public Function LeggiFornitoreDaStazione(ByVal id_stazione As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.LeggiFornitoreDaStazione()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.AppendLine("SELECT f.fornitore_cod, f.fornitore_des ")
            Stb.AppendLine("FROM Agronica_Fornitori f ")
            Stb.AppendLine("INNER JOIN Agronica_Stazioni s on s.Fornitore_Cod = f.Fornitore_Cod ")
            Stb.AppendLine("WHERE s.Stazione_Cod = " & id_stazione)

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


    Public Function LeggiQuadranteDaStazione(
        byval id_stazione as Integer, _         
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.LeggiQuadranteDaStazione()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.AppendLine(" select id_Quadrante ") 
            Stb.AppendLine(" from [dbo].[TB_QuadrantixStazioni] ") 
            Stb.AppendLine(" where id_Stazione = " & id_stazione )

                       
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



    '##############################################################################################
    Public Function Leggi_xFormato_ModelliPrevisionali(
        ByVal xFiltroAggiuntivo As String, _
        ByVal xOrderBy As String, _
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi_xFormato_ModelliPrevisionali()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0


            
            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
           

            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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



    Public Function Agronica_Dati_Rilevati_LeggiPioggeGiornaliere(f_station_name As String,
                                                                    dataDa As Date, dataA As Date,
                                                                    objparametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "Metos_Dati_Rilevati_LeggiPioggeGiornaliere"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try



            StrSQL.Length = 0

            StrSQL.Append(" SELECT CONVERT(Date, [DataOraRilievo], 120) as Data, max (PrecipitazioniCumulo) as Precipitazione24ore ")
            StrSQL.Append("from Agronica_Dati_Rilevati")

            StrSQL.Append(" WHERE   1=1  ")


            'If f_station_name <> "" Then
            StrSQL.Append(" and   Stazione_Cod = " & Agro_SQL_SaveText_NULL(f_station_name) & "  ")
            'End If

            StrSQL.Append(" and CONVERT(Date, [DataOraRilievo], 120)  <= CONVERT(DateTime, " & Agro_SQL_SaveDateTime_NULL(CDate(dataA)) & ", 120)    ")
            StrSQL.Append(" and CONVERT(Date, [DataOraRilievo], 120)  >= CONVERT(DateTime, " & Agro_SQL_SaveDateTime_NULL(CDate(dataDa)) & ", 120)    ")

            StrSQL.Append(" group by CONVERT(Date, [DataOraRilievo], 120)  ")
            StrSQL.Append(" order by Data asc ")

            '-----------------------------------------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objparametri, StrSQL.ToString, NomeRoutine)
            '-----------------------------------------------------------------------------------------------------------

            Return DT

        Catch ex As Exception

            DT = Nothing
            MessaggioErrore = "[" & NomeRoutine & "]:" & ex.Message & " "
            Throw New Exception(MessaggioErrore)

        End Try

    End Function


    '##############################################################################################


    ''' <summary>
    ''' Ottiene i dati meteo da stazioni oppure quadranti, giornalieri oppure orari
    ''' </summary>
    ''' <param name="ID_QuadranteStazione">Codice del quadrante oppure della stazione, è la chiave della tabella dove si leggono i dati per quadranti o stazioni Emilia-Romagna</param>
    ''' <param name="OraRilievo"></param>
    ''' <param name="DataInizio"></param>
    ''' <param name="DataFine"></param>
    ''' <param name="Flag_Quadrante_Stazione">Q per Quadrante, S per Stazione, si attiva solo se Flag_Tabella_Leggi è diverso da G e da M </param>
    ''' <param name="flag_tabella_leggi">Indica la tabella dove leggere i dati (N = Agronica_Dati_Rilevati_GG, M = Agronica_Dati_Rilevati oppure G = dati_meteo_qgg oppure Dati_Meteo_QHH, Dati_Meteo_SHH )</param>
    ''' <param name="NomeStazione">Se Flag_Tabella_Leggi = "M", allora è la chiave primaria della tabella Agronica_Dati_Rilevati </param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    Public Function Dati_Completi(
        ByVal ID_QuadranteStazione As Integer,
        ByVal OraRilievo As Integer,
        ByVal DataInizio As Date,
        ByVal DataFine As Date,
        ByVal Flag_Quadrante_Stazione As String,
        ByVal flag_tabella_leggi As String,
        ByVal NomeStazione As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
     ) As DataTable


        Dim NomeRoutine As String = "xMeteoDAL.Dati_Completi"
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable


        Try

            '-----------------------------------------------------------------------------------------------------------
            ' Colonne : Data , Precipitazione24ore
            '
            ' Sul solo quadrante/stazione indicato,
            ' per ogni giorno dell'intevallo,
            ' prendo i 24 rilievi che precedono l'ora indicata 
            ' e ne sommo il valore delle precipitazioni
            '-----------------------------------------------------------------------------------------------------------

            Stb.Length = 0

            Stb.AppendLine(" SELECT  ")


            Select Case flag_tabella_leggi
                Case "M"
                    Stb.AppendLine(" 	    DataOraRilievo,  ")
                    Stb.AppendLine(" 	    CONVERT(VARCHAR(10), [DataOraRilievo], 105) AS Data, ")
                    Stb.AppendLine(" 	    DATEPART(HOUR,DataOraRilievo) as Ora, ")
                Case "N"
                    Stb.AppendLine(" 	    DataOraRilievo,  ")
                    Stb.AppendLine(" 	    CONVERT(VARCHAR(10), [DataOraRilievo], 105) AS Data, ")
                    Stb.AppendLine(" 	    DATEPART(HOUR,DataOraRilievo) as Ora, ")
                Case Else
                    Stb.AppendLine(" 	    Tempo,  ")
                    Stb.AppendLine(" 	    CONVERT(VARCHAR(10), [Tempo], 105) AS Data, ")
                    Stb.AppendLine(" 	    DATEPART(HOUR,Tempo) as Ora, ")
            End Select


            If flag_tabella_leggi = "G" Then
                Stb.AppendLine(" 	    Data_Agg, EvapoTraspirazione, Temp_Min, Temp_Max, Temp_Media, Vento_Int, Vento_Dir, ")
            Else

                If flag_tabella_leggi = "M" Then

                    'Stb.AppendLine("    f_date as Data_Agg, isnull(Evaporation_Pan, 0) as  EvapoTraspirazione, Air_Temperature_min as  Temp_Min, Air_Temperature_Max as Temp_Max, Air_Temperature_Aver as Temp_Media, isnull( Wind_Speed_Aver, 0) as Vento_Int, 0 as Vento_Dir,  "  )

                    Stb.AppendLine("   DataOraRilievo as Data_Agg ")
                    Stb.AppendLine(" , isnull(Evaporimetro, 0) as  EvapoTraspirazione ")
                    Stb.AppendLine(" , isnull(AriaTemperaturaMin, AriaTemperaturaMedia) as  Temp_Min ") 'null..?
                    Stb.AppendLine(" , isnull(AriaTemperaturaMax, AriaTemperaturaMedia) as Temp_Max ") 'null..?
                    Stb.AppendLine(" , AriaTemperaturaMedia as Temp_Media ")
                    Stb.AppendLine(" , isnull(VentoVelocitaMedia , 0) as Vento_Int ")
                    Stb.AppendLine(" , 0 as Vento_Dir ")
                    Stb.AppendLine(" , DataOraRilievo as TEMPO ")
                    Stb.AppendLine(" , ISNULL(AriaUmidita, 0 ) as UmiditaRelativa ")
                    Stb.AppendLine(" , ISNULL(BagnaturaFogliare, 0 ) as Bagnatura ")
                    Stb.AppendLine(" , ISNULL(Precipitazioni, 0 ) as Precipitazione ")
                    Stb.AppendLine(" , SuoloUmidita1 as UmiditaSuolo ")
                    Stb.AppendLine(" , ")
                    Stb.AppendLine(" ")

                Else

                    '' VAnni: 4/1/2018: per ora  su "if" separato, poichè cambieranno i dati letti
                    If flag_tabella_leggi = "N" Then

                        Stb.AppendLine("   DataOraRilievo as Data_Agg ")
                        Stb.AppendLine(" , isnull(Evaporimetro, 0) as  EvapoTraspirazione ")
                        Stb.AppendLine(" , isnull(AriaTemperaturaMin, AriaTemperaturaMedia) as  Temp_Min ") 'null..?
                        Stb.AppendLine(" , isnull(AriaTemperaturaMax, AriaTemperaturaMedia) as Temp_Max ") 'null..?
                        Stb.AppendLine(" , AriaTemperaturaMedia as Temp_Media ")
                        Stb.AppendLine(" , isnull(VentoVelocitaMedia , 0) as Vento_Int ")
                        Stb.AppendLine(" , 0 as Vento_Dir ")
                        Stb.AppendLine(" , DataOraRilievo as TEMPO ")
                        Stb.AppendLine(" , ISNULL(AriaUmidita, 0 ) as UmiditaRelativa ")
                        Stb.AppendLine(" , ISNULL(BagnaturaFogliare, 0 ) as Bagnatura ")
                        Stb.AppendLine(" , ISNULL(Precipitazioni, 0 ) as Precipitazione ")
                        Stb.AppendLine(" , SuoloUmidita1 as UmiditaSuolo ")
                        Stb.AppendLine(" , ")
                        Stb.AppendLine(" ")

                    Else

                        Stb.AppendLine(" 	    ISNULL(Precipitazione,-999) as Precipitazione, ")
                        Stb.AppendLine(" 	    Data_Agg, UmiditaRelativa, Temp_Media, Bagnatura, ")
                    End If
                End If
            End If

            If flag_tabella_leggi = "G" Then
                Stb.AppendLine(" 	    OP = CASE WHEN (tempo<=Data_Agg) THEN 'O' ELSE 'P' END ")
            Else
                Stb.AppendLine(" 	    OP = 'O' ")
            End If

            If flag_tabella_leggi = "G" Then
                Stb.AppendLine("  from dati_meteo_qgg QGG  ")
                Stb.AppendLine("  inner join [dbo].[TB_QuadrantixStazioni] QS ")
                Stb.AppendLine("      on QGG.ID_Quadrante = QS.ID_Quadrante")


            Else

                If flag_tabella_leggi = "M" Then
                    Stb.AppendLine(" 		FROM    Agronica_dati_rilevati ")
                Else
                    If flag_tabella_leggi = "N" Then
                        Stb.AppendLine(" 		FROM    Agronica_dati_rilevati_GG ")
                    Else

                        '---------------------------------------------------
                        Select Case Flag_Quadrante_Stazione
                            Case "Q"
                                Stb.AppendLine(" 		FROM    Dati_Meteo_QHH ")
                            Case "S"
                                Stb.AppendLine(" 		FROM    Dati_Meteo_SHH ")
                            Case Else
                                Throw New Exception("Parametro non valido")
                        End Select
                        '---------------------------------------------------
                    End If
                End If

            End If

            If flag_tabella_leggi = "M" Or flag_tabella_leggi = "N" Then
                Stb.AppendLine(" WHERE   DataOraRilievo >= " & Agro_SQL_SaveDate(DataInizio) & "   ")
                Stb.AppendLine(" AND     DataOraRilievo < DATEADD(DAY,1," & Agro_SQL_SaveDate(DataFine) & " )  ")

            Else
                Stb.AppendLine(" WHERE   Tempo >= " & Agro_SQL_SaveDate(DataInizio) & "   ")
                Stb.AppendLine(" AND     Tempo < DATEADD(DAY,1," & Agro_SQL_SaveDate(DataFine) & " )  ")

            End If


            If flag_tabella_leggi = "G" Then
                Stb.AppendLine(" AND     QS.ID_Stazione=" & Agro_SQL_SaveNum(ID_QuadranteStazione) & " ")
            Else

                If flag_tabella_leggi = "M" Or flag_tabella_leggi = "N" Then
                    Stb.AppendLine(" AND  Stazione_Cod = " & NomeStazione & " ")
                Else

                    '---------------------------------------------------
                    Select Case Flag_Quadrante_Stazione
                        Case "Q"
                            Stb.AppendLine(" AND     ID_Quadrante=" & Agro_SQL_SaveNum(ID_QuadranteStazione) & " ")
                        Case "S"
                            Stb.AppendLine(" AND     ID_Stazione=" & Agro_SQL_SaveNum(ID_QuadranteStazione) & " ")
                        Case Else
                            Throw New Exception("Parametro non valido")
                    End Select
                    '---------------------------------------------------
                End If

            End If

            If flag_tabella_leggi = "M" Or flag_tabella_leggi = "N" Then
                Stb.AppendLine(" ORDER BY DataOraRilievo ASC ")

            Else
                Stb.AppendLine(" ORDER BY Tempo ASC ")

            End If

            '-----------------------------------------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '-----------------------------------------------------------------------------------------------------------

        Catch ex As Exception

            DT = Nothing
            MessaggioErrore = "[" & NomeRoutine & "]:" & ex.Message & " "
            Throw New Exception(MessaggioErrore)

        End Try

        Return DT

    End Function

    Private Function StazioneConfigurazioneOrDefault(ByVal TipoSorgente As Integer, ByVal ID_Stazione As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "xMeteoDAL.StazioneConfigurazione"
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Dim DTDefault As New DataTable
            Dim row As DataRow

            Dim colonne() As String = {"Colonna", "Descrizione", "Simbolo", "AggFun", "Colore", "Grafico", "Gruppo", "Ordine"}
            Dim righe(,) As String = {
                {"AriaTemperaturaMedia", "Temperatura", "°C", "avg", "rgba(192, 0, 0)", "line", "-1", "1"},
                {"Precipitazioni", "Pioggia", "mm", "sum", "#0080FF", "column", "1", "2"},
                {"AriaUmidita", "Umidità relativa", "%", "avg", "#C0C000", "line", "1", "3"}
            }

            For Each s In colonne
                DTDefault.Columns.Add(s)
            Next

            For r = 0 To righe.GetLength(0) - 1
                row = DTDefault.NewRow()
                For c = 0 To colonne.Length - 1
                    row(colonne(c)) = righe(r, c)
                Next
                DTDefault.Rows.Add(row)
            Next

            If TipoSorgente = enum_Meteo_Tiposorgente.Gias_RER Or TipoSorgente = enum_Meteo_Tiposorgente.Gias_RER_Quadranti Then

                DT = DTDefault

            Else

                Stb.Length = 0
                Stb.AppendLine("SELECT ")
                Stb.AppendLine("    NomeColonna AS Colonna ")
                Stb.AppendLine("    , SensoreDescrizione AS Descrizione ")
                Stb.AppendLine("    , COALESCE(UM.UDM_SIM, '') AS Simbolo ")
                Stb.AppendLine("    , COALESCE(FunzioneAggregazioneDati, 'avg') AS AggFun, FunzioneAggregazioneDati AS AggFun_Orig ")
                Stb.AppendLine("    , COALESCE(Colore1, '#808080') AS Colore, Colore1 AS Colore_Orig ")
                Stb.AppendLine("    , COALESCE(TipoDiGrafico, 'line') AS Grafico, TipoDiGrafico AS Grafico_Orig ")
                Stb.AppendLine("    , COALESCE(Raggruppamento, -1) AS Gruppo ")
                Stb.AppendLine("    , Ordine ")
                Stb.AppendLine("FROM Agronica_Stazioni_Configurazioni ")
                Stb.AppendLine("LEFT JOIN UnitaMisura UM ON UM.UDM_COD = Agronica_Stazioni_Configurazioni.UDM_COD ")
                Stb.AppendLine("WHERE Stazione_Cod = " & ID_Stazione & " ")
                Stb.AppendLine("AND AlgoritmoRicostruizioneDatiMancanti = 0")
                Stb.AppendLine("ORDER BY Ordine ")

                DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)

                If DT Is Nothing OrElse DT.Rows.Count = 0 Then

                    DT = DTDefault

                Else

                    'Provare a gestire i vari sensori con i valori di default nel caso non ci siano valori in tabella
                    Dim defCols(,) As String = {{"AggFun_Orig", "AggFun"}, {"Colore_Orig", "Colore"}, {"Grafico_Orig", "Grafico"}}

                    For Each dr In DT.Rows

                        Dim defRows() As DataRow = DTDefault.Select("Colonna = '" & dr("Colonna") & "'")
                        If defRows.Any() Then

                            For c = 0 To defCols.GetLength(0) - 1

                                If IsDBNull(dr(defCols(c, 0))) Then
                                    dr(defCols(c, 1)) = defRows(0)(defCols(c, 1))
                                End If
                            Next
                        End If
                    Next
                End If
            End If

        Catch ex As Exception

            DT = Nothing
            MessaggioErrore = "[" & NomeRoutine & "]:" & ex.Message & " "
            Throw New Exception(MessaggioErrore)

        End Try

        Return DT

    End Function

    Private Structure QueryMeteoConfig
        Public Property Tabella As String
        Public Property ColonnaStazione As String
        Public Property ColonnaDataOra As String
        Public Property Colonne As List(Of String)
    End Structure

    Private Function ConfigDaTipoSorgente(ByVal TipoSorgente As Integer) As QueryMeteoConfig

        Dim res As New QueryMeteoConfig
        res.Colonne = New List(Of String)

        If TipoSorgente = enum_Meteo_Tiposorgente.Gias_RER OrElse TipoSorgente = enum_Meteo_Tiposorgente.Gias_RER_Quadranti Then

            If TipoSorgente = enum_Meteo_Tiposorgente.Gias_RER Then

                res.Tabella = "Dati_Meteo_SHH"
                res.ColonnaStazione = "ID_Stazione"

            Else

                res.Tabella = "Dati_Meteo_QHH"
                res.ColonnaStazione = "ID_Quadrante"

            End If

            res.ColonnaDataOra = "Tempo"

            res.Colonne.Add("Tempo AS DataOraRilievo")
            res.Colonne.Add("CONVERT(VARCHAR(10), Tempo, 105) AS Data")
            res.Colonne.Add("DATEPART(HOUR, Tempo) AS Ora")
            res.Colonne.Add("Temp_Media AS Temperatura")
            res.Colonne.Add("ISNULL(UmiditaRelativa, 0) AS UmiditaRelativa")
            res.Colonne.Add("ISNULL(Bagnatura, 0) AS Bagnatura")
            res.Colonne.Add("ISNULL(Precipitazione, 0) AS Precipitazione")
            res.Colonne.Add("NULL AS UmiditaSuolo ")

        Else

            res.Tabella = "Agronica_Dati_Rilevati_SubOrari"
            res.ColonnaStazione = "Stazione_Cod"
            res.ColonnaDataOra = "DataOraRilievo"

            res.Colonne.Add("DataOraRilievo")
            res.Colonne.Add("CONVERT(VARCHAR(10), DataOraRilievo, 105) AS Data")
            res.Colonne.Add("DATEPART(HOUR, DataOraRilievo) AS Ora")
            res.Colonne.Add("AriaTemperaturaMedia AS Temperatura")
            res.Colonne.Add("ISNULL(AriaUmidita, 0) AS UmiditaRelativa")
            res.Colonne.Add("ISNULL(BagnaturaFogliare, 0) AS Bagnatura")
            res.Colonne.Add("ISNULL(Precipitazioni, 0) AS Precipitazione")
            res.Colonne.Add("SuoloUmidita1 AS UmiditaSuolo ")

        End If

        Return res
    End Function

    Public Function MeteoDatiOrari(ByVal TipoSorgente As Integer, ByVal ID_Stazione As Integer, ByVal DataInizio As Date, ByVal DataFine As Date, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "xMeteoDAL.MeteoDatiOrari"
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Dim query_cfg = ConfigDaTipoSorgente(TipoSorgente)

            Stb.Length = 0

            Stb.AppendLine("SELECT ")
            Dim prima As Boolean = True
            Dim strcol As String
            For Each col In query_cfg.Colonne
                strcol = "  "
                If Not prima Then
                    strcol &= ", "
                End If
                strcol &= col
                prima = False

                Stb.AppendLine(strcol)
            Next
            Stb.AppendLine("FROM " & query_cfg.Tabella)
            Stb.AppendLine("WHERE " & query_cfg.ColonnaStazione & " = " & ID_Stazione)
            Stb.AppendLine("AND " & query_cfg.ColonnaDataOra & " >= " & Agro_SQL_SaveDate(DataInizio))
            Stb.AppendLine("AND " & query_cfg.ColonnaDataOra & " < DATEADD(DAY, 1, " & Agro_SQL_SaveDate(DataFine) & " )")
            Stb.AppendLine("ORDER BY " & query_cfg.ColonnaDataOra & " ASC")

            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)

        Catch ex As Exception

            DT = Nothing
            MessaggioErrore = "[" & NomeRoutine & "]" & ex.Message & " "
            Throw New Exception(MessaggioErrore)

        End Try

        Return DT

    End Function

    Public Function UltimiDatiOrari(ByVal TipoSorgente As Integer, ByVal ID_Stazione As Integer, ByVal NumOre As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, Optional ByRef StazioneDes As String = Nothing, Optional ByRef StazioneCfg As DataTable = Nothing) As DataTable

        Dim NomeRoutine As String = "xMeteoDAL.UltimiDatiOrari"
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Dim select_stazione As String = ""

            Stb.Length = 0

            Dim dtStazioneCfg = StazioneConfigurazioneOrDefault(TipoSorgente, ID_Stazione, objParametri)

            If StazioneCfg IsNot Nothing Then
                StazioneCfg = dtStazioneCfg
            End If

            If TipoSorgente = enum_Meteo_Tiposorgente.Gias_RER Or TipoSorgente = enum_Meteo_Tiposorgente.Gias_RER_Quadranti Then

                'AriaTemperaturaMedia, AriaUmidita, Precipitazioni

                Dim from_where As String = "FROM "

                If TipoSorgente = enum_Meteo_Tiposorgente.Gias_RER Then

                    from_where &= "Dati_Meteo_SHH WHERE ID_Stazione = " & ID_Stazione
                    select_stazione = "Select Stazione_Des FROM TB_Stazioni WHERE ID_Stazione = " & ID_Stazione

                Else

                    from_where &= "Dati_Meteo_QHH WHERE ID_Quadrante = " & ID_Stazione
                    select_stazione = "Select Quadrante_Des FROM TB_Quadranti WHERE ID_Quadrante = " & ID_Stazione

                End If

                Stb.AppendLine("Declare @LAST_DATE As DATETIME = DATEADD(HOUR, -" & NumOre & ", (Select MAX(Tempo) " & from_where & "))")
                Stb.AppendLine("")
                Stb.AppendLine("Select ")
                Stb.AppendLine("	Tempo As DataOraRilievo ")
                Stb.AppendLine("	, Temp_Media As AriaTemperaturaMedia ")
                Stb.AppendLine("	, UmiditaRelativa As AriaUmidita ")
                Stb.AppendLine("	, Precipitazione As Precipitazioni ")
                Stb.AppendLine(from_where & " And Tempo >= @LAST_DATE")
                Stb.AppendLine("ORDER BY Tempo ASC ")

            Else

                Stb.AppendLine("DECLARE @LAST_DATE AS DATETIME = DATEADD(HOUR, -" & NumOre & ", (Select MAX(DataOraRilievo) FROM Agronica_Dati_Rilevati_SubOrari WHERE Stazione_Cod = " & ID_Stazione & ")) ")

                Dim flagProva = True
                If flagProva Then
                    'Due rilievi nello stesso minuto (@Granularity = 1) sono raggruppati -> da mm:30s a (mm + 1):29s
                    'Il sistema "logicamente" fallisce se ci sono 2 rilievi vicini a cavallo dei 30 secondi...
                    'ad esempio mm:20s e mm:40s saranno considerati diversi (in due righe diverse) anche se "logicamente" sono lo stesso rilievo
                    '(Stesso discorso anche per granularità diverse)
                    Stb.AppendLine("DECLARE @Granularity AS INTEGER = 1 --minuti ")
                    Stb.AppendLine("")
                    Stb.AppendLine("SELECT  ")
                    Stb.AppendLine("	DataOraRilievo ")

                    For Each row In dtStazioneCfg.Rows
                        Stb.AppendLine("	, ROUND(" & row("AggFun") & "(" & row("Colonna") & "), 3) As " & row("Colonna"))
                    Next

                    Stb.AppendLine("FROM ( ")
                    Stb.AppendLine("SELECT ")
                    Stb.AppendLine("	DATEADD(minute, DATEDIFF(minute, 0, DATEADD(second, ( @Granularity * 60 ) / 2, DataOraRilievo )) / @Granularity * @Granularity, 0) AS DataOraRilievo ")

                    For Each row In dtStazioneCfg.Rows
                        Stb.AppendLine("	, " & row("Colonna"))
                    Next

                    Stb.AppendLine("FROM Agronica_Dati_Rilevati_SubOrari ")
                    Stb.AppendLine("WHERE Stazione_Cod = " & ID_Stazione & " And DataOraRilievo >= @LAST_DATE ")
                    Stb.AppendLine(") SubOrari ")
                    Stb.AppendLine("GROUP BY SubOrari.DataOraRilievo ")
                    Stb.AppendLine("ORDER BY DataOraRilievo ASC ")

                Else

                    Stb.AppendLine("")
                    Stb.AppendLine("Select ")
                    Stb.AppendLine("	DataOraRilievo ")

                    For Each row In dtStazioneCfg.Rows
                        Stb.AppendLine("	, " & row("Colonna"))
                    Next

                    Stb.AppendLine("FROM Agronica_Dati_Rilevati_SubOrari ")
                    Stb.AppendLine("WHERE Stazione_Cod = " & ID_Stazione & " And DataOraRilievo >= @LAST_DATE")
                    Stb.AppendLine("ORDER BY DataOraRilievo ASC ")

                End If

                select_stazione = "Select Stazione_Des + (Case When LEN(ISNULL(Stazione_Cod_Fornitore, '')) = 0 THEN '' ELSE ' - ' + Stazione_Cod_Fornitore END) FROM Agronica_Stazioni WHERE Stazione_Cod = " & ID_Stazione

                End If

            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)

            If StazioneDes IsNot Nothing Then
                Dim dtStazione = EseguiQuery_Lettura(objParametri, select_stazione, NomeRoutine)
                If dtStazione.Rows.Count > 0 Then
                    StazioneDes = dtStazione.Rows(0)(0)
                End If
            End If


        Catch ex As Exception

            DT = Nothing
            MessaggioErrore = "[" & NomeRoutine & "]" & ex.Message & " "
            Throw New Exception(MessaggioErrore)

        End Try

        Return DT

    End Function



    '##############################################################################################
    Public Function Dati_Precipitazioni_Sum24H_R(
                                ByVal ID_QuadranteStazione As Integer,
                                ByVal OraRilievo As Integer,
                                ByVal DataInizio As Date,
                                ByVal DataFine As Date,
                                ByVal Flag_Quadrante_Stazione As String,
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,     
                                  ByVal StringaConnessione As String) _
                                  As DataTable

        Dim NomeRoutine As String = "xMeteoDAL.DatiPrecipitazioni_Sum24H_R"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '-----------------------------------------------------------------------------------------------------------
            ' Colonne : Data , Precipitazione24ore
            '
            ' Sul solo quadrante/stazione indicato,
            ' per ogni giorno dell'intevallo,
            ' prendo i 24 rilievi che precedono l'ora indicata 
            ' e ne sommo il valore delle precipitazioni
            '-----------------------------------------------------------------------------------------------------------

            StrSQL.Length = 0

            StrSQL.Append(" SELECT  ")
            StrSQL.Append(" 	    CONVERT(VARCHAR(10), [Tempo], 105) AS Data, ")

            StrSQL.Append(" 		( ")
            StrSQL.Append(" 		SELECT  sum(ISNULL(Precipitazione,0)*10)/10 as Precipitazione24ore  ")

            '---------------------------------------------------
            Select Case Flag_Quadrante_Stazione
                Case "Q"
                    StrSQL.Append(" 		FROM    Dati_Meteo_QHH as HH2 ")
                Case "S"
                    StrSQL.Append(" 		FROM    Dati_Meteo_SHH as HH2 ")
                Case Else
                    Throw New Exception("Parametro non valido")
            End Select
            '---------------------------------------------------

            '---------------------------------------------------
            Select Case Flag_Quadrante_Stazione
                Case "Q"
                    StrSQL.Append(" 		WHERE   ID_Quadrante=" & Agro_SQL_SaveNum(ID_QuadranteStazione) & " ")
                Case "S"
                    StrSQL.Append(" 		WHERE   ID_Stazione=" & Agro_SQL_SaveNum(ID_QuadranteStazione) & " ")
                Case Else
                    Throw New Exception("Parametro non valido")
            End Select
            '---------------------------------------------------

            StrSQL.Append(" 		And     datediff(HOUR,Tempo, HH1.Tempo) >= 0	 ")
            StrSQL.Append(" 		And     datediff(HOUR,Tempo, HH1.Tempo) < 24 ")
            StrSQL.Append(" 		) as Precipitazione24ore ")

            '---------------------------------------------------
            Select Case Flag_Quadrante_Stazione
                Case "Q"
                    StrSQL.Append(" 		FROM    Dati_Meteo_QHH as HH1 ")
                Case "S"
                    StrSQL.Append(" 		FROM    Dati_Meteo_SHH as HH1 ")
                Case Else
                    Throw New Exception("Parametro non valido")
            End Select
            '---------------------------------------------------

            '---------------------------------------------------
            Select Case Flag_Quadrante_Stazione
                Case "Q"
                    StrSQL.Append(" 		WHERE   ID_Quadrante=" & Agro_SQL_SaveNum(ID_QuadranteStazione) & " ")
                Case "S"
                    StrSQL.Append(" 		WHERE   ID_Stazione=" & Agro_SQL_SaveNum(ID_QuadranteStazione) & " ")
                Case Else
                    Throw New Exception("Parametro non valido")
            End Select
            '---------------------------------------------------

            StrSQL.Append(" And     Tempo >=  " & Agro_SQL_SaveDate(DataInizio) & "  ")
            StrSQL.Append(" And     Tempo < DATEADD(DAY,1, " & Agro_SQL_SaveDate(DataFine) & " ) ")
            StrSQL.Append(" And     DATEPART(HOUR,Tempo) = " & Agro_SQL_SaveNum(OraRilievo) & " ")

            StrSQL.Append(" ORDER BY Tempo ")

            '-----------------------------------------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '-----------------------------------------------------------------------------------------------------------

        Catch ex As Exception

            DT = Nothing
            MessaggioErrore = "[" & NomeRoutine & "]:" & ex.Message & " "
            Throw New Exception(MessaggioErrore)

        End Try

        Return DT

    End Function




    '##############################################################################################
    Public Function Dati_Precipitazioni_Orari_R(
                                ByVal ID_QuadranteStazione As Integer,
                                ByVal OraRilievo As Integer,
                                ByVal DataInizio As Date,
                                ByVal DataFine As Date,
                                ByVal Soglia As Single,
                                ByVal Flag_Quadrante_Stazione As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,     
                                    ByVal StringaConnessione As String) _
                                    As DataTable

        Dim NomeRoutine As String = "xMeteoDAL.DatiPrecipitazioni_Orari_R"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '-----------------------------------------------------------------------------------------------------------
            ' Colonne : Tempo, Data , Ora , Precipitazione , Data_Agg , OP
            '
            ' Sul solo quadrante indicato,
            ' per ogni giorno dell'intevallo,
            ' recupero le precipitazioni
            ' il cui valore corrispondente supera la soglia
            '-----------------------------------------------------------------------------------------------------------

            StrSQL.Length = 0

            StrSQL.Append(" SELECT  ")
            StrSQL.Append(" 	    Tempo,  ")
            StrSQL.Append(" 	    CONVERT(VARCHAR(10), [Tempo], 105) AS Data, ")
            StrSQL.Append(" 	    DATEPART(HOUR,Tempo) as Ora, ")
            StrSQL.Append(" 	    ISNULL(Precipitazione,-999) as Precipitazione, ")
            StrSQL.Append(" 	    Data_Agg, ")
            StrSQL.Append(" 	    OP = CASE WHEN (Tempo<=Data_Agg) THEN 'O' ELSE 'P' END ")

            '---------------------------------------------------
            Select Case Flag_Quadrante_Stazione
                Case "Q"
                    StrSQL.Append(" 		FROM    Dati_Meteo_QHH ")
                Case "S"
                    StrSQL.Append(" 		FROM    Dati_Meteo_SHH ")
                Case Else
                    Throw New Exception("Parametro non valido")
            End Select
            '---------------------------------------------------

            StrSQL.Append(" WHERE   Tempo >= " & Agro_SQL_SaveDate(DataInizio) & "   ")
            StrSQL.Append(" AND     Tempo < DATEADD(DAY,1," & Agro_SQL_SaveDate(DataFine) & " )  ")
            StrSQL.Append(" AND     Precipitazione>=" & Agro_SQL_SaveNum(Soglia) & " ")

            '---------------------------------------------------
            Select Case Flag_Quadrante_Stazione
                Case "Q"
                    StrSQL.Append(" AND     ID_Quadrante=" & Agro_SQL_SaveNum(ID_QuadranteStazione) & " ")
                Case "S"
                    StrSQL.Append(" AND     ID_Stazione=" & Agro_SQL_SaveNum(ID_QuadranteStazione) & " ")
                Case Else
                    Throw New Exception("Parametro non valido")
            End Select
            '---------------------------------------------------

            StrSQL.Append(" ORDER BY Tempo ASC ")

            '-----------------------------------------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '-----------------------------------------------------------------------------------------------------------

        Catch ex As Exception

            DT = Nothing
            MessaggioErrore = "[" & NomeRoutine & "]:" & ex.Message & " "
            Throw New Exception(MessaggioErrore)

        End Try

        Return DT

    End Function




    '##############################################################################################
    Public Function Dati_Quadranti_R(
                                ByVal X As Integer,
                                ByVal Y As Integer,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        Dim NomeRoutine As String = "xMeteoDAL.Dati_Quadranti_R"
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '-----------------------------------------------------------------------------------------------------------
            ' Colonne : ID_Quadrante, Quadrante_Des , Provincia_Sigla 
            '           X_UTM , Y_UTM , X_LON_GS , Y_LAT_GS , X_LON_GC , Y_LAT_GC
            '           Altitudine , DATA_AGG
            '
            ' Selezione dei dati del quadrante che contiene il punto (X,Y)
            '-----------------------------------------------------------------------------------------------------------

            stb.Length = 0

            'StrSQL.Append(" SELECT  ")
            'StrSQL.Append(" 	     [ID_Quadrante] ")
            'StrSQL.Append(" 	    ,[Quadrante_Des] ")
            'StrSQL.Append(" 	    ,[Provincia_Sigla] ")
            'StrSQL.Append(" 	    ,[X_UTM] , [Y_UTM] ")
            'StrSQL.Append(" 	    ,[X_LON_GS] , [Y_LAT_GS] , [X_LON_GC] , [Y_LAT_GC] ")
            'StrSQL.Append(" 	    ,[Altitudine] , [DATA_AGG] ")

            'StrSQL.Append(" FROM    TB_Quadranti ")
            'StrSQL.Append(" WHERE	1=1 ")

            'StrSQL.Append(" AND	    ABS(X_UTM - " & Agro_SQL_SaveNum(X) & ") <= 2500 ")
            'StrSQL.Append(" AND	    ABS(Y_UTM - " & Agro_SQL_SaveNum(Y) & ") <= 2500 ")


            stb.AppendLine(" Declare @x int ")
            stb.AppendLine(" Declare @y int ")

            stb.AppendLine(" set @x =  " & Agro_SQL_SaveNum(X))
            stb.AppendLine(" set @y =  " & Agro_SQL_SaveNum(Y))


            stb.AppendLine("     SELECT TOP 10 ")

            stb.AppendLine("   [ID_Quadrante]          ")
            stb.AppendLine("  ,[Quadrante_Des]         ")
            stb.AppendLine("  ,[Provincia_Sigla]           ")
            stb.AppendLine("  ,[X_UTM]  ")
            stb.AppendLine("  ,[Y_UTM]         ")
            stb.AppendLine("  ,[X_LON_GS]  ")
            stb.AppendLine("  ,[Y_LAT_GS]  ")
            stb.AppendLine("  ,[X_LON_GC]  ")
            stb.AppendLine("  ,[Y_LAT_GC]          ")
            stb.AppendLine("  ,[Altitudine]  ")
            stb.AppendLine("  ,[DATA_AGG]   ")
            stb.AppendLine("  ,sqrt(power((X_UTM - @x), 2) + power( (Y_UTM - @y ), 2) ) As Distanza ")
            stb.AppendLine("  ,ABS(X_UTM - @X) as Test1  ")
            stb.AppendLine("  ,ABS(Y_UTM - @y) as Test2 ")
            stb.AppendLine("  ")
            stb.AppendLine("  From TB_Quadranti ")
            stb.AppendLine("  Where 1 = 1 ")
            stb.AppendLine("  And ABS(X_UTM - @X) <= 7000   ")
            stb.AppendLine("  And ABS(Y_UTM - @y) <= 7000 ")
            stb.AppendLine("   ")
            stb.AppendLine("  order by  sqrt( power ((X_UTM - @x), 2) + power( (Y_UTM - @y ), 2) )")


            '-----------------------------------------------------------------------------------------------------------                      
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '-----------------------------------------------------------------------------------------------------------

        Catch ex As Exception

            DT = Nothing
            MessaggioErrore = "[" & NomeRoutine & "]:" & ex.Message & " "
            Throw New Exception(MessaggioErrore)

        End Try

        Return DT

    End Function




    '##############################################################################################
    Public Function Dati_Quadranti_R(
        ByVal ID_Quadrante As Integer,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        Dim NomeRoutine As String = "xMeteoDAL.Dati_Quadranti_R"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '-----------------------------------------------------------------------------------------------------------
            ' Parametri Opzionali :
            '           ID_Quadrante = 0    ==>     Tutti i quadranti
            '
            ' Colonne : ID_Quadrante, Quadrante_Des , Provincia_Sigla 
            '           X_UTM , Y_UTM , X_LON_GS , Y_LAT_GS , X_LON_GC , Y_LAT_GC
            '           Altitudine , DATA_AGG
            '
            ' Selezione dei dati del quadrante con codice ID_Quadrante
            '-----------------------------------------------------------------------------------------------------------

            StrSQL.Length = 0

            StrSQL.Append(" SELECT  ")
            StrSQL.Append(" 	     [ID_Quadrante] ")
            StrSQL.Append(" 	    ,[Quadrante_Des] ")
            StrSQL.Append(" 	    ,[Provincia_Sigla] ")
            StrSQL.Append(" 	    ,[X_UTM] , [Y_UTM] ")
            StrSQL.Append(" 	    ,[X_LON_GS] , [Y_LAT_GS] , [X_LON_GC] , [Y_LAT_GC] ")
            StrSQL.Append(" 	    ,[Altitudine] , [DATA_AGG] ")

            StrSQL.Append(" FROM    TB_Quadranti ")

            If ID_Quadrante <> 0 Then
                StrSQL.Append(" WHERE	ID_Quadrante = " & Agro_SQL_SaveNum(ID_Quadrante) & " ")
            End If

            StrSQL.Append(" ORDER BY    ID_Quadrante ASC ")

            '-----------------------------------------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '-----------------------------------------------------------------------------------------------------------

        Catch ex As Exception

            DT = Nothing
            MessaggioErrore = "[" & NomeRoutine & "]:" & ex.Message & " "
            Throw New Exception(MessaggioErrore)

        End Try

        Return DT

    End Function




    '##############################################################################################
    Public Function Dati_Stazioni_R(
                                ByVal ID_Stazione As Integer,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,     
                                    ByVal StringaConnessione As String) _
                                    As DataTable

        Dim NomeRoutine As String = "xMeteoDAL.Dati_Stazioni_R"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '-----------------------------------------------------------------------------------------------------------
            ' Parametri Opzionali :
            '           ID_Stazione = 0    ==>     Tutte le stazioni
            '
            ' Colonne : ID_Stazione, Stazione_Des , Provincia_Sigla 
            '           X_UTM , Y_UTM , X_LON_GS , Y_LAT_GS , X_LON_GC , Y_LAT_GC
            '           Altitudine , DATA_AGG
            '
            ' Selezione dei dati della stazione con codice ID_Stazione
            '-----------------------------------------------------------------------------------------------------------

            StrSQL.Length = 0

            StrSQL.Append(" SELECT  ")

            StrSQL.Append(" 	     [ID_Stazione] ")
            StrSQL.Append(" 	    ,[Stazione_Des] ")
            StrSQL.Append(" 	    ,[Provincia_Sigla] ")
            StrSQL.Append(" 	    ,[X_UTM] , [Y_UTM] ")
            StrSQL.Append(" 	    ,[X_LON_GS] , [Y_LAT_GS] , [X_LON_GC] , [Y_LAT_GC] ")
            StrSQL.Append(" 	    ,[Altitudine] , [DATA_AGG] ")

            StrSQL.Append(" FROM    TB_Stazioni ")

            If ID_Stazione <> 0 Then
                StrSQL.Append(" WHERE	ID_Quadrante = " & Agro_SQL_SaveNum(ID_Stazione) & " ")
            End If

            StrSQL.Append(" ORDER BY    ID_Stazione ASC ")

            '-----------------------------------------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '-----------------------------------------------------------------------------------------------------------

        Catch ex As Exception

            DT = Nothing
            MessaggioErrore = "[" & NomeRoutine & "]:" & ex.Message & " "
            Throw New Exception(MessaggioErrore)

        End Try

        Return DT

    End Function

End Class

