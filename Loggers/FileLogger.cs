﻿// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WifftOCR.Services;

namespace WifftOCR.Loggers
{
    class FileLogger : ILogger
    {
        private readonly StringBuilder _logBuilder;
        private readonly string _name;

        public FileLogger(string name)
        {
            _logBuilder = new StringBuilder();
            _name = name;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (_name.Equals(typeof(OcrRecorderService).FullName)) {
                string message = formatter(state, exception);
                string logEntry = $"{DateTime.Now} - [{logLevel}]: {message}";

                _logBuilder.AppendLine(logEntry);

                Task.Run(async () => {
                    await Task.Delay(250);

                    await FileLoggerService.WriteLogFileForServiceProviderAsync(_logBuilder.ToString());
                });
            }
        }
    }
}
