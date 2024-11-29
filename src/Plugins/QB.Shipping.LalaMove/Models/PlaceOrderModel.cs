using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace QB.Shipping.LalaMove.Models
{
    public class PlaceOrderModel
    {
        [JsonProperty("scheduleAt")]
        public string ScheduleAT { get; set; }
        [JsonProperty("serviceType")]
        public string ServiceType { get; set; }
        [JsonProperty("specialRequests")]
        public List<string> SpecialRequests { get; set; } = new List<string>();
        [JsonProperty("stops")]
        public List<Waypoint> Stops { get; set; } = new List<Waypoint>();
        [JsonProperty("requesterContact")]
        public Contact RequesterContact { get; set; } = new Contact();
        [JsonProperty("deliveries")]
        public List<DeliveryInfo> Deliveries { get; set; } = new List<DeliveryInfo>();
        [JsonProperty("quotedTotalFee")]
        public Fee QuotedTotalFee { get; set; } = new Fee();
        public string Market { get; set; }
    }
    public class Waypoint
    {
        //[JsonProperty("location")]
        //public Location Location { get; set; } = new Location();
        [JsonProperty("addresses")]
        public Address Address { get; set; } = new Address();
    }
    //public class Location
    //{
    //    [JsonProperty("lat")]
    //    public string Latitude { get; set; }
    //    [JsonProperty("lng")]
    //    public string Longitude { get; set; }
    //}
    public class Address
    {
        [JsonProperty("ms_MY")]
        public AddressDetails CountryCode { get; set; } = new AddressDetails();
    }
    public class AddressDetails
    {
        [JsonProperty("displayString")]
        public string DisplayString { get; set; }
        [JsonProperty("market")]
        public string Market { get; set; }
    }
    public class DeliveryInfo
    {
        [JsonProperty("toStop")]
        public int ToStop { get; set; }
        [JsonProperty("toContact")]
        public Contact ToContact { get; set; } = new Contact();
        [JsonProperty("remarks")]
        public string Remarks { get; set; }
    }
    public class Contact
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("phone")]
        public string Phone { get; set; }
    }
    public class Fee
    {
        [JsonIgnore]
        public decimal Amount { get; set; }
        
        [JsonProperty("currency")]
        public string Currency { get; set; }
        
        
        [JsonProperty("amount")]
        public string StrTotalFee 
        {
            get => Amount.ToString();
            set => Amount = decimal.TryParse(value, out decimal totalFee) ? totalFee : 0;
        }
    }
}
