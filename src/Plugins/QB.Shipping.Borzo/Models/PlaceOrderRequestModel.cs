using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Core.Domain.ShippingShuq.DTO;

namespace QB.Shipping.Borzo.Models
{
    public class PlaceOrderRequestModel
    {
        [JsonProperty("matter")]
        public string Matter { get; set; }
        [JsonProperty("vehicle_type_id")]
        public int VehicleTypeId { get; set; }
        [JsonProperty("total_weight_kg")]
        public int TotalWeightKg { get; set; }
        [JsonProperty("is_client_notification_enabled")]
        public bool IsClientNotificationEnabled { get; set; }
        [JsonProperty("is_contact_person_notification_enabled")]
        public bool IsContactPersonNotificationEnabled { get; set; }

        [JsonProperty("points")]
        public List<Point> Points { get; set; }


    }

    public class Point
    {
        [JsonProperty("address")]
        public string Address { get; set; }
        [JsonProperty("contact_person")]
        public ContactPerson ContactPerson { get; set; }
        [JsonProperty("required_start_datetime")]
        public string RequiredStartDatetime { get; set; }
    }

    public class ContactPerson
    {
        [JsonProperty("phone")]
        public string Phone { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}