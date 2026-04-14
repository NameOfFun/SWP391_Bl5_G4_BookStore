namespace BookStore.Models;

public enum PaymentChannel
{
    CashOnDelivery = 0,
    BankTransfer = 1,
    Vnpay = 2,
    Momo = 3,
    ZaloPay = 4,
    CreditCard = 5,
    Other = 99
}
