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
        bool statusViewMode = false;
        private DispatcherTimer Timer;

        public int id { get; set; }

        Account contaData;
        AccountItem itemData;
        Model.ScreenSetting screenData;

        Model.Settings settings = Utils.SettingsUtil.Instance.GetSettings();
        AccountItemController accountItemController;

        public ItemContaUserControl(AccountItem itemData, Account contaData, Model.ScreenSetting screenData)
        {
            this.InitializeComponent();

            accountItemController = new AccountItemController(settings.ApiIp, settings.ApiPort);

            this.itemData = itemData;
            this.id = itemData.Id;
            this.contaData = contaData;
            this.screenData = screenData;
            this.initialize();
        }

        public void initialize()
        {
            this.tbItemName.Text =  this.itemData.Quantity + " " + this.itemData.Name;
            this.PrepareTime = this.itemData.PrepareTime *this.itemData.Quantity;

            obtertimerMaximo();
            obtertimerMinimo();

            if (this.itemData.Aditionals != null && this.itemData.Aditionals.Count > 0)
            {
                for(int i=0; i < this.itemData.Aditionals.Count; i++)
                {
                    AccountItemAditional adicionalData = this.itemData.Aditionals[i];

                    TextBlock tbAdicional = new TextBlock();
                    tbAdicional.Width = 284;
                    tbAdicional.Text = "- " + adicionalData.Name;
                    tbAdicional.FontSize = 18;
                    tbAdicional.Padding = new Thickness(0);
                    this.stpAdicional.Children.Add(tbAdicional);
                }
            }

            if (this.itemData.ComboName != null && this.itemData.ComboName.Length > 0)
            {
                this.tbCombinado.Text = "(#" + this.itemData.ComboName + ")";
                this.combinado++;
                this.tbCombinado.Visibility = Visibility.Visible;
            }

            if (itemData.Aditionals != null)
                qtdAdicional = itemData.Aditionals.Count;
            if (qtdAdicional == 1)
            {
                widthAdicional = qtdAdicional * 35;
            }
            else
            {
                widthAdicional = qtdAdicional * 27;
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

        public void ViewModeStatus(bool vMode)
        {
            statusViewMode = vMode;

            ViewMode();
        }

        public void ViewMode()
        {
            statusViewMode = screenData.ViewMode;

            if (statusViewMode == true)
            {
                this.Width = Window.Current.Bounds.Width - 45;
                this.Height = 60 + (qtdAdicional * 22);
                this.Margin = new Thickness(1);
            }
            else
            {
                this.Width = 429;
                this.Height = 38 + widthAdicional + (combinado * 13);
                this.Margin = new Thickness(1);
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
            else if (this.itemData.StatusCode == 1)
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
            else if (this.itemData.StatusCode == 2)
            {
                var result = await accountItemController.reportEndPreparation(this.contaData, this.itemData);
                if (result.StatusCode.Equals(HttpStatusCode.OK))
                {
                    elipStatusItem.Fill = new SolidColorBrush(Colors.Green);
                    Timer.Stop();
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
        }

    }

}
