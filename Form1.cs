using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using test;
using test2;
using test3;
using zjapi;
using static test.zjpush;

namespace testpusher
{
    

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //桌面 1 2160x1440:2560,0
        public void GetDesktopParam(string str, out Point p, out Size s)
        {
            p = new Point();
            s = new Size();
            string[] arr = str.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length > 0)
            {
                string[] s1 = arr[arr.Length - 1].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                if (s1.Length > 0)
                {
                    string[] s2 = s1[0].Split(new string[] { "x" }, StringSplitOptions.RemoveEmptyEntries);
                    if (s2.Length == 2)
                    {
                        s.Width = Convert.ToInt32(s2[0]);
                        s.Height = Convert.ToInt32(s2[1]);
                    }
                }

                if (s1.Length > 0)
                {
                    string[] s2 = s1[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    if (s2.Length == 2)
                    {
                        p.X = Convert.ToInt32(s2[0]);
                        p.Y = Convert.ToInt32(s2[1]);
                    }
                }
            }
        }

        int loglevel = (int)LOGLEVEL.AV_LOG_DEBUG;
        zjpush pusher = new zjpush();
        private void Form1_Load(object sender, EventArgs e)
        {
            live1 = new CLIVEPULLER(Path.Combine(Application.StartupPath, "zjpuller.dll"));


            Point p;
            Size s;
            GetDesktopParam("桌面 1 2160x1440:2560,0", out p, out s);
#if X64
            zjfacedetect.initdetectface(@"D:\ZJ_00\stream-media\vs2019-ffplay\Debug64");
#else
            zjfacedetect.initdetectface(@"F:\stream-media-svn\vs2019-ffplay\Debug");
#endif
            pusher.init(loglevel);

            StringBuilder sbDesktop = new StringBuilder(1024);
            zjpush.zjpusher_enumdesktop(sbDesktop, 1024);
            string[] sDestopItem = sbDesktop.ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            cbDesktop.Items.Clear();
            for (int i=0; i<sDestopItem.Length; i++)
            {
                cbDesktop.Items.Add("桌面 " + (i+1).ToString() + " " + sDestopItem[i]);
            }
            cbDesktop.SelectedIndex = 0;
            try
            {
                StringBuilder sbVideo = new StringBuilder(1024);
                StringBuilder sbAudio = new StringBuilder(1024);
                zjpush.zjpusher_enumdevices(sbVideo, 1024, sbAudio, 1024);
                string[] sVideoItem = sbVideo.ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                cbVideo.Items.Clear();
                for (int i = 0; i < sVideoItem.Length; i++)
                {
                    cbVideo.Items.Add(sVideoItem[i]);
                }
                cbVideo.SelectedIndex = 0;
                cbVideo.Items.Add("");

                cbAudio.Items.Clear();
                string[] sAudioItem = sbAudio.ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < sAudioItem.Length; i++)
                {
                    cbAudio.Items.Add(sAudioItem[i]);
                }
                cbAudio.SelectedIndex = 0;
                cbAudio.Items.Add("");
            }
            catch (Exception ex)
            { 
            
            }

            txtUrl.Text = "rtmp://121.36.242.194:1995/live?vhost=nodvr/test"; // "rtmp://172.28.189.22:1935/live/livestream"; // "rtmp://192.168.80.128:1935/live?vhost=live000/test";




