using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YTTimeRentWeb.Models;
using YTTimeRentWeb.utils;
using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using YTTimeRentWeb.Common;

namespace YTTimeRentWeb.Controllers
{
    public class YTTimeRentController : Controller
    {
        TimeRentEntities entity = new TimeRentEntities();
        public ActionResult Index()
        {

            HashSet<string> provinceIdList = getExitCityProvince();
            List<object> ProvinceObj = new List<object>();
            foreach (var item in provinceIdList)
            {
                long proId = Convert.ToInt64(item);
                string ProvinceName = entity.Provinces.SingleOrDefault(p => p.Id == proId).areaname;
                ProvinceObj.Add(new ProvinceModel() { PId = proId, PName = ProvinceName });
            }
            ViewData.Model = ProvinceObj;
            return View();
            
        }

        //getExitCityProvince查询出所有存在门店的省和市
        public HashSet<string> getExitCityProvince()//
        {
            var countryObj = from P in entity.Provinces join S in entity.Shops on P.Id equals S.Country select P;
            var countrylist = countryObj.ToList();//查询出所有存在门店的区；
            List<long> CountryId = new List<long>();
            //根据所有的区，查询出所有的市Id:
            List<string> cityId = new List<string>();
            foreach (var country in countrylist)
            {
                CountryId.Add(country.Id);
                cityId.Add(country.parentno);
            }
            //去除CountryId集合中重复的市：
            HashSet<long> hsCountryId = new HashSet<long>(CountryId);
            //去除citys集合中重复的市：
            HashSet<string> hsCityId = new HashSet<string>(cityId);
            //根据市Id查询出所有有门店的省：
            List<string> provinceId = new List<string>();
            foreach (var city in hsCityId)
            {
                long hscity = Convert.ToInt64(city);
                var city_parentno = entity.Provinces.SingleOrDefault(c => c.Id == hscity);
                if (city_parentno.parentno == "0")
                {
                    city_parentno.parentno = city;
                }
                provinceId.Add(city_parentno.parentno);
            }
            HashSet<string> hsProvinceId = new HashSet<string>(provinceId);
            Session["HScountryId"] = hsCountryId;
            Session["HScityId"] = hsCityId;
            Session["HSprovinceId"] = hsProvinceId;
            return hsProvinceId;
        }

        public JsonResult SelectCity(string prince)
        {
            List<object> NewCity = new List<object>();
            //根据用户选中的省Id,查询出对应的所有市：
            if (Session["HScityId"] == null)
            {
                return Json(false);
            }
            var listcityId = (IEnumerable)Session["HScityId"];//获取所有门店的市Id；
            foreach (var city in listcityId)
            {
                long cityId = Convert.ToInt64(city);
                var cityObj = entity.Provinces.SingleOrDefault(c => c.Id == cityId);
                if (cityId == Convert.ToInt64(prince))
                {
                    NewCity.Add(new CityModel() { CId = cityId, CName = cityObj.areaname });
                    break;
                }
                if (cityObj.parentno == prince)
                {
                    NewCity.Add(new CityModel() { CId = cityId, CName = cityObj.areaname });
                }
            }
            return Json(NewCity);
        }

        public JsonResult SelectCountry(string city)//city表示用户选中城市的Id
        {
            List<CountryModel> country = GetAllCountryByCity(city);
            return Json(country);
        }

        public List<CountryModel> GetAllCountryByCity(string city)
        {
            List<CountryModel> city_contrylist = new List<CountryModel>(); //city_contrylist存放当前city市下的所有的区；

            var ListCountry = (IEnumerable)Session["HScountryId"];
            foreach (var country in ListCountry)
            {
                long cuntryId = Convert.ToInt64(country);
                //查询出当前循环到的区的parentno：
                var countryObj = entity.Provinces.SingleOrDefault(c => c.Id == cuntryId);

                if (countryObj.parentno == city)
                {
                    city_contrylist.Add(new CountryModel() { CtId = cuntryId, CtName = countryObj.areaname });
                }
            }
            return city_contrylist;
        }
        public JsonResult GetShopsByCity(string city, string picktime, string returntime)
        {
            var city_shop = GetAllShopsByCity(city, picktime, returntime);
            return Json(city_shop);

        }
        public List<ShopModel> GetAllShopsByCity(string city, string picktime, string returntime)
        {
            List<ShopModel> city_shoplist = new List<ShopModel>();
            long cityId = Convert.ToInt64(city);
            IQueryable<Provinces_Shops_View> shops = entity.Provinces_Shops_View.Where(s => s.Province_CityId == cityId);
            foreach (var shop in shops)
            {
                int carcount = AvialbeCarcount(shop.Id, picktime, returntime);
                city_shoplist.Add(new ShopModel()
                {
                    Id = shop.Id,
                    Name = shop.Name,
                    Address = shop.Address,
                    Longitude = shop.Longitude,
                    Latitude = shop.Latitude,
                    Carcount = carcount
                });
            }
            return city_shoplist;
        }


