
$(function () {
    InitCenter();
});

function InitCenter() {
    $('.contain-left a').click(function () {
        $(this).parent().css('backgroundColor', '#c0ff80').siblings().css('backgroundColor', 'white');
    });
    $('.Order-Name a').click(function () {
        $(this).css('color', '#effae7').parent().css('backgroundColor', '#74C621').siblings().css('backgroundColor', 'white').children().css('color', '#666666');//JQuery链式编程；

    });

    if ($('#countspan').html() == 0) {
        $('#orderdiv').html("");
        $('#orderdiv').html("<div id='orderzero'><ul><li><img src='/Images/zc_03.png' /> </li><li>还没租过车？速速来体验吧</li><li><input type='button' name='name' value='立即租车'  onclick='ShortlyRENT()' /></li></ul></div>");
    }

}
function ShortlyRENT() {
    window.location.href = "/YTTimeRent/Index";
}
function OrdersRealod() {
    window.location.reload();

}
function Addtbodydata(data) {
    $("#tbodyID").html("");//清空tbodyID中的内容；
    for (var key in data.Personallist) {
        //将数据添加到tbody中：
        var fee = parseFloat(data.Personallist[key].BudgetCost);
        $("#tbodyID").append("<tr><td class='ordernum'><p class='orderone'>"
            + data.Personallist[key].CarName + "</p><p class='orderone'>订单号：<span>"
            + data.Personallist[key].OrderNum + "</span></p></td><td class='msgtd'><div><p class='adress'><img src='/Images/web3_03.png' /><span>"
            + data.Personallist[key].PicAddress + "</span><br /><span class='timespan'>"
            + data.Personallist[key].StartTime + "</span></p></div><div><p class='adress'><img src='/Images/web3_06.png' /><span>"
            + data.Personallist[key].RetAddress + "</span><span class='timespan'>"
            + data.Personallist[key].EndTime + "</span></p></div></td><td class='totalprice-td'>￥"
            + fee + "</td><td ><span>"
            + data.Personallist[key].OrderStatus + "</span><br /> <a href='/PersonalCenter/PersonalDetial?ordernum="
            + data.Personallist[key].OrderNum + "' target=_blank class='detiallink'>详情</a></td></tr>");

    }

    $("#LinkdivID").html(data.Pagerlink);//把所有的分页标签添加到页面上；

}
function Orders(Number) {
    $.post("/PersonalCenter/OrderStatus", { number: Number }, function (data) {
        //alert(data);
        Addtbodydata(data);
    });
    return false;
}

function BindNavLinkClick(element) {
    var strpostdata = element.substr(element.lastIndexOf("?") + 1);
    $.ajax({
        url: "/PersonalCenter/ProcessPager",
        asyn: true,//异步请求
        data: strpostdata,//给后台传递的参数
        dataType: "json",//后台返回数据类型
        type: "post",//请求类型;
        success: function (data) {
            Addtbodydata(data);
        }
    });
}


function CARDSClick() {
    $('#footdiv').css('display', 'block');
    $('#footdiv').html(' <p class="p1">储值卡规则</p><p>1.储值卡支持预付、租金结算、租金押金支付（仅限预授权5000元以下的车型）和违章押金支付；</p><p>2.使用储值卡消费不返积分；</p><p>3.储值卡不计息、不兑现、不透支、不可充值，有效期三年，过期余额不退；</p>');

    var card = $('#Cards').html();
    if (card == 0) {
        var STime = $('#Timespan').html();
        var rent = '<div class="rentorder"><span>我的储值卡</span><div class="order"><input type="text" class="rentday"  value=' + STime + ' /></div><div class="order"><input type="text" class="rentday" value=' + STime + ' /></div><input type="button" name="name" value="查询" /></div>';
        $('#rentorder').html(rent);
        $('#OrderState').html("");
        $('#OrderState').html("<p class='cardP'>暂无可用储值卡</p><div id='orderzero'><ul><li><img src='/Images/web2_03.png' /> </li><li>没有消费记录，快快丰富起来~</li><li><input type='button' name='name' value='立即租车' /></li></ul></div>");
    }
}

function BCountClick() {
    var accountmoney = $('#blancecount').html();//账户余额
    var STime = $('#Timespan').html();
    var rent = '<div class="rentorder"><span>我的储值卡</span><div class="order"><input type="text" class="rentday" value=' + STime + ' /></div><div class="order"><input type="text" class="rentday" value=' + STime + ' /></div><input type="button" name="name" value="查询" /></div>';
    $('#rentorder').html(rent);
    $('#OrderState').html("");
    $('#OrderState').html("<p class='cardP'>账户余额：<span class='Amoney'>￥" + accountmoney + "</span><input type='button' value='充值' class='RechargeBtn' /></p><div id='orderzero'><ul><li><img src='/Images/web1_10.png' /> </li><li>没有消费记录，快快丰富起来~</li><li><input type='button' onclick='ShortlyRENT()' name='name' value='立即租车' /></li></ul></div>");
    $('#footdiv').css('display', 'none');
    //DatePicker();
    $(".rentday").datepicker();
}

