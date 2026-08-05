Imports AgronicaCoreAuditDAL
Imports AgronicaCoreDataProvider

Public Class AuditAttivazione

    Dim objParametri As AgronicaCoreParametri
    ''###########################################################################################################
    Public Sub New(ByVal _objParametri As AgronicaCoreParametri)
        objParametri = _objParametri

    End Sub

    '#######################################################################################
    Public Function Zona(ByVal SuperPiva As String, ByVal Piva As String, ByVal Zona_Cod As Integer) As Boolean
        Dim DLL_AD As New Audit_Profilazione_R
        Dim strErr As String = ""
        Dim bRet As Boolean = False
        bRet = DLL_AD.AuditZona(SuperPiva, Piva, Zona_Cod, "", objParametri)
        DLL_AD = Nothing
        Return bRet
    End Function

    Public Function ZonaZPS(ByVal SuperPiva As String, ByVal Piva As String) As Boolean
        Return Zona(SuperPiva, Piva, TipiEnumerativi.enum_Zone.ZPS)
    End Function

    Public Function ZonaSIC(ByVal SuperPiva As String, ByVal Piva As String) As Boolean
        Return Zona(SuperPiva, Piva, TipiEnumerativi.enum_Zone.SIC)
    End Function

    Public Function ZonaZVN(ByVal SuperPiva As String, ByVal Piva As String) As Boolean
        Return Zona(SuperPiva, Piva, TipiEnumerativi.enum_Zone.ZVN)
    End Function

    Public Function ZonaZO(ByVal SuperPiva As String, ByVal Piva As String) As Boolean
        Return Zona(SuperPiva, Piva, TipiEnumerativi.enum_Zone.ZVN)
    End Function

    '#######################################################################################
    Public Function Fanghi(ByVal SuperPiva As String, ByVal Piva As String) As Boolean
        Return False
    End Function

    '#######################################################################################
    Public Function ZOO(ByVal SuperPiva As String, ByVal Piva As String) As Boolean
        Dim DLL_AD As New Audit_Profilazione_R
        Dim strErr As String = ""
        Dim bRet As Boolean = False
        bRet = DLL_AD.AuditZoo("", Piva, 0, "", objParametri)
        DLL_AD = Nothing
        Return bRet
    End Function

    '#######################################################################################
    Public Function PFS(ByVal SuperPiva As String, ByVal Piva As String) As Boolean
        Dim DLL_AD As New Audit_Profilazione_R
        Dim strErr As String = ""
        Dim bRet As Boolean = False
        bRet = DLL_AD.AuditPFS("", Piva, "", objParametri)
        DLL_AD = Nothing
        Return bRet
    End Function

    '#######################################################################################
    Public Function Oliveti(ByVal SuperPiva As String, ByVal Piva As String) As Boolean
        Dim DLL_AD As New Audit_Profilazione_R
        Dim strErr As String = ""
        Dim bRet As Boolean = False
        bRet = DLL_AD.AuditOliveti("", Piva, "", objParametri)
        DLL_AD = Nothing
        Return bRet
    End Function

    '#######################################################################################
    Public Function Seminativi(ByVal SuperPiva As String, ByVal Piva As String) As Boolean
        Dim DLL_AD As New Audit_Profilazione_R
        Dim strErr As String = ""
        Dim bRet As Boolean = False
        bRet = DLL_AD.AuditSeminativi("", Piva, "", objParametri)
        DLL_AD = Nothing
        Return bRet
    End Function

    '#######################################################################################
    Public Function SeminativiDeclivi(ByVal SuperPiva As String, ByVal Piva As String) As Boolean
        Dim DLL_AD As New Audit_Profilazione_R
        Dim strErr As String = ""
        Dim bRet As Boolean = False

        ' controllo che sia seminativo
        If DLL_AD.AuditSeminativi("", Piva, "", objParametri) Then

            ' controllo se si trova in una possibile area declive
            If DLL_AD.AuditZona(SuperPiva, Piva, TipiEnumerativi.enum_Zone.Collina, "", objParametri) Or
                DLL_AD.AuditZona(SuperPiva, Piva, TipiEnumerativi.enum_Zone.Montagna, "", objParametri) Then
                bRet = True
            End If
        End If

        DLL_AD = Nothing
        Return bRet
    End Function

    '#######################################################################################
    Public Function SetAside(ByVal SuperPiva As String, ByVal Piva As String) As Boolean
        Dim DLL_AD As New Audit_Profilazione_R
        Dim strErr As String = ""
        Dim bRet As Boolean = False
        bRet = DLL_AD.AuditSetAside("", Piva, "", objParametri)
        DLL_AD = Nothing
        Return bRet
    End Function

    '#######################################################################################
    Public Function PascoloPermanente(ByVal SuperPiva As String, ByVal Piva As String) As Boolean
        Dim DLL_AD As New Audit_Profilazione_R
        Dim strErr As String = ""
        Dim bRet As Boolean = False
        bRet = DLL_AD.AuditPascoloPermanente("", Piva, "", objParametri)
        DLL_AD = Nothing
        Return bRet
    End Function

    '#######################################################################################
    Public Function Carburanti(ByVal SuperPiva As String, ByVal Piva As String) As Boolean
        Dim DLL_AD As New Audit_Profilazione_R
        Dim strErr As String = ""
        Dim bRet As Boolean = False
        bRet = DLL_AD.AuditCarburanti("", Piva, "", objParametri)
        If Not bRet Then
            bRet = DLL_AD.AuditParcoMacchine("", Piva, "", objParametri)
        End If
        DLL_AD = Nothing
        Return bRet
    End Function

    '#######################################################################################
    Public Function Particelle(ByVal SuperPiva As String, ByVal Piva As String) As Boolean
        Dim DLL_AD As New Audit_Profilazione_R
        Dim strErr As String = ""
        Dim bRet As Boolean = False
        bRet = DLL_AD.AuditParticelle("", Piva, "", objParametri)
        DLL_AD = Nothing
        Return bRet
    End Function

End Class
