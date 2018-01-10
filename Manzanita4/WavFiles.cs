using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manzanita4
{
    // Blatantly stolen from: https://blogs.msdn.microsoft.com/dawate/2009/06/24/intro-to-audio-programming-part-3-synthesizing-simple-wave-audio-using-c/
    class WavFiles
    {
        public static void Save(string filename, Clip<Stereo> s) => new WaveGenerator(s).Save(filename);
        public static void Save(string filename, Clip<Mono> m) => new WaveGenerator(m).Save(filename);

        private class WaveHeader
        {
            public string sGroupID; // RIFF
            public uint dwFileLength; // total file length minus 8, which is taken up by RIFF
            public string sRiffType; // always WAVE

            public WaveHeader()

            {
                dwFileLength = 0;
                sGroupID = "RIFF";
                sRiffType = "WAVE";
            }
        }

        private class WaveFormatChunk
        {
            public string sChunkID;         // Four bytes: "fmt "
            public uint dwChunkSize;        // Length of header in bytes
            public ushort wFormatTag;       // 1 (MS PCM)
            public ushort wChannels;        // Number of channels
            public uint dwSamplesPerSec;    // Frequency of the audio in Hz... 44100
            public uint dwAvgBytesPerSec;   // for estimating RAM allocation
            public ushort wBlockAlign;      // sample frame size, in bytes
            public ushort wBitsPerSample;    // bits per sample

            public WaveFormatChunk(ushort channels = 2, uint sampleRate = 44100)
            {
                sChunkID = "fmt ";
                dwChunkSize = 16;
                wFormatTag = 1;
                wChannels = channels;
                dwSamplesPerSec = sampleRate;
                wBitsPerSample = 16; // screw you: there's no way you want more than 16 bits
                wBlockAlign = (ushort)(wChannels * (wBitsPerSample / 8));
                dwAvgBytesPerSec = dwSamplesPerSec * wBlockAlign;
            }
        }

        private class WaveDataChunk
        {
            public string sChunkID;     // "data"
            public uint dwChunkSize;    // Length of header in bytes
            public short[] shortArray;  // 8-bit audio

            /// <summary>
            /// Initializes a new data chunk with default values.
            /// </summary>
            public WaveDataChunk()
            {
                shortArray = new short[0];
                dwChunkSize = 0;
                sChunkID = "data";
            }
        }

        private class WaveGenerator
        {
            private WaveHeader header;
            private WaveFormatChunk format;
            private WaveDataChunk data;

            public WaveGenerator(Clip<Stereo> stereo)
            {
                // Init chunks
                header = new WaveHeader();
                format = new WaveFormatChunk(2, (uint) stereo.SampleRate);
                data = new WaveDataChunk();

                uint numSamples = format.wChannels * (uint) stereo.Samples.Length;
                // Initialize the 16-bit array
                data.shortArray = new short[numSamples];

                int amplitude = 32760;  // Max amplitude for 16-bit audio
                for (int i = 0; i < stereo.Samples.Length; i++)
                {
                    // Fill with a simple sine wave at max amplitude
                    data.shortArray[i * 2] =
                        Convert.ToInt16(Clamp1(stereo.Samples[i].Left.Value) * amplitude);
                    data.shortArray[i * 2 + 1] =
                        Convert.ToInt16(Clamp1(stereo.Samples[i].Right.Value) * amplitude);
                }

                // Calculate data chunk size in bytes
                data.dwChunkSize = (uint)(data.shortArray.Length * (format.wBitsPerSample / 8));
            }

            private double Clamp1(double x)
            {
                if (x > 1.0) { return 1.0; }
                if (x < -1.0) { return -1.0; }
                return x;
            }

            public WaveGenerator(Clip<Mono> mono)
            {
                // Init chunks
                header = new WaveHeader();
                format = new WaveFormatChunk(1, (uint) mono.SampleRate);
                data = new WaveDataChunk();

                uint numSamples = format.wChannels * (uint) mono.Samples.Length;
                // Initialize the 16-bit array
                data.shortArray = new short[numSamples];

                int amplitude = 32760;  // Max amplitude for 16-bit audio
                for (int i = 0; i < mono.Samples.Length - 1; i++)
                {
                    // Fill with a simple sine wave at max amplitude
                    data.shortArray[i] =
                        Convert.ToInt16(mono.Samples[i].Value * amplitude);
                }

                // Calculate data chunk size in bytes
                data.dwChunkSize = (uint)(data.shortArray.Length * (format.wBitsPerSample / 8));
            }

            public void Save(string filePath)
            {
                // Create a file (it always overwrites)
                FileStream fileStream = new FileStream(filePath, FileMode.Create);

                // Use BinaryWriter to write the bytes to the file
                BinaryWriter writer = new BinaryWriter(fileStream);

                // Write the header
                writer.Write(header.sGroupID.ToCharArray());
                writer.Write(header.dwFileLength);
                writer.Write(header.sRiffType.ToCharArray());

                // Write the format chunk
                writer.Write(format.sChunkID.ToCharArray());
                writer.Write(format.dwChunkSize);
                writer.Write(format.wFormatTag);
                writer.Write(format.wChannels);
                writer.Write(format.dwSamplesPerSec);
                writer.Write(format.dwAvgBytesPerSec);
                writer.Write(format.wBlockAlign);
                writer.Write(format.wBitsPerSample);

                // Write the data chunk
                writer.Write(data.sChunkID.ToCharArray());
                writer.Write(data.dwChunkSize);

                foreach (short dataPoint in data.shortArray)
                {
                    writer.Write(dataPoint);
                }

                writer.Seek(4, SeekOrigin.Begin);
                uint filesize = (uint)writer.BaseStream.Length;
                writer.Write(filesize - 8);

                // Clean up
                writer.Close();
                fileStream.Close();
            }
        }
    }
}
