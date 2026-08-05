Imports System.Data.Entity
Imports System.Text
Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO

Public Class jDeereDataModelDAL_EntitaGias_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function ListaChiaviBoundaryDaListaChiaviField(ByVal Active As Boolean,
                                                          ByVal xFiltroAggiuntivo As String,
                                                          ByVal xOrderBy As String,
                                                          ByRef objParametri As AgronicaCoreParametri
                                                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_EntitaGias_R.ListaChiaviBoundaryDaListaChiaviField()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" Select bb.ID ")
            stb.AppendLine("     from [dbo].[jDeereDataModel_Field] ff ")
            stb.AppendLine("     inner Join [dbo].[jDeereDataModel_FieldXBoundary] fb ")
            stb.AppendLine("         On fb.IDField = ff.ID ")
            stb.AppendLine("     inner Join [dbo].[jDeereDataModel_Boundary] bb ")
            stb.AppendLine("         on bb.ID = fb.IDBoundary ")
            If Active Then
                stb.AppendLine("         and bb.active = 1 ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   ff.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   ff.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function ListaAppezzamentiGiasNonImportataInJDViaJsonPath(ByVal FiltroAppezza As String,
                                                                     ByVal xFiltroAggiuntivo As String,
                                                                     ByVal xOrderBy As String,
                                                                     ByRef objParametri As AgronicaCoreParametri
                                                                     ) As String

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_EntitaGias_R.ListaAppezzamentiGiasNonImportataInJDViaJsonPath()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" Declare @JSON nvarchar(max) ")
            stb.AppendLine("Set @JSON = (   ")

            stb.AppendLine("  Select ")
            stb.AppendLine("       null as guid ")
            stb.AppendLine("  , a.app_Nome as name --dovrà essere univoco ")
            stb.AppendLine("  , 'AGR-' + a.piva + '-' + cast(a.sa_cod as varchar(50)) + '-' +  cast(a.appezza as varchar(50)) as id ")

            stb.AppendLine("  , (  ")
            stb.AppendLine("      Select ")
            stb.AppendLine("            isnull(jDE.guid, 'AGR-' + a.piva + '-' + cast(a.sa_cod as varchar(50))) as id ")
            stb.AppendLine("          , sa.sa_nome as name --dovrà essere univoco ")
            stb.AppendLine("          , isnull(jDE.guid, 'AGR-' + a.piva + '-' + cast(a.sa_cod as varchar(50))) as guid ")
            stb.AppendLine("      From centri_Aziendali sa ")
            stb.AppendLine("          Left Join( ")
            stb.AppendLine("              select IDJD, piva, sa_cod, fr.guid ")
            stb.AppendLine("                 From jDeereDataModel_EntitaGIAS g ")
            stb.AppendLine("                  inner Join jDeereDataModel_Farm fr ")
            stb.AppendLine("                         On g.IDJd = fr.id ")
            stb.AppendLine("              where TipoJD = " & enum_TipoEntitaJohnDeere.Farm)
            stb.AppendLine("          ) jDE  ")
            stb.AppendLine("              On jDE.piva = sa.piva ")
            stb.AppendLine("              And jDE.sa_cod = sa.sa_cod ")
            stb.AppendLine("      where a.piva = sa.piva  ")
            stb.AppendLine("      And a.sa_cod = sa.sa_cod ")
            stb.AppendLine("      For json path ")
            stb.AppendLine("  ) as 'farms.values' --(centro az.) ")

            stb.AppendLine("  ,null as boundary --boundary ")
            stb.AppendLine("  ,null as client  --client ")
            stb.AppendLine(" From appezzamento a")

            stb.AppendLine(" Where Not exists( ")
            stb.AppendLine("      select 1 ")
            stb.AppendLine("         From jDeereDataModel_EntitaGIAS d ")
            stb.AppendLine("      Where d.TipoJD =  " & enum_TipoEntitaJohnDeere.Field)
            stb.AppendLine("      And d.piva = a.piva  ")
            stb.AppendLine("      And d.sa_cod = a.sa_cod ")
            stb.AppendLine("      And d.appezza = a.appezza ")
            stb.AppendLine("  )")


            If FiltroAppezza <> "" Then
                stb.Append(FiltroAppezza)
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   a.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   a.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            stb.AppendLine("FOR JSON PATH")

            stb.AppendLine(")  ")
            stb.AppendLine(" select @JSON  ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If IsDBNull(dt.Rows(0)(0)) Then
            Return "[]"
        End If

        Return dt.Rows(0)(0)

    End Function

    Public Function LeggiIDAllegato(ByVal ID As String, ByRef objParametri As AgronicaCoreParametri) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_EntitaGias_R.LeggiIDAllegato()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine("  Select  ")
            stb.AppendLine("    a.Gis_Entita_Cod as idAllegato, ")
            stb.AppendLine("    b.name, ")
            stb.AppendLine("    b.guid ")
            stb.AppendLine("  from jDeereDataModel_EntitaGIAS a inner join ")
            stb.AppendLine("       jDeereDataModel_Files b ")
            stb.AppendLine("  on (a.IDJD=b.ID and a.TipoJD=9) ")
            stb.AppendLine("  Where a.IDJD=" & Agro_SQL_SaveNum(ID))

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    Public Function ListaEntitaJDNonImportataInGIAS(ByVal TipoEntitaJD As enum_TipoEntitaJohnDeere,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreParametri
                                                    ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_EntitaGias_R.ListaEntitaJDNonImportataInGIAS()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Dim tabellaTest As String = ""

        'Account = 0
        'Organization = 1
        'Client = 2
        'Farm = 3
        'Field = 4
        'Boundary = 5
        'Assett = 6
        'Machine = 7

        Select Case TipoEntitaJD
            Case enum_TipoEntitaJohnDeere.Boundary
                tabellaTest = "jDeereDataModel_Boundary"
            Case enum_TipoEntitaJohnDeere.Account
                tabellaTest = "jDeereDataModel_Account"
            Case enum_TipoEntitaJohnDeere.Organization
                tabellaTest = "jDeereDataModel_Organization"
            Case enum_TipoEntitaJohnDeere.Client
                tabellaTest = "jDeereDataModel_Client"
            Case enum_TipoEntitaJohnDeere.Farm
                tabellaTest = "jDeereDataModel_Farm"
            Case enum_TipoEntitaJohnDeere.Field
                tabellaTest = "jDeereDataModel_Field"
            Case enum_TipoEntitaJohnDeere.Asset
                tabellaTest = "jDeereDataModel_Asset"
            Case enum_TipoEntitaJohnDeere.Machine
                tabellaTest = "jDeereDataModel_Machine"
        End Select

        Try

            stb.Length = 0

            stb.AppendLine(" Select ")
            stb.AppendLine("     b.id ")
            stb.AppendLine("                 From " & tabellaTest & " b  ")
            stb.AppendLine("     Left Join jDeereDataModel_EntitaGIAS JDE ")
            stb.AppendLine("         On b.ID = IDJD ")
            stb.AppendLine("         And JDE.TipoJD = " & TipoEntitaJD)
            stb.AppendLine("  where JDE.idJD Is null")


            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   b.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   b.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
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
    Public Function LeggiAssociazionePivaSaCodConTipoEntitaJD(ByVal tipoJD As enum_TipoEntitaJohnDeere,
                                                              ByVal piva As String,
                                                              ByVal sa_cod As Integer,
                                                              ByVal xFiltroAggiuntivo As String,
                                                              ByVal xOrderBy As String,
                                                              ByRef objParametri As AgronicaCoreParametri
                                                              ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_EntitaGias_R.LeggiAssociazionePivaSaCodConTipoEntitaJD()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        'imposto tabelle e colonne in base al tipo JD

        Dim tabellaGias As String
        Dim tabellaJd As String
        Dim colonnaGias As String

        Select Case tipoJD
            Case enum_TipoEntitaJohnDeere.Farm
                tabellaGias = "Centri_Aziendali"
                tabellaJd = "jDeereDataModel_Farm"
                colonnaGias = "sa_nome"
            Case enum_TipoEntitaJohnDeere.Client
                tabellaGias = "Imprese"
                tabellaJd = "jDeereDataModel_Client"
                colonnaGias = "rag_soc"
            Case Else
                Throw New NotImplementedException
        End Select

        Try

            stb.Length = 0

            stb.AppendLine(" Select  ")
            stb.AppendLine("       jd.ID ")
            stb.AppendLine("     , jd.Name as val_Cod ")
            stb.AppendLine("     , gg." & colonnaGias & " as Gias ")
            stb.AppendLine("     , jd.Data_Modifica as Data_Modifica_JD ")
            stb.AppendLine("     , gg.Data_Modifica as Data_Modifica_Gias ")
            stb.AppendLine(" From [dbo].[jDeereDataModel_EntitaGIAS] JDE ")
            stb.Append("     inner Join  ")
            stb.Append(tabellaGias)
            stb.Append(" gg ")

            'join su tab agronica
            Select Case tipoJD
                Case enum_TipoEntitaJohnDeere.Client
                    stb.AppendLine("         On gg.piva = JDE.piva  ")
                    stb.AppendLine("         And JDE.TipoJD = " & enum_TipoEntitaJohnDeere.Client)
                Case enum_TipoEntitaJohnDeere.Farm
                    stb.AppendLine("         On gg.piva = JDE.piva  ")
                    stb.AppendLine("         and gg.sa_cod = JDE.sa_cod  ")
                    stb.AppendLine("         And JDE.TipoJD = " & enum_TipoEntitaJohnDeere.Farm)
            End Select

            'join su tab jDeere
            Select Case tipoJD
                Case enum_TipoEntitaJohnDeere.Client
                    stb.AppendLine("     inner Join jDeereDataModel_Client JD ")
                    stb.AppendLine("         On JD.id = JDE.IDJD ")

                Case enum_TipoEntitaJohnDeere.Farm
                    stb.AppendLine("     inner Join jDeereDataModel_Farm JD ")
                    stb.AppendLine("         On JD.id = JDE.IDJD ")
            End Select


            'condizioni Where
            Select Case tipoJD
                Case enum_TipoEntitaJohnDeere.Client
                    stb.AppendLine(" where JDE.piva = '" & Agro_SQL_SaveText(piva) & "' ")

                Case enum_TipoEntitaJohnDeere.Farm
                    stb.AppendLine(" where JDE.piva = '" & Agro_SQL_SaveText(piva) & "' ")
                    stb.AppendLine(" And JDE.sa_cod = " & Agro_SQL_SaveNum(sa_cod))

            End Select


            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   JDE.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   JDE.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function LeggiIDJdDaChiaveGIAS(ByVal tipoJD As enum_TipoEntitaJohnDeere,
                                          ByVal keyStr As String,
                                          ByVal xFiltroAggiuntivo As String,
                                          ByVal xOrderBy As String,
                                          ByRef objParametri As AgronicaCoreParametri
                                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_EntitaGias_R.LeggiIDJdDaChiaveGIAS()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" Select  ")
            stb.AppendLine("       a.IDJD ")
            stb.AppendLine(" From [dbo].[jDeereDataModel_EntitaGIAS] a ")
            stb.AppendLine(" where ")
            stb.AppendLine(" a.TipoJD = " & Agro_SQL_SaveNum(tipoJD))
            If keyStr <> "" Then
                stb.AppendLine(keyStr)
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function LeggiChiaveGIASDaIDJD(ByVal tipoJD As enum_TipoEntitaJohnDeere,
                                          ByVal IDJD As Integer,
                                          ByVal xFiltroAggiuntivo As String,
                                          ByVal xOrderBy As String,
                                          ByRef objParametri As AgronicaCoreParametri
                                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_EntitaGias_R.LeggiChiaveGIASDaIDJD()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" Select  ")
            stb.AppendLine("       a.PivaSuperUser, ")
            stb.AppendLine("       a.piva, ")
            stb.AppendLine("       a.sa_cod, ")
            stb.AppendLine("       a.appezza, ")
            stb.AppendLine("       a.Gis_Entita_Cod ")
            stb.AppendLine(" From [dbo].[jDeereDataModel_EntitaGIAS] a ")
            stb.AppendLine(" where ")
            stb.AppendLine(" a.TipoJD = " & Agro_SQL_SaveNum(tipoJD))
            If IDJD <> 0 Then
                stb.AppendLine(" and a.IDJD =" & Agro_SQL_SaveNum(IDJD))
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function GetJDEntityGIASSyncro(ByVal piva As String,
                                          ByVal sa_cod As Integer,
                                          ByVal appezza As Integer,
                                          ByVal xFiltroAggiuntivo As String,
                                          ByVal xOrderBy As String,
                                          ByRef objParametri As AgronicaCoreParametri
                                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_EntitaGias_R.GetJDEntityGIASSyncro()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" select ")
            stb.AppendLine(" 	a.TipoJD, ")
            stb.AppendLine(" 	a.IDJD, ")
            stb.AppendLine(" 	b.guid As FieldUUID, ")
            stb.AppendLine(" 	b.Name as FieldName, ")
            stb.AppendLine(" 	c.guid  As FarmUUID, ")
            stb.AppendLine(" 	c.Name as FarmName, ")
            stb.AppendLine(" 	d.guid As ClientUUID, ")
            stb.AppendLine(" 	d.Name as ClientName ")
            stb.AppendLine(" from ")
            stb.AppendLine(" 	jDeereDataModel_EntitaGIAS a ")
            stb.AppendLine(" 	left join jDeereDataModel_Field b ")
            stb.AppendLine("    on (a.IDJD=b.ID) ")
            stb.AppendLine("    Left Join jDeereDataModel_Farm c ")
            stb.AppendLine("    On (b.FarmID=c.ID) ")
            stb.AppendLine("    Left Join jDeereDataModel_Client d ")
            stb.AppendLine("    On (b.ClientID=d.ID) ")
            stb.AppendLine(" where ")
            stb.AppendLine(" 	a.PivaSuperUser ='" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' and ")
            stb.AppendLine(" 	a.Gis_Entita_Cod=0 and ")
            stb.AppendLine(" 	a.piva='" & Agro_SQL_SaveText(piva) & "' and ")
            stb.AppendLine(" 	a.sa_Cod=" & Agro_SQL_SaveNum(sa_cod) & " and ")
            stb.AppendLine(" 	a.appezza=" & Agro_SQL_SaveNum(appezza) & " ")

            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

End Class



Public Class jDeereDataModelDAL_EntitaGIAS_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function RiportaAssociazioniMancanti(ByVal TipoEntitaJD As enum_TipoEntitaJohnDeere,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByRef objParametri As AgronicaCoreParametri
                                                ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_EntitaGIAS_W.RiportaAssociazioniMancanti()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            stb.Length = 0

            Dim tabella As String = ""
            Dim critExclude As String = ""

            stb.AppendLine("  insert jDeereDataModel_EntitaGIAS( ")
            stb.AppendLine("    [jDeereDataModel_EntitaGIAS_COD] ")
            stb.AppendLine("  , [TipoJD] ")
            stb.AppendLine("  , [IDJD] ")
            stb.AppendLine("  , [PivaSuperUser] ")
            stb.AppendLine("  , [Gis_Entita_Cod] ")
            stb.AppendLine("  , [piva] ")
            stb.AppendLine("  , [sa_cod] ")
            stb.AppendLine("  , [appezza] ")
            stb.AppendLine("  , [inviato], [datainvio], [Data_Creazione], [Data_Modifica], [Username_Creazione], [Username_Modifica], [Validita_Inizio], [Validita_Fine]) ")
            stb.AppendLine(" Select ")
            stb.AppendLine("       DENSE_RANK() over (order by id) + isnull((Select max(ultimo_Valore)  from sequenza_tabelle where nome_Tabella = 'jDeereDataModel_EntitaGIAS'), 1)  [jDeereDataModel_EntitaGIAS_COD] ")
            stb.AppendLine("  , " & TipoEntitaJD & " [TipoJD] ")
            stb.AppendLine("  , ID as [IDJD] ")
            stb.AppendLine("  , '" & objParametri.PivaSuperUser & "' as [PivaSuperUser] ")

            Select Case TipoEntitaJD
                Case enum_TipoEntitaJohnDeere.Field
                    stb.AppendLine("  , 0 as [Gis_Entita_Cod] ")
                    stb.AppendLine("  , (select strName from dbo.fSplit(a.guid, '-') where code = 2) as [piva] ")
                    stb.AppendLine("  , (select strName from dbo.fSplit(a.guid, '-') where code = 3) as [Sa_cod] ")
                    stb.AppendLine("  , (select strName from dbo.fSplit(a.guid, '-') where code = 4) as [Appezza] ")
                    tabella = "jDeereDataModel_Field"

                    'lorenzo - criterio di esclusione dei record già presenti
                    critExclude = " and ('" & CType(TipoEntitaJD, Integer).ToString() & "'+'-'+cast(a.ID as nvarchar)+'-'+'" & objParametri.PivaSuperUser & "'+'-'+'0'+'-'+(select strName from dbo.fSplit(a.guid, '-') where code = 2)+'-'+(select strName from dbo.fSplit(a.guid, '-') where code = 3)+'-'+(select strName from dbo.fSplit(a.guid, '-') where code = 4)) collate database_default not in "
                    critExclude &= "(select cast(b.TipoJD as nvarchar)+'-'+cast(b.IDJD as nvarchar)+'-'+b.PivaSuperUser+'-'+cast(b.Gis_Entita_Cod as nvarchar)+'-'+b.piva+'-'+cast(b.sa_cod as nvarchar)+'-'+cast(b.appezza as nvarchar) from jDeereDataModel_EntitaGIAS b "
                    critExclude &= "Where TipoJD=" & CType(TipoEntitaJD, Integer).ToString() & " )"

                Case enum_TipoEntitaJohnDeere.Client
                    stb.AppendLine("  , 0 as [Gis_Entita_Cod] ")
                    stb.AppendLine("  , (select strName from dbo.fSplit(a.guid, '-') where code = 2) as [piva] ")
                    stb.AppendLine("  , (select strName from dbo.fSplit(a.guid, '-') where code = 3) as [Sa_cod] ")
                    stb.AppendLine("  , 0 as [Appezza] ")
                    tabella = "jDeereDataModel_Client"

                    'lorenzo - criterio di esclusione dei record già presenti
                    critExclude = " and ('" & CType(TipoEntitaJD, Integer).ToString() & "'+'-'+cast(a.ID as nvarchar)+'-'+'" & objParametri.PivaSuperUser & "'+'-'+'0'+'-'+(select strName from dbo.fSplit(a.guid, '-') where code = 2)+'-'+(select strName from dbo.fSplit(a.guid, '-') where code = 3)+'-0') collate database_default not in "
                    critExclude &= "(select cast(b.TipoJD as nvarchar)+'-'+cast(b.IDJD as nvarchar)+'-'+b.PivaSuperUser+'-'+cast(b.Gis_Entita_Cod as nvarchar)+'-'+b.piva+'-'+cast(b.sa_cod as nvarchar)+'-'+cast(b.appezza as nvarchar) from jDeereDataModel_EntitaGIAS b "
                    critExclude &= "Where TipoJD=" & CType(TipoEntitaJD, Integer).ToString() & " )"

                Case enum_TipoEntitaJohnDeere.Farm
                    stb.AppendLine("  , 0 as [Gis_Entita_Cod] ")
                    stb.AppendLine("  , (select strName from dbo.fSplit(a.guid, '-') where code = 2) as [piva] ")
                    stb.AppendLine("  , (select strName from dbo.fSplit(a.guid, '-') where code = 3) as [Sa_cod] ")
                    stb.AppendLine("  , 0 as [Appezza] ")
                    tabella = "jDeereDataModel_Farm"

                    'lorenzo - criterio di esclusione dei record già presenti
                    critExclude = " and ('" & CType(TipoEntitaJD, Integer).ToString() & "'+'-'+cast(a.ID as nvarchar)+'-'+'" & objParametri.PivaSuperUser & "'+'-'+'0'+'-'+(select strName from dbo.fSplit(a.guid, '-') where code = 2)+'-'+(select strName from dbo.fSplit(a.guid, '-') where code = 3)+'-0') collate database_default not in "
                    critExclude += "(select cast(b.TipoJD as nvarchar)+'-'+cast(b.IDJD as nvarchar)+'-'+b.PivaSuperUser+'-'+cast(b.Gis_Entita_Cod as nvarchar)+'-'+b.piva+'-'+cast(b.sa_cod as nvarchar)+'-'+cast(b.appezza as nvarchar) from jDeereDataModel_EntitaGIAS b "
                    critExclude += "Where TipoJD=" & CType(TipoEntitaJD, Integer).ToString() & " )"

                Case enum_TipoEntitaJohnDeere.Boundary
                    stb.AppendLine("  , (select strName from dbo.fSplit(a.guid, '-') where code = 2) as [Gis_Entita_Cod] ")
                    stb.AppendLine("  , '' as [piva] ")
                    stb.AppendLine("  , 0 as [Sa_cod] ")
                    stb.AppendLine("  , 0 as [Appezza] ")
                    tabella = "jDeereDataModel_Boundary"

                    'lorenzo - criterio di esclusione dei record già presenti
                    critExclude = " and ('" & CType(TipoEntitaJD, Integer).ToString() & "'+'-'+cast(a.ID as nvarchar)+'-'+'" & objParametri.PivaSuperUser & "'+'-'+(select strName from dbo.fSplit(a.guid, '-') where code = 2)+'--0-0') collate database_default not in "
                    critExclude &= "(select cast(b.TipoJD as nvarchar)+'-'+cast(b.IDJD as nvarchar)+'-'+b.PivaSuperUser+'-'+cast(b.Gis_Entita_Cod as nvarchar)+'-'+b.piva+'-'+cast(b.sa_cod as nvarchar)+'-'+cast(b.appezza as nvarchar) from jDeereDataModel_EntitaGIAS b "
                    critExclude &= "Where TipoJD=" & CType(TipoEntitaJD, Integer).ToString() & " )"
            End Select

            stb.AppendLine("  , 0 as inviato ")
            stb.AppendLine("  , null as dataInvio ")
            stb.AppendLine("  , getdate() as Data_Creazione ")
            stb.AppendLine("  , getdate() as Data_Modifica ")
            stb.AppendLine("  , '' as username_Creazione ")
            stb.AppendLine("  , '' as username_modifica ")
            stb.AppendLine("  , '01/01/1900' as validita_inizio ")
            stb.AppendLine("  , '31/12/2100' as validita_fine ")
            stb.AppendLine(" From " & tabella & " a ")
            stb.AppendLine(" Where Guid Like 'AGR-%'")
            If critExclude <> "" Then
                stb.AppendLine(critExclude)
            End If

            If Not String.IsNullOrEmpty(xFiltroAggiuntivo) Then
                stb.AppendLine("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function AssociaAppezzamentoInGisEntitaDaJoinBoundaryJD(ByVal xFiltroAggiuntivo As String,
                                                                   ByRef objParametri As AgronicaCoreParametri
                                                                   ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_EntitaGIAS_W.AssociaAppezzamentoDaJoinBoundaryJD()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            stb.Length = 0
            stb.AppendLine(" update e    ")
            stb.AppendLine("     set appezza = app.appezza     ")
            stb.AppendLine("     , TipoEntita_Cod = 1     ")
            stb.AppendLine(" From [dbo].[jDeereDataModel_EntitaGIAS] app ")
            stb.AppendLine("     inner Join [dbo].[jDeereDataModel_Field] ff ")
            stb.AppendLine("         On ff.ID = app.idJd ")
            stb.AppendLine("         And app.TipoJD = " & enum_TipoEntitaJohnDeere.Field)
            stb.AppendLine("     inner Join [dbo].[jDeereDataModel_FieldXBoundary] fb ")
            stb.AppendLine("         On fb.IDField = ff.ID ")
            stb.AppendLine("     inner Join [dbo].[jDeereDataModel_Boundary] bb ")
            stb.AppendLine("         On bb.active = 1 ")
            stb.AppendLine("         And bb.ID = fb.IDBoundary ")
            stb.AppendLine("     inner join  [dbo].[jDeereDataModel_EntitaGIAS] gis ")
            stb.AppendLine("         On gis.IDJD = bb.id ")
            stb.AppendLine("         And gis.TipoJD =  " & enum_TipoEntitaJohnDeere.Boundary)
            stb.AppendLine("     inner Join gis_elementiGrafici gg ")
            stb.AppendLine("         On  gg.Entita_cod = gis.Gis_Entita_Cod ")
            stb.AppendLine("     inner Join GIS_Entita e ")
            stb.AppendLine("         On e.Entita_Cod = gg.entita_cod")
            stb.AppendLine(" WHERE e.TipoEntita_Cod = " & enum_Gis_LayerElementiGrafici_std.INVISIBILE)

            If Not String.IsNullOrEmpty(xFiltroAggiuntivo) Then
                stb.AppendLine("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="gisTipoEntita_cod">validi: APPEZZAMENTI</param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    Public Function RiportaSuperficieAnagGIASDaTabellaPadreDoveMancante(ByVal gisTipoEntita_cod As enum_GIS2012_TipoEntita,
                                                                        ByRef objParametri As AgronicaCoreParametri
                                                                        ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_EntitaGIAS_W.RiportaSuperficieAnagGIASDaTabellaPadreDoveMancante()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim xRisp As Boolean = False


        Select Case gisTipoEntita_cod
            Case enum_GIS2012_TipoEntita.APPEZZAMENTI
            Case Else
                Throw New Exception("Chiamata non valida")
        End Select

        Try

            stb.Length = 0
            stb.AppendLine(" update e ")
            stb.AppendLine("     set Sup_Imp = a.SUP_APP ")
            stb.AppendLine(" From reg_impianti e ")
            stb.AppendLine("     inner Join Appezzamento a ")
            stb.AppendLine("         On a.piva = e.piva ")
            stb.AppendLine("         And a.SA_COD = e.Sa_Cod ")
            stb.AppendLine("         And a.APPEZZA = e.Appezza ")
            stb.AppendLine(" where e.Sup_Imp = 0 ")
            stb.AppendLine(" And a.SUP_APP <> 0  ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="gisTipoEntita_cod">validi: APPEZZAMENTI, Impianto_Nudo</param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    Public Function RiportaSuperficieAnagGIASDaGisDoveMancante(ByVal gisTipoEntita_cod As enum_GIS2012_TipoEntita,
                                                               ByRef objParametri As AgronicaCoreParametri
                                                               ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_EntitaGIAS_W.RiportaSuperficieAnagGIASDaGisDoveMancante()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim xRisp As Boolean = False

        Dim tabella As String = ""
        Dim colonna As String = ""

        Select Case gisTipoEntita_cod
            Case enum_GIS2012_TipoEntita.APPEZZAMENTI
                tabella = "Appezzamento"
                colonna = "sup_app"
            Case enum_GIS2012_TipoEntita.IMPIANTO_NUDO
                tabella = "Reg_Impianti"
                colonna = "Sup_imp"
            Case Else
                Throw New Exception("Chiamata non valida")
        End Select

        Try

            stb.Length = 0
            stb.AppendLine(" update a ")
            stb.AppendLine("     set " & colonna & " = round(Poligono_GeoEntity.STArea() /10000, 5) ")
            stb.AppendLine(" From gis_elementiGrafici gg ")
            stb.AppendLine("     inner Join GIS_Entita e ")
            stb.AppendLine("         On e.entita_cod = gg.Entita_Cod ")
            stb.AppendLine("     inner Join " & tabella & " a ")
            stb.AppendLine("         On a.piva = e.piva ")
            stb.AppendLine("         And a.SA_COD = e.Sa_Cod ")
            stb.AppendLine("         And a.APPEZZA = e.Appezza ")

            If gisTipoEntita_cod = enum_GIS2012_TipoEntita.IMPIANTO_NUDO Then
                stb.AppendLine("         And a.id_reg = e.id_imp ")
            End If

            stb.AppendLine(" where e.TipoEntita_Cod = " & gisTipoEntita_cod)
            stb.AppendLine(" And a.SUP_APP = 0  ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function AssociaLayerAppezzamentoInGisDoveMancante(ByRef objParametri As AgronicaCoreParametri
                                                              ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_EntitaGIAS_W.AssociaLayerAppezzamentoInGisDoveMancante()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            stb.Length = 0
            stb.AppendLine(" update gg    ")
            stb.AppendLine("     set layerElementiGrafici_Cod = 1     ")
            stb.AppendLine("     from gis_elementiGrafici gg ")
            stb.AppendLine("     inner Join GIS_Entita e ")
            stb.AppendLine("         On e.Entita_Cod = gg.entita_cod")
            stb.AppendLine(" where e.TipoEntita_cod = 1 ")
            stb.AppendLine(" and gg.layerElementiGrafici_Cod = 11 ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    '########################################################################################
    Public Function ScriviAssociazioneGIASJD(ByVal TipoEntitaCodJD As enum_TipoEntitaJohnDeere,
                                             ByVal jDeereDataModel_EntitaGIAS_COD As Integer,
                                             ByVal piva As String,
                                             ByVal sa_cod As Integer,
                                             ByVal appezza As Integer,
                                             ByVal GISEntita_Cod As Integer,
                                             ByVal IDJD As Integer,
                                             ByVal Validita_Inizio As Date,
                                             ByVal Validita_Fine As Date,
                                             ByRef objParametri As AgronicaCoreParametri,
                                             Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                                             Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                                             Optional ByVal username_creazione As String = "",
                                             Optional ByVal username_modifica As String = "",
                                             Optional ByVal Validazione As Integer = 0,
                                             Optional ByVal Data_Validazione As DateTime = #2/1/1900#,
                                             Optional ByVal UserName_Validazione As String = ""
                                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_EntitaGIAS_W.ScriviAssociazioneGIASJD()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        If Data_creazione = #2/1/1900# Then
            Data_creazione = Now
        End If

        If Data_modifica = #2/1/1900# Then
            Data_modifica = Now
        End If

        If username_creazione = "" Then
            username_creazione = objParametri.UsernameOperazione
        End If

        If username_modifica = "" Then
            username_modifica = objParametri.UsernameOperazione
        End If

        If Data_Validazione = #2/1/1900# Then
            Data_Validazione = Now
        End If

        Try

            strSql.Length = 0
            strSql.AppendLine("INSERT INTO jDeereDataModel_EntitaGIAS ( ")
            strSql.AppendLine("                    [jDeereDataModel_EntitaGIAS_COD],      ")
            strSql.AppendLine("                    [TipoJD],    ")
            strSql.AppendLine("                    [IDJD],    ")
            strSql.AppendLine("                    [PivaSuperUser],   [Gis_Entita_Cod], ")
            strSql.AppendLine("                    [Piva],   sa_cod,  appezza, ")

            strSql.AppendLine("                    Inviato, DataInvio, ")
            strSql.AppendLine("                    Data_Creazione,     Data_Modifica, ")
            strSql.AppendLine("                    UserName_Creazione, UserName_Modifica, ")
            strSql.AppendLine("                    Validita_Inizio,    Validita_Fine ")
            strSql.AppendLine("                    ) ")
            strSql.AppendLine("VALUES (")
            strSql.AppendLine("           " & Agro_SQL_SaveNum(jDeereDataModel_EntitaGIAS_COD) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(TipoEntitaCodJD) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(IDJD) & "  ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(GISEntita_Cod) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(piva) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(sa_cod) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(appezza) & " ")

            strSql.AppendLine("         , 0  ")
            strSql.AppendLine("         , Null  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            strSql.AppendLine(")")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function Aggiorna_jDeereDataModel_EntitaGIAS(ByVal EFArrayToInsert As ArrayList,
                                                        ByVal EFArrayToUpdate As ArrayList,
                                                        ByVal EFArrayToDelete As ArrayList,
                                                        ByRef objParametri As AgronicaCoreParametri
                                                        ) As String

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_EntitaGIAS_W.Aggiorna_jDeereDataModel_EntitaGIAS()"

        Dim messaggioErrore As String = ""

        Dim pivaSuperUser = objParametri.PivaSuperUser

        Dim objSequenze = New Agro_Sequenze

        Dim retries As Integer = 3
        Dim success As Boolean = True

        Dim gefutils As New Gias_EF_Utility
        Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Using scope As New TransactionScope()

                Using giasContext As New Gias_DeveloperServer_Entities(efConnString)

                    Dim idSeq As Integer = 0

                    For Each curjDeereDataModel_EntitaGIAS As jDeereDataModel_EntitaGIAS In EFArrayToInsert

                        success = False
                        For i As Integer = 0 To retries - 1

                            Try

                                'Richiedo un nuovo id sequenza
                                idSeq = objSequenze.NuovoId_Tabella_EF(giasContext,
                                               "jDeereDataModel_EntitaGIAS", 0, 2000000000, objParametri)

                                curjDeereDataModel_EntitaGIAS.jDeereDataModel_EntitaGIAS_COD = idSeq
                                giasContext.jDeereDataModel_EntitaGIAS.Add(curjDeereDataModel_EntitaGIAS)
                                giasContext.SaveChanges()
                                success = True

                                Exit For
                            Catch ex As Exception
                                Threading.Thread.Sleep(500) ' 500 milliseconds = 0.5 seconds
                            End Try
                        Next
                        ' Al primo errore evito di continuare le modifiche
                        If Not success Then
                            messaggioErrore = "Non sono riuscito ad aggiornare i dati dopo " & retries & " tentativi."
                            Exit For
                        End If
                    Next

                    If success Then
                        For Each listFattVar As jDeereDataModel_EntitaGIAS In EFArrayToUpdate
                            giasContext.jDeereDataModel_EntitaGIAS.Attach(listFattVar)
                            giasContext.Entry(listFattVar).State = EntityState.Modified
                            giasContext.SaveChanges()
                        Next

                        For Each listFattVar As jDeereDataModel_EntitaGIAS In EFArrayToDelete
                            giasContext.jDeereDataModel_EntitaGIAS.Attach(listFattVar)
                            giasContext.jDeereDataModel_EntitaGIAS.Remove(listFattVar)
                            giasContext.SaveChanges()
                        Next

                        ' COMMIT Effettivo
                        scope.Complete()
                    End If

                End Using

            End Using

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore, ex)

        End Try

        Return messaggioErrore

    End Function

End Class
