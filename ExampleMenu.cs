using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SvetilkaBot
{
    internal class ExampleMenu : AbstractMenu
    {
        public ExampleMenu(ITelegramBotClient botClient, Chat chat, CancellationToken cancellationToken) : base(botClient, chat, cancellationToken)
        {
        }

        public override void Handle()
        {
            throw new NotImplementedException();
        }

        public override void PrintMenu()
        {
            throw new NotImplementedException();
        }
    }
}
