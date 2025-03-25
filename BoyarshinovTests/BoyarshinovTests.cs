using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BoyarshinovLib.Tests
{
    [TestClass]
    public class BusinessPartnerServiceTests
    {
        private ApplicationContext _context;
        private BusinessPartnerService _service;

        // Метод инициализации тестов, выполняется перед каждым тестом
        [TestInitialize]
        public void Initialize()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: "boyarshinov_db")
                .Options;

            _context = new ApplicationContext(options);
            _service = new BusinessPartnerService();

            // Добавление тестовых данных в базу
            _context.BusinessPartner.Add(new BusinessPartner
            {
                PartnerId = 1,
                PartnerName = "Test Partner",
                PartnerType = "ПАО",
                DirectorName = "John Doe",
                ContactEmail = "john@example.com",
                ContactPhone = "+72345678901",
                LegalAddress = "123 Main St",
                PartnerRating = 5
            });

            _context.SaveChanges();
        }

        // Метод очистки после тестов, выполняется после каждого теста
        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
        }

        // Тест проверяет корректность обновления скидок для партнера
        [TestMethod]
        public void UpdateDiscounts_ShouldUpdateDiscountsCorrectly()
        {
            // Подготовка: добавляем продажу для партнера
            var partner = _context.BusinessPartner.First();
            _context.PartnerSale.Add(new PartnerSale
            {
                SalePartnerId = partner.PartnerId,
                ProductQuantity = 20000,
                SaleDate = DateTime.Now,
                ProductName = "Product A"
            });
            _context.SaveChanges();

            // Действие: обновляем скидки
            _service.UpdateDiscounts(_context);

            // Проверка: убеждаемся, что скидка добавлена и процент правильный
            var discount = _context.PartnerDiscount.FirstOrDefault(d => d.AssociatedPartnerId == partner.PartnerId);
            Assert.IsNotNull(discount);
            Assert.AreEqual(5, discount.DiscountPercentage);
        }

        // Тест проверяет поиск партнера по его свойствам (имени)
        [TestMethod]
        public void GetPartnerByProperties_ShouldReturnCorrectPartner()
        {
            // Подготовка: задаем имя партнера для поиска
            var partnerName = "Test Partner";

            // Действие: ищем партнера по имени
            var result = _service.GetPartnerByProperties(_context, partnerName);

            // Проверка: убеждаемся, что партнер найден и его имя совпадает
            Assert.IsNotNull(result);
            Assert.AreEqual(partnerName, result.PartnerName);
        }

        // Тест проверяет загрузку списка партнеров с их скидками
        [TestMethod]
        public void LoadPartners_ShouldReturnListOfPartnersWithDiscounts()
        {
            // Подготовка: добавляем скидку для существующего партнера
            _context.PartnerDiscount.Add(new PartnerDiscount
            {
                AssociatedPartnerId = 1,
                DiscountPercentage = 10
            });
            _context.SaveChanges();

            // Действие: загружаем список партнеров
            var result = _service.LoadPartners(_context);

            // Проверка: убеждаемся, что список не пустой и скидка соответствует
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(10, result[0].DiscountPercentage);
        }

        // Тест проверяет добавление нового партнера в базу данных
        [TestMethod]
        public void AddPartner_ShouldAddPartnerToDatabase()
        {
            // Подготовка: создаем нового партнера
            var newPartner = new BusinessPartner
            {
                PartnerName = "New Partner",
                PartnerType = "ООО",
                DirectorName = "Jane Doe",
                ContactEmail = "jane@example.com",
                ContactPhone = "+7098765432",
                LegalAddress = "456 Elm St",
                PartnerRating = 4
            };

            // Действие: добавляем партнера в базу
            _service.AddPartner(_context, newPartner);

            // Проверка: убеждаемся, что партнер добавлен и данные корректны
            var partner = _context.BusinessPartner.FirstOrDefault(p => p.PartnerName == "New Partner");
            Assert.IsNotNull(partner);
            Assert.AreEqual("ООО", partner.PartnerType);
        }

        // Тест проверяет обновление данных партнера в базе
        [TestMethod]
        public void UpdatePartner_ShouldUpdatePartnerInDatabase()
        {
            // Подготовка: изменяем рейтинг существующего партнера
            var partner = _context.BusinessPartner.First();
            partner.PartnerRating = 10;

            // Действие: обновляем данные партнера
            _service.UpdatePartner(_context, partner);

            // Проверка: убеждаемся, что рейтинг обновился
            var updatedPartner = _context.BusinessPartner.First();
            Assert.AreEqual(10, updatedPartner.PartnerRating);
        }

        // Тест проверяет удаление партнера из базы данных
        [TestMethod]
        public void DeletePartner_ShouldRemovePartnerFromDatabase()
        {
            // Подготовка: берем существующего партнера
            var partner = _context.BusinessPartner.First();

            // Действие: удаляем партнера
            _service.DeletePartner(_context, partner);

            // Проверка: убеждаемся, что партнер больше не существует
            var deletedPartner = _context.BusinessPartner.FirstOrDefault(p => p.PartnerId == 1);
            Assert.IsNull(deletedPartner);
        }

        // Тест проверяет добавление новой продажи в базу данных
        [TestMethod]
        public void AddSale_ShouldAddSaleToDatabase()
        {
            // Подготовка: создаем новую продажу
            var sale = new PartnerSale
            {
                SalePartnerId = 1,
                ProductQuantity = 10,
                SaleDate = DateTime.Now,
                ProductName = "Product B"
            };

            // Действие: добавляем продажу
            _service.AddSale(_context, sale);

            // Проверка: убеждаемся, что продажа добавлена и данные корректны
            var addedSale = _context.PartnerSale.FirstOrDefault(s => s.ProductName == "Product B");
            Assert.IsNotNull(addedSale);
            Assert.AreEqual(10, addedSale.ProductQuantity);
        }

        // Тест проверяет обновление данных продажи в базе
        [TestMethod]
        public void UpdateSale_ShouldUpdateSaleInDatabase()
        {
            // Подготовка: добавляем продажу и изменяем её количество
            var sale = new PartnerSale
            {
                SalePartnerId = 1,
                ProductQuantity = 5,
                SaleDate = DateTime.Now,
                ProductName = "Product C"
            };
            _context.PartnerSale.Add(sale);
            _context.SaveChanges();

            sale.ProductQuantity = 15;

            // Действие: обновляем продажу
            _service.UpdateSale(_context, sale);

            // Проверка: убеждаемся, что количество обновилось
            var updatedSale = _context.PartnerSale.FirstOrDefault(s => s.ProductName == "Product C");
            Assert.IsNotNull(updatedSale);
            Assert.AreEqual(15, updatedSale.ProductQuantity);
        }

        // Тест проверяет удаление продажи из базы данных
        [TestMethod]
        public void DeleteSale_ShouldRemoveSaleFromDatabase()
        {
            // Подготовка: добавляем продажу для последующего удаления
            var sale = new PartnerSale
            {
                SalePartnerId = 1,
                ProductQuantity = 20,
                SaleDate = DateTime.Now,
                ProductName = "Product D"
            };
            _context.PartnerSale.Add(sale);
            _context.SaveChanges();

            // Действие: удаляем продажу
            _service.DeleteSale(_context, sale);

            // Проверка: убеждаемся, что продажа удалена
            var deletedSale = _context.PartnerSale.FirstOrDefault(s => s.ProductName == "Product D");
            Assert.IsNull(deletedSale);
        }

        // Тест проверяет получение списка продаж для конкретного партнера
        [TestMethod]
        public void GetSales_ShouldReturnSalesForPartner()
        {
            // Подготовка: добавляем продажу для существующего партнера
            var partner = _context.BusinessPartner.First();
            _context.PartnerSale.Add(new PartnerSale
            {
                SalePartnerId = partner.PartnerId,
                ProductQuantity = 25,
                SaleDate = DateTime.Now,
                ProductName = "Product E"
            });
            _context.SaveChanges();

            // Действие: получаем список продаж партнера
            var sales = _service.GetSales(_context, partner);

            // Проверка: убеждаемся, что список продаж корректен
            Assert.IsNotNull(sales);
            Assert.AreEqual(1, sales.Count());
            Assert.AreEqual("Product E", sales.First().ProductName);
        }
    }
}