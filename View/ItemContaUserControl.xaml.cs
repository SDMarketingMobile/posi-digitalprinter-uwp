using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using POSIDigitalPrinterAPIUtil.Controller;
using POSIDigitalPrinterAPIUtil.Model;
using System.Threading.Tasks;
using System.Net;
using POSIDigitalPrinter.Utils;

// O modelo de item de Controle de Usuário está documentado em https://go.microsoft.com/fwlink/?LinkId=234236

namespace POSIDigitalPrinter.View
{
    public sealed partial class ItemContaUserControl : UserControl
    {
        int qtdAdicional = 0;
        int widthAdicional = 0;
        int combinado = 0;
        int timerMinimo = 0;
        int timerMaximo = 0;
        int PrepareTime = 0;
        int minLimit = 0;
        int segLimit = 0;
        int seg = 0;
        int min = 0;
        private DispatcherTimer Timer;

        public int id { get; set; }

        Account contaData;
        AccountItem itemData;

        Model.Settings settings = Utils.SettingsUtil.Instance.GetSettings();
        AccountItemController accountItemController;

        Utils.SettingsUtil settingsUtil = Utils.SettingsUtil.Instance;
        Model.Settings localSettings;

        public ItemContaUserControl(AccountItem itemData, Account contaData)
        {
            this.InitializeComponent();

            accountItemController = new AccountItemController(settings.ApiIp, settings.ApiPort);

            this.itemData = itemData;
            this.id = itemData.Id;
            this.contaData = contaData;
            this.initialize();
        }

        public void initialize()
        {
            this.tbItemName.Text =  this.itemData.Quantity + " " + this.itemData.Name;
            this.PrepareTime = this.itemData.PrepareTime *this.itemData.Quantity;

            InitialStatusCode();

            obtertimerMaximo();
            obtertimerMinimo();

            if (this.itemData.Aditionals != null && this.itemData.Aditionals.Count > 0)
            {
                for(int i=0; i < this.itemData.Aditionals.Count; i++)
                {
                    AccountItemAditional adicionalData = this.itemData.Aditionals[i];

                    TextBlock tbAdicional = new TextBlock
                    {
                        Width = 284,
                        Text = "- " + adicionalData.Name,
                        FontSize = 18,
                        Padding = new Thickness(0)
                    };
                    this.stpAdicional.Children.Add(tbAdicional);
                }
            }

            if (this.itemData.ComboName != null && this.itemData.ComboName.Length > 0)
            {
                this.tbCombinado.Text = "(#" + this.itemData.ComboName + ")";
                this.combinado++;
                this.tbCombinado.Visibility = Visibility.Visible;
            }
            else
            {
                stpAdicional.Margin = new Thickness(25, 29, 304, 3);
            }

            if (itemData.Aditionals != null)
                qtdAdicional = itemData.Aditionals.Count;
            if (qtdAdicional <= 0)
            {
                stpAdicional.Visibility = Visibility.Collapsed;
            }
            if (qtdAdicional == 1)
            {
                widthAdicional = qtdAdicional * 27;
            }
            else
            {
                widthAdicional = qtdAdicional * 24;
            }
            ViewMode();

            if (PrepareTime >= 60)
            {
                minLimit = (PrepareTime / 60);
                segLimit = (PrepareTime % 60);
            }
            else
            {
                minLimit = 0;
                segLimit = PrepareTime;
            }
            tbPrepareTime.Text = minLimit.ToString().PadLeft(2, '0') + ":" + segLimit.ToString().PadLeft(2, '0');

            timerScreen();
        }

        public void ViewMode()
        {
            localSettings = settingsUtil.GetSettings(); 

            if (localSettings.ViewMode == Model.ViewMode.LIST)
            {
                this.Width = Window.Current.Bounds.Width - 40;
                this.Height = 80 + (qtdAdicional * 20);
                this.Margin = new Thickness(1);
            }
            else
            {
                this.Width = 429;
                this.Height = 38 + widthAdicional + (combinado * 11);
                this.Margin = new Thickness(1);
            }
        }

        private void InitialStatusCode()
        {
            switch (this.itemData.StatusCode)
            {
                case 0:
                    elipStatusItem.Fill = new SolidColorBrush(Colors.White);
                    break;
                case 1:
                    elipStatusItem.Fill = new SolidColorBrush(Colors.Blue);
                    break;
                case 2:
                    elipStatusItem.Fill = new SolidColorBrush(Colors.Green);
                    break;
            }
        }

        private void timerScreen()
        {
            Timer = new DispatcherTimer();
            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Tick += Timer_Tick;
            Timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            if(this.itemData.StatusCode == 1)
            {
                seg++;
                if (seg == 60)
                {
                    min++;
                    seg = 0;
                }
                this.tbExecutingTime.Text = min.ToString().PadLeft(2, '0') + ":" + seg.ToString().PadLeft(2, '0');
            }
        }

