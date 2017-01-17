//==============YTTimeRentIndex页面的第一个JS文件
var status = "rentlyrent";
time = new Date();
nowday =time.getFullYear() + '-' +(time.getMonth() + 1)+ '-' + time.getDate();//当前日期
var timeparse=Date.parse(time);

$(function () {
    //var time = new Date();
    RentlyRent();

    var picbghour = 0;
    var picbgmin = 0;
    var pickerObj={
        dateFormat: 'yy-m-d',
        minDate:time.getFullYear() + '-' +(time.getMonth() + 1) + '-' + time.getDate(),
        maxDate: 27,
        numberOfMonths: 1,
        //clearText: "清除",
        //closeText: "关闭",
        yearSuffix: '年', 
        monthNames: ['一月', '二月', '三月', '四月', '五月', '六月', '七月', '八月', '九月', '十月', '十一月', '十二月'],
        //dayNames: ['星期日', '星期一', '星期二', '星期三', '星期四', '星期五', '星期六'],
        //dayNamesShort: ['周日', '周一', '周二', '周三', '周四', '周五', '周六'],
        dayNamesMin: ['日', '一', '二', '三', '四', '五', '六']};
         
    var RetpickerSelectObj=Object.create(pickerObj);

    RetpickerSelectObj.onSelect=function (dateText) {
            
        RetDateChange(dateText);    
        RentTimeCount();
    };

    var PicpickerSelectObj=Object.create(pickerObj);

    PicpickerSelectObj.onSelect=function (dateText) {//dateText参数表示用户选中的datepicker的日期

        $("#returnday").datepicker('option', 'minDate', dateText);
        var picbghour = time.getHours();
        var picbgmin = time.getMinutes();
        var datetext = Date.parse(dateText);
        var mindate = $("#pickday").datepicker('option', 'minDate');
        mindate = Date.parse(mindate);

        if (datetext == mindate) {
              
            if (picbgmin > 0) {
                picbghour += 3;
                picbgmin = 0;
            }
        }
        else {
            picbghour = 0;
            picbgmin =0;
        }

        var piclength = (24 * 60 - (picbghour * 60 + picbgmin)) / 30;
        if (piclength <= 0) {
            piclength = (24 * 60 * 2 - (picbghour * 60 + picbgmin)) / 30;
        }
        SPickTime(picbghour, picbgmin, piclength);

        var [pickTime, ReturnTime, ptime,rtime]=GetArguments();
        SetReturnTime(pickTime);//设置还车日期

        RentTimeCount();

    };
         
    $("#returnday").datepicker(RetpickerSelectObj);

    $("#pickday").datepicker(PicpickerSelectObj);

    ProvinceChange(form1.ProvinceName, form1.CityName);

    //CarCount();

    function RetDateChange(dateText){
        var [pickTime2]=GetArguments();
        var datetext = Date.parse(dateText);
        var mindate = $("#returnday").datepicker('option', 'minDate');
        mindate = Date.parse(mindate);

        if (datetext == mindate) {
              
            SetReturnTime(pickTime2);
        }
        else {
            var retbeghour =0;
            var retbegmin =0;
            var retlength = (24 * 60 - (retbeghour * 60 + retbegmin)) / 30;
            if (retlength <= 0) {
                retlength = (24 * 60 * 2 - (retbeghour * 60 + retbegmin)) / 30;
            }
            InitRettime(retbeghour, retbegmin, retlength);

        }
    }

         
});


function PickTimeAddSelf(pickTime){
    var picktime=new Date(pickTime);
    pickdaynow =picktime.getFullYear() + '-' +(picktime.getMonth() + 1)+ '-' + picktime.getDate();//当前日期
    picktime.setDate(picktime.getDate()+1);
    var addday= picktime.getFullYear() + '-' + (picktime.getMonth() + 1) + '-' +picktime.getDate();//在当前日期上加一天的日期
    return [addday,pickdaynow];
};
function RentCarBtns() {
    $('#selectcarbtn').css('backgroundColor', 'gray');
    $('#selectcarbtn').attr({ "disabled": true });
    //alert(status);
    if (status =="rentlyrent") {
        showcarlist();
    }
    else {
        AppointRentCar();
    }
}

