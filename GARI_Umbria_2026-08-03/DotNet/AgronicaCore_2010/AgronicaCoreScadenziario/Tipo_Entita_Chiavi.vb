Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################


Public Class Tipo_Entita_Chiavi_R
    Inherits AgronicaCoreDataProvider.DataProvider


    'Versione che utilizza AgronicaCoreParametri
    '##############################################################################################
    Public Function Leggi(ByVal ID_Tipologia As Integer,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreScadenziario_DAL.Tipo_Entita_Chiavi_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT Tipo_Entita_chiavi.*  ")
            StrSQL.AppendLine(" FROM   Alert_Tipologia  ")

            StrSQL.AppendLine(" INNER JOIN   Alert_Area on Alert_Area.Id_Area = Alert_Tipologia.ID_Area ")
            StrSQL.AppendLine(" INNER JOIN   Tipo_Entita ON Alert_Area.TipoEntita_Cod = Tipo_Entita.TipoEntita_Cod  ")
            StrSQL.AppendLine(" INNER JOIN   Tipo_Entita_Chiavi ON Tipo_Entita.TipoEntita_Cod = Tipo_Entita_Chiavi.TipoEntita_Cod  ")

            StrSQL.AppendLine(" WHERE 1= 1 ")

            If ID_Tipologia <> 0 Then
                StrSQL.AppendLine(" AND ID_Tipologia = " & Agro_SQL_SaveNum(ID_Tipologia) & " ")
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


    '##############################################################################################
    Public Function LeggiTipoEntitaCodSecondario(ByVal ID_Tipologia As Integer,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreScadenziario_DAL.Tipo_Entita_Chiavi_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT Tipo_Entita_chiavi.*  ")
            StrSQL.AppendLine(" FROM   Alert_Tipologia  ")

            StrSQL.AppendLine(" INNER JOIN   Alert_Area on Alert_Area.Id_Area = Alert_Tipologia.ID_Area ")
            StrSQL.AppendLine(" INNER JOIN   Tipo_Entita ON Alert_Area.TipoEntita_Cod_Secondario = Tipo_Entita.TipoEntita_Cod  ")
            StrSQL.AppendLine(" INNER JOIN   Tipo_Entita_Chiavi ON Tipo_Entita.TipoEntita_Cod = Tipo_Entita_Chiavi.TipoEntita_Cod  ")

            StrSQL.AppendLine(" WHERE 1= 1 ")

            If ID_Tipologia <> 0 Then
                StrSQL.AppendLine(" AND ID_Tipologia = " & Agro_SQL_SaveNum(ID_Tipologia) & " ")
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


    Public Function GetTipoEntitaCod(ByVal Id_Tipologia As Integer,
                                     ByRef objParametri_Server As AgronicaCoreParametri,
                                     ByVal Piva As String,
                                     ByVal Sa_Cod As Integer,
                                     ByVal appezza As Integer,
                                     ByVal Analisi_Testata_Cod As Integer,
                                     ByVal PC_Testata_Cod As Integer,
                                     ByVal Pua_Cod As Integer,
                                     ByVal Richiesta_Cod As Integer,
                                     ByVal Id_Agenda As Integer,
                                     ByVal Ricetta_Operazione_cod As Integer,
                                        Optional ByVal Id_ImpresexParticelle As Integer = 0
                                     ) As Integer

        Dim TipoEntita_Cod As Integer = 0

        'Dim objxTipoInput As New AgronicaCoreScadenziario.Tipo_Entita_Chiavi_R
        Dim DT_xTipo = New DataTable

        Dim DT_xTipoPrimario As DataTable = Leggi(Id_Tipologia, objParametri_Server)
        Dim DT_xTipoSecondario As DataTable = LeggiTipoEntitaCodSecondario(Id_Tipologia, objParametri_Server)

        Dim queryP1 = From dtP In DT_xTipoPrimario.AsEnumerable()
                      Where dtP.Field(Of String)("Nome_Chiave") <> "Piva"
                      Select dtP
        Dim queryP2 = From dtP In DT_xTipoPrimario.AsEnumerable()
                      Where dtP.Field(Of String)("Nome_Chiave") = "Piva"
                      Select dtP

        If queryP1 IsNot Nothing And queryP1.Count > 0 Then
            DT_xTipo = queryP1.CopyToDataTable()
        End If

        If DT_xTipoSecondario IsNot Nothing And DT_xTipoSecondario.Rows.Count > 0 Then
            DT_xTipo.Merge(DT_xTipoSecondario)
        End If

        If queryP2 IsNot Nothing And queryP2.Count > 0 Then
            DT_xTipo.Merge(queryP2.CopyToDataTable())
        End If

        For Each dr As DataRow In DT_xTipo.Rows
            Select Case dr.Item("Nome_Chiave").ToString.ToLower
                Case "piva"
                    If Piva <> "" Then
                        TipoEntita_Cod = dr.Item("TipoEntita_Cod")
                        Exit For
                    End If
                Case "sa_cod"
                    If Sa_Cod <> 0 Then
                        TipoEntita_Cod = dr.Item("TipoEntita_Cod")
                        Exit For
                    End If
                Case "appezza"
                    If appezza <> 0 Then
                        TipoEntita_Cod = dr.Item("TipoEntita_Cod")
                        Exit For
                    End If
                Case "mac_cod"
                    'If Mac_Cod <> 0 Then
                    TipoEntita_Cod = dr.Item("TipoEntita_Cod")
                    Exit For
                    ' End If
                Case "cod_contatto"
                    'If Cod_Contatto <> "" Then
                    TipoEntita_Cod = dr.Item("TipoEntita_Cod")
                    Exit For
                    'End If
                Case "analisi_testata_cod"
                    If Analisi_Testata_Cod <> 0 Then
                        TipoEntita_Cod = dr.Item("TipoEntita_Cod")
                        Exit For
                    End If
                Case "pc_testata_cod"
                    If PC_Testata_Cod <> 0 Then
                        TipoEntita_Cod = dr.Item("TipoEntita_Cod")
                        Exit For
                    End If
                Case "pua_cod"
                    If Pua_Cod <> 0 Then
                        TipoEntita_Cod = dr.Item("TipoEntita_Cod")
                        Exit For
                    End If
                Case "id_agenda"
                    If Id_Agenda <> 0 Then
                        TipoEntita_Cod = dr.Item("TipoEntita_Cod")
                        Exit For
                    End If
                Case "richiesta_cod"
                    If Richiesta_Cod <> 0 Then
                        TipoEntita_Cod = dr.Item("TipoEntita_Cod")
                        Exit For
                    End If
                Case "ricetta_operazione_cod"
                    If Ricetta_Operazione_cod <> 0 Then
                        TipoEntita_Cod = dr.Item("TipoEntita_Cod")
                        Exit For
                    End If
                Case "id_ImpresexParticelle"
                    If Id_ImpresexParticelle <> 0 Then
                        TipoEntita_Cod = dr.Item("TipoEntita_Cod")
                        Exit For
                    End If
            End Select
        Next

        Return TipoEntita_Cod

    End Function

End Class

