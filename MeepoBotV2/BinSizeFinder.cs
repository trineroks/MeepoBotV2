using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeepoBotV2 {
    class BinSizeFinder : BinSerializer {
        private int len = 0;

        public override void writeByte(byte data) {
            len += 1;
        }

        public override void writeUInt32(uint value)
        {
            len += 4;
        }

        public override void writeInt(int value)
        {
            len += 4;
        }

        public override void writeUInt64(ulong value)
        {
            len += 8;
        }

        public override void writeUTF8String(string value)
        {
            len += 4; //remember to write the size of the string!
            len += Encoding.UTF8.GetByteCount(value);
        }

        public int size()
        {
            return len;
        }
    }
}
