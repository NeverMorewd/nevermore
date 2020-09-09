using CloudRoom.Home.Controls;
using CloudRoom.Home.Converters;
using CloudRoom.Home.ViewModels;
using CloudRoom.Infrastructure.Common;
using CloudRoom.Infrastructure.Model;
using CloudRoom.Infrastructure.Utils;
using CloudRoom.Nemo;
using CloudRoom.Nemo.AsrServer;
using CloudRoom.Nemo.Models;
using CloudRoom.Service;
using Microsoft.Practices.Prism.PubSubEvents;
using NemoSDK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace CloudRoom.Home.Windows
{
    /// <summary>
    /// SystemSettingsWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SystemSettingsWindow : Window,INotifyPropertyChanged
    {
        #region Constants
        public const string FIRST_PARTICIPANT = "CB0C9E11-BB68-4893-87D4-481A1054D585";

        #endregion

        #region Fields
        private SettingItemEnum _settingItem;
        private Visibility _popVisibility;
        private string _popMessage;
        private SolidColorBrush _popForegroud;
        private ObservableCollection<ClientModel> _participantList = new ObservableCollection<ClientModel>();
        private ObservableCollection<MicrophoneModel> _microphoneList;
        private ObservableCollection<SettingsDeviceListItemInfo> _deviceMicrophoneList = new ObservableCollection<SettingsDeviceListItemInfo>();
        private ObservableCollection<SettingsDeviceListItemInfo> _deviceCameraList = new ObservableCollection<SettingsDeviceListItemInfo>();
        private ObservableCollection<SettingsDeviceListItemInfo> _deviceGaopaiList = new ObservableCollection<SettingsDeviceListItemInfo>();
        private ObservableCollection<SettingsDeviceListItemInfo> _deviceSpeakerList = new ObservableCollection<SettingsDeviceListItemInfo>();
        private List<ClientModel> clientModelsList = new List<ClientModel>();
        private List<SettingsDeviceListItemInfo> _deviceInfoList = null;
        private DispatcherTimer _popTimer = null;
        private DevManagaer devManager = DevManagaer.getInstance();
        private AsrServerManager _asrManager = null;
        private readonly IEventAggregator _eventAggregator;
        private readonly NemoSdkManager _nemo;
        private IDataService _dataService;
        #endregion

        public SystemSettingsWindow(IDataService dataService, IEventAggregator anEventAggregator = null, NemoSdkManager aNemoSdkManager = null)
        {
            InitializeComponent();
            this.DataContext = this;
            _eventAggregator = anEventAggregator;
            _nemo = aNemoSdkManager;
            _dataService = dataService;
            _popTimer = new DispatcherTimer();
            _popTimer.Tick += new EventHandler(PopTimer_Tick);
            _popTimer.IsEnabled = false;
            _deviceInfoList = new List<SettingsDeviceListItemInfo>();

            LoadedCommand = new TFCommand<string>(OnLoaded);
            MicrophoneSelectionChangedCommand = new TFCommand<ExCommandParameter>(OnMicrophoneSelectionChanged);
            CameraCheckBoxCheckedCommand = new TFCommand<ExCommandParameter>(OnCameraCheckBoxChecked);
            GaopaiCheckBoxCheckedCommand = new TFCommand<ExCommandParameter>(OnGaopaiCheckBoxChecked);
            CameraCheckBoxUnCheckedCommand = new TFCommand<ExCommandParameter>(OnCameraCheckBoxUnChecked);
            GaopaiCheckBoxUnCheckedCommand = new TFCommand<ExCommandParameter>(OnGaopaiCheckBoxUnChecked);
            MicrophoneCheckBoxCheckedCommand = new TFCommand<ExCommandParameter>(OnMicrophoneCheckBoxChecked);
            MicrophoneUnCheckBoxCheckedCommand = new TFCommand<ExCommandParameter>(OnMicrophoneUnCheckBoxChecked);
            MicrophoneCheckBoxClickCommand = new TFCommand<ExCommandParameter>(OnMicrophoneCheckBoxClick);
            GaopaiCheckBoxClickCommand = new TFCommand<ExCommandParameter>(OnGaopaiCheckBoxClick);
            CameraCheckBoxClickCommand = new TFCommand<ExCommandParameter>(OnCameraCheckBoxClick);
            SpeakerCheckBoxClickCommand = new TFCommand<ExCommandParameter>(OnSpeakerCheckBoxClick);
            SpeakerCheckBoxUnCheckedCommand = new TFCommand<ExCommandParameter>(OnSpeakerCheckBoxUnChecked);
            SpeakerCheckBoxCheckedCommand = new TFCommand<ExCommandParameter>(OnSpeakerCheckBoxChecked);
            SaveCommand = new TFCommand<string>(OnSaveMicrophoneConfig);
            CancelCommand = new TFCommand<string>(OnCancelMicrophoneConfig);

            SettingItem = SettingItemEnum.Device;
            PopVisibility = Visibility.Collapsed;
        }


        #region Properties
        public SettingItemEnum SettingItem
        {
            get
            {
                return _settingItem;
            }
            set
            {
                if (Equals(_settingItem, value)) return;
                _settingItem = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ClientModel> ParticipantList
        {
            get 
            { 
                return _participantList; 
            }
            set 
            {
                if (Equals(_participantList, value)) return;
                _participantList = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<MicrophoneModel> MicrophoneList
        {
            get
            {
                return _microphoneList;
            }
            set
            {
                if (Equals(_microphoneList, value)) return;
                _microphoneList = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<SettingsDeviceListItemInfo> DeviceCameraList
        {
            get
            {
                return _deviceCameraList;
            }
            set
            {
                if (Equals(_deviceCameraList, value)) return;
                _deviceCameraList = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<SettingsDeviceListItemInfo> DeviceGaopaiList
        {
            get
            {
                return _deviceGaopaiList;
            }
            set
            {
                if (Equals(_deviceGaopaiList, value)) return;
                _deviceGaopaiList = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<SettingsDeviceListItemInfo> DeviceSpeakerList
        {
            get
            {
                return _deviceSpeakerList;
            }
            set
            {
                if (Equals(_deviceSpeakerList, value)) return;
                _deviceSpeakerList = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<SettingsDeviceListItemInfo> DeviceMicrophoneList
        {
            get
            {
                return _deviceMicrophoneList;
            }
            set
            {
                if (Equals(_deviceMicrophoneList, value)) return;
                _deviceMicrophoneList = value;
                OnPropertyChanged();
            }
        }

        public Visibility PopVisibility
        {
            get
            {
                return _popVisibility;
            }
            set
            {
                if (Equals(_popVisibility, value)) return;
                _popVisibility = value;
                OnPropertyChanged();
            }
        }
        public string PopMessage
        {
            get
            {
                return _popMessage;
            }
            set
            {
                if (Equals(_popMessage, value)) return;
                _popMessage = value;
                OnPropertyChanged();
            }
        }
        public SolidColorBrush PopForegroud
        {
            get
            {
                return _popForegroud;
            }
            set
            {
                if (Equals(_popForegroud, value)) return;
                _popForegroud = value;
                OnPropertyChanged();
            }
        }
        public TFCommand<ExCommandParameter> MicrophoneSelectionChangedCommand { get; set; }
        public TFCommand<ExCommandParameter> CameraCheckBoxCheckedCommand { get; set; }
        public TFCommand<ExCommandParameter> GaopaiCheckBoxCheckedCommand { get; set; }
        public TFCommand<ExCommandParameter> CameraCheckBoxUnCheckedCommand { get; set; }
        public TFCommand<ExCommandParameter> GaopaiCheckBoxUnCheckedCommand { get; set; }
        public TFCommand<ExCommandParameter> MicrophoneCheckBoxCheckedCommand { get; set; }
        public TFCommand<ExCommandParameter> MicrophoneUnCheckBoxCheckedCommand { get; set; }
        public TFCommand<ExCommandParameter> CameraCheckBoxClickCommand { get; set; }
        public TFCommand<ExCommandParameter> GaopaiCheckBoxClickCommand { get; set; }
        public TFCommand<ExCommandParameter> MicrophoneCheckBoxClickCommand { get; set; }
        public TFCommand<ExCommandParameter> SpeakerCheckBoxCheckedCommand { get; set; }
        public TFCommand<ExCommandParameter> SpeakerCheckBoxUnCheckedCommand { get; set; }
        public TFCommand<ExCommandParameter> SpeakerCheckBoxClickCommand { get; set; }
        public TFCommand<string> LoadedCommand { get; set; }
        public TFCommand<string> SaveCommand { get; set; }
        public TFCommand<string> CancelCommand { get; set; }
        #endregion
        private void OnLoaded(string obj)
        {
            ParticipantList.Add(new ClientModel { Id = FIRST_PARTICIPANT, TitleName = "（空）", IsSelected = false });
            foreach (Participant person in AppContext.Default.MeetingDetail.Participants)
            {
                if ((person.TitleGroup == 0 || person.TitleGroup == 1 || person.TitleGroup == 2)
                    &&!(UtilsMethods.Decrypt(person.Name) == "原告席") 
                    &&!(UtilsMethods.Decrypt(person.Name)== "被告席"))
                {
                    ParticipantList.Add(new ClientModel
                    {
                        Id = person.Id,
                        TitleName = person.Title + "-" + UtilsMethods.Decrypt(person.Name),
                        TitleGroup = person.TitleGroup,
                        Title = person.Title,
                        Name = UtilsMethods.Decrypt(person.Name),
                    });
                }
            }
            clientModelsList = ParticipantList.ToList();
            ClientModel clientModel = clientModelsList[0];
            MicrophoneList = new ObservableCollection<MicrophoneModel>
            {
                new MicrophoneModel{MicrophoneId = "（1）", MicPort = ConfigFile.micPort1of9,SelectedClient = clientModel},
                new MicrophoneModel{MicrophoneId = "（2）", MicPort = ConfigFile.micPort2of9,SelectedClient = clientModel},
                new MicrophoneModel{MicrophoneId = "（3）", MicPort = ConfigFile.micPort3of9,SelectedClient = clientModel},
                new MicrophoneModel{MicrophoneId = "（4）", MicPort = ConfigFile.micPort4of9,SelectedClient = clientModel},
                new MicrophoneModel{MicrophoneId = "（5）", MicPort = ConfigFile.micPort5of9,SelectedClient = clientModel},
                new MicrophoneModel{MicrophoneId = "（6）", MicPort = ConfigFile.micPort6of9,SelectedClient = clientModel},
                new MicrophoneModel{MicrophoneId = "（7）", MicPort = ConfigFile.micPort7of9,SelectedClient = clientModel},
                new MicrophoneModel{MicrophoneId = "（8）", MicPort = ConfigFile.micPort8of9,SelectedClient = clientModel},
                new MicrophoneModel{MicrophoneId = "（9）", MicPort = ConfigFile.micPort9of9,SelectedClient = clientModel},
            };
            _asrManager = AsrServerManager.GetInstance(_nemo, _dataService, _eventAggregator);
            List<string> res = _asrManager.GetAsrMicrophoneConfig();
            if (res != null && res.Count > 0)
            {
                for (int i = 0; i < res.Count; i++)
                {
                    MicrophoneList[i].TitleName = res[i];
                    string title = string.Empty, name = string.Empty;
                    ConvertString2TitleName(res[i],ref title,ref name);
                    MicrophoneList[i].Title = title;
                    MicrophoneList[i].Name = name;
                    ClientModel clientSelected = clientModelsList.FirstOrDefault(x => x.Id != FIRST_PARTICIPANT && x.Title.Equals(title) && x.Name.Equals(name));
                    if (clientSelected != null)
                    {
                        clientSelected.IsSelected = true;
                        MicrophoneList[i].SelectedClient = clientSelected;
                    }
                }
                ParticipantList = new ObservableCollection<ClientModel>(clientModelsList);
            }
            InitDeviceList();
        }
        
        private void InitDeviceList()
        {
            SelectedDevInfo selectedDevInfo = _nemo.GetSelectedDevInfo();
            _deviceInfoList.Clear();
            foreach (var item in _nemo.GetCameraDevices())
            {
                SettingsDeviceInfo info = new SettingsDeviceInfo();
                info.NemoDevInfoConverter(item, MediaDeviceType.Camera,
                    0 == string.CompareOrdinal(selectedDevInfo.CameraDevId, item.devId),
                    0 == string.CompareOrdinal(selectedDevInfo.SecondCameraDevId, item.devId));
                SettingsDeviceListItemInfo itemDev = new SettingsDeviceListItemInfo(info);
                _deviceInfoList.Add(itemDev);
            }
            InitCameraDeviceList(_deviceInfoList);
            ThreadPool.QueueUserWorkItem(delegate
            {
                SynchronizationContext.SetSynchronizationContext(new
                    DispatcherSynchronizationContext(Application.Current.Dispatcher));
                SynchronizationContext.Current.Post(pl =>
                {
                    PopMessage = "正在加载设备列表，轻稍后";
                    PopVisibility = Visibility.Visible;
                    InitMicrophoneDeviceList(_nemo.GetMicrophoneDevices(), selectedDevInfo.MicrophoneDevId);
                    InitSpeakerDeviceList(_nemo.GetSpeakerDevices(), selectedDevInfo.SpeakerDevId);
                    PopVisibility = Visibility.Collapsed;
                }, null);
            });

        }
        /// <summary>
        /// 初始化摄像头设备列表
        /// </summary>
        /// <param name="devInfos"></param>
        private void InitCameraDeviceList(List<SettingsDeviceListItemInfo> devInfos)
        {
            if (0 >= devInfos.Count)
                return;

            foreach (SettingsDeviceListItemInfo devInfo in devInfos)
            {
                if (devInfo.IsChecked)
                    devInfo.DevType = MediaDeviceType.Camera;
                else if(devInfo.IsSubChecked)
                    devInfo.DevType = MediaDeviceType.SubCamera;
                else
                    devInfo.DevType = MediaDeviceType.None;
                DeviceCameraList.Add(devInfo);
            }
        }
        /// <summary>
        /// 初始化扬声器设备列表
        /// </summary>
        /// <param name="devInfos"></param>
        private void InitSpeakerDeviceList(SDKMediaDevInfo[] devInfos, string selectedDevId)
        {
            if (0 >= devInfos.Length)
                return;

            foreach (SDKMediaDevInfo devInfo in devInfos)
            {
                SettingsDeviceInfo info = new SettingsDeviceInfo();
                if (0 == string.CompareOrdinal(selectedDevId, devInfo.devId))
                {
                    info.NemoDevInfoConverter(devInfo, MediaDeviceType.Speaker, true);
                }
                else
                {
                    info.NemoDevInfoConverter(devInfo, MediaDeviceType.Speaker, false);
                }
                SettingsDeviceListItemInfo itemDev = new SettingsDeviceListItemInfo(info);
                DeviceSpeakerList.Add(itemDev);
                DeviceSpeakerList = new ObservableCollection<SettingsDeviceListItemInfo>(DeviceSpeakerList.OrderBy(x => x.DevName).ToList());
            }
        }

        /// <summary>
        /// 初始化麦克风设备列表
        /// </summary>
        /// <param name="devInfos"></param>
        /// <param name="selectedDevId"></param>
        private void InitMicrophoneDeviceList(SDKMediaDevInfo[] devInfos, string selectedDevId)
        {
            if (0 >= devInfos.Length)
                return;
            foreach (SDKMediaDevInfo devInfo in devInfos)
            {
                SettingsDeviceInfo info = new SettingsDeviceInfo();
                if (0 == string.CompareOrdinal(selectedDevId, devInfo.devId))
                {
                    info.NemoDevInfoConverter(devInfo, MediaDeviceType.Microphone, true);
                }
                else
                {
                    info.NemoDevInfoConverter(devInfo, MediaDeviceType.Microphone, false);
                }
                SettingsDeviceListItemInfo itemDev = new SettingsDeviceListItemInfo(info);
                DeviceMicrophoneList.Add(itemDev);
            }
            DeviceMicrophoneList = new ObservableCollection<SettingsDeviceListItemInfo>(DeviceMicrophoneList.OrderBy(x => x.DevName).ToList());
        }
        /// <summary>
        /// 合议庭声卡麦克风配置
        /// </summary>
        /// <param name="e"></param>
        private void OnMicrophoneSelectionChanged(ExCommandParameter e)
        {
            SelectionChangedEventArgs eArgs = e.EventArgs as SelectionChangedEventArgs;
            if (eArgs.AddedItems.Count > 0)
            {
                ClientModel clientModelAdd = eArgs.AddedItems[0] as ClientModel;
                if (clientModelAdd?.Id != FIRST_PARTICIPANT)
                {
                    clientModelsList.FirstOrDefault(x => x.Id == clientModelAdd.Id).IsSelected = true;
                    //建立麦克风和参会人的映射
                    string microphoneId = e.Parameter as string;
                    MicrophoneModel microphoneModel = MicrophoneList.FirstOrDefault(x => x.MicrophoneId == microphoneId);
                    if (microphoneModel != null)
                    {
                        microphoneModel.Name = clientModelAdd.Name;
                        microphoneModel.Title = clientModelAdd.Title;
                    }
                }
                else if(clientModelAdd?.Id == FIRST_PARTICIPANT)//置空
                {
                    //clientModelsList.FirstOrDefault(x => x.Id == clientModelAdd.Id).IsSelected = true;
                    string microphoneId = e.Parameter as string;
                    MicrophoneModel microphoneModel = MicrophoneList.FirstOrDefault(x => x.MicrophoneId == microphoneId);
                    if (microphoneModel != null)
                    {
                        microphoneModel.Name = string.Empty;
                        microphoneModel.Title = string.Empty;
                    }
                }
            }
            if (eArgs.RemovedItems.Count > 0)
            {
                ClientModel clientModelRemove = eArgs.RemovedItems[0] as ClientModel;
                clientModelsList.FirstOrDefault(x => x.Id == clientModelRemove?.Id).IsSelected = false;
            }
            ParticipantList = new ObservableCollection<ClientModel>(clientModelsList);

        }
        private void OnCancelMicrophoneConfig(string obj)
        {
            this.Close();
        }
        private void OnSaveMicrophoneConfig(string obj)
        {
            try
            {
                int count = MicrophoneList.TakeWhile(x => !string.IsNullOrEmpty(x.Name) && !string.IsNullOrEmpty(x.Title)).Count();
                int count1 = MicrophoneList.Where(x => !string.IsNullOrEmpty(x.Name) && !string.IsNullOrEmpty(x.Title)).Count();
                if (count != count1)
                {
                    SetPopMessage("保存失败！请按顺序设置麦克风",new SolidColorBrush(Color.FromRgb(225,0,0)));
                    return;
                }
                _asrManager = AsrServerManager.GetInstance(_nemo, _dataService, _eventAggregator);
                _asrManager.Stop();

                devManager.SetMode(true);

                devManager.ClearDev();
                foreach (var deviceMic in DeviceMicrophoneList)
                {
                    if (deviceMic.DevName.StartsWith("UAS-1000"))
                    {
                        _asrManager.SetDefaultConfigWithoutStart(deviceMic.DevName);
                    }
                }
                foreach (var device in MicrophoneList)
                {
                    if (!(string.IsNullOrEmpty(device.Name) || string.IsNullOrEmpty(device.Title)))
                    {
                        devManager.AddDevInfo(device.Title, device.Name, device.MicPort);
                    }
                }
                ConfigFile.collegiateBenchSwich = 1;

                _asrManager.IsOpen = true;
                _asrManager.SetDefaultConfigStart();
                SetPopMessage("保存成功", new SolidColorBrush(Color.FromRgb(0, 255, 0)));
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex.Message);
            }
        }
        private void OnCameraCheckBoxChecked(ExCommandParameter e)
        {
            string currentDeviceID = e.Parameter as string;
            DeviceCameraList.FirstOrDefault(x => x.DevId == currentDeviceID).DevType = MediaDeviceType.Camera;
            _nemo.ChooseCameraDev(SDKCameraOrdinal.First, currentDeviceID);
        }
        private void OnCameraCheckBoxUnChecked(ExCommandParameter e)
        {
            string currentDeviceID = e.Parameter as string;
            DeviceCameraList.FirstOrDefault(x => x.DevId == currentDeviceID).DevType = MediaDeviceType.None;
        }
        private void OnGaopaiCheckBoxChecked(ExCommandParameter e)
        {
            string currentDeviceID = e.Parameter as string;
            DeviceCameraList.FirstOrDefault(x => x.DevId == currentDeviceID).DevType = MediaDeviceType.SubCamera;
            try
            {
                _nemo.ChooseCameraDev(SDKCameraOrdinal.Second, currentDeviceID);
            }
            catch
            {
                SetPopMessage("设备异常，请刷新重试！");
            }

        }
        private void OnGaopaiCheckBoxUnChecked(ExCommandParameter e)
        {
            string currentDeviceID = e.Parameter as string;
            DeviceCameraList.FirstOrDefault(x => x.DevId == currentDeviceID).DevType = MediaDeviceType.None;
        }
        private void OnMicrophoneCheckBoxChecked(ExCommandParameter e)
        {
            string currentDeviceID = e.Parameter as string;
            DeviceMicrophoneList.FirstOrDefault(x => x.DevId == currentDeviceID).DevType = MediaDeviceType.Microphone;
            try
            {
                _nemo.ChooseMicrophoneDev(currentDeviceID);
            }
            catch
            {
                SetPopMessage("设备异常，请刷新重试！");
            }
        }
        private void OnMicrophoneUnCheckBoxChecked(ExCommandParameter e)
        {
            string currentDeviceID = e.Parameter as string;
            DeviceMicrophoneList.FirstOrDefault(x => x.DevId == currentDeviceID).DevType = MediaDeviceType.None;
        }
        private void OnMicrophoneCheckBoxClick(ExCommandParameter e)
        {
            string currentDeviceID = e.Parameter as string;
            CheckBox sender = e.Sender as CheckBox;
            if (DeviceMicrophoneList.Where(x => x.IsChecked == true).Count() < 1)
            {
                SetPopMessage("麦克风至少保持一个为开启状态！");
                DeviceMicrophoneList.FirstOrDefault(x => x.DevId == currentDeviceID).IsChecked = true;
                return;
            }
            else 
            {
                SettingsDeviceListItemInfo device = DeviceMicrophoneList.FirstOrDefault(x => x.DevId != currentDeviceID && x.IsChecked);
                if (device != null)
                {
                    device.IsChecked = false;
                }
            }
        }
        private void OnCameraCheckBoxClick(ExCommandParameter e)
        {
            //Click事件在Checked&UnChecked之后触发
            string currentDeviceID = e.Parameter as string;
            if (DeviceCameraList.Where(x => x.DevType == MediaDeviceType.Camera && x.IsChecked).Count() < 1)
            {
                SetPopMessage("摄像头必须保持一个为开启状态！");
                DeviceCameraList.FirstOrDefault(x => x.DevId == currentDeviceID).IsChecked = true;
                return;
            }
            else
            {
                SettingsDeviceListItemInfo device = DeviceCameraList.FirstOrDefault(x => x.DevId != currentDeviceID && x.IsChecked && x.DevType == MediaDeviceType.Camera);
                if (device != null)
                {
                    device.IsChecked = false;
                }
            }
        }
        private void OnGaopaiCheckBoxClick(ExCommandParameter e)
        {
            string currentDeviceID = e.Parameter as string;
            SettingsDeviceListItemInfo device = DeviceCameraList.FirstOrDefault(x => x.DevId != currentDeviceID && x.IsChecked && x.DevType == MediaDeviceType.SubCamera);
            if (device != null)
            {
                device.IsChecked = false;
            }
        }

        private void OnSpeakerCheckBoxChecked(ExCommandParameter e)
        {
            string currentDeviceID = e.Parameter as string;
            DeviceSpeakerList.FirstOrDefault(x => x.DevId == currentDeviceID).DevType = MediaDeviceType.Speaker;
            try
            {
                _nemo.ChooseSpeakerDev(currentDeviceID);
            }
            catch
            {
                SetPopMessage("设备异常，请刷新重试！");
            }
        }
        private void OnSpeakerCheckBoxUnChecked(ExCommandParameter e)
        {
            string currentDeviceID = e.Parameter as string;
            DeviceSpeakerList.FirstOrDefault(x => x.DevId == currentDeviceID).DevType = MediaDeviceType.None;
        }
        private void OnSpeakerCheckBoxClick(ExCommandParameter e)
        {
            string currentDeviceID = e.Parameter as string;
            CheckBox sender = e.Sender as CheckBox;
            //扬声器只能开一个，也可以全关，只用UAS-1000时，默认全关
            if (DeviceSpeakerList.Where(x => x.IsChecked == true).Count() < 1)
            {
                //SetPopMessage("扬声器必须且只能保持一个为开启状态！");
                //DeviceSpeakerList.FirstOrDefault(x => x.DevId == currentDeviceID).IsChecked = true;
                //return;
            }
            else
            {
                SettingsDeviceListItemInfo device = DeviceSpeakerList.FirstOrDefault(x => x.DevId != currentDeviceID && x.IsChecked);
                if (device != null)
                {
                    device.IsChecked = false;
                }
            }
        }

        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void ConvertString2TitleName(string str, ref string Title, ref string Name)
        {
            string[] userDisNameArray = str.Split(' ');
            if (1 >= userDisNameArray.Length)
            {
                Title = str;
                Name = "";
                return;
            }

            Title = userDisNameArray[0];
            Name = userDisNameArray[1];
        }
        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
            eventArg.RoutedEvent = UIElement.MouseWheelEvent;
            eventArg.Source = sender;
            ScrollViewer scrollViewer = sender as ScrollViewer;
            scrollViewer.RaiseEvent(eventArg);
        }

        private void SetPopMessage(string str, SolidColorBrush solidColorBrush = null)
        {
            if(solidColorBrush == null)
            {
                solidColorBrush = new SolidColorBrush(Color.FromRgb(255,255,255));
            }
            PopMessage = str;
            PopVisibility = Visibility.Visible;
            PopForegroud = solidColorBrush;

            if (_popTimer == null)
            {
                _popTimer = new DispatcherTimer();
                _popTimer.Tick += PopTimer_Tick;
                _popTimer.IsEnabled = false;
            }
            if (_popTimer.IsEnabled)
            {
                _popTimer.Stop();
            }

            _popTimer.Interval = TimeSpan.FromSeconds(3);
            _popTimer.IsEnabled = true;
            _popTimer.Start();
        }

        private void PopTimer_Tick(object sender, EventArgs e)
        {
            PopMessage = string.Empty;
            PopVisibility = Visibility.Collapsed;

            _popTimer.Stop();
            _popTimer.IsEnabled = false;
        }
        #region Interface Implenmentations
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    public enum SettingItemEnum
    {
        Device = 1,
        CollegiateBench = 2
    }

    public class ClientModel:INotifyPropertyChanged
    {
        private bool _isSelected;
        public int TitleGroup { get; set; }
        public string Id { get; set; }
        public string TitleName { get; set; }
        public bool IsSelected
        {
            get 
            {
                return _isSelected;
            }
            set 
            {
                if (Equals(_isSelected, value)) return;
                _isSelected = value;
                OnPropertyChanged();
            }
        } 
        public string PhoneNumber { get; set; }
        public string ReserveId { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class MicrophoneModel: INotifyPropertyChanged
    {
        private string _titleName;
        private ClientModel _selectedClient;
        public string MicrophoneId { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public int MicPort { get; set; }
        public string TitleName 
        {
            get
            {
                return _titleName;
            }
            set
            {
                if (Equals(_titleName, value)) return;
                _titleName = value;
                OnPropertyChanged();
            }
        }
        public ClientModel SelectedClient
        {
            get
            {
                return _selectedClient;
            }
            set
            {
                if (Equals(_selectedClient, value)) return;
                _selectedClient = value;
                OnPropertyChanged();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class DeviceModel
    {
        public string DeviceId { get; set; }
        public string DeviceTitle { get; set; }
        public string DeviceName { get; set; }
        public bool DeviceIsEnable { get; set; }
    }

    public class TFCommand<T> : ICommand
    {
        private readonly Func<T, bool> _canExecute;
        private readonly Action<T> _execute;
        public TFCommand(Action<T> execute)
            : this(execute, null)
        {
        }
        public TFCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }
            this._execute = new Action<T>(execute);
            if (canExecute != null)
            {
                this._canExecute = new Func<T, bool>(canExecute);
            }
        }
        public bool CanExecute(object parameter)
        {
            if (this._canExecute == null)
            {
                return true;
            }
            if ((parameter == null) && typeof(T).IsValueType)
            {
                return this._canExecute(default(T));
            }
            if ((parameter == null) || (parameter is T))
            {
                return this._canExecute((T)parameter);
            }
            return false;
        }
        public event EventHandler CanExecuteChanged;
        public void Execute(object parameter)
        {
            object obj2 = parameter;
            if (this.CanExecute(obj2) && (this._execute != null))
            {
                if (obj2 == null)
                {
                    if (typeof(T).IsValueType)
                    {
                        this._execute(default(T));
                    }
                    else
                    {
                        this._execute((T)obj2);
                    }
                }
                else
                {
                    this._execute((T)obj2);
                }
            }
        }
        public void RaiseCanExecuteChanged()
        {
            this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
