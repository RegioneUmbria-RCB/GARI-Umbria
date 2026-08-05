Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri


Public Class Prodotti_Relazioni_R
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Leggi_Vincolo_Formulati(ByVal Pro_Cod As Int32,
                                            ByVal Validita_Inizio As Date,
                                            ByVal Validita_Fine As Date,
                                            ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Prodotti_Relazioni_R.Leggi_Vincolo_Formulati()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Append(" SELECT Prodotti_Relazioni.*, Formulati.Fr_Des as Pro_Des, Formulati_Dipendente.Fr_Des as ProDes_Dipendente  " &
                                " FROM  Prodotti_Relazioni, Formulati, Formulati as Formulati_Dipendente  " &
                                " WHERE Prodotti_Relazioni.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " &
                                " AND   Prodotti_Relazioni.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " " &
                                " AND   Prodotti_Relazioni.Elem_Cod = 191 " &
                                " AND   Prodotti_Relazioni.ElemCod_Dipendente = 191 " &
                                " AND   Formulati.Fr_Cod =  Prodotti_Relazioni.Pro_Cod " &
                                " AND   Formulati_Dipendente.Fr_Cod =  Prodotti_Relazioni.ProCod_Dipendente ")

                    If Pro_Cod <> 0 Then
                        StrSQL.Append(" AND Prodotti_Relazioni.Pro_Cod =  " & Agro_SQL_SaveNum(Pro_Cod) & " ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Tipo_Dipendenza ASC ")
                    End If

                    '##############################################################

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta



            End Select

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
