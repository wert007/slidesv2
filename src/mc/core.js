let sildeIndex = 0;
let stepIndex = 0;
let stepNumericalId = 0;

let currentSlide;
let currentSteps;

let slides;
let steps;

let playingTransition;

function load() {
    index = 0;
    stepIndex = 0;
    transitionDone = false;
    slides = document.getElementsByClassName('slide');
    steps = document.getElementsByClassName('step');
    let transitions = document.getElementsByClassName('transition');
    for(let i = 0; i < transitions.length; i++)
    {
        addClass(transitions[i], 'invisible');
    }
    for (let i = 0; i < slides.length; i++) {
        addClass(slides[i], 'invisible');
    }
    for (let i = 0; i < steps.length; i++) {
        addClass(steps[i], 'invisible');
    }
    loadSlides();
    showSlides();

    initAnimations();
}

function prev() {
   // console.log(slideIndex + ' ' + stepIndex);
    stepNumericalId = Math.max(stepNumericalId - 1, 0);
 
    if (stepIndex == 0) {
        sildeIndex = Math.max(sildeIndex - 1, 0);
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
    stepNumericalId = Math.min(stepNumericalId + 1, steps.length);

    let max;
    if (currentSteps != undefined)
        max = currentSteps.length - 1;
    else
        max = 0; //Or One??
    if (stepIndex == max) {
        let transition = document.getElementById(currentSlide.dataset.transitionId)
        if (transition != undefined)
            playTransition(transition);
        sildeIndex = Math.min(sildeIndex + 1, slides.length - 1);
        loadSlides();
        stepIndex = 0; //Will reset StepIndex no matter if we changed actual Slide ..
    }
    else {
        stepIndex++;
    }

    playAnimations(-1);
    showSlides();
}

function playAnimations(offset)
{
    var animations = getAnimations();
    for (let i = 0; i < animations.length; i++) {
        const animation = animations[i];
        if(animation.step_numerical_id == stepNumericalId)
        {
            let element = document.getElementById(animation.element_id);
            animate(element, animation.animation, animation.duration);
        }
        else if(animation.step_numerical_id == stepNumericalId + offset)
        {
            let element = document.getElementById(animation.element_id);
            revertAnimation(element);
        }
    }
}

function animate(element, animation, duration)
{
    let progress = 0;
    let timerId = setInterval(() => {
        animation(progress / duration, element)
        progress += 20;
        if(progress > duration)
        {
            clearInterval(timerId);
        }
    }, 20);
}

let init_elements = [];

function initAnimations()
{
    for (let i = 0; i < animations.length; i++) {
        const element = animations[i].element_id;
        init_elements[element] = document.getElementById(element);
    }
}

function revertAnimation(element)
{
    element.style = init_elements[element.id];
}

function loadSlides() {
    slides = document.getElementsByClassName('slide');
    steps = document.getElementsByClassName('step');
    for (let i = 0; i < slides.length; i++) {
        if (i == sildeIndex)
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
}

function showSlides() {
    removeClass(currentSlide, 'invisible');
    let localVisibleSteps = stepIndex + 1;
    for (let i = 0; i < steps.length; i++) {
        if (currentSteps.includes(steps[i]) && localVisibleSteps > 0) {
            removeClass(steps[i], 'invisible');
            localVisibleSteps--;
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

function enterSearchSlideMode() {

}

function keyDown(event) {
    //console.log(event);
    if (event.code === 'ArrowUp')
        prev();
    else if (event.code === 'ArrowDown')
        next();
    else if (event.code === 'KeyF')
        enterFullScreen();
    else if (event.code === 'KeyG')
        enterSearchSlideMode();
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