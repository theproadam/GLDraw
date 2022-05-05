using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using glcore.Blit;

namespace glcore
{
    public unsafe partial class GL
    {
        internal static int globalThreadID = 0;
        public static bool overrideThreadSafety = false;

        #region PINVOKE

        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        static extern IntPtr memcpy(IntPtr dest, IntPtr src, UIntPtr count);

        [DllImport("msvcrt.dll", EntryPoint = "memset", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        static extern IntPtr MemSet(IntPtr dest, int c, int byteCount);

        [DllImport("kernel32.dll")]
        internal static extern void RtlZeroMemory(IntPtr dst, int length);

        [DllImport("opengl32.dll")]
        internal static extern IntPtr glClearColor(float r, float g, float b, float a);

        [DllImport("opengl32.dll")]
        internal static extern void glClear(uint val);


        [DllImport("GLCore.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern internal int Initialize();

        [DllImport("GLCore.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern internal void Draw(uint shaderProgram, uint start, uint stop, uint VAO);

        [DllImport("GLCore.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern internal void BindTextures(uint* textureIDs, int textureCount);
        

        [DllImport("gdi32.dll")]
        internal static extern int SwapBuffers(IntPtr HDC);

        [DllImport("GLCore.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int GetError();
        
        [DllImport("GLCore.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void ChangeCurrentFrameBuffer(uint newFBO);

        [DllImport("GLCore.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void ChangeFrameBufferTexture(uint Tex);


        #endregion

        static GL()
        {
            if (sizeof(int*) != 4) throw new Exception("Error: GLDraw only supports 32bit applications!");

            //Initialize GL Garbage Collector
           // ThreadSafetyCheck();            
        }

        internal static void ThreadSafetyCheck()
        {
            if (!overrideThreadSafety)
            {
                if (globalThreadID == 0)
                    globalThreadID = System.Threading.Thread.CurrentThread.GetHashCode();
                else if (globalThreadID != System.Threading.Thread.CurrentThread.GetHashCode())
                    throw new Exception("Fatal Error: Cross-thread attempt detected! OpenGL contexts can only be started and used from one thread!");
            }
        }

        public static void Clear(float r, float b, float g, float a)
        {
            ChangeCurrentFrameBuffer(0);
            glClearColor(r, g, b, a);
            glClear((uint)GLClear.GL_COLOR_BUFFER_BIT | (uint) GLClear.GL_DEPTH_BUFFER_BIT);
        }

        public static void Clear(GLFramebuffer framebuffer, float r, float b, float g, float a)
        {
            ChangeCurrentFrameBuffer(framebuffer.fbo);
            ChangeFrameBufferTexture(framebuffer.tex);

            glClearColor(r, g, b, a);
            glClear((uint)GLClear.GL_COLOR_BUFFER_BIT | (uint)GLClear.GL_DEPTH_BUFFER_BIT);
        }

        public static void Clear(BlitData blitData, float r, float b, float g, float a)
        {
            blitData.MakeCurrent();

            glClearColor(r, g, b, a);
            glClear((uint)GLClear.GL_COLOR_BUFFER_BIT | (uint)GLClear.GL_DEPTH_BUFFER_BIT);
        }

        public static void Blit(BlitData bData)
        {
            SwapBuffers(bData.TargetDC);
        }

        public static void Blit(BlitData bData, GLFramebuffer framebuffer)
        {
            ChangeCurrentFrameBuffer(framebuffer.fbo);
            ChangeFrameBufferTexture(framebuffer.tex);

            SwapBuffers(bData.TargetDC);
        }

        public static void Draw(GLBuffer buffer, Shader shader)
        {
            if (shader.linkedTextures.Count != 0)
            {
                uint[] iDs = new uint[shader.linkedTextures.Count];
                for (int i = 0; i < iDs.Length; i++)
                    iDs[i] = shader.linkedTextures[i].textureID;

                fixed (uint* iptr = iDs)
                {
                    BindTextures(iptr, iDs.Length);
                }
            }

            Draw(shader.shaderProgram, 0, (uint)((buffer._size / 4) / buffer.stride), buffer.VAO);
        }

        public static void Draw(GLFramebuffer framebuffer, GLBuffer buffer, Shader shader)
        {
            if (shader.linkedTextures.Count != 0)
            {
                uint[] iDs = new uint[shader.linkedTextures.Count];
                for (int i = 0; i < iDs.Length; i++)
                    iDs[i] = shader.linkedTextures[i].textureID;

                fixed (uint* iptr = iDs)
                {
                    BindTextures(iptr, iDs.Length);
                }
            }

            ChangeFrameBufferTexture(framebuffer.tex);
            ChangeCurrentFrameBuffer(framebuffer.fbo);

            Draw(shader.shaderProgram, 0, (uint)((buffer._size / 4) / buffer.stride), buffer.VAO);
        }

        public static GLError CheckError()
        {
            return (GLError)GetError();
        }

        public static bool CheckError(out GLError type)
        {
            type = (GLError)GetError();
            return type != GLError.GL_NO_ERROR;
        }
    }

    internal struct GCTarget
    {
        internal GCType type;
        internal uint targetVBO, targetVAO;
        internal uint targetTexture;
        internal uint targetShader;
        internal uint targetFramebuffer;

        public void Dispose()
        {
            if (type == GCType.Buffer)
                GLBuffer.DeleteBuffer(targetVAO, targetVBO);
            else if (type == GCType.Texture)
                GLTexture.DeleteTexture(targetTexture);
            else throw new Exception("Not implemented!");
        }
    }

    /// <summary>
    /// GLDraw's OpenGL compatible Garbage Collector
    /// </summary>
    public unsafe static class GLGC
    {
        internal static object ThreadLock = new object();
        internal static int _msCollectInterval = 5000;
        internal static bool _initialized = false;
        internal static List<GCTarget> GCQueue = new List<GCTarget>();
        internal static Timer GC = new Timer();
        internal static bool isAllowed = true;

        public static void Initialize()
        {
            if (_initialized)
                return;

            _initialized = true;

            GC.Tick += delegate(object sender, EventArgs e)
            {
                Collect();
            };

            GC.Interval = _msCollectInterval;

            if (isAllowed)
                GC.Start();
        }

        /// <summary>
        /// The Collection Interval in Milliseconds
        /// </summary>
        public static int CollectionInterval
        { 
            get 
            { 
                return _msCollectInterval;
            }
            set
            {
                if (value <= 0) throw new Exception("Invalid Collection Interval");
                else _msCollectInterval = value;         
            } 
        }

        public static bool EnableCollection 
        { 
            get 
            {
                return isAllowed;
            } 
            set 
            { 
                if (value == false) GC.Stop(); 
                else GC.Start();

                isAllowed = value;
            } 
        }

        /// <summary>
        /// Forces a collection of OpenGL objects destined for deletion
        /// </summary>
        public static void Collect()
        {
            lock (ThreadLock)
            {
                for (int i = GCQueue.Count - 1; i >= 0; i--)
                {
                    GCQueue[i].Dispose();
                    GCQueue.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Clears the collection Queue. This can cause memory leaks
        /// </summary>
        public static void ClearQueue()
        {
            lock (ThreadLock)
            {
                GCQueue.Clear();
            }
        }
    }

    internal enum GCType
    { 
        Shader,
        Texture,
        Buffer,
        Framebuffer
    }

    public enum GLError
    { 
        GL_NO_ERROR = 0,
        GL_INVALID_ENUM = 0x0500,
        GL_INVALID_VALUE = 0x0501,
        GL_INVALID_OPERATION = 0x0502,
        GL_STACK_OVERFLOW = 0x0503,
        GL_STACK_UNDERFLOW = 0x0504,
        GL_OUT_OF_MEMORY = 0x0505,
    }

    public enum GLType
    {
        GL_FLOAT_VEC2 = 0x8B50,
        GL_FLOAT_VEC3 = 0x8B51,
        GL_FLOAT_VEC4 = 0x8B52,
        GL_INT_VEC2 = 0x8B53,
        GL_INT_VEC3 = 0x8B54,
        GL_INT_VEC4 = 0x8B55,
        GL_BOOL = 0x8B56,
        GL_BOOL_VEC2 = 0x8B57,
        GL_BOOL_VEC3 = 0x8B58,
        GL_BOOL_VEC4 = 0x8B59,
        GL_FLOAT_MAT2 = 0x8B5A,
        GL_FLOAT_MAT3 = 0x8B5B,
        GL_FLOAT_MAT4 = 0x8B5C,
        GL_SAMPLER_1D = 0x8B5D,
        GL_SAMPLER_2D = 0x8B5E,
        GL_SAMPLER_3D = 0x8B5F,
        GL_SAMPLER_CUBE = 0x8B60,
        GL_BYTE = 0x1400,
        GL_UNSIGNED_BYTE = 0x1401,
        GL_SHORT = 0x1402,
        GL_UNSIGNED_SHORT = 0x1403,
        GL_INT = 0x1404,
        GL_UNSIGNED_INT = 0x1405,
        GL_FLOAT = 0x1406,
        GL_2_BYTES = 0x1407,
        GL_3_BYTES = 0x1408,
        GL_4_BYTES = 0x1409,
        GL_DOUBLE = 0x140A
    }

    public enum GLClear
    {
        GL_DEPTH_BUFFER_BIT = 0x00000100,
        GL_STENCIL_BUFFER_BIT = 0x00000400,
        GL_COLOR_BUFFER_BIT = 0x00004000
    }

    public class BlitData : IDisposable
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll")]
        static extern int ChoosePixelFormat(IntPtr hdc, [In] ref PixelFormatDescriptor ppfd);

        [DllImport("gdi32.dll")]
        static extern int SetPixelFormat(IntPtr hdc, int format, [In] ref PixelFormatDescriptor ppfd);

        [DllImport("opengl32.dll")]
        static extern IntPtr wglCreateContext(IntPtr HDC);

        [DllImport("opengl32.dll")]
        static extern int wglDeleteContext(IntPtr HGLRC);

        [DllImport("opengl32.dll")]
        static extern IntPtr wglGetProcAddress([In, MarshalAs(UnmanagedType.LPStr)] string name);

        [DllImport("opengl32.dll")]
        static extern bool wglShareLists(IntPtr hglrc1, IntPtr hglrc2);


        [DllImport("opengl32.dll")]
        static extern void glViewport(int x, int y, int width, int height);

        [DllImport("opengl32.dll")]
        static extern IntPtr wglMakeCurrent(IntPtr HDC, IntPtr HGLRC);

        internal IntPtr TargetDC;
        internal IntPtr LinkedHandle;
        internal IntPtr m_hglrc;

        internal IntPtr shared_m_hglrc;

        bool disposed = false;

        ~BlitData()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                ReleaseDC(LinkedHandle, TargetDC);
                wglDeleteContext(m_hglrc);
                disposed = true;
            }
        }

        public void LinkTo(BlitData p1)
        {
            wglShareLists(m_hglrc, p1.m_hglrc);
            p1.m_hglrc = m_hglrc;
        }

        public unsafe BlitData(Form TargetForm)
        {
            IntPtr OutputHandle = TargetForm.Handle;

            LinkedHandle = OutputHandle;
            TargetDC = GetDC(OutputHandle);

            PixelFormatDescriptor pfd = new PixelFormatDescriptor() { 
                Size = (ushort)sizeof(PixelFormatDescriptor),
                Version = 1,
                Flags = 0x00000004 | 0x00000020 | 0x00000001, //PFD WINDOW, OPENGL, DBUFFER
                PixelType = 0, //RGBA
                DepthBits = 32, //32bit depth
                LayerType = 0 //PFD_MAIN_PLANE
            };

            int iPixelFormat;

            if((iPixelFormat = ChoosePixelFormat(TargetDC, ref pfd)) == 0)
			{
				throw new Exception("ChoosePixelFormat Failed");
			}
			 
			// make that match the device context's current pixel format 
            if (SetPixelFormat(TargetDC, iPixelFormat, ref pfd) == 0)
			{
				throw new Exception("SetPixelFormat Failed");
			}

            if ((m_hglrc = wglCreateContext(TargetDC)) == IntPtr.Zero)
			{
				throw new Exception("wglCreateContext Failed");
			}

            if((wglMakeCurrent(TargetDC, m_hglrc)) == IntPtr.Zero)
			{
                throw new Exception("wglMakeCurrent Failed");
			}

            if (GL.Initialize() != 1)
            {
                throw new Exception("Failed to Initialize!");
            }


        }

        public unsafe BlitData(IntPtr formHandle)
        {
            LinkedHandle = formHandle;
            TargetDC = GetDC(formHandle);

            PixelFormatDescriptor pfd = new PixelFormatDescriptor()
            {
                Size = (ushort)sizeof(PixelFormatDescriptor),
                Version = 1,
                Flags = 0x00000004 | 0x00000020 | 0x00000001, //PFD WINDOW, OPENGL, DBUFFER
                PixelType = 0, //RGBA
                DepthBits = 32, //32bit depth
                LayerType = 0 //PFD_MAIN_PLANE
            };

            int iPixelFormat;

            if ((iPixelFormat = ChoosePixelFormat(TargetDC, ref pfd)) == 0)
            {
                throw new Exception("ChoosePixelFormat Failed");
            }

            // make that match the device context's current pixel format 
            if (SetPixelFormat(TargetDC, iPixelFormat, ref pfd) == 0)
            {
                throw new Exception("SetPixelFormat Failed");
            }

            if ((m_hglrc = wglCreateContext(TargetDC)) == IntPtr.Zero)
            {
                throw new Exception("wglCreateContext Failed");
            }

            if ((wglMakeCurrent(TargetDC, m_hglrc)) == IntPtr.Zero)
            {
                throw new Exception("wglMakeCurrent Failed");
            }

            if (GL.Initialize() != 1)
            {
                throw new Exception("Failed to Initialize!");
            }


        }

        public void MakeCurrent()
        {
            if (shared_m_hglrc != IntPtr.Zero)
            {
                if ((wglMakeCurrent(TargetDC, shared_m_hglrc)) == IntPtr.Zero)
                {
                    throw new Exception("wglMakeCurrent Failed");
                }
            }
            else
                if ((wglMakeCurrent(TargetDC, m_hglrc)) == IntPtr.Zero)
                {
                    throw new Exception("wglMakeCurrent Failed");
                }
        }

        public void Resize(int width, int height)
        {
            MakeCurrent();

            glViewport(0, 0, width, height);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PixelFormatDescriptor
    {
        public ushort Size;
        public ushort Version;
        public uint Flags;
        public byte PixelType;
        public byte ColorBits;
        public byte RedBits;
        public byte RedShift;
        public byte GreenBits;
        public byte GreenShift;
        public byte BlueBits;
        public byte BlueShift;
        public byte AlphaBits;
        public byte AlphaShift;
        public byte AccumBits;
        public byte AccumRedBits;
        public byte AccumGreenBits;
        public byte AccumBlueBits;
        public byte AccumAlphaBits;
        public byte DepthBits;
        public byte StencilBits;
        public byte AuxBuffers;
        public byte LayerType;
        private byte Reserved;
        public uint LayerMask;
        public uint VisibleMask;
        public uint DamageMask;


    }
}

namespace glcore.Blit
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BITMAPINFOHEADER
    {
        public uint biSize;
        public int biWidth;
        public int biHeight;
        public ushort biPlanes;
        public ushort biBitCount;
        public BitmapCompressionMode biCompression;
        public uint biSizeImage;
        public int biXPelsPerMeter;
        public int biYPelsPerMeter;
        public uint biClrUsed;
        public uint biClrImportant;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RGBQUAD
    {
        public byte rgbBlue;
        public byte rgbGreen;
        public byte rgbRed;
        public byte rgbReserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct BITMAPINFO
    {
        public BITMAPINFOHEADER bmiHeader;
        public RGBQUAD bmiColors;
    }

    public enum BitmapCompressionMode : uint
    {
        BI_RGB = 0,
        BI_RLE8 = 1,
        BI_RLE4 = 2,
        BI_BITFIELDS = 3,
        BI_JPEG = 4,
        BI_PNG = 5
    }
}
