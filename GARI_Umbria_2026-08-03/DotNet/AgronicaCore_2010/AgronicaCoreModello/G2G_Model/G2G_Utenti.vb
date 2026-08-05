Imports AgronicaCoreEntityFramework_POCO

Public Class G2G_Utenti
    Inherits G2G_Base

    Public Property utenti_dbserver_insert As List(Of Utenti)
    Public Property utenti_dbserver_update As List(Of Utenti)
    'Public Property utenti_dbserver_delete As List(Of Utenti)

    Public Property utenti_dbutenti_insert As List(Of PUA_Testata)
    Public Property utenti_dbutenti_update As List(Of PUA_Testata)
    'Public Property utenti_dbutenti_delete As List(Of PUA_Testata)

    Public Property utenti_dettagli_insert As List(Of PUA_Testata)
    Public Property utenti_dettagli_update As List(Of PUA_Testata)
    'Public Property utenti_dettagli_delete As List(Of PUA_Testata)

    Public Property utenti_impostazioni_insert As List(Of PUA_Testata)
    Public Property utenti_impostazioni_update As List(Of PUA_Testata)
    'Public Property utenti_impostazioni_delete As List(Of PUA_Testata)

    Public Property utenti_impostazioni_filtromono_insert As List(Of PUA_Testata)
    Public Property utenti_impostazioni_filtromono_update As List(Of PUA_Testata)
    'Public Property Utenti_Impostazioni_FiltroMono_delete As List(Of PUA_Testata)

    Public Property utenti_permessi_insert As List(Of PUA_Testata)
    Public Property utenti_permessi_update As List(Of PUA_Testata)
    'Public Property utenti_permessi_delete As List(Of PUA_Testata)

    Public Property utenti_profili_insert As List(Of PUA_Testata)
    Public Property utenti_profili_update As List(Of PUA_Testata)
    'Public Property utenti_profili_delete As List(Of PUA_Testata)

    Public Property utenti_xgruppi_utente_insert As List(Of PUA_Testata)
    Public Property utenti_xgruppi_utente_update As List(Of PUA_Testata)
    'Public Property utenti_xgruppi_utente_delete As List(Of PUA_Testata)

    Public Property gruppi_utente_insert As List(Of PUA_Testata)
    Public Property gruppi_utente_update As List(Of PUA_Testata)
    'Public Property gruppi_utente_delete As List(Of PUA_Testata)

End Class
