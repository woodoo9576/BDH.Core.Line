window.playUpdatedSound = function () {
    var audio = document.getElementById('updated-audio');
    if (audio) {
        // Dosya tam y�klendiyse hemen �al
        if (audio.readyState >= 3) { // HAVE_FUTURE_DATA
            audio.currentTime = 0;
            audio.play();
        } else {
            // Y�klenene kadar bekle, sonra �al
            audio.addEventListener('canplaythrough', function handler() {
                audio.removeEventListener('canplaythrough', handler);
                audio.currentTime = 0;
                audio.play();
            });
            audio.load();
        }
    }
};
