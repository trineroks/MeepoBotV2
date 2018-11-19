using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Text;

namespace MeepoBotV2 {
    
    class GiveawayModule {
        ulong[] hosts = new ulong[2] {155891785690775552, 110191417522925568};

        //THE FIRST ELEMENT IN THIS LIST WILL ALWAYS BE THE GAME NAME, NOT THE KEY.
        List<List<string>> games = new List<List<string>>();
        Dictionary<ulong, int> userTimeCD = new Dictionary<ulong, int>();
        Dictionary<string, List<KeyGamePair>> claimMap = new Dictionary<string, List<KeyGamePair>>();

        long time = 0;
        int minute = 0;

        private int CLAIM_CD = 1800;

        private class KeyGamePair {
            public string game = "";
            public string key = "";

            public int getSerializableSize() {
                int slen = Encoding.UTF8.GetByteCount(game);
                slen += 4; //Size of the game name string
                slen += Encoding.UTF8.GetByteCount(key);
                slen += 4; //Size of the key string
                return slen;
            }

            public void serialize(BinSerializer s) {
                s.writeUTF8String(game);
                s.writeUTF8String(key);
            }

            public void deserialize(BinReader r) {
                game = r.readUTF8String();
                key = r.readUTF8String();
            }
        }

        private const string gamesSaveFile = "keylist.groks";
        private const string claimantFile = "keylist.claim";
        private const string intervalFile = "intervals.int";
        private string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MeepoBotGiveaway");

        public GiveawayModule() {
            loadKeylist();
            loadClaimlist();
            loadIntervallist();
        }

        public void updateTick(long delta) {
            time += delta;
            Dictionary<ulong, int> tempUserTime = new Dictionary<ulong, int>();
            if (time >= Constants.SECOND) {                
                int deltaTime = (int)(time / Constants.SECOND);
                minute += deltaTime;
                if (minute >= 60) {
                    saveIntervallist();
                    minute = 0;
                }
                foreach (ulong id in userTimeCD.Keys) {
                    if (userTimeCD[id] > 0) { 
                        int curr = userTimeCD[id];
                        tempUserTime[id] = curr - deltaTime;
                    }
                }
                userTimeCD = tempUserTime;
                time = 0;
            }
        }

