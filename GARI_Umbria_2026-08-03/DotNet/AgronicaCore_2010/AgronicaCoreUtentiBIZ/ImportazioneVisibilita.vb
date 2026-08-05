Imports AgronicaCoreDataProvider
Imports AgronicaCoreModelsSTD.profilazione

''' <summary>
''' Classe per la gestione della visibilità sulle imprese da parte degli utenti.
''' Questa classe gestisce la visibilità delle imprese in base al CUAA (Codice Unico di Avviamento Agricolo).
''' Per far riferiemtno alle imprese tramite la PIVA (Partita IVA) è necessario utilizzare la classe
''' <see cref="Utenti_Visibilita"/>.
''' </summary>
Public Class ImportazioneVisibilita

    Public sub ImportVisibilityNewAgri(username As String, companies As IEnumerable(Of String), params As ObjParams)
        Const fullVizKeyword As String = "REGIONE"

        Dim userViz As New Utenti_Visibilita
        Dim companyCodes As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        Dim user = New BaseUtente(username)

        If not companies.Any then
            userViz.AssegnaVisibilitaNulla(user, params.ObjParametri_Server, params.ObjParametri_Utenti)
        ElseIf companies.Contains(fullVizKeyword) then
            Dim full as New List(Of AgronicaCoreModelsSTD.anagrafiche.IImpresaDto)
            userViz.OverwriteVisibility({user}, full, params)
        else
            dim pivas = companyCodes.Piva_from_CUAA(companies, params.ObjParametri_Server).AsEnumerable.
                Select(Function(row) row("PIVA").ToString).ToList
            Dim comps = pivas.Select(Function(row) New AgronicaCoreModelsSTD.anagrafiche.ImpresaDto With {.piva = row}).ToList
            userViz.OverwriteVisibility({user}, comps, params)
        End If
    End sub

End Class
