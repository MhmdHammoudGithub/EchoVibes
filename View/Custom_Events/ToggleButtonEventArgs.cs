using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace EchoVibe.View.Custom_Events
{
    public class ToggleButtonEventArgs : EventArgs
    {
        public ToggleButton ToggledButton { get; }

        public ToggleButtonEventArgs(ToggleButton toggledButton)
        {
            ToggledButton = toggledButton;
        }
    }
}
