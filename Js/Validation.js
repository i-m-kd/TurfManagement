
$(document).ready(function () {
    $("#account").submit(function (e) {
        e.preventDefault(); // Prevent the default form submission

        var email = $("#Input_Email").val();
        var isValid = validateEmail(email); // Your custom validation function

        if (!isValid) {
            $("#emailError").text("Invalid email format."); // Display error message
        } else {
            $("#emailError").text(""); // Clear error message
            this.submit(); // Submit the form if valid
        }
    });

    function validateEmail(email) {
        var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
        return regex.test(email);
    }
});
