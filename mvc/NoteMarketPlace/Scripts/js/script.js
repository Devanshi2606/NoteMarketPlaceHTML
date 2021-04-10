/*jslint browser: true*/
/*global $, window, document*/



/*======================================
|   |   |   Login
======================================*/
//Hide incorrect password
$(".incorrect-pass").hide();

// Show/hide password
$(".toggle-password").click(function () {

    $(this).toggleClass("fa-eye fa-eye-slash");
    var input = $($(this).attr("toggle"));
    if (input.attr("type") == "password") {
        input.attr("type", "text");
    } else {
        input.attr("type", "password");
    }
});

$(".toggle-arrow").click(function () {
    $(this).toggleClass("fa-chevron-down fa-chevron-up");
});

$(function () {
    $(".email").css("color", "#000");
});

/*======================================
|   |   |   Sign Up
======================================*/
// hide verification




/*======================================
|   |   |   |   Mobile Menu
======================================*/
$(function () {
    // Show mobile nav
    $("#mobile-nav-open-btn").click(function () {
        $("#mobile-nav").css("height", "100%");
    });
    // Hide mobile nav
    $("#mobile-nav-close-btn, #mobile-nav a").click(function () {
        $("#mobile-nav").css("height", "0%");
    });
});


/*======================================
|   |   |   |   Navigation
======================================*/
$(function () {
    showHideNav();
    $(window).scroll(function () {
        //show hide nav on windows scroll
        showHideNav();
    });

    function showHideNav() {
        if ($(window).scrollTop() > 50) {
            //show white nav
            $("nav").addClass("white-nav-top");
            //show dark logo
            $(".sticky-logo img").attr("src", "../../Content/images/login/logo.png");
            //Show back-to-top Button
            $("#back-to-top").fadeIn();
        } else {
            //hide white nav 
            $("nav").removeClass("white-nav-top");
            $(".sticky-logo img").attr("src", "../../Content/images/login/top-logo.png");
            $("#back-to-top").fadeOut();
        }
    }
});
// Smooth Scrolling
$(function () {
    $("a.smooth-scroll").click(function (event) {
        event.preventDefault();
        // get section id like #about, #services, #work, #team and etc.
        var section_id = $(this).attr("href");
        $("html, body").animate({
            scrollTop: $(section_id).offset().top - 68
        }, 1250, "easeInOutExpo");
    });
});


/*======================================
|   |   |   |   Search Notes
======================================*/
$(window).on('load', function () {
    var matched = $(".count .notes");
    $(".count-note-text").html("Total " + matched.length + " notes")
});

/*======================================
|   |   |   |   FAQ
======================================*/
function handleClick() {
    var cname = $(this).attr('name');
    this.text = (this.text == '+' ? '-' : '+');
    if (this.text == "-") {
        $("." + cname).css("background-color", "#fff");
        $("." + cname + " a").css("background-color", "#fff");
        $("." + cname + " a").css("font-size", "40px");
        $("." + cname).css("border", "2px solid #f4f4f4");
        $("." + cname + " .question-p").css("font-weight", "600");
    }
    if (this.text == "+") {
        $("." + cname).css("background-color", "#f4f4f4");
        $("." + cname + " a").css("background-color", "#f4f4f4");
        $("." + cname + " a").css("font-size", "30px");
        $("." + cname).css("border", "none");
        $("." + cname + " .question-p").css("font-weight", "400");
    }

}
document.getElementById('collapsible1').onclick = handleClick;
document.getElementById('collapsible2').onclick = handleClick;
document.getElementById('collapsible3').onclick = handleClick;
document.getElementById('collapsible4').onclick = handleClick;
document.getElementById('collapsible5').onclick = handleClick;
document.getElementById('collapsible6').onclick = handleClick;
document.getElementById('collapsible7').onclick = handleClick;
document.getElementById('collapsible8').onclick = handleClick;