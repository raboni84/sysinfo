using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Mono.Unix;
using Mono.Unix.Native;
using toolbelt;

namespace sysinfo
{
    class Program
    {
        static void Main(string[] args)
        {
            bool enableContinuousMode = false;
            if (args.Any(x => x.Equals("-c", StringComparison.OrdinalIgnoreCase)))
                enableContinuousMode = true;

            bool enablePangoMarkup = false;
            if (args.Any(x => x.Equals("-p", StringComparison.OrdinalIgnoreCase)))
                enablePangoMarkup = true;

            bool enableJsonProtocol = false;
            if (args.Any(x => x.Equals("-j", StringComparison.OrdinalIgnoreCase)))
                enableJsonProtocol = true;

            bool enableBars = false;
            if (args.Any(x => x.Equals("-b", StringComparison.OrdinalIgnoreCase)))
                enableBars = true;

            Console.OutputEncoding = new UTF8Encoding(false);

            Util.EnablePangoMarkup = enablePangoMarkup;
            Util.EnableJsonProtocol = enableJsonProtocol;
            Util.EnableBars = enableBars;

            UpdateAll();
            if (enableContinuousMode)
            {
                Thread sig = new Thread(new ThreadStart(SignalThread));
                sig.IsBackground = true;
                sig.Start();
                Thread upd = new Thread(new ThreadStart(UpdateThread));
                upd.IsBackground = true;
                upd.Start();
                Thread inp = new Thread(new ThreadStart(InputThread));
                inp.IsBackground = true;
                inp.Start();
            }

            if (Util.EnableJsonProtocol)
            {
                Console.WriteLine("{ \"version\": 1, \"click_events\": true }");
                Console.WriteLine("[");
                Console.WriteLine("[]");
            }

            while (true)
            {
                PrintAll();
                if (!enableContinuousMode)
                    break;
                Thread.Sleep(2000);
            }

            if (Util.EnableJsonProtocol)
                Console.Write("]");
        }

        public static void SignalThread()
        {
            UnixSignal[] signals = new UnixSignal[] {
                new UnixSignal(Signum.SIGHUP),
                new UnixSignal(Signum.SIGUSR1)
            };

            while (true)
            {
                int idx = UnixSignal.WaitAny(signals);
                bool force = signals[idx].Signum == Signum.SIGUSR1;
                UpdateAll(force);
            }
        }

        public static void UpdateThread()
        {
            while (true)
            {
                Thread.Sleep(3000);
                UpdateAll();
            }
        }

        public static void InputThread()
        {
            try
            {
                using (StreamReader sr = new StreamReader(Console.OpenStandardInput()))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        using (MemoryStream mem = new MemoryStream())
                        {
                            if ((line.Contains($"\"{Cpu.Name}\"") || line.Contains($"\"{nameof(Memory)}\"")) &&
                                line.Contains($"\"button\":1") &&
                                line.Contains($"\"modifiers\":[]"))
                            {
                                ShellUtils.RunShellAsync("urxvt", "-e htop", outputStream: mem, errorStream: mem);
                            }
                            if (line.Contains($"\"{DiskUsageRoot.Name}\"") &&
                                line.Contains($"\"button\":1") &&
                                line.Contains($"\"modifiers\":[]"))
                            {
                                ShellUtils.RunShellAsync("pcmanfm", "/", outputStream: mem, errorStream: mem);
                            }
                            if (line.Contains($"\"{DiskUsageData.Name}\"") &&
                                line.Contains($"\"button\":1") &&
                                line.Contains($"\"modifiers\":[]"))
                            {
                                ShellUtils.RunShellAsync("pcmanfm", $"/home/{Environment.UserName}", outputStream: mem, errorStream: mem);
                            }
                            if (line.Contains($"\"{Volume.Name}\"") &&
                                line.Contains($"\"button\":1") &&
                                line.Contains($"\"modifiers\":[]"))
                            {
                                ShellUtils.RunShellAsync("pavucontrol", outputStream: mem, errorStream: mem);
                            }
                            if (line.Contains($"\"{Volume.Name}\"") &&
                                line.Contains($"\"button\":3") &&
                                line.Contains($"\"modifiers\":[]"))
                            {
                                ShellUtils.RunShellAsync("pamixer", "-t", outputStream: mem, errorStream: mem);
                            }
                            if (line.Contains($"\"{Date.Name}\"") &&
                                line.Contains($"\"button\":1") &&
                                line.Contains($"\"modifiers\":[]"))
                            {
                                ShellUtils.RunShellAsync("evolution", "-c calendar", outputStream: mem, errorStream: mem);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Notify.Message("Exception", ex.ToString());
            }
        }

        public static void UpdateAll(bool force = false)
        {
            try
            {
                Kernel.Update();
                Cpu.Update();
                Memory.Update();
                DiskUsageRoot.Update();
                DiskUsageData.Update();
                Temperature.Update();
                Battery.Update();
                Wifi.Update();
                NetUsage.Update();
                Volume.Update();
                Date.Update();
            }
            catch (Exception)
            { }
        }

        public static void PrintAll()
        {
            string[] vals = new[] {
                Kernel.Value,
                Cpu.Value,
                Memory.Value,
                DiskUsageRoot.Value,
                DiskUsageData.Value,
                Temperature.Value,
                Battery.Value,
                Wifi.Value,
                NetUsage.Value,
                Volume.Value,
                Date.Value
            };
            if (Util.EnableJsonProtocol)
            {
                string[] names = new[] {
                    Kernel.Name,
                    Cpu.Name,
                    Memory.Name,
                    DiskUsageRoot.Name,
                    DiskUsageData.Name,
                    Temperature.Name,
                    Battery.Name,
                    Wifi.Name,
                    NetUsage.Name,
                    Volume.Name,
                    Date.Name
                };
                string txt = string.Join(",", vals
                    .Select((x, i) => new { x, i })
                    .Where(o => o.x != null && o.x != string.Empty)
                    .Select(o => new { x = o.x.Replace("\\", "\\\\").Replace("\"", "\\\""), i = o.i })
                    .Select(o => $"{{\"name\":\"{names[o.i]}\",\"markup\":\"pango\",\"full_text\":\"{o.x}\"}}"));
                Console.WriteLine($",[{txt}]");
            }
            else
            {
                string txt = string.Join(Util.Space,
                    vals.Where(x => x != null && x != string.Empty));
                Console.WriteLine(txt);
            }
        }
    }
}
