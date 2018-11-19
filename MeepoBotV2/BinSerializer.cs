using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeepoBotV2 {
    class BinSerializer {
        public byte[] data;
        public int offset = 0;

        public BinSerializer() {

        }

        public BinSerializer(int size) {
            data = new byte[size];
        }

        public BinSerializer(byte[] data) {
            this.data = data;
        }

        public virtual void writeByte(byte value) {
            data[offset++] = value;
        }

        public virtual void writeUInt64(ulong value) { //8 bytes
            writeUInt32((uint)(value >> 32));
            writeUInt32((uint)(value));
        }

        public virtual void writeInt(int value) { // 4 bytes, 32 bit int
            writeByte((byte)(value >> 24));
            writeByte((byte)(value >> 16));
            writeByte((byte)(value >> 8));
            writeByte((byte)(value));
        }

        public virtual void writeUInt32(uint value) {
            writeByte((byte)(value >> 24));
            writeByte((byte)(value >> 16));
            writeByte((byte)(value >> 8));
            writeByte((byte)(value));
        }

        public virtual void writeUTF8String(string value) {
            int len = Encoding.UTF8.GetByteCount(value);
            writeInt(len);
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            foreach (byte b in bytes) {
                data[offset++] = b;
            }
        }
    }
}