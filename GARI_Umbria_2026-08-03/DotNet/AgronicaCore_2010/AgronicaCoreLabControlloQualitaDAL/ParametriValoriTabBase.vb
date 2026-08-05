Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class ParametriValoriTabBase_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Const NOME_TAB_BASE As String = "LCQ_ParametriValori"

    '##############################################################################################
    Public Function Leggi_ParametriXValoriTabBase( _
                                        ByVal Documento_Cod As Integer?, _
                                        ByVal PrmXMod_Cod As Integer?, _
                                        ByVal DataOraRilevazione As DateTime?, _
                                        ByVal Utente As String, _
                                        ByVal String_Valore As String, _
                                        ByVal DateTime_Valore As DateTime?, _
                                        ByVal Int_Valore As Integer?, _
                                        ByVal Float_Valore As Double?, _
                                        ByVal Provvisorio As Boolean?, _
                                        ByVal xFiltroAggiuntivo As String, _
                                        ByVal xOrderBy As String, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "LabControlloQualitaDAL.ParametriValori_R.Leggi_ParametriValoriTabBase"

        Dim pvg As New ParametriValoriGenerico_R
        Dim DT As DataTable = pvg.Leggi_ParametriXValori(NOME_TAB_BASE, Documento_Cod, PrmXMod_Cod, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, "", "", objParametri)

        Return DT

    End Function


    '##############################################################################################
    Public Function Leggi_ParametriXValoriTabBase( _
                                        ByVal Documento_Cod As Integer, _
                                        ByVal DaValutare As Boolean, _
                                        ByVal xFiltroAggiuntivo As String, _
                                        ByVal xOrderBy As String, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "LabControlloQualitaDAL.ParametriValori_R.Leggi_ParametriValoriTabBase"

        Dim pvg As New ParametriValoriGenerico_R
        Dim DT As DataTable = pvg.Leggi_ParametriXValori(NOME_TAB_BASE, Documento_Cod, DaValutare, "", "", objParametri)

        Return DT

    End Function

    Public Function Verifica_ValEsisteTabBase( _
                                        ByVal Documento_Cod As Integer, _
                                        ByVal PrmXMod_Cod As Integer, _
                                        ByVal DataOraRilevazione As DateTime?, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "LabControlloQualitaDAL.ParametriValori_R.Verifica_ValEsisteTabBase"

        Dim pvg As New ParametriValoriGenerico_R
        Dim esito As Boolean = pvg.Verifica_ValEsiste(NOME_TAB_BASE, Documento_Cod, PrmXMod_Cod, DataOraRilevazione, objParametri)

        Return esito

    End Function

End Class


Public Class ParametriValoriTabBase_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Const NOME_TAB_BASE As String = "LCQ_ParametriValori"

    '##############################################################################################
    Public Function ScriviTabBase(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                        ByVal Documento_Cod As Integer, _
                                        ByVal PrmXMod_Cod As Integer, _
                                        ByVal DataOraRilevazione As DateTime?, _
                                        ByVal Utente As String, _
                                        ByVal String_Valore As String, _
                                        ByVal DateTime_Valore As DateTime?, _
                                        ByVal Int_Valore As Integer?, _
                                        ByVal Float_Valore As Double?, _
                                        ByVal Provvisorio As Boolean?, _
                                        Optional ByVal Data_creazione As Date = #2/1/1900#, _
                                        Optional ByVal Data_modifica As Date = #2/1/1900#, _
                                        Optional ByVal username_creazione As String = "", _
                                        Optional ByVal username_modifica As String = "" _
                                        ) As Boolean


        Dim NomeRoutine As String = "LabControlloQualitaDAL.ParametriValori_W.ScriviTabBase()"

        Dim pvg As New ParametriValoriGenerico_W
        Dim esito As Boolean = pvg.Scrivi(objParametri, NOME_TAB_BASE, _
                       Documento_Cod, PrmXMod_Cod, _
                       DataOraRilevazione, Utente, _
                       String_Valore, DateTime_Valore, _
                       Int_Valore, Float_Valore, Provvisorio, _
                       Data_creazione, Data_modifica, _
                       username_creazione, username_modifica)

        Return esito

    End Function

    '##############################################################################################
    Public Function ModificaTabBase(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                ByVal Old_Documento_Cod As Integer, _
                ByVal Old_PrmXMod_Cod As Integer, _
                ByVal Old_DataOraRilevazione As DateTime?, _
                ByVal New_Utente As String, _
                ByVal New_String_Valore As String, _
                ByVal New_DateTime_Valore As DateTime?, _
                ByVal New_Int_Valore As Integer?, _
                ByVal New_Float_Valore As Double?, _
                ByVal New_Provvisorio As Boolean?, _
                Optional ByVal Data_modifica As Date = #2/1/1900#, _
                Optional ByVal username_modifica As String = "" _
                ) As Boolean


        Dim NomeRoutine As String = "LabControlloQualitaDAL.ParametriValori_W.ModificaTabBase()"

        Dim pvg As New ParametriValoriGenerico_W
        Dim esito As Boolean = pvg.Modifica(objParametri, NOME_TAB_BASE, _
                          Old_Documento_Cod, Old_PrmXMod_Cod, _
                          Old_DataOraRilevazione, New_Utente, _
                          New_String_Valore, New_DateTime_Valore, _
                          New_Int_Valore, New_Float_Valore, New_Provvisorio, _
                          Data_modifica, username_modifica)

        Return esito

    End Function

    '##############################################################################################
    Public Function ModificaOrarioTabBase(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                ByVal Old_Documento_Cod As Integer, _
                ByVal Old_DataOraRilevazione As DateTime, _
                ByVal New_DataOraRilevazione As DateTime, _
                Optional ByVal Data_modifica As Date = #2/1/1900#, _
                Optional ByVal username_modifica As String = "" _
                ) As Boolean


        Dim NomeRoutine As String = "LabControlloQualitaDAL.ParametriValori_W.ModificaOrarioTabBase()"


        Dim pvg As New ParametriValoriGenerico_W
        Dim esito As Boolean = pvg.ModificaOrario(objParametri, NOME_TAB_BASE, _
                          Old_Documento_Cod, Old_DataOraRilevazione, New_DataOraRilevazione, Data_modifica, username_modifica)

        Return esito

    End Function

    '##############################################################################################
    Public Function ModificaUtenteTabBase(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                ByVal Old_Documento_Cod As Integer, _
                ByVal Old_DataOraRilevazione As DateTime, _
                ByVal New_Utente As String, _
                Optional ByVal Data_modifica As Date = #2/1/1900#, _
                Optional ByVal username_modifica As String = "" _
                ) As Boolean


        Dim NomeRoutine As String = "LabControlloQualitaDAL.ParametriValori_W.ModificaUtenteTabBase()"

        Dim pvg As New ParametriValoriGenerico_W
        Dim esito As Boolean = pvg.ModificaUtente(objParametri, NOME_TAB_BASE, _
                          Old_Documento_Cod, Old_DataOraRilevazione, New_Utente, Data_modifica, username_modifica)

        Return esito

    End Function

    '#################################################################
    Public Function CancellaTabBase(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                            ByVal Documento_Cod As Integer, _
                            ByVal PrmXMod_Cod As Integer, _
                            ByVal DataOraRilevazione As DateTime, _
                            ByVal xFiltroAggiuntivo As String _
                              ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "LabControlloQualitaDAL.ParametriValori_W.CancellaTabBase()"

        Dim pvg As New ParametriValoriGenerico_W
        Dim esito As Boolean = pvg.Cancella(objParametri, NOME_TAB_BASE, _
                                            Documento_Cod, PrmXMod_Cod, DataOraRilevazione, xFiltroAggiuntivo)

        Return esito
    End Function

    '#################################################################
    Public Function CancellaDocumentoTabBase(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                            ByVal Documento_Cod As Integer, _
                            ByVal xFiltroAggiuntivo As String _
                              ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "LabControlloQualitaDAL.ParametriValori_W.CancellaDocumentoTabBase()"

        Dim pvg As New ParametriValoriGenerico_W
        Dim esito As Boolean = pvg.CancellaDocumento(objParametri, NOME_TAB_BASE, _
                                            Documento_Cod, xFiltroAggiuntivo)

        Return esito
    End Function
End Class