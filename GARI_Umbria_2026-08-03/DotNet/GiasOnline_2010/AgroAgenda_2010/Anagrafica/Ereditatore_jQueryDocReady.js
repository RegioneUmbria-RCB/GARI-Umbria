
// ----------------------------------------------
// '  Galassi, 10/04/2017 16.44.44:  Ereditatore ready
// 
// ----------------------------------------------
$(document).ready(function () {

	//Do something
	LeggiElencoProprieta();
	KendoProprieta("kendo_ElencoProp");
	//MultiKendoProprieta("kendo_ElencoProp");
	$("#datepicker_distinta").kendoDatePicker(
		/*{
			value: convertDate(Date.toString())
		}*/
	);
	//GrigliaKendoEreditatore("kendo_EredImpianti");

}); // end ready