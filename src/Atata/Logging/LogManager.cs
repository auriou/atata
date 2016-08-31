﻿using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;

namespace Atata
{
    public class LogManager : ILogManager
    {
        private readonly List<ILogConsumer> logConsumers = new List<ILogConsumer>();
        private readonly List<IScreenshotConsumer> screenshotConsumers = new List<IScreenshotConsumer>();

        [ThreadStatic]
        private static Stack<LogSection> sectionEndStack;

        private int screenshotNumber;

        protected IWebDriver Driver
        {
            get { return ATContext.Current.Driver; }
        }

        private static Stack<LogSection> SectionEndStack
        {
            get { return sectionEndStack ?? (sectionEndStack = new Stack<LogSection>()); }
        }

        public void Info(string message, params object[] args)
        {
            Log(LogLevel.Info, message, args);
        }

        public void Warn(string message, params object[] args)
        {
            Log(LogLevel.Warn, message, args);
        }

        public void Error(string message, Exception exception)
        {
            Log(new LogEventInfo
            {
                Level = LogLevel.Error,
                Message = message,
                Exception = exception
            });
        }

        public void Start(LogSection section)
        {
            LogEventInfo eventInfo = new LogEventInfo
            {
                Level = section.Level,
                Message = $"Starting: {section.Message}",
                SectionStart = section
            };

            section.StartedAt = eventInfo.Timestamp;

            Log(eventInfo);

            SectionEndStack.Push(section);
        }

        public void EndSection()
        {
            if (SectionEndStack.Any())
            {
                LogSection section = SectionEndStack.Pop();

                TimeSpan duration = section.GetDuration();

                LogEventInfo eventInfo = new LogEventInfo
                {
                    Level = section.Level,
                    Message = $"Finished: {section.Message} ({Math.Floor(duration.TotalSeconds)}.{duration:fff}s)",
                    SectionEnd = section
                };

                Log(eventInfo);
            }
        }

        private void Log(LogLevel level, string message, object[] args)
        {
            Log(new LogEventInfo
            {
                Level = level,
                Message = message.FormatWith(args)
            });
        }

        private void Log(LogEventInfo eventInfo)
        {
            foreach (ILogConsumer logConsumer in logConsumers)
                logConsumer.Log(eventInfo);
        }

        public void Screenshot(string title = null)
        {
            if (Driver == null || !screenshotConsumers.Any())
                return;

            try
            {
                screenshotNumber++;

                Info($"Take screenshot {screenshotNumber} {title}");

                Screenshot screenshot = Driver.TakeScreenshot();

                foreach (IScreenshotConsumer screenshotConsumer in screenshotConsumers)
                {
                    try
                    {
                        screenshotConsumer.Take(screenshot, screenshotNumber, title);
                    }
                    catch (Exception e)
                    {
                        Error("Screenshot failed", e);
                    }
                }
            }
            catch (Exception e)
            {
                Error("Screenshot failed", e);
            }
        }
    }
}