function RentlyRent(){
    //var nowday = time.getFullYear() + '-' +(time.getMonth() + 1)+'-' + time.getDate();
    $("#pickday").val(nowday);
    $('#pickday').attr({ "disabled": true });
    SPickTime(time.getHours(), time.getMinutes() + 5, 3);
    SReturnTime();
}
function MapRentlyBtn() {
    status = "rentlyrent";
    SetBtnStyle();
    RentlyRent();
    showcarlist();

}
function AppointRentCar() {
    var [pickTime, ReturnTime, ptime,rtime, PickShop]=GetArguments();
    //alert(PickShop);
    $.post("/YTTimeRent/AppointRentCar", {picktime: pickTime, pickshop: PickShop, returntime: ReturnTime}, function (data) {
        AddCarData(data);
    });

    //-----------修改百度地图可租车辆的值：

    var radio = $('input:radio:checked');
    var Idcount = radio.attr('id').match(/\d+/g);
    var appointcar=$('#lab').html();

    $('#maplabel'+Idcount+'').html(appointcar);

}

function SetBtnStyle() {
    $('.xuanche').attr({ "disabled": true });
    $('.xuanche').css('backgroundColor', 'gray');
    if (status == "rentlyrent") {
        $('#selectcarbtn').val('立即租车');
    }
    else {
        $('#selectcarbtn').val('预约租车');
    }
}


function GetArguments() {
    var pickTime = $("#pickday").val() + " " + $("#pickhour option:selected").html();//获取用户选车的完整时间；
    var ReturnTime = $("#returnday").val() + " " + $("#returnhour option:selected").html();//获取用户还车的完整时间；
    var ptime = Date.parse(pickTime);
    var rtime = Date.parse(ReturnTime);

    var PickShop = form1.PikShopName.value;
    var ReturnShop = form1.RetShopName.value;

    return [pickTime, ReturnTime, ptime,rtime, PickShop, ReturnShop];
}

function AddCarData(data){
    for (var i = 0; i < data.length; i++) {
        //alert(data.length);
        $('#CarMsgdiv').html("<div class='showrentcarDIV'><table border='0' class='showcartab'><tr><td class='td1' id='imgID'><img src="
            + data[i].CarPic + " alt='路径错误'/></td><td class='td2'><span class='span1'>"
            + data[i].CarName + "</span><br/><span class='span2'>" + data[i].SeatSizes + "|</span><span class='span3'>可租车辆：<label id='lab'>" + data[i].Carcount + "</label>辆</span></br><span class='span5'>续航：" + data[i].EnduranceMileage
            + "KM</span></td><td class='td3'><span class='span6' id='cost_hourID'>￥"+ data[i].cost_hour
            + "</span>/小时</br></td><td class='td4'><input type=button value='租车' class='selcarbtn' /><input type='hidden' value=" + data[i].CarClass_Id + "></td><tr><table></div>");
       
        //-------------给每个租车按钮注册RentCar单击事件：
        var rent = $('.selcarbtn');
        rent[i].onclick = function (event) {
            RentCar(event);
        };
    }

}

function showcarlist() {
    //设置按钮禁用和灰选：
    $('#selectcarbtn').attr({ "disabled": true });
    $('#selectcarbtn').css('backgroundColor', 'gray');

    var [pickTime, ReturnTime, ptime,rtime, PickShop, ReturnShop]=GetArguments();

    if (ptime >= rtime) {
        alert("取车时间必须小于还车时间！");
    }
    else {
        var radio = $('input:radio:checked');
        //alert(radio);
        if (radio.attr('id')!==undefined) {
            var Idcount = radio.attr('id').match(/\d+/g);
            var cars=$('#maplabel'+Idcount+'').html();
            if (cars==0) {
                alert("当前车辆已租完");
                return false;
            }
        }

        $.post("/YTTimeRent/getAvailableCarsForRenting", { picktime: pickTime, pickshop: PickShop, returntime: ReturnTime }, function (data) {
            $('#CarMsgdiv').html("");
            if (data.length == 0) {
                alert("当前车辆已租完");
                return false;
            }
            else {
                AddCarData(data);
               
                //==================================设置滚动监听事件============

                var CarMsgoffset = $('#CarMsgdiv').offset().top;

                $(window).scrollTop(CarMsgoffset);//设置窗体的滚动距离；

            }
        });
    }
}

function RentTimeCount() {
    //==========计算租车时长=====

    var [days,hours,minutes]=CountTimes();
    //alert("相差小时"+hours);
    $('#RentTimeCountID').html("");
    $('#RentTimeCountID').html(hours + "小时" + minutes + "分");
    if (days > 0) {
        $('#RentTimeCountID').html(days + "天" + hours + "小时" + minutes + "分");
    }

}

