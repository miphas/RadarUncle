using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Ports;

namespace Laser {
    public class MPort {
        ///////////////////////////////////////////////////////////////////////////////////////////
        /// Attribute（属性域）
        ///////////////////////////////////////////////////////////////////////////////////////////
        // 串口实例
        private SerialPort port;
        // 串口名和串口波特率
        private String portName = "COM3";
        private int baudRate = 115200;
        // 正在关闭、正在读取串口状态
        private bool isClosing;
        private bool isReading;
        // 串口返回数据
        private byte[] receData = new byte[2048];
        private Object receLock = new object();
        // 定义一个委托
        public delegate void processData(byte[] receData);
        public processData processDel;

        ///////////////////////////////////////////////////////////////////////////////////////////
        /// Attribute Modify（属性域修改）
        ///////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 串口名
        /// </summary>
        public String PortName{
            get{ return portName;}
            set{ portName = value;}
        }
        /// <summary>
        /// 波特率
        /// </summary>
        public int BaudRate{
            get{ return baudRate;}
            set{ baudRate = value;}
        }
        /// <summary>
        /// 读取数据
        /// </summary>
        public byte[] ReceData {
            get {
                lock (receLock) {
                    return receData;
                }
            }
            set {
                lock (receLock) {
                    receData = value;
                }
            }
        }
        /// <summary>
        /// 串口打开状态
        /// </summary>
        public bool IsOpen {
            get {
                return port.IsOpen;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        /// Method（方法域）
        ///////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 串口实例构造方法
        /// </summary>
        public MPort(String portName, int baudRate = 115200) {
            try {
                // 尝试关闭已使用的串口
                if (port != null) {
                    this.closePort();
                }
                // 尝试打开新串口
                this.PortName = portName;
                this.BaudRate = baudRate;
                port = new SerialPort(this.PortName, this.BaudRate);
            }
            catch {
                // "Check your port!"
            }
        }
        /// <summary>
        /// 打开串口方法
        /// </summary>
        /// <returns>打开是否成功</returns>
        public bool openPort() {
            // 注册读取串口事件
            port.DataReceived -= port_DataReceived;
            port.DataReceived += port_DataReceived;
            // 串口未打开时尝试打开串口
            if (!port.IsOpen) {
                isClosing = false;
                try {
                    port.Open();
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 关闭串口方法
        /// </summary>
        /// <returns>关闭串口是否成功</returns>
        public bool closePort() {
            if (!port.IsOpen) {
                return true;
            }
            isClosing = true;
            // 等待读取事件完毕
            while (isReading) {
                System.Windows.Forms.Application.DoEvents();
            }
            // 尝试关闭串口
            try {
                port.Close();
            }
            catch {
                return false;
            }
            return true;
        }
        

        /// <summary>
        /// 读取串口事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void port_DataReceived(object sender, SerialDataReceivedEventArgs e) {
            // 正在关闭则返回
            if (isClosing) {
                return;
            }
            // 设置正在读取状态
            isReading = true;
            try {
                int count = Math.Min(port.BytesToRead, ReceData.Length);
                port.Read(ReceData, 0, count);
                // 委托方法不为空，调用委托方法
                if (processDel != null) {
                    processDel(ReceData);
                }
            }
            finally {
                isReading = false;
            }
            
        }

        /// <summary>
        /// 向串口写入数据
        /// </summary>
        /// <param name="buff">写入数据的内容</param>
        /// <returns>是否写入成功的返回值</returns>
        public bool writeData(byte[] buff) {
            // 尝试向串口写入数据
            try {
                port.Write(buff, 0, buff.Length);
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

    }
}
