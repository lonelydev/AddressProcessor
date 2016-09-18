using System;
using System.IO;
using System.Text;

namespace AddressProcessing.CSV
{
    /*
        2) Refactor this class into clean, elegant, rock-solid & well performing code, without over-engineering.
           Assume this code is in production and backwards compatibility must be maintained.
    */

    public class CSVReaderWriter : IDisposable
    {
        private StreamReader _readerStream = null;
        private StreamWriter _writerStream = null;

        private readonly char _readSeparator;
        private readonly char _writeSeparator;
        private const char DefaultSeparator = '\t';

        public CSVReaderWriter()
        {
            _readSeparator = DefaultSeparator;
            _writeSeparator = DefaultSeparator;
        }

        /// <summary>
        /// initializes CSVReaderWriter with the read and write separators that you would like to set. 
        /// Default separator is tab character.
        /// </summary>
        /// <param name="readSeparator"></param>
        /// <param name="writeSeparator"></param>
        public CSVReaderWriter(char readSeparator = DefaultSeparator, char writeSeparator = DefaultSeparator)
        {
            _readSeparator = readSeparator;
            _writeSeparator = writeSeparator;
        }

        ~CSVReaderWriter()
        {
            //finalizer/destructor invoked by Garbage collector
            Dispose(false);
        }

        protected void Dispose(bool safeToDispose)
        {
            if (!safeToDispose) return;

            if (_writerStream != null)
            {
                _writerStream.Dispose();
                _writerStream = null;
            }

            if (_readerStream != null)
            {
                _readerStream.Dispose();
                _readerStream = null;
            }
        }

        [Flags]
        public enum Mode { Read = 1, Write = 2 };

        public void Open(string fileName, Mode mode)
        {
            switch (mode)
            {
                case Mode.Read:
                    _readerStream = File.OpenText(fileName);
                    break;
                case Mode.Write:
                    var fileInfo = new FileInfo(fileName);
                    _writerStream = fileInfo.CreateText();
                    break;
                default:
                    throw new Exception("Unknown file mode for " + fileName);
            }
        }

        public void Write(params string[] columns)
        {
            var outPut = new StringBuilder("");

            for (var i = 0; i < columns.Length; i++)
            {
                outPut.Append(columns[i]);
                if ((columns.Length - 1) > i)
                {
                    outPut.Append(_writeSeparator);
                }
            }
            WriteLine(outPut);
        }

        /// <summary>
        /// Because this method always returns two columns, 
        /// when line can't be split into columns, return false with columns as null
        /// when line has only one column, return just the first column other being null
        /// when line is split into more than 1 columns, returns first two columns and true.
        /// </summary>
        /// <param name="column1"></param>
        /// <param name="column2"></param>
        /// <returns></returns>
        public bool Read(out string column1, out string column2)
        {
            column1 = null;
            column2 = null;

            var line = ReadLine();
            if (line == null) return false;
            var columns = line.Split(_readSeparator);
            switch (columns.Length)
            {
                case 0:
                    return false;
                case 1:
                    column1 = columns[0];
                    return true;
                default:
                    column1 = columns[0];
                    column2 = columns[1];
                    return true;
            }
        }

        /// <summary>
        /// Reads the file by line and returns an array of strings from the file that represents a line
        /// split by the read separator that you chose to set in the constructor
        /// </summary>
        /// <returns></returns>
        public string[] Read()
        {
            var line = ReadLine();
            if (line != null)
            {
                return line.Split(_readSeparator);
            }
            return null;
        }


        private void WriteLine(StringBuilder line)
        {
            _writerStream.WriteLine(line);
        }

        private string ReadLine()
        {
            return _readerStream.ReadLine();
        }

        public void Close()
        {
            if (_writerStream != null)
            {
                _writerStream.Close();
            }

            if (_readerStream != null)
            {
                _readerStream.Close();
            }
        }

        public void Dispose()
        {
            //always safe to dispose when calling from Dispose
            Dispose(true);
            //Disallow being called twice - first by user dispose and second by GC later - just in case
            //a good practice when disposing
            GC.SuppressFinalize(this);
        }
    }
}
