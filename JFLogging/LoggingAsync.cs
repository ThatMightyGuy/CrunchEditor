namespace JetFly.Logging;

public class LoggerInstanceAsync
{
    private LoggerAsync logger;
    private string source;
    public LoggerInstanceAsync(LoggerAsync logger, string source)
    {
        this.logger = logger;
        this.source = source;
    }
    public async Task Log(string message, ErrorLevel severity = ErrorLevel.INFO) =>
        await logger.Log(message, source, severity);
}
public sealed class LoggerAsync
{
    private readonly TextWriter? stdout;
    private readonly TextWriter? stderr;
    private readonly TextWriter? file;
    private readonly bool useColor;
    public LoggerAsync(TextWriter? stdout, TextWriter? stderr, TextWriter? file, bool useColor = true)
    {
        this.stdout = stdout;
        this.stderr = stderr;
        this.file = file;
        this.useColor = useColor;
    }
    public async Task Log(string message, string source = "main", ErrorLevel severity = ErrorLevel.INFO)
    {
        async Task WriteAsync(TextWriter? dest, string text)
        {
            if(dest is not null)
            {
                await dest.WriteAsync(text);
                await dest.FlushAsync();
            }
        }
        async Task WriteConsoleAsync(string text)
        {
            if(severity >= ErrorLevel.WARN)
                await WriteAsync(stderr, text);
            else
                await WriteAsync(stdout, text);
        }
        source = source.ToLowerInvariant();
        string log = $"[{DateTime.UtcNow.ToString("hh:mm:ss")}] [{source}/{severity.ToString()}] {message}" + Environment.NewLine;
        await WriteAsync(file, log);
        if(useColor)
        {
            await WriteConsoleAsync($"[{DateTime.UtcNow.ToString("hh:mm:ss")}] ");
            Console.ForegroundColor = severity switch
            {
                ErrorLevel.NOTE  => ConsoleColor.Cyan,
                ErrorLevel.WARN  => ConsoleColor.Yellow,
                ErrorLevel.ERR   => ConsoleColor.Red,
                ErrorLevel.FATAL => ConsoleColor.DarkRed,
                _                => ConsoleColor.White
            };
            await WriteConsoleAsync($"[{source}/{severity.ToString()}] ");
            Console.ForegroundColor = ConsoleColor.White;
            await WriteConsoleAsync(message + Environment.NewLine);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        else
            await WriteConsoleAsync(log);
    }
    public void Close()
    {
        if(stdout is not null) stdout.Close();
        if(stderr is not null) stderr.Close();
        if(file is not null) file.Close();
    }
}