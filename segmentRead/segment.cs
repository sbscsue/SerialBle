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
        private const int sendLength = 80;

        String folderPath;
        String fileName;
        private String dataString;
        private float[] dataFloat;

        int flag = 0;
        
        public segment(String folderPath,String fileName)
        {
            this.folderPath = folderPath;
            this.fileName = fileName;
            flag = 0;
        }


        public Boolean getFileOpenState()
        {
            if (dataString != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int getflagNumber()
        {
            return flag;
        }

        public async Task setFile()
        {
            var folder = await StorageFolder.GetFolderFromPathAsync(folderPath);
            var file = await folder.GetFileAsync(fileName);
            dataString = await FileIO.ReadTextAsync(file);
            dataFloat = parsingStringCsvToFloatArray(dataString);
        }

        public byte[] popData()
        {
            byte[] data = stringToByteArray(floatArrayToString(dataFloat, flag * sendLength, sendLength));
   
            flag += 1;
            return data;
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
        private String floatArrayToString(float[] data, int start, int length)
        {
            ArraySegment<float> ecgSeg = new ArraySegment<float>(dataFloat, start, length);
            string all = string.Join(",", ecgSeg.ToArray());
            return all;
        }

        private byte[] stringToByteArray(String data)
        {
            byte[] StrByte = Encoding.UTF8.GetBytes(data);
            return StrByte;
        }



    }
}
