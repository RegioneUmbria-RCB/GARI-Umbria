Public Class UtenteColdiretti
    Public Property COD_ANAGEN As String
    Public Property COD_TIPO_ANAG As String
    Public Property DAT_INIZIO As DateTime?
    Public Property DAT_FINE As DateTime?
    Public Property data As Blocco_Data

    Public Function GetUserCode() As String
        If COD_TIPO_ANAG = "G" And data.SGL_CODFIS.Length = 16 Then
            Return data.titolare.COD_ANAGEN
        Else
            Return data.COD_ANAGEN
        End If
    End Function
End Class

Public Class Blocco_Data
    Public Property COD_ANAGEN As String
    Public Property DES_RAGIONE_SOCIALE As String
    Public Property SGL_CODFIS As String
    Public Property SGL_PIVA As String
    Public Property titolare As Blocco_Titolare
End Class

Public Class Blocco_Titolare
    Public Property COD_ANAGEN As String
    Public Property DES_NOME As String
    Public Property DES_COGNOME As String
    Public Property SGL_CODFIS As String
    Public Property DAT_NASCITA As DateTime?
    Public Property SGL_COMUNE_NASCITA As String
End Class

Public Class UserRappresentation
    Public Property id As String
    Public Property createdTimestamp As Long
    Public Property username As String
    Public Property enabled As Boolean
    Public Property emailVerified As Boolean
    Public Property firstName As String
    Public Property lastName As String
    Public Property email As String
    Public Property access As UserAccess
End Class

Public Class UserAccess
    Public Property manageGroupMembers As Boolean
    Public Property view As Boolean
    Public Property mapRoles As Boolean
    Public Property impersonate As Boolean
    Public Property manage As Boolean
End Class
