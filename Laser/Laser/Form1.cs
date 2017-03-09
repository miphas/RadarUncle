using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Laser {
    public partial class Form1 : Form {

        // 激光雷达对象
        private Radar radar;
        private DrawRadarImg dImg = new DrawRadarImg();

        public Form1() {
            InitializeComponent();

            // 绑定更新界面方法
            updateBoardDel += new updateBoard(this.updateBoardMethod);
        }

        /// <summary>
        /// 打开关闭串口方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e) {
            radar = Radar.getInstance(this.textBox1.Text.Trim());
            Radar.afterProcessDel = new Radar.afterProcess(this.processData);
            if (!radar.IsOpen) {
                radar.openPort();
                this.button2.Text = "closePort";
                this.textBox1.Enabled = false;
            }
            else {
                radar.closePort();
                this.button2.Text = "openPort";
            }
        }

        /// <summary>
        /// 处理完数据执行画图方法
        /// </summary>
        public void processData() {
            Image img = dImg.drawImg(radar.Dist, radar.Back);
            if (updateBoardDel != null) {
                this.Invoke(updateBoardDel,img);
            }
        }

        /// <summary>
        /// 更新窗口委托
        /// </summary>
        /// <param name="img"></param>
        public delegate void updateBoard(Image img);
        public updateBoard updateBoardDel;

        public void updateBoardMethod(Image img) {
            this.pictureBox1.Image = img;
            this.textSpeed.Text = radar.Speed.ToString();
        }

    }
}
