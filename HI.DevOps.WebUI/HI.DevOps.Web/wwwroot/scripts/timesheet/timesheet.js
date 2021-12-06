var selectedProjectIndex;
var selectedTaskIndex;
var calendar;
var calendarEditEvent;


function onTimeSheetPageLoad() {
    onMonthViewLoad();
    onWeekViewLoad();
    $("#closeForm").click(function () {
        $("#feedback_form_area").animate({
            width: "toggle"
        });
        $('.modal-backdrop').remove();
        $(`#monthProjectId`).val('');
        $("#select2-monthProjectId-container").text('--Select Project--');
        $(`#monthTask`).val('');
        $(`#monthTask`).find('option').remove().end();
        $('#taskMonthHours').val('');
    });
    $("#OpenForm").click(function () {
        $(`#taskMonthHours`).prop("readonly", false);
        $("#taskMonthHours").val('');

        flatpickr(document.getElementById('task-start-date'), {
            enableTime: false,
            dateFormat: "Y-m-d",
            maxDate: new Date()
        });

        var percent = 0.65;
        var add_width = (percent * $('#feedback-form').parent().width()) + 'px';
        $("#feedback_form_area").width(add_width);
        $("#feedback_form_area").animate({
            width: "toggle"
        });

        BackDrop();
    });
    $(".basic").select2({
        tags: true
    });
    $(".popup-basic").select2({
        tags: true,
        dropdownParent: $("#feedback_form_area")
    });

    $.each(calendarEventData, function (key, value) {

        addCalanderEvent(`${key}_calendarEventId`, value.StartDate, value.EndDate, value.Title, value.Description, value.Project, value.Task, value.TimeSheetHours)

    });

    $("#add-time-sheet").click(function () {
        $(`#taskMonthHours`).prop("readonly", false);
        onSaveTimeSheet();
    });
    $("#edit-time-sheet").click(function () {

        onUpdateTimeSheet();
    });

}

function onMonthViewLoad() {
    // Get the modal
    var modal = document.getElementById("addEventsModal");

    // Get the Edit Event button
    var editEvent = document.getElementById("edit-time-sheet");
    // Get the Discard Modal button
    var discardModal = document.querySelectorAll("[data-dismiss='modal']")[0];

    // Get the Edit Event button
    var editEventTitle = document.getElementsByClassName("edit-event-title")[0];

    // Get the <span> element that closes the modal
    var span = document.getElementsByClassName("close")[0];

    // Get the all <input> elements insdie the modal
    var input = document.querySelectorAll('input[type="text"]');
    var radioInput = document.querySelectorAll('input[type="radio"]');

    // Get the all <textarea> elements insdie the modal
    var textarea = document.getElementsByTagName('textarea');

    // Create BackDrop ( Overlay ) Element
    function createBackdropElement() {
        var btn = document.createElement("div");
        btn.setAttribute('class', 'modal-backdrop fade show')
        document.body.appendChild(btn);
    }

    // Reset radio buttons

    function clearRadioGroup(GroupName) {
        var ele = document.getElementsByName(GroupName);
        for (var i = 0; i < ele.length; i++)
            ele[i].checked = false;
    }

    // Reset Modal Data on when modal gets closed
    function modalResetData() {
        modal.style.display = "none";
        for (i = 0; i < input.length; i++) {
            input[i].value = '';
        }
        for (j = 0; j < textarea.length; j++) {
            textarea[j].value = '';
            i
        }
        clearRadioGroup("marker");
        // Get Modal Backdrop
        var getModalBackdrop = document.getElementsByClassName('modal-backdrop')[0];
        $('.modal-backdrop').remove();
    }



    // Clear Data and close the modal when the user clicks on Discard button
    discardModal.onclick = function () {
        modalResetData();
        $('.modal-backdrop').remove();

    }

    // Clear Data and close the modal when the user clicks on <span> (x).
    span.onclick = function () {
        modalResetData();
        $('.modal-backdrop').remove();

    }

    // Clear Data and close the modal when the user clicks anywhere outside of the modal.
    window.onclick = function (event) {
        if (event.target == modal) {
            modalResetData();
            $('.modal-backdrop').remove();
        }
    }

    newDate = new Date()
    monthArray = ['1', '2', '3', '4', '5', '6', '7', '8', '9', '10', '11', '12']

    function getDynamicMonth(monthOrder) {
        var getNumericMonth = parseInt(monthArray[newDate.getMonth()]);
        var getNumericMonthInc = parseInt(monthArray[newDate.getMonth()]) + 1;
        var getNumericMonthDec = parseInt(monthArray[newDate.getMonth()]) - 1;

        if (monthOrder === 'default') {

            if (getNumericMonth < 10) {
                return '0' + getNumericMonth;
            } else if (getNumericMonth >= 10) {
                return getNumericMonth;
            }

        } else if (monthOrder === 'inc') {

            if (getNumericMonthInc < 10) {
                return '0' + getNumericMonthInc;
            } else if (getNumericMonthInc >= 10) {
                return getNumericMonthInc;
            }

        } else if (monthOrder === 'dec') {

            if (getNumericMonthDec < 10) {
                return '0' + getNumericMonthDec;
            } else if (getNumericMonthDec >= 10) {
                return getNumericMonthDec;
            }
        }
    }

    /* initialize the calendar
    -----------------------------------------------------------------*/

    calendar = $('#calendar').fullCalendar({
        header: {
            left: 'prev,next today',
            center: 'title',
            //right: 'month,agendaWeek,agendaDay'
            right: 'resourceTimelineDay,resourceTimelineTenDay,resourceTimelineMonth,resourceTimelineYear'
        },
        displayEventTime: false,
        editable: true,
        eventLimit: true,
        eventRender: function (event, element) {
            element.append("<span class='removebtn'>X</span>");
            element.find(".removebtn").click(function () {
                onDeleteTimeSheet(event.start.format("YYYY-MM-DD"), event.task);
                $('#calendar').fullCalendar('removeEvents', event._id);
            });
        },
        eventMouseover: function (event, jsEvent, view) {
            $(this).attr('id', event.id);

            $('#' + event.id).popover({
                template: '<div class="popover popover-primary" role="tooltip"><div class="arrow"></div><h3 class="popover-header"></h3><div class="popover-body"></div></div>',
                title: event.title,
                content: event.description,
                placement: 'top',
                html: true
            });

            $('#' + event.id).popover('show');
        },
        eventMouseout: function (event, jsEvent, view) {
            $('#' + event.id).popover('hide');
        },
        eventClick: function (info) {
            calendarEditEvent = info;

            editEvent.style.display = 'block';


            editEventTitle.style.display = 'block';
            modal.style.display = "block";
            document.getElementsByTagName('body')[0].style.overflow = 'hidden';
            createBackdropElement();

            // Calendar Event Featch

            var project = info.project;
            var task = info.task;
            // Task Modal Input
            $("#edit-project").val(project);
            $("#task-taskandbugs").val(task);
            $("#edit-taskhours").prop("readonly", false);
            $("#edit-taskhours").val(info.timeSheetHours);
            $("#edit-start-date").val(info.start.format("YYYY-MM-DD"));
            $("#task-timesheetId").val(info.timeSheetId);

        }
    })


    // Setting dynamic style ( padding ) of the highlited ( current ) date

    function setCurrentDateHighlightStyle() {
        getCurrentDate = $('.fc-content-skeleton .fc-today').attr('data-date');
        if (getCurrentDate === undefined) {
            return;
        }
        splitDate = getCurrentDate.split('-');
        if (splitDate[2] < 10) {
            $('.fc-content-skeleton .fc-today .fc-day-number').css('padding', '3px 8px');
        } else if (splitDate[2] >= 10) {
            $('.fc-content-skeleton .fc-today .fc-day-number').css('padding', '3px 4px');
        }
    }
    setCurrentDateHighlightStyle();

    const mailScroll = new PerfectScrollbar('.fc-scroller', {
        suppressScrollX: true
    });

    var fcButtons = document.getElementsByClassName('fc-button');
    for (var i = 0; i < fcButtons.length; i++) {
        fcButtons[i].addEventListener('click', function () {
            const mailScroll = new PerfectScrollbar('.fc-scroller', {
                suppressScrollX: true
            });
            $('.fc-scroller').animate({ scrollTop: 0 }, 100);
            setCurrentDateHighlightStyle();
        })
    }
}

