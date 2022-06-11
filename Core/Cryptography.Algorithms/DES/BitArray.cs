using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Cryptography.Algorithms.DES
{
    public class BitArray: ICollection<bool>,ICloneable,IEnumerable
    {
        private List<bool> intraBitArray;       

        #region Constructors

        public BitArray()
        {
            intraBitArray = new List<bool>();
            IsReadOnly = false;            
        }

        public BitArray(BitArray bitArray)
        {
            intraBitArray = new List<bool>(bitArray.Count);
            Parallel.For(0, bitArray.Count, (i) => { intraBitArray[i] = bitArray[i]; });
            IsReadOnly = false;
        }

        public BitArray(IEnumerable<bool> inputBoolList)
        {
            intraBitArray = (from element in inputBoolList.AsParallel() select element).ToList();
            IsReadOnly = false;
        }

        public BitArray(IEnumerable<byte> inputByteList)
        {
            getBitArrayFromByteArr(inputByteList.ToList());
        }

        public BitArray(IEnumerable<byte> inputByteList,int byteLength)
        {
            intraBitArray = new List<bool>();
            Func<byte, List<bool>> transformByteToListBools = new Func<byte, List<bool>>((element) => {
                List<bool> tempList = new List<bool>();
                for (int i = 0; i < byteLength; i++)
                {
                    tempList.Add(false);
                }
                for (int i = 0; i < byteLength; i++)
                {
                    tempList[i] = FirstSectionOfOperations.getIbit(element,i) == 1 ? true : false;
                }
                tempList.Reverse();
                return tempList;
            });
            inputByteList = inputByteList.Reverse().ToList();
            intraBitArray = (from element in inputByteList.AsParallel() select transformByteToListBools(element)).SelectMany(x => x).ToList();
            IsReadOnly = false;
        }

        public void getBitArrayFromByteArr(List<byte> inputArray)
        {
            intraBitArray = new List<bool>();
            Func<byte, List<bool>> transformByteToListBools = new Func<byte, List<bool>>((element) => 
            {
                var tempList = new List<bool>() { false, false, false, false, false, false, false, false };
                for (int i = 0; i < 8; i++)
                {
                    tempList[i] = FirstSectionOfOperations.getIbit(element,i) == 1 ? true : false;
                }
                tempList.Reverse();
                return tempList;
            });
            inputArray.Reverse();
            intraBitArray = (from element in inputArray.AsParallel() select transformByteToListBools(element)).SelectMany(x => x).ToList();
            
            IsReadOnly = false;
        }

        public BitArray(long inputLongValue)
        {
            byte[] longToByteArr = BitConverter.GetBytes(inputLongValue);
            getBitArrayFromByteArr(longToByteArr.ToList());
            IsReadOnly = false;
        }

        public BitArray(int Length)
        {
            intraBitArray = new List<bool>();
            Parallel.For(0, Length, (i) => {
                lock (intraBitArray)
                {
                    intraBitArray.Add(false);
                }
            });
        }

        public BitArray(string num,bool isBinary = false)
        {
            if(isBinary)
            {
                intraBitArray = new List<bool>();
                foreach (var n in num)
                    if (n == '1' | n == '0') intraBitArray.Add(n == '1');
                    else throw new Exception("Invalid char in binary number");
            }
            else
            {

            }
            
        }
        public BitArray(string num, int size)
        {
            intraBitArray = new List<bool>();
            if (num.Length < size)
            {
                var diff = size - num.Length;
                while (diff-- != 0) intraBitArray.Add(false);
            }
            foreach (var n in num)
            {
                if (n == '1' | n == '0') intraBitArray.Add(n == '1');
                else throw new Exception("Invalid char in binary number");
            }
        }
        #endregion

        #region ICollection Implementation

        /// <summary>
        /// Count of values in BitArray
        /// </summary>
        public int Count
        {
            get
            {
                return intraBitArray.Count;
            }
        }

        /// <summary>
        /// BitArrayAccess mode
        /// </summary>
        public bool IsReadOnly {
            get
            {
                return intraIsReadOnly;
            }
            set
            {
                intraIsReadOnly = value;
            }
        }
        private  bool intraIsReadOnly { get; set; }

        /// <summary>
        /// Add bool value to BitArray
        /// </summary>
        /// <param name="item">Bool value</param>
        public void Add(bool item)
        {
            if(!IsReadOnly)
            {
                intraBitArray.Add(item);
            }
            else
            {
                throw new ReadException("BitArray access mode is only read");
            }
        }

        /// <summary>
        /// Clear BitArray values
        /// </summary>
        public void Clear()
        {
            if(!IsReadOnly)
            {
                intraBitArray.Clear();
            }
            else
            {
                throw new ReadException("BitArray access mode is only read");
            }
        }

        /// <summary>
        /// Is item contains in BitArray
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(bool item)
        {
            return intraBitArray.Contains(item);
        }

        /// <summary>
        /// Procedure to copy BitArray values to bool array from a given index
        /// </summary>
        /// <param name="array">Bool array</param>
        /// <param name="arrayIndex">Start copy index</param>
        public void CopyTo(bool[] array, int arrayIndex)
        {
            Parallel.For(0, intraBitArray.Count,
                (i,state)=> {
                    try
                    {
                        array[i + arrayIndex] = intraBitArray[i];
                    }
                    catch
                    {}
                });
        }

        /// <summary>
        /// Procedure to copy BitArray values to byte array from a given index
        /// </summary>
        /// <param name="array">Byte array</param>
        /// <param name="arrayIndex">Start copy index</param>
        public void CopyTo(byte[] array, int arrayIndex)
        {
            Parallel.For(0, intraBitArray.Count,
                (i, state) =>
                {
                    try
                    {
                        if (intraBitArray[i] == true)
                        {
                            FirstSectionOfOperations.SetIbit(7 - (i + arrayIndex) % 8, ref (array[(array.Count() - 1) - ((int)((arrayIndex + i) / 8))]));
                        }
                        else
                        {
                            FirstSectionOfOperations.TakeOffIbit(7 - (i + arrayIndex) % 8, ref (array[(array.Count() - 1) - ((int)((arrayIndex + i) / 8))]));
                        }
                    }
                    catch
                    { }
                });
        }

        public IEnumerator<bool> GetEnumerator()
        {
            return intraBitArray.GetEnumerator();
        }

        /// <summary>
        /// Remove item from BitArray
        /// </summary>
        /// <param name="item">Bool value</param>
        /// <returns></returns>
        public bool Remove(bool item)
        {
            if(!IsReadOnly)
            {
                return intraBitArray.Remove(item);
            }
            else
            {
                throw new ReadException("BitArray access mode is only read");
            }
        }

        /// <summary>
        /// Remove item at given index from BitArray
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public void RemoveAt(int index)
        {
            if (!IsReadOnly)
            {
                intraBitArray.RemoveAt(index);
            }
            else
            {
                throw new ReadException("BitArray access mode is only read");
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return intraBitArray.GetEnumerator();
        }

        public BitArray Union(BitArray second)
        {
            intraBitArray.AddRange((List<bool>)second);
            return (BitArray)Clone();
        }

        public BitArray Reverse()
        {
            intraBitArray.Reverse();
            return this;
        }

        public BitArray[] DevideByParts(int parts)
        {
            var sizeOfParts = intraBitArray.Count / parts;

            var listBitArrays = new List<BitArray>();

            //Console.WriteLine($"size of parts = {sizeOfParts}");

            int count = 0;
            var temp = new BitArray();

            foreach(var bit in intraBitArray)
            {
                temp.Add(bit);
                count++;
                if (count  > sizeOfParts-1)
                {
                    listBitArrays.Add(temp);
                    count = 0;
                    temp = new BitArray();
                }
            }
            return listBitArrays.ToArray();
            }

        public BitArray JoinArr(BitArray bitArray)
        {
            var res = new List<bool>();
            foreach (var n in intraBitArray) res.Add(n);
            foreach (var n in bitArray) res.Add(n);
            return new BitArray(res);
        }
        public static BitArray ConcatArr(params BitArray[] bitArrays)
        {
            var res = new List<bool>();
            foreach (var bitArray in bitArrays)
                foreach (var bit in bitArray)
                    res.Add(bit);
            return new BitArray(res);
        }
        #endregion

        #region ICloneable Implementation
        public object Clone()
        {
            return new BitArray(intraBitArray);
        }
        #endregion

        #region Operator overloading

        /// <summary>
        /// Bit negation over all members of BitArray
        /// </summary>
        /// <param name="bitArray"></param>
        /// <returns></returns>
        public static BitArray operator !(BitArray bitArray)
        {
            return new BitArray((from element in bitArray.AsParallel() select !element).ToList());
        }

        /// <summary>
        /// Bit XOR over all members of BitArray
        /// </summary>
        /// <param name="bitArray1"></param>
        /// <param name="bitArray2"></param>
        /// <returns></returns>
        public static BitArray operator ^(BitArray bitArray1, BitArray bitArray2)
        {
            Parallel.For(0, bitArray1.Count, (i) =>
            {
                bitArray1[i] ^= bitArray2[i];
            });
            return bitArray1.Clone() as BitArray;
        }

        /// <summary>
        /// Bit conjunction over all members of BitArray
        /// </summary>
        /// <param name="bitArray1"></param>
        /// <param name="bitArray2"></param>
        /// <returns></returns>
        public static BitArray operator &(BitArray bitArray1, BitArray bitArray2)
        {
            Parallel.For(0, bitArray1.Count, (i) => {
                bitArray1[i] &= bitArray2[i];
            });
            return (BitArray)bitArray1.Clone();
        }

        /// <summary>
        /// Bit disjunction over all members of BitArray
        /// </summary>
        /// <param name="bitArray1"></param>
        /// <param name="bitArray2"></param>
        /// <returns></returns>
        public static BitArray operator |(BitArray bitArray1, BitArray bitArray2)
        {
            Parallel.For(0, bitArray1.Count, (i) => {
                bitArray1[i] |= bitArray2[i];
            });
            return (BitArray)bitArray1.Clone();
        }


        public static BitArray operator <<(BitArray bitArray, int n)// циклический сдвиг <<<
        {
            bool tempval = false;
            for (int i = 0; i < n; i++)
            {
                tempval = bitArray[0];
                bitArray.RemoveAt(0);
                bitArray.Add(tempval);
            }
            return (BitArray)bitArray.Clone();
        }

        public override string ToString()
        {
            string res = "";
            foreach (var item in this)
                res += (item == true ? "1" : "0"); //+ " ";
            return res;
        }
        #endregion

        #region Bit Operations Classes
        public static class FirstSectionOfOperations
        {

            public static int getIbit(int Number,int IndexNumber)
            {
                int intraNumber = Number;
                int result = ((1 << IndexNumber) & intraNumber) >> IndexNumber;
                return result;
            }

            public static void SetIbit(int IndexNumber, ref byte Number)
            {
                Number = (byte)((1 << IndexNumber) | Number);
            }

            public static void TakeOffIbit(int IndexNumber, ref byte Number)
            {
                Number = (byte)(~(1 << IndexNumber) & Number);
            }

            public static void swapIJbit(int i, int j, ref int Number)
            {
                int intraNumber = Number;
                int withoutIJbits = intraNumber & (-1 ^ ((1 << i) | (1 << j)));
                Number = ((int)getIbit(i, intraNumber) << j) | (int)getIbit(j, intraNumber) << i | withoutIJbits;
            }

            public static void SetToZero(int m, ref int Number)
            {
                Number = ((-1) << m) & Number;
            }

        }
        #endregion

        #region Extra Functionality

        public void IncreaseLength(int length)
        {
            intraBitArray.Reverse();
            while (length-- != 0) intraBitArray.Add(false);
            intraBitArray.Reverse();
        }

        public void RemoveInsignificant()
        {
            var temp = new List<bool>();

            bool isfisrt = false;

            foreach (var el in intraBitArray)
                if (isfisrt) temp.Add(el);
                else isfisrt = el;

            intraBitArray = temp;
        }
        public bool this[int index]
        {
            get
            {
                try
                {
                    return intraBitArray[index];
                }
                catch (Exception)
                {
                    throw;
                }
            }
            set
            {
                try
                {
                    intraBitArray[index] = value;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        /// <summary>
        ///  Applying input permutations to BitArray
        /// </summary>
        /// <param name="permutations"></param>
        public void applyPermutations(IEnumerable<int> permutations)
        {
            intraBitArray = (from element in permutations select intraBitArray.ElementAt(element - 1)).ToList();
        }

        public byte getByteValueOFBitArray
        {
            get
            {
                if(Count <= 8)
                {
                    byte result = 0;
                    intraBitArray.Reverse();
                    for (int i = 0; i < Count; i++)
                    {
                        if (intraBitArray[i])
                        {
                            FirstSectionOfOperations.SetIbit(i, ref result);
                        }
                        else
                        {
                            FirstSectionOfOperations.TakeOffIbit(i, ref result);
                        }
                    }
                    return result;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Count > 8");
                }
            }
        }

        public void removeNullsfromBegin()
        {
            while (true)
            {
                if (intraBitArray[0] == false)
                {
                    intraBitArray.RemoveAt(0);
                }
                else
                {
                    intraBitArray.Reverse();
                    intraBitArray.Add(false);
                    intraBitArray.Reverse();
                    break;
                }
            }
        }

        public uint getUintValFromBitArray
        {
            get
            {
                byte[] tempRes = new byte[4];
                CopyTo(tempRes, 0);
                return BitConverter.ToUInt32(tempRes, 0);
            }
        }

        public long getLongValFromBitArray
        {
            get
            {
                byte[] tempRes = new byte[8];
                CopyTo(tempRes, 0);
                return BitConverter.ToInt64(tempRes, 0);
            }
        }

        public char[] elementsOfBitArrayInChar
        {
            get
            {
                return ToString().ToArray();
            }
        }

        public string getLongHexadecimalFromBitarray
        {
            get
            {
                return getLongValFromBitArray.ToString("X");
            }
        }

        public string getUintHexadecimalFromBitarray
        {
            get
            {
                return getUintValFromBitArray.ToString("X");
            }
        }

        public byte[] getByteArrayFromBitArray
        {
            get
            {
                byte[] tempByteArr = new byte[Count / 8];
                CopyTo(tempByteArr, 0);
                return tempByteArr;
            }
        }
        #endregion

        #region Exceptions
        public class ReadException : Exception
        {
            public ReadException() : base() {
            }

            public ReadException(string message) : base(message)
            {}

            public ReadException(string message, Exception innerException) :
               base(message, innerException)
            {}

            
        }
        #endregion

        #region Converters

    
        public static explicit operator List<bool>(BitArray bitArray)
        {
            return (from element in bitArray select element).ToList();
        }

        public static explicit operator byte[](BitArray bitArray)
        {
            return bitArray.getByteArrayFromBitArray;
        }

        public static explicit operator long(BitArray bitArray)
        {
            return bitArray.getLongValFromBitArray;
        }

        public static explicit operator uint(BitArray bitArray)
        {
            return bitArray.getUintValFromBitArray;
        }

        public static explicit operator string(BitArray bitArray)
        {
            return bitArray.ToString();
        }

        #endregion
    }
}
