Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class CategTipologiaDocumentiXUtenti
    Inherits AgronicaCoreDataProvider.DataProvider

    'Mostra solo le Categorie e Tipologie che l'utente è stato autorizzato a visualizzare, per una determinata piva.
    'Se non trova permessi per quell' utente e per quella determinata piva prova a cercare per la piva padre. 

    'Public Sub Controlla_Permessi(ByVal piva As String, ByRef DT As DataTable, ByVal ControllaPermessiDaRicercaDocumenti As Boolean, ByVal objParametri_Server As AgronicaCoreParametri)

    '    Dim Utente_Username = objParametri_Server.UtenteUsername

    '    Dim SuperUser_Username = objParametri_Server.SuperUserUsername

    '    'Se l'utente è SuperUser ha tutti i permessi 
    '    If Utente_Username <> SuperUser_Username Then

    '        If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 AndAlso (DT.Columns.Contains("ID_Area") OrElse DT.Columns.Contains("ID_Categoria")) Then

    '            Dim DT_Permessi As New DataTable

    '            'Controllo se sono stati inseriti dei Permessi
    '            'Se non è stato inserito nessun permesso si possono tutte le Categorie e Tipologie (Da Chiedere!!!)
    '            Dim obj_CategTipologiaDocumentiXUtenti As New AgronicaCoreScadenziario.CategTipologiaDocumentiXUtenti_R

    '            DT_Permessi = obj_CategTipologiaDocumentiXUtenti.Leggi("", 0, Nothing, enumSelezioneVariabile.Selezione_TabellaCompleta, objParametri_Server, Nothing, Nothing)

    '            If Not IsNothing(DT_Permessi) AndAlso DT_Permessi.Rows.Count > 0 Then

    '                Dim DT_Appoggio = DT.Copy

    '                DT.Clear()

    '                If ControllaPermessiDaRicercaDocumenti Then

    '                    'TODO Da chiedere con Stefano come fare su COPROB tira fuori più di 4000 righe anche se faccio il distinct per Piva,
    '                    'ID_Area e ID_Tipologia

    '                    Dim DT_Distinct = DT_Appoggio.DefaultView.ToTable(True, {"Piva", "ID_Categoria", "ID_Tipologia"})


    '                    'Provo con la Piva Padre se non ho trovato nulla con la Piva corrente

    '                    Dim PivaPadre As String = String.Empty

    '                    'TODO da chiedere ma dovresti utlizzare Ricava_Stringa_PivePadre e prendere solo la prima piva padre
    '                    Dim gi As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R
    '                    Dim DT_PivePadri = gi.LeggixFiglio("",
    '                                                    enumSelezioneVariabile.Selezione_TabellaCompleta,
    '                                                    "", "",
    '                                                    objParametri_Server)

    '                    If Not IsNothing(DT_Distinct) Then

    '                        Dim DT_Permessi_New As New DataTable

    '                        DT_Permessi_New = DT_Appoggio.Clone()

    '                        For Each DR As DataRow In DT_Distinct.Rows

    '                            piva = DR("Piva").ToString()

    '                            If Not String.IsNullOrEmpty(piva) Then

    '                                Dim DT_PermessiXPiva As New DataTable

    '                                Dim DT_PermessiXPivaPadre As New DataTable

    '                                Dim DR_PermessiXPiva = DT_Permessi.Select("Piva = '" & piva & "' And Username = '" & Utente_Username & "' And Autorizzato = 1")

    '                                If Not IsNothing(DR_PermessiXPiva) AndAlso DR_PermessiXPiva.Length > 0 Then

    '                                    DT_PermessiXPiva = DR_PermessiXPiva.CopyToDataTable()


    '                                    Dim Controlla_Tipologia As Boolean = False

    '                                    Dim ID_Area = 0

    '                                    If DR.Table.Columns.Contains("ID_Categoria") Then
    '                                        ID_Area = CInt(DR("ID_Categoria").ToString())
    '                                    End If

    '                                    Dim ID_Tipologia = 0

    '                                    If DR.Table.Columns.Contains("ID_Tipologia") Then
    '                                        ID_Tipologia = CInt(DR("ID_Tipologia").ToString())
    '                                        Controlla_Tipologia = True
    '                                    End If

    '                                    If TrovaPermessoCategoriaTipologia(ID_Area, ID_Tipologia, Controlla_Tipologia, DT_PermessiXPiva) Then
    '                                        DT_Permessi_New.ImportRow(DR)
    '                                    End If

    '                                Else

    '                                    If Not IsNothing(DT_PivePadri) AndAlso DT_PivePadri.Rows.Count > 0 Then

    '                                        Dim DR_PivePadri = DT_PivePadri.Select("Figlio = '" & piva & "'")

    '                                        If Not IsNothing(DR_PivePadri) AndAlso DR_PivePadri.Length > 0 Then

    '                                            PivaPadre = DR_PivePadri(0)("Padre").ToString()

    '                                            Dim DR_PermessiXPivaPadre = DT_Permessi.Select("Piva = '" & PivaPadre & "' And Username = '" & Utente_Username & "' And Autorizzato = 1")

    '                                            If Not IsNothing(DR_PermessiXPivaPadre) AndAlso DR_PermessiXPivaPadre.Length > 0 Then

    '                                                DT_PermessiXPivaPadre = DR_PermessiXPivaPadre.CopyToDataTable()

    '                                                For Each DR_3 As DataRow In DT_Appoggio.Rows

    '                                                    Dim Controlla_Tipologia As Boolean = False

    '                                                    Dim ID_Area = 0

    '                                                    If DR_3.Table.Columns.Contains("ID_Area") Then
    '                                                        ID_Area = CInt(DR_3("ID_Area").ToString())
    '                                                    Else
    '                                                        If DR_3.Table.Columns.Contains("ID_Categoria") Then
    '                                                            ID_Area = CInt(DR_3("ID_Categoria").ToString())
    '                                                        End If
    '                                                    End If

    '                                                    Dim ID_Tipologia = 0

    '                                                    If DR_3.Table.Columns.Contains("ID_Tipologia") Then
    '                                                        ID_Tipologia = CInt(DR_3("ID_Tipologia").ToString())
    '                                                        Controlla_Tipologia = True
    '                                                    End If

    '                                                    If TrovaPermessoCategoriaTipologia(ID_Area, ID_Tipologia, Controlla_Tipologia, DT_PermessiXPivaPadre) Then
    '                                                        DT_Permessi_New.ImportRow(DR_3)
    '                                                    End If

    '                                                Next

    '                                            End If

    '                                        End If

    '                                    End If

    '                                End If

    '                            Else

    '                                Exit Sub

    '                            End If

    '                        Next


    '                        If Not IsNothing(DT_Permessi_New) AndAlso DT_Permessi_New.Rows.Count > 0 Then

    '                            For Each DR In DT_Appoggio.Rows

    '                                Dim CheckPermesso = DT_Permessi_New.Select("ID_Categoria = " & DR("ID_Categoria") & " And ID_Tipologia = " & DR("ID_Tipologia") & "")

    '                                If Not IsNothing(CheckPermesso) AndAlso CheckPermesso.Length > 0 Then
    '                                    DT.ImportRow(DR)
    '                                End If

    '                            Next

    '                        End If

    '                    End If

    '                Else

    '                    If Not String.IsNullOrEmpty(piva) Then

    '                        Dim DT_PermessiXPiva As New DataTable

    '                        Dim DT_PermessiXPivaPadre As New DataTable


    '                        Dim DR_PermessiXPiva = DT_Permessi.Select("Piva = '" & piva & "' And Username = '" & Utente_Username & "' And Autorizzato = 1")

    '                        If Not IsNothing(DR_PermessiXPiva) AndAlso DR_PermessiXPiva.Length > 0 Then

    '                            DT_PermessiXPiva = DR_PermessiXPiva.CopyToDataTable()

    '                            For Each DR As DataRow In DT_Appoggio.Rows

    '                                Dim Controlla_Tipologia As Boolean = False

    '                                Dim ID_Area = 0

    '                                If DR.Table.Columns.Contains("ID_Area") Then
    '                                    ID_Area = CInt(DR("ID_Area").ToString())
    '                                Else
    '                                    If DR.Table.Columns.Contains("ID_Categoria") Then
    '                                        ID_Area = CInt(DR("ID_Categoria").ToString())
    '                                    End If
    '                                End If

    '                                Dim ID_Tipologia = 0

    '                                If DR.Table.Columns.Contains("ID_Tipologia") Then
    '                                    ID_Tipologia = CInt(DR("ID_Tipologia").ToString())
    '                                    Controlla_Tipologia = True
    '                                End If

    '                                If TrovaPermessoCategoriaTipologia(ID_Area, ID_Tipologia, Controlla_Tipologia, DT_PermessiXPiva) Then
    '                                    DT.ImportRow(DR)
    '                                End If

    '                            Next

    '                        Else

    '                            'Provo con la Piva Padre se non ho trovato nulla con la Piva che era stata passata

    '                            Dim PivaPadre As String = String.Empty

    '                            'TODO da chiedere ma dovresti utlizzare Ricava_Stringa_PivePadre e prendere solo la prima piva padre
    '                            Dim gi As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R
    '                            Dim DT_PivePadri = gi.LeggixFiglio(piva,
    '                                                                    enumSelezioneVariabile.Selezione_TabellaCompleta,
    '                                                                    "", "",
    '                                                                    objParametri_Server)

    '                            If Not IsNothing(DT_PivePadri) AndAlso DT_PivePadri.Rows.Count > 0 Then
    '                                PivaPadre = DT_PivePadri(0)("Padre").ToString()
    '                            End If

    '                            Dim DR_PermessiXPivaPadre = DT_Permessi.Select("Piva = '" & PivaPadre & "' And Username = '" & Utente_Username & "' And Autorizzato = 1")

    '                            If Not IsNothing(DR_PermessiXPivaPadre) AndAlso DR_PermessiXPivaPadre.Length > 0 Then

    '                                DT_PermessiXPivaPadre = DR_PermessiXPivaPadre.CopyToDataTable()

    '                                For Each DR As DataRow In DT_Appoggio.Rows

    '                                    Dim Controlla_Tipologia As Boolean = False

    '                                    Dim ID_Area = 0

    '                                    If DR.Table.Columns.Contains("ID_Area") Then
    '                                        ID_Area = CInt(DR("ID_Area").ToString())
    '                                    Else
    '                                        If DR.Table.Columns.Contains("ID_Categoria") Then
    '                                            ID_Area = CInt(DR("ID_Categoria").ToString())
    '                                        End If
    '                                    End If

    '                                    Dim ID_Tipologia = 0

    '                                    If DR.Table.Columns.Contains("ID_Tipologia") Then
    '                                        ID_Tipologia = CInt(DR("ID_Tipologia").ToString())
    '                                        Controlla_Tipologia = True
    '                                    End If

    '                                    If TrovaPermessoCategoriaTipologia(ID_Area, ID_Tipologia, Controlla_Tipologia, DT_PermessiXPivaPadre) Then
    '                                        DT.ImportRow(DR)
    '                                    End If

    '                                Next

    '                            End If

    '                        End If

    '                    Else

    '                        Exit Sub

    '                    End If

    '                End If

    '            End If

    '        End If

    '    End If

    'End Sub


    'Private Function TrovaPermessoCategoriaTipologia(ByVal ID_Area As Integer, ByVal ID_Tipologia As Integer, ByVal Controlla_Tipologia As Boolean, ByRef DT_PermessiFiltratiXPiva As DataTable) As Boolean

    '    Dim Trovato As Boolean = False

    '    If Not IsNothing(DT_PermessiFiltratiXPiva) Then

    '        Dim DR_PermessiFiltratiXPivaeCategoria = DT_PermessiFiltratiXPiva.Select("ID_Categoria = " & ID_Area)

    '        If Not IsNothing(DR_PermessiFiltratiXPivaeCategoria) AndAlso DR_PermessiFiltratiXPivaeCategoria.Length > 0 Then

    '            If Controlla_Tipologia Then

    '                Dim DT_PermessiFiltratiXPivaCategoria = DR_PermessiFiltratiXPivaeCategoria.CopyToDataTable()

    '                Dim DR_PermessiFiltratiXPivaCategoriaTipologia = DT_PermessiFiltratiXPivaCategoria.Select("ID_Categoria = " & ID_Area & " And ID_Tipologia = " & ID_Tipologia)

    '                If Not IsNothing(DR_PermessiFiltratiXPivaCategoriaTipologia) AndAlso DR_PermessiFiltratiXPivaCategoriaTipologia.Length > 0 Then

    '                    Trovato = True

    '                Else

    '                    If ID_Tipologia <> 0 Then

    '                        Dim DR_PermessiFiltratiXPivaCategoriaTutteTipologie = DT_PermessiFiltratiXPivaCategoria.Select("ID_Categoria = " & ID_Area & " And ID_Tipologia = 0")

    '                        If Not IsNothing(DR_PermessiFiltratiXPivaCategoriaTutteTipologie) AndAlso DR_PermessiFiltratiXPivaCategoriaTutteTipologie.Length > 0 Then

    '                            Trovato = True

    '                        End If

    '                    End If

    '                End If

    '            Else

    '                Trovato = True

    '            End If

    '        End If

    '    End If

    '    Return Trovato

    'End Function

End Class
