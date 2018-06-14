using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace md5BruteForce
{
    internal class Program
    {
        private const string AvailableCharacters = "abcdefghijklmnopqrstuvwxyz";
        private const int MaxWordLength = 10;

        private static void Main()
        {
            const string md5HashInput = "4c48972cfd92c2bbfdec629a29e4bd3d";

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var result = FindWord(md5HashInput);

            stopwatch.Stop();

            Console.WriteLine(string.IsNullOrEmpty(result)
                ? $"No result found... (tried for {stopwatch.Elapsed.ToString()})"
                : $"Result = \"{result}\" (found in {stopwatch.Elapsed.ToString()})");

            Console.WriteLine();
            Console.Read();
        }

        private static string FindWord(string md5HashInput)
        {
            using (var md5HashAlgorithm = MD5.Create())
            {
                for (var wordLength = 1; wordLength <= MaxWordLength; wordLength++)
                {
                    Console.WriteLine($"Trying words with {wordLength} character(s)...");
                    var allWords = GetAllWords(AvailableCharacters, wordLength);
                    foreach (var word in allWords)
                        if (VerifyMd5Hash(md5HashAlgorithm, word, md5HashInput))
                            return word;
                }
            }

            return string.Empty;
        }

        private static IEnumerable<string> GetAllWords(string availableCharacters, int length)
        {
            if (length > 0)
                foreach (var character in availableCharacters)
                foreach (var subword in GetAllWords(availableCharacters, length - 1))
                    yield return character + subword;
            else
                yield return string.Empty;
        }

        private static bool VerifyMd5Hash(MD5 md5HashAlgorithm, string input, string hash)
        {
            var hashOfInput = GetMd5Hash(md5HashAlgorithm, input);
            var comparer = StringComparer.OrdinalIgnoreCase;
            return comparer.Compare(hashOfInput, hash) == 0;
        }

        private static string GetMd5Hash(MD5 md5HashAlgorithm, string input)
        {
            var computeHash = md5HashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sBuilder = new StringBuilder();
            foreach (var b in computeHash)
                sBuilder.Append(b.ToString("x2"));
            return sBuilder.ToString();
        }
    }
}