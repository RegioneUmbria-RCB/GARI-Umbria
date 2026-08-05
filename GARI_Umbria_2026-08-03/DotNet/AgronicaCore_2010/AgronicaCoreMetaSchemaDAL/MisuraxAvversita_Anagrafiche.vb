Imports System.Data.Entity
Imports System.Text
Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO

Public Class MisuraxAvversita_Anagrafiche_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################

    Public Function LeggiAnagrafiche(ByRef objParametriServer As AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = NameOf(LeggiAnagrafiche)

        Dim messaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            stb.Length = 0

            stb.AppendLine(" SELECT")
            stb.AppendLine("            anag.MxAV_Cod, anag.Anag_cod, anag.Anag_des, anag.Anag_valore, anag.DPI_FlagPrivatoPubblico, anag.DPI_COD, mxav.Veg_Cod, veg.Veg_Des, mxav.Av_Cod, av.Av_Des_Vol, um.UDM_DES ")
            stb.AppendLine(" FROM")
            stb.AppendLine("            MisuraXAvversita_Anagrafiche anag")
            stb.AppendLine(" JOIN")
            stb.AppendLine("            MisuraxAvversita mxav")
            stb.AppendLine("            ON anag.MxAV_Cod = mxav.COD")
            stb.AppendLine(" JOIN")
            stb.AppendLine("            SpecieVegetali veg")
            stb.AppendLine("            ON veg.Veg_Cod = mxav.VEG_COD")
            stb.AppendLine(" JOIN")
            stb.AppendLine("            Avversita av")
            stb.AppendLine("            ON av.Av_Cod = mxav.AV_COD")
            stb.AppendLine(" JOIN")
            stb.AppendLine("            UnitaMisura um")
            stb.AppendLine("            ON um.UDM_COD = mxav.UDM_COD")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametriServer, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Leggi(ByVal MxAV_Cod As Integer,
                          ByVal av_cod As Integer,
                          ByVal udm_cod As Integer,
                          ByVal veg_cod As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0

            stb.Append("   SELECT mav.Anag_valore as anag_valore, mav.Anag_des as anag_des, mav.MxAV_Cod, mav.Anag_cod as anag_cod" & vbCrLf)
            stb.Append("   FROM  MisuraXAvversita_Anagrafiche mav " & vbCrLf)
            stb.Append("   INNER JOIN   MisuraXAvversita mv " & vbCrLf)
            stb.Append("   on mav.MxAV_Cod = mv.cod " & vbCrLf)
            stb.Append("   WHERE 1=1 ")

            If MxAV_Cod <> 0 Then
                stb.Append("   AND mav.MxAV_Cod = " & Agro_SQL_SaveNum(MxAV_Cod))
            End If

            If av_cod <> 0 Then
                stb.Append("   AND mv.AV_Cod = " & Agro_SQL_SaveNum(av_cod))
            End If

            If veg_cod <> 0 Then
                stb.Append("   AND mv.Veg_Cod = " & Agro_SQL_SaveNum(veg_cod))
            End If

            If udm_cod <> 0 Then
                stb.Append("   AND mv.udm_Cod = " & Agro_SQL_SaveNum(udm_cod))
            End If


            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   mv.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   mv.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function VerificaUtilizzoAnagrafica(codice As Integer,
                                               objParametri As AgronicaCoreParametri) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.MisuraxAvversita_Anagrafiche_R.VerificaUtilizzoAnagrafica()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT 1 esiste ")
            stb.AppendLine(" FROM Mov_Destinazioni dest ")
            stb.AppendLine("        INNER JOIN Agenda a")
            stb.AppendLine("              ON a.PIVA = dest.PIVA")
            stb.AppendLine("              AND a.Sa_Cod = dest.Sa_Cod")
            stb.AppendLine("              AND a.Id_Agenda = dest.Id_Agenda")
            stb.AppendLine("        INNER JOIN Movimenti_dettagli d ")
            stb.AppendLine("              ON dest.tipo_Destinazione = 0  ")
            stb.AppendLine("              AND dest.id_mov_det = d.id_mov_det ")

            stb.AppendLine("        INNER JOIN Mov_Dettaglio_Tecnico tec ")
            stb.AppendLine("              ON tec.id_mov_det = d.id_mov_det ")

            stb.AppendLine("        INNER JOIN Reg_Impianti imp ")
            stb.AppendLine("              ON imp.piva = dest.piva ")
            stb.AppendLine("              AND imp.sa_Cod = dest.Sa_Cod ")
            stb.AppendLine("              AND imp.APPEZZA = dest.Appezza ")
            stb.AppendLine("              AND imp.ID_REG = dest.Id_Destinazione ")

            stb.AppendLine("        INNER JOIN cultivar c ")
            stb.AppendLine("              ON c.Cul_Cod = imp.CUL_COD ")

            stb.AppendLine("        inner join MisuraxAvversita ma ")
            stb.AppendLine("              ON ma.UDM_COD = tec.Dett_Cod ")
            stb.AppendLine("              AND ma.VEG_COD = c.Veg_Cod ")
            stb.AppendLine("              AND ma.AV_COD = tec.Av_Cod ")

            stb.AppendLine("        INNER JOIN MisuraXAvversita_Anagrafiche anag ")
            stb.AppendLine("              ON anag.MxAV_Cod = ma.COD ")
            stb.AppendLine("              AND anag.Anag_valore = dest.Qta ")

            stb.AppendLine(String.Format(" WHERE anag.MxAV_Cod = {0} ", Agro_SQL_SaveNum(codice)))
            stb.AppendLine(" AND a.Lav_Cod = " + Agro_SQL_SaveNum(CostantiPersonalizzate.LAVCOD_RILIEVO_AVVERSITA_CAMPO) + " ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return (DT IsNot Nothing AndAlso DT.Rows.Count > 0)
    End Function

    Public Function LeggiDaCodice(ByVal MxAV_Cod As Integer,
                          ByVal Anag_Cod As Integer,
                          ByVal Anag_Valore As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.MisuraxAvversita_Anagrafiche_R.LeggiDaCodice()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0

            stb.AppendLine("   SELECT MxAV_Cod
                                      , Anag_cod
                                      , Anag_des, Anag_valore
                                      , DPI_FlagPrivatoPubblico
                                      , DPI_COD")
            stb.AppendLine("   FROM  MisuraXAvversita_Anagrafiche ")
            stb.AppendLine("   WHERE 1=1 ")

            If MxAV_Cod <> 0 Then
                stb.AppendLine(String.Format("   AND MxAV_Cod = {0} ", Agro_SQL_SaveNum(MxAV_Cod)))
            End If

            If Anag_Cod <> 0 Then
                stb.AppendLine(String.Format("   AND Anag_Cod = {0} ", Agro_SQL_SaveNum(Anag_Cod)))
            End If

            If Anag_Valore <> 0 Then
                stb.AppendLine(String.Format("   AND Anag_Valore = {0} ", Agro_SQL_SaveNum(Anag_Valore)))
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(String.Format(" AND {0} ", Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri)))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.AppendLine(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.AppendLine(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.AppendLine(String.Format(" ORDER BY {0} ", Agro_SQL_Save_xOrderBy(xOrderBy, objParametri)))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function Leggi_Misura_xPaginaEdit(
                        ByVal MxAV_Cod As Integer,
                        ByVal xFiltroAggiuntivo As String,
                        ByVal xOrderBy As String,
                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0

            stb.Append("   SELECT cast(mav.MxAV_Cod as varchar(100)) + '-' + cast(mav.Anag_valore as varchar(100)) as kendoKey, mav.* " & vbCrLf)
            stb.Append("   FROM  MisuraXAvversita_Anagrafiche mav " & vbCrLf)
            stb.Append("   WHERE 1=1 ")

            If MxAV_Cod <> 0 Then
                stb.Append("   AND mav.MxAV_Cod = " & Agro_SQL_SaveNum(MxAV_Cod))
            End If


            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
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
                ByVal Anag_Valore As Integer,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As MisuraXAvversita_Anagrafiche

        Dim TestataElem As MisuraXAvversita_Anagrafiche = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.MisuraXAvversita_R.Leggi()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            TestataElem =
           (From m In GiasContext.MisuraXAvversita_Anagrafiche
            Where m.MxAV_Cod = COD AndAlso m.Anag_valore = Anag_Valore
            Select m
                ).FirstOrDefault()

        End Using

        Return TestataElem


    End Function
End Class


'#################################################################
'#################################################################
'#################################################################



Public Class MisuraXAvversita_Anagrafiche_W
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Aggiorna_MisuraXAvversita_Anagrafiche(
                ByVal EFArrayToInsert As ArrayList,
                ByVal EFArrayToUpdate As ArrayList,
                ByVal EFArrayToDelete As ArrayList,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                ) As String

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

        Dim retries As Integer = 3
        Dim success As Boolean = True

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.AgronicaCoreMetaschemaDAL.Aggiorna_MisuraXAvversita_Anagrafiche()"
        Dim MessaggioErrore As String = ""

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Using scope As New TransactionScope()

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    Dim idSeq As Integer = 0

                    For Each curMisuraXAvversita_Anagrafiche As MisuraXAvversita_Anagrafiche In EFArrayToInsert

                        success = False
                        For i As Integer = 0 To retries - 1

                            Try

                                GiasContext.MisuraXAvversita_Anagrafiche.Add(curMisuraXAvversita_Anagrafiche)
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
                        For Each listFattVar As MisuraXAvversita_Anagrafiche In EFArrayToUpdate
                            GiasContext.MisuraXAvversita_Anagrafiche.Attach(listFattVar)
                            GiasContext.Entry(listFattVar).State = EntityState.Modified
                            GiasContext.SaveChanges()
                        Next

                        For Each listFattVar As MisuraXAvversita_Anagrafiche In EFArrayToDelete
                            GiasContext.MisuraXAvversita_Anagrafiche.Attach(listFattVar)
                            GiasContext.MisuraXAvversita_Anagrafiche.Remove(listFattVar)
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

    Public Function EliminaAnagrafica(codiceAnagrafica As Integer,
                                      codiceMisura As Integer,
                                      objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.MisuraXAvversita_Anagrafiche_W.EliminaAnagrafica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" DELETE FROM MisuraXAvversita_Anagrafiche ")
            StrSQL.AppendLine(" WHERE ")
            StrSQL.AppendLine(String.Format(" MxAV_Cod = {0} ", Agro_SQL_SaveNum(codiceMisura)))
            StrSQL.AppendLine(String.Format(" AND Anag_cod = {0} ", Agro_SQL_SaveNum(codiceAnagrafica)))
            '---------------------------------------------

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function AggiornaAnagrafica(codiceAnagrafica As Integer,
                                       codiceMisura As Integer,
                                       descrizione As String,
                                       valoreAnagrafica As Integer,
                                       DPI_FlagPrivatoPubblico As Integer,
                                       DPI_COD As Integer,
                                       objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.MisuraXAvversita_Anagrafiche_W.AggiornaAnagrafica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" UPDATE MisuraXAvversita_Anagrafiche SET ")
            StrSQL.AppendLine(String.Format(" Anag_des = '{0}' ", Agro_SQL_SaveText(descrizione)))
            StrSQL.AppendLine(String.Format(" , Anag_valore = {0} ", Agro_SQL_SaveNum(valoreAnagrafica)))
            StrSQL.AppendLine(String.Format(" , DPI_FlagPrivatoPubblico = {0} ", Agro_SQL_SaveNum(DPI_FlagPrivatoPubblico)))
            StrSQL.AppendLine(String.Format(" , DPI_COD = {0} ", Agro_SQL_SaveNum(DPI_COD)))
            StrSQL.AppendLine(String.Format(" , Username_Modifica = '{0}' ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))
            StrSQL.AppendLine(" , Data_Modifica = GETDATE() ")

            StrSQL.AppendLine(" WHERE ")
            StrSQL.AppendLine(String.Format(" MxAV_Cod = {0} ", Agro_SQL_SaveNum(codiceMisura)))
            StrSQL.AppendLine(String.Format(" AND Anag_cod = {0} ", Agro_SQL_SaveNum(codiceAnagrafica)))
            '---------------------------------------------

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function ScriviAnagrafica(codiceAnagrafica As Integer,
                                     codiceMisura As Integer,
                                     descrizione As String,
                                     valoreAnagrafica As Integer,
                                     DPI_FlagPrivatoPubblico As Integer,
                                     DPI_COD As Integer,
                                     objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.MisuraXAvversita_Anagrafiche_W.ScriviAnagrafica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" INSERT INTO MisuraXAvversita_Anagrafiche ( ")
            StrSQL.AppendLine(" MxAV_Cod ")
            StrSQL.AppendLine(" , Anag_cod ")
            StrSQL.AppendLine(" , Anag_des ")
            StrSQL.AppendLine(" , Anag_valore ")
            StrSQL.AppendLine(" , DPI_FlagPrivatoPubblico ")
            StrSQL.AppendLine(" , DPI_COD ")
            StrSQL.AppendLine(" ) VALUES ( ")

            StrSQL.AppendLine(String.Format(" {0} ", Agro_SQL_SaveNum(codiceMisura)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(codiceAnagrafica)))
            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(descrizione)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(valoreAnagrafica)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(DPI_FlagPrivatoPubblico)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(DPI_COD)))

            StrSQL.AppendLine(" ) ")
            '---------------------------------------------

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp
    End Function
End Class


