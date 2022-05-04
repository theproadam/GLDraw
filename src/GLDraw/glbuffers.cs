using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;

namespace glcore
{
    public unsafe class GLBuffer : IDisposable
    {
        #region PINVOKE

        [DllImport("GLCore.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern internal void CreateBufferIndices(uint* VAO, uint* VBO, uint* EBO, int vertSize, void* vertices, int indiSize, void* indices);

        [DllImport("GLCore.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern internal void CreateBuffer(uint* VAO, uint* VBO, int vertSize, void* vertices, int Stride, int attribs, int* attribSize);

        [DllImport("GLCore.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern internal void DeleteBuffer(uint VAO, uint VBO);

        #endregion

        static GLBuffer()
        {
            GLGC.Initialize();
        }

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
                GLGC.GCQueue.Add(new GCTarget() { 
                    type = GCType.Buffer,
                    targetVAO = VAO,
                    targetVBO = VBO
                });

                disposed = true;
            }
        }

        public void DeleteBuffer()
        {
            DeleteBuffer(VAO, VBO);
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

        public GLBuffer(float[] vertexData, uint[] indices, int Stride = 3)
        {
            if (vertexData == null) throw new ArgumentNullException();

            if (vertexData.Length <= 0) throw new Exception("Size must be bigger than zero!");
            if (vertexData.Length % Stride != 0) throw new Exception("Invalid Stride OR Size!");

            //gen indices->
            //uint[] indices = { 0, 1, 2, 3, 4, 5 };

            stride = Stride;
            _size = vertexData.Length * 4;

            //load data
            uint _VBO, _VAO, _EBO;

            fixed (float* vData = vertexData)
            {
                fixed (uint* iData = indices)
                {
                    CreateBufferIndices(&_VAO, &_VBO, &_EBO, vertexData.Length * 4, vData, indices.Length * 4, iData);
                }
            }


            VBO = _VBO;
            VAO = _VAO;
            EBO = _EBO;

        }

    }

    public unsafe class GLTexture : IDisposable
    {
        #region PINVOKE

        [DllImport("GLCore.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern internal void CreateTexture(uint* textureID, int width, int height, int stride, void* data);

        [DllImport("GLCore.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern internal void DeleteTexture(uint textures);

        #endregion

        static GLTexture()
        {
            GLGC.Initialize();
        }

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
               // DeleteTexture(textureID);
                disposed = true;
            }
        }

        public void DeleteTexture()
        {
            DeleteTexture(textureID);
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


            fixed (byte* b = textureData)
            {
                CreateTexture(&tID, (int)width, (int)height, sD, b);
            }

            textureID = tID;

            GLError t;
            if (GL.CheckError(out t))
                throw new Exception("Error Occured: " + t.ToString());
        }

        public GLTexture(Bitmap sourceBitmap, bool flipBlueAndRed = true)
        {
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
            

            CreateTexture(&tID, bmpWidth, bmpHeight, sD, (void*)bmpData.Scan0);
            textureID = tID;

            bitmap.UnlockBits(bmpData);

            GLError t;
            if (GL.CheckError(out t))
                throw new Exception("Error Occured: " + t.ToString());
        }

    }

    public unsafe class GLFramebuffer
    { 
        
    }
}
