using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace zjapi
{
    

    //public delegate void ZJPULLER_INIT();
    //public delegate void ZJPULLER_UNINIT(IntPtr hHandle);

    public delegate long PCALLBACK(string sid, string url, int iMsg, string szMsg);

    public delegate long ZJPULLER_INIT();
    public delegate long ZJPULLER_OPENSTREAM(out IntPtr hHandle, string url, IntPtr hWnd, int iMuted/*0:SOUND 1:MUTE*/);
    public delegate long ZJPULLER_OPENSTREAMEX(out IntPtr hHandle, int VideoWidth, int VideoHeight, string sid, string url, IntPtr hWnd, int iMuted/*0:SOUND 1:MUTE*/, PCALLBACK pcb);
    public delegate long ZJPULLER_OPENSTREAM_LOG(out IntPtr hHandle, string url, IntPtr hWnd, int iMuted/*0:SOUND 1:MUTE*/, int loglevel);
    public delegate long ZJPULLER_SEEK(IntPtr pHandle, double dseekval);
    public delegate long ZJPULLER_GETMASTERCLOCK(IntPtr pHandle, out double pos);
    public delegate long ZJPULLER_SAVEPIC(IntPtr pHandle, string filepath);
    public delegate long ZJPULLER_PAUSESTREAM(IntPtr pHandle);
    public delegate long ZJPULLER_MUTE(IntPtr pHandle);
    public delegate void ZJPULLER_PLAYSOUND(IntPtr pHandle);
    public delegate void ZJPULLER_STOPSOUND(IntPtr pHandle);
    public delegate void ZJPULLER_CLOSESTREAM(IntPtr hHandle);
    
    public class THREADPARAM
    {
        public object obj;
        public string url;
    }

    public delegate void applyCompleteEventHandle();

    public class CLIVEPULLER
    {
        [DllImport("kernel32.dll", EntryPoint = "LoadLibrary")]
        public static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpLibFileName);

        [DllImport("kernel32.dll", EntryPoint = "GetProcAddress")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

        [DllImport("kernel32.dll", EntryPoint = "FreeLibrary")]
        public static extern bool FreeLibrary(IntPtr hModule);

        public string m_sid;
        public string m_url;
        public PictureBox m_pic;                // 保存用于跨线程操作时访问的控件对象
        private IntPtr m_hlive = IntPtr.Zero;  // 拉流接口返回的句柄，用于接口相关操作（非常重要）
        private int m_iMuted;    //0:开声音，1:关闭声音
        //private ZJPULLER_INIT m_myzjpuller_init = null;
        //private ZJPULLER_UNINIT m_myzjpuller_uninit = null;
        private ZJPULLER_INIT my_myzjpuller_init = null;
        private ZJPULLER_OPENSTREAM m_myzjpuller_openstream = null;
        private ZJPULLER_OPENSTREAMEX m_myzjpuller_openstreamex = null;
        private ZJPULLER_OPENSTREAM_LOG m_myzjpuller_openstream_log = null;
        private ZJPULLER_SEEK m_myzjpuller_seek = null;
        private ZJPULLER_GETMASTERCLOCK m_myzjpuller_getmasterclock = null;
        private ZJPULLER_SAVEPIC m_myzjpuller_savepic = null;
        private ZJPULLER_PAUSESTREAM m_myzjpuller_pausestream = null;
        private ZJPULLER_MUTE m_myzjpuller_mute = null;
        private ZJPULLER_PLAYSOUND m_myzjpuller_playsound = null;
        private ZJPULLER_STOPSOUND m_myzjpuller_stopsound = null;
        private ZJPULLER_CLOSESTREAM m_myzjpuller_closestream = null;

        IntPtr hModule = IntPtr.Zero;
        public IntPtr m_hHandle = IntPtr.Zero;
        public CLIVEPULLER(string dllpath)
        {
            Init(dllpath);
        }

        ~CLIVEPULLER()
        {
            FreeLibrary(hModule);
        }
        public long Init(string dllpath)
        {
            string dllWorkPath = Path.GetDirectoryName(dllpath);
            System.IO.Directory.SetCurrentDirectory(dllWorkPath);
            hModule = LoadLibrary(dllpath);
            if (hModule == IntPtr.Zero)
                return -1;
            System.IO.Directory.SetCurrentDirectory(Application.StartupPath);

            //IntPtr intPtrInit = GetProcAddress(hModule, "zjpuller_init");
            IntPtr intPtrOpenStream = GetProcAddress(hModule, "zjpuller_openstream");
            int err = Marshal.GetLastWin32Error();
            IntPtr intPtrOpenStreamex = GetProcAddress(hModule, "zjpuller_openstreamex");
            IntPtr intPtrOpenStreamlog = GetProcAddress(hModule, "zjpuller_openstream_log");
            IntPtr intPtrSeek = GetProcAddress(hModule, "zjpuller_seek");
            IntPtr intPtrGetMasterClock = GetProcAddress(hModule, "zjpuller_getmasterclock");
            IntPtr iniPtrSavePic = GetProcAddress(hModule, "zjpuller_savepic");
            IntPtr intPtrPauseStream = GetProcAddress(hModule, "zjpuller_pausestream");
            IntPtr intPtrMute = GetProcAddress(hModule, "zjpuller_mute");
            IntPtr intPtrPlaySound = GetProcAddress(hModule, "zjpuller_playsound");
            IntPtr intPtrStopSound = GetProcAddress(hModule, "zjpuller_stopsound");
            IntPtr intPtrCloseStream = GetProcAddress(hModule, "zjpuller_closestream");

            if (/*intPtrInit == IntPtr.Zero
                ||*/intPtrOpenStream == IntPtr.Zero
                || intPtrOpenStreamex == IntPtr.Zero
                || intPtrOpenStreamlog == IntPtr.Zero
                || intPtrSeek == IntPtr.Zero
                || intPtrGetMasterClock == IntPtr.Zero
                || iniPtrSavePic == IntPtr.Zero
                || intPtrPauseStream == IntPtr.Zero
                || intPtrMute == IntPtr.Zero
                || intPtrPlaySound == IntPtr.Zero
                || intPtrStopSound == IntPtr.Zero
                || intPtrCloseStream == IntPtr.Zero)
            {
                return -2;
            }

            //my_myzjpuller_init = (ZJPULLER_INIT)Marshal.GetDelegateForFunctionPointer(intPtrInit, typeof(ZJPULLER_INIT));
            m_myzjpuller_openstream = (ZJPULLER_OPENSTREAM)Marshal.GetDelegateForFunctionPointer(intPtrOpenStream, typeof(ZJPULLER_OPENSTREAM));
            m_myzjpuller_openstreamex = (ZJPULLER_OPENSTREAMEX)Marshal.GetDelegateForFunctionPointer(intPtrOpenStreamex, typeof(ZJPULLER_OPENSTREAMEX));
            m_myzjpuller_openstream_log = (ZJPULLER_OPENSTREAM_LOG)Marshal.GetDelegateForFunctionPointer(intPtrOpenStreamlog, typeof(ZJPULLER_OPENSTREAM_LOG));
            m_myzjpuller_seek = (ZJPULLER_SEEK)Marshal.GetDelegateForFunctionPointer(intPtrSeek, typeof(ZJPULLER_SEEK));
            m_myzjpuller_getmasterclock = (ZJPULLER_GETMASTERCLOCK)Marshal.GetDelegateForFunctionPointer(intPtrGetMasterClock, typeof(ZJPULLER_GETMASTERCLOCK));
            m_myzjpuller_savepic = (ZJPULLER_SAVEPIC)Marshal.GetDelegateForFunctionPointer(iniPtrSavePic, typeof(ZJPULLER_SAVEPIC));
            m_myzjpuller_pausestream = (ZJPULLER_PAUSESTREAM)Marshal.GetDelegateForFunctionPointer(intPtrPauseStream, typeof(ZJPULLER_PAUSESTREAM));
            m_myzjpuller_mute = (ZJPULLER_MUTE)Marshal.GetDelegateForFunctionPointer(intPtrPlaySound, typeof(ZJPULLER_MUTE));
            m_myzjpuller_playsound = (ZJPULLER_PLAYSOUND)Marshal.GetDelegateForFunctionPointer(intPtrPlaySound, typeof(ZJPULLER_PLAYSOUND));
            m_myzjpuller_stopsound = (ZJPULLER_STOPSOUND)Marshal.GetDelegateForFunctionPointer(intPtrStopSound, typeof(ZJPULLER_STOPSOUND));
            m_myzjpuller_closestream = (ZJPULLER_CLOSESTREAM)Marshal.GetDelegateForFunctionPointer(intPtrCloseStream, typeof(ZJPULLER_CLOSESTREAM));

            //my_myzjpuller_init();

            return 0;
        }

        public applyCompleteEventHandle my_applyCompleteEventHandle = null;
        private void applyComplete()
        {
            if (this.m_pic.InvokeRequired)
            {
                this.m_pic.Invoke(new applyCompleteEventHandle(applyComplete1));
            }
            else
            {
                applyComplete1();
            }
        }

        private void applyComplete1()
        {
            long lret = this.play(this.m_url, this.m_pic, m_iMuted);
        }

        private void WorkThread(object param)
        {
            my_applyCompleteEventHandle();
        }

        /// <summary>
        /// 以线程方式开始拉流,返回拉流上下文句柄，用于后续操作
        /// </summary>
        /// <param name="url">rtmp拉流地址</param>
        /// <param name="hwnd">要绘制的C# PictureBox控件对象</param>
        public void playbythread(string url, object hwnd, int iMuted)
        {
            my_applyCompleteEventHandle = new applyCompleteEventHandle(applyComplete);
            Thread t = new Thread(new ParameterizedThreadStart(WorkThread));
            THREADPARAM param = new THREADPARAM();
            param.obj = hwnd;
            param.url = url;

            this.m_url = url;
            this.m_pic = hwnd as PictureBox;
            this.m_iMuted = iMuted;
            t.Start(param);
        }

        /// <summary>
        /// 开始拉流,返回拉流上下文句柄，用于后续操作
        /// </summary>
        /// <param name="url">rtmp拉流地址</param>
        /// <param name="hwnd">要绘制的C# PictureBox控件对象</param>
        /// <returns></returns>
        public long play(string url, object hwnd, int iMuted)
        {
            if (m_myzjpuller_openstream != null)
            {
                m_url = url;
                m_pic = hwnd as PictureBox;
                return m_myzjpuller_openstream(out m_hHandle, url, (IntPtr)(((PictureBox)hwnd).Handle), iMuted);
            }
            return -1;
        }

        public enum LOGLEVEL
        {
            AV_LOG_TRACE = 56,
            AV_LOG_DEBUG = 48,
            AV_LOG_WARNING = 24,
            AV_LOG_ERROR = 16,
            AV_LOG_FATAL = 8,
            AV_LOG_PANIC = 0,
            AV_LOG_QUIET = -8
        }

        public long play_log(string url, object hwnd, int iMuted, int loglevel)
        {
            if (m_myzjpuller_openstream_log != null)
            {
                m_url = url;
                m_pic = hwnd as PictureBox;
                return m_myzjpuller_openstream_log(out m_hHandle, url, (IntPtr)(((PictureBox)hwnd).Handle), iMuted, loglevel);
            }
            return -1;
        }

        public long play_ex(string sid, int width, int height, string url, object hwnd, int iMuted, PCALLBACK pcb)
        {
            if (m_myzjpuller_openstreamex != null)
            {
                m_sid = sid;
                m_url = url;
                m_pic = hwnd as PictureBox;
                if (m_pic == null)
                {
                    return m_myzjpuller_openstreamex(out m_hHandle, width, height, m_sid, url, (IntPtr.Zero), iMuted, pcb);
                }
                else
                {
                    return m_myzjpuller_openstreamex(out m_hHandle, width, height, m_sid, url, (IntPtr)(((PictureBox)hwnd).Handle), iMuted, pcb);
                }
            }
            return -1;
        }

        public long seek(double seekval)
        {
            if (m_myzjpuller_seek != null)
            {
                return m_myzjpuller_seek(m_hHandle, seekval);
            }
            return -1;
        }

        public long getpos(out double pos)
        {
            pos = 0;
            if (m_myzjpuller_getmasterclock != null)
            {
                return m_myzjpuller_getmasterclock(m_hHandle, out pos);
            }
            return -1;
        }

        public long savepic(string filepath)
        {
            if(m_myzjpuller_savepic != null)
            {
                return m_myzjpuller_savepic(m_hHandle, filepath);
            }
            return -1;
        }

        /// <summary>
        /// 暂停拉流
        /// </summary>
        /// <returns></returns>
        public long pause()
        {
            if (m_myzjpuller_pausestream != null)
            {
                return m_myzjpuller_pausestream(m_hHandle);
            }

            return -1;
        }

        /// <summary>
        /// 启停拉流声音
        /// </summary>
        public void playsound()
        {
            if (m_myzjpuller_playsound != null)
            {
                m_myzjpuller_playsound(m_hHandle);
            }
        }

        public void stopsound()
        {
            if (m_myzjpuller_stopsound != null)
            {
                m_myzjpuller_stopsound(m_hHandle);
            }
        }

        public long mute()
        {
            if (m_myzjpuller_mute != null)
            {
                return m_myzjpuller_mute(m_hHandle);
            }

            return -1;
        }


        /// <summary>
        /// 关闭拉流
        /// </summary>
        public void stop()
        {
            if (m_myzjpuller_closestream != null)
            {
                if (m_hHandle != IntPtr.Zero)
                {
                    m_myzjpuller_closestream(m_hHandle);
                    m_hHandle = IntPtr.Zero;
                }
            }

            
        }

        public void Destory()
        {
            if (hModule != IntPtr.Zero)
            {
                FreeLibrary(hModule); hModule = IntPtr.Zero;
            }
        }
    }
}
