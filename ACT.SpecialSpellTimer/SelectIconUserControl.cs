using System.Windows.Forms;
using ACT.SpecialSpellTimer.Image;

namespace ACT.SpecialSpellTimer
{
    public partial class SelectIconUserControl : 
        UserControl
    {
        public Button Button => this.IconButton;

        public Label Label => this.IconLabel;

        public IconController.IconFile Icon { get; set; }

        public SelectIconUserControl()
        {
            this.InitializeComponent();
        }
    }
}
