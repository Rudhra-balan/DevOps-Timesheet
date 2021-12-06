var gridDataViewContent =
    "<div id='gridDataContainer' class='dialog-container'><div id='gridDataInnerContainer' class='dialog-backdrop'><div id='gridDataPopup' class='dialog grid-data-dialog'><span class='dialog-title'><i class='fa fa-th'>&nbsp;</i></span><button type='button' id='closeGridDataButton' class='close-button'><i class='fa fa-times'></i></button><div class='confirm-content'></div></div></div></div>";
var errorDialogContent =
    "<div id='errorContainer' class='dialog-container'><div id='errorInnerContainer' class='dialog-backdrop'><div id='errorPopup' class='dialog confirm'><span class='dialog-title'><i class='fa fa-times-circle'>&nbsp;</i></span><div class='confirm-content'><span class='error-message'></span></div><div class='confirm-buttons'><button type='button' id='errorOK' class='ok-button'>Close</button></div></div></div></div>";
var confirmDialogContent =
    "<div id='confirmContainer' class='dialog-container'><div id='confirmInnerContainer' class='dialog-backdrop'><div id='confirmPopup' class='dialog confirm'><span class='dialog-title'><i class='fa fa-question-circle'>&nbsp;</i></span><button type='button' tabindex='-1' id='closeConfirmButton' class='close-button'><i class='fa fa-times'></i></button><div class='confirm-content'><span class='confirm-message'></span></div><div class='confirm-buttons'><button type='button' id='confirmCancel' class='ok-button'>Cancel</button>&nbsp;&nbsp;<button type='button' id='confirmOK' class='ok-button'>OK</button></div></div></div></div>";

function showConfirm(message, title, okCallback, cancelCallback, callbackArgs) {

    if (message == null || message.trim().length === 0)
        return false;

    if (title == null || title.trim().length === 0)
        title = "Confirm";

    var html = confirmDialogContent;

    $(document.body).append(html);

    $("#confirmPopup").find(".dialog-title").html(title);
    $("#confirmPopup").find(".confirm-message").html(message);

    var handlers = [okCallback, cancelCallback, callbackArgs];

    var dialog = $("#confirmPopup");
    var maxZIndex = $(".dialog");
    dialog.css("z-index", (maxZIndex + 1));

    toggleDialog("confirmPopup", true, hookupConfirmEvents, handlers);
}

function showError(message, title, callback, callbackArgs) {
    if (message == null || message.trim().length === 0)
        return false;

    if (title == null || title.trim().length === 0)
        title = "Error";

    var html = errorDialogContent;

    $(document.body).append(html);

    $("#errorPopup").find(".dialog-title").html(title);
    $("#errorPopup").find(".error-message").html(message);

    var dialog = $("#errorPopup");
    var maxZIndex = $(".dialog");

    dialog.css("z-index", (maxZIndex + 10));

    toggleDialog("errorPopup", true, hookupErrorEvents, callback, callbackArgs);
    return false;
}

function showInformation(message, title, okCallback, callbackArgs) {
    if (message == null || message.trim().length == 0)
        return false;

    if (title == null || title.trim().length == 0)
        title = "Error";

    var html = errorDialogContent;

    $(document.body).append(html);

    $("#errorPopup").find(".dialog-title").html(title);
    $("#errorPopup").find(".error-message").html(message);

    var dialog = $("#errorPopup");
    var maxZIndex = $(".dialog");
    maxZIndex = 99;
    dialog.css("z-index", (maxZIndex + 1));

    toggleDialog("errorPopup", true, hookupErrorEvents, okCallback, callbackArgs);
}

function hideDialog(dialogId, buttonName, callback) {
    $(`#${dialogId}`).animate({
        opacity: "hide"
    },
        {
            duration: 300,
            complete: function () {
                $(`#${dialogId}`).parents(".dialog-container").css("display", "none");

                if (callback != null)
                    callback(false, callbackArgs);

                var eventData = [{ dialogId: dialogId, result: buttonName }];

                $(document).trigger("dialogClose", eventData);
            }
        });
}

function hookupConfirmEvents(visible, handlers) {
    var okButton = $("#confirmPopup").find("#confirmOK");

    $(okButton).unbind();
    $(okButton).focus();
    $(okButton).click(function (e) {
        if (handlers != null && handlers.length > 0 && handlers[0] != null && handlers[2] != null)
            handlers[0](handlers[2]);
        else
            handlers[0]();

        toggleDialog("confirmPopup", false, null, null, "OK");
    });

    var closeConfirmButton = $("#confirmPopup").find("#closeConfirmButton");
    $(closeConfirmButton).unbind();
    $(closeConfirmButton).focus();
    $(closeConfirmButton).click(function (e) {
        toggleDialog("confirmPopup", false, null, null, "OK");
    });

    var cancelButton = $("#confirmPopup").find("#confirmCancel");
    $(cancelButton).unbind();
    $(cancelButton).click(function (e) {
        if (handlers != null && handlers.length > 1 && handlers[1] != null)
            handlers[1]();

        toggleDialog("confirmPopup", false, null, null, "Cancel");
    });

    $("#confirmPopup").on("close",
        function (e, eventData) {
            $("#confirmPopup").parent().parent().remove();
        });
}

function hookupErrorEvents(visible, handler, handlerArgs) {
    var okButton = $("#errorPopup").find("#errorOK");

    $(okButton).focus();

    $(okButton).click(function (e) {
        if (handler != null)
            handler(handlerArgs);

        toggleDialog("errorPopup", false, null, null, "OK");
    });

    $("#errorPopup").on("close",
        function (e, eventData) {
            if (handler != null)
                handler(handlerArgs);

            $("#errorPopup").parent().parent().remove();
        });
}

function toggleDialog(dialogId, show, callback, callbackArgs, buttonName) {
    var container = $(`#${dialogId}`).parents(".dialog-container");

    if (container.length > 0) {
        let maxZ = $(".dialog-container");

        maxZ = (maxZ > 100) ? 200 : maxZ;
        container.css("z-index", maxZ + 1);
    }

    if (show === true) {
        $(`#${dialogId}`).parents(".dialog-container").css("display", "block");
        $(`#${dialogId}`).animate({
            opacity: "show"
        },
            {
                duration: 300,
                complete: function () {
                    $(`#${dialogId}`).tabGuard();
                    $(`#${dialogId}`).find("input:first").focus();

                    if (typeof refreshCollaborators != "undefined")
                        refreshCollaborators(null, dialogId);

                    if (callback != null)
                        callback(true, callbackArgs);
                }
            });
    } else {
        hideDialog(dialogId, buttonName, callback);
    }
}

function convertStringArrayToULList(stringArray, headerMessage) {
    if (stringArray === null || stringArray === undefined)
        return "";
    var htmlMessage = "";
    if (headerMessage !== undefined && headerMessage !== null) {
        htmlMessage = `<label class='message-header'>${headerMessage}</label>`;
    }

    htmlMessage += '<ul class="message-ul">';
    $.each(stringArray,
        function (i, v) {
            htmlMessage += `<li>${v}</li>`;
        });
    htmlMessage += "<ul>";
    return htmlMessage;

}