$(function() {

    "use strict";

    $('.chat-left-inner > .chatonline, .chat-rbox').perfectScrollbar();

    var cht = function() {
        var topOffset = 450;
        var height = ((window.innerHeight > 0) ? window.innerHeight : this.screen.height) - 1;
        height = height - topOffset;
        $(".chat-list").css("height", (height) + "px");
    };
    $(window).ready(cht);
    $(window).on("resize", cht);

    // this is for the left-aside-fix in content area with scroll
    var chtin = function() {
        var topOffset = 270;
        var height = ((window.innerHeight > 0) ? window.innerHeight : this.screen.height) - 1;
        height = height - topOffset;
        $(".chat-left-inner").css("height", (height) + "px");
    };
    $(window).ready(chtin);
    $(window).on("resize", chtin);

    $(". Open-panel").on("click", function() {
        $(".chat-left-aside").toggleClass(" Open-pnl");
        $(". Open-panel i").toggleClass("ti-angle-left");
    });
});