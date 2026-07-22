using Discord.Interactions;

namespace SovereigntyBot.Modules.Results
{
    public class RuntimeErrorResult : RuntimeResult
    {
        public RuntimeErrorResult(InteractionCommandError? error, string reason) : base(error, reason)
        {
        }

        public static RuntimeErrorResult FromError(string reason)
            => new RuntimeErrorResult(InteractionCommandError.Unsuccessful, reason);
        public static RuntimeErrorResult FromSuccess(string reason = null)
            => new RuntimeErrorResult(null, reason);
    }
}