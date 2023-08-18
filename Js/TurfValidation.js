$(document).ready(function () {
    var selectedTimeSlots = [];
    var selectedTurfId;
    var sportsData = {}; 
    var selectedDate;
    // Turf Dropdown Change Event
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
                                console.log("Button clicked for time slot ID:", timeSlot.TimeSlotId);
                                if (selectedTimeSlots.includes(timeSlot.TimeSlotId)) {
                                    // Unbook time slot
                                    console.log("Removing time slot ID from selectedTimeSlots:", timeSlot.TimeSlotId);
                                    selectedTimeSlots = selectedTimeSlots.filter(id => id !== timeSlot.TimeSlotId);
                                    button.removeClass('btn-success').addClass('btn-primary');
                                } else {
                                    console.log("Adding time slot ID to selectedTimeSlots:", timeSlot.TimeSlotId);
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

    // Book Now Button Click Event
    $("#submitButton").click(function () {
        console.log("Button clicked!");
        console.log("Selected time slot IDs:", selectedTimeSlots);//Remove this
        bookTimeSlot(selectedTimeSlots);
    });

    function bookTimeSlot(timeSlotIds) {
        $.ajax({
            url: bookTimeSlotUrl,
            data: { selectedTimeSlots: timeSlotIds, userId: userId, bookingDate: selectedDate, turfId: selectedTurfId },
            type: 'POST',
            traditional: true, // This is important to properly format the array parameter
            success: function (data) {
                if (data.success) {
                    console.log("Booking Successful");
                    $("#successMessage").show(); // Show success message
                    clearPage(); // Clear page after successful booking
                } else {
                    console.log("Booking Failed");
                    $("#errorMessage").show(); // Show error message
                }
            },
            error: function () {
                console.log('An error occurred while booking the time slot.');
                $("#errorMessage").show();
            }
        });
    }

    function resetPage() {
        selectedTimeSlots = [];
        selectedTurfId = undefined;
        selectedDate = undefined;
        sportsData = {};

        $("#turfDropdown").val("");
        $("#sportDropdown").empty().append('<option value="">Select Sport</option>');
        $("#datePicker").val("");

        $("#timeSlotButtonsContainer").empty();

        $("#successMessage").text("").hide();
    }
    function clearPage() {
        selectedTimeSlots = [];
        selectedTurfId = undefined;
        selectedDate = undefined;
        sportsData = {};

        $("#turfDropdown").val("");
        $("#sportDropdown").empty().append('<option value="">Select Sport</option>');
        $("#datePicker").val("");

        $("#timeSlotButtonsContainer").empty();
    }

    // Clear Button Click Event
    $("#clearButton").click(function () {
        resetPage();
    });

});
