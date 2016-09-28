﻿using System;
using System.Linq;
using System.Threading;

using BrowserLog.TinyServer;

using log4net;
using log4net.Config;
using log4net.Layout;

using NSubstitute;

using NUnit.Framework;

namespace BrowserLog
{
    public class BrowserConsoleAppenderTest
    {
        private IEventChannel _channel;

        private BrowserConsoleAppender _appender;

        [SetUp]
        public void ConfigureLogger()
        {
            var layout = new PatternLayout("%-4timestamp [%thread] %-5level %logger %ndc - %message%newline");

            var channelFactory = Substitute.For<ChannelFactory>();
            _channel = Substitute.For<IEventChannel>();
            channelFactory.Create("localhost", 8765, 1).Returns(_channel);
            _appender = new BrowserConsoleAppender(channelFactory)
            {
                Host = "localhost",
                Port = 8765,
                Layout = layout,
                Buffer = 1
            };
            layout.ActivateOptions();
        }

        [Test]
        public void Should_have_no_side_effect_if_active_flag_set_to_false()
        {
            // given
            _appender.ActivateOptions();
            BasicConfigurator.Configure(_appender);

            // when
            LogManager.GetLogger(GetType()).Info("Everything's fine");

            // then
            _channel.DidNotReceive().Send(Arg.Any<ServerSentEvent>(), Arg.Any<CancellationToken>());
        }

        [Test]
        public void Should_send_an_sse_message_when_receiving_a_logging_event()
        {
            // given
            _appender.Active = true;
            _appender.ActivateOptions();
            BasicConfigurator.Configure(_appender);

            // when
            LogManager.GetLogger(GetType()).Info("Everything's fine");

            // then
            _channel.Received()
                .Send(
                    Arg.Is<ServerSentEvent>(evt => evt.ToString().Contains("Everything's fine")),
                    Arg.Any<CancellationToken>());
        }

        [Test]
        public void Should_send_an_sse_message_with_a_type_matching_received_logging_event_level()
        {
            // given
            _appender.Active = true;
            _appender.ActivateOptions();
            BasicConfigurator.Configure(_appender);

            // when
            LogManager.GetLogger(GetType()).Warn("level?");

            // then
            _channel.Received()
                .Send(
                    Arg.Is<ServerSentEvent>(evt => evt.ToString().StartsWith("event: WARN")),
                    Arg.Any<CancellationToken>());
        }

        [Test]
        public void
            Should_send_an_sse_message_without_type_when_received_logging_event_level_has_no_matching_level_on_browser()
        {
            // given
            _appender.Active = true;
            _appender.ActivateOptions();
            BasicConfigurator.Configure(_appender);

            // when
            LogManager.GetLogger(GetType()).Fatal("No fatal logs on the browser");

            // then
            _channel.Received()
                .Send(Arg.Is<ServerSentEvent>(evt => evt.ToString().StartsWith("data:")), Arg.Any<CancellationToken>());
        }

        [Test]
        public void Should_send_a_multiline_sse_message_received_logging_event_for_an_exception()
        {
            // given
            _appender.Active = true;
            _appender.ActivateOptions();
            BasicConfigurator.Configure(_appender);

            // when
            LogManager.GetLogger(GetType()).Warn("An error has occured", new Exception());

            // then
            var lineSeparator = new[] { "\r\n" };
            _channel.Received()
                .Send(
                    Arg.Is<ServerSentEvent>(
                        evt =>
                        evt.ToString()
                            .Split(lineSeparator, StringSplitOptions.RemoveEmptyEntries)
                            .Skip(1)
                            .All(l => l.StartsWith("data:"))),
                    Arg.Any<CancellationToken>());
        }

        [Test]
        public void Should_dispose_channel_on_shutdown()
        {
            // given
            _appender.Active = true;
            _appender.ActivateOptions();
            BasicConfigurator.Configure(_appender);

            // when
            LogManager.Shutdown();

            // then
            _channel.Received().Dispose();
        }
    }
}