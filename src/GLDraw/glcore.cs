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

        internal static bool contextReady = false;

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

        [DllImport("opengl32.dll")]
        internal static extern void glEnable(int val);



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
        static extern internal void ChangeCurrentFrameBuffer(uint newFBO);

        [DllImport("GLCore.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern internal void ChangeFrameBufferTexture(uint Tex);


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
            if (shader.linkedTextures.Count != 0 || shader.linkedFramebuffers.Count != 0)
            {
                uint[] iDs = new uint[shader.linkedTextures.Count + shader.linkedFramebuffers.Count];
                for (int i = 0; i < shader.linkedTextures.Count; i++)
                    iDs[i] = shader.linkedTextures[i].textureID;

                for (int i = shader.linkedTextures.Count; i < shader.linkedTextures.Count + shader.linkedFramebuffers.Count; i++)
                    iDs[i] = shader.linkedFramebuffers[i].tex;


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

            ChangeCurrentFrameBuffer(0);
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
            return;

            throw new Exception("temporarily disabled"); //not implmented correctly :(

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

    #region Enums

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

    #endregion

    public partial class BlitData : IDisposable
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

        [DllImport("opengl32.dll")]
        static extern void glFlush();


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
            MakeCurrent();

            if (m_hglrc == p1.m_hglrc)
                throw new Exception("The two blitData's already share the same rendering context!");

            if (!wglShareLists(m_hglrc, p1.m_hglrc))
                throw new Exception("wglShareLists Failed!");

         //   p1.m_hglrc = m_hglrc;
            p1.shared_m_hglrc = m_hglrc;
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

            GL.contextReady = true;
        }

        public void Bind()
        {
            GL.ChangeCurrentFrameBuffer(0);
            GL.ChangeFrameBufferTexture(0);

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

            GL.contextReady = true;
        }

        public unsafe BlitData(IntPtr formHandle, BlitData existingContext)
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


            m_hglrc = existingContext.m_hglrc;

            if ((wglMakeCurrent(TargetDC, m_hglrc)) == IntPtr.Zero)
            {
                throw new Exception("wglMakeCurrent Failed");
            }

            if (GL.Initialize() != 1)
            {
                throw new Exception("Failed to Initialize!");
            }

            GL.contextReady = true;
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
            glFlush();

            MakeCurrent();

            glFlush();

            glViewport(0, 0, width, height);

            glFlush();
        }
    }

    public enum WGL_ARB
    {
        WGL_NUMBER_PIXEL_FORMATS_ARB = 0x2000,
        WGL_DRAW_TO_WINDOW_ARB = 0x2001,
        WGL_DRAW_TO_BITMAP_ARB = 0x2002,
        WGL_ACCELERATION_ARB = 0x2003,
        WGL_NEED_PALETTE_ARB = 0x2004,
        WGL_NEED_SYSTEM_PALETTE_ARB = 0x2005,
        WGL_SWAP_LAYER_BUFFERS_ARB = 0x2006,
        WGL_SWAP_METHOD_ARB = 0x2007,
        WGL_NUMBER_OVERLAYS_ARB = 0x2008,
        WGL_NUMBER_UNDERLAYS_ARB = 0x2009,
        WGL_TRANSPARENT_ARB = 0x200A,
        WGL_TRANSPARENT_RED_VALUE_ARB = 0x2037,
        WGL_TRANSPARENT_GREEN_VALUE_ARB = 0x2038,
        WGL_TRANSPARENT_BLUE_VALUE_ARB = 0x2039,
        WGL_TRANSPARENT_ALPHA_VALUE_ARB = 0x203A,
        WGL_TRANSPARENT_INDEX_VALUE_ARB = 0x203B,
        WGL_SHARE_DEPTH_ARB = 0x200C,
        WGL_SHARE_STENCIL_ARB = 0x200D,
        WGL_SHARE_ACCUM_ARB = 0x200E,
        WGL_SUPPORT_GDI_ARB = 0x200F,
        WGL_SUPPORT_OPENGL_ARB = 0x2010,
        WGL_DOUBLE_BUFFER_ARB = 0x2011,
        WGL_STEREO_ARB = 0x2012,
        WGL_PIXEL_TYPE_ARB = 0x2013,
        WGL_COLOR_BITS_ARB = 0x2014,
        WGL_RED_BITS_ARB = 0x2015,
        WGL_RED_SHIFT_ARB = 0x2016,
        WGL_GREEN_BITS_ARB = 0x2017,
        WGL_GREEN_SHIFT_ARB = 0x2018,
        WGL_BLUE_BITS_ARB = 0x2019,
        WGL_BLUE_SHIFT_ARB = 0x201A,
        WGL_ALPHA_BITS_ARB = 0x201B,
        WGL_ALPHA_SHIFT_ARB = 0x201C,
        WGL_ACCUM_BITS_ARB = 0x201D,
        WGL_ACCUM_RED_BITS_ARB = 0x201E,
        WGL_ACCUM_GREEN_BITS_ARB = 0x201F,
        WGL_ACCUM_BLUE_BITS_ARB = 0x2020,
        WGL_ACCUM_ALPHA_BITS_ARB = 0x2021,
        WGL_DEPTH_BITS_ARB = 0x2022,
        WGL_STENCIL_BITS_ARB = 0x2023,
        WGL_AUX_BUFFERS_ARB = 0x2024,
        WGL_NO_ACCELERATION_ARB = 0x2025,
        WGL_GENERIC_ACCELERATION_ARB = 0x2026,
        WGL_FULL_ACCELERATION_ARB = 0x2027,
        WGL_SWAP_EXCHANGE_ARB = 0x2028,
        WGL_SWAP_COPY_ARB = 0x2029,
        WGL_SWAP_UNDEFINED_ARB = 0x202A,
        WGL_TYPE_RGBA_ARB = 0x202B,
        WGL_TYPE_COLORINDEX_ARB = 0x202C,
        WGL_SAMPLE_BUFFERS_ARB = 0x2041,
        WGL_SAMPLES_ARB = 0x2042
    }

    public unsafe partial class BlitData : IDisposable
    {
        [DllImport("GLCore.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern internal int wglSetPixelFormat(void* DC, int* aList, float* fList, uint nMaxFormats, int* nPixelFormat2, uint* nNumFormat);


        public BlitData(object target, params int[] wglConfig)
        {
            IntPtr handle;
            if (target.GetType() == typeof(IntPtr))
                handle = (IntPtr)target;
            else if (target.GetType().BaseType == typeof(Control))
                handle = ((Control)target).Handle;
            else if (target.GetType().BaseType == typeof(Form))
                handle = ((Form)target).Handle;
            else throw new Exception("Target must be a Control/Form or a Handle!");



            LinkedHandle = handle;
            TargetDC = GetDC(handle);

            //1. Create Dummy Context

            //dummy context
            Form dummyForm = new Form();
            BlitData tempData = new BlitData(dummyForm);


            int GL_TRUE = 1;

            int[] piAttribIList = new int[] { 
                (int)WGL_ARB.WGL_DRAW_TO_WINDOW_ARB, GL_TRUE,
                (int)WGL_ARB.WGL_SUPPORT_OPENGL_ARB, GL_TRUE,
                (int)WGL_ARB.WGL_COLOR_BITS_ARB, 32,
                (int)WGL_ARB.WGL_RED_BITS_ARB, 8,
                (int)WGL_ARB.WGL_GREEN_BITS_ARB, 8,
                (int)WGL_ARB.WGL_BLUE_BITS_ARB, 8,
                (int)WGL_ARB.WGL_ALPHA_BITS_ARB, 8,
                (int)WGL_ARB.WGL_DEPTH_BITS_ARB, 24,
                (int)WGL_ARB.WGL_STENCIL_BITS_ARB, 0,
                (int)WGL_ARB.WGL_DOUBLE_BUFFER_ARB, GL_TRUE,
                (int)WGL_ARB.WGL_PIXEL_TYPE_ARB, (int)WGL_ARB.WGL_TYPE_RGBA_ARB,
                (int)WGL_ARB.WGL_SAMPLE_BUFFERS_ARB, GL_TRUE,
                (int)WGL_ARB.WGL_SAMPLES_ARB, 16,
                0, 0 };

            float[] fList = new float[2];

            int pixelFormat;
            uint nNumFormats;

            int result;

            fixed (int* iptr = piAttribIList)
            {
                fixed (float* fptr = fList)
                {
                    result = wglSetPixelFormat((void*)TargetDC, iptr, fptr, 1, &pixelFormat, &nNumFormats);
                }
            }

            if (result != 1)
                throw new Exception("wglSetPixelFormat");

            PixelFormatDescriptor pfd = new PixelFormatDescriptor()
            {
                Size = (ushort)sizeof(PixelFormatDescriptor),
                Version = 1,
                Flags = 0x00000004 | 0x00000020 | 0x00000001, //PFD WINDOW, OPENGL, DBUFFER
                PixelType = 0, //RGBA
                DepthBits = 24, //32bit depth
                LayerType = 0, //PFD_MAIN_PLANE
                ColorBits = 32
                
            };


            // make that match the device context's current pixel format 
            if (SetPixelFormat(TargetDC, pixelFormat, ref pfd) == 0)
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

            GL.glEnable(0x0B71);
            GL.glEnable(0x809D);
            
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