function CountTimes(){
    var [pickTime, ReturnTime]=GetArguments();
    //alert("取车时间"+pictimes+",  还车时间"+ReturnTime);
    pickTime = pickTime.replace(/-/g, '/');
    ReturnTime = ReturnTime.replace(/-/g, '/');

    var pictimes = new Date(pickTime);
    var returntimes = new Date(ReturnTime);
    
    var leftsecond = parseInt(returntimes.getTime() - pictimes.getTime());

    var leave = leftsecond % (12 * 30 * 24 * 3600 * 1000);
    //var months = Math.floor(leave / (30 * 24 * 3600 * 1000));
    //计算出相差天数
    var leave0 = leave % (30 * 24 * 3600 * 1000);
    var days = Math.floor(leave0 / (24 * 3600 * 1000));
    //计算出小时数
    var leave1 = leave0 % (24 * 3600 * 1000);     //计算天数后剩余的毫秒数
    var hours = Math.floor(leave1 / (3600 * 1000));
    //计算相差分钟数
    var leave2 = leave1 % (3600 * 1000);         //计算小时数后剩余的毫秒数
    var minutes = Math.floor(leave2 / (60 * 1000));
    //alert(minutes);
    return [days,hours,minutes];

}

function TransTime(hours, mins, lengths) {
    //hours表示起始小时数，mins表示起始分钟数；
    var Picarry = new Array();//完整的时间；
    var Minarry = new Array();
    var Harry = new Array();
    var time = new Date();
    for (var i = 0; i < lengths; i++) {
        Minarry[i] = mins + i * 30;
        Harry[i] = hours + parseInt(Minarry[i] / 60);
        if (Harry[i] >= 24) {//如果当前小时大于等于24点；
            Harry[i] = parseInt(Harry[i] % 24);//让当前的小时数和24取余；
        }
        Picarry.push(Harry[i] + ":" + Minarry[i] % 60);
        var MHindex = Picarry[i].indexOf(":");//indexOf获取‘：’第一次出现的索引；
        var min = Picarry[i].substring(MHindex + 1);//截取取分钟部分；
        if (Harry[i] < 10) {
            Harry[i] = '0' + Harry[i];
        }
        if (min < 10) {
            min = "0" + min;
        }
        Picarry[i] = Harry[i] + ':' + min;
    }
    return Picarry;
}
function SPickTime(CHour, CMins, Timelength) {
    //alert("取车时间段"+Timelength);
    form1.sel.options.length = 0;
    var transarry = TransTime(CHour, CMins, Timelength);
    for (var i = 0; i < transarry.length; i++) {
        form1.sel.options[i] = new Option(transarry[i], i);
    }
}
//SReturnTime设置还车时间的值
function SReturnTime() {
    CarBtnChange();
    
    var [pickTime, ReturnTime, ptime,rtime]=GetArguments();
    var result= TimeReturnFalse();
    if (result==false) {
        return false;
    }

    var [addday,pickdaynow]= PickTimeAddSelf(pickTime);
    var pictime=new Date(pickTime);
    var piknowtimecount=(ptime-timeparse)/(1000*3600);
    if (piknowtimecount<0) {
        $("#pickday").val(addday);//addday应该是在原先的pickTime上面加一天，而不是只在当前时间加一天
    }
    if (status =="rentlyrent"&&piknowtimecount>0) {
        $("#pickday").val(nowday);
    }
    if (status !="rentlyrent"&&piknowtimecount>0) {
        $("#pickday").val(pickdaynow);
    }
    var [pickTime2]=GetArguments();//取车时间在当前时间上加一天或未改变后，重新获取取车时间，将这个新的取车时间作为还车时间的参照；

    SetReturnTime(pickTime2);//根据取车时间设置还车时间

    RentTimeCount();//计算租车时长；

}
function InitRettime(Bghour, RetBigmin, RetLength) {
    var transarry = TransTime(Bghour, RetBigmin, RetLength);
    form1.sel2.options.length = 0;
    for (var j = 0; j < transarry.length; j++) {
        form1.sel2.options[j] = new Option(transarry[j], j);
    }
}

function TimeReturnFalse(){
    var [days,hours,minutes]=CountTimes();
    RentTimeCount();//计算租车时长；
    var retday=$('#returnday').val();
    var retmindate=$('#returnday').datepicker('option', 'minDate');
    if (status =="rentlyrent") {
        if(hours>=1&&retday!=retmindate){
            return false;
        }
        else {
            return true;
        }
    }
    else {
        if(hours>=2){
            return false;
        }
        else {
            return true;
        }
    }
}


