using Game_Kim_Cuong.Sources;
using System.Runtime.CompilerServices;

namespace Game_Kim_Cuong
{
    public partial class MainWindow : Form
    {
        #region Initialize
        //Private
        private BoardGame boardGame;

        #endregion

        #region Setting Default
        #endregion
        public MainWindow()
        {
            InitializeComponent();
            boardGame = new BoardGame();
        }


        private void MainWindow_Load(object sender, EventArgs e)
        {
            this.Controls.Add(boardGame);   
        }
    }
}
