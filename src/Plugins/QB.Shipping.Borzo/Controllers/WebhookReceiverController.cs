using System;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.ShippingShuq;
using Nop.Core.Domain.ShippingShuq.DTO;
using Nop.Services.Shipping;
using Nop.Services.ShuqOrders;
using QB.Shipping.Borzo.Models;
using Nop.Services.Logging;

namespace QB.Shipping.Borzo.Controllers
{
    [Route("api/webhooks/borzo")]
    public class WebhookReceiverController : Controller
    {
        #region Fields

        private readonly IShuqOrderProcessingService _shuqOrderProcessingService;
        private readonly IShipmentService _shipmentService;
        private readonly ILogger _logger;
        private readonly BorzoService _borzoService;

        #endregion

        #region Ctor

        public WebhookReceiverController(IShuqOrderProcessingService shuqOrderProcessingService,
            IShipmentService shipmentService,
            ILogger logger,
            BorzoService borzoService)
        {
            _shuqOrderProcessingService = shuqOrderProcessingService;
            _shipmentService = shipmentService;
            _logger = logger;
            _borzoService = borzoService;
        }
        
        #endregion

        #region Methods

        [HttpPost("incoming")]
        public IActionResult UpdateDeliveryStatus()
        {
            Request.Headers.TryGetValue("X-DV-Signature", out var requestSignature);

            var (rawBody, calculatedSignature) = _borzoService.GetCalculatedSignature(Request);
            var model = JsonConvert.DeserializeObject<DeliveryUpdateModel>(rawBody);

            _logger.InsertLog(LogLevel.Debug, $"Borzo Webhook Invoked : {JsonConvert.SerializeObject(model)}, " +
                                              $"Raw Body : {rawBody}, " +
                                              $"Request Signature : {requestSignature}, " +
                                              $"Calculated Signature : {calculatedSignature}");

            if (requestSignature != calculatedSignature)
                return BadRequest();
            
            if (model.EventType == "delivery_changed")
            {
                var shipment = _shipmentService.GetShipmentByTrackingNumber(model.Delivery.BorzoOrderId);
                if (shipment == null)
                    return BadRequest();

                var response = new ShipmentDetailDTO
                {
                    CurrentStatus = model.Delivery.GetCarrierShippingStatus,
                    ShippingCost = model.Delivery.DeliveryPriceAmount
                };
            
                _shuqOrderProcessingService.ProcessShipment(shipment, response);
                return Ok();
            }
            return Ok();
        }

        #endregion
    }
}