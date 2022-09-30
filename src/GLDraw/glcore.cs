using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using glcore.gl;
using glcore.Types;
using glcore.Enums;

namespace glcore
{
    public partial class GL
    {
        [DllImport("gdi32.dll")]
        internal static extern int SwapBuffers(IntPtr HDC);

        public static void Initialize()
        {
            //initialize dummy context
            Form dummyForm = new Form();
            BlitData tempData = new BlitData(dummyForm);

            //load ogl functions
            GLFunc.LoadFunctions();

            
            GLFunc.glEnable(GLEnum.GL_DEPTH_TEST);
        }

        public static void Clear(float r, float b, float g, float a)
        {
            GLFunc.glClearColor(r, g, b, a);
            GLFunc.glClear((uint)GLClear.GL_COLOR_BUFFER_BIT | (uint)GLClear.GL_DEPTH_BUFFER_BIT);
        }

        public static void ClearDepth()
        {
            GLFunc.glClear((uint)GLClear.GL_DEPTH_BUFFER_BIT);
        }

        public unsafe static void Draw(GLBuffer buffer, Shader shader)
        {
            GLFunc.glUseProgram(shader.shaderProgram);

            uint o = 0;
            for (int i = 0; i < shader.linkedTextures.Count; i++)
            {
                GLFunc.glActiveTexture(GLEnum.GL_TEXTURE0 + (uint)i); //glActiveTexture(GL_TEXTURE0);
                GLFunc.glBindTexture(shader.linkedTextures[i].texture.GetMode(), shader.linkedTextures[i].texture.GetID());
                
                o++;
            }

          //  uint o1 = 0;
            uint o1 = o;

            for (int i = 0; i < shader.linkedRBO.Count; i++)
            {
                for (int j = 0; j < shader.linkedRBO[i].linked.Length; j++)
                {
                    GLFunc.glActiveTexture(GLEnum.GL_TEXTURE0 + o1++);
                    GLFunc.glBindTexture(GLEnum.GL_TEXTURE_2D, shader.linkedRBO[i].linked[j].bufferID);
                }
            }

            const uint GL_TRIANGLES = 0x0004;
            
            GLFunc.glBindVertexArray(buffer.VAO);
            GLFunc.glDrawArrays(GL_TRIANGLES, 0, (int)((buffer._size / 4) / buffer.stride));
          //  Draw(shader.shaderProgram, 0, (uint)((buffer._size / 4) / buffer.stride), buffer.VAO);

        }


        public static void Blit(BlitData bData)
        {
            SwapBuffers(bData.TargetDC);
        }

        public static GLError CheckError()
        {
            return (GLError)GLFunc.glGetError();
        }

        public static bool CheckError(out GLError type)
        {
            type = (GLError)GLFunc.glGetError();
            return type != GLError.GL_NO_ERROR;
        }
    }

    public enum GLClear
    {
        GL_DEPTH_BUFFER_BIT = 0x00000100,
        GL_STENCIL_BUFFER_BIT = 0x00000400,
        GL_COLOR_BUFFER_BIT = 0x00004000
    }