//RetTimeChagne还车时间的改变事件
function RetTimeChagne() {
    CarBtnChange();

    RentTimeCount();
    if ($('.mapdiv') == undefined) {
        return false;
    }
    CarCount();
}


function ProvinceChange(province,city){
    var ProvinceValue=province.value;
    //alert(ProvinceValue);
    city.length = 0;
    
    $.post("/YTTimeRent/SelectCity/", { prince: ProvinceValue }, function (data) {
        for (var i = 0; i < data.length; i++) {
            city.options[i] = new Option(data[i].CName);//将所有的市加载到市下拉框中；
            city.options[i].value = data[i].CId;
            if (data[i] == "南昌市") {
                city.options[i].selected = true;//设置默认南昌市被选中；
            }
        }
        CityChange(form1.CityName,form1.PikShopName,form1.PikShopName)
        //SelectCountry(form1.CitySelectName, form1.CountrySelectName);
    });
}
function CityChange(city,pikshop){
    pikshop.length=0;
    form1.RetShopName.length=0;
    var CityValue=city.value;
    var [pickTime, ReturnTime]=GetArguments();
    var Mapcenter=$("#CityID option:selected").html();

    $.post("/YTTimeRent/GetShopsByCity", { city: CityValue,picktime:pickTime,returntime:ReturnTime}, function (data) {
        InitMAP(data, Mapcenter, pikshop);
    });//post方法结尾；
}


//统计出当前取车门店的可租车辆数量：
function CarCount() {
    var [pickTime, ReturnTime]=GetArguments();

    var radio = $('input:radio:checked');
    var PicShopId = radio.val();
    //正则提取radio的id的数字部分：
    if (!radio.attr('id')) {
        return false;
    }
    var Idcount = radio.attr('id').match(/\d+/g);
    //alert(Idcount+',   ID');
    $.post("/YTTimeRent/CarCounts/", { picshopId: PicShopId, picktime: pickTime, returntime: ReturnTime }, function (data) {
        document.getElementById("kzcl" + Idcount + "").innerHTML = `可租车辆:<label>${data}</label>`

    });
}

//根据返回的坐标加载地图：
function InitMAP(data, Mapcenter, pikshop) {
    //以Mapcenter参数作为地图的中心点；

    //=======根据区名设置地图中心点=============
    var map = new BMap.Map("Mapdiv", { enableMapClick: false });  // 创建Map实例,禁用点击景点弹出景点信息;

    map.centerAndZoom(Mapcenter, 12);      // 初始化地图,用区名设置地图中心点,12表示地图的缩放比例
    map.enableScrollWheelZoom(true);     //开启鼠标滚轮缩放

    var arrys = new Array();

    //alert(data.length);
    for (var i = 0; i < data.length; i++) {
       
        pikshop.options[i] = new Option(data[i].Name);
        pikshop.options[i].value = data[i].Id;
        
        form1.RetShopName.options[i] = new Option(data[i].Name);
        form1.RetShopName.options[i].value =data[i].Id;
        arrys.push(data[i]);
       
        //===========创建点：
        var point = new BMap.Point(data[i].Longitude, data[i].Latitude);//根据坐标创建点位置；

        ////创建自定义图片====
        var myIcon = new BMap.Icon("/Images/web_03.png", new BMap.Size(40, 50));
        var marker2 = new BMap.Marker(point, { icon: myIcon });  // 创建标注
        map.addOverlay(marker2);

        marker2.addEventListener("click", function (e) {
            CarBtnChange();

            var p = e.target;//获取当前点击的点对象；
            for (var j = 0; j < arrys.length; j++) {
                //获取用户点击到的地图上的那个点：
                if (arrys[j].Longitude == p.getPosition().lng && arrys[j].Latitude == p.getPosition().lat) {

                    form1.PikShopName.options[j].selected = true;//设置用户点击的取车门店被选中；
                    //设置用户点击的还车门店被选中；
                    form1.RetShopName.options[j].selected = true;

                    //如果当前点击的这个图标对应的div已经存在，就结束这个方法，取消点击事件，避免重复添加相同的div:
                    if (document.getElementById("mapbox" + j + "")) {
                        document.getElementById("check" + j + "").checked = true;
                        $('.mapdiv').css('display', 'none');//隐藏 
                        document.getElementById("mapbox" + j + "").style.display = "";//显示

                        return false;
                    }
                    // 每点击一次创建一个div元素
                    var div = document.createElement("div");
                   
                    div.innerHTML = "<span class='closemap'>+</span><p id='Pshop" + j + "'></p><p id='Padress" + j + "'></p><p id='Pcarcount" + j + "'><span id='kzcl" + j + "'></span></p><span class='jiantou'></span><input type='radio' name='radioname' value='' id='check" + j + "'/><input type='button' value='预约租车' class='mapbtn yuyuebtn' onclick='AppointCarS()'/><input type='button' value='立即租车' id='rently" + j + "' class='mapbtn rently' onclick='MapRentlyBtn()'/>";
                    // 设置样式:添加类名；
                    div.setAttribute("class", "mapdiv");
                    div.setAttribute("id", "mapbox" + j + "");

                    div.style.width = "360px";
                    div.style.height = "160px";

                    // 将覆盖物的经纬度坐标转为像素坐标，显示在地图上：SetMapPosition
                    SetMapPosition(map, p.point, div);

                    //如果当前门店没有车；
                    if (arrys[j].carcount <= 0) {
                        //div.innerHTML += "<input type='button' value='立即租车' class='mapbtn' onclick='RentCarBtns()'/>";
                        $("#rently" + j + "").attr({ "disabled": true });
                        $("#rently" + j + "").css('backgroundColor', 'gray');
                    }

                    document.getElementById("Pshop" + j + "").innerHTML = "门店：" + arrys[j].Name;
                    document.getElementById("Padress" + j + "").innerHTML = "地址：" + arrys[j].Address;
                   
                    document.getElementById("kzcl" + j + "").innerHTML =`可租车辆：<label id=maplabel${j}> ${arrys[j].Carcount}</label>`;
                    document.getElementById("check" + j + "").value = arrys[j].Id;
                    
                    $('.closemap').click(function () {
                        $('.mapdiv').css('display', 'none');//隐藏 
                    });

                    $('.mapdiv').css('display', 'none');//隐藏 
                    document.getElementById("mapbox" + j + "").style.display = "";//显示   
                    document.getElementById("check" + j + "").checked = true;


                    //zoomend:图更改缩放级别结束时触发触发此事件。 
                    map.addEventListener("zoomend", function () {
                        SetMapPosition(map, p.point, div);
                    });

                    break;
                }
            }
         
        });

        $('#shopcountID').html(data.length);//加载门店数量；
        //==============================================================================
    }
}


