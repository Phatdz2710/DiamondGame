using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Kim_Cuong.Sources
{
    public class LoadAssets
    {
        #region Initialize
        private List<Image> ListImage;
        private string AssetFolderDirectory = Application.StartupPath + "\\Assets";
        #endregion

        public LoadAssets()
        {
            this.ListImage = new List<Image>();
            this.LoadImageFromDirectory();
        }

        private void LoadImageFromDirectory()
        {
            try
            {
                string[] files = Directory.GetFiles(AssetFolderDirectory, "*.png");

                foreach (string file in files)
                {
                    Image image = Image.FromFile(file);
                    ListImage.Add(image);
                }
            } 
            catch
            { 
                MessageBox.Show("Vui lòng kiểm tra lại tệp tin Assets !!!", "ERROR");
            }
        }


        public Image ElementAt(int index)
        {
            if (ListImage.Count == 0 || index >= ListImage.Count) return new Bitmap(1, 1);
            return this.ListImage[index];
        }
    }
}
