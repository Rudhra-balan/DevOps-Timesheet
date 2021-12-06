
function onExportLoad() {
    $("#exportBtn").click(function () {
        onExportClick();
    });

    
    enableDatePicker();
   
}



function enableDatePicker() {
    flatpickr(document.getElementById('export-start-date_1'), {
        enableTime: false,
        dateFormat: "Y-m-d"
    });


    flatpickr(document.getElementById('export-start-date_2'), {
        enableTime: false,
        dateFormat: "Y-m-d"
    });


    flatpickr(document.getElementById('export-end-date_1'), {
        enableTime: false,
        dateFormat: "Y-m-d",
        maxDate: new Date()
    });


    flatpickr(document.getElementById('export-end-date_2'), {
        enableTime: false,
        dateFormat: "Y-m-d"
    });

}

function ValidateExport() {
    if ($("#export-start-date").val() === undefined || $("#export-start-date").val() === '') {
        displayDangerMessage("Please Enter the Start Date");
        return false;
    }
    if ($("#export-end-date").val() === undefined || $("#export-end-date").val() === '') {
        displayDangerMessage("Please Enter the End Date");
        return false;
    }
   
    return true;
}


function onExportSuccess() {
    displaySuccessMessage(EXPORT_SUCCESS);
}

function OnExportFailure() {
    displayDangerMessage(EXPORT_ERROR);
}

var exportQueryControlIndex = 2;
function addQueryControl() {
    exportQueryControlIndex = exportQueryControlIndex + 1;
    const url = `Export/AddQueryControl/${exportQueryControlIndex}`;
    getCommon(url, `queryDiv`, onAddQueryControlSuccess, null, null, true);

}
function onAddQueryControlSuccess() {
    flatpickr(document.getElementById(`export-start-date_${exportQueryControlIndex}`), {
        enableTime: false,
        dateFormat: "Y-m-d"
    });

    flatpickr(document.getElementById(`export-end-date_${exportQueryControlIndex}`), {
        enableTime: false,
        dateFormat: "Y-m-d",
        maxDate: new Date()
    });

  
}
function removeQueryControl(parent) {
    parent.remove();
}

function onFieldChange(ctrl, ctrlIndex) {
    $(`#queryValue_${ctrlIndex}`).removeAttr('disabled');
    switch (ctrl.value.trim()) {
        case 'Start-TimeSheetDate':
            $(`#export-start-date-container_${ctrlIndex}`).show();
            $(`#export-end-date-container_${ctrlIndex}`).hide();
            $(`#value-dropdown-container_${ctrlIndex}`).hide();
            $(`#department-dropdown-container_${ctrlIndex}`).hide();
            dateOperator(ctrlIndex);
            break;
        case 'End-TimeSheetDate':
            $(`#export-start-date-container_${ctrlIndex}`).hide();
            $(`#export-end-date-container_${ctrlIndex}`).show();
            $(`#value-dropdown-container_${ctrlIndex}`).hide();
            $(`#department-dropdown-container_${ctrlIndex}`).hide();
            dateOperator(ctrlIndex);
            break;
        case 'userInfo.DepartmentId':
            $(`#export-start-date-container_${ctrlIndex}`).hide();
            $(`#export-end-date-container_${ctrlIndex}`).hide();
            $(`#value-dropdown-container_${ctrlIndex}`).hide();
            $(`#department-dropdown-container_${ctrlIndex}`).show();
            departmentOperator(ctrlIndex);
            break;
        case 'userInfo.EmailAddress':
            $(`#export-start-date-container_${ctrlIndex}`).hide();
            $(`#export-end-date-container_${ctrlIndex}`).hide();
            $(`#value-dropdown-container_${ctrlIndex}`).show();
            $(`#department-dropdown-container_${ctrlIndex}`).hide();
            userOperator(ctrlIndex);
            break;
        default:
    }
   
}

function dateOperator(index) {
    var options = [];
    $(`#queryOperator_${index}`).find('option').remove().end()
    options.push("<option value='='> = </option>");
    options.push("<option value='>='> >= </option>");
    options.push("<option value='<='> <= </option>");
    $(`#queryOperator_${index}`).append(options.join("")).selectmenu('refresh');
    $(`#queryOperator_${index}`).prop('selectedIndex', 0);
}

