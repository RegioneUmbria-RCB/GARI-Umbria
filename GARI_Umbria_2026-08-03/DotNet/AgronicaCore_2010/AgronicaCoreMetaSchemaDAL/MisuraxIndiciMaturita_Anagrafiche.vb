Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class MisuraxIndiciMaturita_Anagrafiche_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(
        ByVal Ind_Mat_Cod As Integer,
        ByVal udm_cod As Integer,
        ByVal veg_cod As Integer,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreParametri
    ) As DataTable

        Const nomeRoutine = "MisuraxIndiciMaturita_Anagrafiche_R.Leggi()"
        Dim stb As New Text.StringBuilder With {.Length = 0}
        Dim dt As DataTable
        Try
            stb.AppendLine(" select  ")
            stb.AppendLine(" SpecieVegetali.Veg_Des, ")
            stb.AppendLine(" MisuraxIndiciMaturita.IND_MAT_COD, IndiciMaturita.IND_MAT_DES, ")
            stb.AppendLine(" MisuraxIndiciMaturita.UDM_COD, UnitaMisura.UDM_DES, ")
            stb.AppendLine(" MisuraXIndiciMaturita_Anagrafiche.Anag_des, MisuraXIndiciMaturita_Anagrafiche.Anag_valore ")
            stb.AppendLine(" from IndiciMaturitaxSpecieVegetali ")
            stb.AppendLine(" left join  MisuraxIndiciMaturita on IndiciMaturitaxSpecieVegetali.IND_MAT_COD = MisuraxIndiciMaturita.IND_MAT_COD ")
            stb.AppendLine(" left join MisuraXIndiciMaturita_Anagrafiche on MisuraxIndiciMaturita.IND_MAT_COD = MisuraXIndiciMaturita_Anagrafiche.Ind_Mat_Cod and MisuraxIndiciMaturita.UDM_COD = MisuraXIndiciMaturita_Anagrafiche.Udm_Cod ")
            stb.AppendLine(" left join IndiciMaturita on MisuraxIndiciMaturita.IND_MAT_COD = IndiciMaturita.IND_MAT_COD ")
            stb.AppendLine(" left join UnitaMisura on MisuraxIndiciMaturita.UDM_COD = UnitaMisura.UDM_COD ")
            stb.AppendLine(" left join SpecieVegetali on SpecieVegetali.Veg_Cod = IndiciMaturitaxSpecieVegetali.VEG_COD ")
            stb.AppendLine(" where 1=1 ")
            stb.AppendLine("   and MisuraXIndiciMaturita_Anagrafiche.Anag_valore > 0 ")

            If Ind_Mat_Cod <> 0 Then
                stb.AppendLine("   AND MisuraxIndiciMaturita.Ind_Mat_Cod = " & Agro_SQL_SaveNum(Ind_Mat_Cod))
            End If
            If veg_cod <> 0 Then
                stb.AppendLine("   AND SpecieVegetali.Veg_Cod = " & Agro_SQL_SaveNum(veg_cod))
            End If
            If udm_cod <> 0 Then
                stb.AppendLine("   AND MisuraxIndiciMaturita.udm_Cod = " & Agro_SQL_SaveNum(udm_cod))
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    stb.AppendLine(" AND   MisuraXIndiciMaturita_Anagrafiche.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    stb.AppendLine(" AND   MisuraXIndiciMaturita_Anagrafiche.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                stb.AppendLine(" order by SpecieVegetali.Veg_Des ")
            End If
            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
        Return dt
    End Function

    Public Function Leggi_SenzaSpecie(ByVal IND_MAT_COD As Integer,
                                      ByVal UDM_COD As Integer,
                                      ByRef objParametri As AgronicaCoreParametri
                                      ) As DataTable


        Const nomeRoutine = "MisuraxIndiciMaturita_Anagrafiche_R.Leggi_SenzaSpecie()"
        Dim stb As New Text.StringBuilder With {.Length = 0}
        Dim dt As DataTable

        Try
            stb.AppendLine(" SELECT  ")
            stb.AppendLine("      IndiciMaturitaxSpecieVegetali.IND_MAT_COD ")
            stb.AppendLine("    , IndiciMaturita.IND_MAT_DES ")
            stb.AppendLine("    , MisuraxIndiciMaturita.UDM_COD, UnitaMisura.UDM_DES, UnitaMisura.UDM_SIM ")

            stb.AppendLine(" , MisuraXIndiciMaturita_Anagrafiche.Anag_des, MisuraXIndiciMaturita_Anagrafiche.Anag_valore ")

            stb.AppendLine("    , 0 AS Veg_Cod ")
            stb.AppendLine("    , '' AS Veg_Des ")

            stb.AppendLine(" FROM IndiciMaturitaxSpecieVegetali ")
            stb.AppendLine(" LEFT JOIN MisuraxIndiciMaturita ON ")
            stb.AppendLine("     MisuraxIndiciMaturita.IND_MAT_COD = IndiciMaturitaxSpecieVegetali.IND_MAT_COD ")

            stb.AppendLine(" LEFT JOIN MisuraXIndiciMaturita_Anagrafiche ON ")
            stb.AppendLine("     MisuraxIndiciMaturita.IND_MAT_COD = MisuraXIndiciMaturita_Anagrafiche.Ind_Mat_Cod  ")
            stb.AppendLine(" AND MisuraxIndiciMaturita.UDM_COD = MisuraXIndiciMaturita_Anagrafiche.Udm_Cod ")

            stb.AppendLine(" LEFT JOIN IndiciMaturita ON ")
            stb.AppendLine("     IndiciMaturitaxSpecieVegetali.IND_MAT_COD = IndiciMaturita.IND_MAT_COD ")

            stb.AppendLine(" LEFT JOIN UnitaMisura ON ")
            stb.AppendLine("    MisuraxIndiciMaturita.UDM_COD = UnitaMisura.UDM_COD ")

            stb.AppendLine(" WHERE 1 = 1 ")
            stb.AppendLine(" AND MisuraXIndiciMaturita_Anagrafiche.Anag_valore > 0 ")
            stb.AppendLine(" AND IndiciMaturitaxSpecieVegetali.VEG_COD = 0 ")

            If IND_MAT_COD <> 0 Then
                stb.AppendLine("   AND MisuraxIndiciMaturita.Ind_Mat_Cod = " & Agro_SQL_SaveNum(IND_MAT_COD))
            End If

            If UDM_COD <> 0 Then
                stb.AppendLine("   AND MisuraxIndiciMaturita.udm_Cod = " & Agro_SQL_SaveNum(UDM_COD))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    stb.AppendLine(" AND   MisuraXIndiciMaturita_Anagrafiche.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    stb.AppendLine(" AND   MisuraXIndiciMaturita_Anagrafiche.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return dt

    End Function

    Public Function LeggiAnagrafiche(ByVal indMatCod As Integer, ByVal udmCod As Integer, ByVal anagCod As Integer, ByRef objParametriServer As AgronicaCoreParametri) As DataTable
        Const nomeRoutine = NameOf(LeggiAnagrafiche)
        Dim stb As New Text.StringBuilder With {.Length = 0}
        Dim dt As DataTable

        Try
            stb.AppendLine(" SELECT")
            stb.AppendLine("        anag.IND_MAT_COD, im.IND_MAT_DES, anag.Udm_Cod, um.UDM_DES, anag.Anag_Cod, anag.Anag_des, anag.Anag_valore")
            stb.AppendLine(" FROM")
            stb.AppendLine("        MisuraXIndiciMaturita_Anagrafiche anag")
            stb.AppendLine(" INNER JOIN")
            stb.AppendLine("        IndiciMaturita im")
            stb.AppendLine("        ON im.IND_MAT_COD = anag.IND_MAT_COD")
            stb.AppendLine(" INNER JOIN")
            stb.AppendLine("        UnitaMisura um")
            stb.AppendLine("        ON um.UDM_COD = anag.Udm_Cod")
            stb.AppendLine(" WHERE")
            stb.AppendLine("        1 = 1")
            If indMatCod <> -1 Then
                stb.AppendLine("        AND anag.Ind_Mat_Cod = " & Agro_SQL_SaveNum(indMatCod) & " ")
            End If
            If udmCod <> -1 Then
                stb.AppendLine("        AND anag.Udm_Cod = " & Agro_SQL_SaveNum(udmCod) & " ")
            End If
            If anagCod <> 0 Then
                stb.AppendLine("        AND anag.Anag_Cod = " & Agro_SQL_SaveNum(anagCod) & " ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametriServer, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception

            Scrivi_LOG(objParametriServer, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return dt

    End Function

    ''' <summary>
    ''' Verifica se almeno una delle anagrafiche con questo ind_mat_cod e udm_cod è stato usato
    ''' </summary>
    ''' <param name="indMatCod"></param>
    ''' <param name="udmCod"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    Public Function VerificaUtilizzoAnagrafica(indMatCod As Integer, udmCod As Integer, objParametri As AgronicaCoreParametri) As Boolean
        Dim nomeRoutine As String = $"AgronicaCoreMetaSchemaDAL.{NameOf(MisuraxIndiciMaturita_Anagrafiche_R)}.{NameOf(VerificaUtilizzoAnagrafica)}"

        Dim messaggioErrore As String = ""
        Dim sb As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            sb.Length = 0

            sb.AppendLine(" SELECT ")
            sb.AppendLine("            1 esiste ")
            sb.AppendLine(" FROM ")
            sb.AppendLine("            Mov_Destinazioni dest")
            sb.AppendLine(" INNER JOIN")
            sb.AppendLine("            Agenda a")
            sb.AppendLine("            ON a.PIVA = dest.PIVA")
            sb.AppendLine("            AND a.Sa_Cod = dest.Sa_Cod")
            sb.AppendLine("            AND a.Id_Agenda = dest.Id_Agenda")
            sb.AppendLine(" INNER JOIN")
            sb.AppendLine("            Movimenti_dettagli d")
            sb.AppendLine("            ON dest.tipo_Destinazione = 0  ")
            sb.AppendLine("            AND dest.id_mov_det = d.id_mov_det ")
            sb.AppendLine(" INNER JOIN ")
            sb.AppendLine("            Mov_Dettaglio_Tecnico tec ")
            sb.AppendLine("            ON tec.id_mov_det = d.id_mov_det ")
            sb.AppendLine(" INNER JOIN ")
            sb.AppendLine("            MisuraXIndiciMaturita mi ")
            sb.AppendLine("            ON mi.UDM_COD = tec.Dett_Cod ")
            sb.AppendLine("            AND mi.IND_MAT_COD = tec.FF_Classe")
            sb.AppendLine(" INNER JOIN ")
            sb.AppendLine("            MisuraXIndiciMaturita_Anagrafiche anag")
            sb.AppendLine("            ON anag.UDM_COD = mi.UDM_COD")
            sb.AppendLine("            AND anag.IND_MAT_COD = mi.IND_MAT_COD")
            sb.AppendLine(" WHERE ")
            sb.AppendLine("            a.Lav_Cod = " + Agro_SQL_SaveNum(CostantiPersonalizzate.LAVCOD_RILIEVO_INDICI_MATURITA) + " ")
            sb.AppendLine("            AND anag.IND_MAT_COD = " + Agro_SQL_SaveNum(indMatCod) + " ")
            sb.AppendLine("            AND anag.UDM_COD = " + Agro_SQL_SaveNum(udmCod) + " ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, sb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return (dt IsNot Nothing AndAlso dt.Rows.Count > 0)

    End Function

End Class

Public Class MisuraxIndiciMaturita_Anagrafiche_W
    Inherits DataProvider

    Public Function EliminaAnagrafica(Ind_Mat_Cod As Integer,
                                      Udm_Cod As Integer,
                                      Anag_Cod As Integer,
                                      objParametriServer As AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = $"AgronicaCoreMetaSchemaDAL.{NameOf(MisuraxIndiciMaturita_Anagrafiche_W)}.{NameOf(EliminaAnagrafica)}"

        Dim messaggioErrore As String = ""
        Dim sb As New StringBuilder
        Dim response As Boolean = False

        Try
            sb.Length = 0

            sb.AppendLine(" DELETE FROM")
            sb.AppendLine("         MisuraXIndiciMaturita_Anagrafiche ")
            sb.AppendLine(" WHERE")
            sb.AppendLine("         Ind_Mat_Cod = " + Agro_SQL_SaveNum(Ind_Mat_Cod) + " ")
            sb.AppendLine("         AND Udm_Cod = " + Agro_SQL_SaveNum(Udm_Cod) + " ")
            sb.AppendLine("         AND Anag_Cod = " + Agro_SQL_SaveNum(Anag_Cod) + " ")

            response = EseguiQuery_Scrittura(objParametriServer, sb.ToString, nomeRoutine)

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            response = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return response

    End Function

    Public Function AggiornaAnagrafica(Ind_Mat_Cod As Integer,
                                       Udm_Cod As Integer,
                                       Anag_Cod As Integer,
                                       Anag_Des As String,
                                       Anag_Valore As Integer,
                                       objParametriServer As AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = $"AgronicaCoreMetaSchemaDAL.{NameOf(MisuraxIndiciMaturita_Anagrafiche_W)}.{NameOf(AggiornaAnagrafica)}"

        Dim messaggioErrore As String = ""
        Dim sb As New StringBuilder
        Dim response As Boolean = False

        Try
            sb.Length = 0

            sb.AppendLine(" UPDATE ")
            sb.AppendLine("         MisuraXIndiciMaturita_Anagrafiche ")
            sb.AppendLine(" SET ")
            sb.AppendLine("         Anag_des = '" + Agro_SQL_SaveText(Anag_Des) + "', ")
            sb.AppendLine("         Anag_valore = " + Agro_SQL_SaveNum(Anag_Valore) + ", ")
            sb.AppendLine("         Username_Modifica = '" + Agro_SQL_SaveText(objParametriServer.UsernameOperazione) + "', ")
            sb.AppendLine("         Data_Modifica = GETDATE()")
            sb.AppendLine(" WHERE ")
            sb.AppendLine("         Ind_Mat_Cod = " + Agro_SQL_SaveNum(Ind_Mat_Cod) + " ")
            sb.AppendLine("         AND Udm_Cod = " + Agro_SQL_SaveNum(Udm_Cod) + " ")
            sb.AppendLine("         AND Anag_Cod = " + Agro_SQL_SaveNum(Anag_Cod) + " ")

            response = EseguiQuery_Scrittura(objParametriServer, sb.ToString, nomeRoutine)

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            response = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return response

    End Function

    Public Function ScriviAnagrafica(Ind_Mat_Cod As Integer,
                                     Udm_Cod As Integer,
                                     Anag_Cod As Integer,
                                     Anag_Des As String,
                                     Anag_Valore As Integer,
                                     objParametriServer As AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = $"AgronicaCoreMetaSchemaDAL.{NameOf(MisuraxIndiciMaturita_Anagrafiche_W)}.{NameOf(ScriviAnagrafica)}"

        Dim messaggioErrore As String = ""
        Dim sb As New StringBuilder
        Dim response As Boolean = False

        Try
            sb.Length = 0
            sb.AppendLine(" INSERT INTO")
            sb.AppendLine("         MisuraXIndiciMaturita_Anagrafiche")
            sb.AppendLine("         ( Ind_Mat_Cod, Udm_Cod, Anag_Cod,")
            sb.AppendLine("         Anag_Des, Anag_Valore )")
            sb.AppendLine(" VALUES (")
            sb.AppendLine(String.Format("{0}", Agro_SQL_SaveNum(Ind_Mat_Cod)))
            sb.AppendLine(String.Format(", {0}", Agro_SQL_SaveNum(Udm_Cod)))
            sb.AppendLine(String.Format(", {0}", Agro_SQL_SaveNum(Anag_Cod)))
            sb.AppendLine(String.Format(", '{0}'", Agro_SQL_SaveText(Anag_Des)))
            sb.AppendLine(String.Format(", {0}", Agro_SQL_SaveNum(Anag_Valore)))

            sb.AppendLine(" )")

            response = EseguiQuery_Scrittura(objParametriServer, sb.ToString, nomeRoutine)

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            response = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return response

    End Function

End Class
