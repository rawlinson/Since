
using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.InteropServices;

namespace Since.Rdf.Serialization
{
    static class VarInt
    {
        public static void WriteVarInt<T>(this BinaryWriter writer, T value)
            where T : struct, IConvertible
            => VarInt.WriteVarInt(writer, Convert.ToUInt64(value));

        public static void WriteVarInt(BinaryWriter writer, UInt64 value)
        {
            Contract.Requires(writer != null);

            do
            {
                var byteVal = value & 0x7f;
                value >>= 7;

                if (value != 0)
                    byteVal |= 0x80;

                writer.Write((byte)byteVal);

            } while (value != 0);            
        }
  
        public static T ReadVarInt<T>(this BinaryReader reader)
            where T : struct, IConvertible
        {
            Contract.Requires(reader != null);

            switch (default(T).GetTypeCode())
            {
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return (T)Convert.ChangeType(VarInt.ReadVarInt(reader, 8 * Marshal.SizeOf<T>()), typeof(T));
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    var zigzag = VarInt.ReadVarInt(reader, 8 * Marshal.SizeOf<T>());
                    return (T)Convert.ChangeType(VarInt.DecodeZigZag(zigzag), typeof(T));
                default:
                    throw new ArgumentException("Expected an integer numeric type", nameof(T));
            }
        }

        private static UInt64 ReadVarInt(BinaryReader reader, int maximumBits)
        {
            Contract.Requires(reader != null);
            Contract.Requires(maximumBits > 0);

            int shift = 0;
            ulong result = 0;

            while (true)
            {
                ulong byteValue = (ulong)reader.Read();
                ulong tmp = byteValue & 0x7f;
                result |= tmp << shift;

                if (shift > maximumBits)
                    throw new FormatException("Deserialized VarInt is too big for type.");

                if ((byteValue & 0x80) != 0x80)
                    return result;

                shift += 7;
            }
        }
        
        private static long EncodeZigZag(long value, int bitLength)
        {
            return (value << 1) ^ (value >> (bitLength - 1));
        }

        private static long DecodeZigZag(ulong value)
        {
            if ((value & 0x1) == 0x1)
                return (-1 * ((long)(value >> 1) + 1));

            return (long)(value >> 1);
        }
    }
}
