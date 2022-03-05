using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Core.Modules
{
    public class GeneralModule : ModuleBase<ICommandContext>
    {
        private readonly CommandService _service;
        public GeneralModule (CommandService service)
        {
            _service = service;
        }

        [Command("help")]
        [Summary("list of commands")]
        public async Task Help ()
        {
            List<CommandInfo> commands = _service.Commands.ToList();
            EmbedBuilder embedBuilder = new EmbedBuilder();

            foreach (CommandInfo command in commands)
            {
                string embedFieldText = command.Summary ?? "No description available\n";

                embedBuilder.AddField(command.Name, embedFieldText);
            }

            await ReplyAsync("Here's a list of commands and their description: ", false, embedBuilder.Build());
        }
    }
}
