using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Laser {
    public class Radar: MPort {
        ///////////////////////////////////////////////////////////////////////////////////////////
        /// Attribute（属性域）
        ///////////////////////////////////////////////////////////////////////////////////////////
        // 距离信息
        private int[] dist = new int[360];
        // 强度信息
        private int[] back = new int[360];
        // 旋转速度
        private int speed;
        // 读写锁
        private Object readLock = new object();

        ///////////////////////////////////////////////////////////////////////////////////////////
        /// Attribute Modify（属性域修改）
        ///////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 距离信息
        /// </summary>
        public int[] Dist {
            get {
                lock (readLock) {
                    return dist;
                }
            }
            set {
                lock (readLock) {
                    dist = value;
                }
            }
        }
        /// <summary>
        /// 强度信息
        /// </summary>
        public int[] Back {
            get {
                lock (readLock) {
                    return back;
                }
            }
            set {
                lock (readLock) {
                    back = value;
                }
            }
        }
        /// <summary>
        /// 速度信息
        /// </summary>
        public int Speed {
            get {
                lock (readLock){
                    return speed;
                }
            }
            set {
                lock (readLock) {
                    speed = value;
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        /// Method（方法域）
        ///////////////////////////////////////////////////////////////////////////////////////////
        private Radar(String portName, int baudRate = 115200):base(
            portName, baudRate) {
                this.processDel -= new processData(this.processData);
                this.processDel += new processData(this.processData);
        }

        private static Radar radar;
        /// <summary>
        /// 单例模式，创建激光雷达类
        /// </summary>
        /// <param name="portName">串口号</param>
        /// <param name="baudRate">波特率</param>
        /// <returns>激光雷达实例</returns>
        public static Radar getInstance(String portName, int baudRate = 115200) {
            if (radar == null || !portName.Equals(radar.PortName)) {
                radar = new Radar(portName, baudRate);
            }
            return radar;
        }

        /// <summary>
        /// 数据头
        /// </summary>
        public static byte dHead = (byte)0xFA;
        /// <summary>
        /// 数据帧长度
        /// </summary>
        public static int dLen = 22;
        /// <summary>
        /// 处理激光雷达获取的数据信息
        /// </summary>
        /// <param name="data">获取的数据</param>
        public void processData(byte[] data) {
            // 该数据完整则进行处理
            for (int i = 0; i < data.Length && (i + dLen - 1 < data.Length); ) {
                // 寻找帧头
                if (data[i] != dHead) {
                    ++i;
                    continue;
                }
                // 索引号
                int index = data[i + 1];
                Speed = data[i + 2] + 256 * (data[i + 3]);
                // 计算距离和强度信息
                for (int j = 0; j < 4; ++j) {
                    int tag = (index - 160) * 4 + j;
                    if (tag < 0 || tag >= 360) {
                        break;
                    }
                    Dist[tag] = data[i + 3 + j * 4 + 1] + 256 * (data[i + 3 + j * 4 + 2] & 0x3F);
                    Back[tag] = data[i + 3 + j * 4 + 3] + 256 * data[i + 3 + j * 4 + 4];
                }
                i += dLen;
            }
            // 执行数据处理完成后的方法
            if (afterProcessDel != null) {
                afterProcessDel();
            }
        }
        /// <summary>
        /// 定义数据处理后执行方法
        /// </summary>
        public delegate void afterProcess();
        public static afterProcess afterProcessDel;

    }
}
