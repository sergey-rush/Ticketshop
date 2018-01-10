namespace TicketShop.Core
{
    /// <summary>
    /// General Status for most objects
    /// </summary>
    public enum ItemStatus
    {
        [Enum("Неопределен")] None = 0,
        [Enum("Свободен")] OnSale = 1,
        [Enum("В обработке")]Locked = 2,
        [Enum("Зарезервирован")] Reserved = 3,
        [Enum("Оплачен")] Sold = 4,
        [Enum("Списан")] Removed = 5,
        [Enum("Возвращен")] Returned = 6,
        [Enum("Аннулирован")] Cancelled = 7,
        [Enum("На реализации")] OnRetail = 8,
        [Enum("На складе")] OnStock = 9,
        [Enum("Удален")] Deleted = 10,
        [Enum("Распечатан")] Printed = 11,
        [Enum("Создание заказа")]OnOrderCreating = 12,
        [Enum("Изменена бронь")] BookingChanged = 13,
        [Enum("В печать")] ToBePrinted = 14
    }

    //public enum BlankStatus
    //{
    //    [Enum("Неопределен")]None = 0,
    //    [Enum("Действительный")]Valid = 1,
    //    [Enum("Недействительный")]Invalid = 2,
    //    [Enum("Оплачен")]Sold = 3,
    //    [Enum("Распечатан")]Printed = 4,
    //    [Enum("Возвращен")]Returned = 5,
    //    [Enum("Списан")]Removed = 6,
    //    [Enum("Удален")]Deleted = 7,
    //    [Enum("Аннулирован")]Cancelled = 8
    //}

    /// <summary>
    /// Result of operation, transaction or any other action with a seat
    /// </summary>
    public enum OperationResult
    {
        [Enum("Неопределен")]
        None = 0,
        [Enum("Ошибка")]
        Failed = 1,
        [Enum("Успешно")]
        Success = 2,
        [Enum("Невозможно изменить статус билета")]
        UnableChange = 3,
        [Enum("Пользователь не может изменить статус билета")]
        UserUnableChange = 4,
        [Enum("Остаток времени до выступления не позволяет отменить заказ или билет")]
        UnableCancel = 5,
        [Enum("Остаток времени до выступления не позволяет оплатить заказ или билет")]
        UnablePay = 6,
    }

    public enum PaymentType
    {
        [Enum("Наличными")]
        Cash = 1,
        [Enum("Картой")]
        Card = 2,
        [Enum("Безналичный")]
        NonCash = 3
    }

    public enum HttpMethod
    {
        Unknown,
        GET,
        POST,
        PUT,
        DELETE
    }

    public enum WisardStep
    {
        /// <summary>
        /// No status
        /// </summary>
        None,
        /// <summary>
        /// On locked seats
        /// </summary>
        Locked,
        /// <summary>
        /// On order created 
        /// </summary>
        OrderCreated,
        /// <summary>
        /// On order is paid
        /// </summary>
        OrderPaid,
        /// <summary>
        /// On order is printed
        /// </summary>
        Printed
    }
}