namespace WebShop.Dal.Entities;

[Flags]
public enum OrderStatus
{
    Unread = 1,
    Processing = 2,
    Shipping = 4,
    Delivered = 8,
    Rejected = 16,
}
