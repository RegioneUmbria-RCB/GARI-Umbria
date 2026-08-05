'------------------------------------------------------------------------------
' <generato automaticamente>
'     Codice generato da uno strumento.
'
'     Le modifiche a questo file possono causare un comportamento non corretto e verranno perse se
'     il codice viene rigenerato. 
' </generato automaticamente>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On


Partial Public Class PCB_InserimentoMultiplo
    
    '''<summary>
    '''Controllo ddlRegolamento.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents ddlRegolamento As Global.System.Web.UI.WebControls.DropDownList
    
    '''<summary>
    '''Controllo Txt_Descrizione.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_Descrizione As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''Controllo Txt_ValiditaInizio.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_ValiditaInizio As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''Controllo Txt_ValiditaFine.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_ValiditaFine As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''Controllo Txt_Anno.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_Anno As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''Controllo Div_Aggregazione.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Div_Aggregazione As Global.System.Web.UI.HtmlControls.HtmlGenericControl
    
    '''<summary>
    '''Controllo CmbAggregazione.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents CmbAggregazione As Global.System.Web.UI.WebControls.DropDownList
    
    '''<summary>
    '''Controllo grdSpecie.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents grdSpecie As Global.System.Web.UI.WebControls.GridView
    
    '''<summary>
    '''Controllo Btn_Bilancio.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Btn_Bilancio As Global.System.Web.UI.WebControls.Button
    
    '''<summary>
    '''Controllo Btn_Schede.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Btn_Schede As Global.System.Web.UI.WebControls.Button
    
    '''<summary>
    '''Controllo GridViewBilanci.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents GridViewBilanci As Global.System.Web.UI.WebControls.GridView
    
    '''<summary>
    '''Controllo Btn_Salva.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Btn_Salva As Global.System.Web.UI.WebControls.Button
    
    '''<summary>
    '''Controllo Txt_Sabbia.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_Sabbia As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''Controllo Txt_Argilla.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_Argilla As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''Controllo Txt_Limo.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_Limo As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''Controllo Txt_PH.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_PH As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''Controllo Txt_CalcTot.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_CalcTot As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''Controllo Txt_CalcAtt.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_CalcAtt As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''Controllo Txt_SO.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_SO As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''Controllo Txt_CN.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_CN As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''Controllo Txt_N.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_N As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''Controllo Txt_P.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_P As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''Controllo Txt_K.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_K As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''Controllo Txt_Mg.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_Mg As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''Controllo Txt_CSC.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_CSC As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''Controllo BtnAggiornaBilancio.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents BtnAggiornaBilancio As Global.System.Web.UI.WebControls.Button
    
    '''<summary>
    '''Controllo BtnCalcolaBilancio.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents BtnCalcolaBilancio As Global.System.Web.UI.WebControls.Button
    
    '''<summary>
    '''Controllo IndiceBilancio.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents IndiceBilancio As Global.System.Web.UI.HtmlControls.HtmlInputHidden
    
    '''<summary>
    '''Controllo Hidden_Veg_Cod.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Hidden_Veg_Cod As Global.System.Web.UI.HtmlControls.HtmlInputHidden
    
    '''<summary>
    '''Controllo Hidden_Grfi_Cod.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Hidden_Grfi_Cod As Global.System.Web.UI.HtmlControls.HtmlInputHidden
    
    '''<summary>
    '''Controllo Hidden_Fase_Cod.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Hidden_Fase_Cod As Global.System.Web.UI.HtmlControls.HtmlInputHidden
    
    '''<summary>
    '''Controllo Hidden_Resa.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Hidden_Resa As Global.System.Web.UI.HtmlControls.HtmlInputHidden
    
    '''<summary>
    '''Controllo Hidden_AnticipazioniAnni.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Hidden_AnticipazioniAnni As Global.System.Web.UI.HtmlControls.HtmlInputHidden
    
    '''<summary>
    '''Controllo Hidden_Copertura.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Hidden_Copertura As Global.System.Web.UI.HtmlControls.HtmlInputHidden
    
    '''<summary>
    '''Controllo Hidden_UbicazioneCod.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Hidden_UbicazioneCod As Global.System.Web.UI.HtmlControls.HtmlInputHidden
    
    '''<summary>
    '''Controllo Hidden_PercNFissazione.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Hidden_PercNFissazione As Global.System.Web.UI.HtmlControls.HtmlInputHidden
    
    '''<summary>
    '''Controllo Hidden_DispOssigeno.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Hidden_DispOssigeno As Global.System.Web.UI.HtmlControls.HtmlInputHidden
    
    '''<summary>
    '''Controllo Hidden_PiovositaFeb.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Hidden_PiovositaFeb As Global.System.Web.UI.HtmlControls.HtmlInputHidden
    
    '''<summary>
    '''Controllo Hidden_Piovosita.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Hidden_Piovosita As Global.System.Web.UI.HtmlControls.HtmlInputHidden
    
    '''<summary>
    '''Controllo Hidden_Precessione.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Hidden_Precessione As Global.System.Web.UI.HtmlControls.HtmlInputHidden
    
    '''<summary>
    '''Controllo Hidden_TipoFertilizzante.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Hidden_TipoFertilizzante As Global.System.Web.UI.HtmlControls.HtmlInputHidden
    
    '''<summary>
    '''Controllo Hidden_IdFrequenza.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Hidden_IdFrequenza As Global.System.Web.UI.HtmlControls.HtmlInputHidden
    
    '''<summary>
    '''Controllo Hidden_QtaN_FerPrec.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Hidden_QtaN_FerPrec As Global.System.Web.UI.HtmlControls.HtmlInputHidden
    
    '''<summary>
    '''Controllo Hidden_Fattori.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Hidden_Fattori As Global.System.Web.UI.HtmlControls.HtmlInputHidden
    
    '''<summary>
    '''Controllo Div_Bilancio.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Div_Bilancio As Global.System.Web.UI.HtmlControls.HtmlGenericControl
    
    '''<summary>
    '''Controllo grdNecessita.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents grdNecessita As Global.System.Web.UI.WebControls.GridView
    
    '''<summary>
    '''Controllo GrdDisponibilita.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents GrdDisponibilita As Global.System.Web.UI.WebControls.GridView
    
    '''<summary>
    '''Controllo LblAttenzione.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents LblAttenzione As Global.System.Web.UI.WebControls.Label
    
    '''<summary>
    '''Controllo Div_Schede.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Div_Schede As Global.System.Web.UI.HtmlControls.HtmlGenericControl
    
    '''<summary>
    '''Controllo tab_Scheda_N.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents tab_Scheda_N As Global.System.Web.UI.HtmlControls.HtmlGenericControl
    
    '''<summary>
    '''Controllo RicaricaGrid.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents RicaricaGrid As Global.System.Web.UI.WebControls.Button
    
    '''<summary>
    '''Controllo grdDecrementi.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents grdDecrementi As Global.System.Web.UI.WebControls.GridView
    
    '''<summary>
    '''Controllo grdIncrementi.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents grdIncrementi As Global.System.Web.UI.WebControls.GridView
    
    '''<summary>
    '''Controllo TextBox1.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents TextBox1 As Global.System.Web.UI.WebControls.Label
    
    '''<summary>
    '''Controllo Txt_DoseStandard.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_DoseStandard As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''Controllo Label16.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Label16 As Global.System.Web.UI.WebControls.Label
    
    '''<summary>
    '''Controllo Txt_MAS.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_MAS As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''Controllo Label5.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Label5 As Global.System.Web.UI.WebControls.Label
    
    '''<summary>
    '''Controllo Txt_MaxIncrementi.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_MaxIncrementi As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''Controllo Label1.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Label1 As Global.System.Web.UI.WebControls.Label
    
    '''<summary>
    '''Controllo Txt_TotIncrementi.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_TotIncrementi As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''Controllo Label2.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Label2 As Global.System.Web.UI.WebControls.Label
    
    '''<summary>
    '''Controllo Txt_TotDecrementi.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_TotDecrementi As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''Controllo Label3.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Label3 As Global.System.Web.UI.WebControls.Label
    
    '''<summary>
    '''Controllo Txt_DoseRicalcolata.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_DoseRicalcolata As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''Controllo tab_Scheda_P.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents tab_Scheda_P As Global.System.Web.UI.HtmlControls.HtmlGenericControl
    
    '''<summary>
    '''Controllo grdDecrementiP.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents grdDecrementiP As Global.System.Web.UI.WebControls.GridView
    
    '''<summary>
    '''Controllo grdIncrementiP.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents grdIncrementiP As Global.System.Web.UI.WebControls.GridView
    
    '''<summary>
    '''Controllo Label7.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Label7 As Global.System.Web.UI.WebControls.Label
    
    '''<summary>
    '''Controllo ddlDoseP.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents ddlDoseP As Global.System.Web.UI.WebControls.DropDownList
    
    '''<summary>
    '''Controllo Label6.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Label6 As Global.System.Web.UI.WebControls.Label
    
    '''<summary>
    '''Controllo Txt_DoseStandardP.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_DoseStandardP As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''Controllo Label8.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Label8 As Global.System.Web.UI.WebControls.Label
    
    '''<summary>
    '''Controllo Txt_TotIncrementiP.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_TotIncrementiP As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''Controllo Label9.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Label9 As Global.System.Web.UI.WebControls.Label
    
    '''<summary>
    '''Controllo Txt_TotDecrementiP.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_TotDecrementiP As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''Controllo Label10.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Label10 As Global.System.Web.UI.WebControls.Label
    
    '''<summary>
    '''Controllo Txt_DoseRicalcolataP.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_DoseRicalcolataP As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''Controllo tab_Scheda_K.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents tab_Scheda_K As Global.System.Web.UI.HtmlControls.HtmlGenericControl
    
    '''<summary>
    '''Controllo grdDecrementiK.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents grdDecrementiK As Global.System.Web.UI.WebControls.GridView
    
    '''<summary>
    '''Controllo grdIncrementiK.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents grdIncrementiK As Global.System.Web.UI.WebControls.GridView
    
    '''<summary>
    '''Controllo Label11.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Label11 As Global.System.Web.UI.WebControls.Label
    
    '''<summary>
    '''Controllo ddlDoseK.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents ddlDoseK As Global.System.Web.UI.WebControls.DropDownList
    
    '''<summary>
    '''Controllo Label12.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Label12 As Global.System.Web.UI.WebControls.Label
    
    '''<summary>
    '''Controllo Txt_DoseStandardK.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_DoseStandardK As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''Controllo Label13.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Label13 As Global.System.Web.UI.WebControls.Label
    
    '''<summary>
    '''Controllo Txt_TotIncrementiK.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_TotIncrementiK As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''Controllo Label14.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Label14 As Global.System.Web.UI.WebControls.Label
    
    '''<summary>
    '''Controllo Txt_TotDecrementiK.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_TotDecrementiK As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''Controllo Label15.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Label15 As Global.System.Web.UI.WebControls.Label
    
    '''<summary>
    '''Controllo Txt_DoseRicalcolataK.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_DoseRicalcolataK As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''Proprietà Master.
    '''</summary>
    '''<remarks>
    '''Proprietà generata automaticamente.
    '''</remarks>
    Public Shadows ReadOnly Property Master() As PianoConcimazione_2017.MasterConcimazione
        Get
            Return CType(MyBase.Master,PianoConcimazione_2017.MasterConcimazione)
        End Get
    End Property
End Class
