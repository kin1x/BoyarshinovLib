using BoyarshinovLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    // Класс диалогового окна для добавления или редактирования бизнес-партнера
    public partial class PartnerEditWindow : Window
    {
        private readonly ApplicationContext _context; // Контекст базы данных
        private readonly BusinessPartnerService _partnerService; // Сервис для работы с партнерами
        private BusinessPartner _partnerToEdit; // Партнер, который редактируется (null при добавлении)

        // Конструктор окна
        public PartnerEditWindow(ApplicationContext context, BusinessPartner partnerToEdit = null)
        {
            InitializeComponent(); // Инициализация компонентов XAML
            _context = context; // Установка контекста базы данных
            _partnerService = new BusinessPartnerService(); // Создание экземпляра сервиса
            _partnerToEdit = partnerToEdit; // Установка редактируемого партнера

            if (_partnerToEdit != null)
            {
                // Если партнер передан, настраиваем окно для редактирования
                Title = "Редактирование бизнес партнера"; // Изменение заголовка
                TypeComboBox.Text = _partnerToEdit.PartnerType; // Заполнение типа партнера
                NameTextBox.Text = _partnerToEdit.PartnerName; // Заполнение названия
                DirectorTextBox.Text = _partnerToEdit.DirectorName; // Заполнение имени директора
                EmailTextBox.Text = _partnerToEdit.ContactEmail; // Заполнение email
                PhoneTextBox.Text = _partnerToEdit.ContactPhone; // Заполнение телефона
                LegalAddressTextBox.Text = _partnerToEdit.LegalAddress; // Заполнение адреса
                RatingTextBox.Text = _partnerToEdit.PartnerRating.ToString(); // Заполнение рейтинга
            }
            else
            {
                // Если партнер не передан, настраиваем окно для добавления
                Title = "Добавление бизнес партнера"; // Установка заголовка
            }
        }

        // Обработчик нажатия кнопки "Сохранить"
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка обязательных полей и их корректности
                if (string.IsNullOrWhiteSpace(NameTextBox.Text))
                {
                    MessageBox.Show("Поле 'Название' не может быть пустым.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (string.IsNullOrWhiteSpace(TypeComboBox.Text))
                {
                    MessageBox.Show("Поле 'Тип' не может быть пустым.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (string.IsNullOrWhiteSpace(DirectorTextBox.Text))
                {
                    MessageBox.Show("Поле 'Директор' не может быть пустым.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (string.IsNullOrWhiteSpace(EmailTextBox.Text) || !IsValidEmail(EmailTextBox.Text))
                {
                    MessageBox.Show("Введите корректный адрес электронной почты (пример: example@mail.com).", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (string.IsNullOrWhiteSpace(PhoneTextBox.Text) || !IsValidPhone(PhoneTextBox.Text))
                {
                    MessageBox.Show("Введите номер телефона в формате +7XXXXXXXXXX (пример: +79504693322).", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (!int.TryParse(RatingTextBox.Text, out int rating) || rating <= 0)
                {
                    MessageBox.Show("Рейтинг должен быть положительным числом.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Получение выбранного типа партнера из ComboBox
                var selectedType = (TypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

                if (_partnerToEdit == null)
                {
                    // Создание нового партнера, если редактирование не выполняется
                    var newPartner = new BusinessPartner
                    {
                        PartnerType = selectedType,
                        PartnerName = NameTextBox.Text,
                        DirectorName = DirectorTextBox.Text,
                        ContactEmail = EmailTextBox.Text,
                        ContactPhone = PhoneTextBox.Text,
                        LegalAddress = LegalAddressTextBox.Text,
                        PartnerRating = rating
                    };

                    _partnerService.AddPartner(_context, newPartner); // Добавление нового партнера
                }
                else
                {
                    // Обновление существующего партнера
                    _partnerToEdit.PartnerType = selectedType;
                    _partnerToEdit.PartnerName = NameTextBox.Text;
                    _partnerToEdit.DirectorName = DirectorTextBox.Text;
                    _partnerToEdit.ContactEmail = EmailTextBox.Text;
                    _partnerToEdit.ContactPhone = PhoneTextBox.Text;
                    _partnerToEdit.LegalAddress = LegalAddressTextBox.Text; // Поле адреса обновляется
                    _partnerToEdit.PartnerRating = rating;

                    _partnerService.UpdatePartner(_context, _partnerToEdit); // Обновление партнера
                }

                this.DialogResult = true; // Установка результата диалога как успешного
            }
            catch (Exception ex)
            {
                // Обработка ошибок и вывод сообщения
                MessageBox.Show("Произошла ошибка: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Обработчик нажатия кнопки "Отменить"
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Закрытие окна без сохранения
        }

        // Метод проверки корректности email
        private bool IsValidEmail(string email)
        {
            // Проверка формата email с использованием регулярного выражения
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            // Формат: минимум один символ до @, домен после @, точка и зона
        }

        // Метод проверки корректности телефона
        private bool IsValidPhone(string phone)
        {
            // Проверка формата телефона (+7 и 10 цифр) с использованием регулярного выражения
            return Regex.IsMatch(phone, @"^\+\d{11}$");
            // Формат: начинается с +, затем 11 цифр (например, +79504693322)
        }
    }
}