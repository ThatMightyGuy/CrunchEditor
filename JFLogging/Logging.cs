namespace JetFly.Logging;

public enum ErrorLevel
{
    NOTE = 1,
    INFO = 2,
    WARN = 4,
    ERR = 8,
    FATAL = 16
}
public class InstanceExistsException : Exception
{
    public InstanceExistsException(string message) : base(message) {}
}
public class LoggerInstance
{
    private Logger logger;
    private string source;
    public LoggerInstance(Logger logger, string source)
    {
        this.logger = logger;
        this.source = source;
    }
    public void Log(string message, ErrorLevel severity = ErrorLevel.INFO) =>
        logger.Log(message, source, severity);
}
public sealed class Logger
{
    private readonly TextWriter? stdout;
    private readonly TextWriter? stderr;
    private readonly TextWriter? file;
    private readonly bool useColor;
    public Logger(TextWriter? stdout, TextWriter? stderr, TextWriter? file, bool useColor = true)
    {
        this.stdout = stdout;
        this.stderr = stderr;
        this.file = file;
        this.useColor = useColor;
    }
    public void Log(string message, string source = "main", ErrorLevel severity = ErrorLevel.INFO)
    {
        void Write(TextWriter? dest, string text)
        {
            if(dest is not null)
                dest.Write(text);
        }
        void WriteConsole(string text)
        {
            if(severity >= ErrorLevel.WARN)
                Write(stderr, text);
            else
                Write(stdout, text);
        }
        source = source.ToLowerInvariant();
        string log = $"[{DateTime.UtcNow.ToString("hh:mm:ss")}] [{source}/{severity.ToString()}] {message}" + Environment.NewLine;
        if(useColor)
        {
            WriteConsole($"[{DateTime.UtcNow.ToString("hh:mm:ss")}] ");
            Console.ForegroundColor = severity switch
            {
                ErrorLevel.NOTE  => ConsoleColor.Cyan,
                ErrorLevel.WARN  => ConsoleColor.Yellow,
                ErrorLevel.ERR   => ConsoleColor.Red,
                ErrorLevel.FATAL => ConsoleColor.DarkRed,
                _                => ConsoleColor.White
            };
            WriteConsole($"[{source}/{severity.ToString()}] ");
            Console.ForegroundColor = ConsoleColor.White;
            WriteConsole(message + Environment.NewLine);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        else
            WriteConsole(log);
        Write(file, log);
    }
    public void Close()
    {
        if(stdout is not null) stdout.Close();
        if(stderr is not null) stderr.Close();
        if(file is not null) file.Close();
    }
}