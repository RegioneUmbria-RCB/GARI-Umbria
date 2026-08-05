Imports System.Text
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class Contabilita_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Private ReadOnly _lavCodProtVendite As Integer() = {LAVCOD_FATTURA_EMESSA,
                                                        LAVCOD_NOTA_ACCREDITO_EMESSA,
                                                        LAVCOD_FATTURA_LIQ_CONF_EMESSA,
                                                        LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA}

    Private ReadOnly _lavCodProtAcquisti As Integer() = {LAVCOD_FATTURA_RICEVUTA,
                                                         LAVCOD_NOTA_ACCREDITO_RICEVUTA,
                                                         LAVCOD_FATTURA_LIQ_CONF_RICEVUTA,
                                                         LAVCOD_AUTOFATTURA_LIQ_CONF_EMESSA,
                                                         LAVCOD_FATTURA_PROFESSIONISTI}

    Private ReadOnly _cauMovProtDocContab As String() = {CAU_REGISTRAZIONI}

    Private ReadOnly _lavCodProtMagazzino As Integer() = {LAVCOD_CARICO, LAVCOD_SCARICO}

    Private ReadOnly _cauMovProtMagazzino As String() = {CAU_CARICO, CAU_SCARICO}

    Public Sub New()
        MyBase.New()
    End Sub
    '##############################################################################################

#Region "Euresys"

    ''' <summary>
    ''' Legge il contenuto della tabella Codici_Attivita in GIAS INTERSCAMBIO
    ''' </summary>
    ''' <param name="piva">Piva della nostra installazione(opzionale = "")</param>
    ''' <param name="idAttivita">(opzionale = 0)</param>
    ''' <param name="tipo">Tipologia della risorsa: utilizzare le costanti INTERSCAMBIO_ATTIVITA_* (opzionale = "")</param>
    ''' <param name="codAttivita_GIAS">(opzionale = "")</param>
    ''' <param name="xFiltroAggiuntivo">(opzionale = "")</param>
    ''' <param name="xOrderBy">(opzionale = "")</param>
    ''' <param name="objParametri"></param>
    ''' <param name="suffissoColonnaAltro">suffisso sul nome della colonna per l'altro gestionale (ad es: "SAP" che verrà aggiunto a "Cod_Attivita_" (se non specificato = "ALTRO")</param>
    ''' <param name="dataValidita">restituisce solo i valori validi a quella data</param>
    ''' <returns>DataTable contenente i dati della tabella</returns>
    ''' <remarks>Per il tipo è possibile utilizzare le costanti personalizzate che cominciano per INTERSCAMBIO_ATTIVITA_*</remarks>
    Public Function LeggiCausaliEuresys(ByVal piva As String,
                                        ByVal idAttivita As Integer,
                                        ByVal tipo As String,
                                        ByVal codAttivita_GIAS As Integer,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        Optional ByVal suffissoColonnaAltro As String = "ALTRO",
                                        Optional ByVal dataValidita As Date = #2/1/1900#
                                        ) As DataTable

        '====================================================================================
        'Parametri opzionali :
        '   piva = ""
        '   idAttivita = 0
        '   tipo = ""
        '   codAttivita_GIAS = 0
        '   codAttivita_ALTRO = ""
        '   codCentroLavoro_ALTRO = ""
        '====================================================================================

        Dim nomeRoutine As String = "Contabilita.LeggiCausaliEuresys()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM Codici_Attivita_Euresys ")
            stb.AppendLine(" WHERE 1 = 1 ")


            If piva <> "" Then
                stb.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            If idAttivita <> 0 Then
                stb.AppendLine(" AND Id_Attivita = " & Agro_SQL_SaveNum(idAttivita) & " ")
            End If

            If tipo <> "" Then
                stb.AppendLine(" AND Tipo = '" & Agro_SQL_SaveText(tipo) & "' ")
            End If

            If codAttivita_GIAS <> 0 Then
                stb.AppendLine(" AND Cod_Attivita_GIAS = " & Agro_SQL_SaveNum(codAttivita_GIAS) & " ")
            End If

            If dataValidita <> #2/1/1900# Then
                stb.AppendLine(" AND Validita_Inzio <= " & Agro_SQL_SaveDate(dataValidita) & " ")
                stb.AppendLine(" AND Validita_Fine >= " & Agro_SQL_SaveDate(dataValidita) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    Public Function LeggiOreEuresys(
        ByVal Piva As String,
        ByVal Data_Inizio As Date,
        ByVal Data_Fine As Date,
        ByVal Risorse As String,
        ByVal Verso As String,
        ByVal ConsideraOreEsportate As Boolean,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "LeggiOreEuresys()"
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            Dim data_riferimento As String = "data_inserimento"
            If Not String.IsNullOrEmpty(Verso) Then
                data_riferimento = If(Verso = "1", "data_ora_inizio", "data_ora_fine")
            End If

            '---------------------------------------------
            Stb.AppendLine("select a.id_cdg, a.id_agenda, a.cod_risum, a.Piva, b.rag_soc as Azienda, ")
            Stb.AppendLine("e.cognome + ' '+ e.nome as Risorsa, e.NrBadge, Descrizione, ")
            Stb.AppendLine("a.Id_Attivita, c.[desc] as Attivita, c.sigla, Data_Inserimento, ")
            Stb.AppendLine("convert(varchar(10), data_ora_inizio, 103) as DataInizio, convert(varchar(5), data_ora_inizio, 108) as OraInizio, ")
            Stb.AppendLine("convert(varchar(10), data_ora_fine, 103) as DataFine, convert(varchar(5), data_ora_fine, 108) as OraFine, qta as Ore, ")
            Stb.AppendLine("format(data_ora_inizio, 'ddMMyyHHmm') as DataInizioEuresys, format(data_ora_fine, 'ddMMyyHHmm') as DataFineEuresys, ")
            Stb.AppendLine("CASE WHEN OrigineApp<3 THEN 'NO' ELSE 'SI' END as Esportato, OrigineApp, prezzo_unitario, valore_totale ")
            Stb.AppendLine("from cdg_testata a inner join imprese b on a.piva = b.piva ")
            Stb.AppendLine("inner join attivita c on a.Id_Attivita = c.ID_Attivita ")
            Stb.AppendLine("inner join agenda ag on a.piva = ag.piva and a.id_agenda = ag.id_agenda ")
            Stb.AppendLine("inner join risorse_umane d on a.cod_risum = d.cod_risum ")
            Stb.AppendLine("inner join contatti e on d.piva = e.piva and d.cod_contatto = e.cod_contatto ")
            Stb.AppendLine("where a.Budget = 0 And cast(" & data_riferimento & " as Date) >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")
            Stb.AppendLine("and cast(" & data_riferimento & " as Date) <= " & Agro_SQL_SaveDate(Data_Fine) & " and ag.Split=0 ")

            ' filtro su piva azienda
            If Not String.IsNullOrEmpty(Piva) Then
                Stb.Append(" and a.piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            ' filtro su risorse umane
            If Not String.IsNullOrEmpty(Risorse) Then
                Stb.Append(" and a.cod_risum IN (" & Agro_SQL_Save_Clausola_IN(Replace(Risorse, "|", ",")) & ") ")
            End If

            ' escludi ore già esportate
            If Not ConsideraOreEsportate Then
                Stb.Append(" and a.OrigineApp IN (0,1,2) ")
            End If

            Stb.Append("order by b.rag_soc, a.piva, cognome, nome, data_ora_inizio, data_ora_fine")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return dt

    End Function

#End Region

    '##############################################################################################

#Region "Zespri"

    Public Sub Agenda_Lettura_Note_Interventi(ByVal gruppo_cod As Integer, ByVal strAlias As String, ByVal pivaSuper_User As String, ByRef stb As StringBuilder)

        stb.AppendLine(" LEFT JOIN ( ")
        stb.AppendLine(" select  ")
        stb.AppendLine("      min (ni_An_" & strAlias & ".Nota_Cod) as Nota_Cod ")
        stb.AppendLine("    , (select Nota_Des from  Note_Intervento N1_a_" & strAlias & " where N1_a_" & strAlias & ".Nota_Cod =  min (ni_An_" & strAlias & ".Nota_Cod)) as Nota_DES ")
        stb.AppendLine("    , (select Note_Valore_Numerico from  Note_Intervento N2_a_" & strAlias & " where N2_a_" & strAlias & ".Nota_Cod =  min (ni_An_" & strAlias & ".Nota_Cod)) as Note_Valore_Numerico ")
        stb.AppendLine("    , (select Note_Valore_Stringa from  Note_Intervento N3_a_" & strAlias & " where N3_a_" & strAlias & ".Nota_Cod =  min (ni_An_" & strAlias & ".Nota_Cod)) as Note_Valore_Stringa ")
        stb.AppendLine("    , Id_Agenda ")
        stb.AppendLine(" from AgendaxNote AN_" & strAlias & " ")
        stb.AppendLine("    inner join Note_Intervento ni_An_" & strAlias & " ")
        stb.AppendLine("    on AN_" & strAlias & ".Nota_Cod = ni_An_" & strAlias & ".Nota_cod ")
        stb.AppendLine(" where Piva_SuperUser = '" & pivaSuper_User & "' ")
        stb.AppendLine(" and NotaGruppo_Cod = " & gruppo_cod & " ")
        stb.AppendLine("    group by AN_" & strAlias & ".Id_Agenda, ni_An_" & strAlias & ".NotaGruppo_Cod  ")
        stb.AppendLine(" ) Ni_" & strAlias & " ")
        stb.AppendLine(" on Ni_" & strAlias & ".id_agenda = A.Id_Agenda  ")

    End Sub

    Public Sub Operazioni_EsportazioneZespri_Query_Fiore(ByVal strFF_Cod As String, ByRef stb As StringBuilder)

        stb.AppendLine(", ISNULL( ")
        stb.AppendLine("            ( ")
        stb.AppendLine("                SELECT TOP 1 CONVERT(varchar,DestFior.validita_inizio,103) AS validita_inizio   ")
        stb.AppendLine("                FROM  Mov_Dettaglio_Tecnico TecFior  ")
        stb.AppendLine("                    INNER JOIN Movimenti_dettagli DFior ")
        stb.AppendLine("                        ON TecFior.Piva = DFior.PIVA  ")
        stb.AppendLine("                        AND TecFior.Sa_Cod = DFior.Sa_Cod  ")
        stb.AppendLine("                        AND TecFior.Id_Agenda = DFior.Id_Agenda  ")
        stb.AppendLine("                        AND TecFior.Id_Mov = DFior.Id_Mov  ")
        stb.AppendLine("                        AND TecFior.Id_Mov_Det = DFior.Id_Mov_Det  ")
        stb.AppendLine("                    INNER JOIN  Movimenti MFior ")
        stb.AppendLine("                    ON DFior.PIVA = MFior.PIVA  ")
        stb.AppendLine("                        AND DFior.Sa_Cod = MFior.Sa_Cod  ")
        stb.AppendLine("                        AND DFior.Id_Agenda = MFior.Id_Agenda  ")
        stb.AppendLine("                        AND DFior.Id_Mov = MFior.Id_Mov  ")
        stb.AppendLine("                    INNER JOIN  Agenda AFior  ")
        stb.AppendLine("                        ON MFior.PIVA = AFior.PIVA  ")
        stb.AppendLine("                        AND MFior.Sa_Cod = AFior.Sa_Cod  ")
        stb.AppendLine("                        AND MFior.Id_Agenda = AFior.Id_Agenda  ")
        stb.AppendLine("  ")
        stb.AppendLine("                    INNER JOIN Mov_Destinazioni DestFior  ")
        stb.AppendLine("                        ON DFior.PIVA = DestFior.Piva  ")
        stb.AppendLine("                        AND DFior.Sa_Cod = DestFior.Sa_Cod  ")
        stb.AppendLine("                        AND DFior.Id_Agenda = DestFior.Id_Agenda  ")
        stb.AppendLine("                        AND DFior.Id_Mov = DestFior.Id_Mov  ")
        stb.AppendLine("                        AND DFior.Id_Mov_Det = DestFior.Id_Mov_Det    ")
        stb.AppendLine("  ")
        stb.AppendLine("                    WHERE     AFior.Lav_Cod = 79 ")
        stb.AppendLine("                    AND MFior.Cau_Mov = '2100' ")
        stb.AppendLine("                    AND DestFior.PIVA = imp.PIVA    ")
        stb.AppendLine("                    AND DestFior.Sa_Cod = imp.sa_cod   ")
        stb.AppendLine("                    AND DestFior.APPEZZA = imp.APPEZZA   ")
        stb.AppendLine("                    AND DestFior.ID_destinazione = imp.ID_REG   ")
        stb.AppendLine("                    AND TecFior.FF_Classe IN (" & Agro_SQL_Save_Clausola_IN(strFF_Cod) & ") ")
        stb.AppendLine("                    ) ")
        stb.AppendLine("                , '') as [Flowering date]   ")

    End Sub

    Public Sub Operazioni_EsportazioneZespri_Query(
        ByVal RichiediPraticaValida As Boolean,
        ByVal Stato_Export As Integer,
        ByVal cifreArrotondamento As Integer,
        ByVal Esportazione_Elem_cod As Integer,
        ByVal DataInizio As DateTime,
        ByVal DataFine As DateTime,
        ByVal PivaPadreInGerarchia As String,
        ByVal kpin As Integer,
        ByVal strFF_Cod As String,
        ByVal FormatoEsportazioneZespri As enum_FormatoZespriExport,
        ByVal xFiltroAggiuntivo As String,
        ByVal ObJParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
        ByRef stb As StringBuilder,
        Optional ByVal bExport As Boolean = False)

        Dim servizio_cod As Integer = 20

        Dim YearOfHarvest As Integer
        YearOfHarvest = DataFine.Year

        Dim PivaSuperUSer_Zespri As String
        PivaSuperUSer_Zespri = ObJParametri.PivaSuperUser


        Const VentoDirezione As String = "VentoDirezione"
        Const VentoIntensita As String = "VentoIntensita"
        Const Meteo As String = "Meteo"
        Const Orario As String = "Orario"
        Const Motivo As String = "Motivo"
        Const Temp As String = "Temp"

        stb.AppendLine(" SELECT TOP 100 Percent ")
        stb.AppendLine("  ")
        stb.AppendLine("     I.Rag_Soc as [Grower]")
        stb.AppendLine("  , case when stato_export = 0 then '' else 'G2C' +SUBSTRING(cast(Stato_Export As varchar(50)), 7, 2) +  ")
        stb.AppendLine("  SUBSTRING(cast(Stato_Export As varchar(50)), 5, 2) +     ")
        stb.AppendLine("  SUBSTRING(cast(Stato_Export As varchar(50)), 1, 4) +  ")
        stb.AppendLine("  + RIGHT('000' +  CAST(Stato_Export_2 as varchar(5)), 3) ")
        stb.AppendLine("    end as [ExportID]   ")

        stb.AppendLine(" , (Isnull(G2C_Log.Return_Code, '') + Isnull(G2C_Log.Return_Error, '')) as InfoZespri ")

        stb.AppendLine("   , sDet.kpin as [KPIN]   ")
        stb.AppendLine("   , sDet.blockName as [BlockName]   ")
        stb.AppendLine("   , case when sDet.CUL_COD in (5012170, 5011458) then 'GA' else case when sDet.CUL_COD in (5000009, 7512) then 'GK' else '' end end as [BlockVar]     ") 'Altre cultivar ???
        stb.AppendLine("   , case when sDet.Regolamento_Cod = 4 then 'OB' else  'CK' end as [BlockGM]  ")
        stb.AppendLine("   , cast(mov.data_movimento AS DATE) as [Date]   ")
        stb.AppendLine("   , ISNULL(ISNULL(ni_" & Orario & ".Note_Valore_Stringa, ni_" & Orario & ".Nota_Des ), '10:00:00') as [Time]   ")
        stb.AppendLine("   , 'Town Supply1'as [WaterName]   ")

        If Esportazione_Elem_cod = FORMULATI Then
            stb.AppendLine("   , " & Esportazione_Elem_cod & " as [Agronica_Elem_cod]   ")
            stb.AppendLine("   , F.fr_Cod as [Agronica_cod_Prodotto]   ")
            stb.AppendLine("   , Mov_Det_Prodotto.PrincipiAttivi as [Agronica_cod_PrincipiAttivi]   ")

            If FormatoEsportazioneZespri = enum_FormatoZespriExport.Zespri2016 Then
                stb.AppendLine("   , F.fr_des as [Product]   ")
            Else
                stb.AppendLine("   , ISNULL( CAC.Desc_Prodotto_Cliente, 'Other') as [Product]   ")
            End If


        Else
            stb.AppendLine("   , " & Esportazione_Elem_cod & " as [Agronica_Elem_cod]   ")
            stb.AppendLine("   , F.fer_Cod as [Agronica_cod_Prodotto]   ")
            stb.AppendLine("   , '' as [Agronica_cod_PrincipiAttivi]   ")

            If FormatoEsportazioneZespri = enum_FormatoZespriExport.Zespri2016 Then
                stb.AppendLine("   , F.fer_des as [Product]   ")
            Else
                stb.AppendLine("   , ISNULL( CAC.Desc_Prodotto_Cliente, 'Other') as [Product]   ")
            End If


        End If

        If FormatoEsportazioneZespri = enum_FormatoZespriExport.Zespri2016 Then
            stb.AppendLine("   , '' as [Principio_Attivo]   ")
            stb.AppendLine("   , '' as [UnlistedProduct]   ")
            stb.AppendLine("   , '' as [Rate]  ")
            stb.AppendLine("   , '' as [ProductType]  --Letto da Web Service") 'web service        
        End If


        If FormatoEsportazioneZespri = enum_FormatoZespriExport.Zespri2018 Then

            If Esportazione_Elem_cod = FORMULATI Then
                stb.AppendLine("   , case when CAC.Desc_Prodotto_Cliente is null then F.Fr_Des else '' end as [UnlistedProduct]   ")
            Else
                stb.AppendLine("   , case when CAC.Desc_Prodotto_Cliente is null then F.Fer_Des else '' end as [UnlistedProduct]   ")
            End If

            ' VAnni: 9/9/2020: anche sui trattamenti considero la possibile assenza di acqua.
            'If Esportazione_Elem_cod = 191 Then
            '    stb.AppendLine("   , Mov_Det_Prodotto.Qta_Extra as [Rate]  ")
            'Else
            '    stb.AppendLine("   , case when tec.Qta_Ril is null or tec.Qta_Ril = 0 then Mov_Det_Prodotto.Qta else  Mov_Det_Prodotto.Qta_Extra end as [Rate]   ")
            'End If

            stb.AppendLine("   , case when tec.Qta_Ril is null or tec.Qta_Ril = 0 then Mov_Det_Prodotto.Qta else  Mov_Det_Prodotto.Qta_Extra end as [Rate]   ")

            stb.AppendLine("   , '' as [ProductType]  --Letto da Web Service")
            stb.AppendLine("   , Mov_Det_Prodotto.Extra_Int as udm_Dose")
        End If

        stb.AppendLine("   , ISNULL( ISNULL(ni_" & Motivo & ".Note_Valore_Stringa, ni_" & Motivo & ".Nota_Des ), '') as [Reason] ")
        stb.AppendLine("   , '' as [Contractor]   ")
        stb.AppendLine("   , 9999 as [AppGSafe]   ")
        stb.AppendLine("   , 'Sprayer' as [Equipment]   ")
        stb.AppendLine("   , '' as [AINozzle]   ")

        'se 2016 allora imposta qta distribuita su impianto, altrimenti Acqua
        If FormatoEsportazioneZespri = enum_FormatoZespriExport.Zespri2016 Then
            stb.AppendLine("   , Dest.qta as [Litres]   ")
        Else
            'acqua complessiva come in agronica...
            'stb.AppendLine("   , 100 * case when tec.Qta_Ril < 0  then abs(tec.Qta_Ril * sDetSomma.SuperficieDettaglio) else  ( tec.Qta_Ril / sDetTotale.SuperficieDettaglio ) * sDetSomma.SuperficieDettaglio end as [Litres]     ")

            ''acqua redistribuita
            'If Esportazione_Elem_cod = 191 Then
            '    stb.AppendLine("   , 100 * case when tec.Qta_Ril < 0  then abs(tec.Qta_Ril * sDet.SuperficieDettaglio) else  ( tec.Qta_Ril / sDetTotale.SuperficieDettaglio ) * sDet.SuperficieDettaglio end as [Litres]     ")
            'Else
            '    stb.AppendLine("   , case when tec.qta_ril is null or tec.qta_ril = 0  then 1 else  100 * case when tec.Qta_Ril < 0  then abs(tec.Qta_Ril * sDet.SuperficieDettaglio) else  ( tec.Qta_Ril / sDetTotale.SuperficieDettaglio ) * sDet.SuperficieDettaglio end end as [Litres]     ")
            'End If

            ' VAnni: 9/9/2020: anche sui trattamenti considero la possibile assenza di acqua.
            stb.AppendLine("   , case when tec.qta_ril is null or tec.qta_ril = 0  then 1 else  100 * case when tec.Qta_Ril < 0  then abs(tec.Qta_Ril * sDet.SuperficieDettaglio) else  ( tec.Qta_Ril / sDetTotale.SuperficieDettaglio ) * sDet.SuperficieDettaglio end end as [Litres]     ")

            stb.AppendLine("   --, Per verifiche su calcolo acqua decommentare questo .. : ")
            stb.AppendLine("   --, tec.Qta_Ril as Acqua_In_Ettolitri ")
            stb.AppendLine("   --, sDetTotale.SuperficieDettaglio as SuperficieTotale  ")
            stb.AppendLine("   --, sDet.SuperficieDettaglio as SuperficieZespri")

        End If

        stb.AppendLine("   , ISNULL(ni_" & VentoIntensita & ".Note_Valore_Numerico, 0 ) as [WindSpeed]   ")
        stb.AppendLine("   , ISNULL(ISNULL(ni_" & VentoDirezione & ".Note_Valore_Stringa, ni_" & VentoDirezione & ".Nota_Des), 'No Wind') as [WindDir]   ")
        stb.AppendLine("   , ISNULL(ni_" & Temp & ".Note_Valore_Numerico, 25) as [Temp]  ")
        stb.AppendLine("   , ISNULL(ISNULL(ni_" & Meteo & ".Note_Valore_Stringa, ni_" & Meteo & ".Nota_Des ), 'Fine') as [Weather]   ")
        stb.AppendLine("   , '' as [AuditNumber]   ")
        stb.AppendLine("   , 'Agronica' as [AuthorisedBy]   ")
        stb.AppendLine("   , " & YearOfHarvest & " as [YearOfHarvest]   ")

        stb.AppendLine("   , Mov_Det_Prodotto.elem_cod")
        stb.AppendLine("   , Mov_Det_Prodotto.pro_cod")

        If Esportazione_Elem_cod = 191 Then

            stb.AppendLine("   , tecAv.av_cod")
            stb.AppendLine("   , tecAv.av_gru")
        Else

            stb.AppendLine("   , 0 as av_cod")
            stb.AppendLine("   , 0 as av_gru")
        End If
        stb.AppendLine("   , a.lav_Cod")
        stb.AppendLine("   , sDet.SuperficieDettaglio as Area")

        stb.AppendLine("   , cast(A.id_Agenda as varchar(100)) + '_' + sDet.kpin + '_' + sDet.blockName as [kendoKey]   ")

        'Operazioni_EsportazioneZespri_Query_Fiore(strFF_Cod, stb)

        stb.AppendLine(" , case when tec.qta_ril is null or tec.qta_ril = 0  then 0 else 1  end as h2oEsiste ")



        stb.AppendLine("  ")
        stb.AppendLine(" FROM  Agenda  A  ")

        stb.AppendLine(" inner join Imprese I  ")
        stb.AppendLine(" on I.piva = A.Piva  ")

        If PivaPadreInGerarchia <> "" Then
            stb.AppendLine("  inner Join GerarchiaImprese gi ")
            stb.AppendLine("     On gi.Figlio = i.PIVA ")
            stb.AppendLine("  And gi.Padre = '" & Agro_SQL_SaveText(PivaPadreInGerarchia) & "'")
        End If

        If RichiediPraticaValida Then

            stb.AppendLine("    inner join Pratiche p ")
            stb.AppendLine("        on p.Piva = A.PIVA   ")
            stb.AppendLine("        and p.Servizio_Cod =  " & servizio_cod)
            stb.AppendLine("        and p.piva_superUser = '" & ObJParametri.PivaSuperUser & "' ")

            stb.AppendLine("  ")
            stb.AppendLine("    inner join Pratiche_Stati_Attuali SP ")
            stb.AppendLine("        on SP.Pratica_Cod = p.Pratica_Cod  ")
            stb.AppendLine("        and SP.Stato_Cod  = " & Stato_Export)

        End If

        stb.AppendLine("     INNER JOIN Movimenti Mov   ")
        stb.AppendLine("         ON A.PIVA = Mov.PIVA   ")
        stb.AppendLine("         AND A.Sa_Cod = Mov.Sa_Cod   ")
        stb.AppendLine("         AND A.Id_Agenda = Mov.Id_Agenda  ")

        If Esportazione_Elem_cod = FERTILIZZANTI Then
            stb.AppendLine("     AND     Mov.Cau_Mov = '" & Agro_SQL_SaveText(CAU_LAVORAZIONE) & "'   ")
        Else
            stb.AppendLine("     AND     Mov.Cau_Mov = '" & Agro_SQL_SaveText(CAU_TRATTAMENTO) & "'   ")
        End If

        stb.AppendLine("  ")
        stb.AppendLine("    INNER JOIN Movimenti_dettagli Mov_Det_Prodotto    ")
        stb.AppendLine("          ON Mov_Det_Prodotto.PIVA = Mov.PIVA    ")
        stb.AppendLine("          AND Mov_Det_Prodotto.Sa_Cod = Mov.Sa_Cod    ")
        stb.AppendLine("          AND Mov_Det_Prodotto.Id_Agenda = Mov.Id_Agenda    ")
        stb.AppendLine("          AND Mov_Det_Prodotto.Id_Mov = Mov.Id_Mov    ")

        'If FormatoEsportazioneZespri = enum_FormatoZespriExport.Zespri2018 Then

        'lettura Acqua..
        '      If Esportazione_Elem_cod = 191 Then
        '      stb.AppendLine("    INNER JOIN Mov_Dettaglio_Tecnico tec    ")
        '      Else
        stb.AppendLine("    LEFT JOIN Mov_Dettaglio_Tecnico tec    ")
        '      End If


        stb.AppendLine("          ON Mov_Det_Prodotto.PIVA = tec.PIVA    ")
        stb.AppendLine("          AND Mov_Det_Prodotto.Sa_Cod = tec.Sa_Cod    ")
        stb.AppendLine("          AND Mov_Det_Prodotto.Id_Agenda = tec.Id_Agenda    ")
        stb.AppendLine("          AND Mov_Det_Prodotto.Id_Mov = tec.Id_Mov    ")
        stb.AppendLine("          AND tec.Id_Mov_Det = 0 ")

        'lettura Avversità (oppure n,p,k, al momento non richiesti)
        If Esportazione_Elem_cod = 191 Then
            stb.AppendLine("    INNER JOIN Mov_Dettaglio_Tecnico tecAv    ")
            stb.AppendLine("          ON Mov_Det_Prodotto.PIVA = tecAv.PIVA    ")
            stb.AppendLine("          AND Mov_Det_Prodotto.Sa_Cod = tecAv.Sa_Cod    ")
            stb.AppendLine("          AND Mov_Det_Prodotto.Id_Agenda = tecAv.Id_Agenda    ")
            stb.AppendLine("          AND Mov_Det_Prodotto.Id_Mov = tecAv.Id_Mov    ")
            stb.AppendLine("          AND Mov_Det_Prodotto.Id_Mov_Det = tecAv.Id_Mov_Det ")
        End If


        stb.AppendLine("   --lettura delle superfici complessive (tutte le cultivar) necessaria per riproporzionare l'acqua se indicata come ""Totale"" ")
        stb.AppendLine("   inner Join ( ")
        stb.AppendLine("         select  ")
        stb.AppendLine("            id_agenda ")
        stb.AppendLine("          , Id_Mov ")
        stb.AppendLine("          , Id_Mov_Det ")
        stb.AppendLine("          , SUM(qta2) as SuperficieDettaglio ")
        stb.AppendLine("      From Mov_Destinazioni ")
        stb.AppendLine("         Where Tipo_Destinazione = 0 ")
        stb.AppendLine("         Group By id_agenda ")
        stb.AppendLine("          , Id_Mov ")
        stb.AppendLine("          , Id_Mov_Det ")
        stb.AppendLine("  ) sDetTotale ")
        stb.AppendLine("  On Mov_Det_Prodotto.Id_Agenda = sDetTotale.Id_Agenda ")
        stb.AppendLine("  And Mov_Det_Prodotto.Id_Mov = sDetTotale.Id_Mov ")
        stb.AppendLine("  And Mov_Det_Prodotto.Id_Mov_Det = sDetTotale.Id_Mov_Det ")
        stb.AppendLine(" ")

        stb.AppendLine("   --lettura di dati di anagrafica Zespri")
        stb.AppendLine("            inner Join(  ")
        stb.AppendLine("  ")
        stb.AppendLine("          Select  ")
        stb.AppendLine("             dest.id_agenda ")
        stb.AppendLine("           , dest.Id_Mov  ")
        stb.AppendLine("           , dest.Id_Mov_Det ")
        stb.AppendLine("        , cc_kpin.val_cod as kpin ")
        stb.AppendLine("        , cc_blockName.val_cod as blockName          ")
        stb.AppendLine("        , imp.CUL_COD ")
        stb.AppendLine("        , dist.Regolamento_COD ")
        stb.AppendLine("           , SUM(qta2) as SuperficieDettaglio  ")
        stb.AppendLine("       From Mov_Destinazioni Dest ")
        stb.AppendLine("    INNER Join Movimenti Mov    ")
        stb.AppendLine("          On dest.PIVA = Mov.PIVA    ")
        stb.AppendLine("          And dest.Sa_Cod = Mov.Sa_Cod    ")
        stb.AppendLine("          And dest.Id_Agenda = Mov.Id_Agenda   ")
        stb.AppendLine("       And Dest.Id_Mov = Mov.Id_Mov ")
        stb.AppendLine("  ")
        stb.AppendLine("    inner Join Reg_Impianti imp  ")
        stb.AppendLine("          On imp.PIVA = Dest.PIVA    ")
        stb.AppendLine("          And imp.Sa_Cod = Dest.Sa_Cod    ")
        stb.AppendLine("          And imp.APPEZZA  = Dest.Appezza   ")
        stb.AppendLine("          And imp.ID_REG = Dest.Id_Destinazione ")
        stb.AppendLine("  ")
        stb.AppendLine("      inner Join Imprese_Progetti dist  ")
        stb.AppendLine("       On dest.PIVA = dist.PIVA     ")
        stb.AppendLine("           And dest.Sa_Cod = dist.Sa_Cod     ")
        stb.AppendLine("           And dest.APPEZZA  = dist.Appezza    ")
        stb.AppendLine("           And dest.Id_Destinazione = dist.Id_Reg  ")
        stb.AppendLine("        And Mov.Data_Movimento <= dist.Validita_Fine  ")
        stb.AppendLine("        And Mov.Data_Movimento >= dist.Validita_Inizio  ")
        stb.AppendLine("  ")
        stb.AppendLine("     INNER Join Reg_Impianti_Codici cc_kpin  ")
        stb.AppendLine("         On cc_kpin.PIVA = dest.PIVA   ")
        stb.AppendLine("         And cc_kpin.sa_cod = dest.SA_COD   ")
        stb.AppendLine("         And cc_kpin.appezza = dest.APPEZZA   ")
        stb.AppendLine("         And cc_kpin.Id_Reg = dest.Id_Destinazione   ")
        stb.AppendLine("         And ( cc_kpin.Progetto_Cod = dist.Progetto_Cod Or cc_kpin.Progetto_Cod = 0 )   ")
        stb.AppendLine("         And cc_kpin.val_cod <> '' ")
        stb.AppendLine("         And cc_kpin.id_cod =  " & enum_CodiciAnagrafe.Zespri_Codice_kPIN)
        stb.AppendLine("  ")
        stb.AppendLine("     INNER Join Reg_Impianti_Codici cc_blockName  ")
        stb.AppendLine("         On cc_blockName.PIVA = dest.PIVA   ")
        stb.AppendLine("         And cc_blockName.sa_cod = dest.SA_COD   ")
        stb.AppendLine("         And cc_blockName.appezza = dest.APPEZZA   ")
        stb.AppendLine("         And cc_blockName.Id_Reg = dest.Id_Destinazione   ")
        stb.AppendLine("         And ( cc_blockName.Progetto_Cod = dist.Progetto_Cod Or cc_blockName.Progetto_Cod = 0 )   ")
        stb.AppendLine("         And cc_blockName.id_cod =   " & enum_CodiciAnagrafe.Zespri_Block_Name)
        stb.AppendLine("  ")
        stb.AppendLine("          Where Tipo_Destinazione = 0  ")
        stb.AppendLine("          Group By  ")
        stb.AppendLine("             dest.id_agenda ")
        stb.AppendLine("           , dest.Id_Mov  ")
        stb.AppendLine("           , dest.Id_Mov_Det  ")
        stb.AppendLine("        , cc_kpin.val_cod ")
        stb.AppendLine("        , cc_blockName.val_cod ")
        stb.AppendLine("        , imp.CUL_COD ")
        stb.AppendLine("        , dist.Regolamento_COD ")
        stb.AppendLine("  ")
        stb.AppendLine("   ) sDet  ")
        stb.AppendLine("   On Mov_Det_Prodotto.Id_Agenda = sDet.Id_Agenda  ")
        stb.AppendLine("   And Mov_Det_Prodotto.Id_Mov = sDet.Id_Mov  ")
        stb.AppendLine("   And Mov_Det_Prodotto.Id_Mov_Det = sDet.Id_Mov_Det  ")
        stb.AppendLine(" ")


        stb.AppendLine("   --lettura delle superfici complessive (Zespri, in Join e Raggruppati su KPIN, Block) necessaria per riproporzionare l'acqua sia se indicata come ""Totale"", sia ad HA ")
        stb.AppendLine("            inner Join(  ")
        stb.AppendLine("  ")
        stb.AppendLine("          Select  ")
        stb.AppendLine("             dest.id_agenda ")
        stb.AppendLine("           , dest.Id_Mov  ")
        stb.AppendLine("           , dest.Id_Mov_Det ")
        stb.AppendLine("           , SUM(qta2) as SuperficieDettaglio  ")
        stb.AppendLine("       From Mov_Destinazioni Dest ")
        stb.AppendLine("    INNER Join Movimenti Mov    ")
        stb.AppendLine("          On dest.PIVA = Mov.PIVA    ")
        stb.AppendLine("          And dest.Sa_Cod = Mov.Sa_Cod    ")
        stb.AppendLine("          And dest.Id_Agenda = Mov.Id_Agenda   ")
        stb.AppendLine("       And Dest.Id_Mov = Mov.Id_Mov ")
        stb.AppendLine("  ")
        stb.AppendLine("    inner Join Reg_Impianti imp  ")
        stb.AppendLine("          On imp.PIVA = Dest.PIVA    ")
        stb.AppendLine("          And imp.Sa_Cod = Dest.Sa_Cod    ")
        stb.AppendLine("          And imp.APPEZZA  = Dest.Appezza   ")
        stb.AppendLine("          And imp.ID_REG = Dest.Id_Destinazione ")
        stb.AppendLine("  ")
        stb.AppendLine("      inner Join Imprese_Progetti dist  ")
        stb.AppendLine("       On dest.PIVA = dist.PIVA     ")
        stb.AppendLine("           And dest.Sa_Cod = dist.Sa_Cod     ")
        stb.AppendLine("           And dest.APPEZZA  = dist.Appezza    ")
        stb.AppendLine("           And dest.Id_Destinazione = dist.Id_Reg  ")
        stb.AppendLine("        And Mov.Data_Movimento <= dist.Validita_Fine  ")
        stb.AppendLine("        And Mov.Data_Movimento >= dist.Validita_Inizio  ")
        stb.AppendLine("  ")
        stb.AppendLine("     INNER Join Reg_Impianti_Codici cc_kpin  ")
        stb.AppendLine("         On cc_kpin.PIVA = dest.PIVA   ")
        stb.AppendLine("         And cc_kpin.sa_cod = dest.SA_COD   ")
        stb.AppendLine("         And cc_kpin.appezza = dest.APPEZZA   ")
        stb.AppendLine("         And cc_kpin.Id_Reg = dest.Id_Destinazione   ")
        stb.AppendLine("         And ( cc_kpin.Progetto_Cod = dist.Progetto_Cod Or cc_kpin.Progetto_Cod = 0 )   ")
        stb.AppendLine("         And cc_kpin.id_cod =  " & enum_CodiciAnagrafe.Zespri_Codice_kPIN)
        stb.AppendLine("  ")
        stb.AppendLine("     INNER Join Reg_Impianti_Codici cc_blockName  ")
        stb.AppendLine("         On cc_blockName.PIVA = dest.PIVA   ")
        stb.AppendLine("         And cc_blockName.sa_cod = dest.SA_COD   ")
        stb.AppendLine("         And cc_blockName.appezza = dest.APPEZZA   ")
        stb.AppendLine("         And cc_blockName.Id_Reg = dest.Id_Destinazione   ")
        stb.AppendLine("         And ( cc_blockName.Progetto_Cod = dist.Progetto_Cod Or cc_blockName.Progetto_Cod = 0 )   ")
        stb.AppendLine("         And cc_blockName.id_cod =   " & enum_CodiciAnagrafe.Zespri_Block_Name)
        stb.AppendLine("  ")
        stb.AppendLine("          Where Tipo_Destinazione = 0  ")
        stb.AppendLine("          Group By  ")
        stb.AppendLine("             dest.id_agenda ")
        stb.AppendLine("           , dest.Id_Mov  ")
        stb.AppendLine("           , dest.Id_Mov_Det  ")
        stb.AppendLine("  ")
        stb.AppendLine("   ) sDetSomma  ")
        stb.AppendLine("   On Mov_Det_Prodotto.Id_Agenda = sDetSomma.Id_Agenda  ")
        stb.AppendLine("   And Mov_Det_Prodotto.Id_Mov = sDetSomma.Id_Mov  ")
        stb.AppendLine("   And Mov_Det_Prodotto.Id_Mov_Det = sDetSomma.Id_Mov_Det  ")
        stb.AppendLine(" ")

        'End If


        If Esportazione_Elem_cod = 191 Then
            stb.AppendLine("    INNER JOIN Formulati F ")
            stb.AppendLine("        on F.Fr_Cod = Mov_Det_Prodotto.Pro_Cod ")
            stb.AppendLine("  ")
        Else
            stb.AppendLine("    INNER JOIN Fertilizzanti F ")
            stb.AppendLine("        on F.Fer_Cod = Mov_Det_Prodotto.Pro_Cod ")
            stb.AppendLine("  ")
        End If

        'lettura descrizione prodotto da: CAC_Codifica_ProdottiAziendali
        If FormatoEsportazioneZespri = enum_FormatoZespriExport.Zespri2018 Then
            If Esportazione_Elem_cod = 191 Then
                stb.AppendLine(" Left Join  CAC_Codifica_ProdottiAziendali cac ")
                stb.AppendLine("         On f.Fr_Cod = cac.Codice_GIAS ")
                stb.AppendLine("      And cac.Elem_Cod = 191  ")

            Else
                stb.AppendLine(" Left Join  CAC_Codifica_ProdottiAziendali cac ")
                stb.AppendLine("         On f.Fer_Cod = cac.Codice_GIAS ")
                stb.AppendLine("      And cac.Elem_Cod = 3  ")
            End If
        End If

        'G2C_Log
        stb.AppendLine("    LEFT OUTER JOIN G2C_Log ")
        stb.AppendLine("        On G2C_Log.PivaSuperUser = '" & ObJParametri.PivaSuperUser & "' ")
        'stb.AppendLine("        And G2C_Log.Piva = A.Piva   ")
        stb.AppendLine("        And G2C_Log.Id_Agenda = A.Id_Agenda   ")


        Agenda_Lettura_Note_Interventi(-1, Meteo, PivaSuperUSer_Zespri, stb)

        Agenda_Lettura_Note_Interventi(-2, VentoIntensita, PivaSuperUSer_Zespri, stb)

        Agenda_Lettura_Note_Interventi(-3, VentoDirezione, PivaSuperUSer_Zespri, stb)

        Agenda_Lettura_Note_Interventi(-4, Temp, PivaSuperUSer_Zespri, stb)

        Agenda_Lettura_Note_Interventi(-5, Orario, PivaSuperUSer_Zespri, stb)

        Agenda_Lettura_Note_Interventi(-6, Motivo, PivaSuperUSer_Zespri, stb)


        stb.AppendLine("    WHERE   Mov.Data_Movimento <=  " & Agro_SQL_SaveDate(DataFine) & "   ")
        stb.AppendLine("     AND     Mov.Data_Movimento >=  " & Agro_SQL_SaveDate(DataInizio) & "   ")

        'Unici Cul_Cod Validi
        stb.AppendLine(" And sDet.CUL_COD in (5012170, 5011458, 5000009, 7512) ")

        If kpin > 0 Then
            stb.AppendLine("And sDet.kpin = '" & Agro_SQL_SaveText(kpin) & "'")
        End If


        If Esportazione_Elem_cod = 3 Then
            stb.AppendLine("     AND   A.Lav_Cod IN ( " &
                            CStr(LAVCOD_DISTRIBUZIONE_CONCIME) & ", " &
                            CStr(LAVCOD_SARCHIATURA_CONCIMAZIONE) & ", " &
                            CStr(LAVCOD_DISTRIBUZIONE_AMMENDANTI) & ", " &
                            CStr(LAVCOD_FERTIRRIGAZIONE) & ", " &
                            CStr(LAVCOD_CONCIMAZIONE_FOGLIARE) & ", " &
                            CStr(LAVCOD_TRATTAMENTO_ANTIBUTTERATURA) &
                        " )  ")

        Else

            stb.AppendLine("     AND   A.Lav_Cod IN ( " &
                            CStr(LAVCOD_TRATTAMENTO_ANTIPARASSITARIO) & ", " &
                            CStr(LAVCOD_TRATTAMENTO_FITOREGOLATORE) & ", " &
                            CStr(LAVCOD_DISERBO) & ", " &
                            CStr(LAVCOD_GEODISINFESTAZIONE) & ", " &
                            CStr(LAVCOD_CONCIA_SEME) & ", " &
                            CStr(LAVCOD_DISSECCAMENTO) &
                        " )  ")
        End If

        If Not String.IsNullOrEmpty(xFiltroAggiuntivo) Then
            stb.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , ObJParametri))
        End If

        If bExport Then

            'Inserimento Filtro Stato_Export non valorizzazto
            stb.AppendLine("     AND   Stato_Export = 0 And Stato_Export_2 = 0 ")

        End If


        stb.AppendLine(" ORDER BY sDet.kPin, mov.data_movimento, sDet.BlockName")

    End Sub

    ''' <summary>
    ''' Query per leggere i dati da reg_impianti, ws_anagrafica_canopy
    ''' </summary>
    ''' <param name="RichiediPraticaValida"></param>
    ''' <param name="DataInizio"></param>
    ''' <param name="DataFine"></param>
    ''' <param name="kpin"></param> chiave API
    ''' <param name="cifreArrotondamento"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="objParametri"></param>
    ''' <param name="objParametri_Utenti"></param>
    ''' <returns></returns>
    Public Function Operazioni_EsportazioneAnagraficaZespri(
        ByVal RichiediPraticaValida As Boolean,
        ByVal DataInizio As DateTime,
        ByVal DataFine As DateTime,
        ByVal kpin As String, ' A N N A
        ByVal cifreArrotondamento As Integer,
        ByVal xFiltroAggiuntivo As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        Dim nomeRoutine As String = "Operazioni_EsportazioneAnagraficaZespri"

        Dim stb As New StringBuilder
        Dim messaggioErrore As String = ""
        Dim dt As New DataTable

        Try
            stb.Length = 0

            stb.AppendLine(" SELECT D.KPIN + '_' + D.BlockName AS kendoKey                                                                                                                  ")
            stb.AppendLine(" , ISNULL(BlockID, -1) AS BlockID , D.KPIN, D.BlockName, VineStatus, ROUND(Area, 5) AS Area, BlockVar, BlockGM,  ISNULL(Structure, '') AS Structure            ")
            stb.AppendLine(" , CoverStatus ")
            stb.AppendLine(" , CASE WHEN CoverStatus = 'Uncovered' THEN '' ELSE ")
            stb.AppendLine("      CASE WHEN D.CoverDate Is NULL Or cast(D.CoverDate as date) = cast('1900-01-01' as datetime)")
            stb.AppendLine("      THEN CASE WHEN  Data_Inizio_Innesto IS NOT NULL AND  cast(Data_Inizio_Innesto as date) <> cast('1900-01-01' as datetime)  THEN convert(varchar(50), Data_Inizio_Innesto, 103) ELSE '2021-01-01' END ")
            stb.AppendLine("      ELSE convert(varchar(50), D.CoverDate, 103)")
            stb.AppendLine("      END ")
            stb.AppendLine("  END As CoverDate")
            stb.AppendLine(", ISNULL(YearGrafted, 1900) As YearGrafted, ISNULL(OrchardLayout, -1) As OrchardLayout, ")
            stb.AppendLine("ISNULL(DescrizioneSalvataggio,'') AS Description, ISNULL(MessaggiSincroCanopy,'') AS Message,                                                                  ")
            stb.AppendLine("ISNULL(Dettagli.nome + ' '+ Dettagli.cognome, '') AS username_modifica, D.Data_modifica, dettagli.data_modifica AS data_modifica_impianto                       ")

            stb.AppendLine(", progetto_Cod as StatoSincronizzazione  ")
            stb.AppendLine(" FROM (                                         ")
            stb.AppendLine(" SELECT                                         ")
            stb.AppendLine("   cc_kpin.val_cod AS KPIN                      ")
            stb.AppendLine(" , cc_blockName.val_cod AS BlockName            ")

            'stb.AppendLine(" , MAX(dist.Stato_Impianto) AS VineStatus")
            stb.AppendLine(", CASE WHEN MAX(imp.Data_Inizio_Produzione) is NULL                                 ")
            stb.AppendLine("            THEN 'Non Producing'                                                    ")
            stb.AppendLine("       ELSE CASE WHEN MAX(year(imp.Data_Inizio_Produzione))<=year(getdate())        ")
            stb.AppendLine("            THEN 'Producing'                                                        ")
            stb.AppendLine("       ELSE 'Non Producing'                                                         ")
            stb.AppendLine("       END                                                                          ")
            stb.AppendLine("  END AS VineStatus                                                                 ")

            stb.AppendLine("")
            ' VAnni: 7/7/2021: commentata parte che converte a varchar ed impostata lettura del dato originale
            'stb.AppendLine(" , REPLACE(cast(SUM(imp.sup_imp) AS varchar(50)), ',', '.')   AS Area   ")
            stb.AppendLine(" , SUM(imp.sup_imp) AS Area                                                         ")

            'stb.AppendLine(" , CASE WHEN imp.CUL_COD in (5012170, 5011458)                                      ")
            'stb.AppendLine("            THEN 'GA'                                                               ")
            'stb.AppendLine("        ELSE CASE WHEN imp.CUL_COD in (5000009, 7512)                               ")
            'stb.AppendLine("            THEN 'GK'                                                               ")
            'stb.AppendLine("        ELSE ''                                                                     ")
            'stb.AppendLine("        END                                                                         ")
            'stb.AppendLine("   END AS [BlockVar]                                                                ")
            stb.AppendLine("   , 'GA' AS [BlockVar]                                                                ")

            stb.AppendLine(" , CASE WHEN dist.Regolamento_Cod = 4                                               ")
            stb.AppendLine("           THEN 'OB'                                                                ")
            stb.AppendLine("   ELSE 'CK'                                                                        ")
            stb.AppendLine("   END AS [BlockGM]                                                                 ")
            '
            stb.AppendLine(" , CASE WHEN imp.FORAL_COD in (8)                                                   ")
            stb.AppendLine("            THEN 'T-BAR'                                                            ")
            stb.AppendLine("        ELSE CASE WHEN imp.FORAL_COD in (32)                                        ")
            stb.AppendLine("            THEN 'Pergola'                                                          ")
            stb.AppendLine("        ELSE ''                                                                     ")
            stb.AppendLine("        END                                                                         ")
            stb.AppendLine("   END As [Structure]                                                               ")

            'stb.AppendLine(" , MAX(imp.COP_COD) As CoverStatus                                                  ")
            stb.AppendLine(" , CASE WHEN MAX(imp.COP_COD) = 1                                                   ")
            stb.AppendLine("           THEN 'Hail netting'                                                      ")
            stb.AppendLine("       ELSE CASE WHEN MAX(imp.COP_COD) = 15                                         ")
            stb.AppendLine("           THEN 'Waterproof'                                                        ")
            stb.AppendLine("       ELSE 'Uncovered'                                                             ")
            stb.AppendLine("       END                                                                          ")
            stb.AppendLine("   END AS CoverStatus                                                               ")

            stb.AppendLine(" , MAX(imp.COP_DI) as CoverDate                                                    ")


            stb.AppendLine(" , MAX(YEAR(imp.Data_Inizio_Innesto)) As YearGrafted                                ")
            stb.AppendLine(" , MAX(imp.Data_Inizio_Innesto)  AS Data_Inizio_Innesto                                ")

            'stb.AppendLine(" , MAX(imp.Piante_Maschi_InSesto) As OrchardLayout                                  ")
            stb.AppendLine(" , CASE WHEN MAX(imp.Piante_Maschi_InSesto) = 1                                     ")
            stb.AppendLine("            THEN 'Strip Male'                                                       ")
            stb.AppendLine("   ELSE ''                                                                          ")
            stb.AppendLine("   END AS OrchardLayout                                                              ")

            stb.AppendLine(" , MAX(imp.username_modifica) AS username_modifica                                  ")
            stb.AppendLine(" , MAX(imp.data_modifica) AS data_modifica                                          ")
            stb.AppendLine(" , MAX(dist.progetto_Cod) AS progetto_Cod                                          ")

            '
            stb.AppendLine("  ")
            stb.AppendLine(" From Reg_Impianti imp   ")
            stb.AppendLine("     ")
            stb.AppendLine(" inner Join Imprese_Progetti dist   ")
            stb.AppendLine(" On imp.PIVA = dist.PIVA      ")
            stb.AppendLine("     AND imp.Sa_Cod = dist.Sa_Cod      ")
            stb.AppendLine("     AND imp.APPEZZA  = dist.Appezza     ")
            stb.AppendLine("     AND imp.id_reg = dist.Id_Reg   ")
            stb.AppendLine(" AND " & Agro_SQL_SaveDate(DataInizio) & " <= dist.Validita_Fine   ")
            stb.AppendLine(" AND " & Agro_SQL_SaveDate(DataInizio) & " >= dist.Validita_Inizio   ")
            stb.AppendLine("    ")
            stb.AppendLine(" INNER Join Reg_Impianti_Codici cc_kpin   ")
            stb.AppendLine("     On cc_kpin.PIVA = imp.PIVA    ")
            stb.AppendLine("     AND cc_kpin.sa_cod = imp.SA_COD    ")
            stb.AppendLine("     AND cc_kpin.appezza = imp.APPEZZA    ")
            stb.AppendLine("     AND cc_kpin.Id_Reg = imp.id_Reg    ")
            stb.AppendLine("     AND ( cc_kpin.Progetto_Cod = dist.Progetto_Cod Or cc_kpin.Progetto_Cod = 0 )    ")
            stb.AppendLine("     AND cc_kpin.id_cod =  1287 ")
            stb.AppendLine("    ")
            stb.AppendLine(" INNER Join Reg_Impianti_Codici cc_blockName   ")
            stb.AppendLine("     On cc_blockName.PIVA = imp.PIVA    ")
            stb.AppendLine("     AND cc_blockName.sa_cod = imp.SA_COD    ")
            stb.AppendLine("     AND cc_blockName.appezza = imp.APPEZZA    ")
            stb.AppendLine("     AND cc_blockName.Id_Reg = imp.id_reg    ")
            stb.AppendLine("     AND ( cc_blockName.Progetto_Cod = dist.Progetto_Cod Or cc_blockName.Progetto_Cod = 0 )    ")
            stb.AppendLine("     AND cc_blockName.id_cod =   1288 ")
            stb.AppendLine("        ")

            stb.AppendLine(" Group By ")
            stb.AppendLine("   cc_kpin.val_cod  ")
            stb.AppendLine(" , cc_blockName.val_cod  ")
            'stb.AppendLine(" , imp.CUL_COD  ")
            stb.AppendLine(" , dist.Regolamento_COD ")
            stb.AppendLine(" , imp.FORAL_COD ")
            stb.AppendLine(" ) D ")

            stb.AppendLine(" LEFT Join ws_Canopy_Anagrafica canopy   ")
            stb.AppendLine("     On canopy.StatoSincronizzazione = D.progetto_Cod   ")
            'stb.AppendLine("     On canopy.Kpin = D.KPIN   ")
            'stb.AppendLine("     AND canopy.BlockName = D.BLOCKNAME    ")


            Dim NomeDB_Utenti As String = objParametri_Utenti.Recupera_NomeDB()
            stb.AppendLine(" LEFT JOIN " & NomeDB_Utenti & ".dbo.Utenti_Dettagli Dettagli  ")
            stb.AppendLine("    ON Dettagli.CodFisc = D.Username_modifica  ")


            ' - - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
            If kpin <> "" Then
                stb.AppendLine(" WHERE D.KPIN = " & Agro_SQL_SaveNum(kpin))
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(xFiltroAggiuntivo)
            End If
            ' - - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -


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

    Public Function Operazioni_EsportazioneZespri(
        ByVal RichiediPraticaValida As Boolean,
        ByVal SIGPA_ComandiEsportazione_cod As Integer,
        ByVal PIVA_PadreInGerarchia As String,
        ByVal DataInizio As DateTime,
        ByVal DataFine As DateTime,
        ByVal kpin As Integer,
        ByVal AggregaDatiPerKpinBlock As Boolean,
        ByVal Stato_Export As Integer,
        ByVal cifreArrotondamento As Integer,
        ByVal strFF_Cod As String,
        ByVal FormatoEsportazioneZespri As Integer,
        ByVal xFiltroAggiuntivo As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
        Optional ByVal bExport As Boolean = False
    ) As DataTable


        '----------------------------------------------------
        '--- Note ---------------------------
        ' 1. al momento non ci sono raggruppamenti di impianti per kPin o block name (vengono ripetute le righe)
        '----------------------------------------------------

        Dim nomeRoutine As String = "Operazioni_EsportazioneZespri"

        Dim stb As New StringBuilder
        Dim messaggioErrore As String = ""
        Dim dt As New DataTable

        '----------------------------------------------------
        '--- Preparo la Query SQL ---------------------------
        '----------------------------------------------------
        Try
            stb.Length = 0

            If AggregaDatiPerKpinBlock Then


                stb.AppendLine("  Select ")
                stb.AppendLine("  ")
                stb.AppendLine("  Grower ")
                stb.AppendLine(" ,ExportID ")
                stb.AppendLine(" ,KPIN ")
                stb.AppendLine(" ,BlockName ")
                stb.AppendLine(" ,BlockVar ")
                stb.AppendLine(" ,BlockGM ")
                stb.AppendLine(" ,Date ")
                stb.AppendLine(" ,Time ")
                stb.AppendLine(" ,WaterName ")
                stb.AppendLine(" ,Agronica_Elem_cod ")
                stb.AppendLine(" ,Agronica_cod_Prodotto ")
                stb.AppendLine(" ,Agronica_cod_PrincipiAttivi ")
                stb.AppendLine(" ,Product ")
                stb.AppendLine(" ,UnlistedProduct ")
                stb.AppendLine(" ,Rate ")
                stb.AppendLine(" ,ProductType ")
                stb.AppendLine(" ,udm_Dose ")
                stb.AppendLine(" ,Reason ")
                stb.AppendLine(" ,Contractor ")
                stb.AppendLine(" ,AppGSafe ")
                stb.AppendLine(" ,Equipment ")
                stb.AppendLine(" ,AINozzle ")
                stb.AppendLine(" , case when h2oEsiste = 0 then 1 else  sum(Litres) end as Litres ")
                stb.AppendLine(" --,Litres ")
                stb.AppendLine(" ,WindSpeed ")
                stb.AppendLine(" ,WindDir ")
                stb.AppendLine(" ,Temp ")
                stb.AppendLine(" ,Weather ")
                stb.AppendLine(" ,AuditNumber ")
                stb.AppendLine(" ,AuthorisedBy ")
                stb.AppendLine(" ,YearOfHarvest ")
                stb.AppendLine(" ,elem_cod ")
                stb.AppendLine(" ,pro_cod ")
                stb.AppendLine(" ,av_cod ")
                stb.AppendLine(" ,av_gru ")
                stb.AppendLine(" ,lav_Cod ")
                stb.AppendLine(" ,sum(Area) as Area ")
                stb.AppendLine(" --, Area ")
                stb.AppendLine(" ,kendoKey ")
                stb.AppendLine("  ")
                stb.AppendLine(" from( ")
                stb.AppendLine(" ")

            End If
            'AggregaDatiPerKpinBlock

            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM ( ")

            Operazioni_EsportazioneZespri_Query(
                RichiediPraticaValida,
                Stato_Export,
                cifreArrotondamento,
                FORMULATI,
                DataInizio,
                DataFine,
                PIVA_PadreInGerarchia,
                kpin,
                strFF_Cod,
                FormatoEsportazioneZespri,
                xFiltroAggiuntivo,
                objParametri,
                stb,
                bExport)

            stb.AppendLine(" ) Trattamenti ")

            ' VAnni: 28/8/2018: escludo momentaneamente le fertilizzazioni
            ' VAnni: 23/8/2019: dal 2019 rientrano in pista le fertilizzazioni!
            'If FormatoEsportazioneZespri = enum_FormatoZespriExport.Zespri2016 Then
            stb.AppendLine(" union all ")

            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM ( ")

            Operazioni_EsportazioneZespri_Query(
                RichiediPraticaValida,
                Stato_Export,
                cifreArrotondamento,
                FERTILIZZANTI,
                DataInizio,
                DataFine,
                PIVA_PadreInGerarchia,
                kpin,
                strFF_Cod,
                FormatoEsportazioneZespri,
                xFiltroAggiuntivo,
                objParametri,
                stb,
                bExport)

            stb.AppendLine(" ) Fertilizzazioni ")

            If AggregaDatiPerKpinBlock Then

                stb.AppendLine(" ) Totali ")

                stb.AppendLine(" ")
                stb.AppendLine("  group by   ")
                stb.AppendLine("  ")
                stb.AppendLine("  Grower ")
                stb.AppendLine(" ,ExportID ")
                stb.AppendLine(" ,KPIN ")
                stb.AppendLine(" ,BlockName ")
                stb.AppendLine(" ,BlockVar ")
                stb.AppendLine(" ,BlockGM ")
                stb.AppendLine(" ,Date ")
                stb.AppendLine(" ,Time ")
                stb.AppendLine(" ,WaterName ")
                stb.AppendLine(" ,Agronica_Elem_cod ")
                stb.AppendLine(" ,Agronica_cod_Prodotto ")
                stb.AppendLine(" ,Agronica_cod_PrincipiAttivi ")
                stb.AppendLine(" ,Product ")
                stb.AppendLine(" ,UnlistedProduct ")
                stb.AppendLine(" ,Rate ")
                stb.AppendLine(" ,ProductType ")
                stb.AppendLine(" ,udm_Dose ")
                stb.AppendLine(" ,Reason ")
                stb.AppendLine(" ,Contractor ")
                stb.AppendLine(" ,AppGSafe ")
                stb.AppendLine(" ,Equipment ")
                stb.AppendLine(" ,AINozzle ")
                stb.AppendLine(" ,WindSpeed ")
                stb.AppendLine(" ,WindDir ")
                stb.AppendLine(" ,Temp ")
                stb.AppendLine(" ,Weather ")
                stb.AppendLine(" ,AuditNumber ")
                stb.AppendLine(" ,AuthorisedBy ")
                stb.AppendLine(" ,YearOfHarvest ")
                stb.AppendLine(" ,elem_cod ")
                stb.AppendLine(" ,pro_cod ")
                stb.AppendLine(" ,av_cod ")
                stb.AppendLine(" ,av_gru ")
                stb.AppendLine(" ,lav_Cod ")
                stb.AppendLine(" ,kendoKey")
                stb.AppendLine(" ,h2oEsiste")

                stb.AppendLine("order by Agronica_Elem_cod, kpin, totali.Date, BlockName")
            End If
            'AggregaDatiPerKpinBlock


            'End If

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

#End Region

    '##############################################################################################

#Region "Sigpa"

    Public Function SIGPA_VerificheDistribuzioni(ByVal NumeroDellaFornitura As Integer,
                                                 ByVal ProgressivoDellaFornitura As Integer,
                                                 ByVal IdentificativoFunzioneRegistro As String,
                                                 ByVal piva As String,
                                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                 ) As DataTable


        Dim nomeRoutine As String = "ElencoAziendePerChiudGiacenze_Sigpa"

        Dim stb As New StringBuilder
        Dim messaggioErrore As String = ""
        Dim dt As New DataTable

        stb.AppendLine(" select agronica.Data_Movimento, iSigpa.PIVA, iSigpa.Id_Agenda, iSigpa.N_progressivo_operazione, iSigpa.codice_fertilizzante, iSigpa.veg_cod_agea, vegAg.Veg_Des_Agea, iSigpa.quantità, iSigpa.qtaSommata, Agronica.Qta as Agronica_QTA, s.PRODOTTO, udmsalvata.UDM_SIM as udmsalvata_UDM_Sim, udmExtra.UDM_SIM as udmExtra_UDM_SIM, udmSigpa.UDM_SIM as udmSigpa_UDM_SIM, coalesce(convSigpa.FattoreConversione, 1) * coalesce(convSalvaToExtra.FattoreConversione, 1) as  FattoreConversione ")
        stb.AppendLine(" from (  ")
        stb.AppendLine("        select MT11.data_trattamento, ll.PIVA, ll.Id_Agenda, MT11.N_progressivo_operazione, MT11.codice_fertilizzante , MT51.[codice prodotto] as veg_cod_agea, MT11.quantità, SUM(MT51.quantità) as qtaSommata ")
        stb.AppendLine("        from SIGPA_LogEsportazioni ll ")
        stb.AppendLine("        inner join SIGPA_MT11_ModuloDatiTrattamenti MT11 ")
        stb.AppendLine("            on ll.NumeroDellaFornitura = MT11.numero_della_fornitura  ")
        stb.AppendLine("            and ll.ProgressivoDellaFornitura = MT11.progressivo_della_fornitura  ")
        stb.AppendLine("        inner join SIGPA_MT51_ModuloParticelleOggettoDeiTrattamenti MT51 ")
        stb.AppendLine("            on MT11.N_progressivo_operazione = MT51.N_progressivo_operazione         ")
        stb.AppendLine("            and MT11.numero_della_fornitura = MT51.numero_della_fornitura         ")
        stb.AppendLine("            and MT11.identificativo_funzione_registro = MT51.identificativo_funzione_registro         ")
        stb.AppendLine("        where ")
        stb.AppendLine("        ll.NumeroDellaFornitura = " & Agro_SQL_SaveNum(NumeroDellaFornitura))
        stb.AppendLine("        and ll.identificativoFunzioneRegistro = '" & Agro_SQL_SaveText(IdentificativoFunzioneRegistro) & "'")

        If piva <> "" Then
            stb.AppendLine(" and ll.piva = '" & Agro_SQL_SaveText(piva) & "'")
        End If

        stb.AppendLine("  ")
        stb.AppendLine("        group by MT11.data_trattamento, ll.PIVA, ll.id_agenda, mt11.N_progressivo_operazione,MT11.codice_fertilizzante , MT51.[codice prodotto], MT11.quantità ")
        stb.AppendLine("    ) iSigpa ")
        stb.AppendLine("    inner join ( ")
        stb.AppendLine("  ")
        stb.AppendLine("        select m.Data_Movimento, A.PIVA, A.Id_Agenda, cc.val_cod as veg_cod_Agea, cac.Cod_Prodotto_Cliente, sum(Mov_Dest.Qta) as qta, dd.Udm_Cod, dd.Extra_Int ")
        stb.AppendLine("        from Agenda A        ")
        stb.AppendLine("        inner join Movimenti m ")
        stb.AppendLine("            on m.Id_Agenda = A.Id_Agenda  ")
        stb.AppendLine("            and m.PIVA = A.PIVA  ")
        stb.AppendLine("  ")
        stb.AppendLine("  ")
        stb.AppendLine("        inner join Movimenti_dettagli dd ")
        stb.AppendLine("            on m.Id_Agenda = dd.Id_Agenda  ")
        stb.AppendLine("            and m.Id_Mov = dd.Id_Mov  ")
        stb.AppendLine("            and m.PIVA = dd.PIVA  ")
        stb.AppendLine("            and m.Cau_Mov = '2300' ")
        stb.AppendLine("  ")
        stb.AppendLine("        inner join CAC_Codifica_ProdottiAziendali cac ")
        stb.AppendLine("            on cac.Codice_GIAS = dd.Pro_Cod  ")
        stb.AppendLine("  ")
        stb.AppendLine("         INNER JOIN Mov_Destinazioni Mov_Dest ")
        stb.AppendLine("             ON Mov_Dest.PIVA = dd.PIVA    ")
        stb.AppendLine("             AND Mov_Dest.Sa_Cod = dd.Sa_Cod    ")
        stb.AppendLine("             AND Mov_Dest.Id_Agenda = dd.Id_Agenda    ")
        stb.AppendLine("             AND Mov_Dest.Id_Mov = dd.Id_Mov    ")
        stb.AppendLine("             AND Mov_Dest.Id_Mov_Det = dd.Id_Mov_Det  ")
        stb.AppendLine("  ")
        stb.AppendLine("         INNER JOIN Reg_Impianti imp  ")
        stb.AppendLine("             ON imp.PIVA = Mov_Dest.PIVA    ")
        stb.AppendLine("             AND imp.Sa_Cod = Mov_Dest.Sa_Cod    ")
        stb.AppendLine("             AND imp.APPEZZA  = Mov_Dest.Appezza   ")
        stb.AppendLine("             and imp.ID_REG = Mov_Dest.Id_Destinazione  ")
        stb.AppendLine("  ")
        stb.AppendLine("        INNER JOIN Reg_Impianti_Codici cc  ")
        stb.AppendLine("            on cc.PIVA = imp.PIVA   ")
        stb.AppendLine("            and cc.sa_cod = imp.SA_COD   ")
        stb.AppendLine("            and cc.appezza = imp.APPEZZA   ")
        stb.AppendLine("            and cc.Id_Reg = imp.ID_REG   ")
        stb.AppendLine("            and cc.id_cod = 1133 ")
        stb.AppendLine("         group by m.Data_Movimento, A.PIVA, A.id_agenda, cc.val_cod, cac.Cod_Prodotto_Cliente, dd.Udm_Cod, dd.Extra_Int  ")
        stb.AppendLine("    ) Agronica ")
        stb.AppendLine("  ")
        stb.AppendLine("    on Agronica.PIVA = iSigpa.PIVA ")
        stb.AppendLine("    and Agronica.id_Agenda = iSigpa.id_Agenda ")
        stb.AppendLine("    and Agronica.Data_Movimento = iSigpa.data_trattamento  ")
        stb.AppendLine("    and Agronica.Cod_Prodotto_Cliente = iSigpa.codice_fertilizzante  ")
        stb.AppendLine("    and Agronica.veg_cod_Agea = iSigpa.veg_cod_agea  ")
        stb.AppendLine("  ")
        stb.AppendLine("    left JOIN SIGPA_CodificaFertilizzanti s ")
        stb.AppendLine("        on s.CODICE = agronica.Cod_Prodotto_Cliente  ")
        stb.AppendLine("  ")
        stb.AppendLine("  ")
        stb.AppendLine("     inner join UnitaMisura udmSalvata ")
        stb.AppendLine("        on udmSalvata.UDM_COD = Agronica.Udm_Cod ")
        stb.AppendLine("  ")
        stb.AppendLine("  ")
        stb.AppendLine("    LEFT join UnitaMisura udmExtra ")
        stb.AppendLine("        on udmExtra.UDM_COD = agronica.Extra_Int   ")
        stb.AppendLine("  ")
        stb.AppendLine("    left join UnitaMisura udmSigpa  ")
        stb.AppendLine("        on udmSigpa.UDM_SIM = s.UNITA_Misura collate Latin1_General_CI_AS ")
        stb.AppendLine("  ")
        stb.AppendLine("    left join UnitaMisura_Conversione convSalvaToExtra ")
        stb.AppendLine("        on convSalvaToExtra.UDM_COD_Da = agronica.Udm_Cod  ")
        stb.AppendLine("        and convSalvaToExtra.UDM_COD_A = agronica.Extra_Int   ")
        stb.AppendLine("  ")
        stb.AppendLine("  ")
        stb.AppendLine("    left join UnitaMisura_Conversione convSigpa ")
        stb.AppendLine("        on convSigpa.UDM_COD_Da = agronica.Extra_Int  ")
        stb.AppendLine("        and convSigpa.UDM_COD_A = udmSigpa.Udm_Cod   ")
        stb.AppendLine("  ")
        stb.AppendLine("    left join (select distinct veg_cod_agea, veg_Des_Agea from  Codifica_SpecieVegetali_Agea) vegAg ")
        stb.AppendLine("        on vegAg.Veg_Cod_Agea = iSigpa.veg_cod_agea  ")
        stb.AppendLine("  ")

        Try

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

    Public Function SIGPA_VerificheDistribuzioni_Particelle(ByVal NumeroDellaFornitura As Integer,
                                                            ByVal ProgressivoDellaFornitura As Integer,
                                                            ByVal identificativoFunzioneRegistro As Integer,
                                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                            ) As DataTable

        Dim nomeRoutine As String = "ElencoAziendePerChiudGiacenze_Sigpa"

        Dim stb As New StringBuilder
        Dim messaggioErrore As String = ""
        Dim dt As New DataTable

        stb.AppendLine(" select agronica.Data_Movimento, iSigpa.PIVA, iSigpa.N_progressivo_operazione, iSigpa.codice_fertilizzante, iSigpa.veg_cod_agea, vegAg.Veg_Des_Agea, iSigpa.quantità, iSigpa.qtaSommata, Agronica.Qta as Agronica_QTA, s.PRODOTTO, udmsalvata.UDM_SIM as udmsalvata_UDM_Sim, udmExtra.UDM_SIM as udmExtra_UDM_SIM, udmSigpa.UDM_SIM as udmSigpa_UDM_SIM, coalesce(convSigpa.FattoreConversione, 1) * coalesce(convSalvaToExtra.FattoreConversione, 1) as  FattoreConversione ")
        stb.AppendLine(" from (  ")
        stb.AppendLine("        select MT11.data_trattamento, ll.PIVA, MT11.N_progressivo_operazione, MT11.codice_fertilizzante , MT51.[codice prodotto] as veg_cod_agea, MT11.quantità, SUM(MT51.quantità) as qtaSommata ")
        stb.AppendLine("        from SIGPA_LogEsportazioni ll ")
        stb.AppendLine("        inner join SIGPA_MT11_ModuloDatiTrattamenti MT11 ")
        stb.AppendLine("            on ll.NumeroDellaFornitura = MT11.numero_della_fornitura  ")
        stb.AppendLine("            and ll.ProgressivoDellaFornitura = MT11.progressivo_della_fornitura  ")
        stb.AppendLine("        inner join SIGPA_MT51_ModuloParticelleOggettoDeiTrattamenti MT51 ")
        stb.AppendLine("            on MT11.N_progressivo_operazione = MT51.N_progressivo_operazione         ")
        stb.AppendLine("            and MT11.numero_della_fornitura = MT51.numero_della_fornitura         ")
        stb.AppendLine("            and MT11.identificativo_funzione_registro = MT51.identificativo_funzione_registro         ")
        stb.AppendLine("        where ")
        stb.AppendLine("        ll.NumeroDellaFornitura = " & Agro_SQL_SaveNum(NumeroDellaFornitura))

        stb.AppendLine("  ")
        stb.AppendLine("        group by MT11.data_trattamento, ll.PIVA, mt11.N_progressivo_operazione,MT11.codice_fertilizzante , MT51.[codice prodotto], MT11.quantità ")
        stb.AppendLine("    ) iSigpa ")
        stb.AppendLine("    inner join ( ")
        stb.AppendLine("  ")
        stb.AppendLine("        select m.Data_Movimento, A.PIVA, cc.val_cod as veg_cod_Agea, cac.Cod_Prodotto_Cliente, sum(Mov_Dest.Qta) as qta, dd.Udm_Cod, dd.Extra_Int ")
        stb.AppendLine("        from Agenda A        ")
        stb.AppendLine("        inner join Movimenti m ")
        stb.AppendLine("            on m.Id_Agenda = A.Id_Agenda  ")
        stb.AppendLine("            and m.PIVA = A.PIVA  ")
        stb.AppendLine("  ")
        stb.AppendLine("  ")
        stb.AppendLine("        inner join Movimenti_dettagli dd ")
        stb.AppendLine("            on m.Id_Agenda = dd.Id_Agenda  ")
        stb.AppendLine("            and m.Id_Mov = dd.Id_Mov  ")
        stb.AppendLine("            and m.PIVA = dd.PIVA  ")
        stb.AppendLine("            and m.Cau_Mov = '2300' ")
        stb.AppendLine("  ")
        stb.AppendLine("        inner join CAC_Codifica_ProdottiAziendali cac ")
        stb.AppendLine("            on cac.Codice_GIAS = dd.Pro_Cod  ")
        stb.AppendLine("  ")
        stb.AppendLine("         INNER JOIN Mov_Destinazioni Mov_Dest ")
        stb.AppendLine("             ON Mov_Dest.PIVA = dd.PIVA    ")
        stb.AppendLine("             AND Mov_Dest.Sa_Cod = dd.Sa_Cod    ")
        stb.AppendLine("             AND Mov_Dest.Id_Agenda = dd.Id_Agenda    ")
        stb.AppendLine("             AND Mov_Dest.Id_Mov = dd.Id_Mov    ")
        stb.AppendLine("             AND Mov_Dest.Id_Mov_Det = dd.Id_Mov_Det  ")
        stb.AppendLine("  ")
        stb.AppendLine("         INNER JOIN Reg_Impianti imp  ")
        stb.AppendLine("             ON imp.PIVA = Mov_Dest.PIVA    ")
        stb.AppendLine("             AND imp.Sa_Cod = Mov_Dest.Sa_Cod    ")
        stb.AppendLine("             AND imp.APPEZZA  = Mov_Dest.Appezza   ")
        stb.AppendLine("             and imp.ID_REG = Mov_Dest.Id_Destinazione  ")
        stb.AppendLine("  ")
        stb.AppendLine("        INNER JOIN Reg_Impianti_Codici cc  ")
        stb.AppendLine("            on cc.PIVA = imp.PIVA   ")
        stb.AppendLine("            and cc.sa_cod = imp.SA_COD   ")
        stb.AppendLine("            and cc.appezza = imp.APPEZZA   ")
        stb.AppendLine("            and cc.Id_Reg = imp.ID_REG   ")
        stb.AppendLine("            and cc.id_cod = 1133 ")
        stb.AppendLine("         group by m.Data_Movimento, A.PIVA, cc.val_cod, cac.Cod_Prodotto_Cliente, dd.Udm_Cod, dd.Extra_Int  ")
        stb.AppendLine("    ) Agronica ")
        stb.AppendLine("  ")
        stb.AppendLine("    on Agronica.PIVA = iSigpa.PIVA ")
        stb.AppendLine("    and Agronica.Data_Movimento = iSigpa.data_trattamento  ")
        stb.AppendLine("    and Agronica.Cod_Prodotto_Cliente = iSigpa.codice_fertilizzante  ")
        stb.AppendLine("    and Agronica.veg_cod_Agea = iSigpa.veg_cod_agea  ")
        stb.AppendLine("  ")
        stb.AppendLine("    left JOIN SIGPA_CodificaFertilizzanti s ")
        stb.AppendLine("        on s.CODICE = agronica.Cod_Prodotto_Cliente  ")
        stb.AppendLine("  ")
        stb.AppendLine("  ")
        stb.AppendLine("     inner join UnitaMisura udmSalvata ")
        stb.AppendLine("        on udmSalvata.UDM_COD = Agronica.Udm_Cod ")
        stb.AppendLine("  ")
        stb.AppendLine("  ")
        stb.AppendLine("    LEFT join UnitaMisura udmExtra ")
        stb.AppendLine("        on udmExtra.UDM_COD = agronica.Extra_Int   ")
        stb.AppendLine("  ")
        stb.AppendLine("    left join UnitaMisura udmSigpa  ")
        stb.AppendLine("        on udmSigpa.UDM_SIM = s.UNITA_Misura collate Latin1_General_CI_AS ")
        stb.AppendLine("  ")
        stb.AppendLine("    left join UnitaMisura_Conversione convSalvaToExtra ")
        stb.AppendLine("        on convSalvaToExtra.UDM_COD_Da = agronica.Udm_Cod  ")
        stb.AppendLine("        and convSalvaToExtra.UDM_COD_A = agronica.Extra_Int   ")
        stb.AppendLine("  ")
        stb.AppendLine("  ")
        stb.AppendLine("    left join UnitaMisura_Conversione convSigpa ")
        stb.AppendLine("        on convSigpa.UDM_COD_Da = agronica.Extra_Int  ")
        stb.AppendLine("        and convSigpa.UDM_COD_A = udmSigpa.Udm_Cod   ")
        stb.AppendLine("  ")
        stb.AppendLine("    left join (select distinct veg_cod_agea, veg_Des_Agea from  Codifica_SpecieVegetali_Agea) vegAg ")
        stb.AppendLine("        on vegAg.Veg_Cod_Agea = iSigpa.veg_cod_agea  ")
        stb.AppendLine("  ")

        Try

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

    Public Function ElencoAziendePerChiudGiacenze_Sigpa(ByVal SIGPA_ComandiEsportazione_cod As Integer,
                                                        ByVal PIVA_PadreInGerarchia As String,
                                                        ByVal DataRilievoChiusuraGiacenze As DateTime,
                                                        ByVal Esportazione_Elem_cod As Integer,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                        ByVal iBaseCode As Integer
                                                        ) As DataTable

        Dim nomeRoutine As String = "ElencoAziendePerChiudGiacenze_Sigpa"

        Dim stb As New StringBuilder
        Dim messaggioErrore As String = ""
        Dim dt As New DataTable

        Dim tipo_chiusuraGiacenze As enum_CodiciAnagrafe = enum_CodiciAnagrafe.TracciaEsportazioneSIGPA_ChiusuraGiacenze_XFertilizzanti

        If Esportazione_Elem_cod <> 3 Then
            tipo_chiusuraGiacenze = enum_CodiciAnagrafe.TracciaEsportazioneSIGPA_ChiusuraGiacenze_XFormulati
        End If


        Dim servizio_cod As Integer = 4
        If Esportazione_Elem_cod <> 3 Then
            servizio_cod = 3
        End If

        '----------------------------------------------------
        '--- Preparo la Query SQL ---------------------------
        '----------------------------------------------------
        Try
            stb.Length = 0

            stb.AppendLine("    select   ")
            stb.AppendLine("       F.PIVA      ")
            stb.AppendLine("     , F.Validita_Inizio   ")
            stb.AppendLine("     , F.rag_soc   ")
            stb.AppendLine("     , Imprese_Codici.val_cod as CUAA_Impresa  ")
            stb.AppendLine("     , 0 as Giacenza  ")
            stb.AppendLine("     , 0 as Codice_Prodotto_SIGPA  ")
            stb.AppendLine("     , 0 as udm_sim    ")
            stb.AppendLine("     , P.Pratica_Cod    ")

            stb.AppendLine(" from Imprese F ")
            stb.AppendLine(" Inner join ( select distinct figlio from GerarchieImpreseEsplosa  where Padre in  (" & PIVA_PadreInGerarchia & ")  ) es  ")
            stb.AppendLine("         on es.Figlio = F.PIVA   ")

            '  Vanni, 07/02/2014 10:33:39: filtro per una o più imprese
            If SIGPA_ComandiEsportazione_cod > 0 Then
                stb.AppendLine("    inner join SIGPA_ComandiEsportazione_ListaImprese LImp ")
                stb.AppendLine("    on LImp.piva = F.Piva ")
                stb.AppendLine("    and LImp.SIGPA_ComandiEsportazione_cod = " & Agro_SQL_SaveNum(SIGPA_ComandiEsportazione_cod))
            End If

            stb.AppendLine("    inner join Pratiche p ")
            stb.AppendLine("        on p.Piva = F.PIVA   ")
            stb.AppendLine("        and p.Servizio_Cod = " & servizio_cod)
            stb.AppendLine("        and p.Piva_Superuser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")

            'Marcata valida ed esportabile
            stb.AppendLine("  ")
            stb.AppendLine("    inner join Pratiche_Stati_Attuali SP ")
            stb.AppendLine("        on SP.Pratica_Cod = p.Pratica_Cod  ")
            stb.AppendLine("        and SP.Stato_Cod in (2) ")


            'fine chiusura giacenze iniziali
            stb.AppendLine("    inner join Pratiche_Stati_Attuali SP1 ")
            stb.AppendLine("        on SP1.Pratica_Cod = p.Pratica_Cod  ")
            stb.AppendLine("        and SP1.Stato_Cod = 6 ")


            stb.AppendLine("    INNER JOIN Imprese_Codici ")
            stb.AppendLine("         on Imprese_Codici.PIVA = F.PIVA   ")
            stb.AppendLine("         and Imprese_Codici.id_cod = 1010  ")

            'il cui record di chiusura giacenze non sia stato già esportato!
            stb.AppendLine("    left join Imprese_Codici fc ")
            stb.AppendLine("    on fc.PIVA = F.PIVA  ")
            stb.AppendLine("    and fc.id_cod = " & tipo_chiusuraGiacenze)


            stb.AppendLine(" where F.Validita_Inizio <= " & Agro_SQL_SaveDate(DataRilievoChiusuraGiacenze))
            stb.AppendLine(" and substring(F.PIVA, 1,2) <> 'UZ' ")
            stb.AppendLine(" and F.PIVA not in (" & Agro_SQL_Save_Clausola_IN(PIVA_PadreInGerarchia, True) & ")  ")
            stb.AppendLine(" and fc.id_cod is null ")


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

    Public Function ElencoMagazzini_SIGPA(ByVal SIGPA_ComandiEsportazione_cod As Integer,
                                          ByVal PIVA_PadreInGerarchia As String,
                                          ByVal DataRilievoMagazzini As DateTime,
                                          ByVal Esportazione_Elem_cod As Integer,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                          ByVal iBaseCode As Integer
                                          ) As DataTable

        Dim nomeRoutine As String = "ElencaAziendeMovimentatePriveDiCertificatoSIAN"

        Dim stb As New StringBuilder
        Dim messaggioErrore As String = ""
        Dim dt As New DataTable

        Dim TipoMagazzino As enum_CodiciAnagrafe = enum_CodiciAnagrafe.TracciaEsportazioneSIGPA_Fabbricati_XFertilizzanti

        If Esportazione_Elem_cod <> 3 Then
            TipoMagazzino = enum_CodiciAnagrafe.TracciaEsportazioneSIGPA_Fabbricati_XFormulati
        End If

        Dim servizio_cod As Integer = 4
        If Esportazione_Elem_cod <> 3 Then
            servizio_cod = 3
        End If


        '----------------------------------------------------
        '--- Preparo la Query SQL ---------------------------
        '----------------------------------------------------
        Try
            stb.Length = 0

            stb.AppendLine(" select  ")
            stb.AppendLine("      F.PIVA     ")
            stb.AppendLine("    , F.Fabbricato_Cod - " & iBaseCode & " as N_Progressivo ")
            stb.AppendLine("    , F.SA_COD ")
            stb.AppendLine("    , F.Validita_Inizio  ")
            stb.AppendLine("    , F.Fabbricato_Des  ")
            stb.AppendLine("    , I.ind_des + ' ' + I.frz_des as Indirizzo ")
            stb.AppendLine("    , I.pro_cod_istat  ")
            stb.AppendLine("    , I.com_cod_istat  ")
            stb.AppendLine("    , I.CAP  ")
            stb.AppendLine("    , Imprese_Codici.val_cod as CUAA_Impresa ")
            stb.AppendLine("    , 0 as Giacenza ")
            stb.AppendLine("    , 0 as Codice_Prodotto_SIGPA ")
            stb.AppendLine("    , 0 as udm_sim ")
            stb.AppendLine("    , p.pratica_cod ")
            'stb.AppendLine("    , coalesce(deco.codice, '000') as uff_cod ")

            stb.AppendLine(" from Fabbricati F ")
            stb.AppendLine(" inner join  ( select distinct figlio from GerarchieImpreseEsplosa  where Padre in  (" & Agro_SQL_Save_Clausola_IN(PIVA_PadreInGerarchia, True) & ")  ) es  ")
            stb.AppendLine("         on es.Figlio = F.PIVA   ")

            If SIGPA_ComandiEsportazione_cod > 0 Then
                stb.AppendLine("    inner join SIGPA_ComandiEsportazione_ListaImprese LImp ")
                stb.AppendLine("    on LImp.piva = F.Piva ")
                stb.AppendLine("    and LImp.SIGPA_ComandiEsportazione_cod = " & Agro_SQL_SaveNum(SIGPA_ComandiEsportazione_cod))
            End If


            stb.AppendLine("    inner join Pratiche p ")
            stb.AppendLine("        on p.Piva = F.PIVA   ")
            stb.AppendLine("        and p.Servizio_Cod = " & servizio_cod)
            stb.AppendLine("        and p.Piva_Superuser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")

            stb.AppendLine("  ")
            stb.AppendLine("    inner join Pratiche_Stati_Attuali SP ")
            stb.AppendLine("        on SP.Pratica_Cod = p.Pratica_Cod  ")
            stb.AppendLine("        and SP.Stato_Cod in (2) ")

            stb.AppendLine("  ")
            stb.AppendLine("    inner join Indirizzi i  ")
            stb.AppendLine("    on F.Indirizzo_Cod = i.cod_indirizzo  ")
            stb.AppendLine("  ")

            stb.AppendLine("    INNER JOIN Imprese_Codici ")
            stb.AppendLine("         on Imprese_Codici.PIVA = F.PIVA   ")
            stb.AppendLine("         and Imprese_Codici.id_cod = 1010  ")

            stb.AppendLine("    left join Fabbricati_Codici fc ")
            stb.AppendLine("    on fc.PIVA = F.PIVA  ")
            stb.AppendLine("    and fc.sa_cod = F.SA_COD  ")
            stb.AppendLine("    and fc.Fabbricato_cod = F.Fabbricato_Cod  ")
            stb.AppendLine("    and fc.id_cod = " & TipoMagazzino)

            'stb.AppendLine("    left join GerarchiaImprese gi ")
            'stb.AppendLine("        on gi.Figlio = f.PIVA  ")
            'stb.AppendLine("        and substring(gi.Padre , 1,2) = 'UZ' ")
            'stb.AppendLine("  ")
            'stb.AppendLine("    left join sigpa_ufficiZonaDecodifica deco ")
            'stb.AppendLine("        on deco.cod_agea = substring (gi.Padre, 3, 3) ")
            'stb.AppendLine("        and deco.cod_prov_agea = substring (gi.Padre, 6, 3) ")
            'stb.AppendLine("        and deco.cod_uff_agea  = substring (gi.Padre, 9, 3) ")


            stb.AppendLine(" where F.Validita_Inizio <= " & Agro_SQL_SaveDate(DataRilievoMagazzini))
            stb.AppendLine(" and substring(F.PIVA, 1,2) <> 'UZ' ")
            stb.AppendLine(" and F.PIVA not in (" & Agro_SQL_Save_Clausola_IN(PIVA_PadreInGerarchia, True) & ")  ")
            stb.AppendLine(" and fc.id_cod is null ")


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

    Public Function ElencaImpiantiMovimentatiPriviDiDecodificaSpecieAGEA(ByVal PIVA_PadreInGerarchia As String,
                                                                         ByVal DataInizio As DateTime,
                                                                         ByVal DataFine As DateTime,
                                                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                         ) As DataTable

        Dim nomeRoutine As String = "ElencaAziendeMovimentatePriveDiCertificatoSIAN"

        Dim stb As New StringBuilder
        Dim messaggioErrore As String = ""
        Dim dt As New DataTable

        '----------------------------------------------------
        '--- Preparo la Query SQL ---------------------------
        '----------------------------------------------------
        Try
            stb.Length = 0
            'basata su reg_impianti_codici, scritti quando viene chiamato il ws di aggiornamento!

            stb.AppendLine(" Select distinct ")
            stb.AppendLine("                 'Impresa: ' + i.piva + ' - ' + i.rag_soc + ' --- Dati impianto colturale: ' + veg.veg_des + ' - ' + cul.cul_des + ' - ' + fin.grfi_des + ' - ' + grva.grva_des as Descrizione   ")
            stb.AppendLine(" ,   ")
            stb.AppendLine("                 'Codice Specie: ' + cast(veg.Veg_Cod as varchar(100)) +    ")
            stb.AppendLine("                 ' - Codice Varietà: ' + cast(cul.cul_cod as varchar(100)) +   ")
            stb.AppendLine("                 ' - Codice Finalità: ' + cast(fin.grfi_cod as varchar(100)) +    ")
            stb.AppendLine("                 ' - Codice Gruppo Varietale: ' + cast(grva.grva_cod as varchar(100))   ")
            stb.AppendLine(" as Codice   ")
            stb.AppendLine(" from  ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" Select distinct  ")
            stb.AppendLine("    imp.piva ")
            stb.AppendLine("    , veg.Veg_Cod ")
            stb.AppendLine("     , cul.cul_cod  ")
            stb.AppendLine("     , imp.grfi_cod              ")
            stb.AppendLine("     , imp.GRVA_Cod_VEG  ")
            stb.AppendLine("    , ccTare.id_cod  ")
            stb.AppendLine(" FROM    Agenda     ")

            stb.AppendLine("    inner join Pratiche p ")
            stb.AppendLine("        on p.Piva = Agenda.PIVA   ")
            stb.AppendLine("        and p.Servizio_Cod in (3,4) ")
            stb.AppendLine("        and p.Piva_Superuser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")

            stb.AppendLine("  ")
            stb.AppendLine("    inner join Pratiche_Stati_Attuali SP ")
            stb.AppendLine("        on SP.Pratica_Cod = p.Pratica_Cod  ")
            stb.AppendLine("        and SP.Stato_Cod in (2) ")


            stb.AppendLine(" INNER JOIN Movimenti Movimenti_Concimi     ")
            stb.AppendLine("   ON Agenda.PIVA = Movimenti_Concimi.PIVA     ")
            stb.AppendLine("   AND Agenda.Sa_Cod = Movimenti_Concimi.Sa_Cod     ")
            stb.AppendLine("   AND Agenda.Id_Agenda = Movimenti_Concimi.Id_Agenda    ")
            stb.AppendLine("  ")
            stb.AppendLine(" INNER JOIN Movimenti_dettagli Mov_Det_Concimi     ")
            stb.AppendLine("       ON Mov_Det_Concimi.PIVA = Movimenti_Concimi.PIVA     ")
            stb.AppendLine("       AND Mov_Det_Concimi.Sa_Cod = Movimenti_Concimi.Sa_Cod     ")
            stb.AppendLine("       AND Mov_Det_Concimi.Id_Agenda = Movimenti_Concimi.Id_Agenda     ")
            stb.AppendLine("       AND Mov_Det_Concimi.Id_Mov = Movimenti_Concimi.Id_Mov     ")
            stb.AppendLine("  ")
            stb.AppendLine(" INNER JOIN Mov_Destinazioni Mov_Dest_Concimi     ")
            stb.AppendLine("       ON Mov_Dest_Concimi.PIVA = Mov_Det_Concimi.PIVA     ")
            stb.AppendLine("       AND Mov_Dest_Concimi.Sa_Cod = Mov_Det_Concimi.Sa_Cod     ")
            stb.AppendLine("       AND Mov_Dest_Concimi.Id_Agenda = Mov_Det_Concimi.Id_Agenda     ")
            stb.AppendLine("       AND Mov_Dest_Concimi.Id_Mov = Mov_Det_Concimi.Id_Mov     ")
            stb.AppendLine("       AND Mov_Dest_Concimi.Id_Mov_Det = Mov_Det_Concimi.Id_Mov_Det     ")
            stb.AppendLine("  ")
            stb.AppendLine(" inner join   ( select distinct figlio from GerarchieImpreseEsplosa  where Padre in  (" & PIVA_PadreInGerarchia & ")  ) es  ")
            stb.AppendLine("       on es.Figlio = Agenda.PIVA     ")
            stb.AppendLine("  ")
            stb.AppendLine(" INNER JOIN Reg_Impianti imp   ")
            stb.AppendLine("       ON imp.PIVA = Mov_Dest_Concimi.PIVA     ")
            stb.AppendLine("       AND imp.Sa_Cod = Mov_Dest_Concimi.Sa_Cod     ")
            stb.AppendLine("       AND imp.APPEZZA  = Mov_Dest_Concimi.Appezza    ")
            stb.AppendLine("       and imp.ID_REG = Mov_Dest_Concimi.Id_Destinazione    ")
            stb.AppendLine("  ")
            stb.AppendLine("  ")
            stb.AppendLine(" INNER JOIN (    ")
            stb.AppendLine("          select grva_cod, grva_des    ")
            stb.AppendLine("          from GruppoVarietale grva   ")
            stb.AppendLine("          union    ")
            stb.AppendLine("          select 0,'Gruppo varietale non specificato'   ")
            stb.AppendLine("      ) grva   ")
            stb.AppendLine("      on grva.Grva_Cod = imp.GRVA_Cod_VEG    ")
            stb.AppendLine("  ")
            stb.AppendLine(" LEFT JOIN Cultivar cul   ")
            stb.AppendLine("  on cul.Cul_Cod = imp.CUL_COD    ")
            stb.AppendLine("  and imp.CUL_COD<>0   ")
            stb.AppendLine("  ")
            stb.AppendLine(" inner join SpecieVegetali veg   ")
            stb.AppendLine("  on veg.veg_cod = cul.veg_cod   ")
            stb.AppendLine("  ")
            stb.AppendLine(" left JOIN Reg_Impianti_Codici cc   ")
            stb.AppendLine("  on cc.PIVA = imp.PIVA    ")
            stb.AppendLine("  and cc.sa_cod = imp.SA_COD    ")
            stb.AppendLine("  and cc.appezza = imp.APPEZZA    ")
            stb.AppendLine("  and cc.Id_Reg = imp.ID_REG    ")
            stb.AppendLine("  and cc.id_cod = 1133   ")
            stb.AppendLine("  ")
            stb.AppendLine("  left JOIN Reg_Impianti_Codici ccTare   ")
            stb.AppendLine("  on cc.PIVA = imp.PIVA    ")
            stb.AppendLine("  and cc.sa_cod = imp.SA_COD    ")
            stb.AppendLine("  and cc.appezza = imp.APPEZZA    ")
            stb.AppendLine("  and cc.Id_Reg = imp.ID_REG    ")
            stb.AppendLine("  ")
            stb.AppendLine(" WHERE Movimenti_Concimi.Data_Movimento <=  " & Agro_SQL_SaveDate(DataFine) & "   ")
            stb.AppendLine("  AND Movimenti_Concimi.Data_Movimento >=  " & Agro_SQL_SaveDate(DataInizio) & "   ")

            stb.AppendLine(" AND     Movimenti_Concimi.Cau_Mov = '2300'    ")
            stb.AppendLine(" AND   Agenda.Lav_Cod IN ( 14, 106, 26, 156, 123, 124  )   ")
            stb.AppendLine(" AND Agenda.Blocco_Flag = 0  ")
            stb.AppendLine(" and cc.val_cod is null ")
            stb.AppendLine("  ")
            stb.AppendLine(" ) imp ")
            stb.AppendLine(" LEFT JOIN Codifica_SpecieVegetali_Agea cveg  ")
            stb.AppendLine("     on (   ")
            stb.AppendLine("         imp.cul_Cod <> 0   ")
            stb.AppendLine("         and cveg.Veg_cod = imp.Veg_Cod   ")
            stb.AppendLine("         and (   ")
            stb.AppendLine("                 (cveg.Cul_cod = imp.Cul_Cod and   cveg.Grfi_Cod = 0 and cveg.Grva_cod = 0)  ")
            stb.AppendLine("             or (cveg.Cul_cod = 0 and cveg.Grfi_Cod = imp.GRFI_COD and cveg.Grva_cod = 0)  ")
            stb.AppendLine("             or (cveg.Cul_cod = 0 and cveg.Grfi_Cod = 0 and cveg.Grva_cod =imp.GRVA_Cod_VEG )  ")
            stb.AppendLine("         )  ")
            stb.AppendLine("     )   ")
            stb.AppendLine("     or  ")
            stb.AppendLine("     (  ")
            stb.AppendLine("          imp.CUL_COD = 0  ")
            stb.AppendLine("         and imp.id_cod = cveg.id_cod   ")
            stb.AppendLine("         --and cc.Id_Reg = cveg.Reg_cod   ")
            stb.AppendLine("     )  ")
            stb.AppendLine("  ")
            stb.AppendLine(" LEFT JOIN Cultivar cul   ")
            stb.AppendLine("  on cul.Cul_Cod = imp.CUL_COD    ")
            stb.AppendLine("  and imp.CUL_COD<>0   ")
            stb.AppendLine("  ")
            stb.AppendLine(" inner join SpecieVegetali veg   ")
            stb.AppendLine("  on veg.veg_cod = cul.veg_cod  ")
            stb.AppendLine("  ")
            stb.AppendLine(" inner join Imprese i  ")
            stb.AppendLine(" on i.PIVA = imp.PIVA   ")
            stb.AppendLine("  ")
            stb.AppendLine("  ")
            stb.AppendLine(" inner JOIN GruppoFinalita fin   ")
            stb.AppendLine("      on fin.Grfi_Cod = imp.GRFI_COD    ")
            stb.AppendLine("  ")
            stb.AppendLine(" INNER JOIN (    ")
            stb.AppendLine("          select grva_cod, grva_des    ")
            stb.AppendLine("          from GruppoVarietale grva   ")
            stb.AppendLine("          union    ")
            stb.AppendLine("          select 0,'Gruppo varietale non specificato'   ")
            stb.AppendLine("      ) grva   ")
            stb.AppendLine("      on grva.Grva_Cod = imp.GRVA_Cod_VEG  ")
            stb.AppendLine("  ")
            stb.AppendLine(" where cveg.Veg_Cod_Agea is null ")

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

    Public Function ElencaImpiantiMovimentatiPriviDiSpecieAGEA(ByVal PIVA_PadreInGerarchia As String,
                                                               ByVal DataInizio As DateTime,
                                                               ByVal DataFine As DateTime,
                                                               ByVal separatore As Char,
                                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                               ) As DataTable

        Dim nomeRoutine As String = "ElencaAziendeMovimentatePriveDiCertificatoSIAN"

        Dim stb As New StringBuilder
        Dim messaggioErrore As String = ""
        Dim dt As New DataTable

        '----------------------------------------------------
        '--- Preparo la Query SQL ---------------------------
        '----------------------------------------------------
        Try
            stb.Length = 0
            'basata su reg_impianti_codici, scritti quando viene chiamato il ws di aggiornamento!
            stb.AppendLine(" Select distinct ")
            stb.AppendLine("    i.piva + '" & separatore & "' + i.rag_soc + '" & separatore & "' + veg.veg_des + '" & separatore & "' + cul.cul_des + '" & separatore & "' + fin.grfi_des + '" & separatore & "' + grva.grva_des as Descrizione  ")
            stb.AppendLine(" ,  ")
            stb.AppendLine("                 '" & separatore & " Codice Specie: ' + cast(veg.Veg_Cod as varchar(100)) +   ")
            stb.AppendLine("                 '" & separatore & " Codice Varietà: ' + cast(cul.cul_cod as varchar(100)) +  ")
            stb.AppendLine("                 '" & separatore & " Codice Finalità: ' + cast(fin.grfi_cod as varchar(100)) +   ")
            stb.AppendLine("                 '" & separatore & " Codice Gruppo Varietale: ' + cast(grva.grva_cod as varchar(100))  ")
            stb.AppendLine(" as Codice  ")
            stb.AppendLine(" FROM    Agenda    ")
            stb.AppendLine(" INNER JOIN Movimenti Movimenti_Concimi    ")
            stb.AppendLine("      ON Agenda.PIVA = Movimenti_Concimi.PIVA    ")
            stb.AppendLine("      AND Agenda.Sa_Cod = Movimenti_Concimi.Sa_Cod    ")
            stb.AppendLine("      AND Agenda.Id_Agenda = Movimenti_Concimi.Id_Agenda   ")
            stb.AppendLine("  ")
            stb.AppendLine(" inner join Imprese i ")
            stb.AppendLine("    on i.PIVA = Agenda.PIVA  ")
            stb.AppendLine("  ")

            stb.AppendLine("    inner join Pratiche p ")
            stb.AppendLine("        on p.Piva = I.PIVA   ")
            stb.AppendLine("        and p.Servizio_Cod in (3, 4) ")
            stb.AppendLine("        and p.Piva_SuperUser = '" & objParametri.PivaSuperUser & "'")

            stb.AppendLine("  ")
            stb.AppendLine("    inner join Pratiche_Stati_Attuali SP ")
            stb.AppendLine("        on SP.Pratica_Cod = p.Pratica_Cod  ")
            stb.AppendLine("        and SP.Stato_Cod in (2) ")

            stb.AppendLine(" INNER JOIN Movimenti_dettagli Mov_Det_Concimi    ")
            stb.AppendLine("          ON Mov_Det_Concimi.PIVA = Movimenti_Concimi.PIVA    ")
            stb.AppendLine("          AND Mov_Det_Concimi.Sa_Cod = Movimenti_Concimi.Sa_Cod    ")
            stb.AppendLine("          AND Mov_Det_Concimi.Id_Agenda = Movimenti_Concimi.Id_Agenda    ")
            stb.AppendLine("          AND Mov_Det_Concimi.Id_Mov = Movimenti_Concimi.Id_Mov    ")
            stb.AppendLine("  ")
            stb.AppendLine(" INNER JOIN Mov_Destinazioni Mov_Dest_Concimi    ")
            stb.AppendLine("          ON Mov_Dest_Concimi.PIVA = Mov_Det_Concimi.PIVA    ")
            stb.AppendLine("          AND Mov_Dest_Concimi.Sa_Cod = Mov_Det_Concimi.Sa_Cod    ")
            stb.AppendLine("          AND Mov_Dest_Concimi.Id_Agenda = Mov_Det_Concimi.Id_Agenda    ")
            stb.AppendLine("          AND Mov_Dest_Concimi.Id_Mov = Mov_Det_Concimi.Id_Mov    ")
            stb.AppendLine("          AND Mov_Dest_Concimi.Id_Mov_Det = Mov_Det_Concimi.Id_Mov_Det    ")
            stb.AppendLine("  ")
            stb.AppendLine(" inner join   ( select distinct figlio from GerarchieImpreseEsplosa  where Padre in  (" & PIVA_PadreInGerarchia & ")  ) es  ")
            stb.AppendLine("          on es.Figlio = Agenda.PIVA    ")

            stb.AppendLine("  ")
            stb.AppendLine(" INNER JOIN Reg_Impianti imp  ")
            stb.AppendLine("          ON imp.PIVA = Mov_Dest_Concimi.PIVA    ")
            stb.AppendLine("          AND imp.Sa_Cod = Mov_Dest_Concimi.Sa_Cod    ")
            stb.AppendLine("          AND imp.APPEZZA  = Mov_Dest_Concimi.Appezza   ")
            stb.AppendLine("          and imp.ID_REG = Mov_Dest_Concimi.Id_Destinazione   ")
            stb.AppendLine("  ")
            stb.AppendLine(" INNER JOIN GruppoFinalita fin  ")
            stb.AppendLine("         on fin.Grfi_Cod = imp.GRFI_COD   ")
            stb.AppendLine("  ")
            stb.AppendLine(" INNER JOIN (   ")
            stb.AppendLine("             select grva_cod, grva_des   ")
            stb.AppendLine("             from GruppoVarietale grva  ")
            stb.AppendLine("             union   ")
            stb.AppendLine("             select 0,'Gruppo varietale non specificato'  ")
            stb.AppendLine("         ) grva  ")
            stb.AppendLine("         on grva.Grva_Cod = imp.GRVA_Cod_VEG   ")
            stb.AppendLine("  ")
            stb.AppendLine(" INNER JOIN Cultivar cul  ")
            stb.AppendLine("     on cul.Cul_Cod = imp.CUL_COD   ")
            stb.AppendLine("     and imp.CUL_COD<>0  ")
            stb.AppendLine("  ")
            stb.AppendLine(" inner join SpecieVegetali veg  ")
            stb.AppendLine("     on veg.veg_cod = cul.veg_cod  ")
            stb.AppendLine("  ")
            stb.AppendLine(" LEFT JOIN Reg_Impianti_Codici cc  ")
            stb.AppendLine("     on cc.PIVA = imp.PIVA   ")
            stb.AppendLine("     and cc.sa_cod = imp.SA_COD   ")
            stb.AppendLine("     and cc.appezza = imp.APPEZZA   ")
            stb.AppendLine("     and cc.Id_Reg = imp.ID_REG   ")
            stb.AppendLine("     and cc.id_cod = 1133  ")
            stb.AppendLine("  ")

            stb.AppendLine("  ")
            stb.AppendLine(" WHERE Movimenti_Concimi.Data_Movimento <=  " & Agro_SQL_SaveDate(DataFine) & "   ")
            stb.AppendLine("  AND Movimenti_Concimi.Data_Movimento >=  " & Agro_SQL_SaveDate(DataInizio) & "   ")

            stb.AppendLine(" AND     Movimenti_Concimi.Cau_Mov = '2300'    ")
            stb.AppendLine(" AND   Agenda.Lav_Cod IN ( 14, 106, 26, 156, 123, 124  )   ")
            stb.AppendLine(" AND Agenda.Blocco_Flag = 0  ")
            stb.AppendLine(" and cc.val_cod is null ")


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

    Public Function ElencaAziendeMovimentatePriveDiCertificatoSIAN(ByVal PIVA_PadreInGerarchia As String,
                                                                   ByVal DataInizio As DateTime,
                                                                   ByVal DataFine As DateTime,
                                                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                   ) As DataTable

        Dim nomeRoutine As String = "ElencaAziendeMovimentatePriveDiCertificatoSIAN"

        Dim stb As New StringBuilder
        Dim messaggioErrore As String = ""
        Dim dt As New DataTable

        '----------------------------------------------------
        '--- Preparo la Query SQL ---------------------------
        '----------------------------------------------------
        Try
            stb.Length = 0

            stb.AppendLine("            Select distinct ")
            stb.AppendLine("    I.PIVA, I.rag_soc  ")
            stb.AppendLine(" from  ")
            stb.AppendLine("    ( ")
            stb.AppendLine("        select  ")
            stb.AppendLine("              AppezzamentiXParticelle.Piva ")
            stb.AppendLine("            , AppezzamentiXParticelle.Sa_Cod  ")
            stb.AppendLine("            , AppezzamentiXParticelle.Appezza  ")
            stb.AppendLine("            , AppezzamentiXParticelle.PROV  ")
            stb.AppendLine("            , AppezzamentiXParticelle.COM  ")
            stb.AppendLine("            , AppezzamentiXParticelle.SEZIONE  ")
            stb.AppendLine("            , AppezzamentiXParticelle.FOGLIO  ")
            stb.AppendLine("            , AppezzamentiXParticelle.NUMERO  ")
            stb.AppendLine("            , AppezzamentiXParticelle.SUBALTERNO             ")
            stb.AppendLine("        from Agenda A  ")
            stb.AppendLine("        INNER JOIN   ( select distinct figlio from GerarchieImpreseEsplosa  where Padre in  (" & PIVA_PadreInGerarchia & ")  ) es  ")
            stb.AppendLine("            on ES.Figlio = A.PIVA   ")

            stb.AppendLine("  ")
            stb.AppendLine("        INNER JOIN Movimenti Movimenti_Concimi   ")
            stb.AppendLine("            ON A.PIVA = Movimenti_Concimi.PIVA   ")
            stb.AppendLine("            AND A.Sa_Cod = Movimenti_Concimi.Sa_Cod   ")
            stb.AppendLine("            AND A.Id_Agenda = Movimenti_Concimi.Id_Agenda  ")
            stb.AppendLine("  ")
            stb.AppendLine("        INNER JOIN Movimenti_dettagli Mov_Det_Concimi   ")
            stb.AppendLine("            ON Mov_Det_Concimi.PIVA = Movimenti_Concimi.PIVA   ")
            stb.AppendLine("            AND Mov_Det_Concimi.Sa_Cod = Movimenti_Concimi.Sa_Cod   ")
            stb.AppendLine("            AND Mov_Det_Concimi.Id_Agenda = Movimenti_Concimi.Id_Agenda   ")
            stb.AppendLine("            AND Mov_Det_Concimi.Id_Mov = Movimenti_Concimi.Id_Mov   ")
            stb.AppendLine("  ")
            stb.AppendLine("        INNER JOIN Mov_Destinazioni Mov_Dest_Concimi   ")
            stb.AppendLine("            ON Mov_Dest_Concimi.PIVA = Mov_Det_Concimi.PIVA   ")
            stb.AppendLine("            AND Mov_Dest_Concimi.Sa_Cod = Mov_Det_Concimi.Sa_Cod   ")
            stb.AppendLine("            AND Mov_Dest_Concimi.Id_Agenda = Mov_Det_Concimi.Id_Agenda   ")
            stb.AppendLine("            AND Mov_Dest_Concimi.Id_Mov = Mov_Det_Concimi.Id_Mov   ")
            stb.AppendLine("            AND Mov_Dest_Concimi.Id_Mov_Det = Mov_Det_Concimi.Id_Mov_Det   ")
            stb.AppendLine("  ")
            stb.AppendLine("         INNER JOIN Appezzamento    ")
            stb.AppendLine("            ON Mov_Dest_Concimi.PIVA = Appezzamento.PIVA   ")
            stb.AppendLine("            AND Mov_Dest_Concimi.Sa_Cod = Appezzamento.Sa_Cod   ")
            stb.AppendLine("            AND Mov_Dest_Concimi.Appezza = Appezzamento.Appezza   ")
            stb.AppendLine("  ")
            stb.AppendLine("        INNER JOIN AppezzamentiXParticelle    ")
            stb.AppendLine("            ON Appezzamento.PIVA = AppezzamentiXParticelle.PIVA   ")
            stb.AppendLine("            AND Appezzamento.Sa_Cod = AppezzamentiXParticelle.Sa_Cod   ")
            stb.AppendLine("            AND Appezzamento.Appezza = AppezzamentiXParticelle.Appezza   ")
            stb.AppendLine("  ")
            stb.AppendLine("  ")
            stb.AppendLine("        where A.Lav_Cod IN ( 14, 106, 26, 156, 123, 124  )   ")
            stb.AppendLine("        AND Movimenti_Concimi.Data_Movimento <=   " & Agro_SQL_SaveDate(DataFine) & "     ")
            stb.AppendLine("        AND     Movimenti_Concimi.Data_Movimento >=   " & Agro_SQL_SaveDate(DataInizio) & "     ")
            stb.AppendLine("        group by  ")
            stb.AppendLine("             AppezzamentiXParticelle.Piva ")
            stb.AppendLine("            , AppezzamentiXParticelle.Sa_Cod  ")
            stb.AppendLine("            , AppezzamentiXParticelle.Appezza  ")
            stb.AppendLine("            , AppezzamentiXParticelle.PROV  ")
            stb.AppendLine("            , AppezzamentiXParticelle.COM  ")
            stb.AppendLine("            , AppezzamentiXParticelle.SEZIONE  ")
            stb.AppendLine("            , AppezzamentiXParticelle.FOGLIO  ")
            stb.AppendLine("            , AppezzamentiXParticelle.NUMERO  ")
            stb.AppendLine("            , AppezzamentiXParticelle.SUBALTERNO             ")
            stb.AppendLine("   ) AG ")
            stb.AppendLine("   INNER JOIN imprese i ")
            stb.AppendLine("    on AG.Piva = i.PIVA  ")

            stb.AppendLine("    inner join Pratiche p ")
            stb.AppendLine("        on p.Piva = I.PIVA   ")
            stb.AppendLine("        and p.Servizio_Cod in (3, 4) ")
            stb.AppendLine("  ")
            stb.AppendLine("    inner join Pratiche_Stati_Attuali SP ")
            stb.AppendLine("        on SP.Pratica_Cod = p.Pratica_Cod  ")
            stb.AppendLine("        and SP.Stato_Cod in (2) ")

            stb.AppendLine("  ")
            stb.AppendLine("   LEFT JOIN Allegati_EntitaxDocumenti  D  ")
            stb.AppendLine("        on AG.PIVA = D.Piva  ")
            stb.AppendLine("        and AG.PROV = D.Prov  ")
            stb.AppendLine("        and AG.COM = D.Com  ")
            stb.AppendLine("        and AG.FOGLIO = D.Foglio  ")
            stb.AppendLine("        and AG.NUMERO = D.Numero  ")
            stb.AppendLine("        and AG.SUBALTERNO = AG.SUBALTERNO  ")
            stb.AppendLine("  ")
            stb.AppendLine("  ")
            stb.AppendLine("  ")
            stb.AppendLine("  ")
            stb.AppendLine(" where D.Piva is null ")
            stb.AppendLine("  ")


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

    Public Function ElencaProdottiPriviDiDecodifica_SIGPA(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                          ) As DataTable

        Dim nomeRoutine As String = "Contabilita_Movimenti_Dettagli_Leggi"

        Dim stb As New StringBuilder
        Dim messaggioErrore As String = ""
        Dim dt As New DataTable

        '----------------------------------------------------
        '--- Preparo la Query SQL ---------------------------
        '----------------------------------------------------
        Try
            stb.Length = 0


            stb.AppendLine(" select distinct  f.Fer_Cod, f.Fer_Des, f.N, F.P2O5, f.K2O  ")


            stb.AppendLine(" from Fertilizzanti F ")
            stb.AppendLine("  ")
            stb.AppendLine("     LEFT JOIN CAC_Codifica_ProdottiAziendali cac ")
            stb.AppendLine("         on cac.Codice_GIAS = f.Fer_Cod   ")
            stb.AppendLine("         and cac.Elem_Cod = 3  ")
            stb.AppendLine("         and cac.Tipo_Codifica = 5  ")

            stb.AppendLine("    WHERE   cac.Cod_Prodotto_Cliente is null ")


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

    Public Function LeggiOperazioniAgendaDatoFerCod(ByVal PIVA_PadreInGerarchia As String,
                                                    ByVal DataInizio As DateTime,
                                                    ByVal DataFine As DateTime,
                                                    ByVal fer_Cod As Integer,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                    ) As DataTable

        Dim nomeRoutine As String = "LeggiOperazioniAgendaDatoFerCod"

        Dim stb As New StringBuilder
        Dim messaggioErrore As String = ""
        Dim dt As New DataTable

        '----------------------------------------------------
        '--- Preparo la Query SQL ---------------------------
        '----------------------------------------------------
        Try
            stb.Length = 0
            stb.AppendLine(" select det.id_agenda, mm.Id_Mov, det.Id_Mov_Det ")
            stb.AppendLine(" from  ")
            stb.AppendLine("     Imprese i ")
            stb.AppendLine("  ")
            stb.AppendLine("    INNER JOIN GerarchieImpreseEsplosa    ")
            stb.AppendLine("          on GerarchieImpreseEsplosa.Figlio = i.PIVA    ")
            stb.AppendLine("          and GerarchieImpreseEsplosa.Padre in  ('02168620546', '01205100553')   ")
            stb.AppendLine("     inner join Movimenti mm  ")
            stb.AppendLine("        on mm.PIVA = i.PIVA  ")
            stb.AppendLine("  ")
            stb.AppendLine("    inner join Movimenti_dettagli det  ")
            stb.AppendLine("         on mm.Id_Agenda = det.Id_Agenda   ")
            stb.AppendLine("         and mm.Id_Mov = det.Id_Mov   ")
            stb.AppendLine("  ")
            stb.AppendLine("   INNER JOIN CAC_Codifica_ProdottiAziendali cac  ")
            stb.AppendLine("          on cac.Codice_GIAS = det.Pro_Cod    ")
            stb.AppendLine("          and cac.Elem_Cod = 3 ")
            stb.AppendLine("          and cac.Tipo_Codifica = 5 ")
            stb.AppendLine("  ")

            stb.AppendLine(" where pro_cod = " & fer_Cod)


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

    Public Function LeggiPerRiportaUDMSigpaSuOperazioniAgenda(ByVal PIVA_PadreInGerarchia As String,
                                                              ByVal DataInizio As DateTime,
                                                              ByVal DataFine As DateTime,
                                                              ByVal fer_Cod As Integer,
                                                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                              ) As DataTable

        Dim nomeRoutine As String = "LeggiPerRiportaUDMSigpaSuOperazioniAgenda"

        Dim stb As New StringBuilder
        Dim messaggioErrore As String = ""
        Dim dt As New DataTable

        '----------------------------------------------------
        '--- Preparo la Query SQL ---------------------------
        '----------------------------------------------------
        Try
            stb.Length = 0
            stb.AppendLine(" select det.id_agenda, mm.Id_Mov, det.Id_Mov_Det, det.Udm_Cod as udm_vecchio, u.UDM_COD as udm_nuovo ")
            stb.AppendLine(" from  ")
            stb.AppendLine("     Imprese i ")
            stb.AppendLine("  ")
            stb.AppendLine("    INNER JOIN GerarchieImpreseEsplosa    ")
            stb.AppendLine("          on GerarchieImpreseEsplosa.Figlio = i.PIVA    ")
            stb.AppendLine("          and GerarchieImpreseEsplosa.Padre in  ('02168620546', '01205100553')   ")
            stb.AppendLine("     inner join Movimenti mm  ")
            stb.AppendLine("        on mm.PIVA = i.PIVA  ")
            stb.AppendLine("  ")
            stb.AppendLine("    inner join Movimenti_dettagli det  ")
            stb.AppendLine("         on mm.Id_Agenda = det.Id_Agenda   ")
            stb.AppendLine("         and mm.Id_Mov = det.Id_Mov   ")
            stb.AppendLine("         and mm.Cau_Mov in ('7350', '7300') ")

            stb.AppendLine("  ")
            stb.AppendLine("   INNER JOIN CAC_Codifica_ProdottiAziendali cac  ")
            stb.AppendLine("          on cac.Codice_GIAS = det.Pro_Cod    ")
            stb.AppendLine("          and cac.Elem_Cod = 3 ")
            stb.AppendLine("          and cac.Tipo_Codifica = 5 ")
            stb.AppendLine("  ")
            stb.AppendLine("    inner JOIN SIGPA_CodificaFertilizzanti cod ")
            stb.AppendLine("        on cod.CODICE = cac.Cod_Prodotto_Cliente ")
            stb.AppendLine("  ")
            stb.AppendLine("    inner join UnitaMisura u ")
            stb.AppendLine("        on u.UDM_SIM = cod.UNITA_Misura  collate Latin1_General_CI_AS ")
            stb.AppendLine("  ")
            stb.AppendLine(" where pro_cod = " & fer_Cod)


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

    Public Function ElencaProdottiMovimentatiConUDM_SIGPA_Differente(
                                                    ByVal PIVA_PadreInGerarchia As String,
                                                    ByVal DataInizio As DateTime,
                                                    ByVal DataFine As DateTime,
                                                    ByVal ElencaSoloProdotti As Boolean,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As DataTable

        Dim nomeRoutine As String = "ElencaProdottiMovimentatiConUDM_SIGPA_Differente"

        Dim stb As New StringBuilder
        Dim messaggioErrore As String = ""
        Dim dt As New DataTable

        '----------------------------------------------------
        '--- Preparo la Query SQL ---------------------------
        '----------------------------------------------------
        Try
            stb.Length = 0

            stb.AppendLine(" select   ")
            stb.AppendLine("  ")
            stb.AppendLine("      f.Fer_Cod ")
            stb.AppendLine("    , f.Fer_Des ")
            stb.AppendLine("    , f.N  ")
            stb.AppendLine("    , f.P2O5  ")
            stb.AppendLine("    , f.K2O  ")
            stb.AppendLine("    , u.UDM_SIM as UDM_GIAS ")
            stb.AppendLine("    , cod.CODICE  ")
            stb.AppendLine("    , cod.PRODOTTO ")
            stb.AppendLine("    , cod.TITAZOTO  ")
            stb.AppendLine("    , cod.TITFOSFORO  ")
            stb.AppendLine("    , cod.TITPOTASSIO   ")
            stb.AppendLine("    , Cod.UNITA_Misura as UDM ")
            stb.AppendLine("    , 'Conteggio Operazioni: ' + cast(COUNT (*) as varchar(100)) as Note ")
            stb.AppendLine("  from    Imprese i ")
            stb.AppendLine("  ")
            stb.AppendLine("    INNER JOIN GerarchieImpreseEsplosa    ")
            stb.AppendLine("          on GerarchieImpreseEsplosa.Figlio = i.PIVA    ")
            stb.AppendLine("          and GerarchieImpreseEsplosa.Padre in  ('02168620546', '01205100553')   ")
            stb.AppendLine("  ")
            stb.AppendLine("    inner join Imprese_Codici c ")
            stb.AppendLine("        on c.PIVA = i.PIVA  ")
            stb.AppendLine("        and c.id_cod = 1010 ")
            stb.AppendLine("  ")
            stb.AppendLine("    inner join Pratiche p ")
            stb.AppendLine("        on p.Piva = I.PIVA   ")
            stb.AppendLine("        and p.Servizio_Cod = 4 ")
            stb.AppendLine("  ")
            stb.AppendLine("    inner join Pratiche_Stati_Attuali  SP ")
            stb.AppendLine("        on SP.Pratica_Cod = p.Pratica_Cod  ")
            stb.AppendLine("        and SP.Stato_Cod in (2) ")
            stb.AppendLine("  ")
            stb.AppendLine("    inner join Agenda A  ")
            stb.AppendLine("        on A.PIVA =i.piva ")
            stb.AppendLine("  ")
            stb.AppendLine("     inner join Movimenti mm  ")
            stb.AppendLine("         on mm.Id_Agenda = A.Id_Agenda   ")
            stb.AppendLine("         and A.Lav_Cod in ( 14, 106, 26, 156, 123, 124, 1001, 1000, 1023, 1022)    ")
            stb.AppendLine("  ")
            stb.AppendLine("     inner join Movimenti_dettagli det  ")
            stb.AppendLine("         on mm.Id_Agenda = A.Id_Agenda   ")
            stb.AppendLine("         and mm.Id_Mov = det.Id_Mov   ")
            stb.AppendLine("         and mm.Cau_Mov in ('7350', '7300') ")
            stb.AppendLine("  ")
            stb.AppendLine("  ")
            stb.AppendLine("     INNER JOIN Fertilizzanti F  ")
            stb.AppendLine("         on F.Fer_Cod = det.Pro_Cod   ")
            stb.AppendLine("         and det.Elem_Cod = 3  ")
            stb.AppendLine("  ")
            stb.AppendLine("      INNER JOIN CAC_Codifica_ProdottiAziendali cac  ")
            stb.AppendLine("          on cac.Codice_GIAS = det.Pro_Cod    ")
            stb.AppendLine("          and cac.Elem_Cod = 3 ")
            stb.AppendLine("          and cac.Tipo_Codifica = 5 ")
            stb.AppendLine("  ")
            stb.AppendLine("      inner JOIN SIGPA_CodificaFertilizzanti cod ")
            stb.AppendLine("        on cod.CODICE = cac.Cod_Prodotto_Cliente  ")
            stb.AppendLine("  ")
            stb.AppendLine("    inner join UnitaMisura u ")
            stb.AppendLine("        on   u.UDM_COD = det.Udm_Cod ")
            stb.AppendLine("        and cod.UNITA_Misura <> u.UDM_SIM collate  Latin1_General_CI_AS ")
            stb.AppendLine("  ")



            stb.AppendLine("  ")
            stb.AppendLine("    WHERE   mm.Data_Movimento <=  " & Agro_SQL_SaveDate(DataFine) & "   ")
            stb.AppendLine("     AND     mm.Data_Movimento >=  " & Agro_SQL_SaveDate(DataInizio) & "   ")
            stb.AppendLine("  ")
            stb.AppendLine("      group by  ")
            stb.AppendLine("          f.Fer_Cod ")
            stb.AppendLine("        , f.Fer_Des ")
            stb.AppendLine("        , f.N  ")
            stb.AppendLine("        , f.P2O5  ")
            stb.AppendLine("        , f.K2O  ")
            stb.AppendLine("        , u.UDM_SIM  ")
            stb.AppendLine("        , cod.CODICE  ")
            stb.AppendLine("        , cod.PRODOTTO ")
            stb.AppendLine("        , cod.TITAZOTO  ")
            stb.AppendLine("        , cod.TITFOSFORO  ")
            stb.AppendLine("        , cod.TITPOTASSIO   ")
            stb.AppendLine("        , Cod.UNITA_Misura  ")
            stb.AppendLine("  ")
            stb.AppendLine("      order by F.Fer_Des ")


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

    Public Function ElencaProdottiMovimentatiConDecodificaErroneaOppureRimossi_SIGPA(
                                                ByVal PIVA_PadreInGerarchia As String,
                                                ByVal DataInizio As DateTime,
                                                ByVal DataFine As DateTime,
                                                ByVal ElencaSoloProdotti As Boolean,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As DataTable

        Dim nomeRoutine As String = "ElencaProdottiMovimentatiConDecodificaErroneaOppureRimossi_SIGPA"

        Dim stb As New StringBuilder
        Dim messaggioErrore As String = ""
        Dim dt As New DataTable

        '----------------------------------------------------
        '--- Preparo la Query SQL ---------------------------
        '----------------------------------------------------
        Try
            stb.Length = 0

            stb.AppendLine("Select distinct ")
            stb.AppendLine("      f.Fer_Cod ")
            stb.AppendLine("    , f.Fer_Des ")
            stb.AppendLine("    , f.N ")
            stb.AppendLine("    , f.P2O5 ")
            stb.AppendLine("    , f.K2O ")
            stb.AppendLine("    , '' as UDM_GIAS ")
            stb.AppendLine("    , cac.Cod_Prodotto_Cliente as Codice ")
            stb.AppendLine("    , '' as Prodotto ")
            stb.AppendLine("    , '' as TitAzoto ")
            stb.AppendLine("    , '' as TitFosforo ")
            stb.AppendLine("    , '' as TitPotassio ")
            stb.AppendLine("    , '' as UDM ")
            stb.AppendLine("    , 'Prodotto rimosso da sigpa' as Note ")
            stb.AppendLine("  ")
            stb.AppendLine("  ")
            stb.AppendLine("  from Agenda A    ")
            stb.AppendLine("  ")

            stb.AppendLine("  ")
            stb.AppendLine("    INNER JOIN GerarchieImpreseEsplosa    ")
            stb.AppendLine("          on GerarchieImpreseEsplosa.Figlio = A.PIVA    ")
            stb.AppendLine("          and GerarchieImpreseEsplosa.Padre in  (" & PIVA_PadreInGerarchia & ")   ")

            stb.AppendLine("    inner join Pratiche p ")
            stb.AppendLine("        on p.Piva = A.PIVA   ")
            stb.AppendLine("        and p.Servizio_Cod = 4 ")
            stb.AppendLine("  ")
            stb.AppendLine("    inner join Pratiche_Stati_Attuali  SP ")
            stb.AppendLine("        on SP.Pratica_Cod = p.Pratica_Cod  ")
            stb.AppendLine("        and SP.Stato_Cod in (2)               ")
            stb.AppendLine("  ")
            stb.AppendLine("     inner join Movimenti mm  ")
            stb.AppendLine("         on mm.Id_Agenda = A.Id_Agenda   ")
            stb.AppendLine("         and A.Lav_Cod in ( 14, 106, 26, 156, 123, 124, 1001, 1000, 1023, 1022)    ")
            stb.AppendLine("  ")
            stb.AppendLine("     inner join Movimenti_dettagli det  ")
            stb.AppendLine("         on mm.Id_Agenda = A.Id_Agenda   ")
            stb.AppendLine("         and mm.Id_Mov = det.Id_Mov   ")
            stb.AppendLine("  ")
            stb.AppendLine("    inner join CAC_Codifica_ProdottiAziendali cac ")
            stb.AppendLine("        on  cac.Codice_GIAS  = det.Pro_Cod   ")
            stb.AppendLine("         and det.Elem_Cod = 3 ")
            stb.AppendLine("         AND cac.Tipo_codifica =  " & enum_Tipo_CAC_Codifica_ProdottiAziendali.Coldiretti_RegioneUmbria)
            stb.AppendLine("  ")
            stb.AppendLine("    inner join Fertilizzanti f ")
            stb.AppendLine("        on f.Fer_Cod = cac.Codice_GIAS  ")
            stb.AppendLine("        and cac.Tipo_Codifica = 5 ")
            stb.AppendLine("    left join SIGPA_CodificaFertilizzanti sig ")
            stb.AppendLine("        on cac.Cod_Prodotto_Cliente = sig.CODICE  ")
            stb.AppendLine("  ")
            stb.AppendLine(" where sig.CODICE is null ")
            stb.AppendLine("    AND   mm.Data_Movimento <=  " & Agro_SQL_SaveDate(DataFine) & "   ")
            stb.AppendLine("     AND     mm.Data_Movimento >=  " & Agro_SQL_SaveDate(DataInizio) & "   ")
            stb.AppendLine("  ")
            stb.AppendLine(" UNION ALL ")
            stb.AppendLine("  ")
            stb.AppendLine(" select distinct  ")
            stb.AppendLine("      f.Fer_Cod ")
            stb.AppendLine("    , f.Fer_Des ")
            stb.AppendLine("    , f.N ")
            stb.AppendLine("    , f.P2O5 ")
            stb.AppendLine("    , f.K2O ")
            stb.AppendLine("    , '' as UDM_GIAS ")
            stb.AppendLine("    , cod.CODICE ")
            stb.AppendLine("    , cod.PRODOTTO ")
            stb.AppendLine("    , cod.TITAZOTO  ")
            stb.AppendLine("    , cod.TITFOSFORO  ")
            stb.AppendLine("    , cod.TITPOTASSIO  ")

            stb.AppendLine("    , cod.UNITA_MIS as UDM ")
            stb.AppendLine("    , cod.[MOTIVO CANCELLAZIONE] as Note     ")
            stb.AppendLine("  ")
            stb.AppendLine("  from Agenda A    ")

            stb.AppendLine("  ")
            stb.AppendLine("    INNER JOIN GerarchieImpreseEsplosa    ")
            stb.AppendLine("          on GerarchieImpreseEsplosa.Figlio = A.PIVA    ")
            stb.AppendLine("          and GerarchieImpreseEsplosa.Padre in  (" & PIVA_PadreInGerarchia & ")   ")
            stb.AppendLine("  ")
            stb.AppendLine("     inner join Imprese i ")
            stb.AppendLine("        on i.PIVA = A.PIVA  ")
            stb.AppendLine("  ")
            stb.AppendLine("    inner join Imprese_Codici c ")
            stb.AppendLine("        on c.PIVA = i.PIVA  ")
            stb.AppendLine("        and c.id_cod = 1010 ")
            stb.AppendLine("  ")
            stb.AppendLine("    inner join Pratiche p ")
            stb.AppendLine("        on p.Piva = A.PIVA   ")
            stb.AppendLine("        and p.Servizio_Cod = 4 ")
            stb.AppendLine("  ")
            stb.AppendLine("    inner join Pratiche_Stati_Attuali SP ")
            stb.AppendLine("        on SP.Pratica_Cod = p.Pratica_Cod  ")
            stb.AppendLine("        and SP.Stato_Cod in (2)                       ")
            stb.AppendLine("  ")
            stb.AppendLine("     inner join Movimenti mm  ")
            stb.AppendLine("         on mm.Id_Agenda = A.Id_Agenda   ")
            stb.AppendLine("         and A.Lav_Cod in ( 14, 106, 26, 156, 123, 124, 1001, 1000, 1023, 1022)    ")
            stb.AppendLine("  ")
            stb.AppendLine("     inner join Movimenti_dettagli det  ")
            stb.AppendLine("         on mm.Id_Agenda = A.Id_Agenda   ")
            stb.AppendLine("         and mm.Id_Mov = det.Id_Mov   ")
            stb.AppendLine("  ")
            stb.AppendLine("  ")
            stb.AppendLine("     INNER JOIN Fertilizzanti F  ")
            stb.AppendLine("         on F.Fer_Cod = det.Pro_Cod   ")
            stb.AppendLine("         and det.Elem_Cod = 3  ")
            stb.AppendLine("  ")
            stb.AppendLine("      INNER JOIN CAC_Codifica_ProdottiAziendali cac  ")
            stb.AppendLine("          on cac.Codice_GIAS = det.Pro_Cod    ")
            stb.AppendLine("          and cac.Elem_Cod = 3 ")
            stb.AppendLine("          and cac.Tipo_Codifica = 5 ")
            stb.AppendLine("  ")
            stb.AppendLine("      inner JOIN SIGPA_CodificaFertilizzanti cod ")
            stb.AppendLine("        on cod.CODICE = cac.Cod_Prodotto_Cliente  ")
            stb.AppendLine("  ")
            stb.AppendLine("  ")
            stb.AppendLine("  ")
            stb.AppendLine("     WHERE   1=1  ")
            stb.AppendLine("    AND   mm.Data_Movimento <=  " & Agro_SQL_SaveDate(DataFine) & "   ")
            stb.AppendLine("     AND     mm.Data_Movimento >=  " & Agro_SQL_SaveDate(DataInizio) & "   ")
            stb.AppendLine("     AND (  ")
            stb.AppendLine("             ")
            stb.AppendLine("            cod.Attivo <> 'SI' ")
            stb.AppendLine("        ) ")


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

    Public Function ElencaBolleNonAssociateFattura(
                    ByVal PIVA_PadreInGerarchia As String,
                    ByVal DataInizio As DateTime,
                    ByVal DataFine As DateTime,
                    ByVal ElencaSoloProdottiMovimentati As Boolean,
                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                    Optional ByVal piva As String = ""
            ) As DataTable

        Dim nomeRoutine As String = "Contabilita_Movimenti_Dettagli_Leggi"

        Dim stb As New StringBuilder
        Dim messaggioErrore As String = ""
        Dim dt As New DataTable

        '----------------------------------------------------
        '--- Preparo la Query SQL ---------------------------
        '----------------------------------------------------
        Try
            stb.Length = 0

            If piva <> "" Then
                stb.AppendLine(" select distinct ")
                stb.AppendLine("      cast(bolle.Data_Movimento as date) as Data_Movimento ")
                stb.AppendLine("    , des_lib ")
                stb.AppendLine("    , DescrizioneProdotto ")
                stb.AppendLine("  ")
            Else
                stb.AppendLine(" select pad.rag_soc as UOL, cc.val_cod as CUAA, cast(bolle.Data_Movimento as date) as data_bolla, i.rag_soc, bolle.des_lib, DescrizioneProdotto ")
            End If

            stb.AppendLine("    from  ")
            stb.AppendLine("    ( ")
            stb.AppendLine("        select distinct a.PIVA, a.Id_Agenda, des_lib, a.Username_Creazione , m2.Data_Movimento, m2.Id_Mov, m2.Cau_Mov, d.Id_Mov_Det, coalesce(ff.fr_des, coalesce(f.fer_des, '')) as DescrizioneProdotto ")
            stb.AppendLine("        from Agenda a ")

            If piva = "" Then
                stb.AppendLine("INNER JOIN GerarchieImpreseEsplosa   ")
                stb.AppendLine(" on GerarchieImpreseEsplosa.Figlio = a.PIVA   ")
                stb.AppendLine(" and GerarchieImpreseEsplosa.Padre in  ('02168620546', '01205100553')   ")
            End If

            stb.AppendLine("  ")
            stb.AppendLine("    inner join Movimenti m1 ")
            stb.AppendLine("                on a.Id_Agenda = m1.Id_Agenda ")
            stb.AppendLine("                and a.PIVA = m1.PIVA                 ")
            stb.AppendLine("                and m1.Cau_Mov in ('7300') ")
            stb.AppendLine("  ")
            stb.AppendLine("            inner join Movimenti m2 ")
            stb.AppendLine("                on a.Id_Agenda = m2.Id_Agenda ")
            stb.AppendLine("                and a.PIVA = m2.PIVA                 ")
            stb.AppendLine("                and m2.Cau_Mov in ('4000') ")
            stb.AppendLine("  ")
            stb.AppendLine("            inner join Movimenti_dettagli d ")
            stb.AppendLine("                on d.Id_Agenda = m1.Id_Agenda  ")
            stb.AppendLine("                and d.Id_Mov  = m1.Id_Mov")


            stb.AppendLine("  ")
            stb.AppendLine("            left join Fertilizzanti f  ")
            stb.AppendLine("                on f.Fer_Cod = d.Pro_Cod  ")
            stb.AppendLine("                and d.elem_Cod = 3 ")

            stb.AppendLine("            left join Formulati ff ")
            stb.AppendLine("                on ff.Fr_Cod = d.Pro_Cod")
            stb.AppendLine("                and d.elem_Cod <> 3 ")


            stb.AppendLine("        where Lav_Cod in (1025, 1031) ")
            If piva <> "" Then
                stb.AppendLine("        and a.PIVA = '" & Agro_SQL_SaveText(piva) & "'  ")
            End If

            stb.AppendLine("    ) bolle ")
            stb.AppendLine("  ")

            If piva = "" Then
                stb.AppendLine("    inner join Imprese i ")
                stb.AppendLine("        on i.PIVA = bolle.PIVA  ")
                stb.AppendLine("    inner join Imprese_Codici cc ")
                stb.AppendLine("        on i.PIVA = cc.PIVA  ")
                stb.AppendLine("        and cc.id_cod = 1010  ")
                stb.AppendLine("    inner join GerarchiaImprese gg ")
                stb.AppendLine("        on i.PIVA =gg.Figlio  ")
                stb.AppendLine("    inner join Imprese pad ")
                stb.AppendLine("        on pad.PIVA = gg.Padre ")

            End If

            stb.AppendLine("    left join Mov_Dettagli_Riferimenti  ")
            stb.AppendLine("        on bolle.Id_Agenda = Id_Agenda_Rif       ")

            '  Vanni, 25/07/2014 10:29:49: viene memorizzata una coppia in Mov_Dettagli_Riferimenti differente dalla tabella Movimenti_dettagli.. commento id_mov e recupero per movimento dettaglio..
            'stb.AppendLine("        and bolle.Id_Mov = Id_Mov_Rif  ")

            stb.AppendLine("        and bolle.Id_Mov_Det  = Id_Mov_det_Rif ")
            stb.AppendLine("  ")
            stb.AppendLine("    where Mov_Dettagli_Riferimenti.Piva_Rif is null ")
            stb.AppendLine("    order by  cast(bolle.Data_Movimento as date)  ")
            stb.AppendLine("    , des_lib ")
            stb.AppendLine("    , DescrizioneProdotto")

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

    Public Function ElencaCarichiDiMagazzinoNonGiustificatiCorrettamente_SIGPA(
                    ByVal PIVA_PadreInGerarchia As String,
                    ByVal DataInizio As DateTime,
                    ByVal DataFine As DateTime,
                    ByVal ServizioCod As Integer,
                    ByVal ElemCod As Integer,
                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                    Optional ByVal piva As String = ""
            ) As DataTable

        Dim nomeRoutine As String = "Contabilita_Movimenti_Dettagli_Leggi"

        Dim stb As New StringBuilder
        Dim messaggioErrore As String = ""
        Dim dt As New DataTable

        '----------------------------------------------------
        '--- Preparo la Query SQL ---------------------------
        '----------------------------------------------------
        Try
            stb.Length = 0

            DocumentiContabili_CarichiScarichi_EsportazioneSIGPAQry_C_S(0, PIVA_PadreInGerarchia, DataInizio, DataFine, 0, ElemCod, objParametri, stb, ServizioCod, piva, True, False)
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

    Public Function ElencaProdottiMovimentatiPriviDiDecodifica_SIGPA(ByVal PIVA_PadreInGerarchia As String,
                                                                     ByVal DataInizio As DateTime,
                                                                     ByVal DataFine As DateTime,
                                                                     ByVal ElencaSoloProdotti As Boolean,
                                                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                                     Optional ByVal piva As String = ""
                                                                     ) As DataTable

        Dim nomeRoutine As String = "Contabilita_Movimenti_Dettagli_Leggi"

        Dim stb As New StringBuilder
        Dim messaggioErrore As String = ""
        Dim dt As New DataTable

        '----------------------------------------------------
        '--- Preparo la Query SQL ---------------------------
        '----------------------------------------------------
        Try
            stb.Length = 0

            If ElencaSoloProdotti Then
                stb.AppendLine(" select distinct f.Fer_Cod, f.Fer_Des, ROUND(f.N, 2) as N, round(F.P2O5, 2) as P2O5, round(f.K2O, 2) as K2O,'' as UDM_GIAS, '' as UDM, '' as Note  ")
            Else
                stb.AppendLine(" select distinct a.des_lib, i.piva, i.rag_soc, mm.Data_Movimento, f.Fer_Cod, f.Fer_Des  ")
            End If

            stb.AppendLine(" from Agenda A   ")


            stb.AppendLine("    inner join Pratiche p ")
            stb.AppendLine("        on p.Piva = A.PIVA   ")
            stb.AppendLine("        and p.Servizio_Cod in (3, 4) ")
            stb.AppendLine("  ")
            stb.AppendLine("    inner join Pratiche_Stati_Attuali SP ")
            stb.AppendLine("        on SP.Pratica_Cod = p.Pratica_Cod  ")
            stb.AppendLine("        and SP.Stato_Cod in (2) ")


            If piva = "" Then
                stb.AppendLine("    inner join Imprese i ")
            Else
                stb.AppendLine("    inner join ( select piva, rag_soc from Imprese where piva = '" & Agro_SQL_SaveText(piva) & "' ) i ")
            End If





            stb.AppendLine("    on i.piva = a.piva ")

            stb.AppendLine("    inner join Movimenti mm ")
            stb.AppendLine("        on mm.Id_Agenda = A.Id_Agenda  ")
            stb.AppendLine("        and A.Lav_Cod in ( 14, 106, 26, 156, 123, 124, 1001, 1000, 1023, 1022)   ")
            stb.AppendLine("  ")
            stb.AppendLine("    inner join Movimenti_dettagli det ")
            stb.AppendLine("        on mm.Id_Agenda = A.Id_Agenda  ")
            stb.AppendLine("        and mm.Id_Mov = det.Id_Mov  ")
            stb.AppendLine("  ")

            If piva = "" Then
                stb.AppendLine("    INNER JOIN GerarchieImpreseEsplosa   ")
                stb.AppendLine("         on GerarchieImpreseEsplosa.Figlio = mm.PIVA   ")
                stb.AppendLine("         and GerarchieImpreseEsplosa.Padre in  (" & PIVA_PadreInGerarchia & ")  ")
                stb.AppendLine("  ")
            End If

            stb.AppendLine("    INNER JOIN Fertilizzanti F ")
            stb.AppendLine("        on F.Fer_Cod = det.Pro_Cod  ")
            stb.AppendLine("        and det.Elem_Cod = 3 ")
            stb.AppendLine("  ")
            stb.AppendLine("     LEFT JOIN CAC_Codifica_ProdottiAziendali cac ")
            stb.AppendLine("         on cac.Codice_GIAS = det.Pro_Cod   ")
            stb.AppendLine("         and cac.Elem_Cod = 3  ")
            stb.AppendLine("         and cac.Tipo_Codifica = 5  ")


            stb.AppendLine("    WHERE   mm.Data_Movimento <=  " & Agro_SQL_SaveDate(DataFine) & "   ")
            stb.AppendLine("     AND     mm.Data_Movimento >=  " & Agro_SQL_SaveDate(DataInizio) & "   ")
            stb.AppendLine("    and cac.Cod_Prodotto_Cliente is null ")
            stb.AppendLine("     AND a.Blocco_Flag = 0 ")

            stb.AppendLine("    ORDER BY F.FER_DES")


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

    Public Function DocumentiContabili_Trattamenti_EsportazioneSIGPA_ValidazioneFascicolo(ByVal id_agenda As Integer,
                                                                                          ByVal DataSucc_1_DataPrec_2 As Integer,
                                                                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                                          ) As DataTable

        Dim nomeRoutine As String = "Contabilita_Movimenti_Dettagli_Leggi"

        Dim stb As New StringBuilder
        Dim messaggioErrore As String = ""
        Dim dt As New DataTable

        '----------------------------------------------------
        '--- Preparo la Query SQL ---------------------------
        '----------------------------------------------------
        Try


            'vanni, 02/04/2014. il fascicolo viene ora ricercato andando in join sull'impianto, poi sul planning e poi sulla tabella allegati_documenti.
            ' se non viene trovato nulla in questo modo, allora si ricerca per data precedente all'operazione.


            If DataSucc_1_DataPrec_2 = 999 Then

                stb.AppendLine(" select top 1 cast(D.Allegati_Documenti_Numero as varchar(1000)) + '|' + convert(varchar(100), D.Validazione_Data, 103) as SIAN_FASCICOLO ")
                stb.AppendLine(" from ")

                stb.AppendLine(" (  ")
                stb.AppendLine(" select Piva, Sa_Cod, Appezza, Id_Destinazione, Tipo_Destinazione  ")
                stb.AppendLine(" from Mov_Destinazioni    ")
                stb.AppendLine(" where Id_Agenda = " & id_agenda)
                stb.AppendLine(" ) Mov_Dest_Concimi ")

                stb.AppendLine(" inner join Reg_Impianti_Programmazioni rp	 ")
                stb.AppendLine(" on  rp.PIVA = Mov_Dest_Concimi.Piva   ")
                stb.AppendLine(" and rp.Sa_Cod = Mov_Dest_Concimi.SA_COD  ")
                stb.AppendLine(" and rp.Appezza = Mov_Dest_Concimi.APPEZZA  ")
                stb.AppendLine(" and rp.Id_Reg = Mov_Dest_Concimi.Id_Destinazione  ")
                stb.AppendLine(" and Mov_Dest_Concimi.Tipo_Destinazione = 0 ")

                stb.AppendLine(" inner join Allegati_EntitaxDocumenti  AG ")
                stb.AppendLine(" on AG.Programmazione_Cod = RP.Programmazione_Cod  ")
                stb.AppendLine(" and AG.Programmazione_Entita_Cod = rp.Programmazione_Entita_Cod  ")

                stb.AppendLine(" inner join Allegati_Documenti D ")
                stb.AppendLine(" on AG.Allegati_Documenti_COD = D.Allegati_Documenti_Cod  ")
                stb.AppendLine(" and AG.Allegati_Documenti_SuperUser = D.Allegati_Documenti_SuperUser  ")

            Else

                Dim operando As String = ">="
                Dim ascDesc As String = "asc"
                If DataSucc_1_DataPrec_2 = 2 Then
                    operando = "<="
                    ascDesc = "desc"
                End If

                stb.Length = 0
                stb.AppendLine(" select top 1 cast(D.Allegati_Documenti_Numero as varchar(1000)) + '|' + convert(varchar(100), D.Validazione_Data, 103) as SIAN_FASCICOLO ")
                stb.AppendLine(" from  ")
                stb.AppendLine("    Movimenti m  ")
                stb.AppendLine("    inner join Allegati_Documenti D ")
                stb.AppendLine("        on m.piva  = D.Allegati_Documenti_Piva           ")
                stb.AppendLine("            and D.Validazione_Data " & operando & " m.Data_Movimento ")
                stb.AppendLine(" where m.Id_Agenda =  " & Agro_SQL_SaveNum(id_agenda))
                stb.AppendLine(" order by D.Validazione_Data " & ascDesc)

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

    Public Function DocumentiContabili_Trattamenti_EsportazioneSIGPA_VerificaPatentini(
                                                ByVal Piva As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As DataTable

        Dim nomeRoutine As String = "DocumentiContabili_Trattamenti_EsportazioneSIGPA_Terzisti"

        Dim stb As New StringBuilder
        Dim messaggioErrore As String = ""
        Dim dt As New DataTable

        '----------------------------------------------------
        '--- Preparo la Query SQL ---------------------------
        '----------------------------------------------------
        Try
            stb.Length = 0

            stb.AppendLine("select cont.Cod_Contatto as cuaa_Terzista, cont.Nome, cont.Cognome, risum.Patentino, risum.Data_Rilascio_Patentino ")
            stb.AppendLine("  ")
            stb.AppendLine(" from Contatti cont ")
            stb.AppendLine(" inner JOIN Risorse_Umane risum  ")
            stb.AppendLine("     on risum.Cod_Contatto = cont.Cod_Contatto  ")
            stb.AppendLine("     and risum.Piva = cont.Piva  ")
            stb.AppendLine("  ")
            stb.AppendLine(" where Patentino is not null ")
            stb.AppendLine(" and Patentino <> ''     ")
            stb.AppendLine("  ")
            stb.AppendLine(" and cont.piva = '" & Agro_SQL_SaveText(Piva) & "'")


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

    Public Function DocumentiContabili_Trattamenti_EsportazioneSIGPA_Terzisti(
                                                ByVal id_agenda As Integer,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As DataTable

        Dim nomeRoutine As String = "DocumentiContabili_Trattamenti_EsportazioneSIGPA_Terzisti"

        Dim stb As New StringBuilder
        Dim messaggioErrore As String = ""
        Dim dt As New DataTable

        '----------------------------------------------------
        '--- Preparo la Query SQL ---------------------------
        '----------------------------------------------------
        Try
            stb.Length = 0

            stb.AppendLine("select cont.Cod_Contatto as cuaa_Terzista, cont.Nome, cont.Cognome, risum.Patentino, risum.Data_Rilascio_Patentino, risum.Ente_di_rilascio ")
            stb.AppendLine("  ")
            stb.AppendLine(" from Movimenti Mov_Manodopera  ")
            stb.AppendLine("  ")
            stb.AppendLine(" inner JOIN Movimenti_dettagli Mov_Det_Manodopera ")
            stb.AppendLine("        ON Mov_Det_Manodopera.PIVA = Mov_Manodopera.PIVA            ")
            stb.AppendLine("        AND Mov_Det_Manodopera.Id_Agenda = Mov_Manodopera.Id_Agenda ")
            stb.AppendLine("        AND Mov_Det_Manodopera.Id_Mov  = Mov_Manodopera.Id_Mov  ")
            stb.AppendLine("        AND   Mov_Manodopera.Cau_Mov in ( '6850', '6851', '6800') ")
            stb.AppendLine("  ")
            stb.AppendLine("  ")
            stb.AppendLine(" inner JOIN Risorse_Umane risum  ")
            stb.AppendLine("     on risum.cod_risum = Mov_Det_Manodopera.Mat_Cod  ")
            stb.AppendLine("     and Mov_Det_Manodopera.Elem_Cod = 0  ")
            stb.AppendLine("  ")
            stb.AppendLine("  ")
            stb.AppendLine(" inner join Contatti cont ")
            stb.AppendLine("    on cont.Cod_Contatto = risum.Cod_Contatto  ")
            stb.AppendLine("    and cont.Piva = risum.Piva  ")
            stb.AppendLine("  ")
            stb.AppendLine(" inner join Rapporti_Contabili rc ")
            stb.AppendLine("    on risum.cod_Rapporto = rc.cod_rapporto ")
            stb.AppendLine("    and rc.terzista = 1 ")

            stb.AppendLine(" where Mov_Manodopera.Id_Agenda = " & Agro_SQL_SaveNum(id_agenda))

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

    Public Function DocumentiContabili_Trattamenti_EsportazioneSIGPA_Avversita_seNonSpecificata(
                                                ByVal id_agenda As Integer,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As DataTable

        Dim nomeRoutine As String = "DocumentiContabili_Trattamenti_EsportazioneSIGPA_Avversita"

        Dim stb As New StringBuilder
        Dim messaggioErrore As String = ""
        Dim dt As New DataTable

        '----------------------------------------------------
        '--- Preparo la Query SQL ---------------------------
        '----------------------------------------------------
        Try
            stb.Length = 0

            stb.AppendLine("select  ")
            stb.AppendLine("        o.lav_des  ")
            stb.AppendLine("  ")
            stb.AppendLine(" from Agenda a ")
            stb.AppendLine(" inner join Operazioni o  ")
            stb.AppendLine("    on o.lAv_Cod = a.lAv_Cod ")
            stb.AppendLine("  ")
            stb.AppendLine(" where a.Id_Agenda = " & id_agenda)



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

    Public Function DocumentiContabili_Trattamenti_EsportazioneSIGPA_Avversita(
                                            ByVal id_agenda As Integer,
                                            ByVal fr_cod As Integer,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable

        Dim nomeRoutine As String = "DocumentiContabili_Trattamenti_EsportazioneSIGPA_Avversita"

        Dim stb As New StringBuilder
        Dim messaggioErrore As String = ""
        Dim dt As New DataTable

        '----------------------------------------------------
        '--- Preparo la Query SQL ---------------------------
        '----------------------------------------------------
        Try
            stb.Length = 0

            stb.AppendLine("select  ")
            stb.AppendLine("        Mov_Det_Avv.Id_Agenda  ")
            stb.AppendLine("      , av.Av_Cod  ")
            stb.AppendLine("      , coalesce(av.Av_Des_Vol, '') as Av_Des_Vol ")
            stb.AppendLine("      , Mov_Det_Avv.Elem_Cod  ")
            stb.AppendLine("      , Mov_Det_Avv.Pro_Cod  ")
            stb.AppendLine("  ")
            stb.AppendLine(" from Movimenti Mov_Avv ")
            stb.AppendLine("    inner join Movimenti_dettagli Mov_Det_Avv        ")
            stb.AppendLine("        on  Mov_Avv.PIVA = Mov_Det_Avv.PIVA  ")
            stb.AppendLine("        and Mov_Avv.Id_Agenda = mov_det_avv.id_agenda ")
            stb.AppendLine("        and Mov_Avv.Id_Mov = Mov_Det_Avv.Id_Mov  ")
            stb.AppendLine("        and Mov_Avv.Cau_Mov = '2050' ")
            stb.AppendLine("  ")
            stb.AppendLine(" inner join Mov_Dettaglio_Tecnico dt ")
            stb.AppendLine("        on dt.Id_Agenda = Mov_Det_Avv.Id_Agenda  ")
            stb.AppendLine("        and dt.Id_Mov = Mov_Det_Avv.Id_Mov ")
            stb.AppendLine("        and dt.Id_Mov_Det = Mov_Det_Avv.Id_Mov_Det   ")
            stb.AppendLine("        and dt.Av_Cod <> 0       ")
            stb.AppendLine(" inner join Avversita av  ")
            stb.AppendLine("    on dt.Av_Cod = av.Av_Cod ")
            stb.AppendLine("  ")
            stb.AppendLine(" where Mov_Det_Avv.Id_Agenda = " & id_agenda)


            If fr_cod <> 0 Then
                stb.AppendLine(" and Mov_Det_Avv.elem_cod = 191 ")
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

    Public Function DocumentiContabili_Trattamenti_EsportazioneSIGPA_GruppiAvversita(
                                            ByVal id_agenda As Integer,
                                            ByVal fr_cod As Integer,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable

        Dim nomeRoutine As String = "DocumentiContabili_Trattamenti_EsportazioneSIGPA_Avversita"

        Dim stb As New StringBuilder
        Dim messaggioErrore As String = ""
        Dim dt As New DataTable

        '----------------------------------------------------
        '--- Preparo la Query SQL ---------------------------
        '----------------------------------------------------
        Try
            stb.Length = 0

            stb.AppendLine(" select   ")
            stb.AppendLine("        Mov_Det_Avv.Id_Agenda  ")
            stb.AppendLine("      , av.Av_Gru      ")
            stb.AppendLine("      , coalesce(av.av_gru_des , '') as Av_Des_Vol ")
            stb.AppendLine("      , Mov_Det_Avv.Elem_Cod  ")
            stb.AppendLine("      , Mov_Det_Avv.Pro_Cod  ")
            stb.AppendLine("  ")
            stb.AppendLine(" from Movimenti Mov_Avv ")
            stb.AppendLine("    inner join Movimenti_dettagli Mov_Det_Avv        ")
            stb.AppendLine("        on  Mov_Avv.PIVA = Mov_Det_Avv.PIVA  ")
            stb.AppendLine("        and Mov_Avv.Id_Agenda = mov_det_avv.id_agenda ")
            stb.AppendLine("        and Mov_Avv.Id_Mov = Mov_Det_Avv.Id_Mov  ")
            stb.AppendLine("        and Mov_Avv.Cau_Mov = '2050' ")
            stb.AppendLine("  ")
            stb.AppendLine(" inner join Mov_Dettaglio_Tecnico dt ")
            stb.AppendLine("        on dt.Id_Agenda = Mov_Det_Avv.Id_Agenda  ")
            stb.AppendLine("        and dt.Id_Mov = Mov_Det_Avv.Id_Mov ")
            stb.AppendLine("        and dt.Id_Mov_Det = Mov_Det_Avv.Id_Mov_Det   ")
            stb.AppendLine("        and dt.Av_Gru <> 0   ")
            stb.AppendLine("  ")
            stb.AppendLine(" inner join GruppoAvversita av  ")
            stb.AppendLine("    on dt.Av_Gru  = av.Av_Gru  ")
            stb.AppendLine("  ")
            stb.AppendLine(" where Mov_Det_Avv.Id_Agenda =   " & id_agenda)

            If fr_cod <> 0 Then
                stb.AppendLine(" and Mov_Det_Avv.elem_cod in (191, 18) ")
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

    Private Shared Function udm(ByVal elem_cod As Integer) As String

        Dim stb As New StringBuilder

        If elem_cod = 3 Then
            stb.AppendLine("    , case when udmSigpa.UDM_SIM is not null then udmSigpa.UDM_SIM else  ")
            stb.AppendLine("        case when udmExtra.UDM_COD = 0 then  ")
            stb.AppendLine("             udmSalvata.UDM_SIM collate latin1_general_CI_AS ")
            stb.AppendLine("         Else ")
            stb.AppendLine("             udmExtra.UDM_SIM  ")
            stb.AppendLine("             End ")
            stb.AppendLine("    End ")
            stb.AppendLine(" as udm_sim ")
        Else
            stb.AppendLine(" , udmSalvata.UDM_SIM  ")
        End If

        Return stb.ToString

    End Function

    Private Shared Function qta(ByVal cifreArrotondamento As Integer, ByVal elem_cod As Integer) As String
        Dim ddd As String = ""

        If elem_cod = 3 Then
            ddd = " * coalesce(convSigpa.FattoreConversione, 1) * coalesce(convSalvaToExtra.FattoreConversione, 1) "
        End If

        Return ", round(SommaMov_Dest_Concimi.somma_qta   " & ddd & " , " & cifreArrotondamento & ")  as qta"

    End Function

    Private Shared Function ProdottoXParticelleRound(ByVal cifreArrotondamento As Integer, ByVal elem_cod As Integer) As String

        Dim rval As String
        Dim ddd As String = ""

        If elem_cod = 3 Then
            ddd = " * coalesce(convSigpa.FattoreConversione, 1) * coalesce(convSalvaToExtra.FattoreConversione, 1) "
        End If

        Dim stbAreaTotale As New StringBuilder

        DocumentiContabili_Fertilizzazioni_EsportazioneSIGPA_areaTrattata(stbAreaTotale, "")

        Dim areaTotale As String = stbAreaTotale.ToString

        If cifreArrotondamento <> -1 Then
            rval = "          , round(((SommaMov_Dest_Concimi.somma_qta * " & areaTotale & ") /  SommaMov_Dest_Concimi.somma_area ), " & cifreArrotondamento & ") " & ddd & " as ProdottoXParticella   "
        Else
            rval = "          , ( (SommaMov_Dest_Concimi.somma_qta * " & areaTotale & ") / SommaMov_Dest_Concimi.somma_area  ) " & ddd & " as ProdottoXParticella  "
        End If

        Return rval

    End Function

    Public Function DocumentiContabili_NonUtilizzo_xVerifica(ByVal piva As String,
                                                             ByVal d1 As DateTime,
                                                             ByVal d2 As DateTime,
                                                             ByVal lavCod As Integer,
                                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                             ) As DataTable

        Dim nomeRoutine As String = "DocumentiContabili_Fertilizzazioni_EsportazioneSIGPA"

        Dim stb As New StringBuilder
        Dim messaggioErrore As String = ""
        Dim dt As New DataTable

        '----------------------------------------------------
        '--- Preparo la Query SQL ---------------------------
        '----------------------------------------------------
        Try
            stb.Length = 0


            stb.AppendLine("SELECT   1  ")

            stb.AppendLine("  from Agenda a ")
            stb.AppendLine("    inner join Movimenti m ")
            stb.AppendLine("        on m.PIVA = a.PIVA  ")
            stb.AppendLine("        and m.Id_Agenda = a.Id_Agenda  ")
            stb.AppendLine(" where a.piva = '" & Agro_SQL_SaveText(piva) & "'  ")

            If lavCod = LAVCOD_FERTILIZZAZIONI_DICHIARAZIONE_NON_UTILIZZO Then
                stb.AppendLine("     and   a.Lav_Cod IN ( " &
                    CStr(LAVCOD_DISTRIBUZIONE_CONCIME) & ", " &
                    CStr(LAVCOD_SARCHIATURA_CONCIMAZIONE) & ", " &
                    CStr(LAVCOD_DISTRIBUZIONE_AMMENDANTI) & ", " &
                    CStr(LAVCOD_FERTIRRIGAZIONE) & ", " &
                    CStr(LAVCOD_CONCIMAZIONE_FOGLIARE) & ", " &
                    CStr(LAVCOD_TRATTAMENTO_ANTIBUTTERATURA) &
                    " )  ")
            Else
                stb.AppendLine("     and   a.Lav_Cod IN ( " &
                    CStr(LAVCOD_TRATTAMENTO_ANTIPARASSITARIO) & ", " &
                    CStr(LAVCOD_TRATTAMENTO_FITOREGOLATORE) & ", " &
                    CStr(LAVCOD_DISERBO) & ", " &
                    CStr(LAVCOD_GEODISINFESTAZIONE) & ", " &
                    CStr(LAVCOD_CONCIA_SEME) & ", " &
                    CStr(LAVCOD_DISSECCAMENTO) &
                    " )  ")
            End If

            stb.AppendLine("  ")
            stb.AppendLine("  and m.data_movimento >= " & Agro_SQL_SaveDate(d1))
            stb.AppendLine("  and m.data_movimento <= " & Agro_SQL_SaveDate(d2))

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

    Public Function DocumentiContabili_NonUtilizzo_EsportazioneSIGPA(
                     ByVal piva As String,
                     ByVal SIGPA_ComandiEsportazione_cod As Integer,
                     ByVal PIVA_PadreInGerarchia As String,
                     ByVal DataInizio As DateTime,
                     ByVal DataFine As DateTime,
                     ByVal Esportazione_Elem_cod As Integer,
                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
             ) As DataTable

        Dim nomeRoutine As String = "DocumentiContabili_Fertilizzazioni_EsportazioneSIGPA"

        Dim stb As New StringBuilder
        Dim messaggioErrore As String = ""
        Dim dt As New DataTable

        Dim servizio_cod As Integer = 3
        If Esportazione_Elem_cod = 3 Then
            servizio_cod = 4
        End If


        '----------------------------------------------------
        '--- Preparo la Query SQL ---------------------------
        '----------------------------------------------------
        Try
            stb.Length = 0

            If piva = "" Then


                stb.AppendLine("SELECT     ")
                stb.AppendLine("     a.PIVA   ")
                stb.AppendLine("   , a.Sa_Cod    ")
                stb.AppendLine("   , a.Id_Agenda  ")
                stb.AppendLine("   , m.Mov_Desc  ")
                stb.AppendLine("   , m.Data_Movimento   ")
                stb.AppendLine("   , m.Validita_Inizio as Dichiarazione_InizioPeriodo ")
                stb.AppendLine("   , m.Validita_Fine as Dichiarazione_FinePeriodo ")
                stb.AppendLine("   , p.Pratica_Cod  ")
                stb.AppendLine("   , ic.val_cod as CUAA_Impresa")
            Else
                stb.AppendLine("SELECT   1  ")
            End If

            stb.AppendLine("  from Agenda a ")

            stb.AppendLine("    inner join Movimenti m ")
            stb.AppendLine("        on m.PIVA = a.PIVA  ")
            stb.AppendLine("        and m.Id_Agenda = a.Id_Agenda  ")

            If piva = "" Then

                stb.AppendLine(" INNER JOIN  ( select distinct figlio from GerarchieImpreseEsplosa  where Padre in  (" & PIVA_PadreInGerarchia & ")  ) es  ")
                stb.AppendLine("      on es.Figlio = a.PIVA    ")
                'stb.AppendLine("      and GerarchieImpreseEsplosa.Padre in  ('02168620546', '01205100553')  ")


                If SIGPA_ComandiEsportazione_cod > 0 Then
                    stb.AppendLine("    inner join SIGPA_ComandiEsportazione_ListaImprese LImp ")
                    stb.AppendLine("    on LImp.piva = a.Piva ")
                    stb.AppendLine("    and LImp.SIGPA_ComandiEsportazione_cod = " & Agro_SQL_SaveNum(SIGPA_ComandiEsportazione_cod))
                End If


                stb.AppendLine("    inner join Pratiche p ")
                stb.AppendLine("        on p.Piva = a.PIVA  ")
                stb.AppendLine("        and p.Servizio_Cod = " & servizio_cod)
                stb.AppendLine("  ")
                stb.AppendLine("    inner join Pratiche_Stati_Attuali at ")
                stb.AppendLine("        on at.Pratica_Cod = p.Pratica_Cod  ")
                stb.AppendLine("        and at.Stato_Cod = 2 ")



                stb.AppendLine("    inner join Imprese_Codici ic ")
                stb.AppendLine("        on ic.PIVA = a.PIVA  ")
                stb.AppendLine("        and ic.id_cod = 1010 ")

            End If
            stb.AppendLine("  ")

            If piva = "" Then
                stb.AppendLine(" where a.Blocco_Flag = 0 ")
            Else
                stb.AppendLine(" where a.piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If


            stb.AppendLine("  ")
            stb.AppendLine("  and m.data_movimento >= " & Agro_SQL_SaveDate(DataInizio))
            stb.AppendLine("  and m.data_movimento <= " & Agro_SQL_SaveDate(DataFine))

            If Esportazione_Elem_cod = 3 Then
                stb.AppendLine(" and a.Lav_Cod = " & LAVCOD_FERTILIZZAZIONI_DICHIARAZIONE_NON_UTILIZZO)
            Else
                stb.AppendLine(" and a.Lav_Cod = " & LAVCOD_TRATTAMENTO_DICHIARAZIONE_NON_UTILIZZO)
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

    Private Shared Sub DocumentiContabili_Fertilizzazioni_EsportazioneSIGPA_areaTrattata(ByRef stb As StringBuilder, ByVal convertiHA As String)
        stb.AppendLine("          case when apg.ConteggioParticelle > 1 then              ")
        stb.AppendLine("             (  ( AppezzamentiXParticelle.AREA / xAreaTrat.Qta2 ) *  xAreaTrat.Qta2 )   " & convertiHA)
        stb.AppendLine("          else ")
        stb.AppendLine("            xAreaTrat.Qta2 " & convertiHA)
        stb.AppendLine("             End ")
    End Sub

    Public Function DocumentiContabili_Fertilizzazioni_EsportazioneSIGPA(
            ByVal SIGPA_ComandiEsportazione_cod As Integer,
             ByVal PIVA_PadreInGerarchia As String,
             ByVal DataInizio As DateTime,
             ByVal DataFine As DateTime,
             ByVal baseCod As Integer,
             ByVal cifreArrotondamento As Integer,
             ByVal Esportazione_Elem_cod As Integer,
             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
     ) As DataTable

        Dim nomeRoutine As String = "DocumentiContabili_Fertilizzazioni_EsportazioneSIGPA"

        Dim stb As New StringBuilder
        Dim messaggioErrore As String = ""
        Dim dt As New DataTable

        Dim servizio_cod As Integer = 4
        If Esportazione_Elem_cod <> 3 Then
            servizio_cod = 3
        End If


        '----------------------------------------------------
        '--- Preparo la Query SQL ---------------------------
        '----------------------------------------------------
        Try
            stb.Length = 0

            stb.AppendLine(" SELECT   ")
            stb.AppendLine("            Agenda.PIVA  ")
            stb.AppendLine("          , Agenda.Sa_Cod   ")
            stb.AppendLine("          , Agenda.Id_Agenda  ")
            stb.AppendLine("          , Imprese_Codici.val_cod as CUAA_Impresa   ")
            stb.AppendLine("          , Agenda.Lav_Cod  ")
            stb.AppendLine("          , Agenda.des_lib           ")
            stb.AppendLine("          , Movimenti_Concimi.Id_Mov          ")

            stb.AppendLine("          , Movimenti_Concimi.Data_Movimento  ")

            stb.AppendLine(qta(cifreArrotondamento, Esportazione_Elem_cod))

            stb.AppendLine("          , Mov_Det_Concimi.Elem_Cod   ")

            If Esportazione_Elem_cod = 3 Then
                stb.AppendLine("          , CAC_Codifica_ProdottiAziendali.Cod_Prodotto_Cliente as Codice_Prodotto_SIGPA   ")
            Else
                stb.AppendLine("          , Mov_Det_Concimi.Pro_Cod as Codice_Prodotto_SIGPA   ")
            End If

            stb.AppendLine(udm(Esportazione_Elem_cod))

            stb.AppendLine("            , ")

            DocumentiContabili_Fertilizzazioni_EsportazioneSIGPA_areaTrattata(stb, " * 10000 ")

            stb.AppendLine("          as AreaTrattata ")

            stb.AppendLine(ProdottoXParticelleRound(cifreArrotondamento, Esportazione_Elem_cod))

            stb.AppendLine("          , cc.val_cod as VEG_COD_AGEA  ")
            stb.AppendLine("          , AppezzamentiXParticelle.PROV  ")
            stb.AppendLine("          , AppezzamentiXParticelle.COM  ")
            stb.AppendLine("          , AppezzamentiXParticelle.SEZIONE  ")
            stb.AppendLine("          , AppezzamentiXParticelle.FOGLIO  ")
            stb.AppendLine("          , AppezzamentiXParticelle.NUMERO  ")
            stb.AppendLine("          , AppezzamentiXParticelle.SUBALTERNO  ")

            stb.AppendLine("         , ppContaMagazzini.numeroMagazzini  ")
            stb.AppendLine("         , xMAgazzino.Id_Destinazione - " & baseCod & " as Id_Destinazione ")
            stb.AppendLine("         , P.Pratica_cod ")

            stb.AppendLine("          , Movimenti_Concimi.Mov_Desc as NoteMT11  ")

            stb.AppendLine("  ")
            stb.AppendLine(" FROM  ( select distinct figlio from GerarchieImpreseEsplosa  where Padre in  (" & PIVA_PadreInGerarchia & ")  ) es  ")
            stb.AppendLine("  ")
            stb.AppendLine("      INNER JOIN  Agenda    ")
            stb.AppendLine("          on es.Figlio = Agenda.PIVA")


            If SIGPA_ComandiEsportazione_cod > 0 Then
                stb.AppendLine("    inner join SIGPA_ComandiEsportazione_ListaImprese LImp ")
                stb.AppendLine("    on LImp.piva = Agenda.Piva ")
                stb.AppendLine("    and LImp.SIGPA_ComandiEsportazione_cod = " & Agro_SQL_SaveNum(SIGPA_ComandiEsportazione_cod))
            End If

            stb.AppendLine("    inner join Pratiche p ")
            stb.AppendLine("        on p.Piva = Agenda.PIVA   ")
            stb.AppendLine("        and p.Servizio_Cod =  " & servizio_cod)
            stb.AppendLine("        and p.piva_superUser = '" & objParametri.PivaSuperUser & "' ")

            stb.AppendLine("  ")
            stb.AppendLine("    inner join Pratiche_Stati_Attuali SP ")
            stb.AppendLine("        on SP.Pratica_Cod = p.Pratica_Cod  ")
            stb.AppendLine("        and SP.Stato_Cod  = 2 ")


            stb.AppendLine("     INNER JOIN Movimenti Movimenti_Concimi   ")
            stb.AppendLine("         ON Agenda.PIVA = Movimenti_Concimi.PIVA   ")
            stb.AppendLine("         AND Agenda.Sa_Cod = Movimenti_Concimi.Sa_Cod   ")
            stb.AppendLine("         AND Agenda.Id_Agenda = Movimenti_Concimi.Id_Agenda  ")

            If Esportazione_Elem_cod = 3 Then
                stb.AppendLine("     AND     Movimenti_Concimi.Cau_Mov = '" & Agro_SQL_SaveText(CAU_LAVORAZIONE) & "'   ")
            Else
                stb.AppendLine("     AND     Movimenti_Concimi.Cau_Mov = '" & Agro_SQL_SaveText(CAU_TRATTAMENTO) & "'   ")
            End If

            stb.AppendLine("  ")
            stb.AppendLine("     INNER JOIN Movimenti_dettagli Mov_Det_Concimi   ")
            stb.AppendLine("         ON Mov_Det_Concimi.PIVA = Movimenti_Concimi.PIVA   ")
            stb.AppendLine("         AND Mov_Det_Concimi.Sa_Cod = Movimenti_Concimi.Sa_Cod   ")
            stb.AppendLine("         AND Mov_Det_Concimi.Id_Agenda = Movimenti_Concimi.Id_Agenda   ")
            stb.AppendLine("         AND Mov_Det_Concimi.Id_Mov = Movimenti_Concimi.Id_Mov   ")
            stb.AppendLine("  ")
            stb.AppendLine("     INNER JOIN Mov_Destinazioni Mov_Dest_Concimi   ")
            stb.AppendLine("         ON Mov_Dest_Concimi.PIVA = Mov_Det_Concimi.PIVA   ")
            stb.AppendLine("         AND Mov_Dest_Concimi.Sa_Cod = Mov_Det_Concimi.Sa_Cod   ")
            stb.AppendLine("         AND Mov_Dest_Concimi.Id_Agenda = Mov_Det_Concimi.Id_Agenda   ")
            stb.AppendLine("         AND Mov_Dest_Concimi.Id_Mov = Mov_Det_Concimi.Id_Mov   ")
            stb.AppendLine("         AND Mov_Dest_Concimi.Id_Mov_Det = Mov_Det_Concimi.Id_Mov_Det   ")
            stb.AppendLine("  ")
            stb.AppendLine("     INNER JOIN Appezzamento    ")
            stb.AppendLine("         ON Mov_Dest_Concimi.PIVA = Appezzamento.PIVA   ")
            stb.AppendLine("         AND Mov_Dest_Concimi.Sa_Cod = Appezzamento.Sa_Cod   ")
            stb.AppendLine("         AND Mov_Dest_Concimi.Appezza = Appezzamento.Appezza   ")
            stb.AppendLine("  ")
            stb.AppendLine("     INNER JOIN AppezzamentiXParticelle    ")
            stb.AppendLine("         ON Appezzamento.PIVA = AppezzamentiXParticelle.PIVA   ")
            stb.AppendLine("         AND Appezzamento.Sa_Cod = AppezzamentiXParticelle.Sa_Cod   ")
            stb.AppendLine("         AND Appezzamento.Appezza = AppezzamentiXParticelle.Appezza   ")
            stb.AppendLine("  ")

            stb.AppendLine(" inner join ( ")
            stb.AppendLine("        select piva, sa_cod, appezza, count(*)  as ConteggioParticelle ")
            stb.AppendLine("        from AppezzamentiXParticelle ")
            stb.AppendLine("        group by piva, sa_cod, appezza ")
            stb.AppendLine("    ) apg ")
            stb.AppendLine("        on   Appezzamento.PIVA = apg.PIVA    ")
            stb.AppendLine("          AND Appezzamento.Sa_Cod = apg.Sa_Cod    ")
            stb.AppendLine("          AND Appezzamento.Appezza = apg.Appezza ")
            stb.AppendLine("  ")

            stb.AppendLine("    INNER JOIN Mov_Destinazioni xAreaTrat    ")
            stb.AppendLine("          ON xAreaTrat.PIVA = Mov_Det_Concimi.PIVA    ")
            stb.AppendLine("          AND xAreaTrat.Sa_Cod = Mov_Det_Concimi.Sa_Cod    ")
            stb.AppendLine("          AND xAreaTrat.Id_Agenda = Mov_Det_Concimi.Id_Agenda    ")
            stb.AppendLine("          AND xAreaTrat.Id_Mov = Mov_Det_Concimi.Id_Mov    ")
            stb.AppendLine("          AND xAreaTrat.Id_Mov_Det = Mov_Det_Concimi.Id_Mov_Det ")
            stb.AppendLine("          AND xAreaTrat.PIVA = AppezzamentiXParticelle.PIVA    ")
            stb.AppendLine("          AND xAreaTrat.Sa_Cod = AppezzamentiXParticelle.Sa_Cod    ")
            stb.AppendLine("          AND xAreaTrat.Appezza = AppezzamentiXParticelle.Appezza    ")
            stb.AppendLine("  ")
            stb.AppendLine("          and xAreaTrat.tipo_destinazione = 0 ")


            stb.AppendLine("  ")
            stb.AppendLine("    INNER JOIN ( ")
            stb.AppendLine("        Select Piva, COUNT (*) as numeroMagazzini ")
            stb.AppendLine("        from Fabbricati  ")
            stb.AppendLine("        group by piva ")
            stb.AppendLine("    ) ppContaMagazzini  ")
            stb.AppendLine("        on ppContaMagazzini.PIVA = Agenda.PIVA  ")
            stb.AppendLine("  ")

            stb.AppendLine("            inner join ( ")
            stb.AppendLine("      select Mov_Det_Mag.Id_Agenda, Mov_Det_Mag.Pro_Cod, Mov_Det_Mag.Udm_Cod, MovimentiDest_MAG.Id_Destinazione, UnitaMisura.UDM_sim, udmExtra.UDM_COD as udmExtra  ")
            stb.AppendLine("        from Movimenti  Movimenti_Mag             ")
            stb.AppendLine("  ")
            stb.AppendLine("      INNER JOIN Movimenti_dettagli Mov_Det_Mag    ")
            stb.AppendLine("          ON Mov_Det_Mag.PIVA = Movimenti_Mag.PIVA    ")
            stb.AppendLine("          AND Mov_Det_Mag.Id_Agenda = Movimenti_Mag.Id_Agenda    ")
            stb.AppendLine("          AND Mov_Det_Mag.Id_Mov = Movimenti_Mag.Id_Mov ")
            stb.AppendLine("         AND Movimenti_Mag.Cau_Mov = '" & CStr(CAU_SCARICO) & "' ")

            stb.AppendLine("  ")
            stb.AppendLine("     INNER JOIN Mov_Destinazioni MovimentiDest_MAG  ")
            stb.AppendLine("         ON MovimentiDest_MAG.Piva = Mov_Det_Mag.PIVA   ")
            stb.AppendLine("         and MovimentiDest_MAG.Sa_Cod = Mov_Det_Mag.Sa_Cod   ")
            stb.AppendLine("         and MovimentiDest_MAG.Id_Agenda = Mov_Det_Mag.Id_Agenda   ")
            stb.AppendLine("         and MovimentiDest_MAG.Id_Mov = Mov_Det_Mag.Id_Mov   ")
            stb.AppendLine("         and MovimentiDest_MAG.Id_Mov_Det = Mov_Det_Mag.Id_Mov_Det   ")
            stb.AppendLine("         and MovimentiDest_MAG.Tipo_Destinazione = 20  ")
            stb.AppendLine("  ")
            stb.AppendLine("      INNER JOIN UnitaMisura    ")
            stb.AppendLine("            ON Mov_Det_Mag.Udm_Cod = UnitaMisura.Udm_Cod  ")


            stb.AppendLine("      inner join UnitaMisura udmExtra ")
            stb.AppendLine("            on   Mov_Det_Mag.Extra_Int = udmExtra.UDM_COD  ")


            stb.AppendLine("  ")
            stb.AppendLine("      ) xMAgazzino ")
            stb.AppendLine("  ")
            stb.AppendLine("       on xMAgazzino.Id_Agenda = Agenda.Id_Agenda  ")
            stb.AppendLine("       and xMAgazzino.Pro_Cod = Mov_Det_Concimi.Pro_Cod  ")
            stb.AppendLine("       and xMAgazzino.Udm_Cod = Mov_Det_Concimi.Udm_Cod  ")

            stb.AppendLine("    inner join UnitaMisura udmSalvata ")
            stb.AppendLine("        on udmSalvata.UDM_COD = xMAgazzino.Udm_Cod ")



            If Esportazione_Elem_cod = 3 Then
                stb.AppendLine("     INNER JOIN CAC_Codifica_ProdottiAziendali   ")
                stb.AppendLine("         on CAC_Codifica_ProdottiAziendali.Codice_GIAS = Mov_Det_Concimi.Pro_Cod   ")


                stb.AppendLine("        and CAC_Codifica_ProdottiAziendali.Elem_Cod = 3 ")
                stb.AppendLine("         and CAC_Codifica_ProdottiAziendali.Tipo_Codifica = " & enum_Tipo_CAC_Codifica_ProdottiAziendali.Coldiretti_RegioneUmbria)



                stb.AppendLine("    left JOIN SIGPA_CodificaFertilizzanti s ")

                stb.AppendLine("        on s.CODICE = CAC_Codifica_ProdottiAziendali.Cod_Prodotto_Cliente    ")
                stb.AppendLine("  ")
                stb.AppendLine("    LEFT JOIN UnitaMisura udmExtra ")
                stb.AppendLine("        on udmExtra.UDM_COD = Mov_Det_Concimi.Extra_Int   ")
                stb.AppendLine("  ")
                stb.AppendLine("    LEFT JOIN UnitaMisura udmSigpa  ")
                stb.AppendLine("        on udmSigpa.UDM_SIM = s.UNITA_Misura collate Latin1_General_CI_AS ")
                stb.AppendLine("  ")
                stb.AppendLine("    LEFT JOIN UnitaMisura_Conversione convSalvaToExtra ")
                stb.AppendLine("        on convSalvaToExtra.UDM_COD_Da = xMAgazzino.udm_cod  ")
                stb.AppendLine("        and convSalvaToExtra.UDM_COD_A = Mov_Det_Concimi.Extra_Int   ")
                stb.AppendLine("  ")
                stb.AppendLine("    LEFT JOIN UnitaMisura_Conversione convSigpa ")
                stb.AppendLine("        on convSigpa.UDM_COD_Da = Mov_Det_Concimi.Extra_Int  ")
                stb.AppendLine("        and convSigpa.UDM_COD_A = udmSigpa.Udm_Cod  ")

            End If



            stb.AppendLine(" INNER JOIN Reg_Impianti imp ")
            stb.AppendLine("         ON imp.PIVA = Mov_Dest_Concimi.PIVA   ")
            stb.AppendLine("         AND imp.Sa_Cod = Mov_Dest_Concimi.Sa_Cod   ")
            stb.AppendLine("         AND imp.APPEZZA  = Mov_Dest_Concimi.Appezza  ")
            stb.AppendLine("         and imp.ID_REG = Mov_Dest_Concimi.Id_Destinazione  ")
            stb.AppendLine("  ")

            stb.AppendLine("    INNER JOIN Reg_Impianti_Codici cc ")
            stb.AppendLine("        on cc.PIVA = imp.PIVA  ")
            stb.AppendLine("        and cc.sa_cod = imp.SA_COD  ")
            stb.AppendLine("        and cc.appezza = imp.APPEZZA  ")
            stb.AppendLine("        and cc.Id_Reg = imp.ID_REG  ")
            stb.AppendLine("        and cc.id_cod = 1133  ")



            stb.AppendLine("    INNER JOIN (   ")
            stb.AppendLine("       select d.Piva,d.Sa_Cod,Id_Agenda, Id_Mov, Id_Mov_Det, Tipo_Destinazione, cc.val_cod, sum(qta) as somma_qta, sum (qta2) as somma_area  ")
            stb.AppendLine("       from Mov_Destinazioni  d ")
            stb.AppendLine("        inner join Reg_Impianti imp ")
            stb.AppendLine("  ")
            stb.AppendLine("          ON imp.PIVA = d.PIVA    ")
            stb.AppendLine("          AND imp.Sa_Cod = d.Sa_Cod    ")
            stb.AppendLine("          AND imp.APPEZZA  = d.Appezza   ")
            stb.AppendLine("          and imp.ID_REG = d.Id_Destinazione ")
            stb.AppendLine("          inner join Reg_Impianti_Codici cc ")
            stb.AppendLine("            on cc.PIVA = imp.PIVA  ")
            stb.AppendLine("            and cc.sa_cod = imp.SA_COD  ")
            stb.AppendLine("            and cc.appezza = imp.APPEZZA  ")
            stb.AppendLine("            and cc.Id_Reg = imp.ID_REG  ")
            stb.AppendLine("            and cc.id_cod = 1133 ")
            stb.AppendLine("  ")
            stb.AppendLine("       group by d.Piva,d.Sa_Cod,Id_Agenda, Id_Mov, Id_Mov_Det, Tipo_Destinazione, cc.val_cod    ")
            stb.AppendLine("       ) SommaMov_Dest_Concimi  ")
            stb.AppendLine("         ON SommaMov_Dest_Concimi.PIVA = Mov_Det_Concimi.PIVA     ")
            stb.AppendLine("           AND SommaMov_Dest_Concimi.Sa_Cod = Mov_Det_Concimi.Sa_Cod     ")
            stb.AppendLine("           AND SommaMov_Dest_Concimi.Id_Agenda = Mov_Det_Concimi.Id_Agenda     ")
            stb.AppendLine("           AND SommaMov_Dest_Concimi.Id_Mov = Mov_Det_Concimi.Id_Mov     ")
            stb.AppendLine("           AND SommaMov_Dest_Concimi.Id_Mov_Det = Mov_Det_Concimi.Id_Mov_Det  ")
            stb.AppendLine("           and SommaMov_Dest_Concimi.val_cod  = cc.val_cod ")


            stb.AppendLine("     INNER JOIN Imprese_Codici   ")
            stb.AppendLine("         on Imprese_Codici.PIVA = Agenda.PIVA   ")
            stb.AppendLine("         and Imprese_Codici.id_cod = 1010  ")
            stb.AppendLine("  ")

            stb.AppendLine("    WHERE   Movimenti_Concimi.Data_Movimento <=  " & Agro_SQL_SaveDate(DataFine) & "   ")
            stb.AppendLine("     AND     Movimenti_Concimi.Data_Movimento >=  " & Agro_SQL_SaveDate(DataInizio) & "   ")

            If Esportazione_Elem_cod = FERTILIZZANTI Then
                stb.AppendLine("     AND   Agenda.Lav_Cod IN ( " &
                                                    CStr(LAVCOD_DISTRIBUZIONE_CONCIME) & ", " &
                                                    CStr(LAVCOD_SARCHIATURA_CONCIMAZIONE) & ", " &
                                                    CStr(LAVCOD_DISTRIBUZIONE_AMMENDANTI) & ", " &
                                                    CStr(LAVCOD_FERTIRRIGAZIONE) & ", " &
                                                    CStr(LAVCOD_CONCIMAZIONE_FOGLIARE) & ", " &
                                                    CStr(LAVCOD_TRATTAMENTO_ANTIBUTTERATURA) &
                                                    " )  ")
            Else
                stb.AppendLine("     AND   Agenda.Lav_Cod IN ( " &
                                                    CStr(LAVCOD_TRATTAMENTO_ANTIPARASSITARIO) & ", " &
                                                    CStr(LAVCOD_TRATTAMENTO_FITOREGOLATORE) & ", " &
                                                    CStr(LAVCOD_DISERBO) & ", " &
                                                    CStr(LAVCOD_GEODISINFESTAZIONE) & ", " &
                                                    CStr(LAVCOD_CONCIA_SEME) & ", " &
                                                    CStr(LAVCOD_DISSECCAMENTO) &
                                                    " )  ")
            End If


            stb.AppendLine("     AND Agenda.Blocco_Flag = 0 ")


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

    Private Sub DocumentiContabili_CarichiScarichi_EsportazioneSIGPAQry_C_S(
                ByVal SIGPA_ComandiEsportazione_cod As Integer,
                ByVal PIVA_PadreInGerarchia As String,
                ByVal DataInizio As DateTime,
                ByVal DataFine As DateTime,
                ByVal BaseCod As Integer,
                ByVal Esportazione_Elem_Cod As Integer,
                ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                ByRef stb As StringBuilder,
                ByVal servizio_cod As Integer,
                ByVal piva As String,
                ByVal SoloCarichi As Boolean,
                ByVal soloOperazioniNonBoccate As Boolean
        )


        stb.AppendLine(" SELECT   ")
        stb.AppendLine("         Agenda.PIVA  ")
        stb.AppendLine("          , Agenda.Sa_Cod   ")
        stb.AppendLine("          , Agenda.Id_Agenda  ")
        stb.AppendLine("          , Imprese_Codici.val_cod as CUAA_Impresa   ")
        stb.AppendLine("          , Agenda.Lav_Cod  ")
        stb.AppendLine("          , Agenda.des_lib  ")
        stb.AppendLine("          , 0 as Cod_RisUm  ")
        stb.AppendLine("          , Movimenti_Mag.Data_Movimento ")
        stb.AppendLine("          , '' as Doc_Numero_Sin  ")
        stb.AppendLine("          , 0  as Doc_Numero  ")
        stb.AppendLine("          , '' as Doc_Numero_Des          ")
        stb.AppendLine("          , '' as CUAA_Cliente_Fornitore  ")
        stb.AppendLine("          , '' AS Contatto  ")

        If Esportazione_Elem_Cod = 3 Then
            stb.AppendLine("          , Movimenti_dettagli.Qta * coalesce(convSigpa.FattoreConversione, 1) * coalesce(convSalvaToExtra.FattoreConversione, 1) as Qta  ")
        Else
            stb.AppendLine("          , Movimenti_dettagli.Qta  ")
        End If

        stb.AppendLine("          , Movimenti_dettagli.Elem_Cod  ")
        stb.AppendLine("          , Movimenti_dettagli.id_mov  ")
        stb.AppendLine("          , Movimenti_dettagli.id_mov_det  ")


        'codice prodotto e unità misura
        If Esportazione_Elem_Cod = 3 Then
            stb.AppendLine("      , CAC_Codifica_ProdottiAziendali.Cod_Prodotto_Cliente as Codice_Prodotto_SIGPA  ")
            stb.AppendLine("                , case when s.UNITA_Misura is not null then s.UNITA_Misura else  ")
            stb.AppendLine("                case when udmExtra.UDM_COD = 0 then  ")
            stb.AppendLine("                    udmSalvata.UDM_SIM collate latin1_general_CI_AS ")
            stb.AppendLine("            Else ")
            stb.AppendLine("                 udmExtra.UDM_SIM ")
            stb.AppendLine("                 End ")
            stb.AppendLine("            End ")
            stb.AppendLine("           as udm_sim ")

        Else
            stb.AppendLine("      , Movimenti_dettagli.Pro_Cod as Codice_Prodotto_SIGPA  ")
            stb.AppendLine("          , udm_sim  ")
        End If

        stb.AppendLine("          , '' as NumeroBolla ")
        stb.AppendLine("          , GETDATE() as databolla ")
        stb.AppendLine("          , Movimenti_dettagli.Extra_Str as NumeroDenuncia ")
        stb.AppendLine("          , Movimenti_dettagli.Extra_Date as DataDenuncia ")
        stb.AppendLine("          , ppContaMagazzini.numeroMagazzini  ")
        stb.AppendLine("          , MovimentiDest_MAG.Id_Destinazione - " & BaseCod & "  as Id_Destinazione ")
        stb.AppendLine("          , Movimenti_dettagli.Pendente as causaleGIAS ")

        If piva = "" Then
            stb.AppendLine("          ,P.Pratica_COD ")
        End If


        stb.AppendLine("  ")

        If PIVA_PadreInGerarchia <> "" Then
            stb.AppendLine(" FROM  ( select distinct figlio from GerarchieImpreseEsplosa  where Padre in  (" & PIVA_PadreInGerarchia & ")  ) es  ")
            stb.AppendLine("  ")
            stb.AppendLine("      INNER JOIN  Agenda    ")
            stb.AppendLine("          on es.Figlio = Agenda.PIVA")
        Else
            stb.AppendLine(" FROM Agenda    ")
        End If


        If SIGPA_ComandiEsportazione_cod > 0 Then
            stb.AppendLine("    inner join SIGPA_ComandiEsportazione_ListaImprese LImp ")
            stb.AppendLine("    on LImp.piva = Agenda.Piva ")
            stb.AppendLine("    and LImp.SIGPA_ComandiEsportazione_cod = " & Agro_SQL_SaveNum(SIGPA_ComandiEsportazione_cod))
        End If

        If piva = "" Then
            stb.AppendLine("    inner join Pratiche p ")
            stb.AppendLine("        on p.Piva = Agenda.PIVA   ")
            stb.AppendLine("        and p.Servizio_Cod = " & servizio_cod)
            stb.AppendLine("        and p.piva_SuperUser = '" & objParametri.PivaSuperUser & "' ")

            stb.AppendLine("  ")
            stb.AppendLine("    inner join Pratiche_Stati_Attuali SP ")
            stb.AppendLine("        on SP.Pratica_Cod = p.Pratica_Cod  ")
            stb.AppendLine("        and SP.Stato_Cod in (2) ")
        End If

        stb.AppendLine("    INNER JOIN Movimenti Movimenti_Mag  ")
        stb.AppendLine("        ON Agenda.PIVA = Movimenti_Mag.PIVA  ")
        stb.AppendLine("        AND Agenda.Sa_Cod = Movimenti_Mag.Sa_Cod  ")
        stb.AppendLine("        AND Agenda.Id_Agenda = Movimenti_Mag.Id_Agenda ")
        stb.AppendLine("  ")
        stb.AppendLine("  ")
        stb.AppendLine("    INNER JOIN ( ")
        stb.AppendLine("        Select Piva, COUNT (*) as numeroMagazzini ")
        stb.AppendLine("        from Fabbricati  ")
        stb.AppendLine("        group by piva ")
        stb.AppendLine("    ) ppContaMagazzini  ")
        stb.AppendLine("        on ppContaMagazzini.PIVA = Agenda.PIVA  ")
        stb.AppendLine("    INNER JOIN Movimenti_dettagli  ")
        stb.AppendLine("        ON Movimenti_dettagli.PIVA = Movimenti_Mag.PIVA  ")
        stb.AppendLine("        AND Movimenti_dettagli.Id_Agenda = Movimenti_Mag.Id_Agenda  ")
        stb.AppendLine("        AND Movimenti_dettagli.Id_Mov = Movimenti_Mag.Id_Mov  ")
        '  Vanni, 25/06/2013 09:53:35: x formulati
        stb.AppendLine("        AND Movimenti_dettagli.Elem_Cod = " & Esportazione_Elem_Cod)

        '  Vanni, 19/06/2015 17:00:27: Sostituito il join di mov_destinazioni: dalla movimenti alla movimenti dettagli, sostituita tabella "Movimenti_Mag" con "Movimenti_dettagli"
        stb.AppendLine("    INNER JOIN Mov_Destinazioni MovimentiDest_MAG ")
        stb.AppendLine("        ON MovimentiDest_MAG.Piva = Movimenti_dettagli.PIVA  ")
        stb.AppendLine("        and MovimentiDest_MAG.Id_Agenda = Movimenti_dettagli.Id_Agenda  ")
        stb.AppendLine("        and MovimentiDest_MAG.Id_Mov = Movimenti_dettagli.Id_Mov  ")
        stb.AppendLine("        and MovimentiDest_MAG.Id_Mov_det = Movimenti_dettagli.Id_Mov_det  ") 'aggiunta condizione su id_mov_Det
        stb.AppendLine("        and MovimentiDest_MAG.Tipo_Destinazione = 20 ")

        stb.AppendLine("    INNER JOIN UnitaMisura  ")
        stb.AppendLine("        ON Movimenti_dettagli.Udm_Cod = UnitaMisura.Udm_Cod ")
        stb.AppendLine("  ")





        If Esportazione_Elem_Cod = 3 Then

            stb.AppendLine("    INNER JOIN CAC_Codifica_ProdottiAziendali  ")
            stb.AppendLine("        on CAC_Codifica_ProdottiAziendali.Codice_GIAS = Movimenti_dettagli.Pro_Cod  ")

            stb.AppendLine("        and CAC_Codifica_ProdottiAziendali.Elem_Cod = 3 ")
            stb.AppendLine("         and CAC_Codifica_ProdottiAziendali.Tipo_Codifica = " & enum_Tipo_CAC_Codifica_ProdottiAziendali.Coldiretti_RegioneUmbria)


            stb.AppendLine("    inner join UnitaMisura udmSalvata ")
            stb.AppendLine("        on udmSalvata.UDM_COD = Movimenti_dettagli.Udm_Cod ")
            stb.AppendLine("  ")


            stb.AppendLine("    left JOIN SIGPA_CodificaFertilizzanti s ")

            stb.AppendLine("        on s.CODICE = CAC_Codifica_ProdottiAziendali.Cod_Prodotto_Cliente    ")
            stb.AppendLine("  ")
            stb.AppendLine("  ")
            stb.AppendLine("    left JOIN UnitaMisura udmExtra ")
            stb.AppendLine("        on udmExtra.UDM_COD = Movimenti_dettagli.Extra_Int   ")
            stb.AppendLine("  ")
            stb.AppendLine("    left JOIN UnitaMisura udmSigpa  ")
            stb.AppendLine("        on udmSigpa.UDM_SIM = s.UNITA_Misura collate Latin1_General_CI_AS ")
            stb.AppendLine("  ")
            stb.AppendLine("    left join UnitaMisura_Conversione convSalvaToExtra ")
            stb.AppendLine("        on convSalvaToExtra.UDM_COD_Da = Movimenti_dettagli.Udm_Cod   ")
            stb.AppendLine("        and convSalvaToExtra.UDM_COD_A = case when  Movimenti_dettagli.Extra_Int = 0 then Movimenti_dettagli.Udm_Cod else Movimenti_dettagli.Extra_Int end ")
            stb.AppendLine("  ")
            stb.AppendLine("    left JOIN UnitaMisura_Conversione convSigpa ")
            stb.AppendLine("        on convSigpa.UDM_COD_Da = case when  Movimenti_dettagli.Extra_Int = 0 then Movimenti_dettagli.Udm_Cod else Movimenti_dettagli.Extra_Int end ")
            stb.AppendLine("        and convSigpa.UDM_COD_A = udmSigpa.Udm_Cod ")


        End If


        stb.AppendLine("  ")
        stb.AppendLine("    INNER JOIN Imprese_Codici  ")
        stb.AppendLine("        on Imprese_Codici.PIVA = Agenda.PIVA  ")
        stb.AppendLine("        and Imprese_Codici.id_cod = 1010 ")
        stb.AppendLine("  ")
        stb.AppendLine("    WHERE   Movimenti_Mag.Data_Movimento <=  " & Agro_SQL_SaveDate(DataFine) & "   ")
        stb.AppendLine("     AND     Movimenti_Mag.Data_Movimento >=  " & Agro_SQL_SaveDate(DataInizio) & "   ")
        stb.AppendLine("    AND     Movimenti_Mag.Cau_Mov in ('")
        If Not SoloCarichi Then
            stb.AppendLine(Agro_SQL_SaveText(CAU_SCARICO) & "', '")
        End If

        stb.AppendLine(Agro_SQL_SaveText(CAU_CARICO) & "')   ")

        stb.AppendLine("     AND   Agenda.Lav_Cod IN ( ")
        If Not SoloCarichi Then
            stb.AppendLine(CStr(LAVCOD_SCARICO) & ", ")
        End If

        stb.AppendLine(CStr(LAVCOD_CARICO))
        stb.AppendLine(" ) ")

        If piva <> "" Then

            stb.AppendLine("     AND Agenda.piva = '" & Agro_SQL_SaveText(piva) & "' ")
        End If

        If soloOperazioniNonBoccate Then
            stb.AppendLine("     AND Movimenti_dettagli.Dettagli_Blocco_Flag = 0 ")
        End If



    End Sub

    Public Function DocumentiContabili_CarichiScarichi_EsportazioneSIGPA(ByVal SIGPA_ComandiEsportazione_cod As Integer,
                                                                         ByVal PIVA_PadreInGerarchia As String,
                                                                         ByVal DataInizio As DateTime,
                                                                         ByVal DataFine As DateTime,
                                                                         ByVal BaseCod As Integer,
                                                                         ByVal Esportazione_Elem_Cod As Integer,
                                                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                         ) As DataTable

        Dim nomeRoutine As String = "DocumentiContabili_CarichiScarichi_EsportazioneSIGPA"

        Dim stb As New StringBuilder
        Dim messaggioErrore As String = ""
        Dim dt As New DataTable


        Dim servizio_cod As Integer = 4
        If Esportazione_Elem_Cod <> 3 Then
            servizio_cod = 3
        End If

        '----------------------------------------------------
        '--- Preparo la Query SQL ---------------------------
        '----------------------------------------------------
        Try
            stb.Length = 0

            '/************************************************************************************
            '/***** 1° PARTE: FATTURE IMMEDIATE  ***************
            '/************************************************************************************

            stb.AppendLine(" SELECT   ")
            stb.AppendLine("         Agenda.PIVA  ")
            stb.AppendLine("          , Agenda.Sa_Cod   ")
            stb.AppendLine("          , Agenda.Id_Agenda  ")
            stb.AppendLine("          , Imprese_Codici.val_cod as CUAA_Impresa   ")
            stb.AppendLine("          , Agenda.Lav_Cod  ")
            stb.AppendLine("          , Agenda.des_lib  ")
            stb.AppendLine("          , Movimenti_Contab.Cod_RisUm  ")
            stb.AppendLine("          , Movimenti_Contab.Data_Movimento  ")
            stb.AppendLine("          , Movimenti_Contab.Doc_Numero_Sin  ")
            stb.AppendLine("          , Movimenti_Contab.Doc_Numero  ")
            stb.AppendLine("          , Movimenti_Contab.Doc_Numero_Des          ")
            stb.AppendLine("          , case when Contatti_Contab.Codice_Fiscale = '' then Contatti_Contab.Cod_Contatto else Contatti_Contab.Codice_Fiscale end as CUAA_Cliente_Fornitore  ")
            stb.AppendLine("          , (Contatti_Contab.Rag_Soc + Contatti_Contab.Nome +' ' + Contatti_Contab.cognome ) AS Contatto  ")
            If Esportazione_Elem_Cod = 3 Then
                stb.AppendLine("          , Movimenti_dettagli.Qta * coalesce(convSigpa.FattoreConversione, 1) * coalesce(convSalvaToExtra.FattoreConversione, 1) as Qta  ")
            Else
                stb.AppendLine("          , Movimenti_dettagli.Qta  ")
            End If

            stb.AppendLine("          , Movimenti_dettagli.Elem_Cod  ")
            stb.AppendLine("          , Movimenti_dettagli.id_mov  ")
            stb.AppendLine("          , Movimenti_dettagli.id_mov_det  ")

            'codice prodotto e unità misura
            If Esportazione_Elem_Cod = 3 Then
                stb.AppendLine("      , CAC_Codifica_ProdottiAziendali.Cod_Prodotto_Cliente as Codice_Prodotto_SIGPA  ")
                stb.AppendLine("                , case when s.UNITA_Misura is not null then s.UNITA_Misura else  ")
                stb.AppendLine("                case when udmExtra.UDM_COD = 0 then  ")
                stb.AppendLine("                    udmSalvata.UDM_SIM collate latin1_general_CI_AS ")
                stb.AppendLine("            Else ")
                stb.AppendLine("                 udmExtra.UDM_SIM ")
                stb.AppendLine("                 End ")
                stb.AppendLine("            End ")
                stb.AppendLine("           as udm_sim ")

            Else
                stb.AppendLine("      , Movimenti_dettagli.Pro_Cod as Codice_Prodotto_SIGPA  ")
                stb.AppendLine("          , udm_sim  ")
            End If




            stb.AppendLine("          , '' as NumeroBolla ")
            stb.AppendLine("          , GETDATE() as DataBolla ")
            stb.AppendLine("          , '' as DenunciaNumero ")
            stb.AppendLine("          , GETDATE() as DataDenuncia ")
            stb.AppendLine("          , ppContaMagazzini.numeroMagazzini  ")
            stb.AppendLine("          , MovimentiDest_MAG.Id_Destinazione - " & BaseCod & "  as Id_Destinazione ")
            stb.AppendLine("          , Movimenti_dettagli.Pendente as causaleGIAS ")
            stb.AppendLine("          ,P.Pratica_COD ")

            stb.AppendLine("  ")


            stb.AppendLine("  ")

            stb.AppendLine(" FROM  ( select distinct figlio from GerarchieImpreseEsplosa  where Padre in  (" & PIVA_PadreInGerarchia & ")  ) es  ")
            stb.AppendLine("  ")
            stb.AppendLine("      INNER JOIN  Agenda    ")
            stb.AppendLine("          on es.Figlio = Agenda.PIVA")

            '  Vanni, 07/02/2014 10:33:39: filtro per una o più imprese
            If SIGPA_ComandiEsportazione_cod > 0 Then
                stb.AppendLine("    inner join SIGPA_ComandiEsportazione_ListaImprese LImp ")
                stb.AppendLine("    on LImp.piva = Agenda.Piva ")
                stb.AppendLine("    and LImp.SIGPA_ComandiEsportazione_cod = " & Agro_SQL_SaveNum(SIGPA_ComandiEsportazione_cod))
            End If

            stb.AppendLine("    inner join Pratiche p ")
            stb.AppendLine("        on p.Piva = Agenda.PIVA   ")
            stb.AppendLine("        and p.Servizio_Cod =   " & servizio_cod)
            stb.AppendLine("        and p.piva_SuperUser = '" & objParametri.PivaSuperUser & "' ")

            stb.AppendLine("  ")
            stb.AppendLine("    inner join Pratiche_Stati_Attuali SP ")
            stb.AppendLine("        on SP.Pratica_Cod = p.Pratica_Cod  ")
            stb.AppendLine("        and SP.Stato_Cod in (2) ")


            stb.AppendLine("    INNER JOIN Movimenti Movimenti_Contab  ")
            stb.AppendLine("        ON Agenda.PIVA = Movimenti_Contab.PIVA  ")
            stb.AppendLine("        AND Agenda.Sa_Cod = Movimenti_Contab.Sa_Cod  ")
            stb.AppendLine("        AND Agenda.Id_Agenda = Movimenti_Contab.Id_Agenda ")
            stb.AppendLine("  ")
            stb.AppendLine("    INNER JOIN ( ")
            stb.AppendLine("        Select Piva, COUNT (*) as numeroMagazzini ")
            stb.AppendLine("        from Fabbricati  ")
            stb.AppendLine("        group by piva ")
            stb.AppendLine("    ) ppContaMagazzini  ")
            stb.AppendLine("        on ppContaMagazzini.PIVA = Agenda.PIVA  ")
            stb.AppendLine("    INNER JOIN Risorse_Umane RisUm_Contab  ")
            stb.AppendLine("        ON RisUm_Contab.Cod_RisUm = Movimenti_Contab.Cod_RisUm  ")
            stb.AppendLine("  ")
            stb.AppendLine("    INNER JOIN Contatti Contatti_Contab  ")
            stb.AppendLine("        ON RisUm_Contab.Piva = Contatti_Contab.Piva  ")
            stb.AppendLine("        AND RisUm_Contab.Cod_Contatto = Contatti_Contab.Cod_Contatto ")
            stb.AppendLine("  ")
            stb.AppendLine("    INNER JOIN Movimenti Movimenti_Mag  ")
            stb.AppendLine("        ON Agenda.PIVA = Movimenti_Mag.PIVA  ")
            stb.AppendLine("        AND Agenda.Sa_Cod = Movimenti_Mag.Sa_Cod  ")
            stb.AppendLine("        AND Agenda.Id_Agenda = Movimenti_Mag.Id_Agenda ")
            stb.AppendLine("  ")
            stb.AppendLine("    INNER JOIN Movimenti_dettagli  ")
            stb.AppendLine("        ON Movimenti_dettagli.PIVA = Movimenti_Mag.PIVA  ")
            stb.AppendLine("        AND Movimenti_dettagli.Id_Agenda = Movimenti_Mag.Id_Agenda  ")
            stb.AppendLine("        AND Movimenti_dettagli.Id_Mov = Movimenti_Mag.Id_Mov  ")

            '  Vanni, 25/06/2013 09:53:35: x formulati
            stb.AppendLine("        AND Movimenti_dettagli.Elem_Cod = " & Esportazione_Elem_Cod)


            '  Vanni, 19/06/2015 17:00:27: Sostituito il join di mov_destinazioni: dalla movimenti alla movimenti dettagli, sostituita tabella "Movimenti_Mag" con "Movimenti_dettagli"
            stb.AppendLine("    INNER JOIN Mov_Destinazioni MovimentiDest_MAG ")
            stb.AppendLine("        ON MovimentiDest_MAG.Piva = Movimenti_dettagli.PIVA  ")
            stb.AppendLine("        and MovimentiDest_MAG.Id_Agenda = Movimenti_dettagli.Id_Agenda  ")
            stb.AppendLine("        and MovimentiDest_MAG.Id_Mov = Movimenti_dettagli.Id_Mov  ")
            stb.AppendLine("        and MovimentiDest_MAG.Id_Mov_det = Movimenti_dettagli.Id_Mov_det  ") 'aggiunta condizione su id_mov_Det
            stb.AppendLine("        and MovimentiDest_MAG.Tipo_Destinazione = 20 ")


            stb.AppendLine("    INNER JOIN UnitaMisura  ")
            stb.AppendLine("        ON Movimenti_dettagli.Udm_Cod = UnitaMisura.Udm_Cod ")
            stb.AppendLine("  ")


            If Esportazione_Elem_Cod = 3 Then
                stb.AppendLine("    INNER JOIN CAC_Codifica_ProdottiAziendali  ")
                stb.AppendLine("        on CAC_Codifica_ProdottiAziendali.Codice_GIAS = Movimenti_dettagli.Pro_Cod  ")


                stb.AppendLine("        and CAC_Codifica_ProdottiAziendali.Elem_Cod = 3 ")
                stb.AppendLine("         and CAC_Codifica_ProdottiAziendali.Tipo_Codifica =   " & enum_Tipo_CAC_Codifica_ProdottiAziendali.Coldiretti_RegioneUmbria)

                stb.AppendLine("    inner join UnitaMisura udmSalvata ")
                stb.AppendLine("        on udmSalvata.UDM_COD = Movimenti_dettagli.Udm_Cod ")
                stb.AppendLine("  ")


                stb.AppendLine("    left JOIN SIGPA_CodificaFertilizzanti s ")



                stb.AppendLine("        on s.CODICE = CAC_Codifica_ProdottiAziendali.Cod_Prodotto_Cliente    ")
                stb.AppendLine("  ")
                stb.AppendLine("  ")
                stb.AppendLine("    left JOIN UnitaMisura udmExtra ")
                stb.AppendLine("        on udmExtra.UDM_COD = Movimenti_dettagli.Extra_Int   ")
                stb.AppendLine("  ")
                stb.AppendLine("    left JOIN UnitaMisura udmSigpa  ")
                stb.AppendLine("        on udmSigpa.UDM_SIM = s.UNITA_Misura collate Latin1_General_CI_AS ")
                stb.AppendLine("  ")
                stb.AppendLine("    left join UnitaMisura_Conversione convSalvaToExtra ")
                stb.AppendLine("        on convSalvaToExtra.UDM_COD_Da = Movimenti_dettagli.Udm_Cod   ")
                stb.AppendLine("        and convSalvaToExtra.UDM_COD_A = case when  Movimenti_dettagli.Extra_Int = 0 then Movimenti_dettagli.Udm_Cod else Movimenti_dettagli.Extra_Int end ")
                stb.AppendLine("  ")
                stb.AppendLine("    left JOIN UnitaMisura_Conversione convSigpa ")
                stb.AppendLine("        on convSigpa.UDM_COD_Da = case when  Movimenti_dettagli.Extra_Int = 0 then Movimenti_dettagli.Udm_Cod else Movimenti_dettagli.Extra_Int end ")
                stb.AppendLine("        and convSigpa.UDM_COD_A = udmSigpa.Udm_Cod ")
            End If


            stb.AppendLine("  ")
            stb.AppendLine("    INNER JOIN Imprese_Codici  ")
            stb.AppendLine("        on Imprese_Codici.PIVA = Agenda.PIVA  ")
            stb.AppendLine("        and Imprese_Codici.id_cod = 1010 ")
            stb.AppendLine("  ")
            stb.AppendLine("    WHERE   Movimenti_Contab.Data_Movimento <=  " & Agro_SQL_SaveDate(DataFine) & "   ")
            stb.AppendLine("     AND     Movimenti_Contab.Data_Movimento >=  " & Agro_SQL_SaveDate(DataInizio) & "   ")
            stb.AppendLine("    AND     Movimenti_Contab.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'   ")
            stb.AppendLine("     AND    Agenda.Sa_Cod = 0  ")
            stb.AppendLine("     AND   Agenda.Lav_Cod IN ( " &
                                                CStr(LAVCOD_FATTURA_EMESSA) & ", " &
                                                CStr(LAVCOD_FATTURA_RICEVUTA) & " " &
                                                " )  ")

            stb.AppendLine("     AND    Movimenti_Mag.Cau_Mov IN ( '" &
                                                CStr(CAU_CARICO) & "', '" &
                                                CStr(CAU_SCARICO) & "' " &
                                                " )  ")

            stb.AppendLine("     AND Movimenti_dettagli.Dettagli_Blocco_Flag = 0 ")
            stb.AppendLine("     AND Movimenti_dettagli.Jolly_int = 0 ")


            '/************************************************************************************
            '/***** 2° PARTE: FATTURE DIFFERITE  ***************
            '/************************************************************************************
            stb.AppendLine("    UNION  ")

            stb.AppendLine(" SELECT   ")
            stb.AppendLine("         Agenda.PIVA  ")
            stb.AppendLine("          , Agenda.Sa_Cod   ")
            stb.AppendLine("          , Agenda.Id_Agenda  ")
            stb.AppendLine("          , Imprese_Codici.val_cod as CUAA_Impresa   ")
            stb.AppendLine("          , Agenda.Lav_Cod  ")
            stb.AppendLine("          , Agenda.des_lib  ")
            stb.AppendLine("          , Movimenti_Contab.Cod_RisUm  ")
            stb.AppendLine("          , Movimenti_Contab.Data_Movimento  ")
            stb.AppendLine("          , Movimenti_Contab.Doc_Numero_Sin  ")
            stb.AppendLine("          , Movimenti_Contab.Doc_Numero  ")
            stb.AppendLine("          , Movimenti_Contab.Doc_Numero_Des          ")
            stb.AppendLine("          , case when Contatti_Contab.Codice_Fiscale = '' then Contatti_Contab.Cod_Contatto else Contatti_Contab.Codice_Fiscale end as CUAA_Cliente_Fornitore  ")
            stb.AppendLine("          , (Contatti_Contab.Rag_Soc + Contatti_Contab.Nome +' ' + Contatti_Contab.cognome ) AS Contatto  ")
            If Esportazione_Elem_Cod = 3 Then
                stb.AppendLine("          , Movimenti_dettagli.Qta * coalesce(convSigpa.FattoreConversione, 1) * coalesce(convSalvaToExtra.FattoreConversione, 1) as Qta  ")
            Else
                stb.AppendLine("          , Movimenti_dettagli.Qta  ")
            End If
            stb.AppendLine("          , Movimenti_dettagli.Elem_Cod  ")
            stb.AppendLine("          , Movimenti_dettagli.id_mov  ")
            stb.AppendLine("          , Movimenti_dettagli.id_mov_det  ")



            'codice prodotto e unità misura
            If Esportazione_Elem_Cod = 3 Then
                stb.AppendLine("      , CAC_Codifica_ProdottiAziendali.Cod_Prodotto_Cliente as Codice_Prodotto_SIGPA  ")
                stb.AppendLine("                , case when s.UNITA_Misura is not null then s.UNITA_Misura else  ")
                stb.AppendLine("                case when udmExtra.UDM_COD = 0 then  ")
                stb.AppendLine("                    udmSalvata.UDM_SIM collate latin1_general_CI_AS ")
                stb.AppendLine("            Else ")
                stb.AppendLine("                 udmExtra.UDM_SIM ")
                stb.AppendLine("                 End ")
                stb.AppendLine("            End ")
                stb.AppendLine("           as udm_sim ")

            Else
                stb.AppendLine("      , Movimenti_dettagli.Pro_Cod as Codice_Prodotto_SIGPA  ")
                stb.AppendLine("          , udm_sim  ")
            End If

            stb.AppendLine("          , Movimenti_DDT.Doc_Numero_Sin + cast(Movimenti_DDT.Doc_Numero as varchar(100)) + Movimenti_DDT.Doc_Numero_Des as NumeroBolla ")
            stb.AppendLine("          , Movimenti_DDT.Data_Movimento   ")
            stb.AppendLine("          , '' as DenunciaNumero ")
            stb.AppendLine("          , GETDATE() as DataDenuncia ")
            stb.AppendLine("          , ppContaMagazzini.numeroMagazzini  ")
            stb.AppendLine("          , MovimentiDest_MAG.Id_Destinazione - " & BaseCod & "  as Id_Destinazione ")
            stb.AppendLine("          , Movimenti_dettagli.Pendente as causaleGIAS ")
            stb.AppendLine("          , P.Pratica_COD ")


            stb.AppendLine("  ")

            stb.AppendLine(" FROM  ( select distinct figlio from GerarchieImpreseEsplosa  where Padre in  (" & PIVA_PadreInGerarchia & ")  ) es  ")
            stb.AppendLine("  ")
            stb.AppendLine("      INNER JOIN  Agenda    ")
            stb.AppendLine("          on es.Figlio = Agenda.PIVA")

            If SIGPA_ComandiEsportazione_cod > 0 Then
                stb.AppendLine("    inner join SIGPA_ComandiEsportazione_ListaImprese LImp ")
                stb.AppendLine("    on LImp.piva = Agenda.Piva ")
                stb.AppendLine("    and LImp.SIGPA_ComandiEsportazione_cod = " & Agro_SQL_SaveNum(SIGPA_ComandiEsportazione_cod))
            End If

            stb.AppendLine("    inner join Pratiche p ")
            stb.AppendLine("        on p.Piva = Agenda.PIVA   ")
            stb.AppendLine("        and p.Servizio_Cod =  " & servizio_cod)
            stb.AppendLine("        and p.piva_SuperUser = '" & objParametri.PivaSuperUser & "' ")

            stb.AppendLine("  ")
            stb.AppendLine("    inner join Pratiche_Stati_Attuali SP ")
            stb.AppendLine("        on SP.Pratica_Cod = p.Pratica_Cod  ")
            stb.AppendLine("        and SP.Stato_Cod in (2) ")

            stb.AppendLine("    INNER JOIN Movimenti Movimenti_Contab  ")
            stb.AppendLine("        ON Agenda.PIVA = Movimenti_Contab.PIVA  ")
            stb.AppendLine("        AND Agenda.Sa_Cod = Movimenti_Contab.Sa_Cod  ")
            stb.AppendLine("        AND Agenda.Id_Agenda = Movimenti_Contab.Id_Agenda ")
            stb.AppendLine("  ")
            stb.AppendLine("    INNER JOIN ( ")
            stb.AppendLine("        Select Piva, COUNT (*) as numeroMagazzini ")
            stb.AppendLine("        from Fabbricati  ")
            stb.AppendLine("        group by piva ")
            stb.AppendLine("    ) ppContaMagazzini  ")
            stb.AppendLine("        on ppContaMagazzini.PIVA = Agenda.PIVA  ")
            stb.AppendLine("    INNER JOIN Risorse_Umane RisUm_Contab  ")
            stb.AppendLine("        ON RisUm_Contab.Cod_RisUm = Movimenti_Contab.Cod_RisUm  ")
            stb.AppendLine("  ")
            stb.AppendLine("    INNER JOIN Contatti Contatti_Contab  ")
            stb.AppendLine("        ON RisUm_Contab.Piva = Contatti_Contab.Piva  ")
            stb.AppendLine("        AND RisUm_Contab.Cod_Contatto = Contatti_Contab.Cod_Contatto ")
            stb.AppendLine("  ")
            stb.AppendLine("    INNER JOIN Movimenti Movimenti_Mag  ")
            stb.AppendLine("        ON Agenda.PIVA = Movimenti_Mag.PIVA  ")
            stb.AppendLine("        AND Agenda.Sa_Cod = Movimenti_Mag.Sa_Cod  ")
            stb.AppendLine("        AND Agenda.Id_Agenda = Movimenti_Mag.Id_Agenda ")
            stb.AppendLine("  ")
            stb.AppendLine("    INNER JOIN Movimenti_dettagli  ")
            stb.AppendLine("        ON Movimenti_dettagli.PIVA = Movimenti_Mag.PIVA  ")
            stb.AppendLine("        AND Movimenti_dettagli.Id_Agenda = Movimenti_Mag.Id_Agenda  ")
            stb.AppendLine("        AND Movimenti_dettagli.Id_Mov = Movimenti_Mag.Id_Mov  ")
            '  Vanni, 25/06/2013 09:53:35: x formulati
            stb.AppendLine("        AND Movimenti_dettagli.Elem_Cod = " & Esportazione_Elem_Cod)


            '  Vanni, 19/06/2015 17:00:27: Sostituito il join di mov_destinazioni: dalla movimenti alla movimenti dettagli, sostituita tabella "Movimenti_Mag" con "Movimenti_dettagli"
            stb.AppendLine("    INNER JOIN Mov_Destinazioni MovimentiDest_MAG ")
            stb.AppendLine("        ON MovimentiDest_MAG.Piva = Movimenti_dettagli.PIVA  ")
            stb.AppendLine("        and MovimentiDest_MAG.Id_Agenda = Movimenti_dettagli.Id_Agenda  ")
            stb.AppendLine("        and MovimentiDest_MAG.Id_Mov = Movimenti_dettagli.Id_Mov  ")
            stb.AppendLine("        and MovimentiDest_MAG.Id_Mov_det = Movimenti_dettagli.Id_Mov_det  ") 'aggiunta condizione su id_mov_Det
            stb.AppendLine("        and MovimentiDest_MAG.Tipo_Destinazione = 20 ")

            stb.AppendLine("    INNER JOIN UnitaMisura  ")
            stb.AppendLine("        ON Movimenti_dettagli.Udm_Cod = UnitaMisura.Udm_Cod ")
            stb.AppendLine("  ")





            If Esportazione_Elem_Cod = 3 Then

                stb.AppendLine("    INNER JOIN CAC_Codifica_ProdottiAziendali  ")
                stb.AppendLine("        on CAC_Codifica_ProdottiAziendali.Codice_GIAS = Movimenti_dettagli.Pro_Cod  ")

                stb.AppendLine("        and CAC_Codifica_ProdottiAziendali.Elem_Cod = 3 ")
                stb.AppendLine("         and CAC_Codifica_ProdottiAziendali.Tipo_Codifica = " & enum_Tipo_CAC_Codifica_ProdottiAziendali.Coldiretti_RegioneUmbria)


                stb.AppendLine("    inner join UnitaMisura udmSalvata ")
                stb.AppendLine("        on udmSalvata.UDM_COD = Movimenti_dettagli.Udm_Cod ")
                stb.AppendLine("  ")


                stb.AppendLine("    left JOIN SIGPA_CodificaFertilizzanti s ")
                stb.AppendLine("        on s.CODICE = CAC_Codifica_ProdottiAziendali.Cod_Prodotto_Cliente    ")
                stb.AppendLine("  ")
                stb.AppendLine("  ")
                stb.AppendLine("    left JOIN UnitaMisura udmExtra ")
                stb.AppendLine("        on udmExtra.UDM_COD = Movimenti_dettagli.Extra_Int   ")
                stb.AppendLine("  ")
                stb.AppendLine("    left JOIN UnitaMisura udmSigpa  ")
                stb.AppendLine("        on udmSigpa.UDM_SIM = s.UNITA_Misura collate Latin1_General_CI_AS ")
                stb.AppendLine("  ")
                stb.AppendLine("    left join UnitaMisura_Conversione convSalvaToExtra ")
                stb.AppendLine("        on convSalvaToExtra.UDM_COD_Da = Movimenti_dettagli.Udm_Cod   ")
                stb.AppendLine("        and convSalvaToExtra.UDM_COD_A = case when  Movimenti_dettagli.Extra_Int = 0 then Movimenti_dettagli.Udm_Cod else Movimenti_dettagli.Extra_Int end ")
                stb.AppendLine("  ")
                stb.AppendLine("    left JOIN UnitaMisura_Conversione convSigpa ")
                stb.AppendLine("        on convSigpa.UDM_COD_Da = case when  Movimenti_dettagli.Extra_Int = 0 then Movimenti_dettagli.Udm_Cod else Movimenti_dettagli.Extra_Int end ")
                stb.AppendLine("        and convSigpa.UDM_COD_A = udmSigpa.Udm_Cod ")

            End If


            stb.AppendLine("  ")
            stb.AppendLine("    INNER JOIN Imprese_Codici  ")
            stb.AppendLine("        on Imprese_Codici.PIVA = Agenda.PIVA  ")
            stb.AppendLine("        and Imprese_Codici.id_cod = 1010 ")
            stb.AppendLine("  ")
            stb.AppendLine("    INNER JOIN Mov_Dettagli_Riferimenti  ")
            stb.AppendLine("        on Mov_Dettagli_Riferimenti.PIVA = Movimenti_dettagli.PIVA  ")

            '  Vanni, 19/06/2015 15:45:35: viene memorizzata una coppia in Mov_Dettagli_Riferimenti differente dalla tabella Movimenti_dettagli.. commento sa_cod e recupero per movimento dettaglio..
            'stb.AppendLine("        and Mov_Dettagli_Riferimenti.sa_cod = Agenda.sa_cod ")
            stb.AppendLine("        and Mov_Dettagli_Riferimenti.id_agenda = Movimenti_dettagli.id_agenda ")

            '  Vanni, 25/07/2014 10:29:49: viene memorizzata una coppia in Mov_Dettagli_Riferimenti differente dalla tabella Movimenti_dettagli.. commento id_mov e recupero per movimento dettaglio..
            'stb.AppendLine("        and Mov_Dettagli_Riferimenti.id_mov = Movimenti_dettagli.id_mov ")
            stb.AppendLine("        and Mov_Dettagli_Riferimenti.id_mov_det = Movimenti_dettagli.id_mov_det ")
            stb.AppendLine("  ")

            stb.AppendLine("    INNER JOIN Movimenti Movimenti_DDT  ")
            stb.AppendLine("        ON Mov_Dettagli_Riferimenti.PIVA_Rif = Movimenti_DDT.PIVA  ")

            '  Vanni, 02/04/2015 15:52:13: viene memorizzata una coppia in Mov_Dettagli_Riferimenti differente dalla tabella Movimenti_dettagli.. commento sa_cod e recupero per movimento dettaglio..
            'stb.AppendLine("        AND Mov_Dettagli_Riferimenti.Sa_Cod_rif = Movimenti_DDT.Sa_Cod  ")
            stb.AppendLine("        AND Mov_Dettagli_Riferimenti.Id_Agenda_Rif = Movimenti_DDT.Id_Agenda ")
            stb.AppendLine("        AND Mov_Dettagli_Riferimenti.Id_Mov_Rif = Movimenti_DDT.Id_Mov ")
            stb.AppendLine("  ")

            stb.AppendLine("    WHERE   Movimenti_Contab.Data_Movimento <=  " & Agro_SQL_SaveDate(DataFine) & "   ")
            stb.AppendLine("     AND     Movimenti_Contab.Data_Movimento >=  " & Agro_SQL_SaveDate(DataInizio) & "   ")
            stb.AppendLine("    AND     Movimenti_Contab.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'   ")
            stb.AppendLine("     AND    Agenda.Sa_Cod = 0  ")
            stb.AppendLine("     AND   Agenda.Lav_Cod IN ( " &
                                                CStr(LAVCOD_FATTURA_EMESSA) & ", " &
                                                CStr(LAVCOD_FATTURA_RICEVUTA) & " " &
                                                " )  ")

            stb.AppendLine("     AND    Movimenti_Mag.Cau_Mov IN ( '" &
                                                CStr(CAU_CARICO) & "', '" &
                                                CStr(CAU_SCARICO) & "' " &
                                                " )  ")

            stb.AppendLine("     AND Movimenti_dettagli.Dettagli_Blocco_Flag = 0 ")
            stb.AppendLine("     AND Movimenti_dettagli.Jolly_int = 1 ")

            stb.AppendLine("     AND   Mov_Dettagli_Riferimenti.Lav_Cod IN ( " &
                                               CStr(LAVCOD_FATTURA_EMESSA) & ", " &
                                               CStr(LAVCOD_FATTURA_RICEVUTA) & " " &
                                               " )  ")

            stb.AppendLine("     AND   Mov_Dettagli_Riferimenti.Lav_Cod_Rif IN ( " &
                                   CStr(LAVCOD_BOLLA_EMESSA) & ", " &
                                   CStr(LAVCOD_BOLLA_RICEVUTA) & " " &
                                   " )  ")


            '/************************************************************************************
            '/***** 3° PARTE: SCARICO MAGAZZINO  ***************
            '/************************************************************************************
            stb.AppendLine("    UNION  ")

            DocumentiContabili_CarichiScarichi_EsportazioneSIGPAQry_C_S(SIGPA_ComandiEsportazione_cod, PIVA_PadreInGerarchia, DataInizio, DataFine, BaseCod, Esportazione_Elem_Cod, objParametri, stb, servizio_cod, "", False, True)





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

#End Region

    '################################################################################
    Public Function EsisteDocumento(ByVal Piva As String,
                                    ByVal Lav_Cod As Integer,
                                    ByVal Anno As Integer,
                                    ByVal Data As Date,
                                    ByVal Doc_Numero_Sin As String,
                                    ByVal Doc_Numero As Decimal,
                                    ByVal Doc_Numero_Des As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim dt As DataTable
        Dim objMovimenti As New AgronicaCoreContabDAL.Movimenti_R

        dt = objMovimenti.MovimentiContabili(Lav_Cod,
                                             Piva,
                                             0,
                                             0,
                                             0,
                                             0,
                                             CAU_REGISTRAZIONI,
                                             Anno,
                                             Data,
                                             AGRODATAINIZIO,
                                             Doc_Numero_Sin,
                                             Doc_Numero,
                                             Doc_Numero_Des,
                                             0, 0, AGRODATAINIZIO,
                                             "", "",
                                             objParametri)

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            Return True
        Else
            Return False
        End If

    End Function


    '################################################################################
    Public Function EsisteDocumentoAcquisto(ByVal Piva As String,
                                            ByVal Lav_Cod As Integer,
                                            ByVal Cod_RisUm As Integer,
                                            ByVal Anno As Integer,
                                            ByVal Data As Date,
                                            ByVal Doc_Numero_Sin As String,
                                            ByVal Doc_Numero As Decimal,
                                            ByVal Doc_Numero_Des As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As Boolean

        Dim dt As DataTable
        Dim objMovimenti As New AgronicaCoreContabDAL.Movimenti_R

        dt = objMovimenti.MovimentiContabili(Lav_Cod,
                                             Piva,
                                             0,
                                             0,
                                             0,
                                             Cod_RisUm,
                                             CAU_REGISTRAZIONI,
                                             Anno,
                                             Data,
                                             AGRODATAINIZIO,
                                             Doc_Numero_Sin,
                                             Doc_Numero,
                                             Doc_Numero_Des,
                                             0, 0, AGRODATAINIZIO,
                                             "", "",
                                             objParametri)

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            Return True
        Else
            Return False
        End If

    End Function



    '################################################################################
    Public Function NumeroDoc_from_CauMov(ByVal Piva As String,
                                          ByVal Id_Agenda As Integer,
                                          ByVal Cau_Mov As String,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                          ) As String

        Dim dt As DataTable
        Dim objMovimenti As New AgronicaCoreContabDAL.Movimenti_R
        Dim docNumeroSin As String = ""
        Dim docNumero As Decimal = 0
        Dim docNumeroDes As String = ""
        Dim Numero_Doc As String = ""

        dt = objMovimenti.MovimentiContabili(0,
                                             Piva,
                                             0,
                                             Id_Agenda,
                                             0,
                                             0,
                                             Cau_Mov,
                                             0,
                                             AGRODATAINIZIO,
                                             AGRODATAINIZIO,
                                             "XYZ",
                                             0,
                                             "XYZ",
                                             0, 0, AGRODATAINIZIO,
                                             "", "",
                                             objParametri)

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            Numero_Doc = dt.Rows(0).Item("Doc_Numero_Sin") & " " & CStr(dt.Rows(0).Item("Doc_Numero")) & " " & dt.Rows(0).Item("Doc_Numero_Des")
        End If

        Return Numero_Doc

    End Function

#Region "X"
    'Optional ByVal Piva As String = "", _
    'Optional ByVal Sa_Cod As Integer = 0, _
    'Optional ByVal Id_Agenda As Integer = 0, _
    'Optional ByVal Id_Mov As Integer = 0, _
    'Optional ByVal Id_Mov_Det As Integer = 0, _
    'Optional ByVal Elem_Cod As Integer = 0, _
    'Optional ByVal Pro_Cod As Integer = 0, _
    'Optional ByVal Mat_Cod As Integer = 0, _
    'Optional ByVal Cal_Cod As Integer = 0, _
    'Optional ByVal Cod_Progetto As Integer = 0, _
    'Optional ByVal Fase_Cod As Integer = 0, _
    'Optional ByVal Udm_Cod As Integer = 0, _
    'Optional ByVal Lotto As String = "", _
    'Optional ByVal Contabilizzato As Integer = 0, _
    'Optional ByVal Pendente As Integer = 0, _
    'Optional ByVal Cod_Conto As Integer = 0, _
    'Optional ByVal Ric_Cod As Integer = 0, _
    'Optional ByVal Anno As Integer = 0, _
    'Optional ByVal Cau_Mov As String = "", _
    'Optional ByVal Cod_RisUm As Integer = 0, _
    'Optional ByVal Cod_Contatto As String = "", _
    'Optional ByVal Piva_Contatto As String = "", _
    'Optional ByVal FinestraTemp_Inizio As String = "01/01/1900", _
    'Optional ByVal FinestraTemp_Fine As String = "31/12/2100", _
    'Optional ByVal Flag_CostiAccessori_Corrispettivi As Boolean = False, _
    'Optional ByVal Flag_Contabilita As Boolean = True, _
    'Optional ByVal FiltroAggiuntivo As String = "", _
    'Optional ByVal Ordinamento As String = "", _
    'Optional ByVal Cod_RisUm_Origine As Integer = 0, _
    'Optional ByVal Piva_SuperUser_Origine As String = "", _
    'Optional ByVal Flag_AncheImportati As Boolean = True, _
    'Optional ByVal Doc_Numero_Sin As String = "XYZ", _
    'Optional ByVal Doc_Numero As Decimal = 0, _
    'Optional ByVal Doc_Numero_Des As String = "XYZ", _
    'Optional ByVal Scadenza As String = "31/12/2100", _
    'Optional ByVal Flag_AncheMagazzino As Boolean = False, _
#End Region

    Public Function Contabilita_Movimenti_Dettagli_Leggi(
                                  ByVal Piva As String,
                                  ByVal Sa_Cod As Integer,
                                  ByVal Id_Agenda As Integer,
                                  ByVal Id_Mov As Integer,
                                  ByVal Id_Mov_Det As Integer,
                                  ByVal Elem_Cod As Integer,
                                  ByVal Pro_Cod As Integer,
                                  ByVal Mat_Cod As Integer,
                                  ByVal Cal_Cod As Integer,
                                  ByVal Cod_Progetto As Integer,
                                   ByVal Fase_Cod As Integer,
                                   ByVal Udm_Cod As Integer,
                                   ByVal Lotto As String,
                                   ByVal Contabilizzato As Integer,
                                   ByVal Pendente As Integer,
                                   ByVal Cod_Conto As Integer,
                                   ByVal Ric_Cod As Integer,
                                   ByVal Anno As Integer,
                                   ByVal Cau_Mov As String,
                                   ByVal Cod_RisUm As Integer,
                                   ByVal Cod_Contatto As String,
                                   ByVal Piva_Contatto As String,
                                   ByVal FinestraTemp_Inizio As String,
                                   ByVal FinestraTemp_Fine As String,
                                   ByVal Flag_CostiAccessori_Corrispettivi As Boolean,
                                   ByVal Flag_Contabilita As Boolean,
                                   ByVal FiltroAggiuntivo As String,
                                   ByVal Ordinamento As String,
                                   ByVal Cod_RisUm_Origine As Integer,
                                   ByVal Piva_SuperUser_Origine As String,
                                   ByVal Flag_AncheImportati As Boolean,
                                   ByVal Doc_Numero_Sin As String,
                                   ByVal Doc_Numero As Decimal,
                                   ByVal Doc_Numero_Des As String,
                                   ByVal Scadenza As String,
                                   ByVal Flag_AncheMagazzino As Boolean,
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                  ) As DataTable

        'Optional ByVal Flag_Join_CodConto As Boolean = True

        '----- Descrizione

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Contabilita_R.Contabilita_Movimenti_Dettagli_Leggi"

        'Dim StrSQL As String
        Dim SQLGenerale As New StringBuilder
        Dim SQLSelect As New StringBuilder
        Dim SQLSelectSiMag As New StringBuilder
        Dim SQLSelectNoMag As New StringBuilder
        Dim SQLJoin As New StringBuilder
        Dim SQLJoinSiMag As New StringBuilder
        Dim SQLWhere As New StringBuilder
        Dim SQLOrderBy As New StringBuilder

        Dim messaggioErrore As String = ""
        Dim dt As New DataTable

        '----------------------------------------------------
        '--- Preparo la Query SQL ---------------------------
        '----------------------------------------------------
        Try

            SQLGenerale.Length = 0
            SQLSelect.Length = 0
            SQLSelectSiMag.Length = 0
            SQLSelectNoMag.Length = 0
            SQLJoin.Length = 0
            SQLJoinSiMag.Length = 0
            SQLWhere.Length = 0
            SQLOrderBy.Length = 0

            SQLSelect.AppendLine(" SELECT Imprese.Rag_Soc AS Impresa, Operazioni.LAV_DES, ")
            SQLSelect.AppendLine(" Agenda.PIVA, Agenda.Sa_Cod, Agenda.Id_Agenda, Agenda.Lav_Cod, Agenda.des_lib, ")
            SQLSelect.AppendLine(" Agenda.Blocco_Flag, Agenda.Blocco_Data, Agenda.Blocco_Username, Agenda.Stato_Export, ")

            'movimento di magazzino
            SQLSelect.AppendLine("   Movimenti.Id_Mov AS Id_Mov_Mag, Movimenti.Cau_Mov AS Cau_Mov_Mag, Movimenti.Mov_Desc AS Mov_Desc_Mag, Movimenti.Data_Movimento AS Data_Movimento_Mag, ")

            'movimento contabile
            SQLSelect.AppendLine("   Movimenti_Contab.Id_Mov, Movimenti_Contab.Cod_RisUm, Movimenti_Contab.Cau_Mov, Movimenti_Contab.Mov_Desc, ISNULL(Movimenti_Contab.Num_Protocollo,0) AS Num_Protocollo, ")
            SQLSelect.AppendLine("   Movimenti_Contab.Data_Movimento, Movimenti_Contab.Scadenza, Movimenti_Contab.Doc_Numero_Sin, Movimenti_Contab.Doc_Numero, Movimenti_Contab.Doc_Numero_Des, ")
            SQLSelect.AppendLine("    Movimenti_Contab.Cod_IndirizzoRisUm, Movimenti_Contab.Cod_Destinazione, Movimenti_Contab.Cod_IndirizzoDestinazione,  ")
            SQLSelect.AppendLine("    Movimenti_Contab.Mezzo, Movimenti_Contab.Cod_Vettore, Movimenti_Contab.Cod_IndirizzoVettore, Movimenti_Contab.Causale_Trasporto, ")
            SQLSelect.AppendLine("   Movimenti_Contab.Aspetto, Movimenti_Contab.Peso, Movimenti_Contab.Ora, Movimenti_Contab.Colli, Movimenti_Contab.Tipo_Sconto, ")
            SQLSelect.AppendLine("    Movimenti_Contab.Natura_Beni, Movimenti_Contab.Tara_Veicolo, Movimenti_Contab.Tara_Imballi, ")
            SQLSelect.AppendLine("    Movimenti_Contab.Tipo_Peso, Movimenti_Contab.Modalita, Movimenti_Contab.Username_Note, Movimenti_Contab.Scadenza_Extra, ")
            SQLSelect.AppendLine("    Movimenti_Contab.Extra_Str, Movimenti_Contab.Extra_Int, Movimenti_Contab.Extra_Date, ")
            SQLSelect.AppendLine("    Movimenti_Contab.Progr_Protocollo, Movimenti_Contab.Progr_Registrazione, Movimenti_Contab.Data_Registrazione, ")

            SQLSelect.AppendLine("   Movimenti_dettagli.Id_Mov_Det, Movimenti_dettagli.Sa_Cod AS Sa_Cod_Dett, Movimenti_dettagli.Elem_Cod, Movimenti_dettagli.Pro_Cod, Movimenti_dettagli.Mat_Cod, ")
            SQLSelect.AppendLine("   Movimenti_dettagli.Mov_Det_Des, Movimenti_dettagli.Udm_Cod, Movimenti_dettagli.Qta, Movimenti_dettagli.Cod_Iva, Movimenti_dettagli.Cod_IvaIndetraibile, Movimenti_dettagli.Sconto, ")
            SQLSelect.AppendLine("     Movimenti_dettagli.Prezzo_Unitario, Movimenti_dettagli.Cod_Conto, Movimenti_dettagli.Cod_Progetto, Movimenti_dettagli.Fase_Cod, ")
            SQLSelect.AppendLine("   Movimenti_dettagli.Contabilizzato, Movimenti_dettagli.Pendente, Movimenti_dettagli.Cal_Cod, Movimenti_dettagli.Anno, Movimenti_dettagli.Ric_Cod,  ")
            SQLSelect.AppendLine("  Movimenti_dettagli.Imponibile, Movimenti_dettagli.Iva, Movimenti_dettagli.Lotto, Movimenti_dettagli.Jolly_Int, Movimenti_dettagli.Imponibile_Netto, ")
            SQLSelect.AppendLine("   Movimenti_dettagli.Prezzo_Unitario_Netto, Movimenti_dettagli.UDM_COD_EXTRA, Movimenti_dettagli.QTA_EXTRA, Movimenti_dettagli.Prezzo_Effettivo, ")
            SQLSelect.AppendLine(" IVA_Aliquote.Sigla AS Sigla_IVA, ")

            SQLSelect.AppendLine(" RicXConti.Id_Riclassificazione, RicXConti.Dare_Avere, RicXConti.Saldo,  Conti.Conto_Descr, Conti.Flag_UE ")

            If Flag_AncheMagazzino = True Then
                SQLSelectSiMag.AppendLine(" ,Mov_Destinazioni.Id_Destinazione, Mov_Destinazioni.Sa_Cod AS Sa_Cod_Dest, Mov_Destinazioni.Tipo_Destinazione, Mov_Destinazioni.Qta AS Qta_Dest, Mov_Destinazioni.Qta2, ")
                SQLSelectSiMag.AppendLine(" Mov_Destinazioni.Tipo_Scorta, Mov_Destinazioni.Scorta_Min, Fabbricati.Fabbricato_Des, Fabbricati_Tipi.Tipo_Fabbricato_Cod,  ")
                SQLSelectSiMag.AppendLine(" Fabbricati_Tipi.Tipo_Fabbricato_Des ")
            End If

            If Flag_CostiAccessori_Corrispettivi = True Then
                SQLSelect.AppendLine(", ")
                SQLSelect.AppendLine(" Contatti_Corrisp.Id_CF AS Id_CF_Corrisp, Contatti_Corrisp.Rag_Soc AS Rag_Soc_Corrisp, Contatti_Corrisp.Codice_Fiscale AS Codice_Fiscale_Corrisp, Contatti_Corrisp.Sa_Cod AS Sa_Cod_Corrisp, Contatti_Contab.Cod_Contatto AS Cod_Contatto_Corrisp,  ")
                SQLSelect.AppendLine(" RisUm_Corrisp.Cod_RisUm AS Cod_RisUm_Corrisp, RisUm_Corrisp.Cod_Rapporto AS Cod_Rapporto_Corrisp, RisUm_Corrisp.Validita_Inizio AS Validita_Inizio_Corrisp,  RisUm_Corrisp.Validita_Fine AS Validita_Fine_Corrisp,  ")
                SQLSelect.AppendLine(" RisUm_Corrisp.Patentino AS Patentino_Corrisp, RisUm_Corrisp.Data_Rilascio_Patentino AS Data_Rilascio_Patentino_Corrisp, RisUm_Corrisp.Data_Scadenza_Patentino AS Data_Scadenza_Patentino_Corrisp,   ")
                SQLSelect.AppendLine(" RisUm_Corrisp.Cod_RisUm_Origine AS Cod_RisUm_Origine_Corrisp, RisUm_Corrisp.Piva_SuperUser_Origine AS Piva_SuperUser_Origine_Corrisp, ")
                SQLSelect.AppendLine(" RisUm_Corrisp.Settore_Des AS Settore_Des_Corrisp, RisUm_Corrisp.Attivita_Des AS Attivita_Des_Corrisp  ")
            End If

            If Flag_Contabilita = True Then
                SQLSelect.AppendLine(", ")
                SQLSelect.AppendLine(" Contatti_Contab.Id_CF AS Id_CF_Contab, Contatti_Contab.Rag_Soc AS Rag_Soc_Contab, Contatti_Contab.Codice_Fiscale AS Codice_Fiscale_Contab, Contatti_Contab.Sa_Cod AS Sa_Cod_Contab, Contatti_Contab.Cod_Contatto AS Cod_Contatto_Contab,  ")
                SQLSelect.AppendLine(" RisUm_Contab.Cod_RisUm AS Cod_RisUm_Contab, RisUm_Contab.Cod_Rapporto AS Cod_Rapporto_Contab, RisUm_Contab.Validita_Inizio AS Validita_Inizio_Contab,  RisUm_Contab.Validita_Fine AS Validita_Fine_Contab,  ")
                SQLSelect.AppendLine(" RisUm_Contab.Patentino AS Patentino_Contab, RisUm_Contab.Data_Rilascio_Patentino AS Data_Rilascio_Patentino_Contab, RisUm_Contab.Data_Scadenza_Patentino AS Data_Scadenza_Patentino_Contab,   ")
                SQLSelect.AppendLine(" RisUm_Contab.Cod_RisUm_Origine AS Cod_RisUm_Origine_Contab, RisUm_Contab.Piva_SuperUser_Origine AS Piva_SuperUser_Origine_Contab, ")
                SQLSelect.AppendLine(" RisUm_Contab.Settore_Des AS Settore_Des_Contab, RisUm_Contab.Attivita_Des AS Attivita_Des_Contab  ")
            End If

            SQLSelect.AppendLine(", ")
            SQLSelect.AppendLine(" ISNULL((SELECT  SUM(importo) ")
            SQLSelect.AppendLine("         FROM    pagamenti ")
            SQLSelect.AppendLine("         WHERE   movimenti.piva = pagamenti.piva AND movimenti.id_agenda = pagamenti.id_agenda AND movimenti.sa_cod = pagamenti.sa_cod), 0) ")
            SQLSelect.AppendLine("         AS Importo_Pagato ")
            SQLSelect.AppendLine(" ")

            'JOIN AGENDA - MOVIMENTI
            SQLJoin.AppendLine(" FROM    Agenda INNER JOIN Movimenti ")
            SQLJoin.AppendLine(" ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda")

            'JOIN AGENDA - OPERAZIONI
            SQLJoin.AppendLine(" INNER JOIN Operazioni ON Agenda.Lav_Cod = Operazioni.LAV_COD ")

            'JOIN AGENDA - MOVIMENTI CONTAB
            SQLJoin.AppendLine(" INNER JOIN Movimenti Movimenti_Contab ")
            SQLJoin.AppendLine(" ON Agenda.PIVA = Movimenti_Contab.PIVA AND Agenda.Sa_Cod = Movimenti_Contab.Sa_Cod AND Agenda.Id_Agenda = Movimenti_Contab.Id_Agenda")

            'JOIN MOVIMENTI - MOVIMENTI DETTAGLI
            SQLJoin.AppendLine(" INNER JOIN Movimenti_dettagli ")
            SQLJoin.AppendLine(" ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov ")

            'JOIN MOVIMENTI DETTAGLI - IVA ALIQUOTE
            SQLJoin.AppendLine(" INNER JOIN IVA_Aliquote ON IVA_Aliquote.Codice = Movimenti_dettagli.Cod_Iva   ")


            'JOIN MOVIMENTI DETTAGLI - RICXCONTI - CONTI 
            SQLJoin.AppendLine(" LEFT OUTER JOIN  RicXConti ON Movimenti_dettagli.PIVA = RicXConti.Piva AND Movimenti_dettagli.Anno = RicXConti.Anno AND Movimenti_dettagli.Ric_Cod = RicXConti.Ric_Cod ")
            'If Flag_Join_CodConto = True Then
            ''nel caso di filtro su più conti non deve essere messo in join il cod_conto
            SQLJoin.AppendLine(" AND Movimenti_dettagli.Cod_Conto = RicXConti.Cod_Conto ")
            'End If
            SQLJoin.AppendLine(" LEFT OUTER JOIN Conti ON RicXConti.Cod_Conto = Conti.Cod_Conto ")

            If Flag_CostiAccessori_Corrispettivi = True Then
                'JOIN MOVIMENTI DETTAGLI - RISORSE UMANE CORRISPETTIVI
                'non mettere in join la piva, mi raccomando!!!!!!
                SQLJoin.AppendLine(" LEFT OUTER JOIN Risorse_Umane RisUm_Corrisp ON RisUm_Corrisp.Cod_RisUm = Movimenti_dettagli.Mat_Cod ")

                'JOIN RISORSE UMANE - CONTATTI CORRISPETTIVI
                SQLJoin.AppendLine(" LEFT OUTER JOIN Contatti Contatti_Corrisp ON RisUm_Corrisp.Piva = Contatti_Corrisp.Piva AND RisUm_Corrisp.Cod_Contatto = Contatti_Corrisp.Cod_Contatto")
            End If

            If Flag_Contabilita = True Then
                'JOIN MOVIMENTI DETTAGLI - RISORSE UMANE CONTAB
                'non mettere in join la piva, mi raccomando!!!!!!
                SQLJoin.AppendLine(" LEFT OUTER JOIN Risorse_Umane RisUm_Contab ON RisUm_Contab.Cod_RisUm = Movimenti_Contab.Cod_RisUm ")

                'JOIN RISORSE UMANE - CONTATTI CONTAB
                SQLJoin.AppendLine(" LEFT OUTER JOIN Contatti Contatti_Contab ON RisUm_Contab.Piva = Contatti_Contab.Piva AND RisUm_Contab.Cod_Contatto = Contatti_Contab.Cod_Contatto")
            End If

            'JOIN IMPRESE - AGENDA
            SQLJoin.AppendLine(" INNER JOIN Imprese ON Imprese.Piva = Agenda.Piva ")


            If Flag_AncheMagazzino = True Then
                'JOIN MOVIMENTI DETTAGLI - MOVIMENTI DESTINAZIONI
                SQLJoinSiMag.AppendLine(" INNER JOIN Mov_Destinazioni ")
                SQLJoinSiMag.AppendLine(" ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod ")
                SQLJoinSiMag.AppendLine(" AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov ")
                SQLJoinSiMag.AppendLine(" AND Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")

                'JOIN FABBRICATO
                SQLJoinSiMag.AppendLine(" INNER JOIN Fabbricati ON Mov_Destinazioni.Piva = Fabbricati.PIVA AND Mov_Destinazioni.Sa_Cod = Fabbricati.SA_COD AND ")
                SQLJoinSiMag.AppendLine("  Mov_Destinazioni.Id_Destinazione = Fabbricati.Fabbricato_Cod")

                'JOIN TIPO FABBRICATO
                SQLJoinSiMag.AppendLine(" INNER JOIN Fabbricati_Tipi ON Fabbricati.Tipo_Fabbricato_Cod = Fabbricati_Tipi.Tipo_Fabbricato_Cod ")
            End If

            'CONDIZIONI
            SQLWhere.AppendLine(" WHERE   Movimenti.Validita_Inizio <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " ")
            SQLWhere.AppendLine(" AND     Movimenti.Validita_Fine >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")

            SQLWhere.AppendLine(" AND     Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " ")
            SQLWhere.AppendLine(" AND     Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")

            SQLWhere.AppendLine(" AND     Movimenti_Contab.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'   ")

            If Piva <> "" Then
                SQLWhere.AppendLine(" AND Movimenti_Dettagli.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Flag_Contabilita = False Then
                If Sa_Cod <> 0 Then
                    SQLWhere.AppendLine(" AND Movimenti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                End If
            Else
                SQLWhere.AppendLine(" AND Movimenti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Id_Agenda <> 0 Then
                SQLWhere.AppendLine(" AND Movimenti_Dettagli.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
            End If

            If Id_Mov <> 0 Then
                SQLWhere.AppendLine(" AND Movimenti_Dettagli.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
            End If

            If Id_Mov_Det <> 0 Then
                SQLWhere.AppendLine(" AND Movimenti_Dettagli.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")
            End If

            'If Flag_CostiAccessori_Corrispettivi = False Then
            '    'se non mi interessa tirare su la manodopera
            '    'filtro l'elem_cod solo se è valorizzato
            '    If Elem_Cod <> 0 Then
            '         SQLWhere.AppendLine( " AND Movimenti_Dettagli.Elem_Cod = " & SQL_SaveNum(Elem_Cod) & "   ")
            '    End If
            'Else
            '    'mi interessa la manodopera
            '    'devo filtrare elem_cod = 0
            '     SQLWhere.AppendLine( " AND Movimenti_Dettagli.Elem_Cod = " & SQL_SaveNum(Elem_Cod) & "   ")
            'End If

            If Elem_Cod <> 0 Then
                SQLWhere.AppendLine(" AND Movimenti_Dettagli.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
            End If

            If Pro_Cod <> 0 Then
                SQLWhere.AppendLine(" AND Movimenti_Dettagli.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
            End If

            If Mat_Cod <> 0 Then
                'Nel caso di manodopera o corrispettivi
                'mat_cod = cod_risum
                SQLWhere.AppendLine(" AND Movimenti_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
            End If

            If Cal_Cod <> 0 Then
                SQLWhere.AppendLine(" AND Movimenti_Dettagli.Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "   ")
            End If

            If Cod_Progetto <> 0 Then
                SQLWhere.AppendLine(" AND Movimenti_Dettagli.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & "   ")
            End If

            If Fase_Cod <> 0 Then
                SQLWhere.AppendLine(" AND Movimenti_Dettagli.Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   ")
            End If

            If Udm_Cod <> 0 Then
                SQLWhere.AppendLine(" AND Movimenti_Dettagli.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   ")
            End If

            If Lotto <> "" Then
                SQLWhere.AppendLine(" AND Movimenti_Dettagli.Lotto = '" & Agro_SQL_SaveText(Lotto) & "'   ")
            End If

            If Contabilizzato <> 0 Then
                SQLWhere.AppendLine(" AND Movimenti_Dettagli.Contabilizzato = " & Agro_SQL_SaveNum(Contabilizzato) & "   ")
            End If

            If Pendente <> 0 Then
                SQLWhere.AppendLine(" AND Movimenti_Dettagli.Pendente = " & Agro_SQL_SaveNum(Pendente) & "   ")
            End If

            If Ric_Cod <> 0 Then
                SQLWhere.AppendLine(" AND Movimenti_Dettagli.Ric_Cod = " & Agro_SQL_SaveNum(Ric_Cod) & "   ")
            End If

            If Anno <> 0 Then
                SQLWhere.AppendLine(" AND Movimenti_Dettagli.Anno = " & Agro_SQL_SaveNum(Anno) & "   ")
            End If

            If Cod_Conto <> 0 Then
                SQLWhere.AppendLine(" AND Movimenti_Dettagli.Cod_Conto = " & Agro_SQL_SaveNum(Cod_Conto) & "   ")
            End If

            If Cod_RisUm <> 0 Then
                'solo per il caso di documenti contabili, quindi clienti/fornitori
                SQLWhere.AppendLine(" AND Movimenti_Contab.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
            End If

            If Doc_Numero_Sin <> "XYZ" Then
                SQLWhere.AppendLine(" AND Movimenti_Contab.Doc_Numero_Sin = '" & Agro_SQL_SaveText(Doc_Numero_Sin) & "'   ")
            End If

            If Doc_Numero <> 0 Then
                SQLWhere.AppendLine(" AND Movimenti_Contab.Doc_Numero = " & Agro_SQL_SaveNum(Doc_Numero) & "   ")
            End If

            If Doc_Numero_Des <> "XYZ" Then
                SQLWhere.AppendLine(" AND Movimenti_Contab.Doc_Numero_Des = '" & Agro_SQL_SaveText(Doc_Numero_Des) & "'   ")
            End If

            If Scadenza <> "31/12/2100" Then
                SQLWhere.AppendLine(" AND ( ( Movimenti_Contab.Scadenza = " & Agro_SQL_SaveDate(CDate(Scadenza)) & ") OR (  Movimenti_Contab.Scadenza_Extra = " & Agro_SQL_SaveDate(CDate(Scadenza)) & ") )")
            End If

            If Flag_Contabilita = True Then
                If Cod_Contatto <> "" Then
                    SQLWhere.AppendLine(" AND Contatti_Contab.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "'   ")
                End If
                If Piva_Contatto <> "" Then
                    SQLWhere.AppendLine(" AND Contatti_Contab.Piva = '" & Agro_SQL_SaveText(Piva_Contatto) & "'   ")
                End If
                'gias2gias
                If Cod_RisUm_Origine <> 0 Then
                    SQLWhere.AppendLine(" AND  RisUm_Contab.Cod_RisUm_Origine = " & Agro_SQL_SaveNum(Cod_RisUm_Origine) & "   ")
                End If
                If Piva_SuperUser_Origine <> "" Then
                    SQLWhere.AppendLine(" AND  RisUm_Contab.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'")
                End If
                If Flag_AncheImportati = False Then
                    SQLWhere.AppendLine(" AND  RisUm_Contab.Cod_RisUm_Origine = " & Agro_SQL_SaveNum(0) & " ")
                End If
                'fine gias2gias

            ElseIf Flag_CostiAccessori_Corrispettivi = True Then
                If Cod_Contatto <> "" Then
                    SQLWhere.AppendLine(" AND Contatti_Corrisp.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "'   ")
                End If
                If Piva_Contatto <> "" Then
                    SQLWhere.AppendLine(" AND Contatti_Corrisp.Piva = '" & Agro_SQL_SaveText(Piva_Contatto) & "'   ")
                End If
                'gias2gias
                If Cod_RisUm_Origine <> 0 Then
                    SQLWhere.AppendLine(" AND  RisUm_Corrisp.Cod_RisUm_Origine = " & Agro_SQL_SaveNum(Cod_RisUm_Origine) & "   ")
                End If
                If Piva_SuperUser_Origine <> "" Then
                    SQLWhere.AppendLine(" AND  RisUm_Corrisp.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'")
                End If
                If Flag_AncheImportati = False Then
                    SQLWhere.AppendLine(" AND  RisUm_Corrisp.Cod_RisUm_Origine = " & Agro_SQL_SaveNum(0) & " ")
                End If
                'fine gias2gias
            End If

            If Cau_Mov <> "" Then
                SQLWhere.AppendLine(" AND Movimenti.Cau_Mov = '" & Agro_SQL_SaveText(Cau_Mov) & "'   ")
            End If

            If FiltroAggiuntivo <> "" Then
                SQLWhere.AppendLine(Agro_SQL_Save_xFiltroAggiuntivo(FiltroAggiuntivo, , objParametri))
            End If

            If Ordinamento <> "" Then
                SQLOrderBy.AppendLine(Ordinamento)
            Else
                SQLOrderBy.AppendLine(" ORDER BY Agenda.PIVA, Movimenti.Data_Movimento DESC ")
            End If

            '====================================

            SQLGenerale.AppendLine(SQLSelect.ToString)
            If Flag_AncheMagazzino = True Then
                SQLGenerale.AppendLine(SQLSelectSiMag.ToString)
            Else
                'SQLSelectNoMag
            End If
            SQLGenerale.AppendLine(SQLJoin.ToString)
            If Flag_AncheMagazzino = True Then
                SQLGenerale.AppendLine(SQLJoinSiMag.ToString)
            End If

            SQLGenerale.AppendLine(SQLWhere.ToString)

            '--------------------------------------------------
            '--- Recupero il datatable --------------------------
            '----------------------------------------------------

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, SQLGenerale.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    '################################################################################
    Public Function IdAgenda_Documento(ByVal Piva As String,
                                       ByVal Lav_Cod As Integer,
                                       ByVal Anno As Integer,
                                       ByVal Data As Date,
                                       ByVal Doc_Numero_Sin As String,
                                       ByVal Doc_Numero As Integer,
                                       ByVal Doc_Numero_Des As String,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                       ) As Integer

        Dim dt As DataTable
        Dim objMovimenti As New AgronicaCoreContabDAL.Movimenti_R
        Dim idAgenda As Integer = 0

        dt = objMovimenti.MovimentiContabili(Lav_Cod,
                                             Piva,
                                             0,
                                             0,
                                             0,
                                             0,
                                             CAU_REGISTRAZIONI,
                                             Anno,
                                             Data,
                                             AGRODATAINIZIO,
                                             Doc_Numero_Sin,
                                             Doc_Numero,
                                             Doc_Numero_Des,
                                             0, 0, AGRODATAINIZIO,
                                             "", "",
                                             objParametri)

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            idAgenda = dt.Rows(0).Item("Id_agenda")
        End If

        Return idAgenda

    End Function

    '################################################################################
    Public Function LeggiDatiMinimi_MovimentiCarico(ByVal Flag_LeggiMovCont As Boolean,
                                                    ByVal Flag_LeggiMovMag As Boolean,
                                                    ByVal Piva As String,
                                                    ByVal Lav_Cod As Integer,
                                                    ByVal Data_Da As Date,
                                                    ByVal Data_A As Date,
                                                    ByVal Filtro_Agg_MovCont As String,
                                                    ByVal Filtro_Agg_MovMag As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                    ) As DataTable

        Dim Dt_MovCont As DataTable
        Dim Dt_Carichi As DataTable
        Dim objMovimenti As New AgronicaCoreContabDAL.Mov_Destinazioni_R
        Dim Dt_output As New DataTable
        'Dim Filtro_Agg As String
        Dim Dr_output As DataRow
        Dim i As Integer
        'Dim TEMP_piva As String = ""
        Dim TEMP_id_agenda As Integer = 0
        Dim piva_dt As String
        Dim idAgenda As Integer

        Dt_output.Columns.Add(New DataColumn("piva", GetType(String)))
        Dt_output.Columns.Add(New DataColumn("des_lib", GetType(String)))
        Dt_output.Columns.Add(New DataColumn("data_movimento", GetType(String)))
        Dt_output.Columns.Add(New DataColumn("sa_cod", GetType(Integer)))
        Dt_output.Columns.Add(New DataColumn("id_agenda", GetType(Integer)))

        If Flag_LeggiMovCont = True Then

            If Filtro_Agg_MovCont <> "" Then
                Filtro_Agg_MovCont &= " AND "
            End If
            Filtro_Agg_MovCont &= "  ( Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Da)
            Filtro_Agg_MovCont &= " AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Data_A)
            Filtro_Agg_MovCont &= " ) "

            'Dt_MovCont = objMovimenti.MovimentiContabili(Lav_Cod, _
            '                        Piva, _
            '                        0, _
            '                        0, _
            '                        0, _
            '                        0, _
            '                        "", _
            '                        0, _
            '                          AGRODATAINIZIO, _
            '                         AGRODATAINIZIO, _
            '                         "XYZ", _
            '                        0, _
            '                        "XYZ", _
            '                        0, 0, AGRODATAINIZIO, _
            '                        Filtro_Agg_MovCont, _
            '                        " Agenda.Piva, Agenda.Id_Agenda ", _
            '                        objParametri)

            Dt_MovCont = objMovimenti.MovimentiContabili(Lav_Cod,
                                                         Piva,
                                                         0,
                                                         0,
                                                         0,
                                                         0,
                                                         0,
                                                         "",
                                                         0,
                                                         AGRODATAINIZIO,
                                                         AGRODATAINIZIO,
                                                         "XYZ",
                                                         0,
                                                         "XYZ",
                                                         0, 0, AGRODATAINIZIO,
                                                         Filtro_Agg_MovCont,
                                                         " Agenda.Piva, Agenda.Id_Agenda ",
                                                         objParametri)

            If Not IsNothing(Dt_MovCont) AndAlso Dt_MovCont.Rows.Count > 0 Then

                For i = 0 To Dt_MovCont.Rows.Count - 1

                    piva_dt = Dt_MovCont.Rows(i).Item("piva")
                    idAgenda = Dt_MovCont.Rows(i).Item("id_agenda")

                    If TEMP_id_agenda <> idAgenda Then

                        TEMP_id_agenda = idAgenda

                        Dr_output = Dt_output.NewRow

                        Dr_output.Item("piva") = piva_dt
                        Dr_output.Item("des_lib") = Dt_MovCont.Rows(i).Item("des_lib")
                        Dr_output.Item("data_movimento") = Dt_MovCont.Rows(i).Item("data_movimento")
                        Dr_output.Item("sa_cod") = Dt_MovCont.Rows(i).Item("sa_cod")
                        Dr_output.Item("id_agenda") = idAgenda

                        Dt_output.Rows.Add(Dr_output)

                    End If

                Next

            End If

        End If

        TEMP_id_agenda = 0

        If Flag_LeggiMovMag = True Then

            If Filtro_Agg_MovMag <> "" Then
                Filtro_Agg_MovMag &= " AND "
            End If
            Filtro_Agg_MovMag &= "  ( Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Da)
            Filtro_Agg_MovMag &= " AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Data_A)
            Filtro_Agg_MovMag &= " ) "

            'carichi di magazzino
            Dt_Carichi = objMovimenti.CarichiScarichi(Lav_Cod,
                                                      Piva,
                                                      0,
                                                      0, 0, 0,
                                                      CAU_CARICO,
                                                      AGRODATAINIZIO,
                                                      Filtro_Agg_MovMag,
                                                      " Agenda.Piva, Agenda.Id_Agenda ",
                                                      objParametri)

            If Not IsNothing(Dt_Carichi) AndAlso Dt_Carichi.Rows.Count > 0 Then

                For i = 0 To Dt_Carichi.Rows.Count - 1

                    piva_dt = Dt_Carichi.Rows(i).Item("piva")
                    idAgenda = Dt_Carichi.Rows(i).Item("id_agenda")

                    If TEMP_id_agenda <> idAgenda Then

                        TEMP_id_agenda = idAgenda

                        Dr_output = Dt_output.NewRow

                        Dr_output.Item("piva") = piva_dt
                        Dr_output.Item("des_lib") = Dt_Carichi.Rows(i).Item("des_lib")
                        Dr_output.Item("sa_cod") = Dt_Carichi.Rows(i).Item("sa_cod")
                        Dr_output.Item("data_movimento") = Dt_Carichi.Rows(i).Item("data_movimento")
                        Dr_output.Item("id_agenda") = idAgenda

                        Dt_output.Rows.Add(Dr_output)

                    End If

                Next

            End If

        End If

        Return Dt_output


    End Function

    '################################################################################
    Public Sub RecuperaDati_ImpresaFornitore(ByVal Piva_Fornitore As String,
                                             ByRef Rag_Soc As String,
                                             ByRef Cod_Risum As Integer,
                                             ByRef Cod_IndirizzoRisum As Integer,
                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                             )

        Dim dt As DataTable
        Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R
        Dim objImp As New AgronicaCoreAnagrafeDAL.Imprese_Read
        Dim IsImpresaGias As Boolean

        Rag_Soc = ""
        Cod_Risum = 0
        Cod_IndirizzoRisum = 0


        IsImpresaGias = objImp.VerificaEsistenza_PivaGIAS(Piva_Fornitore, objParametri)

        If IsImpresaGias = False Then
            'è solo contatto
            'non va bene, perchè vengono letti 2 record e tanto si prende il primo il codice
            'Dim FiltroAggiuntivo As String = " ( ContattiXIndirizzi.Tipo_Indirizzo IN (" & Agro_SQL_SaveText(enum_IndirizzoTipo.Residenza) & "," & Agro_SQL_SaveText(enum_IndirizzoTipo.SedeLegale) & " ) " & _
            '                                " OR ContattiXIndirizzi.Tipo_Indirizzo IN (" & Agro_SQL_SaveText(enum_IndirizzoTipo.Domicilio) & "," & Agro_SQL_SaveText(enum_IndirizzoTipo.SedeOperativa) & " ) " & _
            '                                " ) "
            Dim FiltroAggiuntivo As String = " ( ContattiXIndirizzi.Tipo_Indirizzo IN (" & enum_IndirizzoTipo.Residenza & "," & enum_IndirizzoTipo.SedeLegale & " )  ) "

            dt = objContatti.Contatti_Contatto_Leggi("",
                                                     Piva_Fornitore,
                                                     0, 0,
                                                     False,
                                                     True, 0, 0,
                                                     False, 0,
                                                     ID_CF_NOFILTRO,
                                                     0, "", True,
                                                     0, 1, 0, 0, 0,
                                                     enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                     FiltroAggiuntivo,
                                                     "",
                                                     objParametri)

        Else
            'è anche impresa gias

            'sarebbe meglio aggiungere al filtro anche AND contatti.sa_cod = " & CStr(PUBBLICO)
            'però non lo faccio perchè non so se da altre parti poi è un problema
            'nella query ho aggiunto allora l'order by ASC sul sa_cod
            Dim FiltroAggiuntivo As String = " Rapporti_Contabili.Fornitore = 1  "

            dt = objContatti.ImpresaGias_Contatto_Leggi(Piva_Fornitore,
                                                        0,
                                                          FiltroAggiuntivo,
                                                        "",
                                                        objParametri)

        End If

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            Rag_Soc = dt.Rows(0).Item("Rag_Soc")
            Cod_Risum = dt.Rows(0).Item("Cod_Risum")
            Cod_IndirizzoRisum = dt.Rows(0).Item("Cod_Indirizzo")
        End If


    End Sub

    '################################################################################
    Public Sub RecuperaDati_ContattoCliente(ByVal Cod_Contatto_Cliente As String,
                                            ByRef Rag_Soc As String,
                                            ByRef Cod_Risum As Integer,
                                            ByRef Cod_IndirizzoRisum As Integer,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim dt As DataTable
        Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R
        Dim objImp As New AgronicaCoreAnagrafeDAL.Imprese_Read
        Dim IsImpresaGias As Boolean

        Rag_Soc = ""
        Cod_Risum = 0
        Cod_IndirizzoRisum = 0


        IsImpresaGias = objImp.VerificaEsistenza_PivaGIAS(Cod_Contatto_Cliente, objParametri)

        If IsImpresaGias = False Then
            'è solo contatto
            'non va bene, perchè vengono letti 2 record e tanto si prende il primo il codice
            'Dim FiltroAggiuntivo As String = " ( ContattiXIndirizzi.Tipo_Indirizzo IN (" & Agro_SQL_SaveText(enum_IndirizzoTipo.Residenza) & "," & Agro_SQL_SaveText(enum_IndirizzoTipo.SedeLegale) & " ) " & _
            '                                " OR ContattiXIndirizzi.Tipo_Indirizzo IN (" & Agro_SQL_SaveText(enum_IndirizzoTipo.Domicilio) & "," & Agro_SQL_SaveText(enum_IndirizzoTipo.SedeOperativa) & " ) " & _
            '                                " ) "
            Dim FiltroAggiuntivo As String = " ( ContattiXIndirizzi.Tipo_Indirizzo IN (" & enum_IndirizzoTipo.Residenza & "," & enum_IndirizzoTipo.SedeLegale & " )  ) "

            dt = objContatti.Contatti_Contatto_Leggi("",
                                                     Cod_Contatto_Cliente,
                                                     0, 0,
                                                     False,
                                                     True, 0, 0,
                                                     False, 0,
                                                     ID_CF_NOFILTRO,
                                                     0, "", True,
                                                     1, 0, 0, 0, 0,
                                                     enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                     FiltroAggiuntivo,
                                                     "",
                                                     objParametri)

        Else
            'è anche impresa gias
            Dim FiltroAggiuntivo As String = " Rapporti_Contabili.Cliente = 1 "

            dt = objContatti.ImpresaGias_Contatto_Leggi(Cod_Contatto_Cliente,
                                                        0,
                                                        FiltroAggiuntivo,
                                                        "",
                                                        objParametri)

        End If

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            Rag_Soc = dt.Rows(0).Item("Rag_Soc") & dt.Rows(0).Item("Cognome") & dt.Rows(0).Item("Nome")
            Cod_Risum = dt.Rows(0).Item("Cod_Risum")
            Cod_IndirizzoRisum = dt.Rows(0).Item("Cod_Indirizzo")
        End If


    End Sub

    '################################################################################
    'passa anche il codice_fornitore (settore_des)
    Public Sub RecuperaDati_ImpresaFornitore2(ByVal Piva_Fornitore As String,
                                              ByVal Codice_Fornitore As String,
                                                ByRef Rag_Soc As String,
                                                ByRef Cod_Risum As Integer,
                                                ByRef Cod_IndirizzoRisum As Integer,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                )

        Dim Dt As DataTable
        Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R
        Dim objImp As New AgronicaCoreAnagrafeDAL.Imprese_Read
        Dim IsImpresaGias As Boolean

        Rag_Soc = ""
        Cod_Risum = 0
        Cod_IndirizzoRisum = 0


        IsImpresaGias = objImp.VerificaEsistenza_PivaGIAS(Piva_Fornitore, objParametri)

        If IsImpresaGias = False Then
            'è solo contatto
            'non va bene, perchè vengono letti 2 record e tanto si prende il primo il codice
            'Dim FiltroAggiuntivo As String = " ( ContattiXIndirizzi.Tipo_Indirizzo IN (" & Agro_SQL_SaveText(enum_IndirizzoTipo.Residenza) & "," & Agro_SQL_SaveText(enum_IndirizzoTipo.SedeLegale) & " ) " & _
            '                                " OR ContattiXIndirizzi.Tipo_Indirizzo IN (" & Agro_SQL_SaveText(enum_IndirizzoTipo.Domicilio) & "," & Agro_SQL_SaveText(enum_IndirizzoTipo.SedeOperativa) & " ) " & _
            '                                " ) "
            Dim FiltroAggiuntivo As String = " ( ContattiXIndirizzi.Tipo_Indirizzo IN (" & enum_IndirizzoTipo.Residenza & "," & enum_IndirizzoTipo.SedeLegale & " )  ) "
            If Codice_Fornitore <> "" Then
                FiltroAggiuntivo &= " AND UPPER(Risorse_Umane.Settore_Des) = '" & CStr(Agro_SQL_SaveText(Codice_Fornitore)).ToUpper & "'"
            End If

            Dt = objContatti.Contatti_Contatto_Leggi("",
                                                     Piva_Fornitore,
                                                     0, 0,
                                                     False,
                                                     True, 0, 0,
                                                     False, 0,
                                                     ID_CF_NOFILTRO,
                                                     0, "", True,
                                                     0, 1, 0, 0, 0,
                                                     enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                     FiltroAggiuntivo,
                                                     "",
                                                     objParametri)

        Else
            'è anche impresa gias
            Dim FiltroAggiuntivo As String = " Rapporti_Contabili.Fornitore = 1 "
            If Codice_Fornitore <> "" Then
                FiltroAggiuntivo &= " AND UPPER(Risorse_Umane.Settore_Des) = '" & CStr(Agro_SQL_SaveText(Codice_Fornitore)).ToUpper & "'"
            End If

            Dt = objContatti.ImpresaGias_Contatto_Leggi(Piva_Fornitore,
                                                        0,
                                                          FiltroAggiuntivo,
                                                        "",
                                                        objParametri)

        End If

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count > 0 Then
            Rag_Soc = Dt.Rows(0).Item("Rag_Soc")
            Cod_Risum = Dt.Rows(0).Item("Cod_Risum")
            Cod_IndirizzoRisum = Dt.Rows(0).Item("Cod_Indirizzo")
        End If


    End Sub


    '################################################################################
    Public Sub DatiSuperUserFornitore(ByRef Rag_Soc As String,
                                        ByRef Cod_Risum As Integer,
                                        ByRef Cod_IndirizzoRisum As Integer,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        )

        Dim Dt As DataTable
        Dim objContattiSU As New AgronicaCoreAnagrafeDAL.Contatti_R

        Rag_Soc = ""
        Cod_Risum = 0
        Cod_IndirizzoRisum = 0

        Dt = objContattiSU.SuperUser_Contatto_Leggi(COD_FORNITORE,
                                                    "", "",
                                                    objParametri)

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count > 0 Then
            Rag_Soc = Dt.Rows(0).Item("Rag_Soc")
            Cod_Risum = Dt.Rows(0).Item("Cod_Risum")
            Cod_IndirizzoRisum = Dt.Rows(0).Item("Cod_Indirizzo")
        End If


    End Sub



    '################################################################################
    Public Function Componi_Descrizione_MateriaPrima(ByVal Piva As String,
                                                    ByVal Elem_Cod As Integer,
                                                    ByVal Mat_Cod As Integer,
                                                    ByVal Cod_Progetto As Integer,
                                                    ByVal Fase_Cod As Integer,
                                                    ByVal Lotto As String,
                                                    ByVal Cal_Cod As Integer,
                                                    ByVal Flag_VisualizzaCategoria As Boolean,
                                                    ByVal Data_Inizio As Date,
                                                    ByVal Data_Fine As Date,
                                                    ByVal Flag_ModalitaAccettazione As Boolean,
                                                    ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                    ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                    Optional ByVal moduliCliente As List(Of Integer) = Nothing
                                                    ) As String


        Dim Nome_Prodotto As String = ""
        Dim Cod_Articolo As String = ""
        Dim Nome_Categoria As String = ""
        Dim Nome_Calibro As String
        Dim Peso_Set As Integer
        Dim Dt_Materie_Prime As DataTable
        Dim i As Integer
        Dim objMP As New AgronicaCoreAnagrafeDAL.Materie_Prime_R

        'Leggo l'anagrafica materie prime
        Dt_Materie_Prime = objMP.MateriePrime_Anagrafica(Piva,
                                                            0,
                                                            Elem_Cod,
                                                            Mat_Cod,
                                                             "",
                                                             0,
                                                            0,
                                                            0,
                                                             0,
                                                             0,
                                                             0,
                                                            0,
                                                            "",
                                                            0,
                                                            "",
                                                            True,
                                                            "", "",
                                                            objParametri_Server,
                                                            objParametri_Utenti)


        If Not IsNothing(Dt_Materie_Prime) Then

            If Dt_Materie_Prime.Rows.Count <> 0 Then

                Peso_Set = Dt_Materie_Prime.Rows(i).Item("Peso_Set")
                Cod_Articolo = " (Cod. " & CStr(Dt_Materie_Prime.Rows(i).Item("Cod_Articolo")) & ")"
                Nome_Prodotto = CStr(Dt_Materie_Prime.Rows(i).Item("Mat_Des"))

                'modifica del 25/11/2011: il cod_articolo viene aggiunto se impostato nelle impostazioni utente
                'Select Case Elem_Cod
                '    Case SEMILAVORATI_VEGETALI, SEMILAVORATI_ANIMALI, SEMENTI, ALTRE_MATERIE
                '        Nome_Prodotto = CStr(Dt_Materie_Prime.Rows(i).Item("Mat_Des")) & _
                '        " (Cod.Articolo: " & CStr(Dt_Materie_Prime.Rows(i).Item("Cod_Articolo")) & ")"
                '    Case Else
                '        Nome_Prodotto = CStr(Dt_Materie_Prime.Rows(i).Item("Mat_Des"))
                'End Select

                'Lotto Interno
                If Cod_Progetto <> 0 Then

                    Select Case Elem_Cod

                        Case SEMILAVORATI_ANIMALI, MATERIE_ANIMALI, BENI_CONFEZ_ANIMALE, TRASFORMATI_ANIMALI

                            Dim RSAnimaleAnagrafe As New DataTable
                            Dim Matricola As String

                            Dim zoo As New AgronicaCoreZooDAL.Zoo_Animali_R
                            RSAnimaleAnagrafe = zoo.Leggi(
                                        Piva,
                                        0,
                                        Cod_Progetto,
                                        "",
                                        0,
                                        0,
                                        0,
                                        0,
                                        "",
                                        "",
                                        enumSelezioneVariabile.Selezione_TabellaCompleta,
                                        "",
                                        "",
                                        objParametri_Server
                                        )

                            If Not IsNothing(RSAnimaleAnagrafe) AndAlso RSAnimaleAnagrafe.Rows.Count <> 0 Then
                                Matricola = CStr(RSAnimaleAnagrafe.Rows(0).Item("matricola"))

                                If Not IsNothing(Matricola) And Matricola <> "" Then
                                    Nome_Prodotto &= " - Matricola: " & Matricola
                                End If

                            End If


                        Case SEMILAVORATI_VEGETALI, MATERIE_VEGETALI, BENI_CONFEZ_VEGETALE, TRASFORMATI_VEGETALI

                            Dim objImpProg As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
                            Nome_Prodotto &= " - Lotto Impianto: " & objImpProg.ProgettoNome_From_ProgettoCod_2(
                                                                    Piva,
                                                                    Cod_Progetto,
                                                                    CAU_PROGETTO_PRODUZIONE,
                                                                    objParametri_Server)

                    End Select

                End If

                '----------------------------------
                'GESTIONE LOTTO PRODOTTI
                Dim DettagliLotto As String = ""
                Dim objLotto As New AgronicaCoreAnagrafeDAL.Materie_PrimexLC_R

                If moduliCliente Is Nothing Then
                    'Leggo i moduli installati
                    Dim objO As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R
                    moduliCliente = objO.Recupera_Moduli_Cliente(Piva, objParametri_Server)
                End If

                DettagliLotto = objLotto.Gestione_LottoProdotto(Piva, Elem_Cod, Mat_Cod, Lotto, moduliCliente, objParametri_Server)

                If DettagliLotto <> "" Then
                    Nome_Prodotto &= " " & DettagliLotto
                End If
                '----------------------------------


                'Calibro
                If Cal_Cod <> 0 Then

                    Dim Cal_Cod_Calibro As Integer = 0
                    Dim Codice_Indice As Integer = 0
                    Dim Codice_Danno As Integer = 0
                    Dim Valore_Indice As String = ""
                    Dim UdmCod_Indice As Integer = 0
                    Dim Desc_Calibro As String = ""
                    Dim Desc_Indice As String = ""
                    Dim Desc_Danno As String = ""

                    Nome_Calibro = Leggi_CampionaturaRaccolto(Cal_Cod_Calibro,
                                                            Desc_Calibro,
                                                            Codice_Indice,
                                                            Desc_Indice,
                                                            Codice_Danno,
                                                            Desc_Danno,
                                                            Valore_Indice,
                                                            UdmCod_Indice,
                                                            Cal_Cod,
                                                            "", 0, 0, 0, "", True,
                                                            objParametri_Server)

                    'se sono in modalità normale, aggiungo il calibro alla descrizione
                    If Flag_ModalitaAccettazione = False Then

                        If Nome_Calibro.ToLower <> "indefinito" And Nome_Calibro <> "" Then
                            Nome_Prodotto &= " - Campionatura: " & Nome_Calibro
                        End If

                    Else
                        'modalità accettazione, non aggiungo il calibro alla descrizione
                    End If

                End If 'cal_cod

            Else
                '0 materie prime
                Return ""
            End If
        Else
            'Dt_Materie_Prime nothing
            Return ""
        End If

        Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim Flag_VisualCodArticolo As String
        Flag_VisualCodArticolo = objUtenti.ImpostazioneValore1_from_ImpostazioneCod(
                                            enum_Impostazioni_Utenti.SUPERUSER_COD_VISUAL_CODARTICOLO_DOCUMENTI,
                                            objParametri_Utenti,
                                            2)

        If Flag_VisualCodArticolo = "1" Then
            Nome_Prodotto &= Cod_Articolo
        End If

        If Flag_VisualizzaCategoria = True Then
            Return Nome_Categoria & ": " & Nome_Prodotto
        Else
            Return Nome_Prodotto
        End If

    End Function

    '################################################################################
    'default:
    'Optional ByVal Tipo As String = "", _
    'Optional ByVal Tipo_Cod As Long = 0, _
    'Optional ByVal Udm_Cod As Long = 0, _
    'Optional ByVal Progressivo_Origine As Integer = 0, _
    'Optional ByVal Piva_SuperUser_Origine As String = "", _
    'Optional ByVal Flag_AncheImportati As Boolean = False
    Public Function Leggi_CampionaturaRaccolto(ByRef Codice_Calibro As Integer,
                                               ByRef Desc_Calibro As String,
                                               ByRef Codice_Indice As Integer,
                                               ByRef Desc_Indice As String,
                                               ByRef Codice_Danno As Integer,
                                               ByRef Desc_Danno As String,
                                               ByRef Valore_Indice As String,
                                               ByRef UdmCod_Indice As Integer,
                                               ByVal Codice As Integer,
                                               ByVal Tipo As String,
                                               ByVal Tipo_Cod As Long,
                                               ByVal Udm_Cod As Long,
                                               ByVal Progressivo_Origine As Integer,
                                               ByVal Piva_SuperUser_Origine As String,
                                               ByVal Flag_AncheImportati As Boolean,
                                               ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
                                               ) As String

        Dim messaggioErrore As String = ""
        Dim Campionatura_Des As String = ""
        Dim Str_CampionaturaRaccolto As String = ""
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Contabilita_R.Leggi_CampionaturaRaccolto()"

        Try

            Dim objCalibri As New AgronicaCoreMetaSchemaDAL.CalibriFrutti_R
            Dim objMPCamp As New AgronicaCoreContabDAL.Materie_Prime_Campionature_R


            Select Case Codice

                Case Is > 0 'cal_cod

                    'Calibro
                    'Str_CampionaturaRaccolto = Leggi_CalibroFrutto(objServer, objSession, objPage, Codice, , , , )
                    Str_CampionaturaRaccolto = objCalibri.CalDes_from_CalCod(Codice, objParametri_Server)

                    Codice_Calibro = Codice
                    Codice_Indice = 0
                    Codice_Danno = 0
                    Valore_Indice = ""
                    UdmCod_Indice = 0


                Case Is <= 0 ' progressivo

                    'Campionatura

                    Str_CampionaturaRaccolto = objMPCamp.Leggi_MateriaPrima_Campionature(Codice_Calibro,
                                                                                        Desc_Calibro,
                                                                                        Codice_Indice,
                                                                                        Desc_Indice,
                                                                                        Codice_Danno,
                                                                                        Desc_Danno,
                                                                                        Valore_Indice,
                                                                                        UdmCod_Indice,
                                                                                        Codice,
                                                                                        Tipo,
                                                                                        Tipo_Cod,
                                                                                        Udm_Cod,
                                                                                        Progressivo_Origine,
                                                                                        Piva_SuperUser_Origine,
                                                                                        Flag_AncheImportati,
                                                                                        "", "",
                                                                                         objParametri_Server)


            End Select

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try


        Return Str_CampionaturaRaccolto


    End Function



    ''################################################################################
    '===========================================
    ' Decodifica della descrizione del prodotto 
    '-------------------------------------------
    Public Function LeggiProdotto(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                  ByRef Nome_Categoria As String,
                                  ByVal Piva As String,
                                  ByVal Elem_Cod As Integer,
                                  ByVal Pro_Cod As Integer,
                                  ByVal Mat_Cod As Integer,
                                  Optional ByVal Cod_Progetto As Integer = 0,
                                  Optional ByVal Fase_Cod As Integer = 0,
                                  Optional ByVal Lotto As String = "",
                                  Optional ByVal Cal_Cod As Integer = 0,
                                  Optional ByVal Flag_VisualizzaCategoria As Boolean = False,
                                  Optional ByVal FinestraTemp_Inizio As Date = AGRODATAINIZIO,
                                  Optional ByVal FinestraTemp_Fine As Date = AGRODATAFINE,
                                  Optional ByVal Flag_FiltraFormulati As Boolean = True
                                  ) As String


        Dim RsProdotto As DataTable
        'Dim Tipo_Des As String = ""
        Dim Nome_Prodotto As String = ""

        '=======================================================
        'Decodifica del tipo prodotto e lettura della descrizione
        '-------------------------------------------------------

        Select Case Elem_Cod

            Case MACCHINE '--------------------- Parco Macchine

                Dim RsMacchina As DataTable

                Dim ParcoMacchine As New AgronicaCoreContabDAL.Parco_Macchine_R

                RsProdotto = ParcoMacchine.Leggi(
                                            CStr(Piva),
                                            CInt(Mat_Cod),
                                            False,
                                           "",
                                           "",
                                           "",
                                           "",
                                           "",
                                           0,
                                           "",
                                           False,
                                           0,
                                           "",
                                           False,
                                           FinestraTemp_Inizio,
                                           FinestraTemp_Fine,
                                           "",
                                           "",
                                           objParametri
                                          )

                If Not IsNothing(RsProdotto) AndAlso RsProdotto.Rows.Count <> 0 Then

                    Nome_Prodotto = RsProdotto.Rows(0).Item("CLASS_DESC")
                Else
                    'Eccezione
                    Return ""

                End If

                RsMacchina = Nothing

                '================================================================================


            Case ZOO_CONSISTENZA '-------------- ZOO ANIMALI

                If Cod_Progetto <> 0 Then

                    Dim RsConsistenza As DataTable

                    'Lettura Consistenza
                    'RsConsistenza = LeggiAnimale_Anagrafe(objServer, objSession, objPage, Piva, Cod_Progetto)
                    Dim zoo As New AgronicaCoreZooDAL.Zoo_Animali_R
                    RsConsistenza = zoo.Leggi(Piva,
                                              0,
                                              Cod_Progetto,
                                              "",
                                              0,
                                              0,
                                              0,
                                              0,
                                              "",
                                              "",
                                              enumSelezioneVariabile.Selezione_TabellaCompleta,
                                              "",
                                              "",
                                              objParametri
                                              )

                    If Not IsNothing(RsConsistenza) AndAlso RsConsistenza.Rows.Count <> 0 Then

                        Dim RsIPro As DataTable
                        Dim zooprod As New AgronicaCoreZooDAL.Lista_Indprod_Animali_R
                        RsIPro = zooprod.Leggi(CLng(RsConsistenza.Rows(0).Item("Gen_Cod")),
                                               CLng(RsConsistenza.Rows(0).Item("Spe_Cod")),
                                               CLng(RsConsistenza.Rows(0).Item("Ipro_Cod")),
                                               enumSelezioneVariabile.Selezione_TabellaCompleta,
                                               "",
                                               "", objParametri)


                        If Not IsNothing(RsIPro) AndAlso RsIPro.Rows.Count <> 0 Then

                            Nome_Prodotto = "Consistenze Zootecniche: " & RsIPro.Rows(0).Item("IPro_Des") & " - Matricola: " & RsConsistenza.Rows(0).Item("Matricola")

                        End If

                    End If

                End If

                '================================================================================

            Case Else '-------------------- Altro Tipo di Prodotto


                'Dim objCategorie As Object
                Dim nomeTabella, nomeCodice, nomeDescrizione As String

                Dim dtCategorie As DataTable
                Dim dtProdotti As DataTable
                Dim dtMateriePrime As DataTable
                Dim i, j As Integer

                'legge le categorie di magazzino
                Dim leggiCategorieMagazzino As New AgronicaCoreMetaSchemaDAL.Categorie_Magazzino_R
                dtCategorie = leggiCategorieMagazzino.Leggi(Elem_Cod,
                                                            CAU_MAGAZZINO,
                                                            False,
                                                            "",
                                                            "",
                                                            objParametri)

                If Not IsNothing(dtCategorie) Then

                    If dtCategorie.Rows.Count <> 0 Then

                        nomeTabella = dtCategorie.Rows(i).Item("Tabella")
                        nomeCodice = dtCategorie.Rows(i).Item("Tabella_Cod")
                        nomeDescrizione = dtCategorie.Rows(i).Item("Tabella_Des")
                        Nome_Categoria = dtCategorie.Rows(i).Item("NomeComune")


                        '=================================================================
                        'Verifico se il prodotto è Generale o Aziendale
                        '-----------------------------------------------------------------
                        If Pro_Cod <> 0 Then

                            'Leggo i prodotti con pro_cod

                            'Dt_Prodotti = NewCom_LeggiTabella_da_CategorieMagazzino(objParametri, objSession, objPage, _
                            '                                                        NomeTabella, _
                            '                                                        NomeCodice, _
                            '                                                        NomeDescrizione, _
                            '                                                        "", _
                            '                                                        Pro_Cod, _
                            '                                                        "")

                            dtProdotti = leggiCategorieMagazzino.LeggiTabella_da_CategorieMagazzino(nomeTabella,
                                                                                                     nomeCodice,
                                                                                                     nomeDescrizione,
                                                                                                     "",
                                                                                                     Pro_Cod,
                                                                                                     "", "", objParametri)

                            If Not IsNothing(dtProdotti) Then

                                '(03/08/2015) commentato perchè i dati delle revoca etc non sono piu in locale
                                'Dim objFiltraFormulati As New AgronicaCoreMetaSchemaDAL.Formulati_R
                                'If Elem_Cod = FORMULATI And Flag_FiltraFormulati = True Then
                                '    Dt_Prodotti = objFiltraFormulati.Filtra_Formulati(Dt_Prodotti, CDate(FinestraTemp_Fine), objParametri)
                                'End If

                                If Not IsNothing(dtProdotti) Then
                                    If dtProdotti.Rows.Count <> 0 Then
                                        Nome_Prodotto = dtProdotti.Rows(j).Item(nomeDescrizione)
                                    Else
                                        '0 prodotti
                                        Return ""
                                    End If
                                Else
                                    'dt prodotti nothing
                                    Return ""
                                End If
                            Else
                                'dt prodotti nothing
                                Return ""
                            End If

                        Else ' Mat_Cod <> 0

                            Dim materiePrimeLetti As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
                            dtMateriePrime = materiePrimeLetti.Leggi(Piva,
                                                                     0,
                                                                     Elem_Cod,
                                                                     Mat_Cod,
                                                                     "",
                                                                     0,
                                                                     0,
                                                                     0,
                                                                     0,
                                                                     0,
                                                                     0,
                                                                     0,
                                                                     "",
                                                                     0,
                                                                     "",
                                                                     False,
                                                                     False,
                                                                     "",
                                                                     enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                     "",
                                                                     "",
                                                                     objParametri)


                            If Not IsNothing(dtMateriePrime) Then

                                If dtMateriePrime.Rows.Count <> 0 Then

                                    Select Case Elem_Cod
                                        Case SEMILAVORATI_VEGETALI, SEMILAVORATI_ANIMALI, SEMENTI, ALTRE_MATERIE
                                            Nome_Prodotto = CStr(dtMateriePrime.Rows(i).Item("Mat_Des")) &
                                            " (" & Gias.CodArticolo & ": " & CStr(dtMateriePrime.Rows(i).Item("Cod_Articolo")) & ")"
                                        Case Else
                                            Nome_Prodotto = CStr(dtMateriePrime.Rows(i).Item("Mat_Des"))
                                    End Select

                                    'Lotto Interno
                                    If Cod_Progetto <> 0 Then


                                        Dim objImpProg As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
                                        Nome_Prodotto &= " - Lotto Impianto: " & objImpProg.ProgettoNome_From_ProgettoCod_2(Piva,
                                                                                                                           Cod_Progetto,
                                                                                                                           CAU_PROGETTO_PRODUZIONE,
                                                                                                                           objParametri)
                                        objImpProg = Nothing

                                    End If

                                    'Lotto Accettazione
                                    If Lotto <> "" And Lotto <> "-1" And Lotto.ToLower <> "indefinito" Then
                                        Nome_Prodotto &= " - Lotto Magazzino: " & Lotto
                                    End If

                                    'Calibro
                                    If Cal_Cod <> 0 Then
                                        Nome_Prodotto &= " - Campionatura: " & Leggi_CampionaturaRaccolto(Cal_Cod, "",
                                                                                                          0, "",
                                                                                                          0, "",
                                                                                                          "", 0, 0, "", 0, 0, 0,
                                                                                                          "", True,
                                                                                                          objParametri)
                                    End If

                                Else
                                    '0 materie prime
                                    Return ""
                                End If
                            Else
                                'Dt_Materie_Prime nothing
                                Return ""
                            End If
                        End If
                    Else
                        '0 categorie
                        Return ""
                    End If
                Else
                    'dt categorie nothing
                    Return ""
                End If

        End Select


        If Flag_VisualizzaCategoria = True Then
            Return Nome_Categoria & ": " & Nome_Prodotto
        Else
            Return Nome_Prodotto
        End If



    End Function





    ''################################################################################
    'è leggermente diversa rispetto al giasonline
    '===========================================
    ' Decodifica della descrizione del prodotto 
    '-------------------------------------------
    Public Function LeggiProdottoStampeContab(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                              ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                              ByRef Nome_Categoria As String,
                                              ByRef Nome_Calibro As String,
                                              ByRef Peso_Set As Integer,
                                              ByVal Piva As String,
                                              ByVal Elem_Cod As Integer,
                                              ByVal Pro_Cod As Integer,
                                              ByVal Mat_Cod As Integer,
                                              Optional ByVal Cod_Progetto As Integer = 0,
                                              Optional ByVal Fase_Cod As Integer = 0,
                                              Optional ByVal Lotto As String = "",
                                              Optional ByVal Cal_Cod As Integer = 0,
                                              Optional ByVal Flag_VisualizzaCategoria As Boolean = False,
                                              Optional ByVal FinestraTemp_Inizio As Date = AGRODATAINIZIO,
                                              Optional ByVal FinestraTemp_Fine As Date = AGRODATAFINE,
                                              Optional ByVal Flag_DescrizioneXRaggruppa As Boolean = False,
                                              Optional ByVal Flag_ModalitaAccettazione As Boolean = False,
                                              Optional ByVal Flag_FiltraFormulati As Boolean = False,
                                              Optional ByRef ret_Cod_Articolo As String = "",
                                              Optional ByRef ret_Descr_prodotto_breve As String = "",
                                              Optional ByRef Flag_Extra As Integer = 0,
                                              Optional ByRef OTabella_Cod_Base As Integer = 0,
                                              Optional ByRef Udm_Cod_Extra As Integer = 0,
                                              Optional ByRef Qta_Extra As Double = 0,
                                              Optional ByVal Flag_CertificazioniDesc As Boolean = False,
                                              Optional ByVal Mat_Cod_Principale As Integer = 0,
                                              Optional ByVal titoloAlcol As Decimal = 0,
                                              Optional ByVal moduliCliente As List(Of Integer) = Nothing
                                              ) As String


        Dim rsProdotto As DataTable
        'Dim Tipo_Des As String = ""
        Dim nomeProdotto As String = ""
        Dim codArticolo As String = ""

        Nome_Categoria = ""
        Nome_Calibro = ""
        Peso_Set = -1

        '=======================================================
        'Decodifica del tipo prodotto e lettura della descrizione
        '-------------------------------------------------------

        Select Case Elem_Cod

            Case MACCHINE '--------------------- Parco Macchine

                Dim parcoMacchine As New AgronicaCoreContabDAL.Parco_Macchine_R

                rsProdotto = parcoMacchine.Leggi(CStr(Piva),
                                                 CInt(Mat_Cod),
                                                 False,
                                                 "",
                                                 "",
                                                 "",
                                                 "",
                                                 "",
                                                 0,
                                                 "",
                                                 False,
                                                 0,
                                                 "",
                                                 False,
                                                 FinestraTemp_Inizio,
                                                 FinestraTemp_Fine,
                                                 "",
                                                 "",
                                                 objParametri_Server)

                If Not IsNothing(rsProdotto) AndAlso rsProdotto.Rows.Count <> 0 Then
                    nomeProdotto = rsProdotto.Rows(0).Item("CLASS_DESC")
                Else
                    'Eccezione
                    Return ""
                End If

                '================================================================================


            Case ZOO_CONSISTENZA '-------------- ZOO ANIMALI

                If Cod_Progetto <> 0 Then

                    Dim rsConsistenza As DataTable

                    'Lettura Consistenza
                    Dim zoo As New AgronicaCoreZooDAL.Zoo_Animali_R
                    rsConsistenza = zoo.Leggi(Piva,
                                              0,
                                              Cod_Progetto,
                                              "",
                                              0,
                                              0,
                                              0,
                                              0,
                                              "",
                                              "",
                                              enumSelezioneVariabile.Selezione_TabellaCompleta,
                                              "",
                                              "",
                                              objParametri_Server)

                    If Not IsNothing(rsConsistenza) AndAlso rsConsistenza.Rows.Count <> 0 Then

                        Dim rsIPro As DataTable
                        Dim zooprod As New AgronicaCoreZooDAL.Lista_Indprod_Animali_R
                        rsIPro = zooprod.Leggi(CLng(rsConsistenza.Rows(0).Item("Gen_Cod")),
                                               CLng(rsConsistenza.Rows(0).Item("Spe_Cod")),
                                               CLng(rsConsistenza.Rows(0).Item("Ipro_Cod")),
                                               enumSelezioneVariabile.Selezione_TabellaCompleta,
                                               "",
                                               "", objParametri_Server)


                        If Not IsNothing(rsIPro) AndAlso rsIPro.Rows.Count <> 0 Then

                            nomeProdotto = rsIPro.Rows(0).Item("IPro_Des") & " - Matricola: " & rsConsistenza.Rows(0).Item("Matricola")

                        End If

                    End If

                End If

                '================================================================================

            Case Else '-------------------- Altro Tipo di Prodotto

                Dim nomeTabella, nomeCodice, nomeDescrizione As String

                Dim dtCategorie As DataTable
                Dim dtProdotti As DataTable
                Dim dtMateriePrime As DataTable
                Dim dtMatPrimeAlias As DataTable
                Dim i, j As Integer

                Dim objCatMagazzino As New AgronicaCoreMetaSchemaDAL.Categorie_Magazzino_R

                'legge le categorie di magazzino
                'dtCategorie = NewCom_CategorieMagazzino_Leggi(objServer, objSession, objPage,
                '                                              Elem_Cod,
                '                                              CAU_MAGAZZINO,
                '                                              "",
                '                                              False)

                dtCategorie = objCatMagazzino.Leggi(Elem_Cod, CAU_MAGAZZINO, False, "", "", objParametri_Server)



                If Not IsNothing(dtCategorie) Then

                    If dtCategorie.Rows.Count <> 0 Then

                        nomeTabella = dtCategorie.Rows(i).Item("Tabella")
                        nomeCodice = dtCategorie.Rows(i).Item("Tabella_Cod")
                        nomeDescrizione = dtCategorie.Rows(i).Item("Tabella_Des")
                        Nome_Categoria = dtCategorie.Rows(i).Item("NomeComune")


                        '=================================================================
                        'Verifico se il prodotto è Generale o Aziendale
                        '-----------------------------------------------------------------
                        If Pro_Cod <> 0 Then

                            'Leggo i prodotti con pro_cod
                            dtProdotti = objCatMagazzino.LeggiTabella_da_CategorieMagazzino(nomeTabella,
                                                                                            nomeCodice,
                                                                                            nomeDescrizione,
                                                                                            "",
                                                                                            Pro_Cod,
                                                                                            "", "", objParametri_Server)


                            If Not IsNothing(dtProdotti) Then

                                '(03/08/2015) commentato perché i dati delle revoca etc non sono piu in locale
                                'If Flag_FiltraFormulati = True Then
                                '    If Elem_Cod = FORMULATI Then
                                '        If FinestraTemp_Fine = AGRODATAFINE Then
                                '            FinestraTemp_Fine = Date.Today
                                '        End If
                                '        Dim objFiltraFormulati As New AgronicaCoreMetaSchemaDAL.Formulati_R
                                '        dtProdotti = objFiltraFormulati.Filtra_Formulati(dtProdotti, CDate(FinestraTemp_Fine), objParametri_Server)
                                '    End If
                                'End If

                                If Not IsNothing(dtProdotti) Then
                                    If dtProdotti.Rows.Count <> 0 Then
                                        nomeProdotto = dtProdotti.Rows(j).Item(nomeDescrizione)

                                        '  Giulia, 02/03/2017 15:43:14: vado a vedere se è presente un codice articolo in CAC_Codifiche_Prodotti
                                        Dim objCACcodProd As New AgronicaCoreAnagrafeDAL.CAC_Codifica_ProdottiAziendali_R
                                        codArticolo = CStr(objCACcodProd.Recupera_CodArticolo_from_CodiceGIAS(Piva,
                                                                                                              enum_Tipo_CAC_Codifica_ProdottiAziendali.NonDefinito,
                                                                                                              Elem_Cod, Pro_Cod, objParametri_Server))
                                    Else
                                        '0 prodotti
                                        Return ""
                                    End If
                                Else
                                    'dt prodotti nothing
                                    Return ""
                                End If
                            Else
                                'dt prodotti nothing
                                Return ""
                            End If

                        Else ' Mat_Cod <> 0

                            Dim objMatPrime As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
                            Dim objMatPrimeAlias As New AgronicaCoreAnagrafeDAL.Materie_Prime_Alias_R
                            Dim matCodIsAlias As Boolean

                            Dim FinestraTemporaleInizioTemp = objParametri_Server.FinestraTemporaleInizio
                            Dim FinestraTemporaleFineTemp = objParametri_Server.FinestraTemporaleFine



                            objParametri_Server.FinestraTemporaleInizio = FinestraTemp_Inizio
                            objParametri_Server.FinestraTemporaleFine = FinestraTemp_Fine

                            dtMateriePrime = objMatPrime.MateriePrime_Anagrafica(Piva,
                                                                                 0,
                                                                                 Elem_Cod,
                                                                                 Mat_Cod,
                                                                                 "", 0, 0, 0, 0, 0, 0, 0, "", 0, "", True, "", "",
                                                                                 objParametri_Server,
                                                                                 objParametri_Utenti)


                            objParametri_Server.FinestraTemporaleInizio = FinestraTemporaleInizioTemp
                            objParametri_Server.FinestraTemporaleFine = FinestraTemporaleFineTemp

                            If Not IsNothing(dtMateriePrime) Then

                                If dtMateriePrime.Rows.Count <> 0 Then

                                    '10/05/2016
                                    'Verifico se il mat_cod che viene passato in realtà è un alias di un altro mat_cod
                                    matCodIsAlias = objMatPrimeAlias.IsAlias(Piva, Mat_Cod, objParametri_Server)
                                    If matCodIsAlias Then
                                        'Lettura configurazione del mat_cod di base
                                        'per evitare che la sottoquery restituisca più di un valore mi passo anche il mat_cod principale, se presente
                                        dtMatPrimeAlias = objMatPrimeAlias.Leggi_ConfigurazioneReferenze(Piva, Mat_Cod, "", "", objParametri_Server, Mat_Cod_Principale)
                                        If Not IsNothing(dtMatPrimeAlias) Then
                                            If dtMatPrimeAlias.Rows.Count <> 0 Then
                                                Flag_Extra = dtMatPrimeAlias.Rows(0).Item("Flag_Extra")
                                                OTabella_Cod_Base = dtMatPrimeAlias.Rows(0).Item("OTabella_Cod_Base")
                                            End If
                                        End If
                                    Else
                                        Flag_Extra = dtMateriePrime.Rows(i).Item("Flag_Extra")
                                        OTabella_Cod_Base = dtMateriePrime.Rows(i).Item("OTabella_Cod_Base")
                                    End If

                                    Peso_Set = dtMateriePrime.Rows(i).Item("Peso_Set")
                                    Udm_Cod_Extra = dtMateriePrime.Rows(i).Item("Udm_Cod_Extra")
                                    Qta_Extra = dtMateriePrime.Rows(i).Item("Qta_Extra")

                                    '22/04/2015: visto che Ortofrutta Grosseto vuole il codice articolo davanti, cambio la formattazione
                                    'Cod_Articolo = " (Cod. " & CStr(Dt_Materie_Prime.Rows(i).Item("Cod_Articolo")) & ")"
                                    codArticolo = CStr(dtMateriePrime.Rows(i).Item("Cod_Articolo"))
                                    nomeProdotto = CStr(dtMateriePrime.Rows(i).Item("Mat_Des"))
                                    ret_Descr_prodotto_breve = CStr(dtMateriePrime.Rows(i).Item("Mat_Des"))

                                    'modifica del 25/11/2011: il cod_articolo viene aggiunto se impostato nelle impostazioni utente
                                    'Select Case Elem_Cod
                                    '    Case SEMILAVORATI_VEGETALI, SEMILAVORATI_ANIMALI, SEMENTI, ALTRE_MATERIE
                                    '        Nome_Prodotto = CStr(Dt_Materie_Prime.Rows(i).Item("Mat_Des")) & _
                                    '        " (Cod.Articolo: " & CStr(Dt_Materie_Prime.Rows(i).Item("Cod_Articolo")) & ")"
                                    '    Case Else
                                    '        Nome_Prodotto = CStr(Dt_Materie_Prime.Rows(i).Item("Mat_Des"))
                                    'End Select

                                    'nel caso di raggruppamento, non considero lotto interno e lotto accettazione
                                    If Flag_DescrizioneXRaggruppa = False Then

                                        'Lotto Interno
                                        If Cod_Progetto <> 0 Then

                                            Select Case Elem_Cod

                                                Case SEMILAVORATI_ANIMALI, MATERIE_ANIMALI, BENI_CONFEZ_ANIMALE, TRASFORMATI_ANIMALI

                                                    Dim rsAnimaleAnagrafe As New DataTable
                                                    Dim matricola As String
                                                    Dim zoo As New AgronicaCoreZooDAL.Zoo_Animali_R

                                                    rsAnimaleAnagrafe = zoo.Leggi(Piva,
                                                                                  0,
                                                                                  Cod_Progetto,
                                                                                  "",
                                                                                  0,
                                                                                  0,
                                                                                  0,
                                                                                  0,
                                                                                  "",
                                                                                  "",
                                                                                  enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                  "",
                                                                                  "",
                                                                                  objParametri_Server)

                                                    If Not IsNothing(rsAnimaleAnagrafe) AndAlso rsAnimaleAnagrafe.Rows.Count <> 0 Then
                                                        matricola = CStr(rsAnimaleAnagrafe.Rows(0).Item("matricola"))

                                                        If Not IsNothing(matricola) And matricola <> "" Then
                                                            nomeProdotto &= " - Matricola: " & matricola
                                                            ret_Descr_prodotto_breve &= " - Matricola: " & matricola
                                                        End If

                                                    End If


                                                Case SEMILAVORATI_VEGETALI, MATERIE_VEGETALI, BENI_CONFEZ_VEGETALE, TRASFORMATI_VEGETALI

                                                    Dim objImpProgetti As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
                                                    objImpProgetti.ProgettoNome_From_ProgettoCod_2(Piva,
                                                                                                   Cod_Progetto,
                                                                                                   CAU_PROGETTO_PRODUZIONE,
                                                                                                   objParametri_Server)

                                            End Select

                                        End If



                                        '----------------------------------
                                        'GESTIONE CERTIFICAZIONE PRODOTTI
                                        If Flag_CertificazioniDesc = True AndAlso Cal_Cod <> 0 Then

                                            Dim objDocContab As New AgronicaCoreStampeDAL.DocContab
                                            Dim descr_cert As String = objDocContab.Descrizione_Certificazione_FF_from_Cal_Cod(Cal_Cod, objParametri_Server)

                                            If descr_cert <> "" Then
                                                nomeProdotto &= " - " & descr_cert
                                            End If
                                        End If
                                        '----------------------------------

                                        '----------------------------------
                                        'TITOLO ALCOL TOTALE
                                        'Se mi arriva, lo inserisco; nella funzione chiamante lo valorizzo se ho l'impostazione
                                        If titoloAlcol <> 0 Then
                                            nomeProdotto &= " (" & titoloAlcol & " %Vol)"
                                        End If
                                        '----------------------------------

                                        '----------------------------------
                                        'GESTIONE LOTTO PRODOTTI
                                        Dim dettagliLotto As String = ""
                                        Dim objLotto As New AgronicaCoreAnagrafeDAL.Materie_PrimexLC_R

                                        'DettagliLotto = ___Gestione_LottoProdotto_VECCHIAVERSIONE(objServer, objSession, objPage, Piva, Elem_Cod, Mat_Cod, Lotto)
                                        ' Giulia: 30/8/2017:devo passare anche il mat_cod principale, perché se sto ricavando la descrizione di un alias, il lotto
                                        '   è collegato al mat_cod principale, non a quello dell'alias

                                        If moduliCliente Is Nothing Then
                                            'Leggo i moduli installati
                                            Dim objO As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R
                                            moduliCliente = objO.Recupera_Moduli_Cliente(Piva, objParametri_Server)
                                        End If

                                        dettagliLotto = objLotto.Gestione_LottoProdotto(Piva, Elem_Cod, Mat_Cod, Lotto, moduliCliente, objParametri_Server, Mat_Cod_Principale)

                                        If dettagliLotto <> "" Then
                                            nomeProdotto &= " " & dettagliLotto
                                            ret_Descr_prodotto_breve &= " " & dettagliLotto
                                        End If
                                        '----------------------------------

                                    End If 'Flag_DescrizioneXRaggruppa


                                    'Calibro
                                    If Cal_Cod <> 0 Then

                                        Dim Cal_Cod_Calibro As Integer = 0
                                        Dim Codice_Indice As Integer = 0
                                        Dim Codice_Danno As Integer = 0
                                        Dim Valore_Indice As String = ""
                                        Dim UdmCod_Indice As Integer = 0
                                        Dim Desc_Calibro As String = ""
                                        Dim Desc_Indice As String = ""
                                        Dim Desc_Danno As String = ""


                                        Nome_Calibro = Leggi_CampionaturaRaccolto(Cal_Cod_Calibro,
                                                                                                  Desc_Calibro,
                                                                                                  Codice_Indice,
                                                                                                  Desc_Indice,
                                                                                                  Codice_Danno,
                                                                                                  Desc_Danno,
                                                                                                  Valore_Indice,
                                                                                                  UdmCod_Indice,
                                                                                                  Cal_Cod,
                                                                                                  "", 0, 0, 0, "", True,
                                                                                                  objParametri_Server)

                                        'se sono in modalità normale, aggiungo il calibro alla descrizione
                                        If Flag_ModalitaAccettazione = False Then

                                            If Nome_Calibro.ToLower <> "indefinito" And Nome_Calibro <> "" Then
                                                nomeProdotto &= " - Campionatura: " & Nome_Calibro
                                            End If

                                        Else
                                            'modalità accettazione, non aggiungo il calibro alla descrizione
                                        End If

                                    End If 'cal_cod

                                Else
                                    '0 materie prime
                                    Return ""
                                End If
                            Else
                                'Dt_Materie_Prime nothing
                                Return ""
                            End If
                        End If
                    Else
                        '0 categorie
                        Return ""
                    End If
                Else
                    'dt categorie nothing
                    Return ""
                End If

        End Select

        Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim Flag_VisualCodArticolo As String
        Flag_VisualCodArticolo = objUtenti.ImpostazioneValore1_from_ImpostazioneCod(
                                            enum_Impostazioni_Utenti.SUPERUSER_COD_VISUAL_CODARTICOLO_DOCUMENTI,
                                            objParametri_Utenti,
                                            2)


        ret_Cod_Articolo = codArticolo

        If Flag_VisualCodArticolo = "1" Then
            'modifica del 22/04/2015
            'Nome_Prodotto &= Cod_Articolo
            nomeProdotto = codArticolo & " - " & nomeProdotto
        End If

        If Flag_VisualizzaCategoria = True Then
            Return Nome_Categoria & ": " & nomeProdotto
        Else
            Return nomeProdotto
        End If

    End Function



    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' legge i seguenti documenti:
    ''' fattura, ddt, ricevuta, nota di accredito, bolla di conferimento
    ''' può leggerne uno in particolare oppure tanti, a seconda del filtro
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function DocumentiContabili_RecuperaChiavi(ByVal Piva As String,
                                                      ByVal Lav_Cod As Integer,
                                                      ByVal Data_Inizio As Date,
                                                      ByVal Data_Fine As Date,
                                                      ByVal Cod_RisUm As Integer,
                                                      ByVal Anno As Integer,
                                                      ByVal Data_Movimento As Date,
                                                      ByVal Scadenza As Date,
                                                      ByVal Doc_Numero_Sin As String,
                                                      ByVal Doc_Numero As Decimal,
                                                      ByVal Doc_Numero_Des As String,
                                                      ByVal Progr_Protocollo As Integer,
                                                      ByVal Progr_Registrazione As Integer,
                                                      ByVal Data_Registrazione As Date,
                                                      ByVal Jolly_Int As enum_TipoMovimentazioneMagazzino,
                                                      ByVal flag_Solo_Prodotti_BdGias As Boolean,
                                                      ByVal flag_AltriBeniStrumentali As Boolean,
                                                      ByVal flag_Mov_Dettaglio_Tecnico_Extra As Boolean,
                                                      ByVal xFiltroAggiuntivo As String,
                                                      ByVal xOrderBy As String,
                                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                      ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Contabilita.DocumentiContabili_RecuperaChiavi"

        Dim messaggioErrore As String = ""
        Dim stbSql As New StringBuilder
        Dim dt As DataTable

        Try

            stbSql.Length = 0

            stbSql.AppendLine(" SELECT * ")
            stbSql.AppendLine(" FROM ")

            stbSql.AppendLine(" ( ")


            '------------------------------------------------------------------------
            '---- PRIMA PARTE DELL'UNION: dettagli con magazzino --------------------
            '------------------------------------------------------------------------
            stbSql.AppendLine(" ( ")
            '------------------------------------------------------ 
            '------------------- SELECT ---------------------------
            '------------------------------------------------------
            stbSql.AppendLine(" SELECT DISTINCT imprese.rag_soc, Agenda.PIVA, Agenda.Lav_Cod,  ")
            stbSql.AppendLine(" Mov_Contabile.Data_Movimento, ")
            stbSql.AppendLine(" Mov_Contabile.Doc_Numero_Sin, Mov_Contabile.Doc_Numero, Mov_Contabile.Doc_Numero_Des  ")


            '------------------------------------------------------ 
            '-------------------- FROM ----------------------------
            '------------------------------------------------------
            stbSql.AppendLine(" FROM Agenda  ")
            stbSql.AppendLine(" INNER JOIN Imprese ON Agenda.piva = imprese.Piva  ")
            stbSql.AppendLine(" INNER JOIN Movimenti Mov_Contabile ON Agenda.PIVA = Mov_Contabile.PIVA AND Agenda.Sa_Cod = Mov_Contabile.Sa_Cod AND Agenda.Id_Agenda = Mov_Contabile.Id_Agenda  ")
            stbSql.AppendLine(" INNER JOIN Movimenti Mov_Magazzino ON Agenda.PIVA = Mov_Magazzino.PIVA AND Agenda.Sa_Cod = Mov_Magazzino.Sa_Cod AND Agenda.Id_Agenda = Mov_Magazzino.Id_Agenda  ")
            stbSql.AppendLine(" INNER JOIN Movimenti_Dettagli Mov_Dett_Magazzino ON Mov_Magazzino.PIVA = Mov_Dett_Magazzino.PIVA AND Mov_Magazzino.Id_Agenda = Mov_Dett_Magazzino.Id_Agenda AND Mov_Magazzino.Id_Mov = Mov_Dett_Magazzino.Id_Mov  ")

            '------------------------------------------------------ 
            '-------------------- WHERE ---------------------------
            '------------------------------------------------------
            stbSql.AppendLine(" WHERE Mov_Contabile.Cau_Mov = '" & CAU_REGISTRAZIONI & "' ")
            ''i documenti in uscita hanno il cau_mov dello scarico, ma i resi hanno quello del carico 
            ''le bolle di conferimento hanno il loro cau_mov
            'StbSQL.AppendLine(" AND    Mov_Magazzino.Cau_Mov IN ( '" &
            '                        Agro_SQL_SaveText(CAU_SCARICO) & "', '" &
            '                        Agro_SQL_SaveText(CAU_CARICO) & "', '" &
            '                        Agro_SQL_SaveText(CAU_ABBUONI) & "', '" &
            '                        Agro_SQL_SaveText(CAU_CONFERIMENTO) & "', '" &
            '                        Agro_SQL_SaveText(CAU_CONFERIMENTO_DIVERSI) & "', '" &
            '                        Agro_SQL_SaveText(CAU_ACCETTAZIONE_BENI) & "', '" &
            '                        Agro_SQL_SaveText(CAU_ACCETTAZIONE_BENI_DA_DIVERSI) &
            '                        "'  ) ")

            If Jolly_Int <> enum_TipoMovimentazioneMagazzino.NonImpostato Then
                stbSql.AppendLine(" AND Mov_Dett_Magazzino.Jolly_Int = " & Agro_SQL_SaveNum(Jolly_Int) & "  ")
            End If

            If flag_Solo_Prodotti_BdGias = True Then
                stbSql.AppendLine(" AND Mov_Dett_Magazzino.Elem_Cod IN ( " & CARBURANTI & ",   ")
                stbSql.AppendLine("                                     " & FERTILIZZANTI & ",   ")
                stbSql.AppendLine("                                     " & FORMULATI & ",   ")
                stbSql.AppendLine("                                     " & INSETTI & ",   ")
                stbSql.AppendLine("                                     " & TRAPPOLE & ",   ")
                stbSql.AppendLine("                                     " & INNESCHI & ")   ")
            End If

            stbSql.AppendLine(" AND Mov_Contabile.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine) & "   ")
            stbSql.AppendLine(" AND Mov_Contabile.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & "   ")

            If Lav_Cod <> 0 Then
                stbSql.AppendLine(" AND Agenda.Lav_Cod = " & CStr(Lav_Cod) & " ")
            End If

            If Piva <> "" Then
                stbSql.AppendLine(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Cod_RisUm <> 0 Then
                stbSql.AppendLine(" AND Mov_Contabile.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
            End If

            If Anno <> 0 Then
                stbSql.AppendLine(" AND Year(Mov_Contabile.Data_Movimento) = " & Agro_SQL_SaveNum(Anno) & "  ")
            End If

            If Data_Movimento <> AGRODATAINIZIO Then
                stbSql.AppendLine(" AND Mov_Contabile.Data_Movimento = " & UCase(Agro_SQL_SaveDate(Data_Movimento)) & "   ")
            End If

            If Scadenza <> AGRODATAFINE And Scadenza <> AGRODATAINIZIO Then
                stbSql.AppendLine(" AND Mov_Contabile.Scadenza = " & UCase(Agro_SQL_SaveDate(Scadenza)) & "   ")
            End If

            If Doc_Numero_Sin.ToUpper <> "XYZ" Then
                stbSql.AppendLine(" AND Mov_Contabile.Doc_Numero_Sin = '" & UCase(Agro_SQL_SaveText(Doc_Numero_Sin)) & "'   ")
            End If

            If Doc_Numero <> 0 Then
                stbSql.AppendLine(" AND Mov_Contabile.Doc_Numero = " & Agro_SQL_SaveNum(Doc_Numero) & "   ")
            End If

            If Doc_Numero_Des.ToUpper <> "XYZ" Then
                stbSql.AppendLine(" AND Mov_Contabile.Doc_Numero_Des = '" & UCase(Agro_SQL_SaveText(Doc_Numero_Des)) & "'   ")
            End If

            If Progr_Protocollo <> 0 Then
                stbSql.AppendLine(" AND Mov_Contabile.Progr_Protocollo = " & Agro_SQL_SaveNum(Progr_Protocollo) & "   ")
            End If

            If Progr_Registrazione <> 0 Then
                stbSql.AppendLine(" AND Mov_Contabile.Progr_Registrazione = " & Agro_SQL_SaveNum(Progr_Registrazione) & "   ")
            End If

            If Data_Registrazione <> AGRODATAINIZIO Then
                stbSql.AppendLine(" AND Mov_Contabile.Data_Registrazione = " & UCase(Agro_SQL_SaveDate(Data_Registrazione)) & "   ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                stbSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------


            stbSql.AppendLine(" ) ")

            If flag_AltriBeniStrumentali = True Then

                stbSql.AppendLine(" UNION ALL ")

                '------------------------------------------------------------------------
                '---- SECONDA PARTE DELL'UNION: dettagli altri beni strumentali ---------
                '------------------------------------------------------------------------
                stbSql.AppendLine(" ( ")
                '------------------------------------------------------ 
                '------------------- SELECT ---------------------------
                '------------------------------------------------------
                stbSql.AppendLine(" SELECT DISTINCT Imprese.Rag_Soc, Agenda.PIVA, Agenda.Lav_Cod,  ")
                stbSql.AppendLine(" Mov_Contabile.Data_Movimento, ")
                stbSql.AppendLine(" Mov_Contabile.Doc_Numero_Sin, Mov_Contabile.Doc_Numero, Mov_Contabile.Doc_Numero_Des  ")

                '------------------------------------------------------ 
                '-------------------- FROM ----------------------------
                '------------------------------------------------------
                stbSql.AppendLine(" FROM Agenda  ")
                stbSql.AppendLine(" INNER JOIN Imprese ON Agenda.piva = imprese.Piva  ")
                stbSql.AppendLine(" INNER JOIN Movimenti Mov_Contabile ON Agenda.PIVA = Mov_Contabile.PIVA AND Agenda.Sa_Cod = Mov_Contabile.Sa_Cod AND Agenda.Id_Agenda = Mov_Contabile.Id_Agenda  ")
                stbSql.AppendLine(" INNER JOIN Movimenti Mov_Magazzino ON Agenda.PIVA = Mov_Magazzino.PIVA AND Agenda.Sa_Cod = Mov_Magazzino.Sa_Cod AND Agenda.Id_Agenda = Mov_Magazzino.Id_Agenda  ")
                stbSql.AppendLine(" INNER JOIN Movimenti_Dettagli Mov_Dett_Magazzino ON Mov_Magazzino.PIVA = Mov_Dett_Magazzino.PIVA AND Mov_Magazzino.Id_Agenda = Mov_Dett_Magazzino.Id_Agenda AND Mov_Magazzino.Id_Mov = Mov_Dett_Magazzino.Id_Mov  ")

                '------------------------------------------------------ 
                '-------------------- WHERE ---------------------------
                '------------------------------------------------------
                stbSql.AppendLine(" WHERE Mov_Contabile.Cau_Mov = '" & CAU_REGISTRAZIONI & "' ")
                ''i documenti in uscita hanno il cau_mov dello scarico, ma i resi hanno quello del carico 
                ''le bolle di conferimento hanno il loro cau_mov
                'StbSQL.AppendLine(" AND    Mov_Magazzino.Cau_Mov IN ( '" &
                '                        Agro_SQL_SaveText(CAU_SCARICO) & "', '" &
                '                        Agro_SQL_SaveText(CAU_CARICO) & "', '" &
                '                        Agro_SQL_SaveText(CAU_ABBUONI) & "', '" &
                '                        Agro_SQL_SaveText(CAU_CONFERIMENTO) & "', '" &
                '                        Agro_SQL_SaveText(CAU_CONFERIMENTO_DIVERSI) & "', '" &
                '                        Agro_SQL_SaveText(CAU_ACCETTAZIONE_BENI) & "', '" &
                '                        Agro_SQL_SaveText(CAU_ACCETTAZIONE_BENI_DA_DIVERSI) &
                '                        "'  ) ")

                stbSql.AppendLine(" AND NOT EXISTS ( ")
                stbSql.AppendLine("                 SELECT 1 ")
                stbSql.AppendLine("                 FROM Mov_Destinazioni  ")
                stbSql.AppendLine("                 WHERE Mov_Destinazioni.PIVA = Mov_Dett_Magazzino.PIVA AND Mov_Destinazioni.Id_Agenda = Mov_Dett_Magazzino.Id_Agenda AND Mov_Destinazioni.Id_Mov = Mov_Dett_Magazzino.Id_Mov AND Mov_Destinazioni.Id_Mov_Det = Mov_Dett_Magazzino.Id_Mov_Det ")
                stbSql.AppendLine("                 )")

                If Lav_Cod <> 0 Then
                    stbSql.AppendLine(" AND Agenda.Lav_Cod = " & CStr(Lav_Cod) & " ")
                End If

                If Piva <> "" Then
                    stbSql.AppendLine(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                End If

                If Cod_RisUm <> 0 Then
                    stbSql.AppendLine(" AND Mov_Contabile.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
                End If

                If Anno <> 0 Then
                    stbSql.AppendLine(" AND Year(Mov_Contabile.Data_Movimento) = " & Agro_SQL_SaveNum(Anno) & "  ")
                End If

                If Data_Movimento <> AGRODATAINIZIO Then
                    stbSql.AppendLine(" AND Mov_Contabile.Data_Movimento = " & UCase(Agro_SQL_SaveDate(Data_Movimento)) & "   ")
                End If

                If Scadenza <> AGRODATAFINE And Scadenza <> AGRODATAINIZIO Then
                    stbSql.AppendLine(" AND Mov_Contabile.Scadenza = " & UCase(Agro_SQL_SaveDate(Scadenza)) & "   ")
                End If

                If Doc_Numero_Sin.ToUpper <> "XYZ" Then
                    stbSql.AppendLine(" AND Mov_Contabile.Doc_Numero_Sin = '" & UCase(Agro_SQL_SaveText(Doc_Numero_Sin)) & "'   ")
                End If

                If Doc_Numero <> 0 Then
                    stbSql.AppendLine(" AND Mov_Contabile.Doc_Numero = " & Agro_SQL_SaveNum(Doc_Numero) & "   ")
                End If

                If Doc_Numero_Des.ToUpper <> "XYZ" Then
                    stbSql.AppendLine(" AND Mov_Contabile.Doc_Numero_Des = '" & UCase(Agro_SQL_SaveText(Doc_Numero_Des)) & "'   ")
                End If

                If Progr_Protocollo <> 0 Then
                    stbSql.AppendLine(" AND Mov_Contabile.Progr_Protocollo = " & Agro_SQL_SaveNum(Progr_Protocollo) & "   ")
                End If

                If Progr_Registrazione <> 0 Then
                    stbSql.AppendLine(" AND Mov_Contabile.Progr_Registrazione = " & Agro_SQL_SaveNum(Progr_Registrazione) & "   ")
                End If

                If Data_Registrazione <> AGRODATAINIZIO Then
                    stbSql.AppendLine(" AND Mov_Contabile.Data_Registrazione = " & UCase(Agro_SQL_SaveDate(Data_Registrazione)) & "   ")
                End If

                '--------------------------------------------------------------------------
                If xFiltroAggiuntivo <> "" Then
                    stbSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                End If
                stbSql.AppendLine(" ) ")

            End If

            '------------------------------------------------------------------------
            '---- FINE UNION ---------
            '------------------------------------------------------------------------
            stbSql.AppendLine(" ) DOCUMENTI ")

            If xOrderBy <> "" Then
                stbSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                stbSql.AppendLine(" ORDER BY PIVA, Lav_Cod,  ")
                stbSql.AppendLine(" Data_Movimento, Doc_Numero_Sin, Doc_Numero, Doc_Numero_Des  ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function





    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' legge i seguenti documenti:
    ''' fattura, ddt, ricevuta, nota di accredito, bolla di conferimento
    ''' può leggerne uno in particolare oppure tanti, a seconda del filtro
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function DocumentiContabili_byfiltri(ByVal Piva As String,
                                                ByVal Lav_Cod As Integer,
                                                ByVal Id_Agenda As Integer,
                                                ByVal Data_Inizio As Date,
                                                ByVal Data_Fine As Date,
                                                ByVal Cod_RisUm As Integer,
                                                ByVal Anno As Integer,
                                                ByVal Data_Movimento As Date,
                                                ByVal Scadenza As Date,
                                                ByVal Doc_Numero_Sin As String,
                                                ByVal Doc_Numero As Decimal,
                                                ByVal Doc_Numero_Des As String,
                                                ByVal Progr_Protocollo As Integer,
                                                ByVal Progr_Registrazione As Integer,
                                                ByVal Data_Registrazione As Date,
                                                ByVal flag_Solo_Prodotti_BdGias As Boolean,
                                                ByVal flag_AltriBeniStrumentali As Boolean,
                                                ByVal flag_Mov_Dettaglio_Tecnico_Extra As Boolean,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                    ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Contabilita.DocumentiContabili_byfiltri"

        Dim messaggioErrore As String = ""
        Dim stbSql As New StringBuilder
        Dim dt As DataTable

        Try

            stbSql.Length = 0

            stbSql.AppendLine(" SELECT * ")
            stbSql.AppendLine(" FROM ")

            stbSql.AppendLine(" ( ")


            '------------------------------------------------------------------------
            '---- PRIMA PARTE DELL'UNION: dettagli con magazzino --------------------
            '------------------------------------------------------------------------
            stbSql.AppendLine(" ( ")
            '------------------------------------------------------ 
            '------------------- SELECT ---------------------------
            '------------------------------------------------------
            stbSql.AppendLine(" SELECT Agenda.PIVA, Agenda.Id_Agenda, Agenda.Lav_Cod, Agenda.des_lib, ")

            stbSql.AppendLine(" Mov_Contabile.Id_Mov AS Id_Mov_Contabile, Mov_Contabile.Data_Movimento, Mov_Contabile.Ora , ")
            stbSql.AppendLine(" Mov_Contabile.Doc_Numero_Sin, Mov_Contabile.Doc_Numero,Mov_Contabile.Doc_Numero_Des, Mov_Contabile.Mov_Desc ,  ")
            stbSql.AppendLine(" Mov_Contabile.Scadenza, Mov_Contabile.Num_Protocollo, Mov_Contabile.Causale_Trasporto, Mov_Contabile.Aspetto, Mov_Contabile.Peso,  ")
            stbSql.AppendLine(" Mov_Contabile.Colli, Mov_Contabile.Extra_Str, Mov_Contabile.Extra_Int, Mov_Contabile.Extra_Date,  ")
            stbSql.AppendLine(" Mov_Contabile.Tipo_Sconto, Mov_Contabile.Natura_Beni, Mov_Contabile.Tara_Veicolo, Mov_Contabile.Tara_Imballi, Mov_Contabile.Tipo_Peso, Mov_Contabile.Modalita, Mov_Contabile.Username_Note,  ")
            stbSql.AppendLine(" Mov_Contabile.Scadenza_Extra, Mov_Contabile.Progr_Protocollo, Mov_Contabile.Progr_Registrazione, Mov_Contabile.Data_Registrazione,  ")
            stbSql.AppendLine(" Mov_Contabile.ChkLayOut_Join_Prodotti, Mov_Contabile.chklayout_bypass_fatturato, Mov_Contabile.ChkLayOut_Peso, Mov_Contabile.ChkLayOut_Prezzo,  ")
            stbSql.AppendLine("  ")
            stbSql.AppendLine(" Mov_Contabile.Cod_RisUm, Contatti_Cliente.Cod_Contatto AS Cod_Contatto_Cliente, Contatti_Cliente.Rag_Soc AS Rag_Soc_Cliente, Contatti_Cliente.Codice_Fiscale AS Codice_Fiscale_Cliente,  ")
            stbSql.AppendLine(" Contatti_Cliente.Id_Cf AS Id_Cf_Cliente, Contatti_Cliente.Nome AS Nome_Cliente, Contatti_Cliente.Cognome AS Cognome_Cliente,  ")
            stbSql.AppendLine("  Mov_Contabile.Cod_IndirizzoRisUm,  ")
            'StbSQL.AppendLine(" Indirizzi_Cliente.ind_des AS ind_des_Cliente, Indirizzi_Cliente.frz_des AS frz_des_Cliente, Indirizzi_Cliente.CAP AS cap_Cliente,  ")
            'StbSQL.AppendLine(" Indirizzi_Cliente.stato AS stato_Cliente, Indirizzi_Cliente.pro_cod_istat AS pro_cod_istat_Cliente,  ")
            'StbSQL.AppendLine(" Indirizzi_Cliente.com_cod_istat AS com_cod_istat_Cliente, Istat_Cliente.LOCALITA AS localita_Cliente, Istat_Cliente.COMUNI_PROV AS comuni_prov_Cliente, ")
            'StbSQL.AppendLine("  ")
            stbSql.AppendLine(" Mov_Contabile.Cod_Destinazione, Mov_Contabile.Cod_IndirizzoDestinazione,  ")
            'StbSQL.AppendLine(" ISNULL(Contatti_DestDiv.Cod_Contatto, '') AS Cod_Contatto_DestDiv, ISNULL(Contatti_DestDiv.Rag_Soc, '') AS Rag_Soc_DestDiv, ISNULL(Contatti_DestDiv.Codice_Fiscale, '') AS Codice_Fiscale_DestDiv,  ")
            'StbSQL.AppendLine(" ISNULL(Contatti_DestDiv.Id_Cf, 0) AS Id_Cf_DestDiv, ISNULL(Contatti_DestDiv.Nome, '') AS Nome_DestDiv, ISNULL(Contatti_DestDiv.Cognome, '') AS Cognome_DestDiv,  ")
            'StbSQL.AppendLine(" ISNULL(Indirizzi_DestDiv.ind_des, '') AS ind_des_DestDiv, ISNULL(Indirizzi_DestDiv.frz_des, '') AS frz_des_DestDiv, ISNULL(Indirizzi_DestDiv.CAP, '') AS cap_DestDiv,  ")
            'StbSQL.AppendLine(" ISNULL(Indirizzi_DestDiv.stato, '') AS stato_DestDiv, ISNULL(Indirizzi_DestDiv.pro_cod_istat, '') AS pro_cod_istat_DestDiv,  ")
            'StbSQL.AppendLine(" ISNULL(Indirizzi_DestDiv.com_cod_istat, '') AS com_cod_istat_DestDiv, ISNULL(Istat_DestDiv.LOCALITA, '') AS localita_DestDiv, ISNULL(Istat_DestDiv.COMUNI_PROV, '') AS comuni_prov_DestDiv, ")
            'StbSQL.AppendLine("  ")
            stbSql.AppendLine(" Mov_Contabile.Mezzo, Mov_Contabile.Cod_Vettore, Mov_Contabile.Cod_IndirizzoVettore,  ")
            'StbSQL.AppendLine(" ISNULL(Contatti_Vettore.Cod_Contatto, '') AS Cod_Contatto_Vettore, ISNULL(Contatti_Vettore.Rag_Soc, '') AS Rag_Soc_Vettore, ISNULL(Contatti_Vettore.Codice_Fiscale, '') AS Codice_Fiscale_Vettore,  ")
            'StbSQL.AppendLine(" ISNULL(Contatti_Vettore.Id_Cf, 0) AS Id_Cf_Vettore, ISNULL(Contatti_Vettore.Nome, '') AS Nome_Vettore, ISNULL(Contatti_Vettore.Cognome, '') AS Cognome_Vettore,  ")
            'StbSQL.AppendLine(" ISNULL(Indirizzi_Vettore.ind_des, '') AS ind_des_Vettore, ISNULL(Indirizzi_Vettore.frz_des, '') AS frz_des_Vettore, ISNULL(Indirizzi_Vettore.CAP, '') AS cap_Vettore,  ")
            'StbSQL.AppendLine(" ISNULL(Indirizzi_Vettore.stato, '') AS stato_Vettore, Indirizzi_Vettore.pro_cod_istat AS pro_cod_istat_Vettore,  ")
            'StbSQL.AppendLine(" Indirizzi_Vettore.com_cod_istat AS com_cod_istat_Vettore, ISNULL(Istat_Vettore.LOCALITA, '') AS localita_Vettore, ISNULL(Istat_Vettore.COMUNI_PROV, '') AS comuni_prov_Vettore,  ")
            'StbSQL.AppendLine("  ")
            ''StbSQL.AppendLine(" Parco_Macchine.Class_Code, Parco_Macchine.Mac_Des, Parco_Macchine.Targa, Parco_Macchine.Telaio, Parco_Macchine.Modello,  ")
            ''StbSQL.AppendLine(" Parco_Macchine.Potenza, Parco_Macchine.Data_Immatricolazione, Parco_Macchine.N_Immatricolazione,  ")
            ''StbSQL.AppendLine(" Parco_Macchine.N_Immatricolazione_Rimorchio, Parco_Macchine.N_Autorizzazione_Trasporto, Parco_Macchine.Data_Rilascio_Autorizzazione, Parco_Macchine.Peso, ")
            ''StbSQL.AppendLine("  ")
            'StbSQL.AppendLine(" Movimento_Extra.Mac_Cod,  Movimento_Extra.Mezzo_Trasporto, Movimento_Extra.Targa AS Targa_2, Movimento_Extra.N_Immatricolazione AS N_Immatricolazione_2, Movimento_Extra.N_Immatricolazione_Rimorchio AS N_Immatricolazione_Rimorchio_2, ")
            'StbSQL.AppendLine(" Movimento_Extra.N_Autorizzazione_Trasporto AS N_Autorizzazione_Trasporto_2, Movimento_Extra.Data_Rilascio_Autorizzazione AS Data_Rilascio_Autorizzazione_2, Movimento_Extra.Peso AS Peso_2, ")
            'StbSQL.AppendLine("  ")
            'StbSQL.AppendLine(" Movimento_Extra.Id_Reg_Dettaglio AS Id_Movimento_Extra, Movimento_Extra.Indicazioni_Complementari, ")
            'StbSQL.AppendLine(" Movimento_Extra.Tipo_Documento, Movimento_Extra.Luogo_Partenza, Movimento_Extra.Luogo_Consegna, Movimento_Extra.Id_Reg_Dettaglio AS Id_Movimento_Extra, Movimento_Extra.Indicazioni_Complementari, ")
            'StbSQL.AppendLine(" Movimento_Extra.Data_Spedizione,  Movimento_Extra.Precisazioni, Movimento_Extra.Annotazioni,  ")
            'StbSQL.AppendLine("  ")
            stbSql.AppendLine(" Mov_Magazzino.Id_Mov AS Id_Mov_Magazzino, Mov_Magazzino.Mov_Desc AS Mov_Desc_Mag, Mov_Magazzino.Data_Movimento AS Data_Mag,Mov_Magazzino.Ora AS Ora_Mag,  ")
            'StbSQL.AppendLine("  ")
            'StbSQL.AppendLine(" Dettagli_Extra.Id_Reg_Dettaglio AS Id_Dettagli_Extra, ISNULL(Dettagli_Extra.Marche_Contenitori, '') AS Marche_Contenitori,  ")
            'StbSQL.AppendLine(" ISNULL(Dettagli_Extra.Num_Contenitori, 0) AS Num_Contenitori, ISNULL(Dettagli_Extra.Des_Contenitori, '') AS Des_Contenitori, ISNULL(Dettagli_Extra.Num_Colli, 0) AS Num_Colli, Dettagli_Extra.Titolo_Alcol, ")
            'StbSQL.AppendLine("  ")
            stbSql.AppendLine(" Mov_Dett_Magazzino.Id_Mov_Det, Mov_Dett_Magazzino.Mov_Det_Des, Mov_Dett_Magazzino.Elem_Cod, Mov_Dett_Magazzino.Pro_Cod, Mov_Dett_Magazzino.Mat_Cod,  ")
            stbSql.AppendLine(" Mov_Dett_Magazzino.Cod_Progetto, Mov_Dett_Magazzino.Fase_Cod, Mov_Dett_Magazzino.Lotto, Mov_Dett_Magazzino.Cal_Cod, Mov_Dett_Magazzino.Udm_Cod, ")
            stbSql.AppendLine(" Mov_Dett_Magazzino.Qta AS Qta, Mov_Dett_Magazzino.Variazione AS Variazione,Mov_Dett_Magazzino.Listino_Cod, Mov_Dett_Magazzino.Tara,  ")
            stbSql.AppendLine(" Mov_Dett_Magazzino.Prezzo_Unitario, Mov_Dett_Magazzino.Prezzo_Unitario_Netto, Mov_Dett_Magazzino.Imponibile, Mov_Dett_Magazzino.Imponibile_Netto,  ")
            stbSql.AppendLine(" Mov_Dett_Magazzino.Jolly_Int, Mov_Dett_Magazzino.Contabilizzato,Mov_Dett_Magazzino.Pendente, Mov_Dett_Magazzino.Prezzo_Effettivo, ")
            stbSql.AppendLine(" Mov_Dett_Magazzino.Cod_Iva, Mov_Dett_Magazzino.Sconto, Mov_Dett_Magazzino.Cod_Conto, Mov_Dett_Magazzino.Extra_Str AS Extra_Str_Dett, Mov_Dett_Magazzino.Extra_Int AS Extra_Int_Dett, ")
            stbSql.AppendLine(" Mov_Dett_Magazzino.Extra_Date AS Extra_Date_Dett, Mov_Dett_Magazzino.Anno, Mov_Dett_Magazzino.Ric_Cod, Mov_Dett_Magazzino.Iva, Mov_Dett_Magazzino.UDM_COD_EXTRA, Mov_Dett_Magazzino.QTA_EXTRA,  ")
            stbSql.AppendLine(" Mov_Dett_Magazzino.Cod_IvaIndetraibile, Mov_Dett_Magazzino.Qta_Extra_Totale, Mov_Dett_Magazzino.ChkLayOut_Hide, ")
            stbSql.AppendLine(" Mov_Dett_Magazzino.sconto_listino, ISNULL(Mov_Dett_Magazzino.sconto_Testo, '') AS Sconto_Testo, Mov_Dett_Magazzino.Sconto_modalita, Mov_Dett_Magazzino.chkiva_manuale, Mov_Dett_Magazzino.Mat_Cod_Alias, Mov_Dett_Magazzino.Mezzo_Det, ")
            stbSql.AppendLine(" IVA_Aliquote.Sigla AS Sigla_IVA, IVA_Aliquote.Aliquota, ")
            stbSql.AppendLine(" Mov_Destinazioni.Sa_cod AS Sa_Cod_Dest, Id_Destinazione, Mov_Destinazioni.Tipo_Destinazione, Mov_Destinazioni.Qta AS Qta_Dest ")
            stbSql.AppendLine("  ")
            If flag_Mov_Dettaglio_Tecnico_Extra = True Then
                stbSql.AppendLine(" , ISNULL(Movimento_Extra.Id_Gestione_Vettore, 0) AS Id_Gestione_Vettore   ")
            End If
            stbSql.AppendLine("  ")
            stbSql.AppendLine("  ")
            stbSql.AppendLine("  ")


            'StbSQL.AppendLine(" ,UnitaMisura.Udm_Des, UnitaMisura.Udm_Sim,   ")
            ''Dettagli_Extra.Codice_Prodotto, Dettagli_Extra.Colore, Dettagli_Extra.Zona_Viticola, Dettagli_Extra.Manipolazioni, 
            'StbSQL.AppendLine(" Materie_Prime.codice_prodotto, Materie_Prime.colore, Materie_Prime.codice_nc, Materie_Prime.manipolazioni ")
            'StbSQL.AppendLine("  ")

            '------------------------------------------------------ 
            '-------------------- FROM ----------------------------
            '------------------------------------------------------
            stbSql.AppendLine(" FROM Agenda  ")

            stbSql.AppendLine(" INNER JOIN Movimenti Mov_Contabile ON Agenda.PIVA = Mov_Contabile.PIVA AND Agenda.Sa_Cod = Mov_Contabile.Sa_Cod AND Agenda.Id_Agenda = Mov_Contabile.Id_Agenda  ")
            stbSql.AppendLine("  ")
            stbSql.AppendLine(" INNER JOIN Risorse_Umane Risorse_Umane_Cliente ON Mov_Contabile.Cod_RisUm = Risorse_Umane_Cliente.Cod_RisUm  ")
            stbSql.AppendLine(" INNER JOIN Contatti Contatti_Cliente ON Risorse_Umane_Cliente.Piva = Contatti_Cliente.Piva AND Risorse_Umane_Cliente.Cod_Contatto = Contatti_Cliente.Cod_Contatto  ")
            'StbSQL.AppendLine(" INNER JOIN Indirizzi Indirizzi_Cliente ON  Mov_Contabile.Cod_IndirizzoRisUm = Indirizzi_Cliente.cod_indirizzo ")
            'StbSQL.AppendLine(" INNER JOIN ISTAT Istat_Cliente ON Indirizzi_Cliente.pro_cod_istat = Istat_Cliente.PROV AND Indirizzi_Cliente.com_cod_istat = Istat_Cliente.COM ")
            stbSql.AppendLine("  ")
            'StbSQL.AppendLine(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_DestDiv ON Mov_Contabile.Cod_Destinazione = Risorse_Umane_DestDiv.Cod_RisUm  ")
            'StbSQL.AppendLine(" LEFT OUTER JOIN Contatti Contatti_DestDiv ON Risorse_Umane_DestDiv.Piva = Contatti_DestDiv.Piva AND Risorse_Umane_DestDiv.Cod_Contatto = Contatti_DestDiv.Cod_Contatto  ")
            'StbSQL.AppendLine(" LEFT OUTER JOIN Indirizzi Indirizzi_DestDiv ON  Mov_Contabile.Cod_IndirizzoDestinazione= Indirizzi_DestDiv.cod_indirizzo ")
            'StbSQL.AppendLine(" LEFT OUTER JOIN ISTAT Istat_DestDiv ON Indirizzi_DestDiv.pro_cod_istat = Istat_DestDiv.PROV AND Indirizzi_DestDiv.com_cod_istat = Istat_DestDiv.COM ")
            'StbSQL.AppendLine("  ")
            'StbSQL.AppendLine(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Vettore ON Mov_Contabile.Cod_Vettore = Risorse_Umane_Vettore.Cod_RisUm  ")
            'StbSQL.AppendLine(" LEFT OUTER JOIN Contatti Contatti_Vettore ON Risorse_Umane_Vettore.Piva = Contatti_Vettore.Piva AND Risorse_Umane_Vettore.Cod_Contatto = Contatti_Vettore.Cod_Contatto  ")
            'StbSQL.AppendLine(" LEFT OUTER JOIN Indirizzi Indirizzi_Vettore ON  Mov_Contabile.Cod_IndirizzoVettore = Indirizzi_Vettore.cod_indirizzo ")
            'StbSQL.AppendLine(" LEFT OUTER JOIN ISTAT Istat_Vettore ON Indirizzi_Vettore.pro_cod_istat = Istat_Vettore.PROV AND Indirizzi_Vettore.com_cod_istat = Istat_Vettore.COM ")
            'StbSQL.AppendLine("  ")
            'StbSQL.AppendLine(" INNER JOIN Mov_Dettaglio_Tecnico_Extra Movimento_Extra ON Movimento_Extra.PIVA = Mov_Contabile.PIVA AND Movimento_Extra.Sa_Cod = Mov_Contabile.Sa_Cod AND Movimento_Extra.Id_Agenda = Mov_Contabile.Id_Agenda AND Movimento_Extra.Id_Mov = Mov_Contabile.Id_Mov ")
            'StbSQL.AppendLine("  ")
            stbSql.AppendLine(" INNER JOIN Movimenti Mov_Magazzino ON Agenda.PIVA = Mov_Magazzino.PIVA AND Agenda.Sa_Cod = Mov_Magazzino.Sa_Cod AND Agenda.Id_Agenda = Mov_Magazzino.Id_Agenda  ")
            stbSql.AppendLine(" INNER JOIN Movimenti_Dettagli Mov_Dett_Magazzino ON Mov_Magazzino.PIVA = Mov_Dett_Magazzino.PIVA AND Mov_Magazzino.Id_Agenda = Mov_Dett_Magazzino.Id_Agenda AND Mov_Magazzino.Id_Mov = Mov_Dett_Magazzino.Id_Mov  ")
            stbSql.AppendLine(" INNER JOIN Mov_Destinazioni ON Mov_Destinazioni.PIVA = Mov_Dett_Magazzino.PIVA AND Mov_Destinazioni.Id_Agenda = Mov_Dett_Magazzino.Id_Agenda AND Mov_Destinazioni.Id_Mov = Mov_Dett_Magazzino.Id_Mov AND Mov_Destinazioni.Id_Mov_Det = Mov_Dett_Magazzino.Id_Mov_Det ")

            stbSql.AppendLine(" INNER JOIN IVA_Aliquote ON IVA_Aliquote.Codice = Mov_Dett_Magazzino.Cod_Iva   ")

            If flag_Mov_Dettaglio_Tecnico_Extra = True Then
                stbSql.AppendLine(" LEFT OUTER JOIN Mov_Dettaglio_Tecnico_Extra Movimento_Extra ON Movimento_Extra.PIVA = Mov_Contabile.PIVA AND Movimento_Extra.Sa_Cod = Mov_Contabile.Sa_Cod AND Movimento_Extra.Id_Agenda = Mov_Contabile.Id_Agenda AND Movimento_Extra.Id_Mov = Mov_Contabile.Id_Mov ")
                stbSql.AppendLine("  ")
            End If

            'StbSQL.AppendLine("  ")
            'StbSQL.AppendLine(" INNER JOIN Materie_Prime ON Materie_Prime.Elem_Cod = Mov_Dett_Magazzino.Elem_Cod AND Materie_Prime.Mat_Cod = Mov_Dett_Magazzino.Mat_Cod   ")
            'StbSQL.AppendLine("  ")
            'StbSQL.AppendLine(" INNER JOIN Mov_Dettaglio_Tecnico_Extra Dettagli_Extra ON Dettagli_Extra.PIVA = Mov_Dett_Magazzino.PIVA AND Dettagli_Extra.Sa_Cod = Mov_Dett_Magazzino.Sa_Cod AND Dettagli_Extra.Id_Agenda = Mov_Dett_Magazzino.Id_Agenda AND Dettagli_Extra.Id_Mov = Mov_Dett_Magazzino.Id_Mov AND Dettagli_Extra.Id_Mov_Det = Mov_Dett_Magazzino.Id_Mov_Det ")
            'StbSQL.AppendLine("  ")
            'StbSQL.AppendLine(" INNER JOIN UnitaMisura ON UnitaMisura.Udm_Cod = Mov_Dett_Magazzino.Udm_Cod ")
            'StbSQL.AppendLine("  ")


            '------------------------------------------------------ 
            '-------------------- WHERE ---------------------------
            '------------------------------------------------------
            stbSql.AppendLine(" WHERE Mov_Contabile.Cau_Mov = '" & CAU_REGISTRAZIONI & "' ")
            'i documenti in uscita hanno il cau_mov dello scarico, ma i resi hanno quello del carico 
            'le bolle di conferimento hanno il loro cau_mov
            stbSql.AppendLine(" AND    Mov_Magazzino.Cau_Mov IN ( '" &
                                    CAU_SCARICO & "', '" &
                                    CAU_CARICO & "', '" &
                                    CAU_ABBUONI & "', '" &
                                    CAU_CONFERIMENTO & "', '" &
                                    CAU_CONFERIMENTO_DIVERSI & "', '" &
                                    CAU_ACCETTAZIONE_BENI & "', '" &
                                    CAU_ACCETTAZIONE_BENI_DA_DIVERSI &
                                    "'  ) ")

            If flag_Solo_Prodotti_BdGias = True Then
                stbSql.AppendLine(" AND Mov_Dett_Magazzino.Elem_Cod IN ( " & CARBURANTI & ",   ")
                stbSql.AppendLine("                                     " & FERTILIZZANTI & ",   ")
                stbSql.AppendLine("                                     " & FORMULATI & ",   ")
                stbSql.AppendLine("                                     " & INSETTI & ",   ")
                stbSql.AppendLine("                                     " & TRAPPOLE & ",   ")
                stbSql.AppendLine("                                     " & INNESCHI & ")   ")
            End If

            stbSql.AppendLine(" AND Mov_Contabile.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine) & "   ")
            stbSql.AppendLine(" AND Mov_Contabile.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & "   ")

            If Lav_Cod <> 0 Then
                stbSql.AppendLine(" AND Agenda.Lav_Cod = " & CStr(Lav_Cod) & " ")
            End If

            If Piva <> "" Then
                stbSql.AppendLine(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Id_Agenda <> 0 Then
                stbSql.AppendLine(" AND Agenda.Id_Agenda = " & CStr(Id_Agenda) & " ")
            End If

            If Cod_RisUm <> 0 Then
                stbSql.AppendLine(" AND Mov_Contabile.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
            End If

            If Anno <> 0 Then
                stbSql.AppendLine(" AND Year(Mov_Contabile.Data_Movimento) = " & Agro_SQL_SaveNum(Anno) & "  ")
            End If

            If Data_Movimento <> AGRODATAINIZIO Then
                stbSql.AppendLine(" AND Mov_Contabile.Data_Movimento = " & UCase(Agro_SQL_SaveDate(Data_Movimento)) & "   ")
            End If

            If Scadenza <> AGRODATAFINE And Scadenza <> AGRODATAINIZIO Then
                stbSql.AppendLine(" AND Mov_Contabile.Scadenza = " & UCase(Agro_SQL_SaveDate(Scadenza)) & "   ")
            End If

            If Doc_Numero_Sin.ToUpper <> "XYZ" Then
                stbSql.AppendLine(" AND Mov_Contabile.Doc_Numero_Sin = '" & UCase(Agro_SQL_SaveText(Doc_Numero_Sin)) & "'   ")
            End If

            If Doc_Numero <> 0 Then
                stbSql.AppendLine(" AND Mov_Contabile.Doc_Numero = " & Agro_SQL_SaveNum(Doc_Numero) & "   ")
            End If

            If Doc_Numero_Des.ToUpper <> "XYZ" Then
                stbSql.AppendLine(" AND Mov_Contabile.Doc_Numero_Des = '" & UCase(Agro_SQL_SaveText(Doc_Numero_Des)) & "'   ")
            End If

            If Progr_Protocollo <> 0 Then
                stbSql.AppendLine(" AND Mov_Contabile.Progr_Protocollo = " & Agro_SQL_SaveNum(Progr_Protocollo) & "   ")
            End If

            If Progr_Registrazione <> 0 Then
                stbSql.AppendLine(" AND Mov_Contabile.Progr_Registrazione = " & Agro_SQL_SaveNum(Progr_Registrazione) & "   ")
            End If

            If Data_Registrazione <> AGRODATAINIZIO Then
                stbSql.AppendLine(" AND Mov_Contabile.Data_Registrazione = " & UCase(Agro_SQL_SaveDate(Data_Registrazione)) & "   ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                stbSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------


            stbSql.AppendLine(" ) ")

            If flag_AltriBeniStrumentali = True Then

                stbSql.AppendLine(" UNION ALL ")

                '------------------------------------------------------------------------
                '---- SECONDA PARTE DELL'UNION: dettagli altri beni strumentali ---------
                '------------------------------------------------------------------------
                stbSql.AppendLine(" ( ")
                '------------------------------------------------------ 
                '------------------- SELECT ---------------------------
                '------------------------------------------------------
                stbSql.AppendLine(" SELECT Agenda.PIVA, Agenda.Id_Agenda, Agenda.Lav_Cod, Agenda.des_lib, ")

                stbSql.AppendLine(" Mov_Contabile.Id_Mov AS Id_Mov_Contabile, Mov_Contabile.Data_Movimento, Mov_Contabile.Ora , ")
                stbSql.AppendLine(" Mov_Contabile.Doc_Numero_Sin, Mov_Contabile.Doc_Numero,Mov_Contabile.Doc_Numero_Des, Mov_Contabile.Mov_Desc ,  ")
                stbSql.AppendLine(" Mov_Contabile.Scadenza, Mov_Contabile.Num_Protocollo, Mov_Contabile.Causale_Trasporto, Mov_Contabile.Aspetto, Mov_Contabile.Peso,  ")
                stbSql.AppendLine(" Mov_Contabile.Colli, Mov_Contabile.Extra_Str, Mov_Contabile.Extra_Int, Mov_Contabile.Extra_Date,  ")
                stbSql.AppendLine(" Mov_Contabile.Tipo_Sconto, Mov_Contabile.Natura_Beni, Mov_Contabile.Tara_Veicolo, Mov_Contabile.Tara_Imballi, Mov_Contabile.Tipo_Peso, Mov_Contabile.Modalita, Mov_Contabile.Username_Note,  ")
                stbSql.AppendLine(" Mov_Contabile.Scadenza_Extra, Mov_Contabile.Progr_Protocollo, Mov_Contabile.Progr_Registrazione, Mov_Contabile.Data_Registrazione,  ")
                stbSql.AppendLine(" Mov_Contabile.ChkLayOut_Join_Prodotti, Mov_Contabile.chklayout_bypass_fatturato, Mov_Contabile.ChkLayOut_Peso, Mov_Contabile.ChkLayOut_Prezzo,  ")
                stbSql.AppendLine("  ")
                stbSql.AppendLine(" Mov_Contabile.Cod_RisUm, Contatti_Cliente.Cod_Contatto AS Cod_Contatto_Cliente, Contatti_Cliente.Rag_Soc AS Rag_Soc_Cliente, Contatti_Cliente.Codice_Fiscale AS Codice_Fiscale_Cliente,  ")
                stbSql.AppendLine(" Contatti_Cliente.Id_Cf AS Id_Cf_Cliente, Contatti_Cliente.Nome AS Nome_Cliente, Contatti_Cliente.Cognome AS Cognome_Cliente,  ")
                stbSql.AppendLine("  Mov_Contabile.Cod_IndirizzoRisUm,  ")
                'StbSQL.AppendLine(" Indirizzi_Cliente.ind_des AS ind_des_Cliente, Indirizzi_Cliente.frz_des AS frz_des_Cliente, Indirizzi_Cliente.CAP AS cap_Cliente,  ")
                'StbSQL.AppendLine(" Indirizzi_Cliente.stato AS stato_Cliente, Indirizzi_Cliente.pro_cod_istat AS pro_cod_istat_Cliente,  ")
                'StbSQL.AppendLine(" Indirizzi_Cliente.com_cod_istat AS com_cod_istat_Cliente, Istat_Cliente.LOCALITA AS localita_Cliente, Istat_Cliente.COMUNI_PROV AS comuni_prov_Cliente, ")
                'StbSQL.AppendLine("  ")
                stbSql.AppendLine(" Mov_Contabile.Cod_Destinazione, Mov_Contabile.Cod_IndirizzoDestinazione,  ")
                'StbSQL.AppendLine(" ISNULL(Contatti_DestDiv.Cod_Contatto, '') AS Cod_Contatto_DestDiv, ISNULL(Contatti_DestDiv.Rag_Soc, '') AS Rag_Soc_DestDiv, ISNULL(Contatti_DestDiv.Codice_Fiscale, '') AS Codice_Fiscale_DestDiv,  ")
                'StbSQL.AppendLine(" ISNULL(Contatti_DestDiv.Id_Cf, 0) AS Id_Cf_DestDiv, ISNULL(Contatti_DestDiv.Nome, '') AS Nome_DestDiv, ISNULL(Contatti_DestDiv.Cognome, '') AS Cognome_DestDiv,  ")
                'StbSQL.AppendLine(" ISNULL(Indirizzi_DestDiv.ind_des, '') AS ind_des_DestDiv, ISNULL(Indirizzi_DestDiv.frz_des, '') AS frz_des_DestDiv, ISNULL(Indirizzi_DestDiv.CAP, '') AS cap_DestDiv,  ")
                'StbSQL.AppendLine(" ISNULL(Indirizzi_DestDiv.stato, '') AS stato_DestDiv, ISNULL(Indirizzi_DestDiv.pro_cod_istat, '') AS pro_cod_istat_DestDiv,  ")
                'StbSQL.AppendLine(" ISNULL(Indirizzi_DestDiv.com_cod_istat, '') AS com_cod_istat_DestDiv, ISNULL(Istat_DestDiv.LOCALITA, '') AS localita_DestDiv, ISNULL(Istat_DestDiv.COMUNI_PROV, '') AS comuni_prov_DestDiv, ")
                'StbSQL.AppendLine("  ")
                stbSql.AppendLine(" Mov_Contabile.Mezzo, Mov_Contabile.Cod_Vettore, Mov_Contabile.Cod_IndirizzoVettore,  ")
                'StbSQL.AppendLine(" ISNULL(Contatti_Vettore.Cod_Contatto, '') AS Cod_Contatto_Vettore, ISNULL(Contatti_Vettore.Rag_Soc, '') AS Rag_Soc_Vettore, ISNULL(Contatti_Vettore.Codice_Fiscale, '') AS Codice_Fiscale_Vettore,  ")
                'StbSQL.AppendLine(" ISNULL(Contatti_Vettore.Id_Cf, 0) AS Id_Cf_Vettore, ISNULL(Contatti_Vettore.Nome, '') AS Nome_Vettore, ISNULL(Contatti_Vettore.Cognome, '') AS Cognome_Vettore,  ")
                'StbSQL.AppendLine(" ISNULL(Indirizzi_Vettore.ind_des, '') AS ind_des_Vettore, ISNULL(Indirizzi_Vettore.frz_des, '') AS frz_des_Vettore, ISNULL(Indirizzi_Vettore.CAP, '') AS cap_Vettore,  ")
                'StbSQL.AppendLine(" ISNULL(Indirizzi_Vettore.stato, '') AS stato_Vettore, Indirizzi_Vettore.pro_cod_istat AS pro_cod_istat_Vettore,  ")
                'StbSQL.AppendLine(" Indirizzi_Vettore.com_cod_istat AS com_cod_istat_Vettore, ISNULL(Istat_Vettore.LOCALITA, '') AS localita_Vettore, ISNULL(Istat_Vettore.COMUNI_PROV, '') AS comuni_prov_Vettore,  ")
                'StbSQL.AppendLine("  ")
                ''StbSQL.AppendLine(" Parco_Macchine.Class_Code, Parco_Macchine.Mac_Des, Parco_Macchine.Targa, Parco_Macchine.Telaio, Parco_Macchine.Modello,  ")
                ''StbSQL.AppendLine(" Parco_Macchine.Potenza, Parco_Macchine.Data_Immatricolazione, Parco_Macchine.N_Immatricolazione,  ")
                ''StbSQL.AppendLine(" Parco_Macchine.N_Immatricolazione_Rimorchio, Parco_Macchine.N_Autorizzazione_Trasporto, Parco_Macchine.Data_Rilascio_Autorizzazione, Parco_Macchine.Peso, ")
                ''StbSQL.AppendLine("  ")
                'StbSQL.AppendLine(" Movimento_Extra.Mac_Cod,  Movimento_Extra.Mezzo_Trasporto, Movimento_Extra.Targa AS Targa_2, Movimento_Extra.N_Immatricolazione AS N_Immatricolazione_2, Movimento_Extra.N_Immatricolazione_Rimorchio AS N_Immatricolazione_Rimorchio_2, ")
                'StbSQL.AppendLine(" Movimento_Extra.N_Autorizzazione_Trasporto AS N_Autorizzazione_Trasporto_2, Movimento_Extra.Data_Rilascio_Autorizzazione AS Data_Rilascio_Autorizzazione_2, Movimento_Extra.Peso AS Peso_2, ")
                'StbSQL.AppendLine("  ")
                'StbSQL.AppendLine(" Movimento_Extra.Id_Reg_Dettaglio AS Id_Movimento_Extra, Movimento_Extra.Indicazioni_Complementari, ")
                'StbSQL.AppendLine(" Movimento_Extra.Tipo_Documento, Movimento_Extra.Luogo_Partenza, Movimento_Extra.Luogo_Consegna, Movimento_Extra.Id_Reg_Dettaglio AS Id_Movimento_Extra, Movimento_Extra.Indicazioni_Complementari, ")
                'StbSQL.AppendLine(" Movimento_Extra.Data_Spedizione,  Movimento_Extra.Precisazioni, Movimento_Extra.Annotazioni,  ")
                'StbSQL.AppendLine("  ")
                stbSql.AppendLine(" Mov_Magazzino.Id_Mov AS Id_Mov_Magazzino, Mov_Magazzino.Mov_Desc AS Mov_Desc_Mag, Mov_Magazzino.Data_Movimento AS Data_Mag,Mov_Magazzino.Ora AS Ora_Mag,  ")
                'StbSQL.AppendLine("  ")
                'StbSQL.AppendLine(" Dettagli_Extra.Id_Reg_Dettaglio AS Id_Dettagli_Extra, ISNULL(Dettagli_Extra.Marche_Contenitori, '') AS Marche_Contenitori,  ")
                'StbSQL.AppendLine(" ISNULL(Dettagli_Extra.Num_Contenitori, 0) AS Num_Contenitori, ISNULL(Dettagli_Extra.Des_Contenitori, '') AS Des_Contenitori, ISNULL(Dettagli_Extra.Num_Colli, 0) AS Num_Colli, Dettagli_Extra.Titolo_Alcol, ")
                'StbSQL.AppendLine("  ")
                stbSql.AppendLine(" Mov_Dett_Magazzino.Id_Mov_Det, Mov_Dett_Magazzino.Mov_Det_Des, Mov_Dett_Magazzino.Elem_Cod, Mov_Dett_Magazzino.Pro_Cod, Mov_Dett_Magazzino.Mat_Cod,  ")
                stbSql.AppendLine(" Mov_Dett_Magazzino.Cod_Progetto, Mov_Dett_Magazzino.Fase_Cod, Mov_Dett_Magazzino.Lotto, Mov_Dett_Magazzino.Cal_Cod, Mov_Dett_Magazzino.Udm_Cod, ")
                stbSql.AppendLine(" Mov_Dett_Magazzino.Qta AS Qta, Mov_Dett_Magazzino.Variazione AS Variazione,Mov_Dett_Magazzino.Listino_Cod, Mov_Dett_Magazzino.Tara,  ")
                stbSql.AppendLine(" Mov_Dett_Magazzino.Prezzo_Unitario, Mov_Dett_Magazzino.Prezzo_Unitario_Netto, Mov_Dett_Magazzino.Imponibile, Mov_Dett_Magazzino.Imponibile_Netto,  ")
                stbSql.AppendLine(" Mov_Dett_Magazzino.Jolly_Int, Mov_Dett_Magazzino.Contabilizzato,Mov_Dett_Magazzino.Pendente, Mov_Dett_Magazzino.Prezzo_Effettivo, ")
                stbSql.AppendLine(" Mov_Dett_Magazzino.Cod_Iva, Mov_Dett_Magazzino.Sconto, Mov_Dett_Magazzino.Cod_Conto, Mov_Dett_Magazzino.Extra_Str AS Extra_Str_Dett, Mov_Dett_Magazzino.Extra_Int AS Extra_Int_Dett, ")
                stbSql.AppendLine(" Mov_Dett_Magazzino.Extra_Date AS Extra_Date_Dett, Mov_Dett_Magazzino.Anno, Mov_Dett_Magazzino.Ric_Cod, Mov_Dett_Magazzino.Iva, Mov_Dett_Magazzino.UDM_COD_EXTRA, Mov_Dett_Magazzino.QTA_EXTRA,  ")
                stbSql.AppendLine(" Mov_Dett_Magazzino.Cod_IvaIndetraibile, Mov_Dett_Magazzino.Qta_Extra_Totale, Mov_Dett_Magazzino.ChkLayOut_Hide, ")
                stbSql.AppendLine(" Mov_Dett_Magazzino.sconto_listino, ISNULL(Mov_Dett_Magazzino.sconto_Testo, '') AS Sconto_Testo, Mov_Dett_Magazzino.Sconto_modalita, Mov_Dett_Magazzino.chkiva_manuale, Mov_Dett_Magazzino.Mat_Cod_Alias, Mov_Dett_Magazzino.Mezzo_Det, ")
                stbSql.AppendLine(" IVA_Aliquote.Sigla AS Sigla_IVA, IVA_Aliquote.Aliquota, ")
                '02/05/2012: gli altri beni strumentali non hanno il magazzino
                stbSql.AppendLine(" 0 AS Sa_Cod_Dest, 0 AS Id_Destinazione, 0 AS Tipo_Destinazione, 0 AS Qta_Dest ")
                stbSql.AppendLine("  ")
                If flag_Mov_Dettaglio_Tecnico_Extra = True Then
                    stbSql.AppendLine(" , ISNULL(Movimento_Extra.Id_Gestione_Vettore, 0) AS Id_Gestione_Vettore  ")
                End If
                stbSql.AppendLine("  ")
                stbSql.AppendLine("  ")
                'StbSQL.AppendLine(" ,UnitaMisura.Udm_Des, UnitaMisura.Udm_Sim,   ")
                ''Dettagli_Extra.Codice_Prodotto, Dettagli_Extra.Colore, Dettagli_Extra.Zona_Viticola, Dettagli_Extra.Manipolazioni, 
                'StbSQL.AppendLine(" Materie_Prime.codice_prodotto, Materie_Prime.colore, Materie_Prime.codice_nc, Materie_Prime.manipolazioni ")
                'StbSQL.AppendLine("  ")

                '------------------------------------------------------ 
                '-------------------- FROM ----------------------------
                '------------------------------------------------------
                stbSql.AppendLine(" FROM Agenda  ")

                stbSql.AppendLine(" INNER JOIN Movimenti Mov_Contabile ON Agenda.PIVA = Mov_Contabile.PIVA AND Agenda.Sa_Cod = Mov_Contabile.Sa_Cod AND Agenda.Id_Agenda = Mov_Contabile.Id_Agenda  ")
                stbSql.AppendLine("  ")
                stbSql.AppendLine(" INNER JOIN Risorse_Umane Risorse_Umane_Cliente ON Mov_Contabile.Cod_RisUm = Risorse_Umane_Cliente.Cod_RisUm  ")
                stbSql.AppendLine(" INNER JOIN Contatti Contatti_Cliente ON Risorse_Umane_Cliente.Piva = Contatti_Cliente.Piva AND Risorse_Umane_Cliente.Cod_Contatto = Contatti_Cliente.Cod_Contatto  ")
                'StbSQL.AppendLine(" INNER JOIN Indirizzi Indirizzi_Cliente ON  Mov_Contabile.Cod_IndirizzoRisUm = Indirizzi_Cliente.cod_indirizzo ")
                'StbSQL.AppendLine(" INNER JOIN ISTAT Istat_Cliente ON Indirizzi_Cliente.pro_cod_istat = Istat_Cliente.PROV AND Indirizzi_Cliente.com_cod_istat = Istat_Cliente.COM ")
                stbSql.AppendLine("  ")
                'StbSQL.AppendLine(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_DestDiv ON Mov_Contabile.Cod_Destinazione = Risorse_Umane_DestDiv.Cod_RisUm  ")
                'StbSQL.AppendLine(" LEFT OUTER JOIN Contatti Contatti_DestDiv ON Risorse_Umane_DestDiv.Piva = Contatti_DestDiv.Piva AND Risorse_Umane_DestDiv.Cod_Contatto = Contatti_DestDiv.Cod_Contatto  ")
                'StbSQL.AppendLine(" LEFT OUTER JOIN Indirizzi Indirizzi_DestDiv ON  Mov_Contabile.Cod_IndirizzoDestinazione= Indirizzi_DestDiv.cod_indirizzo ")
                'StbSQL.AppendLine(" LEFT OUTER JOIN ISTAT Istat_DestDiv ON Indirizzi_DestDiv.pro_cod_istat = Istat_DestDiv.PROV AND Indirizzi_DestDiv.com_cod_istat = Istat_DestDiv.COM ")
                'StbSQL.AppendLine("  ")
                'StbSQL.AppendLine(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Vettore ON Mov_Contabile.Cod_Vettore = Risorse_Umane_Vettore.Cod_RisUm  ")
                'StbSQL.AppendLine(" LEFT OUTER JOIN Contatti Contatti_Vettore ON Risorse_Umane_Vettore.Piva = Contatti_Vettore.Piva AND Risorse_Umane_Vettore.Cod_Contatto = Contatti_Vettore.Cod_Contatto  ")
                'StbSQL.AppendLine(" LEFT OUTER JOIN Indirizzi Indirizzi_Vettore ON  Mov_Contabile.Cod_IndirizzoVettore = Indirizzi_Vettore.cod_indirizzo ")
                'StbSQL.AppendLine(" LEFT OUTER JOIN ISTAT Istat_Vettore ON Indirizzi_Vettore.pro_cod_istat = Istat_Vettore.PROV AND Indirizzi_Vettore.com_cod_istat = Istat_Vettore.COM ")
                'StbSQL.AppendLine("  ")
                'StbSQL.AppendLine(" INNER JOIN Mov_Dettaglio_Tecnico_Extra Movimento_Extra ON Movimento_Extra.PIVA = Mov_Contabile.PIVA AND Movimento_Extra.Sa_Cod = Mov_Contabile.Sa_Cod AND Movimento_Extra.Id_Agenda = Mov_Contabile.Id_Agenda AND Movimento_Extra.Id_Mov = Mov_Contabile.Id_Mov ")
                'StbSQL.AppendLine("  ")
                stbSql.AppendLine(" INNER JOIN Movimenti Mov_Magazzino ON Agenda.PIVA = Mov_Magazzino.PIVA AND Agenda.Sa_Cod = Mov_Magazzino.Sa_Cod AND Agenda.Id_Agenda = Mov_Magazzino.Id_Agenda  ")
                stbSql.AppendLine(" INNER JOIN Movimenti_Dettagli Mov_Dett_Magazzino ON Mov_Magazzino.PIVA = Mov_Dett_Magazzino.PIVA AND Mov_Magazzino.Id_Agenda = Mov_Dett_Magazzino.Id_Agenda AND Mov_Magazzino.Id_Mov = Mov_Dett_Magazzino.Id_Mov  ")
                '02/05/2012: gli altri beni strumentali non hanno il magazzino
                ' StbSQL.AppendLine(" INNER JOIN Mov_Destinazioni ON Mov_Destinazioni.PIVA = Mov_Dett_Magazzino.PIVA AND Mov_Destinazioni.Id_Agenda = Mov_Dett_Magazzino.Id_Agenda AND Mov_Destinazioni.Id_Mov = Mov_Dett_Magazzino.Id_Mov AND Mov_Destinazioni.Id_Mov_Det = Mov_Dett_Magazzino.Id_Mov_Det ")

                stbSql.AppendLine(" INNER JOIN IVA_Aliquote ON IVA_Aliquote.Codice = Mov_Dett_Magazzino.Cod_Iva   ")

                If flag_Mov_Dettaglio_Tecnico_Extra = True Then
                    stbSql.AppendLine(" LEFT OUTER JOIN Mov_Dettaglio_Tecnico_Extra Movimento_Extra ON Movimento_Extra.PIVA = Mov_Contabile.PIVA AND Movimento_Extra.Sa_Cod = Mov_Contabile.Sa_Cod AND Movimento_Extra.Id_Agenda = Mov_Contabile.Id_Agenda AND Movimento_Extra.Id_Mov = Mov_Contabile.Id_Mov ")
                    stbSql.AppendLine("  ")
                End If

                'StbSQL.AppendLine("  ")
                'StbSQL.AppendLine(" INNER JOIN Materie_Prime ON Materie_Prime.Elem_Cod = Mov_Dett_Magazzino.Elem_Cod AND Materie_Prime.Mat_Cod = Mov_Dett_Magazzino.Mat_Cod   ")
                'StbSQL.AppendLine("  ")
                'StbSQL.AppendLine(" INNER JOIN Mov_Dettaglio_Tecnico_Extra Dettagli_Extra ON Dettagli_Extra.PIVA = Mov_Dett_Magazzino.PIVA AND Dettagli_Extra.Sa_Cod = Mov_Dett_Magazzino.Sa_Cod AND Dettagli_Extra.Id_Agenda = Mov_Dett_Magazzino.Id_Agenda AND Dettagli_Extra.Id_Mov = Mov_Dett_Magazzino.Id_Mov AND Dettagli_Extra.Id_Mov_Det = Mov_Dett_Magazzino.Id_Mov_Det ")
                'StbSQL.AppendLine("  ")
                'StbSQL.AppendLine(" INNER JOIN UnitaMisura ON UnitaMisura.Udm_Cod = Mov_Dett_Magazzino.Udm_Cod ")
                'StbSQL.AppendLine("  ")


                '------------------------------------------------------ 
                '-------------------- WHERE ---------------------------
                '------------------------------------------------------
                stbSql.AppendLine(" WHERE Mov_Contabile.Cau_Mov = '" & CAU_REGISTRAZIONI & "' ")
                'i documenti in uscita hanno il cau_mov dello scarico, ma i resi hanno quello del carico 
                'le bolle di conferimento hanno il loro cau_mov
                stbSql.AppendLine(" AND    Mov_Magazzino.Cau_Mov IN ( '" &
                                        CAU_SCARICO & "', '" &
                                        CAU_CARICO & "', '" &
                                        CAU_ABBUONI & "', '" &
                                        CAU_CONFERIMENTO & "', '" &
                                        CAU_CONFERIMENTO_DIVERSI & "', '" &
                                        CAU_ACCETTAZIONE_BENI & "', '" &
                                        CAU_ACCETTAZIONE_BENI_DA_DIVERSI &
                                        "'  ) ")

                stbSql.AppendLine(" AND NOT EXISTS ( ")
                stbSql.AppendLine("                 SELECT 1 ")
                stbSql.AppendLine("                 FROM Mov_Destinazioni  ")
                stbSql.AppendLine("                 WHERE Mov_Destinazioni.PIVA = Mov_Dett_Magazzino.PIVA AND Mov_Destinazioni.Id_Agenda = Mov_Dett_Magazzino.Id_Agenda AND Mov_Destinazioni.Id_Mov = Mov_Dett_Magazzino.Id_Mov AND Mov_Destinazioni.Id_Mov_Det = Mov_Dett_Magazzino.Id_Mov_Det ")
                stbSql.AppendLine("                 )")

                If Lav_Cod <> 0 Then
                    stbSql.AppendLine(" AND Agenda.Lav_Cod = " & CStr(Lav_Cod) & " ")
                End If

                If Piva <> "" Then
                    stbSql.AppendLine(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                End If

                If Id_Agenda <> 0 Then
                    stbSql.AppendLine(" AND Agenda.Id_Agenda = " & CStr(Id_Agenda) & " ")
                End If

                If Cod_RisUm <> 0 Then
                    stbSql.AppendLine(" AND Mov_Contabile.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
                End If

                If Anno <> 0 Then
                    stbSql.AppendLine(" AND Year(Mov_Contabile.Data_Movimento) = " & Agro_SQL_SaveNum(Anno) & "  ")
                End If

                If Data_Movimento <> AGRODATAINIZIO Then
                    stbSql.AppendLine(" AND Mov_Contabile.Data_Movimento = " & UCase(Agro_SQL_SaveDate(Data_Movimento)) & "   ")
                End If

                If Scadenza <> AGRODATAFINE And Scadenza <> AGRODATAINIZIO Then
                    stbSql.AppendLine(" AND Mov_Contabile.Scadenza = " & UCase(Agro_SQL_SaveDate(Scadenza)) & "   ")
                End If

                If Doc_Numero_Sin.ToUpper <> "XYZ" Then
                    stbSql.AppendLine(" AND Mov_Contabile.Doc_Numero_Sin = '" & UCase(Agro_SQL_SaveText(Doc_Numero_Sin)) & "'   ")
                End If

                If Doc_Numero <> 0 Then
                    stbSql.AppendLine(" AND Mov_Contabile.Doc_Numero = " & Agro_SQL_SaveNum(Doc_Numero) & "   ")
                End If

                If Doc_Numero_Des.ToUpper <> "XYZ" Then
                    stbSql.AppendLine(" AND Mov_Contabile.Doc_Numero_Des = '" & UCase(Agro_SQL_SaveText(Doc_Numero_Des)) & "'   ")
                End If

                If Progr_Protocollo <> 0 Then
                    stbSql.AppendLine(" AND Mov_Contabile.Progr_Protocollo = " & Agro_SQL_SaveNum(Progr_Protocollo) & "   ")
                End If

                If Progr_Registrazione <> 0 Then
                    stbSql.AppendLine(" AND Mov_Contabile.Progr_Registrazione = " & Agro_SQL_SaveNum(Progr_Registrazione) & "   ")
                End If

                If Data_Registrazione <> AGRODATAINIZIO Then
                    stbSql.AppendLine(" AND Mov_Contabile.Data_Registrazione = " & UCase(Agro_SQL_SaveDate(Data_Registrazione)) & "   ")
                End If

                '--------------------------------------------------------------------------
                If xFiltroAggiuntivo <> "" Then
                    stbSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                End If
                stbSql.AppendLine(" ) ")

            End If

            '------------------------------------------------------------------------
            '---- FINE UNION ---------
            '------------------------------------------------------------------------
            stbSql.AppendLine(" ) DOCUMENTI ")

            If xOrderBy <> "" Then
                stbSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                stbSql.AppendLine(" ORDER BY Id_Mov_Det ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function

    Public Function NuovoProgressivoLavCod(ByVal lavCod As Integer,
                                           ByVal piva As String,
                                           ByVal anno As Integer,
                                           ByVal sezionaleCod As Integer,
                                           ByVal idAgendaEscluso As Integer,
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                           ) As Integer

        Dim lastProgrProtocollo As Integer = 0

        Select Case True

            Case _lavCodProtVendite.Contains(lavCod)

                lastProgrProtocollo = LastProgressivoVendite(piva,
                                                             anno,
                                                             sezionaleCod,
                                                             idAgendaEscluso,
                                                             objParametri) + 1

            Case _lavCodProtAcquisti.Contains(lavCod)

                lastProgrProtocollo = LastProgressivoAcquisto(piva,
                                                              anno,
                                                              sezionaleCod,
                                                              idAgendaEscluso,
                                                              objParametri) + 1

            Case _lavCodProtMagazzino.Contains(lavCod)

                lastProgrProtocollo = LastProgressivoMagazzino(piva,
                                                               anno,
                                                               sezionaleCod,
                                                               idAgendaEscluso,
                                                               objParametri) + 1

        End Select

        Return lastProgrProtocollo

    End Function

    Public Function LastProgressivoVendite(ByVal piva As String,
                                           ByVal anno As Integer,
                                           ByVal sezionaleCod As Integer,
                                           ByVal idAgendaEscluso As Integer,
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                           ) As Integer

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Contabilita.LastProgressivoVendite"

        Dim messaggioErrore As String = ""
        Dim lastProgrProtocollo As Integer = 0

        Try

            lastProgrProtocollo = DeterminaLastProgressivo(_lavCodProtVendite,
                                                           _cauMovProtDocContab,
                                                           piva,
                                                           anno,
                                                           sezionaleCod,
                                                           idAgendaEscluso,
                                                           nomeRoutine,
                                                           objParametri)

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return lastProgrProtocollo

    End Function

    Public Function LastProgressivoAcquisto(ByVal piva As String,
                                            ByVal anno As Integer,
                                            ByVal sezionaleCod As Integer,
                                            ByVal idAgendaEscluso As Integer,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As Integer

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Contabilita.LastProgressivoAcquisto"

        Dim messaggioErrore As String = ""
        Dim lastProgrProtocollo As Integer = 0

        Try

            lastProgrProtocollo = DeterminaLastProgressivo(_lavCodProtAcquisti,
                                                           _cauMovProtDocContab,
                                                           piva,
                                                           anno,
                                                           sezionaleCod,
                                                           idAgendaEscluso,
                                                           nomeRoutine,
                                                           objParametri)

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return lastProgrProtocollo

    End Function

    Public Function LastProgressivoMagazzino(ByVal piva As String,
                                             ByVal anno As Integer,
                                             ByVal sezionaleCod As Integer,
                                             ByVal idAgendaEscluso As Integer,
                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                             ) As Integer

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Contabilita.LastProgressivoMagazzino"

        Dim messaggioErrore As String = ""
        Dim lastProgrProtocollo As Integer = 0

        Try

            lastProgrProtocollo = DeterminaLastProgressivo(_lavCodProtMagazzino,
                                                           _cauMovProtMagazzino,
                                                           piva,
                                                           anno,
                                                           sezionaleCod,
                                                           idAgendaEscluso,
                                                           nomeRoutine,
                                                           objParametri)

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return lastProgrProtocollo

    End Function

    Private Function DeterminaLastProgressivo(ByVal elencoLavCod() As Integer,
                                              ByVal elencoCauMov() As String,
                                              ByVal piva As String,
                                              ByVal anno As Integer,
                                              ByVal sezionaleCod As Integer,
                                              ByVal idAgendaEscluso As Integer,
                                              ByVal nomeRoutine As String,
                                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer

        Dim lastProgrProtocollo As Integer

        Dim dt As DataTable

        Dim stbSql As New StringBuilder

        stbSql.Length = 0

        stbSql.AppendLine("SELECT Top 1 Movimenti.Progr_Protocollo ")
        stbSql.AppendLine("FROM Movimenti ")
        stbSql.AppendLine("INNER JOIN Agenda ON Agenda.Piva = Movimenti.Piva AND Agenda.Id_Agenda = Movimenti.Id_Agenda ")

        'Caricamento filtro CauMov

        stbSql.Append("WHERE Movimenti.Cau_Mov IN (")

        For indice = 0 To (elencoCauMov.Length - 1)
            If indice > 0 Then
                stbSql.Append(",")
            End If
            stbSql.Append("'" & Agro_SQL_SaveText(elencoCauMov(indice).ToString()) & "'")
        Next

        stbSql.AppendLine(") ")

        'Caricamento filtro LavCod

        stbSql.AppendLine("AND Agenda.Lav_Cod IN (" &
                          Agro_SQL_Save_Clausola_IN(String.Join(",", elencoLavCod)) &
                          ") ")

        'Caricamento altri filtri

        stbSql.AppendLine("And Movimenti.Piva = '" & Agro_SQL_SaveText(piva) & "' ")
        stbSql.AppendLine("AND YEAR(Movimenti.Data_Movimento) = " & Agro_SQL_SaveNum(anno) & " ")
        stbSql.AppendLine("AND Movimenti.Sezionale_Cod = " & Agro_SQL_SaveNum(sezionaleCod) & " ")
        stbSql.AppendLine("AND Agenda.Id_Agenda <> " & Agro_SQL_SaveNum(idAgendaEscluso) & " ")
        stbSql.AppendLine("ORDER BY Progr_Protocollo DESC ")

        '--------------------------------------------------------------------------
        dt = EseguiQuery_Lettura(objParametri, stbSql.ToString, nomeRoutine)
        '--------------------------------------------------------------------------

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 AndAlso IsNumeric(dt.Rows(0).Item("Progr_Protocollo")) Then
            lastProgrProtocollo = CInt(dt.Rows(0).Item("Progr_Protocollo"))
        End If

        Return lastProgrProtocollo

    End Function

    Public Function LastNumDocumento(ByVal piva As String,
                                     ByVal lavCod As Integer,
                                     ByVal anno As Integer,
                                     ByVal docNumeroSin As String,
                                     ByVal docNumeroDes As String,
                                     ByVal codRisUm As Integer,
                                     ByVal cauMov As String,
                                     ByVal strFiltro As String,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                     Optional ByVal finestraTempInizio As Date = AGRODATAINIZIO,
                                     Optional ByVal finestraTempFine As Date = AGRODATAFINE
                                     ) As Integer

        Const nomeRoutine = "AgronicaCoreContabDAL.Contabilita.LastDocument"

        Dim messaggioErrore As String = ""
        Dim stbSql As New StringBuilder
        Dim dt As DataTable
        Dim lastNumDoc As Integer = 0

        Try

            stbSql.Length = 0

            stbSql.AppendLine(" SELECT Top 1 Movimenti.Doc_Numero ")
            stbSql.AppendLine(" FROM Movimenti ")
            stbSql.AppendLine(" INNER JOIN Agenda ON Agenda.Piva = Movimenti.Piva AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda ")
            stbSql.AppendLine(" WHERE Movimenti.Validita_inizio >= " & Agro_SQL_SaveDate(finestraTempInizio) & " ")
            stbSql.AppendLine(" AND Movimenti.Validita_Fine <= " & Agro_SQL_SaveDate(finestraTempFine) & " ")
            stbSql.AppendLine(" AND Upper(Movimenti.Doc_Numero_Sin) = '" & UCase(Agro_SQL_SaveText(docNumeroSin)) & "' ")
            stbSql.AppendLine(" AND Upper(Movimenti.Doc_Numero_Des) = '" & UCase(Agro_SQL_SaveText(docNumeroDes)) & "' ")

            If piva <> "" Then
                stbSql.AppendLine(" AND Movimenti.Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            If lavCod <> 0 Then

                'In alcuni casi devo raggruppare più lavCod sotto una numerazione identica
                Select Case lavCod

                    Case LAVCOD_FATTURA_EMESSA,
                        LAVCOD_FATTURA_LIQ_CONF_EMESSA,
                        LAVCOD_NOTA_ACCREDITO_EMESSA
                        stbSql.AppendLine(" AND Agenda.Lav_Cod IN ( " & LAVCOD_FATTURA_EMESSA & ", ")
                        stbSql.AppendLine("                         " & LAVCOD_FATTURA_LIQ_CONF_EMESSA & ", ")
                        stbSql.AppendLine("                         " & LAVCOD_NOTA_ACCREDITO_EMESSA & ") ")
                        stbSql.AppendLine(" AND Cau_Mov = '" & CAU_REGISTRAZIONI & "' ")
                        stbSql.AppendLine(" AND TipoDocumento <> 28 ")

                    Case LAVCOD_BOLLA_EMESSA,
                        LAVCOD_CONFERIMENTO_DIVERSI,
                        LAVCOD_DOCO_EMESSO,
                        LAVCOD_DDT_CONTABILIZZATO_EMESSO,
                        LAVCOD_AUTO_DDT_EMESSO
                        stbSql.AppendLine(" AND ((Agenda.Lav_Cod IN (" & LAVCOD_BOLLA_EMESSA & ", ")
                        stbSql.AppendLine("                          " & LAVCOD_CONFERIMENTO_DIVERSI & ", ")
                        stbSql.AppendLine("                          " & LAVCOD_DOCO_EMESSO & ", ")
                        stbSql.AppendLine("                          " & LAVCOD_DDT_CONTABILIZZATO_EMESSO & ", ")
                        stbSql.AppendLine("                          " & LAVCOD_AUTO_DDT_EMESSO & ") AND Cau_Mov = '" & CAU_REGISTRAZIONI & "') ")
                        stbSql.AppendLine("      OR (Agenda.Lav_Cod IN (" & LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE & ") AND Cau_Mov = '" & CAU_REGISTRAZIONE_SECONDARIA & "')) ")

                    Case LAVCOD_ACCETTAZIONE_DIVERSI,
                        LAVCOD_DISTINTA_CARICO_ACCETTAZIONE,
                        LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE
                        stbSql.AppendLine(" AND Agenda.Lav_Cod IN (" & LAVCOD_ACCETTAZIONE_DIVERSI & ", ")
                        stbSql.AppendLine("                        " & LAVCOD_DISTINTA_CARICO_ACCETTAZIONE & ", ")
                        stbSql.AppendLine("                        " & LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE & ") ")
                        stbSql.AppendLine(" AND Cau_Mov = '" & CAU_REGISTRAZIONI & "' ")

                    Case LAVCOD_BOLLA_RICEVUTA
                        stbSql.AppendLine(" AND ((Agenda.Lav_Cod IN (" & LAVCOD_BOLLA_RICEVUTA & ") AND Cau_Mov = '" & CAU_REGISTRAZIONI & "') ")
                        stbSql.AppendLine("      OR (Agenda.Lav_Cod IN (" & LAVCOD_ACCETTAZIONE_DIVERSI & ") AND Cau_Mov = '" & CAU_REGISTRAZIONE_SECONDARIA & "')) ")

                    Case LAVCOD_CONTRATTO_AFFITTO
                        stbSql.AppendLine(" AND Agenda.Lav_Cod IN (" & Agro_SQL_Save_Clausola_IN(lavCod) & ") ")
                        If cauMov = "" Then
                            stbSql.AppendLine(" AND Cau_Mov = '" & CAU_REGISTRAZIONI & "' ")
                        End If

                    Case Else
                        stbSql.AppendLine(" AND Agenda.Lav_Cod IN (" & Agro_SQL_Save_Clausola_IN(lavCod) & ") ")

                End Select
            End If

            If anno <> 0 Then
                stbSql.AppendLine(" AND Year(Movimenti.Data_Movimento) = " & Agro_SQL_SaveNum(anno) & " ")
            End If

            If codRisUm <> 0 Then
                stbSql.AppendLine(" AND Movimenti.Cod_RisUm = " & Agro_SQL_SaveNum(codRisUm) & " ")
            End If

            If cauMov <> "" Then
                stbSql.AppendLine(" AND Movimenti.Cau_Mov = '" & Agro_SQL_SaveText(cauMov) & "' ")
            End If

            If Trim(strFiltro) <> "" Then
                stbSql.AppendLine(" AND " & strFiltro)
            End If

            stbSql.AppendLine(" ORDER BY Movimenti.Doc_Numero DESC ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 AndAlso IsNumeric(dt.Rows(0).Item("Doc_Numero")) Then
                lastNumDoc = CInt(dt.Rows(0).Item("Doc_Numero"))
            End If

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return lastNumDoc

    End Function

    Public Function DocumentoPrecedenteSuccessivo(ByVal tipoDoc As enum_TipoDocumento_Numerazione,
                                                  ByVal numDocDaVerificare As Integer,
                                                  ByVal piva As String,
                                                  ByVal lavCod As Integer,
                                                  ByVal anno As Integer,
                                                  ByVal docNumeroSin As String,
                                                  ByVal docNumeroDes As String,
                                                  ByVal codRisUm As Integer,
                                                  ByVal cauMov As String,
                                                  ByVal strFiltro As String,
                                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                  Optional ByVal finestraTempInizio As Date = AGRODATAINIZIO,
                                                  Optional ByVal finestraTempFine As Date = AGRODATAFINE
                                                  ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Contabilita.DocumentoPrecedenteSuccessivo"

        Dim messaggioErrore As String = ""
        Dim stbSql As New StringBuilder
        Dim dt As DataTable

        Try

            stbSql.Length = 0

            stbSql.AppendLine(" SELECT TOP 1 '" & tipoDoc.ToString() & "' AS Tipo_Doc, ")
            stbSql.AppendLine("              CAST(Movimenti.Data_Movimento as date) as Data_Movimento, ")
            stbSql.AppendLine("              Movimenti.Doc_Numero_Sin, Movimenti.Doc_Numero, Movimenti.Doc_Numero_Des, ")
            stbSql.AppendLine("              Movimenti.Doc_Numero_Visualizzato, ")
            stbSql.AppendLine("              Movimenti.Piva, Movimenti.Id_Agenda, Agenda.Lav_Cod, Movimenti.Cod_RisUm ")
            stbSql.AppendLine(" FROM Movimenti ")
            stbSql.AppendLine(" INNER JOIN Agenda ON Movimenti.Piva = Agenda.Piva AND Movimenti.Sa_Cod = Agenda.Sa_Cod AND Movimenti.Id_Agenda = Agenda.Id_Agenda ")
            stbSql.AppendLine(" WHERE Movimenti.Validita_inizio >= " & Agro_SQL_SaveDate(finestraTempInizio) & " ")
            stbSql.AppendLine(" AND Movimenti.Validita_Fine <= " & Agro_SQL_SaveDate(finestraTempFine) & " ")
            stbSql.AppendLine(" AND Upper(Movimenti.Doc_Numero_Sin) = '" & UCase(Agro_SQL_SaveText(docNumeroSin)) & "' ")
            stbSql.AppendLine(" AND Upper(Movimenti.Doc_Numero_Des) = '" & UCase(Agro_SQL_SaveText(docNumeroDes)) & "' ")

            If piva <> "" Then
                stbSql.AppendLine(" AND Movimenti.Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            If lavCod <> 0 Then
                'In alcuni casi devo raggruppare più lavCod sotto una numerazione identica
                Select Case lavCod
                    Case LAVCOD_FATTURA_EMESSA,
                        LAVCOD_FATTURA_LIQ_CONF_EMESSA,
                        LAVCOD_NOTA_ACCREDITO_EMESSA
                        stbSql.AppendLine(" AND Agenda.Lav_Cod IN ( " & LAVCOD_FATTURA_EMESSA & ", ")
                        stbSql.AppendLine("                         " & LAVCOD_FATTURA_LIQ_CONF_EMESSA & ", ")
                        stbSql.AppendLine("                         " & LAVCOD_NOTA_ACCREDITO_EMESSA & ") ")
                        stbSql.AppendLine(" AND Movimenti.Cau_Mov = '" & CAU_REGISTRAZIONI & "' ")
                        stbSql.AppendLine(" AND Movimenti.TipoDocumento <> 28 ")

                    Case LAVCOD_BOLLA_EMESSA,
                        LAVCOD_CONFERIMENTO_DIVERSI,
                        LAVCOD_DOCO_EMESSO,
                        LAVCOD_DDT_CONTABILIZZATO_EMESSO,
                        LAVCOD_AUTO_DDT_EMESSO
                        stbSql.AppendLine(" AND ((Agenda.Lav_Cod IN (" & LAVCOD_BOLLA_EMESSA & ", ")
                        stbSql.AppendLine("                          " & LAVCOD_CONFERIMENTO_DIVERSI & ", ")
                        stbSql.AppendLine("                          " & LAVCOD_DOCO_EMESSO & ", ")
                        stbSql.AppendLine("                          " & LAVCOD_DDT_CONTABILIZZATO_EMESSO & ", ")
                        stbSql.AppendLine("                          " & LAVCOD_AUTO_DDT_EMESSO & ") AND Movimenti.Cau_Mov = '" & CAU_REGISTRAZIONI & "') ")
                        stbSql.AppendLine("      OR (Agenda.Lav_Cod IN (" & LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE & ") AND Movimenti.Cau_Mov = '" & CAU_REGISTRAZIONE_SECONDARIA & "')) ")

                    Case LAVCOD_ACCETTAZIONE_DIVERSI,
                        LAVCOD_DISTINTA_CARICO_ACCETTAZIONE,
                        LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE
                        stbSql.AppendLine(" AND Agenda.Lav_Cod IN (" & LAVCOD_ACCETTAZIONE_DIVERSI & ", ")
                        stbSql.AppendLine("                        " & LAVCOD_DISTINTA_CARICO_ACCETTAZIONE & ", ")
                        stbSql.AppendLine("                        " & LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE & ") ")
                        stbSql.AppendLine(" AND Cau_Mov = '" & CAU_REGISTRAZIONI & "' ")

                    Case LAVCOD_BOLLA_RICEVUTA
                        stbSql.AppendLine(" AND ((Agenda.Lav_Cod IN (" & LAVCOD_BOLLA_RICEVUTA & ") AND Movimenti.Cau_Mov = '" & CAU_REGISTRAZIONI & "') ")
                        stbSql.AppendLine("      OR (Agenda.Lav_Cod IN (" & LAVCOD_ACCETTAZIONE_DIVERSI & ") AND Movimenti.Cau_Mov = '" & CAU_REGISTRAZIONE_SECONDARIA & "')) ")

                    Case Else
                        stbSql.AppendLine(" AND Agenda.Lav_Cod IN (" & Agro_SQL_Save_Clausola_IN(lavCod) & ") ")

                        If cauMov <> "" Then
                            stbSql.AppendLine(" AND Movimenti.Cau_Mov = '" & Agro_SQL_SaveText(cauMov) & "' ")
                        End If

                End Select
            End If

            If anno <> 0 Then
                stbSql.AppendLine(" AND Year(Movimenti.Data_Movimento) = " & Agro_SQL_SaveNum(anno) & " ")
            End If

            If codRisUm <> 0 Then
                stbSql.AppendLine(" AND Movimenti.Cod_RisUm = " & Agro_SQL_SaveNum(codRisUm) & " ")
            End If

            If Trim(strFiltro) <> "" Then
                stbSql.AppendLine(" AND " & strFiltro)
            End If

            Select Case tipoDoc

                Case enum_TipoDocumento_Numerazione.Precedente

                    stbSql.AppendLine(" AND Movimenti.Doc_Numero < " & Agro_SQL_SaveNum(numDocDaVerificare))
                    stbSql.AppendLine(" ORDER BY Movimenti.Doc_Numero DESC, Movimenti.Data_Movimento DESC ")

                Case enum_TipoDocumento_Numerazione.Successivo

                    stbSql.AppendLine(" AND Movimenti.Doc_Numero > " & Agro_SQL_SaveNum(numDocDaVerificare))
                    stbSql.AppendLine(" ORDER BY Movimenti.Doc_Numero ASC, Movimenti.Data_Movimento ASC ")

                Case Else
                    Throw New Exception("Tipo documento non gestito")
            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function

    Public Function ConteggioTotaliDocumento(ByVal piva As String,
                                             ByVal lavCod As Integer,
                                             ByVal idAgenda As Integer,
                                             ByVal strFiltro As String,
                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                             ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Contabilita.ConteggioTotaliDocumento"

        Dim messaggioErrore As String = ""
        Dim stbSql As New StringBuilder
        Dim dt As DataTable

        Try

            stbSql.Length = 0

            stbSql.AppendLine("SELECT Piva, Id_Agenda, Lav_Cod, Data_Doc, Des_Lib, Id_Mov, Cau_Mov, ")
            stbSql.AppendLine("       SUM(Netto) As Netto_Prod_Tot, SUM(Tara) As Tara_Prod_Tot, SUM(Lordo) AS Lordo_Prod_Tot, SUM(Imballi_Vuoti) AS Imballi_Vuoti_Tot, ")
            stbSql.AppendLine("       COUNT(Id_Mov_Det) AS Num_Dettagli, ")
            stbSql.AppendLine("  ")
            stbSql.AppendLine("       ROUND( ")
            stbSql.AppendLine("           SUM( ")
            stbSql.AppendLine("                  (CASE WHEN Num_Contenitori <> 0 THEN Num_Contenitori     -- Num Contenitori in Mov_Det ")
            stbSql.AppendLine("                        WHEN ContenitoriExtra <> 0 THEN ContenitoriExtra   -- Num Contenitori in Mov_Det_Extra ")
            stbSql.AppendLine("                        WHEN Num_Imballi <> 0 THEN Num_Imballi             -- Num Imballi in Mov_Det ")
            stbSql.AppendLine("                        WHEN ImballiExtra <> 0 THEN ImballiExtra           -- Num Imballi in Mov_Det_Extra ")
            stbSql.AppendLine("                        WHEN Udm_Cod = 38 THEN ABS(Qta)                    -- Udm = Numero (se 210, vuol dire che ho confezioni) ")
            stbSql.AppendLine("                        ELSE                                               -- Se accettazione 0, altrimenti la qta a prescindere dall'udm ")
            stbSql.AppendLine("                           CASE WHEN Lav_Cod IN (1054, 1076, 1078) THEN 0 ")
            stbSql.AppendLine("                                ELSE ABS(Qta) ")
            stbSql.AppendLine("                           END ")
            stbSql.AppendLine("                   END) ")
            stbSql.AppendLine("           ), 0 ")
            stbSql.AppendLine("       ) AS Num_Colli_Auto_Tot ")
            stbSql.AppendLine("       , Tara_Veicolo_DB, Tara_Imballi_DB, Peso_DB ")
            stbSql.AppendLine("       -- , Peso_Set_DB, Num_Colli_DB ")
            stbSql.AppendLine("")
            stbSql.AppendLine(" FROM ( ")
            stbSql.AppendLine("")
            stbSql.AppendLine("   SELECT md.Piva, md.Id_Agenda, a.Lav_Cod, CAST(a.Validita_Inizio AS DATE) AS Data_Doc, a.Des_Lib, ")
            stbSql.AppendLine("          md.Id_Mov, m.Cau_Mov, md.Id_Mov_Det, md.Mov_Det_Des, ")
            stbSql.AppendLine("          (CASE WHEN md.Ordine_Det <> 30000 THEN md.Qta_Extra_Totale ELSE 0 END) AS Netto, ")
            stbSql.AppendLine("          md.Tara, ")
            stbSql.AppendLine("          ((CASE WHEN md.Ordine_Det <> 30000 THEN md.Qta_Extra_Totale ELSE 0 END) + md.Tara) AS Lordo, ")
            stbSql.AppendLine("          (CASE WHEN md.Ordine_Det = 30000 THEN md.Qta_Extra_Totale ELSE 0 END) AS Imballi_Vuoti, ")
            stbSql.AppendLine("          Qta_Dettaglio1 AS Num_Contenitori, Qta_Dettaglio2 AS Num_Imballi, ")
            stbSql.AppendLine("          ISNULL(Num_Colli,0) AS ContenitoriExtra, ISNULL(Num_Contenitori,0) AS ImballiExtra, ")
            stbSql.AppendLine("          md.Udm_Cod, md.Qta ")
            stbSql.AppendLine("          , m_Int.Peso AS Peso_DB, m_Int.Tipo_Peso AS Peso_Set_DB, m_Int.Colli AS Num_Colli_DB ")
            stbSql.AppendLine("          , m_Int.Tara_Veicolo AS Tara_Veicolo_DB, m_Int.Tara_Imballi AS Tara_Imballi_DB ")
            stbSql.AppendLine("  ")
            stbSql.AppendLine("   FROM Movimenti_Dettagli md ")
            stbSql.AppendLine("   INNER JOIN Agenda a ON a.Piva = md.Piva AND a.Id_Agenda = md.Id_Agenda ")
            stbSql.AppendLine("   INNER JOIN Movimenti m ON md.Piva = m.Piva AND md.Id_Agenda = m.Id_Agenda AND md.Id_Mov = m.Id_Mov ")
            stbSql.AppendLine("   INNER JOIN Movimenti m_Int ON md.Piva = m_Int.Piva AND md.Id_Agenda = m_Int.Id_Agenda AND m_Int.Cau_Mov = '4000' ")
            stbSql.AppendLine("   LEFT JOIN Mov_Dettaglio_Tecnico_Extra mde ON md.Piva = mde.Piva AND md.Id_Agenda = mde.Id_Agenda AND md.Id_Mov = mde.Id_Mov AND md.Id_Mov_Det = mde.Id_Mov_Det ")
            stbSql.AppendLine("   WHERE md.Ordine_Det <> 1000 ")
            stbSql.AppendLine("   AND Lav_Cod IN (1000,1001,1002,1003,1020,1021,1025,1031,1053,1054,1063,1069,1071,1072,1075,1076,1077,1078,2002,2003,2004,2006) ")
            stbSql.AppendLine("   AND m.Cau_Mov IN ('7300', '7350') ")

            If lavCod <> 0 Then
                stbSql.AppendLine("   AND a.Lav_Cod = " & Agro_SQL_SaveNum(lavCod) & " ")
            End If

            If piva <> "" Then
                stbSql.AppendLine("   AND md.Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            If idAgenda <> 0 Then
                stbSql.AppendLine("   AND md.Id_Agenda = " & Agro_SQL_SaveNum(idAgenda) & " ")
            End If

            stbSql.AppendLine(" ) DETTAGLI ")
            stbSql.AppendLine("  ")
            stbSql.AppendLine(" WHERE 1 = 1")

            If Trim(strFiltro) <> "" Then
                stbSql.AppendLine(" AND " & strFiltro)
            End If

            stbSql.AppendLine(" GROUP BY Piva, Id_Agenda, Lav_Cod, Des_Lib, Data_Doc, Id_Mov, Cau_Mov ")
            stbSql.AppendLine("          , Tara_Veicolo_DB, Tara_Imballi_DB, Peso_DB ")
            stbSql.AppendLine("          --, Peso_Set_DB, Num_Colli_DB ")

            stbSql.AppendLine(" ORDER BY Data_Doc, Id_Agenda ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function

    Public Function IvaConti(ByRef dic As Dictionary(Of String, Integer),
                             ByVal piva As String,
                             ByVal data_movimento As Date,
                             ByVal cod_risum As Integer,
                             ByVal elem_cod As Integer,
                             ByVal pro_cod As Integer,
                             ByVal mat_cod As Integer,
                             ByVal cau_mov As String,
                             ByVal defIva As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Dictionary(Of String, Integer)

        Const nomeRoutine = "AgronicaCoreContabDAL.Contabilita.IvaConti"

        Dim messaggioErrore As String = ""
        Dim stbSql As New StringBuilder
        Dim stbSql2 As New StringBuilder
        Dim dt As DataTable
        Dim superUser As String = objParametri.PivaSuperUser

        'La parte degli acquisti e quella delle vendite si differenziano solo per i campi conto_economico e conto_patrimoniale restituiti nella parte di Prodotto_Extra_Privata (al momento)
        'Vengono inizialmente definiti 4 parametri che andranno a contenere i campi richiesti ossia: 
        'Codice_Iva, Codice_Iva_Compensazione, Codice_Conto_Economico, Codice_Conto_Patrimoniale

        'Seguendo la logica di ricerca si prosegue eseguendo una query e valorizzando i suddetti parametri, per poi controllare
        'se sono stati effettivamente valorizzati. Se ha successo, allora restituisco i quattro parametri, altrimenti
        'proseguo con il livello di ricerca successivo sovrascrivendo i parametri che, sicuramente, saranno NULL

        'la logica di ricerca è: 
        '-	si cercano conti e Iva da Risorse_Umane 
        '-  se non si trovano si cercano conti e Iva da Contatti 
        '-  se non si trovano si cercano conti e Iva da Prodotti_Extra_Privata 
        '-	se non si trovano i conti nessun default
        '-	se non si trova l'IVA si cerca da Categorie_Configurazione 
        '-  se non si trova l'IVA si cerca  da Utenti_Impostazioni Cod = 712 (passato in input come defIVA)

        Try

            Dim defaultIvaConti As New objDefaultIvaConti()

            If cod_risum <> 0 Then

                'Leggo risorse umane

                LeggiIvaContiDaRisorseUmane(piva, cod_risum, defaultIvaConti, objParametri)

                'Leggo contatti

                If defaultIvaConti.ivaDefault Is Nothing OrElse defaultIvaConti.EsisteContoNullo() Then

                    LeggiIvaContiDaContatti(piva, cod_risum, defaultIvaConti, objParametri)

                End If

            End If

            'Leggo prodotti extra privata

            If defaultIvaConti.EsisteIvaNulla() OrElse defaultIvaConti.EsisteContoNullo() Then

                LeggiIvaContiDaProdotti(piva, elem_cod, pro_cod, mat_cod, cau_mov, defaultIvaConti, objParametri)

            End If

            'Per i conti non ho più nulla da scegliere, quindi proseguo solo con l'IVA

            'Per IVA leggo categoria configurazioni

            If defaultIvaConti.ivaDefault Is Nothing Then

                Dim objCatConfigR As New AgronicaCoreAnagrafeDAL.Categorie_Configurazione_R

                Dim dtCatConfig As DataTable = objCatConfigR.Leggi("",
                                                                   0,
                                                                   elem_cod,
                                                                   0,
                                                                   0,
                                                                   "",
                                                                   "",
                                                                   objParametri)

                If dtCatConfig IsNot Nothing AndAlso dtCatConfig.Rows.Count > 0 Then

                    'Se ho solo una riga prendo quello, se ne ho più di una prendo la mia se c'è, oppure piva superuser

                    If dtCatConfig.Rows.Count = 1 Then

                        SeAggiornaCampoIva(dtCatConfig, "Iva_Cod_Cat_Default", defaultIvaConti.ivaDefault)

                    Else

                        Dim drCatConfig() = dtCatConfig.Select(" Piva = '" & Agro_SQL_SaveText(piva, False) & "'")

                        If drCatConfig.Length = 1 Then

                            SeAggiornaCampoIvaDaRiga(drCatConfig(0), "Iva_Cod_Cat_Default", defaultIvaConti.ivaDefault)

                        Else

                            drCatConfig = dtCatConfig.Select(" Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser, False) & "'")

                            If drCatConfig.Length = 1 Then

                                SeAggiornaCampoIvaDaRiga(drCatConfig(0), "Iva_Cod_Cat_Default", defaultIvaConti.ivaDefault)

                            End If

                        End If

                    End If

                End If

            End If

            'Per IVA leggo utenti impostazione 712

            If defaultIvaConti.ivaDefault Is Nothing AndAlso IsNumeric(defIva) AndAlso defIva <> 0 AndAlso defIva <> -1 Then
                defaultIvaConti.ivaDefault = CInt(defIva)
            End If

            dic.Add("Cod_Iva", If(defaultIvaConti.ivaDefault, 0))
            dic.Add("Cod_Iva_Compensazione", If(defaultIvaConti.ivaCompensazioneDefault, 0))
            dic.Add("Cod_Conto_Economico", If(defaultIvaConti.contoEcoDefault, 0))
            dic.Add("Cod_Conto_Patrimoniale", If(defaultIvaConti.contoPatDefault, 0))

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dic

    End Function

    Private Sub LeggiIvaContiDaRisorseUmane(ByVal piva As String,
                                            ByVal cod_risum As Integer,
                                            ByRef defaultIvaConti As objDefaultIvaConti,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim objRisUmR As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R

        Dim dtRisUm As DataTable = objRisUmR.Leggi(piva,
                                                   "",
                                                   cod_risum,
                                                   0,
                                                   0,
                                                   "",
                                                   True,
                                                   True,
                                                   "",
                                                   "",
                                                   objParametri)

        If dtRisUm IsNot Nothing AndAlso dtRisUm.Rows.Count = 1 Then

            'Recupero IVA

            SeAggiornaCampoIva(dtRisUm, "Cod_Iva_Contatto", defaultIvaConti.ivaDefault)

            'I conti li prendo in considerazione solo se sono io il proprietario del contatto e quindi sono i miei conti

            If dtRisUm.Rows(0).Item("Piva") = piva Then

                SeAggiornaCampoConto(dtRisUm, "Cod_Conto_Econ", defaultIvaConti.contoEcoDefault)

                SeAggiornaCampoConto(dtRisUm, "Cod_Conto_Pat", defaultIvaConti.contoPatDefault)

            End If

        End If

    End Sub

    Private Sub LeggiIvaContiDaContatti(ByVal piva As String,
                                        ByVal cod_risum As Integer,
                                        ByRef defaultIvaConti As objDefaultIvaConti,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim objContattiR As New AgronicaCoreAnagrafeDAL.Contatti_R

        Dim dtContatti As DataTable = objContattiR.Leggi_Distinct_Contatti(piva,
                                                                           "",
                                                                           cod_risum,
                                                                           0,
                                                                           True,
                                                                           ID_CF_NOFILTRO,
                                                                           0,
                                                                           "",
                                                                           True,
                                                                           "",
                                                                           0,
                                                                           0,
                                                                           0,
                                                                           0,
                                                                           0,
                                                                           0,
                                                                           0,
                                                                           "",
                                                                           "",
                                                                           objParametri)

        If dtContatti IsNot Nothing AndAlso dtContatti.Rows.Count = 1 Then

            'Recupero IVA

            If defaultIvaConti.ivaDefault Is Nothing Then
                SeAggiornaCampoIva(dtContatti, "Cod_Iva_Contatto", defaultIvaConti.ivaDefault)
            End If

            'I conti li prendo in considerazione solo se sono io il proprietario del contatto e quindi sono i miei conti

            If dtContatti.Rows(0).Item("Piva") = piva Then

                SeAggiornaCampoConto(dtContatti, "Cod_Conto_Econ", defaultIvaConti.contoEcoDefault)

                SeAggiornaCampoConto(dtContatti, "Cod_Conto_Pat", defaultIvaConti.contoPatDefault)

            End If

        End If

    End Sub

    Private Sub LeggiIvaContiDaProdotti(ByVal piva As String,
                                        ByVal elem_cod As Integer,
                                        ByVal pro_cod As Integer,
                                        ByVal mat_cod As Integer,
                                        ByVal cau_mov As String,
                                        ByRef defaultIvaConti As objDefaultIvaConti,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim objProdottiR As New AgronicaCoreAnagrafeDAL.Prodotti_Extra_Privata_R

        Dim dtProdotti As DataTable = objProdottiR.Leggi(piva,
                                                         mat_cod,
                                                         elem_cod,
                                                         pro_cod,
                                                         Nothing,
                                                         Nothing,
                                                         Nothing,
                                                         Nothing,
                                                         Nothing,
                                                         Nothing,
                                                         "",
                                                         "",
                                                         "",
                                                         Nothing,
                                                         Nothing,
                                                         Nothing,
                                                         Nothing,
                                                         Nothing,
                                                         enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                         "",
                                                         "",
                                                         objParametri)

        If dtProdotti IsNot Nothing AndAlso dtProdotti.Rows.Count = 1 Then

            'Recupero IVA

            If defaultIvaConti.ivaDefault Is Nothing Then
                SeAggiornaCampoIva(dtProdotti, "Cod_Iva", defaultIvaConti.ivaDefault)
            End If

            If defaultIvaConti.ivaCompensazioneDefault Is Nothing Then
                SeAggiornaCampoIva(dtProdotti, "Cod_Iva_Compensazione", defaultIvaConti.ivaCompensazioneDefault)
            End If

            'Recupero i conti giusti

            Dim nomeContoEco As String = ""
            Dim nomeContoPat As String = ""
            Select Case cau_mov
                Case CAU_CARICO
                    nomeContoEco = "Cod_Conto_Economico_Acquisto"
                    nomeContoPat = "Cod_Conto_Patrimoniale_Acquisto"
                Case CAU_SCARICO
                    nomeContoEco = "Cod_Conto_Economico_Vendita"
                    nomeContoPat = "Cod_Conto_Patrimoniale_Vendita"
                Case Else
                    Throw New Exception(String.Format("Il Cau_Mov {0} è errato", cau_mov))
            End Select

            If defaultIvaConti.contoEcoDefault Is Nothing Then
                SeAggiornaCampoConto(dtProdotti, nomeContoEco, defaultIvaConti.contoEcoDefault)
            End If

            If defaultIvaConti.contoPatDefault Is Nothing Then
                SeAggiornaCampoConto(dtProdotti, nomeContoPat, defaultIvaConti.contoPatDefault)
            End If

        End If

    End Sub

    Private Sub SeAggiornaCampoIva(ByVal tabella As DataTable,
                                   ByVal nomeCampo As String,
                                   ByRef campoIva As Integer?)

        SeAggiornaCampoIvaDaRiga(tabella.Rows(0), nomeCampo, campoIva)

    End Sub

    Private Sub SeAggiornaCampoIvaDaRiga(ByVal riga As DataRow,
                                         ByVal nomeCampo As String,
                                         ByRef campoIva As Integer?)

        If Not IsDBNull(riga.Item(nomeCampo)) AndAlso riga.Item(nomeCampo) <> 0 AndAlso riga.Item(nomeCampo) <> -1 Then
            campoIva = CInt(riga.Item(nomeCampo))
        End If

    End Sub

    Private Sub SeAggiornaCampoConto(ByVal tabella As DataTable,
                                     ByVal nomeCampo As String,
                                     ByRef campoConto As Integer?)

        If Not IsDBNull(tabella.Rows(0).Item(nomeCampo)) AndAlso
            tabella.Rows(0).Item(nomeCampo) <> 0 Then
            campoConto = CInt(tabella.Rows(0).Item(nomeCampo))
        End If

    End Sub

    Private Class objDefaultIvaConti

        Public ivaDefault As Integer? = Nothing
        Public ivaCompensazioneDefault As Integer? = Nothing
        Public contoEcoDefault As Integer? = Nothing
        Public contoPatDefault As Integer? = Nothing

        Public Function EsisteContoNullo() As Boolean
            Return IsNothing(contoEcoDefault) OrElse IsNothing(contoPatDefault)
        End Function

        Public Function EsisteIvaNulla() As Boolean
            Return IsNothing(ivaDefault) OrElse IsNothing(ivaCompensazioneDefault)
        End Function

    End Class

End Class


'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§


Public Class Contabilita_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '############################################################
    Public Function Sigpa_ModificaUDM(ByVal id_agenda As Integer,
                                      ByVal id_mov As Integer,
                                      ByVal id_mov_det As Integer,
                                      ByVal udm_Nuova As Integer,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                      ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Contabilita_W.Sigpa_ModificaUDM()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------

            If id_agenda = 0 Or id_mov = 0 Or id_mov_det = 0 Then
                Throw New Exception("Sigpa_ModificaUDM --- id_agenda = 0 Or id_mov = 0 Or id_mov_det = 0 non concesso..")
            End If

            strSql.Length = 0
            strSql.AppendLine(" UPDATE md ")
            strSql.AppendLine(" SET ")
            strSql.AppendLine("   udm_cod = " & udm_Nuova)
            strSql.AppendLine(" , Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.AppendLine(" , Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")

            strSql.AppendLine(" FROM Movimenti_dettagli md ")

            strSql.AppendLine(" WHERE ")
            strSql.AppendLine(" ID_Agenda = " & id_agenda)
            strSql.AppendLine(" AND ID_Mov = " & id_mov)
            strSql.AppendLine(" AND ID_Mov_det = " & id_mov_det)

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

    Public Function MarcaOreEuresys(
        ByRef Piva As String,
        ByRef Id_CDG As Integer,
        ByRef OrigineApp As Integer,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "MarcaOreEuresys()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            Stb.Append(" UPDATE CDG_Testata ")
            Stb.Append(" SET ")
            Stb.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            Stb.Append("         ,Data_Modifica= " & Agro_SQL_SaveDateTime(Date.Now) & " ")
            Stb.Append("         ,OrigineApp = " & Agro_SQL_SaveNum(OrigineApp) & " ")
            Stb.Append(" WHERE   Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            Stb.Append(" AND Id_CDG = " & Agro_SQL_SaveNum(Id_CDG))

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
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
