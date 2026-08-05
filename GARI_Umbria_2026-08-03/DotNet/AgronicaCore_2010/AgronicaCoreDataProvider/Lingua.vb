Public Class Lingua

    Public Property Lingua_cod As Integer

    Public Property Nome As String

    Public Property Descrizione As String

    Public Property CodiceISO As String

    Public Shared Sub Gias_InizializzaCultura_DaSession()

        If Web.HttpContext.Current.Session Is Nothing Then
            Exit Sub
        End If

        If Web.HttpContext.Current.Session.IsNewSession Then
            Web.HttpContext.Current.Response.Redirect("~/Custom500.aspx")

        Else

            If IsNothing(Web.HttpContext.Current.Session("LinguaCorrente")) Then
                Web.HttpContext.Current.Response.Redirect("~/Custom500.aspx")
            Else
                Dim linguaSession As Lingua = CType(Web.HttpContext.Current.Session("LinguaCorrente"), Lingua)
                System.Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo(linguaSession.CodiceISO)

                Dim objparametri_Server As New AgronicaCoreParametri(CType(Web.HttpContext.Current.Session("ASG_objParametri_Server"), AgronicaCoreParametri))
                objparametri_Server.Lingua_Cod = linguaSession.Lingua_cod
                Web.HttpContext.Current.Session("ASG_objParametri_Server") = objparametri_Server

                If linguaSession.Lingua_cod <> 1 Then
                    Dim objparametri_Utenti As New AgronicaCoreParametri(CType(Web.HttpContext.Current.Session("ASG_objParametri_Utenti"), AgronicaCoreParametri))
                    objparametri_Utenti.Lingua_Cod = linguaSession.Lingua_cod
                    Web.HttpContext.Current.Session("ASG_objParametri_Utenti") = objparametri_Utenti
                End If

            End If

        End If
    End Sub

    ''' <summary>
    ''' Imposta e restituisce il nostro codice lingua interno in base al valore inserito sulla proprietà CodiceISO
    ''' </summary>
    ''' <returns></returns>
    Public Function Calcola_LinguaCod_da_CodiceISO() As Integer
        If CodiceISO Is Nothing Then
            Throw New Exception("Occorre prima valorizzare la proprietà CodiceISO per poter effettuare la conversione")
        End If

        Dim linguaCodCalcolata As Integer

        Select Case CodiceISO
            Case "it"
                linguaCodCalcolata = 1
            Case "en", "en-US", "en-GB"
                linguaCodCalcolata = 2
            Case "fr"
                linguaCodCalcolata = 3
            Case "it-CH"
                linguaCodCalcolata = 4
            Case "pt"
                linguaCodCalcolata = 5
            Case Else
                linguaCodCalcolata = 0
        End Select

        Lingua_cod = linguaCodCalcolata
        Return linguaCodCalcolata
    End Function
End Class