function initMap() {
    var centerLatLng = { lat: 32.434048, lng: 34.919651 };                  // define map's center view

    var map = new google.maps.Map(document.getElementById('map'), {         // init map
        zoom: 9,
        center: centerLatLng
    });

    var marker = new google.maps.Marker({                                   // init markers (out stations)
        position: { lat: 32.085300, lng: 34.781769 },
        map: map,
    });

    var marker2 = new google.maps.Marker({
        position: { lat: 32.321457, lng: 34.853195 },
        map: map,
    });
    var marker3 = new google.maps.Marker({
        position: { lat: 32.794044, lng: 34.989571 },
        map: map,
    });
}