using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeepoBotV2 {
    class Constants {
        public const long SECOND = 1000;

        public const string URL_OPENDOTA = "https://api.opendota.com/api/";

        public const string COMMAND_PREFIX = "!m";
        public const string URL_GITHUB = "https://github.com/trineroks/MeepoBotV2";
        public const string CREATOR = "trineroks";

        public const bool TESTBUILD = false;

        public const string DEBUG = "DEBUG: ";

        public const string INCORRECT_USE = "Incorrect usage. ";

        public const string COMMAND_GITHUB = COMMAND_PREFIX + "git";
        public const string COMMAND_HELP = COMMAND_PREFIX + "help";
        public const string COMMAND_SETGAME = COMMAND_PREFIX + "setgame";
        public const string COMMAND_SETNICK = COMMAND_PREFIX + "setnick";
        public const string COMMAND_FLOODMESSAGE = COMMAND_PREFIX + "flood";
        public const string COMMAND_LISTUSERS = COMMAND_PREFIX + "listuser";
        public const string COMMAND_TTS = COMMAND_PREFIX + "tts";
        public const string COMMAND_PERMISSION = COMMAND_PREFIX + "perm";
        public const string COMMAND_CREATECHANNEL = COMMAND_PREFIX + "channel";
        public const string COMMAND_SETROLE = COMMAND_PREFIX + "setrole";
        public const string COMMAND_ID = COMMAND_PREFIX + "id";

        public const string COMMAND_DOTA = COMMAND_PREFIX + "dota";

        public class Dota {
            public const string DOTA_COMMAND_PREFIX = "d";
            public const string COMMAND_PAIR = COMMAND_PREFIX + DOTA_COMMAND_PREFIX + "pair";
            public const string COMMAND_GETPROFILE = COMMAND_PREFIX + DOTA_COMMAND_PREFIX + "profile";
        }

        public class Giveaway {
            public const string GIVEAWAY_COMMAND_PREFIX = "g";
            public const string COMMAND_CLAIM = COMMAND_PREFIX + GIVEAWAY_COMMAND_PREFIX + "claim";
            public const string COMMAND_LIST = COMMAND_PREFIX + GIVEAWAY_COMMAND_PREFIX + "list";
            public const string COMMAND_POST = COMMAND_PREFIX + GIVEAWAY_COMMAND_PREFIX + "post";
            public const string COMMAND_ADD = COMMAND_PREFIX + GIVEAWAY_COMMAND_PREFIX + "add";
            public const string COMMAND_SAVE = COMMAND_PREFIX + GIVEAWAY_COMMAND_PREFIX + "save";
            public const string COMMAND_CLEAR = COMMAND_PREFIX + GIVEAWAY_COMMAND_PREFIX + "clear";
            public const string COMMAND_REMOVE = COMMAND_PREFIX + GIVEAWAY_COMMAND_PREFIX + "remove";
            public const string COMMAND_ERASEKEYS = COMMAND_PREFIX + GIVEAWAY_COMMAND_PREFIX + "erasekey";
            public const string COMMAND_LISTKEYS = COMMAND_PREFIX + GIVEAWAY_COMMAND_PREFIX + "listkeys";
            public const string COMMAND_CLAIMCD = COMMAND_PREFIX + GIVEAWAY_COMMAND_PREFIX + "interval";
            public const string COMMAND_LISTCLAIMS = COMMAND_PREFIX + GIVEAWAY_COMMAND_PREFIX + "listclaims";

            public const string COMMAND_HELP = COMMAND_PREFIX + GIVEAWAY_COMMAND_PREFIX + "help";

            public class Help {

                public const string CLAIMCD = "Use " + Constants.Giveaway.COMMAND_CLAIMCD + " time in seconds " +
                    " to set the interval period that each participant will have in between game key claiming. Default is " +
                    "1800 seconds (30 minutes). Example: " + COMMAND_CLAIMCD + " 300";
                public const string CLAIM = "Use " + Constants.Giveaway.COMMAND_CLAIM + 
                    " the numerical index of the game using the command " + Constants.Giveaway.COMMAND_LIST + 
                    "Example: " + COMMAND_CLAIM + "2";
                public const string LIST = "";
                public const string POST = "Use " + Constants.Giveaway.COMMAND_POST +
                            " the game title in quotes followed by any number of keys in quotes. Example: " + Constants.Giveaway.COMMAND_POST +
                            " \"Game Name\" \"Key1\" \"Key2\" ...";
                public const string ADD = "Use " + Constants.Giveaway.COMMAND_ADD +
                            " the game listing number followed by any number of keys in quotes. Example: " + Constants.Giveaway.COMMAND_ADD +
                            " 1 \"Key1\" \"Key2\" ...";
                public const string SAVE = "";
                public const string CLEAR = "WARNING: This will clear the entire game key list that you've created. If this is something you wish to do, please" +
                            "reenter the command and repeat it. Example: " + Constants.Giveaway.COMMAND_CLEAR + " " + Constants.Giveaway.COMMAND_CLEAR;
                public const string REMOVE = "Use " + Constants.Giveaway.COMMAND_REMOVE +
                            " the game listing number to remove the game from the listing. Example: " + Constants.Giveaway.COMMAND_REMOVE +
                            " 1";
                public const string ERASEKEYS = "Use " + Constants.Giveaway.COMMAND_ERASEKEYS +
                            " the game listing number to remove all keys corresponding to the game at that listing. Example: " + Constants.Giveaway.COMMAND_ERASEKEYS +
                            " 1";
                public const string LISTKEYS = "Use " + Constants.Giveaway.COMMAND_LISTKEYS +
                            " the game listing number to show all keys currently on that listing. Example: " + Constants.Giveaway.COMMAND_LISTKEYS +
                            " 1";
            }
        }
    }
}
