/*
* Creato il 18 - 07 - 2017
* 
* Controlli custom lato client inizialmente creati per uniformare i controlli dell'anagrafica 
*
* Author: Simone Galassi
*/

/**
 * Obj usato per validare i controlli lato client
 * se flag = false non ho superato i controlli
 * i vari n_inv contengono il numero di controlli invalidati per tab
 *
 * Corrisponde al var_contr delle funzioni custom seguenti
 */
function customValidator() {
    this.flag = true;
    this.n_inv = 0;
    this.n_inv2 = 0;
    this.n_inv3 = 0;
}

/**
 * controlla se il testo inserito è un numero
 * @param {any} id_controllo id lato client del controllo
 * @param {any} var_contr oggetto customValidator
 * @param {any} numero_tab numero della tab contenente il controllo
 */
function Controlla_numero(id_controllo, var_contr, numero_tab) {
    if ($('#' + id_controllo).val() === "") {
        return;
    }
    // Controllo se id_controllo contiene solo valori numerici
    if ($('#' + id_controllo).val().match(/^[0-9.,]+$/)) {
        var temp_num = $('#' + id_controllo).val().replace(",", ".");
        //controllo se è un numero
        if (isNaN(temp_num)) {
            $('#' + id_controllo).parent().append('<label id="' + id_controllo + '-error" class="custom_val error" for="' + id_controllo + '">Il campo non è un numero valido</label>');
            $('#' + id_controllo).closest("input").css('border', '1px solid #D41E1A');
            var_contr.flag = false;
            switch (numero_tab) {
                case 1:
                    var_contr.n_inv++;
                    break;
                case 2:
                    var_contr.n_inv2++;
                    break;
                case 3:
                    var_contr.n_inv3++;
                    break;
                default:
                    console.log("Errore switch Controlla_numero");
                    break;
            }
            return false;
        }
        $('#' + id_controllo).val(temp_num.replace(".", ","));
        return true;
    }
    else {
        $('#' + id_controllo).parent().append('<label id="' + id_controllo + '-error" class="custom_val error" for="' + id_controllo + '">Il campo può contenere solo numeri</label>');
        $('#' + id_controllo).closest("input").css('border', '1px solid #D41E1A');
        var_contr.flag = false;
        switch (numero_tab) {
            case 1:
                var_contr.n_inv++;
                break;
            case 2:
                var_contr.n_inv2++;
                break;
            case 3:
                var_contr.n_inv3++;
                break;
            default:
                console.log("Errore switch Controlla_numero");
                break;
        }
        return false;
    }
}

/**
 * controlla se la label è un numero
 * @param {any} id_controllo id lato client del controllo
 * @param {any} var_contr oggetto customValidator
 * @param {any} numero_tab numero della tab contenente il controllo
 */
function Controlla_numero_label(id_controllo, var_contr, numero_tab) {
    if ($('#' + id_controllo).html() === "") {
        return;
    }
    // Controllo se id_controllo contiene solo valori numerici
    if ($('#' + id_controllo).html().match(/^[0-9.,]+$/)) {
        var temp_num = $('#' + id_controllo).html().replace(",", ".");
        //controllo se è un numero
        if (isNaN(temp_num)) {
            $('#' + id_controllo).parent().append('<label id="' + id_controllo + '-error" class="custom_val error" for="' + id_controllo + '">Il campo non è un numero valido</label>');
            $('#' + id_controllo).closest("input").css('border', '1px solid #D41E1A');
            var_contr.flag = false;
            switch (numero_tab) {
                case 1:
                    var_contr.n_inv++;
                    break;
                case 2:
                    var_contr.n_inv2++;
                    break;
                case 3:
                    var_contr.n_inv3++;
                    break;
                default:
                    console.log("Errore switch Controlla_numero");
                    break;
            }
            return false;
        }
        $('#' + id_controllo).val(temp_num.replace(".", ","));
        return true;
    }
    else {
        $('#' + id_controllo).parent().append('<label id="' + id_controllo + '-error" class="custom_val error" for="' + id_controllo + '">Il campo può contenere solo numeri</label>');
        $('#' + id_controllo).closest("input").css('border', '1px solid #D41E1A');
        var_contr.flag = false;
        switch (numero_tab) {
            case 1:
                var_contr.n_inv++;
                break;
            case 2:
                var_contr.n_inv2++;
                break;
            case 3:
                var_contr.n_inv3++;
                break;
            default:
                console.log("Errore switch Controlla_numero");
                break;
        }
        return false;
    }
}

/**
 * Controlla se valorizzato il campo testo richiesto
 * @param {String} id_controllo id lato client del controllo
 * @param {obj} var_contr oggetto customValidator
 * @param {Number} numero_tab numero della tab contenente il controllo
 * @param {any} optional_unselectedValue OPZIONALE: se inserito confronta anche se non contiene questo valore
 */
