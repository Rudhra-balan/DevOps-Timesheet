//Global Variable declaration
var weekIndex = 0;
var weekControlIndex = 5;




String.prototype.format = function () {
    var args = arguments;
    return this.replace(/\{\{|\}\}|\{(\d+)\}/g,
        function (m, n) {
            if (m === "{{") {
                return "{";
            }
            if (m === "}}") {
                return "}";
            }
            return args[n];
        });
};

String.prototype.isNullOrEmpty = function () {
    if (typeof this.valueOf() === "undefined" || this.valueOf() === null || this.valueOf() === "") return true;

    return this.valueOf().replace(/\s/g, "").length < 1;
}

function isNullOrWhitespace(input) {

    if (typeof input === "undefined" || input === null || input === "") return true;

    return input.toString().replace(/\s/g, "").length < 1;
}

Date.prototype.getWeek = function (start) {
    //Calcing the starting point
    start = start || 0;
    var today = new Date(this.setHours(0, 0, 0, 0));
    var day = today.getDay() - start;
    var date = today.getDate() - day;

    // Grabbing Start/End Dates
    var StartDate = new Date(today.setDate(date));
    var EndDate = new Date(today.setDate(date + 6));
    return [StartDate, EndDate];
}


// COMMON AREA START
var ErrorDisplayTypeEnum = {
    InForm: 1,

    Popup: 2
};

var NavPageEnum = {
    TimeSheetEnter: 0,
    Export:1
};

function formatDate(date) {
    var d = new Date(date),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();

    if (month.length < 2)
        month = '0' + month;
    if (day.length < 2)
        day = '0' + day;

    return [year, month, day].join('-');
}

var compare_dates = function (date1, date2) {
    if (date1 > date2) return true;
    else if (date1 < date2) return false;
    else return true;
}

function TryParseInt(str, defaultValue) {
    if (typeof str === "undefined" || str === null || str === "") return defaultValue;
    if (str.replace(/\s/g, "").length < 1) return defaultValue;
    var retValue = defaultValue;
    if (str !== null) {
        if (str.length > 0) {
            if (!isNaN(str)) {
                retValue = parseInt(str);
            }
        }
    }
    return retValue;
}
// String Format
function Dictionary() {
    var dictionary = {};

    this.setData = function (key, val) { dictionary[key] = val; };
    this.getData = function (key) { return dictionary[key]; };
    this.getAllKeys = function () {
        return Object.keys(dictionary);
    };
    this.getAllValue = function () {
        return Object.values(dictionary);
    };
    this.getFilterKeyByValue = function (input) {
        var keys = [];
        Object.entries(dictionary).forEach(([key, value]) => {
            if (value == input) {
                keys.push(key);
            }
        });

        return keys;
    };
    this.delete = function (key) {
        if (this.getData(key) !== undefined) {
            delete dictionary[key];
            return true;
        }
        return false;
    };
};



function isEmpty(value) {
    return (
        // null or undefined
        (value == null) ||
        (value == 'null')||
        // has length and it's zero
        (value.hasOwnProperty('length') && value.length === 0) ||

        // is an Object and has no keys
        (value.constructor === Object && Object.keys(value).length === 0)
    )
}

if (!Object.prototype.forEach) {
    Object.defineProperty(Object.prototype, 'forEach', {
        value: function (callback, thisArg) {
            thisArg = thisArg || window;
            for (var key in this) {
                if (this.hasOwnProperty(key)) {
                    callback.call(thisArg, this[key], key, this);
                }
            }
        }
    });
}

// COMMON AREA START

