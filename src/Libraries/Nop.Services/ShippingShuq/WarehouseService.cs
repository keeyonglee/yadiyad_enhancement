using Nop.Core.Domain.Shipping;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Services.ShippingShuq
{
    public class WarehouseService
    {
        #region Fields

        private readonly IRepository<Warehouse> _WarehouseRepository;

        #endregion

        #region Ctor
        public WarehouseService(
            IRepository<Warehouse> WarehouseRepository)
        {
            _WarehouseRepository = WarehouseRepository;
        }
        #endregion

        #region Methods

        public int? GetWarehouseByName(string name)
        {
            if (name == "" || name == null)
                return null;

            var query = _WarehouseRepository.Table;

            return query.Where(x => x.Name == name).FirstOrDefault().Id;
        }

        #endregion
    }
}
