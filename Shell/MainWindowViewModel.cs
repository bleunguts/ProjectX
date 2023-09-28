using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Shell
{
    [Export(typeof(MainWindowViewModel))]
    public class MainWindowViewModel : Conductor<IScreen>.Collection.OneActive, IConductor, IHandle<ModelEvents>
    {
        #region Bindable Properties       
        private string statusText = string.Empty;

        public string StatusText
        {
            get { return statusText; }
            set { statusText = value; NotifyOfPropertyChange(() => StatusText); }
        }

        #endregion

        private readonly IEnumerable<IScreen> screens;
        private readonly IEventAggregator events;

        [ImportingConstructor]
        public MainWindowViewModel([ImportMany] IEnumerable<IScreen> screens, IEventAggregator events)
        {
            this.screens = screens;
            this.events = events;
            Items.Clear();
            this.events.SubscribeOnPublishedThread(this);
            DisplayName = "ProjectX";
        }

        public Task HandleAsync(ModelEvents message, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                var events = message.EventList;
                StatusText = events.First().ToString();
            }, cancellationToken);
        }
    }
}
