using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using test;

namespace test2
{
    public class zjpush2
    {
#if X64
        const string PUSHERDLLNAME = "libzjpusherex64.dll";
        const string DEVENUMDLLNAME = "zjdevenum64.dll";
#else
        const string PUSHERDLLNAME = ".\\dll\\libzjpusherex2.dll";
        const string DEVENUMDLLNAME = ".\\dll\\zjdevenum.dll";
#endif

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// 基于DSHOW的输入设备枚举接口库
        [DllImport(DEVENUMDLLNAME, EntryPoint = "zjpusher_enumdesktop", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall),]
        public static extern long zjpusher_enumdesktop(/*IN_OUT*/StringBuilder desktopdevs, // 枚举到的桌面参数，可用于UI显示
                                                                                            /*IN*/int desktopdevs_size);         // desktopdevs缓冲区大小


        [DllImport(DEVENUMDLLNAME, EntryPoint = "zjpusher_enumdevices", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall),]
        public static extern long zjpusher_enumdevices(/*IN_OUT*/StringBuilder videodevs, // 枚举到的视频采集设备名
                                                                                          /*IN*/    int videodevs_size,      // videodevs缓冲区大小
                                                                                          /*IN_OUT*/StringBuilder audiodevs, // 枚举到的视频采集设备名
                                                                                          /*IN*/    int audiodevs_size);     // audiodevs缓冲区大小

        [DllImport(DEVENUMDLLNAME, EntryPoint = "zjpusher_enumvideodevices", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall),]
        public static extern long zjpusher_enumvideodevices(/*IN_OUT*/StringBuilder videodevs, // 枚举到的视频采集设备名 
                                                                                               /*IN*/    int videodevs_size);     // videodevs缓冲区大小


        [DllImport(DEVENUMDLLNAME, EntryPoint = "zjpusher_enumaudiodevices", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall),]
        public static extern long zjpusher_enumaudiodevices(/*IN_OUT*/StringBuilder audiodevs, // 枚举到的视频采集设备名
                                                                                               /*IN*/    int audiodevs_size);     // audiodevs缓冲区大小

        [DllImport(DEVENUMDLLNAME, EntryPoint = "zjpusher_getmute", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall),]
        public static extern long zjpusher_getmute(/*OUT*/out bool mutestate);  // 获取当前麦克风静音状态，TRUE静音，FALSE未静音

        [DllImport(DEVENUMDLLNAME, EntryPoint = "zjpusher_setmute", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall),]
        public static extern long zjpusher_setmute(/*IN*/bool mute);    //TRUE禁止麦克风采集声音，FALSE恢复麦克风采集声音

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// 基于FFMPEG4.0 WIN32推流接口库

        [DllImport(PUSHERDLLNAME, EntryPoint = "zjpusher_openstream_app_ex", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall),]
        public static extern long zjpusher_openstream_app_ex(string AppWinTitle,   // 窗口标题
                                                           int OutVideoSizeWidth, int OutVideoSizeHeight,  // 推流后桌面宽高
                                                           string szAudio,    // 音频采集设备
                                                           int iframerate,    // 视频帧率
                                                           int VideoBitrate/*BYTES*/,     // 视频码率
                                                           int AudioBitrate/*BYTES*/,     // 音频码率
                                                           string url,                    // 推流地址
                                                           IntPtr hWnd,               // 预览句柄
                                                           bool bOnlyPreview);   // 仅预览不推流

        [DllImport(PUSHERDLLNAME, EntryPoint = "zjpusher_openstream_combine_ex", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall),]
        public static extern long zjpusher_openstream_combine_ex(string szCam1Dev,  // 主视频摄像头设备名
                                                             int SrcMonitorX, int SrcMonitorY, int SrcMonitorWidth, int SrcMonitorHeight,
                                                             int OutMonitorWidth, int OutMonitorHeight/*"1024x768"*/,    // 推流后桌面宽高
                                                             string szVideo,   // 视频采集设备
                                                             int OutSrcVideoX, int OutSrcVideoY,
                                                             int OutVideoSizeWidth, int OutVideoSizeHeight/*"256x144"*/, // 推流后视频宽高
                                                             string szAudio,   // 音频采集设备
                                                             int iframerate,   // 视频帧率
                                                             int VideoBitrate/*BYTES*/,   // 视频码率
                                                             int AudioBitrate/*BYTES*/,   // 音频码率
                                                             string url,                  // 推流地址
                                                             IntPtr hWnd,               // 预览句柄
                                                             bool bOnlyPreview);   // 仅预览不推流

