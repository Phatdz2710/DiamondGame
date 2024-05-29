using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Game_Kim_Cuong.Sources
{
    public class Diamond : Button
    {
        
        //Enum Diamond Tag
        public enum DiamondTag
        {
            Ayaka, Hutao, Lumine, Yoimiya, Keqing, Ganyu, Xiao, Jean, None
        }
        #region Initialize

        //background
        private Image _background { get; set; }

        //Tag
        public DiamondTag bTag { get; private set; }
        //Toa do
        public Point point { get; set; }
        #endregion

        #region Setting Default
        #endregion

        public static Diamond DiamondNull = new Diamond();

        //Constructor
        public Diamond(Image background, DiamondTag tag, Point point, int _width, int _heigth)
        {
            //Set size
            this.Width = _width;
            this.Height = _heigth;
            //Set background
            this._background = new Bitmap(background, new Size(_width, _heigth));
            //Others 
            this.bTag = tag;
            this.point = point;
            this.Reload();
        }

        public Diamond()
        {
            this.Width = this.Height = 0;
            this._background = new Bitmap(1, 1);
            this.bTag = DiamondTag.None;
            this.point = new Point(0, 0);
        }

        public void Reload()
        {
            this.BackgroundImage = this._background;
            this.BackgroundImageLayout = ImageLayout.Center;
        }

        public void ChangeImage(Image _img, DiamondTag _tag)
        {
            this._background = new Bitmap(_img, new Size(this.Width, this.Height));
            this.bTag = _tag;
            this.Reload();
        }

        public void HighLight(bool isOpen)
        {
            if (isOpen)
            {
                this.BackColor = Color.Red;
            }
            else this.BackColor = Color.White;
        }

        public void IsEat()
        {
            this.bTag = DiamondTag.None;
            this.BackgroundImage = new Bitmap(1, 1);
        }
    }
}
