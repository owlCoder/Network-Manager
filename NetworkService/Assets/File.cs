using NetworkService.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.lO
{
    public class File
    {
        public static string[] ReadAllLines(string path)
        {
            return PocetnaViewModel.DELIMITER_CONST.ToArray();
        }

        public static bool Exists(string path)
        {
            return IO.File.Exists(path);
        }
    }
}
