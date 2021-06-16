$(document).ready(function () {
    //auto sets the date picker and time picer on the form to todays date and time when page loaded
    $("#CalculationDetails").hover(function () {
        $(this).css("background-color", "yellow");
    }, function () {
        $(this).css("background-color", "pink");
    });
});


function displayCalculationDetails() {
    var btn = document.getElementById("CalculationDetails");
    var div = document.getElementById("openCalculationDetails");

    btn.addEventListener('click', () => {
        if (div.style.display === "none") {
            div.style.display = "block";
        }
        else {
            div.style.display = "none";
        }
    })
};