        public JsonResult SelectShops(string country, string picktime, string returntime)//根据区，选择所有门店；
        {

            var Country_Id = Convert.ToInt64(country);
            List<Object> shops = GetShopsByCountryId(Country_Id, picktime, returntime);
            return Json(shops);
        }


        //传入picktime和returntime是为了获取可租车辆的数量,跟门店无关；
        public List<Object> GetShopsByCountryId(long Country_Id, string picktime, string returntime)
        {
            var shoplist = from S in entity.Shops join P in entity.Provinces on S.Country equals P.Id where P.Id == Country_Id select S;

            List<Object> shops = new List<Object>();

            if (shoplist != null)
            {
                foreach (var s in shoplist)
                {
                    //根据门店Id,查询出当前门店下所有可租车辆的数量：
                    var Carcount = AvialbeCarcount(s.Id, picktime, returntime);

                    var shopmap = new
                    {
                        s.Name,//门店名称
                        s.Longitude,//经度
                        s.Latitude,//纬度
                        s.Address,//门店地址
                        Carcount,//当前门店中可租车的数量
                        s.Id//取车门店Id
                    };
                    shops.Add(shopmap);
                }
            }
            return shops;
        }

        //public JsonResult RentShops(string city)
        //{
        //    List<ShopModel> city_shoplist = GetAllShopsByCity(city);
        //    return Json(city_shoplist);
        //}

        //CarCounts统计用户选中的取车门店的所有的可租车辆的数量：
        public JsonResult CarCounts(long picshopId, string picktime, string returntime)//picshop:用户选中的取车门店；
        {
            int carcount = AvialbeCarcount(picshopId, picktime, returntime);
            return Json(carcount);
        }
        public int AvialbeCarcount(long picshopId, string picktime, string returntime)
        {
            DateTime startTime = Convert.ToDateTime(picktime);
            DateTime endTime = Convert.ToDateTime(returntime);

            //根据门店Id,查询出当前门店下所有可租车辆的数量：
            var cars = from C in entity.Cars where C.Shop_Id == picshopId && C.Status == 1 select C;
            var carlist = cars.ToList();

            foreach (var caritem in carlist)
            {
                var Reservedcar = entity.Cars_Reserved.Where(a => a.Car_Id == caritem.Id && (a.Status == 1 || a.Status == 2) && (!(startTime > a.Appoint_etime || endTime < a.Appoint_stime))).ToList();
                if (Reservedcar.Count != 0)//如果当前的这辆车被预约啦
                {
                    carlist.Remove(caritem);//就从查询到的可租车辆集合中移除这辆车
                    break;
                }
            }
            return carlist.Count;

        }

        public JsonResult Authentication()
        {
            string strid = Session["IDS"].ToString();//获取登录用户的的账户编号；
            long AId = Convert.ToInt64(strid);
            var stat = entity.User.SingleOrDefault(u => u.Account_Id == AId).Status;
            //0:待审核1:通过审核2:未上传 3：审核失败
            if (stat == 1)//通过审核
            {
                return Json(1);
            }
            else if (stat == 0)
            {
                return Json("身份信息待审核");
            }
            else if (stat == 2)
            {
                return Json("身份信息未上传");
            }
            else
            {
                return Json("身份信息审核失败");
            }
        }

