using System;
using System.Collections.Generic;

namespace BoyarshinovLib
{
    // Частичный класс, представляющий сущность скидки партнера в приложении
    public partial class PartnerDiscount
    {
        // Уникальный идентификатор скидки
        public int DiscountId { get; set; }

        // Идентификатор партнера, связанного со скидкой (может быть null)
        public int? AssociatedPartnerId { get; set; }

        // Процент скидки (может быть null)
        public double? DiscountPercentage { get; set; }

        // Навигационное свойство для доступа к связанному бизнес-партнеру
        public virtual BusinessPartner AssociatedPartner { get; set; }
    }
}