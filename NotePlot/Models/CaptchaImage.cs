using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace NotePlot.Models
{
    public class CaptchaImage :IDisposable
    {
        private string text; // текст капчи
        private int width; // ширина картинки
        private int height; // высота картинки
        public Bitmap Image { get; set; } // само изображение капчи

        public CaptchaImage(string s, int width, int height)
        {
            text = s;
            this.width = width;
            this.height = height;
            GenerateImage();
        }
        // создаем изображение
        private void GenerateImage()
        {
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            Graphics g = Graphics.FromImage(bitmap);
            // отрисовка строки
            RectangleF r = new RectangleF(0, 0, width, height);
            g.FillRectangle(Brushes.DarkGray, r);
            RectangleF r2 = new RectangleF(0, 5, width, height);//здесь выводим текст
            g.DrawString(text, new Font("Arial", height/2, FontStyle.Bold),
                                Brushes.WhiteSmoke, r2);

            g.Dispose();

            Image = bitmap;
        }

        ~CaptchaImage()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                Image.Dispose();
        }
    }
}
