using System;
using System.IO;

namespace MeepoBotV2 {
    static class Constants {
        public static readonly string DATAPATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MeepoBotData");

        public static readonly long SECOND = 1000;

        public static readonly string URL_OPENDOTA = "https://api.opendota.com/api/";

        public static readonly string COMMAND_PREFIX = "!m";
        public static readonly string URL_GITHUB = "https://github.com/trineroks/MeepoBotV2";
        public static readonly string CREATOR = "trineroks";

        public static readonly bool TESTBUILD = false;

        public static readonly string DEBUG = "DEBUG: ";

        public static readonly string INCORRECT_USE = "Incorrect usage. ";

        public static readonly string COMMAND_GITHUB = COMMAND_PREFIX + "git";
        public static readonly string COMMAND_HELP = COMMAND_PREFIX + "help";
        public static readonly string COMMAND_SETGAME = COMMAND_PREFIX + "setgame";
        public static readonly string COMMAND_SETNICK = COMMAND_PREFIX + "setnick";
        public static readonly string COMMAND_FLOODMESSAGE = COMMAND_PREFIX + "flood";
        public static readonly string COMMAND_LISTUSERS = COMMAND_PREFIX + "listuser";
        public static readonly string COMMAND_TTS = COMMAND_PREFIX + "tts";
        public static readonly string COMMAND_PERMISSION = COMMAND_PREFIX + "perm";
        public static readonly string COMMAND_CREATECHANNEL = COMMAND_PREFIX + "channel";
        public static readonly string COMMAND_SETROLE = COMMAND_PREFIX + "setrole";
        public static readonly string COMMAND_ID = COMMAND_PREFIX + "id";

        public static readonly string COMMAND_DOTA = COMMAND_PREFIX + "dota";

        public class Dota {
            public static readonly string DOTA_COMMAND_PREFIX = "d";
            public static readonly string COMMAND_PAIR = COMMAND_PREFIX + DOTA_COMMAND_PREFIX + "pair";
            public static readonly string COMMAND_GETPROFILE = COMMAND_PREFIX + DOTA_COMMAND_PREFIX + "profile";
        }

        public class Giveaway {
            public static readonly string GIVEAWAY_COMMAND_PREFIX = "g";
            public static readonly string COMMAND_CLAIM = COMMAND_PREFIX + GIVEAWAY_COMMAND_PREFIX + "claim";
            public static readonly string COMMAND_LIST = COMMAND_PREFIX + GIVEAWAY_COMMAND_PREFIX + "list";
            public static readonly string COMMAND_POST = COMMAND_PREFIX + GIVEAWAY_COMMAND_PREFIX + "post";
            public static readonly string COMMAND_ADD = COMMAND_PREFIX + GIVEAWAY_COMMAND_PREFIX + "add";
            public static readonly string COMMAND_SAVE = COMMAND_PREFIX + GIVEAWAY_COMMAND_PREFIX + "save";
            public static readonly string COMMAND_CLEAR = COMMAND_PREFIX + GIVEAWAY_COMMAND_PREFIX + "clear";
            public static readonly string COMMAND_REMOVE = COMMAND_PREFIX + GIVEAWAY_COMMAND_PREFIX + "remove";
            public static readonly string COMMAND_ERASEKEYS = COMMAND_PREFIX + GIVEAWAY_COMMAND_PREFIX + "erasekey";
            public static readonly string COMMAND_LISTKEYS = COMMAND_PREFIX + GIVEAWAY_COMMAND_PREFIX + "listkeys";
            public static readonly string COMMAND_CLAIMCD = COMMAND_PREFIX + GIVEAWAY_COMMAND_PREFIX + "interval";
            public static readonly string COMMAND_LISTCLAIMS = COMMAND_PREFIX + GIVEAWAY_COMMAND_PREFIX + "listclaims";

            public static readonly string COMMAND_HELP = COMMAND_PREFIX + GIVEAWAY_COMMAND_PREFIX + "help";

            public class Help {

                public static readonly string CLAIMCD = "Use " + Constants.Giveaway.COMMAND_CLAIMCD + " time in seconds " +
                    " to set the interval period that each participant will have in between game key claiming. Default is " +
                    "1800 seconds (30 minutes). Example: " + COMMAND_CLAIMCD + " 300";
                public static readonly string CLAIM = "Use " + Constants.Giveaway.COMMAND_CLAIM + 
                    " the numerical index of the game using the command " + Constants.Giveaway.COMMAND_LIST + 
                    "Example: " + COMMAND_CLAIM + "2";
                public static readonly string LIST = "";
                public static readonly string POST = "Use " + Constants.Giveaway.COMMAND_POST +
                            " the game title in quotes followed by any number of keys in quotes. Example: " + Constants.Giveaway.COMMAND_POST +
                            " \"Game Name\" \"Key1\" \"Key2\" ...";
                public static readonly string ADD = "Use " + Constants.Giveaway.COMMAND_ADD +
                            " the game listing number followed by any number of keys in quotes. Example: " + Constants.Giveaway.COMMAND_ADD +
                            " 1 \"Key1\" \"Key2\" ...";
                public static readonly string SAVE = "";
                public static readonly string CLEAR = "WARNING: This will clear the entire game key list that you've created. If this is something you wish to do, please" +
                            "reenter the command and repeat it. Example: " + Constants.Giveaway.COMMAND_CLEAR + " " + Constants.Giveaway.COMMAND_CLEAR;
                public static readonly string REMOVE = "Use " + Constants.Giveaway.COMMAND_REMOVE +
                            " the game listing number to remove the game from the listing. Example: " + Constants.Giveaway.COMMAND_REMOVE +
                            " 1";
                public static readonly string ERASEKEYS = "Use " + Constants.Giveaway.COMMAND_ERASEKEYS +
                            " the game listing number to remove all keys corresponding to the game at that listing. Example: " + Constants.Giveaway.COMMAND_ERASEKEYS +
                            " 1";
                public static readonly string LISTKEYS = "Use " + Constants.Giveaway.COMMAND_LISTKEYS +
                            " the game listing number to show all keys currently on that listing. Example: " + Constants.Giveaway.COMMAND_LISTKEYS +
                            " 1";
            }
        }
    }
}
