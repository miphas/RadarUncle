using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Laser {
    public class DrawRadarImg {
        ///////////////////////////////////////////////////////////////////////////////////////////
        /// Attribute（属性域）
        ///////////////////////////////////////////////////////////////////////////////////////////
        // 图像宽度
        private int width;
        // 图像高度
        private int height;
        // 显示比例
        private double rate = 0.05;
        // 像素点大小
        private static int ps = 4;

        ///////////////////////////////////////////////////////////////////////////////////////////
        /// Attribute Modify（属性域修改）
        ///////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 图像宽度
        /// </summary>
        public int Width {
            get { return width; }
            set { width = value; }
        }
        /// <summary>
        /// 图像高度
        /// </summary>
        public int Height {
            get { return height; }
            set { height = value; }
        }
        /// <summary>
        /// 长度比例
        /// </summary>
        public double Rate {
            get { return rate; }
            set { rate = value; }
        }
        /// <summary>
        /// 一个测量点大小
        /// </summary>
        public int PS {
            get { return ps; }
            set { ps = value; }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        /// Method（方法域）
        ///////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 根据距离和强度信息画图
        /// </summary>
        /// <param name="dist">距离信息</param>
        /// <param name="back">强度信息</param>
        /// <returns></returns>
        public Image drawImg(int[] dist, int[] back) {
            Bitmap img = new Bitmap(400, 400);
            Graphics g = Graphics.FromImage(img);
            for (int i = 0; i < 360; ++i) {
                // 计算当前角度、X、Y坐标（偏差90度，与设定相关）
                double ang = ((i + 90) / 180.0) * Math.PI;
                double x = Math.Cos(ang) * dist[i] * rate;
                double y = Math.Sin(ang) * dist[i] * rate;
                // 调整强度显示的颜色
                Brush brush = (back[i] > 300) ? (Brushes.Red) :
                    (back[i] > 200) ? (Brushes.Green) :
                    (back[i] > 100) ? (Brushes.Blue) : Brushes.Purple;
                // 画点
                g.FillEllipse(brush, (int)(x + 200 - ps / 2), (int)(200 - (y - ps / 2)), ps, ps);
            }
            return img;
        }
    }
}
