Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Listini_PrezzixContatti_R
    Inherits DataProvider

    Private Const tabellaGestita As String = "Listini_PrezzixContatti"

    Public Function Leggi(
            ByVal piva As String,
            ByRef objParametri As AgronicaCoreParametri,
            Optional ByVal listinoCod As Integer = 0,
            Optional ByVal saCod As Integer = 0,
            Optional ByVal codRapporto As Integer = 0,
            Optional ByVal codContatto As String = "",
            Optional ByVal codContattoProduttore As String = "",
            Optional ByVal tipoClasse As Integer = 0,
            Optional ByVal usernameCreazione As String = ""
        ) As DataTable

        Dim methodInfo = Reflection.MethodBase.GetCurrentMethod()
        Dim NomeRoutine As String = methodInfo.ReflectedType.FullName & "." & methodInfo.Name

        Dim dtRisp As DataTable

        Dim pivaSuperUser = objParametri.PivaSuperUser
        Dim StrSQL As New Text.StringBuilder

        Dim arrPK = New String() {
            tabellaGestita & ".Piva_SuperUser",
            tabellaGestita & ".Piva",
            "CAST(" & tabellaGestita & ".Listino_Cod AS varchar)",
            "CAST(" & tabellaGestita & ".Sa_Cod AS varchar)",
            "CAST(" & tabellaGestita & ".Cod_Rapporto AS varchar)",
            tabellaGestita & ".Cod_Contatto",
            tabellaGestita & ".Cod_Contatto_Produttore"
        }

        StrSQL.AppendLine("SELECT " & tabellaGestita & ".*,")
        StrSQL.AppendLine(String.Join(" + '_' + ", arrPK) & " AS Chiave_AssocListino,")
        StrSQL.AppendLine("Listini_Prezzi.Listino_Des, Listini_Classi_Prezzi.Listino_Classe_Des, Listini_Classi_Prezzi.Tipo_Classe,")
        StrSQL.AppendLine("COALESCE(Centri_Aziendali.Sa_Nome, '') AS Sa_Nome, COALESCE(Rapporti_Contabili.Rapporto_Des, '') AS Rapporto_Des,")
        StrSQL.AppendLine("LTRIM(COALESCE(Clienti_Fornitori.Rag_Soc, '') + COALESCE(Clienti_Fornitori.Cognome, '') + ' ' + COALESCE(Clienti_Fornitori.Nome, '')) As Des_Cliente_Fornitore,")
        StrSQL.AppendLine("LTRIM(COALESCE(Produttori.Rag_Soc, '') + COALESCE(Produttori.Cognome, '') + ' ' + COALESCE(Produttori.Nome, '')) As Des_Produttore")
        StrSQL.AppendLine("FROM " & tabellaGestita)
        StrSQL.AppendLine("INNER JOIN Listini_Prezzi")
        StrSQL.Append("  ON " & tabellaGestita & ".Piva_SuperUser = Listini_Prezzi.Piva_SuperUser AND " & tabellaGestita & ".Piva = Listini_Prezzi.Piva")
        StrSQL.AppendLine("  AND " & tabellaGestita & ".Listino_Cod = Listini_Prezzi.Listino_Cod")
        StrSQL.AppendLine("INNER JOIN Listini_Classi_Prezzi")
        StrSQL.AppendLine("  ON Listini_Prezzi.Listino_Classe_Cod = Listini_Classi_Prezzi.Listino_Classe_Cod")
        StrSQL.AppendLine("LEFT JOIN Centri_Aziendali")
        StrSQL.AppendLine("  ON " & tabellaGestita & ".Piva = Centri_Aziendali.Piva AND " & tabellaGestita & ".Sa_Cod = Centri_Aziendali.Sa_Cod")
        StrSQL.AppendLine("LEFT JOIN Rapporti_Contabili")
        StrSQL.AppendLine("  ON " & tabellaGestita & ".Piva = Rapporti_Contabili.Piva AND " & tabellaGestita & ".Cod_Rapporto = Rapporti_Contabili.Cod_Rapporto")
        StrSQL.AppendLine("LEFT JOIN Contatti AS Clienti_Fornitori")
        StrSQL.AppendLine("  ON (" & tabellaGestita & ".Piva = Clienti_Fornitori.Piva OR Clienti_Fornitori.Sa_Cod = -1) AND " & tabellaGestita & ".Cod_Contatto = Clienti_Fornitori.Cod_Contatto")
        StrSQL.AppendLine("LEFT JOIN Contatti AS Produttori")
        StrSQL.AppendLine("  ON (" & tabellaGestita & ".Piva = Produttori.Piva OR Produttori.Sa_Cod = -1) AND " & tabellaGestita & ".Cod_Contatto_Produttore = Produttori.Cod_Contatto")
        StrSQL.AppendLine("WHERE " & tabellaGestita & ".Piva_SuperUser = '" & Agro_SQL_SaveText(pivaSuperUser) & "' AND " & tabellaGestita & ".Piva = '" & Agro_SQL_SaveText(piva) & "'")

        If listinoCod <> 0 Then
            StrSQL.AppendLine("AND " & tabellaGestita & ".Listino_Cod = " & Agro_SQL_SaveNum(listinoCod))
        End If

        If saCod <> 0 Then
            StrSQL.AppendLine("AND " & tabellaGestita & ".Sa_Cod = " & Agro_SQL_SaveNum(saCod))
        End If

        If codRapporto <> 0 Then
            StrSQL.AppendLine("AND " & tabellaGestita & ".Cod_Rapporto = " & Agro_SQL_SaveNum(codRapporto))
        End If

        If codContatto <> "" Then
            StrSQL.AppendLine("AND " & tabellaGestita & ".Cod_Contatto = '" & Agro_SQL_SaveText(codContatto) & "'")
        End If

        If codContattoProduttore <> "" Then
            StrSQL.AppendLine("AND " & tabellaGestita & ".Cod_Contatto_Produttore = '" & Agro_SQL_SaveText(codContattoProduttore) & "'")
        End If

        If usernameCreazione <> "" Then
            StrSQL.AppendLine("AND " & tabellaGestita & ".Username_Creazione = '" & Agro_SQL_SaveText(usernameCreazione) & "'")
        End If

        If tipoClasse <> 0 Then
            StrSQL.AppendLine("AND Listini_Classi_Prezzi.Tipo_Classe = " & Agro_SQL_SaveNum(tipoClasse))
        End If

        dtRisp = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
        
        'Per debug: 
        'Dim listNomiColonne As New List(Of String)
        'For Each columnsDt As DataColumn In dtRisp.Columns
        '    listNomiColonne.Add(columnsDt.ColumnName & " - " & columnsDt.DataType.ToString)
        'Next
        'Dim nomiColonne = String.Join(vbCrLf, listNomiColonne)

        Return dtRisp
    End Function

