using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Site___.Logging;
using System.Net;
using System.Collections.Specialized;

namespace Site___.Logging
{
    public class DiscordSink : ILogEventSink
    {
        private readonly string _webhook;
        public DiscordSink(string Webhook)
        {
            _webhook = Webhook;
        }

        public void Emit(LogEvent logEvent)
        {
            var message = logEvent.RenderMessage();
            WebClient Http = new WebClient();
            NameValueCollection NVal = new NameValueCollection();
            NVal["content"] = message;
            Http.UploadValues(_webhook, NVal);
        }
    }
}
public static class DiscordSinkExtension
{
    public static LoggerConfiguration Discord(this LoggerSinkConfiguration loggerConfiguration, string Webhook)
    {
        return loggerConfiguration.Sink(new DiscordSink(Webhook));
    }
}