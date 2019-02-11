using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace ConsoleApp2
{
    class Program
    {

        [DllImport(@"c:\libSIAEp7.dll")]
        public static extern int VerifyPIN(int nPIN, string pin);

        [DllImport(@"c:\libSIAEp7.dll")]
        public static extern Byte GetKeyID();

        [DllImport(@"c:\libSIAEp7.dll")]
        public static extern int isCardIn(int Slot);

        [DllImport(@"c:\libSIAEp7.dll")]
        public static extern int Initialize(int Slot);

        [DllImport(@"c:\libSIAEp7.dll")]
        public static extern int SelectML(int fid, int Slot);


        [DllImport(@"c:\libSIAEp7.dll")]
        public static extern int Finalize();

        [DllImport(@"c:\libSIAEp7.dll")]
        public static extern int GetSN(byte[] serial);

        [DllImport(@"c:\libSIAEp7.dll")]
        public static extern int ReadCounter(ref int value);

        [DllImport(@"c:\libSIAEp7.dll")]
        public static extern int ReadBalance(ref int value);


        [DllImport(@"c:\libSIAEp7.dll")]
        public static extern int GetCertificate(byte[] cert, ref int dim);

        [DllImport(@"c:\libSIAEp7.dll")]
        public static extern int GetCACertificate(byte[] cert, ref int dim);

        [DllImport(@"c:\libSIAEp7.dll")]
        public static extern int GetSIAECertificate(byte[] cert, ref int dim);

        [DllImport(@"c:\libSIAEp7.dll")]
        public static extern int UnblockPIN(int nPIN, string Puk, string Newpin);

        [DllImport(@"c:\libSIAEp7.dll")]
        public static extern int ChangePIN(int nPIN, string Oldpin, string Newpin);

        [DllImport(@"c:\libSIAEp7.dll")]
        public static extern int Sign(byte kx, byte[] tosign, byte[] signed);

        [DllImport(@"c:\libSIAEp7.dll")]
        public static extern int Hash(int mec, byte[] tohash, int len, byte[] signed);

        [DllImport(@"c:\libSIAEp7.dll")]
        public static extern int Padding(byte[] topad, int len, byte[] padded);

        [DllImport(@"c:\libSIAEp7.dll")]
        public static extern int PKCS7SignML(string pin, long slot, string szInputFileName, string szOutputFileName, int bInitialize);

        [DllImport(@"c:\libSIAEp7.dll")]
        public static extern int SMIMESignML(string pin, long slot, string szOutputFilePath, string szFrom, string szTo, string szSubject,
string szOtherHeaders, string szBody, string szAttachments, long dwFlags, int bInitialize);


        [DllImport(@"c:\libSIAEp7.dll")]
        public static extern int ComputeSigillo(byte[] Data_Ora, long Prezzo, byte[] SN,
            byte[] mac, ref long cnt);

        [DllImport(@"c:\libSIAEp7.dll")]
        public static extern int BeginTransaction();


        [DllImport(@"c:\libSIAEp7.dll")]
        public static extern int EndTransaction();

        static void Main(string[] args)
        {
            int ciclo = 0;
            int soglia = Int32.Parse(args[2]);
            while (ciclo < soglia)
            {

                Console.WriteLine("Hello World!");
                int g = isCardIn(0);
                int init = Initialize(0);
                BeginTransaction();
                Console.WriteLine("CARD " + g);
                Console.WriteLine("INIT " + init);
                Byte o = GetKeyID();
                Console.WriteLine(o);
                byte[] serial = new byte[16];
                int sn = GetSN(serial);
                Console.WriteLine("Serial " + Encoding.UTF8.GetString(serial));

                int gg = SelectML(0, 0);
                Console.WriteLine(gg.ToString("X"));
                gg = SelectML(1112, 0);
                Console.WriteLine("SELECTML->" + gg.ToString("X"));

                Console.WriteLine("Pin->{0}", args[0]);
                Console.WriteLine("Puk->{0}", args[1]);

                string pk = args[1] + Char.MinValue;
                string newpin = args[0] + Char.MinValue;



                int i = VerifyPIN(1, newpin);
                // Console.WriteLine(num);
                string myHex = i.ToString("X");  // Gives you hexadecimal
                Console.WriteLine("Esito Verifica pin" + myHex);

                int count = 0;
                int reas = ReadCounter(ref count);
                Console.WriteLine("Counter " + count);


                count = 0;
                reas = ReadBalance(ref count);
                Console.WriteLine("Balance " + count);

                //int ok = UnblockPIN(1, pk,  newpin);
                // Console.WriteLine(ok.ToString("X"));


                int dim = 830;
                byte[] cert = null;
                cert = new byte[dim];
                int re = GetCertificate(cert, ref dim);
                Console.WriteLine(re.ToString("X"));
                // string hex = System.Text.Encoding.UTF8.GetString(cert);
                string hex = Convert.ToBase64String(cert);
                Console.WriteLine(hex);


                Console.WriteLine("CA");
                re = GetCACertificate(null, ref dim);
                cert = new byte[dim];
                re = GetCACertificate(cert, ref dim);
                Console.WriteLine(re.ToString("X"));
                // string hex = System.Text.Encoding.UTF8.GetString(cert);
                hex = Convert.ToBase64String(cert);
                Console.WriteLine(hex);

                Console.WriteLine("SIAE");
                re = GetSIAECertificate(null, ref dim);
                cert = new byte[dim];
                re = GetSIAECertificate(cert, ref dim);
                Console.WriteLine(re.ToString("X"));
                // string hex = System.Text.Encoding.UTF8.GetString(cert);
                hex = Convert.ToBase64String(cert);
                Console.WriteLine(hex);

                byte[] result = new byte[20];
                int rha = Hash(1, cert, cert.Length, result);
                Console.WriteLine();
                Console.WriteLine(result);
                Console.WriteLine(BitConverter.ToString(result));
                Console.WriteLine(Encoding.UTF8.GetString(result));

                string testosign = "Testo da signare";

                byte[] toSign = System.Text.Encoding.Unicode.GetBytes(testosign);
                byte[] Padded = new byte[128]; /* Buffer che conterrà il contenuto da firmare */
                byte[] Hashed = new byte[20]; /* Buffer che conterrà l’Hash da firmare */
                byte[] Signed = new byte[128];
                int rc = 0;


                var kid = GetKeyID();
                rc = Hash(1, toSign, toSign.Length, Hashed);
                rc = Padding(Hashed, 20, Padded);
                rc = Sign(kid, Padded, Signed);

                byte[] Data_Ora = new byte[8];
                DateTime dt = DateTime.Now;
                Data_Ora = BitConverter.GetBytes(dt.Ticks);
                long Prezzo = 88;

                byte[] mac = new byte[8];
                long cnt = 0;

                rc = ComputeSigillo(Data_Ora, Prezzo, serial, mac, ref cnt);
                Console.WriteLine(BitConverter.ToString(mac));

                rc = PKCS7SignML(newpin, 0, "test.txt", "test.txt.p7m", 1);
                Console.WriteLine("p7m OK" + rc);
                rc = SMIMESignML(newpin, 0, "prova.eml", "Mario Rossi <mariorossi@prova.it>",
                        "Giuseppe Verdi <mariorossi@prova.it>", "Prova", null, "Email firmata di prova",
                        "test.txt|c:\\test.txt", 0, 0);

                Console.WriteLine("eml OK" + rc.ToString("X"));
                // rc = SMIMESignML(newpin, 0, "prova.eml", "Mario Rossi <mariorossi@prova.it>",
                // "Giuseppe Verdi <mariorossi@prova.it>", "Prova", null, "Email firmata di prova",
                // "test.txt|c:\\test.txt", 0, 1);

                //  Console.WriteLine("eml OK" + rc);
                /* re = GetSIAECertificate(ref cert, ref dim);
                     Console.WriteLine("cert " + cert.ToString());*/

                //byte[] bytes = System.Text.Encoding.Unicode.GetBytes("10382654" + '\0');


                /*String value = "";
                int read = ReadCounter(ref value);
                Console.WriteLine(read);*/
                //    int n = 10382654;
                //    string num = ("10382654") + Char.MinValue;
                //char[] c = (num).ToCharArray();

                EndTransaction();
                Finalize();
                ciclo++;
                Console.WriteLine("Ciclo numero {0}", ciclo);
            }
            Console.WriteLine("Fine");
                Console.ReadLine();
            
        }
    }
}


