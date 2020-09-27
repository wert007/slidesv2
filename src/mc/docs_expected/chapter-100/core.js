let slideIndex = 0;
let stepIndex = 0;
let stepNumericalId = 0;
let totalTime = 0;

let currentSlide;
let currentSteps;

let slides;
let steps;

let playingTransition;

let plots;
let ytPlayers = undefined;


let searchModule = (function () {
    let searchModeActive = false;
    let selectedSuggestion = -1;
    let suggestionCount;
    let suggestions = [];

    function enterSearchSlideMode() {
        searchModeActive = !searchModeActive;
        let searchPanel = document.getElementById('search-slide');
        if (searchModeActive) {
            removeClass(searchPanel, 'invisible');
            let searchInput = document.getElementById('search-slide-input');
            searchInput.focus();
            searchSlideInputChanged();
        } else {
            addClass(searchPanel, 'invisible');
        }
    }

    function searchSlideInputChanged() {
        let searchInput = document.getElementById('search-slide-input');
        let content = searchInput.value;
        let alreadyTyped = document.getElementById('search-slide-input-already-typed');
        alreadyTyped.innerText = content; //.replace(' ', '&nbsp;'); //&nbsp;
        let suggestionsList = document.getElementById('search-slide-suggestions');
        suggestionsList.innerHTML = '';
        let addSuggestion = (toAdd, index) => {
            let newElement = document.createElement('li');
            newElement.innerHTML = toAdd;
            newElement.onmouseover = function () {
                selectedSuggestion = index;
                updateSuggestionPaint();
            };
            newElement.onclick = function () {
                startSearch();
            };
            suggestions.push(newElement);
            suggestionsList.appendChild(newElement);
        };
        suggestionCount = 0;
        suggestions = [];
        for (const s in slides) {
            if (slides.hasOwnProperty(s)) {
                const element = slides[s];
                if (element.id.substr(0, content.length) !== content) continue;
                addSuggestion(element.id, suggestionCount);
                suggestionCount += 1;
            }
        }
        selectedSuggestion = Math.min(suggestionCount - 1, selectedSuggestion);
        updateSuggestionPaint();
    }

    function updateSuggestionPaint() {
        let searchInput = document.getElementById('search-slide-input');
        let expectedElement = document.getElementById('search-slide-input-expected');
        if (selectedSuggestion >= 0) {
            let alreadyTypedText = searchInput.value;
            let suggestionText = suggestions[selectedSuggestion].innerText;
            expectedElement.innerText = suggestionText.substr(alreadyTypedText.length);
        }
        else {
            expectedElement.innerText = '';
        }
        for (let i = 0; i < suggestions.length; i++) {
            const element = suggestions[i];
            if (i == selectedSuggestion)
                addClass(element, 'selected');
            else
                removeClass(element, 'selected');
        }
    }

    function startSearch() {
        let searchInput = document.getElementById('search-slide-input');
        let content = searchInput.value;
        if (selectedSuggestion >= 0)
            content = suggestions[selectedSuggestion].innerText;
        let foundSomething = false;
        let i = 0;
        for (let s of slides) {
            if (s.id == content) {
                stepIndex = 0;
                slideIndex = i;
                foundSomething = true;
                break;
            }
            i += 1;
        }
        loadSlides();
        showSlides();
        if (foundSomething) {
            enterSearchSlideMode();
            searchInput.value = '';
            selectedSuggestion = -1;
        }
    }

    function keyDown(event) {
        let textBoxHasFocus = document.getElementById('search-slide-input') === document.activeElement;
        if (event.code === 'ArrowUp') {
            selectedSuggestion = Math.max(selectedSuggestion - 1, -1);
            updateSuggestionPaint();
        }
        else if (event.code === 'ArrowDown') {
            selectedSuggestion = Math.min(selectedSuggestion + 1, suggestionCount - 1);
            updateSuggestionPaint();
        }
        else if (textBoxHasFocus && event.code === 'Enter')
            startSearch();
        else if (event.code == 'Escape') {
            searchModule.enterSearchSlideMode();
        }
        else return;
        event.preventDefault();
    }

    function isSearchModeActive() {
        return searchModeActive;
    }

    return {
        isSearchModeActive: isSearchModeActive,
        enterSearchSlideMode: enterSearchSlideMode,
        searchSlideInputChanged: searchSlideInputChanged,
        startSearch: startSearch,
        keyDown: keyDown,
    };
})();