function onWeekViewLoad() {
    $("#next").click(function () {
        if (weekIndex === 0) {
            return false;
        }
        weekIndex = weekIndex - 7;


        var Dates = new Date().getWeek();
        var newFromDate = Dates[0];
        var data = new Date(newFromDate.setDate(newFromDate.getDate() - weekIndex));
        var weekDate = data.getWeek();
        let startDate = weekDate[0].getDate() + "-" + (weekDate[0].getMonth() + 1) + "-" + weekDate[0].getFullYear();
        let endDate = weekDate[1].getDate() + "-" + (weekDate[1].getMonth() + 1) + "-" + weekDate[1].getFullYear();
        $("#weekDiv").empty();
        const url = `TimeSheetWeekEntry/Nextprevious/${startDate}/${endDate}`;
        getCommon(url, 'weekDiv', onNextSuccess);
    });

    $("#prev").click(function () {

        weekIndex = weekIndex + 7;


        var Dates = new Date().getWeek();
        var newFromDate = Dates[0];
        var data = new Date(newFromDate.setDate(newFromDate.getDate() - weekIndex));
        var weekDate = data.getWeek();
        let startDate = weekDate[0].getDate() + "-" + (weekDate[0].getMonth() + 1) + "-" + weekDate[0].getFullYear();
        let endDate = weekDate[1].getDate() + "-" + (weekDate[1].getMonth() + 1) + "-" + weekDate[1].getFullYear();
        $("#weekDiv").empty();
        const url = `TimeSheetWeekEntry/Nextprevious/${startDate}/${endDate}`;
        getCommon(url, 'weekDiv', onPrevSuccess);
    });


    changeLabelName(0);
}

function onNextSuccess() {

    changeLabelName(weekIndex);
}

function onPrevSuccess() {
    changeLabelName(weekIndex);
}
function BackDrop() {
    var backDiv = document.createElement("div");
    backDiv.setAttribute('class', 'modal-backdrop fade show')
    document.body.appendChild(backDiv);
}

function timeSheetData(data) {
    if (data != null || data === undefined) {
        calendarEventData = data;
    }
}

function addCalanderEvent(id, start, end, title, description, project, task, timeSheetHours) {
    var eventObject = {};
        
    eventObject.title= title;
    eventObject.start= start;
    eventObject.end = end;
    eventObject.id = id;
    if (timeSheetHours > 12) {
        eventObject.className = "bg-danger";
    }
    else {
        eventObject.className = "bg-primary";
    }
    eventObject.timeSheetHours = timeSheetHours;
    eventObject.description = description;
    eventObject.project = project;
    eventObject.task = task;
    
    $('#calendar').fullCalendar('renderEvent', eventObject, true);
    return eventObject;
}

