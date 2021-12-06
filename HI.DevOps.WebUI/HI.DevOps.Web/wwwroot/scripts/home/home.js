function onHomePageLoad() {
    $("#timeSheetMenuLink").click(function () {
        renderingNavPageMenu(NavPageEnum.TimeSheetEnter);
    });
    $("#timeSheetExportLink").click(function () {
        renderingNavPageMenu(NavPageEnum.Export);
    });

    renderingNavPageMenu(NavPageEnum.TimeSheetEnter);
}


function renderingNavPageMenu(PageName) {
    switch (PageName) {
        case NavPageEnum.TimeSheetEnter:
            renderTimeSheetEnterPage();
            break;
        case NavPageEnum.Export:
            renderTimeSheetExportPage();
            break;
        default:
            renderTimeSheetEnterPage();
            break;
    }
}


function renderTimeSheetEnterPage() {
    $(".active").removeClass("active");
    $("#timeSheetMenuLink").addClass("active");
    const url = "TimeSheetMonthEntry/MonthView";
    getCommon(url, "PartialArea");
}


function renderTimeSheetExportPage() {
    $(".active").removeClass("active");
    $("#timeSheetExportLink").addClass("active");
    const url = "Export/Export";
    getCommon(url, "PartialArea");
}