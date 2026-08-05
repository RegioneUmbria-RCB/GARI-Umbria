Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class AnagraficaAziendale
    Inherits AgronicaCoreDataProvider.DataProvider

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' query della stampa Quadro P, utilizzata anche dalla sezione E della notifica Bio
    ''' composta da 4 UNION (i filtri aggiuntivi sono per ogni query)
    ''' Modifica in data 06/10/2011: aggiunto titolo di possesso e metodo di produzione
    ''' DEFAULT: Flag_LeggiTitoloPossesso = FALSE per stampa quadro P --> Magari può servire in altri contesti
    '''         Flag_LeggiTitoloPossesso = TRUE per sezione E Notifica Bio
    ''' DEFAULT: Flag_LeggiDescTitoloPossessoBio = FALSE per stampa quadro P
    '''         Flag_LeggiDescTitoloPossessoBio = TRUE per sezione E Notifica Bio
    ''' DEFAULT: Flag_LeggiMetodoProduzione = FALSE per stampa quadro P
    '''         Flag_LeggiMetodoProduzione = TRUE per sezione E Notifica Bio
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function Quadro_P(ByVal Piva As String,
                             ByVal Sa_Cod As Integer,
                             ByVal Data_Stampa As String,
                             ByVal Flag_LeggiTitoloPossesso As Boolean,
                             ByVal Flag_LeggiDescTitoloPossessoBio As Boolean,
                             ByVal Flag_LeggiMetodoProduzione As Boolean,
                             ByVal xFiltroAggiuntivo1 As String,
                             ByVal xFiltroAggiuntivo2 As String,
                             ByVal xFiltroAggiuntivo3 As String,
                             ByVal xFiltroAggiuntivo4 As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As DataTable

        'LEGGI COMMENTO SOTTO, NELLA SEZIONE ORDINAMENTO
        ' ByVal xOrderBy As String, _

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.AnagraficaAziendale.Quadro_P"

        Dim MessaggioErrore As String = ""
        Dim StbQ As New Text.StringBuilder
        Dim DT As DataTable
        Dim i As Integer

        Try

            Dim FiltroCentri As String = ""
            If Sa_Cod = 0 Then
                'leggo se ci sono filtri sui centri
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim DtCentriVisibili As DataTable
                DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    For i = 0 To DtCentriVisibili.Rows.Count - 1
                        FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                    Next
                End If
            End If


            StbQ.Length = 0

            'StbQ.Append(" SELECT * " & vbCrLf)
            'StbQ.Append(" FROM " & vbCrLf)

            'StbQ.Append(" ( " & vbCrLf)

            '-----------------------------------------------------------------------
            'PARTICELLE ASSOCIATE AD APPEZZAMENTI: IMPIANTI NORMALI (NON CONSOCIATI)
            '-----------------------------------------------------------------------
            StbQ.Append(" -- PARTICELLE ASSOCIATE AD APPEZZAMENTI (IMPIANTI NORMALI (NON CONSOCIATI)) " & vbCrLf)
            StbQ.Append(" ( " & vbCrLf)
            StbQ.Append(" SELECT DISTINCT dbo.Reg_Impianti.PIVA, dbo.Reg_Impianti.sa_cod, 0 as Campo_Cod, Reg_Impianti.Appezza as Appezza, Reg_Impianti.Id_Reg as Id_Reg," & vbCrLf)
            StbQ.Append(" dbo.ISTAT.LOCALITA, dbo.ParticelleCatastali.Part_Cod, dbo.ParticelleCatastali.PROV, " & vbCrLf)
            StbQ.Append(" dbo.ParticelleCatastali.COM, dbo.ParticelleCatastali.SEZIONE, " & vbCrLf)
            StbQ.Append(" dbo.ParticelleCatastali.FOGLIO, dbo.ParticelleCatastali.NUMERO, dbo.ParticelleCatastali.SUBALTERNO, " & vbCrLf)

            'devo fare il cast da float a int perché nel dataset il campo è definito int
            StbQ.Append(" CONVERT(int,ParticelleCatastali.ETTARI) AS ETTARI_Sup_Cat, " & vbCrLf)
            StbQ.Append(" dbo.ParticelleCatastali.[ARE] AS ARE_Sup_Cat, dbo.ParticelleCatastali.CENTIARE AS CENTIARE_Sup_Cat, " & vbCrLf)
            'devo fare il cast da float a int perché nel dataset il campo è definito int
            StbQ.Append(" CONVERT(int, AppezzamentiXParticelle.SAU_Convenz_Ettari) AS ETTARI_Sup_Util," & vbCrLf)
            StbQ.Append(" dbo.AppezzamentiXParticelle.SAU_Convenz_Are AS ARE_Sup_Util, dbo.AppezzamentiXParticelle.SAU_Convenz_Centiare AS CENTIARE_Sup_Util, " & vbCrLf)

            StbQ.Append(" dbo.Appezzamento.Validita_Inizio, dbo.Appezzamento.Validita_Fine, " & vbCrLf)
            StbQ.Append(" dbo.SpecieVegetali.Veg_Des, Cultivar.Cul_Des, " & vbCrLf)
            StbQ.Append(" dbo.Reg_Impianti.Validita_Inizio AS Inizio_Impianto, dbo.Reg_Impianti.Validita_Fine AS Fine_Impianto " & vbCrLf)

            If Flag_LeggiTitoloPossesso Then
                StbQ.Append(" , ImpresexParticelle.TitoloPossesso " & vbCrLf)
                If Flag_LeggiDescTitoloPossessoBio Then
                    StbQ.Append(" , TitoloPossesso_Sigla + ' - ' + TitoloPossesso_Des AS TitoloPossesso_Sigla_Des " & vbCrLf)
                End If
            End If

            If Flag_LeggiMetodoProduzione Then
                StbQ.Append(" , ISNULL((SELECT TOP 1 Appezzamento_Codici.Val_Cod  ")
                StbQ.Append("           FROM Appezzamento_Codici ")
                StbQ.Append("           WHERE Appezzamento_Codici.piva = '" & Agro_SQL_SaveText(Piva) & "' AND  Appezzamento_Codici.sa_cod = Appezzamento.sa_cod AND Appezzamento_Codici.appezza = Appezzamento.appezza  ")
                StbQ.Append("           AND Appezzamento_Codici.id_cod = " & CStr(enum_CodiciAnagrafe.MetodoDiProduzione) & "  ), '1') AS MetodoProduzione_Cod ")
            End If

            StbQ.Append(" FROM dbo.SpecieVegetali INNER JOIN " & vbCrLf)
            StbQ.Append(" dbo.Cultivar ON dbo.SpecieVegetali.Veg_Cod = dbo.Cultivar.Veg_Cod RIGHT OUTER JOIN " & vbCrLf)
            StbQ.Append(" dbo.ParticelleCatastali INNER JOIN " & vbCrLf)
            StbQ.Append(" dbo.AppezzamentiXParticelle ON dbo.ParticelleCatastali.PROV = dbo.AppezzamentiXParticelle.PROV AND  " & vbCrLf)
            StbQ.Append(" dbo.ParticelleCatastali.COM = dbo.AppezzamentiXParticelle.COM AND dbo.ParticelleCatastali.SEZIONE = dbo.AppezzamentiXParticelle.SEZIONE AND  " & vbCrLf)
            StbQ.Append(" dbo.ParticelleCatastali.FOGLIO = dbo.AppezzamentiXParticelle.FOGLIO AND  " & vbCrLf)
            StbQ.Append(" dbo.ParticelleCatastali.NUMERO = dbo.AppezzamentiXParticelle.NUMERO AND  " & vbCrLf)
            StbQ.Append(" dbo.ParticelleCatastali.SUBALTERNO = dbo.AppezzamentiXParticelle.SUBALTERNO INNER JOIN " & vbCrLf)
            StbQ.Append(" dbo.ISTAT ON dbo.ParticelleCatastali.PROV = dbo.ISTAT.PROV AND dbo.ParticelleCatastali.COM = dbo.ISTAT.COM INNER JOIN " & vbCrLf)
            StbQ.Append(" dbo.Appezzamento ON dbo.AppezzamentiXParticelle.PIVA = dbo.Appezzamento.PIVA AND  " & vbCrLf)
            StbQ.Append(" dbo.AppezzamentiXParticelle.sa_cod = dbo.Appezzamento.sa_cod AND  " & vbCrLf)
            StbQ.Append(" dbo.AppezzamentiXParticelle.APPEZZA = dbo.Appezzamento.APPEZZA INNER JOIN " & vbCrLf)
            StbQ.Append(" dbo.Reg_Impianti ON dbo.Appezzamento.PIVA = dbo.Reg_Impianti.PIVA AND dbo.Appezzamento.sa_cod = dbo.Reg_Impianti.sa_cod AND  " & vbCrLf)
            StbQ.Append(" dbo.Appezzamento.APPEZZA = dbo.Reg_Impianti.APPEZZA ON dbo.Cultivar.Cul_Cod = dbo.Reg_Impianti.CUL_COD " & vbCrLf)

            If Flag_LeggiTitoloPossesso Then
                StbQ.Append(" INNER JOIN ImpresexParticelle " & vbCrLf)
                StbQ.Append(" ON dbo.ImpreseXParticelle.PIVA = dbo.AppezzamentiXParticelle.PIVA AND dbo.ImpreseXParticelle.Sa_Cod = dbo.AppezzamentiXParticelle.Sa_Cod  " & vbCrLf)
                StbQ.Append(" AND dbo.ImpreseXParticelle.PROV = dbo.AppezzamentiXParticelle.PROV AND dbo.ImpreseXParticelle.COM = dbo.AppezzamentiXParticelle.COM AND dbo.ImpreseXParticelle.SEZIONE = dbo.AppezzamentiXParticelle.SEZIONE  " & vbCrLf)
                StbQ.Append(" AND dbo.ImpreseXParticelle.FOGLIO = dbo.AppezzamentiXParticelle.FOGLIO AND dbo.ImpreseXParticelle.NUMERO = dbo.AppezzamentiXParticelle.NUMERO   " & vbCrLf)
                StbQ.Append(" AND dbo.ImpreseXParticelle.SUBALTERNO = dbo.AppezzamentiXParticelle.SUBALTERNO " & vbCrLf)
                If Flag_LeggiDescTitoloPossessoBio Then
                    StbQ.Append(" INNER JOIN BIO_Dati_TitoloPossesso ON ImpresexParticelle.TitoloPossesso = BIO_Dati_TitoloPossesso.TitoloPossesso_Cod " & vbCrLf)
                End If
            End If

            StbQ.Append(" WHERE dbo.AppezzamentiXParticelle.PIVA = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            If Sa_Cod <> 0 Then
                StbQ.Append(" AND dbo.AppezzamentiXParticelle.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & vbCrLf)
            Else
                If FiltroCentri <> "" Then
                    StbQ.Append(" AND AppezzamentiXParticelle.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1)) & ") ")
                End If
            End If

            StbQ.Append(" AND dbo.AppezzamentiXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)
            StbQ.Append(" AND dbo.AppezzamentiXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)

            StbQ.Append(" AND dbo.Appezzamento.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)
            StbQ.Append(" AND dbo.Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)

            StbQ.Append(" AND dbo.Reg_Impianti.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)
            StbQ.Append(" AND dbo.Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)

            If Flag_LeggiTitoloPossesso Then
                StbQ.Append(" AND dbo.ImpreseXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)
                StbQ.Append(" AND dbo.ImpreseXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)
            End If

            'voglio solo gli impianti non consociati
            'i consociati li gestisco con una query apposita (la successiva)
            StbQ.Append(" AND dbo.Reg_Impianti.id_consociazione = 0 " & vbCrLf)

            If xFiltroAggiuntivo1 <> "" Then
                StbQ.Append(" AND " & xFiltroAggiuntivo1)
            End If

            StbQ.Append(" ) " & vbCrLf)

            '///////////////////////////////////////////////////////////////////////////////////////////////

            StbQ.Append("  " & vbCrLf)
            StbQ.Append(" UNION ALL " & vbCrLf)
            StbQ.Append("  " & vbCrLf)

            '///////////////////////////////////////////////////////////////////////////////////////////////


            '-----------------------------------------------------------------------
            'PARTICELLE ASSOCIATE AD APPEZZAMENTI: IMPIANTI CONSOCIATI
            '-----------------------------------------------------------------------
            StbQ.Append(" -- PARTICELLE ASSOCIATE AD APPEZZAMENTI: IMPIANTI CONSOCIATI " & vbCrLf)
            StbQ.Append(" ( " & vbCrLf)
            StbQ.Append(" SELECT DISTINCT dbo.Reg_Impianti.PIVA, dbo.Reg_Impianti.sa_cod, 0 as Campo_Cod, Reg_Impianti.Appezza as Appezza," & vbCrLf)

            'non posso visualizzare l'id_reg dell'impianto, altrimenti non riesco a raggruppare tutti gli impianti della stessa consociazione
            'uso una costante
            StbQ.Append(" 0 AS Id_Reg," & vbCrLf)

            StbQ.Append(" dbo.ISTAT.LOCALITA, dbo.ParticelleCatastali.Part_Cod, dbo.ParticelleCatastali.PROV, " & vbCrLf)
            StbQ.Append(" dbo.ParticelleCatastali.COM, dbo.ParticelleCatastali.SEZIONE, " & vbCrLf)
            StbQ.Append(" dbo.ParticelleCatastali.FOGLIO, dbo.ParticelleCatastali.NUMERO, dbo.ParticelleCatastali.SUBALTERNO, " & vbCrLf)

            'devo fare il cast da float a int perché nel dataset il campo è definito int
            StbQ.Append(" CONVERT(int,ParticelleCatastali.ETTARI) AS ETTARI_Sup_Cat, " & vbCrLf)
            StbQ.Append(" dbo.ParticelleCatastali.[ARE] AS ARE_Sup_Cat, dbo.ParticelleCatastali.CENTIARE AS CENTIARE_Sup_Cat, " & vbCrLf)
            'devo fare il cast da float a int perché nel dataset il campo è definito int
            StbQ.Append(" CONVERT(int, AppezzamentiXParticelle.SAU_Convenz_Ettari) AS ETTARI_Sup_Util," & vbCrLf)
            StbQ.Append(" dbo.AppezzamentiXParticelle.SAU_Convenz_Are AS ARE_Sup_Util, dbo.AppezzamentiXParticelle.SAU_Convenz_Centiare AS CENTIARE_Sup_Util, " & vbCrLf)

            StbQ.Append(" dbo.Appezzamento.Validita_Inizio, dbo.Appezzamento.Validita_Fine, " & vbCrLf)

            'chiamo una funzione del database che concatena specie (varietà) di tutti gli impianti consociati di un appezzamento, attivi alla data di stampa 
            'la funzione viene creata dal migra 244
            StbQ.Append(" dbo.ElencoImpiantiConsociati_from_AppezzaData(Reg_Impianti.Piva, Reg_Impianti.sa_cod, Reg_Impianti.APPEZZA, " & Agro_SQL_SaveDate(Data_Stampa) & ") AS Veg_Des, '' AS Cul_des, " & vbCrLf)

            'come sopra, non posso visualizzare le date degli impianti, altrimenti non riesco a raggruppare tutti gli impianti della stessa consociazione
            'uso delle costanti
            StbQ.Append(" '01/01/1900' AS Inizio_Impianto, '31/12/2100' AS Fine_Impianto " & vbCrLf)

            If Flag_LeggiTitoloPossesso Then
                StbQ.Append(" , ImpresexParticelle.TitoloPossesso " & vbCrLf)
                If Flag_LeggiDescTitoloPossessoBio Then
                    StbQ.Append(" , TitoloPossesso_Sigla + ' - ' + TitoloPossesso_Des AS TitoloPossesso_Sigla_Des " & vbCrLf)
                End If
            End If

            If Flag_LeggiMetodoProduzione Then
                StbQ.Append(" , ISNULL((SELECT TOP 1 Appezzamento_Codici.Val_Cod  ")
                StbQ.Append("           FROM Appezzamento_Codici ")
                StbQ.Append("           WHERE Appezzamento_Codici.piva = '" & Agro_SQL_SaveText(Piva) & "' AND  Appezzamento_Codici.sa_cod = Appezzamento.sa_cod AND Appezzamento_Codici.appezza = Appezzamento.appezza  ")
                StbQ.Append("           AND Appezzamento_Codici.id_cod = " & CStr(enum_CodiciAnagrafe.MetodoDiProduzione) & "  ), '1') AS MetodoProduzione_Cod ")
            End If

            StbQ.Append(" FROM dbo.SpecieVegetali INNER JOIN " & vbCrLf)
            StbQ.Append(" dbo.Cultivar ON dbo.SpecieVegetali.Veg_Cod = dbo.Cultivar.Veg_Cod RIGHT OUTER JOIN " & vbCrLf)
            StbQ.Append(" dbo.ParticelleCatastali INNER JOIN " & vbCrLf)
            StbQ.Append(" dbo.AppezzamentiXParticelle ON dbo.ParticelleCatastali.PROV = dbo.AppezzamentiXParticelle.PROV AND  " & vbCrLf)
            StbQ.Append(" dbo.ParticelleCatastali.COM = dbo.AppezzamentiXParticelle.COM AND dbo.ParticelleCatastali.SEZIONE = dbo.AppezzamentiXParticelle.SEZIONE AND  " & vbCrLf)
            StbQ.Append(" dbo.ParticelleCatastali.FOGLIO = dbo.AppezzamentiXParticelle.FOGLIO AND  " & vbCrLf)
            StbQ.Append(" dbo.ParticelleCatastali.NUMERO = dbo.AppezzamentiXParticelle.NUMERO AND  " & vbCrLf)
            StbQ.Append(" dbo.ParticelleCatastali.SUBALTERNO = dbo.AppezzamentiXParticelle.SUBALTERNO INNER JOIN " & vbCrLf)
            StbQ.Append(" dbo.ISTAT ON dbo.ParticelleCatastali.PROV = dbo.ISTAT.PROV AND dbo.ParticelleCatastali.COM = dbo.ISTAT.COM INNER JOIN " & vbCrLf)
            StbQ.Append(" dbo.Appezzamento ON dbo.AppezzamentiXParticelle.PIVA = dbo.Appezzamento.PIVA AND  " & vbCrLf)
            StbQ.Append(" dbo.AppezzamentiXParticelle.sa_cod = dbo.Appezzamento.sa_cod AND  " & vbCrLf)
            StbQ.Append(" dbo.AppezzamentiXParticelle.APPEZZA = dbo.Appezzamento.APPEZZA INNER JOIN " & vbCrLf)
            StbQ.Append(" dbo.Reg_Impianti ON dbo.Appezzamento.PIVA = dbo.Reg_Impianti.PIVA AND dbo.Appezzamento.sa_cod = dbo.Reg_Impianti.sa_cod AND  " & vbCrLf)
            StbQ.Append(" dbo.Appezzamento.APPEZZA = dbo.Reg_Impianti.APPEZZA ON dbo.Cultivar.Cul_Cod = dbo.Reg_Impianti.CUL_COD " & vbCrLf)

            If Flag_LeggiTitoloPossesso Then
                StbQ.Append(" INNER JOIN ImpresexParticelle " & vbCrLf)
                StbQ.Append(" ON dbo.ImpreseXParticelle.PIVA = dbo.AppezzamentiXParticelle.PIVA AND dbo.ImpreseXParticelle.Sa_Cod = dbo.AppezzamentiXParticelle.Sa_Cod  " & vbCrLf)
                StbQ.Append(" AND dbo.ImpreseXParticelle.PROV = dbo.AppezzamentiXParticelle.PROV AND dbo.ImpreseXParticelle.COM = dbo.AppezzamentiXParticelle.COM AND dbo.ImpreseXParticelle.SEZIONE = dbo.AppezzamentiXParticelle.SEZIONE  " & vbCrLf)
                StbQ.Append(" AND dbo.ImpreseXParticelle.FOGLIO = dbo.AppezzamentiXParticelle.FOGLIO AND dbo.ImpreseXParticelle.NUMERO = dbo.AppezzamentiXParticelle.NUMERO   " & vbCrLf)
                StbQ.Append(" AND dbo.ImpreseXParticelle.SUBALTERNO = dbo.AppezzamentiXParticelle.SUBALTERNO " & vbCrLf)
                If Flag_LeggiDescTitoloPossessoBio Then
                    StbQ.Append(" INNER JOIN BIO_Dati_TitoloPossesso ON ImpresexParticelle.TitoloPossesso = BIO_Dati_TitoloPossesso.TitoloPossesso_Cod " & vbCrLf)
                End If
            End If

            StbQ.Append(" WHERE dbo.AppezzamentiXParticelle.PIVA = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            If Sa_Cod <> 0 Then
                StbQ.Append(" AND dbo.AppezzamentiXParticelle.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & vbCrLf)
            Else
                If FiltroCentri <> "" Then
                    StbQ.Append(" AND AppezzamentiXParticelle.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1)) & ") ")
                End If
            End If

            StbQ.Append(" AND dbo.AppezzamentiXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)
            StbQ.Append(" AND dbo.AppezzamentiXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)

            StbQ.Append(" AND dbo.Appezzamento.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)
            StbQ.Append(" AND dbo.Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)

            StbQ.Append(" AND dbo.Reg_Impianti.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)
            StbQ.Append(" AND dbo.Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)

            If Flag_LeggiTitoloPossesso Then
                StbQ.Append(" AND dbo.ImpreseXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)
                StbQ.Append(" AND dbo.ImpreseXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)
            End If

            'voglio solo gli impianti consociati
            '(quelli 'normali' sono gestiti nella query sopra)
            StbQ.Append(" AND dbo.Reg_Impianti.id_consociazione <> 0 " & vbCrLf)

            If xFiltroAggiuntivo2 <> "" Then
                StbQ.Append(" AND " & xFiltroAggiuntivo2)
            End If

            StbQ.Append(" GROUP BY dbo.Reg_Impianti.PIVA, dbo.Reg_Impianti.sa_cod, Reg_Impianti.Appezza," & vbCrLf)
            StbQ.Append(" dbo.ISTAT.LOCALITA, dbo.ParticelleCatastali.Part_Cod, dbo.ParticelleCatastali.PROV, " & vbCrLf)
            StbQ.Append(" dbo.ParticelleCatastali.COM, dbo.ParticelleCatastali.SEZIONE, " & vbCrLf)
            StbQ.Append(" dbo.ParticelleCatastali.FOGLIO, dbo.ParticelleCatastali.NUMERO, dbo.ParticelleCatastali.SUBALTERNO, " & vbCrLf)
            StbQ.Append(" dbo.ParticelleCatastali.ETTARI , dbo.ParticelleCatastali.[ARE], dbo.ParticelleCatastali.CENTIARE , " & vbCrLf)
            StbQ.Append(" dbo.AppezzamentiXParticelle.SAU_Convenz_Ettari, dbo.AppezzamentiXParticelle.SAU_Convenz_Are, dbo.AppezzamentiXParticelle.SAU_Convenz_Centiare, " & vbCrLf)
            StbQ.Append(" dbo.Appezzamento.Validita_Inizio, dbo.Appezzamento.Validita_Fine, id_consociazione " & vbCrLf)

            If Flag_LeggiTitoloPossesso Then
                StbQ.Append(", dbo.Appezzamento.Sa_Cod, dbo.Appezzamento.Appezza, ImpreseXParticelle.TitoloPossesso, TitoloPossesso_Sigla, TitoloPossesso_Des " & vbCrLf)
            End If

            StbQ.Append(" )" & vbCrLf)

            '///////////////////////////////////////////////////////////////////////////////////////////////

            StbQ.Append("  " & vbCrLf)
            StbQ.Append(" UNION ALL " & vbCrLf)
            StbQ.Append("  " & vbCrLf)

            '///////////////////////////////////////////////////////////////////////////////////////////////

            '------------------------------------------------
            'PARTICELLE ASSOCIATE A CAMPI 
            '------------------------------------------------
            StbQ.Append(" -- PARTICELLE ASSOCIATE A CAMPI  " & vbCrLf)
            StbQ.Append(" ( " & vbCrLf)
            StbQ.Append(" SELECT DISTINCT dbo.CampiXParticelle.PIVA as PIVA, dbo.CampiXParticelle.sa_cod as sa_cod, dbo.CampiXParticelle.Campo_Cod as Campo_Cod, 0 as Appezza, 0 as Id_Reg," & vbCrLf)
            StbQ.Append(" dbo.ISTAT.LOCALITA, dbo.ParticelleCatastali.Part_Cod, dbo.ParticelleCatastali.PROV, " & vbCrLf)
            StbQ.Append(" dbo.ParticelleCatastali.COM, dbo.ParticelleCatastali.SEZIONE, " & vbCrLf)
            StbQ.Append(" dbo.ParticelleCatastali.FOGLIO, dbo.ParticelleCatastali.NUMERO, dbo.ParticelleCatastali.SUBALTERNO, " & vbCrLf)

            'devo fare il cast da float a int perché nel dataset il campo è definito int
            StbQ.Append(" CONVERT(int,ParticelleCatastali.ETTARI) AS ETTARI_Sup_Cat, " & vbCrLf)
            StbQ.Append(" dbo.ParticelleCatastali.[ARE] AS ARE_Sup_Cat, dbo.ParticelleCatastali.CENTIARE AS CENTIARE_Sup_Cat, " & vbCrLf)
            'devo fare il cast da float a int perché nel dataset il campo è definito int
            StbQ.Append(" CONVERT(int, CampiXParticelle.SAU_Convenz_Ettari) AS ETTARI_Sup_Util," & vbCrLf)
            StbQ.Append(" dbo.CampiXParticelle.SAU_Convenz_Are AS ARE_Sup_Util, dbo.CampiXParticelle.SAU_Convenz_Centiare AS CENTIARE_Sup_Util, " & vbCrLf)

            StbQ.Append(" dbo.Campi.Validita_Inizio as Validita_Inizio, dbo.Campi.Validita_Fine as Validita_Fine, " & vbCrLf)

            'chiamo una funzione del database che concatena specie (varietà) di tutti gli impianti di un campo, attivi alla data di stampa 
            'la funzione viene creata dal migra 246
            StbQ.Append(" dbo.ElencoImpianti_from_CampoData(Campi.Piva, Campi.sa_cod, Campi.Campo_Cod, " & Agro_SQL_SaveDate(Data_Stampa) & ") AS Veg_Des, '' AS Cul_des, " & vbCrLf)
            ' StbQ.Append(" dbo.SpecieVegetali.Veg_Des, '' as Cul_Des, " & vbCrLf)

            'Modifica del 07/10/2011: messo un default sulle date, prima era '' e nella stampa del quadrop la specie veniva oscurata
            StbQ.Append(" '01/01/1900' AS Inizio_Impianto, '31/12/2100' AS Fine_Impianto " & vbCrLf)

            If Flag_LeggiTitoloPossesso Then
                StbQ.Append(" , ImpresexParticelle.TitoloPossesso " & vbCrLf)
                If Flag_LeggiDescTitoloPossessoBio Then
                    StbQ.Append(" , TitoloPossesso_Sigla + ' - ' + TitoloPossesso_Des AS TitoloPossesso_Sigla_Des " & vbCrLf)
                End If
            End If
            If Flag_LeggiMetodoProduzione Then
                'prende il metodo produzione del primo appezzamento sotto al campo
                StbQ.Append(" , ISNULL((SELECT TOP 1 Appezzamento_Codici.Val_Cod  ")
                StbQ.Append("           FROM Appezzamento_Codici ")
                StbQ.Append("           INNER JOIN Appezzamento ON Appezzamento.Piva = Appezzamento_Codici.Piva AND Appezzamento.Sa_Cod = Appezzamento_Codici.Sa_Cod AND Appezzamento.Appezza= Appezzamento_Codici.Appezza " & vbCrLf)
                StbQ.Append("           WHERE Appezzamento.piva = Campi.Piva AND Appezzamento.sa_cod = Campi.sa_cod AND Appezzamento.campo_cod = Campi.campo_cod " & vbCrLf)
                StbQ.Append("           AND Appezzamento_Codici.id_cod = " & CStr(enum_CodiciAnagrafe.MetodoDiProduzione) & "  ), '1') AS MetodoProduzione_Cod ")
            End If

            StbQ.Append(" FROM dbo.ParticelleCatastali " & vbCrLf)
            StbQ.Append(" INNER JOIN dbo.CampiXParticelle ON dbo.ParticelleCatastali.PROV = dbo.CampiXParticelle.PROV " & vbCrLf)
            StbQ.Append(" AND dbo.ParticelleCatastali.COM = dbo.CampiXParticelle.COM " & vbCrLf)
            StbQ.Append(" AND dbo.ParticelleCatastali.sezione = dbo.CampiXParticelle.sezione " & vbCrLf)
            StbQ.Append(" AND dbo.ParticelleCatastali.foglio = dbo.CampiXParticelle.foglio " & vbCrLf)
            StbQ.Append(" AND dbo.ParticelleCatastali.numero = dbo.CampiXParticelle.numero " & vbCrLf)
            StbQ.Append(" AND dbo.ParticelleCatastali.subalterno = dbo.CampiXParticelle.subalterno " & vbCrLf)
            StbQ.Append(" INNER JOIN dbo.ISTAT ON dbo.ParticelleCatastali.PROV = dbo.ISTAT.PROV " & vbCrLf)
            StbQ.Append(" AND dbo.ParticelleCatastali.COM = dbo.ISTAT.COM " & vbCrLf)
            StbQ.Append(" INNER JOIN dbo.Campi ON dbo.CampiXParticelle.PIVA = dbo.Campi.PIVA " & vbCrLf)
            StbQ.Append(" AND dbo.CampiXParticelle.sa_cod = dbo.Campi.sa_cod " & vbCrLf)
            StbQ.Append(" AND dbo.CampiXParticelle.Campo_Cod = dbo.Campi.Campo_Cod " & vbCrLf)
            'StbQ.Append(" LEFT OUTER JOIN dbo.SpecieVegetali ON dbo.Campi.Veg_Cod = dbo.SpecieVegetali.Veg_Cod " & vbCrLf)

            If Flag_LeggiTitoloPossesso Then
                StbQ.Append(" INNER JOIN ImpresexParticelle ")
                StbQ.Append(" ON dbo.ImpreseXParticelle.PIVA = dbo.CampiXParticelle.PIVA AND dbo.ImpreseXParticelle.Sa_Cod = dbo.CampiXParticelle.Sa_Cod  ")
                StbQ.Append(" AND dbo.ImpreseXParticelle.PROV = dbo.CampiXParticelle.PROV AND dbo.ImpreseXParticelle.COM = dbo.CampiXParticelle.COM AND dbo.ImpreseXParticelle.SEZIONE = dbo.CampiXParticelle.SEZIONE ")
                StbQ.Append(" AND dbo.ImpreseXParticelle.FOGLIO = dbo.CampiXParticelle.FOGLIO AND dbo.ImpreseXParticelle.NUMERO = dbo.CampiXParticelle.NUMERO  ")
                StbQ.Append(" AND dbo.ImpreseXParticelle.SUBALTERNO = dbo.CampiXParticelle.SUBALTERNO ")
                If Flag_LeggiDescTitoloPossessoBio Then
                    StbQ.Append(" INNER JOIN BIO_Dati_TitoloPossesso ON ImpresexParticelle.TitoloPossesso = BIO_Dati_TitoloPossesso.TitoloPossesso_Cod " & vbCrLf)
                End If
            End If

            StbQ.Append(" WHERE dbo.CampiXParticelle.PIVA = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            If Sa_Cod <> 0 Then
                StbQ.Append(" AND dbo.CampiXParticelle.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & vbCrLf)
            Else
                If FiltroCentri <> "" Then
                    StbQ.Append(" AND CampiXParticelle.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1)) & ") ")
                End If
            End If

            StbQ.Append(" AND dbo.CampiXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)
            StbQ.Append(" AND dbo.CampiXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)

            StbQ.Append(" AND dbo.Campi.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)
            StbQ.Append(" AND dbo.Campi.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)

            If Flag_LeggiTitoloPossesso Then
                StbQ.Append(" AND dbo.ImpreseXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)
                StbQ.Append(" AND dbo.ImpreseXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)
            End If

            StbQ.Append(" AND Dbo.CampiXParticelle.Campo_Cod NOT IN " & vbCrLf)
            StbQ.Append("       (SELECT DISTINCT dbo.Appezzamento.Campo_Cod " & vbCrLf)
            StbQ.Append("       FROM  dbo.AppezzamentiXParticelle INNER JOIN " & vbCrLf)
            StbQ.Append("       dbo.Appezzamento ON dbo.AppezzamentiXParticelle.PIVA = dbo.Appezzamento.PIVA AND " & vbCrLf)
            StbQ.Append("       dbo.AppezzamentiXParticelle.sa_cod = dbo.Appezzamento.sa_cod AND " & vbCrLf)
            StbQ.Append("       dbo.AppezzamentiXParticelle.APPEZZA = dbo.Appezzamento.APPEZZA " & vbCrLf)
            StbQ.Append("       WHERE AppezzamentiXParticelle.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            If Sa_Cod <> 0 Then
                StbQ.Append("   AND AppezzamentiXParticelle.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & " " & vbCrLf)
            Else
                If FiltroCentri <> "" Then
                    StbQ.Append(" AND AppezzamentiXParticelle.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1)) & ") ")
                End If
            End If

            StbQ.Append("       ) " & vbCrLf)

            If xFiltroAggiuntivo3 <> "" Then
                StbQ.Append(" AND " & xFiltroAggiuntivo3)
            End If

            StbQ.Append(" ) " & vbCrLf)

            '///////////////////////////////////////////////////////////////////////////////////////////////

            StbQ.Append("  " & vbCrLf)
            StbQ.Append(" UNION ALL " & vbCrLf)
            StbQ.Append("  " & vbCrLf)

            '///////////////////////////////////////////////////////////////////////////////////////////////

            '------------------------------------------------
            'PARTICELLE NON ASSOCIATE
            '------------------------------------------------
            StbQ.Append(" -- PARTICELLE NON ASSOCIATE " & vbCrLf)
            StbQ.Append(" ( " & vbCrLf)
            StbQ.Append(" SELECT DISTINCT dbo.ImpreseXParticelle.PIVA as PIVA, dbo.ImpreseXParticelle.sa_cod as sa_cod, 0 as Campo_Cod, 0 as Appezza, 0 as Id_Reg," & vbCrLf)
            StbQ.Append(" dbo.ISTAT.LOCALITA, dbo.ParticelleCatastali.Part_Cod, dbo.ParticelleCatastali.PROV, " & vbCrLf)
            StbQ.Append(" dbo.ParticelleCatastali.COM, dbo.ParticelleCatastali.SEZIONE, " & vbCrLf)
            StbQ.Append(" dbo.ParticelleCatastali.FOGLIO, dbo.ParticelleCatastali.NUMERO, dbo.ParticelleCatastali.SUBALTERNO, " & vbCrLf)
            StbQ.Append(" CONVERT(int,ParticelleCatastali.ETTARI) AS ETTARI_Sup_Cat, dbo.ParticelleCatastali.[ARE] AS ARE_Sup_Cat, dbo.ParticelleCatastali.CENTIARE AS CENTIARE_Sup_Cat, " & vbCrLf)
            StbQ.Append(" 0 AS ETTARI_Sup_Util, 0 AS ARE_Sup_Util, 0 AS CENTIARE_Sup_Util, " & vbCrLf)
            StbQ.Append(" dbo.ImpreseXParticelle.Validita_Inizio as Validita_Inizio, dbo.ImpreseXParticelle.Validita_Fine as Validita_Fine, " & vbCrLf)
            StbQ.Append(" '' AS Veg_Des, '' AS Cul_Des, " & vbCrLf)
            StbQ.Append(" '' AS Inizio_Impianto, '' AS Fine_Impianto " & vbCrLf)

            If Flag_LeggiTitoloPossesso Then
                StbQ.Append(" , ImpresexParticelle.TitoloPossesso " & vbCrLf)
                If Flag_LeggiDescTitoloPossessoBio Then
                    StbQ.Append(" , TitoloPossesso_Sigla + ' - ' + TitoloPossesso_Des AS TitoloPossesso_Sigla_Des " & vbCrLf)
                End If
            End If
            If Flag_LeggiMetodoProduzione Then
                StbQ.Append(" , -1  ")
            End If

            StbQ.Append(" FROM dbo.ParticelleCatastali " & vbCrLf)
            StbQ.Append(" INNER JOIN dbo.ImpreseXParticelle ON " & vbCrLf)
            StbQ.Append(" dbo.ParticelleCatastali.PROV = dbo.ImpreseXParticelle.PROV " & vbCrLf)
            StbQ.Append(" AND dbo.ParticelleCatastali.COM = dbo.ImpreseXParticelle.COM " & vbCrLf)
            StbQ.Append(" AND dbo.ParticelleCatastali.sezione = dbo.ImpreseXParticelle.sezione " & vbCrLf)
            StbQ.Append(" AND dbo.ParticelleCatastali.foglio = dbo.ImpreseXParticelle.foglio " & vbCrLf)
            StbQ.Append(" AND dbo.ParticelleCatastali.numero = dbo.ImpreseXParticelle.numero " & vbCrLf)
            StbQ.Append(" AND dbo.ParticelleCatastali.subalterno = dbo.ImpreseXParticelle.subalterno " & vbCrLf)
            StbQ.Append(" INNER JOIN dbo.ISTAT ON dbo.ParticelleCatastali.PROV = dbo.ISTAT.PROV " & vbCrLf)
            StbQ.Append(" AND dbo.ParticelleCatastali.COM = dbo.ISTAT.COM " & vbCrLf)
            'stbQ.Append(" LEFT OUTER JOIN Appezzamento ON Appezzamento.Piva = ImpreseXParticelle.Piva " + vbcrlf )
            'stbQ.Append(" AND Appezzamento.sa_cod = ImpreseXParticelle.sa_cod " + vbcrlf )
            'stbQ.Append(" LEFT OUTER JOIN dbo.Reg_Impianti ON Appezzamento.Piva = Reg_Impianti.Piva " + vbcrlf )
            'stbQ.Append(" AND Appezzamento.sa_cod = Reg_Impianti.sa_cod " + vbcrlf )
            'stbQ.Append(" AND Appezzamento.Appezza = Reg_Impianti.Appezza " + vbcrlf )
            'stbQ.Append(" LEFT OUTER JOIN dbo.Cultivar ON dbo.Reg_Impianti.CUL_COD = dbo.Cultivar.Cul_Cod " + vbcrlf )
            'stbQ.Append(" LEFT OUTER JOIN dbo.SpecieVegetali ON dbo.Cultivar.Veg_Cod = dbo.SpecieVegetali.Veg_Cod " + vbcrlf )

            If Flag_LeggiDescTitoloPossessoBio Then
                StbQ.Append(" INNER JOIN BIO_Dati_TitoloPossesso ON ImpresexParticelle.TitoloPossesso = BIO_Dati_TitoloPossesso.TitoloPossesso_Cod " & vbCrLf)
            End If

            StbQ.Append(" WHERE dbo.ImpreseXParticelle.PIVA = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            If Sa_Cod <> 0 Then
                StbQ.Append(" AND dbo.ImpreseXParticelle.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & vbCrLf)
            Else
                If FiltroCentri <> "" Then
                    StbQ.Append(" AND ImpreseXParticelle.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1)) & ") ")
                End If
            End If
            StbQ.Append(" AND dbo.ImpreseXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)
            StbQ.Append(" AND dbo.ImpreseXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)

            StbQ.Append(" AND NOT EXISTS " & vbCrLf)
            StbQ.Append("         (SELECT * FROM CampiXParticelle      " & vbCrLf)
            StbQ.Append("          WHERE dbo.ParticelleCatastali.PROV = dbo.CampiXParticelle.PROV And dbo.ParticelleCatastali.COM = dbo.CampiXParticelle.COM " & vbCrLf)
            StbQ.Append("          AND dbo.ParticelleCatastali.sezione = dbo.CampiXParticelle.sezione AND dbo.ParticelleCatastali.foglio = dbo.CampiXParticelle.foglio     " & vbCrLf)
            StbQ.Append("          AND dbo.ParticelleCatastali.numero = dbo.CampiXParticelle.numero AND dbo.ParticelleCatastali.subalterno = dbo.CampiXParticelle.subalterno " & vbCrLf)
            StbQ.Append("          AND dbo.CampiXParticelle.PIVA = ImpreseXParticelle.PIVA AND dbo.CampiXParticelle.SA_COD = dbo.ImpreseXParticelle.SA_COD  " & vbCrLf)
            StbQ.Append("          AND dbo.CampiXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)
            StbQ.Append("          AND dbo.CampiXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)
            StbQ.Append("           ) " & vbCrLf)

            StbQ.Append(" AND NOT EXISTS " & vbCrLf)
            StbQ.Append("         (SELECT * FROM AppezzamentiXParticelle      " & vbCrLf)
            StbQ.Append("          WHERE dbo.ParticelleCatastali.PROV = dbo.AppezzamentiXParticelle.PROV And dbo.ParticelleCatastali.COM = dbo.AppezzamentiXParticelle.COM " & vbCrLf)
            StbQ.Append("          AND dbo.ParticelleCatastali.sezione = dbo.AppezzamentiXParticelle.sezione AND dbo.ParticelleCatastali.foglio = dbo.AppezzamentiXParticelle.foglio      " & vbCrLf)
            StbQ.Append("          AND dbo.ParticelleCatastali.numero = dbo.AppezzamentiXParticelle.numero AND dbo.ParticelleCatastali.subalterno = dbo.AppezzamentiXParticelle.subalterno     " & vbCrLf)
            StbQ.Append("          AND dbo.AppezzamentiXParticelle.PIVA = ImpreseXParticelle.PIVA AND dbo.AppezzamentiXParticelle.SA_COD = dbo.ImpreseXParticelle.SA_COD      " & vbCrLf)
            StbQ.Append("          AND dbo.AppezzamentiXParticelle.PROV = ImpreseXParticelle.PROV AND dbo.AppezzamentiXParticelle.COM = dbo.ImpreseXParticelle.COM      " & vbCrLf)
            StbQ.Append("          AND dbo.AppezzamentiXParticelle.SEZIONE = ImpreseXParticelle.SEZIONE AND dbo.AppezzamentiXParticelle.FOGLIO = dbo.ImpreseXParticelle.FOGLIO      " & vbCrLf)
            StbQ.Append("          AND dbo.AppezzamentiXParticelle.NUMERO = ImpreseXParticelle.NUMERO AND dbo.AppezzamentiXParticelle.SUBALTERNO = dbo.ImpreseXParticelle.SUBALTERNO      " & vbCrLf)
            ' StbQ.Append("          AND AppezzamentiXParticelle.PIVA = Appezzamento.PIVA AND AppezzamentiXParticelle.SA_COD = Appezzamento.SA_COD AND ")
            'StbQ.Append("          AppezzamentiXParticelle.APPEZZA = Appezzamento.APPEZZA ")
            StbQ.Append("           AND dbo.AppezzamentiXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)
            StbQ.Append("           AND dbo.AppezzamentiXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)
            'StbQ.Append("          AND dbo.Appezzamento.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Stampa))
            'StbQ.Append("          AND dbo.Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Stampa) & "")
            StbQ.Append("           ) " & vbCrLf)

            'PEZZO SOSTITUITO CON I NOT EXISTS
            'StbQ.Append(" AND dbo.ParticelleCatastali.Part_Cod NOT IN " & vbCrLf)
            'StbQ.Append("       ( " & vbCrLf)
            'StbQ.Append("           ( " & vbCrLf)
            'StbQ.Append("           SELECT Part_Cod FROM ParticelleCatastali, CampiXParticelle " & vbCrLf)
            'StbQ.Append("           WHERE dbo.ParticelleCatastali.PROV = dbo.CampiXParticelle.PROV " & vbCrLf)
            'StbQ.Append(" 	        AND dbo.ParticelleCatastali.COM = dbo.CampiXParticelle.COM " & vbCrLf)
            'StbQ.Append(" 	        AND dbo.ParticelleCatastali.sezione = dbo.CampiXParticelle.sezione " & vbCrLf)
            'StbQ.Append(" 	        AND dbo.ParticelleCatastali.foglio = dbo.CampiXParticelle.foglio " & vbCrLf)
            'StbQ.Append(" 	        AND dbo.ParticelleCatastali.numero = dbo.CampiXParticelle.numero " & vbCrLf)
            'StbQ.Append("           AND dbo.ParticelleCatastali.subalterno = dbo.CampiXParticelle.subalterno " & vbCrLf)
            'StbQ.Append("           AND dbo.CampiXParticelle.PIVA = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            'If Sa_Cod <> 0 Then
            '    StbQ.Append("       AND dbo.CampiXParticelle.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) + vbCrLf)
            'End If
            'StbQ.Append("           AND dbo.CampiXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Stampa) + vbCrLf)
            'StbQ.Append("           AND dbo.CampiXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Stampa) + vbCrLf)

            'StbQ.Append("           ) " & vbCrLf)
            'StbQ.Append("           UNION ALL  ")
            'StbQ.Append("           ( " & vbCrLf)

            'StbQ.Append("           SELECT Part_Cod FROM ParticelleCatastali, AppezzamentiXParticelle " & vbCrLf)
            'StbQ.Append("           , Reg_Impianti " & vbCrLf)
            'StbQ.Append("           WHERE dbo.ParticelleCatastali.PROV = dbo.AppezzamentiXParticelle.PROV " & vbCrLf)
            'StbQ.Append(" 	        AND dbo.ParticelleCatastali.COM = dbo.AppezzamentiXParticelle.COM " & vbCrLf)
            'StbQ.Append(" 	        AND dbo.ParticelleCatastali.sezione = dbo.AppezzamentiXParticelle.sezione " & vbCrLf)
            'StbQ.Append(" 	        AND dbo.ParticelleCatastali.foglio = dbo.AppezzamentiXParticelle.foglio " & vbCrLf)
            'StbQ.Append(" 	        AND dbo.ParticelleCatastali.numero = dbo.AppezzamentiXParticelle.numero " & vbCrLf)
            'StbQ.Append(" 	        AND dbo.ParticelleCatastali.subalterno = dbo.AppezzamentiXParticelle.subalterno " & vbCrLf)
            'StbQ.Append(" 	        AND AppezzamentiXParticelle.Piva = Reg_Impianti.Piva " & vbCrLf)
            'StbQ.Append(" 	        AND AppezzamentiXParticelle.sa_cod = Reg_Impianti.sa_cod " & vbCrLf)
            'StbQ.Append(" 	        AND AppezzamentiXParticelle.Appezza = Reg_Impianti.Appezza " & vbCrLf)
            'StbQ.Append("           AND dbo.AppezzamentiXParticelle.PIVA = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            'If Sa_Cod <> 0 Then
            '    StbQ.Append("       AND dbo.AppezzamentiXParticelle.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) + vbCrLf)
            'End If
            'StbQ.Append("           AND dbo.AppezzamentiXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Stampa) + vbCrLf)
            'StbQ.Append("           AND dbo.AppezzamentiXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Stampa) + vbCrLf)
            'StbQ.Append("           AND dbo.Reg_Impianti.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Stampa) + vbCrLf)
            'StbQ.Append("           AND dbo.Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Stampa) + vbCrLf)
            'StbQ.Append("           ) " & vbCrLf)
            'StbQ.Append("       ) " & vbCrLf)

            If xFiltroAggiuntivo4 <> "" Then
                StbQ.Append(" AND " & xFiltroAggiuntivo4)
            End If
            StbQ.Append(" ) " & vbCrLf)

            '/*************************************************************************************

            'StbQ.Append(" ) QUADRO_P " & vbCrLf)

            '------------------------------------------------------
            '------------------- ORDINAMENTO ----------------------
            '------------------------------------------------------
            'NON SI PUO' GESTIRE L'ORDINE A PAICERE, PERCHE' ESSENDO UNA UNION BISOGNA AFRE ATTENZIONE CON I CAMPI
            '(CI POSSONO ESSERE AMBIGUITA')
            'If xOrderBy <> "" Then
            '    StbQ.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            'Else
            '    StbQ.Append(" ORDER BY ParticelleCatastali.prov, ParticelleCatastali.com, ParticelleCatastali.sezione, ParticelleCatastali.foglio, ParticelleCatastali.numero, ParticelleCatastali.subalterno " & vbCrLf)
            'End If
            StbQ.Append(" ORDER BY ParticelleCatastali.prov, ParticelleCatastali.com, ParticelleCatastali.sezione, ParticelleCatastali.foglio, ParticelleCatastali.numero, ParticelleCatastali.subalterno " & vbCrLf)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StbQ.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    'a differenza della precedente
    '1. se Flag_LeggiTitoloPossesso=true --> visualizza anche la sup condotta
    '3. visualizza anche il sa_nome
    Public Function Quadro_P(ByVal Piva As String,
                             ByVal Sa_Cod As Integer,
                             ByVal Data_Stampa As String,
                             ByVal Flag_LeggiCentro As Boolean,
                             ByVal Flag_SoloOccupate As Boolean,
                             ByVal Flag_LeggiTitoloPossesso As Boolean,
                             ByVal Flag_LeggiDescTitoloPossessoBio As Boolean,
                             ByVal Flag_LeggiMetodoProduzione As Boolean,
                             ByVal xFiltroAggiuntivo1 As String,
                             ByVal xFiltroAggiuntivo2 As String,
                             ByVal xFiltroAggiuntivo3 As String,
                             ByVal xFiltroAggiuntivo4 As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As DataTable

        'LEGGI COMMENTO SOTTO, NELLA SEZIONE ORDINAMENTO
        ' ByVal xOrderBy As String, _

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.AnagraficaAziendale.Quadro_P"

        Dim MessaggioErrore As String = ""
        Dim StbQ As New Text.StringBuilder
        Dim DT As DataTable
        Dim i As Integer

        Try

            Dim FiltroCentri As String = ""
            If Sa_Cod = 0 Then
                'leggo se ci sono filtri sui centri
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim DtCentriVisibili As DataTable
                DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    For i = 0 To DtCentriVisibili.Rows.Count - 1
                        FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                    Next
                End If
            End If


            StbQ.Length = 0

            '-----------------------------------------------------------------------
            'PARTICELLE ASSOCIATE AD APPEZZAMENTI: IMPIANTI NORMALI (NON CONSOCIATI)
            '-----------------------------------------------------------------------
            StbQ.Append(" -- PARTICELLE ASSOCIATE AD APPEZZAMENTI (IMPIANTI NORMALI (NON CONSOCIATI)) " & vbCrLf)
            StbQ.Append(" ( " & vbCrLf)
            StbQ.Append(" SELECT DISTINCT dbo.Reg_Impianti.PIVA, dbo.Reg_Impianti.sa_cod, 0 as Campo_Cod, Reg_Impianti.Appezza as Appezza, Reg_Impianti.Id_Reg as Id_Reg," & vbCrLf)
            StbQ.Append(" dbo.ISTAT.LOCALITA, dbo.ParticelleCatastali.Part_Cod, dbo.ParticelleCatastali.PROV, " & vbCrLf)
            StbQ.Append(" dbo.ParticelleCatastali.COM, dbo.ParticelleCatastali.SEZIONE, " & vbCrLf)
            StbQ.Append(" dbo.ParticelleCatastali.FOGLIO, dbo.ParticelleCatastali.NUMERO, dbo.ParticelleCatastali.SUBALTERNO, " & vbCrLf)

            'devo fare il cast da float a int perché nel dataset il campo è definito int
            StbQ.Append(" CONVERT(int,ParticelleCatastali.ETTARI) AS ETTARI_Sup_Cat, " & vbCrLf)
            StbQ.Append(" dbo.ParticelleCatastali.[ARE] AS ARE_Sup_Cat, dbo.ParticelleCatastali.CENTIARE AS CENTIARE_Sup_Cat, " & vbCrLf)
            'devo fare il cast da float a int perché nel dataset il campo è definito int
            StbQ.Append(" CONVERT(int, AppezzamentiXParticelle.SAU_Convenz_Ettari) AS ETTARI_Sup_Util," & vbCrLf)
            StbQ.Append(" dbo.AppezzamentiXParticelle.SAU_Convenz_Are AS ARE_Sup_Util, dbo.AppezzamentiXParticelle.SAU_Convenz_Centiare AS CENTIARE_Sup_Util, " & vbCrLf)
            StbQ.Append(" AppezzamentiXParticelle.area AS Sup_Util," & vbCrLf)

            StbQ.Append(" dbo.Appezzamento.Validita_Inizio, dbo.Appezzamento.Validita_Fine, " & vbCrLf)
            StbQ.Append(" dbo.SpecieVegetali.Veg_Des, Cultivar.Cul_Des, " & vbCrLf)
            StbQ.Append(" dbo.Reg_Impianti.Validita_Inizio AS Inizio_Impianto, dbo.Reg_Impianti.Validita_Fine AS Fine_Impianto " & vbCrLf)

            If Flag_LeggiTitoloPossesso Then
                StbQ.Append(" , ImpresexParticelle.TitoloPossesso " & vbCrLf)
                StbQ.Append(" , CASE ImpresexParticelle.TitoloPossesso " & vbCrLf)
                StbQ.Append("       WHEN 1 THEN 'P' " & vbCrLf)
                StbQ.Append("       WHEN 2 THEN 'C' " & vbCrLf)
                StbQ.Append("       WHEN 3 THEN 'A' " & vbCrLf)
                StbQ.Append("       WHEN 4 THEN 'A' " & vbCrLf)
                StbQ.Append("       ELSE '' END AS TitoloPossesso_Des_Abbr" & vbCrLf)
                StbQ.Append(" , ImpresexParticelle.Sup_Condotta " & vbCrLf)
                StbQ.Append(" ,floor((ImpresexParticelle.Sup_Condotta/10000)*10000) as Ettari_Sup_Condotta " & vbCrLf)
                StbQ.Append(" , floor((ImpresexParticelle.Sup_Condotta - floor((ImpresexParticelle.Sup_Condotta/10000)*10000))*100) as Are_Sup_Condotta " & vbCrLf)
                StbQ.Append(" , round( (((ImpresexParticelle.Sup_Condotta/10000)*10000)*10000)- ((floor((((ImpresexParticelle.Sup_Condotta/10000)*10000))*100))*100),0) as Centiare_Sup_Condotta " & vbCrLf)
                If Flag_LeggiDescTitoloPossessoBio Then
                    StbQ.Append(" , TitoloPossesso_Sigla + ' - ' + TitoloPossesso_Des AS TitoloPossesso_Sigla_Des " & vbCrLf)
                End If
            End If

            If Flag_LeggiMetodoProduzione Then
                StbQ.Append(" , ISNULL((SELECT TOP 1 Appezzamento_Codici.Val_Cod  ")
                StbQ.Append("           FROM Appezzamento_Codici ")
                StbQ.Append("           WHERE Appezzamento_Codici.piva = '" & Agro_SQL_SaveText(Piva) & "' AND  Appezzamento_Codici.sa_cod = Appezzamento.sa_cod AND Appezzamento_Codici.appezza = Appezzamento.appezza  ")
                StbQ.Append("           AND Appezzamento_Codici.id_cod = " & CStr(enum_CodiciAnagrafe.MetodoDiProduzione) & "  ), '1') AS MetodoProduzione_Cod ")
            End If

            If Flag_LeggiCentro Then
                StbQ.Append(" , Centri_Aziendali.sa_nome ")
            End If

            StbQ.Append(" FROM dbo.SpecieVegetali INNER JOIN " & vbCrLf)
            StbQ.Append(" dbo.Cultivar ON dbo.SpecieVegetali.Veg_Cod = dbo.Cultivar.Veg_Cod RIGHT OUTER JOIN " & vbCrLf)
            StbQ.Append(" dbo.ParticelleCatastali INNER JOIN " & vbCrLf)
            StbQ.Append(" dbo.AppezzamentiXParticelle ON dbo.ParticelleCatastali.PROV = dbo.AppezzamentiXParticelle.PROV AND  " & vbCrLf)
            StbQ.Append(" dbo.ParticelleCatastali.COM = dbo.AppezzamentiXParticelle.COM AND dbo.ParticelleCatastali.SEZIONE = dbo.AppezzamentiXParticelle.SEZIONE AND  " & vbCrLf)
            StbQ.Append(" dbo.ParticelleCatastali.FOGLIO = dbo.AppezzamentiXParticelle.FOGLIO AND  " & vbCrLf)
            StbQ.Append(" dbo.ParticelleCatastali.NUMERO = dbo.AppezzamentiXParticelle.NUMERO AND  " & vbCrLf)
            StbQ.Append(" dbo.ParticelleCatastali.SUBALTERNO = dbo.AppezzamentiXParticelle.SUBALTERNO INNER JOIN " & vbCrLf)
            StbQ.Append(" dbo.ISTAT ON dbo.ParticelleCatastali.PROV = dbo.ISTAT.PROV AND dbo.ParticelleCatastali.COM = dbo.ISTAT.COM INNER JOIN " & vbCrLf)
            StbQ.Append(" dbo.Appezzamento ON dbo.AppezzamentiXParticelle.PIVA = dbo.Appezzamento.PIVA AND  " & vbCrLf)
            StbQ.Append(" dbo.AppezzamentiXParticelle.sa_cod = dbo.Appezzamento.sa_cod AND  " & vbCrLf)
            StbQ.Append(" dbo.AppezzamentiXParticelle.APPEZZA = dbo.Appezzamento.APPEZZA INNER JOIN " & vbCrLf)
            StbQ.Append(" dbo.Reg_Impianti ON dbo.Appezzamento.PIVA = dbo.Reg_Impianti.PIVA AND dbo.Appezzamento.sa_cod = dbo.Reg_Impianti.sa_cod AND  " & vbCrLf)
            StbQ.Append(" dbo.Appezzamento.APPEZZA = dbo.Reg_Impianti.APPEZZA ON dbo.Cultivar.Cul_Cod = dbo.Reg_Impianti.CUL_COD " & vbCrLf)

            If Flag_LeggiCentro Then
                StbQ.Append(" INNER JOIN Centri_Aziendali on Appezzamento.PIVA= Centri_Aziendali.PIVA AND Appezzamento.sa_cod=Centri_Aziendali.sa_cod " & vbCrLf)
            End If

            If Flag_LeggiTitoloPossesso Then
                StbQ.Append(" INNER JOIN ImpresexParticelle " & vbCrLf)
                StbQ.Append(" ON dbo.ImpreseXParticelle.PIVA = dbo.AppezzamentiXParticelle.PIVA AND dbo.ImpreseXParticelle.Sa_Cod = dbo.AppezzamentiXParticelle.Sa_Cod  " & vbCrLf)
                StbQ.Append(" AND dbo.ImpreseXParticelle.PROV = dbo.AppezzamentiXParticelle.PROV AND dbo.ImpreseXParticelle.COM = dbo.AppezzamentiXParticelle.COM AND dbo.ImpreseXParticelle.SEZIONE = dbo.AppezzamentiXParticelle.SEZIONE  " & vbCrLf)
                StbQ.Append(" AND dbo.ImpreseXParticelle.FOGLIO = dbo.AppezzamentiXParticelle.FOGLIO AND dbo.ImpreseXParticelle.NUMERO = dbo.AppezzamentiXParticelle.NUMERO   " & vbCrLf)
                StbQ.Append(" AND dbo.ImpreseXParticelle.SUBALTERNO = dbo.AppezzamentiXParticelle.SUBALTERNO " & vbCrLf)
                If Flag_LeggiDescTitoloPossessoBio Then
                    StbQ.Append(" INNER JOIN BIO_Dati_TitoloPossesso ON ImpresexParticelle.TitoloPossesso = BIO_Dati_TitoloPossesso.TitoloPossesso_Cod " & vbCrLf)
                End If
            End If

            StbQ.Append(" WHERE dbo.AppezzamentiXParticelle.PIVA = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            If Sa_Cod <> 0 Then
                StbQ.Append(" AND dbo.AppezzamentiXParticelle.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & vbCrLf)
            Else
                If FiltroCentri <> "" Then
                    StbQ.Append(" AND AppezzamentiXParticelle.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1)) & ") ")
                End If
            End If

            StbQ.Append(" AND dbo.AppezzamentiXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)
            StbQ.Append(" AND dbo.AppezzamentiXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)

            StbQ.Append(" AND dbo.Appezzamento.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)
            StbQ.Append(" AND dbo.Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)

            StbQ.Append(" AND dbo.Reg_Impianti.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)
            StbQ.Append(" AND dbo.Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)

            If Flag_LeggiTitoloPossesso Then
                StbQ.Append(" AND dbo.ImpreseXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)
                StbQ.Append(" AND dbo.ImpreseXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)
            End If

            'voglio solo gli impianti non consociati
            'i consociati li gestisco con una query apposita (la successiva)
            StbQ.Append(" AND dbo.Reg_Impianti.id_consociazione = 0 " & vbCrLf)

            If xFiltroAggiuntivo1 <> "" Then
                StbQ.Append(" AND " & xFiltroAggiuntivo1)
            End If

            StbQ.Append(" ) " & vbCrLf)

            '///////////////////////////////////////////////////////////////////////////////////////////////

            StbQ.Append("  " & vbCrLf)
            StbQ.Append(" UNION ALL " & vbCrLf)
            StbQ.Append("  " & vbCrLf)

            '///////////////////////////////////////////////////////////////////////////////////////////////


            '-----------------------------------------------------------------------
            'PARTICELLE ASSOCIATE AD APPEZZAMENTI: IMPIANTI CONSOCIATI
            '-----------------------------------------------------------------------
            StbQ.Append(" -- PARTICELLE ASSOCIATE AD APPEZZAMENTI: IMPIANTI CONSOCIATI " & vbCrLf)
            StbQ.Append(" ( " & vbCrLf)
            StbQ.Append(" SELECT DISTINCT dbo.Reg_Impianti.PIVA, dbo.Reg_Impianti.sa_cod, 0 as Campo_Cod, Reg_Impianti.Appezza as Appezza," & vbCrLf)

            'non posso visualizzare l'id_reg dell'impianto, altrimenti non riesco a raggruppare tutti gli impianti della stessa consociazione
            'uso una costante
            StbQ.Append(" 0 AS Id_Reg," & vbCrLf)

            StbQ.Append(" dbo.ISTAT.LOCALITA, dbo.ParticelleCatastali.Part_Cod, dbo.ParticelleCatastali.PROV, " & vbCrLf)
            StbQ.Append(" dbo.ParticelleCatastali.COM, dbo.ParticelleCatastali.SEZIONE, " & vbCrLf)
            StbQ.Append(" dbo.ParticelleCatastali.FOGLIO, dbo.ParticelleCatastali.NUMERO, dbo.ParticelleCatastali.SUBALTERNO, " & vbCrLf)

            'devo fare il cast da float a int perché nel dataset il campo è definito int
            StbQ.Append(" CONVERT(int,ParticelleCatastali.ETTARI) AS ETTARI_Sup_Cat, " & vbCrLf)
            StbQ.Append(" dbo.ParticelleCatastali.[ARE] AS ARE_Sup_Cat, dbo.ParticelleCatastali.CENTIARE AS CENTIARE_Sup_Cat, " & vbCrLf)
            'devo fare il cast da float a int perché nel dataset il campo è definito int
            StbQ.Append(" CONVERT(int, AppezzamentiXParticelle.SAU_Convenz_Ettari) AS ETTARI_Sup_Util," & vbCrLf)
            StbQ.Append(" dbo.AppezzamentiXParticelle.SAU_Convenz_Are AS ARE_Sup_Util, dbo.AppezzamentiXParticelle.SAU_Convenz_Centiare AS CENTIARE_Sup_Util, " & vbCrLf)
            StbQ.Append(" AppezzamentiXParticelle.area AS Sup_Util," & vbCrLf)

            StbQ.Append(" dbo.Appezzamento.Validita_Inizio, dbo.Appezzamento.Validita_Fine, " & vbCrLf)

            'chiamo una funzione del database che concatena specie (varietà) di tutti gli impianti consociati di un appezzamento, attivi alla data di stampa 
            'la funzione viene creata dal migra 244
            StbQ.Append(" dbo.ElencoImpiantiConsociati_from_AppezzaData(Reg_Impianti.Piva, Reg_Impianti.sa_cod, Reg_Impianti.APPEZZA, " & Agro_SQL_SaveDate(Data_Stampa) & ") AS Veg_Des, '' AS Cul_des, " & vbCrLf)

            'come sopra, non posso visualizzare le date degli impianti, altrimenti non riesco a raggruppare tutti gli impianti della stessa consociazione
            'uso delle costanti
            StbQ.Append(" '01/01/1900' AS Inizio_Impianto, '31/12/2100' AS Fine_Impianto " & vbCrLf)

            If Flag_LeggiTitoloPossesso Then
                StbQ.Append(" , ImpresexParticelle.TitoloPossesso " & vbCrLf)
                StbQ.Append(" , CASE ImpresexParticelle.TitoloPossesso " & vbCrLf)
                StbQ.Append("       WHEN 1 THEN 'P' " & vbCrLf)
                StbQ.Append("       WHEN 2 THEN 'C' " & vbCrLf)
                StbQ.Append("       WHEN 3 THEN 'A' " & vbCrLf)
                StbQ.Append("       WHEN 4 THEN 'A' " & vbCrLf)
                StbQ.Append("       ELSE '' END AS TitoloPossesso_Des_Abbr" & vbCrLf)
                StbQ.Append(" , ImpresexParticelle.Sup_Condotta " & vbCrLf)
                StbQ.Append(" ,floor((ImpresexParticelle.Sup_Condotta/10000)*10000) as Ettari_Sup_Condotta " & vbCrLf)
                StbQ.Append(" , floor((ImpresexParticelle.Sup_Condotta - floor((ImpresexParticelle.Sup_Condotta/10000)*10000))*100) as Are_Sup_Condotta " & vbCrLf)
                StbQ.Append(" , round( (((ImpresexParticelle.Sup_Condotta/10000)*10000)*10000)- ((floor((((ImpresexParticelle.Sup_Condotta/10000)*10000))*100))*100),0) as Centiare_Sup_Condotta " & vbCrLf)
                If Flag_LeggiDescTitoloPossessoBio Then
                    StbQ.Append(" , TitoloPossesso_Sigla + ' - ' + TitoloPossesso_Des AS TitoloPossesso_Sigla_Des " & vbCrLf)
                End If
            End If

            If Flag_LeggiMetodoProduzione Then
                StbQ.Append(" , ISNULL((SELECT TOP 1 Appezzamento_Codici.Val_Cod  ")
                StbQ.Append("           FROM Appezzamento_Codici ")
                StbQ.Append("           WHERE Appezzamento_Codici.piva = '" & Agro_SQL_SaveText(Piva) & "' AND  Appezzamento_Codici.sa_cod = Appezzamento.sa_cod AND Appezzamento_Codici.appezza = Appezzamento.appezza  ")
                StbQ.Append("           AND Appezzamento_Codici.id_cod = " & CStr(enum_CodiciAnagrafe.MetodoDiProduzione) & "  ), '1') AS MetodoProduzione_Cod ")
            End If

            If Flag_LeggiCentro Then
                StbQ.Append(" , Centri_Aziendali.sa_nome ")
            End If

            StbQ.Append(" FROM dbo.SpecieVegetali INNER JOIN " & vbCrLf)
            StbQ.Append(" dbo.Cultivar ON dbo.SpecieVegetali.Veg_Cod = dbo.Cultivar.Veg_Cod RIGHT OUTER JOIN " & vbCrLf)
            StbQ.Append(" dbo.ParticelleCatastali INNER JOIN " & vbCrLf)
            StbQ.Append(" dbo.AppezzamentiXParticelle ON dbo.ParticelleCatastali.PROV = dbo.AppezzamentiXParticelle.PROV AND  " & vbCrLf)
            StbQ.Append(" dbo.ParticelleCatastali.COM = dbo.AppezzamentiXParticelle.COM AND dbo.ParticelleCatastali.SEZIONE = dbo.AppezzamentiXParticelle.SEZIONE AND  " & vbCrLf)
            StbQ.Append(" dbo.ParticelleCatastali.FOGLIO = dbo.AppezzamentiXParticelle.FOGLIO AND  " & vbCrLf)
            StbQ.Append(" dbo.ParticelleCatastali.NUMERO = dbo.AppezzamentiXParticelle.NUMERO AND  " & vbCrLf)
            StbQ.Append(" dbo.ParticelleCatastali.SUBALTERNO = dbo.AppezzamentiXParticelle.SUBALTERNO INNER JOIN " & vbCrLf)
            StbQ.Append(" dbo.ISTAT ON dbo.ParticelleCatastali.PROV = dbo.ISTAT.PROV AND dbo.ParticelleCatastali.COM = dbo.ISTAT.COM INNER JOIN " & vbCrLf)
            StbQ.Append(" dbo.Appezzamento ON dbo.AppezzamentiXParticelle.PIVA = dbo.Appezzamento.PIVA AND  " & vbCrLf)
            StbQ.Append(" dbo.AppezzamentiXParticelle.sa_cod = dbo.Appezzamento.sa_cod AND  " & vbCrLf)
            StbQ.Append(" dbo.AppezzamentiXParticelle.APPEZZA = dbo.Appezzamento.APPEZZA INNER JOIN " & vbCrLf)
            StbQ.Append(" dbo.Reg_Impianti ON dbo.Appezzamento.PIVA = dbo.Reg_Impianti.PIVA AND dbo.Appezzamento.sa_cod = dbo.Reg_Impianti.sa_cod AND  " & vbCrLf)
            StbQ.Append(" dbo.Appezzamento.APPEZZA = dbo.Reg_Impianti.APPEZZA ON dbo.Cultivar.Cul_Cod = dbo.Reg_Impianti.CUL_COD " & vbCrLf)

            If Flag_LeggiCentro Then
                StbQ.Append(" INNER JOIN Centri_Aziendali on Appezzamento.PIVA= Centri_Aziendali.PIVA AND Appezzamento.sa_cod=Centri_Aziendali.sa_cod " & vbCrLf)
            End If

            If Flag_LeggiTitoloPossesso Then
                StbQ.Append(" INNER JOIN ImpresexParticelle " & vbCrLf)
                StbQ.Append(" ON dbo.ImpreseXParticelle.PIVA = dbo.AppezzamentiXParticelle.PIVA AND dbo.ImpreseXParticelle.Sa_Cod = dbo.AppezzamentiXParticelle.Sa_Cod  " & vbCrLf)
                StbQ.Append(" AND dbo.ImpreseXParticelle.PROV = dbo.AppezzamentiXParticelle.PROV AND dbo.ImpreseXParticelle.COM = dbo.AppezzamentiXParticelle.COM AND dbo.ImpreseXParticelle.SEZIONE = dbo.AppezzamentiXParticelle.SEZIONE  " & vbCrLf)
                StbQ.Append(" AND dbo.ImpreseXParticelle.FOGLIO = dbo.AppezzamentiXParticelle.FOGLIO AND dbo.ImpreseXParticelle.NUMERO = dbo.AppezzamentiXParticelle.NUMERO   " & vbCrLf)
                StbQ.Append(" AND dbo.ImpreseXParticelle.SUBALTERNO = dbo.AppezzamentiXParticelle.SUBALTERNO " & vbCrLf)
                If Flag_LeggiDescTitoloPossessoBio Then
                    StbQ.Append(" INNER JOIN BIO_Dati_TitoloPossesso ON ImpresexParticelle.TitoloPossesso = BIO_Dati_TitoloPossesso.TitoloPossesso_Cod " & vbCrLf)
                End If
            End If

            StbQ.Append(" WHERE dbo.AppezzamentiXParticelle.PIVA = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            If Sa_Cod <> 0 Then
                StbQ.Append(" AND dbo.AppezzamentiXParticelle.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & vbCrLf)
            Else
                If FiltroCentri <> "" Then
                    StbQ.Append(" AND AppezzamentiXParticelle.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1)) & ") ")
                End If
            End If

            StbQ.Append(" AND dbo.AppezzamentiXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)
            StbQ.Append(" AND dbo.AppezzamentiXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)

            StbQ.Append(" AND dbo.Appezzamento.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)
            StbQ.Append(" AND dbo.Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)

            StbQ.Append(" AND dbo.Reg_Impianti.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)
            StbQ.Append(" AND dbo.Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)

            If Flag_LeggiTitoloPossesso Then
                StbQ.Append(" AND dbo.ImpreseXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)
                StbQ.Append(" AND dbo.ImpreseXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)
            End If

            'voglio solo gli impianti consociati
            '(quelli 'normali' sono gestiti nella query sopra)
            StbQ.Append(" AND dbo.Reg_Impianti.id_consociazione <> 0 " & vbCrLf)

            If xFiltroAggiuntivo2 <> "" Then
                StbQ.Append(" AND " & xFiltroAggiuntivo2)
            End If

            StbQ.Append(" GROUP BY dbo.Reg_Impianti.PIVA, dbo.Reg_Impianti.sa_cod, Reg_Impianti.Appezza," & vbCrLf)
            StbQ.Append(" dbo.ISTAT.LOCALITA, dbo.ParticelleCatastali.Part_Cod, dbo.ParticelleCatastali.PROV, " & vbCrLf)
            StbQ.Append(" dbo.ParticelleCatastali.COM, dbo.ParticelleCatastali.SEZIONE, " & vbCrLf)
            StbQ.Append(" dbo.ParticelleCatastali.FOGLIO, dbo.ParticelleCatastali.NUMERO, dbo.ParticelleCatastali.SUBALTERNO, " & vbCrLf)
            StbQ.Append(" dbo.ParticelleCatastali.ETTARI , dbo.ParticelleCatastali.[ARE], dbo.ParticelleCatastali.CENTIARE , " & vbCrLf)
            StbQ.Append(" dbo.AppezzamentiXParticelle.SAU_Convenz_Ettari, dbo.AppezzamentiXParticelle.SAU_Convenz_Are, dbo.AppezzamentiXParticelle.SAU_Convenz_Centiare, dbo.AppezzamentiXParticelle.area, " & vbCrLf)
            StbQ.Append(" dbo.Appezzamento.Validita_Inizio, dbo.Appezzamento.Validita_Fine " & vbCrLf)

            If Flag_LeggiCentro Then
                StbQ.Append(" , Centri_Aziendali.sa_nome " & vbCrLf)
            End If

            StbQ.Append(" , id_consociazione " & vbCrLf)

            If Flag_LeggiTitoloPossesso Then
                StbQ.Append(", dbo.Appezzamento.Sa_Cod, dbo.Appezzamento.Appezza, ImpreseXParticelle.TitoloPossesso, ImpresexParticelle.Sup_Condotta " & vbCrLf)
                If Flag_LeggiDescTitoloPossessoBio Then
                    StbQ.Append(", TitoloPossesso_Sigla, TitoloPossesso_Des " & vbCrLf)
                End If
            End If

            StbQ.Append(" )" & vbCrLf)

            '///////////////////////////////////////////////////////////////////////////////////////////////

            StbQ.Append("  " & vbCrLf)
            StbQ.Append(" UNION ALL " & vbCrLf)
            StbQ.Append("  " & vbCrLf)

            '///////////////////////////////////////////////////////////////////////////////////////////////

            '------------------------------------------------
            'PARTICELLE ASSOCIATE A CAMPI 
            '------------------------------------------------
            StbQ.Append(" -- PARTICELLE ASSOCIATE A CAMPI  " & vbCrLf)
            StbQ.Append(" ( " & vbCrLf)
            StbQ.Append(" SELECT DISTINCT dbo.CampiXParticelle.PIVA as PIVA, dbo.CampiXParticelle.sa_cod as sa_cod, dbo.CampiXParticelle.Campo_Cod as Campo_Cod, 0 as Appezza, 0 as Id_Reg," & vbCrLf)
            StbQ.Append(" dbo.ISTAT.LOCALITA, dbo.ParticelleCatastali.Part_Cod, dbo.ParticelleCatastali.PROV, " & vbCrLf)
            StbQ.Append(" dbo.ParticelleCatastali.COM, dbo.ParticelleCatastali.SEZIONE, " & vbCrLf)
            StbQ.Append(" dbo.ParticelleCatastali.FOGLIO, dbo.ParticelleCatastali.NUMERO, dbo.ParticelleCatastali.SUBALTERNO, " & vbCrLf)

            'devo fare il cast da float a int perché nel dataset il campo è definito int
            StbQ.Append(" CONVERT(int,ParticelleCatastali.ETTARI) AS ETTARI_Sup_Cat, " & vbCrLf)
            StbQ.Append(" dbo.ParticelleCatastali.[ARE] AS ARE_Sup_Cat, dbo.ParticelleCatastali.CENTIARE AS CENTIARE_Sup_Cat, " & vbCrLf)
            'devo fare il cast da float a int perché nel dataset il campo è definito int
            StbQ.Append(" CONVERT(int, CampiXParticelle.SAU_Convenz_Ettari) AS ETTARI_Sup_Util," & vbCrLf)
            StbQ.Append(" dbo.CampiXParticelle.SAU_Convenz_Are AS ARE_Sup_Util, dbo.CampiXParticelle.SAU_Convenz_Centiare AS CENTIARE_Sup_Util, " & vbCrLf)
            StbQ.Append(" CampiXParticelle.area AS Sup_Util," & vbCrLf)

            StbQ.Append(" dbo.Campi.Validita_Inizio as Validita_Inizio, dbo.Campi.Validita_Fine as Validita_Fine, " & vbCrLf)

            'chiamo una funzione del database che concatena specie (varietà) di tutti gli impianti di un campo, attivi alla data di stampa 
            'la funzione viene creata dal migra 246
            StbQ.Append(" dbo.ElencoImpianti_from_CampoData(Campi.Piva, Campi.sa_cod, Campi.Campo_Cod, " & Agro_SQL_SaveDate(Data_Stampa) & ") AS Veg_Des, '' AS Cul_des, " & vbCrLf)
            ' StbQ.Append(" dbo.SpecieVegetali.Veg_Des, '' as Cul_Des, " & vbCrLf)

            'Modifica del 07/10/2011: messo un default sulle date, prima era '' e nella stampa del quadrop la specie veniva oscurata
            StbQ.Append(" '01/01/1900' AS Inizio_Impianto, '31/12/2100' AS Fine_Impianto " & vbCrLf)

            If Flag_LeggiTitoloPossesso Then
                StbQ.Append(" , ImpresexParticelle.TitoloPossesso " & vbCrLf)
                StbQ.Append(" , CASE ImpresexParticelle.TitoloPossesso " & vbCrLf)
                StbQ.Append("       WHEN 1 THEN 'P' " & vbCrLf)
                StbQ.Append("       WHEN 2 THEN 'C' " & vbCrLf)
                StbQ.Append("       WHEN 3 THEN 'A' " & vbCrLf)
                StbQ.Append("       WHEN 4 THEN 'A' " & vbCrLf)
                StbQ.Append("       ELSE '' END AS TitoloPossesso_Des_Abbr" & vbCrLf)
                StbQ.Append(" , ImpresexParticelle.Sup_Condotta " & vbCrLf)
                StbQ.Append(" ,floor((ImpresexParticelle.Sup_Condotta/10000)*10000) as Ettari_Sup_Condotta " & vbCrLf)
                StbQ.Append(" , floor((ImpresexParticelle.Sup_Condotta - floor((ImpresexParticelle.Sup_Condotta/10000)*10000))*100) as Are_Sup_Condotta " & vbCrLf)
                StbQ.Append(" , round( (((ImpresexParticelle.Sup_Condotta/10000)*10000)*10000)- ((floor((((ImpresexParticelle.Sup_Condotta/10000)*10000))*100))*100),0) as Centiare_Sup_Condotta " & vbCrLf)
                If Flag_LeggiDescTitoloPossessoBio Then
                    StbQ.Append(" , TitoloPossesso_Sigla + ' - ' + TitoloPossesso_Des AS TitoloPossesso_Sigla_Des " & vbCrLf)
                End If
            End If
            If Flag_LeggiMetodoProduzione Then
                'prende il metodo produzione del primo appezzamento sotto al campo
                StbQ.Append(" , ISNULL((SELECT TOP 1 Appezzamento_Codici.Val_Cod  ")
                StbQ.Append("           FROM Appezzamento_Codici ")
                StbQ.Append("           INNER JOIN Appezzamento ON Appezzamento.Piva = Appezzamento_Codici.Piva AND Appezzamento.Sa_Cod = Appezzamento_Codici.Sa_Cod AND Appezzamento.Appezza= Appezzamento_Codici.Appezza " & vbCrLf)
                StbQ.Append("           WHERE Appezzamento.piva = Campi.Piva AND Appezzamento.sa_cod = Campi.sa_cod AND Appezzamento.campo_cod = Campi.campo_cod " & vbCrLf)
                StbQ.Append("           AND Appezzamento_Codici.id_cod = " & CStr(enum_CodiciAnagrafe.MetodoDiProduzione) & "  ), '1') AS MetodoProduzione_Cod ")
            End If

            If Flag_LeggiCentro Then
                StbQ.Append(" , Centri_Aziendali.sa_nome ")
            End If

            StbQ.Append(" FROM dbo.ParticelleCatastali " & vbCrLf)
            StbQ.Append(" INNER JOIN dbo.CampiXParticelle ON dbo.ParticelleCatastali.PROV = dbo.CampiXParticelle.PROV " & vbCrLf)
            StbQ.Append(" AND dbo.ParticelleCatastali.COM = dbo.CampiXParticelle.COM " & vbCrLf)
            StbQ.Append(" AND dbo.ParticelleCatastali.sezione = dbo.CampiXParticelle.sezione " & vbCrLf)
            StbQ.Append(" AND dbo.ParticelleCatastali.foglio = dbo.CampiXParticelle.foglio " & vbCrLf)
            StbQ.Append(" AND dbo.ParticelleCatastali.numero = dbo.CampiXParticelle.numero " & vbCrLf)
            StbQ.Append(" AND dbo.ParticelleCatastali.subalterno = dbo.CampiXParticelle.subalterno " & vbCrLf)
            StbQ.Append(" INNER JOIN dbo.ISTAT ON dbo.ParticelleCatastali.PROV = dbo.ISTAT.PROV " & vbCrLf)
            StbQ.Append(" AND dbo.ParticelleCatastali.COM = dbo.ISTAT.COM " & vbCrLf)
            StbQ.Append(" INNER JOIN dbo.Campi ON dbo.CampiXParticelle.PIVA = dbo.Campi.PIVA " & vbCrLf)
            StbQ.Append(" AND dbo.CampiXParticelle.sa_cod = dbo.Campi.sa_cod " & vbCrLf)
            StbQ.Append(" AND dbo.CampiXParticelle.Campo_Cod = dbo.Campi.Campo_Cod " & vbCrLf)

            If Flag_LeggiCentro Then
                StbQ.Append(" INNER JOIN Centri_Aziendali on Campi.PIVA= Centri_Aziendali.PIVA AND Campi.sa_cod=Centri_Aziendali.sa_cod " & vbCrLf)
            End If

            If Flag_LeggiTitoloPossesso Then
                StbQ.Append(" INNER JOIN ImpresexParticelle ")
                StbQ.Append(" ON dbo.ImpreseXParticelle.PIVA = dbo.CampiXParticelle.PIVA AND dbo.ImpreseXParticelle.Sa_Cod = dbo.CampiXParticelle.Sa_Cod  ")
                StbQ.Append(" AND dbo.ImpreseXParticelle.PROV = dbo.CampiXParticelle.PROV AND dbo.ImpreseXParticelle.COM = dbo.CampiXParticelle.COM AND dbo.ImpreseXParticelle.SEZIONE = dbo.CampiXParticelle.SEZIONE ")
                StbQ.Append(" AND dbo.ImpreseXParticelle.FOGLIO = dbo.CampiXParticelle.FOGLIO AND dbo.ImpreseXParticelle.NUMERO = dbo.CampiXParticelle.NUMERO  ")
                StbQ.Append(" AND dbo.ImpreseXParticelle.SUBALTERNO = dbo.CampiXParticelle.SUBALTERNO ")
                If Flag_LeggiDescTitoloPossessoBio Then
                    StbQ.Append(" INNER JOIN BIO_Dati_TitoloPossesso ON ImpresexParticelle.TitoloPossesso = BIO_Dati_TitoloPossesso.TitoloPossesso_Cod " & vbCrLf)
                End If
            End If

            StbQ.Append(" WHERE dbo.CampiXParticelle.PIVA = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            If Sa_Cod <> 0 Then
                StbQ.Append(" AND dbo.CampiXParticelle.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & vbCrLf)
            Else
                If FiltroCentri <> "" Then
                    StbQ.Append(" AND CampiXParticelle.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1)) & ") ")
                End If
            End If

            StbQ.Append(" AND dbo.CampiXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)
            StbQ.Append(" AND dbo.CampiXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)

            StbQ.Append(" AND dbo.Campi.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)
            StbQ.Append(" AND dbo.Campi.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)

            If Flag_LeggiTitoloPossesso Then
                StbQ.Append(" AND dbo.ImpreseXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)
                StbQ.Append(" AND dbo.ImpreseXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)
            End If

            StbQ.Append(" AND Dbo.CampiXParticelle.Campo_Cod NOT IN " & vbCrLf)
            StbQ.Append("       (SELECT DISTINCT dbo.Appezzamento.Campo_Cod " & vbCrLf)
            StbQ.Append("       FROM  dbo.AppezzamentiXParticelle INNER JOIN " & vbCrLf)
            StbQ.Append("       dbo.Appezzamento ON dbo.AppezzamentiXParticelle.PIVA = dbo.Appezzamento.PIVA AND " & vbCrLf)
            StbQ.Append("       dbo.AppezzamentiXParticelle.sa_cod = dbo.Appezzamento.sa_cod AND " & vbCrLf)
            StbQ.Append("       dbo.AppezzamentiXParticelle.APPEZZA = dbo.Appezzamento.APPEZZA " & vbCrLf)
            StbQ.Append("       WHERE AppezzamentiXParticelle.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            If Sa_Cod <> 0 Then
                StbQ.Append("   AND AppezzamentiXParticelle.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & " " & vbCrLf)
            Else
                If FiltroCentri <> "" Then
                    StbQ.Append(" AND AppezzamentiXParticelle.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1)) & ") ")
                End If
            End If

            StbQ.Append("       ) " & vbCrLf)

            If xFiltroAggiuntivo3 <> "" Then
                StbQ.Append(" AND " & xFiltroAggiuntivo3)
            End If

            StbQ.Append(" ) " & vbCrLf)

            '///////////////////////////////////////////////////////////////////////////////////////////////

            If Not Flag_SoloOccupate Then

                StbQ.Append("  " & vbCrLf)
                StbQ.Append(" UNION ALL " & vbCrLf)
                StbQ.Append("  " & vbCrLf)

                '///////////////////////////////////////////////////////////////////////////////////////////////
                '------------------------------------------------
                'PARTICELLE NON ASSOCIATE
                '------------------------------------------------
                StbQ.Append(" -- PARTICELLE NON ASSOCIATE " & vbCrLf)
                StbQ.Append(" ( " & vbCrLf)
                StbQ.Append(" SELECT DISTINCT dbo.ImpreseXParticelle.PIVA as PIVA, dbo.ImpreseXParticelle.sa_cod as sa_cod, 0 as Campo_Cod, 0 as Appezza, 0 as Id_Reg," & vbCrLf)
                StbQ.Append(" dbo.ISTAT.LOCALITA, dbo.ParticelleCatastali.Part_Cod, dbo.ParticelleCatastali.PROV, " & vbCrLf)
                StbQ.Append(" dbo.ParticelleCatastali.COM, dbo.ParticelleCatastali.SEZIONE, " & vbCrLf)
                StbQ.Append(" dbo.ParticelleCatastali.FOGLIO, dbo.ParticelleCatastali.NUMERO, dbo.ParticelleCatastali.SUBALTERNO, " & vbCrLf)
                StbQ.Append(" CONVERT(int,ParticelleCatastali.ETTARI) AS ETTARI_Sup_Cat, dbo.ParticelleCatastali.[ARE] AS ARE_Sup_Cat, dbo.ParticelleCatastali.CENTIARE AS CENTIARE_Sup_Cat, " & vbCrLf)
                StbQ.Append(" 0 AS ETTARI_Sup_Util, 0 AS ARE_Sup_Util, 0 AS CENTIARE_Sup_Util, 0 AS Sup_Util, " & vbCrLf)
                StbQ.Append(" dbo.ImpreseXParticelle.Validita_Inizio as Validita_Inizio, dbo.ImpreseXParticelle.Validita_Fine as Validita_Fine, " & vbCrLf)
                StbQ.Append(" '' AS Veg_Des, '' AS Cul_Des, " & vbCrLf)
                StbQ.Append(" '' AS Inizio_Impianto, '' AS Fine_Impianto " & vbCrLf)

                If Flag_LeggiTitoloPossesso Then
                    StbQ.Append(" , ImpresexParticelle.TitoloPossesso " & vbCrLf)
                    StbQ.Append(" , CASE ImpresexParticelle.TitoloPossesso " & vbCrLf)
                    StbQ.Append("       WHEN 1 THEN 'P' " & vbCrLf)
                    StbQ.Append("       WHEN 2 THEN 'C' " & vbCrLf)
                    StbQ.Append("       WHEN 3 THEN 'A' " & vbCrLf)
                    StbQ.Append("       WHEN 4 THEN 'A' " & vbCrLf)
                    StbQ.Append("       ELSE '' END AS TitoloPossesso_Des_Abbr" & vbCrLf)
                    StbQ.Append(" , ImpresexParticelle.Sup_Condotta " & vbCrLf)
                    StbQ.Append(" ,floor((ImpresexParticelle.Sup_Condotta/10000)*10000) as Ettari_Sup_Condotta " & vbCrLf)
                    StbQ.Append(" , floor((ImpresexParticelle.Sup_Condotta - floor((ImpresexParticelle.Sup_Condotta/10000)*10000))*100) as Are_Sup_Condotta " & vbCrLf)
                    StbQ.Append(" , round( (((ImpresexParticelle.Sup_Condotta/10000)*10000)*10000)- ((floor((((ImpresexParticelle.Sup_Condotta/10000)*10000))*100))*100),0) as Centiare_Sup_Condotta " & vbCrLf)
                    If Flag_LeggiDescTitoloPossessoBio Then
                        StbQ.Append(" , TitoloPossesso_Sigla + ' - ' + TitoloPossesso_Des AS TitoloPossesso_Sigla_Des " & vbCrLf)
                    End If
                End If
                If Flag_LeggiMetodoProduzione Then
                    StbQ.Append(" , -1  ")
                End If

                If Flag_LeggiCentro Then
                    StbQ.Append(" , Centri_Aziendali.sa_nome ")
                End If

                StbQ.Append(" FROM dbo.ParticelleCatastali " & vbCrLf)
                StbQ.Append(" INNER JOIN dbo.ImpreseXParticelle ON " & vbCrLf)
                StbQ.Append(" dbo.ParticelleCatastali.PROV = dbo.ImpreseXParticelle.PROV " & vbCrLf)
                StbQ.Append(" AND dbo.ParticelleCatastali.COM = dbo.ImpreseXParticelle.COM " & vbCrLf)
                StbQ.Append(" AND dbo.ParticelleCatastali.sezione = dbo.ImpreseXParticelle.sezione " & vbCrLf)
                StbQ.Append(" AND dbo.ParticelleCatastali.foglio = dbo.ImpreseXParticelle.foglio " & vbCrLf)
                StbQ.Append(" AND dbo.ParticelleCatastali.numero = dbo.ImpreseXParticelle.numero " & vbCrLf)
                StbQ.Append(" AND dbo.ParticelleCatastali.subalterno = dbo.ImpreseXParticelle.subalterno " & vbCrLf)
                StbQ.Append(" INNER JOIN dbo.ISTAT ON dbo.ParticelleCatastali.PROV = dbo.ISTAT.PROV " & vbCrLf)
                StbQ.Append(" AND dbo.ParticelleCatastali.COM = dbo.ISTAT.COM " & vbCrLf)

                If Flag_LeggiCentro Then
                    StbQ.Append(" INNER JOIN Centri_Aziendali on ImpreseXParticelle.PIVA= Centri_Aziendali.PIVA AND ImpreseXParticelle.sa_cod=Centri_Aziendali.sa_cod " & vbCrLf)
                End If

                If Flag_LeggiDescTitoloPossessoBio Then
                    StbQ.Append(" INNER JOIN BIO_Dati_TitoloPossesso ON ImpresexParticelle.TitoloPossesso = BIO_Dati_TitoloPossesso.TitoloPossesso_Cod " & vbCrLf)
                End If

                StbQ.Append(" WHERE dbo.ImpreseXParticelle.PIVA = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
                If Sa_Cod <> 0 Then
                    StbQ.Append(" AND dbo.ImpreseXParticelle.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & vbCrLf)
                Else
                    If FiltroCentri <> "" Then
                        StbQ.Append(" AND ImpreseXParticelle.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1)) & ") ")
                    End If
                End If
                StbQ.Append(" AND dbo.ImpreseXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)
                StbQ.Append(" AND dbo.ImpreseXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)

                StbQ.Append(" AND NOT EXISTS " & vbCrLf)
                StbQ.Append("         (SELECT * FROM CampiXParticelle      " & vbCrLf)
                StbQ.Append("          WHERE dbo.ParticelleCatastali.PROV = dbo.CampiXParticelle.PROV And dbo.ParticelleCatastali.COM = dbo.CampiXParticelle.COM " & vbCrLf)
                StbQ.Append("          AND dbo.ParticelleCatastali.sezione = dbo.CampiXParticelle.sezione AND dbo.ParticelleCatastali.foglio = dbo.CampiXParticelle.foglio     " & vbCrLf)
                StbQ.Append("          AND dbo.ParticelleCatastali.numero = dbo.CampiXParticelle.numero AND dbo.ParticelleCatastali.subalterno = dbo.CampiXParticelle.subalterno " & vbCrLf)
                StbQ.Append("          AND dbo.CampiXParticelle.PIVA = ImpreseXParticelle.PIVA AND dbo.CampiXParticelle.SA_COD = dbo.ImpreseXParticelle.SA_COD  " & vbCrLf)
                StbQ.Append("          AND dbo.CampiXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)
                StbQ.Append("          AND dbo.CampiXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)
                StbQ.Append("           ) " & vbCrLf)

                StbQ.Append(" AND NOT EXISTS " & vbCrLf)
                StbQ.Append("         (SELECT * FROM AppezzamentiXParticelle      " & vbCrLf)
                StbQ.Append("          WHERE dbo.ParticelleCatastali.PROV = dbo.AppezzamentiXParticelle.PROV And dbo.ParticelleCatastali.COM = dbo.AppezzamentiXParticelle.COM " & vbCrLf)
                StbQ.Append("          AND dbo.ParticelleCatastali.sezione = dbo.AppezzamentiXParticelle.sezione AND dbo.ParticelleCatastali.foglio = dbo.AppezzamentiXParticelle.foglio      " & vbCrLf)
                StbQ.Append("          AND dbo.ParticelleCatastali.numero = dbo.AppezzamentiXParticelle.numero AND dbo.ParticelleCatastali.subalterno = dbo.AppezzamentiXParticelle.subalterno     " & vbCrLf)
                StbQ.Append("          AND dbo.AppezzamentiXParticelle.PIVA = ImpreseXParticelle.PIVA AND dbo.AppezzamentiXParticelle.SA_COD = dbo.ImpreseXParticelle.SA_COD      " & vbCrLf)
                StbQ.Append("          AND dbo.AppezzamentiXParticelle.PROV = ImpreseXParticelle.PROV AND dbo.AppezzamentiXParticelle.COM = dbo.ImpreseXParticelle.COM      " & vbCrLf)
                StbQ.Append("          AND dbo.AppezzamentiXParticelle.SEZIONE = ImpreseXParticelle.SEZIONE AND dbo.AppezzamentiXParticelle.FOGLIO = dbo.ImpreseXParticelle.FOGLIO      " & vbCrLf)
                StbQ.Append("          AND dbo.AppezzamentiXParticelle.NUMERO = ImpreseXParticelle.NUMERO AND dbo.AppezzamentiXParticelle.SUBALTERNO = dbo.ImpreseXParticelle.SUBALTERNO      " & vbCrLf)
                StbQ.Append("           AND dbo.AppezzamentiXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)
                StbQ.Append("           AND dbo.AppezzamentiXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Stampa) & vbCrLf)
                StbQ.Append("           ) " & vbCrLf)

                If xFiltroAggiuntivo4 <> "" Then
                    StbQ.Append(" AND " & xFiltroAggiuntivo4)
                End If
                StbQ.Append(" ) " & vbCrLf)

            End If

            '/*************************************************************************************

            'StbQ.Append(" ) QUADRO_P " & vbCrLf)

            '------------------------------------------------------
            '------------------- ORDINAMENTO ----------------------
            '------------------------------------------------------
            'NON SI PUO' GESTIRE L'ORDINE A PAICERE, PERCHE' ESSENDO UNA UNION BISOGNA AFRE ATTENZIONE CON I CAMPI
            '(CI POSSONO ESSERE AMBIGUITA')
            'If xOrderBy <> "" Then
            '    StbQ.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            'Else
            '    StbQ.Append(" ORDER BY ParticelleCatastali.prov, ParticelleCatastali.com, ParticelleCatastali.sezione, ParticelleCatastali.foglio, ParticelleCatastali.numero, ParticelleCatastali.subalterno " & vbCrLf)
            'End If
            StbQ.Append(" ORDER BY ParticelleCatastali.prov, ParticelleCatastali.com, ParticelleCatastali.sezione, ParticelleCatastali.foglio, ParticelleCatastali.numero, ParticelleCatastali.subalterno " & vbCrLf)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StbQ.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    '#########################################################################################################
    Private Function QuadroP_OLD(ByRef objParametri As AgronicaCoreParametri,
                       ByVal Piva As String,
                       ByVal SaCod As Integer,
                       ByVal ValiditaInizio As Date,
                       ByVal ValiditaFine As Date) As DataTable

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.ParticelleCatastali_R.QuadroP()"


        Dim StrSQL As New Text.StringBuilder
        Dim MessaggioErrore As String = ""
        Dim DT As DataTable
        Try
            '----------------------------------------------------
            '--- Preparo la Query SQL ---------------------------
            '----------------------------------------------------

            StrSQL.Length = 0

            StrSQL.Append(" SELECT DISTINCT dbo.AppezzamentiXParticelle.PIVA, dbo.AppezzamentiXParticelle.SA_COD, dbo.Appezzamento.Campo_Cod as Campo_Cod, dbo.Appezzamento.Appezza as Appezza , ")
            StrSQL.Append(" dbo.ISTAT.LOCALITA, dbo.ParticelleCatastali.Part_Cod, dbo.ParticelleCatastali.PROV, ")
            StrSQL.Append(" dbo.ParticelleCatastali.COM, dbo.ParticelleCatastali.SEZIONE, ")
            StrSQL.Append(" dbo.ParticelleCatastali.FOGLIO, dbo.ParticelleCatastali.NUMERO, dbo.ParticelleCatastali.SUBALTERNO, ")
            StrSQL.Append(" dbo.ParticelleCatastali.ETTARI AS ETTARI_Sup_Cat, dbo.ParticelleCatastali.[ARE] AS ARE_Sup_Cat, dbo.ParticelleCatastali.CENTIARE AS CENTIARE_Sup_Cat, ")
            StrSQL.Append(" dbo.AppezzamentiXParticelle.SAU_Convenz_Ettari AS ETTARI_Sup_Util, dbo.AppezzamentiXParticelle.SAU_Convenz_Are AS ARE_Sup_Util, dbo.AppezzamentiXParticelle.SAU_Convenz_Centiare AS CENTIARE_Sup_Util, ")
            StrSQL.Append(" dbo.Appezzamento.Validita_Inizio, dbo.Appezzamento.Validita_Fine, ")
            StrSQL.Append(" dbo.Centri_Aziendali.sa_nome, dbo.Imprese.rag_soc, ")
            StrSQL.Append(" dbo.SpecieVegetali.Veg_Des, Cultivar.Cul_Des ")
            StrSQL.Append(" FROM dbo.ParticelleCatastali ")
            StrSQL.Append(" INNER JOIN dbo.AppezzamentiXParticelle ON dbo.ParticelleCatastali.PROV = dbo.AppezzamentiXParticelle.PROV AND dbo.ParticelleCatastali.COM = dbo.AppezzamentiXParticelle.COM ")
            StrSQL.Append(" AND dbo.ParticelleCatastali.sezione = dbo.AppezzamentiXParticelle.sezione ")
            StrSQL.Append(" AND dbo.ParticelleCatastali.foglio = dbo.AppezzamentiXParticelle.foglio ")
            StrSQL.Append(" AND dbo.ParticelleCatastali.numero = dbo.AppezzamentiXParticelle.numero ")
            StrSQL.Append(" AND dbo.ParticelleCatastali.subalterno = dbo.AppezzamentiXParticelle.subalterno ")
            StrSQL.Append(" INNER JOIN dbo.ISTAT ON dbo.ParticelleCatastali.PROV = dbo.ISTAT.PROV AND dbo.ParticelleCatastali.COM = dbo.ISTAT.COM ")
            StrSQL.Append(" INNER JOIN dbo.Appezzamento ON dbo.AppezzamentiXParticelle.PIVA = dbo.Appezzamento.PIVA AND ")
            StrSQL.Append(" dbo.AppezzamentiXParticelle.SA_COD = dbo.Appezzamento.SA_COD AND ")
            StrSQL.Append(" dbo.AppezzamentiXParticelle.APPEZZA = dbo.Appezzamento.APPEZZA ")
            StrSQL.Append(" INNER JOIN dbo.Centri_Aziendali ON dbo.Appezzamento.PIVA = dbo.Centri_Aziendali.PIVA AND dbo.Appezzamento.SA_COD = dbo.Centri_Aziendali.sa_cod ")
            StrSQL.Append(" INNER JOIN dbo.Imprese ON dbo.Centri_Aziendali.PIVA = dbo.Imprese.PIVA ")
            StrSQL.Append(" INNER JOIN dbo.Reg_Impianti ON dbo.AppezzamentiXParticelle.PIVA = dbo.Reg_Impianti.PIVA ")
            StrSQL.Append(" AND dbo.AppezzamentiXParticelle.SA_COD = dbo.Reg_Impianti.SA_COD ")
            StrSQL.Append(" AND dbo.AppezzamentiXParticelle.APPEZZA = dbo.Reg_Impianti.APPEZZA ")
            StrSQL.Append(" LEFT JOIN dbo.Cultivar ON dbo.Reg_Impianti.CUL_COD = dbo.Cultivar.Cul_Cod ")
            StrSQL.Append(" LEFT JOIN dbo.SpecieVegetali ON dbo.Cultivar.Veg_Cod = dbo.SpecieVegetali.Veg_Cod ")
            StrSQL.Append(" WHERE dbo.AppezzamentiXParticelle.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND dbo.AppezzamentiXParticelle.SA_COD = " & Agro_SQL_SaveNum(SaCod))
            StrSQL.Append(" AND dbo.AppezzamentiXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(ValiditaFine))
            StrSQL.Append(" AND dbo.AppezzamentiXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(ValiditaInizio))

            StrSQL.Append(" AND dbo.Reg_Impianti.Validita_Inizio <= " & Agro_SQL_SaveDate(ValiditaFine))
            StrSQL.Append(" AND dbo.Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(ValiditaInizio))

            StrSQL.Append(" AND Dbo.Appezzamento.Campo_Cod NOT IN ")
            StrSQL.Append(" (SELECT campo_cod FROM CampiXParticelle ")
            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(SaCod) & ") ")

            StrSQL.Append(" UNION ")

            StrSQL.Append(" SELECT DISTINCT dbo.CampiXParticelle.PIVA as PIVA, dbo.CampiXParticelle.SA_COD as Sa_Cod, dbo.CampiXParticelle.Campo_Cod as Campo_Cod, 0 as Appezza, ")
            StrSQL.Append(" dbo.ISTAT.LOCALITA, dbo.ParticelleCatastali.Part_Cod, dbo.ParticelleCatastali.PROV, ")
            StrSQL.Append(" dbo.ParticelleCatastali.COM, dbo.ParticelleCatastali.SEZIONE, ")
            StrSQL.Append(" dbo.ParticelleCatastali.FOGLIO, dbo.ParticelleCatastali.NUMERO, dbo.ParticelleCatastali.SUBALTERNO, ")
            StrSQL.Append(" dbo.ParticelleCatastali.ETTARI AS ETTARI_Sup_Cat, dbo.ParticelleCatastali.[ARE] AS ARE_Sup_Cat, dbo.ParticelleCatastali.CENTIARE AS CENTIARE_Sup_Cat, ")
            StrSQL.Append(" dbo.CampiXParticelle.SAU_Convenz_Ettari AS ETTARI_Sup_Util, dbo.CampiXParticelle.SAU_Convenz_Are AS ARE_Sup_Util, dbo.CampiXParticelle.SAU_Convenz_Centiare AS CENTIARE_Sup_Util, ")
            StrSQL.Append(" dbo.Campi.Validita_Inizio as Validita_Inizio, dbo.Campi.Validita_Fine as Validita_Fine, ")
            StrSQL.Append(" dbo.Centri_Aziendali.sa_nome, dbo.Imprese.rag_soc, SpecieVegetali.Veg_Des as Veg_Des, '' as Cul_Des  ")
            StrSQL.Append(" FROM dbo.ParticelleCatastali ")
            StrSQL.Append(" INNER JOIN dbo.CampiXParticelle ON dbo.ParticelleCatastali.PROV = dbo.CampiXParticelle.PROV ")
            StrSQL.Append(" AND dbo.ParticelleCatastali.COM = dbo.CampiXParticelle.COM ")
            StrSQL.Append(" AND dbo.ParticelleCatastali.sezione = dbo.CampiXParticelle.sezione ")
            StrSQL.Append(" AND dbo.ParticelleCatastali.foglio = dbo.CampiXParticelle.foglio ")
            StrSQL.Append(" AND dbo.ParticelleCatastali.numero = dbo.CampiXParticelle.numero ")
            StrSQL.Append(" AND dbo.ParticelleCatastali.subalterno = dbo.CampiXParticelle.subalterno ")
            StrSQL.Append(" INNER JOIN dbo.ISTAT ON dbo.ParticelleCatastali.PROV = dbo.ISTAT.PROV ")
            StrSQL.Append(" AND dbo.ParticelleCatastali.COM = dbo.ISTAT.COM ")
            StrSQL.Append(" INNER JOIN dbo.Campi ON dbo.CampiXParticelle.PIVA = dbo.Campi.PIVA ")
            StrSQL.Append(" AND dbo.CampiXParticelle.SA_COD = dbo.Campi.SA_COD ")
            StrSQL.Append(" AND dbo.CampiXParticelle.Campo_Cod = dbo.Campi.Campo_Cod ")
            StrSQL.Append(" INNER JOIN dbo.Centri_Aziendali ON dbo.Campi.PIVA = dbo.Centri_Aziendali.PIVA ")
            StrSQL.Append(" AND dbo.Campi.SA_COD = dbo.Centri_Aziendali.sa_cod ")
            StrSQL.Append(" INNER JOIN dbo.Imprese ON dbo.Centri_Aziendali.PIVA = dbo.Imprese.PIVA ")
            StrSQL.Append(" LEFT OUTER JOIN dbo.SpecieVegetali ON dbo.Campi.Veg_Cod = dbo.SpecieVegetali.Veg_Cod")
            StrSQL.Append(" WHERE dbo.CampiXParticelle.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND dbo.CampiXParticelle.SA_COD = " & Agro_SQL_SaveNum(SaCod))
            StrSQL.Append(" AND dbo.CampiXParticelle.Validita_Inizio < = " & Agro_SQL_SaveDate(ValiditaFine))
            StrSQL.Append(" AND dbo.CampiXParticelle.Validita_Fine > " & Agro_SQL_SaveDate(ValiditaInizio))
            StrSQL.Append(" AND dbo.Campi.Veg_Cod <> 0 ")

            StrSQL.Append(" UNION ")

            StrSQL.Append(" SELECT DISTINCT dbo.ImpreseXParticelle.PIVA as PIVA, dbo.ImpreseXParticelle.SA_COD as Sa_Cod, 0 as Campo_Cod, 0 as Appezza,  ")
            StrSQL.Append(" dbo.ISTAT.LOCALITA, dbo.ParticelleCatastali.Part_Cod, dbo.ParticelleCatastali.PROV,  dbo.ParticelleCatastali.COM, dbo.ParticelleCatastali.SEZIONE,  ")
            StrSQL.Append(" dbo.ParticelleCatastali.FOGLIO, dbo.ParticelleCatastali.NUMERO, dbo.ParticelleCatastali.SUBALTERNO,  dbo.ParticelleCatastali.ETTARI AS ETTARI_Sup_Cat, ")
            StrSQL.Append(" dbo.ParticelleCatastali.[ARE] AS ARE_Sup_Cat, dbo.ParticelleCatastali.CENTIARE AS CENTIARE_Sup_Cat,  0 AS ETTARI_Sup_Util, 0 AS ARE_Sup_Util, ")
            StrSQL.Append(" 0 AS CENTIARE_Sup_Util,  dbo.ImpreseXParticelle.Validita_Inizio as Validita_Inizio, dbo.ImpreseXParticelle.Validita_Fine as Validita_Fine,  ")
            StrSQL.Append(" dbo.Centri_Aziendali.sa_nome, dbo.Imprese.rag_soc, '' as Veg_Des, '' as Cul_Des  ")

            StrSQL.Append(" FROM dbo.ParticelleCatastali ")
            StrSQL.Append(" INNER JOIN dbo.ImpreseXParticelle ON  dbo.ParticelleCatastali.PROV = dbo.ImpreseXParticelle.PROV  AND dbo.ParticelleCatastali.COM = dbo.ImpreseXParticelle.COM  AND dbo.ParticelleCatastali.sezione = dbo.ImpreseXParticelle.sezione  AND dbo.ParticelleCatastali.foglio = dbo.ImpreseXParticelle.foglio  AND dbo.ParticelleCatastali.numero = dbo.ImpreseXParticelle.numero  AND dbo.ParticelleCatastali.subalterno = dbo.ImpreseXParticelle.subalterno  INNER JOIN dbo.ISTAT ON dbo.ParticelleCatastali.PROV = dbo.ISTAT.PROV  AND dbo.ParticelleCatastali.COM = dbo.ISTAT.COM  ")
            StrSQL.Append(" INNER JOIN dbo.Centri_Aziendali ON dbo.ImpreseXParticelle.PIVA = dbo.Centri_Aziendali.PIVA  AND dbo.ImpreseXParticelle.SA_COD = dbo.Centri_Aziendali.sa_cod  ")
            StrSQL.Append(" INNER JOIN dbo.Imprese ON dbo.Centri_Aziendali.PIVA = dbo.Imprese.PIVA  ")
            StrSQL.Append(" LEFT OUTER JOIN Appezzamento ON Appezzamento.Piva = ImpreseXParticelle.Piva  AND Appezzamento.Sa_Cod = ImpreseXParticelle.Sa_Cod  ")
            StrSQL.Append(" LEFT OUTER JOIN dbo.Reg_Impianti ON Appezzamento.Piva = Reg_Impianti.Piva  AND Appezzamento.Sa_Cod = Reg_Impianti.Sa_Cod  AND Appezzamento.Appezza = Reg_Impianti.Appezza  ")
            StrSQL.Append(" --LEFT JOIN dbo.Cultivar ON dbo.Reg_Impianti.CUL_COD = dbo.Cultivar.Cul_Cod  ")
            StrSQL.Append(" --LEFT JOIN dbo.SpecieVegetali ON dbo.Cultivar.Veg_Cod = dbo.SpecieVegetali.Veg_Cod  ")

            StrSQL.Append(" WHERE dbo.ImpreseXParticelle.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND dbo.ImpreseXParticelle.SA_COD = " & Agro_SQL_SaveNum(SaCod))
            StrSQL.Append(" AND dbo.ImpreseXParticelle.Validita_Inizio < = " & Agro_SQL_SaveDate(ValiditaFine))
            StrSQL.Append(" AND dbo.ImpreseXParticelle.Validita_Fine > " & Agro_SQL_SaveDate(ValiditaInizio))

            StrSQL.Append(" AND NOT EXISTS ")
            StrSQL.Append("         (SELECT * FROM CampiXParticelle      ")
            StrSQL.Append("          WHERE dbo.ParticelleCatastali.PROV = dbo.CampiXParticelle.PROV And dbo.ParticelleCatastali.COM = dbo.CampiXParticelle.COM ")
            StrSQL.Append("          AND dbo.ParticelleCatastali.sezione = dbo.CampiXParticelle.sezione AND dbo.ParticelleCatastali.foglio = dbo.CampiXParticelle.foglio     ")
            StrSQL.Append("          AND dbo.ParticelleCatastali.numero = dbo.CampiXParticelle.numero AND dbo.ParticelleCatastali.subalterno = dbo.CampiXParticelle.subalterno ")
            StrSQL.Append("          AND dbo.CampiXParticelle.PIVA = ImpreseXParticelle.PIVA AND dbo.CampiXParticelle.SA_COD = dbo.ImpreseXParticelle.SA_COD  ")
            StrSQL.Append("          AND dbo.CampiXParticelle.Validita_Inizio < = " & Agro_SQL_SaveDate(ValiditaFine))
            StrSQL.Append("          AND dbo.CampiXParticelle.Validita_Fine > " & Agro_SQL_SaveDate(ValiditaInizio) & ")")


            StrSQL.Append(" AND NOT EXISTS ")
            StrSQL.Append("         (SELECT * FROM AppezzamentiXParticelle      ")
            StrSQL.Append("          WHERE dbo.ParticelleCatastali.PROV = dbo.AppezzamentiXParticelle.PROV And dbo.ParticelleCatastali.COM = dbo.AppezzamentiXParticelle.COM ")
            StrSQL.Append("          AND dbo.ParticelleCatastali.sezione = dbo.AppezzamentiXParticelle.sezione AND dbo.ParticelleCatastali.foglio = dbo.AppezzamentiXParticelle.foglio      ")
            StrSQL.Append("          AND dbo.ParticelleCatastali.numero = dbo.AppezzamentiXParticelle.numero AND dbo.ParticelleCatastali.subalterno = dbo.AppezzamentiXParticelle.subalterno     ")
            StrSQL.Append("          AND dbo.AppezzamentiXParticelle.PIVA = ImpreseXParticelle.PIVA AND dbo.AppezzamentiXParticelle.SA_COD = dbo.ImpreseXParticelle.SA_COD      ")
            StrSQL.Append("          AND dbo.AppezzamentiXParticelle.PROV = ImpreseXParticelle.PROV AND dbo.AppezzamentiXParticelle.COM = dbo.ImpreseXParticelle.COM      ")
            StrSQL.Append("          AND dbo.AppezzamentiXParticelle.SEZIONE = ImpreseXParticelle.SEZIONE AND dbo.AppezzamentiXParticelle.FOGLIO = dbo.ImpreseXParticelle.FOGLIO      ")
            StrSQL.Append("          AND dbo.AppezzamentiXParticelle.NUMERO = ImpreseXParticelle.NUMERO AND dbo.AppezzamentiXParticelle.SUBALTERNO = dbo.ImpreseXParticelle.SUBALTERNO      ")
            StrSQL.Append("          AND AppezzamentiXParticelle.PIVA = Appezzamento.PIVA AND AppezzamentiXParticelle.SA_COD = Appezzamento.SA_COD AND ")
            StrSQL.Append("          AppezzamentiXParticelle.APPEZZA = Appezzamento.APPEZZA ")
            StrSQL.Append("          AND dbo.Appezzamento.Validita_Inizio < = " & Agro_SQL_SaveDate(ValiditaFine))
            StrSQL.Append("          AND dbo.Appezzamento.Validita_Fine > " & Agro_SQL_SaveDate(ValiditaInizio) & ")")


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



    '##############################################################################################################
    Public Function PianoColturale_Excel_ImpiantiConOperazioni(ByVal Query1_TempTableCreazione As String,
                                                               ByVal Query2_TempTableIndice As String,
                                                               ByVal Query3_TempTableFill As String,
                                                               ByRef objParametri As AgronicaCoreParametri,
                                                               Optional ByVal Mostra_varieta_OI_Pomodoro As Boolean = False
                                                               ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.AnagraficaAziendale.PianoColturale_Excel_ImpiantiConOperazioni"

        Dim MessaggioErrore As String = ""
        Dim StbQ As New Text.StringBuilder
        Dim DT As DataTable
        Dim Query4_TempTableJoin As String

        Try


            StbQ.Length = 0

            'StbQ.Append(" -- Appezzamento.SUP_APP, Appezzamento.Validita_Inizio AS Appezzamento_Validita_Inizio, Appezzamento.Validita_Fine AS Appezzamento_Validita_Fine, " & vbCrLf)
            'StbQ.Append(" --Agenda.des_lib, UnitaMisura.UDM_DES, Movimenti_dettagli.Extra_Int, Movimenti_dettagli.Qta AS Qta_Totale,  '' AS Fornitore " & vbCrLf)

            StbQ.Append(" SELECT *  " & vbCrLf)
            StbQ.Append(" FROM   " & vbCrLf)
            StbQ.Append(" (  " & vbCrLf)


            '---------------------------------------------------------------
            '------- PRIMA PARTE: IMPIANTI CON SPECIE CON RACCOLTE  --------
            '---------------------------------------------------------------
            StbQ.Append("   " & vbCrLf)
            StbQ.Append("   " & vbCrLf)
            StbQ.Append(" -- PRIMA PARTE: IMPIANTI CON SPECIE CON RACCOLTE  " & vbCrLf)
            StbQ.Append(" (  " & vbCrLf)
            StbQ.Append(" SELECT  " & vbCrLf)
            StbQ.Append(" Imprese.rag_soc, Centri_Aziendali.sa_nome,   " & vbCrLf)
            StbQ.Append(" Appezzamento.APP_NOME, Appezzamento.Campo_Cod, " & vbCrLf)
            'StbQ.Append(" ISNULL( (SELECT Campi.Campo_Des " & vbCrLf)
            'StbQ.Append("           FROM Campi " & vbCrLf)
            'StbQ.Append("           WHERE Appezzamento.PIVA = Campi.Piva AND Appezzamento.SA_COD = Campi.Sa_Cod AND Appezzamento.Campo_Cod = Campi.Campo_Cod " & vbCrLf)
            'StbQ.Append("           ) , '' ) AS Campo_Des, " & vbCrLf)
            StbQ.Append(" Reg_Impianti.PIVA, Reg_Impianti.SA_COD, Reg_Impianti.APPEZZA, Reg_Impianti.ID_REG, " & vbCrLf)
            StbQ.Append(" Reg_Impianti.GRVA_Cod_VEG, Reg_Impianti.Sup_Imp,  " & vbCrLf)
            StbQ.Append(" Reg_Impianti.Validita_Inizio AS Reg_Impianti_Validita_Inizio, Reg_Impianti.Validita_Fine AS Reg_Impianti_Validita_Fine,  Imprese_Progetti.regolamento_cod, " & vbCrLf)
            'StbQ.Append(" ISNULL( (SELECT Grva_Des " & vbCrLf)
            'StbQ.Append("           FROM GruppoVarietale " & vbCrLf)
            'StbQ.Append("           WHERE GruppoVarietale.Grva_Cod = ABS(Reg_Impianti.GRVA_Cod_VEG) " & vbCrLf)
            'StbQ.Append("           ) , '' ) AS Grva_Des, " & vbCrLf)
            StbQ.Append(" Imprese_Progetti.P_HA, Imprese_Progetti.Produzione_Prevista AS Resa_Prevista_HA,  " & vbCrLf)
            StbQ.Append(" SpecieVegetali.Veg_Cod, SpecieVegetali.Veg_Des, Reg_Impianti.CUL_COD, Cultivar.Cul_Des, " & vbCrLf)

            StbQ.Append(" Agenda.Id_Agenda, Agenda.Lav_Cod, Movimenti.Data_Movimento, " & vbCrLf)
            StbQ.Append(" Movimenti_dettagli.Udm_Cod, " & vbCrLf)
            StbQ.Append(" Mov_Destinazioni.Qta AS Qta_Impianto,  " & vbCrLf)
            StbQ.Append(" Mov_Destinazioni.Qta2 AS SemTrap_Superficie,  " & vbCrLf)
            StbQ.Append(" Movimenti_dettagli.Cal_Cod, ISNULL(Movimenti_dettagli.Lotto,'') AS Lotto, " & vbCrLf)
            StbQ.Append(" ISNULL(Materie_Prime.Mat_Cod, 0) AS Mat_Cod, ISNULL(Materie_Prime.Mat_Des,'') AS Mat_Des, ISNULL(Materie_Prime.Cod_Articolo,'') AS Cod_Articolo, " & vbCrLf)
            StbQ.Append(" UnitaMisura.UDM_SIM, Materie_Prime_Calibri.cal_des  " & vbCrLf)
            'StbQ.Append(" ISNULL( (SELECT TOP 1 Cal_Des " & vbCrLf)
            'StbQ.Append("           FROM Materie_Prime_Calibri " & vbCrLf)
            'StbQ.Append("           WHERE Materie_Prime_Calibri.Cal_Cod = Movimenti_dettagli.Cal_Cod  " & vbCrLf)
            'StbQ.Append("           ) , '' ) AS Cal_Des, '' AS Fornitore " & vbCrLf)

            If Mostra_varieta_OI_Pomodoro Then
                StbQ.Append(" ,ISNULL(Codifica_Varieta_OIPomodorodaIndustriaNordItalia.Cod_Varieta, 0) AS Cul_Cod_OI_Pomodoro, ISNULL(Codifica_Varieta_OIPomodorodaIndustriaNordItalia.Desc_Varieta, '') AS Cul_Des_OI_Pomodoro " & vbCrLf)
                StbQ.Append(" ,ISNULL(Codifica_Varieta_OIPomodorodaIndustriaNordItalia.grva_cod_gias, 0) AS grva_cod_cultivar " & vbCrLf)
            Else
                StbQ.Append(" ,0 AS Cul_Cod_OI_Pomodoro, '' AS Cul_Des_OI_Pomodoro " & vbCrLf)
                StbQ.Append(" ,0 AS grva_cod_cultivar " & vbCrLf)
            End If

            StbQ.Append(" FROM Imprese  " & vbCrLf)
            StbQ.Append(" INNER JOIN Centri_Aziendali ON Centri_Aziendali.Piva = Imprese.Piva  " & vbCrLf)
            StbQ.Append(" INNER JOIN Appezzamento ON Centri_Aziendali.SA_COD = Appezzamento.SA_COD AND Centri_Aziendali.PIVA = Appezzamento.PIVA " & vbCrLf)
            StbQ.Append(" INNER JOIN Reg_Impianti ON Reg_Impianti.APPEZZA = Appezzamento.APPEZZA AND Reg_Impianti.SA_COD = Appezzamento.SA_COD AND " & vbCrLf)
            StbQ.Append(" Reg_Impianti.PIVA = Appezzamento.PIVA " & vbCrLf)

            StbQ.Append(" INNER JOIN Mov_Destinazioni ON Reg_Impianti.Piva = Mov_Destinazioni.PIVA AND Reg_Impianti.Sa_Cod = Mov_Destinazioni.Sa_Cod AND Mov_Destinazioni.Appezza = Reg_Impianti.Appezza AND Mov_Destinazioni.Id_Destinazione = Reg_Impianti.Id_Reg  " & vbCrLf)
            StbQ.Append(" INNER JOIN Movimenti_dettagli ON Mov_Destinazioni.Piva = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Sa_Cod = Movimenti_dettagli.Sa_Cod AND Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det  " & vbCrLf)
            StbQ.Append(" INNER JOIN Movimenti ON Movimenti.PIVA = Movimenti_Dettagli.PIVA AND Movimenti_dettagli.Sa_Cod = Movimenti.Sa_Cod AND Movimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov  " & vbCrLf)
            StbQ.Append(" INNER JOIN Agenda ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod  AND Agenda.Id_Agenda = Movimenti.Id_Agenda  " & vbCrLf)

            StbQ.Append(" INNER JOIN UnitaMisura ON UnitaMisura.Udm_Cod = Movimenti_Dettagli.Udm_Cod " & vbCrLf)
            StbQ.Append(" LEFT OUTER JOIN Materie_Prime ON Materie_Prime.Elem_Cod = Movimenti_Dettagli.Elem_Cod AND Materie_Prime.Mat_Cod = Movimenti_Dettagli.Mat_Cod   " & vbCrLf)

            StbQ.Append(" INNER JOIN Imprese_Progetti ON Reg_Impianti.PIVA = Imprese_Progetti.Piva " & vbCrLf)
            StbQ.Append("             AND Reg_Impianti.Sa_Cod = Imprese_Progetti.Sa_Cod  " & vbCrLf)
            StbQ.Append("             AND Reg_Impianti.Appezza = Imprese_Progetti.Appezza  " & vbCrLf)
            StbQ.Append("             AND Reg_Impianti.Id_Reg = Imprese_Progetti.Id_reg " & vbCrLf)
            StbQ.Append(" INNER JOIN Cultivar ON Reg_Impianti.Cul_Cod = Cultivar.Cul_Cod " & vbCrLf)
            StbQ.Append(" INNER JOIN SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod " & vbCrLf)

            StbQ.Append(" INNER JOIN Materie_Prime_Calibri ON Materie_Prime_Calibri.Cal_Cod = Movimenti_dettagli.Cal_Cod  " & vbCrLf)

            Aggiungi_Join_OI_Pomodoro(StbQ, Mostra_varieta_OI_Pomodoro)

            '/**************************************************
            '         NUOVO METODO CON TABELLA TEMPORANEA

            '           --> AGGIUNGO QUESTA PARTE:

            StbQ.Append(" INNER JOIN  #tempimpianti " & vbCrLf)
            StbQ.Append(" ON #tempimpianti.Piva = Reg_Impianti.Piva AND Reg_Impianti.Sa_Cod = #tempimpianti.Sa_Cod AND  Reg_Impianti.Appezza = #tempimpianti.Appezza AND  Reg_Impianti.Id_Reg = #tempimpianti.Id_Reg " & vbCrLf)
            '/**************************************************

            'StbQ.Append(" WHERE  Agenda.Lav_Cod IN (" & Agro_SQL_SaveNum(LAVCOD_SEMINA) & ", " & vbCrLf)
            'StbQ.Append("                           " & Agro_SQL_SaveNum(LAVCOD_TRAPIANTO) & ", " & vbCrLf)
            'StbQ.Append("                           " & Agro_SQL_SaveNum(LAVCOD_RACCOLTA) & " " & vbCrLf)
            'StbQ.Append("                           ) " & vbCrLf)

            StbQ.Append(" WHERE  Agenda.Lav_Cod = " & Agro_SQL_SaveNum(LAVCOD_RACCOLTA) & " " & vbCrLf)

            'bisogna filtrare la distinta attiva alla data dell'operazione
            '(altrimenti possono essere letti più distinte per impianto)
            StbQ.Append(" AND	Movimenti.Data_Movimento >= Imprese_Progetti.Validita_Inizio " & vbCrLf)
            StbQ.Append(" AND	Movimenti.Data_Movimento <= Imprese_Progetti.Validita_Fine " & vbCrLf)
            StbQ.Append(" )  " & vbCrLf)


            StbQ.Append(" UNION ALL " & vbCrLf)


            '------------------------------------------------------------------------------
            '------- SECONDA PARTE: IMPIANTI CON SPECIE CON SEMINE SENZA MAGAZZINO  --------
            '(non c'è il join con materie prime e unità di misura)
            'AND Movimenti_Dettagli.MAT_COD=0
            '-------------------------------------------------------------------------------
            StbQ.Append("  " & vbCrLf)
            StbQ.Append("   " & vbCrLf)
            StbQ.Append(" -- SECONDA PARTE: IMPIANTI CON SPECIE CON SEMINE SENZA MAGAZZINO    " & vbCrLf)
            StbQ.Append(" (  " & vbCrLf)
            StbQ.Append(" SELECT  " & vbCrLf)
            StbQ.Append(" Imprese.rag_soc, Centri_Aziendali.sa_nome,   " & vbCrLf)
            StbQ.Append(" Appezzamento.APP_NOME,  Appezzamento.Campo_Cod, " & vbCrLf)
            'StbQ.Append(" ISNULL( (SELECT Campi.Campo_Des " & vbCrLf)
            'StbQ.Append("           FROM Campi " & vbCrLf)
            'StbQ.Append("           WHERE Appezzamento.PIVA = Campi.Piva AND Appezzamento.SA_COD = Campi.Sa_Cod AND Appezzamento.Campo_Cod = Campi.Campo_Cod " & vbCrLf)
            'StbQ.Append("           ) , '' ) AS Campo_Des, " & vbCrLf)
            StbQ.Append(" Reg_Impianti.PIVA, Reg_Impianti.SA_COD, Reg_Impianti.APPEZZA, Reg_Impianti.ID_REG, " & vbCrLf)
            StbQ.Append(" Reg_Impianti.GRVA_Cod_VEG, Reg_Impianti.Sup_Imp,  " & vbCrLf)
            StbQ.Append(" Reg_Impianti.Validita_Inizio AS Reg_Impianti_Validita_Inizio, Reg_Impianti.Validita_Fine AS Reg_Impianti_Validita_Fine, Imprese_Progetti.regolamento_cod, " & vbCrLf)
            'StbQ.Append(" ISNULL( (SELECT Grva_Des " & vbCrLf)
            'StbQ.Append("           FROM GruppoVarietale " & vbCrLf)
            'StbQ.Append("           WHERE GruppoVarietale.Grva_Cod = ABS(Reg_Impianti.GRVA_Cod_VEG) " & vbCrLf)
            'StbQ.Append("           ) , '' ) AS Grva_Des, " & vbCrLf)
            StbQ.Append(" Imprese_Progetti.P_HA, Imprese_Progetti.Produzione_Prevista AS Resa_Prevista_HA,  " & vbCrLf)
            StbQ.Append(" SpecieVegetali.Veg_Cod, SpecieVegetali.Veg_Des, Reg_Impianti.CUL_COD, Cultivar.Cul_Des, " & vbCrLf)
            StbQ.Append("  Agenda.Id_Agenda, Agenda.Lav_Cod, Movimenti.Data_Movimento, " & vbCrLf)
            StbQ.Append(" Movimenti_dettagli.Udm_Cod, " & vbCrLf)
            StbQ.Append(" Mov_Destinazioni.Qta AS Qta_Impianto,  " & vbCrLf)
            StbQ.Append(" Mov_Destinazioni.Qta2 AS SemTrap_Superficie,  " & vbCrLf)
            StbQ.Append(" Movimenti_dettagli.Cal_Cod, ISNULL(Movimenti_dettagli.Lotto,'') AS Lotto, " & vbCrLf)
            StbQ.Append(" 0 AS Mat_Cod, '' AS Mat_Des, '' AS Cod_Articolo, " & vbCrLf)
            StbQ.Append(" ISNULL( (SELECT UDM_SIM " & vbCrLf)
            StbQ.Append("           FROM UnitaMisura " & vbCrLf)
            StbQ.Append("           WHERE UnitaMisura.Udm_Cod = Movimenti_Dettagli.Udm_Cod " & vbCrLf)
            StbQ.Append("           ) , '' ) AS UDM_SIM,  " & vbCrLf)
            StbQ.Append(" ''  AS Cal_Des " & vbCrLf)

            If Mostra_varieta_OI_Pomodoro Then
                StbQ.Append(" , ISNULL(Codifica_Varieta_OIPomodorodaIndustriaNordItalia.Cod_Varieta, 0) AS Cul_Cod_OI_Pomodoro, ISNULL(Codifica_Varieta_OIPomodorodaIndustriaNordItalia.Desc_Varieta,'') AS Cul_Des_OI_Pomodoro " & vbCrLf)
                StbQ.Append(" ,ISNULL(Codifica_Varieta_OIPomodorodaIndustriaNordItalia.grva_cod_gias, 0) AS grva_cod_cultivar " & vbCrLf)
            Else
                StbQ.Append(" ,0 AS Cul_Cod_OI_Pomodoro, '' AS Cul_Des_OI_Pomodoro " & vbCrLf)
                StbQ.Append(" ,0 AS grva_cod_cultivar " & vbCrLf)
            End If

            StbQ.Append(" FROM Imprese  " & vbCrLf)
            StbQ.Append(" INNER JOIN Centri_Aziendali ON Centri_Aziendali.Piva = Imprese.Piva  " & vbCrLf)
            StbQ.Append(" INNER JOIN Appezzamento ON Centri_Aziendali.SA_COD = Appezzamento.SA_COD AND Centri_Aziendali.PIVA = Appezzamento.PIVA " & vbCrLf)
            StbQ.Append(" INNER JOIN Reg_Impianti ON Reg_Impianti.APPEZZA = Appezzamento.APPEZZA AND Reg_Impianti.SA_COD = Appezzamento.SA_COD AND " & vbCrLf)
            StbQ.Append(" Reg_Impianti.PIVA = Appezzamento.PIVA " & vbCrLf)

            StbQ.Append(" INNER JOIN Mov_Destinazioni ON Reg_Impianti.Piva = Mov_Destinazioni.PIVA AND Reg_Impianti.Sa_Cod = Mov_Destinazioni.Sa_Cod AND Mov_Destinazioni.Appezza = Reg_Impianti.Appezza AND Mov_Destinazioni.Id_Destinazione = Reg_Impianti.Id_Reg  " & vbCrLf)
            StbQ.Append(" INNER JOIN Movimenti_dettagli ON Mov_Destinazioni.Piva = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Sa_Cod = Movimenti_dettagli.Sa_Cod AND Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det  " & vbCrLf)
            StbQ.Append(" INNER JOIN Movimenti ON Movimenti.PIVA = Movimenti_Dettagli.PIVA AND Movimenti_dettagli.Sa_Cod = Movimenti.Sa_Cod AND Movimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov  " & vbCrLf)
            StbQ.Append(" INNER JOIN Agenda ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod  AND Agenda.Id_Agenda = Movimenti.Id_Agenda  " & vbCrLf)

            'StbQ.Append(" --INNER JOIN UnitaMisura ON UnitaMisura.Udm_Cod = Movimenti_Dettagli.Udm_Cod " & vbCrLf)
            'StbQ.Append(" --INNER JOIN Materie_Prime ON Materie_Prime.Elem_Cod = Movimenti_Dettagli.Elem_Cod AND Materie_Prime.Mat_Cod = Movimenti_Dettagli.Mat_Cod   " & vbCrLf)

            StbQ.Append(" INNER JOIN Imprese_Progetti ON Reg_Impianti.PIVA = Imprese_Progetti.Piva " & vbCrLf)
            StbQ.Append("             AND Reg_Impianti.Sa_Cod = Imprese_Progetti.Sa_Cod  " & vbCrLf)
            StbQ.Append("             AND Reg_Impianti.Appezza = Imprese_Progetti.Appezza  " & vbCrLf)
            StbQ.Append("             AND Reg_Impianti.Id_Reg = Imprese_Progetti.Id_reg " & vbCrLf)
            StbQ.Append(" INNER JOIN Cultivar ON Reg_Impianti.Cul_Cod = Cultivar.Cul_Cod " & vbCrLf)
            StbQ.Append(" INNER JOIN SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod " & vbCrLf)

            Aggiungi_Join_OI_Pomodoro(StbQ, Mostra_varieta_OI_Pomodoro)

            '/**************************************************
            '         NUOVO METODO CON TABELLA TEMPORANEA

            '           --> AGGIUNGO QUESTA PARTE:

            StbQ.Append(" INNER JOIN  #tempimpianti " & vbCrLf)
            StbQ.Append(" ON #tempimpianti.Piva = Reg_Impianti.Piva AND Reg_Impianti.Sa_Cod = #tempimpianti.Sa_Cod AND  Reg_Impianti.Appezza = #tempimpianti.Appezza AND  Reg_Impianti.Id_Reg = #tempimpianti.Id_Reg " & vbCrLf)
            '/**************************************************

            StbQ.Append(" WHERE  Agenda.Lav_Cod IN (" & Agro_SQL_SaveNum(LAVCOD_SEMINA) & ", " & vbCrLf)
            StbQ.Append("                           " & Agro_SQL_SaveNum(LAVCOD_TRAPIANTO) & ", " & vbCrLf)
            StbQ.Append("                           " & Agro_SQL_SaveNum(LAVCOD_SOVESCIO) & ", " & vbCrLf)
            StbQ.Append("                           " & Agro_SQL_SaveNum(LAVCOD_SOD_SEDDING) & " " & vbCrLf)
            StbQ.Append("                           ) " & vbCrLf)

            'bisogna filtrare la distinta attiva alla data dell'operazione
            '(altrimenti possono essere letti più distinte per impianto)
            StbQ.Append(" AND	Movimenti.Data_Movimento >= Imprese_Progetti.Validita_Inizio " & vbCrLf)
            StbQ.Append(" AND	Movimenti.Data_Movimento <= Imprese_Progetti.Validita_Fine " & vbCrLf)

            StbQ.Append(" AND Movimenti_Dettagli.MAT_COD = 0 " & vbCrLf)

            StbQ.Append(" )  " & vbCrLf)


            StbQ.Append(" UNION ALL " & vbCrLf)

            '----------------------------------------------------------------------------------------------------------
            '------- TERZA E QUARTA PARTE : IMPIANTI CON SPECIE CON SEMINE CON MAGAZZINO -------
            '----------------------------------------------------------------------------------------------------------
            StbQ.Append("  " & vbCrLf)
            StbQ.Append("   " & vbCrLf)
            StbQ.Append(" --  TERZA E QUARTA PARTE : IMPIANTI CON SPECIE CON SEMINE CON MAGAZZINO " & vbCrLf)
            StbQ.Append(" (  " & vbCrLf)
            StbQ.Append(" SELECT  " & vbCrLf)
            StbQ.Append(" Imprese.rag_soc, Centri_Aziendali.sa_nome,   " & vbCrLf)
            StbQ.Append(" Appezzamento.APP_NOME,  Appezzamento.Campo_Cod,  " & vbCrLf)
            'StbQ.Append(" ISNULL( (SELECT Campi.Campo_Des " & vbCrLf)
            'StbQ.Append("           FROM Campi " & vbCrLf)
            'StbQ.Append("           WHERE Appezzamento.PIVA = Campi.Piva AND Appezzamento.SA_COD = Campi.Sa_Cod AND Appezzamento.Campo_Cod = Campi.Campo_Cod " & vbCrLf)
            'StbQ.Append("           ) , '' ) AS Campo_Des, " & vbCrLf)
            StbQ.Append(" Reg_Impianti.PIVA, Reg_Impianti.SA_COD, Reg_Impianti.APPEZZA, Reg_Impianti.ID_REG, " & vbCrLf)
            StbQ.Append(" Reg_Impianti.GRVA_Cod_VEG, Reg_Impianti.Sup_Imp,  " & vbCrLf)
            StbQ.Append(" Reg_Impianti.Validita_Inizio AS Reg_Impianti_Validita_Inizio, Reg_Impianti.Validita_Fine AS Reg_Impianti_Validita_Fine,  Imprese_Progetti.regolamento_cod, " & vbCrLf)
            'StbQ.Append(" ISNULL( (SELECT Grva_Des " & vbCrLf)
            'StbQ.Append("           FROM GruppoVarietale " & vbCrLf)
            'StbQ.Append("           WHERE GruppoVarietale.Grva_Cod = ABS(Reg_Impianti.GRVA_Cod_VEG) " & vbCrLf)
            'StbQ.Append("           ) , '' ) AS Grva_Des, " & vbCrLf)
            StbQ.Append(" Imprese_Progetti.P_HA, Imprese_Progetti.Produzione_Prevista AS Resa_Prevista_HA,  " & vbCrLf)
            StbQ.Append(" SpecieVegetali.Veg_Cod, SpecieVegetali.Veg_Des, Reg_Impianti.CUL_COD, Cultivar.Cul_Des, " & vbCrLf)
            StbQ.Append(" Agenda.Id_Agenda, Agenda.Lav_Cod, Movimenti.Data_Movimento, " & vbCrLf)
            StbQ.Append(" Movimenti_dettagli.Udm_Cod, " & vbCrLf)
            StbQ.Append(" Mov_Destinazioni.Qta AS Qta_Impianto, " & vbCrLf)
            StbQ.Append(" Mov_Destinazioni.Qta2 AS SemTrap_Superficie,  " & vbCrLf)
            StbQ.Append(" Movimenti_dettagli.Cal_Cod, ISNULL(Movimenti_dettagli.Lotto,'') AS Lotto, " & vbCrLf)
            StbQ.Append(" ISNULL(Materie_Prime.Mat_Cod, 0) AS Mat_Cod, ISNULL(Materie_Prime.Mat_Des,'') AS Mat_Des, ISNULL(Materie_Prime.Cod_Articolo,'') AS Cod_Articolo, " & vbCrLf)
            StbQ.Append(" UnitaMisura.UDM_SIM,   " & vbCrLf)
            StbQ.Append(" ''  AS Cal_Des " & vbCrLf)
            ' StbQ.Append("  Contatti.Rag_Soc + Contatti.Nome + ' ' + Contatti.Cognome AS Fornitore  " & vbCrLf)

            If Mostra_varieta_OI_Pomodoro Then
                StbQ.Append(" ,ISNULL(Codifica_Varieta_OIPomodorodaIndustriaNordItalia.Cod_Varieta, 0) AS Cul_Cod_OI_Pomodoro, ISNULL(Codifica_Varieta_OIPomodorodaIndustriaNordItalia.Desc_Varieta, '') AS Cul_Des_OI_Pomodoro " & vbCrLf)
                StbQ.Append(" ,ISNULL(Codifica_Varieta_OIPomodorodaIndustriaNordItalia.grva_cod_gias, 0) AS grva_cod_cultivar " & vbCrLf)
            Else
                StbQ.Append(" ,0 AS Cul_Cod_OI_Pomodoro, '' AS Cul_Des_OI_Pomodoro " & vbCrLf)
                StbQ.Append(" ,0 AS grva_cod_cultivar " & vbCrLf)
            End If

            StbQ.Append(" FROM Imprese  " & vbCrLf)
            StbQ.Append(" INNER JOIN Centri_Aziendali ON Centri_Aziendali.Piva = Imprese.Piva  " & vbCrLf)
            StbQ.Append(" INNER JOIN Appezzamento ON Centri_Aziendali.SA_COD = Appezzamento.SA_COD AND Centri_Aziendali.PIVA = Appezzamento.PIVA " & vbCrLf)
            StbQ.Append(" INNER JOIN Reg_Impianti ON Reg_Impianti.APPEZZA = Appezzamento.APPEZZA AND Reg_Impianti.SA_COD = Appezzamento.SA_COD AND " & vbCrLf)
            StbQ.Append(" Reg_Impianti.PIVA = Appezzamento.PIVA " & vbCrLf)

            StbQ.Append(" INNER JOIN Mov_Destinazioni ON Reg_Impianti.Piva = Mov_Destinazioni.PIVA AND Reg_Impianti.Sa_Cod = Mov_Destinazioni.Sa_Cod AND Mov_Destinazioni.Appezza = Reg_Impianti.Appezza AND Mov_Destinazioni.Id_Destinazione = Reg_Impianti.Id_Reg  " & vbCrLf)
            StbQ.Append(" INNER JOIN Movimenti_dettagli ON Mov_Destinazioni.Piva = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Sa_Cod = Movimenti_dettagli.Sa_Cod AND Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det  " & vbCrLf)
            StbQ.Append(" INNER JOIN Movimenti ON Movimenti.PIVA = Movimenti_Dettagli.PIVA AND Movimenti_dettagli.Sa_Cod = Movimenti.Sa_Cod AND Movimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov  " & vbCrLf)
            StbQ.Append(" INNER JOIN Agenda ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod  AND Agenda.Id_Agenda = Movimenti.Id_Agenda  " & vbCrLf)

            StbQ.Append(" INNER JOIN UnitaMisura ON UnitaMisura.Udm_Cod = Movimenti_Dettagli.Udm_Cod " & vbCrLf)
            StbQ.Append(" INNER JOIN Materie_Prime ON Materie_Prime.Elem_Cod = Movimenti_Dettagli.Elem_Cod AND Materie_Prime.Mat_Cod = Movimenti_Dettagli.Mat_Cod   " & vbCrLf)

            'StbQ.Append(" INNER JOIN Movimenti_dettagli MovDetAcquisto ON Materie_Prime.Elem_Cod = MovDetAcquisto.Elem_Cod AND Materie_Prime.Mat_Cod = MovDetAcquisto.Mat_Cod   " & vbCrLf)
            'StbQ.Append(" INNER JOIN Movimenti MovMagAcquisto ON MovMagAcquisto.PIVA = MovDetAcquisto.PIVA AND MovMagAcquisto.Id_Agenda = MovDetAcquisto.Id_Agenda AND MovMagAcquisto.Id_Mov = MovDetAcquisto.Id_Mov  " & vbCrLf)
            'StbQ.Append(" INNER JOIN Agenda AgendaAcquisto ON AgendaAcquisto.PIVA = MovMagAcquisto.PIVA  AND AgendaAcquisto.Id_Agenda = MovMagAcquisto.Id_Agenda  " & vbCrLf)
            'StbQ.Append(" INNER JOIN Movimenti MovContAcquisto ON AgendaAcquisto.PIVA = MovContAcquisto.PIVA  AND AgendaAcquisto.Id_Agenda = MovContAcquisto.Id_Agenda  " & vbCrLf)
            'StbQ.Append(" INNER JOIN Risorse_Umane ON MovContAcquisto.Cod_RisUm = Risorse_Umane.Cod_RisUm  " & vbCrLf)
            'StbQ.Append(" INNER JOIN Contatti ON Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto  " & vbCrLf)

            StbQ.Append(" INNER JOIN Imprese_Progetti ON Reg_Impianti.PIVA = Imprese_Progetti.Piva " & vbCrLf)
            StbQ.Append("             AND Reg_Impianti.Sa_Cod = Imprese_Progetti.Sa_Cod  " & vbCrLf)
            StbQ.Append("             AND Reg_Impianti.Appezza = Imprese_Progetti.Appezza  " & vbCrLf)
            StbQ.Append("             AND Reg_Impianti.Id_Reg = Imprese_Progetti.Id_reg " & vbCrLf)
            StbQ.Append(" INNER JOIN Cultivar ON Reg_Impianti.Cul_Cod = Cultivar.Cul_Cod " & vbCrLf)
            StbQ.Append(" INNER JOIN SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod " & vbCrLf)

            Aggiungi_Join_OI_Pomodoro(StbQ, Mostra_varieta_OI_Pomodoro)

            '/**************************************************
            '         NUOVO METODO CON TABELLA TEMPORANEA

            '           --> AGGIUNGO QUESTA PARTE:

            StbQ.Append(" INNER JOIN  #tempimpianti " & vbCrLf)
            StbQ.Append(" ON #tempimpianti.Piva = Reg_Impianti.Piva AND Reg_Impianti.Sa_Cod = #tempimpianti.Sa_Cod AND  Reg_Impianti.Appezza = #tempimpianti.Appezza AND  Reg_Impianti.Id_Reg = #tempimpianti.Id_Reg " & vbCrLf)
            '/**************************************************

            StbQ.Append(" WHERE  Agenda.Lav_Cod IN (" & Agro_SQL_SaveNum(LAVCOD_SEMINA) & ", " & vbCrLf)
            StbQ.Append("                           " & Agro_SQL_SaveNum(LAVCOD_TRAPIANTO) & ", " & vbCrLf)
            StbQ.Append("                           " & Agro_SQL_SaveNum(LAVCOD_SOVESCIO) & ", " & vbCrLf)
            StbQ.Append("                           " & Agro_SQL_SaveNum(LAVCOD_SOD_SEDDING) & " " & vbCrLf)
            StbQ.Append("                           ) " & vbCrLf)

            'bisogna filtrare la distinta attiva alla data dell'operazione
            '(altrimenti possono essere letti più distinte per impianto)
            StbQ.Append(" AND	Movimenti.Data_Movimento >= Imprese_Progetti.Validita_Inizio " & vbCrLf)
            StbQ.Append(" AND	Movimenti.Data_Movimento <= Imprese_Progetti.Validita_Fine " & vbCrLf)

            'StbQ.Append(" AND	MovMagAcquisto.cau_mov = '" & Agro_SQL_SaveText(CAU_CARICO) & "' " & vbCrLf)
            'StbQ.Append(" AND	MovContAcquisto.cau_mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "' " & vbCrLf)
            'StbQ.Append(" AND  AgendaAcquisto.Lav_Cod IN ( " + _
            '                   CStr(LAVCOD_BOLLA_RICEVUTA) + "," + _
            '                   CStr(LAVCOD_FATTURA_RICEVUTA) + "" + _
            '                    ")" & vbCrLf)

            StbQ.Append(" )  " & vbCrLf)



            StbQ.Append(" UNION ALL  " & vbCrLf)



            ''----------------------------------------------------------------------------------------------------------
            ''------- TERZA PARTE : IMPIANTI CON SPECIE CON SEMINE CON MAGAZZINO CON ACQUISTO DA FORNITORI  --------
            ''----------------------------------------------------------------------------------------------------------
            'StbQ.Append("  " & vbCrLf)
            'StbQ.Append("   " & vbCrLf)
            'StbQ.Append(" --  TERZA PARTE : IMPIANTI CON SPECIE CON SEMINE CON MAGAZZINO CON ACQUISTO DA FORNITORI " & vbCrLf)
            'StbQ.Append(" (  " & vbCrLf)
            'StbQ.Append(" SELECT  " & vbCrLf)
            'StbQ.Append(" Imprese.rag_soc, Centri_Aziendali.sa_nome,   " & vbCrLf)
            'StbQ.Append(" Appezzamento.APP_NOME,  Appezzamento.Campo_Cod,  " & vbCrLf)
            ''StbQ.Append(" ISNULL( (SELECT Campi.Campo_Des " & vbCrLf)
            ''StbQ.Append("           FROM Campi " & vbCrLf)
            ''StbQ.Append("           WHERE Appezzamento.PIVA = Campi.Piva AND Appezzamento.SA_COD = Campi.Sa_Cod AND Appezzamento.Campo_Cod = Campi.Campo_Cod " & vbCrLf)
            ''StbQ.Append("           ) , '' ) AS Campo_Des, " & vbCrLf)
            'StbQ.Append(" Reg_Impianti.PIVA, Reg_Impianti.SA_COD, Reg_Impianti.APPEZZA, Reg_Impianti.ID_REG, " & vbCrLf)
            'StbQ.Append(" Reg_Impianti.GRVA_Cod_VEG, Reg_Impianti.Sup_Imp,  " & vbCrLf)
            'StbQ.Append(" Reg_Impianti.Validita_Inizio AS Reg_Impianti_Validita_Inizio, Reg_Impianti.Validita_Fine AS Reg_Impianti_Validita_Fine, " & vbCrLf)
            ''StbQ.Append(" ISNULL( (SELECT Grva_Des " & vbCrLf)
            ''StbQ.Append("           FROM GruppoVarietale " & vbCrLf)
            ''StbQ.Append("           WHERE GruppoVarietale.Grva_Cod = ABS(Reg_Impianti.GRVA_Cod_VEG) " & vbCrLf)
            ''StbQ.Append("           ) , '' ) AS Grva_Des, " & vbCrLf)
            'StbQ.Append(" Imprese_Progetti.P_HA, Imprese_Progetti.Produzione_Prevista AS Resa_Prevista_HA,  " & vbCrLf)
            'StbQ.Append(" SpecieVegetali.Veg_Cod, SpecieVegetali.Veg_Des, Reg_Impianti.CUL_COD, Cultivar.Cul_Des, " & vbCrLf)
            'StbQ.Append(" Agenda.Id_Agenda, Agenda.Lav_Cod, Movimenti.Data_Movimento, " & vbCrLf)
            'StbQ.Append(" Movimenti_dettagli.Udm_Cod, " & vbCrLf)
            'StbQ.Append(" Mov_Destinazioni.Qta AS Qta_Impianto, " & vbCrLf)
            'StbQ.Append(" Movimenti_dettagli.Cal_Cod, ISNULL(Movimenti_dettagli.Lotto,'') AS Lotto, " & vbCrLf)
            'StbQ.Append(" ISNULL(Materie_Prime.Mat_Cod, 0) AS Mat_Cod, ISNULL(Materie_Prime.Mat_Des,'') AS Mat_Des, ISNULL(Materie_Prime.Cod_Articolo,'') AS Cod_Articolo, " & vbCrLf)
            'StbQ.Append(" UnitaMisura.UDM_SIM,   " & vbCrLf)
            'StbQ.Append(" ''  AS Cal_Des, Contatti.Rag_Soc + Contatti.Nome + ' ' + Contatti.Cognome AS Fornitore  " & vbCrLf)

            'StbQ.Append(" FROM Imprese  " & vbCrLf)
            'StbQ.Append(" INNER JOIN Centri_Aziendali ON Centri_Aziendali.Piva = Imprese.Piva  " & vbCrLf)
            'StbQ.Append(" INNER JOIN Appezzamento ON Centri_Aziendali.SA_COD = Appezzamento.SA_COD AND Centri_Aziendali.PIVA = Appezzamento.PIVA " & vbCrLf)
            'StbQ.Append(" INNER JOIN Reg_Impianti ON Reg_Impianti.APPEZZA = Appezzamento.APPEZZA AND Reg_Impianti.SA_COD = Appezzamento.SA_COD AND " & vbCrLf)
            'StbQ.Append(" Reg_Impianti.PIVA = Appezzamento.PIVA " & vbCrLf)

            'StbQ.Append(" INNER JOIN Mov_Destinazioni ON Reg_Impianti.Piva = Mov_Destinazioni.PIVA AND Reg_Impianti.Sa_Cod = Mov_Destinazioni.Sa_Cod AND Mov_Destinazioni.Appezza = Reg_Impianti.Appezza AND Mov_Destinazioni.Id_Destinazione = Reg_Impianti.Id_Reg  " & vbCrLf)
            'StbQ.Append(" INNER JOIN Movimenti_dettagli ON Mov_Destinazioni.Piva = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Sa_Cod = Movimenti_dettagli.Sa_Cod AND Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det  " & vbCrLf)
            'StbQ.Append(" INNER JOIN Movimenti ON Movimenti.PIVA = Movimenti_Dettagli.PIVA AND Movimenti_dettagli.Sa_Cod = Movimenti.Sa_Cod AND Movimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov  " & vbCrLf)
            'StbQ.Append(" INNER JOIN Agenda ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod  AND Agenda.Id_Agenda = Movimenti.Id_Agenda  " & vbCrLf)

            'StbQ.Append(" INNER JOIN UnitaMisura ON UnitaMisura.Udm_Cod = Movimenti_Dettagli.Udm_Cod " & vbCrLf)
            'StbQ.Append(" INNER JOIN Materie_Prime ON Materie_Prime.Elem_Cod = Movimenti_Dettagli.Elem_Cod AND Materie_Prime.Mat_Cod = Movimenti_Dettagli.Mat_Cod   " & vbCrLf)


            'StbQ.Append(" INNER JOIN Movimenti_dettagli MovDetAcquisto ON Materie_Prime.Elem_Cod = MovDetAcquisto.Elem_Cod AND Materie_Prime.Mat_Cod = MovDetAcquisto.Mat_Cod   " & vbCrLf)
            'StbQ.Append(" INNER JOIN Movimenti MovMagAcquisto ON MovMagAcquisto.PIVA = MovDetAcquisto.PIVA AND MovMagAcquisto.Id_Agenda = MovDetAcquisto.Id_Agenda AND MovMagAcquisto.Id_Mov = MovDetAcquisto.Id_Mov  " & vbCrLf)
            'StbQ.Append(" INNER JOIN Agenda AgendaAcquisto ON AgendaAcquisto.PIVA = MovMagAcquisto.PIVA  AND AgendaAcquisto.Id_Agenda = MovMagAcquisto.Id_Agenda  " & vbCrLf)
            'StbQ.Append(" INNER JOIN Movimenti MovContAcquisto ON AgendaAcquisto.PIVA = MovContAcquisto.PIVA  AND AgendaAcquisto.Id_Agenda = MovContAcquisto.Id_Agenda  " & vbCrLf)
            'StbQ.Append(" INNER JOIN Risorse_Umane ON MovContAcquisto.Cod_RisUm = Risorse_Umane.Cod_RisUm  " & vbCrLf)
            'StbQ.Append(" INNER JOIN Contatti ON Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto  " & vbCrLf)

            'StbQ.Append(" INNER JOIN Imprese_Progetti ON Reg_Impianti.PIVA = Imprese_Progetti.Piva " & vbCrLf)
            'StbQ.Append("             AND Reg_Impianti.Sa_Cod = Imprese_Progetti.Sa_Cod  " & vbCrLf)
            'StbQ.Append("             AND Reg_Impianti.Appezza = Imprese_Progetti.Appezza  " & vbCrLf)
            'StbQ.Append("             AND Reg_Impianti.Id_Reg = Imprese_Progetti.Id_reg " & vbCrLf)
            'StbQ.Append(" INNER JOIN Cultivar ON Reg_Impianti.Cul_Cod = Cultivar.Cul_Cod " & vbCrLf)
            'StbQ.Append(" INNER JOIN SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod " & vbCrLf)

            ''/**************************************************
            ''         NUOVO METODO CON TABELLA TEMPORANEA

            ''           --> AGGIUNGO QUESTA PARTE:

            'StbQ.Append(" INNER JOIN  #tempimpianti " & vbCrLf)
            'StbQ.Append(" ON #tempimpianti.Piva = Reg_Impianti.Piva AND Reg_Impianti.Sa_Cod = #tempimpianti.Sa_Cod AND  Reg_Impianti.Appezza = #tempimpianti.Appezza AND  Reg_Impianti.Id_Reg = #tempimpianti.Id_Reg " & vbCrLf)
            ''/**************************************************

            'StbQ.Append(" WHERE  Agenda.Lav_Cod IN (" & Agro_SQL_SaveNum(LAVCOD_SEMINA) & ", " & vbCrLf)
            'StbQ.Append("                           " & Agro_SQL_SaveNum(LAVCOD_TRAPIANTO) & " " & vbCrLf)
            'StbQ.Append("                           ) " & vbCrLf)

            ''bisogna filtrare la distinta attiva alla data dell'operazione
            ''(altrimenti possono essere letti più distinte per impianto)
            'StbQ.Append(" AND	Movimenti.Data_Movimento >= Imprese_Progetti.Validita_Inizio " & vbCrLf)
            'StbQ.Append(" AND	Movimenti.Data_Movimento <= Imprese_Progetti.Validita_Fine " & vbCrLf)

            'StbQ.Append(" AND	MovMagAcquisto.cau_mov = '" & Agro_SQL_SaveText(CAU_CARICO) & "' " & vbCrLf)
            'StbQ.Append(" AND	MovContAcquisto.cau_mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "' " & vbCrLf)
            'StbQ.Append(" AND  AgendaAcquisto.Lav_Cod IN ( " + _
            '                   CStr(LAVCOD_BOLLA_RICEVUTA) + "," + _
            '                   CStr(LAVCOD_FATTURA_RICEVUTA) + "" + _
            '                    ")" & vbCrLf)

            'StbQ.Append(" )  " & vbCrLf)



            'StbQ.Append(" UNION ALL  " & vbCrLf)

            ''----------------------------------------------------------------------------------------------------------
            ''------- QUARTA PARTE: IMPIANTI CON SPECIE CON SEMINE CON MAGAZZINO SENZA ACQUISTO DA FORNITORI  --------
            ''----------------------------------------------------------------------------------------------------------
            'StbQ.Append("  " & vbCrLf)
            'StbQ.Append("   " & vbCrLf)
            'StbQ.Append(" -- QUARTA PARTE: IMPIANTI CON SPECIE CON SEMINE CON MAGAZZINO SENZA ACQUISTO DA FORNITORI " & vbCrLf)
            'StbQ.Append(" (  " & vbCrLf)
            'StbQ.Append(" SELECT  " & vbCrLf)
            'StbQ.Append(" Imprese.rag_soc, Centri_Aziendali.sa_nome,   " & vbCrLf)
            'StbQ.Append(" Appezzamento.APP_NOME,  Appezzamento.Campo_Cod,  " & vbCrLf)
            ''StbQ.Append(" ISNULL( (SELECT Campi.Campo_Des " & vbCrLf)
            ''StbQ.Append("           FROM Campi " & vbCrLf)
            ''StbQ.Append("           WHERE Appezzamento.PIVA = Campi.Piva AND Appezzamento.SA_COD = Campi.Sa_Cod AND Appezzamento.Campo_Cod = Campi.Campo_Cod " & vbCrLf)
            ''StbQ.Append("           ) , '' ) AS Campo_Des, " & vbCrLf)
            'StbQ.Append(" Reg_Impianti.PIVA, Reg_Impianti.SA_COD, Reg_Impianti.APPEZZA, Reg_Impianti.ID_REG, " & vbCrLf)
            'StbQ.Append(" Reg_Impianti.GRVA_Cod_VEG, Reg_Impianti.Sup_Imp,  " & vbCrLf)
            'StbQ.Append(" Reg_Impianti.Validita_Inizio AS Reg_Impianti_Validita_Inizio, Reg_Impianti.Validita_Fine AS Reg_Impianti_Validita_Fine, " & vbCrLf)
            ''StbQ.Append(" ISNULL( (SELECT Grva_Des " & vbCrLf)
            ''StbQ.Append("           FROM GruppoVarietale " & vbCrLf)
            ''StbQ.Append("           WHERE GruppoVarietale.Grva_Cod = ABS(Reg_Impianti.GRVA_Cod_VEG) " & vbCrLf)
            ''StbQ.Append("           ) , '' ) AS Grva_Des, " & vbCrLf)
            'StbQ.Append(" Imprese_Progetti.P_HA, Imprese_Progetti.Produzione_Prevista AS Resa_Prevista_HA,  " & vbCrLf)
            'StbQ.Append(" SpecieVegetali.Veg_Cod, SpecieVegetali.Veg_Des, Reg_Impianti.CUL_COD, Cultivar.Cul_Des, " & vbCrLf)
            'StbQ.Append("  Agenda.Id_Agenda, Agenda.Lav_Cod,  Movimenti.Data_Movimento, " & vbCrLf)
            'StbQ.Append(" Movimenti_dettagli.Udm_Cod, " & vbCrLf)
            'StbQ.Append(" Mov_Destinazioni.Qta AS Qta_Impianto,  " & vbCrLf)
            'StbQ.Append(" Movimenti_dettagli.Cal_Cod, ISNULL(Movimenti_dettagli.Lotto,'') AS Lotto, " & vbCrLf)
            'StbQ.Append(" ISNULL(Materie_Prime.Mat_Cod, 0) AS Mat_Cod, ISNULL(Materie_Prime.Mat_Des,'') AS Mat_Des, ISNULL(Materie_Prime.Cod_Articolo,'') AS Cod_Articolo, " & vbCrLf)
            'StbQ.Append(" UnitaMisura.UDM_SIM,   " & vbCrLf)
            'StbQ.Append(" ''  AS Cal_Des, '' AS Fornitore  " & vbCrLf)

            'StbQ.Append(" FROM Imprese  " & vbCrLf)
            'StbQ.Append(" INNER JOIN Centri_Aziendali ON Centri_Aziendali.Piva = Imprese.Piva  " & vbCrLf)
            'StbQ.Append(" INNER JOIN Appezzamento ON Centri_Aziendali.SA_COD = Appezzamento.SA_COD AND Centri_Aziendali.PIVA = Appezzamento.PIVA " & vbCrLf)
            'StbQ.Append(" INNER JOIN Reg_Impianti ON Reg_Impianti.APPEZZA = Appezzamento.APPEZZA AND Reg_Impianti.SA_COD = Appezzamento.SA_COD AND " & vbCrLf)
            'StbQ.Append(" Reg_Impianti.PIVA = Appezzamento.PIVA " & vbCrLf)

            'StbQ.Append(" INNER JOIN Mov_Destinazioni ON Reg_Impianti.Piva = Mov_Destinazioni.PIVA AND Reg_Impianti.Sa_Cod = Mov_Destinazioni.Sa_Cod AND Mov_Destinazioni.Appezza = Reg_Impianti.Appezza AND Mov_Destinazioni.Id_Destinazione = Reg_Impianti.Id_Reg  " & vbCrLf)
            'StbQ.Append(" INNER JOIN Movimenti_dettagli ON Mov_Destinazioni.Piva = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Sa_Cod = Movimenti_dettagli.Sa_Cod AND Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det  " & vbCrLf)
            'StbQ.Append(" INNER JOIN Movimenti ON Movimenti.PIVA = Movimenti_Dettagli.PIVA AND Movimenti_dettagli.Sa_Cod = Movimenti.Sa_Cod AND Movimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov  " & vbCrLf)
            'StbQ.Append(" INNER JOIN Agenda ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod  AND Agenda.Id_Agenda = Movimenti.Id_Agenda  " & vbCrLf)

            'StbQ.Append(" INNER JOIN UnitaMisura ON UnitaMisura.Udm_Cod = Movimenti_Dettagli.Udm_Cod " & vbCrLf)
            'StbQ.Append(" INNER JOIN Materie_Prime ON Materie_Prime.Elem_Cod = Movimenti_Dettagli.Elem_Cod AND Materie_Prime.Mat_Cod = Movimenti_Dettagli.Mat_Cod   " & vbCrLf)

            'StbQ.Append(" INNER JOIN Imprese_Progetti ON Reg_Impianti.PIVA = Imprese_Progetti.Piva " & vbCrLf)
            'StbQ.Append("             AND Reg_Impianti.Sa_Cod = Imprese_Progetti.Sa_Cod  " & vbCrLf)
            'StbQ.Append("             AND Reg_Impianti.Appezza = Imprese_Progetti.Appezza  " & vbCrLf)
            'StbQ.Append("             AND Reg_Impianti.Id_Reg = Imprese_Progetti.Id_reg " & vbCrLf)
            'StbQ.Append(" INNER JOIN Cultivar ON Reg_Impianti.Cul_Cod = Cultivar.Cul_Cod " & vbCrLf)
            'StbQ.Append(" INNER JOIN SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod " & vbCrLf)

            ''/**************************************************
            ''         NUOVO METODO CON TABELLA TEMPORANEA

            ''           --> AGGIUNGO QUESTA PARTE:

            'StbQ.Append(" INNER JOIN  #tempimpianti " & vbCrLf)
            'StbQ.Append(" ON #tempimpianti.Piva = Reg_Impianti.Piva AND Reg_Impianti.Sa_Cod = #tempimpianti.Sa_Cod AND  Reg_Impianti.Appezza = #tempimpianti.Appezza AND  Reg_Impianti.Id_Reg = #tempimpianti.Id_Reg " & vbCrLf)
            ''/**************************************************

            'StbQ.Append(" WHERE  Agenda.Lav_Cod IN (" & Agro_SQL_SaveNum(LAVCOD_SEMINA) & ", " & vbCrLf)
            'StbQ.Append("                           " & Agro_SQL_SaveNum(LAVCOD_TRAPIANTO) & " " & vbCrLf)
            'StbQ.Append("                           ) " & vbCrLf)

            ''bisogna filtrare la distinta attiva alla data dell'operazione
            ''(altrimenti possono essere letti più distinte per impianto)
            'StbQ.Append(" AND	Movimenti.Data_Movimento >= Imprese_Progetti.Validita_Inizio " & vbCrLf)
            'StbQ.Append(" AND	Movimenti.Data_Movimento <= Imprese_Progetti.Validita_Fine " & vbCrLf)


            'StbQ.Append(" AND NOT EXISTS (  " & vbCrLf)
            'StbQ.Append("                   SELECT 1 " & vbCrLf)
            'StbQ.Append("                   FROM Movimenti_dettagli MovDetAcquisto " & vbCrLf)
            'StbQ.Append("                   INNER JOIN Movimenti MovMagAcquisto ON MovMagAcquisto.PIVA = MovDetAcquisto.PIVA AND MovMagAcquisto.Id_Agenda = MovDetAcquisto.Id_Agenda AND MovMagAcquisto.Id_Mov = MovDetAcquisto.Id_Mov  " & vbCrLf)
            'StbQ.Append("                   INNER JOIN Agenda AgendaAcquisto ON AgendaAcquisto.PIVA = MovMagAcquisto.PIVA  AND AgendaAcquisto.Id_Agenda = MovMagAcquisto.Id_Agenda  " & vbCrLf)
            'StbQ.Append("                   INNER JOIN Movimenti MovContAcquisto ON AgendaAcquisto.PIVA = MovContAcquisto.PIVA  AND AgendaAcquisto.Id_Agenda = MovContAcquisto.Id_Agenda  " & vbCrLf)
            'StbQ.Append("                   INNER JOIN Risorse_Umane ON MovContAcquisto.Cod_RisUm = Risorse_Umane.Cod_RisUm  " & vbCrLf)
            'StbQ.Append("                   INNER JOIN Contatti ON Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto  " & vbCrLf)
            'StbQ.Append("                   WHERE Movimenti_Dettagli.Elem_Cod = MovDetAcquisto.Elem_Cod AND Movimenti_Dettagli.Mat_Cod = MovDetAcquisto.Mat_Cod      AND Movimenti_Dettagli.UDM_Cod = MovDetAcquisto.udm_Cod  " & vbCrLf)
            'StbQ.Append("                   AND	MovMagAcquisto.cau_mov = '" & Agro_SQL_SaveText(CAU_CARICO) & "' " & vbCrLf)
            'StbQ.Append("                   AND	MovContAcquisto.cau_mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "' " & vbCrLf)
            'StbQ.Append("                   AND  AgendaAcquisto.Lav_Cod IN ( " + _
            '                                                                CStr(LAVCOD_BOLLA_RICEVUTA) + "," + _
            '                                                                CStr(LAVCOD_FATTURA_RICEVUTA) + "" + _
            '                                                                ")" & vbCrLf)
            'StbQ.Append("                   )  " & vbCrLf)

            'StbQ.Append(" )  " & vbCrLf)

            'StbQ.Append(" UNION ALL " & vbCrLf)


            '---------------------------------------------------------------
            '---------------- QUINTA PARTE: TERRENO NUDO  -----------------
            '(non c'è il join con specie e varietà e con le operazioni)
            'legge la destinazione d'uso 
            '---------------------------------------------------------------
            StbQ.Append("  " & vbCrLf)
            StbQ.Append("   " & vbCrLf)
            StbQ.Append(" -- QUINTA PARTE: TERRENO NUDO " & vbCrLf)
            StbQ.Append(" (  " & vbCrLf)
            StbQ.Append(" SELECT  " & vbCrLf)
            StbQ.Append(" Imprese.rag_soc, Centri_Aziendali.sa_nome,   " & vbCrLf)
            StbQ.Append(" Appezzamento.APP_NOME,  Appezzamento.Campo_Cod,  " & vbCrLf)
            'StbQ.Append(" ISNULL( (SELECT Campi.Campo_Des " & vbCrLf)
            'StbQ.Append("           FROM Campi " & vbCrLf)
            'StbQ.Append("           WHERE Appezzamento.PIVA = Campi.Piva AND Appezzamento.SA_COD = Campi.Sa_Cod AND Appezzamento.Campo_Cod = Campi.Campo_Cod " & vbCrLf)
            'StbQ.Append("           ) , '' ) AS Campo_Des, " & vbCrLf)
            StbQ.Append(" Reg_Impianti.PIVA, Reg_Impianti.SA_COD, Reg_Impianti.APPEZZA, Reg_Impianti.ID_REG, " & vbCrLf)
            StbQ.Append(" Reg_Impianti.GRVA_Cod_VEG, Reg_Impianti.Sup_Imp,  " & vbCrLf)
            StbQ.Append(" Reg_Impianti.Validita_Inizio AS Reg_Impianti_Validita_Inizio, Reg_Impianti.Validita_Fine AS Reg_Impianti_Validita_Fine,  Imprese_Progetti.regolamento_cod, " & vbCrLf)
            StbQ.Append(" Imprese_Progetti.P_HA, Imprese_Progetti.Produzione_Prevista AS Resa_Prevista_HA,  " & vbCrLf)
            StbQ.Append(" 0 AS Veg_Cod,  " & vbCrLf)
            StbQ.Append(" ISNULL( (SELECT Codici_Anagrafe.descrizione " & vbCrLf)
            StbQ.Append("           FROM Reg_Impianti_Codici " & vbCrLf)
            StbQ.Append("           INNER JOIN Codici_Anagrafe ON Reg_Impianti_Codici.ID_COD = Codici_Anagrafe.Codice  " & vbCrLf)
            StbQ.Append("           WHERE Reg_Impianti_Codici.PIVA = Reg_Impianti.Piva AND Reg_Impianti_Codici.SA_COD = Reg_Impianti.Sa_Cod AND Reg_Impianti_Codici.APPEZZa = Reg_Impianti.appezza AND Reg_Impianti_Codici.Id_Reg = Reg_Impianti.Id_Reg " & vbCrLf)
            StbQ.Append("           AND Codici_Anagrafe.Codice >= 3000 AND Codici_Anagrafe.Codice <4000  " & vbCrLf)
            StbQ.Append("           AND Codici_Anagrafe.Gruppo = 'TERRENO'  " & vbCrLf)
            StbQ.Append("           ) , '' ) AS Veg_Des, " & vbCrLf)
            StbQ.Append(" Reg_Impianti.CUL_COD, '' AS Cul_Des, " & vbCrLf)

            StbQ.Append("  0 AS Id_Agenda, 0 AS Lav_Cod,  '01/01/1900' AS Data_Movimento, " & vbCrLf)
            StbQ.Append(" 0 AS Udm_Cod,  0  AS Qta_Impianto,  0 AS SemTrap_Superficie," & vbCrLf)
            StbQ.Append(" 0 AS Cal_Cod, '' AS Lotto, " & vbCrLf)
            StbQ.Append(" 0 AS Mat_Cod, '' AS Mat_Des, '' AS Cod_Articolo, " & vbCrLf)
            StbQ.Append(" '' AS UDM_SIM,  " & vbCrLf)
            StbQ.Append(" ''  AS Cal_Des " & vbCrLf)

            StbQ.Append(" ,0 AS Cul_Cod_OI_Pomodoro, '' AS Cul_Des_OI_Pomodoro, 0 as grva_cod_cultivar " & vbCrLf)

            StbQ.Append(" FROM Imprese  " & vbCrLf)
            StbQ.Append(" INNER JOIN Centri_Aziendali ON Centri_Aziendali.Piva = Imprese.Piva  " & vbCrLf)
            StbQ.Append(" INNER JOIN Appezzamento ON Centri_Aziendali.SA_COD = Appezzamento.SA_COD AND Centri_Aziendali.PIVA = Appezzamento.PIVA " & vbCrLf)
            StbQ.Append(" INNER JOIN Reg_Impianti ON Reg_Impianti.APPEZZA = Appezzamento.APPEZZA AND Reg_Impianti.SA_COD = Appezzamento.SA_COD AND " & vbCrLf)
            StbQ.Append(" Reg_Impianti.PIVA = Appezzamento.PIVA " & vbCrLf)
            StbQ.Append(" INNER JOIN Imprese_Progetti ON Reg_Impianti.PIVA = Imprese_Progetti.Piva " & vbCrLf)
            StbQ.Append("             AND Reg_Impianti.Sa_Cod = Imprese_Progetti.Sa_Cod  " & vbCrLf)
            StbQ.Append("             AND Reg_Impianti.Appezza = Imprese_Progetti.Appezza  " & vbCrLf)
            StbQ.Append("             AND Reg_Impianti.Id_Reg = Imprese_Progetti.Id_reg " & vbCrLf)

            '/**************************************************
            '         NUOVO METODO CON TABELLA TEMPORANEA

            '           --> AGGIUNGO QUESTA PARTE:

            StbQ.Append(" INNER JOIN  #tempimpianti " & vbCrLf)
            StbQ.Append(" ON #tempimpianti.Piva = Reg_Impianti.Piva AND Reg_Impianti.Sa_Cod = #tempimpianti.Sa_Cod AND  Reg_Impianti.Appezza = #tempimpianti.Appezza AND  Reg_Impianti.Id_Reg = #tempimpianti.Id_Reg " & vbCrLf)
            '/**************************************************

            StbQ.Append(" WHERE  Reg_Impianti.CUL_COD = 0 " & vbCrLf)

            'filtro la distinta attiva alla data odierna
            StbQ.Append(" AND	Imprese_Progetti.Validita_Inizio <=  " & Agro_SQL_SaveDate(Date.Today) & " ")
            StbQ.Append(" AND	Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(Date.Today) & " ")

            StbQ.Append(" )  " & vbCrLf)


            StbQ.Append(" UNION ALL " & vbCrLf)


            '------------------------------------------------------------------------------
            '------- SESTA PARTE: IMPIANTI CON SPECIE SENZA OPERAZIONI  --------
            '(non c'è il join con materie prime e unità di misura)
            'not exists sulle operazioni
            '-------------------------------------------------------------------------------
            StbQ.Append("  " & vbCrLf)
            StbQ.Append("   " & vbCrLf)
            StbQ.Append(" -- SESTA PARTE: IMPIANTI CON SPECIE SENZA OPERAZIONI " & vbCrLf)
            StbQ.Append(" (  " & vbCrLf)
            StbQ.Append(" SELECT  " & vbCrLf)
            StbQ.Append(" Imprese.rag_soc, Centri_Aziendali.sa_nome,   " & vbCrLf)
            StbQ.Append(" Appezzamento.APP_NOME,  Appezzamento.Campo_Cod,  " & vbCrLf)
            'StbQ.Append(" ISNULL( (SELECT Campi.Campo_Des " & vbCrLf)
            'StbQ.Append("           FROM Campi " & vbCrLf)
            'StbQ.Append("           WHERE Appezzamento.PIVA = Campi.Piva AND Appezzamento.SA_COD = Campi.Sa_Cod AND Appezzamento.Campo_Cod = Campi.Campo_Cod " & vbCrLf)
            'StbQ.Append("           ) , '' ) AS Campo_Des, " & vbCrLf)
            StbQ.Append(" Reg_Impianti.PIVA, Reg_Impianti.SA_COD, Reg_Impianti.APPEZZA, Reg_Impianti.ID_REG, " & vbCrLf)
            StbQ.Append(" Reg_Impianti.GRVA_Cod_VEG, Reg_Impianti.Sup_Imp,  " & vbCrLf)
            StbQ.Append(" Reg_Impianti.Validita_Inizio AS Reg_Impianti_Validita_Inizio, Reg_Impianti.Validita_Fine AS Reg_Impianti_Validita_Fine,  Imprese_Progetti.regolamento_cod, " & vbCrLf)
            'StbQ.Append(" ISNULL( (SELECT Grva_Des " & vbCrLf)
            'StbQ.Append("           FROM GruppoVarietale " & vbCrLf)
            'StbQ.Append("           WHERE GruppoVarietale.Grva_Cod = ABS(Reg_Impianti.GRVA_Cod_VEG) " & vbCrLf)
            'StbQ.Append("           ) , '' ) AS Grva_Des, " & vbCrLf)
            StbQ.Append(" Imprese_Progetti.P_HA, Imprese_Progetti.Produzione_Prevista AS Resa_Prevista_HA,  " & vbCrLf)
            StbQ.Append(" SpecieVegetali.Veg_Cod, SpecieVegetali.Veg_Des, Reg_Impianti.CUL_COD, Cultivar.Cul_Des, " & vbCrLf)
            StbQ.Append("  0 AS Id_Agenda, 0 AS Lav_Cod,  '01/01/1900' AS Data_Movimento, " & vbCrLf)
            StbQ.Append(" 0 AS Udm_Cod,  0  AS Qta_Impianto,  0 AS SemTrap_Superficie," & vbCrLf)
            StbQ.Append(" 0 AS Cal_Cod, '' AS Lotto, " & vbCrLf)
            StbQ.Append(" 0 AS Mat_Cod, '' AS Mat_Des, '' AS Cod_Articolo, " & vbCrLf)
            StbQ.Append(" '' AS UDM_SIM,  " & vbCrLf)
            StbQ.Append(" ''  AS Cal_Des " & vbCrLf)

            If Mostra_varieta_OI_Pomodoro Then
                StbQ.Append(" , ISNULL(Codifica_Varieta_OIPomodorodaIndustriaNordItalia.Cod_Varieta, 0) AS Cul_Cod_OI_Pomodoro, ISNULL(Codifica_Varieta_OIPomodorodaIndustriaNordItalia.Desc_Varieta, '') AS Cul_Des_OI_Pomodoro " & vbCrLf)
                StbQ.Append(" ,ISNULL(Codifica_Varieta_OIPomodorodaIndustriaNordItalia.grva_cod_gias, 0) AS  grva_cod_cultivar" & vbCrLf)
            Else
                StbQ.Append(" ,0 AS Cul_Cod_OI_Pomodoro, '' AS Cul_Des_OI_Pomodoro " & vbCrLf)
                StbQ.Append(" ,0 AS grva_cod_cultivar " & vbCrLf)
            End If

            StbQ.Append(" FROM Imprese  " & vbCrLf)
            StbQ.Append(" INNER JOIN Centri_Aziendali ON Centri_Aziendali.Piva = Imprese.Piva  " & vbCrLf)
            StbQ.Append(" INNER JOIN Appezzamento ON Centri_Aziendali.SA_COD = Appezzamento.SA_COD AND Centri_Aziendali.PIVA = Appezzamento.PIVA " & vbCrLf)
            StbQ.Append(" INNER JOIN Reg_Impianti ON Reg_Impianti.APPEZZA = Appezzamento.APPEZZA AND Reg_Impianti.SA_COD = Appezzamento.SA_COD AND " & vbCrLf)
            StbQ.Append(" Reg_Impianti.PIVA = Appezzamento.PIVA " & vbCrLf)
            StbQ.Append(" INNER JOIN Imprese_Progetti ON Reg_Impianti.PIVA = Imprese_Progetti.Piva " & vbCrLf)
            StbQ.Append("             AND Reg_Impianti.Sa_Cod = Imprese_Progetti.Sa_Cod  " & vbCrLf)
            StbQ.Append("             AND Reg_Impianti.Appezza = Imprese_Progetti.Appezza  " & vbCrLf)
            StbQ.Append("             AND Reg_Impianti.Id_Reg = Imprese_Progetti.Id_reg " & vbCrLf)
            StbQ.Append(" INNER JOIN Cultivar ON Reg_Impianti.Cul_Cod = Cultivar.Cul_Cod " & vbCrLf)
            StbQ.Append(" INNER JOIN SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod " & vbCrLf)

            Aggiungi_Join_OI_Pomodoro(StbQ, Mostra_varieta_OI_Pomodoro)

            '/**************************************************
            '         NUOVO METODO CON TABELLA TEMPORANEA

            '           --> AGGIUNGO QUESTA PARTE:

            StbQ.Append(" INNER JOIN  #tempimpianti " & vbCrLf)
            StbQ.Append(" ON #tempimpianti.Piva = Reg_Impianti.Piva AND Reg_Impianti.Sa_Cod = #tempimpianti.Sa_Cod AND  Reg_Impianti.Appezza = #tempimpianti.Appezza AND  Reg_Impianti.Id_Reg = #tempimpianti.Id_Reg " & vbCrLf)
            '/**************************************************

            StbQ.Append(" WHERE NOT EXISTS (  " & vbCrLf)
            StbQ.Append("                   SELECT 1 " & vbCrLf)
            StbQ.Append("                   FROM Mov_Destinazioni " & vbCrLf)
            StbQ.Append("                   INNER JOIN Agenda ON Agenda.PIVA = Mov_Destinazioni.PIVA AND Agenda.sa_cod = Mov_Destinazioni.sa_cod AND Agenda.Id_Agenda = Mov_Destinazioni.Id_Agenda  " & vbCrLf)
            StbQ.Append("                   WHERE Reg_Impianti.Piva = Mov_Destinazioni.PIVA AND Reg_Impianti.Sa_Cod = Mov_Destinazioni.Sa_Cod AND Mov_Destinazioni.Appezza = Reg_Impianti.Appezza AND Mov_Destinazioni.Id_Destinazione = Reg_Impianti.Id_Reg" & vbCrLf)
            StbQ.Append("                   AND  Agenda.Lav_Cod IN (" & Agro_SQL_SaveNum(LAVCOD_SEMINA) & ", " & vbCrLf)
            StbQ.Append("                                           " & Agro_SQL_SaveNum(LAVCOD_TRAPIANTO) & ", " & vbCrLf)
            StbQ.Append("                                           " & Agro_SQL_SaveNum(LAVCOD_SOVESCIO) & ", " & vbCrLf)
            StbQ.Append("                                           " & Agro_SQL_SaveNum(LAVCOD_SOD_SEDDING) & ", " & vbCrLf)
            StbQ.Append("                                           " & Agro_SQL_SaveNum(LAVCOD_RACCOLTA) & " " & vbCrLf)
            StbQ.Append("                                           ) " & vbCrLf)
            StbQ.Append("               ) " & vbCrLf)

            'filtro la distinta attiva alla data odierna
            StbQ.Append(" AND	Imprese_Progetti.Validita_Inizio <=  " & Agro_SQL_SaveDate(Date.Today) & " ")
            StbQ.Append(" AND	Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(Date.Today) & " ")
            StbQ.Append(" )  " & vbCrLf)



            StbQ.Append(" ) AS PianoColturale_Operazioni  " & vbCrLf)

            ''L'ORDER BY NON SERVE PERCHE' GLI IMPIANTI VENGONO CMQ LETTI ED ELABORATI NELL'ORDINE IN CUI ARRIVANO DAL FILTRONE
            ''StbQ.Append(" ORDER BY Imprese.rag_soc, Centri_Aziendali.sa_nome, campo_des, Appezzamento.APP_NOME, veg_des, cul_des, grva_des " & vbCrLf)
            'StbQ.Append(" --ORDER BY rag_soc, sa_nome, veg_des, cul_des " & vbCrLf)


            '/**************************************************
            Query4_TempTableJoin = StbQ.ToString

            StbQ = Nothing
            '/**************************************************

            '         NUOVO METODO CON TABELLA TEMPORANEA  


            Dim obj_MultiQuery As New AgronicaCoreDataProvider.AccessoMultiQuery
            obj_MultiQuery.SettaParametriPrecedenti(Me.DammiParametriCollezionati)
            DT = obj_MultiQuery.MLT_SelectFiltrataConTabellaTemporanea_2013(Query1_TempTableCreazione,
                                                                        Query2_TempTableIndice,
                                                                        Query3_TempTableFill,
                                                                        Query4_TempTableJoin,
                                                                        objParametri.StringaConnessione,
                                                                        MessaggioErrore)

            If MessaggioErrore <> "" Then
                Throw New Exception(MessaggioErrore)
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT


    End Function


    '##############################################################################################################
    Public Function PianoColturale_Excel_FornitoriSementi(ByVal Query1_TempTableCreazione As String,
                                                          ByVal Query2_TempTableIndice As String,
                                                          ByVal Query3_TempTableFill As String,
                                                          ByRef objParametri As AgronicaCoreParametri
                                                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.AnagraficaAziendale.PianoColturale_Excel_FornitoriSementi"

        Dim MessaggioErrore As String = ""
        Dim StbQ As New Text.StringBuilder
        Dim DT As DataTable
        Dim Query4_TempTableJoin As String

        Try


            StbQ.Length = 0

            '----------------------------------------------------------------------------------------------------------
            '------- ACQUISTO DA FORNITORI  --------
            '----------------------------------------------------------------------------------------------------------
            StbQ.Append(" SELECT DISTINCT agenda.Piva, Movimenti_Dettagli.Mat_Cod, Contatti.Rag_Soc + Contatti.Nome + ' ' + Contatti.Cognome AS Fornitore  " & vbCrLf)

            StbQ.Append(" FROM Agenda  " & vbCrLf)
            'no AND Agenda.Sa_Cod = Mov_Magazzino.Sa_Cod
            StbQ.Append(" INNER JOIN Movimenti ON Agenda.PIVA = Movimenti.PIVA  AND Agenda.Id_Agenda = Movimenti.Id_Agenda  " & vbCrLf)
            StbQ.Append(" INNER JOIN Movimenti_Dettagli ON Movimenti.PIVA = Movimenti_Dettagli.PIVA AND Movimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov  " & vbCrLf)
            StbQ.Append(" INNER JOIN Materie_Prime ON Materie_Prime.Elem_Cod = Movimenti_Dettagli.Elem_Cod AND Materie_Prime.Mat_Cod = Movimenti_Dettagli.Mat_Cod   " & vbCrLf)

            StbQ.Append(" INNER JOIN Movimenti MovContAcquisto ON Agenda.PIVA = MovContAcquisto.PIVA  AND Agenda.Id_Agenda = MovContAcquisto.Id_Agenda  " & vbCrLf)
            StbQ.Append(" INNER JOIN Risorse_Umane ON MovContAcquisto.Cod_RisUm = Risorse_Umane.Cod_RisUm  " & vbCrLf)
            StbQ.Append(" INNER JOIN Contatti ON Risorse_Umane.piva = Contatti.piva AND Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto  " & vbCrLf)

            '/**************************************************
            '         NUOVO METODO CON TABELLA TEMPORANEA

            '           --> AGGIUNGO QUESTA PARTE:

            StbQ.Append(" INNER JOIN  #tempimprese " & vbCrLf)
            StbQ.Append(" ON #tempimprese.Piva = Agenda.Piva  " & vbCrLf)
            '/**************************************************

            StbQ.Append(" WHERE Movimenti.cau_mov = '" & Agro_SQL_SaveText(CAU_CARICO) & "' " & vbCrLf)
            StbQ.Append(" AND	MovContAcquisto.cau_mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "' " & vbCrLf)
            StbQ.Append(" AND  Agenda.Lav_Cod IN ( " &
                               CStr(LAVCOD_BOLLA_RICEVUTA) & "," &
                               CStr(LAVCOD_FATTURA_RICEVUTA) & "" &
                                ")" & vbCrLf)
            StbQ.Append(" AND Movimenti_Dettagli.Elem_Cod =  " & Agro_SQL_SaveNum(CStr(SEMENTI)) & vbCrLf)

            '/**************************************************
            Query4_TempTableJoin = StbQ.ToString

            StbQ = Nothing
            '/**************************************************

            '         NUOVO METODO CON TABELLA TEMPORANEA  
            Dim obj_MultiQuery As New AgronicaCoreDataProvider.AccessoMultiQuery
            obj_MultiQuery.SettaParametriPrecedenti(Me.DammiParametriCollezionati)
            DT = obj_MultiQuery.MLT_SelectFiltrataConTabellaTemporanea_2013(Query1_TempTableCreazione,
                                                                        Query2_TempTableIndice,
                                                                        Query3_TempTableFill,
                                                                        Query4_TempTableJoin,
                                                                        objParametri.StringaConnessione,
                                                                        MessaggioErrore)

            If MessaggioErrore <> "" Then
                Throw New Exception(MessaggioErrore)
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT


    End Function




    '##############################################################################################################
    Public Function PianoColturale_Excel_ImpiantiSenzaOperazioni_NON_USATO(ByVal Query1_TempTableCreazione As String,
                                                                ByVal Query2_TempTableIndice As String,
                                                                ByVal Query3_TempTableFill As String,
                                                                ByRef objParametri As AgronicaCoreParametri
                                                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.AnagraficaAziendale.PianoColturale_Excel_ImpiantiSenzaOperazioni"

        Dim MessaggioErrore As String = ""
        Dim StbQ As New Text.StringBuilder
        Dim DT As DataTable
        Dim Query4_TempTableJoin As String

        Try

            StbQ.Length = 0






            '/**************************************************
            Query4_TempTableJoin = StbQ.ToString

            StbQ = Nothing
            '/**************************************************

            '         NUOVO METODO CON TABELLA TEMPORANEA  
            Dim obj_MultiQuery As New AgronicaCoreDataProvider.AccessoMultiQuery
            obj_MultiQuery.SettaParametriPrecedenti(Me.DammiParametriCollezionati)
            DT = obj_MultiQuery.MLT_SelectFiltrataConTabellaTemporanea_2013(Query1_TempTableCreazione,
                                                                        Query2_TempTableIndice,
                                                                        Query3_TempTableFill,
                                                                        Query4_TempTableJoin,
                                                                        objParametri.StringaConnessione,
                                                                        MessaggioErrore)

            If MessaggioErrore <> "" Then
                Throw New Exception(MessaggioErrore)
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT


    End Function

    '##############################################################################################################
    Public Function PianoColturale_Excel_Campi(ByVal Query1_TempTableCreazione As String,
                                               ByVal Query2_TempTableIndice As String,
                                               ByVal Query3_TempTableFill As String,
                                               ByVal Flag_1_ConnGias_2_ConnAltro As String,
                                               ByRef objParametri As AgronicaCoreParametri
                                               ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.AnagraficaAziendale.PianoColturale_Excel_Campi"

        Dim MessaggioErrore As String = ""
        Dim StbQ As New Text.StringBuilder
        Dim DT As DataTable
        Dim Query4_TempTableJoin As String

        Try

            StbQ.Length = 0

            StbQ.Append(" SELECT Campi.Piva, Campi.Sa_Cod, Campi.Campo_Cod, Campi.Campo_Des " & vbCrLf)
            StbQ.Append(" FROM Campi " & vbCrLf)
            '/**************************************************
            '         NUOVO METODO CON TABELLA TEMPORANEA

            '           --> AGGIUNGO QUESTA PARTE:

            StbQ.Append(" INNER JOIN  #tempcentri " & vbCrLf)
            StbQ.Append(" ON #tempcentri.Piva = Campi.Piva AND Campi.Sa_Cod = #tempcentri.Sa_Cod  " & vbCrLf)
            '/**************************************************

            '/**************************************************
            Query4_TempTableJoin = StbQ.ToString

            StbQ = Nothing
            '/**************************************************

            '         NUOVO METODO CON TABELLA TEMPORANEA  
            Dim obj_MultiQuery As New AgronicaCoreDataProvider.AccessoMultiQuery
            obj_MultiQuery.SettaParametriPrecedenti(Me.DammiParametriCollezionati)
            DT = obj_MultiQuery.MLT_SelectFiltrataConTabellaTemporanea_2013(Query1_TempTableCreazione,
                                                                        Query2_TempTableIndice,
                                                                        Query3_TempTableFill,
                                                                        Query4_TempTableJoin,
                                                                        objParametri.StringaConnessione,
                                                                        MessaggioErrore)

            If MessaggioErrore <> "" Then
                Throw New Exception(MessaggioErrore)
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT


    End Function




    '##############################################################################################################
    Public Function PianoColturale_Excel_Particelle(ByVal Query1_TempTableCreazione As String,
                                                    ByVal Query2_TempTableIndice As String,
                                                    ByVal Query3_TempTableFill As String,
                                                    ByRef objParametri As AgronicaCoreParametri
                                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.AnagraficaAziendale.PianoColturale_Excel_Particelle"

        Dim MessaggioErrore As String = ""
        Dim StbQ As New Text.StringBuilder
        Dim DT As DataTable
        Dim Query4_TempTableJoin As String

        Try

            StbQ.Length = 0

            StbQ.Append(" SELECT AppezzamentiXParticelle.*, ISTAT.LOCALITA, ISTAT.COMUNI_PROV " & vbCrLf)
            StbQ.Append(" FROM AppezzamentiXParticelle " & vbCrLf)
            StbQ.Append(" INNER JOIN  ISTAT " & vbCrLf)
            StbQ.Append(" ON  AppezzamentiXParticelle.PROV = ISTAT.PROV AND AppezzamentiXParticelle.COM = ISTAT.COM " & vbCrLf)

            '/**************************************************
            '         NUOVO METODO CON TABELLA TEMPORANEA

            '           --> AGGIUNGO QUESTA PARTE:

            StbQ.Append(" INNER JOIN  #tempappezza " & vbCrLf)
            StbQ.Append(" ON #tempappezza.Piva = AppezzamentiXParticelle.Piva AND AppezzamentiXParticelle.Sa_Cod = #tempappezza.Sa_Cod AND  AppezzamentiXParticelle.Appezza = #tempappezza.Appezza  " & vbCrLf)
            '/**************************************************

            StbQ.Append(" ORDER BY PIVA, SA_COD, APPEZZA, COMUNI_PROV, LOCALITA, Sezione, Foglio, Numero, Subalterno " & vbCrLf)

            '/**************************************************
            Query4_TempTableJoin = StbQ.ToString

            StbQ = Nothing
            '/**************************************************

            '         NUOVO METODO CON TABELLA TEMPORANEA  
            Dim obj_MultiQuery As New AgronicaCoreDataProvider.AccessoMultiQuery
            obj_MultiQuery.SettaParametriPrecedenti(Me.DammiParametriCollezionati)
            DT = obj_MultiQuery.MLT_SelectFiltrataConTabellaTemporanea_2013(Query1_TempTableCreazione,
                                                                        Query2_TempTableIndice,
                                                                        Query3_TempTableFill,
                                                                        Query4_TempTableJoin,
                                                                        objParametri.StringaConnessione,
                                                                        MessaggioErrore)

            If MessaggioErrore <> "" Then
                Throw New Exception(MessaggioErrore)
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT


    End Function

    '##############################################################################################################
    Public Function PianoColturale_Excel_CampiParticelle(ByVal Query1_TempTableCreazione As String,
                                                         ByVal Query2_TempTableIndice As String,
                                                         ByVal Query3_TempTableFill As String,
                                                         ByRef objParametri As AgronicaCoreParametri
                                                         ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.AnagraficaAziendale.PianoColturale_Excel_CampiParticelle"

        Dim MessaggioErrore As String = ""
        Dim StbQ As New Text.StringBuilder
        Dim DT As DataTable
        Dim Query4_TempTableJoin As String

        Try

            StbQ.Length = 0

            StbQ.Append(" SELECT CampiXParticelle.*, APP.APPEZZA,  ISTAT.LOCALITA, ISTAT.COMUNI_PROV " & vbCrLf)
            StbQ.Append(" FROM CampiXParticelle " & vbCrLf)
            StbQ.Append(" INNER JOIN  ISTAT " & vbCrLf)
            StbQ.Append(" ON  CampiXParticelle.PROV = ISTAT.PROV AND CampiXParticelle.COM = ISTAT.COM " & vbCrLf)

            StbQ.Append(" INNER JOIN APPEZZAMENTO APP " & vbCrLf)
            StbQ.Append(" ON CampiXParticelle.piva = app.piva " & vbCrLf)
            StbQ.Append(" and CampiXParticelle.sa_cod = app.sa_cod " & vbCrLf)
            StbQ.Append(" and CampiXParticelle.Campo_Cod = app.campo_cod " & vbCrLf)


            '/**************************************************
            '         NUOVO METODO CON TABELLA TEMPORANEA

            '           --> AGGIUNGO QUESTA PARTE:

            StbQ.Append(" INNER JOIN  #tempappezza " & vbCrLf)
            StbQ.Append(" ON #tempappezza.Piva = app.Piva AND app.Sa_Cod = #tempappezza.Sa_Cod AND  app.Appezza = #tempappezza.Appezza  " & vbCrLf)
            '/**************************************************

            StbQ.Append(" ORDER BY PIVA, SA_COD, APPEZZA,CAMPO_COD, COMUNI_PROV, LOCALITA, Sezione, Foglio, Numero, Subalterno  " & vbCrLf)

            '/**************************************************
            Query4_TempTableJoin = StbQ.ToString

            StbQ = Nothing
            '/**************************************************

            '         NUOVO METODO CON TABELLA TEMPORANEA  
            Dim obj_MultiQuery As New AgronicaCoreDataProvider.AccessoMultiQuery
            obj_MultiQuery.SettaParametriPrecedenti(Me.DammiParametriCollezionati)
            DT = obj_MultiQuery.MLT_SelectFiltrataConTabellaTemporanea_2013(Query1_TempTableCreazione, _
                                                                        Query2_TempTableIndice, _
                                                                        Query3_TempTableFill, _
                                                                        Query4_TempTableJoin, _
                                                                        objParametri.StringaConnessione, _
                                                                        MessaggioErrore)

            If MessaggioErrore <> "" Then
                Throw New Exception(MessaggioErrore)
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT


    End Function

    Private Sub Aggiungi_Join_OI_Pomodoro(ByRef StbQ As Text.StringBuilder, ByVal Mostra_varieta_OI_Pomodoro As Boolean)

        If Mostra_varieta_OI_Pomodoro Then
            StbQ.Append("LEFT OUTER JOIN Codifica_Varieta_OIPomodorodaIndustriaNordItalia  ON Codifica_Varieta_OIPomodorodaIndustriaNordItalia.Cul_Cod_Gias = Cultivar.Cul_Cod  " & vbCrLf)
        End If

    End Sub

End Class
