using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pulsar.FileSystem;
using Pulsar.FileSystem.PulsarFileSystem;
namespace Pulsar.Security {
    class Account {
        private static String usseparator = "|";
        private static String uspasssep = ":";
        private static Char[] InvalidChars = new Char[] { ':', '|', ' ', '.', '-', '=', ',', '/', '\\' };
        private String _Username;
        private bool _OK;
        private String _HomeDir;
        private static Encryption.MD5 hasher = new Encryption.MD5();
        ///<summary>
        /// Used to save users text color defualt white
        /// </summary>

        public ConsoleColor textcolor = ConsoleColor.White;

        ///<summary>
        ///Used to save user background color defualt black
        /// </summary>

        public ConsoleColor backcolor = ConsoleColor.Black;
        /// <summary>
        /// The Account username
        /// </summary>
        public String Username {
            get {
                return _Username;
            }
        }

        /// <summary>
        /// Checks if this is a verified Account class 
        /// </summary>
        public bool OK {
            get {
                return _OK;
            }
        }

        /// <summary>
        /// The string that represents the path to user's home directory
        /// </summary>
        public String HomeDir {
            get {
                return _HomeDir;
            }
        }

        /// <summary>
        /// Account constructor method that check's the user credentials
        /// </summary>
        /// <param name="User">Username</param>
        /// <param name="Pass">Password</param>
        public Account(String User, String Pass) {
            PulsarFile accfile = PulsarDirectory.GetFileByFullName("/etc/passwd");
            if(accfile == null) {
                Helper.Error("/etc/passwd not available! Quitting.");
                this._OK = false;
            }
            hasher.Value = Pass;
            if(accfile.ReadAllText() == (User + uspasssep + hasher.FingerPrint + usseparator)) {
                this._Username = User;
                this._HomeDir = "/";
                this._OK = true;
                Helper.WriteLine("Login Succeded!");
            } else {
                this._OK = false;
                Helper.Error("Login Failed! Verify user And Password!");
            }
        }

        /// <summary>
        /// Account constructor method that creates a custom Account
        /// </summary>
        /// <param name="User"></param>
        public Account(string User) {
            this._Username = User;
            this._HomeDir = "/";
            this._OK = true;
        }

        /// <summary>
        /// Prompts a login interface to the user to verify its credentials
        /// </summary>
        public static Account DoLogin() {
            Helper.WriteLine("Login into Pulsar.");
            String User = Helper.ReadLine("Username: ");
            Helper.Write("Password: ");
            bool b = true;
            String Pass = Helper.ReadLine(b);
            Account a = new Account(User, Pass);
            return a;
        }

        /// <summary>
        /// Adds this new user to the Accounts file.
        /// </summary>
        /// <param name="newus">New Username</param>
        /// <param name="newpass">New Password</param>
        public static bool Add(String newus, String newpass) {
            if(!Utils.StringContains(newus, InvalidChars)) {
                PulsarFile accfile = PulsarDirectory.GetFileByFullName("/etc/passwd");
                if(accfile == null) {
                    PulsarFileSystem.mFS.Root.GetDirectoryByName("etc").AddFile("passwd");
                    accfile = PulsarDirectory.GetFileByFullName("/etc/passwd");
                }
                hasher.Value = newpass;
                accfile.WriteAllText(newus + uspasssep + hasher.FingerPrint + usseparator);
                return true;
            } else {
                return false;
            }
        }
    }
}