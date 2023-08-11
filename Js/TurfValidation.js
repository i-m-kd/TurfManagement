$(document).ready(function () {
    $("#turfDropdown").change(function () {
        var selectedTurfId = $(this).val();
        var sportDropdown = $("#sportDropdown");

        if (selectedTurfId !== "") {
            $.ajax({
                url: getSportsUrl, 
                data: { turfId: selectedTurfId },
                type: 'GET',
                dataType: 'json',
                success: function (data) {
                    sportDropdown.empty();
                    sportDropdown.append('<option value="">Select Sport</option>');

                    $.each(data, function (index, sport) {
                        sportDropdown.append($('<option>', {
                            value: sport.SportID,
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
});
