using Discord;
using Discord.WebSocket;
using SovereigntyBot.Services.Endpoints.Cache;
using SovereigntyBot.Services;

namespace SovereigntyBot.Handlers
{
    public static class ButtonHandler
    {
        private static SocketMessageComponent _messageComponent;
        private static MessageComponentData _data;
        public static async Task Handle(SocketMessageComponent messageComponent)
        {
            _messageComponent = messageComponent;
            _data = LoadMessageComponentData();
            switch(messageComponent.Data.CustomId)
            {
                case "tcp-get":
                    await ButtonHandler.UpdateTableControlPanelAsync();
                    break;
                case "tcp-back":
                    await ButtonHandler.GetPreviousComponentAsync();
                    break;
                case "tcp-rotate-right":
                    await ButtonHandler.HorizontalRotateAsync(1);
                    break;
                case "tcp-rotate-left":
                    await ButtonHandler.HorizontalRotateAsync(-1);
                    break;
                case "tcp-rotate-up":
                    await ButtonHandler.VerticalRotateAsync(-1);
                    break;
                case "tcp-rotate-down":
                    await ButtonHandler.VerticalRotateAsync(1);
                    break;
                case "tcp-lock-field-1":
                    await ButtonHandler.SetLockAsync(true, 0);
                    break;
                case "tcp-lock-field-2":
                    await ButtonHandler.SetLockAsync(true, 1);
                    break;
                case "tcp-lock-field-3":
                    await ButtonHandler.SetLockAsync(true, 2);
                    break;
                // CAN IMPROVE LOCKING SYSTEM HERE ( removing unlock and using not operator )
                case "tcp-unlock-field-1":
                    await ButtonHandler.SetLockAsync(false, 0);
                    break;
                case "tcp-unlock-field-2":
                    await ButtonHandler.SetLockAsync(false, 1);
                    break;
                case "tcp-unlock-field-3":
                    await ButtonHandler.SetLockAsync(false, 2);
                    break; 
                case "tcp-reset":
                    await ButtonHandler.ResetFieldsAsync();
                    break;
                default:
                    await Program.Log(new LogMessage(LogSeverity.Error, "ButtonHandler.cs", "ButtonHandler Defaulted, custom-id is incorrect?"));
                    break;
            }
            _messageComponent = null;
        }

        private static MessageComponentData LoadMessageComponentData()
            => (MessageComponentData)Program.CacheService.Load(_messageComponent.Message.Id).Data; 

        private static async Task UpdateTableEmbedAsync(MessageComponentData data, ComponentBuilder wComponent)
        {
            MessageComponentData data2 = EmbedService.ModifyEmbedWithTable(data.wEmbed, data.table);
            await _messageComponent.UpdateAsync(x => { x.Embed = data2.wEmbed.Build(); x.Components = wComponent.Build(); });
        }

        private static async Task UpdateTableControlPanelAsync()
        {
            await UpdateTableEmbedAsync(_data, _data.table.GetControlPanel());
        }

        private static async Task GetPreviousComponentAsync()
        {
            await UpdateTableEmbedAsync(_data, _data.wComponent);
        }

        private static async Task HorizontalRotateAsync(int direction)
        {
            _data.table.RotateHorizontal(direction, false);
            await UpdateTableControlPanelAsync();
        }

        private static async Task VerticalRotateAsync(int direction)
        {
            _data.table.RotateVertical(direction, false);
            await UpdateTableControlPanelAsync();
        }

        private static async Task SetLockAsync(bool Lock, int fieldIndex)
        {
            _data.table.SetLock(Lock, fieldIndex);
            await UpdateTableControlPanelAsync();
        }

        private static async Task ResetFieldsAsync()
        {
            _data.table.Reset();
            await UpdateTableControlPanelAsync();
        }
    }
}