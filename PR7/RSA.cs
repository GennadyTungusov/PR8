using System;
using System.Collections.Generic;
using System.IO;

namespace PR5
{
    public class RSA
    {
        // Переменные для чудо-цикла
        private static ulong c, exit;
        // Произведение открытых ключей
        public static ulong n;
        // Функция Эйлера для открытых ключей (p - 1) * (q - 1)
        public static ulong eiler;
        // Открытая экспонента e (часть открытого ключа)
        public static ulong e;
        // Закрытая экспонента n (часть закрытого ключа)
        public static ulong d; 

        /// <summary>
        /// Возвращает просто число в диопазоне переданных чисел
        /// </summary>
        /// <param name="min">Минимальное допустимое число (включительно)</param>
        /// <param name="max">Максимальное допустимое число (включительно)</param>
        /// <returns>Случайное простое число в текущем диапозоне</returns>
        private static ulong GetSimpleNumber(int min, int max)
        {
            // Создаем случайное число
            int randomNumber;
            // Заполняем его с помощью цикла
            do
            {
                randomNumber = new Random().Next(min, max + 1);
            } 
            while (!IsSimpleNumber(randomNumber));

            // Возвращаем случайное число
            return (ulong)randomNumber;
        }

        /// <summary>
        /// Делит байты по блокам в 64 для шифрования методом RSA
        /// </summary>
        /// <param name="byteArray">Массив байтов файла</param>
        /// <returns>Массив байтов файла разделенный по блокам в 64</returns>
        public static byte[] CovertBytesToRSA(byte[] byteArray)
        {
            byte[] buffer = new byte[byteArray.Length * 4];
            byte oneByte;

            for (int i = 0, j = 0; i < byteArray.Length; i++)
            {
                oneByte = byteArray[i];

                for (int k = 0; k < 4; k++)
                {
                    if (oneByte > 64)
                    {
                        buffer[j] = 64;
                        oneByte -= 64;
                    }
                    else
                    {
                        buffer[j] = oneByte;
                        oneByte = 0;
                    }
                    j++;
                }
            }

            return buffer;
        }

        /// <summary>
        /// Возвращает прежнее значение байтов из блока 64
        /// </summary>
        /// <param name="byteArray">Массив байтов файла с блоком в 64</param>
        /// <returns>Массив байтов файла</returns>
        public static byte[] CovertBytesFromRSA(byte[] byteArray)
        {
            byte[] buffer = new byte[byteArray.Length / 4];

            for (int i = 0, j = 0; i < byteArray.Length; i += 4, j++)
            {
                buffer[j] = (byte)(byteArray[i] + byteArray[i + 1] + byteArray[i + 2] + byteArray[i + 3]);
            }

            return buffer;
        }

        /// <summary>
        /// Находит два случайных неравных простых чисел в диапозоне
        /// </summary>
        /// <param name="min">Мин. диапозон поиска простого числа (включительно)</param>
        /// <param name="max">Макс. диапозон поиска простого числа (включительно)</param>
        public static void GetPartOfKeys(int min, int max)
        {
            ulong buffer;
            ulong p;
            ulong q;

            do
            {
                p = GetSimpleNumber(min, max);
                do
                {
                    q = GetSimpleNumber(min, max);
                } while (q == p);
                buffer = p * q;
            } while (buffer < 65 || buffer > 250);

            GetResultOfSimpleNumbers(p, q);
        }

        /// <summary>
        /// Проверяет число на простоту
        /// </summary>
        /// <param name="number">Проверяемое число</param>
        /// <returns>Является ли число простым</returns>
        public static bool IsSimpleNumber(int number)
        {
            if (number < 2)
            {
                return false;
            }

            if (number == 2)
            {
                return true;
            }

            for (long i = 2; i < number; i++)
            {
                if (number % i == 0)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Получает число e (часть открытой экспоненты) и записывает результат в поле e класса RSA
        /// </summary>
        public static void Calculate_e()
        {
            ulong e = eiler - 1;

            for (ulong i = 2; i <= eiler; i++)
            {
                if (eiler % i == 0 && e % i == 0)
                {
                    e--;
                }
            }

            RSA.e = e;
        }

        /// <summary>
        /// Получает число d (часть закрытой экспоненты) и записывает результат в поле d класса RSA
        /// </summary>
        public static void Calculate_d()
        {
            ulong d = 10;

            while (true)
            {
                if (d * e % eiler == 1 && d != e)
                {
                    break;
                }
                else
                {
                    d++;
                }
            }

            RSA.d = d;
        }

        /// <summary>
        /// Высчитывает функцию Эйлера и произвдение двух простых чисел
        /// </summary>
        /// <param name="firstOpenKey">Первое простое число</param>
        /// <param name="secondOpenKey">Второе простое число</param>
        public static void GetResultOfSimpleNumbers(ulong firstOpenKey, ulong secondOpenKey)
        {
            // Получаем произведение простых чисел
            n = firstOpenKey * secondOpenKey;
            // Получаем функцию Эйлера для простых чисел
            eiler = (firstOpenKey - 1) * (secondOpenKey - 1);
        }

        /// <summary>
        /// Зашифровывает нужное сообщение и возвращает список, где каждый элемент представлен в виде его кода 
        /// </summary>
        /// <param name="message">Строка с нужным текстом</param>
        /// <returns>Список с зашифрованными символами<long></returns>
        public static void EncryptFilesInDirectory(byte[] byteArray, string folderForEncryption)
        {
            // Создаем список для записи зашифрованных байтов
            List<byte> encryptCode = new List<byte>();

            // Производим шифрование байтов и заносим результат в список
            for (int i = 0; i < byteArray.Length; i++)
            {
                c = 1;
                exit = 0;

                while (exit < e)
                {
                    exit++;
                    c = (byteArray[i] * c) % n;
                }
                encryptCode.Add((byte)c);
            }

            // Производим запись файла на ПК
            File.WriteAllBytes(folderForEncryption, encryptCode.ToArray());
        }

        /// <summary>
        /// Производит расшифровку сообщения
        /// </summary>
        /// <param name="encryptCode">Список с зашифрованными символами</param>
        /// <returns>Расшифрованный текст</returns>
        public static void DecryptFilesFromDirectory(byte[] byteArray, string folderForDecryption)
        {
            byte[] bytesOfEncryptedFile = byteArray;
            List<byte> encryptCode = new List<byte>();

            for (int i = 0; i < bytesOfEncryptedFile.Length; i++)
            {
                c = 1;
                exit = 0;

                while (exit < d)
                {
                    exit++;
                    c = (bytesOfEncryptedFile[i] * c) % n;
                }
                encryptCode.Add((byte)c);
            }

            byte[] buffer = RSA.CovertBytesFromRSA(encryptCode.ToArray());
            
            File.WriteAllBytes(folderForDecryption, buffer);
        }
    }
}
