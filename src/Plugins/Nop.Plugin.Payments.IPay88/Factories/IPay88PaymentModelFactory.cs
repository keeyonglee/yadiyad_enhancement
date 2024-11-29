using Nop.Core;
using Nop.Plugin.Payments.IPay88.Domain;
using Nop.Plugin.Payments.IPay88.Models;

namespace Nop.Plugin.Payments.IPay88.Factories
{
    public class IPay88PaymentModelFactory
    {
        private readonly IPay88PaymentSettings _iPay88PaymentSettings;
        private readonly IWebHelper _webHelper;

        public IPay88PaymentModelFactory(IPay88PaymentSettings iPay88PaymentSettings,
            IWebHelper webHelper)
        {
            _iPay88PaymentSettings = iPay88PaymentSettings;
            _webHelper = webHelper;
        }
        public IPay88PostPaymentModel GetIPay88PostPaymentModel(IPay88PaymentRecord iPay88PaymentRecord)
        {
            IPay88PostPaymentModel newIPay88PostPaymentModel = new IPay88PostPaymentModel();
            newIPay88PostPaymentModel.PaymentURL = _iPay88PaymentSettings.PaymentUrl;
            newIPay88PostPaymentModel.PaymentId = _iPay88PaymentSettings.PaymentId;
            newIPay88PostPaymentModel.MerchantCode = _iPay88PaymentSettings.MerchantCode;
            newIPay88PostPaymentModel.RefNo = iPay88PaymentRecord.PaymentNo;
            newIPay88PostPaymentModel.Amount = iPay88PaymentRecord.Amount.ToString("N2");
            newIPay88PostPaymentModel.Currency = iPay88PaymentRecord.CurrencyCode;
            newIPay88PostPaymentModel.ProdDesc = iPay88PaymentRecord.ProdDesc;
            newIPay88PostPaymentModel.UserName = iPay88PaymentRecord.UserName;
            newIPay88PostPaymentModel.UserEmail = iPay88PaymentRecord.UserEmail;
            newIPay88PostPaymentModel.UserContact = iPay88PaymentRecord.UserContact;
            newIPay88PostPaymentModel.Signature = iPay88PaymentRecord.Signature;
            newIPay88PostPaymentModel.SignatureType = iPay88PaymentRecord.SignatureType;
            newIPay88PostPaymentModel.Remark = iPay88PaymentRecord.Remark;

            bool isDebugPaymentGateway = _iPay88PaymentSettings.IsEnableTestPayment;
            if (isDebugPaymentGateway)
            {
                newIPay88PostPaymentModel.ResponseURL = _webHelper.GetStoreLocation(false) + "Plugins/PaymentIPay88/PaymentResult";
                newIPay88PostPaymentModel.BackendURL = _webHelper.GetStoreLocation(false) + "Plugins/PaymentIPay88/BackendUrl";
            }
            else
            {
                newIPay88PostPaymentModel.ResponseURL = _webHelper.GetStoreLocation(true) + "Plugins/PaymentIPay88/PaymentResult";
                newIPay88PostPaymentModel.BackendURL = _webHelper.GetStoreLocation(true) + "Plugins/PaymentIPay88/BackendUrl";
            }


            return newIPay88PostPaymentModel;
        }
    }
}