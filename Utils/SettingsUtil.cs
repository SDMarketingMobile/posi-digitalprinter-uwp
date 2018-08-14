using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace POSIDigitalPrinter.Utils
{
    class SettingsUtil
    {
        private static SettingsUtil instance;

        public static SettingsUtil Instance
        {
            get
            {
                if (instance == null)
                    instance = new SettingsUtil();
                return instance;
            }
        }

        string ContainerName = "pdb";
        ApplicationDataContainer SettingsContainer;

        private SettingsUtil()
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            if (localSettings.Containers.ContainsKey(ContainerName))
            {
                SettingsContainer = localSettings.Containers[ContainerName];
            }
            else
            {
                SettingsContainer = localSettings.CreateContainer(ContainerName, ApplicationDataCreateDisposition.Always);
            }
        }
        
        public void SaveSettings(Model.Settings settings)
        {
            SettingsContainer.Values["view.mode"]               = (int) settings.ViewMode;
            SettingsContainer.Values["screen.type"]             = (int) settings.ScreenType;
            SettingsContainer.Values["api.ip"]                  = settings.ApiIp;
            SettingsContainer.Values["api.port"]                = settings.ApiPort;
            SettingsContainer.Values["socket.port"]             = settings.LocalSocketPort;
            SettingsContainer.Values["delivery.device.ip"]      = settings.DeliveryDeviceIp;
            SettingsContainer.Values["delivery.device.port"]    = settings.DeliveryDevicePort;
        }

        public Model.Settings GetSettings()
        {
            Model.Settings settings = new Model.Settings();

            if (SettingsContainer.Values.ContainsKey("view.mode"))
            {
                settings.ViewMode = (Model.ViewMode)((int)SettingsContainer.Values["view.mode"]);
            }

            if (SettingsContainer.Values.ContainsKey("screen.type"))
            {
                settings.ScreenType = (Model.ScreenType)((int)SettingsContainer.Values["screen.type"]);
            }

            if (SettingsContainer.Values.ContainsKey("api.ip"))
            {
                settings.ApiIp = (string)SettingsContainer.Values["api.ip"];
            }

            if (SettingsContainer.Values.ContainsKey("api.port"))
            {
                settings.ApiPort = (int)SettingsContainer.Values["api.port"];
            }

            if (SettingsContainer.Values.ContainsKey("socket.port"))
            {
                settings.LocalSocketPort = (int)SettingsContainer.Values["socket.port"];
            }

            if (SettingsContainer.Values.ContainsKey("delivery.device.ip"))
            {
                settings.ApiIp = (string)SettingsContainer.Values["delivery.device.ip"];
            }

            if (SettingsContainer.Values.ContainsKey("delivery.device.port"))
            {
                settings.ApiPort = (int)SettingsContainer.Values["delivery.device.port"];
            }

            return settings;
        }
    }
}
