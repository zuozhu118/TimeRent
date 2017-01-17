
$(".main").onepage_scroll({
    sectionContainer: "section",
    easing: "ease",
    animationTime: 1000,
    pagination: true,
    updateURL: false,
    beforeMove: function(index) {
        if (index >= 2) {
            $("header").slideUp(500);
            $("nav").css({
                "position": "fixed",
                "top": 0,
                "left": 0,
                "z-index": 9
            });
        }
    },
    afterMove: function(index) {
        if (index == 1) {
            $("header").slideDown(500);
            $("nav").css({
                "position": "relative"
            });
        }
    },
    loop: false,
    keyboard: true,
    responsiveFallback: false,
    direction: "vertical"
});