function changeLabelName(index) {
    var weekday = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];
    var Dates = new Date().getWeek();
    var newFromDate = Dates[0];
    var data = new Date(newFromDate.setDate(newFromDate.getDate() - index));
    var weekDate = data.getWeek();

    $('#weekDateTime').text(weekDate[0].toLocaleDateString() + '-' + weekDate[1].toLocaleDateString());

    for (var labelIndex = 0; labelIndex < 7; labelIndex++) {
        var tempdate = new Date(newFromDate.setDate(newFromDate.getDate()));
        if (labelIndex > 0) {
            tempdate = new Date(newFromDate.setDate(newFromDate.getDate() + 1));
        }

        if (labelIndex == 0) {
            $("#sundayLabel").text('[' + weekday[tempdate.getDay()] + ']');
            $("#sundayDateLabel").text(("0" + tempdate.getDate()).slice(-2) + '-' + tempdate.getFullYear());

        }
        else if (labelIndex == 1) {
            $("#mondayLabel").text('[' + weekday[tempdate.getDay()] + ']');
            $("#mondayDateLabel").text(("0" + tempdate.getDate()).slice(-2) + '-' + tempdate.getFullYear());

        }
        else if (labelIndex == 2) {
            $("#tuesdayLabel").text('[' + weekday[tempdate.getDay()] + ']');
            $("#tuesdayDateLabel").text(("0" + tempdate.getDate()).slice(-2) + '-' + tempdate.getFullYear());

        }
        else if (labelIndex == 3) {
            $("#wednesdayLabel").text('[' + weekday[tempdate.getDay()] + ']');
            $("#wednesdayDateLabel").text(("0" + tempdate.getDate()).slice(-2) + '-' + tempdate.getFullYear());

        }
        else if (labelIndex == 4) {
            $("#thursdayLabel").text('[' + weekday[tempdate.getDay()] + ']');
            $("#thursdayDateLabel").text(("0" + tempdate.getDate()).slice(-2) + '-' + tempdate.getFullYear());

        }
        else if (labelIndex == 5) {
            $("#fridayLabel").text('[' + weekday[tempdate.getDay()] + ']');
            $("#fridayDateLabel").text(("0" + tempdate.getDate()).slice(-2) + '-' + tempdate.getFullYear());

        }
        else if (labelIndex == 6) {
            $("#saturdayLabel").text('[' + weekday[tempdate.getDay()] + ']');
            $("#saturdayDateLabel").text(("0" + tempdate.getDate()).slice(-2) + '-' + tempdate.getFullYear());
        }
    }

     ResetControlValue();

}

function ResetControlValue() {
    var allElement = $(".week-task");
    $.each(allElement, function (key, value) {
        if (value != null && value != '') {

            if (value.type.toLowerCase() === "select-one") {

                $(`#${value.id}`).val('');    
                if (value.id.toLocaleLowerCase().includes('project')) {
                    $(`#select2-${value.id}-container`).text('-- Select Project--');
                }
                if (value.id.toLocaleLowerCase().includes('task')) {                   
                    $(`#${value.id}`).find('option').remove().end();
                 
                }

            }           
            if (value.type.toLowerCase() === "number") {
                $(`#${value.id}`).val('');
                $(`#${value.id}`).prop("readonly", true);
               
            }
            if (value.type.toLowerCase() === "text") {
                $(`#${value.id}`).val('');
                $(`#${value.id}`).prop("readonly", true);
            }
        }
    });

    
    addTimeSheetModificationEffect();
   
}