        public async Task handleInput(SocketMessage m) {
            string input = m.ToString();
            string[] toParse = input.Split(' ');
            string command = toParse[0];
            int inputLen = toParse.Length;
            ulong id = m.Author.Id;
            if (command == "")
                return;
            else if (command == Constants.Giveaway.COMMAND_CLAIM) {
                if (inputLen != 2) {
                    await m.Channel.SendMessageAsync(Constants.INCORRECT_USE + Constants.Giveaway.Help.CLAIM);
                    return;
                }
                int idx = 0;
                if (!Int32.TryParse(toParse[1], out idx)) {
                    await m.Channel.SendMessageAsync(Constants.INCORRECT_USE + Constants.Giveaway.Help.CLAIM);
                    return;
                }
                else {
                    if (userTimeCD.ContainsKey(id)) {
                        int time = userTimeCD[id];
                        int minutes = time / 60;
                        int seconds = time % 60;
                        await m.Channel.SendMessageAsync(m.Author.Mention + " you have " + minutes + ":" + seconds + " until next claim.");
                    }
                    else {
                        string name = m.Author.Username;
                        string key = claimGameKey(idx, id, name);
                        await m.Author.SendMessageAsync(key);
                    }
                }
            }
            else if (command == Constants.Giveaway.COMMAND_ADD) {
                if (id == hosts[0] || id == hosts[1]) {
                    if (inputLen < 3) {
                        await m.Author.SendMessageAsync(Constants.INCORRECT_USE + Constants.Giveaway.Help.ADD);
                    }
                    else {
                        int idx = 0;
                        if (!Int32.TryParse(toParse[1], out idx) || idx < 1 || idx > games.Count) {
                            await m.Author.SendMessageAsync(Constants.INCORRECT_USE + Constants.Giveaway.Help.ADD);
                        }
                        else {
                            string[] args = Regex.Matches(input, @"[\""].+?[\""]|[^ ]+")
                                        .Cast<Match>()
                                        .Select(ma => ma.Value)
                                        .ToArray();
                            for (int i = 2; i < args.Length; i++) {
                                args[i] = args[i].Trim('"');
                                games[idx - 1].Add(args[i]);
                            }
                            saveKeylist();
                            await m.Author.SendMessageAsync("Successfully added " + (args.Length - 2) + " keys to the game \"" + games[idx - 1][0] + "\".");
                        }
                    }
                }
            }
            else if (command == Constants.Giveaway.COMMAND_POST) {
                if (id == hosts[0] || id == hosts[1]) {
                    if (inputLen < 3) {
                        await m.Author.SendMessageAsync(Constants.INCORRECT_USE + Constants.Giveaway.Help.POST);
                    }
                    else {
                        string temp = input.Substring(input.IndexOf(' ') + 1);

                        string[] args = Regex.Matches(temp, @"[\""].+?[\""]|[^ ]+")
                                        .Cast<Match>()
                                        .Select(ma => ma.Value)
                                        .ToArray();

                        for (int i = 0; i < args.Length; i++) {
                            args[i] = args[i].Trim('"');
                        }

                        List<string> insert = new List<string>();

                        for (int i = 0; i < args.Length; i++) {
                            insert.Add(args[i]);
                        }
                        games.Add(insert);
                        saveKeylist();
                        await m.Author.SendMessageAsync("Game " + args[0] + " added with " + (insert.Count - 1) + " keys");
                    }
                }
            }
            else if (command == Constants.Giveaway.COMMAND_CLAIMCD) {
                if (id == hosts[0] || id == hosts[1]) {
                    if (inputLen < 2) {
                        await m.Author.SendMessageAsync(Constants.INCORRECT_USE + Constants.Giveaway.Help.CLAIMCD);
                    }
                    else {
                        int idx = 0;
                        if (!Int32.TryParse(toParse[1], out idx)) {
                            await m.Author.SendMessageAsync(Constants.INCORRECT_USE + Constants.Giveaway.Help.CLAIMCD);
                        }
                        else {
                            CLAIM_CD = idx;
                            userTimeCD.Clear();
                            int minute = idx / 60;
                            int second = idx % 60;
                            await m.Author.SendMessageAsync("Time interval between game key claiming has been set to " + minute + ":" + second + ".");
                        }
                    }
                }
            }
            else if (command == Constants.Giveaway.COMMAND_CLEAR) {
                if (id == hosts[0] || id == hosts[1]) {
                    if (inputLen < 2) {
                        await m.Author.SendMessageAsync(Constants.Giveaway.Help.CLEAR);
                    }
                    else {
                        if (toParse[1] == Constants.Giveaway.COMMAND_CLEAR) {
                            foreach (List<string> ls in games) {
                                ls.Clear();
                            }
                            games.Clear();
                            saveKeylist();
                            await m.Author.SendMessageAsync("Games and keyset cleared.");
                        } else {
                            await m.Author.SendMessageAsync(Constants.Giveaway.Help.CLEAR);
                        }
                    }
                }
            }
            else if (command == Constants.Giveaway.COMMAND_LISTCLAIMS) {
                if (id == hosts[0] || id == hosts[1]) {
                    if (claimMap.Keys.Count == 0)
                        await m.Author.SendMessageAsync("No keys have been claimed yet.");
                    else {
                        foreach (string user in claimMap.Keys) {
                            string ret = "```" + user + " has claimed the following keys: \n";
                            foreach (KeyGamePair kgp in claimMap[user]) {
                                ret += "\"" + kgp.game + "\": " + kgp.key + "\n";
                            }
                            ret += "```";
                            await m.Author.SendMessageAsync(ret);
                        }
                    }
                }
            }
            else if (command == Constants.Giveaway.COMMAND_REMOVE) {
                if (id == hosts[0] || id == hosts[1]) {
                    if (inputLen < 2) {
                        await m.Author.SendMessageAsync(Constants.INCORRECT_USE + Constants.Giveaway.Help.REMOVE);
                    }
                    else {
                        int idx = 0;
                        if (!Int32.TryParse(toParse[1], out idx) || idx < 1 || idx > games.Count) {
                            await m.Author.SendMessageAsync(Constants.INCORRECT_USE + Constants.Giveaway.Help.REMOVE);
                        }
                        else {
                            string title = games[idx - 1][0];
                            games.RemoveAt(idx - 1);
                            saveKeylist();
                            await m.Author.SendMessageAsync("The game \"" + title + "\" has been removed from the listing.");
                        }
                    }
                }
            }
            else if (command == Constants.Giveaway.COMMAND_ERASEKEYS) {
                if (id == hosts[0] || id == hosts[1]) {
                    if (inputLen < 2) {
                        await m.Author.SendMessageAsync(Constants.INCORRECT_USE + Constants.Giveaway.Help.ERASEKEYS);
                    }
                    else {
                        int idx = 0;
                        if (!Int32.TryParse(toParse[1], out idx) || idx < 1 || idx > games.Count) {
                            await m.Author.SendMessageAsync(Constants.INCORRECT_USE + Constants.Giveaway.Help.ERASEKEYS);
                        }
                        else {
                            string title = games[idx - 1][0];
                            games[idx - 1].Clear();
                            games[idx - 1].Add(title);
                            saveKeylist();
                            await m.Author.SendMessageAsync("The game \"" + title + "\" has had its keys cleared.");
                        }
                    }
                }
            }
            else if (command == Constants.Giveaway.COMMAND_LISTKEYS) {
                if (id == hosts[0] || id == hosts[1]) {
                    if (inputLen < 2) {
                        await m.Author.SendMessageAsync(Constants.INCORRECT_USE + Constants.Giveaway.Help.LISTKEYS);
                    }
                    else {
                        int idx = 0;
                        if (!Int32.TryParse(toParse[1], out idx) || idx < 1 || idx > games.Count) {
                            await m.Author.SendMessageAsync(Constants.INCORRECT_USE + Constants.Giveaway.Help.LISTKEYS);
                        }
                        else {
                            string title = games[idx - 1][0];
                            string ret = "```Keys for the game \"" + title + "\"\n\n";

                            for (int i = 1; i < games[idx-1].Count; i++) {
                                ret += games[idx - 1][i] + "\n";
                            }
                            ret += "```";

                            await m.Author.SendMessageAsync(ret);
                        }
                    }
                }
            }
            else if (command == Constants.Giveaway.COMMAND_SAVE) {
                saveKeylist();
                await m.Channel.SendMessageAsync(Constants.DEBUG + "games list saved");
            }
            else if (command == Constants.Giveaway.COMMAND_LIST) {
                string ret = "```";
                int minute = CLAIM_CD / 60;
                int second = CLAIM_CD % 60;
                ret += "The time interval between claiming games is " + minute + " minutes and " + second + " seconds.\n\n";
                if (games.Count == 0) {
                    ret += "No games to be listed.";
                }
                else {
                    for (int i = 0; i < games.Count; i++) {
                        ret += (i + 1) + ". \"" + games[i][0] + "\" (" + (games[i].Count - 1) + " keys remaining)\n";
                    }
                }
                ret += "```";
                await m.Channel.SendMessageAsync(ret);
            }
            else if (command == Constants.Giveaway.COMMAND_HELP) {
                string ret = "```" +
                    Constants.Giveaway.COMMAND_CLAIM + "\n" +
                    Constants.Giveaway.COMMAND_LIST + "\n\n" +
                    "The following commands can only be used by the giveaway host: \n\n" +
                    Constants.Giveaway.COMMAND_POST + "\n" +
                    Constants.Giveaway.COMMAND_ADD + "\n" +
                    Constants.Giveaway.COMMAND_CLEAR + "\n" +
                    Constants.Giveaway.COMMAND_REMOVE + "\n" +
                    Constants.Giveaway.COMMAND_ERASEKEYS + "\n" +
                    Constants.Giveaway.COMMAND_LISTKEYS + "```";
                await m.Channel.SendMessageAsync(ret);
            }
        }

