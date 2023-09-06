
$(document).ready(function () {
     $.ajax({
         url: '@Url.Action("BookTurf", "Turf")',
         type: 'GET',
         success: function (data) {
             var locationDropdown = $("#locationDropdown");
             locationDropdown.empty(); 
             locationDropdown.append($('<option>').val('').text('Select Location')); 
             $.each(data, function (index, location) {
                 locationDropdown.append($('<option>').val(location).text(location));
             });
         },
         error: function () {
             console.error('Error occurred while fetching location data.');
         }
     });

    $("#locationDropdown").on('change', function () {
        var selectedLocation = $(this).val();

        // Check if a location is selected
        if (selectedLocation) {
            // Make an AJAX request to fetch the turfs
            $.ajax({
                type: "GET",
                url: getTurfsUrl,
                data: { selectedLocation: selectedLocation },
                dataType: 'json',
                success: function (data) {
                    var turfDropdown = $("#turfDropdown");
                    turfDropdown.empty(); // Clear existing options
                    turfDropdown.append($('<option>').val('').text('Select Turf')); // Add default option

                    // Populate the Turf dropdown with data from the AJAX response
                    $.each(data, function (index, turf) {
                        turfDropdown.append($('<option>').val(turf.TurfId).text(turf.TurfName));
                    });

                    // Enable the Turf dropdown
                    turfDropdown.prop("disabled", false);
                },
                error: function (error) {
                    console.log(error);
                }
            });
        } else {
            // If no location is selected, clear and disable the Turf dropdown
            var turfDropdown = $("#turfDropdown");
            turfDropdown.empty(); // Clear existing options
            turfDropdown.prop("disabled", true); // Disable the Turf dropdown
        }
    });
});


    $("#turfDropdown").change(function () {
        selectedTurfId = $(this).val();
        var sportDropdown = $("#sportDropdown");
        var datePicker = $("#datePicker");

        if (selectedTurfId !== "") {
            $.ajax({
                url: getSportsUrl,
                data: { turfId: selectedTurfId },
                type: 'GET',
                dataType: 'json',
                success: function (data) {
                    sportDropdown.empty();
                    sportDropdown.append('<option value="">Select Sport</option>');

                    // Store sport data with IDs in the sportsData object
                    sportsData = {};
                    $.each(data, function (index, sport) {
                        sportsData[sport.SportID] = sport.SportName;
                        sportDropdown.append($('<option>', {
                            value: sport.SportId, // Use sport ID as value
                            text: sport.SportName
                        }));
                    });
                },
                error: function () {
                    sportDropdown.empty();
                    sportDropdown.append('<option value="">Error loading sports</option>');
                }
            });
        } else {
            sportDropdown.empty();
        }
    });
var selectedTimeSlots = [];
    // Sport Dropdown Change Event
    $("#sportDropdown").change(function () {
        var selectedSportId = $(this).val();
        selectedDate = $("#datePicker").val();
        var timeSlotButtonsContainer = $("#timeSlotButtonsContainer");

        if (selectedSportId !== "") {
            $.ajax({
                url: getAvailableTimeSlotsUrl,
                data: { turfId: selectedTurfId, sportId: selectedSportId, date: selectedDate },
                type: 'GET',
                dataType: 'json',
                success: function (data) {
                    timeSlotButtonsContainer.empty();

                    $.each(data, function (index, timeSlot) {
                        var startTime = new Date(parseInt(timeSlot.StartTime.substr(6)));
                        var endTime = new Date(parseInt(timeSlot.EndTime.substr(6)));

                        var startTimeFormatted = startTime.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
                        var endTimeFormatted = endTime.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });

                        var button = $('<button>', {
                            class: 'btn btn-primary btn-sm',
                            value: timeSlot.TimeSlotId,
                            text: startTimeFormatted + ' - ' + endTimeFormatted,
                            click: function () {
                                if (selectedTimeSlots.includes(timeSlot.TimeSlotId)) {
                                    // Unbook time slot
                                    selectedTimeSlots = selectedTimeSlots.filter(id => id !== timeSlot.TimeSlotId);
                                    button.removeClass('btn-success').addClass('btn-primary');
                                } else {
                                    // Book time slot
                                    selectedTimeSlots.push(timeSlot.TimeSlotId);
                                    button.removeClass('btn-primary').addClass('btn-success');
                                }
                            }
                        });

                        timeSlotButtonsContainer.append(button);
                    });
                },
                error: function () {
                    timeSlotButtonsContainer.empty();
                    timeSlotButtonsContainer.append('<p>Error loading time slots</p>');
                }
            });
        } else {
            timeSlotButtonsContainer.empty();
        }
    });

    

    function bookTimeSlot(timeSlotIds) {
        $.ajax({
            url: bookTimeSlotUrl,
            data: { selectedTimeSlots: timeSlotIds, userId: userId, bookingDate: selectedDate, turfId: selectedTurfId },
            type: 'POST',
            traditional: true, // This is important to properly format the array parameter
            success: function (data) {
                if (data.success) {
                    $("#successMessage").show(); // Show success message
                    clearPage(); // Clear page after successful booking
                } else {
                    $("#errorMessage").show(); // Show error message
                }
            },
            error: function () {
                $("#errorMessage").show();
            }
        });
    }

    function resetPage() {
        selectedTimeSlots = [];
        selectedTurfId = undefined;
        selectedDate = undefined;
        sportsData = {};

        $("#datePicker").val("");
        $("#locationDropdown").val("");

        $("#sportDropdown").empty().append('<option value="">Select Sport</option>');
        $("#turfDropdown").empty().append('<option value="">Select Turf</option>');
        

        $("#timeSlotButtonsContainer").empty();

        $("#successMessage").text("").hide();
    }
    function clearPage() {
        selectedTimeSlots = [];
        selectedTurfId = undefined;
        selectedDate = undefined;
        sportsData = {};

        $("#locationDropdown").val("");
        $("#datePicker").val("");
        $("#sportDropdown").empty().append('<option value="">Select Sport</option>');
        $("#locationDropdown").empty().append('<option value="">Select Location</option>');

        $("#timeSlotButtonsContainer").empty();
}

    // Book Now Button Click Event
$("#submitButton").click(function () {
    bookTimeSlot(selectedTimeSlots);
});
    // Clear Button Click Event
    $("#clearButton").click(function () {
        resetPage();
    });

   