function CouponClick() {
    $('#rentorder').html("");
    $('#OrderState').html("");
    $('#footdiv').css('display', 'none');
    var coupon = $('#coupon').html();
    $('#rentorder').html('<div class="mycoupon"><p>我的优惠劵&nbsp&nbsp|&nbsp&nbsp可用</p><span>' + coupon + '张</span></div><div class="couponstate"><ul><li>全部</li><li>未使用</li><li>已使用</li><li>已过期</li></ul></div>')
    $('#OrderState').html('<div class="couponMSG"><ul>  <li class="overdue"><p class="couponPone">￥<span class="couponmoney">100</span><span>已过期</span></p><p class="couponPtwo">有效期:&nbsp&nbsp<span>2015.01.01-2016.01.01</span></p></li></ul></div>');
    //<li class="aviablecoupon"><p class="couponPone">￥<span class="couponmoney">100</span><span>租车劵</span></p><p class="couponPtwo">有效期:&nbsp&nbsp<span>2016.01.01-2017.01.01</span></p></li>  
    $('.couponstate li').click(function () {
        $(this).css('backgroundColor', '#74c621').siblings().css('backgroundColor', 'white');
    });

    DatePicker();
}

function SelectOrder(Number) {//点击查询按钮查询订单；
    var Pictimes = $('#pickday').val();
    var RTime = $('#returnday').val();
    $.post("/PersonalCenter/SelectOrder", { pictime: Pictimes, rtime: RTime}, function (data) {
        Addtbodydata(data);

    });
}

function UserAuthenticate() {
    $('#rentorder').html('<span class="rentorder">用户信息认证</span>');
    $('#OrderState').html('<div class="box"><form action="/PersonalCenter/ImgUpLoad" method="post" target="frameFile" enctype="multipart/form-data" name="form1"><div class="fl"><input type="hidden" name="card" id="hiddenId"/><span class="sfcard">本人身份证正面</span><div class="imgbox" id="UpId">点击下面按钮上传图片</div><label for="xFile1" id="lab1" class="lab">上传图片</label><input type="file" id="xFile1" name="fileUp1" accept="image/*" onchange="UpImg1()" style="position:absolute;clip:rect(0 0 0 0);"></div><div class="fl"><span class="sfcard">驾驶证正副页(请展开拍照)</span><div class="imgbox" id="UpDriver">点击下面按钮上传图片</div><label for="xFile2" id="lab2" class="lab">上传图片</label><input type="file" id="xFile2" name="fileUp2" accept="image/*" style="position:absolute;clip:rect(0 0 0 0);" onchange="UpImg2()"></div><div class="fl"><span class="sfcard">请上传个人头像</span><div class="imgbox" id="UpHead">点击下面按钮上传图片</div><label for="xFile3" id="lab3" class="lab">上传图片</label><input type="file" id="xFile3" name="fileUp3" accept="image/*" style="position:absolute;clip:rect(0 0 0 0);" onchange="UpImg3()"></div></form> <iframe id="frameFile" name="frameFile" style="display: none;"></iframe><p>上传备注：您提供的身份证、驾驶证照片不得涂改，请确保文字图片，证件号码清晰可见，且证件信息保持一致</p></div>');
    $('#footdiv').css('display', 'none');

    JudgeCardStatus();
}
function UpImg1() {
    //alert(22222);
    $('#hiddenId').val("IDcard");
    form1.submit();
   
}
function UpImg2() {
    $('#hiddenId').val("Driver");
    form1.submit();

}
function UpImg3() {
    $('#hiddenId').val("Head");
    form1.submit();

}
function uploadSuccess(data) {
    var ff = data.substr(data.lastIndexOf());//截取imgurl最后一个字符
    var src = data.substring(0, data.length - 1);//获取不包括最后一个字符的完整路径
    if (ff == "0") {
        $('#UpId').html('<img src="' + src + '" alt="路径有误" />');
    }
    else if (ff == "1") {
        $('#UpDriver').html('<img src="' + src + '" alt="路径有误" />');
    }
    else {
        $('#UpHead').html('<img src="' + src + '" alt="路径有误" />');
    }
    //JudgeCardStatus();

}

function JudgeCardStatus() {
    //判断身份证和驾驶证的审核状态：
    $.post("/PersonalCenter/CardPicStatus", {}, function (data) {
        //alert('审核状态是：  ' + data.IDCardMsg);

        switch (data.IDCardMsg) {
            case "2":
                $('#UpId').html("请上传身份证");
                break;
            case "3":
                $('#UpId').html("身份证审核失败,请重新上传");
                break;
            case "1":
                $('#UpId').html("身份证审核成功");
                $('#xFile1').attr({ "disabled": true });
                $('#lab1').css('backgroundColor', 'gray');
                break;
            case "0":
                $('#UpId').html("身份证正在审核中...");
                $('#xFile1').attr({ "disabled": true });
                $('#lab1').css('backgroundColor', 'gray');
                break;
        }


        switch (data.DriverCardMsg) {
            case "2":
                $('#UpDriver').html("请上传驾驶证");
                break;
            case "3":
                $('#UpDriver').html("驾驶证审核失败,请重新上传");
                break;
            case "1":
                $('#UpDriver').html("驾驶证审核成功");
                $('#xFile2').attr({ "disabled": true });
                $('#lab2').css('backgroundColor', 'gray');
                break;
            case "0":
                $('#UpDriver').html("驾驶证正在审核中...");
                $('#xFile2').attr({ "disabled": true });
                $('#lab2').css('backgroundColor', 'gray');
                break;
        }


        switch (data.DriverCardMsg) {
            case "2":
                $('#UpHead').html("请上传头像");
                break;
            case "3":
                $('#UpHead').html("个人头像审核失败,请重新上传");
                break;
            case "1":
                $('#UpHead').html("个人头像审核成功");
                $('#xFile3').attr({ "disabled": true });
                $('#lab3').css('backgroundColor', 'gray');
                break;
            case "0":
                $('#UpHead').html("个人头像正在审核中...");
                $('#xFile3').attr({ "disabled": true });
                $('#lab3').css('backgroundColor', 'gray');
                break;
        }
    });
   
}
