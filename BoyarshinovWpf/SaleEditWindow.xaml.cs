using BoyarshinovLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BoyarshinovWpf
{
    // Класс диалогового окна для добавления или редактирования продажи
    public partial class SaleEditWindow : Window
    {
        private readonly ApplicationContext _context; // Контекст базы данных
        private PartnerSale _sale; // Объект продажи для редактирования или добавления
        private bool _isEditMode; // Флаг, указывающий на режим редактирования или добавления
        private readonly BusinessPartnerService _partnerService; // Сервис для работы с партнерами и продажами
        private BusinessPartner _partner; // Партнер, связанный с продажей

        // Конструктор окна
        public SaleEditWindow(ApplicationContext context, PartnerSale sale = null, BusinessPartner partner = null)
        {
            InitializeComponent(); // Инициализация компонентов XAML
            _context = context; // Установка контекста базы данных
            _partnerService = new BusinessPartnerService(); // Создание экземпляра сервиса
            _partner = partner ?? throw new ArgumentNullException(nameof(partner), "Бизнес партнер не выбран."); // Проверка на null для партнера

            if (sale != null)
            {
                // Если передана продажа, настраиваем окно для редактирования
                _isEditMode = true; // Установка режима редактирования
                _sale = sale; // Установка объекта продажи
                Title = "Редактирование продукта"; // Изменение заголовка
                LoadSaleData(); // Загрузка данных продажи в поля
            }
            else
            {
                // Если продажа не передана, настраиваем окно для добавления
                _isEditMode = false; // Установка режима добавления
                Title = "Добавление продукта"; // Установка заголовка
                _sale = new PartnerSale(); // Создание нового объекта продажи
            }
        }

        // Метод загрузки данных продажи в поля ввода
        private void LoadSaleData()
        {
            ProductNameTextBox.Text = _sale.ProductName; // Заполнение названия продукта
            QuantityTextBox.Text = _sale.ProductQuantity.ToString(); // Заполнение количества
            SaleDatePicker.SelectedDate = _sale.SaleDate; // Заполнение даты продажи
        }

        // Обработчик нажатия кнопки "Сохранить"
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Валидация данных ввода
                if (string.IsNullOrWhiteSpace(ProductNameTextBox.Text))
                {
                    MessageBox.Show("Введите корректное наименование продукта.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!int.TryParse(QuantityTextBox.Text, out int quantity) || quantity <= 0)
                {
                    MessageBox.Show("Количество должно быть положительным числом.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (SaleDatePicker.SelectedDate == null)
                {
                    MessageBox.Show("Выберите дату продажи.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (SaleDatePicker.SelectedDate > DateTime.Today)
                {
                    MessageBox.Show("Дата продажи не может быть в будущем.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Заполнение объекта продажи данными из полей
                _sale.ProductName = ProductNameTextBox.Text; // Установка названия продукта
                _sale.ProductQuantity = quantity; // Установка количества
                _sale.SaleDate = SaleDatePicker.SelectedDate.Value; // Установка даты продажи
                _sale.SalePartnerId = _partner.PartnerId; // Установка ID партнера

                if (_isEditMode)
                {
                    // Если режим редактирования, обновляем существующую продажу
                    _partnerService.UpdateSale(_context, _sale);
                }
                else
                {
                    // Если режим добавления, добавляем новую продажу
                    _partnerService.AddSale(_context, _sale);
                }

                _partnerService.SaveChanges(_context); // Сохранение изменений в базе данных
                DialogResult = true; // Установка результата диалога как успешного
                Close(); // Закрытие окна
            }
            catch (Exception ex)
            {
                // Обработка ошибок и вывод сообщения
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Обработчик нажатия кнопки "Отменить"
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // Установка результата диалога как неуспешного
            Close(); // Закрытие окна без сохранения
        }
    }
}