        private string claimGameKey(int idx, ulong id, string user) {
            string ret = "Invalid index!";
            int i = idx - 1;
            if (i < 0 || i >= games.Count)
                return ret;
            if (games[i].Count < 2) {
                ret = "There are no more keys left for the game \"" + games[i][0] + "\"";
            }
            else {
                int end = games[i].Count - 1;
                string key = games[i][end];
                games[i].RemoveAt(end);
                ret = "Here is a key for the game \"" + games[i][0] + "\": ```" + key + "```";
                userTimeCD.Add(id, CLAIM_CD); //Add user on cooldown upon successful redemption of key.
                if (!claimMap.ContainsKey(user)) {
                    List<KeyGamePair> kgp = new List<KeyGamePair>();
                    KeyGamePair pair = new KeyGamePair();
                    pair.game = games[i][0];
                    pair.key = key;
                    kgp.Add(pair);

                    claimMap.Add(user, kgp);
                } else {
                    KeyGamePair pair = new KeyGamePair();
                    pair.game = games[i][0];
                    pair.key = key;
                    claimMap[user].Add(pair);
                }
                saveClaimlist();
                saveKeylist();
            }
            return ret;
        }

        private void saveIntervallist() {
            int arrLen = 0;
            int listLen = userTimeCD.Count();
            arrLen += 4; //Integer for the list length
            arrLen += (12 * listLen);

            BinSerializer serializer = new BinSerializer(arrLen);

            serializer.writeInt(listLen);
            foreach (ulong id in userTimeCD.Keys) {
                serializer.writeUInt64(id);
                serializer.writeInt(userTimeCD[id]);
            }

            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }

