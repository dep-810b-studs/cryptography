using System;
using System.Numerics;
using System.Threading.Tasks;

namespace Cryptography.Algorithms
{
    public class RSA : ICipher
    {
        private int p=13;
        private int q=11;
        private int d = 0;
        public long e = 23;

        //N больше любог M
        //Сделать все Без Бигинта
        public byte[] EnCrypt(byte[] message)
        {

            long eiler = (p - 1) * (q - 1);
            long N = p * q;
            int tmp = 0;
            int t = 0;
            gcdext((int)e, (int)eiler,ref tmp,ref d,ref t);

            Console.WriteLine((d*e)%eiler);
            //шифр
        
            var shifr = new long[message.Length];

            for (int i = 0; i < message.Length; i++)
                shifr[i] = ModPow(message[i],e,N);

            for (int i = 0; i < shifr.Length; i++)
                message[i] = (byte)BigInteger.ModPow(shifr[i], d, N);

            return message;
        }

        public byte[] DeCrypt(byte[] encryptedText)
        {
            throw new NotImplementedException();
        }

        static public long ModPow(byte value, long exponent, long modulus)
        {
            if (exponent == 0) return 1;
            long z = ModPow(value, exponent / 2, modulus);
            if (exponent % 2 == 0)
                return (z * z) % modulus;
            else
                return (value * z * z) % modulus;
        }


        static public void gcdext(int a, int b, ref int d,ref int x,ref int y)

        {

            int s;

            if (b == 0)

            {

                d = a; x = 1; y = 0;
                if (d > 1) x = 0;
                return;

            }

            gcdext(b, a % b,ref d,ref x,ref y);

            s = y;

            y = x - (a / b) * (y);

            if (d > 1) x = 0;
            else x = s;

        }

        static public bool[] ReshtoEratosfena(int size)
        {
            var res = new bool[size];

            Parallel.For(0, size, (x) => res[x] = true);		
            for (int p=2;p*p<size;)
            {
                for (int i = 2; i * p < size; i++)
                {
                    res[i * p] = false;
                }

                for (int i = p+1; i < size; i++)
                {
                    if (res[i])
                    {
                        p = i;
                        break;
                    }

                }
            }


            return res;
        }
    }
}