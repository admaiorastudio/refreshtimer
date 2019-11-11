using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace RefreshingTimer
{
    class TimedTask : BaseViewModel
    {
        private string _title;
        private DateTime _createDate;
        private TimeSpan _elapsedTime;

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public DateTime CreateDate
        {
            get => _createDate;
            set => SetProperty(ref _createDate, value);
        }

        public TimeSpan ElapsedTime
        {
            get => _elapsedTime;
            set => SetProperty(ref _elapsedTime, value);
        }
    }

    class MainPageViewModel : BaseViewModel
    {
        private List<Task> _timers;
        private CancellationTokenSource _cts;

        public ObservableCollection<TimedTask> Tasks { get; private set; }

        public ICommand AddTaskCommand => RegisterCommand(() =>
        {
            var timedTask = new TimedTask() { Title = $"This is task n. {_timers.Count + 1}", CreateDate = DateTime.Now, ElapsedTime = TimeSpan.FromSeconds(0) };
            var timer = Task.Run(async () =>
            {
                if (_cts.IsCancellationRequested)
                    return;

                while (!_cts.IsCancellationRequested)
                {
                    await Task.Delay(1000);
                    timedTask.ElapsedTime = (DateTime.Now - timedTask.CreateDate);
                }


            }, _cts.Token);

            _timers.Add(timer);
            this.Tasks.Add(timedTask);
        });

        public ICommand ClearAllTasks => RegisterCommand(() =>
        {
            _cts?.Cancel();
        });

        public MainPageViewModel()
        {
            _timers = new List<Task>();
            _cts = new CancellationTokenSource();

            this.Tasks = new ObservableCollection<TimedTask>();
        }
    }
    
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private MainPageViewModel _vm;

        public MainPage()
        {
            InitializeComponent();

            _vm = new MainPageViewModel();
            this.BindingContext = _vm;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _vm?.ClearAllTasks.Execute(null);
        }
    }
}