            string filepath = Path.Combine(path, intervalFile);
            File.WriteAllBytes(filepath, serializer.data);
        }

        private void loadIntervallist() {
            string filepath = Path.Combine(path, intervalFile);

            if (!File.Exists(filepath))
                return;

            userTimeCD.Clear();
            byte[] read = File.ReadAllBytes(filepath);
            BinReader reader = new BinReader(read);

            int count = reader.readInt();
            for (int i = 0; i < count; i++) {
                ulong id = reader.readUInt64();
                int time = reader.readInt();
                userTimeCD.Add(id, time);
            }
        }

        private void saveClaimlist() {
            int arrLen = 0;

            int list1len = claimMap.Count;
            arrLen += 4; //Integer for the superlist length

            foreach (string name in claimMap.Keys) {
                arrLen += 8; //2 ints for name length and KeyGamePair list length
                arrLen += Encoding.UTF8.GetByteCount(name);
                foreach (KeyGamePair p in claimMap[name]) {
                    arrLen += p.getSerializableSize();
                }
            }

            BinSerializer serializer = new BinSerializer(arrLen);
            serializer.writeInt(list1len);
            foreach (string name in claimMap.Keys) {
                serializer.writeUTF8String(name);
                serializer.writeInt(claimMap[name].Count);
                foreach (KeyGamePair p in claimMap[name]) {
                    p.serialize(serializer);
                }
            }

            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }

            string filepath = Path.Combine(path, claimantFile);
            File.WriteAllBytes(filepath, serializer.data);
        }

        private void loadClaimlist() {
            string filepath = Path.Combine(path, claimantFile);

            if (!File.Exists(filepath))
                return;

            claimMap.Clear();
            byte[] read = File.ReadAllBytes(filepath);
            BinReader reader = new BinReader(read);

            int claimantCount = reader.readInt();
            for (int i = 0; i < claimantCount; i++) {
                string name = reader.readUTF8String();
                claimMap.Add(name, new List<KeyGamePair>());
                int len = reader.readInt();
                for (int j = 0; j < len; j++) {
                    KeyGamePair kgp = new KeyGamePair();
                    kgp.deserialize(reader);
                    claimMap[name].Add(kgp);
                }
            }
        }

        private void saveKeylist() {
            int arrLen = 0;

            int list1len = games.Count;
            arrLen += 8; //integer for the superlist length and the interval time at end

            arrLen += 4 * list1len; //integers for the length of each sublist

            for (int i = 0; i < list1len; i++) {
                arrLen += 4 * games[i].Count; //integers for the length of each string
                foreach (string s in games[i]) {
                    int slen = Encoding.UTF8.GetByteCount(s);
                    arrLen += slen; //actual size of the string;
                }
            }
            BinSerializer serializer = new BinSerializer(arrLen);
            serializer.writeInt(list1len);
            foreach (List<string> ls in games) {
                serializer.writeInt(ls.Count);
                foreach (string s in ls) {
                    serializer.writeUTF8String(s);
                }
            }

            serializer.writeInt(CLAIM_CD);

            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }

            string filepath = Path.Combine(path, gamesSaveFile);
            File.WriteAllBytes(filepath, serializer.data);
        }

        private void loadKeylist() {
            string filepath = Path.Combine(path, gamesSaveFile);

            if (!File.Exists(filepath))
                return;

            games.Clear();
            byte[] gamesList = File.ReadAllBytes(filepath);
            BinReader reader = new BinReader(gamesList);

            int gamesCount = reader.readInt();
            for (int i = 0; i < gamesCount; i++) {
                int sublistCount = reader.readInt();
                List<string> ls = new List<string>();
                for (int j = 0; j < sublistCount; j++) {
                    string str = reader.readUTF8String();
                    ls.Add(str);
                }
                games.Add(ls);
            }

            CLAIM_CD = reader.readInt();
        }
    }
}
