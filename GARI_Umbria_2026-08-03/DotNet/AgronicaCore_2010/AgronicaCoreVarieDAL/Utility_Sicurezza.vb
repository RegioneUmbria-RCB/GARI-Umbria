Imports System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder
Imports System.Web
Imports System.Configuration
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports Newtonsoft.Json.Linq

Public Class Utility_Sicurezza

    ''' <summary>
    ''' Restituisce true se è attiva la criptazione della stringa di connessione
    ''' </summary>
    ''' <param name="ID_DB"></param>
    ''' <param name="objParametri_Super_Server"></param>
    ''' <returns>Criptazione attiva true/false</returns>
    Public Shared Function IsEncryptedConnection(ByRef objParametri_Super_Server As AgronicaCoreParametri) As Boolean
        Dim configurazioneSiti As New Configurazione_Siti_R
        Dim cryptConnection As String = configurazioneSiti.Leggi_ValoreNonCacheable("UserPwdConnectionString_toCrypt", "", objParametri_Super_Server)
        Return Not String.IsNullOrEmpty(cryptConnection) AndAlso LCase(cryptConnection) <> "false"
    End Function

    ''' <summary>
    ''' Restituisce la stringa di connessione relativa all'ID_DB passato in input
    ''' Se configurato effettua criptazione e decriptazione dei campi 'UserId' e 'Password'
    ''' </summary>
    ''' <param name="ID_DB"></param>
    ''' <param name="objParametri_Super_Server"></param>
    ''' <returns>Stringa di connessione al DB</returns>
    Public Shared Function Leggi_Stringa_Connessione(ByVal ID_DB As Integer, ByRef objParametri_Super_Server As AgronicaCoreParametri) As String

        If Sicurezza.ExistStringaConnessione(ID_DB) Then
            'connectionString = Sicurezza.GetStringaConnessione(ID_DB)
            Return CStr(ID_DB)
        End If

        If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("DEBUG_LogUMAObj")) Then
            Dim filePath As String = HttpContext.Current.Server.MapPath("~/DebugObjParamUMA.txt")
            Using fstr As IO.FileStream = IO.File.Open(filePath, IO.FileMode.Append, IO.FileAccess.Write, IO.FileShare.ReadWrite)
                Dim sw As New IO.StreamWriter(fstr)
                sw.WriteLine("Utility_Sicurezza Leggi_String_Connessione: " + If(IsNothing(objParametri_Super_Server), "nope", objParametri_Super_Server.ToString))
                sw.Flush()
                sw.Dispose()
            End Using
        End If
        Return Sicurezza.Leggi_Stringa_Connessione(ID_DB, objParametri_Super_Server)

    End Function

    ''' <summary>
    ''' Restituisce le stringhe di connessione leggendole dal super server
    ''' Se configurato effettua criptazione e decriptazione dei campi 'UserId' e 'Password'
    ''' </summary>
    ''' <param name="objParametri_Super_Server"></param>
    ''' <returns>Stringhe di connessione ai DB</returns>
    Public Shared Function Leggi_Stringhe_Connessione(ByRef objParametri_Super_Server As AgronicaCoreParametri) As Dictionary(Of String, String)

        Dim stringheConnessione As New Dictionary(Of String, String)

        Dim configurazioneSiti As New Configurazione_Siti_R
        Dim cryptKey As String = configurazioneSiti.Leggi_ValoreNonCacheable("cr", "", objParametri_Super_Server)
        Dim cryptConnection As String = configurazioneSiti.Leggi_ValoreNonCacheable("UserPwdConnectionString_toCrypt", "", objParametri_Super_Server)

        Dim connessioni As New Connessioni
        Dim dt As DataTable = connessioni.Leggi(
            0, enum_Tipo_DB.TUTTI, "", "", "", "", "", "", "", 0, "",
            CostantiPersonalizzate.AGRODATAINIZIO,
            CostantiPersonalizzate.AGRODATAFINE,
            "", " DB ASC", objParametri_Super_Server)

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            For Each row As DataRow In dt.Rows
                Dim idDB As Integer = Str(row.Item("ID_DB"))
                Dim toCrypt As Boolean = LCase(cryptConnection) = "true" OrElse cryptConnection.Split(",").Contains(idDB)
                Dim Stringa_Connessione As String = Sicurezza.ReadConnectionString(row, objParametri_Super_Server, cryptKey, toCrypt)
                stringheConnessione.Add(CStr(row.Item("ID_DB")), Stringa_Connessione)
            Next
        End If

        Return stringheConnessione

    End Function

    Public Shared Function EncryptString(ByVal plainText As String, ByRef objParametri As AgronicaCoreParametri) As String

        Dim sicurezza As New Sicurezza
        Dim configurazioneSiti As New Configurazione_Siti_R
        Dim cr = configurazioneSiti.Leggi_Valore(0, "cr", "", "", objParametri)

        Return sicurezza.EncryptString(plainText, cr)

    End Function

    Public Shared Function DecryptString(ByVal encryptedText As String, ByRef objParametri As AgronicaCoreParametri) As String

        Dim sicurezza As New Sicurezza
        Dim configurazioneSiti As New Configurazione_Siti_R
        Dim cr = configurazioneSiti.Leggi_Valore(0, "cr", "", "", objParametri)

        Return sicurezza.DecryptString(encryptedText, cr)

    End Function

    Public Shared Function Leggi_Token(ByRef token As String, ByRef objParametri_Super_Server As AgronicaCoreParametri) As String

        If Not String.IsNullOrEmpty(token) Then
            Dim tokens = DecryptString(token, objParametri_Super_Server).Split("|")
            If tokens.Length > 0 Then
                token = tokens(0)
            End If
            If tokens.Length > 1 Then
                Return tokens(1)
            End If
        End If

        Return Nothing

    End Function

End Class
