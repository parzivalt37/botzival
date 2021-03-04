using System;
using System.Linq;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using botzival.Services;

namespace botzival.Preconditions
{
    public class RequireRole : PreconditionAttribute
    {
        //Required role's name
        private readonly string roleName;

        //Constructor
        public RequireRole(string name) => roleName = name;

        //Override the CheckPermissions method
        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            //checks to see if it's a guild user, which is the only context where roles exist
            if (context.User is SocketGuildUser gUser)
            {
                //if this command was executed by a user with the appropriate role, return success
                if (gUser.Roles.Any(r => r.Name == roleName))
                {
                    return await Task.FromResult(PreconditionResult.FromSuccess()).ConfigureAwait(false);
                }
                else
                {
                    await Utilities.PrintError((ISocketMessageChannel)context.Channel, $"You must have the {roleName} role to run this command.");
                    return await Task.FromResult(PreconditionResult.FromError($"You must have the {roleName} role to run this command.")).ConfigureAwait(false);
                }
            }
            else
            {
                return await Task.FromResult(PreconditionResult.FromError("You must be in a guild to run this command.")).ConfigureAwait(false);
            }
        }
    }
}