using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using glcore.Types;
using glcore.gl;

using System.Threading;

namespace glcore
{
    public unsafe class Shader : IDisposable
    {
        internal List<GLTexture> linkedTextures = new List<GLTexture>();
        internal List<GLFramebuffer> linkedFramebuffers = new List<GLFramebuffer>();
        internal List<GLCubemap> linkedCubemaps = new List<GLCubemap>();


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

        static Shader()
        {
            //if (!GL.contextReady)
            //    throw new Exception("Please create a context before using the shader class!");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                //GLFunc.glDeleteShader(shaderProgram);

                //GLError err;
                //if (GL.CheckError(out err))
                //    throw new Exception("GLDraw Error: " + err);

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

            if (errorCheck != 0 || shaderProgram == 0 || GL.CheckError() != GLError.GL_NO_ERROR)
            {
                Shader.CompileLog = Encoding.ASCII.GetString(infolog);
                compiledShader = null;
                return false;
            }

            compiledShader = new Shader(shaderProgram);


            return true;

        }

        internal Shader(uint sProgram)
        {
            shaderProgram = sProgram;

            int _uniforms;
            int _attribs;

            GetShaderConfig(sProgram, &_attribs, &_uniforms);

            byte[] bufT = new byte[16];

            int size, length;
            uint type;

            for (int i = 0; i < _uniforms; i++)
            {
                fixed (byte* bptr = bufT)
                 //   GetActiveUniform(sProgram, (uint)i, 16, &length, &size, &type, bptr);
                    GLFunc.glGetActiveUniform(sProgram, (uint)i, 16, &length, &size, &type, bptr);

                uniforms.Add(new ShaderConfig((GLType)type, size, Encoding.ASCII.GetString(bufT, 0, (int)length)));
            }


            for (int i = 0; i < _attribs; i++)
            {
                fixed (byte* bptr = bufT)
                   // GetActiveAttrib(sProgram, (uint)i, 16, &length, &size, &type, bptr);
                    GLFunc.glGetActiveAttrib(sProgram, (uint)i, 16, &length, &size, &type, bptr);

                attributes.Add(new ShaderConfig((GLType)type, size, Encoding.ASCII.GetString(bufT, 0, (int)length)));
            }


            GLError err;
            if (GL.CheckError(out err))
                throw new Exception("Error Occured: " + err.ToString());
        }

        public void SetValue(string name, object value)
        {
            GCHandle handle = GCHandle.Alloc(value, GCHandleType.Pinned);
            void* ptr = (void*)handle.AddrOfPinnedObject();

            byte[] targetName = Encoding.ASCII.GetBytes(name + "\0");

            fixed (byte* namePtr = targetName)
            {
                if (SetValue(namePtr, shaderProgram, value, ptr) == -1)
                    throw new Exception("The attribute \"" + name + "\" was not found in the shader!");
            }

            handle.Free();
        }

        public void LinkTexture(string name, GLTexture targetTexture)
        {
            byte[] targetName = Encoding.ASCII.GetBytes(name + "\0");
            int count = linkedFramebuffers.Count + linkedCubemaps.Count + linkedTextures.Count;
            
            fixed (byte* namePtr = targetName)
            {
                if (LinkTexture(namePtr, shaderProgram, targetTexture.textureID, (uint)count) == -1)
                    throw new Exception("The attribute \"" + name + "\" was not found in the shader!");

                linkedTextures.Add(targetTexture);
            }
        }

        public void LinkCubemap(string name, GLCubemap targetCubemap)
        {
            byte[] targetName = Encoding.ASCII.GetBytes(name + "\0");
            int count = linkedFramebuffers.Count + linkedCubemaps.Count + linkedTextures.Count;

            fixed (byte* namePtr = targetName)
            {
                if (LinkTexture(namePtr, shaderProgram, targetCubemap.textureID, (uint)count) == -1)
                    throw new Exception("The attribute \"" + name + "\" was not found in the shader!");

                linkedCubemaps.Add(targetCubemap);
            }
        }

        public void LinkTexture(string name, GLFramebuffer framebufferTexture)
        {
            byte[] targetName = Encoding.ASCII.GetBytes(name + "\0");
            int count = linkedFramebuffers.Count + linkedCubemaps.Count + linkedTextures.Count;

            fixed (byte* namePtr = targetName)
            {
                if (LinkTexture(namePtr, shaderProgram, framebufferTexture.tex, (uint)count) == -1)
                    throw new Exception("The attribute \"" + name + "\" was not found in the shader!");

                //    linkedTextures.Add(targetTexture);
                linkedFramebuffers.Add(framebufferTexture);
            }
        }

