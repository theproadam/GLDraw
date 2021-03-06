using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace glcore.gl
{
    public unsafe static class GLFunc
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll")]
        public static extern int ChoosePixelFormat(IntPtr hdc, [In] ref PixelFormatDescriptor ppfd);

        [DllImport("gdi32.dll")]
        public static extern int SetPixelFormat(IntPtr hdc, int format, [In] ref PixelFormatDescriptor ppfd);

        [DllImport("opengl32.dll")]
        public static extern IntPtr wglCreateContext(IntPtr HDC);

        [DllImport("opengl32.dll")]
        public static extern int wglDeleteContext(IntPtr HGLRC);

        [DllImport("opengl32.dll")]
        public static extern IntPtr wglGetProcAddress([In, MarshalAs(UnmanagedType.LPStr)] string name);

        [DllImport("opengl32.dll")]
        public static extern bool wglShareLists(IntPtr hglrc1, IntPtr hglrc2);

        [DllImport("opengl32.dll")]
        public static extern void glViewport(int x, int y, int width, int height);

        [DllImport("opengl32.dll")]
        public static extern IntPtr wglMakeCurrent(IntPtr HDC, IntPtr HGLRC);

        [DllImport("opengl32.dll")]
        public static extern void glFlush();

        [DllImport("opengl32.dll")]
        public static extern void glClearColor(float r, float g, float b, float a); //void not intptr?

        [DllImport("opengl32.dll")]
        public static extern void glClear(uint val);

        [DllImport("opengl32.dll")]
        public static extern void glEnable(int val);

        [DllImport("opengl32.dll")]
        public static extern uint glGetError();

        [DllImport("opengl32.dll")]
        public static extern void glDeleteTextures(int n, uint* textures);

        [DllImport("opengl32.dll")]
        public static extern void glGenTextures(int n, uint* textures);

        [DllImport("opengl32.dll")]
        public static extern void glTexImage2D(uint target, int level, int internalformat, int width, int height, int border, uint format, uint type, void* data);

        [DllImport("opengl32.dll")]
        public static extern void glTexParameteri(uint target, uint pname, int param);




        //WARNING THERE MIGHT BE A TYPE MISMATCH WITH UNSIGNED CHAR (byte) AND SIGNED CHAR (sbyte)

        public delegate uint GLCreateShader(uint shader);
        public static GLCreateShader glCreateShader;

        public delegate void GLShaderSource(uint shader, int count, byte** str, int* length);
        public static GLShaderSource glShaderSource;

        public delegate void GLCompileShader(uint shader);
        public static GLCompileShader glCompileShader;

        public delegate void GLGetShaderiv( uint shader, uint pname, int* _params);
        public static GLGetShaderiv glGetShaderiv;

        public delegate void GLGetShaderInfoLog(uint shader, int maxLength, int* length, byte* infoLog);
        public static GLGetShaderInfoLog glGetShaderInfoLog;

        public delegate uint GLCreateProgram();
        public static GLCreateProgram glCreateProgram;

        public delegate void GLAttachShader(uint program, uint shader);
        public static GLAttachShader glAttachShader;

        public delegate void GLLinkProgram(uint program);
        public static GLLinkProgram glLinkProgram;

        public delegate void GLGetProgramiv( uint program, uint pname, int* _params);
        public static GLGetProgramiv glGetProgramiv;

        public delegate void GLDeleteShader(uint shader);
        public static GLDeleteShader glDeleteShader;

        public delegate void GLGetProgramInfoLog(uint program, int maxLength, int* length, byte* infoLog);
        public static GLGetProgramInfoLog glGetProgramInfoLog;

        public delegate void GLGenVertexArrays(int n, uint* arrays);
        public static GLGenVertexArrays glGenVertexArrays;

        public delegate void GLGenBuffers(int n, uint* buffers);
        public static GLGenBuffers glGenBuffers;

        public delegate void GLBindVertexArray(uint array);
        public static GLBindVertexArray glBindVertexArray;

        public delegate void GLBindBuffer(uint target, uint buffer);
        public static GLBindBuffer glBindBuffer;

        public delegate void GLBufferData(uint target, int size, void* data, uint usage); //no idea if its int size...
        public static GLBufferData glBufferData;

        public delegate void GLVertexAttribPointer(uint index, int size, uint type, byte normalized, int stride, void* pointer);
        public static GLVertexAttribPointer glVertexAttribPointer;

        public delegate void GLEnableVertexAttribArray(uint index);
        public static GLEnableVertexAttribArray glEnableVertexAttribArray;

        public delegate void GLUseProgram(uint program);
        public static GLUseProgram glUseProgram;

        public delegate void GLUniform1i(int location, int v0);
        public static GLUniform1i glUniform1i;

        public delegate int GLGetUniformLocation(uint program, byte* name);
        public static GLGetUniformLocation glGetUniformLocation;

        public delegate void GLUniformMatrix4fv(int location, int count, byte transpose, float* value);
        public static GLUniformMatrix4fv glUniformMatrix4fv;

        public delegate void GLUniform3fv(int location, int count, float* value);
        public static GLUniform3fv glUniform3fv;

        public delegate void GLUniformMatrix3fv(int location, int count, byte transpose, float* value);
        public static GLUniformMatrix3fv glUniformMatrix3fv;

        public delegate void GLGetActiveUniform(uint program, uint index, int bufSize, int* length, int* size, uint* type, byte* name);
        public static GLGetActiveUniform glGetActiveUniform;

        public delegate void GLGetActiveAttrib(uint program, uint index, int bufSize, int* length, int* size, uint* type, byte* name);
        public static GLGetActiveAttrib glGetActiveAttrib;

        public delegate void GLDeleteBuffers(int n, uint* buffers);
        public static GLDeleteBuffers glDeleteBuffers;

        public delegate void GLDeleteVertexArrays(int n, uint* arrays);
        public static GLDeleteVertexArrays glDeleteVertexArrays;

        public delegate void GLBindTexture(uint target, uint texture);
        public static GLBindTexture glBindTexture;

        public delegate void GLActiveTexture(uint texture);
        public static GLActiveTexture glActiveTexture;

        public delegate void GLDrawArrays(uint mode, int first, int count);
        public static GLDrawArrays glDrawArrays;

        public delegate void GLGenFramebuffers(int n, uint* ids);
        public static GLGenFramebuffers glGenFramebuffers;

        public delegate void GLBindFramebuffer(uint target, uint framebuffer);
        public static GLBindFramebuffer glBindFramebuffer;

        public delegate void GLGenRenderbuffers(int n, uint* renderbuffers);
        public static GLGenRenderbuffers glGenRenderbuffers;

        public delegate uint GLCheckFramebufferStatus(uint target);
        public static GLCheckFramebufferStatus glCheckFramebufferStatus;

        public delegate void GLBindRenderbuffer(uint target, uint renderbuffer);
        public static GLBindRenderbuffer glBindRenderbuffer;

        public delegate void GLRenderbufferStorage(uint target, uint internalformat, int width, int height);
        public static GLRenderbufferStorage glRenderbufferStorage;

        public delegate void GLFramebufferRenderbuffer(uint target, uint attachment, uint renderbuffertarget, uint renderbuffer);
        public static GLFramebufferRenderbuffer glFramebufferRenderbuffer;

        public delegate void GLFramebufferTexture2D(uint target, uint attachment, uint textarget, uint texture, int level);
        public static GLFramebufferTexture2D glFramebufferTexture2D;

        public delegate void GLBlitFramebuffer(int srcX0, int srcY0, int srcX1, int srcY1, int dstX0, int dstY0, int dstX1, int dstY1, uint mask, uint filter);
        public static GLBlitFramebuffer glBlitFramebuffer;

        public delegate void GLGenerateMipmap(uint target);
        public static GLGenerateMipmap glGenerateMipmap;

        public static void LoadFunctions()
        {
            //Load glCreateShader
            IntPtr glCreateShaderPtr;
            if ((glCreateShaderPtr = wglGetProcAddress("glCreateShader")) == IntPtr.Zero)
                throw new Exception("Failed To Load glCreateShader!");

            glCreateShader = (GLCreateShader)Marshal.GetDelegateForFunctionPointer(glCreateShaderPtr, typeof(GLCreateShader));
            

            //Load glShaderSource
            IntPtr glShaderSourcePtr;
            if ((glShaderSourcePtr = wglGetProcAddress("glShaderSource")) == IntPtr.Zero)
                throw new Exception("Failed To Load glCreateShader!");

            glShaderSource = (GLShaderSource)Marshal.GetDelegateForFunctionPointer(glShaderSourcePtr, typeof(GLShaderSource));


            //Load glCompileShader
            IntPtr glCompileShaderPtr;
            if ((glCompileShaderPtr = wglGetProcAddress("glCompileShader")) == IntPtr.Zero)
                throw new Exception("Failed To Load glCreateShader!");

            glCompileShader = (GLCompileShader)Marshal.GetDelegateForFunctionPointer(glCompileShaderPtr, typeof(GLCompileShader));


            //Load glGetShaderiv
            IntPtr glGetShaderivPtr;
            if ((glGetShaderivPtr = wglGetProcAddress("glGetShaderiv")) == IntPtr.Zero)
                throw new Exception("Failed To Load glCreateShader!");

            glGetShaderiv = (GLGetShaderiv)Marshal.GetDelegateForFunctionPointer(glGetShaderivPtr, typeof(GLGetShaderiv));


            //Load glGetShaderInfoLog
            IntPtr glGetShaderInfoLogPtr;
            if ((glGetShaderInfoLogPtr = wglGetProcAddress("glGetShaderInfoLog")) == IntPtr.Zero)
                throw new Exception("Failed To Load glCreateShader!");

            glGetShaderInfoLog = (GLGetShaderInfoLog)Marshal.GetDelegateForFunctionPointer(glGetShaderInfoLogPtr, typeof(GLGetShaderInfoLog));


            //Load glCreateProgram
            IntPtr glCreateProgramPtr;
            if ((glCreateProgramPtr = wglGetProcAddress("glCreateProgram")) == IntPtr.Zero)
                throw new Exception("Failed To Load glCreateShader!");

            glCreateProgram = (GLCreateProgram)Marshal.GetDelegateForFunctionPointer(glCreateProgramPtr, typeof(GLCreateProgram));


            //Load glAttachShader
            IntPtr glAttachShaderPtr;
            if ((glAttachShaderPtr = wglGetProcAddress("glAttachShader")) == IntPtr.Zero)
                throw new Exception("Failed To Load glCreateShader!");

            glAttachShader = (GLAttachShader)Marshal.GetDelegateForFunctionPointer(glAttachShaderPtr, typeof(GLAttachShader));


            //Load glLinkProgram
            IntPtr glLinkProgramPtr;
            if ((glLinkProgramPtr = wglGetProcAddress("glLinkProgram")) == IntPtr.Zero)
                throw new Exception("Failed To Load glCreateShader!");

            glLinkProgram = (GLLinkProgram)Marshal.GetDelegateForFunctionPointer(glLinkProgramPtr, typeof(GLLinkProgram));

            //Load glGetProgramiv
            IntPtr glGetProgramivPtr;
            if ((glGetProgramivPtr = wglGetProcAddress("glGetProgramiv")) == IntPtr.Zero)
                throw new Exception("Failed To Load glCreateShader!");

            glGetProgramiv = (GLGetProgramiv)Marshal.GetDelegateForFunctionPointer(glGetProgramivPtr, typeof(GLGetProgramiv));


            //Load glDeleteShader
            IntPtr glDeleteShaderPtr;
            if ((glDeleteShaderPtr = wglGetProcAddress("glDeleteShader")) == IntPtr.Zero)
                throw new Exception("Failed To Load glCreateShader!");

            glDeleteShader = (GLDeleteShader)Marshal.GetDelegateForFunctionPointer(glDeleteShaderPtr, typeof(GLDeleteShader));


            //Load glGetProgramInfoLog
            IntPtr glGetProgramInfoLogPtr;
            if ((glGetProgramInfoLogPtr = wglGetProcAddress("glGetProgramInfoLog")) == IntPtr.Zero)
                throw new Exception("Failed To Load glCreateShader!");

            glGetProgramInfoLog = (GLGetProgramInfoLog)Marshal.GetDelegateForFunctionPointer(glGetProgramInfoLogPtr, typeof(GLGetProgramInfoLog));


            //Load glGenVertexArrays
            IntPtr glGenVertexArraysPtr;
            if ((glGenVertexArraysPtr = wglGetProcAddress("glGenVertexArrays")) == IntPtr.Zero)
                throw new Exception("Failed To Load glCreateShader!");

            glGenVertexArrays = (GLGenVertexArrays)Marshal.GetDelegateForFunctionPointer(glGenVertexArraysPtr, typeof(GLGenVertexArrays));


            //Load glGenBuffers
            IntPtr glGenBuffersPtr;
            if ((glGenBuffersPtr = wglGetProcAddress("glGenBuffers")) == IntPtr.Zero)
                throw new Exception("Failed To Load glCreateShader!");

            glGenBuffers = (GLGenBuffers)Marshal.GetDelegateForFunctionPointer(glGenBuffersPtr, typeof(GLGenBuffers));


            //Load glBindVertexArray
            IntPtr glBindVertexArrayPtr;
            if ((glBindVertexArrayPtr = wglGetProcAddress("glBindVertexArray")) == IntPtr.Zero)
                throw new Exception("Failed To Load glCreateShader!");

            glBindVertexArray = (GLBindVertexArray)Marshal.GetDelegateForFunctionPointer(glBindVertexArrayPtr, typeof(GLBindVertexArray));


            //Load glBindBuffer
            IntPtr glBindBufferPtr;
            if ((glBindBufferPtr = wglGetProcAddress("glBindBuffer")) == IntPtr.Zero)
                throw new Exception("Failed To Load glCreateShader!");

            glBindBuffer = (GLBindBuffer)Marshal.GetDelegateForFunctionPointer(glBindBufferPtr, typeof(GLBindBuffer));


            //Load glBufferData
            IntPtr glBufferDataPtr;
            if ((glBufferDataPtr = wglGetProcAddress("glBufferData")) == IntPtr.Zero)
                throw new Exception("Failed To Load glCreateShader!");

            glBufferData = (GLBufferData)Marshal.GetDelegateForFunctionPointer(glBufferDataPtr, typeof(GLBufferData));


            //Load glVertexAttribPointer
            IntPtr glVertexAttribPointerPtr;
            if ((glVertexAttribPointerPtr = wglGetProcAddress("glVertexAttribPointer")) == IntPtr.Zero)
                throw new Exception("Failed To Load glCreateShader!");

            glVertexAttribPointer = (GLVertexAttribPointer)Marshal.GetDelegateForFunctionPointer(glVertexAttribPointerPtr, typeof(GLVertexAttribPointer));


            //Load glEnableVertexAttribArray
            IntPtr glEnableVertexAttribArrayPtr;
            if ((glEnableVertexAttribArrayPtr = wglGetProcAddress("glEnableVertexAttribArray")) == IntPtr.Zero)
                throw new Exception("Failed To Load glCreateShader!");

            glEnableVertexAttribArray = (GLEnableVertexAttribArray)Marshal.GetDelegateForFunctionPointer(glEnableVertexAttribArrayPtr, typeof(GLEnableVertexAttribArray));


            //Load glUseProgram
            IntPtr glUseProgramPtr;
            if ((glUseProgramPtr = wglGetProcAddress("glUseProgram")) == IntPtr.Zero)
                throw new Exception("Failed To Load glCreateShader!");

            glUseProgram = (GLUseProgram)Marshal.GetDelegateForFunctionPointer(glUseProgramPtr, typeof(GLUseProgram));


            //Load glUniform1i
            IntPtr glUniform1iPtr;
            if ((glUniform1iPtr = wglGetProcAddress("glUniform1i")) == IntPtr.Zero)
                throw new Exception("Failed To Load glCreateShader!");

            glUniform1i = (GLUniform1i)Marshal.GetDelegateForFunctionPointer(glUniform1iPtr, typeof(GLUniform1i));


            //Load glGetUniformLocation
            IntPtr glGetUniformLocationPtr;
            if ((glGetUniformLocationPtr = wglGetProcAddress("glGetUniformLocation")) == IntPtr.Zero)
                throw new Exception("Failed To Load glCreateShader!");

            glGetUniformLocation = (GLGetUniformLocation)Marshal.GetDelegateForFunctionPointer(glGetUniformLocationPtr, typeof(GLGetUniformLocation));


            //Load glUniformMatrix4fv
            IntPtr glUniformMatrix4fvPtr;
            if ((glUniformMatrix4fvPtr = wglGetProcAddress("glUniformMatrix4fv")) == IntPtr.Zero)
                throw new Exception("Failed To Load glCreateShader!");

            glUniformMatrix4fv = (GLUniformMatrix4fv)Marshal.GetDelegateForFunctionPointer(glUniformMatrix4fvPtr, typeof(GLUniformMatrix4fv));

            //Load glUniform3fv
            IntPtr glUniform3fvPtr;
            if ((glUniform3fvPtr = wglGetProcAddress("glUniform3fv")) == IntPtr.Zero)
                throw new Exception("Failed To Load glCreateShader!");

            glUniform3fv = (GLUniform3fv)Marshal.GetDelegateForFunctionPointer(glUniform3fvPtr, typeof(GLUniform3fv));

            //Load glUniformMatrix3fv
            IntPtr glUniformMatrix3fvPtr;
            if ((glUniformMatrix3fvPtr = wglGetProcAddress("glUniformMatrix3fv")) == IntPtr.Zero)
                throw new Exception("Failed To Load glCreateShader!");

            glUniformMatrix3fv = (GLUniformMatrix3fv)Marshal.GetDelegateForFunctionPointer(glUniformMatrix3fvPtr, typeof(GLUniformMatrix3fv));

            //Load glGetActiveUniform
            IntPtr glGetActiveUniformPtr;
            if ((glGetActiveUniformPtr = wglGetProcAddress("glGetActiveUniform")) == IntPtr.Zero)
                throw new Exception("Failed To Load glCreateShader!");

            glGetActiveUniform = (GLGetActiveUniform)Marshal.GetDelegateForFunctionPointer(glGetActiveUniformPtr, typeof(GLGetActiveUniform));

            //Load glGetActiveAttrib
            IntPtr glGetActiveAttribPtr;
            if ((glGetActiveAttribPtr = wglGetProcAddress("glGetActiveAttrib")) == IntPtr.Zero)
                throw new Exception("Failed To Load glCreateShader!");

            glGetActiveAttrib = (GLGetActiveAttrib)Marshal.GetDelegateForFunctionPointer(glGetActiveAttribPtr, typeof(GLGetActiveAttrib));


            //Load glDeleteBuffers
            IntPtr glDeleteBuffersPtr;
            if ((glDeleteBuffersPtr = wglGetProcAddress("glDeleteBuffers")) == IntPtr.Zero)
                throw new Exception("Failed To Load glCreateShader!");

            glDeleteBuffers = (GLDeleteBuffers)Marshal.GetDelegateForFunctionPointer(glDeleteBuffersPtr, typeof(GLDeleteBuffers));


            //Load glDeleteVertexArrays
            IntPtr glDeleteVertexArraysPtr;
            if ((glDeleteVertexArraysPtr = wglGetProcAddress("glDeleteVertexArrays")) == IntPtr.Zero)
                throw new Exception("Failed To Load glCreateShader!");

            glDeleteVertexArrays = (GLDeleteVertexArrays)Marshal.GetDelegateForFunctionPointer(glDeleteVertexArraysPtr, typeof(GLDeleteVertexArrays));


            //Load glBindTexture
            IntPtr glBindTexturePtr;
            if ((glBindTexturePtr = wglGetProcAddress("glBindTexture")) == IntPtr.Zero)
                throw new Exception("Failed To Load glCreateShader!");

            glBindTexture = (GLBindTexture)Marshal.GetDelegateForFunctionPointer(glBindTexturePtr, typeof(GLBindTexture));

            //Load glActiveTexture
            IntPtr glActiveTexturePtr;
            if ((glActiveTexturePtr = wglGetProcAddress("glActiveTexture")) == IntPtr.Zero)
                throw new Exception("Failed To Load glCreateShader!");

            glActiveTexture = (GLActiveTexture)Marshal.GetDelegateForFunctionPointer(glActiveTexturePtr, typeof(GLActiveTexture));

            //Load glDrawArrays
            IntPtr glDrawArraysPtr;
            if ((glDrawArraysPtr = wglGetProcAddress("glDrawArrays")) == IntPtr.Zero)
                throw new Exception("Failed To Load glCreateShader!");

            glDrawArrays = (GLDrawArrays)Marshal.GetDelegateForFunctionPointer(glDrawArraysPtr, typeof(GLDrawArrays));

            //Load glGenFramebuffers
            IntPtr glGenFramebuffersPtr;
            if ((glGenFramebuffersPtr = wglGetProcAddress("glGenFramebuffers")) == IntPtr.Zero)
                throw new Exception("Failed To Load glCreateShader!");

            glGenFramebuffers = (GLGenFramebuffers)Marshal.GetDelegateForFunctionPointer(glGenFramebuffersPtr, typeof(GLGenFramebuffers));

            //Load glBindFramebuffer
            IntPtr glBindFramebufferPtr;
            if ((glBindFramebufferPtr = wglGetProcAddress("glBindFramebuffer")) == IntPtr.Zero)
                throw new Exception("Failed To Load glCreateShader!");

            glBindFramebuffer = (GLBindFramebuffer)Marshal.GetDelegateForFunctionPointer(glBindFramebufferPtr, typeof(GLBindFramebuffer));


            //Load glGenRenderbuffers
            IntPtr glGenRenderbuffersPtr;
            if ((glGenRenderbuffersPtr = wglGetProcAddress("glGenRenderbuffers")) == IntPtr.Zero)
                throw new Exception("Failed To Load glCreateShader!");

            glGenRenderbuffers = (GLGenRenderbuffers)Marshal.GetDelegateForFunctionPointer(glGenRenderbuffersPtr, typeof(GLGenRenderbuffers));

            //Load glCheckFramebufferStatus
            IntPtr glCheckFramebufferStatusPtr;
            if ((glCheckFramebufferStatusPtr = wglGetProcAddress("glCheckFramebufferStatus")) == IntPtr.Zero)
                throw new Exception("Failed To Load glCreateShader!");

            glCheckFramebufferStatus = (GLCheckFramebufferStatus)Marshal.GetDelegateForFunctionPointer(glCheckFramebufferStatusPtr, typeof(GLCheckFramebufferStatus));

            //Load glBindRenderbuffer
            IntPtr glBindRenderbufferPtr;
            if ((glBindRenderbufferPtr = wglGetProcAddress("glBindRenderbuffer")) == IntPtr.Zero)
                throw new Exception("Failed To Load glCreateShader!");

            glBindRenderbuffer = (GLBindRenderbuffer)Marshal.GetDelegateForFunctionPointer(glBindRenderbufferPtr, typeof(GLBindRenderbuffer));

            //Load glRenderbufferStorage
            IntPtr glRenderbufferStoragePtr;
            if ((glRenderbufferStoragePtr = wglGetProcAddress("glRenderbufferStorage")) == IntPtr.Zero)
                throw new Exception("Failed To Load glCreateShader!");

            glRenderbufferStorage = (GLRenderbufferStorage)Marshal.GetDelegateForFunctionPointer(glRenderbufferStoragePtr, typeof(GLRenderbufferStorage));

            //Load glFramebufferRenderbuffer
            IntPtr glFramebufferRenderbufferPtr;
            if ((glFramebufferRenderbufferPtr = wglGetProcAddress("glFramebufferRenderbuffer")) == IntPtr.Zero)
                throw new Exception("Failed To Load glCreateShader!");

            glFramebufferRenderbuffer = (GLFramebufferRenderbuffer)Marshal.GetDelegateForFunctionPointer(glFramebufferRenderbufferPtr, typeof(GLFramebufferRenderbuffer));

            //Load glFramebufferTexture2D
            IntPtr glFramebufferTexture2DPtr;
            if ((glFramebufferTexture2DPtr = wglGetProcAddress("glFramebufferTexture2D")) == IntPtr.Zero)
                throw new Exception("Failed To Load glCreateShader!");

            glFramebufferTexture2D = (GLFramebufferTexture2D)Marshal.GetDelegateForFunctionPointer(glFramebufferTexture2DPtr, typeof(GLFramebufferTexture2D));

            //Load glBlitFramebuffer
            IntPtr glBlitFramebufferPtr;
            if ((glBlitFramebufferPtr = wglGetProcAddress("glBlitFramebuffer")) == IntPtr.Zero)
                throw new Exception("Failed To Load glCreateShader!");

            glBlitFramebuffer = (GLBlitFramebuffer)Marshal.GetDelegateForFunctionPointer(glBlitFramebufferPtr, typeof(GLBlitFramebuffer));


            //Load glGenerateMipmap
            IntPtr glGenerateMipmapPtr;
            if ((glGenerateMipmapPtr = wglGetProcAddress("glGenerateMipmap")) == IntPtr.Zero)
                throw new Exception("Failed To Load glCreateShader!");

            glGenerateMipmap = (GLGenerateMipmap)Marshal.GetDelegateForFunctionPointer(glGenerateMipmapPtr, typeof(GLGenerateMipmap));
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