function setReadOnlyOnWeekList() {

    var CurrentWeekInterVal = $('#weekDateTime').html()
    var currentDate = new Date();
    var weekStarEndDate = new Date(CurrentWeekInterVal.split('-')[0]).getWeek();

    if (currentDate >= weekStarEndDate[0] && currentDate <= weekStarEndDate[1]) {

        for (index = 1; index <= weekControlIndex; index++) {
            var extractDate = new Date(CurrentWeekInterVal.split('-')[0])
            if (compare_dates(extractDate, currentDate)) {
                $(`#sundayUpdateTxt_${index}`).attr("readonly", true);

            }

            if (compare_dates(extractDate.setDate(extractDate.getDate() + 1), currentDate)) {
                $(`#mondayUpdateTxt_${index}`).attr("readonly", true);
            }

            if (compare_dates(extractDate.setDate(extractDate.getDate() + 1), currentDate)) {
                $(`#tuesdayUpdateTxt_${index}`).attr("readonly", true);
            }

            if (compare_dates(extractDate.setDate(extractDate.getDate() + 1), currentDate)) {
                $(`#wednesdayUpdateTxt_${index}`).attr("readonly", true);
            }

            if (compare_dates(extractDate.setDate(extractDate.getDate() + 1), currentDate)) {
                $(`#thursdayUpdateTxt_${index}`).attr("readonly", true);
            }
            else {
                $(`#thursdayUpdateTxt_${index}`).attr("readonly", false);
            }
            if (compare_dates(extractDate.setDate(extractDate.getDate() + 1), currentDate)) {
                $(`#fridayUpdateTxt_${index}`).attr("readonly", true);
            }

            if (compare_dates(extractDate.setDate(extractDate.getDate() + 1), currentDate)) {
                $(`#saturdayUpdateTxt_${index}`).attr("readonly", true);
            }
        }
    }
    else {
        for (index = 1; index <= weekControlIndex; index++) {
            $(`#sundayUpdateTxt_${index}`).attr("readonly", false);
            $(`#mondayUpdateTxt_${index}`).attr("readonly", false);
            $(`#tuesdayUpdateTxt${index}`).attr("readonly", false);
            $(`#wednesdayUpdateTxt_${index}`).attr("readonly", false);
            $(`#thursdayUpdateTxt_${index}`).attr("readonly", false);
            $(`#fridayUpdateTxt_${index}`).attr("readonly", false);
            $(`#saturdayUpdateTxt_${index}`).attr("readonly", false);
        }
    }
}
function Total(controlIndex) {
    var sunday = 0;
    var monday = 0;
    var tuesday = 0;
    var wednesday = 0;
    var thursday = 0;
    var friday = 0;
    var saturday = 0;
    var TotalAmount = 0;

    sunday = $(`#sundayTxt_${controlIndex}`).val() == '' ? 0 : $(`#sundayTxt_${controlIndex}`).val();
    monday = $(`#mondayTxt_${controlIndex}`).val() == '' ? 0 : $(`#mondayTxt_${controlIndex}`).val();
    tuesday = $(`#tuesdayTxt_${controlIndex}`).val() == '' ? 0 : $(`#tuesdayTxt_${controlIndex}`).val();
    wednesday = $(`#wednesdayTxt_${controlIndex}`).val() == '' ? 0 : $(`#wednesdayTxt_${controlIndex}`).val();
    thursday = $(`#thursdayTxt_${controlIndex}`).val() == '' ? 0 : $(`#thursdayTxt_${controlIndex}`).val();
    friday = $(`#fridayTxt_${controlIndex}`).val() == '' ? 0 : $(`#fridayTxt_${controlIndex}`).val();
    saturday = $(`#saturdayTxt_${controlIndex}`).val() == '' ? 0 : $(`#saturdayTxt_${controlIndex}`).val();

    TotalAmount =
        parseInt(sunday) +
        parseInt(monday) +
        parseInt(tuesday) +
        parseInt(wednesday) +
        parseInt(thursday) +
        parseInt(friday) +
        parseInt(saturday);

    $(`#totalTxt_${controlIndex}`).val(TotalAmount);

}

function EditTotal(controlIndex) {
    var sunday = 0;
    var monday = 0;
    var tuesday = 0;
    var wednesday = 0;
    var thursday = 0;
    var friday = 0;
    var saturday = 0;
    var TotalAmount = 0;

    sunday = $(`#sundayUpdateTxt_${controlIndex}`).val() == '' ? 0 : $(`#sundayUpdateTxt_${controlIndex}`).val();
    monday = $(`#mondayUpdateTxt_${controlIndex}`).val() == '' ? 0 : $(`#mondayUpdateTxt_${controlIndex}`).val();
    tuesday = $(`#tuesdayUpdateTxt_${controlIndex}`).val() == '' ? 0 : $(`#tuesdayUpdateTxt_${controlIndex}`).val();
    wednesday = $(`#wednesdayUpdateTxt_${controlIndex}`).val() == '' ? 0 : $(`#wednesdayUpdateTxt_${controlIndex}`).val();
    thursday = $(`#thursdayUpdateTxt_${controlIndex}`).val() == '' ? 0 : $(`#thursdayUpdateTxt_${controlIndex}`).val();
    friday = $(`#fridayUpdateTxt_${controlIndex}`).val() == '' ? 0 : $(`#fridayUpdateTxt_${controlIndex}`).val();
    saturday = $(`#saturdayUpdateTxt_${controlIndex}`).val() == '' ? 0 : $(`#saturdayUpdateTxt_${controlIndex}`).val();

    TotalAmount =
        parseInt(sunday) +
        parseInt(monday) +
        parseInt(tuesday) +
        parseInt(wednesday) +
        parseInt(thursday) +
        parseInt(friday) +
        parseInt(saturday);

    $(`#totalUpdateTxt_${controlIndex}`).val(TotalAmount);

}

function addWeekViewControl() {
    weekControlIndex = weekControlIndex + 1;
    const url = `TimeSheetWeekEntry/AddWeekView/${weekControlIndex}`; 
    getCommon(url, `weekDiv`, null, null, null, true);

}

function loadTask(projectIndex) {
    selectedProjectIndex = projectIndex;

    var projectName = $(`#ProjectId_${projectIndex}`).val();
    $(`#sundayTxt_${selectedProjectIndex}`).val("");
    $(`#mondayTxt_${selectedProjectIndex}`).val("");
    $(`#tuesdayTxt_${selectedProjectIndex}`).val("");
    $(`#wednesdayTxt_${selectedProjectIndex}`).val("");
    $(`#thursdayTxt_${selectedProjectIndex}`).val("");
    $(`#fridayTxt_${selectedProjectIndex}`).val("");
    $(`#saturdayTxt_${selectedProjectIndex}`).val("");

    if (projectName === '') {

        $(`#sundayTxt_${selectedProjectIndex}`).prop("readonly", true);
        $(`#mondayTxt_${selectedProjectIndex}`).prop("readonly", true);
        $(`#tuesdayTxt_${selectedProjectIndex}`).prop("readonly", true);
        $(`#wednesdayTxt_${selectedProjectIndex}`).prop("readonly", true);
        $(`#thursdayTxt_${selectedProjectIndex}`).prop("readonly", true);
        $(`#fridayTxt_${selectedProjectIndex}`).prop("readonly", true);
        $(`#saturdayTxt_${selectedProjectIndex}`).prop("readonly", true);

        var options = [];
        $(`#task_${selectedProjectIndex}`).find('option').remove().end()
        options.push("<option disabled selected>Select Task/Bugs</option>");
        $(`#task_${selectedProjectIndex}`).append(options.join("")).select2().selectmenu('refresh');
        $(`#task_${selectedProjectIndex}`).prop('selectedIndex', 0);

        return false;
    }
    if (projectName !== undefined) {
        const url = `TimeSheetWeekEntry/GetTaskList/${projectName}`;
        getCommon(url, null, onLoadTaskSucessfull, null, null, false, true);
    }

}