function onRequestBegin() {
    $.blockUI({
        message: '<svg version="1.1" class="loadsvg" id="Layer_1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" x="0px" y="0px" viewBox="0 0 307.7 97.3" enable-background="new 0 0 307.7 97.3" xml:space="preserve"><circle class="circle"fill="none" stroke="#8e44ad" stroke-width="7" stroke-miterlimit="10"  stroke-linecap="round" cx="74.6" cy="47" r="18.5"/> <g>	<path class="one" d="M50.4,58.9v9.4H20.6V26.4h10v32.4H50.4z"/><path class="two" d="M127.4,61.7h-16.5l-2.4,6.6H97.7l16.7-41.8h9.5l16.7,41.8h-10.8L127.4,61.7z M124.2,52.9l-3-8.1c-0.9-2.4-2-6.2-2-6.2h-0.1c-0.1,0-1.1,3.8-2,6.2l-3,8.1H124.2z"/>	<path class="three" d="M162.4,26.4c11.9,0,21.6,8.5,21.6,21.1s-9.7,20.7-21.6,20.7h-18.3V26.4H162.4z M162.4,58.9c6,0,11.2-4.2,11.2-11.3c0-7.2-5.2-11.7-11.2-11.7h-8.2v23.1H162.4z"/>	<path class="four" d="M188.2,68.2V26.4h10v41.8H188.2z"/>	<path class="five" d="M240.9,26.4v41.8h-9.4l-8.5-11.6c-3.8-5.3-9.4-13.2-9.5-13.2h-0.1c-0.1,0,0.1,6.4,0.1,15.6v9.3h-10V26.4h9.4l8.9,11.9c2.7,3.6,9.1,12.6,9.1,12.6h0.1c0.1,0-0.1-7.4-0.1-15v-9.6H240.9z"/>	<path class="six" d="M278.1,64.8c-2.4,2.6-7.2,4.2-11.3,4.2c-12,0-21.7-9.1-21.7-21.7c0-12.6,9.7-21.7,21.7-21.7c8.1,0,15.2,4.3,19,11.1l-10.2,2.7c-2-2.4-5.5-3.9-8.8-3.9c-6.9,0-11.2,5-11.2,11.8c0,7.3,5,11.9,11.8,11.9c6.7,0,9.6-3.5,10.8-6.6v-0.1h-11v-8.1h19.5		v23.7h-8.6C278.1,65.5,278.2,64.9,278.1,64.8L278.1,64.8z"/></g></svg>',
        fadeIn: 800,
        showOverlay: true,
        overlayCSS: {
            backgroundColor: '#1b2024',
            opacity: 0.8,
            zIndex: 1200,
            cursor: 'wait'
        },
        css: {
            border: 0,
            color: '#fff',
            zIndex: 1201,
            padding: 0,
            backgroundColor: 'transparent'
        }
    });
}

function onRequestComplete() {
    $.unblockUI(); 
}

// COMMON AREA END
// AJAX START
$(document).ajaxSend(function(event, jquery, settings) {
    if (settings.type.toUpperCase() !== "POST") return;
    jquery.setRequestHeader('RequestVerificationToken', $(".AntiForge" + " input").val());
});

function getCommon(url, targetElement, onSuccessCallback, onFailureCallback, callbackArgs,isLastElement, isCallbackWithData) {
    $.ajax({
        type: "GET",
        url: url,
        beforeSend: onRequestBegin(),
        error: function(data) {
            onRequestComplete();
            if (data.statusText !== null && data.statusText !== undefined) {
                showError(data.statusText, APPLICATION_LEVEL_ERROR_MESSAGE_TITLE, onFailureCallback);
            }
            else {
                showError(APPLICATION_LEVEL_ERROR_MESSAGE, APPLICATION_LEVEL_ERROR_MESSAGE_TITLE, onFailureCallback);
            }
        },
        success: function(data) {
            onRequestComplete();
            if (targetElement !== null && targetElement !== undefined) {
                if (isLastElement !== null && isLastElement !== undefined && isLastElement) {
                    $(`#${targetElement}`).append(data);
                    $(`#${targetElement}`).show();
                }
                else {
                    $(`#${targetElement}`).html(data);
                    $(`#${targetElement}`).show();
                }
            }
            if (onSuccessCallback !== undefined &&
                onSuccessCallback !== null &&
                callbackArgs !== undefined &&
                callbackArgs !== null)
                onSuccessCallback(callbackArgs);
            else if (onSuccessCallback !== undefined &&
                onSuccessCallback !== null &&
                isCallbackWithData !== undefined &&
                isCallbackWithData !== null &&
                isCallbackWithData === true)
                onSuccessCallback(data);
            else if (onSuccessCallback !== undefined && onSuccessCallback !== null)
                onSuccessCallback();

            
        }
    });
}

