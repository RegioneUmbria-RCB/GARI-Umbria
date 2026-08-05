Imports System.Data.Entity
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class Listini_Prezzi_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Piva As String,
                              ByVal Listino_Cod As Integer,
                              ByVal Listino_Classe_Cod As Integer,
                              ByVal Tipo_Classe As Integer,
                              ByVal ChkApplicabilita As Integer,
                              ByVal TipoIva As Integer,
                              ByVal Tipo_Provvigione As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreParametri
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Listini_Prezzi_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT  * ")
            StrSQL.Append(" FROM    Listini_Prezzi lp")
            StrSQL.Append(" INNER JOIN   Listini_Classi_Prezzi lcp ")
            StrSQL.Append(" ON lp.Piva = lcp.Piva ")
            StrSQL.Append(" AND lp.Piva_SuperUser = lcp.Piva_SuperUser ")
            StrSQL.Append(" AND lp.Listino_Classe_Cod = lcp.Listino_Classe_Cod ")
            StrSQL.Append(" WHERE ( lp.Piva_SuperUser = 'AAAAAAAAAAA' OR lp.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ) ")
            StrSQL.Append(" AND lp.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")

            If Listino_Cod <> -1 Then
                StrSQL.Append(" AND lp.Listino_Cod = " & Agro_SQL_SaveNum(Listino_Cod))
            End If

            If Listino_Classe_Cod <> -1 Then
                StrSQL.Append(" AND lp.Listino_Classe_Cod = " & Agro_SQL_SaveNum(Listino_Classe_Cod))
            End If

            If Tipo_Classe <> -1 Then
                StrSQL.Append(" AND lcp.Tipo_Classe = " & Agro_SQL_SaveNum(Tipo_Classe))
            End If


            If ChkApplicabilita <> -1 Then
                StrSQL.Append(" AND lp.ChkApplicabilita = " & Agro_SQL_SaveNum(ChkApplicabilita))
            End If

            If TipoIva <> -1 Then
                StrSQL.Append(" AND lp.Tipo_Iva = " & Agro_SQL_SaveNum(TipoIva))
            End If

            If Tipo_Provvigione <> -1 Then
                StrSQL.Append(" AND lp.Tipo_Provvigione = " & Agro_SQL_SaveNum(Tipo_Provvigione))
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If
            '------------------------------------------------------------------

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


    '##############################################################################################
    Public Function Leggi_Classi_Listini(ByVal piva As String,
                                         ByVal tipo As Integer,
                                         ByVal ricerca As String,
                                         ByVal DataRif As String,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As List(Of Listini_Classi_Prezzi)

        Dim risposta As List(Of Listini_Classi_Prezzi)

        Dim DataRifDateTime As Nullable(Of DateTime)
        DataRifDateTime = Nothing
        If Not String.IsNullOrEmpty(DataRif) Then
            DataRifDateTime = Convert.ToDateTime(DataRif)
        End If

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Listini_Prezzi_R.Leggi_Classi_Listini()"

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim Listini_Classi =
                From list_classi In GiasContext.Listini_Classi_Prezzi
                Where
                    (tipo = 0 OrElse list_classi.Tipo_Classe = tipo) AndAlso
                    list_classi.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                    list_classi.Piva.Equals(piva) AndAlso
                    list_classi.Listino_Classe_Des.Contains(ricerca) AndAlso
                    (DataRifDateTime Is Nothing Or (list_classi.Validita_Inizio <= DataRifDateTime AndAlso list_classi.Validita_Fine >= DataRifDateTime))
                Select list_classi

            risposta = Listini_Classi.OrderBy(Function(x) x.Tipo_Classe).ThenBy(Function(x) x.Listino_Classe_Des).ToList

        End Using

        Return risposta

    End Function

    '##############################################################################################
    Public Function Leggi_Listini(ByVal piva As String,
                                         ByVal tipo As Integer,
                                         ByVal classe As Integer,
                                         ByVal ricerca As String,
                                         ByVal DataRif As String,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As String

        Dim risposta As String = ""

        Dim DataRifDateTime As Nullable(Of DateTime)
        DataRifDateTime = Nothing
        If Not String.IsNullOrEmpty(DataRif) Then
            DataRifDateTime = Convert.ToDateTime(DataRif)
        End If

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Listini_Prezzi_R.Leggi_Listini()"

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim Listini_Prezzi =
                From l In GiasContext.Listini_Prezzi
                Join c In GiasContext.Listini_Classi_Prezzi
                    On c.Piva_SuperUser Equals l.Piva_SuperUser And
                    c.Piva Equals l.Piva And
                    c.Listino_Classe_Cod Equals l.Listino_Classe_Cod
                Where
                    (tipo = 0 OrElse c.Tipo_Classe = tipo) AndAlso
                    (classe = 0 OrElse l.Listino_Classe_Cod = classe) AndAlso
                    (l.Piva_SuperUser.Equals(Piva_SuperUser)) AndAlso
                    (l.Piva.Equals(piva)) AndAlso
                    (l.Listino_Des.Contains(ricerca)) AndAlso
                    (DataRifDateTime Is Nothing Or (l.Validita_Inizio <= DataRifDateTime And l.Validita_Fine >= DataRifDateTime))
                Select c.Tipo_Classe, c.Listino_Classe_Cod, c.Listino_Classe_Des, l.Listino_Cod, l.Listino_Cod_Des, l.Listino_Des, l.Validita_Inizio, l.Validita_Fine, l.ChkApplicabilita

            Dim listini = Listini_Prezzi.OrderBy(Function(x) x.Listino_Classe_Des).ThenBy(Function(x) x.Listino_Des).ToList()
            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(listini, Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function

    '##############################################################################################
    Public Function Leggi_Listini_Prodotti(ByVal piva As String,
                                                  ByVal Listino_Cod As Integer,
                                                  ByRef objParametri As AgronicaCoreParametri
                                                  ) As String

        Dim risposta As String = ""
        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Listini_Prezzi_R.Leggi_Listini_Prodotti()"

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim ListiniPrezziProdotti =
                (From p In GiasContext.Listini_PrezzixRisorse
                 Where p.Piva_SuperUser = Piva_SuperUser AndAlso p.Piva = piva AndAlso p.Listino_Cod = Listino_Cod
                 Join l In GiasContext.Listini_Prezzi On l.Piva_SuperUser Equals p.Piva_SuperUser And l.Piva Equals p.Piva And l.Listino_Cod Equals p.Listino_Cod
                 Join c In GiasContext.CategorieMagazzino On c.Elem_Cod Equals p.Elem_Cod
                 Group Join m In GiasContext.Materie_Prime On m.Mat_Cod Equals p.Mat_Cod Into _m = Group From mp In _m.DefaultIfEmpty()
                 Group Join a In GiasContext.Avversita On a.Av_Cod Equals p.Pro_Cod Into _a = Group From av In _a.DefaultIfEmpty()
                 Group Join f In GiasContext.Fertilizzanti On f.Fer_Cod Equals p.Pro_Cod Into _f = Group From fe In _f.DefaultIfEmpty()
                 Group Join fr In GiasContext.Formulati On fr.Fr_Cod Equals p.Pro_Cod Into _fr = Group From fo In _fr.DefaultIfEmpty()
                 Group Join i In GiasContext.InsettiUtili On i.Ins_Cod Equals p.Pro_Cod Into _i = Group From iu In _i.DefaultIfEmpty()
                 Group Join s In GiasContext.TipologieSementi On s.SEM_COD Equals p.Pro_Cod Into _s = Group From sm In _s.DefaultIfEmpty()
                 Group Join t In GiasContext.Trappole On t.TRAP_COD Equals p.Pro_Cod Into _t = Group From tr In _t.DefaultIfEmpty()
                 Select New With {
                    .KeyListinoProdotto = "",
                    .PrezziPresenti = "",
                    l.Listino_Cod,
                    l.Listino_Cod_Des,
                    l.Listino_Des,
                    p.Elem_Cod,
                    p.Pro_Cod,
                    p.Mat_Cod,
                    .Categoria = c.NomeComune,
                    .Prodotto =
                     If(c.Tabella = "Materie_Prime", mp.Mat_Des,
                     If(c.Tabella = "Avversita", av.Av_Des_Vol,
                     If(c.Tabella = "Fertilizzanti", fe.Fer_Des,
                     If(c.Tabella = "Formulati", fo.Fr_Des,
                     If(c.Tabella = "InsettiUtili", iu.Ins_Des,
                     If(c.Tabella = "TipologieSementi", sm.SEM_DES,
                     If(c.Tabella = "Trappole", tr.TRAP_DES,
                     CStr(p.Pro_Cod)))))))),
                    .Cod_Articolo = If(c.Tabella = "Materie_Prime", mp.Cod_Articolo, ""),
                    .Validita_Inizio = l.Validita_Inizio,
                    .Validita_Fine = l.Validita_Fine
                }).OrderBy(Function(x) x.Categoria).ThenBy(Function(x) x.Prodotto).ToList()

            '.Validita_Inizio = If(p.Validita_Inizio = AGRODATAINIZIO, l.Validita_Inizio, p.Validita_Inizio),
            '.Validita_Fine = If(p.Validita_Fine = AGRODATAFINE, l.Validita_Fine, p.Validita_Fine)

            Dim ListiniPrezziDettagli = (From d In GiasContext.Listini_Prezzi_Dettagli Where d.Piva_SuperUser = Piva_SuperUser AndAlso d.Piva = piva AndAlso d.Listino_Cod = Listino_Cod).ToList

            For Each obj In ListiniPrezziProdotti
                obj.KeyListinoProdotto = piva & "-" & Listino_Cod.ToString() & "-" & obj.Elem_Cod.ToString() & "-" & obj.Pro_Cod.ToString() & "-" & obj.Mat_Cod.ToString()
                obj.PrezziPresenti = If((From p In ListiniPrezziDettagli Where p.Elem_Cod = obj.Elem_Cod AndAlso p.Pro_Cod = obj.Pro_Cod AndAlso p.Mat_Cod = obj.Mat_Cod AndAlso p.Prezzo > 0).Count > 0, "SI", "NO")
            Next

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(ListiniPrezziProdotti, Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function

    '##############################################################################################
    Public Function Leggi_Listino_Prodotto(ByVal piva As String,
                                           ByVal KeyListinoProdotto As String,
                                           ByRef objParametri As AgronicaCoreParametri
                                           ) As Hashtable

        Dim ht As New Hashtable

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim keys As String() = KeyListinoProdotto.Split("-")
        Dim key_piva As String = keys(0)
        Dim key_Listino_Cod As Integer = Integer.Parse(keys(1))
        Dim key_Elem_Cod As Integer = Integer.Parse(keys(2))
        Dim key_Pro_Cod As Integer = Integer.Parse(keys(3))
        Dim key_Mat_Cod As Integer = Integer.Parse(keys(4))

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Listini_Prezzi_R.Leggi_Listino_Prodotto()"

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim ListinoProdotto =
                (From p In GiasContext.Listini_PrezzixRisorse
                 Where p.Piva_SuperUser = Piva_SuperUser AndAlso p.Piva = piva AndAlso p.Listino_Cod = key_Listino_Cod AndAlso p.Elem_Cod = key_Elem_Cod AndAlso p.Pro_Cod = key_Pro_Cod AndAlso p.Mat_Cod = key_Mat_Cod
                 Join l In GiasContext.Listini_Prezzi On l.Piva_SuperUser Equals p.Piva_SuperUser And l.Piva Equals p.Piva And l.Listino_Cod Equals p.Listino_Cod
                 Join c In GiasContext.Listini_Classi_Prezzi On c.Piva_SuperUser Equals l.Piva_SuperUser And c.Piva Equals l.Piva And c.Listino_Classe_Cod Equals l.Listino_Classe_Cod
                 Join cm In GiasContext.CategorieMagazzino On cm.Elem_Cod Equals p.Elem_Cod
                 Select New With {
                    l.Listino_Cod,
                    l.Listino_Cod_Des,
                    l.Listino_Des,
                    c.Listino_Classe_Des,
                    c.Tipo_Classe,
                    p.Elem_Cod,
                    p.Mat_Cod,
                    p.Pro_Cod,
                    cm.Tabella,
                    cm.NomeComune,
                    .Prodotto = "",
                    .Validita_Inizio = l.Validita_Inizio,
                    .Validita_Fine = l.Validita_Fine
                }).FirstOrDefault
            '.Validita_Inizio = If(p.Validita_Inizio = AGRODATAINIZIO, l.Validita_Inizio, p.Validita_Inizio),
            '.Validita_Fine = If(p.Validita_Fine = AGRODATAFINE, l.Validita_Fine, p.Validita_Fine)

            If ListinoProdotto Is Nothing Then
                ht.Add("Listino_Classe_Des", "Listino non trovato")
                ht.Add("Listino_Cod_Des", "Listino non trovato")
                ht.Add("Listino_Des", "Listino non trovato")
                ht.Add("Validita_Inizio", "")
                ht.Add("Validita_Fine", "")
                ht.Add("Tipo_Classe", "")
                ht.Add("Categoria", "")
                ht.Add("Udm_Cod", 0)
                ht.Add("Prodotto", "Prodotto non trovato")
            Else

                ht.Add("Listino_Classe_Des", ListinoProdotto.Listino_Classe_Des)
                ht.Add("Listino_Cod_Des", ListinoProdotto.Listino_Cod_Des)
                ht.Add("Listino_Des", ListinoProdotto.Listino_Des)
                ht.Add("Validita_Inizio", ListinoProdotto.Validita_Inizio.ToString("dd/MM/yyyy"))
                ht.Add("Validita_Fine", ListinoProdotto.Validita_Fine.ToString("dd/MM/yyyy"))
                ht.Add("Tipo_Classe", ListinoProdotto.Tipo_Classe)
                ht.Add("Categoria", ListinoProdotto.NomeComune)

                ' descrizione prodotto
                Dim Udm_Cod As Integer = 0
                Dim Prodotto As String = "Prodotto non trovato"
                If ListinoProdotto.Tabella = "Materie_Prime" Then
                    Dim mp = (From m In GiasContext.Materie_Prime Where m.Piva = piva AndAlso m.Mat_Cod = ListinoProdotto.Mat_Cod Select m).FirstOrDefault()
                    If mp IsNot Nothing Then
                        Prodotto = mp.Mat_Des
                        Udm_Cod = mp.Udm_Cod
                    End If
                ElseIf ListinoProdotto.Tabella = "Avversita" Then
                    Prodotto = (From x In GiasContext.Avversita Where x.Av_Cod = ListinoProdotto.Pro_Cod Select x.Av_Des_Vol).FirstOrDefault()
                ElseIf ListinoProdotto.Tabella = "Fertilizzanti" Then
                    Prodotto = (From x In GiasContext.Fertilizzanti Where x.Fer_Cod = ListinoProdotto.Pro_Cod Select x.Fer_Des).FirstOrDefault()
                ElseIf ListinoProdotto.Tabella = "Formulati" Then
                    Prodotto = (From x In GiasContext.Formulati Where x.Fr_Cod = ListinoProdotto.Pro_Cod Select x.Fr_Des).FirstOrDefault()
                ElseIf ListinoProdotto.Tabella = "InsettiUtili" Then
                    Prodotto = (From x In GiasContext.InsettiUtili Where x.Ins_Cod = ListinoProdotto.Pro_Cod Select x.Ins_Des).FirstOrDefault()
                ElseIf ListinoProdotto.Tabella = "TipologieSementi" Then
                    Prodotto = (From x In GiasContext.TipologieSementi Where x.SEM_COD = ListinoProdotto.Pro_Cod Select x.SEM_DES).FirstOrDefault()
                ElseIf ListinoProdotto.Tabella = "Trappole" Then
                    Prodotto = (From x In GiasContext.Trappole Where x.TRAP_COD = ListinoProdotto.Pro_Cod Select x.TRAP_DES).FirstOrDefault()
                End If

                ht.Add("Prodotto", Prodotto)
                ht.Add("Udm_Cod", Udm_Cod)

            End If

        End Using

        Return ht

    End Function

    '##############################################################################################
    Public Function Leggi_Listino_Categorie(ByVal piva As String, ByVal Listino_Cod As Integer, ByRef objParametri As AgronicaCoreParametri) As List(Of Listini_PrezzixCategorie)

        Dim Categorie As New List(Of Listini_PrezzixCategorie)

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Listini_Prezzi_R.Leggi_Listino_Categorie()"

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
            Categorie = (From c In GiasContext.Listini_PrezzixCategorie Where c.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso c.Piva.Equals(piva) AndAlso c.Listino_Cod = Listino_Cod Select c).ToList()
        End Using

        Return Categorie

    End Function

    '##############################################################################################
    Public Function Leggi_Listino_Prodotti(ByVal piva As String, ByVal Listino_Cod As Integer, ByVal elem_cod As Integer, ByVal pro_cod As Integer, ByVal mat_cod As Integer, ByRef objParametri As AgronicaCoreParametri) As List(Of Listini_PrezzixRisorse)

        Dim Prodotti As New List(Of Listini_PrezzixRisorse)

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Listini_Prezzi_R.Leggi_Listino_Prodotti()"

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
            Prodotti = (From x In GiasContext.Listini_PrezzixRisorse Where
                             x.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso
                             (piva = "" OrElse x.Piva.Equals(piva)) AndAlso
                             (Listino_Cod = 0 OrElse x.Listino_Cod = Listino_Cod) AndAlso
                             (elem_cod = 0 OrElse x.Elem_Cod = elem_cod) AndAlso (pro_cod = 0 OrElse x.Pro_Cod = pro_cod) AndAlso (mat_cod = 0 OrElse x.Mat_Cod = mat_cod)
                        Select x).ToList()
        End Using

        Return Prodotti

    End Function


    '##############################################################################################
    Public Function Leggi_Listino_Dettagli(ByVal piva As String, ByVal Listino_Cod As Integer, ByVal elem_cod As Integer, ByVal pro_cod As Integer, ByVal mat_cod As Integer, ByRef objParametri As AgronicaCoreParametri) As List(Of Listini_Prezzi_Dettagli)

        Dim Dettagli As New List(Of Listini_Prezzi_Dettagli)

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Listini_Prezzi_R.Leggi_Listino_Dettagli()"

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
            Dettagli = (From x In GiasContext.Listini_Prezzi_Dettagli Where
                             x.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso x.Piva.Equals(piva) AndAlso x.Listino_Cod = Listino_Cod AndAlso
                             (elem_cod = 0 OrElse x.Elem_Cod = elem_cod) AndAlso (pro_cod = 0 OrElse x.Pro_Cod = pro_cod) AndAlso (mat_cod = 0 OrElse x.Mat_Cod = mat_cod)
                        Select x).ToList()
        End Using

        Return Dettagli

    End Function

    '##############################################################################################
    Public Function Leggi_Listino_Classe(ByVal piva As String, ByVal Listino_Classe_Cod As Integer, ByRef objParametri As AgronicaCoreParametri) As Listini_Classi_Prezzi

        Dim Classe As Listini_Classi_Prezzi = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Listini_Prezzi_R.Leggi_Listino_Classe()"

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
            Classe = (From c In GiasContext.Listini_Classi_Prezzi Where c.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso c.Piva.Equals(piva) AndAlso c.Listino_Classe_Cod = Listino_Classe_Cod Select c).FirstOrDefault()
        End Using

        Return Classe

    End Function

    '##############################################################################################
    Public Function Leggi_Listino_Prezzi(ByVal piva As String, ByVal Listino_Cod As Integer, ByRef objParametri As AgronicaCoreParametri) As Listini_Prezzi

        Dim Listino As Listini_Prezzi = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Listini_Prezzi_R.Leggi_Listino_Prezzi()"

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
            Listino = (From d In GiasContext.Listini_Prezzi Where d.Piva_SuperUser.Equals(Piva_SuperUser) AndAlso d.Piva.Equals(piva) AndAlso d.Listino_Cod = Listino_Cod Select d).FirstOrDefault()
        End Using

        Return Listino

    End Function


    '##############################################################################################
    Public Function Simula_Prezzi_Listini(ByVal Piva As String, ByVal Tipo_Classe As Integer, ByVal Listino_Cod As Integer, ByVal Data As String, ByVal Centro As Integer, ByVal Contatto As Integer, ByVal Categoria As Integer, ByVal UdM_Cod As Integer, ByVal Prodotto As String, ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim objListini As New Listini_Prezzi_Dettagli_R

        Dim strFiltro As String = ""
        If Not String.IsNullOrEmpty(Prodotto) Then
            strFiltro = " Materie_Prime.Mat_Des LIKE '%" & Agro_SQL_SaveText(Prodotto) & "%' "
            strFiltro &= " OR Materie_Prime.Cod_Articolo = '" & Agro_SQL_SaveText(Prodotto) & "' "
        End If

        If Data = "" Then
            Data = AGRODATAINIZIO
        ElseIf Not IsDate(Data) Then
            Data = CDate(Now)
        End If

        Dim Cod_Rapporto As Integer = 0
        Dim Cod_Contatto As String = ""
        If Contatto <> 0 Then
            Dim ru_R As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R
            Dim risorsa = ru_R.Leggi_RisorseUmane(Contatto, objParametri)
            If risorsa IsNot Nothing Then
                Cod_Rapporto = risorsa.Cod_Rapporto
                Cod_Contatto = risorsa.Cod_Contatto
            End If
        End If

        Dim dt = objListini.LeggiPrezziListini(Piva, Tipo_Classe, Listino_Cod, Data, Centro, Cod_Rapporto, Cod_Contatto, 0, Categoria, UdM_Cod, 0, 0, strFiltro, objParametri)

        Return dt

    End Function

    '##############################################################################################
    Public Function Leggi_xBudget(ByVal Piva As String,
                                  ByVal isCosti As Boolean,
                                  ByVal xFiltroAggiuntivo As String,
                                  ByVal xOrderBy As String,
                                  ByRef objParametri As AgronicaCoreParametri
                                  ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Listini_Prezzi_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Cod_String As String = IIf(isCosti, "Listino_Cod_Costi", "Listino_Cod_Ricavi")
        Dim Des_String As String = IIf(isCosti, "Listino_Des_Costi", "Listino_Des_Ricavi")

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT lp.Listino_Cod AS " & Cod_String & ", ")
            StrSQL.Append(" lp.Listino_Des AS " & Des_String & " ")
            StrSQL.Append(" FROM Listini_Prezzi lp")
            StrSQL.Append(" WHERE lp.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND lp.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If
            '------------------------------------------------------------------

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

Public Class Listini_Prezzi_W
    Inherits AgronicaCoreDataProvider.DataProvider


#Region "Costruttori"

    Public Sub New()
        Provider = Globalization.CultureInfo.InvariantCulture
        Format = "yyyyMMdd"
        ValiditaInizio = Date.ParseExact("19000101", Format, Provider)
        ValiditaFine = Date.ParseExact("21001231", Format, Provider)
    End Sub

#End Region

    Private _format As String
    Public Shadows Property Format() As String
        Get
            Return _format
        End Get
        Set
            _format = Value
        End Set
    End Property

    Private _provider As Globalization.CultureInfo
    Public Shadows Property Provider() As Globalization.CultureInfo
        Get
            Return _provider
        End Get
        Set
            _provider = Value
        End Set
    End Property


    Private _validitaInizio As Date
    Public Shadows Property ValiditaInizio() As Date
        Get
            Return _validitaInizio
        End Get
        Set
            _validitaInizio = Value
        End Set
    End Property

    Private _validitaFine As Date
    Public Shadows Property ValiditaFine() As Date
        Get
            Return _validitaFine
        End Get
        Set
            _validitaFine = Value
        End Set
    End Property

    Public Function Aggiorna_Classi_Listini(
            ByVal piva As String,
            ByVal righeInserite As String,
            ByVal righeModificate As String,
            ByVal righeCancellate As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As String

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Listini_Prezzi_W.Aggiorna_Classi_Listini()"
        Dim Piva_SuperUser = objParametri.PivaSuperUser
        Dim MessaggioErrore As String = String.Empty

        Try

            Dim leggi As New Listini_Prezzi_R
            Dim listino_classe As New Listini_Classi_Prezzi

            Dim righeInseriteArray As JArray = JArray.Parse(righeInserite)
            Dim righeModificateArray As JArray = JArray.Parse(righeModificate)
            Dim righeCancellateArray As JArray = JArray.Parse(righeCancellate)
            Dim EFArrayToInsert As New ArrayList
            Dim EFArrayToUpdate As New ArrayList
            Dim EFArrayToDelete As New ArrayList
            Dim EFArrayRigheProdottiToDelete As New ArrayList

            For Each obj As JObject In righeInseriteArray

                listino_classe = New Listini_Classi_Prezzi
                listino_classe.Piva_SuperUser = Piva_SuperUser
                listino_classe.Piva = piva
                listino_classe.Listino_Classe_Padre_Cod = -1
                listino_classe.Listino_Classe_Des = obj("Listino_Classe_Des")
                listino_classe.Tipo_Classe = obj("Tipo_Classe")

                If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
                    listino_classe.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio"), Format, Provider)
                End If
                If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
                    listino_classe.Validita_Fine = Date.ParseExact(obj("Validita_Fine"), Format, Provider)
                End If
                listino_classe.Data_Creazione = Date.Now
                listino_classe.Username_Creazione = objParametri.UsernameOperazione
                listino_classe.Data_Modifica = Date.Now
                listino_classe.Username_Modifica = objParametri.UsernameOperazione
                listino_classe.inviato = 0

                EFArrayToInsert.Add(listino_classe)

            Next

            For Each obj As JObject In righeModificateArray

                listino_classe = leggi.Leggi_Listino_Classe(piva, obj("Listino_Classe_Cod"), objParametri)

                If listino_classe Is Nothing Then
                    MessaggioErrore += "Riga da aggiornare non trovata"
                Else

                    listino_classe.Listino_Classe_Des = obj("Listino_Classe_Des")
                    listino_classe.Tipo_Classe = obj("Tipo_Classe")
                    listino_classe.Data_Modifica = Date.Now
                    listino_classe.Username_Modifica = objParametri.UsernameOperazione

                    If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
                        listino_classe.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio"), Format, Provider)
                    End If

                    If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
                        listino_classe.Validita_Fine = Date.ParseExact(obj("Validita_Fine"), Format, Provider)
                    End If

                    EFArrayToUpdate.Add(listino_classe)

                End If
            Next

            For Each obj As JObject In righeCancellateArray

                listino_classe = leggi.Leggi_Listino_Classe(piva, obj("Listino_Classe_Cod"), objParametri)

                If listino_classe IsNot Nothing Then
                    EFArrayToDelete.Add(listino_classe)
                End If

            Next

            If String.IsNullOrEmpty(MessaggioErrore) Then

                Dim objSequenze As New Agro_Sequenze
                Dim gefutils As New Gias_EF_Utility
                Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    For Each classe As Listini_Classi_Prezzi In EFArrayToInsert
                        classe.Listino_Classe_Cod = objSequenze.NuovoId_Tabella_EF(GiasContext, "listini_classi_prezzi", 0, 200000000, objParametri)
                        GiasContext.Listini_Classi_Prezzi.Add(classe)
                    Next

                    For Each classe As Listini_Classi_Prezzi In EFArrayToUpdate
                        GiasContext.Listini_Classi_Prezzi.Attach(classe)
                        GiasContext.Entry(classe).State = EntityState.Modified
                    Next

                    For Each classe As Listini_Classi_Prezzi In EFArrayToDelete
                        Dim listini = From l In GiasContext.Listini_Prezzi
                                      Where l.Listino_Classe_Cod = classe.Listino_Classe_Cod AndAlso
                                            l.Piva_SuperUser.Equals(classe.Piva_SuperUser) AndAlso
                                            l.Piva.Equals(classe.Piva)
                        If listini.Count = 0 Then
                            GiasContext.Listini_Classi_Prezzi.Attach(classe)
                            GiasContext.Listini_Classi_Prezzi.Remove(classe)
                        Else
                            MessaggioErrore &= "Non è possibile cancellare la classe listino <b>" & classe.Listino_Classe_Des & "</b> perchè contiene dei listini<br/>"
                        End If
                    Next

                    GiasContext.SaveChanges()

                End Using

            End If

        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
        Finally

        End Try


        If Not String.IsNullOrEmpty(MessaggioErrore) Then
            Throw New Exception(MessaggioErrore)
        End If

        Return MessaggioErrore
    End Function

    Public Function Aggiorna_Listini(
            ByVal piva As String,
            ByVal righeInserite As String,
            ByVal righeModificate As String,
            ByVal righeCancellate As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As String

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Listini_Prezzi_W.Aggiorna_Listini()"
        Dim Piva_SuperUser = objParametri.PivaSuperUser
        Dim MessaggioErrore As String = String.Empty

        Try

            Dim leggi As New Listini_Prezzi_R
            Dim listino_prezzi As New Listini_Prezzi

            Dim righeInseriteArray As JArray = JArray.Parse(righeInserite)
            Dim righeModificateArray As JArray = JArray.Parse(righeModificate)
            Dim righeCancellateArray As JArray = JArray.Parse(righeCancellate)
            Dim EFArrayToInsert As New ArrayList
            Dim EFArrayToUpdate As New ArrayList
            Dim EFArrayToDelete As New ArrayList
            Dim EFArrayRigheProdottiToDelete As New ArrayList

            For Each obj As JObject In righeInseriteArray

                listino_prezzi = New Listini_Prezzi
                listino_prezzi.Piva_SuperUser = Piva_SuperUser
                listino_prezzi.Piva = piva
                listino_prezzi.Listino_Classe_Cod = obj("Listino_Classe_Cod")
                listino_prezzi.Listino_Cod_Des = obj("Listino_Cod_Des")
                listino_prezzi.Listino_Des = obj("Listino_Des")
                listino_prezzi.ChkApplicabilita = obj("ChkApplicabilita")

                If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
                    listino_prezzi.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio"), Format, Provider)
                End If
                If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
                    listino_prezzi.Validita_Fine = Date.ParseExact(obj("Validita_Fine"), Format, Provider)
                End If
                listino_prezzi.Data_Creazione = Date.Now
                listino_prezzi.Username_Creazione = objParametri.UsernameOperazione
                listino_prezzi.Data_Modifica = Date.Now
                listino_prezzi.Username_Modifica = objParametri.UsernameOperazione
                listino_prezzi.inviato = 0

                EFArrayToInsert.Add(listino_prezzi)

            Next

            For Each obj As JObject In righeModificateArray

                listino_prezzi = leggi.Leggi_Listino_Prezzi(piva, obj("Listino_Cod"), objParametri)

                If listino_prezzi Is Nothing Then
                    MessaggioErrore += "Riga da aggiornare non trovata"
                Else

                    listino_prezzi.Listino_Classe_Cod = obj("Listino_Classe_Cod")
                    listino_prezzi.Listino_Cod_Des = obj("Listino_Cod_Des")
                    listino_prezzi.Listino_Des = obj("Listino_Des")
                    listino_prezzi.ChkApplicabilita = obj("ChkApplicabilita")
                    listino_prezzi.Data_Modifica = Date.Now
                    listino_prezzi.Username_Modifica = objParametri.UsernameOperazione

                    If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
                        listino_prezzi.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio"), Format, Provider)
                    End If

                    If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
                        listino_prezzi.Validita_Fine = Date.ParseExact(obj("Validita_Fine"), Format, Provider)
                    End If

                    EFArrayToUpdate.Add(listino_prezzi)

                End If
            Next

            For Each obj As JObject In righeCancellateArray

                listino_prezzi = leggi.Leggi_Listino_Prezzi(piva, obj("Listino_Cod"), objParametri)

                If listino_prezzi IsNot Nothing Then
                    EFArrayToDelete.Add(listino_prezzi)
                End If

            Next

            ' Controlla se ci sono periodi sovrapposti all'interno delle righe che si stanno gestendo
            If False AndAlso String.IsNullOrEmpty(MessaggioErrore) Then

                Dim listino_prezziElem As Listini_Prezzi_Dettagli = Nothing
                Dim listino_prezziToCompare As Listini_Prezzi_Dettagli = Nothing
                Dim trovatoErrore As Boolean = False

                ' Righe nuove
                For i As Integer = 0 To EFArrayToInsert.Count - 1
                    listino_prezziElem = EFArrayToInsert(i)
                    For x As Integer = 0 To EFArrayToInsert.Count - 1
                        listino_prezziToCompare = EFArrayToInsert(x)
                        If i <> x Then
                            If ((listino_prezziElem.Validita_Inizio <= listino_prezziToCompare.Validita_Inizio AndAlso
                               listino_prezziElem.Validita_Fine >= listino_prezziToCompare.Validita_Inizio) OrElse
                               (listino_prezziElem.Validita_Inizio <= listino_prezziToCompare.Validita_Fine AndAlso
                               listino_prezziElem.Validita_Fine >= listino_prezziToCompare.Validita_Fine)) Then
                                trovatoErrore = True
                            End If
                        End If
                    Next
                    For x As Integer = 0 To EFArrayToUpdate.Count - 1
                        listino_prezziToCompare = EFArrayToUpdate(x)
                        If ((listino_prezziElem.Validita_Inizio <= listino_prezziToCompare.Validita_Inizio AndAlso
                               listino_prezziElem.Validita_Fine >= listino_prezziToCompare.Validita_Inizio) OrElse
                               (listino_prezziElem.Validita_Inizio <= listino_prezziToCompare.Validita_Fine AndAlso
                               listino_prezziElem.Validita_Fine >= listino_prezziToCompare.Validita_Fine)) Then
                            trovatoErrore = True
                        End If
                    Next
                Next

                If Not trovatoErrore Then
                    ' Righe modificate
                    For i As Integer = 0 To EFArrayToUpdate.Count - 1
                        listino_prezziElem = EFArrayToUpdate(i)
                        For x As Integer = 0 To EFArrayToUpdate.Count - 1
                            listino_prezziToCompare = EFArrayToUpdate(x)
                            If i <> x Then
                                If ((listino_prezziElem.Validita_Inizio <= listino_prezziToCompare.Validita_Inizio AndAlso
                               listino_prezziElem.Validita_Fine >= listino_prezziToCompare.Validita_Inizio) OrElse
                               (listino_prezziElem.Validita_Inizio <= listino_prezziToCompare.Validita_Fine AndAlso
                               listino_prezziElem.Validita_Fine >= listino_prezziToCompare.Validita_Fine)) Then
                                    trovatoErrore = True
                                End If
                            End If
                        Next
                        For x As Integer = 0 To EFArrayToInsert.Count - 1
                            listino_prezziToCompare = EFArrayToInsert(x)
                            If ((listino_prezziElem.Validita_Inizio <= listino_prezziToCompare.Validita_Inizio AndAlso
                               listino_prezziElem.Validita_Fine >= listino_prezziToCompare.Validita_Inizio) OrElse
                               (listino_prezziElem.Validita_Inizio <= listino_prezziToCompare.Validita_Fine AndAlso
                               listino_prezziElem.Validita_Fine >= listino_prezziToCompare.Validita_Fine)) Then
                                trovatoErrore = True
                            End If
                        Next
                    Next
                End If

                If trovatoErrore Then
                    MessaggioErrore &= "Esistono righe in periodi sovrapposti al periodo " & listino_prezziElem.Validita_Inizio.ToShortDateString & " - " & listino_prezziElem.Validita_Fine.ToShortDateString & "<br>"
                End If

            End If

            If String.IsNullOrEmpty(MessaggioErrore) Then

                Dim objSequenze As New Agro_Sequenze
                Dim gefutils As New Gias_EF_Utility
                Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    For Each listino As Listini_Prezzi In EFArrayToInsert
                        listino.Listino_Cod = objSequenze.NuovoId_Tabella_EF(GiasContext, "listini_prezzi", 0, 200000000, objParametri)
                        GiasContext.Listini_Prezzi.Add(listino)
                    Next

                    For Each listino As Listini_Prezzi In EFArrayToUpdate
                        GiasContext.Listini_Prezzi.Attach(listino)
                        GiasContext.Entry(listino).State = EntityState.Modified
                    Next

                    For Each listino As Listini_Prezzi In EFArrayToDelete

                        Dim contratti = From p In GiasContext.Imprese_Contratto_Fasi
                                        Where p.Listino_Cod = listino.Listino_Cod AndAlso
                                           p.Piva.Equals(listino.Piva)

                        If contratti.Count > 0 Then
                            MessaggioErrore &= "Non è possibile cancellare il listino <b>" & listino.Listino_Des & "</b> perchè sono presenti dei contratti pomodoro collegati"
                            Continue For
                        End If

                        Dim conferimento = From p In GiasContext.Listini_CampionamentoConferito_Prodotti
                                           Where p.Listino_Cod = listino.Listino_Cod AndAlso
                                           p.Piva_SuperUser.Equals(listino.Piva_SuperUser) AndAlso
                                           p.PIVA.Equals(listino.Piva)

                        If conferimento.Count > 0 Then
                            MessaggioErrore &= "Non è possibile cancellare il listino <b>" & listino.Listino_Des & "</b> perchè sono presenti listini di campionamento/liquidazione associati"
                            Continue For
                        End If

                        Dim prezzi = From p In GiasContext.Listini_Prezzi_Dettagli
                                     Where p.Listino_Cod = listino.Listino_Cod AndAlso
                                           p.Piva_SuperUser.Equals(listino.Piva_SuperUser) AndAlso
                                           p.Piva.Equals(listino.Piva)

                        If prezzi.Count = 0 Then

                            Dim prodotti = From p In GiasContext.Listini_PrezzixRisorse
                                           Where p.Listino_Cod = listino.Listino_Cod AndAlso
                                                 p.Piva_SuperUser.Equals(listino.Piva_SuperUser) AndAlso
                                                 p.Piva.Equals(listino.Piva)

                            ' cancella prodotti
                            For Each p In prodotti
                                GiasContext.Listini_PrezzixRisorse.Attach(p)
                                GiasContext.Listini_PrezzixRisorse.Remove(p)
                            Next

                            Dim categorie =
                                From c In GiasContext.Listini_PrezzixCategorie
                                Where c.Listino_Cod = listino.Listino_Cod AndAlso
                                      c.Piva_SuperUser.Equals(listino.Piva_SuperUser) AndAlso
                                      c.Piva.Equals(listino.Piva)

                            ' cancella categorie
                            For Each c In categorie
                                GiasContext.Listini_PrezzixCategorie.Attach(c)
                                GiasContext.Listini_PrezzixCategorie.Remove(c)
                            Next

                            Dim associazioni =
                                    From a In GiasContext.Listini_PrezzixContatti
                                    Where a.Listino_Cod = listino.Listino_Cod AndAlso
                                          a.Piva_SuperUser.Equals(listino.Piva_SuperUser) AndAlso
                                          a.Piva.Equals(listino.Piva)

                            ' cancella associazioni
                            For Each a In associazioni
                                GiasContext.Listini_PrezzixContatti.Attach(a)
                                GiasContext.Listini_PrezzixContatti.Remove(a)
                            Next

                            GiasContext.Listini_Prezzi.Attach(listino)
                            GiasContext.Listini_Prezzi.Remove(listino)

                        Else

                            MessaggioErrore &= "Non è possibile cancellare il listino <b>" & listino.Listino_Des & "</b> perchè sono presenti dei prezzi, rimuovere prima i prodotti"

                        End If

                    Next

                    GiasContext.SaveChanges()

                End Using

            End If

        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
        Finally

        End Try

        Return MessaggioErrore

    End Function

    Public Function Copia_Listini_Prezzi(
            ByVal piva As String,
            ByVal Listino_Cod As Integer,
            ByRef objParametri As AgronicaCoreParametri
        ) As String

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Listini_Prezzi_W.Copia_Listini_Prezzi()"
        Dim Piva_SuperUser = objParametri.PivaSuperUser
        Dim MessaggioErrore As String = String.Empty

        Try

            Dim leggi As New Listini_Prezzi_R
            Dim listino = leggi.Leggi_Listino_Prezzi(piva, Listino_Cod, objParametri)

            If listino IsNot Nothing Then

                Dim listinoCategorie = leggi.Leggi_Listino_Categorie(piva, Listino_Cod, objParametri)
                Dim listinoProdotti = leggi.Leggi_Listino_Prodotti(piva, Listino_Cod, 0, 0, 0, objParametri)
                Dim listinoDettagli = leggi.Leggi_Listino_Dettagli(piva, Listino_Cod, 0, 0, 0, objParametri)

                Dim objSequenze As New Agro_Sequenze
                Dim gefutils As New Gias_EF_Utility
                Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    Dim cod_listino = objSequenze.NuovoId_Tabella_EF(GiasContext, "listini_prezzi", 0, 200000000, objParametri)

                    Dim listino_prezzi = New Listini_Prezzi
                    listino_prezzi.Piva_SuperUser = Piva_SuperUser
                    listino_prezzi.Piva = piva
                    listino_prezzi.Listino_Cod = cod_listino
                    listino_prezzi.Listino_Classe_Cod = listino.Listino_Classe_Cod
                    listino_prezzi.Listino_Cod_Des = listino.Listino_Cod_Des
                    listino_prezzi.Listino_Des = "Copia di " & listino.Listino_Des
                    listino_prezzi.ChkApplicabilita = listino.ChkApplicabilita
                    listino_prezzi.Validita_Inizio = listino.Validita_Inizio
                    listino_prezzi.Validita_Fine = listino.Validita_Fine
                    listino_prezzi.Data_Creazione = Date.Now
                    listino_prezzi.Username_Creazione = objParametri.UsernameOperazione
                    listino_prezzi.Data_Modifica = Date.Now
                    listino_prezzi.Username_Modifica = objParametri.UsernameOperazione
                    listino_prezzi.inviato = 0
                    GiasContext.Listini_Prezzi.Add(listino_prezzi)

                    For Each categoria As Listini_PrezzixCategorie In listinoCategorie

                        Dim listino_categoria = New Listini_PrezzixCategorie
                        listino_categoria.Piva_SuperUser = Piva_SuperUser
                        listino_categoria.Piva = piva
                        listino_categoria.Listino_Cod = cod_listino
                        listino_categoria.Elem_Cod = categoria.Elem_Cod
                        listino_categoria.Validita_Inizio = categoria.Validita_Inizio
                        listino_categoria.Validita_Fine = categoria.Validita_Fine
                        listino_categoria.Data_Creazione = Date.Now
                        listino_categoria.Username_Creazione = objParametri.UsernameOperazione
                        listino_categoria.Data_Modifica = Date.Now
                        listino_categoria.Username_Modifica = objParametri.UsernameOperazione
                        listino_categoria.inviato = 0
                        GiasContext.Listini_PrezzixCategorie.Add(listino_categoria)

                    Next

                    For Each prodotto As Listini_PrezzixRisorse In listinoProdotti

                        Dim listino_prodotto As New Listini_PrezzixRisorse
                        listino_prodotto.Piva_SuperUser = Piva_SuperUser
                        listino_prodotto.Piva = piva
                        listino_prodotto.Listino_Cod = cod_listino
                        listino_prodotto.Elem_Cod = prodotto.Elem_Cod
                        listino_prodotto.Pro_Cod = prodotto.Pro_Cod
                        listino_prodotto.Mat_Cod = prodotto.Mat_Cod
                        listino_prodotto.Cod_Conto_Default = prodotto.Cod_Conto_Default
                        listino_prodotto.Validita_Inizio = prodotto.Validita_Inizio
                        listino_prodotto.Validita_Fine = prodotto.Validita_Fine
                        listino_prodotto.Data_Creazione = Date.Now
                        listino_prodotto.Username_Creazione = objParametri.UsernameOperazione
                        listino_prodotto.Data_Modifica = Date.Now
                        listino_prodotto.Username_Modifica = objParametri.UsernameOperazione
                        listino_prodotto.inviato = 0
                        GiasContext.Listini_PrezzixRisorse.Add(listino_prodotto)

                    Next

                    For Each dettaglio As Listini_Prezzi_Dettagli In listinoDettagli

                        Dim listino_dettaglio As New Listini_Prezzi_Dettagli
                        listino_dettaglio.Piva_SuperUser = Piva_SuperUser
                        listino_dettaglio.Piva = piva
                        listino_dettaglio.Listino_Cod = cod_listino
                        listino_dettaglio.Elem_Cod = dettaglio.Elem_Cod
                        listino_dettaglio.Pro_Cod = dettaglio.Pro_Cod
                        listino_dettaglio.Mat_Cod = dettaglio.Mat_Cod
                        listino_dettaglio.Mezzo = dettaglio.Mezzo
                        listino_dettaglio.Udm_Cod = dettaglio.Udm_Cod
                        listino_dettaglio.Qta = dettaglio.Qta
                        listino_dettaglio.Prezzo = dettaglio.Prezzo
                        listino_dettaglio.Sconto_Add1 = dettaglio.Sconto_Add1
                        listino_dettaglio.Sconto_Condizione_Valore1 = dettaglio.Sconto_Condizione_Valore1
                        listino_dettaglio.Sconto_Condizione1 = dettaglio.Sconto_Condizione1
                        listino_dettaglio.Sconto_Add2 = dettaglio.Sconto_Add2
                        listino_dettaglio.Sconto_Condizione_Valore2 = dettaglio.Sconto_Condizione_Valore2
                        listino_dettaglio.Sconto_Condizione2 = dettaglio.Sconto_Condizione2
                        listino_dettaglio.Sconto_Add3 = dettaglio.Sconto_Add3
                        listino_dettaglio.Sconto_Condizione_Valore3 = dettaglio.Sconto_Condizione_Valore3
                        listino_dettaglio.Sconto_Condizione3 = dettaglio.Sconto_Condizione3
                        listino_dettaglio.Sconto = dettaglio.Sconto
                        listino_dettaglio.Cod_Iva = dettaglio.Cod_Iva
                        listino_dettaglio.Cod_Rapporto = dettaglio.Cod_Rapporto
                        listino_dettaglio.Cod_RisUm = dettaglio.Cod_RisUm
                        listino_dettaglio.Cal_Cod = dettaglio.Cal_Cod
                        listino_dettaglio.Qualita_Cod = dettaglio.Qualita_Cod
                        listino_dettaglio.Prezzo_Livello = dettaglio.Prezzo_Livello
                        listino_dettaglio.Lotto_Cod1 = dettaglio.Lotto_Cod1
                        listino_dettaglio.Lotto_Val1 = dettaglio.Lotto_Val1
                        listino_dettaglio.Lotto_Cod2 = dettaglio.Lotto_Cod2
                        listino_dettaglio.Lotto_Val2 = dettaglio.Lotto_Val2
                        listino_dettaglio.Cod_Conto_Det_Default = dettaglio.Cod_Conto_Det_Default
                        listino_dettaglio.Validita_Inizio = dettaglio.Validita_Inizio
                        listino_dettaglio.Validita_Fine = dettaglio.Validita_Fine
                        listino_dettaglio.Data_Creazione = Date.Now
                        listino_dettaglio.Username_Creazione = objParametri.UsernameOperazione
                        listino_dettaglio.Data_Modifica = Date.Now
                        listino_dettaglio.Username_Modifica = objParametri.UsernameOperazione
                        listino_dettaglio.inviato = 0
                        GiasContext.Listini_Prezzi_Dettagli.Add(listino_dettaglio)

                    Next

                    GiasContext.SaveChanges()

                End Using

            End If

        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
        Finally

        End Try

        If Not String.IsNullOrEmpty(MessaggioErrore) Then
            Throw New Exception(MessaggioErrore)
        End If

        Return MessaggioErrore
    End Function

    Public Function Cancella_Prodotti(
            ByVal piva As String,
            ByVal Listino_Cod As Integer,
            ByVal elem_cod As Integer,
            ByVal pro_cod As Integer,
            ByVal mat_cod As Integer,
            ByRef objParametri As AgronicaCoreParametri
        ) As String

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Listini_Prezzi_W.Cancella_Prodotti()"
        Dim Piva_SuperUser = objParametri.PivaSuperUser
        Dim MessaggioErrore As String = String.Empty

        Try

            Dim leggi As New Listini_Prezzi_R
            'Dim listino = leggi.Leggi_Listino_Prezzi(piva, Listino_Cod, objParametri)
            'Dim listinoCategorie = leggi.Leggi_Listino_Categorie(piva, Listino_Cod, objParametri)
            Dim listinoProdotti = leggi.Leggi_Listino_Prodotti(piva, Listino_Cod, elem_cod, pro_cod, mat_cod, objParametri)

            If listinoProdotti.Count > 0 Then

                Dim gefutils As New Gias_EF_Utility
                Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    For Each prodotto As Listini_PrezzixRisorse In listinoProdotti

                        Dim prezzi = From p In GiasContext.Listini_Prezzi_Dettagli
                                     Where p.Listino_Cod = prodotto.Listino_Cod AndAlso
                                           p.Piva_SuperUser.Equals(prodotto.Piva_SuperUser) AndAlso
                                           p.Piva.Equals(prodotto.Piva) AndAlso
                                           p.Elem_Cod = prodotto.Elem_Cod AndAlso
                                           p.Pro_Cod = prodotto.Pro_Cod AndAlso
                                           p.Mat_Cod = prodotto.Mat_Cod

                        ' cancella prezzi
                        For Each p In prezzi
                            GiasContext.Listini_Prezzi_Dettagli.Attach(p)
                            GiasContext.Listini_Prezzi_Dettagli.Remove(p)
                        Next

                        GiasContext.Listini_PrezzixRisorse.Attach(prodotto)
                        GiasContext.Listini_PrezzixRisorse.Remove(prodotto)

                    Next

                    GiasContext.SaveChanges()

                End Using

            End If

        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
        End Try

        If Not String.IsNullOrEmpty(MessaggioErrore) Then
            Throw New Exception(MessaggioErrore)
        End If

        Return MessaggioErrore
    End Function

    Public Function Aggiungi_Prodotti(
            ByVal piva As String,
            ByVal Listino_Cod As Integer,
            ByVal prodotti As String,
            ByRef objParametri As AgronicaCoreParametri
        ) As String

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Listini_Prezzi_W.Aggiungi_Prodotti()"
        Dim Piva_SuperUser = objParametri.PivaSuperUser
        Dim MessaggioErrore As String = String.Empty

        Try

            Dim leggi As New Listini_Prezzi_R
            Dim listino = leggi.Leggi_Listino_Prezzi(piva, Listino_Cod, objParametri)
            Dim listinoCategorie = leggi.Leggi_Listino_Categorie(piva, Listino_Cod, objParametri)
            Dim listinoProdotti = leggi.Leggi_Listino_Prodotti(piva, Listino_Cod, 0, 0, 0, objParametri)
            Dim prodottiArray As JArray = JArray.Parse(prodotti)

            Dim EFArrayCategorie As New ArrayList
            Dim EFArrayProdotti As New ArrayList

            For Each obj As JObject In prodottiArray

                Dim elem_cod As Integer = CInt(obj("elem_cod"))
                Dim prodotto_cod As Integer = CInt(obj("prodotto_cod"))
                Dim pro_cod = If(prodotto_cod > 0, prodotto_cod, 0)
                Dim mat_cod = If(prodotto_cod > 0, 0, -prodotto_cod)

                Dim categoria = (From c In listinoCategorie Where c.Piva_SuperUser = Piva_SuperUser AndAlso c.Piva = piva AndAlso c.Listino_Cod = Listino_Cod AndAlso c.Elem_Cod = elem_cod).FirstOrDefault

                If categoria Is Nothing Then

                    Dim listino_categoria As New Listini_PrezzixCategorie
                    listino_categoria.Piva_SuperUser = Piva_SuperUser
                    listino_categoria.Piva = piva
                    listino_categoria.Listino_Cod = Listino_Cod
                    listino_categoria.Elem_Cod = elem_cod
                    listino_categoria.Validita_Inizio = listino.Validita_Inizio
                    listino_categoria.Validita_Fine = listino.Validita_Fine
                    listino_categoria.Data_Creazione = Date.Now
                    listino_categoria.Username_Creazione = objParametri.UsernameOperazione
                    listino_categoria.Data_Modifica = Date.Now
                    listino_categoria.Username_Modifica = objParametri.UsernameOperazione
                    listino_categoria.inviato = 0

                    listinoCategorie.Add(listino_categoria)
                    EFArrayCategorie.Add(listino_categoria)

                End If

                Dim prodotto = (From p In listinoProdotti Where p.Piva_SuperUser = Piva_SuperUser AndAlso p.Piva = piva AndAlso p.Listino_Cod = Listino_Cod AndAlso p.Elem_Cod = elem_cod AndAlso p.Pro_Cod = pro_cod AndAlso p.Mat_Cod = mat_cod).FirstOrDefault

                If prodotto Is Nothing Then

                    Dim listino_prodotto As New Listini_PrezzixRisorse
                    listino_prodotto.Piva_SuperUser = Piva_SuperUser
                    listino_prodotto.Piva = piva
                    listino_prodotto.Listino_Cod = Listino_Cod
                    listino_prodotto.Elem_Cod = elem_cod
                    listino_prodotto.Pro_Cod = pro_cod
                    listino_prodotto.Mat_Cod = mat_cod
                    listino_prodotto.Cod_Conto_Default = 0
                    listino_prodotto.Validita_Inizio = listino.Validita_Inizio
                    listino_prodotto.Validita_Fine = listino.Validita_Fine
                    listino_prodotto.Data_Creazione = Date.Now
                    listino_prodotto.Username_Creazione = objParametri.UsernameOperazione
                    listino_prodotto.Data_Modifica = Date.Now
                    listino_prodotto.Username_Modifica = objParametri.UsernameOperazione
                    listino_prodotto.inviato = 0

                    listinoProdotti.Add(listino_prodotto)
                    EFArrayProdotti.Add(listino_prodotto)

                End If

            Next

            If String.IsNullOrEmpty(MessaggioErrore) Then

                Dim objSequenze As New Agro_Sequenze
                Dim gefutils As New Gias_EF_Utility
                Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    For Each categoria As Listini_PrezzixCategorie In EFArrayCategorie
                        GiasContext.Listini_PrezzixCategorie.Add(categoria)
                    Next

                    For Each prodotto As Listini_PrezzixRisorse In EFArrayProdotti
                        GiasContext.Listini_PrezzixRisorse.Add(prodotto)
                    Next

                    GiasContext.SaveChanges()

                End Using

            End If

        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
        Finally

        End Try

        If Not String.IsNullOrEmpty(MessaggioErrore) Then
            Throw New Exception(MessaggioErrore)
        End If

        Return MessaggioErrore
    End Function

End Class