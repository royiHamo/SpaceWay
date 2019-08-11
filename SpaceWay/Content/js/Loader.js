function load() {
    //show the loader div
    document.getElementById('loader').style.visibility = 'visible';
    //load for 5 seconds and the submit the form
    setTimeout(function () { $("#MyForm").submit(); }, 4000);
};