function postCommon(url,
    objData,
    targetElement,
    onSuccessCallback,
    onFailureCallback,
    popup,
    $grid,
    message,
    callbackArgs) {
    if (objData === null || objData === undefined)
        objData = "{}";
    $.ajax({
        type: "POST",
        url: url,
        data: JSON.stringify(objData),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: onRequestBegin(),
        error: function(data) {
            onRequestComplete();
            if (data.statusText !== null && data.statusText !== undefined) {
                showError(data.statusText, APPLICATION_LEVEL_ERROR_MESSAGE_TITLE, onFailureCallback);
            }
            else {
                showError(APPLICATION_LEVEL_ERROR_MESSAGE, APPLICATION_LEVEL_ERROR_MESSAGE_TITLE, onFailureCallback);
            }
        },
        success: function(data) {
            onRequestComplete();
            if (data !== null && data !== undefined && typeof data === "object") {
                if (data.sourceObject !== undefined && data.sourceObject !== null) {
                    if (data.sourceObject.errorList !== undefined && data.sourceObject.errorList.length > 0) {
                        if (data.id === undefined && data.message === undefined) {
                            data.id = data.sourceObject.errorList[0].id;
                            data.message = data.sourceObject.errorList[0].message;
                        }

                    } else if (data.sourceObject.id !== undefined &&
                        data.sourceObject.message !== undefined &&
                        data.sourceObject.message !== null) {
                        data.id = data.sourceObject.id;
                        data.message = data.sourceObject.message;
                    } else if (data.id === undefined && data.message === undefined) {
                        // There are no errors, but server did not set id and message correctly.
                        data.id = 0;
                        data.message = "";
                    }
                }

                if (data.isOperationSuccess !== undefined &&
                    data.isOperationSuccess === false &&
                    data.displayType === ErrorDisplayTypeEnum.InForm) {
                    message = data.errorDescription + "(Error Code: " + data.errorId + ")";
                    displayDangerMessage(message);
                    return;
                } else if (data.isOperationSuccess !== undefined &&
                    data.isOperationSuccess === false &&
                    (data.displayType === ErrorDisplayTypeEnum.Popup || data.displayType === 0)) {
                    hideAlertMessages();
                    message = data.errorDescription + "<br/><br/>Error Code: " + data.errorId;
                    showError(message, APPLICATION_LEVEL_ERROR_MESSAGE_TITLE, onFailureCallback);
                    return;
                } else if (data.redirectUrl !== undefined && data.redirectUrl !== "" && data.redirectUrl !== null) {
                    window.location.href = data.redirectUrl;
                } else if ((message === "") && (data.id > 0 || data.message !== "")) {
                    hideAlertMessages();
                    message = data.message + "<br/><br/>Error Code: " + data.id;
                    showError(message, APPLICATION_LEVEL_ERROR_MESSAGE_TITLE, onFailureCallback);
                    return;
                }
            }


            if (onSuccessCallback !== null && onSuccessCallback !== undefined) {
                if (message !== null && message !== undefined)
                    onSuccessCallback(message, callbackArgs);
                else if (data !== null && data !== undefined)
                    onSuccessCallback(data, callbackArgs);
                else if (callbackArgs !== null && callbackArgs !== undefined)
                    onSuccessCallback(callbackArgs);
                else
                    onSuccessCallback();
            }
        }
    });
}

function uploadCommon(url, formData, onSuccessCallback, onFailureCallback, logout, failMessage, successMessage) {
    $.ajax({
        url: url,
        type: "POST",
        contentType: false, // Not to set any content header  
        processData: false, // Not to process data  
        data: formData,
        beforeSend: onRequestBegin(),
        success: function(result) {
            onRequestComplete();
            if (result) {
                if (result.id !== 0 || result.id == null || result.id == undefined) {
                    displayDangerMessage(result.message);
                } else {
                    displaySettingSuccessMessage(successMessage);
                }

                if (onSuccessCallback != null)
                    onSuccessCallback();
            }
            else {
                displayDangerMessage(failMessage);
                if (onFailureCallback != null)
                    onFailureCallback();
            }
        },
        error: function (data) {
            onRequestComplete();
            if (data.statusText !== null && data.statusText !== undefined) {
                showError(data.statusText, APPLICATION_LEVEL_ERROR_MESSAGE_TITLE, onFailureCallback);
            }
            else {
                showError(APPLICATION_LEVEL_ERROR_MESSAGE, APPLICATION_LEVEL_ERROR_MESSAGE_TITLE, onFailureCallback);
            }
        }
    });
}

