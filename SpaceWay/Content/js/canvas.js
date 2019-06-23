function canvasFunc() {
    var x = 0;
    var y = 0;
    var key, pos = 0;
    var canvas = document.getElementById("myCanvas");
    var ctx = canvas.getContext("2d");
    var w = canvas.clientWidth - 70;
    var h = canvas.clientHeight - 70;
    var img = new Image();
    img.src = "~/Content/img/spaceship.png";
  //  img.src = "https://emojipedia-us.s3.dualstack.us-west-1.amazonaws.com/thumbs/240/microsoft/209/flying-saucer_1f6f8.png";
   

    img.onload = function () {
        ctx.drawImage(img, x, y, 60, 60);
    }

    document.onkeydown = function (e) {
        pos++;
        key = window.event ? e.keyCode : e.which;
    }
    document.onkeyup = function (e) { pos-- }
    setInterval(function () {
        if (pos === 0) return;
        if (x > -1 && x < w) {
            if (key === 37) x -= 2;
            if (key === 39) x += 2;
            if (x < 0) x = 0;
            if (x > w - 1) x = w - 1;
        }
        if (y > -1 && y < h) {
            if (key === 38) y -= 2;

            if (key === 40) y += 2;
            if (y < 0) y = 0;
            if (y > h - 1) y = h - 1;
        }

        canvas.width = canvas.width;
        ctx.drawImage(img, x, y, 60, 60);
    }, 5);
}
