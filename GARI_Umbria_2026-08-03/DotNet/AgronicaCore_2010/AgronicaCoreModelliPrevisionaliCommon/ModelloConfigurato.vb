
Imports Newtonsoft.Json


Friend Class ModelloConfigurato
    Public ChiaveRichiesta As Integer
    Public Tipo_Sorgente As Integer
    Public Stazione_Cod As Integer
    Public Mod_Cod As Integer
    Public Veg_Cod As Integer
    Public Avv_Cod As Integer
    Public Alg_Cod As Integer
    Public ParametriElaborazione As String
    Public InizioPeriodo_gg As Integer
    Public FinePeriodo_gg As Integer
    Public Validita_minuti As Integer
    Public DatiMeteoInizio_gg As Integer
    Public DatiMeteoFine_gg As Integer

    <JsonIgnore>
    Public IdGroup As Integer
    <JsonIgnore>
    Public DescrGroup As String
    <JsonIgnore>
    Public AuxData As Object
End Class



