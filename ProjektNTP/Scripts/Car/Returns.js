$(document).ready(function () {
    $("#Car_Mileage").change(RewriteDistanceInputValue);
})
function RewriteDistanceInputValue() {
    var distanceValue = $("#Car_Mileage").val();
    $('#numberOfDistance').val(distanceValue);
}