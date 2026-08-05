$(document).ready(function () {

    kendo_LetamazioniPrecedenti_Leggi();
    popolaGrigliaLetamazioniPrecedenti('kendo_LetamazioniPrecedenti');
    $('#kendo_LetamazioniPrecedenti .k-footer-template').addClass("allineadestra");

    kendo_LetamazioniPrecedentiQdC_Leggi();
    popolaGrigliaLetamazioniPrecedentiQdC('kendo_LetamazioniPrecedentiQdC');
    $('#kendo_LetamazioniPrecedentiQdC .k-footer-template').addClass("allineadestra");

});