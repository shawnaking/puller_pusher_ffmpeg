using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace testpusher
{
    class zjfacedetect
    {
#if X64
        const string FACEDETETECTRDLLNAME = "zjfacedetect64.dll";
#else
        const string FACEDETETECTRDLLNAME = "zjfacedetect.dll";
#endif

        [DllImport(FACEDETETECTRDLLNAME, EntryPoint = "initdetectface", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl),]
        // 初始化OpenCV人脸检测环境，confpath未etc目录所在绝对路径
        public static extern int initdetectface(string confpath);

        [DllImport(FACEDETETECTRDLLNAME, EntryPoint = "detectfacebypic", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl),]
        // 根据PNG\JPG图片进行人脸检测
        // 检测到则返回大于0的值，未检测到人脸则返回0
        public static extern int detectfacebypic(string filepath);
    }
}
