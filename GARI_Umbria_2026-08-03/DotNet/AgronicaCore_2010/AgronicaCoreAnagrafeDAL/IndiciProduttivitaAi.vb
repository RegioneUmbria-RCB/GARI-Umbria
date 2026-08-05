Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreModelsSTD.analisi.correzioni
Public Class IndiciProduttivitaAi_R
    Inherits DataProvider

    Public Function Leggi(ByVal IDIndiciProduttivita As Integer,
                          ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                          Optional ByVal Validita_Fine As Date = AGRODATAFINE) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.IndiciProduttivitaAi_R.Leggi()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
                    '------------------------------

                    'Query per il prelievo dei dati                 ' #### CLASSE ####

                    StrSQL.Append(" SELECT IndiciProduttivitaAi.* ")
                    StrSQL.Append(" FROM   IndiciProduttivitaAi ")
                    StrSQL.Append(" WHERE  Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
                    StrSQL.Append(" AND    Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Fine) & " ")

                    If IDIndiciProduttivita <> 0 Then
                        StrSQL.Append(" AND IndiciProduttivitaAi_COD = " & Agro_SQL_SaveNum(IDIndiciProduttivita) & " ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If
                    '------------------------------

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
Public Class IndiciProduttivitaAi_W
    Inherits DataProvider
    Public Function Scrivi(ByVal IDIndiciProduttivita As Integer,
                           ByVal Produttività As Decimal,
                           ByVal PLV As Decimal,
                           ByVal IndiceErosione As Decimal,
                           ByVal IndiceCO2 As Decimal,
                           ByVal IndiceRischioMeteoAggregato As Decimal,
                           ByVal IndiceRischioGelata As Decimal,
                           ByVal IndiceRischioVentoForte As Decimal,
                           ByVal IndiceRischioSiccita As Decimal,
                           ByVal IndiceRischioGrandine As Decimal,
                           ByVal IndiceRischioAllagamento As Decimal,
                           ByVal port_Produttivita As Decimal,
                           ByVal port_plv As Decimal,
                           ByVal port_IndiceErosione As Decimal,
                           ByVal port_IndiceCO2 As Decimal,
                           ByVal port_IndiceRischioMeteoAggregato As Decimal,
                           ByVal port_IndiceRischioGelata As Decimal,
                           ByVal port_IndiceRischioVentoForte As Decimal,
                           ByVal port_IndiceRischioSiccita As Decimal,
                           ByVal port_IndiceRischioGrandine As Decimal,
                           ByVal port_IndiceRischioAllagamento As Decimal,
                           ByVal reg_Produttivita As Decimal,
                           ByVal reg_plv As Decimal,
                           ByVal reg_IndiceErosione As Decimal,
                           ByVal reg_IndiceCO2 As Decimal,
                           ByVal reg_IndiceRischioMeteoAggregato As Decimal,
                           ByVal reg_IndiceRischioGelata As Decimal,
                           ByVal reg_IndiceRischioVentoForte As Decimal,
                           ByVal reg_IndiceRischioSiccita As Decimal,
                           ByVal reg_IndiceRischioGrandine As Decimal,
                           ByVal reg_IndiceRischioAllagamento As Decimal,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                           Optional ByVal Validita_Fine As Date = AGRODATAFINE) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.IndiciProduttivitaAi_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine("INSERT INTO IndiciProduttivitaAi( ")
            StrSQL.AppendLine("            IndiciProduttivitaAi_COD, Produttivita, ")
            StrSQL.AppendLine("            PLV, IndiceErosione, IndiceCO2, ")
            StrSQL.AppendLine("            IndiceRischioMeteoAggregato, IndiceRischioGelata, IndiceRischioVentoForte, ")
            StrSQL.AppendLine("            IndiceRischioSiccita, IndiceRischioGrandine, IndiceRischioAllagamento, ")
            StrSQL.AppendLine("            port_Produttivita, port_plv, port_IndiceErosione, ")
            StrSQL.AppendLine("            port_IndiceCO2, port_IndiceRischioMeteoAggregato, port_IndiceRischioGelata, ")
            StrSQL.AppendLine("            port_IndiceRischioVentoForte, port_IndiceRischioSiccita, ")
            StrSQL.AppendLine("            port_IndiceRischioGrandine,port_IndiceRischioAllagamento, reg_Produttivita, ")
            StrSQL.AppendLine("            reg_plv, reg_IndiceErosione, reg_IndiceCO2, reg_IndiceRischioMeteoAggregato, ")
            StrSQL.AppendLine("            reg_IndiceRischioGelata, reg_IndiceRischioVentoForte, ")
            StrSQL.AppendLine("            reg_IndiceRischioSiccita, reg_IndiceRischioGrandine, ")
            StrSQL.AppendLine("            reg_IndiceRischioAllagamento, ")
            StrSQL.AppendLine("            Inviato, DataInvio, ")
            StrSQL.AppendLine("            Data_Creazione,     Data_Modifica, ")
            StrSQL.AppendLine("            UserName_Creazione, UserName_Modifica, ")
            StrSQL.AppendLine("            Validita_Inizio,    Validita_Fine ")
            StrSQL.AppendLine("            ) ")

            StrSQL.AppendLine("VALUES (")
            StrSQL.AppendLine("           " & Agro_SQL_SaveNum(IDIndiciProduttivita) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Produttività) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(PLV) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(IndiceErosione) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(IndiceCO2) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(IndiceRischioMeteoAggregato) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(IndiceRischioGelata) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(IndiceRischioVentoForte) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(IndiceRischioSiccita) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(IndiceRischioGrandine) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(IndiceRischioAllagamento) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(port_Produttivita) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(port_plv) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(port_IndiceErosione) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(port_IndiceCO2) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(port_IndiceRischioMeteoAggregato) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(port_IndiceRischioGelata) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(port_IndiceRischioVentoForte) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(port_IndiceRischioSiccita) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(port_IndiceRischioGrandine) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(port_IndiceRischioAllagamento) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(reg_Produttivita) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(reg_plv) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(reg_IndiceErosione) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(reg_IndiceCO2) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(reg_IndiceRischioMeteoAggregato) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(reg_IndiceRischioGelata) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(reg_IndiceRischioVentoForte) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(reg_IndiceRischioSiccita) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(reg_IndiceRischioGrandine) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(reg_IndiceRischioAllagamento) & "  ")
            StrSQL.AppendLine("         , 0  ")
            StrSQL.AppendLine("         , NULL  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(DateTime.Now) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(DateTime.Now) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.AppendLine(")")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function Modifica(ByVal IDIndiciProduttivita As Integer,
                             ByVal Produttività As Decimal,
                             ByVal PLV As Decimal,
                             ByVal IndiceErosione As Decimal,
                             ByVal IndiceCO2 As Decimal,
                             ByVal IndiceRischioMeteoAggregato As Decimal,
                             ByVal IndiceRischioGelata As Decimal,
                             ByVal IndiceRischioVentoForte As Decimal,
                             ByVal IndiceRischioSiccita As Decimal,
                             ByVal IndiceRischioGrandine As Decimal,
                             ByVal IndiceRischioAllagamento As Decimal,
                             ByVal port_Produttivita As Decimal,
                             ByVal port_plv As Decimal,
                             ByVal port_IndiceErosione As Decimal,
                             ByVal port_IndiceCO2 As Decimal,
                             ByVal port_IndiceRischioMeteoAggregato As Decimal,
                             ByVal port_IndiceRischioGelata As Decimal,
                             ByVal port_IndiceRischioVentoForte As Decimal,
                             ByVal port_IndiceRischioSiccita As Decimal,
                             ByVal port_IndiceRischioGrandine As Decimal,
                             ByVal port_IndiceRischioAllagamento As Decimal,
                             ByVal reg_Produttivita As Decimal,
                             ByVal reg_plv As Decimal,
                             ByVal reg_IndiceErosione As Decimal,
                             ByVal reg_IndiceCO2 As Decimal,
                             ByVal reg_IndiceRischioMeteoAggregato As Decimal,
                             ByVal reg_IndiceRischioGelata As Decimal,
                             ByVal reg_IndiceRischioVentoForte As Decimal,
                             ByVal reg_IndiceRischioSiccita As Decimal,
                             ByVal reg_IndiceRischioGrandine As Decimal,
                             ByVal reg_IndiceRischioAllagamento As Decimal,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                             Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                             Optional ByVal Validita_Fine As Date = AGRODATAFINE) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.IndiciProduttivitaAi_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine("UPDATE IndiciProduttivitaAi SET ")
            StrSQL.AppendLine("   Inviato           =  0 ")
            StrSQL.AppendLine("   ,DataInvio         =  Null ")
            StrSQL.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now))
            StrSQL.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.AppendLine("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.AppendLine("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            StrSQL.AppendLine("   ,Produttivita     =  " & Agro_SQL_SaveNum(Produttività))
            StrSQL.AppendLine("   ,PLV     =  " & Agro_SQL_SaveNum(PLV))
            StrSQL.AppendLine("   ,IndiceErosione     =  " & Agro_SQL_SaveNum(IndiceErosione))
            StrSQL.AppendLine("   ,IndiceCO2     =  " & Agro_SQL_SaveNum(IndiceCO2))
            StrSQL.AppendLine("   ,IndiceRischioMeteoAggregato     =  " & Agro_SQL_SaveNum(IndiceRischioMeteoAggregato))
            StrSQL.AppendLine("   ,IndiceRischioGelata     =  " & Agro_SQL_SaveNum(IndiceRischioGelata))
            StrSQL.AppendLine("   ,IndiceRischioVentoForte     =  " & Agro_SQL_SaveNum(IndiceRischioVentoForte))
            StrSQL.AppendLine("   ,IndiceRischioSiccita     =  " & Agro_SQL_SaveNum(IndiceRischioSiccita))
            StrSQL.AppendLine("   ,IndiceRischioGrandine     =  " & Agro_SQL_SaveNum(IndiceRischioGrandine))
            StrSQL.AppendLine("   ,IndiceRischioAllagamento     =  " & Agro_SQL_SaveNum(IndiceRischioAllagamento))
            StrSQL.AppendLine("   ,port_Produttivita     = " & Agro_SQL_SaveNum(port_Produttivita))
            StrSQL.AppendLine("   ,port_plv     = " & Agro_SQL_SaveNum(port_plv))
            StrSQL.AppendLine("   ,port_IndiceErosione     = " & Agro_SQL_SaveNum(port_IndiceErosione))
            StrSQL.AppendLine("   ,port_IndiceCO2     = " & Agro_SQL_SaveNum(port_IndiceCO2))
            StrSQL.AppendLine("   ,port_IndiceRischioMeteoAggregato     = " & Agro_SQL_SaveNum(port_IndiceRischioMeteoAggregato))
            StrSQL.AppendLine("   ,port_IndiceRischioGelata     = " & Agro_SQL_SaveNum(port_IndiceRischioGelata))
            StrSQL.AppendLine("   ,port_IndiceRischioVentoForte     = " & Agro_SQL_SaveNum(port_IndiceRischioVentoForte))
            StrSQL.AppendLine("   ,port_IndiceRischioSiccita     = " & Agro_SQL_SaveNum(port_IndiceRischioSiccita))
            StrSQL.AppendLine("   ,port_IndiceRischioGrandine     = " & Agro_SQL_SaveNum(port_IndiceRischioGrandine))
            StrSQL.AppendLine("   ,port_IndiceRischioAllagamento     = " & Agro_SQL_SaveNum(port_IndiceRischioAllagamento))
            StrSQL.AppendLine("   ,reg_Produttivita     = " & Agro_SQL_SaveNum(reg_Produttivita))
            StrSQL.AppendLine("   ,reg_plv     = " & Agro_SQL_SaveNum(reg_plv))
            StrSQL.AppendLine("   ,reg_IndiceErosione     = " & Agro_SQL_SaveNum(reg_IndiceErosione))
            StrSQL.AppendLine("   ,reg_IndiceCO2     = " & Agro_SQL_SaveNum(reg_IndiceCO2))
            StrSQL.AppendLine("   ,reg_IndiceRischioMeteoAggregato     = " & Agro_SQL_SaveNum(reg_IndiceRischioMeteoAggregato))
            StrSQL.AppendLine("   ,reg_IndiceRischioGelata     = " & Agro_SQL_SaveNum(reg_IndiceRischioGelata))
            StrSQL.AppendLine("   ,reg_IndiceRischioVentoForte     = " & Agro_SQL_SaveNum(reg_IndiceRischioVentoForte))
            StrSQL.AppendLine("   ,reg_IndiceRischioSiccita     = " & Agro_SQL_SaveNum(reg_IndiceRischioSiccita))
            StrSQL.AppendLine("   ,reg_IndiceRischioGrandine     = " & Agro_SQL_SaveNum(reg_IndiceRischioGrandine))
            StrSQL.AppendLine("   ,reg_IndiceRischioAllagamento     = " & Agro_SQL_SaveNum(reg_IndiceRischioAllagamento))

            StrSQL.AppendLine(" WHERE 1=1 ")
            If IDIndiciProduttivita <> 0 Then
                StrSQL.AppendLine(" and IndiciProduttivitaAi_COD = " & Agro_SQL_SaveNum(IDIndiciProduttivita) & " ")
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function
    Public Function Cancella(ByVal IDIndiciProduttivita As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.IndiciProduttivitaAi_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" DELETE ")
            StrSQL.AppendLine(" FROM     IndiciProduttivitaAi ")
            StrSQL.AppendLine(" WHERE 1=1 ")
            If IDIndiciProduttivita <> 0 Then
                StrSQL.AppendLine(" AND IndiciProduttivitaAi_COD = " & Agro_SQL_SaveNum(IDIndiciProduttivita) & " ")
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function RicalcolaMediaRegionale(ByVal CodRegione As String,
                                            ByVal ValiditaInizio As DateTime,
                                            ByVal ValiditaFine As DateTime,
                                            ByRef objParametri As AgronicaCoreParametri) As Boolean
        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.IndiciProduttivitaAi_W.RicalcolaMediaRegionale()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" merge into IndiciProduttivitaAi dest using( ")
            StrSQL.AppendLine("     select  e.REG, ")
            StrSQL.AppendLine("             b1.validita_inizio, ")
            StrSQL.AppendLine("             b1.validita_fine, ")
            StrSQL.AppendLine("             (cast(e.REG as int) + (YEAR(b1.validita_inizio)-2021)*20)*-1 as IndiciProduttivitaAi_COD, ")
            StrSQL.AppendLine("             cast(sum(b1.Produttivita*r.Sup_Imp)/sum(r.Sup_Imp) as numeric(19,2)) as Produttivita, ")
            StrSQL.AppendLine("             cast(sum(b1.PLV*r.Sup_Imp)/sum(r.Sup_Imp) as numeric(19,2)) as PLV, ")
            StrSQL.AppendLine("             cast(sum(b1.IndiceErosione*r.Sup_Imp)/sum(r.Sup_Imp) as numeric(19,2)) as IndiceErosione, ")
            StrSQL.AppendLine("             cast(sum(b1.IndiceCO2*r.Sup_Imp)/sum(r.Sup_Imp) as numeric(19,2)) as IndiceCO2, ")
            StrSQL.AppendLine("             cast(sum(b1.IndiceRischioMeteoAggregato*r.Sup_Imp)/sum(r.Sup_Imp) as numeric(19,2)) as IndiceRischioMeteoAggregato, ")
            StrSQL.AppendLine("             cast(sum(b1.IndiceRischioGelata*r.Sup_Imp)/sum(r.Sup_Imp) as numeric(19,2)) as IndiceRischioGelata, ")
            StrSQL.AppendLine("             cast(sum(b1.IndiceRischioVentoForte*r.Sup_Imp)/sum(r.Sup_Imp) as numeric(19,2)) as IndiceRischioVentoForte, ")
            StrSQL.AppendLine("             cast(sum(b1.IndiceRischioSiccita*r.Sup_Imp)/sum(r.Sup_Imp) as numeric(19,2)) as IndiceRischioSiccita, ")
            StrSQL.AppendLine("             cast(sum(b1.IndiceRischioGrandine*r.Sup_Imp)/sum(r.Sup_Imp) as numeric(19,2)) as IndiceRischioGrandine, ")
            StrSQL.AppendLine("             cast(sum(b1.IndiceRischioAllagamento*r.Sup_Imp)/sum(r.Sup_Imp) as numeric(19,2)) as IndiceRischioAllagamento ")
            StrSQL.AppendLine("     from  ")
            StrSQL.AppendLine("         Reg_Impianti_XIndiciProduttivitaAi a1 inner join ")
            StrSQL.AppendLine("         IndiciProduttivitaAi b1 on (a1.IndiciProduttivitaAi_COD=b1.IndiciProduttivitaAi_COD) inner join ")
            StrSQL.AppendLine("         Imprese a on (a1.piva=a.piva) inner join ")
            StrSQL.AppendLine("         ImpresexIndirizzi b on (a.PIVA=b.PIVA) inner join ")
            StrSQL.AppendLine("         Indirizzi c on (b.cod_indirizzo=c.cod_indirizzo) inner join ")
            StrSQL.AppendLine("         ISTAT d on (c.pro_cod_istat = d.PROV and c.com_cod_istat=d.COM) inner join ")
            StrSQL.AppendLine("         Lista_Province e on (d.PROV = e.PROV and d.COM=e.COM) inner join ")
            StrSQL.AppendLine("         Reg_Impianti r on (a1.piva=r.PIVA and a1.sa_cod=r.SA_COD and a1.appezza=r.APPEZZA and a1.id_reg=r.id_reg ) ")
            StrSQL.AppendLine("     where ")
            If CodRegione = "" Then
                StrSQL.AppendLine(String.Format("         reg<>'{0}'", "000"))
            Else
                StrSQL.AppendLine(String.Format("         reg='{0}' ", Agro_SQL_SaveText(CodRegione)))
            End If
            If ValiditaInizio <> AGRODATAINIZIO Then
                StrSQL.AppendLine(String.Format("     and b1.validita_inizio>={0} ", Agro_SQL_SaveDateTime(ValiditaInizio)))
            End If
            If ValiditaFine <> AGRODATAFINE Then
                StrSQL.AppendLine(String.Format("     and b1.validita_fine<={0} ", Agro_SQL_SaveDateTime(ValiditaFine)))
            End If

            StrSQL.AppendLine("     group by ")
            StrSQL.AppendLine("         e.REG, ")
            StrSQL.AppendLine("         b1.Validita_Inizio, ")
            StrSQL.AppendLine("         b1.Validita_Fine ")
            StrSQL.AppendLine(" ) as source on (source.IndiciProduttivitaAi_COD=dest.IndiciProduttivitaAi_COD) ")
            StrSQL.AppendLine(" when matched then ")
            StrSQL.AppendLine(" 	update set  ")
            StrSQL.AppendLine(" 		dest.Produttivita =source.Produttivita, ")
            StrSQL.AppendLine(" 		dest.PLV =source.PLV, ")
            StrSQL.AppendLine(" 		dest.IndiceErosione =source.IndiceErosione, ")
            StrSQL.AppendLine(" 		dest.IndiceCO2 =source.IndiceCO2, ")
            StrSQL.AppendLine(" 		dest.IndiceRischioMeteoAggregato =source.IndiceRischioMeteoAggregato, ")
            StrSQL.AppendLine(" 		dest.IndiceRischioGelata =source.IndiceRischioGelata, ")
            StrSQL.AppendLine(" 		dest.IndiceRischioVentoForte =source.IndiceRischioVentoForte, ")
            StrSQL.AppendLine(" 		dest.IndiceRischioSiccita =source.IndiceRischioSiccita, ")
            StrSQL.AppendLine(" 		dest.IndiceRischioGrandine =source.IndiceRischioGrandine, ")
            StrSQL.AppendLine(" 		dest.IndiceRischioAllagamento =source.IndiceRischioAllagamento, ")
            StrSQL.AppendLine(" 		dest.data_modifica =getdate(), ")
            StrSQL.AppendLine(String.Format(" 		dest.username_modifica='{0}' ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))
            StrSQL.AppendLine(" when not matched by target then ")
            StrSQL.AppendLine(" 	insert ( ")
            StrSQL.AppendLine(" 				IndiciProduttivitaAi_COD, ")
            StrSQL.AppendLine(" 				Produttivita, ")
            StrSQL.AppendLine(" 				PLV, ")
            StrSQL.AppendLine(" 				IndiceErosione, ")
            StrSQL.AppendLine(" 				IndiceCO2, ")
            StrSQL.AppendLine(" 				IndiceRischioMeteoAggregato, ")
            StrSQL.AppendLine(" 				IndiceRischioGelata, ")
            StrSQL.AppendLine(" 				IndiceRischioVentoForte, ")
            StrSQL.AppendLine(" 				IndiceRischioSiccita, ")
            StrSQL.AppendLine(" 				IndiceRischioGrandine, ")
            StrSQL.AppendLine(" 				IndiceRischioAllagamento, ")
            StrSQL.AppendLine(" 				inviato, ")
            StrSQL.AppendLine(" 				datainvio, ")
            StrSQL.AppendLine(" 				data_creazione, ")
            StrSQL.AppendLine(" 				data_modifica, ")
            StrSQL.AppendLine(" 				username_creazione, ")
            StrSQL.AppendLine(" 				username_modifica, ")
            StrSQL.AppendLine(" 				validita_inizio, ")
            StrSQL.AppendLine(" 				validita_fine ")
            StrSQL.AppendLine(" 			)values ")
            StrSQL.AppendLine("             ( ")
            StrSQL.AppendLine(" 				source.IndiciProduttivitaAi_COD, ")
            StrSQL.AppendLine(" 				source.Produttivita, ")
            StrSQL.AppendLine(" 				source.PLV, ")
            StrSQL.AppendLine(" 				source.IndiceErosione, ")
            StrSQL.AppendLine(" 				source.IndiceCO2, ")
            StrSQL.AppendLine(" 				source.IndiceRischioMeteoAggregato, ")
            StrSQL.AppendLine(" 				source.IndiceRischioGelata, ")
            StrSQL.AppendLine(" 				source.IndiceRischioVentoForte, ")
            StrSQL.AppendLine(" 				source.IndiceRischioSiccita, ")
            StrSQL.AppendLine(" 				source.IndiceRischioGrandine, ")
            StrSQL.AppendLine(" 				source.IndiceRischioAllagamento, ")
            StrSQL.AppendLine(" 				0, ")
            StrSQL.AppendLine(" 				null, ")
            StrSQL.AppendLine(" 				getdate(), ")
            StrSQL.AppendLine(" 				getdate(), ")
            StrSQL.AppendLine(String.Format(" 				'{0}', ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))
            StrSQL.AppendLine(String.Format(" 				'{0}', ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))
            StrSQL.AppendLine(" 				source.validita_inizio, ")
            StrSQL.AppendLine(" 				source.validita_fine ")
            StrSQL.AppendLine(" 			); ")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("")

            StrSQL.AppendLine(" merge into Reg_Impianti_XIndiciProduttivitaAi dest using( ")
            StrSQL.AppendLine("     select  cast(e.REG as int) *-1 as piva, ")
            StrSQL.AppendLine("             b1.validita_inizio, ")
            StrSQL.AppendLine("             b1.validita_fine, ")
            StrSQL.AppendLine("             (cast(e.REG as int) + (YEAR(b1.validita_inizio)-2021)*20)*-1 as IndiciProduttivitaAi_COD ")
            StrSQL.AppendLine("     from  ")
            StrSQL.AppendLine("         Reg_Impianti_XIndiciProduttivitaAi a1 inner join ")
            StrSQL.AppendLine("         IndiciProduttivitaAi b1 on (a1.IndiciProduttivitaAi_COD=b1.IndiciProduttivitaAi_COD) inner join ")
            StrSQL.AppendLine("         Imprese a on (a1.piva=a.piva) inner join ")
            StrSQL.AppendLine("         ImpresexIndirizzi b on (a.PIVA=b.PIVA) inner join ")
            StrSQL.AppendLine("         Indirizzi c on (b.cod_indirizzo=c.cod_indirizzo) inner join ")
            StrSQL.AppendLine("         ISTAT d on (c.pro_cod_istat = d.PROV and c.com_cod_istat=d.COM) inner join ")
            StrSQL.AppendLine("         Lista_Province e on (d.PROV = e.PROV and d.COM=e.COM) ")
            StrSQL.AppendLine("     where ")
            If CodRegione = "" Then
                StrSQL.AppendLine(String.Format("         reg<>'{0}'", "000"))
            Else
                StrSQL.AppendLine(String.Format("         reg='{0}' ", Agro_SQL_SaveText(CodRegione)))
            End If
            If ValiditaInizio <> AGRODATAINIZIO Then
                StrSQL.AppendLine(String.Format("     and b1.validita_inizio>={0} ", Agro_SQL_SaveDateTime(ValiditaInizio)))
            End If
            If ValiditaFine <> AGRODATAFINE Then
                StrSQL.AppendLine(String.Format("     and b1.validita_fine<={0} ", Agro_SQL_SaveDateTime(ValiditaFine)))
            End If

            StrSQL.AppendLine("     group by ")
            StrSQL.AppendLine("         e.REG, ")
            StrSQL.AppendLine("         b1.Validita_Inizio, ")
            StrSQL.AppendLine("         b1.Validita_Fine ")
            StrSQL.AppendLine(" ) as source on (source.IndiciProduttivitaAi_COD=dest.IndiciProduttivitaAi_COD) ")
            StrSQL.AppendLine(" when not matched by target then ")
            StrSQL.AppendLine(" 	insert ( ")
            StrSQL.AppendLine(" 				IndiciProduttivitaAi_COD, ")
            StrSQL.AppendLine(" 				piva, ")
            StrSQL.AppendLine(" 				sa_cod, ")
            StrSQL.AppendLine(" 				appezza, ")
            StrSQL.AppendLine(" 				id_reg, ")
            StrSQL.AppendLine(" 				inviato, ")
            StrSQL.AppendLine(" 				datainvio, ")
            StrSQL.AppendLine(" 				data_creazione, ")
            StrSQL.AppendLine(" 				data_modifica, ")
            StrSQL.AppendLine(" 				username_creazione, ")
            StrSQL.AppendLine(" 				username_modifica, ")
            StrSQL.AppendLine(" 				validita_inizio, ")
            StrSQL.AppendLine(" 				validita_fine ")
            StrSQL.AppendLine(" 			)values ")
            StrSQL.AppendLine("             ( ")
            StrSQL.AppendLine(" 				source.IndiciProduttivitaAi_COD, ")
            StrSQL.AppendLine(" 				source.piva, ")
            StrSQL.AppendLine(" 				0, ")
            StrSQL.AppendLine(" 				0, ")
            StrSQL.AppendLine(" 				0, ")
            StrSQL.AppendLine(" 				0, ")
            StrSQL.AppendLine(" 				null, ")
            StrSQL.AppendLine(" 				getdate(), ")
            StrSQL.AppendLine(" 				getdate(), ")
            StrSQL.AppendLine(String.Format(" 				'{0}', ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))
            StrSQL.AppendLine(String.Format(" 				'{0}', ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))
            StrSQL.AppendLine(" 				source.validita_inizio, ")
            StrSQL.AppendLine(" 				source.validita_fine ")
            StrSQL.AppendLine(" 			); ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try
        Return xRisp
    End Function

    Public Function RicalcolaMinMax(ByVal ValiditaInizio As DateTime,
                                    ByVal ValiditaFine As DateTime,
                                    ByRef objParametri As AgronicaCoreParametri) As Boolean
        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.IndiciProduttivitaAi_W.RicalcolaMinMax()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" merge into IndiciProduttivitaAi dest using( ")
            StrSQL.AppendLine("     select 'MIN' as t, ")
            StrSQL.AppendLine("     	b1.validita_inizio, ")
            StrSQL.AppendLine("     	b1.validita_fine, ")
            StrSQL.AppendLine("     	(100100 + (YEAR(b1.validita_inizio)-2021))*-1 as IndiciProduttivitaAi_COD, ")
            StrSQL.AppendLine("     	cast(min(b1.Produttivita) as numeric(19,2)) as Produttivita, ")
            StrSQL.AppendLine("     	cast(min(b1.PLV) as numeric(19,2)) as PLV, ")
            StrSQL.AppendLine("     	cast(min(b1.IndiceErosione) as numeric(19,2)) as IndiceErosione, ")
            StrSQL.AppendLine("     	cast(min(b1.IndiceCO2) as numeric(19,2)) as IndiceCO2, ")
            StrSQL.AppendLine("     	0 as IndiceRischioMeteoAggregato, ")
            StrSQL.AppendLine("     	null as IndiceRischioGelata, ")
            StrSQL.AppendLine("     	null as IndiceRischioVentoForte, ")
            StrSQL.AppendLine("     	null as IndiceRischioSiccita, ")
            StrSQL.AppendLine("     	null as IndiceRischioGrandine, ")
            StrSQL.AppendLine("     	null as IndiceRischioAllagamento ")
            StrSQL.AppendLine("     from  ")
            StrSQL.AppendLine("         Reg_Impianti_XIndiciProduttivitaAi a1 inner join ")
            StrSQL.AppendLine("         IndiciProduttivitaAi b1 on (a1.IndiciProduttivitaAi_COD=b1.IndiciProduttivitaAi_COD) inner join ")
            StrSQL.AppendLine("         Imprese a on (a1.piva=a.piva) inner join ")
            StrSQL.AppendLine("         ImpresexIndirizzi b on (a.PIVA=b.PIVA) inner join ")
            StrSQL.AppendLine("         Indirizzi c on (b.cod_indirizzo=c.cod_indirizzo) inner join ")
            StrSQL.AppendLine("         ISTAT d on (c.pro_cod_istat = d.PROV and c.com_cod_istat=d.COM) inner join ")
            StrSQL.AppendLine("         Lista_Province e on (d.PROV = e.PROV and d.COM=e.COM) ")
            StrSQL.AppendLine("     where reg<>'000' ")
            If ValiditaInizio <> AGRODATAINIZIO Then
                StrSQL.AppendLine(String.Format("     and b1.validita_inizio>={0} ", Agro_SQL_SaveDateTime(ValiditaInizio)))
            End If
            If ValiditaFine <> AGRODATAFINE Then
                StrSQL.AppendLine(String.Format("     and b1.validita_fine<={0} ", Agro_SQL_SaveDateTime(ValiditaFine)))
            End If
            StrSQL.AppendLine("     group by ")
            StrSQL.AppendLine("         b1.Validita_Inizio, ")
            StrSQL.AppendLine("         b1.Validita_Fine ")
            StrSQL.AppendLine("     union ")
            StrSQL.AppendLine("     select 'MAX' as t, ")
            StrSQL.AppendLine("         b1.validita_inizio, ")
            StrSQL.AppendLine("         b1.validita_fine, ")
            StrSQL.AppendLine("         (100100 + (YEAR(b1.validita_inizio)-2021)+2)*-1 as IndiciProduttivitaAi_COD, ")
            StrSQL.AppendLine("         cast(max(b1.Produttivita) as numeric(19,2)) as Produttivita, ")
            StrSQL.AppendLine("         cast(max(b1.PLV) as numeric(19,2)) as PLV, ")
            StrSQL.AppendLine("         cast(max(b1.IndiceErosione) as numeric(19,2)) as IndiceErosione, ")
            StrSQL.AppendLine("         cast(max(b1.IndiceCO2) as numeric(19,2)) as IndiceCO2, ")
            StrSQL.AppendLine("         100 as IndiceRischioMeteoAggregato, ")
            StrSQL.AppendLine("         null as IndiceRischioGelata, ")
            StrSQL.AppendLine("         null as IndiceRischioVentoForte, ")
            StrSQL.AppendLine("         null as IndiceRischioSiccita, ")
            StrSQL.AppendLine("         null as IndiceRischioGrandine, ")
            StrSQL.AppendLine("         null as IndiceRischioAllagamento ")
            StrSQL.AppendLine("     from  ")
            StrSQL.AppendLine("     Reg_Impianti_XIndiciProduttivitaAi a1 inner join ")
            StrSQL.AppendLine("     IndiciProduttivitaAi b1 on (a1.IndiciProduttivitaAi_COD=b1.IndiciProduttivitaAi_COD) inner join ")
            StrSQL.AppendLine("     Imprese a on (a1.piva=a.piva) inner join ")
            StrSQL.AppendLine("     ImpresexIndirizzi b on (a.PIVA=b.PIVA) inner join ")
            StrSQL.AppendLine("     Indirizzi c on (b.cod_indirizzo=c.cod_indirizzo) inner join ")
            StrSQL.AppendLine("     ISTAT d on (c.pro_cod_istat = d.PROV and c.com_cod_istat=d.COM) inner join ")
            StrSQL.AppendLine("     Lista_Province e on (d.PROV = e.PROV and d.COM=e.COM) ")
            StrSQL.AppendLine("     where reg<>'000' ")
            If ValiditaInizio <> AGRODATAINIZIO Then
                StrSQL.AppendLine(String.Format("       and  b1.validita_inizio>={0} ", Agro_SQL_SaveDateTime(ValiditaInizio)))
            End If
            If ValiditaFine <> AGRODATAFINE Then
                StrSQL.AppendLine(String.Format("    and  b1.validita_fine<={0} ", Agro_SQL_SaveDateTime(ValiditaFine)))
            End If
            StrSQL.AppendLine("     group by ")
            StrSQL.AppendLine("     b1.Validita_Inizio, ")
            StrSQL.AppendLine("     b1.Validita_Fine ")
            StrSQL.AppendLine(" ) as source on (source.IndiciProduttivitaAi_COD=dest.IndiciProduttivitaAi_COD) ")
            StrSQL.AppendLine(" when matched then ")
            StrSQL.AppendLine(" 	update set  ")
            StrSQL.AppendLine(" 		dest.Produttivita =source.Produttivita, ")
            StrSQL.AppendLine(" 		dest.PLV =source.PLV, ")
            StrSQL.AppendLine(" 		dest.IndiceErosione =source.IndiceErosione, ")
            StrSQL.AppendLine(" 		dest.IndiceCO2 =source.IndiceCO2, ")
            StrSQL.AppendLine(" 		dest.IndiceRischioMeteoAggregato =source.IndiceRischioMeteoAggregato, ")
            StrSQL.AppendLine(" 		dest.IndiceRischioGelata =source.IndiceRischioGelata, ")
            StrSQL.AppendLine(" 		dest.IndiceRischioVentoForte =source.IndiceRischioVentoForte, ")
            StrSQL.AppendLine(" 		dest.IndiceRischioSiccita =source.IndiceRischioSiccita, ")
            StrSQL.AppendLine(" 		dest.IndiceRischioGrandine =source.IndiceRischioGrandine, ")
            StrSQL.AppendLine(" 		dest.IndiceRischioAllagamento =source.IndiceRischioAllagamento, ")
            StrSQL.AppendLine(" 		dest.data_modifica =getdate(), ")
            StrSQL.AppendLine(String.Format(" 		dest.username_modifica='{0}' ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))
            StrSQL.AppendLine(" when not matched by target then ")
            StrSQL.AppendLine(" 	insert ( ")
            StrSQL.AppendLine(" 				IndiciProduttivitaAi_COD, ")
            StrSQL.AppendLine(" 				Produttivita, ")
            StrSQL.AppendLine(" 				PLV, ")
            StrSQL.AppendLine(" 				IndiceErosione, ")
            StrSQL.AppendLine(" 				IndiceCO2, ")
            StrSQL.AppendLine(" 				IndiceRischioMeteoAggregato, ")
            StrSQL.AppendLine(" 				IndiceRischioGelata, ")
            StrSQL.AppendLine(" 				IndiceRischioVentoForte, ")
            StrSQL.AppendLine(" 				IndiceRischioSiccita, ")
            StrSQL.AppendLine(" 				IndiceRischioGrandine, ")
            StrSQL.AppendLine(" 				IndiceRischioAllagamento, ")
            StrSQL.AppendLine(" 				inviato, ")
            StrSQL.AppendLine(" 				datainvio, ")
            StrSQL.AppendLine(" 				data_creazione, ")
            StrSQL.AppendLine(" 				data_modifica, ")
            StrSQL.AppendLine(" 				username_creazione, ")
            StrSQL.AppendLine(" 				username_modifica, ")
            StrSQL.AppendLine(" 				validita_inizio, ")
            StrSQL.AppendLine(" 				validita_fine ")
            StrSQL.AppendLine(" 			)values ")
            StrSQL.AppendLine("             ( ")
            StrSQL.AppendLine(" 				source.IndiciProduttivitaAi_COD, ")
            StrSQL.AppendLine(" 				source.Produttivita, ")
            StrSQL.AppendLine(" 				source.PLV, ")
            StrSQL.AppendLine(" 				source.IndiceErosione, ")
            StrSQL.AppendLine(" 				source.IndiceCO2, ")
            StrSQL.AppendLine(" 				source.IndiceRischioMeteoAggregato, ")
            StrSQL.AppendLine(" 				source.IndiceRischioGelata, ")
            StrSQL.AppendLine(" 				source.IndiceRischioVentoForte, ")
            StrSQL.AppendLine(" 				source.IndiceRischioSiccita, ")
            StrSQL.AppendLine(" 				source.IndiceRischioGrandine, ")
            StrSQL.AppendLine(" 				source.IndiceRischioAllagamento, ")
            StrSQL.AppendLine(" 				0, ")
            StrSQL.AppendLine(" 				null, ")
            StrSQL.AppendLine(" 				getdate(), ")
            StrSQL.AppendLine(" 				getdate(), ")
            StrSQL.AppendLine(String.Format(" 				'{0}', ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))
            StrSQL.AppendLine(String.Format(" 				'{0}', ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))
            StrSQL.AppendLine(" 				source.validita_inizio, ")
            StrSQL.AppendLine(" 				source.validita_fine ")
            StrSQL.AppendLine(" 			); ")

            StrSQL.AppendLine(" merge into Reg_Impianti_XIndiciProduttivitaAi dest using( ")
            StrSQL.AppendLine("     select  'MIN' as t,
                                            '-31' as piva, ")
            StrSQL.AppendLine("             b1.validita_inizio, ")
            StrSQL.AppendLine("             b1.validita_fine, ")
            StrSQL.AppendLine("             (100100 + (YEAR(b1.validita_inizio)-2021))*-1 as IndiciProduttivitaAi_COD ")
            StrSQL.AppendLine("     from  ")
            StrSQL.AppendLine("         Reg_Impianti_XIndiciProduttivitaAi a1 inner join ")
            StrSQL.AppendLine("         IndiciProduttivitaAi b1 on (a1.IndiciProduttivitaAi_COD=b1.IndiciProduttivitaAi_COD) inner join ")
            StrSQL.AppendLine("         Imprese a on (a1.piva=a.piva) inner join ")
            StrSQL.AppendLine("         ImpresexIndirizzi b on (a.PIVA=b.PIVA) inner join ")
            StrSQL.AppendLine("         Indirizzi c on (b.cod_indirizzo=c.cod_indirizzo) inner join ")
            StrSQL.AppendLine("         ISTAT d on (c.pro_cod_istat = d.PROV and c.com_cod_istat=d.COM) inner join ")
            StrSQL.AppendLine("         Lista_Province e on (d.PROV = e.PROV and d.COM=e.COM) ")
            StrSQL.AppendLine("     where reg<>'000' ")
            If ValiditaInizio <> AGRODATAINIZIO Then
                StrSQL.AppendLine(String.Format("     and b1.validita_inizio>={0} ", Agro_SQL_SaveDateTime(ValiditaInizio)))
            End If
            If ValiditaFine <> AGRODATAFINE Then
                StrSQL.AppendLine(String.Format("     and b1.validita_fine<={0} ", Agro_SQL_SaveDateTime(ValiditaFine)))
            End If

            StrSQL.AppendLine("     group by ")
            StrSQL.AppendLine("         b1.Validita_Inizio, ")
            StrSQL.AppendLine("         b1.Validita_Fine ")
            StrSQL.AppendLine("     union ")
            StrSQL.AppendLine("     select  'MAX' as t,
                                            '-32' as piva, ")
            StrSQL.AppendLine("             b1.validita_inizio, ")
            StrSQL.AppendLine("             b1.validita_fine, ")
            StrSQL.AppendLine("             (100100 + (YEAR(b1.validita_inizio)-2021)+2)*-1 as IndiciProduttivitaAi_COD ")
            StrSQL.AppendLine("     from  ")
            StrSQL.AppendLine("         Reg_Impianti_XIndiciProduttivitaAi a1 inner join ")
            StrSQL.AppendLine("         IndiciProduttivitaAi b1 on (a1.IndiciProduttivitaAi_COD=b1.IndiciProduttivitaAi_COD) inner join ")
            StrSQL.AppendLine("         Imprese a on (a1.piva=a.piva) inner join ")
            StrSQL.AppendLine("         ImpresexIndirizzi b on (a.PIVA=b.PIVA) inner join ")
            StrSQL.AppendLine("         Indirizzi c on (b.cod_indirizzo=c.cod_indirizzo) inner join ")
            StrSQL.AppendLine("         ISTAT d on (c.pro_cod_istat = d.PROV and c.com_cod_istat=d.COM) inner join ")
            StrSQL.AppendLine("         Lista_Province e on (d.PROV = e.PROV and d.COM=e.COM) ")
            StrSQL.AppendLine("     where reg<>'000' ")
            If ValiditaInizio <> AGRODATAINIZIO Then
                StrSQL.AppendLine(String.Format("     and b1.validita_inizio>={0} ", Agro_SQL_SaveDateTime(ValiditaInizio)))
            End If
            If ValiditaFine <> AGRODATAFINE Then
                StrSQL.AppendLine(String.Format("     and b1.validita_fine<={0} ", Agro_SQL_SaveDateTime(ValiditaFine)))
            End If

            StrSQL.AppendLine("     group by ")
            StrSQL.AppendLine("         b1.Validita_Inizio, ")
            StrSQL.AppendLine("         b1.Validita_Fine ")

            StrSQL.AppendLine(" ) as source on (source.IndiciProduttivitaAi_COD=dest.IndiciProduttivitaAi_COD) ")
            StrSQL.AppendLine(" when not matched by target then ")
            StrSQL.AppendLine(" 	insert ( ")
            StrSQL.AppendLine(" 				IndiciProduttivitaAi_COD, ")
            StrSQL.AppendLine(" 				piva, ")
            StrSQL.AppendLine(" 				sa_cod, ")
            StrSQL.AppendLine(" 				appezza, ")
            StrSQL.AppendLine(" 				id_reg, ")
            StrSQL.AppendLine(" 				inviato, ")
            StrSQL.AppendLine(" 				datainvio, ")
            StrSQL.AppendLine(" 				data_creazione, ")
            StrSQL.AppendLine(" 				data_modifica, ")
            StrSQL.AppendLine(" 				username_creazione, ")
            StrSQL.AppendLine(" 				username_modifica, ")
            StrSQL.AppendLine(" 				validita_inizio, ")
            StrSQL.AppendLine(" 				validita_fine ")
            StrSQL.AppendLine(" 			)values ")
            StrSQL.AppendLine("             ( ")
            StrSQL.AppendLine(" 				source.IndiciProduttivitaAi_COD, ")
            StrSQL.AppendLine(" 				source.piva, ")
            StrSQL.AppendLine(" 				0, ")
            StrSQL.AppendLine(" 				0, ")
            StrSQL.AppendLine(" 				0, ")
            StrSQL.AppendLine(" 				0, ")
            StrSQL.AppendLine(" 				null, ")
            StrSQL.AppendLine(" 				getdate(), ")
            StrSQL.AppendLine(" 				getdate(), ")
            StrSQL.AppendLine(String.Format(" 				'{0}', ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))
            StrSQL.AppendLine(String.Format(" 				'{0}', ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))
            StrSQL.AppendLine(" 				source.validita_inizio, ")
            StrSQL.AppendLine(" 				source.validita_fine ")
            StrSQL.AppendLine(" 			); ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try
        Return xRisp
    End Function

    Public Function RicalcolaMediaGenerale(ByVal ValiditaInizio As DateTime,
                                           ByVal ValiditaFine As DateTime,
                                           ByRef objParametri As AgronicaCoreParametri) As Boolean
        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.IndiciProduttivitaAi_W.RicalcolaMediaGenerale()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" merge into IndiciProduttivitaAi dest using( ")
            StrSQL.AppendLine("     select 'MED' as t, ")
            StrSQL.AppendLine("     	b1.validita_inizio, ")
            StrSQL.AppendLine("     	b1.validita_fine, ")
            StrSQL.AppendLine("     	(100000 + (YEAR(b1.validita_inizio)-2021))*-1 as IndiciProduttivitaAi_COD, ")
            StrSQL.AppendLine("         cast(sum(b1.Produttivita*r.Sup_Imp)/sum(r.Sup_Imp) as numeric(19,2)) as Produttivita, ")
            StrSQL.AppendLine("         cast(sum(b1.PLV*r.Sup_Imp)/sum(r.Sup_Imp) as numeric(19,2)) as PLV, ")
            StrSQL.AppendLine("         cast(sum(b1.IndiceErosione*r.Sup_Imp)/sum(r.Sup_Imp) as numeric(19,2)) as IndiceErosione, ")
            StrSQL.AppendLine("         cast(sum(b1.IndiceCO2*r.Sup_Imp)/sum(r.Sup_Imp) as numeric(19,2)) as IndiceCO2, ")
            StrSQL.AppendLine("         cast(sum(b1.IndiceRischioMeteoAggregato*r.Sup_Imp)/sum(r.Sup_Imp) as numeric(19,2)) as IndiceRischioMeteoAggregato, ")
            StrSQL.AppendLine("         cast(sum(b1.IndiceRischioGelata*r.Sup_Imp)/sum(r.Sup_Imp) as numeric(19,2)) as IndiceRischioGelata, ")
            StrSQL.AppendLine("         cast(sum(b1.IndiceRischioVentoForte*r.Sup_Imp)/sum(r.Sup_Imp) as numeric(19,2)) as IndiceRischioVentoForte, ")
            StrSQL.AppendLine("         cast(sum(b1.IndiceRischioSiccita*r.Sup_Imp)/sum(r.Sup_Imp) as numeric(19,2)) as IndiceRischioSiccita, ")
            StrSQL.AppendLine("         cast(sum(b1.IndiceRischioGrandine*r.Sup_Imp)/sum(r.Sup_Imp) as numeric(19,2)) as IndiceRischioGrandine, ")
            StrSQL.AppendLine("         cast(sum(b1.IndiceRischioAllagamento*r.Sup_Imp)/sum(r.Sup_Imp) as numeric(19,2)) as IndiceRischioAllagamento ")
            StrSQL.AppendLine("     from  ")
            StrSQL.AppendLine("         Reg_Impianti_XIndiciProduttivitaAi a1 inner join ")
            StrSQL.AppendLine("         IndiciProduttivitaAi b1 on (a1.IndiciProduttivitaAi_COD=b1.IndiciProduttivitaAi_COD) inner join ")
            StrSQL.AppendLine("         Imprese a on (a1.piva=a.piva) inner join ")
            StrSQL.AppendLine("         ImpresexIndirizzi b on (a.PIVA=b.PIVA) inner join ")
            StrSQL.AppendLine("         Indirizzi c on (b.cod_indirizzo=c.cod_indirizzo) inner join ")
            StrSQL.AppendLine("         ISTAT d on (c.pro_cod_istat = d.PROV and c.com_cod_istat=d.COM) inner join ")
            StrSQL.AppendLine("         Lista_Province e on (d.PROV = e.PROV and d.COM=e.COM) inner join ")
            StrSQL.AppendLine("         Reg_Impianti r on (a1.piva=r.PIVA and a1.sa_cod=r.SA_COD and a1.appezza=r.APPEZZA and a1.id_reg=r.id_reg ) ")
            StrSQL.AppendLine("     where reg<>'000' ")
            If ValiditaInizio <> AGRODATAINIZIO Then
                StrSQL.AppendLine(String.Format("     and b1.validita_inizio>={0} ", Agro_SQL_SaveDateTime(ValiditaInizio)))
            End If
            If ValiditaFine <> AGRODATAFINE Then
                StrSQL.AppendLine(String.Format("     and b1.validita_fine<={0} ", Agro_SQL_SaveDateTime(ValiditaFine)))
            End If
            StrSQL.AppendLine("     group by ")
            StrSQL.AppendLine("         b1.Validita_Inizio, ")
            StrSQL.AppendLine("         b1.Validita_Fine ")
            StrSQL.AppendLine(" ) as source on (source.IndiciProduttivitaAi_COD=dest.IndiciProduttivitaAi_COD) ")
            StrSQL.AppendLine(" when matched then ")
            StrSQL.AppendLine(" 	update set  ")
            StrSQL.AppendLine(" 		dest.Produttivita =source.Produttivita, ")
            StrSQL.AppendLine(" 		dest.PLV =source.PLV, ")
            StrSQL.AppendLine(" 		dest.IndiceErosione =source.IndiceErosione, ")
            StrSQL.AppendLine(" 		dest.IndiceCO2 =source.IndiceCO2, ")
            StrSQL.AppendLine(" 		dest.IndiceRischioMeteoAggregato =source.IndiceRischioMeteoAggregato, ")
            StrSQL.AppendLine(" 		dest.IndiceRischioGelata =source.IndiceRischioGelata, ")
            StrSQL.AppendLine(" 		dest.IndiceRischioVentoForte =source.IndiceRischioVentoForte, ")
            StrSQL.AppendLine(" 		dest.IndiceRischioSiccita =source.IndiceRischioSiccita, ")
            StrSQL.AppendLine(" 		dest.IndiceRischioGrandine =source.IndiceRischioGrandine, ")
            StrSQL.AppendLine(" 		dest.IndiceRischioAllagamento =source.IndiceRischioAllagamento, ")
            StrSQL.AppendLine(" 		dest.data_modifica =getdate(), ")
            StrSQL.AppendLine(String.Format(" 		dest.username_modifica='{0}' ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))
            StrSQL.AppendLine(" when not matched by target then ")
            StrSQL.AppendLine(" 	insert ( ")
            StrSQL.AppendLine(" 				IndiciProduttivitaAi_COD, ")
            StrSQL.AppendLine(" 				Produttivita, ")
            StrSQL.AppendLine(" 				PLV, ")
            StrSQL.AppendLine(" 				IndiceErosione, ")
            StrSQL.AppendLine(" 				IndiceCO2, ")
            StrSQL.AppendLine(" 				IndiceRischioMeteoAggregato, ")
            StrSQL.AppendLine(" 				IndiceRischioGelata, ")
            StrSQL.AppendLine(" 				IndiceRischioVentoForte, ")
            StrSQL.AppendLine(" 				IndiceRischioSiccita, ")
            StrSQL.AppendLine(" 				IndiceRischioGrandine, ")
            StrSQL.AppendLine(" 				IndiceRischioAllagamento, ")
            StrSQL.AppendLine(" 				inviato, ")
            StrSQL.AppendLine(" 				datainvio, ")
            StrSQL.AppendLine(" 				data_creazione, ")
            StrSQL.AppendLine(" 				data_modifica, ")
            StrSQL.AppendLine(" 				username_creazione, ")
            StrSQL.AppendLine(" 				username_modifica, ")
            StrSQL.AppendLine(" 				validita_inizio, ")
            StrSQL.AppendLine(" 				validita_fine ")
            StrSQL.AppendLine(" 			)values ")
            StrSQL.AppendLine("             ( ")
            StrSQL.AppendLine(" 				source.IndiciProduttivitaAi_COD, ")
            StrSQL.AppendLine(" 				source.Produttivita, ")
            StrSQL.AppendLine(" 				source.PLV, ")
            StrSQL.AppendLine(" 				source.IndiceErosione, ")
            StrSQL.AppendLine(" 				source.IndiceCO2, ")
            StrSQL.AppendLine(" 				source.IndiceRischioMeteoAggregato, ")
            StrSQL.AppendLine(" 				source.IndiceRischioGelata, ")
            StrSQL.AppendLine(" 				source.IndiceRischioVentoForte, ")
            StrSQL.AppendLine(" 				source.IndiceRischioSiccita, ")
            StrSQL.AppendLine(" 				source.IndiceRischioGrandine, ")
            StrSQL.AppendLine(" 				source.IndiceRischioAllagamento, ")
            StrSQL.AppendLine(" 				0, ")
            StrSQL.AppendLine(" 				null, ")
            StrSQL.AppendLine(" 				getdate(), ")
            StrSQL.AppendLine(" 				getdate(), ")
            StrSQL.AppendLine(String.Format(" 				'{0}', ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))
            StrSQL.AppendLine(String.Format(" 				'{0}', ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))
            StrSQL.AppendLine(" 				source.validita_inizio, ")
            StrSQL.AppendLine(" 				source.validita_fine ")
            StrSQL.AppendLine(" 			); ")

            StrSQL.AppendLine(" merge into Reg_Impianti_XIndiciProduttivitaAi dest using( ")
            StrSQL.AppendLine("     select  'MED' as t,
                                            '-30' as piva, ")
            StrSQL.AppendLine("             b1.validita_inizio, ")
            StrSQL.AppendLine("             b1.validita_fine, ")
            StrSQL.AppendLine("             (100000 + (YEAR(b1.validita_inizio)-2021))*-1 as IndiciProduttivitaAi_COD ")
            StrSQL.AppendLine("     from  ")
            StrSQL.AppendLine("         Reg_Impianti_XIndiciProduttivitaAi a1 inner join ")
            StrSQL.AppendLine("         IndiciProduttivitaAi b1 on (a1.IndiciProduttivitaAi_COD=b1.IndiciProduttivitaAi_COD) inner join ")
            StrSQL.AppendLine("         Imprese a on (a1.piva=a.piva) inner join ")
            StrSQL.AppendLine("         ImpresexIndirizzi b on (a.PIVA=b.PIVA) inner join ")
            StrSQL.AppendLine("         Indirizzi c on (b.cod_indirizzo=c.cod_indirizzo) inner join ")
            StrSQL.AppendLine("         ISTAT d on (c.pro_cod_istat = d.PROV and c.com_cod_istat=d.COM) inner join ")
            StrSQL.AppendLine("         Lista_Province e on (d.PROV = e.PROV and d.COM=e.COM) ")
            StrSQL.AppendLine("     where reg<>'000' ")
            If ValiditaInizio <> AGRODATAINIZIO Then
                StrSQL.AppendLine(String.Format("     and b1.validita_inizio>={0} ", Agro_SQL_SaveDateTime(ValiditaInizio)))
            End If
            If ValiditaFine <> AGRODATAFINE Then
                StrSQL.AppendLine(String.Format("     and b1.validita_fine<={0} ", Agro_SQL_SaveDateTime(ValiditaFine)))
            End If

            StrSQL.AppendLine("     group by ")
            StrSQL.AppendLine("         b1.Validita_Inizio, ")
            StrSQL.AppendLine("         b1.Validita_Fine ")
            StrSQL.AppendLine(" ) as source on (source.IndiciProduttivitaAi_COD=dest.IndiciProduttivitaAi_COD) ")
            StrSQL.AppendLine(" when not matched by target then ")
            StrSQL.AppendLine(" 	insert ( ")
            StrSQL.AppendLine(" 				IndiciProduttivitaAi_COD, ")
            StrSQL.AppendLine(" 				piva, ")
            StrSQL.AppendLine(" 				sa_cod, ")
            StrSQL.AppendLine(" 				appezza, ")
            StrSQL.AppendLine(" 				id_reg, ")
            StrSQL.AppendLine(" 				inviato, ")
            StrSQL.AppendLine(" 				datainvio, ")
            StrSQL.AppendLine(" 				data_creazione, ")
            StrSQL.AppendLine(" 				data_modifica, ")
            StrSQL.AppendLine(" 				username_creazione, ")
            StrSQL.AppendLine(" 				username_modifica, ")
            StrSQL.AppendLine(" 				validita_inizio, ")
            StrSQL.AppendLine(" 				validita_fine ")
            StrSQL.AppendLine(" 			)values ")
            StrSQL.AppendLine("             ( ")
            StrSQL.AppendLine(" 				source.IndiciProduttivitaAi_COD, ")
            StrSQL.AppendLine(" 				source.piva, ")
            StrSQL.AppendLine(" 				0, ")
            StrSQL.AppendLine(" 				0, ")
            StrSQL.AppendLine(" 				0, ")
            StrSQL.AppendLine(" 				0, ")
            StrSQL.AppendLine(" 				null, ")
            StrSQL.AppendLine(" 				getdate(), ")
            StrSQL.AppendLine(" 				getdate(), ")
            StrSQL.AppendLine(String.Format(" 				'{0}', ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))
            StrSQL.AppendLine(String.Format(" 				'{0}', ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))
            StrSQL.AppendLine(" 				source.validita_inizio, ")
            StrSQL.AppendLine(" 				source.validita_fine ")
            StrSQL.AppendLine(" 			); ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try
        Return xRisp
    End Function
End Class

