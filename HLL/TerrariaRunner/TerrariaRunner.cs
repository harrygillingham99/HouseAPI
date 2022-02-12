using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using House.HLL.TerrariaRunner.Interfaces;
using Microsoft.Extensions.Options;
using Serilog;

namespace House.HLL.TerrariaRunner
{
    public class TerrariaRunner : ITerrariaRunner
    {
        private readonly Process _process;
        private readonly TerrariaConfig _config;
        private bool _isRunning;
        private readonly Stopwatch _timer;

        public TerrariaRunner(IOptions<TerrariaConfig> options)
        {
            _timer = new Stopwatch();
            _isRunning = false;
            _process = new Process();
            _config = options.Value;
        }

        public bool Start()
        {
            if (_isRunning) return true;

            var startInfo = new ProcessStartInfo("cmd.exe", $"/c {_config.StartCommand}")
            {
                WorkingDirectory = _config.ServerDirectory,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                CreateNoWindow = true
            };

            void CaptureStdOutput(object sender, DataReceivedEventArgs e)
            {
                if (!string.IsNullOrWhiteSpace(e.Data))
                {
                    Log.Verbose(e.Data);
                }
            }

            _process.StartInfo = startInfo;
            _process.ErrorDataReceived += CaptureStdOutput;
            _process.OutputDataReceived += CaptureStdOutput;
            _isRunning = _process.Start();

            return _isRunning;
        }

        public async Task InputCommand(string command)
        {
            var writer = _process.StandardInput;
            await writer.WriteLineAsync(command);
            await writer.FlushAsync();
            writer.Close();
        }

        public List<string> GetCurrentLogsTail(int secondsToCapture)
        {
            if(!_isRunning) return new List<string>{"Server not running."};

            List<string> output = new List<string>();

            void AppendOutput(object sender, DataReceivedEventArgs e)
            {
                if (!string.IsNullOrWhiteSpace(e.Data))
                {
                    output.Add(e.Data);
                }
            }

            _process.OutputDataReceived += AppendOutput;

            _timer.Start();

            while (_timer.Elapsed < TimeSpan.FromSeconds(secondsToCapture)){}
                
            _timer.Reset();

            _process.OutputDataReceived -= AppendOutput;

            return output;
        }
    }
}
