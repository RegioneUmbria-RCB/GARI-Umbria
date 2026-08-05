Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility
Imports AgronicaCoreModello

Public Class Capitolato_R
    Inherits AgronicaCoreDataProvider.DataProvider

    ''' <summary>
    ''' Data la lista di capitolati da verificare estrae da questa una lista di capitolati cui l'azienda non può partecipare
    ''' </summary>
    ''' <param name="pivaSuperUser"></param>
    ''' <param name="pivaDaVerificare">piva dell'azienda di cui si richiede la partecipazione</param>
    ''' <param name="capitolatiDaVerificare"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    Public Function LeggiElencoCapitolatiEsclusiDataPiva(
        ByVal pivaSuperUser As String,
        ByVal pivaDaVerificare As String,
        ByVal capitolatiDaVerificare As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreCapitolatoClienteDAL.Capitolato_R.LeggiElencoCapitolatiDataPivaSuperUser()"
        Dim MessaggioErrore As String = ""
        Dim DT As DataTable = Nothing
        Dim stb As New StringBuilder

        Try

            stb.Length = 0


            'il flag GestionePartecipazioneAziende vale:  
            '    1 se l'elenco specificato nel capitolato contiene le Aziende che Partecipano
            '    0 se l'elenco specificato nel capitolato contiene le Aziende che sono escluse dal capitolato

            ' se il flag vale 0 (escluse dal capitolato) 
            '    allora è sufficiente selezionare i capitolati andando in join sulla tabella CapitolatoClienteXImprese
            stb.Append("Select  c.capitolato_Cod " & vbCrLf)
            stb.Append("     From CapitolatoClienteXImprese cci " & vbCrLf)
            stb.Append(" 		inner join CapitolatoCliente c " & vbCrLf)
            stb.Append(" 			on cci.Capitolato_Cod = c.Capitolato_COD " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("     Where cci.piva = '" & Agro_SQL_SaveText(pivaDaVerificare) & "' " & vbCrLf)
            stb.Append("     And cci.PivaSuperUser = '" & pivaSuperUser & "' " & vbCrLf)
            stb.Append("     And c.capitolato_Cod In ( " & vbCrLf)
            stb.Append(" " & capitolatiDaVerificare & vbCrLf)
            stb.Append(" ) " & vbCrLf)
            stb.Append(" and c.GestionePartecipazioneAziende = 0  " & vbCrLf)

            stb.Append(" union  " & vbCrLf)

            ' se il flag vale 1 (Aziende che Partecipano) 
            '    allora si selezionano i capitolati dove non esiste un record in CapitolatoClienteXImprese per l'impresa passata come parametro
            stb.Append(" Select  c.capitolato_Cod     " & vbCrLf)
            stb.Append(" from CapitolatoCliente c " & vbCrLf)
            stb.Append(" where c.capitolato_Cod In ( " & vbCrLf)
            stb.Append(" " & capitolatiDaVerificare & vbCrLf)
            stb.Append(" ) " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" and not exists ( " & vbCrLf)
            stb.Append(" 	select 1 " & vbCrLf)
            stb.Append(" 	From CapitolatoClienteXImprese cci " & vbCrLf)
            stb.Append(" 	where cci.Capitolato_Cod = c.Capitolato_COD  " & vbCrLf)
            stb.Append(" 	and  cci.piva = '" & Agro_SQL_SaveText(pivaDaVerificare) & "' " & vbCrLf)
            stb.Append("     And cci.PivaSuperUser = '" & pivaSuperUser & "' " & vbCrLf)
            stb.Append(" ) " & vbCrLf)
            stb.Append(" and c.GestionePartecipazioneAziende = 1")


            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function

    Public Function LeggiElencoCapitolatiDataPivaSuperUser(ByVal Data As Date,
                                                           ByVal piva As String,
                                                           ByVal str_IN_VerCod As String,
                                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                           ) As DataTable

        Const nomeRoutine As String = "AgronicaCoreCapitolatoClienteDAL.Capitolato_R.LeggiElencoCapitolatiDataPivaSuperUser()"
        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable = Nothing

        Try

            stb.AppendLine("SELECT Capitolato_COD, Capitolato_DES, DPI_Impianto, Capitolato_Codice ")
            stb.AppendLine("FROM CapitolatoCliente ")
            stb.AppendLine("WHERE Validita_Inizio <= " & Agro_SQL_SaveDate(Data) & " ")
            stb.AppendLine("AND Validita_Fine >= " & Agro_SQL_SaveDate(Data) & " ")
            stb.AppendLine("AND Piva_SuperUser = '" & Agro_SQL_SaveText(piva) & "' ")
            stb.AppendLine("AND Attivo = 1 ")

            If str_IN_VerCod <> "" Then
                stb.AppendLine("AND Veg_Cod IN " & Agro_SQL_Save_Clausola_IN(str_IN_VerCod) & " ")
            End If

            dt = EseguiQuery_Lettura(objParametri, stb.ToString(), nomeRoutine)

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Leggi_ElencoCapitolati(ByVal PivaSuperUser As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Const nomeRoutine = "AgronicaCoreCapitolatoClienteDAL.Capitolato_R.Leggi_ElencoCapitolati()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" Select ")
            StrSQL.AppendLine("      [Capitolato_COD] ")
            StrSQL.AppendLine("    , [Capitolato_DES] as Nome   ")
            StrSQL.AppendLine("	   , [Capitolato_DES_Breve] as NomeBreve   ")
            StrSQL.AppendLine("    , b.Descrizione as DPCampagnaPubPriv")
            StrSQL.AppendLine("    , c.NomeEsteso as DPCampagnaDES")
            StrSQL.AppendLine("    , [N_Max_PA] ")
            StrSQL.AppendLine("    , [N_Max_Tracce] ")
            StrSQL.AppendLine("    , [Perc_Max_RMA] ")
            StrSQL.AppendLine("    , [Sum_Perc_Max_RMA] ")
            StrSQL.AppendLine("    , [Attivo] ")
            StrSQL.AppendLine("    , [DPI_Impianto] ")
            StrSQL.AppendLine("    , [GestioneVarieta] ")
            StrSQL.AppendLine("    , a.Data_Creazione ")
            StrSQL.AppendLine("    , a.Piva_SuperUser ")
            StrSQL.AppendLine("    , a.Data_Modifica ")
            StrSQL.AppendLine("    , s.Veg_Des as veg_des")
            StrSQL.AppendLine(" from ")
            StrSQL.AppendLine("     CapitolatoCliente a  ")
            StrSQL.AppendLine("     left Join Flag_Privato_Pubblico_DPI b on (a.Flag_Privato_Pubblico_DPI=b.Flag_DPI_COD) ")
            StrSQL.AppendLine("     left Join Disciplinari c on (a.DPI_COD_REGOLAMENTO = c.DPI_COD_REGOLAMENTO And a.Flag_Privato_Pubblico_DPI = c.Flag_Privato_Pubblico) ")
            StrSQL.AppendLine("     left Join SpecieVegetali s on (s.veg_Cod = a.veg_cod) ")
            StrSQL.AppendLine(" where ")
            StrSQL.AppendLine("     1=1 ")
            If PivaSuperUser <> "" Then
                StrSQL.AppendLine("AND a.Piva_SuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
            End If
            StrSQL.AppendLine(" order by [Capitolato_COD] ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    Public Function LeggiXvideo(ByVal PivaSuperUser As String, ByVal idCapitolato As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Const nomeRoutine = "AgronicaCoreCapitolatoClienteDAL.Capitolato_R.LeggiXvideo()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine("	   a.Capitolato_Codice  ")
            StrSQL.AppendLine("	   , a.Capitolato_COD  ")
            StrSQL.AppendLine("    , [Capitolato_DES] AS Nome   ")
            StrSQL.AppendLine("	   , [Capitolato_DES_Breve] AS NomeBreve   ")
            StrSQL.AppendLine("    , b.Descrizione AS DPCampagnaPubPriv")
            StrSQL.AppendLine("    , c.NomeEsteso AS DPCampagnaDES")
            StrSQL.AppendLine("	   , a.Validita_Inizio  ")
            StrSQL.AppendLine("    , a.Validita_Fine  ")
            StrSQL.AppendLine("	   , [Attivo]  ")
            StrSQL.AppendLine("	   , [DPI_Impianto]  ")
            StrSQL.AppendLine("	   , [Flag_Privato_Pubblico_DPI]  ")
            StrSQL.AppendLine("    , a.DPI_COD_REGOLAMENTO  ")
            StrSQL.AppendLine("    , [N_Max_PA]  ")
            StrSQL.AppendLine("    , [N_Max_Tracce]  ")
            StrSQL.AppendLine("    , [Perc_Max_RMA]  ")
            StrSQL.AppendLine("    , [Sum_Perc_Max_RMA]  ")
            StrSQL.AppendLine("    , a.Piva_SuperUser  ")
            StrSQL.AppendLine("    , [GestioneVarieta]  ")
            StrSQL.AppendLine("    , [veg_cod]  ")
            StrSQL.AppendLine("    , [TabellaRMA_COD]  ")
            StrSQL.AppendLine("    , [FlagBIO]  ")
            StrSQL.AppendLine("    , [GestionePartecipazioneAziende]  ")
            StrSQL.AppendLine("    , [AttivaVerificaFormulatoValido]  ")
            StrSQL.AppendLine(" FROM ")
            StrSQL.AppendLine("     CapitolatoCliente a  ")
            StrSQL.AppendLine("     LEFT JOIN Flag_Privato_Pubblico_DPI b ON (a.Flag_Privato_Pubblico_DPI=b.Flag_DPI_COD) ")
            StrSQL.AppendLine("     LEFT JOIN Disciplinari c ON (a.DPI_COD_REGOLAMENTO = c.DPI_COD_REGOLAMENTO AND a.Flag_Privato_Pubblico_DPI = c.Flag_Privato_Pubblico) ")
            StrSQL.AppendLine(" WHERE ")
            StrSQL.AppendLine("     1=1 ")
            StrSQL.AppendLine("     AND a.Piva_SuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
            StrSQL.AppendLine("     AND a.Capitolato_COD = " & Agro_SQL_SaveNum(idCapitolato) & " ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt
    End Function

    'LeggiElencoTabellaRMA

    Public Function Leggi_ElencoTabellaRMA(ByVal PivaSuperUser As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Const nomeRoutine = "AgronicaCoreCapitolatoClienteDAL.Capitolato_R.Leggi_ElencoTabellaRMA()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0
            'Select Case TabellaRMA_COD, TabellaRMA_DES, inviato, datainvio, Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine
            'From TabellaRMA;

            StrSQL.AppendLine("SELECT rma.TabellaRMA_COD, rma.TAbellaRMA_DES")
            StrSQL.AppendLine(" from StrutturaGerarchicaTabellaRMA st")
            StrSQL.AppendLine(" inner join TabellaRMA RMA on RMA.TabellaRMA_COD = st.TabellaRMA_COD")
            StrSQL.AppendLine(" WHERE 1=1")
            StrSQL.AppendLine("  AND st.Operazione = 1 ")
            StrSQL.AppendLine(" and st.Impresa in ( ")
            StrSQL.AppendLine(" select  '" & Agro_SQL_SaveText(PivaSuperUser) & "' as impresa )")

            'StrSQL.AppendLine(" SELECT TabellaRMA_COD, TabellaRMA_DES, inviato, datainvio, Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine ")
            'StrSQL.AppendLine(" FROM TabellaRMA; ")


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt
    End Function

    Public Function Leggi(ByVal PivaSuperUser As String, ByVal idCapitolato As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Const nomeRoutine = "AgronicaCoreCapitolatoClienteDAL.Capitolato_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT Capitolato_COD, Capitolato_DES, DPI_COD_REGOLAMENTO, ")
            StrSQL.AppendLine("	   Flag_Privato_Pubblico_DPI, N_Max_PA, N_Max_Tracce, Perc_Max_RMA, ")
            StrSQL.AppendLine("    Sum_Perc_Max_RMA, Piva_SuperUser, Attivo, DPI_Impianto, inviato,  ")
            StrSQL.AppendLine("	   datainvio, Data_Creazione, Data_Modifica, Username_Creazione,  ")
            StrSQL.AppendLine("    Username_Modifica, Validita_Inizio, Validita_Fine, GestioneVarieta,  ")
            StrSQL.AppendLine("    veg_cod, TabellaRMA_COD, Capitolato_DES_Breve, FlagBIO, GestionePartecipazioneAziende,  ")
            StrSQL.AppendLine("	   AttivaVerificaFormulatoValido, Capitolato_Codice ")
            StrSQL.AppendLine(" FROM CapitolatoCliente ")
            StrSQL.AppendLine(" WHERE ")
            StrSQL.AppendLine("     1=1 ")
            StrSQL.AppendLine("     And Piva_SuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
            StrSQL.AppendLine("     And Capitolato_COD = " & Agro_SQL_SaveNum(idCapitolato) & " ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt
    End Function

    Public Function LeggiDescrizioneFlagPubblicoPrivato(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Const nomeRoutine = "AgronicaCoreCapitolatoClienteDAL.Capitolato_R.LeggiDescrizioneFlagPubblicoPrivato()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" Select ")
            StrSQL.AppendLine("	   [Flag_DPI_COD]  ")
            StrSQL.AppendLine("    ,[Descrizione]  ")

            StrSQL.AppendLine(" from ")
            StrSQL.AppendLine("     Flag_Privato_Pubblico_DPI  ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt
    End Function


    Public Function LeggiRegolamentoDPI(ByVal CapitolatoCOD As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Const nomeRoutine = "AgronicaCoreCapitolatoClienteDAL.Capitolato_R.LeggiDescrizioneFlagPubblicoPrivato()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" Select ")
            StrSQL.AppendLine("      dpi.DPI_COD_REGOLAMENTO ")
            StrSQL.AppendLine("     ,NomeEsteso ")
            StrSQL.AppendLine(" from ")
            StrSQL.AppendLine("     Disciplinari dpi  ")
            StrSQL.AppendLine("     inner Join capitolatocliente c On (dpi.DPI_COD_REGOLAMENTO = c.DPI_COD_REGOLAMENTO And dpi.flag_privato_Pubblico = c.Flag_Privato_Pubblico_DPI) ")
            StrSQL.AppendLine(" where ")
            StrSQL.AppendLine("     1=1 ")
            If CapitolatoCOD <> 0 Then
                StrSQL.AppendLine("AND c.Capitolato_Cod = " & Agro_SQL_SaveNum(CapitolatoCOD) & " ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt
    End Function


    Public Function LeggiRegolamentoDPIPrivPubb(ByVal pivasuperuser As String, ByVal Privato As Boolean, ByVal FiltroAggiuntivo As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Const nomeRoutine = "AgronicaCoreCapitolatoClienteDAL.Capitolato_R.LeggiDescrizioneFlagPubblicoPrivato()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" Select ")
            StrSQL.AppendLine("      dpi.DPI_COD_REGOLAMENTO ")
            StrSQL.AppendLine("     ,NomeEsteso ")
            StrSQL.AppendLine(" from ")
            StrSQL.AppendLine("     Disciplinari dpi  ")
            StrSQL.AppendLine(" where ")
            StrSQL.AppendLine("     1=1 ")
            If Privato = True Then
                StrSQL.AppendLine("AND Flag_Privato_Pubblico = " & Agro_SQL_SaveNum(1) & " ")
            Else
                StrSQL.AppendLine("AND Flag_Privato_Pubblico = " & Agro_SQL_SaveNum(2) & " ")
            End If
            StrSQL.AppendLine("AND (Piva_superuser = '" & Agro_SQL_SaveText(pivasuperuser) & "' or Piva_superuser is null) ")

            If FiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" and " & Agro_SQL_Save_xFiltroAggiuntivo(FiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt
    End Function

    Public Function LeggiElencoSpecieVegetali(ByVal FiltroAggiuntivo As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Const nomeRoutine = "AgronicaCoreCapitolatoClienteDAL.Capitolato_R.LeggiElencoSpecieVegetali()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" Select ")
            StrSQL.AppendLine("      Veg_Cod ")
            StrSQL.AppendLine("     ,Veg_Des ")
            StrSQL.AppendLine(" from ")
            StrSQL.AppendLine("     SpecieVegetali ")

            If FiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" where " & Agro_SQL_Save_xFiltroAggiuntivo(FiltroAggiuntivo, , objParametri))
            End If

            StrSQL.AppendLine(" Order by Veg_Des")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt
    End Function









    Public Function LeggiElencoSostanzeAttive(ByVal capitolato_cod As Integer,
                                              ByVal leggiSingoloElemento As Boolean,
                                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                Optional partecipaSommatoria As Integer = -1) As DataTable

        Const nomeRoutine = "AgronicaCoreCapitolatoClienteDAL.Capitolato_R.LeggiElencoSostanzeAttive()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim partecipa As Integer = 0
        If partecipaSommatoria = 1 Then
            partecipa = 1
        ElseIf partecipaSommatoria = 0 Then
            partecipa = 0
        End If

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT * FROM ( ")
            StrSQL.AppendLine("     SELECT pa_des AS Descrizione, pa.pa_cod AS CodiceSostanzaAttiva, 1 AS Tipo ")
            If partecipaSommatoria <> -1 Then
                StrSQL.AppendLine("         , partecipaSommatoria, LMR AS maxLMR  ")
            End If
            StrSQL.AppendLine("     FROM PrincipiAttivi pa")

            If leggiSingoloElemento = True Then
                StrSQL.AppendLine("       INNER JOIN capitolatoclienteXPrincipiAttiviRilevati pp ")
                StrSQL.AppendLine("            ON pp.pa_cod = pa.pa_cod ")

                StrSQL.AppendLine("     WHERE pp.Capitolato_cod = " & capitolato_cod)
                StrSQL.AppendLine("     AND pp.pa_cod = pa.pa_cod")
                If partecipaSommatoria <> -1 Then
                    StrSQL.AppendLine("     AND partecipaSommatoria = " & partecipa & " AND LMR <> 0")
                Else
                    StrSQL.AppendLine("     AND LMR = 0")
                End If
            End If

            StrSQL.AppendLine("     UNION ALL ")
            StrSQL.AppendLine("     SELECT '(§) '+ descrizione as Descrizione, CAST(f.fam_cod AS int) AS CodiceSostanzaAttiva, 0 AS Tipo")
            If partecipaSommatoria <> -1 Then
                StrSQL.AppendLine("         , partecipaSommatoria, LMR AS maxLMR  ")
            End If

            StrSQL.AppendLine("     FROM FamigliePrincipiAttivi f")

            If leggiSingoloElemento = True Then
                StrSQL.AppendLine("         INNER JOIN capitolatoclienteXFamigliePrincipiAttiviRilevati fp ")
                StrSQL.AppendLine("            ON fp.fam_cod = f.fam_cod ")

                StrSQL.AppendLine("     WHERE fp.Capitolato_cod = " & capitolato_cod)
                StrSQL.AppendLine("     AND fp.fam_cod = f.fam_cod")
                If partecipaSommatoria <> -1 Then
                    StrSQL.AppendLine("     AND partecipaSommatoria = " & partecipa & " AND LMR <> 0")
                Else
                    StrSQL.AppendLine("     AND LMR = 0")
                End If
            End If

            StrSQL.AppendLine("     ) AS QUERY")

            StrSQL.AppendLine("     ORDER BY Descrizione")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            DT = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return DT
    End Function
End Class


Public Class Capitolato_W
    Inherits AgronicaCoreDataProvider.DataProvider
    Private nome_tabella As String = "CapitolatoCliente"
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="properties">Dictionary del modello</param>
    ''' <param name="parametriInjection">Parametri dei campi. Devono essere recuperati dalla 
    ''' classe preposta alla gestione datamodel usata per creare i Dictionary</param>
    ''' <param name="_objParametriServer">Parametri server</param>
    ''' <returns></returns>
    Public Function Scrivi(CapitolatoClienteDataModel As CapitolatoCliente, ByRef _objParametriServer As AgronicaCoreParametri) As Boolean
        Dim nomeRoutine As String = "AgronicaCoreCapitolatoClienteDAL.Capitolato_W.Scrivi()"
        Dim messaggioErrore As String = ""
        Dim strSql As String = ""
        Dim xRisp As Boolean = False
        Dim sql As String = ""
        Dim properties As New Dictionary(Of String, String)
        Dim parametriInjection As Object = Nothing


        Try
            Using dmu As New DatamodelUtils
                ' -- crea un Dictionary delle proprietà dal modello e un Dictionary dei parametri e crea oggetto parametri 
                xRisp = dmu.getPropertysOfDataModell(CapitolatoClienteDataModel, properties, parametriInjection)
                ' -- setta parametri tornati nell'oggetto base 
                MyBase.SettaParametriPrecedenti(parametriInjection)
                ' -- crea sql inserimento dati 
                strSql = dmu.getSqlInserisci(nome_tabella, properties)
                ' -- esegue inserimento dati
                xRisp = EseguiQuery_Scrittura(_objParametriServer, strSql, nomeRoutine)
            End Using

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(_objParametriServer, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" + nomeRoutine & "] : " + messaggioErrore)
        End Try

        Return xRisp
    End Function

    Public Function Aggiorna(Capitolato_COD As String, CapitolatoClienteDataModel As CapitolatoCliente, ByRef _objParametriServer As AgronicaCoreParametri) As Boolean
        Dim nomeRoutine As String = "AgronicaCoreCapitolatoClienteDAL.Capitolato_W.Aggiorna()"
        Dim messaggioErrore As String = ""
        Dim strSql As String = ""
        Dim xRisp As Boolean = False
        Dim sql As String = ""
        Dim properties As New Dictionary(Of String, String)
        Dim parametriInjection As Object = Nothing


        Try
            Using dmu As New DatamodelUtils
                ' -- crea un Dictionary delle proprietà dal modello e un Dictionary dei parametri e crea oggetto parametri 
                xRisp = dmu.getPropertysOfDataModell(CapitolatoClienteDataModel, properties, parametriInjection)
                ' -- setta parametri tornati nell'oggetto base 
                MyBase.SettaParametriPrecedenti(parametriInjection)
                ' -- crea sql inserimento dati 
                strSql = dmu.getSqlAggiorna(nome_tabella, properties)
                strSql = strSql + " WHERE Capitolato_COD = '" + Agro_SQL_SaveText(Capitolato_COD) + "' "
                ' -- esegue inserimento dati
                xRisp = EseguiQuery_Scrittura(_objParametriServer, strSql, nomeRoutine)
            End Using

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(_objParametriServer, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" + nomeRoutine & "] : " + messaggioErrore)
        End Try

        Return xRisp
    End Function

End Class