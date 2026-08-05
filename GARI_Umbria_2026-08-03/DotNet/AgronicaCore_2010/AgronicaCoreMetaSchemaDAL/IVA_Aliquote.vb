Imports System.Text
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class IVA_Aliquote_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    ''' <summary>
    ''' Legge il contenuto della tabella Iva_Aliquote
    ''' </summary>
    ''' <param name="Codice"> Codice Iva </param>
    ''' <param name="Aliquota"> Aliquota IVA (passare -1 per non filtrare) </param>
    ''' <param name="Tipologia">Tipologia Aliquota (passare -1 per non filtrare) </param>
    ''' <param name="Flag_Credito_Imposta_Export"> (passare -1 per non filtrare) </param>
    ''' <param name="NaturaEsclusione"> (passare la costante NATURA_ESCLUSIONE_NOFILTRO (99) per non filtrare) </param>
    ''' <param name="xFiltroAggiuntivo">(opzionale = "")</param>
    ''' <param name="xOrderBy">(opzionale = "")</param>
    ''' <param name="objParametri"></param>
    Public Function Leggi(ByVal Codice As Integer,
                          ByVal Aliquota As Decimal,
                          ByVal Tipologia As Integer,
                          ByVal Flag_Credito_Imposta_Export As Integer,
                          ByVal NaturaEsclusione As String,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.IVA_Aliquote_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        'codice = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim dt As New DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM  IVA_Aliquote ")
            StrSQL.AppendLine(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Codice <> 0 Then
                StrSQL.AppendLine(" AND Codice = " & Agro_SQL_SaveNum(Codice) & " ")
            End If

            If Aliquota <> -1 Then
                StrSQL.AppendLine(" AND Aliquota = " & Agro_SQL_SaveNum(Aliquota) & " ")
            End If

            If Tipologia <> -1 Then
                StrSQL.AppendLine(" AND Tipologia = " & Agro_SQL_SaveNum(Tipologia) & " ")
            End If

            If Flag_Credito_Imposta_Export <> -1 Then
                StrSQL.AppendLine(" AND Flag_Credito_Imposta_Export = " & Agro_SQL_SaveNum(Flag_Credito_Imposta_Export) & " ")
            End If

            If NaturaEsclusione <> NATURA_ESCLUSIONE_NOFILTRO Then
                StrSQL.AppendLine(" AND NaturaEsclusione_2 = '" & Agro_SQL_SaveText(NaturaEsclusione) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Codice ")
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

    '#########################################################
    Public Function Aliquota_from_CodIVA(ByVal Cod_Iva As Integer,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                         ) As String

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.IVA_Aliquote_R.Aliquota_from_CodIVA()"

        '====================================================================================
        '
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim siglaIva As String = ""
        
        Try

            dt = Leggi(Cod_Iva, -1, -1, -1, NATURA_ESCLUSIONE_NOFILTRO,
                       xFiltroAggiuntivo, "", objParametri)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                siglaIva = dt.Rows(0).Item("Sigla")
            End If

            dt = Nothing

        Catch ex As Exception
            siglaIva = ""
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return siglaIva

    End Function

    '#########################################################
    '  Giulia, 13/02/2017 18:01:45: Restituisce il valore effettivo dell'aliquota (utilizzabile per i calcoli)
    Public Function AliquotaFloat_from_CodIVA(ByVal Cod_Iva As Integer,
                                              ByVal xFiltroAggiuntivo As String,
                                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                              ) As Decimal

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.IVA_Aliquote_R.AliquotaFloat_from_CodIVA()"

        '====================================================================================
        '
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim aliquota As Decimal = 0


        Try

            dt = Leggi(Cod_Iva, -1, -1, -1, NATURA_ESCLUSIONE_NOFILTRO,
                       xFiltroAggiuntivo, "", objParametri)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                aliquota = CDec(dt.Rows(0).Item("Aliquota"))
            End If

            dt = Nothing

        Catch ex As Exception
            aliquota = 0
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return aliquota

    End Function

    '#########################################################
    Public Function FlagCreditoImpostaExport_from_CodIVA(ByVal Cod_Iva As Integer,
                                                         ByVal xFiltroAggiuntivo As String,
                                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                         ) As Integer

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.IVA_Aliquote_R.FlagCreditoImpostaExport_from_CodIVA()"

        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim flagCreditoImpostaExport As Integer = 0


        Try

            dt = Leggi(Cod_Iva, -1, -1, -1, NATURA_ESCLUSIONE_NOFILTRO,
                       xFiltroAggiuntivo, "", objParametri)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                flagCreditoImpostaExport = CDec(dt.Rows(0).Item("Flag_Credito_Imposta_Export"))
            End If

            dt = Nothing

        Catch ex As Exception
            flagCreditoImpostaExport = 0
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return flagCreditoImpostaExport

    End Function

End Class
