Imports System
Imports System.Collections.Generic
Imports System.Runtime.CompilerServices
Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreUmaDal.UMA_Configurazione_MacrousixLavorazioni_W
Imports Newtonsoft.Json

Public Class UMA_Configurazione_MacrousixLavorazioni_R
    Inherits DataProvider

    Public Function Leggi(ByVal Regione_Cod As String,
                          ByVal Macrouso_UMA_Cod As String,
                          ByVal Lav_UMA_Cod As String,
                          ByVal Lav_Cod As Integer,
                          ByVal Id_Attivita As Integer,
                          ByRef objParametri As AgronicaCoreParametri,
                          Optional xFiltroAggiuntivo As String = "",
                          Optional validitaInizio As Date = AGRODATAINIZIO,
                          Optional validitaFine As Date = AGRODATAFINE,
                          Optional regolamentoCod As Integer? = Nothing) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Configurazione_MacrousixLavorazioni_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable
        Dim PivaSuperUser = objParametri.PivaSuperUser

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT UMA_Configurazione_MacrousixLavorazioni.*, ")
            stb.AppendLine("        UMA_Macrousi.Macrouso_UMA_Des, ")
            stb.AppendLine("        UMA_Macrousi.Macrouso_UMA_Cod, ")
            stb.AppendLine("        UMA_Lavorazioni.Lav_UMA_Des, ")
            stb.AppendLine("        UMA_Lavorazioni.Lav_UMA_Cod, ")
            stb.AppendLine("        UMA_Lavorazioni.Maggiorazione_Terreno_MedioTenace, ")
            stb.AppendLine("        UMA_Lavorazioni.GestioneTerzista, ")
            stb.AppendLine("        Operazioni.Lav_Des, ")
            stb.AppendLine("        Operazioni.Lav_Cod, ")
            stb.AppendLine("        Attivita.Id_Attivita, ")
            stb.AppendLine("        Attivita.[Desc], ")
            stb.AppendLine("        UMA_Lavorazioni.Utilizzata_Da_Consorzio_Bonifica ")
            stb.AppendLine(" FROM UMA_Configurazione_MacrousixLavorazioni ")
            stb.AppendLine(" JOIN Operazioni ON UMA_Configurazione_MacrousixLavorazioni.Lav_Cod = Operazioni.Lav_Cod ")
            stb.AppendLine(" JOIN UMA_Macrousi ON UMA_Configurazione_MacrousixLavorazioni.Macrouso_UMA_Cod = UMA_Macrousi.Macrouso_UMA_Cod ")
            stb.AppendLine(" JOIN UMA_Lavorazioni ON UMA_Configurazione_MacrousixLavorazioni.Lav_UMA_Cod = UMA_Lavorazioni.Lav_UMA_Cod ")
            stb.AppendLine(" LEFT JOIN Attivita ON UMA_Configurazione_MacrousixLavorazioni.Id_Attivita = Attivita.Id_Attivita ")
            stb.AppendLine(" WHERE 1 = 1 ")

            If Regione_Cod <> "" Then
                stb.AppendLine(" AND UMA_Configurazione_MacrousixLavorazioni.Regione_Cod = " & Agro_SQL_SaveText_NULL(Regione_Cod) & " ")
            End If

            If Macrouso_UMA_Cod <> "" Then
                stb.AppendLine(" AND UMA_Configurazione_MacrousixLavorazioni.Macrouso_UMA_Cod = " & Agro_SQL_SaveText_NULL(Macrouso_UMA_Cod) & " ")
            End If

            If Lav_UMA_Cod <> "" Then
                stb.AppendLine(" AND UMA_Configurazione_MacrousixLavorazioni.Lav_UMA_Cod = " & Agro_SQL_SaveText_NULL(Lav_UMA_Cod) & " ")
            End If

            If Lav_Cod <> 0 Then
                stb.AppendLine(" AND UMA_Configurazione_MacrousixLavorazioni.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & " ")
            End If

            If Id_Attivita <> 0 Then
                stb.AppendLine(" AND UMA_Configurazione_MacrousixLavorazioni.Id_Attivita = " & Agro_SQL_SaveNum(Id_Attivita) & " ")
            End If

            If validitaInizio <> AGRODATAINIZIO OrElse validitaFine <> AGRODATAFINE Then
                stb.AppendLine(" AND UMA_Configurazione_MacrousixLavorazioni.Validita_Inizio <= " & Agro_SQL_SaveDate(validitaFine) & " ")
                stb.AppendLine(" AND UMA_Configurazione_MacrousixLavorazioni.Validita_Fine >= " & Agro_SQL_SaveDate(validitaInizio) & " ")
            End If

            If Not IsNothing(regolamentoCod) Then
                stb.AppendLine(" AND UMA_Configurazione_MacrousixLavorazioni.Regolamento_Cod IN (0, " & Agro_SQL_SaveText_NULL(regolamentoCod) & ") ")
            End If

            If Not String.IsNullOrEmpty(xFiltroAggiuntivo) Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                'stb.AppendLine(" AND getdate() BETWEEN UMA_Configurazione_MacrousixLavorazioni.Validita_Inizio AND UMA_Configurazione_MacrousixLavorazioni.Validita_Fine ")
            End If

            stb.AppendLine(" ORDER BY UMA_Configurazione_MacrousixLavorazioni.Macrouso_UMA_Cod, UMA_Configurazione_MacrousixLavorazioni.Lav_UMA_Cod, UMA_Configurazione_MacrousixLavorazioni.Ordinamento ")

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

    Public Function Leggi(ByRef objParametri As AgronicaCoreParametri, InizioValidita As String, FineValidita As String) As DataTable
        'Dim FlagConnessioneLocale As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Configurazione_MacrousixLavorazioni.AgronicaCoreDataProvider_Leggi()"

        ' ------- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Dim StrSQL As New System.Text.StringBuilder
            StrSQL.Length = 0

            StrSQL.Append("SELECT 
                                op.LAV_DES,
                              macroUsi.Macrouso_UMA_Des,
                              lavorazioni.Lav_Uma_Des, 
                              Attivita.[Desc],
                              UMA_Configurazione_MacrousixLavorazioni.* 
                           FROM UMA_Configurazione_MacrousixLavorazioni
                           INNER JOIN Operazioni op ON UMA_Configurazione_MacrousixLavorazioni.Lav_Cod = op.LAV_COD
                           INNER JOIN UMA_Macrousi macroUsi ON UMA_Configurazione_MacrousixLavorazioni.Regione_Cod = macroUsi.Regione_Cod
                              AND UMA_Configurazione_MacrousixLavorazioni.Macrouso_UMA_Cod = macroUsi.Macrouso_UMA_Cod
                           INNER JOIN UMA_Lavorazioni lavorazioni ON UMA_Configurazione_MacrousixLavorazioni.Regione_Cod = lavorazioni.Regione_Cod
                              AND UMA_Configurazione_MacrousixLavorazioni.Lav_UMA_Cod = lavorazioni.Lav_UMA_Cod
                           LEFT JOIN Attivita ON Attivita.ID_Attivita = UMA_Configurazione_MacrousixLavorazioni.Id_Attivita")

            StrSQL.AppendLine(" WHERE 1 = 1  ")

            If Not String.IsNullOrEmpty(FineValidita) Then
                StrSQL.AppendLine(" AND UMA_Configurazione_MacrousixLavorazioni.Validita_Inizio <=  " & Agro_SQL_SaveDate(CDate(FineValidita), False))
            End If

            If Not String.IsNullOrEmpty(InizioValidita) Then
                StrSQL.AppendLine(" AND UMA_Configurazione_MacrousixLavorazioni.Validita_Fine >=  " & Agro_SQL_SaveDate(CDate(InizioValidita), False))
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
    Public Function Leggi_Esistente(ByRef objParametri As AgronicaCoreParametri, Righe As List(Of UMA_Configurazione_MacrousixLavorazioni_W.LavorazioneUMA), strNotIn As String) As String
        'Dim FlagConnessioneLocale As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Configurazione_MacrousixLavorazioni.AgronicaCoreDataProvider_Leggi_Esistente()"
        Dim Errori As String = ""
        ' ------- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Utility.VerificaApriConnessione(objParametri, False)

            For Each RigadaControllare In Righe


                Dim StrSQL As New System.Text.StringBuilder
                StrSQL.Length = 0

                StrSQL.Append("SELECT * 
                           FROM UMA_Configurazione_MacrousixLavorazioni ")
                StrSQL.Append(" where ")
                StrSQL.Append($" (Id_Attivita = {Agro_SQL_SaveNum(RigadaControllare.IdAttivita)}  ")
                StrSQL.Append($" AND Regione_Cod = '{Agro_SQL_SaveText(RigadaControllare.RegioneCod) }' ")
                StrSQL.Append($" AND Macrouso_UMA_Cod = '{Agro_SQL_SaveText(RigadaControllare.MacrousoUMACod)}' ")
                StrSQL.Append($" And Lav_UMA_Cod = '{Agro_SQL_SaveText(RigadaControllare.LavUMACod)}' ")
                StrSQL.Append($" And Lav_Cod = '{Agro_SQL_SaveText(RigadaControllare.LavCod)}' ")
                If RigadaControllare.Regolamento_Cod <> 0 Then
                    StrSQL.Append($" And Regolamento_Cod in (0, {Agro_SQL_SaveNum(RigadaControllare.Regolamento_Cod)}) ")
                Else
                    StrSQL.Append($" And Regolamento_Cod in (0,1,4) ")
                End If

                StrSQL.Append($" And id <> {Agro_SQL_SaveNum(RigadaControllare.ID)} ")
                If String.IsNullOrEmpty(strNotIn) = False Then StrSQL.Append($" And id not in ({strNotIn}) ")
                StrSQL.Append($" ) and ((  {Agro_SQL_SaveDate(RigadaControllare.ValiditaInizio)} >= Validita_Inizio and {Agro_SQL_SaveDate(RigadaControllare.ValiditaInizio)} <= Validita_Fine ) ")
                StrSQL.Append($" or (  {Agro_SQL_SaveDate(RigadaControllare.ValiditaFine)}  >= Validita_Inizio And {Agro_SQL_SaveDate(RigadaControllare.ValiditaFine)} <= Validita_Fine )")
                StrSQL.Append($" or ( {Agro_SQL_SaveDate(RigadaControllare.ValiditaInizio)} <= Validita_Inizio And {Agro_SQL_SaveDate(RigadaControllare.ValiditaFine)} >= Validita_Fine ))")

                '--------------------------------------------------------------------------
                DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
                '--------------------------------------------------------------------------
                If DT.Rows.Count > 0 Then
                    Dim valInizDaControllare As Date = RigadaControllare.ValiditaInizio
                    Dim valFineDaControllare As Date = RigadaControllare.ValiditaFine
                    Dim valInizSovrapposto As Date = DT.Rows(0)("Validita_Inizio")
                    Dim valFineSovrapposto As Date = DT.Rows(0)("Validita_Fine")
                    Dim tipoRegolamento_Cod As Integer = DT.Rows(0)("Regolamento_Cod")
                    Dim Regolamento_CodDes As String
                    If tipoRegolamento_Cod = 1 Then
                        Regolamento_CodDes = "CONVENZIONALE"
                    ElseIf tipoRegolamento_Cod = 4 Then
                        Regolamento_CodDes = "BIOLOGICO"
                    Else
                        Regolamento_CodDes = "ENTRAMBI"
                    End If

                    Dim Regolamento_CodDesdaj As String
                    If RigadaControllare.Regolamento_Cod = 1 Then
                        Regolamento_CodDesdaj = "CONVENZIONALE"
                    ElseIf RigadaControllare.Regolamento_Cod = 4 Then
                        Regolamento_CodDesdaj = "BIOLOGICO"
                    Else
                        Regolamento_CodDesdaj = "ENTRAMBI"
                    End If

                    Errori = $"L'elemento {RigadaControllare.UMAMacrousi_MacrousoUMADes} - {Regolamento_CodDesdaj} - {RigadaControllare.UMALavorazioni_LavUmaDes} - {RigadaControllare.Operazioni_LavDeS} - {RigadaControllare.Attivita_Desc} nel periodo dal {valInizDaControllare.ToString("dd/MM/yyyy")} al {valFineDaControllare.ToString("dd/MM/yyyy")} si sovrappone al regolamento {Regolamento_CodDes} del periodo dal {valInizSovrapposto.ToString("dd/MM/yyyy")} al {valFineSovrapposto.ToString("dd/MM/yyyy")}"
                    Return Errori
                End If
            Next

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Nothing
    End Function

    Public Function VerificaElementoNonEsisteInDB(lav As UMA_Configurazione_MacrousixLavorazioni_W.LavorazioneUMA,
                                                  efConnString As String) As Boolean
        Using dal As New Gias_DeveloperServer_Entities(efConnString)
            Dim exists = dal.UMA_Configurazione_MacrousixLavorazioni.Any(Function(s) s.Regione_Cod = lav.RegioneCod AndAlso s.Macrouso_UMA_Cod = lav.MacrousoUMACod AndAlso s.Lav_UMA_Cod = lav.LavUMACod AndAlso s.Lav_Cod = lav.LavCod AndAlso s.Id_Attivita = lav.IdAttivita)
            Return exists
        End Using
    End Function
    Public Function VerificaPeriodiSovrappostiDB(righe As List(Of UMA_Configurazione_MacrousixLavorazioni_W.LavorazioneUMA),
                                                 righeCancellate As List(Of UMA_Configurazione_MacrousixLavorazioni_W.LavorazioneUMA),
                                                  efConnString As String,
                                                 objParametri As AgronicaCoreParametri) As String
        Dim Errori As String = ""
        Dim Soprapposto As Boolean = False



        For Each S In righe
            Dim CodRegolamento As New List(Of Integer)()
            If S.Regolamento_Cod <> 0 Then
                CodRegolamento.Add(0)
                CodRegolamento.Add(S.Regolamento_Cod)
            Else
                CodRegolamento.Add(0)
                CodRegolamento.Add(1)
                CodRegolamento.Add(4)
            End If
            Dim Lavorazione = From R In righe
                              Where ((R.IdAttivita = S.IdAttivita) AndAlso (R.RegioneCod = S.RegioneCod) AndAlso (R.MacrousoUMACod = S.MacrousoUMACod) AndAlso (R.LavUMACod = S.LavUMACod) AndAlso (R.LavCod = S.LavCod) AndAlso (CodRegolamento.Contains(R.Regolamento_Cod))) _
                                AndAlso ((S.ValiditaInizio >= R.ValiditaInizio AndAlso S.ValiditaInizio <= R.ValiditaFine) _
                                      OrElse
                                      (S.ValiditaFine >= R.ValiditaInizio AndAlso S.ValiditaFine <= R.ValiditaFine) _
                                      OrElse
                                      (S.ValiditaInizio <= R.ValiditaInizio AndAlso S.ValiditaFine >= R.ValiditaFine))
                              Select R

            If Lavorazione.Count() > 1 Then
                Dim sov = From R In Lavorazione
                          Where R.ValiditaInizio <> S.ValiditaInizio OrElse R.ValiditaFine <> S.ValiditaFine OrElse R.ID <> S.ID
                Dim valInizDaControllare As Date = S.ValiditaInizio
                Dim valFineDaControllare As Date = S.ValiditaFine
                Dim tipoRegolamento_Cod As Integer = S.Regolamento_Cod
                Dim Regolamento_CodDes As String
                If tipoRegolamento_Cod = 1 Then
                    Regolamento_CodDes = "CONVENZIONALE"
                ElseIf tipoRegolamento_Cod = 4 Then
                    Regolamento_CodDes = "BIOLOGICO"
                Else
                    Regolamento_CodDes = "ENTRAMBI"
                End If




                If sov.Count() = 0 Then
                    Errori = $"L'elemento {S.UMAMacrousi_MacrousoUMADes} - {Regolamento_CodDes} - {S.UMALavorazioni_LavUmaDes} - {S.Operazioni_LavDeS} - {S.Attivita_Desc} nel periodo dal {valInizDaControllare.ToString("dd/MM/yyyy")} al {valFineDaControllare.ToString("dd/MM/yyyy")} si sovrappone ad uno dei periodi in corso di modifica"
                Else
                    Dim valInizSovrapposto As Date = sov.FirstOrDefault().ValiditaInizio
                    Dim valFineSovrapposto As Date = sov.FirstOrDefault().ValiditaFine
                    Dim Regolamento_CodDesdaj As String
                    If sov.FirstOrDefault().Regolamento_Cod = 1 Then
                        Regolamento_CodDesdaj = "CONVENZIONALE"
                    ElseIf sov.FirstOrDefault().Regolamento_Cod = 4 Then
                        Regolamento_CodDesdaj = "BIOLOGICO"
                    Else
                        Regolamento_CodDesdaj = "ENTRAMBI"
                    End If
                    Errori = $"L'elemento {S.UMAMacrousi_MacrousoUMADes} - {Regolamento_CodDes} - {S.UMALavorazioni_LavUmaDes} - {S.Operazioni_LavDeS} - {S.Attivita_Desc} nel periodo dal {valInizDaControllare.ToString("dd/MM/yyyy")} al {valFineDaControllare.ToString("dd/MM/yyyy")} si sovrappone al regolamento {Regolamento_CodDesdaj } del periodo dal {valInizSovrapposto.ToString("dd/MM/yyyy")} al {valFineSovrapposto.ToString("dd/MM/yyyy")}"
                End If
                Return Errori
            End If

        Next

        Dim StrNotin As String = ""
        For Each C In righeCancellate
            If StrNotin <> "" Then
                StrNotin += ","
            End If
            StrNotin += C.ID.ToString()
        Next
        For Each C In righe
            If C.ID > 0 Then
                If StrNotin <> "" Then
                    StrNotin += ","
                End If
                StrNotin += C.ID.ToString()
            End If
        Next
        Return Leggi_Esistente(objParametri, righe, StrNotin)

        'Using dal As New Gias_DeveloperServer_Entities(efConnString)
        '    For Each S In righe
        '        Dim Lav = From R In dal.UMA_Configurazione_MacrousixLavorazioni
        '                  Where ((R.Id_Attivita = S.IdAttivita AndAlso R.Regione_Cod = S.RegioneCod AndAlso R.Macrouso_UMA_Cod = S.MacrousoUMACod AndAlso R.Lav_UMA_Cod = S.LavUMACod AndAlso R.Lav_Cod = S.LavCod
        '                            ) _    ' da aggiungere AndAlso S.ID <> 0 AndAlso Not righeCancellate.Any(Function(p) p.ID = S.ID)
        '                        AndAlso ((S.ValiditaInizio >= R.Validita_Inizio AndAlso S.ValiditaInizio <= R.Validita_Fine) _
        '                              OrElse
        '                              (S.ValiditaFine >= R.Validita_Inizio AndAlso S.ValiditaFine <= R.Validita_Fine) _
        '                              OrElse
        '                              (S.ValiditaInizio <= R.Validita_Inizio AndAlso S.ValiditaFine >= R.Validita_Fine)))
        '                  Select R

        '        If Lav.Count > 0 Then
        '            'Dim lavcan = From R In Lav Where (Not righeCancellate.Any(Function(p) p.ID = R.ID AndAlso R.ID <> S.ID))
        '            'If lavcan.Count > 0 Then
        '            Return S
        '            'End If
        '        End If
        '    Next
        'End Using

        Return Nothing

    End Function
    ' ******************************************** Query UMA_Macrousi e crea l'elenco ********************************************
    Public Function LeggiDropdown_UMA_Macrousi(objparametri_Server As AgronicaCoreParametri) As List(Of VoceElencoDiDropdown)

        Dim NomeRoutine As String = "UMA_Configurazione_MacrousixLavorazioni.UMA_.UMA_Configurazione_MacrousixLavorazioni_R.LeggiDropdown_UMA_Macrousi()"

        Dim MessaggioErrore As String = ""
        Dim elenco As New List(Of VoceElencoDiDropdown)
        Dim stb As New System.Text.StringBuilder

        Try
            Dim gefutils As New Gias_EF_Utility
            Dim efConnString As String = gefutils.GetEntityConnectionString(objparametri_Server.StringaConnessione)

            Using db As New Gias_DeveloperServer_Entities(efConnString)
                Dim dbElem = db.UMA_Macrousi.ToList()
                elenco = dbElem.Select(Function(s) New VoceElencoDiDropdown(s.Macrouso_UMA_Cod, s.Macrouso_UMA_Des)).ToList()
            End Using

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objparametri_Server, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return elenco
    End Function

    ' ******************************************** Query UMA_Lavorazioni e crea l'elenco ********************************************
    Public Function LeggiDropdown_UMALavorazioni(objparametri_Server As AgronicaCoreParametri) As List(Of VoceElencoDiDropdown)

        Dim NomeRoutine As String = "UMA_Configurazione_MacrousixLavorazioni.UMA_.UMA_Configurazione_MacrousixLavorazioni_R.LeggiDropdown_UMALavorazioni()"

        Dim MessaggioErrore As String = ""
        Dim elenco As New List(Of VoceElencoDiDropdown)
        Dim stb As New System.Text.StringBuilder

        Try
            Dim gefutils As New Gias_EF_Utility
            Dim efConnString As String = gefutils.GetEntityConnectionString(objparametri_Server.StringaConnessione)

            Using db As New Gias_DeveloperServer_Entities(efConnString)
                Dim dbElem = db.UMA_Lavorazioni.ToList()
                elenco = dbElem.Select(Function(s) New VoceElencoDiDropdown(s.Lav_UMA_Cod, s.Lav_UMA_Des)).ToList()
            End Using

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objparametri_Server, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return elenco
    End Function

    ' ******************************************** Query Operazioni e crea l'elenco ********************************************
    Public Function LeggiDropdown_Operazioni(objparametri_Server As AgronicaCoreParametri) As List(Of VoceElencoDiDropdown)

        Dim NomeRoutine As String = "UMA_Configurazione_MacrousixLavorazioni.UMA_.UMA_Configurazione_MacrousixLavorazioni_R.LeggiDropdown_Operazioni()"

        Dim MessaggioErrore As String = ""
        Dim elenco As New List(Of VoceElencoDiDropdown)
        Dim stb As New System.Text.StringBuilder

        Try
            Dim gefutils As New Gias_EF_Utility
            Dim efConnString As String = gefutils.GetEntityConnectionString(objparametri_Server.StringaConnessione)

            Using db As New Gias_DeveloperServer_Entities(efConnString)
                Dim dbElem = db.Operazioni.ToList()
                elenco = dbElem.Select(Function(s) New VoceElencoDiDropdown(s.LAV_COD, s.LAV_DES)).ToList()
            End Using

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objparametri_Server, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return elenco
    End Function

    ' ******************************************** Query Attivita e crea l'elenco ********************************************
    Public Function LeggiDropdown_Attivita(objparametri_Server As AgronicaCoreParametri) As List(Of VoceElencoDiDropdown)

        Dim NomeRoutine As String = "UMA_Configurazione_MacrousixLavorazioni.UMA_.UMA_Configurazione_MacrousixLavorazioni_R.LeggiDropdown_Attivita()"

        Dim MessaggioErrore As String = ""
        Dim elenco As New List(Of VoceElencoDiDropdown)
        Dim stb As New System.Text.StringBuilder

        Try
            Dim gefutils As New Gias_EF_Utility
            Dim efConnString As String = gefutils.GetEntityConnectionString(objparametri_Server.StringaConnessione)

            Using db As New Gias_DeveloperServer_Entities(efConnString)
                Dim dbElem = db.Attivita.ToList()
                elenco = dbElem.Select(Function(s) New VoceElencoDiDropdown(s.ID_Attivita, s.Desc)).ToList()
            End Using

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objparametri_Server, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return elenco
    End Function



    Class VoceElencoDiDropdown
        Public Code As Integer
        Public Descrizione As String

        Public Sub New(cod As Integer, des As String)
            Code = cod
            Descrizione = des
        End Sub
    End Class
End Class

Public Class UMA_Configurazione_MacrousixLavorazioni_W
    Inherits DataProvider

    ''' <summary>
    ''' Aggiungi ogni riga inserita dal utente nel database. SaveChanges() è 
    ''' chiamato ulteriormente dal metodo Salva_GrigliaLavorazioniUMA.
    ''' </summary>
    ''' <returns></returns>
    Public Function AggiungiNouvi(listLav As List(Of LavorazioneUMA),
                                  context As Gias_DeveloperServer_Entities,
                                  objParametri As AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Configurazione_MacrousixLavorazioni.AggiungiNouvi()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Dim PivaSuperUser = objParametri.PivaSuperUser


        Try
            For Each l In listLav
                '    context.UMA_Configurazione_MacrousixLavorazioni.Add(l.ToLavorazioneUMADB())
                StrSQL = New System.Text.StringBuilder

                StrSQL.Append("INSERT INTO UMA_Configurazione_MacrousixLavorazioni ( ")
                StrSQL.Append("           Regione_Cod,Macrouso_UMA_Cod,Lav_UMA_Cod,Lav_Cod,Id_Attivita")
                StrSQL.Append("           ,Tipo_Operazione,Gasolio_Lt,Benzina_Lt")
                StrSQL.Append("           ,Ordinamento,N_Max_Operazioni,[Default],inviato")
                StrSQL.Append("           ,datainvio,Data_Creazione,Data_Modifica,Username_Creazione")
                StrSQL.Append("           ,Username_Modifica,Validita_Inizio,Validita_Fine")
                StrSQL.Append("           ,Udm_Alternativa,Gasolio_LtxBiologico,Benzina_LtxBiologico")
                StrSQL.Append("           ,Limite_Max,Max_xHa,Regolamento_Cod, Coeff_acq_distr ")
                StrSQL.Append("      ,[FlagNoteCompObbl])        ")

                StrSQL.Append("VALUES (")
                StrSQL.Append("          '" & Agro_SQL_SaveText(l.RegioneCod) & "' ")
                StrSQL.Append("  ,       '" & Agro_SQL_SaveText(l.MacrousoUMACod) & "' ")
                StrSQL.Append("  ,       '" & Agro_SQL_SaveText(l.LavUMACod) & "' ")
                StrSQL.Append("  ,       '" & Agro_SQL_SaveText(l.LavCod) & "' ")
                StrSQL.Append("  ,       " & Agro_SQL_SaveNum(l.IdAttivita) & " ")
                StrSQL.Append("  ,       " & Agro_SQL_SaveNum(l.TipoOperazioneCod) & " ")
                StrSQL.Append("  ,       " & Agro_SQL_SaveNum(l.GasolioLt) & " ")
                StrSQL.Append("  ,       " & Agro_SQL_SaveNum(l.BenzinaLt) & " ")
                StrSQL.Append("  ,       " & Agro_SQL_SaveNum(l.OrdinamentoCod) & " ")
                StrSQL.Append("  ,       " & Agro_SQL_SaveNum(l.NMaxOperazioni) & " ")
                StrSQL.Append("  ,       " & Agro_SQL_SaveNum(l.Default) & " ")
                StrSQL.Append("  ,       " & Agro_SQL_SaveNum(l.Inviato) & " ")
                StrSQL.Append("  ,       " & Agro_SQL_SaveDate(l.DataInvio) & " ")
                StrSQL.Append("  ,       " & Agro_SQL_SaveDate(l.DataCreazione) & " ")
                StrSQL.Append("  ,       " & Agro_SQL_SaveDate(l.DataModifica) & " ")
                StrSQL.Append("  ,       '" & Agro_SQL_SaveText(l.UsernameCreazione) & "' ")
                StrSQL.Append("  ,       '" & Agro_SQL_SaveText(l.UsernameModifica) & "' ")
                StrSQL.Append("  ,       " & Agro_SQL_SaveDate(l.ValiditaInizio) & " ")
                StrSQL.Append("  ,       " & Agro_SQL_SaveDate(l.ValiditaFine) & " ")
                StrSQL.Append("  ,       '" & Agro_SQL_SaveText(l.UDMAlternativa) & "' ")
                StrSQL.Append("  ,       " & Agro_SQL_SaveNum(l.GasolinoLTxBiologico) & " ")
                StrSQL.Append("  ,       " & Agro_SQL_SaveNum(l.BensinaLTxBiologico) & " ")
                StrSQL.Append("  ,       " & Agro_SQL_SaveNum(l.LimiteMax) & " ")
                StrSQL.Append("  ,       " & Agro_SQL_SaveNum(l.MaxxHa) & " ")
                StrSQL.Append("  ,       " & Agro_SQL_SaveNum(l.Regolamento_Cod) & " ")
                StrSQL.Append("  ,       " & Agro_SQL_SaveNum(l.Coefficiente_Distribuzione_Acqua) & " ")
                StrSQL.Append("  ,       " & Agro_SQL_SaveNum(l.FlagNoteCompObbl) & " ")
                StrSQL.Append(" )")

                xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)

            Next
            'context.SaveChanges()
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return True
    End Function

    Public Function Rimuovi(l As LavorazioneUMA, context As Gias_DeveloperServer_Entities, objParametri As AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Configurazione_MacrousixLavorazioni.Rimuovi()"

        ' ------------- Variabili -------------

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Dim PivaSuperUser = objParametri.PivaSuperUser

        Try

            StrSQL.Append("DELETE FROM UMA_Configurazione_MacrousixLavorazioni where ID =  " & Agro_SQL_SaveNum(l.ID) & " ")
            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


            'Dim record As UMA_Configurazione_MacrousixLavorazioni = context.UMA_Configurazione_MacrousixLavorazioni.Where(Function(s) s.Regione_Cod = l.RegioneCod AndAlso s.Lav_UMA_Cod = l.LavUMACod AndAlso s.Macrouso_UMA_Cod = l.MacrousoUMACod AndAlso s.Tipo_Operazione = l.TipoOperazioneCod AndAlso s.Id_Attivita = l.IdAttivita).FirstOrDefault()

            'If Not record Is Nothing Then
            '    context.UMA_Configurazione_MacrousixLavorazioni.Remove(record)
            '    context.SaveChanges()
            'End If
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return True
    End Function

    Public Function Aggiorna(righeModificateArr As List(Of LavorazioneUMA),
                             context As Gias_DeveloperServer_Entities,
                             ByRef objParametri As AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Configurazione_MacrousixLavorazioni.Aggiorna()"

        ' ------------- Variabili -------------
        Dim DT As DataTable
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Dim PivaSuperUser = objParametri.PivaSuperUser

        Try

            For Each l In righeModificateArr

                StrSQL = New System.Text.StringBuilder

                StrSQL.Append("UPDATE  UMA_Configurazione_MacrousixLavorazioni SET ")
                '   StrSQL.Append("  [Regione_Cod]            =         '" & Agro_SQL_SaveText(l.RegioneCod) & "' ")
                'StrSQL.Append(" ,[Macrouso_UMA_Cod]       =         '" & Agro_SQL_SaveText(l.MacrousoUMACod) & "' ")
                'StrSQL.Append(" ,[Lav_UMA_Cod]            =         '" & Agro_SQL_SaveText(l.LavUMACod) & "' ")
                'StrSQL.Append(" ,[Lav_Cod]                =         '" & Agro_SQL_SaveText(l.LavCod) & "' ")
                'StrSQL.Append(" ,[Id_Attivita]            =         " & Agro_SQL_SaveNum(l.IdAttivita) & " ")
                StrSQL.Append(" [Tipo_Operazione]        =         " & Agro_SQL_SaveNum(l.TipoOperazioneCod) & " ")
                StrSQL.Append(" ,[Gasolio_Lt]             =         " & Agro_SQL_SaveNum(l.GasolioLt) & " ")
                StrSQL.Append(" ,[Benzina_Lt]             =         " & Agro_SQL_SaveNum(l.BenzinaLt) & " ")
                StrSQL.Append(" ,[Ordinamento]            =         " & Agro_SQL_SaveNum(l.OrdinamentoCod) & " ")
                StrSQL.Append(" ,[N_Max_Operazioni]       =         " & Agro_SQL_SaveNum(l.NMaxOperazioni) & " ")
                StrSQL.Append(" ,[Default]                =         " & Agro_SQL_SaveNum(l.Default) & " ")
                StrSQL.Append(" ,[inviato]                =         " & Agro_SQL_SaveNum(l.Inviato) & " ")
                StrSQL.Append(" ,[datainvio]              =         " & Agro_SQL_SaveDate(l.DataInvio) & " ")
                StrSQL.Append(" ,[Data_Modifica]          =         " & Agro_SQL_SaveDate(l.DataModifica) & " ")
                StrSQL.Append(" ,[Username_Modifica]      =         '" & Agro_SQL_SaveText(l.UsernameModifica) & "' ")
                StrSQL.Append(" ,[Validita_Inizio]        =         " & Agro_SQL_SaveDate(l.ValiditaInizio) & " ")
                StrSQL.Append(" ,[Validita_Fine]          =         " & Agro_SQL_SaveDate(l.ValiditaFine) & " ")
                StrSQL.Append(" ,[Udm_Alternativa]        =         '" & Agro_SQL_SaveText(l.UDMAlternativa) & "' ")
                StrSQL.Append(" ,[Gasolio_LtxBiologico]   =         " & Agro_SQL_SaveNum(l.GasolinoLTxBiologico) & " ")
                StrSQL.Append(" ,[Benzina_LtxBiologico]   =         " & Agro_SQL_SaveNum(l.BensinaLTxBiologico) & " ")
                StrSQL.Append(" ,[Limite_Max]             =         " & Agro_SQL_SaveNum(l.LimiteMax) & " ")
                StrSQL.Append(" ,[Max_xHa]                =         " & Agro_SQL_SaveNum(l.MaxxHa) & " ")
                StrSQL.Append(" ,[Coeff_acq_distr]        =         " & Agro_SQL_SaveNum(l.Coefficiente_Distribuzione_Acqua) & " ")
                StrSQL.Append(" ,[FlagNoteCompObbl]       =         " & Agro_SQL_SaveNum(l.FlagNoteCompObbl) & " ")
                '  StrSQL.Append(" ,[Regolamento_Cod]                =         " & Agro_SQL_SaveNum(l.Regolamento_Cod) & " ")
                StrSQL.Append(" where ID =  " & Agro_SQL_SaveNum(l.ID) & " ")

                xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)


                'Dim result As UMA_Configurazione_MacrousixLavorazioni = context.UMA_Configurazione_MacrousixLavorazioni.FirstOrDefault(Function(s) s.Regione_Cod = elem.RegioneCod AndAlso s.Macrouso_UMA_Cod = elem.MacrousoUMACod AndAlso s.Lav_UMA_Cod = elem.LavUMACod AndAlso s.Lav_Cod = elem.LavCod AndAlso s.Id_Attivita = elem.IdAttivita)

                'If Not result Is Nothing Then
                '    result.Tipo_Operazione = elem.TipoOperazioneCod
                '    result.Gasolio_Lt = elem.GasolioLt
                '    result.Benzina_Lt = elem.BenzinaLt
                '    result.Ordinamento = elem.OrdinamentoCod
                '    result.N_Max_Operazioni = elem.NMaxOperazioni
                '    result.Default = elem.Default
                '    result.Data_Modifica = elem.DataModifica
                '    result.Username_Modifica = elem.UsernameModifica
                '    result.Validita_Inizio = elem.ValiditaInizio
                '    result.Validita_Fine = elem.ValiditaFine
                '    result.Udm_Alternativa = elem.UDMAlternativa
                '    result.Gasolio_LtxBiologico = elem.GasolinoLTxBiologico
                '    result.Benzina_LtxBiologico = elem.BensinaLTxBiologico
                '    result.Limite_Max = elem.LimiteMax
                '    result.Max_xHa = elem.MaxxHa
                'End If
            Next
            context.SaveChanges()
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return True
    End Function


    Class LavorazioneUMA
        ' *************************** Collone prese in join con altre tabelle ***************************
        Public Operazioni_LavDeS As String
        Public UMAMacrousi_MacrousoUMADes As String
        Public UMALavorazioni_LavUmaDes As String
        Public Attivita_Desc As String

        ' ************************* Collone della tabella UMA_Configurazione_MacrousixLavorazioni *************************
        Public RegioneCod As String
        Public MacrousoUMACod As String
        Public LavUMACod As String
        Public LavCod As Integer
        Public IdAttivita As Integer
        Public TipoOperazioneCod As Integer
        Public GasolioLt As Double
        Public BenzinaLt As Double
        Public OrdinamentoCod As Integer
        Public NMaxOperazioni As Integer
        Public [Default] As Integer?
        Public Inviato As Short
        Public DataInvio As Date?
        Public DataCreazione As Date?
        Public DataModifica As Date?
        Public UsernameCreazione As String
        Public UsernameModifica As String
        Public ValiditaInizio As Date?
        Public ValiditaFine As Date?
        Public UDMAlternativa As String
        Public GasolinoLTxBiologico As Double
        Public BensinaLTxBiologico As Double
        Public LimiteMax As Integer
        Public MaxxHa As Double
        Public ID As Integer
        Public Regolamento_Cod As Integer
        Public Coefficiente_Distribuzione_Acqua As Double
        Public FlagNoteCompObbl As Integer
    End Class

End Class






Module Extensions
    <Extension()>
    Function ToLavorazioneUMADB(ByVal l As UMA_Configurazione_MacrousixLavorazioni_W.LavorazioneUMA) As AgronicaCoreEntityFramework_POCO.UMA_Configurazione_MacrousixLavorazioni
        Dim r As New AgronicaCoreEntityFramework_POCO.UMA_Configurazione_MacrousixLavorazioni
        r.Regione_Cod = l.RegioneCod
        r.Macrouso_UMA_Cod = l.MacrousoUMACod
        r.Lav_UMA_Cod = l.LavUMACod
        r.Lav_Cod = l.LavCod
        r.Id_Attivita = l.IdAttivita
        r.Tipo_Operazione = l.TipoOperazioneCod
        r.Gasolio_Lt = l.GasolioLt
        r.Benzina_Lt = l.BenzinaLt
        r.Ordinamento = l.OrdinamentoCod
        r.N_Max_Operazioni = l.NMaxOperazioni
        r.Default = l.Default
        r.inviato = 0
        r.datainvio = l.DataInvio

        r.Data_Creazione = l.DataCreazione
        r.Data_Modifica = l.DataModifica

        r.Username_Creazione = l.UsernameCreazione
        r.Username_Modifica = l.UsernameModifica
        r.Validita_Inizio = l.ValiditaInizio
        r.Validita_Fine = l.ValiditaFine
        r.Udm_Alternativa = l.UDMAlternativa
        r.Gasolio_LtxBiologico = l.GasolinoLTxBiologico
        r.Benzina_LtxBiologico = l.BensinaLTxBiologico
        r.Limite_Max = l.LimiteMax
        r.Max_xHa = l.MaxxHa
        Return r
    End Function
End Module
