using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace glcore.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector3
    {
        public float x;
        public float y;
        public float z;
        /// <summary>
        /// Creates a new Vector3
        /// </summary>
        /// <param name="posX">X Value</param>
        /// <param name="posY">Y Value</param>
        /// <param name="posZ">Z Value</param>
        public Vector3(float posX, float posY, float posZ)
        {
            x = posX;
            y = posY;
            z = posZ;
        }

        /// <summary>
        /// Calculates the 3 dimensional distance between point A and Point B
        /// </summary>
        /// <param name="From">Point A</param>
        /// <param name="To">Point B</param>
        /// <returns></returns>
        public static float Distance(Vector3 From, Vector3 To)
        {
            return (float)Math.Sqrt(Math.Pow(From.x - To.x, 2) + Math.Pow(From.y - To.y, 2) + Math.Pow(From.z - To.z, 2));
        }
        /// <summary>
        /// Adds two Vector3 together
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static Vector3 operator +(Vector3 A, Vector3 B)
        {
            return new Vector3(A.x + B.x, A.y + B.y, A.z + B.z);
        }
        /// <summary>
        /// Substacts Vector B from Vector A
        /// </summary>
        /// <param name="A">Vector A</param>
        /// <param name="B">Vector B</param>
        /// <returns></returns>
        public static Vector3 operator -(Vector3 A, Vector3 B)
        {
            return new Vector3(A.x - B.x, A.y - B.y, A.z - B.z);
        }

        public static Vector3 operator -(float A, Vector3 B)
        {
            return new Vector3(A - B.x, A - B.y, A - B.z);
        }

        public static Vector3 operator -(Vector3 A, float B)
        {
            return new Vector3(A.x - B, A.y - B, A.z - B);
        }

        public static bool Compare(Vector3 A, Vector3 B)
        {
            return (A.x == B.x && A.y == B.y && A.z == B.z);
        }

        public static Vector3 operator *(Vector3 A, Vector3 B)
        {
            return new Vector3(A.x * B.x, A.y * B.y, A.z * B.z);
        }

        public static Vector3 operator *(Vector3 A, float B)
        {
            return new Vector3(A.x * B, A.y * B, A.z * B);
        }

        public static Vector3 operator *(float A, Vector3 B)
        {
            return new Vector3(A * B.x, A * B.y, A * B.z);
        }

        public static bool operator >(Vector3 A, float B)
        {
            return A.x > B & A.y > B & A.z > B;
        }

        public static bool operator <(Vector3 A, float B)
        {
            return A.x < B & A.y < B & A.z < B;
        }

        public static Vector3 Slerp(Vector3 start, Vector3 end, float percent)
        {
            // Dot product - the cosine of the angle between 2 vectors.
            float dot = Vector3.Dot(start, end);

            // Clamp it to be in the range of Acos()
            // This may be unnecessary, but floating point
            // precision can be a fickle mistress.
            //Math.Clamp(dot, -1.0f, 1.0f);
            if (dot < -1.0f) dot = -1.0f;
            else if (dot > 1.0f) dot = 1.0f;

            // Acos(dot) returns the angle between start and end,
            // And multiplying that by percent returns the angle between
            // start and the final result.
            float theta = (float)Math.Acos(dot) * percent;
            Vector3 RelativeVec = end - start * dot;
            RelativeVec = Vector3.Normalize(RelativeVec);

            // Orthonormal basis
            // The final result.
            return ((start * (float)Math.Cos(theta)) + (RelativeVec * (float)Math.Sin(theta)));
        }

        public float sqrMagnitude()
        {
            return x * x + y * y + z * z;
        }

        public static Vector3 zero { get { return new Vector3(0, 0, 0); } }

        public void Clamp01()
        {
            if (x < 0) x = 0;
            if (x > 1) x = 1;

            if (y < 0) y = 0;
            if (y > 1) y = 1;

            if (z < 0) z = 0;
            if (z > 1) z = 1;
        }

        public Vector3 Abs()
        {
            return new Vector3(Math.Abs(x), Math.Abs(y), Math.Abs(z));
        }

        public static Vector3 LerpAngle(Vector3 a, Vector3 b, float t)
        {
            return new Vector3(Lerp1D(a.x, b.x, t), Lerp1D(a.y, b.y, t), Lerp1D(a.z, b.z, t));
        }

        static float Lerp1D(float a, float b, float t)
        {
            float val = Repeat(b - a, 360);
            if (val > 180f)
                val -= 360f;

            return a + val * Clamp01(t);
        }

        static float Repeat(float t, float length)
        {
            return Clamp(t - (float)Math.Floor(t / length) * length, 0f, length);
        }

        public Vector3 Repeat(float length)
        {
            float x1 = Clamp(x - (float)Math.Floor(x / length) * length, 0f, length);
            float y1 = Clamp(y - (float)Math.Floor(y / length) * length, 0f, length);
            float z1 = Clamp(z - (float)Math.Floor(z / length) * length, 0f, length);

            if (x1 > 180f) x1 -= 360f;
            if (y1 > 180f) y1 -= 360f;
            if (z1 > 180f) z1 -= 360f;

            return new Vector3(x1, y1, z1);
        }

        static float Clamp(float v, float min, float max)
        {
            if (v > max) return max;
            else if (v < min) return min;
            else return v;
        }

        static float Clamp01(float v)
        {
            if (v < 0) return 0;
            if (v > 1) return 1;
            else return v;
        }

        public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
        {
            if (t > 1) t = 1;
            else if (t < 0) t = 0;
            return new Vector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
        }

        /// <summary>
        /// Calculates the squared 3 dimensional distance between point A and Point B
        /// </summary>
        /// <param name="From">Point A</param>
        /// <param name="To">Point B</param>
        /// <returns></returns>
        public static float DistanceSquared(Vector3 From, Vector3 To)
        {
            return (float)(Math.Pow(From.x - To.x, 2) + Math.Pow(From.y - To.y, 2) + Math.Pow(From.z - To.z, 2));
        }


        public static bool operator ==(Vector3 A, Vector3 B)
        {
            return A.x == B.x && A.y == B.y && A.z == B.z;
        }

        public static bool operator !=(Vector3 A, Vector3 B)
        {
            return A.x != B.x || A.y != B.y || A.z != B.z;
        }


        public static Vector3 operator -(Vector3 A)
        {
            return new Vector3(-A.x, -A.y, -A.z);
        }

        public static Vector3 operator /(Vector3 a, float d)
        {
            return new Vector3(a.x / d, a.y / d, a.z / d);
        }

        public static float Magnitude(Vector3 vector)
        {
            return (float)Math.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
        }

        public float Magnitude()
        {
            return (float)Math.Sqrt(x * x + y * y + z * z);
        }

        public static Vector3 Cross(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(
                lhs.y * rhs.z - lhs.z * rhs.y,
                lhs.z * rhs.x - lhs.x * rhs.z,
                lhs.x * rhs.y - lhs.y * rhs.x);
        }

        const float EPSILON = 1E-3f;
        public bool isApproximately(Vector3 CompareTo)
        {
            return Math.Abs(CompareTo.x - x) < EPSILON && Math.Abs(CompareTo.y - y) < EPSILON && Math.Abs(CompareTo.z - z) < EPSILON;
        }

        const float EPSILONM = 1E-4f;
        public bool isApproximatelyM(Vector3 CompareTo)
        {
            return Math.Abs(CompareTo.x - x) < EPSILONM && Math.Abs(CompareTo.y - y) < EPSILONM && Math.Abs(CompareTo.z - z) < EPSILONM;
        }

        const float EPSILONT = 1E-5f;
        public bool isApproximatelyT(Vector3 CompareTo)
        {
            return Math.Abs(CompareTo.x - x) < EPSILONT && Math.Abs(CompareTo.y - y) < EPSILONT && Math.Abs(CompareTo.z - z) < EPSILONT;
        }

        const float EPSILONL = 5E-3f;
        public bool isApproximatelyL(Vector3 CompareTo)
        {
            return Math.Abs(CompareTo.x - x) < EPSILONL && Math.Abs(CompareTo.y - y) < EPSILONL && Math.Abs(CompareTo.z - z) < EPSILONL;
        }

        const float EPSILONLL = 1E-2f;
        public bool isApproximatelyLL(Vector3 CompareTo)
        {
            return Math.Abs(CompareTo.x - x) < EPSILONLL && Math.Abs(CompareTo.y - y) < EPSILONLL && Math.Abs(CompareTo.z - z) < EPSILONLL;
        }

        const float EPSILONLLL = 5E-1f;
        public bool isApproximatelyLLL(Vector3 CompareTo)
        {
            return Math.Abs(CompareTo.x - x) < EPSILONLLL && Math.Abs(CompareTo.y - y) < EPSILONLLL && Math.Abs(CompareTo.z - z) < EPSILONLLL;
        }

        const float EPSILONAE = 5E-7f;
        public bool isApproximatelyAE(Vector3 CompareTo)
        {
            return Math.Abs(CompareTo.x - x) < EPSILONAE && Math.Abs(CompareTo.y - y) < EPSILONAE && Math.Abs(CompareTo.z - z) < EPSILONAE;
        }


        /// <summary>
        /// Returns a string in the format of "Vector3 X: " + X + ", Y: " + Y + ", Z: " + Z
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "X: " + x.ToString() + ", Y: " + y.ToString() + ", Z:" + z.ToString();
        }

        public static Vector3 Reflect(Vector3 inDirection, Vector3 inNormal)
        {
            return -2f * Dot(inNormal, inDirection) * inNormal + inDirection;
        }

        public static float Dot(Vector3 lhs, Vector3 rhs)
        {
            return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
        }

        public static Vector3 Normalize(Vector3 value)
        {
            float num = Magnitude(value);
            if (num > 1E-05f)
            {
                return value / num;
            }
            return new Vector3(0, 0, 0);
        }

        public static Vector3 Max(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(Math.Max(lhs.x, rhs.x), Math.Max(lhs.y, rhs.y), Math.Max(lhs.z, rhs.z));
        }

        public static Vector3 Sin(Vector3 AngleDegrees)
        {
            return new Vector3((float)Math.Sin(AngleDegrees.x * (Math.PI / 180f)), (float)Math.Sin(AngleDegrees.y * (Math.PI / 180f)), (float)Math.Sin(AngleDegrees.z * (Math.PI / 180f)));
        }

        public static Vector3 Cos(Vector3 AngleDegrees)
        {
            return new Vector3((float)Math.Cos(AngleDegrees.x * (Math.PI / 180f)), (float)Math.Cos(AngleDegrees.y * (Math.PI / 180f)), (float)Math.Cos(AngleDegrees.z * (Math.PI / 180f)));
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Vector2
    {
        public float x;
        public float y;

        public Vector2(float posX, float posY)
        {
            x = posX;
            y = posY;
        }

        public Vector2(Vector2 oldVector2)
        {
            x = oldVector2.x;
            y = oldVector2.y;
        }

        public static Vector2 operator +(Vector2 A, Vector2 B)
        {
            return new Vector2(A.x + B.x, A.y + B.y);
        }

        public static Vector2 operator -(Vector2 A, Vector2 B)
        {
            return new Vector2(A.x - B.x, A.y - B.y);
        }

        public static float Distance(Vector2 From, Vector2 To)
        {
            return (float)Math.Sqrt(Math.Pow(From.x - To.x, 2) + Math.Pow(From.y - To.y, 2));
        }

        public override string ToString()
        {
            return "Vector2 X: " + x.ToString() + ", Y: " + y.ToString();
        }

    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Matrix4x4
    {
        public float X0Y0;
        public float X0Y1;
        public float X0Y2;
        public float X0Y3;

        public float X1Y0;
        public float X1Y1;
        public float X1Y2;
        public float X1Y3;

        public float X2Y0;
        public float X2Y1;
        public float X2Y2;
        public float X2Y3;

        public float X3Y0;
        public float X3Y1;
        public float X3Y2;
        public float X3Y3;

        public static Matrix4x4 Identity { get { return IdentityMatrix(); } }

        public static Matrix4x4 IdentityMatrix()
        {
            Matrix4x4 mat = new Matrix4x4();
            mat.SetIdentityMatrix();
            return mat;
        }

        public static implicit operator Matrix4x4(float[,] input)
        {
            if (input.GetLength(0) != 4 || input.GetLength(1) != 4)
                throw new Exception("Invalid Matrix Size!");

            throw new Exception("");

            Matrix4x4 reslt = new Matrix4x4();

            reslt[0, 0] = input[0, 0];
            reslt[1, 0] = input[1, 0];
            reslt[2, 0] = input[2, 0];
            reslt[3, 0] = input[3, 0];

            reslt[0, 1] = input[0, 1];
            reslt[1, 1] = input[1, 1];
            reslt[2, 1] = input[2, 1];
            reslt[3, 1] = input[3, 1];

            reslt[0, 2] = input[0, 2];
            reslt[1, 2] = input[1, 2];
            reslt[2, 2] = input[2, 2];
            reslt[3, 2] = input[3, 2];

            reslt[0, 3] = input[0, 3];
            reslt[1, 3] = input[1, 3];
            reslt[2, 3] = input[2, 3];
            reslt[3, 3] = input[3, 3];

            return reslt;
        }

        /// <summary>
        /// Memory Layout Goes Left Right, Next Row; Left right; Next Row (Opposite of OpenGL)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static implicit operator Matrix4x4(float[] input)
        {
            if (input.Length != 16)
                throw new Exception("Invalid Matrix Size!");

            Matrix4x4 reslt = new Matrix4x4();

            float* fptr = (float*)&reslt;

            fixed (float* f = input)
                for (int i = 0; i < 16; i++)
                    fptr[i] = f[i];

            return reslt;
        }

        public void SetZeroMatrix()
        {
            fixed (Matrix4x4* mat4 = &this)
            {
                float* ptr = (float*)mat4;

                for (int i = 0; i < 16; i++)
                    ptr[i] = 0;
            }
        }

        public void SetIdentityMatrix()
        {
            fixed (Matrix4x4* mat4 = &this)
            {
                float* ptr = (float*)mat4;

                ptr[0] = 1;
                ptr[1] = 0;
                ptr[2] = 0;
                ptr[3] = 0;

                ptr[4] = 0;
                ptr[5] = 1;
                ptr[6] = 0;
                ptr[7] = 0;

                ptr[8] = 0;
                ptr[9] = 0;
                ptr[10] = 1;
                ptr[11] = 0;

                ptr[12] = 0;
                ptr[13] = 0;
                ptr[14] = 0;
                ptr[15] = 1;

            }
        }

        /// <summary>
        /// Arrays start at zero. 
        /// </summary>
        /// <param name="COLUMN (X)"></param>
        /// <param name="ROW (Y)"></param>
        /// <returns></returns>
        public float this[int X, int Y]
        {
            get
            {
                if (Y < 0 || Y > 3) throw new Exception("Not in a valid Y!");
                if (X < 0 || X > 3) throw new Exception("Not in a valid X!");

                float val = 0;

                fixed (Matrix4x4* mat4 = &this)
                {
                    float* ptr = (float*)mat4;
                    val = ptr[X + Y * 4];
                }

                return val;
            }
            set
            {
                if (Y < 0 || Y > 3) throw new Exception("Not in a valid Y!");
                if (X < 0 || X > 3) throw new Exception("Not in a valid X!");

                fixed (Matrix4x4* mat4 = &this)
                {
                    float* ptr = (float*)mat4;
                    ptr[X + Y * 4] = value;
                }
            }
        }

        public Matrix4x4(bool makeIdentityMatrix)
        {
            fixed (Matrix4x4* mat4 = &this)
            {
                //tell the compiler to screw off
            }

            if (makeIdentityMatrix)
                this.SetIdentityMatrix();
            else
                this.SetZeroMatrix();
        }

        public Matrix4x4(Matrix3x3 mat3)
        {
            X0Y0 = mat3.X0Y0;
            X1Y0 = mat3.X1Y0;
            X2Y0 = mat3.X2Y0;
            X3Y0 = 0;

            X0Y1 = mat3.X0Y1;
            X1Y1 = mat3.X1Y1;
            X2Y1 = mat3.X2Y1;
            X3Y1 = 0;

            X0Y2 = mat3.X0Y2;
            X1Y2 = mat3.X1Y2;
            X2Y2 = mat3.X2Y2;
            X3Y2 = 0;

            X0Y3 = 0;
            X1Y3 = 0;
            X2Y3 = 0;
            X3Y3 = 1;
        }

        /// <summary>
        /// Create Camera Rotation where EulerAngles are yaw (z axis) * pitch (y axis) * roll (x axis)
        /// </summary>
        /// <param name="EulerAngles"></param>
        /// <returns></returns>
        public static Matrix4x4 RotationMatrix(Vector3 EulerAngles)
        {
            Matrix4x4 result = new Matrix4x4();
            const float deg2rad = (float)(Math.PI / 180d);

            float cosa = (float)Math.Cos(deg2rad * EulerAngles.z);
            float cosb = (float)Math.Cos(deg2rad * EulerAngles.y);
            float cosy = (float)Math.Cos(deg2rad * EulerAngles.x);

            float sina = (float)Math.Sin(deg2rad * EulerAngles.z);
            float sinb = (float)Math.Sin(deg2rad * EulerAngles.y);
            float siny = (float)Math.Sin(deg2rad * EulerAngles.x);

            result.X0Y0 = cosa * cosb;
            result.X1Y0 = cosa * sinb * siny - sina * cosy;
            result.X2Y0 = cosa * sinb * cosy + sina * siny;
            result.X3Y0 = 0;

            result.X0Y1 = sina * cosb;
            result.X1Y1 = sina * sinb * siny + cosa * cosy;
            result.X2Y1 = sina * sinb * cosy - cosa * siny;
            result.X3Y1 = 0;

            result.X0Y2 = -sinb;
            result.X1Y2 = cosb * siny;
            result.X2Y2 = cosb * cosy;
            result.X3Y2 = 0;

            result.X0Y3 = 0;
            result.X1Y3 = 0;
            result.X2Y3 = 0;
            result.X3Y3 = 1;

            return result;
        }

        public static Matrix4x4 PerspectiveMatrix(float hFOV, float vFOV, float zNear = 0.1f, float zFar = 1000f)
        {
            Matrix4x4 proj = new Matrix4x4();
            const float deg2rad = (float)(Math.PI / 180d);

            float r = zNear * (float)Math.Tan(deg2rad * vFOV / 2f), l = -r;
            float t = zNear * (float)Math.Tan(deg2rad * hFOV / 2f), b = -t;


            proj.X0Y0 = 2 * zNear / (r - l);
            proj.X1Y0 = 0;
            proj.X2Y0 = (r + l) / (r - l);
            proj.X3Y0 = 0;

            proj.X0Y1 = 0;
            proj.X1Y1 = 2 * zNear / (t - b);
            proj.X2Y1 = (t + b) / (t - b);
            proj.X3Y1 = 0;

            proj.X0Y2 = 0;
            proj.X1Y2 = 0;
            proj.X2Y2 = -(zFar + zNear) / (zFar - zNear);
            proj.X3Y2 = -2 * zNear * zFar / (zFar - zNear);

            proj.X0Y3 = 0;
            proj.X1Y3 = 0;
            proj.X2Y3 = -1;
            proj.X3Y3 = 0;

            //reverse Z
            proj.X2Y0 = -proj.X2Y0;
            proj.X2Y1 = -proj.X2Y1;
            proj.X2Y2 = -proj.X2Y2;
            proj.X2Y3 = -proj.X2Y3;

            //scale bias

            //  proj.X2Y2 = -0.0003f;
            //   proj.X3Y2 = -0.300009f;

            return proj;
        }

        public static Matrix4x4 PerspectiveMatrix(float hFOV, int viewportWidth, int viewportHeight, float zNear = 0.1f, float zFar = 1000f)
        {
            float aspect = (float)viewportWidth / (float)viewportHeight;

            //FOV mod uses vFOV, using 1/aspect we can reverse the calculation
            Matrix4x4 proj = PerspectiveMatrix(hFOV, FOVMod(hFOV, 1.0f / aspect), zNear, zFar);

            return proj;
        }

        public static float FOVMod(float FOV, float aspectRatio)
        {
            const float deg2rads = (float)(Math.PI / 180d);
            return 2.0f * (float)Math.Atan(Math.Tan(FOV * 0.5f * deg2rads) / aspectRatio) / deg2rads;
        }

        public static Matrix4x4 OrthographicMatrix(float width, float height, float zNear = 0.1f, float zFar = 1000f)
        {
            Matrix4x4 result = new Matrix4x4();

            result.X0Y0 = 2f / width;
            result.X1Y1 = 2f / height;
            result.X2Y2 = -2f / (zFar - zNear);
            result.X3Y3 = 1;

            result.X3Y2 = -(zFar + zNear) / (zFar - zNear);

            //reverse Z
            result.X2Y0 = -result.X2Y0;
            result.X2Y1 = -result.X2Y1;
            result.X2Y2 = -result.X2Y2;
            result.X2Y3 = -result.X2Y3;

            return result;

            return OrthographicMatrix(width / 2f, -width / 2f, height / 2f, -height / 2f, zNear, zFar);
        }

        public static Matrix4x4 OrthographicMatrix(float r, float l, float t, float b, float n, float f)
        {
            Matrix4x4 result = new Matrix4x4();
           // result.SetZeroMatrix();

            result.X0Y0 = 2.0f / (r - l);
            result.X1Y1 = 2.0f / (t - b);
            result.X2Y2 = -2.0f / (f - n);
            result.X3Y3 = 1.0f;

            result.X3Y0 = -(r + l) / (r - l);
            result.X3Y1 = -(t + b) / (t - b);
           // result.X3Y2 = 1f;
          //  result.X3Y2 = -(f + n) / (f - n);

            //result.X0Y0 = 1.0f / r;
            //result.X1Y1 = 1.0f / t;
            //result.X2Y2 = -2.0f * (f - n);
            //result.X3Y3 = 1.0f;
            //result.X3Y2 = (f + n) / (f - n);



            result.X2Y0 = -result.X2Y0;
            result.X2Y1 = -result.X2Y1;
            result.X2Y2 = -result.X2Y2;
            result.X2Y3 = -result.X2Y3;

            return result;
        }

        public static Matrix4x4 OrthographicMatrix(float size, int viewportWidth, int viewportHeight, float zNear = 0.1f, float zFar = 1000f)
        {
            float aspectRatio = (float)viewportWidth / (float)viewportHeight;
        //    Console.WriteLine("VP -> w: " + size + ", h: " + size / aspectRatio);
            return OrthographicMatrix(size, size / aspectRatio, zNear, zFar);
        }

        public static Matrix4x4 Lerp(Matrix4x4 A, Matrix4x4 B, float C)
        {
            float mC = 1f - C;

            return new Matrix4x4() { 
                X0Y0 = mC * A.X0Y0 + C * B.X0Y0,
                X1Y0 = mC * A.X1Y0 + C * B.X1Y0,
                X2Y0 = mC * A.X2Y0 + C * B.X2Y0,
                X3Y0 = mC * A.X3Y0 + C * B.X3Y0,

                X0Y1 = mC * A.X0Y1 + C * B.X0Y1,
                X1Y1 = mC * A.X1Y1 + C * B.X1Y1,
                X2Y1 = mC * A.X2Y1 + C * B.X2Y1,
                X3Y1 = mC * A.X3Y1 + C * B.X3Y1,

                X0Y2 = mC * A.X0Y2 + C * B.X0Y2,
                X1Y2 = mC * A.X1Y2 + C * B.X1Y2,
                X2Y2 = mC * A.X2Y2 + C * B.X2Y2,
                X3Y2 = mC * A.X3Y2 + C * B.X3Y2,

                X0Y3 = mC * A.X0Y3 + C * B.X0Y3,
                X1Y3 = mC * A.X1Y3 + C * B.X1Y3,
                X2Y3 = mC * A.X2Y3 + C * B.X2Y3,
                X3Y3 = mC * A.X3Y3 + C * B.X3Y3
            };
        }

        public static bool Inverse(Matrix4x4 input, out Matrix4x4 mat4)
        { 
            //ripped from GLU
            float[] inv = new float[16];

            Matrix4x4 mat = input.Transposed(); ;
            float* m = (float*)&mat;

            inv[0] = m[5] * m[10] * m[15] -
                     m[5] * m[11] * m[14] -
                     m[9] * m[6] * m[15] +
                     m[9] * m[7] * m[14] +
                     m[13] * m[6] * m[11] -
                     m[13] * m[7] * m[10];

            inv[4] = -m[4] * m[10] * m[15] +
                      m[4] * m[11] * m[14] +
                      m[8] * m[6] * m[15] -
                      m[8] * m[7] * m[14] -
                      m[12] * m[6] * m[11] +
                      m[12] * m[7] * m[10];

            inv[8] = m[4] * m[9] * m[15] -
                     m[4] * m[11] * m[13] -
                     m[8] * m[5] * m[15] +
                     m[8] * m[7] * m[13] +
                     m[12] * m[5] * m[11] -
                     m[12] * m[7] * m[9];

            inv[12] = -m[4] * m[9] * m[14] +
                       m[4] * m[10] * m[13] +
                       m[8] * m[5] * m[14] -
                       m[8] * m[6] * m[13] -
                       m[12] * m[5] * m[10] +
                       m[12] * m[6] * m[9];

            inv[1] = -m[1] * m[10] * m[15] +
                      m[1] * m[11] * m[14] +
                      m[9] * m[2] * m[15] -
                      m[9] * m[3] * m[14] -
                      m[13] * m[2] * m[11] +
                      m[13] * m[3] * m[10];

            inv[5] = m[0] * m[10] * m[15] -
                     m[0] * m[11] * m[14] -
                     m[8] * m[2] * m[15] +
                     m[8] * m[3] * m[14] +
                     m[12] * m[2] * m[11] -
                     m[12] * m[3] * m[10];

            inv[9] = -m[0] * m[9] * m[15] +
                      m[0] * m[11] * m[13] +
                      m[8] * m[1] * m[15] -
                      m[8] * m[3] * m[13] -
                      m[12] * m[1] * m[11] +
                      m[12] * m[3] * m[9];

            inv[13] = m[0] * m[9] * m[14] -
                      m[0] * m[10] * m[13] -
                      m[8] * m[1] * m[14] +
                      m[8] * m[2] * m[13] +
                      m[12] * m[1] * m[10] -
                      m[12] * m[2] * m[9];

            inv[2] = m[1] * m[6] * m[15] -
                     m[1] * m[7] * m[14] -
                     m[5] * m[2] * m[15] +
                     m[5] * m[3] * m[14] +
                     m[13] * m[2] * m[7] -
                     m[13] * m[3] * m[6];

            inv[6] = -m[0] * m[6] * m[15] +
                      m[0] * m[7] * m[14] +
                      m[4] * m[2] * m[15] -
                      m[4] * m[3] * m[14] -
                      m[12] * m[2] * m[7] +
                      m[12] * m[3] * m[6];

            inv[10] = m[0] * m[5] * m[15] -
                      m[0] * m[7] * m[13] -
                      m[4] * m[1] * m[15] +
                      m[4] * m[3] * m[13] +
                      m[12] * m[1] * m[7] -
                      m[12] * m[3] * m[5];

            inv[14] = -m[0] * m[5] * m[14] +
                       m[0] * m[6] * m[13] +
                       m[4] * m[1] * m[14] -
                       m[4] * m[2] * m[13] -
                       m[12] * m[1] * m[6] +
                       m[12] * m[2] * m[5];

            inv[3] = -m[1] * m[6] * m[11] +
                      m[1] * m[7] * m[10] +
                      m[5] * m[2] * m[11] -
                      m[5] * m[3] * m[10] -
                      m[9] * m[2] * m[7] +
                      m[9] * m[3] * m[6];

            inv[7] = m[0] * m[6] * m[11] -
                     m[0] * m[7] * m[10] -
                     m[4] * m[2] * m[11] +
                     m[4] * m[3] * m[10] +
                     m[8] * m[2] * m[7] -
                     m[8] * m[3] * m[6];

            inv[11] = -m[0] * m[5] * m[11] +
                       m[0] * m[7] * m[9] +
                       m[4] * m[1] * m[11] -
                       m[4] * m[3] * m[9] -
                       m[8] * m[1] * m[7] +
                       m[8] * m[3] * m[5];

            inv[15] = m[0] * m[5] * m[10] -
                      m[0] * m[6] * m[9] -
                      m[4] * m[1] * m[10] +
                      m[4] * m[2] * m[9] +
                      m[8] * m[1] * m[6] -
                      m[8] * m[2] * m[5];

            float det  = m[0] * inv[0] + m[1] * inv[4] + m[2] * inv[8] + m[3] * inv[12];

            if (det == 0)
            {
               // throw new Exception();
                mat4 = new Matrix4x4();
                return false;
            }

            det = 1.0f / det;

            for (int i = 0; i < 16; i++)
                m[i] = inv[i] * det;

            mat4 = mat.Transposed();
            return true;
        }

        public static bool Inverse2(Matrix4x4 input, out Matrix4x4 mat4)
        {
            //ripped from GLU
            float[] inv = new float[16];

            Matrix4x4 mat = input;
            float* m = (float*)&mat;

            inv[0] = m[5] * m[10] * m[15] -
                     m[5] * m[11] * m[14] -
                     m[9] * m[6] * m[15] +
                     m[9] * m[7] * m[14] +
                     m[13] * m[6] * m[11] -
                     m[13] * m[7] * m[10];

            inv[4] = -m[4] * m[10] * m[15] +
                      m[4] * m[11] * m[14] +
                      m[8] * m[6] * m[15] -
                      m[8] * m[7] * m[14] -
                      m[12] * m[6] * m[11] +
                      m[12] * m[7] * m[10];

            inv[8] = m[4] * m[9] * m[15] -
                     m[4] * m[11] * m[13] -
                     m[8] * m[5] * m[15] +
                     m[8] * m[7] * m[13] +
                     m[12] * m[5] * m[11] -
                     m[12] * m[7] * m[9];

            inv[12] = -m[4] * m[9] * m[14] +
                       m[4] * m[10] * m[13] +
                       m[8] * m[5] * m[14] -
                       m[8] * m[6] * m[13] -
                       m[12] * m[5] * m[10] +
                       m[12] * m[6] * m[9];

            inv[1] = -m[1] * m[10] * m[15] +
                      m[1] * m[11] * m[14] +
                      m[9] * m[2] * m[15] -
                      m[9] * m[3] * m[14] -
                      m[13] * m[2] * m[11] +
                      m[13] * m[3] * m[10];

            inv[5] = m[0] * m[10] * m[15] -
                     m[0] * m[11] * m[14] -
                     m[8] * m[2] * m[15] +
                     m[8] * m[3] * m[14] +
                     m[12] * m[2] * m[11] -
                     m[12] * m[3] * m[10];

            inv[9] = -m[0] * m[9] * m[15] +
                      m[0] * m[11] * m[13] +
                      m[8] * m[1] * m[15] -
                      m[8] * m[3] * m[13] -
                      m[12] * m[1] * m[11] +
                      m[12] * m[3] * m[9];

            inv[13] = m[0] * m[9] * m[14] -
                      m[0] * m[10] * m[13] -
                      m[8] * m[1] * m[14] +
                      m[8] * m[2] * m[13] +
                      m[12] * m[1] * m[10] -
                      m[12] * m[2] * m[9];

            inv[2] = m[1] * m[6] * m[15] -
                     m[1] * m[7] * m[14] -
                     m[5] * m[2] * m[15] +
                     m[5] * m[3] * m[14] +
                     m[13] * m[2] * m[7] -
                     m[13] * m[3] * m[6];

            inv[6] = -m[0] * m[6] * m[15] +
                      m[0] * m[7] * m[14] +
                      m[4] * m[2] * m[15] -
                      m[4] * m[3] * m[14] -
                      m[12] * m[2] * m[7] +
                      m[12] * m[3] * m[6];

            inv[10] = m[0] * m[5] * m[15] -
                      m[0] * m[7] * m[13] -
                      m[4] * m[1] * m[15] +
                      m[4] * m[3] * m[13] +
                      m[12] * m[1] * m[7] -
                      m[12] * m[3] * m[5];

            inv[14] = -m[0] * m[5] * m[14] +
                       m[0] * m[6] * m[13] +
                       m[4] * m[1] * m[14] -
                       m[4] * m[2] * m[13] -
                       m[12] * m[1] * m[6] +
                       m[12] * m[2] * m[5];

            inv[3] = -m[1] * m[6] * m[11] +
                      m[1] * m[7] * m[10] +
                      m[5] * m[2] * m[11] -
                      m[5] * m[3] * m[10] -
                      m[9] * m[2] * m[7] +
                      m[9] * m[3] * m[6];

            inv[7] = m[0] * m[6] * m[11] -
                     m[0] * m[7] * m[10] -
                     m[4] * m[2] * m[11] +
                     m[4] * m[3] * m[10] +
                     m[8] * m[2] * m[7] -
                     m[8] * m[3] * m[6];

            inv[11] = -m[0] * m[5] * m[11] +
                       m[0] * m[7] * m[9] +
                       m[4] * m[1] * m[11] -
                       m[4] * m[3] * m[9] -
                       m[8] * m[1] * m[7] +
                       m[8] * m[3] * m[5];

            inv[15] = m[0] * m[5] * m[10] -
                      m[0] * m[6] * m[9] -
                      m[4] * m[1] * m[10] +
                      m[4] * m[2] * m[9] +
                      m[8] * m[1] * m[6] -
                      m[8] * m[2] * m[5];

            float det = m[0] * inv[0] + m[1] * inv[4] + m[2] * inv[8] + m[3] * inv[12];

            if (det <= 1E-5f)
            {
                throw new Exception();
                mat4 = new Matrix4x4();
                return false;
            }

            det = 1.0f / det;

            for (int i = 0; i < 16; i++)
                m[i] = inv[i] * det;

            mat4 = mat;
            return true;
        }



        public static Matrix4x4 TranslationMatrix(Vector3 Position)
        {
            Matrix4x4 result = new Matrix4x4();
            result.SetIdentityMatrix();
            result.X3Y0 = Position.x;
            result.X3Y1 = Position.y;
            result.X3Y2 = Position.z;
            result.X3Y3 = 1;

            return result;
        }

        public static Matrix4x4 ScaleMatrix(Vector3 Scale)
        {
            Matrix4x4 result = new Matrix4x4();
            result.SetIdentityMatrix();
            result.X0Y0 = Scale.x;
            result.X1Y1 = Scale.y;
            result.X2Y2 = Scale.z;
            result.X3Y3 = 1;

            return result;
        }

        public static Matrix4x4 ScaleMatrix(float Scale)
        {
            Matrix4x4 result = new Matrix4x4();
            result.SetIdentityMatrix();
            result.X0Y0 = Scale;
            result.X1Y1 = Scale;
            result.X2Y2 = Scale;
            result.X3Y3 = 1f;

            return result;
        }

        public static Matrix4x4 YawMatrix(float zAxisEulerAngle)
        {
            Matrix4x4 result = new Matrix4x4();

            const float deg2rad = (float)(Math.PI / 180d);

            float cosa = (float)Math.Cos(deg2rad * zAxisEulerAngle);
            float sina = (float)Math.Sin(deg2rad * zAxisEulerAngle);

            result.SetIdentityMatrix();

            result.X0Y0 = cosa;
            result.X1Y0 = -sina;

            result.X0Y1 = sina;
            result.X1Y1 = cosa;

            result.X2Y2 = 1;
            result.X3Y3 = 1;

            return result;
        }

        public static Matrix4x4 PitchMatrix(float yAxisEulerAngle)
        {
            Matrix4x4 result = new Matrix4x4();

            const float deg2rad = (float)(Math.PI / 180d);

            float cosb = (float)Math.Cos(deg2rad * yAxisEulerAngle);
            float sinb = (float)Math.Sin(deg2rad * yAxisEulerAngle);

            result.SetIdentityMatrix();

            result.X0Y0 = cosb;
            result.X2Y0 = sinb;

            result.X1Y1 = 1;

            result.X0Y2 = -sinb;
            result.X2Y2 = cosb;

            return result;
        }

        public static Matrix4x4 RollMatrix(float xAxisEulerAngle)
        {
            Matrix4x4 result = new Matrix4x4();

            const float deg2rad = (float)(Math.PI / 180d);

            float cosy = (float)Math.Cos(deg2rad * xAxisEulerAngle);
            float siny = (float)Math.Sin(deg2rad * xAxisEulerAngle);

            result.SetIdentityMatrix();

            result.X0Y0 = 1;

            result.X1Y1 = cosy;
            result.X2Y1 = -siny;
            result.X1Y2 = siny;
            result.X2Y2 = cosy;

            return result;
        }

        public Matrix4x4 Transposed()
        {
            Matrix4x4 ret = new Matrix4x4();

            ret.X0Y0 = X0Y0;
            ret.X1Y0 = X0Y1;
            ret.X2Y0 = X0Y2;
            ret.X3Y0 = X0Y3;

            ret.X0Y1 = X1Y0;
            ret.X1Y1 = X1Y1;
            ret.X2Y1 = X1Y2;
            ret.X3Y1 = X1Y3;

            ret.X0Y2 = X2Y0;
            ret.X1Y2 = X2Y1;
            ret.X2Y2 = X2Y2;
            ret.X3Y2 = X2Y3;

            ret.X0Y3 = X3Y0;
            ret.X1Y3 = X3Y1;
            ret.X2Y3 = X3Y2;
            ret.X3Y3 = X3Y3;


            return ret;
        }

        public Matrix3x3 ToMatrix3x3()
        {
            Matrix3x3 ret = new Matrix3x3();
            ret.X0Y0 = X0Y0;
            ret.X1Y0 = X1Y0;
            ret.X2Y0 = X2Y0;

            ret.X0Y1 = X0Y1;
            ret.X1Y1 = X1Y1;
            ret.X2Y1 = X2Y1;

            ret.X0Y2 = X0Y2;
            ret.X1Y2 = X1Y2;
            ret.X2Y2 = X2Y2;

            return ret;
        }

        public Matrix4x4 TranslateScaleOnly()
        {
            return new Matrix4x4()
            {
                X0Y0 = 1,
                X1Y1 = 1,
                X2Y2 = 1,

                X0Y3 = this.X0Y3,
                X1Y3 = this.X1Y3,
                X2Y3 = this.X2Y3,

                X3Y0 = this.X3Y0,
                X3Y1 = this.X3Y1,
                X3Y2 = this.X3Y2,

                X3Y3 = this.X3Y3
            };
        }

        public static Matrix4x4 operator +(Matrix4x4 A, Matrix4x4 B)
        {
            Matrix4x4 reslt = new Matrix4x4();

            float* ptr = (float*)&reslt;
            float* ptra = (float*)&A;
            float* ptrb = (float*)&B;

            for (int i = 0; i < 16; i++)
                ptr[i] = ptra[i] + ptrb[i];

            return reslt;
        }

        public override string ToString()
        {
            return
                X0Y0 + " " + X1Y0 + " " + X2Y0 + " " + X3Y0 + "\n" +
                X0Y1 + " " + X1Y1 + " " + X2Y1 + " " + X3Y1 + "\n" +
                X0Y2 + " " + X1Y2 + " " + X2Y2 + " " + X3Y2 + "\n" +
                X0Y3 + " " + X1Y3 + " " + X2Y3 + " " + X3Y3 + "\n";
        }

        public static Matrix4x4 operator -(Matrix4x4 A, Matrix4x4 B)
        {
            Matrix4x4 reslt = new Matrix4x4();

            float* ptr = (float*)&reslt;
            float* ptra = (float*)&A;
            float* ptrb = (float*)&B;

            for (int i = 0; i < 16; i++)
                ptr[i] = ptra[i] - ptrb[i];

            return reslt;
        }

        public static Matrix4x4 operator *(float A, Matrix4x4 B)
        {
            float* ptr = (float*)&B;

            for (int i = 0; i < 16; i++)
                ptr[i] *= A;

            return B;
        }

        public static Matrix4x4 operator *(Matrix4x4 B, float A)
        {
            float* ptr = (float*)&B;

            for (int i = 0; i < 16; i++)
                ptr[i] *= A;

            return B;
        }

        public static Matrix4x4 operator *(Matrix4x4 A, Matrix4x4 B)
        {
            Matrix4x4 result = new Matrix4x4();

            result.X0Y0 = A.X0Y0 * B.X0Y0 + A.X1Y0 * B.X0Y1 + A.X2Y0 * B.X0Y2 + A.X3Y0 * B.X0Y3;
            result.X1Y0 = A.X0Y0 * B.X1Y0 + A.X1Y0 * B.X1Y1 + A.X2Y0 * B.X1Y2 + A.X3Y0 * B.X1Y3;
            result.X2Y0 = A.X0Y0 * B.X2Y0 + A.X1Y0 * B.X2Y1 + A.X2Y0 * B.X2Y2 + A.X3Y0 * B.X2Y3;
            result.X3Y0 = A.X0Y0 * B.X3Y0 + A.X1Y0 * B.X3Y1 + A.X2Y0 * B.X3Y2 + A.X3Y0 * B.X3Y3;

            result.X0Y1 = A.X0Y1 * B.X0Y0 + A.X1Y1 * B.X0Y1 + A.X2Y1 * B.X0Y2 + A.X3Y1 * B.X0Y3;
            result.X1Y1 = A.X0Y1 * B.X1Y0 + A.X1Y1 * B.X1Y1 + A.X2Y1 * B.X1Y2 + A.X3Y1 * B.X1Y3;
            result.X2Y1 = A.X0Y1 * B.X2Y0 + A.X1Y1 * B.X2Y1 + A.X2Y1 * B.X2Y2 + A.X3Y1 * B.X2Y3;
            result.X3Y1 = A.X0Y1 * B.X3Y0 + A.X1Y1 * B.X3Y1 + A.X2Y1 * B.X3Y2 + A.X3Y1 * B.X3Y3;

            result.X0Y2 = A.X0Y2 * B.X0Y0 + A.X1Y2 * B.X0Y1 + A.X2Y2 * B.X0Y2 + A.X3Y2 * B.X0Y3;
            result.X1Y2 = A.X0Y2 * B.X1Y0 + A.X1Y2 * B.X1Y1 + A.X2Y2 * B.X1Y2 + A.X3Y2 * B.X1Y3;
            result.X2Y2 = A.X0Y2 * B.X2Y0 + A.X1Y2 * B.X2Y1 + A.X2Y2 * B.X2Y2 + A.X3Y2 * B.X2Y3;
            result.X3Y2 = A.X0Y2 * B.X3Y0 + A.X1Y2 * B.X3Y1 + A.X2Y2 * B.X3Y2 + A.X3Y2 * B.X3Y3;

            result.X0Y3 = A.X0Y3 * B.X0Y0 + A.X1Y3 * B.X0Y1 + A.X2Y3 * B.X0Y2 + A.X3Y3 * B.X0Y3;
            result.X1Y3 = A.X0Y3 * B.X1Y0 + A.X1Y3 * B.X1Y1 + A.X2Y3 * B.X1Y2 + A.X3Y3 * B.X1Y3;
            result.X2Y3 = A.X0Y3 * B.X2Y0 + A.X1Y3 * B.X2Y1 + A.X2Y3 * B.X2Y2 + A.X3Y3 * B.X2Y3;
            result.X3Y3 = A.X0Y3 * B.X3Y0 + A.X1Y3 * B.X3Y1 + A.X2Y3 * B.X3Y2 + A.X3Y3 * B.X3Y3;

            return result;
        }

        public static Vector4 operator *(Matrix4x4 A, Vector4 B)
        {
            Vector4 result = new Vector4();
            result.x = A.X0Y0 * B.x + A.X1Y0 * B.y + A.X2Y0 * B.z + A.X3Y0 * B.w;
            result.y = A.X0Y1 * B.x + A.X1Y1 * B.y + A.X2Y1 * B.z + A.X3Y1 * B.w;
            result.z = A.X0Y2 * B.x + A.X1Y2 * B.y + A.X2Y2 * B.z + A.X3Y2 * B.w;
            result.w = A.X0Y3 * B.x + A.X1Y3 * B.y + A.X2Y3 * B.z + A.X3Y3 * B.w;

            return result;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Matrix3x3
    {
        public float X0Y0;
        public float X0Y1;
        public float X0Y2;

        public float X1Y0;
        public float X1Y1;
        public float X1Y2;

        public float X2Y0;
        public float X2Y1;
        public float X2Y2;

        public void SetZeroMatrix()
        {
            fixed (Matrix3x3* mat3 = &this)
            {
                float* ptr = (float*)mat3;

                for (int i = 0; i < 9; i++)
                    ptr[i] = 0;
            }
        }

        public void SetIdentityMatrix()
        {
            fixed (Matrix3x3* mat3 = &this)
            {
                float* ptr = (float*)mat3;

                ptr[0] = 1;
                ptr[1] = 0;
                ptr[2] = 0;
                ptr[3] = 0;
                ptr[4] = 1;
                ptr[5] = 0;
                ptr[6] = 0;
                ptr[7] = 0;
                ptr[8] = 1;

            }
        }

        public static Matrix3x3 FromArray(params float[] input)
        {
            if (input.Length != 9)
                throw new Exception("Invalid Size");

            Matrix3x3 mat3 = new Matrix3x3();

            for (int i = 0; i < 9; i++)
            {
                mat3[i] = input[i];
            }

            return mat3;
        }

        public static Matrix3x3 Identity { get { return IdentityMatrix(); } }

        public static Matrix3x3 IdentityMatrix()
        {
            Matrix3x3 mat = new Matrix3x3();
            mat.SetIdentityMatrix();
            return mat;
        }

        /// <summary>
        /// COLUMN MAJOR ORDER
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public float this[int index]
        {
            get {
                if (index == 0) return X0Y0;
                else if (index == 1) return X0Y1;
                else if (index == 2) return X0Y2;
                else if (index == 3) return X1Y0;
                else if (index == 4) return X1Y1;
                else if (index == 5) return X1Y2;
                else if (index == 6) return X2Y0;
                else if (index == 7) return X2Y1;
                else if (index == 8) return X2Y2;
                else throw new Exception("Out of Range!");
            }
            set { if (index == 0) X0Y0 = value;
                else if (index == 1) X0Y1 = value;
                else if (index == 2) X0Y2 = value;
                else if (index == 3) X1Y0 = value;
                else if (index == 4) X1Y1 = value;
                else if (index == 5) X1Y2 = value;
                else if (index == 6) X2Y0 = value;
                else if (index == 7) X2Y1 = value;
                else if (index == 8) X2Y2 = value;
                else throw new Exception("Out of Range!"); }
        }


        public float this[int row, int column]
        {
            get
            {
                return this[row + column * 3];
            }
            set
            {
                this[row + column * 3] = value;
            }
        }

        /// <summary>
        /// Create Camera Rotation where EulerAngles are yaw (z axis) * pitch (y axis) * roll (x axis)
        /// </summary>
        /// <param name="EulerAngles"></param>
        /// <returns></returns>
        public static Matrix3x3 CameraRotation(Vector3 EulerAngles)
        {
            Matrix3x3 result = new Matrix3x3();

            const float deg2rad = (float)(Math.PI / 180d);

            float cosa = (float)Math.Cos(deg2rad * EulerAngles.z);
            float cosb = (float)Math.Cos(deg2rad * EulerAngles.y);
            float cosy = (float)Math.Cos(deg2rad * EulerAngles.x);

            float sina = (float)Math.Sin(deg2rad * EulerAngles.z);
            float sinb = (float)Math.Sin(deg2rad * EulerAngles.y);
            float siny = (float)Math.Sin(deg2rad * EulerAngles.x);

            result.X0Y0 = cosa * cosb;
            result.X1Y0 = cosa * sinb * siny - sina * cosy;
            result.X2Y0 = cosa * sinb * cosy + sina * siny;

            result.X0Y1 = sina * cosb;
            result.X1Y1 = sina * sinb * siny + cosa * cosy;
            result.X2Y1 = sina * sinb * cosy - cosa * siny;

            result.X0Y2 = -sinb;
            result.X1Y2 = cosb * siny;
            result.X2Y2 = cosb * cosy;

            return result;
        }

        public static Matrix3x3 YawMatrix(float zAxisEulerAngle)
        {
            Matrix3x3 result = new Matrix3x3();

            const float deg2rad = (float)(Math.PI / 180d);

            float cosa = (float)Math.Cos(deg2rad * zAxisEulerAngle);
            float sina = (float)Math.Sin(deg2rad * zAxisEulerAngle);

            result.X0Y0 = cosa;
            result.X1Y0 = -sina;
            result.X2Y0 = 0;

            result.X0Y1 = sina;
            result.X1Y1 = cosa;
            result.X2Y1 = 0;

            result.X0Y2 = 0;
            result.X1Y2 = 0;
            result.X2Y2 = 1;

            return result;
        }

        public static Matrix3x3 PitchMatrix(float yAxisEulerAngle)
        {
            Matrix3x3 result = new Matrix3x3();

            const float deg2rad = (float)(Math.PI / 180d);

            float cosb = (float)Math.Cos(deg2rad * yAxisEulerAngle);
            float sinb = (float)Math.Sin(deg2rad * yAxisEulerAngle);

            result.X0Y0 = cosb;
            result.X1Y0 = 0;
            result.X2Y0 = sinb;

            result.X0Y1 = 0;
            result.X1Y1 = 1;
            result.X2Y1 = 0;

            result.X0Y2 = -sinb;
            result.X1Y2 = 0;
            result.X2Y2 = cosb;

            return result;
        }

        public static Matrix3x3 RollMatrix(float xAxisEulerAngle)
        {
            Matrix3x3 result = new Matrix3x3();

            const float deg2rad = (float)(Math.PI / 180d);

            float cosy = (float)Math.Cos(deg2rad * xAxisEulerAngle);
            float siny = (float)Math.Sin(deg2rad * xAxisEulerAngle);

            result.X0Y0 = 1;
            result.X1Y0 = 0;
            result.X2Y0 = 0;

            result.X0Y1 = 0;
            result.X1Y1 = cosy;
            result.X2Y1 = -siny;

            result.X0Y2 = 0;
            result.X1Y2 = siny;
            result.X2Y2 = cosy;

            return result;
        }

        public Matrix3x3 Transposed()
        {
            Matrix3x3 ret = new Matrix3x3();

            ret.X0Y0 = X0Y0;
            ret.X1Y0 = X0Y1;
            ret.X2Y0 = X0Y2;

            ret.X0Y1 = X1Y0;
            ret.X1Y1 = X1Y1;
            ret.X2Y1 = X1Y2;

            ret.X0Y2 = X2Y0;
            ret.X1Y2 = X2Y1;
            ret.X2Y2 = X2Y2;

            return ret;
        }

        public static Matrix3x3 RotationFromNormalTangent(Vector3 direction, Vector3 up = default(Vector3))
        {
            if (direction.isApproximately(up))
                return Matrix3x3.IdentityMatrix();

            if (up.Equals(new Vector3(0, 0, 0))) up = new Vector3(0, 0, 1);

            Vector3 xaxis = Vector3.Normalize(Vector3.Cross(up, direction));
            Vector3 yaxis = Vector3.Normalize(Vector3.Cross(direction, xaxis));

            Matrix3x3 mat3 = new Matrix3x3()
            {
                X0Y0 = xaxis.x,
                X1Y0 = yaxis.x,
                X2Y0 = direction.x,

                X0Y1 = xaxis.y,
                X1Y1 = yaxis.y,
                X2Y1 = direction.y,

                X0Y2 = xaxis.z,
                X1Y2 = yaxis.z,
                X2Y2 = direction.z
            };

            return mat3;
        }

        public static Matrix3x3 FromDirectionVector_transposed(Vector3 direction, Vector3 up)
        {
            //WARNING MATRIX NEEDS TO BE TRANSPOSED!!!
            Vector3 xaxis = Vector3.Cross(up, direction);
            xaxis = Vector3.Normalize(xaxis);

            Vector3 yaxis = Vector3.Cross(direction, xaxis);
            yaxis = Vector3.Normalize(yaxis);

            Matrix3x3 result = new Matrix3x3();

            result.X0Y0 = xaxis.x;
            result.X0Y1 = xaxis.y;
            result.X0Y2 = direction.x;

            result.X1Y0 = xaxis.y;
            result.X1Y1 = yaxis.y;
            result.X1Y2 = direction.y;

            result.X2Y0 = xaxis.z;
            result.X2Y1 = yaxis.z;
            result.X2Y2 = direction.z;

            return result;
        }

        public static Matrix3x3 operator +(Matrix3x3 A, Matrix3x3 B)
        {
            Matrix3x3 reslt = new Matrix3x3();

            reslt.X0Y0 = A.X0Y0 + B.X0Y0;
            reslt.X1Y0 = A.X1Y0 + B.X1Y0;
            reslt.X2Y0 = A.X2Y0 + B.X2Y0;
            reslt.X0Y1 = A.X0Y1 + B.X0Y1;
            reslt.X1Y1 = A.X1Y1 + B.X1Y1;
            reslt.X2Y1 = A.X2Y1 + B.X2Y1;
            reslt.X0Y2 = A.X0Y2 + B.X0Y2;
            reslt.X1Y2 = A.X1Y2 + B.X1Y2;
            reslt.X2Y2 = A.X2Y2 + B.X2Y2;

            return reslt;
        }

        public static Matrix3x3 operator -(Matrix3x3 A, Matrix3x3 B)
        {
            Matrix3x3 reslt = new Matrix3x3();

            reslt.X0Y0 = A.X0Y0 - B.X0Y0;
            reslt.X1Y0 = A.X1Y0 - B.X1Y0;
            reslt.X2Y0 = A.X2Y0 - B.X2Y0;
            reslt.X0Y1 = A.X0Y1 - B.X0Y1;
            reslt.X1Y1 = A.X1Y1 - B.X1Y1;
            reslt.X2Y1 = A.X2Y1 - B.X2Y1;
            reslt.X0Y2 = A.X0Y2 - B.X0Y2;
            reslt.X1Y2 = A.X1Y2 - B.X1Y2;
            reslt.X2Y2 = A.X2Y2 - B.X2Y2;

            return reslt;
        }

        public static Matrix3x3 operator *(float A, Matrix3x3 B)
        {
            float* ptr = (float*)&B;

            for (int i = 0; i < 9; i++)
                ptr[i] *= A;

            return B;
        }

        public static Matrix3x3 operator *(Matrix3x3 B, float A)
        {
            float* ptr = (float*)&B;

            for (int i = 0; i < 9; i++)
                ptr[i] *= A;

            return B;
        }

        public static Matrix3x3 operator *(Matrix3x3 A, Matrix3x3 B)
        {
            Matrix3x3 result = new Matrix3x3();

            result.X0Y0 = A.X0Y0 * B.X0Y0 + A.X1Y0 * B.X0Y1 + A.X2Y0 * B.X0Y2;
            result.X1Y0 = A.X0Y0 * B.X1Y0 + A.X1Y0 * B.X1Y1 + A.X2Y0 * B.X1Y2;
            result.X2Y0 = A.X0Y0 * B.X2Y0 + A.X1Y0 * B.X2Y1 + A.X2Y0 * B.X2Y2;

            result.X0Y1 = A.X0Y1 * B.X0Y0 + A.X1Y1 * B.X0Y1 + A.X2Y1 * B.X0Y2;
            result.X1Y1 = A.X0Y1 * B.X1Y0 + A.X1Y1 * B.X1Y1 + A.X2Y1 * B.X1Y2;
            result.X2Y1 = A.X0Y1 * B.X2Y0 + A.X1Y1 * B.X2Y1 + A.X2Y1 * B.X2Y2;

            result.X0Y2 = A.X0Y2 * B.X0Y0 + A.X1Y2 * B.X0Y1 + A.X2Y2 * B.X0Y2;
            result.X1Y2 = A.X0Y2 * B.X1Y0 + A.X1Y2 * B.X1Y1 + A.X2Y2 * B.X1Y2;
            result.X2Y2 = A.X0Y2 * B.X2Y0 + A.X1Y2 * B.X2Y1 + A.X2Y2 * B.X2Y2;

            return result;
        }

        public static Vector3 operator *(Matrix3x3 A, Vector3 B)
        {
            Vector3 result = new Vector3();
            result.x = A.X0Y0 * B.x + A.X1Y0 * B.y + A.X2Y0 * B.z;
            result.y = A.X0Y1 * B.x + A.X1Y1 * B.y + A.X2Y1 * B.z;
            result.z = A.X0Y2 * B.x + A.X1Y2 * B.y + A.X2Y2 * B.z;
            return result;
        }

        public override string ToString()
        {
            string str = "";

            for (int i = 0; i < 9; i++)
            {
                str += this[i] + " ";
            }

            return str;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Vector4
    {
        public float x, y, z, w;

        public Vector4(float X, float Y, float Z, float W)
        {
            x = X;
            y = Y;
            z = Z;
            w = W;
        }

        public Vector4(Vector3 vec3, float W)
        {
            x = vec3.x;
            y = vec3.y;
            z = vec3.z;
            w = W;
        }

        public Vector3 xyz()
        {
            return new Vector3(x, y, z);
        }

        public static Vector4 operator -(Vector4 A)
        {
            return new Vector4(-A.x, -A.y, -A.z, -A.w);
        }

        /// <summary>
        /// Adds two Vector3 together
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static Vector4 operator +(Vector4 A, Vector4 B)
        {
            return new Vector4(A.x + B.x, A.y + B.y, A.z + B.z, A.w + B.w);
        }
        /// <summary>
        /// Substacts Vector B from Vector A
        /// </summary>
        /// <param name="A">Vector A</param>
        /// <param name="B">Vector B</param>
        /// <returns></returns>
        public static Vector4 operator -(Vector4 A, Vector4 B)
        {
            return new Vector4(A.x - B.x, A.y - B.y, A.z - B.z, A.w - B.w);
        }

        public static Vector4 operator *(Vector4 A, float B)
        {
            return new Vector4(A.x * B, A.y * B, A.z * B, A.w * B);
        }

        /// <summary>
        /// Returns a string in the format of "Vector4
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "X: " + x.ToString() + ", Y: " + y.ToString() + ", Z:" + z.ToString() + ", W: " + w.ToString();
        }
    }
}
