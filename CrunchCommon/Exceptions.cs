namespace CrunchEditor.CrunchCommon;
public class BadInstallException : Exception
{
    public BadInstallException(string message) : base(message) {}
}