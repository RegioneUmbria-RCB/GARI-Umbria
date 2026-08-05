var vettori = {
    remove: function (value, arr) {
        return jQuery.grep(arr, function (elem, index) {
            return elem !== value;
        });
    }
}



