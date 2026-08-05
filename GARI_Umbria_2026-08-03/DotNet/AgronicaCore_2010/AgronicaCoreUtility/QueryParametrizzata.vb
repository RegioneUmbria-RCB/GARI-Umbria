
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider

Public Class QueryParametrizzata_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '#############################################################################################
    Public Function Modifica_Parametrizzata(ByVal NomeTabella As String,
                                            ByVal Campo As String,
                                            ByVal Valore As Object,
                                            ByVal ClausolaWHERE As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            Optional ByVal Flag_DataModifica As Boolean = False,
                                            Optional ByVal Flag_EseguiQuery_ScritturaNum As Boolean = False,
                                            Optional ByRef N_Record_Modificati As Integer = 0) As Boolean


        '----- Descrizione
        'Dim DescrizioneFunzione As String = "Modifica_Parametrizzata"
        Dim NomeRoutine As String = "AnagrafeCoreUtility.QueryParametrizzata_WQueryParametrizzata_W.Modifica_Parametrizzata()"

        '----- Variabili

        Dim xRisp As Boolean = False
        Dim sSql As New System.Text.StringBuilder
        Dim MessaggioErrore As String = ""


        Dim strAssegnamento As String = String.Empty

        ' a seconda del tipo del valore che devo aggiornare, formatto la query
        Dim Stringa As Type = GetType(System.String)
        Dim Data As Type = GetType(System.DateTime)
        Dim Intero32 As Type = GetType(System.Int32)


        Dim TypeVal As Type = Valore.GetType()

        Try


            If TypeVal.Equals(Stringa) Then

                strAssegnamento = Campo & "= '" & Agro_SQL_SaveText(Valore.ToString) & "' "

            ElseIf TypeVal.Equals(Data) Then

                strAssegnamento = Campo & "= " & Agro_SQL_SaveDate(Valore.ToString) & " "

            Else

                strAssegnamento = Campo & "= " & Agro_SQL_SaveNum(Valore.ToString) & " "

            End If


            '----- Genero la query SQL 
            sSql.Length = 0
            sSql.Append(" UPDATE " & NomeTabella & " SET ")

            sSql.Append(strAssegnamento)

            If Flag_DataModifica Then
                sSql.Append(", Data_Modifica = " & Agro_SQL_SaveDateTime(Date.Now))
            End If

            sSql.Append(ClausolaWHERE)


            'NumeroRecordInteressati = objSQL.SqlInsertUpdate_New(Connessione, _
            '                                                    Transazione, _
            '                                                    sSql.ToString, _
            '                                                    Messaggio)



            If Flag_EseguiQuery_ScritturaNum Then

                '--------------------------------------------------------------------------
                xRisp = EseguiQuery_ScritturaNum(objParametri, sSql.ToString, NomeRoutine, N_Record_Modificati)
                '--------------------------------------------------------------------------

            Else

                '--------------------------------------------------------------------------
                xRisp = EseguiQuery_Scrittura(objParametri, sSql.ToString, NomeRoutine)
                '--------------------------------------------------------------------------

            End If


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function


End Class
