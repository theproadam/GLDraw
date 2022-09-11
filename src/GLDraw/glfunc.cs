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
        public static extern void glEnable(uint val);

        [DllImport("opengl32.dll")]
        public static extern void glDisable(uint val);

        [DllImport("opengl32.dll")]
        public static extern uint glGetError();

        [DllImport("opengl32.dll")]
        public static extern void glDeleteTextures(int n, uint* textures);

        [DllImport("opengl32.dll")]
        public static extern void glGenTextures(int n, uint* textures);

        [DllImport("opengl32.dll")]
        public static extern void glTexImage2D(uint target, int level, int internalformat, int width, int height, int border, uint format, uint type, void* data);

        [DllImport("opengl32.dll")]
        public static extern void glTexImage1D(uint target, int level, int internalformat, int width, int border, uint format, uint type, void* data);

        [DllImport("opengl32.dll")]
        public static extern void glTexParameteri(uint target, uint pname, int param);

        [DllImport("opengl32.dll")]
        public static extern void glLineWidth(float width);

        [DllImport("opengl32.dll")]
        public static extern void glReadPixels(int x, int y, int width, int height, uint format, uint type, void* data);

        [DllImport("opengl32.dll")]
        public static extern void glBlendFunc(uint sFactor, uint dFactor);

        [DllImport("opengl32.dll")]
        public static extern void glPolygonOffset(float factor, float units);
       
        [DllImport("opengl32.dll")]
        public static extern void glCullFace(uint mode);

        [DllImport("opengl32.dll")]
        public static extern void glTexSubImage1D(uint target, int level, int xoffset, int width, uint format, uint type, void* pixels);

        [DllImport("opengl32.dll")]
        public static extern void glGetTexImage(uint target, int level, uint format, uint type, void* pixels);

        [DllImport("opengl32.dll")]
        public static extern void glGetTexLevelParameteriv(uint target, int level, uint pname, int* _params);

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

        public delegate void GLUniform1ui(int location, uint v0);
        public static GLUniform1ui glUniform1ui;

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

        [return: MarshalAs(UnmanagedType.Bool)]
        public delegate bool WGLChoosePixelFormatARB(void* DC, int* aList, float* fList, uint nMaxFormats, int* nPixelFormat2, uint* nNumFormat);
        public static WGLChoosePixelFormatARB wglChoosePixelFormatARB;

        public delegate void GLUniform4f(int location, float v0, float v1, float v2, float v3);
        public static GLUniform4f glUniform4f;

        public delegate void GLUniform4fv(int location, int count, float* value);
        public static GLUniform4fv glUniform4fv;

        public delegate void GLDrawBuffers(int n, uint* bufs);
        public static GLDrawBuffers glDrawBuffers;

        public delegate void GLTexImage2DMultisample(uint target, int samples, uint internalformat, int width, int height, byte fixedsamplelocations);
        public static GLTexImage2DMultisample glTexImage2DMultisample;

        public delegate void GLRenderbufferStorageMultisample(uint target, int samples, uint internalformat, int width, int height);
        public static GLRenderbufferStorageMultisample glRenderbufferStorageMultisample;

        public delegate void GLDeleteRenderbuffers(int n, uint* renderbuffers);
        public static GLDeleteRenderbuffers glDeleteRenderbuffers;

        public delegate void GLDeleteFramebuffers(int n, uint* framebuffers);
        public static GLDeleteFramebuffers glDeleteFramebuffers;

        public delegate void GLUniform1f(int location, float v0);
        public static GLUniform1f glUniform1f;

        public delegate void GLTexBuffer(uint target, uint internalformat, uint buffer);
        public static GLTexBuffer glTexBuffer;

        public delegate void GLTexBufferRange(uint target, uint internalformat, uint buffer, IntPtr offset, IntPtr size);
        public static GLTexBufferRange glTexBufferRange;

        public delegate void GLBufferSubData(uint target, IntPtr offset, IntPtr size, void* data);
        public static GLBufferSubData glBufferSubData;

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
                throw new Exception("Failed To Load glShaderSource!");

            glShaderSource = (GLShaderSource)Marshal.GetDelegateForFunctionPointer(glShaderSourcePtr, typeof(GLShaderSource));


            //Load glCompileShader
            IntPtr glCompileShaderPtr;
            if ((glCompileShaderPtr = wglGetProcAddress("glCompileShader")) == IntPtr.Zero)
                throw new Exception("Failed To Load glCompileShader!");

            glCompileShader = (GLCompileShader)Marshal.GetDelegateForFunctionPointer(glCompileShaderPtr, typeof(GLCompileShader));


            //Load glGetShaderiv
            IntPtr glGetShaderivPtr;
            if ((glGetShaderivPtr = wglGetProcAddress("glGetShaderiv")) == IntPtr.Zero)
                throw new Exception("Failed To Load glGetShaderiv!");

            glGetShaderiv = (GLGetShaderiv)Marshal.GetDelegateForFunctionPointer(glGetShaderivPtr, typeof(GLGetShaderiv));


            //Load glGetShaderInfoLog
            IntPtr glGetShaderInfoLogPtr;
            if ((glGetShaderInfoLogPtr = wglGetProcAddress("glGetShaderInfoLog")) == IntPtr.Zero)
                throw new Exception("Failed To Load glGetShaderInfoLog!");

            glGetShaderInfoLog = (GLGetShaderInfoLog)Marshal.GetDelegateForFunctionPointer(glGetShaderInfoLogPtr, typeof(GLGetShaderInfoLog));


            //Load glCreateProgram
            IntPtr glCreateProgramPtr;
            if ((glCreateProgramPtr = wglGetProcAddress("glCreateProgram")) == IntPtr.Zero)
                throw new Exception("Failed To Load glCreateProgram!");

            glCreateProgram = (GLCreateProgram)Marshal.GetDelegateForFunctionPointer(glCreateProgramPtr, typeof(GLCreateProgram));


            //Load glAttachShader
            IntPtr glAttachShaderPtr;
            if ((glAttachShaderPtr = wglGetProcAddress("glAttachShader")) == IntPtr.Zero)
                throw new Exception("Failed To Load glAttachShader!");

            glAttachShader = (GLAttachShader)Marshal.GetDelegateForFunctionPointer(glAttachShaderPtr, typeof(GLAttachShader));


            //Load glLinkProgram
            IntPtr glLinkProgramPtr;
            if ((glLinkProgramPtr = wglGetProcAddress("glLinkProgram")) == IntPtr.Zero)
                throw new Exception("Failed To Load glLinkProgram!");

            glLinkProgram = (GLLinkProgram)Marshal.GetDelegateForFunctionPointer(glLinkProgramPtr, typeof(GLLinkProgram));

            //Load glGetProgramiv
            IntPtr glGetProgramivPtr;
            if ((glGetProgramivPtr = wglGetProcAddress("glGetProgramiv")) == IntPtr.Zero)
                throw new Exception("Failed To Load glGetProgramiv!");

            glGetProgramiv = (GLGetProgramiv)Marshal.GetDelegateForFunctionPointer(glGetProgramivPtr, typeof(GLGetProgramiv));


            //Load glDeleteShader
            IntPtr glDeleteShaderPtr;
            if ((glDeleteShaderPtr = wglGetProcAddress("glDeleteShader")) == IntPtr.Zero)
                throw new Exception("Failed To Load glDeleteShader!");

            glDeleteShader = (GLDeleteShader)Marshal.GetDelegateForFunctionPointer(glDeleteShaderPtr, typeof(GLDeleteShader));


            //Load glGetProgramInfoLog
            IntPtr glGetProgramInfoLogPtr;
            if ((glGetProgramInfoLogPtr = wglGetProcAddress("glGetProgramInfoLog")) == IntPtr.Zero)
                throw new Exception("Failed To Load glGetProgramInfoLog!");

            glGetProgramInfoLog = (GLGetProgramInfoLog)Marshal.GetDelegateForFunctionPointer(glGetProgramInfoLogPtr, typeof(GLGetProgramInfoLog));


            //Load glGenVertexArrays
            IntPtr glGenVertexArraysPtr;
            if ((glGenVertexArraysPtr = wglGetProcAddress("glGenVertexArrays")) == IntPtr.Zero)
                throw new Exception("Failed To Load glGenVertexArrays!");

            glGenVertexArrays = (GLGenVertexArrays)Marshal.GetDelegateForFunctionPointer(glGenVertexArraysPtr, typeof(GLGenVertexArrays));


            //Load glGenBuffers
            IntPtr glGenBuffersPtr;
            if ((glGenBuffersPtr = wglGetProcAddress("glGenBuffers")) == IntPtr.Zero)
                throw new Exception("Failed To Load glGenBuffers!");

            glGenBuffers = (GLGenBuffers)Marshal.GetDelegateForFunctionPointer(glGenBuffersPtr, typeof(GLGenBuffers));


            //Load glBindVertexArray
            IntPtr glBindVertexArrayPtr;
            if ((glBindVertexArrayPtr = wglGetProcAddress("glBindVertexArray")) == IntPtr.Zero)
                throw new Exception("Failed To Load glBindVertexArray!");

            glBindVertexArray = (GLBindVertexArray)Marshal.GetDelegateForFunctionPointer(glBindVertexArrayPtr, typeof(GLBindVertexArray));


            //Load glBindBuffer
            IntPtr glBindBufferPtr;
            if ((glBindBufferPtr = wglGetProcAddress("glBindBuffer")) == IntPtr.Zero)
                throw new Exception("Failed To Load glBindBuffer!");

            glBindBuffer = (GLBindBuffer)Marshal.GetDelegateForFunctionPointer(glBindBufferPtr, typeof(GLBindBuffer));


            //Load glBufferData
            IntPtr glBufferDataPtr;
            if ((glBufferDataPtr = wglGetProcAddress("glBufferData")) == IntPtr.Zero)
                throw new Exception("Failed To Load glBufferData!");

            glBufferData = (GLBufferData)Marshal.GetDelegateForFunctionPointer(glBufferDataPtr, typeof(GLBufferData));


            //Load glVertexAttribPointer
            IntPtr glVertexAttribPointerPtr;
            if ((glVertexAttribPointerPtr = wglGetProcAddress("glVertexAttribPointer")) == IntPtr.Zero)
                throw new Exception("Failed To Load glVertexAttribPointer!");

            glVertexAttribPointer = (GLVertexAttribPointer)Marshal.GetDelegateForFunctionPointer(glVertexAttribPointerPtr, typeof(GLVertexAttribPointer));


            //Load glEnableVertexAttribArray
            IntPtr glEnableVertexAttribArrayPtr;
            if ((glEnableVertexAttribArrayPtr = wglGetProcAddress("glEnableVertexAttribArray")) == IntPtr.Zero)
                throw new Exception("Failed To Load glEnableVertexAttribArray!");

            glEnableVertexAttribArray = (GLEnableVertexAttribArray)Marshal.GetDelegateForFunctionPointer(glEnableVertexAttribArrayPtr, typeof(GLEnableVertexAttribArray));


            //Load glUseProgram
            IntPtr glUseProgramPtr;
            if ((glUseProgramPtr = wglGetProcAddress("glUseProgram")) == IntPtr.Zero)
                throw new Exception("Failed To Load glUseProgram!");

            glUseProgram = (GLUseProgram)Marshal.GetDelegateForFunctionPointer(glUseProgramPtr, typeof(GLUseProgram));


            //Load glUniform1i
            IntPtr glUniform1iPtr;
            if ((glUniform1iPtr = wglGetProcAddress("glUniform1i")) == IntPtr.Zero)
                throw new Exception("Failed To Load glUniform1i!");

            glUniform1i = (GLUniform1i)Marshal.GetDelegateForFunctionPointer(glUniform1iPtr, typeof(GLUniform1i));

            //Load glUniform1ui
            IntPtr glUniform1uiPtr;
            if ((glUniform1uiPtr = wglGetProcAddress("glUniform1ui")) == IntPtr.Zero)
                throw new Exception("Failed To Load glUniform1ui!");

            glUniform1ui = (GLUniform1ui)Marshal.GetDelegateForFunctionPointer(glUniform1uiPtr, typeof(GLUniform1ui));


            //Load glGetUniformLocation
            IntPtr glGetUniformLocationPtr;
            if ((glGetUniformLocationPtr = wglGetProcAddress("glGetUniformLocation")) == IntPtr.Zero)
                throw new Exception("Failed To Load glGetUniformLocation!");

            glGetUniformLocation = (GLGetUniformLocation)Marshal.GetDelegateForFunctionPointer(glGetUniformLocationPtr, typeof(GLGetUniformLocation));


            //Load glUniformMatrix4fv
            IntPtr glUniformMatrix4fvPtr;
            if ((glUniformMatrix4fvPtr = wglGetProcAddress("glUniformMatrix4fv")) == IntPtr.Zero)
                throw new Exception("Failed To Load glUniformMatrix4fv!");

            glUniformMatrix4fv = (GLUniformMatrix4fv)Marshal.GetDelegateForFunctionPointer(glUniformMatrix4fvPtr, typeof(GLUniformMatrix4fv));

            //Load glUniform3fv
            IntPtr glUniform3fvPtr;
            if ((glUniform3fvPtr = wglGetProcAddress("glUniform3fv")) == IntPtr.Zero)
                throw new Exception("Failed To Load glUniform3fv!");

            glUniform3fv = (GLUniform3fv)Marshal.GetDelegateForFunctionPointer(glUniform3fvPtr, typeof(GLUniform3fv));

            //Load glUniformMatrix3fv
            IntPtr glUniformMatrix3fvPtr;
            if ((glUniformMatrix3fvPtr = wglGetProcAddress("glUniformMatrix3fv")) == IntPtr.Zero)
                throw new Exception("Failed To Load glUniformMatrix3fv!");

            glUniformMatrix3fv = (GLUniformMatrix3fv)Marshal.GetDelegateForFunctionPointer(glUniformMatrix3fvPtr, typeof(GLUniformMatrix3fv));

            //Load glGetActiveUniform
            IntPtr glGetActiveUniformPtr;
            if ((glGetActiveUniformPtr = wglGetProcAddress("glGetActiveUniform")) == IntPtr.Zero)
                throw new Exception("Failed To Load glGetActiveUniform!");

            glGetActiveUniform = (GLGetActiveUniform)Marshal.GetDelegateForFunctionPointer(glGetActiveUniformPtr, typeof(GLGetActiveUniform));

            //Load glGetActiveAttrib
            IntPtr glGetActiveAttribPtr;
            if ((glGetActiveAttribPtr = wglGetProcAddress("glGetActiveAttrib")) == IntPtr.Zero)
                throw new Exception("Failed To Load glGetActiveAttrib!");

            glGetActiveAttrib = (GLGetActiveAttrib)Marshal.GetDelegateForFunctionPointer(glGetActiveAttribPtr, typeof(GLGetActiveAttrib));


            //Load glDeleteBuffers
            IntPtr glDeleteBuffersPtr;
            if ((glDeleteBuffersPtr = wglGetProcAddress("glDeleteBuffers")) == IntPtr.Zero)
                throw new Exception("Failed To Load glDeleteBuffers!");

            glDeleteBuffers = (GLDeleteBuffers)Marshal.GetDelegateForFunctionPointer(glDeleteBuffersPtr, typeof(GLDeleteBuffers));


            //Load glDeleteVertexArrays
            IntPtr glDeleteVertexArraysPtr;
            if ((glDeleteVertexArraysPtr = wglGetProcAddress("glDeleteVertexArrays")) == IntPtr.Zero)
                throw new Exception("Failed To Load glDeleteVertexArrays!");

            glDeleteVertexArrays = (GLDeleteVertexArrays)Marshal.GetDelegateForFunctionPointer(glDeleteVertexArraysPtr, typeof(GLDeleteVertexArrays));


            //Load glBindTexture
            IntPtr glBindTexturePtr;
            if ((glBindTexturePtr = wglGetProcAddress("glBindTexture")) == IntPtr.Zero)
                throw new Exception("Failed To Load glBindTexture!");

            glBindTexture = (GLBindTexture)Marshal.GetDelegateForFunctionPointer(glBindTexturePtr, typeof(GLBindTexture));

            //Load glActiveTexture
            IntPtr glActiveTexturePtr;
            if ((glActiveTexturePtr = wglGetProcAddress("glActiveTexture")) == IntPtr.Zero)
                throw new Exception("Failed To Load glActiveTexture!");

            glActiveTexture = (GLActiveTexture)Marshal.GetDelegateForFunctionPointer(glActiveTexturePtr, typeof(GLActiveTexture));

            //Load glDrawArrays
            IntPtr glDrawArraysPtr;
            if ((glDrawArraysPtr = wglGetProcAddress("glDrawArrays")) == IntPtr.Zero)
                throw new Exception("Failed To Load glDrawArrays!");

            glDrawArrays = (GLDrawArrays)Marshal.GetDelegateForFunctionPointer(glDrawArraysPtr, typeof(GLDrawArrays));

            //Load glGenFramebuffers
            IntPtr glGenFramebuffersPtr;
            if ((glGenFramebuffersPtr = wglGetProcAddress("glGenFramebuffers")) == IntPtr.Zero)
                throw new Exception("Failed To Load glGenFramebuffers!");

            glGenFramebuffers = (GLGenFramebuffers)Marshal.GetDelegateForFunctionPointer(glGenFramebuffersPtr, typeof(GLGenFramebuffers));

            //Load glBindFramebuffer
            IntPtr glBindFramebufferPtr;
            if ((glBindFramebufferPtr = wglGetProcAddress("glBindFramebuffer")) == IntPtr.Zero)
                throw new Exception("Failed To Load glBindFramebuffer!");

            glBindFramebuffer = (GLBindFramebuffer)Marshal.GetDelegateForFunctionPointer(glBindFramebufferPtr, typeof(GLBindFramebuffer));


            //Load glGenRenderbuffers
            IntPtr glGenRenderbuffersPtr;
            if ((glGenRenderbuffersPtr = wglGetProcAddress("glGenRenderbuffers")) == IntPtr.Zero)
                throw new Exception("Failed To Load glGenRenderbuffers!");

            glGenRenderbuffers = (GLGenRenderbuffers)Marshal.GetDelegateForFunctionPointer(glGenRenderbuffersPtr, typeof(GLGenRenderbuffers));

            //Load glCheckFramebufferStatus
            IntPtr glCheckFramebufferStatusPtr;
            if ((glCheckFramebufferStatusPtr = wglGetProcAddress("glCheckFramebufferStatus")) == IntPtr.Zero)
                throw new Exception("Failed To Load glCheckFramebufferStatus!");

            glCheckFramebufferStatus = (GLCheckFramebufferStatus)Marshal.GetDelegateForFunctionPointer(glCheckFramebufferStatusPtr, typeof(GLCheckFramebufferStatus));

            //Load glBindRenderbuffer
            IntPtr glBindRenderbufferPtr;
            if ((glBindRenderbufferPtr = wglGetProcAddress("glBindRenderbuffer")) == IntPtr.Zero)
                throw new Exception("Failed To Load glBindRenderbuffer!");

            glBindRenderbuffer = (GLBindRenderbuffer)Marshal.GetDelegateForFunctionPointer(glBindRenderbufferPtr, typeof(GLBindRenderbuffer));

            //Load glRenderbufferStorage
            IntPtr glRenderbufferStoragePtr;
            if ((glRenderbufferStoragePtr = wglGetProcAddress("glRenderbufferStorage")) == IntPtr.Zero)
                throw new Exception("Failed To Load glRenderbufferStorage!");

            glRenderbufferStorage = (GLRenderbufferStorage)Marshal.GetDelegateForFunctionPointer(glRenderbufferStoragePtr, typeof(GLRenderbufferStorage));

            //Load glFramebufferRenderbuffer
            IntPtr glFramebufferRenderbufferPtr;
            if ((glFramebufferRenderbufferPtr = wglGetProcAddress("glFramebufferRenderbuffer")) == IntPtr.Zero)
                throw new Exception("Failed To Load glFramebufferRenderbuffer!");

            glFramebufferRenderbuffer = (GLFramebufferRenderbuffer)Marshal.GetDelegateForFunctionPointer(glFramebufferRenderbufferPtr, typeof(GLFramebufferRenderbuffer));

            //Load glFramebufferTexture2D
            IntPtr glFramebufferTexture2DPtr;
            if ((glFramebufferTexture2DPtr = wglGetProcAddress("glFramebufferTexture2D")) == IntPtr.Zero)
                throw new Exception("Failed To Load glFramebufferTexture2D!");

            glFramebufferTexture2D = (GLFramebufferTexture2D)Marshal.GetDelegateForFunctionPointer(glFramebufferTexture2DPtr, typeof(GLFramebufferTexture2D));

            //Load glBlitFramebuffer
            IntPtr glBlitFramebufferPtr;
            if ((glBlitFramebufferPtr = wglGetProcAddress("glBlitFramebuffer")) == IntPtr.Zero)
                throw new Exception("Failed To Load glBlitFramebuffer!");

            glBlitFramebuffer = (GLBlitFramebuffer)Marshal.GetDelegateForFunctionPointer(glBlitFramebufferPtr, typeof(GLBlitFramebuffer));


            //Load glGenerateMipmap
            IntPtr glGenerateMipmapPtr;
            if ((glGenerateMipmapPtr = wglGetProcAddress("glGenerateMipmap")) == IntPtr.Zero)
                throw new Exception("Failed To Load glGenerateMipmap!");

            glGenerateMipmap = (GLGenerateMipmap)Marshal.GetDelegateForFunctionPointer(glGenerateMipmapPtr, typeof(GLGenerateMipmap));

            //Load wglSetPixelFormat
            IntPtr wglChoosePixelFormatARBPtr;
            if ((wglChoosePixelFormatARBPtr = wglGetProcAddress("wglChoosePixelFormatARB")) == IntPtr.Zero)
                throw new Exception("Failed To Load wglChoosePixelFormatARB!");

            wglChoosePixelFormatARB = (WGLChoosePixelFormatARB)Marshal.GetDelegateForFunctionPointer(wglChoosePixelFormatARBPtr, typeof(WGLChoosePixelFormatARB));

            //Load glUniform4f
            IntPtr glUniform4fPtr;
            if ((glUniform4fPtr = wglGetProcAddress("glUniform4f")) == IntPtr.Zero)
                throw new Exception("Failed To Load glUniform4f!");

            glUniform4f = (GLUniform4f)Marshal.GetDelegateForFunctionPointer(glUniform4fPtr, typeof(GLUniform4f));

            //Load glUniform4fv
            IntPtr glUniform4fvPtr;
            if ((glUniform4fvPtr = wglGetProcAddress("glUniform4fv")) == IntPtr.Zero)
                throw new Exception("Failed To Load glUniform4fv!");

            glUniform4fv = (GLUniform4fv)Marshal.GetDelegateForFunctionPointer(glUniform4fvPtr, typeof(GLUniform4fv));

            //Load glDrawBuffers
            IntPtr glDrawBuffersPtr;
            if ((glDrawBuffersPtr = wglGetProcAddress("glDrawBuffers")) == IntPtr.Zero)
                throw new Exception("Failed To Load glDrawBuffers!");

            glDrawBuffers = (GLDrawBuffers)Marshal.GetDelegateForFunctionPointer(glDrawBuffersPtr, typeof(GLDrawBuffers));

            //Load glTexImage2DMultisample
            IntPtr glTexImage2DMultisamplePtr;
            if ((glTexImage2DMultisamplePtr = wglGetProcAddress("glTexImage2DMultisample")) == IntPtr.Zero)
                throw new Exception("Failed To Load glTexImage2DMultisample!");

            glTexImage2DMultisample = (GLTexImage2DMultisample)Marshal.GetDelegateForFunctionPointer(glTexImage2DMultisamplePtr, typeof(GLTexImage2DMultisample));

            //Load glRenderbufferStorageMultisample
            IntPtr glRenderbufferStorageMultisamplePtr;
            if ((glRenderbufferStorageMultisamplePtr = wglGetProcAddress("glRenderbufferStorageMultisample")) == IntPtr.Zero)
                throw new Exception("Failed To Load glRenderbufferStorageMultisample!");

            glRenderbufferStorageMultisample = (GLRenderbufferStorageMultisample)Marshal.GetDelegateForFunctionPointer(glRenderbufferStorageMultisamplePtr, 
                typeof(GLRenderbufferStorageMultisample));

            //Load glDeleteRenderbuffers
            IntPtr glDeleteRenderbuffersPtr;
            if ((glDeleteRenderbuffersPtr = wglGetProcAddress("glDeleteRenderbuffers")) == IntPtr.Zero)
                throw new Exception("Failed To Load glDeleteRenderbuffers!");

            glDeleteRenderbuffers = (GLDeleteRenderbuffers)Marshal.GetDelegateForFunctionPointer(glDeleteRenderbuffersPtr, typeof(GLDeleteRenderbuffers));

            //Load glDeleteFramebuffers
            IntPtr glDeleteFramebuffersPtr;
            if ((glDeleteFramebuffersPtr = wglGetProcAddress("glDeleteFramebuffers")) == IntPtr.Zero)
                throw new Exception("Failed To Load glDeleteFramebuffers!");

            glDeleteFramebuffers = (GLDeleteFramebuffers)Marshal.GetDelegateForFunctionPointer(glDeleteFramebuffersPtr, typeof(GLDeleteFramebuffers));

            //Load glUniform1f
            IntPtr glUniform1fPtr;
            if ((glUniform1fPtr = wglGetProcAddress("glUniform1f")) == IntPtr.Zero)
                throw new Exception("Failed To Load glUniform1f!");

            glUniform1f = (GLUniform1f)Marshal.GetDelegateForFunctionPointer(glUniform1fPtr, typeof(GLUniform1f));
                
            //Load glTexBuffer
            IntPtr glTexBufferPtr;
            if ((glTexBufferPtr = wglGetProcAddress("glTexBuffer")) == IntPtr.Zero)
                throw new Exception("Failed To Load glTexBuffer!");

            glTexBuffer = (GLTexBuffer)Marshal.GetDelegateForFunctionPointer(glTexBufferPtr, typeof(GLTexBuffer));

            //Load glTexBufferRange
            IntPtr glTexBufferRangePtr;
            if ((glTexBufferRangePtr = wglGetProcAddress("glTexBufferRange")) == IntPtr.Zero)
                throw new Exception("Failed To Load glTexBufferRange!");

            glTexBufferRange = (GLTexBufferRange)Marshal.GetDelegateForFunctionPointer(glTexBufferRangePtr, typeof(GLTexBufferRange));

            //Load glBufferSubData
            IntPtr glBufferSubDataPtr;
            if ((glBufferSubDataPtr = wglGetProcAddress("glBufferSubData")) == IntPtr.Zero)
                throw new Exception("Failed To Load glBufferSubData!");

            glBufferSubData = (GLBufferSubData)Marshal.GetDelegateForFunctionPointer(glBufferSubDataPtr, typeof(GLBufferSubData));
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
