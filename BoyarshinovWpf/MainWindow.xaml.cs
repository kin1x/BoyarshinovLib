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
using System.Windows.Navigation;
using System.Windows.Shapes;
using BoyarshinovLib;

namespace BoyarshinovWpf
{
    // Основной класс окна приложения для управления бизнес-партнерами и продажами
    public partial class MainWindow : Window
    {
        private readonly ApplicationContext _context; // Контекст базы данных
        private BusinessPartnerService partnerService; // Сервис для работы с партнерами и продажами

        // Конструктор окна
        public MainWindow()
        {
            InitializeComponent(); // Инициализация компонентов XAML
            _context = new ApplicationContext(); // Создание контекста базы данных
            partnerService = new BusinessPartnerService(); // Создание экземпляра сервиса
            PartnersList.SelectionChanged += (s, e) => LoadSale(); // Подписка на событие изменения выбора партнера
            LoadPartners(); // Загрузка списка партнеров
            LoadSale(); // Загрузка продаж для выбранного партнера
        }

        // Метод загрузки списка партнеров в ListBox
        private void LoadPartners()
        {
            var selectedPartner = PartnersList.SelectedItem as BusinessPartnerViewModel; // Сохранение текущего выбранного партнера
            partnerService.UpdateDiscounts(_context); // Обновление скидок для всех партнеров
            var partners = partnerService.LoadPartners(_context); // Получение списка партнеров с их скидками
            PartnersList.ItemsSource = partners; // Установка источника данных для списка
            if (selectedPartner != null)
            {
                // Восстановление выбора партнера после обновления списка
                PartnersList.SelectedItem = partners.FirstOrDefault(p => p.PartnerName == selectedPartner.PartnerName);
            }
        }

        // Обработчик события выхода из приложения
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Закрытие окна
        }

        // Метод загрузки продаж для выбранного партнера в DataGrid
        private void LoadSale()
        {
            var selectedPartner = PartnersList.SelectedItem as BusinessPartnerViewModel; // Получение выбранного партнера
            if (selectedPartner != null)
            {
                var partner = partnerService.GetPartnerByProperties(_context, selectedPartner.PartnerName); // Поиск партнера по имени
                if (partner != null)
                {
                    SalesDataGrid.ItemsSource = partnerService.GetSales(_context, partner); // Установка продаж в таблицу
                }
            }
            else
            {
                SalesDataGrid.ItemsSource = null; // Очистка таблицы, если партнер не выбран
            }
        }

        // Обработчик события добавления нового партнера
        private void AddPartner_Click(object sender, RoutedEventArgs e)
        {
            PartnerEditWindow partnerEditWindow = new PartnerEditWindow(_context); // Создание окна редактирования
            partnerEditWindow.Owner = this; // Установка текущего окна как владельца
            if (partnerEditWindow.ShowDialog() == true) // Отображение окна как диалога
            {
                LoadPartners(); // Обновление списка партнеров после добавления
            }
        }

        // Обработчик события редактирования выбранного партнера
        private void EditPartner_Click(object sender, RoutedEventArgs e)
        {
            var selectedPartner = PartnersList.SelectedItem as BusinessPartnerViewModel; // Получение выбранного партнера
            if (selectedPartner != null)
            {
                var partner = partnerService.GetPartnerByProperties(_context, selectedPartner.PartnerName); // Поиск партнера
                PartnerEditWindow partnerEditWindow = new PartnerEditWindow(_context, partner); // Окно редактирования
                partnerEditWindow.Owner = this; // Установка владельца
                if (partnerEditWindow.ShowDialog() == true) // Отображение диалога
                {
                    LoadPartners(); // Обновление списка после редактирования
                }
            }
        }

        // Обработчик события удаления выбранного партнера
        private void DeletePartner_Click(object sender, RoutedEventArgs e)
        {
            var selectedPartner = PartnersList.SelectedItem as BusinessPartnerViewModel; // Получение выбранного партнера
            Window owner = Window.GetWindow(this); // Получение текущего окна как владельца диалога
            if (selectedPartner != null)
            {
                var partner = partnerService.GetPartnerByProperties(_context, selectedPartner.PartnerName); // Поиск партнера
                var result = MessageBox.Show(owner, "Вы действительно хотите удалить выбранного бизнес партнера?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question); // Подтверждение удаления
                if (result == MessageBoxResult.Yes)
                {
                    partnerService.DeletePartner(_context, partner); // Удаление партнера
                }
            }
            LoadPartners(); // Обновление списка после удаления
        }

        // Обработчик события добавления новой продажи
        private void AddSale_Click(object sender, RoutedEventArgs e)
        {
            var selectedPartner = PartnersList.SelectedItem as BusinessPartnerViewModel; // Получение выбранного партнера
            if (selectedPartner != null)
            {
                var partner = partnerService.GetPartnerByProperties(_context, selectedPartner.PartnerName); // Поиск партнера
                if (partner != null)
                {
                    SaleEditWindow saleEditWindow = new SaleEditWindow(_context, null, partner); // Окно добавления продажи
                    saleEditWindow.Owner = this; // Установка владельца
                    if (saleEditWindow.ShowDialog() == true) // Отображение диалога
                    {
                        LoadSale(); // Обновление таблицы продаж
                        LoadPartners(); // Обновление списка партнеров (скидки могут измениться)
                    }
                }
            }
        }

        // Обработчик события редактирования выбранной продажи
        private void EditSale_Click(object sender, RoutedEventArgs e)
        {
            var selectedPartner = PartnersList.SelectedItem as BusinessPartnerViewModel; // Получение выбранного партнера
            var selectedSale = SalesDataGrid.SelectedItem as PartnerSale; // Получение выбранной продажи
            if (selectedPartner != null && selectedSale != null)
            {
                var partner = partnerService.GetPartnerByProperties(_context, selectedPartner.PartnerName); // Поиск партнера
                if (partner != null)
                {
                    SaleEditWindow saleEditWindow = new SaleEditWindow(_context, selectedSale, partner); // Окно редактирования
                    saleEditWindow.Owner = this; // Установка владельца
                    if (saleEditWindow.ShowDialog() == true) // Отображение диалога
                    {
                        LoadSale(); // Обновление таблицы продаж
                        LoadPartners(); // Обновление списка партнеров (скидки могут измениться)
                    }
                }
            }
        }

        // Обработчик события удаления выбранной продажи
        private void DeleteSale_Click(object sender, RoutedEventArgs e)
        {
            var selectedSale = SalesDataGrid.SelectedItem as PartnerSale; // Получение выбранной продажи
            if (selectedSale != null)
            {
                var result = MessageBox.Show("Вы действительно хотите удалить выбранную продажу?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question); // Подтверждение удаления
                if (result == MessageBoxResult.Yes)
                {
                    partnerService.DeleteSale(_context, selectedSale); // Удаление продажи
                    LoadSale(); // Обновление таблицы продаж
                    LoadPartners(); // Обновление списка партнеров (скидки могут измениться)
                }
            }
        }
    }
}