function downloadCommon(url, objData, onSuccessCallback, onFailureCallback, message) {
    $.ajax({
        url: url,
        data: { Data : JSON.stringify(objData) },
        method: "GET",
        xhrFields: {
            responseType: "blob"
        },
        beforeSend: onRequestBegin(),
        success: function(data, status, xhr) {
            onRequestComplete();
            const disposition = xhr.getResponseHeader("Content-Disposition");
            var filename = "";
            if (disposition) {
                const filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
                const matches = filenameRegex.exec(disposition);
                if (matches !== null && matches[1]) filename = matches[1].replace(/['"]/g, "");
                if (filename === "") {
                    // Error Message
                    return;
                }
            }
            const tempDownloadLink = document.createElement("a");
            const url = window.URL.createObjectURL(data);
            tempDownloadLink.href = url;
            tempDownloadLink.download = filename;
            document.body.append(tempDownloadLink);
            tempDownloadLink.click();
            tempDownloadLink.remove();
            window.URL.revokeObjectURL(url);
        },
        error: function (data) {
            onRequestComplete();
            if (data.statusText !== null && data.statusText !== undefined) {
                showError(data.statusText, APPLICATION_LEVEL_ERROR_MESSAGE_TITLE, onFailureCallback);
            }
            else {
                showError(APPLICATION_LEVEL_ERROR_MESSAGE, APPLICATION_LEVEL_ERROR_MESSAGE_TITLE, onFailureCallback);
            }
        }
    });
}
// AJAX END

// ALERT SECTION START

function hideAlertMessages() {
    $(".alert-messages-container").html("");
    $(".alert-messages-container").hide();
}

function displayInfoMessage(message) {
    if (message === undefined || message === "")
        message = "This is just for your information";
    setMessageStyleAndDisplayMessage("alert-info", message);
}

function displaySettingInfoMessage(message) {
    if (message === undefined || message === "")
        message = "This is just for your information";
    setMessageStyleAndDisplayMessage("alert-info-setting", message);
}

function displayDangerMessage(message) {
    if (message === undefined || message === "")
        message = "Error has occurred. please check";
    setMessageStyleAndDisplayMessage("alert-danger", message);
    $(".alert-messages-container").fadeTo(2000, 500).slideUp(500,
        function () {
            $(".alert-messages-container").slideUp(500);
        });
}

function displaySettingSuccessMessage(message) {
    if (message === undefined || message === "")
        message = "Successfully executed the task";
    setMessageStyleAndDisplayMessage("alert-success-setting", message);
    $(".alert-messages-container").fadeTo(2000, 500).slideUp(500,
        function () {
            $(".alert-messages-container").slideUp(500);
        });

}

function displaySuccessMessage(message) {
    if (message === undefined || message === "")
        message = "Successfully executed the task";
    setMessageStyleAndDisplayMessage("alert-success", message);
    $(".alert-messages-container").fadeTo(2000, 500).slideUp(500,
        function () {
            $(".alert-messages-container").slideUp(500);
        });
}

function displayWarningMessage(message) {
    if (message === undefined || message === "")
        message = "Warning! Please check";

    setMessageStyleAndDisplayMessage("alert-warning", message);
    $(".alert-messages-container").fadeTo(2000, 500).slideUp(500,
        function () {
            $(".alert-messages-container").slideUp(500);
        });
}

function setMessageStyleAndDisplayMessage(currentStyle, message) {
    $(".alert-messages-container")
        .removeClass(
            'alert-danger alert-info alert-info-setting alert-success alert-success-setting alert-warning alert-validation')
        .addClass(currentStyle);
    $(".alert-messages-container").html(message);
    $(".alert-messages-container").show().css("z-index", "999");
}

function displayValidationMessages(messages) {

    setMessageStyleAndDisplayMessage("alert-validation",
        convertStringArrayToULList(messages, SAVE_ERROR_MESSAGE_HEADER));
    $(".alert-messages-container").fadeTo(2000, 500).slideUp(500,
        function () {
            $(".alert-messages-container").slideUp(500);
        });
}

function displayValidationSetPinMessages(messages) {

    setMessageStyleAndDisplayMessage("alert-validation",
        convertStringArrayToULList(messages, "Set Pin Failed.Please correct the following errors and try again:"));
    $(".alert-messages-container").fadeTo(2000, 500).slideUp(500,
        function () {
            $(".alert-messages-container").slideUp(500);
        });
}
