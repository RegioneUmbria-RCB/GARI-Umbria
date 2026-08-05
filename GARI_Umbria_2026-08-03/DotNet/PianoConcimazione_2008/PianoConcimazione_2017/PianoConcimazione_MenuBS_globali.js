var MenuBSResx = [];
var resxArrPath = [
    "App_GlobalResources/PianoConcimazione_2017.resx"
];

var Enum_Metodo = {}

var jSonParsed_Kendo_PianiConcimazione;
var jSonParsed_Kendo_PianiDistribuzione;
var window_Distribuzione;
var current_PC;

var template_tipo = "#:Tipo_Des_datoTipoCod(data.PC_Tipo)#";
var template_tipo_pua = "#:Tipo_Des_datoTipoCod_PUA(data.PC_Tipo)#";
var template_tipo_pn = "#:Tipo_Des_datoTipoCod_PN(data.PC_Tipo, data.Regolamento_Tipo)#";

// #region  enum_PUARegolamenti_Tipo
const PianoConcimazione = 1
const PUA = 2
const Pan = 3
const PianoNutrizionale = 4
const PianoNutrizionale_IBF = 5
// #endregion  

