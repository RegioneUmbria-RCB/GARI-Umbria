Public Interface ISqlSave

    Function Agro_SQL_SaveText(ByVal Testo As String,
                               Optional ByVal creaParametroSql As Boolean = True,
                               Optional ByVal injectionGuid As Guid = Nothing) As String

    Function Agro_SQL_SaveText_NULL(ByVal item As Object, Optional ByVal creaParametroSql As Boolean = True, Optional ByVal injectionGuid As Guid = Nothing) As String
    Function Agro_SQL_SaveText_UNICODE(ByVal Testo As String, ByVal par As String, Optional ByVal creaParametroSql As Boolean = True, Optional ByVal injectionGuid As Guid = Nothing) As String

    Function Agro_SQL_Save_Clausola_IN(ByVal clausolaIN As String, ByVal valoriStringa As Boolean, Optional ByVal creaParametriSql As Boolean = True, Optional ByVal injectionGuid As Guid = Nothing) As String
    Function Agro_SQL_SaveDate(ByVal DataItaliana As Date, Optional ByVal creaParametroSql As Boolean = True, Optional ByVal injectionGuid As Guid = Nothing) As String
    Function Agro_SQL_SaveDateTime(ByVal DataOraItaliana As DateTime, Optional ByVal creaParametroSql As Boolean = True, Optional ByVal injectionGuid As Guid = Nothing) As String

    Function Agro_SQL_SaveDateTime_NULL(ByVal item As Object, Optional ByVal creaParametroSql As Boolean = True, Optional ByVal injectionGuid As Guid = Nothing) As String
    Function Agro_SQL_SaveNum(ByVal StringaNumero As String, Optional ByVal creaParametroSql As Boolean = True, Optional ByVal injectionGuid As Guid = Nothing) As String
    Function Agro_SQL_Save_xFiltroAggiuntivo(ByVal filtro As String, Optional ByRef creaParametriSql As Boolean = True, Optional ByVal injectionGuid As Guid = Nothing, Optional ByVal objParametri As AgronicaCoreParametri = Nothing) As String
    Function Agro_SQL_Save_xOrderBy(ByVal filtro As String, Optional ByVal objParametri As AgronicaCoreParametri = Nothing) As String


End Interface
