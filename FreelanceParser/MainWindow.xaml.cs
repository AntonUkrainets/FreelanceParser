using FreelanceParser.Controllers;
using FreelanceParser.Core;
using FreelanceParser.Core.Freelances;
using FreelanceParser.Core.Model;
using FreelanceParser.Data;
using FreelanceParser.Factories;
using FreelanceParser.FreelancesWorkers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace FreelanceParser
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ParserWorker<FreelanceHuntItem> freelanceHuntParser;
        private readonly ParserWorker<FlRuItem> flParser;

        private IEnumerable<FreelanceHuntItem> freelanceHuntTasks;
        private IEnumerable<FlRuItem> flRuTasks;

        private DatabaseWorker DatabaseWorker;

        private FreelanceRuAppService freelanceRuWorker;
        private FreelanceHuntAppService freelanceHuntWorker;

        private ClientsAppService clientController;
        
        private readonly Timer timer;

        private static TelegramBotClient botClient;

        private int currentProviderIndex = 1;

        private IDataContextFactory dataContextFactory;
        private TelegramBotFactory telegramBotFactory;

        public MainWindow()
        {
            InitializeComponent();

            dataContextFactory = new DataContextFactory();
            telegramBotFactory = new TelegramBotFactory();

            DatabaseWorker = new DatabaseWorker(dataContextFactory);

            DatabaseWorker.EnsureDatabase();

            botClient = telegramBotFactory.Create();

            freelanceRuWorker = new FreelanceRuAppService(dataContextFactory, botClient);
            freelanceHuntWorker = new FreelanceHuntAppService(dataContextFactory, botClient);

            #region declare timer
            timer = new Timer(TimeSpan.FromSeconds(10).TotalMilliseconds);
            timer.Enabled = true;
            timer.Elapsed += Timer_Elapsed;
            #endregion

            botClient.StartReceiving();
            botClient.OnMessage += Bot_GettingMessage;

            #region declare freelanceHunt
            freelanceHuntTasks = new List<FreelanceHuntItem>();
            freelanceHuntParser = new ParserWorker<FreelanceHuntItem>(new FreelanceHuntParser());
            freelanceHuntParser.OnCompleted += FreelanceHuntParser_OnCompleted;
            #endregion

            #region declare freelanceRu
            flRuTasks = new List<FlRuItem>();
            flParser = new ParserWorker<FlRuItem>(new FLParser());
            flParser.OnCompleted += FlParser_OnCompleted;
            #endregion
        }

        private void Bot_GettingMessage(object sender, MessageEventArgs e)
        {
            clientController = new ClientsAppService(dataContextFactory);

            int userId = (int)e.Message.Chat.Id;
            string userName = e.Message.Chat.FirstName;
            string userCommand = e.Message.Text;

            clientController.ProcessUserCommand(userId, userName, userCommand);
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ReloadProvider(currentProviderIndex);
        }

        private void FlParser_OnCompleted(IEnumerable<FlRuItem> tasks)
        {
            flRuTasks = tasks;

            freelanceRuWorker.SaveNewTasks(tasks);

            

            var convertedTasks = ConvertToNormalView(tasks);

            Application.Current.Dispatcher.Invoke(
                () =>
                {
                    FillListView(convertedTasks);
                });
        }

        private IEnumerable<FlRuItem> ConvertToNormalView(IEnumerable<FlRuItem> tasks)
        {
            ICollection<FlRuItem> newTasks = new List<FlRuItem>();

            foreach (var task in tasks)
            {
                string countOfBeats = TrimCountOfBeats(task.CountBeats);
                string date = TrimDate(task.Date);

                FlRuItem newTask = new FlRuItem
                {
                    Id = task.Id,
                    Title = task.Title,
                    Price = task.Price,
                    CountBeats = countOfBeats,
                    Date = date,
                    Uri = task.Uri,
                };

                newTasks.Add(newTask);
            }

            return newTasks;
        }

        private string TrimCountOfBeats(string countOfBeats)
        {
            int index = countOfBeats.IndexOf(' ');
            string count = countOfBeats.Substring(index);

            return count;
        }

        private string TrimDate(string date)
        {
            int index = date.IndexOf(' ') + 1;
            var trimedDate = date.Substring(index);

            return trimedDate;
        }

        private void FreelanceHuntParser_OnCompleted(IEnumerable<FreelanceHuntItem> tasks)
        {
            freelanceHuntTasks = tasks;

            freelanceHuntWorker.SaveNewTasks(tasks);

            Application.Current.Dispatcher.Invoke(
                () =>
                {
                    FillListView(freelanceHuntTasks);
                });
        }

        private void FillListView(IEnumerable<TaskItem> tasks)
        {
            TasksListView.Items.Clear();

            foreach (var task in tasks.OrderByDescending(x => x.Id))
                TasksListView.Items.Add(task);
        }

        private void ReloadDataFromFreelanceHunt()
        {
            freelanceHuntParser.Settings = new FreelanceHuntSettings();
            freelanceHuntParser.Start();
        }

        private void ReloadDataFromFlRu()
        {
            flParser.Settings = new FLSettings();
            flParser.Start();
        }

        private void FreelancesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentProviderIndex = FreelancesComboBox.SelectedIndex + 1;

            ReloadProvider(currentProviderIndex);
        }

        private void ReloadProvider(int providerIndex)
        {
            switch (providerIndex)
            {
                case 1:
                    ReloadDataFromFreelanceHunt();
                    break;

                case 2:
                    ReloadDataFromFlRu();
                    break;

                default:
                    break;
            }
        }

        private void TasksListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var url = TasksListView.SelectedItem as TaskItem;
            Process.Start(url.Uri);
        }

        private IEnumerable<Client> GetClients()
        {
            return DatabaseWorker.GetClientsFromDatabase();
        }

        private void CheckButton_Click(object sender, RoutedEventArgs e)
        {
            ClientsListView.Items.Clear();

            var clients = GetClients();

            foreach (var client in clients)
            {
                if (client.IsActive == true)
                    ClientsListView.Items.Add(client);
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            var client = ClientsListView.SelectedItem as Client;
            var task = TasksListView.SelectedItem as TaskItem;

            if (client == null)
                return;

            if (task == null)
                return;

            if (client.IsActive == false)
                return;

            SendTasksToTheClient(client, task);
        }

        private async void SendTasksToTheClient(Client client, TaskItem task)
        {
            if (task == null)
                return;

            if (client.IsActive == false)
                return;

            var message = task.Uri;
            await botClient.SendTextMessageAsync(client.Id, message);
        }
    }
}