            pusher2.init(loglevel);

        }

        private void BtnRefreshDev_Click(object sender, EventArgs e)
        {
            StringBuilder sbDesktop = new StringBuilder(1024);
            zjpush.zjpusher_enumdesktop(sbDesktop, 1024);
            string[] sDestopItem = sbDesktop.ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            cbDesktop.Items.Clear();
            for (int i = 0; i < sDestopItem.Length; i++)
            {
                cbDesktop.Items.Add("桌面 " + (i + 1).ToString() + " " + sDestopItem[i]);
            }
            cbDesktop.SelectedIndex = 0;

            StringBuilder sbVideo = new StringBuilder(1024);
            StringBuilder sbAudio = new StringBuilder(1024);
            zjpush.zjpusher_enumdevices(sbVideo, 1024, sbAudio, 1024);
            string[] sVideoItem = sbVideo.ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            cbVideo.Items.Clear();
            for (int i = 0; i < sVideoItem.Length; i++)
            {
                cbVideo.Items.Add(sVideoItem[i]);
            }
            cbVideo.SelectedIndex = 0;
            cbVideo.Items.Add("");

            cbCam2.Items.Clear();
            for (int i = 0; i < sVideoItem.Length; i++)
            {
                cbCam2.Items.Add(sVideoItem[i]);
            }
            cbCam2.SelectedIndex = 0;
            cbCam2.Items.Add("");



            cbAudio.Items.Clear();
            string[] sAudioItem = sbAudio.ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < sAudioItem.Length; i++)
            {
                cbAudio.Items.Add(sAudioItem[i]);
            }
            cbAudio.SelectedIndex = 0;
            cbAudio.Items.Add("");
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            // 推流 桌面+摄像头+麦克风
            int monitornum = cbDesktop.SelectedIndex;
            Size desktopsize = new Size(Convert.ToInt32(txtoutw.Text), Convert.ToInt32(txtouth.Text));
            string videodev = cbVideo.SelectedItem.ToString(); //"HD Camera";
            Size camsize = new Size(400, 280);
            string audiodev = cbAudio.SelectedItem.ToString(); //"麦克风阵列 (Realtek(R) Audio)";
            int framerate = Convert.ToInt32(txtfps.Text);
            int videorate = Convert.ToInt32(txtv.Text);
            int audiorate = Convert.ToInt32(txta.Text);
            string url = txtUrl.Text.Trim(); //"rtmp://49.232.131.207/wwlive/shawn";
            if (string.IsNullOrEmpty(url))
            {
                MessageBox.Show("请填写推流地址!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }


            pusher.pushbythread(monitornum, desktopsize, videodev, camsize, audiodev, framerate, videorate, audiorate, url, pictureBox1);
            //pusher.push_desktop_camera_audio_mouse(monitornum, -2160, 0, 2160, 1440, desktopsize.Width, desktopsize.Height,
            //    videodev, camsize.Width, camsize.Height,
            //    audiodev, framerate, videorate, audiorate, url, pictureBox1.Handle, false, 2);

            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = true;
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            // 推流 摄像头+麦克风
            string videodev = cbVideo.SelectedItem.ToString(); //"HD Camera";
            Size camsize = new Size(Convert.ToInt32(txtoutw.Text), Convert.ToInt32(txtouth.Text));

            string audiodev = cbAudio.SelectedItem.ToString(); //"麦克风阵列 (Realtek(R) Audio)";

            int framerate = Convert.ToInt32(txtfps.Text);
            int videorate = Convert.ToInt32(txtv.Text);
            int audiorate = Convert.ToInt32(txta.Text);

            string url = txtUrl.Text.Trim(); //"rtmp://49.232.131.207/wwlive/shawn";
            if (string.IsNullOrEmpty(url))
            {
                MessageBox.Show("请填写推流地址!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }


            pusher.pushbythread_CA(videodev, camsize, audiodev, framerate, videorate, audiorate, url, pictureBox1);
            
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = true;
        }
        
        private void Button3_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = true;

            // 推流 桌面+麦克风
            int monitornum = cbDesktop.SelectedIndex;
            Size desktopsize = new Size(Convert.ToInt32(txtoutw.Text), Convert.ToInt32(txtouth.Text));

            string audiodev = cbAudio.SelectedItem.ToString(); //"麦克风阵列 (Realtek(R) Audio)";

            int framerate = Convert.ToInt32(txtfps.Text);
            int videorate = Convert.ToInt32(txtv.Text);
            int audiorate = Convert.ToInt32(txta.Text);

            string url = txtUrl.Text.Trim(); //"rtmp://49.232.131.207/wwlive/shawn";
            if (string.IsNullOrEmpty(url))
            {
                MessageBox.Show("请填写推流地址!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            pusher.pushbythread_DA(monitornum, desktopsize, audiodev, framerate, videorate, audiorate, url, pictureBox1);
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            pusher.close();

            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = false;

            button5.Enabled = true;

            pictureBox1.Update();
            pictureBox1.Refresh();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            pusher.uninit();
            g_fdStop = true;
            if (g_fdthread != null)
            {
                g_fdthread.Abort();
                g_fdthread = null;
            }
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            int monitornum = cbDesktop.SelectedIndex;
            
            Size desktopsize = new Size(Convert.ToInt32(txtMonitorOutWidth.Text), Convert.ToInt32(txtMonitorOutHeight.Text));
            string videodev = "";
            try { 
                videodev = cbVideo.SelectedItem.ToString(); //"HD Camera";
            }
            catch (Exception ex)
            {
                MessageBox.Show("请接入摄像头！");
            }
            Size camsize = new Size(Convert.ToInt32(txtCamWidth.Text), Convert.ToInt32(txtCamHeight.Text));
            string audiodev = cbAudio.SelectedItem.ToString(); //"麦克风阵列 (Realtek(R) Audio)";
            int framerate = 20;
            int videorate = 1280000;
            int audiorate = 960000;
            string url = txtUrl.Text.Trim(); //"rtmp://49.232.131.207/wwlive/shawn";
            bool bOnlyPreview = ckbPreview.Checked;
            /*
            long lret = zjpush.zjpusher_openstream_dca_ex(monitornum, Convert.ToInt32(txtMonitorX.Text), Convert.ToInt32(txtMonitorY.Text), Convert.ToInt32(txtMonitorWidth.Text), Convert.ToInt32(txtMonitorHeight.Text), desktopsize.Width, desktopsize.Height, 
                                                          videodev, Convert.ToInt32(txtCamX.Text), Convert.ToInt32(txtCamY.Text), camsize.Width, camsize.Height, 
                                                          audiodev, framerate, videorate, audiorate, url, pictureBox1.Handle, bOnlyPreview);
            //*/

            //long lret = zjpush.zjpusher_openstream_da_ex(monitornum, Convert.ToInt32(txtMonitorX.Text), Convert.ToInt32(txtMonitorY.Text), Convert.ToInt32(txtMonitorWidth.Text), Convert.ToInt32(txtMonitorHeight.Text), desktopsize.Width, desktopsize.Height, 
            //                                              audiodev, framerate, videorate, audiorate, url, pictureBox1.Handle, bOnlyPreview);
            //long lret = zjpush.zjpusher_openstream_ca(videodev, camsize.Width, camsize.Height, audiodev, framerate, videorate, audiorate, url, pictureBox1.Handle);
            DRAWTEXT_ST stDrawText = new DRAWTEXT_ST();
            stDrawText.fontsize = 40;
            stDrawText.szText = "一二三四";// "技术是第一生产力";
            stDrawText.x = 10; stDrawText.y = 10;

            IntPtr pBuff = Marshal.AllocHGlobal(Marshal.SizeOf(stDrawText));
            Marshal.StructureToPtr(stDrawText, pBuff, false);
            long lret = zjpusher_openstream_ca_ex(videodev, camsize.Width, camsize.Height, pBuff, audiodev, framerate, videorate, audiorate, url, pictureBox1.Handle, bOnlyPreview, 2);

            button5.Enabled = false;

            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = true;
        }

        private void BtnpushDC_Click(object sender, EventArgs e)
        {
            // 推流 桌面+摄像头+麦克风
            int monitornum = cbDesktop.SelectedIndex;
            Size desktopsize = new Size(Convert.ToInt32(txtoutw.Text), Convert.ToInt32(txtouth.Text));
            string videodev = "";
            try
            {
                videodev = cbVideo.SelectedItem.ToString(); //"HD Camera";
            }
            catch (Exception ex)
            {
                MessageBox.Show("请接入摄像头！");
            }
            Size camsize = new Size(480, 256);
           // string audiodev = cbAudio.SelectedItem.ToString(); //"麦克风阵列 (Realtek(R) Audio)";
            int framerate = Convert.ToInt32(txtfps.Text);
            int videorate = Convert.ToInt32(txtv.Text);
            int audiorate = Convert.ToInt32(txta.Text);
            string url = txtUrl.Text.Trim(); //"rtmp://49.232.131.207/wwlive/shawn";
            if (string.IsNullOrEmpty(url))
            {
                MessageBox.Show("请填写推流地址!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // start 解析桌面的真实的宽高
            //桌面 1 2160x1440:2560,0
            int iRealMonitorWidth = 0;      // ComboBox中解析到选中物理桌面的真实宽
            int iRealMonitorHeight = 0;     // ComboBox中解析到选中物理桌面的真实高

            int iRealMonitorX = 0;          // ComboBox中解析到选中物理桌面的真实X坐标
            int iRealMonitorY = 0;          // ComboBox中解析到选中物理桌面的真实Y坐标

            string s = cbDesktop.SelectedItem.ToString();
            string[] arr = s.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length > 0)
            {
                string[] s1 = arr[arr.Length - 1].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                if (s1.Length > 0)
                {
                    string[] s2 = s1[0].Split(new string[] { "x" }, StringSplitOptions.RemoveEmptyEntries);
                    if (s2.Length == 2)
                    {
                        iRealMonitorWidth = Convert.ToInt32(s2[0]);
                        iRealMonitorHeight = Convert.ToInt32(s2[1]);
                    }
                }
            }
            // end 解析桌面的真实的宽高

            zjpush.zjpusher_openstream_dcm(monitornum, iRealMonitorX, iRealMonitorY, iRealMonitorWidth, iRealMonitorHeight, 
                                          desktopsize.Width, desktopsize.Height, 
                                          videodev, camsize.Width, camsize.Height, framerate, videorate, url, pictureBox1.Handle, true, 1);

            button4.Enabled = true;
        }

        private void BtnPushD_Click(object sender, EventArgs e)
        {
            // 推流 桌面+摄像头+麦克风
            int monitornum = cbDesktop.SelectedIndex;
            Size desktopsize = new Size(Convert.ToInt32(txtoutw.Text), Convert.ToInt32(txtouth.Text));
            //string videodev = cbVideo.SelectedItem.ToString(); //"HD Camera";
            Size camsize = new Size(256, 256);
            //string audiodev = cbAudio.SelectedItem.ToString(); //"麦克风阵列 (Realtek(R) Audio)";
            int framerate = Convert.ToInt32(txtfps.Text);
            int videorate = Convert.ToInt32(txtv.Text);
            int audiorate = Convert.ToInt32(txta.Text);
            string url = txtUrl.Text.Trim(); //"rtmp://49.232.131.207/wwlive/shawn";
            if (string.IsNullOrEmpty(url))
            {
                MessageBox.Show("请填写推流地址!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // start 解析桌面的真实的宽高
            //桌面 1 2160x1440:2560,0
            int iRealMonitorWidth = 0;      // ComboBox中解析到选中物理桌面的真实宽
            int iRealMonitorHeight = 0;     // ComboBox中解析到选中物理桌面的真实高

            int iRealMonitorX = 0;          // ComboBox中解析到选中物理桌面的真实X坐标
            int iRealMonitorY = 0;          // ComboBox中解析到选中物理桌面的真实Y坐标

            string s = cbDesktop.SelectedItem.ToString();
            string[] arr = s.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length > 0)
            {
                string[] s1 = arr[arr.Length - 1].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                if (s1.Length > 0)
                {
                    string[] s2 = s1[0].Split(new string[] { "x" }, StringSplitOptions.RemoveEmptyEntries);
                    if (s2.Length == 2)
                    {
                        iRealMonitorWidth = Convert.ToInt32(s2[0]);
                        iRealMonitorHeight = Convert.ToInt32(s2[1]);
                    }
                }
            }
            // end 解析桌面的真实的宽高 @"C:\Users\Dell\Desktop\srs-nginx.conf - Notepad++" @"C:\Users\Dell\Desktop\新建文本文档.txt - Notepad++"
            //zjpush.zjpusher_openstream_dmw_ex(monitornum, iRealMonitorX, iRealMonitorY, iRealMonitorWidth, iRealMonitorHeight,
            //                              desktopsize.Width, desktopsize.Height, framerate, videorate, url, pictureBox1.Handle, true, false,
            //                              "新建.txt - 记事本");


            zjpush.zjpusher_openstream_dm(monitornum, iRealMonitorX, iRealMonitorY, iRealMonitorWidth, iRealMonitorHeight,
                                          desktopsize.Width, desktopsize.Height, framerate, videorate, url, pictureBox1.Handle, false, 2);

            button4.Enabled = true;
        }

        private void BtnPushCam_Click(object sender, EventArgs e)
        {
            // 推流 摄像头+麦克风
            string videodev = "";
            try { 
                videodev = cbVideo.SelectedItem.ToString(); //"HD Camera";
            }
            catch (Exception ex)
            {
                MessageBox.Show("请接入摄像头！");
            }
            Size camsize = new Size(Convert.ToInt32(txtoutw.Text), Convert.ToInt32(txtouth.Text));

            //string audiodev = cbAudio.SelectedItem.ToString(); //"麦克风阵列 (Realtek(R) Audio)";

            int framerate = Convert.ToInt32(txtfps.Text);
            int videorate = Convert.ToInt32(txtv.Text);
            int audiorate = Convert.ToInt32(txta.Text);

            string url = txtUrl.Text.Trim(); //"rtmp://49.232.131.207/wwlive/shawn";
            if (string.IsNullOrEmpty(url))
            {
                MessageBox.Show("请填写推流地址!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //zjpush.zjpusher_openstream_multiwnd_c(videodev, camsize.Width, camsize.Height, framerate, videorate, url, pictureBox1.Handle, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, true);
            //zjpush.zjpusher_openstream_multiwnd_c(videodev, camsize.Width, camsize.Height, framerate, videorate, url, pictureBox1.Handle, pictureBox3.Handle, IntPtr.Zero, IntPtr.Zero, false);
            zjpush.zjpusher_openstream_c(videodev, camsize.Width, camsize.Height, framerate, videorate, url, pictureBox1.Handle, false, 2);

            button4.Enabled = true;
        }

        private static void facedetetctworker()
        {
            while (!g_fdStop)
            {
                string sSavePath = @"D:\picshot\face-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".jpg";
                zjpush.zjpusher_savepic(sSavePath);
                int ret = zjfacedetect.detectfacebypic(sSavePath);
                if (ret > 0)
                {
                    //MessageBox.Show("检测到文件" + filepath + " 存在人脸");
                }
                else
                {
                    //MessageBox.Show("未检测到文件" + filepath + " 存在人脸");
                }

                Thread.Sleep(1000 * 10);
                //Thread.Sleep(1000 * 60 * 20);
            }
        }
        Thread g_fdthread = null;
        static bool g_fdStop = false;
        private void BtnSavePic_Click(object sender, EventArgs e)
        {
            //zjpusher_openstream_a_rec("麦克风阵列 (Realtek(R) Audio)", "D:\\test.aac");

            //Thread.Sleep(1000 * 1000);
            ////zjpusher_openstream_a_rec_showwaves(@"D:\hardwaredetect\bin\x86\Debug\test.aac", @"D:\hardwaredetect\bin\x86\Debug\showwaves.mkv");
            //zjpusher_closestream();
            //return;
            SaveFileDialog sfd = new SaveFileDialog();
            if (DialogResult.OK == sfd.ShowDialog())
            {
                string sSavePath = sfd.FileName;
                zjpush.zjpusher_savepic(sSavePath);
            }

            g_fdthread = new Thread(facedetetctworker);
            g_fdthread.IsBackground = true;
            g_fdthread.Start();
        }

        private void BtnFaceDetect_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();//打开文件对话框 
            if (DialogResult.OK == ofd.ShowDialog())
            {
                string filepath = ofd.FileName;
                int ret = zjfacedetect.detectfacebypic(filepath);
                if (ret > 0)
                {
                    MessageBox.Show("检测到文件" + filepath + " 存在人脸");
                }
                else
                {
                    MessageBox.Show("未检测到文件" + filepath + " 存在人脸");
                }
            }
        }




        zjpush2 pusher2 = new zjpush2();

        private void Btn2PushCA_Click(object sender, EventArgs e)
        {
            // 推流 摄像头+麦克风
            string videodev = cbVideo.SelectedItem.ToString(); //"HD Camera";
            Size camsize = new Size(1024, 768);

            string audiodev = cbAudio.SelectedItem.ToString(); //"麦克风阵列 (Realtek(R) Audio)";

            int framerate = 15;
            int videorate = 1250000;
            int audiorate = 960000;

            string url = txt2PushAddress.Text.Trim();
            if (string.IsNullOrEmpty(url))
            {
                MessageBox.Show("请填写推流地址!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            pusher2.pushbythread_CA(videodev, camsize, audiodev, framerate, videorate, audiorate, url, pictureBox2);
        }

        private void Btn2PushDA_Click(object sender, EventArgs e)
        {
            // 推流 桌面+麦克风
            int monitornum = cbDesktop.SelectedIndex;
            Size desktopsize = new Size(1024, 768);

            string audiodev = cbAudio.SelectedItem.ToString(); //"麦克风阵列 (Realtek(R) Audio)";

            int framerate = 20;
            int videorate = 1250000;
            int audiorate = 1250000;

            string url = txt2PushAddress.Text.Trim();
            if (string.IsNullOrEmpty(url))
            {
                MessageBox.Show("请填写推流地址!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //pusher2.push_desktop_audio_mouse_ex(monitornum, 2560, 0, 2160, 1440, 792, 449, audiodev, framerate, videorate, audiorate, url, pictureBox2.Handle, false, true);
            pusher2.push_desktop_audio_mouse(monitornum, 2560, 0, 2160, 1440, 792, 449, audiodev, framerate, videorate, audiorate, url, pictureBox2.Handle, true, 2);//
            //pusher2.pushbythread_DA(monitornum, desktopsize, audiodev, framerate, videorate, audiorate, url, pictureBox2);
        }

        private void Btn2PushClose_Click(object sender, EventArgs e)
        {
            pusher2.close();

            pictureBox2.Update();
            pictureBox2.Refresh();
        }

        private void BtnAudio_Click(object sender, EventArgs e)
        {
            pusher.m_audiodev = cbAudio.SelectedItem.ToString();

            pusher.m_audiorate = 1250000;

            pusher.m_url = txtUrl.Text.Trim();
            pusher.push_mic(IntPtr.Zero);
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            zjpusher_setmute(true);
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            zjpusher_setmute(false);
        }

        public long OnPullEvent(string sid, string url, int iMsg, string szMsg)
        {
            Debug.WriteLine("sid:" + sid + " url:" + url + " error:" + iMsg.ToString() + " " + szMsg);
            return 0;
        }

        string slive1 = "rtmp://media3.sinovision.net:1935/live/livestream";
        CLIVEPULLER live1 = null;

        PCALLBACK pcb1 = null;
        private void btnPullStream_Click(object sender, EventArgs e)
        {
            int width = 800;
            int height = 600;
            pcb1 = new PCALLBACK(OnPullEvent);
            //live1.play(txturl.Text.Trim(), pblive1, 1);
            //pblive1.Location = new Point(10, 10);
            //pblive1.Size = new Size(width, height);
            //pblive1.Dock = DockStyle.None;
            
            //http://ivi.bupt.edu.cn/hls/cctv5phd.m3u8

            live1.play_ex("sinovision", width, height, txtpullurl.Text.Trim(), pictureBox2, 0, pcb1);
        }

        private void btnClosePull_Click(object sender, EventArgs e)
        {
            live1.stop();

            pictureBox2.Update();
            pictureBox2.Refresh();
        }

        private void btnCAM2_Click(object sender, EventArgs e)
        {
            int monitornum = cbDesktop.SelectedIndex;

            Size desktopsize = new Size(Convert.ToInt32(txtMonitorOutWidth.Text), Convert.ToInt32(txtMonitorOutHeight.Text));
            string videodev = cbCam2.SelectedItem.ToString(); //"HD Camera";
            Size camsize = new Size(Convert.ToInt32(txtCamWidth.Text), Convert.ToInt32(txtCamHeight.Text));
            string audiodev = cbAudio.SelectedItem.ToString(); //"麦克风阵列 (Realtek(R) Audio)";
            int framerate = 20;
            int videorate = 1280000;
            int audiorate = 960000;
            string url = txt2PushAddress.Text.Trim(); //"rtmp://49.232.131.207/wwlive/shawn";
            bool bOnlyPreview = ckbPreview.Checked;

            DRAWTEXT_ST stDrawText = new DRAWTEXT_ST();
            stDrawText.fontsize = 40;
            stDrawText.szText = "一二三四";// "技术是第一生产力";
            stDrawText.x = 10; stDrawText.y = 10;

            IntPtr pBuff = Marshal.AllocHGlobal(Marshal.SizeOf(stDrawText));
            Marshal.StructureToPtr(stDrawText, pBuff, false);

            
            long lret = zjpush2.zjpusher_openstream_ca(videodev,
                camsize.Width,
                camsize.Height,
                audiodev, framerate, videorate, audiorate, url, pictureBox2.Handle, 2);
            //long lret = zjpush2.zjpusher_openstream_ca_ex(videodev, 
            //    camsize.Width, 
            //    camsize.Height, 
            //    pBuff, audiodev, framerate, videorate, audiorate, url, pictureBox2.Handle, bOnlyPreview, 2);
        }

        private void btnCloseCAM2_Click(object sender, EventArgs e)
        {
            pusher2.close();

            pictureBox2.Update();
            pictureBox2.Refresh();
        }

        zjpush3 pusher3 = new zjpush3();

        private void btnAB_Click(object sender, EventArgs e)
        {
            Size desktopsize = new Size(Convert.ToInt32(txtMonitorOutWidth.Text), Convert.ToInt32(txtMonitorOutHeight.Text));
            string videodev = cbVideo.SelectedItem.ToString(); //"HD Camera";
            Size camsize = new Size(Convert.ToInt32(txtCamWidth.Text), Convert.ToInt32(txtCamHeight.Text));
            string audiodev = cbAudio.SelectedItem.ToString(); //"麦克风阵列 (Realtek(R) Audio)";
            int framerate = 20;
            int videorate = 1280000;
            int audiorate = 960000;
            string url = txtfinalpushaddr.Text.Trim(); //"rtmp://49.232.131.207/wwlive/shawn";
            bool bOnlyPreview = ckbPreview.Checked;

            zjpush3.zjpusher_openstream_combine_ex( videodev, Convert.ToInt32(txtMonitorX.Text), Convert.ToInt32(txtMonitorY.Text), Convert.ToInt32(txtMonitorWidth.Text), Convert.ToInt32(txtMonitorHeight.Text), 
                                                    desktopsize.Width, desktopsize.Height,
                                                    cbCam2.SelectedItem.ToString(), Convert.ToInt32(txtCamX.Text), Convert.ToInt32(txtCamY.Text), camsize.Width, camsize.Height, 
                                                    audiodev, framerate, videorate, audiorate, url, pictureBox3.Handle, false);
        }

        private void btnBA_Click(object sender, EventArgs e)
        {
            Size desktopsize = new Size(Convert.ToInt32(txtMonitorOutWidth.Text), Convert.ToInt32(txtMonitorOutHeight.Text));
            string videodev = cbVideo.SelectedItem.ToString(); //"HD Camera";
            Size camsize = new Size(Convert.ToInt32(txtCamWidth.Text), Convert.ToInt32(txtCamHeight.Text));
            string audiodev = cbAudio.SelectedItem.ToString(); //"麦克风阵列 (Realtek(R) Audio)";
            int framerate = 20;
            int videorate = 1280000;
            int audiorate = 960000;
            string url = txtfinalpushaddr.Text.Trim(); //"rtmp://49.232.131.207/wwlive/shawn";
            bool bOnlyPreview = ckbPreview.Checked;

            zjpush3.zjpusher_openstream_combine_ex(cbCam2.SelectedItem.ToString(), Convert.ToInt32(txtMonitorX.Text), Convert.ToInt32(txtMonitorY.Text), Convert.ToInt32(txtMonitorWidth.Text), Convert.ToInt32(txtMonitorHeight.Text),
                                                    desktopsize.Width, desktopsize.Height,
                                                    videodev, Convert.ToInt32(txtCamX.Text), Convert.ToInt32(txtCamY.Text), camsize.Width, camsize.Height,
                                                    audiodev, framerate, videorate, audiorate, url, pictureBox3.Handle, bOnlyPreview);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            pusher3.close();

            pictureBox3.Update();
            pictureBox3.Refresh();
        }

        private void button9_Click(object sender, EventArgs e)
        {

            string audiodev = cbAudio.SelectedItem.ToString(); //"麦克风阵列 (Realtek(R) Audio)";
            int framerate = 20;
            int videorate = 1280000;
            int audiorate = 960000;
            string url = txtfinalpushaddr.Text.Trim(); //"rtmp://49.232.131.207/wwlive/shawn";
            bool bOnlyPreview = ckbPreview.Checked;
            //string title = "推流测试";
            string title = "fitcan_app";
            zjpush3.zjpusher_openstream_app_ex(title, 800, 600, cbAudio.SelectedItem.ToString(), framerate, videorate, audiorate, url, pictureBox3.Handle, false);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            pusher3.close();

            pictureBox3.Update();
            pictureBox3.Refresh();
        }

        private bool bEnableBeauty = true;
        private int ilevel = 0;
        private void BtnBeauty_Click(object sender, EventArgs e)
        {
            //bEnableBeauty = !bEnableBeauty;
            //if (bEnableBeauty)
            //{
            //    zjpush.zjpusher_beautylevel(2);
            //}
            //else
            //{
            //    zjpush.zjpusher_beautylevel(0);
            //}

            zjpush.zjpusher_beautylevel(ilevel);
            ilevel = (++ilevel % 3);
        }

        public long facedetector_callbackfun(string filepath, bool bResult)
        {
            if(bResult)
                MessageBox.Show(filepath, "检测到人", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show(filepath, "未检测到人", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            return 0L;
        }

        FACEDETECTORCALLBACK m_facedetector_pcb = null;
        private void Button11_Click(object sender, EventArgs e)
        {
            m_facedetector_pcb = new FACEDETECTORCALLBACK(facedetector_callbackfun);
            zjpusher_facedetector(6, "D:\\testfolder\\aaa.jpg", m_facedetector_pcb);
        }
    }
}
