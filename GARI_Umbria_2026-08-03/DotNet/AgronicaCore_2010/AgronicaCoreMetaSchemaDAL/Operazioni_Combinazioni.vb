Imports System.Data.Common
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.DataProviderExtensions

Public Class Operazioni_Combinazioni_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal lista_LavCod As Integer(),
                              ByVal FiltroImpostazioniUtente As String,
                              ByVal xOrderBy As String,
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As List(Of AgronicaCoreModelsSTD.attivita.Lavorazione)




        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Operazioni_Combinazioni_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim operazioniList As New List(Of AgronicaCoreModelsSTD.attivita.Lavorazione)

        'nrColonneLavCod serve per sapere quante iterazioni con le UNION mi servono
        Dim nrColonneLavCod As Integer = 5
        Dim i As Integer

        Try


            '---------------------------------------------
            StrSQL.Length = 0

            For i = 1 To nrColonneLavCod
                StrSQL.AppendLine(" (")
                StrSQL.AppendLine("     SELECT DISTINCT ID, Lav_Cod" & i & " AS Lav_Cod, Lav_Des ")
                StrSQL.AppendLine("     FROM  Operazioni_Combinazioni ")

                StrSQL.AppendLine("     LEFT JOIN Operazioni ON Operazioni.LAV_COD = Operazioni_Combinazioni.Lav_Cod" & i & " ")

                If Not IsNothing(lista_LavCod) And lista_LavCod.Count > 0 Then
                    StrSQL.AppendLine("     WHERE ")
                End If

                'Dummy mi serve per sapere se sono dalla seconda iterazione in poi, per poter aggiungere AND
                Dim dummy As Integer = 0
                'Ciclo per filtrare i diversi lav_cod passati nella lista su ogni colonna
                For Each Lav_Cod In lista_LavCod
                    If dummy >= 1 Then
                        StrSQL.AppendLine("     AND  ")
                    End If

                    StrSQL.AppendLine("     ( ")
                    StrSQL.AppendLine("         Lav_Cod1 = " & Agro_SQL_SaveNum(Lav_Cod) & " ")
                    StrSQL.AppendLine("         OR Lav_Cod2 = " & Agro_SQL_SaveNum(Lav_Cod) & " ")
                    StrSQL.AppendLine("         OR Lav_Cod3 = " & Agro_SQL_SaveNum(Lav_Cod) & " ")
                    StrSQL.AppendLine("         OR Lav_Cod4 = " & Agro_SQL_SaveNum(Lav_Cod) & " ")
                    StrSQL.AppendLine("         OR Lav_Cod5 = " & Agro_SQL_SaveNum(Lav_Cod) & " ")
                    StrSQL.AppendLine("     )")

                    dummy += 1
                Next

                'Aggiungo il filtro utente, per mostrare solo le operazioni che l'utente vuole visualizzare
                If FiltroImpostazioniUtente <> "" Then
                    StrSQL.AppendLine("     AND ( ")
                    StrSQL.AppendLine("         Lav_Cod1 IN (" & Agro_SQL_Save_Clausola_IN(FiltroImpostazioniUtente) & ") ")
                    StrSQL.AppendLine("         AND Lav_Cod2 IN (" & Agro_SQL_Save_Clausola_IN(FiltroImpostazioniUtente) & ") ")
                    StrSQL.AppendLine("         AND Lav_Cod3 IN (" & Agro_SQL_Save_Clausola_IN(FiltroImpostazioniUtente) & ") ")
                    StrSQL.AppendLine("         AND Lav_Cod4 IN (" & Agro_SQL_Save_Clausola_IN(FiltroImpostazioniUtente) & ") ")
                    StrSQL.AppendLine("         AND Lav_Cod5 IN (" & Agro_SQL_Save_Clausola_IN(FiltroImpostazioniUtente) & ") ")
                    StrSQL.AppendLine("         )")
                End If
                StrSQL.AppendLine(" )")

                'Non aggiungo la UNION se siamo all'ultima colonna
                If i < nrColonneLavCod Then
                    StrSQL.AppendLine(" UNION")
                End If

            Next

            StrSQL.AppendLine(" ORDER BY LAV_DES")


            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)

            'Aggiungo alla lista solo i Lav_Cod che rispettano le condizioni
            ' - Lav_Cod non gia presente in lista
            ' - Lav_Cod <> 0
            Dim j As Integer = 0
            Dim counter As New List(Of Integer)

            For j = 0 To DT.Rows.Count - 1 'DT.Rows.Count - 1 To 0 Step -1
                If Not lista_LavCod.Contains(DT.Rows(j)("Lav_Cod")) AndAlso DT.Rows(j)("Lav_Cod") <> 0 AndAlso Not counter.Contains(DT.Rows(j)("Lav_Cod")) Then
                    counter.Add(DT.Rows(j)("Lav_Cod"))
                    operazioniList.Add(New AgronicaCoreModelsSTD.attivita.Lavorazione(DT.Rows(j)("Lav_Cod"), DT.Rows(j)("Lav_Des")))
                End If
            Next

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return operazioniList

    End Function

    Public Function isAmmissibile(ByVal lista_LavCod As Integer(),
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Operazioni_Combinazioni_R.isAmmissibile()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim ammissibile As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine("     SELECT * ")
            StrSQL.AppendLine("     FROM  Operazioni_Combinazioni ")

            If Not IsNothing(lista_LavCod) And lista_LavCod.Count > 0 Then
                StrSQL.AppendLine("     WHERE ")
            End If

            'Dummy mi serve per sapere se sono dalla seconda iterazione in poi, per poter aggiungere AND
            Dim dummy As Integer = 0
            'Ciclo per filtrare i diversi lav_cod passati nella lista su ogni colonna
            For Each Lav_Cod In lista_LavCod
                If dummy >= 1 Then
                    StrSQL.AppendLine("     AND  ")
                End If

                StrSQL.AppendLine("     ( ")
                StrSQL.AppendLine("         Lav_Cod1 = " & Agro_SQL_SaveNum(Lav_Cod) & " ")
                StrSQL.AppendLine("         OR Lav_Cod2 = " & Agro_SQL_SaveNum(Lav_Cod) & " ")
                StrSQL.AppendLine("         OR Lav_Cod3 = " & Agro_SQL_SaveNum(Lav_Cod) & " ")
                StrSQL.AppendLine("         OR Lav_Cod4 = " & Agro_SQL_SaveNum(Lav_Cod) & " ")
                StrSQL.AppendLine("         OR Lav_Cod5 = " & Agro_SQL_SaveNum(Lav_Cod) & " ")
                StrSQL.AppendLine("     )")

                dummy += 1
            Next

            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)

            If DT.Rows.Count > 0 Then
                ammissibile = True
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return ammissibile

    End Function

    Public Function Leggi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Operazioni_Combinazioni_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim operazioniList As New List(Of AgronicaCoreModelsSTD.attivita.Lavorazione)

        'nrColonneLavCod serve per sapere quante iterazioni con le UNION mi servono
        Dim nrColonneLavCod As Integer = 5
        Dim i As Integer

        Try

            StrSQL.AppendLine(" SELECT * FROM Operazioni_Combinazioni ")
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function
End Class