//---------- 将覆盖物的经纬度坐标转为像素坐标，显示在地图上：SetMapPosition；
function SetMapPosition(map, point, div) {
    var position = map.pointToOverlayPixel(point);//获取当前点击的这个点的像素坐标；
    //alert(position.x + ",   " + position.y);
    var divleft = position.x - parseInt(div.style.width) / 2 - 5 + "px";;
    var divtop = position.y - parseInt(div.style.height) - 30 + "px";;
    div.style.left = divleft;
    div.style.top = divtop;
    // 将div添加到覆盖物容器中  
    map.getPanes().markerPane.appendChild(div);

}
function ShopChanged() {
    CarBtnChange();
}
function CarBtnChange() {
    $('#CarMsgdiv').html("");

    $('#selectcarbtn').attr({ "disabled": false });
    $('#selectcarbtn').css('backgroundColor', '#74c621');

}
function RentCar(event) {
    var event = event || window.event;
    var rentbtn = event.target;
    var carclass_Id = rentbtn.nextSibling.value;
   
    //------------------调接口----------------------------------------

    var accountId=$('#Account_id').val();
    var [pickTime, ReturnTime, ptime,rtime, PickShop, ReturnShop]=GetArguments();

    var denglu = $("#denglulink").html();
    //alert(CarId + ',   ' + PicCity + ',   ' + RetCity + ',   ' + PicShop + ',   ' + RetShop + ',   ' + Picday + ',   ' + Retday + ',   ' + Pictime + ',   ' + Rettime);
    if (denglu == "登录") {
        alert("请先登录");
        window.location.href = "/Account/Logon";
    }
    else {
        $.post("/YTTimeRent/Authentication", {}, function (data) {
            //身份验证是否通过审核：Authentication
            if (data == 1) {
                $.post("/BasicMsg/OrderState/", {}, function (data) {
                    //alert(data.msg);
                    if (data.msg == "已有未支付订单") {
                        var r=confirm('当前已有订单,去支付？');
                        if (r) {
                            window.open('/PersonalCenter/PersonalDetial?ordernum=' + data.OrderNum + '');
                        }
                        else {
                            return false;
                        }
                    }
                    else if (data.msg == "已有进行中订单") {
                        alert('订单正在进行中！');
                        return false;
                    }
                    else {

                        var btn = $('#selectcarbtn').val();
                        
                        //alert(accountId+',   '+carclass_Id+',   '+PickShop+',   '+ReturnShop+',   '+pickTime+',   '+ReturnTime+',   ');
                        var requestUrl='http://121.40.210.7:8043/Account/RentCar';
                        if (status!='rentlyrent') {
                            requestUrl='http://121.40.210.7:8043/ReservedOrders/AppointCar';

                        }
                        $.post(requestUrl,{AccountId:accountId,CarClassId:carclass_Id,PickShopId:PickShop,ReturnShopId:ReturnShop,PickupTime:pickTime,Returntime:ReturnTime,CouponID:0},function(data){
                            //alert(data.OrderId);
                            //alert(data.isSuccess);
                            //alert(data.Message);
                            if (data.isSuccess) {
                                window.location.href = `/BasicMsg/Index?OrderId=${data.OrderId}`;
                            }
                            else {
                                return false;
                            }
                        });
                    }

                });
            }
            else {
                alert(data);
            }
        });

    }
}
function SelectIMG(Citys, shops) {

    var InputshopS = $('#shopinput').val();
    var citys = Citys.value;
    var countryhtml = $("#CityID option:selected").html();
    var pickTime = $("#pickday").val() + " " + $("#pickhour option:selected").html();//获取用户选车的完整时间；
    var ReturnTime = $("#returnday").val() + " " + $("#returnhour option:selected").html();//获取用户还车的完整时间；

    $.post("/YTTimeRent/SearShopClick", {city: citys, picktime: pickTime, returntime: ReturnTime, inputshop: InputshopS }, function (data) {
        if (data.length == 0) {
            return false;
        }
        shops.length = 0;
        InitMAP(data, countryhtml, shops);
    });
}

