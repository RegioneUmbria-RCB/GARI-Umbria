Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

'########################################################################
'########################################################################
'########################################################################
'########################################################################
'########################################################################
'########################################################################

Public Class OrganismoReferente_Read
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Leggi(ByVal FlagLeggiPubblici As Boolean, _
                                ByVal Piva As String, _
                                ByVal Piva_Organismo As String, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.OrganismoReferente_Read.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim DT_ORG As New DataTable

        DT_ORG.Columns.Add(New DataColumn("piva_padre", GetType(String)))
        DT_ORG.Columns.Add(New DataColumn("ragsoc_padre", GetType(String)))

        Dim objcontatti As New AgronicaCoreAnagrafeDAL.Contatti_R

        Try

            DT = objcontatti.Leggi_Contatti_ByCod_Rapporto(FlagLeggiPubblici, _
                                                            Piva, _
                                                            Piva_Organismo, _
                                                            COD_ORGANISMO_REFERENTE, _
                                                            "", "", _
                                                            objParametri)

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                Dim i As Integer
                Dim Dr As DataRow

                For i = 0 To DT.Rows.Count - 1

                    Dr = DT_ORG.NewRow

                    Dr.Item("piva_padre") = DT.Rows(i).Item("Cod_Contatto")
                    Dr.Item("ragsoc_padre") = DT.Rows(i).Item("rag_soc")

                    DT_ORG.Rows.Add(Dr)

                Next

            End If


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT_ORG = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT_ORG

    End Function

    '################################################################################
    Public Function OrganismoReferente_from_Piva(ByVal Piva_Organismo_Referente As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                    ) As String

        Dim DT As DataTable

        DT = Leggi(True, "", Piva_Organismo_Referente,
                        "", "",
                        objParametri)
        If DT.Rows.Count = 0 Then
            Dim objGerarchia As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R
            DT = objGerarchia.LeggiPadriGerarchia(Piva_Organismo_Referente, "", 0, 0, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri)
        End If
        If DT.Rows.Count <> 0 Then
            Return DT.Rows(0).Item("ragsoc_padre")
        Else
            Return ""
        End If


    End Function

    '##############################################################################################
    Public Function Leggi_OLD(ByVal Piva As String, _
                          ByVal Piva_Padre As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_Campione_Read.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            StrSQL.Append("  (   SELECT  GerarchiaImprese.Padre AS piva_padre, Imprese.rag_soc AS ragsoc_padre ")
            StrSQL.Append("      FROM    GerarchiaImprese INNER JOIN ")
            StrSQL.Append("             Imprese ON GerarchiaImprese.Padre = Imprese.PIVA ")
            StrSQL.Append("             where 1= 1 ")
            If Piva <> "" Then
                StrSQL.Append(" AND     GerarchiaImprese.Figlio  = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If
            If Piva_Padre <> "" Then
                StrSQL.Append(" AND     GerarchiaImprese.Padre = '" & Agro_SQL_SaveText(Piva_Padre) & "'   ")
            End If
            StrSQL.Append(" ) ")


            StrSQL.Append("     UNION ")

            StrSQL.Append(" (   SELECT  Contatti.cod_contatto AS piva_padre, Contatti.rag_soc AS ragsoc_padre ")
            StrSQL.Append("     FROM    Contatti INNER JOIN ")
            StrSQL.Append("             Risorse_Umane ON Contatti.piva = Risorse_Umane.piva AND Contatti.cod_contatto = Risorse_Umane.cod_contatto ")
            StrSQL.Append("             INNER JOIN UtentiXImprese ON Contatti.Piva = UtentiXImprese.PIVA ")
            StrSQL.Append("     WHERE   Risorse_Umane.cod_rapporto = - 2 ")
            StrSQL.Append("     AND     UtentiXImprese.[USER] = '" & CStr(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("     AND     Contatti.id_cf = 1 ")
            If Piva <> "" Then
                'cerco le possibili cooperative referenti, create dall'azienda o pubbliche (create da altre aziende)
                StrSQL.Append(" AND    (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' OR Contatti.Sa_Cod = -1 )  ")
            End If
            If Piva_Padre <> "" Then
                'cerco un particolare organismo referente
                StrSQL.Append(" AND     Contatti.cod_contatto = '" & Agro_SQL_SaveText(Piva_Padre) & "'   ")
            End If
            StrSQL.Append("     AND   Risorse_Umane.Cod_RisUm_Origine = 0   ")
            StrSQL.Append(" ) ")
            StrSQL.Append("")

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