Imports AgronicaCoreDataProvider

Public Class Visite_Categoria2_R

    Public Function Leggi(ByRef objParametri As AgronicaCoreParametri, ID_Categoria1 As Integer) As DataTable

        Dim r As New AgronicaCoreVisiteDAL.Visite_Categoria2_R
        Return r.Leggi(0, ID_Categoria1, "", objParametri)

    End Function

    Public Function TestNomeCategoria2GiaEsistente(ByRef objParametri As AgronicaCoreParametri, ID_Categoria1 As Integer, nome As String) As Boolean
        Dim r As New AgronicaCoreVisiteDAL.Visite_Categoria2_R
        Dim dt As DataTable = r.Leggi(0, ID_Categoria1, nome, objParametri)

        'Se c'è almeno un record vuol dire che il nome esiste già per quella categoria padre
        Return (dt.Rows.Count > 0)

    End Function

    Public Function TestCategoria2GiaInUso(ByRef objParametri As AgronicaCoreParametri, ID_Categoria1 As Integer) As Boolean
        'Dim cat2_R As New AgronicaCoreVisiteDAL.Visite_Categoria2_R
        'Dim dt As DataTable = cat2_R.Leggi(0, ID_Categoria1, "", objParametri)

        ''Se c'è almeno un record vuol dire che la categoria ha delle sottocategorie
        'Return (dt.Rows.Count > 0)
        Return True

    End Function

End Class

Public Class Visite_Categoria2_W

    Public Function Aggiungi(ByRef objParametri As AgronicaCoreParametri,
                              ByVal ID_Categoria1 As Integer,
                              ByVal Nome As String
                              ) As String

        'Verifico che non esista già un elemento con quel nome
        Dim r As New AgronicaCoreVisiteBIZ.Visite_Categoria2_R
        If r.TestNomeCategoria2GiaEsistente(objParametri, ID_Categoria1, Nome) Then
            Return "Errore: Esiste già una categoria con questo nome"
        End If

        Dim res As String = ""

        Try
            'apro una transazione
            ConnessioniTransazioni.ApriConnessione(True, objParametri)

            'Creo la nuova Chiave
            Dim seq As New Agro_Sequenze()
            Dim ID_Categoria2 As Integer = seq.NuovoId_Tabella("Visiste_ID_Categoria2", 0, 2000000000, objParametri)

            'Salvo il nuovo elemento
            Dim w As New AgronicaCoreVisiteDAL.Visite_Categoria2_W
            Dim esito As Boolean = w.Scrivi(ID_Categoria1, ID_Categoria2, Nome, objParametri)

            If esito = False Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri) 'Flag_Commit1_Rollback2
                res = "Impossibile salvare la Categoria di Visita"
                Exit Try
            End If

            'Se è andato tutto bene
            ConnessioniTransazioni.ChiudiTransazione(1, objParametri) 'Flag_Commit1_Rollback2

        Catch ex As Exception
            res = ex.Message
            ConnessioniTransazioni.ChiudiTransazione(2, objParametri) 'Flag_Commit1_Rollback2
        End Try

        Return res

    End Function

    Public Function Modifica(ByRef objParametri As AgronicaCoreParametri,
                              ByVal ID_Categoria1 As Integer,
                              ByVal ID_Categoria2 As Integer,
                              ByVal Nome As String
                              ) As String

        'Verifico che non esista già un elemento con quel nome
        Dim r As New AgronicaCoreVisiteBIZ.Visite_Categoria2_R
        If r.TestNomeCategoria2GiaEsistente(objParametri, ID_Categoria1, Nome) Then
            Return "Errore: Esiste già una categoria con questo nome"
        End If

        Dim res As String = ""

        Try
            'apro una transazione
            ConnessioniTransazioni.ApriConnessione(True, objParametri)

            'Modifico l'elemento
            Dim w As New AgronicaCoreVisiteDAL.Visite_Categoria2_W
            Dim esito As Boolean = w.Modifica(ID_Categoria2, Nome, objParametri)

            If esito = False Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri) 'Flag_Commit1_Rollback2
                res = "Impossibile Modificare la Categoria di Visita"
                Exit Try
            End If

            'Se è andato tutto bene
            ConnessioniTransazioni.ChiudiTransazione(1, objParametri) 'Flag_Commit1_Rollback2

        Catch ex As Exception
            res = ex.Message
            ConnessioniTransazioni.ChiudiTransazione(2, objParametri) 'Flag_Commit1_Rollback2
        End Try

        Return res

    End Function

    Public Function Cancella(ByRef objParametri As AgronicaCoreParametri,
                              ByVal ID_Categoria2 As Integer
                              ) As String

        'Verifico che la categoria non abbia sottocategorie (e che quindi non siano in uso per qualche visita)
        Dim r As New AgronicaCoreVisiteBIZ.Visite_Categoria2_R
        If r.TestCategoria2GiaInUso(objParametri, ID_Categoria2) Then
            Return "Errore: La Categoria è in uso per qualche visita, impossibile eliminare"
        End If

        Dim res As String = ""

        Try
            'apro una transazione
            ConnessioniTransazioni.ApriConnessione(True, objParametri)

            'Cancello l'elemento
            Dim w As New AgronicaCoreVisiteDAL.Visite_Categoria2_W
            Dim esito As Boolean = w.Cancella(ID_Categoria2, objParametri)

            If esito = False Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri) 'Flag_Commit1_Rollback2
                res = "Impossibile Cancellare la Categoria di Visita"
                Exit Try
            End If

            'Se è andato tutto bene
            ConnessioniTransazioni.ChiudiTransazione(1, objParametri) 'Flag_Commit1_Rollback2

        Catch ex As Exception
            res = ex.Message
            ConnessioniTransazioni.ChiudiTransazione(2, objParametri) 'Flag_Commit1_Rollback2
        End Try

        Return res

    End Function

End Class
