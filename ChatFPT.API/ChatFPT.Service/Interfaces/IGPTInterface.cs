using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatFPT.Service.Interfaces
{
    public interface IGPTInterface
    {
         Task<string> GetGptResponse(string message);
    }
}
