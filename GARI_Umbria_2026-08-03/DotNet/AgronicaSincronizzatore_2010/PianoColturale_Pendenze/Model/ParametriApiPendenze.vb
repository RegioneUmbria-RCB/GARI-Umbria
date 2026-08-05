Imports Microsoft.VisualBasic

Public Class ParametriApiPendenze
    Public Property API_pendenze_url_base As String
    Public Property API_pendende_url_login As String
    Public Property API_pendenze_login As String
    Public Property API_pendenze_p As String
    Public Property API_pendenze_url_particelle As String
    Public Property API_pendenze_epsg As Integer
    Public Property Use_POST_API As Boolean
    Public Property PIVA_Azienda As String
    Public Property LOG_level As Integer
    Public Property LOG_DB As Integer
    Public Property TipoLetturaParticelle As enum_TipoLetturaParticelle = enum_TipoLetturaParticelle.GIS
    Public Property GIS_IntersectionTollerance As Double = 0
    Public Property Filtro_AxiendePrioritarie As Boolean = False
    Public Property ElencoProvince As List(Of String)
End Class

Public Enum enum_TipoLetturaParticelle
    GIS = 0
    AGEA = 1
    AGEA_GIS = 2
End Enum