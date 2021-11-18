namespace Zork.Common
{
    public interface IOutputService
    {
        void Write(string value);

        void WriteLine(string value);

        void Clear();
    }
}
