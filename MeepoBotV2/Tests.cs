using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MeepoBotV2 {
    class Tests {

        private const string gamesSaveFile = "keylist1.groks";
        private string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MeepoBotGiveaway");

        List<List<String>> test = new List<List<String>>();

        public void testSave() {
            test.Add(new List<string>());
            test.Add(new List<string>());
            test.Add(new List<string>());
            test[0].Add("Item01");
            test[1].Add("Item11");
            test[0].Add("Item02");
            test[2].Add("Item21");

            int arrLen = 0;

            int list1len = test.Count;
            arrLen += 4; //integer for the superlist length

            arrLen += 4 * list1len; //integers for the length of each sublist

            for (int i = 0; i < list1len; i++) {
                arrLen += 4 * test[i].Count; //integers for the length of each string
                foreach (string s in test[i]) {
                    int slen = Encoding.UTF8.GetByteCount(s);
                    arrLen += slen; //actual size of the string;
                }
            }
            BinSerializer serializer = new BinSerializer(arrLen);
            serializer.writeInt(list1len);
            foreach (List<string> ls in test) {
                serializer.writeInt(ls.Count);
                foreach (string s in ls) {
                    int slen = Encoding.UTF8.GetByteCount(s);
                    serializer.writeInt(slen);
                    serializer.writeUTF8String(s);
                }
            }

            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }

            string filepath = Path.Combine(path, gamesSaveFile);
            File.WriteAllBytes(filepath, serializer.data);
        }

        public void testLoad() {
            string filepath = Path.Combine(path, gamesSaveFile);

            if (!File.Exists(filepath))
                return;

            test.Clear();
            byte[] gamesList = File.ReadAllBytes(filepath);
            BinReader reader = new BinReader(gamesList);

            int gamesCount = reader.readInt();
            for (int i = 0; i < gamesCount; i++) {
                int sublistCount = reader.readInt();
                List<string> ls = new List<string>();
                for (int j = 0; j < sublistCount; j++) {
                    int strLen = reader.readInt();
                    string str = reader.readUTF8String(strLen);
                    ls.Add(str);
                }
                test.Add(ls);
            }
        }
    }
}