        public JsonResult SearShopClick(string city, string picktime, string returntime, string inputshop)
        {
            //直接从数据库中查询出所有包含inputshop字符的所有门店名称；
            long cityid=Convert.ToInt64(city);
            List<Object> searchshops = new List<Object>();
            var search_shop = entity.Provinces_Shops_View.Where(s => s.Province_CityId == cityid && s.Name.Contains(inputshop));
            foreach (var item in search_shop)
            {
                int Carcount = AvialbeCarcount(item.Id, picktime, returntime);
                var shopmap = new
                {
                    item.Name,//门店名称
                    item.Longitude,//经度
                    item.Latitude,//纬度
                    item.Address,//门店地址
                    Carcount,//当前门店中可租车的数量
                    item.Id//取车门店Id
                };
                searchshops.Add(shopmap);
            }

            ////模糊查询：查询出当前city中包含inputshop字符的所有门店名称；
            
            return Json(searchshops);
        }

        #region 取消的插入订单方法
        //public JsonResult InsertOrders(string btnname, string carid, string piccity, string retcity, string picshop, string retshop, string pickday, string retday, string pictime, string rettime)
        //{
        //    BasicMsgController bm = new BasicMsgController();
        //    string strid;
        //    int result;
        //    try
        //    {
        //        strid = Session["IDS"].ToString();//获取登录用户的的账户编号；
        //    }
        //    catch (Exception)
        //    {
        //        return Json(false);
        //    }

        //    using (TransactionScope transaction = new TransactionScope())
        //    {
        //        long AId = Convert.ToInt64(strid);

        //        long Carid = Convert.ToInt64(carid);
        //        Order orders = new Order();
        //        orders.CreatTime = DateTime.Now;
        //        orders.Account_Id = AId;
        //        orders.Shop_Id = entity.Cars.SingleOrDefault(c => c.Id == Carid).Shop_Id;
        //        orders.Account_Id = AId;
        //        orders.CarClass_Id = entity.Cars.SingleOrDefault(a => a.Id == Carid).CarClass_Id;
        //        orders.Car_Id = Carid;
        //        orders.OrderCategory = 1;//即时订单；
        //        var STime = DateTime.Parse(pickday + " " + pictime);
        //        var ETime = DateTime.Parse(retday + " " + rettime);
        //        orders.StartTime = STime;
        //        orders.EndTime = ETime;
        //        orders.PickupShop_Id = entity.Shops.SingleOrDefault(s => s.Name == picshop).Id;
        //        orders.ReturnShop_Id = entity.Shops.SingleOrDefault(s => s.Name == retshop).Id;
        //        orders.OrderNum = "";
        //        orders.OrderStatus = 1;//预订单；
        //        orders.PayStatus = 0;//未支付；

        //        var carclass = entity.Car_Class.SingleOrDefault(d => d.Id == orders.CarClass_Id);
        //        ComputeFees cf = new ComputeFees(STime, ETime, carclass.Code);
        //        orders.BudgetCost = Math.Round(cf.getRentCarFee(), 2);//保留2位小数；

        //        orders.OrderDeposit = bm.UdepositCount(AId);//OrderDeposit表示用户每一条订单所需要交的订单押金；

        //        entity.Order.Add(orders);//将数据添加到Order表中；
        //        entity.SaveChanges();
        //        orders.OrderNum = bm.GeneratingUniqueNumber("E", orders.Id.ToString());


        //        User users = new User();
        //        var username = entity.User.SingleOrDefault(u => u.Account_Id == AId).Name;

        //        Cars car = entity.Cars.SingleOrDefault(c => c.Id == Carid);
        //        if (btnname == "立即租车")
        //        {
        //            car.Status = 2;
        //        }

        //        Cars_Reserved carre = new Cars_Reserved();
        //        carre.Car_Id = Carid;
        //        carre.Appoint_stime = STime;
        //        carre.Appoint_etime = ETime;
        //        carre.Status = 2;

        //        InsuranceSeriel insurance = new InsuranceSeriel();//保险流水表
        //        insurance.AccountId = AId;
        //        insurance.OrderId = orders.Id;
        //        insurance.Deposit = Math.Round(cf.getInsuranceFee(), 2);
        //        insurance.CreatTime = orders.CreatTime;

        //        Pickup_Return picret = new Pickup_Return();
        //        picret.Order_Id = orders.Id;
        //        picret.PickupShop_Id = orders.PickupShop_Id;
        //        picret.ReturnShop_Id = orders.ReturnShop_Id;

