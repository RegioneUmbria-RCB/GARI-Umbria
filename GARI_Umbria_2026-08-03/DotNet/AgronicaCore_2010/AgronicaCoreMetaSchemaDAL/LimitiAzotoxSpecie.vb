Imports AgronicaCoreDataProvider


Public Class LimitiAzotoxSpecie_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function RecuperaRangeAzotoFromVeg_Cod(ByVal Veg_Cod As Integer,
                                                  ByRef NMinimo As Integer,
                                                  ByRef NMassimo As Integer,
                                                  ByRef objParametri As AgronicaCoreParametri
                                                  ) As Integer

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.LimitiAzotoxSpecie_R.RecuperaRangeAzotoFromVeg_Cod()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable
        Dim risp As Integer

        Try

            StrSQL.Length = 0
            StrSQL.Append(" Select MAX(N) AS N From LimitiAzotoxSpecie WHERE Veg_Cod = " & Veg_Cod.ToString)

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
            If dt.Rows.Count > 0 Then

                If CInt(dt.Rows(0).Item("N")) <> 0 Then
                    NMassimo = CInt(dt.Rows(0).Item("N"))
                End If
            Else

                risp = -1

            End If

            If risp <> -1 Then
                StrSQL.Length = 0
                StrSQL.Append(" Select MIN(N) AS N From LimitiAzotoxSpecie WHERE Veg_Cod = " & Veg_Cod.ToString)
                '--------------------------------------------------------------------------
                dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
                '--------------------------------------------------------------------------


                If dt.Rows.Count <> 0 Then


                    If CInt(dt.Rows(0).Item("N")) <> 0 Then

                        NMinimo = CInt(dt.Rows(0).Item("N"))
                    End If

                Else

                    risp = -1

                End If

            End If


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            risp = -2
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return risp

    End Function


    '##############################################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' default ByVal valoreMax As Boolean = True
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function RecuperaAzotoFromVeg_Cod(ByVal Veg_Cod As Integer,
                                             ByVal Grfi_Cod As Integer,
                                             ByVal Stato_Cod As Integer,
                                             ByVal ValiditaInizio As Date,
                                             ByVal ValiditaInizioDistinta As DateTime,
                                             ByVal valoreMax As Boolean,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As Integer

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.LimitiAzotoxSpecie_R.RecuperaAzotoFromVeg_Cod()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable
        Dim risp As Integer
        Dim Id_Fase As Integer


        Try

            If ValiditaInizioDistinta.Year = ValiditaInizio.Year AndAlso Stato_Cod = 101 Then

                ' Allevamento arboreo 1 anno
                Id_Fase = 2

            ElseIf ValiditaInizioDistinta.Year > ValiditaInizio.Year AndAlso Stato_Cod = 101 Then

                ' Allevamento arboreo 2 anno o successivi 
                Id_Fase = 3

            ElseIf Stato_Cod = 102 Then

                'Produzione 
                Id_Fase = 6

            End If

            If valoreMax Then
                StrSQL.Append(" Select MAX(N) AS NMAX From LimitiAzotoxSpecie WHERE Veg_Cod = " & Veg_Cod.ToString & " AND Grfi_Cod = " & Grfi_Cod.ToString & " AND Id_Fase = " & Id_Fase.ToString)
            Else
                StrSQL.Append(" Select MIN(N) AS NMAX From LimitiAzotoxSpecie WHERE Veg_Cod = " & Veg_Cod.ToString & " AND Grfi_Cod = " & Grfi_Cod.ToString & " AND Id_Fase = " & Id_Fase.ToString)
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If dt.Rows.Count <> 0 Then

                If dt.Rows(0).Item("NMAX") IsNot DBNull.Value Then

                    If CInt(dt.Rows(0).Item("NMAX")) <> 0 Then

                        risp = CInt(dt.Rows(0).Item("NMAX"))

                    End If

                Else

                    risp = -1

                End If

            Else

                risp = -1

            End If

        Catch ex As Exception

            risp = -2

        Finally

        End Try

        Return risp

    End Function


    '###############################################################################################
    ' filtro anche sul regolamento 
    Public Function RecuperaAzotoFromVegCod_Regolamento(ByVal Veg_Cod As Integer,
                                                        ByVal Grfi_Cod As Integer,
                                                        ByVal Stato_Cod As Integer,
                                                        ByVal ValiditaInizio As Date,
                                                        ByVal ValiditaInizioDistinta As DateTime,
                                                        ByVal valoreMax As Boolean,
                                                        ByVal Regolamento_Cod As Integer,
                                                        ByRef objParametri As AgronicaCoreParametri
                                                        ) As Integer

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.LimitiAzotoxSpecie_R.RecuperaAzotoFromVeg_Cod()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable
        Dim Risp As Integer

        Try
            ' '' recupero il gruppo vegetale
            ''StrSQL.Append(" Select Gru_Cod FROM SpecieVegetali WHERE Veg_Cod = " & Veg_Cod)

            ' ''--------------------------------------------------------------------------
            ''dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            ' ''--------------------------------------------------------------------------

            ''If dt.Rows.Count <> 0 Then
            ''    Gru_Cod = dt.Rows(0).Item(0)
            ''End If

            StrSQL.Length = 0

            If Regolamento_Cod = 1 Then

                'PROMEMORIA
                'valoreMin e valoreMax servono per il discorso del pomodoro che assume valori diversi se trapiantato prima o dopo il 5 maggio

                If valoreMax Then
                    ' determinazione n° 7609 del 04/08/2009
                    ' per le colture non presenti nella tabella 7a dell'allegato 2 si fissa un limite pari a 240kg/ha
                    StrSQL.Append(" SELECT ISNULL(MAX(N),240) FROM LimitiAzotoxSpecie ")
                    StrSQL.Append(" INNER JOIN FasiCicloColturalexGruppoFinalita ON LimitiAzotoxSpecie.id_fase = FasiCicloColturalexGruppoFinalita.id_fase ")
                    StrSQL.Append(" AND LimitiAzotoxSpecie.regolamento_cod = FasiCicloColturalexGruppoFinalita.regolamento_cod ")
                    StrSQL.Append(" WHERE LimitiAzotoxSpecie.Veg_Cod = " & Veg_Cod.ToString)
                    StrSQL.Append(" AND LimitiAzotoxSpecie.Grfi_Cod = " & Grfi_Cod.ToString)
                    StrSQL.Append(" AND FasiCicloColturalexGruppoFinalita.Grfi_Cod = " & Stato_Cod.ToString)
                    StrSQL.Append(" AND LimitiAzotoxSpecie.Regolamento_Cod = " & Regolamento_Cod.ToString)
                Else
                    ' determinazione n° 7609 del 04/08/2009
                    ' per le colture non presenti nella tabella 7a dell'allegato 2 si fissa un limite pari a 240kg/ha
                    StrSQL.Append(" SELECT ISNULL(MIN(N),240) FROM LimitiAzotoxSpecie ")
                    StrSQL.Append(" INNER JOIN FasiCicloColturalexGruppoFinalita ON LimitiAzotoxSpecie.id_fase = FasiCicloColturalexGruppoFinalita.id_fase ")
                    StrSQL.Append(" AND LimitiAzotoxSpecie.regolamento_cod = FasiCicloColturalexGruppoFinalita.regolamento_cod ")
                    StrSQL.Append(" WHERE LimitiAzotoxSpecie.Veg_Cod = " & Veg_Cod.ToString)
                    StrSQL.Append(" AND LimitiAzotoxSpecie.Grfi_Cod = " & Grfi_Cod.ToString)
                    StrSQL.Append(" AND FasiCicloColturalexGruppoFinalita.Grfi_Cod = " & Stato_Cod.ToString)
                    StrSQL.Append(" AND LimitiAzotoxSpecie.Regolamento_Cod = " & Regolamento_Cod.ToString)
                End If

            Else

                StrSQL.Append(" SELECT ISNULL(N,-1) FROM LimitiAzotoxSpecie ")
                StrSQL.Append(" INNER JOIN FasiCicloColturalexGruppoFinalita ON LimitiAzotoxSpecie.id_fase = FasiCicloColturalexGruppoFinalita.id_fase ")
                StrSQL.Append(" AND LimitiAzotoxSpecie.regolamento_cod = FasiCicloColturalexGruppoFinalita.regolamento_cod ")
                StrSQL.Append(" WHERE LimitiAzotoxSpecie.Veg_Cod = " & Veg_Cod.ToString)
                StrSQL.Append(" AND LimitiAzotoxSpecie.Grfi_Cod = " & Grfi_Cod.ToString)
                StrSQL.Append(" AND FasiCicloColturalexGruppoFinalita.Grfi_Cod = " & Stato_Cod.ToString)
                StrSQL.Append(" AND LimitiAzotoxSpecie.Regolamento_Cod = " & Regolamento_Cod.ToString)

            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If dt.Rows.Count <> 0 Then

                If dt.Rows(0).Item(0) IsNot DBNull.Value Then
                    Risp = CInt(dt.Rows(0).Item(0))
                Else
                    Risp = -1
                End If
            Else
                Risp = -1
            End If

        Catch ex As Exception

            Risp = -1

        Finally

        End Try

        Return Risp

    End Function


    Public Function RecuperaAzotoResaFromVegCod_Regolamento(ByVal Veg_Cod As Integer,
                                                            ByVal Grfi_Cod As Integer,
                                                            ByVal Stato_Cod As Integer,
                                                            ByVal valoreMax As Boolean,
                                                            ByVal Regolamento_Cod As Integer,
                                                            ByRef Resa As Decimal, ByRef FattoreCorrettivo_N As Decimal,
                                                            ByRef objParametri As AgronicaCoreParametri
                                                            ) As Integer

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.LimitiAzotoxSpecie_R.RecuperaAzotoResaFromVegCod_Regolamento()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable
        Dim risp As Integer = -1
        Resa = -1

        Try

            StrSQL.Length = 0

            If Regolamento_Cod = 1 Then

                'PROMEMORIA
                'valoreMin e valoreMax servono per il discorso del pomodoro che assume valori diversi se trapiantato prima o dopo il 5 maggio

                If valoreMax Then
                    ' determinazione n° 7609 del 04/08/2009
                    ' per le colture non presenti nella tabella 7a dell'allegato 2 si fissa un limite pari a 240kg/ha
                    StrSQL.Append(" SELECT ISNULL(MAX(N),240) AS N, ISNULL(Resa,-1) as Resa, ISNULL(FattoreCorrettivo_N,0) as FattoreCorrettivo_N FROM LimitiAzotoxSpecie ")
                    StrSQL.Append(" INNER JOIN FasiCicloColturalexGruppoFinalita ON LimitiAzotoxSpecie.id_fase = FasiCicloColturalexGruppoFinalita.id_fase ")
                    StrSQL.Append(" AND LimitiAzotoxSpecie.regolamento_cod = FasiCicloColturalexGruppoFinalita.regolamento_cod ")
                    StrSQL.Append(" WHERE LimitiAzotoxSpecie.Veg_Cod = " & Veg_Cod.ToString)
                    StrSQL.Append(" AND LimitiAzotoxSpecie.Grfi_Cod = " & Grfi_Cod.ToString)
                    StrSQL.Append(" AND FasiCicloColturalexGruppoFinalita.Grfi_Cod = " & Stato_Cod.ToString)
                    StrSQL.Append(" AND LimitiAzotoxSpecie.Regolamento_Cod = " & Regolamento_Cod.ToString)
                Else
                    ' determinazione n° 7609 del 04/08/2009
                    ' per le colture non presenti nella tabella 7a dell'allegato 2 si fissa un limite pari a 240kg/ha
                    StrSQL.Append(" SELECT ISNULL(MIN(N),240) AS N, ISNULL(Resa,-1) as Resa, ISNULL(FattoreCorrettivo_N,0) as FattoreCorrettivo_N FROM LimitiAzotoxSpecie ")
                    StrSQL.Append(" INNER JOIN FasiCicloColturalexGruppoFinalita ON LimitiAzotoxSpecie.id_fase = FasiCicloColturalexGruppoFinalita.id_fase ")
                    StrSQL.Append(" AND LimitiAzotoxSpecie.regolamento_cod = FasiCicloColturalexGruppoFinalita.regolamento_cod ")
                    StrSQL.Append(" WHERE LimitiAzotoxSpecie.Veg_Cod = " & Veg_Cod.ToString)
                    StrSQL.Append(" AND LimitiAzotoxSpecie.Grfi_Cod = " & Grfi_Cod.ToString)
                    StrSQL.Append(" AND FasiCicloColturalexGruppoFinalita.Grfi_Cod = " & Stato_Cod.ToString)
                    StrSQL.Append(" AND LimitiAzotoxSpecie.Regolamento_Cod = " & Regolamento_Cod.ToString)
                End If

            Else

                StrSQL.Append(" SELECT ISNULL(N,-1) AS N, ISNULL(Resa,-1) as Resa, ISNULL(FattoreCorrettivo_N,0) as FattoreCorrettivo_N FROM LimitiAzotoxSpecie ")
                StrSQL.Append(" INNER JOIN FasiCicloColturalexGruppoFinalita ON LimitiAzotoxSpecie.id_fase = FasiCicloColturalexGruppoFinalita.id_fase ")
                StrSQL.Append(" AND LimitiAzotoxSpecie.regolamento_cod = FasiCicloColturalexGruppoFinalita.regolamento_cod ")
                StrSQL.Append(" WHERE LimitiAzotoxSpecie.Veg_Cod = " & Veg_Cod.ToString)
                If Grfi_Cod <> 0 Then
                    StrSQL.Append(" AND LimitiAzotoxSpecie.Grfi_Cod = " & Grfi_Cod.ToString)
                End If
                StrSQL.Append(" AND FasiCicloColturalexGruppoFinalita.Grfi_Cod = " & Stato_Cod.ToString)
                StrSQL.Append(" AND LimitiAzotoxSpecie.Regolamento_Cod = " & Regolamento_Cod.ToString)

            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If dt.Rows.Count > 0 Then
                If Not IsDBNull(dt.Rows(0).Item("N")) Then
                    risp = dt.Rows(0).Item("N")
                End If
                If Not IsDBNull(dt.Rows(0).Item("Resa")) Then
                    Resa = dt.Rows(0).Item("Resa")
                End If
                If Not IsDBNull(dt.Rows(0).Item("FattoreCorrettivo_N")) Then
                    FattoreCorrettivo_N = dt.Rows(0).Item("FattoreCorrettivo_N")
                End If
            End If

        Catch ex As Exception

            Return risp

        Finally

        End Try

        Return risp

    End Function

    Public Function RecuperaAzotoResaFromVegCod_Regolamento_Elenco(
                             ByVal Veg_Cod_Elenco As String,
                             ByVal valoreMax As Boolean,
                             ByVal Regolamento_Cod As Integer,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.LimitiAzotoxSpecie_R.RecuperaAzotoResaFromVegCod_Regolamento_Elenco()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            If Regolamento_Cod = 1 Then

                'PROMEMORIA
                'valoreMin e valoreMax servono per il discorso del pomodoro che assume valori diversi se trapiantato prima o dopo il 5 maggio

                If valoreMax Then
                    ' determinazione n° 7609 del 04/08/2009
                    ' per le colture non presenti nella tabella 7a dell'allegato 2 si fissa un limite pari a 240kg/ha
                    StrSQL.Append(" SELECT ISNULL(MAX(N),240) AS N, ISNULL(Resa,-1) as Resa, ISNULL(FattoreCorrettivo_N,0) as FattoreCorrettivo_N,  ")
                    StrSQL.Append(" LimitiAzotoxSpecie.Regolamento_Cod, LimitiAzotoxSpecie.Veg_Cod, LimitiAzotoxSpecie.Grfi_Cod, FasiCicloColturalexGruppoFinalita.Grfi_Cod as stato_cod ")
                    StrSQL.Append(" FROM LimitiAzotoxSpecie ")
                    StrSQL.Append(" INNER JOIN FasiCicloColturalexGruppoFinalita ON LimitiAzotoxSpecie.id_fase = FasiCicloColturalexGruppoFinalita.id_fase ")
                    StrSQL.Append(" AND LimitiAzotoxSpecie.regolamento_cod = FasiCicloColturalexGruppoFinalita.regolamento_cod ")

                Else
                    ' determinazione n° 7609 del 04/08/2009
                    ' per le colture non presenti nella tabella 7a dell'allegato 2 si fissa un limite pari a 240kg/ha
                    StrSQL.Append(" SELECT ISNULL(MIN(N),240) AS N, ISNULL(Resa,-1) as Resa, ISNULL(FattoreCorrettivo_N,0) as FattoreCorrettivo_N,  ")
                    StrSQL.Append(" LimitiAzotoxSpecie.Regolamento_Cod, LimitiAzotoxSpecie.Veg_Cod, LimitiAzotoxSpecie.Grfi_Cod, FasiCicloColturalexGruppoFinalita.Grfi_Cod as stato_cod ")
                    StrSQL.Append(" FROM LimitiAzotoxSpecie ")
                    StrSQL.Append(" INNER JOIN FasiCicloColturalexGruppoFinalita ON LimitiAzotoxSpecie.id_fase = FasiCicloColturalexGruppoFinalita.id_fase ")
                    StrSQL.Append(" AND LimitiAzotoxSpecie.regolamento_cod = FasiCicloColturalexGruppoFinalita.regolamento_cod ")

                End If

            Else

                StrSQL.Append(" SELECT ISNULL(N,-1) AS N, ISNULL(Resa,-1) as Resa, ISNULL(FattoreCorrettivo_N,0) as FattoreCorrettivo_N,  ")
                StrSQL.Append(" LimitiAzotoxSpecie.Regolamento_Cod, LimitiAzotoxSpecie.Veg_Cod, LimitiAzotoxSpecie.Grfi_Cod, FasiCicloColturalexGruppoFinalita.Grfi_Cod as stato_cod ")
                StrSQL.Append(" FROM LimitiAzotoxSpecie ")
                StrSQL.Append(" INNER JOIN FasiCicloColturalexGruppoFinalita ON LimitiAzotoxSpecie.id_fase = FasiCicloColturalexGruppoFinalita.id_fase ")
                StrSQL.Append(" AND LimitiAzotoxSpecie.regolamento_cod = FasiCicloColturalexGruppoFinalita.regolamento_cod ")

            End If

            StrSQL.Append(" WHERE LimitiAzotoxSpecie.Regolamento_Cod = " & Regolamento_Cod.ToString)

            If Veg_Cod_Elenco <> "" Then
                StrSQL.Append(" AND LimitiAzotoxSpecie.Veg_Cod IN (" & Agro_SQL_Save_Clausola_IN(Veg_Cod_Elenco) & ")")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Return dt
        Finally

        End Try

        Return dt

    End Function

End Class
