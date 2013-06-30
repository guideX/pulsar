#region Namespace Imports
using System;
using System.Collections.Generic;
#endregion
namespace Pulsar.Commands {
    public abstract class CommandBase {
        #region Ctor
        public CommandBase(string name, int nArgs) {
            if(String.IsNullOrEmpty(name))
                throw new InvalidOperationException("Can not create a command with the specified name.");
            NumArgs = nArgs;
            Name = name;
        }
        #endregion
        #region Memebers
        public string Name { get; private set; }
        public abstract string Help { get; set; }
        public abstract AccessRights AccessRights { get; set; }
        public int NumArgs { get; private set; }
        #endregion
        #region Implementations
        public abstract bool CanExecute(String args);
        public abstract void Execute(String args);
        #endregion
    }
    public class Command : CommandBase {
        #region Ctor
        public Command(string name, int nArgs, Action<IList<Object>> exec, Predicate<IList<Object>> canExec)
            : base(name, nArgs) {
        }
        #endregion
        #region ** CommandBase
        public override string Help {
            get;
            set;
        }
        public override AccessRights AccessRights {
            get;
            set;
        }
        public override bool CanExecute(String args) {
            if(_canExec == null)
                return true;
            return _canExec(args);
        }
        public override void Execute(String args) {
            if(_exec == null)
                return;
            _exec(args);
        }
        #endregion
        #region Fields
        private Predicate<String> _canExec = null;
        private Action<String> _exec = null;
        #endregion
    }
}