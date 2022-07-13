using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using glcore.gl;

namespace glcore
{
    public unsafe class GLBuffer : IDisposable
    {
       internal int _size;

        public int Size { get { return _size; } }
        public int Stride { get { return stride; } }

        internal bool disposed = false;
        internal int stride;

        internal uint VBO, VAO, EBO;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~GLBuffer()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                //dispose here
                //GLGC.GCQueue.Add(new GCTarget()
                //{
                //    type = GCType.Buffer,
                //    targetVAO = VAO,
                //    targetVBO = VBO
                //});

                disposed = true;
            }
        }

        public void DeleteBuffer()
        {
          //  DeleteBuffer(VAO, VBO);

            uint _VAO = VAO, _VBO = VBO;

            GLFunc.glDeleteVertexArrays(1, &_VAO);
            GLFunc.glDeleteBuffers(1, &_VBO);

            disposed = true;
        }

        public GLBuffer(float[] vertexData, params Type[] attributeTypes)
        {
            if (vertexData == null) throw new ArgumentNullException();
            if (vertexData.Length <= 0) throw new Exception("Size must be bigger than zero!");

            if (attributeTypes.Length == 0)
                throw new Exception("Please specify the attribute input Types!");

            int[] attribConfig = new int[attributeTypes.Length];

            int Stride = 0;
            for (int i = 0; i < attributeTypes.Length; i++)
            {
                int a_size = Marshal.SizeOf(attributeTypes[i]) / 4;
                Stride += a_size;
                attribConfig[i] = a_size;
            }

            if (vertexData.Length % Stride != 0) throw new Exception("Invalid Stride OR Size!");

            stride = Stride;
            _size = vertexData.Length * 4;

            //load data
            uint _VBO, _VAO;

            fixed (float* vData = vertexData)
            {
                fixed (int* iData = attribConfig)
                {
                    CreateBuffer(&_VAO, &_VBO, vertexData.Length * 4, vData, Stride, attributeTypes.Length, iData);
                }
            }

            //WARNING RISKY !!!
            VBO = _VBO;
            VAO = _VAO;



            //DeleteBuffer(&_VAO, &_VBO);
            //   throw new Exception();


        }

        public GLBuffer(float[] vertexData, Shader targetShader)
        {
            if (vertexData == null) throw new ArgumentNullException();
            if (vertexData.Length <= 0) throw new Exception("Size must be bigger than zero!");

            int[] attribConfig = new int[targetShader.attributes.Count];

            int Stride = 0;
            for (int i = 0; i < attribConfig.Length; i++)
            {
                int a_size = Shader.typeToSize(targetShader.attributes[i].type);
                Stride += a_size;
                attribConfig[i] = a_size;
            }

            if (vertexData.Length % Stride != 0) throw new Exception("Invalid Stride OR Size! Please make sure you vertex data aligns with the shader attributes!");

            stride = Stride;
            _size = vertexData.Length * 4;

            //load data
            uint _VBO, _VAO;

            fixed (float* vData = vertexData)
            {
                fixed (int* iData = attribConfig)
                {
                    CreateBuffer(&_VAO, &_VBO, vertexData.Length * 4, vData, Stride, attribConfig.Length, iData);
                }
            }

            //WARNING RISKY !!! [on second thought it should be fine]
            VBO = _VBO;
            VAO = _VAO;
        }

        static void CreateBuffer(uint* VAO, uint* VBO, int vertSize, void* vertices, int Stride, int attribs, int* attribSize)
        {
            const uint GL_ARRAY_BUFFER = 0x8892;
            const uint GL_STATIC_DRAW = 0x88E4;
            const uint GL_FLOAT = 0x1406;

            GLFunc.glGenVertexArrays(1, VAO);
            GLFunc.glGenBuffers(1, VBO);
            // bind the Vertex Array Object first, then bind and set vertex buffer(s), and then configure vertex attributes(s).
            GLFunc.glBindVertexArray(*VAO);

            GLFunc.glBindBuffer(GL_ARRAY_BUFFER, *VBO);
            GLFunc.glBufferData(GL_ARRAY_BUFFER, vertSize, vertices, GL_STATIC_DRAW);

            int offset = 0;

            for (int i = 0; i < attribs; i++)
            {
                GLFunc.glVertexAttribPointer((uint)i, attribSize[i], GL_FLOAT, (byte)0, Stride * sizeof(float), (void*)(offset * sizeof(float)));
                GLFunc.glEnableVertexAttribArray((uint)i);
                offset += attribSize[i];
            }


            //glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 3 * sizeof(float), (void*)0);
            //glEnableVertexAttribArray(0);

            // note that this is allowed, the call to glVertexAttribPointer registered VBO as the vertex attribute's bound vertex buffer object so afterwards we can safely unbind
            GLFunc.glBindBuffer(GL_ARRAY_BUFFER, 0);


            // remember: do NOT unbind the EBO while a VAO is active as the bound element buffer object IS stored in the VAO; keep the EBO bound.
            //glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, 0);

            // You can unbind the VAO afterwards so other VAO calls won't accidentally modify this VAO, but this rarely happens. Modifying other
            // VAOs requires a call to glBindVertexArray anyways so we generally don't unbind VAOs (nor VBOs) when it's not directly necessary.
            GLFunc.glBindVertexArray(0);
        }
    }

    public unsafe class GLTexture : IDisposable
    {
        private int _width;
        private int _height;
        private int _stride;

        public int Width { get { return _width; } }
        public int Height { get { return _height; } }
        public int Stride { get { return _stride; } }

        internal bool disposed = false;
        internal uint textureID;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~GLTexture()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                //GLGC.GCQueue.Add(new GCTarget()
                //{
                //    type = GCType.Texture,
                //    targetTexture = textureID
                //});

                disposed = true;
            }
        }

        public void DeleteTexture()
        {
            uint tex = textureID;
            GLFunc.glDeleteTextures(1, &tex);
        }

        public GLTexture(byte[] textureData, uint width, uint height)
        {
            uint tID;

            if (textureData.Length % width != 0)
                throw new Exception("TextureData Invalid Size!");

            if (textureData.Length % height != 0)
                throw new Exception("TextureData Invalid Size!");

            if (textureData.Length % (width * height) != 0)
                throw new Exception("TextureData Invalid Size!");

            int sD = (int)(textureData.Length % (width * height));

            _width = (int)width;
            _height = (int)height;

            fixed (byte* b = textureData)
            {
                CreateTexture(&tID, (int)width, (int)height, sD, b);
            }

            textureID = tID;

            GLError t;
            if (GL.CheckError(out t))
                throw new Exception("Error Occured: " + t.ToString());
        }

        static void CreateTexture(uint* textureID, int width, int height, int stride, void* data)
        {
            const uint GL_TEXTURE_2D = 0x0DE1;
            const uint GL_TEXTURE_MAG_FILTER = 0x2800;
            const uint GL_TEXTURE_MIN_FILTER = 0x2801;
            const uint GL_TEXTURE_WRAP_S = 0x2802;
            const uint GL_TEXTURE_WRAP_T = 0x2803;
            const uint GL_CLAMP = 0x2900;
            const uint GL_REPEAT = 0x2901;

            GLFunc.glGenTextures(1, textureID);
            GLFunc.glBindTexture(GL_TEXTURE_2D, *textureID);

            GLFunc.glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, 0x2901);
            GLFunc.glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, 0x2901);

            GLFunc.glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, 0x2601);
            GLFunc.glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, 0x2601);

            GLFunc.glTexImage2D(GL_TEXTURE_2D, 0, stride == 4 ? 0x1908 : 0x1907, width, height, 0, stride == 4 ? (uint)0x1908 : (uint)0x1907, 0x1401, data);
            GLFunc.glGenerateMipmap(GL_TEXTURE_2D);
        }

        public GLTexture(Bitmap sourceBitmap, bool flipBlueAndRed = true)
        {
            //strip the bitmap away from any stupid formats
            Bitmap bitmap = new Bitmap(sourceBitmap);

            int sD = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
            int bmpWidth = bitmap.Width, bmpHeight = bitmap.Height;

            if (!(sD == 4 || sD == 3))
                throw new Exception("not yet supported!");

            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bmpWidth, bmpHeight), ImageLockMode.ReadWrite, bitmap.PixelFormat);

            uint tID;

            byte* bptr = (byte*)bmpData.Scan0;

            if (flipBlueAndRed)
            {
                for (int h = 0; h < bmpHeight; h++)
                {
                    for (int w = 0; w < bmpWidth; w++)
                    {
                        byte* target = bptr + h * bmpWidth * sD + sD * w;
                        byte temp = target[0];
                        target[0] = target[2];
                        target[2] = temp;

                    }
                }
            }

            _width = bmpWidth;
            _height = bmpHeight;

            CreateTexture(&tID, bmpWidth, bmpHeight, sD, (void*)bmpData.Scan0);
            textureID = tID;

            bitmap.UnlockBits(bmpData);

            GLError t;
            if (GL.CheckError(out t))
                throw new Exception("Error Occured: " + t.ToString());
        }
    }

    public unsafe class GLFramebuffer : IDisposable
    {
        private int _width;
        private int _height;
        private int _stride;

        public int Width { get { return _width; } }
        public int Height { get { return _height; } }
        public int Stride { get { return _stride; } }

        internal bool disposed = false;

        internal uint fbo;
        internal uint rbo;
        internal uint tex;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~GLFramebuffer()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                // DeleteTexture(textureID);
                disposed = true;
            }
        }

        public GLFramebuffer(uint width, uint height)
        {
            uint _fbo;
            uint _rbo;
            uint _tex;

            if (CreateFrameBuffer(&_rbo, &_fbo, &_tex, width, height) == -1)
                throw new Exception("Failed To Create Framebuffer: Framebuffer not complete!");

            fbo = _fbo;
            rbo = _rbo;
            tex = _tex;

            _width = (int)width;
            _height = (int)height;

            GLError t;
            if (GL.CheckError(out t))
                throw new Exception("Error Occured: " + t.ToString());
        }

        public void Bind()
        {
            //GL.ChangeFrameBufferTexture(tex);
            //GL.ChangeCurrentFrameBuffer(fbo);
        }

        public void CopyInto(GLFramebuffer dest)
        {
            if (_width != dest._width)
                throw new Exception("Widths are not the same!");

            if (_height != dest._height)
                throw new Exception("Heights are not the same!");

           // CopyFrameBuffer(fbo, dest.fbo, tex, dest.tex, _width, _height);
        }

        public void CopyInto(BlitData defaultFramebuffer)
        {
            defaultFramebuffer.MakeCurrent();
           //CopyFrameBuffer(fbo, 0, tex, 0, _width, _height);
        }

        static int CreateFrameBuffer(uint* rbo, uint* framebuffer, uint* texColBuf, uint width, uint height)
        {
            const uint GL_FRAMEBUFFER = 0x8D40;
            const uint GL_RENDERBUFFER = 0x8D41;
            const uint GL_DEPTH24_STENCIL8 = 0x88F0;
            const uint GL_DEPTH_STENCIL_ATTACHMENT = 0x821A;
            const uint GL_FRAMEBUFFER_COMPLETE = 0x8CD5;
            const uint GL_COLOR_ATTACHMENT0 = 0x8CE0;

            const uint GL_NEAREST = 0x2600;
            const uint GL_LINEAR = 0x2601;

            GLFunc.glGenFramebuffers(1, framebuffer);
            GLFunc.glBindFramebuffer(GL_FRAMEBUFFER, *framebuffer);


            //GLFunc.glGenTextures(1, texColBuf);
            //GLFunc.glBindTexture(GL_TEXTURE_2D, *texColBuf);
            //GLFunc.glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB, width, height, 0, GL_RGB, GL_UNSIGNED_BYTE, NULL);
            //GLFunc.glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
            //GLFunc.glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);

            //GLFunc.glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_2D, *texColBuf, 0);


            GLFunc.glGenRenderbuffers(1, rbo);
            GLFunc.glBindRenderbuffer(GL_RENDERBUFFER, *rbo);
            GLFunc.glRenderbufferStorage(GL_RENDERBUFFER, GL_DEPTH24_STENCIL8, (int)width, (int)height);
            GLFunc.glFramebufferRenderbuffer(GL_FRAMEBUFFER, GL_DEPTH_STENCIL_ATTACHMENT, GL_RENDERBUFFER, *rbo);


            // now that we actually created the framebuffer and added all attachments we want to check if it is actually complete now
            if (GLFunc.glCheckFramebufferStatus(GL_FRAMEBUFFER) != GL_FRAMEBUFFER_COMPLETE)
            {
                return -1;
            }

            //switch back to default framebuffer!
            GLFunc.glBindFramebuffer(GL_FRAMEBUFFER, 0);

            return 0;
        }
    }
}
