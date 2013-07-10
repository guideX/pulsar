using System;
using System.Collections.Generic;
using System.Text;
using Sys = Cosmos.System;
using Pulsar.Environment;
using Pulsar.FileSystem.Physical.Drivers;
using Pulsar.Commands;
using Pulsar.FileSystem.PulsarFileSystem;
using Cosmos.Hardware;
namespace Pulsar {
    public class Kernel : Sys.Kernel {
        protected override void BeforeRun() {
            int initres = 0;
            try {
                initres = Init.RunInit();
            } catch(Exception ex) {
                Helper.Error("Error! " + ex.Message);
                this.Stop();
                return;
            }
            if(Init.Error) {
                this.Stop();
                return;
            }
            if(initres == 1) {
                //Power Off
                this.Stop();
            }
            if(initres == 2) {
                //Reboot
                this.Stop();
            }
        }
        protected override void Run() {
            while(true) {
                Login();
            }
        }
        private void Login() {
            Security.Account acc;
            do {
                acc = Security.Account.DoLogin();
            } while(!acc.OK);
            GlobalEnvironment.Init(acc);
            CommandInit.Init();
            Shell.StartShell();
        }
        protected override void AfterRun() {

        }
    }
}