using StaySeoul_SS5API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StaySeoul_SS5API.Controllers
{
    public class ApiController : Controller
    {
        SS5Entities ent = new SS5Entities();
        public static List<Cart> cartList = new List<Cart>();
        
        // GET: Api
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult Login(LoginUser user)
        {
            var loginUser = ent.Users.Where(x => x.Username == user.Username && x.Password == user.Password).FirstOrDefault();
            if (loginUser != null)
            {
                LoginUser loggedInUser = new LoginUser();
                loggedInUser.Username = loginUser.Username;
                loggedInUser.FullName = loginUser.FullName;
                loggedInUser.Password = loginUser.Password;
                loggedInUser.ID = loginUser.ID;
                loggedInUser.FamilyCount = loginUser.FamilyCount;
                return Json(loggedInUser, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetServiceTypes()
        {
            List<ServiceTypes> serviceTypeList = new List<ServiceTypes>();
            var seviceType = ent.ServiceTypes.ToList();
            foreach(var s in seviceType) {
                ServiceTypes service = new ServiceTypes();
                service.serviceID = s.ID;
                service.iconName = s.IconName.Substring(4).ToString();
                if(service.iconName == "spoon-knife.png")
                {
                    service.iconName = "spoon.png";
                }
                service.serviceType = s.Name;
                service.description = s.Description;
                serviceTypeList.Add(service);
            }
            return Json(serviceTypeList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetService(Services s)
        {
            var serviceList = ent.Services.Where(x=> x.ServiceTypeID == s.ServiceTypeID).ToList();
            if (serviceList != null)
            {
                List<Services> servicesList = new List<Services>();
                foreach (var ser in serviceList) {
                    
                    Services services = new Services();
                    services.Name = ser.Name;
                    services.Description = ser.Description;
                    services.Price = ser.Price;
                    services.DailyCap = ser.DailyCap;
                    services.BookingCap = ser.BookingCap;
                    services.DayOfWeek = ser.DayOfWeek;
                    if(string.IsNullOrEmpty(ser.DayOfWeek) == true)
                    {
                        services.DayOfWeek = "0";
                    }
                    services.DayOfMonth = ser.DayOfMonth;
                    if(string.IsNullOrEmpty(ser.DayOfMonth) == true)
                    {
                        services.DayOfMonth = "0";
                    }
                    services.Duration = (long)ser.Duration;
                    services.ServiceTypeID = ser.ServiceTypeID;
                    services.ID = ser.ID;
                    servicesList.Add(services);
                }
                return Json(servicesList, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }
        }

        public JsonResult GetSpotInfo(ServiceSpot s)
        {
            var dailyCap = ent.Services.Where(x => x.ID == s.serviceID).FirstOrDefault();
            var booked = ent.AddonServiceDetails.Where(x=> x.ServiceID == s.serviceID && x.FromDate == s.date.Date);
            long noOfPeople = 0;
            foreach(var b in booked)
            {
                noOfPeople += b.NumberOfPeople;
            }
            long totalnumber = (long)(dailyCap.DailyCap * dailyCap.BookingCap);
            long availSpots = (totalnumber-noOfPeople);
            
            return Json(availSpots, JsonRequestBehavior.AllowGet);
        }
        public JsonResult StoreToCart(Cart c)
        {
            var alreadyExists = cartList.Where(x=> x.serviceID == c.serviceID && x.UID == c.UID && x.fromDate == c.fromDate).FirstOrDefault();
            if (alreadyExists != null)
            {
                cartList.Remove(alreadyExists);
            }
            c.cartID = cartList.Count() + 1;
            c.guid = Guid.NewGuid();
            cartList.Add(c);
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCartService(Cart c)
        {
            var data = cartList.Where(x=> x.UID == c.UID).ToList();
            List<Cart> pList = data.ToList();
            return Json(pList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RemoveFromCart(Cart c)
        {
            var removed = cartList.Where(x => x.cartID == c.cartID).FirstOrDefault();
            if(removed != null)
            {
                cartList.Remove(removed);
            }
          return Json(true,JsonRequestBehavior.AllowGet);
        }
        public JsonResult Discount(ServiceDiscount s)
        {
            var discount = ent.Coupons.Where(x => x.CouponCode == s.couponCode).FirstOrDefault();
            if(discount != null)
            {
                decimal total = s.total;
                decimal d = discount.DiscountPercent;
                if((total*d)> discount.MaximimDiscountAmount)
                {
                    total = total - discount.MaximimDiscountAmount;
                }
                else
                {
                    total = total - (total*d);
                }
                ServiceDiscount disc = new ServiceDiscount();
                disc.total = total;
                return Json(disc,JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
       
        }
        public JsonResult Checkout(List<Cart> c)
        {
            try
            {
                string couponcode = c.Select(x => x.couponCode).FirstOrDefault();
                var couponID = ent.Coupons.Where(x => x.CouponCode == couponcode).FirstOrDefault();
                foreach (var list in cartList)
                {
                    foreach (var cart in c)
                    {
                        if (list.cartID == cart.cartID)
                        {
                            cartList.Remove(cart);
                        }
                    }
                }
                AddonService addonService = new AddonService();
                addonService.GUID = Guid.NewGuid();
                addonService.ID = ent.AddonServices.Count() + 1;
                addonService.UserID = c.Select(x=> x.UID).FirstOrDefault();
                if (couponID != null)
                {
                    addonService.CouponID = couponID.ID;
                }
                ent.AddonServices.Add(addonService);
                ent.SaveChanges();
                foreach (var cart in c)
                {
                    AddonServiceDetail addonServiceDetail = new AddonServiceDetail();
                    addonServiceDetail.Price = cart.payableAmount;
                    addonServiceDetail.AddonServiceID = addonService.ID;
                    addonServiceDetail.GUID = cart.guid;
                    addonServiceDetail.ID = ent.AddonServiceDetails.Count() + 1;
                    addonServiceDetail.ServiceID = cart.serviceID;
                    addonServiceDetail.Notes = cart.addOnNotes;
                    addonServiceDetail.FromDate = cart.fromDate;
                    addonServiceDetail.isRefund = false;
                    addonServiceDetail.NumberOfPeople = cart.noOfPPl;
                    ent.AddonServiceDetails.Add(addonServiceDetail);
                    ent.SaveChanges();
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }


    }
}