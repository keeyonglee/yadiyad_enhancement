using Nop.Core.Domain.Customers;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YadiYad.Pro.Services.Services.Operator
{
    public partial class OperatorService : IOperatorService
    {
        private readonly IRepository<CustomerRole> _CustomerRoleRepository;
        private readonly IRepository<Nop.Core.Domain.Customers.Customer> _CustomerRepository;
        private readonly IRepository<CustomerCustomerRoleMapping> _CustomerCustomerRoleMappingRepository;

        public OperatorService(
            IRepository<CustomerRole> CustomerRoleRepository,
            IRepository<Nop.Core.Domain.Customers.Customer> CustomerRepository,
            IRepository<CustomerCustomerRoleMapping> CustomerCustomerRoleMappingRepository)
        {
            _CustomerRoleRepository = CustomerRoleRepository;
            _CustomerRepository = CustomerRepository;
            _CustomerCustomerRoleMappingRepository = CustomerCustomerRoleMappingRepository;
        }

        public List<int> GetAllOperatorCustomerIds()
        {
            var roleId = _CustomerRoleRepository.Table
                         .Where(x => x.SystemName == OperatorDefaults.OperatorSystemName)
                         .Select(x => x.Id)
                         .FirstOrDefault();

            var operatorCustomerIds = from c in _CustomerRepository.Table
                                      join ccrm in _CustomerCustomerRoleMappingRepository.Table on c.Id equals ccrm.CustomerId
                                      join cr in _CustomerRoleRepository.Table on ccrm.CustomerRoleId equals cr.Id
                                      where cr.Id == roleId
                                      select  c.Id;

            return operatorCustomerIds.ToList();
        }
    }
}
