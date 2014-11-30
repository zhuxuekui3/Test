using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Spmatrix;


/*
 * 时间：2014.1.12
 * 作者：胡正坤
 * 版本：V1.0
 * 程序描述：序列化和反序列化方法
 * 详细介绍：Serialize和Desrialize为使用“智能车公共数据池标准v4.1”中的数据格式，稀疏矩阵不用泛型，U使用byte类型
 * */
public class SerializeTool
{
    /// <summary>
    /// 将稀疏矩阵序列化为byte数组(按自定义的序列化方法)
    /// </summary>
    /// <param name="obj">稀疏矩阵对象</param>
    /// <param name="startoffset">稀疏矩阵节点在byte数组的起始位置</param>
    /// <param name="PointLength">每个稀疏矩阵节点在byte数组中的存储长度（即字节数）</param>
    public static byte[] Serialize(spmatrix obj, int startoffset, int PointLength)
    {
        try
        {
            int td = obj.td;
            byte[] buffer = new byte[startoffset + td * PointLength];
            byte[] c = System.BitConverter.GetBytes(td);

            // If the system architecture is little-endian (that is, little end first),
            // reverse the byte array.
            //if (BitConverter.IsLittleEndian)
            //    Array.Reverse(c);

            for (byte i = 0; i < c.Length; i++)
            {
                buffer[i] = c[i];
            }

            for (int i = 0; i < td; i++)
            {
                byte[] tu_byte = ByteAndStruct.StructureToByteArray(obj.data[i]);

                for (int j = 0; j < PointLength; j++)
                {
                    buffer[startoffset + j + i * PointLength] = tu_byte[j];
                }

            }

            return buffer;
        }
        catch (Exception ex)
        {
            throw new Exception("序列化失败,原因:" + ex.Message);
        }
    }

    /// <summary>
    /// 将byte数组反序列化为稀疏矩阵(按C#自带的反序列化)
    /// </summary>
    /// <param name="T">泛型</param>
    /// <param name="bytes">序列化处理后的字节数组</param>
    public static byte[,] Desrialize(byte[,] mat, byte[] bytes, int startoffset, int PointLength)
    {
        try
        {

            tupletype tu = new tupletype();

            byte[] td_byte = new byte[startoffset];

            for (byte i = 0; i < startoffset; i++)
            {
                td_byte[i] = bytes[i];
            }

            // If the system architecture is little-endian (that is, little end first),
            // reverse the byte array.
            //if (BitConverter.IsLittleEndian)
            //    Array.Reverse(td_byte);

            int td = BitConverter.ToInt32(td_byte, 0);


            for (int i = 0; i < td; i++)
            {
                byte[] tu_byte = new byte[PointLength];
                for (byte j = 0; j < PointLength; j++)
                {
                    tu_byte[j] = bytes[startoffset + PointLength * i + j];
                }

                object tu_obj = tu;

                ByteAndStruct.ByteArrayToStructure(tu_byte, ref tu_obj, 0);
                tu = (tupletype)tu_obj;

                mat[tu.x, tu.y] = tu.value;
            }

        }
        catch (Exception ex)
        {
            throw new Exception("反序列化失败,原因:" + ex.Message);
        }
        return mat;
    }
}
