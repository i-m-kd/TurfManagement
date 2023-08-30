$(document).ready(function () {
    $('#datepicker').datepicker({
        dateFormat: 'yy-mm-dd', // Format as desired
        changeMonth: true,
        changeYear: true,
    });

    $('#startTimepicker').timepicker({
        timeFormat: 'h:mm p',
        interval: 30, // 30-minute interval
        dropdown: true,
    });

    $('#endTimepicker').timepicker({
        timeFormat: 'h:mm p',
        interval: 30, // 30-minute interval
        dropdown: true,
    });
});