function onLoadTaskSucessfull(data) {
    $(`#task_${selectedProjectIndex}`).find('option').remove().end()

    var options = [];
    options.push("<option disabled selected>Select Task/Bugs</option>");
    $.each(data, function (key, value) {
        if (value.task != null && value.task != '')
            options.push("<option value='" + value.task + "'project='" + value.project + "'parentLink='" + value.parentLink + "'>" + value.task + "</option>");
    });

    $(`#task_${selectedProjectIndex}`).append(options.join("")).select2().selectmenu('refresh');
   
    $(`#task_${selectedProjectIndex}`).prop('selectedIndex', 0);
}

function onTaskChange(projectIndex) {
    selectedTaskIndex = projectIndex;
    for (index = 1; index <= weekControlIndex; index++) {
      
        var taskBugs = $(`#task_${index}`).val();
        var SelecttaskBugs = $(`#task_${projectIndex}`).val();
        if (taskBugs === SelecttaskBugs && index !== projectIndex) {
            showError(TASK_SELECT_ERROR, ERROR_MESSAGE_TITLE, onTaskChangeErrorCallBack, projectIndex);
            break;
        }

        var taskBugsTxt = $(`#TaskIdTxt_${index}`).val();
        if (taskBugsTxt === SelecttaskBugs) {
            showError(TASK_SELECT_ERROR, ERROR_MESSAGE_TITLE, onTaskChangeErrorCallBack, projectIndex);
            break;
        }
    }
    var CurrentWeekInterVal = $('#weekDateTime').html()
    var currentDate = new Date();
    var extractDate = new Date(CurrentWeekInterVal.split('-')[0])

    if (compare_dates(currentDate, extractDate)) {
        $(`#sundayTxt_${projectIndex}`).attr("readonly", false); 
     
    }
    if (compare_dates(currentDate, extractDate.setDate(extractDate.getDate() + 1))) {
        $(`#mondayTxt_${projectIndex}`).attr("readonly", false);
    }
    if (compare_dates(currentDate, extractDate.setDate(extractDate.getDate() + 1))) {
        $(`#tuesdayTxt_${projectIndex}`).attr("readonly", false);
    }
    if (compare_dates(currentDate, extractDate.setDate(extractDate.getDate() + 1))) {
        $(`#wednesdayTxt_${projectIndex}`).attr("readonly", false);
    }
    if (compare_dates(currentDate, extractDate.setDate(extractDate.getDate() + 1))) {
        $(`#thursdayTxt_${projectIndex}`).attr("readonly", false);
    }
    if (compare_dates(currentDate, extractDate.setDate(extractDate.getDate() + 1))) {
        $(`#fridayTxt_${projectIndex}`).attr("readonly", false);
    }
    if (compare_dates(currentDate, extractDate.setDate(extractDate.getDate() + 1))) {
        $(`#saturdayTxt_${projectIndex}`).attr("readonly", false);
    }
   
}

function onTaskChangeErrorCallBack() {
    $(`#task_${selectedTaskIndex}`).prop('selectedIndex', 0)
    $(`#sundayTxt_${selectedTaskIndex}`).val("");
    $(`#mondayTxt_${selectedTaskIndex}`).val("");
    $(`#tuesdayTxt_${selectedTaskIndex}`).val("");
    $(`#wednesdayTxt_${selectedTaskIndex}`).val("");
    $(`#thursdayTxt_${selectedTaskIndex}`).val("");
    $(`#fridayTxt_${selectedTaskIndex}`).val("");
    $(`#saturdayTxt_${selectedTaskIndex}`).val("");
    $(`#sundayTxt_${selectedTaskIndex}`).attr("readonly", true);
    $(`#mondayTxt_${selectedTaskIndex}`).attr("readonly", true);
    $(`#tuesdayTxt_${selectedTaskIndex}`).attr("readonly", true);
    $(`#wednesdayTxt_${selectedTaskIndex}`).attr("readonly", true);
    $(`#thursdayTxt_${selectedTaskIndex}`).attr("readonly", true);
    $(`#fridayTxt_${selectedTaskIndex}`).attr("readonly", true);
    $(`#saturdayTxt_${selectedTaskIndex}`).attr("readonly", true); 
}

function onSaveWeekTimeSheetList() {
  
    var weekTimeSheetList = [];
    for (index = 1; index <= weekControlIndex; index++) {

        var CurrentWeekInterVal = $('#weekDateTime').html()

        GetExistingTimeSheet(weekTimeSheetList, index, CurrentWeekInterVal);

        var projectName = $(`#ProjectId_${index}`).val();
        if (isNullOrWhitespace(projectName)) { continue; }

        var taskBugs = $(`#task_${index}`).val();
        if (isNullOrWhitespace(taskBugs)) { continue; }

      
        GetNewAddedTimeSheet(weekTimeSheetList, index, CurrentWeekInterVal);
      
    }

    if (weekTimeSheetList.length > 0) {
        var url = "TimeSheetWeekEntry/SaveWeeKTimeWorkItems";
        postCommon(url, weekTimeSheetList, null, OnSaveWeekTimeSheetSuccess, OnSaveWeekTimeSheetFailure)
    }
}

