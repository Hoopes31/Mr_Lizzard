using System;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using scaffold.Models;
using DbConnection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace scaffold.Controllers
{
    public class AuctionController : Controller
    {
        private readonly AuctionFactory auctionFactory;
        private readonly UserFactory userFactory;
        public AuctionController(AuctionFactory connectFactory, UserFactory connectUser)
        {
            auctionFactory = connectFactory;
            userFactory = connectUser;
        }

        [HttpGet]
        [Route("auctions")]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("id") == null)
            {
                return RedirectToAction("Index", "Home");
            }
            int id = (int)HttpContext.Session.GetInt32("id");
            var profile = userFactory.FindById(id);
            var listings = auctionFactory.GetAll();
            CheckDebts(listings);

            foreach (var time in listings)
            {
                System.TimeSpan timer = time.end_date - DateTime.Now;
                time.timer = Math.Floor(timer.TotalDays);
            }
            if (profile != null)
            {
                ViewBag.users_id = id;
                ViewBag.balance = profile.balance;
                ViewBag.listings = listings;
            }
            else
            {
                RedirectToAction("Index", "Home");
            }

            var auctions = auctionFactory.GetAll();
            if (auctions != null)
            {
                ViewBag.Auctions = auctions;
            }
            return View();
        }

        [HttpGet]
        [Route("new_listing")]
        public IActionResult NewListing()
        {
            if (HttpContext.Session.GetInt32("id") == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [Route("add_listing")]
        public IActionResult AddListing(AuctionView model)
        {
            if (HttpContext.Session.GetInt32("id") == null)
            {
                return RedirectToAction("Index", "Home");
            }

            int user_id = (int)HttpContext.Session.GetInt32("id");
            if (ModelState.IsValid)
            {
                DateTime date = DateTime.Now;
                AuctionItem item = new AuctionItem
                {
                    product_name = model.product_name,
                    description = model.description,
                    starting_bid = model.starting_bid,
                    end_date = model.end_date,
                    created_date = date,
                };
                auctionFactory.AddListing(user_id, item);
                auctionFactory.AddBid(user_id, model.starting_bid);

                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpGet]
        [Route("show_listing")]
        public IActionResult ShowListing(int item_id, double time, string seller)
        {
            if (HttpContext.Session.GetInt32("id") == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var bids = auctionFactory.GetBid(item_id);
            ViewBag.form = new BidView(); 
            ViewBag.seller = seller;
            ViewBag.timer = time;
            ViewBag.bids = bids;
            ViewBag.listing_id = item_id;
    
            return View();
        }
        [HttpGet]
        [Route("delete_listing")]
        public IActionResult DeleteListing(int listing_id)
        {
            if (HttpContext.Session.GetInt32("id") == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (ModelState.IsValid)
            {
                auctionFactory.DeleteListing(listing_id);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("place_bid")]
        public IActionResult PlaceBid(BidView model)
        {
            if (HttpContext.Session.GetInt32("id") == null)
            {
                return RedirectToAction("Index", "Home");
            }

            int id = (int)HttpContext.Session.GetInt32("id");
            User user = userFactory.FindById(id);
            if (ModelState.IsValid)
            {
                if (model.amount <= model.current_bid)
                {
                    ModelState.AddModelError("amount", "Bid must be greater than current bid.");
                    return View(model);
                }
                if (model.amount > user.balance)
                {
                    ModelState.AddModelError("amount", "Bid must be less than your spending balance.");
                    return View(model);
                }
                else
                {
                    auctionFactory.UpdateBid(id, model.listing_id, model.amount);
                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }
        public void CheckDebts(dynamic listings)
        {
            foreach (var list in listings)
            {
                if (list is AuctionItem)
                {
                    if (list.end_date < DateTime.Now)
                    {
                        User seller = userFactory.FindById(list.users_id);
                        BidInfo bid = auctionFactory.GetBid(list.listing_id);
                        User buyer = userFactory.FindById(bid.users_id);

                        float sellPrice = bid.starting_bid;
                        float buyerBalance = buyer.balance - sellPrice;
                        float sellerBalance = seller.balance + sellPrice;
                        auctionFactory.AddFunds(buyerBalance, seller.id);
                        auctionFactory.ReduceFunds(sellerBalance, buyer.id);
                    }
                }
            }
        }
    }
}


    // Session:
    // HttpContext.Session.SetString("KeyName", "Value");
    // string sessionStr = HttpContext.Session.GetString("KeyName");

    // HttpContext.Session.SetInt32("KeyName", Int);
    // int? sessionInt = HttpContext.Session.GetInt32("KeyName");

    // JSON:
    // session.SetString(key, JsonConvert.SerializeObject(value);
    // string jsonValue = session.GetString(key)
    // JsonConvert.DeserializeObject<T>(value);
