Imports System.Data.Entity
Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO


'#################################################################
'#################################################################
'#################################################################

Public Class MisuraXAvversita_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi_Misura_xPaginaEdit(
        ByVal Veg_Cod As Int32,
        ByVal Av_Cod As Int32,
        ByVal Abbreviazione As String,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreParametri
     ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Avversita_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            StrSQL.Length = 0
            StrSQL.Append(" SELECT  ")
            StrSQL.Append("     MisuraxAvversita.VEG_COD,  ")
            StrSQL.Append("     Avversita.Av_Cod, ")
            StrSQL.Append("     Avversita.Av_Des_Vol, ")
            StrSQL.Append("     UnitaMisura.UDM_COD, ")
            StrSQL.Append("     UnitaMisura.UDM_SIM, ")
            StrSQL.Append("     UnitaMisura.UDM_DES, ")
            StrSQL.Append("     MisuraxAvversita.COD,  ")
            StrSQL.Append("     Veg.Veg_Des,  ")
            StrSQL.Append("     ff.FF_COD,  ")
            StrSQL.Append("     ff.FF_DES,  ")
            StrSQL.Append("     MisuraXAvversita.Ordine,  ")
            StrSQL.Append("     MisuraXAvversita.Fondamentale  ")
            StrSQL.Append(" FROM    MisuraxAvversita INNER JOIN      ")
            StrSQL.Append("     Avversita ON MisuraxAvversita.AV_COD = Avversita.Av_Cod INNER JOIN  ")
            StrSQL.Append("     UnitaMisura ON MisuraxAvversita.UDM_COD = UnitaMisura.UDM_COD  ")
            StrSQL.Append("     inner join specieVegetali Veg on veg.veg_cod = MisuraXAvversita.Veg_Cod ")
            StrSQL.Append("     inner join (select 0 as ff_cod, 'Non Specificata' as FF_DES union all select ff_cod, ff_Des from FasiFenologiche) ff on ff.ff_cod = MisuraXAvversita.FF_Cod ")

            StrSQL.Append(" WHERE  1=1  ")




            If Veg_Cod <> 0 Then
                StrSQL.Append("     AND  (MisuraxAvversita.VEG_COD = " & Agro_SQL_SaveNum(Veg_Cod) & ")  ")
            End If

            If Av_Cod <> 0 Then
                StrSQL.Append(" AND Avversita.Av_Cod = " & Agro_SQL_SaveNum(Av_Cod) & "  ")
            End If

            If Abbreviazione <> "" Then
                StrSQL.Append(" AND Avversita.Abbreviazione LIKE '%" & Agro_SQL_SaveNum(Abbreviazione) & "%'  ")
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If


            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Avversita.Av_Des_Vol, UnitaMisura.UDM_DES      ")

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


    Public Function Leggi(
                ByVal COD As String,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As MisuraxAvversita

        Dim TestataElem As MisuraxAvversita = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.MisuraXAvversita_R.Leggi()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            TestataElem =
           (From m In GiasContext.MisuraxAvversita
            Where m.COD = COD
            Select m).FirstOrDefault()

        End Using

        Return TestataElem


    End Function

    Public Function Leggi_WS(ByVal Veg_Cod As Int32,
                                ByVal Av_Cod As Int32, ByVal Av_Gru As Int32,
                                ByVal Dpi_Cod As Int32, ByVal Id_Rcdpi As Int32,
                                ByVal TipoTestata As Int32,
                                ByVal SoloVisibili As Boolean,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                    ByRef ObjParametri_Disciplinari As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        Optional ByVal Personalizzate As Boolean = False,
                                        Optional ByVal Piva_SuperUser As String = "",
                                        Optional ByVal estraiPersonalizzatePerAPP As Boolean = False
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.MisuraXAvversita_R.Leggi_WS()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Dim NomeDB_Disciplinari As String
            NomeDB_Disciplinari = ObjParametri_Disciplinari.StringaConnessione.Split(";")(2)
            NomeDB_Disciplinari = NomeDB_Disciplinari.Split("=")(1)


            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT  ")
            StrSQL.AppendLine("     ma.COD,  ")
            StrSQL.AppendLine("     ma.VEG_COD,  ")
            StrSQL.AppendLine("     ISNULL(ma.Av_Cod,0) AS Av_Cod,  ISNULL(ma.Av_Gru,0) AS Av_Gru, ")
            StrSQL.AppendLine("     ma.UDM_COD, ")
            StrSQL.AppendLine("     ma.Fondamentale, ")
            StrSQL.AppendLine("     ma.FF_COD, ")
            StrSQL.AppendLine("     ma.Ordine, ")
            StrSQL.AppendLine("     ISNULL(a.Av_Des_Vol,'') AS Av_Des_Vol, ")
            StrSQL.AppendLine("     ISNULL(a.Av_Des_Lat,'') AS Av_Des_Lat, ")
            StrSQL.AppendLine("     ISNULL(a.Abbreviazione,'') AS Abbreviazione, ")
            StrSQL.AppendLine("     ISNULL(ga.Av_Gru_Des,'') AS Av_Gru_Des, ")
            StrSQL.AppendLine("     ISNULL(ga.Av_Gru_Des_Lat,'') AS Av_Gru_Des_Lat, ")
            StrSQL.AppendLine("     um.UDM_SIM, ")
            StrSQL.AppendLine("     um.UDM_DES, ")
            StrSQL.AppendLine("     maa.Anag_DES, ")
            StrSQL.AppendLine("     maa.Anag_Valore, ")
            StrSQL.AppendLine("     ISNULL(um.TipoControllo_Cod, 0) AS TipoControllo_Cod")

            If Dpi_Cod > 0 Then
                StrSQL.AppendLine(" , S.SI_COD ")
            End If

            If estraiPersonalizzatePerAPP Then
                StrSQL.AppendLine(" , sp.Piva_superUser ")
            End If

            StrSQL.AppendLine(" FROM MisuraxAvversita ma INNER JOIN      ")
            StrSQL.AppendLine("     UnitaMisura um ON ma.UDM_COD = um.UDM_COD   ")
            StrSQL.AppendLine("     LEFT OUTER JOIN  Avversita a ON ma.AV_COD = a.Av_Cod ")
            StrSQL.AppendLine("     LEFT OUTER JOIN  GruppoAvversita ga ON ma.AV_GRU = ga.Av_Gru ")

            If Personalizzate = True Then
                StrSQL.AppendLine(" INNER JOIN SuperUser_OperazioniPersonalizzate sp on sp.codice = ma.cod ")
            End If

            '(29/10/2020 fede) modificata lettura per includere le avversità del dpi (non soglia)
            If Dpi_Cod > 0 Then
                'StrSQL.AppendLine(" INNER JOIN " & NomeDB_Disciplinari & ".dbo.SoglieIntervento s on ma.udm_cod=s.udm_cod_specifico " & vbCrLf)
                'StrSQL.AppendLine(" INNER JOIN " & NomeDB_Disciplinari & ".dbo.difesarighe dr on dr.DFR_COD=s.dfr_cod " & vbCrLf)
                'StrSQL.AppendLine(" INNER JOIN " & NomeDB_Disciplinari & ".dbo.infestanti i on i.dft_cod=dr.DFt_COD and i.dfr_cod=dr.DFR_COD and ma.av_cod=i.av_cod  " & vbCrLf)
                'StrSQL.AppendLine(" INNER JOIN " & NomeDB_Disciplinari & ".dbo.difesatestata dt on dt.DFT_COD=dr.dft_cod " & vbCrLf)
                'StrSQL.AppendLine(" INNER JOIN " & NomeDB_Disciplinari & ".dbo.RegolamentiXDifesaTestata rdt on rdt.dft_cod=dt.dft_cod " & vbCrLf)
                'StrSQL.AppendLine(" INNER JOIN " & NomeDB_Disciplinari & ".dbo.RaggruppamentiDPIXSpecieVegetali rs on rs.id_rcdpi=dt.id_rcdpi and ma.veg_cod=rs.veg_cod " & vbCrLf)
                StrSQL.AppendLine(" INNER JOIN " & NomeDB_Disciplinari & ".dbo.infestanti i on ma.av_cod=i.av_cod " & vbCrLf)
                StrSQL.AppendLine(" INNER JOIN " & NomeDB_Disciplinari & ".dbo.difesarighe dr on dr.DFR_COD=i.dfr_cod and dr.DFT_COD=i.dft_cod  " & vbCrLf)
                StrSQL.AppendLine(" INNER JOIN " & NomeDB_Disciplinari & ".dbo.difesatestata dt on dt.DFT_COD=dr.dft_cod " & vbCrLf)
                StrSQL.AppendLine(" INNER JOIN " & NomeDB_Disciplinari & ".dbo.RegolamentiXDifesaTestata rdt on rdt.dft_cod=dt.dft_cod " & vbCrLf)
                StrSQL.AppendLine(" INNER JOIN " & NomeDB_Disciplinari & ".dbo.RaggruppamentiDPIXSpecieVegetali rs on rs.id_rcdpi=dt.id_rcdpi and ma.veg_cod=rs.veg_cod " & vbCrLf)
                StrSQL.AppendLine(" LEFT JOIN " & NomeDB_Disciplinari & ".dbo.SoglieIntervento S on dr.DFR_COD=s.dfr_cod AND ma.udm_cod=s.udm_cod_specifico " & vbCrLf)
            End If

            StrSQL.AppendLine("     LEFT JOIN MisuraXAvversita_Anagrafiche maa ON ma.COD = maa.MxAV_COD  ")

            StrSQL.AppendLine("     WHERE a.Av_Des_Vol NOT LIKE '%non usare%' ")
            StrSQL.AppendLine("     AND a.Av_Des_Vol NOT LIKE '%(#)%' ")

            If SoloVisibili = True Then
                StrSQL.AppendLine(" AND    (ma.Fondamentale <> 0)    ")
            End If

            If Veg_Cod <> 0 Then
                StrSQL.AppendLine("     AND  (ma.VEG_COD = " & Agro_SQL_SaveNum(Veg_Cod) & ")  ")
            End If

            If Av_Cod <> 0 Then
                StrSQL.AppendLine(" AND a.Av_Cod = " & Agro_SQL_SaveNum(Av_Cod) & "  ")
            End If

            If Dpi_Cod > 0 Then
                StrSQL.AppendLine(" AND rdt.cod_regolamento = " & Agro_SQL_SaveNum(Dpi_Cod) & "  ")
                StrSQL.AppendLine(" AND rs.id_rcdpi = " & Agro_SQL_SaveNum(Id_Rcdpi) & "  ")
            End If

            If Personalizzate = True Then
                'Per l'estrazione da APP dobbiamo estrarre TUTTE le personalzzate, indipendentemente dall'superuser
                If estraiPersonalizzatePerAPP = False Then
                    StrSQL.AppendLine(" AND sp.Piva_superUser = '" & Agro_SQL_SaveText(Piva_SuperUser) & "'")
                End If
                Select Case TipoTestata
                    Case 0
                        StrSQL.AppendLine(" AND sp.lav_cod = " & AgronicaCoreDataProvider.CostantiPersonalizzate.LAVCOD_RILIEVO_AVVERSITA_CAMPO)
                    Case 1
                        StrSQL.AppendLine(" AND sp.lav_cod = " & AgronicaCoreDataProvider.CostantiPersonalizzate.LAVCOD_RILIEVO_ERBE_INFESTANTI)
                End Select
            End If

                '--------------------------------------------------------------------------
                If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                If Dpi_Cod > 0 Then
                    StrSQL.AppendLine(" ORDER BY a.Av_Des_Vol, S.SI_COD DESC, um.UDM_DES ")
                Else
                    StrSQL.AppendLine(" ORDER BY a.Av_Des_Vol, um.UDM_DES ")
                End If
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



Public Class MisuraXAvversita_W
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Aggiorna_MisuraXAvversita(
                ByVal EFArrayToInsert As ArrayList,
                ByVal EFArrayToUpdate As ArrayList,
                ByVal EFArrayToDelete As ArrayList,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                ) As String

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

        Dim retries As Integer = 3
        Dim success As Boolean = True

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.AgronicaCoreMetaschemaDAL.Aggiorna_MisuraXAvversita()"
        Dim MessaggioErrore As String = ""

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Using scope As New TransactionScope()

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    Dim idSeq As Integer = 0

                    For Each curMisuraXAvversita As MisuraxAvversita In EFArrayToInsert

                        success = False
                        For i As Integer = 0 To retries - 1

                            Try

                                'Richiedo un nuovo id sequenza
                                idSeq = ObjSequenze.NuovoId_Tabella_EF(GiasContext,
                                               "MisuraXAvversita", 0, 2000000000, objParametri)

                                curMisuraXAvversita.COD = idSeq
                                GiasContext.MisuraxAvversita.Add(curMisuraXAvversita)
                                GiasContext.SaveChanges()
                                success = True

                                Exit For
                            Catch ex As Exception
                                Threading.Thread.Sleep(500) ' 500 milliseconds = 0.5 seconds
                            End Try
                        Next
                        ' Al primo errore evito di continuare le modifiche
                        If Not success Then
                            MessaggioErrore = "Non sono riuscito ad aggiornare i dati dopo " & retries & " tentativi."
                            Exit For
                        End If
                    Next

                    If success Then
                        For Each listFattVar As MisuraxAvversita In EFArrayToUpdate
                            GiasContext.MisuraxAvversita.Attach(listFattVar)
                            GiasContext.Entry(listFattVar).State = EntityState.Modified
                            GiasContext.SaveChanges()
                        Next

                        For Each listFattVar As MisuraxAvversita In EFArrayToDelete
                            GiasContext.MisuraxAvversita.Attach(listFattVar)
                            GiasContext.MisuraxAvversita.Remove(listFattVar)
                            GiasContext.SaveChanges()
                        Next

                        ' COMMIT Effettivo
                        scope.Complete()
                    End If

                End Using
            End Using


        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)

        End Try

        Return MessaggioErrore

    End Function



End Class

