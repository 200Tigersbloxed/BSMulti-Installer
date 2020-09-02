using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSMulti_Installer2.XML
{
    public interface IComponentInstaller
    {
        void Install(string source, string destinationDirectory);
    }
}
