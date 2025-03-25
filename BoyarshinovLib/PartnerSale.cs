using System;
using System.Collections.Generic;

namespace BoyarshinovLib
{
    // Частичный класс, представляющий сущность продажи партнера в приложении
    public partial class PartnerSale
    {
        // Уникальный идентификатор продажи
        public int SaleId { get; set; }

        // Идентификатор партнера, связанного с продажей (может быть null)
        public int? SalePartnerId { get; set; }

        // Количество проданных продуктов
        public int ProductQuantity { get; set; }

        // Дата продажи
        public DateTime SaleDate { get; set; }

        // Название проданного продукта
        public string ProductName { get; set; }

        // Навигационное свойство для доступа к связанному бизнес-партнеру
        public virtual BusinessPartner SalePartner { get; set; }
    }
}