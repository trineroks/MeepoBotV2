using System;
using System.Net.Http;
using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MeepoBotV2 {
    class OpenDotaModule {
        Dictionary<ulong, string> users = new Dictionary<ulong, string>();

        string[] ranks = new string[] { "N/A", "Herald", "Guardian", "Crusader", "Archon", "Legend", "Ancient", "Divine" };

        private class WinLoss {
            public int win;
            public int lose;
        }

        public class MmrEstimate {
            public int? estimate { get; set; }
            public int? stdDev { get; set; }
            public int n { get; set; }
        }

        public class Profile {
            public int account_id { get; set; }
            public string personaname { get; set; }
            public string name { get; set; }
            public int cheese { get; set; }
            public string steamid { get; set; }
            public string avatar { get; set; }
            public string avatarmedium { get; set; }
            public string avatarfull { get; set; }
            public string profileurl { get; set; }
            public string last_login { get; set; }
            public string loccountrycode { get; set; }
        }

        public class UserObject {
            public string tracked_until { get; set; }
            public string solo_competitive_rank { get; set; }
            public string competitive_rank { get; set; }
            public int? rank_tier { get; set; }
            public int? leaderboard_rank { get; set; }
            public MmrEstimate mmr_estimate { get; set; }
            public Profile profile { get; set; }
        }

        public async Task handleInput(SocketMessage m) {
            string input = m.ToString();
            string[] toParse = input.Split(' ');
            string command = toParse[0];
            int inputLen = toParse.Length;
            if (command == "")
                return;
            else if (command == Constants.Dota.COMMAND_PAIR) {
                if (inputLen > 2 || inputLen < 2) {
                    await m.Channel.SendMessageAsync("Incorrect usage. Use " + Constants.Dota.COMMAND_PAIR + " your SteamID.");
                }
                else {
                    users.Add(m.Author.Id, toParse[1]);
                    await m.Channel.SendMessageAsync(m.Author.Mention + " has been paired with SteamID " + toParse[1]);
                }
            }
            else if (command == Constants.Dota.COMMAND_GETPROFILE) {
                if (users.ContainsKey(m.Author.Id)) {
                    string id = users[m.Author.Id];

                    string s = await callOpenDotaForProfile(id);
                    UserObject user = JsonConvert.DeserializeObject<UserObject>(s);

                    s = await callOpenDotaForWinLoss(id);
                    WinLoss wl = JsonConvert.DeserializeObject<WinLoss>(s);

                    float fwinrate = (((float)wl.win) / ((float)wl.win + (float)wl.lose));
                    fwinrate *= 100;
                    string winrate = fwinrate.ToString("0.00");
                    string rank = getRank(user.rank_tier);
                    await m.Channel.SendMessageAsync(m.Author.Mention + " here is your Dota 2 profile: ```" +
                        "Nickname: " + user.profile.personaname + "\n" +
                        "Rank: " + rank + "\n" +
                        "Wins: " + wl.win + " Losses: " + wl.lose + " Winrate: " + winrate + "%" + "\n" +
                        "```" + "\n" +
                        "Full profile at: https://www.opendota.com/players/" + id);
                }
            }
        }

        private string getRank(int? rank) {
            string ret = "";
            if (rank == null)
                ret += ranks[0];
            else {
                int stars = (int)rank % 10;
                int rankIndex = (int)rank / 10;
                ret += ranks[rankIndex] + " [" + stars + "]";
            }
            return ret;
        }

        private async Task<string> callOpenDotaForWinLoss(string id) {
            var client = new HttpClient();
            string URL = Constants.URL_OPENDOTA + "players/" + id + "/wl";
            HttpResponseMessage r = await client.GetAsync(URL);
            r.EnsureSuccessStatusCode();
            string ret = await r.Content.ReadAsStringAsync();
            return ret;
        }

        private async Task<string> callOpenDotaForProfile(string id) {
            var client = new HttpClient();
            string URL = Constants.URL_OPENDOTA + "players/" + id;
            HttpResponseMessage r = await client.GetAsync(URL);
            r.EnsureSuccessStatusCode();
            string ret = await r.Content.ReadAsStringAsync();
            return ret;
        }
    }
}

//private async Task<string> testCallOpenDota() {
//    var client = new HttpClient();
//    HttpResponseMessage r = await client.GetAsync(Constants.URL_OPENDOTA);
//    r.EnsureSuccessStatusCode();
//    string ret = await r.Content.ReadAsStringAsync();
//    return ret;
//}