window.onhashchange = function () {
    let wanted = window.location.hash;
    for (let i = 0; i < slides.length; i++) {
        if ('#' + slides[i].id === wanted) {
            slideIndex = i;
            break;
        }
    }
    loadSlides();
    showSlides();
};

function load() {
    YT = undefined;
    plots = [];
    transitionDone = false;
    slides = document.getElementsByClassName('slide');
    steps = document.getElementsByClassName('step');
    let transitions = document.getElementsByClassName('transition');
    let wanted = window.location.hash;
    slideIndex = 0;
    for (let i = 0; i < transitions.length; i++) {
        addClass(transitions[i], 'invisible');
    }
    for (let i = 0; i < slides.length; i++) {
        addClass(slides[i], 'invisible');
        if ('#' + slides[i].id === wanted) {
            slideIndex = i;
            break;
        }
    }
    for (let i = 0; i < steps.length; i++) {
        addClass(steps[i], 'invisible');
    }
    loadSlides();
    showSlides();
    loadInner();

    let timerId = setInterval(() => {
        totalTime += 20;
        update_totalTime();
        if (YT === undefined) return;
        if (YT.loaded && ytPlayers == undefined) {
            youtubeAPIReady();
        }
    }, 20);

    initAnimations();
}

function prev() {
    // console.log(slideIndex + ' ' + stepIndex);
    stepNumericalId = Math.max(stepNumericalId - 1, 0);

    if (stepIndex == 0) {
        slideIndex = Math.max(slideIndex - 1, 0);
        loadSlides();

        let transition = document.getElementById(currentSlide.dataset.transitionId)
        if (transition != undefined)
            playTransition(transition);

        stepIndex = currentSteps.length - 1;
        if (stepIndex < 0) {
            console.error('It seems no steps have been load.')
            console.log(currentSlide);
            console.log(currentSteps);
        }
    }
    else {
        stepIndex--;
    }

    playAnimations(1);
    showSlides();
}

function next() {
    // console.log(slideIndex + ' ' + stepIndex);
    stepNumericalId = Math.min(stepNumericalId + 1, steps.length - 1);

    let max;
    if (currentSteps != undefined)
        max = currentSteps.length - 1;
    else
        max = 0; //Or One??
    if (stepIndex == max) {
        let transition = document.getElementById(currentSlide.dataset.transitionId)
        if (transition != undefined)
            playTransition(transition);
        slideIndex = Math.min(slideIndex + 1, slides.length - 1);
        loadSlides();
        stepIndex = 0; //Will reset StepIndex no matter if we changed actual Slide ..
    }
    else {
        stepIndex++;
    }

    playAnimations(-1);
    showSlides();
}

function playAnimations(offset) {
    let animations = getAnimations();
    let actualNumericalId = steps[stepNumericalId].dataset.stepNumericalId;
    for (let i = 0; i < animations.length; i++) {
        const animation = animations[i];
        if (animation.step_numerical_id == actualNumericalId) {
            let element = document.getElementById(animation.element_id);
            animate(element, animation.animation, animation.duration);
        }
        else if (animation.step_numerical_id == actualNumericalId + offset) {
            let element = document.getElementById(animation.element_id);
            revertAnimation(element);
        }
    }
}

function animate(element, animation, duration) {
    let progress = 0;
    let timerId = setInterval(() => {
        animation(progress / duration, element)
        progress += 20;
        if (progress > duration) {
            clearInterval(timerId);
        }
    }, 20);
}

let init_elements = [];

function initAnimations() {
    for (let i = 0; i < animations.length; i++) {
        const element = animations[i].element_id;
        init_elements[element] = document.getElementById(element);
    }
}

function getPlot(id) {
    for (let i = 0; i < plots.length; i++) {
        if (plots[i].id == id)
            return plots[i].value;
    }
    return undefined;
}

function revertAnimation(element) {
    element.style = init_elements[element.id];
}