function Controlla_required(id_controllo, var_contr, numero_tab, optional_unselectedValue) {

    optional_unselectedValue = (typeof optional_unselectedValue === 'undefined') ? "" : optional_unselectedValue;

    if ($('#' + id_controllo).val() == "" || $('#' + id_controllo).val() === optional_unselectedValue) {
        $('#' + id_controllo).parent().append('<label id="' + id_controllo + '-error" class="custom_val error" for="' + id_controllo + '">Il campo è obbligatorio</label>');
        $('#' + id_controllo).closest("input").css('border', '1px solid #D41E1A');
        var_contr.flag = false;
        switch (numero_tab) {
            case 1:
                var_contr.n_inv++;
                break;
            case 2:
                var_contr.n_inv2++;
                break;
            case 3:
                var_contr.n_inv3++;
                break;
            default:
                console.log("Errore switch Controlla_required");
                break;
        }
        return false;
    }
    return true;
}

/**
 * Controlla se valorizzato il campo label richiesto
 * @param {any} id_controllo id lato client del controllo
 * @param {any} var_contr oggetto customValidator
 * @param {any} numero_tab numero della tab contenente il controllo
 * @param {any} optional_unselectedValue OPZIONALE: se inserito confronta anche se non contiene questo valore
 */
function Controlla_required_label(id_controllo, var_contr, numero_tab, optional_unselectedValue) {

    optional_unselectedValue = (typeof optional_unselectedValue === 'undefined') ? "" : optional_unselectedValue;

    if ($('#' + id_controllo).html() == "" || $('#' + id_controllo).html() === optional_unselectedValue) {
        $('#' + id_controllo).parent().append('<label id="' + id_controllo + '-error" class="custom_val error" for="' + id_controllo + '">Il campo è obbligatorio</label>');
        $('#' + id_controllo).closest("input").css('border', '1px solid #D41E1A');
        var_contr.flag = false;
        switch (numero_tab) {
            case 1:
                var_contr.n_inv++;
                break;
            case 2:
                var_contr.n_inv2++;
                break;
            case 3:
                var_contr.n_inv3++;
                break;
            default:
                console.log("Errore switch Controlla_required");
                break;
        }
        return false;
    }
    return true;
}


//'#Region "utility per Storage"

const STORAGE_TYPE1 = "sessionStorage"

/**
 * Crea l'istanza di Storage
 * @param {string} type indica il tipo di storage da usare: "localStorage" o "sessionStorage"
 * @returns {Storage} Restituisce lo storage
 */
function getStorage(type) {
    var storage;
    try {
        storage = window[type];
        var x = '__storage_test__';
        storage.setItem(x, x);
        storage.removeItem(x);
        return storage;
    }
    catch (e) {
        return undefined;
    }
}

var storage = getStorage(STORAGE_TYPE1);

/**
 * Restituisce l'oggetto salvato in storage
 * @param {string} key Chiave dove è memorizzato l'oggetto
 * @returns {Object} Oggetto memorizzato
 */
function storageGetItem(key) {
    if (storage !== undefined) {
        try {
            let item = storage.getItem(key);
            if (item !== undefined) {
                return JSON.parse(item);
            } else {
                return undefined
            }
        } catch (e) {
            return undefined;
        }
    }
}

/**
 * Restituisce l'oggetto salvato in storage
 * @param {string} key Chiave dove memorizzare l'oggetto
 * @param {Object} value Oggetto da memorizzare
 */
function storageSetItem(key, value) {
    if (storage !== undefined) {
        try {
            if (value !== undefined) {
                storage.setItem(key, JSON.stringify(value));
            }
        } catch (e) {
            return undefined;
        }
    }
}

/**
 * Elimina l'oggetto salvato in storage
 * @param {string} key Chiave dove è memorizzato l'oggetto
 */
function storageRemoveItem(key) {
    if (storage !== undefined) {
        try {
            storage.removeItem(key);
        } catch (e) {
            return undefined;
        }
    }
}

/**
 * Elimina tutti gli oggetti salvati in storage
 */
function storageClear() {
    if (storage !== undefined) {
        try {
            storage.clear();
        } catch (e) {
            return undefined;
        }
    }
}

/**
 * Verifica l'esistenza della chiave nello storage
 * @param {string} key Chiave dove è memorizzato l'oggetto
 * @returns {Boolean} Presenza della chiave
 */
function storageExistItem(key) {
    if (storage !== undefined) {
        try {
            if (storage.getItem(key) !== undefined && storage.getItem(key) !== null) {
                return true;
            } else {
                return false;
            }
        } catch (e) {
            return false;
        }
    }
    return false;
}
//'#End Region "utility per Storage"

function SanitizeTesto_MantieniVirgoletteECaratteriAccentati(testo) {
    // 1. Caratteri pericolosi in HTML e JavaScript Injection:
    //    Tag HTML: <, >.
    //    Caratteri speciali HTML: & (entità HTML come & lt;), ' e " (delimitatori di attributi).
    //    Caratteri JavaScript: ()(funzioni), {}, [](strutture di codice), ;, = (assegnazioni).
    // 2. Caratteri pericolosi in SQL Injection:
    //    Separatori e commenti: ;, --, /, *.
    //    Delimitatori stringhe: ', ".
    // 3. Caratteri di escape:
    //     Backslash: \ (utilizzato per fare escaping).

    let pattern = /[^a-zA-Z0-9àèéìòùÀÈÉÌÒÙçÇ .,?!\-_]/g;
    testo = testo.replace(pattern, '');

    return testo;
}