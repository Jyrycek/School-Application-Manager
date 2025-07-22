using EvidencePrihlasek.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EvidencePrihlasek.EventHandlers
{
    public static class ButtonEventHandler
    {
        public static void BlockButton(Button button, Func<Window> windowFactory)
        {
            button.IsEnabled = false;

            Window window = windowFactory();

            window.Closed += (sender, args) => { button.IsEnabled = true; };

            window.Show();
        }
    }
}
