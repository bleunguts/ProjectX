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

        public string StatusText
        {
            get { return statusText; }
            set { statusText = value; NotifyOfPropertyChange(() => StatusText); }
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
        public void OnClick(object sender)
        {
            Button btn = sender as Button;
            int num = Convert.ToInt16(btn.Content.ToString().Split(' ')[1]);
            string ch = "Ch";
            if (num < 10)
                ch += "0" + num.ToString();
            else
                ch += num.ToString();

            Items.Clear();
            var sc = from q in screens where q.ToString().Contains(ch) orderby q.DisplayName select q;
            Items.AddRange(sc);

            var view = this.GetView() as ShellView;
            foreach (Button b in view.buttonPanel.Children)
            {
                b.Background = Brushes.Transparent;
                b.Foreground = Brushes.Black;
            }

            btn.Background = Brushes.Black;
            btn.Foreground = Brushes.White;
        }
    }
}
