using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ServerOne
{
    /// <summary>
    /// 数据包编码的工具类
    /// </summary>
     public  static class EncodeTool
    {
        #region 封装和解析数据包

        /// <summary>
        /// 构造数据包
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] EncodePackage(byte[] data) 
        {
            using (MemoryStream ms=new MemoryStream()) //使用using关键字会自动释放资源
            {
                using (BinaryWriter bw=new BinaryWriter(ms))
                {
                    bw.Write(data.Length); //先写入长度头
                    bw.Write(data); //再写入数据体尾
                    byte[] byteArray = new byte[(int)ms.Length];
                    Buffer.BlockCopy(ms.GetBuffer(), 0, byteArray, 0, (int)ms.Length); //在内存中高效率拷贝
                    return byteArray;
                }
            }
        }

        /// <summary>
        /// 解析数据包
        /// </summary>
        /// <param name="dataCache"></param>
        /// <returns></returns>
        public static byte[] DecodePackage(ref List<byte> dataCache)
        {
            if (dataCache.Count < 4) //如果数据缓存长度小于四字节int长度，无法构成数据包
            {
                throw new Exception("数据缓存长度不足4无法构成完整数据包");
            }
            using (MemoryStream ms = new MemoryStream(dataCache.ToArray()))
            {
                using (BinaryReader br=new BinaryReader(ms))
                {
                    int length = br.ReadInt32(); //包头约定长度
                    int dataRemainLength = (int)(ms.Length - ms.Position); //剩余长度
                    if (length > dataRemainLength) //如果包头表示的数据长度大于剩余数据长度说明包不完整
                    {
                        throw new Exception("数据长度不够包头所表示长度，无法构成完整数据包");
                    }
                    byte[] data = br.ReadBytes(length);
                    //更新数据缓存
                    dataCache.Clear(); //先清空
                    dataCache.AddRange(br.ReadBytes(dataRemainLength)); //再将剩余的数据存入
                    return data;
                }
            }
        }

        #endregion
    }
}
