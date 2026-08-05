
Imports AgronicaCoreDataProvider.TipiEnumerativi


Public Interface iAgronicaControlliCommons

    Property GiasVersioneCorrente As String

    ReadOnly Property PATH_GIASBASE As String

    Sub inizializza()

    Sub AppendCssToHeader(basePath As String, puntoInterrogativo As String, css As String, placeHolder As PlaceHolder)

    Function LeggiDaSessioneOppureDaConfigSiti(ByVal chiave As String, ByVal valoreDefault As String, ByVal TipoDB As agronicacoreparametri_tipoDB, Optional ByVal MemorizzaInSessioneDopoLettura As Boolean = True, Optional ByVal paramSessioneObjParametriValue As String = "") As String


End Interface
