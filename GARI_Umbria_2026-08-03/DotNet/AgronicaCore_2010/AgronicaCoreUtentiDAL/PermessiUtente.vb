Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports System.Web


Public Class PermessiUtente
    Private PersmessiLista As Hashtable

    Public Sub New()
        ''costrutture che legge da session o da db
        Dim objPermessiUtente = HttpContext.Current.Session("PersmessiLista")
        If (IsNothing(objPermessiUtente)) Then
            'se è nullo lo leggo da db 

            PersmessiLista = New Hashtable
            Dim items As Array
            items = System.Enum.GetValues(GetType(enum_Security_Attivita))

            Dim names As Array
            names = System.Enum.GetNames(GetType(enum_Security_Attivita))

            'leggo da db
            Dim objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))
            Dim objU As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            Dim dt As DataTable = objU.LeggixOggetto(HttpContext.Current.Session("ASG_Utente_Username"), Date.Now, objParametri_Utenti)


            Dim i As Integer = 0
            Dim item As String
            For Each item In items
                Dim nome As String = names(i)

                Dim n As New TipoPermesso

                Dim dr() As DataRow = dt.Select("ID_Attivita =" & item)

                For Each d In dr
                    If d.Item("ID_operazione") = 0 Then
                        n.Lettura = True
                    End If
                    If d.Item("ID_operazione") = 2 Then
                        n.Scrittura = True
                    End If
                Next

                PersmessiLista.Add(nome, n)
                i = i + 1
            Next

            'salvo in session 
            HttpContext.Current.Session("PersmessiLista") = PersmessiLista
        Else
            ' lo leggo da session 
            PersmessiLista = objPermessiUtente
        End If
    End Sub

    Public Function getPermesso(ByVal tipo As enum_Security_Attivita) As TipoPermesso
        Dim nome As String = System.Enum.GetName(GetType(enum_Security_Attivita), CInt(tipo))
        Return PersmessiLista(nome)
    End Function

End Class

'''di default imposto i permessi a false
Public Class TipoPermesso
    Public Lettura As Boolean
    Public Scrittura As Boolean

    Public Sub New()
        Lettura = False
        Scrittura = False
    End Sub
End Class