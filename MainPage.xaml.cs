﻿using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.System;
using Windows.UI;
using POSIDigitalPrinterAPIUtil.Model;
using POSIDigitalPrinter.Printer;
using POSIDigitalPrinter.Enumerator;
using System.Threading;
using Windows.Devices.SerialCommunication;
using POSIDigitalPrinterAPIUtil.Enumerator;
using POSIDigitalPrinter.Utils;
using Windows.Networking.Connectivity;
using System.Linq;
using Windows.Networking;
using System.Collections.ObjectModel;

namespace POSIDigitalPrinter
{
    public sealed partial class MainPage : Page
    {
        int configView = 0;
        bool statusEnter = false;
        bool enterUpdate = false;
        int tpTela = 0;
        int tipoTela = 0;
        int navIndexConta = 0;
        int navIndexConfig = 0;
        int qtdConta = 0;

        bool ipConfigA = false;
        int ipA1, ipA2, ipA3, ipA4, ipIndexA;

        bool prtConfigA = false;
        int prtA1, prtA2, prtA3, prtA4, prtA5 , prtIndexA;

        bool prtConfigS = false;
        int prtS1, prtS2, prtS3, prtS4, prtS5 , prtIndexS;

        bool ipConfigD = false;
        int ipD1, ipD2, ipD3, ipD4, ipIndexD;

        bool prtConfigD = false;
        int prtD1, prtD2, prtD3, prtD4, prtD5, prtIndexD;

        string ipLocal;

        Utils.SettingsUtil settingsUtil = Utils.SettingsUtil.Instance;
        Model.Settings localSettings;

        public MainPage()
        {
            this.InitializeComponent();
            localSettings = settingsUtil.GetSettings();

            if(localSettings.ApiIp == null)
            {
                var settings = new Model.Settings();
                settings.ApiIp = "192.168.0.5";
                settings.ApiPort = 3000;
                settings.LocalSocketPort = 9000;
                settings.ViewMode = Model.ViewMode.GRID;
                settings.ScreenType = Model.ScreenType.PRODUCTION;
                if (localSettings.DeliveryDeviceIp == null)
                {
                    settings.DeliveryDeviceIp = "192.168.0.0";
                    settings.DeliveryDevicePort = 0;
                }

                settingsUtil.SaveSettings(settings);
            }

            LoadConfig();

            if (localSettings != null && localSettings.LocalSocketPort > 0)
            {
                this.StartSocketServer();
            }
            //this.testPrint();
        }

        public async void testPrint()
        {
            /*using (var client = new SocketClient())
            {
                client.Connect(localSettings.DeliveryDeviceIp, localSettings.DeliveryDevicePort);
                client.sendData(""); // converter objeto p/ json
            }*/

            var opt = new PrinterImpl();
            List<SerialDevice> devs = await opt.ListSerialPort();
            if (devs.Count > 0)
            {
                Account accoutMock = new Account();

                accoutMock.Type = AccountType.SALAO;
                accoutMock.Number = 5;
                accoutMock.Items = new List<AccountItem>();

                AccountItem i1 = new AccountItem();
                i1.Id = 0;
                i1.Name = "DUPLO BACON";
                i1.Quantity = 1;
                i1.PrepareTime = 50;
                i1.ComboName = "DUPLO BACON";
                i1.Aditionals = new List<AccountItemAditional>();

                AccountItemAditional a1 = new AccountItemAditional();
                a1.Name = "HAMBURGUE";

                AccountItemAditional a2 = new AccountItemAditional();
                a2.Name = "BATATAS";

                i1.Aditionals.Add(a1);
                i1.Aditionals.Add(a2);
                accoutMock.Items.Add(i1);

                AccountItem i2 = new AccountItem();
                i2.Id = 1;
                i2.Name = "LANCHE";
                i2.Quantity = 1;
                i2.PrepareTime = 48;
                i2.Aditionals = new List<AccountItemAditional>();

                AccountItemAditional a3 = new AccountItemAditional();
                a3.Name = "BATATAS";

                i2.Aditionals.Add(a3);

                accoutMock.Items.Add(i2);

                AccountItem i3 = new AccountItem();
                i3.Id = 2;
                i3.Name = "LANCHE";
                i3.Quantity = 1;
                i3.PrepareTime = 46;

                accoutMock.Items.Add(i3);

                AccountItem i4 = new AccountItem();
                i4.Id = 3;
                i4.Name = "asdasdLANCHE";
                i4.Quantity = 1;
                i4.PrepareTime = 46;

                accoutMock.Items.Add(i4);

                AccountItem i5 = new AccountItem();
                i5.Id = 5;
                i5.Name = "asdasdLANCHE";
                i5.Quantity = 1;
                i5.PrepareTime = 46;

                accoutMock.Items.Add(i5);

                this.printReceipt(devs[0], accoutMock);
            }
        }

        private Utils.SocketServer socket;

        private void StartSocketServer()
        {
            this.socket = new Utils.SocketServer(localSettings.LocalSocketPort);
            this.socket.Start();
            this.socket.OnError += Socket_OnError; ;
            this.elipStatusSocketConnection.Fill = new SolidColorBrush(Colors.Green);
        }

        private void Socket_OnError(string message)
        {
            throw new NotImplementedException();
        }

        private async void Socket_OnDataReceived(string data)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine(data);
                this.socket.Send("OBRIGADO...");

                Account account = Newtonsoft.Json.JsonConvert.DeserializeObject<Account>(data);

                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                () =>
                {
                    this.addAccountView(account);
                }
                );
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        public void printReceipt(SerialDevice serialDevice, Account account)
        {
            using (var daruma = new DarumaImpl(serialDevice))
            {
                daruma.Print("------------------------------------------------\n");
                daruma.Print("Data: 07/06/2017 15:50:23\n");
                daruma.Font(Fonts.FONT_EXPANDED);
                daruma.FontStyle(Enumerator.Style.BOLD);
                daruma.Print(account.Type.GetName() + ": "+ account.Number.ToString().PadLeft(4, '0') + "\n");
                daruma.Font(Fonts.FONT_NORMAL);
                daruma.FontStyle(Enumerator.Style.PLAIN);
                daruma.Print("------------------------------------------------\n");

                daruma.Print("\n");

                daruma.FontStyle(Enumerator.Style.BOLD);
                daruma.Print("Qtd    Descricao\n");
                daruma.FontStyle(Enumerator.Style.PLAIN);
                daruma.Font(Fonts.FONT_LARGE);

                foreach(AccountItem item in account.Items)
                {
                    daruma.Print(item.Quantity.ToString().PadLeft(2, '0') + "    "+ item.Name);
                    if (!String.IsNullOrEmpty(item.ComboName))
                    {
                        daruma.Print(" (" + item.ComboName + ")");
                    }
                    daruma.Print("\n");

                    if (item.Aditionals != null && item.Aditionals.Count > 0)
                    {
                        foreach (AccountItemAditional aditional in item.Aditionals)
                        {
                            daruma.Print("      - " + aditional.Name + "\n");
                        }
                    }
                }

                daruma.Font(Fonts.FONT_NORMAL);
                daruma.Print("------------------------------------------------\n");
                daruma.Forward(1);
                daruma.Beep();
            }
        }

        DispatcherTimer timer;
        int BlinkState = 0;

        public void BlinkSocketDataReceiving()
        {
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 200);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            BlinkState++;
            var colorGreen = new SolidColorBrush(Colors.Green);
            var colorYellow = new SolidColorBrush(Colors.Yellow);

