Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class ModelloColturaAvversita_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiSpecieXModelliXPiva(ByVal Piva_Superuser As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreModelliPrevisionaliDAL.LeggiSpecieXModelliXPiva()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0
            Stb.AppendLine("SELECT DISTINCT ")
            Stb.AppendLine("	modelli.Tipo_Visibilita ")
            Stb.AppendLine("    , modelli.Mod_Cod ")
            Stb.AppendLine("    , msa.Veg_Cod ")
            Stb.AppendLine("    , veg.Veg_Des ")
            Stb.AppendLine("FROM ModellixSpecieXAvversita msa ")
            Stb.AppendLine("INNER JOIN SpecieVegetali                                           veg     ON veg.Veg_Cod = msa.Veg_Cod ")
            Stb.AppendLine("INNER JOIN ModelliPrevisionali                                      modelli ON modelli.Mod_cod = msa.Mod_cod ")
            Stb.AppendLine("LEFT JOIN ModelliPrevisionaliXpiva_superUser_OperazioniAutorizzate  opaut   ON opaut.Mod_Cod = modelli.Mod_Cod")
            Stb.AppendLine("WHERE msa.attivo = 1 ")
            Stb.AppendLine("AND (opaut.Piva_Superuser IS NULL OR opaut.Piva_Superuser = '" & Agro_SQL_SaveText(Piva_Superuser) & "') ")
            Stb.AppendLine("ORDER BY Veg_Des ")

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

    Public Function LeggiModelliXVegXPiva(ByVal Veg_Cod As Integer, ByVal Piva_Superuser As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.LeggiModelliXAvvXPiva()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0
            Stb.AppendLine("SELECT ")
            Stb.AppendLine("    CAST(Av_Cod AS VARCHAR(10)) + '-' +   CAST(Veg_Cod AS VARCHAR(10)) + '-' + CAST(Mod_Cod AS VARCHAR(10)) + '-' +  CAST(Alg_Cod AS VARCHAR(10)) AS cAvModAlg_Cod ")
            Dim vDes As String = ""
            If Veg_Cod = -1 Then
                vDes = " veg_des + ' - ' + "
            End If
            'Stb.AppendLine("    , " & vDes & " Av_Des + ' - ' + Mod_Des + CASE WHEN LEN(Alg_Des) > 0 THEN ' - ' ELSE '' END + Alg_Des AS cAvModAlg_Des ")
            Stb.AppendLine("    , " & vDes & " Av_Des + ' - ' + Mod_Des + CASE WHEN LEN(Alg_Des) > 0 THEN ' - ' ELSE '' END + Alg_Des + CASE WHEN LEN(Mod_Des_Agg) > 0 THEN ' [' + Mod_Des_Agg + ']' ELSE '' END AS cAvModAlg_Des ")
            Stb.AppendLine("    , Tipo_Visibilita ")
            Stb.AppendLine("    , Mod_Cod AS modello ")
            Stb.AppendLine("    , Av_Des AS descr_avversita ")
            Stb.AppendLine("    , Mod_Des AS descr_modello ")
            Stb.AppendLine("    , Alg_Des AS descr_algoritmo")

            Stb.AppendLine("    , veg_Cod ")
            Stb.AppendLine("    , veg_Des ")

            Stb.AppendLine("FROM ( ")
            Stb.AppendLine("SELECT DISTINCT ")
            Stb.AppendLine("    msa.Av_Cod, msa.Veg_Cod, msa.Mod_Cod, COALESCE(alg.Algoritmo_Cod, 0) AS Alg_Cod, ")
            Stb.AppendLine("    avv.Av_Des_Vol AS Av_Des, ")
            'Stb.AppendLine("    COALESCE(opaut.DescrizioneAggiuntiva, modelli.Mod_Des) AS Mod_Des, ")
            Stb.AppendLine("    modelli.Mod_Des AS Mod_Des, ")
            Stb.AppendLine("    COALESCE(opaut.DescrizioneAggiuntiva, '') AS Mod_Des_Agg, ")
            Stb.AppendLine("    COALESCE(alg.algoritmo_des collate Latin1_General_CI_AS, '') AS Alg_Des, ")
            Stb.AppendLine("	modelli.Tipo_Visibilita ")
            Stb.AppendLine(" , veg.veg_des")
            Stb.AppendLine("FROM ModellixSpecieXAvversita msa ")
            Stb.AppendLine("INNER JOIN SpecieVegetali                                           veg     ON veg.Veg_Cod = msa.Veg_Cod ")
            Stb.AppendLine("INNER JOIN ModelliPrevisionali                                      modelli ON modelli.Mod_cod = msa.Mod_cod ")
            Stb.AppendLine("INNER JOIN avversita                                                avv     ON avv.Av_Cod = msa.AV_Cod ")
            Stb.AppendLine("LEFT JOIN ModelliPrevisionali_Algoritmo                             alg     ON alg.modello = msa.Mod_Cod ")
            Stb.AppendLine("LEFT JOIN ModelliPrevisionaliXpiva_superUser_OperazioniAutorizzate  opaut   ON opaut.Mod_Cod = modelli.Mod_Cod")
            Stb.AppendLine("WHERE msa.attivo = 1 ")
            If Veg_Cod <> -1 Then
                Stb.AppendLine("AND msa.Veg_Cod = " & Veg_Cod)
            End If

            Stb.AppendLine("AND (opaut.Piva_Superuser IS NULL OR opaut.Piva_Superuser = '" & Agro_SQL_SaveText(Piva_Superuser) & "') ")
            Stb.AppendLine(") tbl ")
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
    Public Function Leggi(
            byval Av_Cod As Integer, _ 
            ByVal Veg_Cod As Integer, _
            byval modello As Integer, _        
            ByVal xSelezioneVariabile as enumSelezioneVariabile, _      
            ByVal xFiltroAggiuntivo As String, _
            ByVal xOrderBy As String, _
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            
            Stb.AppendLine(" ") 

            Select Case xSelezioneVariabile
                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    Stb.AppendLine(" select av.Av_Cod as Avversita, MCV.Veg_Cod as Coltura , MCV.Mod_Cod as Modello, veg.Veg_Cod, veg.Veg_Des, pp.Mod_Des as NomeModello, av.Av_Des_Vol, pp.Tipo_Visibilita ")

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    Stb.AppendLine(" select distinct MCV.Mod_cod as Modello, pp.Mod_Des as NomeModello, Tipo_visibilita ")

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni
                    Stb.AppendLine(" select distinct ")
                    Stb.AppendLine(" cast(MCV.Av_Cod as varchar(10)) + '-' +   ") 
                    Stb.AppendLine(" cast(MCV.Veg_Cod as varchar(10)) + '-' +  ") 
                    Stb.AppendLine(" cast(MCV.Mod_cod as varchar(10)) + '-' +   ") 
                    Stb.AppendLine(" cast(isnull(aa.algoritmo_Cod, 0) as varchar(10)) as cAvModAlg_Cod, ") 
                    Stb.AppendLine("  ") 
                    Stb.AppendLine(" av.Av_Des_Vol + ' - ' +  ") 
                    Stb.AppendLine("  pp.Mod_Des +  ") 
                    Stb.AppendLine("  isnull(' - ' + aa.algoritmo_des collate Latin1_General_CI_AS, '') as cAvModAlg_Des"  )
                    Stb.AppendLine("  , pp.Tipo_Visibilita ")
                    Stb.AppendLine("  , pp.Mod_Cod as modello ")
                    Stb.AppendLine("  , av.av_des_vol as descr_avversita")
                    Stb.AppendLine("  , pp.mod_des as descr_modello")
                    Stb.AppendLine("  , isnull(aa.algoritmo_des collate Latin1_General_CI_AS, '') as descr_algoritmo")

                Case Else
                    Stb.AppendLine(" select av.Av_Cod as Avversita, MCV.Veg_Cod as Coltura , MCV.Mod_Cod as Modello, veg.Veg_Cod, veg.Veg_Des, pp.Mod_Des as NomeModello, av.Av_Des_Vol, pp.Tipo_Visibilita ")

            End Select
            
            Stb.AppendLine(" from [dbo].[ModellixSpecieXAvversita] MCV ") 
            Stb.AppendLine("  ") 
            Stb.AppendLine(" inner join SpecieVegetali veg  ") 
            Stb.AppendLine("  on veg.veg_cod = MCV.Veg_Cod ") 
            Stb.AppendLine("  ") 
            Stb.AppendLine(" inner join ModelliPrevisionali pp ") 
            Stb.AppendLine("  on pp.Mod_cod = MCV.Mod_cod ") 
            Stb.AppendLine("  ") 
            Stb.AppendLine(" inner join avversita av ") 
            Stb.AppendLine("  on av.Av_Cod = MCV.AV_Cod ") 
            Stb.AppendLine("  ") 
            Stb.AppendLine("  ") 
            
            Stb.AppendLine(" left join ModelliPrevisionali_Algoritmo aa ") 
            Stb.AppendLine("  on aa.modello = MCV.Mod_Cod ") 
            Stb.AppendLine(" "  )

            Stb.AppendLine(" where MCV.attivo = 1 "  )
            Stb.AppendLine(" and MCV.Veg_Cod = " & Veg_Cod & " " )

            if Av_Cod <> 0 Then
                Stb.AppendLine(" and MCV.Av_cod = " & Av_Cod & "  " )                
            End If
            
            If modello <> 0 Then
                Stb.AppendLine(" MCV.Mod_Cod = " & modello)    
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

   

    '##############################################################################################
    Public Function Leggi_Avversita_Disponibili(
            byval Av_Cod As Integer, _ 
            ByVal Veg_Cod As Integer, _
            byval Tipo_cod As Integer, _        
            ByVal xSelezioneVariabile as enumSelezioneVariabile, _      
            ByVal xFiltroAggiuntivo As String, _
            ByVal xOrderBy As String, _
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0


            Stb.AppendLine(" ")
            Stb.AppendLine(" select distinct MCV.Avversita as av_Cod, av.av_des_vol as av_des ")

            
            Leggi_Comune(Av_Cod, Veg_Cod, Tipo_cod, Stb)


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
    Public Function Leggi_Algoritmo(
            byval Modello As Integer, _             
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0


            Stb.AppendLine(" ")
            Stb.AppendLine(" select Algoritmo_Cod, Algoritmo_Des ")
            Stb.AppendLine(" from ModelliPrevisionali_Algoritmo")
            Stb.AppendLine(" where modello = " & Modello)
            
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
    Private Shared Sub Leggi_Comune(Av_Cod As Integer, Veg_Cod As Integer, Mod_cod As Integer, Stb As Text.StringBuilder)
        
            Stb.AppendLine(" from [dbo].[ModellixSpecieXAvversita] MCV ") 
            Stb.AppendLine("  ") 
            Stb.AppendLine(" inner join SpecieVegetali veg  ") 
            Stb.AppendLine("  on veg.veg_cod = MCV.Veg_Cod ") 
            Stb.AppendLine("  ") 
            Stb.AppendLine(" inner join ModelliPrevisionali pp ") 
            Stb.AppendLine("  on pp.Mod_cod = MCV.Mod_cod ") 
            Stb.AppendLine("  ") 
            Stb.AppendLine(" inner join avversita av ") 
            Stb.AppendLine("  on av.Av_Cod = MCV.AV_Cod ") 
            Stb.AppendLine("  ") 
            Stb.AppendLine("  ") 
            
            Stb.AppendLine(" left join ModelliPrevisionali_Algoritmo aa ") 
            Stb.AppendLine("  on aa.modello = MCV.Mod_Cod ") 
            Stb.AppendLine(" "  )

            Stb.AppendLine(" where MCV.attivo = 1 "  )
            Stb.AppendLine(" and MCV.Veg_Cod = " & Veg_Cod & " " )

            if Av_Cod <> 0 Then
                Stb.AppendLine(" and MCV.Av_cod = " & Av_Cod & "  " )                
            End If
            
            If Mod_cod <> 0 Then
                Stb.AppendLine(" MCV.Mod_Cod = " & Mod_cod)    
            End If
    End Sub
End Class

