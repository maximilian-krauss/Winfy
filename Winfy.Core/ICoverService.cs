using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winfy.Core {
    public interface ICoverService {
        string FetchCover(string artist, string track);
        void ClearCache();
    }
}