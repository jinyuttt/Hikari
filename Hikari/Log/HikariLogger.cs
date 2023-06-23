﻿using Microsoft.Extensions.Logging;
using System;

namespace Hikari.Log
{
    /// <summary>
    /// 日志封装类
    /// </summary>
    public class HikariLogger
    {
       

       
        private readonly ILogger _logger;


        public HikariLogger(ILogger logger)
        {
            this._logger = logger;

        }
        public HikariLogger(ILoggerFactory loggerFactory)
        {
          
            _logger= loggerFactory.CreateLogger<HikariLogger>();

        }


        #region [ 参数 ]

        public bool IsDebugEnabled
        {
            get { return _logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Debug); }
        }
        public bool IsInfoEnabled
        {
            get { return _logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Information); }
        }
        public bool IsWarnEnabled
        {
            get { return _logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Warning); }
        }
        public bool IsErrorEnabled
        {
            get { return _logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Error); }
        }
        public bool IsFatalEnabled
        {
            get { return _logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Critical); }
        }

        #endregion

        #region [ 接口方法 ]

        #region [ Debug ]

        public void Debug(string message)
        {
            if (this.IsDebugEnabled)
            {
                this.Log(LogLevel.Debug, message);
            }
        }

        public void Debug(string message, Exception exception)
        {
            if (this.IsDebugEnabled)
            {
                this.Log(LogLevel.Debug, message, exception);
            }
        }

        public void DebugFormat(string format, params object[] args)
        {
            if (this.IsDebugEnabled)
            {
                this.Log(LogLevel.Debug, format, args);
            }
        }

        public void DebugFormat(string format, Exception exception, params object[] args)
        {
            if (this.IsDebugEnabled)
            {
                this.Log(LogLevel.Debug, string.Format(format, args), exception);
            }
        }

        #endregion

        #region [ Info ]

        public void Info(string message)
        {
            if (this.IsInfoEnabled)
            {
                this.Log(LogLevel.Info, message);
            }
        }

        public void Info(string message, Exception exception)
        {
            if (this.IsInfoEnabled)
            {
                this.Log(LogLevel.Info, message, exception);
            }
        }

        public void InfoFormat(string format, params object[] args)
        {
            if (this.IsInfoEnabled)
            {
                this.Log(LogLevel.Info, format, args);
            }
        }

        public void InfoFormat(string format, Exception exception, params object[] args)
        {
            if (this.IsInfoEnabled)
            {
                this.Log(LogLevel.Info, string.Format(format, args), exception);
            }
        }

        #endregion

        #region  [ Warn ]

        public void Warn(string message)
        {
            if (this.IsWarnEnabled)
            {
                this.Log(LogLevel.Warn, message);
            }
        }

        public void Warn(string message, Exception exception)
        {
            if (this.IsWarnEnabled)
            {
                this.Log(LogLevel.Warn, message, exception);
            }
        }

        public void WarnFormat(string format, params object[] args)
        {
            if (this.IsWarnEnabled)
            {
                this.Log(LogLevel.Warn, format, args);
            }
        }

        public void WarnFormat(string format, Exception exception, params object[] args)
        {
            if (this.IsWarnEnabled)
            {
                this.Log(LogLevel.Warn, string.Format(format, args), exception);
            }
        }

        #endregion

        #region  [ Error ]

        public void Error(string message)
        {
            if (this.IsErrorEnabled)
            {
                this.Log(LogLevel.Error, message);
            }
        }

        public void Error(string message, Exception exception)
        {
            if (this.IsErrorEnabled)
            {
                this.Log(LogLevel.Error, message, exception);
            }
        }

        public void ErrorFormat(string format, params object[] args)
        {
            if (this.IsErrorEnabled)
            {
                this.Log(LogLevel.Error, format, args);
            }
        }

        public void ErrorFormat(string format, Exception exception, params object[] args)
        {
            if (this.IsErrorEnabled)
            {
                this.Log(LogLevel.Error, string.Format(format, args), exception);
            }
        }
        #endregion

        #region  [ Fatal ]

        public void Fatal(string message)
        {
            if (this.IsFatalEnabled)
            {
                this.Log(LogLevel.Fatal, message);
            }
        }

        public void Fatal(string message, Exception exception)
        {
            if (this.IsFatalEnabled)
            {
                this.Log(LogLevel.Fatal, message, exception);
            }
        }

        public void FatalFormat(string format, params object[] args)
        {
            if (this.IsFatalEnabled)
            {
                this.Log(LogLevel.Fatal, format, args);
            }
        }

        public void FatalFormat(string format, Exception exception, params object[] args)
        {
            if (this.IsFatalEnabled)
            {
                this.Log(LogLevel.Fatal, string.Format(format, args), exception);
            }
        }
        #endregion

        #endregion

        #region [ 内部方法 ]  


        /// <summary>  
        /// 输出普通日志  
        /// </summary>  
        /// <param name="level"></param>  
        /// <param name="format"></param>  
        /// <param name="args"></param>  
        private void Log(LogLevel level, string format, params object[] args)
        {
            switch (level)
            {
                case LogLevel.Debug:

                    _logger.LogDebug(format, args);
                    break;
                case LogLevel.Info:
                    _logger.LogInformation(format, args);
                    break;
                case LogLevel.Warn:
                    _logger.LogWarning(format, args);
                    break;
                case LogLevel.Error:
                    _logger.LogError(format, args);
                    break;
                case LogLevel.Fatal:
                    _logger.LogCritical(format, args);
                    break;
            }
        }

            /// <summary>  
            /// 格式化输出异常信息  
            /// </summary>  
            /// <param name="level"></param>  
            /// <param name="message"></param>  
            /// <param name="exception"></param>  
           void Log(LogLevel level, string message, Exception exception)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    _logger.LogDebug(exception, message);
                    break;
                case LogLevel.Info:
                    _logger.LogInformation(exception, message);
                    break;
                case LogLevel.Warn:
                    _logger.LogWarning(exception, message);
                    break;
                case LogLevel.Error:
                    _logger.LogError(exception, message);
                    break;
                case LogLevel.Fatal:
                    _logger.LogCritical(exception, message);
                    break;
            }
        }


        #endregion
    }

}