function GetNewAddedTimeSheet(weekTimeSheetList, index, CurrentWeekInterVal) {

    var extractDate = new Date();
    var projectName = $(`#ProjectId_${index}`).val();
    var taskBugs = $(`#task_${index}`).val();
    
    var total = TryParseInt($(`#totalTxt_${index}`).val(), null)
    if (total == null) {
        showError(WEEK_TIMESHEET_ENTRY_ERROR, ERROR_MESSAGE_TITLE);
        return false;
    }

    var sundayHours = TryParseInt($(`#sundayTxt_${index}`).val(), 0)
    if (sundayHours !== 0) {
        extractDate = new Date(CurrentWeekInterVal.split('-')[0]);
        weekTimeSheetList.push(GetTimeSheet($(`#task_${index}`), projectName, sundayHours, formatDate(extractDate.setDate(extractDate.getDate() + 0))));
    }


    var mondayHours = TryParseInt($(`#mondayTxt_${index}`).val(), 0)
    if (mondayHours !== 0) {
        extractDate = new Date(CurrentWeekInterVal.split('-')[0]);
        weekTimeSheetList.push(GetTimeSheet($(`#task_${index}`), projectName, mondayHours, formatDate(extractDate.setDate(extractDate.getDate() + 1))));
    }
    var tuesdayHours = TryParseInt($(`#tuesdayTxt_${index}`).val(), 0)
    if (tuesdayHours !== 0) {
        extractDate = new Date(CurrentWeekInterVal.split('-')[0]);
        weekTimeSheetList.push(GetTimeSheet($(`#task_${index}`), projectName, tuesdayHours, formatDate(extractDate.setDate(extractDate.getDate() + 2))));
    }
    var wednesdayHours = TryParseInt($(`#wednesdayTxt_${index}`).val(), 0)
    if (wednesdayHours !== 0) {
        extractDate = new Date(CurrentWeekInterVal.split('-')[0]);
        weekTimeSheetList.push(GetTimeSheet($(`#task_${index}`), projectName, wednesdayHours, formatDate(extractDate.setDate(extractDate.getDate() + 3))));
    }
    var thursdayHours = TryParseInt($(`#thursdayTxt_${index}`).val(), 0)
    if (thursdayHours !== 0) {
        extractDate = new Date(CurrentWeekInterVal.split('-')[0]);
        weekTimeSheetList.push(GetTimeSheet($(`#task_${index}`), projectName, thursdayHours, formatDate(extractDate.setDate(extractDate.getDate() + 4))));
    }
    var fridayHours = TryParseInt($(`#fridayTxt_${index}`).val(), 0)
    if (fridayHours !== 0) {
        extractDate = new Date(CurrentWeekInterVal.split('-')[0]);
        weekTimeSheetList.push(GetTimeSheet($(`#task_${index}`), projectName, fridayHours, formatDate(extractDate.setDate(extractDate.getDate() + 5))));
    }
    var saturdayHours = TryParseInt($(`#saturdayTxt_${index}`).val(), 0)
    if (saturdayHours !== 0) {
        extractDate = new Date(CurrentWeekInterVal.split('-')[0]);
        weekTimeSheetList.push(GetTimeSheet($(`#task_${index}`), projectName, saturdayHours, formatDate(extractDate.setDate(extractDate.getDate() + 6))));
    }
}
function GetExistingTimeSheet(weekTimeSheetList, index, CurrentWeekInterVal) {
    var extractDate = new Date();
    if ($(`#sundayUpdateTxt_${index}`).hasClass('modified')) {
        extractDate = new Date(CurrentWeekInterVal.split('-')[0]);
        weekTimeSheetList.push(GetUpdateTimeSheet($(`#sundayUpdateTxt_${index}`), formatDate(extractDate.setDate(extractDate.getDate() + 0))));
    }
    if ($(`#mondayUpdateTxt_${index}`).hasClass('modified')) {
        extractDate = new Date(CurrentWeekInterVal.split('-')[0]);
        weekTimeSheetList.push(GetUpdateTimeSheet($(`#mondayUpdateTxt_${index}`), formatDate(extractDate.setDate(extractDate.getDate() + 1))));
    }
    if ($(`#tuesdayUpdateTxt_${index}`).hasClass('modified')) {
        extractDate = new Date(CurrentWeekInterVal.split('-')[0]);
        weekTimeSheetList.push(GetUpdateTimeSheet($(`#tuesdayUpdateTxt_${index}`), formatDate(extractDate.setDate(extractDate.getDate() + 2))));
    }
    if ($(`#wednesdayUpdateTxt_${index}`).hasClass('modified')) {
        extractDate = new Date(CurrentWeekInterVal.split('-')[0]);
        weekTimeSheetList.push(GetUpdateTimeSheet($(`#wednesdayUpdateTxt_${index}`), formatDate(extractDate.setDate(extractDate.getDate() + 3))));
    }
    if ($(`#thursdayUpdateTxt_${index}`).hasClass('modified')) {
        extractDate = new Date(CurrentWeekInterVal.split('-')[0]);
        weekTimeSheetList.push(GetUpdateTimeSheet($(`#thursdayUpdateTxt_${index}`), formatDate(extractDate.setDate(extractDate.getDate() + 4))));
    }
    if ($(`#fridayUpdateTxt_${index}`).hasClass('modified')) {
        extractDate = new Date(CurrentWeekInterVal.split('-')[0]);
        weekTimeSheetList.push(GetUpdateTimeSheet($(`#fridayUpdateTxt_${index}`), formatDate(extractDate.setDate(extractDate.getDate() + 5))));
    }
    if ($(`#saturdayUpdateTxt_${index}`).hasClass('modified')) {
        extractDate = new Date(CurrentWeekInterVal.split('-')[0]);
        weekTimeSheetList.push(GetUpdateTimeSheet($(`#saturdayUpdateTxt_${index}`), formatDate(extractDate.setDate(extractDate.getDate() + 6))));
    }
}



