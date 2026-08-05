Imports AgronicaCoreDataProvider
Imports AgronicaCoreModelsSTD.profilazione
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports Newtonsoft.Json
Imports InData.Zoo

Public Class Gruppi_UtenteBiz
    Inherits LogProvider

    Public readonly DEFAULT_GROUP As Integer = 99999999
    Public objPServer As AgronicaCoreParametri
    Public objPUtenti As AgronicaCoreParametri

    Sub New(objPServer As AgronicaCoreParametri, objPUtenti As AgronicaCoreParametri)
        Me.objPServer = objPServer
        Me.objPUtenti = objPUtenti
    End Sub


    Public Function GetGruppiUtenti() As DataTable
        Try
            Dim dal As New Gruppi_Utente_R
            Dim dt = dal.Leggi(0, "", "", objPUtenti)

            Return dt
        Catch ex As Exception
            If objPUtenti IsNot Nothing Then
                Dim routine As String = Reflection.MethodBase.GetCurrentMethod().Name
                Scrivi_LOG(objPUtenti, routine, ex.Message)
            End If

            Throw ex
        End Try
    End Function

    Public Function GetUtentiFromGruppoUtenti(ByVal groups As IEnumerable(Of Integer)) As DataTable
        Dim userGroupReader As New Utenti_xGruppi_Utente_R
        Return userGroupReader.LeggiUtentiDaGruppo(groups, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, objPUtenti)
    End Function

    Public Function SalvaGruppoUtente(gruppo As GruppoUtente, Optional isAtomic As Boolean = True)
        Dim noFiltro = ""
        Dim result = False
        Dim objWrite As New Gruppi_Utente_W
        Dim objRead As New Gruppi_Utente_R
        Try
            If isAtomic Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, Me.objPUtenti)
            End If
            Dim found = objRead.Leggi(gruppo.codice, noFiltro, noFiltro, Me.objPUtenti)
            If found.AsEnumerable.Any Then
                result = objWrite.Modifica(gruppo.codice, gruppo.descrizione,
                              gruppo.Identificativo, noFiltro, Me.objPUtenti)
            Else
                gruppo.codice = CalcolaNuovoCodice()
                result = objWrite.Scrivi(gruppo.codice, gruppo.descrizione, gruppo.Identificativo,
                            AGRODATAINIZIO, AGRODATAFINE, Me.objPUtenti)
            End If
            If isAtomic Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, Me.objPUtenti)
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(Me.objPUtenti)
            End If
        Catch ex As Exception
            If isAtomic Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, Me.objPUtenti)
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(Me.objPUtenti)
            End If
            Throw ex
        End Try
        Return result
    End Function

    Public Function FindGruppoUtente(codice As Integer) As GruppoUtente  
       Dim objRead As New Gruppi_Utente_R  
       Dim found = objRead.Leggi(codice, xFiltroAggiuntivo:=String.Empty, xOrderBy:=String.Empty, Me.objPUtenti)  
       If found.AsEnumerable.Any Then
            Dim row = found.AsEnumerable.First
            Return  New GruppoUtente(row("Gruppi_Utente_cod"), row("Gruppi_Utente_des"))
       Else  
           Return Nothing  
       End If  
    End Function

    Public Function SetUserGroup(username As String, group As Integer, esisteUtente As Boolean) As boolean
        If group > 0 Then
            Dim objGruppi As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_W
            If esisteUtente Then
                objGruppi.Cancella(0, username, "", Me.objPUtenti)
            End If
            return objGruppi.Scrivi(group, username, AGRODATAINIZIO, AGRODATAFINE, Me.objPUtenti)
        End If
        Return false
    End Function



    Private Function CalcolaNuovoCodice() As Integer
        Dim objRead As New Gruppi_Utente_R
        Dim tuttiGruppi = 0
        Dim gruppoDefault = 99999999
        Dim codiceDecr = " Gruppi_Utente_cod desc "
        Dim eccettoDefault = " Gruppi_Utente_cod != " & gruppoDefault & " "
        Dim maxItem = objRead.Leggi(tuttiGruppi, eccettoDefault, codiceDecr, Me.objPUtenti).
            AsEnumerable.FirstOrDefault()
        If IsNothing(maxItem) Then
            Return 1
        ElseIf maxItem.Item("Gruppi_Utente_cod") = (gruppoDefault - 1) Then
            Return gruppoDefault + 2
        Else
            Return maxItem.Item("Gruppi_Utente_cod") + 1
        End If
    End Function

End Class
