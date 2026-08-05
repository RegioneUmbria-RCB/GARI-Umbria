Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class Statistiche_Utilizzo_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi_NumOperazioni_xStatistiche_DistinctPivaVegCod_ConUtenti(ByVal FiltroData_Operazione1_Registrazione2 As Integer,
                                                                                  ByVal DataInizio As Date,
                                                                                  ByVal DataFine As Date,
                                                                                  ByVal Username As String,
                                                                                  ByVal xFiltroAggiuntivo As String,
                                                                                  ByVal xFiltroAggiuntivo1 As String,
                                                                                  ByVal xOrderBy As String,
                                                                                  ByRef objParametri_Server As AgronicaCoreParametri,
                                                                                  ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                                                  ByVal Applica_VisibilitaUtente As Boolean,
                                                                                  ByVal FiltroAggiuntivo_Imprese As String) As DataTable

        Dim nomeRoutine As String = "Statistiche_Utilizzo_R.Leggi_NumOperazioni_xStatistiche_DistinctPivaVegCod_ConUtenti"
        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable = Nothing

        Try

            strSql.AppendLine(" SET NOCOUNT ON ")

            '=====================================================================================================
            ' Generazione tabella #Pive
            '=====================================================================================================
            If Not String.IsNullOrEmpty(FiltroAggiuntivo_Imprese) Then
                strSql.AppendLine(Crea_TabellaPive(FiltroAggiuntivo_Imprese))
            End If
            '--- WITH --------------------------------------------------------------------------------------------
            'Utenti_Visibilita_Appoggio_CTE
            strSql.AppendLine(" ;with Utenti_Visibilita_Appoggio_CTE AS ( ")
            strSql.AppendLine(" Select piva from Utenti_Visibilita_Appoggio (NOLOCK) ")
            strSql.AppendLine(" where Utenti_Visibilita_Appoggio.Username = '" & Agro_SQL_SaveText(objParametri_Server.UtenteUsername) & "'")
            strSql.AppendLine(" And Utenti_Visibilita_Appoggio.Entita_Cod = 1 ) ")
            '#Utenti_Visibilita_Appoggio_CTE
            strSql.AppendLine(" Select * into #Utenti_Visibilita_Appoggio_CTE from Utenti_Visibilita_Appoggio_CTE ")
            strSql.AppendLine(" create Index idx_Utenti_Visibilita_Appoggio_CTE on #Utenti_Visibilita_Appoggio_CTE (piva) ")
            '--- WITH --------------------------------------------------------------------------------------------
            'imprese_tutte_CTE
            strSql.AppendLine(" ;with imprese_tutte_CTE as ( ")
            strSql.AppendLine(" Select piva from imprese ) ")
            '#tutte_le_pive
            strSql.AppendLine(" Select * into #tutte_le_pive from ")
            strSql.AppendLine(" ( ")
            strSql.AppendLine(" Select i.piva from imprese_tutte_CTE i ")
            If Applica_VisibilitaUtente Then
                strSql.AppendLine(" inner Join #Utenti_Visibilita_Appoggio_CTE u on i.piva = u.piva ")
            End If
            If Not String.IsNullOrEmpty(FiltroAggiuntivo_Imprese) Then
                strSql.AppendLine(" inner Join #pive p on i.piva = p.piva ")
            End If
            strSql.AppendLine("  ) qi ")
            strSql.AppendLine(" CREATE INDEX Idx_pive_CTE on #tutte_le_pive (piva) ")

            '=====================================================================================================
            ' Preparazioni per generazioni tabelle #Table_OpMag, #Table_OpCamp
            '=====================================================================================================
            '--- WITH --------------------------------------------------------------------------------------------
            'Utenti_Dettagli_CTE
            strSql.AppendLine(" ;with Utenti_Dettagli_CTE AS ( ")
            strSql.AppendLine(" Select nome, Cognome, CodFisc FROM " & objParametri_Utenti.Recupera_NomeDB() & ".dbo.Utenti_Dettagli ) ")
            strSql.AppendLine(" Select * into #Utenti_Dettagli_CTE from Utenti_Dettagli_CTE ")
            strSql.AppendLine(" CREATE Index IDX_C_Utenti_Dettagli_CTE ON #Utenti_Dettagli_CTE(CodFisc); ")
            'agenda_cte
            strSql.AppendLine(" ;with agenda_cte as ( ")
            strSql.AppendLine(" Select a.piva, a.Id_Agenda,a.Data_Modifica, a.Username_Modifica, a.lav_cod, a.des_lib, a.Username_Creazione ")
            strSql.AppendLine(" From agenda a ")
            strSql.AppendLine(" inner Join #tutte_le_pive p on a.PIVA = p.piva ")
            strSql.AppendLine("  ), ")
            'movimenti_CTE
            strSql.AppendLine(" movimenti_CTE as ( ")
            strSql.AppendLine(" Select m.Id_Agenda, m.piva, m.Data_Movimento, m.Id_Mov, m.Cau_Mov ")
            strSql.AppendLine(" From movimenti m ")
            strSql.AppendLine(" inner Join #tutte_le_pive p on m.PIVA = p.piva ")
            AggiungiFiltroData(FiltroData_Operazione1_Registrazione2, DataInizio, DataFine, strSql)
            strSql.AppendLine(String.Format(" And m.cau_mov IN ('{0}','{1}','{2}','{3}') ",
                                            CAU_TRATTAMENTO,
                                            CAU_RILIEVO_CAMPO,
                                            CAU_RILIEVO_RACCOLTA,
                                            CAU_LAVORAZIONE))
            strSql.AppendLine(" AND   m.username_creazione <> '" & objParametri_Server.PivaSuperUser & "' ), ")
            'movimenti_x_Agenda_CTE
            strSql.AppendLine(" movimenti_x_Agenda_CTE as ( ")
            strSql.AppendLine(" Select m.*, a.Lav_Cod, a.des_lib, a.Username_Creazione, a.Username_Modifica from movimenti_CTE m ")
            strSql.AppendLine(" inner Join agenda_cte a on m.id_agenda = a.id_agenda And a.piva = m.PIVA) ")
            '#movimenti_x_Agenda_CTE
            strSql.AppendLine(" Select * into #movimenti_x_Agenda_CTE from movimenti_x_Agenda_CTE ")
            strSql.AppendLine(" create Index IDX_#movimenti_x_Agenda_CTE On #movimenti_x_Agenda_CTE(id_agenda, id_mov) ")

            '=====================================================================================================
            ' Generazione tabella #Table_OpCamp
            '=====================================================================================================
            strSql.AppendLine(" Select * into #Table_OpCamp from ")
            strSql.AppendLine(" ( ")
            'Impianti con specie/varietà
            strSql.AppendLine(" Select ")
            strSql.AppendLine(" m.piva, m.id_agenda, m.Lav_Cod, m.des_lib, Convert(Date, m.Data_Movimento,120) as data_movimento ")
            strSql.AppendLine(", c.Veg_Cod, m.Username_Creazione, ud.nome + ' ' + ud.Cognome as utente ")
            strSql.AppendLine(" From #movimenti_x_Agenda_CTE m ")
            strSql.AppendLine(" inner Join Mov_Destinazioni md on m.id_agenda=md.id_agenda  And m.Id_Mov=md.Id_Mov ")
            strSql.AppendLine(" inner Join Reg_Impianti r on r.piva=m.piva And r.sa_cod=md.Sa_Cod And r.APPEZZA=md.Appezza And r.ID_REG=md.Id_Destinazione ")
            strSql.AppendLine(" inner Join cultivar c on r.CUL_COD=c.Cul_Cod ")
            strSql.AppendLine(" inner Join #Utenti_Dettagli_CTE ud on ud.CodFisc=m.Username_Creazione ")
            strSql.AppendLine(" where md.Tipo_Destinazione = 0 ")
            'Impianti terreno nudo senza destinazione d'uso
            strSql.AppendLine(" UNION ")
            strSql.AppendLine(" Select ")
            strSql.AppendLine(" m.piva, m.id_agenda, m.Lav_Cod, m.des_lib, Convert(Date, m.Data_Movimento,120) as data_movimento ")
            strSql.AppendLine(" ,0 as Veg_Cod , m.Username_Creazione, ud.nome + ' ' + ud.Cognome as utente ")
            strSql.AppendLine(" From #movimenti_x_Agenda_CTE m ")
            strSql.AppendLine(" inner Join Mov_Destinazioni md on m.id_agenda=md.id_agenda  And m.Id_Mov=md.Id_Mov ")
            strSql.AppendLine(" inner Join Reg_Impianti r on r.piva=m.piva And r.sa_cod=md.Sa_Cod And r.APPEZZA=md.Appezza And r.ID_REG=md.Id_Destinazione ")
            strSql.AppendLine(" inner Join #Utenti_Dettagli_CTE ud on ud.CodFisc=m.Username_Creazione ")
            strSql.AppendLine(" where r.cul_cod = 0 And md.Tipo_Destinazione = 0 ")
            strSql.AppendLine(" and Not exists ( ")
            strSql.AppendLine(" Select 1 ")
            strSql.AppendLine(" From Reg_Impianti_Codici ic ")
            strSql.AppendLine(" Where r.PIVA = ic.PIVA ")
            strSql.AppendLine(" And r.SA_COD = ic.sa_cod ")
            strSql.AppendLine(" And r.APPEZZA = ic.appezza ")
            strSql.AppendLine(" And r.ID_REG = ic.Id_Reg ")
            strSql.AppendLine(" And ic.id_cod >= 3000 And ic.id_cod < 4000 ")
            strSql.AppendLine(" ) ")
            'Impianti terreno nudo con destinazione d'uso
            strSql.AppendLine(" UNION")
            strSql.AppendLine(" Select ")
            strSql.AppendLine(" m.piva, m.id_agenda, m.Lav_Cod, m.des_lib, Convert(Date, m.Data_Movimento,120) as data_movimento ")
            strSql.AppendLine(" , -rc.id_cod as Veg_Cod , m.Username_Creazione, ud.nome + ' ' + ud.Cognome as utente ")
            strSql.AppendLine(" From #movimenti_x_Agenda_CTE m ")
            strSql.AppendLine(" inner Join Mov_Destinazioni md on m.id_agenda=md.id_agenda  And m.Id_Mov=md.Id_Mov ")
            strSql.AppendLine(" inner Join Reg_Impianti r on r.piva=m.piva And r.sa_cod=md.Sa_Cod And r.APPEZZA=md.Appezza And r.ID_REG=md.Id_Destinazione ")
            strSql.AppendLine(" inner Join Reg_Impianti_Codici rc on r.piva=rc.piva And r.sa_cod=rc.Sa_Cod And r.APPEZZA=rc.Appezza And r.ID_REG=rc.Id_Reg And Progetto_Cod=0 ")
            strSql.AppendLine(" And rc.id_cod >= 3000 And rc.id_cod < 4000 ")
            strSql.AppendLine(" inner Join #Utenti_Dettagli_CTE ud on ud.CodFisc=m.Username_Creazione ")
            strSql.AppendLine(" where r.cul_cod = 0 And md.Tipo_Destinazione = 0 	) qp ")

            '=====================================================================================================
            ' Generazione tabella #Table_OpMag
            '=====================================================================================================
            strSql.AppendLine(" Select * into #Table_OpMag from ")
            strSql.AppendLine(" ( ")
            strSql.AppendLine(" Select a.piva , a.id_agenda, a.Lav_Cod, a.des_lib, cast(m.Data_Movimento as date) as data_movimento ")
            strSql.AppendLine(" , a.Username_Creazione, ud.nome + ' ' + ud.Cognome as utente ")
            strSql.AppendLine(" From agenda a ")
            strSql.AppendLine(" INNER Join #tutte_le_pive p ON a.Piva = p.Piva ")
            strSql.AppendLine(" inner Join Movimenti m on a.piva=m.piva And a.Id_Agenda=m.Id_Agenda ")
            strSql.AppendLine(" inner Join Mov_Destinazioni md ON m.piva=md.piva And m.id_agenda=md.id_agenda  And m.Id_Mov=md.Id_Mov And md.Tipo_Destinazione = 20 ")
            strSql.AppendLine(" inner Join #Utenti_Dettagli_CTE ud on ud.CodFisc=a.Username_Creazione ")
            AggiungiFiltroData(FiltroData_Operazione1_Registrazione2, DataInizio, DataFine, strSql)
            strSql.AppendLine(" And m.cau_mov = '" + CAU_CARICO + "' ")
            strSql.AppendLine(" AND m.username_creazione <> '" & objParametri_Server.PivaSuperUser & "' ) qp ")

            'Elimino tabella temporanea movimenti_x_Agenda_CTE
            strSql.AppendLine(Elimina_Tabella_Temp("#movimenti_x_Agenda_CTE "))

            '=====================================================================================================
            ' Preparazioni per generazione #Table_Dett
            '=====================================================================================================
            '--- WITH --------------------------------------------------------------------------------------------
            'imprese_cte
            strSql.AppendLine(" ;with imprese_cte as ( ")
            strSql.AppendLine(" Select i.piva, i.rag_soc from imprese i ")
            strSql.AppendLine(" inner Join #tutte_le_pive p on i.piva = p.piva ")
            strSql.AppendLine(" ), ")
            'agenda_cte
            strSql.AppendLine(" agenda_cte as ( ")
            strSql.AppendLine(" Select a.piva, a.Id_Agenda,a.Data_Modifica, a.Username_Modifica ")
            strSql.AppendLine(" From agenda a ")
            strSql.AppendLine(" inner Join #tutte_le_pive p on a.piva = p.piva ")
            strSql.AppendLine(" ), ")
            'movimenti_tutti_CTE
            strSql.AppendLine(" movimenti_tutti_CTE as ( ")
            strSql.AppendLine(" Select m.Id_Agenda, m.piva, m.Data_Movimento, m.Id_Mov, m.Cau_Mov ")
            strSql.AppendLine(" From movimenti m ")
            strSql.AppendLine(" inner Join #tutte_le_pive p on m.PIVA = p.piva ")
            AggiungiFiltroData(FiltroData_Operazione1_Registrazione2, DataInizio, DataFine, strSql)
            strSql.AppendLine(String.Format(" And m.cau_mov IN ('{0}','{1}','{2}','{3}','{4}') ",
                                            CAU_TRATTAMENTO,
                                            CAU_RILIEVO_CAMPO,
                                            CAU_RILIEVO_RACCOLTA,
                                            CAU_LAVORAZIONE,
                                            CAU_CARICO))
            strSql.AppendLine(" AND m.username_creazione <> '" & objParametri_Server.PivaSuperUser & "' ), ")
            'movimenti_Cte
            strSql.AppendLine(" movimenti_Cte as ( ")
            strSql.AppendLine(" Select * From movimenti_tutti_CTE m ")
            strSql.AppendLine(String.Format(" Where m.Cau_Mov In ('{0}','{1}','{2}','{3}') ), ",
                                            CAU_TRATTAMENTO,
                                            CAU_RILIEVO_CAMPO,
                                            CAU_RILIEVO_RACCOLTA,
                                            CAU_LAVORAZIONE))
            'movimenti_magazzino_CTE
            strSql.AppendLine(" movimenti_magazzino_CTE as ( ")
            strSql.AppendLine(" Select * From movimenti_tutti_CTE m ")
            strSql.AppendLine(" Where m.Cau_Mov = '" + CAU_CARICO + "' ), ")
            'coltura_CTE
            strSql.AppendLine(" coltura_CTE as ( ")
            strSql.AppendLine(" Select SpecieVegetali.veg_cod, SpecieVegetali.veg_des,  Reg_Impianti.piva, Reg_Impianti.sa_cod, Reg_Impianti.appezza, Reg_Impianti.id_reg , Reg_Impianti.Sup_Imp ")
            strSql.AppendLine(" From Reg_Impianti ")
            strSql.AppendLine(" inner Join #tutte_le_pive on Reg_Impianti.PIVA = #tutte_le_pive.piva ")
            strSql.AppendLine(" inner Join cultivar  On cultivar.Cul_Cod = Reg_Impianti.CUL_COD ")
            strSql.AppendLine(" inner Join  SpecieVegetali  On SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ")
            strSql.AppendLine(" where Reg_Impianti.CUL_COD <> 0 ")
            strSql.AppendLine(" union ")
            strSql.AppendLine(" Select -Reg_Impianti_Codici.id_cod As veg_cod, Codici_Anagrafe.descrizione As veg_des,  Reg_Impianti.piva, Reg_Impianti.sa_cod, Reg_Impianti.appezza, Reg_Impianti.id_reg , Reg_Impianti.Sup_Imp ")
            strSql.AppendLine(" From Reg_Impianti ")
            strSql.AppendLine(" inner Join #tutte_le_pive on Reg_Impianti.PIVA = #tutte_le_pive.piva ")
            strSql.AppendLine(" inner Join Reg_Impianti_Codici  On Reg_Impianti.PIVA = Reg_Impianti_Codici.PIVA ")
            strSql.AppendLine(" And Reg_Impianti.SA_COD = Reg_Impianti_Codici.sa_cod ")
            strSql.AppendLine(" And Reg_Impianti.APPEZZA = Reg_Impianti_Codici.appezza ")
            strSql.AppendLine(" And Reg_Impianti.ID_REG = Reg_Impianti_Codici.Id_Reg ")
            strSql.AppendLine(" And Reg_Impianti_Codici.id_cod >= 3000 And Reg_Impianti_Codici.id_cod < 4000 ")
            strSql.AppendLine(" inner Join Codici_Anagrafe  On Codici_Anagrafe.codice = Reg_Impianti_Codici.id_cod ")
            strSql.AppendLine(" where Reg_Impianti.CUL_COD = 0 ")
            strSql.AppendLine(" union ")
            strSql.AppendLine(" Select 0 As veg_cod, 'Terreno Nudo' as veg_des,  Reg_Impianti.piva, Reg_Impianti.sa_cod, Reg_Impianti.appezza, Reg_Impianti.id_reg , Reg_Impianti.Sup_Imp ")
            strSql.AppendLine(" From Reg_Impianti ")
            strSql.AppendLine(" inner Join #tutte_le_pive p on Reg_Impianti.PIVA = p.piva ")
            strSql.AppendLine(" Where Not exists( ")
            strSql.AppendLine(" Select 1 ")
            strSql.AppendLine(" From Reg_Impianti_Codici ic ")
            strSql.AppendLine(" inner Join #tutte_le_pive on Reg_Impianti.PIVA = #tutte_le_pive.piva ")
            strSql.AppendLine(" Where Reg_Impianti.PIVA = ic.PIVA ")
            strSql.AppendLine(" And Reg_Impianti.SA_COD = ic.sa_cod ")
            strSql.AppendLine(" And Reg_Impianti.APPEZZA = ic.appezza ")
            strSql.AppendLine(" And Reg_Impianti.ID_REG = ic.Id_Reg ")
            strSql.AppendLine(" And ic.id_cod >= 3000 And ic.id_cod < 4000  ) ")
            strSql.AppendLine(" And  Reg_Impianti.CUL_COD  = 0 ), ")
            'reg_cte
            strSql.AppendLine(" reg_cte as ( ")
            strSql.AppendLine(" Select reg.* ")
            strSql.AppendLine(" from ")
            strSql.AppendLine(" ( ")
            strSql.AppendLine(" Select distinct veg_cod, veg_des, piva , anno ")
            '---#1
            strSql.AppendLine(" from coltura_cte ")
            '---#2
            'strSql.AppendLine(" from ( ")
            'strSql.AppendLine(" Select SpecieVegetali.veg_cod, SpecieVegetali.veg_des, Reg_Impianti.Piva ")
            'strSql.AppendLine(" From Reg_Impianti ")
            'strSql.AppendLine(" inner Join cultivar On cultivar.Cul_Cod = Reg_Impianti.CUL_COD ")
            'strSql.AppendLine(" inner Join SpecieVegetali On SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ")
            'strSql.AppendLine(" inner Join #tutte_le_pive p on Reg_Impianti.PIVA = p.piva ")
            'strSql.AppendLine(" where Reg_Impianti.CUL_COD <> 0 ")
            'strSql.AppendLine(" union ")
            'strSql.AppendLine(" Select -Reg_Impianti_Codici.id_cod As veg_cod, Codici_Anagrafe.descrizione As veg_des,  Reg_Impianti.Piva ")
            'strSql.AppendLine(" From Reg_Impianti ")
            'strSql.AppendLine(" inner Join Reg_Impianti_Codici  On Reg_Impianti.PIVA = Reg_Impianti_Codici.PIVA ")
            'strSql.AppendLine(" inner Join #tutte_le_pive p on Reg_Impianti.PIVA = p.piva ")
            'strSql.AppendLine(" And Reg_Impianti.SA_COD = Reg_Impianti_Codici.sa_cod ")
            'strSql.AppendLine(" And Reg_Impianti.APPEZZA = Reg_Impianti_Codici.appezza ")
            'strSql.AppendLine(" And Reg_Impianti.ID_REG = Reg_Impianti_Codici.Id_Reg ")
            'strSql.AppendLine(" And Reg_Impianti_Codici.id_cod >= 3000 And Reg_Impianti_Codici.id_cod < 4000 ")
            'strSql.AppendLine(" inner Join Codici_Anagrafe  On Codici_Anagrafe.codice = Reg_Impianti_Codici.id_cod ")
            'strSql.AppendLine(" where Reg_Impianti.CUL_COD = 0 ")
            'strSql.AppendLine(" union ")
            'strSql.AppendLine(" Select 0 As veg_cod, 'Terreno Nudo' as veg_des,  Reg_Impianti.Piva ")
            'strSql.AppendLine(" From Reg_Impianti ")
            'strSql.AppendLine(" inner Join #tutte_le_pive p on Reg_Impianti.PIVA = p.piva ")
            'strSql.AppendLine(" Where Not exists( ")
            'strSql.AppendLine(" Select 1 ")
            'strSql.AppendLine(" From Reg_Impianti_Codici ic ")
            'strSql.AppendLine(" Where Reg_Impianti.PIVA = ic.PIVA ")
            'strSql.AppendLine(" And Reg_Impianti.SA_COD = ic.sa_cod ")
            'strSql.AppendLine(" And Reg_Impianti.APPEZZA = ic.appezza ")
            'strSql.AppendLine(" And Reg_Impianti.ID_REG = ic.Id_Reg ")
            'strSql.AppendLine(" And ic.id_cod >= 3000 And ic.id_cod < 4000 ")
            'strSql.AppendLine(" ) ")
            'strSql.AppendLine(" And  Reg_Impianti.CUL_COD  = 0 ) Colture ")
            '---#
            strSql.AppendLine(" inner Join( ")
            strSql.AppendLine(" Select distinct year(movimenti_CTE.data_movimento) as anno ")
            strSql.AppendLine(" From movimenti_CTE ")
            strSql.AppendLine(" ) a on 1=1  ) reg ")
            strSql.AppendLine(" inner Join imprese_cte i on reg.PIVA = i.piva ), ")
            'xspecie_CTE
            strSql.AppendLine(" xspecie_CTE as ( ")
            strSql.AppendLine(" Select veg_cod, veg_des, piva, anno, count(*) As conteggio , ")
            strSql.AppendLine(" coalesce(veg_des,'') + '-' + piva + '-' + convert(nvarchar(4), anno) as chiave ")
            strSql.AppendLine(" from( ")
            strSql.AppendLine(" Select DISTINCT coltura_CTE.veg_cod, coltura_CTE.veg_des, Mov_Destinazioni.Piva, mov_destinazioni.id_agenda, Year(movimenti_CTE.data_movimento) as anno ")
            strSql.AppendLine(" From coltura_CTE ")
            strSql.AppendLine(" inner Join Mov_Destinazioni ")
            strSql.AppendLine(" On coltura_CTE.PIVA=Mov_Destinazioni.piva ")
            strSql.AppendLine(" And coltura_CTE.sa_cod=Mov_Destinazioni.sa_cod ")
            strSql.AppendLine(" And coltura_CTE.appezza=Mov_Destinazioni.appezza ")
            strSql.AppendLine(" And coltura_CTE.id_reg=Mov_Destinazioni.id_destinazione ")
            strSql.AppendLine(" inner Join movimenti_CTE ")
            strSql.AppendLine(" On movimenti_CTE.piva = mov_destinazioni.piva ")
            strSql.AppendLine(" And movimenti_CTE.id_agenda = mov_destinazioni.id_agenda ")
            strSql.AppendLine(" And movimenti_CTE.id_mov = mov_destinazioni.Id_Mov ")
            strSql.AppendLine(" where Mov_Destinazioni.Tipo_Destinazione = 0 ")
            strSql.AppendLine(" ) xSpec1 ")
            strSql.AppendLine(" group by xSpec1.Piva, xspec1.veg_cod, xSpec1.Veg_Des, xSpec1.anno ), ")
            'xspecie_superficie_CTE
            strSql.AppendLine(" xspecie_superficie_CTE as ( ")
            strSql.AppendLine(" Select xSpec2.Veg_Des, xSpec2.Piva, anno, sum(Sup_Imp) As SupTrattata ")
            strSql.AppendLine(" from( ")
            strSql.AppendLine(" Select DISTINCT ")
            strSql.AppendLine(" veg_des, mov_destinazioni.piva, mov_destinazioni.sa_cod, mov_destinazioni.appezza ")
            strSql.AppendLine(" , mov_destinazioni.id_Destinazione as id_Reg, Coltura.Sup_Imp , year(movimenti_CTE.data_movimento) as anno ")
            strSql.AppendLine(" From ")
            strSql.AppendLine(" coltura_cte coltura ")
            strSql.AppendLine(" inner Join Mov_Destinazioni ")
            strSql.AppendLine(" On Coltura.PIVA=mov_destinazioni.piva ")
            strSql.AppendLine(" And Coltura.sa_cod=mov_destinazioni.sa_cod ")
            strSql.AppendLine(" And Coltura.appezza=mov_destinazioni.appezza ")
            strSql.AppendLine(" And Coltura.id_reg=mov_destinazioni.id_destinazione ")
            strSql.AppendLine(" inner Join movimenti_CTE ")
            strSql.AppendLine(" On movimenti_CTE.piva = mov_destinazioni.piva ")
            strSql.AppendLine(" And movimenti_CTE.id_agenda = mov_destinazioni.id_agenda ")
            strSql.AppendLine(" And movimenti_CTE.id_mov = mov_destinazioni.Id_Mov ")
            strSql.AppendLine(" where Mov_Destinazioni.Tipo_Destinazione = 0 ")
            strSql.AppendLine(" ) xSpec2 ")
            strSql.AppendLine(" group by xSpec2.Piva, xSpec2.Veg_Des, xSpec2.anno ), ")
            'xprimaregistrazione_CTE
            strSql.AppendLine(" xprimaregistrazione_CTE as ( ")
            strSql.AppendLine(" Select agenda.piva ")
            strSql.AppendLine(" , year(movimenti_cte.data_movimento) as anno ")
            strSql.AppendLine(" , cast(min(agenda.data_modifica) as date) As PrimaRegistrazione ")
            strSql.AppendLine(" , cast(min(movimenti_cte.data_movimento) as date) As PrimaOperazione ")
            strSql.AppendLine(" , cast(max(movimenti_cte.data_movimento)as date) As UltimaOperazione ")
            strSql.AppendLine(" , veg_des ")
            strSql.AppendLine(" From agenda_cte agenda ")
            strSql.AppendLine(" inner Join movimenti_cte ")
            strSql.AppendLine(" On movimenti_cte.piva = agenda.piva ")
            strSql.AppendLine(" And movimenti_cte.id_agenda = agenda.id_agenda	 ")
            strSql.AppendLine(" inner Join mov_destinazioni ")
            strSql.AppendLine(" On movimenti_cte.piva = mov_destinazioni.piva ")
            strSql.AppendLine(" And movimenti_cte.id_agenda = mov_destinazioni.id_agenda ")
            strSql.AppendLine(" And movimenti_cte.id_mov = mov_destinazioni.Id_Mov ")
            strSql.AppendLine(" inner Join coltura_CTE coltura ")
            strSql.AppendLine(" On Coltura.PIVA=mov_destinazioni.piva ")
            strSql.AppendLine(" And Coltura.sa_cod=mov_destinazioni.sa_cod ")
            strSql.AppendLine(" And Coltura.appezza=mov_destinazioni.appezza ")
            strSql.AppendLine(" And Coltura.id_reg=mov_destinazioni.id_destinazione ")
            strSql.AppendLine(" where  Mov_Destinazioni.Tipo_Destinazione = 0 ")
            strSql.AppendLine(" group by agenda.piva  , Year(movimenti_cte.data_movimento), Veg_Des  ), ")
            'xConteggioUtenti_CTE
            strSql.AppendLine(" xConteggioUtenti_CTE as ( ")
            strSql.AppendLine(" Select piva, anno, veg_des, count(*) As conteggio ")
            strSql.AppendLine(" from( ")
            strSql.AppendLine(" Select distinct agenda.piva, Year(movimenti_cte.data_movimento) As anno, agenda.Username_Modifica, veg_des ")
            strSql.AppendLine(" From agenda_cte agenda ")
            strSql.AppendLine(" inner Join movimenti_cte ")
            strSql.AppendLine(" On movimenti_cte.piva = agenda.piva ")
            strSql.AppendLine(" And movimenti_cte.id_agenda = agenda.id_agenda	 ")
            strSql.AppendLine(" inner Join mov_destinazioni ")
            strSql.AppendLine(" On movimenti_cte.piva = mov_destinazioni.piva ")
            strSql.AppendLine(" And movimenti_cte.id_agenda = mov_destinazioni.id_agenda ")
            strSql.AppendLine(" And movimenti_cte.id_mov = mov_destinazioni.Id_Mov ")
            strSql.AppendLine(" inner Join coltura_CTE coltura ")
            strSql.AppendLine(" On Coltura.PIVA=mov_destinazioni.piva ")
            strSql.AppendLine(" And Coltura.sa_cod=mov_destinazioni.sa_cod ")
            strSql.AppendLine(" And Coltura.appezza=mov_destinazioni.appezza ")
            strSql.AppendLine(" And Coltura.id_reg=mov_destinazioni.id_destinazione ")
            strSql.AppendLine(" where Mov_Destinazioni.Tipo_Destinazione = 0 ) xUtenti1 ")
            strSql.AppendLine(" group by piva , anno, veg_des ), ")
            'xMagazzino_CTE
            strSql.AppendLine(" xMagazzino_CTE as ( ")
            strSql.AppendLine(" Select agenda.piva, year(movimenti_magazzino_CTE.data_movimento) As anno, count(*) As conteggio ")
            strSql.AppendLine(" From agenda_cte agenda ")
            strSql.AppendLine(" INNER Join movimenti_magazzino_CTE ")
            strSql.AppendLine(" On Agenda.PIVA = movimenti_magazzino_CTE.PIVA ")
            strSql.AppendLine(" And Agenda.Id_Agenda = movimenti_magazzino_CTE.Id_Agenda ")
            strSql.AppendLine(" group by agenda.piva, Year(movimenti_magazzino_CTE.data_movimento)  ), ")
            strSql.AppendLine(" xConteggioUtenti_Magazzino_CTE as (")
            strSql.AppendLine(" Select piva, anno, count(*) As conteggio ")
            strSql.AppendLine(" from ( ")
            strSql.AppendLine(" Select distinct agenda.piva, Year(movimenti_magazzino_CTE.data_movimento) As anno, agenda.Username_Modifica ")
            strSql.AppendLine(" From agenda_cte agenda ")
            strSql.AppendLine(" inner Join movimenti_magazzino_CTE ")
            strSql.AppendLine(" On movimenti_magazzino_CTE.piva = agenda.piva ")
            strSql.AppendLine(" And movimenti_magazzino_CTE.id_agenda = agenda.id_agenda	")
            strSql.AppendLine(" ) xUtenti1 ")
            strSql.AppendLine(" group by piva, anno ), ")
            'xprimaRegistrazione_Magazzino_CTE
            strSql.AppendLine(" xprimaRegistrazione_Magazzino_CTE as ( ")
            strSql.AppendLine(" Select agenda.piva ")
            strSql.AppendLine(" , year(movimenti_magazzino_CTE.data_movimento) as anno ")
            strSql.AppendLine(" , cast(min(agenda.data_modifica) as date) As PrimaRegistrazione ")
            strSql.AppendLine(" , cast(min(movimenti_magazzino_CTE.data_movimento) as date) As PrimaOperazione ")
            strSql.AppendLine(" , cast(max(movimenti_magazzino_CTE.data_movimento) as date) As UltimaOperazione ")
            strSql.AppendLine(" From agenda_cte agenda ")
            strSql.AppendLine(" inner Join movimenti_magazzino_CTE ")
            strSql.AppendLine(" On movimenti_magazzino_CTE.piva = agenda.piva ")
            strSql.AppendLine(" And movimenti_magazzino_CTE.id_agenda = agenda.id_agenda	 ")
            strSql.AppendLine(" group by agenda.piva, Year(movimenti_magazzino_CTE.data_movimento) ) ")

            '=====================================================================================================
            ' Generazione tabella #Table_Dett
            '=====================================================================================================
            strSql.AppendLine(" Select * into #Table_Dett from ")
            strSql.AppendLine(" ( ")
            'Operazioni Campagna
            strSql.AppendLine(" Select ")
            strSql.AppendLine("   ISNULL(Lista_Regioni.regione_des, '') as Regione ")
            strSql.AppendLine(" , ISNULL(istat.COMUNI_PROV, '') as Provincia ")
            strSql.AppendLine(" , ISNULL(istat.LOCALITA, '') as Comune ")
            strSql.AppendLine(" , imprese.rag_soc As Ragione_Sociale, imprese.Piva ")
            strSql.AppendLine(" , ISNULL(CUAA.Val_cod, '') as Cuaa ")
            strSql.AppendLine(" , ISNULL(GerarchiaImprese.Padre, ' ') as Referente ")
            strSql.AppendLine(" , ISNULL(( Rappresentante_Legale.Cognome + ' ' + Rappresentante_Legale.Nome ), ' ') as Rappresentante_Legale ")
            strSql.AppendLine(" , convert(varchar,xPrimaRegistrazione.PrimaOperazione,103) as DataPrimaOperazione ")
            strSql.AppendLine(" , convert(varchar,xPrimaRegistrazione.UltimaOperazione,103) as DataUltimaOperazione ")
            strSql.AppendLine(" , xSpecie.Veg_Des as Uso ")
            strSql.AppendLine(" , xSpecie.Veg_Cod ")
            strSql.AppendLine(" , xSpecie.conteggio as N_Operazioni ")
            strSql.AppendLine(" , xSpecieSuperficie.SupTrattata ")
            strSql.AppendLine(" , convert(varchar,xPrimaRegistrazione.PrimaRegistrazione,103) as Data_Prima_Registrazione ")
            strSql.AppendLine(" , xConteggioUtenti.conteggio as N_Utenti ")
            strSql.AppendLine(" From imprese_cte imprese ")
            strSql.AppendLine(" CROSS APPLY(SELECT TOP 1 * FROM ImpresexIndirizzi WHERE Imprese.PIVA = ImpresexIndirizzi.PIVA) ii ")
            strSql.AppendLine(" INNER Join Indirizzi ON ii.cod_indirizzo = Indirizzi.cod_indirizzo ")
            strSql.AppendLine(" Left Join ISTAT ON ISTAT.PROV = Indirizzi.pro_cod_istat And ISTAT.COM = Indirizzi.com_cod_istat ")
            strSql.AppendLine(" Left Join Lista_Province ON ISTAT.PROV = Lista_Province.PROV ")
            strSql.AppendLine(" Left Join Lista_Regioni ON Lista_Province.REG = Lista_Regioni.Reg ")
            strSql.AppendLine(" Left Join Imprese_Codici CUAA ON Imprese.Piva = CUAA.Piva And CUAA.ID_Cod = 1010 ")
            strSql.AppendLine(" CROSS APPLY(SELECT TOP 1 GerarchiaImprese.Padre FROM GerarchiaImprese WHERE Imprese.PIVA = GerarchiaImprese.figlio) GerarchiaImprese ")
            strSql.AppendLine(" CROSS APPLY(SELECT TOP 1 Contatti.Nome, Contatti.Cognome ")
            strSql.AppendLine("             From Risorse_Umane ")
            strSql.AppendLine("             INNER Join Contatti On Risorse_Umane.cod_contatto = Contatti.Cod_Contatto And contatti.piva=imprese.piva ")
            strSql.AppendLine("             WHERE Risorse_Umane.Cod_Rapporto = -1 And Risorse_Umane.Piva = Imprese.Piva ) Rappresentante_Legale ")
            strSql.AppendLine(" inner Join reg_cte reg on reg.PIVA = imprese.PIVA ")
            strSql.AppendLine(" inner Join xspecie_CTE xspecie On xSpecie.piva = Imprese.piva ")
            strSql.AppendLine("                               And xSpecie.Veg_Des = reg.Veg_Des ")
            strSql.AppendLine("                               And xSpecie.anno = reg.anno ")
            strSql.AppendLine(" inner Join xspecie_superficie_CTE xSpecieSuperficie On xSpecieSuperficie.piva = Imprese.piva ")
            strSql.AppendLine("                                                    And xSpecieSuperficie.Veg_Des = reg.Veg_Des ")
            strSql.AppendLine("                                                    And xSpecieSuperficie.anno = reg.anno ")
            strSql.AppendLine(" inner Join xprimaregistrazione_CTE xPrimaRegistrazione On xPrimaRegistrazione.piva = Imprese.piva ")
            strSql.AppendLine("                                                       And xPrimaRegistrazione.Veg_Des = reg.Veg_Des ")
            strSql.AppendLine("                                                       And xPrimaRegistrazione.anno = reg.anno ")
            strSql.AppendLine(" inner Join xconteggioutenti_Cte xConteggioUtenti On xConteggioUtenti.piva = Imprese.piva ")
            strSql.AppendLine("                                                 And xConteggioUtenti.Veg_Des = reg.Veg_Des ")
            strSql.AppendLine("                                                 And xConteggioUtenti.anno = reg.anno ")
            'Operazioni Magazzino
            strSql.AppendLine(" UNION ")
            strSql.AppendLine(" Select ")
            strSql.AppendLine("   ISNULL(regione_des, '') as Regione ")
            strSql.AppendLine(" , ISNULL(COMUNI_PROV, '') as Provincia ")
            strSql.AppendLine(" , ISNULL(LOCALITA, '') as Comune ")
            strSql.AppendLine(" , imprese.rag_soc As Ragione_Sociale, imprese.Piva ")
            strSql.AppendLine(" , ISNULL(CUAA, '') as Cuaa ")
            strSql.AppendLine(" , ISNULL(Padre, ' ') as Referente ")
            strSql.AppendLine(" , ISNULL(( Cognome + ' ' + Nome ), ' ') as Rappresentante_Legale ")
            strSql.AppendLine(" , convert(varchar,xPrimaRegistrazione.PrimaOperazione,103) as DataPrimaOperazione ")
            strSql.AppendLine(" , convert(varchar,xPrimaRegistrazione.UltimaOperazione,103) as DataUltimaOperazione ")
            strSql.AppendLine(" , ' Movimenti Magazzino' as Uso ")
            strSql.AppendLine(" , -1 as Veg_Cod ")
            strSql.AppendLine(" , xMagazzino.conteggio as N_Operazioni ")
            strSql.AppendLine(" , 0 as SupTrattata ")
            strSql.AppendLine(" , convert(varchar,xPrimaRegistrazione.PrimaRegistrazione,103) as Data_Prima_Registrazione ")
            strSql.AppendLine(" , xConteggioUtenti.conteggio as N_Utenti ")
            strSql.AppendLine(" From( ")
            strSql.AppendLine(" Select imprese.piva , imprese.rag_soc, anno, Lista_Regioni.regione_Des, istat.COMUNI_PROV, istat.LOCALITA, CUAA.val_cod As CUAA, GerarchiaImprese.Padre, Rappresentante_Legale.Nome, Rappresentante_Legale.Cognome ")
            strSql.AppendLine(" From imprese_cte imprese ")
            strSql.AppendLine(" CROSS APPLY(SELECT TOP 1 * FROM ImpresexIndirizzi WHERE Imprese.PIVA = ImpresexIndirizzi.PIVA) ii ")
            strSql.AppendLine(" INNER Join Indirizzi ON ii.cod_indirizzo = Indirizzi.cod_indirizzo ")
            strSql.AppendLine(" Left Join ISTAT ON ISTAT.PROV = Indirizzi.pro_cod_istat And ISTAT.COM = Indirizzi.com_cod_istat ")
            strSql.AppendLine(" Left Join Lista_Province ON ISTAT.PROV = Lista_Province.PROV ")
            strSql.AppendLine(" Left Join Lista_Regioni ON Lista_Province.REG = Lista_Regioni.Reg ")
            strSql.AppendLine(" Left Join Imprese_Codici CUAA ON Imprese.Piva = CUAA.Piva And CUAA.ID_Cod = 1010 ")
            strSql.AppendLine(" CROSS APPLY(SELECT TOP 1 GerarchiaImprese.Padre FROM GerarchiaImprese WHERE Imprese.PIVA = GerarchiaImprese.figlio) GerarchiaImprese ")
            strSql.AppendLine(" CROSS APPLY(SELECT TOP 1 Contatti.Nome, Contatti.Cognome ")
            strSql.AppendLine("             From Risorse_Umane ")
            strSql.AppendLine("             INNER Join Contatti On Risorse_Umane.cod_contatto = Contatti.Cod_Contatto And contatti.piva=imprese.piva ")
            strSql.AppendLine("             WHERE Risorse_Umane.Cod_Rapporto = -1 And Risorse_Umane.Piva = Imprese.Piva ) Rappresentante_Legale ")
            strSql.AppendLine(" inner Join ( ")
            strSql.AppendLine("              Select distinct year(movimenti_CTE.data_movimento) as anno ")
            strSql.AppendLine("              From movimenti_CTE) a On 1=1 ")
            strSql.AppendLine("            ) imprese ")
            strSql.AppendLine(" inner Join xMagazzino_CTE xMagazzino On xMagazzino.piva = Imprese.piva ")
            strSql.AppendLine("                                     And xMagazzino.anno = Imprese.anno ")
            strSql.AppendLine(" inner Join xConteggioUtenti_Magazzino_CTE xConteggioUtenti On xConteggioUtenti.piva = imprese.piva ")
            strSql.AppendLine("                                                           And xConteggioUtenti.anno = imprese.anno ")
            strSql.AppendLine(" inner Join xprimaRegistrazione_Magazzino_CTE xPrimaRegistrazione On xPrimaRegistrazione.PIVA = Imprese.piva ")
            strSql.AppendLine("                                                                 And xPrimaRegistrazione.anno = Imprese.anno ) qp")
            'Aggiunta colonne
            AggiungiColonneTableDett(strSql)
            'Aggiorna elenco utenti operazioni magazzino
            UpdateUtentiOpMagazzino(strSql)
            'Aggiorna elenco utenti operazioni campagna
            UpdateUtentiOpCampagna(strSql)
            'Eliminazione tabelle
            strSql.AppendLine(Elimina_Tabella_Temp("#Table_OpCamp"))
            strSql.AppendLine(Elimina_Tabella_Temp("#Table_OpMag"))
            strSql.AppendLine(Elimina_Tabella_Temp("#Utenti_Dettagli_CTE"))
            strSql.AppendLine(Elimina_Tabella_Temp("#Utenti_Visibilita_Appoggio_CTE"))
            '-----------------------------------------------------------------------------------------------------
            'Query definitiva
            '-----------------------------------------------------------------------------------------------------
            strSql.AppendLine(" Select * from #Table_Dett ")
            strSql.AppendLine(" ORDER BY Ragione_Sociale, DataPrimaOperazione desc, Uso asc ")
            '-----------------------------------------------------------------------------------------------------
            'Eliminazione tabelle
            strSql.AppendLine(Elimina_Tabella_Temp("#Table_Dett"))
            strSql.AppendLine(Elimina_Tabella_Temp("#Pive"))
            strSql.AppendLine(Elimina_Tabella_Temp("#tutte_le_pive"))
            '=====================================================================================================
            dt = EseguiQuery_Lettura(objParametri_Server, strSql.ToString, nomeRoutine)
            '=====================================================================================================
        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Private Sub AggiungiFiltroData(ByVal FiltroData_Operazione1_Registrazione2 As Integer,
                                   ByVal DataInizio As Date,
                                   ByVal DataFine As Date,
                                   ByRef strSql As StringBuilder)
        Select Case FiltroData_Operazione1_Registrazione2

            Case 1
                strSql.AppendLine(" WHERE m.data_movimento <= " & Agro_SQL_SaveDate(DataFine) & " ")
                strSql.AppendLine(" And   m.data_movimento >= " & Agro_SQL_SaveDate(DataInizio) & " ")

            Case Else
                strSql.AppendLine(" WHERE m.data_creazione <= " & Agro_SQL_SaveDate(DataFine) & " ")
                strSql.AppendLine(" And   m.data_creazione >= " & Agro_SQL_SaveDate(DataInizio) & " ")

        End Select

    End Sub

    Private Sub AggiungiColonneTableDett(ByRef strSql As StringBuilder)
        strSql.AppendLine(" alter table #table_Dett add Utenti VARCHAR(MAX) Default '' ")
        strSql.AppendLine(" alter table #table_Dett alter column DataPrimaOperazione datetime ")
        strSql.AppendLine(" alter table #table_Dett alter column DataUltimaOperazione datetime ")
        strSql.AppendLine(" alter table #table_Dett alter column Data_Prima_Registrazione datetime ")
    End Sub

    Private Sub UpdateUtentiOpCampagna(ByRef strSql As StringBuilder)
        strSql.AppendLine(" UPDATE td ")
        strSql.AppendLine(" SET Utenti=( ")
        strSql.AppendLine(" STUFF(( ")
        strSql.AppendLine(" Select DISTINCT ',' + c.utente FROM #table_opCamp c ")
        strSql.AppendLine(" WHERE c.piva = td.piva And c.veg_cod = td.veg_cod And c.data_movimento <= td.dataultimaoperazione And c.data_movimento >= td.DataPrimaOperazione ")
        strSql.AppendLine(" For Xml PATH('') ")
        strSql.AppendLine(" ), 1, 1, '' ) ")
        strSql.AppendLine(" ) ")
        strSql.AppendLine(" FROM #Table_Dett td ")
        strSql.AppendLine(" WHERE Veg_Cod <> -1 ")
    End Sub

    Private Sub UpdateUtentiOpMagazzino(ByRef strSql As StringBuilder)
        strSql.AppendLine(" UPDATE td ")
        strSql.AppendLine(" SET Utenti=( ")
        strSql.AppendLine(" STUFF(( ")
        strSql.AppendLine(" Select DISTINCT ',' + m.utente FROM #Table_OpMag m ")
        strSql.AppendLine(" WHERE m.piva = td.piva And m.data_movimento <= td.dataultimaoperazione And m.data_movimento >= td.DataPrimaOperazione ")
        strSql.AppendLine(" For Xml PATH('') ")
        strSql.AppendLine(" ), 1, 1, '' ) ")
        strSql.AppendLine(" ) ")
        strSql.AppendLine(" FROM #Table_Dett td ")
        strSql.AppendLine(" WHERE Veg_Cod = -1 ")
    End Sub

    Private Function Crea_TabellaPive(ByVal FiltroAggiuntivo_Imprese As String) As String

        Dim strSql As New StringBuilder
        strSql.AppendLine(" ;WITH Pive_CTE as ( ")
        strSql.AppendLine(" SELECT Piva FROM Imprese WHERE Piva IN ( " + Agro_SQL_Save_Clausola_IN(FiltroAggiuntivo_Imprese, True) + " ) )")
        strSql.AppendLine(" SELECT * INTO #Pive From Pive_CTE ")
        Return strSql.ToString()

    End Function

    Private Function Elimina_Tabella_Temp(ByVal nomeTabella As String) As String
        Dim stb As New StringBuilder

        stb.AppendLine(" IF NOT OBJECT_ID('tempdb.dbo." + nomeTabella + "') IS NULL BEGIN ")
        stb.AppendLine("    DROP TABLE " + nomeTabella)
        stb.AppendLine(" END ")

        Return stb.ToString()
    End Function

End Class
