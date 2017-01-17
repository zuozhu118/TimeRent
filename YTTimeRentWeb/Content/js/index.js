//图片切换见

var pos = 0;

var bar = {
    "e100": $("#e100"),
    "e110": $("#e110")
}

var unslider = $('.banner').unslider({
    speed: 500,
    delay: 3600000,
    complete: function() {},
    keys: false,
    dots: false,
    fluid: false
});

var data = unslider.data('unslider');

function slide (position, direction) {
    if (position == 0 && direction == "next") {
        data.next();
        pos = 1;
        bar_change(bar, "right");
        $("#prev").css({ 'background-image': 'url("/content/img/home/left_normal.png")' });
        $("#next").css({ 'background-image': 'url("/content/img/home/right_hover.png")' });
    } else if (position == 1 && direction == "prev") {
        data.prev();
        pos = 0;
        bar_change(bar, "left");
        $("#prev").css({ 'background-image': 'url("/content/img/home/left_hover.png")' });
        $("#next").css({ 'background-image': 'url("/content/img/home/right_normal.png")' });
    }
}

function bar_change (obj, direction) {
    if (direction == "left") {
        obj.e100.addClass("nav-selected")
        obj.e110.removeClass("nav-selected")
    } else if (direction == "right") {
        obj.e110.addClass("nav-selected")
        obj.e100.removeClass("nav-selected")
    }

    $("#navBar").css({"float": direction})
}

//arrow点击事件
$('.unslider-arrow').click(function() {
    var fn = this.className.split(' ')[1];
    slide(pos, fn);
});

//保证图片高度
$(".banner").css({"height":"679px"})

//nav点击事件
$("#e100").click(function () {
    bar_change(bar, "left");
    slide(pos, "prev");
});

$("#e110").click(function () {
    bar_change(bar, "right");
    slide(pos, "next");
});


