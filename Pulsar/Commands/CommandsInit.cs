namespace Pulsar.Commands {
    class CommandInit {
        public static void Init() {
            CommandManager.Register(new MakeDirectory());
            CommandManager.Register(new ListDirectories());
            CommandManager.Register(new ChangeDirectory());
            CommandManager.Register(new RemoveDirectory());
            CommandManager.Register(new CatCommand());
            CommandManager.Register(new TouchCommand());
            CommandManager.Register(new HelpCommand());
            //CommandManager.Register(new ExecCommand());
            CommandManager.Register(new FileEditor());
        }
    }
}