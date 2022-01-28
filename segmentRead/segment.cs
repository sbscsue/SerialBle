using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace segmentRead
{
    public class segment

    {
        String folderPath;
        Queue<StorageFile> files;
        IReadOnlyList<StorageFile> d;
        StorageFile dddd;
        
        public segment(String folderPath)
        {
            this.folderPath = folderPath;
        }

        public async Task getFileQueueAsync()
        {
            var storageFolder = await StorageFolder.GetFolderFromPathAsync(folderPath);
            var storageFiles = await storageFolder.GetFilesAsync();
            this.d = storageFiles;
            this.dddd = storageFiles.Last();
            this.files = new Queue<StorageFile>(storageFiles);
            
          
        }

        public int segmentNumber()
        {
            return files.Count;
        }

        public async Task<byte[]> popDataAsync()
        {
            var file = files.Dequeue();
            var ecgString = await FileIO.ReadTextAsync(file);

            float[] ecgFloatArray = parsingStringCsvToFloatArray(ecgString);
            byte[] ecgByteArray = stringToByteArray(floatArrayToString(ecgFloatArray));

            return ecgByteArray;
        }


        private float[] parsingStringCsvToFloatArray(String data)
        {
            String[] dirtySplitSample = data.Split("\r\n");
            String[] splitSample = dirtySplitSample.Take(dirtySplitSample.Length - 1).ToArray();

            float[] ecgFloat = new float[splitSample.Length];
            for (int i = 0; i < splitSample.Length; i++)
            {
                ecgFloat[i] = Convert.ToSingle(splitSample[i]);
            }

            return ecgFloat;


        }

        private String floatArrayToString(float[] data)
        {
            string all = string.Join(",", data);
            return all;
        }

        private byte[] stringToByteArray(String data)
        {
            byte[] StrByte = Encoding.UTF8.GetBytes(data);
            return StrByte;
        }



    }
}