    public partial class BlitData : IDisposable
    {
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
                GLFunc.ReleaseDC(LinkedHandle, TargetDC);
                GLFunc.wglDeleteContext(m_hglrc);
                disposed = true;
            }
        }

        public unsafe BlitData(Form TargetForm)
        {
            IntPtr OutputHandle = TargetForm.Handle;

            LinkedHandle = OutputHandle;
            TargetDC = GLFunc.GetDC(OutputHandle);

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

            if ((iPixelFormat = GLFunc.ChoosePixelFormat(TargetDC, ref pfd)) == 0)
            {
                throw new Exception("ChoosePixelFormat Failed");
            }

            // make that match the device context's current pixel format 
            if (GLFunc.SetPixelFormat(TargetDC, iPixelFormat, ref pfd) == 0)
            {
                throw new Exception("SetPixelFormat Failed");
            }

            if ((m_hglrc = GLFunc.wglCreateContext(TargetDC)) == IntPtr.Zero)
            {
                throw new Exception("wglCreateContext Failed");
            }

            if ((GLFunc.wglMakeCurrent(TargetDC, m_hglrc)) == IntPtr.Zero)
            {
                throw new Exception("wglMakeCurrent Failed");
            }

          
        }

        public unsafe BlitData(object target)
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
            TargetDC = GLFunc.GetDC(handle);

            PixelFormatDescriptor pfd = new PixelFormatDescriptor()
            {
                Size = (ushort)sizeof(PixelFormatDescriptor),
                Version = 1,
                Flags = 0x00000004 | 0x00000020 | 0x00000001, //PFD WINDOW, OPENGL, DBUFFER
                PixelType = 0, //RGBA
                ColorBits = 32,
                DepthBits = 24, //32bit depth
                LayerType = 0 //PFD_MAIN_PLANE
            };

            int iPixelFormat;

            if ((iPixelFormat = GLFunc.ChoosePixelFormat(TargetDC, ref pfd)) == 0)
            {
                throw new Exception("ChoosePixelFormat Failed");
            }

            // make that match the device context's current pixel format 
            if (GLFunc.SetPixelFormat(TargetDC, iPixelFormat, ref pfd) == 0)
            {
                throw new Exception("SetPixelFormat Failed");
            }

            if ((m_hglrc = GLFunc.wglCreateContext(TargetDC)) == IntPtr.Zero)
            {
                throw new Exception("wglCreateContext Failed");
            }

            if ((GLFunc.wglMakeCurrent(TargetDC, m_hglrc)) == IntPtr.Zero)
            {
                throw new Exception("wglMakeCurrent Failed");
            }


        }


        public void MakeCurrent()
        {
            if (shared_m_hglrc != IntPtr.Zero)
            {
                if ((GLFunc.wglMakeCurrent(TargetDC, shared_m_hglrc)) == IntPtr.Zero)
                {
                    throw new Exception("wglMakeCurrent Failed");
                }
            }
            else
                if ((GLFunc.wglMakeCurrent(TargetDC, m_hglrc)) == IntPtr.Zero)
                {
                    throw new Exception("wglMakeCurrent Failed");
                }
        }

        public void Resize(int width, int height)
        {
            GLFunc.glFlush();

            MakeCurrent();

            GLFunc.glFlush();

            GLFunc.glViewport(0, 0, width, height);

            GLFunc.glFlush();
        }
    }

    public unsafe partial class BlitData : IDisposable
    {
        public BlitData(object target, bool enableAA)
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
            TargetDC = GLFunc.GetDC(handle);


            int GL_TRUE = 1;
            int GL_FALSE = 0;

            int[] piAttribIList = new int[] { 
                (int)WGL_ARB.WGL_DRAW_TO_WINDOW_ARB, GL_TRUE,
                (int)WGL_ARB.WGL_SUPPORT_OPENGL_ARB, GL_TRUE,
                (int)WGL_ARB.WGL_COLOR_BITS_ARB, 32,
                (int)WGL_ARB.WGL_RED_BITS_ARB, 8,
                (int)WGL_ARB.WGL_GREEN_BITS_ARB, 8,
                (int)WGL_ARB.WGL_BLUE_BITS_ARB, 8,
                (int)WGL_ARB.WGL_ALPHA_BITS_ARB, 8,
                (int)WGL_ARB.WGL_DEPTH_BITS_ARB, 24,
                (int)WGL_ARB.WGL_STENCIL_BITS_ARB, 8,
                (int)WGL_ARB.WGL_DOUBLE_BUFFER_ARB, GL_TRUE,
                (int)WGL_ARB.WGL_PIXEL_TYPE_ARB, (int)WGL_ARB.WGL_TYPE_RGBA_ARB,
                (int)WGL_ARB.WGL_SAMPLE_BUFFERS_ARB, GL_TRUE,
                (int)WGL_ARB.WGL_SAMPLES_ARB, 16,
                0, 0 };

            if (!enableAA)
                piAttribIList = new int[]
                {
                    (int)WGL_ARB.WGL_DRAW_TO_WINDOW_ARB, GL_TRUE,
                    (int)WGL_ARB.WGL_SUPPORT_OPENGL_ARB, GL_TRUE,
                    (int)WGL_ARB.WGL_DOUBLE_BUFFER_ARB, GL_TRUE,
                    (int)WGL_ARB.WGL_PIXEL_TYPE_ARB, (int)WGL_ARB.WGL_TYPE_RGBA_ARB,
                    (int)WGL_ARB.WGL_COLOR_BITS_ARB, 32,
                    (int)WGL_ARB.WGL_DEPTH_BITS_ARB, 32,
                    (int)WGL_ARB.WGL_STENCIL_BITS_ARB, 0,
                    0, // End
                };

            int pixelFormat;
            uint nNumFormats;

            bool result;

            fixed (int* iptr = piAttribIList)
            {
                //result = wglSetPixelFormat((void*)TargetDC, iptr, fptr, 1, &pixelFormat, &nNumFormats);
                result = GLFunc.wglChoosePixelFormatARB((void*)TargetDC, iptr, (float*)0, 1, &pixelFormat, &nNumFormats);
            }

            if (!result)
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
            if (GLFunc.SetPixelFormat(TargetDC, pixelFormat, ref pfd) == 0)
            {
                throw new Exception("SetPixelFormat Failed");
            }

            if ((m_hglrc = GLFunc.wglCreateContext(TargetDC)) == IntPtr.Zero)
            {
                throw new Exception("wglCreateContext Failed");
            }

            if ((GLFunc.wglMakeCurrent(TargetDC, m_hglrc)) == IntPtr.Zero)
            {
                throw new Exception("wglMakeCurrent Failed");
            }

           // GL.glEnable(0x0B71);
           // GL.glEnable(0x809D);

        }



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
            TargetDC = GLFunc.GetDC(handle);


            int GL_TRUE = 1;
            int GL_FALSE = 0;

            int[] piAttribIList = wglConfig;

            //int[] piAttribIList = new int[]
            //{
            //    (int)WGL_ARB.WGL_DRAW_TO_WINDOW_ARB, GL_TRUE,
            //    (int)WGL_ARB.WGL_SUPPORT_OPENGL_ARB, GL_TRUE,
            //    (int)WGL_ARB.WGL_DOUBLE_BUFFER_ARB, GL_TRUE,
            //    (int)WGL_ARB.WGL_PIXEL_TYPE_ARB, (int)WGL_ARB.WGL_TYPE_RGBA_ARB,
            //    (int)WGL_ARB.WGL_COLOR_BITS_ARB, 32,
            //    (int)WGL_ARB.WGL_DEPTH_BITS_ARB, 32,
            //    (int)WGL_ARB.WGL_STENCIL_BITS_ARB, 0,
            //    0, // End
            //};

            float[] fList = new float[2];

            int pixelFormat;
            uint nNumFormats;

            bool result;

            fixed (int* iptr = piAttribIList)
            {
                fixed (float* fptr = fList)
                {
                    //result = wglSetPixelFormat((void*)TargetDC, iptr, fptr, 1, &pixelFormat, &nNumFormats);
                    result = GLFunc.wglChoosePixelFormatARB((void*)TargetDC, iptr, (float*)0, 1, &pixelFormat, &nNumFormats);
                }
            }

            if (!result)
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
            if (GLFunc.SetPixelFormat(TargetDC, pixelFormat, ref pfd) == 0)
            {
                throw new Exception("SetPixelFormat Failed");
            }

            if ((m_hglrc = GLFunc.wglCreateContext(TargetDC)) == IntPtr.Zero)
            {
                throw new Exception("wglCreateContext Failed");
            }

            if ((GLFunc.wglMakeCurrent(TargetDC, m_hglrc)) == IntPtr.Zero)
            {
                throw new Exception("wglMakeCurrent Failed");
            }

            // GL.glEnable(0x0B71);
            // GL.glEnable(0x809D);

        }


        public void Bind()
        {
            GLFunc.glBindFramebuffer(GLEnum.GL_FRAMEBUFFER, 0);
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
    }

}
