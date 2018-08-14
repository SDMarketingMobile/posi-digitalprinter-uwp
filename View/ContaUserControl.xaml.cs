using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI;
using Windows.UI.Popups;
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
using POSIDigitalPrinterAPIUtil.Enumerator;

// O modelo de item de Controle de Usuário está documentado em https://go.microsoft.com/fwlink/?LinkId=234236

namespace POSIDigitalPrinter.View
{
    public sealed partial class ContaUserControl : UserControl
    {
        Utils.SettingsUtil settingsUtil = Utils.SettingsUtil.Instance;
        Model.Settings localSettings;

        Account contaData;
        int navIndexItem = 0;
        int timerInicial = -1;
        int timerMinimo = 0;
        int timerMaximo = 0;
        int seg = 0;
        int min = 0;
        bool statusEnter = false;
        private DispatcherTimer Timer;

        public ContaUserControl(Account contaData)
        {
            this.InitializeComponent();
            this.contaData = contaData;
            this.localSettings = settingsUtil.GetSettings();
            this.Initialize();
        }

        public void Initialize()
        {
            this.tbconta.Text = AccountTypeExtensions.GetName(this.contaData.Type) + ": " + this.contaData.Number.ToString().PadLeft(4, '0');

            obtertimerMaximo();
            obtertimerMinimo();

            ViewMode();
            
            timerScreen();
        }