function departmentOperator(index) {
    var options = [];
    $(`#queryOperator_${index}`).find('option').remove().end()
    options.push("<option value='='> = </option>");
    options.push("<option value='!='> != </option>");
    options.push("<option value='in'> In </option>");
    options.push("<option value='not in'> Not in </option>");
    $(`#queryOperator_${index}`).append(options.join("")).selectmenu('refresh');
    $(`#queryOperator_${index}`).prop('selectedIndex', 0);
}

function userOperator(index) {
    var options = [];
    $(`#queryOperator_${index}`).find('option').remove().end()
    options.push("<option value='='> = </option>");
    options.push("<option value='!='> != </option>");
    options.push("<option value='like'> contains </option>");
    $(`#queryOperator_${index}`).append(options.join("")).selectmenu('refresh');
    $(`#queryOperator_${index}`).prop('selectedIndex', 0);
}

function onExportClick() {
    var exportRequestList = [];
    for (var exportIndex = 1; exportIndex <= exportQueryControlIndex; exportIndex++) {
        var exportObj = {}
        var isGroupSelected = document.querySelector(`#queryGroup_${exportIndex}`).checked;
        if (isNullOrWhitespace(isGroupSelected)) {
            continue;
        }
        exportObj.IsGroupSelected = $("#isQueryGrouped").is(":checked") && isGroupSelected;
        var field = $(`#queryField_${exportIndex}`).val();
        if (isNullOrWhitespace(field)) {
            showError('Please select the field to export the timesheet','Error')
            return;
        }
        exportObj.Field = field;

        var operator = $(`#queryOperator_${exportIndex}`).val();
        if (isNullOrWhitespace(operator)) {
            showError('Please select the operator to export the timesheet', 'Error')
            return;
        }

        exportObj.Operator = operator;
        exportObj.LogicOperator = $(`#queryLogic_${exportIndex}`).val();;
        GetFieldValue(field, exportIndex, exportObj);
        exportRequestList.push(exportObj);
    }

  
    var url = "Export/ExportTimeSheet";
    downloadCommon(url, exportRequestList, onExportSuccess, OnExportFailure);
}

function onQueryGroupChange(ctrl) {
    var sequenceDictionary = new Dictionary();
    $(".query-group").each(function (key, value) {
        sequenceDictionary.setData(key, value.checked);
    });

    var groupedArray = sequenceDictionary.getFilterKeyByValue(true);
    var differenceAry = groupedArray.slice(1).map(function (n, i) { return n - groupedArray[i]; })
    ctrl.checked = differenceAry.every(value => value == 1)
}

function isQueryCanGroup() {
  
    if ($('#isQueryGrouped:checked').val() === undefined) return;
    var sequenceDictionary = new Dictionary();
    $(".query-group").each(function (key, value) {
        sequenceDictionary.setData(key, value.checked);
    });

    var groupedArray = sequenceDictionary.getFilterKeyByValue(true);
    var differenceAry = groupedArray.slice(1).map(function (n, i) { return n - groupedArray[i]; })
    $("#isQueryGrouped").prop("checked", differenceAry.every(value => value == 1));
   
}

function GetFieldValue(fieldValue,exportIndex, exportObj) {
    switch (fieldValue.trim()) {
        case 'Start-TimeSheetDate':
            exportObj.Field = 'TimeSheetDate';
            var startDateValue = $(`#export-start-date_${exportIndex}`).val();
            if (isNullOrWhitespace(startDateValue)) {
                showError('Please select the start date to export the timesheet', 'Error')
                return;
            }

            exportObj.FieldValue = startDateValue;
           
            
            break;
        case 'End-TimeSheetDate':
            exportObj.Field = 'TimeSheetDate';
            var endDateValue = $(`#export-end-date_${exportIndex}`).val();
            if (isNullOrWhitespace(endDateValue)) {
                showError('Please select the end date to export the timesheet', 'Error')
                return;
            }
            exportObj.FieldValue = endDateValue;
            break;
        case 'userInfo.DepartmentId':
            var departmentIdValue = $(`#queryDepartment_${exportIndex}`).val();
            if (isNullOrWhitespace(departmentIdValue)) {
                showError('Please select the department Value to export the timesheet', 'Error')
                return;
            }
            exportObj.FieldValue = departmentIdValue;
            break;
        case 'userInfo.EmailAddress':
            var emailAddressValue = $(`#queryValue_${exportIndex}`).val();
            if (isNullOrWhitespace(emailAddressValue)) {
                showError('Please select the EmailAddress Value to export the timesheet', 'Error')
                return;
            }
            exportObj.FieldValue = emailAddressValue;
            break;
        default:
    }
}