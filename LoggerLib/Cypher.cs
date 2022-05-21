using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.CompilerServices;

namespace LoggerLib
{
    public interface ICypher
    {
        public bool GenerateKeys();
        public bool ValidateKeys();
        public string Encrypt(string text);

    }
    public class KuznechikCypher:ICypher
    {
        public KuznechikCypher()
        {

        }
        public bool GenerateKeys()
        {
            throw new NotImplementedException();
        }

        public bool ValidateKeys()
        {
            throw new NotImplementedException();
        }

        public string Encrypt(string text)
        {
            throw new NotImplementedException();
        }
    }
}
