namespace Nop.Core.Domain.Discounts
{
    /// <summary>
    /// Represents a discount type
    /// </summary>
    public enum DiscountType
    {
        /// <summary>
        /// Assigned to order total 
        /// </summary>
        AssignedToOrderTotal = 1,

        /// <summary>
        /// Assigned to products (SKUs)
        /// </summary>
        AssignedToProducts = 2,

        /// <summary>
        /// Assigned to categories (all products in a category)
        /// </summary>
        AssignedToCategories = 5,

        /// <summary>
        /// Assigned to manufacturers (all products of a manufacturer)
        /// </summary>
        AssignedToManufacturers = 6,

        /// <summary>
        /// Assigned to shipping
        /// </summary>
        AssignedToShipping = 10,

        /// <summary>
        /// Assigned to order subtotal
        /// </summary>
        AssignedToOrderSubTotal = 20,

        /// <summary>
        /// Assigned to platform shipping
        /// </summary>
        AssignedToPlatformShipping = 1001,

        /// <summary>
        /// Assigned to platform order sub total
        /// </summary>
        AssignedToPlatformOrderSubTotal = 2001,
    }

    public enum CustomVendorDiscountType
    {


        /// <summary>
        /// Assigned to products (SKUs)
        /// </summary>
        AssignedToProducts = 2,

        /// <summary>
        /// Assigned to shipping
        /// </summary>
        AssignedToShipping = 10,

        /// <summary>
        /// Assigned to order total 
        /// </summary>
        AssignedToOrderSubTotal = 20,

    }

    public enum CustomAdminDiscountType
    {
        /// <summary>
        /// Assigned to products (SKUs)
        /// </summary>
        AssignedToProducts = 2,

        /// <summary>
        /// Assigned to shipping
        /// </summary>
        AssignedToShipping = 10,

        /// <summary>
        /// Assigned to order total 
        /// </summary>
        AssignedToOrderSubTotal = 20,

        /// <summary>
        /// Assigned to platform shipping
        /// </summary>
        AssignedToPlatformShipping = 1001,

        /// <summary>
        /// Assigned to platform order total
        /// </summary>
        AssignedToPlatformOrderSubTotal = 2001,



    }
}