        [DllImport(PUSHERDLLNAME, EntryPoint = "zjpusher_callback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall),]
        public static extern long zjpusher_callback(PUSHCALLBACK pcb);

        [DllImport(PUSHERDLLNAME, EntryPoint = "zjpusher_savepic", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall),]
        public static extern long zjpusher_savepic(string filepath);

        /*开始推流*/
        [DllImport(PUSHERDLLNAME, EntryPoint = "zjpusher_openstream_dca_ex", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall),]
        public static extern long zjpusher_openstream_dca_ex(int iMonitorNum,  // 桌面设备索引
                                                              int SrcMonitorX, int SrcMonitorY, int SrcMonitorWidth, int SrcMonitorHeight,
                                                              int OutMonitorWidth, int OutMonitorHeight/*"1024x768"*/,    // 推流后桌面宽高
                                                              string szVideo,   // 视频采集设备
                                                              int OutSrcVideoX, int OutSrcVideoY,
                                                              int OutVideoSizeWidth, int OutVideoSizeHeight/*"256x144"*/, // 推流后视频宽高
                                                              string szAudio,   // 音频采集设备
                                                              int iframerate,   // 视频帧率
                                                              int VideoBitrate/*BYTES*/,   // 视频码率
                                                              int AudioBitrate/*BYTES*/,   // 音频码率
                                                              string url,                  // 推流地址
                                                              IntPtr hWnd,               // 预览句柄
                                                              bool bOnlyPreview,   // 仅预览不推流
                                                              int quality);

        [DllImport(PUSHERDLLNAME, EntryPoint = "zjpusher_openstream_dcam_ex", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall),]
        public static extern long zjpusher_openstream_dcam_ex(int iMonitorNum,  // 桌面设备索引
                                                              int SrcMonitorX, int SrcMonitorY, int SrcMonitorWidth, int SrcMonitorHeight,
                                                              int OutMonitorWidth, int OutMonitorHeight/*"1024x768"*/,    // 推流后桌面宽高
                                                              string szVideo,   // 视频采集设备
                                                              int OutSrcVideoX, int OutSrcVideoY,
                                                              int OutVideoSizeWidth, int OutVideoSizeHeight/*"256x144"*/, // 推流后视频宽高
                                                              string szAudio,   // 音频采集设备
                                                              int iframerate,   // 视频帧率
                                                              int VideoBitrate/*BYTES*/,   // 视频码率
                                                              int AudioBitrate/*BYTES*/,   // 音频码率
                                                              string url,                  // 推流地址
                                                              IntPtr hWnd,               // 预览句柄
                                                              bool bOnlyPreview,    // 仅预览不推流
                                                              bool bCaptureMouse,  // 是否捕获鼠标，TRUE捕获，FALSE不捕获
                                                              int quality);

        [DllImport(PUSHERDLLNAME, EntryPoint = "zjpusher_openstream_da_ex", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall),]
        public static extern long zjpusher_openstream_da_ex(int iMonitorNum,   // 桌面设备索引
                                                            int SrcMonitorX, int SrcMonitorY, int SrcMonitorWidth, int SrcMonitorHeight, // 桌面采集区域
                                                            int OutVideoSizeWidth, int OutVideoSizeHeight,  // 推流后桌面宽高
                                                            string szAudio,    // 音频采集设备
                                                            int iframerate,    // 视频帧率
                                                            int VideoBitrate/*BYTES*/,     // 视频码率
                                                            int AudioBitrate/*BYTES*/,     // 音频码率
                                                            string url,                    // 推流地址
                                                            IntPtr hWnd,               // 预览句柄
                                                            bool bOnlyPreview,   // 仅预览不推流
                                                            int quality);

        [DllImport(PUSHERDLLNAME, EntryPoint = "zjpusher_openstream_dam_ex", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall),]
        public static extern long zjpusher_openstream_dam_ex(int iMonitorNum,   // 桌面设备索引
                                                             int SrcMonitorX, int SrcMonitorY, int SrcMonitorWidth, int SrcMonitorHeight, // 桌面采集区域
                                                             int OutVideoSizeWidth, int OutVideoSizeHeight,  // 推流后桌面宽高
                                                             string szAudio,    // 音频采集设备
                                                             int iframerate,    // 视频帧率
                                                             int VideoBitrate/*BYTES*/,     // 视频码率
                                                             int AudioBitrate/*BYTES*/,     // 音频码率
                                                             string url,                    // 推流地址
                                                             IntPtr hWnd,               // 预览句柄
                                                             bool bOnlyPreview,   // 仅预览不推流
                                                             bool bCaptureMouse,   // 是否捕获鼠标，TRUE捕获，FALSE不捕获
                                                             int quality);

        /*开始推流*/
        [DllImport(PUSHERDLLNAME, EntryPoint = "zjpusher_openstream_dca", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall),]
        public static extern long zjpusher_openstream_dca(int iMonitorNum,  // 桌面设备索引
                                                          int MonitorWidth, int MonitorHeight/*"1024x768"*/,    // 推流后桌面宽高
                                                          string szVideo,   // 视频采集设备
                                                          int VideoSizeWidth, int VideoSizeHeight/*"256x144"*/, // 推流后视频宽高
                                                          string szAudio,   // 音频采集设备
                                                          int iframerate,   // 视频帧率
                                                          int VideoBitrate/*BYTES*/,   // 视频码率
                                                          int AudioBitrate/*BYTES*/,   // 音频码率
                                                          string url,                  // 推流地址
                                                          IntPtr hWnd,                // 预览句柄
                                                          int quality);

        [DllImport(PUSHERDLLNAME, EntryPoint = "zjpusher_openstream_dcam", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall),]
        public static extern long zjpusher_openstream_dcam(int iMonitorNum,  // 桌面设备索引
                                                          int SrcMonitorX, int SrcMonitorY, int SrcMonitorWidth, int SrcMonitorHeight,
                                                          int MonitorWidth, int MonitorHeight/*"1024x768"*/,    // 推流后桌面宽高
                                                          string szVideo,   // 视频采集设备
                                                          int VideoSizeWidth, int VideoSizeHeight/*"256x144"*/, // 推流后视频宽高
                                                          string szAudio,   // 音频采集设备
                                                          int iframerate,   // 视频帧率
                                                          int VideoBitrate/*BYTES*/,   // 视频码率
                                                          int AudioBitrate/*BYTES*/,   // 音频码率
                                                          string url,                  // 推流地址
                                                          IntPtr hWnd,               // 预览句柄
                                                          bool bCaptureMouse,   // 是否捕获鼠标，TRUE捕获，FALSE不捕获
                                                          int quality);


        [DllImport(PUSHERDLLNAME, EntryPoint = "zjpusher_openstream_dc", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall),]
        public static extern long zjpusher_openstream_dc(int iMonitorNum,  // 桌面设备索引
                                                          int MonitorWidth, int MonitorHeight/*"1024x768"*/,    // 推流后桌面宽高
                                                          string szVideo,   // 视频采集设备
                                                          int VideoSizeWidth, int VideoSizeHeight/*"256x144"*/, // 推流后视频宽高
                                                          int iframerate,   // 视频帧率
                                                          int VideoBitrate/*BYTES*/,   // 视频码率
                                                          string url,                  // 推流地址
                                                          IntPtr hWnd,                // 预览句柄
                                                          int quality);

        [DllImport(PUSHERDLLNAME, EntryPoint = "zjpusher_openstream_dcm", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall),]
        public static extern long zjpusher_openstream_dcm(int iMonitorNum,  // 桌面设备索引
                                                          int SrcMonitorX, int SrcMonitorY, int SrcMonitorWidth, int SrcMonitorHeight,
                                                          int MonitorWidth, int MonitorHeight/*"1024x768"*/,    // 推流后桌面宽高
                                                          string szVideo,   // 视频采集设备
                                                          int VideoSizeWidth, int VideoSizeHeight/*"256x144"*/, // 推流后视频宽高
                                                          int iframerate,   // 视频帧率
                                                          int VideoBitrate/*BYTES*/,   // 视频码率
                                                          string url,                  // 推流地址
                                                          IntPtr hWnd,                // 预览句柄
                                                          bool bCaptureMouse,   // 是否捕获鼠标，TRUE捕获，FALSE不捕获
                                                          int quality);

        [DllImport(PUSHERDLLNAME, EntryPoint = "zjpusher_openstream_ca", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall),]
        public static extern long zjpusher_openstream_ca(string szVideo, // 视频采集设备
                                                         int VideoSizeWidth, int VideoSizeHeight/*"256x144"*/, // 推流后视频宽高
                                                         string szAudio, // 音频采集设备
                                                         int iframerate, // 视频帧率
                                                         int VideoBitrate/*BYTES*/, // 视频码率
                                                         int AudioBitrate/*BYTES*/, // 音频码率
                                                         string url,                // 推流地址
                                                         IntPtr hWnd,              // 预览句柄
                                                         int quality);

        [DllImport(PUSHERDLLNAME, EntryPoint = "zjpusher_openstream_c", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall),]
        public static extern long zjpusher_openstream_c(string szVideo, // 视频采集设备
                                                         int VideoSizeWidth, int VideoSizeHeight/*"256x144"*/, // 推流后视频宽高
                                                         int iframerate, // 视频帧率
                                                         int VideoBitrate/*BYTES*/, // 视频码率
                                                         string url,                // 推流地址
                                                         IntPtr hWnd,              // 预览句柄
                                                         bool bOnlyPreview,        // 仅预览不推流
                                                         int quality);


        [DllImport(PUSHERDLLNAME, EntryPoint = "zjpusher_openstream_multiwnd_c", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall),]
        public static extern long zjpusher_openstream_multiwnd_c(string szVideo, // 视频采集设备
                                                                 int VideoSizeWidth, int VideoSizeHeight/*"256x144"*/, // 推流后视频宽高
                                                                 int iframerate, // 视频帧率
                                                                 int VideoBitrate/*BYTES*/, // 视频码率
                                                                 string url,                // 推流地址
                                                                 IntPtr hWnd,              // 预览句柄
                                                                 IntPtr hWnd1,              // 预览句柄
                                                                 IntPtr hWnd2,              // 预览句柄
                                                                 IntPtr hWnd3,              // 预览句柄
                                                                 bool bOnlyPreview,        // 仅预览不推流
                                                                 int quality);

        [StructLayout(LayoutKind.Sequential, CharSet =CharSet.Ansi, Pack = 1)]
        public struct DRAWTEXT_ST
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szText;
            [MarshalAs(UnmanagedType.I4, SizeConst = 4)]
            public int x;
            [MarshalAs(UnmanagedType.I4, SizeConst = 4)]
            public int y;
            [MarshalAs(UnmanagedType.I4, SizeConst = 4)]
            public int fontsize;
        };

        [DllImport(PUSHERDLLNAME, EntryPoint = "zjpusher_openstream_ca_ex", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall),]
        public static extern long zjpusher_openstream_ca_ex(string szVideo, // 视频采集设备
                                                        int VideoSizeWidth, int VideoSizeHeight/*"256x144"*/, // 推流后视频宽高
                                                        IntPtr stDrawText,
                                                        string szAudio, // 音频采集设备
                                                        int iframerate, // 视频帧率
                                                        int VideoBitrate/*BYTES*/, // 视频码率
                                                        int AudioBitrate/*BYTES*/, // 音频码率
                                                        string url,                // 推流地址
                                                        IntPtr hWnd,              // 预览句柄
                                                        bool bOnlyPreview,        // 仅预览不推流
                                                        int quality);


        [DllImport(PUSHERDLLNAME, EntryPoint = "zjpusher_openstream_da", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall),]
        public static extern long zjpusher_openstream_da(int iMonitorNum,   // 桌面设备索引
                                                         int VideoSizeWidth, int VideoSizeHeight/*"256x144"*/,  // 推流后桌面宽高
                                                         string szAudio,    // 音频采集设备
                                                         int iframerate,    // 视频帧率
                                                         int VideoBitrate/*BYTES*/,     // 视频码率
                                                         int AudioBitrate/*BYTES*/,     // 音频码率
                                                         string url,                    // 推流地址
                                                         IntPtr hWnd,                  // 预览句柄
                                                         int quality);

        [DllImport(PUSHERDLLNAME, EntryPoint = "zjpusher_openstream_dam", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall),]
        public static extern long zjpusher_openstream_dam(int iMonitorNum,   // 桌面设备索引
                                                         int SrcMonitorX, int SrcMonitorY, int SrcMonitorWidth, int SrcMonitorHeight,
                                                         int VideoSizeWidth, int VideoSizeHeight/*"256x144"*/,  // 推流后桌面宽高
                                                         string szAudio,    // 音频采集设备
                                                         int iframerate,    // 视频帧率
                                                         int VideoBitrate/*BYTES*/,     // 视频码率
                                                         int AudioBitrate/*BYTES*/,     // 音频码率
                                                         string url,                    // 推流地址
                                                         IntPtr hWnd,                  // 预览句柄
                                                         bool bCaptureMouse,   // 是否捕获鼠标，TRUE捕获，FALSE不捕获
                                                         int quality);

        [DllImport(PUSHERDLLNAME, EntryPoint = "zjpusher_openstream_d", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall),]
        public static extern long zjpusher_openstream_d(int iMonitorNum,   // 桌面设备索引
                                                         int VideoSizeWidth, int VideoSizeHeight/*"256x144"*/,  // 推流后桌面宽高
                                                         int iframerate,    // 视频帧率
                                                         int VideoBitrate/*BYTES*/,     // 视频码率
                                                         string url,                    // 推流地址
                                                         IntPtr hWnd,                  // 预览句柄
                                                         int quality);

        [DllImport(PUSHERDLLNAME, EntryPoint = "zjpusher_openstream_dm", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall),]
        public static extern long zjpusher_openstream_dm(int iMonitorNum,   // 桌面设备索引
                                                        int SrcMonitorX, int SrcMonitorY, int SrcMonitorWidth, int SrcMonitorHeight,
                                                        int VideoSizeWidth, int VideoSizeHeight/*"256x144"*/,  // 推流后桌面宽高
                                                        int iframerate,    // 视频帧率
                                                        int VideoBitrate/*BYTES*/,     // 视频码率
                                                        string url,                    // 推流地址
                                                        IntPtr hWnd,                  // 预览句柄
                                                        bool bCaptureMouse,   // 是否捕获鼠标，TRUE捕获，FALSE不捕获
                                                        int quality);

        [DllImport(PUSHERDLLNAME, EntryPoint = "zjpusher_openstream_dmw_ex", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall),]
        public static extern long zjpusher_openstream_dmw_ex(int iMonitorNum,   // 桌面设备索引
                                                        int SrcMonitorX, int SrcMonitorY, int SrcMonitorWidth, int SrcMonitorHeight,
                                                        int VideoSizeWidth, int VideoSizeHeight/*"256x144"*/,  // 推流后桌面宽高
                                                        int iframerate,    // 视频帧率
                                                        int VideoBitrate/*BYTES*/,     // 视频码率
                                                        string url,                    // 推流地址
                                                        IntPtr hWnd,                  // 预览句柄
                                                        bool bCaptureMouse,   // 是否捕获鼠标，TRUE捕获，FALSE不捕获
                                                        bool bOnlyPreview,
                                                        string title,
                                                        int quality);

        [DllImport(PUSHERDLLNAME, EntryPoint = "zjpusher_openstream_a", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall),]
        public static extern long zjpusher_openstream_a(string szAudio,             // 音频采集设备
                                                        int AudioBitrate/*BYTES*/,  // 音频码率
                                                        string url,                 // 推流地址
                                                        IntPtr hWnd,                 // 预览句柄
                                                        int quality);
                                                        
        [DllImport(PUSHERDLLNAME, EntryPoint = "zjpusher_openstream_a_rec", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall),]
        // 指定麦克风设备，录制AAC格式音频文件到指定路径
        // szAudio 麦克风设备名称
        // 录制后的文件路径.aac文件
        public static extern long zjpusher_openstream_a_rec(string szAudio, string filepath);

        [DllImport(PUSHERDLLNAME, EntryPoint = "zjpusher_openstream_a_rec_showwaves", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall),]
        // 将录制的AAC文件转换成声波视频文件
        // inputfile 待转换的AAC音频文件
        // filepath 转换后的h264格式声波图形视频文件
        public static extern long zjpusher_openstream_a_rec_showwaves(string inputfile, string filepath);

        [DllImport(PUSHERDLLNAME, EntryPoint = "zjpusher_closestream", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall),]
        public static extern long zjpusher_closestream();   // 关闭当前推流

        [DllImport(PUSHERDLLNAME, EntryPoint = "zjpusher_init", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall),]
        public static extern long zjpusher_init(int loglevel); // 推流前务必调用

        [DllImport(PUSHERDLLNAME, EntryPoint = "zjpusher_init2", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall),]
        public static extern long zjpusher_init2(int loglevel, string logpath, string foldername); // 推流前务必调用

        [DllImport(PUSHERDLLNAME, EntryPoint = "zjpusher_uninit", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall),]
        public static extern long zjpusher_uninit();    // 应用退出前务必调用

        [DllImport(PUSHERDLLNAME, EntryPoint = "zjpusher_beautylevel", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall),]
        public static extern long zjpusher_beautylevel(int ilevel);    // ilevel 美颜效果，0:关闭美颜, 1:1级美颜， 2:2级美颜


        public bool m_bPusher;           // 是否正在推流
        public string m_sid;
        public string m_url;            // 推流地址
        public int m_desktopdevnum;     // 桌面设备索引
        public Size m_desktopsize;      // 桌面推流宽高
        public string m_camdev;         // 摄像头设备名称
        public Size m_camsize;          // 摄像头推流宽高
        public string m_audiodev;       // 麦克风设备名称
        public int m_framerate;         // 视频帧率
        public int m_videorate;         // 视频码率
        public int m_audiorate;         // 音频码率
        public int m_quality;

        public PUSHCALLBACK m_pcb;
        public PictureBox m_pic;         // 保存用于跨线程操作时访问的控件对象

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

        public long pushercallbackfun(int imsg, string msg)
        {
            if (imsg != 0)
            {
                Debug.WriteLine(imsg.ToString() + " " + msg);
                MessageBox.Show(imsg.ToString("x8") + "\n" + msg, imsg.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            return 0L;
        }

        public long init(int loglevel)
        {
            m_pcb = new PUSHCALLBACK(pushercallbackfun);
            zjpusher_callback(m_pcb);
            /*
             * 日志级别 AV_LOG_TRACE = 56 
		               AV_LOG_DEBUG = 48 
		               AV_LOG_WARNING = 24 
		               AV_LOG_ERROR = 16 
		               AV_LOG_FATAL = 8
		               AV_LOG_PANIC = 0
		               AV_LOG_QUIET = -8
           //*/
            //return zjpusher_init(loglevel);
            return zjpusher_init2(loglevel, "zjpusher.log", "dll");
        }

        public long uninit()
        {
            return zjpusher_uninit();
        }

        public long push_desktop_cam_mic(IntPtr hWnd)
        {
            long ret = zjpusher_openstream_dca(m_desktopdevnum, m_desktopsize.Width, m_desktopsize.Height, m_camdev, m_camsize.Width, m_camsize.Height, m_audiodev, m_framerate, m_videorate, m_audiorate, m_url, hWnd, m_quality);
            return ret;
        }
        public long push_desktop_cam(IntPtr hWnd)
        {
            long ret = zjpusher_openstream_dc(m_desktopdevnum, m_desktopsize.Width, m_desktopsize.Height, m_camdev, m_camsize.Width, m_camsize.Height, m_framerate, m_videorate, m_url, hWnd, m_quality);
            return ret;
        }

        public long push_cam_mic(IntPtr hWnd)
        {
            long ret = zjpusher_openstream_ca(m_camdev, m_camsize.Width, m_camsize.Height, m_audiodev, m_framerate, m_videorate, m_audiorate, m_url, hWnd, m_quality);
            return ret;
        }

        public long push_cam(IntPtr hWnd)
        {
            long ret = zjpusher_openstream_c(m_camdev, m_camsize.Width, m_camsize.Height, m_framerate, m_videorate, m_url, hWnd, false, m_quality);
            return ret;
        }

        public long push_desktop_mic(IntPtr hWnd)
        {

            long ret = zjpusher_openstream_da(m_desktopdevnum, m_desktopsize.Width, m_desktopsize.Height, m_audiodev, m_framerate, m_videorate, m_audiorate, m_url, hWnd, m_quality);
            return ret;
        }

        public long push_desktop(IntPtr hWnd)
        {

            long ret = zjpusher_openstream_d(m_desktopdevnum, m_desktopsize.Width, m_desktopsize.Height, m_framerate, m_videorate, m_url, hWnd, m_quality);
            return ret;
        }

        public long push_mic(IntPtr hWnd)
        {
            long ret = zjpusher_openstream_a(m_audiodev, m_audiorate, m_url, hWnd, m_quality);
            return ret;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// 新增接口，之前接口尽量不要使用
        public long push_desktop_camera_audio_mouse_ex(int iMonitorNum,  // 桌面设备索引
                                                        int SrcMonitorX, int SrcMonitorY, int SrcMonitorWidth, int SrcMonitorHeight,
                                                        int OutMonitorWidth, int OutMonitorHeight/*"1024x768"*/,    // 推流后桌面宽高
                                                        string szVideo,   // 视频采集设备
                                                        int OutSrcVideoX, int OutSrcVideoY,
                                                        int OutVideoSizeWidth, int OutVideoSizeHeight/*"256x144"*/, // 推流后视频宽高
                                                        string szAudio,   // 音频采集设备
                                                        int iframerate,   // 视频帧率
                                                        int VideoBitrate/*BYTES*/,   // 视频码率
                                                        int AudioBitrate/*BYTES*/,   // 音频码率
                                                        string url,                  // 推流地址
                                                        IntPtr hWnd,               // 预览句柄
                                                        bool bOnlyPreview,    // 仅预览不推流
                                                        bool bCaptureMouse,
                                                        int iQuality)
        {
            long ret = zjpusher_openstream_dcam_ex(iMonitorNum, SrcMonitorX, SrcMonitorY, SrcMonitorWidth, SrcMonitorHeight,
                                                  OutMonitorWidth, OutMonitorHeight,
                                                  szVideo, OutSrcVideoX, OutSrcVideoY, OutVideoSizeWidth, OutVideoSizeHeight,
                                                  szAudio, iframerate, VideoBitrate, AudioBitrate,
                                                  url, hWnd, bOnlyPreview, bCaptureMouse, iQuality);
            return ret;
        }

        public long push_desktop_audio_mouse_ex(int iMonitorNum,   // 桌面设备索引
                                                             int SrcMonitorX, int SrcMonitorY, int SrcMonitorWidth, int SrcMonitorHeight, // 桌面采集区域
                                                             int OutVideoSizeWidth, int OutVideoSizeHeight,  // 推流后桌面宽高
                                                             string szAudio,    // 音频采集设备
                                                             int iframerate,    // 视频帧率
                                                             int VideoBitrate/*BYTES*/,     // 视频码率
                                                             int AudioBitrate/*BYTES*/,     // 音频码率
                                                             string url,                    // 推流地址
                                                             IntPtr hWnd,               // 预览句柄
                                                             bool bOnlyPreview,   // 仅预览不推流
                                                             bool bCaptureMouse,    // 是否捕获鼠标，TRUE捕获，FALSE不捕获
                                                             int iQuality)
        {
            long ret = zjpusher_openstream_dam_ex(iMonitorNum,   // 桌面设备索引
                                                    SrcMonitorX, SrcMonitorY, SrcMonitorWidth, SrcMonitorHeight, // 桌面采集区域
                                                    OutVideoSizeWidth, OutVideoSizeHeight,  // 推流后桌面宽高
                                                    szAudio,    // 音频采集设备
                                                    iframerate,    // 视频帧率
                                                    VideoBitrate/*BYTES*/,     // 视频码率
                                                    AudioBitrate/*BYTES*/,     // 音频码率
                                                    url,                    // 推流地址
                                                    hWnd,               // 预览句柄
                                                    bOnlyPreview,   // 仅预览不推流
                                                    bCaptureMouse,   // 是否捕获鼠标，TRUE捕获，FALSE不捕获
                                                    iQuality);
            return ret;
        }

        public long push_desktop_camera_audio_mouse(int iMonitorNum,  // 桌面设备索引
                                                          int SrcMonitorX, int SrcMonitorY, int SrcMonitorWidth, int SrcMonitorHeight,
                                                          int MonitorWidth, int MonitorHeight/*"1024x768"*/,    // 推流后桌面宽高
                                                          string szVideo,   // 视频采集设备
                                                          int VideoSizeWidth, int VideoSizeHeight/*"256x144"*/, // 推流后视频宽高
                                                          string szAudio,   // 音频采集设备
                                                          int iframerate,   // 视频帧率
                                                          int VideoBitrate/*BYTES*/,   // 视频码率
                                                          int AudioBitrate/*BYTES*/,   // 音频码率
                                                          string url,                  // 推流地址
                                                          IntPtr hWnd,               // 预览句柄
                                                          bool bCaptureMouse,
                                                          int iQuality)
        {
            long ret = zjpusher_openstream_dcam(iMonitorNum,  // 桌面设备索引
                                                SrcMonitorX, SrcMonitorY, SrcMonitorWidth, SrcMonitorHeight,
                                                MonitorWidth, MonitorHeight/*"1024x768"*/,    // 推流后桌面宽高
                                                szVideo,   // 视频采集设备
                                                VideoSizeWidth, VideoSizeHeight/*"256x144"*/, // 推流后视频宽高
                                                szAudio,   // 音频采集设备
                                                iframerate,   // 视频帧率
                                                VideoBitrate/*BYTES*/,   // 视频码率
                                                AudioBitrate/*BYTES*/,   // 音频码率
                                                url,                  // 推流地址
                                                hWnd,               // 预览句柄
                                                bCaptureMouse,
                                                iQuality);
            return ret;
        }

        public long push_desktop_camera_mouse(int iMonitorNum,  // 桌面设备索引
                                                          int SrcMonitorX, int SrcMonitorY, int SrcMonitorWidth, int SrcMonitorHeight,
                                                          int MonitorWidth, int MonitorHeight/*"1024x768"*/,    // 推流后桌面宽高
                                                          string szVideo,   // 视频采集设备
                                                          int VideoSizeWidth, int VideoSizeHeight/*"256x144"*/, // 推流后视频宽高
                                                          int iframerate,   // 视频帧率
                                                          int VideoBitrate/*BYTES*/,   // 视频码率
                                                          string url,                  // 推流地址
                                                          IntPtr hWnd,                // 预览句柄
                                                          bool bCaptureMouse,
                                                          int iQuality)
        {
            long ret = zjpusher_openstream_dcm(iMonitorNum,  // 桌面设备索引
                                                          SrcMonitorX, SrcMonitorY, SrcMonitorWidth, SrcMonitorHeight,
                                                          MonitorWidth, MonitorHeight/*"1024x768"*/,    // 推流后桌面宽高
                                                          szVideo,   // 视频采集设备
                                                          VideoSizeWidth, VideoSizeHeight/*"256x144"*/, // 推流后视频宽高
                                                          iframerate,   // 视频帧率
                                                          VideoBitrate/*BYTES*/,   // 视频码率
                                                          url,                  // 推流地址
                                                          hWnd,                // 预览句柄
                                                          bCaptureMouse,
                                                          iQuality);
            return ret;
        }

        public long push_desktop_audio_mouse(int iMonitorNum,   // 桌面设备索引
                                                         int SrcMonitorX, int SrcMonitorY, int SrcMonitorWidth, int SrcMonitorHeight,
                                                         int VideoSizeWidth, int VideoSizeHeight/*"256x144"*/,  // 推流后桌面宽高
                                                         string szAudio,    // 音频采集设备
                                                         int iframerate,    // 视频帧率
                                                         int VideoBitrate/*BYTES*/,     // 视频码率
                                                         int AudioBitrate/*BYTES*/,     // 音频码率
                                                         string url,                    // 推流地址
                                                         IntPtr hWnd,                  // 预览句柄
                                                         bool bCaptureMouse,
                                                         int iQuality)
        {
            long ret = zjpusher_openstream_dam(iMonitorNum,   // 桌面设备索引
                                                SrcMonitorX, SrcMonitorY, SrcMonitorWidth, SrcMonitorHeight,
                                                VideoSizeWidth, VideoSizeHeight/*"256x144"*/,  // 推流后桌面宽高
                                                szAudio,    // 音频采集设备
                                                iframerate,    // 视频帧率
                                                VideoBitrate/*BYTES*/,     // 视频码率
                                                AudioBitrate/*BYTES*/,     // 音频码率
                                                url,                    // 推流地址
                                                hWnd,                  // 预览句柄
                                                bCaptureMouse,   // 是否捕获鼠标，TRUE捕获，FALSE不捕获
                                                iQuality);
            return ret;
        }

        public long push_desktop_mouse(int iMonitorNum,   // 桌面设备索引
                                                        int SrcMonitorX, int SrcMonitorY, int SrcMonitorWidth, int SrcMonitorHeight,
                                                        int VideoSizeWidth, int VideoSizeHeight/*"256x144"*/,  // 推流后桌面宽高
                                                        int iframerate,    // 视频帧率
                                                        int VideoBitrate/*BYTES*/,     // 视频码率
                                                        string url,                    // 推流地址
                                                        IntPtr hWnd,                  // 预览句柄
                                                        bool bCaptureMouse,
                                                        int iQuality)
        {
            long ret = zjpusher_openstream_dm(iMonitorNum,   // 桌面设备索引
                                                        SrcMonitorX, SrcMonitorY, SrcMonitorWidth, SrcMonitorHeight,
                                                        VideoSizeWidth, VideoSizeHeight/*"256x144"*/,  // 推流后桌面宽高
                                                        iframerate,    // 视频帧率
                                                        VideoBitrate/*BYTES*/,     // 视频码率
                                                        url,                    // 推流地址
                                                        hWnd,                  // 预览句柄
                                                        bCaptureMouse,   // 是否捕获鼠标，TRUE捕获，FALSE不捕获
                                                        iQuality);
            return ret;
        }


        public long push_desktop_mouse_wndtitle(int iMonitorNum,   // 桌面设备索引
                                                        int SrcMonitorX, int SrcMonitorY, int SrcMonitorWidth, int SrcMonitorHeight,
                                                        int VideoSizeWidth, int VideoSizeHeight/*"256x144"*/,  // 推流后桌面宽高
                                                        int iframerate,    // 视频帧率
                                                        int VideoBitrate/*BYTES*/,     // 视频码率
                                                        string url,                    // 推流地址
                                                        IntPtr hWnd,                  // 预览句柄
                                                        bool bCaptureMouse,
                                                         bool bOnlyPreview,
                                                        string title,
                                                        int iQuality)
        {
            long ret = zjpusher_openstream_dmw_ex(iMonitorNum,   // 桌面设备索引
                                                        SrcMonitorX, SrcMonitorY, SrcMonitorWidth, SrcMonitorHeight,
                                                        VideoSizeWidth, VideoSizeHeight/*"256x144"*/,  // 推流后桌面宽高
                                                        iframerate,    // 视频帧率
                                                        VideoBitrate/*BYTES*/,     // 视频码率
                                                        url,                    // 推流地址
                                                        hWnd,                  // 预览句柄
                                                        bCaptureMouse,// 是否捕获鼠标，TRUE捕获，FALSE不捕获
                                                        bOnlyPreview,
                                                        title,
                                                        iQuality);   
            return ret;
        }

        public long push_camera_audio(string szVideo, // 视频采集设备
                                                         int VideoSizeWidth, int VideoSizeHeight/*"256x144"*/, // 推流后视频宽高
                                                         string szAudio, // 音频采集设备
                                                         int iframerate, // 视频帧率
                                                         int VideoBitrate/*BYTES*/, // 视频码率
                                                         int AudioBitrate/*BYTES*/, // 音频码率
                                                         string url,                // 推流地址
                                                         IntPtr hWnd,
                                                         int iQuality)
        {
            long ret = zjpusher_openstream_ca(szVideo, // 视频采集设备
                                                VideoSizeWidth, VideoSizeHeight/*"256x144"*/, // 推流后视频宽高
                                                szAudio, // 音频采集设备
                                                iframerate, // 视频帧率
                                                VideoBitrate/*BYTES*/, // 视频码率
                                                AudioBitrate/*BYTES*/, // 音频码率
                                                url,                // 推流地址
                                                hWnd,              // 预览句柄
                                                iQuality);
            return ret;
        }

        public long close()
        {
            m_bPusher = false;
            return zjpusher_closestream();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // 推流 桌面+摄像头+麦克风
        public PushCompleteEventHandle my_PushCompleteEventHandle = null;
        private void applyComplete()
        {
            if (this.m_pic.InvokeRequired)
            {
                this.m_pic.Invoke(new PushCompleteEventHandle(applyComplete1));
            }
            else
            {
                applyComplete1();
            }
        }

        private void applyComplete1()
        {
            //long lret = this.push_desktop_camera_audio_mouse(0, 2560, 0, 2160, 1440, 1024, 768, "HD Camera", 520, 244, "麦克风阵列 (Realtek(R) Audio)", 20, 1250000, 960000, m_url, this.m_pic.Handle, true);
            long lret = this.push_desktop_cam_mic(this.m_pic.Handle);
        }

        private void WorkThread(object param)
        {
            if (my_PushCompleteEventHandle != null)
                my_PushCompleteEventHandle();
        }

        /// <summary>
        /// 推流 桌面+摄像头+麦克风
        /// </summary>
        /// <param name="monitornum">桌面索引号，由接口zjpusher_enumdesktop返回</param>
        /// <param name="destktopsize">推流桌面的宽高</param>
        /// <param name="videodev">视频采集设备名称，由接口zjpusher_enumdevices返回</param>
        /// <param name="camsize">推流视频的宽高</param>
        /// <param name="audiodev">音频采集设备名称，由接口zjpusher_enumdevices返回</param>
        /// <param name="framerate">视频帧率</param>
        /// <param name="videorate">视频码率</param>
        /// <param name="audiorate">音频码率</param>
        /// <param name="url">推流地址</param>
        /// <param name="obj">预览PictureBox对象</param>
        public void pushbythread(int monitornum, Size destktopsize, string videodev, Size camsize, string audiodev, int framerate, int videorate, int audiorate, string url, object obj)
        {
            my_PushCompleteEventHandle = new PushCompleteEventHandle(applyComplete);
            Thread t = new Thread(new ParameterizedThreadStart(WorkThread));
            t.IsBackground = true;

            this.m_desktopdevnum = monitornum;
            this.m_desktopsize = destktopsize;

            this.m_camdev = videodev;
            this.m_camsize = camsize;

            this.m_audiodev = audiodev;

            this.m_framerate = framerate;
            this.m_videorate = videorate;
            this.m_audiorate = audiorate;

            this.m_url = url;
            this.m_pic = obj as PictureBox;

            t.Start();
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // 推流 桌面+摄像头
        public PushCompleteEventHandle my_PushCompleteEventHandle_DC = null;
        private void applyComplete_DC()
        {
            if (this.m_pic.InvokeRequired)
            {
                this.m_pic.Invoke(new PushCompleteEventHandle(applyComplete1_DC));
            }
            else
            {
                applyComplete1_DC();
            }
        }

        private void applyComplete1_DC()
        {
            long lret = this.push_desktop_cam(this.m_pic.Handle);
        }

        private void WorkThread_DC(object param)
        {
            if (my_PushCompleteEventHandle_DC != null)
                my_PushCompleteEventHandle_DC();
        }

        /// <summary>
        /// 推流 桌面+摄像头
        /// </summary>
        /// <param name="monitornum">桌面索引号，由接口zjpusher_enumdesktop返回</param>
        /// <param name="destktopsize">推流桌面的宽高</param>
        /// <param name="videodev">视频采集设备名称，由接口zjpusher_enumdevices返回</param>
        /// <param name="camsize">推流视频的宽高</param>
        /// <param name="framerate">视频帧率</param>
        /// <param name="videorate">视频码率</param>
        /// <param name="url">推流地址</param>
        /// <param name="obj">预览PictureBox对象</param>
        public void pushbythread_DC(int monitornum, Size destktopsize, string videodev, Size camsize, int framerate, int videorate, string url, object obj)
        {
            my_PushCompleteEventHandle_DC = new PushCompleteEventHandle(applyComplete_DC);
            Thread t = new Thread(new ParameterizedThreadStart(WorkThread_DC));
            t.IsBackground = true;
            this.m_desktopdevnum = monitornum;
            this.m_desktopsize = destktopsize;

            this.m_camdev = videodev;
            this.m_camsize = camsize;


            this.m_framerate = framerate;
            this.m_videorate = videorate;

            this.m_url = url;
            this.m_pic = obj as PictureBox;

            t.Start();
        }





        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // 推流 摄像头+麦克风
        public PushCompleteEventHandle my_PushCompleteEventHandle_CA = null;
        private void applyComplete_CA()
        {
            if (this.m_pic.InvokeRequired)
            {
                this.m_pic.Invoke(new PushCompleteEventHandle(applyComplete1_CA));
            }
            else
            {
                applyComplete1_CA();
            }
        }

        private void applyComplete1_CA()
        {
            long lret = this.push_cam_mic(this.m_pic.Handle);
        }

        private void WorkThread_CA(object param)
        {
            if(my_PushCompleteEventHandle_CA != null)
                my_PushCompleteEventHandle_CA();
        }

        /// <summary>
        /// 推流摄像头+麦克风
        /// </summary>
        /// <param name="videodev">视频采集设备名称，由接口zjpusher_enumdevices返回</param>
        /// <param name="camsize">推流视频的宽高</param>
        /// <param name="audiodev">音频采集设备名称，由接口zjpusher_enumdevices返回</param>
        /// <param name="framerate">视频帧率</param>
        /// <param name="videorate">视频码率</param>
        /// <param name="audiorate">音频码率</param>
        /// <param name="url">推流地址</param>
        /// <param name="obj">预览PictureBox对象</param>
        public void pushbythread_CA(string videodev, Size camsize, string audiodev, int framerate, int videorate, int audiorate, string url, object obj)
        {
            my_PushCompleteEventHandle_CA = new PushCompleteEventHandle(applyComplete_CA);
            Thread t = new Thread(new ParameterizedThreadStart(WorkThread_CA));
            t.IsBackground = true;

            this.m_camdev = videodev;
            this.m_camsize = camsize;

            this.m_audiodev = audiodev;

            this.m_framerate = framerate;
            this.m_videorate = videorate;
            this.m_audiorate = audiorate;

            this.m_url = url;
            this.m_pic = obj as PictureBox;

            t.Start();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // 推流 摄像头
        public PushCompleteEventHandle my_PushCompleteEventHandle_C = null;
        private void applyComplete_C()
        {
            if (this.m_pic.InvokeRequired)
            {
                this.m_pic.Invoke(new PushCompleteEventHandle(applyComplete1_C));
            }
            else
            {
                applyComplete1_C();
            }
        }

        private void applyComplete1_C()
        {
            long lret = this.push_cam(this.m_pic.Handle);
        }

        private void WorkThread_C(object param)
        {
            if (my_PushCompleteEventHandle_C != null)
                my_PushCompleteEventHandle_C();
        }

        /// <summary>
        /// 推流摄像头
        /// </summary>
        /// <param name="videodev">视频采集设备名称，由接口zjpusher_enumdevices返回</param>
        /// <param name="camsize">推流视频的宽高</param>
        /// <param name="framerate">视频帧率</param>
        /// <param name="videorate">视频码率</param>
        /// <param name="url">推流地址</param>
        /// <param name="obj">预览PictureBox对象</param>
        public void pushbythread_C(string videodev, Size camsize, int framerate, int videorate, string url, object obj)
        {
            my_PushCompleteEventHandle_CA = new PushCompleteEventHandle(applyComplete_C);
            Thread t = new Thread(new ParameterizedThreadStart(WorkThread_C));
            t.IsBackground = true;

            this.m_camdev = videodev;
            this.m_camsize = camsize;
            
            this.m_framerate = framerate;
            this.m_videorate = videorate;

            this.m_url = url;
            this.m_pic = obj as PictureBox;


            t.Start();
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // 推流 桌面+麦克风
        public PushCompleteEventHandle my_PushCompleteEventHandle_DA = null;
        private void applyComplete_DA()
        {
            if (this.m_pic.InvokeRequired)
            {
                this.m_pic.Invoke(new PushCompleteEventHandle(applyComplete1_DA));
            }
            else
            {
                applyComplete1_DA();
            }
        }

        private void applyComplete1_DA()
        {
            long lret = this.push_desktop_mic(this.m_pic.Handle);
        }

        private void WorkThread_DA(object param)
        {
            if(my_PushCompleteEventHandle_DA != null)
                my_PushCompleteEventHandle_DA();
        }

        /// <summary>
        /// 推流 桌面+麦克风
        /// </summary>
        /// <param name="monitornum">桌面索引号，由接口zjpusher_enumdesktop返回</param>
        /// <param name="destktopsize">推流桌面的宽高</param>
        /// <param name="audiodev">频采集设备名称，由接口zjpusher_enumdevices返回</param>
        /// <param name="framerate">视频帧率</param>
        /// <param name="videorate">视频码率</param>
        /// <param name="audiorate">音频码率</param>
        /// <param name="url">推流地址</param>
        /// <param name="obj">预览PictureBox对象</param>
        public void pushbythread_DA(int monitornum, Size destktopsize, string audiodev, int framerate, int videorate, int audiorate, string url, object obj)
        {
            my_PushCompleteEventHandle_DA = new PushCompleteEventHandle(applyComplete_DA);
            Thread t = new Thread(new ParameterizedThreadStart(WorkThread_DA));
            t.IsBackground = true;

            this.m_desktopdevnum = monitornum;
            this.m_desktopsize = destktopsize;

            this.m_audiodev = audiodev;

            this.m_framerate = framerate;
            this.m_videorate = videorate;
            this.m_audiorate = audiorate;

            this.m_url = url;
            this.m_pic = obj as PictureBox;

            t.Start();
        }



        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // 推流 桌面
        public PushCompleteEventHandle my_PushCompleteEventHandle_D = null;
        private void applyComplete_D()
        {
            if (this.m_pic.InvokeRequired)
            {
                this.m_pic.Invoke(new PushCompleteEventHandle(applyComplete1_D));
            }
            else
            {
                applyComplete1_D();
            }
        }

        private void applyComplete1_D()
        {
            long lret = this.push_desktop(this.m_pic.Handle);
        }

        private void WorkThread_D(object param)
        {
            if (my_PushCompleteEventHandle_D != null)
                my_PushCompleteEventHandle_D();
        }

        /// <summary>
        /// 推流 桌面+麦克风
        /// </summary>
        /// <param name="monitornum">桌面索引号，由接口zjpusher_enumdesktop返回</param>
        /// <param name="destktopsize">推流桌面的宽高</param>
        /// <param name="framerate">视频帧率</param>
        /// <param name="videorate">视频码率</param>
        /// <param name="url">推流地址</param>
        /// <param name="obj">预览PictureBox对象</param>
        public void pushbythread_D(int monitornum, Size destktopsize, int framerate, int videorate, string url, object obj)
        {
            my_PushCompleteEventHandle_D = new PushCompleteEventHandle(applyComplete_D);
            Thread t = new Thread(new ParameterizedThreadStart(WorkThread_D));
            t.IsBackground = true;

            this.m_desktopdevnum = monitornum;
            this.m_desktopsize = destktopsize;


            this.m_framerate = framerate;
            this.m_videorate = videorate;

            this.m_url = url;
            this.m_pic = obj as PictureBox;

            t.Start();
        }
    }
}
