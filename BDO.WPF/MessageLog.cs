using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BDO.WPF
{
    class MessageLog
    {
        //todo: extract to UTL
        //todo: implement stack tracing 

        readonly List<string> _messages = new List<string>();
        static MessageLog _messageLog;
        Queue<string> _messageQueue = new Queue<string>();
        object _queueLock = new object();
        bool _writing;
        object _writingLock = new object();

        MessageLog()
        {
            Messages = new ReadOnlyCollection<string>(_messages);
            MostRecentMessage = string.Empty;
        }

        public event EventHandler MessageAdded;

        public bool LogToFile { get; set; } = false;
        public bool TimeStampLog { get; set; } = true;
        public ReadOnlyCollection<string> Messages { get; }
        public string MostRecentMessage { get; private set; }


        public static MessageLog GetLog()
        {
            return _messageLog ?? (_messageLog = new MessageLog());
        }
        
        public void LogMessage(string message)
        {
            _messages.Add(message);
            MostRecentMessage = message;
            MessageAdded?.Invoke(this, new EventArgs());
            CheckWrite(message);
        }

        void CheckWrite(string msg)
        {
            if (!LogToFile)
                return;
            lock (_queueLock)
                _messageQueue.Enqueue(msg);
            lock (_writingLock)
            {
                if (_writing)
                    return;
                var t= new Thread(() => WriteToLog());
                t.Start();
            }
        }

        void WriteToLog()
        {
            lock (_writingLock)
            {
                _writing = true;
            }

            int count = 0;

            while (true)
            {
                string msg = string.Empty;
                lock (_writingLock)
                {
                    lock (_queueLock)
                    {
                        if (_messageQueue.Count == 0)
                        {
                            count++;
                        }
                        else
                        {
                            msg = _messageQueue.Dequeue();
                            count = 0;
                        }
                    }
                }

                if (string.IsNullOrEmpty(msg))
                {
                    if (count > 10)
                    {
                        lock (_writingLock)
                            _writing = false;
                        return;
                    }
                    Thread.Sleep(1000);
                    continue;
                }

                if (TimeStampLog)
                    msg = DateTime.Now.ToString("G") + ":" + msg;

                lock(_writingLock)
                {
                    using (var sw = new StreamWriter(StaticSettings.LogFile, true))
                    sw.WriteLine(msg);
                }
            }
        }
        
    }
}
