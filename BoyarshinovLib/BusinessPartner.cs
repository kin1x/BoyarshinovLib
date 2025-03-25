using System;
using System.Collections.Generic;

namespace BoyarshinovLib
{
    // Частичный класс, представляющий сущность бизнес-партнера в приложении
    public partial class BusinessPartner
    {
        // Уникальный идентификатор партнера
        public int PartnerId { get; set; }

        // Тип партнера (например, "ПАО", "ООО" и т.д.)
        public string PartnerType { get; set; }

        // Название партнера (обязательное поле)
        public string PartnerName { get; set; }

        // Имя директора партнера (обязательное поле)
        public string DirectorName { get; set; }

        // Контактный email партнера (обязательное поле)
        public string ContactEmail { get; set; }

        // Контактный телефон партнера (обязательное поле)
        public string ContactPhone { get; set; }

        // Юридический адрес партнера (обязательное поле)
        public string LegalAddress { get; set; }

        // Рейтинг партнера (необязательное поле, может быть null)
        public int? PartnerRating { get; set; }

        // Коллекция скидок, связанных с данным партнером
        public virtual ICollection<PartnerDiscount> PartnerDiscount { get; set; }

        // Коллекция продаж, связанных с данным партнером
        public virtual ICollection<PartnerSale> PartnerSale { get; set; }
    }
}