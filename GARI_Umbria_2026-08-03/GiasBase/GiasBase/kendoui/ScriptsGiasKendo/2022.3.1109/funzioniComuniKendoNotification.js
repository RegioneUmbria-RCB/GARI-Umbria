// KENDO 2022.3.1109

function creaKendoDialog(
    id,
    type,
    title,
    content = "",
    actions = [],
    width = 500,
    height = 450,
) {

    const dialogClasses = [
        "xonne-dialog-success",
        "xonne-dialog-warning",
        "xonne-dialog-info",
        "xonne-dialog-error"
    ];

    let previousContent = `<div class="icon-container">
                                <i class="icon ${type}-dialog"></i>
                            </div>
                            <p class="title ${type}-dialog">${title}</p>`;
    if (!content) content = '';
    if (!content.startsWith('<p>')) content = '<p>' + content;
    if (!content.endsWith('</p>')) content = content + '</p>';

    $("#"+id).kendoDialog({
        width: width,
        height: height,
        title: title,
        closable: true,
        modal: true,
        visible: false,
        content: previousContent + content,
        actions: actions.length && actions || [
            { text: 'Conferma', primary: true },
            { text: 'Annulla' }
        ]
    });

    var dialog = $("#" + id).data("kendoDialog");
    var dialogWrapper = dialog.wrapper;

    $(dialogWrapper).removeClass(dialogClasses);
    dialogWrapper.addClass("xonne-dialog-" + type);
    dialog.open();
}

function creaKendoNotification(
    id,
    autoHideAfter = 0
) {

    function getHtml(type, additionalClasses) {
        return `<div class="gias-notification-container gias-notification-${type} ${additionalClasses}">
            <div class="icon"></div>
            <div class="content">
                <p class="gias-notification-title">#= title #</p>
                <p class="gias-notification-message">#= message #</p>
            </div>
        </div>`;
    }

    $("#" + id).kendoNotification({
        position: {
            pinned: true,
            bottom: 30,
            right: 30
        },
        autoHideAfter: autoHideAfter,
        hideOnClick: autoHideAfter <= 0,
        button: autoHideAfter <= 0,
        stacking: "up",
        templates: [{
            type: "info",
            template: getHtml("info", autoHideAfter <= 0 ? "gias-cursor-pointer" : "")
        }, {
            type: "error",
            template: getHtml("error", autoHideAfter <= 0 ? "gias-cursor-pointer" : "")
        }, {
            type: "success",
            template: getHtml("success", autoHideAfter <= 0 ? "gias-cursor-pointer" : "")
        }, {
            type: "warning",
            template: getHtml("warning", autoHideAfter <= 0 ? "gias-cursor-pointer" : "")
        }]
    });
}
