using CommandLine;

namespace BackupVideos
{
    internal class Options
    {
        [Option('o', "outputDir", HelpText = "The base output directory to save the arlo files", Required = true)]
        public string OutputDir { get; set; }

        [Option('d', "days", HelpText = "The number of days (including today) that you'd like to go back and scrape from Arlo's services", Required = false)]
        public uint DaysToScrape { get; set; } = 1;

        [Option('u', "username", HelpText = "Your login username", Required = true)]
        public string Username { get; set; }

        [Option('p', "password", HelpText = "Your login password", Required = true)]
        public string Password { get; set; }
    }
}
