$(document).ready(function () {
    $('#Txt_DataInizio').kendoDatePicker();
    $('#Txt_DataFine').kendoDatePicker();
    $('#Txt_Mese').kendoDatePicker({
        // defines the start view
        start: "year",
        // defines when the calendar should return date
        depth: "year",
        // display month and year in the input
        format: "MM/yyyy",
        // specifies that DateInput is used for masking the input element
        dateInput: true
    });
    var currentTime = new Date();
    var month = currentTime.getMonth() + 1;
    var day = currentTime.getDate();
    var year = currentTime.getFullYear();
    $('#Txt_Mese').val(month + '/' + year);
    $('#MeseDiv').show();
    $('#TemporaleDiv').hide();
    $('#Txt_DataInizio').val('');
    $('#Txt_DataFine').val('');
    $('input[name=finestraTemporale]').change(function () {
        var selValue = $('input[name=finestraTemporale]:checked').val();
        switch (selValue) {
            case "0": //Mese
                $('#MeseDiv').show();
                $('#TemporaleDiv').hide();
                $('#Txt_DataInizio').val('');
                $('#Txt_DataFine').val('');
                $('#Txt_Mese').val('');
                break;
            case "1": //Temporale
                $('#TemporaleDiv').show();
                $('#MeseDiv').hide();
                $('#Txt_DataInizio').val('');
                $('#Txt_DataFine').val('');
                $('#Txt_Mese').val('');
                break;
        }
    });
});