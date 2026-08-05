Imports System.Text
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.DataProviderExtensions


Public Class G2GLocal_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiConfigurazioni(
                                ByVal G2GLocalConfigurazioni_COD As Integer,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                ByVal Optional G2GLocalConfigurazioni_DES As String = ""
    ) As DataTable
        Dim dt As DataTable
        Dim stb As New StringBuilder
        Dim NomeRoutine As String = "AgronicaCoreG2GLocalDal.G2GLocal_R.LeggiConfigurazioni"
        Dim MessaggioErrore As String = ""
        Try

            stb.Append("        select * " & vbCrLf)
            ' Condizione sempre vera per poter concatenare condizioni con "and"
            stb.Append(" from G2GLocalConfigurazioni where 1 = 1" & vbCrLf)

            If G2GLocalConfigurazioni_COD <> 0 Then
                stb.Append(" and G2GLocalConfigurazioni_COD = " & G2GLocalConfigurazioni_COD & vbCrLf)
            End If

            If G2GLocalConfigurazioni_DES <> "" Then
                stb.Append(" and G2GLocalConfigurazioni_DES = '" & Agro_SQL_SaveText(G2GLocalConfigurazioni_DES) & "'" & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return dt

    End Function


    Public Function LeggiImpreseXML( _
                                   ByRef includi_centriAziendali As Boolean, _
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                   Optional ByVal xFiltroAggiuntivo As String = "", _
                                   Optional ByVal SoloDatiNonImportatiGrafica As Boolean = False _
                    ) As String

        Dim DT As String
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.Leggi("
        Dim MessaggioErrore As String = ""

        Try
            Dim StrSQL As New StringBuilder
            LeggiImpreseXML_getQuery(includi_centriAziendali, xFiltroAggiuntivo, SoloDatiNonImportatiGrafica, StrSQL, objParametri)


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura_XML(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    Private Sub LeggiImpreseXML_getQuery(ByVal includi_centriAziendali As Boolean, ByVal xFiltroAggiuntivo As String, ByVal SoloDatiNonImportatiGrafica As Boolean, ByRef stb As StringBuilder, ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri)


        stb.AppendLine(" SET NOCOUNT ON ")
        stb.AppendLine("  ")
        stb.AppendLine(" Declare @baseCod integer ")
        stb.AppendLine(" Select top 1 @baseCod = cast(Sa_cod / (2 ^ 17) As int) * (2 ^ 17) ")
        stb.AppendLine(" from Grafica  ")
        stb.AppendLine(" where Sa_Cod Is Not null  ")
        stb.AppendLine(" And Sa_Cod <> 0 ")
        stb.AppendLine("  ")
        stb.AppendLine(" Set  @baseCod  = ISNULL(@baseCod, 0)")

        stb.Append(" Select  " & vbCrLf)
        stb.Append("      origine  " & vbCrLf)
        stb.Append("    , rag_soc  " & vbCrLf)

        If includi_centriAziendali Then
            stb.Append("    , sa_cod  " & vbCrLf)
        End If

        stb.Append("    , destinazione  " & vbCrLf)
        stb.Append("    , padre " & vbCrLf)
        stb.Append("    , 0 as Livello " & vbCrLf)
        stb.Append("    , flagimporta_audit " & vbCrLf)
        stb.Append("    , flagimporta_audit_interviste " & vbCrLf)
        stb.Append("    , flagimporta_agenda " & vbCrLf)
        stb.Append("    , flagimporta_pap " & vbCrLf)
        stb.Append("    , flagimporta_papz " & vbCrLf)
        stb.Append("    , flagimporta_notificabio " & vbCrLf)
        stb.Append("    , flagimporta_planning " & vbCrLf)
        stb.Append("    , flagimporta_distinta " & vbCrLf)
        stb.Append("    , flagimporta_ricette " & vbCrLf)
        stb.Append("    , flagimporta_pua " & vbCrLf)
        stb.Append("    , flagimporta_materieprime " & vbCrLf)
        stb.Append("    , flagimporta_pianocolturale " & vbCrLf)
        stb.Append("    , flagimporta_catasto " & vbCrLf)
        stb.Append("    , flagimporta_profilazione " & vbCrLf)
        stb.Append("    , flagimporta_gis " & vbCrLf)
        stb.Append("    , flagAccodaDatiSeEsistePivaDestinazione " & vbCrLf)
        stb.Append("    , flagimporta_lineeproduttive " & vbCrLf)
        stb.Append("    , flagimporta_piani_di_campionamento " & vbCrLf)
        stb.Append("    , flagimporta_analisi " & vbCrLf)

        stb.Append(" from ( " & vbCrLf)
        stb.Append(" select  " & vbCrLf)
        stb.Append("    imp.PIVA as origine " & vbCrLf)
        stb.Append("    , imp.rag_soc " & vbCrLf)
        stb.Append("    , substring(imp.piva, 1,11) as destinazione " & vbCrLf)

        If includi_centriAziendali Then
            stb.Append("    , sa.sa_cod as sa_cod " & vbCrLf)
        End If


        stb.Append("    , (select padre as piva, livello from GerarchiaImprese g where imp.PIVA = g.Figlio for xml path('impresa'), type, elements) padre  " & vbCrLf)

        stb.Append("    , (select top 1 case when aud.PIVA is null then null else '' end from audit aud where aud.piva = imp.piva ) as flagimporta_audit " & vbCrLf)
        stb.Append("    , (select top 1 case when aud.PIVA is null then null else '' end from Audit_Interviste aud where aud.piva = imp.piva ) as flagimporta_audit_interviste " & vbCrLf)
        stb.Append("    , (select top 1 case when a.PIVA is null then null else '' end from agenda a where a.piva = imp.piva ) as flagimporta_agenda " & vbCrLf)
        stb.Append("    , (select top 1 case when pap.PIVA is null then null else '' end from cdx_pap pap where pap.Piva = imp.piva ) as flagimporta_pap " & vbCrLf)
        stb.Append("    , (select top 1 case when papz.PAP_Piva  is null then null else '' end from CDX_PAPZeta papz where papz.pap_piva = imp.PIVA ) as flagimporta_papz " & vbCrLf)
        stb.Append("    , (select top 1 case when bionot.Notifica_Piva is null then null else '' end from BIO_Notifica_Frontespizio bionot where bionot.Notifica_Piva = imp.PIVA ) as flagimporta_notificabio " & vbCrLf)
        stb.Append("    , (select top 1 case when plann.Piva is null then null else '' end from Programmazione_Testata  plann where plann.Piva = imp.PIVA ) as flagimporta_planning " & vbCrLf)
        stb.Append("    , (select top 1 case when dist.PIVA is null then null else '' end from imprese_progetti dist where dist.piva = imp.piva ) as flagimporta_distinta " & vbCrLf)
        stb.Append("    , (select top 1 case when r.PIVA is null then null else '' end from ricette r where r.Piva = imp.PIVA ) as flagimporta_ricette " & vbCrLf)
        stb.Append("    , (select top 1 case when pua.PIVA is null then null else '' end from PUA_Testata pua where pua.Piva = imp.piva) as flagimporta_pua " & vbCrLf)
        stb.Append("    , (select top 1 case when mp.PIVA is null then null else '' end from Materie_Prime mp where mp.Piva = imp.PIVA ) as flagimporta_materieprime " & vbCrLf)
        stb.Append("    , (select top 1 case when appezza.PIVA is null then null else '' end from Appezzamento appezza where appezza.PIVA = imp.piva ) as flagimporta_pianocolturale " & vbCrLf)
        stb.Append("    , (select top 1 case when cat.PIVA is null then null else '' end from ImpreseXParticelle cat where  cat.piva = imp.piva ) as flagimporta_catasto " & vbCrLf)
        stb.Append("    , (select top 1 case when profilazione.PIVA is null then null else '' end from Profilazione_Dati profilazione where profilazione.PIVA = imp.PIVA ) as flagimporta_profilazione " & vbCrLf)
        stb.Append("    , (select top 1 case when gis.PIVA is null then null else '' end from Grafica gis where gis.piva = imp.piva) flagimporta_gis " & vbCrLf)
        stb.Append("    , (select top 1 case when lp.PIVA is null then null else '' end from linee_produzioni lp where lp.piva = imp.piva) flagimporta_lineeproduttive " & vbCrLf)
        stb.Append("    , (select top 1 case when pdc.PIVA is null then null else '' end from PDC_Dettagli pdc where pdc.piva = imp.piva) flagimporta_piani_di_campionamento " & vbCrLf)
        stb.Append("    , (select top 1 case when aet.PIVA is null then null else '' end from Analisi_EntitaxTestata aet where aet.piva = imp.piva) flagimporta_analisi " & vbCrLf)
        stb.Append("    , null as flagAccodaDatiSeEsistePivaDestinazione " & vbCrLf)
        stb.Append(" from Imprese imp " & vbCrLf)

        If includi_centriAziendali Then
            stb.Append("    inner join Centri_Aziendali sa on imp.piva = sa.piva " & vbCrLf)
        End If

        If SoloDatiNonImportatiGrafica Then
            Dim stb1 As New StringBuilder
            Dim anag = New AgronicaCoreGraficaDAL.Grafica_Read
            stb1.Length = 0
            anag.LeggiPerSincronizzazioneGIS2012_GetQuery("", 0, "", "", "", 1, "", "", objParametri_server, stb1, True)
            stb1.Append("UNION " & vbCrLf)
            anag.LeggiPerSincronizzazioneGIS2012_GetQuery("", 0, "", "", "", 2, "", "", objParametri_server, stb1, True)

            Dim s As String = stb1.ToString

            stb.Append(" inner join ( " & vbCrLf)
            stb.Append(s)

            stb.Append(" ) DatiNonImportatiGrafica " & vbCrLf)
            stb.Append(" on imp.PIVA = DatiNonImportatiGrafica.Piva  " & vbCrLf)

            If includi_centriAziendali Then
                stb.Append("    and sa.sa_cod = sa.sa_cod  " & vbCrLf)
            End If

        End If

        stb.Append(" ) aa  " & vbCrLf)
        stb.Append(" where 1=1  " & vbCrLf)

        If xFiltroAggiuntivo <> "" Then
            stb.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_server))
        End If


        stb.Append(" for xml path('impresa'), root('imprese') " & vbCrLf)



    End Sub





    Public Function LeggiLayerImportGraficaXML(
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim DT As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.Leggi("
        Dim MessaggioErrore As String = ""

        Try
            Dim StrSQL As New StringBuilder
            LeggiLayerImportGraficaXML_getQuery(objParametri, StrSQL)


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

    Private Shared Sub LeggiLayerImportGraficaXML_getQueryUtente(ByVal objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri, ByRef stb As StringBuilder)
        If objParametri_server.UtenteCodFiscale <> objParametri_server.PivaSuperUser Then
            stb.AppendLine("  and ( oldG.Username_Modifica in ( '" & objParametri_server.UtenteUsername & "', '" & objParametri_server.UtenteCodFiscale & "') ")
            stb.AppendLine(" or ( oldG.Username_Modifica in ( '" & objParametri_server.SuperUserUsername & "', '" & objParametri_server.PivaSuperUser & "') AND imp.Username_Modifica in ( '" & objParametri_server.UtenteUsername & "', '" & objParametri_server.UtenteCodFiscale & "') ) ")
            stb.AppendLine(" ) ")
        End If
    End Sub

    Private Sub LeggiLayerImportGraficaXML_getQuery(ByVal objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri, ByRef stb As StringBuilder)

        'seleziono il base cod, se non esistono record con sa_Cod non nulli e diversi da zero comunque non ci sono impianti disegnati e posso accettare zero come base_cod
        stb.AppendLine(" SET NOCOUNT ON ")
        stb.AppendLine("  ")
        stb.AppendLine(" Declare @baseCod Integer ")
        stb.AppendLine(" Select top 1 @baseCod = cast(Sa_cod / (2 ^ 17) As int) * (2 ^ 17) ")
        stb.AppendLine(" from Grafica  ")
        stb.AppendLine(" where Sa_Cod Is Not null  ")
        stb.AppendLine(" And Sa_Cod <> 0")
        stb.AppendLine("  ")

        stb.AppendLine("Set  @baseCod  = ISNULL(@baseCod, 0)")
        stb.AppendLine("  ")


        stb.Append("Select 1 As TipoOperazione_DB, substring(oldG.id,1,1) As codice, ll.Layer_Des, COUNT(*) As numeroNuovi " & vbCrLf)
        stb.Append("  from Grafica oldG   " & vbCrLf)
        stb.Append("  inner join Layers ll On substring(oldG.id, 1,1) = ll.Prefisso  " & vbCrLf)

        stb.AppendLine("  Left Join Reg_Impianti imp ")
        stb.AppendLine("     On imp.PIVA = oldG.Piva  ")
        stb.AppendLine("  And imp.SA_COD = oldG.Sa_Cod ")
        stb.AppendLine("  And substring(oldG.Id, 1, 1) = 'I' ")
        stb.AppendLine("  And imp.APPEZZA = @baseCod -1 + CONVERT(BIGINT,CONVERT(varbinary(2), master.dbo.fn_cdc_hexstrtobin( substring(oldG.id, 2,4)))) ")
        stb.AppendLine("  And imp.ID_REG = @baseCod -1 + CONVERT(BIGINT,CONVERT(varbinary(2), master.dbo.fn_cdc_hexstrtobin( substring(oldG.id, 6,4))))")

        stb.Append("     left join (  " & vbCrLf)
        stb.Append("     select Entita_Cod, OLDGrafica_ID, piva, sa_cod  " & vbCrLf)
        stb.Append("     from GIS_Entita  " & vbCrLf)
        stb.Append("     where OLDGrafica_ID  is not null " & vbCrLf)
        stb.Append("        union " & vbCrLf)
        stb.Append("     select -1 as Entita_Cod, OLDGrafica_ID, piva, sa_cod " & vbCrLf)
        stb.Append("     from GIS_ElementiGrafici_LogErrori err " & vbCrLf)
        stb.Append("      ) ee    " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("     on oldG.Id = ee.OLDGrafica_ID   " & vbCrLf)
        stb.Append("    and oldG.piva = ee.piva " & vbCrLf)
        stb.Append("    and oldG.sa_cod = ee.sa_cod   " & vbCrLf)
        stb.Append("  where ee.Entita_Cod is null  " & vbCrLf)
        stb.Append("  AND   not (oldG.vx1 is null and oldG.vy1 is null and oldG.vx2 is null and oldG.vy2 is null) " & vbCrLf)

        LeggiLayerImportGraficaXML_getQueryUtente(objParametri_server, stb)


        stb.Append("  group by substring(oldG.id,1,1), ll.Layer_Des " & vbCrLf)


        stb.Append(" union " & vbCrLf)

        stb.Append("  " & vbCrLf)
        stb.Append(" select  2 as TipoOperazione_DB, substring(oldG.id,1,1) as codice, ll.Layer_Des, COUNT(*) as numeroNuovi  " & vbCrLf)
        stb.Append(" from Grafica oldG  " & vbCrLf)
        stb.AppendLine("  Left Join Reg_Impianti imp ")
        stb.AppendLine("     On imp.PIVA = oldG.Piva  ")
        stb.AppendLine("  And imp.SA_COD = oldG.Sa_Cod ")
        stb.AppendLine("  And substring(oldG.Id, 1, 1) = 'I' ")
        stb.AppendLine("  And imp.APPEZZA = @baseCod -1 + CONVERT(BIGINT,CONVERT(varbinary(2), master.dbo.fn_cdc_hexstrtobin( substring(oldG.id, 2,4)))) ")
        stb.AppendLine("  And imp.ID_REG = @baseCod -1 + CONVERT(BIGINT,CONVERT(varbinary(2), master.dbo.fn_cdc_hexstrtobin( substring(oldG.id, 6,4))))")
        stb.Append("    inner join Layers ll on substring(oldG.id, 1,1) = ll.Prefisso   " & vbCrLf)
        stb.Append("    inner join ( " & vbCrLf)
        stb.Append("    select piva, sa_cod, Entita_Cod, OLDGrafica_ID, data_modifica  " & vbCrLf)
        stb.Append("    from GIS_Entita " & vbCrLf)
        stb.Append("    where OLDGrafica_ID  is not null) ee  " & vbCrLf)
        stb.Append("    on oldG.Id = ee.OLDGrafica_ID  " & vbCrLf)
        stb.Append("    and oldG.piva = ee.piva " & vbCrLf)
        stb.Append("    and oldG.sa_cod = ee.sa_cod   " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append(" where oldG.data_modifica>ee.data_modifica " & vbCrLf)
        stb.Append(" and oldG.GPS = 1 " & vbCrLf)
        stb.Append("  AND   not (oldG.vx1 is null and oldG.vy1 is null and oldG.vx2 is null and oldG.vy2 is null) " & vbCrLf)

        LeggiLayerImportGraficaXML_getQueryUtente(objParametri_server, stb)

        stb.Append(" group by substring(oldG.id,1,1), ll.Layer_Des " & vbCrLf)
        stb.Append("  " & vbCrLf)





    End Sub

End Class





'#################################################################
'#################################################################
'#################################################################

Public Class G2GLocal_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    Public Function Scrivi(
                          ByVal G2GLocalConfigurazioni_COD As Integer _
                          , ByVal G2GLocalConfigurazioni_DES As String _
                          , ByVal G2GLocalConfigurazioni_CFG As String _
                          , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                , Optional ByVal Data_creazione As Date = #2/1/1900# _
                , Optional ByVal Data_modifica As Date = #2/1/1900# _
                , Optional ByVal username_creazione As String = "" _
                , Optional ByVal username_modifica As String = ""
                ) As Boolean


        Dim NomeRoutine As String = "Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_creazione = #2/1/1900# Then
                Data_creazione = Date.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If



            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" INSERT G2GLocalConfigurazioni " + vbCrLf)

            StrSQL.Append("              (")

            StrSQL.Append("              G2GLocalConfigurazioni_COD,             ")
            StrSQL.Append("              G2GLocalConfigurazioni_DES,             ")
            StrSQL.Append("              G2GLocalConfigurazioni_CFG,             ")

            StrSQL.Append("              Inviato,            datainvio, ")
            StrSQL.Append("              Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("              UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("              Validita_Inizio,    Validita_Fine ")

            StrSQL.Append("              ) ")

            StrSQL.Append(" VALUES ( ")

            StrSQL.Append("			  " & Agro_SQL_SaveNum(G2GLocalConfigurazioni_COD) & " ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(G2GLocalConfigurazioni_DES) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(G2GLocalConfigurazioni_CFG) & "' ")


            StrSQL.Append("         , 0  " + vbCrLf)
            StrSQL.Append("         , Null  " + vbCrLf)

            StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.Append("			, " & Agro_SQL_SaveDate(AGRODATAINIZIO) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDate(AGRODATAFINE) & "  ")


            StrSQL.Append(") ")

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




    '#################################################################
    Public Function Modifica(
                            ByVal G2GLocalConfigurazioni_COD As Integer _
                          , ByVal G2GLocalConfigurazioni_DES As String _
                          , ByVal G2GLocalConfigurazioni_CFG As String _
                          , ByVal xFiltroAggiuntivo As String,
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                              ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "Modifica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            '---------------------------------------------
            StrSQL.Append(" UPDATE G2GLocalConfigurazioni ")
            StrSQL.Append(" SET ")
            StrSQL.Append("         G2GLocalConfigurazioni_DES = '" & Agro_SQL_SaveText(G2GLocalConfigurazioni_DES) & "' ")
            StrSQL.Append("        , G2GLocalConfigurazioni_CFG = '" & Agro_SQL_SaveText(G2GLocalConfigurazioni_CFG) & "' ")
            StrSQL.Append("        , Data_Modifica = " & Agro_SQL_SaveDateTime(Now()) & " ")
            StrSQL.Append("        , username_modifica = '" & Agro_SQL_SaveText(objParametri.UtenteCodFiscale) & "' ")
            StrSQL.Append(" WHERE   G2GLocalConfigurazioni_COD = " & Agro_SQL_SaveNum(G2GLocalConfigurazioni_COD) & " ")

            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

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





    '#################################################################
    Public Function Cancella(ByVal xFiltroAggiuntivo As String,
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                              ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                StrSQL.Append(" UPDATE ... ")
                StrSQL.Append(" SET ")
                StrSQL.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE   1=1 ")
                StrSQL.Append(" AND     Inviato >= 0 ")
            Else
                StrSQL.Append(" DELETE FROM ... ")
                StrSQL.Append(" WHERE 1=1 ")
            End If
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

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



