Imports AgronicaCoreAnagrafeBIZ.AnagrafeNG


Public Class CentroAziendaleDropdowns
    Public Tipologia As DropDownList
    Public TitoloPossesso As DropDownList
    Public Provincie As DropDownList
    Public Comune As DropDownList
    Public Stati As DropDownList
    Public Codici As DropDownList
    Public TipoAttività As DropDownList
    Public OrganismiControllo As DropDownList
    Public OTEs As DropDownList
End Class

Public Class CentroAziendaleNG
    Public Centro As AgronicaCoreModelsSTD.anagrafiche.CentroAziendale
    Public DatiVisibili As CentroDropdownLists
End Class
