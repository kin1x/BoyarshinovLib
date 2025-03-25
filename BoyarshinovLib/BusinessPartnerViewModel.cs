using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoyarshinovLib
{
    // Класс модели представления для отображения данных о бизнес-партнере
    public class BusinessPartnerViewModel
    {
        // Уникальный идентификатор партнера (используется, если требуется привязка к базе данных)
        public int Id { get; set; }

        // Тип партнера (например, "ПАО", "ООО" и т.д.)
        public string PartnerType { get; set; }

        // Название партнера
        public string PartnerName { get; set; }

        // Имя директора партнера
        public string DirectorName { get; set; }

        // Контактный телефон партнера
        public string ContactPhone { get; set; }

        // Рейтинг партнера (необязательное поле, может быть null)
        public int? PartnerRating { get; set; }

        // Процент скидки партнера (необязательное поле, может быть null)
        public double? DiscountPercentage { get; set; }
    }
}