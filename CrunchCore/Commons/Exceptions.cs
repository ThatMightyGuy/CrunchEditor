namespace CrunchEditor;
public class BadInstallException : Exception
{
    public BadInstallException(string message) : base(message) {}
}