Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate


Public Class AnalisiCosti_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '###############################################################################
    Public Function Leggi_Ripetizione_Macchina_CostiAccessori(ByVal Piva As String,
                                                              ByVal Dal_Data_Verifica As Date,
                                                              ByVal Al_Data_Verifica As Date,
                                                              ByRef objParametri As AgronicaCoreParametri
                                                              ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.AnalisiCosti_R.Leggi_Ripetizione_Macchina_CostiAccessori()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" select Movimenti_dettagli.Id_Agenda, Mat_Cod, COUNT(*) as ripetizioni, des_lib, data_movimento " & vbCrLf)
            StrSQL.Append(" , parco_macchine.mac_des + ' - ' + parco_macchine.modello AS macchina " & vbCrLf)

            StrSQL.Append(" from Movimenti_dettagli " & vbCrLf)
            StrSQL.Append(" inner join Agenda on agenda.PIVA=Movimenti_dettagli.piva  " & vbCrLf)
            StrSQL.Append(" and agenda.id_agenda=Movimenti_dettagli.id_agenda " & vbCrLf)
            StrSQL.Append(" inner join Movimenti " & vbCrLf)
            StrSQL.Append(" on Movimenti.PIVA=Movimenti_dettagli.piva " & vbCrLf)
            StrSQL.Append(" and Movimenti.id_agenda=Movimenti_dettagli.id_agenda " & vbCrLf)
            StrSQL.Append(" and Movimenti.id_mov=Movimenti_dettagli.id_mov " & vbCrLf)
            StrSQL.Append(" inner join parco_macchine on  parco_macchine.mac_cod=Movimenti_dettagli.mat_cod " & vbCrLf)

            StrSQL.Append("  " & vbCrLf)
            StrSQL.Append(" where Movimenti_dettagli.Elem_Cod= " & MACCHINE & " " & vbCrLf)
            StrSQL.Append(" and Movimenti.CAU_MOV = '" & CAU_IMPUTAZIONE_PARCOMACCHINE & "' " & vbCrLf)
            StrSQL.Append(" and Movimenti_dettagli.piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            StrSQL.Append(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Al_Data_Verifica) & " " & vbCrLf)
            StrSQL.Append(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Dal_Data_Verifica) & " " & vbCrLf)
            StrSQL.Append(" group by Movimenti_dettagli.Id_Agenda, Mat_Cod,des_lib,data_movimento " & vbCrLf)
            StrSQL.Append(" , parco_macchine.mac_des, parco_macchine.modello " & vbCrLf)

            StrSQL.Append(" having COUNT(*) > 1 " & vbCrLf)
            StrSQL.Append(" order by Data_Movimento " & vbCrLf)

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

    '###############################################################################
    Public Function Verifica_Ripetizione_Macchina_CostiAccessori(ByVal Piva As String,
                                                                 ByVal Dal_Data_Verifica As Date,
                                                                 ByVal Al_Data_Verifica As Date,
                                                                 ByRef Str_Errore As String,
                                                                 ByRef objParametri As AgronicaCoreParametri
                                                                 ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.AnalisiCosti_R.Verifica_Ripetizione_Macchina_CostiAccessori()"

        Dim MessaggioErrore As String = ""
        Dim DT As DataTable
        Dim Flag_Errore As Boolean = False

        Try

            DT = Leggi_Ripetizione_Macchina_CostiAccessori(Piva, Dal_Data_Verifica, Al_Data_Verifica, objParametri)

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                'se ci sono record, ci sono registrazioni errate
                Flag_Errore = True

                Str_Errore = "Sono presenti registrazioni di costi accessori errate:" & vbCrLf

                Dim i As Integer
                Dim des_lib, data, ripetizioni, macchina As String
                For i = 0 To DT.Rows.Count - 1

                    des_lib = DT.Rows(i).Item("des_lib")
                    data = DT.Rows(i).Item("data_movimento")
                    ripetizioni = DT.Rows(i).Item("ripetizioni")
                    macchina = DT.Rows(i).Item("macchina")

                    Str_Errore &= data & " " & des_lib & ": " & ripetizioni & " ripetizioni di " & macchina & vbCrLf

                Next

            End If

        Catch ex As Exception
            Flag_Errore = True
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Flag_Errore

    End Function

    '###############################################################################
    Public Function Leggi_Ripetizione_Contatto_CostiAccessori(ByVal Piva As String, _
                                                                ByVal Dal_Data_Verifica As Date, _
                                                                ByVal Al_Data_Verifica As Date, _
                                                                ByRef objParametri As AgronicaCoreParametri _
                                                                ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreContabDAL.AnalisiCosti_R.Leggi_Ripetizione_Contatto_CostiAccessori()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable


        Try

            StrSQL.Length = 0

            StrSQL.Append(" select Movimenti_dettagli.Id_Agenda, Movimenti_dettagli.ID_Attivita,Turno_Cod,Mat_Cod, COUNT(*) as ripetizioni, des_lib, data_movimento  " & vbCrLf)
            '21/02/2017: aggiunti campi da considerare nella gestione costi di SBTF
            StrSQL.Append(" , Movimenti_dettagli.qualifica_cod, Movimenti_dettagli.tariffa_cod " & vbCrLf)
            StrSQL.Append(" , (Contatti.Rag_Soc + contatti.nome + ' ' + Contatti.Cognome) AS Contatto " & vbCrLf)

            StrSQL.Append(" from Movimenti_dettagli  " & vbCrLf)
            StrSQL.Append(" inner join Agenda on agenda.PIVA=Movimenti_dettagli.piva  " & vbCrLf)
            StrSQL.Append(" and agenda.id_agenda=Movimenti_dettagli.id_agenda  " & vbCrLf)
            StrSQL.Append(" inner join Movimenti " & vbCrLf)
            StrSQL.Append(" on Movimenti.PIVA=Movimenti_dettagli.piva " & vbCrLf)
            StrSQL.Append(" and Movimenti.id_agenda=Movimenti_dettagli.id_agenda " & vbCrLf)
            StrSQL.Append(" and Movimenti.id_mov=Movimenti_dettagli.id_mov " & vbCrLf)
            StrSQL.Append(" inner join risorse_umane on  risorse_umane.cod_risum=Movimenti_dettagli.mat_cod " & vbCrLf)
            StrSQL.Append(" inner join contatti on  risorse_umane.cod_contatto=contatti.cod_contatto and risorse_umane.piva=contatti.piva " & vbCrLf)

            StrSQL.Append(" where Movimenti_dettagli.Elem_Cod= " & ELEMCOD_MANODOPERA & " " & vbCrLf)
            ' StrSQL.Append(" -- and Movimenti_dettagli.ID_Attivita <> 0 " & vbCrLf)
            StrSQL.Append(" and Movimenti.CAU_MOV IN ('" & CAU_IMPUTAZIONE_MANODOPERA & "', '" & CAU_IMPUTAZIONE_TERZISTI & "') " & vbCrLf)
            StrSQL.Append(" and Movimenti_dettagli.piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            StrSQL.Append(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Al_Data_Verifica) & " " & vbCrLf)
            StrSQL.Append(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Dal_Data_Verifica) & " " & vbCrLf)
            StrSQL.Append(" group by Movimenti_dettagli.Id_Agenda, Movimenti_dettagli.ID_Attivita,Turno_Cod,Mat_Cod,des_lib,data_movimento " & vbCrLf)
            StrSQL.Append(" , Movimenti_dettagli.qualifica_cod, Movimenti_dettagli.tariffa_cod " & vbCrLf)
            StrSQL.Append(" , Contatti.Rag_Soc, contatti.nome, Contatti.Cognome " & vbCrLf)

            StrSQL.Append(" having COUNT(*) > 1 " & vbCrLf)
            StrSQL.Append(" order by Data_Movimento " & vbCrLf)

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

    '###############################################################################
    Public Function Verifica_Ripetizione_Contatto_CostiAccessori(ByVal Piva As String,
                                                                 ByVal Dal_Data_Verifica As Date,
                                                                 ByVal Al_Data_Verifica As Date,
                                                                 ByRef Str_Errore As String,
                                                                 ByRef objParametri As AgronicaCoreParametri
                                                                 ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.AnalisiCosti_R.Verifica_Ripetizione_Contatto_CostiAccessori()"

        Dim MessaggioErrore As String = ""
        Dim DT As DataTable
        Dim Flag_Errore As Boolean = False

        Try

            DT = Leggi_Ripetizione_Contatto_CostiAccessori(Piva, Dal_Data_Verifica, Al_Data_Verifica, objParametri)

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                'se ci sono record, ci sono registrazioni errate
                Flag_Errore = True

                Str_Errore = "Sono presenti registrazioni di costi accessori errate:" & vbCrLf

                Dim i As Integer
                Dim des_lib, data, ripetizioni, contatto As String
                For i = 0 To DT.Rows.Count - 1

                    des_lib = DT.Rows(i).Item("des_lib")
                    data = DT.Rows(i).Item("data_movimento")
                    ripetizioni = DT.Rows(i).Item("ripetizioni")
                    contatto = DT.Rows(i).Item("contatto")

                    Str_Errore &= data & " " & des_lib & ": " & ripetizioni & " ripetizioni di " & contatto & vbCrLf

                Next

            End If

        Catch ex As Exception
            Flag_Errore = True
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Flag_Errore

    End Function

    '###############################################################################
    'Copiata dai COM+: Agro_Contab_AD.Movimenti_Dettagli_R
    Public Function ValorizzazioneProdotto_New(ByVal Tipo_Valorizzazione As Integer, _
                                                ByVal Piva As String, _
                                                ByVal Elem_Cod As Int32, _
                                                ByVal Pro_Cod As Int32, _
                                                ByVal Mat_Cod As Int32, _
                                                ByVal Udm_Cod As Int32, _
                                                ByVal Cal_Cod As Int32, _
                                                ByVal Cod_Progetto As Int32, _
                                                ByVal Fase_Cod As Int32, _
                                                ByVal Lotto As String, _
                                                ByVal Dal_Data_Verifica As Date, _
                                                ByVal Al_Data_Verifica As Date, _
                                                ByRef objParametri As AgronicaCoreParametri _
                                                ) As Decimal

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.AnalisiCosti_R.ValorizzazioneProdotto_New()"
        Dim MessaggioErrore As String = ""
        Dim DT As DataTable
        Dim PrezzoPonderato As Decimal

        Try

            Select Case Tipo_Valorizzazione

                Case 1, 2

                    '##############################################################################################################
                    '##########################################  MEDIA PONDERATA  #################################################
                    '##############################################################################################################

                    DT = Leggi_PrezzixQta_QtaComplessiva(Piva, _
                                                            Elem_Cod, _
                                                            Pro_Cod, _
                                                            Mat_Cod, _
                                                            Udm_Cod, _
                                                            Cal_Cod, _
                                                            Cod_Progetto, _
                                                            Fase_Cod, _
                                                            Lotto, _
                                                            Dal_Data_Verifica, _
                                                            Al_Data_Verifica, _
                                                            objParametri)

                    If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

                        If IsNumeric(DT.Rows(0).Item("Delta")) AndAlso IsNumeric(DT.Rows(0).Item("Qta_Complessiva")) Then

                            Select Case CDbl(DT.Rows(0).Item("Qta_Complessiva"))

                                Case 0 'Non esistono valorizzazioni del bene nel periodo competenza

                                    PrezzoPonderato = 0

                                Case Else 'Esistono valorizzazioni del bene nel periodo di competenza

                                    PrezzoPonderato = DT.Rows(0).Item("Delta") / DT.Rows(0).Item("Qta_Complessiva")

                            End Select

                        Else
                            'Gestione Eccezione
                            PrezzoPonderato = 0
                        End If
                    Else
                        'Gestione Eccezione
                        PrezzoPonderato = 0
                    End If

                Case Else
                    PrezzoPonderato = 0

            End Select

            DT = Nothing


        Catch ex As Exception
            PrezzoPonderato = -1
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return PrezzoPonderato

    End Function


    '###############################################################################
    'A differenza della precedente ritorna un datatable
    Public Function ValorizzazioneProdotto(ByVal Tipo_Valorizzazione As Integer,
                                           ByVal Piva As String,
                                           ByVal Elem_Cod As Int32,
                                           ByVal Pro_Cod As Int32,
                                           ByVal Mat_Cod As Int32,
                                           ByVal Udm_Cod As Int32,
                                           ByVal Cal_Cod As Int32,
                                           ByVal Cod_Progetto As Int32,
                                           ByVal Fase_Cod As Int32,
                                           ByVal Lotto As String,
                                           ByVal Dal_Data_Verifica As Date,
                                           ByVal Al_Data_Verifica As Date,
                                           ByRef objParametri As AgronicaCoreParametri
                                           ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.AnalisiCosti_R.ValorizzazioneProdotto()"
        Dim DT As DataTable = Nothing
        Dim i As Integer

        Try

            Select Case Tipo_Valorizzazione

                Case 1, 2

                    '##############################################################################################################
                    '##########################################  MEDIA PONDERATA  #################################################
                    '##############################################################################################################

                    DT = Leggi_PrezzixQta_QtaComplessiva2(Piva,
                                                            Elem_Cod,
                                                            Pro_Cod,
                                                            Mat_Cod,
                                                            Udm_Cod,
                                                            Cal_Cod,
                                                            Cod_Progetto,
                                                            Fase_Cod,
                                                            Lotto,
                                                            Dal_Data_Verifica,
                                                            Al_Data_Verifica,
                                                            objParametri)

                    If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

                        DT.Columns.Add("PrezzoPonderato", GetType(Decimal))

                        For i = 0 To DT.Rows.Count - 1

                            If IsNumeric(DT.Rows(i).Item("Delta")) AndAlso IsNumeric(DT.Rows(i).Item("Qta_Complessiva")) Then

                                Select Case CDbl(DT.Rows(i).Item("Qta_Complessiva"))
                                    Case 0 'Non esistono valorizzazioni del bene nel periodo competenza
                                        DT.Rows(i).Item("PrezzoPonderato") = 0
                                    Case Else 'Esistono valorizzazioni del bene nel periodo di competenza
                                        DT.Rows(i).Item("PrezzoPonderato") = DT.Rows(i).Item("Delta") / DT.Rows(i).Item("Qta_Complessiva")
                                End Select

                            Else
                                DT.Rows(i).Item("PrezzoPonderato") = 0
                            End If

                        Next

                    End If

            End Select


        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
        End Try

        Return DT


    End Function


    '###############################################################################
    Public Function Leggi_PrezzixQta_QtaComplessiva(ByVal Piva As String, _
                                                    ByVal Elem_Cod As Int32, _
                                                    ByVal Pro_Cod As Int32, _
                                                    ByVal Mat_Cod As Int32, _
                                                    ByVal Udm_Cod As Int32, _
                                                    ByVal Cal_Cod As Int32, _
                                                    ByVal Cod_Progetto As Int32, _
                                                    ByVal Fase_Cod As Int32, _
                                                    ByVal Lotto As String, _
                                                    ByVal Dal_Data_Verifica As Date, _
                                                    ByVal Al_Data_Verifica As Date, _
                                                    ByRef objParametri As AgronicaCoreParametri _
                                                    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreContabDAL.AnalisiCosti_R.Leggi_PrezzixQta_QtaComplessiva()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        '----------------------------------------------------
        '--- Preparo la Query SQL ---------------------------
        '----------------------------------------------------

        Try

            StrSQL.Length = 0
            'Nota: escludo le bolle allegate a fatture

            StrSQL.Append(" SELECT ISNULL(SUM(ISNULL(Movimenti_Dettagli.Prezzo_Unitario_Netto,0) * ISNULL(Movimenti_Dettagli.Qta ,0)), 0) AS Delta,  " & vbCrLf)
            StrSQL.Append("        ISNULL(SUM(ISNULL(Movimenti_Dettagli.Qta ,0)),0) AS Qta_Complessiva  " & vbCrLf)

            StrSQL.Append(" FROM   Agenda " & vbCrLf)

            StrSQL.Append(" INNER JOIN Movimenti " & vbCrLf)
            StrSQL.Append(" ON      Agenda.Piva = Movimenti.Piva " & vbCrLf)
            StrSQL.Append(" AND     Agenda.Id_Agenda = Movimenti.Id_Agenda " & vbCrLf)

            StrSQL.Append(" INNER JOIN Movimenti_Dettagli " & vbCrLf)
            StrSQL.Append(" ON      Movimenti.Piva = Movimenti_Dettagli.Piva " & vbCrLf)
            StrSQL.Append(" AND     Movimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda " & vbCrLf)
            StrSQL.Append(" AND     Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov " & vbCrLf)

            StrSQL.Append(" INNER JOIN Mov_Destinazioni " & vbCrLf)
            StrSQL.Append(" ON   Movimenti_Dettagli.Piva = Mov_Destinazioni.Piva " & vbCrLf)
            StrSQL.Append(" AND   Movimenti_Dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda " & vbCrLf)
            StrSQL.Append(" AND   Movimenti_Dettagli.Id_Mov = Mov_Destinazioni.Id_Mov " & vbCrLf)
            StrSQL.Append(" AND   Movimenti_Dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det " & vbCrLf)

            StrSQL.Append(" WHERE  Movimenti_Dettagli.Id_Mov_Det NOT IN " & vbCrLf)
            StrSQL.Append("           (SELECT DISTINCT Id_Mov_Det_Rif " & vbCrLf)
            StrSQL.Append("            FROM Mov_Dettagli_Riferimenti " & vbCrLf)
            StrSQL.Append("            WHERE Lav_Cod IN (" & CStr(LAVCOD_FATTURA_RICEVUTA) & ", " & CStr(LAVCOD_FATTURA_EMESSA) & ")" & vbCrLf)
            If Piva <> "" Then
                StrSQL.Append("        AND Piva = '" & Agro_SQL_SaveText(Piva) & "'" & vbCrLf)
            End If
            StrSQL.Append(" )" & vbCrLf)

            '======================================================================================================================================================================
            'COSTO PONDERATO --> Bolle Ricevute, Fatture Ricevute, Acquisto, Carico, Ricevimento Fatture Liquidazione, Emissione AutoFatture Liquidazione, Note di Accredito Emesse,
            '                    Aumento Consistenze Zoo
            '----------------------------------------------------------------------------------------------------------------------------------------------------------------------
            '======================================================================================================================================================================
            'RICAVO PONDERATO --> Bolle Emesse, Fatture Emesse, Vendita, Scarico, Emissione Ricevute Fiscali, Emissione Fatture Liquidazione, Ricevimento AutoFatture Liquidazione,
            '                     Note di Accredito Ricevute, Autoconsumo
            '----------------------------------------------------------------------------------------------------------------------------------------------------------------------
            StrSQL.Append(" AND Agenda.Lav_Cod IN ( " & CStr(LAVCOD_BOLLA_RICEVUTA) & ", " & CStr(LAVCOD_FATTURA_RICEVUTA) & ", " & vbCrLf)
            StrSQL.Append(CStr(LAVCOD_ACQUISTO) & ", " & CStr(LAVCOD_CARICO) & ", " & CStr(LAVCOD_FATTURA_LIQ_CONF_RICEVUTA) & ", " & vbCrLf)
            StrSQL.Append(CStr(LAVCOD_AUTOFATTURA_LIQ_CONF_EMESSA) & ", " & CStr(LAVCOD_NOTA_ACCREDITO_EMESSA) & ", " & CStr(LAVCOD_INCREMENTO_CONSISTENZE_ZOO) & ", " & vbCrLf)
            StrSQL.Append(CStr(LAVCOD_DDT_CONTABILIZZATO_EMESSO) & ", " & CStr(LAVCOD_BOLLA_EMESSA) & ", " & CStr(LAVCOD_FATTURA_EMESSA) & ", " & CStr(LAVCOD_VENDITA) & ", " & CStr(LAVCOD_CORRISPETTIVO_VENDITA_SFUSO) & ", " & CStr(LAVCOD_SCARICO) & ", " & vbCrLf)
            StrSQL.Append(CStr(LAVCOD_RICEVUTA_EMESSA) & ", " & CStr(LAVCOD_CONFERIMENTO_DIVERSI) & ", " & CStr(LAVCOD_FATTURA_LIQ_CONF_EMESSA) & ", " & vbCrLf)
            StrSQL.Append(CStr(LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA) & ", " & CStr(LAVCOD_NOTA_ACCREDITO_RICEVUTA) & ", " & CStr(LAVCOD_AUTOCONSUMO) & ", " & CStr(LAVCOD_AUTOCONSUMO_VINO_SFUSO) & " ) " & vbCrLf)

            'Lettura mirata del prodotto
            If Piva <> "" Then
                StrSQL.Append(" AND Mov_Destinazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "'   " & vbCrLf)
            End If
            If Elem_Cod <> 0 Then
                StrSQL.Append(" AND Movimenti_Dettagli.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   " & vbCrLf)
            End If
            If Pro_Cod <> 0 Then
                StrSQL.Append(" AND Movimenti_Dettagli.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   " & vbCrLf)
            End If
            If Mat_Cod <> 0 Then
                StrSQL.Append(" AND Movimenti_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   " & vbCrLf)
            End If
            If Udm_Cod <> 0 Then
                StrSQL.Append(" AND Movimenti_Dettagli.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   " & vbCrLf)
            End If
            If Cal_Cod <> 0 Then
                StrSQL.Append(" AND Movimenti_Dettagli.Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "   " & vbCrLf)
            End If
            If Fase_Cod <> 0 Then
                StrSQL.Append(" AND Movimenti_Dettagli.Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   " & vbCrLf)
            End If
            If Lotto <> "" Then
                StrSQL.Append(" AND Upper(Movimenti_Dettagli.Lotto) = '" & UCase(Agro_SQL_SaveText(Lotto)) & "'   " & vbCrLf)
            End If

            StrSQL.Append(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Dal_Data_Verifica) & vbCrLf)
            StrSQL.Append(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Al_Data_Verifica) & vbCrLf)

            'La condizione necessaria è che il dettaglio abbia una imputazione economica e una quantità valida
            StrSQL.Append(" AND Movimenti_Dettagli.Prezzo_Unitario_Netto > 0 AND Movimenti_Dettagli.Qta > 0 " & vbCrLf)

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Agenda.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Agenda.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------


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

    'A differenza della precedente ritorna anche le colonne elem_cod,mat_cod,pro_cod etc...
    '###############################################################################
    Public Function Leggi_PrezzixQta_QtaComplessiva2(ByVal Piva As String, _
                                                    ByVal Elem_Cod As Int32, _
                                                    ByVal Pro_Cod As Int32, _
                                                    ByVal Mat_Cod As Int32, _
                                                    ByVal Udm_Cod As Int32, _
                                                    ByVal Cal_Cod As Int32, _
                                                    ByVal Cod_Progetto As Int32, _
                                                    ByVal Fase_Cod As Int32, _
                                                    ByVal Lotto As String, _
                                                    ByVal Dal_Data_Verifica As Date, _
                                                    ByVal Al_Data_Verifica As Date, _
                                                    ByRef objParametri As AgronicaCoreParametri _
                                                    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreContabDAL.AnalisiCosti_R.Leggi_PrezzixQta_QtaComplessiva()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        '----------------------------------------------------
        '--- Preparo la Query SQL ---------------------------
        '----------------------------------------------------

        Try

            StrSQL.Length = 0
            'Nota: escludo le bolle allegate a fatture

            StrSQL.Append(" SELECT ISNULL(SUM(ISNULL(Movimenti_Dettagli.Prezzo_Unitario_Netto,0) * ISNULL(Movimenti_Dettagli.Qta ,0)), 0) AS Delta,  " & vbCrLf)
            StrSQL.Append("        ISNULL(SUM(ISNULL(Movimenti_Dettagli.Qta ,0)),0) AS Qta_Complessiva  " & vbCrLf)

            StrSQL.Append(" FROM   Agenda " & vbCrLf)

            StrSQL.Append(" INNER JOIN Movimenti " & vbCrLf)
            StrSQL.Append(" ON      Agenda.Piva = Movimenti.Piva " & vbCrLf)
            StrSQL.Append(" AND     Agenda.Id_Agenda = Movimenti.Id_Agenda " & vbCrLf)

            StrSQL.Append(" INNER JOIN Movimenti_Dettagli " & vbCrLf)
            StrSQL.Append(" ON      Movimenti.Piva = Movimenti_Dettagli.Piva " & vbCrLf)
            StrSQL.Append(" AND     Movimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda " & vbCrLf)
            StrSQL.Append(" AND     Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov " & vbCrLf)

            StrSQL.Append(" INNER JOIN Mov_Destinazioni " & vbCrLf)
            StrSQL.Append(" ON   Movimenti_Dettagli.Piva = Mov_Destinazioni.Piva " & vbCrLf)
            StrSQL.Append(" AND   Movimenti_Dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda " & vbCrLf)
            StrSQL.Append(" AND   Movimenti_Dettagli.Id_Mov = Mov_Destinazioni.Id_Mov " & vbCrLf)
            StrSQL.Append(" AND   Movimenti_Dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det " & vbCrLf)

            StrSQL.Append(" WHERE  Movimenti_Dettagli.Id_Mov_Det NOT IN " & vbCrLf)
            StrSQL.Append("           (SELECT DISTINCT Id_Mov_Det_Rif " & vbCrLf)
            StrSQL.Append("            FROM Mov_Dettagli_Riferimenti " & vbCrLf)
            StrSQL.Append("            WHERE Lav_Cod IN (" & CStr(LAVCOD_FATTURA_RICEVUTA) & ", " & CStr(LAVCOD_FATTURA_EMESSA) & ")" & vbCrLf)
            If Piva <> "" Then
                StrSQL.Append("        AND Piva = '" & Agro_SQL_SaveText(Piva) & "'" & vbCrLf)
            End If
            StrSQL.Append(" )" & vbCrLf)

            '======================================================================================================================================================================
            'COSTO PONDERATO --> Bolle Ricevute, Fatture Ricevute, Acquisto, Carico, Ricevimento Fatture Liquidazione, Emissione AutoFatture Liquidazione, Note di Accredito Emesse,
            '                    Aumento Consistenze Zoo
            '----------------------------------------------------------------------------------------------------------------------------------------------------------------------
            '======================================================================================================================================================================
            'RICAVO PONDERATO --> Bolle Emesse, Fatture Emesse, Vendita, Scarico, Emissione Ricevute Fiscali, Emissione Fatture Liquidazione, Ricevimento AutoFatture Liquidazione,
            '                     Note di Accredito Ricevute, Autoconsumo
            '----------------------------------------------------------------------------------------------------------------------------------------------------------------------
            StrSQL.Append(" AND Agenda.Lav_Cod IN ( " & CStr(LAVCOD_BOLLA_RICEVUTA) & ", " & CStr(LAVCOD_FATTURA_RICEVUTA) & ", " & vbCrLf)
            StrSQL.Append(CStr(LAVCOD_ACQUISTO) & ", " & CStr(LAVCOD_CARICO) & ", " & CStr(LAVCOD_FATTURA_LIQ_CONF_RICEVUTA) & ", " & vbCrLf)
            StrSQL.Append(CStr(LAVCOD_AUTOFATTURA_LIQ_CONF_EMESSA) & ", " & CStr(LAVCOD_NOTA_ACCREDITO_EMESSA) & ", " & CStr(LAVCOD_INCREMENTO_CONSISTENZE_ZOO) & ", " & vbCrLf)
            StrSQL.Append(CStr(LAVCOD_DDT_CONTABILIZZATO_EMESSO) & ", " & CStr(LAVCOD_BOLLA_EMESSA) & ", " & CStr(LAVCOD_FATTURA_EMESSA) & ", " & CStr(LAVCOD_VENDITA) & ", " & CStr(LAVCOD_CORRISPETTIVO_VENDITA_SFUSO) & ", " & CStr(LAVCOD_SCARICO) & ", " & vbCrLf)
            StrSQL.Append(CStr(LAVCOD_RICEVUTA_EMESSA) & ", " & CStr(LAVCOD_CONFERIMENTO_DIVERSI) & ", " & CStr(LAVCOD_FATTURA_LIQ_CONF_EMESSA) & ", " & vbCrLf)
            StrSQL.Append(CStr(LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA) & ", " & CStr(LAVCOD_NOTA_ACCREDITO_RICEVUTA) & ", " & CStr(LAVCOD_AUTOCONSUMO) & ", " & CStr(LAVCOD_AUTOCONSUMO_VINO_SFUSO) & " ) " & vbCrLf)

            'Lettura mirata del prodotto
            If Piva <> "" Then
                StrSQL.Append(" AND Mov_Destinazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "'   " & vbCrLf)
            End If
            If Elem_Cod <> 0 Then
                StrSQL.Append(" AND Movimenti_Dettagli.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   " & vbCrLf)
            End If
            If Pro_Cod <> 0 Then
                StrSQL.Append(" AND Movimenti_Dettagli.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   " & vbCrLf)
            End If
            If Mat_Cod <> 0 Then
                StrSQL.Append(" AND Movimenti_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   " & vbCrLf)
            End If
            If Udm_Cod <> 0 Then
                StrSQL.Append(" AND Movimenti_Dettagli.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   " & vbCrLf)
            End If
            If Cal_Cod <> 0 Then
                StrSQL.Append(" AND Movimenti_Dettagli.Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "   " & vbCrLf)
            End If
            If Fase_Cod <> 0 Then
                StrSQL.Append(" AND Movimenti_Dettagli.Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   " & vbCrLf)
            End If
            If Lotto <> "" Then
                StrSQL.Append(" AND Upper(Movimenti_Dettagli.Lotto) = '" & UCase(Agro_SQL_SaveText(Lotto)) & "'   " & vbCrLf)
            End If

            StrSQL.Append(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Dal_Data_Verifica) & vbCrLf)
            StrSQL.Append(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Al_Data_Verifica) & vbCrLf)

            'La condizione necessaria è che il dettaglio abbia una imputazione economica e una quantità valida
            StrSQL.Append(" AND Movimenti_Dettagli.Prezzo_Unitario_Netto > 0 AND Movimenti_Dettagli.Qta > 0 " & vbCrLf)

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Agenda.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Agenda.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------


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

    Public Function Leggi_Dati_Agenda(ByVal strId_Agenda As String,
                                      ByRef objParametri As AgronicaCoreParametri
                                      ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.AnalisiCosti_R.Leggi_Dati_Agenda()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        '----------------------------------------------------
        '--- Preparo la Query SQL ---------------------------
        '----------------------------------------------------

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT  Agenda.PIVA, Agenda.Sa_Cod, Agenda.Id_Agenda, Agenda.Lav_Cod, Agenda.des_lib, ")
            StrSQL.Append(" Movimenti.Cau_Mov, Movimenti.Mov_Desc, Movimenti.Data_Movimento, ")
            StrSQL.Append(" Movimenti_dettagli.Elem_Cod, Movimenti_dettagli.Pro_Cod, Movimenti_dettagli.Mat_Cod, Movimenti_dettagli.Udm_Cod, Movimenti_dettagli.Qta, ")
            StrSQL.Append(" Mov_Destinazioni.Piva AS Piva_Dest, Mov_Destinazioni.Sa_Cod AS sa_cod_Dest, Mov_Destinazioni.Appezza as appezza_dest,  Mov_Destinazioni.Id_Destinazione , ")
            StrSQL.Append(" Mov_Destinazioni.Qta AS qta_dest, Mov_Destinazioni.Qta2, UnitaMisura.UDM_SIM ")

            StrSQL.Append(" FROM    Agenda INNER JOIN ")
            StrSQL.Append("         Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda INNER JOIN ")
            StrSQL.Append("         Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND  ")
            StrSQL.Append("         Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov INNER JOIN ")
            StrSQL.Append("         Mov_Destinazioni ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND  ")
            StrSQL.Append("         Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det INNER JOIN UnitaMisura ON Movimenti_dettagli.Udm_Cod = UnitaMisura.UDM_COD ")

            StrSQL.Append("  WHERE (Cau_Mov <> '7350' AND Cau_Mov <> '7300' AND Cau_Mov <> '8100' AND Cau_Mov <> '6800' AND Cau_Mov <> '6850') ")
            StrSQL.Append("  AND (Agenda.Id_Agenda IN ( " & Agro_SQL_Save_Clausola_IN(strId_Agenda) & ") )")

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Agenda.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Agenda.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            StrSQL.Append("  UNION ")

            StrSQL.Append("  SELECT     Agenda.PIVA, Agenda.Sa_Cod, Agenda.Id_Agenda, Agenda.Lav_Cod, Agenda.des_lib, ")
            StrSQL.Append("  Movimenti.Cau_Mov, Movimenti.Mov_Desc, Movimenti.Data_Movimento, ")
            StrSQL.Append("  Movimenti_dettagli.Elem_Cod, Movimenti_dettagli.Pro_Cod, Movimenti_dettagli.Mat_Cod, Movimenti_dettagli.Udm_Cod, Movimenti_dettagli.Qta, ")
            StrSQL.Append("  '' AS Piva_Dest, 0 AS sa_cod_Dest, 0 as appezza_dest,  0 as Id_Destinazione , ")
            StrSQL.Append("  0 AS qta_dest, 0 as Qta2, ")
            StrSQL.Append("  CASE Udm_Cod ")
            StrSQL.Append("  WHEN 1 THEN 'Ha' ")
            StrSQL.Append("  WHEN 2 THEN 'ora' ")
            StrSQL.Append("  ELSE '' END AS Udm_Sim")

            StrSQL.Append(" FROM    Agenda INNER JOIN ")
            StrSQL.Append("         Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda INNER JOIN ")
            StrSQL.Append("         Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND  ")
            StrSQL.Append("         Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov ")
            StrSQL.Append(" WHERE     (Cau_Mov ='8100' or Cau_Mov='6800' or Cau_Mov='6850') ")
            StrSQL.Append("  AND (Agenda.Id_Agenda IN ( " & Agro_SQL_Save_Clausola_IN(strId_Agenda) & ") )")


            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Agenda.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Agenda.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            'Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function


End Class

