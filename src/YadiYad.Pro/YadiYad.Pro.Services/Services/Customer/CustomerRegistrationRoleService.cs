using Nop.Core.Domain.Customers;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YadiYad.Pro.Services.Services.Customer
{
    public class CustomerRegistrationRoleService
    {
        #region Fields
        private readonly IRepository<CustomerCustomerRoleMapping> _customerRoleMappingRepository;
        private readonly IRepository<CustomerRole> _customerRoleRepository;
        #endregion

        #region Ctor

        public CustomerRegistrationRoleService(
            IRepository<CustomerCustomerRoleMapping> customerRoleMappingRepository,
            IRepository<CustomerRole> customerRoleRepository)
        {
            _customerRoleMappingRepository = customerRoleMappingRepository;
            _customerRoleRepository = customerRoleRepository;
        }

        #endregion

        #region Methods

        public virtual void CreateCustomerRoleMapping(CustomerCustomerRoleMapping customer)
        {
            _customerRoleMappingRepository.Insert(customer);
        }

        public virtual List<string> GetCustomerRolesByCustomerId(int customerId)
        {
            if (customerId == 0)
                return null;

            var record = from crm in _customerRoleMappingRepository.Table
                         where crm.CustomerId == customerId
                         from cr in _customerRoleRepository.Table
                         where cr.Id == crm.CustomerRoleId
                         select cr.Name;

            return record.ToList();
        }

        #endregion

    }
}
