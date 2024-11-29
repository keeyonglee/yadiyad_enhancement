using Nop.Core;
using Nop.Plugin.Payments.IPay88.Domain;
using System.Collections.Generic;

namespace Nop.Plugin.Payments.IPay88.Services
{
    public partial interface IIPay88Service
    {
        void DeleteIPay88PaymentRecord(IPay88PaymentRecord iPay88PaymentRecord);

        List<IPay88PaymentRecord> GetAll(int pageIndex = 0, int pageSize = int.MaxValue);

        IPay88PaymentRecord FindRecord(string signature, string internalPaymentStatus = "");

        IPay88PaymentRecord GetById(int iPay88PaymentRecordId);

        void InsertIPay88PaymentRecord(IPay88PaymentRecord iPay88PaymentRecord);

        void UpdateIPay88PaymentRecord(IPay88PaymentRecord iPay88PaymentRecord);
        IPay88PaymentRecord GetByOrderId(int orderId, int orderTypeId);
    }
}
