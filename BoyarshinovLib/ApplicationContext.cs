using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BoyarshinovLib
{
    // Класс контекста базы данных для работы с сущностями приложения
    public partial class ApplicationContext : DbContext
    {
        // Конструктор по умолчанию
        public ApplicationContext()
        {
        }

        // Конструктор с параметрами для передачи опций конфигурации контекста
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }

        // Наборы данных (таблицы) для работы с сущностями
        public virtual DbSet<BusinessPartner> BusinessPartner { get; set; } // Таблица бизнес-партнеров
        public virtual DbSet<PartnerDiscount> PartnerDiscount { get; set; } // Таблица скидок партнеров
        public virtual DbSet<PartnerSale> PartnerSale { get; set; }         // Таблица продаж партнеров

        // Метод настройки подключения к базе данных
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Если опции не сконфигурированы, задаем подключение к PostgreSQL
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=localhost;Database=boyarshinov_db;Username=app;Password=123456789");
            }
        }

        // Метод настройки структуры модели базы данных
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Конфигурация сущности BusinessPartner (бизнес-партнер)
            modelBuilder.Entity<BusinessPartner>(entity =>
            {
                // Установка первичного ключа
                entity.HasKey(e => e.PartnerId)
                    .HasName("business_partner_pkey");

                // Указание таблицы и схемы в базе данных
                entity.ToTable("business_partner", "app");

                // Настройка свойства PartnerId (ID партнера)
                entity.Property(e => e.PartnerId)
                    .HasColumnName("partner_id")
                    .HasDefaultValueSql("nextval('business_partner_partner_id_seq'::regclass)"); // Автоинкремент

                // Настройка обязательного поля ContactEmail (контактный email)
                entity.Property(e => e.ContactEmail)
                    .IsRequired()
                    .HasColumnName("contact_email");

                // Настройка обязательного поля ContactPhone (контактный телефон)
                entity.Property(e => e.ContactPhone)
                    .IsRequired()
                    .HasColumnName("contact_phone");

                // Настройка обязательного поля DirectorName (имя директора)
                entity.Property(e => e.DirectorName)
                    .IsRequired()
                    .HasColumnName("director_name");

                // Настройка обязательного поля LegalAddress (юридический адрес)
                entity.Property(e => e.LegalAddress)
                    .IsRequired()
                    .HasColumnName("legal_address");

                // Настройка обязательного поля PartnerName (название партнера)
                entity.Property(e => e.PartnerName)
                    .IsRequired()
                    .HasColumnName("partner_name");

                // Настройка поля PartnerRating (рейтинг партнера)
                entity.Property(e => e.PartnerRating)
                    .HasColumnName("partner_rating");

                // Настройка обязательного поля PartnerType (тип партнера)
                entity.Property(e => e.PartnerType)
                    .IsRequired()
                    .HasColumnName("partner_type");
            });

            // Конфигурация сущности PartnerDiscount (скидка партнера)
            modelBuilder.Entity<PartnerDiscount>(entity =>
            {
                // Установка первичного ключа
                entity.HasKey(e => e.DiscountId)
                    .HasName("partner_discount_pkey");

                // Указание таблицы и схемы в базе данных
                entity.ToTable("partner_discount", "app");

                // Настройка свойства DiscountId (ID скидки)
                entity.Property(e => e.DiscountId)
                    .HasColumnName("discount_id")
                    .HasDefaultValueSql("nextval('partner_discount_discount_id_seq'::regclass)"); // Автоинкремент

                // Настройка поля AssociatedPartnerId (ID связанного партнера)
                entity.Property(e => e.AssociatedPartnerId)
                    .HasColumnName("associated_partner_id");

                // Настройка поля DiscountPercentage (процент скидки)
                entity.Property(e => e.DiscountPercentage)
                    .HasColumnName("discount_percentage");

                // Установка внешнего ключа для связи с BusinessPartner
                entity.HasOne(d => d.AssociatedPartner)
                    .WithMany(p => p.PartnerDiscount)
                    .HasForeignKey(d => d.AssociatedPartnerId)
                    .OnDelete(DeleteBehavior.SetNull) // При удалении партнера скидка остается с NULL
                    .HasConstraintName("partner_discount_associated_partner_id_fkey");
            });

            // Конфигурация сущности PartnerSale (продажа партнера)
            modelBuilder.Entity<PartnerSale>(entity =>
            {
                // Установка первичного ключа
                entity.HasKey(e => e.SaleId)
                    .HasName("partner_sale_pkey");

                // Указание таблицы и схемы в базе данных
                entity.ToTable("partner_sale", "app");

                // Настройка свойства SaleId (ID продажи)
                entity.Property(e => e.SaleId)
                    .HasColumnName("sale_id")
                    .HasDefaultValueSql("nextval('partner_sale_sale_id_seq'::regclass)"); // Автоинкремент

                // Настройка обязательного поля ProductName (название продукта)
                entity.Property(e => e.ProductName)
                    .IsRequired()
                    .HasColumnName("product_name")
                    .HasColumnType("character varying");

                // Настройка поля ProductQuantity (количество продукта)
                entity.Property(e => e.ProductQuantity)
                    .HasColumnName("product_quantity");

                // Настройка поля SaleDate (дата продажи)
                entity.Property(e => e.SaleDate)
                    .HasColumnName("sale_date")
                    .HasColumnType("date");

                // Настройка поля SalePartnerId (ID партнера, связанного с продажей)
                entity.Property(e => e.SalePartnerId)
                    .HasColumnName("sale_partner_id");

                // Установка внешнего ключа для связи с BusinessPartner
                entity.HasOne(d => d.SalePartner)
                    .WithMany(p => p.PartnerSale)
                    .HasForeignKey(d => d.SalePartnerId)
                    .OnDelete(DeleteBehavior.SetNull) // При удалении партнера продажа остается с NULL
                    .HasConstraintName("partner_sale_sale_partner_id_fkey");
            });

            // Вызов частичного метода для дополнительной настройки модели (если требуется)
            OnModelCreatingPartial(modelBuilder);
        }

        // Частичный метод для расширения настройки модели (пустая реализация по умолчанию)
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}