using System.Diagnostics;
using Microsoft.Extensions.Options;
using Serilog;

namespace House.HLL.TerrariaRunner
{
    public class TerrariaRunner : ITerrariaRunner
    {
        private readonly Process _process;
        private readonly TerrariaConfig _config;

        public TerrariaRunner(IOptions<TerrariaConfig> options)
        {
            _process = new Process();
            _config = options.Value;
        }

        public bool Start()
        {
            var startInfo = new ProcessStartInfo("cmd.exe", $"/c {_config.StartCommand}")
            {
                WorkingDirectory = _config.ServerDirectory,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };
            _process.StartInfo = startInfo;
            _process.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data != null) Log.Error(e.Data);
            };
            _process.OutputDataReceived += (sender, e) =>
            {
                if (e.Data != null) Log.Information(e.Data);
            };

            return _process.Start();
        }
    }

    public interface ITerrariaRunner
    {
    }
}
