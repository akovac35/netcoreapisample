using System.CommandLine;
using Domain;

namespace Infrastructure.CommandLine
{
    public class SampleRootCommand : RootCommand
    {
        public SampleRootCommand() : base($"{Constants.ApplicationAcronym} - {Constants.ApplicationName}") { }
    }
}
