//ajax
var req = new XMLHttpRequest();

//connect nasa web service
var url = "https://api.nasa.gov/planetary/apod?api_key=";
var api_key = "uQHWlFsAhTgRAXr4BcBPcyh69ImYbfB5ITjIzgSG";

//nasa get function in controller
req.open("GET", url + api_key);
req.send();

//load data to page
req.addEventListener("load", function () {
    if (req.status == 200 && req.readyState == 4) {
        var response = JSON.parse(req.responseText);
        document.getElementById("title").textContent = response.title;
        document.getElementById("date").textContent = response.date;
        document.getElementById("img").src = response.hdurl;
        document.getElementById("explanation").textContent = response.explanation;
    }
})