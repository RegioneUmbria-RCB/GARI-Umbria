
/* personalizzazioni jQueryUI */

function SliderjQueryUI() {

    $(".kendoSlider").hide();

    $('.aslider').each(function () {

        var selettoreSlide = "#" + this.id.toString().replace("slider", "slider-txt");
        var valoreSlide = parseFloat($(selettoreSlide).val().toString().replace(",", ".") * 100);

        console.log("Selettore -> " + selettoreSlide + " - " + valoreSlide);

        $(this).slider({
            min: 0,
            max: 100,
            step: 10,
            slide: function (event, ui) {                
                $("#" + this.id.toString().replace("slider", "slider-txt")).val(parseFloat(ui.value) / 100);
            }
        });

        $(this).slider("value", valoreSlide);
    });

}

function ColorPickerJQueryUI() {


    var objsel;
    $('.tavolozza').ColorPicker({
        onSubmit: function (hsb, hex, rgb, el) {
            objsel.val(hex);
            $('.tavolozza').ColorPickerHide();
        },
        onBeforeShow: function () {
            objsel = $(this);
            $(this).ColorPickerSetColor(this.value);
        }
    }).bind('keyup', function () {
        objsel.ColorPickerSetColor(this.value);
    });


}