//----------------预约租车------------------
function AppointCarS() {
    //alert('点击预约租车按钮---');
    
    $('body').css('overflowY', 'hidden');
    $('.motel').css("display", "block");
    $('.AlertDia').css("display", "block");

}
function Cancle() {
    $('body').css('overflowY', 'auto');
    $('.motel').css("display", "none");
    $('.AlertDia').css("display", "none");
    SetBtnStyle();
    return false;
}

function SureRent() {
    SetBtnStyle();
    status = "appointrent";
    Cancle();
    $('#pickday').attr({ "disabled": false });

    $("#pickday").val(nowday);
    var picbghour = time.getHours();
    var picbgmin = time.getMinutes(); 
    if (picbgmin > 0) {
        picbghour += 3;
        picbgmin = 0;
    }
    var  piclength = (24 * 60 - (picbghour * 60 + picbgmin)) / 30;
    if (piclength<=0) {
        piclength = (24 * 60*2 - (picbghour * 60 + picbgmin)) / 30;
    }
    SPickTime(picbghour,picbgmin,piclength);
    var [pickTime]=GetArguments();
    var [addday]= PickTimeAddSelf(pickTime);

    var pictime=new Date(pickTime);
    if (pictime.getHours() < time.getHours()) {
        $("#pickday").datepicker('option', 'minDate',addday);
    }
    var [pickTime2]=GetArguments();
    SetReturnTime(pickTime2);
   
    RentTimeCount();//计算租车时长；
    AppointRentCar();//查询出所有预约租车的车辆信息；
}

function SetReturnTime(pickTime2){
    //------------根据取车时间的值，设置还车时间是在取车时间后一小时或两小时----------
    //alert('这是传到SetReturnTime方法中的取车时间'+pickTime2);
    var pictime2=new Date(pickTime2);

   
    if (status =="appointrent") {
        pictime2.setHours(pictime2.getHours()+2);
    }
    else {
        pictime2.setHours(pictime2.getHours()+1);
    }
    var retdate=pictime2.getFullYear() + '-' + (pictime2.getMonth() + 1) + '-' +pictime2.getDate();
    //alert(retdate+'SetReturnTime中的retdate');
    $("#returnday").datepicker('option', 'minDate',retdate);
    $("#returnday").val(retdate);

    var retbeghour =pictime2.getHours();
    var retbegmin = pictime2.getMinutes();
    var retlength = (24 * 60 - (retbeghour * 60 + retbegmin)) / 30;
    if (retlength<=0) {
        retlength = (24 * 60*2 - (retbeghour * 60 + retbegmin)) / 30;
    }
    InitRettime(retbeghour, retbegmin, retlength);

}

