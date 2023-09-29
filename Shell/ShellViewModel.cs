using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace Shell
{
    [Export(typeof(ShellViewModel))]
    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive, IConductor, IHandle<ModelEvents>
    {
        #region Bindable Properties       
        private string statusText = "Ready.";
        private int progressMin = 0;
        private int progressMax = 1;
        private int progressValue;

        public string StatusText
        {
            get { return statusText; }
            set { statusText = value; NotifyOfPropertyChange(() => StatusText); }
        }        

        public int ProgressMin
        {
            get { return progressMin; }
            set { progressMin = value; NotifyOfPropertyChange(() => ProgressMin); }
        }

        public int ProgressMax
        {
            get { return progressMax; }
            set { progressMax = value; NotifyOfPropertyChange(() => ProgressMax); }
        }        
        public int ProgressValue
        {
            get { return progressValue; }
            set { progressValue = value; }
        }
        #endregion

        private readonly IEnumerable<IScreen> screens;
        private readonly IEventAggregator events;

        [ImportingConstructor]
        public ShellViewModel([ImportMany] IEnumerable<IScreen> screens, IEventAggregator events)
        {
            this.screens = screens;
            this.events = events;
            Items.Clear();
            this.events.Subscribe(this);
            DisplayName = "ProjectX";
        }   
        public void OnClick(object sender)
        {
            Button? btn = sender as Button;
            var selectedScreen = btn!.Content.ToString();
           
            Items.Clear();
            var selectedScreens = screens
                                    .Where(s => s.ToString().Contains($"Screens.{selectedScreen}"))
                                    .OrderBy(s => s.DisplayName)
                                    .Select(s => s);            
            Items.AddRange(selectedScreens);

            var view = this.GetView() as ShellView;
            foreach (Button b in view!.buttonPanel.Children)
            {
                b.Background = Brushes.Transparent;
                b.Foreground = Brushes.Black;
            }

            btn.Background = Brushes.Black;
            btn.Foreground = Brushes.White;
        }

        public void Handle(ModelEvents message)
        {
            var events = message.EventList;
            StatusText = events.First().ToString();
        }
    }
}
