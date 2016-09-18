using System.IO;
using AddressProcessing.CSV;
using NUnit.Framework;

namespace Csv.Tests
{
    [TestFixture]
    public class CSVReaderWriterTests
    {
        private const string TestInputFile = @"test_data\contacts.csv";
        private const string NonExistantTestInputFile = @"test_data\nonexistant.csv";
        private const string TestInputFileWithOneLineOneColumn = @"test_data\contacts_with_one_col.csv";
        private const string TestInputFileThatsEmpty = @"test_data\empty_contacts_file.csv";
        private const string TestOutputFile = @"test_data\contacts_out.csv";

        [Test]
        public void NonExistantFileOpenTest()
        {
            using (var csvReaderWriter = new CSVReaderWriter())
            {
                Assert.Throws<FileNotFoundException>(() =>
                {
                    csvReaderWriter.Open(NonExistantTestInputFile, CSVReaderWriter.Mode.Read);
                });
            }
        }

        [Test]
        public void ExistingFileOpenTest()
        {
            using (var csvReaderWriter = new CSVReaderWriter())
            {
                Assert.DoesNotThrow(() =>
                {
                    csvReaderWriter.Open(TestInputFile, CSVReaderWriter.Mode.Read);
                });
            }
        }

        [Test]
        public void ReadFileWithOneColumnOpenTest()
        {
            using (var csvReaderWriter = new CSVReaderWriter())
            {
                csvReaderWriter.Open(TestInputFileWithOneLineOneColumn, CSVReaderWriter.Mode.Read);
                var columnStrings = csvReaderWriter.Read();
                Assert.AreEqual(1, columnStrings.Length);
            }
        }

        [Test]
        public void ReadEmptyFileTest()
        {
            using (var csvReaderWriter = new CSVReaderWriter())
            {
                csvReaderWriter.Open(TestInputFileThatsEmpty, CSVReaderWriter.Mode.Read);
                var columnStrings = csvReaderWriter.Read();
                Assert.IsNull(columnStrings);
            }
        }

        [Test]
        public void ReadFileWithNewLineSeparatorTest()
        {
            using (var csvReaderWriter = new CSVReaderWriter('\n'))
            {
                csvReaderWriter.Open(TestInputFile, CSVReaderWriter.Mode.Read);
                var columnStrings = csvReaderWriter.Read();
                Assert.AreEqual(1, columnStrings.Length);
            }
        }

        [Test]
        public void WriteFileWitDefaultSeparatorTest()
        {
            using (var csvReaderWriter = new CSVReaderWriter())
            {
                csvReaderWriter.Open(TestOutputFile, CSVReaderWriter.Mode.Write);
                var columnStrings = new[] { "my", "test", "is", "awesome" };
                for (var i = 0; i < 10; i++)
                {
                    csvReaderWriter.Write(columnStrings);
                }
                csvReaderWriter.Close();

                csvReaderWriter.Open(TestOutputFile, CSVReaderWriter.Mode.Read);
                var line = csvReaderWriter.Read();
                Assert.AreEqual(4, line.Length);
            }
        }

        [Test]
        public void WriteFileWitNewLineSeparatorTest()
        {
            using (var csvReaderWriter = new CSVReaderWriter('\n', '\n'))
            {
                csvReaderWriter.Open(TestOutputFile, CSVReaderWriter.Mode.Write);
                var columnStrings = new[] { "my", "test", "is", "awesome" };
                for (var i = 0; i < 10; i++)
                {
                    csvReaderWriter.Write(columnStrings);
                }
                csvReaderWriter.Close();

                csvReaderWriter.Open(TestOutputFile, CSVReaderWriter.Mode.Read);
                var line = csvReaderWriter.Read();
                Assert.AreEqual(1, line.Length);
            }
        }

        [Test]
        public void WriteFileWitDefaultSeparatorAndReadFullFileTest()
        {
            using (var csvReaderWriter = new CSVReaderWriter())
            {
                csvReaderWriter.Open(TestOutputFile, CSVReaderWriter.Mode.Write);
                var columnStrings = new[] { "my", "test", "is", "awesome" };
                for (var i = 0; i < 2; i++)
                {
                    csvReaderWriter.Write(columnStrings);
                }
                csvReaderWriter.Close();

                csvReaderWriter.Open(TestOutputFile, CSVReaderWriter.Mode.Read);
                string[] columnStringsRead;
                while ((columnStringsRead = csvReaderWriter.Read()) != null)
                {
                    if (columnStringsRead.Length > 0)
                        Assert.AreEqual(4, columnStringsRead.Length);
                }
            }
        }
    }
}
