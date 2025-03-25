using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoyarshinovLib
{
    // Класс сервиса для работы с бизнес-партнерами и связанными сущностями
    public class BusinessPartnerService
    {
        // Метод обновления скидок для всех партнеров на основе их продаж
        public void UpdateDiscounts(ApplicationContext _context)
        {
            // Загружаем всех партнеров с их продажами
            var partners = _context.BusinessPartner
                .Include(p => p.PartnerSale)
                .ToList();

            if (partners != null)
            {
                foreach (var partner in partners)
                {
                    // Вычисляем общее количество проданных продуктов
                    var totalSales = partner.PartnerSale?.Sum(s => s.ProductQuantity) ?? 0;

                    double discountPercentage;

                    // Определяем процент скидки в зависимости от общего объема продаж
                    if (totalSales <= 10000)
                    {
                        discountPercentage = 0;
                    }
                    else if (totalSales <= 50000)
                    {
                        discountPercentage = 5;
                    }
                    else if (totalSales <= 300000)
                    {
                        discountPercentage = 10;
                    }
                    else
                    {
                        discountPercentage = 15;
                    }

                    // Проверяем, есть ли уже скидка для партнера
                    var discount = _context.PartnerDiscount.FirstOrDefault(d => d.AssociatedPartnerId == partner.PartnerId);
                    if (discount == null)
                    {
                        // Если скидки нет, добавляем новую
                        _context.PartnerDiscount.Add(new PartnerDiscount
                        {
                            AssociatedPartnerId = partner.PartnerId,
                            DiscountPercentage = discountPercentage
                        });
                    }
                    else
                    {
                        // Если скидка есть, обновляем её значение
                        discount.DiscountPercentage = discountPercentage;
                    }
                }
                // Сохраняем все изменения в базе данных
                _context.SaveChanges();
            }
        }

        // Метод поиска партнера по имени
        public BusinessPartner GetPartnerByProperties(ApplicationContext _context, string name)
        {
            // Возвращаем первого партнера, чье имя совпадает с заданным, включая его продажи
            return _context.BusinessPartner
                .Include(p => p.PartnerSale)
                .FirstOrDefault(p => p.PartnerName == name);
        }

        // Метод сохранения изменений в контексте базы данных
        public void SaveChanges(ApplicationContext _context)
        {
            if (_context != null)
            {
                // Сохраняем изменения в базе данных
                _context.SaveChanges();
            }
        }

        // Метод загрузки списка партнеров с их скидками в виде модели представления
        public List<BusinessPartnerViewModel> LoadPartners(ApplicationContext _context)
        {
            // Загружаем партнеров с их скидками и преобразуем в модель представления
            return _context.BusinessPartner
                .Include(p => p.PartnerDiscount)
                .Select(p => new BusinessPartnerViewModel
                {
                    PartnerType = p.PartnerType,
                    PartnerName = p.PartnerName,
                    DirectorName = p.DirectorName,
                    ContactPhone = p.ContactPhone,
                    PartnerRating = p.PartnerRating,
                    DiscountPercentage = p.PartnerDiscount.First().DiscountPercentage // Предполагается, что у партнера одна скидка
                })
                .ToList();
        }

        // Метод добавления нового партнера в базу данных
        public void AddPartner(ApplicationContext _context, BusinessPartner partner)
        {
            if (partner != null)
            {
                // Добавляем партнера в контекст и сохраняем изменения
                _context.BusinessPartner.Add(partner);
                _context.SaveChanges();
            }
        }

        // Метод обновления данных существующего партнера
        public void UpdatePartner(ApplicationContext _context, BusinessPartner partner)
        {
            if (partner != null)
            {
                // Обновляем данные партнера в контексте и сохраняем изменения
                _context.BusinessPartner.Update(partner);
                _context.SaveChanges();
            }
        }

        // Метод удаления партнера из базы данных
        public void DeletePartner(ApplicationContext _context, BusinessPartner partner)
        {
            if (partner != null)
            {
                // Находим и удаляем все продажи, связанные с партнером
                var sales = _context.PartnerSale
                            .Where(s => s.SalePartnerId == partner.PartnerId)
                            .ToList();
                if (sales != null)
                {
                    _context.PartnerSale.RemoveRange(sales);
                }
                // Удаляем самого партнера и сохраняем изменения
                _context.BusinessPartner.Remove(partner);
                _context.SaveChanges();
            }
        }

        // Метод получения списка всех партнеров с их скидками
        public IEnumerable<BusinessPartner> GetPartners(ApplicationContext _context)
        {
            // Возвращаем всех партнеров с включенными скидками
            return _context.BusinessPartner.Include(p => p.PartnerDiscount).ToList();
        }

        // Метод добавления новой продажи в базу данных
        public void AddSale(ApplicationContext _context, PartnerSale sale)
        {
            if (sale != null)
            {
                // Добавляем продажу в контекст и сохраняем изменения
                _context.PartnerSale.Add(sale);
                _context.SaveChanges();
            }
        }

        // Метод обновления данных существующей продажи
        public void UpdateSale(ApplicationContext _context, PartnerSale sale)
        {
            if (sale != null)
            {
                // Обновляем данные продажи в контексте и сохраняем изменения
                _context.PartnerSale.Update(sale);
                _context.SaveChanges();
            }
        }

        // Метод удаления продажи из базы данных
        public void DeleteSale(ApplicationContext _context, PartnerSale sale)
        {
            if (sale != null)
            {
                // Удаляем продажу из контекста и сохраняем изменения
                _context.PartnerSale.Remove(sale);
                _context.SaveChanges();
            }
        }

        // Метод получения списка продаж для конкретного партнера
        public IEnumerable<PartnerSale> GetSales(ApplicationContext _context, BusinessPartner partner)
        {
            if (partner == null)
                // Если партнер не указан, возвращаем пустой список
                return Enumerable.Empty<PartnerSale>();

            // Возвращаем все продажи, связанные с заданным партнером
            return _context.PartnerSale
                            .Where(s => s.SalePartnerId == partner.PartnerId)
                            .ToList();
        }
    }
}