function GetUpdateTimeSheet(control, date) {
    var timeSheet = {};
    timeSheet.Project = control.attr('project');
    timeSheet.ParentLink = null;
    timeSheet.Task = control.attr('task');
    timeSheet.Epic = control.attr('epic');
    timeSheet.Feature = control.attr('feature');
    timeSheet.UserStory = control.attr('userStory');
    timeSheet.Requirements = control.attr('requirement');
    timeSheet.TimeSheetDate = new Date(date);
    timeSheet.TimeSheetHours = isNaN(parseInt(control.val())) ? 0 : parseInt(control.val());
    return timeSheet;
}

function GetTimeSheet(taskControl,Project,hours,date) {
    var timeSheet = {};
    timeSheet.Project = Project;
    timeSheet.ParentLink = isEmpty(taskControl.children("option:selected").attr('parentLink')) ? '' : taskControl.children("option:selected").attr('parentLink');
   
    timeSheet.Task = taskControl.val();
    timeSheet.TimeSheetDate = new Date(date);;
    timeSheet.TimeSheetHours = parseInt(hours);

    return timeSheet;
}
function OnSaveWeekTimeSheetSuccess(data) {
    var CurrentWeekInterVal = $('#weekDateTime').html()
    $("#weekDiv").empty();
    var weekDate;
    if (isNullOrWhitespace(CurrentWeekInterVal)) {
        weekDate = new Date().getWeek();
    }
    else {
        weekDate = new Date(CurrentWeekInterVal.split('-')[0]).getWeek();
    }

    let startDate = weekDate[0].getDate() + "-" + (weekDate[0].getMonth() + 1) + "-" + weekDate[0].getFullYear();
    let endDate = weekDate[1].getDate() + "-" + (weekDate[1].getMonth() + 1) + "-" + weekDate[1].getFullYear();
    const url = `TimeSheetWeekEntry/WeekListByDate/${startDate}/${endDate}`;
    getCommon(url, 'weekDiv', onrenderWeekListTimeSheetSuccess);
}
function renderWeekListOnTabClick() {
    $("#weekDiv").empty();
    var weekDate = new Date().getWeek();
  
    let startDate = weekDate[0].getDate() + "-" + (weekDate[0].getMonth() + 1) + "-" + weekDate[0].getFullYear();
    let endDate = weekDate[1].getDate() + "-" + (weekDate[1].getMonth() + 1) + "-" + weekDate[1].getFullYear();
    const url = `TimeSheetWeekEntry/WeekListByDate/${startDate}/${endDate}`;
    getCommon(url, 'weekDiv', onRenderWeekTabSuccess);

}
function onRenderWeekTabSuccess() {
    addTimeSheetModificationEffect();
}
function renderCalenderOnTabClick() {
    const url = `TimeSheetMonthEntry/GetCalenderData`;
    getCommon(url, null, onCalenderDataLoadSuccess, onCalenderDataLoadFailure, null, null, true);
}
function onCalenderDataLoadSuccess(data) {
    $('#calendar').fullCalendar('removeEvents');
    $.each(data, function (key, value) {
       addCalanderEvent(`${key}_calendarEventId`, value.startDate, value.endDate, value.title, value.description, value.project, value.task, value.timeSheetHours)
    });
}
function onCalenderDataLoadFailure() {
    displayDangerMessage("Load Failed. Please Try again later")
}
function addTimeSheetModificationEffect() {
    var updateElement = $(".updateTask");
    $.each(updateElement, function (key, value) {
        if (value.type.toLowerCase() === "number") {
            $(`#${value.id}`).on('input propertychange paste', function () {
                $(`#${value.id}`).addClass('modified');
            });
        }
    });

    setReadOnlyOnWeekList();
}
function onrenderWeekListTimeSheetSuccess() {
    addTimeSheetModificationEffect();
    displaySuccessMessage(WEEK_TIMESHEET_SAVE_SUCCESS);
}
function OnSaveWeekTimeSheetFailure() {closeForm
    displayDangerMessage(WEEK_TIMESHEET_ENTRY_ERROR)
}
function onChangeMonthProject()
{

    var projectName = $(`#monthProjectId`).val();
   
    if (projectName === '') {

        var options = [];
        $(`#monthTask`).find('option').remove().end()
        options.push("<option disabled selected>Select Task/Bugs</option>");
        $(`#monthTask`).append(options.join("")).select2({
            tags: true,
            dropdownParent: $("#feedback_form_area")
        }).selectmenu('refresh');
        $(`#monthTask`).prop('selectedIndex', 0);

        return false;
    }
    if (projectName !== undefined) {
        const url = `TimeSheetWeekEntry/GetTaskList/${projectName}`;
        getCommon(url, null, onMonthTaskSucessfull, null, null, false, true);
    }

}
function onMonthTaskSucessfull(data) {
    $(`#monthTask`).find('option').remove().end()

    var options = [];
    options.push("<option disabled selected>Select Task/Bugs</option>");
    $.each(data, function (key, value) {
        if (value.task != null && value.task != '')
            options.push("<option value='" + value.task + "'project='" + value.project + "'parentLink='" + value.parentLink + "'timeSheetId='" + value.timeSheetId + "'>" + value.task + "</option>");
    });

    $(`#monthTask`).append(options.join("")).select2({
        tags: true,
        dropdownParent: $("#feedback_form_area")
    }).selectmenu('refresh');
    $(`#monthTask`).prop('selectedIndex', 0);
    $("#select2-monthProjectId-container").text('-- Select Project--');
}

