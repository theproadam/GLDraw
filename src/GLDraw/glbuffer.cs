using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using glcore.gl;
using glcore.Enums;

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
                uint _VAO = VAO;
                uint _VBO = VBO;

                //crash due to GC running on another thread :(

                //GLFunc.glDeleteVertexArrays(1, &_VAO);
                //GLFunc.glDeleteBuffers(1, &_VBO);

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

    public unsafe class GLTexture : IDisposable, ITexture
    {
        private int _width;
        private int _height;

        public int Width { get { return _width; } }
        public int Height { get { return _height; } }

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
            ComputeWH();

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
            ComputeWH();

            bitmap.UnlockBits(bmpData);

            GLError t;
            if (GL.CheckError(out t))
                throw new Exception("Error Occured: " + t.ToString());
        }

        void ComputeWH()
        {
            int w, h;
            GLFunc.glGetTexLevelParameteriv(GLEnum.GL_TEXTURE_2D, 0, GLEnum.GL_TEXTURE_WIDTH, &w);
            GLFunc.glGetTexLevelParameteriv(GLEnum.GL_TEXTURE_2D, 0, GLEnum.GL_TEXTURE_HEIGHT, &h);

            _width = w;
            _height = h;
        }

        public uint GetID()
        {
            return textureID;
        }

        public uint GetMode()
        {
            return GLEnum.GL_TEXTURE_2D;
        }
    }

    public unsafe class GLTexture1D : IDisposable, ITexture
    {
        internal bool disposed = false;
        internal uint textureID;
        internal TexSpec pixelSpec;

        internal int _width;
        public int Size { get { return _width ;} }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~GLTexture1D()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {

                disposed = true;
            }
        }

        public void DeleteTexture()
        {
            uint tex = textureID;
            GLFunc.glDeleteTextures(1, &tex);
        }

        public GLTexture1D(int size, TexSpec stride)
        {
            uint tID;
            GLFunc.glGenTextures(1, &tID);
            GLFunc.glBindTexture(GLEnum.GL_TEXTURE_1D, tID);

            GLFunc.glTexParameteri(GLEnum.GL_TEXTURE_1D, GLEnum.GL_TEXTURE_MIN_FILTER, (int)GLEnum.GL_NEAREST);
            GLFunc.glTexParameteri(GLEnum.GL_TEXTURE_1D, GLEnum.GL_TEXTURE_MAG_FILTER, (int)GLEnum.GL_NEAREST);

            GLFunc.glTexImage1D(GLEnum.GL_TEXTURE_1D, 0, stride.internalFormat, size, 0, stride.format, stride.type, (void*)0);

            textureID = tID;

          
            pixelSpec = stride;

            int w;
            GLFunc.glGetTexLevelParameteriv(GLEnum.GL_TEXTURE_1D, 0, GLEnum.GL_TEXTURE_WIDTH, &w);
            _width = w;

            GLFunc.glBindTexture(GLEnum.GL_TEXTURE_1D, 0); //unbind

            GLError t;
            if (GL.CheckError(out t))
                throw new Exception("Error Occured: " + t.ToString());


        }

        public GLTexture1D(int[] data, TexSpec stride)
        {
            int size = data.Length;
            const uint GL_MAX_TEXTURE_SIZE = 0x0D33;

            if (data.Length >= GL_MAX_TEXTURE_SIZE)
                throw new Exception("Data Size Too Big!");

            uint tID;
            GLFunc.glGenTextures(1, &tID);
            GLFunc.glBindTexture(GLEnum.GL_TEXTURE_1D, tID);

            GLFunc.glTexParameteri(GLEnum.GL_TEXTURE_1D, GLEnum.GL_TEXTURE_MIN_FILTER, (int)GLEnum.GL_NEAREST);
            GLFunc.glTexParameteri(GLEnum.GL_TEXTURE_1D, GLEnum.GL_TEXTURE_MAG_FILTER, (int)GLEnum.GL_NEAREST);

            if (GL.CheckError() != GLError.GL_NO_ERROR)
                throw new Exception();


            fixed (int* iptr = data)
            {
                GLFunc.glTexImage1D(GLEnum.GL_TEXTURE_1D, 0, stride.internalFormat, size, 0, stride.format, stride.type, (void*)iptr);
            }

           

            if (GL.CheckError() != GLError.GL_NO_ERROR)
                throw new Exception();

            textureID = tID;
            pixelSpec = stride;

            int w;
            GLFunc.glGetTexLevelParameteriv(GLEnum.GL_TEXTURE_1D, 0, GLEnum.GL_TEXTURE_WIDTH, &w);
            _width = w;

            GLFunc.glBindTexture(GLEnum.GL_TEXTURE_1D, 0); //unbind

            GLError t;
            if (GL.CheckError(out t))
                throw new Exception("Error Occured: " + t.ToString());

           
        }

        public void SetPixel(int pos, object value)
        {
            GCHandle handle = GCHandle.Alloc(value, GCHandleType.Pinned);

            GLFunc.glBindTexture(GLEnum.GL_TEXTURE_1D, textureID);
            GLError ea = GL.CheckError();

            GLFunc.glTexSubImage1D(GLEnum.GL_TEXTURE_1D, 0, pos, 1, pixelSpec.format, pixelSpec.type, (void*)handle.AddrOfPinnedObject());

            GLError ea1 = GL.CheckError();

            GLFunc.glBindTexture(GLEnum.GL_TEXTURE_1D, 0); //unbind

            handle.Free();
        }

        public int[] tempPixel(int sz)
        {
            int[] asd = new int[sz];

            GLFunc.glBindTexture(GLEnum.GL_TEXTURE_1D, textureID);
            fixed (int* iptr = asd)
            {
                GLFunc.glGetTexImage(GLEnum.GL_TEXTURE_1D, 0, pixelSpec.format, pixelSpec.type, (void*)iptr);
            }

            GLFunc.glBindFramebuffer(GLEnum.GL_FRAMEBUFFER, 0);

            return asd;
        }

        public T GetPixel0<T>(int pos, int sz) where T : struct
        {
            throw new Exception("debug, not implemented");
           // T value = default(T);
           // GCHandle handle = GCHandle.Alloc(value, GCHandleType.Pinned);


            GLFunc.glBindTexture(GLEnum.GL_TEXTURE_1D, textureID);
           // GLFunc.glGetTexImage(GLEnum.GL_TEXTURE_1D, 0, pixelSpec.format, pixelSpec.type, (void*)handle.AddrOfPinnedObject());
           
            int[] asd = new int[sz];

            fixed (int* iptr = asd)
            {
                GLFunc.glGetTexImage(GLEnum.GL_TEXTURE_1D, 0, pixelSpec.format, pixelSpec.type, (void*)iptr);
            }

            GLFunc.glBindFramebuffer(GLEnum.GL_FRAMEBUFFER, 0);
          //  handle.Free();

            return default(T);
            //return value;
        }

        public uint GetID()
        {
            return textureID;
        }

        public uint GetMode()
        {
            return GLEnum.GL_TEXTURE_1D;
        }
    }

    public unsafe class GLRenderBuffer
    {
        private int _width;
        private int _height;

        public int Width { get { return _width; } }
        public int Height { get { return _height; } }

        public RBO[] SavedTypes { get { return linked.ToArray(); } }

        internal bool disposed = false;

        internal uint rbo;
        internal uint dep;

        internal RBO[] linked;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~GLRenderBuffer()
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

        public void DeleteBuffer()
        {
            GLFunc.glBindFramebuffer(GLEnum.GL_FRAMEBUFFER, rbo);

            for (int i = 0; i < linked.Length; i++)
            {
                GLFunc.glBindTexture(GLEnum.GL_TEXTURE_2D, linked[i].bufferID);
                uint val = linked[i].bufferID;
                GLFunc.glDeleteTextures(1, &val);
            }

            GLFunc.glBindRenderbuffer(GLEnum.GL_RENDERBUFFER, rbo);
            uint val0 = rbo;
            GLFunc.glDeleteRenderbuffers(1, &val0);
            GLFunc.glDeleteFramebuffers(1, &val0);

            GLError t;
            if (GL.CheckError(out t))
                throw new Exception("Error Occured: " + t.ToString());
        }

        public void Bind()
        {
            GLFunc.glBindFramebuffer(GLEnum.GL_FRAMEBUFFER, rbo);

        }

        public uint ReadPixel_UINT32(int X, int Y, int targetFBAttachment)
        {
            GLFunc.glBindFramebuffer(GLEnum.GL_FRAMEBUFFER, rbo);
            GLFunc.glBindTexture(GLEnum.GL_TEXTURE_2D, linked[targetFBAttachment].bufferID);

            uint val = 0;
            GLFunc.glReadPixels(X, Y, 1, 1, linked[targetFBAttachment].format, linked[targetFBAttachment].type, &val);

            GLFunc.glBindFramebuffer(GLEnum.GL_FRAMEBUFFER, 0);
            //Console.WriteLine("\n");
            //byte* bptr = (byte*)&val;
            //for (int i = 0; i < 4; i++)
            //    Console.WriteLine(bptr[i]);

            return val;
        }

        public int ReadPixel_INT32(int X, int Y, int targetFBAttachment)
        {
            GLFunc.glBindFramebuffer(GLEnum.GL_FRAMEBUFFER, rbo);
            GLFunc.glBindTexture(GLEnum.GL_TEXTURE_2D, linked[targetFBAttachment].bufferID);

            int val = 0;
            GLFunc.glReadPixels(X, Y, 1, 1, linked[targetFBAttachment].format, linked[targetFBAttachment].type, &val);

            GLFunc.glBindFramebuffer(GLEnum.GL_FRAMEBUFFER, 0);
            //Console.WriteLine("\n");
            //byte* bptr = (byte*)&val;
            //for (int i = 0; i < 4; i++)
            //    Console.WriteLine(bptr[i]);

            return val;
        }

        public T ReadPixel<T>(int X, int Y, int targetFBAttachment) where T : struct
        {
            T value = default(T);
            GCHandle handle = GCHandle.Alloc(value, GCHandleType.Pinned);

            GLFunc.glBindFramebuffer(GLEnum.GL_FRAMEBUFFER, rbo);
            GLFunc.glBindTexture(GLEnum.GL_TEXTURE_2D, linked[targetFBAttachment].bufferID);

            GLFunc.glReadPixels(X, Y, 1, 1, linked[targetFBAttachment].format, linked[targetFBAttachment].type, (void*)handle.AddrOfPinnedObject());

            GLFunc.glBindFramebuffer(GLEnum.GL_FRAMEBUFFER, 0);
            handle.Free();

            return value;
        }

        public GLRenderBuffer(uint width, uint height, params RBO[] types)
        {
            uint _rbo;
            uint _dep;

            linked = types; 

            if (CreateRenderBuffer(&_rbo, &_dep, types, (int)width, (int)height) == -1)
                throw new Exception("Failed To Create Framebuffer: Framebuffer not complete!");

            rbo = _rbo;
            dep = _dep;

            _width = (int)width;
            _height = (int)height;

            GLError t;
            if (GL.CheckError(out t))
                throw new Exception("Error Occured: " + t.ToString());
        }

        static int CreateRenderBuffer(uint* rbo, uint* dep, RBO[] types, int SCR_WIDTH, int SCR_HEIGHT)
        {
            const uint GL_FRAMEBUFFER = 0x8D40;
            const uint GL_COLOR_ATTACHMENT0 = 0x8CE0;

            GLFunc.glGenFramebuffers(1, rbo);
            GLFunc.glBindFramebuffer(GL_FRAMEBUFFER, *rbo);

            uint[] attachments = new uint[types.Length];

            for (int i = 0; i < types.Length; i++)
            {
                uint val;
                
                GLFunc.glGenTextures(1, &val);
                GLFunc.glBindTexture(GLEnum.GL_TEXTURE_2D, val);
                GLFunc.glTexImage2D(GLEnum.GL_TEXTURE_2D, 0, types[i].internalFormat, SCR_WIDTH, SCR_HEIGHT, 0, types[i].format, types[i].type, (void*)0);
               // GLFunc.glTexParameteri(GLEnum.GL_TEXTURE_2D, GLEnum.GL_TEXTURE_MIN_FILTER, (int)GLEnum.GL_NEAREST);
               // GLFunc.glTexParameteri(GLEnum.GL_TEXTURE_2D, GLEnum.GL_TEXTURE_MAG_FILTER, (int)GLEnum.GL_NEAREST);
                GLFunc.glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0 + (uint)i, GLEnum.GL_TEXTURE_2D, val, 0);

                //GLFunc.glBindTexture(GLEnum.GL_TEXTURE_2D_MULTISAMPLE, val);
                //GLFunc.glTexImage2DMultisample(GLEnum.GL_TEXTURE_2D_MULTISAMPLE, 16, GLEnum.GL_RGB, SCR_WIDTH, SCR_HEIGHT, 1);
                //GLFunc.glBindTexture(GLEnum.GL_TEXTURE_2D_MULTISAMPLE, 0);
                //GLFunc.glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GLEnum.GL_TEXTURE_2D_MULTISAMPLE, val, 0);

                attachments[i] = GL_COLOR_ATTACHMENT0 + (uint)i;
                types[i].bufferID = val;
            }

            fixed (uint* uiptr = attachments)
                GLFunc.glDrawBuffers(types.Length, uiptr);

            //depth now
            GLFunc.glGenRenderbuffers(1, dep);
            GLFunc.glBindRenderbuffer(GLEnum.GL_RENDERBUFFER, *dep);
            GLFunc.glRenderbufferStorage(GLEnum.GL_RENDERBUFFER, GLEnum.GL_DEPTH_COMPONENT, SCR_WIDTH, SCR_HEIGHT);
            GLFunc.glFramebufferRenderbuffer(GLEnum.GL_FRAMEBUFFER, GLEnum.GL_DEPTH_ATTACHMENT, GLEnum.GL_RENDERBUFFER, *dep);
            
            //GLFunc.glGenRenderbuffers(1, rbo);
            //GLFunc.glBindRenderbuffer(GLEnum.GL_RENDERBUFFER, *rbo);
            //GLFunc.glRenderbufferStorageMultisample(GLEnum.GL_RENDERBUFFER, 16, GLEnum.GL_DEPTH_COMPONENT, SCR_WIDTH, SCR_HEIGHT);
            //GLFunc.glBindRenderbuffer(GLEnum.GL_RENDERBUFFER, 0);
            //GLFunc.glFramebufferRenderbuffer(GLEnum.GL_FRAMEBUFFER, GLEnum.GL_DEPTH_ATTACHMENT, GLEnum.GL_RENDERBUFFER, *rbo);


            if (GLFunc.glCheckFramebufferStatus(GLEnum.GL_FRAMEBUFFER) != GLEnum.GL_FRAMEBUFFER_COMPLETE)
                return -1;

            //bind default
            GLFunc.glBindFramebuffer(GL_FRAMEBUFFER, 0);

            return 1;
        }

        public void BlitIntoDefault()
        {
            GLFunc.glBindFramebuffer(GLEnum.GL_READ_FRAMEBUFFER, rbo);
            GLFunc.glBindFramebuffer(GLEnum.GL_DRAW_FRAMEBUFFER, 0);

            GLFunc.glBlitFramebuffer(0, 0, _width, _height, 0, 0, _width, _height, GLEnum.GL_COLOR_BUFFER_BIT |  GLEnum.GL_DEPTH_BUFFER_BIT, GLEnum.GL_NEAREST);
            GLFunc.glBindFramebuffer(GLEnum.GL_FRAMEBUFFER, 0);
        }

        public void Resize(int newWidth, int newHeight)
        {
            throw new NotImplementedException("Not supported. Please Call DeleteBuffer() and re-create the object.");

            if (newWidth <= 0 || newHeight <= 0)
                throw new Exception("Invalid Framebuffer Size!");

            GLFunc.glBindFramebuffer(GLEnum.GL_FRAMEBUFFER, rbo);

            for (int i = 0; i < linked.Length; i++)
            {
                GLFunc.glBindTexture(GLEnum.GL_TEXTURE_2D_MULTISAMPLE, linked[i].bufferID);
                GLFunc.glTexImage2DMultisample(GLEnum.GL_TEXTURE_2D_MULTISAMPLE, 16, GLEnum.GL_RGB, newWidth, newHeight, 1);
            }

         //   GLFunc.glDeleteTextures(1, )

            GLFunc.glBindRenderbuffer(GLEnum.GL_RENDERBUFFER, rbo);
            GLFunc.glRenderbufferStorageMultisample(GLEnum.GL_RENDERBUFFER, 16, GLEnum.GL_DEPTH_COMPONENT, newWidth, newHeight);
            GLFunc.glBindFramebuffer(GLEnum.GL_FRAMEBUFFER, 0);
        }
    }

    public struct RBO
    {
        public int internalFormat;
        public uint format;
        public uint type;

        internal uint bufferID;

        public RBO(GL_Enum internalFormat, GL_Enum format, GL_Enum type)
        {
            this.internalFormat = (int)internalFormat;
            this.format = (uint)format;
            this.type = (uint)type;
            bufferID = 0;
        }
    }

    public struct TexSpec
    { 
       public int internalFormat;
       public uint format;
       public uint type;

       public TexSpec(GL_Enum internalFormat, GL_Enum format, GL_Enum type)
       {
           this.internalFormat = (int)internalFormat;
           this.format = (uint)format;
           this.type = (uint)type;
       }
    }

    public unsafe class GLCubemap : ITexture
    {
        internal bool disposed = false;
        public uint textureID;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~GLCubemap()
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

        public GLCubemap(string px, string nx, string py, string ny, string pz, string nz, bool flipBlueAndRed = true)
        {
            uint tID;
            const uint GL_TEXTURE_CUBE_MAP = 0x8513;
            const uint GL_TEXTURE_CUBE_MAP_POSITIVE_X = 0x8515;
            const int GL_RGB = 0x1907;
            const uint GL_UNSIGNED_BYTE = 0x1401;

            Bitmap[] data = { new Bitmap(px), new Bitmap(nx), new Bitmap(py), new Bitmap(ny), new Bitmap(pz), new Bitmap(nz) };

            GLFunc.glGenTextures(1, &tID);
            GLFunc.glBindTexture(GL_TEXTURE_CUBE_MAP, tID);

            for (int i = 0; i < 6; i++)
            {
                int bmpWidth = data[i].Width, bmpHeight = data[i].Height;
                BitmapData bmpData = data[i].LockBits(new Rectangle(0, 0, bmpWidth, bmpHeight), ImageLockMode.ReadWrite, data[i].PixelFormat);
                int sD = Image.GetPixelFormatSize(data[i].PixelFormat) / 8;

                if (!(sD == 4 || sD == 3))
                    throw new Exception("not yet supported!");

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

                //GLFunc.glTexImage2D(GL_TEXTURE_CUBE_MAP_POSITIVE_X + 0, 0, GL_RGB, bmpWidth, bmpHeight, 0, GL_RGB, GL_UNSIGNED_BYTE, (void*)bptr);

                GLFunc.glTexImage2D(GL_TEXTURE_CUBE_MAP_POSITIVE_X + (uint)i, 0, sD == 4 ? 0x1908 : 0x1907, 
                    bmpWidth, bmpHeight, 0, sD == 4 ? (uint)0x1908 : (uint)0x1907, 0x1401, (void*)bptr);

                data[i].UnlockBits(bmpData);
            }

            const uint GL_TEXTURE_MAG_FILTER = 0x2800;
            const uint GL_TEXTURE_MIN_FILTER = 0x2801;
            const int GL_LINEAR = 0x2601;

            GLFunc.glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
            GLFunc.glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
            GLFunc.glTexParameteri(GL_TEXTURE_CUBE_MAP, 0x2802, 0x812F);
            GLFunc.glTexParameteri(GL_TEXTURE_CUBE_MAP, 0x2803, 0x812F);
            GLFunc.glTexParameteri(GL_TEXTURE_CUBE_MAP, 0x8072, 0x812F);


            GLError t;
            if (GL.CheckError(out t))
                throw new Exception("Error Occured: " + t.ToString());
            textureID = tID;
        }

        public uint GetID()
        {
            return textureID;
        }

        public uint GetMode()
        {
            const uint GL_TEXTURE_CUBEMAP = 0x8513;
            return GL_TEXTURE_CUBEMAP;
        }
    }

    public unsafe class GLBufferTexture : ITexture
    { 
        internal bool disposed = false;
        internal uint textureID;
        internal uint vID;

        internal uint _iformat;

        internal int _width;
        public int Size { get { return _width ;} }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~GLBufferTexture()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {

                disposed = true;
            }
        }

        public void DeleteTexture()
        {
            uint tex = textureID, vbo = vID;
            GLFunc.glDeleteTextures(1, &tex);
            GLFunc.glDeleteBuffers(1, &vbo);
        }

        int oldSize;

        public GLBufferTexture(int[] data, uint internalFormat)
        {
            int size = data.Length;
            uint tbo, tex;
            GLFunc.glGenBuffers(1, &tbo);
            GLFunc.glBindBuffer(GLEnum.GL_TEXTURE_BUFFER, tbo);

            GLFunc.glGenTextures(1, &tex);
            GLFunc.glBindTexture(GLEnum.GL_TEXTURE_BUFFER, tex);

            GLFunc.glTexBuffer(GLEnum.GL_TEXTURE_BUFFER, internalFormat, tbo);
            GLFunc.glBindTexture(GLEnum.GL_TEXTURE_BUFFER, 0);

            GLFunc.glBindBuffer(GLEnum.GL_TEXTURE_BUFFER, tbo);
            GLFunc.glBufferData(GLEnum.GL_TEXTURE_BUFFER, size * 4, (void*)0, GLEnum.GL_DYNAMIC_DRAW);

            textureID = tex;
            vID = tbo;
            _iformat = internalFormat;
            oldSize = data.Length;

            fixed (int* iptr = data)
            {
                GLFunc.glBufferSubData(GLEnum.GL_TEXTURE_BUFFER, (IntPtr)0, (IntPtr)(size * 4), iptr);
            }

            GLError t;
            if (GL.CheckError(out t))
                throw new Exception("Error Occured: " + t.ToString());
        }

        public void Update(int[] newData)
        {
            if (newData.Length != oldSize)
                throw new Exception("To Resize a buffer please delete and make a new buffer!");

            GLFunc.glBindTexture(GLEnum.GL_TEXTURE_BUFFER, textureID);
            GLFunc.glBindBuffer(GLEnum.GL_TEXTURE_BUFFER, vID);

            fixed (int* iptr = newData)
            {
                GLFunc.glBufferSubData(GLEnum.GL_TEXTURE_BUFFER, (IntPtr)0, (IntPtr)(newData.Length * 4), iptr);
            }
        }

        public void SetPixel(int pos, object value)
        {
            GCHandle handle = GCHandle.Alloc(value, GCHandleType.Pinned);

            GLFunc.glBindTexture(GLEnum.GL_TEXTURE_1D, textureID);
            GLError ea = GL.CheckError();

          //  GLFunc.glTexSubImage1D(GLEnum.GL_TEXTURE_1D, 0, pos, 1, pixelSpec.format, pixelSpec.type, (void*)handle.AddrOfPinnedObject());

            GLError ea1 = GL.CheckError();

            GLFunc.glBindTexture(GLEnum.GL_TEXTURE_1D, 0); //unbind

            handle.Free();
        }

        public uint GetID()
        {
            return textureID;
        }

        public uint GetMode()
        {
            return GLEnum.GL_TEXTURE_BUFFER;
        }

    }
}
