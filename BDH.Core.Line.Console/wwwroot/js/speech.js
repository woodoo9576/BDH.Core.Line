window.speechQueue = [];
window.speechActive = false;

window.speakText = function (text) {
    if (!('speechSynthesis' in window)) {
        console.error('Tarayıcınız konuşma sentezini desteklemiyor.');
        return;
    }
    window.speechQueue.push(text);
    if (!window.speechActive) {
        window.processSpeechQueue();
    }
};

window.processSpeechQueue = function () {
    if (window.speechQueue.length === 0) {
        window.speechActive = false;
        return;
    }
    window.speechActive = true;
    const text = window.speechQueue.shift();
    const utterance = new window.SpeechSynthesisUtterance(text);
    utterance.lang = 'tr-TR';
    utterance.rate = 0.7;
    const voices = window.speechSynthesis.getVoices();
    
    const selectedVoice = voices.find(v => v.lang === 'tr-TR');
    //const selectedVoice = voices[310];
    if (selectedVoice) {
        utterance.voice = selectedVoice;
    }
    utterance.onend = function () {
        window.processSpeechQueue();
    };
    window.speechSynthesis.speak(utterance);
};