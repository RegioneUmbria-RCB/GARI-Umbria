Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports System.Text

Public Class BIO_ReportBio : Inherits AgronicaCoreDataProvider.DataProvider

    Private descrizioniCategoriePersonalizzate As Dictionary(Of String, String) = Nothing

    Public Function Report_Carichi_Scarichi(ByVal piva As String,
                                             ByVal tipo_Report As Integer,
                                             ByVal dataMovDal As DateTime, ByVal dataMovAl As DateTime,
                                             ByVal categorieProdotti As List(Of Integer),
                                             ByVal causaliMovimento As List(Of String),
                                             ByVal tipoAppezzamento As List(Of Integer),
                                             ByVal origineDatiScarichi As Integer,
                                             ByVal sa_cod As Integer,
                                             ByVal xOrderBy As String,
                                             ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "BIO_ReportBio.Report_Carichi_Scarichi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Dim elencoCausaliMagazzinoTutte As String = String.Format("'{0}','{1}','{2}','{3}', '{4}', '{5}', '{6}', '{7}'", CAU_CARICO, CAU_SCARICO, CAU_TRATTAMENTO, CAU_LAVORAZIONE, CAU_ANIMALE, CAU_ALIMENTAZIONE, CAU_CARICO_CONSISTENZE, CAU_SCARICO_CONSISTENZE)
            Dim elencoCausaliMagazzinoTrattamentiLavorazioni As String = String.Format("'{0}','{1}'", CAU_TRATTAMENTO, CAU_LAVORAZIONE)
            Dim elencoCausaliAnimali As String = String.Format("'{0}','{1}','{2}','{3}'", CAU_ANIMALE, CAU_ALIMENTAZIONE, CAU_CARICO_CONSISTENZE, CAU_SCARICO_CONSISTENZE)
            Dim lavCodAmmessi = New List(Of Integer) From
                {
                    LAVCOD_DISTRIBUZIONE_CONCIME, LAVCOD_SARCHIATURA_CONCIMAZIONE, LAVCOD_DISTRIBUZIONE_AMMENDANTI, LAVCOD_CONCIMAZIONE_FOGLIARE, LAVCOD_FERTIRRIGAZIONE, LAVCOD_TRATTAMENTO_ANTIBUTTERATURA,
                    LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_DISERBO, LAVCOD_DISSECCAMENTO, LAVCOD_GEODISINFESTAZIONE, LAVCOD_CONCIA_SEME, LAVCOD_TRATTAMENTO_FITOREGOLATORE, LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE
                }

            For i As Integer = 0 To causaliMovimento.Count - 1
                causaliMovimento(i) = "'" & Agro_SQL_SaveText(causaliMovimento(i)) & "'"
            Next

            StrSQL.AppendLine(" select  distinct piva into #pive from Imprese ")
            StrSQL.AppendLine("")

            ' Temp table #Movimenti_Magazzino
            StrSQL.AppendLine(" Select m.piva, m.id_agenda, m.cod_risum, ")
            StrSQL.AppendLine(" m.id_mov, m.cau_mov, m.data_movimento, m.ora, ")
            StrSQL.AppendLine(" MD.Id_Mov_Det, MD.Elem_Cod, md.Pro_Cod, md.Mat_Cod, md.principiattivi as principi_attivi , md.lotto, md.qta, md.cod_progetto, md.sa_cod, ")
            StrSQL.AppendLine(" mdt.n, mdt.Cu, mdt.p, mdt.Efficienza, rif_esterno, rif_esterno_2 ")
            StrSQL.AppendLine(" into #Movimenti_Magazzino ")
            StrSQL.AppendLine(" from movimenti m (nolock) ")
            StrSQL.AppendLine(" inner Join Movimenti_dettagli md (nolock) on md.id_mov = m.id_mov ")
            StrSQL.AppendLine(" inner Join #PIVE p on md.PIVA = p.piva ")
            StrSQL.AppendLine(" Left Join mov_dettaglio_tecnico mdt (nolock) on md.id_mov_det = mdt.id_mov_det ")
            StrSQL.AppendLine(" where m.cau_mov IN (" & elencoCausaliMagazzinoTutte & ") ")
            StrSQL.AppendLine(" And p.piva = '" & Agro_SQL_SaveText(piva) & "' ")
            If categorieProdotti.Count > 0 Then
                StrSQL.AppendLine(" And md.elem_cod  in (" & Agro_SQL_Save_Clausola_IN(String.Join(",", categorieProdotti)) & ") ")
            End If
            If dataMovDal <> AGRODATAINIZIO Then
                StrSQL.AppendLine(" AND Data_Movimento >= " & Agro_SQL_SaveDate(dataMovDal.Date) & "")
            End If
            If dataMovAl <> AGRODATAFINE Then
                'Aggiungo un giorno alla data inserita e controllo le date precedenti per recuperare le movimentazioni anche dell'ultimo giorno richiesto,
                'per quelle che sono registrate anche con l'ora. In questo modo dato che Agro_SQL_SaveDate aggiunge un CONVERT(datetime) al parametro recupero
                'anche movimentazioni registrate come:
                '2025-05-08 15:30
                'altrimenti verrebbero recuperate solo quelle in cui l'orario non è specificato, ergo registrate a mezzanotte:
                '2025-05-08 00:00
                StrSQL.AppendLine(" AND Data_Movimento < " & Agro_SQL_SaveDate(dataMovAl.Date.AddDays(1)) & "")
            End If
            ''StrSQL.AppendLine("  AND M.id_agenda = 138889 ")
            ''StrSQL.AppendLine("  AND M.id_agenda IN (160496, 160495) ")

            ' Temp Table #Movimenti_CaricoScarico
            StrSQL.AppendLine("")
            StrSQL.AppendLine(" Select piva, id_agenda, cod_risum,  id_mov, cau_mov, data_movimento, ora, Id_Mov_Det, elem_cod, Pro_Cod, Mat_Cod, principi_attivi, lotto, qta, cod_progetto, sa_cod, n, cu, p, rif_esterno, rif_esterno_2 ")
            StrSQL.AppendLine(" into #Movimenti_CaricoScarico ")
            StrSQL.AppendLine(" From #Movimenti_Magazzino ")
            StrSQL.AppendLine(" Where cau_mov in (" & Agro_SQL_Save_Clausola_IN(String.Join(",", causaliMovimento), True) & " )")
            StrSQL.AppendLine(" CREATE NONCLUSTERED INDEX idx_mcs1 On #Movimenti_CaricoScarico (Piva, Id_Agenda, sa_cod); ")
            StrSQL.AppendLine(" CREATE NONCLUSTERED INDEX idx_mcs2 On #Movimenti_CaricoScarico (Mat_cod, Pro_Cod, Elem_Cod, Lotto); ")

            ' Tempa Table  #Movimenti_Trattamenti
            StrSQL.AppendLine("")
            StrSQL.AppendLine(" Select piva, id_agenda, cod_risum, id_mov, cau_mov, data_movimento, ora, Id_Mov_Det, elem_cod, Pro_Cod, Mat_Cod, principi_attivi, lotto, qta, cod_progetto, sa_cod, n, cu, p, Efficienza ")
            StrSQL.AppendLine(" into #Movimenti_Trattamenti ")
            StrSQL.AppendLine(" From #Movimenti_Magazzino ")
            StrSQL.AppendLine(" Where cau_mov In (" & elencoCausaliMagazzinoTrattamentiLavorazioni & ") ")

            ' Temp Table #Movimenti_Carico
            StrSQL.AppendLine("")
            StrSQL.AppendLine(" Select m.Cod_RisUm, m.Elem_Cod, m.Pro_Cod, m.Mat_Cod, m.Lotto ")
            StrSQL.AppendLine(" into #Movimenti_Carico ")
            StrSQL.AppendLine(" from #Movimenti_CaricoScarico m ")
            StrSQL.AppendLine(" where m.cau_mov = '" & CAU_CARICO & "' ")

            ' Temp Table #Movimenti_Animali
            StrSQL.AppendLine("")
            StrSQL.AppendLine(" Select piva, id_agenda, cod_risum, id_mov, cau_mov, data_movimento, ora, Id_Mov_Det, elem_cod, Pro_Cod, Mat_Cod, principi_attivi, lotto, qta, cod_progetto, sa_cod, n, cu, p, Efficienza ")
            StrSQL.AppendLine(" into #Movimenti_Animali ")
            StrSQL.AppendLine(" From #Movimenti_Magazzino ")
            StrSQL.AppendLine(" Where cau_mov In (" & elencoCausaliAnimali & ")")

            ' Costi
            StrSQL.AppendLine("")
            StrSQL.AppendLine(" ;With ")
            StrSQL.AppendLine(" AgendeDistinct  as ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine(" Select distinct id_agenda from #Movimenti_CaricoScarico ")
            StrSQL.AppendLine(" ), ")
            StrSQL.AppendLine(" Riferimenti(Id_agenda, Id_Agenda_rif) ")
            StrSQL.AppendLine(" as ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine(" Select distinct mrif.Id_Agenda, mrif.Id_Agenda_rif from AgendeDistinct mcs ")
            StrSQL.AppendLine(" inner Join Mov_Dettagli_Riferimenti mrif (nolock) on mcs.id_agenda = mrif.Id_Agenda ")
            StrSQL.AppendLine("), ")
            StrSQL.AppendLine(" Costi_Campagna(tipo, piva, Id_agenda, Id_agenda_Rif, Mat_Cod, Pro_Cod, Elem_Cod, lotto, sa_cod, Appezza, Id_Reg, Id_Imputazione, Progetto_Cod, QtaTotale, Valore, Id_CDG, imputazione, data_movimento) ")
            StrSQL.AppendLine(" As ")
            StrSQL.AppendLine(" (  ")
            StrSQL.AppendLine(" Select 'campagna' as tipo, t.Piva, rif.Id_Agenda, rif.Id_Agenda_Rif, t.Mat_Cod, t.Pro_Cod, t.Elem_Cod, t.lotto, t.sa_cod, d.Appezza, d.Id_Reg, d.Id_Imputazione, d.Progetto_Cod,t.qta, d.Valore, t.Id_CDG, t.Tab_Imputazione, NULL as data_movimento ")
            StrSQL.AppendLine(" From Riferimenti rif ")
            StrSQL.AppendLine(" inner Join cdg_testata t (nolock) On t.Id_Agenda =rif.Id_agenda_rif And t.Budget = 0")
            StrSQL.AppendLine("inner Join CDG_Dettagli d (nolock) On t.Id_CDG = d.Id_CDG ")
            StrSQL.AppendLine(" where t.elem_cod <> 0 And (t.pro_cod <> 0 Or t.mat_cod <> 0) And t.elem_cod <> 700 And (d.Cod_Animale = 0 And d.Cod_Animale_Distinta = 0) ")
            StrSQL.AppendLine(" ), ")
            StrSQL.AppendLine(" Costi_Non_Campagna(tipo, piva, Id_agenda, Id_agenda_Rif, Mat_Cod, Pro_Cod, Elem_Cod, lotto, sa_cod, Appezza, Id_Reg, Id_Imputazione, Progetto_Cod, QtaTotale, Valore, Id_CDG, imputazione, data_movimento) ")
            StrSQL.AppendLine("As ")
            StrSQL.AppendLine(" (  ")
            StrSQL.AppendLine(" Select 'non campagna' as tipo, t.Piva, t.Id_Agenda, 0 As Id_agenda_Rif, t.Mat_Cod, t.Pro_Cod, t.Elem_Cod, t.lotto, t.sa_cod, d.Appezza, d.Id_Reg, d.Id_Imputazione, d.Progetto_Cod,t.qta, d.Valore, t.Id_CDG, t.Tab_Imputazione, NULL as data_movimento ")
            StrSQL.AppendLine(" From #Movimenti_CaricoScarico mcs ")
            StrSQL.AppendLine(" inner Join cdg_testata t (nolock) On t.Id_Agenda = mcs.id_agenda And t.Budget = 0")
            StrSQL.AppendLine(" inner Join CDG_Dettagli d (nolock) On t.Id_CDG = d.Id_CDG  ")
            StrSQL.AppendLine(" where t.elem_cod <> 0 And (t.pro_cod <> 0 Or t.mat_cod <> 0) And t.elem_cod <> 700 And (d.Cod_Animale = 0 And d.Cod_Animale_Distinta = 0) ")
            StrSQL.AppendLine(" ), ")
            StrSQL.AppendLine(" Costi_Animali(tipo, piva, Id_agenda, Id_agenda_Rif, Mat_Cod, Pro_Cod, Elem_Cod, lotto, sa_cod, Appezza, Id_Reg, Id_Imputazione, Progetto_Cod, QtaTotale, Valore, Id_CDG, imputazione, data_movimento) ")
            StrSQL.AppendLine("As ")
            StrSQL.AppendLine(" (  ")
            StrSQL.AppendLine(" Select 'animali' as tipo, t.Piva, rif.Id_Agenda, rif.Id_Agenda_Rif, t.Mat_Cod, t.Pro_Cod, t.Elem_Cod, t.lotto, t.sa_cod, d.Appezza, d.Cod_Animale, d.Id_Imputazione, d.Progetto_Cod,t.qta, d.Valore, t.Id_CDG , t.Tab_Imputazione, m.Data_Movimento as data_movimento ")
            StrSQL.AppendLine(" From Riferimenti rif  ")
            StrSQL.AppendLine(" inner Join cdg_testata t (nolock) On t.Id_Agenda =rif.Id_agenda_rif And t.Budget = 0")
            StrSQL.AppendLine(" inner Join CDG_Dettagli d (nolock) On t.Id_CDG = d.Id_CDG  ")
            StrSQL.AppendLine(" inner join Movimenti m on m.Id_Agenda = rif.Id_Agenda_rif and m.Cau_Mov = 7350")
            StrSQL.AppendLine(" where t.elem_cod <> 0 And (t.pro_cod <> 0 Or t.mat_cod <> 0) And t.elem_cod <> 700 And d.Cod_Animale <> 0 And d.Cod_Animale_Distinta <> 0 ")
            StrSQL.AppendLine(" ),  ")
            StrSQL.AppendLine(" Costi_Tutti(tipo, piva, Id_agenda, Id_agenda_Rif, Mat_Cod, Pro_Cod, Elem_Cod, lotto, sa_cod, Appezza, Id_Reg, Id_Imputazione, Progetto_Cod, QtaTotale, Valore, Id_CDG, imputazione, data_movimento) ")
            StrSQL.AppendLine(" As ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine(" Select tipo, piva, Id_agenda, Id_agenda_Rif, Mat_Cod, Pro_Cod, Elem_Cod, lotto, sa_cod, Appezza, Id_Reg, Id_Imputazione, Progetto_Cod, QtaTotale, Valore, Id_CDG, imputazione, data_movimento from Costi_Campagna ")
            StrSQL.AppendLine(" union ")
            StrSQL.AppendLine(" Select tipo, piva, Id_agenda, Id_agenda_Rif, Mat_Cod, Pro_Cod, Elem_Cod, lotto, sa_cod, Appezza, Id_Reg, Id_Imputazione, Progetto_Cod, QtaTotale, Valore, Id_CDG, imputazione, data_movimento from Costi_Non_Campagna ")
            StrSQL.AppendLine(" union ")
            StrSQL.AppendLine(" Select tipo, piva, Id_agenda, Id_agenda_Rif, Mat_Cod, Pro_Cod, Elem_Cod, lotto, sa_cod, Appezza, Id_Reg, Id_Imputazione, Progetto_Cod, QtaTotale, Valore, Id_CDG, imputazione, data_movimento from Costi_Animali ")
            StrSQL.AppendLine(" ) ")
            StrSQL.AppendLine(" Select * into #Costi_Tutti from Costi_Tutti ct ")
            Select Case origineDatiScarichi
                Case 1  'qdc
                    StrSQL.AppendLine(" where ct.imputazione = 'EREDITA' ")
                Case 2  'costi
                    StrSQL.AppendLine(" where ct.imputazione <> 'EREDITA' ")
            End Select

            StrSQL.AppendLine("")
            StrSQL.AppendLine(" Select * into #costi_animali from #Costi_Tutti ct where ct.tipo = 'animali' ")
            Select Case origineDatiScarichi
                Case 1  'qdc
                    StrSQL.AppendLine(" AND ct.imputazione = 'EREDITA' ")
                Case 2  'costi
                    StrSQL.AppendLine(" AND ct.imputazione <> 'EREDITA' ")
            End Select

            ' Imprese Progetti
            StrSQL.AppendLine("")
            StrSQL.AppendLine(";With  ")
            StrSQL.AppendLine(" Imprese_Progetti_Campagna(id_agenda, piva, sa_cod, cod_progetto, appezza, id_reg, progetto_nome) ")
            StrSQL.AppendLine(" As ")
            StrSQL.AppendLine("( ")
            StrSQL.AppendLine(" Select Distinct mcs.id_agenda , mcs.piva, mcs.sa_cod, mcs.cod_progetto, ip.appezza, ip.id_reg, ip.progetto_nome from #Movimenti_CaricoScarico mcs ")
            StrSQL.AppendLine(" inner Join imprese_progetti ip (nolock) On mcs.piva = ip.piva And mcs.cod_progetto = ip.Progetto_Cod ")
            StrSQL.AppendLine(" )   , ")
            StrSQL.AppendLine(" ImpreseProgetti_Non_Campagna(id_agenda, piva, sa_cod, cod_progetto, appezza, id_reg, progetto_nome) ")
            StrSQL.AppendLine(" As ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine(" Select Distinct c.Id_agenda, c.Piva, c.sa_cod, c.Progetto_Cod, ip.appezza, ip.id_reg , ip.progetto_nome from #costi_tutti c ")
            StrSQL.AppendLine(" inner Join imprese_progetti ip (nolock) On c.piva = ip.piva And ip.Progetto_Cod = c.Progetto_Cod And ip.Piva = c.Piva And ip.Id_Reg = c.Id_Reg And ip.Appezza = c.Appezza And ip.Sa_Cod = c.sa_cod And c.tipo <> 'animali' ")
            StrSQL.AppendLine(" ), ")

            StrSQL.AppendLine(" Progetti_Animali(id_agenda, piva, sa_cod, cod_progetto, appezza, id_reg, progetto_nome)  As ")
            StrSQL.AppendLine(" (  ")
            StrSQL.AppendLine(" Select distinct c.id_agenda, c.Piva, c.sa_cod,  za.Cod_Progetto, ")
            StrSQL.AppendLine(" c.appezza, c.Id_Reg, zad.codice_distinta as progetto_nome ")
            StrSQL.AppendLine(" From #costi_animali c  ")
            StrSQL.AppendLine(" Left Join Zoo_Animali za on c.id_reg = za.Cod_Progetto And c.piva = za.PIVA ")
            StrSQL.AppendLine(" inner Join Zoo_Animali_Distinte zad on zad.Cod_Animale = c.Id_Reg And c.PIVA = zad.Piva ")
            StrSQL.AppendLine(" where c.data_movimento >= zad.Validita_Inizio and c.data_movimento <= zad.Validita_Fine ")
            'StrSQL.AppendLine(" where zad.distinta_chiusa = 0 ")
            StrSQL.AppendLine(" ), ")


            StrSQL.AppendLine(" Imprese_Progetti_Tutti(id_agenda, piva, sa_cod, cod_progetto, appezza, id_reg, progetto_nome, tipo)")
            StrSQL.AppendLine(" As ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine(" Select id_agenda, piva, sa_cod, cod_progetto, appezza, id_reg ,progetto_nome, 'campagna' as tipo from Imprese_Progetti_Campagna ")
            StrSQL.AppendLine(" UNION ")
            StrSQL.AppendLine(" Select id_agenda, piva, sa_cod, cod_progetto, appezza, id_reg ,progetto_nome, 'non campagna' as tipo from ImpreseProgetti_Non_Campagna ")
            StrSQL.AppendLine(" union ")
            StrSQL.AppendLine(" Select id_agenda, piva, sa_cod, cod_progetto, appezza, id_reg ,progetto_nome , 'animali' as tipo from Progetti_Animali ")
            StrSQL.AppendLine(" ) ")
            StrSQL.AppendLine(" Select * into #Imprese_Progetti_Tutti from Imprese_Progetti_Tutti ")
            StrSQL.AppendLine("")
            StrSQL.AppendLine(" Select * into #Progetti_Animali from #Imprese_Progetti_Tutti ipt where ipt.tipo = 'animali' ")
            StrSQL.AppendLine("")
            StrSQL.AppendLine(" Select distinct id_agenda, piva, sa_cod, cod_progetto, appezza, id_reg, progetto_nome into #Imprese_Progetti_Distinct From #Imprese_Progetti_Tutti ")

            ' Progetti + costi (assieme)
            StrSQL.AppendLine("")
            StrSQL.AppendLine("; with ")
            StrSQL.AppendLine(" Progetti_Presenti_Nei_Costi as ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine(" Select ip.piva, ip.id_agenda, ip.sa_cod, ip.progetto_nome, ip.cod_progetto, ip.appezza, ip.id_reg, ")
            StrSQL.AppendLine(" c.Elem_Cod, c.Mat_Cod, c.Pro_Cod, c.lotto, c.Id_agenda_Rif, c.Id_CDG, c.Id_Imputazione, c.imputazione, c.QtaTotale, c.Valore ")
            StrSQL.AppendLine(" from #Imprese_Progetti_Distinct ip ")
            StrSQL.AppendLine(" inner Join #Costi_Tutti c ")
            StrSQL.AppendLine(" On c.Id_agenda = ip.id_agenda And c.piva = ip.piva And c.sa_cod = ip.sa_cod And c.Appezza = ip.appezza And c.Id_Reg = ip.id_reg AND c.Progetto_Cod = ip.cod_progetto AND c.tipo <> 'animali' ")
            StrSQL.AppendLine(" union")
            StrSQL.AppendLine(" Select ip.piva, ip.id_agenda, ip.sa_cod, ip.progetto_nome, ip.cod_progetto, ip.appezza, ip.id_reg, ")
            StrSQL.AppendLine(" c.Elem_Cod, c.Mat_Cod, c.Pro_Cod, c.lotto, c.Id_agenda_Rif, c.Id_CDG, c.Id_Imputazione, c.imputazione, c.QtaTotale, c.Valore ")
            StrSQL.AppendLine(" from #Imprese_Progetti_Distinct ip ")
            StrSQL.AppendLine(" inner Join #costi_animali c ")
            StrSQL.AppendLine(" On c.Id_agenda = ip.id_agenda And c.piva = ip.piva And c.sa_cod = ip.sa_cod And c.Appezza = ip.appezza And c.Id_Reg = ip.id_reg ")
            StrSQL.AppendLine(" ) , ")
            StrSQL.AppendLine(" Progetti_Non_Presenti_Nei_Costi as ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine(" Select ip.id_agenda, ip.sa_cod, ip.progetto_nome, ip.cod_progetto, ip.appezza, ip.id_reg, ip.piva ")
            StrSQL.AppendLine(" FROM #Imprese_Progetti_Distinct  ip ")
            StrSQL.AppendLine(" WHERE Not EXISTS ")
            StrSQL.AppendLine(" (SELECT * FROM #Costi_Tutti c WHERE ip.id_agenda = c.Id_agenda and ip.appezza = c.Appezza and ip.id_reg = c.Id_Reg and ip.sa_cod = c.sa_cod)  ")
            StrSQL.AppendLine(" ), ")
            StrSQL.AppendLine(" Progetti_Non_presenti_Nei_Costi_Completa as ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine(" Select p.piva, p.id_agenda, p.sa_cod, p.progetto_nome, p.cod_progetto, p.appezza, p.id_reg, ")
            StrSQL.AppendLine(" mcs.elem_cod, mcs.Mat_Cod, mcs.Pro_Cod, mcs.lotto, null As Id_agenda_Rif, null As Id_CDG, null As Id_Imputazione, ")
            StrSQL.AppendLine(" null as imputazione, null As QtaTotale, null as Valore ")
            StrSQL.AppendLine(" from #Movimenti_CaricoScarico  mcs ")
            StrSQL.AppendLine(" inner Join Progetti_Non_Presenti_Nei_Costi p ")
            StrSQL.AppendLine(" On p.id_agenda = mcs.id_agenda And p.sa_cod = mcs.sa_cod ")
            StrSQL.AppendLine(" ), ")
            StrSQL.AppendLine(" Progetti_Costi_Completa as ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine(" Select Distinct * From Progetti_Presenti_Nei_Costi ")
            StrSQL.AppendLine(" union ")
            StrSQL.AppendLine(" Select Distinct * From Progetti_Non_presenti_Nei_Costi_Completa ")
            StrSQL.AppendLine(" ) ")
            StrSQL.AppendLine(" select distinct piva, id_agenda, sa_cod, progetto_nome, cod_progetto, ")
            StrSQL.AppendLine(" appezza, id_reg, elem_cod, mat_cod, pro_cod, lotto, id_imputazione, ")
            StrSQL.AppendLine(" imputazione, qtatotale, valore ")
            StrSQL.AppendLine(" into #Progetti_Costi_Completa from Progetti_Costi_Completa ")

            StrSQL.AppendLine("")
            StrSQL.AppendLine("-- Query Principale")
            StrSQL.AppendLine(" ;With ")
            ' CTE Movimenti_Registrazioni
            StrSQL.AppendLine(" Movimenti_Registrazioni ")
            StrSQL.AppendLine(" as ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine(" Select * from Movimenti (nolock) ")
            StrSQL.AppendLine(" where piva = '" & Agro_SQL_SaveText(piva) & "' ")
            StrSQL.AppendLine(" And cau_mov = '4000' ")
            If dataMovDal <> AGRODATAINIZIO Then
                StrSQL.AppendLine(" AND Data_Movimento >= " & Agro_SQL_SaveDate(dataMovDal) & "")
            End If
            If dataMovAl <> AGRODATAFINE Then
                StrSQL.AppendLine(" AND Data_Movimento < " & Agro_SQL_SaveDate(dataMovAl.Date.AddDays(1)) & "")
            End If
            StrSQL.AppendLine(" ), ")
            StrSQL.AppendLine(" Progetti_Imputazioni  ")
            StrSQL.AppendLine(" As ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine(" Select * from Imputazioni (nolock) ")
            StrSQL.AppendLine(" ), ")
            ' CTE Fornitori
            StrSQL.AppendLine(" Fornitori ")
            StrSQL.AppendLine(" As ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine(" Select distinct mcs.elem_cod, mcs.pro_cod, mcs.mat_cod, mcs.lotto , ")
            StrSQL.AppendLine(" ru.settore_des, ru.cod_risum, ")
            StrSQL.AppendLine(" contatti.cod_contatto, contatti.cognome, contatti.nome, contatti.rag_soc ")
            StrSQL.AppendLine(" From risorse_umane ru (nolock) ")
            StrSQL.AppendLine(" inner Join contatti (nolock) On contatti.cod_contatto = ru.cod_contatto And (ru.Piva = Contatti.Piva  Or Contatti.Sa_Cod = -1)    ")
            StrSQL.AppendLine(" inner Join #Movimenti_carico mc On ru.cod_risum = mc.Cod_RisUm  ")
            StrSQL.AppendLine(" inner Join #Movimenti_CaricoScarico mcs On mc.elem_cod = mcs.elem_cod And mc.pro_cod = mcs.pro_cod And mc.Mat_Cod = mcs.mat_cod And mc.lotto = mcs.lotto ")
            StrSQL.AppendLine("where ru.Piva = '" & Agro_SQL_SaveText(piva) & "'")
            StrSQL.AppendLine(" ), ")
            StrSQL.AppendLine(" fornitori_top ")
            StrSQL.AppendLine(" as ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine(" Select * from ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine(" Select *, ROW_NUMBER () OVER (PARTITION BY elem_cod, pro_cod, mat_cod, lotto ORDER BY cod_risum DESC) as prog ")
            StrSQL.AppendLine(" From Fornitori) q ")
            StrSQL.AppendLine(" Where prog = 1 ")
            StrSQL.AppendLine(" ), ")

            ' CTE Classificazione_Formulati_CTE
            StrSQL.AppendLine("Classificazione_Formulati_CTE ")
            StrSQL.AppendLine("as ")
            StrSQL.AppendLine("(")
            StrSQL.AppendLine("Select Fr_Cod, fr_des, fc.Class_Cod, cf.Class_Des from Formulati f ")
            StrSQL.AppendLine("Left Join formulatixclassificazioni fc ")
            StrSQL.AppendLine("On f.Fr_Cod = fc.For_Cod ")
            StrSQL.AppendLine("Left Join ClassificazioniFormulati cf ")
            StrSQL.AppendLine("On cf.Class_Cod = fc.Class_Cod ")
            StrSQL.AppendLine(" ), ")

            ' CTE Dati_Animali
            StrSQL.AppendLine(" Dati_Animali as  ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine(" Select distinct ")
            StrSQL.AppendLine(" za.Cod_Progetto, Matricola, za.Gen_cod, za.Spe_Cod, za.IPRO_Cod, za.RAZ_COD, Nome, ")
            StrSQL.AppendLine(" gen.GEN_DES, spe.SPE_DES, raz.RAZ_DES, za.Progetto as Progetto_Animale ")
            StrSQL.AppendLine(" From zoo_animali za ")
            StrSQL.AppendLine(" inner Join #Progetti_Animali pa on za.Cod_Progetto = pa.id_reg ")
            StrSQL.AppendLine(" inner Join Lista_Generi_Animali gen on za.GEN_COD = gen.GEN_COD  ")
            StrSQL.AppendLine(" inner Join Lista_Specie_Animali spe on za.gen_cod = spe.gen_cod And  za.SPE_COD = spe.SPE_COD  ")
            StrSQL.AppendLine(" inner Join Lista_Razze_Animali raz on za.GEN_COD = raz.GEN_COD And za.SPE_COD = raz.SPE_COD And za.RAZ_COD = raz.RAZ_COD ")
            StrSQL.AppendLine(" ) ")

            'Inizio query principale
            StrSQL.AppendLine(" Select qp.*, mov_reg.doc_numero_sin + LTRIM(STR(mov_reg.doc_numero, 10, 0)) + mov_reg.doc_numero_des ddt_numero, mov_reg.data_movimento ddt_data, mov_reg.ora ddt_ora from ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine(" Select mcs.id_agenda,  mcs.sa_cod, mcs.elem_cod, mcs.pro_cod, mcs.mat_cod, mcs.id_mov, mcs.id_mov_det, mcs.Piva, mcs.Cau_Mov, mcs.cod_risum, ")
            StrSQL.AppendLine(" Case   When  mcs.cau_mov  = '7300' Then 'Carico' ")
            StrSQL.AppendLine(" When  mcs.cau_mov  = '7350' Then 'Scarico' ")
            StrSQL.AppendLine(" End As Tipo_Movimento, ")
            StrSQL.AppendLine(" Case   When  mcs.Elem_Cod  = 3 Then 'Fertilizzante' ")
            StrSQL.AppendLine(" When  mcs.Elem_Cod  = 10 Then 'Semente' ")
            StrSQL.AppendLine(" When  mcs.Elem_Cod  = 191 Then 'Fitofarmaci' ")
            StrSQL.AppendLine(" When  mcs.Elem_Cod  = 304 Then 'Materie Prime Animali' ")
            StrSQL.AppendLine(" WHEN  mcs.Elem_Cod  = 201 Then 'Semilavorati Vegetali' ")
            StrSQL.AppendLine(" Else isnull(cm.NomeComune, '') ")
            StrSQL.AppendLine(" End As Tipo_Prodotto, ")
            StrSQL.AppendLine(" Case  When ISNULL(mp.Cod_Articolo, '') <> '' Then mp.Cod_Articolo ")
            StrSQL.AppendLine(" WHEN ISNULL(mp.Cod_Articolo, '') = '' Then  ")
            StrSQL.AppendLine(" Case   ")
            StrSQL.AppendLine(" WHEN Not EXISTS(SELECT 1 FROM CAC_Codifica_ProdottiAziendali (nolock) WHERE CAC_Codifica_ProdottiAziendali.Elem_Cod = mcs.Elem_Cod)  ")
            StrSQL.AppendLine(" THEN ''  ")
            StrSQL.AppendLine(" Else ")
            StrSQL.AppendLine(" ISNULL((SELECT TOP(1) Cod_Prodotto_Cliente  ")
            StrSQL.AppendLine(" From CAC_Codifica_ProdottiAziendali (nolock) ")
            StrSQL.AppendLine(" Where CAC_Codifica_ProdottiAziendali.Elem_Cod = mcs.Elem_Cod ")
            StrSQL.AppendLine(" And Codice_GIAS = mcs.Pro_Cod  ")
            StrSQL.AppendLine(" And Piva = mcs.Piva) , '') ")
            StrSQL.AppendLine(" End ")
            StrSQL.AppendLine(" End as Articolo, ")
            StrSQL.AppendLine(" Case   When  mcs.Elem_Cod  = 3 Then ISNULL(fertilizzanti.fer_des, '')  ")
            StrSQL.AppendLine(" When  mcs.Elem_Cod  = 10 Then ISNULL(mp.Mat_Des, '') ")
            StrSQL.AppendLine(" When  mcs.Elem_Cod  = 191 Then ISNULL(formulati.fr_des, '') ")
            StrSQL.AppendLine(" When  mcs.Elem_Cod  = 304 Then ISNULL(mp.Mat_Des, '')  ")
            StrSQL.AppendLine(" WHEN  mcs.Elem_Cod  = 201 Then ISNULL(mp.Mat_Des, '')  ")
            StrSQL.AppendLine(" End As Descrizione,  ")
            StrSQL.AppendLine("Case when mcs.elem_cod = 191 then ")
            StrSQL.AppendLine(" coalesce(STUFF(( ")
            StrSQL.AppendLine(" Select ', ' + cf.Class_Des ")
            StrSQL.AppendLine(" From Classificazione_Formulati_CTE cf ")
            StrSQL.AppendLine("Where cf.fr_cod = mcs.pro_cod ")
            StrSQL.AppendLine(" For Xml PATH ('')),1,2,''), '')  ")
            StrSQL.AppendLine(" Else '' end as Classificazione, ")

            StrSQL.AppendLine(" ISNULL(c.cod_progetto, '') as Progetto_Cod, ")
            StrSQL.AppendLine(" ISNULL(c.progetto_nome, ISNULL(imp.CodiceSecondario, '')) as OP, ")

            StrSQL.AppendLine(" isnull(f.settore_des, '' ) as Fornitore, ")
            StrSQL.AppendLine(" RTrim(isnull(isnull(f.rag_soc, '')  + isnull(f.cognome, '') + ' ' + isnull(f.nome, ''),'')) as  Fornitore_Ragione_Sociale, ")
            StrSQL.AppendLine(" Fabbricato_Des,  ")
            StrSQL.AppendLine(" dest.id_destinazione, ")
            StrSQL.AppendLine(" Case When ISNULL(apc.val_Cod, '') = '' Then ''   ")
            StrSQL.AppendLine(" WHEN ISNULL(apc.val_Cod, '') = '1' Then 'Integrato'  ")
            StrSQL.AppendLine(" WHEN ISNULL(apc.val_Cod, '') = '2' Then 'In Conversione'  ")
            StrSQL.AppendLine(" WHEN ISNULL(apc.val_Cod, '') = '3' Then 'Biologico'   end as Bio_Convers,  ")
            StrSQL.AppendLine(" Isnull(apc.val_cod, 0) as Codice_Bio_Convers, ")
            StrSQL.AppendLine(" ISNULL(apc_BIO.val_Cod, '') as App_BIO, ")
            StrSQL.AppendLine(" mcs.Lotto, ")
            StrSQL.AppendLine(" mcs.data_movimento, ")
            StrSQL.AppendLine(" mcs.Ora, ")

            StrSQL.AppendLine(" Case when mcs.cau_mov = '7300' then isnull(c.QtaTotale,0) else c.QtaTotale * -1 end as QtaTotale_CDG, ")
            StrSQL.AppendLine(" Case when c.QtaTotale Is null then ")
            StrSQL.AppendLine(" Case when mcs.cau_mov = '7300' then mcs.Qta else mcs.Qta * -1 end  ")
            StrSQL.AppendLine(" Else ")
            StrSQL.AppendLine(" Case when mcs.cau_mov = '7300' then  round(  (isnull(c.QtaTotale, 0) * c.Valore) / 100,6)	else round( ((isnull(c.QtaTotale, 0) * c.Valore) / 100),6) * -1 end ")
            StrSQL.AppendLine(" End As Qta, ")
            StrSQL.AppendLine(" isnull(mcs.qta, 0) As qta_originale, ")
            'StrSQL.AppendLine(" ISNULL(ric.val_cod, ISNULL(imp.CodicePrincipale, '')) as Progetto, ")
            StrSQL.AppendLine(" Case when anim.Progetto_Animale Is null then ")
            StrSQL.AppendLine(" ISNULL(ric.val_cod, ISNULL(imp.CodicePrincipale, '')) ")
            StrSQL.AppendLine(" Else anim.Progetto_Animale End As Progetto, ")
            StrSQL.AppendLine(" anim.Progetto_Animale, ")
            StrSQL.AppendLine(" ISNULL(apc_cod.val_Cod, '') as Campo, ")
            StrSQL.AppendLine(" ISNULL(app_nome, '') as Campo_Descrizione, ")
            StrSQL.AppendLine(" ISNULL(ri.sup_imp, 0) As Sup,   ")
            StrSQL.AppendLine(" ISNULL(mt.principi_attivi,'') as tratta_Principi_attivi, ")

            StrSQL.AppendLine(" Case When  mcs.Elem_Cod  = 3 then ")
            StrSQL.AppendLine("     Case when mt.cu Is null then ")
            StrSQL.AppendLine("         isnull(FERTILIZZANTI.Cu, 0) ")
            StrSQL.AppendLine(" Else mt.cu End ")
            StrSQL.AppendLine(" Else ")
            StrSQL.AppendLine(" ISNULL(mt.cu, 0) End As Tratta_cu, ")
            StrSQL.AppendLine(" Case When  mcs.Elem_Cod  = 3 then ")
            StrSQL.AppendLine("     Case when mt.n Is null then ")
            StrSQL.AppendLine("         isnull(FERTILIZZANTI.n, 0) ")
            StrSQL.AppendLine(" Else mt.n End ")
            StrSQL.AppendLine(" Else ")
            StrSQL.AppendLine(" ISNULL(mt.n, 0) End As Tratta_Azoto, ")
            StrSQL.AppendLine(" Case When  mcs.Elem_Cod  = 3 then ")
            StrSQL.AppendLine("     Case when mt.p Is null then ")
            StrSQL.AppendLine("         isnull(FERTILIZZANTI.P2O5, 0) ")
            StrSQL.AppendLine(" Else mt.p End ")
            StrSQL.AppendLine(" Else ")
            StrSQL.AppendLine(" ISNULL(mt.p, 0) End As Tratta_Fosforo, ")


            StrSQL.AppendLine(" ISNULL(mt.Efficienza, 0) As tratta_Efficienza, ")
            StrSQL.AppendLine(" mcs.principi_attivi As carscar_Principi_attivi, ")
            StrSQL.AppendLine(" mcs.cu as carscar_CU, ")
            StrSQL.AppendLine(" mcs.n as carscar_Azoto, ")
            StrSQL.AppendLine(" mcs.p as carscar_Fosforo, ")

            StrSQL.AppendLine(" isnull(c.QtaTotale, 0) As QtaTotale,  ")
            StrSQL.AppendLine(" isnull(c.Valore, 0) As Valore, ")
            StrSQL.AppendLine(" coalesce(anim.Matricola, '') as Matricola, ")
            StrSQL.AppendLine(" coalesce(anim.GEN_COD, 0) as Gen_Cod, ")
            StrSQL.AppendLine(" coalesce(anim.gen_des, '') as Gen_Des, ")
            StrSQL.AppendLine(" coalesce(anim.spe_cod, 0) as Spe_Cod, ")
            StrSQL.AppendLine(" coalesce(anim.SPE_DES, '') as Spe_Des, ")
            StrSQL.AppendLine(" coalesce(anim.RAZ_COD, 0) as Raz_Cod, ")
            StrSQL.AppendLine(" coalesce(anim.RAZ_DES, '') as Raz_Des, ")
            StrSQL.AppendLine(" coalesce(anim.Nome, '') as Nome_Animale, ")
            StrSQL.AppendLine(" coalesce(mcs.rif_esterno, '') as rif_esterno, ")
            StrSQL.AppendLine(" coalesce(mcs.rif_esterno_2,'') as  rif_esterno_2 ")

            StrSQL.AppendLine(" From #Movimenti_CaricoScarico mcs  ")
            If origineDatiScarichi = 0 Then
                StrSQL.AppendLine("left Join ")
            Else
                StrSQL.AppendLine("inner Join ")
            End If

            StrSQL.AppendLine(" #Progetti_Costi_Completa c on mcs.id_agenda = c.Id_agenda and mcs.Mat_Cod = c.Mat_Cod and mcs.pro_cod = c.Pro_cod and mcs.elem_cod = c.elem_cod and mcs.lotto = c.lotto ")
            StrSQL.AppendLine(" Left Join #Movimenti_Trattamenti mt on mcs.id_agenda = mt.id_agenda And mcs.elem_cod = mt.elem_cod And mcs.pro_cod = mt.pro_cod And mcs.mat_cod = mt.mat_cod And mcs.lotto = mt.lotto  ")
            StrSQL.AppendLine(" Left Join fornitori_top f on mcs.elem_cod = f.elem_cod And mcs.Pro_Cod = f.Pro_Cod And mcs.Mat_Cod = f.Mat_Cod And mcs.lotto = f.lotto ")
            StrSQL.AppendLine(" Left Join materie_prime mp (nolock) On mp.elem_cod = mcs.elem_cod And mp.mat_cod = mcs.mat_cod  ")
            StrSQL.AppendLine(" Left Join fertilizzanti (nolock) On fertilizzanti.fer_cod = mcs.pro_cod  And mcs.elem_cod = 3   ")
            StrSQL.AppendLine(" Left Join formulati (nolock) On formulati.fr_cod = mcs.pro_cod  And mcs.elem_cod = 191   ")
            StrSQL.AppendLine(" Left Join categoriemagazzino cm (nolock) on cm.Elem_Cod = mt.elem_cod ")
            StrSQL.AppendLine(" Left Join Progetti_Imputazioni imp on imp.piva = c.piva and imp.imputazione_cod = c.id_imputazione ")
            StrSQL.AppendLine(" Left Join reg_impianti_codici ric (nolock) On ric.piva = c.piva And ric.sa_cod = c.sa_cod ")
            StrSQL.AppendLine(" And ric.appezza= c.appezza And ric.id_reg=c.id_reg And id_cod = 1300 ")
            StrSQL.AppendLine(" Left Join reg_impianti ri (nolock) On ri.piva = c.piva And ri.sa_cod = c.sa_cod ")
            StrSQL.AppendLine(" And ri.appezza= c.appezza And ri.id_reg=c.id_reg ")
            StrSQL.AppendLine(" Left Join Appezzamento ap (nolock) On ap.piva = ri.piva And ap.sa_cod = ri.sa_cod ")
            StrSQL.AppendLine(" And ap.appezza= ri.appezza  ")
            StrSQL.AppendLine(" Left Join Appezzamento_Codici apc (nolock) On apc.piva = ap.piva And apc.sa_cod = ap.sa_cod  ")
            StrSQL.AppendLine(" And apc.appezza = ap.appezza And apc.id_cod = 1018 ")
            StrSQL.AppendLine(" Left Join Appezzamento_Codici apc_BIO (nolock) On apc_BIO.piva = ap.piva And apc_BIO.sa_cod = ap.sa_cod  ")
            StrSQL.AppendLine(" And apc_BIO.appezza = ap.appezza And apc_BIO.id_cod = 1085 ")
            StrSQL.AppendLine(" Left Join Appezzamento_Codici apc_cod (nolock) On apc_cod.piva = ap.piva And apc_cod.sa_cod = ap.sa_cod ")
            StrSQL.AppendLine(" And apc_cod.appezza = ap.appezza And apc_cod.id_cod = 1104 ")
            StrSQL.AppendLine(" Join mov_destinazioni dest (nolock) On dest.Piva = mcs.piva ")
            StrSQL.AppendLine(" And dest.Sa_Cod = mcs.sa_cod And dest.Id_Agenda = mcs.Id_Agenda And dest.Id_Mov = mcs.Id_Mov ")
            StrSQL.AppendLine(" And dest.Id_Mov_Det = mcs.Id_Mov_Det --, Id_Destinazione ")
            StrSQL.AppendLine(" Left Join fabbricati (nolock) On  dest.Piva = fabbricati.piva ")
            StrSQL.AppendLine(" And dest.Sa_Cod = fabbricati.sa_cod ")
            StrSQL.AppendLine(" And dest.Id_Destinazione = fabbricati.fabbricato_cod ")
            StrSQL.AppendLine(" Left Join Dati_Animali anim on anim.Cod_Progetto = c.id_reg -- TODO Check con scatto ")
            StrSQL.AppendLine(" ) qp")
            StrSQL.AppendLine(" Left Join Movimenti_Registrazioni AS mov_reg On")
            StrSQL.AppendLine(" qp.piva = mov_reg.piva")
            StrSQL.AppendLine(" AND qp.id_agenda = mov_reg.id_agenda")
            If tipoAppezzamento IsNot Nothing AndAlso tipoAppezzamento.Any Then
                StrSQL.Append(" WHERE qp.Codice_Bio_Convers IN (" & Agro_SQL_Save_Clausola_IN(String.Join(",", tipoAppezzamento)) & ") ")
            End If
            StrSQL.AppendLine(" order by qp.data_movimento desc, qp.ora, qp.articolo, qp.lotto, qp.qta ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            EseguiQuery_Scrittura(objParametri, Elimina_Tabella_Temporanea("#pive"), NomeRoutine)
            EseguiQuery_Scrittura(objParametri, Elimina_Tabella_Temporanea("#Movimenti_Magazzino"), NomeRoutine)
            EseguiQuery_Scrittura(objParametri, Elimina_Tabella_Temporanea("#Movimenti_CaricoScarico "), NomeRoutine)
            EseguiQuery_Scrittura(objParametri, Elimina_Tabella_Temporanea("#Movimenti_Trattamenti "), NomeRoutine)
            EseguiQuery_Scrittura(objParametri, Elimina_Tabella_Temporanea("#Movimenti_Carico "), NomeRoutine)
            EseguiQuery_Scrittura(objParametri, Elimina_Tabella_Temporanea("#Movimenti_Animali "), NomeRoutine)
            EseguiQuery_Scrittura(objParametri, Elimina_Tabella_Temporanea("#Costi_Tutti "), NomeRoutine)
            EseguiQuery_Scrittura(objParametri, Elimina_Tabella_Temporanea("#costi_animali "), NomeRoutine)
            EseguiQuery_Scrittura(objParametri, Elimina_Tabella_Temporanea("#Imprese_Progetti_Distinct "), NomeRoutine)
            EseguiQuery_Scrittura(objParametri, Elimina_Tabella_Temporanea("#Progetti_Costi_Completa "), NomeRoutine)
            EseguiQuery_Scrittura(objParametri, Elimina_Tabella_Temporanea("#Imprese_Progetti_Tutti "), NomeRoutine)
            EseguiQuery_Scrittura(objParametri, Elimina_Tabella_Temporanea("#Progetti_Animali "), NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Private Function Elimina_Tabella_Temporanea(nomeTabella As String) As String
        Dim stb As New StringBuilder



        stb.AppendLine(String.Format(" IF NOT OBJECT_ID('tempdb.dbo.{0}') IS NULL BEGIN ", nomeTabella))
        stb.AppendLine(String.Format("    DROP TABLE {0} ", nomeTabella))
        stb.AppendLine(" END ")

        Return stb.ToString()

    End Function

    Public Function Report_Carichi_Scarichi_OLD(ByVal piva As String,
                                             ByVal tipo_Report As Integer,
                                             ByVal dataMovDal As DateTime, ByVal dataMovAl As DateTime,
                                             ByVal categorieProdotti As List(Of Integer),
                                             ByVal causaliMovimento As List(Of String),
                                             ByVal tipoAppezzamento As List(Of Integer),
                                             ByVal origineDatiScarichi As Integer,
                                             ByVal sa_cod As Integer,
                                             ByVal xOrderBy As String,
                                             ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "BIO_ReportBio.Report_Carichi_Scarichi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Dim elencoCausaliMagazzinoTutte As String = String.Format("'{0}','{1}','{2}','{3}', '{4}', '{5}', '{6}', '{7}'", CAU_CARICO, CAU_SCARICO, CAU_TRATTAMENTO, CAU_LAVORAZIONE, CAU_ANIMALE, CAU_ALIMENTAZIONE, CAU_CARICO_CONSISTENZE, CAU_SCARICO_CONSISTENZE)
            Dim elencoCausaliMagazzinoTrattamentiLavorazioni As String = String.Format("'{0}','{1}'", CAU_TRATTAMENTO, CAU_LAVORAZIONE)
            Dim elencoCausaliAnimali As String = String.Format("'{0}','{1}','{2}','{3}'", CAU_ANIMALE, CAU_ALIMENTAZIONE, CAU_CARICO_CONSISTENZE, CAU_SCARICO_CONSISTENZE)
            Dim lavCodAmmessi = New List(Of Integer) From
                {
                    LAVCOD_DISTRIBUZIONE_CONCIME, LAVCOD_SARCHIATURA_CONCIMAZIONE, LAVCOD_DISTRIBUZIONE_AMMENDANTI, LAVCOD_CONCIMAZIONE_FOGLIARE, LAVCOD_FERTIRRIGAZIONE, LAVCOD_TRATTAMENTO_ANTIBUTTERATURA,
                    LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_DISERBO, LAVCOD_DISSECCAMENTO, LAVCOD_GEODISINFESTAZIONE, LAVCOD_CONCIA_SEME, LAVCOD_TRATTAMENTO_FITOREGOLATORE, LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE
                }

            For i As Integer = 0 To causaliMovimento.Count - 1
                causaliMovimento(i) = "'" & Agro_SQL_SaveText(causaliMovimento(i)) & "'"
            Next

            StrSQL.AppendLine(" With ")
            StrSQL.AppendLine(" PIVE(piva) ")
            StrSQL.AppendLine(" as ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine(" Select distinct piva from imprese (nolock) ")
            StrSQL.AppendLine(" ), ")

            ' CTE Movimenti_Registrazioni
            StrSQL.AppendLine(" Movimenti_Registrazioni ")
            StrSQL.AppendLine(" as ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine(" Select * from Movimenti (nolock) ")
            StrSQL.AppendLine(" where piva = '" & Agro_SQL_SaveText(piva) & "' ")
            StrSQL.AppendLine(" And cau_mov = '4000' ")
            If dataMovDal <> AGRODATAINIZIO Then
                StrSQL.Append(" AND Data_Movimento >= " & Agro_SQL_SaveDate(dataMovDal) & "")
            End If
            If dataMovAl <> AGRODATAFINE Then
                StrSQL.Append(" AND Data_Movimento <= " & Agro_SQL_SaveDate(dataMovAl) & "")
            End If
            StrSQL.AppendLine(" ), ")

            ' CTE Movimenti_Magazzino
            StrSQL.AppendLine(" Movimenti_Magazzino(piva, id_agenda, cod_risum, id_mov, cau_mov, data_movimento, ora, Id_Mov_Det, elem_cod, Pro_Cod, Mat_Cod, principi_attivi, lotto, qta, cod_progetto, sa_cod, n, cu, Efficienza, rif_esterno, rif_esterno_2) ")
            StrSQL.AppendLine(" AS  ")
            StrSQL.AppendLine(" (    ")
            StrSQL.AppendLine(" Select m.piva, m.id_agenda, m.cod_risum, ")
            StrSQL.AppendLine(" m.id_mov, m.cau_mov, m.data_movimento, m.ora, ")
            StrSQL.AppendLine(" MD.Id_Mov_Det, MD.Elem_Cod, md.Pro_Cod, md.Mat_Cod, md.principiattivi, md.lotto, md.qta, md.cod_progetto, md.sa_cod, ")
            StrSQL.AppendLine(" mdt.n, mdt.Cu, mdt.Efficienza, rif_esterno, rif_esterno_2 ")
            StrSQL.AppendLine(" from movimenti m (nolock) ")
            StrSQL.AppendLine(" inner Join Movimenti_dettagli md (nolock) on md.id_mov = m.id_mov ")
            StrSQL.AppendLine(" inner Join PIVE on md.PIVA = pive.piva ")
            StrSQL.AppendLine(" Left Join mov_dettaglio_tecnico mdt (nolock) on md.id_mov_det = mdt.id_mov_det ")
            StrSQL.AppendLine(" where m.cau_mov IN (" & elencoCausaliMagazzinoTutte & ") ")
            StrSQL.AppendLine(" And pive.piva = '" & Agro_SQL_SaveText(piva) & "' ")
            If categorieProdotti.Count > 0 Then
                StrSQL.AppendLine(" And md.elem_cod  in (" & Agro_SQL_Save_Clausola_IN(String.Join(",", categorieProdotti)) & ") ")
            End If
            ''StrSQL.AppendLine("  AND M.id_agenda = 138889 ")
            ''StrSQL.AppendLine("  AND M.id_agenda IN (160496, 160495) ")
            StrSQL.AppendLine(" ), ")

            ' CTE Movimenti_Trattamenti
            StrSQL.AppendLine(" Movimenti_Trattamenti(piva, id_agenda, cod_risum, id_mov, cau_mov, data_movimento, ora, Id_Mov_Det, elem_cod, Pro_Cod, Mat_Cod, principi_attivi, lotto, qta, cod_progetto, sa_cod, n, cu, Efficienza) ")
            StrSQL.AppendLine(" AS  ")
            StrSQL.AppendLine(" (    ")
            StrSQL.AppendLine(" Select piva, id_agenda, cod_risum, id_mov, cau_mov, data_movimento, ora, Id_Mov_Det, elem_cod, Pro_Cod, Mat_Cod, principi_attivi, lotto, qta, cod_progetto, sa_cod, n, cu, Efficienza ")
            StrSQL.AppendLine(" From Movimenti_Magazzino ")
            StrSQL.AppendLine(" Where cau_mov In (" & elencoCausaliMagazzinoTrattamentiLavorazioni & ") ")
            If dataMovDal <> AGRODATAINIZIO Then
                StrSQL.Append(" AND Data_Movimento >= " & Agro_SQL_SaveDate(dataMovDal) & "")
            End If
            If dataMovAl <> AGRODATAFINE Then
                StrSQL.Append(" AND Data_Movimento <= " & Agro_SQL_SaveDate(dataMovAl) & "")
            End If
            StrSQL.AppendLine(" ), ")

            ' CTE Movimenti_CaricoScarico
            StrSQL.AppendLine(" Movimenti_CaricoScarico(piva, id_agenda, cod_risum, id_mov, cau_mov, data_movimento, ora, Id_Mov_Det, elem_cod, Pro_Cod, Mat_Cod, principi_attivi, lotto, qta, cod_progetto, sa_cod, n, cu, rif_esterno, rif_esterno_2) ")
            StrSQL.AppendLine(" AS  ")
            StrSQL.AppendLine(" (  ")
            StrSQL.AppendLine(" Select piva, id_agenda, cod_risum,  id_mov, cau_mov, data_movimento, ora, Id_Mov_Det, elem_cod, Pro_Cod, Mat_Cod, principi_attivi, lotto, qta, cod_progetto, sa_cod, n, cu, rif_esterno, rif_esterno_2 ")
            StrSQL.AppendLine(" From Movimenti_Magazzino ")
            StrSQL.AppendLine(" Where cau_mov in (" & Agro_SQL_Save_Clausola_IN(String.Join(",", causaliMovimento), True) & " )")
            If dataMovDal <> AGRODATAINIZIO Then
                StrSQL.Append(" AND Data_Movimento >= " & Agro_SQL_SaveDate(dataMovDal) & "")
            End If
            If dataMovAl <> AGRODATAFINE Then
                StrSQL.Append(" AND Data_Movimento <= " & Agro_SQL_SaveDate(dataMovAl) & "")
            End If
            StrSQL.AppendLine(" ), ")

            ' CTE Movimenti_Carico
            StrSQL.AppendLine(" Movimenti_Carico ")
            StrSQL.AppendLine(" as ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine(" Select m.Cod_RisUm, m.Elem_Cod, m.Pro_Cod, m.Mat_Cod, m.Lotto from ")
            StrSQL.AppendLine(" Movimenti_CaricoScarico m ")
            StrSQL.AppendLine(" where m.cau_mov = '" & CAU_CARICO & "' ")
            StrSQL.AppendLine(" ), ")

            ' CTE Movimenti_Animali
            StrSQL.AppendLine("Movimenti_Animali as ")
            StrSQL.AppendLine("( ")
            StrSQL.AppendLine(" Select piva, id_agenda, cod_risum, id_mov, cau_mov, data_movimento, ora, Id_Mov_Det, elem_cod, Pro_Cod, Mat_Cod, principi_attivi, lotto, qta, cod_progetto, sa_cod, n, cu, Efficienza ")
            StrSQL.AppendLine(" From Movimenti_Magazzino ")
            StrSQL.AppendLine(" Where cau_mov In (" & elencoCausaliAnimali & ")")
            If dataMovDal <> AGRODATAINIZIO Then
                StrSQL.Append(" AND Data_Movimento >= " & Agro_SQL_SaveDate(dataMovDal) & "")
            End If
            If dataMovAl <> AGRODATAFINE Then
                StrSQL.Append(" AND Data_Movimento <= " & Agro_SQL_SaveDate(dataMovAl) & "")
            End If
            StrSQL.AppendLine(" ), ")

            StrSQL.AppendLine("AgendeDistinct ")
            StrSQL.AppendLine(" as ")
            StrSQL.AppendLine(" (Select distinct id_agenda from Movimenti_CaricoScarico), ")
            StrSQL.AppendLine(" Riferimenti(Id_agenda, Id_Agenda_rif) ")
            StrSQL.AppendLine(" as ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine(" Select distinct mrif.Id_Agenda, mrif.Id_Agenda_rif from AgendeDistinct mcs ")
            StrSQL.AppendLine(" inner Join Mov_Dettagli_Riferimenti mrif (nolock) on mcs.id_agenda = mrif.Id_Agenda ")
            StrSQL.AppendLine(" ), ")

            ' CTE Costi_Campagna
            StrSQL.AppendLine(" Costi_Campagna(tipo, piva, Id_agenda, Id_agenda_Rif, Mat_Cod, Pro_Cod, Elem_Cod, lotto, sa_cod, Appezza, Id_Reg, Id_Imputazione, Progetto_Cod, QtaTotale, Valore, Id_CDG, imputazione) ")
            StrSQL.AppendLine(" As ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine(" Select 'campagna' as tipo, t.Piva, rif.Id_Agenda, rif.Id_Agenda_Rif, t.Mat_Cod, t.Pro_Cod, t.Elem_Cod, t.lotto, d.sa_cod, d.Appezza, d.Id_Reg, d.Id_Imputazione, d.Progetto_Cod,t.qta, d.Valore, t.Id_CDG, t.Tab_Imputazione ")
            StrSQL.AppendLine(" From Riferimenti rif ")
            StrSQL.AppendLine(" inner Join cdg_testata t (nolock) On t.Id_Agenda =rif.Id_agenda_rif And t.Budget = 0 ")
            StrSQL.AppendLine(" inner Join CDG_Dettagli d (nolock) On t.Id_CDG = d.Id_CDG ")
            StrSQL.AppendLine(" where t.elem_cod <> 0 And (t.pro_cod <> 0 Or t.mat_cod <> 0) And t.elem_cod <> 700 and (d.Cod_Animale = 0 and d.Cod_Animale_Distinta = 0) ")
            Select Case origineDatiScarichi
                Case 1  'qdc
                    StrSQL.AppendLine(" and t.tab_imputazione = 'Eredita' ")
                Case 2  'costi
                    StrSQL.AppendLine(" and t.tab_imputazione <> 'Eredita' ")
            End Select
            StrSQL.AppendLine(" ), ")

            ' CTE Costi_Non_Campagna
            StrSQL.AppendLine(" Costi_Non_Campagna(tipo, piva, Id_agenda, Id_agenda_Rif, Mat_Cod, Pro_Cod, Elem_Cod, lotto, sa_cod, Appezza, Id_Reg, Id_Imputazione, Progetto_Cod, QtaTotale, Valore, Id_CDG, imputazione) ")
            StrSQL.AppendLine(" As ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine(" Select 'non campagna' as tipo, t.Piva, t.Id_Agenda, 0 As Id_agenda_Rif, t.Mat_Cod, t.Pro_Cod, t.Elem_Cod, t.lotto, d.sa_cod, d.Appezza, d.Id_Reg, d.Id_Imputazione, d.Progetto_Cod,t.qta, d.Valore, t.Id_CDG, t.Tab_Imputazione ")
            StrSQL.AppendLine(" From Movimenti_CaricoScarico mcs ")
            StrSQL.AppendLine(" inner Join cdg_testata t (nolock) On t.Id_Agenda = mcs.id_agenda And t.Budget = 0 ")
            StrSQL.AppendLine(" inner Join CDG_Dettagli d (nolock) On t.Id_CDG = d.Id_CDG ")
            StrSQL.AppendLine(" where t.elem_cod <> 0 And (t.pro_cod <> 0 Or t.mat_cod <> 0) And t.elem_cod <> 700 and (d.Cod_Animale = 0 and d.Cod_Animale_Distinta = 0) ")
            Select Case origineDatiScarichi
                Case 1  'qdc
                    StrSQL.AppendLine(" and t.tab_imputazione = 'Eredita' ")
                Case 2  'costi
                    StrSQL.AppendLine(" and t.tab_imputazione <> 'Eredita' ")
            End Select
            StrSQL.AppendLine(" ), ")

            ' CTE Costi_Animali
            StrSQL.AppendLine(" Costi_Animali(tipo, piva, Id_agenda, Id_agenda_Rif, Mat_Cod, Pro_Cod, Elem_Cod, lotto, sa_cod, Appezza, Id_Reg, Id_Imputazione, Progetto_Cod, QtaTotale, Valore, Id_CDG, imputazione) ")
            StrSQL.AppendLine(" As ")
            StrSQL.AppendLine(" (  ")
            StrSQL.AppendLine(" Select 'animali' as tipo, t.Piva, rif.Id_Agenda, rif.Id_Agenda_Rif, t.Mat_Cod, t.Pro_Cod, t.Elem_Cod, t.lotto, d.sa_cod, d.Appezza, d.Id_Reg, d.Id_Imputazione, d.Progetto_Cod,t.qta, d.Valore, t.Id_CDG , t.Tab_Imputazione ")
            StrSQL.AppendLine(" From Riferimenti rif  ")
            StrSQL.AppendLine(" inner Join cdg_testata t (nolock) On t.Id_Agenda =rif.Id_agenda_rif And t.Budget = 0 ")
            StrSQL.AppendLine(" inner Join CDG_Dettagli d (nolock) On t.Id_CDG = d.Id_CDG  ")
            StrSQL.AppendLine(" where t.elem_cod <> 0 And (t.pro_cod <> 0 Or t.mat_cod <> 0) And t.elem_cod <> 700 And d.Cod_Animale <> 0 And d.Cod_Animale_Distinta <> 0 ")
            Select Case origineDatiScarichi
                Case 1  'qdc
                    StrSQL.AppendLine(" and t.tab_imputazione = 'Eredita' ")
                Case 2  'costi
                    StrSQL.AppendLine(" and t.tab_imputazione <> 'Eredita' ")
            End Select
            StrSQL.AppendLine(" ), ")

            ' CTE Costi_Tutti
            StrSQL.AppendLine(" Costi_Tutti(tipo, piva, Id_agenda, Id_agenda_Rif, Mat_Cod, Pro_Cod, Elem_Cod, lotto, sa_cod, Appezza, Id_Reg, Id_Imputazione, Progetto_Cod, QtaTotale, Valore, Id_CDG, imputazione) ")
            StrSQL.AppendLine(" As ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine(" Select tipo, piva, Id_agenda, Id_agenda_Rif, Mat_Cod, Pro_Cod, Elem_Cod, lotto, sa_cod, Appezza, Id_Reg, Id_Imputazione, Progetto_Cod, QtaTotale, Valore, Id_CDG, imputazione from Costi_Campagna ")
            StrSQL.AppendLine(" union ")
            StrSQL.AppendLine(" Select tipo, piva, Id_agenda, Id_agenda_Rif, Mat_Cod, Pro_Cod, Elem_Cod, lotto, sa_cod, Appezza, Id_Reg, Id_Imputazione, Progetto_Cod, QtaTotale, Valore, Id_CDG, imputazione from Costi_Non_Campagna ")
            StrSQL.AppendLine(" union ")
            StrSQL.AppendLine(" Select tipo, piva, Id_agenda, Id_agenda_Rif, Mat_Cod, Pro_Cod, Elem_Cod, lotto, sa_cod, Appezza, Id_Reg, Id_Imputazione, Progetto_Cod, QtaTotale, Valore, Id_CDG, imputazione from Costi_Animali ")
            StrSQL.AppendLine(" ), ")

            ' CTE Imprese_Progetti_Campagna
            StrSQL.AppendLine(" Imprese_Progetti_Campagna(id_agenda, piva, sa_cod, cod_progetto, appezza, id_reg, progetto_nome) ")
            StrSQL.AppendLine(" As ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine(" Select Distinct mcs.id_agenda , mcs.piva, mcs.sa_cod, mcs.cod_progetto, ip.appezza, ip.id_reg, ip.progetto_nome from Movimenti_CaricoScarico mcs ")
            StrSQL.AppendLine(" inner Join imprese_progetti ip (nolock) On mcs.piva = ip.piva And mcs.cod_progetto = ip.Progetto_Cod ")
            StrSQL.AppendLine(" ), ")

            ' CTE ImpreseProgetti_Non_Campagna
            StrSQL.AppendLine(" ImpreseProgetti_Non_Campagna(id_agenda, piva, sa_cod, cod_progetto, appezza, id_reg, progetto_nome) ")
            StrSQL.AppendLine(" As ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine(" Select Distinct c.Id_agenda, c.Piva, c.sa_cod, c.Progetto_Cod, ip.appezza, ip.id_reg , ip.progetto_nome from costi_tutti c ")
            StrSQL.AppendLine(" inner Join imprese_progetti ip (nolock) On c.piva = ip.piva And ip.Progetto_Cod = c.Progetto_Cod And ip.Piva = c.Piva And ip.Id_Reg = c.Id_Reg And ip.Appezza = c.Appezza And ip.Sa_Cod = c.sa_cod and c.tipo <> 'animale' ")
            StrSQL.AppendLine(" ), ")

            ' CTE Progetti_Animali
            StrSQL.AppendLine(" Progetti_Animali(id_agenda, piva, sa_cod, cod_progetto, appezza, id_reg, progetto_nome)  As ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine(" Select distinct c.id_agenda, c.Piva, c.sa_cod,  99 As Progetto_Cod, --zad.Progetto_Des As Progetto_Cod //TODO Check Scatto ")
            StrSQL.AppendLine(" c.appezza, c.Id_Reg, za.Progetto as progetto_nome ")
            StrSQL.AppendLine(" From costi_animali c ")
            StrSQL.AppendLine(" Left Join Zoo_Animali za on c.id_reg = za.Cod_Progetto And c.piva = za.PIVA ")
            StrSQL.AppendLine(" Left Join Zoo_Animali_Distinte zad on zad.Cod_Animale = c.Id_Reg And c.PIVA = zad.Piva ")
            StrSQL.AppendLine(" ), ")

            ' CTE Imprese_Progetti_Tutti
            StrSQL.AppendLine(" Imprese_Progetti_Tutti(id_agenda, piva, sa_cod, cod_progetto, appezza, id_reg, progetto_nome) ")
            StrSQL.AppendLine(" As ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine("     Select id_agenda, piva, sa_cod, cod_progetto, appezza, id_reg ,progetto_nome from Imprese_Progetti_Campagna ")
            StrSQL.AppendLine("     UNION ")
            StrSQL.AppendLine("     Select id_agenda, piva, sa_cod, cod_progetto, appezza, id_reg ,progetto_nome from ImpreseProgetti_Non_Campagna ")
            StrSQL.AppendLine("     union ")
            StrSQL.AppendLine("     Select id_agenda, piva, sa_cod, cod_progetto, appezza, id_reg ,progetto_nome from Progetti_Animali ")
            StrSQL.AppendLine(" ), ")

            ' CTE Imprese_Progetti_Distinct
            StrSQL.AppendLine(" Imprese_Progetti_Distinct(id_agenda, piva, sa_cod, cod_progetto, appezza, id_reg, progetto_nome) ")
            StrSQL.AppendLine(" As ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine(" Select distinct id_agenda, piva, sa_cod, cod_progetto, appezza, id_reg, progetto_nome ")
            StrSQL.AppendLine(" From Imprese_Progetti_Tutti ")
            StrSQL.AppendLine(" ), ")

            StrSQL.AppendLine(" Progetti_Imputazioni  ")
            StrSQL.AppendLine(" As ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine(" Select * from Imputazioni (nolock) ")
            StrSQL.AppendLine(" ), ")

            ' CTE Fornitori
            StrSQL.AppendLine(" Fornitori ")
            StrSQL.AppendLine(" As ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine(" Select distinct mcs.elem_cod, mcs.pro_cod, mcs.mat_cod, mcs.lotto , ")
            StrSQL.AppendLine(" ru.settore_des, ru.cod_risum, ")
            StrSQL.AppendLine(" contatti.cod_contatto, contatti.cognome, contatti.nome, contatti.rag_soc ")
            StrSQL.AppendLine(" From risorse_umane ru (nolock) ")
            StrSQL.AppendLine(" inner Join contatti (nolock) On contatti.cod_contatto = ru.cod_contatto And (ru.Piva = Contatti.Piva  Or Contatti.Sa_Cod = -1)    ")
            StrSQL.AppendLine(" inner Join Movimenti_carico mc On ru.cod_risum = mc.Cod_RisUm  ")
            StrSQL.AppendLine(" inner Join Movimenti_CaricoScarico mcs On mc.elem_cod = mcs.elem_cod And mc.pro_cod = mcs.pro_cod And mc.Mat_Cod = mcs.mat_cod And mc.lotto = mcs.lotto ")
            StrSQL.AppendLine("where ru.Piva = '" & Agro_SQL_SaveText(piva) & "'")
            StrSQL.AppendLine(" ), ")
            StrSQL.AppendLine(" fornitori_top ")
            StrSQL.AppendLine(" as ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine(" Select * from ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine(" Select *, ROW_NUMBER () OVER (PARTITION BY elem_cod, pro_cod, mat_cod, lotto ORDER BY cod_risum DESC) as prog ")
            StrSQL.AppendLine(" From Fornitori) q ")
            StrSQL.AppendLine(" Where prog = 1 ")
            StrSQL.AppendLine(" ), ")

            ' CTE Classificazione_Formulati_CTE
            StrSQL.AppendLine("Classificazione_Formulati_CTE ")
            StrSQL.AppendLine("as ")
            StrSQL.AppendLine("(")
            StrSQL.AppendLine("Select Fr_Cod, fr_des, fc.Class_Cod, cf.Class_Des from Formulati f ")
            StrSQL.AppendLine("Left Join formulatixclassificazioni fc ")
            StrSQL.AppendLine("On f.Fr_Cod = fc.For_Cod ")
            StrSQL.AppendLine("Left Join ClassificazioniFormulati cf ")
            StrSQL.AppendLine("On cf.Class_Cod = fc.Class_Cod ")
            StrSQL.AppendLine(" ), ")

            ' CTE Dati_Animali
            StrSQL.AppendLine(" Dati_Animali as  ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine(" Select distinct ")
            StrSQL.AppendLine(" za.Cod_Progetto, Matricola, za.Gen_cod, za.Spe_Cod, za.IPRO_Cod, za.RAZ_COD, Nome, ")
            StrSQL.AppendLine(" gen.GEN_DES, spe.SPE_DES, raz.RAZ_DES ")
            StrSQL.AppendLine(" From zoo_animali za ")
            StrSQL.AppendLine(" inner Join Progetti_Animali pa on za.Cod_Progetto = pa.id_reg ")
            StrSQL.AppendLine(" inner Join Lista_Generi_Animali gen on za.GEN_COD = gen.GEN_COD  ")
            StrSQL.AppendLine(" inner Join Lista_Specie_Animali spe on za.gen_cod = spe.gen_cod And  za.SPE_COD = spe.SPE_COD  ")
            StrSQL.AppendLine(" inner Join Lista_Razze_Animali raz on za.GEN_COD = raz.GEN_COD And za.SPE_COD = raz.SPE_COD And za.RAZ_COD = raz.RAZ_COD ")
            StrSQL.AppendLine(" ) ")

            'Inizio query principale
            StrSQL.AppendLine(" Select qp.*, mov_reg.doc_numero_sin + LTRIM(STR(mov_reg.doc_numero, 10, 0)) + mov_reg.doc_numero_des ddt_numero, mov_reg.data_movimento ddt_data, mov_reg.ora ddt_ora from ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine(" Select mcs.id_agenda, c.id_agenda_rif,  mcs.sa_cod, mcs.elem_cod, mcs.pro_cod, mcs.mat_cod, mcs.id_mov, mcs.id_mov_det, mcs.Piva, mcs.Cau_Mov, mcs.cod_risum, ")
            StrSQL.AppendLine(" Case   When  mcs.cau_mov  = '7300' Then 'Carico' ")
            StrSQL.AppendLine(" When  mcs.cau_mov  = '7350' Then 'Scarico' ")
            StrSQL.AppendLine(" End As Tipo_Movimento, ")
            StrSQL.AppendLine(" Case   When  mcs.Elem_Cod  = 3 Then 'Fertilizzante' ")
            StrSQL.AppendLine(" When  mcs.Elem_Cod  = 10 Then 'Semente' ")
            StrSQL.AppendLine(" When  mcs.Elem_Cod  = 191 Then 'Fitofarmaci' ")
            StrSQL.AppendLine(" When  mcs.Elem_Cod  = 304 Then 'Materie Prime Animali' ")
            StrSQL.AppendLine(" WHEN  mcs.Elem_Cod  = 201 Then 'Semilavorati Vegetali' ")
            StrSQL.AppendLine(" Else isnull(cm.NomeComune, '') ")
            StrSQL.AppendLine(" End As Tipo_Prodotto, ")
            StrSQL.AppendLine(" Case  When ISNULL(mp.Cod_Articolo, '') <> '' Then mp.Cod_Articolo ")
            StrSQL.AppendLine(" WHEN ISNULL(mp.Cod_Articolo, '') = '' Then  ")
            StrSQL.AppendLine(" Case   ")
            StrSQL.AppendLine(" WHEN Not EXISTS(SELECT 1 FROM CAC_Codifica_ProdottiAziendali (nolock) WHERE CAC_Codifica_ProdottiAziendali.Elem_Cod = mcs.Elem_Cod)  ")
            StrSQL.AppendLine(" THEN ''  ")
            StrSQL.AppendLine(" Else ")
            StrSQL.AppendLine(" ISNULL((SELECT TOP(1) Cod_Prodotto_Cliente  ")
            StrSQL.AppendLine(" From CAC_Codifica_ProdottiAziendali (nolock) ")
            StrSQL.AppendLine(" Where CAC_Codifica_ProdottiAziendali.Elem_Cod = mcs.Elem_Cod ")
            StrSQL.AppendLine(" And Codice_GIAS = mcs.Pro_Cod  ")
            StrSQL.AppendLine(" And Piva = mcs.Piva) , '') ")
            StrSQL.AppendLine(" End ")
            StrSQL.AppendLine(" End as Articolo, ")
            StrSQL.AppendLine(" Case   When  mcs.Elem_Cod  = 3 Then ISNULL(fertilizzanti.fer_des, '')  ")
            StrSQL.AppendLine(" When  mcs.Elem_Cod  = 10 Then ISNULL(mp.Mat_Des, '') ")
            StrSQL.AppendLine(" When  mcs.Elem_Cod  = 191 Then ISNULL(formulati.fr_des, '') ")
            StrSQL.AppendLine(" When  mcs.Elem_Cod  = 304 Then ISNULL(mp.Mat_Des, '')  ")
            StrSQL.AppendLine(" WHEN  mcs.Elem_Cod  = 201 Then ISNULL(mp.Mat_Des, '')  ")
            StrSQL.AppendLine(" End As Descrizione,  ")
            StrSQL.AppendLine("Case when mcs.elem_cod = 191 then ")
            StrSQL.AppendLine(" coalesce(STUFF(( ")
            StrSQL.AppendLine(" Select ', ' + cf.Class_Des ")
            StrSQL.AppendLine(" From Classificazione_Formulati_CTE cf ")
            StrSQL.AppendLine("Where cf.fr_cod = mcs.pro_cod ")
            StrSQL.AppendLine(" For Xml PATH ('')),1,2,''), '')  ")
            StrSQL.AppendLine(" Else '' end as Classificazione, ")

            StrSQL.AppendLine(" ISNULL(ip.cod_progetto, '') as Progetto_Cod, ")
            StrSQL.AppendLine(" ISNULL(ip.progetto_nome, ISNULL(imp.CodiceSecondario, '')) as OP, ")

            StrSQL.AppendLine(" isnull(f.settore_des, '' ) as Fornitore, ")
            StrSQL.AppendLine(" RTrim(isnull(isnull(f.rag_soc, '')  + isnull(f.cognome, '') + ' ' + isnull(f.nome, ''),'')) as  Fornitore_Ragione_Sociale, ")
            StrSQL.AppendLine(" Fabbricato_Des,  ")
            StrSQL.AppendLine(" dest.id_destinazione, ")
            StrSQL.AppendLine(" Case When ISNULL(apc.val_Cod, '') = '' Then ''   ")
            StrSQL.AppendLine(" WHEN ISNULL(apc.val_Cod, '') = '1' Then 'Integrato'  ")
            StrSQL.AppendLine(" WHEN ISNULL(apc.val_Cod, '') = '2' Then 'In Conversione'  ")
            StrSQL.AppendLine(" WHEN ISNULL(apc.val_Cod, '') = '3' Then 'Biologico'   end as Bio_Convers,  ")
            StrSQL.AppendLine(" Isnull(apc.val_cod, 0) as Codice_Bio_Convers, ")
            StrSQL.AppendLine(" ISNULL(apc_BIO.val_Cod, '') as App_BIO, ")
            StrSQL.AppendLine(" mcs.Lotto, ")
            StrSQL.AppendLine(" mcs.data_movimento, ")
            StrSQL.AppendLine(" mcs.Ora, ")

            StrSQL.AppendLine(" Case when mcs.cau_mov = '7300' then isnull(c.QtaTotale,0) else c.QtaTotale * -1 end as QtaTotale_CDG, ")
            StrSQL.AppendLine(" Case when c.QtaTotale Is null then ")
            StrSQL.AppendLine(" Case when mcs.cau_mov = '7300' then mcs.Qta else mcs.Qta * -1 end  ")
            StrSQL.AppendLine(" Else ")
            StrSQL.AppendLine(" Case when mcs.cau_mov = '7300' then  round(  (isnull(c.QtaTotale, 0) * c.Valore) / 100,6)	else round( ((isnull(c.QtaTotale, 0) * c.Valore) / 100),6) * -1 end ")
            StrSQL.AppendLine(" End As Qta, ")
            StrSQL.AppendLine(" isnull(mcs.qta, 0) As qta_originale, ")
            StrSQL.AppendLine(" ISNULL(ric.val_cod, ISNULL(imp.CodicePrincipale, '')) as Progetto, ")

            StrSQL.AppendLine(" ISNULL(apc_cod.val_Cod, '') as Campo, ")
            StrSQL.AppendLine(" ISNULL(app_nome, '') as Campo_Descrizione, ")
            StrSQL.AppendLine(" ISNULL(ri.sup_imp, 0) As Sup,   ")
            StrSQL.AppendLine(" ISNULL(mt.principi_attivi,'') as tratta_Principi_attivi, ")

            StrSQL.AppendLine(" Case When  mcs.Elem_Cod  = 3 then ")
            StrSQL.AppendLine("     Case when mt.cu Is null then ")
            StrSQL.AppendLine("         isnull(FERTILIZZANTI.Cu, 0) ")
            StrSQL.AppendLine(" Else mt.cu End ")
            StrSQL.AppendLine(" Else ")
            StrSQL.AppendLine(" ISNULL(mt.cu, 0) End As Tratta_cu, ")
            StrSQL.AppendLine(" Case When  mcs.Elem_Cod  = 3 then ")
            StrSQL.AppendLine("     Case when mt.n Is null then ")
            StrSQL.AppendLine("         isnull(FERTILIZZANTI.n, 0) ")
            StrSQL.AppendLine(" Else mt.n End ")
            StrSQL.AppendLine(" Else ")
            StrSQL.AppendLine(" ISNULL(mt.n, 0) End As Tratta_Azoto, ")


            StrSQL.AppendLine(" ISNULL(mt.Efficienza, 0) As tratta_Efficienza, ")
            StrSQL.AppendLine(" mcs.principi_attivi As carscar_Principi_attivi, ")
            StrSQL.AppendLine(" mcs.cu as carscar_CU, ")
            StrSQL.AppendLine(" mcs.n as carscar_Azoto, ")

            StrSQL.AppendLine(" isnull(c.QtaTotale, 0) As QtaTotale,  ")
            StrSQL.AppendLine(" isnull(c.Valore, 0) As Valore, ")
            StrSQL.AppendLine(" coalesce(anim.Matricola, '') as Matricola, ")
            StrSQL.AppendLine(" coalesce(anim.GEN_COD, 0) as Gen_Cod, ")
            StrSQL.AppendLine(" coalesce(anim.gen_des, '') as Gen_Des, ")
            StrSQL.AppendLine(" coalesce(anim.spe_cod, 0) as Spe_Cod, ")
            StrSQL.AppendLine(" coalesce(anim.SPE_DES, '') as Spe_Des, ")
            StrSQL.AppendLine(" coalesce(anim.RAZ_COD, 0) as Raz_Cod, ")
            StrSQL.AppendLine(" coalesce(anim.RAZ_DES, '') as Raz_Des, ")
            StrSQL.AppendLine(" coalesce(anim.Nome, '') as Nome_Animale, ")
            StrSQL.AppendLine(" coalesce(mcs.rif_esterno, '') as rif_esterno, ")
            StrSQL.AppendLine(" coalesce(mcs.rif_esterno_2,'') as  rif_esterno_2 ")

            StrSQL.AppendLine(" From Movimenti_CaricoScarico mcs  ")
            If origineDatiScarichi = 0 Then
                StrSQL.AppendLine("left Join ")
            Else
                StrSQL.AppendLine("inner Join ")
            End If

            StrSQL.AppendLine(" Costi_Tutti c on mcs.id_agenda = c.Id_agenda and mcs.Mat_Cod = c.Mat_Cod and mcs.pro_cod = c.Pro_cod and mcs.elem_cod = c.elem_cod and mcs.lotto = c.lotto ")
            StrSQL.AppendLine(" Left Join Imprese_Progetti_Distinct ip on ip.id_agenda = c.id_agenda and ip.piva = c.piva  and ip.sa_cod = c.sa_cod and c.Id_Reg = ip.id_reg ")
            StrSQL.AppendLine(" Left Join Movimenti_Trattamenti mt on mcs.id_agenda = mt.id_agenda And mcs.elem_cod = mt.elem_cod And mcs.pro_cod = mt.pro_cod And mcs.mat_cod = mt.mat_cod And mcs.lotto = mt.lotto  ")
            StrSQL.AppendLine(" Left Join fornitori_top f on mcs.elem_cod = f.elem_cod And mcs.Pro_Cod = f.Pro_Cod And mcs.Mat_Cod = f.Mat_Cod And mcs.lotto = f.lotto ")
            StrSQL.AppendLine(" Left Join materie_prime mp (nolock) On mp.elem_cod = mcs.elem_cod And mp.mat_cod = mcs.mat_cod  ")
            StrSQL.AppendLine(" Left Join fertilizzanti (nolock) On fertilizzanti.fer_cod = mcs.pro_cod  And mcs.elem_cod = 3   ")
            StrSQL.AppendLine(" Left Join formulati (nolock) On formulati.fr_cod = mcs.pro_cod  And mcs.elem_cod = 191   ")
            StrSQL.AppendLine(" Left Join categoriemagazzino cm (nolock) on cm.Elem_Cod = mt.elem_cod ")
            StrSQL.AppendLine(" Left Join Progetti_Imputazioni imp on imp.piva = c.piva and imp.imputazione_cod = c.id_imputazione ")
            StrSQL.AppendLine(" Left Join reg_impianti_codici ric (nolock) On ric.piva = ip.piva And ric.sa_cod = ip.sa_cod ")
            StrSQL.AppendLine(" And ric.appezza= ip.appezza And ric.id_reg=ip.id_reg And id_cod = 1300 ")
            StrSQL.AppendLine(" Left Join reg_impianti ri (nolock) On ri.piva = ip.piva And ri.sa_cod = ip.sa_cod ")
            StrSQL.AppendLine(" And ri.appezza= ip.appezza And ri.id_reg=ip.id_reg ")
            StrSQL.AppendLine(" Left Join Appezzamento ap (nolock) On ap.piva = ri.piva And ap.sa_cod = ri.sa_cod ")
            StrSQL.AppendLine(" And ap.appezza= ri.appezza  ")
            StrSQL.AppendLine(" Left Join Appezzamento_Codici apc (nolock) On apc.piva = ap.piva And apc.sa_cod = ap.sa_cod  ")
            StrSQL.AppendLine(" And apc.appezza = ap.appezza And apc.id_cod = 1018 ")
            StrSQL.AppendLine(" Left Join Appezzamento_Codici apc_BIO (nolock) On apc_BIO.piva = ap.piva And apc_BIO.sa_cod = ap.sa_cod  ")
            StrSQL.AppendLine(" And apc_BIO.appezza = ap.appezza And apc_BIO.id_cod = 1085 ")
            StrSQL.AppendLine(" Left Join Appezzamento_Codici apc_cod (nolock) On apc_cod.piva = ap.piva And apc_cod.sa_cod = ap.sa_cod ")
            StrSQL.AppendLine(" And apc_cod.appezza = ap.appezza And apc_cod.id_cod = 1104 ")
            StrSQL.AppendLine(" Join mov_destinazioni dest (nolock) On dest.Piva = mcs.piva ")
            StrSQL.AppendLine(" And dest.Sa_Cod = mcs.sa_cod And dest.Id_Agenda = mcs.Id_Agenda And dest.Id_Mov = mcs.Id_Mov ")
            StrSQL.AppendLine(" And dest.Id_Mov_Det = mcs.Id_Mov_Det --, Id_Destinazione ")
            StrSQL.AppendLine(" Left Join fabbricati (nolock) On  dest.Piva = fabbricati.piva ")
            StrSQL.AppendLine(" And dest.Sa_Cod = fabbricati.sa_cod ")
            StrSQL.AppendLine(" And dest.Id_Destinazione = fabbricati.fabbricato_cod ")
            StrSQL.AppendLine(" Left Join Dati_Animali anim on anim.Cod_Progetto = ip.id_reg -- TODO Check con scatto ")
            StrSQL.AppendLine(" ) qp")
            StrSQL.AppendLine(" Left Join Movimenti_Registrazioni AS mov_reg On")
            StrSQL.AppendLine(" qp.piva = mov_reg.piva")
            StrSQL.AppendLine(" AND qp.id_agenda = mov_reg.id_agenda")
            'StrSQL.AppendLine("  where rif_esterno = '500014878600012021' or rif_esterno_2 = '500014878600012021' ")

            If tipoAppezzamento IsNot Nothing AndAlso tipoAppezzamento.Any Then
                StrSQL.Append(" WHERE qp.Codice_Bio_Convers IN (" & Agro_SQL_Save_Clausola_IN(String.Join(",", tipoAppezzamento)) & ") ")
            End If


            StrSQL.AppendLine(" order by qp.data_movimento desc, qp.ora, qp.articolo, qp.lotto, qp.qta ")

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

    Private Function DataInStringa(ByVal data As DateTime) As String
        Return data.ToString("yyyy/MM/dd")
    End Function

    Public Function leggiFornitori(ByVal piva As String,
                                   ByVal datainizio As Date,
                                   ByVal datafine As Date,
                                   ByVal centro As String,
                                   ByVal prodotti As String,
                                   ByRef objParametri As AgronicaCoreParametri) As DataTable
        Dim dt As DataTable
        Dim StrSQL As New System.Text.StringBuilder
        Dim NomeRoutine As String = "BIO_ReportBio.leggiFornitori()"
        Dim MessaggioErrore As String = ""

        Try

            StrSQL.AppendLine(" Select Distinct Rag_Soc, mv.cod_risum ")
            StrSQL.AppendLine(" From Contatti c ")
            StrSQL.AppendLine(" Join Risorse_Umane ru on ru.Cod_Contatto = c.Cod_Contatto ")
            StrSQL.AppendLine(" Join Movimenti mv on mv.Cod_RisUm = ru.Cod_RisUm ")
            StrSQL.AppendLine(" Join Movimenti_dettagli mvd on mvd.Id_Mov = mv.Id_Mov ")
            StrSQL.AppendLine(" Where mv.Cau_Mov = '" & CAU_CARICO & "' and mv.Data_Movimento between " & Agro_SQL_SaveDate(datainizio) & " and " & Agro_SQL_SaveDate(datafine) & " ")
            StrSQL.AppendLine(" And mv.PIVA = '" & Agro_SQL_SaveText(piva) & "' and mvd.mat_cod in ( " & Agro_SQL_Save_Clausola_IN(prodotti) & " ) and mvd.elem_cod = '700'  ")

            If (centro <> 0) Then
                StrSQL.AppendLine(" and mvd.sa_cod = " & CInt(centro))
            End If


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return dt
    End Function

End Class
