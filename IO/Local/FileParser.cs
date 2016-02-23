using System.Collections.Generic;
using System.IO;

namespace JLib.IO.Net
{
    public class FileParser
    {
        /// <summary>
        /// A convenience function for cleanly retrieving the rows of a file in a List<string> format.
        /// </summary>
        /// <param name="URL">The complete local address.</param>
        protected List<string> RetrieveFileRows(string URL)
        {
            List<string> returnList = new List<string>();

            using (StreamReader reader = new StreamReader(new FileStream(URL, FileMode.Open)))
            {
                while (reader.EndOfStream == false)
                {
                    returnList.Add(reader.ReadLine());
                }
            }

            return returnList;
        }
    }
}
