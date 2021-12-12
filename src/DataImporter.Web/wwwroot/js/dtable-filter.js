let fStartDate = moment().subtract(1, 'months').format();
let fStartDateBackup = fStartDate;
let fEndDate = moment().format();
let fEndDateBackup = fEndDate;

$('#switch-end-date').on('click', function () {
    $('#filter-end-date').attr('disabled', !this.checked);
    if (this.checked) {
        fEndDate = fEndDateBackup;
    } else {
        fEndDateBackup = fEndDate;
        fEndDate = "";
    }
    if (dTable != null) {
        dTable.draw();
    }
});

$('#switch-start-date').on('click', function () {
    $('#filter-start-date').attr('disabled', !this.checked);
    if (this.checked) {
        fStartDate = fStartDateBackup;
    } else {
        fStartDateBackup = fStartDate;
        fStartDate = "";
    }
    if (dTable != null) {
        dTable.draw();
    }
});

const getDatePickerOptions = (dateVariable) => {
    return {
        singleDatePicker: true,
        showDropdowns: true,
        startDate: moment(dateVariable),
        autoApply: true,
        locale: {
            format: 'DD/M/YYYY'
        }
    }
}

$('#search-box').on('input',
    function () {
        if (dTable != null) {
            dTable.search($("#search-box").val()).draw();
        }
    });

function pickerCallBack(date, variable) {
    date = moment(date).format();
    if (variable == 0)
        fStartDate = date;
    else
        fEndDate = date;
    if (dTable != null) {
        dTable.draw();
    }
}

$('#filter-start-date').daterangepicker(
    getDatePickerOptions(fStartDate),
    function (date, _, _1) {
        pickerCallBack(date, 0);
    });

$('#filter-end-date').daterangepicker(
    getDatePickerOptions(fEndDate),
    function (date, _, _1) {
        pickerCallBack(date, 1);
    });

const dTableCommonConfig = {
    responsive: true,
    lengthChange: true,
    autoWidth: false,
    processing: true,
    serverSide: true,
}