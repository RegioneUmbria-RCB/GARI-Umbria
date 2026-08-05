
/**
* Creazione di una editor Kendo
*
* @param {string} IDControllo Rappresenta il selector JQuery del div a cui si associa l'editor
* @param {string} value contenuto iniziale del testo
* @param {object} editorTools parametri di configurazione
*/
function creaKendoEditor(
        // PARAMETRI OBBLIGATORI
        IDControllo,
        value,
        editorTools,
        autorizzazioni) {

    if (IDControllo == null) {
        alert("Non mi hai passato l'ID del DIV che contiene l'editor");
        return;
    }

    if (value === undefined || value === null) {
        value = "";
    }  


    // Se il parametro non viene passato si assume che l'utente abbia i permessi
    autorizzazioni.UtenteAbilitatoInserimentoModifica = (typeof autorizzazioni.UtenteAbilitatoInserimentoModifica === 'undefined') ? true : autorizzazioni.UtenteAbilitatoInserimentoModifica;

    editorTools = (typeof editorTools === 'undefined' || editorTools == null) ? {} : editorTools;
    editorTools.bold = (typeof editorTools.bold === 'undefined') || editorTools.bold ? "bold" : editorTools.bold;
    editorTools.italic = (typeof editorTools.italic === 'undefined') || editorTools.italic ? "italic" : editorTools.italic;
    editorTools.underline = (typeof editorTools.underline === 'undefined') || editorTools.underline ? "underline" : editorTools.underline;
    editorTools.strikethrough = (typeof editorTools.strikethrough === 'undefined') || editorTools.strikethrough ? "strikethrough" : editorTools.strikethrough;
    editorTools.justifyLeft = (typeof editorTools.justifyLeft === 'undefined') || editorTools.justifyLeft ? "justifyLeft" : editorTools.justifyLeft;
    editorTools.justifyCenter = (typeof editorTools.justifyCenter === 'undefined') || editorTools.justifyCenter ? "justifyCenter" : editorTools.justifyCenter;
    editorTools.justifyRight = (typeof editorTools.justifyRight === 'undefined') || editorTools.justifyRight ? "justifyRight" : editorTools.justifyRight;
    editorTools.justifyFull = (typeof editorTools.justifyFull === 'undefined') || editorTools.justifyFull ? "justifyFull" : editorTools.justifyFull;
    editorTools.insertUnorderedList = (typeof editorTools.insertUnorderedList === 'undefined') || editorTools.insertUnorderedList ? "insertUnorderedList" : editorTools.insertUnorderedList;
    editorTools.insertOrderedList = (typeof editorTools.insertOrderedList === 'undefined') || editorTools.insertOrderedList ? "insertOrderedList" : editorTools.insertOrderedList;
    editorTools.indent = (typeof editorTools.indent === 'undefined') || editorTools.indent ? "indent" : editorTools.indent;
    editorTools.outdent = (typeof editorTools.outdent === 'undefined') || editorTools.outdent ? "outdent" : editorTools.outdent;
    editorTools.createLink = (typeof editorTools.createLink === 'undefined') || editorTools.createLink ? "createLink" : editorTools.createLink;
    editorTools.unlink = (typeof editorTools.unlink === 'undefined') || editorTools.unlink ? "unlink" : editorTools.unlink;
    editorTools.insertImage = (typeof editorTools.insertImage === 'undefined') || editorTools.insertImage ? "insertImage" : editorTools.insertImage;
    editorTools.insertFile = (typeof editorTools.insertFile === 'undefined') || editorTools.insertFile ? "" : editorTools.insertFile;
    editorTools.subscript = (typeof editorTools.subscript === 'undefined') || editorTools.subscript ? "subscript" : editorTools.subscript;
    editorTools.superscript = (typeof editorTools.superscript === 'undefined') || editorTools.superscript ? "superscript" : editorTools.superscript;
    editorTools.tableWizard = (typeof editorTools.tableWizard === 'undefined') || editorTools.tableWizard ? "tableWizard" : editorTools.tableWizard;
    editorTools.createTable = (typeof editorTools.createTable === 'undefined') || editorTools.createTable ? "createTable" : editorTools.createTable;
    editorTools.addRowAbove = (typeof editorTools.addRowAbove === 'undefined') || editorTools.addRowAbove ? "addRowAbove" : editorTools.addRowAbove;
    editorTools.addRowBelow = (typeof editorTools.addRowBelow === 'undefined') || editorTools.addRowBelow ? "addRowBelow" : editorTools.addRowBelow;
    editorTools.addColumnLeft = (typeof editorTools.addColumnLeft === 'undefined') || editorTools.addColumnLeft ? "addColumnLeft" : editorTools.addColumnLeft;
    editorTools.addColumnRight = (typeof editorTools.addColumnRight === 'undefined') || editorTools.addColumnRight ? "addColumnRight" : editorTools.addColumnRight;
    editorTools.deleteRow = (typeof editorTools.deleteRow === 'undefined') || editorTools.deleteRow ? "deleteRow" : editorTools.deleteRow;
    editorTools.deleteColumn = (typeof editorTools.deleteColumn === 'undefined') || editorTools.deleteColumn ? "deleteColumn" : editorTools.deleteColumn;
    editorTools.viewHtml = (typeof editorTools.viewHtml === 'undefined') || editorTools.viewHtml ? "" : editorTools.viewHtml;
    editorTools.formatting = (typeof editorTools.formatting === 'undefined') || editorTools.formatting ? "formatting" : editorTools.formatting;
    editorTools.cleanFormatting = (typeof editorTools.cleanFormatting === 'undefined') || editorTools.cleanFormatting ? "cleanFormatting" : editorTools.cleanFormatting;
    editorTools.fontName = (typeof editorTools.fontName === 'undefined') || editorTools.fontName ? "fontName" : editorTools.fontName;
    editorTools.fontSize = (typeof editorTools.fontSize === 'undefined') || editorTools.fontSize ? "fontSize" : editorTools.fontSize;
    editorTools.foreColor = (typeof editorTools.foreColor === 'undefined') || editorTools.foreColor ? "foreColor" : editorTools.foreColor;
    editorTools.backColor = (typeof editorTools.backColor === 'undefined') || editorTools.backColor ? "backColor" : editorTools.backColor;
    editorTools.print = (typeof editorTools.print === 'undefined') || editorTools.print ? "print" : editorTools.print;
    
    editorTools.pdf = (typeof editorTools.pdf === 'undefined') || editorTools.pdf ? "pdf" : editorTools.pdf;
     

    $("#" + IDControllo).kendoEditor(
        {
            tools: [
                editorTools.bold,
                editorTools.italic,
                editorTools.underline,
                editorTools.strikethrough,
                editorTools.justifyLeft,
                editorTools.justifyCenter,
                editorTools.justifyRight,
                editorTools.justifyFull,
                editorTools.insertUnorderedList,
                editorTools.insertOrderedList,
                editorTools.indent,
                editorTools.outdent,
                editorTools.createLink,
                editorTools.unlink,
                editorTools.insertImage,
                editorTools.insertFile,
                editorTools.subscript,
                editorTools.superscript,
                editorTools.tableWizard,
                editorTools.createTable,
                editorTools.addRowAbove,
                editorTools.addRowBelow,
                editorTools.addColumnLeft,
                editorTools.addColumnRight,
                editorTools.deleteRow,
                editorTools.deleteColumn,
                editorTools.viewHtml,
                editorTools.formatting,
                editorTools.cleanFormatting,
                editorTools.fontName,
                editorTools.fontSize,
                editorTools.foreColor,
                editorTools.backColor,
                editorTools.print,
                editorTools.pdf
            ],
            pdf: {
                paperSize: "A4",
                margin: {
                    bottom: 20,
                    left: 20,
                    right: 20,
                    top: 20
                }
            }
        }
    );
    var kendo_editor = $("#" + IDControllo).data("kendoEditor");
    kendo_editor.value(value);

    if (!autorizzazioni.UtenteAbilitatoInserimentoModifica)
        $($("#" + IDControllo).data().kendoEditor.body).attr('contenteditable', false);

    kendo.pdf.defineFont({
        "DejaVu Sans": "https://kendo.cdn.telerik.com/2022.3.1109/styles/fonts/DejaVu/DejaVuSans.ttf",
        "DejaVu Sans|Bold": "https://kendo.cdn.telerik.com/2022.3.1109/styles/fonts/DejaVu/DejaVuSans-Bold.ttf",
        "DejaVu Sans|Bold|Italic": "https://kendo.cdn.telerik.com/2022.3.1109/styles/fonts/DejaVu/DejaVuSans-Oblique.ttf",
        "DejaVu Sans|Italic": "https://kendo.cdn.telerik.com/2022.3.1109/styles/fonts/DejaVu/DejaVuSans-Oblique.ttf"
    });

    return kendo_editor;

}   