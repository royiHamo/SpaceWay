function initMap() {
    var myLatLng = { lat: 32.085300, lng: 34.781769 };

    var map = new google.maps.Map(document.getElementById('map'), {
        zoom: 10,
        center: myLatLng
    });

    var marker = new google.maps.Marker({
        position: myLatLng,
        map: map,
    });

    var marker2 = new google.maps.Marker({
        position: { lat: 32.321457, lng: 34.853195 },
        map: map,
    });
}