Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Listini
    Inherits AgronicaCoreDataProvider.DataProvider


    '###############################################################################
    'default:
    'Listino_Classe_Padre_Cod = -1 
    'ChkApplicabilita =-1
    'Conto_Terzi = -1
    'Chk_Vettore =-1
    'Mezzo =-1
    'Cod_Conto_Det_Default =-1
    Public Function Listini_Export(ByVal Piva As String,
                                   ByVal Listino_Classe_Cod As Integer,
                                   ByVal Listino_Cod As Integer,
                                   ByVal Elem_Cod As Integer,
                                   ByVal Pro_Cod As Integer,
                                   ByVal Mat_Cod As Integer,
                                   ByVal Tipo_Classe As Integer,
                                   ByVal Listino_Classe_Padre_Cod As Integer,
                                   ByVal ChkApplicabilita As Integer,
                                   ByVal Tipo_Iva As Integer,
                                   ByVal Tipo_Provvigione As Integer,
                                   ByVal Conto_Terzi As Integer,
                                   ByVal Cod_Conto_Default As Integer,
                                   ByVal Cal_Cod As Integer,
                                   ByVal Lav_Cod As Integer,
                                   ByVal Chk_Vettore As Integer,
                                   ByVal Cod_Rapporto As Integer,
                                   ByVal Cod_RisUm As Integer,
                                   ByVal Mezzo As Integer,
                                   ByVal Udm_Cod As Integer,
                                   ByVal Cod_Iva As Integer,
                                   ByVal Cod_Conto_Det_Default As Integer,
                                   ByVal xFiltroAggiuntivo1 As String,
                                   ByVal xFiltroAggiuntivo2 As String,
                                   ByVal xOrderBy As String,
                                   ByRef objParametri As AgronicaCoreParametri
                                   ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.Listini.Listini_Export()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.Append(" ( " & vbCrLf)

            '------------------------------------------------------------------------
            '---- PRIMA PARTE DELL'UNION: listini materie prime --------------------
            '------------------------------------------------------------------------

            Stb.Append(" SELECT  Listini_Classi_Prezzi.Piva_SuperUser, Listini_Classi_Prezzi.Piva, Listini_Classi_Prezzi.Listino_Classe_Cod, Listini_Classi_Prezzi.Listino_Classe_Des, Listini_Classi_Prezzi.Tipo_Classe, Listini_Classi_Prezzi.Listino_Classe_Padre_Cod, " & vbCrLf)

            Stb.Append(" Listini_Prezzi.Listino_Cod, Listini_Prezzi.Listino_Des, Listini_Prezzi.Listino_Cod_Des, Listini_Prezzi.ChkApplicabilita, Listini_Prezzi.Tipo_Iva, Listini_Prezzi.Tipo_Provvigione,  " & vbCrLf)
            Stb.Append(" Listini_Prezzi.Validita_Inizio AS Validita_Inizio_Listino, Listini_Prezzi.Validita_Fine AS Validita_Fine_Listino, Listini_PrezzixCategorie.Elem_Cod, " & vbCrLf)

            Stb.Append(" Listini_PrezzixRisorse.Pro_Cod, Listini_PrezzixRisorse.Mat_Cod, Listini_PrezzixRisorse.Conto_Terzi, Listini_PrezzixRisorse.Cod_Conto_Default, " & vbCrLf)

            Stb.Append(" Listini_Prezzi_Dettagli.Cal_Cod, Listini_Prezzi_Dettagli.Lotto_Cod1, Listini_Prezzi_Dettagli.Lotto_Val1, Listini_Prezzi_Dettagli.Lotto_Cod2, Listini_Prezzi_Dettagli.Lotto_Val2, Listini_Prezzi_Dettagli.Lav_Cod, Listini_Prezzi_Dettagli.ChkVettore, " & vbCrLf)
            Stb.Append(" Listini_Prezzi_Dettagli.Cod_Rapporto, Listini_Prezzi_Dettagli.Cod_RisUm, Listini_Prezzi_Dettagli.Mezzo, Listini_Prezzi_Dettagli.Udm_Cod, Listini_Prezzi_Dettagli.Qta, Listini_Prezzi_Dettagli.Prezzo, Listini_Prezzi_Dettagli.Validita_Inizio, Listini_Prezzi_Dettagli.Validita_Fine, " & vbCrLf)
            Stb.Append(" Listini_Prezzi_Dettagli.ChkSemina, Listini_Prezzi_Dettagli.Cod_Iva, Listini_Prezzi_Dettagli.Sconto, Listini_Prezzi_Dettagli.Cod_Conto_Det_Default, Listini_Prezzi_Dettagli.Tipo_Iva_Det, Listini_Prezzi_Dettagli.Tipo_Provvigione_Det, Listini_Prezzi_Dettagli.Provvigione_Listino " & vbCrLf)
            Stb.Append(" , CategorieMagazzino.NomeComune, UnitaMisura.Udm_Des, UnitaMisura.Udm_Sim " & vbCrLf)
            Stb.Append(" , Materie_Prime.Mat_Des, Materie_Prime.Cod_Articolo  " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" " & vbCrLf)
            Stb.Append(" FROM Listini_Classi_Prezzi " & vbCrLf)
            Stb.Append(" INNER JOIN Listini_Prezzi ON Listini_Classi_Prezzi.Piva_SuperUser = Listini_Prezzi.Piva_SuperUser        " & vbCrLf)
            Stb.Append(" AND Listini_Classi_Prezzi.Piva = Listini_Prezzi.Piva   " & vbCrLf)
            Stb.Append(" AND Listini_Classi_Prezzi.Listino_Classe_Cod = Listini_Prezzi.Listino_Classe_Cod   " & vbCrLf)

            Stb.Append(" INNER JOIN Listini_PrezzixCategorie ON Listini_PrezzixCategorie.Piva_SuperUser = Listini_Prezzi.Piva_SuperUser " & vbCrLf)
            Stb.Append(" AND Listini_PrezzixCategorie.Piva = Listini_Prezzi.Piva   " & vbCrLf)
            Stb.Append(" AND Listini_PrezzixCategorie.Listino_Cod = Listini_Prezzi.Listino_Cod   " & vbCrLf)

            Stb.Append(" INNER JOIN Listini_PrezzixRisorse ON Listini_PrezzixRisorse.Piva_SuperUser = Listini_PrezzixCategorie.Piva_SuperUser " & vbCrLf)
            Stb.Append(" AND Listini_PrezzixRisorse.Piva = Listini_PrezzixCategorie.Piva   " & vbCrLf)
            Stb.Append(" AND Listini_PrezzixRisorse.Listino_Cod = Listini_PrezzixCategorie.Listino_Cod   " & vbCrLf)
            Stb.Append(" AND Listini_PrezzixRisorse.Elem_Cod = Listini_PrezzixCategorie.Elem_Cod   " & vbCrLf)

            Stb.Append(" INNER JOIN Listini_Prezzi_Dettagli ON Listini_PrezzixRisorse.Piva_SuperUser = Listini_Prezzi_Dettagli.Piva_SuperUser " & vbCrLf)
            Stb.Append(" AND Listini_Prezzi_Dettagli.Piva = Listini_PrezzixRisorse.Piva   " & vbCrLf)
            Stb.Append(" AND Listini_Prezzi_Dettagli.Listino_Cod = Listini_PrezzixRisorse.Listino_Cod   " & vbCrLf)
            Stb.Append(" AND Listini_Prezzi_Dettagli.Elem_Cod = Listini_PrezzixRisorse.Elem_Cod   " & vbCrLf)
            Stb.Append(" AND Listini_Prezzi_Dettagli.Pro_Cod = Listini_PrezzixRisorse.Pro_Cod   " & vbCrLf)
            Stb.Append(" AND Listini_Prezzi_Dettagli.Mat_Cod = Listini_PrezzixRisorse.Mat_Cod   " & vbCrLf)

            Stb.Append(" INNER JOIN UnitaMisura ON Listini_Prezzi_Dettagli.Udm_Cod = UnitaMisura.Udm_Cod " & vbCrLf)
            Stb.Append(" INNER JOIN CategorieMagazzino ON Listini_PrezzixCategorie.Elem_Cod = CategorieMagazzino.Elem_Cod " & vbCrLf)
            Stb.Append(" INNER JOIN Materie_Prime ON Listini_PrezzixRisorse.Elem_Cod = Materie_Prime.Elem_Cod AND  Listini_PrezzixRisorse.Mat_Cod = Materie_Prime.Mat_Cod " & vbCrLf)

            Stb.Append("  " & vbCrLf)
            Stb.Append("  " & vbCrLf)

            Stb.Append(" WHERE Listini_Classi_Prezzi.Piva_SuperUser  = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  " & vbCrLf)

            Stb.Append(" AND Listini_Prezzi_Dettagli.Prezzo <> 0 " & vbCrLf)
            Stb.Append(" " & vbCrLf)

            If Piva <> "" Then
                Stb.Append(" AND Listini_Classi_Prezzi.Piva = '" & Agro_SQL_SaveText(Piva) & "'  " & vbCrLf)
            End If

            If Listino_Classe_Cod <> 0 Then
                Stb.Append(" AND Listini_Classi_Prezzi.Listino_Classe_Cod = " & Agro_SQL_SaveNum(Listino_Classe_Cod) & "  " & vbCrLf)
            End If

            If Tipo_Classe <> 0 Then
                Stb.Append(" AND Listini_Classi_Prezzi.Tipo_Classe = " & Agro_SQL_SaveNum(Tipo_Classe) & "  " & vbCrLf)
            End If

            'default-1
            If Listino_Classe_Padre_Cod <> -1 Then
                Stb.Append(" AND Listini_Classi_Prezzi.Listino_Classe_Padre_Cod = " & Agro_SQL_SaveNum(Listino_Classe_Padre_Cod) & "  " & vbCrLf)
            End If

            '----

            If Listino_Cod <> 0 Then
                Stb.Append(" AND Listini_Prezzi.Listino_Cod = " & Agro_SQL_SaveNum(Listino_Cod) & "  " & vbCrLf)
            End If

            If ChkApplicabilita <> -1 Then
                Stb.Append(" AND Listini_Prezzi.ChkApplicabilita = " & Agro_SQL_SaveNum(ChkApplicabilita) & "  " & vbCrLf)
            End If

            If Tipo_Iva <> 0 Then
                Stb.Append(" AND Listini_Prezzi.Tipo_Iva = " & Agro_SQL_SaveNum(Tipo_Iva) & "  " & vbCrLf)
            End If

            If Tipo_Provvigione <> 0 Then
                Stb.Append(" AND Listini_Prezzi.Tipo_Provvigione = " & Agro_SQL_SaveNum(Tipo_Provvigione) & "  " & vbCrLf)
            End If

            '----

            If Elem_Cod <> 0 Then
                Stb.Append(" AND Listini_PrezzixCategorie.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "  " & vbCrLf)
            End If

            '----

            If Pro_Cod <> 0 Then
                Stb.Append(" AND Listini_PrezzixRisorse.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "  " & vbCrLf)
            End If

            If Mat_Cod <> 0 Then
                Stb.Append(" AND Listini_PrezzixRisorse.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "  " & vbCrLf)
            End If

            If Conto_Terzi <> -1 Then
                Stb.Append(" AND Listini_PrezzixRisorse.Conto_Terzi = " & Agro_SQL_SaveNum(Conto_Terzi) & "  " & vbCrLf)
            End If

            If Cod_Conto_Default <> 0 Then
                Stb.Append(" AND Listini_PrezzixRisorse.Cod_Conto_Default = " & Agro_SQL_SaveNum(Cod_Conto_Default) & "  " & vbCrLf)
            End If

            '----

            If Cal_Cod <> 0 Then
                Stb.Append(" AND Listini_Prezzi_Dettagli.Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "  " & vbCrLf)
            End If

            If Lav_Cod <> 0 Then
                Stb.Append(" AND Listini_Prezzi_Dettagli.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & "  " & vbCrLf)
            End If

            If Chk_Vettore <> -1 Then
                Stb.Append(" AND Listini_Prezzi_Dettagli.ChkVettore = " & Agro_SQL_SaveNum(Chk_Vettore) & "  " & vbCrLf)
            End If

            If Cod_Rapporto <> 0 Then
                Stb.Append(" AND Listini_Prezzi_Dettagli.Cod_Rapporto = " & Agro_SQL_SaveNum(Cod_Rapporto) & "  " & vbCrLf)
            End If

            If Cod_RisUm <> 0 Then
                Stb.Append(" AND Listini_Prezzi_Dettagli.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "  " & vbCrLf)
            End If

            If Mezzo <> -1 Then
                Stb.Append(" AND Listini_Prezzi_Dettagli.Mezzo = " & Agro_SQL_SaveNum(Mezzo) & "  " & vbCrLf)
            End If

            If Udm_Cod <> 0 Then
                Stb.Append(" AND Listini_Prezzi_Dettagli.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "  " & vbCrLf)
            End If

            If Cal_Cod <> 0 Then
                Stb.Append(" AND Listini_Prezzi_Dettagli.Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "  " & vbCrLf)
            End If

            If Cod_Iva <> 0 Then
                Stb.Append(" AND Listini_Prezzi_Dettagli.Cod_Iva = " & Agro_SQL_SaveNum(Cod_Iva) & "  " & vbCrLf)
            End If

            If Cod_Conto_Det_Default <> -1 Then
                Stb.Append(" AND Listini_Prezzi_Dettagli.Cod_Conto_Det_Default = " & Agro_SQL_SaveNum(Cod_Conto_Det_Default) & "  " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo1 <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo1, , objParametri))
            End If

            '#############################################
            Stb.Append(" UNION ALL " & vbCrLf)
            '#############################################

            '------------------------------------------------------------------------
            '---- SECONDA PARTE DELL'UNION: listini prodotti banca dati ---------
            '------------------------------------------------------------------------

            Stb.Append(" SELECT  Listini_Classi_Prezzi.Piva_SuperUser, Listini_Classi_Prezzi.Piva, Listini_Classi_Prezzi.Listino_Classe_Cod, Listini_Classi_Prezzi.Listino_Classe_Des, Listini_Classi_Prezzi.Tipo_Classe, Listini_Classi_Prezzi.Listino_Classe_Padre_Cod, " & vbCrLf)

            Stb.Append(" Listini_Prezzi.Listino_Cod, Listini_Prezzi.Listino_Des, Listini_Prezzi.Listino_Cod_Des, Listini_Prezzi.ChkApplicabilita, Listini_Prezzi.Tipo_Iva, Listini_Prezzi.Tipo_Provvigione,  " & vbCrLf)
            Stb.Append(" Listini_Prezzi.Validita_Inizio AS Validita_Inizio_Listino, Listini_Prezzi.Validita_Fine AS Validita_Fine_Listino, Listini_PrezzixCategorie.Elem_Cod, " & vbCrLf)

            Stb.Append(" Listini_PrezzixRisorse.Pro_Cod, Listini_PrezzixRisorse.Mat_Cod, Listini_PrezzixRisorse.Conto_Terzi, Listini_PrezzixRisorse.Cod_Conto_Default, " & vbCrLf)

            Stb.Append(" Listini_Prezzi_Dettagli.Cal_Cod, Listini_Prezzi_Dettagli.Lotto_Cod1, Listini_Prezzi_Dettagli.Lotto_Val1, Listini_Prezzi_Dettagli.Lotto_Cod2, Listini_Prezzi_Dettagli.Lotto_Val2, Listini_Prezzi_Dettagli.Lav_Cod, Listini_Prezzi_Dettagli.ChkVettore, " & vbCrLf)
            Stb.Append(" Listini_Prezzi_Dettagli.Cod_Rapporto, Listini_Prezzi_Dettagli.Cod_RisUm, Listini_Prezzi_Dettagli.Mezzo, Listini_Prezzi_Dettagli.Udm_Cod, Listini_Prezzi_Dettagli.Qta, Listini_Prezzi_Dettagli.Prezzo, Listini_Prezzi_Dettagli.Validita_Inizio, Listini_Prezzi_Dettagli.Validita_Fine, " & vbCrLf)
            Stb.Append(" Listini_Prezzi_Dettagli.ChkSemina, Listini_Prezzi_Dettagli.Cod_Iva, Listini_Prezzi_Dettagli.Sconto, Listini_Prezzi_Dettagli.Cod_Conto_Det_Default, Listini_Prezzi_Dettagli.Tipo_Iva_Det, Listini_Prezzi_Dettagli.Tipo_Provvigione_Det, Listini_Prezzi_Dettagli.Provvigione_Listino " & vbCrLf)
            Stb.Append(" , CategorieMagazzino.NomeComune, UnitaMisura.Udm_Des, UnitaMisura.Udm_Sim " & vbCrLf)

            Stb.Append(" , '' AS Mat_Des, '' AS Cod_Articolo  " & vbCrLf)

            Stb.Append("  " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" " & vbCrLf)
            Stb.Append(" FROM Listini_Classi_Prezzi " & vbCrLf)
            Stb.Append(" INNER JOIN Listini_Prezzi ON Listini_Classi_Prezzi.Piva_SuperUser = Listini_Prezzi.Piva_SuperUser        " & vbCrLf)
            Stb.Append(" AND Listini_Classi_Prezzi.Piva = Listini_Prezzi.Piva   " & vbCrLf)
            Stb.Append(" AND Listini_Classi_Prezzi.Listino_Classe_Cod = Listini_Prezzi.Listino_Classe_Cod   " & vbCrLf)

            Stb.Append(" INNER JOIN Listini_PrezzixCategorie ON Listini_PrezzixCategorie.Piva_SuperUser = Listini_Prezzi.Piva_SuperUser " & vbCrLf)
            Stb.Append(" AND Listini_PrezzixCategorie.Piva = Listini_Prezzi.Piva   " & vbCrLf)
            Stb.Append(" AND Listini_PrezzixCategorie.Listino_Cod = Listini_Prezzi.Listino_Cod   " & vbCrLf)

            Stb.Append(" INNER JOIN Listini_PrezzixRisorse ON Listini_PrezzixRisorse.Piva_SuperUser = Listini_PrezzixCategorie.Piva_SuperUser " & vbCrLf)
            Stb.Append(" AND Listini_PrezzixRisorse.Piva = Listini_PrezzixCategorie.Piva   " & vbCrLf)
            Stb.Append(" AND Listini_PrezzixRisorse.Listino_Cod = Listini_PrezzixCategorie.Listino_Cod   " & vbCrLf)
            Stb.Append(" AND Listini_PrezzixRisorse.Elem_Cod = Listini_PrezzixCategorie.Elem_Cod   " & vbCrLf)

            Stb.Append(" INNER JOIN Listini_Prezzi_Dettagli ON Listini_PrezzixRisorse.Piva_SuperUser = Listini_Prezzi_Dettagli.Piva_SuperUser " & vbCrLf)
            Stb.Append(" AND Listini_Prezzi_Dettagli.Piva = Listini_PrezzixRisorse.Piva   " & vbCrLf)
            Stb.Append(" AND Listini_Prezzi_Dettagli.Listino_Cod = Listini_PrezzixRisorse.Listino_Cod   " & vbCrLf)
            Stb.Append(" AND Listini_Prezzi_Dettagli.Elem_Cod = Listini_PrezzixRisorse.Elem_Cod   " & vbCrLf)
            Stb.Append(" AND Listini_Prezzi_Dettagli.Pro_Cod = Listini_PrezzixRisorse.Pro_Cod   " & vbCrLf)
            Stb.Append(" AND Listini_Prezzi_Dettagli.Mat_Cod = Listini_PrezzixRisorse.Mat_Cod   " & vbCrLf)

            Stb.Append(" INNER JOIN UnitaMisura ON Listini_Prezzi_Dettagli.Udm_Cod = UnitaMisura.Udm_Cod " & vbCrLf)
            Stb.Append(" INNER JOIN CategorieMagazzino ON Listini_PrezzixCategorie.Elem_Cod = CategorieMagazzino.Elem_Cod " & vbCrLf)

            Stb.Append("  " & vbCrLf)
            Stb.Append("  " & vbCrLf)

            Stb.Append(" WHERE Listini_Classi_Prezzi.Piva_SuperUser  = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  " & vbCrLf)

            Stb.Append(" AND Listini_Prezzi_Dettagli.Prezzo <> 0 " & vbCrLf)
            Stb.Append(" AND Listini_PrezzixRisorse.Mat_Cod = 0 " & vbCrLf)

            If Piva <> "" Then
                Stb.Append(" AND Listini_Classi_Prezzi.Piva = '" & Agro_SQL_SaveText(Piva) & "'  " & vbCrLf)
            End If

            If Listino_Classe_Cod <> 0 Then
                Stb.Append(" AND Listini_Classi_Prezzi.Listino_Classe_Cod = " & Agro_SQL_SaveNum(Listino_Classe_Cod) & "  " & vbCrLf)
            End If

            If Tipo_Classe <> 0 Then
                Stb.Append(" AND Listini_Classi_Prezzi.Tipo_Classe = " & Agro_SQL_SaveNum(Tipo_Classe) & "  " & vbCrLf)
            End If

            'default-1
            If Listino_Classe_Padre_Cod <> -1 Then
                Stb.Append(" AND Listini_Classi_Prezzi.Listino_Classe_Padre_Cod = " & Agro_SQL_SaveNum(Listino_Classe_Padre_Cod) & "  " & vbCrLf)
            End If

            '----

            If Listino_Cod <> 0 Then
                Stb.Append(" AND Listini_Prezzi.Listino_Cod = " & Agro_SQL_SaveNum(Listino_Cod) & "  " & vbCrLf)
            End If

            If ChkApplicabilita <> -1 Then
                Stb.Append(" AND Listini_Prezzi.ChkApplicabilita = " & Agro_SQL_SaveNum(ChkApplicabilita) & "  " & vbCrLf)
            End If

            If Tipo_Iva <> 0 Then
                Stb.Append(" AND Listini_Prezzi.Tipo_Iva = " & Agro_SQL_SaveNum(Tipo_Iva) & "  " & vbCrLf)
            End If

            If Tipo_Provvigione <> 0 Then
                Stb.Append(" AND Listini_Prezzi.Tipo_Provvigione = " & Agro_SQL_SaveNum(Tipo_Provvigione) & "  " & vbCrLf)
            End If

            '----

            If Elem_Cod <> 0 Then
                Stb.Append(" AND Listini_PrezzixCategorie.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "  " & vbCrLf)
            End If

            '----

            If Pro_Cod <> 0 Then
                Stb.Append(" AND Listini_PrezzixRisorse.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "  " & vbCrLf)
            End If

            If Mat_Cod <> 0 Then
                Stb.Append(" AND Listini_PrezzixRisorse.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "  " & vbCrLf)
            End If

            If Conto_Terzi <> -1 Then
                Stb.Append(" AND Listini_PrezzixRisorse.Conto_Terzi = " & Agro_SQL_SaveNum(Conto_Terzi) & "  " & vbCrLf)
            End If

            If Cod_Conto_Default <> 0 Then
                Stb.Append(" AND Listini_PrezzixRisorse.Cod_Conto_Default = " & Agro_SQL_SaveNum(Cod_Conto_Default) & "  " & vbCrLf)
            End If

            '----

            If Cal_Cod <> 0 Then
                Stb.Append(" AND Listini_Prezzi_Dettagli.Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "  " & vbCrLf)
            End If

            If Lav_Cod <> 0 Then
                Stb.Append(" AND Listini_Prezzi_Dettagli.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & "  " & vbCrLf)
            End If

            If Chk_Vettore <> -1 Then
                Stb.Append(" AND Listini_Prezzi_Dettagli.ChkVettore = " & Agro_SQL_SaveNum(Chk_Vettore) & "  " & vbCrLf)
            End If

            If Cod_Rapporto <> 0 Then
                Stb.Append(" AND Listini_Prezzi_Dettagli.Cod_Rapporto = " & Agro_SQL_SaveNum(Cod_Rapporto) & "  " & vbCrLf)
            End If

            If Cod_RisUm <> 0 Then
                Stb.Append(" AND Listini_Prezzi_Dettagli.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "  " & vbCrLf)
            End If

            If Mezzo <> -1 Then
                Stb.Append(" AND Listini_Prezzi_Dettagli.Mezzo = " & Agro_SQL_SaveNum(Mezzo) & "  " & vbCrLf)
            End If

            If Udm_Cod <> 0 Then
                Stb.Append(" AND Listini_Prezzi_Dettagli.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "  " & vbCrLf)
            End If

            If Cal_Cod <> 0 Then
                Stb.Append(" AND Listini_Prezzi_Dettagli.Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "  " & vbCrLf)
            End If

            If Cod_Iva <> 0 Then
                Stb.Append(" AND Listini_Prezzi_Dettagli.Cod_Iva = " & Agro_SQL_SaveNum(Cod_Iva) & "  " & vbCrLf)
            End If

            If Cod_Conto_Det_Default <> -1 Then
                Stb.Append(" AND Listini_Prezzi_Dettagli.Cod_Conto_Det_Default = " & Agro_SQL_SaveNum(Cod_Conto_Det_Default) & "  " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo2 <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo2, , objParametri))
            End If

            Stb.Append(" ) " & vbCrLf)

            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                Stb.Append(" ORDER BY Listini_Classi_Prezzi.Tipo_Classe, Listini_Classi_Prezzi.Listino_Classe_Des, Listini_Prezzi.Listino_Des  ")
                Stb.Append(" , CategorieMagazzino.NomeComune , Materie_Prime.Mat_Des  " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
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
