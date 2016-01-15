using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;

namespace Benchmarking.Tests
{
    public class PartitioningTests
    {
        private readonly ITestOutputHelper _output;
        private readonly string[] _emails;

        public PartitioningTests(ITestOutputHelper output)
        {
            _output = output;
            _emails = (File.ReadAllLines("C:\\shard.txt")).Skip(1).Where(x => Regex.IsMatch(x[0].ToString(), "[a-zA-Z]")).ToArray();
        }

        [Fact]
        public void PartitioningComparison()
        {
            var alphabet = Alphabet();
            var jump = JumpConsistentHash();
            var ranging = Ranging();

            _output.WriteLine("|Partition|Alphabet|Consistent Hash|Ranging|");
            _output.WriteLine("|-|-|-|-|");
            for (int i = 0; i < 26; i++)
            {
                var letter = alphabet.Keys.ToArray()[i];
                var alphabetValue = alphabet[letter];
                var consistentHashValue = jump[i];
                var rangingValue = ranging[i];

                _output.WriteLine($"|{i}|{alphabetValue}|{consistentHashValue}|{rangingValue}|");
            }
        }

        public Dictionary<string, int> Alphabet()
        {
            return _emails.GroupBy(x => x[0].ToString().ToLower()).ToDictionary(x => x.Key, x => x.Count());
        }

        public Dictionary<int, int> JumpConsistentHash()
        {
            return _emails.GroupBy(x => GetShard(x, 26)).ToDictionary(x => x.Key, x => x.Count());
        }

        public Dictionary<long, int> Ranging()
        {
            return _emails.GroupBy(x =>
            {
                var md5 = MD5.Create();
                var value = md5.ComputeHash(Encoding.ASCII.GetBytes(x));
                var key = BitConverter.ToInt64(value, 0);

                return Math.Abs(key % 26);
            }).ToDictionary(x => x.Key, x => x.Count());
        }

        public Int32 GetShard(String key, Int32 buckets)
        {
            var value = MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(key));
            ulong key1 = BitConverter.ToUInt32(value, 0);

            Int64 b = 1;
            Int64 j = 0;

            while (j < buckets)
            {
                b = j;
                key1 = key1 * 2862933555777941757 + 1;

                var x = (Double)(b + 1);
                var y = (Double)(((Int64)(1)) << 31);
                var z = (Double)((key1 >> 33) + 1);

                j = (Int64)(x * (y / z));
            }

            return (Int32)b;
        }
    }
}