        //        entity.Cars_Reserved.Add(carre);
        //        entity.InsuranceSeriel.Add(insurance);//保险流水表；
        //        entity.Pickup_Return.Add(picret);

        //        result = entity.SaveChanges();//保存数据修改到数据库； 
        //        transaction.Complete();//事务回滚
        //    }
        //    if (result > 0)
        //    {
        //        return Json(true);
        //    }
        //    else
        //    {
        //        return Json(false);
        //    }
        //}

        #endregion


        /// <summary>
        /// 根据订单号生成流水号
        /// </summary>
        /// <param name="orderid"></param>
        /// <returns></returns>
        //public string OrderNumber(string orderid)
        //{
        //    string OrderNum;
        //    var LastOrder = entity.Order.ToList().LastOrDefault();
        //    if (LastOrder == null)         //当Order表中没有任何数据
        //    {
        //        OrderNum = "E" + DateTime.Now.ToString("yyyyMMdd") + "00000";
        //    }
        //    else
        //    {
        //        int time1 = LastOrder.CreatTime.DayOfYear;
        //        long num = DateTime.Now.DayOfYear > time1 ? 0 : long.Parse(LastOrder.OrderNum.Substring(9, 5)) + 1;
        //        //string last = tr.Order.Select(d => d.E_Signature).ToList().LastOrDefault();
        //        //string LastOrderNum = string.IsNullOrEmpty(last) ? "0" : last;
        //        OrderNum = "E" + DateTime.Now.ToString("yyyyMMdd") + num.ToString("00000");
        //    }
        //    return OrderNum;
        //}

        //功能：根据取车门店，取车时间，换车门店，换车时间来查询可用车辆
        //更新数据表：车辆预定表，订单表
        //辅助表：车辆表，车型表，单天租车费用表，单位租车费用表，节假日表
        /// <summary>
        /// 根据取车时间，门店，还车时间，门店来获取所有可用车辆信息
        /// </summary>
        /// <param name="pickUpShopName">取车门店</param>
        /// <param name="pickUpTime">取车时间</param>
        /// <param name="returnshop">换车门店</param>
        /// <param name="returnTime">换车时间</param>
        /// <param name="daytype">是否节假日（只用于判断法定节假日，不判断工作日和双休日）</param>
        /// <returns>返回json数据</returns>
        /// json格式：
        /// {
        ///                CarName: "奥迪A2"
        ///                CarPic: "img\Car\2016051117544744.jpg"
        ///                DayPrice: 0
        ///                EnduranceMileage: 400
        ///                MonthPrice: 0
        ///                SeatSize: 2
        /// }
        struct car_info
        {
            public string CarPic,//照片路径
             CarName,//车型名称
             EnduranceMileage,//续航
             SeatSizes,//几厢
             cost_hour,//小时租金
             cost_total,//总共租金
             Battery,//车的电量；
             Carcount,//每一种车型下可组车的数目
             AT_MT,//手动自动档
             CarClass_Id;//车型Id

        };
        /// <summary>
        /// HttpPost方法从后台POST提交数据到接口
        /// </summary>
        /// <param name="Url">接口地址</param>
        /// <param name="postDataStr">提交给接口的参数</param>
        /// <returns>接口返回参数</returns>
        public static string HttpPost(string Url, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postDataStr.Length;
            StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.ASCII);
            writer.Write(postDataStr);
            writer.Flush();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string encoding = response.ContentEncoding;
            if (encoding == null || encoding.Length < 1)
            {
                encoding = "UTF-8"; //默认编码  
            }
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
            string retString = reader.ReadToEnd();
            return retString;
        }  

