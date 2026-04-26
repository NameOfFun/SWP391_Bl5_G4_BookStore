namespace BookStore.Models;

public enum OrderStatus
{
    Pending    = 0,
    Confirmed  = 1,
    Processing = 2,
    Delivered  = 4,
    Cancelled  = 5,
}
