using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Tool.Utils.Data
{
    /// <summary>
    /// 对Array类进行升级
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public static class ArrayExtension
    {
        //// <summary>
        //// 给数组加新的值
        //// </summary>
        //// <param name="txt">Array</param>
        //// <param name="txt1">新增的值</param>
        //// <returns>返回Add后的新数组</returns>
        //public static unsafe void Add(this Array txt, object txt1)
        //{

        //    object[] array = new object[txt.Length + 1];
        //    int i = 0;
        //    int num = txt.GetLowerBound(0) + txt.Length - 1;
        //    TypedReference typedReference = __makeref(txt); //default(TypedReference);

        //    TypedReference.SetTypedReference(typedReference,);

        //    while (i < num)
        //    {
        //        array.SetValue(txt1,);
        //        num--;
        //    }
        //    txt.SetValue(txt1, txt.Length - 1);
        //}

        /// <summary>
        /// 给数组加新的值,效率不高，不建议循环使用,目前只适合一维数组
        /// </summary>
        /// <param name="array">数据源</param>
        /// <param name="obj">添加的数据</param>
        public static void ArrayAdd(ref Array array, object obj)//[In]
        {
            if (array.Rank != 1)
            {
                throw new Exception("目前指支持一维数组，请选择其他方法。");
            }

            Type type = array.GetType();
            string assemblyQualifiedName = type.AssemblyQualifiedName.Replace("[]", string.Empty);
            Type elType = Type.GetType(assemblyQualifiedName);

            if (elType == null)
            {
                throw new Exception("无法Add请见谅，请选择其他方法。");
            }

            if (elType != obj.GetType())
            {
                throw new Exception("您要添加的数据格式与数组数据类型不一致，请规范赋值参数。");
            }

            Array arrays = Array.CreateInstance(elType, array.Length + 1);
            Array.Copy(array, arrays, array.Length);

            arrays.SetValue(obj, arrays.Length - 1);

            array = arrays;
        }

        /// <summary>
        /// 给数组加新的值,效率不高，不建议循环使用,目前只适合一维数组
        /// </summary>
        /// <param name="array">数据源</param>
        /// <param name="obj">添加的数据</param>
        /// <param name="_array">返回一个新数组</param>
        public static void ArrayAdd([In] this Array array, object obj, out Array _array)//[In]
        {
            if (array.Rank != 1)
            {
                throw new Exception("目前指支持一维数组，请选择其他方法。");
            }

            Type type = array.GetType();
            string assemblyQualifiedName = type.AssemblyQualifiedName.Replace("[]", string.Empty);
            Type elType = Type.GetType(assemblyQualifiedName);

            if (elType == null)
            {
                throw new Exception("无法Add请见谅，请选择其他方法。");
            }

            if (elType != obj.GetType())
            {
                throw new Exception("您要添加的数据格式与数组数据类型不一致，请规范赋值参数。");
            }

            Array arrays = Array.CreateInstance(elType, array.Length + 1);
            Array.Copy(array, arrays, array.Length);

            arrays.SetValue(obj, arrays.Length - 1);

            _array = arrays;
            //_array = array;

            //_array = arrays.GetIntPtrInt();

            //var ptr = array.GetIntPtr();

            //var ptr1 = arrays.GetIntPtr();

            //array = arrays;

            //GCHandle hander = new GCHandle();

            //IntPtr @int =  _array

            //var data = ObjectExtension.Read<Array>(ptr);
            //try
            //{
            //    //hander = GCHandle.Alloc(arrays);// GCHandleType.Pinned

            //    //var ptr1 = hander.AddrOfPinnedObject();


            //    unsafe
            //    {
            //        //ptr.ToPointer();
            //        void* p = ptr.ToPointer();

            //        p = ptr1.ToPointer();

            //        int s = Other.IntPtrHelper.ReadMemoryValue(ptr1, System.Diagnostics.Process.GetCurrentProcess().Id);

            //        data = ObjectExtension.Read<Array>(s);

            //        s  = s + 0x950;
            //        //fixed (void* v = array)
            //        //{
            //        //    v = arrays;
            //        //}

            //        IntPtr.Add(ptr, _array);
            //        //*(int*)ptr = ptr.ToPointer();

            //        //p = (int*)array.GetIntPtrInt();

            //    }
            //}
            //catch (Exception)
            //{
            //    throw new System.SystemException(string.Format("该对象(\"{0}\")不能被固定内存指针！", obj.GetType().ToString()));
            //}



            //var data1 = ObjectExtension.Read<Array>(ptr);
        }

        /// <summary>
        /// 返回 <see cref="System.Array"/> 的 <see cref="Enumerator{T}"/>。
        /// </summary>
        /// <typeparam name="T">当前类型</typeparam>
        /// <param name="array">当前数据源</param>
        /// <returns>返回 <see cref="System.Array"/> 的 <see cref="Enumerator{T}"/>。</returns>
        public static Enumerator<T> GetEnumerator<T>(this Array array)
        {
            return new Enumerator<T>(array as T[]);
        }

        /// <summary>
        /// 重写封装的Copy方法（暂时未写，不用调用了）
        /// </summary>
        /// <param name="sourceArray">源数组对象</param>
        /// <param name="sourceIndex">源数据开始读取的位置</param>
        /// <param name="destinationArray">新数组对象</param>
        /// <param name="destinationIndex">开始存储的位置</param>
        /// <param name="length">从源数组取多少？</param>
        /// <returns></returns>
        public static int Read(this Array sourceArray, long sourceIndex, [In] [Out] Array destinationArray, long destinationIndex, long length)
        {
            return 0;
        }
    }

    /// <summary>
    /// 返回 <see cref="System.Array"/> 的 <see cref="Enumerator{T}"/>。
    /// </summary>
    /// <typeparam name="T">当前类型</typeparam>
    [Serializable]
    public struct Enumerator<T> : IEnumerator
    {
        private T[] array;

        /// <summary>
        /// 框架自己调用的构造
        /// </summary>
        /// <param name="array">数组对象</param>
        internal Enumerator(T[] array)
        {
            this.length = array.Length;
            this.index = -1;
            this.current = default(T);
            this.array = array;
        }

        /// <summary>
        /// 获取集合中的当前元素。
        /// </summary>
        /// <returns>集合中的当前元素。</returns>
        public T Current
        {
            get
            {
                return this.current;
            }
        }

        /// <summary>
        /// 将枚举数推进到集合的下一个元素。
        /// </summary>
        /// <returns>如果枚举数成功地推进到下一个元素，则为 true；如果枚举数越过集合的结尾，则为 false。</returns>
        /// <exception cref="System.InvalidOperationException">在创建了枚举数后集合被修改了。</exception>
        public bool MoveNext()
        {
            if (this.index == (this.length - 1))
            {
                this.current = default(T);
                return false;
            }

            this.index++;

            this.current = this.array[this.index];

            return true;
        }

        /// <summary>
        /// 将枚举数设置为其初始位置，该位置位于集合中第一个元素之前。
        /// </summary>
        /// <exception cref="System.InvalidOperationException">在创建了枚举数后集合被修改了。</exception>
        public void Reset()
        {
            this.index = -1;
        }

        /// <summary>
        /// 当前下标
        /// </summary>
        public int Index { get { return index; } }

        /// <summary>
        /// 总数
        /// </summary>
        public int Length { get { return length; } }

        object IEnumerator.Current => this.current;

        private int index;

        private int length;

        private T current;
    }
}