        public void obtertimerMinimo()
        {
            AccountItem item1 = this.contaData.Items[0];
            timerMinimo = item1.PrepareTime * itemData.Quantity;
            for (int i = 0; i < this.contaData.Items.Count; i++)
            {
                AccountItem item = this.contaData.Items[i];
                if (item.PrepareTime * item.Quantity < timerMinimo)
                {
                    timerMinimo = item.PrepareTime * item.Quantity;
                }
            }
        }

        public void obtertimerMaximo()
        {
            AccountItem itemData = this.contaData.Items[0];
            timerMaximo = itemData.PrepareTime * itemData.Quantity;
            for (int i = 0; i < this.contaData.Items.Count; i++)
            {
                AccountItem item = this.contaData.Items[i];
                if (item.PrepareTime * item.Quantity > timerMaximo)
                {
                    timerMaximo = item.PrepareTime * item.Quantity;
                }
            }
        }

        public async Task<int> UpdateStatusItem()
        {
            this.itemData.StatusCode++;

            if (this.itemData.StatusCode == 0)
            {
                elipStatusItem.Fill = new SolidColorBrush(Colors.White);
            }
            else if (this.itemData.StatusCode == 1) // quando inicia a producao do item...
            {
                var result = await accountItemController.reportBeginPreparation(this.contaData, this.itemData);
                if (result.StatusCode.Equals(HttpStatusCode.OK))
                {
                    elipStatusItem.Fill = new SolidColorBrush(Colors.Blue);
                    Timer.Start();
                }
                else
                {
                    this.itemData.StatusCode--;
                }
            }
            else if (this.itemData.StatusCode == 2) // quando finaliza a producao do item...
            {
                var result = await accountItemController.reportEndPreparation(this.contaData, this.itemData);
                if (result.StatusCode.Equals(HttpStatusCode.OK))
                {
                    elipStatusItem.Fill = new SolidColorBrush(Colors.Green);
                    Timer.Stop();

                    localSettings = settingsUtil.GetSettings();

                    string account = Newtonsoft.Json.JsonConvert.SerializeObject(contaData);
                    Account clone = Newtonsoft.Json.JsonConvert.DeserializeObject<Account>(account);
                    clone.Items.Clear();
                    clone.Items.Add(itemData);


                    if (localSettings.ScreenType == Model.ScreenType.PRODUCTION)
                    {
                        string contaEntrega = Newtonsoft.Json.JsonConvert.SerializeObject(clone);

                        using (var client = new SocketClient())
                        {
                            client.Connect(localSettings.DeliveryDeviceIp, localSettings.DeliveryDevicePort);
                            client.sendData(contaEntrega); // converter objeto p/ json
                        }
                    }
                }
                else
                {
                    this.itemData.StatusCode--;
                }
            }

            return itemData.StatusCode;
        }

        public void ColorItem()
        {
            grid.Background = new SolidColorBrush(Colors.Orange);
            tbItemName.Foreground = new SolidColorBrush(Colors.White);
            tbCombinado.Foreground = new SolidColorBrush(Colors.White);
            tbExecutingTime.Foreground = new SolidColorBrush(Colors.White);
            tbPrepareTime.Foreground = new SolidColorBrush(Colors.White);

            stpAdicional.Children.Clear();

            if (this.itemData.Aditionals != null && this.itemData.Aditionals.Count > 0)
            {
                for (int i = 0; i < this.itemData.Aditionals.Count; i++)
                {
                    AccountItemAditional adicionalData = this.itemData.Aditionals[i];

                    TextBlock tbAdicional = new TextBlock
                    {
                        Width = 284,
                        Text = "- " + adicionalData.Name,
                        FontSize = 18,
                        Foreground = new SolidColorBrush(Colors.White),
                        Padding = new Thickness(0)
                    };
                    this.stpAdicional.Children.Add(tbAdicional);
                }
            }
        }

        public void DefaultColorItem(int timerInicial)
        {
            if (timerInicial <= (timerMinimo * 0.75))
            {
                grid.Background = new SolidColorBrush(Colors.LightGreen);
            }
            else if (timerInicial >= timerMinimo)
            {
                grid.Background = new SolidColorBrush(Colors.LightCoral);
            }
            else
            {
                grid.Background = new SolidColorBrush(Colors.LightYellow);
            }
            tbItemName.Foreground = new SolidColorBrush(Colors.Black);
            tbCombinado.Foreground = new SolidColorBrush(Colors.Black);
            tbExecutingTime.Foreground = new SolidColorBrush(Colors.Black);
            tbPrepareTime.Foreground = new SolidColorBrush(Colors.Black);

            stpAdicional.Children.Clear();

            if (this.itemData.Aditionals != null && this.itemData.Aditionals.Count > 0)
            {
                for (int i = 0; i < this.itemData.Aditionals.Count; i++)
                {
                    AccountItemAditional adicionalData = this.itemData.Aditionals[i];

                    TextBlock tbAdicional = new TextBlock
                    {
                        Width = 284,
                        Text = "- " + adicionalData.Name,
                        FontSize = 18,
                        Foreground = new SolidColorBrush(Colors.Black),
                        Padding = new Thickness(0)
                    };
                    this.stpAdicional.Children.Add(tbAdicional);
                }
            }
        }
    }

}