        public void ViewMode()
        {
            localSettings = settingsUtil.GetSettings();

            if (localSettings.ViewMode == Model.ViewMode.LIST)
            {
                this.Width = Window.Current.Bounds.Width - 30;
                this.Height = 315;
                this.Margin = new Thickness(2);
                for (int i = 0; i <= (ctrlListView.Items.Count - 1); i++)
                {
                    ItemContaUserControl itemUC = (ItemContaUserControl)ctrlListView.Items[i];
                    if (ctrlListView.Items.Count >= 1)
                    {
                        itemUC.ViewMode();
                    }
                }
            }
            else
            {
                this.Width = 440;
                this.Height = 315;
                this.Margin = new Thickness(2);
            }

            for (int i = 0; i <= (ctrlListView.Items.Count - 1); i++)
            {
                ItemContaUserControl itemUC = (ItemContaUserControl)ctrlListView.Items[i];
                if (ctrlListView.Items.Count >= 1)
                {
                    itemUC.ViewMode();
                }
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
            timerInicial++;
            seg++;
            if (seg == 60)
            {
                min++;
                seg = 0;
            }
            this.tbTimerScreen.Text = min.ToString().PadLeft(2, '0') + ":" + seg.ToString().PadLeft(2, '0');

            AddScreenItem();

            for(int i=0; i <= contaData.Items.Count; i++)
            {
                if (ctrlListView.SelectedIndex != i)
                {
                    ItemContaUserControl itemUC = (ItemContaUserControl)this.ctrlListView.Items[i];
                    if (itemUC != null)
                        itemUC.DefaultColorItem(timerInicial);
                }
                else
                {
                    if (statusEnter == true)
                    {
                        ItemContaUserControl itemUC = (ItemContaUserControl)this.ctrlListView.Items[i];
                        itemUC.ColorItem();
                    }
                    else
                    {
                        ItemContaUserControl itemUC = (ItemContaUserControl)this.ctrlListView.Items[i];
                        itemUC.DefaultColorItem(timerInicial);
                    }
                }
            }
        }

        private void AddScreenItem()
        {

            foreach (AccountItem itemContaData in this.contaData.Items)
            {   
                int visebleTimer = 0;
                visebleTimer = timerMaximo - itemContaData.PrepareTime * itemContaData.Quantity;
                if (visebleTimer == timerInicial)
                {
                    ItemContaUserControl itemContaUC = new ItemContaUserControl(itemContaData, contaData);
                    this.ctrlListView.Items.Add(itemContaUC);
                }
            }
        }

        public async Task<Boolean> updateStatusItem()
        {
            Boolean removeFromScreen = false;

            if (ctrlListView.SelectedIndex != -1)
            {
                ItemContaUserControl itemUC = (ItemContaUserControl)this.ctrlListView.SelectedItem;

                foreach (AccountItem itemContaData in this.contaData.Items)
                {
                    if (itemContaData.Id == itemUC.id)
                    {
                        itemContaData.StatusCode = await itemUC.UpdateStatusItem();
                        
                        if (itemContaData.StatusCode == 2)
                        {
                            ctrlListView.Items.RemoveAt(ctrlListView.SelectedIndex);

                            if(ctrlListView.Items.Count != 0)
                            {
                                if (navIndexItem == 0)
                                {
                                    SelectItem();
                                }
                                else
                                {
                                    navIndexItem -= 1;
                                    SelectItem();
                                }
                            }
                        }
                    }
                }
            }

            int qtdFinalizados = 0;

            foreach(AccountItem itemContaData in this.contaData.Items)
            {
                if (itemContaData.StatusCode == 2)
                    qtdFinalizados++;
            }

            if (qtdFinalizados == this.contaData.Items.Count)
                removeFromScreen = true;

            return removeFromScreen;
        }

        public void obtertimerMinimo()
        {
            AccountItem itemData = this.contaData.Items[0];
            timerMinimo = itemData.PrepareTime * itemData.Quantity;
            for (   int i = 0; i < this.contaData.Items.Count; i++)
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

        public void ColorConta()
        {
            grid.Background = new SolidColorBrush(Colors.DarkOrange);
            tbconta.Foreground = new SolidColorBrush(Colors.White);
            tbTimerScreen.Foreground = new SolidColorBrush(Colors.White);
        }

        public void DefaultfColorConta()
        {
            grid.Background = new SolidColorBrush(Colors.Gray);
            tbconta.Foreground = new SolidColorBrush(Colors.Black);
            tbTimerScreen.Foreground = new SolidColorBrush(Colors.Black);
        }

        public void ColorItem()
        {
            for (int i = 0; i <= ctrlListView.Items.Count; i++)
            {

                if (navIndexItem == ctrlListView.SelectedIndex)
                {
                    ItemContaUserControl contaUCslc = (ItemContaUserControl)this.ctrlListView.Items[navIndexItem];
                    contaUCslc.ColorItem();
                    if (statusEnter == false)
                    {
                        ItemContaUserControl contaUCslc1 = (ItemContaUserControl)this.ctrlListView.Items[navIndexItem];
                        contaUCslc1.DefaultColorItem(timerInicial);
                    }
                }
            }
        }

        private void ctrlListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            for (int i = 0; i <= (ctrlListView.Items.Count - 1); i++)
            {
                if (navIndexItem == i)
                {
                    ItemContaUserControl itemUC = (ItemContaUserControl)this.ctrlListView.Items[i];
                    itemUC.ColorItem();
                }
                else
                {
                    ItemContaUserControl itemUC = (ItemContaUserControl)this.ctrlListView.Items[i];
                    itemUC.DefaultColorItem(timerInicial);
                }
            }
        }

        public void StatusEnter(bool enter)
        {
            statusEnter = enter;
        }

        public void SelectItem()
        {
            ctrlListView.Focus(FocusState.Programmatic);
            if (ctrlListView.Items.Count > 0)
                ctrlListView.SelectedIndex = navIndexItem;
        }

        public void NavItem(int estado)
        {
            if(estado == 1)
            {
                navIndexItem++;
                NavLimit();
                SelectItem();
            }

            if(estado == 2)
            {
                navIndexItem--;
                NavLimit();
                SelectItem();
            }
            if(estado == 3)
            {
                navIndexItem = 0;
                NavLimit();
                SelectItem();
                ColorItem();
            }
        }

        private void NavLimit()
        {
            if ((navIndexItem + 1) > ctrlListView.Items.Count)
                navIndexItem = 0;
            else if (navIndexItem < 0)
                navIndexItem = (ctrlListView.Items.Count - 1);
        }
    }
}
