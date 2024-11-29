using Nop.Core;
using Nop.Core.Domain.Documents;
using Nop.Data;
using Nop.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace Nop.Services.Documents
{
    public class DocumentNumberService
    {
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IRepository<RunningNumber> _runningNumberRepo;

        public DocumentNumberService(
            IDateTimeHelper dateTimeHelper,
            IRepository<RunningNumber> runningNumberRepo)
        {
            _dateTimeHelper = dateTimeHelper;
            _runningNumberRepo = runningNumberRepo;
        }

        public int GetNextRunningNumber(RunningNumberType type, int year, int noOfNextRunningNumber = 1, int? customerId = null, int? vendorId = null)
        {
            var runningNumber = _runningNumberRepo.Table
                .Where(x => x.RunningNumberTypeId == (int)type
                && x.CustomerId == customerId
                && x.VendorId == vendorId)
                .FirstOrDefault();

            if (runningNumber == null)
            {
                runningNumber = new RunningNumber
                {
                    RunningNumberType = type,
                    CustomerId = customerId,
                    VendorId = vendorId,
                    LastId = 0,
                    LastYear = year,
                    Name = type.GetName()
                    .Replace("{{Customer}}", customerId + "")
                    .Replace("{{VendorId}}", vendorId + "")
                };

                _runningNumberRepo.Insert(runningNumber);
            }

            var lastId = runningNumber.LastId;

            if (year != runningNumber.LastYear)
            {
                lastId = 0;
            }

            runningNumber.LastId = lastId + noOfNextRunningNumber;
            runningNumber.LastYear = year;

            _runningNumberRepo.Update(runningNumber);

            return runningNumber.LastId;
        }

        public IList<string> GetDocumentNumbers(RunningNumberType type, int noOfNextRunningNumber, int? customerId = null, int? vendorId = null)
        {
            var localToday = _dateTimeHelper.ConvertToUserTime(DateTime.UtcNow, DateTimeKind.Utc);
            var newDocumentNumbers = new List<string>();
            var nextId = GetNextRunningNumber(type, localToday.Year, noOfNextRunningNumber, customerId, vendorId);

            var numberFormat = type.GetDisplayFormat();

            for (int i = 0; i < noOfNextRunningNumber; i++)
            {
                var newDocumentNumber = numberFormat
                    .Replace("{{Year}}", localToday.Year.ToString())
                    .Replace("{{RunningNumber:000000}}", (nextId + i).ToString("000000"))
                    .Replace("{{RunningNumber:0000}}", (nextId + i).ToString("0000"))
                    .Replace("{{CustomerId}}", customerId?.ToString())
                    .Replace("{{VendorId}}", vendorId?.ToString());

                newDocumentNumbers.Add(newDocumentNumber);
            }

            return newDocumentNumbers;
        }
    }
}
