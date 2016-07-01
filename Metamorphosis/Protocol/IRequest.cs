using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metamorphosis.Protocol
{
    public interface IRequest
    {
        void Pack(ByteBuffer buffer);
    }
}