        static uint CompileShaders(byte* vsData, byte* fsData, int* errorCheck, byte* infoLog)
        {
            const uint GL_FRAGMENT_SHADER = 0x8B30;
            const uint GL_VERTEX_SHADER = 0x8B31;
            const uint GL_COMPILE_STATUS = 0x8B81;
            const uint GL_LINK_STATUS = 0x8B82;
            int* NULL = (int*)0;

            byte* vD = (byte*)vsData;
            byte* fD = (byte*)fsData;

            uint vertexShader = GLFunc.glCreateShader(GL_VERTEX_SHADER);

            GLFunc.glShaderSource(vertexShader, 1, &vD, NULL);
            GLFunc.glCompileShader(vertexShader);




            // check for shader compile errors
            int success;
            GLFunc.glGetShaderiv(vertexShader, GL_COMPILE_STATUS, &success);
            if (success == 0)
            {
                GLFunc.glGetShaderInfoLog(vertexShader, 512, NULL, infoLog);
                *errorCheck = 1;
                return 0;
            }



            // fragment shader
            uint fragmentShader = GLFunc.glCreateShader(GL_FRAGMENT_SHADER);
            GLFunc.glShaderSource(fragmentShader, 1, &fD, NULL);
            GLFunc.glCompileShader(fragmentShader);
            // check for shader compile errors
            GLFunc.glGetShaderiv(fragmentShader, GL_COMPILE_STATUS, &success);
            if (success == 0)
            {
                GLFunc.glGetShaderInfoLog(fragmentShader, 512, NULL, infoLog);
                *errorCheck = 2;
                return 0;
            }

            // link shaders
            uint shaderProgram = GLFunc.glCreateProgram();
            GLFunc.glAttachShader(shaderProgram, vertexShader);
            GLFunc.glAttachShader(shaderProgram, fragmentShader);
            GLFunc.glLinkProgram(shaderProgram);
            // check for linking errors
            GLFunc.glGetProgramiv(shaderProgram, GL_LINK_STATUS, &success);
            if (success == 0)
            {
                GLFunc.glGetProgramInfoLog(shaderProgram, 512, NULL, infoLog);
                *errorCheck = 3;
                return 0;
            }

            GLFunc.glDeleteShader(vertexShader);
            GLFunc.glDeleteShader(fragmentShader);

            return shaderProgram;
        }

        static void GetShaderConfig(uint program, int* attributes, int* uniforms)
        {
            GLFunc.glUseProgram(program);

            const uint GL_ACTIVE_ATTRIBUTES = 0x8B89;
            const uint GL_ACTIVE_UNIFORMS = 0x8B86;

            GLFunc.glGetProgramiv(program, GL_ACTIVE_ATTRIBUTES, attributes);
            GLFunc.glGetProgramiv(program, GL_ACTIVE_UNIFORMS, uniforms);
        }

        static int LinkTexture(byte* name, uint shaderProgram, uint textureID, uint textureUnit)
	    {
            //WARNING THIS TEXTURE UNIT SETUP IS NOT CORRECT!!!!
		    GLFunc.glUseProgram(shaderProgram);
            int location = GLFunc.glGetUniformLocation(shaderProgram, name);

            //cast to int valid?
            GLFunc.glUniform1i(location, (int)textureUnit);
		
		    return location;
	    }

        static int SetValue(byte* name, uint shaderProgram, object type, void* data)
	    {
		    //probably shouldnt be doing this each time
		    GLFunc.glUseProgram(shaderProgram);

		    int location = GLFunc.glGetUniformLocation(shaderProgram, name);
		    //DataType t = (DataType)type;
            Type t = type.GetType();

            const byte FALSE = 0;

            if (t == typeof(Matrix4x4))
            {
                GLFunc.glUniformMatrix4fv(location, 1, FALSE, (float*)data);
            }
            else if (t == typeof(Matrix3x3))
            {
                GLFunc.glUniformMatrix3fv(location, 1, FALSE, (float*)data);
            }
            else if (t == typeof(Vector3))
            {
                GLFunc.glUniform3fv(location, 1, (float*)data);
            }
            else if (t == typeof(int))
            {
                GLFunc.glUniform1i(location, *(int*)data);
            }
            else if (t == typeof(Vector4))
            {
                GLFunc.glUniform4fv(location, 1, (float*)data);
            }
            else if (t == typeof(uint))
            {
                GLFunc.glUniform1ui(location, *(uint*)data);
            }
            else throw new Exception("not implemented!");

		    return location;
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
}
