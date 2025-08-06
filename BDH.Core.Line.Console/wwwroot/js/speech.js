window.speakText = function (text) {
    if (!('speechSynthesis' in window)) {
        console.error('Tarayıcınız konuşma sentezini desteklemiyor.');
        return;
    }

    const utterance = new SpeechSynthesisUtterance(text);
    utterance.lang = 'tr-TR';
    utterance.rate = .8;

    const voices = window.speechSynthesis.getVoices();
    const selectedVoice = voices.find(v => v.lang === 'tr-TR');
    if (selectedVoice) {
        utterance.voice = selectedVoice;
    }

    window.speechSynthesis.speak(utterance);
};