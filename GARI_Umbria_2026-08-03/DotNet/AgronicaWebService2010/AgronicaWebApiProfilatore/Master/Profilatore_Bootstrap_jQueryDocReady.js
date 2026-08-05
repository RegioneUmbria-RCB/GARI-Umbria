//DOCUMENT READY
jQuery(function () {


    console.log('Start..DocReady: ' + GetTime());

    MasterPage_InizilizzaControlli();




    console.log('End..DocReady: ' + GetTime());

});


function MasterPage_InizilizzaControlli() {

    $('.datepicker').datepicker({ format: 'dd/mm/yyyy', autoclose: true });
    $('.selectpicker').selectpicker();

}


function GetTime() {
    var currentdate = new Date();
    var datetime = currentdate.getDate() + "/" + (currentdate.getMonth()+1)
        + "/" + currentdate.getFullYear() + " @ "
        + currentdate.getHours() + ":"
        + currentdate.getMinutes() + ":" + currentdate.getSeconds() + "." + currentdate.getMilliseconds();

    return datetime;
}