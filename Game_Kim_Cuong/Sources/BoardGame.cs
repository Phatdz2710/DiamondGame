using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Game_Kim_Cuong.Sources
{
    public class BoardGame : FlowLayoutPanel
    {
        #region Const Value
        private const int _width = 900;
        private const int _height = 900;
        public int c_row {get; private set;} = 10; // So luong diamond tren mot hang
        public int c_col {get; private set;} = 10; // So luong diamond tren mot cot
        #endregion

        #region Initialize
        private int d_width { get; set; } //Chiều dài
        private int d_height { get; set; } //Chiều rộng

        public LoadAssets MyAssets { get; private set; } //Load Asset
        public List<List<Diamond>> diamondList; //Ma trận các Diamond

        private GameLogic gameLogic; //Xử lý các thuật toán

        private Diamond _focus = Diamond.DiamondNull; //Diamond đang được trỏ đến
        #endregion

        #region Setting Default
        //Setting kích thước mặc định
        private void SetDefaultSize() 
        {
            this.Height = _height;
            this.Width = _width;
            this.d_width = _width / c_row;
            this.d_height = _height / c_col;
        }

        //Khởi tạo bàn cờ
        private void CreateBoardGame()
        {
            Diamond.DiamondTag tag;
            int rand = 0;
            for (int i = 0; i < c_col; i++)
            {
                diamondList.Add(new List<Diamond>());
                for (int j = 0; j < c_row; j++)
                {
                    do
                    {
                        rand = RandNumInt(1, 8);
                        if (rand == 1) tag = Diamond.DiamondTag.Hutao;
                        else if (rand == 2) tag = Diamond.DiamondTag.Ayaka;
                        else if (rand == 3) tag = Diamond.DiamondTag.Ganyu;
                        else if (rand == 4) tag = Diamond.DiamondTag.Lumine;
                        else if (rand == 5) tag = Diamond.DiamondTag.Keqing;
                        else if (rand == 6) tag = Diamond.DiamondTag.Yoimiya;
                        else if (rand == 7) tag = Diamond.DiamondTag.Xiao;
                        else if (rand == 8) tag = Diamond.DiamondTag.Jean;
                        else tag = Diamond.DiamondTag.Hutao;

                    } while (!(this.CheckAbove(tag, new Point(i, j)) && this.CheckRight(tag, new Point(i, j))));

                    Diamond newDiamond = new Diamond(MyAssets.ElementAt(rand - 1), tag, new Point(i, j), d_width, d_height);
                    newDiamond.Click += Diamond_Click;

                    newDiamond.Margin = new Padding(0);
                    this.Controls.Add(newDiamond);
                    this.diamondList[i].Add(newDiamond);
                }
            }
        }
        #endregion

        

        //Constructor
        public BoardGame()
        {
            gameLogic = new GameLogic(this);
            //Set Default Size
            this.SetDefaultSize();

            //Create List Diamond Button
            diamondList = new List<List<Diamond>>();

            this.MyAssets = new LoadAssets();

            this.CreateBoardGame();
        
        }

        //Others
        //Random số
        public int RandNumInt(int begin, int end)
        {
            Random randomInt = new Random();
            return randomInt.Next(begin, end + 1);
        }


        #region Logic

        //Hoán đổi vị trí 2 Diamond

        //Kiểm tra phía bên trên
        private bool CheckAbove(Diamond.DiamondTag tag, Point _point)
        {
            if (_point.X < 2) return true;
            if (this.diamondList[_point.X - 1][_point.Y].bTag == this.diamondList[_point.X - 2][_point.Y].bTag
                && this.diamondList[_point.X - 1][_point.Y].bTag == tag) return false;
            return true;
        }

        //Kiểm tra phía bên phải
        private bool CheckRight(Diamond.DiamondTag tag, Point _point)
        {
            if (_point.Y < 2) return true;
            if (this.diamondList[_point.X][_point.Y - 1].bTag == this.diamondList[_point.X][_point.Y - 2].bTag
                && this.diamondList[_point.X][_point.Y - 1].bTag == tag) return false;
            return true;
        }

        #endregion

        #region Events
        private bool isTaskRunning = false; //Biến để lưu trạng thái đang xử lý các câu lệnh click không

        private async void Diamond_Click(object sender, EventArgs e)
        {
            if (isTaskRunning) // Kiểm tra xem có đang xử lý sự kiện click không
                return;

            isTaskRunning = true; // Đánh dấu rằng đang trong quá trình xử lý

            try
            {
                Diamond click = (Diamond)sender;
                if (click == _focus)
                {
                    _focus.HighLight(false);
                    _focus = Diamond.DiamondNull;
                }
                else if (_focus == Diamond.DiamondNull)
                {
                    _focus = click;
                    _focus.HighLight(true);
                }
                else
                {
                    _focus.HighLight(false);
                    click.HighLight(false);
                    if ((click.point.Y == _focus.point.Y && Math.Abs(click.point.X - _focus.point.X) == 1)
                        || (click.point.X == _focus.point.X && Math.Abs(click.point.Y - _focus.point.Y) == 1))
                    {
                        this.gameLogic.SwapDiamond(click, _focus);
                        await Task.Delay(500);

                        await this.gameLogic.DestroyMatchingDiamond(click, _focus);
                    }
                    
                    _focus = Diamond.DiamondNull;
                    this.gameLogic.Hint();
                }
            }
            finally
            {
                isTaskRunning = false; // Đánh dấu rằng xử lý đã hoàn thành
            }
        }

        #endregion
    }
}