function loadSlides() {
    slides = document.getElementsByClassName('slide');
    steps = document.getElementsByClassName('step');
    for (let i = 0; i < slides.length; i++) {
        if (i == slideIndex)
            currentSlide = slides[i];
        addClass(slides[i], 'invisible');
    }
    stepLength = 0;
    currentSteps = [];
    for (let i = 0; i < steps.length; i++) {
        addClass(steps[i], 'invisible');
        if (steps[i].dataset.slideId === currentSlide.id) {
            currentSteps.push(steps[i]);
            stepLength++;
        }
    }
    window.location.hash = currentSlide.id;
}

function handleYouTubePlayers() {
    if (ytPlayers === undefined)
        return;


    var actualNumericalId = parseInt(currentSteps[currentSteps.length - 1].dataset.stepNumericalId);

    for (let p of ytPlayers) {
        if ((currentSlide.id !== p.slideId || p.stepNumericalId > actualNumericalId)
            && !p.keepPlaying) {
            p.autoPaused = p.getPlayerState() != YT.PlayerState.PAUSED
            p.pauseVideo();
        }
        else if (p.getPlayerState() == YT.PlayerState.PAUSED && p.autoPaused)
            p.playVideo();
    }
}

function showSlides() {
    removeClass(currentSlide, 'invisible');
    currentSlide.startTime = totalTime;
    handleYouTubePlayers();
    let localVisibleSteps = stepIndex + 1;
    for (let i = 0; i < steps.length; i++) {
        if (currentSteps.includes(steps[i]) && localVisibleSteps > 0) {
            removeClass(steps[i], 'invisible');
            localVisibleSteps--;
            if (localVisibleSteps == 0) {
                update_step(steps[i]);
                break;
            }
        }
    }
}

function playTransition(transition) {
    if (playingTransition != undefined) {
        reset_animation(playingTransition);
    }
    playingTransition = transition;
    //Activate transition
    addClass(playingTransition, 'active');
    removeClass(playingTransition, 'invisible');
    let millisecondsToWait = playingTransition.dataset.duration;
    setTimeout(function () {
        //Transition is done
        addClass(transition, 'invisible');
        removeClass(transition, 'active');
        playingTransition = undefined;
    }, millisecondsToWait);
}

function keyDown(event) {
    // console.log(event);
    var textBoxHasFocus = document.getElementById('search-slide-input') === document.activeElement;
    if (searchModule.isSearchModeActive()) {
        searchModule.keyDown(event);
        return;
    }
    if (event.code === 'ArrowUp') {
        prev();
    }
    else if (event.code === 'ArrowDown')
        next();
    else if (!textBoxHasFocus && event.code === 'KeyF')
        enterFullScreen();
    else if (!textBoxHasFocus && event.code === 'KeyG')
        searchModule.enterSearchSlideMode();
    else return;
    event.preventDefault();
}

//source: https://stackoverflow.com/questions/6268508/restart-animation-in-css3-any-better-way-than-removing-the-element#6303311
function reset_animation(el) {
    el.style.animation = 'none';
    el.offsetHeight; /* trigger reflow */
    el.style.animation = null;
}

//source: https://stackoverflow.com/questions/9454125/javascript-request-fullscreen-is-unreliable#9747340
function enterFullScreen() {

    var isInFullScreen = (document.fullScreenElement && document.fullScreenElement !== null) ||    // alternative standard method  
        (document.mozFullScreen || document.webkitIsFullScreen);

    var docElm = document.documentElement;
    if (!isInFullScreen) {

        if (docElm.requestFullscreen) {
            docElm.requestFullscreen();
        }
        else if (docElm.mozRequestFullScreen) {
            docElm.mozRequestFullScreen();
        }
        else if (docElm.webkitRequestFullScreen) {
            docElm.webkitRequestFullScreen();
        }
    }
}

//source: https://jaketrent.com/post/addremove-classes-raw-javascript/
function hasClass(el, className) {
    if (el.classList)
        return el.classList.contains(className)
    else
        return !!el.className.match(new RegExp('(\\s|^)' + className + '(\\s|$)'))
}

function addClass(el, className) {
    if (el.classList)
        el.classList.add(className)
    else if (!hasClass(el, className)) el.className += ' ' + className
}

function removeClass(el, className) {
    if (el.classList)
        el.classList.remove(className)
    else if (hasClass(el, className)) {
        var reg = new RegExp('(\\s|^)' + className + '(\\s|$)')
        el.className = el.className.replace(reg, ' ')
    }
}