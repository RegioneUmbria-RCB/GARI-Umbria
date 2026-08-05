Imports System.Data
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.DataProviderExtensions

Public Class MenuBS_2017_Operazioni_DB_R
    Inherits AgronicaCoreDataProvider.DataProvider

    'controllo attivazione nuovo menu
    Public Function MenuAttivo(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim DTConfigSiti = objConfigSiti.Leggi(0, "MenuBS_2017", "", "", objParametri)
        Return Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 AndAlso DTConfigSiti.Rows(0)("valore").ToString.ToLower = "true"
    End Function

    Public Function LeggiIDSezione(ByVal sitoRichiesto As Integer, ByVal paginaRichiesta As Integer,
                                   ByRef objparametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer
        Dim NomeRoutine As String = "MenuBS_2017_Operazioni_DB_R.LeggiIDSezione"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim idSezione As Integer = 0
        Try

            Dim NomeDB_Utenti As String
            NomeDB_Utenti = objParametri_Utenti.StringaConnessione.Split(";")(2)
            NomeDB_Utenti = NomeDB_Utenti.Split("=")(1)

            Dim isSuperUser As Boolean = (objparametri_Server.SuperUserUsername = objparametri_Server.UtenteUsername)

            Stb.AppendLine("SELECT s.IDSezione")
            Stb.AppendLine("FROM MenuBS_2017_Sezioni s ")
            Stb.AppendLine("LEFT JOIN MenuBS_2017_Sezioni p ON s.IDSezionePadre = p.IDSezione ")

            If Not isSuperUser Then
                Stb.AppendLine("LEFT JOIN (  ")
                Stb.AppendLine("  SELECT DISTINCT Id_Attivita*(1-Id_Operazione) AS Id_Attivita")
                Stb.AppendLine("  FROM " & NomeDB_Utenti & ".dbo.Utenti_Permessi  ")
                Stb.AppendLine("  WHERE UserName = '" & objparametri_Server.UtenteUsername & "' ")
                Stb.AppendLine(") u ON u.Id_Attivita = s.id_attivita")
            End If

            Stb.AppendLine("WHERE s.Pubblica = 1")
            Stb.AppendLine(" AND (s.PivaSuperUser IS NULL OR s.PivaSuperUser = " & Agro_SQL_SaveText_NULL(objparametri_Server.PivaSuperUser) & ")")
            Stb.AppendLine(" AND s.IDSezionePadre IS NOT NULL ")
            Stb.AppendLine(" AND s.IDTipoSezione = 2 ")

            If Not isSuperUser Then
                Stb.AppendLine(" AND u.Id_Attivita is not null or s.id_attivita = 0 ")
            End If
            Stb.AppendLine("AND s.Enum_SiteRedirector = " & sitoRichiesto)
            Stb.AppendLine("AND s.PaginaRichiesta = " & paginaRichiesta)

            'esecuzione della query
            DT = EseguiQuery_Lettura(objparametri_Server, Stb.ToString, NomeRoutine)

            If DT.Rows.Count > 0 Then
                idSezione = DT.Rows(0)("IDSezione")
            End If
        Catch ex As Exception
            MessaggioErrore = ex.Message & " QUERY: " & Stb.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objparametri_Server, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return idSezione
    End Function


    Public Function LeggiSezioniMenu(ByVal IDSezionePadre As Integer, ByVal Ricerca As String,
                                     ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                     ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "MenuBS_2017_Operazioni_DB_R.LeggiSezioniMenu"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Dim NomeDB_Utenti As String
            NomeDB_Utenti = objParametri_Utenti.StringaConnessione.Split(";")(2)
            NomeDB_Utenti = NomeDB_Utenti.Split("=")(1)

            Dim isSuperUser As Boolean = (objParametri_Server.SuperUserUsername = objParametri_Server.UtenteUsername)

            Stb.AppendLine("SELECT s.*, p.Testo AS Padre ")
            Stb.AppendLine("FROM MenuBS_2017_Sezioni s ")
            Stb.AppendLine("LEFT JOIN MenuBS_2017_Sezioni p ON s.IDSezionePadre = p.IDSezione ")

            If Not isSuperUser Then
                Stb.AppendLine("LEFT JOIN (  ")
                Stb.AppendLine("  SELECT DISTINCT Id_Attivita*(1-Id_Operazione) AS Id_Attivita")
                Stb.AppendLine("  FROM " & NomeDB_Utenti & ".dbo.Utenti_Permessi  ")
                Stb.AppendLine("  WHERE UserName = '" & objParametri_Server.UtenteUsername & "' ")
                Stb.AppendLine(") u ON u.Id_Attivita = s.id_attivita")
            End If

            Stb.AppendLine("WHERE s.Pubblica = 1")
            Stb.AppendLine(" AND (s.PivaSuperUser IS NULL OR s.PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri_Server.PivaSuperUser) & ")")
            Stb.AppendLine(" AND s.IDSezionePadre IS NOT NULL ")
            Stb.AppendLine(" AND s.IDTipoSezione = 2 ")

            If Not isSuperUser Then
                Stb.AppendLine(" AND u.Id_Attivita is not null or s.id_attivita = 0 ")
            End If

            If IDSezionePadre <> 0 Then
                Stb.AppendLine(" AND s.IDSezionePadre = " & IDSezionePadre)
            End If

            If Not String.IsNullOrEmpty(Ricerca) Then
                Stb.AppendLine(" AND s.Testo LIKE '%" & Ricerca & "%'")
            End If

            Stb.AppendLine(" ORDER BY p.Ordinamento, s.Ordinamento")

            'esecuzione della query
            DT = EseguiQuery_Lettura(objParametri_Server, Stb.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message & " QUERY: " & Stb.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function


    '------------------------------------------------------------
    '------------------------DASHBOARD---------------------------
    '------------------------------------------------------------

    'lettura delle sezioni che compongono il menu della dashboard
    Public Function letturaMenu(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "MenuBS_2017_Operazioni_DB_R.letturaMenu"

        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            strSQL.Length = 0

            strSQL.AppendLine("SELECT IDPulsante_Menu, Colore, id_html, Testo, ClasseCSS, Pubblica, IDLingua")
            strSQL.AppendLine("FROM MenuBS_2017_PulsantiMenu")
            strSQL.AppendLine("WHERE Pubblica = 1")
            strSQL.AppendLine(" AND (PivaSuperUser IS NULL OR PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser) & ")")
            strSQL.AppendLine("ORDER BY IDPulsante_Menu")

            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message & " QUERY: " & strSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    'lettura di una specifica sezione dato l'id
    Public Function LeggiSezione(ByVal IDSezione As Integer,
                                 ByVal IDTipoSezione As Integer,
                                 ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                 ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                 ) As DataTable

        Dim NomeRoutine As String = "MenuBS_2017_Operazioni_DB_R.leggiSezione"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Dim NomeDB_Utenti As String
            NomeDB_Utenti = objParametri_Utenti.StringaConnessione.Split(";")(2)
            NomeDB_Utenti = NomeDB_Utenti.Split("=")(1)

            Dim isSuperUser As Boolean = (objParametri_Server.SuperUserUsername = objParametri_Server.UtenteUsername)

            Stb.Length = 0
            Stb.AppendLine("  Select ")
            Stb.AppendLine("   IDSezione ")
            Stb.AppendLine(" , Testo ")
            Stb.AppendLine(" , id_html ")
            Stb.AppendLine(" , inviato ")
            Stb.AppendLine(" , datainvio ")
            Stb.AppendLine(" , Data_Creazione ")
            Stb.AppendLine(" , Data_Modifica ")
            Stb.AppendLine(" , Username_Creazione ")
            Stb.AppendLine(" , Username_Modifica ")
            Stb.AppendLine(" , Validita_Inizio ")
            Stb.AppendLine(" , Validita_Fine ")
            Stb.AppendLine(" , Pubblica ")
            Stb.AppendLine(" , IDLingua ")
            Stb.AppendLine(" , PivaSuperUser ")
            Stb.AppendLine(" , Piva ")
            Stb.AppendLine(" , Colonna_Bootstrap ")
            Stb.AppendLine(" , Colore ")
            Stb.AppendLine(" , IDSezionePadre ")
            Stb.AppendLine(" , S.ID_Attivita ")
            Stb.AppendLine(" , IDTipoSezione ")
            Stb.AppendLine(" , Ordinamento ")
            Stb.AppendLine(" , RichiedeAziendaSelezionata ")
            Stb.AppendLine(" , Enum_SiteRedirector ")
            Stb.AppendLine(" , PaginaRichiesta ")
            Stb.AppendLine(" , RedirectURL ")
            Stb.AppendLine(" , Enum_TipoAperturaPagina ")
            Stb.AppendLine(" , Configurazione ")

            If isSuperUser Then
                Stb.AppendLine("  , ClasseCSS ")
            Else
                Stb.AppendLine("  , case when u.Id_Attivita is not null or s.id_attivita = 0   then ClasseCSS  else ClasseCSS + ' disattivato' end as ClasseCSS ")
            End If

            Stb.AppendLine("FROM MenuBS_2017_Sezioni s")

            If Not isSuperUser Then

                'in distinct senza distinzione fra lettura e scrittura..

                Stb.AppendLine("  Left Join (  ")
                Stb.AppendLine("  ")
                Stb.AppendLine("     Select distinct Id_Attivita*(1-Id_Operazione) As Id_Attivita ")
                Stb.AppendLine("     From " & NomeDB_Utenti & ".dbo.Utenti_Permessi  ")
                Stb.AppendLine("  Where UserName = '" & objParametri_Server.UtenteUsername & "' ")
                Stb.AppendLine("  ) u on u.Id_Attivita = s.id_attivita")

            End If

            'valida
            Stb.AppendLine("WHERE 1 = 1")
            'filtro pivasuperuser
            Stb.AppendLine(" AND (PivaSuperUser IS NULL OR PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri_Server.PivaSuperUser) & ")")
            'indice della sezione
            Stb.AppendLine(" AND IDSezione = " & IDSezione)
            'per sicurezza dico che non voglio macrosezioni
            Stb.AppendLine(" AND  s.IDTipoSezione = " & IDTipoSezione)

            'esecuzione della query
            DT = EseguiQuery_Lettura(objParametri_Server, Stb.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message & " QUERY: " & Stb.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function

    Public Function LeggiIconaPerBreadCrum(
                                 ByVal Testo As String,
                                 ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                 ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                 ) As DataTable
        Dim NomeRoutine As String = "MenuBS_2017_Operazioni_DB_R.LeggiIconaPerBreadCrum"

        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        'idTipo 1 macrocategorie, idTipo2 sottocattegoria
        Dim IDTipoSezioneMacrocategoria As Integer = 1
        Dim IDTipoSezioneSottocategoria As Integer = 2
        Dim NomeDB_Utenti As String
        Dim isSuperUser As Boolean = (objParametri_Server.SuperUserUsername = objParametri_Server.UtenteUsername)

        Dim campiDaPredere = "IDSezione, ClasseCssIcona, ColoreAlternativo"

        Try

            NomeDB_Utenti = objParametri_Utenti.StringaConnessione.Split(";")(2)
            NomeDB_Utenti = NomeDB_Utenti.Split("=")(1)


            'Inizio query macrocategorie
            Stb.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")
            Stb.AppendLine(" Select " & campiDaPredere)
            Stb.AppendLine(" From MenuBS_2017_Sezioni s ")
            Stb.AppendLine(" WHERE s.testo = '" & Agro_SQL_SaveText(Testo) & "'")



            'esecuzione della query
            DT = EseguiQuery_Lettura(objParametri_Server, Stb.ToString, NomeRoutine)

        Catch ex As Exception
            Dim MessaggioErrore As String = ""
            MessaggioErrore = ex.Message & " QUERY: " & Stb.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT


    End Function

    Public Function LeggiTestoDatoIDSezione(
                                 ByVal IdSezione As Integer,
                                 ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                 ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                 ) As DataTable
        Dim NomeRoutine As String = "MenuBS_2017_Operazioni_DB_R.LeggiIconaPerBreadCrum"

        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim NomeDB_Utenti As String


        Try

            NomeDB_Utenti = objParametri_Utenti.StringaConnessione.Split(";")(2)
            NomeDB_Utenti = NomeDB_Utenti.Split("=")(1)

            Dim isSuperUser As Boolean = (objParametri_Server.SuperUserUsername = objParametri_Server.UtenteUsername)

            Stb.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")
            Stb.AppendLine(" with #figlio AS (select testo, IDSezionePadre from MenuBS_2017_Sezioni 
                where IDSezione = " & IdSezione & ")

                Select testo, IDSezionePadre from MenuBS_2017_Sezioni 
                WHERE IDSezione in (Select IDSezionePadre from #figlio)
                UNION ALL
                SELECT * from #figlio
                ")


            'esecuzione della query
            DT = EseguiQuery_Lettura(objParametri_Server, Stb.ToString, NomeRoutine)

        Catch ex As Exception
            Dim MessaggioErrore As String = ""
            MessaggioErrore = ex.Message & " QUERY: " & Stb.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT


    End Function


    'legge sia le macrocategorie sia i pulsanti in quella macrocategoria
    Public Function LeggiPulsantiPadriFigli(
                                 ByVal NascondiMenu As Boolean,
                                 ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                 ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                 ) As DataTable
        Dim NomeRoutine As String = "MenuBS_2017_Operazioni_DB_R.LeggiPulsantiPadriFigli"


        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        'idTipo 1 macrocategorie, idTipo2 sottocattegoria
        Dim IDTipoSezioneMacrocategoria As Integer = 1
        Dim IDTipoSezioneSottocategoria As Integer = 2
        Dim NomeDB_Utenti As String
        Dim isSuperUser As Boolean = (objParametri_Server.SuperUserUsername = objParametri_Server.UtenteUsername)

        Dim campiDaPredere = "IDSezione 
                         , Testo 
                         , id_html 
                         , inviato 
                         , datainvio 
                         , Data_Creazione 
                         , Data_Modifica 
                         , Username_Creazione 
                         , Username_Modifica 
                         , Validita_Inizio 
                         , Validita_Fine 
                         , Pubblica 
                         , IDLingua 
                         , PivaSuperUser 
                         , Piva 
                         , Colonna_Bootstrap 
                         , Colore 
                         , IDSezionePadre 
                         , S.ID_Attivita 
                         , IDTipoSezione 
                         , Ordinamento 
                         , RichiedeAziendaSelezionata 
                         , Enum_SiteRedirector 
                         , PaginaRichiesta 
                         , RedirectURL 
                         , Enum_TipoAperturaPagina
                         , ColoreAlternativo
                         , ClasseCssIcona"

        Try

            NomeDB_Utenti = objParametri_Utenti.StringaConnessione.Split(";")(2)
            NomeDB_Utenti = NomeDB_Utenti.Split("=")(1)


            'Inizio query macrocategorie
            Stb.Length = 0
            Stb.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
            Stb.AppendLine("  WITH #padri as (")
            Stb.AppendLine("Select ")
            Stb.AppendLine(campiDaPredere)
            Stb.AppendLine("From MenuBS_2017_Sezioni s")
            If Not isSuperUser Then


                Stb.AppendLine(" Left Join (  ")
                Stb.AppendLine(" Select distinct IDSezionePadre As id_sezione, 0 As id_attivita ")
                Stb.AppendLine(" From MenuBS_2017_Sezioni s ")

                Stb.AppendLine("  Left Join (  ")
                Stb.AppendLine("     Select distinct Id_Attivita*(1-Id_Operazione) As Id_Attivita ")
                Stb.AppendLine("     From " & NomeDB_Utenti & ".dbo.Utenti_Permessi  ")
                Stb.AppendLine("  Where UserName = '" & objParametri_Server.UtenteUsername & "' ")
                Stb.AppendLine("  ) u on u.Id_Attivita = s.id_attivita")

                Stb.AppendLine(" Where Pubblica = 1 And s.IDTipoSezione = " & IDTipoSezioneSottocategoria)
                Stb.AppendLine(" And (u.Id_Attivita Is Not null Or s.id_attivita = 0) ")
                Stb.AppendLine(" ) u on u.id_sezione = s.IDSezione ")

            End If
            Stb.AppendLine("WHERE s.Pubblica = 1")
            Stb.AppendLine("And (s.PivaSuperUser IS NULL OR s.PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri_Server.PivaSuperUser) & ")")
            Stb.AppendLine("And  s.IDTipoSezione =  " & IDTipoSezioneMacrocategoria)

            If Not isSuperUser AndAlso NascondiMenu Then
                Stb.AppendLine("And (u.Id_Attivita is not null or s.id_attivita = 0)")
            End If

            Stb.AppendLine(")")

            'Fine query macrocategorie



            'Inizio query sottocattegorie
            Stb.AppendLine("  Select * FROM ( ")
            Stb.AppendLine("  Select ")
            Stb.AppendLine(campiDaPredere)


            Stb.AppendLine("FROM MenuBS_2017_Sezioni s")

            If Not isSuperUser Then

                Stb.AppendLine("  Left Join (  ")
                Stb.AppendLine("     Select distinct Id_Attivita*(1-Id_Operazione) As Id_Attivita ")
                Stb.AppendLine("     From " & NomeDB_Utenti & ".dbo.Utenti_Permessi  ")
                Stb.AppendLine("  Where UserName = '" & objParametri_Server.UtenteUsername & "' ")
                Stb.AppendLine("  ) u on u.Id_Attivita = s.id_attivita")

            End If

            Stb.AppendLine("WHERE Pubblica = 1")

            'filtro pivasuperuser
            Stb.AppendLine(" AND (PivaSuperUser IS NULL OR PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri_Server.PivaSuperUser) & ")")


            Stb.AppendLine(" AND (( s.IDTipoSezione = " & IDTipoSezioneSottocategoria & "  AND  s.IDSezionePadre IN (Select #padri.IDSezione from #Padri))")

            If Not isSuperUser AndAlso NascondiMenu Then
                Stb.AppendLine("And (u.Id_Attivita is not null or s.id_attivita = 0)")
            End If
            'Fine query sottocattegorie

            Stb.AppendLine(") UNION")
            Stb.AppendLine("Select * FROM #padri) as p")

            Stb.AppendLine(" ORDER BY Ordinamento")


            'esecuzione della query
            DT = EseguiQuery_Lettura(objParametri_Server, Stb.ToString, NomeRoutine)

        Catch ex As Exception
            Dim MessaggioErrore As String = ""
            MessaggioErrore = ex.Message & " QUERY: " & Stb.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT


    End Function

    'lettura per pulsanti menu di tipo checklist
    Public Function LeggiPulsantiChecklist(
                                 ByVal NascondiMenu As Boolean,
                                 ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                 ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                 ) As DataTable
        Dim NomeRoutine As String = "MenuBS_2017_Operazioni_DB_R.LeggiPulsantiChecklist"

        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        'idTipo 1 macrocategorie, idTipo2 sottocattegoria
        Dim IDTipoSezioneMacrocategoria As Integer = 1
        Dim IDTipoSezioneSottocategoria As Integer = 2
        Dim NomeDB_Utenti As String
        Dim isSuperUser As Boolean = (objParametri_Server.SuperUserUsername = objParametri_Server.UtenteUsername)

        Dim campiDaPredere = "IDSezione 
                         , Testo 
                         , id_html 
                         , inviato 
                         , datainvio 
                         , Data_Creazione 
                         , Data_Modifica 
                         , Username_Creazione 
                         , Username_Modifica 
                         , Validita_Inizio 
                         , Validita_Fine 
                         , Pubblica 
                         , IDLingua 
                         , PivaSuperUser 
                         , Piva 
                         , Colonna_Bootstrap 
                         , Colore 
                         , IDSezionePadre 
                         , S.ID_Attivita 
                         , IDTipoSezione 
                         , Ordinamento 
                         , RichiedeAziendaSelezionata 
                         , Enum_SiteRedirector 
                         , PaginaRichiesta 
                         , RedirectURL 
                         , Enum_TipoAperturaPagina
                         , ColoreAlternativo
                         , ClasseCssIcona
                         , isChecklist"
        Try

            Stb.AppendLine("  Select ")
            Stb.AppendLine(campiDaPredere)


            Stb.AppendLine("FROM MenuBS_2017_Sezioni s")

            If Not isSuperUser Then

                Stb.AppendLine("  Left Join (  ")
                Stb.AppendLine("     Select distinct Id_Attivita*(1-Id_Operazione) As Id_Attivita ")
                Stb.AppendLine("     From " & NomeDB_Utenti & ".dbo.Utenti_Permessi  ")
                Stb.AppendLine("  Where UserName = '" & objParametri_Server.UtenteUsername & "' ")
                Stb.AppendLine("  ) u on u.Id_Attivita = s.id_attivita")

            End If

            Stb.AppendLine("WHERE Pubblica = 1")

            'filtro pivasuperuser
            Stb.AppendLine(" AND (PivaSuperUser IS NULL OR PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri_Server.PivaSuperUser) & ")")

            'filtro checklist
            Stb.AppendLine(" AND s.isChecklist = 1 ")

            If Not isSuperUser AndAlso NascondiMenu Then
                Stb.AppendLine("And (u.Id_Attivita is not null or s.id_attivita = 0)")
            End If
            'Fine query sottocattegorie

            'esecuzione della query
            DT = EseguiQuery_Lettura(objParametri_Server, Stb.ToString, NomeRoutine)

        Catch ex As Exception
            Dim MessaggioErrore As String = ""
            MessaggioErrore = ex.Message & " QUERY: " & Stb.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function LeggiUltimeAziendeSelezionate(
                                     ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                     ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "MenuBS_2017_Operazioni_DB_R.LeggiUltimeAziendeSelezionate"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim NomeDB_Utenti As String
        NomeDB_Utenti = objParametri_Utenti.StringaConnessione.Split(";")(2)
        NomeDB_Utenti = NomeDB_Utenti.Split("=")(1)

        Dim NomeDB_Server As String
        NomeDB_Server = objParametri_Server.StringaConnessione.Split(";")(2)
        NomeDB_Server = NomeDB_Server.Split("=")(1)

        Dim isSuperUser As Boolean = (objParametri_Server.SuperUserUsername = objParametri_Server.UtenteUsername)



        Try


            Stb.Length = 0



            'Elimino le aziende che non posso vedere
            Stb.AppendLine("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
            Stb.AppendLine("")
            Stb.AppendLine("if exists(")
            Stb.AppendLine("    select * from " & NomeDB_Utenti & ".dbo.utenti_profili")
            Stb.AppendLine("    where Utente = '" & objParametri_Utenti.UtenteUsername & "'")
            Stb.AppendLine("    AND Descrizione_2 <> ''")
            Stb.AppendLine(")")
            'questo perche Utenti_Visibilita_Appoggio non viene valorizzata se utenti_profili.Descrizione_2 = ''
            'in quel caso visibilita su tutta l azienda
            Stb.AppendLine("")
            Stb.AppendLine("begin")
            Stb.AppendLine("")

            Stb.AppendLine("WITH #daEliminare as (")
            Stb.AppendLine("    Select * from " & NomeDB_Utenti & ".dbo.utenti_navigazione_aziende a")
            Stb.AppendLine("    WHERE a.piva not in (")
            Stb.AppendLine("        Select  piva FROM " & NomeDB_Server & ".dbo.Utenti_Visibilita_Appoggio u")
            Stb.AppendLine("        WHERE u.Username = '" & objParametri_Utenti.UtenteUsername & "'")
            Stb.AppendLine("        AND u.Entita_Cod = 1")
            Stb.AppendLine("    )")
            Stb.AppendLine("    AND a.Username = '" & objParametri_Utenti.UtenteUsername & "'")
            Stb.AppendLine(")")

            Stb.AppendLine("")
            Stb.AppendLine("DELETE FROM " & NomeDB_Utenti & ".dbo.utenti_navigazione_aziende")
            Stb.AppendLine("WHERE Username = '" & objParametri_Utenti.UtenteUsername & "'")
            Stb.AppendLine("AND piva in (Select piva from #daEliminare)")


            Stb.AppendLine("")
            Stb.AppendLine("end;")
            Stb.AppendLine("")


            Stb.AppendLine("SELECT i.rag_soc, u.piva, ic.val_cod as CodiceCUAA")
            Stb.AppendLine("FROM " & NomeDB_Utenti & ".dbo.Utenti_Navigazione_Aziende u")
            Stb.AppendLine("INNER JOIN " & NomeDB_Server & ".dbo.Imprese i ON u.piva = i.piva")
            Stb.AppendLine($"INNER JOIN {NomeDB_Server}.dbo.Imprese_Codici ic ON i.PIVA = ic.PIVA AND ic.id_cod = {CInt(enum_CodiciAnagrafe.CodiceCUAA)}")
            Stb.AppendLine(" ")
            Stb.AppendLine("WHERE u.UserName = '" & objParametri_Utenti.UtenteUsername & "'")
            Stb.AppendLine("ORDER BY u.datainvio DESC")

            'esecuzione della query
            DT = EseguiQuery_Lettura(objParametri_Server, Stb.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message & " QUERY:  " & Stb.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function LeggiSezioni(ByVal IDTipoSezione As Integer,
                                 ByVal IDSezionePadre As Integer,
                                 ByVal Ricerca As String,
                                 ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                 ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                 ) As DataTable

        Dim NomeRoutine As String = "MenuBS_2017_Operazioni_DB_R.LeggiSezioni"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Dim NomeDB_Utenti As String
            NomeDB_Utenti = objParametri_Utenti.StringaConnessione.Split(";")(2)
            NomeDB_Utenti = NomeDB_Utenti.Split("=")(1)

            Dim isSuperUser As Boolean = (objParametri_Server.SuperUserUsername = objParametri_Server.UtenteUsername)

            Stb.Length = 0
            Stb.AppendLine("  Select ")
            Stb.AppendLine("   IDSezione ")
            Stb.AppendLine(" , Testo ")
            Stb.AppendLine(" , id_html ")
            Stb.AppendLine(" , inviato ")
            Stb.AppendLine(" , datainvio ")
            Stb.AppendLine(" , Data_Creazione ")
            Stb.AppendLine(" , Data_Modifica ")
            Stb.AppendLine(" , Username_Creazione ")
            Stb.AppendLine(" , Username_Modifica ")
            Stb.AppendLine(" , Validita_Inizio ")
            Stb.AppendLine(" , Validita_Fine ")
            Stb.AppendLine(" , Pubblica ")
            Stb.AppendLine(" , IDLingua ")
            Stb.AppendLine(" , PivaSuperUser ")
            Stb.AppendLine(" , Piva ")
            Stb.AppendLine(" , Colonna_Bootstrap ")
            Stb.AppendLine(" , Colore ")
            Stb.AppendLine(" , IDSezionePadre ")
            Stb.AppendLine(" , S.ID_Attivita ")
            Stb.AppendLine(" , IDTipoSezione ")
            Stb.AppendLine(" , Ordinamento ")
            Stb.AppendLine(" , RichiedeAziendaSelezionata ")
            Stb.AppendLine(" , Enum_SiteRedirector ")
            Stb.AppendLine(" , PaginaRichiesta ")
            Stb.AppendLine(" , RedirectURL ")
            Stb.AppendLine(" , Enum_TipoAperturaPagina ")

            If isSuperUser Then
                Stb.AppendLine("  , ClasseCSS ")
            Else
                Stb.AppendLine("  , case when u.Id_Attivita is not null or s.id_attivita = 0   then ClasseCSS  else ClasseCSS + ' disattivato' end as ClasseCSS ")
            End If

            Stb.AppendLine("FROM MenuBS_2017_Sezioni s")

            If Not isSuperUser Then

                'in distinct senza distinzione fra lettura e scrittura..

                If IDTipoSezione = 1 Then
                    Stb.AppendLine(" Left Join (  ")
                    Stb.AppendLine(" Select distinct IDSezionePadre As id_sezione, 0 As id_attivita ")
                    Stb.AppendLine(" From MenuBS_2017_Sezioni s ")
                End If

                Stb.AppendLine("  Left Join (  ")
                Stb.AppendLine("     Select distinct Id_Attivita*(1-Id_Operazione) As Id_Attivita ")
                Stb.AppendLine("     From " & NomeDB_Utenti & ".dbo.Utenti_Permessi  ")
                Stb.AppendLine("  Where UserName = '" & objParametri_Server.UtenteUsername & "' ")
                Stb.AppendLine("  ) u on u.Id_Attivita = s.id_attivita")

                If IDTipoSezione = 1 Then
                    Stb.AppendLine(" Where Pubblica = 1 And s.IDTipoSezione = 2 ")
                    Stb.AppendLine(" And (u.Id_Attivita Is Not null Or s.id_attivita = 0) ")
                    Stb.AppendLine(" ) u on u.id_sezione = s.IDSezione ")
                End If

            End If

            Stb.AppendLine("WHERE Pubblica = 1")

            'filtro pivasuperuser
            Stb.AppendLine(" AND (PivaSuperUser IS NULL OR PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri_Server.PivaSuperUser) & ")")

            If IDTipoSezione = 0 And IDSezionePadre = 0 Then
                Stb.AppendLine(" AND s.IDSezionePadre IS NOT NULL")
            End If

            If IDTipoSezione <> 0 Then
                Stb.AppendLine(" AND  s.IDTipoSezione = " & IDTipoSezione)
            End If

            If IDSezionePadre <> 0 Then
                Stb.AppendLine(" AND  s.IDSezionePadre = " & IDSezionePadre)
            End If

            If Not String.IsNullOrEmpty(Ricerca) Then
                Stb.AppendLine(" AND  s.Testo LIKE '%" & Ricerca & "%'")
            End If

            If IDTipoSezione <> 1 And IDSezionePadre = 0 Then
                Stb.AppendLine(" ORDER BY s.IDSezionePadre,s.Ordinamento")
            Else
                Stb.AppendLine(" ORDER BY s.Ordinamento")
            End If

            'esecuzione della query
            DT = EseguiQuery_Lettura(objParametri_Server, Stb.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message & " QUERY: " & Stb.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT


    End Function


    'lettura dei widget degli allarmi
    Public Function leggiWidgetAllarmi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "MenuBS_2017_Operazioni_DB_R.leggiWidgetAllarmi"

        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            strSQL.Length = 0
            strSQL.AppendLine("SELECT IDWidget_Allarme, Colore, id_html, Testo, ClasseCSS, Pubblica, Lingua")
            strSQL.AppendLine("FROM MenuBS_2017_WidgetAllarmi")
            strSQL.AppendLine("WHERE Pubblica = 1")
            strSQL.AppendLine(" AND (PivaSuperUser IS NULL OR PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser) & ")")
            strSQL.AppendLine("ORDER BY IDWidget_Allarme")

            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message & " QUERY: " & strSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

End Class

Public Class MenuBS_2017_Operazione_DB_W
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function inserisciNavigazioneAziendeUtente(piva As String, ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                     ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri, ByVal NUM_DI_OPERAZIONE_DA_TENERE As Integer) As Boolean

        Dim NomeRoutine As String = "MenuBS_2017_Operazione_DB_W.inserisciNavigazioneAziendeUtente()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------
        Dim NomeDB_Utenti As String
        NomeDB_Utenti = objParametri_Utenti.StringaConnessione.Split(";")(2)
        NomeDB_Utenti = NomeDB_Utenti.Split("=")(1)



        Dim username = objParametri_Utenti.UtenteUsername


        Try
            '---------------------------------------------
            StrSQL.Length = 0




            StrSQL.AppendLine(String.Format("if exists
	                (select * from {0}.[dbo].[Utenti_Navigazione_Aziende]
		                where username = '{1}' and piva = '{2}')",
                                            NomeDB_Utenti, username, Agro_SQL_SaveText(piva)))

            'update, nel caso l'azienda sia stata selezionata di recente e sia gia presente il record
            StrSQL.AppendLine(String.Format(
                    "begin
                    UPDATE {0}.[dbo].[Utenti_Navigazione_Aziende]
                    SET datainvio = getdate()
                    WHERE username = '{1}' and piva = '{2}';
                   end", NomeDB_Utenti, username, Agro_SQL_SaveText(piva)))

            'insert, inserisco il record
            StrSQL.AppendLine(String.Format(
                "else
                begin
                    insert into {0}.dbo.Utenti_Navigazione_Aziende (UserName, piva, datainvio) values
                    ('{1}', '{2}', getdate())
                end; ", NomeDB_Utenti, username, Agro_SQL_SaveText(piva)))


            StrSQL.AppendLine(String.Format("
                With #daNonEliminare as (
                    Select TOP {0} *
                    From {2}.dbo.Utenti_Navigazione_Aziende u
                    Where u.UserName = '{1}'
                    ORDER BY datainvio DESC
                )", NUM_DI_OPERAZIONE_DA_TENERE, username, NomeDB_Utenti))


            StrSQL.Append(String.Format("DELETE From {0}.dbo.Utenti_Navigazione_Aziende WHERE UserName = '{1}' AND Piva NOT IN (Select Piva FROM #daNonEliminare) ",
                                        NomeDB_Utenti, username))
            '---------------------------------------------

            xRisp = EseguiQuery_Scrittura(objParametri_Server, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message & " QUERY: " & StrSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function


End Class