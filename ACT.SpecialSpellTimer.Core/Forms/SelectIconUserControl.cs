using System.Windows.Forms;
using ACT.SpecialSpellTimer.Image;

namespace ACT.SpecialSpellTimer.Forms
{
    public partial class SelectIconUserControl :
        UserControl
    {
        public SelectIconUserControl()
        {
            this.InitializeComponent();
        }

        public Button Button => this.IconButton;

        public IconController.IconFile Icon { get; set; }
        public Label Label => this.IconLabel;
    }
}
