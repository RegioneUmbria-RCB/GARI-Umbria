'------------------------------------------------------------------------------
' <generato automaticamente>
'     Questo codice è stato generato da uno strumento.
'
'     Le modifiche a questo file possono causare un comportamento non corretto e verranno perse se
'     il codice viene rigenerato. 
' </generato automaticamente>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On


Partial Public Class PianoNutrizionale_IBF

    '''<summary>
    '''Controllo hd_Piva.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents hd_Piva As Global.System.Web.UI.WebControls.HiddenField

    '''<summary>
    '''Controllo hd_Operazione.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents hd_Operazione As Global.System.Web.UI.WebControls.HiddenField

    '''<summary>
    '''Controllo hd_Tipo.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents hd_Tipo As Global.System.Web.UI.WebControls.HiddenField

    '''<summary>
    '''Controllo hd_usaAnalisiModelloNG.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents hd_usaAnalisiModelloNG As Global.System.Web.UI.WebControls.HiddenField

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
    '''Controllo Txt_Note.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_Note As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo Chk_NonUtilizzo_Fertilizzanti.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Chk_NonUtilizzo_Fertilizzanti As Global.System.Web.UI.WebControls.CheckBox

    '''<summary>
    '''Controllo Cmb_Centro.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Cmb_Centro As Global.System.Web.UI.WebControls.DropDownList

    '''<summary>
    '''Controllo RBL_Specie.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents RBL_Specie As Global.System.Web.UI.WebControls.RadioButtonList

    '''<summary>
    '''Controllo Cmb_Specie_ColturaPrincipale.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Cmb_Specie_ColturaPrincipale As Global.System.Web.UI.WebControls.DropDownList

    '''<summary>
    '''Controllo ddlFinalitaRer_ColturaPrincipale.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents ddlFinalitaRer_ColturaPrincipale As Global.System.Web.UI.WebControls.DropDownList

    '''<summary>
    '''Controllo Txt_Resa.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_Resa As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo Lbl_ResaRiferimento.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Lbl_ResaRiferimento As Global.System.Web.UI.HtmlControls.HtmlGenericControl

    '''<summary>
    '''Controllo cmb_PeriodoSeminaColturaPrincipale.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents cmb_PeriodoSeminaColturaPrincipale As Global.System.Web.UI.HtmlControls.HtmlInputHidden

    '''<summary>
    '''Controllo cmb_PeriodoRaccoltaColturaPrincipale.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents cmb_PeriodoRaccoltaColturaPrincipale As Global.System.Web.UI.HtmlControls.HtmlInputHidden

    '''<summary>
    '''Controllo ddlAnalisi.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents ddlAnalisi As Global.System.Web.UI.WebControls.DropDownList

    '''<summary>
    '''Controllo BtnAnalisi.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents BtnAnalisi As Global.System.Web.UI.WebControls.Button

    '''<summary>
    '''Controllo Btn_hidden_CaricaAnalisi.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Btn_hidden_CaricaAnalisi As Global.System.Web.UI.WebControls.Button

    '''<summary>
    '''Controllo BtnVisualizza.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents BtnVisualizza As Global.System.Web.UI.WebControls.Button

    '''<summary>
    '''Controllo BtnRicerca.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents BtnRicerca As Global.System.Web.UI.WebControls.Button

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
    '''Controllo DDL_P2O5.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents DDL_P2O5 As Global.System.Web.UI.WebControls.DropDownList

    '''<summary>
    '''Controllo Txt_P.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_P As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo DDL_K2O.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents DDL_K2O As Global.System.Web.UI.WebControls.DropDownList

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
    '''Controllo Div_SalvaAnalisi.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Div_SalvaAnalisi As Global.System.Web.UI.HtmlControls.HtmlGenericControl

    '''<summary>
    '''Controllo Btn_SalvaAnalisi.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Btn_SalvaAnalisi As Global.System.Web.UI.WebControls.Button

    '''<summary>
    '''Controllo Txt_AnalisiDes.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_AnalisiDes As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo Cmb_Specie_Precessione.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Cmb_Specie_Precessione As Global.System.Web.UI.WebControls.DropDownList

    '''<summary>
    '''Controllo ddlFinalitaRer_Precessione.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents ddlFinalitaRer_Precessione As Global.System.Web.UI.WebControls.DropDownList

    '''<summary>
    '''Controllo Chk_ResiduiPrecessione.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Chk_ResiduiPrecessione As Global.System.Web.UI.WebControls.CheckBox

    '''<summary>
    '''Controllo cmb_PeriodoInterramentoResiduiPrecessione.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents cmb_PeriodoInterramentoResiduiPrecessione As Global.System.Web.UI.HtmlControls.HtmlInputHidden

    '''<summary>
    '''Controllo Txt_ResaStoricaPrecessione.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_ResaStoricaPrecessione As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo ddlConcimeOrganico.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents ddlConcimeOrganico As Global.System.Web.UI.WebControls.DropDownList

    '''<summary>
    '''Controllo Txt_QtaN_KGHa.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_QtaN_KGHa As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo ddlEpocaModalitaDistribuzione.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents ddlEpocaModalitaDistribuzione As Global.System.Web.UI.WebControls.DropDownList

    '''<summary>
    '''Controllo Meteo_ChkAgenda.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Meteo_ChkAgenda As Global.System.Web.UI.HtmlControls.HtmlInputHidden

    '''<summary>
    '''Controllo Meteo_TipoSorgente.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Meteo_TipoSorgente As Global.System.Web.UI.HtmlControls.HtmlInputHidden

    '''<summary>
    '''Controllo Meteo_TipoSorgente_Real.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Meteo_TipoSorgente_Real As Global.System.Web.UI.HtmlControls.HtmlInputHidden

    '''<summary>
    '''Controllo Meteo_Sorgente.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Meteo_Sorgente As Global.System.Web.UI.HtmlControls.HtmlInputHidden

    '''<summary>
    '''Controllo label_data_fine_piogge.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents label_data_fine_piogge As Global.System.Web.UI.WebControls.Label

    '''<summary>
    '''Controllo Txt_Pioggia.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_Pioggia As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo label_AvgTemperatura_ColturaInCampo.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents label_AvgTemperatura_ColturaInCampo As Global.System.Web.UI.WebControls.Label

    '''<summary>
    '''Controllo Txt_AvgTemperatura_ColturaInCampo.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_AvgTemperatura_ColturaInCampo As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo label_AvgTemperatura_MeseSemina_Febbraio.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents label_AvgTemperatura_MeseSemina_Febbraio As Global.System.Web.UI.WebControls.Label

    '''<summary>
    '''Controllo Txt_AvgTemperatura_MeseSemina_Febbraio.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_AvgTemperatura_MeseSemina_Febbraio As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo label_Txt_PercUmiditaColturaPrincipale.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents label_Txt_PercUmiditaColturaPrincipale As Global.System.Web.UI.WebControls.Label

    '''<summary>
    '''Controllo Txt_PercUmiditaColturaPrincipale.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_PercUmiditaColturaPrincipale As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo label_Txt_PercUmiditaRaccoltaPrecessione.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents label_Txt_PercUmiditaRaccoltaPrecessione As Global.System.Web.UI.WebControls.Label

    '''<summary>
    '''Controllo Txt_PercUmiditaRaccoltaPrecessione.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_PercUmiditaRaccoltaPrecessione As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo Div_BtnVisualizza.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Div_BtnVisualizza As Global.System.Web.UI.HtmlControls.HtmlGenericControl

    '''<summary>
    '''Controllo Btn_Bilancio.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Btn_Bilancio As Global.System.Web.UI.WebControls.Button

    '''<summary>
    '''Controllo Testata_Cod.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Testata_Cod As Global.System.Web.UI.HtmlControls.HtmlInputHidden

    '''<summary>
    '''Controllo N_Ammesso.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents N_Ammesso As Global.System.Web.UI.HtmlControls.HtmlInputHidden

    '''<summary>
    '''Controllo K_Ammesso.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents K_Ammesso As Global.System.Web.UI.HtmlControls.HtmlInputHidden

    '''<summary>
    '''Controllo P_Ammesso.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents P_Ammesso As Global.System.Web.UI.HtmlControls.HtmlInputHidden

    '''<summary>
    '''Controllo N_MAS.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents N_MAS As Global.System.Web.UI.HtmlControls.HtmlInputHidden

    '''<summary>
    '''Controllo Fattore_Correttivo_N_Resa.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Fattore_Correttivo_N_Resa As Global.System.Web.UI.HtmlControls.HtmlInputHidden

    '''<summary>
    '''Controllo ResaBassa.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents ResaBassa As Global.System.Web.UI.HtmlControls.HtmlInputHidden

    '''<summary>
    '''Controllo ResaAlta.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents ResaAlta As Global.System.Web.UI.HtmlControls.HtmlInputHidden

    '''<summary>
    '''Controllo ResaBassaDes.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents ResaBassaDes As Global.System.Web.UI.HtmlControls.HtmlInputHidden

    '''<summary>
    '''Controllo ResaAltaDes.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents ResaAltaDes As Global.System.Web.UI.HtmlControls.HtmlInputHidden

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
    '''Controllo ddlMasB.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents ddlMasB As Global.System.Web.UI.WebControls.DropDownList

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
    '''Controllo LabelMAS.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents LabelMAS As Global.System.Web.UI.WebControls.Label

    '''<summary>
    '''Controllo ddlMasS.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents ddlMasS As Global.System.Web.UI.WebControls.DropDownList

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
    '''Controllo LabelMAS_Nota.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents LabelMAS_Nota As Global.System.Web.UI.WebControls.Label

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
    '''Controllo Div_BTNSalva.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Div_BTNSalva As Global.System.Web.UI.HtmlControls.HtmlGenericControl

    '''<summary>
    '''Controllo Btn_Salva.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Btn_Salva As Global.System.Web.UI.WebControls.Button

    '''<summary>
    '''Controllo Div_Appezzamenti.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Div_Appezzamenti As Global.System.Web.UI.HtmlControls.HtmlGenericControl

    '''<summary>
    '''Controllo Div_NPK_Calcolati.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Div_NPK_Calcolati As Global.System.Web.UI.HtmlControls.HtmlGenericControl

    '''<summary>
    '''Controllo label17.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents label17 As Global.System.Web.UI.WebControls.Label

    '''<summary>
    '''Controllo Txt_N_Da_Applicare.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_N_Da_Applicare As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo Div_BtnApplica.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Div_BtnApplica As Global.System.Web.UI.HtmlControls.HtmlGenericControl

    '''<summary>
    '''Controllo Btn_Applica.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Btn_Applica As Global.System.Web.UI.WebControls.Button

    '''<summary>
    '''Controllo GridView_Impianti.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents GridView_Impianti As Global.System.Web.UI.WebControls.GridView

    '''<summary>
    '''Controllo UpdatePanelPerScript.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents UpdatePanelPerScript As Global.System.Web.UI.UpdatePanel

    '''<summary>
    '''Proprietà Master.
    '''</summary>
    '''<remarks>
    '''Proprietà generata automaticamente.
    '''</remarks>
    Public Shadows ReadOnly Property Master() As PianoConcimazione_2017.MasterConcimazione
        Get
            Return CType(MyBase.Master, PianoConcimazione_2017.MasterConcimazione)
        End Get
    End Property
End Class
