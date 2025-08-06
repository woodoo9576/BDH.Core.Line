window.playUpdatedSound = function () {
         var audio = document.getElementById('updated-audio');
         if (audio) {
             audio.currentTime = 0;
             audio.play();
         }
     }