End Class

Public Class Listini_PrezzixContatti_W
    Inherits DataProvider

    Private Const tabellaGestita As String = "Listini_PrezzixContatti"

    Public Function Insert(
            ByVal piva As String,
            ByVal listinoCod As Integer,
            ByVal saCod As Integer,
            ByVal codRapporto As Integer,
            ByVal codContatto As String,
            ByVal codContattoProduttore As String,
            ByRef objParametri As AgronicaCoreParametri,
            Optional ByVal userName As String = ""
        ) As Boolean
                
        Dim methodInfo = Reflection.MethodBase.GetCurrentMethod()
        Dim NomeRoutine As String = methodInfo.ReflectedType.FullName & "." & methodInfo.Name

        Dim risp As Boolean

        Dim pivaSuperUser = objParametri.PivaSuperUser
        Dim StrSQL As New Text.StringBuilder
        Dim utenteUserName As String = objParametri.UtenteUsername

        If Not String.IsNullOrEmpty(userName) Then
            utenteUserName = userName
        End If

        piva = UtilityProvider.Agro_SQL_SaveText(piva)
        listinoCod = UtilityProvider.Agro_SQL_SaveNum(listinoCod)
        saCod = UtilityProvider.Agro_SQL_SaveNum(saCod)
        codRapporto = UtilityProvider.Agro_SQL_SaveNum(codRapporto)
        codContatto = UtilityProvider.Agro_SQL_SaveText(codContatto)
        codContattoProduttore = UtilityProvider.Agro_SQL_SaveText(codContattoProduttore)
        utenteUserName = UtilityProvider.Agro_SQL_SaveText(utenteUserName)

        Dim arrCampiInsert = New String() {
            "Piva_SuperUser",
            "Piva",
            "Listino_Cod",
            "Sa_Cod",
            "Cod_Rapporto",
            "Cod_Contatto",
            "Cod_Contatto_Produttore",
            "Username_Creazione",
            "Username_Modifica"
        }
        Dim arrValues = New String() {
            "'" & pivaSuperUser & "'",
            "'" & piva & "'",
            listinoCod,
            saCod,
            codRapporto,
            "'" & codContatto & "'",
            "'" & codContattoProduttore & "'",
            "'" & utenteUserName & "'",
            "'" & utenteUserName & "'"
        }
        StrSQL.AppendLine("IF NOT EXISTS (SELECT 1 FROM " & tabellaGestita & " WHERE ")
        StrSQL.AppendLine("    Piva_SuperUser = '" & pivaSuperUser & "' AND")
        StrSQL.AppendLine("    Piva = '" & piva & "' AND")
        StrSQL.AppendLine("    Listino_Cod = " & listinoCod & " AND")
        StrSQL.AppendLine("    Sa_Cod = " & saCod & " AND")
        StrSQL.AppendLine("    Cod_Rapporto = " & codRapporto & " AND")
        StrSQL.AppendLine("    Cod_Contatto = '" & codContatto & "' AND")
        StrSQL.AppendLine("    Cod_Contatto_Produttore = '" & codContattoProduttore & "')")
        StrSQL.AppendLine("INSERT INTO " & tabellaGestita & "(")
        StrSQL.AppendLine("    " & String.Join(vbCrLf & "   ,", arrCampiInsert))
        StrSQL.AppendLine(")")
        StrSQL.AppendLine("VALUES(")
        StrSQL.AppendLine("    " & String.Join(vbCrLf & "   ,", arrValues))
        StrSQL.AppendLine(")")

        risp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString(), NomeRoutine)

        Return risp
    End Function

    Public Function Delete(
            ByVal piva As String,
            ByVal listinoCod As Integer,
            ByVal saCod As Integer,
            ByVal codRapporto As Integer,
            ByVal codContatto As String,
            ByVal codContattoProduttore As String,
            ByRef objParametri As AgronicaCoreParametri
        ) As Boolean

        Dim methodInfo = Reflection.MethodBase.GetCurrentMethod()
        Dim NomeRoutine As String = methodInfo.ReflectedType.FullName & "." & methodInfo.Name

        Dim risp As Boolean

        Dim pivaSuperUser = objParametri.PivaSuperUser
        Dim StrSQL As New Text.StringBuilder

        StrSQL.AppendLine("DELETE FROM " & tabellaGestita)
        StrSQL.AppendLine("WHERE")
        StrSQL.AppendLine("    Piva_SuperUser = '" & pivaSuperUser & "' AND")
        StrSQL.AppendLine("    Piva = '" & Agro_SQL_SaveText(piva) & "' AND")
        StrSQL.AppendLine("    Listino_Cod = " & Agro_SQL_SaveNum(listinoCod) & " AND")
        StrSQL.AppendLine("    Sa_Cod = " & Agro_SQL_SaveNum(saCod) & " AND")
        StrSQL.AppendLine("    Cod_Rapporto = " & Agro_SQL_SaveNum(codRapporto) & " AND")
        StrSQL.AppendLine("    Cod_Contatto = '" & Agro_SQL_SaveText(codContatto) & "' AND")
        StrSQL.AppendLine("    Cod_Contatto_Produttore = '" & Agro_SQL_SaveText(codContattoProduttore) & "'")

        risp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString(), NomeRoutine)

        Return risp
    End Function

End Class