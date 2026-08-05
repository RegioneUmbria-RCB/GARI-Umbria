Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Public Class SpecieVegetaliXStadiCrescita_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal VEG_COD As Integer,
                          ByVal ID_BBCH As Integer,
                          ByVal FF_COD As Integer,
                          ByVal SoloVisibili As Boolean,
                          ByVal SoloFioritura As Boolean,
                          ByVal SoloRipresaVegetativa As Boolean,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          Optional ByVal Personalizzate As Boolean = False,
                          Optional ByVal Piva_SuperUser As String = "",
                          Optional ByVal estraiPersonalizzatePerAPP As Boolean = False
                          ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.SpecieVegetaliXStadiCrescita_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  SpecieVegetaliXStadiCrescita  ss inner join SpecieVegetali s on ss.VEG_COD = s.VEG_COD ")
            StrSQL.Append(" left outer join Stadi_Crescita_BBCH sc on ss.ID_BBCH = sc.ID_BBCH ")

            If Personalizzate = True Then
                StrSQL.Append(" inner join SuperUser_OperazioniPersonalizzate sp on sp.codice = ss.cod_ss ")
            End If

            StrSQL.Append(" WHERE ss.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   ss.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.Append(" AND   s.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   s.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            'StrSQL.Append(" AND   sc.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            'StrSQL.Append(" AND   sc.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Personalizzate = True Then
                'Per l'estrazione da APP dobbiamo estrarre TUTTE le personalzzate, indipendentemente dall'superuser
                If estraiPersonalizzatePerAPP = False Then
                    StrSQL.Append(" AND sp.Piva_superUser = '" & Agro_SQL_SaveText(Piva_SuperUser) & "'")
                End If
                StrSQL.Append(" AND sp.lav_cod = " & AgronicaCoreDataProvider.CostantiPersonalizzate.LAVCOD_FASI_FENOLOGICHE & " ")
            End If

            If VEG_COD <> 0 Then
                StrSQL.Append(" AND ss.VEG_COD =  " & Agro_SQL_SaveNum(VEG_COD) & "  ")
            End If

            If ID_BBCH <> 0 Then
                StrSQL.Append(" AND sc.ID_BBCH =  " & Agro_SQL_SaveNum(ID_BBCH) & "  ")
            End If

            If FF_COD <> 0 Then
                StrSQL.Append(" AND ss.FF_COD =  " & Agro_SQL_SaveNum(FF_COD) & "  ")
            End If

            If SoloVisibili = True Then
                StrSQL.Append(" AND ss.Flag_Visibile = 1 ")
            End If

            If SoloFioritura = True Then
                StrSQL.Append(" AND ss.Flag_Fioritura = 1 ")
            End If

            If SoloRipresaVegetativa = True Then
                StrSQL.Append(" AND ss.Flag_RipresaVegetativa = 1 ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   ss.Inviato >=0 ")
                    StrSQL.Append(" AND   s.Inviato >=0 ")
                    'StrSQL.Append(" AND   sc.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   ss.Inviato =-1 ")
                    StrSQL.Append(" AND   s.Inviato =-1 ")
                    'StrSQL.Append(" AND   sc.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------


            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY s.VEG_DES ASC, ss.progressivo ASC ")
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


    Public Function Leggi_DaEpoche(ByVal VEG_COD As Integer,
                                   ByVal strEpoche As String,
                          ByVal SoloVisibili As Boolean,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          Optional ByVal Personalizzate As Boolean = False,
                          Optional ByVal Piva_SuperUser As String = ""
                          ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.SpecieVegetaliXStadiCrescita_R.Leggi_DaEpoche()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  SpecieVegetaliXStadiCrescita  ss inner join SpecieVegetaliXStadiCrescitaXEpoche sse on ss.Cod_SS = sse.Cod_SS ")
            StrSQL.Append(" inner join Stadi_Crescita_BBCH sc on ss.ID_BBCH = sc.ID_BBCH ")

            If Personalizzate = True Then
                StrSQL.Append(" inner join SuperUser_OperazioniPersonalizzate sp on sp.codice = ss.cod_ss ")
            End If

            StrSQL.Append(" WHERE ss.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   ss.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Personalizzate = True Then
                StrSQL.Append(" AND sp.Piva_superUser = '" & Agro_SQL_SaveText(Piva_SuperUser) & "'")
                StrSQL.Append(" AND sp.lav_cod = " & AgronicaCoreDataProvider.CostantiPersonalizzate.LAVCOD_FASI_FENOLOGICHE & " ")
            End If

            If VEG_COD <> 0 Then
                StrSQL.Append(" AND ss.VEG_COD =  " & Agro_SQL_SaveNum(VEG_COD) & "  ")
            End If

            If SoloVisibili = True Then
                StrSQL.Append(" AND ss.Flag_Visibile = 1 ")
            End If

            If strEpoche <> "" Then
                StrSQL.Append(" AND sse.ep_cod IN ( " & Agro_SQL_Save_Clausola_IN(strEpoche) & ") ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   ss.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   ss.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------


            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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
