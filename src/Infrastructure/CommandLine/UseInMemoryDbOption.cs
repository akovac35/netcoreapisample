using System.CommandLine;

namespace Infrastructure.CommandLine
{
    public static class UseInMemoryDbOption
    {
        public static bool DefaultValue { get; } = true;
        public static Option<bool> AddUseInMemoryDbOption(this SampleRootCommand rootCommand)
        {
            var option = new Option<bool>(
            name: "--useInMemoryDb",
            description: "Set to true to use in-memory database.",
            getDefaultValue: () => DefaultValue);

            rootCommand.AddOption(option);
            return option;
        }
    }
}
