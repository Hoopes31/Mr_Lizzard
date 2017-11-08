using System.Collections.Generic;
using System;
using System.Data;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System.Linq;
using Dapper;
using scaffold.Models;

namespace DbConnection
{
    public class AuctionFactory
    {
        private readonly IOptions<MySqlOptions> MySqlConfig;
        public AuctionFactory(IOptions<MySqlOptions> config)
        {
            MySqlConfig = config;
        }
        internal IDbConnection Connection {
            get {
                // Create the IDbConnection for this instances use
                return new MySqlConnection(MySqlConfig.Value.ConnectionString);
            }
        }

        public IEnumerable<AuctionItem> GetAll()
        {
            using (IDbConnection dbConnection = Connection)
            {
                string query = "SELECT * FROM listings JOIN users ON listings.users_id WHERE users.id = listings.users_id";
                dbConnection.Open();
                var auctionItem = dbConnection.Query<AuctionItem>(query).ToList().OrderBy(list => list.end_date);
                return auctionItem;
            };
        }
        public AuctionItem GetListing(int item_id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                string query = $"SELECT * FROM listings JOIN users ON listings.users_id WHERE listing_id = {item_id}";
                dbConnection.Open();
                AuctionItem item = dbConnection.Query<AuctionItem>(query).SingleOrDefault();
                return item;
            }
        }
        public void AddBid(int user_id, float bid)
        {
            using (IDbConnection dbConnection = Connection)
            {
                string query = $"INSERT INTO winning_bids (users_id, listings_id) VALUES ({user_id}, LAST_INSERT_ID())";
                dbConnection.Open();
                dbConnection.Execute(query);
                dbConnection.Close();
            }
        }
        public BidInfo GetBid(int item_id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                string query =  "SELECT * FROM winning_bids " +
                                $"WHERE winning_bids.listings_id = {item_id}";
                dbConnection.Open();
                BidInfo bid = dbConnection.Query<BidInfo>(query).SingleOrDefault();

                query = $"SELECT * FROM listings WHERE listing_id = {item_id}";
                var listing = dbConnection.Query(query).SingleOrDefault();

                query = $"SELECT * FROM users WHERE users.id = {listing.users_id}";
                var user = dbConnection.Query(query).SingleOrDefault();

                bid.first_name = user.first_name;
                bid.description =listing.description;
                bid.product_name = listing.product_name;
                bid.starting_bid = listing.starting_bid;
                return bid;
            }
        }
        public void UpdateBid(int user_id, int listing_id, float bid)
        {
            using (IDbConnection dbConnection = Connection)
            {
                string query = $"UPDATE winning_bids SET winning_bids.users_id = {user_id} WHERE winning_bids.listings_id = {listing_id}";
                dbConnection.Open();
                dbConnection.Execute(query);
                query = $"UPDATE listings SET starting_bid = {bid} WHERE listings.listing_id = {listing_id}";
                dbConnection.Execute(query);
            }
        }

        public void AddListing (int id, AuctionItem item)
        {
            using (IDbConnection dbConnection = Connection)
            {
                string query = $"INSERT INTO listings (users_id, product_name, description, starting_bid, end_date, created_date, updated_date) " + 
                               $"VALUES ({id}, @product_name, @description, @starting_bid, @end_date, NOW(), NOW())";
                dbConnection.Open();
                dbConnection.Execute(query, item);
                dbConnection.Close();
            }
        }
        public void DeleteListing (int item_id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                string query = $"DELETE FROM listings WHERE listings.listing_id = {item_id}";
                dbConnection.Open();
                dbConnection.Execute(query);
                dbConnection.Close();
            }
        }
        public void ReduceFunds(float newBalance, int buyer_id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                string query = $"UPDATE users SET balance = {newBalance} WHERE users.id = {buyer_id}";
            }
        }
        public void AddFunds(float newBalance, int seller_id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                string query = $"UPDATE users SET balance = {newBalance} WHERE users.id = {seller_id}";
            }
        }
    }
}
