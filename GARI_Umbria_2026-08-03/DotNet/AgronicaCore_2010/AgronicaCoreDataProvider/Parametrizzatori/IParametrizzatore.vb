Imports System.Data.SqlClient

Public Interface IParametrizzatore : Inherits ISqlSave

    ReadOnly Property Parametri() As Dictionary(Of Int32, AgroDBParametro)
    ReadOnly Property CopiaParametri() As Dictionary(Of Int32, AgroDBParametro)
    Property FiltroAggiuntivoOriginale() As String
    Property FiltroAggiuntivoManipolato() As String
    Property OrderByOriginale() As String
    Property OrderByManipolato() As String
    Property ByPassaLog() As Boolean

    Function Parametrizza(ByRef stringaSql As String,
                                 ByVal parametriInput As Dictionary(Of Int32, AgroDBParametro),
                                 ByVal orderByEFiltroAggiuntivo As OrderByFiltroAggiuntivo,
                                 ByRef parametriOutput As List(Of SqlParameter),
                                 Optional ByVal objParametri_Server As AgronicaCoreParametri = Nothing) As Boolean

    Sub SettaCopiaParametri(ByVal parametri As Dictionary(Of Integer, AgroDBParametro))
    Function DammiStackTrace() As String
    Sub Logga(ByVal NomeRoutine As String,
              ByVal MessaggioErrore As String,
                Optional ByVal objParametri As AgronicaCoreParametri = Nothing,
                Optional ByVal verificaInviaElasticSearch As Boolean = False)

    Function Agro_SQL_Save_xFiltroAggiuntivo_Semplificato(ByVal filtro As String,
                                                   Optional ByVal creaParametriSql As Boolean = True,
                                                   Optional ByVal injectionGuid As Guid = Nothing,
                                                   Optional ByVal objParametri As AgronicaCoreParametri = Nothing) As String


End Interface