function onDeleteTimeSheet(date,task) {

    if (isNullOrWhitespace(date) || isNullOrWhitespace(task)) {
        return;
    }
    var url = `TimeSheetMonthEntry/DeleteTimeSheet/${date}/${task}`;
    postCommon(url, null, null, OnDeleteTimeSheetSuccess, OnDeleteTimeSheetFailure)
}

function OnDeleteTimeSheetSuccess() {
    displaySuccessMessage(TIMESHEET_DELETE_SUCCESS);
}

function OnDeleteTimeSheetFailure() {
    displayDangerMessage(TIMESHEET_DELETE_ERROR)
}
function onUpdateTimeSheet() {
    var timesheet = {};
    if (!validateEditMonthTask()) {
        return false;
    }

    timesheet.Task = $("#task-taskandbugs").val();
    timesheet.TimeSheetHours = parseInt($("#edit-taskhours").val());
    timesheet.TimeSheetDate = new Date($("#edit-start-date").val());
    calendarEditEvent.timeSheetHours = parseInt($("#edit-taskhours").val()); 
    calendarEditEvent.title = `${calendarEditEvent.title.split(' ')[0]} ${calendarEditEvent.title.split(' ')[1]} ${calendarEditEvent.timeSheetHours} ${calendarEditEvent.title.split(' ')[3]}`; 
   
    var url = "TimeSheetMonthEntry/UpdateTimeWorkItems";
    postCommon(url, timesheet, null, OnUpdateimeSheetSuccess, OnUpdateTimeSheetFailure)
}

function OnUpdateimeSheetSuccess() {

    document.getElementById("addEventsModal").style.display = "none";
  
    // Get Modal Backdrop
    var getModalBackdrop = document.getElementsByClassName('modal-backdrop')[0];
    document.body.removeChild(getModalBackdrop)

    document.getElementsByTagName('body')[0].removeAttribute('style');
    $('#calendar').fullCalendar('updateEvent', calendarEditEvent);
   
    displaySuccessMessage(TIMESHEET_UPDATE_SUCCESS);
}

function OnUpdateTimeSheetFailure() {
    showError(TIMESHEET_UPDATE_ERROR, ERROR_MESSAGE_TITLE);
}


function onSaveTimeSheet() {
    
    if (!validateMonthTask()) {
        return false;
    }
    $("#feedback-form").css("z-index", "100");
    var timeSheet = GetTimeSheet($(`#monthTask`), $(`#monthProjectId`).val(), $('#taskMonthHours').val(), formatDate($('#task-start-date').val()));
   
    var url = "TimeSheetMonthEntry/SaveTimeWorkItems";
    postCommon(url,timeSheet, null, OnSaveTimeSheetSuccess, OnSaveTimeSheetFailure)
}

function OnSaveTimeSheetSuccess(data) {
    data = data.sourceObject;
    $("#feedback-form").css("z-index", "13200");
    $("#feedback_form_area").animate({
        width: "toggle"
    });
    $('.modal-backdrop').remove();
    $("#select2-monthProjectId-container").text('-- Select Project--');
    $(`#monthTask`).val('');
    $(`#monthTask`).find('option').remove().end();
    $('#taskMonthHours').val('');
    var id = $('#calendar').fullCalendar('clientEvents').length;
    var calendarid = parseInt(id) * 22;
    addCalanderEvent(`${calendarid}_CalaenderId`, data.startDate, data.endDate, data.title, data.Description, data.project, data.task, data.timeSheetHours);
    displaySuccessMessage(TIMESHEET_SAVE_SUCCESS);
}

function OnSaveTimeSheetFailure() {
    $("#feedback-form").css("z-index", "13200");
    $("#feedback_form_area").animate({
        width: "toggle"
    });
    $('.modal-backdrop').remove();
    showError(TIMESHEET_ENTRY_ERROR, ERROR_MESSAGE_TITLE); 
}

function validateEditMonthTask() {

    var time = $('#edit-taskhours').val();
    if (isNullOrWhitespace(time) || isNaN(parseInt(time))) {
        showError(VALIDATION_ERROR, ERROR_MESSAGE_TITLE);
        return false;
    }


    return true;
}

function validateMonthTask() {

    var time = $('#taskMonthHours').val();
    if (isNullOrWhitespace(time) || isNaN(parseInt(time))) {
        showError(VALIDATION_ERROR, ERROR_MESSAGE_TITLE);
        return false;
    }

    if (isNullOrWhitespace(($('#task-start-date').val()))) {
        showError(VALIDATION_ERROR, ERROR_MESSAGE_TITLE);
        return false;
    }

    var taskDate = new Date(($('#task-start-date').val()));
    if (taskDate > new Date()) {
        showError(VALIDATION_DATE_ERROR, ERROR_MESSAGE_TITLE);
        return false;
    }
  

    return true;
}