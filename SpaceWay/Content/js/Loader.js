function load() {
    //show the loader div
    document.getElementById('loader').style.visibility = 'visible';
    //load for 3 seconds and the submit the form, if not valid show error
    setTimeout(function () { $("#realS").click(); document.getElementById('loader').style.visibility = 'hidden';}, 3000);
};