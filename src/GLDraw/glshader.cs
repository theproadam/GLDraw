using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using glcore.Types;

namespace glcore
{
    public unsafe class Shader : IDisposable
    {
        #region PINVOKE


        [DllImport("GLCore.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern uint CompileShaders(byte* vsData, byte* fsData, int* errorCheck, byte* errorOutput);

        [DllImport("GLCore.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int SetValue(byte* name, uint shaderProgram, int type, void* data);

        [DllImport("GLCore.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int LinkTexture(byte* name, uint shaderProgram, uint textureID, uint textureUnit);

        [DllImport("GLCore.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void GetActiveUniform(uint program, uint index, int bufSize, int* length, int* size, int* type, byte* name);

        [DllImport("GLCore.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void GetActiveAttrib(uint program, uint index, int bufSize, int* length, int* size, int* type, byte* name);


        [DllImport("GLCore.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void GetShaderConfig(uint program, int* attributes, int* uniforms);

        #endregion

        internal List<GLTexture> linkedTextures = new List<GLTexture>();
        internal List<ShaderConfig> attributes = new List<ShaderConfig>();
        internal List<ShaderConfig> uniforms = new List<ShaderConfig>();

        internal uint shaderProgram;

        internal bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Shader()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                GLGC.GCQueue.Add(new GCTarget()
                {
                    type = GCType.Shader,
                    targetShader = shaderProgram
                });

                disposed = true;
            }
        }

        public static string CompileLog = "";

        public static bool Compile(string VS, string FS, out Shader compiledShader)
        { 
            byte[] vsData = Encoding.ASCII.GetBytes(VS + "\0");
            byte[] fsData = Encoding.ASCII.GetBytes(FS + "\0");

            int errorCheck = 0;

            byte[] infolog = new byte[512];

            uint shaderProgram;

            fixed (byte* vsptr = vsData)
            {
                fixed (byte* fsptr = fsData)
                {
                    fixed (byte* err = infolog)
                    {
                        shaderProgram = CompileShaders(vsptr, fsptr, &errorCheck, err);
                    }
                }                
            }

            compiledShader = new Shader(shaderProgram);

            if (errorCheck != 0)
            {
                Shader.CompileLog = Encoding.ASCII.GetString(infolog);
                return false;
            }

            return true;

        }

        internal Shader(uint sProgram)
        {
            shaderProgram = sProgram;

            int _uniforms;
            int _attribs;

            GetShaderConfig(sProgram, &_attribs, &_uniforms);

            byte[] bufT = new byte[16];

            int length, size, type;

            for (int i = 0; i < _uniforms; i++)
            {
                fixed (byte* bptr = bufT)
                    GetActiveUniform(sProgram, (uint)i, 16, &length, &size, &type, bptr);

                uniforms.Add(new ShaderConfig((GLType)type, size, Encoding.ASCII.GetString(bufT, 0, length)));
            }

           
            for (int i = 0; i < _attribs; i++)
            {
                fixed (byte* bptr = bufT)
                    GetActiveAttrib(sProgram, (uint)i, 16, &length, &size, &type, bptr);

                attributes.Add(new ShaderConfig((GLType)type, size, Encoding.ASCII.GetString(bufT, 0, length)));
            }
            

            GLError err;
            if (GL.CheckError(out err))
                throw new Exception("Error Occured: " + err.ToString());
        }

        public void SetValue(string name, object value)
        {
            int type = (int)objectToType(value);

            GCHandle handle = GCHandle.Alloc(value, GCHandleType.Pinned);
            void* ptr = (void*)handle.AddrOfPinnedObject();

            byte[] targetName = Encoding.ASCII.GetBytes(name + "\0");

            fixed (byte* namePtr = targetName)
            {
                if (SetValue(namePtr, shaderProgram, type, ptr) == -1)
                    throw new Exception("The attribute \"" + name + "\" was not found in the shader!");
            }

            handle.Free();      
        }

        public void LinkTexture(string name, GLTexture targetTexture)
        {
            byte[] targetName = Encoding.ASCII.GetBytes(name + "\0");

            fixed (byte* namePtr = targetName)
            {
                if (LinkTexture(namePtr, shaderProgram, targetTexture.textureID, (uint)linkedTextures.Count) == -1)
                    throw new Exception("The attribute \"" + name + "\" was not found in the shader!");

                linkedTextures.Add(targetTexture);
            }
        }

        DataType objectToType(object value)
        {

            if (value.GetType() == typeof(Matrix4x4)) return DataType.mat4;
            else if (value.GetType() == typeof(Vector3)) return DataType.vec3;
            else if (value.GetType() == typeof(Vector2)) return DataType.vec2;
            else if (value.GetType() == typeof(float)) return DataType.fp32;
            else if (value.GetType() == typeof(int)) return DataType.int32;
            else if (value.GetType() == typeof(Int16)) return DataType.int2;
            else if (value.GetType() == typeof(Matrix3x3)) return DataType.mat3;

            else throw new Exception("Not a valid object Type!");


        }

        enum DataType
        {
            vec3 = 0,// = 4 * 3,
            vec2 = 1,// = 4 * 2,
            fp32 = 2,// = 4,
            int32 = 3,// = 4,
            int2 = 4,// = 4 * 2,
            byte4 = 5,// = 4,
            mat4 = 6,
            mat3 = 7,
            sampler2D = 8,
            sampler1D = 9,
            samplerCube = 10
        }

        internal static int typeToSize(GLType t)
        {
            if (t == GLType.GL_FLOAT) return 1;
            if (t == GLType.GL_FLOAT_VEC2) return 2;
            else if (t == GLType.GL_FLOAT_VEC3) return 3;
            else if (t == GLType.GL_FLOAT_VEC4) return 4;
            else throw new Exception("Not Yet Implmented!");
        }
    }

    struct ShaderConfig
    {
        public GLType type;
        public int size;
        public string name;

        public ShaderConfig(GLType t, int s, string n)
        {
            type = t;
            size = s;
            name = n;
        }
    }
}
