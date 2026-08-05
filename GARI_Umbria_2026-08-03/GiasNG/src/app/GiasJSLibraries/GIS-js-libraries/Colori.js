var colori = {
    hexToRgb: function (h) {
        var cutH = this.cutHex(h);
        var r = parseInt(cutH.substring(0, 2), 16),
            g = parseInt(cutH.substring(2, 4), 16),
            b = parseInt(cutH.substring(4, 6), 16);
        return r + '|' + g + '|' + b;
    }
    ,
    cutHex: function (h) {
        return (h.charAt(0) == "#") ? h.substring(1, 7) : h
    }
    ,
    RGB2Color: function (r, g, b) {
        return '#' + this.byte2Hex(r) + this.byte2Hex(g) + this.byte2Hex(b);
    }
    ,
    byte2Hex: function (n) {
        var nybHexString = "0123456789ABCDEF";
        return String(nybHexString.substring((n >> 4) & 0x0F, 1)) + nybHexString.substr(n & 0x0F, 1);
    }
    ,
    calcolaColore: function (v_min, v_max, valore, colore1, colore2, varianza) {
        if (colore2 == "") {
            return colore1;
        } else {
            var range = [{
                0: parseInt(this.hexToRgb(colore1).split("|")[0]),
                1: parseInt(this.hexToRgb(colore2).split("|")[0])
            }, {
                0: parseInt(this.hexToRgb(colore1).split("|")[1]),
                1: parseInt(this.hexToRgb(colore2).split("|")[1])
            }, {
                0: parseInt(this.hexToRgb(colore1).split("|")[2]),
                1: parseInt(this.hexToRgb(colore2).split("|")[2])
            }];

            var n_step = (v_max - v_min) / varianza;


            var moltiplicatore = 0.0;
            moltiplicatore = parseFloat(((valore - v_min) / n_step) - 0.5);


            if (moltiplicatore < 0) moltiplicatore = -moltiplicatore;
            var app;

            var r, g, b;

            app = (range[0]['1'] - range[0]['0']) / (varianza - 1);
            r = parseInt(parseInt(range[0]['0']) + parseInt(app * moltiplicatore));

            app = (range[1]['1'] - range[1]['0']) / (varianza - 1);
            g = parseInt(parseInt(range[1]['0']) + parseInt(app * moltiplicatore));

            app = (range[2]['1'] - range[2]['0']) / (varianza - 1);
            b = parseInt(parseInt(range[2]['0']) + parseInt(app * moltiplicatore));

            return this.RGB2Color(r, g, b);
        }
    }
}

export {colori};