        public JsonResult getAvailableCarsForRenting(string pickshop, string picktime, string returntime)
        {
            long shopId = Convert.ToInt64(pickshop);
            object Aid = Session["IDS"];
            if (Aid==null)
            {
                Aid = 0;
            }
            string aid = Aid.ToString();
            DateTime startTime = Convert.ToDateTime(picktime);
            DateTime endTime = Convert.ToDateTime(returntime);
            int carcount = AvialbeCarcount(shopId, picktime, returntime);
            //查询出当前取车门店下的所有车型：
            var CarClass = from C in entity.Cars
                           join S in entity.Shops on C.Shop_Id equals S.Id
                           join P in entity.Car_Class on C.CarClass_Id equals P.Id
                           where S.Id == shopId && C.Status == 1
                           group P by P.Id;
            //select P;//按照车型Id进行分组；

            List<Object> list = new List<Object>();
            foreach (var carclass in CarClass)//遍历每一组车型:CarClass表示所有的车型；
            {
                var car_class = carclass.ToList()[0];
                //=============================后台调用接口==============================================================================
                string carclassId = car_class.Id.ToString();
               
                string url = "http://121.40.210.7:8043/Account/GetBudgetData";
                string data = "CarClassId=" + carclassId + "&PickupTime=" + picktime + "&Returntime=" + returntime + "&AccountId=" + aid + "&ShopID=" + pickshop + "&CouponID=0";
                string str = HttpPost(url, data);
                Root root = JsonConvert.DeserializeObject<Root>(str);
                string hour_cost = root.list.Cost.ToString();


                string carimg = "";
                ComputeFees cf = new ComputeFees(startTime, endTime, car_class.Code);
                carimg = car_class.CarPic.Replace("\\", "/");
                car_info cars = new car_info
                {
                    CarPic = "http://121.40.210.7:8043/" + carimg,//照片路径
                    CarName = car_class.CarName,//车型名称
                    EnduranceMileage = car_class.EnduranceMileage.ToString(),//续航
                    SeatSizes = car_class.SeatSize.ToString(),//几厢
                    //cost_total =result.Cost_total, //cf.getActualRentCarFee().ToString(),
                    cost_hour = hour_cost,//cf.getSinglePrice().ToString(),
                    AT_MT = car_class.AT_MT,
                    Carcount = carcount.ToString(),
                    CarClass_Id = car_class.Id.ToString()
                };
                list.Add(cars);
            }

            return Json(list.ToList());
        }

        public JsonResult AppointRentCar(string pickshop, string picktime, string returntime)
        {
            //查询出当前门店下，当前时间段下，所有没有被预约的车辆信息：
            long shopid = Convert.ToInt64(pickshop);
            DateTime startTime = Convert.ToDateTime(picktime);
            DateTime endTime = Convert.ToDateTime(returntime);
            //1,查询出当前门店下的所有车辆：
            var cars = from C in entity.Cars join S in entity.Shops on C.Shop_Id equals S.Id where S.Id == shopid select C;
            var carlist = cars.ToList();
            List<Cars> NoReservedcar = new List<Cars>();
            foreach (var car in cars)
            {
                //查询出所有没有被预约，可租的所有车辆；
                var noreservedcar = entity.Cars_Reserved.Where(r => r.Car_Id == car.Id && (r.Status == 0 || r.Status == 3) && ((startTime > r.Appoint_etime || endTime < r.Appoint_stime))).ToList();
                if (noreservedcar.Count > 0)
                {
                    NoReservedcar.Add(car);
                }
            }
            var GroupCar = NoReservedcar.GroupBy(c => c.CarClass_Id);
            List<Object> list = new List<Object>();
            foreach (var item in GroupCar)
            {
                var gcar = item.ToList()[0];

                var carclass = entity.Car_Class.SingleOrDefault(c => c.Id == gcar.CarClass_Id);

                string carimg = "";
                ComputeFees cf = new ComputeFees(startTime, endTime, carclass.Code);
                carimg = carclass.CarPic.Replace("\\", "/");
                car_info carinfo = new car_info
                {
                    CarPic = "http://121.40.210.7:8023/" + carimg,//照片路径
                    CarName = carclass.CarName,//车型名称
                    EnduranceMileage = carclass.EnduranceMileage.ToString(),//续航
                    SeatSizes = carclass.SeatSize.ToString(),//几厢
                    cost_total = cf.getActualRentCarFee().ToString(),
                    cost_hour = cf.getSinglePrice().ToString(),
                    AT_MT = carclass.AT_MT,
                    //Carcount = carclass.Count().ToString()//每种车型下可租车的数目
                    Carcount = item.Count().ToString(),
                    CarClass_Id = carclass.Id.ToString()
                };
                list.Add(carinfo);
            }
            return Json(list);
        }
    }
}
