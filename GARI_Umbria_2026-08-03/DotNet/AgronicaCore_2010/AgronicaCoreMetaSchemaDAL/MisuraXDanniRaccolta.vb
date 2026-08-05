Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.DataProviderExtensions

Public Class MisuraXDanniRaccolta_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi_WS(ByVal Veg_Cod As Int32,
                             ByVal DR_Cod As Int32,
                             ByVal SoloVisibili As Boolean,
                             ByVal xFiltroAggiuntivo As String,
                             ByVal xOrderBy As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                             Optional ByVal Personalizzate As Boolean = False,
                             Optional ByVal Piva_SuperUser As String = "",
                             Optional ByVal NoSpecie As Boolean = False,
                             Optional ByVal FiltraSpecie As Boolean = True,
                             Optional joinPersonalizzate As Boolean = True,
                             Optional ByVal estraiPersonalizzatePerAPP As Boolean = False
                             ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.MisuraXDanniRaccolta_R.Leggi_WS()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine("     drxsp.VEG_COD, ")
            StrSQL.AppendLine("     mdr.Dr_Cod,0, ")
            StrSQL.AppendLine("     mdr.UDM_COD, ")
            StrSQL.AppendLine("     dr.Dr_Des, ")
            StrSQL.AppendLine("     um.UDM_SIM, ")
            StrSQL.AppendLine("     um.UDM_DES ")

            If estraiPersonalizzatePerAPP Then
                StrSQL.AppendLine("     , sp.Piva_superUser ")
            End If

            StrSQL.AppendLine(" FROM       MisuraxDanniRaccolta mdr ")
            StrSQL.AppendLine(" INNER JOIN UnitaMisura um ON mdr.UDM_COD = um.UDM_COD   ")
            StrSQL.AppendLine(" INNER JOIN DanniRaccolta dr ON mdr.DR_COD = dr.DR_Cod ")
            StrSQL.AppendLine(" INNER JOIN DanniRaccoltaxSpecieVegetali drxsp ON mdr.DR_COD = drxsp.DR_Cod ")

            If joinPersonalizzate Then
                If Personalizzate Then
                    StrSQL.AppendLine(" INNER JOIN SuperUser_OperazioniPersonalizzate AS sp ON ")
                    StrSQL.AppendLine("     sp.codice = mdr.DR_Cod ")
                    'StrSQL.AppendLine(" AND sp.Veg_Cod = drxsp.Veg_Cod ")
                Else
                    StrSQL.AppendLine(" LEFT OUTER JOIN SuperUser_OperazioniPersonalizzate AS sp ON ")
                    StrSQL.AppendLine("     sp.codice = mdr.DR_Cod ")
                    'StrSQL.AppendLine(" AND sp.Veg_Cod = drxsp.Veg_Cod ")
                End If
            End If

            StrSQL.AppendLine(" WHERE mdr.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine))
            StrSQL.AppendLine(" AND   mdr.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))
            StrSQL.AppendLine(" AND   dr.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine))
            StrSQL.AppendLine(" AND   dr.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))
            StrSQL.AppendLine(" AND   drxsp.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine))
            StrSQL.AppendLine(" AND   drxsp.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))
            StrSQL.AppendLine(" AND   um.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine))
            StrSQL.AppendLine(" AND   um.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))


            If SoloVisibili = True Then
                'StrSQL.AppendLine(" AND mdr.Fondamentale <> 0 ")
            End If

            If FiltraSpecie Then
                If NoSpecie Then
                    StrSQL.AppendLine(" AND drxsp.VEG_COD = 0")
                Else
                    If Veg_Cod <> 0 Then
                        StrSQL.AppendLine(" AND drxsp.VEG_COD = " & Agro_SQL_SaveNum(Veg_Cod))
                    End If
                End If
            End If

            If DR_Cod <> 0 Then
                StrSQL.AppendLine(" AND mdr.Dr_Cod = " & Agro_SQL_SaveNum(DR_Cod))
            End If

            If joinPersonalizzate Then
                If Personalizzate = True Then
                    'Per l'estrazione da APP dobbiamo estrarre TUTTE le personalzzate, indipendentemente dall'superuser
                    If estraiPersonalizzatePerAPP = False Then
                        StrSQL.AppendLine(" AND sp.Piva_superUser = '" & Agro_SQL_SaveText(Piva_SuperUser) & "'")
                    End If
                    StrSQL.AppendLine(" AND sp.lav_cod = " & AgronicaCoreDataProvider.CostantiPersonalizzate.LAVCOD_DANNI_RACCOLTA)
                Else
                    'Se solo NON personalizzati (da nessuna PIVA SUPERUSER), eslcudiamo quelli personalizzati
                    StrSQL.Append(" AND sp.codice IS NULL ")
                End If
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY dr.Dr_Des, um.UDM_DES ")
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

End Class
