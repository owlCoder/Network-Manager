using NetworkService.ViewModel;

namespace System.lO
{
    public class File
    {
        public static string[] ReadAllLines(string path)
        {
            return MainWindowViewModel.System_Delimiter.ToArray();
        }

        public static bool Exists(string path)
        {
            return IO.File.Exists(path);
        }
    }
}
