namespace Nouhaila.netProjet.Models
{
    public enum ProductType
    {
        Menu,
        Individual,
        Drink,
        Dessert,
        Side
    }

    public enum OrderStatus
    {
        Pending,
        Preparing,
        OnTheWay,
        Delivered,
        Cancelled
    }

    public enum PaymentMethod
    {
        CreditCard,
        CashOnDelivery
    }
}
