
//start the function after clicking the button
$(document).ready($('#sendSearch').click(sendAjax));
$(document).ready(loadAjaxGet);

//send and accept ajax
function sendAjax() {

    //take the values from the html
    var type = $(".rButtons:checked").val();
    var inputint = $("#input").val();
    var input = inputint.toString();
    var url = '/Passengers/Index/';

    //create ajax request
    $.ajax({
        type: "POST",

        //send to function Search in Controller
        url: url,

        //Serialize to json
        data: JSON.stringify({ type: type, input: input }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            //remove old data from html
            $("#resultsTable > tbody").html('');
            //create the table with the new results
            $.each(response.passengers, function (index, passenger) {
                $("#resultsTable > tbody").append(`
                            <tr><td>${passenger.Name}</td>
                            <td>${passenger.Username}</td>
                            <td>${passenger.Stars}</td>
                            <td>${passenger.IsAdmin}</td>
                            <td>${passenger.TotalDistance}</td>
                            <td>
                             <a href="/Passengers/Edit/${passenger.PassengerID}">Edit</a>
                            <a href="/Passengers/Details/${passenger.PassengerID}">Details</a>
                            <a href="/Passengers/Delete/${passenger.PassengerID}">Delete</a>
                            </td>
                          </tr>`);
            });
        },
        //alert in case of an error in the function
        error: function () {
            alert("Error");
        }
    });
    return false;
}


//get all the passengers in GET request
function loadAjaxGet() {
    //take the values from the html
    var url = '/Passengers/IndexGetAjax/';

    //create ajax request
    $.ajax({
        type: "GET",

        //send to function Search in Controller
        url: url,

        //Serialize to json
        data: {},
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            //remove old data from html
            $("#resultsTable > tbody").html('');
            //create the table with the new results
            $.each(response.passengers, function (index, passenger) {
                $("#resultsTable > tbody").append(`
                            <tr><td>${passenger.Name}</td>
                            <td>${passenger.Username}</td>
                            <td>${passenger.Stars}</td>
                            <td>${passenger.IsAdmin}</td>
                            <td>${passenger.TotalDistance}</td>
                            <td>
                             <a href="/Passengers/Edit/${passenger.PassengerID}">Edit</a>
                            <a href="/Passengers/Details/${passenger.PassengerID}">Details</a>
                            <a href="/Passengers/Delete/${passenger.PassengerID}">Delete</a>
                            </td>
                          </tr>`);
            });
        },
        //alert in case of an error in the function
        error: function () {
            alert("Error");
        }
    });
    return false;
}