            switch(BlinkState)
            {
                case 0:
                    this.elipStatusSocketData.Fill = colorYellow;
                    break;
                case 1:
                    this.elipStatusSocketData.Fill = colorGreen;
                    break;
                case 2:
                    this.elipStatusSocketData.Fill = colorYellow;
                    break;
                case 3:
                    this.elipStatusSocketData.Fill = colorGreen;
                    break;
                case 4:
                    this.elipStatusSocketData.Fill = colorYellow;
                    break;
                case 5:
                    this.elipStatusSocketData.Fill = colorGreen;
                    break;
                case 6:
                    this.elipStatusSocketData.Fill = colorYellow;
                    break;
                case 7:
                    this.elipStatusSocketData.Fill = colorGreen;
                    break;
            }

            if(BlinkState == 8)
            {
                timer.Stop();
                BlinkState = 0;
                this.elipStatusSocketData.Fill = colorGreen;
            }
        }

        public void addAccountView(Account account)
        {
            this.BlinkSocketDataReceiving();
            View.ContaUserControl contaUC = new View.ContaUserControl(account);

            this.ctrlGridView.Items.Add(contaUC);

            if (ctrlGridView.Items.Count == 1)
            {
                navIndexConta = 1;
                NavLimit();
                NavConta();
            }

            pagination();
        }

        // ------------------------------- NAVEGAÇÃO DO TECLADO ---------------------------------------- //
        private async void Page_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if(configView == 0)
            {
                switch (e.Key)
                {
                    // --------- ADD CONTA ----------
                    case VirtualKey.Number8:;
                        localSettings = settingsUtil.GetSettings();

                        Account contaData = new Account();

                        contaData.Type = AccountType.SALAO;
                        contaData.Number = 5;
                        contaData.Items = new List<AccountItem>();

                        AccountItem i1 = new AccountItem();
                        i1.Id = 0;
                        i1.Name = "DUPLO BACON";
                        i1.Quantity = 1;
                        i1.PrepareTime = 50;
                        i1.ComboName = "DUPLO BACON";
                        i1.Aditionals = new List<AccountItemAditional>();

                        AccountItemAditional a1 = new AccountItemAditional();
                        a1.Name = "HAMBURGUE";

                        AccountItemAditional a2 = new AccountItemAditional();
                        a2.Name = "BATATAS";

                        i1.Aditionals.Add(a1);
                        i1.Aditionals.Add(a2);
                        contaData.Items.Add(i1);

                        AccountItem i2 = new AccountItem();
                        i2.Id = 1;
                        i2.Name = "LANCHE";
                        i2.Quantity = 1;
                        i2.PrepareTime = 48;
                        i2.Aditionals = new List<AccountItemAditional>();

                        AccountItemAditional a3 = new AccountItemAditional();
                        a3.Name = "BATATAS";

                        i2.Aditionals.Add(a3);

                        contaData.Items.Add(i2);

                        AccountItem i3 = new AccountItem();
                        i3.Id = 2;
                        i3.Name = "LANCHE";
                        i3.Quantity = 1;
                        i3.PrepareTime = 46;

                        contaData.Items.Add(i3);

                        AccountItem i4 = new AccountItem();
                        i4.Id = 3;
                        i4.Name = "asdasdLANCHE";
                        i4.Quantity = 1;
                        i4.PrepareTime = 46;

                        contaData.Items.Add(i4);

                        AccountItem i5 = new AccountItem();
                        i5.Id = 5;
                        i5.Name = "asdasdLANCHE";
                        i5.Quantity = 1;
                        i5.PrepareTime = 46;

                        contaData.Items.Add(i5);

                        this.addAccountView(contaData);
                        break;

                    // --------- LEFT ----------
                    case VirtualKey.Escape:
                        if(ctrlGridView.Items.Count != 0)
                        {
                            if (statusEnter == false)
                            {
                                navIndexConta--;
                                NavLimit();
                                NavConta();
                            }
                        }
                        break;

                    // --------- RIGHT ----------
                    case VirtualKey.Enter:
                        if (ctrlGridView.Items.Count != 0)
                        {
                            if (statusEnter == false)
                            {
                                navIndexConta++;
                                NavLimit();
                                NavConta();
                            }
                        }
                        break;

                    // -------- DOWN ---------
                    case VirtualKey.Number0:
                        if (ctrlGridView.Items.Count != 0)
                        {
                            ContaQtd();
                            if (statusEnter == true && localSettings.ScreenType == Model.ScreenType.PRODUCTION)
                            {
                                View.ContaUserControl contaUCslc = (View.ContaUserControl)this.ctrlGridView.SelectedItem;
                                contaUCslc.NavItem(1);
                            }
                            else
                            {
                                navIndexConta += qtdConta;
                                NavLimit();
                                NavConta();
                            }
                        }
                        break;

                    // --------- UP ----------
                    case VirtualKey.Number2:
                        if (ctrlGridView.Items.Count != 0)
                        {
                            ContaQtd();
                            if (statusEnter == true && localSettings.ScreenType == Model.ScreenType.PRODUCTION)
                            {
                                View.ContaUserControl contaUCslc = (View.ContaUserControl)this.ctrlGridView.SelectedItem;
                                contaUCslc.NavItem(2);
                            }
                            else
                            {
                                navIndexConta -= qtdConta;
                                NavLimit();
                                NavConta();
                            }
                        }
                        break;

                    // --------- ENTER ----------
                    case VirtualKey.Number3:
                        localSettings = settingsUtil.GetSettings();
                        if (ctrlGridView.Items.Count != 0 && ctrlGridView.SelectedIndex != -1)
                        {
                            if (this.statusEnter == false && localSettings.ScreenType == Model.ScreenType.PRODUCTION)
                            {
                                this.statusEnter = true;
                            }

                            // --------------------------- ENTER TELA DE PRODUÇÃO ----------------------------- //
                            if (this.statusEnter == true && this.enterUpdate == true && localSettings.ScreenType == Model.ScreenType.PRODUCTION)
                            {
                                if (ctrlGridView.SelectedIndex == navIndexConta)
                                {
                                    View.ContaUserControl contaUCslc = (View.ContaUserControl)this.ctrlGridView.SelectedItem;
                                    Boolean removeFromScreen = await contaUCslc.updateStatusItem();

                                    if (removeFromScreen)
                                    {
                                        this.ctrlGridView.Items.RemoveAt(ctrlGridView.SelectedIndex);

                                        if (this.navIndexConta == 0)
                                        {
                                            this.statusEnter = false;
                                            this.enterUpdate = false;
                                            this.NavConta();
                                        }
                                        else
                                        {
                                            this.statusEnter = false;
                                            this.enterUpdate = false;
                                            this.navIndexConta -= 1;
                                            this.NavConta();
                                        }
                                        
                                    }
                                }
                            }

                            // --------------------------- ENTER TELA DE CONFERENCIA ----------------------------- //
                            if (this.statusEnter == false && this.enterUpdate == false && localSettings.ScreenType == Model.ScreenType.CONFERENCE)
                            {
                                View.ContaUserControl contaUCslc = (View.ContaUserControl)this.ctrlGridView.SelectedItem;
                                Boolean removeFromScreenCf = contaUCslc.RemoveContaCf();
                                if (removeFromScreenCf)
                                {
                                    this.ctrlGridView.Items.RemoveAt(ctrlGridView.SelectedIndex);

                                    if (this.navIndexConta == 0)
                                    {
                                        this.statusEnter = false;
                                        this.enterUpdate = false;
                                        this.NavConta();
                                    }
                                    else
                                    {
                                        this.statusEnter = false;
                                        this.enterUpdate = false;
                                        this.navIndexConta -= 1;
                                        this.NavConta();
                                    }
                                }
                            }

                            // --------------------------- ENTER TELA DE ENTREGA ----------------------------- //
                            if (this.statusEnter == false && this.enterUpdate == false && localSettings.ScreenType == Model.ScreenType.ENTREGA)
                            {
                                View.ContaUserControl contaUCslc = (View.ContaUserControl)this.ctrlGridView.SelectedItem;
                                Boolean removeFromScreenE = await contaUCslc.PrintConta();
                                if (removeFromScreenE)
                                {
                                    this.ctrlGridView.Items.RemoveAt(ctrlGridView.SelectedIndex);

                                    if (this.navIndexConta == 0)
                                    {
                                        this.statusEnter = false;
                                        this.enterUpdate = false;
                                        this.NavConta();
                                    }
                                    else
                                    {
                                        this.statusEnter = false;
                                        this.enterUpdate = false;
                                        this.navIndexConta -= 1;
                                        this.NavConta();
                                    }
                                }
                            }

                            if (statusEnter == true && enterUpdate == false)
                            {
                                this.enterUpdate = true;
                            }

                            if (this.statusEnter == true && localSettings.ScreenType == Model.ScreenType.PRODUCTION)
                            {
                                View.ContaUserControl contaUCslc = (View.ContaUserControl)this.ctrlGridView.SelectedItem;
                                contaUCslc.StatusEnter(statusEnter);
                                contaUCslc.SelectItem();
                                contaUCslc.ColorItem();
                            }
                        }
                        break;

                    // --------- VOLTA ----------
                    case VirtualKey.Number1:
                        if (this.ctrlGridView.Items.Count != 0)
                        {
                            if (this.statusEnter == true && tipoTela == 0)
                            {
                                this.statusEnter = false;
                                this.enterUpdate = false;
                                View.ContaUserControl contaUCslc1 = (View.ContaUserControl)this.ctrlGridView.SelectedItem;
                                contaUCslc1.StatusEnter(statusEnter);
                                contaUCslc1.NavItem(3);
                            }
                            ctrlGridView.Focus(FocusState.Programmatic);
                        }
                        break;

                    // --------- CONFIG ----------
                    case VirtualKey.Number6:
                        this.configView = 1;
                        this.LoadConfig();
                        await MyContentDialog.ShowAsync();
                        this.NavConfig();
                        break;

                }
            }
        }

        private void NavConta()
        {
            ctrlGridView.Focus(FocusState.Programmatic);
            if (ctrlGridView.Items.Count > 0)
                ctrlGridView.SelectedIndex = navIndexConta;
        }
        private void NavLimit()
        {
            if ((navIndexConta + 1) > ctrlGridView.Items.Count)
                navIndexConta = 0;
            else if (navIndexConta < 0)
                navIndexConta = (ctrlGridView.Items.Count - 1);
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            for (int i = 0; i <= (ctrlGridView.Items.Count - 1); i++)
            {
                View.ContaUserControl contaUC = (View.ContaUserControl)ctrlGridView.Items[i];
                if (ctrlGridView.Items.Count >= 1)
                {
                    contaUC.ChangeViewMode();
                }
            }
            ContaQtd();
        }

        private void ContaQtd()
        {
            if (localSettings.ViewMode == Model.ViewMode.GRID)
            {
                qtdConta = (Convert.ToInt16(Window.Current.Bounds.Width)) / 440;
            }
            else
            {
                qtdConta = 1;
            }
        }

        private void pagination()
        {
            tbPag.Text ="Conta: " + (ctrlGridView.SelectedIndex + 1).ToString() + "/" + ctrlGridView.Items.Count;
        }

        private void ctrlGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            pagination();
           
            for(int i = 0; i <= (ctrlGridView.Items.Count - 1); i++)
            {
                if(ctrlGridView.SelectedIndex == i)
                {
                    View.ContaUserControl contaUCslc = (View.ContaUserControl)this.ctrlGridView.SelectedItem;
                    contaUCslc.ColorConta();
                }
                else
                {
                    View.ContaUserControl contaUCslc = (View.ContaUserControl)this.ctrlGridView.Items[i];
                    contaUCslc.DefaultfColorConta();
                }
            }
        }

        // ----------------------------------- CONFIGURAÇÕES ------------------------------------------ //

        private Model.ViewMode viewMode;

        private Model.ScreenType screenType;

        private string GetLocalIp()
        {
            var icp = NetworkInformation.GetInternetConnectionProfile();

            if (icp?.NetworkAdapter == null) return null;
            var hostname =
                NetworkInformation.GetHostNames()
                    .SingleOrDefault(
                        hn =>
                            hn.IPInformation?.NetworkAdapter != null && hn.IPInformation.NetworkAdapter.NetworkAdapterId
                            == icp.NetworkAdapter.NetworkAdapterId);

            // the ip address
            return hostname?.CanonicalName;
        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch togglesw = sender as ToggleSwitch;
            if(togglesw != null)
            {
                
                var settings = new Model.Settings();
                if (togglesw.IsOn == true)
                {
                    viewMode = Model.ViewMode.LIST;
                }
                else
                {
                    viewMode = Model.ViewMode.GRID;
                }
            }
            ContaQtd();
        }

        private void MyContentDialog_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if(configView == 1)
            {
                switch (e.Key)
                {
                    // --------- ENTER ---------- 
                    case VirtualKey.Number3:
                        int qtd = lvConfig.Items.Count;
                        if (qtd >= 1 && lvConfig.SelectedIndex == -1)
                        {
                            lvConfig.SelectedIndex = 0;
                        }
                        
                        // ------------ config visualização conta ----------
                        if (lvConfig.SelectedIndex == 0)
                        {
                            if (tgViewMode.IsOn == false)
                                tgViewMode.IsOn = true;
                            else
                                tgViewMode.IsOn = false;
                        }

                        // ------------ config tipo de tela ------------------
                        if (lvConfig.SelectedIndex == 1)
                        {
                            tpTela++;
                            TpTela();
                        }

                        // ------------ config API IP ------------------
                        if (lvConfig.SelectedIndex == 2)
                        {
                            if(ipConfigA == false)
                            {
                                ipConfigA = true;
                                ipIndexA = 1;
                                ColorIpA();
                            }
                        }

                        // ------------ config API Port ------------------
                        if (lvConfig.SelectedIndex == 3)
                        {
                            if (prtConfigA == false)
                            {
                                prtConfigA = true;
                                prtIndexA = 1;
                                ColorPrtA();
                            }
                        }

                        // ------------ config Socket Port ------------------
                        if (lvConfig.SelectedIndex == 4)
                        {
                            if (prtConfigS == false)
                            {
                                prtConfigS = true;
                                prtIndexS = 1;
                                ColorPrtS();
                            }
                        }

                        // ------------ config delivery IP ------------------
                        if (lvConfig.SelectedIndex == 5)
                        {
                            if (tpTela == 0)
                            {
                                if (ipConfigD == false)
                                {
                                    ipConfigD = true;
                                    ipIndexD = 1;
                                    ColorIpD();
                                }
                            }
                        }

                        // ------------ config delivery Port ------------------
                        if (lvConfig.SelectedIndex == 6)
                        {
                            if (tpTela == 0)
                            {
                                if (prtConfigD == false)
                                {
                                    prtConfigD = true;
                                    prtIndexD = 1;
                                    ColorPrtD();
                                }
                            }
                        }


                        // ------------ Salvar ---------------
                        if (lvConfig.SelectedIndex == 7)
                        {

                            localSettings = settingsUtil.GetSettings();

                            var settings = new Model.Settings();

                            // ------------------------ OBTEM O IP DA API ------------------------------------
                            settings.ApiIp = ipA1 + "." + ipA2 + "." + ipA3 + "." + ipA4;

                            // ------------------------ OBTEM A PORTA DA API ---------------------------------
                            settings.ApiPort = (prtA1 * 10000) + (prtA2 * 1000) + (prtA3 * 100) + (prtA4 * 10) + prtA5;

                            if (prtA1 == 0)
                                settings.ApiPort = (prtA2 * 1000) + (prtA3 * 100) + (prtA4 * 10) + prtA5;
                            if (prtA1 == 0 && prtA2 == 0)
                                settings.ApiPort = (prtA3 * 100) + (prtA4 * 10) + prtA5;
                            if (prtA1 == 0 && prtA2 == 0 && prtA3 == 0)
                                settings.ApiPort = (prtA4 * 10) + prtA5;
                            if (prtA1 == 0 && prtA2 == 0 && prtA3 == 0 && prtA4 == 0)
                                settings.ApiPort = prtA5;

                            // ------------------------ OBTEM A PORTA DO SOCKET ---------------------------------
                            settings.LocalSocketPort = (prtS1 * 10000) + (prtS2 * 1000) + (prtS3 * 100) + (prtS4 * 10) + prtS5;

                            if (prtS1 == 0)
                                settings.LocalSocketPort = (prtS2 * 1000) + (prtS3 * 100) + (prtS4 * 10) + prtS5;
                            if (prtS1 == 0 && prtS2 == 0)
                                settings.LocalSocketPort = (prtS3 * 100) + (prtS4 * 10) + prtS5;
                            if (prtS1 == 0 && prtS2 == 0 && prtS3 == 0)
                                settings.LocalSocketPort = (prtS4 * 10) + prtS5;
                            if (prtS1 == 0 && prtS2 == 0 && prtS3 == 0 && prtS4 == 0)
                                settings.LocalSocketPort = prtS5;

                            // ------------------------ OBTEM O IP DA DELIVERY REMOTE ----------------------------
                            settings.DeliveryDeviceIp = ipD1 + "." + ipD2 + "." + ipD3 + "." + ipD4;

                            // -------------------- OBTEM A PORTA DO DELIVERY REMOTE -----------------------------
                            settings.DeliveryDevicePort = (prtD1 * 10000) + (prtD2 * 1000) + (prtD3 * 100) + (prtD4 * 10) + prtD5;

                            if (prtD1 == 0)
                                settings.DeliveryDevicePort = (prtD2 * 1000) + (prtD3 * 100) + (prtD4 * 10) + prtD5;
                            if (prtD1 == 0 && prtD2 == 0)
                                settings.DeliveryDevicePort = (prtD3 * 100) + (prtD4 * 10) + prtD5;
                            if (prtD1 == 0 && prtD2 == 0 && prtD3 == 0)
                                settings.DeliveryDevicePort = (prtD4 * 10) + prtD5;
                            if (prtD1 == 0 && prtD2 == 0 && prtD3 == 0 && prtD4 == 0)
                                settings.DeliveryDevicePort = prtD5;

                            settings.ViewMode = viewMode;

                            TpTela();
                            settings.ScreenType = screenType;

                            settingsUtil.SaveSettings(settings);

                            localSettings = settingsUtil.GetSettings();

                            if (ctrlGridView.Items.Count >= 1)
                            {
                                for (int i = 0; i <= (ctrlGridView.Items.Count - 1); i++)
                                {
                                    View.ContaUserControl contaUC = (View.ContaUserControl)ctrlGridView.Items[i];
                                    if (ctrlGridView.Items.Count >= 1)
                                    {
                                        contaUC.ChangeViewMode();
                                    }
                                }

                                switch (localSettings.ScreenType)
                                {
                                    case Model.ScreenType.CONFERENCE:
                                        this.statusEnter = false;
                                        this.enterUpdate = false;
                                        View.ContaUserControl contaUCslc1 = (View.ContaUserControl)this.ctrlGridView.SelectedItem;
                                        contaUCslc1.StatusEnter(statusEnter);
                                        contaUCslc1.NavItem(3);
                                        break;
                                    case Model.ScreenType.ENTREGA:
                                        this.statusEnter = false;
                                        this.enterUpdate = false;
                                        View.ContaUserControl contaUCslc = (View.ContaUserControl)this.ctrlGridView.SelectedItem;
                                        contaUCslc.StatusEnter(statusEnter);
                                        contaUCslc.NavItem(3); ;
                                        break;
                                }
                            }
                            configView = 0;
                            MyContentDialog.Hide();
                            navIndexConfig = 0;

                            if (localSettings != null && localSettings.LocalSocketPort > 0)
                            {
                                this.StartSocketServer();
                            }
                        }
                        break;

                    // --------- CONFIG/VOLTA ----------
                    case VirtualKey.Number6:

                        if(ipConfigA == false && prtConfigA == false && prtConfigS == false && ipConfigD == false && prtConfigD == false)
                        {
                            configView = 0;
                            tgViewMode.IsOn = false;

                            MyContentDialog.Hide();
                            navIndexConfig = 0;
                        }
                        break;

                    // --------- DOWN ----------
                    case VirtualKey.Number0:
                        if(ipConfigA == false && prtConfigA == false && prtConfigS == false && ipConfigD == false && prtConfigD == false)
                        {
                            navIndexConfig++;
                            NavLimitConfig();
                            NavConfig();
                        }
                        break;

                    // --------- UP ----------
                    case VirtualKey.Number2:
                        if(ipConfigA == false && prtConfigA == false && prtConfigS == false && ipConfigD == false && prtConfigD == false)
                        {
                            navIndexConfig--;
                            NavLimitConfig();
                            NavConfig();
                        }
                        break;

                    // --------- VOLTA ----------
                    case VirtualKey.Number1:
                        if (ipConfigA == false && prtConfigA == false && prtConfigS == false && ipConfigD == false && prtConfigD == false)
                        {
                            if(configView == 1 && ipConfigA == false)
                            configView = 0;
                            tgViewMode.IsOn = false;

                            MyContentDialog.Hide();
                            navIndexConfig = 0;
                        }
                        else
                        {
                            ipConfigA = false;
                            ipIndexA = 0;
                            ColorIpA();

                            prtConfigA = false;
                            prtIndexA = 0;
                            ColorPrtA();

                            prtConfigS = false;
                            prtIndexS = 0;
                            ColorPrtS();
                        }
                        break;
                    case VirtualKey.Escape:
                        if (ipConfigA == false && prtConfigA == false && prtConfigS == false && ipConfigD == false && prtConfigD == false)
                        {
                            if (configView == 1 && ipConfigA == false)
                                configView = 0;

                            MyContentDialog.Hide();
                            navIndexConfig = 0;
                        }
                        else
                        {
                            ipConfigA = false;
                            ipIndexA = 0;
                            ColorIpA();

                            prtConfigA = false;
                            prtIndexA = 0;
                            ColorPrtA();

                            prtConfigS = false;
                            prtIndexS = 0;
                            ColorPrtS();
                        }
                        break;
                }
            }
        }
        
        private void LoadConfig()
        {
            foreach (HostName localHostName in NetworkInformation.GetHostNames())
            {
                if (localHostName.IPInformation != null)
                {
                    if (localHostName.Type == HostNameType.Ipv4)
                    {
                        ipLocal = localHostName.ToString();
                        break;
                    }
                }
            }

            tbIpLocal.Text = ipLocal;

            localSettings = settingsUtil.GetSettings();
            // ------------ LOAD VIEWMODE ----------- //
            if (localSettings.ViewMode == Model.ViewMode.GRID)
                tgViewMode.IsOn = false;
            else
                tgViewMode.IsOn = true;

            //------------ LOAD SCREEN TYPE ------------ //
            switch (localSettings.ScreenType)
            {
                case Model.ScreenType.PRODUCTION:
                    tpTela = 0;
                    tbTpTela.Text = "Produção";
                    gridApiD.Opacity = 1;
                    gridPortD.Opacity = 1;
                    break;
                case Model.ScreenType.CONFERENCE:
                    tpTela = 1;
                    tbTpTela.Text = "Conferência";
                    gridApiD.Opacity = 0.3;
                    gridPortD.Opacity = 0.3;
                    break;
                case Model.ScreenType.ENTREGA:
                    tpTela = 2;
                    tbTpTela.Text = "Entrega";
                    gridApiD.Opacity = 0.3;
                    gridPortD.Opacity = 0.3;
                    break;
            }

            //---------- LOAD API IP ------------------- //
            int indexPonto = localSettings.ApiIp.IndexOf(".");
            int indexPonto2 = localSettings.ApiIp.IndexOf(".", (indexPonto + 1));
            int indexPonto3 = localSettings.ApiIp.IndexOf(".", (indexPonto2 + 1));

            this.ipA1 = Convert.ToInt16(localSettings.ApiIp.Substring(0, indexPonto));
            this.ipA2 = Convert.ToInt16(localSettings.ApiIp.Substring((indexPonto + 1) , (indexPonto2 - indexPonto - 1)));
            this.ipA3 = Convert.ToInt16(localSettings.ApiIp.Substring(indexPonto2 + 1, (indexPonto3 - indexPonto2 - 1)));
            this.ipA4 = Convert.ToInt16(localSettings.ApiIp.Substring(indexPonto3 + 1, localSettings.ApiIp.Length - indexPonto3 - 1));

            //--------- LOAD API PORT ---------------- //
            string portA = localSettings.ApiPort.ToString();
            switch (portA.Length)
            {
                case 1:
                    this.prtA5 = Convert.ToInt32(portA.Substring(0, 1));
                    break;
                case 2:
                    this.prtA5 = Convert.ToInt16(portA.Substring(1, 1));
                    this.prtA4 = Convert.ToInt16(portA.Substring(0, 1));
                    break;
                case 3:
                    this.prtA5 = Convert.ToInt16(portA.Substring(2, 1));
                    this.prtA4 = Convert.ToInt16(portA.Substring(1, 1));
                    this.prtA3 = Convert.ToInt16(portA.Substring(0, 1));
                    break;
                case 4:
                    this.prtA5 = Convert.ToInt16(portA.Substring(3, 1));
                    this.prtA4 = Convert.ToInt16(portA.Substring(2, 1));
                    this.prtA3 = Convert.ToInt16(portA.Substring(1, 1));
                    this.prtA2 = Convert.ToInt16(portA.Substring(0, 1));
                    break;
                case 5:
                    this.prtA5 = Convert.ToInt16(portA.Substring(4, 1));
                    this.prtA4 = Convert.ToInt16(portA.Substring(3, 1));
                    this.prtA3 = Convert.ToInt16(portA.Substring(2, 1));
                    this.prtA2 = Convert.ToInt16(portA.Substring(1, 1));
                    this.prtA1 = Convert.ToInt16(portA.Substring(0, 1));
                    break;
            }

            //--------- LOAD SOCKET PORT -------------- //
            string portS = localSettings.LocalSocketPort.ToString();
            switch (portS.Length)
            {
                case 1:
                    this.prtS5 = Convert.ToInt32(portS.Substring(0, 1));
                    break;
                case 2:
                    this.prtS5 = Convert.ToInt16(portS.Substring(1, 1));
                    this.prtS4 = Convert.ToInt16(portS.Substring(0, 1));
                    break;
                case 3:
                    this.prtS5 = Convert.ToInt16(portS.Substring(2, 1));
                    this.prtS4 = Convert.ToInt16(portS.Substring(1, 1));
                    this.prtS3 = Convert.ToInt16(portS.Substring(0, 1));
                    break;
                case 4:
                    this.prtS5 = Convert.ToInt16(portS.Substring(3, 1));
                    this.prtS4 = Convert.ToInt16(portS.Substring(2, 1));
                    this.prtS3 = Convert.ToInt16(portS.Substring(1, 1));
                    this.prtS2 = Convert.ToInt16(portS.Substring(0, 1));
                    break;
                case 5:
                    this.prtS5 = Convert.ToInt16(portS.Substring(4, 1));
                    this.prtS4 = Convert.ToInt16(portS.Substring(3, 1));
                    this.prtS3 = Convert.ToInt16(portS.Substring(2, 1));
                    this.prtS2 = Convert.ToInt16(portS.Substring(1, 1));
                    this.prtS1 = Convert.ToInt16(portS.Substring(0, 1));
                    break;
            }

            //---------- LOAD DELIVERY IP ------------------- //
            int indexPontoD = localSettings.DeliveryDeviceIp.IndexOf(".");
            int indexPontoD2 = localSettings.DeliveryDeviceIp.IndexOf(".", (indexPontoD + 1));
            int indexPontoD3 = localSettings.DeliveryDeviceIp.IndexOf(".", (indexPontoD2 + 1));

            this.ipD1 = Convert.ToInt16(localSettings.DeliveryDeviceIp.Substring(0, indexPontoD));
            this.ipD2 = Convert.ToInt16(localSettings.DeliveryDeviceIp.Substring((indexPontoD + 1), (indexPontoD2 - indexPontoD - 1)));
            this.ipD3 = Convert.ToInt16(localSettings.DeliveryDeviceIp.Substring(indexPontoD2 + 1, (indexPontoD3 - indexPontoD2 - 1)));
            this.ipD4 = Convert.ToInt16(localSettings.DeliveryDeviceIp.Substring(indexPontoD3 + 1, localSettings.DeliveryDeviceIp.Length - indexPontoD3 - 1));

            //--------- LOAD SOCKET PORT -------------- //
            string portD = localSettings.DeliveryDevicePort.ToString();
            switch (portD.Length)
            {
                case 1:
                    this.prtD5 = Convert.ToInt32(portD.Substring(0, 1));
                    break;
                case 2:
                    this.prtD5 = Convert.ToInt16(portD.Substring(1, 1));
                    this.prtD4 = Convert.ToInt16(portD.Substring(0, 1));
                    break;
                case 3:
                    this.prtD5 = Convert.ToInt16(portD.Substring(2, 1));
                    this.prtD4 = Convert.ToInt16(portD.Substring(1, 1));
                    this.prtD3 = Convert.ToInt16(portD.Substring(0, 1));
                    break;
                case 4:
                    this.prtD5 = Convert.ToInt16(portD.Substring(3, 1));
                    this.prtD4 = Convert.ToInt16(portD.Substring(2, 1));
                    this.prtD3 = Convert.ToInt16(portD.Substring(1, 1));
                    this.prtD2 = Convert.ToInt16(portD.Substring(0, 1));
                    break;
                case 5:
                    this.prtD5 = Convert.ToInt16(portD.Substring(4, 1));
                    this.prtD4 = Convert.ToInt16(portD.Substring(3, 1));
                    this.prtD3 = Convert.ToInt16(portD.Substring(2, 1));
                    this.prtD2 = Convert.ToInt16(portD.Substring(1, 1));
                    this.prtD1 = Convert.ToInt16(portD.Substring(0, 1));
                    break;
            }

            AtualizarIps();
        }

        private void TpTela()
        {
            switch (tpTela)
            {
                case 0:
                    tbTpTela.Text = "Produção";
                    screenType = Model.ScreenType.PRODUCTION;
                    gridApiD.Opacity = 1;
                    gridPortD.Opacity = 1;
                    break;
                case 1:
                    tbTpTela.Text = "Conferência";
                    screenType = Model.ScreenType.CONFERENCE;
                    gridApiD.Opacity = 0.3;
                    gridPortD.Opacity = 0.3;
                    break;
                case 2:
                    tbTpTela.Text = "Entrega";
                    screenType = Model.ScreenType.ENTREGA;
                    gridApiD.Opacity = 0.3;
                    gridPortD.Opacity = 0.3;
                    break;
                case 3:
                    tpTela = 0;
                    tbTpTela.Text = "Produção";
                    screenType = Model.ScreenType.PRODUCTION;
                    gridApiD.Opacity = 1;
                    gridPortD.Opacity = 1;
                    break;
            }
        }
        private void NavConfig()
        {
            lvConfig.SelectedIndex = navIndexConfig;
        }
        private void NavLimitConfig()
        {
            if ((navIndexConfig + 1) > lvConfig.Items.Count)
                navIndexConfig = 0;
            else if (navIndexConfig < 0)
                navIndexConfig = (lvConfig.Items.Count - 1);
        }

        // --------------- CONFIG API IP -------------------
        private void IpAPicker(int ipIndexA)
        {
            switch (ipIndexA)
            {
                case 1:
                    ipA1++;
                    ipA1Limit();
                    break;
                case 2:
                    ipA2++;
                    ipA2Limit();
                    break;
                case 3:
                    ipA3++;
                    ipA3Limit();
                    break;
                case 4:
                    ipA4++;
                    ipA4Limit();
                    break;
                case -1:
                    ipA1--;
                    ipA1Limit();
                    break;
                case -2:
                    ipA2--;
                    ipA2Limit();
                    break;
                case -3:
                    ipA3--;
                    ipA3Limit();
                    break;
                case -4:
                    ipA4--;
                    ipA4Limit();
                    break;
            }
        }
        private void ipA1Limit()
        {
            if ((ipA1 + 2) > 1000)
                ipA1 = 0;
            else if (ipA1-1 < -1)
                ipA1 = 999;
        }
        private void ipA2Limit()
        {
            if ((ipA2 + 2) > 1000)
                ipA2 = 0;
            else if (ipA2 - 1 < -1)
                ipA2 = 999;
        }
        private void ipA3Limit()
        {
            if ((ipA3 + 2) > 1000)
                ipA3 = 0;
            else if (ipA3 - 1 < -1)
                ipA3 = 999;
        }
        private void ipA4Limit()
        {
            if ((ipA4 + 2) > 1000)
                ipA4 = 0;
            else if (ipA4 - 1 < -1)
                ipA4 = 999;
        }

        private void ColorIpA()
        {
            tbipA1.Foreground = new SolidColorBrush(Colors.Black);
            tbipA2.Foreground = new SolidColorBrush(Colors.Black);
            tbipA3.Foreground = new SolidColorBrush(Colors.Black);
            tbipA4.Foreground = new SolidColorBrush(Colors.Black);

            switch (ipIndexA)
            {
                case 1:
                    tbipA1.Foreground = new SolidColorBrush(Colors.OrangeRed);
                    break;
                case 2:
                    tbipA2.Foreground = new SolidColorBrush(Colors.OrangeRed);
                    break;
                case 3:
                    tbipA3.Foreground = new SolidColorBrush(Colors.OrangeRed);
                    break;
                case 4:
                    tbipA4.Foreground = new SolidColorBrush(Colors.OrangeRed);
                    break;
            }
        }

        // ------------ CONFIG API PORT -------------
        private void PortPickerApi(int prtIndexA)
        {
            switch (prtIndexA)
            {
                case 1:
                    prtA1++;
                    prtLimitA1();
                    break;
                case 2:
                    prtA2++;
                    prtLimitA2();
                    break;
                case 3:
                    prtA3++;
                    prtLimitA3();
                    break;
                case 4:
                    prtA4++;
                    prtLimitA4();
                    break;
                case 5:
                    prtA5++;
                    prtLimitA5();
                    break;
                case -1:
                    prtA1--;
                    prtLimitA1();
                    break;
                case -2:
                    prtA2--;
                    prtLimitA2();
                    break;
                case -3:
                    prtA3--;
                    prtLimitA3();
                    break;
                case -4:
                    prtA4--;
                    prtLimitA4();
                    break;
                case -5:
                    prtA5--;
                    prtLimitA5();
                    break;
            }
        }
        private void prtLimitA1()
        {
            if ((prtA1) > 9)
                prtA1 = 0;
            else if (prtA1 - 1 < -1)
                prtA1 = 9;
        }
        private void prtLimitA2()
        {
            if ((prtA2) > 9)
                prtA2 = 0;
            else if (prtA2 - 1 < -1)
                prtA2 = 9;
        }
        private void prtLimitA3()
        {
            if ((prtA3) > 9)
                prtA3 = 0;
            else if (prtA3 - 1 < -1)
                prtA3 = 9;
        }
        private void prtLimitA4()
        {
            if ((prtA4) > 9)
                prtA4 = 0;
            else if (prtA4 - 1 < -1)
                prtA4 = 9;
        }
        private void prtLimitA5()
        {
            if ((prtA5) > 9)
                prtA5 = 0;
            else if (prtA5 - 1 < -1)
                prtA5 = 9;
        }

        private void ColorPrtA()
        {
            tbPortAPI1.Foreground = new SolidColorBrush(Colors.Black);
            tbPortAPI2.Foreground = new SolidColorBrush(Colors.Black);
            tbPortAPI3.Foreground = new SolidColorBrush(Colors.Black);
            tbPortAPI4.Foreground = new SolidColorBrush(Colors.Black);
            tbPortAPI5.Foreground = new SolidColorBrush(Colors.Black);

            switch (prtIndexA)
            {
                case 1:
                    tbPortAPI1.Foreground = new SolidColorBrush(Colors.OrangeRed);
                    break;
                case 2:
                    tbPortAPI2.Foreground = new SolidColorBrush(Colors.OrangeRed);
                    break;
                case 3:
                    tbPortAPI3.Foreground = new SolidColorBrush(Colors.OrangeRed);
                    break;
                case 4:
                    tbPortAPI4.Foreground = new SolidColorBrush(Colors.OrangeRed);
                    break;
                case 5:
                    tbPortAPI5.Foreground = new SolidColorBrush(Colors.OrangeRed);
                    break;
            }
        }

        // ------------ CONFIG SOCKET PORT -------------
        private void PortPickerSOC(int prtIndexS)
        {
            switch (prtIndexS)
            {
                case 1:
                    prtS1++;
                    prtLimitS1();
                    break;
                case 2:
                    prtS2++;
                    prtLimitS2();
                    break;
                case 3:
                    prtS3++;
                    prtLimitS3();
                    break;
                case 4:
                    prtS4++;
                    prtLimitS4();
                    break;
                case 5:
                    prtS5++;
                    prtLimitS5();
                    break;
                case -1:
                    prtS1--;
                    prtLimitS1();
                    break;
                case -2:
                    prtS2--;
                    prtLimitS2();
                    break;
                case -3:
                    prtS3--;
                    prtLimitS3();
                    break;
                case -4:
                    prtS4--;
                    prtLimitS4();
                    break;
                case -5:
                    prtS5--;
                    prtLimitS5();
                    break;
            }
        }
        private void prtLimitS1()
        {
            if ((prtS1) > 9)
                prtS1 = 0;
            else if (prtS1 - 1 < -1)
                prtS1 = 9;
        }
        private void prtLimitS2()
        {
            if ((prtS2) > 9)
                prtS2 = 0;
            else if (prtS2 - 1 < -1)
                prtS2 = 9;
        }
        private void prtLimitS3()
        {
            if ((prtS3) > 9)
                prtS3 = 0;
            else if (prtS3 - 1 < -1)
                prtS3 = 9;
        }
        private void prtLimitS4()
        {
            if ((prtS4) > 9)
                prtS4 = 0;
            else if (prtS4 - 1 < -1)
                prtS4 = 9;
        }
        private void prtLimitS5()
        {
            if ((prtS5) > 9)
                prtS5 = 0;
            else if (prtS5 - 1 < -1)
                prtS5 = 9;
        }

        private void ColorPrtS()
        {
            tbPortSOC1.Foreground = new SolidColorBrush(Colors.Black);
            tbPortSOC2.Foreground = new SolidColorBrush(Colors.Black);
            tbPortSOC3.Foreground = new SolidColorBrush(Colors.Black);
            tbPortSOC4.Foreground = new SolidColorBrush(Colors.Black);
            tbPortSOC5.Foreground = new SolidColorBrush(Colors.Black);

            switch (prtIndexS)
            {
                case 1:
                    tbPortSOC1.Foreground = new SolidColorBrush(Colors.OrangeRed);
                    break;
                case 2:
                    tbPortSOC2.Foreground = new SolidColorBrush(Colors.OrangeRed);
                    break;
                case 3:
                    tbPortSOC3.Foreground = new SolidColorBrush(Colors.OrangeRed);
                    break;
                case 4:
                    tbPortSOC4.Foreground = new SolidColorBrush(Colors.OrangeRed);
                    break;
                case 5:
                    tbPortSOC5.Foreground = new SolidColorBrush(Colors.OrangeRed);
                    break;
            }
        }

        // --------------- CONFIG DELIVERY IP -------------------
        private void IpDPicker(int ipIndexD)
        {
            switch (ipIndexD)
            {
                case 1:
                    ipD1++;
                    ipD1Limit();
                    break;
                case 2:
                    ipD2++;
                    ipD2Limit();
                    break;
                case 3:
                    ipD3++;
                    ipD3Limit();
                    break;
                case 4:
                    ipD4++;
                    ipD4Limit();
                    break;
                case -1:
                    ipD1--;
                    ipD1Limit();
                    break;
                case -2:
                    ipD2--;
                    ipD2Limit();
                    break;
                case -3:
                    ipD3--;
                    ipD3Limit();
                    break;
                case -4:
                    ipD4--;
                    ipD4Limit();
                    break;
            }
        }
        private void ipD1Limit()
        {
            if ((ipD1 + 2) > 1000)
                ipD1 = 0;
            else if (ipD1 - 1 < -1)
                ipD1 = 999;
        }
        private void ipD2Limit()
        {
            if ((ipD2 + 2) > 1000)
                ipD2 = 0;
            else if (ipD2 - 1 < -1)
                ipD2 = 999;
        }
        private void ipD3Limit()
        {
            if ((ipD3 + 2) > 1000)
                ipD3 = 0;
            else if (ipD3 - 1 < -1)
                ipD3 = 999;
        }
        private void ipD4Limit()
        {
            if ((ipD4 + 2) > 1000)
                ipD4 = 0;
            else if (ipD4 - 1 < -1)
                ipD4 = 999;
        }

        private void ColorIpD()
        {
            tbipD1.Foreground = new SolidColorBrush(Colors.Black);
            tbipD2.Foreground = new SolidColorBrush(Colors.Black);
            tbipD3.Foreground = new SolidColorBrush(Colors.Black);
            tbipD4.Foreground = new SolidColorBrush(Colors.Black);

            switch (ipIndexD)
            {
                case 1:
                    tbipD1.Foreground = new SolidColorBrush(Colors.OrangeRed);
                    break;
                case 2:
                    tbipD2.Foreground = new SolidColorBrush(Colors.OrangeRed);
                    break;
                case 3:
                    tbipD3.Foreground = new SolidColorBrush(Colors.OrangeRed);
                    break;
                case 4:
                    tbipD4.Foreground = new SolidColorBrush(Colors.OrangeRed);
                    break;
            }
        }

        // ------------ CONFIG SOCKET PORT -------------
        private void PortPickerD(int prtIndexD)
        {
            switch (prtIndexD)
            {
                case 1:
                    prtD1++;
                    prtLimitD1();
                    break;
                case 2:
                    prtD2++;
                    prtLimitD2();
                    break;
                case 3:
                    prtD3++;
                    prtLimitD3();
                    break;
                case 4:
                    prtD4++;
                    prtLimitD4();
                    break;
                case 5:
                    prtD5++;
                    prtLimitD5();
                    break;
                case -1:
                    prtD1--;
                    prtLimitD1();
                    break;
                case -2:
                    prtD2--;
                    prtLimitD2();
                    break;
                case -3:
                    prtD3--;
                    prtLimitD3();
                    break;
                case -4:
                    prtD4--;
                    prtLimitD4();
                    break;
                case -5:
                    prtD5--;
                    prtLimitD5();
                    break;
            }
        }
        private void prtLimitD1()
        {
            if ((prtD1) > 9)
                prtD1 = 0;
            else if (prtD1 - 1 < -1)
                prtD1 = 9;
        }
        private void prtLimitD2()
        {
            if ((prtD2) > 9)
                prtD2 = 0;
            else if (prtD2 - 1 < -1)
                prtD2 = 9;
        }
        private void prtLimitD3()
        {
            if ((prtD3) > 9)
                prtD3 = 0;
            else if (prtD3 - 1 < -1)
                prtD3 = 9;
        }
        private void prtLimitD4()
        {
            if ((prtD4) > 9)
                prtD4 = 0;
            else if (prtD4 - 1 < -1)
                prtD4 = 9;
        }
        private void prtLimitD5()
        {
            if ((prtD5) > 9)
                prtD5 = 0;
            else if (prtD5 - 1 < -1)
                prtD5 = 9;
        }

        private void ColorPrtD()
        {
            tbPortD1.Foreground = new SolidColorBrush(Colors.Black);
            tbPortD2.Foreground = new SolidColorBrush(Colors.Black);
            tbPortD3.Foreground = new SolidColorBrush(Colors.Black);
            tbPortD4.Foreground = new SolidColorBrush(Colors.Black);
            tbPortD5.Foreground = new SolidColorBrush(Colors.Black);

            switch (prtIndexD)
            {
                case 1:
                    tbPortD1.Foreground = new SolidColorBrush(Colors.OrangeRed);
                    break;
                case 2:
                    tbPortD2.Foreground = new SolidColorBrush(Colors.OrangeRed);
                    break;
                case 3:
                    tbPortD3.Foreground = new SolidColorBrush(Colors.OrangeRed);
                    break;
                case 4:
                    tbPortD4.Foreground = new SolidColorBrush(Colors.OrangeRed);
                    break;
                case 5:
                    tbPortD5.Foreground = new SolidColorBrush(Colors.OrangeRed);
                    break;
            }
        }

        // ----------------- EVENTOS TECLAS IP -------------------- //
        private void lvConfig_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                // ----------- DOWN -------------
                case VirtualKey.Number0:

                    // ------- API IP ----------- //
                    if (ipConfigA == true)
                    {
                        switch (ipIndexA)
                        {
                            case 1:
                                IpAPicker(-1);
                                break;
                            case 2:
                                IpAPicker(-2);
                                break;
                            case 3:
                                IpAPicker(-3);
                                break;
                            case 4:
                                IpAPicker(-4);
                                break;
                            case 5:
                                ipConfigA = false;
                                ipIndexA = 0;
                                break;
                        }
                    }

                    // ------- API PORT --------- //
                    if (prtConfigA == true)
                    {
                        switch (prtIndexA)
                        {
                            case 1:
                                PortPickerApi(-1);
                                break;
                            case 2:
                                PortPickerApi(-2);
                                break;
                            case 3:
                                PortPickerApi(-3);
                                break;
                            case 4:
                                PortPickerApi(-4);
                                break;
                            case 5:
                                PortPickerApi(-5);
                                break;
                            case 6:
                                prtConfigA = false;
                                prtIndexA = 0;
                                break;
                        }
                    }

                    // ------ SOCKET PORT ------- //
                    if (prtConfigS == true)
                    {
                        switch (prtIndexS)
                        {
                            case 1:
                                PortPickerSOC(-1);
                                break;
                            case 2:
                                PortPickerSOC(-2);
                                break;
                            case 3:
                                PortPickerSOC(-3);
                                break;
                            case 4:
                                PortPickerSOC(-4);
                                break;
                            case 5:
                                PortPickerSOC(-5);
                                break;
                            case 6:
                                prtConfigS = false;
                                prtIndexS = 0;
                                break;
                        }
                    }

                    // ------ DELIVERY IP ------- //
                    if (ipConfigD == true)
                    {
                        switch (ipIndexD)
                        {
                            case 1:
                                IpDPicker(-1);
                                break;
                            case 2:
                                IpDPicker(-2);
                                break;
                            case 3:
                                IpDPicker(-3);
                                break;
                            case 4:
                                IpDPicker(-4);
                                break;
                            case 5:
                                ipConfigD= false;
                                ipIndexD = 0;
                                break;
                        }
                    }

                    // ----- DELIVERY PORT ------ //
                    if (prtConfigD == true)
                    {
                        switch (prtIndexD)
                        {
                            case 1:
                                PortPickerD(-1);
                                break;
                            case 2:
                                PortPickerD(-2);
                                break;
                            case 3:
                                PortPickerD(-3);
                                break;
                            case 4:
                                PortPickerD(-4);
                                break;
                            case 5:
                                PortPickerD(-5);
                                break;
                            case 6:
                                prtConfigD = false;
                                prtIndexD = 0;
                                break;
                        }
                    }
                    break;

                // ----------- UP -------------
                case VirtualKey.Number2:

                    // ------- API IP ----------- //
                    if (ipConfigA == true)
                    {
                        switch (ipIndexA)
                        {
                            case 1:
                                IpAPicker(1);
                                break;
                            case 2:
                                IpAPicker(2);
                                break;
                            case 3:
                                IpAPicker(3);
                                break;
                            case 4:
                                IpAPicker(4);
                                break;
                            case 5:
                                ipConfigA = false;
                                ipIndexA = 0;
                                break;
                        }  
                    }

                    // ------- API PORT --------- //
                    if (prtConfigA == true)
                    {
                        switch (prtIndexA)
                        {
                            case 1:
                                PortPickerApi(1);
                                break;
                            case 2:
                                PortPickerApi(2);
                                break;
                            case 3:
                                PortPickerApi(3);
                                break;
                            case 4:
                                PortPickerApi(4);
                                break;
                            case 5:
                                PortPickerApi(5);
                                break;
                            case 6:
                                prtConfigA = false;
                                prtIndexA = 0;
                                break;
                        }
                    }

                    // ------ SOCKET PORT ------- //
                    if (prtConfigS == true)
                    {
                        switch (prtIndexS)
                        {
                            case 1:
                                PortPickerSOC(1);
                                break;
                            case 2:
                                PortPickerSOC(2);
                                break;
                            case 3:
                                PortPickerSOC(3);
                                break;
                            case 4:
                                PortPickerSOC(4);
                                break;
                            case 5:
                                PortPickerSOC(5);
                                break;
                            case 6:
                                prtConfigS = false;
                                prtIndexS = 0;
                                break;
                        }
                    }

                    // ------ DELIVERY IP ------- //
                    if (ipConfigD == true)
                    {
                        switch (ipIndexD)
                        {
                            case 1:
                                IpDPicker(1);
                                break;
                            case 2:
                                IpDPicker(2);
                                break;
                            case 3:
                                IpDPicker(3);
                                break;
                            case 4:
                                IpDPicker(4);
                                break;
                            case 5:
                                ipConfigD = false;
                                ipIndexD = 0;
                                break;
                        }
                    }

                    // ----- DELIVERY PORT ------ //
                    if (prtConfigD == true)
                    {
                        switch (prtIndexD)
                        {
                            case 1:
                                PortPickerD(1);
                                break;
                            case 2:
                                PortPickerD(2);
                                break;
                            case 3:
                                PortPickerD(3);
                                break;
                            case 4:
                                PortPickerD(4);
                                break;
                            case 5:
                                PortPickerD(5);
                                break;
                            case 6:
                                prtConfigD = false;
                                prtIndexD = 0;
                                break;
                        }
                    }
                    break;

                // ----------- ENTER -------------
                case VirtualKey.Number3:

                    // ------- API IP ----------- //
                    if (ipIndexA >= 5)
                    {
                        ipConfigA = false;
                    }
                    if (ipConfigA == true)
                    {
                        ipIndexA++;
                    }

                    // ------- API PORT --------- //
                    if (prtIndexA >= 6)
                    {
                        prtConfigA = false;
                    }
                    if (prtConfigA == true)
                    {
                        prtIndexA++;
                    }

                    // ------ SOCKET PORT ------- //
                    if (prtIndexS >= 6)
                    {
                        prtConfigS = false;
                    }
                    if (prtConfigS == true)
                    {
                        prtIndexS++;
                    }

                    // ------ DELIVERY IP ------- //
                    if (ipIndexD >= 5)
                    {
                        ipConfigD = false;
                    }
                    if (ipConfigD == true)
                    {
                        ipIndexD++;
                    }

                    // ----- DELIVERY PORT ------ //
                    if (prtIndexD >= 6)
                    {
                        prtConfigD = false;
                    }
                    if (prtConfigD == true)
                    {
                        prtIndexD++;
                    }
                    break;

                case VirtualKey.Escape:
                    if (ipConfigA == false && prtConfigA == false && prtConfigS == false)
                    {
                        if (configView == 1 && ipConfigA == false)
                            configView = 0;
                        tgViewMode.IsOn = false;

                        MyContentDialog.Hide();
                        navIndexConfig = 0;
                    }
                    else
                    {
                        ipConfigA = false;
                        ipIndexA = 0;
                        ColorIpA();

                        prtConfigA = false;
                        prtIndexA = 0;
                        ColorPrtA();

                        prtConfigS = false;
                        prtIndexS = 0;
                        ColorPrtS();
                    }
                    break;
            }

            AtualizarIps();
        }

        private void AtualizarIps()
        {

            // ------- API IP ----------- //
            ColorIpA();
            tbipA1.Text = ipA1.ToString();
            tbipA2.Text = ipA2.ToString();
            tbipA3.Text = ipA3.ToString();
            tbipA4.Text = ipA4.ToString();

            // ------- API PORT --------- //
            ColorPrtA();
            tbPortAPI1.Text = prtA1.ToString();
            tbPortAPI2.Text = prtA2.ToString();
            tbPortAPI3.Text = prtA3.ToString();
            tbPortAPI4.Text = prtA4.ToString();
            tbPortAPI5.Text = prtA5.ToString();

            // ------ SOCKET PORT ------- //
            ColorPrtS();
            tbPortSOC1.Text = prtS1.ToString();
            tbPortSOC2.Text = prtS2.ToString();
            tbPortSOC3.Text = prtS3.ToString();
            tbPortSOC4.Text = prtS4.ToString();
            tbPortSOC5.Text = prtS5.ToString();

            // ------ DELIVERY IP ------- //
            ColorIpD();
            tbipD1.Text = ipD1.ToString();
            tbipD2.Text = ipD2.ToString();
            tbipD3.Text = ipD3.ToString();
            tbipD4.Text = ipD4.ToString();

            // ----- DELIVERY PORT ------ //
            ColorPrtD();
            tbPortD1.Text = prtD1.ToString();
            tbPortD2.Text = prtD2.ToString();
            tbPortD3.Text = prtD3.ToString();
            tbPortD4.Text = prtD4.ToString();
            tbPortD5.Text = prtD5.ToString();
        }
    }
}


