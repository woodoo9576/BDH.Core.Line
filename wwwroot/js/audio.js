window.playUpdatedSound = function () {
    var audio = document.getElementById('updated-audio');
    if (audio) {
        // Dosya tam yüklendiyse hemen çal
        if (audio.readyState >= 3) { // HAVE_FUTURE_DATA
            audio.currentTime = 0;
            audio.play();
        } else {
            // Yüklenene kadar bekle, sonra çal
            audio.addEventListener('canplaythrough', function handler() {
                audio.removeEventListener('canplaythrough', handler);
                audio.currentTime = 0;
                audio.play();
            });
            audio.load();
        }
    }
};
